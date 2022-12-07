using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
[Serializable]
public struct BSGTerrainCell
{
	// Token: 0x060005E9 RID: 1513 RVA: 0x000406A4 File Offset: 0x0003E8A4
	public static Bounds CalculateCellBounds(Bounds chunkBounds, int cellSingleDimensionCount, int cellZ, int cellX)
	{
		Vector3 size = chunkBounds.size;
		size.x /= (float)cellSingleDimensionCount;
		size.z /= (float)cellSingleDimensionCount;
		Vector3 vector = chunkBounds.min + new Vector3(size.x * (float)cellX, 0f, size.z * (float)cellZ);
		Vector3 point = vector + size;
		Bounds result = new Bounds(vector, Vector3.zero);
		result.Encapsulate(point);
		return result;
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0004071C File Offset: 0x0003E91C
	public static RectInt CalculateCellAlphamapBounds(RectInt alphamapRect, int cellSingleDimensionCount, int cellZ, int cellX)
	{
		Vector2Int a = (alphamapRect.size + Vector2Int.one) / cellSingleDimensionCount;
		return new RectInt(alphamapRect.min + a * new Vector2Int(cellX, cellZ), a - Vector2Int.one);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x000023FD File Offset: 0x000005FD
	public void Cleanup()
	{
	}

	// Token: 0x04000575 RID: 1397
	public Bounds bounds;

	// Token: 0x04000576 RID: 1398
	public Matrix4x4 object_to_world_matrix;

	// Token: 0x04000577 RID: 1399
	public RectInt alphamap_bouds;

	// Token: 0x04000578 RID: 1400
	public int alphamap_index;

	// Token: 0x04000579 RID: 1401
	public Vector4 primaryMappings;

	// Token: 0x0400057A RID: 1402
	public Vector4 secondaryMappings;
}
