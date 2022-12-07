using System;
using System.Collections.Generic;

// Token: 0x020002C8 RID: 712
public class TableRowComparer : IComparer<UITableRow>
{
	// Token: 0x06002CB5 RID: 11445 RVA: 0x00174D9F File Offset: 0x00172F9F
	public TableRowComparer(Comparison<UITableRow> comparison)
	{
		this.comparison = comparison;
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x00174DB5 File Offset: 0x00172FB5
	public int Compare(UITableRow x, UITableRow y)
	{
		if (this.comparison == null)
		{
			return 0;
		}
		if (!this.descending)
		{
			return -1 * this.comparison(x, y);
		}
		return this.comparison(x, y);
	}

	// Token: 0x04001E77 RID: 7799
	public Comparison<UITableRow> comparison;

	// Token: 0x04001E78 RID: 7800
	public bool descending = true;
}
