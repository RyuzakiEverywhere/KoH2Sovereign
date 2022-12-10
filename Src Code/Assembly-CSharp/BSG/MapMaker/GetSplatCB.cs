using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace BSG.MapMaker
{
	// Token: 0x02000374 RID: 884
	[Serializable]
	public struct GetSplatCB
	{
		// Token: 0x06003437 RID: 13367 RVA: 0x001A2758 File Offset: 0x001A0958
		public static FunctionPointer<GetSplatCB.GetTileFunc> GetFuncPtr(GetSplatCB.GetTileFunc func)
		{
			FunctionPointer<GetSplatCB.GetTileFunc> result = BurstCompiler.CompileFunctionPointer<GetSplatCB.GetTileFunc>(func);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(0f, 0f, null, 0, null);
			}
			return result;
		}

		// Token: 0x06003438 RID: 13368 RVA: 0x001A2794 File Offset: 0x001A0994
		public unsafe void GetTile(float wx, float wy, float* result, int layers)
		{
			if (this.get_tile_func != IntPtr.Zero)
			{
				FunctionPointer<GetSplatCB.GetTileFunc> functionPointer = new FunctionPointer<GetSplatCB.GetTileFunc>(this.get_tile_func);
				functionPointer.Invoke(wx, wy, result, layers, this.data);
				return;
			}
		}

		// Token: 0x04002328 RID: 9000
		[NativeDisableUnsafePtrRestriction]
		public IntPtr get_tile_func;

		// Token: 0x04002329 RID: 9001
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* data;

		// Token: 0x020008B4 RID: 2228
		// (Invoke) Token: 0x060051DE RID: 20958
		public unsafe delegate void GetTileFunc(float wx, float wy, float* result, int layers, void* data);
	}
}
