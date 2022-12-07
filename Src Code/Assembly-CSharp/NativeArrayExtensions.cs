using System;
using Unity.Collections;

// Token: 0x02000112 RID: 274
public static class NativeArrayExtensions
{
	// Token: 0x06000C8E RID: 3214 RVA: 0x0008B6FD File Offset: 0x000898FD
	public static void ResizeNativeArray<T>(ref NativeArray<T> array, int length) where T : struct
	{
		if (array.Length != length)
		{
			if (array.IsCreated)
			{
				array.Dispose();
			}
			array = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}
	}
}
