using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001E8 RID: 488
public class UICharacterInteractionIcon : Hotspot
{
	// Token: 0x06001D62 RID: 7522 RVA: 0x00114C6D File Offset: 0x00112E6D
	protected override void OnEnable()
	{
		Tooltip.Get(base.gameObject, true).handler = new Tooltip.Handler(this.HandleTooltip);
		base.OnEnable();
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x00114C92 File Offset: 0x00112E92
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.Data != null)
		{
			Debug.Log("Select Action " + this.Data.Name);
		}
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x00114CBD File Offset: 0x00112EBD
	public void SetData(UserCharacterInteraction data)
	{
		this.Data = data;
		this.Refresh();
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x00114CCC File Offset: 0x00112ECC
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Image_Icon != null)
		{
			this.Image_Icon.sprite = this.Data.Icon;
		}
		UIText.SetText(this.TMP_ActionName, this.Data.Name);
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x00114D1C File Offset: 0x00112F1C
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Show)
		{
			if (this.Data == null)
			{
				return true;
			}
			if (string.IsNullOrEmpty(this.Data.ToolTipText))
			{
				tooltip.TextKey = string.Format("Unknown", Array.Empty<object>());
			}
			else
			{
				tooltip.TextKey = this.Data.ToolTipText;
			}
			return false;
		}
		else
		{
			if (evt != Tooltip.Event.Update)
			{
				return false;
			}
			if (this.Data == null)
			{
				ui.DestroyTooltip();
				return true;
			}
			return false;
		}
	}

	// Token: 0x04001339 RID: 4921
	[UIFieldTarget("id_StatusIcon")]
	[SerializeField]
	private Image Image_Icon;

	// Token: 0x0400133A RID: 4922
	[UIFieldTarget("id_actionName")]
	[SerializeField]
	private TextMeshProUGUI TMP_ActionName;

	// Token: 0x0400133B RID: 4923
	public UserCharacterInteraction Data;
}
