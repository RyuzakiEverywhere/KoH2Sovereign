using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class UIBattleViewFiltersWindow : UIWindow
{
	// Token: 0x06001AF4 RID: 6900 RVA: 0x001039B9 File Offset: 0x00101BB9
	public override void Show()
	{
		base.Show();
		this.Initialize();
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x001039C7 File Offset: 0x00101BC7
	public override void Hide(bool silent = false)
	{
		this.RemoveListeners();
		base.Hide(silent);
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x001039D6 File Offset: 0x00101BD6
	public override string GetDefId()
	{
		return UIBattleViewFiltersWindow.def_id;
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x001039DD File Offset: 0x00101BDD
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIBattleViewFiltersWindow.def_id, null);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x001039EA File Offset: 0x00101BEA
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		this.LoadSettings();
		this.LoadFilters();
		this.AddListeners();
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x00103A08 File Offset: 0x00101C08
	private void LoadSettings()
	{
		if (this.m_CaptionLabel != null)
		{
			this.m_CaptionLabel.color = global::Defs.GetColor(UIBattleViewFiltersWindow.def_id, "title_label_color");
			UIText.SetText(this.m_CaptionLabel, global::Defs.Localize(this.window_def, "title", null, null, true, true));
		}
		if (this.m_ToggleAllLabel != null)
		{
			this.m_ToggleAllLabel.color = global::Defs.GetColor(UIBattleViewFiltersWindow.def_id, "toggle_all_label_color");
			UIText.SetText(this.m_ToggleAllLabel, global::Defs.Localize(this.window_def, "toggle_all", null, null, true, true));
		}
		if (this.m_ResetLabel != null)
		{
			this.m_ResetLabel.color = global::Defs.GetColor(UIBattleViewFiltersWindow.def_id, "reset_label_color");
			UIText.SetText(this.m_ResetLabel, global::Defs.Localize(this.window_def, "reset", null, null, true, true));
		}
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x00103AEC File Offset: 0x00101CEC
	private void LoadFilters()
	{
		if (this.m_Container == null)
		{
			return;
		}
		this.m_ButtonsGroup.SetParent(base.transform);
		UICommon.DeleteChildren(this.m_Container);
		this.m_filters.Clear();
		this.m_filterPrefab = global::Defs.GetObj<GameObject>(this.window_def, "filter_prefab", null);
		List<DT.Field> list = this.window_def.FindChild("Filters", null, true, true, true, '.').Children();
		for (int i = 0; i < list.Count; i++)
		{
			if (!string.IsNullOrEmpty(list[i].key))
			{
				UIBattleViewFilterToggle component = global::Common.Spawn(this.m_filterPrefab, this.m_Container, false, "").GetComponent<UIBattleViewFilterToggle>();
				component.Set(list[i], BattleViewUI.Get().FiltersEventHub);
				this.m_filters.Add(component);
			}
		}
		this.m_ButtonsGroup.SetParent(this.m_Container);
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x00103BD8 File Offset: 0x00101DD8
	private void AddListeners()
	{
		if (this.m_Close != null)
		{
			BSGButton close = this.m_Close;
			close.onClick = (BSGButton.OnClick)Delegate.Combine(close.onClick, new BSGButton.OnClick(this.CloseButton_OnClick));
		}
		if (this.m_ToggleAllButton != null)
		{
			BSGButton toggleAllButton = this.m_ToggleAllButton;
			toggleAllButton.onClick = (BSGButton.OnClick)Delegate.Combine(toggleAllButton.onClick, new BSGButton.OnClick(this.ToggleAllButton_OnClick));
		}
		if (this.m_ResetButton != null)
		{
			BSGButton resetButton = this.m_ResetButton;
			resetButton.onClick = (BSGButton.OnClick)Delegate.Combine(resetButton.onClick, new BSGButton.OnClick(this.ResetButton_OnClick));
		}
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x00103C84 File Offset: 0x00101E84
	private void RemoveListeners()
	{
		if (this.m_Close != null)
		{
			BSGButton close = this.m_Close;
			close.onClick = (BSGButton.OnClick)Delegate.Remove(close.onClick, new BSGButton.OnClick(this.CloseButton_OnClick));
		}
		if (this.m_ToggleAllButton != null)
		{
			BSGButton toggleAllButton = this.m_ToggleAllButton;
			toggleAllButton.onClick = (BSGButton.OnClick)Delegate.Remove(toggleAllButton.onClick, new BSGButton.OnClick(this.ToggleAllButton_OnClick));
		}
		if (this.m_ResetButton != null)
		{
			BSGButton resetButton = this.m_ResetButton;
			resetButton.onClick = (BSGButton.OnClick)Delegate.Remove(resetButton.onClick, new BSGButton.OnClick(this.ResetButton_OnClick));
		}
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x00103D30 File Offset: 0x00101F30
	private void CloseButton_OnClick(BSGButton b)
	{
		if (this.m_Close)
		{
			this.m_Close.onClick = null;
		}
		this.Close(false);
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x00103D54 File Offset: 0x00101F54
	private void ToggleAllButton_OnClick(BSGButton b)
	{
		bool flag = false;
		bool flag2 = true;
		for (int i = 0; i < this.m_filters.Count; i++)
		{
			bool flag3 = this.m_filters[i].IsSelected();
			if (i == 0)
			{
				flag = flag3;
			}
			else if (flag != flag3)
			{
				flag2 = false;
				break;
			}
		}
		if (flag2)
		{
			this.m_nextToggleSelectionState = !flag;
		}
		this.SetAllFilters(this.m_nextToggleSelectionState);
		this.m_nextToggleSelectionState = !this.m_nextToggleSelectionState;
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x00103DC4 File Offset: 0x00101FC4
	private void SetAllFilters(bool selected)
	{
		for (int i = 0; i < this.m_filters.Count; i++)
		{
			this.m_filters[i].SetOn(selected, true);
		}
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x00103DFC File Offset: 0x00101FFC
	private void ResetButton_OnClick(BSGButton b)
	{
		foreach (UIBattleViewFilterToggle uibattleViewFilterToggle in this.m_filters)
		{
			uibattleViewFilterToggle.SetOn(true, true);
		}
		this.m_nextToggleSelectionState = false;
	}

	// Token: 0x04001190 RID: 4496
	private static string def_id = "UIBattleViewFiltersWindow";

	// Token: 0x04001191 RID: 4497
	[UIFieldTarget("id_Container")]
	private RectTransform m_Container;

	// Token: 0x04001192 RID: 4498
	[UIFieldTarget("id_ButtonsGroup")]
	private RectTransform m_ButtonsGroup;

	// Token: 0x04001193 RID: 4499
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001194 RID: 4500
	[UIFieldTarget("id_ToggleAllButton")]
	private BSGButton m_ToggleAllButton;

	// Token: 0x04001195 RID: 4501
	[UIFieldTarget("id_ResetButton")]
	private BSGButton m_ResetButton;

	// Token: 0x04001196 RID: 4502
	[UIFieldTarget("id_CaptionLabel")]
	private TextMeshProUGUI m_CaptionLabel;

	// Token: 0x04001197 RID: 4503
	[UIFieldTarget("id_ToggleAllLabel")]
	private TextMeshProUGUI m_ToggleAllLabel;

	// Token: 0x04001198 RID: 4504
	[UIFieldTarget("id_ResetLabel")]
	private TextMeshProUGUI m_ResetLabel;

	// Token: 0x04001199 RID: 4505
	private GameObject m_filterPrefab;

	// Token: 0x0400119A RID: 4506
	private List<UIBattleViewFilterToggle> m_filters = new List<UIBattleViewFilterToggle>();

	// Token: 0x0400119B RID: 4507
	private bool m_nextToggleSelectionState;
}
