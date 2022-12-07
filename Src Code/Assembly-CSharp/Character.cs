using System;
using Logic;
using UnityEngine;

// Token: 0x02000102 RID: 258
[Serializable]
public class Character : IListener
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x000866CC File Offset: 0x000848CC
	public GameObject Obj
	{
		get
		{
			if (this.logic == null)
			{
				return null;
			}
			MapObject location = this.logic.GetLocation();
			if (location == null)
			{
				return null;
			}
			GameLogic.Behaviour behaviour = location.visuals as GameLogic.Behaviour;
			if (behaviour == null)
			{
				return null;
			}
			return behaviour.gameObject;
		}
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00086714 File Offset: 0x00084914
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Character character = logic_obj as Logic.Character;
		if (character == null)
		{
			return;
		}
		new global::Character().Init(character);
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00086737 File Offset: 0x00084937
	public static Sprite GetIcon(Logic.Character logic, float size = 64f)
	{
		return DynamicIconBuilder.Instance.GetSprite(logic, size);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00086745 File Offset: 0x00084945
	public Sprite GetIcon()
	{
		return global::Character.GetIcon(this.logic, 64f);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00086757 File Offset: 0x00084957
	public void Init(Logic.Character logic)
	{
		this.logic = logic;
		logic.visuals = this;
		this.CreateActions();
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00086770 File Offset: 0x00084970
	private void CreateActions()
	{
		if (this.logic.actions == null)
		{
			return;
		}
		for (int i = 0; i < this.logic.actions.Count; i++)
		{
			Action action = this.logic.actions[i];
			if (action.visuals == null)
			{
				ActionVisuals.Create(action);
			}
		}
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x000867C6 File Offset: 0x000849C6
	public void PlayVoiceLine(string voiceLinePath, string soundEffectPath = null)
	{
		if (string.IsNullOrEmpty(voiceLinePath))
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent(voiceLinePath, this.logic);
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x000867E8 File Offset: 0x000849E8
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2384332897U)
		{
			if (num <= 1231110159U)
			{
				if (num <= 529334177U)
				{
					if (num != 37176574U)
					{
						if (num != 102649782U)
						{
							if (num != 529334177U)
							{
								return;
							}
							if (!(message == "character_imprisoned_occupation"))
							{
								return;
							}
							if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
							{
								Vars vars = new Vars(this.logic);
								vars.Set<Logic.Realm>("realm", param as Logic.Realm);
								MessageIcon.Create("CharacterImprisonedOccupation", vars, true, null);
								return;
							}
							return;
						}
						else
						{
							if (!(message == "stopped_importing_good_production_stopped"))
							{
								return;
							}
							if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
							{
								Vars vars2 = param as Vars;
								DT.Field defField = global::Defs.GetDefField(vars2.Get<string>("goodName", null), null);
								vars2.Set<string>("produced_good", "#" + global::Defs.Localize(defField, "name", null, null, true, true));
								vars2.Set<Logic.Character>("merchant", this.logic);
								MessageIcon.Create("GoodImportEndedProductionStopped", vars2, true, null);
								return;
							}
							return;
						}
					}
					else
					{
						if (!(message == "stopped_importing_good_now_produced"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							Vars vars3 = param as Vars;
							string text = vars3.Get<string>("goodName", null);
							DT.Field defField2 = global::Defs.GetDefField(text, null);
							vars3.Set<string>("produced_good", "#" + global::Defs.Localize(defField2, "name", null, null, true, true));
							vars3.Set<Logic.Character>("merchant", this.logic);
							Logic.Kingdom kingdom = this.logic.GetKingdom();
							for (int j = 0; j < kingdom.realms.Count; j++)
							{
								Logic.Realm realm = kingdom.realms[j];
								if (realm.HasTag(text, 1))
								{
									vars3.Set<Logic.Realm>("realm", realm);
									break;
								}
							}
							MessageIcon.Create("GoodImportEndedStartedProduction", vars3, true, null);
							return;
						}
						return;
					}
				}
				else if (num <= 635439285U)
				{
					if (num != 577859235U)
					{
						if (num != 635439285U)
						{
							return;
						}
						if (!(message == "character_recalled_occupation"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							Vars vars4 = new Vars(this.logic);
							vars4.Set<Logic.Realm>("realm", param as Logic.Realm);
							MessageIcon.Create("CharacterRecalledOccupation", vars4, true, null);
							return;
						}
						return;
					}
					else if (!(message == "init_king"))
					{
						return;
					}
				}
				else if (num != 879715872U)
				{
					if (num != 1231110159U)
					{
						return;
					}
					if (!(message == "no_longer_cardinal"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						Vars vars5 = new Vars(this.logic);
						vars5.Set<object>("reason", param);
						MessageIcon.Create("ClericNoLongerCardinal", vars5, true, null);
						return;
					}
					return;
				}
				else
				{
					if (!(message == "imprisoned_in_battle"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						Vars vars6 = new Vars();
						vars6.Set<Logic.Character>("character", this.logic);
						Logic.Settlement settlement = param as Logic.Settlement;
						Logic.Battle battle = param as Logic.Battle;
						if (settlement != null)
						{
							vars6.Set<Logic.Settlement>("settlement", settlement);
						}
						if (battle != null)
						{
							vars6.Set<Logic.Battle>("battle", battle);
						}
						MessageIcon.Create("ImprisonedInBattleMessage", vars6, true, null);
						return;
					}
					return;
				}
			}
			else if (num <= 1665162939U)
			{
				if (num != 1287148698U)
				{
					if (num != 1304184561U)
					{
						if (num != 1665162939U)
						{
							return;
						}
						if (!(message == "character_specialization_query"))
						{
							return;
						}
						WorldUI worldUI = WorldUI.Get();
						if (!(worldUI != null))
						{
							return;
						}
						global::Kingdom kingdom2 = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
						Logic.Character rf = obj as Logic.Character;
						if (rf.sex != Logic.Character.Sex.Male || kingdom2 == null || kingdom2.id != rf.kingdom_id || !kingdom2.logic.royalFamily.Children.Contains(rf))
						{
							return;
						}
						string def_id = "RoyalFamilySpecializationQuery";
						Vars vars7 = new Vars(rf);
						MessageIcon messageIcon = MessageIcon.Create(def_id, vars7, true, null);
						if (messageIcon != null)
						{
							messageIcon.on_update = delegate(MessageIcon i)
							{
								if (!rf.IsPrince())
								{
									i.Dismiss(true);
									return;
								}
								if (rf.skills != null && rf.skills.Count > 0)
								{
									i.Dismiss(true);
									return;
								}
							};
							return;
						}
						return;
					}
					else
					{
						if (!(message == "demoted_merchant_rel_drop_reason"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("DemotedMerchantRelDropMessage", new Vars(this.logic), true, null);
							return;
						}
						return;
					}
				}
				else
				{
					if (!(message == "lost_puppet"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						Vars vars8 = param as Vars;
						string str = vars8.Get<string>("reason", null);
						MessageIcon.Create("PuppetLost" + str + "Message", vars8, true, null);
						return;
					}
					return;
				}
			}
			else if (num <= 1829630738U)
			{
				if (num != 1666043269U)
				{
					if (num != 1829630738U)
					{
						return;
					}
					if (!(message == "dying"))
					{
						return;
					}
					Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
					if (this.logic == null || kingdom3 == null || this.logic.kingdom_id != kingdom3.id)
					{
						return;
					}
					bool flag = this.logic.IsInCourt();
					bool flag2 = this.logic.IsKing();
					if (flag && !flag2)
					{
						string def_id2 = "DeadCourtMemberMessage";
						Vars vars9 = new Vars(this.logic);
						Logic.Army army = this.logic.GetArmy();
						if (army != null)
						{
							vars9.Set<Logic.Army>("goto_target", army);
						}
						DeadStatus deadStatus = param as DeadStatus;
						if (deadStatus != null && deadStatus.reason != null)
						{
							vars9.Set<string>("reason", deadStatus.reason);
							vars9.Set<DeadStatus>("dead_status", deadStatus);
						}
						MessageIcon.Create(def_id2, vars9, true, null);
						return;
					}
					return;
				}
				else
				{
					if (!(message == "merchant_shunned"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						Vars vars10 = new Vars();
						vars10.Set<Logic.Kingdom>("kingdom", param as Logic.Kingdom);
						vars10.Set<Logic.Character>("merchant", this.logic);
						MessageIcon.Create("MerchantShunnedMessage", vars10, true, null);
						return;
					}
					return;
				}
			}
			else if (num != 2103757264U)
			{
				if (num != 2384332897U)
				{
					return;
				}
				message == "character_age_change";
				return;
			}
			else if (!(message == "became_king"))
			{
				return;
			}
		}
		else
		{
			if (num <= 3142161587U)
			{
				if (num <= 2637838419U)
				{
					if (num != 2393473934U)
					{
						if (num != 2571960665U)
						{
							if (num != 2637838419U)
							{
								return;
							}
							if (!(message == "no_longer_cardinal_elect"))
							{
								return;
							}
							if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
							{
								Vars vars11 = new Vars(this.logic);
								vars11.Set<object>("reason", param);
								MessageIcon.Create("ClericNoLongerCardinalElect", vars11, true, null);
								return;
							}
							return;
						}
						else
						{
							if (!(message == "cleric_action_interupted_occupation"))
							{
								return;
							}
							if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
							{
								Vars vars12 = new Vars(this.logic);
								vars12.Set<Logic.Realm>("realm", param as Logic.Realm);
								MessageIcon.Create("ClericActionInteuptedOccupation", vars12, true, null);
								return;
							}
							return;
						}
					}
					else
					{
						if (!(message == "character_coming_of_age"))
						{
							return;
						}
						WorldUI worldUI2 = WorldUI.Get();
						if (!(worldUI2 != null))
						{
							return;
						}
						global::Kingdom kingdom4 = global::Kingdom.Get(worldUI2.GetCurrentKingdomId());
						Logic.Character rf = obj as Logic.Character;
						if (rf.sex == Logic.Character.Sex.Male && kingdom4 != null && kingdom4.id == rf.kingdom_id && kingdom4.logic.royalFamily.Children.Contains(rf))
						{
							string def_id3 = "RoyalFamilyComingOfAge";
							Vars vars13 = new Vars(rf);
							MessageIcon messageIcon2 = MessageIcon.Create(def_id3, vars13, true, null);
							messageIcon2.on_update = (MessageIcon.OnUpdate)Delegate.Combine(messageIcon2.on_update, new MessageIcon.OnUpdate(delegate(MessageIcon i)
							{
								if (rf.IsRebel() || !rf.IsPrince())
								{
									i.Dismiss(true);
								}
							}));
							return;
						}
						return;
					}
				}
				else if (num <= 2913063157U)
				{
					if (num != 2847741721U)
					{
						if (num != 2913063157U)
						{
							return;
						}
						if (!(message == "demoted_merchant_insufficient_commerce_reason"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("DemotedMerchantInsufficientCommerceMessage", new Vars(this.logic), true, null);
							return;
						}
						return;
					}
					else
					{
						if (!(message == "spy_action_interupted_occupation"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("SpyActionInteuptedOccupation", param as Vars, true, null);
							return;
						}
						return;
					}
				}
				else if (num != 2935363440U)
				{
					if (num != 3142161587U)
					{
						return;
					}
					if (!(message == "new_opportunity"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "character_class_change"))
					{
						return;
					}
					goto IL_374;
				}
			}
			else if (num <= 3397542847U)
			{
				if (num != 3200945746U)
				{
					if (num != 3393490757U)
					{
						if (num != 3397542847U)
						{
							return;
						}
						if (!(message == "became_cardinal_elect"))
						{
							return;
						}
						if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
						{
							MessageIcon.Create("ClericBecameCardinalElect", new Vars(this.logic), true, null);
							return;
						}
						return;
					}
					else if (!(message == "opportunity_lost"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "stopped_promoting_pagan_belief_no_penalty"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						Vars vars14 = new Vars(this.logic);
						vars14.Set<string>("belief", "Pagan." + (string)param + ".name");
						vars14.Set<Logic.Character>("character", this.logic);
						MessageIcon.Create("StoppedPromotingPaganBeliefNoPenalty", vars14, true, null);
						return;
					}
					return;
				}
			}
			else if (num <= 3490792022U)
			{
				if (num != 3462471407U)
				{
					if (num != 3490792022U)
					{
						return;
					}
					if (!(message == "fatal_attrition_damage"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id && !this.logic.IsPrisoner())
					{
						Vars vars15 = new Vars(this.logic);
						vars15.Set<Logic.Character>("character", this.logic);
						vars15.Set<object>("settlement", param);
						MessageIcon.Create("FatalAttritionDamageMessage", vars15, true, null);
						return;
					}
					return;
				}
				else
				{
					if (!(message == "puppet_became_king"))
					{
						return;
					}
					if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
					{
						DT.Field soundsDef = BaseUI.soundsDef;
						BaseUI.PlayVoiceEvent((soundsDef != null) ? soundsDef.GetString("our_puppet_became_king", null, "", true, true, true, '.') : null, null);
						return;
					}
					return;
				}
			}
			else if (num != 3744245156U)
			{
				if (num != 4127546144U)
				{
					return;
				}
				if (!(message == "stopped_promoting_pagan_belief_penalty"))
				{
					return;
				}
				if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
				{
					Vars vars16 = new Vars(this.logic);
					vars16.Set<string>("belief", "Pagan." + (string)param + ".name");
					vars16.Set<Logic.Character>("character", this.logic);
					MessageIcon.Create("StoppedPromotingPaganBeliefPenalty", vars16, true, null);
					return;
				}
				return;
			}
			else
			{
				if (!(message == "not_governing"))
				{
					return;
				}
				WorldUI worldUI3 = WorldUI.Get();
				if (!(worldUI3 != null))
				{
					return;
				}
				global::Kingdom kingdom5 = global::Kingdom.Get(worldUI3.GetCurrentKingdomId());
				Logic.Character character = obj as Logic.Character;
				if (kingdom5 != null && kingdom5.id == character.kingdom_id)
				{
					string def_id4 = "CharacterNotGoverning";
					Vars vars17 = new Vars(character);
					MessageIcon.Create(def_id4, vars17, true, null);
					return;
				}
				return;
			}
			if (this.logic != null && this.logic.kingdom_id == BaseUI.LogicKingdom().id)
			{
				MessageIcon.Create(param as Opportunity, message != "opportunity_lost", true);
				return;
			}
			return;
		}
		IL_374:
		this.CreateActions();
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00087574 File Offset: 0x00085774
	public static string GetLevelText(Logic.Character c)
	{
		if (c == null)
		{
			return "";
		}
		if (c.GetSkillsCount() != 0)
		{
			return c.GetClassLevel().ToString();
		}
		return "-";
	}

	// Token: 0x04000962 RID: 2402
	public Logic.Character logic;
}
