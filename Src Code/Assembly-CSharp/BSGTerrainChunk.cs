using System;
using Unity.Collections;
using UnityEngine;

// Token: 0x020000A6 RID: 166
[Serializable]
public struct BSGTerrainChunk
{
	// Token: 0x060005EC RID: 1516 RVA: 0x0004076C File Offset: 0x0003E96C
	public static Bounds CalculateChunkBounds(Bounds terrainBounds, int chunkSingleDimensionCount, int chunkZ, int chunkX)
	{
		Vector3 vector = terrainBounds.extents * 2f;
		vector.x /= (float)chunkSingleDimensionCount;
		vector.z /= (float)chunkSingleDimensionCount;
		Vector3 vector2 = terrainBounds.min + new Vector3(vector.x * (float)chunkX, 0f, vector.z * (float)chunkZ);
		Vector3 point = vector2 + vector;
		Bounds result = new Bounds(vector2, Vector3.zero);
		result.Encapsulate(point);
		return result;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000407EC File Offset: 0x0003E9EC
	public static RectInt CalculateChunkAlphamapBounds(RectInt alphamapRect, int chunkSingleDimensionCount, int chunkZ, int chunkX)
	{
		Vector2Int a = (alphamapRect.size + Vector2Int.one) / chunkSingleDimensionCount;
		return new RectInt(alphamapRect.min + a * new Vector2Int(chunkX, chunkZ), a - Vector2Int.one);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0004083C File Offset: 0x0003EA3C
	public BSGTerrainCell GetCell(int cellX, int cellY)
	{
		int num = Mathf.RoundToInt(Mathf.Sqrt((float)this.cells.Length));
		return this.cells[cellY * num + cellX];
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00040870 File Offset: 0x0003EA70
	public void Cleanup()
	{
		if (Application.isPlaying)
		{
			if (this.primary_alphamap_array != null)
			{
				Object.Destroy(this.primary_alphamap_array);
			}
			if (this.secondary_alphamap_array != null)
			{
				Object.Destroy(this.secondary_alphamap_array);
			}
		}
		else
		{
			if (this.primary_alphamap_array != null)
			{
				Object.DestroyImmediate(this.primary_alphamap_array);
			}
			if (this.secondary_alphamap_array != null)
			{
				Object.DestroyImmediate(this.secondary_alphamap_array);
			}
		}
		foreach (BSGTerrainCell bsgterrainCell in this.cells)
		{
			bsgterrainCell.Cleanup();
		}
		this.native_cells_array.Dispose();
		this.cells = null;
	}

	// Token: 0x0400057B RID: 1403
	public Texture2DArray primary_alphamap_array;

	// Token: 0x0400057C RID: 1404
	public Texture2DArray secondary_alphamap_array;

	// Token: 0x0400057D RID: 1405
	public BSGTerrainCell[] cells;

	// Token: 0x0400057E RID: 1406
	public Bounds bounds;

	// Token: 0x0400057F RID: 1407
	public BoundingSphere boundingSphere;

	// Token: 0x04000580 RID: 1408
	public RectInt alphamap_bounds;

	// Token: 0x04000581 RID: 1409
	public const int CELL_COUNT_IN_SINGLE_DIMENSION = 16;

	// Token: 0x04000582 RID: 1410
	public BSGTerrainChunkRenderData bakedRenderData;

	// Token: 0x04000583 RID: 1411
	public NativeArray<BSGTerrainCell> native_cells_array;
}
