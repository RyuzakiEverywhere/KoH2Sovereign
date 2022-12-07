using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class BSGTerrainTools
{
	// Token: 0x060002CE RID: 718 RVA: 0x00026090 File Offset: 0x00024290
	public static Vector2 V2(Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x000260A3 File Offset: 0x000242A3
	public static Vector3 V3(Vector2 v, float y = 0f)
	{
		return new Vector3(v.x, y, v.y);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x000260B7 File Offset: 0x000242B7
	public static Vector2Int HeightsResolution(TerrainData td)
	{
		return new Vector2Int(td.heightmapResolution - 1, td.heightmapResolution - 1);
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x000260CE File Offset: 0x000242CE
	public static Vector2Int SplatsResolution(TerrainData td)
	{
		return new Vector2Int(td.alphamapWidth, td.alphamapHeight);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x000260E1 File Offset: 0x000242E1
	public static float GridToWorld(float size, int resolution, int coord)
	{
		return size * (float)coord / (float)resolution;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000260EA File Offset: 0x000242EA
	public static int WorldToGrid(float size, int resolution, float coord)
	{
		return Mathf.FloorToInt(coord * (float)resolution / size);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000260F8 File Offset: 0x000242F8
	public static Vector2 GridToWorld(Bounds terrain_bounds, Vector2Int resolution, Vector2Int tile)
	{
		return new Vector2(terrain_bounds.min.x + BSGTerrainTools.GridToWorld(terrain_bounds.size.x, resolution.x, tile.x), terrain_bounds.min.z + BSGTerrainTools.GridToWorld(terrain_bounds.size.z, resolution.y, tile.y));
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00026164 File Offset: 0x00024364
	public static Vector2Int WorldToGrid(Bounds terrain_bounds, Vector2Int resolution, Vector2 pt)
	{
		return new Vector2Int(BSGTerrainTools.WorldToGrid(terrain_bounds.size.x, resolution.x, pt.x), BSGTerrainTools.WorldToGrid(terrain_bounds.size.z, resolution.y, pt.y));
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x000261B4 File Offset: 0x000243B4
	public static Vector2 Clamp(Vector3 pt, Bounds bounds)
	{
		if (pt.x < bounds.min.x)
		{
			pt.x = bounds.min.x;
		}
		if (pt.z < bounds.min.z)
		{
			pt.z = bounds.min.z;
		}
		if (pt.x > bounds.max.x)
		{
			pt.x = bounds.max.x;
		}
		if (pt.z > bounds.max.z)
		{
			pt.z = bounds.max.z;
		}
		return BSGTerrainTools.V2(pt);
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00026264 File Offset: 0x00024464
	public static void CalcGridBounds(Bounds bounds, Bounds terrain_bounds, Vector2Int resolution, out Vector2Int tile, out Vector2Int size)
	{
		Vector2 pt = BSGTerrainTools.Clamp(bounds.min, terrain_bounds);
		Vector2 pt2 = BSGTerrainTools.Clamp(bounds.max, terrain_bounds);
		tile = BSGTerrainTools.WorldToGrid(terrain_bounds, resolution, pt);
		size = BSGTerrainTools.WorldToGrid(terrain_bounds, resolution, pt2) - tile;
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000262B8 File Offset: 0x000244B8
	public static Bounds CalcWorldBounds(Bounds terrain_bounds, Vector2Int resolution, Vector2Int tile, Vector2Int size)
	{
		Vector2 vector = BSGTerrainTools.GridToWorld(terrain_bounds, resolution, tile);
		Vector2 vector2 = BSGTerrainTools.GridToWorld(terrain_bounds, resolution, tile + size + Vector2Int.one);
		Bounds result = default(Bounds);
		result.SetMinMax(new Vector3(vector.x, terrain_bounds.min.y, vector.y), new Vector3(vector2.x, terrain_bounds.max.y, vector2.y));
		return result;
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00026334 File Offset: 0x00024534
	public static Bounds SnapHeightsBounds(Bounds bounds, TerrainData td)
	{
		if (td == null)
		{
			return bounds;
		}
		Vector2Int resolution = BSGTerrainTools.HeightsResolution(td);
		Vector2Int tile;
		Vector2Int size;
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
		return BSGTerrainTools.CalcWorldBounds(td.bounds, resolution, tile, size);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00026374 File Offset: 0x00024574
	public static Bounds SnapHeightsBounds(Bounds bounds, BSGTerrainTools.TerrainInfo ti)
	{
		if (ti == null)
		{
			return bounds;
		}
		Vector2Int heights_resolution = ti.heights_resolution;
		Vector2Int tile;
		Vector2Int size;
		BSGTerrainTools.CalcGridBounds(bounds, ti.bounds, heights_resolution, out tile, out size);
		return BSGTerrainTools.CalcWorldBounds(ti.bounds, heights_resolution, tile, size);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x000263AC File Offset: 0x000245AC
	public static Bounds SnapSplatsBounds(Bounds bounds, TerrainData td)
	{
		if (td == null)
		{
			return bounds;
		}
		Vector2Int resolution = BSGTerrainTools.SplatsResolution(td);
		Vector2Int tile;
		Vector2Int size;
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
		return BSGTerrainTools.CalcWorldBounds(td.bounds, resolution, tile, size);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x000263EC File Offset: 0x000245EC
	public static Bounds SnapSplatsBounds(Bounds bounds, BSGTerrainTools.TerrainInfo ti)
	{
		if (ti == null)
		{
			return bounds;
		}
		Vector2Int splats_resolution = ti.splats_resolution;
		Vector2Int tile;
		Vector2Int size;
		BSGTerrainTools.CalcGridBounds(bounds, ti.bounds, splats_resolution, out tile, out size);
		return BSGTerrainTools.CalcWorldBounds(ti.bounds, splats_resolution, tile, size);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00026424 File Offset: 0x00024624
	public static void ModifyHeights(BSGTerrainTools.TerrainBlock block, Bounds bounds, BSGTerrainTools.Gen2D gen_heights, BSGTerrainTools.Gen2D gen_alphas)
	{
		Vector2Int heights_resolution = block.ti.heights_resolution;
		Vector2Int vector2Int;
		Vector2Int b;
		BSGTerrainTools.CalcGridBounds(bounds, block.ti.bounds, heights_resolution, out vector2Int, out b);
		Vector2Int vector2Int2 = vector2Int + b;
		if (vector2Int.x < block.heights_ofs.x)
		{
			vector2Int.x = block.heights_ofs.x;
		}
		if (vector2Int.y < block.heights_ofs.y)
		{
			vector2Int.y = block.heights_ofs.y;
		}
		if (vector2Int2.x >= block.heights_ofs.x + block.heights_size.x)
		{
			vector2Int2.x = block.heights_ofs.x + block.heights_size.x - 1;
		}
		if (vector2Int2.y >= block.heights_ofs.y + block.heights_size.y)
		{
			vector2Int2.y = block.heights_ofs.y + block.heights_size.y - 1;
		}
		Vector2Int tile = vector2Int;
		while (tile.y <= vector2Int2.y)
		{
			tile.x = vector2Int.x;
			int num8;
			while (tile.x <= vector2Int2.x)
			{
				Vector2 vector = BSGTerrainTools.GridToWorld(block.ti.bounds, heights_resolution, tile);
				float num = 1f;
				float num2 = (gen_alphas == null) ? 1f : gen_alphas.Get(vector.x, vector.y, 1f, ref num);
				if (num2 > 0f)
				{
					int num3 = tile.x - block.heights_ofs.x;
					int num4 = tile.y - block.heights_ofs.y;
					float num5 = block.heights[num4, num3];
					float num6 = gen_heights.Get(vector.x, vector.y, num5, ref num2);
					if (num2 > 0f)
					{
						float num7 = (1f - num2) * num5 + num2 * num6;
						block.heights[num4, num3] = num7;
					}
				}
				num8 = tile.x + 1;
				tile.x = num8;
			}
			num8 = tile.y + 1;
			tile.y = num8;
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00026660 File Offset: 0x00024860
	public static void ModifySplats(BSGTerrainTools.TerrainBlock block, Bounds bounds, BSGTerrainTools.Gen3D gen_splats, BSGTerrainTools.Gen2D gen_alphas)
	{
		Vector2Int splats_resolution = block.ti.splats_resolution;
		Vector2Int vector2Int;
		Vector2Int b;
		BSGTerrainTools.CalcGridBounds(bounds, block.ti.bounds, splats_resolution, out vector2Int, out b);
		Vector2Int vector2Int2 = vector2Int + b;
		if (vector2Int.x < block.splats_ofs.x)
		{
			vector2Int.x = block.splats_ofs.x;
		}
		if (vector2Int.y < block.splats_ofs.y)
		{
			vector2Int.y = block.splats_ofs.y;
		}
		if (vector2Int2.x >= block.splats_ofs.x + block.splats_size.x)
		{
			vector2Int2.x = block.splats_ofs.x + block.splats_size.x - 1;
		}
		if (vector2Int2.y >= block.splats_ofs.y + block.splats_size.y)
		{
			vector2Int2.y = block.splats_ofs.y + block.splats_size.y - 1;
		}
		int length = block.splats.GetLength(2);
		Vector2Int tile = vector2Int;
		while (tile.y <= vector2Int2.y)
		{
			tile.x = vector2Int.x;
			int num8;
			while (tile.x <= vector2Int2.x)
			{
				Vector2 vector = BSGTerrainTools.GridToWorld(block.ti.bounds, splats_resolution, tile);
				float num = 1f;
				float num2 = (gen_alphas == null) ? 1f : gen_alphas.Get(vector.x, vector.y, 1f, ref num);
				if (num2 > 0f)
				{
					int num3 = tile.x - block.splats_ofs.x;
					int num4 = tile.y - block.splats_ofs.y;
					for (int i = 0; i < length; i++)
					{
						float num5 = block.splats[num4, num3, i];
						num = num2;
						float num6 = gen_splats.Get(vector.x, vector.y, i, num5, ref num);
						if (num > 0f)
						{
							float num7 = (1f - num) * num5 + num * num6;
							if (num7 < 0.1f)
							{
								num7 = 0f;
							}
							block.splats[num4, num3, i] = num7;
						}
					}
					BSGTerrainTools.NormalizeSplatAlphas(block.splats, num3, num4);
				}
				num8 = tile.x + 1;
				tile.x = num8;
			}
			num8 = tile.y + 1;
			tile.y = num8;
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000268E4 File Offset: 0x00024AE4
	public static void NormalizeSplatAlphas(float[,,] splats, int x, int y)
	{
		int length = splats.GetLength(2);
		float num = 0f;
		for (int i = 0; i < length; i++)
		{
			num += splats[y, x, i];
		}
		if (num == 1f)
		{
			return;
		}
		if (num == 0f)
		{
			splats[y, x, 0] = 1f;
			return;
		}
		float num2 = 1f / num;
		for (int j = 0; j < length; j++)
		{
			splats[y, x, j] *= num2;
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000023FD File Offset: 0x000005FD
	public static void RebuildBasemap(TerrainData td)
	{
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00026960 File Offset: 0x00024B60
	public static void SaveTree(BinaryWriter writer, TreeInstance ti)
	{
		writer.Write(ti.position.x);
		writer.Write(ti.position.y);
		writer.Write(ti.position.z);
		writer.Write(ti.widthScale);
		writer.Write(ti.heightScale);
		writer.Write(ti.rotation);
		writer.Write(ti.color.r);
		writer.Write(ti.color.g);
		writer.Write(ti.color.b);
		writer.Write(ti.color.a);
		writer.Write(ti.lightmapColor.r);
		writer.Write(ti.lightmapColor.g);
		writer.Write(ti.lightmapColor.b);
		writer.Write(ti.lightmapColor.a);
		writer.Write(ti.prototypeIndex);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00026A58 File Offset: 0x00024C58
	public static TreeInstance LoadTree(BinaryReader reader)
	{
		TreeInstance result = default(TreeInstance);
		result.position = default(Vector3);
		result.position.x = reader.ReadSingle();
		result.position.y = reader.ReadSingle();
		result.position.z = reader.ReadSingle();
		result.widthScale = reader.ReadSingle();
		result.heightScale = reader.ReadSingle();
		result.rotation = reader.ReadSingle();
		result.color = default(Color32);
		result.color.r = reader.ReadByte();
		result.color.g = reader.ReadByte();
		result.color.b = reader.ReadByte();
		result.color.a = reader.ReadByte();
		result.lightmapColor = default(Color32);
		result.lightmapColor.r = reader.ReadByte();
		result.lightmapColor.g = reader.ReadByte();
		result.lightmapColor.b = reader.ReadByte();
		result.lightmapColor.a = reader.ReadByte();
		result.prototypeIndex = reader.ReadInt32();
		return result;
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00026B90 File Offset: 0x00024D90
	public static int ExtractTrees(List<TreeInstance> all_trees, Bounds terrain_bounds, Bounds bounds, List<TreeInstance> to = null, BSGTerrainTools.Gen2D gen_alphas = null, float min_alpha = 1f)
	{
		if (all_trees == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < all_trees.Count; i++)
		{
			TreeInstance treeInstance = all_trees[i];
			Vector3 position = treeInstance.position;
			position.x = terrain_bounds.min.x + position.x * terrain_bounds.size.x;
			position.z = terrain_bounds.min.z + position.z * terrain_bounds.size.z;
			if (position.x >= bounds.min.x && position.x <= bounds.max.x && position.z >= bounds.min.z && position.z <= bounds.max.z)
			{
				if (gen_alphas != null)
				{
					float num2 = 1f;
					float num3 = gen_alphas.Get(position.x, position.z, 1f, ref num2);
					if (num3 <= 0f || num3 < min_alpha)
					{
						goto IL_10D;
					}
				}
				num++;
				all_trees.RemoveAt(i);
				i--;
				if (to != null)
				{
					to.Add(treeInstance);
				}
			}
			IL_10D:;
		}
		return num;
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060002E4 RID: 740 RVA: 0x00026CBB File Offset: 0x00024EBB
	public static BSGTerrainTools.Concurrent concurrentIDMap
	{
		get
		{
			return new BSGTerrainTools.Concurrent(BSGTerrainTools.mapVecToID);
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060002E5 RID: 741 RVA: 0x00026CC7 File Offset: 0x00024EC7
	public static BSGTerrainTools.ConcurrentOutput concurrentOutput
	{
		get
		{
			return new BSGTerrainTools.ConcurrentOutput(BSGTerrainTools.mapIDToOutput);
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00026CD3 File Offset: 0x00024ED3
	private unsafe static float* getPointer(float* arr, int x, int y, int layer, int width, int height, int layers)
	{
		return arr + layer + x * layers + y * width * layers;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00026CD3 File Offset: 0x00024ED3
	private unsafe static int* getDetailPointer(int* arr, int x, int y, int layer, int width, int height, int layers)
	{
		return arr + layer + x * layers + y * width * layers;
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00026CEE File Offset: 0x00024EEE
	private unsafe static bool OutOfBounds(int x, int y, int layer, int remapped_layer, float* arr, int width, int height, int layers)
	{
		return x < 0 || x >= width || y < 0 || y >= height || remapped_layer < 0;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00026D08 File Offset: 0x00024F08
	private unsafe static float GetPixel(int x, int y, int layer, int remapped_layer, float* arr, int width, int height, int layers)
	{
		if (!BSGTerrainTools.OutOfBounds(x, y, layer, remapped_layer, arr, width, height, layers))
		{
			return *BSGTerrainTools.getPointer(arr, x, y, remapped_layer, width, height, layers);
		}
		if (layer == 0)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00026D3D File Offset: 0x00024F3D
	private unsafe static void SetPixel(float val, int x, int y, int layer, int remapped_layer, float* arr, int width, int height, int layers)
	{
		*BSGTerrainTools.getPointer(arr, x, y, remapped_layer, width, height, layers) = val;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00026D54 File Offset: 0x00024F54
	public static void PasteTrees(TerrainData td, Point center, List<TreeInstance> trees, BSGTerrainTools.TerrainRemap remap, Vector3 lossyScale, float angle = 0f)
	{
		if (trees == null || trees.Count == 0)
		{
			return;
		}
		TreeInstance[] treeInstances = td.treeInstances;
		int num = treeInstances.Length;
		TreeInstance[] array = new TreeInstance[treeInstances.Length + trees.Count];
		for (int i = 0; i < num; i++)
		{
			array[i] = treeInstances[i];
		}
		for (int j = 0; j < trees.Count; j++)
		{
			TreeInstance treeInstance = trees[j];
			int prototypeIndex;
			if (remap != null && remap.remapped_trees.TryGetValue(treeInstance.prototypeIndex, out prototypeIndex))
			{
				treeInstance.prototypeIndex = prototypeIndex;
			}
			Point rotated;
			if (remap != null)
			{
				rotated = new Point(treeInstance.position.x * remap.src.size.x, treeInstance.position.z * remap.src.size.z);
			}
			else
			{
				rotated = new Point(treeInstance.position.x * td.size.x, treeInstance.position.z * td.size.z);
			}
			rotated = rotated.GetRotated(angle);
			Vector3 vector = global::Common.SnapToTerrain(new Point(rotated.x * lossyScale.x, rotated.y * lossyScale.z) + center, 0f, null, -1f, false);
			treeInstance.position = new Vector3(vector.x / td.size.x, vector.y / td.size.y, vector.z / td.size.z);
			treeInstance.widthScale *= lossyScale.x;
			treeInstance.heightScale *= lossyScale.y;
			array[num + j] = treeInstance;
		}
		td.SetTreeInstances(array, false);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00026F30 File Offset: 0x00025130
	public static void EraseTrees(TerrainData td, PrefabGrid pg, Point center, int x, int y)
	{
		Bounds bounds = pg.GetBounds(center, x, y);
		BSGTerrainTools.EraseTrees(td, bounds);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00026F50 File Offset: 0x00025150
	public static void EraseTrees(TerrainData td, Bounds bounds)
	{
		List<TreeInstance> list = new List<TreeInstance>();
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		for (int i = 0; i < td.treeInstanceCount; i++)
		{
			TreeInstance treeInstance = td.GetTreeInstance(i);
			Vector3 vector = Vector3.Scale(treeInstance.position, td.size);
			if (vector.x < min.x || vector.x > max.x || vector.z < min.z || vector.z > max.z)
			{
				list.Add(treeInstance);
			}
		}
		td.SetTreeInstances(list.ToArray(), false);
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00026FF4 File Offset: 0x000251F4
	private static void AddCopyInfoToJobs(BSGTerrainTools.PerTerrainInfo.PerCellInfo copyinfo, BSGTerrainTools.TerrainRemap terrain_remap, TerrainData src_terrain, TerrainData dst_terrain, List<ulong> to_dispose, Vector3 position, Vector3 rotation, Vector3 scale, Bounds bounds, List<BSGTerrainTools.SplatPGInput> input_list, bool erase = false)
	{
		BSGTerrainTools.AddCopyInfoToJobs(copyinfo, terrain_remap, src_terrain, dst_terrain, to_dispose, position, rotation, scale, bounds, input_list, Vector3.zero, Vector3.zero, -1f, erase);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00027028 File Offset: 0x00025228
	private unsafe static void AddCopyInfoToJobs(BSGTerrainTools.PerTerrainInfo.PerCellInfo copyinfo, BSGTerrainTools.TerrainRemap terrain_remap, TerrainData src_terrain, TerrainData dst_terrain, List<ulong> to_dispose, Vector3 position, Vector3 rotation, Vector3 scale, Bounds bounds, List<BSGTerrainTools.SplatPGInput> input_list, Vector3 alpha_start, Vector3 alpha_end, float alpha_dist, bool erase = false)
	{
		if (copyinfo == null)
		{
			return;
		}
		BSGTerrainTools.SplatPGInput splatPGInput = default(BSGTerrainTools.SplatPGInput);
		Point point = position;
		if (erase)
		{
			BSGTerrainTools.EraseTrees(dst_terrain, bounds);
		}
		else
		{
			BSGTerrainTools.PasteTrees(dst_terrain, point, copyinfo.trees, terrain_remap, scale, rotation.y);
		}
		splatPGInput.pos = new Point((float)dst_terrain.alphamapWidth * point.x / dst_terrain.size.x, (float)dst_terrain.alphamapHeight * point.y / dst_terrain.size.z);
		float[,,] array;
		if (erase)
		{
			array = copyinfo.bg_splats.ToArray(null);
		}
		else
		{
			array = copyinfo.splats.ToArray(null);
		}
		int length = array.GetLength(1);
		int length2 = array.GetLength(0);
		int length3 = array.GetLength(2);
		splatPGInput.width = length;
		splatPGInput.height = length2;
		splatPGInput.layers = length3;
		ulong item = 0UL;
		splatPGInput.input = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item);
		to_dispose.Add(item);
		float num = -(3.1415927f * rotation.y) / 180f;
		float num2 = (float)Math.Cos((double)num);
		float num3 = (float)Math.Sin((double)num);
		float2 @float = math.float3(src_terrain.bounds.size).xz / (float)src_terrain.alphamapResolution;
		splatPGInput.scale_x = scale.x * @float.x;
		splatPGInput.scale_y = scale.z * @float.y;
		float x = (float)(-(float)length2) * num3;
		float x2 = (float)length2 * num2;
		float x3 = (float)length * num2 - (float)length2 * num3;
		float x4 = (float)length2 * num2 + (float)length * num3;
		float y = (float)length * num2;
		float y2 = (float)length * num3;
		float num4 = math.min(0f, math.min(x, math.min(x3, y)));
		float num5 = math.min(0f, math.min(x2, math.min(x4, y2)));
		float x5 = math.max(0f, math.max(x, math.max(x3, y)));
		int num6 = (int)math.ceil(math.abs(math.max(0f, math.max(x2, math.max(x4, y2)))) - num5);
		int num7 = (int)math.ceil(math.abs(x5) - num4);
		splatPGInput.cosine = num2;
		splatPGInput.sine = num3;
		splatPGInput.minx = num4;
		splatPGInput.miny = num5;
		splatPGInput.new_width = (int)math.ceil((float)num6 * splatPGInput.scale_x);
		splatPGInput.new_height = (int)math.ceil((float)num7 * splatPGInput.scale_y);
		splatPGInput.new_layers = dst_terrain.alphamapLayers;
		splatPGInput.alpha_start = alpha_start;
		splatPGInput.alpha_end = alpha_end;
		splatPGInput.alpha_dist = alpha_dist;
		float[,,] target = new float[splatPGInput.new_height, splatPGInput.new_width, splatPGInput.new_layers];
		ulong item2 = 0UL;
		splatPGInput.output = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target, ref item2);
		to_dispose.Add(item2);
		input_list.Add(splatPGInput);
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0002732C File Offset: 0x0002552C
	private static void AddCopyInfoToJobs(BSGTerrainTools.PerTerrainInfo.PerCellInfo copyinfo, int x, int y, BSGTerrainTools.TerrainRemap terrain_remap, TerrainData src_terrain, TerrainData dst_terrain, List<ulong> to_dispose, PrefabGrid pg, List<BSGTerrainTools.SplatPGInput> input_list, bool erase = false)
	{
		Bounds bounds = pg.GetBounds(pg.transform.position, x, y);
		if (x == -1 && y == -1)
		{
			BSGTerrainTools.AddCopyInfoToJobs(copyinfo, terrain_remap, src_terrain, dst_terrain, to_dispose, pg.transform.position, pg.transform.rotation.eulerAngles, pg.transform.lossyScale, bounds, input_list, erase);
			return;
		}
		BSGTerrainTools.AddCopyInfoToJobs(copyinfo, terrain_remap, src_terrain, dst_terrain, to_dispose, bounds.center, pg.transform.rotation.eulerAngles, pg.transform.lossyScale, bounds, input_list, erase);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x000273D4 File Offset: 0x000255D4
	private unsafe static void StartSplatJobs(NativeArray<int2> remapped_layers, List<BSGTerrainTools.SplatPGInput> input_list, float* splats_data, int* details_data, TerrainData src_terrain, TerrainData dst_terrain, float[,,] splats_arr, int[,,] details_arr, List<ulong> to_dispose, bool transparent = true)
	{
		BSGTerrainTools.SplatCalculateJob jobData = new BSGTerrainTools.SplatCalculateJob
		{
			pg_input = new NativeArray<BSGTerrainTools.SplatPGInput>(input_list.ToArray(), Allocator.Persistent),
			mapIDs = BSGTerrainTools.concurrentIDMap,
			mapOutput = BSGTerrainTools.concurrentOutput,
			remapped_layers = remapped_layers
		};
		BSGTerrainTools.SplatSumJob jobData2 = new BSGTerrainTools.SplatSumJob
		{
			splats = splats_data,
			details = details_data,
			width = dst_terrain.alphamapWidth,
			height = dst_terrain.alphamapHeight,
			layers = dst_terrain.alphamapLayers,
			detail_width = dst_terrain.detailWidth,
			detail_height = dst_terrain.detailHeight,
			detail_layers = dst_terrain.detailPrototypes.Length,
			map = BSGTerrainTools.mapVecToID,
			mapOutput = BSGTerrainTools.mapIDToOutput,
			transparent = transparent
		};
		NativeArray<int2> keyArray;
		if (Troops.debug_jobs)
		{
			jobData.Run(input_list.Count);
			keyArray = BSGTerrainTools.mapVecToID.GetKeyArray(Allocator.Persistent);
			jobData2.map_coords = keyArray;
			jobData2.Run(keyArray.Length);
		}
		else
		{
			int processorCount = SystemInfo.processorCount;
			jobData.Schedule(input_list.Count, input_list.Count / processorCount, default(JobHandle)).Complete();
			keyArray = BSGTerrainTools.mapVecToID.GetKeyArray(Allocator.Persistent);
			jobData2.map_coords = keyArray;
			jobData2.Schedule(keyArray.Length, keyArray.Length / processorCount, default(JobHandle)).Complete();
		}
		jobData.pg_input.Dispose();
		keyArray.Dispose();
		BSGTerrainTools.mapVecToID.Dispose();
		BSGTerrainTools.mapIDToOutput.Dispose();
		dst_terrain.SetAlphamaps(0, 0, splats_arr);
		int length = details_arr.GetLength(0);
		int length2 = details_arr.GetLength(1);
		int length3 = details_arr.GetLength(2);
		for (int i = 0; i < length3; i++)
		{
			int[,] array = new int[length, length2];
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					array[j, k] = details_arr[j, k, i];
				}
			}
			dst_terrain.SetDetailLayer(0, 0, i, array);
		}
		for (int l = 0; l < to_dispose.Count; l++)
		{
			ulong value = to_dispose[l];
			AllocationManager.ReleaseGCObject(ref value);
			to_dispose[l] = value;
		}
		remapped_layers.Dispose();
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00027640 File Offset: 0x00025840
	public unsafe static void ApplySplatMaps(List<SettlementBV.RoadBV> roads, PrefabGrid.Info info, TerrainData src_terrain, TerrainData dst_terrain)
	{
		if (roads == null || roads.Count == 0)
		{
			return;
		}
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>();
		List<ulong> list = new List<ulong>();
		Bounds bounds = new Bounds(roads[0].path.transform.position, Vector3.zero);
		int num = 0;
		for (int i = 0; i < roads.Count; i++)
		{
			TerrainPath path = roads[i].path;
			bounds.Encapsulate(path.bounds);
			num += path.path_points.Count;
		}
		BSGTerrainTools.PerTerrainInfo terrainInfo = PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = PrefabGrid.CopySingle(info, terrainInfo, info.architectures_position, 0, 0);
		if (perCellInfo == null)
		{
			return;
		}
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(num, Allocator.Persistent);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		list.Add(item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int j = 0; j < dst_terrain.detailPrototypes.Length; j++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, j);
			for (int k = 0; k < dst_terrain.detailWidth; k++)
			{
				for (int l = 0; l < dst_terrain.detailHeight; l++)
				{
					array[k, l, j] = detailLayer[k, l];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num2 = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num2++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int m = 0; m < roads.Count; m++)
		{
			SettlementBV.RoadBV roadBV = roads[m];
			TerrainPath path2 = roadBV.path;
			Vector3 vector = path2.path_points[0];
			Vector3 vector2 = path2.path_points[path2.path_points.Count - 1];
			float alpha_dist = path2.alpha_blend_dist;
			if (roadBV.type == SettlementBV.RoadBV.Type.Highway && path2.path_points.Count > 4)
			{
				Vector3 b = path2.path_points[1] - vector;
				b.y = 0f;
				vector -= b;
				b = path2.path_points[path2.path_points.Count - 2] - vector2;
				b.y = 0f;
				vector2 -= b;
			}
			else
			{
				alpha_dist = -1f;
			}
			for (int n = 0; n < path2.path_points.Count - 1; n++)
			{
				Vector3 vector3 = path2.path_points[n];
				Vector3 vector4 = path2.path_points[n + 1] - vector3;
				vector4.y = 0f;
				if (vector4.magnitude > 0.01f && info.tile_size > 0.01f)
				{
					Vector3 eulerAngles = Quaternion.LookRotation(vector4).eulerAngles;
					eulerAngles.y += 90f;
					float num3;
					float num4;
					info.GetSplatSize(out num3, out num4);
					float d = 1f;
					Vector3 vector5 = vector3 + vector4 * 0.5f;
					BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, terrainRemap, src_terrain, dst_terrain, list, vector5, eulerAngles, Vector3.one * d, new Bounds(vector5, new Vector3(info.rect_width, info.rect_height)), input_list, vector, vector2, alpha_dist, false);
				}
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00027A6C File Offset: 0x00025C6C
	public unsafe static void ApplySplatMaps(List<PrefabGrid> grids, TerrainData src_terrain, TerrainData dst_terrain)
	{
		if (grids == null || grids.Count == 0)
		{
			return;
		}
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>(grids.Count);
		List<ulong> list = new List<ulong>();
		Bounds bounds = new Bounds(grids[0].transform.position, Vector3.zero);
		for (int i = 0; i < grids.Count; i++)
		{
			Bounds bounds2 = grids[i].CalcBounds();
			Vector3 lossyScale = grids[i].transform.lossyScale;
			Vector3 size = bounds2.size;
			size.x *= lossyScale.x;
			size.z *= lossyScale.z;
			bounds2.size = size;
			bounds.Encapsulate(bounds2);
		}
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(grids.Count, Allocator.Persistent);
		BSGTerrainTools.PerTerrainInfo terrainInfo = PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		list.Add(item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int j = 0; j < dst_terrain.detailPrototypes.Length; j++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, j);
			for (int k = 0; k < dst_terrain.detailWidth; k++)
			{
				for (int l = 0; l < dst_terrain.detailHeight; l++)
				{
					array[k, l, j] = detailLayer[k, l];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int m = 0; m < grids.Count; m++)
		{
			PrefabGrid prefabGrid = grids[m];
			BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = prefabGrid.CopySingle(terrainInfo, prefabGrid.info.architectures_position, prefabGrid.cur_variant - 1, prefabGrid.cur_level - 1);
			if (perCellInfo != null)
			{
				BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, -1, -1, terrainRemap, src_terrain, dst_terrain, list, prefabGrid, input_list, false);
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00027D34 File Offset: 0x00025F34
	public unsafe static void ApplySplatMaps(Wall wall, TerrainData src_terrain, TerrainData dst_terrain)
	{
		if (wall == null || wall.corners.Count == 0 || wall.segments.Count == 0)
		{
			return;
		}
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>(wall.corners.Count + wall.segments.Count);
		List<ulong> list = new List<ulong>();
		Bounds bounds = new Bounds(wall.corners[0].transform.position, Vector3.zero);
		for (int i = 0; i < wall.corners.Count; i++)
		{
			bounds.Encapsulate(new Bounds(wall.corners[i].transform.position, Vector3.one * 10f));
		}
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(wall.corners.Count + wall.segments.Count, Allocator.Persistent);
		BSGTerrainTools.PerTerrainInfo terrainInfo = PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int j = 0; j < dst_terrain.detailPrototypes.Length; j++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, j);
			for (int k = 0; k < dst_terrain.detailWidth; k++)
			{
				for (int l = 0; l < dst_terrain.detailHeight; l++)
				{
					array[k, l, j] = detailLayer[k, l];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		list.Add(item);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int m = 0; m < wall.corners.Count; m++)
		{
			PrefabGrid component = wall.corners[m].GetComponent<PrefabGrid>();
			if (!(component == null))
			{
				BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = component.CopySingle(terrainInfo, component.info.architectures_position, component.cur_variant - 1, component.cur_level - 1);
				if (perCellInfo != null)
				{
					BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, -1, -1, terrainRemap, src_terrain, dst_terrain, list, component, input_list, false);
				}
			}
		}
		PrefabGrid.Info info = PrefabGrid.Info.Get("Walls", wall.segments_architecture, false);
		BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo2 = PrefabGrid.CopySingle(terrainInfo, info.architectures_position, info.grid_size, info.tile_size, info.tile_size, wall.segments_variant - 1, wall.level - 1);
		for (int n = 0; n < wall.segments.Count; n++)
		{
			GameObject gameObject = wall.segments[n];
			if (perCellInfo2 != null)
			{
				BSGTerrainTools.AddCopyInfoToJobs(perCellInfo2, terrainRemap, src_terrain, dst_terrain, list, gameObject.transform.position, gameObject.transform.rotation.eulerAngles, gameObject.transform.lossyScale, new Bounds(gameObject.transform.position, new Vector3(info.tile_size, 0f, info.tile_size)), input_list, false);
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00028104 File Offset: 0x00026304
	public unsafe static void ApplySplatMaps(PrefabGrid grid, TerrainData src_terrain, TerrainData dst_terrain)
	{
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>(grid.info.max_variant * grid.info.max_level);
		List<ulong> list = new List<ulong>();
		Bounds bounds = grid.CalcBounds();
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(grid.info.max_variant * grid.info.max_level, Allocator.Persistent);
		BSGTerrainTools.PerTerrainInfo terrainInfo = PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		list.Add(item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int i = 0; i < dst_terrain.detailPrototypes.Length; i++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, i);
			for (int j = 0; j < dst_terrain.detailWidth; j++)
			{
				for (int k = 0; k < dst_terrain.detailHeight; k++)
				{
					array[j, k, i] = detailLayer[j, k];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int l = 0; l < grid.info.max_variant; l++)
		{
			for (int m = 0; m < grid.info.max_level; m++)
			{
				BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = grid.CopySingle(terrainInfo, grid.info.architectures_position, l, m);
				if (perCellInfo != null)
				{
					BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, l, m, terrainRemap, src_terrain, dst_terrain, list, grid, input_list, false);
				}
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00028360 File Offset: 0x00026560
	public unsafe static void ApplySplatMaps(BSGTerrainTools.PerTerrainInfo.PerCellInfo[,] grids, PrefabGrid pg, TerrainData src_terrain, TerrainData dst_terrain)
	{
		if (grids == null)
		{
			return;
		}
		int length = grids.GetLength(0);
		int length2 = grids.GetLength(1);
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>(length * length2);
		List<ulong> list = new List<ulong>();
		Bounds bounds = pg.CalcBounds();
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(length * length2, Allocator.Persistent);
		PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		list.Add(item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int i = 0; i < dst_terrain.detailPrototypes.Length; i++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, i);
			for (int j = 0; j < dst_terrain.detailWidth; j++)
			{
				for (int k = 0; k < dst_terrain.detailHeight; k++)
				{
					array[j, k, i] = detailLayer[j, k];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int l = 0; l < length; l++)
		{
			for (int m = 0; m < length2; m++)
			{
				BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = grids[l, m];
				if (perCellInfo != null)
				{
					BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, l, m, terrainRemap, src_terrain, dst_terrain, list, pg, input_list, false);
				}
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00028584 File Offset: 0x00026784
	public unsafe static void EraseSplatMaps(BSGTerrainTools.PerTerrainInfo.PerCellInfo[,] grids, PrefabGrid pg, TerrainData src_terrain, TerrainData dst_terrain)
	{
		if (grids == null)
		{
			return;
		}
		int length = grids.GetLength(0);
		int length2 = grids.GetLength(1);
		List<BSGTerrainTools.SplatPGInput> input_list = new List<BSGTerrainTools.SplatPGInput>(length * length2);
		List<ulong> list = new List<ulong>();
		Bounds bounds = pg.CalcBounds();
		BSGTerrainTools.mapVecToID = new NativeMultiHashMap<int2, int>((int)(bounds.size.x * bounds.size.z * 10f), Allocator.Persistent);
		BSGTerrainTools.mapIDToOutput = new NativeHashMap<int, BSGTerrainTools.SplatPGOutput>(length * length2, Allocator.Persistent);
		PrefabGrid.GetTerrainInfo(src_terrain);
		BSGTerrainTools.TerrainRemap terrainRemap = new BSGTerrainTools.TerrainRemap(src_terrain, dst_terrain);
		ulong item = 0UL;
		float[,,] alphamaps = dst_terrain.GetAlphamaps(0, 0, dst_terrain.alphamapWidth, dst_terrain.alphamapHeight);
		float* splats_data = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref item);
		list.Add(item);
		ulong item2 = 0UL;
		int[,,] array = new int[dst_terrain.detailWidth, dst_terrain.detailHeight, dst_terrain.detailPrototypes.Length];
		for (int i = 0; i < dst_terrain.detailPrototypes.Length; i++)
		{
			int[,] detailLayer = dst_terrain.GetDetailLayer(0, 0, dst_terrain.detailWidth, dst_terrain.detailHeight, i);
			for (int j = 0; j < dst_terrain.detailWidth; j++)
			{
				for (int k = 0; k < dst_terrain.detailHeight; k++)
				{
					array[j, k, i] = detailLayer[j, k];
				}
			}
		}
		int* details_data = (int*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref item2);
		list.Add(item2);
		NativeArray<int2> remapped_layers = new NativeArray<int2>(terrainRemap.remapped_layers.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in terrainRemap.remapped_layers)
		{
			remapped_layers[num++] = new int2(keyValuePair.Key, keyValuePair.Value);
		}
		for (int l = 0; l < length; l++)
		{
			for (int m = 0; m < length2; m++)
			{
				BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = grids[l, m];
				if (perCellInfo != null)
				{
					BSGTerrainTools.AddCopyInfoToJobs(perCellInfo, l, m, terrainRemap, src_terrain, dst_terrain, list, pg, input_list, true);
				}
			}
		}
		BSGTerrainTools.StartSplatJobs(remapped_layers, input_list, splats_data, details_data, src_terrain, dst_terrain, alphamaps, array, list, true);
	}

	// Token: 0x040003E9 RID: 1001
	private static NativeMultiHashMap<int2, int> mapVecToID;

	// Token: 0x040003EA RID: 1002
	private static NativeHashMap<int, BSGTerrainTools.SplatPGOutput> mapIDToOutput;

	// Token: 0x02000528 RID: 1320
	public class TerrainInfo
	{
		// Token: 0x060042E3 RID: 17123 RVA: 0x001FA2B2 File Offset: 0x001F84B2
		public TerrainInfo()
		{
		}

		// Token: 0x060042E4 RID: 17124 RVA: 0x001FA2E8 File Offset: 0x001F84E8
		public TerrainInfo(TerrainData td)
		{
			Vector3 min = td.bounds.min;
			Vector3 max = td.bounds.max;
			min.y = 0f;
			max.y = td.size.y;
			this.bounds.SetMinMax(min, max);
			this.heights_resolution = BSGTerrainTools.HeightsResolution(td);
			this.splats_resolution = BSGTerrainTools.SplatsResolution(td);
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x001FA388 File Offset: 0x001F8588
		public void Save(BinaryWriter writer)
		{
			writer.Write(this.bounds.min.x);
			writer.Write(this.bounds.min.y);
			writer.Write(this.bounds.min.z);
			writer.Write(this.bounds.max.x);
			writer.Write(this.bounds.max.y);
			writer.Write(this.bounds.max.z);
			writer.Write(this.heights_resolution.x);
			writer.Write(this.heights_resolution.y);
			writer.Write(this.splats_resolution.x);
			writer.Write(this.splats_resolution.y);
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x001FA460 File Offset: 0x001F8660
		public static BSGTerrainTools.TerrainInfo Load(BinaryReader reader)
		{
			BSGTerrainTools.TerrainInfo terrainInfo = new BSGTerrainTools.TerrainInfo();
			Vector3 min = default(Vector3);
			Vector3 max = default(Vector3);
			min.x = reader.ReadSingle();
			min.y = reader.ReadSingle();
			min.z = reader.ReadSingle();
			max.x = reader.ReadSingle();
			max.y = reader.ReadSingle();
			max.z = reader.ReadSingle();
			terrainInfo.bounds.SetMinMax(min, max);
			terrainInfo.heights_resolution.x = reader.ReadInt32();
			terrainInfo.heights_resolution.y = reader.ReadInt32();
			terrainInfo.splats_resolution.x = reader.ReadInt32();
			terrainInfo.splats_resolution.y = reader.ReadInt32();
			return terrainInfo;
		}

		// Token: 0x04002F22 RID: 12066
		public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x04002F23 RID: 12067
		public Vector2Int heights_resolution = Vector2Int.zero;

		// Token: 0x04002F24 RID: 12068
		public Vector2Int splats_resolution = Vector2Int.zero;
	}

	// Token: 0x02000529 RID: 1321
	public class TerrainBlock
	{
		// Token: 0x060042E7 RID: 17127 RVA: 0x001FA524 File Offset: 0x001F8724
		public TerrainBlock(TerrainData td, Bounds bounds)
		{
			this.td = td;
			this.ti = new BSGTerrainTools.TerrainInfo(td);
			this.bounds = bounds;
			BSGTerrainTools.CalcGridBounds(bounds, td.bounds, BSGTerrainTools.HeightsResolution(td), out this.heights_ofs, out this.heights_size);
			BSGTerrainTools.CalcGridBounds(bounds, td.bounds, BSGTerrainTools.SplatsResolution(td), out this.splats_ofs, out this.splats_size);
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x001FA58D File Offset: 0x001F878D
		public void Import(bool import_heights = true, bool import_splats = true, bool import_trees = false)
		{
			this.ImportHeights(import_heights);
			this.ImportSplats(import_splats);
			this.ImportTrees(import_trees);
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x001FA5A4 File Offset: 0x001F87A4
		public void Apply(bool apply_heights = true, bool apply_splats = true, bool apply_trees = true)
		{
			if (apply_heights)
			{
				this.ApplyHeights();
			}
			if (apply_splats)
			{
				this.ApplySplats();
			}
			if (apply_trees)
			{
				this.ApplyTrees();
			}
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x001FA5C1 File Offset: 0x001F87C1
		public void Save(BinaryWriter writer, bool save_heights = true, bool save_splats = true, bool save_trees = true)
		{
			this.ti.Save(writer);
			if (save_heights)
			{
				this.SaveHeights(writer);
			}
			if (save_splats)
			{
				this.SaveSplats(writer);
			}
			if (save_trees)
			{
				this.SaveTrees(writer);
			}
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x001FA5EE File Offset: 0x001F87EE
		public void Load(BinaryReader reader, bool load_heights = true, bool load_splats = true, bool load_trees = true)
		{
			this.ti = BSGTerrainTools.TerrainInfo.Load(reader);
			if (load_heights)
			{
				this.LoadHeights(reader);
			}
			if (load_splats)
			{
				this.LoadSplats(reader);
			}
			if (load_trees)
			{
				this.LoadTrees(reader);
			}
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x001FA61C File Offset: 0x001F881C
		public void Save(string filename, bool save_heights = true, bool save_splats = true, bool save_trees = true)
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					this.Save(binaryWriter, save_heights, save_splats, save_trees);
				}
			}
		}

		// Token: 0x060042ED RID: 17133 RVA: 0x001FA678 File Offset: 0x001F8878
		public bool Load(string filename, bool load_heights = true, bool load_splats = true, bool load_trees = true)
		{
			bool result;
			try
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						this.Load(binaryReader, load_heights, load_splats, load_trees);
					}
				}
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060042EE RID: 17134 RVA: 0x001FA6EC File Offset: 0x001F88EC
		public void ImportHeights(bool import_heights = true)
		{
			this.heights = (import_heights ? this.td.GetHeights(this.heights_ofs.x, this.heights_ofs.y, this.heights_size.x, this.heights_size.y) : null);
		}

		// Token: 0x060042EF RID: 17135 RVA: 0x001FA73C File Offset: 0x001F893C
		public void ImportSplats(bool import_splats)
		{
			this.splats = (import_splats ? this.td.GetAlphamaps(this.splats_ofs.x, this.splats_ofs.y, this.splats_size.x, this.splats_size.y) : null);
		}

		// Token: 0x060042F0 RID: 17136 RVA: 0x001FA78C File Offset: 0x001F898C
		public void ImportTrees(bool import_trees = true)
		{
			this.trees = (import_trees ? new List<TreeInstance>(this.td.treeInstances) : null);
		}

		// Token: 0x060042F1 RID: 17137 RVA: 0x001FA7AA File Offset: 0x001F89AA
		public void ApplyHeights()
		{
			if (this.heights != null)
			{
				this.td.SetHeights(this.heights_ofs.x, this.heights_ofs.y, this.heights);
			}
		}

		// Token: 0x060042F2 RID: 17138 RVA: 0x001FA7DB File Offset: 0x001F89DB
		public void ApplySplats()
		{
			if (this.splats != null)
			{
				this.td.SetAlphamaps(this.splats_ofs.x, this.splats_ofs.y, this.splats);
			}
		}

		// Token: 0x060042F3 RID: 17139 RVA: 0x001FA80C File Offset: 0x001F8A0C
		public void ApplyTrees()
		{
			if (this.trees != null)
			{
				this.td.treeInstances = this.trees.ToArray();
			}
		}

		// Token: 0x060042F4 RID: 17140 RVA: 0x001FA82C File Offset: 0x001F8A2C
		public void SaveHeights(BinaryWriter writer)
		{
			if (this.heights == null)
			{
				writer.Write(-1);
				return;
			}
			int length = this.heights.GetLength(0);
			int length2 = this.heights.GetLength(1);
			writer.Write(length);
			writer.Write(length2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					writer.Write(this.heights[i, j]);
				}
			}
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x001FA89C File Offset: 0x001F8A9C
		public void LoadHeights(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0)
			{
				this.heights = null;
				return;
			}
			int num2 = reader.ReadInt32();
			this.heights = new float[num, num2];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					this.heights[i, j] = reader.ReadSingle();
				}
			}
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x001FA8FC File Offset: 0x001F8AFC
		public void SaveSplats(BinaryWriter writer)
		{
			if (this.splats == null)
			{
				writer.Write(-1);
				return;
			}
			int length = this.splats.GetLength(0);
			int length2 = this.splats.GetLength(1);
			int length3 = this.splats.GetLength(2);
			writer.Write(length);
			writer.Write(length2);
			writer.Write(length3);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						writer.Write(this.splats[i, j, k]);
					}
				}
			}
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x001FA998 File Offset: 0x001F8B98
		public void LoadSplats(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0)
			{
				this.splats = null;
				return;
			}
			int num2 = reader.ReadInt32();
			int num3 = reader.ReadInt32();
			this.splats = new float[num, num2, num3];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						this.splats[i, j, k] = reader.ReadSingle();
					}
				}
			}
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x001FAA18 File Offset: 0x001F8C18
		public void SaveTrees(BinaryWriter writer)
		{
			if (this.trees == null)
			{
				writer.Write(-1);
				return;
			}
			int count = this.trees.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				TreeInstance treeInstance = this.trees[i];
				BSGTerrainTools.SaveTree(writer, treeInstance);
			}
		}

		// Token: 0x060042F9 RID: 17145 RVA: 0x001FAA68 File Offset: 0x001F8C68
		public void LoadTrees(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0)
			{
				this.trees = null;
				return;
			}
			this.trees = new List<TreeInstance>(num);
			for (int i = 0; i < num; i++)
			{
				TreeInstance item = BSGTerrainTools.LoadTree(reader);
				this.trees.Add(item);
			}
		}

		// Token: 0x060042FA RID: 17146 RVA: 0x001FAAB4 File Offset: 0x001F8CB4
		public float GetHeight(float wx, float wy)
		{
			int num = BSGTerrainTools.WorldToGrid(this.ti.bounds.size.x, this.ti.heights_resolution.x, wx);
			int num2 = BSGTerrainTools.WorldToGrid(this.ti.bounds.size.z, this.ti.heights_resolution.y, wy);
			return this.heights[num2, num];
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x001FAB28 File Offset: 0x001F8D28
		public float CalcSlope(float wx, float wy)
		{
			int num = BSGTerrainTools.WorldToGrid(this.ti.bounds.size.x, this.ti.heights_resolution.x, wx);
			int num2 = BSGTerrainTools.WorldToGrid(this.ti.bounds.size.z, this.ti.heights_resolution.y, wy);
			if (num < 1 || num + 1 >= this.ti.heights_resolution.x || num2 < 1 || num2 + 1 >= this.ti.heights_resolution.y)
			{
				return 0f;
			}
			float y = this.ti.bounds.size.y;
			float x = (this.heights[num2, num + 1] - this.heights[num2, num - 1]) * y * (float)this.ti.heights_resolution.x / this.ti.bounds.size.x;
			float z = (this.heights[num2 + 1, num] - this.heights[num2 - 1, num]) * y * (float)this.ti.heights_resolution.y / this.ti.bounds.size.z;
			return Mathf.Acos(Vector3.Dot(new Vector3(x, 2f, z).normalized, Vector3.up)) * 57.29578f;
		}

		// Token: 0x04002F25 RID: 12069
		public TerrainData td;

		// Token: 0x04002F26 RID: 12070
		public BSGTerrainTools.TerrainInfo ti;

		// Token: 0x04002F27 RID: 12071
		public Bounds bounds;

		// Token: 0x04002F28 RID: 12072
		public Vector2Int heights_ofs;

		// Token: 0x04002F29 RID: 12073
		public Vector2Int heights_size;

		// Token: 0x04002F2A RID: 12074
		public Vector2Int splats_ofs;

		// Token: 0x04002F2B RID: 12075
		public Vector2Int splats_size;

		// Token: 0x04002F2C RID: 12076
		public float[,] heights;

		// Token: 0x04002F2D RID: 12077
		public float[,,] splats;

		// Token: 0x04002F2E RID: 12078
		public List<TreeInstance> trees;
	}

	// Token: 0x0200052A RID: 1322
	public class Float2D
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x060042FC RID: 17148 RVA: 0x001FAC97 File Offset: 0x001F8E97
		public int arr_width
		{
			get
			{
				return this.arr.GetLength(1);
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060042FD RID: 17149 RVA: 0x001FACA5 File Offset: 0x001F8EA5
		public int arr_height
		{
			get
			{
				return this.arr.GetLength(0);
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x060042FE RID: 17150 RVA: 0x001FACB3 File Offset: 0x001F8EB3
		public float world_width
		{
			get
			{
				return (float)this.width * this.world_scale_x;
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x060042FF RID: 17151 RVA: 0x001FACC3 File Offset: 0x001F8EC3
		public float world_height
		{
			get
			{
				return (float)this.height * this.world_scale_y;
			}
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x001FACD4 File Offset: 0x001F8ED4
		public Float2D(float val)
		{
			float[,] array = new float[1, 1];
			array[0, 0] = val;
			this.arr = array;
			this.xofs = (this.yofs = 0);
			this.width = (this.height = 1);
			this.tile = false;
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x001FAD48 File Offset: 0x001F8F48
		public Float2D(int w, int h, float val = 0f)
		{
			this.arr = new float[h, w];
			this.xofs = (this.yofs = 0);
			this.width = w;
			this.height = h;
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					this.arr[i, j] = val;
				}
			}
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x001FADD8 File Offset: 0x001F8FD8
		public Float2D(float[,] arr)
		{
			this.arr = arr;
			this.xofs = 0;
			this.yofs = 0;
			this.width = this.arr_width;
			this.height = this.arr_height;
			this.tile = false;
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x001FAE40 File Offset: 0x001F9040
		public float[,] ToArray()
		{
			if (this.xofs == 0 && this.yofs == 0 && this.width == this.arr_width && this.height == this.arr_height && this.mul == 1f && this.add == 0f)
			{
				return this.arr;
			}
			float[,] array = new float[this.height, this.width];
			for (int i = 0; i < this.height; i++)
			{
				int j = 0;
				while (j < this.width)
				{
					array[i, j] = this.GetRaw(j, i);
					i++;
				}
			}
			return array;
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x001FAEE0 File Offset: 0x001F90E0
		public void SetLocalRect(int x, int y, int w, int h)
		{
			if (x < 0)
			{
				x = 0;
			}
			else if (x >= this.arr_width)
			{
				x = this.arr_width - 1;
			}
			if (y < 0)
			{
				y = 0;
			}
			else if (y >= this.arr_height)
			{
				y = this.arr_height - 1;
			}
			if (x + w > this.arr_width)
			{
				w = this.arr_width - x;
			}
			if (y + h > this.arr_height)
			{
				h = this.arr_height - y;
			}
			this.xofs = x;
			this.yofs = y;
			this.width = w;
			this.height = h;
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x001FAF6D File Offset: 0x001F916D
		public void SetWorldPos(float wx, float wy)
		{
			this.world_x = wx;
			this.world_y = wy;
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x001FAF7D File Offset: 0x001F917D
		public void SetWorldSize(float w, float h)
		{
			this.world_scale_x = w / (float)this.width;
			this.world_scale_y = h / (float)this.height;
		}

		// Token: 0x06004307 RID: 17159 RVA: 0x001FAF9D File Offset: 0x001F919D
		public void SetWorldRect(float x, float y, float w, float h)
		{
			this.SetWorldPos(x, y);
			this.SetWorldSize(w, h);
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x001FAFB0 File Offset: 0x001F91B0
		public void SetWorldRect(BSGTerrainTools.Float2D f2d)
		{
			this.SetWorldPos(f2d.world_x, f2d.world_y);
			this.SetWorldSize(f2d.world_width, f2d.world_height);
		}

		// Token: 0x06004309 RID: 17161 RVA: 0x001FAFD8 File Offset: 0x001F91D8
		public void SetScale(float scale)
		{
			this.mul = scale;
			this.world_scale_y = scale;
			this.world_scale_x = scale;
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x001FB000 File Offset: 0x001F9200
		public static BSGTerrainTools.Float2D Load(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			float[,] array = new float[num2, num];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					float num3 = reader.ReadSingle();
					array[i, j] = num3;
				}
			}
			return new BSGTerrainTools.Float2D(array);
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x001FB058 File Offset: 0x001F9258
		public void Save(BinaryWriter writer)
		{
			writer.Write(this.width);
			writer.Write(this.height);
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					float value = this.arr[i + this.yofs, j + this.xofs];
					writer.Write(value);
				}
			}
		}

		// Token: 0x0600430C RID: 17164 RVA: 0x001FB0C4 File Offset: 0x001F92C4
		public static BSGTerrainTools.Float2D ImportRaw(string filename)
		{
			byte[] array;
			try
			{
				array = File.ReadAllBytes(filename);
			}
			catch
			{
				return null;
			}
			int num = Convert.ToInt32(Mathf.Sqrt((float)(array.Length / 2)));
			int num2 = array.Length / num;
			float[,] array2 = new float[num2, num];
			int num3 = 0;
			for (int i = num2 - 1; i >= 0; i--)
			{
				for (int j = 0; j < num; j++)
				{
					uint num4 = (uint)array[num3++];
					uint num5 = (uint)array[num3++];
					float num6 = (num4 | num5 << 8) / 65535f;
					array2[i, j] = num6;
				}
			}
			return new BSGTerrainTools.Float2D(array2);
		}

		// Token: 0x0600430D RID: 17165 RVA: 0x001FB16C File Offset: 0x001F936C
		public static BSGTerrainTools.Float2D ImportTexture(string filename, int iChannel = 0)
		{
			BSGTerrainTools.Float3D float3D = BSGTerrainTools.Float3D.ImportTexture(filename, iChannel + 1);
			if (float3D == null)
			{
				return null;
			}
			return float3D.layers[iChannel];
		}

		// Token: 0x0600430E RID: 17166 RVA: 0x001FB190 File Offset: 0x001F9390
		public static BSGTerrainTools.Float2D ImportHeights(TerrainData td)
		{
			return BSGTerrainTools.Float2D.ImportHeights(td, 0, 0, td.heightmapResolution, td.heightmapResolution);
		}

		// Token: 0x0600430F RID: 17167 RVA: 0x001FB1A6 File Offset: 0x001F93A6
		public static BSGTerrainTools.Float2D ImportHeights(TerrainData td, int x, int y, int width, int height)
		{
			return new BSGTerrainTools.Float2D(td.GetHeights(x, y, width, height));
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x001FB1B8 File Offset: 0x001F93B8
		public void ExportHeights(TerrainData td, int x = 0, int y = 0)
		{
			td.SetHeights(x, y, this.ToArray());
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x001FB1C8 File Offset: 0x001F93C8
		public Texture2D ExportTexture()
		{
			Texture2D texture2D = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					float raw = this.GetRaw(j, i);
					Color color = new Color(raw, raw, raw);
					texture2D.SetPixel(j, i, color);
				}
			}
			return texture2D;
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x001FB22C File Offset: 0x001F942C
		public void SavePNG(string filename)
		{
			Texture2D texture2D = this.ExportTexture();
			byte[] bytes = texture2D.EncodeToPNG();
			UnityEngine.Object.DestroyImmediate(texture2D);
			File.WriteAllBytes(filename, bytes);
		}

		// Token: 0x06004313 RID: 17171 RVA: 0x001FB252 File Offset: 0x001F9452
		public float GetRaw(int x, int y)
		{
			x += this.xofs;
			y += this.yofs;
			return this.arr[y, x] * this.mul + this.add;
		}

		// Token: 0x06004314 RID: 17172 RVA: 0x001FB283 File Offset: 0x001F9483
		public void SetRaw(int x, int y, float val)
		{
			x += this.xofs;
			y += this.yofs;
			this.arr[y, x] = (val - this.add) / this.mul;
		}

		// Token: 0x06004315 RID: 17173 RVA: 0x001FB2B5 File Offset: 0x001F94B5
		public static float FixCoord(float f, int sz, bool tile)
		{
			sz--;
			if (f < 0f)
			{
				if (!tile)
				{
					return 0f;
				}
				return (float)sz + f % (float)sz;
			}
			else
			{
				if (f <= (float)sz)
				{
					return f;
				}
				if (!tile)
				{
					return (float)sz;
				}
				return f % (float)sz;
			}
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x001FB2E8 File Offset: 0x001F94E8
		public float GetLocal(float lx, float ly)
		{
			lx = BSGTerrainTools.Float2D.FixCoord(lx, this.width, this.tile);
			ly = BSGTerrainTools.Float2D.FixCoord(ly, this.height, this.tile);
			int num = (int)lx;
			int num2 = (int)ly;
			lx -= (float)num;
			ly -= (float)num2;
			int num3 = num + 1;
			if (num3 == this.width)
			{
				num3 = (this.tile ? 0 : (this.width - 1));
			}
			int num4 = num2 + 1;
			if (num4 == this.height)
			{
				num4 = (this.tile ? 0 : (this.height - 1));
			}
			float raw = this.GetRaw(num, num2);
			float raw2 = this.GetRaw(num3, num2);
			float raw3 = this.GetRaw(num, num4);
			float raw4 = this.GetRaw(num3, num4);
			float num5 = raw * (1f - lx) + raw2 * lx;
			float num6 = raw3 * (1f - lx) + raw4 * lx;
			return num5 * (1f - ly) + num6 * ly;
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x001FB3C4 File Offset: 0x001F95C4
		public float GetUV(float u, float v)
		{
			float lx = u * (float)(this.width - 1);
			float ly = v * (float)(this.height - 1);
			return this.GetLocal(lx, ly);
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x001FB3F4 File Offset: 0x001F95F4
		public float GetWorld(float wx, float wy)
		{
			float lx = (wx - this.world_x) / this.world_scale_x;
			float ly = (wy - this.world_y) / this.world_scale_y;
			return this.GetLocal(lx, ly);
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x001FB42C File Offset: 0x001F962C
		public void BlitTo(BSGTerrainTools.Float2D dst, bool flip_x = false, bool flip_y = false, bool swap_xy = false)
		{
			for (int i = 0; i < dst.height; i++)
			{
				float num = (float)i / (float)(dst.height - 1);
				if (flip_y)
				{
					num = 1f - num;
				}
				for (int j = 0; j < dst.width; j++)
				{
					float num2 = (float)j / (float)(dst.width - 1);
					if (flip_y)
					{
						num2 = 1f - num2;
					}
					float val = swap_xy ? this.GetUV(num, num2) : this.GetUV(num2, num);
					dst.SetRaw(j, i, val);
				}
			}
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x001FB4B0 File Offset: 0x001F96B0
		public BSGTerrainTools.Float2D GetResampled(int w, int h, bool flip_x = false, bool flip_y = false, bool swap_xy = false)
		{
			if (w <= 0)
			{
				w = this.width;
			}
			if (h <= 0)
			{
				h = this.height;
			}
			BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(w, h, 0f);
			float2D.world_x = this.world_x;
			float2D.world_y = this.world_y;
			float2D.SetWorldSize(this.world_width, this.world_height);
			this.BlitTo(float2D, flip_x, flip_y, swap_xy);
			return float2D;
		}

		// Token: 0x0600431B RID: 17179 RVA: 0x001FB51C File Offset: 0x001F971C
		public BSGTerrainTools.Float2D GetModified(BSGTerrainTools.Gen2D gen_val, BSGTerrainTools.Gen2D gen_alpha = null)
		{
			BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(this.width, this.height, 0f);
			float2D.world_x = this.world_x;
			float2D.world_y = this.world_y;
			float2D.world_scale_x = this.world_scale_x;
			float2D.world_scale_y = this.world_scale_y;
			for (int i = 0; i < this.height; i++)
			{
				float wy = this.world_y + (float)i * this.world_scale_y;
				int j = 0;
				while (j < this.width)
				{
					float wx = this.world_x + (float)j * this.world_scale_x;
					float raw = this.GetRaw(j, i);
					float num;
					if (gen_alpha == null)
					{
						num = 1f;
						goto IL_C9;
					}
					float num2 = 1f;
					num = gen_alpha.Get(wx, wy, raw, ref num2);
					if (num <= 0f)
					{
						float2D.SetRaw(j, i, raw);
					}
					else
					{
						if (num > 1f)
						{
							num = 1f;
							goto IL_C9;
						}
						goto IL_C9;
					}
					IL_F1:
					j++;
					continue;
					IL_C9:
					float val = gen_val.Get(wx, wy, raw, ref num) * num + raw * (1f - num);
					float2D.SetRaw(j, i, val);
					goto IL_F1;
				}
			}
			return float2D;
		}

		// Token: 0x04002F2F RID: 12079
		public float[,] arr;

		// Token: 0x04002F30 RID: 12080
		public float mul = 1f;

		// Token: 0x04002F31 RID: 12081
		public float add;

		// Token: 0x04002F32 RID: 12082
		public int xofs;

		// Token: 0x04002F33 RID: 12083
		public int yofs;

		// Token: 0x04002F34 RID: 12084
		public int width;

		// Token: 0x04002F35 RID: 12085
		public int height;

		// Token: 0x04002F36 RID: 12086
		public bool tile;

		// Token: 0x04002F37 RID: 12087
		public float world_x;

		// Token: 0x04002F38 RID: 12088
		public float world_y;

		// Token: 0x04002F39 RID: 12089
		public float world_scale_x = 1f;

		// Token: 0x04002F3A RID: 12090
		public float world_scale_y = 1f;
	}

	// Token: 0x0200052B RID: 1323
	public class Float3D
	{
		// Token: 0x0600431C RID: 17180 RVA: 0x001FB63B File Offset: 0x001F983B
		public Float3D(BSGTerrainTools.Float2D[] layers)
		{
			this.layers = layers;
		}

		// Token: 0x0600431D RID: 17181 RVA: 0x001FB64C File Offset: 0x001F984C
		public Float3D(float[,,] arr)
		{
			int length = arr.GetLength(1);
			int length2 = arr.GetLength(0);
			int length3 = arr.GetLength(2);
			this.layers = new BSGTerrainTools.Float2D[length3];
			for (int i = 0; i < length3; i++)
			{
				float[,] array = new float[length2, length];
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length; k++)
					{
						array[j, k] = arr[j, k, i];
					}
				}
				this.layers[i] = new BSGTerrainTools.Float2D(array);
			}
		}

		// Token: 0x0600431E RID: 17182 RVA: 0x001FB6E0 File Offset: 0x001F98E0
		public Float3D(int num_layers, float val = 0f)
		{
			this.layers = new BSGTerrainTools.Float2D[num_layers];
			for (int i = 0; i < num_layers; i++)
			{
				this.layers[i] = new BSGTerrainTools.Float2D(val);
			}
		}

		// Token: 0x0600431F RID: 17183 RVA: 0x001FB71C File Offset: 0x001F991C
		public Float3D(int width, int height, int num_layers, float val = 0f)
		{
			this.layers = new BSGTerrainTools.Float2D[num_layers];
			for (int i = 0; i < num_layers; i++)
			{
				this.layers[i] = new BSGTerrainTools.Float2D(width, height, val);
			}
		}

		// Token: 0x06004320 RID: 17184 RVA: 0x001FB758 File Offset: 0x001F9958
		public Float3D(Color clr, int num_layers = 4)
		{
			this.layers = new BSGTerrainTools.Float2D[num_layers];
			for (int i = 0; i < num_layers; i++)
			{
				this.layers[i] = new BSGTerrainTools.Float2D((i < 4) ? clr[i] : 0f);
			}
		}

		// Token: 0x06004321 RID: 17185 RVA: 0x001FB7A4 File Offset: 0x001F99A4
		public Float3D(int width, int height, Color clr, int num_layers = 4)
		{
			this.layers = new BSGTerrainTools.Float2D[num_layers];
			for (int i = 0; i < num_layers; i++)
			{
				this.layers[i] = new BSGTerrainTools.Float2D(width, height, (i < 4) ? clr[i] : 0f);
			}
		}

		// Token: 0x06004322 RID: 17186 RVA: 0x001FB7F4 File Offset: 0x001F99F4
		public void SetLocalRect(int x, int y, int w, int h)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetLocalRect(x, y, w, h);
			}
		}

		// Token: 0x06004323 RID: 17187 RVA: 0x001FB828 File Offset: 0x001F9A28
		public void SetWorldPos(float wx, float wy)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetWorldPos(wx, wy);
			}
		}

		// Token: 0x06004324 RID: 17188 RVA: 0x001FB858 File Offset: 0x001F9A58
		public void SetWorldSize(float w, float h)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetWorldSize(w, h);
			}
		}

		// Token: 0x06004325 RID: 17189 RVA: 0x001FB888 File Offset: 0x001F9A88
		public void SetWorldRect(float x, float y, float w, float h)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetWorldRect(x, y, w, h);
			}
		}

		// Token: 0x06004326 RID: 17190 RVA: 0x001FB8BC File Offset: 0x001F9ABC
		public void SetWorldRect(BSGTerrainTools.Float2D f2d)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetWorldRect(f2d);
			}
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x001FB8EC File Offset: 0x001F9AEC
		public void SetScale(float scale)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].SetScale(scale);
			}
		}

		// Token: 0x06004328 RID: 17192 RVA: 0x001FB91C File Offset: 0x001F9B1C
		public float[,,] ToArray(BSGTerrainTools.Float2D[] layers = null)
		{
			if (layers == null)
			{
				layers = this.layers;
			}
			int width = layers[0].width;
			int height = layers[0].height;
			float[,,] array = new float[height, width, layers.Length];
			for (int i = 0; i < layers.Length; i++)
			{
				BSGTerrainTools.Float2D float2D = layers[i];
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < width; k++)
					{
						array[j, k, i] = float2D.GetRaw(k, j);
					}
				}
			}
			return array;
		}

		// Token: 0x06004329 RID: 17193 RVA: 0x001FB99C File Offset: 0x001F9B9C
		public static BSGTerrainTools.Float3D Load(BinaryReader reader)
		{
			BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[reader.ReadInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = BSGTerrainTools.Float2D.Load(reader);
			}
			return new BSGTerrainTools.Float3D(array);
		}

		// Token: 0x0600432A RID: 17194 RVA: 0x001FB9D4 File Offset: 0x001F9BD4
		public void Save(BinaryWriter writer)
		{
			writer.Write(this.layers.Length);
			for (int i = 0; i < this.layers.Length; i++)
			{
				this.layers[i].Save(writer);
			}
		}

		// Token: 0x0600432B RID: 17195 RVA: 0x001FBA10 File Offset: 0x001F9C10
		public static BSGTerrainTools.Float3D ImportSplats(TerrainData td)
		{
			return BSGTerrainTools.Float3D.ImportSplats(td, 0, 0, td.alphamapWidth, td.alphamapHeight);
		}

		// Token: 0x0600432C RID: 17196 RVA: 0x001FBA26 File Offset: 0x001F9C26
		public static BSGTerrainTools.Float3D ImportSplats(TerrainData td, int x, int y, int width, int height)
		{
			return new BSGTerrainTools.Float3D(td.GetAlphamaps(x, y, width, height));
		}

		// Token: 0x0600432D RID: 17197 RVA: 0x001FBA38 File Offset: 0x001F9C38
		private void shear(float angle, int x, int y, out int new_x, out int new_y)
		{
			float num = (float)Math.Tan((double)(angle / 2f));
			new_x = (int)Math.Round((double)((float)x - (float)y * num));
			new_y = y;
			new_y = (int)Math.Round((double)new_x * Math.Sin((double)angle) + (double)new_y);
			new_x = (int)Math.Round((double)((float)new_x - (float)new_y * num));
		}

		// Token: 0x0600432E RID: 17198 RVA: 0x001FBA98 File Offset: 0x001F9C98
		public void ExportSplats(TerrainData td, int x, int y, bool transparent = false, float width_scale = 1f, float height_scale = 1f, float angle = 0f, BSGTerrainTools.TerrainRemap remap = null)
		{
			if (this.layers.Length != 0)
			{
				int num = this.layers.Length;
				BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[num];
				angle = -angle * 0.0174533f;
				float num2 = (float)Math.Cos((double)angle);
				float num3 = (float)Math.Sin((double)angle);
				int arr_width = this.layers[0].arr_width;
				int arr_height = this.layers[0].arr_height;
				int num4 = (int)(Math.Round((double)(Math.Abs((float)arr_height * num2) + Math.Abs((float)arr_width * num3))) + 1.0);
				int num5 = (int)(Math.Round((double)(Math.Abs((float)arr_width * num2) + Math.Abs((float)arr_height * num3))) + 1.0);
				int num6 = (int)Math.Round((double)((float)(arr_height + 1) / 2f - 1f));
				int num7 = (int)Math.Round((double)((float)(arr_width + 1) / 2f - 1f));
				int num8 = (int)Math.Round((double)((float)(num4 + 1) / 2f - 1f));
				int num9 = (int)Math.Round((double)((float)(num5 + 1) / 2f - 1f));
				int num10 = (int)((float)num5 * width_scale);
				int num11 = (int)((float)num4 * height_scale);
				for (int i = 0; i < num; i++)
				{
					BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(num5, num4, (float)((i == 0) ? 1 : 0));
					BSGTerrainTools.Float2D float2D2 = this.layers[i];
					if (float2D2 != null)
					{
						array[i] = float2D;
						for (int j = 0; j < arr_height; j++)
						{
							for (int k = 0; k < arr_width; k++)
							{
								int y2 = arr_height - 1 - j - num6;
								int x2 = arr_width - 1 - k - num7;
								int num12;
								int num13;
								this.shear(angle, x2, y2, out num12, out num13);
								num13 = num8 - num13;
								num12 = num9 - num12;
								if (0 <= num12 && num12 < num5 && 0 <= num13 && num13 < num4 && num12 >= 0 && num13 >= 0)
								{
									float2D.arr[num13, num12] = float2D2.arr[j, k];
								}
							}
						}
						if (width_scale != 1f || height_scale != 1f)
						{
							BSGTerrainTools.Float2D float2D3 = new BSGTerrainTools.Float2D(num10, num11, 0f);
							for (int l = 0; l < num11; l++)
							{
								for (int m = 0; m < num10; m++)
								{
									float2D3.arr[l, m] = float2D.arr[(int)((float)l / height_scale), (int)((float)m / width_scale)];
								}
							}
							array[i] = float2D3;
						}
					}
				}
				num5 = num10;
				num4 = num11;
				if (remap != null && remap.remapped_layers.Count > 0)
				{
					BSGTerrainTools.Float2D[] array2 = new BSGTerrainTools.Float2D[remap.dst.alphamapLayers];
					array2[0] = array[0];
					for (int n = 1; n < array2.Length; n++)
					{
						if (!remap.remapped_layers.ContainsValue(n))
						{
							array2[n] = new BSGTerrainTools.Float2D(array[0].arr_width, array[0].arr_height, 0f);
						}
					}
					for (int num14 = 1; num14 < array.Length; num14++)
					{
						int num15;
						if (remap.remapped_layers.TryGetValue(num14, out num15))
						{
							array2[num15] = array[num14];
						}
					}
					array = array2;
					num = array.Length;
				}
				x -= (num5 - arr_width) / 2;
				y -= (num4 - arr_height) / 2;
				if (transparent)
				{
					float[,,] alphamaps = td.GetAlphamaps(x, y, num5, num4);
					for (int num16 = 0; num16 < num5; num16++)
					{
						for (int num17 = 0; num17 < num4; num17++)
						{
							float num18 = alphamaps[num17, num16, 0];
							float num19 = array[0].arr[num17, num16];
							if (num18 != 0f || num19 != 0f)
							{
								array[0].arr[num17, num16] = Math.Min(num19, num18);
								for (int num20 = 1; num20 < num; num20++)
								{
									float num21 = alphamaps[num17, num16, num20];
									array[num20].arr[num17, num16] += num21;
								}
								float num22 = 0f;
								for (int num23 = 0; num23 < array.Length; num23++)
								{
									num22 += array[num23].arr[num17, num16];
								}
								if (num22 == 0f)
								{
									array[0].arr[num17, num16] = 1f;
								}
								else
								{
									float num24 = 1f / num22;
									for (int num25 = 0; num25 < array.Length; num25++)
									{
										array[num25].arr[num17, num16] *= num24;
									}
								}
							}
						}
					}
				}
				td.SetAlphamaps(x, y, this.ToArray(array));
				return;
			}
			td.SetAlphamaps(x, y, this.ToArray(null));
		}

		// Token: 0x0600432F RID: 17199 RVA: 0x001FBF54 File Offset: 0x001FA154
		public static BSGTerrainTools.Float3D ImportTexture(string filename, int num_layers = -1)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			try
			{
				texture2D.LoadImage(File.ReadAllBytes(filename));
			}
			catch
			{
				return null;
			}
			if (num_layers <= 0)
			{
				num_layers = ((texture2D.format == TextureFormat.RGBA32) ? 4 : 3);
			}
			BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[num_layers];
			for (int i = 0; i < num_layers; i++)
			{
				array[i] = new BSGTerrainTools.Float2D(texture2D.width, texture2D.height, 0f);
			}
			Color[] pixels = texture2D.GetPixels();
			int num = 0;
			for (int j = 0; j < texture2D.height; j++)
			{
				for (int k = 0; k < texture2D.width; k++)
				{
					Color color = pixels[num++];
					array[0].arr[j, k] = color.r;
					if (num_layers > 1)
					{
						array[1].arr[j, k] = color.g;
					}
					if (num_layers > 2)
					{
						array[2].arr[j, k] = color.b;
					}
					if (num_layers > 3)
					{
						array[3].arr[j, k] = color.a;
					}
				}
			}
			return new BSGTerrainTools.Float3D(array);
		}

		// Token: 0x06004330 RID: 17200 RVA: 0x001FC094 File Offset: 0x001FA294
		public Texture2D ExportTexture()
		{
			int width = this.layers[0].width;
			int height = this.layers[0].height;
			int num = this.layers.Length;
			if (num > 4)
			{
				num = 4;
			}
			Texture2D texture2D = new Texture2D(width, height, (num > 3) ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color black = Color.black;
					for (int k = 0; k < num; k++)
					{
						black[k] = this.layers[k].GetRaw(j, i);
					}
					texture2D.SetPixel(j, i, black);
				}
			}
			return texture2D;
		}

		// Token: 0x06004331 RID: 17201 RVA: 0x001FC13C File Offset: 0x001FA33C
		public void SavePNG(string filename)
		{
			Texture2D texture2D = this.ExportTexture();
			byte[] bytes = texture2D.EncodeToPNG();
			UnityEngine.Object.DestroyImmediate(texture2D);
			File.WriteAllBytes(filename, bytes);
		}

		// Token: 0x06004332 RID: 17202 RVA: 0x001FC164 File Offset: 0x001FA364
		public BSGTerrainTools.Float3D GetResampled(int w, int h, bool flip_x = false, bool flip_y = false, bool swap_xy = false)
		{
			BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[this.layers.Length];
			for (int i = 0; i < this.layers.Length; i++)
			{
				array[i] = this.layers[i].GetResampled(w, h, flip_x, flip_y, swap_xy);
			}
			return new BSGTerrainTools.Float3D(array);
		}

		// Token: 0x06004333 RID: 17203 RVA: 0x001FC1B0 File Offset: 0x001FA3B0
		public BSGTerrainTools.Float3D GetModified(BSGTerrainTools.Gen2D gen_val, BSGTerrainTools.Gen2D gen_alpha = null)
		{
			BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[this.layers.Length];
			for (int i = 0; i < this.layers.Length; i++)
			{
				array[i] = this.layers[i].GetModified(gen_val, gen_alpha);
			}
			return new BSGTerrainTools.Float3D(array);
		}

		// Token: 0x04002F3B RID: 12091
		public BSGTerrainTools.Float2D[] layers;
	}

	// Token: 0x0200052C RID: 1324
	public class Gen2D
	{
		// Token: 0x06004334 RID: 17204 RVA: 0x001FC1F6 File Offset: 0x001FA3F6
		public Gen2D(float val)
		{
			this.val = val;
		}

		// Token: 0x06004335 RID: 17205 RVA: 0x001FC205 File Offset: 0x001FA405
		public Gen2D(BSGTerrainTools.Float2D f2d)
		{
			this.f2d = f2d;
		}

		// Token: 0x06004336 RID: 17206 RVA: 0x001FC214 File Offset: 0x001FA414
		public Gen2D(BSGTerrainTools.Gen2D.Func func, BSGTerrainTools.Float2D f2d = null)
		{
			this.func = func;
			this.f2d = f2d;
		}

		// Token: 0x06004337 RID: 17207 RVA: 0x001FC22A File Offset: 0x001FA42A
		public Gen2D(BSGTerrainTools.Gen2D.Func func, float val)
		{
			this.func = func;
			this.val = val;
		}

		// Token: 0x06004338 RID: 17208 RVA: 0x001FC240 File Offset: 0x001FA440
		public static float add(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			return src_val + tgt_val;
		}

		// Token: 0x06004339 RID: 17209 RVA: 0x001FC246 File Offset: 0x001FA446
		public static float mul(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			return src_val * tgt_val;
		}

		// Token: 0x0600433A RID: 17210 RVA: 0x001FC24C File Offset: 0x001FA44C
		public float Get(float wx, float wy, float src_val, ref float alpha)
		{
			float num = (this.f2d == null) ? this.val : this.f2d.GetWorld(wx, wy);
			if (this.func != null)
			{
				num = this.func(this, wx, wy, src_val, num, ref alpha);
			}
			return num;
		}

		// Token: 0x04002F3C RID: 12092
		public float val;

		// Token: 0x04002F3D RID: 12093
		public BSGTerrainTools.Float2D f2d;

		// Token: 0x04002F3E RID: 12094
		public BSGTerrainTools.Gen2D.Func func;

		// Token: 0x020009D1 RID: 2513
		// (Invoke) Token: 0x060054E1 RID: 21729
		public delegate float Func(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha);
	}

	// Token: 0x0200052D RID: 1325
	public class Gen3D
	{
		// Token: 0x0600433B RID: 17211 RVA: 0x001FC293 File Offset: 0x001FA493
		public Gen3D(float val)
		{
			this.val = val;
		}

		// Token: 0x0600433C RID: 17212 RVA: 0x001FC2A2 File Offset: 0x001FA4A2
		public Gen3D(BSGTerrainTools.Float3D f3d)
		{
			this.f3d = f3d;
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x001FC2B1 File Offset: 0x001FA4B1
		public Gen3D(BSGTerrainTools.Gen3D.Func func, BSGTerrainTools.Float3D f3d = null)
		{
			this.func = func;
			this.f3d = f3d;
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x001FC2C7 File Offset: 0x001FA4C7
		public Gen3D(BSGTerrainTools.Gen3D.Func func, float val)
		{
			this.func = func;
			this.val = val;
		}

		// Token: 0x0600433F RID: 17215 RVA: 0x001FC2E0 File Offset: 0x001FA4E0
		public float Get(float wx, float wy, int layer, float src_val, ref float alpha)
		{
			float num = (this.f3d == null) ? this.val : this.f3d.layers[layer].GetWorld(wx, wy);
			if (this.func != null)
			{
				num = this.func(this, wx, wy, layer, src_val, num, ref alpha);
			}
			return num;
		}

		// Token: 0x04002F3F RID: 12095
		public float val;

		// Token: 0x04002F40 RID: 12096
		public BSGTerrainTools.Float3D f3d;

		// Token: 0x04002F41 RID: 12097
		public BSGTerrainTools.Gen3D.Func func;

		// Token: 0x020009D2 RID: 2514
		// (Invoke) Token: 0x060054E5 RID: 21733
		public delegate float Func(BSGTerrainTools.Gen3D gen, float wx, float wy, int layer, float src_val, float tgt_val, ref float alpha);
	}

	// Token: 0x0200052E RID: 1326
	public struct SplatPGInput
	{
		// Token: 0x04002F42 RID: 12098
		public Point pos;

		// Token: 0x04002F43 RID: 12099
		public float scale_x;

		// Token: 0x04002F44 RID: 12100
		public float scale_y;

		// Token: 0x04002F45 RID: 12101
		public float sine;

		// Token: 0x04002F46 RID: 12102
		public float cosine;

		// Token: 0x04002F47 RID: 12103
		public float minx;

		// Token: 0x04002F48 RID: 12104
		public float miny;

		// Token: 0x04002F49 RID: 12105
		public unsafe float* input;

		// Token: 0x04002F4A RID: 12106
		public unsafe float* output;

		// Token: 0x04002F4B RID: 12107
		public int width;

		// Token: 0x04002F4C RID: 12108
		public int height;

		// Token: 0x04002F4D RID: 12109
		public int layers;

		// Token: 0x04002F4E RID: 12110
		public int new_width;

		// Token: 0x04002F4F RID: 12111
		public int new_height;

		// Token: 0x04002F50 RID: 12112
		public int new_layers;

		// Token: 0x04002F51 RID: 12113
		public Point alpha_start;

		// Token: 0x04002F52 RID: 12114
		public Point alpha_end;

		// Token: 0x04002F53 RID: 12115
		public float alpha_dist;
	}

	// Token: 0x0200052F RID: 1327
	public struct SplatPGOutput
	{
		// Token: 0x04002F54 RID: 12116
		public Point pos;

		// Token: 0x04002F55 RID: 12117
		public int width;

		// Token: 0x04002F56 RID: 12118
		public int height;

		// Token: 0x04002F57 RID: 12119
		public int layers;

		// Token: 0x04002F58 RID: 12120
		public unsafe float* arr;

		// Token: 0x04002F59 RID: 12121
		public Point alpha_start;

		// Token: 0x04002F5A RID: 12122
		public Point alpha_end;

		// Token: 0x04002F5B RID: 12123
		public float alpha_dist;
	}

	// Token: 0x02000530 RID: 1328
	public struct Concurrent
	{
		// Token: 0x06004340 RID: 17216 RVA: 0x001FC330 File Offset: 0x001FA530
		public Concurrent(NativeMultiHashMap<int2, int> map)
		{
			this.map = map.AsParallelWriter();
		}

		// Token: 0x06004341 RID: 17217 RVA: 0x001FC33F File Offset: 0x001FA53F
		public void Add(int2 coord, int output)
		{
			this.map.Add(coord, output);
		}

		// Token: 0x04002F5C RID: 12124
		private NativeMultiHashMap<int2, int>.ParallelWriter map;
	}

	// Token: 0x02000531 RID: 1329
	public struct ConcurrentOutput
	{
		// Token: 0x06004342 RID: 17218 RVA: 0x001FC34E File Offset: 0x001FA54E
		public ConcurrentOutput(NativeHashMap<int, BSGTerrainTools.SplatPGOutput> map)
		{
			this.map = map.AsParallelWriter();
		}

		// Token: 0x06004343 RID: 17219 RVA: 0x001FC35D File Offset: 0x001FA55D
		public void Add(int coord, BSGTerrainTools.SplatPGOutput output)
		{
			this.map.TryAdd(coord, output);
		}

		// Token: 0x04002F5D RID: 12125
		private NativeHashMap<int, BSGTerrainTools.SplatPGOutput>.ParallelWriter map;
	}

	// Token: 0x02000532 RID: 1330
	[BurstCompile]
	public struct SplatCalculateJob : IJobParallelFor
	{
		// Token: 0x06004344 RID: 17220 RVA: 0x001FC370 File Offset: 0x001FA570
		public int GetOutputLayer(int input_layer)
		{
			if (input_layer == 0)
			{
				return 0;
			}
			for (int i = 0; i < this.remapped_layers.Length; i++)
			{
				int2 @int = this.remapped_layers[i];
				if (@int.x == input_layer)
				{
					return @int.y;
				}
			}
			return -1;
		}

		// Token: 0x06004345 RID: 17221 RVA: 0x001FC3B8 File Offset: 0x001FA5B8
		public void Execute(int index)
		{
			BSGTerrainTools.SplatPGInput splatPGInput = this.pg_input[index];
			BSGTerrainTools.SplatPGOutput splatPGOutput = default(BSGTerrainTools.SplatPGOutput);
			int width = splatPGInput.width;
			int height = splatPGInput.height;
			Point point = new Point(splatPGInput.pos.x - (float)splatPGInput.new_width / 2f, splatPGInput.pos.y - (float)splatPGInput.new_height / 2f);
			Point point2 = new Point(splatPGInput.pos.x + (float)splatPGInput.new_width / 2f, splatPGInput.pos.y + (float)splatPGInput.new_height / 2f);
			splatPGOutput.pos = point;
			splatPGOutput.arr = splatPGInput.output;
			splatPGOutput.width = splatPGInput.new_width;
			splatPGOutput.height = splatPGInput.new_height;
			splatPGOutput.layers = splatPGInput.new_layers;
			splatPGOutput.alpha_dist = splatPGInput.alpha_dist;
			splatPGOutput.alpha_end = splatPGInput.alpha_end;
			splatPGOutput.alpha_start = splatPGInput.alpha_start;
			for (int i = 0; i < splatPGInput.layers; i++)
			{
				int outputLayer = this.GetOutputLayer(i);
				for (int j = 0; j < splatPGOutput.height; j++)
				{
					for (int k = 0; k < splatPGOutput.width; k++)
					{
						float num = (float)k / splatPGInput.scale_x;
						float num2 = (float)j / splatPGInput.scale_y;
						float num3 = (num + splatPGInput.minx) * splatPGInput.cosine + (num2 + splatPGInput.miny) * splatPGInput.sine;
						float num4 = (num2 + splatPGInput.miny) * splatPGInput.cosine - (num + splatPGInput.minx) * splatPGInput.sine;
						int num5 = (int)math.floor(num3);
						int num6 = (int)math.floor(num4);
						int x = num5 + 1;
						int y = num6 + 1;
						float num7 = num3 - (float)num5;
						float num8 = num4 - (float)num6;
						float num9 = 1f - num7;
						float num10 = 1f - num8;
						float pixel = BSGTerrainTools.GetPixel(num5, num6, i, i, splatPGInput.input, width, height, splatPGInput.layers);
						float pixel2 = BSGTerrainTools.GetPixel(x, num6, i, i, splatPGInput.input, width, height, splatPGInput.layers);
						float pixel3 = BSGTerrainTools.GetPixel(num5, y, i, i, splatPGInput.input, width, height, splatPGInput.layers);
						float pixel4 = BSGTerrainTools.GetPixel(x, y, i, i, splatPGInput.input, width, height, splatPGInput.layers);
						float num11 = num9 * pixel + num7 * pixel2;
						float num12 = num9 * pixel3 + num7 * pixel4;
						BSGTerrainTools.SetPixel(num10 * num11 + num8 * num12, k, j, i, outputLayer, splatPGOutput.arr, splatPGOutput.width, splatPGOutput.height, splatPGOutput.layers);
					}
				}
			}
			for (int l = (int)point.x; l < (int)point2.x; l++)
			{
				for (int m = (int)point.y; m < (int)point2.y; m++)
				{
					this.mapIDs.Add(new int2(l, m), index);
				}
			}
			this.mapOutput.Add(index, splatPGOutput);
		}

		// Token: 0x04002F5E RID: 12126
		[ReadOnly]
		public NativeArray<BSGTerrainTools.SplatPGInput> pg_input;

		// Token: 0x04002F5F RID: 12127
		[ReadOnly]
		public NativeArray<int2> remapped_layers;

		// Token: 0x04002F60 RID: 12128
		public BSGTerrainTools.Concurrent mapIDs;

		// Token: 0x04002F61 RID: 12129
		public BSGTerrainTools.ConcurrentOutput mapOutput;
	}

	// Token: 0x02000533 RID: 1331
	[BurstCompile]
	public struct SplatSumJob : IJobParallelFor
	{
		// Token: 0x06004346 RID: 17222 RVA: 0x001FC6D0 File Offset: 0x001FA8D0
		public unsafe void Execute(int index)
		{
			int2 @int = this.map_coords[index];
			int id;
			NativeMultiHashMapIterator<int2> nativeMultiHashMapIterator;
			if (!this.map.TryGetFirstValue(@int, out id, out nativeMultiHashMapIterator))
			{
				return;
			}
			int x = @int.x;
			int y = @int.y;
			if (x < 0 || x >= this.width || y < 0 || y >= this.height)
			{
				return;
			}
			float num = 1f;
			this.ApplySplat(x, y, id, ref num);
			while (this.map.TryGetNextValue(out id, ref nativeMultiHashMapIterator))
			{
				this.ApplySplat(x, y, id, ref num);
			}
			for (int i = 0; i < this.detail_layers; i++)
			{
				int* detailPointer = BSGTerrainTools.getDetailPointer(this.details, x, y, i, this.detail_width, this.detail_height, this.detail_layers);
				int num2 = *detailPointer;
				*detailPointer = (int)((float)num2 * num);
			}
			this.Normalize(x, y);
		}

		// Token: 0x06004347 RID: 17223 RVA: 0x001FC7A8 File Offset: 0x001FA9A8
		private unsafe void ApplySplat(int x, int y, int id, ref float total_source_transparency)
		{
			BSGTerrainTools.SplatPGOutput splatPGOutput = this.mapOutput[id];
			int num = x - (int)splatPGOutput.pos.x;
			int num2 = y - (int)splatPGOutput.pos.y;
			if (num < 0 || num >= splatPGOutput.width || num2 < 0 || num2 >= splatPGOutput.height)
			{
				return;
			}
			float* pointer = BSGTerrainTools.getPointer(splatPGOutput.arr, 0, 0, 0, splatPGOutput.width, splatPGOutput.height, splatPGOutput.layers);
			float* pointer2 = BSGTerrainTools.getPointer(splatPGOutput.arr, num, num2, 0, splatPGOutput.width, splatPGOutput.height, splatPGOutput.layers);
			float num3 = *pointer2;
			long num4 = (long)(pointer2 - pointer);
			float num5 = 1f;
			if (splatPGOutput.alpha_dist > 0f)
			{
				Point point = new Point((float)x, (float)y);
				float a = point.Dist(splatPGOutput.alpha_start);
				float b = point.Dist(splatPGOutput.alpha_end);
				num5 = math.clamp(Mathf.Min(a, b) / splatPGOutput.alpha_dist, 0f, 1f);
			}
			for (int i = 0; i < this.layers; i++)
			{
				if (num4 != (long)i)
				{
					float num6 = *BSGTerrainTools.getPointer(splatPGOutput.arr, num, num2, i, splatPGOutput.width, splatPGOutput.height, splatPGOutput.layers);
					float* pointer3 = BSGTerrainTools.getPointer(this.splats, x, y, i, this.width, this.height, this.layers);
					float x2 = *pointer3;
					float num7;
					if (this.transparent)
					{
						num7 = math.lerp(x2, num6, num5 * (1f - num3));
					}
					else
					{
						num7 = num6;
					}
					*pointer3 = num7;
				}
			}
			if (num3 < total_source_transparency)
			{
				total_source_transparency = num3;
			}
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x001FC93C File Offset: 0x001FAB3C
		private unsafe void Normalize(int x, int y)
		{
			float num = 0f;
			for (int i = 0; i < this.layers; i++)
			{
				num += *BSGTerrainTools.getPointer(this.splats, x, y, i, this.width, this.height, this.layers);
			}
			if (num == 1f)
			{
				return;
			}
			if (num == 0f)
			{
				*BSGTerrainTools.getPointer(this.splats, x, y, 0, this.width, this.height, this.layers) = 1f;
				return;
			}
			float num2 = 1f / num;
			for (int j = 0; j < this.layers; j++)
			{
				*BSGTerrainTools.getPointer(this.splats, x, y, j, this.width, this.height, this.layers) *= num2;
			}
		}

		// Token: 0x04002F62 RID: 12130
		[ReadOnly]
		public NativeMultiHashMap<int2, int> map;

		// Token: 0x04002F63 RID: 12131
		[ReadOnly]
		public NativeHashMap<int, BSGTerrainTools.SplatPGOutput> mapOutput;

		// Token: 0x04002F64 RID: 12132
		[ReadOnly]
		public NativeArray<int2> map_coords;

		// Token: 0x04002F65 RID: 12133
		[NativeDisableUnsafePtrRestriction]
		public unsafe float* splats;

		// Token: 0x04002F66 RID: 12134
		[NativeDisableUnsafePtrRestriction]
		public unsafe int* details;

		// Token: 0x04002F67 RID: 12135
		public int width;

		// Token: 0x04002F68 RID: 12136
		public int height;

		// Token: 0x04002F69 RID: 12137
		public int layers;

		// Token: 0x04002F6A RID: 12138
		public int detail_width;

		// Token: 0x04002F6B RID: 12139
		public int detail_height;

		// Token: 0x04002F6C RID: 12140
		public int detail_layers;

		// Token: 0x04002F6D RID: 12141
		public bool transparent;
	}

	// Token: 0x02000534 RID: 1332
	public class PerTerrainInfo
	{
		// Token: 0x04002F6E RID: 12142
		public TerrainData td;

		// Token: 0x04002F6F RID: 12143
		public Terrain t;

		// Token: 0x04002F70 RID: 12144
		public BSGTerrainTools.PerTerrainInfo.PerCellInfo[,] cells;

		// Token: 0x020009D3 RID: 2515
		public class PerCellInfo
		{
			// Token: 0x0400455C RID: 17756
			public BSGTerrainTools.Float2D heights;

			// Token: 0x0400455D RID: 17757
			public BSGTerrainTools.Float3D splats;

			// Token: 0x0400455E RID: 17758
			public List<TreeInstance> trees;

			// Token: 0x0400455F RID: 17759
			public BSGTerrainTools.Float2D bg_heights;

			// Token: 0x04004560 RID: 17760
			public BSGTerrainTools.Float3D bg_splats;

			// Token: 0x04004561 RID: 17761
			public List<TreeInstance> bg_trees;
		}
	}

	// Token: 0x02000535 RID: 1333
	public class TerrainRemap
	{
		// Token: 0x0600434A RID: 17226 RVA: 0x001FC9FC File Offset: 0x001FABFC
		public TerrainRemap(TerrainData src, TerrainData dst)
		{
			this.src = src;
			this.dst = dst;
			this.remapped_layers = new Dictionary<int, int>();
			this.remapped_trees = new Dictionary<int, int>();
			TerrainLayer[] terrainLayers = this.src.terrainLayers;
			TerrainLayer[] terrainLayers2 = this.dst.terrainLayers;
			TreePrototype[] treePrototypes = this.src.treePrototypes;
			TreePrototype[] treePrototypes2 = this.dst.treePrototypes;
			List<ValueTuple<int, TerrainLayer>> list = new List<ValueTuple<int, TerrainLayer>>();
			List<ValueTuple<int, TreePrototype>> list2 = new List<ValueTuple<int, TreePrototype>>();
			for (int i = 1; i < terrainLayers.Length; i++)
			{
				TerrainLayer terrainLayer = terrainLayers[i];
				bool flag = false;
				for (int j = 1; j < terrainLayers2.Length; j++)
				{
					TerrainLayer terrainLayer2 = terrainLayers2[j];
					if (terrainLayer2 == terrainLayer)
					{
						this.remapped_layers[i] = j;
						flag = true;
						break;
					}
					if (terrainLayer2.diffuseTexture == terrainLayer.diffuseTexture)
					{
						Debug.LogWarning("Same textures in different layers " + terrainLayer2.name + " and " + terrainLayer.name);
					}
				}
				if (!flag)
				{
					list.Add(new ValueTuple<int, TerrainLayer>(i, terrainLayer));
				}
			}
			TerrainLayer[] array = new TerrainLayer[terrainLayers2.Length + list.Count];
			for (int k = 0; k < terrainLayers2.Length; k++)
			{
				array[k] = terrainLayers2[k];
			}
			for (int l = terrainLayers2.Length; l < array.Length; l++)
			{
				ValueTuple<int, TerrainLayer> valueTuple = list[l - terrainLayers2.Length];
				array[l] = valueTuple.Item2;
				this.remapped_layers[valueTuple.Item1] = l;
			}
			for (int m = 0; m < treePrototypes.Length; m++)
			{
				TreePrototype treePrototype = treePrototypes[m];
				bool flag2 = false;
				for (int n = 0; n < treePrototypes2.Length; n++)
				{
					if (treePrototypes2[n].prefab == treePrototype.prefab)
					{
						this.remapped_trees[m] = n;
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					list2.Add(new ValueTuple<int, TreePrototype>(m, treePrototype));
				}
			}
			TreePrototype[] array2 = new TreePrototype[treePrototypes2.Length + list2.Count];
			for (int num = 0; num < treePrototypes2.Length; num++)
			{
				array2[num] = treePrototypes2[num];
			}
			for (int num2 = treePrototypes2.Length; num2 < array2.Length; num2++)
			{
				ValueTuple<int, TreePrototype> valueTuple2 = list2[num2 - treePrototypes2.Length];
				array2[num2] = valueTuple2.Item2;
				this.remapped_trees[valueTuple2.Item1] = num2;
			}
			dst.terrainLayers = array;
			dst.treePrototypes = array2;
		}

		// Token: 0x04002F71 RID: 12145
		public TerrainData src;

		// Token: 0x04002F72 RID: 12146
		public TerrainData dst;

		// Token: 0x04002F73 RID: 12147
		public Dictionary<int, int> remapped_layers;

		// Token: 0x04002F74 RID: 12148
		public Dictionary<int, int> remapped_trees;
	}
}
