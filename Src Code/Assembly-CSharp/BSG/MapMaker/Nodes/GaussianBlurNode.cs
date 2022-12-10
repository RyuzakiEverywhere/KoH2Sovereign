using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000389 RID: 905
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/GaussianBlur")]
	[Serializable]
	public class GaussianBlurNode : Node
	{
		// Token: 0x060034A2 RID: 13474 RVA: 0x001A4F90 File Offset: 0x001A3190
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref GaussianBlurNode.Data ptr = ref *(GaussianBlurNode.Data*)data;
			if (ptr.floats == null)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			float num = ptr.strength;
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
			}
			int num2 = (int)(wy * ptr.inv_cell_size.y);
			int num3 = (int)(wx * ptr.inv_cell_size.x);
			int num4 = ptr.resolution.x * ptr.resolution.y;
			int num5 = num2 * ptr.resolution.y + num3;
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num5 >= num4)
			{
				num5 = num4 - 1;
			}
			float y = GaussianBlurNode.GaussBlurSinglePoint(ptr.floats, ptr.kernel, ptr.resolution.x, ptr.resolution.y, ptr.radius, num2, num3);
			num = math.clamp(num, 0f, 1f);
			return math.lerp(ptr.floats[num5], y, num);
		}

		// Token: 0x060034A3 RID: 13475 RVA: 0x001A50A0 File Offset: 0x001A32A0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<GaussianBlurNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (GaussianBlurNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<GaussianBlurNode.Data>();
			if (GaussianBlurNode.get_value_funcs == IntPtr.Zero)
			{
				GaussianBlurNode.get_value_funcs = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(GaussianBlurNode.GetValue)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x060034A4 RID: 13476 RVA: 0x001A5106 File Offset: 0x001A3306
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
			this.res_cache.CleanUp();
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x001A5130 File Offset: 0x001A3330
		public override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			if (this.res_cache.IsCached())
			{
				return this.res_cache.GetCallback();
			}
			return this.GetCallback();
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x001A5180 File Offset: 0x001A3380
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = GaussianBlurNode.get_value_funcs,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x001A51AF File Offset: 0x001A33AF
		private void FillData()
		{
			this.PrepareData();
			this.isDataFilled = true;
		}

		// Token: 0x060034A8 RID: 13480 RVA: 0x001A51C0 File Offset: 0x001A33C0
		private unsafe void PrepareData()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			GetFloat2DCB inputValue = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
			if (inputValue.data == null)
			{
				return;
			}
			ref GaussianBlurNode.Data ptr = ref *this.pdata;
			ptr.in1 = inputValue;
			ptr.mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr.radius = this.radius;
			ptr.strength = this.strength;
			int heightmapResolution = terrainData.heightmapResolution;
			ptr.resolution = heightmapResolution;
			float2 @float = new float2(terrainData.size.x, terrainData.size.z) / (ptr.resolution - 1);
			ptr.inv_cell_size = 1f / @float;
			float[,] target = NodeHelper.SampleInput(inputValue, heightmapResolution, @float);
			float[,] target2 = this.CalculateGaussKernel(this.radius);
			ptr.kernel = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target2, ref ptr.h_kernel);
			ptr.floats = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target, ref ptr.h_floats);
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				NodePort outputPort = base.GetOutputPort("res");
				GetFloat2DCB callback = this.GetCallback();
				this.res_cache.CacheData(outputPort, callback, heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
			}
		}

		// Token: 0x060034A9 RID: 13481 RVA: 0x001A5338 File Offset: 0x001A3538
		public float[,] CalculateGaussKernel(float weight)
		{
			int num = (int)weight * 2 + 1;
			float[,] array = new float[num, num];
			double num2 = 0.0;
			int num3 = (num - 1) / 2;
			float num4 = 1f / (6.2831855f * weight * weight);
			for (int i = -num3; i <= num3; i++)
			{
				for (int j = -num3; j <= num3; j++)
				{
					float num5 = (float)(i * i + j * j) / (2f * weight * weight);
					array[i + num3, j + num3] = num4 * (float)Math.Exp((double)(-(double)num5));
					num2 += (double)array[i + num3, j + num3];
				}
			}
			return array;
		}

		// Token: 0x060034AA RID: 13482 RVA: 0x001A53E0 File Offset: 0x001A35E0
		public unsafe static float GaussBlurSinglePoint(float* scl, float* kernel, int w, int h, float r, int wy, int wx)
		{
			int num = (int)r;
			double num2 = 0.0;
			double num3 = 0.0;
			int num4 = ((int)(r * 2f + 1f) - 1) / 2;
			for (int i = wy - num; i < wy + num + 1; i++)
			{
				for (int j = wx - num; j < wx + num + 1; j++)
				{
					int num5 = math.min(w - 1, math.max(0, j));
					int num6 = math.min(h - 1, math.max(0, i));
					int num7 = j - wx + num4;
					int num8 = i - wy + num4;
					double num9 = (double)kernel[num8 * num + num7];
					num2 += (double)scl[num6 * h + num5] * num9;
					num3 += num9;
				}
			}
			return (float)(num2 / num3);
		}

		// Token: 0x04002394 RID: 9108
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB in1;

		// Token: 0x04002395 RID: 9109
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x04002396 RID: 9110
		public float strength = 1f;

		// Token: 0x04002397 RID: 9111
		[Space]
		[Min(0f)]
		public float radius = 1f;

		// Token: 0x04002398 RID: 9112
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x04002399 RID: 9113
		public bool cache = true;

		// Token: 0x0400239A RID: 9114
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x0400239B RID: 9115
		private bool isDataFilled;

		// Token: 0x0400239C RID: 9116
		public static IntPtr get_value_funcs;

		// Token: 0x0400239D RID: 9117
		public NativeArray<GaussianBlurNode.Data> arr_data;

		// Token: 0x0400239E RID: 9118
		public unsafe GaussianBlurNode.Data* pdata;

		// Token: 0x020008C3 RID: 2243
		public struct Data
		{
			// Token: 0x060051FD RID: 20989 RVA: 0x0023EE9A File Offset: 0x0023D09A
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_floats);
				AllocationManager.ReleaseGCObject(ref this.h_kernel);
			}

			// Token: 0x040040CD RID: 16589
			public GetFloat2DCB in1;

			// Token: 0x040040CE RID: 16590
			public GetFloat2DCB mask;

			// Token: 0x040040CF RID: 16591
			public float strength;

			// Token: 0x040040D0 RID: 16592
			public int2 resolution;

			// Token: 0x040040D1 RID: 16593
			public float2 inv_cell_size;

			// Token: 0x040040D2 RID: 16594
			public float radius;

			// Token: 0x040040D3 RID: 16595
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* floats;

			// Token: 0x040040D4 RID: 16596
			public ulong h_floats;

			// Token: 0x040040D5 RID: 16597
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* kernel;

			// Token: 0x040040D6 RID: 16598
			public ulong h_kernel;
		}
	}
}
