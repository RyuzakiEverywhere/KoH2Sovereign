using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace BSG.MapMaker
{
	// Token: 0x02000376 RID: 886
	[Serializable]
	public struct GetTreesCB
	{
		// Token: 0x0600343A RID: 13370 RVA: 0x001A2800 File Offset: 0x001A0A00
		public static FunctionPointer<GetTreesCB.GetTreesFunc> GetFuncPtr(GetTreesCB.GetTreesFunc func)
		{
			FunctionPointer<GetTreesCB.GetTreesFunc> result = BurstCompiler.CompileFunctionPointer<GetTreesCB.GetTreesFunc>(func);
			FunctionPointer<GetTreesCB.AddTreeFunc> cb = default(FunctionPointer<GetTreesCB.AddTreeFunc>);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(0f, 0f, 0f, 0f, null, cb, null);
			}
			return result;
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x001A2850 File Offset: 0x001A0A50
		public static FunctionPointer<GetTreesCB.AddTreeFunc> GetFuncPtr(GetTreesCB.AddTreeFunc func)
		{
			FunctionPointer<GetTreesCB.AddTreeFunc> result = BurstCompiler.CompileFunctionPointer<GetTreesCB.AddTreeFunc>(func);
			TreeInstance treeInstance = default(TreeInstance);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(ref treeInstance, null);
			}
			return result;
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x001A288C File Offset: 0x001A0A8C
		public unsafe void GetTrees(float wx, float wy, float ww, float wh, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data)
		{
			if (this.get_trees_func == IntPtr.Zero)
			{
				return;
			}
			if (!cb.IsCreated)
			{
				return;
			}
			FunctionPointer<GetTreesCB.GetTreesFunc> functionPointer = new FunctionPointer<GetTreesCB.GetTreesFunc>(this.get_trees_func);
			functionPointer.Invoke(wx, wy, ww, wh, this.data, cb, cb_data);
		}

		// Token: 0x0400232B RID: 9003
		[NativeDisableUnsafePtrRestriction]
		public IntPtr get_trees_func;

		// Token: 0x0400232C RID: 9004
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* data;

		// Token: 0x020008B6 RID: 2230
		// (Invoke) Token: 0x060051E6 RID: 20966
		public unsafe delegate void GetTreesFunc(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data);

		// Token: 0x020008B7 RID: 2231
		// (Invoke) Token: 0x060051EA RID: 20970
		public unsafe delegate void AddTreeFunc(ref TreeInstance tree, void* data);
	}
}
