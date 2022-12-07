using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

// Token: 0x02000093 RID: 147
public struct CacheSplatsCBData
{
	// Token: 0x0600056F RID: 1391 RVA: 0x0003CA1D File Offset: 0x0003AC1D
	public void Dispose()
	{
		AllocationManager.ReleaseGCObject(ref this.h_alphas);
	}

	// Token: 0x0400050D RID: 1293
	public int2 resolution;

	// Token: 0x0400050E RID: 1294
	public float2 inv_cell_size;

	// Token: 0x0400050F RID: 1295
	public int layers;

	// Token: 0x04000510 RID: 1296
	[NativeDisableUnsafePtrRestriction]
	public unsafe float* alphas;

	// Token: 0x04000511 RID: 1297
	public ulong h_alphas;
}
