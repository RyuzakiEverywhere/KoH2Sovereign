using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

// Token: 0x02000092 RID: 146
public struct CacheFloat2DData
{
	// Token: 0x0600056E RID: 1390 RVA: 0x0003CA10 File Offset: 0x0003AC10
	public void Dispose()
	{
		AllocationManager.ReleaseGCObject(ref this.h_values);
	}

	// Token: 0x04000509 RID: 1289
	public int2 resolution;

	// Token: 0x0400050A RID: 1290
	public float2 inv_cell_size;

	// Token: 0x0400050B RID: 1291
	[NativeDisableUnsafePtrRestriction]
	public unsafe float* values;

	// Token: 0x0400050C RID: 1292
	public ulong h_values;
}
