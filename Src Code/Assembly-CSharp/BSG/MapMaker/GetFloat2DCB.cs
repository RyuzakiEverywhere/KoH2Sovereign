using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace BSG.MapMaker
{
	// Token: 0x02000373 RID: 883
	[Serializable]
	public struct GetFloat2DCB
	{
		// Token: 0x06003435 RID: 13365 RVA: 0x001A26D4 File Offset: 0x001A08D4
		public static FunctionPointer<GetFloat2DCB.GetValueFunc> GetFuncPtr(GetFloat2DCB.GetValueFunc func)
		{
			FunctionPointer<GetFloat2DCB.GetValueFunc> result = BurstCompiler.CompileFunctionPointer<GetFloat2DCB.GetValueFunc>(func);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(0f, 0f, null);
			}
			return result;
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x001A2710 File Offset: 0x001A0910
		public float GetValue(float wx, float wy)
		{
			if (this.get_value_func == IntPtr.Zero)
			{
				return 0f;
			}
			FunctionPointer<GetFloat2DCB.GetValueFunc> functionPointer = new FunctionPointer<GetFloat2DCB.GetValueFunc>(this.get_value_func);
			return functionPointer.Invoke(wx, wy, this.data);
		}

		// Token: 0x04002326 RID: 8998
		[NativeDisableUnsafePtrRestriction]
		public IntPtr get_value_func;

		// Token: 0x04002327 RID: 8999
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* data;

		// Token: 0x020008B3 RID: 2227
		// (Invoke) Token: 0x060051DA RID: 20954
		public unsafe delegate float GetValueFunc(float wx, float wy, void* data);
	}
}
