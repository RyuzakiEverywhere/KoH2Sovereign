using System;
using Unity.Collections;

// Token: 0x0200008A RID: 138
public struct JobTLSData
{
	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600053E RID: 1342 RVA: 0x0003B391 File Offset: 0x00039591
	public static int Count
	{
		get
		{
			return 128;
		}
	}

	// Token: 0x040004E0 RID: 1248
	public Allocator allocator;

	// Token: 0x040004E1 RID: 1249
	public int item_size;
}
