using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x02000118 RID: 280
[RequireComponent(typeof(Terrain))]
public class TerrainQualityManager : MonoBehaviour
{
	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000CB6 RID: 3254 RVA: 0x0008DA98 File Offset: 0x0008BC98
	public static string DIFFUSE_BINDINGS_BUNDLE_DIR
	{
		get
		{
			return "AssetBundles";
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x0008DA9F File Offset: 0x0008BC9F
	public static string DIFFUSE_BINDINGS_BUNDLE_NAME
	{
		get
		{
			return "bsg_terrain_diffuse_textures";
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0008DAA6 File Offset: 0x0008BCA6
	public static string DIFFUSE_BINDINGS_BUNDLE_PATH
	{
		get
		{
			return Path.Combine(TerrainQualityManager.DIFFUSE_BINDINGS_BUNDLE_DIR, TerrainQualityManager.DIFFUSE_BINDINGS_BUNDLE_NAME);
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x0008DA98 File Offset: 0x0008BC98
	public static string NORMAL_BINDINGS_BUNDLE_DIR
	{
		get
		{
			return "AssetBundles";
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000CBA RID: 3258 RVA: 0x0008DAB7 File Offset: 0x0008BCB7
	public static string NORMAL_BINDINGS_BUNDLE_NAME
	{
		get
		{
			return "bsg_terrain_normal_textures";
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000CBB RID: 3259 RVA: 0x0008DABE File Offset: 0x0008BCBE
	public static string NORMAL_BINDINGS_BUNDLE_PATH
	{
		get
		{
			return Path.Combine(TerrainQualityManager.NORMAL_BINDINGS_BUNDLE_DIR, TerrainQualityManager.NORMAL_BINDINGS_BUNDLE_NAME);
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000CBC RID: 3260 RVA: 0x0008DACF File Offset: 0x0008BCCF
	public static TerrainQualityManager ActiveInstance
	{
		get
		{
			return TerrainQualityManager.Instances.LastOrDefault<TerrainQualityManager>();
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000CBD RID: 3261 RVA: 0x00033DA0 File Offset: 0x00031FA0
	private Terrain unity_terrain
	{
		get
		{
			return base.GetComponent<Terrain>();
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000CBE RID: 3262 RVA: 0x0008DADB File Offset: 0x0008BCDB
	// (set) Token: 0x06000CBF RID: 3263 RVA: 0x0008DAE2 File Offset: 0x0008BCE2
	public static TerrainQualityManager.QualityOption Quality
	{
		get
		{
			return TerrainQualityManager.quality;
		}
		set
		{
			if (TerrainQualityManager.quality == value)
			{
				return;
			}
			TerrainQualityManager.quality = value;
			TerrainQualityManager activeInstance = TerrainQualityManager.ActiveInstance;
			if (activeInstance == null)
			{
				return;
			}
			activeInstance.UpdateTerrain();
		}
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0008DB02 File Offset: 0x0008BD02
	private void OnEnable()
	{
		TerrainQualityManager.Instances.Add(this);
		if (this.wait_for_terrain_generation)
		{
			SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.UpdateTerrain));
			return;
		}
		this.UpdateTerrain();
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0008DB3E File Offset: 0x0008BD3E
	private void OnDisable()
	{
		TerrainQualityManager.Instances.Remove(this);
		if (this.wait_for_terrain_generation)
		{
			SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.UpdateTerrain));
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0008DB74 File Offset: 0x0008BD74
	private void UpdateTerrain()
	{
		switch (TerrainQualityManager.Quality)
		{
		case TerrainQualityManager.QualityOption.Low:
			this.SetLowQuality();
			return;
		case TerrainQualityManager.QualityOption.Medium:
			this.SetMediumQuality();
			return;
		case TerrainQualityManager.QualityOption.High:
			this.SetHighQuality();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0008DBAE File Offset: 0x0008BDAE
	private void SetHighQuality()
	{
		this.DestroyBSGTerrainIfExists();
		Shader.EnableKeyword("PARALLAX_EFFECT");
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0008DBC0 File Offset: 0x0008BDC0
	private void SetMediumQuality()
	{
		this.CreateBSGTerrainIfNotExists();
		BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._4);
		BSGTerrain.EnableNormalTextures();
		this.bsg_terrain_instance.SetMeshQuality(1f);
		this.bsg_terrain_instance.SetLODOffset(-0.25f);
		this.bsg_terrain_instance.cast_shadows = true;
		Shader.DisableKeyword("PARALLAX_EFFECT");
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0008DC14 File Offset: 0x0008BE14
	private void SetLowQuality()
	{
		this.CreateBSGTerrainIfNotExists();
		BSGTerrain.SetNumberOfTextures(BSGTerrain.NumberOfTextures._3);
		BSGTerrain.DisableNormalTextures();
		this.bsg_terrain_instance.SetMeshQuality(0.6f);
		this.bsg_terrain_instance.SetLODOffset(0.25f);
		this.bsg_terrain_instance.cast_shadows = false;
		Shader.DisableKeyword("PARALLAX_EFFECT");
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0008DC68 File Offset: 0x0008BE68
	private void DestroyBSGTerrainIfExists()
	{
		if (this.bsg_terrain_instance != null)
		{
			Object.Destroy(this.bsg_terrain_instance);
			this.bsg_terrain_instance = null;
		}
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0008DC8C File Offset: 0x0008BE8C
	private void CreateBSGTerrainIfNotExists()
	{
		if (this.bsg_terrain_instance == null)
		{
			if (this.unity_terrain == null)
			{
				return;
			}
			if (this.unity_terrain.terrainData == null)
			{
				return;
			}
			if (!File.Exists(TerrainQualityManager.DIFFUSE_BINDINGS_BUNDLE_PATH) || !File.Exists(TerrainQualityManager.NORMAL_BINDINGS_BUNDLE_PATH))
			{
				Debug.LogWarning("No terrain texture bundles were found, processing textures on the fly...");
				this.bsg_terrain_instance = BSGTerrain.ConvertUnityTerrain(this.unity_terrain, 16, 1024, true);
				return;
			}
			if (TerrainQualityManager.diffuse_binder_bundle == null)
			{
				TerrainQualityManager.diffuse_binder_bundle = AssetBundle.LoadFromFile(TerrainQualityManager.DIFFUSE_BINDINGS_BUNDLE_PATH);
			}
			if (TerrainQualityManager.normal_binder_bundle == null)
			{
				TerrainQualityManager.normal_binder_bundle = AssetBundle.LoadFromFile(TerrainQualityManager.NORMAL_BINDINGS_BUNDLE_PATH);
			}
			BSGTerrainTextureBindings bsgterrainTextureBindings = (BSGTerrainTextureBindings)TerrainQualityManager.diffuse_binder_bundle.LoadAllAssets().FirstOrDefault((Object o) => o is BSGTerrainTextureBindings);
			BSGTerrainTextureBindings bsgterrainTextureBindings2 = (BSGTerrainTextureBindings)TerrainQualityManager.normal_binder_bundle.LoadAllAssets().FirstOrDefault((Object o) => o is BSGTerrainTextureBindings);
			if (bsgterrainTextureBindings != null && bsgterrainTextureBindings2 != null)
			{
				this.bsg_terrain_instance = BSGTerrain.ConvertUnityTerrain(this.unity_terrain, bsgterrainTextureBindings, bsgterrainTextureBindings2, 16);
				return;
			}
			Debug.LogWarning("Can't load terrain texture binders, processing textures on the fly...");
			this.bsg_terrain_instance = BSGTerrain.ConvertUnityTerrain(this.unity_terrain, 16, 1024, true);
		}
	}

	// Token: 0x040009E4 RID: 2532
	private static List<TerrainQualityManager> Instances = new List<TerrainQualityManager>();

	// Token: 0x040009E5 RID: 2533
	private BSGTerrain bsg_terrain_instance;

	// Token: 0x040009E6 RID: 2534
	private static TerrainQualityManager.QualityOption quality = TerrainQualityManager.QualityOption.High;

	// Token: 0x040009E7 RID: 2535
	public bool wait_for_terrain_generation;

	// Token: 0x040009E8 RID: 2536
	private static AssetBundle diffuse_binder_bundle = null;

	// Token: 0x040009E9 RID: 2537
	private static AssetBundle normal_binder_bundle = null;

	// Token: 0x02000618 RID: 1560
	public enum QualityOption
	{
		// Token: 0x040033D1 RID: 13265
		Low,
		// Token: 0x040033D2 RID: 13266
		Medium,
		// Token: 0x040033D3 RID: 13267
		High
	}
}
