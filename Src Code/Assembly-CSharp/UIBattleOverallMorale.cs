using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C0 RID: 448
public class UIBattleOverallMorale : MonoBehaviour
{
	// Token: 0x06001A88 RID: 6792 RVA: 0x0010108A File Offset: 0x000FF28A
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x00101092 File Offset: 0x000FF292
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x001010AB File Offset: 0x000FF2AB
	public void SetData(Logic.Battle battle, int battle_side)
	{
		this.battle = battle;
		this.battle_side = battle_side;
		this.Init();
		this.Refresh();
		this.initial_morale = battle.simulation.OverallMorale(battle_side);
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x001010D9 File Offset: 0x000FF2D9
	public void Clear()
	{
		this.battle = null;
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x001010E4 File Offset: 0x000FF2E4
	public string GetFactorsTextBattle()
	{
		Logic.Battle battle = this.battle;
		if (((battle != null) ? battle.simulation : null) == null)
		{
			return null;
		}
		List<Logic.Squad> list = this.battle.squads.Get(this.battle_side);
		bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "army morale show inactive factors", true);
		List<BattleSimulation.Def.MoraleFactor> morale_factors = this.battle.simulation.def.morale_factors;
		StringBuilder stringBuilder = new StringBuilder(1024);
		UIBattleOverallMorale.tmp_vars.Clear();
		bool flag2 = true;
		float num = this.battle.simulation.TotalManpower(this.battle_side);
		for (int i = 0; i < morale_factors.Count; i++)
		{
			BattleSimulation.Def.MoraleFactor moraleFactor = morale_factors[i];
			UIBattleOverallMorale.tmp_vars.Set<string>("mod_name", moraleFactor.field.Path(false, false, '.') + ".description");
			float num2 = 0f;
			for (int j = 0; j < list.Count; j++)
			{
				Logic.Squad squad = list[j];
				if (squad != null && !squad.IsDefeated())
				{
					BattleSimulation.Squad simulation = squad.simulation;
					if (((simulation != null) ? simulation.permanent_morale_factors : null) != null)
					{
						float num3 = simulation.total_manpower() / num;
						if (float.IsNaN(num3))
						{
							num3 = 0f;
						}
						num2 += simulation.permanent_morale_factors[i] * num3;
					}
				}
			}
			if (num != 0f && (num2 != 0f || flag))
			{
				UIBattleOverallMorale.tmp_vars.Set<float>("value", num2);
				string value = global::Defs.Localize("SquadMoraleTooltip.base_value", UIBattleOverallMorale.tmp_vars, null, true, true);
				if (flag2)
				{
					flag2 = false;
				}
				else
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(value);
			}
		}
		UIBattleOverallMorale.tmp_vars.Clear();
		return "#" + stringBuilder.ToString();
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x001012B8 File Offset: 0x000FF4B8
	public void Refresh()
	{
		if (this.vars == null)
		{
			this.vars = new Vars();
		}
		this.UpdateMorale();
		this.vars.Set<Vars.Func0>("factors_text", new Vars.Func0(this.GetFactorsTextBattle));
		Tooltip.Get(base.gameObject, true).SetDef("OverallMoraleTooltip", this.vars);
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x00101316 File Offset: 0x000FF516
	protected void LateUpdate()
	{
		this.UpdateMorale();
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x00101320 File Offset: 0x000FF520
	private void UpdateMorale()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.vars == null)
		{
			return;
		}
		if (this.battle.IsFinishing() || this.battle.battle_map_finished)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = this.battle.simulation.TotalManpower(this.battle_side);
		List<Logic.Squad> list = this.battle.squads.Get(this.battle_side);
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad = list[i];
			if (squad != null && !squad.IsDefeated() && squad.is_main_squad)
			{
				BattleSimulation.Squad simulation = squad.simulation;
				float num6 = simulation.total_manpower() / num5;
				num += simulation.GetMorale() * num6;
				num2 += simulation.permanent_morale * num6;
				num3 += simulation.temporary_morale * num6;
				num4 = simulation.simulation.def.unit_max_morale;
			}
		}
		if (num4 <= 0f)
		{
			return;
		}
		if (num2 > num4)
		{
			num2 = num4;
		}
		if (num > num4)
		{
			num = num4;
		}
		if (this.cur_permanent == num2 && this.cur_temporary == num3)
		{
			return;
		}
		this.vars.Set<float>("morale", num);
		this.vars.Set<float>("max_morale", num4);
		this.vars.Set<float>("permanent_morale", num2);
		this.vars.Set<float>("temporary_morale", num3);
		Logic.Battle battle = this.battle;
		bool flag;
		if (battle == null)
		{
			flag = (null != null);
		}
		else
		{
			BattleSimulation simulation2 = battle.simulation;
			flag = (((simulation2 != null) ? simulation2.def : null) != null);
		}
		if (flag)
		{
			this.vars.Set<float>("bonus_defense_per_morale", this.battle.simulation.def.BonusDefenseFromMorale(num));
		}
		float num7 = this.cur_permanent + this.cur_temporary;
		this.cur_permanent = num2;
		this.cur_temporary = num3;
		UISquadMorale.SetMorale(num2, num3, num4, this.m_MoraleSegments, this.morale_permanent, this.morale_temporary_negative, this.morale_temporary_positive);
		if (this.cur_permanent + this.cur_temporary < this.initial_morale / 2f && num7 >= this.initial_morale / 2f)
		{
			if (this.battle_side == global::Battle.PlayerBattleSide())
			{
				BaseUI.PlayVoiceEvent(this.battle.def.our_morale_below_half_voice_line, null);
				return;
			}
			BaseUI.PlayVoiceEvent(this.battle.def.enemy_morale_below_half_voice_line, null);
		}
	}

	// Token: 0x04001136 RID: 4406
	private int battle_side;

	// Token: 0x04001137 RID: 4407
	private Logic.Battle battle;

	// Token: 0x04001138 RID: 4408
	[UIFieldTarget("id_MoraleSegment", true)]
	private Image[] m_MoraleSegments;

	// Token: 0x04001139 RID: 4409
	public Sprite morale_permanent;

	// Token: 0x0400113A RID: 4410
	public Sprite morale_temporary_positive;

	// Token: 0x0400113B RID: 4411
	public Sprite morale_temporary_negative;

	// Token: 0x0400113C RID: 4412
	private RectTransform m_MoraleContainerRect;

	// Token: 0x0400113D RID: 4413
	private float cur_permanent = -1f;

	// Token: 0x0400113E RID: 4414
	private float cur_temporary = -1f;

	// Token: 0x0400113F RID: 4415
	private Vars vars;

	// Token: 0x04001140 RID: 4416
	private bool m_Initialzied;

	// Token: 0x04001141 RID: 4417
	private float initial_morale = -1f;

	// Token: 0x04001142 RID: 4418
	private static Vars tmp_vars = new Vars();
}
