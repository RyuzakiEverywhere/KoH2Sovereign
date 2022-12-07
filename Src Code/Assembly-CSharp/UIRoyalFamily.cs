using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class UIRoyalFamily : UIWindow, IListener
{
	// Token: 0x060028FD RID: 10493 RVA: 0x0015D1B6 File Offset: 0x0015B3B6
	public override string GetDefId()
	{
		return UIRoyalFamily.def_id;
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060028FE RID: 10494 RVA: 0x0015D1BD File Offset: 0x0015B3BD
	// (set) Token: 0x060028FF RID: 10495 RVA: 0x0015D1C5 File Offset: 0x0015B3C5
	public Logic.RoyalFamily Data { get; private set; }

	// Token: 0x06002900 RID: 10496 RVA: 0x0015D1D0 File Offset: 0x0015B3D0
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_TraditionsHost != null)
		{
			this.traditionsPanel = this.m_TraditionsHost.AddComponent<UIKingdomTraditions>();
		}
		if (this.m_RelativesContainer != null)
		{
			global::Common.FindChildrenWithComponent<UICharacterIcon>(this.m_RelativesContainer, this.m_Relatives);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x0015D234 File Offset: 0x0015B434
	private void SetData(Logic.RoyalFamily royalFamilily)
	{
		this.Init();
		this.RemoveListeners();
		this.Data = royalFamilily;
		if (this.traditionsPanel != null)
		{
			this.traditionsPanel.SetData(royalFamilily.GetKingdom());
		}
		this.AddListeners();
		UIKingAbilities kingAbilities = this.m_KingAbilities;
		if (kingAbilities != null)
		{
			Logic.RoyalFamily data = this.Data;
			kingAbilities.SetData((data != null) ? data.Sovereign : null, null);
		}
		this.Refresh();
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x0015D2A3 File Offset: 0x0015B4A3
	protected override void Update()
	{
		base.Update();
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x0015D2C0 File Offset: 0x0015B4C0
	private void HandleOnLogic(object arg1, string arg2, object arg3)
	{
		if (base.isActiveAndEnabled)
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x0015D2D4 File Offset: 0x0015B4D4
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		UIKingAbilities kingAbilities = this.m_KingAbilities;
		if (kingAbilities != null)
		{
			Logic.RoyalFamily data = this.Data;
			kingAbilities.SetData((data != null) ? data.Sovereign : null, null);
		}
		this.PopulateMisc();
		this.PopulateKing();
		this.UpdateClassLevel();
		this.UpdateAge();
		this.UpdateSpouse();
		this.PopulateChildren();
		this.PopulateRelatives();
		this.RefreshKingGeneration();
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x0015D340 File Offset: 0x0015B540
	private void PopulateMisc()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.TMP_Caption != null)
		{
			global::Kingdom kingdom = global::Kingdom.Get(this.Data.kingdom_id);
			if (kingdom != null)
			{
				UIText.SetTextKey(this.TMP_Caption, "RoyalFamilyWindow.caption", new Vars(kingdom.logic), null);
			}
		}
		if (this.m_Crest != null)
		{
			this.m_Crest.SetObject(this.Data.GetKingdom(), null);
		}
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x0015D3C0 File Offset: 0x0015B5C0
	private void PopulateKing()
	{
		if (this.Data.Sovereign != null)
		{
			if (this.Icon_King != null)
			{
				this.Icon_King.ShowCrest(false);
				this.Icon_King.ShowMissonKingdomCrest(true);
				this.Icon_King.ShowPrisonKingdomCrest(true);
				this.Icon_King.ShowStatus(false);
				this.Icon_King.SetObject(this.Data.Sovereign, null);
			}
			UIText.SetTextKey(this.TMP_KingName, "Character.name", new Vars(this.Data.Sovereign), null);
			UIText.SetTextKey(this.TMP_KingAge, "Character.age." + this.Data.Sovereign.age.ToString(), null, null);
			UIText.SetTextKey(this.m_ClassName, this.Data.Sovereign.class_title, null, null);
		}
		if (this.m_AgeWarrning)
		{
			Tooltip.Get(this.m_AgeWarrning, true).SetDef("CharacterAgeWarrningTooltip", new Vars(this.Data.Sovereign));
		}
		if (this.m_ClassLevelIcon != null)
		{
			Tooltip tooltip = Tooltip.Get(this.m_ClassLevelIcon, true);
			string text = "ClassLevelTooltip";
			Logic.RoyalFamily data = this.Data;
			tooltip.SetDef(text, new Vars((data != null) ? data.Sovereign : null));
		}
		if (this.m_KingGenerationIcon != null)
		{
			Tooltip tooltip2 = Tooltip.Get(this.m_KingGenerationIcon, true);
			string text2 = "GenerationsPassedTooltip";
			Logic.RoyalFamily data2 = this.Data;
			tooltip2.SetDef(text2, new Vars((data2 != null) ? data2.Sovereign : null));
		}
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x0015D560 File Offset: 0x0015B760
	private void UpdateClassLevel()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_KingClassLevel == null)
		{
			return;
		}
		if (this.Data.Sovereign != null)
		{
			UIText.SetText(this.m_KingClassLevel, global::Character.GetLevelText(this.Data.Sovereign));
		}
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x0015D5B0 File Offset: 0x0015B7B0
	private void RefreshKingGeneration()
	{
		if (this.Data == null)
		{
			return;
		}
		bool flag = global::Defs.GetBool("RoyalFamilyWindow", "always_show_king_generation", null, false);
		if (!flag)
		{
			flag = (this.Data.game.rules.time_limits.type == Game.CampaignRules.TimeLimits.Type.Generations);
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

	// Token: 0x06002909 RID: 10505 RVA: 0x0015D648 File Offset: 0x0015B848
	private void UpdateAge()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.Sovereign == null)
		{
			return;
		}
		string key = "Character.age." + this.Data.Sovereign.age.ToString();
		UIText.SetTextKey(this.TMP_KingAge, key, null, null);
		if (this.m_AgeWarrning != null)
		{
			this.m_AgeWarrning.gameObject.SetActive(this.Data.Sovereign.age == Logic.Character.Age.Venerable);
		}
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x0015D6D4 File Offset: 0x0015B8D4
	private void UpdateSpouse()
	{
		Logic.RoyalFamily data = this.Data;
		Logic.Character character;
		if (data == null)
		{
			character = null;
		}
		else
		{
			Logic.Kingdom kingdom = data.GetKingdom();
			character = ((kingdom != null) ? kingdom.royalFamily.Spouse : null);
		}
		Logic.Character character2 = character;
		if (this.m_SpouseIcon != null)
		{
			this.m_SpouseIcon.SetObject(character2, null);
			this.m_SpouseIcon.ShowCrest(false);
			this.m_SpouseIcon.ShowStatus(false);
			this.m_SpouseIcon.gameObject.SetActive(character2 != null);
		}
		if (this.m_QueenOriginKingdom != null)
		{
			this.m_QueenOriginKingdom.gameObject.SetActive(character2 != null && character2.original_kingdom_id != character2.GetKingdom().id);
			ObjectIcon queenOriginKingdom = this.m_QueenOriginKingdom;
			object obj;
			if (character2 == null)
			{
				obj = null;
			}
			else
			{
				Game game = character2.game;
				obj = ((game != null) ? game.GetKingdom(character2.original_kingdom_id) : null);
			}
			queenOriginKingdom.SetObject(obj, null);
		}
		if (this.m_SpouseName != null)
		{
			this.m_SpouseName.gameObject.SetActive(character2 != null);
			UIText.SetTextKey(this.m_SpouseName, "Character.title_name", character2, null);
		}
		if (this.m_SpouseOriginTitle != null)
		{
			this.m_SpouseOriginTitle.gameObject.SetActive(false);
		}
		if (this.m_SpouseAditinalData != null)
		{
			this.m_SpouseAditinalData.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x0015D824 File Offset: 0x0015BA24
	private void PopulateChildren()
	{
		if (this.m_ChildrenContainer == null)
		{
			return;
		}
		Logic.RoyalFamily data = this.Data;
		int? num = (data != null) ? new int?(data.MaxChildren()) : null;
		if (num == null)
		{
			num = new int?(4);
		}
		Logic.RoyalFamily data2 = this.Data;
		List<Logic.Character> list;
		if (data2 == null)
		{
			list = null;
		}
		else
		{
			Logic.Kingdom kingdom = data2.GetKingdom();
			list = ((kingdom != null) ? kingdom.royalFamily.Children : null);
		}
		List<Logic.Character> list2 = list;
		UICharacterIcon[] componentsInChildren = this.m_ChildrenContainer.GetComponentsInChildren<UICharacterIcon>();
		GameObject childrenSpouseContianer = this.m_ChildrenSpouseContianer;
		UICharacterIcon[] array = (childrenSpouseContianer != null) ? childrenSpouseContianer.GetComponentsInChildren<UICharacterIcon>() : null;
		UIActionIcon[] componentsInChildren2 = this.m_HairSelectionButtonsContianer.GetComponentsInChildren<UIActionIcon>();
		for (int i = 0; i < num.Value; i++)
		{
			Logic.Character character = (list2.Count > i) ? list2[i] : null;
			UICharacterIcon uicharacterIcon;
			if (componentsInChildren != null && componentsInChildren.Length > i)
			{
				uicharacterIcon = componentsInChildren[i];
				uicharacterIcon.SetObject(character, null);
				uicharacterIcon.ShowStatus(false);
			}
			else
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "royal_family");
				if (character != null)
				{
					GameObject icon = ObjectIcon.GetIcon(character, vars, this.m_ChildrenContainer.transform as RectTransform);
					uicharacterIcon = ((icon != null) ? icon.GetComponent<UICharacterIcon>() : null);
				}
				else
				{
					GameObject icon2 = ObjectIcon.GetIcon("Character", vars, this.m_ChildrenContainer.transform as RectTransform);
					uicharacterIcon = ((icon2 != null) ? icon2.GetComponent<UICharacterIcon>() : null);
				}
				if (uicharacterIcon != null)
				{
					uicharacterIcon.ShowStatus(false);
				}
			}
			if (array != null && i < array.Length)
			{
				Logic.Character character2 = (character != null) ? character.GetSpouse() : null;
				array[i].SetObject(character2, null);
				if (character2 != null)
				{
					array[i].ShowStatus(false);
					array[i].ShowMissonKingdomCrest(false);
					UIKingdomIcon component = global::Common.FindChildByName(array[i].gameObject, "_Crest", true, true).GetComponent<UIKingdomIcon>();
					if (character2.IsRoyalChild() || character2.sex == Logic.Character.Sex.Male)
					{
						component.SetObject(character2.GetOriginalKingdom(), null);
					}
					else
					{
						component.gameObject.SetActive(false);
					}
				}
				if (character == null || (character2 == null && !character.CanMarry()))
				{
					global::Common.FindChildByName(array[i].gameObject, "Group_Empty", true, true).SetActive(false);
					global::Common.FindChildByName(array[i].gameObject, "Group_Populated", true, true).SetActive(false);
				}
			}
			if (uicharacterIcon != null)
			{
				uicharacterIcon.ShowCrest(false);
			}
			if (componentsInChildren2 != null && componentsInChildren2.Length > i)
			{
				UIActionIcon uiactionIcon = componentsInChildren2[i];
				GameObject gameObject = global::Common.FindChildByName(uiactionIcon.gameObject, "id_Active", true, true);
				GameObject gameObject2 = global::Common.FindChildByName(uiactionIcon.gameObject, "id_Empty", true, true);
				if (character == null || character.sex != Logic.Character.Sex.Male)
				{
					uiactionIcon.SetObject(null, null);
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
					if (gameObject2 != null)
					{
						gameObject2.SetActive(false);
					}
				}
				else
				{
					Logic.Character character3 = character;
					Logic.Kingdom kingdom2 = character.GetKingdom();
					object obj;
					if (kingdom2 == null)
					{
						obj = null;
					}
					else
					{
						Logic.RoyalFamily royalFamily = kingdom2.royalFamily;
						obj = ((royalFamily != null) ? royalFamily.Heir : null);
					}
					bool flag = character3 == obj;
					Action action;
					if (character == null)
					{
						action = null;
					}
					else
					{
						Actions actions = character.actions;
						action = ((actions != null) ? actions.Find("ChangeHeirAction") : null);
					}
					Action obj2 = action;
					if (gameObject != null)
					{
						gameObject.SetActive(flag);
					}
					if (gameObject2 != null)
					{
						gameObject2.SetActive(!flag);
					}
					uiactionIcon.SetObject(obj2, null);
				}
			}
		}
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x0015DB80 File Offset: 0x0015BD80
	private void PopulateRelatives()
	{
		if (this.m_RelativesLabel != null)
		{
			UIText.SetTextKey(this.m_RelativesLabel, "RoyalFamilyWindow.relatives_label", this.Data.GetKingdom(), null);
		}
		if (this.m_RelativesDescription != null)
		{
			UIText.SetTextKey(this.m_RelativesDescription, "RoyalFamilyWindow.relatives_description", this.Data.GetKingdom(), null);
		}
		int num = Mathf.Max(new int[]
		{
			this.Data.Relatives.Count,
			this.m_MinRelativesSlotCount,
			this.m_Relatives.Count
		});
		for (int i = 0; i < num; i++)
		{
			Logic.Character character = (this.Data.Relatives.Count > i) ? this.Data.Relatives[i] : null;
			UICharacterIcon uicharacterIcon = null;
			if (this.m_Relatives != null && this.m_Relatives.Count > i)
			{
				uicharacterIcon = this.m_Relatives[i];
				uicharacterIcon.SetObject(character, null);
			}
			else if (character != null)
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "royal_family");
				GameObject icon = ObjectIcon.GetIcon(character, vars, this.m_RelativesContainer.transform as RectTransform);
				uicharacterIcon = ((icon != null) ? icon.GetComponent<UICharacterIcon>() : null);
				uicharacterIcon.SetObject(character, null);
				this.m_Relatives.Add(uicharacterIcon);
			}
			if (uicharacterIcon != null)
			{
				uicharacterIcon.ShowCrest(false);
			}
			if (uicharacterIcon != null)
			{
				uicharacterIcon.ShowStatus(false);
			}
		}
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x0015DCF0 File Offset: 0x0015BEF0
	private void AddListeners()
	{
		if (this.Data != null && this.Data.visuals != null && this.Data.visuals is global::RoyalFamily)
		{
			(this.Data.visuals as global::RoyalFamily).onLogic += this.HandleOnLogic;
		}
		if (this.Button_Close != null)
		{
			for (int i = 0; i < this.Button_Close.Length; i++)
			{
				this.Button_Close[i].onClick = new BSGButton.OnClick(this.HandleOnCloseButtonClick);
			}
		}
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x0015DD90 File Offset: 0x0015BF90
	private void RemoveListeners()
	{
		if (this.Data != null && this.Data.visuals != null && this.Data.visuals is global::RoyalFamily)
		{
			(this.Data.visuals as global::RoyalFamily).onLogic -= this.HandleOnLogic;
		}
		if (this.Button_Close != null)
		{
			for (int i = 0; i < this.Button_Close.Length; i++)
			{
				this.Button_Close[i].onClick = null;
			}
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButtonClick(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x0015DE24 File Offset: 0x0015C024
	public void OnMessage(object obj, string message, object param)
	{
		if (obj is Logic.Character)
		{
			if (message == "skills_changed")
			{
				this.UpdateClassLevel();
				return;
			}
			if (message == "character_age_change")
			{
				this.UpdateAge();
				return;
			}
			if (!(message == "character_class_change") && !(message == "title_changed"))
			{
				return;
			}
			this.Refresh();
		}
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x0015DE84 File Offset: 0x0015C084
	protected override void OnDestroy()
	{
		this.RemoveListeners();
		if (this.traditionsPanel != null)
		{
			this.traditionsPanel.Clear();
		}
		if (this.m_KingAbilities != null)
		{
			this.m_KingAbilities.SetData(null, null);
		}
		this.Data = null;
		base.OnDestroy();
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x0015DED8 File Offset: 0x0015C0D8
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIRoyalFamily.def_id, null);
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x0015DEE8 File Offset: 0x0015C0E8
	public static void ToggleOpen(Logic.Kingdom k)
	{
		if (k == null)
		{
			if (UIRoyalFamily.current != null)
			{
				UIRoyalFamily.current.Close(false);
			}
			return;
		}
		if (UIRoyalFamily.current != null)
		{
			UIRoyalFamily uiroyalFamily = UIRoyalFamily.current;
			Logic.Kingdom kingdom;
			if (uiroyalFamily == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.RoyalFamily data = uiroyalFamily.Data;
				kingdom = ((data != null) ? data.GetKingdom() : null);
			}
			if (kingdom == k)
			{
				UIRoyalFamily.current.Close(false);
				return;
			}
			UIRoyalFamily.current.SetData((k != null) ? k.royalFamily : null);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIRoyalFamily.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIRoyalFamily));
				UIRoyalFamily.current = UIRoyalFamily.Create(k.royalFamily, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x0015DFCF File Offset: 0x0015C1CF
	public static bool IsActive()
	{
		return UIRoyalFamily.current != null;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x0015DFDC File Offset: 0x0015C1DC
	public static UIRoyalFamily Create(Logic.RoyalFamily royalFamilily, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (royalFamilily == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
		UIRoyalFamily uiroyalFamily = gameObject.GetComponent<UIRoyalFamily>();
		if (uiroyalFamily == null)
		{
			uiroyalFamily = gameObject.AddComponent<UIRoyalFamily>();
		}
		uiroyalFamily.SetData(royalFamilily);
		uiroyalFamily.on_close = delegate(UIWindow _)
		{
			UIRoyalFamily.current = null;
		};
		uiroyalFamily.Open();
		return uiroyalFamily;
	}

	// Token: 0x04001BB4 RID: 7092
	private static string def_id = "RoyalFamilyWindow";

	// Token: 0x04001BB5 RID: 7093
	[UIFieldTarget("id_Crest")]
	private UIKingdomIcon m_Crest;

	// Token: 0x04001BB6 RID: 7094
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI TMP_Caption;

	// Token: 0x04001BB7 RID: 7095
	[UIFieldTarget("id_KingIcon")]
	private UICharacterIcon Icon_King;

	// Token: 0x04001BB8 RID: 7096
	[UIFieldTarget("id_KingName")]
	private TextMeshProUGUI TMP_KingName;

	// Token: 0x04001BB9 RID: 7097
	[UIFieldTarget("id_KingTitle")]
	private TextMeshProUGUI TMP_KingTitle;

	// Token: 0x04001BBA RID: 7098
	[UIFieldTarget("id_KingAge")]
	private TextMeshProUGUI TMP_KingAge;

	// Token: 0x04001BBB RID: 7099
	[UIFieldTarget("id_AgeWarrning")]
	private GameObject m_AgeWarrning;

	// Token: 0x04001BBC RID: 7100
	[UIFieldTarget("id_ClassLevelIcon")]
	private GameObject m_ClassLevelIcon;

	// Token: 0x04001BBD RID: 7101
	[UIFieldTarget("id_KingClassLevel")]
	private TextMeshProUGUI m_KingClassLevel;

	// Token: 0x04001BBE RID: 7102
	[UIFieldTarget("id_KingGenerationIcon")]
	private GameObject m_KingGenerationIcon;

	// Token: 0x04001BBF RID: 7103
	[UIFieldTarget("id_KingGeneration")]
	private GameObject m_KingGeneration;

	// Token: 0x04001BC0 RID: 7104
	[UIFieldTarget("id_KingGenerationLabel")]
	private TextMeshProUGUI m_KingGenerationLabel;

	// Token: 0x04001BC1 RID: 7105
	[UIFieldTarget("id_SpouseIcon")]
	private UICharacterIcon m_SpouseIcon;

	// Token: 0x04001BC2 RID: 7106
	[UIFieldTarget("id_QueenOriginKingdom")]
	private UIKingdomIcon m_QueenOriginKingdom;

	// Token: 0x04001BC3 RID: 7107
	[UIFieldTarget("id_SpouseName")]
	private TextMeshProUGUI m_SpouseName;

	// Token: 0x04001BC4 RID: 7108
	[UIFieldTarget("id_SpouseOriginTitle")]
	private TextMeshProUGUI m_SpouseOriginTitle;

	// Token: 0x04001BC5 RID: 7109
	[UIFieldTarget("id_SpouseAditinalData")]
	private TextMeshProUGUI m_SpouseAditinalData;

	// Token: 0x04001BC6 RID: 7110
	[UIFieldTarget("id_ClassName")]
	private TextMeshProUGUI m_ClassName;

	// Token: 0x04001BC7 RID: 7111
	[UIFieldTarget("id_ChildrenContainer")]
	private GameObject m_ChildrenContainer;

	// Token: 0x04001BC8 RID: 7112
	[UIFieldTarget("id_ChildrenSpouseContianer")]
	private GameObject m_ChildrenSpouseContianer;

	// Token: 0x04001BC9 RID: 7113
	[UIFieldTarget("id_KingAbilities")]
	private UIKingAbilities m_KingAbilities;

	// Token: 0x04001BCA RID: 7114
	[UIFieldTarget("id_HairSelectionButtonsContianer")]
	private GameObject m_HairSelectionButtonsContianer;

	// Token: 0x04001BCB RID: 7115
	[UIFieldTarget("id_RelativesContainer")]
	private GameObject m_RelativesContainer;

	// Token: 0x04001BCC RID: 7116
	[UIFieldTarget("id_RelativesLabel")]
	private TextMeshProUGUI m_RelativesLabel;

	// Token: 0x04001BCD RID: 7117
	[UIFieldTarget("id_RelativesDescription")]
	private TextMeshProUGUI m_RelativesDescription;

	// Token: 0x04001BCE RID: 7118
	[UIFieldTarget("id_Button_Close", true)]
	private BSGButton[] Button_Close;

	// Token: 0x04001BCF RID: 7119
	[UIFieldTarget("id_Traditions")]
	private GameObject m_TraditionsHost;

	// Token: 0x04001BD0 RID: 7120
	private UIKingdomTraditions traditionsPanel;

	// Token: 0x04001BD1 RID: 7121
	private List<UICharacterIcon> m_Relatives = new List<UICharacterIcon>(4);

	// Token: 0x04001BD3 RID: 7123
	private bool m_Invalidate;

	// Token: 0x04001BD4 RID: 7124
	private int m_MinRelativesSlotCount = 3;

	// Token: 0x04001BD5 RID: 7125
	private static UIRoyalFamily current;
}
