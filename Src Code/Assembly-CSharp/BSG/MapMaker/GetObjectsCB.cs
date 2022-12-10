using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace BSG.MapMaker
{
	// Token: 0x02000377 RID: 887
	[Serializable]
	public struct GetObjectsCB
	{
		// Token: 0x0600343D RID: 13373 RVA: 0x001A28E0 File Offset: 0x001A0AE0
		public static FunctionPointer<GetObjectsCB.GetObjectsFunc> GetFuncPtr(GetObjectsCB.GetObjectsFunc func)
		{
			FunctionPointer<GetObjectsCB.GetObjectsFunc> result = BurstCompiler.CompileFunctionPointer<GetObjectsCB.GetObjectsFunc>(func);
			FunctionPointer<GetObjectsCB.AddObjectsFunc> cb = default(FunctionPointer<GetObjectsCB.AddObjectsFunc>);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(0f, 0f, 0f, 0f, null, cb, null);
			}
			return result;
		}

		// Token: 0x0600343E RID: 13374 RVA: 0x001A2930 File Offset: 0x001A0B30
		public static FunctionPointer<GetObjectsCB.AddObjectsFunc> GetFuncPtr(GetObjectsCB.AddObjectsFunc func)
		{
			FunctionPointer<GetObjectsCB.AddObjectsFunc> result = BurstCompiler.CompileFunctionPointer<GetObjectsCB.AddObjectsFunc>(func);
			ObjectInstance objectInstance = default(ObjectInstance);
			for (int i = 0; i < 4; i++)
			{
				result.Invoke(ref objectInstance, null);
			}
			return result;
		}

		// Token: 0x0600343F RID: 13375 RVA: 0x001A296C File Offset: 0x001A0B6C
		public unsafe void GetObjects(float wx, float wy, float ww, float wh, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data)
		{
			if (this.get_objects_func == IntPtr.Zero)
			{
				return;
			}
			if (!cb.IsCreated)
			{
				return;
			}
			FunctionPointer<GetObjectsCB.GetObjectsFunc> functionPointer = new FunctionPointer<GetObjectsCB.GetObjectsFunc>(this.get_objects_func);
			functionPointer.Invoke(wx, wy, ww, wh, this.data, cb, cb_data);
		}

		// Token: 0x0400232D RID: 9005
		[NativeDisableUnsafePtrRestriction]
		public IntPtr get_objects_func;

		// Token: 0x0400232E RID: 9006
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* data;

		// Token: 0x020008B8 RID: 2232
		// (Invoke) Token: 0x060051EE RID: 20974
		public unsafe delegate void GetObjectsFunc(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data);

		// Token: 0x020008B9 RID: 2233
		// (Invoke) Token: 0x060051F2 RID: 20978
		public unsafe delegate void AddObjectsFunc(ref ObjectInstance obj, void* data);
	}
}
