using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class DirtRoadGraph
{
	// Token: 0x060008B0 RID: 2224 RVA: 0x0005DFDC File Offset: 0x0005C1DC
	public DirtRoadGraph(VoronoiGraph voronoi, Bounds terrain_bounds)
	{
		this.voronoi_graph = voronoi;
		VoronoiGraph.GenerationParameters parameters = voronoi.parameters;
		this.voronoi_to_world = Matrix4x4.identity;
		this.voronoi_to_world = Matrix4x4.Translate(new Vector3((float)(-(float)parameters.resolution.x), (float)(-(float)parameters.resolution.y), 0f) * 0.5f) * this.voronoi_to_world;
		this.voronoi_to_world = Matrix4x4.Scale(new Vector3(1f / (float)parameters.resolution.x, 1f / (float)parameters.resolution.y, 0f)) * this.voronoi_to_world;
		this.voronoi_to_world = Matrix4x4.Rotate(Quaternion.Euler(90f, 0f, 0f)) * this.voronoi_to_world;
		this.voronoi_to_world = Matrix4x4.Scale(terrain_bounds.size) * this.voronoi_to_world;
		this.voronoi_to_world = Matrix4x4.Translate(terrain_bounds.center) * this.voronoi_to_world;
		Dictionary<VoronoiGraph.Edge, DirtRoadGraph.Edge> dictionary = new Dictionary<VoronoiGraph.Edge, DirtRoadGraph.Edge>();
		Dictionary<VoronoiGraph.Vertex, DirtRoadGraph.Vertex> dictionary2 = new Dictionary<VoronoiGraph.Vertex, DirtRoadGraph.Vertex>();
		this.ConvertVoronoiVertex(voronoi.vertices.First<VoronoiGraph.Vertex>(), dictionary, dictionary2);
		foreach (KeyValuePair<VoronoiGraph.Edge, DirtRoadGraph.Edge> keyValuePair in dictionary)
		{
			this.edges.Add(keyValuePair.Value);
		}
		foreach (KeyValuePair<VoronoiGraph.Vertex, DirtRoadGraph.Vertex> keyValuePair2 in dictionary2)
		{
			this.vertices.Add(keyValuePair2.Value);
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0005E1C4 File Offset: 0x0005C3C4
	private DirtRoadGraph.Vertex ConvertVoronoiVertex(VoronoiGraph.Vertex voronoiVertex, Dictionary<VoronoiGraph.Edge, DirtRoadGraph.Edge> edge_map, Dictionary<VoronoiGraph.Vertex, DirtRoadGraph.Vertex> vertex_map)
	{
		DirtRoadGraph.Vertex result;
		if (vertex_map.TryGetValue(voronoiVertex, out result))
		{
			return result;
		}
		DirtRoadGraph.Vertex vertex = new DirtRoadGraph.Vertex();
		vertex_map.Add(voronoiVertex, vertex);
		vertex.position = Common.SnapToTerrain(this.voronoi_to_world.MultiplyPoint(new float3(voronoiVertex.position.xy, 0f)), 0f, null, -1f, false);
		vertex.edges = (from e in voronoiVertex.edges
		select this.ConvertVoronoiEdge(e, edge_map, vertex_map)).ToList<DirtRoadGraph.Edge>();
		return vertex;
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0005E278 File Offset: 0x0005C478
	private DirtRoadGraph.Edge ConvertVoronoiEdge(VoronoiGraph.Edge voronoiEdge, Dictionary<VoronoiGraph.Edge, DirtRoadGraph.Edge> edge_map, Dictionary<VoronoiGraph.Vertex, DirtRoadGraph.Vertex> vertex_map)
	{
		DirtRoadGraph.Edge result;
		if (edge_map.TryGetValue(voronoiEdge, out result))
		{
			return result;
		}
		DirtRoadGraph.Edge edge = new DirtRoadGraph.Edge();
		edge_map.Add(voronoiEdge, edge);
		edge.end1 = this.ConvertVoronoiVertex(voronoiEdge.end1, edge_map, vertex_map);
		edge.end2 = this.ConvertVoronoiVertex(voronoiEdge.end2, edge_map, vertex_map);
		return edge;
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0005E2CC File Offset: 0x0005C4CC
	public void IntersectWithSDF(Func<float3, float> sdf)
	{
		foreach (DirtRoadGraph.Edge edge in this.edges)
		{
			if (sdf(edge.end1.position) > 0f)
			{
				edge.end1.state = DirtRoadGraph.GraphElementState.Discarded;
			}
			if (sdf(edge.end2.position) > 0f)
			{
				edge.end2.state = DirtRoadGraph.GraphElementState.Discarded;
			}
			if (edge.end1.state == DirtRoadGraph.GraphElementState.Discarded && edge.end2.state == DirtRoadGraph.GraphElementState.Discarded)
			{
				edge.state = DirtRoadGraph.GraphElementState.Discarded;
				edge.end1.edges.Remove(edge);
				edge.end2.edges.Remove(edge);
				edge.end1 = null;
				edge.end2 = null;
			}
		}
		for (int i = this.edges.Count - 1; i >= 0; i--)
		{
			if (this.edges[i].state == DirtRoadGraph.GraphElementState.Discarded)
			{
				this.edges.RemoveAt(i);
			}
		}
		for (int j = this.vertices.Count - 1; j >= 0; j--)
		{
			if (this.vertices[j].state == DirtRoadGraph.GraphElementState.Discarded)
			{
				this.vertices.RemoveAt(j);
			}
		}
		for (int k = 0; k < this.edges.Count; k++)
		{
			DirtRoadGraph.Edge edge2 = this.edges[k];
			if (edge2.end1.state != edge2.end2.state)
			{
				float3 position = SDFUtils.FindClosestSurfacePointBetween(sdf, edge2.end1.position, edge2.end2.position, 15);
				DirtRoadGraph.Vertex vertex = new DirtRoadGraph.Vertex
				{
					position = position,
					edges = new List<DirtRoadGraph.Edge>
					{
						edge2
					}
				};
				if (edge2.end1.state == DirtRoadGraph.GraphElementState.Discarded)
				{
					edge2.end1 = vertex;
				}
				else
				{
					edge2.end2 = vertex;
				}
			}
		}
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0005E4DC File Offset: 0x0005C6DC
	public void SnapVertivesToSDFSurface(Func<float3, float> sdf, float snap_distance)
	{
		foreach (DirtRoadGraph.Vertex vertex in this.vertices)
		{
			if (sdf(vertex.position) < snap_distance)
			{
				vertex.position = SDFUtils.SnapToSDF(sdf, vertex.position);
			}
		}
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0005E54C File Offset: 0x0005C74C
	public void RemoveEdgesNearSDFObject(Func<float3, float> sdf, float min_end_distance, float min_middle_distance)
	{
		for (int i = this.edges.Count - 1; i >= 0; i--)
		{
			DirtRoadGraph.Edge edge = this.edges[i];
			float num = sdf(edge.end1.position) - min_end_distance;
			float num2 = sdf((edge.end1.position + edge.end2.position) * 0.5f) - min_middle_distance;
			float num3 = sdf(edge.end2.position) - min_end_distance;
			if ((num < 0f && num2 < 0f && num3 < 0f) || num2 < 0f)
			{
				this.RemoveEdge(edge);
			}
		}
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0005E600 File Offset: 0x0005C800
	private void RemoveEdge(DirtRoadGraph.Edge edge)
	{
		edge.state = DirtRoadGraph.GraphElementState.Discarded;
		edge.end1.edges.Remove(edge);
		edge.end2.edges.Remove(edge);
		if (edge.end1.edges.Count == 0)
		{
			this.vertices.Remove(edge.end1);
		}
		if (edge.end2.edges.Count == 0)
		{
			this.vertices.Remove(edge.end2);
		}
		this.edges.Remove(edge);
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0005E690 File Offset: 0x0005C890
	public IEnumerable<DirtRoadGraph.RoadPath> GenerateRoadPaths(Func<float3, float> distortion_strength_func, float averageStepDistance = 6f, float distortion_strength = 0.8f, float distortion_scale = 3f)
	{
		List<DirtRoadGraph.RoadPath> list = new List<DirtRoadGraph.RoadPath>();
		foreach (DirtRoadGraph.Edge edge in this.edges)
		{
			int num = (int)math.round(edge.Length / averageStepDistance);
			num = math.max(3, num);
			DirtRoadGraph.RoadPath roadPath = new DirtRoadGraph.RoadPath();
			for (int i = 0; i < num; i++)
			{
				roadPath.points.Add(Common.SnapToTerrain(math.lerp(edge.end1.position, edge.end2.position, (float)i / (float)(num - 1)), 0f, null, -1f, false));
			}
			list.Add(roadPath);
		}
		foreach (DirtRoadGraph.RoadPath roadPath2 in list)
		{
			for (int j = 0; j < roadPath2.points.Count; j++)
			{
				float distortion_strength2 = distortion_strength * math.clamp(distortion_strength_func(roadPath2.points[j]), 0f, 1f);
				roadPath2.points[j] = Common.SnapToTerrain(DirtRoadGraph.PointDistortion(roadPath2.points[j], distortion_strength2, distortion_scale), 0f, null, -1f, false);
			}
		}
		return list;
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0005E82C File Offset: 0x0005CA2C
	public void RemoveEdgesThatIntersectSDFObject(Func<float3, float> sdf, float margin)
	{
		for (int i = this.edges.Count - 1; i >= 0; i--)
		{
			DirtRoadGraph.Edge edge = this.edges[i];
			for (int j = 0; j <= 10; j++)
			{
				float s = (float)j / 10f;
				float3 arg = math.lerp(edge.end1.position, edge.end2.position, s);
				if (sdf(arg) < margin)
				{
					this.RemoveEdge(edge);
					break;
				}
			}
		}
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0005E8A8 File Offset: 0x0005CAA8
	public static float3 PointDistortion(float3 point, float distortion_strength, float distortion_scale)
	{
		float rhs = distortion_scale * 0.01f;
		point *= rhs;
		float3 x = point;
		float num = 3f;
		float num2 = 6f;
		float x2 = 1.3f;
		float num3 = 0.25f;
		for (float num4 = num; num4 <= num + num2; num4 += 1f)
		{
			point.x += num3 * math.sin(point.z * math.pow(x2, num4) + num4 * 0.18f) / math.pow(x2, num4);
			point.z += num3 * math.sin(point.x * math.pow(x2, num4) + num4 * 0.21f) / math.pow(x2, num4);
		}
		return math.lerp(x, point, distortion_strength) / rhs;
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0005E974 File Offset: 0x0005CB74
	public static float2 PointDistortion(float2 point, float distortion_strength, float distortion_scale)
	{
		float rhs = distortion_scale * 0.01f;
		point *= rhs;
		float2 x = point;
		float num = 3f;
		float num2 = 6f;
		float x2 = 1.3f;
		float num3 = 0.25f;
		for (float num4 = num; num4 <= num + num2; num4 += 1f)
		{
			point.x += num3 * math.sin(point.y * math.pow(x2, num4) + num4 * 0.18f) / math.pow(x2, num4);
			point.y += num3 * math.sin(point.x * math.pow(x2, num4) + num4 * 0.21f) / math.pow(x2, num4);
		}
		return math.lerp(x, point, distortion_strength) / rhs;
	}

	// Token: 0x040006D7 RID: 1751
	public List<DirtRoadGraph.Vertex> vertices = new List<DirtRoadGraph.Vertex>();

	// Token: 0x040006D8 RID: 1752
	public List<DirtRoadGraph.Edge> edges = new List<DirtRoadGraph.Edge>();

	// Token: 0x040006D9 RID: 1753
	public Matrix4x4 voronoi_to_world;

	// Token: 0x040006DA RID: 1754
	public VoronoiGraph voronoi_graph;

	// Token: 0x020005A4 RID: 1444
	public enum GraphElementState
	{
		// Token: 0x04003121 RID: 12577
		Ok,
		// Token: 0x04003122 RID: 12578
		Discarded
	}

	// Token: 0x020005A5 RID: 1445
	public class Edge
	{
		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x0600447A RID: 17530 RVA: 0x002020F1 File Offset: 0x002002F1
		public float Length
		{
			get
			{
				return math.distance(this.end1.position, this.end2.position);
			}
		}

		// Token: 0x04003123 RID: 12579
		public DirtRoadGraph.Vertex end1;

		// Token: 0x04003124 RID: 12580
		public DirtRoadGraph.Vertex end2;

		// Token: 0x04003125 RID: 12581
		public DirtRoadGraph.GraphElementState state;
	}

	// Token: 0x020005A6 RID: 1446
	public class Vertex
	{
		// Token: 0x04003126 RID: 12582
		public float3 position;

		// Token: 0x04003127 RID: 12583
		public List<DirtRoadGraph.Edge> edges = new List<DirtRoadGraph.Edge>();

		// Token: 0x04003128 RID: 12584
		public DirtRoadGraph.GraphElementState state;
	}

	// Token: 0x020005A7 RID: 1447
	public class RoadPath
	{
		// Token: 0x0600447D RID: 17533 RVA: 0x00202124 File Offset: 0x00200324
		public float3 GetNormal()
		{
			float3 lhs = this.points.First<float3>();
			float3 rhs = this.points.Last<float3>();
			float3 @float = lhs - rhs;
			return math.normalize(math.float3(@float.z, 0f, -@float.x));
		}

		// Token: 0x0600447E RID: 17534 RVA: 0x0020216C File Offset: 0x0020036C
		public float GetLength()
		{
			float num = 0f;
			for (int i = 0; i < this.points.Count - 1; i++)
			{
				num += math.distance(this.points[i], this.points[i + 1]);
			}
			return num;
		}

		// Token: 0x04003129 RID: 12585
		public List<float3> points = new List<float3>();
	}
}
