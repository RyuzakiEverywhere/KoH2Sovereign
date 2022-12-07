using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// Token: 0x020000A8 RID: 168
[Serializable]
public struct BSGTerrainData
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x00040A0C File Offset: 0x0003EC0C
	public BSGTerrainChunk GetChunk(int x, int y)
	{
		int num = Mathf.RoundToInt(Mathf.Sqrt((float)this.chunks.Length));
		return this.chunks[y * num + x];
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00040A40 File Offset: 0x0003EC40
	public static BSGTerrainData FromUnityTerrain(Terrain terrain, int alphamap_cell_resolution, int target_texture_resolution, bool compress_textures)
	{
		BSGTerrainData result = BSGTerrainData.FromUnityTerrain(terrain, alphamap_cell_resolution);
		BSGTerrainData.CreateTerrainTextureArrays(terrain, target_texture_resolution, compress_textures, out result.diffuse_array, out result.normal_array);
		return result;
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00040A6C File Offset: 0x0003EC6C
	public static BSGTerrainData FromUnityTerrain(Terrain terrain, int alphamap_cell_resolution, BSGTerrainTextureBindings diffuse_binder, BSGTerrainTextureBindings normal_binder)
	{
		BSGTerrainData result = BSGTerrainData.FromUnityTerrain(terrain, alphamap_cell_resolution);
		BSGTerrainData.CreateTerrainTextureArrays(terrain, diffuse_binder, normal_binder, out result.diffuse_array, out result.normal_array);
		return result;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00040A98 File Offset: 0x0003EC98
	private static BSGTerrainData FromUnityTerrain(Terrain terrain, int alphamap_cell_resolution)
	{
		BSGTerrainData.<>c__DisplayClass11_0 CS$<>8__locals1 = new BSGTerrainData.<>c__DisplayClass11_0();
		CS$<>8__locals1.alphamap_cell_resolution = alphamap_cell_resolution;
		Stopwatch stopwatch = Stopwatch.StartNew();
		Debug.Log("Started terrain processing...");
		CS$<>8__locals1.terrain_data = terrain.terrainData;
		BSGTerrainData bsgterrainData = default(BSGTerrainData);
		bsgterrainData.bounds = CS$<>8__locals1.terrain_data.bounds;
		bsgterrainData.alphamap_cell_resolution = CS$<>8__locals1.alphamap_cell_resolution;
		bsgterrainData.texture_properties = (from l in CS$<>8__locals1.terrain_data.terrainLayers
		select TextureProperties.FromTerrainLayer(l)).ToArray<TextureProperties>();
		CS$<>8__locals1.terrain_alphamap_resolution = CS$<>8__locals1.terrain_data.alphamapResolution;
		CS$<>8__locals1.cell_single_dimension_count = CS$<>8__locals1.terrain_alphamap_resolution / CS$<>8__locals1.alphamap_cell_resolution;
		bsgterrainData.cells_count_in_single_dimension = CS$<>8__locals1.cell_single_dimension_count;
		int num = CS$<>8__locals1.cell_single_dimension_count / 16;
		RectInt alphamapRect = new RectInt(Vector2Int.zero, Vector2Int.one * CS$<>8__locals1.terrain_data.alphamapResolution - Vector2Int.one);
		CS$<>8__locals1.fallback_alphamap = new Texture2D(CS$<>8__locals1.terrain_alphamap_resolution, CS$<>8__locals1.terrain_alphamap_resolution, GraphicsFormat.R8G8B8A8_UNorm, 0, TextureCreationFlags.None);
		Color32[] pixels = new Color32[CS$<>8__locals1.terrain_alphamap_resolution * CS$<>8__locals1.terrain_alphamap_resolution];
		CS$<>8__locals1.fallback_alphamap.SetPixels32(pixels);
		CS$<>8__locals1.fallback_alphamap.Apply(false, false);
		CS$<>8__locals1.terrain_splats = new List<Color32[]>();
		for (int i = 0; i < CS$<>8__locals1.terrain_data.alphamapTextureCount; i++)
		{
			CS$<>8__locals1.terrain_splats.Add(CS$<>8__locals1.<FromUnityTerrain>g__GetAlphamapTexture|1(i).GetPixels32());
		}
		List<Texture2D> list = new List<Texture2D>();
		for (int j = 0; j < CS$<>8__locals1.terrain_data.alphamapTextureCount; j++)
		{
			list.Add(BSGTerrainData.GenerateMaxsplat(CS$<>8__locals1.<FromUnityTerrain>g__GetAlphamapTexture|1(j), CS$<>8__locals1.alphamap_cell_resolution));
		}
		List<Color32[]> list2 = new List<Color32[]>();
		for (int k = 0; k < list.Count; k++)
		{
			list2.Add(list[k].GetPixels32());
		}
		int num2 = list2[0].Length;
		CS$<>8__locals1.maxsplat_pixel_datas = new AlphamapPixelData[num2][];
		for (int l2 = 0; l2 < num2; l2++)
		{
			CS$<>8__locals1.maxsplat_pixel_datas[l2] = new AlphamapPixelData[list.Count * 4];
			int num3 = 0;
			for (int m = 0; m < list.Count; m++)
			{
				Color32 color = list2[m][l2];
				CS$<>8__locals1.maxsplat_pixel_datas[l2][m * 4] = new AlphamapPixelData(color.r, m, 0, m * 4);
				CS$<>8__locals1.maxsplat_pixel_datas[l2][m * 4 + 1] = new AlphamapPixelData(color.g, m, 1, m * 4 + 1);
				CS$<>8__locals1.maxsplat_pixel_datas[l2][m * 4 + 2] = new AlphamapPixelData(color.b, m, 2, m * 4 + 2);
				CS$<>8__locals1.maxsplat_pixel_datas[l2][m * 4 + 3] = new AlphamapPixelData(color.a, m, 3, m * 4 + 3);
				num3++;
			}
			Array.Sort<AlphamapPixelData>(CS$<>8__locals1.maxsplat_pixel_datas[l2], (AlphamapPixelData a, AlphamapPixelData b) => (int)(b.value - a.value));
		}
		Common.DestroyObj(CS$<>8__locals1.fallback_alphamap);
		Parallel.For(0, CS$<>8__locals1.cell_single_dimension_count, new ParallelOptions
		{
			MaxDegreeOfParallelism = 4
		}, delegate(int cellY)
		{
			for (int num6 = 0; num6 < CS$<>8__locals1.cell_single_dimension_count; num6++)
			{
				Vector2Int a = new Vector2Int(CS$<>8__locals1.alphamap_cell_resolution * num6, CS$<>8__locals1.alphamap_cell_resolution * cellY);
				Vector2Int vector2Int = a + Vector2Int.one * (CS$<>8__locals1.alphamap_cell_resolution - 1);
				BSGTerrainData.<>c__DisplayClass11_1 CS$<>8__locals5;
				CS$<>8__locals5.cell_channels = base.<FromUnityTerrain>g__GetAlphamapDataFromCell|2(num6, cellY);
				AlphamapPixelData[] neighbouring_channels = base.<FromUnityTerrain>g__GetAlphamapDataFromCell|2(num6, cellY + 1);
				AlphamapPixelData[] neighbouring_channels2 = base.<FromUnityTerrain>g__GetAlphamapDataFromCell|2(num6, cellY - 1);
				AlphamapPixelData[] neighbouring_channels3 = base.<FromUnityTerrain>g__GetAlphamapDataFromCell|2(num6 - 1, cellY);
				AlphamapPixelData[] neighbouring_channels4 = base.<FromUnityTerrain>g__GetAlphamapDataFromCell|2(num6 + 1, cellY);
				base.<FromUnityTerrain>g__DiscardPixels|9(neighbouring_channels, new Vector2Int(a.x, vector2Int.y), new Vector2Int(vector2Int.x, vector2Int.y + 1), ref CS$<>8__locals5);
				base.<FromUnityTerrain>g__DiscardPixels|9(neighbouring_channels2, new Vector2Int(a.x, a.y - 1), new Vector2Int(vector2Int.x, a.y), ref CS$<>8__locals5);
				base.<FromUnityTerrain>g__DiscardPixels|9(neighbouring_channels4, new Vector2Int(vector2Int.x, a.y), new Vector2Int(vector2Int.x + 1, vector2Int.y), ref CS$<>8__locals5);
				base.<FromUnityTerrain>g__DiscardPixels|9(neighbouring_channels3, new Vector2Int(a.x - 1, a.y), new Vector2Int(a.x, vector2Int.y), ref CS$<>8__locals5);
			}
		});
		bsgterrainData.chunks = new BSGTerrainChunk[num * num];
		CS$<>8__locals1.alphamap_resolution = CS$<>8__locals1.terrain_data.alphamapResolution;
		int chunk_z;
		int num5;
		for (chunk_z = 0; chunk_z < num; chunk_z = num5 + 1)
		{
			int chunk_x;
			for (chunk_x = 0; chunk_x < num; chunk_x = num5 + 1)
			{
				BSGTerrainChunk chunk = default(BSGTerrainChunk);
				chunk.bounds = BSGTerrainChunk.CalculateChunkBounds(CS$<>8__locals1.terrain_data.bounds, num, chunk_z, chunk_x);
				chunk.boundingSphere = new BoundingSphere(chunk.bounds.center, chunk.bounds.extents.magnitude);
				chunk.alphamap_bounds = BSGTerrainChunk.CalculateChunkAlphamapBounds(alphamapRect, num, chunk_z, chunk_x);
				int depth = 256;
				chunk.primary_alphamap_array = new Texture2DArray(CS$<>8__locals1.alphamap_cell_resolution + 2, CS$<>8__locals1.alphamap_cell_resolution + 2, depth, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None, 0);
				chunk.primary_alphamap_array.filterMode = FilterMode.Bilinear;
				chunk.primary_alphamap_array.wrapMode = TextureWrapMode.Clamp;
				chunk.secondary_alphamap_array = new Texture2DArray(CS$<>8__locals1.alphamap_cell_resolution + 2, CS$<>8__locals1.alphamap_cell_resolution + 2, depth, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None, 0);
				chunk.secondary_alphamap_array.filterMode = FilterMode.Bilinear;
				chunk.secondary_alphamap_array.wrapMode = TextureWrapMode.Clamp;
				chunk.cells = new BSGTerrainCell[256];
				Color32[,][] primary_splat_texels_array = new Color32[16, 16][];
				Color32[,][] secondary_splat_texels_array = new Color32[16, 16][];
				Parallel.For(0, 16, new ParallelOptions
				{
					MaxDegreeOfParallelism = 4
				}, delegate(int cellZ)
				{
					for (int num6 = 0; num6 < 16; num6++)
					{
						BSGTerrainCell bsgterrainCell = default(BSGTerrainCell);
						bsgterrainCell.bounds = BSGTerrainCell.CalculateCellBounds(chunk.bounds, 16, cellZ, num6);
						bsgterrainCell.object_to_world_matrix = Matrix4x4.TRS(bsgterrainCell.bounds.center, Quaternion.identity, bsgterrainCell.bounds.extents);
						bsgterrainCell.alphamap_bouds = BSGTerrainCell.CalculateCellAlphamapBounds(chunk.alphamap_bounds, 16, cellZ, num6);
						bsgterrainCell.alphamap_index = cellZ * 16 + num6;
						Vector2Int min = bsgterrainCell.alphamap_bouds.min;
						Vector2Int vector2Int = new Vector2Int(chunk_x * 16 + num6, chunk_z * 16 + cellZ);
						int num7 = vector2Int.y * CS$<>8__locals1.cell_single_dimension_count + vector2Int.x;
						bsgterrainCell.primaryMappings.x = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][0].texture_layer_index;
						bsgterrainCell.primaryMappings.y = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][1].texture_layer_index;
						bsgterrainCell.primaryMappings.z = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][2].texture_layer_index;
						bsgterrainCell.primaryMappings.w = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][3].texture_layer_index;
						bsgterrainCell.secondaryMappings.x = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][4].texture_layer_index;
						bsgterrainCell.secondaryMappings.y = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][5].texture_layer_index;
						bsgterrainCell.secondaryMappings.z = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][6].texture_layer_index;
						bsgterrainCell.secondaryMappings.w = (float)CS$<>8__locals1.maxsplat_pixel_datas[num7][7].texture_layer_index;
						AlphamapPixelData pixel_data = CS$<>8__locals1.maxsplat_pixel_datas[num7][0];
						AlphamapPixelData pixel_data2 = CS$<>8__locals1.maxsplat_pixel_datas[num7][1];
						AlphamapPixelData pixel_data3 = CS$<>8__locals1.maxsplat_pixel_datas[num7][2];
						AlphamapPixelData pixel_data4 = CS$<>8__locals1.maxsplat_pixel_datas[num7][3];
						AlphamapPixelData pixel_data5 = CS$<>8__locals1.maxsplat_pixel_datas[num7][4];
						AlphamapPixelData pixel_data6 = CS$<>8__locals1.maxsplat_pixel_datas[num7][5];
						AlphamapPixelData pixel_data7 = CS$<>8__locals1.maxsplat_pixel_datas[num7][6];
						AlphamapPixelData pixel_data8 = CS$<>8__locals1.maxsplat_pixel_datas[num7][7];
						int num8 = CS$<>8__locals1.alphamap_cell_resolution + 2;
						Color32[] array = new Color32[num8 * num8];
						Color32[] array2 = new Color32[num8 * num8];
						for (int num9 = -1; num9 < CS$<>8__locals1.alphamap_cell_resolution + 1; num9++)
						{
							for (int num10 = -1; num10 < CS$<>8__locals1.alphamap_cell_resolution + 1; num10++)
							{
								Color32 color2 = default(Color32);
								Color32 color3 = default(Color32);
								Vector2Int vector2Int2 = new Vector2Int(min.x + num10, min.y + num9);
								vector2Int2.x = Mathf.Clamp(vector2Int2.x, 0, CS$<>8__locals1.alphamap_resolution - 1);
								vector2Int2.y = Mathf.Clamp(vector2Int2.y, 0, CS$<>8__locals1.alphamap_resolution - 1);
								int splat_texel_index = vector2Int2.y * CS$<>8__locals1.alphamap_resolution + vector2Int2.x;
								color2.r = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data);
								color2.g = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data2);
								color2.b = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data3);
								color2.a = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data4);
								color3.r = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data5);
								color3.g = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data6);
								color3.b = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data7);
								color3.a = CS$<>8__locals1.<FromUnityTerrain>g__GetChannel|12(splat_texel_index, pixel_data8);
								int num11 = num9 + 1;
								int num12 = num10 + 1;
								array[num11 * num8 + num12] = color2;
								array2[num11 * num8 + num12] = color3;
							}
						}
						primary_splat_texels_array[cellZ, num6] = array;
						secondary_splat_texels_array[cellZ, num6] = array2;
						chunk.cells[bsgterrainCell.alphamap_index] = bsgterrainCell;
					}
				});
				for (int n = 0; n < 16; n++)
				{
					for (int num4 = 0; num4 < 16; num4++)
					{
						chunk.primary_alphamap_array.SetPixels32(primary_splat_texels_array[n, num4], n * 16 + num4, 0);
						chunk.secondary_alphamap_array.SetPixels32(secondary_splat_texels_array[n, num4], n * 16 + num4, 0);
					}
				}
				chunk.primary_alphamap_array.Apply(false, true);
				chunk.secondary_alphamap_array.Apply(false, true);
				chunk.bakedRenderData = BSGTerrainChunkRenderData.FromChunk(chunk);
				chunk.native_cells_array = new NativeArray<BSGTerrainCell>(chunk.cells, Allocator.Persistent);
				bsgterrainData.chunks[chunk_z * num + chunk_x] = chunk;
				num5 = chunk_x;
			}
			num5 = chunk_z;
		}
		foreach (Texture2D obj in list)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(obj);
			}
			else
			{
				Object.DestroyImmediate(obj);
			}
		}
		Debug.Log(string.Format("Finished terrain processing: {0}ms", stopwatch.Elapsed.TotalMilliseconds));
		stopwatch.Stop();
		return bsgterrainData;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0004122C File Offset: 0x0003F42C
	public void Cleanup()
	{
		if (Application.isPlaying)
		{
			if (this.normal_array != null)
			{
				Object.Destroy(this.normal_array);
			}
			if (this.diffuse_array != null)
			{
				Object.Destroy(this.diffuse_array);
			}
		}
		else
		{
			if (this.normal_array != null)
			{
				Object.DestroyImmediate(this.normal_array);
			}
			if (this.diffuse_array != null)
			{
				Object.DestroyImmediate(this.diffuse_array);
			}
		}
		foreach (BSGTerrainChunk bsgterrainChunk in this.chunks)
		{
			bsgterrainChunk.Cleanup();
		}
		this.texture_properties = null;
		this.chunks = null;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x000412D8 File Offset: 0x0003F4D8
	private static Texture2D GenerateMaxsplat(Texture2D alphamap, int alphamap_cell_resolution)
	{
		int num = alphamap.width / alphamap_cell_resolution;
		int num2 = alphamap.height / alphamap_cell_resolution;
		Texture2D texture2D = new Texture2D(num, num2, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		RenderTexture renderTexture = new RenderTexture(num, num2, 0, GraphicsFormat.R8G8B8A8_UNorm, 0);
		renderTexture.Create();
		Shader.SetGlobalInt("_AlphamapCellResolution", alphamap_cell_resolution);
		Graphics.Blit(alphamap, renderTexture, BSGTerrain.Resources.max_splat_material);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		renderTexture.Release();
		if (Application.isPlaying)
		{
			Object.Destroy(renderTexture);
			return texture2D;
		}
		Object.DestroyImmediate(renderTexture);
		return texture2D;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00041378 File Offset: 0x0003F578
	public static void CreateTerrainTextureArrays(Terrain terrain, int target_resolution, bool compress_textures, out Texture2DArray diffuse_array, out Texture2DArray normal_array)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<Texture2D> terrainDiffuseTextures = BSGTerrainData.GetTerrainDiffuseTextures(terrain.terrainData);
		List<Texture2D> terrainNormalTextures = BSGTerrainData.GetTerrainNormalTextures(terrain.terrainData);
		diffuse_array = BSGTerrainData.ConvertToTexture2DArray(terrainDiffuseTextures, target_resolution, compress_textures);
		normal_array = BSGTerrainData.ConvertToTexture2DArray(terrainNormalTextures, target_resolution, compress_textures);
		Debug.Log(string.Format("Creating terrain diffuse and normals array: {0}", stopwatch.Elapsed.TotalMilliseconds));
		stopwatch.Stop();
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x000413E0 File Offset: 0x0003F5E0
	public static void CreateTerrainTextureArrays(Terrain terrain, BSGTerrainTextureBindings diffuse_binder, BSGTerrainTextureBindings normal_binder, out Texture2DArray diffuse_array, out Texture2DArray normal_array)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<Texture2D> terrainDiffuseTextures = BSGTerrainData.GetTerrainDiffuseTextures(terrain.terrainData);
		List<Texture2D> terrainNormalTextures = BSGTerrainData.GetTerrainNormalTextures(terrain.terrainData);
		diffuse_array = BSGTerrainData.ConvertToTexture2DArray(terrainDiffuseTextures, diffuse_binder);
		normal_array = BSGTerrainData.ConvertToTexture2DArray(terrainNormalTextures, normal_binder);
		Debug.Log(string.Format("Creating terrain diffuse and normals array: {0}", stopwatch.Elapsed.TotalMilliseconds));
		stopwatch.Stop();
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00041448 File Offset: 0x0003F648
	private static List<Texture2D> GetTerrainDiffuseTextures(TerrainData terrain_data)
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < terrain_data.terrainLayers.Length; i++)
		{
			Texture2D diffuseTexture = terrain_data.terrainLayers[i].diffuseTexture;
			list.Add(diffuseTexture);
		}
		return list;
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00041484 File Offset: 0x0003F684
	private static List<Texture2D> GetTerrainNormalTextures(TerrainData terrain_data)
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < terrain_data.terrainLayers.Length; i++)
		{
			Texture2D texture2D = terrain_data.terrainLayers[i].normalMapTexture;
			if (texture2D == null)
			{
				texture2D = BSGTerrain.Resources.default_normal_texture;
			}
			list.Add(texture2D);
		}
		return list;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x000414D4 File Offset: 0x0003F6D4
	private static Texture2DArray ConvertToTexture2DArray(List<Texture2D> textures, int target_resolution, bool compress_textures)
	{
		bool sRGB = textures[0].graphicsFormat.ToString().ToLowerInvariant().Contains("srgb");
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < textures.Count; i++)
		{
			Texture2D texture2D = BSGTerrainData.ResizeTexture(textures[i], target_resolution, sRGB);
			if (compress_textures)
			{
				texture2D.Compress(false);
			}
			list.Add(texture2D);
		}
		Texture2DArray texture2DArray = new Texture2DArray(target_resolution, target_resolution, textures.Count, list[0].graphicsFormat, TextureCreationFlags.MipChain, list[0].mipmapCount);
		for (int j = 0; j < list.Count; j++)
		{
			Graphics.CopyTexture(list[j], 0, texture2DArray, j);
		}
		texture2DArray.Apply(false, true);
		foreach (Texture2D unity_object in list)
		{
			BSGTerrainData.DestroyIfNotNull(unity_object);
		}
		list.Clear();
		return texture2DArray;
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x000415E8 File Offset: 0x0003F7E8
	private static Texture2DArray ConvertToTexture2DArray(List<Texture2D> textures, BSGTerrainTextureBindings binder)
	{
		int width = binder.texture_bindings[0].texture.width;
		GraphicsFormat graphicsFormat = binder.texture_bindings[0].texture.graphicsFormat;
		int mipmapCount = binder.texture_bindings[0].texture.mipmapCount;
		Texture2DArray texture2DArray = new Texture2DArray(width, width, textures.Count, graphicsFormat, TextureCreationFlags.MipChain, mipmapCount);
		for (int i = 0; i < textures.Count; i++)
		{
			Texture2D texture2D = binder.GetTextureBinding(textures[i]);
			if (texture2D == null)
			{
				texture2D = binder.fallback_texture;
				Debug.LogWarning("No texture binding for texture with name " + textures[i].name + ", using fallback", textures[i]);
			}
			Graphics.CopyTexture(texture2D, 0, texture2DArray, i);
		}
		texture2DArray.Apply(false, true);
		return texture2DArray;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x000416B8 File Offset: 0x0003F8B8
	private static Texture2D ResizeTexture(Texture2D source, int resolution, bool sRGB)
	{
		RenderTexture renderTexture = new RenderTexture(new RenderTextureDescriptor
		{
			width = resolution,
			height = resolution,
			depthBufferBits = 0,
			dimension = TextureDimension.Tex2D,
			graphicsFormat = (sRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm),
			colorFormat = RenderTextureFormat.ARGB32,
			volumeDepth = 1,
			msaaSamples = 1
		});
		renderTexture.DiscardContents();
		Graphics.Blit(source, renderTexture);
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(resolution, resolution, sRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.MipChain);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
		texture2D.Apply(true, false);
		RenderTexture.active = null;
		renderTexture.Release();
		Object.DestroyImmediate(renderTexture);
		return texture2D;
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0003FFEE File Offset: 0x0003E1EE
	private static void DestroyIfNotNull(Object unity_object)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(unity_object);
			return;
		}
		Object.DestroyImmediate(unity_object);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0004177B File Offset: 0x0003F97B
	[CompilerGenerated]
	internal static void <FromUnityTerrain>g__RemoveColorChannel|11_5(ref Color32 color, int channel)
	{
		switch (channel)
		{
		case 0:
			color.r = 0;
			return;
		case 1:
			color.g = 0;
			return;
		case 2:
			color.b = 0;
			return;
		case 3:
			color.a = 0;
			return;
		default:
			return;
		}
	}

	// Token: 0x04000588 RID: 1416
	public Bounds bounds;

	// Token: 0x04000589 RID: 1417
	public BSGTerrainChunk[] chunks;

	// Token: 0x0400058A RID: 1418
	public Texture2DArray diffuse_array;

	// Token: 0x0400058B RID: 1419
	public Texture2DArray normal_array;

	// Token: 0x0400058C RID: 1420
	public TextureProperties[] texture_properties;

	// Token: 0x0400058D RID: 1421
	public int alphamap_cell_resolution;

	// Token: 0x0400058E RID: 1422
	public int cells_count_in_single_dimension;

	// Token: 0x0400058F RID: 1423
	public int heightmap_cell_resolution;
}
