using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000392 RID: 914
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/InputTexture")]
	[Serializable]
	public class InputTextureNode : Node
	{
		// Token: 0x060034E6 RID: 13542 RVA: 0x001A8664 File Offset: 0x001A6864
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetR(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTextureNode.Data ptr = ref *(InputTextureNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x001A869C File Offset: 0x001A689C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetG(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTextureNode.Data ptr = ref *(InputTextureNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 1, ptr.filter_type);
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x001A86D4 File Offset: 0x001A68D4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetB(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTextureNode.Data ptr = ref *(InputTextureNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 2, ptr.filter_type);
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x001A870C File Offset: 0x001A690C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetA(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTextureNode.Data ptr = ref *(InputTextureNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 3, ptr.filter_type);
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x001A8744 File Offset: 0x001A6944
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputTextureNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputTextureNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputTextureNode.Data>();
			if (InputTextureNode.get_r_func == IntPtr.Zero)
			{
				InputTextureNode.get_r_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTextureNode.GetR)).Value;
			}
			if (InputTextureNode.get_g_func == IntPtr.Zero)
			{
				InputTextureNode.get_g_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTextureNode.GetG)).Value;
			}
			if (InputTextureNode.get_b_func == IntPtr.Zero)
			{
				InputTextureNode.get_b_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTextureNode.GetB)).Value;
			}
			if (InputTextureNode.get_a_func == IntPtr.Zero)
			{
				InputTextureNode.get_a_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTextureNode.GetA)).Value;
			}
		}

		// Token: 0x060034EB RID: 13547 RVA: 0x001A882C File Offset: 0x001A6A2C
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060034EC RID: 13548 RVA: 0x001A8854 File Offset: 0x001A6A54
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			string fieldName = port.fieldName;
			if (fieldName == "r")
			{
				return new GetFloat2DCB
				{
					get_value_func = InputTextureNode.get_r_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "g")
			{
				return new GetFloat2DCB
				{
					get_value_func = InputTextureNode.get_g_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "b")
			{
				return new GetFloat2DCB
				{
					get_value_func = InputTextureNode.get_b_func,
					data = (void*)this.pdata
				};
			}
			if (!(fieldName == "a"))
			{
				return null;
			}
			return new GetFloat2DCB
			{
				get_value_func = InputTextureNode.get_a_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034ED RID: 13549 RVA: 0x001A895C File Offset: 0x001A6B5C
		private unsafe void FillData()
		{
			ref InputTextureNode.Data ptr = ref *this.pdata;
			ptr.grid.resolution = this.texture.width;
			ptr.grid.cell_size = 1f;
			int2 offset = this.subRect.ApplyTo(ref ptr.grid);
			this.transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			float[,,] array = new float[ptr.grid.resolution.y, ptr.grid.resolution.x, 4];
			Stopwatch stopwatch = Stopwatch.StartNew();
			Color[] pixels = this.texture.GetPixels();
			fixed (float* ptr2 = &array[0, 0, 0])
			{
				float* prgba = ptr2;
				fixed (Color* ptr3 = ref pixels[0])
				{
					Color* ppixels = ptr3;
					InputTextureNode.BuildTextureDataJob jobData = default(InputTextureNode.BuildTextureDataJob);
					jobData.offset = offset;
					jobData.gridResolution = ptr.grid.resolution;
					jobData.textureResolution = this.texture.width;
					jobData.prgba = prgba;
					jobData.ppixels = ppixels;
					if (this.SingleThreaded)
					{
						jobData.Run(ptr.grid.resolution.y);
					}
					else
					{
						jobData.Schedule(ptr.grid.resolution.y, 16, default(JobHandle)).Complete();
					}
				}
			}
			Debug.Log("Creating texture data time " + stopwatch.ElapsedMilliseconds);
			ptr.rgba = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref ptr.h_rgba);
			ptr.filter_type = this.filterType;
			this.isDataFilled = true;
		}

		// Token: 0x040023FF RID: 9215
		public Texture2D texture;

		// Token: 0x04002400 RID: 9216
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x04002401 RID: 9217
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public SubRect subRect;

		// Token: 0x04002402 RID: 9218
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB r;

		// Token: 0x04002403 RID: 9219
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB g;

		// Token: 0x04002404 RID: 9220
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB b;

		// Token: 0x04002405 RID: 9221
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB a;

		// Token: 0x04002406 RID: 9222
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x04002407 RID: 9223
		public bool SingleThreaded = true;

		// Token: 0x04002408 RID: 9224
		private bool isDataFilled;

		// Token: 0x04002409 RID: 9225
		public static IntPtr get_r_func;

		// Token: 0x0400240A RID: 9226
		public static IntPtr get_g_func;

		// Token: 0x0400240B RID: 9227
		public static IntPtr get_b_func;

		// Token: 0x0400240C RID: 9228
		public static IntPtr get_a_func;

		// Token: 0x0400240D RID: 9229
		public NativeArray<InputTextureNode.Data> arr_data;

		// Token: 0x0400240E RID: 9230
		public unsafe InputTextureNode.Data* pdata;

		// Token: 0x020008D2 RID: 2258
		public struct Data
		{
			// Token: 0x06005210 RID: 21008 RVA: 0x0023F74B File Offset: 0x0023D94B
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_rgba);
			}

			// Token: 0x0400411D RID: 16669
			public GridData grid;

			// Token: 0x0400411E RID: 16670
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* rgba;

			// Token: 0x0400411F RID: 16671
			public ulong h_rgba;

			// Token: 0x04004120 RID: 16672
			public FilterType filter_type;
		}

		// Token: 0x020008D3 RID: 2259
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildTextureDataJob : IJobParallelFor
		{
			// Token: 0x06005211 RID: 21009 RVA: 0x0023F758 File Offset: 0x0023D958
			public unsafe void Execute(int y)
			{
				for (int i = 0; i < this.gridResolution.x; i++)
				{
					Color color = this.ppixels[(this.offset.y + y) * this.textureResolution + (this.offset.x + i)];
					this.prgba[y * this.gridResolution.y * 4 + 4 * i] = color.r;
					this.prgba[y * this.gridResolution.y * 4 + 4 * i + 1] = color.g;
					this.prgba[y * this.gridResolution.y * 4 + 4 * i + 2] = color.b;
					this.prgba[y * this.gridResolution.y * 4 + 4 * i + 3] = color.a;
				}
			}

			// Token: 0x04004121 RID: 16673
			public int textureResolution;

			// Token: 0x04004122 RID: 16674
			public int2 gridResolution;

			// Token: 0x04004123 RID: 16675
			public int2 offset;

			// Token: 0x04004124 RID: 16676
			[NativeDisableUnsafePtrRestriction]
			public unsafe Color* ppixels;

			// Token: 0x04004125 RID: 16677
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* prgba;
		}
	}
}
