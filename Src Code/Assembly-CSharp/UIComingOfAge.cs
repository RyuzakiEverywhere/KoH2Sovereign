using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001EC RID: 492
public class UIComingOfAge : MonoBehaviour, IListener
{
	// Token: 0x06001D85 RID: 7557 RVA: 0x00115AF0 File Offset: 0x00113CF0
	private void Init()
	{
		if (this.m_Initilized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initilized = true;
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x00115B09 File Offset: 0x00113D09
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x00115B18 File Offset: 0x00113D18
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x00115B29 File Offset: 0x00113D29
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x00115B3C File Offset: 0x00113D3C
	private void ExtractData()
	{
		this.m_DataExtracted = true;
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			this.field = component.def_field;
			Logic.Character character = component.vars.obj.Get<Logic.Character>();
			if (character != null)
			{
				this.SetData(character, component.def_field, component.vars);
			}
		}
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x00115B93 File Offset: 0x00113D93
	private void Update()
	{
		if (!this.IsValidCharacter(this.Data))
		{
			this.SetData(null, null, null);
			return;
		}
		this.UpdateSkillStatus();
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x00115BB4 File Offset: 0x00113DB4
	private void UpdateSkillStatus()
	{
		Logic.Character data = this.Data;
		bool flag = data != null && data.CanLearnNewSkills();
		if (this.Button_Accept != null)
		{
			this.Button_Accept.gameObject.SetActive(flag);
		}
		if (this.Button_Ok != null)
		{
			this.Button_Ok.gameObject.SetActive(!flag);
		}
		if (this.Value_Body)
		{
			UIText.SetText(this.Value_Body, this.field, "body", this.Vars, null);
		}
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x00115C3F File Offset: 0x00113E3F
	private bool IsValidCharacter(Logic.Character c)
	{
		return c != null && c.IsAlive() && (c.IsPrince() || c.IsPrincess() || c.IsKing() || c.IsInCourt());
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x00115C70 File Offset: 0x00113E70
	public void SetData(Logic.Character characterData, DT.Field def_field, Vars vars)
	{
		this.Init();
		this.Vars = vars;
		this.Data = characterData;
		if (this.Data == null || !this.Data.IsAlive())
		{
			this.Close();
			return;
		}
		this.Data.AddListener(this);
		if (this.Button_Accept != null)
		{
			this.Button_Accept.onClick = new BSGButton.OnClick(this.HandleOnCloseClick);
			UIText.SetText(this.Button_Accept.gameObject, "id_Text", this.field, "button_close", vars, null);
		}
		if (this.Button_Ok != null)
		{
			this.Button_Ok.onClick = new BSGButton.OnClick(this.HandleOnCloseClick);
			UIText.SetText(this.Button_Ok.gameObject, "id_Text", this.field, "button_ok", vars, null);
		}
		if (this.Icon_Child != null)
		{
			this.Icon_Child.SetObject(characterData, null);
		}
		UIText.ForceNextLinks(UIText.LinkSettings.Mode.NotColorized);
		UIText.SetText(this.Value_Caption, def_field, "caption", vars, null);
		if (this.Button_RoyalFamily != null)
		{
			this.Button_RoyalFamily.onClick = new BSGButton.OnClick(this.HanldeOpenFamily);
			UIText.SetText(this.Button_RoyalFamily.gameObject, "id_Text", this.field, "button_family", vars, null);
		}
		if (this.Value_Age != null)
		{
			UIText.SetTextKey(this.Value_Age, "CharacterTooltip.character_age", characterData, null);
		}
		UIText.SetTextKey(this.Value_Class, this.Data.class_title, null, null);
		this.PopulateSkills();
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x00115E08 File Offset: 0x00114008
	private void PopulateSkills()
	{
		if (this.Skill_Prototype == null)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.Group_SkillOptions);
		List<Skill.Def> new_skills = this.Data.new_skills;
		if (new_skills == null)
		{
			return;
		}
		for (int i = 0; i < new_skills.Count; i++)
		{
			UISkill component = UnityEngine.Object.Instantiate<GameObject>(this.Skill_Prototype, Vector3.zero, Quaternion.identity, this.Group_SkillOptions).GetComponent<UISkill>();
			if (!(component == null))
			{
				component.gameObject.SetActive(true);
				Transform transform = component.transform;
				component.SetData(this.Data, new_skills[i], true, false);
				component.OnSelect += this.HandleSkillSelect;
			}
		}
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x00115EB4 File Offset: 0x001140B4
	private void HandleSkillSelect(UISkill btn, PointerEventData arg2)
	{
		this.Close();
		Logic.Character data = this.Data;
		object obj;
		if (data == null)
		{
			obj = null;
		}
		else
		{
			Actions actions = data.actions;
			obj = ((actions != null) ? actions.Find("LearnNewSkillAction") : null);
		}
		LearnNewSkillAction learnNewSkillAction = obj as LearnNewSkillAction;
		if (learnNewSkillAction != null)
		{
			BaseUI.PlaySoundEvent(learnNewSkillAction.def.field.GetRandomString("prepare_sound_effect", learnNewSkillAction, "", true, true, true, '.'), null);
		}
		this.Data.AddSkill(btn.def, true);
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x00115F2B File Offset: 0x0011412B
	private void HanldeOpenFamily(BSGButton b)
	{
		UIRoyalFamily.ToggleOpen(this.Data.GetKingdom());
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x00115F3D File Offset: 0x0011413D
	private void HandleAdditionalAction(BSGButton btn)
	{
		this.Close();
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x00115F3D File Offset: 0x0011413D
	private void HandleOnCloseClick(BSGButton btn)
	{
		this.Close();
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x00115F45 File Offset: 0x00114145
	public void OnMessage(object obj, string message, object param)
	{
		if (this.Data == obj && message == "skills_changed")
		{
			UICommon.DeleteChildren(this.Group_SkillOptions);
			this.PopulateSkills();
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x00115F70 File Offset: 0x00114170
	private void Close()
	{
		Logic.Character data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x04001361 RID: 4961
	[SerializeField]
	private GameObject Skill_Prototype;

	// Token: 0x04001362 RID: 4962
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon Icon_Child;

	// Token: 0x04001363 RID: 4963
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI Value_Caption;

	// Token: 0x04001364 RID: 4964
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI Value_Body;

	// Token: 0x04001365 RID: 4965
	[UIFieldTarget("id_CharacterAge")]
	private TextMeshProUGUI Value_Age;

	// Token: 0x04001366 RID: 4966
	[UIFieldTarget("id_CharacterClass")]
	private TextMeshProUGUI Value_Class;

	// Token: 0x04001367 RID: 4967
	[UIFieldTarget("id_SkillOptions")]
	private RectTransform Group_SkillOptions;

	// Token: 0x04001368 RID: 4968
	[UIFieldTarget("id_Button_RoyalFamily")]
	private BSGButton Button_RoyalFamily;

	// Token: 0x04001369 RID: 4969
	[UIFieldTarget("id_Button_Accept")]
	private BSGButton Button_Accept;

	// Token: 0x0400136A RID: 4970
	[UIFieldTarget("id_Button_Ok")]
	private BSGButton Button_Ok;

	// Token: 0x0400136B RID: 4971
	public DT.Field field;

	// Token: 0x0400136C RID: 4972
	private Logic.Character Data;

	// Token: 0x0400136D RID: 4973
	private Vars Vars;

	// Token: 0x0400136E RID: 4974
	private bool m_Initilized;

	// Token: 0x0400136F RID: 4975
	private bool m_Started;

	// Token: 0x04001370 RID: 4976
	private bool m_DataExtracted;
}
