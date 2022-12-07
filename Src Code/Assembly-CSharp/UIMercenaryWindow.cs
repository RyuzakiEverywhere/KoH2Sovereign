using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class UIMercenaryWindow : ObjectWindow
{
	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060021FF RID: 8703 RVA: 0x00132D25 File Offset: 0x00130F25
	// (set) Token: 0x06002200 RID: 8704 RVA: 0x00132D2D File Offset: 0x00130F2D
	public Mercenary logic { get; private set; }

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06002201 RID: 8705 RVA: 0x00132D36 File Offset: 0x00130F36
	// (set) Token: 0x06002202 RID: 8706 RVA: 0x00132D3E File Offset: 0x00130F3E
	public Logic.Army Army { get; private set; }

	// Token: 0x06002203 RID: 8707 RVA: 0x00132D47 File Offset: 0x00130F47
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x00132D50 File Offset: 0x00130F50
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
		Mercenary mercenary = worldUI.selected_logic_obj as Mercenary;
		Logic.Army army = null;
		if (mercenary == null)
		{
			global::Army component = worldUI.selected_obj.GetComponent<global::Army>();
			if (component != null)
			{
				Mercenary mercenary2;
				component.HasTransferTarget(out mercenary2);
				if (mercenary2 != null)
				{
					mercenary = mercenary2;
					army = component.logic;
				}
			}
		}
		if (mercenary != null && (mercenary != this.logic || army != this.m_PreferedClient))
		{
			this.m_PreferedClient = army;
			this.SetObject(mercenary, new Vars(worldUI.selected_logic_obj));
		}
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x00132DEC File Offset: 0x00130FEC
	private void OnEnable()
	{
		if (this.logicObject == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x00132DFC File Offset: 0x00130FFC
	private void OnDisable()
	{
		this.Clean();
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x00132E04 File Offset: 0x00131004
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.UnitFramePrefab = UICommon.GetPrefab("UnitSlot", null);
		if (this.m_HireHelpWars != null)
		{
			for (int i = 0; i < this.m_HireHelpWars.Length; i++)
			{
				this.m_HireHelpWars[i].onClick = new BSGButton.OnClick(this.OnHireHelpWars);
			}
		}
		if (this.m_HireHelpRebels != null)
		{
			for (int j = 0; j < this.m_HireHelpRebels.Length; j++)
			{
				this.m_HireHelpRebels[j].onClick = new BSGButton.OnClick(this.OnHireHelpRebels);
			}
		}
		if (this.m_VisitingArmyArmyFoodContaner != null)
		{
			this.m_VisitingArmyArmyFood = this.m_VisitingArmyArmyFoodContaner.GetOrAddComponent<UIArmyFood>();
		}
		this.EquipmentFramePrefab = UICommon.GetPrefab("ArmyInvetoryItemSlot", null);
		this.m_Initialzied = true;
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x00132ED4 File Offset: 0x001310D4
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.Init();
		this.Clean();
		if (obj is Mercenary)
		{
			this.logic = (obj as Mercenary);
			this.logic.AddListener(this);
			if (this.logic.army != null)
			{
				this.Army = this.logic.army;
				this.Army.AddListener(this);
			}
			this.help_in_wars_mission = this.logic.game.defs.Find<MercenaryMission.Def>("HelpInWars");
			this.help_with_rebels_mission = this.logic.game.defs.Find<MercenaryMission.Def>("HelpWithRebels");
			Logic.Kingdom val = BaseUI.LogicKingdom();
			if (this.m_HireHelpWars != null)
			{
				this.help_in_wars_vars = new Vars(this.help_in_wars_mission);
				this.help_in_wars_vars.Set<Mercenary>("merc", this.logic);
				this.help_in_wars_vars.Set<Logic.Kingdom>("hire_kingdom", val);
				for (int i = 0; i < this.m_HireHelpWars.Length; i++)
				{
					Tooltip.Get(this.m_HireHelpWars[i].gameObject, true).SetDef("MercenaryMissionTooltip", this.help_in_wars_vars);
				}
			}
			if (this.m_HireHelpRebels != null)
			{
				this.help_with_rebel_vars = new Vars(this.help_with_rebels_mission);
				this.help_with_rebel_vars.Set<Mercenary>("merc", this.logic);
				this.help_with_rebel_vars.Set<Logic.Kingdom>("hire_kingdom", val);
				for (int j = 0; j < this.m_HireHelpRebels.Length; j++)
				{
					Tooltip.Get(this.m_HireHelpRebels[j].gameObject, true).SetDef("MercenaryMissionTooltip", this.help_with_rebel_vars);
				}
			}
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
			UIArmyManpower manpower = this.m_Manpower;
			Logic.Army army = this.Army;
			manpower.AddArmy(((army != null) ? army.visuals : null) as global::Army);
		}
		if (this.m_MercMorale != null)
		{
			this.m_MercMorale.Clear();
			UIArmyMorale mercMorale = this.m_MercMorale;
			Logic.Army army2 = this.Army;
			mercMorale.AddArmy(((army2 != null) ? army2.visuals : null) as global::Army);
		}
		if (this.m_ProvinceIllustration != null)
		{
			this.m_ProvinceIllustration.SetObject(this.Army);
		}
		if (this.m_VisitingArmyArmyFood != null)
		{
			this.m_VisitingArmyArmyFood.Clear();
		}
		this.Refresh();
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x00133134 File Offset: 0x00131334
	private UIMercenaryWindow.State DesideState()
	{
		if (this.logic == null)
		{
			return UIMercenaryWindow.State.Normal;
		}
		if (!this.logic.ValidForHireAsArmy() && !this.logic.army.IsHeadless())
		{
			return UIMercenaryWindow.State.Hired;
		}
		List<Logic.Army> validBuyers = this.GetValidBuyers(this.logic);
		if (validBuyers != null && validBuyers.Count > 0)
		{
			return UIMercenaryWindow.State.Visitors;
		}
		return UIMercenaryWindow.State.Normal;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x00133188 File Offset: 0x00131388
	public override void Refresh()
	{
		this.state = this.DesideState();
		this.m_GroupNormal.gameObject.SetActive(this.state == UIMercenaryWindow.State.Normal);
		this.m_GroupVisitors.gameObject.SetActive(this.state == UIMercenaryWindow.State.Visitors);
		this.m_GroupHired.gameObject.SetActive(this.state == UIMercenaryWindow.State.Hired);
		if (this.m_GroupMercUnits != null)
		{
			Vector3 localPosition = this.m_GroupMercUnits.localPosition;
			this.m_GroupMercUnits.localPosition = ((this.state == UIMercenaryWindow.State.Hired) ? new Vector3(0f, localPosition.y) : new Vector3(-348f, localPosition.y));
		}
		this.BuildOwn();
		this.BuildVisitors();
		this.UpdateLeader();
		this.UpdateShields();
		this.UpdateButtons();
		this.UpdateDescription();
		this.UpdateUpkeep();
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x00133268 File Offset: 0x00131468
	protected override void Update()
	{
		base.Update();
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
		if (this.m_InvalidateDescriptionCurrentAction)
		{
			this.UpdateDescription();
			this.m_InvalidateDescriptionCurrentAction = false;
		}
		this.UpdateHireButtons();
		this.SetCosts();
		this.CheckVisitors();
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x001332B8 File Offset: 0x001314B8
	private void SetCosts()
	{
		if (this.vars == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		this.vars.Set<Resource>("available", kingdom.resources);
		this.vars.Set<Resource>("upkeep_merc", this.logic.army.GetUpkeepMerc(kingdom));
		this.help_in_wars_vars.Set<Resource>("available", kingdom.resources);
		this.help_with_rebel_vars.Set<Resource>("available", kingdom.resources);
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x00133338 File Offset: 0x00131538
	private void BuildOwn()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.Army == null)
		{
			return;
		}
		int num = this.Army.IsHeadless() ? this.Army.MaxUnits() : this.logic.def.units_max_cnt;
		num = Mathf.Max(num, this.Army.CountUnits());
		num = Mathf.Max(num, this.logic.def.units_max_cnt);
		this.mercUnitSlot = new UIUnitSlot[num];
		this.CreateUnitFrames(this.Army.visuals as global::Army, ref this.mercUnitSlot, this.m_MercUnitSlotsContainer);
		this.UpdateUnitFrames(this.Army.visuals as global::Army, ref this.mercUnitSlot, this.logic.selected_buyer, BaseUI.LogicKingdom().id != this.logic.former_owner_id);
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x0013341C File Offset: 0x0013161C
	private void BuildVisitors()
	{
		for (int i = 0; i < this.currentVisitors.Count; i++)
		{
			this.currentVisitors[i].Clear();
		}
		this.currentVisitors.Clear();
		List<Logic.Army> validBuyers = this.GetValidBuyers(this.logic);
		if (validBuyers == null || validBuyers.Count == 0)
		{
			this.SelectVisitorArmy(null);
			return;
		}
		for (int j = 0; j < validBuyers.Count; j++)
		{
			Logic.Army army = validBuyers[j];
			if (army != null && army.leader != null && army.IsValid())
			{
				UIMercenaryWindow.Visitor visitor = new UIMercenaryWindow.Visitor(army, this);
				visitor.OnDestoryed += this.Visitor_OnDestoryed;
				this.currentVisitors.Add(visitor);
			}
		}
		if (this.logic.selected_buyer != null && !this.logic.selected_buyer.IsValid())
		{
			this.logic.selected_buyer = null;
		}
		this.SelectVisitorArmy((this.m_PreferedClient != null) ? this.m_PreferedClient : validBuyers[0]);
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x00133518 File Offset: 0x00131718
	private List<Logic.Army> GetValidBuyers(Mercenary mercenary)
	{
		if (mercenary == null || mercenary.buyers == null || mercenary.buyers.Count == 0)
		{
			return null;
		}
		List<Logic.Army> list = new List<Logic.Army>();
		for (int i = 0; i < mercenary.buyers.Count; i++)
		{
			if (mercenary.buyers[i].GetKingdom() == BaseUI.LogicKingdom() && mercenary.buyers[i].IsValid())
			{
				list.Add(mercenary.buyers[i]);
			}
		}
		return list;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x0013359C File Offset: 0x0013179C
	private void CheckVisitors()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (this.logic.selected_buyer != null && this.logic.selected_buyer == worldUI.selected_logic_obj)
		{
			if (this.logic.selected_buyer.movement.IsMoving(true))
			{
				worldUI.SelectObj(worldUI.selected_obj, false, true, true, true);
				return;
			}
			if (this.logic.selected_buyer.battle != null)
			{
				worldUI.SelectObj(worldUI.selected_obj, false, true, true, true);
				return;
			}
			if (this.logic.selected_buyer.castle != null)
			{
				worldUI.SelectObj(worldUI.selected_obj, false, true, true, true);
			}
		}
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x0013364C File Offset: 0x0013184C
	private void UpdateUpkeep()
	{
		if (this.state != UIMercenaryWindow.State.Hired)
		{
			return;
		}
		if (this.m_Upkeep == null)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<Resource>("upkeep", this.logic.army.GetUpkeep());
		UIText.SetTextKey(this.m_Upkeep, "Mercenary.upkeep_text", vars, null);
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x001336A5 File Offset: 0x001318A5
	private void Visitor_OnDestoryed(UIMercenaryWindow.Visitor visistor)
	{
		this.BuildVisitors();
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x001336AD File Offset: 0x001318AD
	private void HandleOnLeaderSelect(UICharacterIcon leaderIcon)
	{
		this.SelectVisitorArmy(leaderIcon.Data.GetArmy());
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x001336C0 File Offset: 0x001318C0
	private void SelectVisitorArmy(Logic.Army visitor)
	{
		if (this.logic == null)
		{
			return;
		}
		this.logic.selected_buyer = visitor;
		if (visitor != null)
		{
			if (this.m_IconVisitingLeader != null)
			{
				this.m_IconVisitingLeader.SetObject(visitor.leader, null);
			}
			if (this.m_VisitingArmyManpower != null)
			{
				this.m_VisitingArmyManpower.Clear();
				this.m_VisitingArmyManpower.AddArmy(visitor.visuals as global::Army);
			}
			if (this.m_VisitingArmyArmyFood != null)
			{
				this.m_VisitingArmyArmyFood.Clear();
				this.m_VisitingArmyArmyFood.AddArmy(visitor.visuals as global::Army);
			}
			if (this.visitingUnitSlot == null || this.visitingUnitSlot.Length != visitor.MaxUnits())
			{
				this.visitingUnitSlot = new UIUnitSlot[visitor.MaxUnits()];
				this.CreateUnitFrames(visitor.visuals as global::Army, ref this.visitingUnitSlot, this.m_VisitingArmyContainer);
			}
			this.UpdateUnitFrames(visitor.visuals as global::Army, ref this.visitingUnitSlot, null, false);
		}
		for (int i = 0; i < this.currentVisitors.Count; i++)
		{
			this.currentVisitors[i].Select(this.currentVisitors[i].Data == visitor);
		}
		this.CreateVisitorEquipmentFrame();
		this.UpdateVisitorEquipment();
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x0013380C File Offset: 0x00131A0C
	private void CreateVisitorEquipmentFrame()
	{
		if (this.EquipmentFramePrefab == null)
		{
			return;
		}
		if (this.equipmentSlotsContainer == null)
		{
			return;
		}
		int num = 4;
		if (this.equipmentSlots == null)
		{
			this.equipmentSlots = new List<UIArmyItemIcon>(10);
		}
		if (this.equipmentSlots.Count < num)
		{
			int num2 = num - this.equipmentSlots.Count;
			for (int i = 0; i < num2; i++)
			{
				UIArmyItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.EquipmentFramePrefab, this.equipmentSlotsContainer).GetComponent<UIArmyItemIcon>();
				this.equipmentSlots.Add(component);
			}
		}
		for (int j = 0; j < this.equipmentSlots.Count; j++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.equipmentSlots[j];
			if (!(uiarmyItemIcon == null))
			{
				if (j > num)
				{
					uiarmyItemIcon.gameObject.SetActive(false);
				}
				else
				{
					UIArmyItemIcon uiarmyItemIcon2 = uiarmyItemIcon;
					InventoryItem unit = null;
					int slotIndex = j;
					Logic.Army selected_buyer = this.logic.selected_buyer;
					uiarmyItemIcon2.SetUnitInstance(unit, slotIndex, (selected_buyer != null) ? selected_buyer.castle : null);
					uiarmyItemIcon.ShowAddIcon(false);
					uiarmyItemIcon.ShowDisbandButton(false);
				}
			}
		}
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x00133914 File Offset: 0x00131B14
	private void UpdateVisitorEquipment()
	{
		int count = this.equipmentSlots.Count;
		Logic.Army selected_buyer = this.logic.selected_buyer;
		List<InventoryItem> list = (selected_buyer != null) ? selected_buyer.siege_equipment : null;
		int num = (selected_buyer != null) ? selected_buyer.MaxItems() : 4;
		for (int i = 0; i < count; i++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.equipmentSlots[i];
			if (!(uiarmyItemIcon == null))
			{
				if (list != null && list.Count > i)
				{
					uiarmyItemIcon.SetUnitInstance(list[i], i, (selected_buyer != null) ? selected_buyer.castle : null);
					uiarmyItemIcon.ShowDisbandButton(true);
					uiarmyItemIcon.ShowAddIcon(num > i);
				}
				else
				{
					uiarmyItemIcon.SetDef(null, i, (selected_buyer != null) ? selected_buyer.castle : null, null);
					uiarmyItemIcon.ShowAddIcon(true);
					uiarmyItemIcon.ShowLockIcon(num <= i);
				}
			}
		}
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x001339F0 File Offset: 0x00131BF0
	private void CreateUnitFrames(global::Army army, ref UIUnitSlot[] slots, RectTransform container)
	{
		if (army == null)
		{
			return;
		}
		if (slots == null)
		{
			return;
		}
		if (this.UnitFramePrefab == null)
		{
			return;
		}
		if (container == null)
		{
			return;
		}
		UICommon.DeleteChildren(container);
		int num = slots.Length;
		for (int i = 0; i < num; i++)
		{
			UIUnitSlot component = UnityEngine.Object.Instantiate<GameObject>(this.UnitFramePrefab, container).GetComponent<UIUnitSlot>();
			if (component != null)
			{
				component.SetUnitInstance(null, i, (army != null) ? army.logic : null, null);
				slots[i] = component;
			}
		}
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x00133A71 File Offset: 0x00131C71
	private void UpdateUnitFrames(global::Army army, ref UIUnitSlot[] slots, Logic.Army buyer = null, bool show_hire_tooltip = false)
	{
		UIArmyWindow.UpdateUnitFrames((army != null) ? army.logic : null, slots);
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x00133A86 File Offset: 0x00131C86
	private void OnHireHelpWars(BSGButton b)
	{
		if (this.logic == null)
		{
			return;
		}
		this.logic.HireForKingdom(BaseUI.LogicKingdom(), this.help_in_wars_mission);
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x00133AA8 File Offset: 0x00131CA8
	private void OnHireHelpRebels(BSGButton b)
	{
		if (this.logic == null)
		{
			return;
		}
		this.logic.HireForKingdom(BaseUI.LogicKingdom(), this.help_with_rebels_mission);
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x00133ACC File Offset: 0x00131CCC
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
		if (obj is Logic.Army)
		{
			if (message == "units_changed")
			{
				if (this.logic.army == null)
				{
					return;
				}
				this.UpdateUnitFrames(this.logic.army.visuals as global::Army, ref this.mercUnitSlot, this.logic.selected_buyer, BaseUI.LogicKingdom().id != this.logic.former_owner_id);
				this.UpdateUpkeep();
			}
		}
		else if (message == "target_changed" || message == "action_changed")
		{
			this.m_InvalidateDescriptionCurrentAction = true;
		}
		if (message == "buyers_changed" || message == "command_changed" || message == "became_regular" || message == "became_mercenary")
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x00133BAF File Offset: 0x00131DAF
	public void RefreshVisistors()
	{
		if (this.logic.selected_buyer != null)
		{
			this.SelectVisitorArmy(this.logic.selected_buyer);
		}
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x00133BCF File Offset: 0x00131DCF
	public override void Release()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		if (this.Army != null)
		{
			this.Army.DelListener(this);
		}
		this.logic = null;
		this.Army = null;
		base.Release();
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x00133C10 File Offset: 0x00131E10
	private void Clean()
	{
		if (this.Army != null)
		{
			this.Army.DelListener(this);
		}
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
		this.logic = null;
		this.Army = null;
		if (this.currentVisitors != null && this.currentVisitors.Count > 0)
		{
			for (int i = 0; i < this.currentVisitors.Count; i++)
			{
				this.currentVisitors[i].Clear();
			}
		}
		this.currentVisitors.Clear();
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x00133C9B File Offset: 0x00131E9B
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.Clean();
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x00133CAC File Offset: 0x00131EAC
	private void UpdateLeader()
	{
		if (this.m_MercLeaderIcon != null && this.m_MercLeaderIcon.Length != 0 && this.logic != null)
		{
			for (int i = 0; i < this.m_MercLeaderIcon.Length; i++)
			{
				if (this.logic.army.leader == null)
				{
					this.m_MercLeaderIcon[i].gameObject.SetActive(false);
				}
				else
				{
					this.m_MercLeaderIcon[i].gameObject.SetActive(true);
					this.m_MercLeaderIcon[i].SetObject(this.logic.army.leader, null);
					this.m_MercLeaderIcon[i].ShowCrest(this.logic.kingdom_id != BaseUI.LogicKingdom().id);
					this.m_MercLeaderIcon[i].ShowStatus(false);
				}
			}
		}
		if (this.m_LeaderlessIcon != null && this.m_LeaderlessIcon.Length != 0)
		{
			for (int j = 0; j < this.m_LeaderlessIcon.Length; j++)
			{
				GameObject gameObject = this.m_LeaderlessIcon[j].gameObject;
				Mercenary logic = this.logic;
				object obj;
				if (logic == null)
				{
					obj = null;
				}
				else
				{
					Logic.Army army = logic.army;
					obj = ((army != null) ? army.leader : null);
				}
				gameObject.SetActive(obj == null);
			}
		}
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x00133DD8 File Offset: 0x00131FD8
	private void UpdateShields()
	{
		if (this.m_EmployerKingdomCrest != null)
		{
			this.m_EmployerKingdomCrest.SetObject(this.Army.GetKingdom(), null);
		}
		if (this.m_MercShield != null)
		{
			Vars vars = new Vars(this.Army.GetKingdom());
			vars.Set<Mercenary>("mercenary", this.logic);
			for (int i = 0; i < this.m_MercShield.Length; i++)
			{
				UIKingdomIcon uikingdomIcon = this.m_MercShield[i];
				if (uikingdomIcon != null)
				{
					uikingdomIcon.SetObject(this.Army.GetKingdom(), vars);
				}
			}
		}
		if (this.m_HeadlessCrestExtension != null)
		{
			Mercenary logic = this.logic;
			bool active = ((logic != null) ? logic.army : null) == null || this.logic.army.IsHeadless();
			for (int j = 0; j < this.m_HeadlessCrestExtension.Length; j++)
			{
				this.m_HeadlessCrestExtension[j].SetActive(active);
			}
		}
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x00133EBC File Offset: 0x001320BC
	private void UpdateDescription()
	{
		if (this.m_Description == null)
		{
			return;
		}
		if (this.logic.ValidForHireAsUnit())
		{
			string key;
			if (this.logic.former_owner_id == BaseUI.LogicKingdom().id)
			{
				key = "Mercenary.description_own_army";
			}
			else if (!this.logic.ValidForHireAsArmy())
			{
				key = "Mercenary.description_headless_other";
			}
			else
			{
				key = "Mercenary.description";
			}
			UIText.SetTextKey(this.m_Description, key, new Vars(this.logic), null);
			return;
		}
		Vars vars = new Vars(this.logic);
		Logic.Kingdom kingdom = this.logic.army.GetKingdom();
		if (kingdom == BaseUI.LogicKingdom())
		{
			UIText.ForceNextLinks(UIText.LinkSettings.Mode.AutoColorize);
			UIText.SetTextKey(this.m_MercStatus, "Mercenary.description_current_action", vars, null);
			return;
		}
		vars.Set<Logic.Kingdom>("hire_kingdom", kingdom);
		UIText.ForceNextLinks(UIText.LinkSettings.Mode.AutoColorize);
		UIText.SetTextKey(this.m_MercStatus, "Mercenary.hired_other_text", vars, null);
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x00133FB0 File Offset: 0x001321B0
	private void UpdateHireButtons()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.m_HireHelpWars != null && this.m_HireHelpRebels != null)
		{
			if (this.logic.army.IsHeadless())
			{
				for (int i = 0; i < this.m_HireHelpWars.Length; i++)
				{
					this.m_HireHelpWars[i].gameObject.SetActive(false);
				}
				for (int j = 0; j < this.m_HireHelpRebels.Length; j++)
				{
					this.m_HireHelpRebels[j].gameObject.SetActive(false);
				}
				return;
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			bool flag = this.logic.ValidForHireAsArmy();
			flag &= !this.logic.army.movement.IsMoving(true);
			bool enable = flag && this.help_in_wars_mission.Validate(this.logic, kingdom) && kingdom.resources.CanAfford(this.help_in_wars_mission.GetCost(this.logic, kingdom), 1f, Array.Empty<ResourceType>());
			for (int k = 0; k < this.m_HireHelpWars.Length; k++)
			{
				this.m_HireHelpWars[k].gameObject.SetActive(true);
				this.m_HireHelpWars[k].Enable(enable, false);
			}
			bool enable2 = flag && this.help_with_rebels_mission.Validate(this.logic, kingdom) && kingdom.resources.CanAfford(this.help_with_rebels_mission.GetCost(this.logic, kingdom), 1f, Array.Empty<ResourceType>());
			for (int l = 0; l < this.m_HireHelpRebels.Length; l++)
			{
				this.m_HireHelpRebels[l].gameObject.SetActive(true);
				this.m_HireHelpRebels[l].Enable(enable2, false);
			}
		}
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x00134170 File Offset: 0x00132370
	private void UpdateButtons()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.ValidForHireAsArmy())
		{
			return;
		}
		if (this.m_DismissCommand != null)
		{
			if (this.logic.army.GetKingdom() == BaseUI.LogicKingdom())
			{
				this.m_DismissCommand.gameObject.SetActive(true);
				this.m_DismissCommand.onClick = delegate(BSGButton x)
				{
					MessageWnd.Create("ConfirmDismissMercenaryMessage", this.vars, null, new MessageWnd.OnButton(this.OnDismissCommand));
				};
				Tooltip.Get(this.m_DismissCommand.gameObject, true).SetDef("DismissMercenaryTooltip", new Vars(this.logic));
				return;
			}
			this.m_DismissCommand.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x00134220 File Offset: 0x00132420
	private bool OnDismissCommand(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			Logic.Army army = this.logic.army;
			string eventPath;
			if (army == null)
			{
				eventPath = null;
			}
			else
			{
				Logic.Character leader = army.leader;
				eventPath = ((leader != null) ? leader.GetVoiceLine("DismissMercenaryArmy") : null);
			}
			Logic.Army army2 = this.logic.army;
			BaseUI.PlayVoiceEvent(eventPath, (army2 != null) ? army2.leader : null);
			this.logic.DismissOrRebel(true, null);
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x00132D47 File Offset: 0x00130F47
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x040016CC RID: 5836
	[UIFieldTarget("Group_Normal")]
	private GameObject m_GroupNormal;

	// Token: 0x040016CD RID: 5837
	[UIFieldTarget("Group_Hired")]
	private GameObject m_GroupHired;

	// Token: 0x040016CE RID: 5838
	[UIFieldTarget("Group_Visitors")]
	private GameObject m_GroupVisitors;

	// Token: 0x040016CF RID: 5839
	[UIFieldTarget("id_IconMercLeader")]
	private UICharacterIcon[] m_MercLeaderIcon;

	// Token: 0x040016D0 RID: 5840
	[UIFieldTarget("id_LeaderlessIcon")]
	private GameObject[] m_LeaderlessIcon;

	// Token: 0x040016D1 RID: 5841
	[UIFieldTarget("id_MercCrest")]
	private UIKingdomIcon[] m_MercShield;

	// Token: 0x040016D2 RID: 5842
	[UIFieldTarget("id_HeadlessCrestExtension")]
	private GameObject[] m_HeadlessCrestExtension;

	// Token: 0x040016D3 RID: 5843
	[UIFieldTarget("id_Group_MercUnits")]
	private RectTransform m_GroupMercUnits;

	// Token: 0x040016D4 RID: 5844
	[UIFieldTarget("id_EmployerKingdom")]
	private UIKingdomIcon m_EmployerKingdomCrest;

	// Token: 0x040016D5 RID: 5845
	[UIFieldTarget("id_Manpower")]
	private UIArmyManpower m_Manpower;

	// Token: 0x040016D6 RID: 5846
	[UIFieldTarget("id_MercMorale")]
	private UIArmyMorale m_MercMorale;

	// Token: 0x040016D7 RID: 5847
	[UIFieldTarget("id_IconVisitingLeader")]
	private UICharacterIcon m_IconVisitingLeader;

	// Token: 0x040016D8 RID: 5848
	[UIFieldTarget("id_VisitingArmyManpower")]
	private UIArmyManpower m_VisitingArmyManpower;

	// Token: 0x040016D9 RID: 5849
	[UIFieldTarget("id_VisitingArmyArmyFoodContaner")]
	private GameObject m_VisitingArmyArmyFoodContaner;

	// Token: 0x040016DA RID: 5850
	[UIFieldTarget("id_VisitingArmyEquipment")]
	private RectTransform equipmentSlotsContainer;

	// Token: 0x040016DB RID: 5851
	[UIFieldTarget("id_MercUnitsContainer")]
	private RectTransform m_MercUnitSlotsContainer;

	// Token: 0x040016DC RID: 5852
	[UIFieldTarget("id_VisitingArmyContainer")]
	private RectTransform m_VisitingArmyContainer;

	// Token: 0x040016DD RID: 5853
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x040016DE RID: 5854
	[UIFieldTarget("id_MercStatus")]
	private TextMeshProUGUI m_MercStatus;

	// Token: 0x040016DF RID: 5855
	[UIFieldTarget("id_HireHelpWars")]
	private BSGButton[] m_HireHelpWars;

	// Token: 0x040016E0 RID: 5856
	[UIFieldTarget("id_HireHelpRebels")]
	private BSGButton[] m_HireHelpRebels;

	// Token: 0x040016E1 RID: 5857
	[UIFieldTarget("id_DismissCommand")]
	private BSGButton m_DismissCommand;

	// Token: 0x040016E2 RID: 5858
	[UIFieldTarget("id_Upkeep")]
	private TextMeshProUGUI m_Upkeep;

	// Token: 0x040016E3 RID: 5859
	[UIFieldTarget("id_ProvinceIllustration")]
	private UIProvinceIllustration m_ProvinceIllustration;

	// Token: 0x040016E4 RID: 5860
	private GameObject UnitFramePrefab;

	// Token: 0x040016E5 RID: 5861
	private UIUnitSlot[] mercUnitSlot;

	// Token: 0x040016E6 RID: 5862
	private UIUnitSlot[] visitingUnitSlot;

	// Token: 0x040016E7 RID: 5863
	private UIArmyFood m_VisitingArmyArmyFood;

	// Token: 0x040016E8 RID: 5864
	private List<UIMercenaryWindow.Visitor> currentVisitors = new List<UIMercenaryWindow.Visitor>();

	// Token: 0x040016E9 RID: 5865
	private List<UIArmyItemIcon> equipmentSlots;

	// Token: 0x040016EC RID: 5868
	private Logic.Army m_PreferedClient;

	// Token: 0x040016ED RID: 5869
	private UIMercenaryWindow.State state;

	// Token: 0x040016EE RID: 5870
	private bool m_Invalidate;

	// Token: 0x040016EF RID: 5871
	private bool m_InvalidateDescriptionCurrentAction;

	// Token: 0x040016F0 RID: 5872
	private bool m_Initialzied;

	// Token: 0x040016F1 RID: 5873
	private MercenaryMission.Def help_in_wars_mission;

	// Token: 0x040016F2 RID: 5874
	private Vars help_in_wars_vars;

	// Token: 0x040016F3 RID: 5875
	private MercenaryMission.Def help_with_rebels_mission;

	// Token: 0x040016F4 RID: 5876
	private Vars help_with_rebel_vars;

	// Token: 0x040016F5 RID: 5877
	private GameObject EquipmentFramePrefab;

	// Token: 0x02000785 RID: 1925
	private enum State
	{
		// Token: 0x04003AEF RID: 15087
		Normal,
		// Token: 0x04003AF0 RID: 15088
		Hired,
		// Token: 0x04003AF1 RID: 15089
		Visitors
	}

	// Token: 0x02000786 RID: 1926
	public class Visitor : IListener
	{
		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06004C76 RID: 19574 RVA: 0x00229D99 File Offset: 0x00227F99
		// (set) Token: 0x06004C77 RID: 19575 RVA: 0x00229DA1 File Offset: 0x00227FA1
		public Logic.Army Data { get; private set; }

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06004C78 RID: 19576 RVA: 0x00229DAC File Offset: 0x00227FAC
		// (remove) Token: 0x06004C79 RID: 19577 RVA: 0x00229DE4 File Offset: 0x00227FE4
		public event Action<UIMercenaryWindow.Visitor> OnDestoryed;

		// Token: 0x06004C7A RID: 19578 RVA: 0x00229E19 File Offset: 0x00228019
		public Visitor(Logic.Army army, UIMercenaryWindow window)
		{
			this.m_Window = window;
			this.Data = army;
			army.AddListener(this);
			new Vars(army.leader);
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x000023FD File Offset: 0x000005FD
		public void Select(bool selected)
		{
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x00229E48 File Offset: 0x00228048
		public void OnMessage(object obj, string message, object param)
		{
			if (!(message == "destroying") && !(message == "finishing"))
			{
				if (message == "units_changed")
				{
					UIMercenaryWindow window = this.m_Window;
					if (window == null)
					{
						return;
					}
					window.RefreshVisistors();
				}
				return;
			}
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
			Action<UIMercenaryWindow.Visitor> onDestoryed = this.OnDestoryed;
			if (onDestoryed == null)
			{
				return;
			}
			onDestoryed(this);
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x00229EB2 File Offset: 0x002280B2
		public void Clear()
		{
			if (this.Data != null)
			{
				this.Data.DelListener(this);
			}
			UnityEngine.Object.Destroy(this.m_Icon);
			this.m_Window = null;
			this.Data = null;
		}

		// Token: 0x04003AF3 RID: 15091
		private GameObject m_Icon;

		// Token: 0x04003AF4 RID: 15092
		private UIMercenaryWindow m_Window;
	}
}
