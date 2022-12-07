using System;
using Logic;

// Token: 0x020001A1 RID: 417
public class UISquadIcon : ObjectIcon
{
	// Token: 0x060017A0 RID: 6048 RVA: 0x000E7F7F File Offset: 0x000E617F
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x000023FD File Offset: 0x000005FD
	private void Refresh()
	{
	}
}
