using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000259 RID: 601
public class UILogin : UIWindow, IListener
{
	// Token: 0x06002503 RID: 9475 RVA: 0x0014A28C File Offset: 0x0014848C
	public override string GetDefId()
	{
		return UILogin.wnd_def_id;
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x0014A294 File Offset: 0x00148494
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnLogin != null)
		{
			this.btnLogin.onClick = new BSGButton.OnClick(this.OnLogin);
		}
		if (this.btnRegister != null)
		{
			this.btnRegister.onClick = new BSGButton.OnClick(this.OnRegister);
		}
		if (this.btnForgotPassword != null)
		{
			this.btnForgotPassword.onClick = new BSGButton.OnClick(this.OnForgotPassword);
		}
		if (this.btnBack != null)
		{
			this.btnBack.onClick = new BSGButton.OnClick(this.OnBack);
		}
		if (this.btnHiddenEye != null)
		{
			this.btnHiddenEye.onClick = new BSGButton.OnClick(this.OnTogglePasswordVisibility);
		}
		this.inputs.Add(this.emailInput);
		this.inputs.Add(this.passwordInput);
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.AddListener(this);
		}
		UIText.SetText(this.statusMessage, string.Empty);
		this.m_Initialized = true;
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x0014A3B4 File Offset: 0x001485B4
	protected override void Awake()
	{
		base.Awake();
		this.eventSystem = EventSystem.current;
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x0014A3C8 File Offset: 0x001485C8
	private void OnEnable()
	{
		this.Init();
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UILogin.wnd_def_id + ".caption", null, null);
		}
		if (this.emailLabel != null)
		{
			UIText.SetTextKey(this.emailLabel, UILogin.wnd_def_id + ".email", null, null);
		}
		if (this.emailInputPlaceholder != null)
		{
			UIText.SetTextKey(this.emailInputPlaceholder, UILogin.wnd_def_id + ".email_prompt", null, null);
		}
		if (this.passwordLabel != null)
		{
			UIText.SetTextKey(this.passwordLabel, UILogin.wnd_def_id + ".password", null, null);
		}
		if (this.passwordInputPlaceholder != null)
		{
			UIText.SetTextKey(this.passwordInputPlaceholder, UILogin.wnd_def_id + ".password_prompt", null, null);
		}
		if (this.loginLabel != null)
		{
			UIText.SetTextKey(this.loginLabel, UILogin.wnd_def_id + ".login", null, null);
		}
		if (this.registerLabel != null)
		{
			UIText.SetTextKey(this.registerLabel, UILogin.wnd_def_id + ".register", null, null);
		}
		if (this.forgotPasswordLabel != null)
		{
			UIText.SetTextKey(this.forgotPasswordLabel, UILogin.wnd_def_id + ".forgot_password", null, null);
		}
		if (this.emailInput != null && this.passwordInput != null && this.statusMessage != null && string.IsNullOrEmpty(this.email) && string.IsNullOrEmpty(this.password))
		{
			this.emailInput.text = "";
			this.emailInput.caretPosition = 0;
			this.passwordInput.text = "";
			this.passwordInput.caretPosition = 0;
		}
		if (this.emailInput != null && !string.IsNullOrEmpty(this.email))
		{
			this.emailInput.text = this.email;
		}
		if (this.passwordInput != null)
		{
			if (string.IsNullOrEmpty(this.password))
			{
				this.password = string.Empty;
			}
			this.passwordInput.text = this.password;
		}
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x0014A60A File Offset: 0x0014880A
	private void OnDisable()
	{
		UIText.SetText(this.statusMessage, string.Empty);
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x0014A61C File Offset: 0x0014881C
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
			this.Login();
		}
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x0014A704 File Offset: 0x00148904
	private void OnLogin(BSGButton btn)
	{
		this.Login();
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x0014A70C File Offset: 0x0014890C
	private void OnTogglePasswordVisibility(BSGButton btn)
	{
		if (this.passwordInput == null)
		{
			return;
		}
		if (this.passwordInput.contentType == TMP_InputField.ContentType.Standard)
		{
			this.passwordInput.contentType = TMP_InputField.ContentType.Password;
			this.passwordInput.ForceLabelUpdate();
			return;
		}
		if (this.passwordInput.contentType == TMP_InputField.ContentType.Password)
		{
			this.passwordInput.contentType = TMP_InputField.ContentType.Standard;
			this.passwordInput.ForceLabelUpdate();
		}
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x0014A772 File Offset: 0x00148972
	public override bool OnBackInputAction()
	{
		base.OnBackInputAction();
		this.Back();
		return true;
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x0014A784 File Offset: 0x00148984
	private void Login()
	{
		UIText.SetText(this.statusMessage, string.Empty);
		if (this.emailInput == null || this.passwordInput == null)
		{
			Debug.LogWarning("UI is missing!");
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
		action("login", this.emailInput.text, this.passwordInput.text);
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x0014A7FC File Offset: 0x001489FC
	private void Back()
	{
		Action<string, string, string> action = this.onUserChoice;
		if (action != null)
		{
			action("back", this.emailInput.text, this.passwordInput.text);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x0014A836 File Offset: 0x00148A36
	private void OnRegister(BSGButton btn)
	{
		Action<string, string, string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("register", this.emailInput.text, this.passwordInput.text);
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x0014A863 File Offset: 0x00148A63
	private void OnForgotPassword(BSGButton btn)
	{
		Action<string, string, string> action = this.onUserChoice;
		if (action != null)
		{
			action("forgot_password", this.emailInput.text, this.passwordInput.text);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x0014A89D File Offset: 0x00148A9D
	private void OnBack(BSGButton btn)
	{
		this.Back();
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x0014A8A8 File Offset: 0x00148AA8
	private bool ValidateInput()
	{
		if (string.IsNullOrEmpty(this.emailInput.text))
		{
			UIText.SetText(this.statusMessage, global::Defs.Localize("TitleScreen.Multiplayer.Login.enter_email", null, null, true, true));
			return false;
		}
		if (string.IsNullOrEmpty(this.passwordInput.text))
		{
			UIText.SetText(this.statusMessage, global::Defs.Localize("TitleScreen.Multiplayer.Login.enter_password", null, null, true, true));
			return false;
		}
		return true;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x0014A910 File Offset: 0x00148B10
	public static string SignInResultToMessage(SignInResultCD result)
	{
		if (result.result == Logic.Common.APIResult.SignIn_AuthenticationFailed || result.result == Logic.Common.APIResult.Account_AuthenticationFailed)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Login.StatusMessage.authentication_failed", null, null, true, true);
		}
		if (result.result == Logic.Common.APIResult.ServiceUnavaliable)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Login.StatusMessage.service_unavailable", null, null, true, true);
		}
		if (!result.networkingAvaliable)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Login.StatusMessage.networking_unavailable", null, null, true, true);
		}
		return string.Format("Error: Cannot parse THQNO result message! Result: {0}", result.result);
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x001471BC File Offset: 0x001453BC
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

	// Token: 0x06002515 RID: 9493 RVA: 0x0014A98B File Offset: 0x00148B8B
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "register_response")
		{
			UIText.SetText(this.statusMessage, (string)param);
		}
	}

	// Token: 0x04001926 RID: 6438
	private static string wnd_def_id = "TitleLogin";

	// Token: 0x04001927 RID: 6439
	[HideInInspector]
	public string email = "";

	// Token: 0x04001928 RID: 6440
	[HideInInspector]
	public string password = "";

	// Token: 0x04001929 RID: 6441
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x0400192A RID: 6442
	[UIFieldTarget("id_EmailLabel")]
	private TextMeshProUGUI emailLabel;

	// Token: 0x0400192B RID: 6443
	[UIFieldTarget("id_EmailInput")]
	private TMP_InputField emailInput;

	// Token: 0x0400192C RID: 6444
	[UIFieldTarget("id_EmailInputPlaceholder")]
	private TextMeshProUGUI emailInputPlaceholder;

	// Token: 0x0400192D RID: 6445
	[UIFieldTarget("id_PasswordLabel")]
	private TextMeshProUGUI passwordLabel;

	// Token: 0x0400192E RID: 6446
	[UIFieldTarget("id_PasswordInput")]
	private TMP_InputField passwordInput;

	// Token: 0x0400192F RID: 6447
	[UIFieldTarget("id_PasswordInputPlaceholder")]
	private TextMeshProUGUI passwordInputPlaceholder;

	// Token: 0x04001930 RID: 6448
	[UIFieldTarget("id_HiddenEye")]
	private BSGButton btnHiddenEye;

	// Token: 0x04001931 RID: 6449
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x04001932 RID: 6450
	[UIFieldTarget("btn_Login")]
	private BSGButton btnLogin;

	// Token: 0x04001933 RID: 6451
	[UIFieldTarget("id_LoginLabel")]
	private TextMeshProUGUI loginLabel;

	// Token: 0x04001934 RID: 6452
	[UIFieldTarget("btn_Register")]
	private BSGButton btnRegister;

	// Token: 0x04001935 RID: 6453
	[UIFieldTarget("id_RegisterLabel")]
	private TextMeshProUGUI registerLabel;

	// Token: 0x04001936 RID: 6454
	[UIFieldTarget("btn_ForgotPassword")]
	private BSGButton btnForgotPassword;

	// Token: 0x04001937 RID: 6455
	[UIFieldTarget("id_ForgotPasswordLabel")]
	private TextMeshProUGUI forgotPasswordLabel;

	// Token: 0x04001938 RID: 6456
	[UIFieldTarget("id_StatusMessage")]
	public TextMeshProUGUI statusMessage;

	// Token: 0x04001939 RID: 6457
	private EventSystem eventSystem;

	// Token: 0x0400193A RID: 6458
	private List<TMP_InputField> inputs = new List<TMP_InputField>();

	// Token: 0x0400193B RID: 6459
	private GameObject selectedInput;

	// Token: 0x0400193C RID: 6460
	private int inputSelectionIndex = 1;

	// Token: 0x0400193D RID: 6461
	public Action<string, string, string> onUserChoice;
}
