using System;
using TMPro;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class UIDataCollectionPromptWindow : UIWindow
{
	// Token: 0x06002DE7 RID: 11751 RVA: 0x0017CAE2 File Offset: 0x0017ACE2
	public override string GetDefId()
	{
		return UIDataCollectionPromptWindow.def_id;
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x0017CAE9 File Offset: 0x0017ACE9
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.Populate();
		this.m_Initialized = true;
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x0017CB08 File Offset: 0x0017AD08
	private void OnClickAllow(BSGButton button)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("data_collection");
		setting.new_value = true;
		setting.Apply(false);
		this.OnClickSkip(button);
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x0017CB2D File Offset: 0x0017AD2D
	private void OnClickSkip(BSGButton button)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("data_collection_asked");
		setting.new_value = true;
		setting.Apply(false);
		this.Close(false);
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x0017CB54 File Offset: 0x0017AD54
	private void Populate()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "DataCollectionPrompt.caption", null, null);
		}
		if (this.m_Message != null)
		{
			UIText.SetTextKey(this.m_Message, "DataCollectionPrompt.message", null, null);
		}
		if (this.m_AllowLabel != null)
		{
			UIText.SetTextKey(this.m_AllowLabel, "DataCollectionPrompt.allow_button_text", null, null);
		}
		if (this.m_SkipLabel != null)
		{
			UIText.SetTextKey(this.m_SkipLabel, "DataCollectionPrompt.skip_button_text", null, null);
		}
		if (this.m_AllowBtn != null)
		{
			BSGButton allowBtn = this.m_AllowBtn;
			allowBtn.onClick = (BSGButton.OnClick)Delegate.Combine(allowBtn.onClick, new BSGButton.OnClick(this.OnClickAllow));
		}
		if (this.m_SkipBtn != null)
		{
			BSGButton skipBtn = this.m_SkipBtn;
			skipBtn.onClick = (BSGButton.OnClick)Delegate.Combine(skipBtn.onClick, new BSGButton.OnClick(this.OnClickSkip));
		}
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x0017CC4C File Offset: 0x0017AE4C
	public static UIDataCollectionPromptWindow Create(RectTransform parent)
	{
		if (parent == null)
		{
			return null;
		}
		GameObject prefab = UICommon.GetPrefab("DataCollectionPrompt", null);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(prefab, parent);
		UIDataCollectionPromptWindow uidataCollectionPromptWindow = gameObject.GetComponent<UIDataCollectionPromptWindow>();
		if (uidataCollectionPromptWindow == null)
		{
			uidataCollectionPromptWindow = gameObject.AddComponent<UIDataCollectionPromptWindow>();
		}
		uidataCollectionPromptWindow.Init();
		uidataCollectionPromptWindow.Open();
		return uidataCollectionPromptWindow;
	}

	// Token: 0x04001F13 RID: 7955
	private static string def_id = "DataCollectionPrompt";

	// Token: 0x04001F14 RID: 7956
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001F15 RID: 7957
	[UIFieldTarget("id_Message")]
	private TextMeshProUGUI m_Message;

	// Token: 0x04001F16 RID: 7958
	[UIFieldTarget("id_SkipLabel")]
	private TextMeshProUGUI m_SkipLabel;

	// Token: 0x04001F17 RID: 7959
	[UIFieldTarget("id_AllowLabel")]
	private TextMeshProUGUI m_AllowLabel;

	// Token: 0x04001F18 RID: 7960
	[UIFieldTarget("btn_Allow")]
	private BSGButton m_AllowBtn;

	// Token: 0x04001F19 RID: 7961
	[UIFieldTarget("btn_Skip")]
	private BSGButton m_SkipBtn;
}
