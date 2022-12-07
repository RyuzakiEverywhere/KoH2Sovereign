using System;
using UnityEngine;

// Token: 0x0200032E RID: 814
[ExecuteInEditMode]
public class PerlinNoiseGenerator : MonoBehaviour
{
	// Token: 0x06003227 RID: 12839 RVA: 0x00196918 File Offset: 0x00194B18
	public static float[,] GenerateNoise(Vector2 periods, Vector2Int size)
	{
		float[,] array = new float[size.x, size.y];
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				float num = Mathf.PerlinNoise((float)j / periods.x, (float)i / periods.y);
				array[j, i] = num;
			}
		}
		return array;
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x0019697F File Offset: 0x00194B7F
	public static void SaveToTexture(string path, BSGTerrainTools.Float2D f2d)
	{
		f2d.SavePNG(path);
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x00196988 File Offset: 0x00194B88
	public void CreatePerlinTexture()
	{
		float[,] arr = PerlinNoiseGenerator.GenerateNoise(this.periods, this.size);
		PerlinNoiseGenerator.SaveToTexture(this.textureSafePath, new BSGTerrainTools.Float2D(arr));
	}

	// Token: 0x040021B2 RID: 8626
	public Vector2 periods;

	// Token: 0x040021B3 RID: 8627
	public Vector2Int size;

	// Token: 0x040021B4 RID: 8628
	public string textureSafePath;
}
