using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B8 RID: 952
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Trees/VegetationMixer")]
	[Serializable]
	public class VegetationMixerNode : Node
	{
		// Token: 0x060035B3 RID: 13747 RVA: 0x001AEEC8 File Offset: 0x001AD0C8
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTrees(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref VegetationMixerNode.Data ptr = ref *(VegetationMixerNode.Data*)data;
			VegetationMixerNode.AddData addData = default(VegetationMixerNode.AddData);
			addData.data = (VegetationMixerNode.Data*)data;
			addData.cb_func = cb.Value;
			addData.cb_data = cb_data;
			FunctionPointer<GetTreesCB.AddTreeFunc> cb2 = new FunctionPointer<GetTreesCB.AddTreeFunc>(ptr.add_tree_func);
			FunctionPointer<GetTreesCB.AddTreeFunc> cb3 = new FunctionPointer<GetTreesCB.AddTreeFunc>(ptr.add_tree_with_mask_func);
			ptr.in1.GetTrees(wx, wy, ww, wh, cb2, (void*)(&addData));
			ptr.in2.GetTrees(wx, wy, ww, wh, cb3, (void*)(&addData));
		}

		// Token: 0x060035B4 RID: 13748 RVA: 0x001AEF47 File Offset: 0x001AD147
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddTree(ref TreeInstance tree, void* data)
		{
			if (data == null)
			{
				return;
			}
			VegetationMixerNode.Data data2 = *((VegetationMixerNode.AddData*)data)->data;
			((VegetationMixerNode.AddData*)data)->Add(ref tree);
		}

		// Token: 0x060035B5 RID: 13749 RVA: 0x001AEF64 File Offset: 0x001AD164
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddTreeWithMask(ref TreeInstance tree, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref VegetationMixerNode.AddData ptr = ref *(VegetationMixerNode.AddData*)data;
			ref VegetationMixerNode.Data data2 = ref *ptr.data;
			float num;
			if (data2.mask.data != null)
			{
				num = data2.mask.GetValue(tree.position.x, tree.position.z);
			}
			else
			{
				num = 1f;
			}
			if (num < data2.cutout.x || num > data2.cutout.y)
			{
				return;
			}
			ptr.Add(ref tree);
		}

		// Token: 0x060035B6 RID: 13750 RVA: 0x001AEFE0 File Offset: 0x001AD1E0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<VegetationMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (VegetationMixerNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<VegetationMixerNode.Data>();
			if (VegetationMixerNode.get_trees_func == IntPtr.Zero)
			{
				VegetationMixerNode.get_trees_func = GetTreesCB.GetFuncPtr(new GetTreesCB.GetTreesFunc(VegetationMixerNode.GetTrees)).Value;
			}
			if (VegetationMixerNode.add_tree_func == IntPtr.Zero)
			{
				VegetationMixerNode.add_tree_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(VegetationMixerNode.AddTree)).Value;
			}
			if (VegetationMixerNode.add_tree_with_mask == IntPtr.Zero)
			{
				VegetationMixerNode.add_tree_with_mask = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(VegetationMixerNode.AddTreeWithMask)).Value;
			}
		}

		// Token: 0x060035B7 RID: 13751 RVA: 0x001AF099 File Offset: 0x001AD299
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060035B8 RID: 13752 RVA: 0x001AF0B8 File Offset: 0x001AD2B8
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetTreesCB
			{
				get_trees_func = VegetationMixerNode.get_trees_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060035B9 RID: 13753 RVA: 0x001AF104 File Offset: 0x001AD304
		private unsafe void FillData()
		{
			VegetationMixerNode.Data* ptr = this.pdata;
			ptr->in1 = base.GetInputValue<GetTreesCB>("in1", default(GetTreesCB));
			ptr->in2 = base.GetInputValue<GetTreesCB>("in2", default(GetTreesCB));
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.cutout;
			ptr->add_tree_func = VegetationMixerNode.add_tree_func;
			ptr->add_tree_with_mask_func = VegetationMixerNode.add_tree_with_mask;
			this.isDataFilled = true;
		}

		// Token: 0x0400253E RID: 9534
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB in1;

		// Token: 0x0400253F RID: 9535
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB in2;

		// Token: 0x04002540 RID: 9536
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x04002541 RID: 9537
		public float2 cutout = new float2(0f, 1f);

		// Token: 0x04002542 RID: 9538
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB trees;

		// Token: 0x04002543 RID: 9539
		private bool isDataFilled;

		// Token: 0x04002544 RID: 9540
		public static IntPtr get_trees_func;

		// Token: 0x04002545 RID: 9541
		public static IntPtr add_tree_func;

		// Token: 0x04002546 RID: 9542
		public static IntPtr add_tree_with_mask;

		// Token: 0x04002547 RID: 9543
		public NativeArray<VegetationMixerNode.Data> arr_data;

		// Token: 0x04002548 RID: 9544
		public unsafe VegetationMixerNode.Data* pdata;

		// Token: 0x020008FD RID: 2301
		public struct Data
		{
			// Token: 0x040041E9 RID: 16873
			public GetTreesCB in1;

			// Token: 0x040041EA RID: 16874
			public GetTreesCB in2;

			// Token: 0x040041EB RID: 16875
			public GetFloat2DCB mask;

			// Token: 0x040041EC RID: 16876
			public float2 cutout;

			// Token: 0x040041ED RID: 16877
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_tree_func;

			// Token: 0x040041EE RID: 16878
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_tree_with_mask_func;
		}

		// Token: 0x020008FE RID: 2302
		public struct AddData
		{
			// Token: 0x06005230 RID: 21040 RVA: 0x00240948 File Offset: 0x0023EB48
			public void Add(ref TreeInstance tree)
			{
				if (this.cb_func == IntPtr.Zero)
				{
					return;
				}
				FunctionPointer<GetTreesCB.AddTreeFunc> functionPointer = new FunctionPointer<GetTreesCB.AddTreeFunc>(this.cb_func);
				functionPointer.Invoke(ref tree, this.cb_data);
			}

			// Token: 0x040041EF RID: 16879
			[NativeDisableUnsafePtrRestriction]
			public unsafe VegetationMixerNode.Data* data;

			// Token: 0x040041F0 RID: 16880
			[NativeDisableUnsafePtrRestriction]
			public IntPtr cb_func;

			// Token: 0x040041F1 RID: 16881
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* cb_data;
		}
	}
}
