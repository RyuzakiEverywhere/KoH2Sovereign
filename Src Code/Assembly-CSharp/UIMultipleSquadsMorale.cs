using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A0 RID: 416
public class UIMultipleSquadsMorale : MonoBehaviour
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06001793 RID: 6035 RVA: 0x000E7877 File Offset: 0x000E5A77
	public bool is_battle
	{
		get
		{
			return this.battle != null;
		}
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x000E7882 File Offset: 0x000E5A82
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x000E788A File Offset: 0x000E5A8A
	protected void LateUpdate()
	{
		this.UpdateMorale();
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x000E7894 File Offset: 0x000E5A94
	public void AddSquads(List<Logic.Squad> squads)
	{
		this.Init();
		if (squads == null)
		{
			return;
		}
		this.m_squads = squads;
		this.battle = BattleMap.battle;
		if (this.m_squads == null || this.m_squads.Count == 0 || this.m_squads[0] == null)
		{
			return;
		}
		this.Refresh();
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000E78E7 File Offset: 0x000E5AE7
	public void Clear()
	{
		this.m_squads.Clear();
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000E3AE9 File Offset: 0x000E1CE9
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

	// Token: 0x06001799 RID: 6041 RVA: 0x000E78F4 File Offset: 0x000E5AF4
	public string GetFactorsTextBattle()
	{
		if (this.m_squads != null && this.m_squads.Count != 0)
		{
			Logic.Battle battle = this.battle;
			if (((battle != null) ? battle.simulation : null) != null)
			{
				bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "army morale show inactive factors", true);
				List<BattleSimulation.Def.MoraleFactor> morale_factors = this.battle.simulation.def.morale_factors;
				StringBuilder stringBuilder = new StringBuilder(1024);
				UIMultipleSquadsMorale.tmp_vars.Clear();
				bool flag2 = true;
				float num = 0f;
				for (int i = 0; i < this.m_squads.Count; i++)
				{
					BattleSimulation.Squad simulation = this.m_squads[i].simulation;
					if (simulation != null && !simulation.def.is_siege_eq)
					{
						num += simulation.total_manpower();
					}
				}
				for (int j = 0; j < morale_factors.Count; j++)
				{
					BattleSimulation.Def.MoraleFactor moraleFactor = morale_factors[j];
					UIMultipleSquadsMorale.tmp_vars.Set<string>("mod_name", moraleFactor.field.Path(false, false, '.') + ".description");
					float num2 = 0f;
					for (int k = 0; k < this.m_squads.Count; k++)
					{
						BattleSimulation.Squad simulation2 = this.m_squads[k].simulation;
						bool flag3;
						if (simulation2 == null)
						{
							flag3 = true;
						}
						else
						{
							float num3 = simulation2.permanent_morale_factors[j];
							flag3 = false;
						}
						if (!flag3 && !simulation2.def.is_siege_eq)
						{
							float num4 = simulation2.manpower() / num;
							if (float.IsNaN(num4))
							{
								num4 = 0f;
							}
							num2 += simulation2.permanent_morale_factors[j] * num4;
						}
					}
					if (num != 0f && (num2 != 0f || flag))
					{
						UIMultipleSquadsMorale.tmp_vars.Set<float>("value", num2);
						string value = global::Defs.Localize("SquadMoraleTooltip.base_value", UIMultipleSquadsMorale.tmp_vars, null, true, true);
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
				UIMultipleSquadsMorale.tmp_vars.Clear();
				return "#" + stringBuilder.ToString();
			}
		}
		return null;
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x000E7B08 File Offset: 0x000E5D08
	public string GetFactorsText()
	{
		if (this.is_battle || this.m_squads == null || this.m_squads.Count == 0)
		{
			return null;
		}
		Logic.Squad squad = this.m_squads[0];
		bool flag;
		if (squad == null)
		{
			flag = (null != null);
		}
		else
		{
			BattleSimulation.Squad simulation = squad.simulation;
			flag = (((simulation != null) ? simulation.permanent_morale_factors : null) != null);
		}
		if (!flag)
		{
			return "";
		}
		bool flag2 = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "army morale show inactive factors", true);
		List<BattleSimulation.Def.MoraleFactor> morale_factors = squad.simulation.simulation.def.morale_factors;
		StringBuilder stringBuilder = new StringBuilder(1024);
		UIMultipleSquadsMorale.tmp_vars.Clear();
		bool flag3 = true;
		for (int i = 0; i < morale_factors.Count; i++)
		{
			BattleSimulation.Def.MoraleFactor moraleFactor = morale_factors[i];
			float num = squad.simulation.permanent_morale_factors[i];
			if (num != 0f || flag2)
			{
				UIMultipleSquadsMorale.tmp_vars.Set<string>("mod_name", moraleFactor.field.Path(false, false, '.') + ".description");
				UIMultipleSquadsMorale.tmp_vars.Set<float>("value", num);
				string value = global::Defs.Localize("SquadMoraleTooltip.base_value", UIMultipleSquadsMorale.tmp_vars, null, true, true);
				if (flag3)
				{
					flag3 = false;
				}
				else
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(value);
			}
		}
		UIMultipleSquadsMorale.tmp_vars.Clear();
		return "#" + stringBuilder.ToString();
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000E7C70 File Offset: 0x000E5E70
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
		Tooltip.Get(base.gameObject, true).SetDef("SquadMoraleTooltip", this.vars);
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x000E7CF4 File Offset: 0x000E5EF4
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x000E7D10 File Offset: 0x000E5F10
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
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		for (int i = 0; i < this.m_squads.Count; i++)
		{
			BattleSimulation.Squad simulation = this.m_squads[i].simulation;
			if (simulation != null && !simulation.def.is_siege_eq)
			{
				num5 += simulation.total_manpower();
			}
		}
		for (int j = 0; j < this.m_squads.Count; j++)
		{
			BattleSimulation.Squad simulation2 = this.m_squads[j].simulation;
			if (simulation2 != null && !simulation2.def.is_siege_eq)
			{
				float num6 = simulation2.total_manpower() / num5;
				num += simulation2.GetMorale() * num6;
				num2 += simulation2.permanent_morale * num6;
				num3 += simulation2.temporary_morale * num6;
				num4 = simulation2.simulation.def.unit_max_morale;
			}
		}
		if (this.cur_permanent == num2 && this.cur_temporary == num3)
		{
			return;
		}
		if (this.m_squads.Count > 0)
		{
			this.vars.Set<Logic.Squad>("squad", this.m_squads[0]);
			Logic.Battle battle = this.battle;
			bool flag;
			if (battle == null)
			{
				flag = (null != null);
			}
			else
			{
				BattleSimulation simulation3 = battle.simulation;
				flag = (((simulation3 != null) ? simulation3.def : null) != null);
			}
			if (flag)
			{
				this.vars.Set<float>("bonus_defense_per_morale", this.battle.simulation.def.BonusDefenseFromMorale(num));
			}
		}
		else
		{
			this.vars.Set<Logic.Army>("army", null);
		}
		this.vars.Set<float>("morale", num);
		this.vars.Set<float>("max_morale", num4);
		this.vars.Set<float>("permanent_morale", num2);
		this.vars.Set<float>("temporary_morale", num3);
		this.vars.Set<bool>("is_multiple", this.m_squads.Count > 1);
		this.cur_permanent = num2;
		this.cur_temporary = num3;
		UISquadMorale.SetMorale(num2, num3, num4, this.m_MoraleSegments, this.morale_permanent, this.morale_temporary_negative, this.morale_temporary_positive);
	}

	// Token: 0x04000F28 RID: 3880
	[UIFieldTarget("id_MoraleSegment", true)]
	private Image[] m_MoraleSegments;

	// Token: 0x04000F29 RID: 3881
	public Sprite morale_permanent;

	// Token: 0x04000F2A RID: 3882
	public Sprite morale_temporary_positive;

	// Token: 0x04000F2B RID: 3883
	public Sprite morale_temporary_negative;

	// Token: 0x04000F2C RID: 3884
	private float cur_permanent = -1f;

	// Token: 0x04000F2D RID: 3885
	private float cur_temporary = -1f;

	// Token: 0x04000F2E RID: 3886
	private List<Logic.Squad> m_squads = new List<Logic.Squad>();

	// Token: 0x04000F2F RID: 3887
	private Vars vars;

	// Token: 0x04000F30 RID: 3888
	private static Vars tmp_vars = new Vars();

	// Token: 0x04000F31 RID: 3889
	private Logic.Battle battle;

	// Token: 0x04000F32 RID: 3890
	private bool m_Initialzied;
}
