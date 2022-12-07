using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A4 RID: 420
public class UISquadStatusBar : MonoBehaviour, IListener, IPoolable
{
	// Token: 0x060017B4 RID: 6068 RVA: 0x000E87E5 File Offset: 0x000E69E5
	protected void LateUpdate()
	{
		if (this.m_logic == null)
		{
			return;
		}
		this.UpdateStats();
		this.CheckTooltipToggleChanged(false);
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x000E8800 File Offset: 0x000E6A00
	public void SetSquad(Logic.Squad squad)
	{
		if (squad == this.m_logic)
		{
			return;
		}
		if (this.m_logic != null)
		{
			this.RemoveListeners(this.m_logic);
		}
		this.m_logic = squad;
		this.Visuals = (this.m_logic.visuals as global::Squad);
		if (this.m_logic.def.is_siege_eq)
		{
			this.m_windowDefinition = global::Defs.GetDefField("SquadSiegeStatusBar", null);
		}
		else
		{
			this.m_windowDefinition = global::Defs.GetDefField("SquadStatusBar", null);
		}
		this.Init();
		if (this.m_Morale != null)
		{
			this.m_Morale.SetSquad(squad);
		}
		this.AddListeners(squad);
		this.Refresh();
		this.CheckTooltipToggleChanged(true);
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x000E88B2 File Offset: 0x000E6AB2
	public void Refresh()
	{
		this.RefreshBackground();
		this.UpdateStance();
		this.UpdateStats();
		this.UpdateStatsColors();
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x000E88CC File Offset: 0x000E6ACC
	public void RefreshBackground()
	{
		bool flag = this.HealthVisible();
		bool flag2 = !this.pinnable;
		if (this.m_Background != null)
		{
			for (int i = 0; i < this.m_Background.Length; i++)
			{
				this.m_Background[i].SetActive(flag2 && flag && !this.m_logic.def.is_siege_eq);
			}
		}
		if (this.m_BackgroundSmall != null)
		{
			this.m_BackgroundSmall.SetActive(flag2 && !this.m_logic.def.is_siege_eq && !flag);
		}
		if (this.m_BackgroundSmallSiege != null)
		{
			this.m_BackgroundSmallSiege.SetActive(flag2 && this.m_logic.def.is_siege_eq && flag);
		}
		if (flag2)
		{
			if (flag)
			{
				this.m_Morale.transform.localPosition = this.m_MoralePosition;
				return;
			}
			if (this.m_MoraleSecondPosition != null)
			{
				this.m_Morale.transform.localPosition = this.m_MoraleSecondPosition.localPosition;
			}
		}
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x000E89DC File Offset: 0x000E6BDC
	public static DT.Field GetThresholdField(DT.Field levels_field, float val)
	{
		if (((levels_field != null) ? levels_field.children : null) == null || levels_field.children.Count == 0)
		{
			return null;
		}
		DT.Field result = null;
		int num = int.MinValue;
		for (int i = 0; i < levels_field.children.Count; i++)
		{
			DT.Field field = levels_field.children[i];
			if (field.children != null && field.children.Count != 0)
			{
				int @int = field.GetInt("threshold", null, 0, true, true, true, '.');
				if ((float)@int <= val && @int > num)
				{
					num = @int;
					result = field;
				}
			}
		}
		return result;
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x000E8A6B File Offset: 0x000E6C6B
	public void AddListeners(Logic.Squad logic)
	{
		if (logic == null)
		{
			return;
		}
		logic.AddListener(this);
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x000E8A78 File Offset: 0x000E6C78
	public void RemoveListeners(Logic.Squad logic)
	{
		if (logic == null)
		{
			return;
		}
		logic.DelListener(this);
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x000E8A85 File Offset: 0x000E6C85
	public void RefreshListeners()
	{
		if (this.m_logic == null)
		{
			return;
		}
		this.RemoveListeners(this.m_logic);
		this.AddListeners(this.m_logic);
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x000E8AA8 File Offset: 0x000E6CA8
	public void Release(bool set_visible = true)
	{
		this.RemoveListeners(this.m_logic);
		if (this.m_Morale != null)
		{
			this.m_Morale.Clear();
		}
		this.m_logic = null;
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x000E8AD8 File Offset: 0x000E6CD8
	public void ShowHealthBar(bool show)
	{
		this.m_isHealthVisible = show;
		if (this.m_logic == null)
		{
			if (this.m_MachineHealthContainer != null)
			{
				this.m_MachineHealthContainer.gameObject.SetActive(false);
			}
			if (this.m_HealthContainer != null)
			{
				this.m_HealthContainer.gameObject.SetActive(false);
			}
			return;
		}
		if (this.m_logic.def.is_siege_eq && this.m_MachineHealthContainer != null)
		{
			this.m_MachineHealthContainer.gameObject.SetActive(show && this.m_isHealthVisibleFilter);
		}
		else if (this.m_HealthContainer != null)
		{
			this.m_HealthContainer.gameObject.SetActive(this.m_isHealthVisible && this.m_isHealthVisibleFilter);
		}
		this.RefreshBackground();
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x000E8BA6 File Offset: 0x000E6DA6
	public bool HealthVisible()
	{
		return this.m_isHealthVisible && this.m_isHealthVisibleFilter;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x000E8BB8 File Offset: 0x000E6DB8
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

	// Token: 0x060017C0 RID: 6080 RVA: 0x000E8C36 File Offset: 0x000E6E36
	public bool StaminaVisible()
	{
		return this.m_isStaminaVisible && this.m_isStaminaVisibleFilter;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x000E8C48 File Offset: 0x000E6E48
	public void ShowMoraleBar(bool show)
	{
		if (this.m_Morale == null)
		{
			return;
		}
		if (this.m_logic == null)
		{
			this.m_Morale.gameObject.SetActive(false);
			return;
		}
		if (!this.m_logic.def.is_siege_eq)
		{
			this.m_Morale.gameObject.SetActive(show);
		}
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x000E8CA4 File Offset: 0x000E6EA4
	public void ShowSalvoBar(bool show)
	{
		if (this.m_SalvoContainer == null)
		{
			return;
		}
		if (this.m_logic == null)
		{
			this.m_SalvoContainer.gameObject.SetActive(false);
			return;
		}
		if (!this.m_logic.def.is_siege_eq)
		{
			this.m_SalvoContainer.gameObject.SetActive(show);
		}
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x000E8D00 File Offset: 0x000E6F00
	public void ShowDeploymentBar(bool show)
	{
		if (this.m_DeploymentContainer == null)
		{
			return;
		}
		if (this.m_logic == null || !this.pinnable)
		{
			this.m_DeploymentContainer.gameObject.SetActive(false);
			return;
		}
		if (this.m_logic.def.is_siege_eq)
		{
			this.m_DeploymentContainer.gameObject.SetActive(show);
		}
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x000E8D64 File Offset: 0x000E6F64
	public void UpdateHealthVisibilityFilter(bool show)
	{
		this.m_isHealthVisibleFilter = show;
		if (this.m_logic == null)
		{
			return;
		}
		if (this.m_logic.def.is_siege_eq && this.m_MachineHealthContainer != null)
		{
			this.ShowHealthBar(this.m_isHealthVisible);
			return;
		}
		if (this.m_HealthContainer != null)
		{
			this.ShowHealthBar(this.m_isHealthVisible);
		}
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x000E8DC8 File Offset: 0x000E6FC8
	public void UpdateStaminaVisibilityFilter(bool show)
	{
		this.m_isStaminaVisibleFilter = false;
		if (this.m_logic == null)
		{
			return;
		}
		if (!this.m_logic.def.is_siege_eq && this.m_StaminaContainer != null)
		{
			this.ShowStaminaBar(this.m_isStaminaVisible);
		}
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x000E8E08 File Offset: 0x000E7008
	public bool IsContentVisible()
	{
		if (this.m_logic == null)
		{
			return false;
		}
		if (this.m_logic.def.is_siege_eq)
		{
			return true;
		}
		if (!base.gameObject.activeSelf && this.m_Initialized)
		{
			return false;
		}
		if (this.m_SquadStatsContainer == null || !this.m_SquadStatsContainer.gameObject.activeSelf)
		{
			return false;
		}
		bool flag = this.m_HealthContainer != null && this.m_HealthContainer.gameObject.activeSelf && this.m_Health != null && this.m_Health.gameObject.activeSelf;
		bool flag2 = this.m_StaminaContainer != null && this.m_StaminaContainer.gameObject.activeSelf && this.m_Stamina != null && this.m_Stamina.gameObject.activeSelf;
		bool flag3 = this.m_Morale != null && this.m_Morale.gameObject.activeSelf;
		return flag || flag2 || flag3;
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x000E8F1B File Offset: 0x000E711B
	public void OnPoolDeactivated()
	{
		this.Release(true);
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x000E8F24 File Offset: 0x000E7124
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "players_changed" || message == "kingdom_changed" || message == "stance_changed")
		{
			this.UpdateStance();
			return;
		}
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x000E8F54 File Offset: 0x000E7154
	public void Clear()
	{
		this.m_Initialized = false;
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x000E8F60 File Offset: 0x000E7160
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.m_statsDefinition = global::Defs.GetDefField("SquadStatusColors", null);
		UICommon.FindComponents(this, false);
		if (this.m_SquadStatsContainer != null)
		{
			this.m_SquadStatsContainer.SetActive(!this.m_logic.def.is_siege_eq);
		}
		if (this.m_SiegeMachineStatsContainer != null)
		{
			this.m_SiegeMachineStatsContainer.SetActive(this.m_logic.def.is_siege_eq);
		}
		if (this.m_DeploymentContainer != null)
		{
			this.m_DeploymentContainer.gameObject.SetActive(this.m_logic.def.name.Equals("Trebuchet"));
		}
		if (this.m_SalvoContainer != null && !this.m_logic.def.is_ranged)
		{
			this.m_SalvoContainer.gameObject.SetActive(false);
		}
		if (this.m_logic.def.is_ranged)
		{
			this.m_highGroundBuff = this.m_logic.high_ground_buff;
		}
		if (this.m_Morale != null)
		{
			this.m_MoralePosition = this.m_Morale.transform.localPosition;
		}
		this.m_Initialized = true;
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x000E909C File Offset: 0x000E729C
	private void UpdateStatsColors()
	{
		if (this.m_Stamina != null)
		{
			this.m_Stamina.color = global::Defs.GetColor(this.m_statsDefinition, "stamina", null);
		}
		if (this.m_Deployment != null)
		{
			this.m_Deployment.color = global::Defs.GetColor(this.m_statsDefinition, "siege_machine_deployment", null);
		}
		if (this.m_SalvoCapacity != null)
		{
			this.m_SalvoCapacity.color = global::Defs.GetColor(this.m_statsDefinition, "ammunition", null);
		}
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x000E9128 File Offset: 0x000E7328
	private void UpdateStance()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		string value = null;
		if (kingdom.IsEnemy(this.m_logic))
		{
			Logic.Kingdom kingdom2 = this.m_logic.GetKingdom();
			War war = kingdom.FindWarWith(this.m_logic.GetKingdom());
			if (war != null && !war.IsLeader(kingdom2))
			{
				value = "EnemySupporter";
			}
			else
			{
				value = "EnemyLeader";
			}
		}
		else if (kingdom.IsOwnStance(this.m_logic))
		{
			value = "Mine";
		}
		else if (kingdom.IsNeutral(this.m_logic))
		{
			value = "Ally";
		}
		else if (kingdom.IsAlly(this.m_logic))
		{
			value = "Ally";
		}
		string.IsNullOrEmpty(value);
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x000E91CC File Offset: 0x000E73CC
	private void UpdateLevel(UISquadStatusBar.ArmyInfo info, TextMeshProUGUI text_field)
	{
		int last_level = info.last_level;
		if (text_field == null)
		{
			return;
		}
		DT.Field thresholdField = UISquadStatusBar.GetThresholdField(global::Defs.GetDefField("ArmyStatusBar", "Levels"), (float)last_level);
		if (thresholdField == null)
		{
			return;
		}
		Color color = global::Defs.GetColor(thresholdField, "col", null);
		text_field.color = color;
		if (last_level <= 0)
		{
			UIText.SetText(text_field, "-");
			return;
		}
		UIText.SetText(text_field, last_level.ToString());
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x000E9238 File Offset: 0x000E7438
	private void UpdateStats()
	{
		if (this.m_logic.def.is_siege_eq)
		{
			this.UpdateSiegeEquipmentHealthBar();
			this.UpdateSiegeEquipmentDeployBar();
			return;
		}
		this.UpdateSquadHealthBar();
		this.UpdateStaminaBar();
		if (this.m_logic.def.is_ranged)
		{
			this.UpdateSalvoBar();
		}
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x000E9288 File Offset: 0x000E7488
	private unsafe void UpdateSiegeEquipmentHealthBar()
	{
		if (!(this.Visuals == null))
		{
			Logic.Squad logic = this.m_logic;
			if (((logic != null) ? logic.def : null) != null && this.Visuals.GetID() != -1)
			{
				this.m_MachineHealth.fillAmount = this.Visuals.data->health / this.m_logic.def.max_health;
				return;
			}
		}
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x000E92F4 File Offset: 0x000E74F4
	private void UpdateSiegeEquipmentDeployBar()
	{
		if (this.m_Deployment == null || this.m_logic == null)
		{
			return;
		}
		this.m_Deployment.fillAmount = (this.m_logic.is_packed ? this.m_logic.PackProgressNormalized() : (1f - this.m_logic.PackProgressNormalized()));
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x000E9350 File Offset: 0x000E7550
	private void UpdateSquadHealthBar()
	{
		if (this.m_Health != null)
		{
			global::Squad visuals = this.Visuals;
			bool flag;
			if (visuals == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.Squad logic = visuals.logic;
				flag = (((logic != null) ? logic.simulation : null) != null);
			}
			if (flag)
			{
				this.m_Health.fillAmount = (1f - this.Visuals.logic.simulation.damage) / this.Visuals.logic.simulation.max_damage;
			}
		}
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x000E93C8 File Offset: 0x000E75C8
	private void UpdateStaminaBar()
	{
		if (this.m_Stamina != null && this.Visuals != null && this.Visuals.logic != null)
		{
			this.m_Stamina.fillAmount = this.Visuals.logic.GetStamina() / this.Visuals.logic.MaxStamina();
		}
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x000E942C File Offset: 0x000E762C
	private void UpdateSalvoBar()
	{
		if (this.m_SalvoCapacity != null && this.Visuals != null && this.Visuals.logic != null)
		{
			this.m_SalvoCapacity.fillAmount = (float)this.m_logic.salvos_left / (float)this.m_logic.max_salvos;
			if (this.m_logic.ranged_enemy == null)
			{
				this.m_SalvoCapacity.color = this.GetSalvoColor("idle");
				return;
			}
			if (this.m_highGroundBuff != null)
			{
				float cthshootMod = this.m_highGroundBuff.GetCTHShootMod();
				if (cthshootMod == 0f)
				{
					this.m_SalvoCapacity.color = this.GetSalvoColor("no_advantage");
					return;
				}
				if (cthshootMod > 0f)
				{
					this.m_SalvoCapacity.color = this.GetSalvoColor("advantage");
					return;
				}
				this.m_SalvoCapacity.color = this.GetSalvoColor("disadvantage");
			}
		}
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x000E951B File Offset: 0x000E771B
	private Color GetSalvoColor(string archer_buff)
	{
		return global::Defs.GetColor(this.m_statsDefinition.FindChild("Salvo", null, true, true, true, '.').FindChild(archer_buff, null, true, true, true, '.'), "salvo_capacity_color", null);
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x000E954A File Offset: 0x000E774A
	private void OnClick(BSGButton b)
	{
		if (this.m_logic != null)
		{
			BaseUI.Get().SelectObjFromLogic(this.m_logic, false, true);
		}
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x000E9568 File Offset: 0x000E7768
	public void CheckTooltipToggleChanged(bool force = false)
	{
		if (this.Visuals == null)
		{
			return;
		}
		bool nameplateTooltipFilter = this.Visuals.m_NameplateTooltipFilter;
		bool flag = this.last_tooltip_enabled == 1;
		if (nameplateTooltipFilter == flag && this.last_tooltip_enabled != -1 && !force)
		{
			return;
		}
		this.last_tooltip_enabled = (nameplateTooltipFilter ? 1 : 0);
		this.ToggleTooltip(nameplateTooltipFilter);
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x000E95C0 File Offset: 0x000E77C0
	public void ToggleTooltip(bool isFilterOn)
	{
		GameObject obj = this.m_HealthContainer;
		if (this.m_logic.def.is_siege_eq)
		{
			obj = this.m_MachineHealthContainer.gameObject;
		}
		if (this.pinnable)
		{
			Tooltip tooltip = Tooltip.Get(obj, true);
			string def_id = "SquadHealthTooltip";
			Logic.Squad logic = this.m_logic;
			tooltip.SetDef(def_id, new Vars((logic != null) ? logic.simulation : null));
			return;
		}
		if (isFilterOn)
		{
			Tooltip tooltip2 = Tooltip.Get(obj, true);
			string def_id2 = "SquadHealthTooltipUnPinable";
			Logic.Squad logic2 = this.m_logic;
			tooltip2.SetDef(def_id2, new Vars((logic2 != null) ? logic2.simulation : null));
			return;
		}
		Tooltip tooltip3 = Tooltip.Get(obj, false);
		if (tooltip3 != null)
		{
			global::Common.DestroyObj(tooltip3);
		}
	}

	// Token: 0x04000F46 RID: 3910
	[UIFieldTarget("id_Background", findMultiple = true)]
	private GameObject[] m_Background;

	// Token: 0x04000F47 RID: 3911
	[UIFieldTarget("id_BackgroundSmall")]
	private GameObject m_BackgroundSmall;

	// Token: 0x04000F48 RID: 3912
	[UIFieldTarget("id_BackgroundSmallSiege")]
	private GameObject m_BackgroundSmallSiege;

	// Token: 0x04000F49 RID: 3913
	[UIFieldTarget("id_SquadStatsContainer")]
	private GameObject m_SquadStatsContainer;

	// Token: 0x04000F4A RID: 3914
	[UIFieldTarget("id_SiegeMachineStatsContainer")]
	private GameObject m_SiegeMachineStatsContainer;

	// Token: 0x04000F4B RID: 3915
	[UIFieldTarget("id_Morale")]
	private UISquadMorale m_Morale;

	// Token: 0x04000F4C RID: 3916
	[UIFieldTarget("id_StaminaContainer")]
	private GameObject m_StaminaContainer;

	// Token: 0x04000F4D RID: 3917
	[UIFieldTarget("id_Stamina")]
	private Image m_Stamina;

	// Token: 0x04000F4E RID: 3918
	[UIFieldTarget("id_HealthContainer")]
	private GameObject m_HealthContainer;

	// Token: 0x04000F4F RID: 3919
	[UIFieldTarget("id_Health")]
	private Image m_Health;

	// Token: 0x04000F50 RID: 3920
	[UIFieldTarget("id_MachineHealthContainer")]
	private Image m_MachineHealthContainer;

	// Token: 0x04000F51 RID: 3921
	[UIFieldTarget("id_MachineHealth")]
	private Image m_MachineHealth;

	// Token: 0x04000F52 RID: 3922
	[UIFieldTarget("id_DeploymentContainer")]
	private Image m_DeploymentContainer;

	// Token: 0x04000F53 RID: 3923
	[UIFieldTarget("id_Deployment")]
	private Image m_Deployment;

	// Token: 0x04000F54 RID: 3924
	[UIFieldTarget("id_SalvoContainer")]
	private Image m_SalvoContainer;

	// Token: 0x04000F55 RID: 3925
	[UIFieldTarget("id_SalvoCapacity")]
	private Image m_SalvoCapacity;

	// Token: 0x04000F56 RID: 3926
	[UIFieldTarget("id_MoraleSecondPosition")]
	private Transform m_MoraleSecondPosition;

	// Token: 0x04000F57 RID: 3927
	private bool m_isHealthVisibleFilter = true;

	// Token: 0x04000F58 RID: 3928
	private bool m_isStaminaVisibleFilter = true;

	// Token: 0x04000F59 RID: 3929
	private bool m_isHealthVisible = true;

	// Token: 0x04000F5A RID: 3930
	private bool m_isStaminaVisible = true;

	// Token: 0x04000F5B RID: 3931
	private bool m_Initialized;

	// Token: 0x04000F5C RID: 3932
	private Logic.Squad m_logic;

	// Token: 0x04000F5D RID: 3933
	private global::Squad Visuals;

	// Token: 0x04000F5E RID: 3934
	private DT.Field m_windowDefinition;

	// Token: 0x04000F5F RID: 3935
	private DT.Field m_statsDefinition;

	// Token: 0x04000F60 RID: 3936
	private SquadHighGroundBuff m_highGroundBuff;

	// Token: 0x04000F61 RID: 3937
	private Vector3 m_MoralePosition;

	// Token: 0x04000F62 RID: 3938
	public bool pinnable = true;

	// Token: 0x04000F63 RID: 3939
	private int last_tooltip_enabled = -1;

	// Token: 0x020006D2 RID: 1746
	public class ArmyInfo
	{
		// Token: 0x04003730 RID: 14128
		public Logic.Settlement settlement;

		// Token: 0x04003731 RID: 14129
		public Logic.Kingdom kingdom;

		// Token: 0x04003732 RID: 14130
		public global::Army army;

		// Token: 0x04003733 RID: 14131
		public int last_level = -1;

		// Token: 0x04003734 RID: 14132
		public float last_unit_level = -1f;
	}
}
