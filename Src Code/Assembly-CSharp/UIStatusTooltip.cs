using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class UIStatusTooltip : MonoBehaviour
{
	// Token: 0x06001677 RID: 5751 RVA: 0x000E1507 File Offset: 0x000DF707
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = true;
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x000E1520 File Offset: 0x000DF720
	public bool HandleTooltip(Logic.Status status, BaseUI ui, Tooltip.Event evt)
	{
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return false;
		}
		if (status == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return true;
		}
		this.Status = status;
		this.vars = new Vars(this.Status);
		this.Refresh();
		return true;
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x000E1560 File Offset: 0x000DF760
	protected virtual void Refresh()
	{
		this.Init();
		this.RefreshCaption();
		this.RefreshDescription();
		this.RefreshContextAction();
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000E157A File Offset: 0x000DF77A
	protected virtual void RefreshCaption()
	{
		UIText.SetTextKey(this.m_Caption, "StatusTooltip.caption", this.vars, null);
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x000E1593 File Offset: 0x000DF793
	protected virtual void RefreshDescription()
	{
		UIText.SetTextKey(this.m_Descritpion, "StatusTooltip.text", this.vars, null);
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x000E15AC File Offset: 0x000DF7AC
	private void RefreshContextAction()
	{
		if (this.m_ContexActionDescription != null)
		{
			this.m_ContexActionDescription.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000E80 RID: 3712
	[UIFieldTarget("id_Caption")]
	protected TextMeshProUGUI m_Caption;

	// Token: 0x04000E81 RID: 3713
	[UIFieldTarget("id_Descritpion")]
	protected TextMeshProUGUI m_Descritpion;

	// Token: 0x04000E82 RID: 3714
	[UIFieldTarget("id_ContexActionDescription")]
	private TextMeshProUGUI m_ContexActionDescription;

	// Token: 0x04000E83 RID: 3715
	protected Logic.Status Status;

	// Token: 0x04000E84 RID: 3716
	protected Vars vars;

	// Token: 0x04000E85 RID: 3717
	private bool m_Initialzed;
}
