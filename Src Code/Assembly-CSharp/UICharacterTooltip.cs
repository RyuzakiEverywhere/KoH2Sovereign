using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001EB RID: 491
public class UICharacterTooltip : MonoBehaviour, Tooltip.IHandler, IListener
{
	// Token: 0x06001D70 RID: 7536 RVA: 0x00114E08 File Offset: 0x00113008
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.id_SkillIconPrototype != null)
		{
			this.id_SkillIconPrototype.gameObject.SetActive(false);
		}
		if (this.id_CharacterIcon != null)
		{
			this.id_CharacterIcon.ShowCrest(true);
			this.id_CharacterIcon.ShowMissonKingdomCrest(false);
			this.id_CharacterIcon.ShowStatus(false);
			this.id_CharacterIcon.ShowMissonKingdomCrest(false);
			this.id_CharacterIcon.ShowArmyBanner(false);
		}
		if (this.id_ClassLevel != null)
		{
			this.classLevel = this.id_ClassLevel.AddComponent<UICharacterTooltip.ClassLevel>();
		}
		this.AllocateSkillIcons(0);
		this.m_Initialzed = true;
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x00114EBC File Offset: 0x001130BC
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Character c = null;
		if (tooltip.vars != null)
		{
			c = tooltip.vars.obj.Get<Logic.Character>();
		}
		return this.HandleTooltip(c, tooltip.vars, ui, evt);
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x00114EF4 File Offset: 0x001130F4
	public bool HandleTooltip(Logic.Character c, Vars vars, BaseUI ui, Tooltip.Event evt)
	{
		this.Init();
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return false;
		}
		if (c == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return true;
		}
		this.SetCharacter(c);
		this.tooltipVars = vars;
		if (this.id_Abilities != null && evt == Tooltip.Event.Fill)
		{
			Logic.Character data = this.Data;
			bool flag = ((data != null) ? data.GetRoyalAbilities() : null) != null && this.Data.IsRoyalty();
			if (flag && this.id_Abilities.Character != this.Data)
			{
				this.id_Abilities.SetData(this.Data, null);
			}
			else
			{
				this.id_Abilities.SetData(null, null);
			}
			this.id_Abilities.gameObject.SetActive(flag);
		}
		if (this.id_CharacterIcon != null)
		{
			this.id_CharacterIcon.SetObject(this.Data, null);
		}
		if (this.classLevel != null)
		{
			this.classLevel.SetData(this.Data);
		}
		this.foreign = (c.GetKingdom() != BaseUI.LogicKingdom());
		this.Refresh(evt != Tooltip.Event.Update);
		return true;
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x00115018 File Offset: 0x00113218
	private void SetCharacter(Logic.Character character)
	{
		if (character == this.Data)
		{
			return;
		}
		Logic.Character data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = character;
		Logic.Character data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		if (this.m_KingGeneration != null)
		{
			Tooltip.Get(this.m_KingGeneration, true).SetDef("GenerationsPassedTooltip", new Vars(this.Data));
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x00115090 File Offset: 0x00113290
	private void Refresh(bool full = true)
	{
		this.Init();
		this.RefreshIconLayout();
		if (this.m_InvalidateClass || full)
		{
			this.RefreshClassColor();
			this.RefreshKingdomAndClass();
			this.m_InvalidateClass = false;
		}
		if (this.m_InvalidateName || full)
		{
			this.RefreshName();
		}
		if (this.m_InvalidateMarriage || full)
		{
			this.RefreshMarriage();
			this.m_InvalidateMarriage = false;
		}
		this.RefreshSkills(full);
		if (this.m_InvalidateStatus || full)
		{
			this.RefreshStatus();
			this.m_InvalidateStatus = false;
		}
		this.RefreshAddText();
		this.RefreshAdviseLabel();
		if (full)
		{
			this.RefreshKingGeneration();
		}
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x00115120 File Offset: 0x00113320
	private void RefreshIconLayout()
	{
		bool flag = this.tooltipVars.Get<bool>("hide_icon", false);
		this.id_CrestLeft.SetObject(this.Data, null);
		UIKingdomIcon uikingdomIcon = this.id_CrestLeft;
		if (uikingdomIcon != null)
		{
			uikingdomIcon.gameObject.SetActive(flag);
		}
		UICharacterIcon uicharacterIcon = this.id_CharacterIcon;
		if (uicharacterIcon == null)
		{
			return;
		}
		uicharacterIcon.gameObject.SetActive(!flag);
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x00115184 File Offset: 0x00113384
	private void RefreshClassColor()
	{
		if (this.id_ClassColor == null)
		{
			return;
		}
		string class_name = this.Data.class_name;
		Color color = global::Defs.GetColor("CharacterTooltip", "class_gradient_colors." + class_name, Color.clear);
		if (color == Color.clear)
		{
			this.id_ClassColor.gameObject.SetActive(false);
			return;
		}
		this.id_ClassColor.gameObject.SetActive(true);
		this.id_ClassColor.color = color;
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x00115203 File Offset: 0x00113403
	private void RefreshName()
	{
		if (this.id_CharacterName == null)
		{
			return;
		}
		UIText.SetTextKey(this.id_CharacterName, "CharacterTooltip.character_name", this.Data, null);
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x0011522C File Offset: 0x0011342C
	private void RefreshKingGeneration()
	{
		if (this.Data == null)
		{
			return;
		}
		bool flag = false;
		if (this.Data.IsKing())
		{
			flag = global::Defs.GetBool("CharacterTooltip", "always_show_king_generation", null, false);
			if (!flag)
			{
				flag = (this.Data.game.rules.time_limits.type == Game.CampaignRules.TimeLimits.Type.Generations);
			}
		}
		if (this.m_KingGeneration != null)
		{
			this.m_KingGeneration.gameObject.SetActive(flag);
		}
		if (this.m_KingGenerationLabel != null)
		{
			UIText.SetText(this.m_KingGenerationLabel, this.Data.GetKingdom().generationsPassed.ToString());
		}
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x001152D4 File Offset: 0x001134D4
	private void RefreshKingdomAndClass()
	{
		if (this.id_ClassAndAge == null)
		{
			return;
		}
		if (this.Data.sex == Logic.Character.Sex.Female && this.Data.IsMarried())
		{
			this.id_ClassAndAge.gameObject.SetActive(false);
			return;
		}
		this.id_ClassAndAge.gameObject.SetActive(true);
		UIText.SetTextKey(this.id_ClassAndAge, "Character.kingdom_class_and_age", this.Data, null);
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x00115348 File Offset: 0x00113548
	private void RefreshMarriage()
	{
		if (this.id_Marriage == null)
		{
			return;
		}
		if (this.Data.sex == Logic.Character.Sex.Female && this.Data.IsMarried())
		{
			this.id_Marriage.gameObject.SetActive(true);
			UIText.SetTextKey(this.id_Marriage, "CharacterTooltip.marriage", this.Data, null);
			return;
		}
		this.id_Marriage.gameObject.SetActive(false);
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x001153BC File Offset: 0x001135BC
	private bool CheckShowAge(Logic.Character c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.IsAging() || c.can_die)
		{
			return true;
		}
		Logic.Kingdom kingdom = c.GetKingdom();
		List<Logic.Character> list;
		if (kingdom == null)
		{
			list = null;
		}
		else
		{
			Logic.RoyalFamily royalFamily = kingdom.royalFamily;
			list = ((royalFamily != null) ? royalFamily.Relatives : null);
		}
		List<Logic.Character> list2 = list;
		return list2 != null && list2.Contains(c);
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x00115410 File Offset: 0x00113610
	private void AllocateSkillIcons(int num = 0)
	{
		if (this.id_ClassSkills == null)
		{
			return;
		}
		int num2 = num;
		if (num2 == 0)
		{
			Game game = GameLogic.Get(false);
			CharacterClass.Def def = (game != null) ? game.defs.GetBase<CharacterClass.Def>() : null;
			if (def != null)
			{
				num2 = def.skill_slots.Count;
			}
		}
		for (int i = 0; i < num2; i++)
		{
			UISkill component;
			if (this.id_SkillIconPrototype != null)
			{
				GameObject gameObject = global::Common.Spawn(this.id_SkillIconPrototype, this.id_ClassSkills, false, "");
				gameObject.gameObject.SetActive(true);
				component = gameObject.GetComponent<UISkill>();
			}
			else
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "rounded");
				component = ObjectIcon.GetIcon("UISkill", vars, this.id_ClassSkills).GetComponent<UISkill>();
			}
			if (component != null)
			{
				component.ShowAddButton(false);
				this.m_SkilIcons.Add(component);
			}
		}
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x001154F4 File Offset: 0x001136F4
	private void RefreshSkills(bool full)
	{
		if (this.id_ClassSkills == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		bool flag = false;
		if (this.Data.skills != null)
		{
			for (int i = 0; i < this.Data.skills.Count; i++)
			{
				if (this.Data.skills[i] != null)
				{
					flag = true;
				}
			}
		}
		if (this.Data.sex == Logic.Character.Sex.Female || (!flag && this.hideZeroSkills))
		{
			this.id_ClassSkills.gameObject.SetActive(false);
			return;
		}
		this.id_ClassSkills.gameObject.SetActive(true);
		int num = this.Data.NumSkillSlots();
		if (num > this.m_SkilIcons.Count)
		{
			this.AllocateSkillIcons(this.m_SkilIcons.Count - num);
		}
		for (int j = 0; j < this.m_SkilIcons.Count; j++)
		{
			UISkill uiskill = this.m_SkilIcons[j];
			if (j >= num)
			{
				uiskill.SetData(null, null, true, true, full);
				uiskill.gameObject.SetActive(false);
			}
			Skill skill = this.Data.GetSkill(j);
			uiskill.SetData(this.Data, skill, false, true, full);
			uiskill.gameObject.SetActive(true);
			if (skill == null)
			{
				uiskill.OverrideType(this.Data.SkillSlotType(j));
			}
		}
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x0011564C File Offset: 0x0011384C
	private void RefreshStatus()
	{
		if (this.id_Status == null)
		{
			return;
		}
		bool flag = this.Data.IsPope() && BaseUI.LogicKingdom().HasPope();
		bool flag2 = !this.foreign || flag || this.Data.sex == Logic.Character.Sex.Female;
		Logic.Status status = flag ? this.Data.FindStatus<PopeStatus>() : this.Data.GetStatus();
		bool flag3 = flag2;
		object obj;
		if (status == null)
		{
			obj = null;
		}
		else
		{
			Logic.Status.Def def = status.def;
			obj = ((def != null) ? def.field : null);
		}
		flag2 = (flag3 & obj != null);
		this.id_Status.SetActive(flag2);
		if (!flag2)
		{
			return;
		}
		if (this.id_StatusIcon != null)
		{
			Sprite obj2 = global::Defs.GetObj<Sprite>(status.def.field, "icon", status);
			this.id_StatusIcon.overrideSprite = obj2;
			this.id_StatusIcon.gameObject.SetActive(obj2 != null);
		}
		UIText.SetText(this.id_StatusCaption, status.def.field, "name", status, null);
		UIText.SetText(this.id_StatusText, status.def.field, "status_text", status, null);
		this.id_Status.SetActive(true);
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x0011577C File Offset: 0x0011397C
	private void SetAddTextVars(Vars vars)
	{
		vars.Set<bool>("own", !this.foreign);
		vars.Set<bool>("foreign", this.foreign);
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.Data.masters != null && kingdom != null)
		{
			for (int i = 0; i < this.Data.masters.Count; i++)
			{
				Logic.Character character = this.Data.masters[i];
				if (character.GetKingdom() == kingdom)
				{
					vars.Set<Logic.Character>("puppet_of", character);
					return;
				}
			}
		}
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x00115808 File Offset: 0x00113A08
	private void RefreshAddText()
	{
		if (this.id_AddText == null)
		{
			return;
		}
		Vars vars = new Vars(this.Data);
		this.SetAddTextVars(vars);
		string form = null;
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "debug-mode tooltip", false))
		{
			form = "alt";
		}
		string text = global::Defs.Localize(global::Defs.Get(false).dt.Find("CharacterTooltip.add_text", null), vars, form, true, true);
		if (string.IsNullOrEmpty(text))
		{
			this.id_AddText.gameObject.SetActive(false);
			return;
		}
		UIText.SetText(this.id_AddText, text);
		this.id_AddText.gameObject.SetActive(true);
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x001158B8 File Offset: 0x00113AB8
	private void RefreshAdviseLabel()
	{
		if (this.id_AdviseLabel == null)
		{
			return;
		}
		this.id_AdviseLabel.gameObject.SetActive(!this.foreign);
		if (this.foreign)
		{
			return;
		}
		Vars vars = new Vars(this.Data);
		CourtAdvice.SetAdviceVars(this.Data, vars);
		string text = global::Defs.Localize("CharacterTooltip.advise_label", vars, null, true, true);
		if (string.IsNullOrEmpty(text))
		{
			this.id_AdviseLabel.gameObject.SetActive(false);
			return;
		}
		UIText.SetText(this.id_AdviseLabel, text);
		this.id_AdviseLabel.gameObject.SetActive(true);
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x0011595C File Offset: 0x00113B5C
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1615855124U)
		{
			if (num <= 696919718U)
			{
				if (num != 482296307U)
				{
					if (num != 586732532U)
					{
						if (num != 696919718U)
						{
							return;
						}
						if (!(message == "crusader_status_changed"))
						{
							return;
						}
						goto IL_14B;
					}
					else if (!(message == "skills_changed"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "royal_relative_status_changed"))
					{
						return;
					}
					goto IL_14B;
				}
			}
			else if (num != 845558108U)
			{
				if (num != 1153383198U)
				{
					if (num != 1615855124U)
					{
						return;
					}
					if (!(message == "class_changed"))
					{
						return;
					}
					goto IL_14B;
				}
				else
				{
					if (!(message == "got_married"))
					{
						return;
					}
					goto IL_15A;
				}
			}
			else
			{
				if (!(message == "refresh_tags"))
				{
					return;
				}
				goto IL_14B;
			}
		}
		else if (num <= 2935363440U)
		{
			if (num != 2137427461U)
			{
				if (num != 2247867164U)
				{
					if (num != 2935363440U)
					{
						return;
					}
					if (!(message == "character_class_change"))
					{
						return;
					}
					goto IL_14B;
				}
				else if (!(message == "statuses_changed"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "divorce"))
				{
					return;
				}
				goto IL_15A;
			}
		}
		else if (num != 3510854372U)
		{
			if (num != 3780989157U)
			{
				if (num != 4092421942U)
				{
					return;
				}
				if (!(message == "title_changed"))
				{
					return;
				}
				goto IL_14B;
			}
			else
			{
				if (!(message == "kingdom_changed"))
				{
					return;
				}
				goto IL_14B;
			}
		}
		else if (!(message == "status_changed"))
		{
			return;
		}
		this.m_InvalidateStatus = true;
		return;
		IL_14B:
		this.m_InvalidateClass = true;
		this.m_InvalidateName = true;
		return;
		IL_15A:
		this.m_InvalidateMarriage = true;
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x00115ACA File Offset: 0x00113CCA
	private void OnDisable()
	{
		Logic.Character data = this.Data;
		if (data == null)
		{
			return;
		}
		data.DelListener(this);
	}

	// Token: 0x04001344 RID: 4932
	[UIFieldTarget("id_CrestLeft")]
	private UIKingdomIcon id_CrestLeft;

	// Token: 0x04001345 RID: 4933
	[UIFieldTarget("id_ClassColor")]
	private RawImage id_ClassColor;

	// Token: 0x04001346 RID: 4934
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon id_CharacterIcon;

	// Token: 0x04001347 RID: 4935
	[UIFieldTarget("id_CharacterName")]
	private TextMeshProUGUI id_CharacterName;

	// Token: 0x04001348 RID: 4936
	[UIFieldTarget("id_ClassAndAge")]
	private TextMeshProUGUI id_ClassAndAge;

	// Token: 0x04001349 RID: 4937
	[UIFieldTarget("id_ClassLevel")]
	private GameObject id_ClassLevel;

	// Token: 0x0400134A RID: 4938
	[UIFieldTarget("id_KingGeneration")]
	private GameObject m_KingGeneration;

	// Token: 0x0400134B RID: 4939
	[UIFieldTarget("id_KingGenerationLabel")]
	private TextMeshProUGUI m_KingGenerationLabel;

	// Token: 0x0400134C RID: 4940
	[UIFieldTarget("id_Marriage")]
	private TextMeshProUGUI id_Marriage;

	// Token: 0x0400134D RID: 4941
	[UIFieldTarget("id_ClassSkills")]
	private RectTransform id_ClassSkills;

	// Token: 0x0400134E RID: 4942
	[UIFieldTarget("id_SkillIconPrototype")]
	private GameObject id_SkillIconPrototype;

	// Token: 0x0400134F RID: 4943
	[UIFieldTarget("id_Status")]
	private GameObject id_Status;

	// Token: 0x04001350 RID: 4944
	[UIFieldTarget("id_StatusIcon")]
	private Image id_StatusIcon;

	// Token: 0x04001351 RID: 4945
	[UIFieldTarget("id_StatusCaption")]
	private TextMeshProUGUI id_StatusCaption;

	// Token: 0x04001352 RID: 4946
	[UIFieldTarget("id_StatusText")]
	private TextMeshProUGUI id_StatusText;

	// Token: 0x04001353 RID: 4947
	[UIFieldTarget("id_AddText")]
	private TextMeshProUGUI id_AddText;

	// Token: 0x04001354 RID: 4948
	[UIFieldTarget("id_AdviseLabel")]
	private TextMeshProUGUI id_AdviseLabel;

	// Token: 0x04001355 RID: 4949
	[UIFieldTarget("id_Abilities")]
	private UIKingAbilities id_Abilities;

	// Token: 0x04001356 RID: 4950
	private UICharacterTooltip.ClassLevel classLevel;

	// Token: 0x04001357 RID: 4951
	private Logic.Character Data;

	// Token: 0x04001358 RID: 4952
	private Vars tooltipVars;

	// Token: 0x04001359 RID: 4953
	private bool foreign;

	// Token: 0x0400135A RID: 4954
	private List<UISkill> m_SkilIcons = new List<UISkill>();

	// Token: 0x0400135B RID: 4955
	private bool m_InvalidateClass;

	// Token: 0x0400135C RID: 4956
	private bool m_InvalidateMarriage;

	// Token: 0x0400135D RID: 4957
	private bool m_InvalidateStatus;

	// Token: 0x0400135E RID: 4958
	private bool m_InvalidateName;

	// Token: 0x0400135F RID: 4959
	private bool m_Initialzed;

	// Token: 0x04001360 RID: 4960
	private bool hideZeroSkills;

	// Token: 0x02000730 RID: 1840
	internal class ClassLevel : MonoBehaviour
	{
		// Token: 0x06004A0F RID: 18959 RVA: 0x0021F1E4 File Offset: 0x0021D3E4
		private void Init()
		{
			if (this.m_initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_initialzied = true;
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x0021F200 File Offset: 0x0021D400
		public void SetData(Logic.Character character)
		{
			this.Init();
			this.Data = character;
			this.m_lastlevel = this.Data.GetClassLevel();
			base.enabled = (this.Data != null);
			Tooltip.Get(base.gameObject, true).SetDef("ClassLevelTooltip", new Vars(this.Data));
			this.Refresh();
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x0021F266 File Offset: 0x0021D466
		private void Update()
		{
			if (this.Data == null)
			{
				base.enabled = false;
				return;
			}
			if (this.Data.GetClassLevel() != this.m_lastlevel)
			{
				int lastlevel = this.m_lastlevel;
				this.Refresh();
			}
		}

		// Token: 0x06004A12 RID: 18962 RVA: 0x0021F298 File Offset: 0x0021D498
		private void Refresh()
		{
			if (this.id_CharacterLevel != null)
			{
				UIText.SetText(this.id_CharacterLevel, global::Character.GetLevelText(this.Data));
			}
		}

		// Token: 0x040038DF RID: 14559
		[UIFieldTarget("id_CharacterLevelBackground")]
		private Image id_CharacterLevelBackground;

		// Token: 0x040038E0 RID: 14560
		[UIFieldTarget("id_CharacterLevelBorder")]
		private Image id_CharacterLevelBorder;

		// Token: 0x040038E1 RID: 14561
		[UIFieldTarget("id_CharacterLevel")]
		private TextMeshProUGUI id_CharacterLevel;

		// Token: 0x040038E2 RID: 14562
		private Logic.Character Data;

		// Token: 0x040038E3 RID: 14563
		private int m_lastlevel = -1;

		// Token: 0x040038E4 RID: 14564
		private bool m_initialzied;
	}
}
