using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class MinimapButtons : MonoBehaviour
{
	// Token: 0x06002363 RID: 9059 RVA: 0x0013FBC2 File Offset: 0x0013DDC2
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, base.transform);
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x040017B0 RID: 6064
	[HideInInspector]
	public MinimapViewModeButton selected;
}
