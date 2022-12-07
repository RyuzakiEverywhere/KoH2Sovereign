using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000273 RID: 627
public class UIPreferences : UIWindow
{
	// Token: 0x0600264F RID: 9807 RVA: 0x00150769 File Offset: 0x0014E969
	public override string GetDefId()
	{
		return UIPreferences.def_id;
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x00150770 File Offset: 0x0014E970
	public static Sprite GetLanguageFlag(string languageKey)
	{
		DT.Field defField = global::Defs.GetDefField("Preferences", null);
		return global::Defs.GetObj<Sprite>((defField != null) ? defField.FindChild("language_flags", null, true, true, true, '.') : null, languageKey, null);
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x0015079C File Offset: 0x0014E99C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_CategoryPrototype != null)
		{
			this.m_CategoryPrototype.SetActive(false);
		}
		if (this.m_LanguageCategoryPrototype != null)
		{
			this.m_LanguageCategoryPrototype.SetActive(false);
		}
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnCloseButton);
		}
		if (this.m_ApplyChanges != null)
		{
			this.m_ApplyChanges.onClick = new BSGButton.OnClick(this.HandleOnApplyChangesButton);
		}
		if (this.m_RevertDefaults != null)
		{
			this.m_RevertDefaults.onClick = new BSGButton.OnClick(this.HandleOnApplyDefaultsButton);
		}
		UICommon.DeleteChildren(this.m_CategoryContent);
		this.m_Initialzied = true;
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x00150870 File Offset: 0x0014EA70
	private void LocalizedStatic()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "Preferences.caption", null, null);
		}
		if (this.m_ApplyChangesLabel != null)
		{
			UIText.SetTextKey(this.m_ApplyChangesLabel, "Preferences.apply_chnages", null, null);
		}
		if (this.m_RevertDefaultsLabel != null)
		{
			UIText.SetTextKey(this.m_RevertDefaultsLabel, "Preferences.apply_default_settings", null, null);
		}
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x001508DD File Offset: 0x0014EADD
	public void OnEnable()
	{
		this.Init();
		this.preferencesDef = global::Defs.GetDefField("Preferences", null);
		this.LocalizedStatic();
		this.BuildLayout();
		UserSettings.analyticsSettingsData.Clear();
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x0015090C File Offset: 0x0014EB0C
	public void OnDisable()
	{
		foreach (KeyValuePair<string, UserSettings.SettingChangeData> keyValuePair in UserSettings.analyticsSettingsData)
		{
			string key = keyValuePair.Key;
			UserSettings.SettingChangeData value = keyValuePair.Value;
			string old_value = value.old_value.ToString();
			value = keyValuePair.Value;
			UserSettings.RecordSettingAnalyticsImmediately(key, old_value, value.new_value.ToString());
		}
		UserSettings.analyticsSettingsData.Clear();
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x001509A4 File Offset: 0x0014EBA4
	public override void Open()
	{
		base.Open();
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x001509B8 File Offset: 0x0014EBB8
	public override void Close(bool silent = false)
	{
		if (!base.IsShown())
		{
			return;
		}
		base.Hide(silent);
		this.RevertChanges();
		InGameMenu current = InGameMenu.current;
		if (current)
		{
			current.OnPreferencesClosed();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x001509FC File Offset: 0x0014EBFC
	public void BuildCategoriesLayout<T>(string type, RectTransform container, GameObject prototype, List<UIPreferences.Category> categories) where T : UIPreferences.Category
	{
		DT.Field field = this.preferencesDef;
		List<DT.Field> list = (field != null) ? field.Children() : null;
		this.tmp_Eligable.Clear();
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field2 = list[i];
				if (!(field2.type != type))
				{
					this.tmp_Eligable.Add(field2);
				}
			}
		}
		while (categories.Count < this.tmp_Eligable.Count)
		{
			UIPreferences.Category category = UIPreferences.Category.Create<T>(null, prototype, container);
			category.OnSelect = new Action<UIPreferences.Category>(this.HandleOnSelectCategory);
			categories.Add(category);
		}
		for (int j = 0; j < categories.Count; j++)
		{
			UIPreferences.Category category2 = categories[j];
			DT.Field field3 = (this.tmp_Eligable != null && this.tmp_Eligable.Count > j) ? this.tmp_Eligable[j] : null;
			category2.SetDefField(field3);
			category2.gameObject.SetActive(field3 != null);
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x00150B00 File Offset: 0x0014ED00
	public void BuildLayout()
	{
		UIPreferences.<>c__DisplayClass31_0 CS$<>8__locals1 = new UIPreferences.<>c__DisplayClass31_0();
		UIPreferences.<>c__DisplayClass31_0 CS$<>8__locals2 = CS$<>8__locals1;
		UIPreferences.Category selectedCategory = this.m_SelectedCategory;
		CS$<>8__locals2.lastSelectedDef = ((selectedCategory != null) ? selectedCategory.def : null);
		this.BuildCategoriesLayout<UIPreferences.Category>("category", this.m_CategoriesContainer, this.m_CategoryPrototype, this.m_RootCategories);
		this.BuildCategoriesLayout<UIPreferences.LanguageCategory>("language_category", this.m_LanguageCategoriesContainer, this.m_LanguageCategoryPrototype, this.m_LanguageCategories);
		this.BuildCategoriesLayout<UIPreferences.ModCategory>("mod_category", this.m_CategoriesContainer, this.m_CategoryPrototype, this.m_ModCategories);
		UIPreferences.Category category = null;
		if (CS$<>8__locals1.lastSelectedDef != null)
		{
			category = this.m_RootCategories.Find((UIPreferences.Category c) => c.def == CS$<>8__locals1.lastSelectedDef);
			if (category == null)
			{
				category = this.m_LanguageCategories.Find((UIPreferences.Category c) => c.def == CS$<>8__locals1.lastSelectedDef);
			}
		}
		if (category == null)
		{
			if (this.m_RootCategories.Count > 0)
			{
				category = this.m_RootCategories[0];
			}
			else if (this.m_LanguageCategories.Count > 0)
			{
				category = this.m_LanguageCategories[0];
			}
		}
		this.m_SelectedCategory = null;
		this.SelectCategory(category, false);
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x00150C15 File Offset: 0x0014EE15
	private void HandleOnSelectCategory(UIPreferences.Category c)
	{
		this.SelectCategory(c, true);
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x00150C20 File Offset: 0x0014EE20
	public bool OnApplyChnagesMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.ApplySettings(this.m_ActivePreferences);
		}
		else
		{
			this.RevertChanges();
			for (int i = 0; i < this.m_ActivePreferences.Count; i++)
			{
				UIPreferences.Preference preference = this.m_ActivePreferences[i];
				if (((preference != null) ? preference.setting : null) != null)
				{
					this.m_ActivePreferences[i].RefreshValues();
				}
			}
		}
		wnd.Close(false);
		UIPreferences.Category category = wnd.vars.obj.obj_val as UIPreferences.Category;
		if (category != null)
		{
			this.SelectCategory(category, false);
		}
		return true;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x00150CC0 File Offset: 0x0014EEC0
	public void SelectCategory(UIPreferences.Category c, bool checkForChanges = true)
	{
		if (this.m_SelectedCategory == c)
		{
			return;
		}
		if (checkForChanges && this.HasUnappledChangesInSelectedCategory())
		{
			MessageWnd.Create("ApplyUnsavedSettingsMessage", new Vars(c), null, new MessageWnd.OnButton(this.OnApplyChnagesMessage)).on_update = delegate(MessageWnd w)
			{
				if (!base.isActiveAndEnabled)
				{
					w.Close(false);
				}
			};
			return;
		}
		this.m_SelectedCategory = c;
		for (int i = 0; i < this.m_RootCategories.Count; i++)
		{
			this.m_RootCategories[i].Select(false);
		}
		for (int j = 0; j < this.m_LanguageCategories.Count; j++)
		{
			this.m_LanguageCategories[j].Select(false);
		}
		for (int k = 0; k < this.m_ModCategories.Count; k++)
		{
			this.m_ModCategories[k].Select(false);
		}
		if (this.m_SelectedCategory != null)
		{
			UIPreferences.Category category = this.m_SelectedCategory.GetDefaultSubSelection();
			while (category != null)
			{
				UIPreferences.Category defaultSubSelection = category.GetDefaultSubSelection();
				if (defaultSubSelection == null)
				{
					break;
				}
				category = defaultSubSelection;
			}
			this.m_SelectedCategory = (category ?? this.m_SelectedCategory);
			UIPreferences.Category parentCategory = this.m_SelectedCategory.GetParentCategory();
			while (parentCategory != null)
			{
				parentCategory.Select(true);
				parentCategory = parentCategory.GetParentCategory();
			}
			this.m_SelectedCategory.Select(true);
		}
		this.ShowCategoryPreferences(this.m_SelectedCategory);
		if (this.m_SelectedCategory.def.type == "language_category")
		{
			this.ShowLanguageCategory(this.m_SelectedCategory);
		}
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x00150E4C File Offset: 0x0014F04C
	public void ShowApplyChangesMessage(Action callback)
	{
		UIPreferences.<>c__DisplayClass35_0 CS$<>8__locals1 = new UIPreferences.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.callback = callback;
		if (!this.HasUnappledChangesInSelectedCategory())
		{
			CS$<>8__locals1.callback();
			return;
		}
		MessageWnd.Create("ApplyUnsavedSettingsMessage", new Vars(this.m_SelectedCategory), null, new MessageWnd.OnButton(CS$<>8__locals1.<ShowApplyChangesMessage>g__onMessage|0)).on_update = delegate(MessageWnd w)
		{
			if (!CS$<>8__locals1.<>4__this.isActiveAndEnabled)
			{
				w.Close(false);
			}
		};
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x00150EB4 File Offset: 0x0014F0B4
	private void ShowCategoryPreferences(UIPreferences.Category c)
	{
		this.RevertChanges();
		if (this.m_Confirmation)
		{
			this.m_Confirmation.Close(false);
			this.m_Confirmation = null;
		}
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			global::Common.DestroyObj(this.m_ActivePreferences[i].gameObject);
		}
		this.m_ActivePreferences.Clear();
		List<DT.Field> list = c.def.Children();
		for (int j = 0; j < list.Count; j++)
		{
			DT.Field field = list[j];
			GameObject settingsPrefab = this.GetSettingsPrefab(field.type);
			if (!(settingsPrefab == null))
			{
				UIPreferences.Preference preference = null;
				string type = field.type;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
				if (num <= 1135768689U)
				{
					if (num != 905338595U)
					{
						if (num != 1076453893U)
						{
							if (num == 1135768689U)
							{
								if (type == "button")
								{
									preference = UIPreferences.Preference.Create<UIPreferences.ButtonPreference>(field, settingsPrefab, this.m_CategoryContent);
								}
							}
						}
						else if (type == "toggle")
						{
							preference = UIPreferences.Preference.Create<UIPreferences.TogglePreference>(field, settingsPrefab, this.m_CategoryContent);
						}
					}
					else if (type == "keybind")
					{
						UIPreferences.KeyBindPreference keyBindPreference = UIPreferences.Preference.Create<UIPreferences.KeyBindPreference>(field, settingsPrefab, this.m_CategoryContent);
						if (keyBindPreference != null)
						{
							keyBindPreference.OnBindChange = new Action<KeyBindings.KeyBind>(this.HandleOnKeybindChange);
						}
						preference = keyBindPreference;
					}
				}
				else if (num <= 2459792088U)
				{
					if (num != 1856792572U)
					{
						if (num == 2459792088U)
						{
							if (type == "dropdown")
							{
								preference = UIPreferences.Preference.Create<UIPreferences.DropDownPreference>(field, settingsPrefab, this.m_CategoryContent);
							}
						}
					}
					else if (type == "slider")
					{
						preference = UIPreferences.Preference.Create<UIPreferences.SliderPreference>(field, settingsPrefab, this.m_CategoryContent);
					}
				}
				else if (num != 2517238543U)
				{
					if (num == 4137097213U)
					{
						if (type == "label")
						{
							preference = UIPreferences.Preference.Create<UIPreferences.Label>(field, settingsPrefab, this.m_CategoryContent);
						}
					}
				}
				else if (type == "audio_slider")
				{
					preference = UIPreferences.Preference.Create<UIPreferences.SoundSliderPreference>(field, settingsPrefab, this.m_CategoryContent);
				}
				if (preference != null)
				{
					UIPreferences.Preference preference2 = preference;
					preference2.onApply = (UIPreferences.Preference.OnApply)Delegate.Combine(preference2.onApply, new UIPreferences.Preference.OnApply(this.RefreshEnabledPreferences));
					if (preference.def.FindChild("confirmation", null, true, true, true, '.') != null)
					{
						preference.OnConfirmation = new UIPreferences.Preference.ConfirmationEvent(this.HandleOnShowConfirmation);
					}
					if (preference.def.FindChild("keep_changes_confirmation", null, true, true, true, '.') != null)
					{
						preference.OnKeepChangesConfirmation = new UIPreferences.Preference.ConfirmationEvent(this.HandleOnShowKeepChangesConfirmation);
					}
					preference.applyChangesPopup = this.m_ApplyChangesPopup;
					this.m_ActivePreferences.Add(preference);
				}
			}
		}
		if (c is UIPreferences.ModCategory)
		{
			this.PopulateMods();
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x001511B8 File Offset: 0x0014F3B8
	private void PopulateMods()
	{
		List<UIPreferences.ModPreference.ModData> list = UIPreferences.ModPreference.ModData.BuildModData();
		GameObject settingsPrefab = this.GetSettingsPrefab("mod");
		for (int i = 0; i < list.Count; i++)
		{
			UIPreferences.ModPreference modPreference = UIPreferences.ModPreference.Create(list[i], settingsPrefab, this.m_CategoryContent);
			if (modPreference != null)
			{
				modPreference.onActivation = new Action(this.HandleRefreshPreferences);
				this.m_ActivePreferences.Add(modPreference);
			}
		}
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x00151224 File Offset: 0x0014F424
	private void ShowLanguageCategory(UIPreferences.Category c)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting(c.def.key);
		DT.Field def = c.def.FindChild("language_preference", null, true, true, true, '.');
		for (int i = 0; i < setting.options.Count; i++)
		{
			Value val = setting.options[i];
			GameObject settingsPrefab = this.GetSettingsPrefab("language_toggle");
			if (!(settingsPrefab == null))
			{
				UIPreferences.LanguageTogglePreference languageTogglePreference = UIPreferences.LanguageTogglePreference.Create(def, setting, val, settingsPrefab, this.m_CategoryContent);
				if (languageTogglePreference != null)
				{
					if (languageTogglePreference.def.FindChild("confirmation", null, true, true, true, '.') != null)
					{
						languageTogglePreference.OnConfirmation = new UIPreferences.Preference.ConfirmationEvent(this.HandleOnShowConfirmation);
					}
					if (languageTogglePreference.def.FindChild("keep_changes_confirmation", null, true, true, true, '.') != null)
					{
						languageTogglePreference.OnKeepChangesConfirmation = new UIPreferences.Preference.ConfirmationEvent(this.HandleOnShowKeepChangesConfirmation);
					}
					languageTogglePreference.applyChangesPopup = this.m_ApplyChangesPopup;
					languageTogglePreference.onActivation = new Action(this.HandleRefreshPreferences);
					this.m_ActivePreferences.Add(languageTogglePreference);
				}
			}
		}
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x00151345 File Offset: 0x0014F545
	protected override void Update()
	{
		base.Update();
		this.UpdateButtons();
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x00151354 File Offset: 0x0014F554
	private void UpdateButtons()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			UIPreferences.Preference preference = this.m_ActivePreferences[i];
			if (((preference != null) ? preference.setting : null) != null)
			{
				flag |= preference.HasUnappledChanges();
				flag2 |= !preference.IsDefaultValues();
				flag3 |= !preference.IsInstantApply();
			}
		}
		if (this.m_ApplyChanges != null)
		{
			this.m_ApplyChanges.gameObject.SetActive(flag3);
			this.m_ApplyChanges.Enable(flag, false);
		}
		if (this.m_RevertDefaults != null)
		{
			this.m_RevertDefaults.Enable(flag2, false);
		}
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x00151408 File Offset: 0x0014F608
	public bool HasUnappledChangesInSelectedCategory()
	{
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			UIPreferences.Preference preference = this.m_ActivePreferences[i];
			if (((preference != null) ? preference.setting : null) != null && preference.HasUnappledChanges())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x00151451 File Offset: 0x0014F651
	private void RevertChanges()
	{
		UserSettings.RevertChanges();
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x00151458 File Offset: 0x0014F658
	private GameObject GetSettingsPrefab(string key)
	{
		DT.Field field = this.preferencesDef.FindChild(key, null, true, true, true, '.');
		if (field == null)
		{
			return null;
		}
		if (field.type != "prefab")
		{
			return null;
		}
		return global::Defs.GetObj<GameObject>(this.preferencesDef, key, null);
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButton(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x001514A0 File Offset: 0x0014F6A0
	private void HandleRefreshPreferences()
	{
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			this.m_ActivePreferences[i].RefreshValues();
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x001514D4 File Offset: 0x0014F6D4
	private void RefreshEnabledPreferences()
	{
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			this.m_ActivePreferences[i].RefreshEnabled();
		}
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x00151508 File Offset: 0x0014F708
	private void HandleOnApplyDefaultsButton(BSGButton button)
	{
		this.m_ApplyChangesPopup.DoWhenVisible(delegate
		{
			for (int i = 0; i < this.m_ActivePreferences.Count; i++)
			{
				UIPreferences.Preference preference = this.m_ActivePreferences[i];
				if (((preference != null) ? preference.setting : null) != null)
				{
					List<UserSettings.SettingData> settings = this.m_ActivePreferences[i].GetSettings();
					for (int j = 0; j < settings.Count; j++)
					{
						UserSettings.SettingData settingData = settings[j];
						Value value = settingData.value;
						UserSettings.OnChangeAnalytics(settingData.id, value, settingData.default_value, true);
						UserSettings.analyticsSettingsData.Remove(settingData.id);
					}
					this.m_ActivePreferences[i].ApplyDefaultValue();
				}
			}
		});
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x00151521 File Offset: 0x0014F721
	private void HandleOnApplyChangesButton(BSGButton button)
	{
		this.m_ApplyChangesPopup.DoWhenVisible(delegate
		{
			this.ApplySettings(this.m_ActivePreferences);
		});
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x0015153C File Offset: 0x0014F73C
	private void ApplySettings(List<UIPreferences.Preference> preferences)
	{
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			UIPreferences.Preference preference = this.m_ActivePreferences[i];
			UserSettings.SettingData settingData = (preference != null) ? preference.setting : null;
			if (settingData != null)
			{
				Value value = settingData.value;
				settingData.Apply(false);
				UserSettings.OnChangeAnalytics(settingData.id, value, settingData.value, true);
				this.m_ActivePreferences[i].RefreshValues();
			}
		}
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x001515B0 File Offset: 0x0014F7B0
	private void HandleOnKeybindChange(KeyBindings.KeyBind bind)
	{
		string a = KeyBindings.ParseToStr(new KeyBindings.KeyBind[]
		{
			bind
		});
		for (int i = 0; i < this.m_ActivePreferences.Count; i++)
		{
			UIPreferences.Preference preference = this.m_ActivePreferences[i];
			UserSettings.SettingData settingData = (preference != null) ? preference.setting : null;
			if (settingData != null && !(settingData.def.type != "keybind"))
			{
				string[] array = settingData.new_value.String(null).Split(new char[]
				{
					KeyBindings.bind_separator
				});
				int num = 0;
				string text = "";
				for (int j = 0; j < array.Length; j++)
				{
					if (a != array[j])
					{
						if (num > 0)
						{
							text += KeyBindings.bind_separator.ToString();
						}
						text += array[j];
						num++;
					}
				}
				if (num != array.Length)
				{
					preference.Apply(text, false);
					preference.RefreshValues();
				}
			}
		}
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x001516BC File Offset: 0x0014F8BC
	private void HandleOnShowConfirmation(UIPreferences.Preference pref, Value new_value)
	{
		Action onConfirm = delegate()
		{
			pref.Apply(new_value, true);
		};
		this.m_Confirmation = UIPreferenceConfirmation.Create(pref.def.FindChild("confirmation", null, true, true, true, '.'), base.gameObject.transform, onConfirm, null);
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x00151720 File Offset: 0x0014F920
	private void HandleOnShowKeepChangesConfirmation(UIPreferences.Preference pref, Value new_value)
	{
		Value old_value = pref.setting.value;
		Action onCancel = delegate()
		{
			pref.Apply(old_value, true);
		};
		this.m_Confirmation = UIPreferenceConfirmation.Create(pref.def.FindChild("keep_changes_confirmation", null, true, true, true, '.'), base.gameObject.transform, null, onCancel);
	}

	// Token: 0x040019EA RID: 6634
	private static string def_id = "PreferencesWindow";

	// Token: 0x040019EB RID: 6635
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040019EC RID: 6636
	[UIFieldTarget("id_CategoriesContainer")]
	private RectTransform m_CategoriesContainer;

	// Token: 0x040019ED RID: 6637
	[UIFieldTarget("id_LanguageCategoriesContainer")]
	private RectTransform m_LanguageCategoriesContainer;

	// Token: 0x040019EE RID: 6638
	[UIFieldTarget("id_CategoryPrototype")]
	private GameObject m_CategoryPrototype;

	// Token: 0x040019EF RID: 6639
	[UIFieldTarget("id_LanguageCategoryPrototype")]
	private GameObject m_LanguageCategoryPrototype;

	// Token: 0x040019F0 RID: 6640
	[UIFieldTarget("id_CategoryContent")]
	private RectTransform m_CategoryContent;

	// Token: 0x040019F1 RID: 6641
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_Close;

	// Token: 0x040019F2 RID: 6642
	[UIFieldTarget("id_ApplyChanges")]
	private BSGButton m_ApplyChanges;

	// Token: 0x040019F3 RID: 6643
	[UIFieldTarget("id_ApplyChangesLabel")]
	private TextMeshProUGUI m_ApplyChangesLabel;

	// Token: 0x040019F4 RID: 6644
	[UIFieldTarget("id_RevertDefaults")]
	private BSGButton m_RevertDefaults;

	// Token: 0x040019F5 RID: 6645
	[UIFieldTarget("id_RevertDefaultsLabel")]
	private TextMeshProUGUI m_RevertDefaultsLabel;

	// Token: 0x040019F6 RID: 6646
	[UIFieldTarget("id_ApplyChangesPopup")]
	private ApplyChangesPopup m_ApplyChangesPopup;

	// Token: 0x040019F7 RID: 6647
	private UIPreferenceConfirmation m_Confirmation;

	// Token: 0x040019F8 RID: 6648
	private UIPreferences.Category m_SelectedCategory;

	// Token: 0x040019F9 RID: 6649
	private bool m_Initialzied;

	// Token: 0x040019FA RID: 6650
	private DT.Field preferencesDef;

	// Token: 0x040019FB RID: 6651
	private List<UIPreferences.Category> m_RootCategories = new List<UIPreferences.Category>();

	// Token: 0x040019FC RID: 6652
	private List<UIPreferences.Category> m_LanguageCategories = new List<UIPreferences.Category>();

	// Token: 0x040019FD RID: 6653
	private List<UIPreferences.Category> m_ModCategories = new List<UIPreferences.Category>();

	// Token: 0x040019FE RID: 6654
	private List<DT.Field> tmp_Eligable = new List<DT.Field>();

	// Token: 0x040019FF RID: 6655
	private List<UIPreferences.Preference> m_ActivePreferences = new List<UIPreferences.Preference>();

	// Token: 0x020007C8 RID: 1992
	public class Category : Hotspot
	{
		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06004DE1 RID: 19937 RVA: 0x0022FF8B File Offset: 0x0022E18B
		// (set) Token: 0x06004DE2 RID: 19938 RVA: 0x0022FF93 File Offset: 0x0022E193
		public bool ContainsPreferenses { get; private set; }

		// Token: 0x06004DE3 RID: 19939 RVA: 0x0022FF9C File Offset: 0x0022E19C
		protected virtual void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_SubCategoryPrototype != null)
			{
				this.m_SubCategoryPrototype.gameObject.SetActive(false);
			}
			this.m_Initialzied = true;
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x0022FFD4 File Offset: 0x0022E1D4
		public virtual void SetDefField(DT.Field def)
		{
			this.Init();
			this.def = def;
			this.settings = ((def != null) ? def.FindChild("settings", null, true, true, true, '.') : null);
			this.PopulateSubCategories();
			this.ContainsPreferenses = this.HasSettingsInfo();
			this.Refresh();
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x00230024 File Offset: 0x0022E224
		protected virtual void Refresh()
		{
			if (this.m_CategoryName != null)
			{
				UIText.SetText(this.m_CategoryName, global::Defs.Localize(this.def, "name", null, null, true, true));
			}
			bool flag = false;
			for (int i = 0; i < this.m_SubCategories.Count; i++)
			{
				if (this.m_SubCategories[i].def != null)
				{
					flag = true;
					break;
				}
			}
			this.m_CategoriesContainer.gameObject.SetActive(flag && this.m_Selected);
			this.Updatehighlight();
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x002300B0 File Offset: 0x0022E2B0
		private void PopulateSubCategories()
		{
			DT.Field field = this.def;
			List<DT.Field> list = (field != null) ? field.Children() : null;
			this.tmp_Eligable.Clear();
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DT.Field field2 = list[i];
					if (!(field2.type != "category"))
					{
						this.tmp_Eligable.Add(field2);
					}
				}
			}
			while (this.m_SubCategories.Count < this.tmp_Eligable.Count)
			{
				UIPreferences.Category category = UIPreferences.Category.Create<UIPreferences.Category>(null, this.m_SubCategoryPrototype, this.m_CategoriesContainer);
				category.OnSelect = new Action<UIPreferences.Category>(this.HandleOnSubCategorySelected);
				this.m_SubCategories.Add(category);
				category.m_parentCategory = this;
			}
			for (int j = 0; j < this.m_SubCategories.Count; j++)
			{
				UIPreferences.Category category2 = this.m_SubCategories[j];
				DT.Field field3 = (this.tmp_Eligable != null && this.tmp_Eligable.Count > j) ? this.tmp_Eligable[j] : null;
				category2.SetDefField(field3);
				category2.gameObject.SetActive(field3 != null);
			}
		}

		// Token: 0x06004DE7 RID: 19943 RVA: 0x002301D7 File Offset: 0x0022E3D7
		public void HandleOnSubCategorySelected(UIPreferences.Category c)
		{
			Action<UIPreferences.Category> onSelect = this.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(c);
		}

		// Token: 0x06004DE8 RID: 19944 RVA: 0x002301EC File Offset: 0x0022E3EC
		public void Select(bool selected)
		{
			if (this.m_Selected == selected)
			{
				return;
			}
			this.m_Selected = selected;
			for (int i = 0; i < this.m_SubCategories.Count; i++)
			{
				this.m_SubCategories[i].Select(false);
			}
			this.m_Selected = selected;
			if (this.m_Selected)
			{
				this.Expand();
			}
			else
			{
				this.Shrink();
			}
			this.Refresh();
		}

		// Token: 0x06004DE9 RID: 19945 RVA: 0x00230255 File Offset: 0x0022E455
		private void Expand()
		{
			if (this.m_CategoriesContainer != null)
			{
				this.m_CategoriesContainer.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004DEA RID: 19946 RVA: 0x00230276 File Offset: 0x0022E476
		private void Shrink()
		{
			if (this.m_CategoriesContainer != null)
			{
				this.m_CategoriesContainer.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x00230297 File Offset: 0x0022E497
		public override void OnPointerEnter(PointerEventData e)
		{
			base.OnPointerEnter(e);
			this.Updatehighlight();
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x002302A6 File Offset: 0x0022E4A6
		public override void OnPointerExit(PointerEventData e)
		{
			base.OnPointerExit(e);
			this.Updatehighlight();
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x002302B5 File Offset: 0x0022E4B5
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			Action<UIPreferences.Category> onSelect = this.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		// Token: 0x06004DEE RID: 19950 RVA: 0x002302D0 File Offset: 0x0022E4D0
		public void Updatehighlight()
		{
			if (this.m_Bullet != null)
			{
				if (this.m_Selected)
				{
					this.m_Bullet.overrideSprite = global::Defs.GetObj<Sprite>(this.settings, "bullet_selected", null);
				}
				else if (this.mouse_in)
				{
					this.m_Bullet.overrideSprite = global::Defs.GetObj<Sprite>(this.settings, "bullet_over", null);
				}
				this.m_Bullet.gameObject.SetActive(this.mouse_in || this.m_Selected);
			}
			if (this.m_CategoryName != null)
			{
				Color color = this.m_CategoryName.color;
				if (this.m_Selected)
				{
					color = global::Defs.GetColor(this.settings, "color_label_selected", null);
				}
				else if (this.mouse_in)
				{
					color = global::Defs.GetColor(this.settings, "color_label_over", null);
				}
				else
				{
					color = global::Defs.GetColor(this.settings, "color_label_normal", null);
				}
				this.m_CategoryName.color = color;
			}
			if (this.m_Background != null)
			{
				if (this.m_Selected)
				{
					this.m_Background.overrideSprite = global::Defs.GetObj<Sprite>(this.settings, "background_selected", null);
					return;
				}
				if (this.mouse_in)
				{
					this.m_Background.overrideSprite = global::Defs.GetObj<Sprite>(this.settings, "background_over", null);
					return;
				}
				this.m_Background.overrideSprite = global::Defs.GetObj<Sprite>(this.settings, "background_normal", null);
			}
		}

		// Token: 0x06004DEF RID: 19951 RVA: 0x0023043C File Offset: 0x0022E63C
		private bool HasSettingsInfo()
		{
			if (this.def == null)
			{
				return false;
			}
			List<DT.Field> list = this.def.Children();
			for (int i = 0; i < list.Count; i++)
			{
				string type = list[i].type;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
				if (num <= 1135768689U)
				{
					if (num != 905338595U)
					{
						if (num != 1076453893U)
						{
							if (num == 1135768689U)
							{
								if (type == "button")
								{
									return true;
								}
							}
						}
						else if (type == "toggle")
						{
							return true;
						}
					}
					else if (type == "keybind")
					{
						return true;
					}
				}
				else if (num <= 2459792088U)
				{
					if (num != 1856792572U)
					{
						if (num == 2459792088U)
						{
							if (type == "dropdown")
							{
								return true;
							}
						}
					}
					else if (type == "slider")
					{
						return true;
					}
				}
				else if (num != 2517238543U)
				{
					if (num == 4137097213U)
					{
						if (type == "label")
						{
							return true;
						}
					}
				}
				else if (type == "audio_slider")
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004DF0 RID: 19952 RVA: 0x00230548 File Offset: 0x0022E748
		public UIPreferences.Category GetDefaultSubSelection()
		{
			if (this.ContainsPreferenses)
			{
				return null;
			}
			if (this.m_SubCategories.Count == 0)
			{
				return null;
			}
			return this.m_SubCategories[0];
		}

		// Token: 0x06004DF1 RID: 19953 RVA: 0x0023056F File Offset: 0x0022E76F
		public UIPreferences.Category GetParentCategory()
		{
			return this.m_parentCategory;
		}

		// Token: 0x06004DF2 RID: 19954 RVA: 0x00230578 File Offset: 0x0022E778
		public static UIPreferences.Category Create<T>(DT.Field def, GameObject prototype, RectTransform parent) where T : UIPreferences.Category
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			T t = gameObject.GetOrAddComponent<T>();
			t.SetDefField(def);
			gameObject.SetActive(true);
			return t;
		}

		// Token: 0x06004DF3 RID: 19955 RVA: 0x002305C2 File Offset: 0x0022E7C2
		public override string ToString()
		{
			object[] array = new object[4];
			array[0] = "Category ";
			int num = 1;
			DT.Field field = this.def;
			array[num] = ((field != null) ? field.key : null);
			array[2] = ", Parent ";
			array[3] = this.m_parentCategory;
			return string.Concat(array);
		}

		// Token: 0x04003C6B RID: 15467
		[UIFieldTarget("id_Background")]
		protected Image m_Background;

		// Token: 0x04003C6C RID: 15468
		[UIFieldTarget("id_CategoryName")]
		protected TextMeshProUGUI m_CategoryName;

		// Token: 0x04003C6D RID: 15469
		[UIFieldTarget("id_CategoriesContainer")]
		protected RectTransform m_CategoriesContainer;

		// Token: 0x04003C6E RID: 15470
		[UIFieldTarget("id_SubCategoryPrototype")]
		protected GameObject m_SubCategoryPrototype;

		// Token: 0x04003C6F RID: 15471
		[UIFieldTarget("id_Bullet")]
		protected Image m_Bullet;

		// Token: 0x04003C70 RID: 15472
		public UIPreferences.Category m_parentCategory;

		// Token: 0x04003C71 RID: 15473
		public Action<UIPreferences.Category> OnSelect;

		// Token: 0x04003C72 RID: 15474
		public DT.Field def;

		// Token: 0x04003C73 RID: 15475
		public DT.Field settings;

		// Token: 0x04003C75 RID: 15477
		protected List<UIPreferences.Category> m_SubCategories = new List<UIPreferences.Category>();

		// Token: 0x04003C76 RID: 15478
		protected bool m_Selected;

		// Token: 0x04003C77 RID: 15479
		protected bool m_Initialzied;

		// Token: 0x04003C78 RID: 15480
		private List<DT.Field> tmp_Eligable = new List<DT.Field>();
	}

	// Token: 0x020007C9 RID: 1993
	public class LanguageCategory : UIPreferences.Category
	{
		// Token: 0x06004DF5 RID: 19957 RVA: 0x0023061B File Offset: 0x0022E81B
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			UserSettings.OnSettingChange += this.HandleOnSettingChange;
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x0023063D File Offset: 0x0022E83D
		private void HandleOnSettingChange()
		{
			this.Refresh();
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x00230648 File Offset: 0x0022E848
		protected override void Refresh()
		{
			base.Refresh();
			if (this.m_Flag != null && this.def != null)
			{
				Image flag = this.m_Flag;
				UserSettings.SettingData setting = UserSettings.GetSetting(this.def.key);
				Value? value = (setting != null) ? new Value?(setting.value) : null;
				flag.overrideSprite = UIPreferences.GetLanguageFlag((value != null) ? value.GetValueOrDefault() : null);
			}
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x002306C3 File Offset: 0x0022E8C3
		private void OnDestroy()
		{
			UserSettings.OnSettingChange -= this.HandleOnSettingChange;
		}

		// Token: 0x04003C79 RID: 15481
		[UIFieldTarget("id_Flag")]
		protected Image m_Flag;
	}

	// Token: 0x020007CA RID: 1994
	public class ModCategory : UIPreferences.Category
	{
		// Token: 0x06004DFA RID: 19962 RVA: 0x002306DE File Offset: 0x0022E8DE
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			UserSettings.OnSettingChange += this.HandleOnSettingChange;
		}

		// Token: 0x06004DFB RID: 19963 RVA: 0x0023063D File Offset: 0x0022E83D
		private void HandleOnSettingChange()
		{
			this.Refresh();
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x00230700 File Offset: 0x0022E900
		protected override void Refresh()
		{
			base.Refresh();
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x00230708 File Offset: 0x0022E908
		private void OnDestroy()
		{
			UserSettings.OnSettingChange -= this.HandleOnSettingChange;
		}
	}

	// Token: 0x020007CB RID: 1995
	public class Preference : MonoBehaviour
	{
		// Token: 0x06004DFF RID: 19967 RVA: 0x0023071B File Offset: 0x0022E91B
		protected virtual void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, true);
			UserSettings.OnSettingChange += this.HandleOnSettingChange;
			this.m_Initialzied = true;
		}

		// Token: 0x06004E00 RID: 19968 RVA: 0x00230745 File Offset: 0x0022E945
		private void HandleOnSettingChange()
		{
			this.RefreshValues();
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x0023074D File Offset: 0x0022E94D
		public virtual void SetDefField(DT.Field defField)
		{
			this.Init();
			this.def = defField;
			this.setting = UserSettings.GetSetting(this.def.key);
			this.Populate();
			this.CheckRestartWarning();
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x0023077E File Offset: 0x0022E97E
		public virtual List<UserSettings.SettingData> GetSettings()
		{
			return new List<UserSettings.SettingData>
			{
				this.setting
			};
		}

		// Token: 0x06004E03 RID: 19971 RVA: 0x00230794 File Offset: 0x0022E994
		public string ValidateWhileGameRuns()
		{
			Game game = GameLogic.Get(true);
			if (game == null || !game.IsRunning())
			{
				return "ok";
			}
			if (game.IsMultiplayer())
			{
				if (!this.def.GetBool("allowed_in_multiplayer", null, false, true, true, true, '.'))
				{
					return "WhileGameRuns";
				}
			}
			else if (!this.def.GetBool("allowed_in_singleplayer", null, false, true, true, true, '.'))
			{
				return "WhileGameRuns";
			}
			return "ok";
		}

		// Token: 0x06004E04 RID: 19972 RVA: 0x00230804 File Offset: 0x0022EA04
		public virtual string ValidateEnabled()
		{
			string text = this.ValidateWhileGameRuns();
			if (text != "ok")
			{
				return text;
			}
			if (this.setting == null)
			{
				return "ok";
			}
			string id = this.setting.id;
			if (!(id == "fps_limit"))
			{
				return "ok";
			}
			UserSettings.SettingData settingData = UserSettings.GetSetting("vsync");
			if (settingData != null && settingData.new_value == true)
			{
				return "BecauseOfVsync";
			}
			return "ok";
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x00230880 File Offset: 0x0022EA80
		public void SetDisabledTooltip(GameObject obj)
		{
			if (obj == null)
			{
				return;
			}
			string text = this.ValidateEnabled();
			if (text != "ok")
			{
				Tooltip.Get(obj.gameObject, true).SetDef("UserPreferernceDisabled" + text + "Tooltip", new Vars(this.def));
				return;
			}
			Tooltip.Get(obj.gameObject, true).Clear(true);
		}

		// Token: 0x06004E06 RID: 19974 RVA: 0x002308EA File Offset: 0x0022EAEA
		public bool IsDisabled()
		{
			return this.ValidateEnabled() != "ok";
		}

		// Token: 0x06004E07 RID: 19975 RVA: 0x002308FC File Offset: 0x0022EAFC
		public virtual bool IsDefaultValues()
		{
			return this.setting != null && this.setting.IsDefaultValue();
		}

		// Token: 0x06004E08 RID: 19976 RVA: 0x00230913 File Offset: 0x0022EB13
		public virtual bool HasUnappledChanges()
		{
			return this.setting != null && this.setting.chnaged;
		}

		// Token: 0x06004E09 RID: 19977 RVA: 0x0023092A File Offset: 0x0022EB2A
		public virtual bool IsInstantApply()
		{
			return this.def != null && this.def.GetBool("instant_apply", null, false, true, true, true, '.');
		}

		// Token: 0x06004E0A RID: 19978 RVA: 0x0023094D File Offset: 0x0022EB4D
		public virtual void ApplyDefaultValue()
		{
			if (this.setting == null)
			{
				return;
			}
			this.setting.ApplyValue(this.setting.default_value);
			this.RefreshValues();
		}

		// Token: 0x06004E0B RID: 19979 RVA: 0x00230974 File Offset: 0x0022EB74
		public virtual void Populate()
		{
			if (this.m_Label != null)
			{
				UIText.SetText(this.m_Label, this.def, "name", null, null);
				if (this.m_ShowTooltip)
				{
					Tooltip.Get(this.m_Label.gameObject, true).SetDef("UserPreferernceTooltip", new Vars(this.def));
				}
			}
			if (this.m_RestartWarningLabel != null)
			{
				UIText.SetText(this.m_RestartWarningLabel, this.def, "restart_warning", null, null);
			}
			this.RefreshValues();
		}

		// Token: 0x06004E0C RID: 19980 RVA: 0x00230A04 File Offset: 0x0022EC04
		public virtual void Apply(Value v, bool ignore_confirmations = false)
		{
			if (this.setting != null)
			{
				Value old_value = this.setting.value;
				if (!ignore_confirmations)
				{
					if (this.CheckConfirmation(v))
					{
						return;
					}
					this.CheckKeepChangesConfirmation(v);
				}
				this.setting.SetValue(v);
				if (this.IsInstantApply())
				{
					if (this.def.GetBool("popup_when_applying", null, false, true, true, true, '.'))
					{
						this.applyChangesPopup.DoWhenVisible(delegate
						{
							this.setting.Apply(false);
							UserSettings.OnChangeAnalytics(this.setting.id, old_value, this.setting.value, true);
						});
					}
					else
					{
						this.setting.Apply(false);
						UserSettings.OnChangeAnalytics(this.setting.id, old_value, this.setting.value, false);
					}
				}
			}
			if (this.onApply != null)
			{
				this.onApply();
			}
			this.CheckRestartWarning();
		}

		// Token: 0x06004E0D RID: 19981 RVA: 0x00230ADC File Offset: 0x0022ECDC
		protected void CheckRestartWarning()
		{
			if (this.m_RestartWarningLabel == null)
			{
				return;
			}
			if (this.setting == null)
			{
				this.m_RestartWarningLabel.gameObject.SetActive(false);
				return;
			}
			bool active = this.setting.chnaged && this.def.GetBool("reqire_restart", null, false, true, true, true, '.');
			this.m_RestartWarningLabel.gameObject.SetActive(active);
		}

		// Token: 0x06004E0E RID: 19982 RVA: 0x00230B4B File Offset: 0x0022ED4B
		protected bool CheckConfirmation(Value new_value)
		{
			if (this.setting == null)
			{
				return false;
			}
			UIPreferences.Preference.ConfirmationEvent onConfirmation = this.OnConfirmation;
			if (onConfirmation != null)
			{
				onConfirmation(this, new_value);
			}
			return this.OnConfirmation != null;
		}

		// Token: 0x06004E0F RID: 19983 RVA: 0x00230B73 File Offset: 0x0022ED73
		protected void CheckKeepChangesConfirmation(Value new_value)
		{
			if (this.setting == null)
			{
				return;
			}
			UIPreferences.Preference.ConfirmationEvent onKeepChangesConfirmation = this.OnKeepChangesConfirmation;
			if (onKeepChangesConfirmation == null)
			{
				return;
			}
			onKeepChangesConfirmation(this, new_value);
		}

		// Token: 0x06004E10 RID: 19984 RVA: 0x00230B90 File Offset: 0x0022ED90
		protected virtual void OnDestroy()
		{
			UserSettings.OnSettingChange -= this.HandleOnSettingChange;
		}

		// Token: 0x06004E11 RID: 19985 RVA: 0x00230BA3 File Offset: 0x0022EDA3
		public virtual void RefreshValues()
		{
			this.RefreshEnabled();
		}

		// Token: 0x06004E12 RID: 19986 RVA: 0x00230BAC File Offset: 0x0022EDAC
		public virtual void RefreshEnabled()
		{
			if (this.m_Label != null)
			{
				Color color = this.m_Label.color;
				if (this.IsDisabled())
				{
					color = global::Defs.GetColor(this.def, "color_label_disabled", color, null);
				}
				else
				{
					color = global::Defs.GetColor(this.def, "color_label_normal", color, null);
				}
				this.m_Label.color = color;
			}
		}

		// Token: 0x06004E13 RID: 19987 RVA: 0x00230C10 File Offset: 0x0022EE10
		public static T Create<T>(DT.Field def, GameObject prototype, RectTransform parent) where T : UIPreferences.Preference
		{
			if (prototype == null)
			{
				return default(T);
			}
			if (parent == null)
			{
				return default(T);
			}
			T orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<T>();
			orAddComponent.SetDefField(def);
			return orAddComponent;
		}

		// Token: 0x04003C7A RID: 15482
		[UIFieldTarget("id_Label")]
		protected TextMeshProUGUI m_Label;

		// Token: 0x04003C7B RID: 15483
		[UIFieldTarget("id_RestartWarningLabel")]
		private TextMeshProUGUI m_RestartWarningLabel;

		// Token: 0x04003C7C RID: 15484
		[UIFieldTarget("id_RestartWarning")]
		private GameObject m_RestartWarningContainer;

		// Token: 0x04003C7D RID: 15485
		public DT.Field def;

		// Token: 0x04003C7E RID: 15486
		public UserSettings.SettingData setting;

		// Token: 0x04003C7F RID: 15487
		public UIPreferences.Preference.OnApply onApply;

		// Token: 0x04003C80 RID: 15488
		protected bool m_Initialzied;

		// Token: 0x04003C81 RID: 15489
		protected bool m_ShowTooltip = true;

		// Token: 0x04003C82 RID: 15490
		public UIPreferences.Preference.ConfirmationEvent OnConfirmation;

		// Token: 0x04003C83 RID: 15491
		public UIPreferences.Preference.ConfirmationEvent OnKeepChangesConfirmation;

		// Token: 0x04003C84 RID: 15492
		public ApplyChangesPopup applyChangesPopup;

		// Token: 0x02000A2F RID: 2607
		// (Invoke) Token: 0x060055A4 RID: 21924
		public delegate void ConfirmationEvent(UIPreferences.Preference pref, Value new_value);

		// Token: 0x02000A30 RID: 2608
		// (Invoke) Token: 0x060055A8 RID: 21928
		public delegate void OnApply();
	}

	// Token: 0x020007CC RID: 1996
	public class Label : UIPreferences.Preference
	{
		// Token: 0x06004E15 RID: 19989 RVA: 0x00230C70 File Offset: 0x0022EE70
		protected override void Init()
		{
			this.m_ShowTooltip = false;
			base.Init();
		}
	}

	// Token: 0x020007CD RID: 1997
	public class TogglePreference : UIPreferences.Preference
	{
		// Token: 0x06004E17 RID: 19991 RVA: 0x00230C87 File Offset: 0x0022EE87
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			if (this.m_Toggle != null)
			{
				this.m_Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleOnToggleValueChange));
			}
		}

		// Token: 0x06004E18 RID: 19992 RVA: 0x00230CC3 File Offset: 0x0022EEC3
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.m_Toggle == null)
			{
				return;
			}
			if (this.setting == null)
			{
				return;
			}
			this.m_Toggle.SetIsOnWithoutNotify(this.setting.value);
		}

		// Token: 0x06004E19 RID: 19993 RVA: 0x00230CFE File Offset: 0x0022EEFE
		public override void RefreshEnabled()
		{
			base.RefreshEnabled();
			if (this.m_Toggle == null)
			{
				return;
			}
			this.m_Toggle.interactable = !base.IsDisabled();
			base.SetDisabledTooltip(this.m_Toggle.gameObject);
		}

		// Token: 0x06004E1A RID: 19994 RVA: 0x00230D3A File Offset: 0x0022EF3A
		protected virtual void HandleOnToggleValueChange(bool v)
		{
			this.Apply(v, false);
		}

		// Token: 0x04003C85 RID: 15493
		[UIFieldTarget("id_Toggle")]
		protected Toggle m_Toggle;
	}

	// Token: 0x020007CE RID: 1998
	public class SliderPreference : UIPreferences.Preference
	{
		// Token: 0x06004E1C RID: 19996 RVA: 0x00230D4C File Offset: 0x0022EF4C
		public override void Populate()
		{
			base.Populate();
			if (this.setting != null)
			{
				this.m_Slider.onValueChanged.RemoveListener(new UnityAction<float>(this.HandleOnSliderValueChange));
				this.m_Slider.minValue = this.setting.GetMinValue();
				this.m_Slider.maxValue = this.setting.GetMaxValue();
				this.m_Slider.wholeNumbers = (this.setting.type == Value.Type.Int);
				this.m_Slider.SetValueWithoutNotify(this.setting.value);
				this.m_Slider.onValueChanged.AddListener(new UnityAction<float>(this.HandleOnSliderValueChange));
			}
			this.UpdateValueLabel();
		}

		// Token: 0x06004E1D RID: 19997 RVA: 0x00230E14 File Offset: 0x0022F014
		private void UpdateValueLabel()
		{
			if (this.m_ValueLabel != null)
			{
				string text;
				if (this.setting == null)
				{
					text = "-";
				}
				else if (this.setting.type == Value.Type.Int)
				{
					text = this.setting.new_value.int_val.ToString();
				}
				else if (this.setting.type == Value.Type.Float)
				{
					text = this.setting.new_value.float_val.ToString("F2");
				}
				else
				{
					text = this.setting.new_value.ToString();
				}
				UIText.SetText(this.m_ValueLabel, text);
			}
		}

		// Token: 0x06004E1E RID: 19998 RVA: 0x00230EB8 File Offset: 0x0022F0B8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateValueLabel();
			if (this.setting == null)
			{
				return;
			}
			if (this.m_Slider == null)
			{
				return;
			}
			this.m_Slider.SetValueWithoutNotify(this.setting.value);
		}

		// Token: 0x06004E1F RID: 19999 RVA: 0x00230F04 File Offset: 0x0022F104
		public override void RefreshEnabled()
		{
			base.RefreshEnabled();
			if (this.m_Slider == null)
			{
				return;
			}
			this.m_Slider.interactable = !base.IsDisabled();
			base.SetDisabledTooltip(this.m_Slider.gameObject);
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x00230F40 File Offset: 0x0022F140
		private void HandleOnSliderValueChange(float v)
		{
			if (this.setting == null)
			{
				return;
			}
			float num = this.setting.GetStepValue().Float(0f);
			if (num != 0f)
			{
				v -= (float)Math.IEEERemainder((double)v, (double)num);
				if (this.m_Slider != null)
				{
					this.m_Slider.SetValueWithoutNotify(v);
				}
			}
			if (this.setting.type == Value.Type.Int)
			{
				this.Apply((int)v, false);
			}
			else
			{
				this.Apply(v, false);
			}
			this.UpdateValueLabel();
		}

		// Token: 0x04003C86 RID: 15494
		[UIFieldTarget("id_Slider")]
		private Slider m_Slider;

		// Token: 0x04003C87 RID: 15495
		[UIFieldTarget("id_ValueLabel")]
		private TextMeshProUGUI m_ValueLabel;
	}

	// Token: 0x020007CF RID: 1999
	public class SoundSliderPreference : UIPreferences.Preference
	{
		// Token: 0x06004E22 RID: 20002 RVA: 0x00230FD0 File Offset: 0x0022F1D0
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			if (this.m_Toggle != null)
			{
				this.m_Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleOnToggle));
			}
		}

		// Token: 0x06004E23 RID: 20003 RVA: 0x0023100B File Offset: 0x0022F20B
		public override void SetDefField(DT.Field defField)
		{
			this.enable_setting = UserSettings.GetSetting(defField.key + "_enabled");
			base.SetDefField(defField);
		}

		// Token: 0x06004E24 RID: 20004 RVA: 0x00231030 File Offset: 0x0022F230
		public override void Populate()
		{
			base.Populate();
			base.IsDisabled();
			if (this.setting != null)
			{
				this.m_Slider.onValueChanged.RemoveListener(new UnityAction<float>(this.HandleOnSliderValueChange));
				this.m_Slider.minValue = this.setting.GetMinValue();
				this.m_Slider.maxValue = this.setting.GetMaxValue();
				this.m_Slider.onValueChanged.AddListener(new UnityAction<float>(this.HandleOnSliderValueChange));
			}
			this.RefreshValues();
			this.UpdateValueLabel();
		}

		// Token: 0x06004E25 RID: 20005 RVA: 0x002310CC File Offset: 0x0022F2CC
		private void UpdateValueLabel()
		{
			if (this.m_ValueLabel != null)
			{
				string text = "-";
				if (this.setting != null && this.setting.new_value.is_number)
				{
					text = Mathf.RoundToInt(this.setting.new_value * 100f).ToString();
				}
				UIText.SetText(this.m_ValueLabel, text);
			}
		}

		// Token: 0x06004E26 RID: 20006 RVA: 0x00231138 File Offset: 0x0022F338
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateValueLabel();
			if (this.setting != null)
			{
				this.m_Slider.SetValueWithoutNotify(this.setting.value);
			}
			if (this.enable_setting != null)
			{
				this.m_Toggle.SetIsOnWithoutNotify(this.enable_setting.value);
			}
		}

		// Token: 0x06004E27 RID: 20007 RVA: 0x00231198 File Offset: 0x0022F398
		public override void RefreshEnabled()
		{
			base.RefreshEnabled();
			bool flag = base.IsDisabled();
			if (this.setting != null)
			{
				this.m_Slider.interactable = !flag;
				base.SetDisabledTooltip(this.m_Slider.gameObject);
			}
			if (this.enable_setting != null)
			{
				this.m_Toggle.interactable = !flag;
			}
		}

		// Token: 0x06004E28 RID: 20008 RVA: 0x002311F1 File Offset: 0x0022F3F1
		public override List<UserSettings.SettingData> GetSettings()
		{
			return new List<UserSettings.SettingData>
			{
				this.setting,
				this.enable_setting
			};
		}

		// Token: 0x06004E29 RID: 20009 RVA: 0x00231210 File Offset: 0x0022F410
		private void HandleOnToggle(bool v)
		{
			if (this.enable_setting != null)
			{
				Value value = this.enable_setting.value;
				this.enable_setting.SetValue(v);
				this.enable_setting.Apply(false);
				UserSettings.OnChangeAnalytics(this.enable_setting.id, value, this.enable_setting.value, true);
			}
		}

		// Token: 0x06004E2A RID: 20010 RVA: 0x0023126C File Offset: 0x0022F46C
		private void HandleOnSliderValueChange(float v)
		{
			float num = this.setting.GetStepValue().Float(0f);
			if (num != 0f)
			{
				v -= (float)Math.IEEERemainder((double)v, (double)num);
				if (this.m_Slider != null)
				{
					this.m_Slider.SetValueWithoutNotify(v);
				}
			}
			float num2 = this.setting.value;
			this.Apply(v, false);
			if (num2 > 0f && v == 0f)
			{
				this.HandleOnToggle(false);
			}
			else if (num2 == 0f && v > 0f)
			{
				this.HandleOnToggle(true);
			}
			this.UpdateValueLabel();
		}

		// Token: 0x06004E2B RID: 20011 RVA: 0x00231316 File Offset: 0x0022F516
		public override void ApplyDefaultValue()
		{
			if (this.enable_setting != null)
			{
				this.enable_setting.ApplyValue(this.enable_setting.default_value);
			}
			base.ApplyDefaultValue();
			this.RefreshValues();
		}

		// Token: 0x06004E2C RID: 20012 RVA: 0x00231344 File Offset: 0x0022F544
		public override bool IsDefaultValues()
		{
			bool flag = base.IsDefaultValues();
			if (this.enable_setting != null)
			{
				flag &= this.enable_setting.IsDefaultValue();
			}
			return flag;
		}

		// Token: 0x06004E2D RID: 20013 RVA: 0x00231370 File Offset: 0x0022F570
		public override bool HasUnappledChanges()
		{
			bool flag = base.HasUnappledChanges();
			if (this.enable_setting != null)
			{
				flag &= this.enable_setting.chnaged;
			}
			return flag;
		}

		// Token: 0x04003C88 RID: 15496
		[UIFieldTarget("id_Slider")]
		private Slider m_Slider;

		// Token: 0x04003C89 RID: 15497
		[UIFieldTarget("id_Toggle")]
		private Toggle m_Toggle;

		// Token: 0x04003C8A RID: 15498
		[UIFieldTarget("id_ValueLabel")]
		private TextMeshProUGUI m_ValueLabel;

		// Token: 0x04003C8B RID: 15499
		public UserSettings.SettingData enable_setting;
	}

	// Token: 0x020007D0 RID: 2000
	public class DropDownPreference : UIPreferences.Preference
	{
		// Token: 0x06004E2F RID: 20015 RVA: 0x0023139B File Offset: 0x0022F59B
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			if (this.m_Dropdown != null)
			{
				this.m_Dropdown.onValueChanged.AddListener(new UnityAction<int>(this.HandleOnValueChange));
			}
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x002313D8 File Offset: 0x0022F5D8
		public override void Populate()
		{
			base.Populate();
			this.m_Dropdown.options.Clear();
			this.m_Dropdown.indexesToHide.Clear();
			if (this.setting != null)
			{
				List<Value> options = this.setting.GetOptions();
				DT.Field field = this.setting.def.FindChild("options", null, true, true, true, '.');
				if (options != null)
				{
					for (int i = 0; i < options.Count; i++)
					{
						string text = options[i].String(null);
						DT.Field field2 = (field != null) ? field.FindChild(text, null, true, true, true, '.') : null;
						if (field2 != null)
						{
							this.m_Dropdown.options.Add(new TMP_Dropdown.OptionData(global::Defs.Localize(field2, "name", null, null, true, true)));
						}
						else
						{
							this.m_Dropdown.options.Add(new TMP_Dropdown.OptionData(text));
						}
						if (text.StartsWith("hiden_", StringComparison.Ordinal))
						{
							this.m_Dropdown.HideOption(i, true);
						}
					}
					this.m_Dropdown.SetValueWithoutNotify(this.setting.GetCurrentOptionIdx());
				}
			}
		}

		// Token: 0x06004E31 RID: 20017 RVA: 0x002314F4 File Offset: 0x0022F6F4
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.setting == null)
			{
				return;
			}
			if (this.m_Dropdown == null)
			{
				return;
			}
			this.m_Dropdown.SetValueWithoutNotify(this.setting.GetCurrentOptionIdx());
		}

		// Token: 0x06004E32 RID: 20018 RVA: 0x0023152A File Offset: 0x0022F72A
		public override void RefreshEnabled()
		{
			base.RefreshEnabled();
			if (this.m_Dropdown == null)
			{
				return;
			}
			this.m_Dropdown.interactable = !base.IsDisabled();
			base.SetDisabledTooltip(this.m_Dropdown.gameObject);
		}

		// Token: 0x06004E33 RID: 20019 RVA: 0x00231566 File Offset: 0x0022F766
		private void HandleOnValueChange(int v)
		{
			if (this.setting == null)
			{
				return;
			}
			this.Apply(this.setting.options[v], false);
		}

		// Token: 0x04003C8C RID: 15500
		[UIFieldTarget("id_Dropdown")]
		private BSG_TMP_DropDown m_Dropdown;
	}

	// Token: 0x020007D1 RID: 2001
	public class ButtonPreference : UIPreferences.Preference
	{
		// Token: 0x06004E35 RID: 20021 RVA: 0x00231589 File Offset: 0x0022F789
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			if (this.m_Button != null)
			{
				this.m_Button.onClick = new BSGButton.OnClick(this.HandleOnButtonClick);
			}
		}

		// Token: 0x06004E36 RID: 20022 RVA: 0x002315BF File Offset: 0x0022F7BF
		public override void Populate()
		{
			base.Populate();
			UIText.SetText(this.m_ButtonLabel, this.def, "button_label", null, null);
		}

		// Token: 0x06004E37 RID: 20023 RVA: 0x002315DF File Offset: 0x0022F7DF
		private void HandleOnButtonClick(BSGButton b)
		{
			if (this.setting != null)
			{
				if (this.OnConfirmation != null)
				{
					this.OnConfirmation(this, Value.Unknown);
					return;
				}
				this.Apply(Value.Unknown, false);
			}
		}

		// Token: 0x06004E38 RID: 20024 RVA: 0x0023160F File Offset: 0x0022F80F
		public override void RefreshEnabled()
		{
			base.RefreshEnabled();
			this.m_Button.Enable(!base.IsDisabled(), false);
			base.SetDisabledTooltip(this.m_Button.gameObject);
		}

		// Token: 0x06004E39 RID: 20025 RVA: 0x0023163D File Offset: 0x0022F83D
		public override void Apply(Value v, bool ignore_confirmations = false)
		{
			if (this.setting != null)
			{
				this.setting.ApplyAction();
			}
		}

		// Token: 0x04003C8D RID: 15501
		[UIFieldTarget("id_Button")]
		private BSGButton m_Button;

		// Token: 0x04003C8E RID: 15502
		[UIFieldTarget("id_ButtonLabel")]
		private TextMeshProUGUI m_ButtonLabel;
	}

	// Token: 0x020007D2 RID: 2002
	public class KeyBindPreference : UIPreferences.Preference
	{
		// Token: 0x06004E3B RID: 20027 RVA: 0x00231654 File Offset: 0x0022F854
		protected override void Init()
		{
			base.Init();
			TMP_InputField input = this.m_Input;
			UIPreferences.KeyBindPreference.FixScrollRect fixScrollRect = (input != null) ? input.GetOrAddComponent<UIPreferences.KeyBindPreference.FixScrollRect>() : null;
			if (this.m_Input != null)
			{
				this.m_Input.onSelect.AddListener(new UnityAction<string>(this.OnFocusGain));
				this.m_Input.onDeselect.AddListener(new UnityAction<string>(this.OnFocusLoss));
			}
			if (fixScrollRect != null)
			{
				fixScrollRect.MainScroll = base.transform.parent.parent.parent.GetComponent<ScrollRect>();
			}
			if (this.m_Focus != null)
			{
				this.m_Focus.SetActive(false);
			}
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x00231703 File Offset: 0x0022F903
		private void OnEnable()
		{
			UIPreferences.KeyBindPreference.disableOtherInput = true;
			this.m_Focused = false;
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x00231712 File Offset: 0x0022F912
		private void OnDisable()
		{
			UIPreferences.KeyBindPreference.disableOtherInput = false;
		}

		// Token: 0x06004E3E RID: 20030 RVA: 0x0023171C File Offset: 0x0022F91C
		public override void Populate()
		{
			base.Populate();
			if (this.m_Input != null)
			{
				this.m_Input.onValidateInput = ((string text, int charIndex, char addedChar) => '\0');
			}
			if (this.m_Focus != null)
			{
				this.m_Focus.SetActive(false);
			}
			this.RefreshValues();
			this.UpdateTextColor();
		}

		// Token: 0x06004E3F RID: 20031 RVA: 0x00231790 File Offset: 0x0022F990
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.setting != null)
			{
				string[] array = this.setting.new_value.String(null).Split(new char[]
				{
					KeyBindings.bind_separator
				});
				KeyBindings.KeyBind keyBind;
				if (array != null && array.Length != 0 && KeyBindings.TryParseSingleBind(array[0], out keyBind))
				{
					UIText.SetText(this.m_InputPlaceholder, keyBind.Localize());
					return;
				}
				UIText.SetText(this.m_InputPlaceholder, "");
			}
		}

		// Token: 0x06004E40 RID: 20032 RVA: 0x00231808 File Offset: 0x0022FA08
		private void LateUpdate()
		{
			KeyBindings.KeyBind keyBind;
			if (this.m_Input != null && this.m_Input.isFocused && this.DetectKeyBind(out keyBind))
			{
				Action<KeyBindings.KeyBind> onBindChange = this.OnBindChange;
				if (onBindChange != null)
				{
					onBindChange(keyBind);
				}
				this.Apply(KeyBindings.ParseToStr(new KeyBindings.KeyBind[]
				{
					keyBind
				}), false);
				UIText.SetText(this.m_InputPlaceholder, keyBind.Localize());
				this.RefreshValues();
				this.m_Input.DeactivateInputField(false);
			}
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x00231890 File Offset: 0x0022FA90
		private void UpdateTextColor()
		{
			if (this.m_InputPlaceholder == null)
			{
				return;
			}
			string key = "keybind_text_color." + (this.m_Focused ? "focus" : "normal");
			this.m_InputPlaceholder.color = global::Defs.GetColor(this.def, key, null);
		}

		// Token: 0x06004E42 RID: 20034 RVA: 0x002318E4 File Offset: 0x0022FAE4
		public bool DetectKeyBind(out KeyBindings.KeyBind keybind)
		{
			keybind = default(KeyBindings.KeyBind);
			foreach (object obj in Enum.GetValues(typeof(KeyCode)))
			{
				KeyCode key = (KeyCode)obj;
				if (!KeyBindings.IsKeyForbidden(key))
				{
					if (KeyBindings.IsKeyModifier(key))
					{
						if (Input.GetKeyUp(key))
						{
							keybind.key = key;
							return true;
						}
					}
					else if (Input.GetKeyDown(key))
					{
						keybind.key = key;
						foreach (KeyCode keyCode in KeyBindings.GetModifiers())
						{
							if (UICommon.GetKey(keyCode, true))
							{
								keybind.modifier = keyCode;
								break;
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004E43 RID: 20035 RVA: 0x002319D8 File Offset: 0x0022FBD8
		private void OnFocusGain(string selectionEvent)
		{
			this.m_Focused = true;
			if (this.m_Focus != null)
			{
				this.m_Focus.gameObject.SetActive(true);
			}
			this.UpdateTextColor();
		}

		// Token: 0x06004E44 RID: 20036 RVA: 0x00231A06 File Offset: 0x0022FC06
		private void OnFocusLoss(string selectionEvent)
		{
			this.m_Focused = false;
			if (this.m_Focus != null)
			{
				this.m_Focus.gameObject.SetActive(false);
			}
			this.UpdateTextColor();
		}

		// Token: 0x04003C8F RID: 15503
		[UIFieldTarget("id_Input")]
		private TMP_InputField m_Input;

		// Token: 0x04003C90 RID: 15504
		[UIFieldTarget("id_InputPlaceholder")]
		private TextMeshProUGUI m_InputPlaceholder;

		// Token: 0x04003C91 RID: 15505
		[UIFieldTarget("id_Focus")]
		private GameObject m_Focus;

		// Token: 0x04003C92 RID: 15506
		public Action<KeyBindings.KeyBind> OnBindChange;

		// Token: 0x04003C93 RID: 15507
		private bool m_Focused;

		// Token: 0x04003C94 RID: 15508
		public static bool disableOtherInput;

		// Token: 0x02000A32 RID: 2610
		public class FixScrollRect : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollHandler
		{
			// Token: 0x060055AD RID: 21933 RVA: 0x00249EB1 File Offset: 0x002480B1
			public void OnBeginDrag(PointerEventData eventData)
			{
				this.MainScroll.OnBeginDrag(eventData);
			}

			// Token: 0x060055AE RID: 21934 RVA: 0x00249EBF File Offset: 0x002480BF
			public void OnDrag(PointerEventData eventData)
			{
				this.MainScroll.OnDrag(eventData);
			}

			// Token: 0x060055AF RID: 21935 RVA: 0x00249ECD File Offset: 0x002480CD
			public void OnEndDrag(PointerEventData eventData)
			{
				this.MainScroll.OnEndDrag(eventData);
			}

			// Token: 0x060055B0 RID: 21936 RVA: 0x00249EDB File Offset: 0x002480DB
			public void OnScroll(PointerEventData data)
			{
				this.MainScroll.OnScroll(data);
			}

			// Token: 0x040046B4 RID: 18100
			public ScrollRect MainScroll;
		}
	}

	// Token: 0x020007D3 RID: 2003
	public class LanguageTogglePreference : UIPreferences.TogglePreference
	{
		// Token: 0x06004E47 RID: 20039 RVA: 0x00231A34 File Offset: 0x0022FC34
		public void SetData(DT.Field defField, UserSettings.SettingData setting, string languageKey)
		{
			this.SetDefField(defField);
			this.setting = setting;
			this.languageKey = languageKey;
			this.Populate();
			base.CheckRestartWarning();
		}

		// Token: 0x06004E48 RID: 20040 RVA: 0x00231A5C File Offset: 0x0022FC5C
		protected override void Init()
		{
			base.Init();
			if (this.m_HitBox != null)
			{
				this.m_HitBox.onClick = new BSGButton.OnClick(this.HandleOnHitBox);
			}
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x00231A8C File Offset: 0x0022FC8C
		public override string ValidateEnabled()
		{
			string text = base.ValidateEnabled();
			if (text != "ok")
			{
				return text;
			}
			if (this.setting == null)
			{
				return "ok";
			}
			string id = this.setting.id;
			if (!(id == "language"))
			{
				if (!(id == "audio_language"))
				{
					return "ok";
				}
				if (UserSettings.ValidateAudioLanguageKey(this.languageKey))
				{
					return "ok";
				}
				return "IncorrectLanguage";
			}
			else
			{
				if (UserSettings.ValidateLanguageKey(this.languageKey))
				{
					return "ok";
				}
				return "IncorrectLanguage";
			}
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x00231B28 File Offset: 0x0022FD28
		public override void Populate()
		{
			base.Populate();
			if (this.setting != null)
			{
				this.m_Toggle.SetIsOnWithoutNotify(this.setting.new_value == this.languageKey);
				GameObject selected = this.m_Selected;
				if (selected != null)
				{
					selected.SetActive(this.setting.new_value == this.languageKey);
				}
			}
			if (this.m_Label != null)
			{
				global::Defs defs = global::Defs.Get(false);
				List<DT.Field> list = (defs != null) ? defs.languages : null;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].key == this.languageKey)
						{
							UIText.SetText(this.m_Label, list[i].Value(null, true, true));
							break;
						}
					}
				}
				if (this.m_ShowTooltip)
				{
					Tooltip.Get(this.m_Label.gameObject, true).Clear(true);
				}
			}
			if (this.m_Flag != null)
			{
				this.m_Flag.overrideSprite = UIPreferences.GetLanguageFlag(this.languageKey);
			}
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x00231C4C File Offset: 0x0022FE4C
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.setting == null)
			{
				return;
			}
			Toggle toggle = this.m_Toggle;
			if (toggle != null)
			{
				toggle.SetIsOnWithoutNotify(this.setting.new_value == this.languageKey);
			}
			GameObject selected = this.m_Selected;
			if (selected == null)
			{
				return;
			}
			selected.SetActive(this.setting.new_value == this.languageKey);
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x00231CB5 File Offset: 0x0022FEB5
		protected override void HandleOnToggleValueChange(bool v)
		{
			if (v)
			{
				this.Apply(this.languageKey, false);
				this.onActivation();
				return;
			}
			this.RefreshValues();
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x00231CD9 File Offset: 0x0022FED9
		private void HandleOnHitBox(BSGButton btn)
		{
			if (base.IsDisabled())
			{
				return;
			}
			this.m_Toggle.isOn = !this.m_Toggle.isOn;
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x00231CFD File Offset: 0x0022FEFD
		public static UIPreferences.LanguageTogglePreference Create(DT.Field def, UserSettings.SettingData setting, string languageKey, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			UIPreferences.LanguageTogglePreference orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIPreferences.LanguageTogglePreference>();
			orAddComponent.SetData(def, setting, languageKey);
			return orAddComponent;
		}

		// Token: 0x04003C95 RID: 15509
		[UIFieldTarget("id_Flag")]
		protected Image m_Flag;

		// Token: 0x04003C96 RID: 15510
		[UIFieldTarget("id_Selected")]
		protected GameObject m_Selected;

		// Token: 0x04003C97 RID: 15511
		[UIFieldTarget("id_HitBox")]
		protected BSGButton m_HitBox;

		// Token: 0x04003C98 RID: 15512
		private Value languageKey;

		// Token: 0x04003C99 RID: 15513
		public Action onActivation;
	}

	// Token: 0x020007D4 RID: 2004
	public class ModPreference : UIPreferences.Preference
	{
		// Token: 0x06004E50 RID: 20048 RVA: 0x00231D3A File Offset: 0x0022FF3A
		public void SetData(UIPreferences.ModPreference.ModData modData)
		{
			this.Init();
			this.Data = modData;
			this.def = global::Defs.GetDefField("Preferences", "Preference");
			this.setting = UserSettings.GetSetting("active_mod");
			this.Populate();
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x00231D74 File Offset: 0x0022FF74
		protected override void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			base.Init();
			if (this.m_Toggle != null)
			{
				this.m_Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleOnToggleValueChange));
			}
			if (this.m_VersionDropdown != null)
			{
				this.m_VersionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.HandleOnVersionValueChange));
			}
			if (this.m_HitBox != null)
			{
				this.m_HitBox.onClick = new BSGButton.OnClick(this.HandleOnHitBox);
			}
		}

		// Token: 0x06004E52 RID: 20050 RVA: 0x00231E0C File Offset: 0x0023000C
		public override void Populate()
		{
			Game game = GameLogic.Get(true);
			bool flag = game != null && !game.IsRunning();
			if (this.m_Label != null)
			{
				Color color = this.m_Label.color;
				if (!flag)
				{
					color = global::Defs.GetColor(this.def, "color_label_disabled", color, null);
				}
				else
				{
					color = global::Defs.GetColor(this.def, "color_label_normal", color, null);
				}
				this.m_Label.color = color;
				UIText.SetText(this.m_Label, this.Data.GetName());
			}
			if (this.m_Toggle != null)
			{
				this.m_Toggle.interactable = flag;
				if (!flag)
				{
					Tooltip.Get(this.m_Toggle.gameObject, true).SetDef("ChangingModSettingsDisabledWhileGameRunsTooltip", new Vars(this.def));
				}
				else
				{
					Tooltip.Get(this.m_Toggle.gameObject, true).Clear(true);
				}
			}
			if (this.m_VersionDropdown != null)
			{
				bool flag2 = this.Data.NumVariants() > 1;
				this.m_VersionDropdown.gameObject.SetActive(flag2);
				if (flag2)
				{
					this.m_VersionDropdown.interactable = flag;
					if (!flag)
					{
						Tooltip.Get(this.m_VersionDropdown.gameObject, true).SetDef("ChangingModSettingsDisabledWhileGameRunsTooltip", new Vars(this.def));
					}
					else
					{
						Tooltip.Get(this.m_VersionDropdown.gameObject, true).Clear(true);
					}
					this.m_VersionDropdown.ClearOptions();
					for (int i = 0; i < this.Data.variants.Count; i++)
					{
						string text = this.Data.variants[i].version;
						if (string.IsNullOrEmpty(text))
						{
							text = global::Defs.Localize("Preferences.mod_base", null, null, true, true);
						}
						this.m_VersionDropdown.options.Add(new TMP_Dropdown.OptionData(text));
					}
					this.m_VersionDropdown.SetValueWithoutNotify(this.Data.GetBaseVersionIndex());
				}
			}
			this.RefreshValues();
		}

		// Token: 0x06004E53 RID: 20051 RVA: 0x00232008 File Offset: 0x00230208
		public override void RefreshValues()
		{
			if (this.setting == null)
			{
				return;
			}
			string text = this.setting.new_value.String(null);
			if (!string.IsNullOrEmpty(text) || !this.Data.IsNone())
			{
				string[] array = text.Split(new char[]
				{
					'/'
				});
				string a = "";
				string text2 = "";
				if (array.Length == 3)
				{
					a = array[0];
					text2 = array[1];
					string text3 = array[2];
				}
				bool flag = a == this.Data.GetName();
				if (this.m_Toggle != null)
				{
					this.m_Toggle.SetIsOnWithoutNotify(flag);
				}
				if (this.m_Selected != null)
				{
					this.m_Selected.SetActive(flag);
				}
				if (this.m_VersionDropdown != null)
				{
					int num;
					if (flag)
					{
						num = ((!string.IsNullOrEmpty(text2)) ? this.Data.GetVerionIndex(text2) : this.Data.GetBaseVersionIndex());
					}
					else
					{
						num = this.m_lastSelectedVerison;
					}
					if (num != -1)
					{
						this.m_VersionDropdown.SetValueWithoutNotify(num);
					}
				}
				return;
			}
			this.m_Toggle.SetIsOnWithoutNotify(true);
			GameObject selected = this.m_Selected;
			if (selected == null)
			{
				return;
			}
			selected.SetActive(true);
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x00232130 File Offset: 0x00230330
		protected void HandleOnToggleValueChange(bool v)
		{
			if (this.Data.IsNone())
			{
				this.Apply("", false);
			}
			else
			{
				Mod version;
				if (this.Data.NumVariants() > 1)
				{
					version = this.Data.GetVersion(this.m_VersionDropdown.value);
				}
				else
				{
					version = this.Data.GetVersion(0);
				}
				if (version != null)
				{
					this.Apply(version.mod_id, false);
				}
			}
			if (this.onActivation != null)
			{
				this.onActivation();
				return;
			}
			this.RefreshValues();
		}

		// Token: 0x06004E55 RID: 20053 RVA: 0x002321C0 File Offset: 0x002303C0
		private void HandleOnVersionValueChange(int v)
		{
			Mod version = this.Data.GetVersion(v);
			if (version != null)
			{
				this.Apply(version.mod_id, false);
				this.m_lastSelectedVerison = v;
			}
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x002321F6 File Offset: 0x002303F6
		private void HandleOnHitBox(BSGButton btn)
		{
			this.m_Toggle.isOn = !this.m_Toggle.isOn;
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x00232211 File Offset: 0x00230411
		public static UIPreferences.ModPreference Create(UIPreferences.ModPreference.ModData mod, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			UIPreferences.ModPreference orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIPreferences.ModPreference>();
			orAddComponent.SetData(mod);
			return orAddComponent;
		}

		// Token: 0x04003C9A RID: 15514
		[UIFieldTarget("id_Selected")]
		protected GameObject m_Selected;

		// Token: 0x04003C9B RID: 15515
		[UIFieldTarget("id_HitBox")]
		protected BSGButton m_HitBox;

		// Token: 0x04003C9C RID: 15516
		[UIFieldTarget("id_Toggle")]
		protected Toggle m_Toggle;

		// Token: 0x04003C9D RID: 15517
		[UIFieldTarget("id_VersionDropdown")]
		private BSG_TMP_DropDown m_VersionDropdown;

		// Token: 0x04003C9E RID: 15518
		public Action onActivation;

		// Token: 0x04003C9F RID: 15519
		public UIPreferences.ModPreference.ModData Data;

		// Token: 0x04003CA0 RID: 15520
		private int m_lastSelectedVerison = -1;

		// Token: 0x02000A34 RID: 2612
		public class ModData
		{
			// Token: 0x060055B5 RID: 21941 RVA: 0x0000B82A File Offset: 0x00009A2A
			public ModData()
			{
			}

			// Token: 0x060055B6 RID: 21942 RVA: 0x00249EF5 File Offset: 0x002480F5
			public ModData(List<Mod> variants)
			{
				this.variants = variants;
			}

			// Token: 0x060055B7 RID: 21943 RVA: 0x00249F04 File Offset: 0x00248104
			public bool IsNone()
			{
				return this.variants == null;
			}

			// Token: 0x060055B8 RID: 21944 RVA: 0x00249F0F File Offset: 0x0024810F
			public string GetName()
			{
				if (this.variants == null || this.variants.Count == 0)
				{
					return global::Defs.Localize("Preferences.mod_none", null, null, true, true);
				}
				return this.variants[0].name;
			}

			// Token: 0x060055B9 RID: 21945 RVA: 0x00249F46 File Offset: 0x00248146
			public string GetActiveId()
			{
				if (this.variants == null || this.variants.Count == 0)
				{
					return "";
				}
				return this.variants[0].mod_id;
			}

			// Token: 0x060055BA RID: 21946 RVA: 0x00249F74 File Offset: 0x00248174
			public Mod GetVersion(int index)
			{
				if (this.variants == null || this.variants.Count == 0 || this.variants.Count <= index)
				{
					return null;
				}
				return this.variants[index];
			}

			// Token: 0x060055BB RID: 21947 RVA: 0x00249FA8 File Offset: 0x002481A8
			public string GetVersionName(int index)
			{
				Mod version = this.GetVersion(index);
				if (version == null)
				{
					return "";
				}
				string version2 = version.version;
				if (string.IsNullOrEmpty(version2))
				{
					return global::Defs.Localize("Preferences.mod_base", null, null, true, true);
				}
				return version2;
			}

			// Token: 0x060055BC RID: 21948 RVA: 0x00249FE5 File Offset: 0x002481E5
			public int NumVariants()
			{
				if (this.variants == null)
				{
					return 0;
				}
				return this.variants.Count;
			}

			// Token: 0x060055BD RID: 21949 RVA: 0x00249FFC File Offset: 0x002481FC
			public int GetVerionIndex(string version)
			{
				if (this.variants == null || this.variants.Count == 0)
				{
					return -1;
				}
				for (int i = 0; i < this.variants.Count; i++)
				{
					if (this.variants[i].version == version)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060055BE RID: 21950 RVA: 0x0024A054 File Offset: 0x00248254
			public Mod GetBaseVersion()
			{
				if (this.variants == null || this.variants.Count == 0)
				{
					return null;
				}
				for (int i = 0; i < this.variants.Count; i++)
				{
					Mod mod = this.variants[i];
					if (string.IsNullOrEmpty(mod.version))
					{
						return mod;
					}
				}
				return null;
			}

			// Token: 0x060055BF RID: 21951 RVA: 0x0024A0AC File Offset: 0x002482AC
			public int GetBaseVersionIndex()
			{
				if (this.variants == null || this.variants.Count == 0)
				{
					return -1;
				}
				for (int i = 0; i < this.variants.Count; i++)
				{
					if (string.IsNullOrEmpty(this.variants[i].version))
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x060055C0 RID: 21952 RVA: 0x0024A104 File Offset: 0x00248304
			public static List<UIPreferences.ModPreference.ModData> BuildModData()
			{
				UIPreferences.ModPreference.ModData.tmp_ModVariants.Clear();
				List<UIPreferences.ModPreference.ModData> list = new List<UIPreferences.ModPreference.ModData>();
				list.Add(new UIPreferences.ModPreference.ModData());
				ModManager modManager = ModManager.Get(false);
				if (modManager != null)
				{
					List<Mod> allMods = modManager.GetAllMods();
					if (allMods != null && allMods.Count > 0)
					{
						for (int i = 0; i < allMods.Count; i++)
						{
							Mod mod = allMods[i];
							if (!UIPreferences.ModPreference.ModData.tmp_ModVariants.ContainsKey(mod.name))
							{
								UIPreferences.ModPreference.ModData.tmp_ModVariants.Add(mod.name, new List<Mod>());
							}
							UIPreferences.ModPreference.ModData.tmp_ModVariants[mod.name].Add(mod);
						}
					}
					foreach (KeyValuePair<string, List<Mod>> keyValuePair in UIPreferences.ModPreference.ModData.tmp_ModVariants)
					{
						list.Add(new UIPreferences.ModPreference.ModData(keyValuePair.Value));
					}
					UIPreferences.ModPreference.ModData.tmp_ModVariants.Clear();
				}
				return list;
			}

			// Token: 0x040046B7 RID: 18103
			public List<Mod> variants;

			// Token: 0x040046B8 RID: 18104
			private static Dictionary<string, List<Mod>> tmp_ModVariants = new Dictionary<string, List<Mod>>();
		}
	}
}
