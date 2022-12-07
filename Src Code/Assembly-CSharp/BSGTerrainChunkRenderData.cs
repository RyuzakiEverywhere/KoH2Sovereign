using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class BSGTerrainChunkRenderData
{
	// Token: 0x060005F0 RID: 1520 RVA: 0x00040920 File Offset: 0x0003EB20
	public static BSGTerrainChunkRenderData FromChunk(BSGTerrainChunk chunk)
	{
		BSGTerrainChunkRenderData bsgterrainChunkRenderData = new BSGTerrainChunkRenderData();
		for (int i = 0; i < chunk.cells.Length; i++)
		{
			bsgterrainChunkRenderData.primary_alphamapping[i] = chunk.cells[i].primaryMappings;
			bsgterrainChunkRenderData.secondary_alphamapping[i] = chunk.cells[i].secondaryMappings;
			bsgterrainChunkRenderData.matrices_array[i] = chunk.cells[i].object_to_world_matrix;
			bsgterrainChunkRenderData.alphamap_indices[i] = (float)chunk.cells[i].alphamap_index;
		}
		return bsgterrainChunkRenderData;
	}

	// Token: 0x04000584 RID: 1412
	public Vector4[] primary_alphamapping = new Vector4[256];

	// Token: 0x04000585 RID: 1413
	public Vector4[] secondary_alphamapping = new Vector4[256];

	// Token: 0x04000586 RID: 1414
	public float[] alphamap_indices = new float[256];

	// Token: 0x04000587 RID: 1415
	public Matrix4x4[] matrices_array = new Matrix4x4[256];
}
