using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class UIBattleViewGroupsControler : MonoBehaviour
{
	// Token: 0x06001B0C RID: 6924 RVA: 0x00103FEC File Offset: 0x001021EC
	private void Awake()
	{
		this.Initialize();
		this.Refresh();
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x00103FFA File Offset: 0x001021FA
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x00104004 File Offset: 0x00102204
	public void Refresh()
	{
		for (int i = 1; i <= this.m_controlGroupButtons.Count; i++)
		{
			if (this.m_ui.groups.ContainsKey(i) && this.m_ui.groups[i].squads.Count > 0)
			{
				this.ShowControlGroup(i, true);
			}
			else
			{
				this.ShowControlGroup(i, false);
			}
		}
		if (this.m_ui.SelectedGroupId > 0 && this.m_controlGroupButtons.ContainsKey(this.m_ui.SelectedGroupId))
		{
			this.SelectControlGroup(this.m_ui.SelectedGroupId);
			return;
		}
		this.DeselectControlGroup();
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x001040A8 File Offset: 0x001022A8
	private void Initialize()
	{
		this.m_ui = BattleViewUI.Get();
		this.InitializeComponents();
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x001040BC File Offset: 0x001022BC
	private void InitializeComponents()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			UIBattleViewControlGroup component = base.transform.GetChild(i).GetComponent<UIBattleViewControlGroup>();
			if (!(component == null))
			{
				component.Initialize();
				this.m_controlGroupButtons.Add(i + 1, component);
				component.SetText((i + 1).ToString());
				component.onClick += this.ControlGroupButton_OnClick;
			}
		}
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x00104132 File Offset: 0x00102332
	private void ShowControlGroup(int id, bool show)
	{
		this.m_controlGroupButtons[id].gameObject.SetActive(show);
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x0010414C File Offset: 0x0010234C
	private void SelectControlGroup(int id)
	{
		this.DeselectControlGroup();
		UIBattleViewControlGroup uibattleViewControlGroup = this.m_controlGroupButtons[id];
		if (uibattleViewControlGroup == null)
		{
			return;
		}
		if (uibattleViewControlGroup == this.m_selectedGroupButton)
		{
			return;
		}
		uibattleViewControlGroup.Select(true);
		this.m_selectedGroupButton = uibattleViewControlGroup;
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x00104193 File Offset: 0x00102393
	private void DeselectControlGroup()
	{
		if (this.m_selectedGroupButton != null)
		{
			this.m_selectedGroupButton.Select(false);
			this.m_selectedGroupButton = null;
		}
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x001041B8 File Offset: 0x001023B8
	private void Clear()
	{
		for (int i = 1; i <= this.m_controlGroupButtons.Count; i++)
		{
			this.m_controlGroupButtons[i].onClick -= this.ControlGroupButton_OnClick;
		}
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x001041F8 File Offset: 0x001023F8
	private void ControlGroupButton_OnClick(UIBattleViewControlGroup button)
	{
		this.m_ui.SelectControlGroup(button.transform.GetSiblingIndex() + 1);
		if (Time.unscaledTime - this.m_lastPressTimestamp < this.m_ui.dblclk_delay)
		{
			this.m_ui.CenterCameraOnSelection();
		}
		this.m_lastPressTimestamp = Time.unscaledTime;
	}

	// Token: 0x0400119F RID: 4511
	private BattleViewUI m_ui;

	// Token: 0x040011A0 RID: 4512
	private UIBattleViewControlGroup m_selectedGroupButton;

	// Token: 0x040011A1 RID: 4513
	private Dictionary<int, UIBattleViewControlGroup> m_controlGroupButtons = new Dictionary<int, UIBattleViewControlGroup>();

	// Token: 0x040011A2 RID: 4514
	private float m_lastPressTimestamp = float.PositiveInfinity;
}
