using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200013D RID: 317
public static class PathUtils
{
	// Token: 0x060010C1 RID: 4289 RVA: 0x000B2ABC File Offset: 0x000B0CBC
	public static void SmoothPath(Func<float2, float> smooth_strength_func, List<float2> path, int iterations, int spread)
	{
		for (int i = 0; i < iterations; i++)
		{
			List<float2> list = new List<float2>();
			for (int j = 0; j < path.Count; j++)
			{
				int num = math.clamp(j - spread, 0, path.Count - 1);
				int num2 = math.clamp(j + spread, 0, path.Count - 1);
				float2 @float = 0f;
				for (int k = num; k <= num2; k++)
				{
					@float += path[k];
				}
				@float /= (float)(num2 - num + 1);
				float s = math.saturate(smooth_strength_func(path[j]));
				float2 item = math.lerp(path[j], @float, s);
				list.Add(item);
			}
			path.Clear();
			path.AddRange(list);
		}
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x000B2B98 File Offset: 0x000B0D98
	public static void SmoothLoopedPath(Func<float3, float> smooth_strength_func, List<float3> path, int iterations, int spread)
	{
		for (int i = 0; i < iterations; i++)
		{
			List<float3> list = new List<float3>();
			for (int j = 0; j < path.Count; j++)
			{
				float3 @float = 0f;
				for (int k = -spread; k <= spread; k++)
				{
					@float += path[(j + k + path.Count) % path.Count];
				}
				@float /= (float)spread * 2f + 1f;
				float s = math.saturate(smooth_strength_func(path[j]));
				float3 item = math.lerp(path[j], @float, s);
				list.Add(item);
			}
			path.Clear();
			path.AddRange(list);
		}
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x000B2C60 File Offset: 0x000B0E60
	public static void MakePathDense(List<float3> path, float step_size)
	{
		List<float3> list = new List<float3>();
		list.Add(path[0]);
		for (int i = 0; i < path.Count - 1; i++)
		{
			float3 @float = path[i];
			float3 float2 = path[i + 1];
			float num = math.distance(float2, @float);
			int num2 = math.max(2, (int)(num / step_size));
			for (int j = 1; j <= num2; j++)
			{
				list.Add(math.lerp(@float, float2, (float)j / (float)num2));
			}
		}
		path.Clear();
		path.AddRange(list);
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x000B2CF0 File Offset: 0x000B0EF0
	public static void MakePathDense(List<float2> path, float step_size)
	{
		List<float2> list = new List<float2>();
		list.Add(path[0]);
		for (int i = 0; i < path.Count - 1; i++)
		{
			float2 @float = path[i];
			float2 float2 = path[i + 1];
			float num = math.distance(float2, @float);
			int num2 = math.max(2, (int)(num / step_size));
			for (int j = 1; j <= num2; j++)
			{
				list.Add(math.lerp(@float, float2, (float)j / (float)num2));
			}
		}
		path.Clear();
		path.AddRange(list);
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x000B2D80 File Offset: 0x000B0F80
	public static void MakePathAvoidSDF(Func<float3, float> sdf, List<float3> path, float step, float margin, Logic.PathFinding lpf)
	{
		List<float2> list = (from p in path
		select p.xz).ToList<float2>();
		PathUtils.MakePathAvoidSDF((float2 p) => sdf(math.float3(p.x, 0f, p.y)), list, step, margin, lpf, false);
		path.Clear();
		path.AddRange(from p in list
		select math.float3(p.x, 0f, p.y));
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x000B2E0C File Offset: 0x000B100C
	public static void MakePathAvoidSDF(Func<float2, float> sdf, List<float2> path, float step, float margin, Logic.PathFinding lpf, bool looped = false)
	{
		Func<float2, float> expanded_sdf = (float2 p) => sdf(p) - 2f;
		List<int2> list = PathUtils.FindInvalidSegments(path, expanded_sdf);
		if (list.Count == 0)
		{
			return;
		}
		if (looped)
		{
			List<float2> list2 = new List<float2>();
			for (int i = 0; i < path.Count; i++)
			{
				list2.Add(path[(i + 30) % path.Count]);
			}
			path.Clear();
			path.AddRange(list2);
			list = PathUtils.FindInvalidSegments(path, expanded_sdf);
		}
		if (list.Last<int2>().y == path.Count - 1)
		{
			list.RemoveAt(list.Count - 1);
		}
		for (int j = 0; j < list.Count; j++)
		{
			int2 @int = list[j];
			float2 @float = path[@int.x];
			float2 float2 = path[@int.y];
			Path path2 = new Path(null, @float, PathData.PassableArea.Type.All, false);
			path2.use_max_steps_possible = true;
			path2.can_enter_water = false;
			path2.min_radius = margin;
			path2.max_radius = margin;
			path2.Find(float2, 0f, false);
			path2.state = Path.State.Pending;
			lpf.pending.Insert(0, path2);
			while (path2.state == Path.State.Pending)
			{
				lpf.Process(true, false);
			}
			List<float2> list3 = new List<float2>();
			list3.AddRange(from s in path2.segments
			select math.float2(s.pt.pos.x, s.pt.pos.y));
			if (path2.state != Path.State.Failed)
			{
				int num = @int.y - @int.x - 1;
				int count = list3.Count;
				path.RemoveRange(@int.x + 1, num);
				path.InsertRange(@int.x + 1, list3);
				for (int k = 0; k < list.Count; k++)
				{
					List<int2> list4 = list;
					int index = k;
					list4[index] += count - num;
				}
				for (int l = 0; l < 7; l++)
				{
					for (int m = 1; m < path.Count - 1; m++)
					{
						float2 y = (path[m - 1] + path[m] + path[m + 1]) / 3f;
						float x = math.min(math.distance(path[m], @float), math.distance(path[m], float2));
						path[m] = math.lerp(path[m], y, math.smoothstep(30f, 0f, x));
					}
				}
			}
		}
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x000B30D0 File Offset: 0x000B12D0
	public static void ReduceLoopsInLoopedPath(List<float3> path)
	{
		PathUtils.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.path = path;
		for (int i = 0; i < CS$<>8__locals1.path.Count - 1; i++)
		{
			PathUtils.Segment segment = PathUtils.<ReduceLoopsInLoopedPath>g__GetPathSegment|6_0(i, ref CS$<>8__locals1);
			int j = i + 2;
			while (j < CS$<>8__locals1.path.Count - 1)
			{
				PathUtils.Segment other = PathUtils.<ReduceLoopsInLoopedPath>g__GetPathSegment|6_0(j, ref CS$<>8__locals1);
				float2 @float;
				if (segment.IntersectsWith(other, out @float))
				{
					int num = i + 1 + (CS$<>8__locals1.path.Count - j - 1);
					int num2 = j - i;
					if (num > num2)
					{
						CS$<>8__locals1.path.RemoveRange(i + 1, num2);
						break;
					}
					List<float3> list = new List<float3>();
					list.AddRange(CS$<>8__locals1.path.GetRange(i + 1, num2));
					CS$<>8__locals1.path.Clear();
					CS$<>8__locals1.path.AddRange(list);
					break;
				}
				else
				{
					j++;
				}
			}
		}
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x000B31AC File Offset: 0x000B13AC
	private static bool SegmentIntersection(PathUtils.Segment a, PathUtils.Segment b, out float2 intersection)
	{
		float2 @float = a.end1 - a.end2;
		float2 float2 = b.end1 - b.end2;
		float2 float3 = math.float2(-@float.y, @float.x);
		float lhs = math.dot(b.end1 - a.end1, float3) / math.dot(-float3, float2);
		intersection = lhs * float2 + b.end1;
		return (double)math.dot(intersection - a.end1, intersection - a.end2) < 0.0 && (double)math.dot(intersection - b.end1, intersection - b.end2) < 0.0;
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x000B3298 File Offset: 0x000B1498
	private static List<int2> FindInvalidSegments(List<float2> path, Func<float2, float> expanded_sdf)
	{
		List<int2> list = new List<int2>();
		int i = 0;
		while (i < path.Count - 1)
		{
			if (expanded_sdf(path[i]) > 0f && expanded_sdf(path[i + 1]) < 0f)
			{
				int2 item = math.int2(0, 0);
				item.x = i;
				i++;
				while ((double)expanded_sdf(path[i]) < 0.0 && i < path.Count - 1)
				{
					i++;
				}
				item.y = i;
				list.Add(item);
			}
			else
			{
				i++;
			}
		}
		return list;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x000B333C File Offset: 0x000B153C
	[CompilerGenerated]
	internal static PathUtils.Segment <ReduceLoopsInLoopedPath>g__GetPathSegment|6_0(int begin_point, ref PathUtils.<>c__DisplayClass6_0 A_1)
	{
		return new PathUtils.Segment(A_1.path[begin_point % A_1.path.Count].xz, A_1.path[(begin_point + 1) % A_1.path.Count].xz);
	}

	// Token: 0x0200065A RID: 1626
	private struct Segment
	{
		// Token: 0x0600476B RID: 18283 RVA: 0x00213952 File Offset: 0x00211B52
		public Segment(float2 end1, float2 end2)
		{
			this.end1 = end1;
			this.end2 = end2;
		}

		// Token: 0x0600476C RID: 18284 RVA: 0x00213962 File Offset: 0x00211B62
		public bool IntersectsWith(PathUtils.Segment other, out float2 intersection)
		{
			return PathUtils.SegmentIntersection(this, other, out intersection);
		}

		// Token: 0x0400352D RID: 13613
		public float2 end1;

		// Token: 0x0400352E RID: 13614
		public float2 end2;
	}

	// Token: 0x0200065B RID: 1627
	public class AStarPathfinder
	{
		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x0600476D RID: 18285 RVA: 0x00213971 File Offset: 0x00211B71
		// (set) Token: 0x0600476E RID: 18286 RVA: 0x00213979 File Offset: 0x00211B79
		public float grid_cell_size { get; private set; }

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x0600476F RID: 18287 RVA: 0x00213982 File Offset: 0x00211B82
		public IEnumerable<float2> Obstacles
		{
			get
			{
				return this.obstacles.Select(new Func<int2, float2>(this.GridCoordToMapPosition));
			}
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x0021399C File Offset: 0x00211B9C
		public AStarPathfinder(float grid_cell_size, int seed)
		{
			this.grid_cell_size = grid_cell_size;
			this.random = default(Unity.Mathematics.Random);
			this.random.InitState((uint)seed);
		}

		// Token: 0x06004771 RID: 18289 RVA: 0x00213A6D File Offset: 0x00211C6D
		public void AddObstacle(float2 obstacle_position)
		{
			this.obstacles.Add(this.MapPositionToGridCoord(obstacle_position));
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x00213A84 File Offset: 0x00211C84
		public void AddObstacle(float2 obstacle_position, float radius)
		{
			float2 map_pos = obstacle_position - radius;
			float2 map_pos2 = obstacle_position + radius;
			int2 @int = this.MapPositionToGridCoord(map_pos);
			int2 int2 = this.MapPositionToGridCoord(map_pos2);
			for (int i = @int.y; i <= int2.y; i++)
			{
				for (int j = @int.x; j < int2.x; j++)
				{
					this.obstacles.Add(math.int2(j, i));
				}
			}
		}

		// Token: 0x06004773 RID: 18291 RVA: 0x00213AFA File Offset: 0x00211CFA
		public void AddObstacles(IEnumerable<float2> obstacle_positions)
		{
			this.obstacles.UnionWith(obstacle_positions.Select(new Func<float2, int2>(this.MapPositionToGridCoord)));
		}

		// Token: 0x06004774 RID: 18292 RVA: 0x00213B19 File Offset: 0x00211D19
		public void RemoveObstacle(float2 obstacle_position)
		{
			this.obstacles.Remove(this.MapPositionToGridCoord(obstacle_position));
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x00213B30 File Offset: 0x00211D30
		public void RemoveObstacle(float2 obstacle_position, float radius)
		{
			float2 map_pos = obstacle_position - radius;
			float2 map_pos2 = obstacle_position + radius;
			int2 @int = this.MapPositionToGridCoord(map_pos);
			int2 int2 = this.MapPositionToGridCoord(map_pos2);
			for (int i = @int.y; i <= int2.y; i++)
			{
				for (int j = @int.x; j < int2.x; j++)
				{
					this.obstacles.Remove(math.int2(j, i));
				}
			}
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x00213BA6 File Offset: 0x00211DA6
		public void RemoveObstacles(IEnumerable<float2> obstacle_positions)
		{
			this.obstacles.ExceptWith(obstacle_positions.Select(new Func<float2, int2>(this.MapPositionToGridCoord)));
		}

		// Token: 0x06004777 RID: 18295 RVA: 0x00213BC5 File Offset: 0x00211DC5
		public bool IsObstacle(float2 position)
		{
			return this.obstacles.Contains(this.MapPositionToGridCoord(position));
		}

		// Token: 0x06004778 RID: 18296 RVA: 0x00213BDC File Offset: 0x00211DDC
		public bool TryFindPath(float2 start_pos, Func<float2, float> target_sdf_function, ref List<float2> path, bool get_closest = false)
		{
			PathUtils.AStarPathfinder.<>c__DisplayClass22_0 CS$<>8__locals1 = new PathUtils.AStarPathfinder.<>c__DisplayClass22_0();
			CS$<>8__locals1.target_sdf_function = target_sdf_function;
			CS$<>8__locals1.<>4__this = this;
			if (path == null)
			{
				path = new List<float2>();
			}
			path.Clear();
			int2 @int = this.MapPositionToGridCoord(start_pos);
			if (this.obstacles.Contains(@int))
			{
				return false;
			}
			if (this.TryFindPath(@int, new Func<int2, float>(CS$<>8__locals1.<TryFindPath>g__GridDistanceToTarget|0), get_closest))
			{
				path.AddRange(this.output_path.Select(new Func<int2, float2>(this.GridCoordToMapPosition)));
				return true;
			}
			return false;
		}

		// Token: 0x06004779 RID: 18297 RVA: 0x00213C60 File Offset: 0x00211E60
		public bool TryFindPath(float2 start_pos, float2 end_pos, ref List<float2> path, bool get_closest = false)
		{
			PathUtils.AStarPathfinder.<>c__DisplayClass23_0 CS$<>8__locals1 = new PathUtils.AStarPathfinder.<>c__DisplayClass23_0();
			CS$<>8__locals1.<>4__this = this;
			if (path == null)
			{
				path = new List<float2>();
			}
			path.Clear();
			int2 @int = this.MapPositionToGridCoord(start_pos);
			if (this.obstacles.Contains(@int))
			{
				return false;
			}
			CS$<>8__locals1.end_coord = this.MapPositionToGridCoord(end_pos);
			if (this.TryFindPath(@int, new Func<int2, float>(CS$<>8__locals1.<TryFindPath>g__GridDistanceToTarget|0), get_closest))
			{
				path.AddRange(this.output_path.Select(new Func<int2, float2>(this.GridCoordToMapPosition)));
				return true;
			}
			return false;
		}

		// Token: 0x0600477A RID: 18298 RVA: 0x00213CEC File Offset: 0x00211EEC
		public void DebugDrawObstacles()
		{
			foreach (int2 grid_coord in this.obstacles)
			{
				float3 v = global::Common.SnapToTerrain(new float3
				{
					xz = this.GridCoordToMapPosition(grid_coord)
				}, 0f, null, -1f, false);
				Debug.DrawRay(v, Vector3.up * 50f, Color.red, 60f);
			}
		}

		// Token: 0x0600477B RID: 18299 RVA: 0x00213D90 File Offset: 0x00211F90
		private bool TryFindPath(int2 startCoord, Func<int2, float> distance_to_target, bool get_closest = false)
		{
			this.open_nodes.Clear();
			this.closed_nodes.Clear();
			PathUtils.AStarPathfinder.NodeData nodeData = new PathUtils.AStarPathfinder.NodeData(startCoord, 0f, distance_to_target(startCoord), null);
			this.open_nodes.Add(startCoord, nodeData);
			this.output_path.Clear();
			float num = float.MaxValue;
			PathUtils.AStarPathfinder.NodeData nodeData2 = nodeData;
			for (;;)
			{
				if (this.closed_nodes.Count > 50000)
				{
					break;
				}
				PathUtils.AStarPathfinder.NodeData current_node = this.open_nodes.Values.Aggregate(delegate(PathUtils.AStarPathfinder.NodeData n1, PathUtils.AStarPathfinder.NodeData n2)
				{
					if (n1.Cost >= n2.Cost)
					{
						return n2;
					}
					return n1;
				});
				this.open_nodes.Remove(current_node.coord);
				this.closed_nodes.Add(current_node.coord, current_node);
				float num2 = distance_to_target(current_node.coord);
				if (num2 < num)
				{
					num = num2;
					nodeData2 = current_node;
				}
				if (num2 <= 0f)
				{
					goto Block_4;
				}
				for (int i = 0; i < 3; i++)
				{
					this.ShuffleNeighbours();
				}
				this.ForeachNeighbour(current_node.coord, delegate(int2 neighbour_coord)
				{
					if (this.obstacles.Contains(neighbour_coord) || this.closed_nodes.ContainsKey(neighbour_coord))
					{
						return;
					}
					PathUtils.AStarPathfinder.NodeData nodeData5;
					if (this.open_nodes.TryGetValue(neighbour_coord, out nodeData5))
					{
						if (current_node.distance_from_start + 1f < nodeData5.distance_from_start)
						{
							nodeData5.previous = new int2?(current_node.coord);
							nodeData5.distance_from_start = current_node.distance_from_start + math.distance(current_node.coord, neighbour_coord) * this.grid_cell_size * 1f;
						}
						this.open_nodes[neighbour_coord] = nodeData5;
						return;
					}
					PathUtils.AStarPathfinder.NodeData nodeData6 = new PathUtils.AStarPathfinder.NodeData(neighbour_coord, current_node.distance_from_start + math.distance(current_node.coord, neighbour_coord) * this.grid_cell_size * 1f, distance_to_target(neighbour_coord), new int2?(current_node.coord));
					this.open_nodes.Add(nodeData6.coord, nodeData6);
				});
				if (this.open_nodes.Count == 0)
				{
					goto Block_7;
				}
			}
			return false;
			Block_4:
			this.output_path.Clear();
			PathUtils.AStarPathfinder.NodeData nodeData3 = CS$<>8__locals2.current_node;
			while (nodeData3.previous != null)
			{
				this.output_path.Add(nodeData3.coord);
				nodeData3 = this.closed_nodes[nodeData3.previous.Value];
			}
			this.output_path.Add(nodeData3.coord);
			this.output_path.Reverse();
			return true;
			Block_7:
			if (get_closest)
			{
				this.output_path.Clear();
				PathUtils.AStarPathfinder.NodeData nodeData4 = nodeData2;
				while (nodeData4.previous != null)
				{
					this.output_path.Add(nodeData4.coord);
					nodeData4 = this.closed_nodes[nodeData4.previous.Value];
				}
				this.output_path.Add(nodeData4.coord);
				this.output_path.Reverse();
				return true;
			}
			return false;
		}

		// Token: 0x0600477C RID: 18300 RVA: 0x00213FF0 File Offset: 0x002121F0
		private void ForeachNeighbour(int2 coord, Action<int2> action)
		{
			for (int i = 0; i < this.neighbour_offsets.Count; i++)
			{
				action(coord + this.neighbour_offsets[i]);
			}
		}

		// Token: 0x0600477D RID: 18301 RVA: 0x0021402C File Offset: 0x0021222C
		private void ShuffleNeighbours()
		{
			int index = math.clamp(this.random.NextInt(this.neighbour_offsets.Count), 0, this.neighbour_offsets.Count - 1);
			int index2 = math.clamp(this.random.NextInt(this.neighbour_offsets.Count), 0, this.neighbour_offsets.Count - 1);
			int2 value = this.neighbour_offsets[index];
			this.neighbour_offsets[index] = this.neighbour_offsets[index2];
			this.neighbour_offsets[index2] = value;
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x002140BF File Offset: 0x002122BF
		private int2 MapPositionToGridCoord(float2 map_pos)
		{
			return (int2)math.round(map_pos / this.grid_cell_size);
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x002140D7 File Offset: 0x002122D7
		private float2 GridCoordToMapPosition(int2 grid_coord)
		{
			return math.float2(grid_coord) * this.grid_cell_size;
		}

		// Token: 0x04003530 RID: 13616
		private Unity.Mathematics.Random random;

		// Token: 0x04003531 RID: 13617
		private HashSet<int2> obstacles = new HashSet<int2>();

		// Token: 0x04003532 RID: 13618
		private const int MAX_SEARCH_ITERATIONS = 50000;

		// Token: 0x04003533 RID: 13619
		private Dictionary<int2, PathUtils.AStarPathfinder.NodeData> open_nodes = new Dictionary<int2, PathUtils.AStarPathfinder.NodeData>();

		// Token: 0x04003534 RID: 13620
		private Dictionary<int2, PathUtils.AStarPathfinder.NodeData> closed_nodes = new Dictionary<int2, PathUtils.AStarPathfinder.NodeData>();

		// Token: 0x04003535 RID: 13621
		private List<int2> output_path = new List<int2>();

		// Token: 0x04003536 RID: 13622
		private List<int2> neighbour_offsets = new List<int2>
		{
			math.int2(-1, -1),
			math.int2(-1, 0),
			math.int2(-1, 1),
			math.int2(0, -1),
			math.int2(0, 1),
			math.int2(1, -1),
			math.int2(1, 0),
			math.int2(1, 1)
		};

		// Token: 0x020009FC RID: 2556
		private struct NodeData
		{
			// Token: 0x17000735 RID: 1845
			// (get) Token: 0x06005528 RID: 21800 RVA: 0x00248821 File Offset: 0x00246A21
			public float Cost
			{
				get
				{
					return this.distance_from_start + this.distance_to_target;
				}
			}

			// Token: 0x06005529 RID: 21801 RVA: 0x00248830 File Offset: 0x00246A30
			public NodeData(int2 coord, float distanceFromStart, float distanceToTarget, int2? previous)
			{
				this.coord = coord;
				this.distance_from_start = distanceFromStart;
				this.distance_to_target = distanceToTarget;
				this.previous = previous;
			}

			// Token: 0x0600552A RID: 21802 RVA: 0x00248850 File Offset: 0x00246A50
			public override bool Equals(object obj)
			{
				if (obj is PathUtils.AStarPathfinder.NodeData)
				{
					PathUtils.AStarPathfinder.NodeData nodeData = (PathUtils.AStarPathfinder.NodeData)obj;
					return this.coord.Equals(nodeData.coord);
				}
				return false;
			}

			// Token: 0x0600552B RID: 21803 RVA: 0x00248881 File Offset: 0x00246A81
			public override int GetHashCode()
			{
				return this.coord.GetHashCode();
			}

			// Token: 0x04004614 RID: 17940
			public int2 coord;

			// Token: 0x04004615 RID: 17941
			public float distance_from_start;

			// Token: 0x04004616 RID: 17942
			public float distance_to_target;

			// Token: 0x04004617 RID: 17943
			public int2? previous;
		}
	}
}
