using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000249 RID: 585
public class UICampaignSettings : MonoBehaviour, RemoteVars.IListener
{
	// Token: 0x060023A7 RID: 9127 RVA: 0x00141464 File Offset: 0x0013F664
	public void SetData(Campaign campaign)
	{
		using (Game.Profile("UICampaignSettings.SetData", false, 0f, null))
		{
			this.Init();
			this.m_Game = GameLogic.Get(false);
			Campaign campaign2 = this.campaign;
			if (campaign2 != null)
			{
				campaign2.DelVarsListener(this);
			}
			this.campaign = campaign;
			Campaign campaign3 = this.campaign;
			if (campaign3 != null)
			{
				campaign3.AddVarsListener(this);
			}
			this.BuildMainGoal();
			this.BuildStartPeriod();
			this.BuildSettings(true);
			if (this.m_LoadFromSave != null)
			{
				bool flag = this.campaign.IsAuthority();
				Vars vars = new Vars(this.campaign);
				vars.Set<bool>("is_host", flag);
				Tooltip.Get(this.m_LoadFromSave.gameObject, true).SetDef("NewCampaignFromSaveTooltip", vars);
				bool enable = flag && campaign.state == Campaign.State.Created;
				this.m_LoadFromSave.Enable(enable, false);
			}
			if (this.m_ModCompatibility != null)
			{
				this.m_ModCompatibility.SetObject(campaign, null);
			}
		}
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x0014157C File Offset: 0x0013F77C
	private void OnEnable()
	{
		this.m_Invalidate = true;
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x00141588 File Offset: 0x0013F788
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_SettingsRulePrototype != null)
		{
			this.m_SettingsRulePrototype.gameObject.SetActive(false);
		}
		if (this.m_MainGoalPrototype != null)
		{
			this.m_MainGoalPrototype.gameObject.SetActive(false);
		}
		if (this.m_StartingPeriodPrototype != null)
		{
			this.m_StartingPeriodPrototype.gameObject.SetActive(false);
		}
		if (this.m_LoadFromSave != null)
		{
			this.m_LoadFromSave.onClick = new BSGButton.OnClick(this.HandleOnLoadFromSave);
		}
		this.m_Initialzed = true;
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x0014162E File Offset: 0x0013F82E
	private void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.BuildMainGoal();
			this.BuildStartPeriod();
			this.BuildSettings(false);
			this.m_Invalidate = false;
		}
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x00141654 File Offset: 0x0013F854
	private void BuildMainGoal()
	{
		if (this.campaign == null)
		{
			return;
		}
		using (Game.Profile("UICampaignSettings.BuildMainGoal", false, 0f, null))
		{
			DT.Field varsDef = this.campaign.GetVarsDef();
			if (varsDef != null && varsDef.children != null)
			{
				DT.Field field = varsDef.FindChild("main_goal", null, true, true, true, '.');
				if (field != null)
				{
					DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.campaign, field);
					if (selectedOption != null)
					{
						UIText.SetText(this.m_GameMode, global::Defs.Localize(selectedOption, "name", this.campaign, null, true, true));
						UIText.SetText(this.m_GameModelDescription, global::Defs.Localize(selectedOption, "description", this.campaign, null, true, true));
						UIText.SetText(this.m_VictoryCondition, global::Defs.Localize(selectedOption, "victory_condition", this.campaign, null, true, true));
					}
					UICommon.DeleteChildren(this.m_MainGoalContaner);
					if (this.m_GameModeIconContainer != null)
					{
						this.m_GameModeIconContainer.overrideSprite = global::Defs.GetObj<Sprite>(selectedOption, "icon", null);
					}
					this.m_Goal = UICampaignSettings.SettingsRule.Create(this.campaign, field, this.m_MainGoalPrototype, this.m_MainGoalContaner, null);
					if (this.m_SubGoalsContaner != null)
					{
						this.m_SubGoals.Clear();
						UICommon.DeleteChildren(this.m_SubGoalsContaner);
						for (int i = 0; i < varsDef.children.Count; i++)
						{
							DT.Field field2 = varsDef.children[i];
							if (!string.IsNullOrEmpty(field2.key))
							{
								DT.Field field3 = field2.FindChild("sub_rule", null, true, true, true, '.');
								if (field3 != null && field3.Bool(null, false) && this.campaign.CheckCondition(field2.key))
								{
									UICampaignSettings.SettingsRule settingsRule = UICampaignSettings.SettingsRule.Create(this.campaign, field2, this.m_SettingsRulePrototype, this.m_SubGoalsContaner, null);
									if (settingsRule != null)
									{
										this.m_SubGoals.Add(settingsRule);
									}
								}
							}
						}
					}
					if (this.m_MainGoalLock != null)
					{
						bool active = this.campaign.IsMultiplayerCampaign() && this.campaign.state >= Campaign.State.Started;
						this.m_MainGoalLock.gameObject.SetActive(active);
					}
				}
			}
		}
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x001418AC File Offset: 0x0013FAAC
	private void BuildStartPeriod()
	{
		if (this.campaign == null)
		{
			return;
		}
		DT.Field varsDef = this.campaign.GetVarsDef();
		if (varsDef == null || varsDef.children == null)
		{
			return;
		}
		DT.Field field = varsDef.FindChild("start_period", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		CampaignUtils.GetSelectedOption(this.campaign, field);
		UICommon.DeleteChildren(this.m_StartingPeriodContainer);
		this.m_StarPeriodRule = UICampaignSettings.SettingsRule.Create(this.campaign, field, this.m_StartingPeriodPrototype, this.m_StartingPeriodContainer, new Action<UICampaignSettings.SettingsRule>(this.HandleOnStartingPeriodRuleChnage));
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x00141934 File Offset: 0x0013FB34
	private void BuildSettings(bool initial)
	{
		if (this.campaign == null)
		{
			return;
		}
		using (Game.Profile("UICampaignSettings.BuildSettings", false, 0f, null))
		{
			DT.Field varsDef = this.campaign.GetVarsDef();
			if (varsDef != null && varsDef.children != null)
			{
				UICommon.DeleteChildren(this.m_SettingsContainer);
				this.m_Rules.Clear();
				for (int i = 0; i < varsDef.children.Count; i++)
				{
					DT.Field field = varsDef.children[i];
					if (!string.IsNullOrEmpty(field.key) && !(field.key == "start_period") && !(field.key == "main_goal"))
					{
						DT.Field varOptions = this.campaign.GetVarOptions(field.key);
						if (varOptions != null && varOptions.children != null && field.GetBool("show_in_rules", this.campaign, true, true, true, true, '.'))
						{
							DT.Field field2 = field.FindChild("sub_rule", null, true, true, true, '.');
							if (field2 == null || !field2.Bool(null, false))
							{
								UICampaignSettings.SettingsRule settingsRule = UICampaignSettings.SettingsRule.Create(this.campaign, field, this.m_SettingsRulePrototype, this.m_SettingsContainer, null);
								if (settingsRule != null)
								{
									this.m_Rules.Add(settingsRule);
									if (field.Key(false) == "ai_difficulty" && (!this.campaign.IsMultiplayerCampaign() && initial))
									{
										UserSettings.SettingData setting = UserSettings.GetSetting("difficulty");
										List<Value> options = setting.GetOptions();
										for (int j = 0; j < options.Count; j++)
										{
											if (options[j] == setting.value)
											{
												settingsRule.HandleOnValueChange(j);
											}
										}
									}
								}
							}
						}
					}
				}
				if (this.m_RulesLock != null)
				{
					bool active = this.campaign.IsMultiplayerCampaign() && this.campaign.state >= Campaign.State.Started;
					this.m_RulesLock.gameObject.SetActive(active);
				}
			}
		}
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x00141B6C File Offset: 0x0013FD6C
	private void HandleOnLoadFromSave(BSGButton b)
	{
		UILoadGameWindow uiloadGameWindow = UILoadGameWindow.Create(BaseUI.Get().message_container.transform, true);
		if (uiloadGameWindow != null)
		{
			uiloadGameWindow.SetLocalPosition(new Vector2(2f, 178f));
		}
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x0014157C File Offset: 0x0013F77C
	private void HandleOnStartingPeriodRuleChnage(UICampaignSettings.SettingsRule r)
	{
		this.m_Invalidate = true;
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x00141BB0 File Offset: 0x0013FDB0
	public static void LocalNotifySettingChanged(UICampaignSettings.SettingsRule rule, Value old_val, Value new_val)
	{
		if (rule == null)
		{
			return;
		}
		Campaign campaign = rule.Campaign;
		if (campaign == null)
		{
			return;
		}
		if (old_val == new_val)
		{
			return;
		}
		if (campaign.IsAuthority())
		{
			return;
		}
		if (!campaign.IsLocalPlayerReady())
		{
			return;
		}
		DT.Field val = null;
		DT.Field val2 = null;
		DT.Field varOptions = campaign.GetVarOptions(rule.Rule.key);
		for (int i = 0; i < varOptions.children.Count; i++)
		{
			DT.Field field = varOptions.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				Value optionValue = campaign.GetOptionValue(field);
				if (optionValue.is_valid)
				{
					if (optionValue == old_val)
					{
						val = field;
					}
					if (optionValue == new_val)
					{
						val2 = field;
					}
				}
			}
		}
		Vars vars = new Vars();
		vars.Set<DT.Field>("rule", rule.Rule);
		vars.Set<DT.Field>("old_val", val);
		vars.Set<DT.Field>("new_val", val2);
		vars.Set<string>("message", global::Defs.Localize("TitleChat.setting_changed", vars, null, true, true));
		UILobbyChat.AddChatMessage(campaign, vars);
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x00141CC0 File Offset: 0x0013FEC0
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		UICampaignSettings.SettingsRule settingRule = this.GetSettingRule(key);
		this.m_Invalidate |= (settingRule != null);
		UICampaignSettings.LocalNotifySettingChanged(settingRule, old_val, new_val);
		if (key == "start_period" || key == "start_countdown" || key == "team_size")
		{
			this.RefreshSettings();
			return;
		}
		if (!(key == "from_save_id"))
		{
			return;
		}
		UICampaignSettings.SettingsRule starPeriodRule = this.m_StarPeriodRule;
		if (starPeriodRule != null)
		{
			starPeriodRule.InvalidatePatches();
		}
		this.m_Invalidate = true;
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x00141D48 File Offset: 0x0013FF48
	private void RefreshSettings()
	{
		if (this.m_Goal != null)
		{
			this.m_Goal.Refresh();
		}
		for (int i = 0; i < this.m_Rules.Count; i++)
		{
			this.m_Rules[i].Refresh();
		}
		for (int j = 0; j < this.m_SubGoals.Count; j++)
		{
			this.m_SubGoals[j].Refresh();
		}
		if (this.m_StarPeriodRule != null)
		{
			this.m_StarPeriodRule.Refresh();
		}
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x00141DD8 File Offset: 0x0013FFD8
	private UICampaignSettings.SettingsRule GetSettingRule(string key)
	{
		if (this.m_Goal != null && this.m_Goal.Rule.key == key)
		{
			return this.m_Goal;
		}
		for (int i = 0; i < this.m_Rules.Count; i++)
		{
			if (this.m_Rules[i].Rule.key == key)
			{
				return this.m_Rules[i];
			}
		}
		for (int j = 0; j < this.m_SubGoals.Count; j++)
		{
			if (this.m_SubGoals[j].Rule.key == key)
			{
				return this.m_SubGoals[j];
			}
		}
		return null;
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x00141E98 File Offset: 0x00140098
	public static DT.Field GetDefaultOrValidOption(Campaign Campaign, DT.Field Rule)
	{
		Value defaultValue = Campaign.GetDefaultValue(Rule.key);
		DT.Field varOptions = Campaign.GetVarOptions(Rule.key);
		for (int i = 0; i < varOptions.children.Count; i++)
		{
			DT.Field field = varOptions.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				if (defaultValue.is_unknown)
				{
					return field;
				}
				Value optionValue = Campaign.GetOptionValue(field);
				if (optionValue.is_valid && optionValue == defaultValue)
				{
					return field;
				}
			}
		}
		return null;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x00141F1C File Offset: 0x0014011C
	public static DT.Field GetSelectedOption(Campaign Campaign, DT.Field Rule)
	{
		if (Campaign == null)
		{
			return null;
		}
		if (Campaign.campaignData == null)
		{
			return null;
		}
		if (Rule == null)
		{
			return null;
		}
		Value var = Campaign.campaignData.GetVar(Rule.key, null, true);
		DT.Field varOptions = Campaign.GetVarOptions(Rule.key);
		for (int i = 0; i < varOptions.children.Count; i++)
		{
			DT.Field field = varOptions.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				Value optionValue = Campaign.GetOptionValue(field);
				if (optionValue.is_valid && optionValue == var)
				{
					return field;
				}
			}
		}
		return null;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x00141FB0 File Offset: 0x001401B0
	public static Value GetNextOption(Campaign campaign, RemoteVars vars, string rule_key)
	{
		Value var = vars.GetVar(rule_key, null, true);
		if (vars == campaign.campaignData)
		{
			DT.Field varOptions = campaign.GetVarOptions(rule_key);
			if (varOptions != null && varOptions.children != null)
			{
				List<Value> list = new List<Value>();
				int num = -1;
				for (int i = 0; i < varOptions.children.Count; i++)
				{
					DT.Field field = varOptions.children[i];
					if (!string.IsNullOrEmpty(field.key))
					{
						Value optionValue = campaign.GetOptionValue(field);
						if (optionValue.is_valid)
						{
							if (optionValue == var)
							{
								num = list.Count;
							}
							list.Add(optionValue);
						}
					}
				}
				if (list.Count > 0)
				{
					return list[(num + 1) % list.Count];
				}
			}
		}
		return Value.Unknown;
	}

	// Token: 0x040017F2 RID: 6130
	[UIFieldTarget("id_SettingsRulePrototype")]
	private GameObject m_SettingsRulePrototype;

	// Token: 0x040017F3 RID: 6131
	[UIFieldTarget("id_MainGoalPrototype")]
	private GameObject m_MainGoalPrototype;

	// Token: 0x040017F4 RID: 6132
	[UIFieldTarget("id_MainGoalContaner")]
	private RectTransform m_MainGoalContaner;

	// Token: 0x040017F5 RID: 6133
	[UIFieldTarget("id_SubGoalsContaner")]
	private RectTransform m_SubGoalsContaner;

	// Token: 0x040017F6 RID: 6134
	[UIFieldTarget("id_SettingsContainer")]
	private RectTransform m_SettingsContainer;

	// Token: 0x040017F7 RID: 6135
	[UIFieldTarget("id_GameMode")]
	private TMP_Text m_GameMode;

	// Token: 0x040017F8 RID: 6136
	[UIFieldTarget("id_GameModelDescription")]
	private TMP_Text m_GameModelDescription;

	// Token: 0x040017F9 RID: 6137
	[UIFieldTarget("id_VictoryCondition")]
	private TMP_Text m_VictoryCondition;

	// Token: 0x040017FA RID: 6138
	[UIFieldTarget("id_GameModeIconContainer")]
	private Image m_GameModeIconContainer;

	// Token: 0x040017FB RID: 6139
	[UIFieldTarget("id_MainGoalLock")]
	private RectTransform m_MainGoalLock;

	// Token: 0x040017FC RID: 6140
	[UIFieldTarget("id_RulesLock")]
	private RectTransform m_RulesLock;

	// Token: 0x040017FD RID: 6141
	[UIFieldTarget("id_StartingPeriodContainer")]
	private RectTransform m_StartingPeriodContainer;

	// Token: 0x040017FE RID: 6142
	[UIFieldTarget("id_StartingPeriodPrototype")]
	private GameObject m_StartingPeriodPrototype;

	// Token: 0x040017FF RID: 6143
	[UIFieldTarget("id_LoadFromSave")]
	private BSGButton m_LoadFromSave;

	// Token: 0x04001800 RID: 6144
	[UIFieldTarget("id_ModCompatibility")]
	private UIModCompatibilityIcon m_ModCompatibility;

	// Token: 0x04001801 RID: 6145
	public Campaign campaign;

	// Token: 0x04001802 RID: 6146
	private Game m_Game;

	// Token: 0x04001803 RID: 6147
	private UICampaignSettings.SettingsRule m_Goal;

	// Token: 0x04001804 RID: 6148
	private UICampaignSettings.SettingsRule m_StarPeriodRule;

	// Token: 0x04001805 RID: 6149
	private List<UICampaignSettings.SettingsRule> m_SubGoals = new List<UICampaignSettings.SettingsRule>();

	// Token: 0x04001806 RID: 6150
	private List<UICampaignSettings.SettingsRule> m_Rules = new List<UICampaignSettings.SettingsRule>();

	// Token: 0x04001807 RID: 6151
	private bool m_Initialzed;

	// Token: 0x04001808 RID: 6152
	private bool m_Invalidate;

	// Token: 0x020007A1 RID: 1953
	public class SettingsRule : MonoBehaviour
	{
		// Token: 0x06004CC4 RID: 19652 RVA: 0x0022A8CC File Offset: 0x00228ACC
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Dropdown.onValueChanged.AddListener(new UnityAction<int>(this.HandleOnValueChange));
			BSGDropdown dropdown = this.m_Dropdown;
			dropdown.OnShow = (Action)Delegate.Combine(dropdown.OnShow, new Action(this.HandleOnShow));
			this.m_Initialized = true;
		}

		// Token: 0x06004CC5 RID: 19653 RVA: 0x0022A933 File Offset: 0x00228B33
		public void SetData(Campaign campaign, DT.Field ruleField, Action<UICampaignSettings.SettingsRule> onSelectCallBack)
		{
			this.Init();
			this.Campaign = campaign;
			this.Rule = ruleField;
			this.OnSelect = onSelectCallBack;
			this.Refresh();
		}

		// Token: 0x06004CC6 RID: 19654 RVA: 0x0022A958 File Offset: 0x00228B58
		public void Refresh()
		{
			if (this.m_RuleLabel != null)
			{
				UIText.SetText(this.m_RuleLabel, global::Defs.Localize(this.Rule, "name", null, null, true, true));
				Vars vars = new Vars(this.Rule);
				Tooltip tooltip = Tooltip.Get(this.m_RuleLabel.gameObject, true);
				tooltip.SetDef("GameSettingTooltip", vars);
				tooltip.pinable = 0;
			}
			if (this.m_Dropdown != null)
			{
				this.m_Dropdown.interactable = this.CanChangeRule();
			}
			this.BuildOptions(this.Campaign, this.Rule);
			this.UpdateSelectedRuleTooltip();
		}

		// Token: 0x06004CC7 RID: 19655 RVA: 0x0022A9F8 File Offset: 0x00228BF8
		private void BuildOptions(Campaign campaign, DT.Field rule)
		{
			DT.Field varOptions = campaign.GetVarOptions(rule.key);
			if (varOptions == null || varOptions.children == null)
			{
				return;
			}
			this.m_Dropdown.options.Clear();
			this.m_Options.Clear();
			for (int i = 0; i < varOptions.children.Count; i++)
			{
				DT.Field field = varOptions.children[i];
				if (!string.IsNullOrEmpty(field.key) && !field.GetBool("hidden", null, false, true, true, true, '.') && campaign.CheckOptionCondition(field) && this.IsOptionEligbaleForCampaign(field, rule, campaign))
				{
					this.m_Options.Add(field);
					string text = global::Defs.Localize(field, "name", null, null, true, true);
					this.m_Dropdown.options.Add(new TMP_Dropdown.OptionData(text));
				}
			}
			int num = this.GetSelectedOptionsIndex();
			if (campaign.IsFromSave() && rule.key == "start_period")
			{
				string text2 = global::Defs.Localize("TitleScreen.Multiplayer.GameBrowser.starting_period_custom", null, null, true, true);
				this.m_Dropdown.options.Add(new TMP_Dropdown.OptionData(text2));
				num = this.m_Options.Count;
			}
			if (num == -1)
			{
				num = 0;
			}
			this.m_Dropdown.SetValueWithoutNotify(num);
			this.m_Dropdown.RefreshShownValue();
			this.m_ApplyPatches = true;
		}

		// Token: 0x06004CC8 RID: 19656 RVA: 0x0022AB40 File Offset: 0x00228D40
		private bool IsOptionEligbaleForCampaign(DT.Field option, DT.Field rule, Campaign campaign)
		{
			if (option == null || rule == null || campaign == null)
			{
				return true;
			}
			if (!campaign.IsFromSave())
			{
				return true;
			}
			if (rule.key == "main_goal")
			{
				DT.Field selectedOption = CampaignUtils.GetSelectedOption(campaign, rule);
				return selectedOption == null || option == selectedOption || option.key == "None";
			}
			return true;
		}

		// Token: 0x06004CC9 RID: 19657 RVA: 0x0022AB9E File Offset: 0x00228D9E
		public void InvalidatePatches()
		{
			this.m_ApplyPatches = true;
		}

		// Token: 0x06004CCA RID: 19658 RVA: 0x0022ABA7 File Offset: 0x00228DA7
		private void LateUpdate()
		{
			if (this.m_ApplyPatches)
			{
				this.PatchLabelForCustomStartingPeriod();
				this.m_ApplyPatches = false;
			}
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x0022ABBE File Offset: 0x00228DBE
		private void PatchLabelForCustomStartingPeriod()
		{
			if (this.Rule.key != "start_period")
			{
				return;
			}
			if (!this.Campaign.IsFromSave())
			{
				return;
			}
			UIText.SetTextKey(this.m_Dropdown.captionText, "TitleScreen.Multiplayer.GameBrowser.starting_period_custom", null, null);
		}

		// Token: 0x06004CCC RID: 19660 RVA: 0x0022AC00 File Offset: 0x00228E00
		private int GetSelectedOptionsIndex()
		{
			DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.Campaign, this.Rule);
			int num = -1;
			if (selectedOption != null && this.Campaign.CheckOptionCondition(selectedOption))
			{
				num = this.m_Options.IndexOf(selectedOption);
			}
			if (num == -1 && CampaignUtils.GetDefaultOrValidOption(this.Campaign, this.Rule) != null)
			{
				num = this.m_Options.IndexOf(selectedOption);
			}
			return num;
		}

		// Token: 0x06004CCD RID: 19661 RVA: 0x0022AC64 File Offset: 0x00228E64
		private void PopulateOptionsTooltips()
		{
			GameObject gameObject = global::Common.FindChildByName(this.m_Dropdown.gameObject, "Dropdown List", true, true);
			if (gameObject == null)
			{
				return;
			}
			ScrollRect component = gameObject.GetComponent<ScrollRect>();
			if (component == null)
			{
				return;
			}
			bool val = this.IsLocked();
			bool val2 = this.Campaign.IsAuthority();
			int num = 1;
			while (num < component.content.transform.childCount && this.m_Options.Count >= num)
			{
				Vars vars = new Vars(this.m_Options[num - 1]);
				vars.Set<bool>("is_locked", val);
				vars.Set<bool>("is_host", val2);
				Tooltip.Get(component.content.transform.GetChild(num).gameObject, true).SetDef("GameSettingOptionTooltip", vars);
				num++;
			}
		}

		// Token: 0x06004CCE RID: 19662 RVA: 0x0022AD44 File Offset: 0x00228F44
		private void UpdateSelectedRuleTooltip()
		{
			if (this.m_Dropdown != null)
			{
				int num = this.GetSelectedOptionsIndex();
				if (num == -1)
				{
					num = 0;
				}
				if (this.m_Options != null && this.m_Options.Count > 0)
				{
					Vars vars = new Vars(this.m_Options[num]);
					vars.Set<bool>("is_locked", this.IsLocked());
					vars.Set<bool>("is_host", this.Campaign.IsAuthority());
					Tooltip.Get(this.m_Dropdown.gameObject, true).SetDef("GameSettingOptionTooltip", vars);
				}
			}
		}

		// Token: 0x06004CCF RID: 19663 RVA: 0x0022ADD7 File Offset: 0x00228FD7
		private void HandleOnShow()
		{
			this.PopulateOptionsTooltips();
		}

		// Token: 0x06004CD0 RID: 19664 RVA: 0x0022ADDF File Offset: 0x00228FDF
		public void HandleOnValueChange(int v)
		{
			if (v < 0 || v >= this.m_Options.Count)
			{
				return;
			}
			this.ApplyRuleOption(this.m_Options[v]);
			Action<UICampaignSettings.SettingsRule> onSelect = this.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		// Token: 0x06004CD1 RID: 19665 RVA: 0x0022AE18 File Offset: 0x00229018
		private bool IsLocked()
		{
			if (this.Campaign.IsFromSave())
			{
				return this.Campaign.IsVarChangeableAfterCopy(this.Rule);
			}
			return this.Campaign.state >= Campaign.State.Started && this.Campaign.IsVarChangeableAfterStart(this.Rule);
		}

		// Token: 0x06004CD2 RID: 19666 RVA: 0x0022AE68 File Offset: 0x00229068
		private bool CanChangeRule()
		{
			if (this.Rule == null)
			{
				return false;
			}
			if (!this.Campaign.IsMultiplayerCampaign())
			{
				return true;
			}
			if (!this.Campaign.IsAuthority())
			{
				return false;
			}
			if (this.Campaign.state >= Campaign.State.Started)
			{
				return this.Campaign.IsVarChangeableAfterStart(this.Rule);
			}
			if (this.Campaign.IsFromSave())
			{
				return this.Campaign.IsVarChangeableAfterCopy(this.Rule);
			}
			return !this.Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, this.Campaign.GetLocalPlayerID(), "start_countdown");
		}

		// Token: 0x06004CD3 RID: 19667 RVA: 0x0022AF04 File Offset: 0x00229104
		public void ApplyRuleOption(DT.Field option)
		{
			string key = this.Rule.key;
			Value optionValue = this.Campaign.GetOptionValue(option);
			this.Campaign.campaignData.Set(key, optionValue, true);
			this.UpdateSelectedRuleTooltip();
			if (this.Campaign.IsFromSave() && this.Rule.key == "start_period")
			{
				this.Campaign.SetFromSave(null);
			}
		}

		// Token: 0x06004CD4 RID: 19668 RVA: 0x0022AF74 File Offset: 0x00229174
		public static UICampaignSettings.SettingsRule Create(Campaign campaign, DT.Field field, GameObject prototype, RectTransform parent, Action<UICampaignSettings.SettingsRule> onSelectCallBack = null)
		{
			if (field == null)
			{
				return null;
			}
			if (campaign == null)
			{
				return null;
			}
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent, false);
			UICampaignSettings.SettingsRule settingsRule = gameObject.AddComponent<UICampaignSettings.SettingsRule>();
			settingsRule.SetData(campaign, field, onSelectCallBack);
			gameObject.SetActive(true);
			return settingsRule;
		}

		// Token: 0x06004CD5 RID: 19669 RVA: 0x0022AFC1 File Offset: 0x002291C1
		public static UICampaignSettings.SettingsRule Possess(GameObject host, Campaign campaign, DT.Field field, Action<UICampaignSettings.SettingsRule> onSelectCallBack = null)
		{
			if (host == null)
			{
				return null;
			}
			if (field == null)
			{
				return null;
			}
			if (campaign == null)
			{
				return null;
			}
			UICampaignSettings.SettingsRule orAddComponent = host.GetOrAddComponent<UICampaignSettings.SettingsRule>();
			orAddComponent.SetData(campaign, field, onSelectCallBack);
			host.SetActive(true);
			return orAddComponent;
		}

		// Token: 0x04003B6A RID: 15210
		[UIFieldTarget("id_Dropdown")]
		private BSGDropdown m_Dropdown;

		// Token: 0x04003B6B RID: 15211
		[UIFieldTarget("id_RuleLabel")]
		private TextMeshProUGUI m_RuleLabel;

		// Token: 0x04003B6C RID: 15212
		public Action<UICampaignSettings.SettingsRule> OnSelect;

		// Token: 0x04003B6D RID: 15213
		public DT.Field Rule;

		// Token: 0x04003B6E RID: 15214
		public Campaign Campaign;

		// Token: 0x04003B6F RID: 15215
		private bool m_Initialized;

		// Token: 0x04003B70 RID: 15216
		private bool m_ApplyPatches;

		// Token: 0x04003B71 RID: 15217
		private List<DT.Field> m_Options = new List<DT.Field>();
	}
}
