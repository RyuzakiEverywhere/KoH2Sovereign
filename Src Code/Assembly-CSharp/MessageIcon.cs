using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000233 RID: 563
public class MessageIcon : MonoBehaviour, IVars
{
	// Token: 0x06002229 RID: 8745 RVA: 0x001342C6 File Offset: 0x001324C6
	public static bool IsMessageDef(DT.Field field)
	{
		if (field == null)
		{
			return false;
		}
		if (field.Type() != "def")
		{
			return false;
		}
		while (field.based_on != null)
		{
			field = field.based_on;
		}
		return field.key == "Message";
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x00134304 File Offset: 0x00132504
	public static DT.Field GetMessageDef(Logic.Event evt)
	{
		DT.Field field = evt.GetVar("message", null, false).Get<DT.Field>();
		if (field != null)
		{
			DT.Field field2 = field.Ref(evt, true, true);
			if (field2 != null)
			{
				return field2;
			}
		}
		if (MessageIcon.IsMessageDef(evt.field))
		{
			return evt.field;
		}
		DT.Field defField = global::Defs.GetDefField(evt.id, null);
		if (defField != null)
		{
			return defField;
		}
		return null;
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x00134360 File Offset: 0x00132560
	public static void OnEvent(Logic.Event evt)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom.IsDefeated() || LoadingScreen.IsShown())
		{
			return;
		}
		evt.SetPlayer(kingdom);
		if (evt.param is Offer)
		{
			Offer offer = evt.param as Offer;
			if (evt.outcomes != null)
			{
				offer.outcomes = evt.outcomes;
				offer.outcome_vars = evt.vars;
				offer.unique_outcomes = OutcomeDef.UniqueOutcomes(offer.outcomes);
			}
			if (offer.to == kingdom)
			{
				MessageIcon.Create(offer, "message_result", true);
			}
			return;
		}
		if (evt.outcomes != null)
		{
			List<OutcomeDef> unique_outcomes = OutcomeDef.UniqueOutcomes(evt.outcomes);
			MessageIcon.Create(evt.outcomes, unique_outcomes, evt, true, evt.obj);
			return;
		}
		DT.Field messageDef = MessageIcon.GetMessageDef(evt);
		if (messageDef == null)
		{
			return;
		}
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
		Vars vars = new Vars(evt);
		MessageIcon.Create(messageDef, vars, messageIcons, MessageIcon.Type.Message, true, null);
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x00134464 File Offset: 0x00132664
	public static MessageIcon Create(string def_id, Vars vars, bool playSound = true, IVars voiceVars = null)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar messageIcons = worldUI.GetMessageIcons();
		return MessageIcon.Create(def_id, vars, messageIcons, MessageIcon.Type.Message, playSound, voiceVars);
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000023FD File Offset: 0x000005FD
	private static void OnForeignOfferAnalytics()
	{
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x00134494 File Offset: 0x00132694
	public static MessageIcon Create(Offer offer, string form, bool playSound = true)
	{
		if (offer == null)
		{
			return null;
		}
		string event_name = offer.def.field.GetString("diplomacy_event_id", null, "", true, true, true, '.');
		BaseUI baseUI = BaseUI.Get();
		AudienceWindow audienceWindow = (baseUI != null) ? baseUI.window_dispatcher.GetWindow<AudienceWindow>() : null;
		Diplomacy.TextType txt_type;
		Logic.Kingdom kingdom;
		Logic.Kingdom kingdom2;
		if (form == "consider")
		{
			kingdom = (offer.from as Logic.Kingdom);
			kingdom2 = (offer.to as Logic.Kingdom);
			txt_type = Diplomacy.TextType.Consider;
		}
		else if (form == "answered")
		{
			if (audienceWindow == null && !offer.NeedsAnswer())
			{
				return null;
			}
			kingdom = (offer.from as Logic.Kingdom);
			kingdom2 = (offer.to as Logic.Kingdom);
			if (offer.answer == "accept")
			{
				txt_type = (offer.NeedsAnswer() ? Diplomacy.TextType.Accept : Diplomacy.TextType.Comment);
			}
			else
			{
				if (!(offer.answer == "decline"))
				{
					return null;
				}
				txt_type = Diplomacy.TextType.Decline;
			}
		}
		else if (form == "proposed")
		{
			kingdom2 = (offer.from as Logic.Kingdom);
			kingdom = (offer.to as Logic.Kingdom);
			txt_type = Diplomacy.TextType.Propose;
		}
		else if (form == "message")
		{
			kingdom2 = (offer.from as Logic.Kingdom);
			kingdom = (offer.to as Logic.Kingdom);
			txt_type = Diplomacy.TextType.Propose;
		}
		else if (form == "counter_offer")
		{
			kingdom2 = (offer.from as Logic.Kingdom);
			kingdom = (offer.to as Logic.Kingdom);
			txt_type = Diplomacy.TextType.CounterOffer;
		}
		else if (form == "sweeten_offer")
		{
			kingdom2 = (offer.from as Logic.Kingdom);
			kingdom = (offer.to as Logic.Kingdom);
			txt_type = Diplomacy.TextType.SweetenOffer;
		}
		else
		{
			if (!(form == "message_result"))
			{
				return null;
			}
			kingdom2 = (offer.from as Logic.Kingdom);
			kingdom = (offer.to as Logic.Kingdom);
			if (offer.NeedsAnswer())
			{
				txt_type = Diplomacy.TextType.Comment;
			}
			else
			{
				txt_type = Diplomacy.TextType.Propose;
			}
		}
		if (kingdom2 == null && offer.from != null)
		{
			kingdom2 = offer.from.GetKingdom();
		}
		Reason reason = null;
		Reason reason4 = Diplomacy.FindRelationshipReason(txt_type, event_name, kingdom2, kingdom);
		Reason reason2 = null;
		ProsAndCons prosAndCons = ProsAndCons.Get(offer, (txt_type == Diplomacy.TextType.Propose) ? "propose" : "accept", false);
		if (prosAndCons != null)
		{
			ProsAndCons.Factor reason3 = prosAndCons.GetReason(txt_type != Diplomacy.TextType.Decline, delegate(ProsAndCons.Factor factor)
			{
				string key = factor.def.field.key;
				return Diplomacy.GetTextKey(txt_type, event_name, key, true, null) != null || (reason != null && (key == "pc_we_have_bad_relation" || key == "pc_we_have_good_relation"));
			});
			if (reason3 != null)
			{
				reason2 = new Reason(kingdom2, kingdom, reason3.def.field, (float)reason3.value, true, null, offer);
			}
		}
		List<ValueTuple<Reason, float>> list = new List<ValueTuple<Reason, float>>();
		list.Add(new ValueTuple<Reason, float>(null, offer.def.field.GetFloat("diplomacy_base_texts_weight", offer, 0f, true, true, true, '.')));
		float num = list[0].Item2;
		if (reason4 != null)
		{
			list.Add(new ValueTuple<Reason, float>(reason4, offer.def.field.GetFloat("diplomacy_rel_texts_weight", offer, 0f, true, true, true, '.')));
			num += list[list.Count - 1].Item2;
		}
		if (reason2 != null)
		{
			list.Add(new ValueTuple<Reason, float>(reason2, offer.def.field.GetFloat("diplomacy_pc_texts_weight", offer, 0f, true, true, true, '.')));
			num += list[list.Count - 1].Item2;
		}
		float num2 = offer.game.Random(0f, num);
		for (int i = 0; i < list.Count; i++)
		{
			ValueTuple<Reason, float> valueTuple = list[i];
			Reason item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			if (item2 > 0f)
			{
				if (num2 <= item2)
				{
					reason = item;
					break;
				}
				num2 -= item2;
			}
		}
		string text = "";
		if (form == "counter_offer" || form == "sweeten_offer")
		{
			Offer offer2 = offer.args[0].obj_val as Offer;
			string @string = offer2.def.field.GetString("diplomacy_event_id", null, "", true, true, true, '.');
			txt_type = ((form == "counter_offer") ? Diplomacy.TextType.ProposeWithCondition : Diplomacy.TextType.NeedsSweetening);
			text = text + Diplomacy.GetText(txt_type, @string, offer2, kingdom2, kingdom, reason) + "\n";
			if (offer.args.Count == 2)
			{
				Offer offer3 = null;
				if (offer.args.Count == 2)
				{
					offer3 = (offer.args[1].obj_val as Offer);
				}
				string text2 = (offer3 != null) ? offer3.def.field.GetString("diplomacy_event_id", null, "", true, true, true, '.') : null;
				txt_type = ((form == "counter_offer") ? Diplomacy.TextType.CounterOffer : Diplomacy.TextType.SweetenOffer);
				if (text2 != null)
				{
					text = text + "\n" + Diplomacy.GetText(txt_type, text2, offer3, kingdom2, kingdom, reason);
				}
			}
		}
		else
		{
			text += Diplomacy.GetText(txt_type, event_name, offer, kingdom2, kingdom, reason);
		}
		Vars vars = offer.outcome_vars ?? new Vars(offer);
		if (!vars.ContainsKey("kingdom"))
		{
			vars.Set<Logic.Kingdom>("kingdom", kingdom2);
		}
		if (!vars.ContainsKey("plr_kingdom"))
		{
			vars.Set<Logic.Kingdom>("plr_kingdom", kingdom);
		}
		if (!vars.ContainsKey("offer"))
		{
			vars.Set<Offer>("offer", offer);
		}
		if ((form == "proposed" || form == "message" || form == "sweeten_offer") && !vars.ContainsKey("caption"))
		{
			vars.Set<string>("caption", offer.def.field.FindChild("propose_caption", offer, true, true, true, '.').Path(false, false, '.'));
		}
		else if (form == "consider")
		{
			vars.Set<string>("caption", offer.def.field.FindChild("consider_caption", offer, true, true, true, '.').Path(false, false, '.'));
		}
		if ((offer.def.field.key.Contains("Tribute") || (form == "counter_offer" && offer.args.Count > 2)) && kingdom == offer.to)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int j = (form == "counter_offer") ? 1 : 0; j < offer.args.Count; j++)
			{
				Value value = offer.args[j];
				if (value.obj_val == null || !(value.obj_val is EmptyOffer))
				{
					flag = false;
					string text3 = global::Defs.Localize("@{align:left}{offer.arg" + j + "}", vars, null, true, true);
					if (!string.IsNullOrEmpty(text3))
					{
						stringBuilder.AppendLine();
						stringBuilder.Append("  • " + text3);
					}
				}
			}
			if (flag)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("  • " + global::Defs.Localize("@{align:left}{offer.arg0}", vars, null, true, true));
			}
			if (stringBuilder.Length > 0)
			{
				text += stringBuilder.ToString();
			}
		}
		if (form == "answered")
		{
			Logic.Object @object;
			if (audienceWindow == null)
			{
				@object = null;
			}
			else
			{
				global::Kingdom kOther = audienceWindow.kOther;
				@object = ((kOther != null) ? kOther.logic : null);
			}
			if (@object == offer.to)
			{
				if (!vars.ContainsKey("audience_text"))
				{
					vars.Set<string>("audience_text", text);
				}
				audienceWindow.OnMessage(offer, "offer_answered", vars);
				return null;
			}
		}
		if (form == "answered")
		{
			if (!vars.ContainsKey("outcome_text"))
			{
				vars.Set<string>("outcome_text", "#" + text);
			}
			return MessageIcon.Create(offer.outcomes, offer.unique_outcomes, vars, playSound, offer);
		}
		if (form == "message_result")
		{
			return MessageIcon.Create(offer.outcomes, offer.unique_outcomes, vars, playSound, offer);
		}
		if (!(form == "consider") && !(form == "proposed") && !(form == "counter_offer") && !(form == "sweeten_offer") && !(form == "message"))
		{
			return null;
		}
		if (!vars.ContainsKey("body"))
		{
			vars.Set<string>("body", "#" + text);
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		string text4;
		IconsBar iconsBar;
		MessageIcon.Type type;
		if (form == "consider")
		{
			text4 = "DiplomacyOutgoingOfferMessage";
			iconsBar = worldUI.GetIOIcons();
			type = MessageIcon.Type.PendingOffer;
		}
		else if (form == "proposed" || form == "sweeten_offer")
		{
			text4 = "DiplomacyOfferMessage";
			iconsBar = worldUI.GetIOIcons();
			type = MessageIcon.Type.PendingOffer;
		}
		else if (form == "counter_offer")
		{
			text4 = "DiplomacyCounterOfferMessage";
			iconsBar = worldUI.GetIOIcons();
			type = MessageIcon.Type.PendingOffer;
		}
		else
		{
			text4 = "DiplomacyMessage";
			iconsBar = worldUI.GetMessageIcons();
			type = MessageIcon.Type.Message;
		}
		text4 = offer.def.field.GetString("propose_message", offer, text4, true, true, true, '.');
		MessageIcon messageIcon = MessageIcon.Create(text4, vars, iconsBar, type, playSound, offer);
		if (messageIcon == null)
		{
			return null;
		}
		if (form == "proposed" || form == "counter_offer" || form == "sweeten_offer")
		{
			Logic.Object object2;
			if (audienceWindow == null)
			{
				object2 = null;
			}
			else
			{
				global::Kingdom kCurrent = audienceWindow.kCurrent;
				object2 = ((kCurrent != null) ? kCurrent.logic : null);
			}
			if (object2 == offer.to)
			{
				if (!vars.ContainsKey("audience_text"))
				{
					vars.Set<string>("audience_text", text);
				}
				if (!vars.ContainsKey("message_icon"))
				{
					vars.Set<MessageIcon>("message_icon", messageIcon);
				}
				audienceWindow.OnMessage(offer, "offer_proposed", vars);
				goto IL_A97;
			}
		}
		if (form == "proposed")
		{
			vars.Set<string>("caption", "#" + messageIcon.vars.Get<string>("_localized_caption", null));
			vars.Set<string>("body", "#" + messageIcon.vars.Get<string>("_localized_body", null));
			offer.msg_icon_left = MessageIcon.Create("DiplomacyOfferLeftMessage", vars, worldUI.GetMessageIcons(), type, false, null);
		}
		IL_A97:
		if (form == "sweeten_offer")
		{
			BSGButton component = messageIcon.GetComponent<BSGButton>();
			if (component != null)
			{
				component.onClick(component);
			}
		}
		return messageIcon;
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x00134F64 File Offset: 0x00133164
	public static MessageIcon CreateNewJihad(Logic.Kingdom ksrc, Logic.Kingdom ktgt)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return null;
		}
		War war = ksrc.FindWarWith(ktgt);
		string text;
		if (ktgt == kingdom)
		{
			text = "NewJihadAgainstUs";
		}
		else if (ksrc == kingdom)
		{
			text = "NewJihadOwn";
		}
		else if (kingdom.is_muslim)
		{
			if (kingdom.IsEnemy(ktgt))
			{
				if (war.GetLeader(kingdom) == ksrc)
				{
					text = "NewJihadMuslimAtWarCaliphateSide";
				}
				else
				{
					text = "NewJihadMuslimAtWar";
				}
			}
			else
			{
				text = "NewJihadMuslimInPeace";
			}
		}
		else if (kingdom.IsEnemy(ksrc) && war.GetLeader(kingdom) == ktgt)
		{
			text = "NewJihadNonMuslimAtWarJihadTargetSide";
		}
		else if (kingdom.IsEnemy(ktgt))
		{
			text = "NewJihadNonMuslimAtWar";
		}
		else
		{
			text = "NewJihadNonMuslimInPeace";
		}
		if (text == null)
		{
			return null;
		}
		Vars vars = new Vars(kingdom);
		vars.Set<Logic.Kingdom>("src_kingdom", ksrc);
		vars.Set<Logic.Kingdom>("tgt_kingdom", ktgt);
		if (ksrc.game.religions.jihad_kingdoms.Count > 0)
		{
			vars.Set<float>("other_jihads_rel_change", KingdomAndKingdomRelation.GetValueOfModifier(kingdom.game, "rel_new_jihad_with_other_jihad_leaders", null) * (float)(ksrc.game.religions.jihad_kingdoms.Count - 1));
		}
		return MessageIcon.Create(text, vars, true, null);
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x00135080 File Offset: 0x00133280
	public static MessageIcon CreateJihadEnded(Logic.Kingdom ksrc, Logic.Kingdom ktgt, string reason)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return null;
		}
		bool flag = false;
		string text;
		if (kingdom == ktgt)
		{
			if (reason == "caliph_dead")
			{
				text = "EndJihadAgainstUsCaliphDied";
			}
			else
			{
				text = "EndJihadAgainstUs";
			}
		}
		else if (kingdom == ksrc)
		{
			text = "OurJihadEnded";
			flag = true;
		}
		else if (kingdom.is_muslim && kingdom.IsEnemy(ktgt))
		{
			text = "EndJihadMuslimAtWar";
		}
		else
		{
			text = "EndJihad";
		}
		if (text == null)
		{
			return null;
		}
		Vars vars = new Vars(kingdom);
		vars.Set<Logic.Kingdom>("src_kingdom", ksrc);
		vars.Set<Logic.Kingdom>("tgt_kingdom", ktgt);
		if (flag)
		{
			vars.Set<string>("body", string.Concat(new string[]
			{
				"@{",
				text,
				".body_texts.",
				string.IsNullOrEmpty(reason) ? "default" : reason,
				"}"
			}));
		}
		return MessageIcon.Create(text, vars, true, null);
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x00135164 File Offset: 0x00133364
	public static MessageIcon CreateStartAlert(Logic.Kingdom k, Logic.Character target)
	{
		Logic.Kingdom k2 = BaseUI.LogicKingdom();
		Logic.Character spyFrom = k.GetSpyFrom(k2);
		if (spyFrom == null)
		{
			return null;
		}
		Vars vars = new Vars(spyFrom);
		vars.Set<Logic.Character>("target", target);
		return MessageIcon.Create("StartAlertMessage", vars, true, null);
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x001351AC File Offset: 0x001333AC
	public static MessageIcon CreateEndAlert(Logic.Kingdom k)
	{
		Logic.Kingdom k2 = BaseUI.LogicKingdom();
		Logic.Character spyFrom = k.GetSpyFrom(k2);
		if (spyFrom == null)
		{
			return null;
		}
		Vars vars = new Vars(spyFrom);
		return MessageIcon.Create("EndAlertMessage", vars, true, null);
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x001351E8 File Offset: 0x001333E8
	public static MessageIcon CreateAnotherPlayerDisconnected(Vars vars, bool playSound = true)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		return MessageIcon.Create("AnotherPlayerDisconnectedMessage", vars, ongoingIcons, MessageIcon.Type.Message, playSound, null);
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x0013521C File Offset: 0x0013341C
	public static MessageIcon CreatePlayerReconnected(Vars vars, bool playSound = true)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		return MessageIcon.Create("PlayerReconnectedMessage", vars, ongoingIcons, MessageIcon.Type.Message, playSound, null);
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x00135250 File Offset: 0x00133450
	public static MessageIcon Create(List<OutcomeDef> outcomes, List<OutcomeDef> unique_outcomes, IVars ivars, bool playSound = true, IVars voiceVars = null)
	{
		if (outcomes == null || unique_outcomes == null)
		{
			return null;
		}
		Vars vars = new Vars(ivars);
		DT.Field field = OutcomeDef.FindMessageDef(outcomes, vars);
		if (field == null)
		{
			return null;
		}
		if (vars.GetVar("outcome_caption", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"parent.force_caption",
				"message.caption",
				"default.message.caption",
				"default.caption"
			});
			string val = (field2 == null) ? null : ("#" + global::Defs.Localize(field2, vars, null, true, true));
			vars.Set<string>("outcome_caption", val);
		}
		for (int i = 0; i < unique_outcomes.Count; i++)
		{
			OutcomeDef outcomeDef = unique_outcomes[i];
			if (!(outcomeDef.field.type == "silent") && !(outcomeDef.field.type == "also_silent"))
			{
				DT.Field field2 = outcomeDef.FindValue(vars, false, new string[]
				{
					"message.list",
					"list",
					"default.message.list",
					"default.list"
				});
				if (field2 != null && field2.value != Value.Null)
				{
					vars.Set<OutcomeDef>("outcome", outcomeDef);
					global::Defs.HandleListsLocalization(field2, vars);
				}
			}
		}
		if (vars.GetVar("outcome_text", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"parent.force_text",
				"message.text",
				"message",
				"default.message.text",
				"default.text"
			});
			string text = (field2 == null) ? null : ("#" + global::Defs.Localize(field2, vars, null, true, true));
			vars.Set<string>("outcome_text", text);
			if (text == null)
			{
				string text2 = "";
				foreach (OutcomeDef outcomeDef2 in outcomes)
				{
					text2 = text2 + outcomeDef2.id + ", ";
				}
				foreach (OutcomeDef outcomeDef3 in unique_outcomes)
				{
					text2 = text2 + outcomeDef3.id + "*, ";
				}
				Debug.Log("Fishing broken outcome_text: " + text2);
			}
		}
		if (vars.GetVar("outcome_icon", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"message.icon",
				"default.message.icon",
				"default.icon"
			});
			Sprite obj = global::Defs.GetObj<Sprite>(field2, vars);
			vars.Set<Sprite>("outcome_icon", obj);
		}
		if (vars.GetVar("outcome_illustration", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"message.illustration",
				"default.message.illustration",
				"default.illustration"
			});
			Sprite randomObj = global::Defs.GetRandomObj<Sprite>(field2, vars);
			vars.Set<Sprite>("outcome_illustration", randomObj);
		}
		if (vars.GetVar("outcome_voice_line", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"message.voice_line",
				"default.message.voice_line",
				"default.voice_line"
			});
			if (field2 != null)
			{
				string text3 = field2.RandomString(vars, "");
				Logic.Event @event = ((vars != null) ? vars.obj.obj_val : null) as Logic.Event;
				Action action = ((@event != null) ? @event.param : null) as Action;
				if (text3.StartsWith("narrator_voice:", StringComparison.Ordinal) || text3.StartsWith("character_voice:", StringComparison.Ordinal))
				{
					vars.Set<string>("outcome_voice_line", text3);
				}
				else if (action != null && action.def.field.GetBool("voice_from_character", null, false, true, true, true, '.'))
				{
					Logic.Character voicingCharacter = action.GetVoicingCharacter();
					text3 = ((voicingCharacter != null) ? voicingCharacter.GetVoiceLine(text3) : null);
					if (text3 != null)
					{
						if (voiceVars is Vars)
						{
							(voiceVars as Vars).Set<Logic.Character>("character", voicingCharacter);
						}
						else
						{
							Vars vars2 = new Vars(voiceVars);
							vars2.Set<Logic.Character>("character", voicingCharacter);
							voiceVars = vars2;
						}
						vars.Set<string>("outcome_voice_line", text3);
					}
				}
			}
		}
		if (vars.GetVar("outcome_sound_effect", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"message.sound_effect",
				"default.message.sound_effect",
				"default.sound_effect"
			});
			if (field2 != null)
			{
				string val2 = field2.RandomString(vars, "");
				vars.Set<string>("outcome_sound_effect", val2);
			}
		}
		if (vars.GetVar("outcome_add_text", null, true).is_unknown)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < unique_outcomes.Count; j++)
			{
				OutcomeDef outcomeDef4 = unique_outcomes[j];
				if (!(outcomeDef4.field.type == "silent") && !(outcomeDef4.field.type == "also_silent"))
				{
					vars.Set<OutcomeDef>("outcome", outcomeDef4);
					DT.Field field2 = outcomeDef4.FindValue(vars, false, new string[]
					{
						"message.add_text",
						"add_text",
						"default.message.add_text",
						"default.add_text"
					});
					string value = null;
					if (field2 != null && field2.value != Value.Null)
					{
						value = global::Defs.Localize(field2, vars, null, false, true);
					}
					if (!string.IsNullOrEmpty(value))
					{
						if (stringBuilder.Length == 0)
						{
							stringBuilder.Append("#");
						}
						stringBuilder.Append(value);
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				vars.Set<string>("outcome_add_text", stringBuilder.ToString());
			}
		}
		if (vars.GetVar("outcome_bullets", null, true).is_unknown)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int k = 0; k < unique_outcomes.Count; k++)
			{
				OutcomeDef outcomeDef5 = unique_outcomes[k];
				if (!(outcomeDef5.field.type == "silent") && !(outcomeDef5.field.type == "also_silent"))
				{
					vars.Set<OutcomeDef>("outcome", outcomeDef5);
					DT.Field field2 = outcomeDef5.FindValue(vars, false, new string[]
					{
						"message.bullet",
						"bullet",
						"default.value",
						"default.message.bullet",
						"default.bullet"
					});
					string text4 = null;
					if (field2 != null && field2.value != Value.Null)
					{
						text4 = global::Defs.Localize(field2, vars, null, false, true);
					}
					if (!string.IsNullOrEmpty(text4))
					{
						if (stringBuilder2.Length == 0)
						{
							stringBuilder2.Append("#");
						}
						else
						{
							stringBuilder2.AppendLine();
						}
						stringBuilder2.Append("  • " + text4);
					}
				}
			}
			if (stringBuilder2.Length > 0)
			{
				vars.Set<string>("outcome_bullets", stringBuilder2.ToString());
			}
		}
		vars.Set<Value>("outcome", Value.Null);
		if (vars.GetVar("goto_target", null, true).is_unknown)
		{
			DT.Field field2 = OutcomeDef.FindValue(outcomes, vars, true, new string[]
			{
				"parent.goto_target",
				"message.goto_target",
				"default.message.goto_target",
				"default.goto_target"
			});
			MapObject val3 = (field2 == null) ? null : (field2.Value(vars, true, true).obj_val as MapObject);
			vars.Set<MapObject>("goto_target", val3);
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar messageIcons = worldUI.GetMessageIcons();
		return MessageIcon.Create(field, vars, messageIcons, MessageIcon.Type.Message, playSound, voiceVars);
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x00135A10 File Offset: 0x00133C10
	public static MessageIcon Create(global::Battle battle, bool playSound = true)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		return MessageIcon.Create(battle.OngoingMessageDefId(), battle.Vars(), ongoingIcons, MessageIcon.Type.Battle, playSound, null);
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x00135A4C File Offset: 0x00133C4C
	public static MessageIcon Create(Opportunity opportunity, bool active, bool playSound = true)
	{
		if (opportunity == null)
		{
			return null;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		string key = active ? "activate_message" : "deactivate_message";
		DT.Field @ref = opportunity.def.field.GetRef(key, opportunity, true, true, true, '.');
		if (@ref == null)
		{
			return null;
		}
		IconsBar messageIcons = worldUI.GetMessageIcons();
		MessageIcon.Type type = active ? MessageIcon.Type.Opportunity : MessageIcon.Type.Message;
		MessageIcon messageIcon = MessageIcon.Create(@ref, new Vars(opportunity), messageIcons, type, playSound, null);
		MessageWnd window = worldUI.window_dispatcher.GetWindow<MessageWnd>();
		if (!active && window != null && window.type == MessageIcon.Type.Opportunity && window.vars != null && window.vars.obj.obj_val == opportunity)
		{
			window.CloseAndDismiss(false);
			messageIcon.OnClick(null);
		}
		return messageIcon;
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x00135B18 File Offset: 0x00133D18
	public static MessageIcon Create(Castle castle, bool playSound = true)
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		Vars vars = new Vars(castle);
		vars.Set<Logic.Realm>("realm", castle.GetRealm());
		return MessageIcon.Create("CastleRepairInPorgress", vars, ongoingIcons, MessageIcon.Type.Repair, playSound, null);
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x00135B6C File Offset: 0x00133D6C
	public static MessageIcon Create(Action action, bool playSound = true)
	{
		if (action.def.prepare_message == null)
		{
			return null;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		for (int i = 0; i < ongoingIcons.icons.Count; i++)
		{
			IconsBar.IconInfo iconInfo = ongoingIcons.icons[i];
			if (action.def.prepare_message == iconInfo.pars.def_field)
			{
				iconInfo.rt.GetComponent<MessageIcon>().Dismiss(true);
			}
		}
		Vars vars = new Vars(action);
		return MessageIcon.Create(action.def.prepare_message, vars, ongoingIcons, MessageIcon.Type.Action, playSound, null);
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x00135C14 File Offset: 0x00133E14
	public static MessageIcon Create(Logic.Kingdom k, Castle castle, Building.Def building, bool playSound = true)
	{
		MessageIcon.<>c__DisplayClass40_0 CS$<>8__locals1;
		CS$<>8__locals1.building = building;
		CS$<>8__locals1.k = k;
		if (CS$<>8__locals1.building == null)
		{
			return null;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		IconsBar ongoingIcons = worldUI.GetOngoingIcons();
		Vars vars = new Vars(castle);
		vars.Set<Building.Def>("building", CS$<>8__locals1.building);
		vars.Set<Sprite>("override_icon", CS$<>8__locals1.building.field.GetValue("icon", null, true, true, true, '.').obj_val as Sprite);
		vars.Set<MapObject>("focus_object", (castle != null) ? castle : MessageIcon.<Create>g__GetUpgradeCastle|40_0(CS$<>8__locals1.k, ref CS$<>8__locals1));
		MessageIcon messageIcon = MessageIcon.Create("CastleBuildingInPorgress", vars, ongoingIcons, MessageIcon.Type.Building, playSound, null);
		if (messageIcon == null)
		{
			return null;
		}
		BuildProgressBar orAddComponent = messageIcon.GetOrAddComponent<BuildProgressBar>();
		if (orAddComponent != null)
		{
			orAddComponent.SetData(castle, CS$<>8__locals1.building);
			orAddComponent.KeepAfterComplete = true;
		}
		return messageIcon;
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x00135D06 File Offset: 0x00133F06
	public static MessageIcon Create(string def_id, Vars vars, IconsBar bar, MessageIcon.Type type = MessageIcon.Type.Message, bool playSound = true, IVars voiceVars = null)
	{
		return MessageIcon.Create(global::Defs.GetDefField(def_id, null), vars, bar, type, playSound, voiceVars);
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x00135D1C File Offset: 0x00133F1C
	public static MessageIcon Create(DT.Field def_field, Vars vars, IconsBar bar, MessageIcon.Type type = MessageIcon.Type.Message, bool playSound = true, IVars voiceVars = null)
	{
		if (bar == null)
		{
			return null;
		}
		if (def_field == null)
		{
			return null;
		}
		if (BaseUI.LogicKingdom() == null)
		{
			return null;
		}
		if (BaseUI.LogicKingdom().IsDefeated())
		{
			return null;
		}
		if (!def_field.GetBool("condition", vars, true, true, true, true, '.'))
		{
			return null;
		}
		if (vars == null)
		{
			vars = new Vars();
		}
		global::Defs.HandleListsLocalization(def_field, vars, new string[]
		{
			"list",
			"default.list"
		});
		string text = global::Defs.Localize(def_field, "caption", vars, null, false, true);
		if (text != null)
		{
			vars.Set<string>("_localized_caption", text);
		}
		string text2 = global::Defs.Localize(def_field, "radio", vars, null, false, true);
		if (text2 != null)
		{
			vars.Set<string>("_localized_radio", text2);
		}
		string text3 = global::Defs.Localize(def_field, "body", vars, null, false, true);
		if (text3 != null)
		{
			vars.Set<string>("_localized_body", text3);
		}
		bool flag = !string.IsNullOrEmpty(text2) && def_field.GetBool("log", vars, false, true, true, true, '.');
		bool flag2 = def_field.GetBool("drop_down", vars, true, true, true, true, '.');
		if (string.IsNullOrEmpty(text))
		{
			flag2 = false;
		}
		if (string.IsNullOrEmpty(text3) && type == MessageIcon.Type.Message && def_field.FindChild("hypertext", null, true, true, true, '.') == null)
		{
			flag2 = false;
		}
		if (!vars.ContainsKey("goto_target"))
		{
			Logic.Event @event = ((vars != null) ? vars.obj.obj_val : null) as Logic.Event;
			bool? flag3;
			if (@event == null)
			{
				flag3 = null;
			}
			else
			{
				Vars vars2 = @event.vars;
				flag3 = ((vars2 != null) ? new bool?(vars2.ContainsKey("goto_target")) : null);
			}
			if (flag3 ?? false)
			{
				vars.Set<Value>("goto_target", @event.vars.Get("goto_target", true));
			}
			else if (((@event != null) ? @event.obj : null) != null)
			{
				Logic.Object @object = @event.obj;
				if (@object is Logic.Realm)
				{
					@object = (@object as Logic.Realm).castle;
				}
				else if (@object is Logic.Kingdom)
				{
					Logic.Realm capital = (@object as Logic.Kingdom).GetCapital();
					@object = ((capital != null) ? capital.castle : null);
				}
				else if (@object is Logic.Character)
				{
					Logic.Character character = @object as Logic.Character;
					@object = character.CurLocation();
					if (@object == null)
					{
						Logic.Kingdom kingdom = character.GetKingdom();
						@object = ((kingdom != null) ? kingdom.GetCapital() : null);
					}
				}
				if (@object != null)
				{
					vars.Set<Logic.Object>("goto_target", @object);
				}
			}
		}
		MessageIcon messageIcon = null;
		if (flag2)
		{
			messageIcon = MessageIcon.CreateImmediate(new MessageIcon.CreateParams
			{
				def_field = def_field,
				vars = vars,
				bar = bar,
				type = type,
				playSound = playSound,
				voiceVars = voiceVars
			});
			if (messageIcon != null && messageIcon.type == MessageIcon.Type.PendingOffer)
			{
				MessageIcon.pendingIcons.Add(messageIcon);
			}
		}
		if (flag)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return null;
			}
			worldUI.GetEventLogger().AddMessage(def_field, vars);
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("radio_message", null, "", true, true, true, '.') : null, null);
		}
		return messageIcon;
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x00136030 File Offset: 0x00134230
	private static MessageIcon CreateImmediate(MessageIcon.CreateParams pars)
	{
		if (pars.bar == null || pars.def_field == null)
		{
			return null;
		}
		DT.Field field = pars.def_field.FindChild("show_filter_id", null, true, true, true, '.');
		if (field != null)
		{
			UserSettings.SettingData setting = UserSettings.GetSetting(field.String(null, ""));
			if (setting != null && setting.value == false)
			{
				return null;
			}
		}
		WorldUI x = WorldUI.Get();
		if (x == null)
		{
			return null;
		}
		GameObject gameObject = pars.bar.AddIcon(pars);
		if (gameObject == null)
		{
			return null;
		}
		MessageIcon messageIcon = gameObject.GetComponent<MessageIcon>();
		if (messageIcon == null)
		{
			messageIcon = gameObject.AddComponent<MessageIcon>();
		}
		messageIcon.type = pars.type;
		messageIcon.def_field = pars.def_field;
		messageIcon.vars = pars.vars;
		if (messageIcon.vars == null)
		{
			messageIcon.vars = new Vars();
		}
		messageIcon.ui = x;
		messageIcon.bar = pars.bar;
		messageIcon.Init();
		return messageIcon;
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x00136130 File Offset: 0x00134330
	public static MessageIcon CreateInheritance(string def_id, Logic.Kingdom inheritor, Logic.Kingdom inheritingFrom, bool playSound = true)
	{
		Inheritance inheritance = inheritingFrom.GetComponent<Inheritance>();
		Logic.Character princess = inheritance.currentPrincess;
		Vars vars = new Vars();
		vars.Set<Logic.Character>("princess", inheritance.currentPrincess);
		vars.Set<Logic.Kingdom>("inheritingFrom", inheritingFrom);
		vars.Set<List<Logic.Realm>>("realms", inheritance.realms);
		MessageIcon messageIcon = MessageIcon.Create(def_id, vars, true, null);
		messageIcon.SetExpireTime(inheritance.GetExpireTime() - GameLogic.Get(true).time);
		messageIcon.type = MessageIcon.Type.Inheritance;
		messageIcon.on_button = delegate(MessageWnd wnd, string button_id)
		{
			if (inheritance.currentKingdom != inheritor)
			{
				wnd.CloseAndDismiss(false);
				return true;
			}
			vars.Get<List<Logic.Realm>>("realms", null);
			if (button_id == "accept")
			{
				Offer offer = Offers.Find(inheritor, inheritingFrom);
				if (offer == null)
				{
					offer = Offers.Find(inheritingFrom, inheritor);
				}
				if (offer != null)
				{
					offer.Decline();
				}
				PrincessClaimInheritanceOffer princessClaimInheritanceOffer = new PrincessClaimInheritanceOffer(inheritor, inheritingFrom);
				princessClaimInheritanceOffer.SetArg(0, princess);
				princessClaimInheritanceOffer.SetArg(1, new Value(inheritance.realms));
				if (princessClaimInheritanceOffer.Validate() == "ok")
				{
					princessClaimInheritanceOffer.Send(true);
					wnd.CloseAndDismiss(true);
				}
				else
				{
					Tooltip.Get(global::Common.FindChildByNameBFS(wnd.gameObject, "accept", true, true), true).SetDef("ClaimInheritanceBtnUnavailableTooltip", null);
				}
			}
			if (button_id == "let_me_think")
			{
				wnd.Close(false);
			}
			if (button_id == "decline")
			{
				inheritance.Abstain();
				wnd.CloseAndDismiss(true);
			}
			return true;
		};
		return messageIcon;
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x00136214 File Offset: 0x00134414
	public void DismissStack()
	{
		WorldUI worldUI = this.ui;
		MessageWnd messageWnd = (worldUI != null) ? worldUI.window_dispatcher.GetWindow<MessageWnd>() : null;
		this.bar.DelStack(base.gameObject, ((messageWnd != null) ? messageWnd.icon : null) == this);
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x0013625C File Offset: 0x0013445C
	public void Dismiss(bool close_window = true)
	{
		WorldUI worldUI = this.ui;
		MessageWnd messageWnd = (worldUI != null) ? worldUI.window_dispatcher.GetWindow<MessageWnd>() : null;
		if (this.type == MessageIcon.Type.PendingOffer)
		{
			MessageIcon.pendingIcons.Remove(this);
		}
		bool open_next_in_stack = false;
		if (messageWnd != null && messageWnd.icon == this)
		{
			open_next_in_stack = !UIEndGameWindow.IsActive();
			if (close_window)
			{
				messageWnd.Close(false);
			}
			messageWnd.icon = null;
		}
		this.bar.DelIcon(base.gameObject, open_next_in_stack);
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x001362E8 File Offset: 0x001344E8
	public void UpdateCaption(bool force = false)
	{
		Vars vars = this.vars;
		string text = (vars != null) ? vars.Get<string>("_localized_caption", null) : null;
		if (text != null && !force)
		{
			UIText.SetText(this.caption, text);
			return;
		}
		UIText.SetText(this.caption, this.def_field, "caption", this.vars, null);
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x00136340 File Offset: 0x00134540
	public void UpdateIcon()
	{
		bool flag = true;
		if (this.source_kingdom_icon != null)
		{
			Logic.Kingdom kingdom = this.vars.Get<Logic.Kingdom>("src_kingdom", null);
			this.source_kingdom_icon.SetObject(kingdom, null);
			flag = (kingdom == null);
			this.source_kingdom_icon.gameObject.SetActive(kingdom != null);
		}
		if (this.image == null)
		{
			return;
		}
		Sprite sprite = null;
		if (this.vars != null)
		{
			sprite = this.vars.Get<Sprite>("override_icon", null);
		}
		if (sprite == null)
		{
			sprite = global::Defs.GetObj<Sprite>(this.def_field, "icon", this.vars);
		}
		this.image.enabled = (flag && sprite != null);
		this.image.sprite = sprite;
		if (this.secondary_image != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.def_field, "secoundary_icon", this.vars);
			this.secondary_image.sprite = obj;
			this.secondary_image.gameObject.SetActive(obj != null);
		}
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x0013644C File Offset: 0x0013464C
	private void Init()
	{
		this.caption = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_Caption");
		this.caption_container = global::Common.FindChildByName(base.gameObject, "id_CaptionContainer", true, true);
		this.UpdateCaption(false);
		this.ShowCaption(false);
		BSGButton component = base.GetComponent<BSGButton>();
		if (component != null)
		{
			component.onClick = new BSGButton.OnClick(this.OnClick);
			component.onDoubleClick = new BSGButton.OnClick(this.OnDoubleClick);
			component.onEvent = new BSGButton.OnEvent(this.OnButtonEvent);
			component.SetAudioSet("DefaultAudioSetPaper");
		}
		this.image = global::Common.FindChildComponent<Image>(base.gameObject, "id_Icon");
		this.secondary_image = global::Common.FindChildComponent<Image>(base.gameObject, "id_SecondaryIcon");
		this.source_kingdom_icon = global::Common.FindChildComponent<UIKingdomIcon>(base.gameObject, "id_SourceKingdom");
		this.warning = global::Common.FindChildComponent<Image>(base.gameObject, "id_Warning");
		this.influence_warning = global::Common.FindChildComponent<Image>(base.gameObject, "id_InfluenceWarning");
		this.UpdateIcon();
		this.InitProgress();
		this.UpdateProgress();
		this.InitTooltip();
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x0013656D File Offset: 0x0013476D
	private void ShowCaption(bool show)
	{
		TextMeshProUGUI textMeshProUGUI = this.caption;
		if (textMeshProUGUI != null)
		{
			textMeshProUGUI.gameObject.SetActive(show);
		}
		GameObject gameObject = this.caption_container;
		if (gameObject == null)
		{
			return;
		}
		gameObject.gameObject.SetActive(show);
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x0013659C File Offset: 0x0013479C
	private float SetProgress(float val, float max)
	{
		if (this.progress_bar == null)
		{
			return 0f;
		}
		if (max <= 0f)
		{
			this.progress.SetActive(false);
			return 0f;
		}
		if (val > max)
		{
			val = max;
		}
		this.progress.SetActive(true);
		float num = val / max;
		if (float.IsNaN(num))
		{
			num = 0f;
		}
		Image component = this.progress_bar.GetComponent<Image>();
		if (component != null && component.type == Image.Type.Filled)
		{
			component.fillAmount = (this.IsInvertedProgress() ? num : (1f - num));
		}
		else
		{
			Vector3 localScale = this.progress_bar.transform.localScale;
			localScale.y = (this.IsInvertedProgress() ? num : (1f - num));
			this.progress_bar.transform.localScale = localScale;
		}
		return num;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x00136670 File Offset: 0x00134870
	private bool IsInvertedProgress()
	{
		return this.type == MessageIcon.Type.Battle;
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x00136680 File Offset: 0x00134880
	private void UpdateProgressColor()
	{
		if (this.type != MessageIcon.Type.Battle)
		{
			return;
		}
		Logic.Battle battle = this.vars.Get<Logic.Battle>("battle", null);
		if (this.ui == null || battle == null || battle.settlement == null || this.ui.selectionSettings == null)
		{
			return;
		}
		Image component = this.progress_bar.GetComponent<Image>();
		if (component == null)
		{
			return;
		}
		component.color = ((this.ui.kingdom == battle.defender_kingdom.id) ? this.ui.selectionSettings.enemyColor : this.ui.selectionSettings.friendColor);
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x00136730 File Offset: 0x00134930
	private void InitProgress()
	{
		this.progress = global::Common.FindChildByName(base.gameObject, "id_Progress", true, true);
		this.progress_bar = global::Common.FindChildByName(this.progress, "id_Bar", true, true);
		if (this.progress_bar == null)
		{
			return;
		}
		this.UpdateProgressColor();
		if (this.type == MessageIcon.Type.Battle)
		{
			return;
		}
		this.init_time = GameLogic.Get(true).time;
		if (this.type == MessageIcon.Type.PendingOffer)
		{
			return;
		}
		this.expire_time = this.def_field.GetFloat("expire_time", null, 0f, true, true, true, '.');
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x001367C8 File Offset: 0x001349C8
	public void SetExpireTime(float time)
	{
		this.expire_time = time;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x001367D1 File Offset: 0x001349D1
	public float GetExpireTime()
	{
		return this.expire_time;
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x001367DC File Offset: 0x001349DC
	private float UpdateProgress()
	{
		if (this.type == MessageIcon.Type.Battle)
		{
			Logic.Battle battle = this.vars.Get<Logic.Battle>("battle", null);
			if (battle == null)
			{
				return 0f;
			}
			switch (battle.type)
			{
			case Logic.Battle.Type.Plunder:
				this.expire_time = battle.def.duration;
				return this.SetProgress(battle.PlunderProgress(), 1f);
			case Logic.Battle.Type.Siege:
			{
				float num = (float)((battle.defenders.Count == 0) ? 2 : 1);
				if (battle.siege_defense / battle.initial_siege_defense < battle.resilience / battle.initial_resilience)
				{
					return this.SetProgress((1f - battle.siege_defense / battle.initial_siege_defense) * num, 1f);
				}
				return this.SetProgress((1f - battle.resilience / battle.initial_resilience) * num, 1f);
			}
			case Logic.Battle.Type.Assault:
			case Logic.Battle.Type.BreakSiege:
			case Logic.Battle.Type.PlunderInterrupt:
				return this.SetProgress(0f, 0f);
			}
			this.init_time = battle.stage_time;
			if (battle.stage == Logic.Battle.Stage.Preparing)
			{
				this.expire_time = battle.preparation_time_cached;
			}
			else if (battle.stage == Logic.Battle.Stage.Ongoing)
			{
				this.expire_time = battle.def.duration;
			}
			else
			{
				this.expire_time = 0f;
			}
		}
		if (this.type == MessageIcon.Type.Building)
		{
			Building.Def def = this.vars.Get<Building.Def>("building", null);
			if (def == null)
			{
				return -1f;
			}
			if (def.IsUpgrade())
			{
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (kingdom == null)
				{
					return -1f;
				}
				if (kingdom.HasBuildingUpgrade(def))
				{
					return 1f;
				}
				return kingdom.GetUpgradeProgress(def);
			}
			else
			{
				Castle castle = this.vars.obj.Get<Castle>();
				if (castle == null)
				{
					return -1f;
				}
				if (castle.GetKingdom() != BaseUI.LogicKingdom())
				{
					return -1f;
				}
				if (castle.HasBuilding(def))
				{
					return 1f;
				}
				return castle.GetBuildPorgress();
			}
		}
		else if (this.type == MessageIcon.Type.Repair)
		{
			Castle castle2 = this.vars.obj.Get<Castle>();
			if (castle2 != null && castle2.GetKingdom() == BaseUI.LogicKingdom())
			{
				float num2 = this.SetProgress(castle2.initial_sack_damage - castle2.sack_damage, castle2.initial_sack_damage);
				if (num2 >= 1f || castle2.battle != null)
				{
					this.Dismiss(true);
				}
				return num2;
			}
			return -1f;
		}
		else if (this.type == MessageIcon.Type.PendingOffer)
		{
			Offer offer = this.vars.Get<Offer>("offer", null);
			if (offer == null)
			{
				return -1f;
			}
			float val;
			float max;
			offer.GetProgress(out val, out max);
			return this.SetProgress(val, max);
		}
		else if (this.type == MessageIcon.Type.Action)
		{
			Action action = this.vars.obj.obj_val as Action;
			if (action == null)
			{
				this.SetProgress(-1f, -1f);
				return -1f;
			}
			if (action.state != Action.State.Preparing)
			{
				this.SetProgress(-1f, -1f);
				return -1f;
			}
			float val2;
			float max2;
			action.GetProgress(out val2, out max2);
			return this.SetProgress(val2, max2);
		}
		else
		{
			if (this.init_time == Logic.Time.Zero || this.expire_time <= 0f)
			{
				return this.SetProgress(0f, 0f);
			}
			return this.SetProgress(GameLogic.Get(true).time - this.init_time, this.expire_time);
		}
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x00136B34 File Offset: 0x00134D34
	private void InitTooltip()
	{
		DT.Field field = this.def_field;
		DT.Field field2 = (field != null) ? field.FindChild("tooltip", null, true, true, true, '.') : null;
		Tooltip.Get(base.gameObject, true).SetDef((field2 != null) ? field2.def : null, this.vars, null);
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x00136B84 File Offset: 0x00134D84
	public void OnClick(BSGButton btn)
	{
		if (this.type == MessageIcon.Type.Battle)
		{
			Logic.Battle battle = this.vars.Get<Logic.Battle>("battle", null);
			if (battle == null)
			{
				return;
			}
			global::Battle battle2 = battle.visuals as global::Battle;
			if (battle2 == null)
			{
				return;
			}
			WorldUI worldUI = WorldUI.Get();
			if (worldUI != null)
			{
				worldUI.SelectObj(battle2.gameObject, false, true, true, true);
			}
			return;
		}
		else
		{
			if (this.type == MessageIcon.Type.Building)
			{
				MapObject mapObject = this.vars.Get<MapObject>("focus_object", null);
				if (mapObject != null)
				{
					WorldUI worldUI2 = WorldUI.Get();
					if (worldUI2 != null && mapObject.visuals is GameLogic.Behaviour)
					{
						GameObject gameObject = (mapObject.visuals as GameLogic.Behaviour).gameObject;
						if (gameObject != null)
						{
							worldUI2.SelectObj(gameObject, false, true, true, true);
						}
					}
				}
				return;
			}
			if (this.type == MessageIcon.Type.AdoptingIdea)
			{
				return;
			}
			if (this.type == MessageIcon.Type.Repair)
			{
				MapObject mapObject2 = this.vars.Get<MapObject>("obj", null);
				if (mapObject2 != null)
				{
					WorldUI worldUI3 = WorldUI.Get();
					if (worldUI3 != null && mapObject2.visuals is GameLogic.Behaviour)
					{
						GameObject gameObject2 = (mapObject2.visuals as GameLogic.Behaviour).gameObject;
						if (gameObject2 != null)
						{
							worldUI3.SelectObj(gameObject2, false, true, true, true);
						}
					}
				}
				return;
			}
			if (this.type == MessageIcon.Type.PendingOffer)
			{
				Offer offer = this.vars.Get<Offer>("offer", null);
				if (offer != null)
				{
					Logic.Kingdom kingdom = offer.from as Logic.Kingdom;
					if (((kingdom != null) ? kingdom.religion : null) != null)
					{
						BackgroundMusic.OnTrigger("DiplomacyOfferOpenTrigger", kingdom.religion.name);
					}
				}
			}
			if (this.type == MessageIcon.Type.Action)
			{
				Action action = this.vars.obj.Object(true) as Action;
				if ((action is ClaimCaliphateAction || action is ChangeReligionAction) && !UIReligionWindow.IsActive())
				{
					UIReligionWindow.ToggleOpen(BaseUI.LogicKingdom());
				}
			}
			MessageWnd messageWnd = MessageWnd.Create(this.def_field, this.vars, this, this.on_button);
			if (messageWnd == null)
			{
				return;
			}
			messageWnd.validate_button = this.validate_button;
			messageWnd.on_update = this.on_wnd_update;
			return;
		}
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x00136D98 File Offset: 0x00134F98
	private void OnDoubleClick(BSGButton btn)
	{
		this.OnDoubleClick();
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x00136DA0 File Offset: 0x00134FA0
	public void OnRightClick()
	{
		if (this.type == MessageIcon.Type.Message || this.type == MessageIcon.Type.Opportunity)
		{
			if (this.on_button != null)
			{
				this.on_button(null, "icon_dismissed");
			}
			if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
			{
				this.DismissStack();
				return;
			}
			this.Dismiss(true);
			return;
		}
		else
		{
			if (this.type == MessageIcon.Type.Building)
			{
				this.Dismiss(true);
				return;
			}
			if (this.type == MessageIcon.Type.PendingOffer)
			{
				DT.Field field = this.def_field;
				string a;
				if (field == null)
				{
					a = null;
				}
				else
				{
					DT.Field based_on = field.based_on;
					a = ((based_on != null) ? based_on.key : null);
				}
				if (a == "PendingIcon")
				{
					Analytics instance = Analytics.instance;
					if (instance != null)
					{
						instance.OnDecisionTaken(this.type, this.vars, this.def_field, this.GetExpireTime(), "dismiss");
					}
				}
				if (this.def_field.key == "DiplomacyOfferLeftMessage")
				{
					this.Dismiss(true);
					return;
				}
				Offer offer = this.vars.Get<Offer>("offer", null);
				if (offer == null)
				{
					this.Dismiss(true);
					return;
				}
				Logic.Kingdom kingdom = BaseUI.LogicKingdom();
				if (offer.from as Logic.Kingdom == kingdom)
				{
					offer.Cancel();
					return;
				}
				offer.OnExpire();
				return;
			}
			else
			{
				if (this.type == MessageIcon.Type.Action)
				{
					this.Dismiss(true);
					return;
				}
				return;
			}
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x00136EE8 File Offset: 0x001350E8
	private void OnDoubleClick()
	{
		if (this.type == MessageIcon.Type.Building)
		{
			MapObject mapObject = this.vars.Get<MapObject>("focus_object", null);
			if (mapObject != null)
			{
				WorldUI worldUI = WorldUI.Get();
				if (worldUI != null)
				{
					worldUI.LookAt(mapObject.position, false);
					if (mapObject.visuals is GameLogic.Behaviour)
					{
						GameObject gameObject = (mapObject.visuals as GameLogic.Behaviour).gameObject;
						if (gameObject != null)
						{
							worldUI.SelectObj(gameObject, false, true, true, true);
						}
					}
				}
			}
			return;
		}
		if (this.type == MessageIcon.Type.Battle || this.type == MessageIcon.Type.Message)
		{
			MapObject mapObject2 = this.vars.Get<MapObject>("obj", null);
			if (mapObject2 == null)
			{
				mapObject2 = this.vars.Get<MapObject>("goto_target", null);
			}
			if (mapObject2 == null)
			{
				return;
			}
			WorldUI worldUI2 = WorldUI.Get();
			if (worldUI2 != null)
			{
				worldUI2.LookAt(mapObject2.position, false);
			}
		}
		if (this.type == MessageIcon.Type.Message)
		{
			MapObject mapObject3 = this.vars.Get<MapObject>("focus_object", null);
			if (mapObject3 != null)
			{
				WorldUI worldUI3 = WorldUI.Get();
				if (worldUI3 != null)
				{
					worldUI3.LookAt(mapObject3.position, false);
					if (mapObject3.visuals is GameLogic.Behaviour)
					{
						GameObject gameObject2 = (mapObject3.visuals as GameLogic.Behaviour).gameObject;
						if (gameObject2 != null)
						{
							worldUI3.SelectObj(gameObject2, false, true, true, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x00137044 File Offset: 0x00135244
	private void OnButtonEvent(BSGButton btn, BSGButton.Event e, PointerEventData eventData)
	{
		if (e == BSGButton.Event.Down && eventData.button == PointerEventData.InputButton.Right)
		{
			this.OnRightClick();
		}
		if (e == BSGButton.Event.Down && eventData.button == PointerEventData.InputButton.Left && eventData.clickCount > 1)
		{
			this.OnDoubleClick();
		}
		if (e == BSGButton.Event.Enter)
		{
			this.ShowCaption(true);
		}
		if (e == BSGButton.Event.Leave)
		{
			this.ShowCaption(false);
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x00137094 File Offset: 0x00135294
	public Value GetVar(string key, IVars _vars, bool as_value)
	{
		if (this.vars != null)
		{
			Value var = this.vars.GetVar(key, _vars, as_value);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		if (key == "message_def_field" || key == "def_field")
		{
			return new Value(this.def_field);
		}
		if (key == "message_def_id" || key == "def_id")
		{
			DT.Field field = this.def_field;
			return (field != null) ? field.Path(false, false, '.') : null;
		}
		if (!(key == "message_type") && !(key == "type"))
		{
			return Value.Unknown;
		}
		return this.type.ToString();
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x00137158 File Offset: 0x00135358
	private void Update()
	{
		float num = this.UpdateProgress();
		if (this.type == MessageIcon.Type.Building)
		{
			if (num >= 1f)
			{
				if (this.m_ColorFlickCoutine == null)
				{
					this.m_ColorFlickCoutine = base.StartCoroutine(this.DelayedDismiss(this.def_field.GetFloat("expire_time", null, 0f, true, true, true, '.'), new Color?(global::Defs.GetColor(this.def_field, "expire_color", Color.green, null))));
					UIText.SetText(this.caption, this.def_field, "completed", this.vars, null);
				}
			}
			else if (num == -1f)
			{
				this.Dismiss(true);
				return;
			}
		}
		if (this.type == MessageIcon.Type.Opportunity)
		{
			Opportunity opportunity = this.vars.obj.obj_val as Opportunity;
			if (num >= 1f || opportunity == null || !opportunity.active || !Action.ShouldBeVisible(opportunity.Validate()))
			{
				this.Dismiss(true);
				return;
			}
		}
		if (this.type == MessageIcon.Type.Inheritance && this.vars.Get<Logic.Character>("princess", null) != this.vars.Get<Logic.Kingdom>("inheritingFrom", null).GetComponent<Inheritance>().currentPrincess)
		{
			this.Dismiss(true);
			return;
		}
		if (num >= 1f && (this.type == MessageIcon.Type.Message || this.type == MessageIcon.Type.AdoptingIdea))
		{
			this.Dismiss(true);
		}
		if (this.type == MessageIcon.Type.PendingOffer)
		{
			Offer offer = this.vars.Get<Offer>("offer", null);
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			string text = null;
			if (offer == null || (offer.from != kingdom && offer.to != kingdom) || (text = offer.Validate()) != "ok")
			{
				float num2 = GameLogic.Get(true).time - this.init_time;
				if (num2 < 10f)
				{
					Debug.Log("Offer invalidated within " + num2 + " seconds");
				}
				this.Dismiss(true);
				if (offer != null && offer.to == kingdom && text == "expired")
				{
					DT.Field field = this.def_field;
					string a;
					if (field == null)
					{
						a = null;
					}
					else
					{
						DT.Field based_on = field.based_on;
						a = ((based_on != null) ? based_on.key : null);
					}
					if (a == "PendingIcon")
					{
						Analytics instance = Analytics.instance;
						if (instance != null)
						{
							instance.OnDecisionTaken(this.type, this.vars, this.def_field, this.GetExpireTime(), text);
						}
					}
				}
			}
			else if (num == -1f)
			{
				offer.OnExpire();
			}
		}
		if (this.type == MessageIcon.Type.Action && num == -1f)
		{
			if (this.m_ColorFlickCoutine == null)
			{
				this.m_ColorFlickCoutine = base.StartCoroutine(this.DelayedDismiss(this.def_field.GetFloat("expire_time", null, 0f, true, true, true, '.'), new Color?(global::Defs.GetColor(this.def_field, "expire_color", Color.green, null))));
				UIText.SetText(this.caption, this.def_field, "completed", this.vars, null);
			}
			return;
		}
		if (this.warning != null)
		{
			DT.Field field2 = this.def_field.FindChild("show_warning", this.vars, true, true, true, '.');
			if (field2 != null)
			{
				bool active = field2.Bool(this.vars, false);
				this.warning.gameObject.SetActive(active);
			}
			else
			{
				this.warning.gameObject.SetActive(false);
			}
		}
		if (this.influence_warning != null)
		{
			DT.Field field3 = this.def_field.FindChild("show_influence_warning", null, true, true, true, '.');
			if (field3 != null)
			{
				bool active2 = field3.Bool(this.vars, false);
				this.influence_warning.gameObject.SetActive(active2);
			}
			else
			{
				this.influence_warning.gameObject.SetActive(false);
			}
		}
		if (this.on_update != null && this != null)
		{
			this.on_update(this);
		}
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x0013751A File Offset: 0x0013571A
	private IEnumerator DelayedDismiss(float duration, Color? color = null)
	{
		float elapsedTime = 0f;
		Image background = base.GetComponent<Image>();
		float changeDelta = 0f;
		Color normal_color = Color.white;
		Color flick_color = color ?? Color.green;
		float flick_rate = 0.25f;
		bool flick = false;
		for (;;)
		{
			elapsedTime += ((UnityEngine.Time.timeScale >= 1f) ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);
			if (elapsedTime > duration)
			{
				break;
			}
			changeDelta += UnityEngine.Time.unscaledDeltaTime;
			if (changeDelta >= flick_rate)
			{
				changeDelta = 0f;
				if (background != null)
				{
					background.color = (flick ? flick_color : normal_color);
				}
				flick = !flick;
			}
			yield return null;
		}
		this.Dismiss(true);
		yield break;
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x00137538 File Offset: 0x00135738
	public static void DeleteAll()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		IconsBar[] componentsInChildren = worldUI.gameObject.GetComponentsInChildren<IconsBar>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Clear(true);
		}
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x00137578 File Offset: 0x00135778
	public static void RecreateAll()
	{
		WorldUI x = WorldUI.Get();
		if (x == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		MessageIcon.RecreateOffers(x, kingdom);
		MessageIcon.RecreateBattles(x, kingdom);
		MessageIcon.RecreateBuildings(x, kingdom);
		MessageIcon.RecreateInheritance(x, kingdom);
		MessageIcon.RecreateChoosePatriarch(x, kingdom);
		MessageIcon.RecreateKingodmActions(x, kingdom);
		MessageIcon.RecreateDestroyKingdomNotification(x, kingdom);
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x001375D0 File Offset: 0x001357D0
	public static void RecreateOffers(WorldUI ui, Logic.Kingdom k)
	{
		MessageIcon.pendingIcons.Clear();
		Offers component = k.GetComponent<Offers>();
		if (component == null)
		{
			return;
		}
		if (component.incoming != null)
		{
			for (int i = 0; i < component.incoming.Count; i++)
			{
				Offer offer = component.incoming[i];
				string form = "proposed";
				if (offer is CounterOffer || offer is AdditionalConditionOffer)
				{
					form = "counter_offer";
				}
				else if (offer is SweetenOffer)
				{
					form = "sweeten_offer";
				}
				offer.msg_icon = MessageIcon.Create(offer, form, false);
			}
		}
		if (component.outgoing != null)
		{
			for (int j = 0; j < component.outgoing.Count; j++)
			{
				Offer offer2 = component.outgoing[j];
				offer2.msg_icon = MessageIcon.Create(offer2, "consider", false);
			}
		}
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x00137698 File Offset: 0x00135898
	public static void RecreateBattles(WorldUI ui, Logic.Kingdom k)
	{
		List<global::Battle> list = new List<global::Battle>();
		for (int i = 0; i < k.realms.Count; i++)
		{
			Logic.Realm realm = k.realms[i];
			for (int j = 0; j < realm.settlements.Count; j++)
			{
				Logic.Settlement settlement = realm.settlements[j];
				object obj;
				if (settlement == null)
				{
					obj = null;
				}
				else
				{
					Logic.Battle battle = settlement.battle;
					obj = ((battle != null) ? battle.visuals : null);
				}
				global::Battle battle2 = obj as global::Battle;
				if (battle2 != null)
				{
					list.Add(battle2);
				}
			}
		}
		for (int l = 0; l < k.armies.Count; l++)
		{
			Logic.Army army = k.armies[l];
			object obj2;
			if (army == null)
			{
				obj2 = null;
			}
			else
			{
				Logic.Battle battle3 = army.battle;
				obj2 = ((battle3 != null) ? battle3.visuals : null);
			}
			global::Battle battle4 = obj2 as global::Battle;
			if (!(battle4 == null) && !list.Contains(battle4))
			{
				list.Add(battle4);
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			MessageIcon.Create(list[m], false);
		}
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x001377A8 File Offset: 0x001359A8
	public static void RecreateBuildings(WorldUI ui, Logic.Kingdom k)
	{
		if (k.upgrading != null)
		{
			for (int i = 0; i < k.upgrading.Count; i++)
			{
				Logic.Kingdom.Upgrading upgrading = k.upgrading[i];
				MessageIcon.Create(k, null, upgrading.def, false);
			}
		}
		for (int j = 0; j < k.realms.Count; j++)
		{
			Logic.Realm realm = k.realms[j];
			Castle castle = (realm != null) ? realm.castle : null;
			if (castle != null)
			{
				Building.Def currentBuildingBuild = castle.GetCurrentBuildingBuild();
				if (currentBuildingBuild != null)
				{
					MessageIcon.Create(k, castle, currentBuildingBuild, false);
				}
				SackingRepair component = castle.GetComponent<SackingRepair>();
				if (component != null && component.running)
				{
					MessageIcon.Create(castle, false);
				}
			}
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x00137858 File Offset: 0x00135A58
	public static void RecreateRealmBuildings(WorldUI ui, Logic.Realm r)
	{
		Castle castle = (r != null) ? r.castle : null;
		if (castle == null)
		{
			return;
		}
		Building.Def currentBuildingBuild = castle.GetCurrentBuildingBuild();
		if (currentBuildingBuild != null)
		{
			MessageIcon.Create(r.GetKingdom(), castle, currentBuildingBuild, false);
		}
		SackingRepair component = castle.GetComponent<SackingRepair>();
		if (component != null && component.running)
		{
			MessageIcon.Create(castle, false);
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x001378AC File Offset: 0x00135AAC
	public static void RecreateInheritance(WorldUI ui, Logic.Kingdom k)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated() && kingdom != k && kingdom.GetComponent<Inheritance>().currentKingdom == k)
			{
				MessageIcon.CreateInheritance("ClaimIneritanceMessage", k, kingdom, true);
			}
		}
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x00137910 File Offset: 0x00135B10
	public static void RecreateChoosePatriarch(WorldUI ui, Logic.Kingdom k)
	{
		if (((k != null) ? k.patriarch_candidates : null) == null)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<Logic.Kingdom>("kingdom", k);
		k.game.religions.NotifyListeners("choose_patriarch", vars);
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x00137954 File Offset: 0x00135B54
	public static void RecreateKingodmActions(WorldUI ui, Logic.Kingdom k)
	{
		if (k == null)
		{
			return;
		}
		if (k.actions == null || k.actions.Count == 0)
		{
			return;
		}
		if (GameLogic.Get(false) == null)
		{
			return;
		}
		for (int i = 0; i < k.actions.Count; i++)
		{
			Action action = k.actions[i];
			if (action != null && action.state == Action.State.Preparing)
			{
				MessageIcon.Create(action, true);
			}
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x001379BC File Offset: 0x00135BBC
	public static void RecreateDestroyKingdomNotification(WorldUI ui, Logic.Kingdom k)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (game.rules.targetKingdom == null)
		{
			return;
		}
		if (game.rules.main_goal != "DestroyKingdom")
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<Logic.Kingdom>("target_kingdom", game.rules.targetKingdom);
		MessageIcon.Create("GameModeDestroyKingdomTargetMessage", vars, true, null);
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x00137A24 File Offset: 0x00135C24
	private void OnDisable()
	{
		this.m_ColorFlickCoutine = null;
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x00137A4C File Offset: 0x00135C4C
	[CompilerGenerated]
	internal static Castle <Create>g__GetUpgradeCastle|40_0(Logic.Kingdom kingdom, ref MessageIcon.<>c__DisplayClass40_0 A_1)
	{
		if (kingdom == null)
		{
			return null;
		}
		Building.Def firstUpgradeOf = A_1.building.GetFirstUpgradeOf();
		if (firstUpgradeOf == null)
		{
			return null;
		}
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			Logic.Realm realm = A_1.k.realms[i];
			if (realm.HasTag(firstUpgradeOf.id, 1))
			{
				return realm.castle;
			}
		}
		return null;
	}

	// Token: 0x040016F6 RID: 5878
	[HideInInspector]
	public DT.Field def_field;

	// Token: 0x040016F7 RID: 5879
	[HideInInspector]
	public Vars vars;

	// Token: 0x040016F8 RID: 5880
	public MessageIcon.Type type;

	// Token: 0x040016F9 RID: 5881
	private WorldUI ui;

	// Token: 0x040016FA RID: 5882
	private IconsBar bar;

	// Token: 0x040016FB RID: 5883
	private TextMeshProUGUI caption;

	// Token: 0x040016FC RID: 5884
	private GameObject caption_container;

	// Token: 0x040016FD RID: 5885
	private Image image;

	// Token: 0x040016FE RID: 5886
	private Image secondary_image;

	// Token: 0x040016FF RID: 5887
	private Image warning;

	// Token: 0x04001700 RID: 5888
	private Image influence_warning;

	// Token: 0x04001701 RID: 5889
	private UIKingdomIcon source_kingdom_icon;

	// Token: 0x04001702 RID: 5890
	private GameObject progress;

	// Token: 0x04001703 RID: 5891
	private GameObject progress_bar;

	// Token: 0x04001704 RID: 5892
	private Logic.Time init_time = Logic.Time.Zero;

	// Token: 0x04001705 RID: 5893
	private float expire_time;

	// Token: 0x04001706 RID: 5894
	public MessageWnd.OnButton on_button;

	// Token: 0x04001707 RID: 5895
	public MessageWnd.ValidateButton validate_button;

	// Token: 0x04001708 RID: 5896
	public MessageWnd.OnUpdate on_wnd_update;

	// Token: 0x04001709 RID: 5897
	public MessageIcon.OnUpdate on_update;

	// Token: 0x0400170A RID: 5898
	public static List<MessageIcon> pendingIcons = new List<MessageIcon>();

	// Token: 0x0400170B RID: 5899
	private UnityEngine.Coroutine m_ColorFlickCoutine;

	// Token: 0x02000787 RID: 1927
	public enum Type
	{
		// Token: 0x04003AF7 RID: 15095
		Message,
		// Token: 0x04003AF8 RID: 15096
		Battle,
		// Token: 0x04003AF9 RID: 15097
		Opportunity,
		// Token: 0x04003AFA RID: 15098
		Building,
		// Token: 0x04003AFB RID: 15099
		AdoptingIdea,
		// Token: 0x04003AFC RID: 15100
		PendingOffer,
		// Token: 0x04003AFD RID: 15101
		Crusade,
		// Token: 0x04003AFE RID: 15102
		Repair,
		// Token: 0x04003AFF RID: 15103
		Inheritance,
		// Token: 0x04003B00 RID: 15104
		Action
	}

	// Token: 0x02000788 RID: 1928
	// (Invoke) Token: 0x06004C7F RID: 19583
	public delegate void OnUpdate(MessageIcon icon);

	// Token: 0x02000789 RID: 1929
	public struct CreateParams
	{
		// Token: 0x04003B01 RID: 15105
		public DT.Field def_field;

		// Token: 0x04003B02 RID: 15106
		public Vars vars;

		// Token: 0x04003B03 RID: 15107
		public IconsBar bar;

		// Token: 0x04003B04 RID: 15108
		public MessageIcon.Type type;

		// Token: 0x04003B05 RID: 15109
		public bool playSound;

		// Token: 0x04003B06 RID: 15110
		public IVars voiceVars;
	}
}
