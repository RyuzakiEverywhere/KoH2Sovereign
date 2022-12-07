using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class UIResources : MonoBehaviour
{
	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060020BE RID: 8382 RVA: 0x0012AC0B File Offset: 0x00128E0B
	// (set) Token: 0x060020BF RID: 8383 RVA: 0x0012AC13 File Offset: 0x00128E13
	public Resource Data { get; private set; }

	// Token: 0x060020C0 RID: 8384 RVA: 0x0012AC1C File Offset: 0x00128E1C
	public void SetData(Resource resources)
	{
		this.Data = resources;
	}

	// Token: 0x060020C1 RID: 8385 RVA: 0x0012AC25 File Offset: 0x00128E25
	private void Refresh()
	{
		this.Data == null;
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x0012AC34 File Offset: 0x00128E34
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.vars : null;
		if (vars == null)
		{
			return Value.Unknown;
		}
		Resource.Def def = vars.GetVar("resource", null, true).Get<Resource.Def>();
		if (def == null)
		{
			def = vars.GetVar("obj", null, true).Get<Resource.Def>();
			if (def == null)
			{
				return Value.Unknown;
			}
		}
		Vars vars2 = vars as Vars;
		if (vars2 == null)
		{
			vars2 = new Vars(vars);
			arg.ht.vars = vars2;
		}
		Logic.Kingdom k = (UIText.cur_article != null) ? null : BaseUI.LogicKingdom();
		UIResources.FillProducedIn(vars2, def, k);
		UIResources.FillRequiredFor(vars2, def, k);
		UIResources.FillAvailability(vars2, "Resource.availability_texts", def.id, k);
		UIResources.FillAdvantages(vars2, def, k);
		UIResources.FillProvinceFeautres(vars2, def, k);
		UIResources.FillTeamMates(vars2, def, k);
		bool val = def.mdefs.Count > 0;
		vars2.Set<bool>("resource_have_mods", val);
		return Value.Unknown;
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x0012AD20 File Offset: 0x00128F20
	public static Value GetModText(DT.Field mdef_field, Logic.Kingdom k, DT.Field stat_field)
	{
		DT.Field field = global::Defs.FindTextField(mdef_field, "tooltip");
		if (field != null)
		{
			if (!field.value.is_valid)
			{
				return Value.Null;
			}
			return new Value(field);
		}
		else
		{
			if (stat_field != null && !stat_field.GetBool("show_value", null, true, true, true, true, '.'))
			{
				return Value.Null;
			}
			field = global::Defs.FindTextField(stat_field, "mod_text");
			if (field == null)
			{
				return "Stats.mod_text";
			}
			if (!field.value.is_valid)
			{
				return Value.Null;
			}
			return new Value(field);
		}
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x0012ADA8 File Offset: 0x00128FA8
	public static void SetValueVars(Vars vars, DT.Field mod_field, Logic.Object target, DT.Field stat_field)
	{
		int num = mod_field.NumValues();
		float num2 = mod_field.Float(0, target, 0f);
		bool flag = num2 < 0f;
		bool flag2 = num == 1 && num2 == 0f;
		bool @bool = stat_field.GetBool("penalty", null, false, true, true, true, '.');
		string text = "active";
		string str = "inactive";
		text += "_tradition";
		str += "_tradition";
		if (flag == @bool)
		{
			text += "_buff";
			str += "_buff";
		}
		else
		{
			text += "_penalty";
			str += "_penalty";
		}
		string str2;
		if (flag)
		{
			str2 = "-";
		}
		else if (stat_field.GetBool("show_plus_sign", null, true, true, true, true, '.'))
		{
			str2 = "+";
		}
		else
		{
			str2 = "";
		}
		string text2 = "@";
		for (int i = 0; i < num; i++)
		{
			if (i > 0)
			{
				text2 += "{SkillTooltip.value_separator}";
			}
			string str3 = text;
			text2 = text2 + "{clr:" + str3 + "}";
			if (i == 0)
			{
				text2 += str2;
			}
			float num3 = mod_field.Float(i, target, 0f);
			if (num3 < 0f)
			{
				num3 = -num3;
			}
			text2 += DT.FloatToStr(num3, 3);
			if (i == num - 1 && (mod_field.FindChild("perc", null, true, true, true, '.') != null || stat_field.key.EndsWith("_perc", StringComparison.Ordinal)))
			{
				text2 += "%";
			}
			text2 += "{/clr}";
		}
		vars.Set<string>("value", text2);
		if (flag2)
		{
			vars.Set<bool>("inactive", true);
		}
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x0012AF78 File Offset: 0x00129178
	public static Value PopulateResourceKingdomEffects(UIHyperText.CallbackParams arg)
	{
		List<Vars> list = new List<Vars>();
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.RootVars() : null;
		Logic.Kingdom kingdom = (UIText.cur_article != null) ? null : BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return Value.Unknown;
		}
		Resource.Def def = vars.GetVar("resource", null, true).Get<Resource.Def>();
		if (def == null)
		{
			def = vars.GetVar("obj", null, true).Get<Resource.Def>();
			if (def == null)
			{
				return Value.Unknown;
			}
		}
		Game game = GameLogic.Get(false);
		Stats.Def def2;
		if (game == null)
		{
			def2 = null;
		}
		else
		{
			Logic.Defs defs = game.defs;
			def2 = ((defs != null) ? defs.Get<Stats.Def>("KingdomStats") : null);
		}
		Stats.Def def3 = def2;
		for (int i = 0; i < def.mdefs.Count; i++)
		{
			Resource.StatModifier.Def def4 = def.mdefs[i];
			DT.Field defField = global::Defs.GetDefField("KingdomStats", def4.stat_name);
			Value modText = UIResources.GetModText(def4.field, kingdom, defField);
			if (!modText.is_null)
			{
				Vars vars2 = new Vars();
				UIResources.SetValueVars(vars2, def4.field, null, defField);
				vars2.Set<string>("name", "KingdomStats." + def4.stat_name + ".name");
				vars2.Set<Value>("effect_text", modText);
				Stat.Def def5 = (def3 != null) ? def3.FindStat(def4.stat_name, false) : null;
				float num;
				if (def5 != null)
				{
					num = def5.CalcMultiplier(kingdom);
				}
				else
				{
					num = -1f;
				}
				if (num >= 0f)
				{
					vars2.Set<float>("multiplier", num);
					if (num == 0f)
					{
						vars2.Set<bool>("inactive", true);
					}
				}
				else
				{
					vars2.Set<Value>("multiplier", Value.Null);
				}
				list.Add(vars2);
			}
		}
		return new Value(list);
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x0012B13C File Offset: 0x0012933C
	private static bool GetAvailabilityColors(Logic.Kingdom k, string def_id, out string open_tag, out string close_tag)
	{
		open_tag = "";
		close_tag = "";
		if (k == null || def_id == null)
		{
			return false;
		}
		ResourceInfo resourceInfo = k.GetResourceInfo(def_id, true, true);
		if (resourceInfo == null)
		{
			return false;
		}
		string colorKey = resourceInfo.GetColorKey();
		if (colorKey == null)
		{
			return false;
		}
		open_tag = "{" + colorKey + "}";
		close_tag = "{/" + colorKey + "}";
		return true;
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x0012B1A0 File Offset: 0x001293A0
	public static string ResourceText(Logic.Kingdom k, string id, bool add_upgrade_building)
	{
		string text;
		string text2;
		string text3;
		if (UIResources.GetAvailabilityColors(k, id, out text, out text2))
		{
			text3 = string.Concat(new string[]
			{
				"{",
				id,
				":link}",
				text,
				"{",
				id,
				":nolink}",
				text2,
				"{",
				id,
				":/link}"
			});
		}
		else
		{
			text3 = "{" + id + "}";
		}
		if (!add_upgrade_building)
		{
			return text3;
		}
		Building.Def def = GameLogic.Get(true).defs.Find<Building.Def>(id);
		if (def == null)
		{
			return text3;
		}
		List<Building.Def> upgradeOf = def.GetUpgradeOf();
		if (upgradeOf != null && upgradeOf.Count > 0)
		{
			for (int i = 0; i < upgradeOf.Count; i++)
			{
				Building.Def def2 = upgradeOf[i];
				if (i == 0)
				{
					text3 += " (";
				}
				else
				{
					text3 += ", ";
				}
				text3 += UIResources.ResourceText(k, def2.id, false);
			}
			text3 += ")";
		}
		return text3;
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x0012B2B4 File Offset: 0x001294B4
	private static void FillProducedIn(Vars vars, Resource.Def rdef, Logic.Kingdom k)
	{
		if (rdef.produced_in == null || rdef.produced_in.Count == 0)
		{
			return;
		}
		string text = "@";
		for (int i = 0; i < rdef.produced_in.Count; i++)
		{
			Building.Def def = rdef.produced_in[i];
			if (i > 0)
			{
				text += ", ";
			}
			text += UIResources.ResourceText(k, def.id, true);
		}
		vars.Set<string>("produced_in_text", text);
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x0012B330 File Offset: 0x00129530
	private static void FillRequiredFor(Vars vars, Resource.Def rdef, Logic.Kingdom k)
	{
		if (rdef.required_for == null || rdef.required_for.Count == 0)
		{
			return;
		}
		string text = "@";
		for (int i = 0; i < rdef.required_for.Count; i++)
		{
			string id = rdef.required_for[i];
			if (i > 0)
			{
				text += ", ";
			}
			text += UIResources.ResourceText(k, id, false);
		}
		vars.Set<string>("required_for_text", text);
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x0012B3A8 File Offset: 0x001295A8
	public static void FillAvailability(Vars vars, string availability_texts_path, string name, Logic.Kingdom k)
	{
		ResourceInfo resourceInfo = (k != null) ? k.GetResourceInfo(name, true, true) : null;
		if (resourceInfo != null)
		{
			resourceInfo.SetTextVars(vars, availability_texts_path);
		}
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x0012B3D0 File Offset: 0x001295D0
	private static void GetAdvantageColor(KingdomAdvantage adv, out string open_tag, out string close_tag)
	{
		if (adv == null)
		{
			open_tag = "";
			close_tag = "";
			return;
		}
		if (adv.CheckRequirements())
		{
			open_tag = "{requirement_met}";
			close_tag = "{/requirement_met}";
			return;
		}
		if (adv.CheckHardRequirements())
		{
			open_tag = "{requirement_not_met}";
			close_tag = "{/requirement_not_met}";
			return;
		}
		open_tag = "{requirement_currently_impossible}";
		close_tag = "{/requirement_currently_impossible}";
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x0012B42C File Offset: 0x0012962C
	private static void FillAdvantages(Vars vars, Resource.Def rdef, Logic.Kingdom k)
	{
		if (rdef.advantages == null || rdef.advantages.Count == 0)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < rdef.advantages.Count; i++)
		{
			string text2 = rdef.advantages[i];
			KingdomAdvantage adv;
			if (k == null)
			{
				adv = null;
			}
			else
			{
				KingdomAdvantages advantages = k.advantages;
				adv = ((advantages != null) ? advantages.FindById(text2) : null);
			}
			if (i > 0)
			{
				if (i == rdef.advantages.Count - 1)
				{
					text += "{list_final_separator}";
				}
				else
				{
					text += "{list_separator}";
				}
			}
			string text3;
			string text4;
			UIResources.GetAdvantageColor(adv, out text3, out text4);
			text = string.Concat(new string[]
			{
				text,
				"{",
				text2,
				":link}",
				text3,
				"{",
				text2,
				":nolink}",
				text4,
				"{",
				text2,
				":/link}"
			});
		}
		if (!string.IsNullOrEmpty(text))
		{
			vars.Set<string>("advantages_text", "@" + text);
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x0012B548 File Offset: 0x00129748
	private static void FillProvinceFeautres(Vars vars, Resource.Def rdef, Logic.Kingdom k)
	{
		if (rdef.province_features == null || rdef.province_features.Count == 0)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < rdef.province_features.Count; i++)
		{
			string id = rdef.province_features[i];
			if (i > 0)
			{
				if (i == rdef.province_features.Count - 1)
				{
					text += "{list_final_separator_or}";
				}
				else
				{
					text += "{list_separator}";
				}
			}
			text += UIResources.ResourceText(k, id, false);
		}
		if (!string.IsNullOrEmpty(text))
		{
			vars.Set<string>("province_features_text", "@" + text);
		}
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x0012B5F0 File Offset: 0x001297F0
	private static void FillTeamMates(Vars vars, Resource.Def rdef, Logic.Kingdom k)
	{
		bool flag;
		if (k == null)
		{
			flag = (null != null);
		}
		else
		{
			Game game = k.game;
			flag = (((game != null) ? game.campaign : null) != null);
		}
		if (!flag || !k.game.campaign.IsMultiplayerCampaign())
		{
			return;
		}
		List<Game.Player> list;
		if (k == null)
		{
			list = null;
		}
		else
		{
			Game game2 = k.game;
			if (game2 == null)
			{
				list = null;
			}
			else
			{
				Game.Teams teams = game2.teams;
				if (teams == null)
				{
					list = null;
				}
				else
				{
					Game.Team team = teams.Get(k);
					list = ((team != null) ? team.players : null);
				}
			}
		}
		List<Game.Player> list2 = list;
		if (list2 == null || list2.Count == 1)
		{
			return;
		}
		List<Logic.Kingdom> list3 = new List<Logic.Kingdom>();
		for (int i = 0; i < list2.Count; i++)
		{
			Logic.Kingdom kingdom = k.game.GetKingdom(list2[i].kingdom_id);
			if (kingdom != null && kingdom != k && kingdom.GetRealmTag(rdef.field.key) > 0)
			{
				list3.Add(kingdom);
			}
		}
		if (list3.Count > 0)
		{
			vars.Set<List<Logic.Kingdom>>("team_mate_kingdoms", list3);
		}
	}
}
