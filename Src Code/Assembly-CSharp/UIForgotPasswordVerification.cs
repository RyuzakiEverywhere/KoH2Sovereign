using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000255 RID: 597
public class UIForgotPasswordVerification : UIWindow
{
	// Token: 0x06002492 RID: 9362 RVA: 0x00147220 File Offset: 0x00145420
	public override string GetDefId()
	{
		return UIForgotPasswordVerification.wnd_def_id;
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x00147228 File Offset: 0x00145428
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btn_SetNewPassword != null)
		{
			this.btn_SetNewPassword.onClick = new BSGButton.OnClick(this.OnSetNewPassword);
		}
		if (this.btn_SendAnotherCode != null)
		{
			this.btn_SendAnotherCode.onClick = new BSGButton.OnClick(this.OnSendAnotherCode);
		}
		if (this.btn_Back != null)
		{
			this.btn_Back.onClick = new BSGButton.OnClick(this.OnBack);
		}
		if (this.btn_hiddenEye != null)
		{
			this.btn_hiddenEye.onClick = new BSGButton.OnClick(this.OnTogglePasswordVisibility);
		}
		this.inputs.Add(this.verificationCodeInput);
		this.inputs.Add(this.newPasswordInput);
		this.m_Initialized = true;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x00147302 File Offset: 0x00145502
	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = GameObject.Find("UI");
		this.title = ((gameObject != null) ? gameObject.GetComponent<Title>() : null);
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x00147328 File Offset: 0x00145528
	private void OnEnable()
	{
		this.Init();
		UIText.SetText(this.statusMessage, string.Empty);
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UIForgotPasswordVerification.wnd_def_id + ".caption", null, null);
		}
		if (this.tip != null)
		{
			UIText.SetTextKey(this.tip, UIForgotPasswordVerification.wnd_def_id + ".tip", null, null);
		}
		if (this.verificationCodeLabel != null)
		{
			UIText.SetTextKey(this.verificationCodeLabel, UIForgotPasswordVerification.wnd_def_id + ".verification", null, null);
		}
		if (this.verificationCodePlaceholder != null)
		{
			UIText.SetTextKey(this.verificationCodePlaceholder, UIForgotPasswordVerification.wnd_def_id + ".verification_placeholder", null, null);
		}
		if (this.newPasswordLabel != null)
		{
			UIText.SetTextKey(this.newPasswordLabel, UIForgotPasswordVerification.wnd_def_id + ".newpassword", null, null);
		}
		if (this.newPasswordPlaceholder != null)
		{
			UIText.SetTextKey(this.newPasswordPlaceholder, UIForgotPasswordVerification.wnd_def_id + ".newpassword_placeholder", null, null);
		}
		if (this.setNewPasswordLabel != null)
		{
			UIText.SetTextKey(this.setNewPasswordLabel, UIForgotPasswordVerification.wnd_def_id + ".setnewpassword", null, null);
		}
		if (this.sendAnotherCodeLabel != null)
		{
			UIText.SetTextKey(this.sendAnotherCodeLabel, UIForgotPasswordVerification.wnd_def_id + ".sendanothercode", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIForgotPasswordVerification.wnd_def_id + ".back", null, null);
		}
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x001474C8 File Offset: 0x001456C8
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (this.inputSelectionIndex >= this.inputs.Count)
			{
				this.selectedInput = this.inputs[0].gameObject;
				this.inputSelectionIndex = 0;
			}
			else
			{
				this.selectedInput = this.inputs[this.inputSelectionIndex].gameObject;
			}
			this.inputSelectionIndex++;
			if (this.selectedInput != null)
			{
				TMP_InputField component = this.selectedInput.GetComponent<TMP_InputField>();
				if (component != null)
				{
					component.OnPointerClick(new PointerEventData(this.eventSystem));
					this.eventSystem.SetSelectedGameObject(this.selectedInput, new BaseEventData(this.eventSystem));
					return;
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			this.SetNewPassword();
		}
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x001475B0 File Offset: 0x001457B0
	private void SetNewPassword()
	{
		if (!this.ValidateInput())
		{
			return;
		}
		Action<string, string, string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("set_new_password", this.newPasswordInput.text, this.verificationCodeInput.text);
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x001475E8 File Offset: 0x001457E8
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
		BaseUI.Get<TitleUI>().EnableForgotPasswordVerificationWindow(false, null, null);
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x0014763B File Offset: 0x0014583B
	private void OnSetNewPassword(BSGButton btn)
	{
		this.SetNewPassword();
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x00147643 File Offset: 0x00145843
	private void OnSendAnotherCode(BSGButton btn)
	{
		if (this.title == null)
		{
			Debug.LogWarning("Title is missing!");
			return;
		}
		Action<string, string, string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("send_another_code", string.Empty, string.Empty);
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x00147680 File Offset: 0x00145880
	private void OnTogglePasswordVisibility(BSGButton btn)
	{
		if (this.newPasswordInput == null)
		{
			return;
		}
		if (this.newPasswordInput.contentType == TMP_InputField.ContentType.Standard)
		{
			this.newPasswordInput.contentType = TMP_InputField.ContentType.Password;
			this.newPasswordInput.ForceLabelUpdate();
			return;
		}
		if (this.newPasswordInput.contentType == TMP_InputField.ContentType.Password)
		{
			this.newPasswordInput.contentType = TMP_InputField.ContentType.Standard;
			this.newPasswordInput.ForceLabelUpdate();
		}
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x001476E6 File Offset: 0x001458E6
	private void OnBack(BSGButton btn)
	{
		this.Back();
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x001476EE File Offset: 0x001458EE
	public override bool OnBackInputAction()
	{
		base.OnBackInputAction();
		this.Back();
		return true;
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x00147700 File Offset: 0x00145900
	private bool ValidateInput()
	{
		if (string.IsNullOrEmpty(this.verificationCodeInput.text))
		{
			UIText.SetText(this.statusMessage, global::Defs.Localize("TitleScreen.Multiplayer.PasswordRecovery.enter_verification_code", null, null, true, true));
			return false;
		}
		if (string.IsNullOrEmpty(this.newPasswordInput.text))
		{
			UIText.SetText(this.statusMessage, global::Defs.Localize("TitleScreen.Multiplayer.PasswordRecovery.enter_new_password", null, null, true, true));
			return false;
		}
		return true;
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x00147768 File Offset: 0x00145968
	public static string ConfirmPasswordRecoveryResultToMessage(Logic.Common.APIResult result)
	{
		if (result == Logic.Common.APIResult.PasswordRecovery_PasswordTooShort)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_short", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.PasswordRecovery_PasswordTooLong)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_long", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.PasswordRecovery_PasswordTooWeakUseMoreCharacters || result == Logic.Common.APIResult.PasswordRecovery_PasswordTooWeakUseMoreVariation)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_weak", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.PasswordRecovery_PasswordHasInvalidASCIICharacters || result == Logic.Common.APIResult.PasswordRecovery_PasswordHasInvalidUTF8Characters || result == Logic.Common.APIResult.PasswordRecovery_InvalidPassword)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.invalid_password", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.PasswordRecovery_AuthenticationFailed)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.PasswordRecovery.StatusMessage.invalid_verification_code", null, null, true, true);
		}
		return string.Format("Error: Cannot parse THQNO result message! Result: {0}", result);
	}

	// Token: 0x040018CA RID: 6346
	private static string wnd_def_id = "PasswordReset";

	// Token: 0x040018CB RID: 6347
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x040018CC RID: 6348
	[UIFieldTarget("id_Tip")]
	private TextMeshProUGUI tip;

	// Token: 0x040018CD RID: 6349
	[UIFieldTarget("id_VerificationCodeLabel")]
	private TextMeshProUGUI verificationCodeLabel;

	// Token: 0x040018CE RID: 6350
	[UIFieldTarget("id_VerificationCodeInput")]
	private TMP_InputField verificationCodeInput;

	// Token: 0x040018CF RID: 6351
	[UIFieldTarget("id_VerificationCodePlaceholder")]
	private TextMeshProUGUI verificationCodePlaceholder;

	// Token: 0x040018D0 RID: 6352
	[UIFieldTarget("id_NewPasswordLabel")]
	private TextMeshProUGUI newPasswordLabel;

	// Token: 0x040018D1 RID: 6353
	[UIFieldTarget("id_NewPasswordInput")]
	private TMP_InputField newPasswordInput;

	// Token: 0x040018D2 RID: 6354
	[UIFieldTarget("id_NewPasswordPlaceholder")]
	private TextMeshProUGUI newPasswordPlaceholder;

	// Token: 0x040018D3 RID: 6355
	[UIFieldTarget("id_HiddenEye")]
	private BSGButton btn_hiddenEye;

	// Token: 0x040018D4 RID: 6356
	[UIFieldTarget("btn_SetNewPassword")]
	private BSGButton btn_SetNewPassword;

	// Token: 0x040018D5 RID: 6357
	[UIFieldTarget("id_SetNewPasswordLabel")]
	private TextMeshProUGUI setNewPasswordLabel;

	// Token: 0x040018D6 RID: 6358
	[UIFieldTarget("btn_SendAnotherCode")]
	private BSGButton btn_SendAnotherCode;

	// Token: 0x040018D7 RID: 6359
	[UIFieldTarget("id_SendAnotherCodeLabel")]
	private TextMeshProUGUI sendAnotherCodeLabel;

	// Token: 0x040018D8 RID: 6360
	[UIFieldTarget("id_StatusMessage")]
	public TextMeshProUGUI statusMessage;

	// Token: 0x040018D9 RID: 6361
	[UIFieldTarget("btn_Back")]
	private BSGButton btn_Back;

	// Token: 0x040018DA RID: 6362
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x040018DB RID: 6363
	private EventSystem eventSystem;

	// Token: 0x040018DC RID: 6364
	private List<TMP_InputField> inputs = new List<TMP_InputField>();

	// Token: 0x040018DD RID: 6365
	private GameObject selectedInput;

	// Token: 0x040018DE RID: 6366
	private int inputSelectionIndex;

	// Token: 0x040018DF RID: 6367
	public Action<string, string, string> onUserChoice;

	// Token: 0x040018E0 RID: 6368
	private Title title;
}
