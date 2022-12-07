using System;
using TMPro;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class UIChooseDisplayName : UIWindow
{
	// Token: 0x06002460 RID: 9312 RVA: 0x00146153 File Offset: 0x00144353
	public override string GetDefId()
	{
		return UIChooseDisplayName.wnd_def_id;
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x0014615C File Offset: 0x0014435C
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnChooseDisplayName != null)
		{
			this.btnChooseDisplayName.onClick = new BSGButton.OnClick(this.OnChooseDisplayName);
		}
		if (this.btnBack != null)
		{
			this.btnBack.onClick = new BSGButton.OnClick(this.OnBack);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x001461CA File Offset: 0x001443CA
	protected override void Awake()
	{
		base.Awake();
		this.displayNameInput = Common.FindChildComponent<TMP_InputField>(base.gameObject, "id_DisplayNameInput");
		this.statusMessage = Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_StatusMessage");
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x00146200 File Offset: 0x00144400
	private void OnEnable()
	{
		this.Init();
		UIText.SetText(this.statusMessage, string.Empty);
		if (this.m_CaptionLabel != null)
		{
			UIText.SetTextKey(this.m_CaptionLabel, UIChooseDisplayName.wnd_def_id + ".caption", null, null);
		}
		if (this.m_TextField != null)
		{
			UIText.SetTextKey(this.m_TextField, UIChooseDisplayName.wnd_def_id + ".text_field", null, null);
		}
		if (this.displayNamePlaceholder != null)
		{
			UIText.SetTextKey(this.displayNamePlaceholder, UIChooseDisplayName.wnd_def_id + ".display_name_prompt", null, null);
		}
		if (this.chooseNameLabel != null)
		{
			UIText.SetTextKey(this.chooseNameLabel, UIChooseDisplayName.wnd_def_id + ".choose_name", null, null);
		}
		if (this.backLabel != null)
		{
			UIText.SetTextKey(this.backLabel, UIChooseDisplayName.wnd_def_id + ".back", null, null);
		}
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x001462F8 File Offset: 0x001444F8
	private void OnChooseDisplayName(BSGButton btn)
	{
		if (this.displayNameInput == null)
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
		action("choose_display_name", this.displayNameInput.text, string.Empty);
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x0014634C File Offset: 0x0014454C
	private void OnBack(BSGButton btn)
	{
		Action<string, string, string> action = this.onUserChoice;
		if (action == null)
		{
			return;
		}
		action("back", string.Empty, string.Empty);
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x0014636D File Offset: 0x0014456D
	private bool ValidateInput()
	{
		if (string.IsNullOrEmpty(this.displayNameInput.text))
		{
			UIText.SetText(this.statusMessage, Defs.Localize("TitleScreen.Multiplayer.Register.enter_display_name", null, null, true, true));
			return false;
		}
		return true;
	}

	// Token: 0x040018A3 RID: 6307
	private static string wnd_def_id = "ChooseDisplayName";

	// Token: 0x040018A4 RID: 6308
	[UIFieldTarget("id_CaptionLabel")]
	public TextMeshProUGUI m_CaptionLabel;

	// Token: 0x040018A5 RID: 6309
	[UIFieldTarget("id_TextField")]
	private TextMeshProUGUI m_TextField;

	// Token: 0x040018A6 RID: 6310
	[UIFieldTarget("id_DisplayNameInput")]
	private TMP_InputField displayNameInput;

	// Token: 0x040018A7 RID: 6311
	[UIFieldTarget("id_DisplayNamePlaceholder")]
	private TextMeshProUGUI displayNamePlaceholder;

	// Token: 0x040018A8 RID: 6312
	[UIFieldTarget("btn_ChooseDisplayName")]
	private BSGButton btnChooseDisplayName;

	// Token: 0x040018A9 RID: 6313
	[UIFieldTarget("id_ChooseNameLabel")]
	private TextMeshProUGUI chooseNameLabel;

	// Token: 0x040018AA RID: 6314
	[UIFieldTarget("btn_Back")]
	private BSGButton btnBack;

	// Token: 0x040018AB RID: 6315
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI backLabel;

	// Token: 0x040018AC RID: 6316
	[UIFieldTarget("id_StatusMessage")]
	public TextMeshProUGUI statusMessage;

	// Token: 0x040018AD RID: 6317
	public Action<string, string, string> onUserChoice;
}
