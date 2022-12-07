using System;
using TMPro;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class UISingleplayerLogin : UIWindow
{
	// Token: 0x06002564 RID: 9572 RVA: 0x0014C257 File Offset: 0x0014A457
	public override string GetDefId()
	{
		return UISingleplayerLogin.wnd_def_id;
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x0014C260 File Offset: 0x0014A460
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btn_SignIn != null)
		{
			this.btn_SignIn.onClick = new BSGButton.OnClick(this.OnSignIn);
		}
		if (this.btn_LoginOffline != null)
		{
			this.btn_LoginOffline.onClick = new BSGButton.OnClick(this.OnContinueOffline);
		}
		if (this.btn_Back != null)
		{
			this.btn_Back.onClick = new BSGButton.OnClick(this.OnBack);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x0014C2F3 File Offset: 0x0014A4F3
	private void OnSignIn(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("login");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x0014C317 File Offset: 0x0014A517
	private void OnContinueOffline(BSGButton btn)
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.CheckForSignInCoroAndTerminate();
		}
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("continue_offline");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x0014C34B File Offset: 0x0014A54B
	private void OnBack(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("back");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x0014C370 File Offset: 0x0014A570
	private void OnEnable()
	{
		this.Init();
		if (this.m_CaptionLabel != null)
		{
			UIText.SetTextKey(this.m_CaptionLabel, UISingleplayerLogin.wnd_def_id + ".caption", null, null);
		}
		if (this.m_DescriptionLabel != null)
		{
			UIText.SetTextKey(this.m_DescriptionLabel, UISingleplayerLogin.wnd_def_id + ".description", null, null);
		}
		if (this.m_SignInLabel != null)
		{
			UIText.SetTextKey(this.m_SignInLabel, UISingleplayerLogin.wnd_def_id + ".signin", null, null);
		}
		if (this.m_LoginOfflineLabel != null)
		{
			UIText.SetTextKey(this.m_LoginOfflineLabel, UISingleplayerLogin.wnd_def_id + ".loginoffline", null, null);
		}
		if (this.m_BackLabel != null)
		{
			UIText.SetTextKey(this.m_BackLabel, UISingleplayerLogin.wnd_def_id + ".back", null, null);
		}
		if (this.btn_SignIn != null)
		{
			GameObject gameObject = this.btn_SignIn.gameObject;
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			gameObject.SetActive(titleUI == null || titleUI.showSignButtons);
		}
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x0014C484 File Offset: 0x0014A684
	public override void Show()
	{
		this.Init();
		base.Show();
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400197D RID: 6525
	private static string wnd_def_id = "SingleplayerLogin";

	// Token: 0x0400197E RID: 6526
	public Action<string> onUserChoice;

	// Token: 0x0400197F RID: 6527
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI m_CaptionLabel;

	// Token: 0x04001980 RID: 6528
	[UIFieldTarget("id_DescriptionLabel")]
	private TextMeshProUGUI m_DescriptionLabel;

	// Token: 0x04001981 RID: 6529
	[UIFieldTarget("btn_SignIn")]
	private BSGButton btn_SignIn;

	// Token: 0x04001982 RID: 6530
	[UIFieldTarget("id_SignInLabel")]
	private TextMeshProUGUI m_SignInLabel;

	// Token: 0x04001983 RID: 6531
	[UIFieldTarget("btn_LoginOffline")]
	private BSGButton btn_LoginOffline;

	// Token: 0x04001984 RID: 6532
	[UIFieldTarget("id_LoginOfflineLabel")]
	private TextMeshProUGUI m_LoginOfflineLabel;

	// Token: 0x04001985 RID: 6533
	[UIFieldTarget("btn_Back")]
	private BSGButton btn_Back;

	// Token: 0x04001986 RID: 6534
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI m_BackLabel;
}
