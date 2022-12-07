using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001A7 RID: 423
public class UIUnitSlot : Hotspot
{
	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06001810 RID: 6160 RVA: 0x000EA933 File Offset: 0x000E8B33
	// (set) Token: 0x06001811 RID: 6161 RVA: 0x000EA93B File Offset: 0x000E8B3B
	public Logic.Unit UnitInstance { get; private set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06001812 RID: 6162 RVA: 0x000EA944 File Offset: 0x000E8B44
	// (set) Token: 0x06001813 RID: 6163 RVA: 0x000EA94C File Offset: 0x000E8B4C
	public Logic.Unit.Def UnitDef { get; private set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06001814 RID: 6164 RVA: 0x000EA955 File Offset: 0x000E8B55
	// (set) Token: 0x06001815 RID: 6165 RVA: 0x000EA95D File Offset: 0x000E8B5D
	public Logic.Army Army { get; private set; }

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06001816 RID: 6166 RVA: 0x000EA966 File Offset: 0x000E8B66
	// (set) Token: 0x06001817 RID: 6167 RVA: 0x000EA96E File Offset: 0x000E8B6E
	public Castle Castle { get; private set; }

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06001818 RID: 6168 RVA: 0x000EA977 File Offset: 0x000E8B77
	// (set) Token: 0x06001819 RID: 6169 RVA: 0x000EA97F File Offset: 0x000E8B7F
	public int SlotIndex { get; private set; }

	// Token: 0x0600181A RID: 6170 RVA: 0x000EA988 File Offset: 0x000E8B88
	private void Init()
	{
		this.Army = null;
		this.Castle = null;
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.vars = new Vars();
		if (this.m_DisbandButton != null)
		{
			Tooltip.Get(this.m_DisbandButton.gameObject, true).SetDef("DissmisUnitTooltip", this.vars);
			this.m_DisbandButton.onClick = new BSGButton.OnClick(this.HandleOnDisband);
		}
		if (this.m_Button_Heal != null)
		{
			Tooltip.Get(this.m_Button_Heal.gameObject, true).SetDef("HealGarrisonUnitTooltip", this.vars);
			this.m_Button_Heal.onClick = new BSGButton.OnClick(this.HandleOnHeal);
		}
		if (this.m_StarPrototype != null)
		{
			this.m_StarPrototype.gameObject.SetActive(false);
		}
		if (this.m_TutorialFirstFreeSlot != null)
		{
			this.m_TutorialFirstFreeSlot.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x000EAA8F File Offset: 0x000E8C8F
	protected override void OnEnable()
	{
		base.OnEnable();
		UIUnitSlot.all.Add(this);
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x000EAAA2 File Offset: 0x000E8CA2
	protected override void OnDisable()
	{
		UIUnitSlot.all.Remove(this);
		this.SetSelected(false, true);
		base.OnDisable();
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x000EAABF File Offset: 0x000E8CBF
	private void OnDestroy()
	{
		this.UnitDef = null;
		this.UnitInstance = null;
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x000EAACF File Offset: 0x000E8CCF
	public void SetDef(Logic.Unit.Def unitDef, float damage, float shock = 0f)
	{
		this.SetDef(unitDef, -1);
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x000EAADC File Offset: 0x000E8CDC
	public void SetDef(Logic.Unit.Def unitDef, int slotIndex)
	{
		this.Init();
		this.SlotIndex = slotIndex;
		this.UnitDef = unitDef;
		Hotspot.EndDrag(this);
		if (unitDef == null)
		{
			this.SetEmpty();
		}
		else
		{
			this.SetAsOccupied();
		}
		UIArmyHealthBar statusBar = this.m_StatusBar;
		if (statusBar != null)
		{
			statusBar.SetData(null);
		}
		this.Refresh();
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x000EAB2C File Offset: 0x000E8D2C
	public void SetUnitInstance(Logic.Unit unit, int slotIndex, Logic.Army army, Castle castle)
	{
		this.Init();
		if (unit != this.UnitInstance)
		{
			Hotspot.EndDrag(this);
		}
		this.SlotIndex = slotIndex;
		this.UnitInstance = unit;
		this.Army = army;
		this.Castle = (castle ?? ((army != null) ? army.castle : null));
		this.vars.Clear();
		this.vars.obj = new Value(unit);
		this.vars.Set<Logic.Army>("army", this.Army);
		this.vars.Set<Castle>("castle", this.Castle);
		this.vars.Set<UIUnitSlot>("slot", this);
		if (unit == null)
		{
			this.SetEmpty();
		}
		else
		{
			this.SetAsOccupied();
		}
		UIArmyHealthBar statusBar = this.m_StatusBar;
		if (statusBar != null)
		{
			statusBar.SetData(unit);
		}
		this.Refresh();
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x000EABFD File Offset: 0x000E8DFD
	public void EnableTutorialHighlight(bool enable)
	{
		if (this.m_TutorialFirstFreeSlot != null)
		{
			this.m_TutorialFirstFreeSlot.SetActive(enable);
		}
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x000EAC1C File Offset: 0x000E8E1C
	public static Value SetupAttributeHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		Vars vars = ((ht != null) ? ht.vars : null) as Vars;
		if (vars == null)
		{
			return Value.Unknown;
		}
		UIHyperText.Row row = vars.obj.Get<UIHyperText.Row>();
		if (row == null)
		{
			return Value.Unknown;
		}
		DT.Field def = row.def;
		DT.Field field = (def != null) ? def.FindChild("text", null, true, true, true, '.') : null;
		if (field == null)
		{
			return Value.Unknown;
		}
		vars.Set<object>("attribute_def", row.instance_obj);
		vars.Set<DT.Field>("row_text", field);
		return true;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x000EACAC File Offset: 0x000E8EAC
	public static Value PopulateAttributeBreakdown(UIHyperText.CallbackParams arg)
	{
		IVars vars = arg.GetVar("unit", null, true).Get<IVars>();
		if (vars == null)
		{
			return Value.Unknown;
		}
		Logic.Unit.Def def = vars.GetVar("unit_def", null, true).Get<Logic.Unit.Def>();
		if (def == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht = arg.ht;
		DT.Field field = ((ht != null) ? ht.RootVars() : null).GetVar("attribute_def", null, true).Get<DT.Field>();
		if (field == null)
		{
			return Value.Unknown;
		}
		DT.Field field2 = field.FindChild("breakdown", null, true, true, true, '.');
		if (field2 == null)
		{
			return Value.Unknown;
		}
		Logic.Unit unit = vars.GetVar("unit", null, true).Get<Logic.Unit>();
		Logic.Army army = vars.GetVar("army", null, true).Get<Logic.Army>();
		Garrison garrison = vars.GetVar("garrison", null, true).Get<Garrison>();
		Logic.Kingdom kingdom;
		if ((kingdom = ((army != null) ? army.GetKingdom() : null)) == null)
		{
			if (garrison == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.Settlement settlement = garrison.settlement;
				kingdom = ((settlement != null) ? settlement.GetKingdom() : null);
			}
		}
		Logic.Kingdom kingdom2 = kingdom;
		Logic.Squad squad = arg.GetVar("squad", null, true).Get<Logic.Squad>();
		BattleSimulation.Squad squad2 = (squad != null) ? squad.simulation : null;
		if (squad2 == null)
		{
			squad2 = ((unit != null) ? unit.simulation : null);
		}
		UIUnitSlot.<>c__DisplayClass54_0 CS$<>8__locals1;
		CS$<>8__locals1.flat_sum = 0f;
		UIUnitSlot.tmp_attribute_factors.Clear();
		Logic.Kingdom kingdom3;
		if (army == null)
		{
			kingdom3 = null;
		}
		else
		{
			Logic.Character leader = army.leader;
			kingdom3 = ((leader != null) ? leader.GetKingdom() : null);
		}
		Logic.Kingdom kingdom4 = kingdom3;
		if (kingdom4 == null)
		{
			Logic.Kingdom kingdom5;
			if (garrison == null)
			{
				kingdom5 = null;
			}
			else
			{
				Logic.Settlement settlement2 = garrison.settlement;
				kingdom5 = ((settlement2 != null) ? settlement2.GetKingdom() : null);
			}
			kingdom4 = kingdom5;
		}
		Value val = field.GetVar("base_val", arg, true);
		DT.Field source = global::Defs.GetDefField("UnitAttributeTooltip", "Base");
		List<DT.Field> list = field2.Children();
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field3 = list[i];
				if (field3.key == "base")
				{
					Value value = field3.Value(arg, true, true);
					if (!value.is_unknown)
					{
						val = value;
					}
					DT.Field field4 = field3.FindChild("name", null, true, true, true, '.');
					if (field4 != null)
					{
						source = field4;
					}
				}
				else
				{
					string text = field3.Type();
					if (!string.IsNullOrEmpty(text))
					{
						if (text == "custom")
						{
							string key = field3.key;
							if (key == "morale_factors" && ((squad2 != null) ? squad2.permanent_morale_factors : null) != null)
							{
								for (int j = 0; j < squad2.permanent_morale_factors.Length; j++)
								{
									float value2 = squad2.permanent_morale_factors[j];
									DT.Field field5 = squad2.simulation.def.morale_factors[j].field;
									if (!(field5.key == "initial"))
									{
										DT.Field field6 = field5.FindChild("description", null, true, true, true, '.');
										if (field6 != null)
										{
											UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field6, value2, squad2.simulation.def.morale_factors[j].field, ref CS$<>8__locals1);
										}
									}
								}
							}
						}
						else if (text == "bonus")
						{
							Logic.Unit.BonusDef bonusDef = def.FindBonusDef(field3.key);
							if (bonusDef != null && bonusDef.Validate((unit != null) ? unit.battle : null, -1))
							{
								if (army != null)
								{
									Stats stats;
									if (army == null)
									{
										stats = null;
									}
									else
									{
										Logic.Character leader2 = army.leader;
										stats = ((leader2 != null) ? leader2.stats : null);
									}
									UIUnitSlot.<PopulateAttributeBreakdown>g__AddStatsFactors|54_4(stats, bonusDef.character_stats, field3, ref CS$<>8__locals1);
								}
								else if (garrison != null && bonusDef.realm_stats != null)
								{
									Logic.Settlement settlement3 = garrison.settlement;
									Stats stats2;
									if (settlement3 == null)
									{
										stats2 = null;
									}
									else
									{
										Logic.Realm realm = settlement3.GetRealm();
										stats2 = ((realm != null) ? realm.stats : null);
									}
									UIUnitSlot.<PopulateAttributeBreakdown>g__AddStatsFactors|54_4(stats2, bonusDef.realm_stats, field3, ref CS$<>8__locals1);
								}
								else if (kingdom2 != null)
								{
									UIUnitSlot.<PopulateAttributeBreakdown>g__AddStatsFactors|54_4((kingdom2 != null) ? kingdom2.stats : null, bonusDef.kingdom_stats, field3, ref CS$<>8__locals1);
								}
								if (bonusDef != null && bonusDef.battle_mod != null && ((unit != null) ? unit.battle : null) != null && unit.battle.battle_bonuses != null && unit.battle.batte_view_game == null)
								{
									int battle_side = unit.battle_side;
									string battle_mod = bonusDef.battle_mod;
									for (int k = 0; k < unit.battle.battle_bonuses.Count; k++)
									{
										BattleBonus.Def def2 = unit.battle.battle_bonuses[k];
										for (int l = 0; l < def2.mods.Count; l++)
										{
											BattleBonus.StatModifier.Def def3 = def2.mods[l];
											if (def3.Validate(battle_side, def, battle_mod))
											{
												DT.Field field7 = def3.field.FindChild("mod_name", null, true, true, true, '.');
												if (field7 == null)
												{
													DT.Field field8 = def3.field;
													DT.Field field9;
													if (field8 == null)
													{
														field9 = null;
													}
													else
													{
														DT.Field parent = field8.parent;
														field9 = ((parent != null) ? parent.FindChild("mod_name", null, true, true, true, '.') : null);
													}
													field7 = field9;
												}
												if (field7 == null)
												{
													DT.Field field10 = def3.field;
													DT.Field field11;
													if (field10 == null)
													{
														field11 = null;
													}
													else
													{
														DT.Field parent2 = field10.parent;
														field11 = ((parent2 != null) ? parent2.parent.FindChild("name", null, true, true, true, '.') : null);
													}
													field7 = field11;
												}
												if (field7 != null)
												{
													UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field7, def3.value, def3.field, ref CS$<>8__locals1);
												}
											}
										}
									}
								}
								if (kingdom4 != null)
								{
									for (int m = 0; m < kingdom4.wars.Count; m++)
									{
										Logic.Kingdom enemyLeader = kingdom4.wars[m].GetEnemyLeader(kingdom4);
										War.Bonus bonus;
										if (enemyLeader != null && War.GetBonusField(kingdom4, enemyLeader, bonusDef.name, out bonus))
										{
											DT.Field field12 = bonus.field.FindChild("mod_name", null, true, true, true, '.');
											if (field12 != null)
											{
												UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field12, bonus.value, bonus.field, ref CS$<>8__locals1);
											}
										}
									}
								}
							}
						}
						else if (text == "var")
						{
							DT.Field field13 = field3.FindChild("name", null, true, true, true, '.');
							if (field13 != null)
							{
								Value val2 = field3.Value(arg, true, true);
								if (val2.is_unknown)
								{
									if (squad2 != null)
									{
										val2 = squad2.GetVar(field3.key, null, true);
									}
									else
									{
										val2 = vars.GetVar(field3.key, null, true);
									}
								}
								UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field13, val2, field3, ref CS$<>8__locals1);
							}
						}
					}
				}
			}
		}
		if (UIUnitSlot.tmp_attribute_factors.Count == 0)
		{
			return Value.Unknown;
		}
		UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(source, val, null, ref CS$<>8__locals1);
		List<Vars> list2 = new List<Vars>(UIUnitSlot.tmp_attribute_factors.Count);
		for (int n = 0; n < UIUnitSlot.tmp_attribute_factors.Count; n++)
		{
			UIUnitSlot.AttributeFactor attributeFactor = UIUnitSlot.tmp_attribute_factors[n];
			if (attributeFactor.perc != 0f)
			{
				if (attributeFactor.flat_field != null)
				{
					attributeFactor.flat = attributeFactor.flat_field.Value(arg, true, true);
				}
				else
				{
					attributeFactor.flat = CS$<>8__locals1.flat_sum * attributeFactor.perc * 0.01f;
				}
			}
			Vars vars2 = new Vars();
			vars2.Set<DT.Field>("source", attributeFactor.source);
			vars2.Set<DT.Field>("source_type", attributeFactor.source_type);
			vars2.Set<float>("value", attributeFactor.flat);
			if (attributeFactor.perc != 0f)
			{
				vars2.Set<float>("perc_bonus", attributeFactor.perc);
			}
			if (attributeFactor.is_base)
			{
				vars2.Set<bool>("is_base", true);
			}
			list2.Add(vars2);
		}
		return new Value(list2);
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x000EB470 File Offset: 0x000E9670
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.vars : null;
		if (vars == null)
		{
			return Value.Unknown;
		}
		Value var = vars.GetVar("unit", null, true);
		Value value = var;
		if (value.is_unknown)
		{
			value = vars.GetVar("obj", null, true);
			if (value.is_unknown)
			{
				return Value.Unknown;
			}
		}
		Logic.Unit unit = null;
		Logic.Unit.Def def = null;
		Logic.Unit.Def def2;
		Logic.Unit unit2;
		DT.Field field;
		if ((def2 = (value.obj_val as Logic.Unit.Def)) != null)
		{
			def = def2;
		}
		else if ((unit2 = (value.obj_val as Logic.Unit)) != null)
		{
			unit = unit2;
			def = unit2.def;
		}
		else if ((field = (value.obj_val as DT.Field)) != null)
		{
			def = Def.Get<Logic.Unit.Def>(field);
		}
		Vars vars2 = vars as Vars;
		if (vars2 == null)
		{
			vars2 = new Vars(vars);
			arg.ht.vars = vars2;
		}
		Logic.Squad squad = vars.GetVar("squad", null, true).Get<Logic.Squad>();
		BattleSimulation.Squad squad2 = (squad != null) ? squad.simulation : null;
		if (squad2 != null)
		{
			unit = ((squad2 != null) ? squad2.unit : null);
			vars2.Set<BattleSimulation.Squad>("unit", squad2);
		}
		else if (!var.is_valid)
		{
			if (unit != null)
			{
				vars2.Set<Logic.Unit>("unit", unit);
			}
			else
			{
				vars2.Set<Logic.Unit.Def>("unit", def);
			}
		}
		UIUnitSlot.FillTTVars(vars2, def, unit);
		return true;
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x000EB5C4 File Offset: 0x000E97C4
	private static void FillTTVars(Vars vars, Logic.Unit.Def def, Logic.Unit unit)
	{
		Castle castle = Vars.Get<Castle>(vars, "castle", null);
		Logic.Army army = Vars.Get<Logic.Army>(vars, "army", null);
		if (army == null)
		{
			army = ((unit != null) ? unit.army : null);
			if (army == null)
			{
				army = ((castle != null) ? castle.army : null);
			}
			vars.Set<Logic.Army>("army", army);
		}
		if (castle == null)
		{
			castle = ((army != null) ? army.castle : null);
			vars.Set<Castle>("castle", castle);
		}
		Logic.Settlement settlement = castle;
		if (settlement == null)
		{
			Logic.Settlement settlement2;
			if (unit == null)
			{
				settlement2 = null;
			}
			else
			{
				BattleSimulation.Squad simulation = unit.simulation;
				if (simulation == null)
				{
					settlement2 = null;
				}
				else
				{
					Garrison garrison = simulation.garrison;
					settlement2 = ((garrison != null) ? garrison.settlement : null);
				}
			}
			settlement = settlement2;
		}
		vars.Set<Logic.Army>("castle_army", (castle != null) ? castle.army : null);
		vars.Set<Logic.Unit>("unit_instance", unit);
		vars.Set<bool>("in_ungovernt_garrison", unit != null && unit.garrison != null && unit.garrison.settlement.GetRealm().castle.governor == null);
		Logic.Kingdom kingdom = Vars.Get<Logic.Kingdom>(vars, "unit_kingdom", null);
		if (kingdom == null)
		{
			kingdom = (((army != null) ? army.GetKingdom() : null) ?? ((castle != null) ? castle.GetKingdom() : null));
			vars.Set<Logic.Kingdom>("unit_kingdom", kingdom);
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		vars.Set<Logic.Kingdom>("kingdom", kingdom2);
		bool flag = kingdom == kingdom2;
		vars.Set<bool>("is_own", flag);
		bool flag2 = army != null && army.IsMercenary();
		if (flag2)
		{
			vars.Set<Logic.Army>("buyer", army.mercenary.selected_buyer);
		}
		else
		{
			vars.Set<Logic.Army>("buyer", army);
		}
		vars.Set<bool>("is_merc", flag2);
		bool val = false;
		bool val2 = false;
		bool val3 = flag;
		if (flag && castle != null && unit == null)
		{
			val = true;
			val2 = true;
		}
		if (flag2)
		{
			val2 = true;
			val3 = true;
			if (army.IsHeadless())
			{
				Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
				if (kingdom3 != null && army.mercenary.former_owner_id == kingdom3.id)
				{
					val2 = false;
				}
			}
		}
		bool flag3 = ((army != null) ? army.battle : null) != null || ((settlement != null) ? settlement.battle : null) != null;
		vars.Set<bool>("in_battle", flag3);
		if (flag3)
		{
			val = false;
			val2 = false;
			val3 = false;
		}
		vars.Set<bool>("show_requirements", val);
		vars.Set<bool>("show_cost", val2);
		vars.Set<bool>("show_upkeep", val3);
		bool flag4 = UIUnitSlot.IsSelectable(unit);
		bool flag5 = UIUnitSlot.IsSelected(unit);
		bool flag6 = UIUnitSlot.IsTheOnlySelected(unit);
		Logic.Object groupSelectionObj = UIUnitSlot.GetGroupSelectionObj(unit);
		Logic.Object otherGroupSelectionObj = UIUnitSlot.GetOtherGroupSelectionObj(unit);
		int num = UIUnitSlot.NumSelected(groupSelectionObj);
		int num2 = UIUnitSlot.NumSelected(otherGroupSelectionObj);
		bool flag7 = (flag5 && num == 1) || (!flag5 && num == 0);
		bool flag8 = flag && UIUnitSlot.TryTransfer(unit, true);
		bool flag9 = flag8 && num2 > 0;
		vars.Set<Logic.Army>("other_army", otherGroupSelectionObj as Logic.Army);
		vars.Set<bool>("is_single_selectable", flag4 && !flag6);
		vars.Set<bool>("is_single_deselectable", flag6);
		vars.Set<bool>("is_multi_selectable", flag4 && !flag5 && UIUnitSlot.selection.Count != 0);
		vars.Set<bool>("is_multi_deselectable", flag5 && UIUnitSlot.selection.Count > 1);
		vars.Set<bool>("is_dragable_to_garrison", !flag3 && flag && otherGroupSelectionObj is Castle);
		vars.Set<bool>("is_dragable_to_army", !flag3 && flag && otherGroupSelectionObj is Logic.Army);
		vars.Set<bool>("is_swapable_in_army", !flag3 && flag && UIUnitSlot.IsSwapableInArmy(unit));
		vars.Set<bool>("is_mergeable_in_army", !flag3 && flag && UIUnitSlot.IsMergeableInArmy(unit));
		vars.Set<bool>("is_mergeable_in_garrison", !flag3 && flag && UIUnitSlot.IsMergeableInGarrison(unit));
		vars.Set<bool>("is_single_transferable", !flag3 && flag8 && (UIUnitSlot.selection.Count == 0 || flag6));
		vars.Set<bool>("is_multi_transferable_selected", !flag3 && flag8 && flag5 && num2 == 0 && num > 1);
		vars.Set<bool>("is_multi_transferable_not_selected", !flag3 && flag8 && !flag5 && num2 == 0 && num > 0);
		vars.Set<bool>("is_single_swapable", !flag3 && flag9 && flag7);
		vars.Set<bool>("is_multi_swapable_selected", !flag3 && flag9 && flag5 && !flag7);
		vars.Set<bool>("is_multi_swapable_not_selected", !flag3 && flag9 && !flag5 && !flag7);
		vars.Set<bool>("has_free_army_slots", army != null && army.CountUnits() < army.MaxUnits());
		vars.Set<bool>("has_free_garrison_slots", castle != null && castle.garrison.units.Count < castle.garrison.SlotCount());
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x000EBAA4 File Offset: 0x000E9CA4
	public static bool IsOwnUnit(Logic.Unit u, bool allow_cheats)
	{
		if (u == null)
		{
			return false;
		}
		if (Game.CheckCheatLevel(Game.CheatLevel.High, "treat non-player units as own", false))
		{
			return true;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return false;
		}
		Logic.Army army = u.army;
		Logic.Kingdom kingdom2;
		if ((kingdom2 = ((army != null) ? army.GetKingdom() : null)) == null)
		{
			Garrison garrison = u.garrison;
			if (garrison == null)
			{
				kingdom2 = null;
			}
			else
			{
				Logic.Settlement settlement = garrison.settlement;
				kingdom2 = ((settlement != null) ? settlement.GetKingdom() : null);
			}
		}
		return kingdom2 == kingdom;
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x000EBB0B File Offset: 0x000E9D0B
	public bool IsEmpty()
	{
		return this.UnitDef == null && this.UnitInstance == null;
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x000EBB20 File Offset: 0x000E9D20
	public bool IsOwn()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return false;
		}
		Logic.Army army = this.Army;
		Logic.Kingdom kingdom2;
		if ((kingdom2 = ((army != null) ? army.GetKingdom() : null)) == null)
		{
			Castle castle = this.Castle;
			kingdom2 = ((castle != null) ? castle.GetKingdom() : null);
		}
		return kingdom2 == kingdom;
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x000EBB68 File Offset: 0x000E9D68
	public static bool IsSwapableInArmy(Logic.Unit u)
	{
		bool flag;
		if (u == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Army army = u.army;
			flag = (((army != null) ? army.units : null) != null);
		}
		if (!flag)
		{
			return false;
		}
		for (int i = 0; i < u.army.units.Count; i++)
		{
			Logic.Unit unit = u.army.units[i];
			if (u != unit && unit.def.type != Logic.Unit.Type.Noble)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x000EBBD4 File Offset: 0x000E9DD4
	public static bool IsMergeableInArmy(Logic.Unit u)
	{
		bool flag;
		if (u == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Army army = u.army;
			flag = (((army != null) ? army.units : null) != null);
		}
		if (!flag)
		{
			return false;
		}
		for (int i = 0; i < u.army.units.Count; i++)
		{
			Logic.Unit unit = u.army.units[i];
			if (u != unit && u.army.CanMergeUnits(u, unit))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x0002C538 File Offset: 0x0002A738
	public static bool IsMergeableInGarrison(Logic.Unit u)
	{
		return false;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x000EBC40 File Offset: 0x000E9E40
	public static bool IsSelectable(Logic.Unit u)
	{
		if (u == null)
		{
			return false;
		}
		Logic.Object groupSelectionObj = UIUnitSlot.GetGroupSelectionObj(u);
		if (groupSelectionObj == null)
		{
			return false;
		}
		Logic.Kingdom kingdom = groupSelectionObj.GetKingdom();
		if (kingdom == null)
		{
			return false;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		return kingdom == kingdom2 && u.battle == null;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x000EBC81 File Offset: 0x000E9E81
	public bool IsSelectable()
	{
		return UIUnitSlot.IsSelectable(this.UnitInstance);
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x000EBC8E File Offset: 0x000E9E8E
	public static Logic.Object GetGroupSelectionObj(Logic.Unit u)
	{
		if (u == null)
		{
			return null;
		}
		if (u.army != null)
		{
			return u.army;
		}
		Garrison garrison = u.garrison;
		if (((garrison != null) ? garrison.settlement : null) != null)
		{
			return u.garrison.settlement;
		}
		return null;
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x000EBCC5 File Offset: 0x000E9EC5
	public Logic.Object GetSameGroupSelectionObj()
	{
		return UIUnitSlot.GetGroupSelectionObj(this.UnitInstance);
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x000EBCD4 File Offset: 0x000E9ED4
	public static Logic.Object GetOtherGroupSelectionObj(Logic.Unit u)
	{
		if (u == null)
		{
			return null;
		}
		if (u.army != null)
		{
			if (u.army.castle != null)
			{
				return u.army.castle;
			}
			if (u.army.interact_target != null)
			{
				return u.army.interact_target;
			}
			if (u.army.interactor != null)
			{
				return u.army.interactor;
			}
			return null;
		}
		else
		{
			Garrison garrison = u.garrison;
			Castle castle;
			if ((castle = (((garrison != null) ? garrison.settlement : null) as Castle)) == null)
			{
				return null;
			}
			if (castle.army != null)
			{
				return castle.army;
			}
			return null;
		}
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000EBD68 File Offset: 0x000E9F68
	public Logic.Object GetOtherGroupSelectionObj()
	{
		return UIUnitSlot.GetOtherGroupSelectionObj(this.UnitInstance);
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x000EBD75 File Offset: 0x000E9F75
	public static bool IsSelected(Logic.Unit u)
	{
		return u != null && UIUnitSlot.selection.Contains(u);
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x000EBD8C File Offset: 0x000E9F8C
	public bool IsSelected()
	{
		return UIUnitSlot.IsSelected(this.UnitInstance);
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x000EBD9C File Offset: 0x000E9F9C
	public static bool IsTheOnlySelected(Logic.Unit u)
	{
		if (u == null)
		{
			return false;
		}
		if (UIUnitSlot.selection.Count != 1)
		{
			return false;
		}
		Logic.Unit unit = UIUnitSlot.selection[0];
		return u == unit;
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x000EBDD0 File Offset: 0x000E9FD0
	public static bool ClearSelection(bool notify_selection_changed)
	{
		if (UIUnitSlot.selection.Count == 0)
		{
			return false;
		}
		UIUnitSlot.selection.Clear();
		if (notify_selection_changed)
		{
			UIUnitSlot.OnSelectionChanged();
		}
		return true;
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x000EBDF4 File Offset: 0x000E9FF4
	public static bool ClearSelection(Logic.Object grp_obj, bool notify_selection_changed)
	{
		if (grp_obj == null)
		{
			return false;
		}
		bool flag = false;
		for (int i = UIUnitSlot.selection.Count - 1; i >= 0; i--)
		{
			if (UIUnitSlot.GetGroupSelectionObj(UIUnitSlot.selection[i]) == grp_obj)
			{
				UIUnitSlot.selection.RemoveAt(i);
				flag = true;
			}
		}
		if (flag && notify_selection_changed)
		{
			UIUnitSlot.OnSelectionChanged();
		}
		return flag;
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x000EBE4A File Offset: 0x000EA04A
	public bool ClearSameGroupSelection(bool notify_selection_changed)
	{
		return UIUnitSlot.ClearSelection(this.GetSameGroupSelectionObj(), notify_selection_changed);
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x000EBE58 File Offset: 0x000EA058
	public static int NumSelected(Logic.Object grp_obj)
	{
		if (grp_obj == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < UIUnitSlot.selection.Count; i++)
		{
			if (UIUnitSlot.GetGroupSelectionObj(UIUnitSlot.selection[i]) == grp_obj)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x000EBE99 File Offset: 0x000EA099
	public int NumSelectedInSameGroup()
	{
		return UIUnitSlot.NumSelected(this.GetSameGroupSelectionObj());
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x000EBEA6 File Offset: 0x000EA0A6
	public int NumSelectedInOtherGroup()
	{
		return UIUnitSlot.NumSelected(this.GetOtherGroupSelectionObj());
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x000EBEB4 File Offset: 0x000EA0B4
	public static int GetNumFreeSlots(Logic.Object grp_obj)
	{
		if (grp_obj == null)
		{
			return 0;
		}
		Logic.Army army;
		if ((army = (grp_obj as Logic.Army)) != null)
		{
			int num = army.units.Count - 1;
			int num2 = army.MaxUnits() - num;
			if (num2 < 0)
			{
				return 0;
			}
			return num2;
		}
		else
		{
			Castle castle;
			if ((castle = (grp_obj as Castle)) == null)
			{
				return 0;
			}
			if (castle.garrison == null)
			{
				return 0;
			}
			int count = castle.garrison.units.Count;
			int num3 = castle.garrison.MaxSlotCount() - count;
			if (num3 < 0)
			{
				return 0;
			}
			return num3;
		}
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x000EBF30 File Offset: 0x000EA130
	public int GetFreeSlotsInSameGroup()
	{
		return UIUnitSlot.GetNumFreeSlots(this.GetSameGroupSelectionObj());
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x000EBF3D File Offset: 0x000EA13D
	public int GetFreeSlotsInOtherGroup()
	{
		return UIUnitSlot.GetNumFreeSlots(this.GetOtherGroupSelectionObj());
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x000EBF4C File Offset: 0x000EA14C
	public static bool SetSelected(Logic.Unit unit, bool selected, bool notify_selection_changed)
	{
		if (UIUnitSlot.lock_selection)
		{
			return false;
		}
		bool flag = UIUnitSlot.IsSelected(unit);
		if (selected == flag)
		{
			return false;
		}
		if (unit == null)
		{
			return false;
		}
		if (selected)
		{
			UIUnitSlot.selection.Add(unit);
		}
		else
		{
			UIUnitSlot.selection.Remove(unit);
		}
		if (notify_selection_changed)
		{
			UIUnitSlot.OnSelectionChanged();
		}
		return true;
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x000EBF99 File Offset: 0x000EA199
	public bool SetSelected(bool selected, bool notify_selection_changed)
	{
		return UIUnitSlot.SetSelected(this.UnitInstance, selected, notify_selection_changed);
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x000EBFA8 File Offset: 0x000EA1A8
	public void SetFocused(bool focused)
	{
		this.m_IsFocused = focused;
		this.UpdateHighlight();
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x000EBFB8 File Offset: 0x000EA1B8
	public static void OnSelectionChanged()
	{
		for (int i = 0; i < UIUnitSlot.all.Count; i++)
		{
			UIUnitSlot uiunitSlot = UIUnitSlot.all[i];
			uiunitSlot.UpdateHighlight();
			BaseUI baseUI = BaseUI.Get();
			if (baseUI != null)
			{
				baseUI.RefreshTooltip(Tooltip.Get(uiunitSlot.gameObject, false), false);
			}
		}
	}

	// Token: 0x06001842 RID: 6210 RVA: 0x000EC009 File Offset: 0x000EA209
	private void ShowDisbandButton(bool shown, bool update_highlight)
	{
		this.m_ShowDisband = shown;
		if (update_highlight)
		{
			this.UpdateHighlight();
		}
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x000EC01B File Offset: 0x000EA21B
	private void ShowHealButton(bool shown)
	{
		this.m_ShowHeal = shown;
		this.UpdateHighlight();
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x000EC02A File Offset: 0x000EA22A
	private void ShowAddIcon(bool shown)
	{
		if (this.m_ShowAddIcon == shown)
		{
			return;
		}
		this.m_ShowAddIcon = shown;
		if (this.m_IconAdd != null)
		{
			this.m_IconAdd.gameObject.SetActive(shown);
		}
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x000EC05C File Offset: 0x000EA25C
	private void ShowMergeIcon(bool shown)
	{
		if (this.m_ShowMergeIcon == shown)
		{
			return;
		}
		this.m_ShowMergeIcon = shown;
		if (this.m_IconMerge != null)
		{
			this.m_IconMerge.gameObject.SetActive(shown);
		}
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x000EC090 File Offset: 0x000EA290
	private void UpdateState(bool update_highlight)
	{
		this.ShowAddIcon(this.UnitInstance == null);
		bool flag = UIUnitSlot.IsOwnUnit(this.UnitInstance, true);
		this.ShowDisbandButton(flag, false);
		this.ShowHealButton(flag && this.CanBeHealed());
		if (update_highlight)
		{
			this.UpdateHighlight();
		}
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x000EC0DC File Offset: 0x000EA2DC
	private void Refresh()
	{
		this.UpdateState(false);
		this.UpdateLevel(false);
		this.UpdateHighlight();
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x000EC0F4 File Offset: 0x000EA2F4
	private void HandleOnDisband(BSGButton btn)
	{
		if (this.UnitInstance == null)
		{
			return;
		}
		if (this.Army == null && this.Castle == null)
		{
			return;
		}
		Logic.Army army = this.Army;
		if (army != null && army.IsMercenary())
		{
			return;
		}
		Logic.Army army2 = this.Army;
		if (((army2 != null) ? army2.GetKingdom() : null) != BaseUI.LogicKingdom())
		{
			Castle castle = this.Castle;
			if (((castle != null) ? castle.GetKingdom() : null) != BaseUI.LogicKingdom())
			{
				return;
			}
		}
		if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
		{
			this.DisbandUnitNoConfirmation();
			return;
		}
		MessageWnd.Create("ConfirmDisbandUnitMessage", new Vars(this.UnitInstance), null, new MessageWnd.OnButton(this.OnDisbandUnit)).on_update = new MessageWnd.OnUpdate(this.ValidateDisbandConfirmation);
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x000EC1B8 File Offset: 0x000EA3B8
	private void ValidateDisbandConfirmation(MessageWnd wnd)
	{
		Logic.Unit unit = (((wnd != null) ? wnd.vars : null) == null) ? null : (wnd.vars.obj.obj_val as Logic.Unit);
		if (this == null || base.gameObject == null || !base.gameObject.activeInHierarchy || unit != this.UnitInstance)
		{
			wnd.Close(false);
		}
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x000EC220 File Offset: 0x000EA420
	private bool OnDisbandUnit(MessageWnd wnd, string btn_id)
	{
		Logic.Unit unit = (((wnd != null) ? wnd.vars : null) == null) ? null : (wnd.vars.obj.obj_val as Logic.Unit);
		wnd.CloseAndDismiss(true);
		if (btn_id != "ok")
		{
			return true;
		}
		if (unit == null)
		{
			return true;
		}
		if (unit.army != null)
		{
			unit.OnAssignedAnalytics(unit.army, null, "army", "delete");
			unit.army.DelUnit(unit, true);
		}
		else
		{
			if (unit.garrison == null)
			{
				return true;
			}
			unit.OnAssignedAnalytics(null, unit.garrison, "garrison", "delete");
			unit.garrison.DelUnit(unit, true);
		}
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("army_disband", null, "", true, true, true, '.') : null, null);
		return true;
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x000EC2F8 File Offset: 0x000EA4F8
	private void DisbandUnitNoConfirmation()
	{
		if (this.UnitInstance == null)
		{
			return;
		}
		if (this.UnitInstance.army != null)
		{
			this.UnitInstance.OnAssignedAnalytics(this.UnitInstance.army, null, "army", "delete");
			this.UnitInstance.army.DelUnit(this.UnitInstance, true);
			return;
		}
		if (this.UnitInstance.garrison != null)
		{
			this.UnitInstance.OnAssignedAnalytics(null, this.UnitInstance.garrison, "garrison", "delete");
			this.UnitInstance.garrison.DelUnit(this.UnitInstance, true);
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x000EC39C File Offset: 0x000EA59C
	private void HandleOnHeal(BSGButton btn)
	{
		if (this.UnitInstance == null)
		{
			return;
		}
		if (this.Castle == null)
		{
			return;
		}
		if (this.Army == null)
		{
			if (this.Castle.garrison == null)
			{
				return;
			}
			int num = this.Castle.garrison.units.IndexOf(this.UnitInstance);
			if (num != -1)
			{
				this.Castle.HealGarrisonUnit(num);
			}
			return;
		}
		else
		{
			int num = this.Army.units.IndexOf(this.UnitInstance);
			if (num == -1)
			{
				return;
			}
			Logic.Character leader = this.Army.leader;
			Action action;
			if (leader == null)
			{
				action = null;
			}
			else
			{
				Actions actions = leader.actions;
				action = ((actions != null) ? actions.Find("HealArmyUnitAction") : null);
			}
			Action action2 = action;
			if (action2 == null)
			{
				return;
			}
			if (action2.args != null)
			{
				action2.args = null;
			}
			action2.AddArg(ref action2.args, num, 0);
			action2.Execute(null);
			return;
		}
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x000EC474 File Offset: 0x000EA674
	private void SetEmpty()
	{
		if (this.m_UnitLevel != null)
		{
			this.m_UnitLevel.gameObject.SetActive(false);
		}
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(false);
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Army army = this.Army;
		Logic.Kingdom kingdom2;
		if ((kingdom2 = ((army != null) ? army.GetKingdom() : null)) == null)
		{
			Castle castle = this.Castle;
			kingdom2 = ((castle != null) ? castle.GetKingdom() : null);
		}
		Logic.Kingdom kingdom3 = kingdom2;
		if (kingdom3 != null && kingdom3 == kingdom)
		{
			Tooltip.Get(base.gameObject, true).SetDef("EmptyUnitSlotTooltip", this.vars);
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, false);
		if (tooltip == null)
		{
			return;
		}
		tooltip.Clear(true);
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000EC52C File Offset: 0x000EA72C
	private void SetAsOccupied()
	{
		if (this.m_UnitLevel != null)
		{
			this.m_UnitLevel.gameObject.SetActive(true);
		}
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(true);
		}
		if (this.m_Mercenary != null)
		{
			this.m_Mercenary.gameObject.SetActive(this.UnitInstance != null && this.UnitInstance.mercenary);
		}
		if (this.UnitInstance != null)
		{
			if (this.m_UnitIcon != null)
			{
				this.m_UnitIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.UnitInstance.def.dt_def.field, "icon", null);
				this.m_IconColor = Color.white;
				if (this.UnitInstance.simulation != null)
				{
					if (this.UnitInstance.simulation.state == BattleSimulation.Squad.State.Dead)
					{
						this.m_IconColor = new Color32(byte.MaxValue, 128, 128, 192);
					}
					else if (this.UnitInstance.simulation.state == BattleSimulation.Squad.State.Fled)
					{
						this.m_IconColor = new Color32(128, 128, 128, 192);
					}
				}
			}
			this.vars.obj = new Value(this.UnitInstance);
		}
		else if (this.UnitDef != null)
		{
			this.m_UnitIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.UnitDef.field, "icon", null);
			this.vars.obj = this.UnitDef;
			this.vars.Set<Logic.Kingdom>("kingdom", BaseUI.LogicKingdom());
		}
		Tooltip.Get(base.gameObject, true).SetDef("UnitTooltip", this.vars);
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000EC704 File Offset: 0x000EA904
	private void UpdateLevel(bool update_highlight)
	{
		if (this.m_UnitLevel == null)
		{
			return;
		}
		int i = (this.UnitInstance != null) ? this.UnitInstance.level : 0;
		this.m_LevelBorderNormal = global::Defs.GetObj<Sprite>(i, "UnitSlot", "level_border", null);
		this.m_LevelBorderSelected = global::Defs.GetObj<Sprite>(i, "UnitSlot", "level_border_selected", null);
		if (this.m_StarPrototype != null && this.m_LevelStarsContainer != null)
		{
			float num = (this.m_LevelStarsStartAmgle - (float)(i - 1) * this.m_LevelStarsStepAnge / 2f) * 0.017453292f;
			while (i > this.m_LevelStars.Count)
			{
				GameObject gameObject = global::Common.Spawn(this.m_StarPrototype, this.m_LevelStarsContainer, false, "");
				if (gameObject == null)
				{
					break;
				}
				this.m_LevelStars.Add(gameObject);
			}
			float num2 = this.m_LevelStarsContainer.rect.width / 2f + this.m_LevelStarsCenterOffset;
			for (int j = 0; j < this.m_LevelStars.Count; j++)
			{
				this.m_LevelStars[j].gameObject.SetActive(j < i);
				float x = Mathf.Cos(num) * num2;
				float y = Mathf.Sin(num) * num2;
				this.m_LevelStars[j].transform.localPosition = new Vector3(x, y, 0f);
				num += this.m_LevelStarsStepAnge * 0.017453292f;
			}
		}
		if (update_highlight)
		{
			this.UpdateHighlight();
		}
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000EC88C File Offset: 0x000EAA8C
	private void Update()
	{
		if (this.UnitInstance != null)
		{
			if (this.m_UnitIcon != null && this.UnitInstance.simulation != null)
			{
				this.m_IconColor = Color.white;
				if (this.UnitInstance.simulation.state == BattleSimulation.Squad.State.Dead)
				{
					this.m_IconColor = new Color32(byte.MaxValue, 128, 128, 192);
				}
				else if (this.UnitInstance.simulation.state == BattleSimulation.Squad.State.Fled)
				{
					this.m_IconColor = new Color32(128, 128, 128, 192);
				}
				this.UpdateHighlight();
			}
			if (UnityEngine.Time.frameCount % 10 == 0)
			{
				this.UpdateState(true);
			}
		}
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x000EC957 File Offset: 0x000EAB57
	private bool CanBeHealed()
	{
		return this.UnitInstance != null && this.UnitInstance.damage > 0f && this.Castle != null;
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x000EC982 File Offset: 0x000EAB82
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		this.HandleIconClicks(e);
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x000EC992 File Offset: 0x000EAB92
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		this.HandleIconClicks(e);
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x000EC9A4 File Offset: 0x000EABA4
	private void HandleIconClicks(PointerEventData e)
	{
		bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "open debug hire window", true);
		if (this.UnitInstance == null || flag || (UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl) && UICommon.GetModifierKey(UICommon.ModifierKey.Shift)))
		{
			UIUnitSlot.ClearSelection(true);
			UIArmyWindow componentInParent = base.GetComponentInParent<UIArmyWindow>();
			if (componentInParent != null)
			{
				Logic.Kingdom kingdom;
				if (componentInParent == null)
				{
					kingdom = null;
				}
				else
				{
					Logic.Army logic = componentInParent.logic;
					kingdom = ((logic != null) ? logic.GetKingdom() : null);
				}
				if (kingdom != BaseUI.LogicKingdom())
				{
					return;
				}
				componentInParent.OpenRecruitWindow(this.SlotIndex, flag);
				return;
			}
			else
			{
				UICastleGarisson componentInParent2 = base.GetComponentInParent<UICastleGarisson>();
				if (!(componentInParent2 != null))
				{
					return;
				}
				Logic.Kingdom kingdom2;
				if (componentInParent2 == null)
				{
					kingdom2 = null;
				}
				else
				{
					Castle castle = componentInParent2.Castle;
					kingdom2 = ((castle != null) ? castle.GetKingdom() : null);
				}
				if (kingdom2 != BaseUI.LogicKingdom())
				{
					return;
				}
				componentInParent2.OpenRecruitWindow(this.SlotIndex, flag);
				return;
			}
		}
		else if (this.Army != null && this.Army.IsMercenary())
		{
			Mercenary mercenary = this.Army.mercenary;
			Logic.Army army = (mercenary != null) ? mercenary.selected_buyer : null;
			if (army == null)
			{
				return;
			}
			Logic.Unit.Def unitDef = this.UnitDef;
			string def_id = (unitDef != null) ? unitDef.id : null;
			if (this.Army.mercenary.Buy(this.UnitInstance, army))
			{
				BaseUI.PlaySoundEvent(global::Defs.GetString(def_id, "hire_sound", null, ""), null);
				return;
			}
			BaseUI.PlaySoundEvent(global::Defs.GetString(def_id, "hire_failed_sound", null, ""), null);
			return;
		}
		else
		{
			if (!this.IsSelectable())
			{
				return;
			}
			if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
			{
				UIUnitSlot.TryTransfer(this.UnitInstance, false);
				return;
			}
			bool shift = UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false);
			bool dblclk = e != null && e.clickCount > 1;
			this.Select(shift, dblclk, true);
			return;
		}
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x000ECB78 File Offset: 0x000EAD78
	private void Select(bool shift, bool dblclk, bool notify_selection_changed)
	{
		if (shift)
		{
			bool flag = this.IsSelected();
			this.SetSelected(!flag, notify_selection_changed);
			return;
		}
		if (UIUnitSlot.IsTheOnlySelected(this.UnitInstance))
		{
			UIUnitSlot.ClearSelection(notify_selection_changed);
			return;
		}
		UIUnitSlot.ClearSelection(false);
		this.SetSelected(true, notify_selection_changed);
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000ECBC4 File Offset: 0x000EADC4
	private static bool TryTransfer(Logic.Unit unit, bool validate_only)
	{
		if (unit == null)
		{
			return false;
		}
		Logic.Object groupSelectionObj = UIUnitSlot.GetGroupSelectionObj(unit);
		if (groupSelectionObj == null)
		{
			return false;
		}
		Logic.Object otherGroupSelectionObj = UIUnitSlot.GetOtherGroupSelectionObj(unit);
		if (otherGroupSelectionObj == null)
		{
			return false;
		}
		Logic.Unit add_unit = null;
		if (!UIUnitSlot.IsSelected(unit))
		{
			if (UIUnitSlot.selection.Count == 0)
			{
				return UIUnitSlot.Transfer(groupSelectionObj, unit, otherGroupSelectionObj, null, validate_only);
			}
			if (validate_only)
			{
				add_unit = unit;
			}
			else
			{
				UIUnitSlot.SetSelected(unit, true, true);
			}
		}
		UIUnitSlot.lock_selection = true;
		bool flag = UIUnitSlot.TransferSelected(groupSelectionObj, otherGroupSelectionObj, validate_only, add_unit);
		UIUnitSlot.lock_selection = false;
		if (flag && !validate_only)
		{
			UIUnitSlot.OnSelectionChanged();
		}
		return flag;
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x000ECC40 File Offset: 0x000EAE40
	private static bool Transfer(Logic.Object grp1, Logic.Unit u1, Logic.Object grp2, Logic.Unit u2, bool validate_only)
	{
		if (u1 == null)
		{
			return u2 != null && UIUnitSlot.Transfer(grp2, u2, grp1, u1, validate_only);
		}
		if (u2 == null && UIUnitSlot.GetNumFreeSlots(grp2) == 0)
		{
			return false;
		}
		Logic.Army army;
		if ((army = (grp1 as Logic.Army)) != null)
		{
			Logic.Army dest;
			if ((dest = (grp2 as Logic.Army)) != null)
			{
				if (!validate_only)
				{
					army.TransferUnit(dest, u1, u2);
				}
				return true;
			}
			Castle dest2;
			if ((dest2 = (grp2 as Castle)) != null)
			{
				if (!validate_only)
				{
					if (u2 == null)
					{
						army.TransferUnit(dest2, u1, null);
					}
					else
					{
						army.SwapUnitWithGarrison(u1, u2);
					}
				}
				return true;
			}
			return false;
		}
		else
		{
			if (!(grp1 is Castle))
			{
				return false;
			}
			Logic.Army army2;
			if ((army2 = (grp2 as Logic.Army)) != null)
			{
				if (!validate_only)
				{
					if (u2 == null)
					{
						army2.MoveUnitFromGarrison(u1, true);
					}
					else
					{
						army2.SwapUnitWithGarrison(u2, u1);
					}
				}
				return true;
			}
			return false;
		}
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x000ECCF0 File Offset: 0x000EAEF0
	public static bool TransferSelected(Logic.Object grp1, Logic.Object grp2, bool validate_only, Logic.Unit add_unit = null)
	{
		if (UIUnitSlot.selection.Count == 0)
		{
			return false;
		}
		if (grp1 == null || grp2 == null)
		{
			return false;
		}
		int num = UIUnitSlot.NumSelected(grp1);
		int num2 = UIUnitSlot.NumSelected(grp2);
		if (add_unit != null)
		{
			Logic.Object groupSelectionObj = UIUnitSlot.GetGroupSelectionObj(add_unit);
			if (groupSelectionObj == grp1)
			{
				num++;
			}
			else if (groupSelectionObj == grp2)
			{
				num2++;
			}
		}
		if (num == 0 && num2 == 0)
		{
			return false;
		}
		if (num > num2)
		{
			if (UIUnitSlot.GetNumFreeSlots(grp2) < num - num2)
			{
				return false;
			}
		}
		else if (num < num2 && UIUnitSlot.GetNumFreeSlots(grp1) < num2 - num)
		{
			return false;
		}
		if (validate_only)
		{
			return true;
		}
		UIUnitSlot.tmp_lst1.Clear();
		UIUnitSlot.tmp_lst2.Clear();
		for (int i = 0; i < UIUnitSlot.selection.Count; i++)
		{
			Logic.Unit unit = UIUnitSlot.selection[i];
			Logic.Object groupSelectionObj2 = UIUnitSlot.GetGroupSelectionObj(unit);
			if (groupSelectionObj2 == grp1)
			{
				UIUnitSlot.tmp_lst1.Add(unit);
			}
			else if (groupSelectionObj2 == grp2)
			{
				UIUnitSlot.tmp_lst2.Add(unit);
			}
		}
		for (;;)
		{
			Logic.Unit unit2 = UIUnitSlot.<TransferSelected>g__Pop|116_0(UIUnitSlot.tmp_lst1);
			Logic.Unit unit3 = UIUnitSlot.<TransferSelected>g__Pop|116_0(UIUnitSlot.tmp_lst2);
			if (unit2 == null && unit3 == null)
			{
				break;
			}
			UIUnitSlot.Transfer(grp1, unit2, grp2, unit3, false);
		}
		return true;
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x000ECDFF File Offset: 0x000EAFFF
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000ECE0E File Offset: 0x000EB00E
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x0002C53B File Offset: 0x0002A73B
	public override bool AcceptsDrop()
	{
		return true;
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x000ECE20 File Offset: 0x000EB020
	public override GameObject GetDragObject()
	{
		if (this.UnitInstance == null)
		{
			return null;
		}
		Logic.Army army = this.Army;
		Logic.Kingdom kingdom;
		if ((kingdom = ((army != null) ? army.GetKingdom() : null)) == null)
		{
			Castle castle = this.Castle;
			kingdom = ((castle != null) ? castle.GetKingdom() : null);
		}
		Logic.Kingdom kingdom2 = kingdom;
		if (kingdom2 == null)
		{
			return null;
		}
		if (kingdom2 != BaseUI.LogicKingdom())
		{
			return null;
		}
		return base.gameObject;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x000ECE75 File Offset: 0x000EB075
	public override void PostProcessDragObject(GameObject obj)
	{
		GameObject gameObject = global::Common.FindChildByName(obj, "id_Button_Disband", true, true);
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = global::Common.FindChildByName(obj, "id_Button_Heal", true, true);
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.SetActive(false);
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x000ECEA8 File Offset: 0x000EB0A8
	public override string ValidateDrop(Hotspot src_hotspot, GameObject dragged_obj)
	{
		UIUnitSlot uiunitSlot;
		if ((uiunitSlot = (src_hotspot as UIUnitSlot)) == null)
		{
			return null;
		}
		if (uiunitSlot == this)
		{
			return null;
		}
		if (uiunitSlot.UnitInstance == null)
		{
			return null;
		}
		if (uiunitSlot.Army != null)
		{
			if (this.Army != null)
			{
				if (uiunitSlot.Army == this.Army)
				{
					if (this.UnitInstance == null)
					{
						return null;
					}
					if (!this.Army.CanMergeUnits(uiunitSlot.UnitInstance, this.UnitInstance))
					{
						return "swap_with_other_army";
					}
					return "merge_army_units";
				}
				else
				{
					if (uiunitSlot.Army.GetKingdom() != this.Army.GetKingdom())
					{
						return null;
					}
					if (this.UnitInstance == null)
					{
						return "move_from_army_to_army";
					}
					return "swap_with_other_army";
				}
			}
			else
			{
				if (this.Castle == null)
				{
					return null;
				}
				if (uiunitSlot.Army.castle != this.Castle)
				{
					return null;
				}
				if (this.UnitInstance == null)
				{
					return "move_to_garrison";
				}
				return "swap_with_garrison";
			}
		}
		else
		{
			if (uiunitSlot.Castle == null)
			{
				return null;
			}
			if (this.Army == null)
			{
				Castle castle = this.Castle;
				return null;
			}
			if (this.Army.castle != this.Castle)
			{
				return null;
			}
			if (this.UnitInstance == null)
			{
				return "move_garrison_to_army";
			}
			return "swap_garrison_with_army";
		}
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x000ECFCC File Offset: 0x000EB1CC
	public override bool AcceptDrop(string operation, Hotspot src_hotspot, GameObject dragged_obj)
	{
		UIUnitSlot uiunitSlot;
		if ((uiunitSlot = (src_hotspot as UIUnitSlot)) == null)
		{
			return false;
		}
		if (uiunitSlot == this)
		{
			return false;
		}
		if (uiunitSlot.UnitInstance == null)
		{
			return false;
		}
		UIUnitSlot.lock_selection = true;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(operation);
		if (num <= 1634868485U)
		{
			if (num != 346487999U)
			{
				if (num != 1221707661U)
				{
					if (num == 1634868485U)
					{
						if (operation == "swap_garrison_with_army")
						{
							Logic.Army army = this.Army;
							if (army == null)
							{
								goto IL_213;
							}
							army.SwapUnitWithGarrison(this.UnitInstance, uiunitSlot.UnitInstance);
							goto IL_213;
						}
					}
				}
				else if (operation == "swap_with_garrison")
				{
					uiunitSlot.Army.SwapUnitWithGarrison(uiunitSlot.UnitInstance, this.UnitInstance);
					goto IL_213;
				}
			}
			else if (operation == "move_from_army_to_army")
			{
				Logic.Army army2 = uiunitSlot.Army;
				if (army2 == null)
				{
					goto IL_213;
				}
				army2.TransferUnit(this.Army, uiunitSlot.UnitInstance, null);
				goto IL_213;
			}
		}
		else if (num <= 2508196110U)
		{
			if (num != 2307484716U)
			{
				if (num == 2508196110U)
				{
					if (operation == "swap_with_other_army")
					{
						Logic.Army army3 = uiunitSlot.Army;
						if (army3 == null)
						{
							goto IL_213;
						}
						army3.TransferUnit(this.Army, uiunitSlot.UnitInstance, this.UnitInstance);
						goto IL_213;
					}
				}
			}
			else if (operation == "move_garrison_to_army")
			{
				Logic.Army army4 = this.Army;
				if (army4 == null)
				{
					goto IL_213;
				}
				army4.MoveUnitFromGarrison(uiunitSlot.UnitInstance, true);
				goto IL_213;
			}
		}
		else if (num != 3025837160U)
		{
			if (num == 3959797501U)
			{
				if (operation == "merge_army_units")
				{
					Logic.Army army5 = uiunitSlot.Army;
					if (army5 == null)
					{
						goto IL_213;
					}
					army5.MergeUnits(uiunitSlot.UnitInstance, this.UnitInstance, true);
					goto IL_213;
				}
			}
		}
		else if (operation == "move_to_garrison")
		{
			Logic.Army army6 = uiunitSlot.Army;
			if (army6 == null)
			{
				goto IL_213;
			}
			army6.TransferUnit(this.Castle, uiunitSlot.UnitInstance, null);
			goto IL_213;
		}
		UIUnitSlot.lock_selection = false;
		return false;
		IL_213:
		UIUnitSlot.lock_selection = false;
		UIUnitSlot.OnSelectionChanged();
		return true;
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x000ED1F8 File Offset: 0x000EB3F8
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		bool flag = false;
		if (this.UnitInstance != null)
		{
			bool flag2 = flag;
			Logic.Army army = this.UnitInstance.army;
			flag = (flag2 | ((army != null) ? army.GetKingdom() : null) == BaseUI.LogicKingdom());
			bool flag3 = flag;
			Garrison garrison = this.UnitInstance.garrison;
			Logic.Kingdom kingdom;
			if (garrison == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.Settlement settlement = garrison.settlement;
				if (settlement == null)
				{
					kingdom = null;
				}
				else
				{
					Logic.Realm realm = settlement.GetRealm();
					kingdom = ((realm != null) ? realm.GetKingdom() : null);
				}
			}
			flag = (flag3 | kingdom == BaseUI.LogicKingdom());
			bool flag4 = flag;
			Logic.Army army2 = this.UnitInstance.army;
			flag = (flag4 & (army2 == null || !army2.IsMercenary()));
		}
		bool flag5 = this.CanHealUnit();
		if (this.m_DisbandButton != null)
		{
			this.m_DisbandButton.gameObject.SetActive(flag && this.m_ShowDisband && this.mouse_in);
		}
		if (this.m_Button_Heal != null)
		{
			this.m_Button_Heal.gameObject.SetActive(flag && flag5 && this.m_ShowHeal && this.mouse_in);
		}
		if (this.m_UnitLevel != null)
		{
			this.m_UnitLevel.overrideSprite = (this.IsSelected() ? this.m_LevelBorderSelected : this.m_LevelBorderNormal);
		}
		if (this.m_FocusHighlight != null)
		{
			this.m_FocusHighlight.SetActive(this.m_IsFocused);
		}
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x000ED349 File Offset: 0x000EB549
	public override void UpdateDragHighlight(string operation, Hotspot src_hotspot, GameObject dragged_obj)
	{
		base.UpdateDragHighlight(operation, src_hotspot, dragged_obj);
		this.ShowMergeIcon(operation == "merge_army_units");
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000ED368 File Offset: 0x000EB568
	public bool CanHealUnit()
	{
		if (this.UnitInstance == null)
		{
			return false;
		}
		if (this.Castle == null)
		{
			return false;
		}
		if (this.Army == null)
		{
			if (this.Castle.garrison == null)
			{
				return false;
			}
			int num = this.Castle.garrison.units.IndexOf(this.UnitInstance);
			return this.Castle.CanHealGarrisonUnit(num);
		}
		else
		{
			int num = this.Army.units.IndexOf(this.UnitInstance);
			if (num == -1)
			{
				return false;
			}
			Logic.Character leader = this.Army.leader;
			Action action;
			if (leader == null)
			{
				action = null;
			}
			else
			{
				Actions actions = leader.actions;
				action = ((actions != null) ? actions.Find("HealArmyUnitAction") : null);
			}
			Action action2 = action;
			if (action2 == null)
			{
				return false;
			}
			if (action2.args != null)
			{
				action2.args.Clear();
			}
			action2.AddArg(ref action2.args, num, 0);
			return action2.Validate(false) == "ok";
		}
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000ED4CC File Offset: 0x000EB6CC
	[CompilerGenerated]
	internal static bool <PopulateAttributeBreakdown>g__IsPerc|54_0(DT.Field ff)
	{
		if (ff == null)
		{
			return false;
		}
		bool result = false;
		DT.Field field = ff.FindChild("perc", null, true, true, true, '.');
		if (field != null)
		{
			result = field.Bool(null, true);
		}
		else if (ff.key.EndsWith("_perc", StringComparison.Ordinal))
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x000ED518 File Offset: 0x000EB718
	[CompilerGenerated]
	internal static bool <PopulateAttributeBreakdown>g__IsFlatAdd|54_1(DT.Field ff)
	{
		if (ff == null)
		{
			return false;
		}
		bool result = false;
		DT.Field field = ff.FindChild("add", null, true, true, true, '.');
		if (field != null)
		{
			result = field.Bool(null, true);
		}
		return result;
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000ED54C File Offset: 0x000EB74C
	[CompilerGenerated]
	internal static DT.Field <PopulateAttributeBreakdown>g__GetSourceType|54_2(DT.Field source)
	{
		if (source == null)
		{
			return null;
		}
		DT.Field field = source.BaseRoot();
		if (field == null)
		{
			return null;
		}
		if (field.Type() == "def")
		{
			DT.Field defField = global::Defs.GetDefField("UnitAttributeTooltip", field.key);
			if (defField != null)
			{
				return defField;
			}
		}
		return null;
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x000ED594 File Offset: 0x000EB794
	[CompilerGenerated]
	internal static void <PopulateAttributeBreakdown>g__AddFactor|54_3(DT.Field source, float value, DT.Field ff, ref UIUnitSlot.<>c__DisplayClass54_0 A_3)
	{
		if (value == 0f && ff != null)
		{
			return;
		}
		UIUnitSlot.AttributeFactor item = default(UIUnitSlot.AttributeFactor);
		item.source = source;
		item.source_type = UIUnitSlot.<PopulateAttributeBreakdown>g__GetSourceType|54_2(source);
		if (UIUnitSlot.<PopulateAttributeBreakdown>g__IsPerc|54_0(ff))
		{
			item.perc = value;
			item.flat_field = ff.FindChild("flat", null, true, true, true, '.');
		}
		else
		{
			item.flat = value;
			if (!UIUnitSlot.<PopulateAttributeBreakdown>g__IsFlatAdd|54_1(ff))
			{
				A_3.flat_sum += value;
			}
		}
		if (ff == null)
		{
			item.is_base = true;
			UIUnitSlot.tmp_attribute_factors.Insert(0, item);
			return;
		}
		UIUnitSlot.tmp_attribute_factors.Add(item);
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x000ED638 File Offset: 0x000EB838
	[CompilerGenerated]
	internal static void <PopulateAttributeBreakdown>g__AddStatsFactors|54_4(Stats stats, List<string> stat_names, DT.Field ff, ref UIUnitSlot.<>c__DisplayClass54_0 A_3)
	{
		if (stats == null || stat_names == null)
		{
			return;
		}
		for (int i = 0; i < stat_names.Count; i++)
		{
			string name = stat_names[i];
			UIUnitSlot.<PopulateAttributeBreakdown>g__AddStatFactors|54_5(stats.Find(name, true, false), ff, ref A_3);
		}
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x000ED678 File Offset: 0x000EB878
	[CompilerGenerated]
	internal static void <PopulateAttributeBreakdown>g__AddStatFactors|54_5(Stat stat, DT.Field ff, ref UIUnitSlot.<>c__DisplayClass54_0 A_2)
	{
		if (stat == null)
		{
			return;
		}
		List<Stat.Factor> factors = stat.GetFactors(false);
		if (factors == null)
		{
			return;
		}
		for (int i = 0; i < factors.Count; i++)
		{
			Stat.Factor factor = factors[i];
			if (factor.value != 0f)
			{
				if (factor.mod != null)
				{
					DT.Field field = factor.mod.GetNameField();
					if (field == null)
					{
						field = factor.mod.GetField();
					}
					UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field, factor.value, ff, ref A_2);
				}
				else
				{
					Stat stat2 = factor.stat;
					bool flag;
					if (stat2 == null)
					{
						flag = (null != null);
					}
					else
					{
						Stat.Def def = stat2.def;
						flag = (((def != null) ? def.field : null) != null);
					}
					if (flag)
					{
						DT.Field field2 = factor.stat.def.field.FindChild("mod_name", null, true, true, true, '.');
						if (field2 == null)
						{
							field2 = factor.stat.def.field.FindChild("name", null, true, true, true, '.');
						}
						if (field2 == null)
						{
							field2 = factor.stat.def.field;
						}
						UIUnitSlot.<PopulateAttributeBreakdown>g__AddFactor|54_3(field2, factor.value, ff, ref A_2);
					}
				}
			}
		}
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x000ED787 File Offset: 0x000EB987
	[CompilerGenerated]
	internal static Logic.Unit <TransferSelected>g__Pop|116_0(List<Logic.Unit> lst)
	{
		if (lst.Count == 0)
		{
			return null;
		}
		Logic.Unit result = lst[0];
		lst.RemoveAt(0);
		return result;
	}

	// Token: 0x04000F8D RID: 3981
	[UIFieldTarget("id_Icon")]
	private Image m_UnitIcon;

	// Token: 0x04000F8E RID: 3982
	private Color m_IconColor = Color.white;

	// Token: 0x04000F8F RID: 3983
	[UIFieldTarget("id_Button_Disband")]
	private BSGButton m_DisbandButton;

	// Token: 0x04000F90 RID: 3984
	[UIFieldTarget("id_Button_Heal")]
	private BSGButton m_Button_Heal;

	// Token: 0x04000F91 RID: 3985
	[UIFieldTarget("id_StatusBar")]
	private UIArmyHealthBar m_StatusBar;

	// Token: 0x04000F92 RID: 3986
	[UIFieldTarget("id_UnitLevel")]
	private Image m_UnitLevel;

	// Token: 0x04000F93 RID: 3987
	[UIFieldTarget("id_Icon_Add")]
	private GameObject m_IconAdd;

	// Token: 0x04000F94 RID: 3988
	[UIFieldTarget("id_Icon_Merge")]
	private GameObject m_IconMerge;

	// Token: 0x04000F95 RID: 3989
	[UIFieldTarget("id_Icon_Lock")]
	private GameObject m_IconLock;

	// Token: 0x04000F96 RID: 3990
	[UIFieldTarget("id_Mercenary")]
	private GameObject m_Mercenary;

	// Token: 0x04000F97 RID: 3991
	[UIFieldTarget("id_LevelStarsContainer")]
	private RectTransform m_LevelStarsContainer;

	// Token: 0x04000F98 RID: 3992
	[UIFieldTarget("id_StarPrototype")]
	private GameObject m_StarPrototype;

	// Token: 0x04000F99 RID: 3993
	[UIFieldTarget("id_FocusHighlight")]
	private GameObject m_FocusHighlight;

	// Token: 0x04000F9A RID: 3994
	[UIFieldTarget("tut_FirstFreeSlot")]
	private GameObject m_TutorialFirstFreeSlot;

	// Token: 0x04000FA0 RID: 4000
	private bool m_ShowDisband;

	// Token: 0x04000FA1 RID: 4001
	private bool m_ShowHeal;

	// Token: 0x04000FA2 RID: 4002
	private bool m_ShowAddIcon;

	// Token: 0x04000FA3 RID: 4003
	private bool m_IsFocused;

	// Token: 0x04000FA4 RID: 4004
	private bool m_ShowMergeIcon;

	// Token: 0x04000FA5 RID: 4005
	public Vars vars;

	// Token: 0x04000FA6 RID: 4006
	public static List<UIUnitSlot> all = new List<UIUnitSlot>();

	// Token: 0x04000FA7 RID: 4007
	public static List<Logic.Unit> selection = new List<Logic.Unit>();

	// Token: 0x04000FA8 RID: 4008
	private bool m_Initialzied;

	// Token: 0x04000FA9 RID: 4009
	private static List<UIUnitSlot.AttributeFactor> tmp_attribute_factors = new List<UIUnitSlot.AttributeFactor>();

	// Token: 0x04000FAA RID: 4010
	public static bool lock_selection = false;

	// Token: 0x04000FAB RID: 4011
	private Sprite m_LevelBorderNormal;

	// Token: 0x04000FAC RID: 4012
	private Sprite m_LevelBorderSelected;

	// Token: 0x04000FAD RID: 4013
	private List<GameObject> m_LevelStars = new List<GameObject>(4);

	// Token: 0x04000FAE RID: 4014
	public float m_LevelStarsStartAmgle = 90f;

	// Token: 0x04000FAF RID: 4015
	public float m_LevelStarsStepAnge = 17f;

	// Token: 0x04000FB0 RID: 4016
	public float m_LevelStarsCenterOffset = -1f;

	// Token: 0x04000FB1 RID: 4017
	private static List<Logic.Unit> tmp_lst1 = new List<Logic.Unit>();

	// Token: 0x04000FB2 RID: 4018
	private static List<Logic.Unit> tmp_lst2 = new List<Logic.Unit>();

	// Token: 0x020006D3 RID: 1747
	private struct AttributeFactor
	{
		// Token: 0x04003735 RID: 14133
		public DT.Field source;

		// Token: 0x04003736 RID: 14134
		public DT.Field source_type;

		// Token: 0x04003737 RID: 14135
		public DT.Field flat_field;

		// Token: 0x04003738 RID: 14136
		public float flat;

		// Token: 0x04003739 RID: 14137
		public float perc;

		// Token: 0x0400373A RID: 14138
		public bool is_base;
	}
}
