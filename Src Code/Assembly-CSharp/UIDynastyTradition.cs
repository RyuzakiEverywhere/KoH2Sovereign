using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001EE RID: 494
public class UIDynastyTradition : ObjectIcon, IListener
{
	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06001DA2 RID: 7586 RVA: 0x00116214 File Offset: 0x00114414
	// (remove) Token: 0x06001DA3 RID: 7587 RVA: 0x0011624C File Offset: 0x0011444C
	public event Action<UIDynastyTradition, PointerEventData> OnSelect;

	// Token: 0x06001DA4 RID: 7588 RVA: 0x00116284 File Offset: 0x00114484
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_RemoveTradition != null)
		{
			this.m_RemoveTradition.onClick = new BSGButton.OnClick(this.HandleRemoveTradition);
			Tooltip.Get(this.m_RemoveTradition.gameObject, true).SetDef("AbandonTraditionTooltip", null);
		}
		if (this.m_Abandon != null)
		{
			this.m_Abandon.onClick = new BSGButton.OnClick(this.HandleOnAbandon);
			this.m_Abandon.gameObject.SetActive(false);
		}
		this.ui_def = global::Defs.GetDefField("TraditionIcon", null);
		this.m_CanvasGroup = base.GetComponent<CanvasGroup>();
		this.UpdateVisualState();
		this.m_Initialized = true;
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x00116344 File Offset: 0x00114544
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		base.SetObject(obj, vars);
		Game game = GameLogic.Get(true);
		Logic.Kingdom kingdom = ((vars != null) ? vars.Get<Logic.Object>("owner", null) : null) as Logic.Kingdom;
		Tradition tradition;
		if ((tradition = (obj as Tradition)) != null)
		{
			this.SetData(kingdom, tradition, true);
			return;
		}
		Tradition.Def def;
		if ((def = (obj as Tradition.Def)) != null)
		{
			this.SetData(kingdom, def, true);
			return;
		}
		DT.Field field;
		if ((field = (obj as DT.Field)) != null)
		{
			Tradition.Def def2 = game.defs.Find<Tradition.Def>(field.key);
			this.SetData(kingdom, def2, true);
			return;
		}
		this.SetData(kingdom, null, true);
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x001163D9 File Offset: 0x001145D9
	private void SetKingdom(Logic.Kingdom k)
	{
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
		}
		this.kingdom = k;
		if (this.kingdom != null)
		{
			this.kingdom.AddListener(this);
		}
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x0011640A File Offset: 0x0011460A
	public void SetData(Logic.Kingdom kingdom, Tradition tradition, bool enabled = true)
	{
		this.Init();
		this.SetKingdom(kingdom);
		this.tradition = tradition;
		this.def = ((tradition != null) ? tradition.def : null);
		this.m_Enabled = enabled;
		this.Refresh();
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x0011643F File Offset: 0x0011463F
	public void SetData(Logic.Kingdom kingdom, Tradition.Def def, bool enabled = true)
	{
		this.Init();
		this.SetKingdom(kingdom);
		this.tradition = null;
		this.m_Enabled = enabled;
		this.def = def;
		this.Refresh();
	}

	// Token: 0x06001DA9 RID: 7593 RVA: 0x00116469 File Offset: 0x00114669
	public void Select(bool selected)
	{
		this.m_IsSelected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x00116478 File Offset: 0x00114678
	public void OnMessage(object obj, string message, object param)
	{
		if ((this == null || base.gameObject == null) && this.kingdom != null)
		{
			this.kingdom.DelListener(this);
			return;
		}
		if (message == "refresh_tags")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x06001DAB RID: 7595 RVA: 0x001164C5 File Offset: 0x001146C5
	public void Refresh()
	{
		if (this.tradition != null)
		{
			this.BuildAsInstanceData();
		}
		else if (this.def != null)
		{
			this.BuildAsDefData();
		}
		else
		{
			this.BuildAsEmpty();
		}
		this.UpdateVisualState();
	}

	// Token: 0x06001DAC RID: 7596 RVA: 0x001164F4 File Offset: 0x001146F4
	private void BuildAsEmpty()
	{
		if (this.m_GroupPopulated != null)
		{
			this.m_GroupPopulated.SetActive(false);
		}
		if (this.m_GroupEmpty != null)
		{
			this.m_GroupEmpty.SetActive(true);
		}
		bool flag = this.kingdom != null && this.kingdom == BaseUI.LogicKingdom();
		if (this.m_EmptyIcon_Add != null)
		{
			this.m_EmptyIcon_Add.SetActive(flag);
		}
		if (this.m_EmptyIcon_Neutral != null)
		{
			this.m_EmptyIcon_Neutral.SetActive(!flag);
		}
		this.UpdateName();
		this.SetupTooltip();
	}

	// Token: 0x06001DAD RID: 7597 RVA: 0x00116591 File Offset: 0x00114791
	public void SetHighlightAdopted(bool newValue)
	{
		this.m_HighlightAdopted = newValue;
		this.UpdateHighlight();
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x001165A0 File Offset: 0x001147A0
	private void SetupTooltip()
	{
		if (this.def == null)
		{
			UnityEngine.Object.Destroy(Tooltip.Get(base.gameObject, false));
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		Vars vars = new Vars();
		if (this.tradition != null)
		{
			vars.Set<Tradition>("tradition", this.tradition);
		}
		else
		{
			vars.Set<Tradition.Def>("tradition", this.def);
		}
		vars.Set<Logic.Kingdom>("kingdom", this.kingdom);
		int num = (this.kingdom == null) ? 0 : this.kingdom.GetTraditionRank(this.def);
		vars.Set<int>("rank", num);
		if (num > 0)
		{
			vars.Set<string>("rank_text", string.Format("SkillTooltip.rank_texts.{0}", num));
		}
		Vars vars2 = vars;
		string key = "slot_index";
		Vars vars3 = this.vars;
		vars2.Set<int>(key, (vars3 != null) ? vars3.Get<int>("slot_index", -1) : -1);
		tooltip.SetDef("TraditionTooltip", vars);
		if (this.m_Abandon != null)
		{
			Action obj = this.kingdom.actions.Find("AbandonTraditionAction");
			Tooltip tooltip2 = Tooltip.Get(this.m_Abandon.gameObject, true);
			tooltip2.SetObj(obj, null, null);
			tooltip2.vars.Set<Tradition.Def>("tradition", this.def);
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x001166E0 File Offset: 0x001148E0
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.vars : null;
		if (vars == null)
		{
			return Value.Unknown;
		}
		Value var = vars.GetVar("tradition", null, true);
		Tradition tradition;
		Tradition.Def tdef;
		if ((tradition = (var.obj_val as Tradition)) != null)
		{
			tdef = tradition.def;
		}
		else
		{
			Tradition.Def def;
			if ((def = (var.obj_val as Tradition.Def)) == null)
			{
				return Value.Unknown;
			}
			tdef = def;
		}
		Vars vars2 = vars as Vars;
		if (vars2 == null)
		{
			vars2 = new Vars(vars);
			arg.ht.vars = vars2;
		}
		UIDynastyTradition.FillHasSkillBonusesVars(vars2, tdef);
		Logic.Kingdom kingdom = arg.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		UIDynastyTradition.FillAdoptVars(vars2, tdef, kingdom);
		return Value.Unknown;
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x0011679C File Offset: 0x0011499C
	public static Tradition.Def GetTraditionDef(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.RootVars() : null;
		if (vars == null)
		{
			return null;
		}
		object obj_val = vars.GetVar("tradition", null, true).obj_val;
		Tradition tradition;
		if ((tradition = (obj_val as Tradition)) != null)
		{
			return tradition.def;
		}
		Tradition.Def result;
		if ((result = (obj_val as Tradition.Def)) != null)
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x001167F2 File Offset: 0x001149F2
	public static Skill.Def GetSkillDef(UIHyperText.CallbackParams arg)
	{
		return UIDynastyTradition.GetSkillDef(UIDynastyTradition.GetTraditionDef(arg));
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x001167FF File Offset: 0x001149FF
	public static Skill.Def GetSkillDef(Tradition.Def tdef)
	{
		if (((tdef != null) ? tdef.skills : null) == null || tdef.skills.Count == 0)
		{
			return null;
		}
		Tradition.Def.SkillInfo skillInfo = tdef.skills[0];
		if (skillInfo == null)
		{
			return null;
		}
		return skillInfo.def;
	}

	// Token: 0x06001DB3 RID: 7603 RVA: 0x00116838 File Offset: 0x00114A38
	public static Value PopulateAppliesToIcons(UIHyperText.CallbackParams arg)
	{
		Skill.Def skillDef = UIDynastyTradition.GetSkillDef(arg);
		if (skillDef == null)
		{
			return Value.Unknown;
		}
		DT.Field defField = global::Defs.GetDefField("CharacterClass", null);
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.defs : null) != null);
		}
		if (!flag)
		{
			return Value.Unknown;
		}
		List<Vars> list = new List<Vars>();
		for (int i = 0; i < defField.def.defs.Count; i++)
		{
			DT.Field field = defField.def.defs[i].field;
			Vars vars = new Vars(field);
			string key = field.key;
			vars.Set<string>("section", key);
			SkillsTable.CellDef cellDef = (skillDef != null) ? skillDef.row_def.FindCell(key) : null;
			if (cellDef == null)
			{
				vars.Set<bool>("inactive", true);
			}
			else if (cellDef.slot_type == "primary")
			{
				vars.Set<bool>("primary", true);
			}
			list.Add(vars);
		}
		return new Value(list);
	}

	// Token: 0x06001DB4 RID: 7604 RVA: 0x00116934 File Offset: 0x00114B34
	public static Value SetupSkillTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText.Element e = arg.e;
		GameObject gameObject;
		if (e == null)
		{
			gameObject = null;
		}
		else
		{
			RectTransform rt = e.rt;
			gameObject = ((rt != null) ? rt.gameObject : null);
		}
		GameObject gameObject2 = gameObject;
		if (gameObject2 == null)
		{
			return Value.Unknown;
		}
		string text = arg.e.GetVar("section", null, true).String(null);
		if (text == null)
		{
			return Value.Unknown;
		}
		DT.Field defField = global::Defs.GetDefField(text, null);
		if (defField == null || defField.base_path != "CharacterClass")
		{
			return Value.Unknown;
		}
		string key = defField.key;
		Skill.Def skillDef = UIDynastyTradition.GetSkillDef(arg);
		if (skillDef == null)
		{
			return Value.Unknown;
		}
		if (!skillDef.IsApplicableTo(key))
		{
			return Value.Unknown;
		}
		Logic.Kingdom target = arg.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		Vars vars = new Vars();
		UISkill.FillTTVars(vars, skillDef, target, key, true);
		Tooltip.Get(gameObject2, true).SetDef("SkillTooltip", vars);
		return true;
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x00116A28 File Offset: 0x00114C28
	public static Value PopulateTraditionKingdomEffects(UIHyperText.CallbackParams arg)
	{
		Tradition.Def traditionDef = UIDynastyTradition.GetTraditionDef(arg);
		if (traditionDef == null)
		{
			return Value.Unknown;
		}
		if (traditionDef.mods == null)
		{
			return Value.Null;
		}
		Logic.Kingdom kingdom = arg.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		int num = (kingdom == null) ? 0 : kingdom.GetTraditionRank(traditionDef);
		List<Vars> list = new List<Vars>();
		for (int i = 0; i < traditionDef.mods.Count; i++)
		{
			Tradition.StatModifier.Def def = traditionDef.mods[i];
			DT.Field defField = global::Defs.GetDefField("KingdomStats", def.stat_name);
			Value modText = UISkill.GetModText(def.field, kingdom, defField);
			if (!modText.is_null)
			{
				Vars vars = new Vars();
				if (num > 0 && num < def.min_rank)
				{
					vars.Set<bool>("inactive", true);
				}
				UISkill.SetValueVars(vars, def.field, null, defField, num, def.min_rank, true);
				UIDynastyTradition.FillTraditionKingdomModMultiplierVars(vars, kingdom, def, defField);
				vars.Set<string>("name", "KingdomStats." + def.stat_name + ".name");
				vars.Set<Value>("effect_text", modText);
				if (def.min_rank > 1)
				{
					vars.Set<int>("min_rank", def.min_rank);
				}
				list.Add(vars);
			}
		}
		return new Value(list);
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x00116B84 File Offset: 0x00114D84
	private static void FillTraditionKingdomModMultiplierVars(Vars mvars, Logic.Kingdom k, Tradition.StatModifier.Def mdef, DT.Field stat_field)
	{
		Stats.Def def = Def.Get<Stats.Def>((stat_field != null) ? stat_field.parent : null);
		Stat.Def def2 = (def != null) ? def.FindStat(mdef.stat_name, false) : null;
		float num;
		if (def2 != null)
		{
			num = def2.CalcMultiplier(k);
		}
		else
		{
			num = -1f;
		}
		if (num >= 0f)
		{
			mvars.Set<float>("multiplier", num);
			if (num == 0f)
			{
				mvars.Set<bool>("inactive", true);
				return;
			}
		}
		else
		{
			mvars.Set<Value>("multiplier", Value.Null);
		}
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x00116C08 File Offset: 0x00114E08
	private static void FillAdoptVars(Vars vars, Tradition.Def tdef, Logic.Kingdom kingdom)
	{
		Action action;
		if (kingdom == null)
		{
			action = null;
		}
		else
		{
			Actions actions = kingdom.actions;
			action = ((actions != null) ? actions.Find("AddAnyTraditionAction") : null);
		}
		Action action2 = action;
		if (action2 == null)
		{
			return;
		}
		vars.Set<Tradition.Def>("target", tdef);
		if (action2.args != null)
		{
			action2.args.Clear();
		}
		action2.AddArg(ref action2.args, tdef.id, 0);
		int num = vars.Get<int>("slot_index", -1);
		if (num >= 0 && kingdom.GetTradition(num) != null)
		{
			num = kingdom.GetFreeTraditionIndex(tdef.type);
		}
		action2.AddArg(ref action2.args, num, 1);
		DT.Field validatePrompt = action2.GetValidatePrompt(vars);
		vars.Set<DT.Field>("adopt_prompt", validatePrompt);
		vars.Set<Resource>("cost", action2.GetCost());
		vars.Set<bool>("adopted", kingdom.HasTradition(tdef.id));
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x00116CE4 File Offset: 0x00114EE4
	private static void FillHasSkillBonusesVars(Vars vars, Tradition.Def tdef)
	{
		bool val = false;
		bool val2 = false;
		Tradition.Def.SkillInfo skillInfo = tdef.skills[0];
		if (((skillInfo != null) ? skillInfo.bonus_cells : null) != null)
		{
			for (int i = 0; i < skillInfo.bonus_cells.Count; i++)
			{
				UIDynastyTradition.CheckHasSkillBonuses(skillInfo.bonus_cells[i], ref val, ref val2);
			}
		}
		vars.Set<bool>("has_govern_bonuses", val);
		vars.Set<bool>("has_non_govern_bonuses", val2);
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x00116D54 File Offset: 0x00114F54
	private static void CheckHasSkillBonuses(SkillsTable.CellDef cdef, ref bool has_govern_bonuses, ref bool has_non_govern_bonuses)
	{
		bool flag;
		bool flag2;
		if (cdef.HasBonuses() && UIDynastyTradition.ResolveTTSection(cdef, out flag, out flag2) != null)
		{
			if (flag)
			{
				has_govern_bonuses = true;
			}
			else
			{
				has_non_govern_bonuses = true;
			}
		}
		if (cdef.subcells != null)
		{
			for (int i = 0; i < cdef.subcells.Count; i++)
			{
				UIDynastyTradition.CheckHasSkillBonuses(cdef.subcells[i], ref has_govern_bonuses, ref has_non_govern_bonuses);
			}
		}
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x00116DB4 File Offset: 0x00114FB4
	public static Value PopulateTraditionSkillBonuses(UIHyperText.CallbackParams arg)
	{
		Tradition.Def traditionDef = UIDynastyTradition.GetTraditionDef(arg);
		if (traditionDef == null)
		{
			return Value.Unknown;
		}
		if (traditionDef.skills == null || traditionDef.skills.Count == 0)
		{
			return Value.Null;
		}
		Logic.Kingdom kingdom = arg.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		Tradition.Def.SkillInfo skillInfo = traditionDef.skills[0];
		if (((skillInfo != null) ? skillInfo.bonus_cells : null) == null)
		{
			return Value.Null;
		}
		int govern_cells = arg.GetVar("govern", null, true).Int(-1);
		int default_cell = arg.GetVar("default", null, true).Int(-1);
		List<Vars> list = new List<Vars>();
		for (int i = 0; i < skillInfo.bonus_cells.Count; i++)
		{
			SkillsTable.CellDef cdef = skillInfo.bonus_cells[i];
			UIDynastyTradition.AddTTCell(list, cdef, kingdom, govern_cells, default_cell);
		}
		for (int j = 0; j < list.Count; j++)
		{
			UISkill.SortEffects(list[j]);
		}
		return new Value(list);
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x00116EC4 File Offset: 0x001150C4
	private static Vars AddTTCell(List<Vars> sections, SkillsTable.CellDef cdef, Logic.Kingdom kingdom, int govern_cells, int default_cell)
	{
		Vars vars = null;
		if (cdef.HasBonuses())
		{
			string text = UIDynastyTradition.ResolveTTSection(cdef, govern_cells, default_cell);
			if (text != null)
			{
				vars = UISkill.FindTTSection(sections, text);
				if (vars == null)
				{
					vars = new Vars();
					UIDynastyTradition.FillSectionVars(vars, cdef, text, kingdom);
					sections.Add(vars);
				}
				bool valid = kingdom == null || cdef.Validate(kingdom, true, false, false);
				UISkill.AddEffects(vars, cdef, kingdom, valid);
			}
		}
		if (cdef.subcells != null)
		{
			for (int i = 0; i < cdef.subcells.Count; i++)
			{
				SkillsTable.CellDef cdef2 = cdef.subcells[i];
				UIDynastyTradition.AddTTCell(sections, cdef2, kingdom, govern_cells, default_cell);
			}
		}
		return vars;
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x00116F60 File Offset: 0x00115160
	private static void FillSectionVars(Vars svars, SkillsTable.CellDef cdef, string section, Logic.Kingdom kingdom)
	{
		svars.Set<string>("section", section);
		if (section == "Default" || section == "Govern")
		{
			svars.Set<Value>("section_label", Value.Null);
			return;
		}
		DT.Field field = global::Defs.FindTextField("TraditionTooltipHT.section_labels." + section);
		if (field == null)
		{
			field = global::Defs.FindTextField(section + ".name");
		}
		if (field != null)
		{
			svars.Set<Value>("section_label", new Value(field));
			return;
		}
		svars.Set<string>("section_label", section);
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x00116FEC File Offset: 0x001151EC
	private static string ResolveTTSection(SkillsTable.CellDef cdef, int govern_cells, int default_cell)
	{
		bool flag;
		bool flag2;
		string text = UIDynastyTradition.ResolveTTSection(cdef, out flag, out flag2);
		if (text == null)
		{
			return null;
		}
		if (govern_cells >= 0 && flag != (govern_cells != 0))
		{
			return null;
		}
		if (default_cell >= 0 && flag2 != (default_cell != 0))
		{
			return null;
		}
		return text;
	}

	// Token: 0x06001DBE RID: 7614 RVA: 0x00117024 File Offset: 0x00115224
	private static string ResolveTTSection(SkillsTable.CellDef cdef, out bool is_govern, out bool is_default)
	{
		is_govern = (is_default = false);
		if (!(cdef.field.type == "at_rank"))
		{
			if (cdef.tradition_def != null)
			{
				cdef = cdef.parent;
				if (cdef == null)
				{
					return null;
				}
			}
			if (cdef.name == "Govern")
			{
				is_govern = true;
				if (cdef.parent != null)
				{
					cdef = cdef.parent;
					if (cdef.tradition_def != null)
					{
						cdef = cdef.parent;
						if (cdef == null)
						{
							return null;
						}
					}
				}
				else
				{
					is_default = true;
				}
			}
			if (cdef.name == "Default")
			{
				is_default = true;
			}
			return cdef.name;
		}
		if (cdef.parent == null)
		{
			return null;
		}
		return UIDynastyTradition.ResolveTTSection(cdef.parent, out is_govern, out is_default);
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x001170D8 File Offset: 0x001152D8
	private void BuildAsDefData()
	{
		if (this.m_GroupEmpty != null)
		{
			this.m_GroupEmpty.SetActive(false);
		}
		if (this.m_GroupPopulated != null)
		{
			this.m_GroupPopulated.SetActive(true);
		}
		if (this.def == null)
		{
			return;
		}
		if (this.m_Icon != null)
		{
			Rect rect = this.m_Icon.rectTransform.rect;
			string key = (rect.width / rect.height > 1.3f) ? "icon_wide" : "icon";
			this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.def.dt_def.field, key, null);
		}
		this.UpdateName();
		this.SetupTooltip();
		this.UpdateHighlight();
	}

	// Token: 0x06001DC0 RID: 7616 RVA: 0x00117198 File Offset: 0x00115398
	private void BuildAsInstanceData()
	{
		if (this.m_GroupEmpty != null)
		{
			this.m_GroupEmpty.SetActive(false);
		}
		if (this.m_GroupPopulated != null)
		{
			this.m_GroupPopulated.SetActive(true);
		}
		if (this.tradition == null)
		{
			return;
		}
		if (this.m_Icon != null)
		{
			this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.def.dt_def.field, "icon", null);
		}
		this.UpdateName();
		this.SetupTooltip();
		this.UpdateHighlight();
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x00117228 File Offset: 0x00115428
	private void UpdateName()
	{
		bool flag = this.def != null || this.tradition != null;
		if (this.m_Name != null)
		{
			if (flag && this.m_ShowName)
			{
				Vars vars = new Vars();
				vars.Set<DT.Field>("name", global::Defs.FindTextField(this.def.field, "name"));
				int num = (this.kingdom == null) ? 0 : this.kingdom.GetTraditionRank(this.def);
				vars.Set<int>("rank", num);
				if (num > 0)
				{
					vars.Set<string>("rank_text", string.Format("SkillTooltip.rank_texts.{0}", num));
				}
				UIText.SetTextKey(this.m_Name, "Tradition.name_rank", vars, null);
			}
			this.m_Name.gameObject.SetActive(this.m_ShowName && flag);
		}
		if (this.m_NameBackground != null)
		{
			this.m_NameBackground.gameObject.SetActive(this.m_ShowName && flag);
		}
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x0011732C File Offset: 0x0011552C
	private void HandleRemoveTradition(BSGButton b)
	{
		if (this.kingdom == null)
		{
			return;
		}
		if (this.kingdom.traditions == null || this.kingdom.traditions.Count == 0)
		{
			return;
		}
		if (this.def == null)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < this.kingdom.traditions.Count; i++)
		{
			Tradition tradition = this.kingdom.traditions[i];
			if (((tradition != null) ? tradition.def : null) == this.def)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			this.kingdom.SetTradition(num, null, 0, true);
		}
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x001173C5 File Offset: 0x001155C5
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x001173D4 File Offset: 0x001155D4
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x001173E4 File Offset: 0x001155E4
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		bool flag = this.m_CurrentVisualState == "adopted";
		if (this.m_Hover != null && this.m_Hover.Length != 0)
		{
			for (int i = 0; i < this.m_Hover.Length; i++)
			{
				this.m_Hover[i].gameObject.SetActive(this.mouse_in && this.m_Enabled && (!flag || !this.m_HighlightAdopted));
			}
		}
		if (this.m_Selected != null && this.m_Selected.Length != 0)
		{
			for (int j = 0; j < this.m_Selected.Length; j++)
			{
				this.m_Selected[j].gameObject.SetActive(this.m_IsSelected);
			}
		}
		if (this.m_Adopted != null)
		{
			this.m_Adopted.SetActive(flag && this.m_HighlightAdopted);
		}
		if (this.m_Abandon != null)
		{
			this.m_Abandon.gameObject.SetActive(this.mouse_in && flag);
		}
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x001174E9 File Offset: 0x001156E9
	public void SetVisualState(string v)
	{
		this.m_CurrentVisualState = v;
		this.UpdateVisualState();
		this.UpdateHighlight();
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x001174FE File Offset: 0x001156FE
	public string GetVisualState()
	{
		return this.m_CurrentVisualState;
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x00117508 File Offset: 0x00115708
	private void UpdateVisualState()
	{
		string currentVisualState = this.m_CurrentVisualState;
		if (!(currentVisualState == "unknown") && !(currentVisualState == "adopted"))
		{
			if (!(currentVisualState == "reqierment_not_met"))
			{
				if (!(currentVisualState == "ivalid_slot"))
				{
					if (!(currentVisualState == "adoptable"))
					{
						return;
					}
					this.m_Icon.color = global::Defs.GetColor(this.ui_def, "icon_tint.adoptable", Color.white, null);
					if (this.m_BorderActive != null)
					{
						this.m_BorderActive.gameObject.SetActive(false);
					}
					if (this.m_BorderAvailable != null)
					{
						this.m_BorderAvailable.gameObject.SetActive(true);
					}
				}
				else
				{
					this.m_Icon.color = global::Defs.GetColor(this.ui_def, "icon_tint.invalid_slot", UIDynastyTradition.iconTint_IncompatibleSlot, null);
					if (this.m_BorderActive != null)
					{
						this.m_BorderActive.gameObject.SetActive(false);
					}
					if (this.m_BorderAvailable != null)
					{
						this.m_BorderAvailable.gameObject.SetActive(false);
						return;
					}
				}
			}
			else
			{
				this.m_Icon.color = global::Defs.GetColor(this.ui_def, "icon_tint.reqierment_not_met", UIDynastyTradition.iconTint_ReqiermentsNotMet, null);
				if (this.m_BorderActive != null)
				{
					this.m_BorderActive.gameObject.SetActive(false);
				}
				if (this.m_BorderAvailable != null)
				{
					this.m_BorderAvailable.gameObject.SetActive(false);
					return;
				}
			}
		}
		else
		{
			this.m_Icon.color = global::Defs.GetColor(this.ui_def, "icon_tint.adopted", Color.white, null);
			if (this.m_BorderActive != null)
			{
				this.m_BorderActive.gameObject.SetActive(true);
			}
			if (this.m_BorderAvailable != null)
			{
				this.m_BorderAvailable.gameObject.SetActive(false);
				return;
			}
		}
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x001176F6 File Offset: 0x001158F6
	public void ShowName(bool shown)
	{
		this.m_ShowName = shown;
		this.UpdateName();
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x00117705 File Offset: 0x00115905
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
			return;
		}
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x00117724 File Offset: 0x00115924
	private void HandleOnAbandon(BSGButton btn)
	{
		if (this.def == null)
		{
			return;
		}
		if (this.kingdom == null)
		{
			return;
		}
		Action action = this.kingdom.actions.Find("AbandonTraditionAction");
		if (action != null)
		{
			if (action.args != null)
			{
				action.args = null;
			}
			action.AddArg(ref action.args, this.def.id, 0);
			ActionVisuals.ExecuteAction(action);
		}
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x0011778E File Offset: 0x0011598E
	private void OnDestroy()
	{
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
		}
		this.OnSelect = null;
		this.SetKingdom(null);
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x001177B4 File Offset: 0x001159B4
	public static GameObject GetPrefab(string variant = "")
	{
		string text = "prefab";
		if (!string.IsNullOrEmpty(variant))
		{
			text = text + "." + variant;
		}
		GameObject obj = global::Defs.GetObj<GameObject>("TraditionIcon", text, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<GameObject>("TraditionIcon", "prefab", null);
		}
		return obj;
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x00117804 File Offset: 0x00115A04
	public static UIDynastyTradition Create(Tradition.Def tradition, GameObject prototype, RectTransform parent, Vars vars = null)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIDynastyTradition orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIDynastyTradition>();
		orAddComponent.SetObject(tradition, vars);
		orAddComponent.gameObject.SetActive(true);
		return orAddComponent;
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x00117804 File Offset: 0x00115A04
	public static UIDynastyTradition Create(Tradition tradition, GameObject prototype, RectTransform parent, Vars vars = null)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIDynastyTradition orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIDynastyTradition>();
		orAddComponent.SetObject(tradition, vars);
		orAddComponent.gameObject.SetActive(true);
		return orAddComponent;
	}

	// Token: 0x04001376 RID: 4982
	[UIFieldTarget("id_SkillIcon")]
	[SerializeField]
	private Image m_Icon;

	// Token: 0x04001377 RID: 4983
	[UIFieldTarget("id_Group_Empty")]
	private GameObject m_GroupEmpty;

	// Token: 0x04001378 RID: 4984
	[UIFieldTarget("id_EmptyIcon_Neutral")]
	private GameObject m_EmptyIcon_Neutral;

	// Token: 0x04001379 RID: 4985
	[UIFieldTarget("id_EmptyIcon_Add")]
	private GameObject m_EmptyIcon_Add;

	// Token: 0x0400137A RID: 4986
	[UIFieldTarget("id_Group_Populated")]
	private GameObject m_GroupPopulated;

	// Token: 0x0400137B RID: 4987
	[UIFieldTarget("id_OverrideIcon")]
	private Image[] m_OverrideIcon;

	// Token: 0x0400137C RID: 4988
	[UIFieldTarget("id_RemoveTradition")]
	private BSGButton m_RemoveTradition;

	// Token: 0x0400137D RID: 4989
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI m_Name;

	// Token: 0x0400137E RID: 4990
	[UIFieldTarget("id_NameBackground")]
	private GameObject m_NameBackground;

	// Token: 0x0400137F RID: 4991
	[UIFieldTarget("id_BorderActive")]
	private GameObject m_BorderActive;

	// Token: 0x04001380 RID: 4992
	[UIFieldTarget("id_BorderAvailable")]
	private GameObject m_BorderAvailable;

	// Token: 0x04001381 RID: 4993
	[UIFieldTarget("id_Hover")]
	private GameObject[] m_Hover;

	// Token: 0x04001382 RID: 4994
	[UIFieldTarget("id_Selected")]
	private GameObject[] m_Selected;

	// Token: 0x04001383 RID: 4995
	[UIFieldTarget("id_Abandon")]
	private BSGButton m_Abandon;

	// Token: 0x04001384 RID: 4996
	[UIFieldTarget("id_Adopted")]
	private GameObject m_Adopted;

	// Token: 0x04001386 RID: 4998
	[HideInInspector]
	public Logic.Kingdom kingdom;

	// Token: 0x04001387 RID: 4999
	[HideInInspector]
	public Tradition.Def def;

	// Token: 0x04001388 RID: 5000
	[HideInInspector]
	public Tradition tradition;

	// Token: 0x04001389 RID: 5001
	[HideInInspector]
	public DT.Field ui_def;

	// Token: 0x0400138A RID: 5002
	private CanvasGroup m_CanvasGroup;

	// Token: 0x0400138B RID: 5003
	private bool m_Enabled;

	// Token: 0x0400138C RID: 5004
	private bool m_Initialized;

	// Token: 0x0400138D RID: 5005
	private bool m_ShowName = true;

	// Token: 0x0400138E RID: 5006
	private bool m_IsSelected;

	// Token: 0x0400138F RID: 5007
	private bool m_HighlightAdopted;

	// Token: 0x04001390 RID: 5008
	private string m_CurrentVisualState = "adoptable";

	// Token: 0x04001391 RID: 5009
	private static Color32 iconTint_ReqiermentsNotMet = new Color32(143, 138, 152, 20);

	// Token: 0x04001392 RID: 5010
	private static Color32 iconTint_IncompatibleSlot = new Color32(byte.MaxValue, 170, 238, 180);
}
