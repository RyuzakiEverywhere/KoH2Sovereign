using System;
using TMPro;

// Token: 0x02000251 RID: 593
public class UIApiNoConnection : UIWindow
{
	// Token: 0x0600245A RID: 9306 RVA: 0x00146022 File Offset: 0x00144222
	public override string GetDefId()
	{
		return UIApiNoConnection.wnd_def_id;
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x00146029 File Offset: 0x00144229
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnBack != null)
		{
			this.btnBack.onClick = new BSGButton.OnClick(this.OnBack);
		}
		this.m_Initialized = true;
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x00146068 File Offset: 0x00144268
	private void OnEnable()
	{
		this.Init();
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UIApiNoConnection.wnd_def_id + ".caption", null, null);
		}
		if (this.message != null)
		{
			UIText.SetTextKey(this.message, UIApiNoConnection.wnd_def_id + ".message", null, null);
		}
		if (this.message2 != null)
		{
			UIText.SetTextKey(this.message2, UIApiNoConnection.wnd_def_id + ".message2", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIApiNoConnection.wnd_def_id + ".back", null, null);
		}
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x00146123 File Offset: 0x00144323
	private void OnBack(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("back");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400189C RID: 6300
	private static string wnd_def_id = "CannotConnect";

	// Token: 0x0400189D RID: 6301
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x0400189E RID: 6302
	[UIFieldTarget("id_Message")]
	private TextMeshProUGUI message;

	// Token: 0x0400189F RID: 6303
	[UIFieldTarget("id_Message2")]
	private TextMeshProUGUI message2;

	// Token: 0x040018A0 RID: 6304
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x040018A1 RID: 6305
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x040018A2 RID: 6306
	public Action<string> onUserChoice;
}
