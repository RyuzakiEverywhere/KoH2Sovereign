using System;
using TMPro;

// Token: 0x02000250 RID: 592
public class UIAPIVersionMismatch : UIWindow
{
	// Token: 0x06002452 RID: 9298 RVA: 0x00145E48 File Offset: 0x00144048
	public override string GetDefId()
	{
		return UIAPIVersionMismatch.wnd_def_id;
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x00145E50 File Offset: 0x00144050
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnContinue != null)
		{
			this.btnContinue.onClick = new BSGButton.OnClick(this.OnContinue);
		}
		if (this.btnBack != null)
		{
			this.btnBack.onClick = new BSGButton.OnClick(this.OnBack);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x00145EC0 File Offset: 0x001440C0
	private void OnEnable()
	{
		this.Init();
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UIAPIVersionMismatch.wnd_def_id + ".caption", null, null);
		}
		if (this.message != null)
		{
			UIText.SetTextKey(this.message, UIAPIVersionMismatch.wnd_def_id + ".message", null, null);
		}
		if (this.message2 != null)
		{
			UIText.SetTextKey(this.message2, UIAPIVersionMismatch.wnd_def_id + ".message2", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIAPIVersionMismatch.wnd_def_id + ".back", null, null);
		}
		if (this.continueLabel != null)
		{
			UIText.SetTextKey(this.continueLabel, UIAPIVersionMismatch.wnd_def_id + ".continue", null, null);
		}
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x00145FA5 File Offset: 0x001441A5
	private void OnContinue(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("ok");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x00145FF2 File Offset: 0x001441F2
	private void OnBack(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("back");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001893 RID: 6291
	private static string wnd_def_id = "TitleRegister";

	// Token: 0x04001894 RID: 6292
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x04001895 RID: 6293
	[UIFieldTarget("id_Message")]
	private TextMeshProUGUI message;

	// Token: 0x04001896 RID: 6294
	[UIFieldTarget("id_Message2")]
	private TextMeshProUGUI message2;

	// Token: 0x04001897 RID: 6295
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x04001898 RID: 6296
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x04001899 RID: 6297
	[UIFieldTarget("btn_Continue")]
	private BSGButton btnContinue;

	// Token: 0x0400189A RID: 6298
	[UIFieldTarget("id_Continue")]
	private TextMeshProUGUI continueLabel;

	// Token: 0x0400189B RID: 6299
	public Action<string> onUserChoice;
}
