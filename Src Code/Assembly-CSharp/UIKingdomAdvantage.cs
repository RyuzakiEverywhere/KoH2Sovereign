using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000223 RID: 547
public class UIKingdomAdvantage : MonoBehaviour
{
	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06002127 RID: 8487 RVA: 0x0012D1B2 File Offset: 0x0012B3B2
	// (set) Token: 0x06002128 RID: 8488 RVA: 0x0012D1BA File Offset: 0x0012B3BA
	public UIKingdomAdvantage.State state { get; private set; }

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06002129 RID: 8489 RVA: 0x0012D1C3 File Offset: 0x0012B3C3
	// (set) Token: 0x0600212A RID: 8490 RVA: 0x0012D1CB File Offset: 0x0012B3CB
	public DT.Field state_def { get; private set; }

	// Token: 0x0600212B RID: 8491 RVA: 0x0012D1D4 File Offset: 0x0012B3D4
	public void SetObject(KingdomAdvantage adv)
	{
		this.Data = adv;
		this.Init();
		this.InitRequirements();
		this.m_Invalidate = true;
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x0012D1F0 File Offset: 0x0012B3F0
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.ui_def = global::Defs.GetDefField("AdvantageSlot", null);
		if (!this.m_DisableTooltip)
		{
			Tooltip.Get(this.m_TooltipHotSpot.gameObject ?? base.gameObject, true).SetDef("KingdomAdvantageTooltip", new Vars(this.Data));
		}
		if (this.m_TooltipHotSpot != null)
		{
			this.m_TooltipHotSpot.onEvent = new BSGButton.OnEvent(this.OnEvent);
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = global::Defs.GetObj<Sprite>(this.Data.def.field, "illustration", null);
		}
		if (this.m_AdvantageName != null)
		{
			UIText.SetTextKey(this.m_AdvantageName, "KingdomAdvantages.advantageName", this.Data.def, null);
		}
		this.m_Initialized = true;
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x0012D2E8 File Offset: 0x0012B4E8
	private void InitRequirements()
	{
		this.m_Requirements.Clear();
		if (this.m_RequirementsContainer == null)
		{
			return;
		}
		int i = this.m_RequirementsContainer.transform.childCount;
		if (this.m_RequirementPrototype == null)
		{
			if (i < 1)
			{
				return;
			}
			this.m_RequirementPrototype = this.m_RequirementsContainer.transform.GetChild(0).gameObject;
		}
		this.m_RequirementPrototype.SetActive(false);
		while (i > 1)
		{
			global::Common.DestroyObj(this.m_RequirementsContainer.transform.GetChild(--i).gameObject);
		}
		List<KingdomAdvantage.RequirementInfo> soft_requires = this.Data.def.soft_requires;
		if (soft_requires == null)
		{
			return;
		}
		for (int j = 0; j < soft_requires.Count; j++)
		{
			KingdomAdvantage.RequirementInfo requirementInfo = soft_requires[j];
			GameObject gameObject = global::Common.Spawn(this.m_RequirementPrototype, false, false);
			gameObject.transform.SetParent(this.m_RequirementsContainer.transform, false);
			gameObject.SetActive(true);
			UIKingdomAdvantagesWindow.GoodsSlot orAddComponent = gameObject.GetOrAddComponent<UIKingdomAdvantagesWindow.GoodsSlot>();
			orAddComponent.SetDef(requirementInfo.def.def, this.Data.kingdom);
			this.m_Requirements.Add(orAddComponent);
		}
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x0012D40A File Offset: 0x0012B60A
	private void OnEvent(BSGButton btn, BSGButton.Event e, PointerEventData eventData)
	{
		if (e == BSGButton.Event.Enter)
		{
			this.m_Focused.SetActive(true);
			return;
		}
		if (e != BSGButton.Event.Leave)
		{
			return;
		}
		this.m_Focused.SetActive(false);
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x0012D42D File Offset: 0x0012B62D
	public void Invalidate()
	{
		this.m_Initialized = true;
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x0012D436 File Offset: 0x0012B636
	private void Update()
	{
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Initialized = false;
		}
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x0012D44D File Offset: 0x0012B64D
	private void Refresh()
	{
		this.UpdateState();
		this.RefreshRequirements();
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x0012D45C File Offset: 0x0012B65C
	private void RefreshRequirements()
	{
		if (this.m_Requirements == null)
		{
			return;
		}
		for (int i = 0; i < this.m_Requirements.Count; i++)
		{
			this.m_Requirements[i].UpdateState();
		}
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x0012D49C File Offset: 0x0012B69C
	public void SetState(UIKingdomAdvantage.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.ui_def == null)
		{
			this.ui_def = global::Defs.GetDefField("AdvantageSlot", null);
		}
		if (this.ui_def != null)
		{
			this.state_def = this.ui_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.ui_def.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.state_def = null;
		}
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x0012D549 File Offset: 0x0012B749
	public UIKingdomAdvantage.State DecideState()
	{
		if (this.Data == null)
		{
			return UIKingdomAdvantage.State.Default;
		}
		if (this.Data.CheckRequirements())
		{
			return UIKingdomAdvantage.State.Completed;
		}
		return UIKingdomAdvantage.State.Default;
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x0012D568 File Offset: 0x0012B768
	public void UpdateState()
	{
		UIKingdomAdvantage.State state = this.DecideState();
		this.SetState(state);
		this.UpdateVisualState();
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x0012D58C File Offset: 0x0012B78C
	private void UpdateVisualState()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.color = global::Defs.GetColor(this.state_def, "illustration_color", null);
		}
		if (this.m_ActiveAdvantage != null)
		{
			this.m_ActiveAdvantage.gameObject.SetActive(this.state_def.GetBool("show_activated_border", null, false, true, true, true, '.'));
		}
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x0012D604 File Offset: 0x0012B804
	public static string GetModText(Logic.Kingdom k, KingdomAdvantage.StatModifier.Def mod_def, bool no_null = false)
	{
		float num = mod_def.CalcValue(k);
		if (num == 0f && !no_null)
		{
			return null;
		}
		return global::Defs.LocalizeStatModifier((mod_def.field.type == "instant") ? "InstantBonuses" : "KingdomStats", mod_def.stat_name, num, mod_def.type, k, true, true);
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x0012D660 File Offset: 0x0012B860
	public static string GetAdvantagesBonusesText(KingdomAdvantage.Def def, Logic.Kingdom kingdom, string new_line = "\n")
	{
		string text = "";
		if (def == null)
		{
			return text;
		}
		for (int i = 0; i < def.mods.Count; i++)
		{
			KingdomAdvantage.StatModifier.Def mod_def = def.mods[i];
			string modText = UIKingdomAdvantage.GetModText(kingdom, mod_def, false);
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

	// Token: 0x06002139 RID: 8505 RVA: 0x0012D6CC File Offset: 0x0012B8CC
	public static Value SetupHTAdvantage(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		if (((ht != null) ? ht.vars : null) == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht2 = arg.ht;
		Vars vars = ((ht2 != null) ? ht2.vars : null) as Vars;
		KingdomAdvantage.Def def = null;
		Logic.Kingdom kingdom = vars.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		KingdomAdvantage kingdomAdvantage;
		KingdomAdvantage.Def def2;
		string def_id;
		if ((kingdomAdvantage = (vars.obj.obj_val as KingdomAdvantage)) != null)
		{
			def = kingdomAdvantage.def;
			kingdom = kingdomAdvantage.kingdom;
		}
		else if ((def2 = (vars.GetVar("obj", null, true).obj_val as KingdomAdvantage.Def)) != null)
		{
			def = def2;
			if (kingdom != null)
			{
				kingdom.advantages.FindById(def2.id);
			}
		}
		else if ((def_id = (vars.GetVar("obj", null, true).obj_val as string)) != null)
		{
			DT.Field defField = global::Defs.GetDefField(def_id, null);
			object obj;
			if (defField == null)
			{
				obj = null;
			}
			else
			{
				DT.Def def3 = defField.def;
				obj = ((def3 != null) ? def3.def : null);
			}
			def = (obj as KingdomAdvantage.Def);
			if (kingdom != null)
			{
				kingdom.advantages.FindById(def_id);
			}
		}
		Vars vars2 = vars;
		vars2.Set<string>("bonuses_text", "#" + UIKingdomAdvantage.GetAdvantagesBonusesText(def, kingdom, "\n"));
		UIKingdomAdvantage.FillRequirements(vars2, (def != null) ? def.soft_requires : null, "soft_requirements", kingdom);
		UIKingdomAdvantage.FillRequirements(vars2, (def != null) ? def.hard_requires : null, "hard_requirements", kingdom);
		UIKingdomAdvantage.FillRequirements(vars2, (def != null) ? def.hard_requires_or : null, "hard_requirements_or", kingdom);
		return true;
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x0012D844 File Offset: 0x0012BA44
	private static void FillRequirements(Vars vars, List<KingdomAdvantage.RequirementInfo> reqs, string name, Logic.Kingdom kingdom)
	{
		if (reqs == null)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < reqs.Count; i++)
		{
			KingdomAdvantage.RequirementInfo requirementInfo = reqs[i];
			string text2 = null;
			string text3 = null;
			if (requirementInfo.type != "Religion")
			{
				if (kingdom != null)
				{
					ResourceInfo resourceInfo = kingdom.GetResourceInfo(requirementInfo.key, true, true);
					if (resourceInfo != null)
					{
						resourceInfo.GetColorTags(out text2, out text3);
					}
				}
			}
			else
			{
				Religion religion = (kingdom != null) ? kingdom.religion : null;
				if (religion != null)
				{
					if (religion.HasTag(requirementInfo.key))
					{
						text2 = "requirement_met";
					}
					else
					{
						text2 = "requirement_not_met";
					}
					text3 = "/" + text2;
				}
			}
			DT.Field defField = global::Defs.GetDefField(requirementInfo.field.key, null);
			if (i > 0)
			{
				if (i == reqs.Count - 1)
				{
					text += "{list_final_separator}";
				}
				else
				{
					text += "{list_separator}";
				}
			}
			if (defField != null)
			{
				text = text + "{" + requirementInfo.field.key + ":link}";
			}
			if (text2 != null)
			{
				text = text + "{" + text2 + "}";
			}
			text = text + "{" + requirementInfo.field.key + ".name}";
			if (requirementInfo.amount > 1)
			{
				text = text + " x" + requirementInfo.amount;
			}
			if (text3 != null)
			{
				text = text + "{" + text3 + "}";
			}
			if (defField != null)
			{
				text = text + "{" + requirementInfo.field.key + ":/link}";
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			vars.Set<string>(name, "@" + text);
		}
	}

	// Token: 0x0400162A RID: 5674
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x0400162B RID: 5675
	[UIFieldTarget("id_TooltipHotSpot")]
	private BSGButton m_TooltipHotSpot;

	// Token: 0x0400162C RID: 5676
	[UIFieldTarget("id_Focused")]
	private GameObject m_Focused;

	// Token: 0x0400162D RID: 5677
	[UIFieldTarget("id_ActiveAdvantage")]
	private GameObject m_ActiveAdvantage;

	// Token: 0x0400162E RID: 5678
	[UIFieldTarget("id_AdvantageName")]
	private TextMeshProUGUI m_AdvantageName;

	// Token: 0x0400162F RID: 5679
	[UIFieldTarget("id_RequirementsContainer")]
	private GameObject m_RequirementsContainer;

	// Token: 0x04001630 RID: 5680
	[UIFieldTarget("id_RequirementPrototype")]
	private GameObject m_RequirementPrototype;

	// Token: 0x04001631 RID: 5681
	public List<UIKingdomAdvantagesWindow.GoodsSlot> m_Requirements = new List<UIKingdomAdvantagesWindow.GoodsSlot>();

	// Token: 0x04001632 RID: 5682
	public KingdomAdvantage Data;

	// Token: 0x04001633 RID: 5683
	[HideInInspector]
	public DT.Field ui_def;

	// Token: 0x04001636 RID: 5686
	private bool m_DisableTooltip;

	// Token: 0x04001637 RID: 5687
	private bool m_Initialized;

	// Token: 0x04001638 RID: 5688
	private bool m_Invalidate;

	// Token: 0x02000773 RID: 1907
	public enum State
	{
		// Token: 0x04003AA7 RID: 15015
		Default,
		// Token: 0x04003AA8 RID: 15016
		Completed
	}
}
