using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200019A RID: 410
public class UIArmyMorale : MonoBehaviour
{
	// Token: 0x17000136 RID: 310
	// (get) Token: 0x060016F0 RID: 5872 RVA: 0x000E3994 File Offset: 0x000E1B94
	public bool is_battle
	{
		get
		{
			return this.battle != null;
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x000E399F File Offset: 0x000E1B9F
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x000E39A7 File Offset: 0x000E1BA7
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000E39C0 File Offset: 0x000E1BC0
	public void AddArmy(global::Army army)
	{
		this.Init();
		for (int i = 0; i < this.armies.Count; i++)
		{
			if (this.armies[i].army == army)
			{
				return;
			}
		}
		if (army == null)
		{
			return;
		}
		UIArmyStatusBar.ArmyInfo armyInfo = new UIArmyStatusBar.ArmyInfo();
		armyInfo.army = army;
		this.armies.Add(armyInfo);
		this.Refresh();
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x000E3A2C File Offset: 0x000E1C2C
	public void SetSettlement(Logic.Settlement settlement)
	{
		this.Init();
		if (settlement == null)
		{
			return;
		}
		this.settlement_info = new UIArmyStatusBar.ArmyInfo
		{
			settlement = settlement
		};
		this.Refresh();
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x000E3A5D File Offset: 0x000E1C5D
	public void SetBattle(Logic.Battle battle)
	{
		this.Init();
		this.battle = battle;
		this.Refresh();
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x000E3A74 File Offset: 0x000E1C74
	public void DelArmy(global::Army army)
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			if (this.armies[i].army == army)
			{
				this.armies.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x000E3ABD File Offset: 0x000E1CBD
	public void Clear()
	{
		Vars vars = this.vars;
		if (vars != null)
		{
			vars.Clear();
		}
		this.vars = null;
		this.settlement_info = null;
		this.armies.Clear();
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x000E3AE9 File Offset: 0x000E1CE9
	public static int CompareMoraleFactors(Morale.MoraleFactor a, Morale.MoraleFactor b)
	{
		if (a.mod == null && b.mod != null)
		{
			return -1;
		}
		if (a.mod != null && b.mod == null)
		{
			return 1;
		}
		return b.value.CompareTo(a.value);
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x000E3B24 File Offset: 0x000E1D24
	public string GetFactorsTextBattle()
	{
		if (this.armies != null)
		{
			Logic.Battle battle = this.battle;
			if (((battle != null) ? battle.simulation : null) != null)
			{
				bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "army morale show inactive factors", true);
				List<BattleSimulation.Def.MoraleFactor> morale_factors = this.battle.simulation.def.morale_factors;
				StringBuilder stringBuilder = new StringBuilder(1024);
				UIArmyMorale.tmp_vars.Clear();
				bool flag2 = true;
				float num = 0f;
				for (int i = 0; i < this.armies.Count; i++)
				{
					Logic.Army logic = this.armies[i].army.logic;
					if (logic != null)
					{
						num += (float)logic.GetManPower();
					}
				}
				UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
				Garrison garrison;
				if (armyInfo == null)
				{
					garrison = null;
				}
				else
				{
					Logic.Settlement settlement = armyInfo.settlement;
					garrison = ((settlement != null) ? settlement.garrison : null);
				}
				Garrison garrison2 = garrison;
				if (garrison2 != null)
				{
					num += (float)garrison2.GetManPower();
				}
				for (int j = 0; j < morale_factors.Count; j++)
				{
					BattleSimulation.Def.MoraleFactor moraleFactor = morale_factors[j];
					UIArmyMorale.tmp_vars.Set<string>("mod_name", moraleFactor.field.Path(false, false, '.') + ".description");
					float num2 = 0f;
					for (int k = 0; k < this.armies.Count; k++)
					{
						Logic.Army logic2 = this.armies[k].army.logic;
						for (int l = 0; l < logic2.units.Count; l++)
						{
							Logic.Unit unit = logic2.units[l];
							bool flag3;
							if (unit == null)
							{
								flag3 = (null != null);
							}
							else
							{
								BattleSimulation.Squad simulation = unit.simulation;
								flag3 = (((simulation != null) ? simulation.permanent_morale_factors : null) != null);
							}
							if (flag3)
							{
								float num3 = (float)unit.manpower_alive_modified() / num;
								if (float.IsNaN(num3))
								{
									num3 = 0f;
								}
								num2 += unit.simulation.permanent_morale_factors[j] * num3;
							}
						}
					}
					UIArmyStatusBar.ArmyInfo armyInfo2 = this.settlement_info;
					bool flag4;
					if (armyInfo2 == null)
					{
						flag4 = (null != null);
					}
					else
					{
						Logic.Settlement settlement2 = armyInfo2.settlement;
						flag4 = (((settlement2 != null) ? settlement2.garrison : null) != null);
					}
					if (flag4)
					{
						List<Logic.Unit> units = this.settlement_info.settlement.garrison.units;
						for (int m = 0; m < units.Count; m++)
						{
							Logic.Unit unit2 = units[m];
							bool flag5;
							if (unit2 == null)
							{
								flag5 = (null != null);
							}
							else
							{
								BattleSimulation.Squad simulation2 = unit2.simulation;
								flag5 = (((simulation2 != null) ? simulation2.permanent_morale_factors : null) != null);
							}
							if (flag5)
							{
								float num4 = (float)unit2.manpower_alive_modified() / num;
								if (float.IsNaN(num4))
								{
									num4 = 0f;
								}
								num2 += unit2.simulation.permanent_morale_factors[j] * num4;
							}
						}
					}
					if (num != 0f && (num2 != 0f || flag))
					{
						UIArmyMorale.tmp_vars.Set<float>("value", num2);
						string value = global::Defs.Localize("SquadMoraleTooltip.base_value", UIArmyMorale.tmp_vars, null, true, true);
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
				UIArmyMorale.tmp_vars.Clear();
				return "#" + stringBuilder.ToString();
			}
		}
		return null;
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x000E3E34 File Offset: 0x000E2034
	public string GetFactorsText()
	{
		if (this.is_battle || this.armies == null || this.armies.Count == 0)
		{
			return null;
		}
		UIArmyStatusBar.ArmyInfo armyInfo = this.armies[0];
		Logic.Army army;
		if (armyInfo == null)
		{
			army = null;
		}
		else
		{
			global::Army army2 = armyInfo.army;
			army = ((army2 != null) ? army2.logic : null);
		}
		Logic.Army army3 = army;
		if (((army3 != null) ? army3.morale : null) == null)
		{
			return null;
		}
		bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "army morale show inactive factors", true);
		List<Morale.MoraleFactor> moraleFactorStats = army3.morale.GetMoraleFactorStats();
		Morale.AddMoraleFactorMods(flag);
		moraleFactorStats.Sort(new Comparison<Morale.MoraleFactor>(UIArmyMorale.CompareMoraleFactors));
		StringBuilder stringBuilder = new StringBuilder(1024);
		UIArmyMorale.tmp_vars.Clear();
		bool flag2 = true;
		int i = 0;
		while (i < moraleFactorStats.Count)
		{
			Morale.MoraleFactor moraleFactor = moraleFactorStats[i];
			string value;
			if (moraleFactor.mod != null)
			{
				DT.Field field = moraleFactor.mod.GetField();
				string val = "#" + global::Defs.Localize(field, "mod_name", moraleFactor.stat, null, true, true);
				UIArmyMorale.tmp_vars.Set<string>("mod_name", val);
				UIArmyMorale.tmp_vars.Set<float>("value", moraleFactor.value);
				value = global::Defs.Localize("ArmyMoraleTooltip.mod_factor", UIArmyMorale.tmp_vars, null, true, true);
				goto IL_1A7;
			}
			if (moraleFactor.value != 0f || flag)
			{
				DT.Field val2 = moraleFactor.stat.def.field.FindChild("base_mod_name", null, true, true, true, '.');
				UIArmyMorale.tmp_vars.Set<DT.Field>("mod_name", val2);
				UIArmyMorale.tmp_vars.Set<float>("value", moraleFactor.value);
				value = global::Defs.Localize("ArmyMoraleTooltip.base_value", UIArmyMorale.tmp_vars, null, true, true);
				goto IL_1A7;
			}
			IL_1C0:
			i++;
			continue;
			IL_1A7:
			if (flag2)
			{
				flag2 = false;
			}
			else
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(value);
			goto IL_1C0;
		}
		UIArmyMorale.tmp_vars.Clear();
		moraleFactorStats.Clear();
		return "#" + stringBuilder.ToString();
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x000E4034 File Offset: 0x000E2234
	public void Refresh()
	{
		if (this.vars == null)
		{
			this.vars = new Vars();
		}
		this.UpdateMorale();
		if (this.is_battle)
		{
			this.vars.Set<Vars.Func0>("factors_text", new Vars.Func0(this.GetFactorsTextBattle));
		}
		else
		{
			this.vars.Set<Vars.Func0>("factors_text", new Vars.Func0(this.GetFactorsText));
		}
		Tooltip.Get(base.gameObject, true).SetDef("ArmyMoraleTooltip", this.vars);
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x000E40B8 File Offset: 0x000E22B8
	protected void LateUpdate()
	{
		this.UpdateMorale();
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x000E40C0 File Offset: 0x000E22C0
	private void UpdateMorale()
	{
		using (new Stat.ForceCached("UIArmyMorale.UpdateMorale"))
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (this.vars != null)
				{
					float num = 0f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (this.is_battle)
					{
						for (int i = 0; i < this.armies.Count; i++)
						{
							Logic.Army logic = this.armies[i].army.logic;
							if (logic != null)
							{
								num5 += (float)logic.GetManPower();
							}
						}
						UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
						Garrison garrison;
						if (armyInfo == null)
						{
							garrison = null;
						}
						else
						{
							Logic.Settlement settlement = armyInfo.settlement;
							garrison = ((settlement != null) ? settlement.garrison : null);
						}
						Garrison garrison2 = garrison;
						if (garrison2 != null)
						{
							num5 += (float)garrison2.GetManPower();
						}
					}
					for (int j = 0; j < this.armies.Count; j++)
					{
						Logic.Army logic2 = this.armies[j].army.logic;
						if (logic2 != null)
						{
							if (!this.is_battle)
							{
								num += logic2.GetMorale(true);
								num2 += logic2.GetPermanentMorale();
								num3 += logic2.GetTemporaryMorale();
								num4 += logic2.morale.def.max_morale;
							}
							else
							{
								for (int k = 0; k < logic2.units.Count; k++)
								{
									Logic.Unit unit = logic2.units[k];
									BattleSimulation.Squad squad = (unit != null) ? unit.simulation : null;
									if (squad != null)
									{
										float num6 = (float)squad.unit.manpower_alive_modified() / num5;
										num += squad.GetMorale() * num6;
										num2 += squad.permanent_morale * num6;
										num3 += squad.temporary_morale * num6;
										num4 = squad.simulation.def.unit_max_morale;
									}
								}
							}
						}
					}
					if (this.is_battle)
					{
						UIArmyStatusBar.ArmyInfo armyInfo2 = this.settlement_info;
						Garrison garrison3;
						if (armyInfo2 == null)
						{
							garrison3 = null;
						}
						else
						{
							Logic.Settlement settlement2 = armyInfo2.settlement;
							garrison3 = ((settlement2 != null) ? settlement2.garrison : null);
						}
						Garrison garrison4 = garrison3;
						if (garrison4 != null)
						{
							for (int l = 0; l < garrison4.units.Count; l++)
							{
								Logic.Unit unit2 = garrison4.units[l];
								BattleSimulation.Squad squad2 = (unit2 != null) ? unit2.simulation : null;
								if (squad2 != null)
								{
									float num7 = (float)squad2.unit.manpower_alive_modified() / num5;
									if (float.IsNaN(num7))
									{
										num7 = 0f;
									}
									num += squad2.GetMorale() * num7;
									num2 += squad2.permanent_morale * num7;
									num3 += squad2.temporary_morale * num7;
									num4 = squad2.simulation.def.unit_max_morale;
								}
							}
						}
						if (num2 > num4)
						{
							num2 = num4;
						}
						if (num > num4)
						{
							num = num4;
						}
					}
					float num8 = 0f;
					float num9 = 0f;
					float num10 = 0f;
					if (num4 > 0f)
					{
						num8 = num2 / num4;
						if (num3 > 0f)
						{
							num9 = num3 / num4;
						}
						else if (num3 < 0f)
						{
							num10 = Mathf.Min(num8, -num3 / num4);
							num8 -= num10;
							if (num8 < 0f)
							{
								num8 = 0f;
							}
						}
					}
					if (this.cur_positive != num8 || this.cur_positive_less != num9 || this.cur_negative_less != num10)
					{
						if (this.armies.Count > 0)
						{
							Logic.Army logic3 = this.armies[0].army.logic;
							this.vars.Set<Logic.Army>("army", logic3);
							this.vars.Set<Logic.Kingdom>("kingdom", logic3.GetKingdom());
						}
						else
						{
							this.vars.Set<Logic.Army>("army", null);
						}
						this.vars.Set<float>("morale", num);
						this.vars.Set<float>("max_morale", num4);
						this.vars.Set<float>("permanent_morale", num2);
						this.vars.Set<float>("temporary_morale", num3);
						if (UIArmyMorale.battle_sim_def == null)
						{
							UIArmyMorale.battle_sim_def = GameLogic.Get(true).defs.GetBase<BattleSimulation.Def>();
						}
						if (UIArmyMorale.battle_sim_def != null)
						{
							this.vars.Set<float>("bonus_defense_per_morale", UIArmyMorale.battle_sim_def.BonusDefenseFromMorale(num));
						}
						this.cur_positive = num8;
						this.cur_positive_less = num9;
						this.cur_negative_less = num10;
						this.m_MoralePermanent.desired_val = this.cur_positive;
						this.m_MoraleTemporaryPositive.desired_val = this.cur_positive_less;
						this.m_MoraleTemporaryNegative.desired_val = this.cur_negative_less;
					}
				}
			}
		}
	}

	// Token: 0x04000ED4 RID: 3796
	[UIFieldTarget("id_MoralePermanent")]
	private VariableGridCell m_MoralePermanent;

	// Token: 0x04000ED5 RID: 3797
	[UIFieldTarget("id_MoraleTemporaryPositive")]
	private VariableGridCell m_MoraleTemporaryPositive;

	// Token: 0x04000ED6 RID: 3798
	[UIFieldTarget("id_MoraleTemporaryNegative")]
	private VariableGridCell m_MoraleTemporaryNegative;

	// Token: 0x04000ED7 RID: 3799
	private float cur_positive = -1f;

	// Token: 0x04000ED8 RID: 3800
	private float cur_positive_less = -1f;

	// Token: 0x04000ED9 RID: 3801
	private float cur_negative_less = -1f;

	// Token: 0x04000EDA RID: 3802
	private List<UIArmyStatusBar.ArmyInfo> armies = new List<UIArmyStatusBar.ArmyInfo>();

	// Token: 0x04000EDB RID: 3803
	private UIArmyStatusBar.ArmyInfo settlement_info;

	// Token: 0x04000EDC RID: 3804
	private Vars vars;

	// Token: 0x04000EDD RID: 3805
	private Logic.Battle battle;

	// Token: 0x04000EDE RID: 3806
	private static BattleSimulation.Def battle_sim_def;

	// Token: 0x04000EDF RID: 3807
	private bool m_Initialzied;

	// Token: 0x04000EE0 RID: 3808
	private static Vars tmp_vars = new Vars();
}
