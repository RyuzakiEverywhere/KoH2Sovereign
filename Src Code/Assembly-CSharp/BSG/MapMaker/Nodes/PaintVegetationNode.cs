using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A3 RID: 931
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Trees/PaintVegetation")]
	[Serializable]
	public class PaintVegetationNode : Node
	{
		// Token: 0x0600354C RID: 13644 RVA: 0x001ABB44 File Offset: 0x001A9D44
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTrees(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref PaintVegetationNode.Data ptr = ref *(PaintVegetationNode.Data*)data;
			if (ptr.trees.data == null)
			{
				return;
			}
			PaintVegetationNode.AddData addData = default(PaintVegetationNode.AddData);
			addData.data = (PaintVegetationNode.Data*)data;
			addData.cb_func = cb.Value;
			addData.cb_data = cb_data;
			FunctionPointer<GetTreesCB.AddTreeFunc> cb2 = new FunctionPointer<GetTreesCB.AddTreeFunc>(ptr.add_tree_func);
			ptr.trees.GetTrees(wx, wy, ww, wh, cb2, (void*)(&addData));
		}

		// Token: 0x0600354D RID: 13645 RVA: 0x001ABBB4 File Offset: 0x001A9DB4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddTree(ref TreeInstance tree, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref PaintVegetationNode.AddData ptr = ref *(PaintVegetationNode.AddData*)data;
			ref PaintVegetationNode.Data data2 = ref *ptr.data;
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

		// Token: 0x0600354E RID: 13646 RVA: 0x001ABC30 File Offset: 0x001A9E30
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<PaintVegetationNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (PaintVegetationNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<PaintVegetationNode.Data>();
			if (PaintVegetationNode.get_trees_func == IntPtr.Zero)
			{
				PaintVegetationNode.get_trees_func = GetTreesCB.GetFuncPtr(new GetTreesCB.GetTreesFunc(PaintVegetationNode.GetTrees)).Value;
			}
			if (PaintVegetationNode.add_tree_func == IntPtr.Zero)
			{
				PaintVegetationNode.add_tree_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(PaintVegetationNode.AddTree)).Value;
			}
		}

		// Token: 0x0600354F RID: 13647 RVA: 0x001ABCBA File Offset: 0x001A9EBA
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x06003550 RID: 13648 RVA: 0x001ABCD8 File Offset: 0x001A9ED8
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetTreesCB
			{
				get_trees_func = PaintVegetationNode.get_trees_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x06003551 RID: 13649 RVA: 0x001ABD24 File Offset: 0x001A9F24
		private unsafe void FillData()
		{
			PaintVegetationNode.Data* ptr = this.pdata;
			ptr->trees = base.GetInputValue<GetTreesCB>("trees", default(GetTreesCB));
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.cutout;
			ptr->add_tree_func = PaintVegetationNode.add_tree_func;
			this.isDataFilled = true;
		}

		// Token: 0x04002484 RID: 9348
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB trees;

		// Token: 0x04002485 RID: 9349
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x04002486 RID: 9350
		public float2 cutout = new float2(0f, 1f);

		// Token: 0x04002487 RID: 9351
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB res;

		// Token: 0x04002488 RID: 9352
		private bool isDataFilled;

		// Token: 0x04002489 RID: 9353
		public static IntPtr get_trees_func;

		// Token: 0x0400248A RID: 9354
		public static IntPtr add_tree_func;

		// Token: 0x0400248B RID: 9355
		public NativeArray<PaintVegetationNode.Data> arr_data;

		// Token: 0x0400248C RID: 9356
		public unsafe PaintVegetationNode.Data* pdata;

		// Token: 0x020008ED RID: 2285
		public struct Data
		{
			// Token: 0x04004193 RID: 16787
			public GetTreesCB trees;

			// Token: 0x04004194 RID: 16788
			public GetFloat2DCB mask;

			// Token: 0x04004195 RID: 16789
			public float2 cutout;

			// Token: 0x04004196 RID: 16790
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_tree_func;
		}

		// Token: 0x020008EE RID: 2286
		public struct AddData
		{
			// Token: 0x06005227 RID: 21031 RVA: 0x0024028C File Offset: 0x0023E48C
			public void Add(ref TreeInstance tree)
			{
				if (this.cb_func == IntPtr.Zero)
				{
					return;
				}
				FunctionPointer<GetTreesCB.AddTreeFunc> functionPointer = new FunctionPointer<GetTreesCB.AddTreeFunc>(this.cb_func);
				functionPointer.Invoke(ref tree, this.cb_data);
			}

			// Token: 0x04004197 RID: 16791
			[NativeDisableUnsafePtrRestriction]
			public unsafe PaintVegetationNode.Data* data;

			// Token: 0x04004198 RID: 16792
			[NativeDisableUnsafePtrRestriction]
			public IntPtr cb_func;

			// Token: 0x04004199 RID: 16793
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* cb_data;
		}
	}
}
