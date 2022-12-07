using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001D4 RID: 468
public class UIBattleViewSiegeEquipmentContainer : MonoBehaviour
{
	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x00107999 File Offset: 0x00105B99
	// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x001079A1 File Offset: 0x00105BA1
	public Logic.Battle Logic { get; private set; }

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06001BA5 RID: 7077 RVA: 0x001079AA File Offset: 0x00105BAA
	// (set) Token: 0x06001BA6 RID: 7078 RVA: 0x001079B2 File Offset: 0x00105BB2
	public int BattleSide { get; private set; }

	// Token: 0x06001BA7 RID: 7079 RVA: 0x001079BB File Offset: 0x00105BBB
	public void SetObject(List<InventoryItem> armyItems)
	{
		this.Initialize();
		if (armyItems == null || armyItems.Count == 0)
		{
			return;
		}
		this.m_siegeEquipment = armyItems;
		this.UpdateSiegeEquipment();
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x001079DC File Offset: 0x00105BDC
	public void UpdateSelection()
	{
		for (int i = this.m_siegeEquipmentSlots.Count - 1; i >= 0; i--)
		{
			if (!(this.m_siegeEquipmentSlots[i] == null) && this.m_siegeEquipmentSlots[i].IsSelectable)
			{
				this.m_siegeEquipmentSlots[i].UpdateSelection();
			}
		}
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x00107A3C File Offset: 0x00105C3C
	public void Clear()
	{
		this.m_siegeEquipmentSlots.Clear();
		this.m_siegeEquipment.Clear();
		this.m_emptySlots.Clear();
		this.m_logicToUI.Clear();
		if (this != null && base.gameObject != null && base.transform != null)
		{
			global::Common.DestroyChildren(base.transform);
		}
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x00107AA5 File Offset: 0x00105CA5
	private void Initialize()
	{
		if (this.m_definition == null)
		{
			this.m_definition = global::Defs.GetDefField("UIBattleViewSiegeEquipmentContainer", null);
		}
		this.InitializeSlots();
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x00107AC8 File Offset: 0x00105CC8
	private void InitializeSlots()
	{
		if (this.m_siegeEquipmentSlots.Count == 0)
		{
			UICommon.DeleteChildren(base.transform);
			this.m_emptySlots = new Queue<UIBattleViewSiegeEquipment>(4);
			for (int i = 0; i < 4; i++)
			{
				UIBattleViewSiegeEquipment uibattleViewSiegeEquipment = this.CreateSlot();
				if (uibattleViewSiegeEquipment != null)
				{
					this.m_emptySlots.Enqueue(uibattleViewSiegeEquipment);
				}
			}
		}
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x00107B24 File Offset: 0x00105D24
	private UIBattleViewSiegeEquipment CreateSlot()
	{
		GameObject obj = global::Defs.GetObj<GameObject>(this.m_definition, "siege_eqipment_prefab", null);
		if (obj == null)
		{
			return null;
		}
		UIBattleViewSiegeEquipment component = global::Common.Spawn(obj, base.transform, false, "").GetComponent<UIBattleViewSiegeEquipment>();
		if (component == null)
		{
			return null;
		}
		component.SetInventoryItem(null);
		this.m_siegeEquipmentSlots.Add(component);
		return component;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x00107B88 File Offset: 0x00105D88
	private void UpdateSiegeEquipment()
	{
		for (int i = 0; i < this.m_siegeEquipment.Count; i++)
		{
			if ((!this.m_siegeEquipment[i].def.is_siege_eq || this.m_siegeEquipment[i].simulation != null) && !this.m_logicToUI.ContainsKey(this.m_siegeEquipment[i]))
			{
				this.UpdateEmptySlot(this.m_siegeEquipment[i]);
			}
		}
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x00107C04 File Offset: 0x00105E04
	private void UpdateEmptySlot(InventoryItem inventoryItem)
	{
		UIBattleViewSiegeEquipment uibattleViewSiegeEquipment;
		if (this.m_emptySlots.Count == 0)
		{
			uibattleViewSiegeEquipment = this.CreateSlot();
		}
		else
		{
			uibattleViewSiegeEquipment = this.m_emptySlots.Dequeue();
		}
		uibattleViewSiegeEquipment.SetInventoryItem(inventoryItem);
		uibattleViewSiegeEquipment.onClick += this.UIBattleViewSiegeEquipment_OnClick;
		uibattleViewSiegeEquipment.onRemoved += this.UIBattleViewSiegeEquipment_OnRemoved;
		this.m_logicToUI.Add(inventoryItem, uibattleViewSiegeEquipment);
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x00107C6C File Offset: 0x00105E6C
	private void UIBattleViewSiegeEquipment_OnClick(UIBattleViewSiegeEquipment s, PointerEventData e)
	{
		if (s == null || s.SquadLogic == null || s.Visuals == null)
		{
			return;
		}
		global::Squad visuals = s.Visuals;
		if (visuals != null)
		{
			BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
			battleViewUI.SelectObj(visuals.gameObject, false, true, true, true);
			if (e.clickCount > 1 && battleViewUI.selected_obj != null)
			{
				battleViewUI.LookAt(battleViewUI.selected_obj.transform.position, false);
			}
		}
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00107CED File Offset: 0x00105EED
	private void UIBattleViewSiegeEquipment_OnRemoved(UIBattleViewSiegeEquipment siegeEquipment)
	{
		this.RemoveSiegeEquipment(siegeEquipment);
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x00107CF8 File Offset: 0x00105EF8
	private void RemoveSiegeEquipment(UIBattleViewSiegeEquipment siegeEquipment)
	{
		if (this.m_siegeEquipmentSlots == null)
		{
			return;
		}
		if (siegeEquipment.logic != null)
		{
			this.m_logicToUI.Remove(siegeEquipment.logic);
		}
		if (siegeEquipment.transform.GetSiblingIndex() < 4)
		{
			siegeEquipment.SetInventoryItem(null);
			this.m_emptySlots.Enqueue(siegeEquipment);
			return;
		}
		siegeEquipment.onClick -= this.UIBattleViewSiegeEquipment_OnClick;
		siegeEquipment.onRemoved -= this.UIBattleViewSiegeEquipment_OnRemoved;
		this.m_siegeEquipmentSlots.Remove(siegeEquipment);
		global::Common.DestroyObj(siegeEquipment.gameObject);
	}

	// Token: 0x040011F9 RID: 4601
	private DT.Field m_definition;

	// Token: 0x040011FA RID: 4602
	private const int NUMBER_OF_SLOTS = 4;

	// Token: 0x040011FB RID: 4603
	private List<InventoryItem> m_siegeEquipment = new List<InventoryItem>();

	// Token: 0x040011FC RID: 4604
	private Dictionary<InventoryItem, UIBattleViewSiegeEquipment> m_logicToUI = new Dictionary<InventoryItem, UIBattleViewSiegeEquipment>();

	// Token: 0x040011FD RID: 4605
	private List<UIBattleViewSiegeEquipment> m_siegeEquipmentSlots = new List<UIBattleViewSiegeEquipment>();

	// Token: 0x040011FE RID: 4606
	private Queue<UIBattleViewSiegeEquipment> m_emptySlots;
}
