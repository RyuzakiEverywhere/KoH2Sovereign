using System;
using System.Text;
using UnityEngine;

// Token: 0x020000AE RID: 174
[RequireComponent(typeof(Terrain))]
public class DebugTerrainData : MonoBehaviour
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600060B RID: 1547 RVA: 0x00041B9D File Offset: 0x0003FD9D
	public TerrainData terrainData
	{
		get
		{
			return this.terrain.terrainData;
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00041BAA File Offset: 0x0003FDAA
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.CalculateTerrainSquares();
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00041BBC File Offset: 0x0003FDBC
	private void OnDrawGizmos()
	{
		if (this.usedTextureCounts == null)
		{
			return;
		}
		Bounds bounds = this.terrain.terrainData.bounds;
		Vector3 vector = bounds.extents * 2f / (float)this.usedTextureCounts.GetLength(0);
		vector.y = bounds.extents.y * 2f;
		for (int i = 0; i < this.usedTextureCounts.GetLength(0); i++)
		{
			for (int j = 0; j < this.usedTextureCounts.GetLength(1); j++)
			{
				int num = this.usedTextureCounts[i, j];
				if (num <= this.firstTextureLimit)
				{
					Gizmos.color = Color.green;
				}
				else if (num <= this.secondTextureLimit)
				{
					Gizmos.color = Color.yellow;
				}
				else
				{
					Gizmos.color = Color.Lerp(Color.red, Color.black, (float)(num - (this.secondTextureLimit + 1)) / 4f);
				}
				Color color = Gizmos.color;
				color.a = this.gizmoAlpha;
				Gizmos.color = color;
				Gizmos.DrawCube(bounds.min + new Vector3((float)i * vector.x, 0f, (float)j * vector.z) + vector * 0.5f, new Vector3(vector.x, 1f, vector.z));
			}
		}
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00041D2C File Offset: 0x0003FF2C
	[ContextMenu("CalculateTerrainSquares")]
	private void CalculateTerrainSquares()
	{
		this.firstSplat = this.terrainData.GetAlphamapTexture(0);
		int num = this.terrainData.alphamapWidth / this.squareSize;
		int num2 = this.terrainData.alphamapHeight / this.squareSize;
		int num3 = 0;
		int num4 = 100;
		StringBuilder stringBuilder = new StringBuilder();
		this.usedTextureCounts = new int[num, num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num5 = 0;
				for (int k = 0; k < this.terrainData.alphamapTextureCount; k++)
				{
					Color[] pixels = this.terrainData.GetAlphamapTexture(k).GetPixels(j * this.squareSize, i * this.squareSize, this.squareSize, this.squareSize, 0);
					Vector4 zero = Vector4.zero;
					foreach (Color color in pixels)
					{
						zero.x = Mathf.Max(zero.x, color.r);
						zero.y = Mathf.Max(zero.y, color.g);
						zero.z = Mathf.Max(zero.z, color.b);
						zero.w = Mathf.Max(zero.w, color.a);
					}
					if (zero.x > this.minSplatWeight)
					{
						num5++;
					}
					if (zero.y > this.minSplatWeight)
					{
						num5++;
					}
					if (zero.z > this.minSplatWeight)
					{
						num5++;
					}
					if (zero.w > this.minSplatWeight)
					{
						num5++;
					}
				}
				num3 = Mathf.Max(num3, num5);
				num4 = Mathf.Min(num4, num5);
				stringBuilder.Append(string.Format("{0,2} ", num5));
				this.usedTextureCounts[j, i] = num5;
			}
			stringBuilder.AppendLine();
		}
		Debug.Log(string.Format("Max: {0}, Min: {1}. Data: \n{2}", num3, num4, stringBuilder));
	}

	// Token: 0x040005A5 RID: 1445
	public Terrain terrain;

	// Token: 0x040005A6 RID: 1446
	public Texture2D firstSplat;

	// Token: 0x040005A7 RID: 1447
	public int squareSize = 64;

	// Token: 0x040005A8 RID: 1448
	public float minSplatWeight = 0.05f;

	// Token: 0x040005A9 RID: 1449
	public int secondTextureLimit = 8;

	// Token: 0x040005AA RID: 1450
	public int firstTextureLimit = 4;

	// Token: 0x040005AB RID: 1451
	public float gizmoAlpha = 0.9f;

	// Token: 0x040005AC RID: 1452
	private int[,] usedTextureCounts;
}
