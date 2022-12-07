using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class OverlayView : WorldView
{
	// Token: 0x06000BB9 RID: 3001 RVA: 0x00083E7C File Offset: 0x0008207C
	protected override void OnActivate()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain != null)
		{
			if (this.org_terrain_material == null)
			{
				this.org_terrain_material = activeTerrain.materialTemplate;
			}
			if (this.terrain_material != null)
			{
				activeTerrain.materialTemplate = this.terrain_material;
			}
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00083ECC File Offset: 0x000820CC
	protected override void OnDeactivate()
	{
		Shader.SetGlobalTexture("_OverlayTex", null);
		if (this.org_terrain_material != null)
		{
			Terrain activeTerrain = Terrain.activeTerrain;
			if (activeTerrain != null)
			{
				activeTerrain.materialTemplate = this.org_terrain_material;
			}
		}
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00083F10 File Offset: 0x00082110
	public override void SetShaderGlobals(bool secondary)
	{
		base.SetShaderGlobals(secondary);
		MapData mapData = MapData.Get();
		if (mapData != null && this.idx >= 0 && this.idx < mapData.OverlayTextures.Length)
		{
			Shader.SetGlobalTexture("_OverlayTex", mapData.OverlayTextures[this.idx]);
			Shader.SetGlobalColor("_OverlayColor", new Color(1f, 1f, 1f, mapData.OverlayAlpha));
		}
		else
		{
			Shader.SetGlobalTexture("_OverlayTex", null);
		}
		Shader.SetGlobalFloat("ground1", mapData.TerrainHeights.WaterLevel);
		Shader.SetGlobalFloat("ground2", mapData.TerrainHeights.BeachMin);
		Shader.SetGlobalFloat("ground3", mapData.TerrainHeights.BeachMax);
		Shader.SetGlobalFloat("mountainsLow", mapData.TerrainHeights.MountainsMin);
		Shader.SetGlobalFloat("mountainsHigh", mapData.TerrainHeights.MountainsMax);
	}

	// Token: 0x04000924 RID: 2340
	public int idx;

	// Token: 0x04000925 RID: 2341
	private Material org_terrain_material;
}
