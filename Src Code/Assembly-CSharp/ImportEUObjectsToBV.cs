using System;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class ImportEUObjectsToBV : MonoBehaviour
{
	// Token: 0x0600321A RID: 12826 RVA: 0x0019636C File Offset: 0x0019456C
	public void Import()
	{
		if (this.objectsPrefab == null)
		{
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		TerrainData terrainData = (activeTerrain != null) ? activeTerrain.terrainData : null;
		if (terrainData == null)
		{
			return;
		}
		if (this.sourceTerrain != null)
		{
			this.sourceTerrainSize = this.sourceTerrain.size;
			this.sourceTerrainResolution = this.sourceTerrain.heightmapResolution;
		}
		Vector2 vector = new Vector2(this.worldSizeTiles.x / (float)this.sourceTerrainResolution * this.sourceTerrainSize.x, this.worldSizeTiles.y / (float)this.sourceTerrainResolution * this.sourceTerrainSize.z);
		Vector3 vector2 = new Vector3(this.worldPos.x, 0f, this.worldPos.y);
		Vector3 a = new Vector3(terrainData.size.x / 2f, 0f, terrainData.size.z / 2f);
		Vector3 b;
		Vector3 a2 = b = Vector3.Scale(terrainData.size, new Vector3(1f / vector.x, 1f / terrainData.size.y, 1f / vector.y));
		if (this.sourceTerrain != null)
		{
			b.y = terrainData.size.y / this.sourceTerrain.size.y;
		}
		Vector3 b2 = Vector3.Scale(a2, this.additionalScale);
		Transform transform = base.transform.Find(this.objectsPrefab.name);
		if (transform != null)
		{
			Object.DestroyImmediate(transform.gameObject);
		}
		transform = new GameObject(this.objectsPrefab.name).transform;
		transform.SetParent(base.transform, true);
		Bounds bounds = new Bounds(vector2, new Vector3(vector.x, 10000f, vector.y));
		Transform transform2 = this.objectsPrefab.transform;
		for (int i = 0; i < transform2.childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			if (bounds.Contains(child.localPosition))
			{
				Vector3 b3 = Vector3.Scale(child.position - vector2, b);
				Transform transform3 = Object.Instantiate<GameObject>(child.gameObject, transform).transform;
				transform3.localPosition = a + b3;
				transform3.localScale = Vector3.Scale(child.localScale, b2);
			}
		}
	}

	// Token: 0x04002180 RID: 8576
	public GameObject objectsPrefab;

	// Token: 0x04002181 RID: 8577
	public TerrainData sourceTerrain;

	// Token: 0x04002182 RID: 8578
	public Vector3 sourceTerrainSize = new Vector3(3750f, 40f, 2625f);

	// Token: 0x04002183 RID: 8579
	public int sourceTerrainResolution = 4097;

	// Token: 0x04002184 RID: 8580
	public Vector2 worldPos = new Vector2(1605.689f, 1176.181f);

	// Token: 0x04002185 RID: 8581
	public Vector2 worldSizeTiles = new Vector2(160f, 160f);

	// Token: 0x04002186 RID: 8582
	public Vector3 additionalScale = Vector3.one;
}
