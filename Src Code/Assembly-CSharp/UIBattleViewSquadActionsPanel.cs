using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200019E RID: 414
public class UIBattleViewSquadActionsPanel : MonoBehaviour
{
	// Token: 0x0600177F RID: 6015 RVA: 0x000E729B File Offset: 0x000E549B
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x000E72A3 File Offset: 0x000E54A3
	private void OnValidate()
	{
		this.UpdateLayoutStyle();
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x000E72AB File Offset: 0x000E54AB
	public void Setup(Vars vars = null)
	{
		this.m_vars = vars;
		this.Initialize();
		this.Refresh();
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x000E72C0 File Offset: 0x000E54C0
	public void Refresh()
	{
		if (!this.m_isInitialized)
		{
			return;
		}
		this.UpdateVisibility();
		foreach (UIBattleViewSquadActionButton uibattleViewSquadActionButton in this.m_squadActionButtons)
		{
			uibattleViewSquadActionButton.Refresh();
		}
		foreach (KeyValuePair<string, ButtonsGroup> keyValuePair in this.m_buttonGroups)
		{
			keyValuePair.Value.Refresh();
		}
		this.UpdateLayoutStyle();
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x000023FD File Offset: 0x000005FD
	public void Clear()
	{
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x000E736C File Offset: 0x000E556C
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		this.m_BattleViewSquadActions = BattleViewUI.Get().SquadActions;
		UICommon.FindComponents(this, false);
		this.InitializeDefinition();
		UICommon.DeleteChildren(base.transform);
		this.SetupButtons();
		this.m_isInitialized = true;
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x000E73AC File Offset: 0x000E55AC
	private void InitializeDefinition()
	{
		if (this.m_definition == null)
		{
			this.m_definition = global::Defs.GetDefField("UISquadActionsPanel", null);
		}
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x000E73C8 File Offset: 0x000E55C8
	private void SetupButtons()
	{
		List<DT.Field> list = this.m_definition.FindChild("Buttons", null, true, true, true, '.').Children();
		for (int i = 0; i < list.Count; i++)
		{
			if (!string.IsNullOrEmpty(list[i].key) && list[i] != null)
			{
				UIBattleViewSquadActionButton button = this.GetButton(i);
				if (!(button == null))
				{
					button.Setup(list[i]);
					string @string = list[i].GetString("group_id", null, "", true, true, true, '.');
					if (!string.IsNullOrEmpty(@string))
					{
						ButtonsGroup buttonsGroup = null;
						if (!this.m_buttonGroups.TryGetValue(@string, out buttonsGroup))
						{
							buttonsGroup = new ButtonsGroup();
							this.m_buttonGroups.Add(@string, buttonsGroup);
						}
						button.SetGroup(buttonsGroup);
					}
				}
			}
		}
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x000E7498 File Offset: 0x000E5698
	public void UpdateVisibility()
	{
		for (int i = 0; i < this.m_squadActionButtons.Count; i++)
		{
			this.m_squadActionButtons[i].SetActive(this.CanButtonBeDisplayed(this.m_squadActionButtons[i].GetDefinition()));
		}
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x000E74E4 File Offset: 0x000E56E4
	private UIBattleViewSquadActionButton GetButton(int index)
	{
		if (index >= 0 && index < this.m_squadActionButtons.Count)
		{
			return this.m_squadActionButtons[index];
		}
		UIBattleViewSquadActionButton component = global::Common.Spawn(global::Defs.GetObj<GameObject>(this.m_definition, "button_prefab", null), base.transform, false, "").GetComponent<UIBattleViewSquadActionButton>();
		this.m_squadActionButtons.Add(component);
		return component;
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x000E7548 File Offset: 0x000E5748
	private bool CanButtonBeDisplayed(DT.Field buttonDefinition)
	{
		return buttonDefinition.GetBool("is_visible", null, false, true, true, true, '.') && (this.m_BattleViewSquadActions.SelectedUnits <= 1 || buttonDefinition.GetBool("is_valid_for_multiple_squads", null, false, true, true, true, '.')) && (this.m_BattleViewSquadActions.AreRangedUnitsSelected || !buttonDefinition.GetBool("is_valid_only_for_ranged_squads", null, false, true, true, true, '.')) && (!this.m_BattleViewSquadActions.AreSiegeUnitsOnlySelected || buttonDefinition.GetBool("is_valid_for_siege_equipment_squads", null, false, true, true, true, '.')) && (this.m_BattleViewSquadActions.ArePackableUnitsOnlySelected || !buttonDefinition.GetBool("is_valid_only_for_deployable_squads", null, false, true, true, true, '.'));
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000E75FC File Offset: 0x000E57FC
	private void UpdateLayoutStyle()
	{
		int num = 0;
		float num2 = 0f;
		foreach (object obj in base.transform)
		{
			RectTransform rectTransform = (RectTransform)obj;
			if (rectTransform.gameObject.activeSelf)
			{
				num++;
				num2 += rectTransform.rect.width;
			}
		}
		HorizontalLayoutGroup component = base.GetComponent<HorizontalLayoutGroup>();
		if (component == null)
		{
			return;
		}
		if ((long)num < (long)((ulong)this.minimalChildCountToSpread))
		{
			component.childForceExpandWidth = false;
			component.childAlignment = TextAnchor.MiddleLeft;
			component.spacing = this.defaultSpacing;
			return;
		}
		RectTransform component2 = component.GetComponent<RectTransform>();
		component.childAlignment = TextAnchor.MiddleCenter;
		component.childForceExpandWidth = true;
		if (component2.rect.width > num2)
		{
			component.spacing = (component2.rect.width - num2) / (float)num;
			return;
		}
		component.spacing = this.defaultSpacing;
	}

	// Token: 0x04000F1A RID: 3866
	[SerializeField]
	private uint minimalChildCountToSpread = 8U;

	// Token: 0x04000F1B RID: 3867
	[SerializeField]
	private float defaultSpacing = 3.5f;

	// Token: 0x04000F1C RID: 3868
	private Vars m_vars;

	// Token: 0x04000F1D RID: 3869
	private DT.Field m_definition;

	// Token: 0x04000F1E RID: 3870
	private BattleViewSquadActions m_BattleViewSquadActions;

	// Token: 0x04000F1F RID: 3871
	private Dictionary<string, ButtonsGroup> m_buttonGroups = new Dictionary<string, ButtonsGroup>();

	// Token: 0x04000F20 RID: 3872
	private List<UIBattleViewSquadActionButton> m_squadActionButtons = new List<UIBattleViewSquadActionButton>();

	// Token: 0x04000F21 RID: 3873
	private bool m_isInitialized;
}
