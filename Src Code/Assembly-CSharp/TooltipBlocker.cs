using System;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class TooltipBlocker : MonoBehaviour
{
	// Token: 0x06002E41 RID: 11841 RVA: 0x0017E846 File Offset: 0x0017CA46
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, this.parent);
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x04001F4C RID: 8012
	public Transform parent;
}
