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
	// Token: 0x0200037E RID: 894
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Noise/AdvancedPerlinNoise")]
	[Serializable]
	public class AdvancedPerlinNoiseNode : Node
	{
		// Token: 0x06003463 RID: 13411 RVA: 0x001A342A File Offset: 0x001A162A
		public void OnValidate()
		{
			this.size = (float)((int)this.size);
			this.octaves = (float)((int)this.octaves);
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x001A3448 File Offset: 0x001A1648
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref AdvancedPerlinNoiseNode.Data ptr = ref *(AdvancedPerlinNoiseNode.Data*)data;
			return ptr.grid.GetValue(ptr.cache, 1, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x001A3480 File Offset: 0x001A1680
		public unsafe float[,] CreateNoise(int seed, int size, float scale, int octaves, float persistance, float lacunarity, float2 offset)
		{
			float[,] array = new float[size, size];
			System.Random random = new System.Random(seed);
			Vector2[] array2 = new Vector2[octaves];
			for (int i = 0; i < octaves; i++)
			{
				float x = (float)random.Next(-100000, 100000) + offset.x;
				float y = (float)random.Next(-100000, 100000) + offset.y;
				array2[i] = new Vector2(x, y);
			}
			if (scale <= 0f)
			{
				scale = 0.0001f;
			}
			float halfWidth = (float)size / 2f;
			float halfHeight = (float)size / 2f;
			float[] array3 = new float[size];
			float[] array4 = new float[size];
			for (int j = 0; j < size; j++)
			{
				array3[j] = float.MinValue;
				array4[j] = float.MaxValue;
			}
			fixed (float* ptr = &array[0, 0])
			{
				float* pnoiseMap = ptr;
				fixed (Vector2* ptr2 = &array2[0])
				{
					Vector2* poctaveOffsets = ptr2;
					fixed (float* ptr3 = &array3[0])
					{
						float* pmaxNoiseHeights = ptr3;
						fixed (float* ptr4 = &array4[0])
						{
							float* pminNoiseHeights = ptr4;
							AdvancedPerlinNoiseNode.NoiseMapJob jobData = default(AdvancedPerlinNoiseNode.NoiseMapJob);
							jobData.size = size;
							jobData.halfWidth = halfWidth;
							jobData.halfHeight = halfHeight;
							jobData.scale = scale;
							jobData.octaves = octaves;
							jobData.persistance = persistance;
							jobData.lacunarity = lacunarity;
							jobData.pnoiseMap = pnoiseMap;
							jobData.poctaveOffsets = poctaveOffsets;
							jobData.pmaxNoiseHeights = pmaxNoiseHeights;
							jobData.pminNoiseHeights = pminNoiseHeights;
							if (this.SingleThreaded)
							{
								jobData.Run(size);
							}
							else
							{
								jobData.Schedule(size, 16, default(JobHandle)).Complete();
							}
						}
					}
				}
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int k = 0; k < size; k++)
			{
				float num3 = array3[k];
				if (num3 > num)
				{
					num = num3;
				}
				num3 = array4[k];
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			fixed (float* ptr = &array[0, 0])
			{
				float* pnoiseMap2 = ptr;
				AdvancedPerlinNoiseNode.InverseLerpJob jobData2 = default(AdvancedPerlinNoiseNode.InverseLerpJob);
				jobData2.pnoiseMap = pnoiseMap2;
				jobData2.size = size;
				jobData2.maxNoiseHeight = num;
				jobData2.minNoiseHeight = num2;
				if (this.SingleThreaded)
				{
					jobData2.Run(size);
				}
				else
				{
					jobData2.Schedule(size, 16, default(JobHandle)).Complete();
				}
			}
			return array;
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x001A36E4 File Offset: 0x001A18E4
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<AdvancedPerlinNoiseNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (AdvancedPerlinNoiseNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<AdvancedPerlinNoiseNode.Data>();
			if (AdvancedPerlinNoiseNode.get_value_func == IntPtr.Zero)
			{
				AdvancedPerlinNoiseNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(AdvancedPerlinNoiseNode.GetValue)).Value;
			}
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x001A373F File Offset: 0x001A193F
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x001A3768 File Offset: 0x001A1968
		private unsafe void FillData()
		{
			ref AdvancedPerlinNoiseNode.Data ptr = ref *this.pdata;
			ptr.size = base.GetInputValue<int>("size", (int)this.size);
			float inputValue = base.GetInputValue<float>("scale", this.scale);
			int inputValue2 = base.GetInputValue<int>("octaves", (int)this.octaves);
			float inputValue3 = base.GetInputValue<float>("persistance", this.persistance);
			float inputValue4 = base.GetInputValue<float>("lacunarity", this.lacunarity);
			float2 inputValue5 = base.GetInputValue<float2>("offset", this.offset);
			float[,] target = this.CreateNoise(this.seed, ptr.size, inputValue, inputValue2, inputValue3, inputValue4, inputValue5);
			ptr.cache = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target, ref ptr.h_cache);
			ptr.grid.resolution = ptr.size;
			ptr.grid.cell_size = 1;
			ptr.filter_type = this.filterType;
			this.transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			this.isDataFilled = true;
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x001A3878 File Offset: 0x001A1A78
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetFloat2DCB
			{
				get_value_func = AdvancedPerlinNoiseNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x04002343 RID: 9027
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float size = 1025f;

		// Token: 0x04002344 RID: 9028
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float scale = 120f;

		// Token: 0x04002345 RID: 9029
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float octaves = 8f;

		// Token: 0x04002346 RID: 9030
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float persistance = 0.5f;

		// Token: 0x04002347 RID: 9031
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float lacunarity = 2f;

		// Token: 0x04002348 RID: 9032
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float2 offset;

		// Token: 0x04002349 RID: 9033
		[Space]
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x0400234A RID: 9034
		public int seed = 777;

		// Token: 0x0400234B RID: 9035
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x0400234C RID: 9036
		public bool SingleThreaded;

		// Token: 0x0400234D RID: 9037
		private bool isDataFilled;

		// Token: 0x0400234E RID: 9038
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x0400234F RID: 9039
		private float[,] cache;

		// Token: 0x04002350 RID: 9040
		public static IntPtr get_value_func;

		// Token: 0x04002351 RID: 9041
		public NativeArray<AdvancedPerlinNoiseNode.Data> arr_data;

		// Token: 0x04002352 RID: 9042
		public unsafe AdvancedPerlinNoiseNode.Data* pdata;

		// Token: 0x020008BA RID: 2234
		public struct Data
		{
			// Token: 0x060051F5 RID: 20981 RVA: 0x0023EC25 File Offset: 0x0023CE25
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_cache);
			}

			// Token: 0x0400409D RID: 16541
			public int size;

			// Token: 0x0400409E RID: 16542
			public GridData grid;

			// Token: 0x0400409F RID: 16543
			public FilterType filter_type;

			// Token: 0x040040A0 RID: 16544
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* cache;

			// Token: 0x040040A1 RID: 16545
			public ulong h_cache;
		}

		// Token: 0x020008BB RID: 2235
		[BurstCompile(CompileSynchronously = true)]
		private struct NoiseMapJob : IJobParallelFor
		{
			// Token: 0x060051F6 RID: 20982 RVA: 0x0023EC34 File Offset: 0x0023CE34
			public unsafe void Execute(int y)
			{
				for (int i = 0; i < this.size; i++)
				{
					float num = 1f;
					float num2 = 1f;
					float num3 = 0f;
					for (int j = 0; j < this.octaves; j++)
					{
						float x = ((float)i - this.halfWidth) / this.scale * num2 + this.poctaveOffsets[j].x;
						float y2 = ((float)y - this.halfHeight) / this.scale * num2 + this.poctaveOffsets[j].y;
						float num4 = Mathf.PerlinNoise(x, y2) * 2f - 1f;
						num3 += num4 * num;
						num *= this.persistance;
						num2 *= this.lacunarity;
					}
					if (num3 > this.pmaxNoiseHeights[y])
					{
						this.pmaxNoiseHeights[y] = num3;
					}
					else if (num3 < this.pminNoiseHeights[y])
					{
						this.pminNoiseHeights[y] = num3;
					}
					this.pnoiseMap[y * this.size + i] = num3;
				}
			}

			// Token: 0x040040A2 RID: 16546
			public int size;

			// Token: 0x040040A3 RID: 16547
			public float halfWidth;

			// Token: 0x040040A4 RID: 16548
			public float halfHeight;

			// Token: 0x040040A5 RID: 16549
			public float scale;

			// Token: 0x040040A6 RID: 16550
			public int octaves;

			// Token: 0x040040A7 RID: 16551
			public float persistance;

			// Token: 0x040040A8 RID: 16552
			public float lacunarity;

			// Token: 0x040040A9 RID: 16553
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pnoiseMap;

			// Token: 0x040040AA RID: 16554
			[NativeDisableUnsafePtrRestriction]
			public unsafe Vector2* poctaveOffsets;

			// Token: 0x040040AB RID: 16555
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pmaxNoiseHeights;

			// Token: 0x040040AC RID: 16556
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pminNoiseHeights;
		}

		// Token: 0x020008BC RID: 2236
		[BurstCompile(CompileSynchronously = true)]
		private struct InverseLerpJob : IJobParallelFor
		{
			// Token: 0x060051F7 RID: 20983 RVA: 0x0023ED58 File Offset: 0x0023CF58
			public unsafe void Execute(int y)
			{
				for (int i = 0; i < this.size; i++)
				{
					this.pnoiseMap[y * this.size + i] = Mathf.InverseLerp(this.minNoiseHeight, this.maxNoiseHeight, this.pnoiseMap[y * this.size + i]);
				}
			}

			// Token: 0x040040AD RID: 16557
			public int size;

			// Token: 0x040040AE RID: 16558
			public float minNoiseHeight;

			// Token: 0x040040AF RID: 16559
			public float maxNoiseHeight;

			// Token: 0x040040B0 RID: 16560
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pnoiseMap;
		}
	}
}
