using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000390 RID: 912
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Splats/InputSplats")]
	[Serializable]
	public class InputSplatsNode : Node
	{
		// Token: 0x060034CA RID: 13514 RVA: 0x001A6644 File Offset: 0x001A4844
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref InputSplatsNode.Data ptr = ref *(InputSplatsNode.Data*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int2 @int = new int2(@float);
			float2 float2 = @float - @int;
			FilterType filterType = (ptr.filter_type != FilterType.None && math.any(float2)) ? ptr.filter_type : FilterType.None;
			for (int i = 0; i < layers; i++)
			{
				if (i >= ptr.remap_count)
				{
					result[i] = 0f;
				}
				else
				{
					int num = ptr.remap[i];
					if (num < 0)
					{
						result[i] = 0f;
					}
					else
					{
						result[i] = ptr.grid.GetLocal(ptr.alphas, ptr.layers, @int, float2, num, filterType);
					}
				}
			}
		}

		// Token: 0x060034CB RID: 13515 RVA: 0x001A6710 File Offset: 0x001A4910
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputSplatsNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputSplatsNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputSplatsNode.Data>();
			if (InputSplatsNode.get_tile_func == IntPtr.Zero)
			{
				InputSplatsNode.get_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(InputSplatsNode.GetTile)).Value;
			}
		}

		// Token: 0x060034CC RID: 13516 RVA: 0x001A676B File Offset: 0x001A496B
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060034CD RID: 13517 RVA: 0x001A6794 File Offset: 0x001A4994
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetSplatCB
			{
				get_tile_func = InputSplatsNode.get_tile_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034CE RID: 13518 RVA: 0x001A67E0 File Offset: 0x001A49E0
		private unsafe void FillData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputSplatsNode.Data ptr = ref *this.pdata;
			ptr.grid.resolution = this.td.alphamapResolution;
			ptr.grid.cell_size = new float2(this.td.size.x, this.td.size.z) / ptr.grid.resolution;
			int2 @int = subRect.ApplyTo(ref ptr.grid);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			ptr.layers = this.td.alphamapLayers;
			ptr.filter_type = this.filterType;
			float[,,] alphamaps = this.td.GetAlphamaps(@int.x, @int.y, ptr.grid.resolution.x, ptr.grid.resolution.y);
			ptr.remap = (int*)AllocationManager.PinGCArrayAndGetDataAddress(this.splats_remap.ToArray(), ref ptr.h_remap);
			ptr.remap_count = this.splats_remap.Count;
			ptr.alphas = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref ptr.h_alphas);
			this.isDataFilled = true;
		}

		// Token: 0x040023C4 RID: 9156
		public TerrainData td;

		// Token: 0x040023C5 RID: 9157
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public SubRect subRect;

		// Token: 0x040023C6 RID: 9158
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x040023C7 RID: 9159
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x040023C8 RID: 9160
		[HideInInspector]
		public List<int> splats_remap = new List<int>();

		// Token: 0x040023C9 RID: 9161
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB splats;

		// Token: 0x040023CA RID: 9162
		private bool isDataFilled;

		// Token: 0x040023CB RID: 9163
		public static IntPtr get_tile_func;

		// Token: 0x040023CC RID: 9164
		public NativeArray<InputSplatsNode.Data> arr_data;

		// Token: 0x040023CD RID: 9165
		public unsafe InputSplatsNode.Data* pdata;

		// Token: 0x020008CA RID: 2250
		public struct Data
		{
			// Token: 0x06005205 RID: 20997 RVA: 0x0023F19B File Offset: 0x0023D39B
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_remap);
				AllocationManager.ReleaseGCObject(ref this.h_alphas);
			}

			// Token: 0x040040F1 RID: 16625
			public GridData grid;

			// Token: 0x040040F2 RID: 16626
			public int layers;

			// Token: 0x040040F3 RID: 16627
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* remap;

			// Token: 0x040040F4 RID: 16628
			public int remap_count;

			// Token: 0x040040F5 RID: 16629
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* alphas;

			// Token: 0x040040F6 RID: 16630
			public ulong h_remap;

			// Token: 0x040040F7 RID: 16631
			public ulong h_alphas;

			// Token: 0x040040F8 RID: 16632
			public FilterType filter_type;
		}
	}
}
