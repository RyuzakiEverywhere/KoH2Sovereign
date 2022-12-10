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
	// Token: 0x020003AF RID: 943
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/SnapTo")]
	[Serializable]
	public class SnapToNode : Node
	{
		// Token: 0x0600357D RID: 13693 RVA: 0x001ACDD0 File Offset: 0x001AAFD0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetSnapValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SnapToNode.Data ptr = ref *(SnapToNode.Data*)data;
			float num = ptr.in1.GetValue(wx, wy);
			if (ptr.snap_to_min)
			{
				num -= ptr.min_value;
			}
			if (ptr.snap_to_max)
			{
				num -= ptr.max_value;
			}
			if (ptr.snap_to_avg)
			{
				num -= ptr.avg_value;
			}
			if (ptr.snap_to_mid)
			{
				num -= ptr.mid_value;
			}
			return num;
		}

		// Token: 0x0600357E RID: 13694 RVA: 0x001ACE40 File Offset: 0x001AB040
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SnapToNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SnapToNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SnapToNode.Data>();
			if (SnapToNode.get_value_funcs == IntPtr.Zero)
			{
				SnapToNode.get_value_funcs = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SnapToNode.GetSnapValue)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x0600357F RID: 13695 RVA: 0x001ACEA8 File Offset: 0x001AB0A8
		private void CalculateSnapValues()
		{
			if (this.snapToMid || this.snapToMin || this.snapToMax || this.snapToAvg)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData == null)
				{
					return;
				}
				int heightmapResolution = terrainData.heightmapResolution;
				GetFloat2DCB inputValue = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
				if (inputValue.data == null)
				{
					return;
				}
				float2 tileSize = new float2(terrainData.size.x, terrainData.size.z) / (float)(heightmapResolution - 1);
				float[,] inputValues = NodeHelper.SampleInput(inputValue, heightmapResolution, tileSize);
				if (this.snapToMin || this.snapToMax || this.snapToAvg)
				{
					float2 @float = this.CalculateRangeValue(inputValues, heightmapResolution, out this.avg_value);
					this.min_value = @float.x;
					this.max_value = @float.y;
				}
				if (this.snapToMid)
				{
					this.mid_value = this.GetMedian(inputValues);
				}
			}
		}

		// Token: 0x06003580 RID: 13696 RVA: 0x001ACFAC File Offset: 0x001AB1AC
		private float GetMedian(float[,] inputValues)
		{
			float[] array = new float[inputValues.GetLength(0) * inputValues.GetLength(1)];
			int length = inputValues.GetLength(0);
			for (int i = 0; i < inputValues.GetLength(0); i++)
			{
				for (int j = 0; j < inputValues.GetLength(1); j++)
				{
					array[i * length + j] = inputValues[i, j];
				}
			}
			using (NativeArray<float> entries = new NativeArray<float>(array.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory))
			{
				entries.CopyFrom(array);
				new QuickSortJob
				{
					entries = entries
				}.Run<QuickSortJob>();
				array = entries.ToArray();
			}
			int num = array.Length;
			int num2 = num / 2;
			if (num % 2 == 0)
			{
				return (array[num2] + array[num2 - 1]) / 2f;
			}
			return array[num2];
		}

		// Token: 0x06003581 RID: 13697 RVA: 0x001AD084 File Offset: 0x001AB284
		private unsafe float2 CalculateRangeValue(float[,] inputValues, int resolution, out float avg_value)
		{
			float2 result;
			using (NativeQueue<float> mins = new NativeQueue<float>(Allocator.TempJob))
			{
				using (NativeQueue<float> maxs = new NativeQueue<float>(Allocator.TempJob))
				{
					using (NativeQueue<float> avgs = new NativeQueue<float>(Allocator.TempJob))
					{
						using (NativeArray<float> min = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
						{
							using (NativeArray<float> max = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
							{
								using (NativeArray<float> avg = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
								{
									SnapToNode.RangeJob jobData = default(SnapToNode.RangeJob);
									jobData.resolution = resolution;
									jobData.Min = mins.AsParallelWriter();
									jobData.Max = maxs.AsParallelWriter();
									jobData.Avg = avgs.AsParallelWriter();
									jobData.inputs = (float*)AllocationManager.PinGCArrayAndGetDataAddress(inputValues, ref jobData.h_inputs);
									if (this.SingleThreaded)
									{
										jobData.Run(resolution);
									}
									else
									{
										jobData.Schedule(resolution, 16, default(JobHandle)).Complete();
									}
									AllocationManager.ReleaseGCObject(ref jobData.h_inputs);
									SnapToNode.RangeCombineJob jobData2 = new SnapToNode.RangeCombineJob
									{
										Mins = mins,
										Maxs = maxs,
										Avgs = avgs,
										Min = min,
										Max = max,
										Avg = avg
									};
									jobData2.Schedule(default(JobHandle)).Complete();
									result = new float2(jobData2.Min[0], jobData2.Max[0]);
									avg_value = jobData2.Avg[0];
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x001AD2C0 File Offset: 0x001AB4C0
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
			this.min_value = float.MaxValue;
			this.max_value = float.MinValue;
			this.mid_value = 0f;
			this.avg_value = 0f;
			this.res_cache.CleanUp();
		}

		// Token: 0x06003583 RID: 13699 RVA: 0x001AD320 File Offset: 0x001AB520
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

		// Token: 0x06003584 RID: 13700 RVA: 0x001AD370 File Offset: 0x001AB570
		private unsafe void FillData()
		{
			SnapToNode.Data* ptr = this.pdata;
			ptr->in1 = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
			ptr->snap_to_min = this.snapToMin;
			ptr->snap_to_max = this.snapToMax;
			ptr->snap_to_avg = this.snapToAvg;
			ptr->snap_to_mid = this.snapToMid;
			this.CalculateSnapValues();
			ptr->min_value = this.min_value;
			ptr->max_value = this.max_value;
			ptr->avg_value = this.avg_value;
			ptr->mid_value = this.mid_value;
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

		// Token: 0x06003585 RID: 13701 RVA: 0x001AD47C File Offset: 0x001AB67C
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = SnapToNode.get_value_funcs,
				data = (void*)this.pdata
			};
		}

		// Token: 0x040024CB RID: 9419
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB in1;

		// Token: 0x040024CC RID: 9420
		[Space]
		public bool snapToMin;

		// Token: 0x040024CD RID: 9421
		public bool snapToMax;

		// Token: 0x040024CE RID: 9422
		public bool snapToAvg;

		// Token: 0x040024CF RID: 9423
		public bool snapToMid;

		// Token: 0x040024D0 RID: 9424
		[Space]
		public bool SingleThreaded;

		// Token: 0x040024D1 RID: 9425
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x040024D2 RID: 9426
		public bool cache = true;

		// Token: 0x040024D3 RID: 9427
		private float min_value;

		// Token: 0x040024D4 RID: 9428
		private float max_value;

		// Token: 0x040024D5 RID: 9429
		private float avg_value;

		// Token: 0x040024D6 RID: 9430
		private float mid_value;

		// Token: 0x040024D7 RID: 9431
		private bool isDataFilled;

		// Token: 0x040024D8 RID: 9432
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x040024D9 RID: 9433
		public static IntPtr get_value_funcs;

		// Token: 0x040024DA RID: 9434
		public NativeArray<SnapToNode.Data> arr_data;

		// Token: 0x040024DB RID: 9435
		public unsafe SnapToNode.Data* pdata;

		// Token: 0x020008F5 RID: 2293
		public struct Data
		{
			// Token: 0x040041B3 RID: 16819
			public GetFloat2DCB in1;

			// Token: 0x040041B4 RID: 16820
			public bool snap_to_min;

			// Token: 0x040041B5 RID: 16821
			public bool snap_to_max;

			// Token: 0x040041B6 RID: 16822
			public bool snap_to_avg;

			// Token: 0x040041B7 RID: 16823
			public bool snap_to_mid;

			// Token: 0x040041B8 RID: 16824
			public float min_value;

			// Token: 0x040041B9 RID: 16825
			public float max_value;

			// Token: 0x040041BA RID: 16826
			public float avg_value;

			// Token: 0x040041BB RID: 16827
			public float mid_value;
		}

		// Token: 0x020008F6 RID: 2294
		[BurstCompile(CompileSynchronously = true)]
		public struct RangeJob : IJobParallelFor
		{
			// Token: 0x0600522A RID: 21034 RVA: 0x0024046C File Offset: 0x0023E66C
			public unsafe void Execute(int y)
			{
				float num = float.MaxValue;
				float num2 = float.MinValue;
				float num3 = 0f;
				for (int i = 0; i < this.resolution; i++)
				{
					float num4 = this.inputs[y * this.resolution + i];
					if (num4 > num2)
					{
						num2 = num4;
					}
					if (num4 < num)
					{
						num = num4;
					}
					num3 += num4;
				}
				float value = num3 / (float)this.resolution;
				this.Min.Enqueue(num);
				this.Max.Enqueue(num2);
				this.Avg.Enqueue(value);
			}

			// Token: 0x040041BC RID: 16828
			public int resolution;

			// Token: 0x040041BD RID: 16829
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* inputs;

			// Token: 0x040041BE RID: 16830
			public ulong h_inputs;

			// Token: 0x040041BF RID: 16831
			public NativeQueue<float>.ParallelWriter Min;

			// Token: 0x040041C0 RID: 16832
			public NativeQueue<float>.ParallelWriter Max;

			// Token: 0x040041C1 RID: 16833
			public NativeQueue<float>.ParallelWriter Avg;
		}

		// Token: 0x020008F7 RID: 2295
		[BurstCompile]
		public struct RangeCombineJob : IJob
		{
			// Token: 0x0600522B RID: 21035 RVA: 0x00240504 File Offset: 0x0023E704
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
				float num5 = 0f;
				float num6 = (float)this.Avgs.Count;
				float num7;
				while (this.Avgs.TryDequeue(out num7))
				{
					num5 += num7;
				}
				float value = num5 / num6;
				this.Min[0] = num;
				this.Max[0] = num3;
				this.Avg[0] = value;
			}

			// Token: 0x040041C2 RID: 16834
			public NativeQueue<float> Mins;

			// Token: 0x040041C3 RID: 16835
			public NativeQueue<float> Maxs;

			// Token: 0x040041C4 RID: 16836
			public NativeQueue<float> Avgs;

			// Token: 0x040041C5 RID: 16837
			public NativeArray<float> Min;

			// Token: 0x040041C6 RID: 16838
			public NativeArray<float> Max;

			// Token: 0x040041C7 RID: 16839
			public NativeArray<float> Avg;
		}
	}
}
