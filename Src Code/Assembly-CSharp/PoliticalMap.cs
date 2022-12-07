using System;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class PoliticalMap : MonoBehaviour
{
	// Token: 0x0600024D RID: 589 RVA: 0x00021984 File Offset: 0x0001FB84
	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return;
		}
		float x = activeTerrain.terrainData.size.x;
		float z = activeTerrain.terrainData.size.z;
		base.transform.localScale = new Vector3(x, z, 1f);
		base.transform.localPosition = new Vector3(x / 2f, -0.1f, z / 2f);
		base.transform.eulerAngles = new Vector3(90f, 0f, 0f);
	}
}
