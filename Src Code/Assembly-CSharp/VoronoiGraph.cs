using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class VoronoiGraph
{
	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06001199 RID: 4505 RVA: 0x000B8EA2 File Offset: 0x000B70A2
	public bool IsInitialized
	{
		get
		{
			return this.vertices != null && this.parameters != null && this.cells_grid != null && this.edges != null;
		}
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x000B8EC8 File Offset: 0x000B70C8
	public VoronoiGraph.Cell GetClosestCell(float2 position)
	{
		int2 @int = (int2)math.floor(position);
		@int = math.clamp(@int, 0, this.parameters.resolution - 1);
		float num = 999f;
		VoronoiGraph.Cell result = this.cells_grid[@int.y, @int.x];
		for (int i = -this.parameters.iterations; i <= this.parameters.iterations; i++)
		{
			for (int j = -this.parameters.iterations; j <= this.parameters.iterations; j++)
			{
				int2 int2 = math.clamp(@int + math.int2(j, i), 0, this.parameters.resolution - 1);
				VoronoiGraph.Cell cell = this.cells_grid[int2.y, int2.x];
				float num2 = math.distancesq(cell.center, position);
				if (num2 < num)
				{
					num = num2;
					result = cell;
				}
			}
		}
		return result;
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x000B8FCC File Offset: 0x000B71CC
	public VoronoiGraph.Edge GetClosestEdge(float2 position)
	{
		VoronoiGraph.Cell closestCell = this.GetClosestCell(position);
		float num = 999f;
		VoronoiGraph.Edge result = closestCell.edges[0];
		foreach (VoronoiGraph.Edge edge in closestCell.edges)
		{
			float num2 = new VoronoiGraphUtils.Line2D
			{
				origin = edge.end1.position,
				tangent = math.normalize(edge.end1.position - edge.end2.position)
			}.DistanceTo(position);
			if (num2 < num)
			{
				num = num2;
				result = edge;
			}
		}
		return result;
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x000B908C File Offset: 0x000B728C
	public VoronoiGraph.Vertex GetClosestVertex(float2 position)
	{
		VoronoiGraph.Cell closestCell = this.GetClosestCell(position);
		float num = 999f;
		IEnumerable<VoronoiGraph.Vertex> enumerable = closestCell.GetVertices();
		VoronoiGraph.Vertex result = enumerable.First<VoronoiGraph.Vertex>();
		foreach (VoronoiGraph.Vertex vertex in enumerable)
		{
			float num2 = math.distancesq(position, vertex.position);
			if (num2 < num)
			{
				num = num2;
				result = vertex;
			}
		}
		return result;
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x000B9100 File Offset: 0x000B7300
	private VoronoiGraph()
	{
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x000B9120 File Offset: 0x000B7320
	public static VoronoiGraph Generate(VoronoiGraph.GenerationParameters parameters)
	{
		parameters.Validate();
		VoronoiGraph graph = new VoronoiGraph();
		graph.parameters = parameters;
		graph.cells_grid = new VoronoiGraph.Cell[parameters.resolution.y, parameters.resolution.x];
		Parallel.For(0, parameters.resolution.y, delegate(int y)
		{
			for (int i = 0; i < parameters.resolution.x; i++)
			{
				VoronoiGraph.Cell cell = graph.CreateCell(new int2(i, y));
				graph.cells_grid[y, i] = cell;
			}
		});
		graph.BuildEdgesAndVertices();
		return graph;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x000B91C8 File Offset: 0x000B73C8
	private void BuildEdgesAndVertices()
	{
		VoronoiGraph.Cell[,] array = this.cells_grid;
		int upperBound = array.GetUpperBound(0);
		int upperBound2 = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
			{
				VoronoiGraph.Cell cell = array[i, j];
				this.FixCellReferencesInCellEdges(cell);
			}
		}
		array = this.cells_grid;
		upperBound2 = array.GetUpperBound(0);
		upperBound = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound2; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound; j++)
			{
				VoronoiGraph.Cell cell2 = array[i, j];
				this.FixEdgeReferencesInCell(cell2);
			}
		}
		array = this.cells_grid;
		upperBound = array.GetUpperBound(0);
		upperBound2 = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
			{
				VoronoiGraph.Cell cell3 = array[i, j];
				this.FixVertexReferencesInCell(cell3);
			}
		}
		HashSet<VoronoiGraph.Edge> hashSet = new HashSet<VoronoiGraph.Edge>();
		array = this.cells_grid;
		upperBound2 = array.GetUpperBound(0);
		upperBound = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound2; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound; j++)
			{
				VoronoiGraph.Cell cell4 = array[i, j];
				hashSet.UnionWith(cell4.edges);
			}
		}
		this.edges.Clear();
		this.edges.AddRange(hashSet);
		HashSet<VoronoiGraph.Vertex> hashSet2 = new HashSet<VoronoiGraph.Vertex>();
		for (int k = 0; k < this.edges.Count; k++)
		{
			hashSet2.Add(this.edges[k].end1);
			hashSet2.Add(this.edges[k].end2);
		}
		this.vertices.Clear();
		this.vertices.AddRange(hashSet2);
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x000B93C4 File Offset: 0x000B75C4
	private void FixVertexReferencesInCell(VoronoiGraph.Cell cell)
	{
		VoronoiGraph.<>c__DisplayClass16_0 CS$<>8__locals1 = new VoronoiGraph.<>c__DisplayClass16_0();
		CS$<>8__locals1.all_neighbour_edges = new HashSet<VoronoiGraph.Edge>();
		CS$<>8__locals1.all_neighbour_edges.UnionWith(cell.Neigbourhood.SelectMany((VoronoiGraph.Cell c) => c.edges));
		CS$<>8__locals1.all_neighbour_edges.ToList<VoronoiGraph.Edge>();
		CS$<>8__locals1.merge_distance_sq = this.parameters.merge_distance * this.parameters.merge_distance;
		for (int i = cell.edges.Count - 1; i >= 0; i--)
		{
			VoronoiGraph.Edge edge = cell.edges[i];
			CS$<>8__locals1.<FixVertexReferencesInCell>g__FixEdges|1(edge.end1);
			CS$<>8__locals1.<FixVertexReferencesInCell>g__FixEdges|1(edge.end2);
		}
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x000B9480 File Offset: 0x000B7680
	private void FixEdgeReferencesInCell(VoronoiGraph.Cell cell)
	{
		Predicate<VoronoiGraph.Edge> <>9__0;
		for (int i = 0; i < cell.edges.Count; i++)
		{
			VoronoiGraph.Edge edge = cell.edges[i];
			VoronoiGraph.Cell cell2 = (edge.cell1 == cell) ? edge.cell2 : edge.cell1;
			List<VoronoiGraph.Edge> list = cell2.edges;
			Predicate<VoronoiGraph.Edge> match;
			if ((match = <>9__0) == null)
			{
				match = (<>9__0 = ((VoronoiGraph.Edge e) => e.cell1 == cell || e.cell2 == cell));
			}
			int num = list.FindIndex(match);
			if (num >= 0)
			{
				cell2.edges[num] = edge;
			}
		}
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x000B9528 File Offset: 0x000B7728
	private void FixCellReferencesInCellEdges(VoronoiGraph.Cell cell)
	{
		for (int i = 0; i < cell.edges.Count; i++)
		{
			VoronoiGraph.Edge edge = cell.edges[i];
			if (math.all(edge.cell1.id >= 0 & edge.cell1.id < this.parameters.resolution))
			{
				edge.cell1 = this.cells_grid[edge.cell1.id.y, edge.cell1.id.x];
			}
			if (math.all(edge.cell2.id >= 0 & edge.cell2.id < this.parameters.resolution))
			{
				edge.cell2 = this.cells_grid[edge.cell2.id.y, edge.cell2.id.x];
			}
		}
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x000B9630 File Offset: 0x000B7830
	private VoronoiGraph.Cell CreateCell(int2 cellID)
	{
		VoronoiGraph.Cell cell = new VoronoiGraph.Cell(cellID, this.GetCellCenter(cellID));
		List<VoronoiGraph.Cell> list = new List<VoronoiGraph.Cell>();
		for (int i = -this.parameters.iterations; i <= this.parameters.iterations; i++)
		{
			for (int j = -this.parameters.iterations; j <= this.parameters.iterations; j++)
			{
				int2 @int = cellID + math.int2(j, i);
				if (!math.all(@int == cellID))
				{
					list.Add(new VoronoiGraph.Cell(@int, this.GetCellCenter(@int)));
				}
			}
		}
		VoronoiGraph.Cell cell4 = list.Aggregate(delegate(VoronoiGraph.Cell c1, VoronoiGraph.Cell c2)
		{
			if (math.distance(c1.center, cell.center) >= math.distance(c2.center, cell.center))
			{
				return c2;
			}
			return c1;
		});
		List<VoronoiGraphUtils.CellEdge> list2 = (from neighbour in list
		select new VoronoiGraphUtils.CellEdge
		{
			cell = neighbour,
			line = new VoronoiGraphUtils.Line2D
			{
				origin = (cell.center + neighbour.center) * 0.5f,
				tangent = VoronoiGraphUtils.Rotate90Clockwise(math.normalize(neighbour.center - cell.center))
			}
		}).ToList<VoronoiGraphUtils.CellEdge>();
		List<float2> list3 = new List<float2>();
		List<VoronoiGraph.Cell> list4 = new List<VoronoiGraph.Cell>();
		VoronoiGraph.Cell cell2 = cell4;
		VoronoiGraphUtils.Ray2D ray2D = default(VoronoiGraphUtils.Ray2D);
		ray2D.origin = (cell4.center + cell.center) * 0.5f;
		ray2D.direction = VoronoiGraphUtils.Rotate90Clockwise(math.normalize(cell4.center - cell.center));
		VoronoiGraph.Cell cell3 = cell4;
		bool flag = true;
		for (;;)
		{
			float2 @float = ray2D.origin + math.normalize(ray2D.direction) * 1000f;
			VoronoiGraphUtils.CellEdge cellEdge = list2[0];
			int index = 0;
			for (int k = 0; k < list2.Count; k++)
			{
				VoronoiGraphUtils.CellEdge cellEdge2 = list2[k];
				float2 float2;
				if (cellEdge2.cell != cell2 && cellEdge2.cell != cell3 && VoronoiGraphUtils.Intersect(ray2D, cellEdge2.line, out float2) && math.distancesq(float2, ray2D.origin) < math.distancesq(@float, ray2D.origin))
				{
					@float = float2;
					cellEdge = cellEdge2;
					index = k;
				}
			}
			ray2D.origin = @float;
			ray2D.direction = VoronoiGraphUtils.Rotate90Clockwise(math.normalize(cellEdge.cell.center - cell.center));
			list3.Add(@float);
			list4.Add(cellEdge.cell);
			if (list4.Count >= list2.Count)
			{
				break;
			}
			cell3 = cell2;
			cell2 = cellEdge.cell;
			if (!flag)
			{
				list2.RemoveAt(index);
			}
			flag = false;
			if (cell2 == cell4)
			{
				goto IL_263;
			}
		}
		Debug.LogError("Error during voronoi cell generation - too many edges!");
		IL_263:
		for (int l = 0; l < list3.Count; l++)
		{
			VoronoiGraph.Vertex vertex = new VoronoiGraph.Vertex(list3[l]);
			VoronoiGraph.Vertex vertex2 = new VoronoiGraph.Vertex(list3[(l + 1) % list3.Count]);
			VoronoiGraph.Edge item = new VoronoiGraph.Edge
			{
				cell1 = cell,
				cell2 = list4[l],
				end1 = vertex,
				end2 = vertex2
			};
			vertex.edges.Add(item);
			vertex2.edges.Add(item);
			cell.edges.Add(item);
		}
		return cell;
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x000B994C File Offset: 0x000B7B4C
	private float2 GetCellCenter(int2 cellID)
	{
		float2 lhs = this.GetCellHash(cellID) % 10000 / 10000f - 0.5f;
		return cellID + new float2(0.5f, 0.5f) + lhs * this.parameters.center_offset * (float)this.parameters.iterations;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x000B99C8 File Offset: 0x000B7BC8
	private int2 GetCellHash(int2 cellID)
	{
		int x = this.hash(this.hash(cellID.x + this.parameters.seed) + cellID.y);
		int y = this.hash(this.hash(cellID.y + this.parameters.seed - 87349) + cellID.x);
		return new int2(x, y);
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x000B9A2C File Offset: 0x000B7C2C
	private int hash(int x)
	{
		x = (x >> 16 ^ x) * 73244475;
		x = (x >> 16 ^ x) * 73244475;
		x = (x >> 16 ^ x);
		return x;
	}

	// Token: 0x04000BDE RID: 3038
	public VoronoiGraph.GenerationParameters parameters;

	// Token: 0x04000BDF RID: 3039
	public VoronoiGraph.Cell[,] cells_grid;

	// Token: 0x04000BE0 RID: 3040
	public List<VoronoiGraph.Edge> edges = new List<VoronoiGraph.Edge>();

	// Token: 0x04000BE1 RID: 3041
	public List<VoronoiGraph.Vertex> vertices = new List<VoronoiGraph.Vertex>();

	// Token: 0x02000671 RID: 1649
	public class Cell
	{
		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060047C4 RID: 18372 RVA: 0x002156CC File Offset: 0x002138CC
		public IEnumerable<VoronoiGraph.Cell> Neigbourhood
		{
			get
			{
				HashSet<VoronoiGraph.Cell> hashSet = new HashSet<VoronoiGraph.Cell>();
				foreach (VoronoiGraph.Edge edge in this.edges)
				{
					hashSet.Add(edge.cell1);
					hashSet.Add(edge.cell2);
				}
				return hashSet;
			}
		}

		// Token: 0x060047C5 RID: 18373 RVA: 0x0021573C File Offset: 0x0021393C
		public IEnumerable<VoronoiGraph.Vertex> GetVertices()
		{
			HashSet<VoronoiGraph.Vertex> hashSet = new HashSet<VoronoiGraph.Vertex>();
			hashSet.UnionWith(from e in this.edges
			select e.end1);
			hashSet.UnionWith(from e in this.edges
			select e.end2);
			return hashSet;
		}

		// Token: 0x060047C6 RID: 18374 RVA: 0x002157AE File Offset: 0x002139AE
		public Cell(int2 id, float2 center)
		{
			this.id = id;
			this.center = center;
		}

		// Token: 0x04003596 RID: 13718
		public int2 id;

		// Token: 0x04003597 RID: 13719
		public float2 center;

		// Token: 0x04003598 RID: 13720
		public List<VoronoiGraph.Edge> edges = new List<VoronoiGraph.Edge>();
	}

	// Token: 0x02000672 RID: 1650
	public class Edge
	{
		// Token: 0x04003599 RID: 13721
		public VoronoiGraph.Cell cell1;

		// Token: 0x0400359A RID: 13722
		public VoronoiGraph.Cell cell2;

		// Token: 0x0400359B RID: 13723
		public VoronoiGraph.Vertex end1;

		// Token: 0x0400359C RID: 13724
		public VoronoiGraph.Vertex end2;
	}

	// Token: 0x02000673 RID: 1651
	public class Vertex
	{
		// Token: 0x060047C8 RID: 18376 RVA: 0x002157CF File Offset: 0x002139CF
		public Vertex(float2 position)
		{
			this.position = position;
		}

		// Token: 0x0400359D RID: 13725
		public float2 position;

		// Token: 0x0400359E RID: 13726
		public List<VoronoiGraph.Edge> edges = new List<VoronoiGraph.Edge>();
	}

	// Token: 0x02000674 RID: 1652
	[Serializable]
	public class GenerationParameters
	{
		// Token: 0x060047C9 RID: 18377 RVA: 0x002157EC File Offset: 0x002139EC
		public void Validate()
		{
			this.resolution = math.clamp(this.resolution, 0, 1000);
			this.iterations = math.clamp(this.iterations, 0, 10);
			this.merge_distance = math.clamp(this.merge_distance, 1E-06f, 1f);
			this.center_offset = math.clamp(this.center_offset, 0f, 0.5f);
		}

		// Token: 0x0400359F RID: 13727
		public int2 resolution;

		// Token: 0x040035A0 RID: 13728
		public int iterations;

		// Token: 0x040035A1 RID: 13729
		public int seed;

		// Token: 0x040035A2 RID: 13730
		public float merge_distance = 1E-05f;

		// Token: 0x040035A3 RID: 13731
		public float center_offset = 1f;
	}
}
