using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000087 RID: 135
public struct GrowBufferData : IDisposable
{
	// Token: 0x06000523 RID: 1315 RVA: 0x0003ACF4 File Offset: 0x00038EF4
	public void Dispose()
	{
		if (this.allocator <= Allocator.None)
		{
			return;
		}
		if (this.ptr != null)
		{
			UnsafeUtility.Free(this.ptr, this.allocator);
		}
		this.allocator = Allocator.None;
		this.ptr = null;
		this.count = -1;
		this.capacity = -1;
	}

	// Token: 0x040004D6 RID: 1238
	[NativeDisableUnsafePtrRestriction]
	public unsafe void* ptr;

	// Token: 0x040004D7 RID: 1239
	public int count;

	// Token: 0x040004D8 RID: 1240
	public int capacity;

	// Token: 0x040004D9 RID: 1241
	public int locked;

	// Token: 0x040004DA RID: 1242
	public Allocator allocator;

	// Token: 0x040004DB RID: 1243
	public int panic;

	// Token: 0x040004DC RID: 1244
	public int panic_locked;
}
