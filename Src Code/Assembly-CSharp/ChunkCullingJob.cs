using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000AC RID: 172
[BurstCompile(CompileSynchronously = true)]
public struct ChunkCullingJob : IJob
{
	// Token: 0x06000607 RID: 1543 RVA: 0x00041868 File Offset: 0x0003FA68
	public void Execute()
	{
		int num = 0;
		for (int i = 0; i < this.input_cells.Length; i++)
		{
			if (this.TestPlanesAABB(this.frustrum_planes, this.input_cells[i].bounds))
			{
				this.primary_alphamapping[num] = this.input_cells[i].primaryMappings;
				this.secondary_alphamapping[num] = this.input_cells[i].secondaryMappings;
				this.matrices_array[num] = this.input_cells[i].object_to_world_matrix;
				this.alphamap_indices[num] = (float)this.input_cells[i].alphamap_index;
				num++;
			}
		}
		this.output_count[0] = num;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00041938 File Offset: 0x0003FB38
	public bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)
	{
		for (int i = 0; i < planes.Length; i++)
		{
			Plane plane = planes[i];
			float3 @float = plane.normal;
			float distance = plane.distance;
			float3 rhs = math.sign(@float);
			if (math.dot(bounds.center + bounds.extents * rhs, @float) + distance < 0f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400059A RID: 1434
	[ReadOnly]
	public NativeArray<BSGTerrainCell> input_cells;

	// Token: 0x0400059B RID: 1435
	[ReadOnly]
	public NativeArray<Plane> frustrum_planes;

	// Token: 0x0400059C RID: 1436
	[WriteOnly]
	public NativeArray<Vector4> primary_alphamapping;

	// Token: 0x0400059D RID: 1437
	[WriteOnly]
	public NativeArray<Vector4> secondary_alphamapping;

	// Token: 0x0400059E RID: 1438
	[WriteOnly]
	public NativeArray<float> alphamap_indices;

	// Token: 0x0400059F RID: 1439
	[WriteOnly]
	public NativeArray<Matrix4x4> matrices_array;

	// Token: 0x040005A0 RID: 1440
	[WriteOnly]
	public NativeArray<int> output_count;
}
