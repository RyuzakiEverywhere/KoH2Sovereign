using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class TooltipOrigin : MonoBehaviour
{
	// Token: 0x06002E52 RID: 11858 RVA: 0x0017EFFE File Offset: 0x0017D1FE
	public static bool TryGetOrigin(out TooltipOrigin o)
	{
		if (TooltipOrigin.tooltipOrigins == null || TooltipOrigin.tooltipOrigins.Count == 0)
		{
			o = null;
			return false;
		}
		o = TooltipOrigin.tooltipOrigins.Peek();
		return true;
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x0017F025 File Offset: 0x0017D225
	private void OnEnable()
	{
		TooltipOrigin.tooltipOrigins.Push(this);
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x0017F032 File Offset: 0x0017D232
	private void OnDisable()
	{
		TooltipOrigin.tooltipOrigins.Pop();
	}

	// Token: 0x04001F5B RID: 8027
	private static Stack<TooltipOrigin> tooltipOrigins = new Stack<TooltipOrigin>();

	// Token: 0x04001F5C RID: 8028
	public TextAnchor ancor = TextAnchor.MiddleCenter;
}
