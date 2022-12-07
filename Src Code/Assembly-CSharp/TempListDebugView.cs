using System;

// Token: 0x020000D5 RID: 213
internal class TempListDebugView<T> where T : struct
{
	// Token: 0x06000A42 RID: 2626 RVA: 0x00075A1D File Offset: 0x00073C1D
	public TempListDebugView(TempList<T> lst)
	{
		this.lst = lst;
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00075A2C File Offset: 0x00073C2C
	public T[] Items
	{
		get
		{
			return this.lst.ToArray();
		}
	}

	// Token: 0x0400083F RID: 2111
	private TempList<T> lst;
}
