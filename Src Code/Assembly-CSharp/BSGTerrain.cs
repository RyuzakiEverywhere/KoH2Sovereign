using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A4 RID: 164
[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
public class BSGTerrain : MonoBehaviour
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060005CE RID: 1486 RVA: 0x0003F76B File Offset: 0x0003D96B
	public static BSGTerrainResources Resources
	{
		get
		{
			BSGTerrainResources result;
			if ((result = BSGTerrain.resources) == null)
			{
				result = (BSGTerrain.resources = UnityEngine.Resources.Load<BSGTerrainResources>("BSGTerrainResources"));
			}
			return result;
		}
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x0003F788 File Offset: 0x0003D988
	public static BSGTerrain ConvertUnityTerrain(Terrain unity_terrain, int alphamap_cell_resolution = 16, int texture_resolution = 1024, bool compress_textures = false)
	{
		BSGTerrain bsgterrain = unity_terrain.GetComponent<BSGTerrain>();
		if (bsgterrain == null)
		{
			bsgterrain = unity_terrain.gameObject.AddComponent<BSGTerrain>();
		}
		bsgterrain.unity_terrain = unity_terrain;
		bsgterrain.alphamap_cell_resolution = alphamap_cell_resolution;
		bsgterrain.texture_resolution = texture_resolution;
		bsgterrain.compress_textures = compress_textures;
		bsgterrain.ConvertTerrain(null, null);
		return bsgterrain;
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x0003F7D8 File Offset: 0x0003D9D8
	public static BSGTerrain ConvertUnityTerrain(Terrain unity_terrain, BSGTerrainTextureBindings diffuse_binder, BSGTerrainTextureBindings normal_binder, int alphamap_cell_resolution = 16)
	{
		BSGTerrain bsgterrain = unity_terrain.GetComponent<BSGTerrain>();
		if (bsgterrain == null)
		{
			bsgterrain = unity_terrain.gameObject.AddComponent<BSGTerrain>();
		}
		bsgterrain.unity_terrain = unity_terrain;
		bsgterrain.alphamap_cell_resolution = alphamap_cell_resolution;
		bsgterrain.texture_resolution = diffuse_binder.resolution;
		bsgterrain.ConvertTerrain(diffuse_binder, normal_binder);
		return bsgterrain;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x0003F824 File Offset: 0x0003DA24
	[ContextMenu("Convert terrain")]
	public void ConvertTerrain(BSGTerrainTextureBindings diffuse_binder = null, BSGTerrainTextureBindings normal_binder = null)
	{
		if (this.unity_terrain == null)
		{
			this.unity_terrain = base.GetComponent<Terrain>();
		}
		if (this.terrain_data != null)
		{
			this.terrain_data.Value.Cleanup();
		}
		BSGTerrain.Resources.terrain_material.renderQueue = this.unity_terrain.materialTemplate.renderQueue;
		if (diffuse_binder == null || normal_binder == null)
		{
			this.terrain_data = new BSGTerrainData?(BSGTerrainData.FromUnityTerrain(this.unity_terrain, this.alphamap_cell_resolution, this.texture_resolution, this.compress_textures));
		}
		else
		{
			this.terrain_data = new BSGTerrainData?(BSGTerrainData.FromUnityTerrain(this.unity_terrain, this.alphamap_cell_resolution, diffuse_binder, normal_binder));
		}
		this.SetMeshQuality((float)Mathf.RoundToInt((float)this.alphamap_cell_resolution * ((float)(this.unity_terrain.terrainData.heightmapResolution - 1) / (float)this.unity_terrain.terrainData.alphamapResolution)));
		this.unity_terrain.drawHeightmap = false;
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0003F929 File Offset: 0x0003DB29
	public static void EnableNormalTextures()
	{
		Shader.DisableKeyword("BSGTERRAIN_DISABLE_NORMALS");
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x0003F935 File Offset: 0x0003DB35
	public static void EnableChunkBorders()
	{
		Shader.EnableKeyword("BSGTERRAIN_DISPLAY_CHUNK_BORDERS");
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0003F941 File Offset: 0x0003DB41
	public static void DisableChunkBorders()
	{
		Shader.DisableKeyword("BSGTERRAIN_DISPLAY_CHUNK_BORDERS");
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0003F94D File Offset: 0x0003DB4D
	public static void DisableNormalTextures()
	{
		Shader.EnableKeyword("BSGTERRAIN_DISABLE_NORMALS");
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x0003F959 File Offset: 0x0003DB59
	public static void SetNumberOfTextures(BSGTerrain.NumberOfTextures numberOfTextures)
	{
		Shader.DisableKeyword("BSGTERRAIN_USE_3_TEXTURES");
		if (numberOfTextures == BSGTerrain.NumberOfTextures._3)
		{
			Shader.EnableKeyword("BSGTERRAIN_USE_3_TEXTURES");
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0003F974 File Offset: 0x0003DB74
	public void SetMeshQuality(float mesh_quality)
	{
		mesh_quality = Mathf.Clamp01(mesh_quality);
		int heightmap_cell_resolution = Mathf.RoundToInt((float)(this.alphamap_cell_resolution * ((this.unity_terrain.terrainData.heightmapResolution - 1) / this.unity_terrain.terrainData.alphamapResolution)) * mesh_quality);
		if (this.cell_mesh != null)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this.cell_mesh);
			}
			else
			{
				Object.DestroyImmediate(this.cell_mesh);
			}
		}
		this.cell_mesh = BSGTerrain.CreatePlaneMesh(heightmap_cell_resolution);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0003F9F5 File Offset: 0x0003DBF5
	public void SetLODOffset(float lod_offset)
	{
		Shader.SetGlobalFloat("_BSGTerrain_LodOffset", lod_offset);
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0003FA04 File Offset: 0x0003DC04
	private void RenderTerrain(Camera camera)
	{
		if (this.terrain_data == null || BSGTerrain.resources == null)
		{
			return;
		}
		if ((camera.cullingMask & 256) == 0)
		{
			return;
		}
		if (camera.gameObject.name.Contains("Ocean") || camera.gameObject.name.Contains("ocean"))
		{
			return;
		}
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
		this.frustrum_planes_native.CopyFrom(array);
		this.SetGlobalShaderProperties(this.terrain_data.Value);
		MaterialPropertyBlock materialPropertyBlock = this.property_block;
		int bsgterrain_TerrainBounds_Min = this._BSGTerrain_TerrainBounds_Min;
		BSGTerrainData value = this.terrain_data.Value;
		materialPropertyBlock.SetVector(bsgterrain_TerrainBounds_Min, value.bounds.min);
		this.property_block.SetTexture(this._BSGTerrain_Heightmap, this.unity_terrain.terrainData.heightmapTexture);
		MaterialPropertyBlock materialPropertyBlock2 = this.property_block;
		int bsgterrain_TerrainBounds_Max = this._BSGTerrain_TerrainBounds_Max;
		value = this.terrain_data.Value;
		materialPropertyBlock2.SetVector(bsgterrain_TerrainBounds_Max, value.bounds.max);
		this.property_block.SetInt(this._BSGTerrain_AlphamapCellResolution, this.terrain_data.Value.alphamap_cell_resolution);
		foreach (BSGTerrainChunk chunk in this.terrain_data.Value.chunks)
		{
			this.RenderChunk(chunk, array, camera);
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0003FB60 File Offset: 0x0003DD60
	private void RenderChunk(BSGTerrainChunk chunk, Plane[] frustrum_planes, Camera camera)
	{
		BSGTerrain.<>c__DisplayClass56_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.chunk = chunk;
		CS$<>8__locals1.camera = camera;
		BSGTerrain.CullResult cullResult = BSGTerrain.TestPlanesAABBExtended(frustrum_planes, CS$<>8__locals1.chunk.bounds);
		if (cullResult == BSGTerrain.CullResult.FullIn)
		{
			BSGTerrainChunkRenderData bakedRenderData = CS$<>8__locals1.chunk.bakedRenderData;
			this.<RenderChunk>g__DrawInstanced|56_0(bakedRenderData.matrices_array.Length, bakedRenderData.matrices_array, bakedRenderData.alphamap_indices, bakedRenderData.primary_alphamapping, bakedRenderData.secondary_alphamapping, ref CS$<>8__locals1);
			return;
		}
		if (cullResult == BSGTerrain.CullResult.PartiallyCulled)
		{
			this.chunk_culling_job = default(ChunkCullingJob);
			this.chunk_culling_job.input_cells = CS$<>8__locals1.chunk.native_cells_array;
			this.chunk_culling_job.frustrum_planes = this.frustrum_planes_native;
			this.chunk_culling_job.alphamap_indices = this.alphamap_indices_native;
			this.chunk_culling_job.matrices_array = this.matrices_array_native;
			this.chunk_culling_job.primary_alphamapping = this.primary_alphamapping_native;
			this.chunk_culling_job.secondary_alphamapping = this.secondary_alphamapping_native;
			this.chunk_culling_job.output_count = new NativeArray<int>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.chunk_culling_job.Schedule(default(JobHandle)).Complete();
			int num = this.chunk_culling_job.output_count[0];
			this.chunk_culling_job.output_count.Dispose();
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.alphamap_indices[i] = this.alphamap_indices_native[i];
					this.matrices_array[i] = this.matrices_array_native[i];
					this.primary_alphamapping[i] = this.primary_alphamapping_native[i];
					this.secondary_alphamapping[i] = this.secondary_alphamapping_native[i];
				}
				this.<RenderChunk>g__DrawInstanced|56_0(num, this.matrices_array, this.alphamap_indices, this.primary_alphamapping, this.secondary_alphamapping, ref CS$<>8__locals1);
			}
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0003FD3C File Offset: 0x0003DF3C
	private void SetGlobalShaderProperties(BSGTerrainData terrain_data)
	{
		for (int i = 0; i < terrain_data.texture_properties.Length; i++)
		{
			this.smoothness_array[i] = terrain_data.texture_properties[i].smoothness;
			this.metallic_array[i] = terrain_data.texture_properties[i].metallic;
			this.specular_array[i] = terrain_data.texture_properties[i].specular;
			this.normal_strength_array[i] = terrain_data.texture_properties[i].normalScale;
			Vector2 tileSize = terrain_data.texture_properties[i].tileSize;
			Vector2 tileOffset = terrain_data.texture_properties[i].tileOffset;
			this.texture_st_array[i] = new Vector4(tileSize.x, tileSize.y, tileOffset.x, tileOffset.y);
		}
		Shader.SetGlobalTexture(this._BSGTerrain_DiffuseTextureArray, terrain_data.diffuse_array);
		Shader.SetGlobalTexture(this._BSGTerrain_NormalTextureArray, terrain_data.normal_array);
		Shader.SetGlobalFloatArray(this._BSGTerrain_SmoothnessArray, this.smoothness_array);
		Shader.SetGlobalFloatArray(this._BSGTerrain_MetallicArray, this.metallic_array);
		Shader.SetGlobalVectorArray(this._BSGTerrain_SpecularArray, this.specular_array);
		Shader.SetGlobalVectorArray(this._BSGTerrain_TextureSTArray, this.texture_st_array);
		Shader.SetGlobalFloat(this._BSGTerrain_AlphamapCellResolution, (float)terrain_data.alphamap_cell_resolution);
		Shader.SetGlobalFloat(this._BSGTerrain_CellsCountInSingleDimension, (float)terrain_data.cells_count_in_single_dimension);
		Shader.SetGlobalVector(this._BSGTerrain_TerrainHeightmapScale, this.unity_terrain.terrainData.heightmapScale);
		Shader.SetGlobalFloatArray(this._BSGTerrain_NormalStrengthArray, this.normal_strength_array);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0003FED8 File Offset: 0x0003E0D8
	private static BSGTerrain.CullResult TestPlanesAABBExtended(Plane[] planes, Bounds chunk_bounds)
	{
		BSGTerrain.<>c__DisplayClass58_0 CS$<>8__locals1;
		CS$<>8__locals1.planes = planes;
		CS$<>8__locals1.minDistance = float.MaxValue;
		BSGTerrain.<TestPlanesAABBExtended>g__TestPoint|58_0(chunk_bounds.center - Vector3.Scale(chunk_bounds.extents, new Vector3(-1f, -1f, -1f)), ref CS$<>8__locals1);
		BSGTerrain.<TestPlanesAABBExtended>g__TestPoint|58_0(chunk_bounds.center - Vector3.Scale(chunk_bounds.extents, new Vector3(1f, -1f, -1f)), ref CS$<>8__locals1);
		BSGTerrain.<TestPlanesAABBExtended>g__TestPoint|58_0(chunk_bounds.center - Vector3.Scale(chunk_bounds.extents, new Vector3(-1f, -1f, 1f)), ref CS$<>8__locals1);
		BSGTerrain.<TestPlanesAABBExtended>g__TestPoint|58_0(chunk_bounds.center - Vector3.Scale(chunk_bounds.extents, new Vector3(1f, -1f, 1f)), ref CS$<>8__locals1);
		if (CS$<>8__locals1.minDistance > 0f)
		{
			return BSGTerrain.CullResult.FullIn;
		}
		if (BSGTerrain.TestPlanesAABB(CS$<>8__locals1.planes, chunk_bounds))
		{
			return BSGTerrain.CullResult.PartiallyCulled;
		}
		return BSGTerrain.CullResult.Culled;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0003FFE5 File Offset: 0x0003E1E5
	private static bool TestPlanesAABB(Plane[] planes, Bounds bounds)
	{
		return GeometryUtility.TestPlanesAABB(planes, bounds);
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0003FFEE File Offset: 0x0003E1EE
	private static void DestroyIfNotNull(Object unity_object)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(unity_object);
			return;
		}
		Object.DestroyImmediate(unity_object);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00040004 File Offset: 0x0003E204
	private void AllocateMemory()
	{
		this.alphamap_indices_native = new NativeArray<float>(256, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.matrices_array_native = new NativeArray<Matrix4x4>(256, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.primary_alphamapping_native = new NativeArray<Vector4>(256, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.secondary_alphamapping_native = new NativeArray<Vector4>(256, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.frustrum_planes_native = new NativeArray<Plane>(6, Allocator.Persistent, NativeArrayOptions.ClearMemory);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00040068 File Offset: 0x0003E268
	private void DealocateMemory()
	{
		if (this.alphamap_indices_native.IsCreated)
		{
			this.alphamap_indices_native.Dispose();
		}
		if (this.matrices_array_native.IsCreated)
		{
			this.matrices_array_native.Dispose();
		}
		if (this.primary_alphamapping_native.IsCreated)
		{
			this.primary_alphamapping_native.Dispose();
		}
		if (this.secondary_alphamapping_native.IsCreated)
		{
			this.secondary_alphamapping_native.Dispose();
		}
		if (this.frustrum_planes_native.IsCreated)
		{
			this.frustrum_planes_native.Dispose();
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x000400F0 File Offset: 0x0003E2F0
	private void DisposeTerrainData()
	{
		if (this.terrain_data != null)
		{
			this.terrain_data.Value.Cleanup();
			this.terrain_data = null;
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0004012C File Offset: 0x0003E32C
	private static Mesh CreatePlaneMesh(int heightmap_cell_resolution = 32)
	{
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Vector2> list3 = new List<Vector2>();
		for (int i = 0; i <= heightmap_cell_resolution; i++)
		{
			for (int j = 0; j <= heightmap_cell_resolution; j++)
			{
				Vector3 zero = Vector3.zero;
				zero.x = (float)j / (float)heightmap_cell_resolution * 2f - 1f;
				zero.z = (float)i / (float)heightmap_cell_resolution * 2f - 1f;
				zero.y = 0f;
				Vector2 item = new Vector2((float)j / (float)heightmap_cell_resolution, (float)i / (float)heightmap_cell_resolution);
				list3.Add(item);
				list2.Add(Vector3.up);
				list.Add(zero);
			}
		}
		List<int> list4 = new List<int>();
		for (int k = 0; k < heightmap_cell_resolution; k++)
		{
			for (int l = 0; l < heightmap_cell_resolution; l++)
			{
				int num = heightmap_cell_resolution + 1;
				int num2 = l + k * num;
				list4.Add(num2);
				list4.Add(num2 + 1 + num);
				list4.Add(num2 + 1);
				list4.Add(num2);
				list4.Add(num2 + num);
				list4.Add(num2 + 1 + num);
			}
		}
		mesh.SetVertices(list);
		mesh.SetNormals(list2);
		mesh.SetUVs(0, list3);
		mesh.SetIndices(list4, 0, list4.Count, MeshTopology.Triangles, 0, false, 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 300f);
		return mesh;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000402B0 File Offset: 0x0003E4B0
	private void OnEnable()
	{
		this.property_block = new MaterialPropertyBlock();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.RenderTerrain));
		if (this.unity_terrain != null && this.terrain_data != null)
		{
			this.unity_terrain.drawHeightmap = false;
		}
		this.AllocateMemory();
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00040318 File Offset: 0x0003E518
	private void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.RenderTerrain));
		if (this.unity_terrain != null && this.terrain_data != null)
		{
			this.unity_terrain.drawHeightmap = true;
		}
		this.DealocateMemory();
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00040374 File Offset: 0x0003E574
	private void OnDestroy()
	{
		BSGTerrain.DestroyIfNotNull(this.cell_mesh);
		if (this.terrain_data != null)
		{
			this.terrain_data.Value.Cleanup();
			this.terrain_data = null;
		}
		UnityEngine.Resources.UnloadUnusedAssets();
		if (this.unity_terrain != null)
		{
			this.unity_terrain.drawHeightmap = true;
		}
		this.DealocateMemory();
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000405B0 File Offset: 0x0003E7B0
	[CompilerGenerated]
	private void <RenderChunk>g__DrawInstanced|56_0(int count, Matrix4x4[] matrices, float[] alphamap_indices, Vector4[] primary_alphamapping, Vector4[] secondary_alphamapping, ref BSGTerrain.<>c__DisplayClass56_0 A_6)
	{
		this.property_block.SetFloatArray(this._AlphamapIndex, alphamap_indices);
		this.property_block.SetVectorArray(this._PrimaryMappings, primary_alphamapping);
		this.property_block.SetVectorArray(this._SecondaryMappings, secondary_alphamapping);
		this.property_block.SetTexture(this._BSGTerrain_PrimaryAlphamaps, A_6.chunk.primary_alphamap_array);
		this.property_block.SetTexture(this._BSGTerrain_SecondaryAlphamaps, A_6.chunk.secondary_alphamap_array);
		Graphics.DrawMeshInstanced(this.cell_mesh, 0, BSGTerrain.Resources.terrain_material, matrices, count, this.property_block, ShadowCastingMode.On, this.cast_shadows, 8, A_6.camera);
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0004065C File Offset: 0x0003E85C
	[CompilerGenerated]
	internal static void <TestPlanesAABBExtended>g__TestPoint|58_0(Vector3 point, ref BSGTerrain.<>c__DisplayClass58_0 A_1)
	{
		for (int i = 0; i < A_1.planes.Length; i++)
		{
			float distanceToPoint = A_1.planes[i].GetDistanceToPoint(point);
			A_1.minDistance = Mathf.Min(distanceToPoint, A_1.minDistance);
		}
	}

	// Token: 0x0400054B RID: 1355
	private Terrain unity_terrain;

	// Token: 0x0400054C RID: 1356
	[NonSerialized]
	public BSGTerrainData? terrain_data;

	// Token: 0x0400054D RID: 1357
	public int alphamap_cell_resolution = 32;

	// Token: 0x0400054E RID: 1358
	public int texture_resolution = 1024;

	// Token: 0x0400054F RID: 1359
	public Mesh cell_mesh;

	// Token: 0x04000550 RID: 1360
	public bool cast_shadows = true;

	// Token: 0x04000551 RID: 1361
	private bool compress_textures;

	// Token: 0x04000552 RID: 1362
	private static BSGTerrainResources resources;

	// Token: 0x04000553 RID: 1363
	private MaterialPropertyBlock property_block;

	// Token: 0x04000554 RID: 1364
	private readonly int _BSGTerrain_NormalStrengthArray = Shader.PropertyToID("_BSGTerrain_NormalStrengthArray");

	// Token: 0x04000555 RID: 1365
	private readonly int _BSGTerrain_PrimaryAlphamaps = Shader.PropertyToID("_BSGTerrain_PrimaryAlphamaps");

	// Token: 0x04000556 RID: 1366
	private readonly int _AlphamapIndex = Shader.PropertyToID("_AlphamapIndex");

	// Token: 0x04000557 RID: 1367
	private readonly int _BSGTerrain_Heightmap = Shader.PropertyToID("_BSGTerrain_Heightmap");

	// Token: 0x04000558 RID: 1368
	private readonly int _BSGTerrain_TerrainBounds_Min = Shader.PropertyToID("_BSGTerrain_TerrainBounds_Min");

	// Token: 0x04000559 RID: 1369
	private readonly int _BSGTerrain_TerrainBounds_Max = Shader.PropertyToID("_BSGTerrain_TerrainBounds_Max");

	// Token: 0x0400055A RID: 1370
	private readonly int _BSGTerrain_SmoothnessArray = Shader.PropertyToID("_BSGTerrain_SmoothnessArray");

	// Token: 0x0400055B RID: 1371
	private readonly int _BSGTerrain_DiffuseTextureArray = Shader.PropertyToID("_BSGTerrain_DiffuseTextureArray");

	// Token: 0x0400055C RID: 1372
	private readonly int _BSGTerrain_NormalTextureArray = Shader.PropertyToID("_BSGTerrain_NormalTextureArray");

	// Token: 0x0400055D RID: 1373
	private readonly int _BSGTerrain_MetallicArray = Shader.PropertyToID("_BSGTerrain_MetallicArray");

	// Token: 0x0400055E RID: 1374
	private readonly int _BSGTerrain_SpecularArray = Shader.PropertyToID("_BSGTerrain_SpecularArray");

	// Token: 0x0400055F RID: 1375
	private readonly int _BSGTerrain_TextureSTArray = Shader.PropertyToID("_BSGTerrain_TextureSTArray");

	// Token: 0x04000560 RID: 1376
	private readonly int _SecondaryMappings = Shader.PropertyToID("_SecondaryMappings");

	// Token: 0x04000561 RID: 1377
	private readonly int _PrimaryMappings = Shader.PropertyToID("_PrimaryMappings");

	// Token: 0x04000562 RID: 1378
	private readonly int _BSGTerrain_SecondaryAlphamaps = Shader.PropertyToID("_BSGTerrain_SecondaryAlphamaps");

	// Token: 0x04000563 RID: 1379
	private readonly int _BSGTerrain_AlphamapCellResolution = Shader.PropertyToID("_BSGTerrain_AlphamapCellResolution");

	// Token: 0x04000564 RID: 1380
	private readonly int _BSGTerrain_CellsCountInSingleDimension = Shader.PropertyToID("_BSGTerrain_CellsCountInSingleDimension");

	// Token: 0x04000565 RID: 1381
	private readonly int _BSGTerrain_TerrainHeightmapScale = Shader.PropertyToID("_BSGTerrain_TerrainHeightmapScale");

	// Token: 0x04000566 RID: 1382
	private Vector4[] primary_alphamapping = new Vector4[256];

	// Token: 0x04000567 RID: 1383
	private Vector4[] secondary_alphamapping = new Vector4[256];

	// Token: 0x04000568 RID: 1384
	private float[] alphamap_indices = new float[256];

	// Token: 0x04000569 RID: 1385
	private Matrix4x4[] matrices_array = new Matrix4x4[256];

	// Token: 0x0400056A RID: 1386
	private NativeArray<Vector4> primary_alphamapping_native;

	// Token: 0x0400056B RID: 1387
	private NativeArray<Vector4> secondary_alphamapping_native;

	// Token: 0x0400056C RID: 1388
	private NativeArray<float> alphamap_indices_native;

	// Token: 0x0400056D RID: 1389
	private NativeArray<Matrix4x4> matrices_array_native;

	// Token: 0x0400056E RID: 1390
	private NativeArray<Plane> frustrum_planes_native;

	// Token: 0x0400056F RID: 1391
	private float[] smoothness_array = new float[64];

	// Token: 0x04000570 RID: 1392
	private float[] metallic_array = new float[64];

	// Token: 0x04000571 RID: 1393
	private Vector4[] specular_array = new Vector4[64];

	// Token: 0x04000572 RID: 1394
	private Vector4[] texture_st_array = new Vector4[64];

	// Token: 0x04000573 RID: 1395
	private float[] normal_strength_array = new float[64];

	// Token: 0x04000574 RID: 1396
	private ChunkCullingJob chunk_culling_job;

	// Token: 0x0200056A RID: 1386
	public enum NumberOfTextures
	{
		// Token: 0x0400303C RID: 12348
		_3 = 3,
		// Token: 0x0400303D RID: 12349
		_4
	}

	// Token: 0x0200056B RID: 1387
	private enum CullResult
	{
		// Token: 0x0400303F RID: 12351
		FullIn,
		// Token: 0x04003040 RID: 12352
		PartiallyCulled,
		// Token: 0x04003041 RID: 12353
		Culled
	}
}
