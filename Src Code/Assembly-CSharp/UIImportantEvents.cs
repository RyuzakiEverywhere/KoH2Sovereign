using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000236 RID: 566
public class UIImportantEvents : MonoBehaviour, IListener
{
	// Token: 0x06002290 RID: 8848 RVA: 0x00139004 File Offset: 0x00137204
	public static void UpdateCategory(string category)
	{
		if (category == "mercenary")
		{
			UIImportantEvents uiimportantEvents = UIImportantEvents.current;
			if (uiimportantEvents != null)
			{
				uiimportantEvents.UpdateLoyalMercenaries();
			}
		}
		if (category == "quests")
		{
			UIImportantEvents uiimportantEvents2 = UIImportantEvents.current;
			if (uiimportantEvents2 == null)
			{
				return;
			}
			uiimportantEvents2.UpdateQuests();
		}
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x0013903F File Offset: 0x0013723F
	private IEnumerator Start()
	{
		yield return null;
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		UIImportantEvents.current = this;
		yield break;
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x00139050 File Offset: 0x00137250
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		try
		{
			UICommon.FindComponents(this, false);
			if (this.m_CrusadeContainer != null)
			{
				GameObject icon = ObjectIcon.GetIcon("Crusade", null, this.m_CrusadeContainer);
				if (icon != null)
				{
					this.crusadeIcon = icon.GetOrAddComponent<UICrusadeIcon>();
				}
			}
			if (this.m_PatriarchContianer != null)
			{
				Vars vars = new Vars();
				vars.Set<string>("variant", "important_message");
				GameObject icon2 = ObjectIcon.GetIcon("Character", vars, this.m_PatriarchContianer);
				if (icon2 != null)
				{
					this.patriarchIcon = icon2.GetComponent<UICharacterIcon>();
				}
			}
			if (this.m_MissingGoodsContainer != null)
			{
				this.missingGoodsIcon = UIMissinGoodsIcon.Create(this.Kingdom, null, this.m_MissingGoodsContainer);
				if (this.missingGoodsIcon != null)
				{
					this.missingGoodsIcon.gameObject.SetActive(false);
				}
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		if (this.m_PatriarchContianer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_PatriarchContianer));
		}
		if (this.m_CrusadeContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_CrusadeContainer));
		}
		if (this.m_RebelsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_RebelsContainer));
		}
		if (this.m_PrisonersContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_PrisonersContainer));
		}
		if (this.m_PactsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_PactsContainer));
		}
		if (this.m_PactsAgainstContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_PactsAgainstContainer));
		}
		if (this.m_WarsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_WarsContainer));
		}
		if (this.m_ThirdPartyJihadsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_ThirdPartyJihadsContainer));
		}
		if (this.m_LoyalMercenariesContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_LoyalMercenariesContainer));
		}
		if (this.m_QuestsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_QuestsContainer));
		}
		if (this.m_MissingGoodsContainer != null)
		{
			this.messageContainerDatas.Add(new UIImportantEvents.MessageContainerData(this.m_MissingGoodsContainer));
		}
		this.m_maxWidth = this.GetMaxAvaliableWidth();
		this.m_Initialzied = true;
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x001392EC File Offset: 0x001374EC
	private float GetMaxAvaliableWidth()
	{
		float width = (base.transform as RectTransform).rect.width;
		float spacing = base.GetComponent<HorizontalLayoutGroup>().spacing;
		return Mathf.Clamp(width - (float)(base.transform.childCount - 1) * spacing, 0f, float.PositiveInfinity);
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x00139340 File Offset: 0x00137540
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			Game game = kingdom2.game;
			if (game != null)
			{
				Logic.Religions religions = game.religions;
				if (religions != null)
				{
					religions.DelListener(this);
				}
			}
		}
		this.Kingdom = k;
		Logic.Kingdom kingdom3 = this.Kingdom;
		if (kingdom3 != null)
		{
			kingdom3.AddListener(this);
		}
		Logic.Kingdom kingdom4 = this.Kingdom;
		if (kingdom4 != null)
		{
			Game game2 = kingdom4.game;
			if (game2 != null)
			{
				Logic.Religions religions2 = game2.religions;
				if (religions2 != null)
				{
					religions2.AddListener(this);
				}
			}
		}
		if (this.missingGoodsIcon != null)
		{
			this.missingGoodsIcon.SetKingdom(k, null);
		}
		this.Refresh();
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x001393F0 File Offset: 0x001375F0
	private void RebuildLayout()
	{
		float num = 48f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.messageContainerDatas.Count; i++)
		{
			UIImportantEvents.MessageContainerData messageContainerData = this.messageContainerDatas[i];
			if (messageContainerData.gameObject.activeSelf)
			{
				LayoutElement layoutElement = messageContainerData.layoutElement;
				if (layoutElement == null || !layoutElement.ignoreLayout)
				{
					messageContainerData.UpdateDesiredSize();
					num3 += messageContainerData.activeIcons;
					num2 += messageContainerData.desiredSize;
					if (messageContainerData.activeIcons > 0)
					{
						num4++;
					}
				}
			}
		}
		bool flag = (this.m_maxWidth - num * (float)num4) / num2 < 1f;
		float num5 = num;
		if (flag)
		{
			float num6 = this.m_maxWidth - num * (float)num4;
			num5 = num * (num6 / ((float)(num3 - num4) * num));
		}
		for (int j = 0; j < this.messageContainerDatas.Count; j++)
		{
			UIImportantEvents.MessageContainerData messageContainerData2 = this.messageContainerDatas[j];
			if (messageContainerData2.gameObject.activeSelf && !(messageContainerData2.layoutElement == null))
			{
				LayoutElement layoutElement2 = messageContainerData2.layoutElement;
				if (layoutElement2 == null || !layoutElement2.ignoreLayout)
				{
					float a = flag ? (num + num5 * (float)(messageContainerData2.activeIcons - 1)) : messageContainerData2.desiredSize;
					messageContainerData2.layoutElement.preferredWidth = Mathf.Max(a, messageContainerData2.minWidth);
					StackableIconsContainer stackableIconsContainer = messageContainerData2.stackableIconsContainer;
					if (stackableIconsContainer != null)
					{
						stackableIconsContainer.Refresh();
					}
				}
			}
		}
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x00139564 File Offset: 0x00137764
	private void UpdateEmptyContainer()
	{
		for (int i = 0; i < this.messageContainerDatas.Count; i++)
		{
			this.messageContainerDatas[i].CheckPopulatedContainer();
		}
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x00139598 File Offset: 0x00137798
	private void Update()
	{
		if (this.Kingdom != BaseUI.LogicKingdom())
		{
			this.SetKingdom(BaseUI.LogicKingdom());
			return;
		}
		if (this.m_LastCrusadeUpdate + this.m_CrusadeUpdateInterval < UnityEngine.Time.unscaledTime)
		{
			this.UpdateCrusader();
			this.m_LastCrusadeUpdate = UnityEngine.Time.unscaledTime;
		}
		if (this.m_invalidateContainers)
		{
			this.UpdateEmptyContainer();
			this.RebuildLayout();
			this.m_invalidateContainers = false;
		}
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x001395FE File Offset: 0x001377FE
	private void Refresh()
	{
		this.UpdateCrusader();
		this.UpdatePatriarch();
		this.UpdatePrisoners();
		this.UpdateWars();
		this.UpdateThirdPartyJihads();
		this.UpdatePacts();
		this.UpdateRebels();
		this.UpdateQuests();
		this.UpdateLoyalMercenaries();
		this.UpdateMissingGoods();
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x0013963C File Offset: 0x0013783C
	private void UpdatePatriarch()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_PatriarchContianer == null)
		{
			return;
		}
		if (this.patriarchIcon == null)
		{
			return;
		}
		Logic.Character character = null;
		if (this.Kingdom.patriarch != null && !this.Kingdom.court.Contains(this.Kingdom.patriarch))
		{
			character = this.Kingdom.patriarch;
		}
		this.patriarchIcon.gameObject.SetActive(character != null);
		if (character != null)
		{
			this.patriarchIcon.SetObject(character, null);
			Vars vars = new Vars(character);
			if (!string.IsNullOrEmpty(global::Religions.GetPatriarchBonusesText(this.Kingdom, "\n")))
			{
				vars.Set<string>("patriarch_effects", "#" + global::Religions.GetPatriarchBonusesText(this.Kingdom, "\n"));
			}
			Tooltip.Get(this.patriarchIcon.gameObject, true).SetDef("NonOrthodoxOwnPatriarchTooltip", vars);
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x00139730 File Offset: 0x00137930
	private void UpdateCrusader()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_CrusadeContainer == null)
		{
			return;
		}
		if (this.crusadeIcon == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		Crusade crusade;
		if (kingdom == null)
		{
			crusade = null;
		}
		else
		{
			Game game = kingdom.game;
			if (game == null)
			{
				crusade = null;
			}
			else
			{
				Logic.Religions religions = game.religions;
				if (religions == null)
				{
					crusade = null;
				}
				else
				{
					Catholic catholic = religions.catholic;
					crusade = ((catholic != null) ? catholic.crusade : null);
				}
			}
		}
		Crusade crusade2 = crusade;
		bool flag = crusade2 != null && crusade2.Validate() == "ok";
		this.crusadeIcon.gameObject.SetActive(flag);
		if (flag && this.crusadeIcon.Crusade != crusade2)
		{
			this.crusadeIcon.SetObject(crusade2, null);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x001397E8 File Offset: 0x001379E8
	private void UpdatePrisoners()
	{
		if (this.m_PrisonersContainer == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		int? num;
		if (kingdom == null)
		{
			num = null;
		}
		else
		{
			RoyalDungeon royal_dungeon = kingdom.royal_dungeon;
			num = ((royal_dungeon != null) ? new int?(royal_dungeon.prisoners.Count) : null);
		}
		int num2 = num ?? 0;
		while (this.prisonerIcons.Count < num2)
		{
			Vars vars = new Vars();
			vars.Set<string>("variant", "important_message");
			GameObject icon = ObjectIcon.GetIcon("Character", vars, this.m_PrisonersContainer);
			if (icon == null)
			{
				break;
			}
			UICharacterIcon component = icon.GetComponent<UICharacterIcon>();
			if (component == null)
			{
				break;
			}
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			float num3 = orAddComponent.preferredHeight / orAddComponent.preferredWidth;
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize * num3;
			component.OnSelect += this.HandleOnPrisonerSelected;
			this.prisonerIcons.Add(component);
		}
		if (this.prisonerIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.prisonerIcons.Count; i++)
		{
			UICharacterIcon uicharacterIcon = this.prisonerIcons[i];
			Logic.Kingdom kingdom2 = this.Kingdom;
			Logic.Character character = (((kingdom2 != null) ? kingdom2.prisoners : null) != null && this.Kingdom.prisoners.Count > i) ? this.Kingdom.prisoners[i] : null;
			uicharacterIcon.SetObject(character, null);
			uicharacterIcon.gameObject.SetActive(character != null);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x00139997 File Offset: 0x00137B97
	private void HandleOnPrisonerStackSelected(UIStackIcon i, PointerEventData e)
	{
		UIRoyalDungeon.ToggleOpen(this.Kingdom);
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x001399A4 File Offset: 0x00137BA4
	private void HandleOnPrisonerSelected(UICharacterIcon icon)
	{
		UIRoyalDungeon.ToggleOpen(this.Kingdom);
		if (((icon != null) ? icon.Data : null) != null)
		{
			BaseUI.PlayVoiceEvent("character_voice:greet_enemy_prisoner", icon.Data);
		}
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x001399CF File Offset: 0x00137BCF
	private void UpdatePacts()
	{
		this.UpdatePacts(Pact.Type.Defensive, false);
		this.UpdatePacts(Pact.Type.Offensive, false);
		this.UpdatePacts(Pact.Type.Defensive, true);
		this.UpdatePacts(Pact.Type.Offensive, true);
		this.m_invalidateContainers = true;
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x001399F8 File Offset: 0x00137BF8
	private void UpdatePacts(Pact.Type type, bool against)
	{
		RectTransform parent;
		List<UIPactIcon> list;
		if (type == Pact.Type.Defensive)
		{
			parent = (against ? this.m_PactsAgainstContainer : this.m_PactsContainer);
			list = (against ? this.defensivePactAgainstIcons : this.defensivePactIcons);
		}
		else
		{
			if (type != Pact.Type.Offensive)
			{
				return;
			}
			parent = (against ? this.m_PactsAgainstContainer : this.m_PactsContainer);
			list = (against ? this.offfensivePactAcainstIcons : this.offfensivePactIcons);
		}
		UIImportantEvents.tmp_pacts.Clear();
		if (against)
		{
			Logic.Kingdom kingdom = this.Kingdom;
			if (kingdom != null)
			{
				kingdom.GetPactsAgainst(type, UIImportantEvents.tmp_pacts);
			}
		}
		else
		{
			Logic.Kingdom kingdom2 = this.Kingdom;
			if (kingdom2 != null)
			{
				kingdom2.GetPacts(type, UIImportantEvents.tmp_pacts);
			}
		}
		while (list.Count < UIImportantEvents.tmp_pacts.Count)
		{
			GameObject icon = ObjectIcon.GetIcon("Pact", null, parent);
			if (icon == null)
			{
				break;
			}
			UIPactIcon component = icon.GetComponent<UIPactIcon>();
			if (component == null)
			{
				break;
			}
			component.OnSelect += this.HandleOnPactSelected;
			list.Add(component);
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			float num = orAddComponent.preferredHeight / orAddComponent.preferredWidth;
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize * num;
		}
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			UIPactIcon uipactIcon = list[i];
			Pact pact = (i < UIImportantEvents.tmp_pacts.Count) ? UIImportantEvents.tmp_pacts[i] : null;
			bool flag = pact != null && pact.IsValid();
			uipactIcon.SetObject(flag ? pact : null, null);
			uipactIcon.ShowOwnerCrest(against);
			uipactIcon.gameObject.SetActive(flag);
		}
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x00139BA5 File Offset: 0x00137DA5
	private void HandleOnPactSelected(UIPactIcon icon)
	{
		UIWarsOverviewWindow.ToggleOpen(this.Kingdom, null, icon.Pact);
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x00139BBC File Offset: 0x00137DBC
	private void UpdateWars()
	{
		if (this.m_WarsContainer == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		int? num;
		if (kingdom == null)
		{
			num = null;
		}
		else
		{
			List<War> wars = kingdom.wars;
			num = ((wars != null) ? new int?(wars.Count) : null);
		}
		int num2 = num ?? 0;
		while (this.warIcons.Count < num2)
		{
			GameObject icon = ObjectIcon.GetIcon("War", null, this.m_WarsContainer);
			if (icon == null)
			{
				break;
			}
			UIWarIcon component = icon.GetComponent<UIWarIcon>();
			if (component == null)
			{
				break;
			}
			component.SetAudioSet("DefaultAudioSetMetal");
			component.OnSelect += this.HandleOnWarSelected;
			this.warIcons.Add(component);
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			float num3 = orAddComponent.preferredHeight / orAddComponent.preferredWidth;
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize * num3;
		}
		if (this.warIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.warIcons.Count; i++)
		{
			UIWarIcon uiwarIcon = this.warIcons[i];
			War war = (num2 > i) ? this.Kingdom.wars[i] : null;
			bool flag = war != null && !war.IsConcluded();
			uiwarIcon.SetObject(flag ? war : null, null);
			uiwarIcon.gameObject.SetActive(flag);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x00139D50 File Offset: 0x00137F50
	private void CalcThirdPartyJihads()
	{
		this.thirdPartyJihadWars.Clear();
		if (this.Kingdom != null)
		{
			for (int i = 0; i < this.Kingdom.game.religions.jihad_kingdoms.Count; i++)
			{
				War jihad = this.Kingdom.game.religions.jihad_kingdoms[i].jihad;
				if (jihad != null && !jihad.attackers.Contains(this.Kingdom) && !jihad.defenders.Contains(this.Kingdom))
				{
					this.thirdPartyJihadWars.Add(jihad);
				}
			}
		}
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x00139DEC File Offset: 0x00137FEC
	private void UpdateThirdPartyJihads()
	{
		if (this.m_ThirdPartyJihadsContainer == null)
		{
			return;
		}
		this.CalcThirdPartyJihads();
		while (this.thirdPartyJihadIcons.Count < this.thirdPartyJihadWars.Count)
		{
			Vars vars = new Vars();
			vars.Set<string>("variant", "thirdPartyJihad");
			GameObject icon = ObjectIcon.GetIcon("War", vars, this.m_ThirdPartyJihadsContainer);
			if (icon == null)
			{
				break;
			}
			UIWarIcon component = icon.GetComponent<UIWarIcon>();
			if (component == null)
			{
				break;
			}
			this.thirdPartyJihadIcons.Add(component);
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			float num = orAddComponent.preferredHeight / orAddComponent.preferredWidth;
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize * num;
		}
		if (this.thirdPartyJihadIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.thirdPartyJihadIcons.Count; i++)
		{
			UIWarIcon uiwarIcon = this.thirdPartyJihadIcons[i];
			War war = (this.thirdPartyJihadWars.Count > i) ? this.thirdPartyJihadWars[i] : null;
			bool flag = war != null && !war.IsConcluded();
			uiwarIcon.SetObject(flag ? war : null, null);
			uiwarIcon.gameObject.SetActive(flag);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x00139F35 File Offset: 0x00138135
	private void HandleOnWarStackSelected(UIStackIcon i, PointerEventData e)
	{
		UIWarsOverviewWindow.ToggleOpen(this.Kingdom, null, null);
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x00139F44 File Offset: 0x00138144
	private void HandleOnWarSelected(UIWarIcon icon)
	{
		UIWarsOverviewWindow.ToggleOpen(this.Kingdom, icon.War, null);
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x00139F58 File Offset: 0x00138158
	private void UpdateRebels()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_RebelsContainer == null)
		{
			return;
		}
		int num = (this.Kingdom.rebellions != null) ? this.Kingdom.rebellions.Count : 0;
		while (this.rebelionIcons.Count < num)
		{
			Vars vars = new Vars();
			vars.Set<string>("variant", "important_message");
			GameObject icon = ObjectIcon.GetIcon("Rebellion", vars, this.m_RebelsContainer);
			if (icon == null)
			{
				break;
			}
			UIRebellionIcon component = icon.GetComponent<UIRebellionIcon>();
			if (component == null)
			{
				break;
			}
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize;
			component.OnSelect += this.HandleOnRebelionSelected;
			this.rebelionIcons.Add(component);
		}
		if (this.rebelionIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.rebelionIcons.Count; i++)
		{
			UIRebellionIcon uirebellionIcon = this.rebelionIcons[i];
			Rebellion rebellion = (num > i) ? this.Kingdom.rebellions[i] : null;
			Vars vars2 = null;
			if (rebellion != null)
			{
				vars2 = new Vars(rebellion);
				vars2.Set<bool>("show_inspect_hint", true);
			}
			uirebellionIcon.SetObject(rebellion, vars2);
			uirebellionIcon.gameObject.SetActive(rebellion != null);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x0013A0C0 File Offset: 0x001382C0
	private void HandleOnRebelionSelected(UIRebellionIcon icon)
	{
		object obj;
		if (icon == null)
		{
			obj = null;
		}
		else
		{
			Rebellion data = icon.Data;
			if (data == null)
			{
				obj = null;
			}
			else
			{
				Logic.Rebel leader = data.leader;
				if (leader == null)
				{
					obj = null;
				}
				else
				{
					Logic.Army army = leader.army;
					obj = ((army != null) ? army.visuals : null);
				}
			}
		}
		global::Army army2 = obj as global::Army;
		if (army2 == null)
		{
			return;
		}
		if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
		{
			WorldUI.Get().LookAt(army2.transform.position, false);
			return;
		}
		WorldUI.Get().SelectObj(army2.gameObject, false, true, true, true);
		UIRebellions.ToggleOpen(this.Kingdom, icon.Data);
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x0013A164 File Offset: 0x00138364
	private void UpdateQuests()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_QuestsContainer == null)
		{
			return;
		}
		int num = (this.Kingdom.quests != null) ? this.Kingdom.quests.Count : 0;
		while (this.questIcons.Count < num)
		{
			Vars vars = new Vars();
			vars.Set<string>("variant", "important_message");
			GameObject icon = ObjectIcon.GetIcon("Quest", vars, this.m_QuestsContainer);
			if (icon == null)
			{
				break;
			}
			UIQuestIcon component = icon.GetComponent<UIQuestIcon>();
			if (component == null)
			{
				break;
			}
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize;
			component.OnSelect += this.HandleOnQuestSelected;
			this.questIcons.Add(component);
		}
		if (this.questIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.questIcons.Count; i++)
		{
			UIQuestIcon uiquestIcon = this.questIcons[i];
			Quest quest = (num > i) ? this.Kingdom.quests[i] : null;
			bool flag = quest != null && quest.CheckConditions();
			Vars vars2 = null;
			if (quest != null)
			{
				vars2 = new Vars(quest);
				vars2.Set<bool>("show_inspect_hint", true);
				if (flag)
				{
					if (quest.message_icon == null)
					{
						MessageIcon messageIcon = MessageIcon.Create("UniteQuestActivatedMessage", vars2, true, null);
						messageIcon.on_button = new MessageWnd.OnButton(this.HandleOnQuestButton);
						quest.message_icon = messageIcon;
					}
				}
				else
				{
					MessageIcon messageIcon2 = quest.message_icon as MessageIcon;
					if (messageIcon2 != null)
					{
						messageIcon2.Dismiss(true);
					}
					quest.message_icon = null;
				}
			}
			uiquestIcon.SetObject(quest, vars2);
			uiquestIcon.gameObject.SetActive(flag);
		}
		this.m_invalidateContainers = true;
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x0013A344 File Offset: 0x00138544
	private bool HandleOnQuestButton(MessageWnd wnd, string btn_id)
	{
		if (!(btn_id == "accept"))
		{
			if (!(btn_id == "reject"))
			{
				if (btn_id == "decide_later")
				{
					wnd.Close(false);
				}
			}
			else
			{
				Quest quest = wnd.vars.obj.obj_val as Quest;
				if (quest != null)
				{
					this.Kingdom.quests.Remove(quest, true);
					MessageIcon messageIcon = quest.message_icon as MessageIcon;
					if (messageIcon != null)
					{
						messageIcon.Dismiss(true);
					}
					quest.message_icon = null;
				}
				wnd.CloseAndDismiss(true);
			}
		}
		else
		{
			Quest quest2 = wnd.vars.obj.obj_val as Quest;
			if (quest2 != null)
			{
				if (quest2.CheckConditions())
				{
					quest2.Complete();
				}
				MessageIcon messageIcon2 = quest2.message_icon as MessageIcon;
				if (messageIcon2 != null)
				{
					messageIcon2.Dismiss(true);
				}
				quest2.message_icon = null;
			}
			wnd.CloseAndDismiss(true);
		}
		return true;
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x0013A432 File Offset: 0x00138632
	private void HandleOnQuestSelected(UIQuestIcon icon)
	{
		MessageWnd.Create("UniteQuestActivatedMessage", new Vars(icon.logicObject), null, new MessageWnd.OnButton(this.HandleOnQuestButton));
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x0013A458 File Offset: 0x00138658
	private void UpdateLoyalMercenaries()
	{
		if (this.m_LoyalMercenariesContainer == null)
		{
			return;
		}
		this.m_TmpMercList.Clear();
		Mercenary.GetLoyalMercenaries(BaseUI.LogicKingdom(), this.m_TmpMercList);
		int count = this.m_TmpMercList.Count;
		while (this.mercIcons.Count < count)
		{
			Vars vars = new Vars();
			GameObject icon = ObjectIcon.GetIcon("Mercenary", vars, this.m_LoyalMercenariesContainer);
			if (icon == null)
			{
				break;
			}
			UIMercenaryIcon component = icon.GetComponent<UIMercenaryIcon>();
			if (component == null)
			{
				break;
			}
			LayoutElement orAddComponent = component.GetOrAddComponent<LayoutElement>();
			float num = orAddComponent.preferredHeight / orAddComponent.preferredWidth;
			orAddComponent.preferredWidth = this.m_IconSize;
			orAddComponent.preferredHeight = this.m_IconSize * num;
			component.OnSelect += this.HandleMercenarySelected;
			this.mercIcons.Add(component);
		}
		if (this.mercIcons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.mercIcons.Count; i++)
		{
			UIMercenaryIcon uimercenaryIcon = this.mercIcons[i];
			Mercenary mercenary = (this.m_TmpMercList != null && this.m_TmpMercList.Count > i) ? this.m_TmpMercList[i] : null;
			uimercenaryIcon.SetObject(mercenary, null);
			uimercenaryIcon.gameObject.SetActive(mercenary != null);
		}
		this.m_TmpMercList.Clear();
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x0013A5B8 File Offset: 0x001387B8
	private int GetOwnMercenariesCount()
	{
		int num = 0;
		Game game = GameLogic.Get(true);
		Mercenary.Def @base = game.defs.GetBase<Mercenary.Def>();
		if (@base == null)
		{
			return num;
		}
		Logic.Kingdom factionKingdom = FactionUtils.GetFactionKingdom(game, @base.kingdom_key);
		if (factionKingdom == null)
		{
			return num;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		for (int i = 0; i < factionKingdom.armies.Count; i++)
		{
			Mercenary mercenary = factionKingdom.armies[i].mercenary;
			if (mercenary != null && mercenary.IsValid())
			{
				Logic.Army army = mercenary.army;
				if ((army == null || army.IsValid()) && mercenary.former_owner_id == kingdom.id)
				{
					num++;
				}
			}
		}
		for (int j = 0; j < kingdom.mercenaries.Count; j++)
		{
			if (kingdom.mercenaries[j].IsOwnStance(kingdom))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x0013A698 File Offset: 0x00138898
	private void HandleMercenarySelected(UIMercenaryIcon icon)
	{
		object obj;
		if (icon == null)
		{
			obj = null;
		}
		else
		{
			Mercenary mercenary = icon.Mercenary;
			if (mercenary == null)
			{
				obj = null;
			}
			else
			{
				Logic.Army army = mercenary.army;
				obj = ((army != null) ? army.visuals : null);
			}
		}
		global::Army army2 = obj as global::Army;
		if (army2 != null)
		{
			if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
			{
				WorldUI.Get().LookAt(army2.transform.position, false);
			}
			WorldUI.Get().SelectObj(army2.gameObject, false, true, true, true);
		}
		UIHiredMercenaries.ToggleOpen(this.Kingdom, (icon != null) ? icon.Mercenary : null);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x0013A734 File Offset: 0x00138934
	private void HandleOnMercenaryStackSelected(UIStackIcon icon, PointerEventData e)
	{
		Mercenary.GetLoyalMercenaries(BaseUI.LogicKingdom(), this.m_TmpMercList);
		if (this.m_TmpMercList == null || this.m_TmpMercList.Count == 0)
		{
			return;
		}
		Mercenary mercenary = WorldUI.Get().selected_logic_obj as Mercenary;
		Mercenary mercenary2 = null;
		if (mercenary != null)
		{
			for (int i = 0; i < this.m_TmpMercList.Count; i++)
			{
				if (mercenary == this.m_TmpMercList[i])
				{
					mercenary2 = ((this.m_TmpMercList.Count > i + 1) ? this.m_TmpMercList[i + 1] : this.m_TmpMercList[0]);
				}
			}
		}
		if (mercenary2 == null)
		{
			mercenary2 = this.m_TmpMercList[0];
		}
		object obj;
		if (mercenary2 == null)
		{
			obj = null;
		}
		else
		{
			Logic.Army army = mercenary2.army;
			obj = ((army != null) ? army.visuals : null);
		}
		global::Army army2 = obj as global::Army;
		if (army2 != null)
		{
			if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
			{
				WorldUI.Get().LookAt(army2.transform.position, false);
			}
			WorldUI.Get().SelectObj(army2.gameObject, false, true, true, true);
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x0013A849 File Offset: 0x00138A49
	private void UpdateMissingGoods()
	{
		if (this.m_MissingGoodsContainer == null)
		{
			return;
		}
		if (this.missingGoodsIcon != null)
		{
			this.missingGoodsIcon.gameObject.SetActive(this.missingGoodsIcon.HasMissingGoods());
		}
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x0013A884 File Offset: 0x00138A84
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2261655967U)
		{
			if (num > 363871432U)
			{
				if (num <= 1203136459U)
				{
					if (num <= 623326961U)
					{
						if (num != 526064098U)
						{
							if (num != 623326961U)
							{
								return;
							}
							if (!(message == "prison_changed"))
							{
								return;
							}
							using (Game.Profile("UIImporantEvents UpdatePrisoners", false, 0f, null))
							{
								this.UpdatePrisoners();
								return;
							}
						}
						else
						{
							if (!(message == "quest_changed"))
							{
								return;
							}
							this.UpdateQuests();
							return;
						}
					}
					else if (num != 708058303U)
					{
						if (num != 1203136459U)
						{
							return;
						}
						if (!(message == "rebel_type_changed"))
						{
							return;
						}
						goto IL_453;
					}
					else if (!(message == "crusade_ended"))
					{
						return;
					}
				}
				else if (num <= 1700710542U)
				{
					if (num != 1319443009U)
					{
						if (num != 1700710542U)
						{
							return;
						}
						if (!(message == "rebellion_ended"))
						{
							return;
						}
						goto IL_453;
					}
					else if (!(message == "new_crusade"))
					{
						return;
					}
				}
				else if (num != 2105032289U)
				{
					if (num != 2186266987U)
					{
						if (num != 2261655967U)
						{
							return;
						}
						if (!(message == "war_started"))
						{
							return;
						}
						goto IL_43E;
					}
					else
					{
						if (!(message == "subordinated"))
						{
							return;
						}
						goto IL_49D;
					}
				}
				else
				{
					if (!(message == "religion_changed"))
					{
						return;
					}
					goto IL_49D;
				}
				this.UpdateCrusader();
				return;
			}
			if (num <= 150182469U)
			{
				if (num <= 66019599U)
				{
					if (num != 819938U)
					{
						if (num != 66019599U)
						{
							return;
						}
						if (!(message == "del_pact_against"))
						{
							return;
						}
						goto IL_445;
					}
					else
					{
						if (!(message == "realm_deleted"))
						{
							return;
						}
						goto IL_490;
					}
				}
				else if (num != 98381933U)
				{
					if (num != 150182469U)
					{
						return;
					}
					if (!(message == "rebellion_leader_started"))
					{
						return;
					}
					goto IL_453;
				}
				else
				{
					if (!(message == "left_war"))
					{
						return;
					}
					goto IL_43E;
				}
			}
			else if (num <= 274589411U)
			{
				if (num != 191321419U)
				{
					if (num != 274589411U)
					{
						return;
					}
					if (!(message == "del_pact"))
					{
						return;
					}
					goto IL_445;
				}
				else
				{
					if (!(message == "court_changed"))
					{
						return;
					}
					goto IL_489;
				}
			}
			else if (num != 298126040U)
			{
				if (num != 363871432U)
				{
					return;
				}
				if (!(message == "buildings_missing_resources_change"))
				{
					return;
				}
				this.UpdateMissingGoods();
			}
			else
			{
				if (!(message == "rebellion_new_leader"))
				{
					return;
				}
				goto IL_453;
			}
			return;
		}
		if (num <= 2818635807U)
		{
			if (num <= 2568120518U)
			{
				if (num <= 2314870149U)
				{
					if (num != 2275136621U)
					{
						if (num != 2314870149U)
						{
							return;
						}
						if (!(message == "reveal_pact"))
						{
							return;
						}
						goto IL_445;
					}
					else
					{
						if (!(message == "realm_added"))
						{
							return;
						}
						goto IL_490;
					}
				}
				else if (num != 2500072957U)
				{
					if (num != 2568120518U)
					{
						return;
					}
					if (!(message == "jihad_changed"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "add_pact_against"))
					{
						return;
					}
					goto IL_445;
				}
			}
			else if (num <= 2642120633U)
			{
				if (num != 2599339670U)
				{
					if (num != 2642120633U)
					{
						return;
					}
					if (!(message == "rebellions_changed"))
					{
						return;
					}
					goto IL_453;
				}
				else
				{
					if (!(message == "autocephaly"))
					{
						return;
					}
					goto IL_49D;
				}
			}
			else if (num != 2652411498U)
			{
				if (num != 2818635807U)
				{
					return;
				}
				if (!(message == "infiltrated"))
				{
					return;
				}
				goto IL_445;
			}
			else
			{
				if (!(message == "war_ended"))
				{
					return;
				}
				goto IL_43E;
			}
		}
		else if (num <= 3715214635U)
		{
			if (num <= 3079482253U)
			{
				if (num != 2842647092U)
				{
					if (num != 3079482253U)
					{
						return;
					}
					if (!(message == "joined_war"))
					{
						return;
					}
					goto IL_43E;
				}
				else
				{
					if (!(message == "patriarch_changed"))
					{
						return;
					}
					goto IL_489;
				}
			}
			else if (num != 3448667035U)
			{
				if (num != 3715214635U)
				{
					return;
				}
				if (!(message == "end_jihad"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "excommunicated"))
				{
					return;
				}
				goto IL_49D;
			}
		}
		else if (num <= 3917709380U)
		{
			if (num != 3771806931U)
			{
				if (num != 3917709380U)
				{
					return;
				}
				if (!(message == "new_jihad"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "conceal_pact"))
				{
					return;
				}
				goto IL_445;
			}
		}
		else if (num != 4023029508U)
		{
			if (num != 4137619576U)
			{
				if (num != 4192422753U)
				{
					return;
				}
				if (!(message == "add_pact"))
				{
					return;
				}
				goto IL_445;
			}
			else
			{
				if (!(message == "unexcommunicated"))
				{
					return;
				}
				goto IL_49D;
			}
		}
		else
		{
			if (!(message == "rebellion_zone_joined"))
			{
				return;
			}
			goto IL_453;
		}
		this.UpdateThirdPartyJihads();
		return;
		IL_43E:
		this.UpdateWars();
		return;
		IL_445:
		this.UpdatePacts();
		return;
		IL_453:
		this.UpdateRebels();
		return;
		IL_489:
		this.UpdatePatriarch();
		return;
		IL_490:
		this.UpdateQuests();
		this.UpdatePatriarch();
		return;
		IL_49D:
		this.UpdateQuests();
		this.UpdatePatriarch();
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x0013AD58 File Offset: 0x00138F58
	private void OnDestroy()
	{
		if (UIImportantEvents.current == this)
		{
			UIImportantEvents.current = null;
		}
	}

	// Token: 0x04001720 RID: 5920
	[UIFieldTarget("id_PatriarchContianer")]
	private RectTransform m_PatriarchContianer;

	// Token: 0x04001721 RID: 5921
	[UIFieldTarget("id_CrusadeContainer")]
	private RectTransform m_CrusadeContainer;

	// Token: 0x04001722 RID: 5922
	[UIFieldTarget("id_RebelsContainer")]
	private RectTransform m_RebelsContainer;

	// Token: 0x04001723 RID: 5923
	[UIFieldTarget("id_QuestsContainer")]
	private RectTransform m_QuestsContainer;

	// Token: 0x04001724 RID: 5924
	[UIFieldTarget("id_PrisonersContainer")]
	private RectTransform m_PrisonersContainer;

	// Token: 0x04001725 RID: 5925
	[UIFieldTarget("id_PactsContainer")]
	private RectTransform m_PactsContainer;

	// Token: 0x04001726 RID: 5926
	[UIFieldTarget("id_PactsAgainstContainer")]
	private RectTransform m_PactsAgainstContainer;

	// Token: 0x04001727 RID: 5927
	[UIFieldTarget("id_WarsContainer")]
	private RectTransform m_WarsContainer;

	// Token: 0x04001728 RID: 5928
	[UIFieldTarget("id_ThirdPartyJihadsContainer")]
	private RectTransform m_ThirdPartyJihadsContainer;

	// Token: 0x04001729 RID: 5929
	[UIFieldTarget("id_LoyalMercenaries")]
	private RectTransform m_LoyalMercenariesContainer;

	// Token: 0x0400172A RID: 5930
	[UIFieldTarget("id_MissingGoodsContainer")]
	private RectTransform m_MissingGoodsContainer;

	// Token: 0x0400172B RID: 5931
	public Logic.Kingdom Kingdom;

	// Token: 0x0400172C RID: 5932
	[SerializeField]
	private float m_IconSize = 60f;

	// Token: 0x0400172D RID: 5933
	private List<UIImportantEvents.MessageContainerData> messageContainerDatas = new List<UIImportantEvents.MessageContainerData>();

	// Token: 0x0400172E RID: 5934
	private List<UIWarIcon> warIcons = new List<UIWarIcon>();

	// Token: 0x0400172F RID: 5935
	private List<UIWarIcon> thirdPartyJihadIcons = new List<UIWarIcon>();

	// Token: 0x04001730 RID: 5936
	private List<War> thirdPartyJihadWars = new List<War>();

	// Token: 0x04001731 RID: 5937
	private List<UICharacterIcon> prisonerIcons = new List<UICharacterIcon>();

	// Token: 0x04001732 RID: 5938
	private List<UIRebellionIcon> rebelionIcons = new List<UIRebellionIcon>();

	// Token: 0x04001733 RID: 5939
	private List<UIQuestIcon> questIcons = new List<UIQuestIcon>();

	// Token: 0x04001734 RID: 5940
	private List<UIMercenaryIcon> mercIcons = new List<UIMercenaryIcon>();

	// Token: 0x04001735 RID: 5941
	private List<UIPactIcon> defensivePactIcons = new List<UIPactIcon>();

	// Token: 0x04001736 RID: 5942
	private List<UIPactIcon> defensivePactAgainstIcons = new List<UIPactIcon>();

	// Token: 0x04001737 RID: 5943
	private List<UIPactIcon> offfensivePactIcons = new List<UIPactIcon>();

	// Token: 0x04001738 RID: 5944
	private List<UIPactIcon> offfensivePactAcainstIcons = new List<UIPactIcon>();

	// Token: 0x04001739 RID: 5945
	private UICharacterIcon patriarchIcon;

	// Token: 0x0400173A RID: 5946
	private UICrusadeIcon crusadeIcon;

	// Token: 0x0400173B RID: 5947
	private UIMissinGoodsIcon missingGoodsIcon;

	// Token: 0x0400173C RID: 5948
	private float m_maxWidth = 300f;

	// Token: 0x0400173D RID: 5949
	private bool m_Initialzied;

	// Token: 0x0400173E RID: 5950
	private static UIImportantEvents current;

	// Token: 0x0400173F RID: 5951
	private float m_LastCrusadeUpdate;

	// Token: 0x04001740 RID: 5952
	private float m_CrusadeUpdateInterval = 2f;

	// Token: 0x04001741 RID: 5953
	private bool m_invalidateContainers;

	// Token: 0x04001742 RID: 5954
	private static List<Pact> tmp_pacts = new List<Pact>();

	// Token: 0x04001743 RID: 5955
	private List<Mercenary> m_TmpMercList = new List<Mercenary>();

	// Token: 0x02000791 RID: 1937
	private class MessageContainerData
	{
		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06004C98 RID: 19608 RVA: 0x0022A1DC File Offset: 0x002283DC
		// (set) Token: 0x06004C99 RID: 19609 RVA: 0x0022A1E4 File Offset: 0x002283E4
		public float desiredSize { get; private set; }

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06004C9A RID: 19610 RVA: 0x0022A1ED File Offset: 0x002283ED
		// (set) Token: 0x06004C9B RID: 19611 RVA: 0x0022A1F5 File Offset: 0x002283F5
		public int activeIcons { get; private set; }

		// Token: 0x06004C9C RID: 19612 RVA: 0x0022A200 File Offset: 0x00228400
		public MessageContainerData(RectTransform rt)
		{
			this.rectTransfrom = rt;
			this.gameObject = ((rt != null) ? rt.gameObject : null);
			GameObject gameObject = this.gameObject;
			this.layoutElement = ((gameObject != null) ? gameObject.GetComponent<LayoutElement>() : null);
			GameObject gameObject2 = this.gameObject;
			this.stackableIconsContainer = ((gameObject2 != null) ? gameObject2.GetComponent<StackableIconsContainer>() : null);
			LayoutElement layoutElement = this.layoutElement;
			this.minWidth = ((layoutElement != null) ? layoutElement.minWidth : this.minWidth);
		}

		// Token: 0x06004C9D RID: 19613 RVA: 0x0022A284 File Offset: 0x00228484
		public void UpdateDesiredSize()
		{
			this.desiredSize = 0f;
			this.activeIcons = 0;
			if (this.rectTransfrom == null)
			{
				return;
			}
			if (this.rectTransfrom.childCount == 0)
			{
				return;
			}
			float num = 0f;
			int i = 0;
			int childCount = this.rectTransfrom.childCount;
			while (i < childCount)
			{
				RectTransform rectTransform = this.rectTransfrom.GetChild(i) as RectTransform;
				if (rectTransform.gameObject.activeSelf)
				{
					LayoutElement component = rectTransform.GetComponent<LayoutElement>();
					if (component == null || !component.ignoreLayout)
					{
						num += rectTransform.rect.width;
						int activeIcons = this.activeIcons;
						this.activeIcons = activeIcons + 1;
					}
				}
				i++;
			}
			this.desiredSize = Mathf.Max(num, this.minWidth);
		}

		// Token: 0x06004C9E RID: 19614 RVA: 0x0022A348 File Offset: 0x00228548
		public void CheckPopulatedContainer()
		{
			if (this.rectTransfrom == null)
			{
				return;
			}
			if (this.rectTransfrom.childCount == 0)
			{
				this.rectTransfrom.gameObject.SetActive(false);
				return;
			}
			int i = 0;
			int childCount = this.rectTransfrom.childCount;
			while (i < childCount)
			{
				Transform child = this.rectTransfrom.GetChild(i);
				LayoutElement component = child.GetComponent<LayoutElement>();
				if ((component == null || !component.ignoreLayout) && child.gameObject.activeSelf)
				{
					this.rectTransfrom.gameObject.SetActive(true);
					return;
				}
				i++;
			}
			this.rectTransfrom.gameObject.SetActive(false);
		}

		// Token: 0x04003B1D RID: 15133
		public GameObject gameObject;

		// Token: 0x04003B1E RID: 15134
		public RectTransform rectTransfrom;

		// Token: 0x04003B1F RID: 15135
		public LayoutElement layoutElement;

		// Token: 0x04003B20 RID: 15136
		public StackableIconsContainer stackableIconsContainer;

		// Token: 0x04003B21 RID: 15137
		public float minWidth = 48f;
	}
}
