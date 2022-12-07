using System;
using Logic;
using UnityEngine;

// Token: 0x0200010C RID: 268
public static class Diplomacy
{
	// Token: 0x06000C50 RID: 3152 RVA: 0x0008A484 File Offset: 0x00088684
	private static string GetTextKey(string prefix, string attitude)
	{
		if (!string.IsNullOrEmpty(attitude))
		{
			string text = prefix + "." + attitude;
			if (global::Defs.FindTextField(text) != null)
			{
				return text;
			}
		}
		if (attitude != "Neutral")
		{
			string text = prefix + ".Neutral";
			if (global::Defs.FindTextField(text) != null)
			{
				return text;
			}
		}
		if (global::Defs.FindTextField(prefix) != null)
		{
			return prefix;
		}
		return null;
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0008A4E0 File Offset: 0x000886E0
	public static Logic.Kingdom GetSource(Stat.Modifier mod)
	{
		FadingModifier fadingModifier = mod as FadingModifier;
		if (fadingModifier != null)
		{
			return fadingModifier.source as Logic.Kingdom;
		}
		IndirectFadableModifier indirectFadableModifier = mod as IndirectFadableModifier;
		if (indirectFadableModifier != null)
		{
			return indirectFadableModifier.mod.source as Logic.Kingdom;
		}
		return null;
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0008A520 File Offset: 0x00088720
	public static string GetTextKey(Diplomacy.TextType type, string event_name, string reason, bool direct, string attitude)
	{
		string text = "Diplomacy.";
		if (!string.IsNullOrEmpty(reason))
		{
			text = text + reason + "." + (direct ? "Direct." : "Indirect.");
		}
		text += type.ToString();
		string textKey;
		if (!string.IsNullOrEmpty(event_name))
		{
			textKey = Diplomacy.GetTextKey(text + "." + event_name, attitude);
			if (textKey != null)
			{
				return textKey;
			}
		}
		textKey = Diplomacy.GetTextKey(text, attitude);
		if (textKey != null)
		{
			return textKey;
		}
		if (reason != null && reason.StartsWith("rel_break_", StringComparison.Ordinal))
		{
			return Diplomacy.GetTextKey(type, event_name, "rel_declare_war", direct, attitude);
		}
		return null;
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0008A5BC File Offset: 0x000887BC
	public static string DecideAttitude(Diplomacy.TextType text_type, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom)
	{
		if (kingdom == null || plr_kingdom == null)
		{
			return "Neutral";
		}
		float num = kingdom.CalcArmyStrength();
		float num2 = plr_kingdom.CalcArmyStrength();
		if (num > num2 * 1.5f)
		{
			return "Strong";
		}
		if (num2 > num * 1.5f)
		{
			return "Weak";
		}
		return "Neutral";
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x0008A608 File Offset: 0x00088808
	public static string GetTextKey(Diplomacy.TextType type, string event_name, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom, Reason reason)
	{
		string reason2 = null;
		bool direct = false;
		string attitude = Diplomacy.DecideAttitude(type, kingdom, plr_kingdom);
		if (reason != null)
		{
			direct = reason.isDirect;
			reason2 = reason.field.key;
		}
		string textKey = Diplomacy.GetTextKey(type, event_name, reason2, direct, attitude);
		if (textKey == null)
		{
			return null;
		}
		return textKey;
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x0008A64C File Offset: 0x0008884C
	public static Vars FillVars(Diplomacy.TextType type, string event_name, Offer offer, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom, Reason reason)
	{
		Vars vars = new Vars(reason);
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		vars.Set<Logic.Kingdom>("plr_kingdom", plr_kingdom);
		vars.Set<Logic.Object>("target", (reason == null) ? kingdom : (reason.isDirect ? reason.target : reason.indirectTarget));
		vars.Set<Offer>("offer", offer);
		return vars;
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x0008A6B8 File Offset: 0x000888B8
	public static string GetText(Diplomacy.TextType type, string event_name, Offer offer, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom, Reason reason = null)
	{
		string textKey = Diplomacy.GetTextKey(type, event_name, kingdom, plr_kingdom, reason);
		if (textKey == null)
		{
			return null;
		}
		Vars vars = Diplomacy.FillVars(type, event_name, offer, kingdom, plr_kingdom, reason);
		return global::Defs.Localize(textKey, vars, null, false, true);
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x0008A6EF File Offset: 0x000888EF
	public static string GetText(Diplomacy.TextType type, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom, Reason reason = null)
	{
		return Diplomacy.GetText(type, null, null, kingdom, plr_kingdom, reason);
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0008A6FC File Offset: 0x000888FC
	public static Reason FindRelationshipReason(Diplomacy.TextType type, string event_name, Logic.Kingdom kingdom, Logic.Kingdom plr_kingdom)
	{
		if (string.IsNullOrEmpty(event_name) || kingdom == null || plr_kingdom == null)
		{
			return null;
		}
		bool flag = true;
		bool flag2 = false;
		if (type == Diplomacy.TextType.Greeting || type == Diplomacy.TextType.Prompt || type == Diplomacy.TextType.Comment)
		{
			flag2 = true;
		}
		else if (type == Diplomacy.TextType.Pleased || type == Diplomacy.TextType.Accept || (type == Diplomacy.TextType.Propose && event_name != "War" && event_name != "DeclareIndependence" && event_name != "BreakMarriage" && !event_name.Contains("Demand")))
		{
			flag = false;
			flag2 = true;
		}
		Reason result = null;
		float num = 0f;
		for (int i = 0; i < plr_kingdom.diplomacyReasons.Count; i++)
		{
			Reason reason = plr_kingdom.diplomacyReasons[i];
			if (reason.IsAffectedBy(kingdom))
			{
				float num2 = reason.value;
				Logic.Kingdom kingdom2;
				if (num2 != 0f && (num2 >= 0f || flag) && (num2 <= 0f || flag2) && reason.source == plr_kingdom && ((kingdom2 = (reason.indirectTarget as Logic.Kingdom)) == null || !kingdom2.IsDefeated()) && Diplomacy.GetTextKey(type, event_name, kingdom, plr_kingdom, reason) != null)
				{
					if (num2 < 0f)
					{
						num2 = -num2;
					}
					if (num2 >= num)
					{
						result = reason;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x0008A828 File Offset: 0x00088A28
	public static void Test()
	{
		DT.Field defField = global::Defs.GetDefField("TestDiplomacy", null);
		if (defField == null)
		{
			Debug.LogError("TestDiplomacy not found");
			return;
		}
		string @string = defField.GetString("TextType", null, "", true, true, true, '.');
		Diplomacy.TextType type;
		try
		{
			type = (Diplomacy.TextType)Enum.Parse(typeof(Diplomacy.TextType), @string);
		}
		catch
		{
			Debug.LogError("Invalid TextType: " + @string);
			return;
		}
		string string2 = defField.GetString("EventName", null, "", true, true, true, '.');
		if (!Application.isPlaying)
		{
			string string3 = defField.GetString("Reason", null, "", true, true, true, '.');
			bool @bool = defField.GetBool("Direct", null, false, true, true, true, '.');
			string string4 = defField.GetString("Attitude", null, "", true, true, true, '.');
			Debug.Log(Diplomacy.GetTextKey(type, string2, string3, @bool, string4) ?? "null");
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom();
		if (kingdom == null)
		{
			Debug.LogError("No selected kingdom");
			return;
		}
		Logic.Kingdom plr_kingdom = BaseUI.LogicKingdom();
		Reason reason = Diplomacy.FindRelationshipReason(type, string2, kingdom, plr_kingdom);
		Debug.Log(Diplomacy.GetText(type, string2, null, kingdom, plr_kingdom, reason) ?? "null");
	}

	// Token: 0x0200060C RID: 1548
	public enum TextType
	{
		// Token: 0x0400338F RID: 13199
		Greeting,
		// Token: 0x04003390 RID: 13200
		Pleased,
		// Token: 0x04003391 RID: 13201
		Angry,
		// Token: 0x04003392 RID: 13202
		Threat,
		// Token: 0x04003393 RID: 13203
		Propose,
		// Token: 0x04003394 RID: 13204
		Comment,
		// Token: 0x04003395 RID: 13205
		CounterOffer,
		// Token: 0x04003396 RID: 13206
		ProposeWithCondition,
		// Token: 0x04003397 RID: 13207
		NeedsSweetening,
		// Token: 0x04003398 RID: 13208
		SweetenOffer,
		// Token: 0x04003399 RID: 13209
		Accept,
		// Token: 0x0400339A RID: 13210
		Decline,
		// Token: 0x0400339B RID: 13211
		Prompt,
		// Token: 0x0400339C RID: 13212
		Consider,
		// Token: 0x0400339D RID: 13213
		InvalidOffer,
		// Token: 0x0400339E RID: 13214
		COUNT
	}
}
