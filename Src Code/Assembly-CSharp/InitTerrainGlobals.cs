using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
[ExecuteInEditMode]
public class InitTerrainGlobals : MonoBehaviour
{
	// Token: 0x06003252 RID: 12882 RVA: 0x00198318 File Offset: 0x00196518
	private void Init()
	{
		Terrain terrain = base.GetComponent<Terrain>();
		if (terrain == null)
		{
			terrain = Terrain.activeTerrain;
			if (terrain == null)
			{
				Debug.LogError("Terrain object in 'heights' is NULL!");
				return;
			}
		}
		Texture heightmapTexture = terrain.terrainData.heightmapTexture;
		if (heightmapTexture == null)
		{
			Debug.Log("Terrain heights texture in 'heights' is NULL!");
			return;
		}
		Shader.SetGlobalTexture("_Terrain_HM", heightmapTexture);
		Shader.SetGlobalVector("_Terrain_Size", terrain.terrainData.size);
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x00198394 File Offset: 0x00196594
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00198394 File Offset: 0x00196594
	private void OnEnable()
	{
		this.Init();
	}
}
