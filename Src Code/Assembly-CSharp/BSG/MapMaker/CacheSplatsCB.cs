using System;
using BSG.MapMaker.Nodes;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker
{
	// Token: 0x0200037D RID: 893
	[BurstCompile(CompileSynchronously = true)]
	public class CacheSplatsCB
	{
		// Token: 0x0600345C RID: 13404 RVA: 0x001A3238 File Offset: 0x001A1438
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetCachedTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref CacheSplatsCBData ptr = ref *(CacheSplatsCBData*)data;
			int num = (int)(wy * ptr.inv_cell_size.y);
			int num2 = (int)(wx * ptr.inv_cell_size.x);
			int num3 = ptr.resolution.x * ptr.resolution.y * layers;
			int num4 = num * ptr.resolution.y * layers + num2 * layers;
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 >= num3)
			{
				num4 = num3 - 1;
			}
			float* ptr2 = ptr.alphas + num4;
			for (int i = 0; i < layers; i++)
			{
				float num5 = ptr2[i];
				result[i] = num5;
			}
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x001A32DC File Offset: 0x001A14DC
		public unsafe void Initialize()
		{
			this.cache_arr_data = new NativeArray<CacheSplatsCBData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.cache_pdata = (CacheSplatsCBData*)this.cache_arr_data.GetUnsafeReadOnlyPtr<CacheSplatsCBData>();
			if (CacheSplatsCB.get_tile_cached_funcs == IntPtr.Zero)
			{
				CacheSplatsCB.get_tile_cached_funcs = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(CacheSplatsCB.GetCachedTile)).Value;
			}
		}

		// Token: 0x0600345E RID: 13406 RVA: 0x001A3337 File Offset: 0x001A1537
		public void CleanUp()
		{
			this.cache_pdata = null;
			this.cache_arr_data.Dispose();
			this.cached_values = null;
		}

		// Token: 0x0600345F RID: 13407 RVA: 0x001A3354 File Offset: 0x001A1554
		public unsafe GetSplatCB GetCallback()
		{
			return new GetSplatCB
			{
				get_tile_func = CacheSplatsCB.get_tile_cached_funcs,
				data = (void*)this.cache_pdata
			};
		}

		// Token: 0x06003460 RID: 13408 RVA: 0x001A3383 File Offset: 0x001A1583
		public bool IsCached()
		{
			return this.cached_values != null;
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x001A3390 File Offset: 0x001A1590
		public unsafe void CacheData(NodePort outputPort, GetSplatCB output, int baseResolution, float2 terrainSize, int layers)
		{
			ref CacheSplatsCBData ptr = ref *this.cache_pdata;
			if (output.data == null)
			{
				return;
			}
			if (NodeHelper.GetConnectionCount(outputPort) <= 1)
			{
				return;
			}
			float2 tileSize = terrainSize / (float)baseResolution;
			this.cached_values = NodeHelper.SampleSplatInput(output, baseResolution, tileSize, layers);
			if (this.cached_values != null)
			{
				ptr.alphas = (float*)AllocationManager.PinGCArrayAndGetDataAddress(this.cached_values, ref ptr.h_alphas);
			}
			ptr.resolution = baseResolution;
			float2 rhs = terrainSize / (ptr.resolution - 1);
			ptr.inv_cell_size = 1f / rhs;
		}

		// Token: 0x0400233F RID: 9023
		private float[,,] cached_values;

		// Token: 0x04002340 RID: 9024
		public static IntPtr get_tile_cached_funcs;

		// Token: 0x04002341 RID: 9025
		public NativeArray<CacheSplatsCBData> cache_arr_data;

		// Token: 0x04002342 RID: 9026
		public unsafe CacheSplatsCBData* cache_pdata;
	}
}
