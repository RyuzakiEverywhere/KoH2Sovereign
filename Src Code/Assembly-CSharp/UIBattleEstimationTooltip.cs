using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BE RID: 446
public class UIBattleEstimationTooltip : MonoBehaviour, Tooltip.IHandler, IListener
{
	// Token: 0x06001A68 RID: 6760 RVA: 0x000FF368 File Offset: 0x000FD568
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Battle battle = null;
		if (tooltip.vars != null)
		{
			battle = tooltip.vars.obj.Get<Logic.Battle>();
		}
		return this.HandleTooltip(battle, tooltip.vars, ui, evt);
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x000FF3A0 File Offset: 0x000FD5A0
	public bool HandleTooltip(Logic.Battle battle, Vars vars, BaseUI ui, Tooltip.Event evt)
	{
		this.Init();
		this.SetBattle(battle);
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			if (evt == Tooltip.Event.Hide)
			{
				this.SetBattle(null);
			}
			return false;
		}
		if (battle == null)
		{
			TooltipInstance component = base.GetComponent<TooltipInstance>();
			if (component != null)
			{
				component.Close(null);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return true;
		}
		this.Refresh(evt != Tooltip.Event.Update);
		return true;
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x000FF409 File Offset: 0x000FD609
	private void OnDisable()
	{
		this.SetBattle(null);
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x000FF412 File Offset: 0x000FD612
	private void SetBattle(Logic.Battle battle)
	{
		if (this.logic == battle)
		{
			return;
		}
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		this.logic = battle;
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x000FF450 File Offset: 0x000FD650
	private void FillStars(Transform starsContainer, GameObject noneContainer, float val, float max)
	{
		if (max <= 0f)
		{
			if (starsContainer != null)
			{
				starsContainer.gameObject.SetActive(false);
			}
			if (noneContainer != null)
			{
				noneContainer.SetActive(true);
			}
			return;
		}
		if (starsContainer == null)
		{
			return;
		}
		starsContainer.gameObject.SetActive(true);
		if (noneContainer != null)
		{
			noneContainer.SetActive(false);
		}
		int childCount = starsContainer.childCount;
		float num = val / max * (float)childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform transform = starsContainer.transform.GetChild(i).Find("id_FullStar");
			if (!(transform == null))
			{
				Image component = transform.GetComponent<Image>();
				float num2 = (float)i + 0.25f;
				float num3 = (float)i + 0.75f;
				if (num >= num3)
				{
					component.fillAmount = 1f;
				}
				else if (num3 > num && num > num2)
				{
					component.fillAmount = 0.5f;
				}
				else
				{
					component.fillAmount = 0f;
				}
			}
		}
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x000FF540 File Offset: 0x000FD740
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = true;
		this.CalcMaxNavalpower();
		this.equipment_slot_prefab = UICommon.GetPrefab("ArmyInvetoryItemSlot", null);
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x000FF570 File Offset: 0x000FD770
	private void Refresh(bool full = true)
	{
		this.Init();
		if (full)
		{
			if (this.logic != null)
			{
				this.vars = this.logic.Vars();
			}
			if (this.m_EstimationBar != null)
			{
				this.m_EstimationBar.SetObject(this.logic);
			}
			if (this.m_BattleName != null)
			{
				UIText.SetTextKey(this.m_BattleName, this.vars.Get<string>("BATTLE", null), this.vars, null);
			}
			global::Battle.CalcSides(this.logic, out this.logic_sides[0], out this.logic_sides[1], false);
			if (this.m_ManpowerDescription != null)
			{
				UIText.SetTextKey(this.m_ManpowerDescription, "Battle.Manpower.Description", null, null);
			}
			if (this.m_UnitTypeDescription != null)
			{
				UIText.SetTextKey(this.m_UnitTypeDescription, "Battle.UnitType.Description", null, null);
			}
			if (this.m_LevelsDescription != null)
			{
				UIText.SetTextKey(this.m_LevelsDescription, "Battle.Level.Description", null, null);
			}
			if (this.m_TierDescription != null)
			{
				UIText.SetTextKey(this.m_TierDescription, "Battle.Tier.Description", null, null);
			}
			if (this.m_UnitCounterDescription != null)
			{
				UIText.SetTextKey(this.m_UnitCounterDescription, "Battle.UnitCounter.Description", null, null);
			}
			if (this.m_NavalPowerDescription != null)
			{
				UIText.SetTextKey(this.m_NavalPowerDescription, "Battle.NavalPower.Description", null, null);
			}
			if (this.m_SiegeStrengthDescription != null)
			{
				UIText.SetTextKey(this.m_SiegeStrengthDescription, "Battle.SiegeStrength.Description", null, null);
			}
			if (this.m_EquipmentDescription != null)
			{
				UIText.SetTextKey(this.m_EquipmentDescription, "ArmyEquipmentTutorialHotspot.tooltip.caption", null, null);
			}
			this.RefreshCrests();
			this.RefreshEstimationText();
			this.RefreshManpowerAndStats();
			this.RefreshLevels();
			this.RefreshTier();
			this.RefreshBattleDescription();
			this.RefreshGoal();
			this.RefreshFortifications();
			this.RefreshSiegeEquipment();
			this.RefreshAttrition();
		}
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x000FF754 File Offset: 0x000FD954
	private void RefreshGoal()
	{
		if (this.m_GoalContainer != null)
		{
			Logic.Battle battle = this.logic;
			if (((battle != null) ? battle.batte_view_game : null) != null)
			{
				this.m_GoalContainer.SetActive(true);
				string str = this.vars.Get<string>("type_key", null);
				string key;
				if (global::Battle.PlayerIsAttacker(this.logic, true))
				{
					key = "Battle.GoalDescription." + str + ".Attacker";
				}
				else
				{
					key = "Battle.GoalDescription." + str + ".Defender";
				}
				if (this.m_Goal != null)
				{
					UIText.SetTextKey(this.m_Goal, key, this.vars, null);
					return;
				}
			}
			else
			{
				this.m_GoalContainer.SetActive(false);
			}
		}
	}

	// Token: 0x06001A70 RID: 6768 RVA: 0x000FF804 File Offset: 0x000FDA04
	private void RefreshAttrition()
	{
		if (this.logic.is_siege)
		{
			this.m_AttritionContainer.SetActive(true);
			if (this.m_Attrition != null)
			{
				UIText.SetTextKey(this.m_Attrition, "Battle.Attrition.Description", this.logic, null);
				return;
			}
		}
		else
		{
			this.m_AttritionContainer.SetActive(false);
		}
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x000FF85C File Offset: 0x000FDA5C
	private void RefreshFortifications()
	{
		if (this.m_FortificationsContainer != null)
		{
			float num = 0f;
			float num2 = 0f;
			Logic.Battle battle = this.logic;
			if (((battle != null) ? battle.fortifications : null) != null && this.logic.fortifications.Count > 0)
			{
				for (int i = 0; i < this.logic.fortifications.Count; i++)
				{
					Logic.Fortification fortification = this.logic.fortifications[i];
					num2 += fortification.health;
					num += fortification.max_health;
				}
			}
			else if (this.logic.is_siege && this.logic.batte_view_game == null)
			{
				num2 = this.logic.siege_defense;
				num = this.logic.initial_siege_defense_pre_condition;
			}
			if (num > 0f)
			{
				this.m_FortificationsContainer.SetActive(true);
				float num3 = 1f - num2 / num;
				if (num3 <= 0f)
				{
					UIText.SetTextKey(this.m_Fortifications, "Battle.Fortifications.Intact", null, null);
					return;
				}
				if (num3 <= 0.25f)
				{
					UIText.SetTextKey(this.m_Fortifications, "Battle.Fortifications.SlightlyDamaged", null, null);
					return;
				}
				if (num3 <= 0.5f)
				{
					UIText.SetTextKey(this.m_Fortifications, "Battle.Fortifications.Damaged", null, null);
					return;
				}
				if (num3 <= 0.75f)
				{
					UIText.SetTextKey(this.m_Fortifications, "Battle.Fortifications.HeavilyDamaged", null, null);
					return;
				}
				UIText.SetTextKey(this.m_Fortifications, "Battle.Fortifications.InRuins", null, null);
				return;
			}
			else
			{
				this.m_FortificationsContainer.SetActive(false);
			}
		}
	}

	// Token: 0x06001A72 RID: 6770 RVA: 0x000FF9D4 File Offset: 0x000FDBD4
	private void RefreshCrests()
	{
		if (this.logic_sides[0] == 0)
		{
			if (this.m_LeftKingdomCrest != null)
			{
				this.m_LeftKingdomCrest.SetObject(this.logic.attacker_kingdom, null);
			}
			if (this.m_RightKingdomCrest != null)
			{
				this.m_RightKingdomCrest.SetObject(this.logic.defender_kingdom, null);
			}
			if (this.m_LeftKingdomSupporterCrest != null)
			{
				this.SetupSupporterCrest(this.m_LeftKingdomSupporterCrest, 0);
			}
			if (this.m_RightKingdomSupporterCrest != null)
			{
				this.SetupSupporterCrest(this.m_RightKingdomSupporterCrest, 1);
				return;
			}
		}
		else
		{
			if (this.m_LeftKingdomCrest != null)
			{
				this.m_LeftKingdomCrest.SetObject(this.logic.defender_kingdom, null);
			}
			if (this.m_RightKingdomCrest != null)
			{
				this.m_RightKingdomCrest.SetObject(this.logic.attacker_kingdom, null);
			}
			if (this.m_LeftKingdomSupporterCrest != null)
			{
				this.SetupSupporterCrest(this.m_LeftKingdomSupporterCrest, 1);
			}
			if (this.m_RightKingdomSupporterCrest != null)
			{
				this.SetupSupporterCrest(this.m_RightKingdomSupporterCrest, 0);
			}
		}
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x000FFAF4 File Offset: 0x000FDCF4
	private void RefreshBattleDescription()
	{
		if (this.m_BattleDescription != null)
		{
			string str = this.vars.Get<string>("type_key", null);
			string text;
			if (global::Battle.PlayerIsAttacker(this.logic, true))
			{
				text = "Battle.BattleDescription." + str + ".Attacker";
			}
			else if (global::Battle.PlayerIsDefender(this.logic, true))
			{
				text = "Battle.BattleDescription." + str + ".Defender";
			}
			else
			{
				text = "Battle.BattleDescription." + str + ".Default";
			}
			if (this.logic.type == Logic.Battle.Type.BreakSiege && this.logic.settlement is Castle)
			{
				bool flag = false;
				for (int i = 0; i < this.logic.defenders.Count; i++)
				{
					if (this.logic.defenders[i].castle != this.logic.settlement)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					text += ".Outside";
				}
				else
				{
					text += ".Inside";
				}
			}
			UIText.SetTextKey(this.m_BattleDescription, text, this.vars, null);
		}
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x000FFC14 File Offset: 0x000FDE14
	private void SetupSupporterCrest(UIKingdomIcon crest, int side)
	{
		Logic.Kingdom kingdom = null;
		List<Logic.Army> armies = this.logic.GetArmies(side);
		Logic.Kingdom sideKingdom = this.logic.GetSideKingdom(side);
		for (int i = 0; i < armies.Count; i++)
		{
			Logic.Army army = armies[i];
			Logic.Kingdom kingdom2 = (army != null) ? army.GetKingdom() : null;
			if (kingdom2 != null && kingdom2 != sideKingdom)
			{
				kingdom = kingdom2;
				break;
			}
		}
		if (kingdom != null)
		{
			crest.gameObject.SetActive(true);
			crest.SetObject(kingdom, null);
			return;
		}
		crest.gameObject.SetActive(false);
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x000FFC98 File Offset: 0x000FDE98
	private void RefreshEstimationText()
	{
		if (this.m_ValueEstimation != null)
		{
			float num;
			if (this.logic.winner < 0)
			{
				num = this.logic.simulation.GetEstimation();
				if (this.logic_sides[0] != 0)
				{
					num = 1f - num;
				}
			}
			else
			{
				num = ((this.logic_sides[0] == this.logic.winner) ? 1f : 0f);
			}
			int num2 = 0;
			string text;
			if (this.logic.winner >= 0)
			{
				text = ((num2 == this.logic.winner) ? "won" : "lost");
			}
			else
			{
				text = global::Battle.GetEstimationKey((num2 == 0) ? (1f - num) : num);
			}
			this.m_ValueEstimation.color = global::Battle.GetEstimationColor(text);
			if (this.logic.stage == Logic.Battle.Stage.Preparing)
			{
				text = "preparation_" + text;
			}
			else if (global::Battle.PlayerIsAttacker(this.logic, true) || global::Battle.PlayerIsDefender(this.logic, true))
			{
				text = "player_" + text;
			}
			UIText.SetText(this.m_ValueEstimation, global::Battle.GetEstimationText(text));
		}
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000FFDB4 File Offset: 0x000FDFB4
	private void CountSquadTypes()
	{
		for (int i = 0; i < 2; i++)
		{
			int num = this.logic_sides[i];
			this.manpower[num] = 0;
			this.ranged[num] = 0;
			this.cavalry[num] = 0;
			this.ranged_cavalry[num] = 0;
			this.infantry[num] = 0;
			this.defense[num] = 0;
			this.militia[num] = 0;
			this.noble[num] = 0;
			this.max_manpower[num] = 0;
			List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(num);
			for (int j = 0; j < squads.Count; j++)
			{
				BattleSimulation.Squad squad = squads[j];
				if (squad.sub_squads != null)
				{
					squad = squad.sub_squads[0];
				}
				if (squad != null && (squad.squad == null || squad.squad.is_main_squad))
				{
					if (this.logic.batte_view_game != null)
					{
						ushort[] array = this.max_manpower;
						int num2 = num;
						array[num2] += (ushort)squad.max_troops;
					}
					else
					{
						ushort[] array2 = this.max_manpower;
						int num3 = num;
						array2[num3] += (ushort)squad.total_manpower();
					}
					if (!squad.IsDefeated())
					{
						ushort num4;
						if (this.logic.batte_view_game != null)
						{
							num4 = (ushort)squad.NumTroops();
						}
						else
						{
							num4 = (ushort)squad.manpower();
						}
						if (squad.def.is_ranged && squad.def.is_cavalry)
						{
							ushort[] array3 = this.ranged_cavalry;
							int num5 = num;
							array3[num5] += num4;
						}
						else if (squad.def.is_ranged)
						{
							ushort[] array4 = this.ranged;
							int num6 = num;
							array4[num6] += num4;
						}
						else if (squad.def.is_cavalry)
						{
							ushort[] array5 = this.cavalry;
							int num7 = num;
							array5[num7] += num4;
						}
						else if (squad.def.is_defense)
						{
							ushort[] array6 = this.defense;
							int num8 = num;
							array6[num8] += num4;
						}
						else if (squad.def.type == Logic.Unit.Type.Militia)
						{
							ushort[] array7 = this.militia;
							int num9 = num;
							array7[num9] += num4;
						}
						else
						{
							ushort[] array8 = this.infantry;
							int num10 = num;
							array8[num10] += num4;
						}
						if (squad.def.type == Logic.Unit.Type.Noble)
						{
							ushort[] array9 = this.noble;
							int num11 = num;
							array9[num11] += num4;
						}
						ushort[] array10 = this.manpower;
						int num12 = num;
						array10[num12] += num4;
					}
				}
			}
		}
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x00100010 File Offset: 0x000FE210
	private void CalcMainUnitType()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 2; i++)
		{
			int num3 = this.logic_sides[i];
			string str = "None";
			if (this.max_manpower[num3] > 0)
			{
				ushort num4 = this.manpower[num3] / 2;
				str = "Balanced";
				if (this.manpower[num3] > 0)
				{
					if (this.ranged_cavalry[num3] >= num4)
					{
						str = "RangedCavalry";
					}
					else if (this.ranged[num3] >= num4)
					{
						str = "Ranged";
					}
					else if (this.cavalry[num3] >= num4)
					{
						str = "Cavalry";
					}
					else if (this.defense[num3] >= num4)
					{
						str = "Defense";
					}
					else if (this.infantry[num3] >= num4)
					{
						str = "Infantry";
					}
					else if (this.militia[num3] >= num4)
					{
						str = "Militia";
					}
				}
			}
			if (i == 0)
			{
				if (this.max_manpower[num3] > 0)
				{
					num = (int)this.manpower[num3];
				}
				else
				{
					num = -1;
				}
				UIText.SetTextKey(this.m_UnitTypeLeft, "Battle.UnitType." + str, null, null);
			}
			else
			{
				if (this.max_manpower[num3] > 0)
				{
					num2 = (int)this.manpower[num3];
				}
				else
				{
					num2 = -1;
				}
				UIText.SetTextKey(this.m_UnitTypeRight, "Battle.UnitType." + str, null, null);
			}
		}
		string form = null;
		string form2 = null;
		if (num > num2)
		{
			form = "bonus_no_sign";
		}
		else if (num < num2)
		{
			form2 = "bonus_no_sign";
		}
		if (num >= 0)
		{
			UIText.SetText(this.m_ManpowerLeft, global::Defs.LocalizedNumber(num, null, form));
		}
		else
		{
			UIText.SetText(this.m_ManpowerLeft, "-");
		}
		if (num2 >= 0)
		{
			UIText.SetText(this.m_ManpowerRight, global::Defs.LocalizedNumber(num2, null, form2));
			return;
		}
		UIText.SetText(this.m_ManpowerRight, "-");
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x001001E0 File Offset: 0x000FE3E0
	private void CalcUnitCounters()
	{
		if (this.m_UnitCounterLeft != null && this.m_UnitCounterRight != null)
		{
			if (this.manpower[0] == 0 || this.manpower[1] == 0)
			{
				this.FillStars(this.m_UnitCounterLeft, this.m_UnitCounterLeftNone, 0f, 0f);
				this.FillStars(this.m_UnitCounterRight, this.m_UnitCounterRightNone, 0f, 0f);
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				int num = this.logic_sides[i];
				float num2 = 0f;
				float num3 = (float)this.manpower[num];
				float num4 = (float)this.manpower[1 - num];
				float num5 = num3 - (float)this.noble[num];
				float num6 = (float)(this.infantry[1 - num] + this.militia[1 - num]) / num4;
				float num7 = (float)(this.cavalry[num] + this.ranged_cavalry[num]) / num3;
				num2 += num6 * num7;
				if (this.logic.type == Logic.Battle.Type.Naval || this.logic.is_siege)
				{
					float num8 = (float)(this.infantry[1 - num] + this.defense[1 - num] + this.militia[1 - num] + this.cavalry[1 - num]) / num4;
					float num9 = (float)(this.ranged[num] + this.ranged_cavalry[num]) / num5;
					num2 += num8 * num9;
				}
				else
				{
					float num10 = (float)this.ranged[1 - num] / num4;
					float num11 = (float)(this.cavalry[num] + this.ranged_cavalry[num]) / num3;
					num2 += num11 * num10;
				}
				float num12 = (float)(this.cavalry[1 - num] + this.ranged_cavalry[1 - num]) / num4;
				float num13 = (float)this.defense[num] / num5;
				num2 += num12 * num13;
				float num14 = (float)this.defense[1 - num] / num4;
				float num15 = (float)this.infantry[num] / num5;
				num2 += num14 * num15;
				if (i == 0)
				{
					this.FillStars(this.m_UnitCounterLeft, this.m_UnitCounterLeftNone, num2, 1f);
				}
				else
				{
					this.FillStars(this.m_UnitCounterRight, this.m_UnitCounterRightNone, num2, 1f);
				}
			}
		}
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x00100404 File Offset: 0x000FE604
	private void CalcMaxNavalpower()
	{
		List<Logic.Unit.Def> defs = GameLogic.Get(true).defs.GetDefs<Logic.Unit.Def>();
		this.max_naval_power = 0f;
		for (int i = 0; i < defs.Count; i++)
		{
			Logic.Unit.Def def = defs[i];
			if (def.naval_CTH_perc > this.max_naval_power)
			{
				this.max_naval_power = def.naval_CTH_perc;
			}
		}
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x00100460 File Offset: 0x000FE660
	private void CalcNavalPower()
	{
		if (this.m_NavalPowerContainer != null)
		{
			if (this.logic.type != Logic.Battle.Type.Naval)
			{
				this.m_NavalPowerContainer.SetActive(false);
				return;
			}
			this.m_NavalPowerContainer.SetActive(true);
			for (int i = 0; i < 2; i++)
			{
				int num = this.logic_sides[i];
				if (this.max_manpower[num] <= 0)
				{
					if (i == 0)
					{
						this.FillStars(this.m_NavalPowerLeft, this.m_NavalPowerLeftNone, 0f, 0f);
					}
					else
					{
						this.FillStars(this.m_NavalPowerRight, this.m_NavalPowerRightNone, 0f, 0f);
					}
				}
				else
				{
					float num2 = (float)(this.manpower[num] - this.noble[num]);
					List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(num);
					float num3 = 0f;
					for (int j = 0; j < squads.Count; j++)
					{
						BattleSimulation.Squad squad = squads[j];
						if (squad.sub_squads != null)
						{
							squad = squad.sub_squads[0];
						}
						if (squad != null && (squad.squad == null || squad.squad.is_main_squad) && !squad.IsDefeated() && squad.def.type != Logic.Unit.Type.Noble)
						{
							ushort num4;
							if (this.logic.batte_view_game != null)
							{
								num4 = (ushort)squad.NumTroops();
							}
							else
							{
								num4 = (ushort)squad.manpower();
							}
							num3 += squad.def.naval_CTH_perc * (float)num4 / num2;
						}
					}
					if (i == 0)
					{
						this.FillStars(this.m_NavalPowerLeft, this.m_NavalPowerLeftNone, num3, this.max_naval_power);
					}
					else
					{
						this.FillStars(this.m_NavalPowerRight, this.m_NavalPowerRightNone, num3, this.max_naval_power);
					}
				}
			}
		}
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x00100624 File Offset: 0x000FE824
	private void CalcSiegeStrength()
	{
		if (this.m_SiegeStrengthContainer != null)
		{
			if (!this.logic.is_siege)
			{
				this.m_SiegeStrengthContainer.SetActive(false);
				return;
			}
			this.m_SiegeStrengthContainer.SetActive(true);
			for (int i = 0; i < 2; i++)
			{
				int num = this.logic_sides[i];
				if (this.max_manpower[num] <= 0 && num != 1)
				{
					if (i == 0)
					{
						this.FillStars(this.m_SiegeStrengthLeft, this.m_SiegeStrengthLeftNone, 0f, 0f);
					}
					else
					{
						this.FillStars(this.m_SiegeStrengthRight, this.m_SiegeStrengthRightNone, 0f, 0f);
					}
				}
				else
				{
					float num2 = (float)(this.manpower[num] - this.noble[num]);
					List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(num);
					float num3 = 0f;
					float max;
					if (num == 0)
					{
						for (int j = 0; j < squads.Count; j++)
						{
							BattleSimulation.Squad squad = squads[j];
							if (squad.sub_squads != null)
							{
								squad = squad.sub_squads[0];
							}
							if (squad != null && (squad.squad == null || squad.squad.is_main_squad) && !squad.IsDefeated() && squad.def.type != Logic.Unit.Type.Noble && !squad.def.is_siege_eq)
							{
								ushort num4;
								if (this.logic.batte_view_game != null)
								{
									num4 = (ushort)squad.NumTroops();
								}
								else
								{
									num4 = (ushort)squad.manpower();
								}
								num3 += squad.unit.siege_strength_modified() * (float)num4 / num2;
							}
						}
						List<InventoryItem> equipment = this.logic.simulation.GetEquipment(num);
						for (int k = 0; k < equipment.Count; k++)
						{
							InventoryItem inventoryItem = equipment[k];
							if (inventoryItem != null && !inventoryItem.IsDefeated())
							{
								float num5 = num3;
								Logic.Unit.Def def = inventoryItem.def;
								Logic.Battle battle = this.logic;
								bool use_battle_bonuses = true;
								int battle_side = num;
								int level = 0;
								Logic.Army army = inventoryItem.army;
								num3 = num5 + def.siege_strength_modified(battle, use_battle_bonuses, battle_side, level, (army != null) ? army.leader : null, null);
							}
						}
						max = 14f;
					}
					else
					{
						num3 += this.logic.GetRealm().GetStat(Stats.rs_attrition_damage, true);
						num3 += (float)(100 * (this.ranged[num] + this.ranged_cavalry[num])) / num2;
						num3 /= 50f;
						max = 3f;
					}
					if (i == 0)
					{
						this.FillStars(this.m_SiegeStrengthLeft, this.m_SiegeStrengthLeftNone, num3, max);
					}
					else
					{
						this.FillStars(this.m_SiegeStrengthRight, this.m_SiegeStrengthRightNone, num3, max);
					}
				}
			}
		}
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x001008BE File Offset: 0x000FEABE
	private void RefreshManpowerAndStats()
	{
		this.CountSquadTypes();
		this.CalcMainUnitType();
		this.CalcUnitCounters();
		this.CalcNavalPower();
		this.CalcSiegeStrength();
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x001008E0 File Offset: 0x000FEAE0
	private void RefreshLevels()
	{
		for (int i = 0; i < 2; i++)
		{
			int side = this.logic_sides[i];
			List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(side);
			float num = 0f;
			float num2 = 0f;
			for (int j = 0; j < squads.Count; j++)
			{
				BattleSimulation.Squad squad = squads[j];
				if (squad.sub_squads != null)
				{
					squad = squad.sub_squads[0];
				}
				if (squad != null && !squad.IsDefeated() && (squad.squad == null || squad.squad.is_main_squad) && squad.def.type != Logic.Unit.Type.Noble && !squad.def.is_siege_eq)
				{
					num += (float)squad.unit.level;
					num2 += (float)squad.unit.def.experience_to_next.items.Count;
				}
			}
			if (i == 0)
			{
				this.FillStars(this.m_LevelsLeft, this.m_LevelsLeftNone, num, num2);
			}
			else
			{
				this.FillStars(this.m_LevelsRight, this.m_LevelsRightNone, num, num2);
			}
		}
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x00100A08 File Offset: 0x000FEC08
	private void RefreshTier()
	{
		for (int i = 0; i < 2; i++)
		{
			int side = this.logic_sides[i];
			List<BattleSimulation.Squad> squads = this.logic.simulation.GetSquads(side);
			float num = 0f;
			float num2 = 0f;
			for (int j = 0; j < squads.Count; j++)
			{
				BattleSimulation.Squad squad = squads[j];
				if (squad.sub_squads != null)
				{
					squad = squad.sub_squads[0];
				}
				if (squad != null && !squad.IsDefeated() && (squad.squad == null || squad.squad.is_main_squad) && !squad.def.is_siege_eq && squad.def.type != Logic.Unit.Type.Noble)
				{
					num += (float)squad.unit.def.tier;
					num2 += 2f;
				}
			}
			if (i == 0)
			{
				this.FillStars(this.m_TierLeft, this.m_TierLeftNone, num, num2);
			}
			else
			{
				this.FillStars(this.m_TierRight, this.m_TierRightNone, num, num2);
			}
		}
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x00100B1C File Offset: 0x000FED1C
	private void RefreshSiegeEquipment()
	{
		if (this.equipment_slot_prefab == null || (this.logic.simulation.GetEquipment(0).Count == 0 && this.logic.simulation.GetEquipment(1).Count == 0) || this.m_EquipmentLeft == null || this.m_EquipmentLeft == null)
		{
			if (this.m_EquipmentContainer != null)
			{
				this.m_EquipmentContainer.SetActive(false);
			}
			return;
		}
		this.m_EquipmentContainer.SetActive(true);
		global::Common.DestroyChildren(this.m_EquipmentLeft);
		global::Common.DestroyChildren(this.m_EquipmentRight);
		for (int i = 0; i < 2; i++)
		{
			Dictionary<Logic.Unit.Def, byte> dictionary = new Dictionary<Logic.Unit.Def, byte>();
			int side = this.logic_sides[i];
			List<InventoryItem> equipment = this.logic.simulation.GetEquipment(side);
			for (int j = 0; j < equipment.Count; j++)
			{
				InventoryItem inventoryItem = equipment[j];
				if (!dictionary.ContainsKey(inventoryItem.def))
				{
					dictionary[inventoryItem.def] = 0;
				}
				if (!inventoryItem.IsDefeated())
				{
					Dictionary<Logic.Unit.Def, byte> dictionary2 = dictionary;
					Logic.Unit.Def def = inventoryItem.def;
					byte b = dictionary2[def];
					dictionary2[def] = b + 1;
				}
			}
			Transform transform;
			if (i == 0)
			{
				transform = this.m_EquipmentLeft.transform;
			}
			else
			{
				transform = this.m_EquipmentRight.transform;
			}
			int slotIndex = 0;
			foreach (KeyValuePair<Logic.Unit.Def, byte> keyValuePair in dictionary)
			{
				UIArmyItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.equipment_slot_prefab, transform).GetComponent<UIArmyItemIcon>();
				component.SetDef(keyValuePair.Key, slotIndex, null, this.logic);
				component.ShowAddIcon(false);
				component.ShowDisbandButton(false);
				component.ShowLockIcon(false);
				component.SetNumItems((int)keyValuePair.Value);
			}
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x00100D00 File Offset: 0x000FEF00
	private void Update()
	{
		if (this.logic == null || this.logic.IsValid())
		{
			if (this.m_Invalidate)
			{
				this.m_Invalidate = false;
				this.Refresh(true);
			}
			if (this.m_InvalidateTotals)
			{
				this.m_InvalidateTotals = false;
				this.RefreshEstimationText();
				this.RefreshManpowerAndStats();
				this.RefreshLevels();
				this.RefreshTier();
				this.RefreshSiegeEquipment();
			}
			if (this.m_InvalidateFortifications)
			{
				this.m_InvalidateFortifications = false;
				this.RefreshFortifications();
			}
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		baseUI.DestroyTooltipInstance(base.gameObject);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00100D90 File Offset: 0x000FEF90
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "fortification_health_changed")
		{
			this.m_InvalidateFortifications = true;
			return;
		}
		if (message == "armies_changed")
		{
			this.m_Invalidate = true;
			return;
		}
		if (!(message == "changed"))
		{
			return;
		}
		this.m_InvalidateTotals = true;
	}

	// Token: 0x040010EA RID: 4330
	[UIFieldTarget("id_BattleEstimation")]
	private UIBattleEstimationBar m_EstimationBar;

	// Token: 0x040010EB RID: 4331
	[UIFieldTarget("id_LeftKingdomCrest")]
	private UIKingdomIcon m_LeftKingdomCrest;

	// Token: 0x040010EC RID: 4332
	[UIFieldTarget("id_LeftKingdomSupporterCrest")]
	private UIKingdomIcon m_LeftKingdomSupporterCrest;

	// Token: 0x040010ED RID: 4333
	[UIFieldTarget("id_RightKingdomCrest")]
	private UIKingdomIcon m_RightKingdomCrest;

	// Token: 0x040010EE RID: 4334
	[UIFieldTarget("id_RightKingdomSupporterCrest")]
	private UIKingdomIcon m_RightKingdomSupporterCrest;

	// Token: 0x040010EF RID: 4335
	[UIFieldTarget("id_GoalContainer")]
	private GameObject m_GoalContainer;

	// Token: 0x040010F0 RID: 4336
	[UIFieldTarget("id_FortificationsContainer")]
	private GameObject m_FortificationsContainer;

	// Token: 0x040010F1 RID: 4337
	[UIFieldTarget("id_AttritionContainer")]
	private GameObject m_AttritionContainer;

	// Token: 0x040010F2 RID: 4338
	[UIFieldTarget("id_EquipmentContainer")]
	private GameObject m_EquipmentContainer;

	// Token: 0x040010F3 RID: 4339
	[UIFieldTarget("id_EquipmentLeft")]
	private GameObject m_EquipmentLeft;

	// Token: 0x040010F4 RID: 4340
	[UIFieldTarget("id_EquipmentRight")]
	private GameObject m_EquipmentRight;

	// Token: 0x040010F5 RID: 4341
	private GameObject equipment_slot_prefab;

	// Token: 0x040010F6 RID: 4342
	[UIFieldTarget("id_LevelsLeft")]
	private Transform m_LevelsLeft;

	// Token: 0x040010F7 RID: 4343
	[UIFieldTarget("id_LevelsRight")]
	private Transform m_LevelsRight;

	// Token: 0x040010F8 RID: 4344
	[UIFieldTarget("id_LevelsLeftNone")]
	private GameObject m_LevelsLeftNone;

	// Token: 0x040010F9 RID: 4345
	[UIFieldTarget("id_LevelsRightNone")]
	private GameObject m_LevelsRightNone;

	// Token: 0x040010FA RID: 4346
	[UIFieldTarget("id_TierLeft")]
	private Transform m_TierLeft;

	// Token: 0x040010FB RID: 4347
	[UIFieldTarget("id_TierRight")]
	private Transform m_TierRight;

	// Token: 0x040010FC RID: 4348
	[UIFieldTarget("id_TierLeftNone")]
	private GameObject m_TierLeftNone;

	// Token: 0x040010FD RID: 4349
	[UIFieldTarget("id_TierRightNone")]
	private GameObject m_TierRightNone;

	// Token: 0x040010FE RID: 4350
	[UIFieldTarget("id_NavalPowerLeft")]
	private Transform m_NavalPowerLeft;

	// Token: 0x040010FF RID: 4351
	[UIFieldTarget("id_NavalPowerRight")]
	private Transform m_NavalPowerRight;

	// Token: 0x04001100 RID: 4352
	[UIFieldTarget("id_NavalPowerLeftNone")]
	private GameObject m_NavalPowerLeftNone;

	// Token: 0x04001101 RID: 4353
	[UIFieldTarget("id_NavalPowerRightNone")]
	private GameObject m_NavalPowerRightNone;

	// Token: 0x04001102 RID: 4354
	[UIFieldTarget("id_NavalPowerContainer")]
	private GameObject m_NavalPowerContainer;

	// Token: 0x04001103 RID: 4355
	private float max_naval_power;

	// Token: 0x04001104 RID: 4356
	[UIFieldTarget("id_SiegeStrengthLeft")]
	private Transform m_SiegeStrengthLeft;

	// Token: 0x04001105 RID: 4357
	[UIFieldTarget("id_SiegeStrengthRight")]
	private Transform m_SiegeStrengthRight;

	// Token: 0x04001106 RID: 4358
	[UIFieldTarget("id_SiegeStrengthLeftNone")]
	private GameObject m_SiegeStrengthLeftNone;

	// Token: 0x04001107 RID: 4359
	[UIFieldTarget("id_SiegeStrengthRightNone")]
	private GameObject m_SiegeStrengthRightNone;

	// Token: 0x04001108 RID: 4360
	[UIFieldTarget("id_SiegeStrengthContainer")]
	private GameObject m_SiegeStrengthContainer;

	// Token: 0x04001109 RID: 4361
	[UIFieldTarget("id_UnitCounterLeft")]
	private Transform m_UnitCounterLeft;

	// Token: 0x0400110A RID: 4362
	[UIFieldTarget("id_UnitCounterRight")]
	private Transform m_UnitCounterRight;

	// Token: 0x0400110B RID: 4363
	[UIFieldTarget("id_UnitCounterLeftNone")]
	private GameObject m_UnitCounterLeftNone;

	// Token: 0x0400110C RID: 4364
	[UIFieldTarget("id_UnitCounterRightNone")]
	private GameObject m_UnitCounterRightNone;

	// Token: 0x0400110D RID: 4365
	[UIFieldTarget("id_BattleName")]
	private TextMeshProUGUI m_BattleName;

	// Token: 0x0400110E RID: 4366
	[UIFieldTarget("id_ValueEstimation")]
	private TextMeshProUGUI m_ValueEstimation;

	// Token: 0x0400110F RID: 4367
	[UIFieldTarget("id_BattleDescription")]
	private TextMeshProUGUI m_BattleDescription;

	// Token: 0x04001110 RID: 4368
	[UIFieldTarget("id_ManpowerDescription")]
	private TextMeshProUGUI m_ManpowerDescription;

	// Token: 0x04001111 RID: 4369
	[UIFieldTarget("id_ManpowerLeft")]
	private TextMeshProUGUI m_ManpowerLeft;

	// Token: 0x04001112 RID: 4370
	[UIFieldTarget("id_ManpowerRight")]
	private TextMeshProUGUI m_ManpowerRight;

	// Token: 0x04001113 RID: 4371
	[UIFieldTarget("id_UnitTypeDescription")]
	private TextMeshProUGUI m_UnitTypeDescription;

	// Token: 0x04001114 RID: 4372
	[UIFieldTarget("id_UnitTypeLeft")]
	private TextMeshProUGUI m_UnitTypeLeft;

	// Token: 0x04001115 RID: 4373
	[UIFieldTarget("id_UnitTypeRight")]
	private TextMeshProUGUI m_UnitTypeRight;

	// Token: 0x04001116 RID: 4374
	[UIFieldTarget("id_Goal")]
	private TextMeshProUGUI m_Goal;

	// Token: 0x04001117 RID: 4375
	[UIFieldTarget("id_Fortifications")]
	private TextMeshProUGUI m_Fortifications;

	// Token: 0x04001118 RID: 4376
	[UIFieldTarget("id_LevelsDescription")]
	private TextMeshProUGUI m_LevelsDescription;

	// Token: 0x04001119 RID: 4377
	[UIFieldTarget("id_TierDescription")]
	private TextMeshProUGUI m_TierDescription;

	// Token: 0x0400111A RID: 4378
	[UIFieldTarget("id_UnitCounterDescription")]
	private TextMeshProUGUI m_UnitCounterDescription;

	// Token: 0x0400111B RID: 4379
	[UIFieldTarget("id_NavalPowerDescription")]
	private TextMeshProUGUI m_NavalPowerDescription;

	// Token: 0x0400111C RID: 4380
	[UIFieldTarget("id_SiegeStrengthDescription")]
	private TextMeshProUGUI m_SiegeStrengthDescription;

	// Token: 0x0400111D RID: 4381
	[UIFieldTarget("id_Attrition")]
	private TextMeshProUGUI m_Attrition;

	// Token: 0x0400111E RID: 4382
	[UIFieldTarget("id_EquipmentDescription")]
	private TextMeshProUGUI m_EquipmentDescription;

	// Token: 0x0400111F RID: 4383
	private Logic.Battle logic;

	// Token: 0x04001120 RID: 4384
	private Vars vars;

	// Token: 0x04001121 RID: 4385
	private bool m_Initialzed;

	// Token: 0x04001122 RID: 4386
	private int[] logic_sides = new int[2];

	// Token: 0x04001123 RID: 4387
	private ushort[] max_manpower = new ushort[2];

	// Token: 0x04001124 RID: 4388
	private ushort[] manpower = new ushort[2];

	// Token: 0x04001125 RID: 4389
	private ushort[] ranged = new ushort[2];

	// Token: 0x04001126 RID: 4390
	private ushort[] cavalry = new ushort[2];

	// Token: 0x04001127 RID: 4391
	private ushort[] ranged_cavalry = new ushort[2];

	// Token: 0x04001128 RID: 4392
	private ushort[] infantry = new ushort[2];

	// Token: 0x04001129 RID: 4393
	private ushort[] defense = new ushort[2];

	// Token: 0x0400112A RID: 4394
	private ushort[] militia = new ushort[2];

	// Token: 0x0400112B RID: 4395
	private ushort[] noble = new ushort[2];

	// Token: 0x0400112C RID: 4396
	private bool m_Invalidate;

	// Token: 0x0400112D RID: 4397
	private bool m_InvalidateTotals;

	// Token: 0x0400112E RID: 4398
	private bool m_InvalidateFortifications;
}
