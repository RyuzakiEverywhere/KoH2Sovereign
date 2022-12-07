using System;
using Logic;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class UserCharacterInteraction
{
	// Token: 0x06001D6F RID: 7535 RVA: 0x0000B82A File Offset: 0x00009A2A
	public UserCharacterInteraction(Vars vars)
	{
	}

	// Token: 0x0400133F RID: 4927
	public Sprite Icon;

	// Token: 0x04001340 RID: 4928
	public string Name;

	// Token: 0x04001341 RID: 4929
	public string ToolTipText;

	// Token: 0x04001342 RID: 4930
	public string FlaivorText;

	// Token: 0x04001343 RID: 4931
	public Func<global::Character> Action;
}
