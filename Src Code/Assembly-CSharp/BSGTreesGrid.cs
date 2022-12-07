using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class BSGTreesGrid
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000590 RID: 1424 RVA: 0x0003D733 File Offset: 0x0003B933
	public Vector2Int Grid_Size
	{
		get
		{
			return this.grid_size;
		}
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0003D73C File Offset: 0x0003B93C
	public BSGTreesGrid.InstancesPerType GetInstances(int type)
	{
		if (this.instances_per_type == null)
		{
			this.instances_per_type = new Dictionary<int, BSGTreesGrid.InstancesPerType>();
		}
		BSGTreesGrid.InstancesPerType instancesPerType;
		if (this.instances_per_type.TryGetValue(type, out instancesPerType))
		{
			return instancesPerType;
		}
		instancesPerType = new BSGTreesGrid.InstancesPerType(type);
		this.instances_per_type.Add(type, instancesPerType);
		return instancesPerType;
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0003D783 File Offset: 0x0003B983
	public Vector2Int WorldToGrid(Vector3 ptw)
	{
		ptw /= this.tile_size;
		return new Vector2Int((int)ptw.x, (int)ptw.z);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0003D7A6 File Offset: 0x0003B9A6
	public Vector2 GridToWorld(Vector2Int ptg)
	{
		return new Vector3((float)ptg.x + 0.5f, 0f, (float)ptg.y + 0.5f) * this.tile_size;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0003D7E0 File Offset: 0x0003B9E0
	public void ImportTrees(Terrain terrain, float tile_size = 16f)
	{
		TerrainData terrainData = terrain.terrainData;
		if (this.grid != null && this.terrain_trees_count == terrainData.treeInstanceCount)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		this.terrain_trees_count = terrainData.treeInstanceCount;
		this.terrain_size = terrainData.size;
		this.tile_size = tile_size;
		this.grid_size = new Vector2Int((int)(this.terrain_size.x / tile_size), (int)(this.terrain_size.z / tile_size));
		this.grid = new List<BSGTreesGrid.TreeInfo>[this.grid_size.y, this.grid_size.x];
		this.instances_per_type = null;
		this.trees_count = 0;
		for (int i = 0; i < this.terrain_trees_count; i++)
		{
			TreeInstance treeInstance = terrainData.GetTreeInstance(i);
			BSGTreesGrid.TreeInfo ti = new BSGTreesGrid.TreeInfo(treeInstance, this.terrain_size);
			this.AddTree(ti);
		}
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Debug.Log(string.Format("Imported {0} trees for {1}ms", this.terrain_trees_count, elapsedMilliseconds));
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0003D8E0 File Offset: 0x0003BAE0
	public void ApplyToTresBatching(Terrain terrain)
	{
		TreesBatching treesBatching = (terrain != null) ? terrain.GetComponent<TreesBatching>() : null;
		if (treesBatching == null)
		{
			return;
		}
		if (this.instances_per_type == null)
		{
			return;
		}
		foreach (KeyValuePair<int, BSGTreesGrid.InstancesPerType> keyValuePair in this.instances_per_type)
		{
			BSGTreesGrid.InstancesPerType value = keyValuePair.Value;
			if (value.dirty)
			{
				value.dirty = true;
				treesBatching.UpdateTreesData(value.type, value.instances, true);
			}
		}
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0003D978 File Offset: 0x0003BB78
	public void ApplyToUnity(Terrain terrain)
	{
		if (terrain == null || this.instances_per_type == null)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		TreeInstance[] array = new TreeInstance[this.trees_count];
		int num = 0;
		Vector3 size = terrain.terrainData.size;
		Vector3 b = new Vector3(1f / size.x, 1f / size.y, 1f / size.z);
		foreach (KeyValuePair<int, BSGTreesGrid.InstancesPerType> keyValuePair in this.instances_per_type)
		{
			BSGTreesGrid.InstancesPerType value = keyValuePair.Value;
			for (int i = 0; i < value.instances.Length; i++)
			{
				TreesBatching.TreeShaderData treeShaderData = value.instances[i];
				if (treeShaderData.sx != 0f)
				{
					TreeInstance treeInstance = default(TreeInstance);
					treeInstance.prototypeIndex = value.type;
					treeInstance.position = Vector3.Scale(new Vector3(treeShaderData.x, treeShaderData.y, treeShaderData.z), b);
					treeInstance.widthScale = treeShaderData.sx;
					treeInstance.heightScale = treeShaderData.sy;
					treeInstance.rotation = treeShaderData.angle;
					treeInstance.color = Color.white;
					if (num >= array.Length)
					{
						Debug.LogError("Trees buffer overflow");
						break;
					}
					array[num++] = treeInstance;
				}
			}
		}
		if (num != array.Length)
		{
			Debug.LogError("Trees buffer underflow");
		}
		terrain.terrainData.treeInstances = array;
		this.terrain_trees_count = this.trees_count;
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0003DB38 File Offset: 0x0003BD38
	public bool AddTree(BSGTreesGrid.TreeInfo ti)
	{
		Vector3 ptw = new Vector3(ti.data.x, ti.data.y, ti.data.z);
		Vector2Int vector2Int = this.WorldToGrid(ptw);
		if (ti.data.x < 0f || ti.data.z < 0f || vector2Int.x < 0 || vector2Int.y < 0 || vector2Int.x >= this.grid_size.x || vector2Int.y >= this.grid_size.y)
		{
			return false;
		}
		this.GetInstances(ti.type).Add(ref ti);
		List<BSGTreesGrid.TreeInfo> list = this.grid[vector2Int.y, vector2Int.x];
		if (list == null)
		{
			list = new List<BSGTreesGrid.TreeInfo>();
			this.grid[vector2Int.y, vector2Int.x] = list;
		}
		list.Add(ti);
		this.trees_count++;
		return true;
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0003DC40 File Offset: 0x0003BE40
	public void DelTrees(List<BSGTreesGrid.TreeInfo> grid_list, List<BSGTreesGrid.TreeInfo> trees_to_remove)
	{
		int num = 0;
		for (int i = trees_to_remove.Count - 1; i >= 0; i--)
		{
			BSGTreesGrid.TreeInfo treeInfo = trees_to_remove[i];
			this.GetInstances(treeInfo.type).Del(treeInfo.idx);
			grid_list.Remove(treeInfo);
			num++;
			if (grid_list.Count == 0)
			{
				grid_list = null;
			}
		}
		this.trees_count -= num;
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0003DCA6 File Offset: 0x0003BEA6
	public void DelTree(List<BSGTreesGrid.TreeInfo> grid_list, BSGTreesGrid.TreeInfo tree_to_remove)
	{
		this.GetInstances(tree_to_remove.type).Del(tree_to_remove.idx);
		grid_list.Remove(tree_to_remove);
		if (grid_list.Count == 0)
		{
			grid_list = null;
		}
		this.trees_count--;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0003DCE0 File Offset: 0x0003BEE0
	public List<BSGTreesGrid.TreeInfo> GetGrid(int y, int x)
	{
		return this.grid[y, x];
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0003DCF0 File Offset: 0x0003BEF0
	public void PickTree(Vector3 ptw, ref int tree_index)
	{
		Vector2Int vector2Int = this.WorldToGrid(ptw);
		List<BSGTreesGrid.TreeInfo> list = this.grid[vector2Int.y, vector2Int.x];
		if (list == null)
		{
			return;
		}
		float num = 5f;
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 b = new Vector3(list[i].data.x, list[i].data.y, list[i].data.z);
			float sqrMagnitude = (ptw - b).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				tree_index = list[i].type;
				num = sqrMagnitude;
			}
		}
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0003DDA4 File Offset: 0x0003BFA4
	public bool IsTreePositionValid(Vector3 ptw, float range)
	{
		Vector3 ptw2 = new Vector3(ptw.x - range, ptw.y - range, ptw.z - range);
		Vector3 ptw3 = new Vector3(ptw.x + range, ptw.y + range, ptw.z + range);
		Vector2Int vector2Int = this.WorldToGrid(ptw2);
		Vector2Int vector2Int2 = this.WorldToGrid(ptw3);
		float num = range * range;
		for (int i = vector2Int.x; i <= vector2Int2.x; i++)
		{
			for (int j = vector2Int.y; j <= vector2Int2.y; j++)
			{
				if (j >= 0 && i >= 0 && j < this.grid_size.y && i < this.grid_size.x)
				{
					List<BSGTreesGrid.TreeInfo> list = this.grid[j, i];
					if (list != null)
					{
						for (int k = list.Count - 1; k >= 0; k--)
						{
							BSGTreesGrid.TreeInfo treeInfo = list[k];
							Vector3 b = new Vector3(treeInfo.data.x, treeInfo.data.y, treeInfo.data.z);
							if ((ptw - b).sqrMagnitude < num)
							{
								return false;
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0003DEF4 File Offset: 0x0003C0F4
	public void RebuildTreeData(Terrain terrain)
	{
		TreesBatching treesBatching = (terrain != null) ? terrain.GetComponent<TreesBatching>() : null;
		if (treesBatching == null)
		{
			return;
		}
		treesBatching.Rebuild();
	}

	// Token: 0x04000524 RID: 1316
	private Vector3 terrain_size;

	// Token: 0x04000525 RID: 1317
	private Vector2Int grid_size;

	// Token: 0x04000526 RID: 1318
	private float tile_size;

	// Token: 0x04000527 RID: 1319
	private int trees_count;

	// Token: 0x04000528 RID: 1320
	private int terrain_trees_count;

	// Token: 0x04000529 RID: 1321
	private Dictionary<int, BSGTreesGrid.InstancesPerType> instances_per_type;

	// Token: 0x0400052A RID: 1322
	private List<BSGTreesGrid.TreeInfo>[,] grid;

	// Token: 0x02000566 RID: 1382
	public struct TreeInfo
	{
		// Token: 0x060043C2 RID: 17346 RVA: 0x001FE4F8 File Offset: 0x001FC6F8
		public TreeInfo(int type, Vector3 world_pos, float rotation, float width_scale, float height_scale)
		{
			this.type = type;
			this.idx = -1;
			this.data.x = world_pos.x;
			this.data.y = world_pos.y;
			this.data.z = world_pos.z;
			this.data.angle = rotation;
			this.data.sx = width_scale;
			this.data.sy = height_scale;
			this.data.sz = width_scale;
			this.data.next_free_idx = 0f;
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x001FE58C File Offset: 0x001FC78C
		public TreeInfo(TreeInstance ti, Vector3 terrain_size)
		{
			this.type = ti.prototypeIndex;
			this.idx = -1;
			Vector3 vector = Vector3.Scale(ti.position, terrain_size);
			this.data.x = vector.x;
			this.data.y = vector.y;
			this.data.z = vector.z;
			this.data.angle = ti.rotation;
			this.data.sx = ti.widthScale;
			this.data.sy = ti.heightScale;
			this.data.sz = ti.widthScale;
			this.data.next_free_idx = 0f;
		}

		// Token: 0x0400302C RID: 12332
		public int type;

		// Token: 0x0400302D RID: 12333
		public int idx;

		// Token: 0x0400302E RID: 12334
		public TreesBatching.TreeShaderData data;
	}

	// Token: 0x02000567 RID: 1383
	public class InstancesPerType
	{
		// Token: 0x060043C4 RID: 17348 RVA: 0x001FE640 File Offset: 0x001FC840
		public InstancesPerType(int type)
		{
			this.type = type;
			this.dirty = true;
			this.instances = new TreesBatching.TreeShaderData[10000];
			this.first_free = 0;
			this.filled = 0;
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x001FE674 File Offset: 0x001FC874
		public void Add(ref BSGTreesGrid.TreeInfo ti)
		{
			this.dirty = true;
			if (this.first_free > 0)
			{
				ti.idx = this.first_free - 1;
				TreesBatching.TreeShaderData treeShaderData = this.instances[ti.idx];
				this.first_free = (int)treeShaderData.next_free_idx;
			}
			if (this.filled < this.instances.Length)
			{
				int num = this.filled;
				this.filled = num + 1;
				ti.idx = num;
			}
			else
			{
				TreesBatching.TreeShaderData[] array = new TreesBatching.TreeShaderData[this.filled + 10000];
				for (int i = 0; i < this.filled; i++)
				{
					TreesBatching.TreeShaderData treeShaderData2 = this.instances[i];
					array[i] = treeShaderData2;
				}
				int num = this.filled;
				this.filled = num + 1;
				ti.idx = num;
				this.instances = array;
			}
			this.instances[ti.idx] = ti.data;
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x001FE758 File Offset: 0x001FC958
		public void Del(int idx)
		{
			this.dirty = true;
			TreesBatching.TreeShaderData treeShaderData = this.instances[idx];
			treeShaderData.sx = (treeShaderData.sy = (treeShaderData.sz = 0f));
			treeShaderData.next_free_idx = (float)this.first_free;
			this.instances[idx] = treeShaderData;
			this.first_free = idx + 1;
		}

		// Token: 0x0400302F RID: 12335
		public int type;

		// Token: 0x04003030 RID: 12336
		public bool dirty;

		// Token: 0x04003031 RID: 12337
		public TreesBatching.TreeShaderData[] instances;

		// Token: 0x04003032 RID: 12338
		public int filled;

		// Token: 0x04003033 RID: 12339
		public int first_free;

		// Token: 0x04003034 RID: 12340
		private const int PAGE_SIZE = 10000;
	}
}
