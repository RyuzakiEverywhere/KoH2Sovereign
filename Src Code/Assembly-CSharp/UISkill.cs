using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001F5 RID: 501
public class UISkill : ObjectIcon, IListener
{
	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06001E50 RID: 7760 RVA: 0x00119F48 File Offset: 0x00118148
	// (remove) Token: 0x06001E51 RID: 7761 RVA: 0x00119F80 File Offset: 0x00118180
	public event Action<UISkill, PointerEventData> OnSelect;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06001E52 RID: 7762 RVA: 0x00119FB8 File Offset: 0x001181B8
	// (remove) Token: 0x06001E53 RID: 7763 RVA: 0x00119FF0 File Offset: 0x001181F0
	public event Action<UISkill, PointerEventData> OnFocusGain;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06001E54 RID: 7764 RVA: 0x0011A028 File Offset: 0x00118228
	// (remove) Token: 0x06001E55 RID: 7765 RVA: 0x0011A060 File Offset: 0x00118260
	public event Action<UISkill, PointerEventData> OnFocusLoss;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06001E56 RID: 7766 RVA: 0x0011A098 File Offset: 0x00118298
	// (remove) Token: 0x06001E57 RID: 7767 RVA: 0x0011A0D0 File Offset: 0x001182D0
	public event Action<UISkill> OnReplace;

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001E58 RID: 7768 RVA: 0x0011A105 File Offset: 0x00118305
	// (set) Token: 0x06001E59 RID: 7769 RVA: 0x0011A10D File Offset: 0x0011830D
	public string Type { get; private set; }

	// Token: 0x06001E5A RID: 7770 RVA: 0x0011A116 File Offset: 0x00118316
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.IconDef = global::Defs.GetDefField("SkillIcon", null);
		this.m_Initialzied = true;
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x0011A140 File Offset: 0x00118340
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		Game game = GameLogic.Get(true);
		Logic.Object @object = (vars != null) ? vars.Get<Logic.Object>("owner", null) : null;
		if (obj is DT.Field)
		{
			DT.Field field = obj as DT.Field;
			if (field != null)
			{
				Skill.Def def = game.defs.Find<Skill.Def>(field.key);
				if (def != null)
				{
					this.SetData(@object, def, true, true);
					return;
				}
			}
		}
		else if (obj is Skill.Def)
		{
			Skill.Def def2 = obj as Skill.Def;
			if (def2 != null)
			{
				this.SetData(@object, def2, true, true);
			}
		}
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x0011A1C0 File Offset: 0x001183C0
	public void OverrideType(string type)
	{
		this.Init();
		this.Type = type;
		this.UpdateIcon();
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x0011A1D5 File Offset: 0x001183D5
	private void SetOwner(Logic.Object o)
	{
		if (this.owner != null)
		{
			this.owner.DelListener(this);
		}
		this.owner = o;
		if (this.owner != null)
		{
			this.owner.AddListener(this);
		}
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x0011A208 File Offset: 0x00118408
	public void SetData(Logic.Object owner, Skill skill, bool enabled = true, bool show_cost = true, bool full = true)
	{
		this.Init();
		int num = (skill == null) ? 0 : skill.rank;
		if (!full && owner == this.owner && skill == this.skill && num == this.rank)
		{
			return;
		}
		this.rank = num;
		this.SetOwner(owner);
		this.skill = skill;
		this.def = ((skill == null) ? null : skill.def);
		this.m_Enabled = enabled;
		this.m_ShowCost = show_cost;
		this.CalcType();
		this.Refresh();
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x0011A28A File Offset: 0x0011848A
	public void SetData(Logic.Object owner, Skill.Def skillDef, bool enabled = true, bool show_cost = true)
	{
		this.Init();
		this.SetOwner(owner);
		this.skill = null;
		this.m_Enabled = enabled;
		this.def = skillDef;
		this.m_ShowCost = show_cost;
		this.CalcType();
		this.Refresh();
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x0011A2C4 File Offset: 0x001184C4
	private void CalcType()
	{
		if (this.owner is Logic.Character)
		{
			Skill.Def def = this.def;
			string text = (def != null) ? def.GetSlotType(this.owner as Logic.Character) : null;
			if (!string.IsNullOrEmpty(text))
			{
				this.Type = text;
			}
			return;
		}
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x0011A30C File Offset: 0x0011850C
	public void SetOverrideIcon(Sprite s)
	{
		this.m_OverrideSprite = s;
		this.UpdateIcon();
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x0011A31B File Offset: 0x0011851B
	public void SetEligable(bool eligable)
	{
		if (this.m_Eilgable == eligable)
		{
			return;
		}
		this.m_Eilgable = eligable;
		this.UpdateHighlight();
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x0011A334 File Offset: 0x00118534
	public void Select(bool selectd)
	{
		if (this.m_Selected == selectd)
		{
			return;
		}
		this.m_Selected = selectd;
		this.UpdateHighlight();
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x0011A350 File Offset: 0x00118550
	private void UpdateIcon()
	{
		if (this.skill == null && this.def == null && this.m_SkillLocked != null)
		{
			this.m_SkillLocked.SetActive(this.m_OverrideSprite == null && !this.m_Enabled);
		}
		if (this.m_OverrideIcon != null && this.m_OverrideIcon.Length != 0)
		{
			for (int i = 0; i < this.m_OverrideIcon.Length; i++)
			{
				if (this.m_OverrideIcon[i] != null)
				{
					this.m_OverrideIcon[i].sprite = this.m_OverrideSprite;
					this.m_OverrideIcon[i].gameObject.SetActive(this.m_OverrideSprite != null);
				}
			}
		}
		if (this.m_IconType != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.IconDef, "type." + this.Type, null);
			this.m_IconType.overrideSprite = obj;
		}
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x0011A43C File Offset: 0x0011863C
	public void ShowReplaceButton(bool shown)
	{
		this.m_ShowReplace = shown;
		if (this.m_ReplaceSkill != null)
		{
			this.m_ReplaceSkill.gameObject.SetActive(this.m_ShowReplace);
		}
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x0011A469 File Offset: 0x00118669
	public void ShowAddButton(bool shown)
	{
		this.m_ShowAdd = shown;
		this.UpdateHighlight();
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x0011A478 File Offset: 0x00118678
	private void HandleOnReplace(BSGButton btn)
	{
		if (this.OnReplace != null)
		{
			this.OnReplace(this);
		}
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x0011A490 File Offset: 0x00118690
	public void OnMessage(object obj, string message, object param)
	{
		if (base.gameObject == null && this.owner != null)
		{
			this.owner.DelListener(this);
			return;
		}
		if (message == "refresh_tags" || message == "skills_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x0011A4E0 File Offset: 0x001186E0
	public void Refresh()
	{
		if (this.m_ReplaceSkill != null)
		{
			this.m_ReplaceSkill.onClick = new BSGButton.OnClick(this.HandleOnReplace);
		}
		if (this.m_ReplaceSkill != null)
		{
			this.m_ReplaceSkill.gameObject.SetActive(this.m_ShowReplace);
		}
		if (this.skill != null)
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
		this.UpdateRank();
		this.UpdateHighlight();
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x0011A568 File Offset: 0x00118768
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
		this.UpdateIcon();
		this.SetupTooltip();
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x0011A5B8 File Offset: 0x001187B8
	private void UpdateAddIndication()
	{
		if (this.m_SkillAdd != null)
		{
			bool flag = false;
			if (this.owner != null && this.owner is Logic.Character)
			{
				flag = !(this.owner as Logic.Character).CanLearnNewSkills();
			}
			Color color = this.m_SkillAdd.color;
			color.a = (flag ? 0.5f : 1f);
			this.m_SkillAdd.color = color;
		}
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x0011A62C File Offset: 0x0011882C
	private void SetupTooltip()
	{
		if (this.def == null && this.m_Enabled)
		{
			Tooltip tooltip = Tooltip.Get(base.gameObject, true);
			tooltip.SetDef("SkillSlotTooltip", null);
			if (this.tooltipVars == null)
			{
				this.tooltipVars = new Vars();
			}
			this.tooltipVars.Clear();
			this.tooltipVars.obj = this.owner;
			Logic.Character character = this.owner as Logic.Character;
			if (character != null)
			{
				this.tooltipVars.Set<bool>("is_player_court_member", character.IsInCourt(BaseUI.LogicKingdom()));
			}
			tooltip.SetVars(this.tooltipVars);
			return;
		}
		if (this.def != null)
		{
			Tooltip tooltip2 = Tooltip.Get(base.gameObject, true);
			tooltip2.SetDef("SkillTooltip", null);
			if (this.tooltipVars == null)
			{
				this.tooltipVars = new Vars();
			}
			this.tooltipVars.Clear();
			this.FillTTVars();
			tooltip2.SetVars(this.tooltipVars);
			return;
		}
		Tooltip.Get(base.gameObject, true).Clear(true);
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x0011A734 File Offset: 0x00118934
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
		UIText.SetText(this.TMP_Caption, this.def.field, "name", null, null);
		if (this.Skill_Icon != null)
		{
			this.Skill_Icon.sprite = global::Defs.GetObj<Sprite>(0, this.def.dt_def.field, "icon", null);
		}
		if (this.Skill_Border != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(0, this.IconDef, "tier_border", null);
			this.Skill_Border.sprite = obj;
		}
		if (this.m_Selection != null && this.m_Selection.Length != 0)
		{
			for (int i = 0; i < this.m_Selection.Length; i++)
			{
				Sprite obj2 = global::Defs.GetObj<Sprite>(0, this.IconDef, "tier_selection", null);
				this.m_Selection[i].sprite = obj2;
			}
		}
		if (this.m_TraditionIcon != null)
		{
			this.m_TraditionIcon.gameObject.SetActive(this.owner != null && this.def != null && this.def.IsGrantedByTradition(this.owner.GetKingdom()));
		}
		this.SetupTooltip();
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x0011A890 File Offset: 0x00118A90
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
		if (this.skill == null)
		{
			return;
		}
		UIText.SetText(this.TMP_Caption, this.skill.def.field, "name", null, null);
		int idx = 0;
		if (this.owner != null)
		{
			idx = this.def.GetRank(this.owner) - 1;
		}
		if (this.Skill_Icon != null)
		{
			this.Skill_Icon.sprite = global::Defs.GetObj<Sprite>(idx, this.def.dt_def.field, "icon", null);
		}
		if (this.Skill_Border != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(idx, this.IconDef, "tier_border", null);
			this.Skill_Border.sprite = obj;
		}
		if (this.m_Selection != null && this.m_Selection.Length != 0)
		{
			for (int i = 0; i < this.m_Selection.Length; i++)
			{
				Sprite obj2 = global::Defs.GetObj<Sprite>(idx, this.IconDef, "tier_selection", null);
				this.m_Selection[i].sprite = obj2;
			}
		}
		if (this.m_TraditionIcon != null)
		{
			this.m_TraditionIcon.gameObject.SetActive(this.owner != null && this.skill != null && this.skill.def != null && this.skill.def.IsGrantedByTradition(this.owner.GetKingdom()));
		}
		this.SetupTooltip();
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x0011AA20 File Offset: 0x00118C20
	private void UpdateRank()
	{
		if (this.m_Borders != null && this.m_Borders.Length != 0)
		{
			Skill skill = this.skill;
			int num = (skill != null) ? skill.rank : 0;
			for (int i = 0; i < this.m_Borders.Length; i++)
			{
				this.m_Borders[i].overrideSprite = this.GetRankBorder(num);
			}
		}
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x0011AA78 File Offset: 0x00118C78
	private Sprite GetRankBorder(int rank)
	{
		if (this.m_RankBorders == null || this.m_RankBorders.Length == 0)
		{
			return null;
		}
		rank = Mathf.Clamp(rank, 0, this.m_RankBorders.Length - 1);
		return this.m_RankBorders[rank];
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x0011AAA8 File Offset: 0x00118CA8
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x0011AAC6 File Offset: 0x00118CC6
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x0011AAE4 File Offset: 0x00118CE4
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
		if (this.OnFocusGain != null)
		{
			this.OnFocusGain(this, eventData);
		}
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x0011AB08 File Offset: 0x00118D08
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
		if (this.OnFocusLoss != null)
		{
			this.OnFocusLoss(this, eventData);
		}
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x0011AB2C File Offset: 0x00118D2C
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_Selection != null)
		{
			for (int i = 0; i < this.m_Selection.Length; i++)
			{
				this.m_Selection[i].gameObject.SetActive(this.m_Selected || this.mouse_in);
			}
		}
		if (this.m_SkillAdd != null)
		{
			this.m_SkillAdd.gameObject.SetActive(this.mouse_in && this.m_ShowAdd);
		}
		if (this.m_NotEligable != null && this.m_NotEligable.Length != 0)
		{
			for (int j = 0; j < this.m_NotEligable.Length; j++)
			{
				this.m_NotEligable[j].gameObject.SetActive(!this.m_Eilgable);
			}
		}
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x0011ABED File Offset: 0x00118DED
	private void OnDestroy()
	{
		if (this.owner != null)
		{
			this.owner.DelListener(this);
		}
		this.OnFocusGain = null;
		this.OnFocusGain = null;
		this.OnSelect = null;
		this.OnReplace = null;
		this.SetOwner(null);
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x0011AC28 File Offset: 0x00118E28
	private bool[] CalcInsufficientResources()
	{
		bool[] array = this.tmp_insufficient_resources;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = false;
		}
		Logic.Character character;
		if ((character = (this.owner as Logic.Character)) == null)
		{
			return array;
		}
		Logic.Kingdom kingdom = character.GetKingdom();
		if (kingdom == null)
		{
			return array;
		}
		Resource resource;
		if (character.CanAddSkill(this.def))
		{
			resource = this.def.GetLearnCost(character);
		}
		else
		{
			if (!character.CanAddSkillRank(this.def))
			{
				return array;
			}
			resource = this.def.GetUpgardeCost(character);
		}
		if (resource == null)
		{
			return array;
		}
		for (int j = 0; j < resource.amounts.Length; j++)
		{
			float num = resource.amounts[j];
			if (num > 0f && kingdom.resources.amounts[j] < num)
			{
				array[j] = true;
			}
		}
		return array;
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x0011ACFC File Offset: 0x00118EFC
	private bool CheckInsufficientResourcesChanged()
	{
		bool[] array = this.CalcInsufficientResources();
		bool result = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != this.last_insufficient_resources[i])
			{
				this.last_insufficient_resources[i] = array[i];
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x0011AD3C File Offset: 0x00118F3C
	private void Update()
	{
		if (this.def == null)
		{
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		if (((tooltip != null) ? tooltip.instance : null) == null)
		{
			return;
		}
		bool flag = UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "advanced skill tooltips", true);
		bool flag2 = this.CheckInsufficientResourcesChanged();
		if (flag == this.last_alt && !flag2)
		{
			return;
		}
		this.last_alt = flag;
		this.SetupTooltip();
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x0011ADB1 File Offset: 0x00118FB1
	private void FillTTVars()
	{
		UISkill.FillTTVars(this.tooltipVars, this.def, this.owner, null, this.m_ShowCost);
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x0011ADD4 File Offset: 0x00118FD4
	public static void FillTTVars(Vars vars, Skill.Def def, Logic.Object target, string class_name, bool show_cost = true)
	{
		vars.Set<Skill.Def>("skill", def);
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "skill debug tooltip", true))
		{
			target = null;
		}
		vars.Set<Logic.Object>("target", target);
		Logic.Kingdom kingdom = (target != null) ? target.GetKingdom() : null;
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		Logic.Character character;
		if ((character = (target as Logic.Character)) != null)
		{
			if (class_name == null)
			{
				class_name = character.class_name;
			}
			int num = def.GetRank(character);
			vars.Set<int>("rank", num);
			vars.Set<int>("max_rank", def.max_rank);
			DT.Field val = global::Defs.FindTextField(string.Format("SkillTooltip.rank_texts.{0}", num));
			vars.Set<DT.Field>("rank_text", val);
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			bool val2 = false;
			bool val3 = false;
			Resource val4 = null;
			Resource val5 = null;
			if (kingdom != null && kingdom == kingdom2)
			{
				bool flag = !character.IsPrisoner();
				flag &= character.IsInCourt(BaseUI.LogicKingdom());
				flag &= !character.IsRebel();
				if (flag && character.CanAddSkill(def))
				{
					val2 = true;
					val4 = def.GetLearnCost(character);
				}
				else if (flag && character.CanAddSkillRank(def))
				{
					val3 = true;
					val5 = def.GetUpgardeCost(character);
				}
			}
			vars.Set<bool>("can_learn", val2);
			vars.Set<bool>("can_increase_rank", val3);
			if (show_cost)
			{
				vars.Set<Resource>("learn_cost", val4);
				vars.Set<Resource>("rank_cost", val5);
			}
		}
		vars.Set<string>("class_name", class_name);
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x0011AF4C File Offset: 0x0011914C
	public static Value SetupSkillDescr(UIHyperText.CallbackParams arg)
	{
		if (arg.ht == null || arg.e == null)
		{
			return Value.Unknown;
		}
		Skill.Def def = arg.ht.GetVar("skill", null, true).Get<Skill.Def>();
		string text = arg.ht.GetVar("class_name", null, true).String(null);
		if (def == null || string.IsNullOrEmpty(text))
		{
			return Value.Unknown;
		}
		Vars vars = arg.e.vars as Vars;
		if (vars == null)
		{
			vars = (arg.e.vars = new Vars(arg.e.vars));
		}
		string slotType = def.GetSlotType(text);
		DT.Field val = global::Defs.FindTextField("SkillTooltip.skill_descr." + slotType);
		vars.Set<DT.Field>("skill_descr", val);
		return true;
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x0011B020 File Offset: 0x00119220
	public static Value PopulateTooltipHT(UIHyperText.CallbackParams arg)
	{
		Skill.Def def = arg.ht.GetVar("skill", null, true).obj_val as Skill.Def;
		if (def == null)
		{
			return Value.Unknown;
		}
		Logic.Object target = arg.ht.GetVar("target", null, true).obj_val as Logic.Object;
		string class_name = arg.ht.GetVar("class_name", null, true).String(null);
		List<Vars> list = new List<Vars>();
		UISkill.FillTTSections(list, def, target, class_name);
		UISkill.AddClassLevelMod(list, def, target, class_name);
		return new Value(list);
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x0011B0A8 File Offset: 0x001192A8
	private static void FillTTSections(List<Vars> sections, Skill.Def def, Logic.Object target, string class_name)
	{
		SkillsTable.RowDef row_def = def.row_def;
		if (((row_def != null) ? row_def.cell_defs : null) == null)
		{
			return;
		}
		for (int i = 0; i < def.row_def.cell_defs.Count; i++)
		{
			SkillsTable.CellDef cdef = def.row_def.cell_defs[i];
			UISkill.AddTTCell(sections, cdef, target, class_name, null);
		}
		for (int j = 0; j < sections.Count; j++)
		{
			UISkill.SortEffects(sections[j]);
		}
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x0011B120 File Offset: 0x00119320
	public static Vars FindTTSection(List<Vars> sections, string section)
	{
		for (int i = 0; i < sections.Count; i++)
		{
			Vars vars = sections[i];
			if (vars.GetVar("section", null, true).String(null) == section)
			{
				return vars;
			}
		}
		return null;
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x0011B168 File Offset: 0x00119368
	private static string ResolveTTSection(SkillsTable.CellDef cdef, string class_name)
	{
		if (!(cdef.field.type == "at_rank"))
		{
			if (cdef.GetTraditionDef() == null)
			{
				if (class_name != null && cdef.class_def != null)
				{
					return "Default";
				}
				if (cdef.name == "Govern")
				{
					if (cdef.parent != null)
					{
						return UISkill.ResolveTTSection(cdef.parent, class_name);
					}
					return "Default";
				}
			}
			else if (cdef.parent != null)
			{
				string text = UISkill.ResolveTTSection(cdef.parent, class_name);
				if (text != "Default")
				{
					return text;
				}
			}
			return cdef.name;
		}
		if (cdef.parent == null)
		{
			return "Default";
		}
		return UISkill.ResolveTTSection(cdef.parent, class_name);
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x0011B218 File Offset: 0x00119418
	private static bool ValidateSection(SkillsTable.CellDef cdef, string section, Logic.Object target)
	{
		if (section == "Default")
		{
			return true;
		}
		if (target == null)
		{
			return true;
		}
		Logic.Character character;
		if ((character = (target as Logic.Character)) == null)
		{
			return true;
		}
		Tradition.Def traditionDef = cdef.GetTraditionDef();
		if (traditionDef != null)
		{
			Logic.Kingdom kingdom = character.GetKingdom();
			return kingdom != null && kingdom.HasTradition(traditionDef);
		}
		DT.Field field = cdef.field;
		string a = (field != null) ? field.Type() : null;
		if (!(a == "tag"))
		{
			return a == "class" && character.class_name == cdef.name;
		}
		return (cdef.name == "Govern" && UISkill.tmp_governed_castle != null) || character.HasTag(cdef.name, true);
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x0011B2D4 File Offset: 0x001194D4
	public static void FillSectionVars(Vars svars, SkillsTable.CellDef cdef, string section, Logic.Object target)
	{
		svars.Set<string>("section", section);
		bool flag = UISkill.ValidateSection(cdef, section, target);
		if (!flag)
		{
			svars.Set<bool>("inactive", true);
		}
		Skill.Def def = GameLogic.Get(true).defs.Find<Skill.Def>(section);
		Tradition.Def traditionDef = cdef.GetTraditionDef();
		if (traditionDef != null)
		{
			svars.Set<Tradition.Def>("tradition", traditionDef);
			int skillRank = cdef.GetSkillRank(target);
			int skillMaxRank = cdef.GetSkillMaxRank(target);
			if (flag && skillRank > 0 && skillRank < skillMaxRank)
			{
				svars.Set<bool>("tradition_needs_rank", true);
			}
		}
		Sprite obj = global::Defs.GetObj<Sprite>("SkillTooltip", "tag_icons." + section, null);
		if (obj == null)
		{
			if (def != null)
			{
				obj = global::Defs.GetObj<Sprite>(def.field, "icon", null);
			}
			else
			{
				CharacterClass.Def classDef = cdef.GetClassDef();
				obj = global::Defs.GetObj<Sprite>((classDef != null) ? classDef.field : null, "icon", null);
			}
		}
		svars.Set<Sprite>("icon", obj);
		if (traditionDef != null)
		{
			svars.Set<string>("caption", "SkillTooltip.captions.Tradition");
			return;
		}
		if (def != null)
		{
			svars.Set<string>("caption", "SkillTooltip.captions.Skill");
			svars.Set<Skill.Def>("skill", def);
			int num = def.GetRank(target);
			if (num > 0)
			{
				svars.Set<string>("rank_text", string.Format("SkillTooltip.rank_texts.{0}", num));
				return;
			}
		}
		else if (cdef.class_def != null)
		{
			if (!(target is Logic.Character))
			{
				svars.Set<string>("class_name", cdef.class_def.name);
				SkillsTable.RowDef row_def = cdef.row_def;
				string str = (row_def != null) ? row_def.skill_def.GetSlotType(cdef.class_def) : null;
				svars.Set<string>("caption", "SkillTooltip.skill_descr." + str);
				return;
			}
			if (section == cdef.class_def.id)
			{
				svars.Set<string>("class_name", cdef.class_def.name);
				svars.Set<string>("caption", "SkillTooltip.captions.Class");
				return;
			}
		}
		else
		{
			DT.Field val = global::Defs.FindTextField("SkillTooltip.captions." + section);
			svars.Set<Value>("caption", new Value(val));
		}
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x0011B4DC File Offset: 0x001196DC
	public static Vars AddTTCell(List<Vars> sections, SkillsTable.CellDef cdef, Logic.Object target, string class_name, string section = null)
	{
		Logic.Character character;
		if (class_name == null && (character = (target as Logic.Character)) != null)
		{
			CharacterClass.Def class_def = character.class_def;
			class_name = ((class_def != null) ? class_def.name : null);
		}
		CharacterClass.Def classDef = cdef.GetClassDef();
		if (classDef != null && class_name != null && classDef.name != class_name)
		{
			return null;
		}
		if (section == null)
		{
			section = UISkill.ResolveTTSection(cdef, class_name);
		}
		Vars vars = UISkill.FindTTSection(sections, section);
		if (vars == null)
		{
			vars = new Vars();
			UISkill.FillSectionVars(vars, cdef, section, target);
			sections.Add(vars);
		}
		bool valid = target == null || cdef.Validate(target, true, false, UISkill.tmp_governed_castle != null);
		UISkill.AddEffects(vars, cdef, target, valid);
		if (cdef.subcells != null)
		{
			for (int i = 0; i < cdef.subcells.Count; i++)
			{
				SkillsTable.CellDef cdef2 = cdef.subcells[i];
				UISkill.AddTTCell(sections, cdef2, target, class_name, null);
			}
		}
		return vars;
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x0011B5B7 File Offset: 0x001197B7
	public static bool AddEffects(Vars svars, SkillsTable.CellDef cdef, Logic.Object target, bool valid)
	{
		return UISkill.AddMods(svars, cdef, target, valid) | UISkill.AddBattleTactics(svars, cdef, target, valid) | UISkill.AddActions(svars, cdef, target, valid);
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x0011B5D8 File Offset: 0x001197D8
	private static int CompareInt(Vars a, Vars b, string key, int def_val = 0)
	{
		int num = a.GetVar(key, null, true).Int(def_val);
		int value = b.GetVar(key, null, true).Int(def_val);
		return num.CompareTo(value);
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x0011B614 File Offset: 0x00119814
	public static void SortEffects(Vars svars)
	{
		List<Vars> list = svars.GetVar("effects", null, true).obj_val as List<Vars>;
		if (list == null)
		{
			return;
		}
		list.Sort(delegate(Vars a, Vars b)
		{
			int num = UISkill.CompareInt(a, b, "min_rank", 1);
			if (num != 0)
			{
				return num;
			}
			int num2 = UISkill.CompareInt(a, b, "effect_priority", 0);
			if (num2 != 0)
			{
				return num2;
			}
			return 0;
		});
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x0011B664 File Offset: 0x00119864
	private static void AddClassLevelMod(List<Vars> sections, Skill.Def def, Logic.Object target, string class_name)
	{
		Logic.Character character = target as Logic.Character;
		if (class_name == null)
		{
			string text;
			if (character == null)
			{
				text = null;
			}
			else
			{
				CharacterClass.Def class_def = character.class_def;
				text = ((class_def != null) ? class_def.name : null);
			}
			class_name = text;
		}
		int num = 0;
		if (character != null)
		{
			num = def.GetRank(character);
		}
		if (num == 0 && !def.IsApplicableTo(class_name))
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<bool>("inactive", false);
		vars.Set<string>("value", UISkill.GetClassLevelValueText(def, num));
		vars.Set<string>("name", "SkillTooltip.class_level_name");
		vars.Set<string>("effect_text", "Stats.mod_text");
		vars.Set<string>("effect_type", "mod");
		vars.Set<int>("effect_priority", 0);
		Vars vars2 = UISkill.FindTTSection(sections, "Default");
		if (vars2 == null)
		{
			vars2 = new Vars();
			sections.Insert(0, vars2);
		}
		List<Vars> list = vars2.GetVar("effects", null, true).obj_val as List<Vars>;
		if (list == null)
		{
			list = new List<Vars>();
			vars2.Set<Value>("effects", new Value(list));
		}
		if (list.Count > 0)
		{
			list.Insert(0, vars);
			return;
		}
		list.Add(vars);
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x0011B780 File Offset: 0x00119980
	private static string GetClassLevelValueText(Skill.Def def, int rank)
	{
		string text = "@";
		text += ((rank == 0 || rank == 1) ? "{clr:active_buff}" : "{clr:inactive_buff}");
		text += "+{/clr}";
		for (int i = 1; i <= def.max_rank; i++)
		{
			if (i > 1)
			{
				text += "{SkillTooltip.value_separator}";
			}
			if (rank == 0 || rank == i)
			{
				text += "{clr:active_buff}";
			}
			else
			{
				text += "{clr:inactive_buff}";
			}
			text += i;
			text += "{/clr}";
		}
		return text;
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x0011B818 File Offset: 0x00119A18
	private static bool AddMods(Vars svars, SkillsTable.CellDef cdef, Logic.Object target, bool valid)
	{
		if (cdef.mod_defs == null || cdef.mod_defs.Count == 0)
		{
			return false;
		}
		bool result = false;
		List<Vars> list = svars.GetVar("effects", null, true).obj_val as List<Vars>;
		for (int i = 0; i < cdef.mod_defs.Count; i++)
		{
			Skill.StatModifier.Def mdef = cdef.mod_defs[i];
			Vars vars = UISkill.CreateModVars(svars, mdef, target, valid);
			if (vars != null)
			{
				if (list == null)
				{
					list = new List<Vars>();
					svars.Set<Value>("effects", new Value(list));
				}
				list.Add(vars);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x0011B8B0 File Offset: 0x00119AB0
	public static void SetValueVars(Vars vars, DT.Field mod_field, Logic.Object target, DT.Field stat_field, int rank, int min_rank, bool tradition)
	{
		int num = mod_field.NumValues();
		float num2 = mod_field.Float(0, target, 0f);
		bool flag = num2 < 0f;
		bool flag2 = num == 1 && num2 == 0f;
		bool @bool = stat_field.GetBool("penalty", null, false, true, true, true, '.');
		string text = "active";
		string text2 = "inactive";
		if (tradition)
		{
			text += "_tradition";
			text2 += "_tradition";
		}
		if (flag == @bool)
		{
			text += "_buff";
			text2 += "_buff";
		}
		else
		{
			text += "_penalty";
			text2 += "_penalty";
		}
		string str;
		if (flag)
		{
			str = "-";
		}
		else if (stat_field.GetBool("show_plus_sign", null, true, true, true, true, '.'))
		{
			str = "+";
		}
		else
		{
			str = "";
		}
		string text3 = "@";
		for (int i = 0; i < num; i++)
		{
			if (i > 0)
			{
				text3 += "{SkillTooltip.value_separator}";
			}
			string str2 = (rank == 0 || rank == i + min_rank || num == 1) ? text : text2;
			text3 = text3 + "{clr:" + str2 + "}";
			if (i == 0)
			{
				text3 += str;
			}
			float num3 = mod_field.Float(i, target, 0f);
			if (num3 < 0f)
			{
				num3 = -num3;
			}
			text3 += DT.FloatToStr(num3, 3);
			if (i == num - 1 && (mod_field.FindChild("perc", null, true, true, true, '.') != null || stat_field.key.EndsWith("_perc", StringComparison.Ordinal)))
			{
				text3 += "%";
			}
			text3 += "{/clr}";
		}
		vars.Set<string>("value", text3);
		if (flag2)
		{
			vars.Set<bool>("inactive", true);
		}
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x0011BA9C File Offset: 0x00119C9C
	public static Value GetModText(DT.Field mdef_field, Logic.Object target, DT.Field stat_field)
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

	// Token: 0x06001E8C RID: 7820 RVA: 0x0011BB24 File Offset: 0x00119D24
	private static Vars CreateModVars(Vars svars, Skill.StatModifier.Def mdef, Logic.Object target, bool valid)
	{
		Castle castle;
		if ((castle = UISkill.tmp_governed_castle) == null)
		{
			Logic.Character character = target as Logic.Character;
			castle = ((character != null) ? character.GovernTarget() : null);
		}
		Castle castle2 = castle;
		DT.Field field = null;
		if (mdef.condition != null)
		{
			UISkill.tmp_vars.Clear();
			UISkill.tmp_vars.obj = target;
			UISkill.tmp_vars.Set<Castle>("governed_castle", castle2);
			bool flag = mdef.condition.Bool(UISkill.tmp_vars, false);
			if (!flag)
			{
				valid = false;
			}
			field = global::Defs.FindTextField(mdef.field, "condition_text");
			if (field == null && !flag)
			{
				return null;
			}
		}
		string text = mdef.TargetStatsType();
		DT.Field defField = global::Defs.GetDefField(text, mdef.stat_name);
		Value modText = UISkill.GetModText(mdef.field, target, defField);
		if (modText.is_null)
		{
			return null;
		}
		Vars vars = new Vars(svars);
		if (!valid)
		{
			vars.Set<bool>("inactive", true);
		}
		Tradition.Def traditionDef = mdef.cell_def.GetTraditionDef();
		if (traditionDef != null)
		{
			vars.Set<bool>("from_tradition", true);
		}
		int num = mdef.GetRank(target, true);
		int min_rank = mdef.min_rank;
		vars.Set<Skill.StatModifier.Def>("mod_def", mdef);
		vars.Set<DT.Field>("stat_def", defField);
		vars.Set<Logic.Object>("owner", target);
		UISkill.SetValueVars(vars, mdef.field, target, defField, num, min_rank, traditionDef != null);
		vars.Set<string>("name", text + "." + mdef.stat_name + ".name");
		vars.Set<Castle>("governed_castle", castle2);
		Logic.Realm realm = (castle2 != null) ? castle2.GetRealm() : null;
		vars.Set<Logic.Realm>("realm", realm);
		Stats.Def def = Def.Get<Stats.Def>((defField != null) ? defField.parent : null);
		Stat.Def def2 = (def != null) ? def.FindStat(mdef.stat_name, false) : null;
		float num2;
		if (def2 != null)
		{
			num2 = def2.CalcMultiplier(realm, target);
		}
		else
		{
			num2 = -1f;
		}
		if (num2 >= 0f)
		{
			vars.Set<float>("multiplier", num2);
			if (num2 == 0f)
			{
				vars.Set<bool>("inactive", true);
			}
		}
		else
		{
			vars.Set<Value>("multiplier", Value.Null);
		}
		vars.Set<DT.Field>("condition_text", field);
		vars.Set<Value>("effect_text", modText);
		vars.Set<string>("effect_type", "mod");
		vars.Set<int>("effect_priority", 1);
		if (mdef.cell_def.min_rank > 1)
		{
			vars.Set<int>("min_rank", mdef.cell_def.min_rank);
		}
		return vars;
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x0011BD9C File Offset: 0x00119F9C
	private static bool AddBattleTactics(Vars svars, SkillsTable.CellDef cdef, Logic.Object target, bool valid)
	{
		if (cdef.tactic_defs == null || cdef.tactic_defs.Count == 0)
		{
			return true;
		}
		bool result = false;
		List<Vars> list = svars.GetVar("effects", null, true).obj_val as List<Vars>;
		for (int i = 0; i < cdef.tactic_defs.Count; i++)
		{
			BattleTactic.Def btdef = cdef.tactic_defs[i];
			Vars vars = UISkill.CreateBattleTacticsVars(svars, btdef, cdef, target, valid);
			if (vars != null)
			{
				if (list == null)
				{
					list = new List<Vars>();
					svars.Set<Value>("effects", new Value(list));
				}
				list.Add(vars);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x0011BE34 File Offset: 0x0011A034
	private static Vars CreateBattleTacticsVars(Vars svars, BattleTactic.Def btdef, SkillsTable.CellDef cdef, Logic.Object target, bool valid)
	{
		Vars vars = new Vars(svars);
		if (!valid)
		{
			vars.Set<bool>("inactive", true);
		}
		if (cdef.GetTraditionDef() != null)
		{
			vars.Set<bool>("from_tradition", true);
		}
		vars.Set<string>("name", btdef.id + ".name");
		vars.Set<string>("effect_text", "SkillTooltip.tactic_text");
		vars.Set<string>("effect_type", "tactic");
		vars.Set<int>("effect_priority", 2);
		if (cdef.min_rank > 1)
		{
			vars.Set<int>("min_rank", cdef.min_rank);
		}
		return vars;
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x0011BED4 File Offset: 0x0011A0D4
	private static bool AddActions(Vars svars, SkillsTable.CellDef cdef, Logic.Object target, bool valid)
	{
		if (cdef.action_defs == null || cdef.action_defs.Count == 0)
		{
			return false;
		}
		bool result = false;
		List<Vars> list = svars.GetVar("effects", null, true).obj_val as List<Vars>;
		for (int i = 0; i < cdef.action_defs.Count; i++)
		{
			SkillsTable.ActionDef adef = cdef.action_defs[i];
			Vars vars = UISkill.CreateActionVars(svars, adef, target, valid);
			if (vars != null)
			{
				if (list == null)
				{
					list = new List<Vars>();
					svars.Set<Value>("effects", new Value(list));
				}
				list.Add(vars);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x0011BF6C File Offset: 0x0011A16C
	private static Vars CreateActionVars(Vars svars, SkillsTable.ActionDef adef, Logic.Object target, bool valid)
	{
		Vars vars = new Vars(svars);
		if (!valid)
		{
			vars.Set<bool>("inactive", true);
		}
		if (adef.cell_def.GetTraditionDef() != null)
		{
			vars.Set<bool>("from_tradition", true);
		}
		vars.Set<string>("name", adef.name + ".name");
		vars.Set<string>("action_type", "SkillTooltip.action_types." + adef.field.Type());
		vars.Set<string>("effect_text", "SkillTooltip.action_text");
		vars.Set<string>("effect_type", "action");
		vars.Set<int>("effect_priority", 4);
		if (adef.cell_def.min_rank > 1)
		{
			vars.Set<int>("min_rank", adef.cell_def.min_rank);
		}
		return vars;
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x0011C03C File Offset: 0x0011A23C
	public static Value PopulateGovernorTooltipHT(UIHyperText.CallbackParams arg)
	{
		Logic.Character character = arg.ht.GetVar("governor", null, true).obj_val as Logic.Character;
		if (character == null)
		{
			character = (arg.ht.GetVar("prepairng_governor", null, true).obj_val as Logic.Character);
		}
		if (character == null)
		{
			return Value.Unknown;
		}
		UISkill.tmp_governed_castle = (arg.ht.GetVar("castle", null, true).obj_val as Castle);
		if (character != null && arg.ht != null)
		{
			Vars vars = arg.ht.vars as Vars;
			if (vars == null)
			{
				vars = (arg.ht.vars = new Vars(arg.ht.vars));
			}
			vars.Set<Logic.Character>("target", character);
			vars.Set<string>("class_name", character.class_name);
		}
		List<Vars> list = new List<Vars>();
		SkillsTable.RowDef rowDef = SkillsTable.FindGovernRowDef(character.game);
		if (rowDef != null)
		{
			UISkill.AddGovernorTTCell(list, character, rowDef, "Default", null);
			UISkill.AddGovernorTTCell(list, character, rowDef, character.class_name, null);
			if (character.IsKing())
			{
				UISkill.AddGovernorTTCell(list, character, rowDef, "King", null);
			}
			if (character.IsCardinal())
			{
				UISkill.AddGovernorTTCell(list, character, rowDef, "Cardinal", null);
			}
			if (character.IsPatriarch())
			{
				UISkill.AddGovernorTTCell(list, character, rowDef, "Patriarch", null);
			}
		}
		UISkill.AddGovernorSkillCells(list, character);
		UISkill.tmp_governed_castle = null;
		return new Value(list);
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x0011C19C File Offset: 0x0011A39C
	private static void AddGovernorSkillCells(List<Vars> sections, Logic.Character governor)
	{
		if (governor.skills == null)
		{
			return;
		}
		for (int i = 0; i < governor.skills.Count; i++)
		{
			Skill skill = governor.skills[i];
			bool flag;
			if (skill == null)
			{
				flag = (null != null);
			}
			else
			{
				Skill.Def def = skill.def;
				flag = (((def != null) ? def.row_def : null) != null);
			}
			if (flag)
			{
				UISkill.AddGovernorTTCell(sections, governor, skill.def.row_def, "Govern", skill.def.id);
				SkillsTable.CellDef cellDef = skill.def.row_def.FindCell(governor.class_name);
				if (((cellDef != null) ? cellDef.subcells : null) != null)
				{
					for (int j = 0; j < cellDef.subcells.Count; j++)
					{
						SkillsTable.CellDef cellDef2 = cellDef.subcells[j];
						if (cellDef2.name == "Govern")
						{
							UISkill.AddTTCell(sections, cellDef2, governor, governor.class_name, skill.def.id);
						}
						else if (cellDef2.tradition_def != null)
						{
							SkillsTable.CellDef cellDef3 = cellDef2.FindSubCell("Govern");
							if (cellDef3 != null)
							{
								UISkill.AddTTCell(sections, cellDef3, governor, governor.class_name, null);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x0011C2C4 File Offset: 0x0011A4C4
	private static Vars AddGovernorTTCell(List<Vars> sections, Logic.Character governor, SkillsTable.RowDef row_def, string cell_name, string section = null)
	{
		SkillsTable.CellDef cellDef = (row_def != null) ? row_def.FindCell(cell_name) : null;
		if (cellDef == null)
		{
			return null;
		}
		return UISkill.AddTTCell(sections, cellDef, governor, governor.class_name, section ?? cell_name);
	}

	// Token: 0x040013DC RID: 5084
	[UIFieldTarget("id_SkilIcon")]
	private Image Skill_Icon;

	// Token: 0x040013DD RID: 5085
	[UIFieldTarget("id_SkillBorder")]
	private Image Skill_Border;

	// Token: 0x040013DE RID: 5086
	[UIFieldTarget("id_IconType")]
	private Image m_IconType;

	// Token: 0x040013DF RID: 5087
	[UIFieldTarget("id_SkilName")]
	private TextMeshProUGUI TMP_Caption;

	// Token: 0x040013E0 RID: 5088
	[UIFieldTarget("id_Group_Empty")]
	private GameObject m_GroupEmpty;

	// Token: 0x040013E1 RID: 5089
	[UIFieldTarget("id_Group_Populated")]
	private GameObject m_GroupPopulated;

	// Token: 0x040013E2 RID: 5090
	[UIFieldTarget("id_Button_Add")]
	private Image m_SkillAdd;

	// Token: 0x040013E3 RID: 5091
	[UIFieldTarget("id_Button_Locked")]
	private GameObject m_SkillLocked;

	// Token: 0x040013E4 RID: 5092
	[UIFieldTarget("id_Replace")]
	private BSGButton m_ReplaceSkill;

	// Token: 0x040013E5 RID: 5093
	[UIFieldTarget("id_OverrideIcon")]
	private Image[] m_OverrideIcon;

	// Token: 0x040013E6 RID: 5094
	[UIFieldTarget("id_SkillPointCount")]
	private TextMeshProUGUI m_SkillPointCount;

	// Token: 0x040013E7 RID: 5095
	[UIFieldTarget("id_TraditionIcon")]
	private GameObject m_TraditionIcon;

	// Token: 0x040013E8 RID: 5096
	[UIFieldTarget("id_Image_Border")]
	private Image[] m_Borders;

	// Token: 0x040013E9 RID: 5097
	[UIFieldTarget("id_Selection")]
	private Image[] m_Selection;

	// Token: 0x040013EA RID: 5098
	[UIFieldTarget("id_Highlight")]
	private GameObject[] m_Highlight;

	// Token: 0x040013EB RID: 5099
	[UIFieldTarget("id_NotEligable")]
	private GameObject[] m_NotEligable;

	// Token: 0x040013F0 RID: 5104
	[HideInInspector]
	public Logic.Object owner;

	// Token: 0x040013F1 RID: 5105
	[HideInInspector]
	public Skill.Def def;

	// Token: 0x040013F2 RID: 5106
	[HideInInspector]
	public Skill skill;

	// Token: 0x040013F3 RID: 5107
	private int rank;

	// Token: 0x040013F5 RID: 5109
	private bool m_Enabled;

	// Token: 0x040013F6 RID: 5110
	private bool m_ShowReplace;

	// Token: 0x040013F7 RID: 5111
	private bool m_ShowAdd;

	// Token: 0x040013F8 RID: 5112
	private bool m_Selected;

	// Token: 0x040013F9 RID: 5113
	private bool m_ShowCost = true;

	// Token: 0x040013FA RID: 5114
	private Sprite m_OverrideSprite;

	// Token: 0x040013FB RID: 5115
	private bool m_Initialzied;

	// Token: 0x040013FC RID: 5116
	private bool m_Eilgable = true;

	// Token: 0x040013FD RID: 5117
	private DT.Field IconDef;

	// Token: 0x040013FE RID: 5118
	[SerializeField]
	private Sprite[] m_RankBorders;

	// Token: 0x040013FF RID: 5119
	private Vars tooltipVars;

	// Token: 0x04001400 RID: 5120
	private bool last_alt;

	// Token: 0x04001401 RID: 5121
	private bool[] last_insufficient_resources = new bool[13];

	// Token: 0x04001402 RID: 5122
	private bool[] tmp_insufficient_resources = new bool[13];

	// Token: 0x04001403 RID: 5123
	private static Vars tmp_vars = new Vars();

	// Token: 0x04001404 RID: 5124
	private static Castle tmp_governed_castle = null;
}
