using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AE RID: 686
public class UICastleWindow : ObjectWindow, IPoolable, IVars
{
	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x0016B740 File Offset: 0x00169940
	public UIArmyWindow ArmyWindow
	{
		get
		{
			return this.m_ArmyWindow;
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06002AF3 RID: 10995 RVA: 0x0016B748 File Offset: 0x00169948
	public UICastleGarisson GarrisonWindow
	{
		get
		{
			return this.castleGarrison;
		}
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x0016B750 File Offset: 0x00169950
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x0016B758 File Offset: 0x00169958
	private void OnEnable()
	{
		if (this.m_Initialzied)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x0016B768 File Offset: 0x00169968
	protected override void Update()
	{
		base.Update();
		if (this.governorInTransit != null)
		{
			this.UpdateGovernorAssignmentProgress();
		}
		this.RefreshLocalAuthority();
		this.UpdateRebelionRisk();
		this.RefreshFame();
		this.UpdateButtonStates();
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x0016B798 File Offset: 0x00169998
	private void LateUpdate()
	{
		if (this.m_InvalidateGovernorAssigment)
		{
			this.CheckForGovernorAssigments();
			this.SetGovernorInTransit(this.governorInTransit);
			this.m_InvalidateGovernorAssigment = false;
		}
		if (this.m_InvalidateVisitingArmy)
		{
			this.PopulateVisitingArmy();
			this.m_InvalidateVisitingArmy = false;
		}
		if (this.m_InvalidateDisorder)
		{
			this.RefreshDisorder();
			this.m_InvalidateDisorder = false;
		}
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x0016B7F0 File Offset: 0x001699F0
	private void ExtractLogicObject()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (worldUI.selected_obj == null)
		{
			return;
		}
		global::Settlement component = worldUI.selected_obj.GetComponent<global::Settlement>();
		if (component != null && component.logic != this.Data)
		{
			this.SetObject(component.logic, new Vars(component.logic));
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x0016B85C File Offset: 0x00169A5C
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.Init();
		Castle data = this.Data;
		if (data != null)
		{
			Logic.Kingdom kingdom = data.GetKingdom();
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
		}
		if (this.logicObject is Castle)
		{
			this.Data = (this.logicObject as Castle);
			Castle data2 = this.Data;
			if (data2 != null)
			{
				Logic.Kingdom kingdom2 = data2.GetKingdom();
				if (kingdom2 != null)
				{
					kingdom2.AddListener(this);
				}
			}
		}
		else
		{
			this.Data = null;
			Debug.LogWarningFormat("{0} is not a Logic.Castle!", new object[]
			{
				this.logicObject
			});
		}
		Vars vars2 = new Vars(this.Data);
		Castle data3 = this.Data;
		Vars vars3 = new Vars((data3 != null) ? data3.GetRealm() : null);
		if (this.m_LocalAuthority != null)
		{
			Tooltip.Get(this.m_LocalAuthority, true).SetDef("LocalAuthorityTooltip", vars2);
		}
		if (this.m_RebelionRisk != null)
		{
			Tooltip.Get(this.m_RebelionRisk, true).SetDef("LocalRebelRiskTooltip", vars3);
		}
		if (this.m_GoodsProduction != null)
		{
			this.m_GoodsProduction.SetObject(this.Data.GetRealm());
		}
		if (this.m_ProvinceFeatures != null)
		{
			this.m_ProvinceFeatures.SetObject(this.Data.GetRealm());
		}
		if (this.m_ProvinceIllustration != null)
		{
			this.m_ProvinceIllustration.SetObject(this.Data);
		}
		if (this.m_Fame != null)
		{
			Tooltip.Get(this.m_Fame, true).SetDef("RealmFameTooltip", vars3);
		}
		if (this.m_RealmProvinceFeaturesLabel != null)
		{
			UIText.SetTextKey(this.m_RealmProvinceFeaturesLabel, "CastleWindow.features_and_goods", vars2, null);
		}
		if (this.m_NoSpy != null && this.m_NoSpy.Length != 0)
		{
			for (int i = 0; i < this.m_NoSpy.Length; i++)
			{
				Tooltip.Get(this.m_NoSpy[i], true).SetDef("ForeignProvinceInformationTooltip", vars3);
			}
		}
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.Select(false);
		}
		this.CheckForGovernorAssigments();
		this.Refresh();
		this.RestoreState();
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x0016BA80 File Offset: 0x00169C80
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.OnSelect += this.HandleOnGovernorIconSelect;
		}
		if (this.m_RealmGoodsProduction != null)
		{
			this.m_GoodsProduction = this.m_RealmGoodsProduction.AddComponent<UICastleWindow.UIGoodsProduction>();
		}
		if (this.m_RealmProvinceFeatures != null)
		{
			this.m_ProvinceFeatures = this.m_RealmProvinceFeatures.AddComponent<UICastleWindow.UIProvinceFeatures>();
		}
		if (this.m_ProvinceIllustrationContainer != null)
		{
			this.m_ProvinceIllustration = this.m_ProvinceIllustrationContainer.AddComponent<UICastleWindow.UIProvinceIllustration>();
		}
		if (this.m_DisorderActionInfo != null)
		{
			this.m_DisorderActionInfo.gameObject.SetActive(false);
		}
		if (this.m_DisorderHint != null)
		{
			this.m_DisorderHint.gameObject.SetActive(false);
		}
		if (this.m_EstablishOrderAction != null)
		{
			this.m_EstablishOrderAction.gameObject.SetActive(false);
			this.m_EstablishOrderAction.OnStateChange = new Action<UIActionIcon>(this.HandleEstablishOrderActionStateChange);
		}
		Vars vars = new Vars(this);
		if (this.m_OpenBuildings != null)
		{
			this.m_OpenBuildings.onClick = new BSGButton.OnClick(this.HandleOnOpenBuilding);
			Tooltip.Get(this.m_OpenBuildings.gameObject, true).SetDef("CastleViewBuildingTooltip", vars);
		}
		if (this.m_CloseBuildings != null)
		{
			this.m_CloseBuildings.onClick = new BSGButton.OnClick(this.HandleOnCloseBuilding);
			Tooltip.Get(this.m_CloseBuildings.gameObject, true).SetDef("CastleViewBuildingTooltip", vars);
		}
		if (this.m_OpenHire != null)
		{
			this.m_OpenHire.onClick = new BSGButton.OnClick(this.HandleOnOpenHire);
			Tooltip.Get(this.m_OpenHire.gameObject, true).SetDef("CastleViewRecruitmentTooltip", vars);
		}
		if (this.m_CloseHire != null)
		{
			this.m_CloseHire.onClick = new BSGButton.OnClick(this.HandleOnCloseHire);
			Tooltip.Get(this.m_CloseHire.gameObject, true).SetDef("CastleViewRecruitmentTooltip", vars);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x001289C7 File Offset: 0x00126BC7
	public void Close()
	{
		BaseUI.Get().SelectObj(null, false, true, true, true);
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x0016BCA8 File Offset: 0x00169EA8
	public override void AddListeners()
	{
		base.AddListeners();
		if (this.logicObject is Castle)
		{
			Castle castle = this.logicObject as Castle;
			Logic.Kingdom kingdom = (castle != null) ? castle.GetKingdom() : null;
			if (kingdom != null)
			{
				kingdom.AddListener(this);
			}
			Castle castle2 = this.logicObject as Castle;
			Logic.Realm realm = (castle2 != null) ? castle2.GetRealm() : null;
			if (realm != null)
			{
				realm.AddListener(this);
			}
		}
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x0016BD0C File Offset: 0x00169F0C
	public override void RemoveListeners()
	{
		base.RemoveListeners();
		if (this.logicObject is Castle)
		{
			Castle castle = this.logicObject as Castle;
			Logic.Kingdom kingdom = (castle != null) ? castle.GetKingdom() : null;
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
			Castle castle2 = this.logicObject as Castle;
			Logic.Realm realm = (castle2 != null) ? castle2.GetRealm() : null;
			if (realm != null)
			{
				realm.DelListener(this);
			}
		}
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x0016BD70 File Offset: 0x00169F70
	public override void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		this.PopulateCityGlobals();
		this.PopulateVisitingArmy();
		this.PopulateSettlements();
		this.SetupGovernor();
		this.RefreshGovernor();
		this.RefreshPopulation();
		this.RefreshFood();
		this.RefreshDisorder();
		this.RefreshLocalAuthority();
		this.RefreshBuildings();
		this.RefreshFortifications();
		this.RefreshRealmReligion();
		this.RefreshFame();
		this.RefreshForeignCastleData();
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x0016BDDC File Offset: 0x00169FDC
	private void PopulateCityGlobals()
	{
		Logic.Settlement settlement = this.logicObject as Logic.Settlement;
		if (settlement == null)
		{
			return;
		}
		Logic.Realm realm = settlement.GetRealm();
		if (this.m_KingdomIcon != null && this.m_KingdomIcon.Length != 0)
		{
			for (int i = 0; i < this.m_KingdomIcon.Length; i++)
			{
				this.m_KingdomIcon[i].SetObject(this.Data.GetKingdom(), null);
			}
		}
		Logic.Kingdom kingdom;
		if (realm == null)
		{
			kingdom = null;
		}
		else
		{
			Logic.Object controller = realm.controller;
			kingdom = ((controller != null) ? controller.GetKingdom() : null);
		}
		Logic.Kingdom kingdom2 = kingdom;
		Vars vars = null;
		if (kingdom2 != null && kingdom2.type == Logic.Kingdom.Type.LoyalistsFaction)
		{
			this.vars = new Vars(kingdom2);
			this.vars.Set<Logic.Object>("target", realm.controller);
		}
		if (this.m_KingdomOccupator != null)
		{
			if (realm.controller.GetKingdom() != this.Data.GetKingdom())
			{
				this.m_KingdomOccupator.SetObject(realm.controller.GetKingdom(), vars);
				this.m_KingdomOccupator.gameObject.SetActive(true);
			}
			else
			{
				this.m_KingdomOccupator.gameObject.SetActive(false);
			}
		}
		if (this.m_ControllerKingdom != null && this.m_ControllerKingdom.Length != 0)
		{
			for (int j = 0; j < this.m_ControllerKingdom.Length; j++)
			{
				this.m_ControllerKingdom[j].SetObject(realm.controller.GetKingdom(), vars);
			}
		}
		if (this.m_RebelLoyalToKingdom != null)
		{
			Rebellion rebellion = realm.controller as Rebellion;
			if (rebellion != null && rebellion.IsLoyalist())
			{
				this.m_RebelLoyalToKingdom.SetObject(this.Data.game.GetKingdom(rebellion.loyal_to), null);
				this.m_RebelLoyalToKingdom.gameObject.SetActive(true);
			}
			else
			{
				this.m_RebelLoyalToKingdom.gameObject.SetActive(false);
			}
		}
		if (this.m_KingdomName != null && this.m_KingdomName.Length != 0)
		{
			UIText.SetText(this.m_KingdomName[0], global::Defs.LocalizedObjName(settlement.GetKingdom(), this.vars, "", true));
			if (this.m_KingdomName.Length > 1)
			{
				UIText.SetText(this.m_KingdomName[1], global::Defs.LocalizedObjName(realm.controller.GetKingdom(), this.vars, "", true));
			}
		}
		if (this.m_OccupiedStateIcon != null)
		{
			this.m_OccupiedStateIcon.gameObject.SetActive(realm.IsOccupied());
			Vars vars2 = new Vars(this.Data);
			vars2.Set<Logic.Kingdom>("occupant", realm.controller.GetKingdom());
			Tooltip.Get(this.m_OccupiedStateIcon.gameObject, true).SetDef("OccupiedStateTooltip", vars2);
		}
		if (this.m_KingdomRoles != null && this.m_KingdomRoles.Length != 0)
		{
			bool flag = realm.IsOccupied();
			for (int k = 0; k < this.m_KingdomRoles.Length; k++)
			{
				this.m_KingdomRoles[k].SetActive(flag);
			}
			if (flag && this.m_KingdomRoleTexts != null && this.m_KingdomRoleTexts.Length != 0)
			{
				UIText.SetText(this.m_KingdomRoleTexts[0], global::Defs.Localize("CastleWindow.ownerRole", null, null, true, true));
				if (this.m_KingdomRoleTexts.Length > 1)
				{
					UIText.SetText(this.m_KingdomRoleTexts[1], global::Defs.Localize("CastleWindow.occupatorRole", null, null, true, true));
				}
			}
		}
		if (this.Text_TownName != null && this.Text_TownName.Length != 0)
		{
			for (int l = 0; l < this.Text_TownName.Length; l++)
			{
				UIText.SetText(this.Text_TownName[l], global::Defs.LocalizedObjName(this.logicObject, this.vars, "", true));
			}
		}
		if (this.Text_TownTitle != null)
		{
			UIText.SetTextKey(this.Text_TownTitle, "Settlement.title_realm", new Vars(realm), null);
		}
		Vars vars3 = new Vars(this.logicObject);
		vars3.Set<Color>("color_negative", global::Defs.GetColor("UICastleWindow", (this.Data.governor != null) ? "resource_negative_govern" : "resource_negative_not_govern"));
		vars3.Set<Color>("color_positive", global::Defs.GetColor("UICastleWindow", (this.Data.governor != null) ? "resource_positive_govern" : "resource_positive_not_govern"));
		if (this.incomeGold != null)
		{
			this.incomeGold.ShowSign(false);
			this.incomeGold.SetObject(ResourceType.Gold, this.logicObject, vars3);
		}
		if (this.incomeProduction != null)
		{
			this.incomeProduction.ShowSign(false);
			this.incomeProduction.SetObject(ResourceType.Hammers, this.logicObject, vars3);
		}
		if (this.incomeFood != null)
		{
			this.incomeFood.ShowSign(false);
			this.incomeFood.SetObject(ResourceType.Food, this.logicObject, vars3);
		}
		if (this.incomeBooks != null)
		{
			this.incomeBooks.ShowSign(false);
			this.incomeBooks.SetObject(ResourceType.Books, this.logicObject, vars3);
		}
		if (this.incomePiety != null)
		{
			this.incomePiety.SetObject(ResourceType.Piety, this.logicObject, vars3);
			this.incomePiety.ShowSign(false);
		}
		if (this.incomeTrade != null)
		{
			this.incomeTrade.ShowSign(false);
			this.incomeTrade.SetObject(ResourceType.Trade, this.logicObject, vars3);
		}
		if (this.incomeLevy != null)
		{
			this.incomeLevy.ShowSign(false);
			this.incomeLevy.SetObject(ResourceType.Levy, this.logicObject, vars3);
		}
		if (this.cityPopulation != null)
		{
			this.cityPopulation.SetData(this.Data);
		}
		UIText.SetText(this.provinceName, global::Defs.LocalizedObjName(realm, null, "", true));
		if (this.m_TradeCenterInfo != null)
		{
			this.m_TradeCenterInfo.SetObject(realm);
		}
		if (this.m_LocalNobility != null)
		{
			UIText.SetTextKey(this.m_LocalNobility, "Castle.Window.local_nobility", this.vars, null);
		}
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x0016C3B0 File Offset: 0x0016A5B0
	private void SetupGovernor()
	{
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.ShowCrest(false);
		}
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x0016C3CC File Offset: 0x0016A5CC
	private void SetGovernorInTransit(Logic.Character newGovernor)
	{
		if (this.ValidateIncomingGovernor(newGovernor, this.Data))
		{
			this.governorInTransit = newGovernor;
			this.RefreshGovernor();
			this.UpdateGovernorAssignmentProgress();
			return;
		}
		this.governorInTransit = null;
		this.RefreshGovernor();
		this.UpdateGovernorAssignmentProgress();
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x0016C404 File Offset: 0x0016A604
	private void CheckForGovernorAssigments()
	{
		this.governorInTransit = null;
		for (int i = 0; i < this.Data.GetKingdom().court.Count; i++)
		{
			Logic.Character character = this.Data.GetKingdom().court[i];
			if (character != null && this.ValidateIncomingGovernor(character, this.Data))
			{
				this.governorInTransit = character;
				return;
			}
		}
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x0016C46C File Offset: 0x0016A66C
	private bool ValidateIncomingGovernor(Logic.Character character, Castle castle)
	{
		if (character == null)
		{
			return false;
		}
		if (character.actions == null)
		{
			return false;
		}
		if (castle == null)
		{
			return false;
		}
		Action action = character.actions.Find("GovernCityAction");
		if (action == null || !action.is_active)
		{
			action = character.actions.Find("AssignNewGovernCityAction");
		}
		if (action == null || !action.is_active)
		{
			action = character.actions.Find("ReassignGovernCityAction");
		}
		return action != null && action.is_active && action.target == castle.GetRealm();
	}

	// Token: 0x06002B04 RID: 11012 RVA: 0x0016C4F4 File Offset: 0x0016A6F4
	private void UpdateGovernorAssignmentProgress()
	{
		if (!this.ValidateIncomingGovernor(this.governorInTransit, this.Data))
		{
			this.governorInTransit = null;
			this.RefreshGovernor();
			return;
		}
		if (this.m_GovernorInTransitProgress != null)
		{
			float num = this.<UpdateGovernorAssignmentProgress>g__GetProgress|94_0(this.governorInTransit);
			this.m_GovernorInTransitProgress.fillAmount = 1f - num;
		}
	}

	// Token: 0x06002B05 RID: 11013 RVA: 0x0016C550 File Offset: 0x0016A750
	private void RefreshGovernor()
	{
		if (this.m_GovernorIcon == null)
		{
			return;
		}
		bool flag = this.Data.GetKingdom() == BaseUI.LogicKingdom();
		bool flag2 = !flag && this.Data.governor != null;
		flag2 &= !this.Data.IsOccupied();
		if (this.m_GroupGovernor != null)
		{
			this.m_GroupGovernor.gameObject.SetActive(flag2);
		}
		if (this.m_GroupNoGovernor != null)
		{
			this.m_GroupNoGovernor.gameObject.SetActive(!flag2);
		}
		if (flag)
		{
			if (this.Data.governor != null)
			{
				this.m_GovernorIcon.SetObject(this.Data.governor, null);
			}
			else if (this.governorInTransit != null)
			{
				this.m_GovernorIcon.SetObject(this.governorInTransit, null);
			}
			else
			{
				this.m_GovernorIcon.SetObject(null, null);
			}
			if (this.m_GovernorInTransitProgress != null)
			{
				this.m_GovernorInTransitProgress.gameObject.SetActive(this.governorInTransit != null);
			}
			Tooltip.Get(this.m_GovernorIcon.gameObject, true).SetDef("GovernorTooltip", new Vars(this.Data));
		}
		else if (this.Data.governor != null)
		{
			this.m_GovernorIcon.SetObject(this.Data.governor, null);
		}
		else
		{
			this.m_GovernorIcon.SetObject(null, null);
		}
		this.m_GovernorIcon.gameObject.SetActive(this.m_GovernorIcon.Data != null || flag);
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x0016C6E4 File Offset: 0x0016A8E4
	private void PopulateVisitingArmy()
	{
		this.m_ArmyWindow = null;
		if (this.m_VisitingArmyContainer != null)
		{
			UICommon.DeleteChildren(this.m_VisitingArmyContainer);
			if (this.Data.army != null)
			{
				Vars vars = new Vars(this.Data.army);
				Logic.Object controller = this.Data.GetRealm().controller;
				Logic.Kingdom kingdom = (controller != null) ? controller.GetKingdom() : null;
				if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat inspect castle", true))
				{
					vars.Set<string>("variant", "in_castle");
				}
				else
				{
					vars.Set<string>("variant", (kingdom == BaseUI.LogicKingdom()) ? "in_castle" : "in_enemy_castle");
				}
				GameObject window = ObjectWindow.GetWindow(this.Data.army, vars, this.m_VisitingArmyContainer);
				this.m_ArmyWindow = window.GetComponent<UIArmyWindow>();
			}
		}
		if (this.m_IllustrationDarkenEffect != null)
		{
			this.m_IllustrationDarkenEffect.gameObject.SetActive(this.Data.army != null);
		}
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x0016C7F3 File Offset: 0x0016A9F3
	private void RefreshPopulation()
	{
		if (this.cityPopulation != null)
		{
			this.cityPopulation.Refresh();
		}
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x0016C810 File Offset: 0x0016AA10
	private void RefreshBuildings()
	{
		if (this.castleBuildings != null)
		{
			this.castleBuildings.SetObject(this.Data);
		}
		if (this.m_ContexSelector != null)
		{
			this.m_ContexSelector.SetCastle(this.Data);
		}
		if (this.castleGarrison != null)
		{
			this.castleGarrison.SetCastle(this.Data);
		}
		if (this.m_liftOccupation != null)
		{
			this.m_liftOccupation.SetObject(this.Data);
		}
		if (this.m_castleAnnex != null)
		{
			this.m_castleAnnex.SetObject(this.Data);
		}
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x0016C8B8 File Offset: 0x0016AAB8
	private void RefreshFortifications()
	{
		if (this.m_Fortifications != null)
		{
			this.m_Fortifications.SetObject(this.Data);
		}
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x0016C8DC File Offset: 0x0016AADC
	private void RefreshPopMajority()
	{
		if (this.m_PopMajorityKingdom == null)
		{
			return;
		}
		Logic.Realm realm = this.Data.GetRealm();
		Logic.Kingdom kingdom = this.Data.GetKingdom();
		Vars vars = new Vars();
		vars.Set<Logic.Realm>("realm", realm);
		vars.Set<Logic.Kingdom>("kingdom_loyal_to", realm.pop_majority.kingdom);
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		if (kingdom != realm.pop_majority.kingdom)
		{
			this.m_PopMajorityKingdom.gameObject.SetActive(true);
			this.m_PopMajorityKingdom.SetObject(realm.pop_majority.kingdom, null);
			return;
		}
		this.m_PopMajorityKingdom.gameObject.SetActive(false);
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x0016C98C File Offset: 0x0016AB8C
	private void RefreshRealmReligion()
	{
		if (this.Data == null)
		{
			return;
		}
		Logic.Realm realm = this.Data.GetRealm();
		bool flag;
		if (realm != null)
		{
			Religion religion = realm.religion;
			Logic.Kingdom kingdom = this.Data.GetKingdom();
			flag = (religion != ((kingdom != null) ? kingdom.religion : null));
		}
		else
		{
			flag = false;
		}
		bool active = flag;
		if (this.m_RealmReligion != null)
		{
			this.m_RealmReligion.gameObject.SetActive(active);
			Tooltip.Get(this.m_RealmReligion, true).SetDef("ReligionTensionTooltip", new Vars(this.Data));
		}
		if (this.m_RealmReligionIcon != null && realm != null)
		{
			this.m_RealmReligionIcon.overrideSprite = global::Defs.GetObj<Sprite>(realm.religion.def.field, "icon", null);
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x0016CA54 File Offset: 0x0016AC54
	private void RefreshFame()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_ProvinceFame == null && this.m_Fame == null)
		{
			return;
		}
		float fame = this.Data.GetRealm().GetFame();
		if (fame > 0f)
		{
			GameObject fame2 = this.m_Fame;
			if (fame2 != null)
			{
				fame2.gameObject.SetActive(true);
			}
			TextMeshProUGUI provinceFame = this.m_ProvinceFame;
			if (provinceFame != null)
			{
				provinceFame.gameObject.SetActive(true);
			}
			UIText.SetText(this.m_ProvinceFame, fame.ToString());
			return;
		}
		GameObject fame3 = this.m_Fame;
		if (fame3 != null)
		{
			fame3.gameObject.SetActive(false);
		}
		TextMeshProUGUI provinceFame2 = this.m_ProvinceFame;
		if (provinceFame2 == null)
		{
			return;
		}
		provinceFame2.gameObject.SetActive(false);
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x0016CB10 File Offset: 0x0016AD10
	private void RefreshForeignCastleData()
	{
		if (this.Data == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Data.GetKingdom();
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		bool flag = this.Data.IsOccupied();
		bool flag2 = kingdom != kingdom2;
		bool flag3 = kingdom.GetSpyFrom(kingdom2) != null;
		bool flag4 = !flag && (!flag2 || flag3);
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat inspect castle", true))
		{
			flag4 = true;
		}
		if (this.m_NoSpy != null && this.m_NoSpy.Length != 0)
		{
			for (int i = 0; i < this.m_NoSpy.Length; i++)
			{
				this.m_NoSpy[i].SetActive(!flag4);
			}
		}
		if (this.castleBuildings != null)
		{
			this.castleBuildings.gameObject.SetActive(flag4);
		}
		if (this.foodStorage != null)
		{
			this.foodStorage.gameObject.SetActive(flag4);
		}
		if (this.m_Fortifications != null)
		{
			this.m_Fortifications.gameObject.SetActive(flag4);
		}
		if (this.castleGarrison != null)
		{
			this.castleGarrison.gameObject.SetActive(flag4);
		}
		if (this.m_VisitingArmyContainer != null)
		{
			this.m_VisitingArmyContainer.gameObject.SetActive(flag4);
		}
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x0016CC5C File Offset: 0x0016AE5C
	private void UpdateButtonStates()
	{
		if (this.Data == null)
		{
			return;
		}
		bool flag = UICastleBuildWindow.IsVisible();
		if (this.m_OpenBuildings != null)
		{
			this.m_OpenBuildings.gameObject.SetActive(!flag);
		}
		if (this.m_CloseBuildings != null)
		{
			this.m_CloseBuildings.gameObject.SetActive(flag);
		}
		bool flag2 = UIUnitRecruitmentWindow.IsActive();
		if (this.m_OpenHire != null)
		{
			this.m_OpenHire.gameObject.SetActive(!flag2);
		}
		if (this.m_CloseHire != null)
		{
			this.m_CloseHire.gameObject.SetActive(flag2);
		}
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x0016CD00 File Offset: 0x0016AF00
	private void RefreshDisorder()
	{
		Logic.Realm realm = this.Data.GetRealm();
		Logic.Kingdom kingdom = this.Data.GetKingdom();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = realm.IsDisorder();
		Castle data = this.Data;
		Action action;
		if (data == null)
		{
			action = null;
		}
		else
		{
			Logic.Army army = data.army;
			if (army == null)
			{
				action = null;
			}
			else
			{
				Logic.Character leader = army.leader;
				if (leader == null)
				{
					action = null;
				}
				else
				{
					Actions actions = leader.actions;
					action = ((actions != null) ? actions.Find("EstablishOrderAction") : null);
				}
			}
		}
		Action action2 = action;
		if (action2 != null)
		{
			string a = action2.Validate(false);
			flag = (a == "ok" || a == "_in_progress");
			flag2 = (action2.state == Action.State.Preparing);
		}
		if (this.m_EstablishOrderAction != null)
		{
			this.m_EstablishOrderAction.SetObject(action2, null);
			this.m_EstablishOrderAction.gameObject.SetActive(flag3 && flag && !flag2);
		}
		if (this.m_Disorder != null)
		{
			Vars vars = new Vars();
			vars.Set<Logic.Realm>("realm", realm);
			vars.Set<Logic.Kingdom>("kingdom_loyal_to", realm.pop_majority.kingdom);
			vars.Set<Logic.Kingdom>("kingdom", kingdom);
			this.m_Disorder.SetActive(flag3);
			Tooltip.Get(this.m_Disorder, true).SetDef("DisorderTooltip", vars);
		}
		if (this.m_DisorderHint != null)
		{
			UIText.SetTextKey(this.m_DisorderHint, "CastleWindow.disorder_hint", realm, null);
			this.m_DisorderHint.gameObject.SetActive(flag3 && !flag);
		}
		if (this.m_DisorderActionInfo != null)
		{
			this.m_DisorderActionInfo.SetData(action2);
			this.m_DisorderActionInfo.gameObject.SetActive(flag3 && flag2);
		}
		this.RefreshPopMajority();
		if (this.m_Group_Disorder != null)
		{
			this.m_Group_Disorder.gameObject.SetActive(flag3);
		}
		if (this.m_Group_Buildings != null)
		{
			this.m_Group_Buildings.gameObject.SetActive(!flag3);
		}
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x0016CEFC File Offset: 0x0016B0FC
	private void HandleEstablishOrderActionStateChange(UIActionIcon i)
	{
		this.m_InvalidateDisorder = true;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x0016CF08 File Offset: 0x0016B108
	private void RefreshFood()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.foodStorage != null)
		{
			float num = this.Data.GetFoodStorage();
			Tooltip.Get(this.foodStorage, true).SetDef("RealmFoodStorageTootlip", new Vars(this.Data));
			UnityEngine.Component component = global::Common.FindChildByName(this.foodStorage, "id_ProgressBarForeground", true, true).transform as RectTransform;
			float fillAmount = num / this.Data.GetMaxFoodStorage();
			Image component2 = component.GetComponent<Image>();
			if (component2 != null)
			{
				component2.fillAmount = fillAmount;
			}
			UIText.SetText(this.foodStorage, "id_TextValue", this.Data.GetMaxFoodStorage().ToString());
		}
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x0016CFC8 File Offset: 0x0016B1C8
	private void RefreshLocalAuthority()
	{
		if (this.m_LocalAuthorityValue != null)
		{
			TMP_Text localAuthorityValue = this.m_LocalAuthorityValue;
			Castle data = this.Data;
			string text;
			if (data == null)
			{
				text = null;
			}
			else
			{
				Logic.Realm realm = data.GetRealm();
				text = ((realm != null) ? realm.GetTotalRebellionRisk().ToString() : null);
			}
			localAuthorityValue.text = (text ?? "0");
		}
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x0016D020 File Offset: 0x0016B220
	private void PopulateSettlements()
	{
		if (this.m_ProvinceInfo != null)
		{
			this.m_ProvinceInfo.SetObject(this.Data);
		}
		if (this.settelementIconsContainer != null)
		{
			UICommon.DeleteChildren(this.settelementIconsContainer);
			Logic.Realm realm = this.Data.GetRealm();
			if (realm != null && realm.settlements != null && realm.settlements.Count > 1)
			{
				Logic.Settlement[] array = realm.settlements.ToArray();
				Array.Sort<Logic.Settlement>(array, (Logic.Settlement x, Logic.Settlement y) => x.type.CompareTo(y.type));
				foreach (Logic.Settlement settlement in array)
				{
					if (!(settlement.type == "Castle") && settlement.IsActiveSettlement())
					{
						UISettelmentIcon component = ObjectIcon.GetIcon(settlement, null, this.settelementIconsContainer).GetComponent<UISettelmentIcon>();
						if (component != null)
						{
							component.OnSelect += this.HandleOnSettlementClick;
						}
					}
				}
			}
		}
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x0016D12C File Offset: 0x0016B32C
	private Logic.Settlement GetSettlementSubSelection()
	{
		WorldUI worldUI = BaseUI.Get<WorldUI>();
		if (worldUI != null && worldUI.selected_orig != null && worldUI.selected_orig != worldUI.selected_obj)
		{
			global::Settlement component = worldUI.selected_orig.gameObject.GetComponent<global::Settlement>();
			if (component != null && component.logic.GetRealm() == this.Data.GetRealm())
			{
				return component.logic;
			}
		}
		return null;
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x0016D1A4 File Offset: 0x0016B3A4
	private void UpdateRebelionRisk()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_RebelionRisk == null)
		{
			return;
		}
		this.riskVars.Set<float>("local_rebelion_risk", this.Data.GetRealm().GetTotalRebellionRisk());
		UIText.SetTextKey(this.m_RebelionRiskValue, "CastleWindow.rebelion_risk", this.riskVars, null);
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x0016D200 File Offset: 0x0016B400
	private void HandleOnOpenBuilding(BSGButton b)
	{
		int num = -1;
		if (this.castleBuildings != null)
		{
			num = this.castleBuildings.SelectedSlotIndex;
			if (num < 0)
			{
				num = this.castleBuildings.FindEmptyIdx(this.Data.buildings);
			}
			if (num >= 0)
			{
				this.castleBuildings.Select(num, true);
			}
		}
		UICastleBuildWindow.Create(this.Data.GetKingdom(), this.Data, num, null);
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x0016D26E File Offset: 0x0016B46E
	private void HandleOnCloseBuilding(BSGButton b)
	{
		UICastleBuildWindow.CloseInstance();
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x0016D275 File Offset: 0x0016B475
	private void HandleOnOpenHire(BSGButton b)
	{
		this.castleGarrison.OpenRecruitWindow(-1, false);
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000E6F77 File Offset: 0x000E5177
	private void HandleOnCloseHire(BSGButton b)
	{
		UIUnitRecruitmentWindow.CloseInstance();
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x0016D284 File Offset: 0x0016B484
	private void HandleOnSettlementClick(UISettelmentIcon e)
	{
		if (!(e.logicObject is Logic.Settlement))
		{
			return;
		}
		Logic.Settlement settlement = e.logicObject as Logic.Settlement;
		WorldUI worldUI = BaseUI.Get<WorldUI>();
		if (worldUI != null && settlement.visuals is global::Settlement)
		{
			worldUI.SelectObj((settlement.visuals as global::Settlement).gameObject, false, true, true, true);
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x0016D2E4 File Offset: 0x0016B4E4
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1607432438U)
		{
			if (num <= 769429365U)
			{
				if (num != 24823524U)
				{
					if (num != 424646424U)
					{
						if (num == 769429365U)
						{
							if (message == "population_changed")
							{
								this.RefreshPopulation();
							}
						}
					}
					else if (message == "governor_changed")
					{
						this.RefreshGovernor();
					}
				}
				else if (message == "structures_changed")
				{
					if (this.m_VisitingArmyContainer != null && this.m_VisitingArmyContainer.transform.childCount > 0)
					{
						UIArmyWindow componentInChildren = this.m_VisitingArmyContainer.GetComponentInChildren<UIArmyWindow>();
						if (componentInChildren != null)
						{
							componentInChildren.RefreshHireWindow();
						}
					}
				}
			}
			else if (num != 1101300521U)
			{
				if (num != 1201570319U)
				{
					if (num == 1607432438U)
					{
						if (message == "food_changed")
						{
							this.RefreshFood();
						}
					}
				}
				else if (message == "begin_governor_assigment")
				{
					this.m_InvalidateGovernorAssigment = true;
				}
			}
			else if (message == "income_changed")
			{
				this.RefreshFood();
			}
		}
		else if (num <= 2105032289U)
		{
			if (num != 1947459821U)
			{
				if (num != 2078899078U)
				{
					if (num == 2105032289U)
					{
						if (message == "religion_changed")
						{
							this.PopulateCityGlobals();
							this.RefreshRealmReligion();
						}
					}
				}
				else if (message == "disorder_changed")
				{
					this.Refresh();
				}
			}
			else if (message == "infiltrated_by")
			{
				Logic.Character character = param as Logic.Character;
				if (((character != null) ? character.GetKingdom() : null) == BaseUI.LogicKingdom())
				{
					this.Refresh();
				}
			}
		}
		else if (num != 2282658765U)
		{
			if (num != 2465430116U)
			{
				if (num == 2597913759U)
				{
					if (message == "army_changed")
					{
						this.m_InvalidateVisitingArmy = true;
						this.m_InvalidateDisorder = true;
					}
				}
			}
			else if (message == "cancel_governor_assigment")
			{
				this.SetGovernorInTransit(null);
			}
		}
		else if (message == "pop_majority_changed")
		{
			this.RefreshPopMajority();
		}
		base.HandleLogicMessage(obj, message, param);
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x0016D570 File Offset: 0x0016B770
	private void HandleOnGovernorIconSelect(UICharacterIcon obj)
	{
		if (this.Data.GetKingdom() != BaseUI.LogicKingdom())
		{
			return;
		}
		if (((obj != null) ? obj.Data : null) != null && (UICommon.GetKeyDown(KeyCode.LeftShift, UICommon.ModifierKey.None, UICommon.ModifierKey.None) || UICommon.GetKeyDown(KeyCode.RightShift, UICommon.ModifierKey.None, UICommon.ModifierKey.None)))
		{
			obj.ExecuteDefaultSelectAction();
			return;
		}
		List<Logic.Character> list = new List<Logic.Character>();
		Logic.Kingdom kingdom = this.Data.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		if (kingdom.royalFamily != null)
		{
			if (kingdom.royalFamily.Sovereign != null && kingdom.royalFamily.Sovereign != this.Data.governor && this.IsValidGovernor(kingdom.royalFamily.Sovereign))
			{
				list.Add(kingdom.royalFamily.Sovereign);
			}
			for (int i = 0; i < kingdom.royalFamily.Children.Count; i++)
			{
				Logic.Character character = kingdom.royalFamily.Children[i];
				if (character != null && character != this.Data.governor && this.IsValidGovernor(character))
				{
					list.Add(character);
				}
			}
		}
		if (kingdom.court != null)
		{
			for (int j = 0; j < kingdom.court.Count; j++)
			{
				Logic.Character character2 = kingdom.court[j];
				if (character2 != null && character2 != this.Data.governor && !list.Contains(character2) && this.IsValidGovernor(character2))
				{
					list.Add(character2);
				}
			}
		}
		list.Sort(delegate(Logic.Character x, Logic.Character y)
		{
			if (x.governed_castle == null && y.governed_castle == null)
			{
				return 0;
			}
			if (x.governed_castle == null)
			{
				return -1;
			}
			if (y.governed_castle == null)
			{
				return 1;
			}
			return 0;
		});
		List<Vars> list2 = new List<Vars>(list.Count);
		for (int k = 0; k < list.Count; k++)
		{
			Logic.Character character3 = list[k];
			Vars vars = new Vars();
			vars.Set<Castle>("castle", this.Data);
			vars.Set<Logic.Character>("governor", character3);
			vars.Set<string>("tooltip_def", "ChooseGovernorTooltip");
			Vars vars2 = vars;
			string key = "additinalObject";
			Castle governed_castle = character3.governed_castle;
			vars2.Set<Logic.Object>(key, (governed_castle != null) ? governed_castle.GetRealm() : null);
			vars.Set<string>("icon_variant", "bribed");
			list2.Add(vars);
		}
		this.m_HasOpenGovernorSelector = UITargetSelectWindow.ShowDialog(TargetPickerData.Create(new List<Logic.Object>(list), list2, null), null, new Action<Value>(this.HandleNewGovernoSelected), new Action(this.HandleOnCancelGovernerSelection), new Vars(this.Data), null, null, null, "GovernorTargetPicker", "");
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.Select(true);
		}
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x0016D80C File Offset: 0x0016BA0C
	private void HandleNewGovernoSelected(Value target)
	{
		this.m_HasOpenGovernorSelector = false;
		if (!target.is_valid)
		{
			return;
		}
		Logic.Character character = target.obj_val as Logic.Character;
		if (character == null)
		{
			return;
		}
		if (character.actions == null)
		{
			return;
		}
		Action action = character.actions.Find(character.IsGovernor() ? "ReassignGovernCityAction" : (character.IsBanishedGovernor() ? "AssignNewGovernCityAction" : "GovernCityAction"));
		if (action == null)
		{
			return;
		}
		ActionVisuals actionVisuals = action.visuals as ActionVisuals;
		if (actionVisuals == null)
		{
			return;
		}
		actionVisuals.logic.target = this.Data.GetRealm();
		actionVisuals.Begin();
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.Select(false);
		}
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x0016D8BC File Offset: 0x0016BABC
	private void HandleOnCancelGovernerSelection()
	{
		this.m_HasOpenGovernorSelector = false;
		if (this.m_GovernorIcon != null)
		{
			this.m_GovernorIcon.Select(false);
		}
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x0016D8E0 File Offset: 0x0016BAE0
	private bool IsValidGovernor(Logic.Character c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.cur_action is ReassignGovernCityAction || c.cur_action is GovernCityAction)
		{
			return false;
		}
		if (c.actions == null)
		{
			return false;
		}
		Action action = c.actions.Find(c.IsGovernor() ? "ReassignGovernCityAction" : (c.IsBanishedGovernor() ? "AssignNewGovernCityAction" : "GovernCityAction"));
		return action != null && !(action.Validate(true) != "ok") && action.ValidateTarget(this.Data.GetRealm());
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x0016D974 File Offset: 0x0016BB74
	public override void RestoreState()
	{
		if (UICastleWindow.storage.garrisonHireWindow && this.castleGarrison != null)
		{
			this.castleGarrison.OpenRecruitWindow(-1, false);
		}
		UICastleBuildings uicastleBuildings = this.castleBuildings;
		if (uicastleBuildings != null)
		{
			uicastleBuildings.Select(UICastleWindow.storage.seleced_building_slot_index, UICastleWindow.storage.seleced_building_slot_index != -1);
		}
		if (this.Data != null && this.Data.army != null && this.m_VisitingArmyContainer != null)
		{
			if (UICastleWindow.storage.hireHireWindow && this.m_VisitingArmyContainer != null)
			{
				UIArmyWindow componentInChildren = this.m_VisitingArmyContainer.GetComponentInChildren<UIArmyWindow>();
				if (componentInChildren != null)
				{
					componentInChildren.OpenRecruitWindow(-1, false);
				}
			}
			if (UICastleWindow.storage.buyEquipmentWindow && this.m_VisitingArmyContainer != null)
			{
				UIArmyWindow componentInChildren2 = this.m_VisitingArmyContainer.GetComponentInChildren<UIArmyWindow>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.OpenEquipmentPurchaseWindow(-1, false);
				}
			}
		}
		if (UICastleWindow.storage.setGovernor)
		{
			this.HandleOnGovernorIconSelect(null);
		}
		UICastleWindow.storage.Clear();
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x0016DA84 File Offset: 0x0016BC84
	public override void StoreState()
	{
		UICastleWindow.storage.buildWindow = (this.castleBuildings && this.castleBuildings.GetStoreState());
		UICastleWindow.storage.seleced_building_slot_index = (this.castleBuildings ? this.castleBuildings.SelectedSlotIndex : -1);
		UICastleWindow.storage.garrisonHireWindow = (this.castleGarrison && this.castleGarrison.IsRecruitmentWindowActive());
		UICastleWindow.storage.setGovernor = this.m_HasOpenGovernorSelector;
		if (this.Data != null && this.Data.army != null && this.m_VisitingArmyContainer != null)
		{
			UIArmyWindow componentInChildren = this.m_VisitingArmyContainer.GetComponentInChildren<UIArmyWindow>();
			if (componentInChildren != null)
			{
				UICastleWindow.storage.hireHireWindow = componentInChildren.IsRecruitmentWindowActive();
				UICastleWindow.storage.buyEquipmentWindow = componentInChildren.IsEqupmentWindowActive();
			}
		}
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x0016DB68 File Offset: 0x0016BD68
	public override void Release()
	{
		this.Data = null;
		base.Release();
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x0016B750 File Offset: 0x00169950
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x0016DB77 File Offset: 0x0016BD77
	public void OnPoolDeactivated()
	{
		this.Data = null;
		this.governorInTransit = null;
		this.castleBuildings.Select(-1, true);
		this.OnDestroy();
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x0016DB9C File Offset: 0x0016BD9C
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "building_window_is_open")
		{
			return UICastleBuildWindow.IsVisible();
		}
		if (key == "recruitment_window_is_open")
		{
			return UIUnitRecruitmentWindow.IsActive();
		}
		if (this.Data != null)
		{
			return this.Data.GetVar(key, vars, as_value);
		}
		return Value.Unknown;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x0016DC18 File Offset: 0x0016BE18
	[CompilerGenerated]
	private float <UpdateGovernorAssignmentProgress>g__GetProgress|94_0(Logic.Character g)
	{
		if (g == null)
		{
			return 0f;
		}
		Actions actions = g.actions;
		Action action = (actions != null) ? actions.Find("GovernCityAction") : null;
		if (action == null || !action.is_active)
		{
			Actions actions2 = g.actions;
			action = ((actions2 != null) ? actions2.Find("AssignNewGovernCityAction") : null);
		}
		if (action == null || !action.is_active)
		{
			Actions actions3 = g.actions;
			action = ((actions3 != null) ? actions3.Find("ReassignGovernCityAction") : null);
		}
		if (action != null && action.is_active && action.target == this.Data.GetRealm())
		{
			float num;
			float num2;
			action.GetProgress(out num, out num2);
			return num / num2;
		}
		return 0f;
	}

	// Token: 0x04001D20 RID: 7456
	[UIFieldTarget("id_Kingdom")]
	private UIKingdomIcon[] m_KingdomIcon;

	// Token: 0x04001D21 RID: 7457
	[UIFieldTarget("id_ControllerKingdom")]
	private UIKingdomIcon[] m_ControllerKingdom;

	// Token: 0x04001D22 RID: 7458
	[UIFieldTarget("id_KingdomOccupator")]
	private UIKingdomIcon m_KingdomOccupator;

	// Token: 0x04001D23 RID: 7459
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI[] m_KingdomName;

	// Token: 0x04001D24 RID: 7460
	[UIFieldTarget("id_KingdomRole")]
	private GameObject[] m_KingdomRoles;

	// Token: 0x04001D25 RID: 7461
	[UIFieldTarget("id_KingdomRoleText")]
	private TextMeshProUGUI[] m_KingdomRoleTexts;

	// Token: 0x04001D26 RID: 7462
	[UIFieldTarget("id_OccupiedStateIcon")]
	private Image m_OccupiedStateIcon;

	// Token: 0x04001D27 RID: 7463
	[UIFieldTarget("id_GroupNoGovernor")]
	private GameObject m_GroupNoGovernor;

	// Token: 0x04001D28 RID: 7464
	[UIFieldTarget("id_GroupGovernor")]
	private GameObject m_GroupGovernor;

	// Token: 0x04001D29 RID: 7465
	[UIFieldTarget("id_Governer")]
	private UICharacterIcon m_GovernorIcon;

	// Token: 0x04001D2A RID: 7466
	[UIFieldTarget("id_GovernorInTransitProgress")]
	private Image m_GovernorInTransitProgress;

	// Token: 0x04001D2B RID: 7467
	[UIFieldTarget("id_SelectedSettlementContainer")]
	private RectTransform m_SelectedSettlementContainer;

	// Token: 0x04001D2C RID: 7468
	[UIFieldTarget("id_VisitingArmyContainer")]
	private RectTransform m_VisitingArmyContainer;

	// Token: 0x04001D2D RID: 7469
	[UIFieldTarget("id_TownName")]
	private TextMeshProUGUI[] Text_TownName;

	// Token: 0x04001D2E RID: 7470
	[UIFieldTarget("id_TownTitle")]
	private TextMeshProUGUI Text_TownTitle;

	// Token: 0x04001D2F RID: 7471
	[UIFieldTarget("id_ResoureIncome_Gold")]
	private UIResourceIncome incomeGold;

	// Token: 0x04001D30 RID: 7472
	[UIFieldTarget("id_ResoureIncome_Production")]
	private UIResourceIncome incomeProduction;

	// Token: 0x04001D31 RID: 7473
	[UIFieldTarget("id_ResoureIncome_Food")]
	private UIResourceIncome incomeFood;

	// Token: 0x04001D32 RID: 7474
	[UIFieldTarget("id_ResoureIncome_Books")]
	private UIResourceIncome incomeBooks;

	// Token: 0x04001D33 RID: 7475
	[UIFieldTarget("id_ResoureIncome_Piety")]
	private UIResourceIncome incomePiety;

	// Token: 0x04001D34 RID: 7476
	[UIFieldTarget("id_ResoureIncome_Trade")]
	private UIResourceIncome incomeTrade;

	// Token: 0x04001D35 RID: 7477
	[UIFieldTarget("id_ResoureIncome_Hammers")]
	private UIResourceIncome incomeHammers;

	// Token: 0x04001D36 RID: 7478
	[UIFieldTarget("id_ResoureIncome_Arms")]
	private UIResourceIncome incomeArms;

	// Token: 0x04001D37 RID: 7479
	[UIFieldTarget("id_ResoureIncome_Levy")]
	private UIResourceIncome incomeLevy;

	// Token: 0x04001D38 RID: 7480
	[UIFieldTarget("id_FoodStorage")]
	private GameObject foodStorage;

	// Token: 0x04001D39 RID: 7481
	[UIFieldTarget("id_RebelLoyalToKingdom")]
	private UIKingdomIcon m_RebelLoyalToKingdom;

	// Token: 0x04001D3A RID: 7482
	[UIFieldTarget("id_LocalAuthority")]
	private GameObject m_LocalAuthority;

	// Token: 0x04001D3B RID: 7483
	[UIFieldTarget("id_RebelionRisk")]
	private GameObject m_RebelionRisk;

	// Token: 0x04001D3C RID: 7484
	[UIFieldTarget("id_RebelionRiskValue")]
	private TextMeshProUGUI m_RebelionRiskValue;

	// Token: 0x04001D3D RID: 7485
	[UIFieldTarget("id_LocalAuthorityValue")]
	private TextMeshProUGUI m_LocalAuthorityValue;

	// Token: 0x04001D3E RID: 7486
	[UIFieldTarget("id_SettelmentGrid")]
	private RectTransform settelementIconsContainer;

	// Token: 0x04001D3F RID: 7487
	[UIFieldTarget("id_ProvinceName")]
	private TextMeshProUGUI provinceName;

	// Token: 0x04001D40 RID: 7488
	[UIFieldTarget("id_Population")]
	private UICityPopulation cityPopulation;

	// Token: 0x04001D41 RID: 7489
	[UIFieldTarget("id_Buildings")]
	private UICastleBuildings castleBuildings;

	// Token: 0x04001D42 RID: 7490
	[UIFieldTarget("id_GarissonSlots")]
	private UICastleGarisson castleGarrison;

	// Token: 0x04001D43 RID: 7491
	[UIFieldTarget("id_Province")]
	private UICastleRegionInfo m_ProvinceInfo;

	// Token: 0x04001D44 RID: 7492
	[UIFieldTarget("id_Label_OccupedDescription")]
	private TextMeshProUGUI m_OccupedDescription;

	// Token: 0x04001D45 RID: 7493
	[UIFieldTarget("id_TradeCenter")]
	private UITradeCenterInfo m_TradeCenterInfo;

	// Token: 0x04001D46 RID: 7494
	[UIFieldTarget("id_CloseButton")]
	private BSGButton m_CloseButton;

	// Token: 0x04001D47 RID: 7495
	[UIFieldTarget("id_ContextSelection")]
	private UICastleContextSelection m_ContexSelector;

	// Token: 0x04001D48 RID: 7496
	[UIFieldTarget("id_GeartPersonSlots")]
	private UIGreatPerson m_GreatPerson;

	// Token: 0x04001D49 RID: 7497
	[UIFieldTarget("id_LiftOccupation")]
	private UICastleLiftOccupation m_liftOccupation;

	// Token: 0x04001D4A RID: 7498
	[UIFieldTarget("id_CastleAnnex")]
	private UICastleAnnex m_castleAnnex;

	// Token: 0x04001D4B RID: 7499
	[UIFieldTarget("id_Disorder")]
	private GameObject m_Disorder;

	// Token: 0x04001D4C RID: 7500
	[UIFieldTarget("id_EstablishOrderAction")]
	private UIActionIcon m_EstablishOrderAction;

	// Token: 0x04001D4D RID: 7501
	[UIFieldTarget("id_DisorderHint")]
	private TextMeshProUGUI m_DisorderHint;

	// Token: 0x04001D4E RID: 7502
	[UIFieldTarget("id_DisorderActionInfo")]
	private UIActionProgressInfo m_DisorderActionInfo;

	// Token: 0x04001D4F RID: 7503
	[UIFieldTarget("id_PopMajorityKingdom")]
	private UIKingdomIcon m_PopMajorityKingdom;

	// Token: 0x04001D50 RID: 7504
	[UIFieldTarget("id_RealmGoodsProduction")]
	private GameObject m_RealmGoodsProduction;

	// Token: 0x04001D51 RID: 7505
	[UIFieldTarget("id_RealmProvinceFeatures")]
	private GameObject m_RealmProvinceFeatures;

	// Token: 0x04001D52 RID: 7506
	[UIFieldTarget("id_RealmProvinceFeaturesLabel")]
	private TextMeshProUGUI m_RealmProvinceFeaturesLabel;

	// Token: 0x04001D53 RID: 7507
	[UIFieldTarget("id_Fortifications")]
	private UICastleFortifications m_Fortifications;

	// Token: 0x04001D54 RID: 7508
	[UIFieldTarget("id_RealmReligion")]
	private GameObject m_RealmReligion;

	// Token: 0x04001D55 RID: 7509
	[UIFieldTarget("id_RealmReligonIcon")]
	private Image m_RealmReligionIcon;

	// Token: 0x04001D56 RID: 7510
	[UIFieldTarget("id_ProvinceIllustration")]
	private GameObject m_ProvinceIllustrationContainer;

	// Token: 0x04001D57 RID: 7511
	[UIFieldTarget("id_IllustrationDarkenEffect")]
	private GameObject m_IllustrationDarkenEffect;

	// Token: 0x04001D58 RID: 7512
	[UIFieldTarget("id_Fame")]
	private GameObject m_Fame;

	// Token: 0x04001D59 RID: 7513
	[UIFieldTarget("id_ProvinceFame")]
	private TextMeshProUGUI m_ProvinceFame;

	// Token: 0x04001D5A RID: 7514
	[UIFieldTarget("id_LocalNobility")]
	private TextMeshProUGUI m_LocalNobility;

	// Token: 0x04001D5B RID: 7515
	[UIFieldTarget("Group_Buildings")]
	private GameObject m_Group_Buildings;

	// Token: 0x04001D5C RID: 7516
	[UIFieldTarget("Group_Disorder")]
	private GameObject m_Group_Disorder;

	// Token: 0x04001D5D RID: 7517
	[UIFieldTarget("id_OpenBuildings")]
	private BSGButton m_OpenBuildings;

	// Token: 0x04001D5E RID: 7518
	[UIFieldTarget("id_CloseBuildings")]
	private BSGButton m_CloseBuildings;

	// Token: 0x04001D5F RID: 7519
	[UIFieldTarget("id_OpenHire")]
	private BSGButton m_OpenHire;

	// Token: 0x04001D60 RID: 7520
	[UIFieldTarget("id_CloseHire")]
	private BSGButton m_CloseHire;

	// Token: 0x04001D61 RID: 7521
	[UIFieldTarget("id_NoSpy")]
	private GameObject[] m_NoSpy;

	// Token: 0x04001D62 RID: 7522
	private Castle Data;

	// Token: 0x04001D63 RID: 7523
	private UICastleWindow.UIGoodsProduction m_GoodsProduction;

	// Token: 0x04001D64 RID: 7524
	private UICastleWindow.UIProvinceFeatures m_ProvinceFeatures;

	// Token: 0x04001D65 RID: 7525
	private UICastleWindow.UIProvinceIllustration m_ProvinceIllustration;

	// Token: 0x04001D66 RID: 7526
	private UIArmyWindow m_ArmyWindow;

	// Token: 0x04001D67 RID: 7527
	private bool m_InvalidateDisorder;

	// Token: 0x04001D68 RID: 7528
	private bool m_Initialzied;

	// Token: 0x04001D69 RID: 7529
	private Logic.Character governorInTransit;

	// Token: 0x04001D6A RID: 7530
	private bool m_InvalidateVisitingArmy;

	// Token: 0x04001D6B RID: 7531
	private Vars riskVars = new Vars();

	// Token: 0x04001D6C RID: 7532
	private bool m_InvalidateGovernorAssigment;

	// Token: 0x04001D6D RID: 7533
	private bool m_HasOpenGovernorSelector;

	// Token: 0x04001D6E RID: 7534
	private static UICastleWindow.StateStorage storage = new UICastleWindow.StateStorage();

	// Token: 0x0200080A RID: 2058
	private class StateStorage
	{
		// Token: 0x06004F6E RID: 20334 RVA: 0x0023562E File Offset: 0x0023382E
		public void Clear()
		{
			this.tab_index = 0;
			this.buildWindow = false;
			this.seleced_building_slot_index = -1;
			this.garrisonHireWindow = false;
			this.hireHireWindow = false;
			this.buyEquipmentWindow = false;
			this.setGovernor = false;
		}

		// Token: 0x04003D7E RID: 15742
		public int tab_index;

		// Token: 0x04003D7F RID: 15743
		public int seleced_building_slot_index = -1;

		// Token: 0x04003D80 RID: 15744
		public bool buildWindow;

		// Token: 0x04003D81 RID: 15745
		public bool garrisonHireWindow;

		// Token: 0x04003D82 RID: 15746
		public bool hireHireWindow;

		// Token: 0x04003D83 RID: 15747
		public bool buyEquipmentWindow;

		// Token: 0x04003D84 RID: 15748
		public bool setGovernor;
	}

	// Token: 0x0200080B RID: 2059
	internal class UIProvinceFeatures : MonoBehaviour, IListener
	{
		// Token: 0x06004F70 RID: 20336 RVA: 0x00235670 File Offset: 0x00233870
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x00235689 File Offset: 0x00233889
		public void SetObject(Logic.Realm realm)
		{
			this.Init();
			this.Realm = realm;
			this.PopulateProvinceFeatures();
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x002356A0 File Offset: 0x002338A0
		private void PopulateProvinceFeatures()
		{
			if (this.Realm == null)
			{
				return;
			}
			if (this.m_ProvinceFeatureContainer == null)
			{
				return;
			}
			UICommon.DeleteChildren(this.m_ProvinceFeatureContainer);
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return;
			}
			List<DT.Def> defs = global::Defs.GetDefField("SettlementType", null).def.defs;
			if (defs != null && defs.Count > 0)
			{
				for (int i = 0; i < defs.Count; i++)
				{
					DT.Field field = defs[i].field;
					DT.Field field2 = field.FindChild("type", null, true, true, true, '.');
					string text = (field2 != null) ? field2.String(null, "") : null;
					if (text == "CoastalSettlement" && this.Realm.GetSettlementCount("CoastalTown", "realm", false) > 0)
					{
						UISettlementTypeIcon.GetIcon(field, this.Realm, this.m_ProvinceFeatureContainer);
					}
					else if (this.Realm.GetSettlementCount(text, "realm", false) > 0)
					{
						UISettlementTypeIcon.GetIcon(field, this.Realm, this.m_ProvinceFeatureContainer);
					}
				}
			}
			for (int j = 0; j < this.Realm.features.Count; j++)
			{
				string id = this.Realm.features[j];
				ProvinceFeature.Def def = game.defs.Get<ProvinceFeature.Def>(id);
				if (def != null && def.show_in_political_view && def.show_in_castle)
				{
					UIProvinceFeature.GetIcon(this.Realm.features[j], null, this.m_ProvinceFeatureContainer);
				}
			}
			StackableIconsContainer component = this.m_ProvinceFeatureContainer.GetComponent<StackableIconsContainer>();
			if (component != null)
			{
				component.Refresh();
			}
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnMessage(object obj, string message, object param)
		{
		}

		// Token: 0x04003D85 RID: 15749
		[UIFieldTarget("id_ProvinceFeatureContainer")]
		private RectTransform m_ProvinceFeatureContainer;

		// Token: 0x04003D86 RID: 15750
		public Logic.Realm Realm;

		// Token: 0x04003D87 RID: 15751
		private bool m_Initialzied;
	}

	// Token: 0x0200080C RID: 2060
	internal class UIGoodsProduction : MonoBehaviour, IListener
	{
		// Token: 0x06004F75 RID: 20341 RVA: 0x00235847 File Offset: 0x00233A47
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x00235860 File Offset: 0x00233A60
		public void SetObject(Logic.Realm realm)
		{
			this.Init();
			Logic.Realm realm2 = this.Realm;
			if (realm2 != null)
			{
				realm2.DelListener(this);
			}
			this.Realm = realm;
			Logic.Realm realm3 = this.Realm;
			if (realm3 != null)
			{
				realm3.AddListener(this);
			}
			this.PopulateGoods();
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x0023589C File Offset: 0x00233A9C
		private void PopulateGoods()
		{
			if (this.Realm == null)
			{
				return;
			}
			if (this.m_GoodsContainer == null)
			{
				return;
			}
			DT.Field defField = global::Defs.GetDefField("Resource", null);
			if (defField == null || defField.def == null || defField.def.defs == null)
			{
				return;
			}
			List<DT.Def> defs = defField.def.defs;
			UICommon.DeleteChildren(this.m_GoodsContainer);
			for (int i = 0; i < defs.Count; i++)
			{
				DT.Def def = defs[i];
				if (def != null && this.Realm.HasTag(def.field.key, 1))
				{
					UIGoodsIcon.GetIcon(def.field.key, null, this.m_GoodsContainer);
				}
			}
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x0023594A File Offset: 0x00233B4A
		private void LateUpdate()
		{
			if (this.m_Invalidate)
			{
				this.PopulateGoods();
				this.m_Invalidate = false;
			}
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x00235961 File Offset: 0x00233B61
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "refresh_tags")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x04003D88 RID: 15752
		[UIFieldTarget("id_GoodsContainer")]
		private RectTransform m_GoodsContainer;

		// Token: 0x04003D89 RID: 15753
		public Logic.Realm Realm;

		// Token: 0x04003D8A RID: 15754
		private bool m_Initialzied;

		// Token: 0x04003D8B RID: 15755
		private bool m_Invalidate;
	}

	// Token: 0x0200080D RID: 2061
	internal class UIProvinceIllustration : MonoBehaviour
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06004F7B RID: 20347 RVA: 0x00235977 File Offset: 0x00233B77
		// (set) Token: 0x06004F7C RID: 20348 RVA: 0x0023597F File Offset: 0x00233B7F
		public Castle Data { get; protected set; }

		// Token: 0x06004F7D RID: 20349 RVA: 0x00235988 File Offset: 0x00233B88
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F7E RID: 20350 RVA: 0x002359A1 File Offset: 0x00233BA1
		public void SetObject(Castle r)
		{
			this.Init();
			this.Data = r;
			this.BuildIllustration();
		}

		// Token: 0x06004F7F RID: 20351 RVA: 0x002359B8 File Offset: 0x00233BB8
		private void BuildIllustration()
		{
			string text = this.Data.game.GetClimateZone(this.Data.position).ToString();
			bool @bool = this.Data.GetRealm().def.GetBool("has_mountains", null, false, true, true, true, '.');
			bool available = this.Data.game.GetTerrainType(this.Data.position) == TerrainType.Hills;
			bool bool2 = this.Data.GetRealm().def.GetBool("has_rivers", null, false, true, true, true, '.');
			bool coastal = this.Data.coastal;
			bool forest = this.Data.forest;
			if (this.m_Settlement != null)
			{
				Sprite illustration = this.GetIllustration(this.Data, text);
				this.m_Settlement.gameObject.SetActive(illustration != null);
				if (illustration != null)
				{
					this.m_Settlement.sprite = illustration;
				}
			}
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(true, this.m_Sky, text, "sky");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(@bool, this.m_Mountain, text, "mountains");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(available, this.m_Hills, text, "hills");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(true, this.m_TerrainBase, text, "base");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(forest, this.m_Forest, text, "forest");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(coastal, this.m_Ocean, text, "ocean");
			UICastleWindow.UIProvinceIllustration.<BuildIllustration>g__UpdateFeatureSprite|15_0(bool2, this.m_River, text, "river");
		}

		// Token: 0x06004F80 RID: 20352 RVA: 0x00235B38 File Offset: 0x00233D38
		private Sprite GetIllustration(Castle castle, string climateZome)
		{
			string houses_architecture = (castle.visuals as global::Settlement).houses_architecture;
			string text = "settlements.Village";
			string text2 = text + "." + houses_architecture;
			string key = text2 + "." + climateZome;
			Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", key, null);
			if (obj == null)
			{
				obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", text2, null);
			}
			if (obj == null)
			{
				obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", text, null);
			}
			return obj;
		}

		// Token: 0x06004F82 RID: 20354 RVA: 0x00235BB8 File Offset: 0x00233DB8
		[CompilerGenerated]
		internal static void <BuildIllustration>g__UpdateFeatureSprite|15_0(bool available, Image i, string cz, string feature)
		{
			if (i == null)
			{
				return;
			}
			i.gameObject.SetActive(available);
			if (!available)
			{
				return;
			}
			Sprite obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature + "." + cz, null);
			if (obj == null)
			{
				obj = global::Defs.GetObj<Sprite>("SettlementIllustrationSettings", "terrain." + feature, null);
			}
			i.gameObject.SetActive(obj != null);
			if (obj != null)
			{
				i.overrideSprite = obj;
			}
		}

		// Token: 0x04003D8C RID: 15756
		[UIFieldTarget("id_Sky")]
		protected Image m_Sky;

		// Token: 0x04003D8D RID: 15757
		[UIFieldTarget("id_Mountain")]
		protected Image m_Mountain;

		// Token: 0x04003D8E RID: 15758
		[UIFieldTarget("id_Hills")]
		protected Image m_Hills;

		// Token: 0x04003D8F RID: 15759
		[UIFieldTarget("id_TerrainBase")]
		protected Image m_TerrainBase;

		// Token: 0x04003D90 RID: 15760
		[UIFieldTarget("id_Ocean")]
		protected Image m_Ocean;

		// Token: 0x04003D91 RID: 15761
		[UIFieldTarget("id_River")]
		protected Image m_River;

		// Token: 0x04003D92 RID: 15762
		[UIFieldTarget("id_Forest")]
		protected Image m_Forest;

		// Token: 0x04003D93 RID: 15763
		[UIFieldTarget("id_Settlement")]
		protected Image m_Settlement;

		// Token: 0x04003D95 RID: 15765
		private bool m_Initialzied;
	}
}
