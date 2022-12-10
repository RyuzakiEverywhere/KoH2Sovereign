using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038C RID: 908
	[BurstCompile(CompileSynchronously = true)]
	public struct SampleFloatsJob : IJobParallelFor
	{
		// Token: 0x060034B4 RID: 13492 RVA: 0x001A5944 File Offset: 0x001A3B44
		public unsafe void Execute(int y)
		{
			float wy = (float)y * this.tile_size.y;
			for (int i = 0; i < this.resolution; i++)
			{
				float wx = (float)i * this.tile_size.x;
				float value = this.cb.GetValue(wx, wy);
				this.poutputs[y * this.resolution + i] = value;
			}
		}

		// Token: 0x0400239F RID: 9119
		public int resolution;

		// Token: 0x040023A0 RID: 9120
		public float2 tile_size;

		// Token: 0x040023A1 RID: 9121
		[NativeDisableUnsafePtrRestriction]
		public unsafe float* poutputs;

		// Token: 0x040023A2 RID: 9122
		public GetFloat2DCB cb;
	}
}
