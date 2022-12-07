using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000291 RID: 657
public class UICourtMemberSkills : MonoBehaviour, IListener
{
	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600285E RID: 10334 RVA: 0x00159FE0 File Offset: 0x001581E0
	// (set) Token: 0x0600285F RID: 10335 RVA: 0x00159FE8 File Offset: 0x001581E8
	public Logic.Character Character { get; private set; }

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06002860 RID: 10336 RVA: 0x00159FF1 File Offset: 0x001581F1
	// (set) Token: 0x06002861 RID: 10337 RVA: 0x00159FF9 File Offset: 0x001581F9
	public Vars Vars { get; private set; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06002862 RID: 10338 RVA: 0x0015A002 File Offset: 0x00158202
	// (set) Token: 0x06002863 RID: 10339 RVA: 0x0015A00A File Offset: 0x0015820A
	public int SelectedSlot { get; private set; }

	// Token: 0x06002864 RID: 10340 RVA: 0x0015A013 File Offset: 0x00158213
	public void SetData(Logic.Character character, Vars vars)
	{
		Logic.Character character2 = this.Character;
		if (character2 != null)
		{
			character2.DelListener(this);
		}
		this.Character = character;
		Logic.Character character3 = this.Character;
		if (character3 != null)
		{
			character3.AddListener(this);
		}
		this.Init();
		this.Vars = vars;
		this.Refresh();
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x0015A053 File Offset: 0x00158253
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.PopulateSkills();
		this.m_Initalized = true;
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x0015A072 File Offset: 0x00158272
	private void Refresh()
	{
		this.UpdateSkills();
		this.UpdateOptions();
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x0015A080 File Offset: 0x00158280
	private void UpdateOptions()
	{
		if (this.Character == null || this.Character.IsPrisoner() || !this.Character.IsInCourt(BaseUI.LogicKingdom()))
		{
			this.m_SkillOptions.gameObject.SetActive(false);
			return;
		}
		this.m_SkillOptions.gameObject.SetActive(true);
		Logic.Kingdom kingdom = this.Character.GetKingdom();
		if (this.m_LabelAfilinty != null)
		{
			UIText.SetTextKey(this.m_LabelAfilinty, "Character.pick_skill.affinity_label", new Vars(this.Character), null);
		}
		if (this.m_LabeMaxSkills != null)
		{
			UIText.SetTextKey(this.m_LabeMaxSkills, "Character.pick_skill.max_skills", new Vars(this.Character), null);
		}
		UICommon.DeleteChildren(this.m_AffinitySkillsContianer);
		UICommon.DeleteChildren(this.m_OtherTraditionsContianer);
		List<Skill.Def> newSkillOptions = this.Character.GetNewSkillOptions("all", true, false);
		if (newSkillOptions != null && newSkillOptions.Count > 0)
		{
			for (int i = 0; i < newSkillOptions.Count; i++)
			{
				Skill.Def def = newSkillOptions[i];
				if (def != null && this.Character.GetSkill(def.name) == null)
				{
					if (def.IsAffinitySkill(this.Character))
					{
						this.AddSkillIcon(def, this.m_AffinitySkillsContianer);
					}
					if (def.IsGrantedByTradition(kingdom))
					{
						this.AddSkillIcon(def, this.m_OtherTraditionsContianer);
					}
					else if (def.IsInherited(this.Character))
					{
						this.AddSkillIcon(def, this.m_OtherTraditionsContianer);
					}
				}
			}
		}
		this.m_AffnityOptions.gameObject.SetActive(this.m_AffinitySkillsContianer.childCount > 0);
		this.m_TraditionsAndOtherOptions.gameObject.SetActive(this.m_OtherTraditionsContianer.childCount > 0);
		this.m_MaxSkills.gameObject.SetActive(newSkillOptions == null || newSkillOptions.Count == 0);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x0015A254 File Offset: 0x00158454
	private void PopulateSkills()
	{
		int num = this.Character.NumSkillSlots();
		UICommon.DeleteChildren(this.m_PrimarySkillsContainer);
		UICommon.DeleteChildren(this.m_SecondarySkillsContainer);
		for (int i = 0; i < num; i++)
		{
			Vars vars = new Vars();
			vars.Set<string>("variant", "rounded");
			string text = this.Character.SkillSlotType(i);
			RectTransform rectTransform = null;
			if (text == "primary")
			{
				rectTransform = this.m_PrimarySkillsContainer;
			}
			else if (text == "secondary")
			{
				rectTransform = this.m_SecondarySkillsContainer;
			}
			if (!(rectTransform == null))
			{
				GameObject icon = ObjectIcon.GetIcon("Skill", vars, rectTransform);
				if (icon == null)
				{
					break;
				}
				UISkill orAddComponent = icon.GetOrAddComponent<UISkill>();
				orAddComponent.OnSelect += this.HandleOnSkillSlotSelected;
				orAddComponent.OverrideType(text);
				this.m_SkillIcons.Add(orAddComponent);
			}
		}
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x0015A33C File Offset: 0x0015853C
	private void HandleOnSkillSlotSelected(UISkill icon, PointerEventData e)
	{
		if (this.Character == null)
		{
			return;
		}
		if (this.Character.IsPrisoner())
		{
			return;
		}
		if (!this.Character.IsInCourt(BaseUI.LogicKingdom()))
		{
			return;
		}
		if (icon.skill != null)
		{
			Actions actions = this.Character.actions;
			Action action = (actions != null) ? actions.Find("IncreaseSkillRankAction") : null;
			if (action != null)
			{
				if (action.args != null)
				{
					action.args = null;
				}
				action.AddArg(ref action.args, icon.skill.def.id, 0);
				ActionVisuals.ExecuteAction(action);
			}
		}
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x0015A3D4 File Offset: 0x001585D4
	private void UpdateSkills()
	{
		if (this.Character == null)
		{
			return;
		}
		for (int i = 0; i < this.m_SkillIcons.Count; i++)
		{
			Skill skill = (this.Character.skills != null && this.Character.skills.Count > i) ? this.Character.skills[i] : null;
			this.m_SkillIcons[i].SetData(this.Character, skill, true, true, true);
		}
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x0015A450 File Offset: 0x00158650
	private void HandleSkillHighlight(Skill.Def def, bool gain)
	{
		if (def == null)
		{
			return;
		}
		string slotType = def.GetSlotType(this.Character);
		bool flag = false;
		for (int i = 0; i < this.m_SkillIcons.Count; i++)
		{
			UISkill uiskill = this.m_SkillIcons[i];
			if (!(uiskill == null))
			{
				uiskill.Select(false);
				if (gain && !flag && uiskill.def == null && uiskill.skill == null && ((slotType == "primary" && uiskill.transform.parent == this.m_PrimarySkillsContainer) || (slotType == "secondary" && uiskill.transform.parent == this.m_SecondarySkillsContainer)))
				{
					uiskill.Select(true);
					flag = true;
				}
			}
		}
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x0015A512 File Offset: 0x00158712
	public void Close()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x0015A531 File Offset: 0x00158731
	private void OnDestroy()
	{
		Logic.Character character = this.Character;
		if (character == null)
		{
			return;
		}
		character.DelListener(this);
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x0015A544 File Offset: 0x00158744
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "skills_changed")
		{
			this.Refresh();
			return;
		}
		if (!(message == "prison_changed"))
		{
			return;
		}
		using (Game.Profile("UICourtMemberSkills OnMessage", false, 0f, null))
		{
			this.Refresh();
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x0015A5AC File Offset: 0x001587AC
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberSkills", null);
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x0015A5BC File Offset: 0x001587BC
	public static UICourtMemberSkills Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberSkills orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberSkills>();
		orAddComponent.SetData(character, vars);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x0015A610 File Offset: 0x00158810
	private void AddSkillIcon(Skill.Def def, RectTransform contianer)
	{
		Vars vars = new Vars();
		vars.Set<string>("variant", "rounded");
		GameObject icon = ObjectIcon.GetIcon("Skill", vars, contianer);
		if (icon == null)
		{
			return;
		}
		UISkill orAddComponent = icon.GetOrAddComponent<UISkill>();
		orAddComponent.SetData(this.Character, def, true, true);
		orAddComponent.OnSelect += this.HandleOnOptionSelected;
		orAddComponent.OnFocusGain += this.HandleOnSkillOptionFocusGain;
		orAddComponent.OnFocusLoss += this.HandleOnSkillOptionFocusLoss;
		if (this.HasSkill(def))
		{
			orAddComponent.GetComponent<CanvasGroup>().alpha = 0.5f;
		}
		if (!this.IsLearnableSkill(def))
		{
			orAddComponent.SetEligable(false);
		}
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x0015A6BF File Offset: 0x001588BF
	private bool IsLearnableSkill(Skill.Def skillDef)
	{
		return skillDef != null && this.Character != null && this.Character.CanAddSkill(skillDef);
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x0015A6DC File Offset: 0x001588DC
	private bool HasFreeSlots(Skill.Def skillDef)
	{
		return this.Character != null && this.Character.GetNewSkillSlot(skillDef) != -1;
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x0015A6FC File Offset: 0x001588FC
	private bool HasSkill(Skill.Def skillDef)
	{
		if (skillDef == null)
		{
			return false;
		}
		if (this.Character == null)
		{
			return false;
		}
		if (this.Character.skills == null || this.Character.skills.Count == 0)
		{
			return false;
		}
		int i = 0;
		int count = this.Character.skills.Count;
		while (i < count)
		{
			if (this.Character.skills[i] != null && this.Character.skills[i].def == skillDef)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x0015A788 File Offset: 0x00158988
	private void HandleOnOptionSelected(UISkill skillIcon, PointerEventData e)
	{
		Actions actions = this.Character.actions;
		LearnNewSkillAction learnNewSkillAction = ((actions != null) ? actions.Find("LearnNewSkillAction") : null) as LearnNewSkillAction;
		if (learnNewSkillAction == null)
		{
			return;
		}
		if (this.HasSkill(skillIcon.def) || !this.IsLearnableSkill(skillIcon.def) || !this.HasFreeSlots(skillIcon.def))
		{
			BaseUI.PlayVoiceEvent(learnNewSkillAction.GetVar("decline_voice_line", learnNewSkillAction, true), learnNewSkillAction.GetVoiceVars());
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("action_cost_not_met", null, "", true, true, true, '.') : null, null);
			return;
		}
		learnNewSkillAction.args = new List<Value>();
		learnNewSkillAction.target = this.Character;
		learnNewSkillAction.args.Add(new Value(skillIcon.def.field.key));
		if (learnNewSkillAction.CheckCost(null))
		{
			ActionVisuals.ExecuteAction(learnNewSkillAction);
			return;
		}
		BaseUI.PlayVoiceEvent(learnNewSkillAction.GetVar("decline_voice_line", learnNewSkillAction, true), learnNewSkillAction.GetVoiceVars());
		DT.Field soundsDef2 = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("action_cost_not_met", null, "", true, true, true, '.') : null, null);
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x0015A8B5 File Offset: 0x00158AB5
	private void HandleOnSkillOptionFocusGain(UISkill skillIcon, PointerEventData e)
	{
		this.HandleSkillHighlight((skillIcon != null) ? skillIcon.def : null, true);
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x0015A8CA File Offset: 0x00158ACA
	private void HandleOnSkillOptionFocusLoss(UISkill skillIcon, PointerEventData e)
	{
		this.HandleSkillHighlight((skillIcon != null) ? skillIcon.def : null, false);
	}

	// Token: 0x04001B53 RID: 6995
	[UIFieldTarget("id_PrimarySkillsContainer")]
	private RectTransform m_PrimarySkillsContainer;

	// Token: 0x04001B54 RID: 6996
	[UIFieldTarget("id_SecondarySkillsContainer")]
	private RectTransform m_SecondarySkillsContainer;

	// Token: 0x04001B55 RID: 6997
	[UIFieldTarget("id_UnknownSkillsContainer")]
	private RectTransform m_UnknownSkillsContainer;

	// Token: 0x04001B56 RID: 6998
	[UIFieldTarget("id_SkillOptions")]
	private GameObject m_SkillOptions;

	// Token: 0x04001B57 RID: 6999
	[UIFieldTarget("id_AffnityOptions")]
	private GameObject m_AffnityOptions;

	// Token: 0x04001B58 RID: 7000
	[UIFieldTarget("id_TraditionsAndOtherOptions")]
	private GameObject m_TraditionsAndOtherOptions;

	// Token: 0x04001B59 RID: 7001
	[UIFieldTarget("id_MaxSkills")]
	private GameObject m_MaxSkills;

	// Token: 0x04001B5A RID: 7002
	[UIFieldTarget("id_AffinitySkillsContianer")]
	private RectTransform m_AffinitySkillsContianer;

	// Token: 0x04001B5B RID: 7003
	[UIFieldTarget("id_OtherTraditionsContianer")]
	private RectTransform m_OtherTraditionsContianer;

	// Token: 0x04001B5C RID: 7004
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001B5D RID: 7005
	[UIFieldTarget("id_LabelAfilinty")]
	private TextMeshProUGUI m_LabelAfilinty;

	// Token: 0x04001B5E RID: 7006
	[UIFieldTarget("id_LabeMaxSkills")]
	private TextMeshProUGUI m_LabeMaxSkills;

	// Token: 0x04001B62 RID: 7010
	private List<UISkill> m_SkillIcons = new List<UISkill>();

	// Token: 0x04001B63 RID: 7011
	private bool m_Initalized;
}
