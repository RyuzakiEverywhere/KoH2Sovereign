using System;
using System.Text.RegularExpressions;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001A5 RID: 421
public class UISquadTypeIcon : ObjectIcon
{
	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060017DC RID: 6108 RVA: 0x000E9698 File Offset: 0x000E7898
	// (remove) Token: 0x060017DD RID: 6109 RVA: 0x000E96D0 File Offset: 0x000E78D0
	public event Action<PointerEventData> OnSquadIconClicked;

	// Token: 0x060017DE RID: 6110 RVA: 0x000E9708 File Offset: 0x000E7908
	public override void SetObject(object obj, Vars vars = null)
	{
		if (this.logicObject != null && this.logicObject == obj)
		{
			return;
		}
		if (obj is Logic.Squad)
		{
			this.m_logic = (obj as Logic.Squad);
		}
		this.m_squadTypeIconSettings = global::Defs.GetDefField("SquadTypeIconSettings", null);
		base.SetObject(obj, vars);
		UICommon.FindComponents(this, false);
		if (this.m_logic != null)
		{
			if (this.m_Forest != null)
			{
				this.m_Forest.SetDef(this.m_logic.trees_buff, null);
			}
			if (this.m_Ladder != null)
			{
				this.m_Ladder.SetDef(this.m_logic.ladder_buff, null);
			}
		}
		this.InitializeSquadIcon();
		this.UpdateBattleStance();
		this.UpdateSquadExperienceStars();
		this.OnParentClampedChange(false);
		this.CheckTooltipToggleChanged(true);
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x000E97D0 File Offset: 0x000E79D0
	private void UpdateBonuses()
	{
		if (this.m_logic == null)
		{
			return;
		}
		if (this.m_Forest != null)
		{
			this.m_Forest.gameObject.SetActive(this.m_logic.trees_buff != null && this.m_logic.trees_buff.enabled);
		}
		if (this.m_Ladder != null)
		{
			this.m_Ladder.gameObject.SetActive(this.m_logic.climbing_buff != null && this.m_logic.climbing_buff.enabled);
		}
		Logic.Character character = null;
		if (this.m_logic.def.type == Logic.Unit.Type.Noble)
		{
			BattleSimulation.Squad simulation = this.m_logic.simulation;
			Logic.Character character2;
			if (simulation == null)
			{
				character2 = null;
			}
			else
			{
				Logic.Army army = simulation.army;
				character2 = ((army != null) ? army.leader : null);
			}
			character = character2;
		}
		if (this.m_CrownKing != null)
		{
			this.m_CrownKing.SetActive(character != null && character.IsKing());
		}
		if (this.m_CrownPrinceHeir != null)
		{
			this.m_CrownPrinceHeir.SetActive(character != null && character.IsHeir());
		}
		if (this.m_CrownPrince != null)
		{
			this.m_CrownPrince.SetActive(character != null && character.IsPrince() && !character.IsHeir());
		}
		if (this.m_Target != null)
		{
			this.m_Target.SetActive(this.m_logic.marked_as_target);
		}
		if (this.m_ArrowBonus != null && this.m_ArrowPenalty != null)
		{
			Logic.Squad squad = this.m_logic.ranged_enemy as Logic.Squad;
			bool flag = squad != null;
			bool flag2 = flag && this.m_logic.CanShoot(squad, true);
			float num = (this.m_logic.high_ground_buff == null) ? 0f : this.m_logic.high_ground_buff.GetCTHShootMod();
			bool flag3 = num < 0f || (this.m_logic.trees_buff != null && this.m_logic.trees_buff.enabled) || (flag && squad.trees_buff != null && squad.trees_buff.enabled);
			this.m_ArrowPenalty.SetActive(flag && flag2 && flag3);
			this.m_ArrowBonus.SetActive(flag && flag2 && !flag3 && num > 0f);
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x000E9A22 File Offset: 0x000E7C22
	public void OnParentClampedChange(bool clamped)
	{
		this._clamped = clamped;
		this.RefreshStaminaContainer(this._clamped);
		this.SetLeaderClamped(this._clamped);
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x000E9A44 File Offset: 0x000E7C44
	public void ShowStaminaBar(bool show)
	{
		this.m_isStaminaVisible = show;
		if (this.m_StaminaContainer == null)
		{
			return;
		}
		if (this.m_logic == null)
		{
			this.m_StaminaContainer.gameObject.SetActive(false);
			return;
		}
		if (!this.m_logic.def.is_siege_eq && this.m_StaminaContainer != null)
		{
			this.m_StaminaContainer.gameObject.SetActive(this.m_isStaminaVisible && this.m_isStaminaVisibleFilter);
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x000E9AC2 File Offset: 0x000E7CC2
	public bool StaminaVisible()
	{
		return this.m_isStaminaVisible && this.m_isStaminaVisibleFilter && this.m_StaminaContainer != null && this.m_StaminaContainer.activeSelf;
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x000E9AEF File Offset: 0x000E7CEF
	public bool DeploymentVisible()
	{
		return this.m_DeploymentContainer.activeSelf;
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x000E9AFC File Offset: 0x000E7CFC
	public void UpdateStaminaVisibilityFilter(bool show)
	{
		this.m_isStaminaVisibleFilter = show;
		if (this.m_logic == null)
		{
			return;
		}
		if (!this.m_logic.def.is_siege_eq && this.m_StaminaContainer != null)
		{
			this.ShowStaminaBar(this.m_isStaminaVisible);
			this.OnParentClampedChange(this._clamped);
		}
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x000E9B54 File Offset: 0x000E7D54
	public void RefreshStaminaContainer(bool clamped)
	{
		if (this.m_StaminaContainer != null)
		{
			this.m_StaminaContainer.SetActive(this.m_logic != null && this.m_logic.def.type != Logic.Unit.Type.InventoryItem && !clamped && this.m_isStaminaVisibleFilter);
		}
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x000E9BA4 File Offset: 0x000E7DA4
	public void RefreshDeploymentContainer(bool clamped)
	{
		if (this.m_DeploymentContainer != null)
		{
			this.m_DeploymentContainer.SetActive(this.m_logic != null && this.m_logic.def.type == Logic.Unit.Type.InventoryItem && !clamped && this.m_logic.IsPacking());
		}
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x000E9BF6 File Offset: 0x000E7DF6
	public void SetLeaderClamped(bool clamped)
	{
		if (this.m_leaderIcon != null)
		{
			this.m_leaderIcon.SetSemiTransparent(clamped);
		}
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x000E9C12 File Offset: 0x000E7E12
	public void ForceToShowUnitTypeIcon(bool force)
	{
		this.m_IsUnitTypeIconForced = force;
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x000E9C1B File Offset: 0x000E7E1B
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		Action<PointerEventData> onSquadIconClicked = this.OnSquadIconClicked;
		if (onSquadIconClicked == null)
		{
			return;
		}
		onSquadIconClicked(e);
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x000E9C35 File Offset: 0x000E7E35
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		Action<PointerEventData> onSquadIconClicked = this.OnSquadIconClicked;
		if (onSquadIconClicked == null)
		{
			return;
		}
		onSquadIconClicked(e);
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x000E9C4F File Offset: 0x000E7E4F
	public override void OnRightClick(PointerEventData e)
	{
		base.OnRightClick(e);
		Action<PointerEventData> onSquadIconClicked = this.OnSquadIconClicked;
		if (onSquadIconClicked == null)
		{
			return;
		}
		onSquadIconClicked(e);
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x000E9C6C File Offset: 0x000E7E6C
	public override DT.Field GetCursorFieldKey(DT.Field field)
	{
		Logic.Squad squad = BattleViewUI.Get().selected_logic_obj as Logic.Squad;
		Logic.Squad logic = this.m_logic;
		return BattleViewUI.SquadToSquadCursorField(squad, logic, field);
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x000E9C98 File Offset: 0x000E7E98
	private void UpdateBattleStance()
	{
		string stance = this.GetStance();
		this.m_UnitTypeBackground.sprite = global::Defs.GetObj<Sprite>(this.m_squadTypeIconSettings.FindChild(stance, null, true, true, true, '.'), "unitTypeBackground", null);
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x000E9CD4 File Offset: 0x000E7ED4
	private string GetStance()
	{
		string result = string.Empty;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom.IsEnemy(this.m_logic))
		{
			Logic.Kingdom kingdom2 = this.m_logic.GetKingdom();
			War war = kingdom.FindWarWith(this.m_logic.GetKingdom());
			if (war != null && !war.IsLeader(kingdom2))
			{
				result = "EnemySupporter";
			}
			else
			{
				result = "EnemyLeader";
			}
		}
		else if (kingdom.IsOwnStance(this.m_logic) && !this.m_logic.IsMercenary())
		{
			result = "Mine";
		}
		else if (kingdom.IsAlly(this.m_logic))
		{
			result = "Ally";
		}
		else
		{
			result = "Ally";
		}
		return result;
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x000E9D78 File Offset: 0x000E7F78
	private void InitializeSquadIcon()
	{
		if (this.m_UnitTypeIcon != null)
		{
			Logic.Army army = this.m_logic.simulation.army;
			Logic.Character character = (army != null) ? army.leader : null;
			if (this.m_logic.def.type == Logic.Unit.Type.Noble && !this.m_IsUnitTypeIconForced && character != null)
			{
				if (this.m_LeaderIconContainer.childCount == 0)
				{
					Vars vars = new Vars();
					vars.Set<string>("variant", "squad_leader");
					vars.Set<string>("stance", this.GetStance());
					this.m_leaderIcon = ObjectIcon.GetIcon(character, vars, this.m_LeaderIconContainer).GetComponent<UICharacterIcon>();
					this.m_leaderIcon.DisableTooltip(true);
					UICommon.FitInParent(this.m_leaderIcon.transform as RectTransform);
					this.m_leaderIcon.ShowCrest(false);
					this.m_leaderIcon.ShowStatus(false);
					this.m_leaderIcon.ShowCrown(false);
					this.m_leaderIcon.EnableClassLevel(false);
					this.m_leaderIcon.OnPointerEvent += this.LeaderIcon_OnPointerEvent;
				}
				this.m_LeaderIconContainer.gameObject.SetActive(true);
				this.m_UnitTypeBackground.gameObject.SetActive(false);
				return;
			}
			DT.Field defField = global::Defs.GetDefField(this.m_logic.def.name, null);
			string text;
			if (this.m_logic.def.is_guard)
			{
				text = Regex.Replace(defField.GetString("secondary_type", null, "", true, true, true, '.'), "\\s", "");
			}
			else if (this.m_logic.def.is_siege_eq)
			{
				text = this.m_logic.def.name;
			}
			else
			{
				text = Regex.Replace(defField.GetString("unit_type", null, "", true, true, true, '.'), "\\s", "");
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "Default";
			}
			this.m_UnitTypeIcon.sprite = global::Defs.GetObj<Sprite>(this.m_squadTypeIconSettings, text, null);
			this.m_LeaderIconContainer.gameObject.SetActive(false);
			this.m_UnitTypeBackground.gameObject.SetActive(true);
		}
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x000E9F9F File Offset: 0x000E819F
	public void UpdateLevelVisibilityFilter(bool isFilterOn)
	{
		if (this.m_leaderIcon != null)
		{
			this.m_leaderIcon.UpdateLevelVisibilityFilter(false);
		}
		RectTransform experienceLevelContainer = this.m_ExperienceLevelContainer;
		if (experienceLevelContainer == null)
		{
			return;
		}
		experienceLevelContainer.gameObject.SetActive(isFilterOn);
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x000E9FD1 File Offset: 0x000E81D1
	private void LeaderIcon_OnPointerEvent(Hotspot arg1, EventTriggerType arg2, PointerEventData arg3)
	{
		if (arg2 == EventTriggerType.PointerClick)
		{
			base.OnClick(arg3);
			Action<PointerEventData> onSquadIconClicked = this.OnSquadIconClicked;
			if (onSquadIconClicked == null)
			{
				return;
			}
			onSquadIconClicked(arg3);
		}
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x000E9FF0 File Offset: 0x000E81F0
	private void UpdateSquadExperienceStars()
	{
		if (this.m_logic != null && this.m_ExperienceLevelContainer != null)
		{
			int num = this.m_logic.simulation.unit.level;
			if (this.m_logic.def.type == Logic.Unit.Type.Noble)
			{
				Logic.Army army = this.m_logic.simulation.army;
				Logic.Character c = (army != null) ? army.leader : null;
				num = this.GetSkillAtMaxLevelCount(c);
			}
			for (int i = 0; i < this.m_ExperienceLevelContainer.transform.childCount; i++)
			{
				this.m_ExperienceLevelContainer.GetChild(i).gameObject.SetActive(i == num);
			}
		}
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x000EA09C File Offset: 0x000E829C
	private int GetSkillAtMaxLevelCount(Logic.Character c)
	{
		if (c == null)
		{
			return 0;
		}
		if (c.skills == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < c.skills.Count; i++)
		{
			Skill skill = c.skills[i];
			if (skill != null && c.GetSkillRank(skill) == skill.MaxRank())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x000EA0F4 File Offset: 0x000E82F4
	private void Update()
	{
		this.RefreshDeploymentContainer(this._clamped);
		this.UpdateStaminaBar();
		this.UpdateDeploymentBar();
		this.UpdateBonuses();
		if (!this.pinnable)
		{
			this.CheckTooltipToggleChanged(false);
			int num = this.last_tooltip_enabled;
			return;
		}
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x000EA130 File Offset: 0x000E8330
	private void UpdateStaminaBar()
	{
		if (this.m_StaminaBar != null && this.m_logic != null && this.m_StaminaContainer.activeSelf)
		{
			this.m_StaminaBar.fillAmount = this.m_logic.GetStamina() / this.m_logic.MaxStamina();
		}
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x000EA182 File Offset: 0x000E8382
	private void UpdateDeploymentBar()
	{
		if (this.m_DeploymentBar != null && this.m_logic != null && this.m_DeploymentContainer.activeSelf)
		{
			this.m_DeploymentBar.fillAmount = this.m_logic.PackProgressNormalized();
		}
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x000EA1C0 File Offset: 0x000E83C0
	public void CheckTooltipToggleChanged(bool force = false)
	{
		Logic.Squad logic = this.m_logic;
		global::Squad squad = ((logic != null) ? logic.visuals : null) as global::Squad;
		if (squad == null)
		{
			return;
		}
		bool nameplateTooltipFilter = squad.m_NameplateTooltipFilter;
		bool flag = this.last_tooltip_enabled == 1;
		if (nameplateTooltipFilter == flag && this.last_tooltip_enabled != -1 && !force)
		{
			return;
		}
		this.last_tooltip_enabled = (nameplateTooltipFilter ? 1 : 0);
		this.ToggleShootBuffTooltip(nameplateTooltipFilter);
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x000EA228 File Offset: 0x000E8428
	private void ToggleShootBuffTooltip(bool enabled)
	{
		Vars vars = new Vars(this.m_logic.simulation);
		if (this.pinnable)
		{
			Tooltip tooltip = Tooltip.Get(this.m_ArrowBonus, true);
			if (tooltip != null)
			{
				tooltip.SetDef("SquadShootBuffsTooltip", vars);
			}
			Tooltip tooltip2 = Tooltip.Get(this.m_ArrowPenalty, true);
			if (tooltip2 == null)
			{
				return;
			}
			tooltip2.SetDef("SquadShootBuffsTooltip", vars);
			return;
		}
		else
		{
			if (!enabled)
			{
				Tooltip tooltip3 = Tooltip.Get(this.m_ArrowBonus, false);
				if (tooltip3 != null)
				{
					global::Common.DestroyObj(tooltip3);
				}
				tooltip3 = Tooltip.Get(this.m_ArrowPenalty, false);
				if (tooltip3 != null)
				{
					global::Common.DestroyObj(tooltip3);
				}
				return;
			}
			Tooltip tooltip4 = Tooltip.Get(this.m_ArrowBonus, true);
			if (tooltip4 != null)
			{
				tooltip4.SetDef("SquadShootBuffsTooltipUnpinnable", vars);
			}
			Tooltip tooltip5 = Tooltip.Get(this.m_ArrowPenalty, true);
			if (tooltip5 == null)
			{
				return;
			}
			tooltip5.SetDef("SquadShootBuffsTooltipUnpinnable", vars);
			return;
		}
	}

	// Token: 0x04000F65 RID: 3941
	[UIFieldTarget("id_UnitTypeBackground")]
	private Image m_UnitTypeBackground;

	// Token: 0x04000F66 RID: 3942
	[UIFieldTarget("id_UnitTypeIcon")]
	private Image m_UnitTypeIcon;

	// Token: 0x04000F67 RID: 3943
	[UIFieldTarget("id_ExperienceLevelContainer")]
	private RectTransform m_ExperienceLevelContainer;

	// Token: 0x04000F68 RID: 3944
	[UIFieldTarget("id_LeaderIconContainer")]
	private RectTransform m_LeaderIconContainer;

	// Token: 0x04000F69 RID: 3945
	[UIFieldTarget("id_StaminaContainer")]
	private GameObject m_StaminaContainer;

	// Token: 0x04000F6A RID: 3946
	[UIFieldTarget("id_StaminaBar")]
	private Image m_StaminaBar;

	// Token: 0x04000F6B RID: 3947
	[UIFieldTarget("id_DeploymentContainer")]
	private GameObject m_DeploymentContainer;

	// Token: 0x04000F6C RID: 3948
	[UIFieldTarget("id_DeploymentBar")]
	private Image m_DeploymentBar;

	// Token: 0x04000F6D RID: 3949
	[UIFieldTarget("id_Forest")]
	private UISquadStatus m_Forest;

	// Token: 0x04000F6E RID: 3950
	[UIFieldTarget("id_CrownKing")]
	private GameObject m_CrownKing;

	// Token: 0x04000F6F RID: 3951
	[UIFieldTarget("id_CrownPrince")]
	private GameObject m_CrownPrince;

	// Token: 0x04000F70 RID: 3952
	[UIFieldTarget("id_CrownPrinceHeir")]
	private GameObject m_CrownPrinceHeir;

	// Token: 0x04000F71 RID: 3953
	[UIFieldTarget("id_Target")]
	private GameObject m_Target;

	// Token: 0x04000F72 RID: 3954
	[UIFieldTarget("id_Ladder")]
	private UISquadStatus m_Ladder;

	// Token: 0x04000F73 RID: 3955
	[UIFieldTarget("id_ArrowBonus")]
	private GameObject m_ArrowBonus;

	// Token: 0x04000F74 RID: 3956
	[UIFieldTarget("id_ArrowPenalty")]
	private GameObject m_ArrowPenalty;

	// Token: 0x04000F75 RID: 3957
	private bool m_IsUnitTypeIconForced;

	// Token: 0x04000F76 RID: 3958
	private bool m_isStaminaVisible = true;

	// Token: 0x04000F77 RID: 3959
	private bool m_isStaminaVisibleFilter = true;

	// Token: 0x04000F78 RID: 3960
	private Logic.Squad m_logic;

	// Token: 0x04000F79 RID: 3961
	private DT.Field m_squadTypeIconSettings;

	// Token: 0x04000F7A RID: 3962
	private UICharacterIcon m_leaderIcon;

	// Token: 0x04000F7B RID: 3963
	private bool _clamped;

	// Token: 0x04000F7C RID: 3964
	public bool pinnable = true;

	// Token: 0x04000F7D RID: 3965
	private int last_tooltip_enabled = -1;
}
