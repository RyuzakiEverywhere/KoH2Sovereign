using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000290 RID: 656
public class UICourtMemberSkillPicker : MonoBehaviour
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06002848 RID: 10312 RVA: 0x00159B72 File Offset: 0x00157D72
	// (set) Token: 0x06002849 RID: 10313 RVA: 0x00159B7A File Offset: 0x00157D7A
	public Logic.Character Character { get; private set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600284A RID: 10314 RVA: 0x00159B83 File Offset: 0x00157D83
	// (set) Token: 0x0600284B RID: 10315 RVA: 0x00159B8B File Offset: 0x00157D8B
	public int Slot { get; private set; }

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x0600284C RID: 10316 RVA: 0x00159B94 File Offset: 0x00157D94
	// (set) Token: 0x0600284D RID: 10317 RVA: 0x00159B9C File Offset: 0x00157D9C
	public Vars Vars { get; private set; }

	// Token: 0x0600284E RID: 10318 RVA: 0x00159BA5 File Offset: 0x00157DA5
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = false;
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x00159BBE File Offset: 0x00157DBE
	private void SetData(Logic.Character character, int slot, Vars vars)
	{
		this.Init();
		this.Character = character;
		this.Slot = slot;
		this.Vars = vars;
		this.Refresh();
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x00159BE4 File Offset: 0x00157DE4
	private void Refresh()
	{
		if (this.Character == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Character.GetKingdom();
		if (this.m_LabelAfilinty != null)
		{
			UIText.SetTextKey(this.m_LabelAfilinty, "Character.pick_skill.affinity_label", new Vars(this.Character), null);
		}
		List<Skill.Def> newSkillOptions = this.Character.GetNewSkillOptions("all", true, true);
		if (newSkillOptions == null || newSkillOptions.Count == 0)
		{
			return;
		}
		for (int i = 0; i < newSkillOptions.Count; i++)
		{
			Skill.Def def = newSkillOptions[i];
			if (def != null)
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

	// Token: 0x06002853 RID: 10323 RVA: 0x00159CC0 File Offset: 0x00157EC0
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
		orAddComponent.OnSelect += this.HandleOnSkillSlotSelected;
		orAddComponent.OnFocusGain += this.HandleOnSkillSlotFocusGain;
		orAddComponent.OnFocusLoss += this.HandleOnSkillSlotFocusLoss;
		if (this.HasSkill(def))
		{
			orAddComponent.GetComponent<CanvasGroup>().alpha = 0.5f;
		}
		if (!this.IsLearnableSkill(def))
		{
			orAddComponent.SetEligable(false);
		}
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x00159D6F File Offset: 0x00157F6F
	private bool IsLearnableSkill(Skill.Def skillDef)
	{
		return skillDef != null && this.Character != null && this.Character.CanAddSkill(skillDef);
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x00159D8C File Offset: 0x00157F8C
	private bool HasFreeSlots(Skill.Def skillDef)
	{
		return this.Character != null && this.Character.GetNewSkillSlot(skillDef) != -1;
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x00159DAC File Offset: 0x00157FAC
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

	// Token: 0x06002857 RID: 10327 RVA: 0x00159E38 File Offset: 0x00158038
	private void HandleOnSkillSlotSelected(UISkill skillIcon, PointerEventData e)
	{
		if (this.HasSkill(skillIcon.def) || !this.IsLearnableSkill(skillIcon.def) || !this.HasFreeSlots(skillIcon.def))
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("action_cost_not_met", null, "", true, true, true, '.') : null, null);
			return;
		}
		Actions actions = this.Character.actions;
		LearnNewSkillAction learnNewSkillAction = ((actions != null) ? actions.Find("LearnNewSkillAction") : null) as LearnNewSkillAction;
		if (learnNewSkillAction == null)
		{
			return;
		}
		learnNewSkillAction.args = new List<Value>();
		learnNewSkillAction.target = this.Character;
		learnNewSkillAction.args.Add(new Value(skillIcon.def.field.key));
		if (learnNewSkillAction.CheckCost(null))
		{
			ActionVisuals.ExecuteAction(learnNewSkillAction);
			this.Close();
			return;
		}
		DT.Field soundsDef2 = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("action_cost_not_met", null, "", true, true, true, '.') : null, null);
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x00159F31 File Offset: 0x00158131
	private void HandleOnSkillSlotFocusGain(UISkill skillIcon, PointerEventData e)
	{
		if (this.OnSkillHighlight != null)
		{
			this.OnSkillHighlight(skillIcon.def, true);
		}
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x00159F4D File Offset: 0x0015814D
	private void HandleOnSkillSlotFocusLoss(UISkill skillIcon, PointerEventData e)
	{
		if (this.OnSkillHighlight != null)
		{
			this.OnSkillHighlight(skillIcon.def, false);
		}
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x00159F69 File Offset: 0x00158169
	public void Close()
	{
		this.OnSkillHighlight = null;
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x00159F7D File Offset: 0x0015817D
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberPickSkill", null);
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x00159F8C File Offset: 0x0015818C
	public static UICourtMemberSkillPicker Create(Logic.Character character, int slot, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberSkillPicker orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberSkillPicker>();
		orAddComponent.SetData(character, slot, vars);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x04001B4A RID: 6986
	[UIFieldTarget("id_AffinitySkillsContianer")]
	private RectTransform m_AffinitySkillsContianer;

	// Token: 0x04001B4B RID: 6987
	[UIFieldTarget("id_OtherTraditionsContianer")]
	private RectTransform m_OtherTraditionsContianer;

	// Token: 0x04001B4C RID: 6988
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001B4D RID: 6989
	[UIFieldTarget("id_LabelAfilinty")]
	private TextMeshProUGUI m_LabelAfilinty;

	// Token: 0x04001B51 RID: 6993
	private bool m_Initialzed;

	// Token: 0x04001B52 RID: 6994
	public Action<Skill.Def, bool> OnSkillHighlight;
}
