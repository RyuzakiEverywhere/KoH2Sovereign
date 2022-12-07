using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200020B RID: 523
public class UIKingdom : MonoBehaviour
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x00125872 File Offset: 0x00123A72
	// (set) Token: 0x06001FD2 RID: 8146 RVA: 0x0012587A File Offset: 0x00123A7A
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x06001FD3 RID: 8147 RVA: 0x00125883 File Offset: 0x00123A83
	public void SetData(Logic.Kingdom kingdom)
	{
		this.Kingdom = kingdom;
	}

	// Token: 0x06001FD4 RID: 8148 RVA: 0x0012588C File Offset: 0x00123A8C
	private void Refresh()
	{
		Logic.Kingdom kingdom = this.Kingdom;
	}

	// Token: 0x04001518 RID: 5400
	public Text Value_Name;
}
