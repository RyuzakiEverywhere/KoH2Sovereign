using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000332 RID: 818
[ExecuteInEditMode]
public class ShapeSnapshotTool : MonoBehaviour
{
	// Token: 0x06003234 RID: 12852 RVA: 0x00196BF8 File Offset: 0x00194DF8
	public static ShapeSnapshot TakeSnapshot(Terrain terrain, Bounds bounds, Vector3 worldPosition)
	{
		TerrainData terrainData = terrain.terrainData;
		ShapeSnapshot shapeSnapshot = new ShapeSnapshot(bounds, worldPosition);
		Vector2Int resolution = BSGTerrainTools.HeightsResolution(terrainData);
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		BSGTerrainTools.CalcGridBounds(bounds, terrainData.bounds, resolution, out vector2Int, out vector2Int2);
		BSGTerrainTools.Float2D float2D = BSGTerrainTools.Float2D.ImportHeights(terrainData, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
		shapeSnapshot.SetHeights(float2D.ToArray());
		resolution = BSGTerrainTools.SplatsResolution(terrainData);
		BSGTerrainTools.CalcGridBounds(bounds, terrainData.bounds, resolution, out vector2Int, out vector2Int2);
		BSGTerrainTools.Float3D float3D = new BSGTerrainTools.Float3D(terrainData.GetAlphamaps(vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y));
		shapeSnapshot.SetSplats(float3D.ToArray(null));
		List<TreeInstance> all_trees = new List<TreeInstance>(terrainData.treeInstances);
		List<TreeInstance> list = new List<TreeInstance>();
		BSGTerrainTools.Float2D float2D2 = new BSGTerrainTools.Float2D(float3D.layers[0].arr);
		float2D2.SetWorldRect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
		BSGTerrainTools.Gen2D trees_alphas_gen = new BSGTerrainTools.Gen2D(float2D2);
		trees_alphas_gen.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			return 1f - trees_alphas_gen.f2d.GetWorld(wx, wy);
		};
		BSGTerrainTools.ExtractTrees(all_trees, terrainData.bounds, bounds, list, trees_alphas_gen, 0f);
		for (int i = 0; i < list.Count; i++)
		{
			TreeInstance treeInstance = list[i];
			float num = treeInstance.position.x * terrainData.size.x - bounds.min.x;
			treeInstance.position.x = num / bounds.size.x;
			float num2 = treeInstance.position.z * terrainData.size.z - bounds.min.z;
			treeInstance.position.z = num2 / bounds.size.z;
			shapeSnapshot.trees.Add(TreeData.Create(treeInstance));
		}
		shapeSnapshot.objects = ShapeSnapshotTool.GetShotObjects(shapeSnapshot, terrain);
		return shapeSnapshot;
	}

	// Token: 0x06003235 RID: 12853 RVA: 0x00196E20 File Offset: 0x00195020
	public static void PasteSnapshot(ShapeSnapshot shot, Terrain terrain, Bounds bounds)
	{
		if (terrain == null || shot == null)
		{
			return;
		}
		TerrainData terrainData = terrain.terrainData;
		BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(shot.GetHeights());
		BSGTerrainTools.Float3D float3D = new BSGTerrainTools.Float3D(shot.GetSplats());
		Bounds bounds2 = BSGTerrainTools.SnapHeightsBounds(bounds, terrainData);
		float2D.SetWorldRect(bounds2.min.x, bounds2.min.z, bounds2.size.x, bounds2.size.z);
		Bounds bounds3 = BSGTerrainTools.SnapSplatsBounds(bounds, terrainData);
		float3D.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(terrainData, bounds);
		terrainBlock.Import(true, true, true);
		for (int i = 0; i < float2D.arr_height; i++)
		{
			for (int j = 0; j < float2D.arr_width; j++)
			{
				float num = 1f - float3D.layers[0].arr[i, j];
				if (num > 0f)
				{
					float num2 = terrainBlock.heights[i, j];
					float num3 = float2D.arr[i, j];
					float num4 = (1f - num) * num2 + num * num3;
					terrainBlock.heights[i, j] = num4;
					int num5 = 0;
					while (num5 < terrainBlock.splats.GetLength(2) || num5 < float3D.layers.Length)
					{
						num2 = terrainBlock.splats[i, j, num5];
						num3 = float3D.layers[num5].arr[i, j];
						num4 = (1f - num) * num2 + num * num3;
						terrainBlock.splats[i, j, num5] = num4;
						num5++;
					}
					terrainBlock.splats[i, j, 0] = 0f;
					BSGTerrainTools.NormalizeSplatAlphas(terrainBlock.splats, j, i);
				}
			}
		}
		terrainBlock.Apply(true, true, false);
		BSGTerrainTools.Float2D float2D2 = new BSGTerrainTools.Float2D(float3D.layers[0].arr);
		float2D2.SetWorldRect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
		BSGTerrainTools.Gen2D trees_alphas_gen = new BSGTerrainTools.Gen2D(float2D2);
		trees_alphas_gen.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			return 1f - trees_alphas_gen.f2d.GetWorld(wx, wy);
		};
		List<TreeInstance> list = new List<TreeInstance>(terrainBlock.td.treeInstances);
		BSGTerrainTools.ExtractTrees(list, terrainBlock.td.bounds, bounds, null, trees_alphas_gen, 0.4f);
		if (shot.trees != null)
		{
			for (int k = 0; k < shot.trees.Count; k++)
			{
				TreeData treeData = shot.trees[k];
				float num6 = treeData.position.x * bounds.size.x + bounds.min.x;
				treeData.position.x = num6 / terrainData.bounds.size.x;
				float num7 = treeData.position.z * bounds.size.z + bounds.min.z;
				treeData.position.z = num7 / terrainData.bounds.size.z;
				treeData.position.y = ShapeSnapshotTool.GetTerrainHeight01(new Vector3(num6, 0f, num7) + terrain.GetPosition(), terrain);
				shot.trees[k] = treeData;
				list.Add(shot.trees[k].ToTreeInstance());
			}
		}
		terrainBlock.trees = list;
		terrainBlock.Apply(false, false, true);
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x000023FD File Offset: 0x000005FD
	public static void CreatePrefab(string path, ShapeSnapshot shot)
	{
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x00197224 File Offset: 0x00195424
	public static List<GameObject> GetShotObjects(ShapeSnapshot shot, Terrain terrain)
	{
		Vector3 position = terrain.GetPosition();
		Vector2Int gridCor = new Vector2Int(Mathf.RoundToInt(shot.bounds.min.x), Mathf.RoundToInt(shot.bounds.min.z));
		Vector3 vector = ShapeSnapshotTool.TerrainToWorldPoint(terrain.terrainData, gridCor, position);
		Vector3 vector2 = vector + new Vector3(shot.bounds.max.x, 0f, shot.bounds.max.z);
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!ShapeSnapshotTool.IgnoreObj(array[i].transform) && array[i].transform.position.x >= vector.x && array[i].transform.position.x <= vector2.x && array[i].transform.position.z >= vector.z && array[i].transform.position.z <= vector2.z)
			{
				list.Add(array[i]);
			}
		}
		return list;
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x00197360 File Offset: 0x00195560
	private static bool IgnoreObj(Transform t)
	{
		if ((t.gameObject.hideFlags & HideFlags.DontSave) != HideFlags.None)
		{
			return true;
		}
		for (int i = 0; i < ShapeSnapshotTool.IgnoreAnyObjectsWithComponents.Length; i++)
		{
			Type type = ShapeSnapshotTool.IgnoreAnyObjectsWithComponents[i];
			if (t.GetComponent(type) != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x001973AC File Offset: 0x001955AC
	public static Vector2Int WorldToTerrainPoint(TerrainData td, Vector3 worldCor, Vector3 terrainPos)
	{
		Vector2Int result = default(Vector2Int);
		result.x = Mathf.RoundToInt((worldCor.x - terrainPos.x) / td.size.x * (float)td.heightmapResolution);
		result.y = Mathf.RoundToInt((worldCor.z - terrainPos.z) / td.size.z * (float)td.heightmapResolution);
		result.x = Mathf.Clamp(result.x, 0, td.heightmapResolution);
		result.y = Mathf.Clamp(result.y, 0, td.heightmapResolution);
		return result;
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00197450 File Offset: 0x00195650
	public static Vector3 TerrainToWorldPoint(TerrainData td, Vector2Int gridCor, Vector3 terrainPos)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = terrainPos.x + vector.x * td.size.x / (float)td.heightmapResolution;
		vector.z = terrainPos.z + vector.y * td.size.z / (float)td.heightmapResolution;
		return vector;
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x001974C4 File Offset: 0x001956C4
	public static float GetTerrainHeight(Vector3 wpos, Terrain terrain)
	{
		Vector3 vector = wpos - terrain.GetPosition();
		Vector2Int vector2Int = new Vector2Int(terrain.terrainData.heightmapResolution - 1, terrain.terrainData.heightmapResolution - 1);
		int xBase = Mathf.FloorToInt(vector.x * (float)vector2Int.x / terrain.terrainData.size.x);
		int yBase = Mathf.FloorToInt(vector.z * (float)vector2Int.y / terrain.terrainData.size.z);
		return terrain.terrainData.GetHeights(xBase, yBase, 1, 1)[0, 0] * terrain.terrainData.size.y;
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x00197570 File Offset: 0x00195770
	public static float GetTerrainHeight01(Vector3 wpos, Terrain terrain)
	{
		Vector3 vector = wpos - terrain.GetPosition();
		Vector2Int vector2Int = new Vector2Int(terrain.terrainData.heightmapResolution - 1, terrain.terrainData.heightmapResolution - 1);
		int xBase = Mathf.FloorToInt(vector.x * (float)vector2Int.x / terrain.terrainData.size.x);
		int yBase = Mathf.FloorToInt(vector.z * (float)vector2Int.y / terrain.terrainData.size.z);
		return terrain.terrainData.GetHeights(xBase, yBase, 1, 1)[0, 0];
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x0019760C File Offset: 0x0019580C
	private void OnDrawGizmos()
	{
		if (this.terrain == null)
		{
			return;
		}
		TerrainData terrainData = this.terrain.terrainData;
		if (terrainData == null)
		{
			return;
		}
		base.transform.position.y = terrainData.size.y / 2f;
		Vector3 size = new Vector3(this.width, terrainData.size.y, this.height);
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(base.transform.position, size);
	}

	// Token: 0x040021C6 RID: 8646
	public float width = 128f;

	// Token: 0x040021C7 RID: 8647
	public float height = 128f;

	// Token: 0x040021C8 RID: 8648
	public Terrain terrain;

	// Token: 0x040021C9 RID: 8649
	public string savePath;

	// Token: 0x040021CA RID: 8650
	public static ShapeSnapshot testShot;

	// Token: 0x040021CB RID: 8651
	private static Type[] IgnoreAnyObjectsWithComponents = new Type[]
	{
		typeof(Camera),
		typeof(Light),
		typeof(Terrain),
		typeof(Defs),
		typeof(BSGTerrainEdit),
		typeof(WorldMap),
		typeof(ShapeSnapshotTool)
	};
}
