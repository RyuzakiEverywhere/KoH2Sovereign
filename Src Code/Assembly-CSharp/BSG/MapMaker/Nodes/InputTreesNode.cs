using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000393 RID: 915
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Trees/InputTrees")]
	[Serializable]
	public class InputTreesNode : Node
	{
		// Token: 0x060034EF RID: 13551 RVA: 0x001A8B14 File Offset: 0x001A6D14
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTrees(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref InputTreesNode.Data ptr = ref *(InputTreesNode.Data*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int cx = (int)@float.x;
			int cy = (int)@float.y;
			float num = (float)((int)(wx * ptr.grid.inv_cell_size.x)) * ptr.grid.cell_size.x;
			float num2 = (float)((int)(wy * ptr.grid.inv_cell_size.y)) * ptr.grid.cell_size.y;
			GrowBuffer<TreeInstance> cell = ptr.GetCell(cx, cy);
			int count = cell.Count;
			for (int i = 0; i < count; i++)
			{
				TreeInstance treeInstance = *cell[i];
				treeInstance.position.x = treeInstance.position.x + num;
				treeInstance.position.z = treeInstance.position.z + num2;
				treeInstance.position.x = treeInstance.position.x * ptr.grid.scale.x;
				treeInstance.position.z = treeInstance.position.z * ptr.grid.scale.y;
				cb.Invoke(ref treeInstance, cb_data);
			}
		}

		// Token: 0x060034F0 RID: 13552 RVA: 0x001A8C48 File Offset: 0x001A6E48
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputTreesNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputTreesNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputTreesNode.Data>();
			if (InputTreesNode.get_trees_func == IntPtr.Zero)
			{
				InputTreesNode.get_trees_func = GetTreesCB.GetFuncPtr(new GetTreesCB.GetTreesFunc(InputTreesNode.GetTrees)).Value;
			}
		}

		// Token: 0x060034F1 RID: 13553 RVA: 0x001A8CA4 File Offset: 0x001A6EA4
		public unsafe override void CleanUp()
		{
			if (this.pdata == null)
			{
				return;
			}
			ref InputTreesNode.Data ptr = ref *this.pdata;
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					ptr.GetCell(j, i).Dispose();
				}
			}
			this.cells.Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060034F2 RID: 13554 RVA: 0x001A8D30 File Offset: 0x001A6F30
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetTreesCB
			{
				get_trees_func = InputTreesNode.get_trees_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x060034F3 RID: 13555 RVA: 0x001A8D7C File Offset: 0x001A6F7C
		private unsafe void FillData()
		{
			ref InputTreesNode.Data ptr = ref *this.pdata;
			ptr.grid.cell_size = new float2(32f / this.transformation.Scale.x, 32f / this.transformation.Scale.y);
			ptr.grid.resolution = (int)(this.td.size.x / ptr.grid.cell_size.x);
			this.transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			TreeInstance[] treeInstances = this.td.treeInstances;
			this.cells = new NativeArray<IntPtr>(ptr.grid.resolution.y * ptr.grid.resolution.x, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			ptr.cells = (IntPtr*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<IntPtr>(this.cells);
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					GrowBuffer<TreeInstance> gb = new GrowBuffer<TreeInstance>(Allocator.TempJob, 0);
					ptr.SetCell(j, i, gb);
				}
			}
			TreeInstance[] array;
			TreeInstance* arr_trees;
			if ((array = treeInstances) == null || array.Length == 0)
			{
				arr_trees = null;
			}
			else
			{
				arr_trees = &array[0];
			}
			int[] array2;
			int* ptr2;
			if ((array2 = this.trees_remap.ToArray()) == null || array2.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array2[0];
			}
			InputTreesNode.FillJob fillJob = default(InputTreesNode.FillJob);
			fillJob.arr_trees = arr_trees;
			fillJob.trees_remap = ptr2;
			fillJob.pdata = this.pdata;
			fillJob.size = this.td.size;
			fillJob.verify = this.Verify;
			if (this.SingleThreaded)
			{
				fillJob.Run(treeInstances.Length);
			}
			else
			{
				fillJob.Schedule(treeInstances.Length, 1, default(JobHandle)).Complete();
			}
			if (fillJob.ignored != 0)
			{
				Debug.LogWarning(string.Format("Ignored {0} trees", fillJob.ignored));
			}
			array2 = null;
			array = null;
			this.isDataFilled = true;
			if (this.Verify && !this.Validate(treeInstances))
			{
				this.CleanUp();
			}
		}

		// Token: 0x060034F4 RID: 13556 RVA: 0x001A8FB4 File Offset: 0x001A71B4
		public unsafe bool Validate(TreeInstance[] arr_trees)
		{
			ref InputTreesNode.Data ptr = ref *this.pdata;
			int[] array = new int[arr_trees.Length];
			int num = 0;
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					GrowBuffer<TreeInstance> cell = ptr.GetCell(j, i);
					int count = cell.Count;
					num += count;
					for (int k = 0; k < count; k++)
					{
						ref TreeInstance ptr2 = ref cell[k];
						int num2 = ptr2.prototypeIndex >> 4;
						ptr2.prototypeIndex &= 15;
						array[num2]++;
					}
				}
			}
			if (num != array.Length)
			{
				Debug.LogError(string.Format("Wrong number of imported trees: {0} instead of {1}", num, array.Length));
				return false;
			}
			for (int l = 0; l < num; l++)
			{
				int num3 = array[l];
				if (num3 != 1)
				{
					Debug.LogError(string.Format("Tree {0} imported {1} times", l, num3));
					return false;
				}
			}
			Debug.Log(string.Format("Successfuly imported {0} trees", num));
			return true;
		}

		// Token: 0x0400240F RID: 9231
		public TerrainData td;

		// Token: 0x04002410 RID: 9232
		public Transformation transformation;

		// Token: 0x04002411 RID: 9233
		public bool SingleThreaded;

		// Token: 0x04002412 RID: 9234
		public bool Verify;

		// Token: 0x04002413 RID: 9235
		[HideInInspector]
		public List<int> trees_remap = new List<int>();

		// Token: 0x04002414 RID: 9236
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB trees;

		// Token: 0x04002415 RID: 9237
		private bool isDataFilled;

		// Token: 0x04002416 RID: 9238
		public NativeArray<IntPtr> cells;

		// Token: 0x04002417 RID: 9239
		public static IntPtr get_trees_func;

		// Token: 0x04002418 RID: 9240
		public NativeArray<InputTreesNode.Data> arr_data;

		// Token: 0x04002419 RID: 9241
		public unsafe InputTreesNode.Data* pdata;

		// Token: 0x020008D4 RID: 2260
		public struct Data
		{
			// Token: 0x06005212 RID: 21010 RVA: 0x0023F84E File Offset: 0x0023DA4E
			public unsafe GrowBuffer<TreeInstance> GetCell(int cx, int cy)
			{
				return new GrowBuffer<TreeInstance>(this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}

			// Token: 0x06005213 RID: 21011 RVA: 0x0023F879 File Offset: 0x0023DA79
			public unsafe void SetCell(int cx, int cy, GrowBuffer<TreeInstance> gb)
			{
				this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = gb.ToIntPtr();
			}

			// Token: 0x04004126 RID: 16678
			public GridData grid;

			// Token: 0x04004127 RID: 16679
			[NativeDisableUnsafePtrRestriction]
			public unsafe IntPtr* cells;
		}

		// Token: 0x020008D5 RID: 2261
		[BurstCompile]
		private struct FillJob : IJobParallelFor
		{
			// Token: 0x06005214 RID: 21012 RVA: 0x0023F8A8 File Offset: 0x0023DAA8
			public unsafe void Execute(int index)
			{
				ref InputTreesNode.Data ptr = ref *this.pdata;
				TreeInstance treeInstance = this.arr_trees[index];
				treeInstance.prototypeIndex = this.trees_remap[treeInstance.prototypeIndex];
				if (this.verify)
				{
					treeInstance.prototypeIndex |= index << 4;
				}
				treeInstance.position.Scale(this.size);
				int num = (int)(treeInstance.position.x * ptr.grid.inv_cell_size.x);
				int num2 = (int)(treeInstance.position.z * ptr.grid.inv_cell_size.y);
				if (num < 0 || num >= ptr.grid.resolution.x || num2 < 0 || num2 >= ptr.grid.resolution.y)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				treeInstance.position.x = treeInstance.position.x - (float)num * ptr.grid.cell_size.x;
				treeInstance.position.z = treeInstance.position.z - (float)num2 * ptr.grid.cell_size.y;
				GrowBuffer<TreeInstance> cell = ptr.GetCell(num, num2);
				if (!cell.IsCreated)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				cell.Add(treeInstance);
			}

			// Token: 0x04004128 RID: 16680
			[NativeDisableUnsafePtrRestriction]
			public unsafe TreeInstance* arr_trees;

			// Token: 0x04004129 RID: 16681
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* trees_remap;

			// Token: 0x0400412A RID: 16682
			[NativeDisableUnsafePtrRestriction]
			public unsafe InputTreesNode.Data* pdata;

			// Token: 0x0400412B RID: 16683
			public Vector3 size;

			// Token: 0x0400412C RID: 16684
			public bool verify;

			// Token: 0x0400412D RID: 16685
			public int ignored;
		}
	}
}
