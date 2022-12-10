using System;
using BSG.MapMaker.Nodes;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker
{
	// Token: 0x0200037C RID: 892
	[BurstCompile(CompileSynchronously = true)]
	public class CacheFloat2D
	{
		// Token: 0x06003455 RID: 13397 RVA: 0x001A3040 File Offset: 0x001A1240
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetCachedValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CacheFloat2DData ptr = ref *(CacheFloat2DData*)data;
			int num = (int)(wy * ptr.inv_cell_size.y);
			int num2 = (int)(wx * ptr.inv_cell_size.x);
			int num3 = ptr.resolution.x * ptr.resolution.y;
			int num4 = num * ptr.resolution.y + num2;
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 >= num3)
			{
				num4 = num3 - 1;
			}
			return ptr.values[num4];
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x001A30B8 File Offset: 0x001A12B8
		public unsafe void Initialize()
		{
			this.cache_arr_data = new NativeArray<CacheFloat2DData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.cache_pdata = (CacheFloat2DData*)this.cache_arr_data.GetUnsafeReadOnlyPtr<CacheFloat2DData>();
			if (CacheFloat2D.get_value_cached_funcs == IntPtr.Zero)
			{
				CacheFloat2D.get_value_cached_funcs = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CacheFloat2D.GetCachedValue)).Value;
			}
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x001A3113 File Offset: 0x001A1313
		public unsafe void CleanUp()
		{
			this.cache_pdata->Dispose();
			this.cache_pdata = null;
			this.cache_arr_data.Dispose();
			this.cached_values = null;
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x001A313C File Offset: 0x001A133C
		public unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = CacheFloat2D.get_value_cached_funcs,
				data = (void*)this.cache_pdata
			};
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x001A316B File Offset: 0x001A136B
		public bool IsCached()
		{
			return this.cached_values != null;
		}

		// Token: 0x0600345A RID: 13402 RVA: 0x001A3178 File Offset: 0x001A1378
		public unsafe void CacheData(NodePort outputPort, GetFloat2DCB output, int baseResolution, float2 terrainSize)
		{
			ref CacheFloat2DData ptr = ref *this.cache_pdata;
			if (output.data == null)
			{
				return;
			}
			if (NodeHelper.GetConnectionCount(outputPort) <= 1)
			{
				return;
			}
			int num = baseResolution;
			int2 @int = NodeHelper.GetOutputResolution(outputPort).x;
			if (@int.x > 0)
			{
				num = @int.x;
			}
			float2 tileSize = terrainSize / (float)(num - 1);
			this.cached_values = NodeHelper.SampleInput(output, num, tileSize);
			if (this.cached_values != null)
			{
				ptr.values = (float*)AllocationManager.PinGCArrayAndGetDataAddress(this.cached_values, ref ptr.h_values);
			}
			ptr.resolution = num;
			float2 rhs = terrainSize / (ptr.resolution - 1);
			ptr.inv_cell_size = 1f / rhs;
		}

		// Token: 0x0400233B RID: 9019
		private float[,] cached_values;

		// Token: 0x0400233C RID: 9020
		public static IntPtr get_value_cached_funcs;

		// Token: 0x0400233D RID: 9021
		public NativeArray<CacheFloat2DData> cache_arr_data;

		// Token: 0x0400233E RID: 9022
		public unsafe CacheFloat2DData* cache_pdata;
	}
}
