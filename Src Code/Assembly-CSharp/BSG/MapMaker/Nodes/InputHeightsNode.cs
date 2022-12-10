using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038D RID: 909
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Heights/InputHeights")]
	[Serializable]
	public class InputHeightsNode : Node
	{
		// Token: 0x060034B5 RID: 13493 RVA: 0x001A59A4 File Offset: 0x001A3BA4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputHeightsNode.Data ptr = ref *(InputHeightsNode.Data*)data;
			return ptr.grid.GetValue(ptr.heights, 1, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x001A59DC File Offset: 0x001A3BDC
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputHeightsNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputHeightsNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputHeightsNode.Data>();
			if (InputHeightsNode.get_value_func == IntPtr.Zero)
			{
				InputHeightsNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputHeightsNode.GetValue)).Value;
			}
		}

		// Token: 0x060034B7 RID: 13495 RVA: 0x001A5A37 File Offset: 0x001A3C37
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060034B8 RID: 13496 RVA: 0x001A5A60 File Offset: 0x001A3C60
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetFloat2DCB
			{
				get_value_func = InputHeightsNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034B9 RID: 13497 RVA: 0x001A5AAC File Offset: 0x001A3CAC
		private unsafe void FillData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputHeightsNode.Data ptr = ref *this.pdata;
			ptr.grid.resolution = this.td.heightmapResolution - 1;
			ptr.grid.cell_size = new float2(this.td.size.x, this.td.size.z) / ptr.grid.resolution;
			int2 @int = subRect.ApplyTo(ref ptr.grid);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			ptr.filter_type = this.filterType;
			float[,] target = this.td.GetHeights(@int.x, @int.y, ptr.grid.resolution.x, ptr.grid.resolution.y);
			ptr.heights = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target, ref ptr.h_heights);
			this.isDataFilled = true;
		}

		// Token: 0x040023A3 RID: 9123
		public TerrainData td;

		// Token: 0x040023A4 RID: 9124
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public SubRect subRect;

		// Token: 0x040023A5 RID: 9125
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x040023A6 RID: 9126
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x040023A7 RID: 9127
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB heights;

		// Token: 0x040023A8 RID: 9128
		private bool isDataFilled;

		// Token: 0x040023A9 RID: 9129
		public static IntPtr get_value_func;

		// Token: 0x040023AA RID: 9130
		public NativeArray<InputHeightsNode.Data> arr_data;

		// Token: 0x040023AB RID: 9131
		public unsafe InputHeightsNode.Data* pdata;

		// Token: 0x020008C5 RID: 2245
		public struct Data
		{
			// Token: 0x060051FF RID: 20991 RVA: 0x0023EF2A File Offset: 0x0023D12A
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_heights);
			}

			// Token: 0x040040DC RID: 16604
			public GridData grid;

			// Token: 0x040040DD RID: 16605
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* heights;

			// Token: 0x040040DE RID: 16606
			public ulong h_heights;

			// Token: 0x040040DF RID: 16607
			public FilterType filter_type;
		}
	}
}
