using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000254 RID: 596
public class UIForgotPasswordRequest : UIWindow, IListener
{
	// Token: 0x06002481 RID: 9345 RVA: 0x00146DCE File Offset: 0x00144FCE
	public override string GetDefId()
	{
		return UIForgotPasswordRequest.wnd_def_id;
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x00146DD8 File Offset: 0x00144FD8
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
		if (this.btnResetPassword != null)
		{
			this.btnResetPassword.onClick = new BSGButton.OnClick(this.OnResetPassword);
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.AddListener(this);
		}
		UIText.SetText(this.statusMessage, string.Empty);
		this.m_Initialized = true;
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x00146E67 File Offset: 0x00145067
	protected override void Awake()
	{
		base.Awake();
		this.emailInput = global::Common.FindChildComponent<TMP_InputField>(base.gameObject, "id_EmailInput");
		GameObject gameObject = GameObject.Find("UI");
		this.title = ((gameObject != null) ? gameObject.GetComponent<Title>() : null);
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x00146EA4 File Offset: 0x001450A4
	private void OnEnable()
	{
		this.Init();
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UIForgotPasswordRequest.wnd_def_id + ".caption", null, null);
		}
		if (this.descriptionLabel != null)
		{
			UIText.SetTextKey(this.descriptionLabel, UIForgotPasswordRequest.wnd_def_id + ".description", null, null);
		}
		if (this.emailLabel != null)
		{
			UIText.SetTextKey(this.emailLabel, UIForgotPasswordRequest.wnd_def_id + ".email", null, null);
		}
		if (this.emailInputPlaceholder != null)
		{
			UIText.SetTextKey(this.emailInputPlaceholder, UIForgotPasswordRequest.wnd_def_id + ".email_prompt", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIForgotPasswordRequest.wnd_def_id + ".back", null, null);
		}
		if (this.resetPasswordLabel != null)
		{
			UIText.SetTextKey(this.resetPasswordLabel, UIForgotPasswordRequest.wnd_def_id + ".resetPassword", null, null);
		}
		if (this.emailInput != null)
		{
			if (string.IsNullOrEmpty(this.email))
			{
				this.email = string.Empty;
			}
			this.emailInput.text = this.email;
			this.emailInput.caretPosition = (string.IsNullOrEmpty(this.email) ? 0 : this.email.Length);
		}
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x00147010 File Offset: 0x00145210
	private void OnDisable()
	{
		UIText.SetText(this.statusMessage, string.Empty);
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x00147024 File Offset: 0x00145224
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (this.emailInput != null)
			{
				EventSystem current = EventSystem.current;
				this.emailInput.OnPointerClick(new PointerEventData(current));
				current.SetSelectedGameObject(this.emailInput.gameObject, new BaseEventData(current));
				return;
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				this.ResetPassword();
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.Back();
			}
		}
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x001470A8 File Offset: 0x001452A8
	private void ResetPassword()
	{
		if (this.emailInput == null)
		{
			Debug.LogWarning("Email input is missing!");
			return;
		}
		if (!this.ValidateInput())
		{
			return;
		}
		Action<string, string, string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("reset_password", this.emailInput.text, string.Empty);
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x001470FC File Offset: 0x001452FC
	private void Back()
	{
		if (this.title == null)
		{
			Debug.LogWarning("Title is missing!");
			return;
		}
		Action<string, string, string> action = this.onUserChoice;
		if (action != null)
		{
			action("back", string.Empty, string.Empty);
		}
		BaseUI.Get<TitleUI>().EnableForgotPasswordRequestWindow(false, null, null, null);
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x00147150 File Offset: 0x00145350
	private void OnResetPassword(BSGButton btn)
	{
		this.ResetPassword();
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x00147158 File Offset: 0x00145358
	private void OnBack(BSGButton btn)
	{
		this.Back();
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x00147160 File Offset: 0x00145360
	private bool ValidateInput()
	{
		if (string.IsNullOrEmpty(this.emailInput.text))
		{
			UIText.SetText(this.statusMessage, global::Defs.Localize("TitleScreen.Multiplayer.Login.enter_email", null, null, true, true));
			return false;
		}
		return true;
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x00147190 File Offset: 0x00145390
	public static string PasswordRecoveryResultToMessage(Logic.Common.APIResult result)
	{
		if (result == Logic.Common.APIResult.PasswordRecovery_InsufficientContactData || result == Logic.Common.APIResult.Fail)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.PasswordRecovery.StatusMessage.insufficient_data", null, null, true, true);
		}
		return string.Format("Error: Cannot parse THQNO result message! Result: {0}", result);
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x001471BC File Offset: 0x001453BC
	protected override void OnDestroy()
	{
		base.OnDestroy();
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.DelListener(this);
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x001471D4 File Offset: 0x001453D4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "forgot_password_response" || message == "set_new_password_response")
		{
			UIText.SetText(this.statusMessage, (string)param);
		}
	}

	// Token: 0x040018BC RID: 6332
	private static string wnd_def_id = "ForgotPasswordRequest";

	// Token: 0x040018BD RID: 6333
	[HideInInspector]
	public string email = string.Empty;

	// Token: 0x040018BE RID: 6334
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x040018BF RID: 6335
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI descriptionLabel;

	// Token: 0x040018C0 RID: 6336
	[UIFieldTarget("id_EmailLabel")]
	private TextMeshProUGUI emailLabel;

	// Token: 0x040018C1 RID: 6337
	[UIFieldTarget("id_EmailInput")]
	private TMP_InputField emailInput;

	// Token: 0x040018C2 RID: 6338
	[UIFieldTarget("id_EmailInputPlaceholder")]
	private TextMeshProUGUI emailInputPlaceholder;

	// Token: 0x040018C3 RID: 6339
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x040018C4 RID: 6340
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x040018C5 RID: 6341
	[UIFieldTarget("btn_ResetPassword")]
	private BSGButton btnResetPassword;

	// Token: 0x040018C6 RID: 6342
	[UIFieldTarget("id_ResetPasswordLabel")]
	private TextMeshProUGUI resetPasswordLabel;

	// Token: 0x040018C7 RID: 6343
	[UIFieldTarget("id_StatusMessage")]
	public TextMeshProUGUI statusMessage;

	// Token: 0x040018C8 RID: 6344
	private Title title;

	// Token: 0x040018C9 RID: 6345
	public Action<string, string, string> onUserChoice;
}
