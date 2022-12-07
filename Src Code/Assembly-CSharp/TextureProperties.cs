using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
[Serializable]
public struct TextureProperties
{
	// Token: 0x06000610 RID: 1552 RVA: 0x00041F88 File Offset: 0x00040188
	public static TextureProperties FromTerrainLayer(TerrainLayer terrainLayer)
	{
		return new TextureProperties
		{
			smoothness = terrainLayer.smoothness,
			metallic = terrainLayer.metallic,
			specular = terrainLayer.specular,
			tileOffset = terrainLayer.tileOffset,
			tileSize = terrainLayer.tileSize,
			normalScale = terrainLayer.normalScale
		};
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00041FEC File Offset: 0x000401EC
	public static TextureProperties RandomProperties()
	{
		return new TextureProperties
		{
			smoothness = Random.Range(0f, 1f),
			metallic = Random.Range(0f, 1f),
			specular = Color.white,
			tileSize = Vector2.one * Random.Range(15f, 50f),
			tileOffset = Vector2.zero
		};
	}

	// Token: 0x040005AD RID: 1453
	public float smoothness;

	// Token: 0x040005AE RID: 1454
	public float metallic;

	// Token: 0x040005AF RID: 1455
	public Color specular;

	// Token: 0x040005B0 RID: 1456
	public Vector2 tileOffset;

	// Token: 0x040005B1 RID: 1457
	public Vector2 tileSize;

	// Token: 0x040005B2 RID: 1458
	public float normalScale;
}
