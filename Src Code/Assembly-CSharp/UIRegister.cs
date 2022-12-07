using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200025F RID: 607
public class UIRegister : UIWindow, IListener
{
	// Token: 0x06002552 RID: 9554 RVA: 0x0014BBBE File Offset: 0x00149DBE
	public override string GetDefId()
	{
		return UIRegister.wnd_def_id;
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x0014BBC8 File Offset: 0x00149DC8
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnRegister != null)
		{
			this.btnRegister.onClick = new BSGButton.OnClick(this.OnRegister);
		}
		if (this.btnBack != null)
		{
			this.btnBack.onClick = new BSGButton.OnClick(this.OnBack);
		}
		if (this.hiddenEye != null)
		{
			this.hiddenEye.onClick = new BSGButton.OnClick(this.OnTogglePasswordVisibility);
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

	// Token: 0x06002554 RID: 9556 RVA: 0x0014BCA0 File Offset: 0x00149EA0
	private void OnEnable()
	{
		this.Init();
		this.eventSystem = EventSystem.current;
		UIText.SetText(this.statusMessage, string.Empty);
		if (this.captionLabel != null)
		{
			UIText.SetTextKey(this.captionLabel, UIRegister.wnd_def_id + ".caption", null, null);
		}
		if (this.disclaimer != null)
		{
			UIText.SetTextKey(this.disclaimer, UIRegister.wnd_def_id + ".disclaimer", null, null);
		}
		if (this.emailLabel != null)
		{
			UIText.SetTextKey(this.emailLabel, UIRegister.wnd_def_id + ".email", null, null);
		}
		if (this.emailPlaceholder != null)
		{
			UIText.SetTextKey(this.emailPlaceholder, UIRegister.wnd_def_id + ".email_prompt", null, null);
		}
		if (this.passwordLabel != null)
		{
			UIText.SetTextKey(this.passwordLabel, UIRegister.wnd_def_id + ".password", null, null);
		}
		if (this.passwordPlaceholder != null)
		{
			UIText.SetTextKey(this.passwordPlaceholder, UIRegister.wnd_def_id + ".password_prompt", null, null);
		}
		if (this.registerLabel != null)
		{
			UIText.SetTextKey(this.registerLabel, UIRegister.wnd_def_id + ".register", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIRegister.wnd_def_id + ".back", null, null);
		}
		if (this.emailInput != null)
		{
			if (string.IsNullOrEmpty(this.email))
			{
				this.email = string.Empty;
			}
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

	// Token: 0x06002555 RID: 9557 RVA: 0x0014BE8C File Offset: 0x0014A08C
	private void OnDisable()
	{
		UIText.SetText(this.statusMessage, string.Empty);
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x0014BEA0 File Offset: 0x0014A0A0
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
			if (this.selectedInput != null && this.eventSystem != null)
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
			this.Register();
		}
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x0014BF98 File Offset: 0x0014A198
	private void Register()
	{
		if (this.emailInput == null || this.passwordInput == null)
		{
			Debug.LogWarning("Input is missing!");
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
		action("register", this.emailInput.text, this.passwordInput.text);
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x0014C000 File Offset: 0x0014A200
	private void Back()
	{
		Action<string, string, string> action = this.onUserChoice;
		if (action != null)
		{
			action("back", this.emailInput.text, this.passwordInput.text);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x0014C03A File Offset: 0x0014A23A
	public override bool OnBackInputAction()
	{
		base.OnBackInputAction();
		this.Back();
		return true;
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x0014C04A File Offset: 0x0014A24A
	private void OnRegister(BSGButton btn)
	{
		this.Register();
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x0014C054 File Offset: 0x0014A254
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

	// Token: 0x0600255D RID: 9565 RVA: 0x0014C0BA File Offset: 0x0014A2BA
	private void OnBack(BSGButton btn)
	{
		this.Back();
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x0014C0C4 File Offset: 0x0014A2C4
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

	// Token: 0x0600255F RID: 9567 RVA: 0x0014C12C File Offset: 0x0014A32C
	public static string RegisterResultToMessage(Logic.Common.APIResult result)
	{
		if (result == Logic.Common.APIResult.Register_SomeAccountIdentifiersAlreadyPresent)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.email_already_registered", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_PasswordTooShort)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_short", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_PasswordTooLong)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_long", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_PasswordTooWeakUseMoreCharacters || result == Logic.Common.APIResult.Register_PasswordTooWeakUseMoreVariation)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_too_weak", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_PasswordHasInvalidCharacters || result == Logic.Common.APIResult.Register_PasswordHasInvalidASCIICharacters || result == Logic.Common.APIResult.Register_PasswordHasInvalidUTF8Characters)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.password_invalid_characters", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_InvalidPassword)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.invalid_password", null, null, true, true);
		}
		if (result == Logic.Common.APIResult.Register_InvalidEmailAddressProvided)
		{
			return global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.invalid_email", null, null, true, true);
		}
		return string.Format("Error: Cannot parse THQNO result message! Result: {0}", result);
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x001471BC File Offset: 0x001453BC
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

	// Token: 0x06002561 RID: 9569 RVA: 0x0014C202 File Offset: 0x0014A402
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "register_response")
		{
			UIText.SetText(this.statusMessage, (string)param);
		}
	}

	// Token: 0x04001967 RID: 6503
	private static string wnd_def_id = "TitleRegister";

	// Token: 0x04001968 RID: 6504
	[HideInInspector]
	public string email = "";

	// Token: 0x04001969 RID: 6505
	[HideInInspector]
	public string password = "";

	// Token: 0x0400196A RID: 6506
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI captionLabel;

	// Token: 0x0400196B RID: 6507
	[UIFieldTarget("id_Disclaimer")]
	private TextMeshProUGUI disclaimer;

	// Token: 0x0400196C RID: 6508
	[UIFieldTarget("id_StatusMessage")]
	public TextMeshProUGUI statusMessage;

	// Token: 0x0400196D RID: 6509
	[UIFieldTarget("id_EmailLabel")]
	private TextMeshProUGUI emailLabel;

	// Token: 0x0400196E RID: 6510
	[UIFieldTarget("id_EmailInput")]
	private TMP_InputField emailInput;

	// Token: 0x0400196F RID: 6511
	[UIFieldTarget("id_EmailPlaceholder")]
	private TextMeshProUGUI emailPlaceholder;

	// Token: 0x04001970 RID: 6512
	[UIFieldTarget("id_PasswordLabel")]
	private TextMeshProUGUI passwordLabel;

	// Token: 0x04001971 RID: 6513
	[UIFieldTarget("id_PasswordInput")]
	private TMP_InputField passwordInput;

	// Token: 0x04001972 RID: 6514
	[UIFieldTarget("id_PasswordPlaceholder")]
	private TextMeshProUGUI passwordPlaceholder;

	// Token: 0x04001973 RID: 6515
	[UIFieldTarget("id_hiddenEye")]
	private BSGButton hiddenEye;

	// Token: 0x04001974 RID: 6516
	[UIFieldTarget("id_RegisterLabel")]
	private TextMeshProUGUI registerLabel;

	// Token: 0x04001975 RID: 6517
	[UIFieldTarget("btn_Register")]
	private BSGButton btnRegister;

	// Token: 0x04001976 RID: 6518
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x04001977 RID: 6519
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x04001978 RID: 6520
	private EventSystem eventSystem;

	// Token: 0x04001979 RID: 6521
	private List<TMP_InputField> inputs = new List<TMP_InputField>();

	// Token: 0x0400197A RID: 6522
	private GameObject selectedInput;

	// Token: 0x0400197B RID: 6523
	private int inputSelectionIndex;

	// Token: 0x0400197C RID: 6524
	public Action<string, string, string> onUserChoice;
}
