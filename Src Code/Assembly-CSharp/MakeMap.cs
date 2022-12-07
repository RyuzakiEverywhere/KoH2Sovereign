using System;
using System.Collections.Generic;
using BSG.MapMaker;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class MakeMap : MonoBehaviour
{
	// Token: 0x0600056C RID: 1388 RVA: 0x0003C974 File Offset: 0x0003AB74
	public void Run()
	{
		if (this.graph == null)
		{
			return;
		}
		Terrain terrain = base.GetComponent<Terrain>();
		if (terrain == null)
		{
			float num = 0f;
			Terrain[] activeTerrains = Terrain.activeTerrains;
			for (int i = 0; i < activeTerrains.Length; i++)
			{
				float x = activeTerrains[i].terrainData.size.x;
				if (x > num)
				{
					terrain = activeTerrains[i];
					num = x;
				}
			}
		}
		if (terrain == null)
		{
			return;
		}
		this.graph.generated_floats = new Dictionary<string, float>();
		this.graph.generated_textures = new Dictionary<string, Texture2D>();
		this.graph.Run(terrain);
	}

	// Token: 0x04000508 RID: 1288
	public MapMakerGraph graph;
}
