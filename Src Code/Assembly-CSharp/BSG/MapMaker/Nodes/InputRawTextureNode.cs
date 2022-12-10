using System;
using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038F RID: 911
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/InputRawTexture")]
	[Serializable]
	public class InputRawTextureNode : Node
	{
		// Token: 0x060034C4 RID: 13508 RVA: 0x001A6398 File Offset: 0x001A4598
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputRawTextureNode.Data ptr = ref *(InputRawTextureNode.Data*)data;
			return ptr.grid.GetValue(ptr.raw, 1, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x060034C5 RID: 13509 RVA: 0x001A63D0 File Offset: 0x001A45D0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputRawTextureNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputRawTextureNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputRawTextureNode.Data>();
			if (InputRawTextureNode.get_value_func == IntPtr.Zero)
			{
				InputRawTextureNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputRawTextureNode.GetValue)).Value;
			}
		}

		// Token: 0x060034C6 RID: 13510 RVA: 0x001A642B File Offset: 0x001A462B
		public unsafe override void CleanUp()
		{
			if (this.isDataFilled)
			{
				this.pdata->Dispose();
				this.pdata = null;
				this.arr_data.Dispose();
				this.isDataFilled = false;
			}
		}

		// Token: 0x060034C7 RID: 13511 RVA: 0x001A645C File Offset: 0x001A465C
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			if (!this.isDataFilled)
			{
				return null;
			}
			return new GetFloat2DCB
			{
				get_value_func = InputRawTextureNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034C8 RID: 13512 RVA: 0x001A64B4 File Offset: 0x001A46B4
		private unsafe void FillData()
		{
			if (string.IsNullOrEmpty(this.rawFilePath) || !File.Exists(this.rawFilePath))
			{
				Debug.LogWarning("Raw file not found.");
				return;
			}
			ref InputRawTextureNode.Data ptr = ref *this.pdata;
			byte[] array = File.ReadAllBytes(this.rawFilePath);
			int num = (int)Mathf.Sqrt((float)(array.Length / 2));
			ptr.grid.resolution = num;
			ptr.grid.cell_size = 1f;
			int2 offset = this.subRect.ApplyTo(ref ptr.grid);
			this.transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			float[,] array2 = new float[num, num];
			fixed (float* ptr2 = &array2[0, 0])
			{
				float* parr = ptr2;
				fixed (byte* ptr3 = &array[0])
				{
					byte* pbytes = ptr3;
					InputRawTextureNode.BuildTextureDataJob jobData = default(InputRawTextureNode.BuildTextureDataJob);
					jobData.fileSize = num;
					jobData.gridResolution = ptr.grid.resolution;
					jobData.parr = parr;
					jobData.pbytes = pbytes;
					jobData.offset = offset;
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
			ptr.raw = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array2, ref ptr.h_raw);
			ptr.filter_type = this.filterType;
			this.isDataFilled = true;
		}

		// Token: 0x040023BA RID: 9146
		public string rawFilePath;

		// Token: 0x040023BB RID: 9147
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x040023BC RID: 9148
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public SubRect subRect;

		// Token: 0x040023BD RID: 9149
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB raw;

		// Token: 0x040023BE RID: 9150
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x040023BF RID: 9151
		public bool SingleThreaded;

		// Token: 0x040023C0 RID: 9152
		private bool isDataFilled;

		// Token: 0x040023C1 RID: 9153
		public static IntPtr get_value_func;

		// Token: 0x040023C2 RID: 9154
		public NativeArray<InputRawTextureNode.Data> arr_data;

		// Token: 0x040023C3 RID: 9155
		public unsafe InputRawTextureNode.Data* pdata;

		// Token: 0x020008C8 RID: 2248
		public struct Data
		{
			// Token: 0x06005203 RID: 20995 RVA: 0x0023F103 File Offset: 0x0023D303
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_raw);
			}

			// Token: 0x040040E8 RID: 16616
			public GridData grid;

			// Token: 0x040040E9 RID: 16617
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* raw;

			// Token: 0x040040EA RID: 16618
			public ulong h_raw;

			// Token: 0x040040EB RID: 16619
			public FilterType filter_type;
		}

		// Token: 0x020008C9 RID: 2249
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildTextureDataJob : IJobParallelFor
		{
			// Token: 0x06005204 RID: 20996 RVA: 0x0023F110 File Offset: 0x0023D310
			public unsafe void Execute(int y)
			{
				int num = (y + this.offset.y) * this.fileSize * 2 + this.offset.x * 2;
				for (int i = 0; i < this.gridResolution.x; i++)
				{
					uint num2 = (uint)this.pbytes[num++];
					uint num3 = (uint)this.pbytes[num++];
					float num4 = (num2 | num3 << 8) / 65535f;
					this.parr[y * this.gridResolution.y + i] = num4;
				}
			}

			// Token: 0x040040EC RID: 16620
			public int fileSize;

			// Token: 0x040040ED RID: 16621
			public int2 gridResolution;

			// Token: 0x040040EE RID: 16622
			public int2 offset;

			// Token: 0x040040EF RID: 16623
			[NativeDisableUnsafePtrRestriction]
			public unsafe byte* pbytes;

			// Token: 0x040040F0 RID: 16624
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* parr;
		}
	}
}
