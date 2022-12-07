using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000072 RID: 114
[ExecuteAlways]
[RequireComponent(typeof(Terrain))]
public class BSGGrass : MonoBehaviour
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600044B RID: 1099 RVA: 0x00033DA0 File Offset: 0x00031FA0
	public Terrain terrain
	{
		get
		{
			return base.GetComponent<Terrain>();
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600044C RID: 1100 RVA: 0x00033DA8 File Offset: 0x00031FA8
	public TerrainData terrain_data
	{
		get
		{
			return this.terrain.terrainData;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600044D RID: 1101 RVA: 0x00033DB8 File Offset: 0x00031FB8
	public Vector4 TerrainMin
	{
		get
		{
			return this.terrain_data.bounds.min + base.transform.position;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600044E RID: 1102 RVA: 0x00033DF0 File Offset: 0x00031FF0
	public Vector4 TerrainMax
	{
		get
		{
			return this.terrain_data.bounds.max + base.transform.position;
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600044F RID: 1103 RVA: 0x00033E25 File Offset: 0x00032025
	public static Shader TerrainOverrideShader
	{
		get
		{
			if (!(BSGGrass.terrainOverrideShader != null))
			{
				return BSGGrass.terrainOverrideShader = Shader.Find("Nature/Terrain/BSGGrassSupportStandard");
			}
			return BSGGrass.terrainOverrideShader;
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00033E4A File Offset: 0x0003204A
	public void RegenerateBasemap()
	{
		if (this.basemap != null)
		{
			this.basemap.Release();
			this.basemap = null;
		}
		this.basemap = BasemapRenderer.CreateBasemap(this.terrain_data, 1024);
		this.SetShaderUniformsOfTerrainsData();
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00033E88 File Offset: 0x00032088
	public static void DisableGrass()
	{
		BSGGrass.enable_rendering = false;
		foreach (BSGGrass bsggrass in BSGGrass.Instances)
		{
			bsggrass.terrain.drawTreesAndFoliage = false;
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00033EE0 File Offset: 0x000320E0
	public static void EnableGrass()
	{
		BSGGrass.enable_rendering = true;
		foreach (BSGGrass bsggrass in BSGGrass.Instances)
		{
			bsggrass.terrain.drawTreesAndFoliage = true;
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00033F38 File Offset: 0x00032138
	private void OnEnable()
	{
		this.terrain.drawTreesAndFoliage = true;
		this.is_basemap_baked_for_battle_view = false;
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.OnMapGenerationComplete));
		this.Setup();
		BSGGrassData.SetShaderProperties();
		if (!BSGGrass.instances.Contains(this))
		{
			BSGGrass.instances.Add(this);
		}
		this.basemap = BasemapRenderer.CreateBasemap(this.terrain_data, 1024);
		this.SetShaderUniformsOfTerrainsData();
		if (BSGGrass.enable_rendering)
		{
			BSGGrass.EnableGrass();
			return;
		}
		BSGGrass.DisableGrass();
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00033FCC File Offset: 0x000321CC
	private void OnDisable()
	{
		if (BSGGrass.instances.Contains(this))
		{
			BSGGrass.instances.Remove(this);
		}
		if (this.basemap != null)
		{
			this.basemap.Release();
			this.basemap = null;
		}
		this.SetShaderUniformsOfTerrainsData();
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.OnMapGenerationComplete));
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00034038 File Offset: 0x00032238
	private void Setup()
	{
		if (base.isActiveAndEnabled && BSGGrass.enable_rendering)
		{
			this.SetupTerrainShader();
			this.UpdateTerrainDetails();
		}
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00034055 File Offset: 0x00032255
	private void OnMapGenerationComplete()
	{
		if (this.is_basemap_baked_for_battle_view)
		{
			return;
		}
		BasemapRenderer.RenderBasemap(this.terrain_data, this.basemap);
		this.SetShaderUniformsOfTerrainsData();
		this.is_basemap_baked_for_battle_view = true;
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00034080 File Offset: 0x00032280
	private void UpdateTerrainDetails()
	{
		DetailPrototype[] detailPrototypes = this.terrain_data.detailPrototypes;
		bool flag = false;
		for (int i = 0; i < detailPrototypes.Length; i++)
		{
			if (BSGGrassData.IsDetailBSGGrassPreset(detailPrototypes[i]))
			{
				detailPrototypes[i] = BSGGrassData.GetPreset(detailPrototypes[i]).CreatePrototype();
				flag = true;
			}
		}
		if (flag)
		{
			this.terrain_data.detailPrototypes = detailPrototypes;
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000340D4 File Offset: 0x000322D4
	private void SetupTerrainShader()
	{
		if (this.terrain.materialTemplate == null)
		{
			this.terrain.materialTemplate = new Material(BSGGrass.TerrainOverrideShader);
			return;
		}
		if (!this.terrain.materialTemplate.shader.name.Equals(BSGGrass.TerrainOverrideShader.name))
		{
			this.terrain.materialTemplate = new Material(BSGGrass.TerrainOverrideShader);
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000459 RID: 1113 RVA: 0x00034145 File Offset: 0x00032345
	public static IReadOnlyList<BSGGrass> Instances
	{
		get
		{
			return BSGGrass.instances;
		}
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0003414C File Offset: 0x0003234C
	private void SetShaderUniformsOfTerrainsData()
	{
		BSGGrass.UpdateBounds();
		BSGGrass.UpdateTerrainHeights();
		BSGGrass.UpdateTextureArray();
		BSGGrass.UpdateShaderUniforms();
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00034164 File Offset: 0x00032364
	public static void UpdateTextureArray()
	{
		if (BSGGrass.basemaps != null)
		{
			Common.DestroyObj(BSGGrass.basemaps);
			BSGGrass.basemaps = null;
		}
		BSGGrass.basemaps = new Texture2DArray(1024, 1024, Mathf.Max(1, BSGGrass.instances.Count), TextureFormat.RGBAHalf, false, false);
		for (int i = 0; i < BSGGrass.instances.Count; i++)
		{
			BSGGrass bsggrass = BSGGrass.instances[i];
			if (!(bsggrass.basemap == null))
			{
				Graphics.CopyTexture(bsggrass.basemap, 0, 0, BSGGrass.basemaps, i, 0);
			}
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000341FC File Offset: 0x000323FC
	public static void UpdateBounds()
	{
		BSGGrass.bounds.Clear();
		for (int i = 0; i < BSGGrass.instances.Count; i++)
		{
			BSGGrass bsggrass = BSGGrass.instances[i];
			BSGGrass.bounds.Add(new Vector4(bsggrass.TerrainMin.x, bsggrass.TerrainMin.z, bsggrass.TerrainMax.x, bsggrass.TerrainMax.z));
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00034270 File Offset: 0x00032470
	public static void UpdateTerrainHeights()
	{
		BSGGrass.terrain_heights.Clear();
		for (int i = 0; i < BSGGrass.instances.Count; i++)
		{
			BSGGrass bsggrass = BSGGrass.instances[i];
			BSGGrass.terrain_heights.Add(bsggrass.terrain_data.bounds.size.y);
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x000342CC File Offset: 0x000324CC
	public static void UpdateShaderUniforms()
	{
		int num = 0;
		while (num < BSGGrass.instances.Count && num < 32)
		{
			BSGGrass.bounds_uniform[num] = BSGGrass.bounds[num];
			BSGGrass.terrain_heights_uniform[num] = BSGGrass.terrain_heights[num];
			num++;
		}
		Shader.SetGlobalInt("_BSGGrass_InstanceCount", BSGGrass.instances.Count);
		Shader.SetGlobalVectorArray("_BSGGrass_Bounds", BSGGrass.bounds_uniform);
		Shader.SetGlobalFloatArray("_BSGGrass_TerrainHeights", BSGGrass.terrain_heights_uniform);
		Shader.SetGlobalTexture("_BSGGrass_Basemaps", BSGGrass.basemaps);
		Shader.SetGlobalVector("_BSGGrass_Basemaps_TexelSize", new Vector4(1f / (float)BSGGrass.basemaps.width, 1f / (float)BSGGrass.basemaps.height, 0f, 0f));
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00034398 File Offset: 0x00032598
	public void MockupFirstLayer()
	{
		DetailPrototype[] detailPrototypes = new DetailPrototype[]
		{
			BSGGrassData.GetAllPresets().First<BSGGrassPreset>().CreatePrototype()
		};
		this.terrain_data.detailPrototypes = detailPrototypes;
		int[,] detailLayer = this.terrain_data.GetDetailLayer(0, 0, this.terrain_data.detailResolution, this.terrain_data.detailResolution, 0);
		for (int i = 0; i < detailLayer.GetLength(0); i++)
		{
			for (int j = 0; j < detailLayer.GetLength(1); j++)
			{
				detailLayer[i, j] = 15;
			}
		}
		this.terrain_data.SetDetailLayer(0, 0, 0, detailLayer);
		this.terrain.detailObjectDistance = 150f;
	}

	// Token: 0x0400044C RID: 1100
	public const int DEFAULT_RESOLUTION = 1024;

	// Token: 0x0400044D RID: 1101
	public static bool enable_rendering = true;

	// Token: 0x0400044E RID: 1102
	public RenderTexture basemap;

	// Token: 0x0400044F RID: 1103
	private static Shader terrainOverrideShader = null;

	// Token: 0x04000450 RID: 1104
	private bool is_basemap_baked_for_battle_view;

	// Token: 0x04000451 RID: 1105
	private const int MAX_RENDERED_TERRAINS_COUNT = 32;

	// Token: 0x04000452 RID: 1106
	private static List<BSGGrass> instances = new List<BSGGrass>();

	// Token: 0x04000453 RID: 1107
	private static Texture2DArray basemaps;

	// Token: 0x04000454 RID: 1108
	private static List<Vector4> bounds = new List<Vector4>();

	// Token: 0x04000455 RID: 1109
	private static Vector4[] bounds_uniform = new Vector4[32];

	// Token: 0x04000456 RID: 1110
	private static List<float> terrain_heights = new List<float>();

	// Token: 0x04000457 RID: 1111
	private static float[] terrain_heights_uniform = new float[32];
}
