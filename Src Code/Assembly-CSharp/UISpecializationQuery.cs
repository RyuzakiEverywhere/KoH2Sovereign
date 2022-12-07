using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001F7 RID: 503
public class UISpecializationQuery : MonoBehaviour
{
	// Token: 0x06001E97 RID: 7831 RVA: 0x0011C33B File Offset: 0x0011A53B
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x0011C34A File Offset: 0x0011A54A
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x0011C35B File Offset: 0x0011A55B
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x0011C36C File Offset: 0x0011A56C
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

	// Token: 0x06001E9B RID: 7835 RVA: 0x0011C3C4 File Offset: 0x0011A5C4
	private void PopulateSkills()
	{
		this.selected_skill = null;
		if (this.Group_Skills == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.Group_Skills);
		if (this.Skill_Prototype == null)
		{
			return;
		}
		List<Skill.Def> new_skills = this.Data.new_skills;
		if (new_skills == null)
		{
			return;
		}
		for (int i = 0; i < new_skills.Count; i++)
		{
			UISkill component = UnityEngine.Object.Instantiate<GameObject>(this.Skill_Prototype, Vector3.zero, Quaternion.identity, this.Group_Skills).GetComponent<UISkill>();
			if (!(component == null))
			{
				(component.transform as RectTransform).sizeDelta = new Vector2(97f, 80f);
				component.SetData(this.Data, new_skills[i], true, false);
				component.OnSelect += this.HandleSkillSelect;
			}
		}
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x0011C492 File Offset: 0x0011A692
	private void Update()
	{
		if (!this.IsValidCharacter(this.Data))
		{
			this.SetData(null, null, null);
		}
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x00115C3F File Offset: 0x00113E3F
	private bool IsValidCharacter(Logic.Character c)
	{
		return c != null && c.IsAlive() && (c.IsPrince() || c.IsPrincess() || c.IsKing() || c.IsInCourt());
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x0011C4AC File Offset: 0x0011A6AC
	public void SetData(Logic.Character characterData, DT.Field def_field, Vars vars)
	{
		this.Data = characterData;
		if (this.Data == null || !this.Data.IsAlive())
		{
			this.Close();
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.Icon_Child != null)
		{
			this.Icon_Child.SetObject(characterData, null);
		}
		UICharacter component = base.GetComponent<UICharacter>();
		if (component != null)
		{
			component.SetObject(characterData, vars);
		}
		UIText.ForceNextLinks(UIText.LinkSettings.Mode.NotColorized);
		UIText.SetText(this.Value_Caption, def_field, "caption", vars, null);
		UIText.SetText(this.Value_Body, def_field, "body", vars, null);
		if (this.Value_Age != null)
		{
			UIText.SetTextKey(this.Value_Age, "CharacterTooltip.character_age", this.Data, null);
		}
		UIText.SetTextKey(this.Value_Class, this.Data.class_title, null, null);
		if (this.Button_Accept != null)
		{
			this.Button_Accept.onClick = new BSGButton.OnClick(this.HandleOnCloseClick);
			UIText.SetText(this.Button_Accept.gameObject, "id_Text", this.field, "button_close", vars, null);
		}
		if (this.Button_RoyalFamily != null)
		{
			this.Button_RoyalFamily.onClick = new BSGButton.OnClick(this.HanldeOpenFamily);
			UIText.SetText(this.Button_RoyalFamily.gameObject, "id_Text", this.field, "button_family", vars, null);
		}
		this.PopulateSkills();
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x0011C618 File Offset: 0x0011A818
	private void HandleSkillSelect(UISkill btn, PointerEventData arg2)
	{
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
		this.selected_skill = btn.def;
		this.Data.AddSkill(btn.def, true);
		this.Close();
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x0011C69B File Offset: 0x0011A89B
	private void HandleOnCloseClick(BSGButton btn)
	{
		this.Close();
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x0011C6A3 File Offset: 0x0011A8A3
	private void HanldeOpenFamily(BSGButton b)
	{
		UIRoyalFamily.ToggleOpen(this.Data.GetKingdom());
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x0011C6B8 File Offset: 0x0011A8B8
	private void Close()
	{
		if (this.Data != null && this.selected_skill == null)
		{
			this.Data.AddRandomNewSkill();
		}
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x04001405 RID: 5125
	[SerializeField]
	private GameObject Skill_Prototype;

	// Token: 0x04001406 RID: 5126
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon Icon_Child;

	// Token: 0x04001407 RID: 5127
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI Value_Caption;

	// Token: 0x04001408 RID: 5128
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI Value_Body;

	// Token: 0x04001409 RID: 5129
	[UIFieldTarget("id_CharacterAge")]
	private TextMeshProUGUI Value_Age;

	// Token: 0x0400140A RID: 5130
	[UIFieldTarget("id_CharacterClass")]
	private TextMeshProUGUI Value_Class;

	// Token: 0x0400140B RID: 5131
	[UIFieldTarget("id_Skills")]
	private RectTransform Group_Skills;

	// Token: 0x0400140C RID: 5132
	[UIFieldTarget("id_Button_Accept")]
	private BSGButton Button_Accept;

	// Token: 0x0400140D RID: 5133
	[UIFieldTarget("id_Button_RoyalFamily")]
	private BSGButton Button_RoyalFamily;

	// Token: 0x0400140E RID: 5134
	private Logic.Character Data;

	// Token: 0x0400140F RID: 5135
	public DT.Field field;

	// Token: 0x04001410 RID: 5136
	private bool m_Started;

	// Token: 0x04001411 RID: 5137
	private bool m_DataExtracted;

	// Token: 0x04001412 RID: 5138
	private Skill.Def selected_skill;
}
