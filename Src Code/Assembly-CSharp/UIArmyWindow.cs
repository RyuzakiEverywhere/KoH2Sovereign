using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200019D RID: 413
public class UIArmyWindow : ObjectWindow, IPoolable
{
	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06001741 RID: 5953 RVA: 0x000E5B91 File Offset: 0x000E3D91
	// (set) Token: 0x06001742 RID: 5954 RVA: 0x000E5B99 File Offset: 0x000E3D99
	public Logic.Army logic { get; private set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06001743 RID: 5955 RVA: 0x000E5BA2 File Offset: 0x000E3DA2
	// (set) Token: 0x06001744 RID: 5956 RVA: 0x000E5BAA File Offset: 0x000E3DAA
	public Logic.Army unitTransferTarget { get; private set; }

	// Token: 0x06001745 RID: 5957 RVA: 0x000E5BB3 File Offset: 0x000E3DB3
	private void Start()
	{
		if (this.logicObject == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x000E5BB3 File Offset: 0x000E3DB3
	private void OnEnable()
	{
		if (this.logicObject == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x000E5BC4 File Offset: 0x000E3DC4
	private void ExtractLogicObject()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null || worldUI.selected_obj == null)
		{
			return;
		}
		global::Army component = worldUI.selected_obj.GetComponent<global::Army>();
		Logic.Army army = (component != null) ? component.logic : null;
		if (army != null && army != this.logic)
		{
			this.SetObject(army, null);
		}
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x000E5C24 File Offset: 0x000E3E24
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		this.Init();
		if (vars == null)
		{
			vars = new Vars(obj);
		}
		base.SetObject(obj, vars);
		if (this.m_ArmyMorale != null)
		{
			this.m_ArmyMorale.Clear();
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
		}
		Logic.Object logicObject = this.logicObject;
		this.armyVisuals = (((logicObject != null) ? logicObject.visuals : null) as global::Army);
		this.logic = (this.logicObject as Logic.Army);
		Logic.Army logic = this.logic;
		this.m_CurrentMaxUnitSlots = ((logic != null) ? logic.MaxUnits() : 6);
		Logic.Army logic2 = this.logic;
		this.m_CcurrentMaxItemSlots = ((logic2 != null) ? logic2.MaxItems() : 4);
		if (this.m_ArmyMorale != null)
		{
			UIArmyMorale armyMorale = this.m_ArmyMorale;
			Logic.Army logic3 = this.logic;
			armyMorale.AddArmy(((logic3 != null) ? logic3.visuals : null) as global::Army);
		}
		if (this.m_Manpower != null)
		{
			UIArmyManpower manpower = this.m_Manpower;
			Logic.Army logic4 = this.logic;
			manpower.AddArmy(((logic4 != null) ? logic4.visuals : null) as global::Army);
		}
		if (this.m_KingdomCrest != null)
		{
			Logic.Army logic5 = this.logic;
			Logic.Kingdom kingdom = (logic5 != null) ? logic5.GetKingdom() : null;
			this.m_KingdomCrest.SetObject(obj, null);
			this.m_KingdomCrest.gameObject.SetActive(kingdom != null);
		}
		if (this.m_ProvinceIllustration != null)
		{
			this.m_ProvinceIllustration.SetObject(this.logic);
		}
		if (this.m_Resupply != null)
		{
			Tooltip.Get(this.m_Resupply.gameObject, true).SetDef("ArmySuppliesTooltip", vars);
		}
		if (this.m_ArmyFood != null)
		{
			this.m_ArmyFood.Clear();
			this.m_ArmyFood.AddArmy(this.logic.visuals as global::Army);
		}
		this.Refresh();
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x000E5E04 File Offset: 0x000E4004
	public void SetUnitTransferTarget(Logic.Army army)
	{
		this.unitTransferTarget = army;
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x000E5E0D File Offset: 0x000E400D
	public void SetTransferWindow(UIArmyTransferWindow window)
	{
		this.m_ArmyTransferWindow = window;
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x000E5E18 File Offset: 0x000E4018
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.UnitFramePrefab = UICommon.GetPrefab("UnitSlot", null);
		this.EquipmentFramePrefab = UICommon.GetPrefab("ArmyInvetoryItemSlot", null);
		if (this.m_Resupply != null)
		{
			this.m_Resupply.onClick = new BSGButton.OnClick(this.HandleResupply);
		}
		new Vars(this);
		if (this.m_ArmyFoodContainer != null)
		{
			this.m_ArmyFood = this.m_ArmyFoodContainer.GetOrAddComponent<UIArmyFood>();
		}
		this.m_Initialzied = true;
		if (this.marshalicon != null)
		{
			this.marshalicon.OnPointerEvent += this.OnCharacterPointerEvent;
		}
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x000E5ED0 File Offset: 0x000E40D0
	private void OnCharacterPointerEvent(Hotspot h, EventTriggerType et, PointerEventData e)
	{
		if (et != EventTriggerType.PointerUp)
		{
			return;
		}
		UICharacterIcon uicharacterIcon = h as UICharacterIcon;
		object obj;
		if (uicharacterIcon == null)
		{
			obj = null;
		}
		else
		{
			Logic.Character data = uicharacterIcon.Data;
			if (data == null)
			{
				obj = null;
			}
			else
			{
				Logic.Army army = data.GetArmy();
				obj = ((army != null) ? army.visuals : null);
			}
		}
		global::Army army2 = obj as global::Army;
		if (((army2 != null) ? army2.gameObject : null) != null)
		{
			GameObject selectionObj = BaseUI.Get().GetSelectionObj(army2.gameObject);
			if (selectionObj == null)
			{
				return;
			}
			if (e.clickCount > 1)
			{
				BaseUI.Get().LookAt(selectionObj.transform.position, false);
			}
		}
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x000E5F60 File Offset: 0x000E4160
	public override void Refresh()
	{
		this.UpdateLeader();
		this.CreateUnitFrames();
		this.UpdateUnitFrames();
		this.CreateEquipmentFrame();
		this.UpdateEquipment();
		this.UpdateActions();
		this.SetupReplenishAction();
		this.RefreshStatuses();
		this.UpdateFocusedUnit();
		this.UpdateFocusedInvetoryItems();
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000E5F9E File Offset: 0x000E419E
	private void OnHireWindowClose(UIWindow window)
	{
		this.m_CurrentHireWindow = null;
		window.on_close = (UIWindow.OnClose)Delegate.Remove(window.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
		this.UpdateFocusedUnit();
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x000E5FCF File Offset: 0x000E41CF
	private void OnPurchaseWindowClose(UIWindow window)
	{
		this.m_CurrentPurchaseWindow = null;
		window.on_close = (UIWindow.OnClose)Delegate.Remove(window.on_close, new UIWindow.OnClose(this.OnPurchaseWindowClose));
		this.UpdateFocusedInvetoryItems();
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x000E6000 File Offset: 0x000E4200
	public override void AddListeners()
	{
		if (this.logic != null)
		{
			if (this.logic.castle != null)
			{
				this.logic.castle.AddListener(this);
			}
			if (this.logic.leader != null)
			{
				this.logic.leader.AddListener(this);
			}
		}
		base.AddListeners();
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x000E6058 File Offset: 0x000E4258
	public override void RemoveListeners()
	{
		if (this.logic != null)
		{
			if (this.logic.castle != null)
			{
				this.logic.castle.DelListener(this);
			}
			if (this.logic.leader != null)
			{
				this.logic.leader.DelListener(this);
			}
		}
		base.RemoveListeners();
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x000E60B0 File Offset: 0x000E42B0
	private void UpdateLeader()
	{
		if (this.marshalicon != null && this.logic != null)
		{
			this.marshalicon.SetObject(this.logic.leader, null);
			BaseUI baseUI = BaseUI.Get();
			this.marshalicon.ShowCrest(this.logic.kingdom_id != baseUI.kingdom.id);
			this.marshalicon.ShowStatus(false);
			this.marshalicon.EnableClassLevel(true);
		}
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x000E6130 File Offset: 0x000E4330
	private void UpdateActions()
	{
		if (this.logic == null)
		{
			return;
		}
		bool active = this.logic.CanResupply();
		if (this.m_Resupply != null)
		{
			this.m_Resupply.gameObject.SetActive(active);
		}
		if (this.m_ResupplyIcon != null)
		{
			this.m_ResupplyIcon.gameObject.SetActive(active);
		}
		if (this.m_HealActionIcon != null)
		{
			UIActionIcon healActionIcon = this.m_HealActionIcon;
			string text;
			if (healActionIcon == null)
			{
				text = null;
			}
			else
			{
				ActionVisuals data = healActionIcon.Data;
				text = ((data != null) ? data.logic.Validate(false) : null);
			}
			string text2 = text;
			bool active2 = text2 == "ok" || text2.StartsWith("_", StringComparison.Ordinal);
			this.m_HealActionIcon.gameObject.SetActive(active2);
		}
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x000E61F4 File Offset: 0x000E43F4
	private void RefreshStatuses()
	{
		Logic.Army logic = this.logic;
		bool flag;
		if (logic == null)
		{
			flag = (null != null);
		}
		else
		{
			Statuses statuses = logic.statuses;
			flag = (((statuses != null) ? statuses.Get(0) : null) != null);
		}
		int num = (!flag) ? 1 : 0;
		for (int i = 0; i < this.m_Statuses.Length; i++)
		{
			UIStatusIcon uistatusIcon = this.m_Statuses[i];
			if (!(uistatusIcon == null))
			{
				bool flag2 = true;
				Logic.Army logic2 = this.logic;
				Logic.Status status;
				if (logic2 == null)
				{
					status = null;
				}
				else
				{
					Statuses statuses2 = logic2.statuses;
					status = ((statuses2 != null) ? statuses2.Get(i + num) : null);
				}
				Logic.Status status2 = status;
				if (status2 == null)
				{
					flag2 = false;
				}
				uistatusIcon.KeepAlive(true);
				uistatusIcon.gameObject.SetActive(flag2);
				if (flag2)
				{
					this.m_Statuses[i].SetObject(status2, null);
				}
			}
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000E62A0 File Offset: 0x000E44A0
	private void SetupReplenishAction()
	{
		if (this.m_Heal == null)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		Logic.Character leader = this.logic.leader;
		if (leader == null)
		{
			return;
		}
		if (leader.actions == null)
		{
			return;
		}
		for (int i = 0; i < leader.actions.Count; i++)
		{
			Action action = leader.actions[i];
			if (!(action.def.field.key != "CampArmyAction"))
			{
				Vars vars = new Vars(leader);
				this.m_HealActionIcon = UIActionIcon.Possess(action.visuals as ActionVisuals, this.m_Heal.gameObject, vars);
				if (this.m_HealActionIcon != null)
				{
					this.m_HealActionIcon.SetSkin("Neutral");
				}
			}
		}
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000E636C File Offset: 0x000E456C
	private void CreateUnitFrames()
	{
		if (this.UnitFramePrefab == null)
		{
			return;
		}
		if (this.unitSlotsContainer == null)
		{
			return;
		}
		if (this.armyVisuals == null)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		if (this.m_UnitSlots == null)
		{
			this.m_UnitSlots = new List<UIUnitSlot>(30);
		}
		int num = this.armyVisuals.logic.MaxUnits();
		if (this.m_UnitSlots.Count < num)
		{
			int num2 = num - this.m_UnitSlots.Count;
			for (int i = 0; i < num2; i++)
			{
				UIUnitSlot component = UnityEngine.Object.Instantiate<GameObject>(this.UnitFramePrefab, this.unitSlotsContainer).GetComponent<UIUnitSlot>();
				this.m_UnitSlots.Add(component);
			}
		}
		for (int j = 0; j < this.m_UnitSlots.Count; j++)
		{
			UIUnitSlot uiunitSlot = this.m_UnitSlots[j];
			if (!(uiunitSlot == null))
			{
				if (j >= num)
				{
					uiunitSlot.gameObject.SetActive(false);
				}
				else
				{
					uiunitSlot.gameObject.SetActive(true);
					uiunitSlot.SetUnitInstance(null, j, this.armyVisuals.logic, null);
				}
			}
		}
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000E648C File Offset: 0x000E468C
	private void CreateEquipmentFrame()
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
		if (this.m_EquipmentSlots == null)
		{
			this.m_EquipmentSlots = new List<UIArmyItemIcon>(10);
		}
		if (this.m_EquipmentSlots.Count < num)
		{
			int num2 = num - this.m_EquipmentSlots.Count;
			for (int i = 0; i < num2; i++)
			{
				UIArmyItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.EquipmentFramePrefab, this.equipmentSlotsContainer).GetComponent<UIArmyItemIcon>();
				component.OnSelected += this.HanldeOnEquipmentFrameClick;
				component.OnDisband += this.HanldeOnItemRemove;
				this.m_EquipmentSlots.Add(component);
			}
		}
		global::Army army = this.armyVisuals;
		bool shown = ((army != null) ? army.GetCastle() : null) != null;
		for (int j = 0; j < this.m_EquipmentSlots.Count; j++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.m_EquipmentSlots[j];
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
					Logic.Army logic = this.logic;
					uiarmyItemIcon2.SetUnitInstance(unit, slotIndex, (logic != null) ? logic.castle : null);
					uiarmyItemIcon.ShowAddIcon(shown);
					uiarmyItemIcon.ShowDisbandButton(false);
				}
			}
		}
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x000E65D0 File Offset: 0x000E47D0
	private void UpdateEquipment()
	{
		if (this.m_EquipmentSlots == null)
		{
			return;
		}
		if (this.m_EquipmentSlots.Count == 0)
		{
			return;
		}
		int count = this.m_EquipmentSlots.Count;
		Logic.Army logic = this.logic;
		List<InventoryItem> list = (logic != null) ? logic.siege_equipment : null;
		for (int i = 0; i < count; i++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.m_EquipmentSlots[i];
			if (!(uiarmyItemIcon == null))
			{
				if (list != null && list.Count > i)
				{
					UIArmyItemIcon uiarmyItemIcon2 = uiarmyItemIcon;
					InventoryItem unit = list[i];
					int slotIndex = i;
					Logic.Army logic2 = this.logic;
					uiarmyItemIcon2.SetUnitInstance(unit, slotIndex, (logic2 != null) ? logic2.castle : null);
					uiarmyItemIcon.ShowDisbandButton(true);
					uiarmyItemIcon.ShowAddIcon(this.m_CcurrentMaxItemSlots > i);
				}
				else
				{
					UIArmyItemIcon uiarmyItemIcon3 = uiarmyItemIcon;
					Logic.Unit.Def unitDef = null;
					int slotIndex2 = i;
					Logic.Army logic3 = this.logic;
					uiarmyItemIcon3.SetDef(unitDef, slotIndex2, (logic3 != null) ? logic3.castle : null, null);
					uiarmyItemIcon.ShowAddIcon(true);
					uiarmyItemIcon.ShowLockIcon(this.m_CcurrentMaxItemSlots <= i);
				}
			}
		}
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000E66B4 File Offset: 0x000E48B4
	public static void UpdateUnitFrames(Logic.Army logic, UIUnitSlot[] m_UnitSlots)
	{
		if (logic == null)
		{
			return;
		}
		if (m_UnitSlots == null)
		{
			return;
		}
		int num = m_UnitSlots.Length;
		if (logic.units.Count > 0)
		{
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				UIUnitSlot uiunitSlot = m_UnitSlots[i];
				if (!(uiunitSlot == null))
				{
					Logic.Unit unit = null;
					for (int j = num2; j < logic.units.Count; j++)
					{
						Logic.Unit unit2 = logic.units[j];
						if (unit2 != null && unit2.def.type != Logic.Unit.Type.Noble)
						{
							unit = unit2;
							num2 = j + 1;
							break;
						}
					}
					if (unit != null)
					{
						uiunitSlot.SetUnitInstance(unit, i, logic, null);
					}
					else
					{
						uiunitSlot.SetUnitInstance(null, i, logic, null);
					}
				}
			}
			return;
		}
		for (int k = 0; k < num; k++)
		{
			UIUnitSlot uiunitSlot2 = m_UnitSlots[k];
			if (!(uiunitSlot2 == null))
			{
				uiunitSlot2.SetUnitInstance(null, k, logic, null);
			}
		}
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000E678B File Offset: 0x000E498B
	private void UpdateUnitFrames()
	{
		UIArmyWindow.UpdateUnitFrames(this.logic, this.m_UnitSlots.ToArray());
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000E67A4 File Offset: 0x000E49A4
	private DT.Field GetThresholdField(DT.Field levels_field, float val)
	{
		if (((levels_field != null) ? levels_field.children : null) == null || levels_field.children.Count == 0)
		{
			return null;
		}
		DT.Field result = null;
		int num = int.MinValue;
		for (int i = 0; i < levels_field.children.Count; i++)
		{
			DT.Field field = levels_field.children[i];
			if (field.children != null && field.children.Count != 0)
			{
				int @int = field.GetInt("threshold", null, 0, true, true, true, '.');
				if ((float)@int <= val && @int > num)
				{
					num = @int;
					result = field;
				}
			}
		}
		return result;
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x000E6834 File Offset: 0x000E4A34
	private void SelectEquipmentSlot(UIArmyItemIcon slot)
	{
		for (int i = 0; i < this.m_EquipmentSlots.Count; i++)
		{
			this.m_EquipmentSlots[i].Select(this.m_EquipmentSlots[i] == slot);
		}
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x000E687C File Offset: 0x000E4A7C
	public void HanldeOnEquipmentFrameClick(UIArmyItemIcon itemSlot, PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			this.SelectEquipmentSlot(itemSlot);
			if (itemSlot.UnitInstance == null)
			{
				this.OpenEquipmentPurchaseWindow(itemSlot.SlotIndex, UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat equipment window", true));
				this.UpdateFocusedInvetoryItems();
			}
		}
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x000E68CE File Offset: 0x000E4ACE
	public void HanldeOnItemRemove(UIArmyItemIcon itemSlot)
	{
		if (itemSlot.UnitInstance != null)
		{
			itemSlot.UnitInstance.OnAssignedAnalytics(itemSlot.UnitInstance.army, "delete");
			this.RemoveEquipment(itemSlot.UnitInstance);
		}
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x000E6900 File Offset: 0x000E4B00
	public void OpenRecruitWindow(int slotIndex, bool debug = false)
	{
		Castle castle = this.armyVisuals.logic.castle;
		if (castle == null && !debug)
		{
			return;
		}
		if (this.m_CurrentHireWindow != null)
		{
			return;
		}
		GameObject gameObject = debug ? UIUnitRecruitmentWindow.GetPrefab("debug") : UIUnitRecruitmentWindow.GetPrefab(null);
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject2 = global::Common.FindChildByName((baseUI != null) ? baseUI.gameObject : null, "id_MessageContainer", true, true);
		if (gameObject != null && gameObject2 != null)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject, gameObject2.transform);
			this.m_CurrentHireWindow = gameObject3.GetComponent<UIUnitRecruitmentWindow>();
			if (this.m_CurrentHireWindow != null)
			{
				if (debug)
				{
					this.m_CurrentHireWindow.SetData(castle, this.logic, slotIndex, delegate(Logic.Unit.Def ud, int si)
					{
						this.HandleOnHireDebug(ud, si);
					}, debug);
				}
				else
				{
					this.m_CurrentHireWindow.SetData(castle, this.logic, slotIndex, new Action<Logic.Unit.Def, int>(this.HandleOnHire), debug);
				}
				UIUnitRecruitmentWindow currentHireWindow = this.m_CurrentHireWindow;
				currentHireWindow.on_close = (UIWindow.OnClose)Delegate.Combine(currentHireWindow.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
				if (debug)
				{
					UICommon.SetAligment(gameObject3.transform as RectTransform, TextAnchor.MiddleCenter);
				}
			}
			else
			{
				global::Common.DestroyObj(gameObject3);
			}
		}
		this.UpdateFocusedUnit();
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000E6A38 File Offset: 0x000E4C38
	private void HandleOnHire(Logic.Unit.Def unitDef, int slotIndex)
	{
		Castle castle = this.armyVisuals.logic.castle;
		if (castle == null)
		{
			return;
		}
		if (castle.HireUnit(unitDef, this.armyVisuals.logic, -1))
		{
			BaseUI.PlaySoundEvent(global::Defs.GetString(unitDef.id, "hire_sound", null, ""), null);
			return;
		}
		BaseUI.PlaySoundEvent(global::Defs.GetString(unitDef.id, "hire_failed_sound", null, ""), null);
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x000E6AA8 File Offset: 0x000E4CA8
	private void HandleOnHireDebug(Logic.Unit.Def unitDef, int slotIndex)
	{
		this.armyVisuals.logic.AddUnit(unitDef.name, -1, false, true);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x000E6AC4 File Offset: 0x000E4CC4
	private void DisbandUnit(Logic.Unit unit)
	{
		unit.OnAssignedAnalytics(unit.army, null, "army", "delete");
		this.armyVisuals.logic.DelUnit(unit, true);
		this.UpdateUnitFrames();
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x000E6AF8 File Offset: 0x000E4CF8
	public void OpenEquipmentPurchaseWindow(int slotIndex, bool debug = false)
	{
		Castle castle = this.armyVisuals.logic.castle;
		if (castle == null && !debug)
		{
			return;
		}
		if (this.m_CurrentPurchaseWindow != null)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject = global::Common.FindChildByName((baseUI != null) ? baseUI.gameObject : null, "id_MessageContainer", true, true);
		GameObject prefab = UIEquipmentPurchaseWindow.GetPrefab();
		if (prefab != null && gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(prefab, gameObject.transform);
			this.m_CurrentPurchaseWindow = gameObject2.GetComponent<UIEquipmentPurchaseWindow>();
			if (this.m_CurrentPurchaseWindow != null)
			{
				if (debug)
				{
					this.m_CurrentPurchaseWindow.SetData(castle, slotIndex, delegate(Logic.Unit.Def ud, int si)
					{
						this.HandleOnPurchaseDebug(ud, si);
						if (this.m_CurrentPurchaseWindow != null)
						{
							this.m_CurrentPurchaseWindow.Close(false);
						}
					}, debug);
				}
				else
				{
					this.m_CurrentPurchaseWindow.SetData(castle, slotIndex, new Action<Logic.Unit.Def, int>(this.HandleOnPurchase), debug);
				}
				UIEquipmentPurchaseWindow currentPurchaseWindow = this.m_CurrentPurchaseWindow;
				currentPurchaseWindow.on_close = (UIWindow.OnClose)Delegate.Combine(currentPurchaseWindow.on_close, new UIWindow.OnClose(this.OnPurchaseWindowClose));
				return;
			}
			UnityEngine.Object.Destroy(gameObject2);
		}
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x000E6BF8 File Offset: 0x000E4DF8
	private void HandleOnPurchase(Logic.Unit.Def unitDef, int slotIndex)
	{
		if (this.armyVisuals == null | this.armyVisuals.logic == null)
		{
			return;
		}
		Castle castle = this.armyVisuals.logic.castle;
		if (castle == null)
		{
			return;
		}
		if (castle.BuyEquipments(unitDef, this.armyVisuals.logic, slotIndex))
		{
			BaseUI.PlaySoundEvent(global::Defs.GetString(unitDef.id, "hire_sound", null, ""), null);
			return;
		}
		BaseUI.PlaySoundEvent(global::Defs.GetString(unitDef.id, "hire_failed_sound", null, ""), null);
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x000E6C86 File Offset: 0x000E4E86
	private void HandleOnPurchaseDebug(Logic.Unit.Def unitDef, int slotIndex)
	{
		this.armyVisuals.logic.AddInvetoryItem(unitDef.name, slotIndex, true);
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x000E6CA1 File Offset: 0x000E4EA1
	private void RemoveEquipment(InventoryItem item)
	{
		this.armyVisuals.logic.DelInvetoryItem(item, true);
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x000E6CB8 File Offset: 0x000E4EB8
	private void HandleResupply(BSGButton b)
	{
		global::Settlement castle = this.armyVisuals.GetCastle();
		Castle castle2 = ((castle != null) ? castle.logic : null) as Castle;
		if (castle2 == null)
		{
			return;
		}
		castle2.ResupplyArmy(this.armyVisuals.logic);
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("army_resupply", null, "", true, true, true, '.') : null, null);
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x000E6D20 File Offset: 0x000E4F20
	protected override void Update()
	{
		base.Update();
		if (this.logic == null)
		{
			return;
		}
		if (this.m_InvalidateUnits)
		{
			this.RefreshUnits();
			this.m_InvalidateUnits = false;
		}
		int num = this.logic.MaxUnits();
		int num2 = this.logic.MaxItems();
		if (this.m_CurrentMaxUnitSlots != num)
		{
			this.m_CurrentMaxUnitSlots = num;
			this.UpdateUnitFrames();
		}
		if (this.m_CcurrentMaxItemSlots != num2)
		{
			this.m_CcurrentMaxItemSlots = num2;
			this.UpdateEquipment();
		}
		this.UpdateActions();
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x000E6D9C File Offset: 0x000E4F9C
	public override void Release()
	{
		this.RemoveListeners();
		this.logicObject = null;
		this.logic = null;
		this.unitTransferTarget = null;
		this.m_ArmyTransferWindow = null;
		if (this.m_ArmyMorale != null)
		{
			this.m_ArmyMorale.Clear();
		}
		if (this.m_CurrentHireWindow != null)
		{
			this.m_CurrentHireWindow.Close(false);
		}
		base.Release();
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x000E6E04 File Offset: 0x000E5004
	private void RefreshUnits()
	{
		this.CreateUnitFrames();
		this.UpdateUnitFrames();
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x000E6E14 File Offset: 0x000E5014
	private void UpdateFocusedUnit()
	{
		int count = this.m_UnitSlots.Count;
		bool flag = this.m_CurrentHireWindow != null;
		for (int i = 0; i < count; i++)
		{
			UIUnitSlot uiunitSlot = this.m_UnitSlots[i];
			if (flag && uiunitSlot.IsEmpty())
			{
				uiunitSlot.SetFocused(true);
				flag = false;
			}
			else
			{
				uiunitSlot.SetFocused(false);
			}
		}
		if (this.m_CurrentHireWindow != null && !this.HasFreeSlots())
		{
			UICastleWindow componentInParent = base.GetComponentInParent<UICastleWindow>();
			if (componentInParent != null)
			{
				UICastleGarisson garrisonWindow = componentInParent.GarrisonWindow;
				if (garrisonWindow != null && garrisonWindow.HasFreeSlots())
				{
					componentInParent.GarrisonWindow.OpenRecruitWindow(-1, false);
				}
			}
		}
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x000E6EBC File Offset: 0x000E50BC
	private void UpdateFocusedInvetoryItems()
	{
		if (this.m_EquipmentSlots == null)
		{
			return;
		}
		if (this.m_EquipmentSlots.Count == 0)
		{
			return;
		}
		bool flag = this.m_CurrentPurchaseWindow != null;
		int count = this.m_EquipmentSlots.Count;
		Logic.Army logic = this.logic;
		if (logic != null)
		{
			List<InventoryItem> siege_equipment = logic.siege_equipment;
		}
		for (int i = 0; i < count; i++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.m_EquipmentSlots[i];
			if (flag && uiarmyItemIcon.IsEmpty() && !uiarmyItemIcon.Locked)
			{
				uiarmyItemIcon.SetFocused(true);
				flag = false;
			}
			else
			{
				uiarmyItemIcon.SetFocused(false);
			}
		}
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x000E6F49 File Offset: 0x000E5149
	public bool HasFreeSlots()
	{
		return this.logic != null && this.logic.MaxUnits() > this.logic.CountUnits();
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x000E6F6D File Offset: 0x000E516D
	private void HandleOnOpenHire(BSGButton b)
	{
		this.OpenRecruitWindow(-1, false);
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x000E6F77 File Offset: 0x000E5177
	private void HandleOnCloseHire(BSGButton b)
	{
		UIUnitRecruitmentWindow.CloseInstance();
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x000E6F80 File Offset: 0x000E5180
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2247867164U)
		{
			if (num <= 1211309691U)
			{
				if (num == 586732532U)
				{
					message == "skills_changed";
					return;
				}
				if (num != 1211309691U)
				{
					return;
				}
				if (!(message == "destroying"))
				{
					return;
				}
			}
			else if (num != 1649643086U)
			{
				if (num != 2247867164U)
				{
					return;
				}
				if (!(message == "statuses_changed"))
				{
					return;
				}
				this.RefreshStatuses();
				return;
			}
			else if (!(message == "finishing"))
			{
				return;
			}
			if (this.logic != null && this.logic.castle != null)
			{
				this.logic.castle.DelListener(this);
			}
			this.logic = null;
			return;
		}
		if (num <= 2316278087U)
		{
			if (num != 2266749524U)
			{
				if (num != 2316278087U)
				{
					return;
				}
				if (!(message == "units_changed"))
				{
					return;
				}
				this.m_InvalidateUnits = true;
				return;
			}
			else
			{
				if (!(message == "inventory_changed"))
				{
					return;
				}
				this.UpdateEquipment();
				this.UpdateFocusedInvetoryItems();
				return;
			}
		}
		else if (num != 2790575441U)
		{
			if (num != 3237025842U)
			{
				if (num != 3993615455U)
				{
					return;
				}
				if (!(message == "leader_changed"))
				{
					return;
				}
				this.Refresh();
				return;
			}
			else
			{
				if (!(message == "enter_castle"))
				{
					return;
				}
				if (this.logic != null && this.logic.castle != null)
				{
					this.logic.castle.AddListener(this);
				}
				UICharacterIcon uicharacterIcon = this.marshalicon;
				if (uicharacterIcon == null)
				{
					return;
				}
				uicharacterIcon.ShowCrest(false);
				return;
			}
		}
		else
		{
			if (!(message == "leave_castle"))
			{
				return;
			}
			if (this.logic != null && this.logic.castle != null)
			{
				this.logic.castle.DelListener(this);
			}
			UICharacterIcon uicharacterIcon2 = this.marshalicon;
			if (uicharacterIcon2 == null)
			{
				return;
			}
			uicharacterIcon2.ShowCrest(true);
			return;
		}
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x000E7141 File Offset: 0x000E5341
	public bool IsRecruitmentWindowActive()
	{
		return this.m_CurrentHireWindow != null && this.m_CurrentHireWindow.isActiveAndEnabled;
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x000E715E File Offset: 0x000E535E
	public bool IsEqupmentWindowActive()
	{
		return this.m_CurrentPurchaseWindow != null && this.m_CurrentPurchaseWindow.isActiveAndEnabled;
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x000E717C File Offset: 0x000E537C
	protected void OnDisable()
	{
		if (this.m_CurrentHireWindow != null)
		{
			UIUnitRecruitmentWindow currentHireWindow = this.m_CurrentHireWindow;
			currentHireWindow.on_close = (UIWindow.OnClose)Delegate.Remove(currentHireWindow.on_close, new UIWindow.OnClose(this.OnHireWindowClose));
			this.m_CurrentHireWindow.Close(false);
			this.m_CurrentHireWindow = null;
		}
		if (this.m_CurrentPurchaseWindow != null)
		{
			this.m_CurrentPurchaseWindow.Close(false);
			this.m_CurrentPurchaseWindow = null;
		}
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x000E71F2 File Offset: 0x000E53F2
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.m_CurrentHireWindow != null)
		{
			this.m_CurrentHireWindow.Close(false);
		}
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x000E7214 File Offset: 0x000E5414
	public void RefreshHireWindow()
	{
		if (this.m_CurrentHireWindow != null)
		{
			this.m_CurrentHireWindow.Refresh();
		}
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x000E722F File Offset: 0x000E542F
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06001777 RID: 6007 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x000E5B7B File Offset: 0x000E3D7B
	public void OnPoolDeactivated()
	{
		this.Release();
		this.OnDestroy();
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x000E7237 File Offset: 0x000E5437
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "recruitment_window_is_open")
		{
			return UIUnitRecruitmentWindow.IsActive();
		}
		if (this.logic != null)
		{
			return this.logic.GetVar(key, vars, as_value);
		}
		return Value.Unknown;
	}

	// Token: 0x04000EF8 RID: 3832
	[UIFieldTarget("id_IconMarshal")]
	private UICharacterIcon marshalicon;

	// Token: 0x04000EF9 RID: 3833
	[UIFieldTarget("id_KingdomCrest")]
	private UIKingdomIcon m_KingdomCrest;

	// Token: 0x04000EFA RID: 3834
	[UIFieldTarget("id_GorupUnits")]
	private RectTransform unitSlotsContainer;

	// Token: 0x04000EFB RID: 3835
	[UIFieldTarget("id_GroupEquipment")]
	private RectTransform equipmentSlotsContainer;

	// Token: 0x04000EFC RID: 3836
	[UIFieldTarget("id_ArmyMorale")]
	private UIArmyMorale m_ArmyMorale;

	// Token: 0x04000EFD RID: 3837
	[UIFieldTarget("id_ArmyFoodContaner")]
	private RectTransform m_ArmyFoodContainer;

	// Token: 0x04000EFE RID: 3838
	[UIFieldTarget("id_Resupply")]
	private BSGButton m_Resupply;

	// Token: 0x04000EFF RID: 3839
	[UIFieldTarget("id_ResupplyIcon")]
	private Image m_ResupplyIcon;

	// Token: 0x04000F00 RID: 3840
	[UIFieldTarget("id_Heal")]
	private GameObject m_Heal;

	// Token: 0x04000F01 RID: 3841
	[UIFieldTarget("id_ArmyActions")]
	private RectTransform m_ArmyActions;

	// Token: 0x04000F02 RID: 3842
	[UIFieldTarget("id_ArmyStatus")]
	private UIStatusIcon[] m_Statuses;

	// Token: 0x04000F03 RID: 3843
	[UIFieldTarget("id_Manpower")]
	private UIArmyManpower m_Manpower;

	// Token: 0x04000F04 RID: 3844
	[UIFieldTarget("id_Health")]
	private Image m_Health;

	// Token: 0x04000F05 RID: 3845
	[UIFieldTarget("id_HealthBackground")]
	private GameObject m_HealthBackground;

	// Token: 0x04000F06 RID: 3846
	[UIFieldTarget("id_ProvinceIllustration")]
	private UIProvinceIllustration m_ProvinceIllustration;

	// Token: 0x04000F09 RID: 3849
	private GameObject UnitFramePrefab;

	// Token: 0x04000F0A RID: 3850
	private GameObject EquipmentFramePrefab;

	// Token: 0x04000F0B RID: 3851
	private global::Army armyVisuals;

	// Token: 0x04000F0C RID: 3852
	private List<UIUnitSlot> m_UnitSlots;

	// Token: 0x04000F0D RID: 3853
	private List<UIArmyItemIcon> m_EquipmentSlots;

	// Token: 0x04000F0E RID: 3854
	private const int MAX_INVETORY_SLOTS = 4;

	// Token: 0x04000F0F RID: 3855
	private const int MAX_UNIT_SLOTS = 6;

	// Token: 0x04000F10 RID: 3856
	private UIUnitRecruitmentWindow m_CurrentHireWindow;

	// Token: 0x04000F11 RID: 3857
	private UIEquipmentPurchaseWindow m_CurrentPurchaseWindow;

	// Token: 0x04000F12 RID: 3858
	private UIArmyTransferWindow m_ArmyTransferWindow;

	// Token: 0x04000F13 RID: 3859
	private UIArmyFood m_ArmyFood;

	// Token: 0x04000F14 RID: 3860
	private int m_CurrentMaxUnitSlots;

	// Token: 0x04000F15 RID: 3861
	private int m_CcurrentMaxItemSlots;

	// Token: 0x04000F16 RID: 3862
	private int m_CurSupplies;

	// Token: 0x04000F17 RID: 3863
	private bool m_Initialzied;

	// Token: 0x04000F18 RID: 3864
	private bool m_InvalidateUnits;

	// Token: 0x04000F19 RID: 3865
	private UIActionIcon m_HealActionIcon;
}
