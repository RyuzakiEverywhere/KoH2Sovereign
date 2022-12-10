using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000382 RID: 898
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Combiner")]
	[Serializable]
	public class CombinerNode : Node
	{
		// Token: 0x06003479 RID: 13433 RVA: 0x001A3F68 File Offset: 0x001A2168
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float BlendFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			if (num >= 1f)
			{
				return ptr.in2.GetValue(wx, wy);
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			return (1f - num) * value + num * value2;
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x001A4014 File Offset: 0x001A2214
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float AddFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			return value + value2 * num;
		}

		// Token: 0x0600347B RID: 13435 RVA: 0x001A40A0 File Offset: 0x001A22A0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float MinFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			if (value >= value2)
			{
				return value2;
			}
			return value;
		}

		// Token: 0x0600347C RID: 13436 RVA: 0x001A4130 File Offset: 0x001A2330
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float MaxFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			if (value <= value2)
			{
				return value2;
			}
			return value;
		}

		// Token: 0x0600347D RID: 13437 RVA: 0x001A41C0 File Offset: 0x001A23C0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float AverageFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			return (value + value2) / 2f;
		}

		// Token: 0x0600347E RID: 13438 RVA: 0x001A4250 File Offset: 0x001A2450
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float SubtractFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			return value - value2 * num;
		}

		// Token: 0x0600347F RID: 13439 RVA: 0x001A42DC File Offset: 0x001A24DC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float MultiplyFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			return value * value2 * num;
		}

		// Token: 0x06003480 RID: 13440 RVA: 0x001A4368 File Offset: 0x001A2568
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float ModulateFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CombinerNode.Data ptr = ref *(CombinerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					return ptr.in1.GetValue(wx, wy);
				}
			}
			float value = ptr.in1.GetValue(wx, wy);
			float value2 = ptr.in2.GetValue(wx, wy);
			float num2 = (ptr.range.y - ptr.range.x) * 0.5f;
			float num3 = value2 - num2;
			num3 = Mathf.Abs(num3);
			float b = (value + num3) % 1f;
			return Mathf.Lerp(value, b, num);
		}

		// Token: 0x06003481 RID: 13441 RVA: 0x001A442C File Offset: 0x001A262C
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<CombinerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (CombinerNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<CombinerNode.Data>();
			if (CombinerNode.get_value_funcs == null)
			{
				CombinerNode.get_value_funcs = new IntPtr[8];
				CombinerNode.get_value_funcs[0] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.BlendFunc)).Value;
				CombinerNode.get_value_funcs[1] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.AddFunc)).Value;
				CombinerNode.get_value_funcs[2] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MinFunc)).Value;
				CombinerNode.get_value_funcs[3] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MaxFunc)).Value;
				CombinerNode.get_value_funcs[4] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.AverageFunc)).Value;
				CombinerNode.get_value_funcs[5] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.SubtractFunc)).Value;
				CombinerNode.get_value_funcs[6] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MultiplyFunc)).Value;
				CombinerNode.get_value_funcs[7] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.ModulateFunc)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x06003482 RID: 13442 RVA: 0x001A4578 File Offset: 0x001A2778
		private void CalculateRangeFloat2DCB()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			int heightmapResolution = terrainData.heightmapResolution;
			this.in2 = base.GetInputValue<GetFloat2DCB>("in2", default(GetFloat2DCB));
			if (this.in2.data == null)
			{
				return;
			}
			using (NativeQueue<float> mins = new NativeQueue<float>(Allocator.TempJob))
			{
				using (NativeQueue<float> maxs = new NativeQueue<float>(Allocator.TempJob))
				{
					using (NativeArray<float> nativeArray = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
					{
						using (NativeArray<float> nativeArray2 = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
						{
							CombinerNode.RangeJob jobData = default(CombinerNode.RangeJob);
							jobData.resolution = heightmapResolution;
							jobData.Min = mins.AsParallelWriter();
							jobData.Max = maxs.AsParallelWriter();
							jobData.input = this.in2;
							if (this.SingleThreaded)
							{
								jobData.Run(heightmapResolution);
							}
							else
							{
								jobData.Schedule(heightmapResolution, 16, default(JobHandle)).Complete();
							}
							CombinerNode.RangeCombineJob jobData2 = new CombinerNode.RangeCombineJob
							{
								Mins = mins,
								Maxs = maxs,
								Min = nativeArray,
								Max = nativeArray2
							};
							jobData2.Schedule(default(JobHandle)).Complete();
							this.min = jobData2.Min[0];
							this.max = jobData2.Max[0];
						}
					}
				}
			}
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x001A4770 File Offset: 0x001A2970
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.res_cache.CleanUp();
			this.min = float.MaxValue;
			this.max = float.MinValue;
			this.isDataFilled = false;
		}

		// Token: 0x06003484 RID: 13444 RVA: 0x001A47B0 File Offset: 0x001A29B0
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

		// Token: 0x06003485 RID: 13445 RVA: 0x001A4800 File Offset: 0x001A2A00
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = CombinerNode.get_value_funcs[(int)this.operation],
				data = (void*)this.pdata
			};
		}

		// Token: 0x06003486 RID: 13446 RVA: 0x001A4838 File Offset: 0x001A2A38
		private unsafe void FillData()
		{
			CombinerNode.Data* ptr = this.pdata;
			ptr->in1 = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
			ptr->in2 = base.GetInputValue<GetFloat2DCB>("in2", default(GetFloat2DCB));
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->strength = this.strength;
			if (this.operation == CombineOperationType.Modulate)
			{
				this.CalculateRangeFloat2DCB();
			}
			ptr->range = new float2(this.min, this.max);
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					NodePort outputPort = base.GetOutputPort("res");
					GetFloat2DCB callback = this.GetCallback();
					this.res_cache.CacheData(outputPort, callback, terrainData.heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x0400236A RID: 9066
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB in1;

		// Token: 0x0400236B RID: 9067
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB in2;

		// Token: 0x0400236C RID: 9068
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x0400236D RID: 9069
		public float strength = 1f;

		// Token: 0x0400236E RID: 9070
		public bool SingleThreaded;

		// Token: 0x0400236F RID: 9071
		public CombineOperationType operation;

		// Token: 0x04002370 RID: 9072
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x04002371 RID: 9073
		public bool cache = true;

		// Token: 0x04002372 RID: 9074
		private float min = float.MaxValue;

		// Token: 0x04002373 RID: 9075
		private float max = float.MinValue;

		// Token: 0x04002374 RID: 9076
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x04002375 RID: 9077
		private bool isDataFilled;

		// Token: 0x04002376 RID: 9078
		public static IntPtr[] get_value_funcs;

		// Token: 0x04002377 RID: 9079
		public NativeArray<CombinerNode.Data> arr_data;

		// Token: 0x04002378 RID: 9080
		public unsafe CombinerNode.Data* pdata;

		// Token: 0x020008BF RID: 2239
		public struct Data
		{
			// Token: 0x040040BC RID: 16572
			public GetFloat2DCB in1;

			// Token: 0x040040BD RID: 16573
			public GetFloat2DCB in2;

			// Token: 0x040040BE RID: 16574
			public GetFloat2DCB mask;

			// Token: 0x040040BF RID: 16575
			public float strength;

			// Token: 0x040040C0 RID: 16576
			public float2 range;
		}

		// Token: 0x020008C0 RID: 2240
		[BurstCompile(CompileSynchronously = true)]
		public struct RangeJob : IJobParallelFor
		{
			// Token: 0x060051FA RID: 20986 RVA: 0x0023EDCC File Offset: 0x0023CFCC
			public void Execute(int y)
			{
				float num = float.MaxValue;
				float num2 = float.MinValue;
				for (int i = 0; i < this.resolution; i++)
				{
					float value = this.input.GetValue((float)i, (float)y);
					if (value > num2)
					{
						num2 = value;
					}
					if (value < num)
					{
						num = value;
					}
				}
				this.Min.Enqueue(num);
				this.Max.Enqueue(num2);
			}

			// Token: 0x040040C1 RID: 16577
			public int resolution;

			// Token: 0x040040C2 RID: 16578
			public GetFloat2DCB input;

			// Token: 0x040040C3 RID: 16579
			public NativeQueue<float>.ParallelWriter Min;

			// Token: 0x040040C4 RID: 16580
			public NativeQueue<float>.ParallelWriter Max;
		}

		// Token: 0x020008C1 RID: 2241
		[BurstCompile]
		public struct RangeCombineJob : IJob
		{
			// Token: 0x060051FB RID: 20987 RVA: 0x0023EE2C File Offset: 0x0023D02C
			public void Execute()
			{
				float num = float.MaxValue;
				float num2;
				while (this.Mins.TryDequeue(out num2))
				{
					if (num2 < num)
					{
						num = num2;
					}
				}
				float num3 = float.MinValue;
				float num4;
				while (this.Maxs.TryDequeue(out num4))
				{
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
				this.Min[0] = num;
				this.Max[0] = num3;
			}

			// Token: 0x040040C5 RID: 16581
			public NativeQueue<float> Mins;

			// Token: 0x040040C6 RID: 16582
			public NativeQueue<float> Maxs;

			// Token: 0x040040C7 RID: 16583
			public NativeArray<float> Min;

			// Token: 0x040040C8 RID: 16584
			public NativeArray<float> Max;
		}
	}
}
