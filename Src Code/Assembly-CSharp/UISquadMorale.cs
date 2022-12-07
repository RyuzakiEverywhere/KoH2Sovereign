using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A2 RID: 418
public class UISquadMorale : MonoBehaviour
{
	// Token: 0x060017A3 RID: 6051 RVA: 0x000E7F96 File Offset: 0x000E6196
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x000E7F9E File Offset: 0x000E619E
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x000E7FB7 File Offset: 0x000E61B7
	public void SetSquad(Logic.Squad squad)
	{
		this.Init();
		if (squad == this.squad)
		{
			return;
		}
		this.squad = squad;
		BaseUI.LogicKingdom();
		this.Refresh();
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x000E7FDC File Offset: 0x000E61DC
	public void Clear()
	{
		this.squad = null;
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x000E7FE8 File Offset: 0x000E61E8
	public string GetFactorsText()
	{
		Logic.Squad squad = this.squad;
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
		List<BattleSimulation.Def.MoraleFactor> morale_factors = this.squad.simulation.simulation.def.morale_factors;
		StringBuilder stringBuilder = new StringBuilder(1024);
		UISquadMorale.tmp_vars.Clear();
		bool flag3 = true;
		for (int i = 0; i < morale_factors.Count; i++)
		{
			BattleSimulation.Def.MoraleFactor moraleFactor = morale_factors[i];
			float num = this.squad.simulation.permanent_morale_factors[i];
			if (num != 0f || flag2)
			{
				UISquadMorale.tmp_vars.Set<string>("mod_name", moraleFactor.field.Path(false, false, '.') + ".description");
				UISquadMorale.tmp_vars.Set<float>("value", num);
				string value = global::Defs.Localize("SquadMoraleTooltip.base_value", UISquadMorale.tmp_vars, null, true, true);
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
		UISquadMorale.tmp_vars.Clear();
		return "#" + stringBuilder.ToString();
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x000E812F File Offset: 0x000E632F
	public void Refresh()
	{
		if (this.vars == null)
		{
			this.vars = new Vars();
		}
		this.UpdateMorale();
		this.vars.Set<Vars.Func0>("factors_text", new Vars.Func0(this.GetFactorsText));
		this.CheckTooltipToggleChanged(true);
	}

	// Token: 0x060017A9 RID: 6057 RVA: 0x000E8170 File Offset: 0x000E6370
	public void CheckTooltipToggleChanged(bool force = false)
	{
		global::Squad squad = this.squad.visuals as global::Squad;
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
		this.ToggleTooltip(nameplateTooltipFilter);
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x000E81D0 File Offset: 0x000E63D0
	public void ToggleTooltip(bool isFilterOn)
	{
		if (this.pinnable)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadMoraleTooltip", this.vars);
			return;
		}
		if (isFilterOn)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadMoraleTooltipUnPinable", this.vars);
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, false);
		if (tooltip != null)
		{
			global::Common.DestroyObj(tooltip);
		}
	}

	// Token: 0x060017AB RID: 6059 RVA: 0x000E823E File Offset: 0x000E643E
	protected void LateUpdate()
	{
		this.UpdateMorale();
		this.CheckTooltipToggleChanged(false);
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x000E8250 File Offset: 0x000E6450
	private void SetMorale(float permanent, float temporary)
	{
		float max = 0f;
		Logic.Squad squad = this.squad;
		bool flag;
		if (squad == null)
		{
			flag = (null != null);
		}
		else
		{
			BattleSimulation.Squad simulation = squad.simulation;
			if (simulation == null)
			{
				flag = (null != null);
			}
			else
			{
				BattleSimulation simulation2 = simulation.simulation;
				flag = (((simulation2 != null) ? simulation2.def : null) != null);
			}
		}
		if (flag)
		{
			max = this.squad.simulation.simulation.def.unit_max_morale;
		}
		UISquadMorale.SetMorale(permanent, temporary, max, this.m_MoraleSegments, this.morale_permanent, this.morale_temporary_negative, this.morale_temporary_positive);
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x000E82CC File Offset: 0x000E64CC
	public static void SetMorale(float permanent, float temporary, float max, Image[] m_MoraleSegments, Sprite morale_permanent, Sprite morale_temporary_negative, Sprite morale_temporary_positive)
	{
		float num = permanent;
		float num2 = permanent;
		if (temporary > 0f)
		{
			num += temporary;
		}
		else
		{
			num2 += temporary;
		}
		float num3 = max / 10f;
		num -= num % num3;
		int num4 = (int)(10f * (num / max));
		num = num2 - num2 % num3;
		int num5 = (int)(10f * (num / max));
		for (int i = 0; i < m_MoraleSegments.Length; i++)
		{
			bool flag = i < num4;
			Image image = m_MoraleSegments[i];
			image.gameObject.SetActive(flag);
			if (flag)
			{
				if (i < num5)
				{
					image.sprite = morale_permanent;
				}
				else if (temporary < 0f)
				{
					image.sprite = morale_temporary_negative;
				}
				else
				{
					image.sprite = morale_temporary_positive;
				}
			}
		}
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x000E8380 File Offset: 0x000E6580
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
		Logic.Squad squad = this.squad;
		if (((squad != null) ? squad.simulation : null) == null)
		{
			return;
		}
		if (this.squad == null || this.squad.simulation.IsDefeated())
		{
			this.SetMorale(0f, 0f);
			return;
		}
		float morale = this.squad.simulation.GetMorale();
		float permanent_morale = this.squad.simulation.permanent_morale;
		float temporary_morale = this.squad.simulation.temporary_morale;
		float unit_max_morale = this.squad.simulation.simulation.def.unit_max_morale;
		if (this.cur_positive == permanent_morale && this.cur_temporary == temporary_morale)
		{
			return;
		}
		this.vars.Set<Logic.Squad>("squad", this.squad);
		this.vars.Set<float>("morale", morale);
		this.vars.Set<float>("max_morale", unit_max_morale);
		this.vars.Set<float>("permanent_morale", permanent_morale);
		this.vars.Set<float>("temporary_morale", temporary_morale);
		Logic.Squad squad2 = this.squad;
		bool flag;
		if (squad2 == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Battle battle = squad2.battle;
			if (battle == null)
			{
				flag = (null != null);
			}
			else
			{
				BattleSimulation simulation = battle.simulation;
				flag = (((simulation != null) ? simulation.def : null) != null);
			}
		}
		if (flag)
		{
			this.vars.Set<float>("bonus_defense_per_morale", this.squad.battle.simulation.def.BonusDefenseFromMorale(morale));
		}
		this.vars.Set<bool>("is_multiple", false);
		this.cur_positive = permanent_morale;
		this.cur_temporary = temporary_morale;
		this.SetMorale(permanent_morale, temporary_morale);
	}

	// Token: 0x04000F33 RID: 3891
	[UIFieldTarget("id_MoraleSegment", true)]
	private Image[] m_MoraleSegments;

	// Token: 0x04000F34 RID: 3892
	public Sprite morale_permanent;

	// Token: 0x04000F35 RID: 3893
	public Sprite morale_temporary_positive;

	// Token: 0x04000F36 RID: 3894
	public Sprite morale_temporary_negative;

	// Token: 0x04000F37 RID: 3895
	public bool pinnable;

	// Token: 0x04000F38 RID: 3896
	private float cur_positive = -1f;

	// Token: 0x04000F39 RID: 3897
	private float cur_temporary = -1f;

	// Token: 0x04000F3A RID: 3898
	private Logic.Squad squad;

	// Token: 0x04000F3B RID: 3899
	private Vars vars;

	// Token: 0x04000F3C RID: 3900
	private bool m_Initialzied;

	// Token: 0x04000F3D RID: 3901
	private static Vars tmp_vars = new Vars();

	// Token: 0x04000F3E RID: 3902
	private int last_tooltip_enabled = -1;
}
