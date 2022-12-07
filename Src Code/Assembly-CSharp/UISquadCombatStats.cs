using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DC RID: 476
public class UISquadCombatStats : MonoBehaviour
{
	// Token: 0x06001C4D RID: 7245 RVA: 0x0010B424 File Offset: 0x00109624
	public void SetSquad(BattleSimulation.Squad logic, Vars vars = null)
	{
		this.m_logic = logic;
		this.m_vars = vars;
		this.Initialize();
		this.Refresh();
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x0010B440 File Offset: 0x00109640
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		this.m_windowDefinition = global::Defs.GetDefField("UISquadCombatStats", null);
		this.m_rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x0010B468 File Offset: 0x00109668
	public void Refresh()
	{
		if ((this.m_logic.def.is_ranged && this.m_logic.def.can_attack_melee) || (this.m_logic.def.is_siege_eq && this.m_logic.def.is_ranged))
		{
			this.UpdateField(this.m_AttackValue, this.m_AttackIcon, "CTH", "CTH_bonus", "CTH_ranged", "CTH_ranged_bonus", "AttackMultiStat");
		}
		else if (this.m_logic.def.is_ranged)
		{
			this.UpdateField(this.m_AttackValue, this.m_AttackIcon, "CTH_ranged", "CTH_ranged_bonus", "AttackStat");
		}
		else
		{
			this.UpdateField(this.m_AttackValue, this.m_AttackIcon, "CTH", "CTH_bonus", "AttackStat");
		}
		this.UpdateField(this.m_DefenseValue, this.m_DefenseIcon, "defense", "defense_bonus", "defense_against_ranged", "defense_against_ranged_bonus", "DefenseMultiStat");
		this.UpdateField(this.m_ShockDamageValue, this.m_ShockDamageIcon, "chance_to_shock_base", "chance_to_shock_bonus", "ShockDamageStat");
		this.UpdateField(this.m_ResilienceValue, this.m_ResilienceIcon, "resilience_base", "resilience_bonus", "ResilienceStat");
		this.UpdateField(this.m_MovementSpeedValue, this.m_MovementSpeedIcon, "move_speed", "move_speed_bonus", "MovementSpeedStat");
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x0010B5D0 File Offset: 0x001097D0
	private void UpdateField(TextMeshProUGUI text, Image icon, string propertyKey, string propertyBonusKey, string secondPropertyKey, string secondPropertyBonusKey, string tooltipDefKey)
	{
		if (this.m_logic == null || this.m_windowDefinition == null)
		{
			return;
		}
		float num = this.m_logic.GetVar(propertyKey, null, true);
		float num2 = this.m_logic.GetVar(secondPropertyKey, null, true);
		float num3 = this.m_logic.GetVar(propertyBonusKey, null, true);
		float num4 = this.m_logic.GetVar(secondPropertyBonusKey, null, true);
		if (text != null)
		{
			Vars vars = new Vars();
			vars.Set<float>("property", num + num3);
			vars.Set<float>("max", num2 + num4);
			string text2 = "";
			if (num3 > 0f)
			{
				text2 = ":bonus_no_sign";
			}
			else if (num3 < 0f)
			{
				text2 = ":penalty_no_sign";
			}
			string text3 = "";
			if (num4 > 0f)
			{
				text3 = ":bonus_no_sign";
			}
			else if (num4 < 0f)
			{
				text3 = ":penalty_no_sign";
			}
			text.text = global::Defs.GetLocalized(string.Concat(new string[]
			{
				"{property:F0",
				text2,
				"}/{max:F0",
				text3,
				"}"
			}), vars);
		}
		if (icon != null)
		{
			icon.sprite = global::Defs.GetObj<Sprite>(this.m_windowDefinition.FindChild(propertyKey + "_and_" + secondPropertyKey, null, true, true, true, '.'), "icon", null);
			Tooltip.Get(icon.gameObject, true).SetDef(tooltipDefKey, null);
		}
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x0010B748 File Offset: 0x00109948
	private void UpdateField(TextMeshProUGUI text, Image icon, string propertyKey, string propertyBonusKey, string tooltipDefKey)
	{
		if (this.m_logic == null || this.m_windowDefinition == null)
		{
			return;
		}
		float num = this.m_logic.GetVar(propertyBonusKey, null, true);
		if (text != null)
		{
			Vars vars = new Vars();
			vars.Set<float>("property", this.m_logic.GetVar(propertyKey, null, true) + num);
			string str = "";
			if (num > 0f)
			{
				str = ":bonus_no_sign";
			}
			else if (num < 0f)
			{
				str = ":penalty_no_sign";
			}
			text.text = global::Defs.GetLocalized("{property:F0" + str + "}", vars);
		}
		if (icon != null)
		{
			icon.sprite = global::Defs.GetObj<Sprite>(this.m_windowDefinition.FindChild(propertyKey, null, true, true, true, '.'), "icon", null);
			Tooltip.Get(icon.gameObject, true).SetDef(tooltipDefKey, null);
		}
	}

	// Token: 0x04001273 RID: 4723
	[UIFieldTarget("id_Attack")]
	private Image m_Attack;

	// Token: 0x04001274 RID: 4724
	[UIFieldTarget("id_AttackIcon")]
	private Image m_AttackIcon;

	// Token: 0x04001275 RID: 4725
	[UIFieldTarget("id_AttackValue")]
	private TextMeshProUGUI m_AttackValue;

	// Token: 0x04001276 RID: 4726
	[UIFieldTarget("id_Defense")]
	private Image id_Defense;

	// Token: 0x04001277 RID: 4727
	[UIFieldTarget("id_DefenseIcon")]
	private Image m_DefenseIcon;

	// Token: 0x04001278 RID: 4728
	[UIFieldTarget("id_DefenseValue")]
	private TextMeshProUGUI m_DefenseValue;

	// Token: 0x04001279 RID: 4729
	[UIFieldTarget("id_ShockDamage")]
	private Image id_ShockDamage;

	// Token: 0x0400127A RID: 4730
	[UIFieldTarget("id_ShockDamageIcon")]
	private Image m_ShockDamageIcon;

	// Token: 0x0400127B RID: 4731
	[UIFieldTarget("id_ShockDamageValue")]
	private TextMeshProUGUI m_ShockDamageValue;

	// Token: 0x0400127C RID: 4732
	[UIFieldTarget("id_Resilience")]
	private Image id_Resilience;

	// Token: 0x0400127D RID: 4733
	[UIFieldTarget("id_ResilienceIcon")]
	private Image m_ResilienceIcon;

	// Token: 0x0400127E RID: 4734
	[UIFieldTarget("id_ResilienceValue")]
	private TextMeshProUGUI m_ResilienceValue;

	// Token: 0x0400127F RID: 4735
	[UIFieldTarget("id_MovementSpeed")]
	private Image id_MovementSpeed;

	// Token: 0x04001280 RID: 4736
	[UIFieldTarget("id_MovementSpeedIcon")]
	private Image m_MovementSpeedIcon;

	// Token: 0x04001281 RID: 4737
	[UIFieldTarget("id_MovementSpeedValue")]
	private TextMeshProUGUI m_MovementSpeedValue;

	// Token: 0x04001282 RID: 4738
	private RectTransform m_rectTransform;

	// Token: 0x04001283 RID: 4739
	private HorizontalLayoutGroup layoutGroup;

	// Token: 0x04001284 RID: 4740
	private BattleSimulation.Squad m_logic;

	// Token: 0x04001285 RID: 4741
	private Vars m_vars;

	// Token: 0x04001286 RID: 4742
	private DT.Field m_windowDefinition;

	// Token: 0x04001287 RID: 4743
	private Color regular = Color.white;

	// Token: 0x04001288 RID: 4744
	private Color modified = Color.green;
}
