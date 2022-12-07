using System;
using System.Collections.Generic;
using Logic;

// Token: 0x02000172 RID: 370
public class Religions : IListener
{
	// Token: 0x060012E3 RID: 4835 RVA: 0x000C4B30 File Offset: 0x000C2D30
	public static void CreateVisuals(Object logic_obj)
	{
		Logic.Religions religions = logic_obj as Logic.Religions;
		if (religions == null)
		{
			return;
		}
		new global::Religions().Init(religions);
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x000C4B54 File Offset: 0x000C2D54
	public void Init(Logic.Religions logic)
	{
		this.logic = logic;
		logic.visuals = this;
		Religion.get_religion_mods_text = new Religion.GeReligiontModsText(global::Religions.GetReligionModsText);
		Religion.get_pope_bonuses_text = new Religion.GetCharacterBonusesText(global::Religions.GetPopeBonusesText);
		Religion.get_patriarch_bonuses_text = new Religion.GetCharacterBonusesText(global::Religions.GetPatriarchBonusesText);
		Religion.get_patriarch_candidate_bonuses_text = new Religion.GetPatriarchCandidateBonusesText(global::Religions.GetPatriarchBonusesText);
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x000C4BB4 File Offset: 0x000C2DB4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "pope_died")
		{
			Logic.Kingdom kingdom12 = BaseUI.LogicKingdom();
			if (kingdom12 != null && kingdom12.is_catholic)
			{
				BackgroundMusic.OnTrigger("PopeDiedTrigger", null);
			}
			return;
		}
		if (message == "new_pope_created")
		{
			Vars vars = param as Vars;
			vars.Set<bool>("can_drop_down", BaseUI.LogicKingdom().is_catholic);
			MessageIcon.Create("NewPopeCreated", vars, true, null);
			return;
		}
		if (message == "new_pope_chosen")
		{
			Vars vars2 = param as Vars;
			vars2.Set<bool>("can_drop_down", BaseUI.LogicKingdom().is_catholic);
			Logic.Character head = this.logic.catholic.head;
			if (((head != null) ? head.GetSpecialCourtKingdom() : null) == BaseUI.LogicKingdom())
			{
				MessageIcon.Create("OurPopeChosen", vars2, true, null);
				return;
			}
			MessageIcon.Create("NewPopeChosen", vars2, true, null);
			return;
		}
		else if (message == "orthodox_destroyed")
		{
			Vars vars3 = new Vars();
			Logic.Kingdom head_kingdom = this.logic.orthodox.head_kingdom;
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			vars3.Set<Logic.Realm>("hq_realm", this.logic.orthodox.hq_realm);
			vars3.Set<Logic.Kingdom>("head_kingdom", this.logic.orthodox.head_kingdom);
			if (head_kingdom == kingdom2)
			{
				MessageIcon.Create("WeConqueredOrthodox", vars3, true, null);
				return;
			}
			if (kingdom2.religion == this.logic.orthodox)
			{
				MessageIcon.Create("OrthodoxConquered", vars3, true, null);
			}
			return;
		}
		else
		{
			if (message == "orthodox_restored")
			{
				Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
				Vars vars4 = new Vars();
				vars4.Set<Logic.Realm>("hq_realm", this.logic.orthodox.hq_realm);
				vars4.Set<Logic.Kingdom>("head_kingdom", this.logic.orthodox.head_kingdom);
				vars4.Set<bool>("can_drop_down", kingdom3.religion == this.logic.orthodox);
				MessageIcon.Create("OrthodoxRestored", vars4, true, null);
				return;
			}
			if (message == "papacy_restored")
			{
				Vars vars5 = param as Vars;
				Logic.Character head2 = this.logic.catholic.head;
				if (((head2 != null) ? head2.GetKingdom() : null) == BaseUI.LogicKingdom())
				{
					MessageIcon.Create("PapacyRestoredOurPope", vars5, true, null);
					return;
				}
				MessageIcon.Create("PapacyRestored", vars5, true, null);
				return;
			}
			else if (message == "papacy_destroyed")
			{
				Vars vars6 = param as Vars;
				if (this.logic.catholic.hq_realm.GetKingdom() == BaseUI.LogicKingdom())
				{
					MessageIcon.Create("WeDestroyedPapacy", vars6, true, null);
					return;
				}
				MessageIcon.Create("PapacyDestroyed", vars6, true, null);
				return;
			}
			else
			{
				if (message == "papacy_lost")
				{
					if (param as Logic.Kingdom == BaseUI.LogicKingdom())
					{
						MessageIcon.Create("WeLostPapacy", null, true, null);
					}
					return;
				}
				if (message == "ask_restore_papacy")
				{
					Logic.Kingdom k = param as Logic.Kingdom;
					if (k != BaseUI.LogicKingdom())
					{
						return;
					}
					Vars vars7 = new Vars(k);
					MessageIcon messageIcon = MessageIcon.Create("RestorePapacy", vars7, true, null);
					if (messageIcon == null)
					{
						return;
					}
					messageIcon.on_button = delegate(MessageWnd wnd, string btn)
					{
						GameLogic.Get(true).religions.catholic.RestorePapacyAnswer(btn);
						if (wnd != null)
						{
							wnd.CloseAndDismiss(true);
						}
						return true;
					};
					messageIcon.validate_button = delegate(MessageWnd wnd, string btn)
					{
						if (btn != "puppet")
						{
							return "ok";
						}
						if (!k.religion.def.christian)
						{
							return "not_christian";
						}
						if (k.game.religions.catholic.ChooseNewPope(k, null) == null)
						{
							return "_no_cleric";
						}
						return "ok";
					};
					messageIcon.on_update = delegate(MessageIcon i)
					{
						if (k.game.religions.catholic.hq_realm.GetKingdom() != k)
						{
							i.Dismiss(true);
						}
					};
					return;
				}
				else if (message == "choose_patriarch")
				{
					Vars vars8 = param as Vars;
					Logic.Kingdom kingdom = vars8.Get<Logic.Kingdom>("kingdom", null);
					if (kingdom != BaseUI.LogicKingdom())
					{
						return;
					}
					MessageIcon icon;
					if (kingdom.HasEcumenicalPatriarch())
					{
						icon = MessageIcon.Create("ChooseNewEcumenicalPatriarch", vars8, true, null);
					}
					else
					{
						icon = MessageIcon.Create("ChooseNewPatriarch", vars8, true, null);
					}
					if (icon != null && kingdom != null)
					{
						icon.on_update = delegate(MessageIcon ico)
						{
							if (kingdom.patriarch_candidates == null)
							{
								icon.Dismiss(true);
							}
						};
						icon.on_button = delegate(MessageWnd wnd, string btn)
						{
							if (btn == "icon_dismissed")
							{
								kingdom.game.religions.orthodox.PatriarchChosen(kingdom, null, true);
							}
							return false;
						};
					}
					return;
				}
				else
				{
					if (message == "open_choose_patriarch_message")
					{
						WorldUI worldUI = WorldUI.Get();
						if (worldUI == null)
						{
							return;
						}
						IconsBar messageIcons = worldUI.GetMessageIcons();
						if (messageIcons == null)
						{
							return;
						}
						Logic.Kingdom kingdom4 = param as Logic.Kingdom;
						if (kingdom4 == null)
						{
							return;
						}
						string b = kingdom4.HasEcumenicalPatriarch() ? "ChooseNewEcumenicalPatriarch" : "ChooseNewPatriarch";
						for (int j = 0; j < messageIcons.icons.Count; j++)
						{
							IconsBar.IconInfo iconInfo = messageIcons.icons[j];
							if (iconInfo.pars.def_field.key == b)
							{
								MessageIcon component = iconInfo.rt.GetComponent<MessageIcon>();
								if (component != null)
								{
									component.OnClick(null);
								}
								return;
							}
						}
					}
					if (message == "new_patriarch_chosen")
					{
						Vars vars9 = param as Vars;
						vars9.Set<bool>("can_drop_down", BaseUI.LogicKingdom().is_orthodox);
						Logic.Kingdom kingdom5 = vars9.Get<Logic.Kingdom>("kingdom", null);
						if (kingdom5.HasEcumenicalPatriarch())
						{
							MessageIcon.Create("EcumenicalPatriarchChosen", vars9, true, null);
							return;
						}
						if (kingdom5 != BaseUI.LogicKingdom())
						{
							return;
						}
						MessageIcon.Create("PatriarchChosen", vars9, true, null);
						return;
					}
					else
					{
						if (message == "character_died")
						{
							Vars vars10 = param as Vars;
							Logic.Character character = vars10.obj.Get<Logic.Character>();
							Logic.Kingdom kingdom6 = character.GetKingdom();
							Logic.Kingdom kingdom7 = BaseUI.LogicKingdom();
							vars10.Set<bool>("can_drop_down", (kingdom6.is_catholic && kingdom7.is_catholic) || (kingdom6.is_orthodox && kingdom7.is_orthodox));
							string def_id = character.title + "Died";
							if (global::Defs.GetDefField(def_id, null) == null)
							{
								return;
							}
							MessageIcon.Create(def_id, vars10, true, null);
						}
						if (message == "new_crusade")
						{
							Crusade crusade = param as Crusade;
							Logic.Kingdom val = BaseUI.LogicKingdom();
							Vars vars11 = new Vars(crusade);
							vars11.Set<Logic.Character>("pope", crusade.game.religions.catholic.head);
							vars11.Set<Logic.Kingdom>("plr_kingdom", val);
							if (crusade.target == BaseUI.LogicKingdom())
							{
								MessageIcon.Create("NewCrusadeAgainstUsMessage", vars11, true, null);
							}
							else if (crusade.leader.GetKingdom() == BaseUI.LogicKingdom())
							{
								if (crusade.reason != "forced")
								{
									MessageIcon.Create("WeLeadCrusadeMessage", vars11, true, null);
								}
							}
							else
							{
								MessageIcon.Create("NewCrusadeMessage", vars11, true, null);
							}
							BackgroundMusic.OnTrigger("CrusadeTrigger", crusade.religion.def.field.key);
							return;
						}
						if (message == "crusade_ended")
						{
							Vars vars12 = param as Vars;
							Crusade crusade2 = (vars12 != null) ? vars12.Get<Crusade>("obj", null) : null;
							if (vars12 == null)
							{
								crusade2 = (param as Crusade);
								vars12 = new Vars(crusade2);
							}
							vars12.Set<Logic.Character>("pope", crusade2.game.religions.catholic.head);
							string text = "Crusade";
							string text2 = crusade2.end_reason;
							uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
							if (num <= 1786826715U)
							{
								if (num <= 801774986U)
								{
									if (num != 380521037U)
									{
										if (num != 801774986U)
										{
											goto IL_992;
										}
										if (!(text2 == "force_go_rogue"))
										{
											goto IL_992;
										}
									}
									else
									{
										if (!(text2 == "papacy_destroyed"))
										{
											goto IL_992;
										}
										text += "PapacyDestroyed";
										goto IL_999;
									}
								}
								else if (num != 1494584559U)
								{
									if (num != 1510211419U)
									{
										if (num != 1786826715U)
										{
											goto IL_992;
										}
										if (!(text2 == "new_kingdom"))
										{
											goto IL_992;
										}
										text += "FormedNewKingdom";
										goto IL_999;
									}
									else
									{
										if (!(text2 == "leader_dead"))
										{
											goto IL_992;
										}
										text += "LeaderDead";
										goto IL_999;
									}
								}
								else
								{
									if (!(text2 == "target_defeated"))
									{
										goto IL_992;
									}
									text += "TargetDefeated";
									goto IL_999;
								}
							}
							else if (num <= 2105032289U)
							{
								if (num != 1842288223U)
								{
									if (num != 2017079708U)
									{
										if (num != 2105032289U)
										{
											goto IL_992;
										}
										if (!(text2 == "religion_changed"))
										{
											goto IL_992;
										}
										text += "LeaderChangedReligion";
										goto IL_999;
									}
									else
									{
										if (!(text2 == "crusade_diverted"))
										{
											goto IL_992;
										}
										text += "CrusadeDiverted";
										goto IL_999;
									}
								}
								else
								{
									if (!(text2 == "defeated"))
									{
										goto IL_992;
									}
									text += "Defeated";
									goto IL_999;
								}
							}
							else if (num != 2213722128U)
							{
								if (num != 2651579509U)
								{
									if (num != 3501520968U)
									{
										goto IL_992;
									}
									if (!(text2 == "force_go_rogue_loyalist"))
									{
										goto IL_992;
									}
								}
								else
								{
									if (!(text2 == "kingdom_defeated"))
									{
										goto IL_992;
									}
									text += "LeaderKingdomDefeated";
									goto IL_999;
								}
							}
							else
							{
								if (!(text2 == "leader_exiled"))
								{
									goto IL_992;
								}
								text += "LeaderExiled";
								goto IL_999;
							}
							text += "ForcedToRebel";
							goto IL_999;
							IL_992:
							text = "Ended";
							IL_999:
							text2 = crusade2.end_outcome;
							if (!(text2 == "no_leader_mercenary"))
							{
								if (!(text2 == "no_leader_rogue"))
								{
									if (!(text2 == "go_home"))
									{
										if (!(text2 == "go_rogue"))
										{
											if (text2 == "go_loyalist")
											{
												text += "GoLoyalist";
											}
										}
										else
										{
											text += "GoRogue";
										}
									}
									else
									{
										text += "GoHome";
									}
								}
								else
								{
									text += "ArmyBecameRebel";
								}
							}
							else
							{
								text += "ArmyBecameMercenary";
							}
							Logic.Army val2 = vars12.Get<Logic.Army>("army", null) ?? crusade2.army;
							vars12.Set<Logic.Army>("goto_target", val2);
							if (!vars12.ContainsKey("leader_kingdom"))
							{
								vars12.Set<Logic.Kingdom>("leader_kingdom", crusade2.src_kingdom);
							}
							text += "Message";
							MessageIcon.Create(text, vars12, true, null);
							MessageIcon messageIcon2 = crusade2.message_icon as MessageIcon;
							crusade2.message_icon = null;
							if (messageIcon2 != null)
							{
								messageIcon2.Dismiss(true);
							}
						}
						if (message == "crusade_removed")
						{
							Crusade crusade3 = param as Crusade;
							if (crusade3 == null)
							{
								return;
							}
							MessageIcon messageIcon3 = crusade3.message_icon as MessageIcon;
							crusade3.message_icon = null;
							if (messageIcon3 != null)
							{
								messageIcon3.Dismiss(true);
							}
						}
						if (message == "caliphate_abandoned")
						{
							Logic.Kingdom kingdom8 = param as Logic.Kingdom;
							Logic.Kingdom kingdom9 = BaseUI.LogicKingdom();
							Vars vars13 = new Vars();
							vars13.Set<Logic.Kingdom>("kingdom", kingdom8);
							vars13.Set<bool>("can_drop_down", kingdom9 != kingdom8);
							MessageIcon.Create("CaliphateAbandoned", vars13, true, null);
						}
						if (message == "caliphate_claimed")
						{
							Logic.Kingdom kingdom10 = param as Logic.Kingdom;
							Logic.Kingdom kingdom11 = BaseUI.LogicKingdom();
							Vars vars14 = new Vars();
							vars14.Set<Logic.Kingdom>("kingdom", kingdom10);
							vars14.Set<bool>("can_drop_down", kingdom11 != kingdom10);
							MessageIcon.Create("CaliphateClaimed", vars14, true, null);
						}
						return;
					}
				}
			}
		}
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x000C5770 File Offset: 0x000C3970
	public static Logic.Character GetPope()
	{
		return GameLogic.Get(true).religions.catholic.head;
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000C5787 File Offset: 0x000C3987
	public static Logic.Character GetPatriarch()
	{
		return GameLogic.Get(true).religions.orthodox.head;
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x000C579E File Offset: 0x000C399E
	public static Crusade GetCrusade()
	{
		return GameLogic.Get(true).religions.catholic.crusade;
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x000C57B8 File Offset: 0x000C39B8
	public static string GetModText(Logic.Kingdom k, Religion.StatModifier.Def mod_def, bool no_null = false, bool dark = false)
	{
		float num = mod_def.CalcValue(k);
		if (num == 0f && !no_null)
		{
			return null;
		}
		return global::Defs.LocalizeStatModifier((mod_def.field.type == "instant") ? "InstantBonuses" : "KingdomStats", mod_def.stat_name, num, mod_def.type, k, dark, true);
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x000C5814 File Offset: 0x000C3A14
	public static string GetReligionModsText(Logic.Kingdom k, string new_line = "\n")
	{
		if (k.religion_mods == null)
		{
			return "";
		}
		string text = "";
		for (int i = 0; i < k.religion_mods.Count; i++)
		{
			Religion.StatModifier.Def def = k.religion_mods[i].def;
			if (def.field.FindChild("hidden", null, true, true, true, '.') == null && !(def.field.parent.type == "tradition"))
			{
				string modText = global::Religions.GetModText(k, def, false, false);
				if (!string.IsNullOrEmpty(modText))
				{
					if (text != "")
					{
						text += new_line;
					}
					text += modText;
				}
			}
		}
		return text;
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x000C58C4 File Offset: 0x000C3AC4
	private static string GetPopeBonusText(Logic.Kingdom k, Logic.Character pope, Religion.CharacterBonus bonus, bool include_instants, string new_line = "\n", bool dark = false)
	{
		string text = "";
		for (int i = 0; i < bonus.mods.Count; i++)
		{
			Religion.StatModifier.Def def = bonus.mods[i];
			if (!(def.field.type == "instant") || include_instants)
			{
				string modText = global::Religions.GetModText(k, def, false, dark);
				if (!string.IsNullOrEmpty(modText))
				{
					if (text != "")
					{
						text += new_line;
					}
					text += modText;
				}
			}
		}
		return text;
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x000C5948 File Offset: 0x000C3B48
	private static string GetPopeBonusesText(Logic.Kingdom k, bool include_instants, string new_line = "\n", bool dark = false)
	{
		List<Religion.CharacterBonus> pope_bonuses = k.game.religions.catholic.pope_bonuses;
		if (pope_bonuses == null || !k.HasPope() || k != BaseUI.LogicKingdom())
		{
			return null;
		}
		Logic.Character head = k.game.religions.catholic.head;
		string text = "";
		for (int i = 0; i < pope_bonuses.Count; i++)
		{
			Religion.CharacterBonus bonus = pope_bonuses[i];
			string popeBonusText = global::Religions.GetPopeBonusText(k, head, bonus, include_instants, new_line, dark);
			if (!string.IsNullOrEmpty(popeBonusText))
			{
				if (text != "")
				{
					text += new_line;
				}
				text += popeBonusText;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return text;
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x000C59F6 File Offset: 0x000C3BF6
	public static string GetPopeBonusesText(Logic.Kingdom k, string new_line = "\n")
	{
		return global::Religions.GetPopeBonusesText(k, true, new_line, false);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000C5A04 File Offset: 0x000C3C04
	private static string GetPatriarchBonusText(Logic.Kingdom k, Logic.Character patriarch, Religion.CharacterBonus bonus, bool include_instants, string new_line = "\n", bool dark = false)
	{
		string text = "";
		k.cur_patriarch_candidate = patriarch;
		for (int i = 0; i < bonus.mods.Count; i++)
		{
			Religion.StatModifier.Def def = bonus.mods[i];
			if (!(def.field.type == "instant") || include_instants)
			{
				string modText = global::Religions.GetModText(k, def, false, dark);
				if (!string.IsNullOrEmpty(modText))
				{
					if (text != "")
					{
						text += new_line;
					}
					text += modText;
				}
			}
		}
		k.cur_patriarch_candidate = null;
		return text;
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x000C5A94 File Offset: 0x000C3C94
	private static string GetPatriarchBonusesText(Logic.Kingdom k, Logic.Character patriarch, List<Religion.CharacterBonus> bonuses, bool include_instants, string new_line = "\n", bool dark = false)
	{
		if (bonuses == null)
		{
			return "";
		}
		string text = "";
		for (int i = 0; i < bonuses.Count; i++)
		{
			Religion.CharacterBonus bonus = bonuses[i];
			string patriarchBonusText = global::Religions.GetPatriarchBonusText(k, patriarch, bonus, include_instants, new_line, dark);
			if (!string.IsNullOrEmpty(patriarchBonusText))
			{
				if (text != "")
				{
					text += new_line;
				}
				text += patriarchBonusText;
			}
		}
		return text;
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x000C5AFE File Offset: 0x000C3CFE
	public static string GetPatriarchBonusesText(Logic.Kingdom k, string new_line = "\n")
	{
		return global::Religions.GetPatriarchBonusesText(k, k.patriarch, k.patriarch_bonuses, true, new_line, false);
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x000C5B15 File Offset: 0x000C3D15
	public static string GetPatriarchBonusesText(Logic.Kingdom k, Orthodox.PatriarchCandidate pc, string new_line = "\n", bool dark = false)
	{
		return global::Religions.GetPatriarchBonusesText(k, pc.cleric, pc.bonuses, true, new_line, dark);
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x000C5B2C File Offset: 0x000C3D2C
	public static string GetJihadBonusesText(Logic.Kingdom k, string new_line = "\n")
	{
		War war = (k != null) ? k.FindWarWith(k.jihad_target) : null;
		if (war == null)
		{
			return null;
		}
		Dictionary<string, War.Bonus> dictionary = (war != null) ? war.GetBonuses(k) : null;
		if (dictionary == null)
		{
			return null;
		}
		string text = "";
		foreach (KeyValuePair<string, War.Bonus> keyValuePair in dictionary)
		{
			War.Bonus value = keyValuePair.Value;
			if (value.condition.Value(k, true, true))
			{
				Vars vars = new Vars();
				vars.Set<DT.Field>("name", value.field.FindChild("name", null, true, true, true, '.'));
				vars.Set<float>("value", value.value);
				text += global::Defs.Localize("Jihad.bonus_text", vars, null, true, true);
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return null;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x000C5C28 File Offset: 0x000C3E28
	public static string GetPaganTraditionNameText(Logic.Kingdom k, Religion.PaganBelief pt)
	{
		return global::Defs.Localize("Pagan." + pt.name + ".name", k, null, true, true);
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x000C5C48 File Offset: 0x000C3E48
	public static string GetPaganTraditionBonusesText(Logic.Kingdom k, Religion.PaganBelief pt, string new_line = "\n")
	{
		string text = "";
		if (pt == null)
		{
			return text;
		}
		for (int i = 0; i < pt.mods.Count; i++)
		{
			Religion.StatModifier.Def mod_def = pt.mods[i];
			string modText = global::Religions.GetModText(k, mod_def, false, false);
			if (!string.IsNullOrEmpty(modText))
			{
				if (text != "")
				{
					text += new_line;
				}
				text += modText;
			}
		}
		return text;
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x000C5CB4 File Offset: 0x000C3EB4
	public static Value SetupHTBeliefBonuses(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		Vars vars;
		if ((vars = (((ht != null) ? ht.vars : null) as Vars)) == null)
		{
			return Value.Unknown;
		}
		Game game = GameLogic.Get(false);
		Logic.Kingdom k = arg.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		Religion.PaganBelief pt = null;
		object obj_val = vars.obj.obj_val;
		if (obj_val != null)
		{
			PaganBeliefStatus paganBeliefStatus;
			if ((paganBeliefStatus = (obj_val as PaganBeliefStatus)) == null)
			{
				string text;
				if ((text = (obj_val as string)) == null)
				{
					Religion.PaganBelief paganBelief;
					if ((paganBelief = (obj_val as Religion.PaganBelief)) == null)
					{
						DT.Field field;
						if ((field = (obj_val as DT.Field)) != null)
						{
							DT.Field field2 = field;
							Religion.PaganBelief paganBelief2;
							if (game == null)
							{
								paganBelief2 = null;
							}
							else
							{
								Logic.Religions religions = game.religions;
								if (religions == null)
								{
									paganBelief2 = null;
								}
								else
								{
									Pagan pagan = religions.pagan;
									if (pagan == null)
									{
										paganBelief2 = null;
									}
									else
									{
										Religion.Def def = pagan.def;
										paganBelief2 = ((def != null) ? def.FindPaganBelief(field2.key) : null);
									}
								}
							}
							pt = paganBelief2;
						}
					}
					else
					{
						pt = paganBelief;
					}
				}
				else
				{
					string name = text;
					Religion.PaganBelief paganBelief3;
					if (game == null)
					{
						paganBelief3 = null;
					}
					else
					{
						Logic.Religions religions2 = game.religions;
						if (religions2 == null)
						{
							paganBelief3 = null;
						}
						else
						{
							Pagan pagan2 = religions2.pagan;
							if (pagan2 == null)
							{
								paganBelief3 = null;
							}
							else
							{
								Religion.Def def2 = pagan2.def;
								paganBelief3 = ((def2 != null) ? def2.FindPaganBelief(name) : null);
							}
						}
					}
					pt = paganBelief3;
				}
			}
			else
			{
				pt = paganBeliefStatus.own_character.paganBelief;
			}
		}
		vars.Set<string>("bonuses_text", "#" + global::Religions.GetPaganTraditionBonusesText(k, pt, "\n"));
		return true;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x000C5DFC File Offset: 0x000C3FFC
	public static string GetRelgionNameKey(Logic.Kingdom k)
	{
		if (k.is_catholic)
		{
			if (!k.excommunicated)
			{
				return "Catholic.full_name.default";
			}
			return "Catholic.full_name.excommunicated";
		}
		else if (k.is_orthodox)
		{
			if (k.HasEcumenicalPatriarch())
			{
				return "Orthodox.full_name.ecumenical";
			}
			if (!k.subordinated)
			{
				return "Orthodox.full_name.independent";
			}
			return "Orthodox.full_name.subordinated";
		}
		else if (k.is_sunni)
		{
			if (!k.caliphate)
			{
				return "Sunni.full_name.default";
			}
			return "Sunni.full_name.caliphate";
		}
		else if (k.is_shia)
		{
			if (!k.caliphate)
			{
				return "Shia.full_name.default";
			}
			return "Shia.full_name.caliphate";
		}
		else
		{
			if (k.is_pagan)
			{
				return "Pagan.full_name.default";
			}
			return string.Empty;
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x000C5E9C File Offset: 0x000C409C
	public static string GetRelgionIllustrationbKey(Logic.Kingdom k)
	{
		if (k.is_catholic)
		{
			if (!k.excommunicated)
			{
				return "illustration";
			}
			return "illustration.excommunicated";
		}
		else
		{
			if (!k.is_orthodox)
			{
				return "illustration";
			}
			if (k.HasEcumenicalPatriarch())
			{
				return "illustration";
			}
			if (!k.subordinated)
			{
				return "illustration.independent";
			}
			return "illustration.subordinated";
		}
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x000C5EF4 File Offset: 0x000C40F4
	public static string GetRelgionIconKey(Logic.Kingdom k)
	{
		if (k.is_catholic)
		{
			if (!k.excommunicated)
			{
				return "icon";
			}
			return "icon.excommunicated";
		}
		else
		{
			if (!k.is_orthodox)
			{
				return "icon";
			}
			if (k.HasEcumenicalPatriarch())
			{
				return "icon";
			}
			if (!k.subordinated)
			{
				return "icon.independent";
			}
			return "icon.subordinated";
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x000C5F4C File Offset: 0x000C414C
	public static string GetRelgionIconKey(Religion.Def def, bool negative = false)
	{
		if (def == null)
		{
			return string.Empty;
		}
		string str = def.name;
		if (negative)
		{
			str += "_negative";
		}
		return str + "_icon";
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x000C5F83 File Offset: 0x000C4183
	public static string GetRelgionIconKey(string religion, bool negative = false)
	{
		if (string.IsNullOrEmpty(religion))
		{
			return string.Empty;
		}
		religion = religion.ToLowerInvariant();
		if (negative)
		{
			religion += "_negative";
		}
		return religion + "_icon";
	}

	// Token: 0x04000CAB RID: 3243
	public Logic.Religions logic;
}
