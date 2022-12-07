using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002A8 RID: 680
public class UICastleBuildings : MonoBehaviour, IListener
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06002A92 RID: 10898 RVA: 0x00169649 File Offset: 0x00167849
	// (set) Token: 0x06002A93 RID: 10899 RVA: 0x00169651 File Offset: 0x00167851
	public Castle Castle { get; private set; }

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06002A94 RID: 10900 RVA: 0x0016965A File Offset: 0x0016785A
	// (set) Token: 0x06002A95 RID: 10901 RVA: 0x00169662 File Offset: 0x00167862
	public int SelectedSlotIndex { get; private set; } = -1;

	// Token: 0x06002A96 RID: 10902 RVA: 0x0016966B File Offset: 0x0016786B
	public static UICastleBuildings Get()
	{
		return UICastleBuildings.m_Instance;
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x00169672 File Offset: 0x00167872
	public static int GetSelectedSlotIndex()
	{
		UICastleBuildings instance = UICastleBuildings.m_Instance;
		if (instance == null)
		{
			return -1;
		}
		return instance.SelectedSlotIndex;
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x00169684 File Offset: 0x00167884
	public void SetObject(Castle castle)
	{
		UICastleBuildings.m_Instance = this;
		this.Init();
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
		this.Castle = castle;
		if (this.Castle != null)
		{
			this.Castle.AddListener(this);
		}
		this.Refresh();
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x001696D4 File Offset: 0x001678D4
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_BuildingSlotHostPrototype != null)
		{
			this.m_BuildingSlotHostPrototype.gameObject.SetActive(false);
		}
		if (this.m_BuildingsContainer != null)
		{
			int childCount = this.m_BuildingsContainer.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UICastleBuildings.BuildSlotHost orAddComponent = this.m_BuildingsContainer.GetChild(i).GetOrAddComponent<UICastleBuildings.BuildSlotHost>();
				orAddComponent.SetObject(null, null, null, null);
				UIBuildingSlot hostedSlot = orAddComponent.HostedSlot;
				if (hostedSlot != null)
				{
					hostedSlot.OnSelected += this.HandleOpenBuildWindowRequest;
					hostedSlot.SetSlotIdx(-1);
					hostedSlot.AllowNamePlates = false;
				}
				this.building_slots.Add(orAddComponent);
			}
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x00169795 File Offset: 0x00167995
	private void Refresh()
	{
		this.PopulateBuildings();
		this.RefreshFirstFreeTutorialHighlightSlot();
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x001697A4 File Offset: 0x001679A4
	private void RefreshFirstFreeTutorialHighlightSlot()
	{
		bool flag = false;
		for (int i = 0; i < this.building_slots.Count; i++)
		{
			UICastleBuildings.BuildSlotHost buildSlotHost = this.building_slots[i];
			bool flag2 = !flag && buildSlotHost.HostedSlot.state == UIBuildingSlot.State.Empty;
			buildSlotHost.EnableTutorialHighlight(flag2);
			if (flag2)
			{
				flag = true;
			}
		}
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x001697F7 File Offset: 0x001679F7
	public UIBuildingSlot GetSelectedSlot()
	{
		if (this.SelectedSlotIndex < 0)
		{
			return null;
		}
		if (this.building_slots.Count <= this.SelectedSlotIndex)
		{
			return null;
		}
		return this.building_slots[this.SelectedSlotIndex].HostedSlot;
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x00169830 File Offset: 0x00167A30
	public void DeselectAll()
	{
		this.SelectedSlotIndex = -1;
		if (this.building_slots == null)
		{
			return;
		}
		for (int i = 0; i < this.building_slots.Count; i++)
		{
			UICastleBuildings.BuildSlotHost buildSlotHost = this.building_slots[i];
			if (buildSlotHost != null)
			{
				UIBuildingSlot hostedSlot = buildSlotHost.HostedSlot;
				if (hostedSlot != null)
				{
					hostedSlot.Select(false);
				}
			}
		}
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x00169888 File Offset: 0x00167A88
	public void Select(int slot_index, bool or_first_empty = true)
	{
		UIBuildingSlot selectedSlot = this.GetSelectedSlot();
		if (selectedSlot != null)
		{
			selectedSlot.Select(false);
		}
		if (slot_index >= this.Castle.AvailableBuildingSlots())
		{
			slot_index = -1;
		}
		if (or_first_empty && slot_index >= 0)
		{
			List<Building> buildings = this.Castle.buildings;
			if (this.Castle.GetBuilding(slot_index) != null)
			{
				bool flag = false;
				for (int i = 0; i < buildings.Count; i++)
				{
					if (buildings[i] == null)
					{
						slot_index = i;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					slot_index = buildings.Count;
				}
			}
		}
		if (or_first_empty && slot_index == -1)
		{
			slot_index = this.FindEmptyIdx(this.Castle.buildings);
		}
		this.SelectedSlotIndex = slot_index;
		UIBuildingSlot selectedSlot2 = this.GetSelectedSlot();
		if (selectedSlot2 == null)
		{
			return;
		}
		selectedSlot2.Select(true);
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x0016993C File Offset: 0x00167B3C
	public int FindEmptyIdx(List<Building> buildings)
	{
		if (buildings == null)
		{
			return 0;
		}
		Castle castle = this.Castle;
		bool flag;
		if (castle == null)
		{
			flag = false;
		}
		else
		{
			Building.Def currentBuildingBuild = castle.GetCurrentBuildingBuild();
			bool? flag2 = (currentBuildingBuild != null) ? new bool?(currentBuildingBuild.IsUpgrade()) : null;
			bool flag3 = false;
			flag = (flag2.GetValueOrDefault() == flag3 & flag2 != null);
		}
		bool flag4 = flag;
		for (int i = 0; i < buildings.Count; i++)
		{
			if ((!flag4 || this.Castle.structure_build.prefered_slot_index != i) && buildings[i] == null)
			{
				return i;
			}
		}
		if (buildings.Count >= this.Castle.AvailableBuildingSlots())
		{
			return -1;
		}
		return buildings.Count;
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x001699E4 File Offset: 0x00167BE4
	public void Select(UIBuildingSlot slot)
	{
		if (slot == null)
		{
			this.Select(-1, true);
			return;
		}
		int slotIndex = slot.SlotIndex;
		this.Select(slotIndex, true);
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x00169A14 File Offset: 0x00167C14
	private void PopulateBuildings()
	{
		if (this.m_BuildingsContainer == null)
		{
			return;
		}
		GameLogic.Get(true);
		int num = this.Castle.MaxBuildingSlots();
		int num2 = this.Castle.AvailableBuildingSlots();
		List<Building> buildings = this.Castle.buildings;
		if (this.building_slots.Count < num)
		{
			int num3 = num - this.building_slots.Count;
			int num4 = 0;
			while (num4 < num3 && !(this.m_BuildingSlotHostPrototype == null))
			{
				UICastleBuildings.BuildSlotHost orAddComponent = global::Common.Spawn(this.m_BuildingSlotHostPrototype, this.m_BuildingsContainer, false, "").GetOrAddComponent<UICastleBuildings.BuildSlotHost>();
				orAddComponent.SetObject(null, null, null, null);
				orAddComponent.HostedSlot.AllowNamePlates = false;
				orAddComponent.HostedSlot.OnSelected += this.HandleOpenBuildWindowRequest;
				this.building_slots.Add(orAddComponent);
				num4++;
			}
		}
		int num5 = -1;
		Castle.Build structure_build = this.Castle.structure_build;
		if (((structure_build != null) ? structure_build.current_building_def : null) != null)
		{
			num5 = this.Castle.structure_build.prefered_slot_index;
		}
		for (int i = 0; i < this.building_slots.Count; i++)
		{
			UICastleBuildings.BuildSlotHost buildSlotHost = this.building_slots[i];
			buildSlotHost.HostedSlot.SlotIndex = i;
			if (i >= num)
			{
				buildSlotHost.gameObject.SetActive(false);
			}
			else
			{
				buildSlotHost.gameObject.SetActive(true);
				if (buildings != null && i < buildings.Count)
				{
					Building building = buildings[i];
					if (building != null)
					{
						buildSlotHost.SetObject(null, this.Castle, building.def, null);
					}
					else if (num5 != -1 && num5 == i)
					{
						buildSlotHost.SetObject(null, this.Castle, this.Castle.structure_build.current_building_def, null);
					}
					else
					{
						buildSlotHost.SetObject(null, this.Castle, null, null);
					}
				}
				else if (num5 != -1 && num5 == i)
				{
					buildSlotHost.SetObject(null, this.Castle, this.Castle.structure_build.current_building_def, null);
				}
				else
				{
					buildSlotHost.SetObject(null, this.Castle, null, null);
				}
				buildSlotHost.HostedSlot.SetLocked(i >= num2 && buildSlotHost.HostedSlot.Def == null && buildSlotHost.HostedSlot.Inst == null);
			}
		}
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x00169C5C File Offset: 0x00167E5C
	private void HandleOpenBuildWindowRequest(UIBuildingSlot slot, PointerEventData e)
	{
		if (this.Castle.GetKingdom() != BaseUI.LogicKingdom() && !Game.CheckCheatLevel(Game.CheatLevel.High, "cheat buildings window", false))
		{
			return;
		}
		if (slot.state == UIBuildingSlot.State.Locked)
		{
			if (UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				this.UnlockNoRequest();
				return;
			}
			this.HandleUnlockRequest(null);
			return;
		}
		else if (this.Castle.HasBuilding(slot.Def) || this.Castle.IsBuilding(slot.Def) || this.Castle.HasPlannedBuilding(slot.Def))
		{
			if (UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				UIBuildingWindow.CloseInstance();
				UICastleBuildWindow.Create(null, this.Castle, this.SelectedSlotIndex, slot.Def);
				return;
			}
			UICastleBuildWindow.CloseInstance();
			bool flag;
			UIBuildingWindow uibuildingWindow = UIBuildingWindow.Create(this.Castle, null, slot.Def, this.Castle, out flag);
			if (uibuildingWindow != null && this.m_BuildingWindowPosition != null && flag)
			{
				Vector3 position = this.m_BuildingWindowPosition.transform.position;
				Vector2 v;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(uibuildingWindow.transform.parent as RectTransform, position, null, out v);
				uibuildingWindow.transform.localPosition = v;
				UICommon.EnsureInScreen(uibuildingWindow.gameObject, 0.85f);
			}
			return;
		}
		else
		{
			if (slot == null || !slot.state_def.GetBool("allow_open_build_window", null, false, true, true, true, '.'))
			{
				return;
			}
			UIBuildingWindow.CloseInstance();
			this.Select(slot);
			UICastleBuildWindow.Create(null, this.Castle, this.SelectedSlotIndex, slot.Def);
			return;
		}
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x00169DE4 File Offset: 0x00167FE4
	private void HandleUnlockRequest(BSGButton b)
	{
		if (this.Castle == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Castle.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Resource expandCost = this.Castle.GetExpandCost();
		Logic.Realm realm = this.Castle.GetRealm();
		if (!kingdom.resources.CanAfford(expandCost, 1f, Array.Empty<ResourceType>()) || realm.IsOccupied() || realm.IsDisorder())
		{
			return;
		}
		Vars vars = new Vars(this.Castle);
		MessageWnd.Create("ExpandCityMessage", vars, null, new MessageWnd.OnButton(this.Expand));
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x00169E78 File Offset: 0x00168078
	private void UnlockNoRequest()
	{
		if (this.Castle == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Castle.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Resource expandCost = this.Castle.GetExpandCost();
		Logic.Realm realm = this.Castle.GetRealm();
		if (!kingdom.resources.CanAfford(expandCost, 1f, Array.Empty<ResourceType>()) || realm.IsOccupied() || realm.IsDisorder())
		{
			return;
		}
		this.Castle.ExpandCity();
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x00169EEB File Offset: 0x001680EB
	private bool Expand(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.Castle.ExpandCity();
		}
		wnd.Close(false);
		return true;
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x00169F10 File Offset: 0x00168110
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "build_window_select")
		{
			return;
		}
		if (message == "structures_changed")
		{
			this.Refresh();
			return;
		}
		if (message == "tier_changed")
		{
			this.Refresh();
			return;
		}
		if (message == "build_started" || message == "build_canceled" || message == "build_finished")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x00169F81 File Offset: 0x00168181
	private void OnDestroy()
	{
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x00169F97 File Offset: 0x00168197
	public bool GetStoreState()
	{
		return UICastleBuildWindow.IsVisible();
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x00169F9E File Offset: 0x0016819E
	private void OnDisable()
	{
		UICastleBuildings.m_Instance = null;
	}

	// Token: 0x04001CDB RID: 7387
	[UIFieldTarget("id_BuildingSlots")]
	private RectTransform m_BuildingsContainer;

	// Token: 0x04001CDC RID: 7388
	[UIFieldTarget("id_UnlockSlotsContainer")]
	private RectTransform m_UnlockSlotsContainer;

	// Token: 0x04001CDD RID: 7389
	[UIFieldTarget("id_BuildingWindowPosition")]
	private RectTransform m_BuildingWindowPosition;

	// Token: 0x04001CDE RID: 7390
	[UIFieldTarget("id_BuildingSlotHostPrototype")]
	private GameObject m_BuildingSlotHostPrototype;

	// Token: 0x04001CE1 RID: 7393
	private List<UICastleBuildings.BuildSlotHost> building_slots = new List<UICastleBuildings.BuildSlotHost>();

	// Token: 0x04001CE2 RID: 7394
	private bool m_Initialzied;

	// Token: 0x04001CE3 RID: 7395
	private static UICastleBuildings m_Instance;

	// Token: 0x02000806 RID: 2054
	internal class BuildSlotHost : MonoBehaviour
	{
		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06004F4E RID: 20302 RVA: 0x00234EB7 File Offset: 0x002330B7
		public UIBuildingSlot HostedSlot
		{
			get
			{
				return this.m_BuildingSlot;
			}
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x00234EBF File Offset: 0x002330BF
		public void SetObject(Logic.Kingdom kingdom, Castle castle, Building.Def def, District.Def district)
		{
			this.Init();
			this.m_BuildingSlot.SetDef(kingdom, castle, def, district);
			this.m_Castle = castle;
			this.Refresh();
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x00234EE4 File Offset: 0x002330E4
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_TutorialFirstFreeSlot != null)
			{
				this.m_TutorialFirstFreeSlot.gameObject.SetActive(false);
			}
			this.m_Initialzied = true;
		}

		// Token: 0x06004F51 RID: 20305 RVA: 0x00234F1C File Offset: 0x0023311C
		private void Refresh()
		{
			if (this.m_BuildingSlot == null)
			{
				return;
			}
			if (this.m_Overlay == null)
			{
				return;
			}
			bool active = true;
			DT.Field slot_def = this.m_BuildingSlot.slot_def;
			bool selected = this.m_BuildingSlot.Selected;
			Castle castle = this.m_Castle;
			Logic.Kingdom kingdom;
			if (castle == null)
			{
				kingdom = null;
			}
			else
			{
				Logic.Realm realm = castle.GetRealm();
				if (realm == null)
				{
					kingdom = null;
				}
				else
				{
					Logic.Object controller = realm.controller;
					kingdom = ((controller != null) ? controller.GetKingdom() : null);
				}
			}
			if (kingdom != BaseUI.LogicKingdom() && (this.m_BuildingSlot.state == UIBuildingSlot.State.Locked || this.m_BuildingSlot.state == UIBuildingSlot.State.Empty))
			{
				this.m_Overlay.overrideSprite = global::Defs.GetObj<Sprite>(slot_def, "casle_slot_foreign." + (this.m_BuildingSlot.mouse_in ? "over" : (selected ? "over" : "normal")), null);
			}
			else if (this.m_BuildingSlot.state == UIBuildingSlot.State.Locked)
			{
				this.m_Overlay.overrideSprite = global::Defs.GetObj<Sprite>(slot_def, "casle_slot_unlock." + (this.m_BuildingSlot.mouse_in ? "over" : (selected ? "over" : "normal")), null);
			}
			else if (this.m_BuildingSlot.state == UIBuildingSlot.State.Empty)
			{
				this.m_Overlay.overrideSprite = global::Defs.GetObj<Sprite>(slot_def, "casle_slot_empty." + (this.m_BuildingSlot.mouse_in ? "over" : (selected ? "over" : "normal")), null);
			}
			else
			{
				active = false;
			}
			this.m_Overlay.gameObject.SetActive(active);
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x002350A7 File Offset: 0x002332A7
		public void EnableTutorialHighlight(bool enable)
		{
			if (this.m_TutorialFirstFreeSlot != null)
			{
				this.m_TutorialFirstFreeSlot.SetActive(enable);
			}
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x002350C3 File Offset: 0x002332C3
		private void LateUpdate()
		{
			this.Refresh();
		}

		// Token: 0x04003D6B RID: 15723
		[UIFieldTarget("id_BuildingSlot")]
		private UIBuildingSlot m_BuildingSlot;

		// Token: 0x04003D6C RID: 15724
		[UIFieldTarget("id_Overlay")]
		private Image m_Overlay;

		// Token: 0x04003D6D RID: 15725
		[UIFieldTarget("tut_FirstFreeSlot")]
		private GameObject m_TutorialFirstFreeSlot;

		// Token: 0x04003D6E RID: 15726
		private Castle m_Castle;

		// Token: 0x04003D6F RID: 15727
		private bool m_Initialzied;
	}
}
