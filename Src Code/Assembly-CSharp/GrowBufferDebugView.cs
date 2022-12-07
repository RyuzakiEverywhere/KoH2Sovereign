using System;
using System.Runtime.CompilerServices;

// Token: 0x02000089 RID: 137
internal sealed class GrowBufferDebugView<[IsUnmanaged] T> where T : struct, ValueType
{
	// Token: 0x0600053C RID: 1340 RVA: 0x0003B375 File Offset: 0x00039575
	public GrowBufferDebugView(GrowBuffer<T> buf)
	{
		this.buf = buf;
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600053D RID: 1341 RVA: 0x0003B384 File Offset: 0x00039584
	public T[] Items
	{
		get
		{
			return this.buf.ToArray();
		}
	}

	// Token: 0x040004DF RID: 1247
	private GrowBuffer<T> buf;
}
