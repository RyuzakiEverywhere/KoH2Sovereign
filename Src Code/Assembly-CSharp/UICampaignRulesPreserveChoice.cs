using System;
using TMPro;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class UICampaignRulesPreserveChoice : MonoBehaviour
{
	// Token: 0x060023A2 RID: 9122 RVA: 0x001412F4 File Offset: 0x0013F4F4
	private void OnEnable()
	{
		UICommon.FindComponents(this, false);
		if (this.caption != null)
		{
			UIText.SetTextKey(this.caption, "TitleScreen.Multiplayer.CloneCampaign.CampaignRulesPreserveChoice.caption", null, null);
		}
		if (this.messageReset != null)
		{
			UIText.SetTextKey(this.messageReset, "TitleScreen.Multiplayer.CloneCampaign.CampaignRulesPreserveChoice.messageReset", null, null);
		}
		if (this.messagePreserve != null)
		{
			UIText.SetTextKey(this.messagePreserve, "TitleScreen.Multiplayer.CloneCampaign.CampaignRulesPreserveChoice.messagePreserve", null, null);
		}
		if (this.btnPreserveRules != null)
		{
			this.btnPreserveRules.onClick = new BSGButton.OnClick(this.OnPreserveRules);
			Tooltip.Get(this.btnPreserveRules.gameObject, true).SetDef("CampaignRulesPreserveChoicePreserveTooltip", null);
		}
		if (this.btnResetRules != null)
		{
			this.btnResetRules.onClick = new BSGButton.OnClick(this.OnResetRules);
			Tooltip.Get(this.btnResetRules.gameObject, true).SetDef("CampaignRulesPreserveChoiceResetTooltip", null);
		}
		if (this.btnClose != null)
		{
			this.btnClose.onClick = new BSGButton.OnClick(this.OnClose);
		}
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x0014140F File Offset: 0x0013F60F
	private void OnPreserveRules(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("preserve");
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x00141426 File Offset: 0x0013F626
	private void OnResetRules(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("reset");
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x0014143D File Offset: 0x0013F63D
	private void OnClose(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("close");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x040017EB RID: 6123
	[UIFieldTarget("btn_PreserveRules")]
	private BSGButton btnPreserveRules;

	// Token: 0x040017EC RID: 6124
	[UIFieldTarget("btn_ResetRules")]
	private BSGButton btnResetRules;

	// Token: 0x040017ED RID: 6125
	[UIFieldTarget("btn_Close")]
	private BSGButton btnClose;

	// Token: 0x040017EE RID: 6126
	[UIFieldTarget("id_Caption")]
	private TMP_Text caption;

	// Token: 0x040017EF RID: 6127
	[UIFieldTarget("id_MessageReset")]
	private TMP_Text messageReset;

	// Token: 0x040017F0 RID: 6128
	[UIFieldTarget("id_MessagePreserve")]
	private TMP_Text messagePreserve;

	// Token: 0x040017F1 RID: 6129
	public Action<string> onUserChoice;
}
