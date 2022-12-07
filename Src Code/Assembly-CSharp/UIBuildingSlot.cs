using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002A2 RID: 674
[ExecuteInEditMode]
public class UIBuildingSlot : Hotspot, IListener, IPoolable
{
	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060029AB RID: 10667 RVA: 0x00161B53 File Offset: 0x0015FD53
	// (set) Token: 0x060029AC RID: 10668 RVA: 0x00161B5B File Offset: 0x0015FD5B
	public DT.Field slot_def { get; private set; }

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060029AD RID: 10669 RVA: 0x00161B64 File Offset: 0x0015FD64
	// (set) Token: 0x060029AE RID: 10670 RVA: 0x00161B6C File Offset: 0x0015FD6C
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060029AF RID: 10671 RVA: 0x00161B75 File Offset: 0x0015FD75
	// (set) Token: 0x060029B0 RID: 10672 RVA: 0x00161B7D File Offset: 0x0015FD7D
	public Castle OrgCastle { get; private set; }

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060029B1 RID: 10673 RVA: 0x00161B86 File Offset: 0x0015FD86
	// (set) Token: 0x060029B2 RID: 10674 RVA: 0x00161B8E File Offset: 0x0015FD8E
	public Castle Castle { get; private set; }

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060029B3 RID: 10675 RVA: 0x00161B97 File Offset: 0x0015FD97
	// (set) Token: 0x060029B4 RID: 10676 RVA: 0x00161B9F File Offset: 0x0015FD9F
	public District.Def District { get; private set; }

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x060029B5 RID: 10677 RVA: 0x00161BA8 File Offset: 0x0015FDA8
	// (set) Token: 0x060029B6 RID: 10678 RVA: 0x00161BB0 File Offset: 0x0015FDB0
	public Building.Def Def { get; private set; }

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x060029B7 RID: 10679 RVA: 0x00161BB9 File Offset: 0x0015FDB9
	// (set) Token: 0x060029B8 RID: 10680 RVA: 0x00161BC1 File Offset: 0x0015FDC1
	public Building Inst { get; private set; }

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x060029B9 RID: 10681 RVA: 0x00161BCA File Offset: 0x0015FDCA
	public Game game
	{
		get
		{
			return GameLogic.Get(false);
		}
	}

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x060029BA RID: 10682 RVA: 0x00161BD4 File Offset: 0x0015FDD4
	// (remove) Token: 0x060029BB RID: 10683 RVA: 0x00161C0C File Offset: 0x0015FE0C
	public event Action<UIBuildingSlot, PointerEventData> OnGainFocus;

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x060029BC RID: 10684 RVA: 0x00161C44 File Offset: 0x0015FE44
	// (remove) Token: 0x060029BD RID: 10685 RVA: 0x00161C7C File Offset: 0x0015FE7C
	public event Action<UIBuildingSlot, PointerEventData> OnLooseFocus;

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x060029BE RID: 10686 RVA: 0x00161CB4 File Offset: 0x0015FEB4
	// (remove) Token: 0x060029BF RID: 10687 RVA: 0x00161CEC File Offset: 0x0015FEEC
	public event Action<UIBuildingSlot, PointerEventData> OnSelected;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x060029C0 RID: 10688 RVA: 0x00161D24 File Offset: 0x0015FF24
	// (remove) Token: 0x060029C1 RID: 10689 RVA: 0x00161D5C File Offset: 0x0015FF5C
	public event Action<UIBuildingSlot> OnRemove;

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x060029C2 RID: 10690 RVA: 0x00161D91 File Offset: 0x0015FF91
	public bool Selected
	{
		get
		{
			return this.m_Selected;
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x060029C3 RID: 10691 RVA: 0x00161D99 File Offset: 0x0015FF99
	// (set) Token: 0x060029C4 RID: 10692 RVA: 0x00161DA1 File Offset: 0x0015FFA1
	public bool AllowRemove
	{
		get
		{
			return this.m_AllowRemove;
		}
		set
		{
			if (value != this.m_AllowRemove)
			{
				this.m_AllowRemove = value;
				this.Refresh();
			}
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x060029C5 RID: 10693 RVA: 0x00161DB9 File Offset: 0x0015FFB9
	// (set) Token: 0x060029C6 RID: 10694 RVA: 0x00161DC1 File Offset: 0x0015FFC1
	public bool AllowNamePlates
	{
		get
		{
			return this.m_AllowNamePlates;
		}
		set
		{
			if (value != this.m_AllowNamePlates)
			{
				this.m_AllowNamePlates = value;
				this.Refresh();
			}
		}
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x060029C7 RID: 10695 RVA: 0x00161DD9 File Offset: 0x0015FFD9
	// (set) Token: 0x060029C8 RID: 10696 RVA: 0x00161DE1 File Offset: 0x0015FFE1
	public bool AllowUpgardeIcons
	{
		get
		{
			return this.m_AllowUpgardeIcons;
		}
		set
		{
			if (value != this.m_AllowUpgardeIcons)
			{
				this.m_AllowUpgardeIcons = value;
				this.Refresh();
			}
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x060029C9 RID: 10697 RVA: 0x00161DF9 File Offset: 0x0015FFF9
	// (set) Token: 0x060029CA RID: 10698 RVA: 0x00161E01 File Offset: 0x00160001
	public bool ShowDistrictProgress
	{
		get
		{
			return this.m_ShowDistrictProgress;
		}
		set
		{
			if (value != this.m_ShowDistrictProgress)
			{
				this.m_ShowDistrictProgress = value;
				this.Refresh();
			}
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x060029CB RID: 10699 RVA: 0x00161E19 File Offset: 0x00160019
	// (set) Token: 0x060029CC RID: 10700 RVA: 0x00161E21 File Offset: 0x00160021
	public UIBuildingSlot.State state { get; private set; }

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x060029CD RID: 10701 RVA: 0x00161E2A File Offset: 0x0016002A
	// (set) Token: 0x060029CE RID: 10702 RVA: 0x00161E32 File Offset: 0x00160032
	public DT.Field state_def { get; private set; }

	// Token: 0x060029CF RID: 10703 RVA: 0x00161E3C File Offset: 0x0016003C
	public void SetDef(Logic.Kingdom kingdom, Castle castle, Building.Def def, District.Def district)
	{
		this.SetKingdom(kingdom ?? ((castle != null) ? castle.GetKingdom() : null));
		this.OrgCastle = castle;
		this.SetCastle(castle, false);
		this.Def = def;
		this.District = district;
		this.UpdateInst();
		this.Init();
		this.Refresh();
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x00161E90 File Offset: 0x00160090
	private void SetKingdom(Logic.Kingdom kingdom)
	{
		if (kingdom == this.Kingdom)
		{
			return;
		}
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = kingdom;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
		}
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x00161ECC File Offset: 0x001600CC
	private void SetCastle(Castle castle, bool update_tt_vars = false)
	{
		if (castle == this.Castle)
		{
			return;
		}
		if (this.Castle != null)
		{
			this.Castle.DelListener(this);
		}
		this.Castle = castle;
		if (this.Castle != null)
		{
			this.Castle.AddListener(this);
		}
		if (update_tt_vars)
		{
			this.BuildTooltip();
		}
	}

	// Token: 0x060029D2 RID: 10706 RVA: 0x00161F1C File Offset: 0x0016011C
	public void SetLocked(bool locked)
	{
		this.Init();
		this.m_Locked = locked;
		this.Refresh();
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x00161F34 File Offset: 0x00160134
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		this.m_Initialzed = true;
		UICommon.FindComponents(this, false);
		if (this.m_CancelBuild != null)
		{
			this.m_CancelBuild.onClick = new BSGButton.OnClick(this.HandleCancelBuild);
		}
		if (this.Button_RemoveStructure != null)
		{
			this.Button_RemoveStructure.onClick = new BSGButton.OnClick(this.HandleRemoveButton);
		}
		if (this.m_BuildProgress != null)
		{
			this.m_BuildProgress.Setup(5, true);
		}
		if (this.m_RepairProgress != null)
		{
			this.m_RepairProgress.Setup(5, true);
		}
		if (this.m_HostedBuildingProgress != null)
		{
			this.m_HostedBuildingProgress.Setup(5, true);
		}
		if (this.m_BuildingSlotCountProtootype != null)
		{
			this.m_BuildingSlotCountProtootype.gameObject.SetActive(false);
		}
		if (this.m_DistrictMaxedBorder != null)
		{
			this.m_DistrictMaxedBorder.gameObject.SetActive(false);
		}
		if (this.m_UpgardeIconsContainer)
		{
			UICommon.DeleteChildren(this.m_UpgardeIconsContainer);
		}
		if (this.m_UpgardeDotsContainer)
		{
			UICommon.DeleteChildren(this.m_UpgardeDotsContainer);
		}
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x00162064 File Offset: 0x00160264
	public bool IsUnderConstruction()
	{
		if (this.Def == null)
		{
			return false;
		}
		if (this.Def.IsUpgrade())
		{
			return this.Kingdom != null && this.Kingdom.IsUpgrading(this.Def);
		}
		Castle castle = this.Castle;
		return ((castle != null) ? castle.structure_build : null) != null && this.Castle.structure_build.current_building_def == this.Def && (this.SlotIndex < 0 || this.SlotIndex == this.Castle.structure_build.prefered_slot_index);
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x001620F8 File Offset: 0x001602F8
	public bool IsUnderRepair()
	{
		if (this.Inst != null)
		{
			Castle castle = this.Castle;
			if (((castle != null) ? castle.structure_build : null) != null && this.Castle.structure_build.current_building_repair == this.Inst)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x00162134 File Offset: 0x00160334
	public UIBuildingSlot.State DecideState()
	{
		if (this.m_Locked)
		{
			return UIBuildingSlot.State.Locked;
		}
		if (this.m_CurRelationHighlight == "will_be_destroyed")
		{
			return UIBuildingSlot.State.WillBeDestroyed;
		}
		Castle castle = this.Castle;
		Logic.Realm realm = (castle != null) ? castle.GetRealm() : null;
		if (this.Def == null)
		{
			if (realm != null && realm.IsOccupied())
			{
				return UIBuildingSlot.State.EmptyOccupied;
			}
			return UIBuildingSlot.State.Empty;
		}
		else
		{
			if (this.Def.id == null)
			{
				return UIBuildingSlot.State.Invalid;
			}
			bool flag = this.Def.IsUpgrade();
			if (this.IsUnderConstruction())
			{
				if (!flag && realm != null && realm.IsOccupied())
				{
					return UIBuildingSlot.State.BuildingOccupied;
				}
				if (flag && this.Kingdom != null && this.Kingdom.IsUpgradingElsewhere(this.Def))
				{
					return UIBuildingSlot.State.UpgradingElsewhere;
				}
				return UIBuildingSlot.State.Building;
			}
			else
			{
				if (!flag && realm != null && realm.IsOccupied())
				{
					return UIBuildingSlot.State.StalledOccupied;
				}
				if (this.Inst != null)
				{
					if (this.Inst.state == Building.State.TemporaryDeactivated)
					{
						return UIBuildingSlot.State.Working;
					}
					UIBuildingSlot.State state;
					if (!Enum.TryParse<UIBuildingSlot.State>(this.Inst.state.ToString(), out state))
					{
						Debug.LogError(string.Format("Logic.Building.Def.State.{0} does not have corresponding value in UIBuildingSlot.State", this.Inst.state));
						return UIBuildingSlot.State.Invalid;
					}
					if (state == UIBuildingSlot.State.Planned && this.Castle.CanBuildBuilding(this.Def, false, false, false) == Castle.StructureBuildAvailability.Available)
					{
						return UIBuildingSlot.State.PlannedCanBuild;
					}
					if (state == UIBuildingSlot.State.Working && !flag)
					{
						Building.Def def = this.Def;
						bool flag2;
						if (def == null)
						{
							flag2 = (null != null);
						}
						else
						{
							District.Def upgrades = def.upgrades;
							flag2 = (((upgrades != null) ? upgrades.buildings : null) != null);
						}
						if (!flag2 || this.Def.upgrades.buildings.Count == 0 || this.Inst.CalcCompleted())
						{
							return UIBuildingSlot.State.WorkingComplated;
						}
					}
					return state;
				}
				else
				{
					if (flag)
					{
						if (this.Kingdom == null)
						{
							return UIBuildingSlot.State.Unbuildabe;
						}
						if (this.Kingdom.HasBuildingUpgrade(this.Def))
						{
							return UIBuildingSlot.State.Abandoned;
						}
						if (this.Kingdom.HasPlannedUpgrade(this.Def))
						{
							if (this.Castle == null)
							{
								return UIBuildingSlot.State.Planned;
							}
							if (this.Castle.CanBuildBuilding(this.Def, false, false, false) == Castle.StructureBuildAvailability.Available)
							{
								return UIBuildingSlot.State.PlannedCanBuild;
							}
							return UIBuildingSlot.State.Planned;
						}
					}
					if (this.Castle == null)
					{
						return UIBuildingSlot.State.Unbuildabe;
					}
					if (!this.Castle.MayBuildBuilding(this.Def, true))
					{
						return UIBuildingSlot.State.Unbuildabe;
					}
					Castle.StructureBuildAvailability structureBuildAvailability = this.Castle.CanBuildBuilding(this.Def, false, false, false);
					if (Castle.CheckAvailabilityFlags(structureBuildAvailability, Castle.StructureBuildAvailability.NoParentNoRequirements))
					{
						return UIBuildingSlot.State.CanNotBuildPrerequisitesGoods;
					}
					if (Castle.CheckAvailabilityFlags(structureBuildAvailability, Castle.StructureBuildAvailability.NoParent))
					{
						return UIBuildingSlot.State.CanNotBuildPrerequisites;
					}
					if (Castle.CheckAvailabilityFlags(structureBuildAvailability, Castle.StructureBuildAvailability.NoRequirements))
					{
						return UIBuildingSlot.State.CanNotBuild;
					}
					if (Castle.CheckAvailabilityFlags(structureBuildAvailability, Castle.StructureBuildAvailability.MaxCountReached))
					{
						return UIBuildingSlot.State.CanNotBuildSlots;
					}
					if (structureBuildAvailability == Castle.StructureBuildAvailability.CannotAfford)
					{
						return UIBuildingSlot.State.CanNotBuildCost;
					}
					if (structureBuildAvailability != Castle.StructureBuildAvailability.Available)
					{
						return UIBuildingSlot.State.CanNotBuild;
					}
					return UIBuildingSlot.State.CanBuild;
				}
			}
		}
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x0016239C File Offset: 0x0016059C
	private void UpdateInst()
	{
		Castle castle = this.OrgCastle;
		if (castle == null)
		{
			this.Inst = null;
		}
		else if (this.SlotIndex < 0)
		{
			this.Inst = castle.FindBuilding(this.Def);
		}
		else
		{
			this.Inst = castle.GetBuilding(this.SlotIndex);
		}
		if (this.Def != null && (castle == null || this.Def.IsUpgrade()) && this.Kingdom != null)
		{
			Building building = this.Kingdom.FindBuilding(this.Def);
			if (building != null && (this.Inst == null || building.state > this.Inst.state))
			{
				this.Inst = building;
				castle = building.castle;
			}
			if (building == null)
			{
				this.Inst = null;
				this.Kingdom.CalcBuildAvailability(this.Def, ref castle, null, false);
			}
		}
		this.SetCastle(castle, true);
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x00162474 File Offset: 0x00160674
	public void UpdateDefAndInst()
	{
		if (this.Def == null && this.SlotIndex >= 0)
		{
			Castle castle = this.Castle;
			if (((castle != null) ? castle.structure_build : null) != null)
			{
				int num = this.Castle.structure_build.prefered_slot_index;
				if (num < 0)
				{
					List<Building> buildings = this.Castle.GetBuildings(null);
					num = this.Castle.FindEmptyIdx(buildings);
				}
				if (num == this.SlotIndex)
				{
					this.Def = this.Castle.structure_build.current_building_def;
					this.Inst = null;
					return;
				}
			}
		}
		this.UpdateInst();
		if (this.Inst != null)
		{
			this.Def = this.Inst.def;
		}
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x0016251C File Offset: 0x0016071C
	public void UpdateState()
	{
		this.UpdateDefAndInst();
		UIBuildingSlot.State state = this.DecideState();
		this.SetState(state);
		this.UpdateIcon();
		this.UpdateDistrictColor();
		this.UpdateStatusIcon();
		this.UpdateBuildingsCount();
		this.UpdateInteractions();
		this.UpdateBuildProgress();
		this.UpdateRepairProgress();
		this.UpdateDefName();
		this.UpdateBuildingName();
		this.UpdateHostedBuildingProgress();
		this.UpdateBuildingCountPorgress();
		this.UpdateNamePlate();
		this.SetupTooltip();
		this.UpdateUpgradeSlots();
		this.UpdateReligionIncompatibility();
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x00162598 File Offset: 0x00160798
	public void SetState(UIBuildingSlot.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.slot_def == null)
		{
			this.slot_def = global::Defs.GetDefField(this.slot_type.ToString(), null);
		}
		if (this.slot_def != null)
		{
			this.state_def = this.slot_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.slot_def.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.state_def = null;
		}
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x00162654 File Offset: 0x00160854
	private void Update()
	{
		if (this.DecideState() != this.state)
		{
			this.Refresh();
		}
		if (this.m_isCurrentlyBuilding)
		{
			if (this.m_BuildProgress != null && this.IsUnderConstruction())
			{
				if (this.Def.IsUpgrade())
				{
					this.m_BuildProgress.SetProgress(this.Kingdom.GetUpgradeProgress(this.Def));
					return;
				}
				this.m_BuildProgress.SetProgress(this.Castle.structure_build.GetProgress());
				return;
			}
			else if (this.m_RepairProgress != null && this.IsUnderRepair())
			{
				this.m_RepairProgress.SetProgress(this.Castle.structure_build.GetProgress());
			}
		}
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x0016270D File Offset: 0x0016090D
	private void Refresh()
	{
		this.UpdateState();
		this.UpdateHighlight();
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x0016271B File Offset: 0x0016091B
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x00162734 File Offset: 0x00160934
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num <= 2596447321U)
		{
			if (num <= 954139509U)
			{
				if (num != 446289113U)
				{
					if (num != 954139509U)
					{
						goto IL_240;
					}
					if (!(key == "building"))
					{
						goto IL_240;
					}
				}
				else
				{
					if (!(key == "inst"))
					{
						goto IL_240;
					}
					return this.Inst;
				}
			}
			else if (num != 1378207623U)
			{
				if (num != 2065409846U)
				{
					if (num != 2596447321U)
					{
						goto IL_240;
					}
					if (!(key == "castle"))
					{
						goto IL_240;
					}
					return this.Castle;
				}
				else
				{
					if (!(key == "org_castle"))
					{
						goto IL_240;
					}
					return this.OrgCastle;
				}
			}
			else
			{
				if (!(key == "obj_type"))
				{
					goto IL_240;
				}
				return "Building";
			}
		}
		else if (num <= 3199239212U)
		{
			if (num != 2784473194U)
			{
				if (num != 3137648494U)
				{
					if (num != 3199239212U)
					{
						goto IL_240;
					}
					if (!(key == "def_id"))
					{
						goto IL_240;
					}
					Building.Def def = this.Def;
					return (def != null) ? def.id : null;
				}
				else
				{
					if (!(key == "buildings_count"))
					{
						goto IL_240;
					}
					return this.GetBuildingsCountValue();
				}
			}
			else
			{
				if (!(key == "other_castle"))
				{
					goto IL_240;
				}
				if (this.Castle == this.OrgCastle)
				{
					return Value.Null;
				}
				return this.Castle;
			}
		}
		else if (num != 3209079471U)
		{
			if (num != 3310976652U)
			{
				if (num != 3343205242U)
				{
					goto IL_240;
				}
				if (!(key == "obj"))
				{
					goto IL_240;
				}
			}
			else
			{
				if (!(key == "def"))
				{
					goto IL_240;
				}
				return this.Def;
			}
		}
		else
		{
			if (!(key == "district"))
			{
				goto IL_240;
			}
			return this.District;
		}
		if (this.Inst != null)
		{
			return this.Inst;
		}
		if (this.Def != null)
		{
			return this.Def;
		}
		return Value.Null;
		IL_240:
		return Value.Unknown;
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x00162988 File Offset: 0x00160B88
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		Value result;
		using (Game.Profile("UIBuildingSlot.SetupHTTooltip", false, 0f, null))
		{
			UIHyperText ht = arg.ht;
			IVars vars = (ht != null) ? ht.vars : null;
			if (vars == null)
			{
				result = Value.Unknown;
			}
			else
			{
				Value var = vars.GetVar("building", null, true);
				if (var.is_unknown)
				{
					var = vars.GetVar("obj", null, true);
					if (var.is_unknown)
					{
						return Value.Unknown;
					}
				}
				Building.Def def = null;
				Building.Def def2;
				Building building;
				DT.Field field;
				if ((def2 = (var.obj_val as Building.Def)) != null)
				{
					def = def2;
				}
				else if ((building = (var.obj_val as Building)) != null)
				{
					def = building.def;
				}
				else if ((field = (var.obj_val as DT.Field)) != null)
				{
					def = Logic.Def.Get<Building.Def>(field);
				}
				if (def == null)
				{
					result = Value.Unknown;
				}
				else
				{
					Logic.Kingdom kingdom = vars.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
					Castle castle = vars.GetVar("castle", null, true).Get<Castle>();
					Castle castle2 = vars.GetVar("other_castle", null, true).Get<Castle>();
					District.Def district = vars.GetVar("district", null, true).Get<District.Def>();
					Building inst = vars.GetVar("instance", null, true).Get<Building>();
					UIBuildingSlot slot = vars.GetVar("slot", null, true).Get<UIBuildingSlot>();
					Vars vars2 = vars as Vars;
					if (vars2 == null)
					{
						vars2 = new Vars(vars);
						arg.ht.vars = vars2;
					}
					if (var.obj_val != def)
					{
						vars2.Set<Building.Def>("building", def);
					}
					UIBuildingSlot.FillTTVars(vars2, def, district, kingdom, castle, castle2 ?? castle, inst, slot);
					vars2.obj = def;
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x00162B80 File Offset: 0x00160D80
	public static Value SetupUpgradesPanel(UIHyperText.CallbackParams arg)
	{
		UIBuildingsPanel component = arg.e.rt.GetComponent<UIBuildingsPanel>();
		if (component == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht = arg.ht;
		IVars vars;
		if (ht == null)
		{
			vars = null;
		}
		else
		{
			UIHyperText uihyperText = ht.RootHT();
			vars = ((uihyperText != null) ? uihyperText.vars : null);
		}
		IVars vars2 = vars;
		if (vars2 == null)
		{
			return Value.Unknown;
		}
		Value var = vars2.GetVar("building", null, true);
		if (var.is_unknown)
		{
			var = vars2.GetVar("obj", null, true);
			if (var.is_unknown)
			{
				return Value.Unknown;
			}
		}
		Building.Def def = null;
		Building.Def def2;
		Building building;
		DT.Field field;
		if ((def2 = (var.obj_val as Building.Def)) != null)
		{
			def = def2;
		}
		else if ((building = (var.obj_val as Building)) != null)
		{
			def = building.def;
		}
		else if ((field = (var.obj_val as DT.Field)) != null)
		{
			def = Logic.Def.Get<Building.Def>(field);
		}
		if (def == null)
		{
			return Value.Unknown;
		}
		Logic.Kingdom kingdom = vars2.GetVar("kingdom", null, true).Get<Logic.Kingdom>();
		Castle castle = vars2.GetVar("castle", null, true).Get<Castle>();
		component.Init(def.upgrades, kingdom, castle);
		return true;
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x00162CA4 File Offset: 0x00160EA4
	public static string BuildPromptKey(Castle Castle, Building.Def Def)
	{
		Castle.StructureBuildAvailability structureBuildAvailability = Castle.CanBuildBuilding(Def, false, false, false);
		if (structureBuildAvailability == Castle.StructureBuildAvailability.AlreadyBuilt)
		{
			return "AlreadyBuilt";
		}
		if (structureBuildAvailability == Castle.StructureBuildAvailability.Available)
		{
			return "Available";
		}
		bool flag = (structureBuildAvailability & Castle.StructureBuildAvailability.NoRequirements) > Castle.StructureBuildAvailability.Available;
		bool flag2 = (structureBuildAvailability & Castle.StructureBuildAvailability.NoParent) > Castle.StructureBuildAvailability.Available;
		if (flag && flag2)
		{
			return "NoParentNoRequirements";
		}
		if (flag)
		{
			return "NoRequirements";
		}
		if (flag2)
		{
			return "NoParent";
		}
		if ((structureBuildAvailability & Castle.StructureBuildAvailability.CannotAfford) != Castle.StructureBuildAvailability.Available)
		{
			return "CannotAfford";
		}
		if ((structureBuildAvailability & Castle.StructureBuildAvailability.UnderConstruction) != Castle.StructureBuildAvailability.Available)
		{
			if (Def.IsUpgrade())
			{
				Logic.Kingdom kingdom = Castle.GetKingdom();
				if (kingdom != null && kingdom.IsUpgrading(Def))
				{
					return "UnderConstruction";
				}
			}
			else if (Castle.IsBuilding(Def))
			{
				return "UnderConstruction";
			}
			return "AnotherBuildInProgress";
		}
		if ((structureBuildAvailability & Castle.StructureBuildAvailability.MaxCountReached) == Castle.StructureBuildAvailability.Available)
		{
			return structureBuildAvailability.ToString();
		}
		if (Castle.GetTier() >= Castle.GetMaxTier())
		{
			return "MaxCountReached";
		}
		return "MustExpandTown";
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x00162D78 File Offset: 0x00160F78
	private static void FillTTVars(Vars vars, Building.Def Def, District.Def District, Logic.Kingdom kingdom, Castle OrgCastle, Castle Castle, Building Inst, UIBuildingSlot slot = null)
	{
		using (Game.Profile("UIBuildingSlot.FillTTVars", false, 0f, null))
		{
			Logic.Realm realm = (Castle != null) ? Castle.GetRealm() : null;
			if (realm != null)
			{
				vars.Set<Logic.Realm>("realm", realm);
			}
			if (kingdom != null)
			{
				vars.Set<Logic.Kingdom>("kingdom", kingdom);
			}
			vars.Set<Castle>("castle", OrgCastle);
			if (Castle != OrgCastle)
			{
				vars.Set<Castle>("other_castle", Castle);
			}
			else
			{
				vars.Set<Value>("other_castle", Value.Null);
			}
			vars.Set<DT.Field>("name", (Def != null) ? Def.field.FindChild("name", null, true, true, true, '.') : null);
			vars.Set<District.Def>("district", District);
			string val = (kingdom != null) ? kingdom.GetPietyIcon(false) : null;
			vars.Set<string>("piety_icon", val);
			vars.Set<Resource>("upkeep", (Def != null) ? Def.CalcUpkeep(kingdom) : null);
			vars.Set<Resource>("cost", (Def != null) ? Def.GetCost(realm, null) : null);
			vars.Set<Resource>("original_cost", (Def != null) ? Def.cost : null);
			if (OrgCastle != null && !OrgCastle.MayBuildBuilding(Def, true))
			{
				vars.Set<bool>("unbuildable", true);
			}
			if (kingdom != null && Def != null && Def.IsUpgrade())
			{
				Resource resource = Def.CalcUpgradeTotalProduction(null, kingdom, 1, true, 1f, 0f, 0f, 0);
				if (resource != null)
				{
					resource.ClearRealmLocalResources();
					if (resource.IsZero())
					{
						resource = null;
					}
				}
				vars.Set<string>("total_kingdom_production", (resource != null) ? resource.GetText("bonus", Def.piety_type, kingdom) : null);
			}
			else
			{
				vars.Set<Value>("total_kingdom_production", Value.Null);
			}
			if (Castle != null && Def != null)
			{
				string str = UIBuildingSlot.BuildPromptKey(Castle, Def);
				DT.Field val2 = Def.field.FindChild("texts.build_prompts." + str, null, true, true, true, '.');
				vars.Set<DT.Field>("build_prompt", val2);
				Resource resource2 = (Inst == null) ? Def.CalcProduction(null, Castle, 1, true, 1f, 0f, 0f, 0) : Inst.CalcProduction(null, true);
				vars.Set<string>("total_production", (resource2 != null) ? resource2.GetText("bonus", Def.piety_type, kingdom) : null);
			}
			else
			{
				vars.Set<Value>("build_prompt", Value.Null);
				vars.Set<Value>("total_production", Value.Null);
			}
			vars.Set<int>("level", (Def == null) ? 0 : Def.CalcLevel(Castle));
			vars.Set<int>("applied_level", (Inst == null) ? 0 : Inst.applied_level);
			if (slot != null)
			{
				vars.Set<string>("state", "#" + slot.state.ToString());
			}
			Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
			ResourceInfo resourceInfo = (kingdom2 != null) ? kingdom2.GetResourceInfo(Def.id, true, true) : null;
			if (resourceInfo != null)
			{
				resourceInfo.SetTextVars(vars, Def.IsUpgrade() ? "Upgrade.availability_texts" : "Building.availability_texts");
				vars.Set<string>("res_info", "#" + resourceInfo.Dump());
			}
			else
			{
				vars.Set<string>("availability_color", "");
				vars.Set<string>("/availability_color", "");
				vars.Set<Value>("res_info", Value.Null);
			}
			UIBuildingSlot.SetMultipleInstanceVars(vars, Def, Castle, Inst);
			UIBuildingSlot.FillRequirements(vars, Def, District, Castle, Inst);
			UIBuildingSlot.FillUpgradesVars(vars, Def, District, kingdom, Inst);
			Building.Def.Bonuses bonuses = (Def != null) ? Def.CalcCurrentBonuses(kingdom) : null;
			string productionText = UIBuildingSlot.GetProductionText(bonuses, vars, Def, OrgCastle, Inst);
			vars.Set<string>("production_tooltip_text", productionText);
			string modsText = UIBuildingSlot.GetModsText(bonuses, vars, Def, OrgCastle, Inst, true);
			vars.Set<string>("mods_tooltip_text", modsText);
			vars.Set<bool>("has_bonuses", !string.IsNullOrEmpty(productionText) || !string.IsNullOrEmpty(modsText));
			vars.Set<string>("produces_tooltip_text", UIBuildingSlot.GetProducesText(vars, Def, kingdom, OrgCastle, Inst, false));
			if (Building.alt_tooltips)
			{
				vars.Set<string>("produces_tooltip_text2", UIBuildingSlot.GetProducesText(vars, Def, kingdom, OrgCastle, Inst, true));
			}
			else
			{
				vars.Set<Value>("produces_tooltip_text2", Value.Null);
			}
		}
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x001631C4 File Offset: 0x001613C4
	public static void FillBuiltLocationsText(Vars vars, Building.Def def, Logic.Kingdom kingdom)
	{
		if (vars == null || def == null || kingdom == null)
		{
			return;
		}
		List<Castle> list = null;
		for (int i = 0; i < kingdom.realms.Count; i++)
		{
			Logic.Realm realm = kingdom.realms[i];
			if (((realm != null) ? realm.castle : null) != null && realm.castle.HasBuilding(def))
			{
				if (list == null)
				{
					list = new List<Castle>();
				}
				list.Add(realm.castle);
			}
		}
		if (list == null || list.Count == 0)
		{
			return;
		}
		list.Sort((Castle a, Castle b) => (b.governor != null).CompareTo(a.governor != null));
		int count = kingdom.court.Count;
		int num = list.Count - count;
		if (num > 0)
		{
			vars.Set<int>("num_additional_built_locations", num);
		}
		int num2 = list.Count;
		if (num2 > count)
		{
			num2 = count;
		}
		string text = "@";
		for (int j = 0; j < num2; j++)
		{
			Castle obj = list[j];
			text = text + "{p}" + global::Defs.LocalizedObjName(obj, null, "", true);
		}
		vars.Set<string>("build_locations", text);
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x001632E8 File Offset: 0x001614E8
	private Vars BuildTooltip()
	{
		Vars vars = new Vars(this);
		vars.Set<Building.Def>("building", this.Def);
		vars.Set<District.Def>("district", this.District);
		vars.Set<Logic.Kingdom>("kingdom", this.Kingdom);
		vars.Set<Castle>("castle", this.OrgCastle);
		if (this.Castle != this.OrgCastle)
		{
			vars.Set<Castle>("other_castle", this.Castle);
		}
		vars.Set<Building>("instance", this.Inst);
		vars.Set<UIBuildingSlot>("slot", this);
		UIBuildingSlot.FillBuiltLocationsText(vars, this.Def, this.Kingdom);
		return vars;
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x00163390 File Offset: 0x00161590
	private static void SetMultipleInstanceVars(Vars vars, Building.Def Def, Castle Castle, Building Inst)
	{
		vars.Set<Value>("num_instances", (Castle == null) ? Value.Null : new Value(Castle.NumInstances(Def, true)));
		if (Def == null || Def.max_instances <= 1)
		{
			vars.Set<Value>("multi_instances", Value.Null);
			return;
		}
		vars.Set<int>("multi_instances", Def.max_instances);
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x001633F0 File Offset: 0x001615F0
	private static void SetValueVars(Vars vars, Building.Def.Bonuses bonuses, string settlement_type, bool is_upgrade, Value final_value)
	{
		Castle castle = vars.Get<Castle>("castle", null);
		Logic.Realm realm = (castle != null) ? castle.GetRealm() : null;
		if (realm == null)
		{
			return;
		}
		int val = Building.CalcSetllementTypeMul(realm, settlement_type, bonuses.location);
		vars.Set<int>("smul", val);
		vars.Set<Value>("final_value", final_value);
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x00163444 File Offset: 0x00161644
	private static DT.Field GetOptionalText(DT.Field condition)
	{
		if (condition == null)
		{
			return null;
		}
		DT.Field field = condition.FindChild("optional_text", null, true, true, true, '.');
		if (field != null)
		{
			return field;
		}
		return null;
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x00163470 File Offset: 0x00161670
	private static void AddBonusText(int line_number, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		UIBuildingSlot.tmp_bonus_texts.Add(new UIBuildingSlot.BonusText
		{
			line_number = line_number,
			text = text
		});
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x001634AC File Offset: 0x001616AC
	private static string GetFinalBonusTexts()
	{
		if (UIBuildingSlot.tmp_bonus_texts.Count == 0)
		{
			return null;
		}
		UIBuildingSlot.tmp_bonus_texts.Sort((UIBuildingSlot.BonusText a, UIBuildingSlot.BonusText b) => a.line_number.CompareTo(b.line_number));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("#");
		for (int i = 0; i < UIBuildingSlot.tmp_bonus_texts.Count; i++)
		{
			string text = UIBuildingSlot.tmp_bonus_texts[i].text;
			if (i > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x00163544 File Offset: 0x00161744
	private static string GetProductionText(Building.Def.Bonuses bonuses, Vars vars, Building.Def Def, Castle Castle, Building Inst)
	{
		if (bonuses == null)
		{
			return null;
		}
		string result;
		using (Game.Profile("UIBuildingSlot.GetProductionText", false, 0f, null))
		{
			DT.Field field = bonuses.field.FindChild("production_tooltip", null, false, true, true, '.');
			if (field != null)
			{
				string text = global::Defs.Localize(field, Def.field, null, false, true);
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					result = "#" + text;
				}
			}
			else
			{
				UIBuildingSlot.tmp_bonus_texts.Clear();
				UIBuildingSlot.AddProductionText(bonuses, vars, Def, Castle, Inst);
				UIBuildingSlot.AddProductionText(bonuses.per_level, vars, Def, Castle, Inst);
				UIBuildingSlot.AddProductionText(bonuses.region, vars, Def, Castle, Inst);
				Building.Def.Bonuses region = bonuses.region;
				UIBuildingSlot.AddProductionText((region != null) ? region.per_level : null, vars, Def, Castle, Inst);
				result = UIBuildingSlot.GetFinalBonusTexts();
			}
		}
		return result;
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x00163624 File Offset: 0x00161824
	private static void AddProductionText(Building.Def.Bonuses bonuses, Vars vars, Building.Def Def, Castle Castle, Building Inst)
	{
		if (((bonuses != null) ? bonuses.productions : null) == null)
		{
			return;
		}
		DT.Field field = (Def != null) ? Def.field.FindChild("bonus_texts." + bonuses.location + ".bonus_row", null, true, true, true, '.') : null;
		if (field == null)
		{
			return;
		}
		if (!bonuses.flat && bonuses.min_level > 1)
		{
			vars.Set<int>("min_level", bonuses.min_level);
			vars.Set<int>("above_level", bonuses.min_level - 1);
		}
		else
		{
			vars.Set<Value>("min_level", Value.Null);
			vars.Set<Value>("above_level", Value.Null);
		}
		int num = vars.Get("level", true).Int(0);
		if (num < 1)
		{
			num = 1;
		}
		bool is_upgrade = Def.IsUpgrade();
		bool flag = vars.Get("unbuildable", true).Bool();
		foreach (KeyValuePair<string, List<Building.ConditionalProduction.Def>> keyValuePair in bonuses.productions)
		{
			string key = keyValuePair.Key;
			List<Building.ConditionalProduction.Def> value = keyValuePair.Value;
			if (value != null)
			{
				int i = 0;
				while (i < value.Count)
				{
					Building.ConditionalProduction.Def def = value[i];
					Resource resource = new Resource();
					def.AddValue(resource, Castle, num, false, true, 1f);
					bool flag2 = resource.IsZero();
					if (!flag2)
					{
						goto IL_170;
					}
					if (def.condition == null || !def.condition.GetBool("hide_if_inactive", null, false, true, true, true, '.'))
					{
						if (Castle == null)
						{
							flag2 = false;
							goto IL_170;
						}
						goto IL_170;
					}
					IL_210:
					i++;
					continue;
					IL_170:
					vars.Set<bool>("inactive", flag2);
					vars.Set<DT.Field>("optional_text", UIBuildingSlot.GetOptionalText(def.condition));
					UIBuildingSlot.SetValueVars(vars, bonuses, key, is_upgrade, (resource != null) ? resource.GetText(null, (Def != null) ? Def.piety_type : null, (Castle != null) ? Castle.GetKingdom() : null) : null);
					string text = UIBuildingSlot.LocalizeProduction(def, bonuses.location, key, Def, Castle, vars, flag2);
					if (!string.IsNullOrEmpty(text))
					{
						vars.Set<string>("bonus_text", text);
						string text2 = global::Defs.Localize(field, vars, null, true, true);
						UIBuildingSlot.AddBonusText(def.line_number, text2);
						goto IL_210;
					}
					goto IL_210;
				}
			}
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x00163890 File Offset: 0x00161A90
	public static string LocalizeProduction(Building.ConditionalProduction.Def prod_def, string location, string settlement_type, Building.Def def, Castle castle, Vars vars, bool inactive)
	{
		if (settlement_type == "Castle")
		{
			settlement_type = "Town";
		}
		string text = prod_def.resources.GetText(inactive ? null : "bonus", def.piety_type, (castle != null) ? castle.GetKingdom() : null);
		DT.Field field;
		if (def == null)
		{
			field = null;
		}
		else
		{
			DT.Field field2 = def.field;
			field = ((field2 != null) ? field2.FindChild("bonus_texts." + location + "." + settlement_type, null, true, true, true, '.') : null);
		}
		DT.Field field3 = field;
		if (field3 == null)
		{
			return text;
		}
		vars.Set<string>("bonus", text);
		return "#" + global::Defs.Localize(field3, vars, null, true, true);
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x00163934 File Offset: 0x00161B34
	private static string GetModsText(Building.Def.Bonuses bonuses, Vars vars, Building.Def Def, Castle Castle, Building Inst, bool add_tooltip = false)
	{
		string result;
		using (Game.Profile("UIBuildingSlot.GetModsText", false, 0f, null))
		{
			DT.Field field;
			if (Def == null)
			{
				field = null;
			}
			else
			{
				DT.Field field2 = Def.field;
				field = ((field2 != null) ? field2.FindChild("mods_tooltip", null, false, true, true, '.') : null);
			}
			DT.Field field3 = field;
			if (field3 != null)
			{
				string text = global::Defs.Localize(field3, Def.field, null, false, true);
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					result = "#" + text;
				}
			}
			else
			{
				UIBuildingSlot.tmp_bonus_texts.Clear();
				UIBuildingSlot.AddModsText(bonuses, vars, Def, Castle, Inst);
				string text2 = UIBuildingSlot.GetFinalBonusTexts();
				if (add_tooltip)
				{
					string text3 = global::Defs.Localize((Def != null) ? Def.field : null, "add_tooltip", vars, null, false, true);
					if (!string.IsNullOrEmpty(text3))
					{
						if (string.IsNullOrEmpty(text2))
						{
							text2 = "#";
						}
						else
						{
							text2 += "\n";
						}
						text2 += text3;
					}
				}
				result = text2;
			}
		}
		return result;
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x00163A38 File Offset: 0x00161C38
	private static bool HideIfInactive(Building.Def Def, Building.StatModifier.Def mod_def, Vars vars, bool is_kingdom_mod)
	{
		if (mod_def.condition == null)
		{
			return false;
		}
		DT.Field condition = mod_def.condition;
		DT.Field field = (condition != null) ? condition.FindChild("hide_if_inactive", null, true, true, true, '.') : null;
		return field != null && field.Bool(vars, false);
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x00163A80 File Offset: 0x00161C80
	private static void AddModsText(Building.Def.Bonuses bonuses, Vars vars, Building.Def Def, Castle Castle, Building Inst)
	{
		if (((bonuses != null) ? bonuses.stat_mods : null) == null)
		{
			return;
		}
		DT.Field field = (Def != null) ? Def.field.FindChild("bonus_texts." + bonuses.location + ".bonus_row", null, true, true, true, '.') : null;
		if (field == null)
		{
			return;
		}
		if (!bonuses.flat && bonuses.min_level > 1)
		{
			vars.Set<int>("min_level", bonuses.min_level);
			vars.Set<int>("above_level", bonuses.min_level - 1);
		}
		else
		{
			vars.Set<Value>("min_level", Value.Null);
			vars.Set<Value>("above_level", Value.Null);
		}
		Castle castle = vars.Get<Castle>("castle", null);
		int num = vars.Get("level", true).Int(0);
		if (num < 1)
		{
			num = 1;
		}
		bool is_upgrade = Def.IsUpgrade();
		bool flag = vars.Get("unbuildable", true).Bool();
		foreach (KeyValuePair<string, Building.Def.PerSettlementModidfiers> keyValuePair in ((bonuses != null) ? bonuses.stat_mods : null))
		{
			string key = keyValuePair.Key;
			Building.Def.PerSettlementModidfiers value = keyValuePair.Value;
			if (value.realm_mods != null)
			{
				for (int i = 0; i < value.realm_mods.Count; i++)
				{
					Building.StatModifier.Def def = value.realm_mods[i];
					float num2 = flag ? 0f : def.CalcValue(castle, num);
					bool flag2 = Castle != null && num2 == 0f;
					if (!flag2 || !UIBuildingSlot.HideIfInactive(Def, def, vars, false))
					{
						vars.Set<bool>("inactive", flag2);
						vars.Set<DT.Field>("optional_text", UIBuildingSlot.GetOptionalText(def.condition));
						UIBuildingSlot.SetValueVars(vars, bonuses, key, is_upgrade, num2);
						string text = UIBuildingSlot.LocalizeStatModifier(def, "RealmStats", bonuses.location, key, Def, vars);
						if (!string.IsNullOrEmpty(text))
						{
							vars.Set<string>("bonus_text", "#" + text);
							string text2 = global::Defs.Localize(field, vars, null, true, true);
							UIBuildingSlot.AddBonusText(def.field.line, text2);
						}
					}
				}
			}
			if (value.kingdom_mods != null)
			{
				for (int j = 0; j < value.kingdom_mods.Count; j++)
				{
					Building.StatModifier.Def def2 = value.kingdom_mods[j];
					float num3 = flag ? 0f : def2.CalcValue(castle, num);
					bool flag3 = Castle != null && num3 == 0f;
					if (!flag3 || !UIBuildingSlot.HideIfInactive(Def, def2, vars, true))
					{
						vars.Set<bool>("inactive", flag3);
						vars.Set<DT.Field>("optional_text", UIBuildingSlot.GetOptionalText(def2.condition));
						UIBuildingSlot.SetValueVars(vars, bonuses, key, is_upgrade, num3);
						string text3 = UIBuildingSlot.LocalizeStatModifier(def2, "KingdomStats", bonuses.location, key, Def, vars);
						if (!string.IsNullOrEmpty(text3))
						{
							vars.Set<string>("bonus_text", "#" + text3);
							string text4 = global::Defs.Localize(field, vars, null, true, true);
							UIBuildingSlot.AddBonusText(def2.field.line, text4);
						}
					}
				}
			}
		}
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x00163DE0 File Offset: 0x00161FE0
	public static string LocalizeStatModifier(Building.StatModifier.Def mod_def, string stats, string location, string settlement_type, Building.Def def, Vars vars)
	{
		vars.obj = new Value((mod_def != null) ? mod_def.field : null);
		vars.Set<float>("value", mod_def.value);
		DT.Field field = mod_def.field.FindChild("tooltip", null, true, true, true, '.');
		if (field != null)
		{
			return global::Defs.Localize(field, vars, null, false, true);
		}
		string text = global::Defs.LocalizeStatModifier(stats, mod_def.stat_name, mod_def.value, mod_def.type, vars, false, false);
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		if (string.IsNullOrEmpty(settlement_type))
		{
			return text;
		}
		DT.Field field2;
		if (def == null)
		{
			field2 = null;
		}
		else
		{
			DT.Field field3 = def.field;
			field2 = ((field3 != null) ? field3.FindChild("bonus_texts." + location + "." + settlement_type, null, true, true, true, '.') : null);
		}
		DT.Field field4 = field2;
		if (field4 == null)
		{
			return text;
		}
		vars.Set<string>("bonus", "#" + text);
		return global::Defs.Localize(field4, vars, null, true, true);
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x00163ECC File Offset: 0x001620CC
	private static void AddProduces(List<Building.Def.ProducedResource> res, List<Building.Def.ProducedResource> lst, ResourceInfo.Availability availability)
	{
		for (int i = 0; i < lst.Count; i++)
		{
			Building.Def.ProducedResource pr = lst[i];
			pr.availability = availability;
			UIBuildingSlot.AddProduces(res, pr);
		}
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x00163F04 File Offset: 0x00162104
	private static void AddProduces(List<Building.Def.ProducedResource> res, Building.Def.ProducedResource pr)
	{
		int i = 0;
		while (i < res.Count)
		{
			Building.Def.ProducedResource producedResource = res[i];
			if (!(producedResource.resource != pr.resource) && !(producedResource.type != pr.type))
			{
				if (producedResource.availability <= pr.availability)
				{
					return;
				}
				res[i] = pr;
				return;
			}
			else
			{
				i++;
			}
		}
		res.Add(pr);
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x00163F70 File Offset: 0x00162170
	private static bool CheckPrerequisites(Building.Def bef, District.Def.BuildingInfo bi, Logic.Kingdom kingdom)
	{
		if (bi.prerequisites != null)
		{
			for (int i = 0; i < bi.prerequisites.Count; i++)
			{
				Building.Def def = bi.prerequisites[i];
				if (!def.IsUpgrade())
				{
					Debug.LogError(string.Format("{0} has non-upgrade prerequisite {1}", bi.def, def));
				}
				else if (!kingdom.HasBuildingUpgrade(def))
				{
					return false;
				}
			}
		}
		if (bi.prerequisites_or == null)
		{
			return true;
		}
		for (int j = 0; j < bi.prerequisites_or.Count; j++)
		{
			Building.Def def2 = bi.prerequisites_or[j];
			if (!def2.IsUpgrade())
			{
				Debug.LogError(string.Format("{0} has non-upgrade prerequisite {1}", bi.def, def2));
			}
			else if (kingdom.HasBuildingUpgrade(def2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x0016402C File Offset: 0x0016222C
	private static bool CheckRequirements(Building.Def bef, District.Def.BuildingInfo bi, Logic.Kingdom kingdom)
	{
		if (bi.def.requires != null)
		{
			for (int i = 0; i < bi.def.requires.Count; i++)
			{
				if (!UIBuildingSlot.CheckRequirement(bi.def.requires[i], kingdom))
				{
					return false;
				}
			}
		}
		if (bi.def.requires_or == null)
		{
			return true;
		}
		for (int j = 0; j < bi.def.requires_or.Count; j++)
		{
			if (UIBuildingSlot.CheckRequirement(bi.def.requires_or[j], kingdom))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x001640C4 File Offset: 0x001622C4
	private static bool CheckRequirement(Building.Def.RequirementInfo req, Logic.Kingdom kingdom)
	{
		if (req.type == "Religion")
		{
			return kingdom.religion != null && kingdom.religion.HasTag(req.key);
		}
		return kingdom.GetRealmTag(req.key) > 0;
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x00164113 File Offset: 0x00162313
	private static ResourceInfo.Availability CalcUpgradeAvailability(Building.Def bdef, District.Def.BuildingInfo bi, Logic.Kingdom kingdom)
	{
		if (kingdom == null)
		{
			return ResourceInfo.Availability.DirectlyObtainable;
		}
		if (kingdom.HasBuildingUpgrade(bi.def))
		{
			return ResourceInfo.Availability.Available;
		}
		if (!UIBuildingSlot.CheckPrerequisites(bdef, bi, kingdom))
		{
			return ResourceInfo.Availability.Impossible;
		}
		if (!UIBuildingSlot.CheckRequirements(bdef, bi, kingdom))
		{
			return ResourceInfo.Availability.Impossible;
		}
		return ResourceInfo.Availability.DirectlyObtainable;
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x00164144 File Offset: 0x00162344
	private static string ColorCodedProducesText(string def_id, ResourceInfo.Availability availability)
	{
		if (global::Defs.GetDefField(def_id, null) == null)
		{
			return "{" + def_id + ".name}";
		}
		string text;
		if (availability != ResourceInfo.Availability.Available)
		{
			if (availability != ResourceInfo.Availability.DirectlyObtainable)
			{
				text = "clr:sp_resource_gray_color";
			}
			else
			{
				text = "clr:sp_resource_dark_color";
			}
		}
		else
		{
			text = "clr:sp_resource_color";
		}
		string text2 = "/clr";
		return string.Concat(new string[]
		{
			"{",
			def_id,
			":link}{",
			text,
			"}{",
			def_id,
			".name}{",
			text2,
			"}{/link}"
		});
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x001641D4 File Offset: 0x001623D4
	public static List<Building.Def.ProducedResource> AggregateProduces(Building.Def Def, Logic.Kingdom kingdom, bool missing)
	{
		if (Def == null)
		{
			return null;
		}
		District.Def upgrades = Def.upgrades;
		if (((upgrades != null) ? upgrades.buildings : null) == null)
		{
			if (!missing)
			{
				return Def.produces;
			}
			return null;
		}
		else
		{
			List<Building.Def.ProducedResource> list = null;
			for (int i = 0; i < Def.upgrades.buildings.Count; i++)
			{
				District.Def.BuildingInfo buildingInfo = Def.upgrades.buildings[i];
				if (buildingInfo.def.produces != null && (!Building.alt_tooltips || (kingdom != null && kingdom.HasBuildingUpgrade(buildingInfo.def)) != missing))
				{
					if (list == null)
					{
						if (Def.produces == null || missing)
						{
							list = new List<Building.Def.ProducedResource>();
						}
						else
						{
							list = new List<Building.Def.ProducedResource>(Def.produces);
						}
					}
					ResourceInfo.Availability availability = Building.alt_tooltips ? ResourceInfo.Availability.Unknown : UIBuildingSlot.CalcUpgradeAvailability(Def, buildingInfo, kingdom);
					UIBuildingSlot.AddProduces(list, buildingInfo.def.produces, availability);
				}
			}
			if (list == null)
			{
				return Def.produces;
			}
			return list;
		}
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x001642B8 File Offset: 0x001624B8
	public static string GetProducesText(List<Building.Def.ProducedResource> produces, Building.Def Def, Logic.Kingdom kingdom, Castle Castle, Building Inst)
	{
		if (produces == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (kingdom == null)
		{
			kingdom = ((Castle != null) ? Castle.GetKingdom() : null);
		}
		for (int i = 0; i < produces.Count; i++)
		{
			Building.Def.ProducedResource producedResource = produces[i];
			if (!(producedResource.type == "tag"))
			{
				string value;
				if (Building.alt_tooltips)
				{
					value = UIBuildingSlot.ColorCodedText(producedResource.resource, null, kingdom);
				}
				else
				{
					value = UIBuildingSlot.ColorCodedProducesText(producedResource.resource, producedResource.availability);
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value);
			}
		}
		if (stringBuilder.Length == 0)
		{
			return null;
		}
		return "@" + stringBuilder.ToString();
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x0016436C File Offset: 0x0016256C
	private static string GetProducesText(Vars vars, Building.Def Def, Logic.Kingdom kingdom, Castle Castle, Building Inst, bool missing = false)
	{
		if (!missing)
		{
			DT.Field field;
			if (Def == null)
			{
				field = null;
			}
			else
			{
				DT.Field field2 = Def.field;
				field = ((field2 != null) ? field2.FindChild("produces_tooltip", null, false, true, true, '.') : null);
			}
			DT.Field field3 = field;
			if (field3 != null)
			{
				return "#" + global::Defs.Localize(field3, Def.field, null, true, true);
			}
		}
		return UIBuildingSlot.GetProducesText(UIBuildingSlot.AggregateProduces(Def, kingdom, missing), Def, kingdom, Castle, Inst);
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x001643D4 File Offset: 0x001625D4
	public static string ColorCodedText(string def_id, Castle castle, Logic.Kingdom kingdom = null)
	{
		string text = null;
		string text2 = null;
		if (kingdom == null)
		{
			kingdom = ((castle != null) ? castle.GetKingdom() : null);
		}
		global::Defs defs = global::Defs.Get(false);
		string a;
		DT.Field field;
		bool flag = Building.Def.ResolveRequirement(def_id, (defs != null) ? defs.dt : null, out a, out field);
		if (((field != null) ? field.Type() : null) != "def")
		{
			flag = false;
		}
		if (a != "Religion")
		{
			if (kingdom != null)
			{
				ResourceInfo resourceInfo = (castle == null || a == "Resource") ? kingdom.GetResourceInfo(def_id, true, true) : castle.GetResourceInfo(def_id, true, true);
				if (resourceInfo != null)
				{
					resourceInfo.GetColorTags(out text, out text2);
				}
			}
		}
		else
		{
			Religion religion = (kingdom != null) ? kingdom.religion : null;
			if (religion != null)
			{
				if (religion.HasTag(def_id))
				{
					text = "requirement_met";
				}
				else
				{
					text = "requirement_not_met";
				}
			}
			if (text != null)
			{
				text2 = "/" + text;
			}
		}
		string text3 = "";
		if (flag)
		{
			text3 = text3 + "{" + def_id + ":link}";
		}
		if (text != null)
		{
			text3 = text3 + "{" + text + "}";
		}
		text3 = text3 + "{" + def_id + ".name}";
		if (text2 != null)
		{
			text3 = text3 + "{" + text2 + "}";
		}
		if (flag)
		{
			text3 = text3 + "{" + def_id + ":/link}";
		}
		return text3;
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x0016452C File Offset: 0x0016272C
	private static bool FillRequirements(string key, Vars vars, List<Building.Def.RequirementInfo> reqs, string prefix, Building.Def Def, Castle Castle, Building Inst)
	{
		bool result;
		using (Game.Profile("UIBuildingSlot.FillRequirements", false, 0f, null))
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < reqs.Count; i++)
			{
				Building.Def.RequirementInfo requirementInfo = reqs[i];
				string text = null;
				string text2 = null;
				Logic.Kingdom kingdom = ((Castle != null) ? Castle.GetKingdom() : null) ?? vars.Get<Logic.Kingdom>("kingdom", null);
				if (requirementInfo.type != "Religion")
				{
					flag3 = true;
					if (kingdom != null)
					{
						ResourceInfo resourceInfo = (Castle == null || requirementInfo.type == "Resource") ? kingdom.GetResourceInfo(requirementInfo.key, true, true) : Castle.GetResourceInfo(requirementInfo.key, true, true);
						if (resourceInfo != null)
						{
							resourceInfo.GetColorTags(out text, out text2);
						}
					}
				}
				else
				{
					Religion religion = (kingdom != null) ? kingdom.religion : null;
					if (religion != null)
					{
						if (religion.HasTag(requirementInfo.key))
						{
							text = "requirement_met";
							flag2 = true;
						}
						else
						{
							text = "requirement_not_met";
							flag = false;
						}
					}
					else
					{
						flag3 = true;
					}
					if (text != null)
					{
						text2 = "/" + text;
					}
				}
				DT.Field def = requirementInfo.def;
				string text3 = "@";
				if (def != null)
				{
					text3 = text3 + "{" + requirementInfo.field.key + ":link}";
				}
				if (text != null)
				{
					text3 = text3 + "{" + text + "}";
				}
				text3 = text3 + "{" + requirementInfo.field.key + ".name}";
				if (text2 != null)
				{
					text3 = text3 + "{" + text2 + "}";
				}
				if (def != null)
				{
					text3 = text3 + "{" + requirementInfo.field.key + ":/link}";
				}
				vars.Set<string>(prefix + (i + 1), text3);
			}
			if (flag3)
			{
				result = false;
			}
			else
			{
				result = ((key == "requirements_or") ? (flag2 || reqs.Count == 0) : flag);
			}
		}
		return result;
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x00164764 File Offset: 0x00162964
	private static bool NeedsExplicitDistrictPrerequisite(List<Building.Def> preqs, District.Def District, Castle Castle)
	{
		return Castle != null && District != null && District.IsOptional();
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x0016477C File Offset: 0x0016297C
	private static bool FillPrerequisites(string key, Vars vars, List<Building.Def> preqs, string prefix, Building.Def Def, District.Def District, Castle Castle, Building Inst)
	{
		bool result;
		using (Game.Profile("UIBuildingSlot.FillPrerequisites", false, 0f, null))
		{
			int num = (preqs == null) ? 0 : preqs.Count;
			int num2 = 0;
			bool flag = true;
			bool flag2 = false;
			if (key == "prerequisites" && UIBuildingSlot.NeedsExplicitDistrictPrerequisite(preqs, District, Castle))
			{
				string text = null;
				string text2 = null;
				if (Castle.HasDistrict(District))
				{
					text = "requirement_met";
					text2 = "/requirement_met";
				}
				else
				{
					flag = false;
					text = "requirement_currently_impossible";
					text2 = "/requirement_currently_impossible";
					Building.Def parent = District.GetParent();
					if (parent != null)
					{
						ResourceInfo resourceInfo = Castle.GetResourceInfo(parent.id, true, true);
						if (resourceInfo != null)
						{
							resourceInfo.GetColorTags(out text, out text2);
						}
					}
				}
				Building.Def parent2 = District.GetParent();
				string str = ((parent2 != null) ? parent2.id : null) ?? District.id;
				string text3 = "@";
				text3 = text3 + "{" + str + ":link}";
				if (text != null)
				{
					text3 = text3 + "{" + text + "}";
				}
				text3 = text3 + "{" + str + ".name}";
				if (text2 != null)
				{
					text3 = text3 + "{" + text2 + "}";
				}
				text3 = text3 + "{" + str + ":/link}";
				vars.Set<string>(prefix + "1", text3);
				num2++;
				num++;
			}
			if (num == 0)
			{
				result = flag;
			}
			else
			{
				vars.Set<string>(key, string.Format("Building.{0}_text_{1}", key, num));
				Logic.Kingdom kingdom = (Castle != null) ? Castle.GetKingdom() : null;
				if (kingdom == null)
				{
					kingdom = vars.Get("kingdom", true).Get<Logic.Kingdom>();
				}
				for (int i = num2; i < num; i++)
				{
					Building.Def def = preqs[i - num2];
					string text4 = null;
					string text5 = null;
					if (Castle != null)
					{
						if (Castle.HasWorkingBuilding(def))
						{
							text4 = "requirement_met";
							text5 = "/requirement_met";
							flag2 = true;
						}
						else
						{
							ResourceInfo resourceInfo2 = Castle.GetResourceInfo(def.id, true, true);
							if (resourceInfo2 != null)
							{
								resourceInfo2.GetColorTags(out text4, out text5);
							}
							flag = false;
						}
					}
					else
					{
						flag = false;
						flag2 = false;
						ResourceInfo resourceInfo3 = (kingdom != null) ? kingdom.GetResourceInfo(def.id, true, true) : null;
						if (resourceInfo3 != null)
						{
							resourceInfo3.GetColorTags(out text4, out text5);
						}
					}
					string text6 = "@";
					text6 = text6 + "{" + def.id + ":link}";
					if (text4 != null)
					{
						text6 = text6 + "{" + text4 + "}";
					}
					text6 = text6 + "{" + def.id + ".name}";
					if (text5 != null)
					{
						text6 = text6 + "{" + text5 + "}";
					}
					text6 = text6 + "{" + def.id + ":/link}";
					vars.Set<string>(prefix + (i + 1), text6);
				}
				result = ((key == "prerequisites") ? flag : flag2);
			}
		}
		return result;
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x00164AB0 File Offset: 0x00162CB0
	private static void FillRequirements(Vars vars, Building.Def Def, District.Def District, Castle Castle, Building Inst)
	{
		if (Def == null)
		{
			return;
		}
		bool flag = true;
		if (Def.requires != null)
		{
			vars.Set<string>("requirements", "Building.requires_text_" + Def.requires.Count);
			flag &= UIBuildingSlot.FillRequirements("requirements", vars, Def.requires, "req", Def, Castle, Inst);
		}
		if (Def.requires_or != null)
		{
			vars.Set<string>("requirements_or", "Building.requires_or_text_" + Def.requires_or.Count);
			flag &= UIBuildingSlot.FillRequirements("requirements_or", vars, Def.requires_or, "req_or", Def, Castle, Inst);
		}
		vars.Set<bool>("requirements_met", flag);
		List<Building.Def> prerequisites = Def.GetPrerequisites(District);
		bool flag2 = UIBuildingSlot.FillPrerequisites("prerequisites", vars, prerequisites, "preq", Def, District, Castle, Inst);
		List<Building.Def> prerequisitesOr = Def.GetPrerequisitesOr(District);
		flag2 &= UIBuildingSlot.FillPrerequisites("prerequisites_or", vars, prerequisitesOr, "preq_or_", Def, District, Castle, Inst);
		vars.Set<bool>("prerequisites_met", flag2);
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x00164BB0 File Offset: 0x00162DB0
	private static void FillUpgradesVars(Vars vars, Building.Def Def, District.Def District, Logic.Kingdom k, Building Inst)
	{
		bool flag;
		if (Def == null)
		{
			flag = (null != null);
		}
		else
		{
			District.Def upgrades = Def.upgrades;
			flag = (((upgrades != null) ? upgrades.buildings : null) != null);
		}
		if (!flag)
		{
			return;
		}
		using (Game.Profile("UIBuildingSlot.FillUpgradesVars", false, 0f, null))
		{
			int num = 0;
			int num2 = 0;
			global::Defs defs = global::Defs.Get(false);
			if (defs != null)
			{
				DT dt = defs.dt;
			}
			List<string> list = null;
			for (int i = 0; i < Def.upgrades.buildings.Count; i++)
			{
				District.Def.BuildingInfo buildingInfo = Def.upgrades.buildings[i];
				if (k == null || k.CheckReligionRequirements(buildingInfo.def))
				{
					num++;
					if (list == null)
					{
						list = new List<string>();
					}
					ResourceInfo.Availability availability = UIBuildingSlot.CalcUpgradeAvailability(Def, buildingInfo, k);
					string item = "@" + (Building.alt_tooltips ? UIBuildingSlot.ColorCodedText(buildingInfo.def.id, null, k) : UIBuildingSlot.ColorCodedProducesText(buildingInfo.def.id, availability));
					list.Add(item);
					if (k != null && k.HasBuildingUpgrade(buildingInfo.def))
					{
						num2++;
					}
				}
			}
			vars.Set<int>("num_upgrades", num);
			if (k != null)
			{
				vars.Set<int>("num_unlocked_upgrades", num2);
				vars.Set<Value>("upgrades_list", new Value(list));
			}
		}
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x00164D18 File Offset: 0x00162F18
	private void SetupTooltip()
	{
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		if (this.state == UIBuildingSlot.State.Locked)
		{
			tooltip.SetDef("ExpandCitySlotsTootip", new Vars(this.Castle));
		}
		else if (this.Inst == null && this.Def == null)
		{
			tooltip.SetDef("EmptyBuildingSlotTooltip", new Vars(this.Castle));
		}
		else
		{
			this.tooltipVars = this.BuildTooltip();
			tooltip.SetDef("BuildingTooltip", this.tooltipVars);
		}
		if (this.m_BuildingsCount != null)
		{
			Tooltip.Get(this.m_BuildingsCount, true).SetDef("BuildingsCountTooltip", this.tooltipVars);
		}
		if (this.m_CancelBuild != null)
		{
			Tooltip.Get(this.m_CancelBuild.gameObject, true).SetDef("CancelBuildTooltip", this.tooltipVars);
		}
		if (this.Button_RemoveStructure != null)
		{
			Tooltip.Get(this.Button_RemoveStructure.gameObject, true).SetDef("RemoveBuildTooltip", this.tooltipVars);
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		baseUI.RefreshTooltip(tooltip, false);
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x00164E3C File Offset: 0x0016303C
	private static bool ValidateCancelBuild(Vars vars, out Logic.Kingdom kingdom, out Castle castle, out Building.Def def)
	{
		kingdom = null;
		castle = null;
		def = null;
		if (vars == null)
		{
			return false;
		}
		def = vars.obj.Get<Building.Def>();
		if (def == null)
		{
			return false;
		}
		kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
		castle = vars.Get<Castle>("castle", null);
		if (kingdom == null)
		{
			Castle castle2 = castle;
			kingdom = ((castle2 != null) ? castle2.GetKingdom() : null);
			if (kingdom == null)
			{
				return false;
			}
		}
		Value value = vars.Get("source", true);
		if (!value.is_unknown)
		{
			GameObject gameObject = value.Get<GameObject>();
			if (gameObject == null || !gameObject.activeInHierarchy)
			{
				return false;
			}
		}
		if (castle == null)
		{
			return kingdom.IsUpgrading(def);
		}
		return castle.IsBuilding(def) && castle.GetKingdom() == kingdom;
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x00164F00 File Offset: 0x00163100
	private static void OnCancelBuldWndUpdate(MessageWnd wnd)
	{
		Logic.Kingdom kingdom;
		Castle castle;
		Building.Def def;
		if (!UIBuildingSlot.ValidateCancelBuild(wnd.vars, out kingdom, out castle, out def))
		{
			wnd.Close(false);
		}
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x00164F28 File Offset: 0x00163128
	private static bool CancelBuild(Vars vars)
	{
		Logic.Kingdom kingdom;
		Castle castle;
		Building.Def def;
		if (!UIBuildingSlot.ValidateCancelBuild(vars, out kingdom, out castle, out def))
		{
			return false;
		}
		if (castle != null)
		{
			castle.CancelBuild();
			return true;
		}
		return kingdom.CancelUpgrading(def);
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x00164F58 File Offset: 0x00163158
	private static bool OnCancelBuldWndButtonClick(MessageWnd wnd, string btn_id)
	{
		bool result = true;
		if (btn_id == "confirm")
		{
			result = UIBuildingSlot.CancelBuild(wnd.vars);
		}
		wnd.Close(false);
		return result;
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x00164F88 File Offset: 0x00163188
	public static Vars CreateCancelBuildVars(Castle castle, Logic.Kingdom kingdom, Building.Def def, GameObject source = null)
	{
		if (kingdom == null)
		{
			kingdom = ((castle != null) ? castle.GetKingdom() : null);
			if (kingdom == null)
			{
				return null;
			}
		}
		if (def == null)
		{
			Building.Def def2;
			if (castle == null)
			{
				def2 = null;
			}
			else
			{
				Castle.Build structure_build = castle.structure_build;
				def2 = ((structure_build != null) ? structure_build.current_building_def : null);
			}
			def = def2;
			if (def == null)
			{
				return null;
			}
		}
		Logic.Realm realm = (castle != null) ? castle.GetRealm() : null;
		Vars vars = new Vars(def);
		vars.Set<Castle>("castle", castle);
		vars.Set<Logic.Realm>("realm", realm);
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		if (source != null)
		{
			vars.Set<GameObject>("source", source);
		}
		vars.Set<Resource>("cancel_refunds", def.GetBuildRefunds(realm, kingdom));
		return vars;
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x00165034 File Offset: 0x00163234
	public static MessageWnd HandleCancelBuild(Castle castle, Logic.Kingdom kingdom, Building.Def def, GameObject source = null)
	{
		if (castle != null && castle.GetRealm().IsDisorder())
		{
			return null;
		}
		Vars vars = UIBuildingSlot.CreateCancelBuildVars(castle, kingdom, def, source);
		if (vars == null)
		{
			return null;
		}
		if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
		{
			UIBuildingSlot.CancelBuild(vars);
			return null;
		}
		MessageWnd messageWnd = MessageWnd.Create("ConfirmCancelBuilding", vars, null, new MessageWnd.OnButton(UIBuildingSlot.OnCancelBuldWndButtonClick));
		if (messageWnd == null)
		{
			return null;
		}
		messageWnd.on_update = new MessageWnd.OnUpdate(UIBuildingSlot.OnCancelBuldWndUpdate);
		return messageWnd;
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x001650BC File Offset: 0x001632BC
	private void HandleCancelBuild(BSGButton b)
	{
		if (!this.IsUnderConstruction())
		{
			return;
		}
		UIBuildingSlot.HandleCancelBuild(this.Def.IsUpgrade() ? null : this.Castle, this.Kingdom, this.Def, base.gameObject);
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x001650F8 File Offset: 0x001632F8
	private void UpdateInteractions()
	{
		bool flag = this.Castle != null && this.Castle.GetRealm().IsDisorder();
		if (this.Button_RemoveStructure != null)
		{
			bool flag2;
			if (this.mouse_in && !flag && this.state_def != null)
			{
				Building.Def def = this.Def;
				if (def != null && !def.IsUpgrade())
				{
					flag2 = this.state_def.GetBool("show_remove_button", null, false, true, true, true, '.');
					goto IL_6E;
				}
			}
			flag2 = false;
			IL_6E:
			bool active = flag2;
			this.Button_RemoveStructure.gameObject.SetActive(active);
		}
		if (this.m_CancelBuild != null)
		{
			bool active2 = this.mouse_in && !flag && this.IsUnderConstruction();
			this.m_CancelBuild.gameObject.SetActive(active2);
		}
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x001651BC File Offset: 0x001633BC
	private void UpdateStatusIcon()
	{
		if (this.m_Status == null)
		{
			return;
		}
		if (this.state_def == null)
		{
			return;
		}
		bool @bool = this.state_def.GetBool("show_status_icon", null, false, true, true, true, '.');
		this.m_Status.gameObject.SetActive(@bool);
		if (!@bool)
		{
			return;
		}
		if (this.m_StatusIcon != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.state_def, "status_icon", null);
			if (obj != null)
			{
				this.m_StatusIcon.overrideSprite = obj;
			}
			this.m_StatusIcon.color = global::Defs.GetColor(this.state_def, "status_icon_color", null);
		}
		if (this.m_StatusIconBorder != null)
		{
			this.m_StatusIconBorder.color = global::Defs.GetColor(this.state_def, "status_frame_color", null);
		}
		if (this.m_StatusBackground != null)
		{
			this.m_StatusBackground.color = global::Defs.GetColor(this.state_def, "status_background_color", null);
		}
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x001652B4 File Offset: 0x001634B4
	private int GetBuildingsCountValue()
	{
		if (this.Def == null || this.Def.IsUpgrade())
		{
			return -1;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom == null)
		{
			return -1;
		}
		return kingdom.GetRealmTag(this.Def.id);
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x001652F8 File Offset: 0x001634F8
	private void UpdateBuildingsCount()
	{
		if (this.m_BuildingsCount == null)
		{
			return;
		}
		int buildingsCountValue = this.GetBuildingsCountValue();
		if (buildingsCountValue < 1)
		{
			this.m_BuildingsCount.SetActive(false);
			return;
		}
		UIText.SetText(this.m_BuildingsCountValue, buildingsCountValue.ToString());
		this.m_BuildingsCount.SetActive(true);
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x0016534C File Offset: 0x0016354C
	private void UpdateNamePlate()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_BuildingNameContainer != null)
		{
			bool flag = this.state_def.GetBool("show_nameplate", null, false, true, true, true, '.') && this.m_AllowNamePlates;
			this.m_BuildingNameContainer.gameObject.SetActive(flag);
			if (flag)
			{
				Image component = this.m_BuildingNameContainer.GetComponent<Image>();
				if (component != null)
				{
					component.color = global::Defs.GetColor(this.state_def, "label_background_color", null);
				}
			}
		}
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x001653D4 File Offset: 0x001635D4
	private void UpdateDistrictColor()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_DistrictType == null)
		{
			return;
		}
		District.Def district = this.District;
		DT.Field field = (district != null) ? district.field : null;
		UIDistrictPanel parentComponent = global::Common.GetParentComponent<UIDistrictPanel>(base.gameObject);
		bool flag = this.Inst != null || ((((parentComponent != null) ? parentComponent.def : null) == null || parentComponent.def.panel_background <= 0f) && this.District != null && !this.District.IsCommon() && !this.District.IsPF() && this.state_def.GetBool("show_district_type", null, false, true, true, true, '.'));
		this.m_DistrictType.gameObject.SetActive(flag);
		if (flag)
		{
			this.m_DistrictType.overrideSprite = ((field != null) ? global::Defs.GetObj<Sprite>(field, "backgroud", null) : null);
		}
		Color color = (this.state_def != null) ? global::Defs.GetColor(this.state_def, "district_type_color", null) : Color.white;
		this.m_DistrictType.color = color;
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x001654E4 File Offset: 0x001636E4
	private void UpdateIcon()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_Icon != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.state_def, "icon", this.Kingdom);
			if (obj != null)
			{
				this.m_Icon.overrideSprite = obj;
			}
			else if (this.Def != null)
			{
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.Def.field, "icon", this.Kingdom);
			}
			else
			{
				this.m_Icon.overrideSprite = null;
			}
			this.m_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		if (this.m_OverGlow != null)
		{
			this.m_OverGlow.sprite = global::Defs.GetObj<Sprite>(this.state_def, "hover_sprite", null);
			this.m_OverGlow.color = global::Defs.GetColor(this.state_def, "hover_color", null);
		}
		if (this.m_EdgeLight != null)
		{
			bool @bool = this.state_def.GetBool("show_edge_light", null, false, true, true, true, '.');
			this.m_EdgeLight.gameObject.SetActive(@bool);
			if (@bool)
			{
				this.m_EdgeLight.overrideSprite = global::Defs.GetObj<Sprite>(this.state_def, "edge_sprite", null);
				this.m_EdgeLight.color = global::Defs.GetColor(this.state_def, "edge_color", null);
			}
		}
		if (this.m_FocusBorder != null)
		{
			District.Def district = this.District;
			DT.Field field = (district != null) ? district.field : null;
			this.m_DistrictBorder.overrideSprite = ((field != null) ? global::Defs.GetObj<Sprite>(field, "focus_border", null) : null);
		}
		if (this.m_StaticBorder != null)
		{
			this.m_StaticBorder.gameObject.SetActive(!this.state_def.GetBool("show_border", null, false, true, true, true, '.'));
		}
		if (this.m_DistrictBorder != null)
		{
			this.m_DistrictBorder.gameObject.SetActive(this.state_def.GetBool("show_border", null, false, true, true, true, '.'));
			this.m_DistrictBorder.color = global::Defs.GetColor(this.state_def, "border_color", null);
			if (this.state_def.GetBool("show_district_type", null, false, true, true, true, '.'))
			{
				District.Def district2 = this.District;
				DT.Field field2 = (district2 != null) ? district2.field : null;
				this.m_DistrictBorder.overrideSprite = ((field2 != null) ? global::Defs.GetObj<Sprite>(field2, "border", null) : null);
			}
			else
			{
				this.m_DistrictBorder.overrideSprite = null;
			}
		}
		if (this.m_CompleteBorder != null)
		{
			this.m_CompleteBorder.gameObject.SetActive(this.state == UIBuildingSlot.State.WorkingComplated);
		}
		if (this.m_InnerFrame)
		{
			bool bool2 = this.state_def.GetBool("show_inner_frame", null, false, true, true, true, '.');
			this.m_InnerFrame.gameObject.SetActive(bool2);
			this.m_InnerFrame.color = global::Defs.GetColor(this.state_def, "inner_frame_color", null);
		}
		if (this.m_Background_Unavailabale)
		{
			this.m_Background_Unavailabale.SetActive(this.state_def.GetBool("show_unavailabale", null, false, true, true, true, '.'));
		}
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x00165818 File Offset: 0x00163A18
	private void UpdateBuildProgress()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_BuildProgress == null)
		{
			return;
		}
		bool @bool = this.state_def.GetBool("show_build_progress", null, false, true, true, true, '.');
		bool flag = this.IsUnderConstruction();
		if (@bool)
		{
			this.m_BuildProgress.Show(true);
		}
		else
		{
			this.m_BuildProgress.Hide(true);
		}
		this.m_isCurrentlyBuilding = (this.m_isCurrentlyBuilding || flag);
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x00165888 File Offset: 0x00163A88
	private void UpdateHostedBuildingProgress()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_HostedBuilding == null)
		{
			return;
		}
		bool flag = this.state_def.GetBool("show_hosted_building", null, false, true, true, true, '.');
		bool flag2 = false;
		flag = (flag && flag2);
		this.m_HostedBuilding.gameObject.SetActive(flag);
		if (flag)
		{
			BSGProgressBar hostedBuildingProgress = this.m_HostedBuildingProgress;
			if (hostedBuildingProgress != null)
			{
				hostedBuildingProgress.Show(true);
			}
		}
		else
		{
			BSGProgressBar hostedBuildingProgress2 = this.m_HostedBuildingProgress;
			if (hostedBuildingProgress2 != null)
			{
				hostedBuildingProgress2.Hide(true);
			}
		}
		this.m_isCurrentlyBuilding = (this.m_isCurrentlyBuilding || flag2);
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x00165914 File Offset: 0x00163B14
	private void UpdateRepairProgress()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_RepairProgress == null)
		{
			return;
		}
		bool @bool = this.state_def.GetBool("show_repair_progress", null, false, true, true, true, '.');
		bool flag = this.IsUnderRepair();
		if (@bool)
		{
			this.m_RepairProgress.Show(true);
		}
		else
		{
			this.m_RepairProgress.Hide(true);
		}
		this.m_isCurrentlyBuilding = (this.m_isCurrentlyBuilding || flag);
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x00165981 File Offset: 0x00163B81
	private void UpdateBuildingCountPorgress()
	{
		if (this.m_DistrictProgress == null)
		{
			return;
		}
		this.m_DistrictProgress.gameObject.SetActive(false);
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x001659A4 File Offset: 0x00163BA4
	private void UpdateDefName()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.m_DefKey == null)
		{
			return;
		}
		bool @bool = this.state_def.GetBool("show_def_name", null, false, true, true, true, '.');
		this.m_DefKey.gameObject.SetActive(@bool);
		TMP_Text defKey = this.m_DefKey;
		Building.Def def = this.Def;
		defKey.text = (((def != null) ? def.id : null) ?? "");
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x00165A18 File Offset: 0x00163C18
	private void UpdateBuildingName()
	{
		if (this.m_BuildingName != null)
		{
			if (Application.isPlaying)
			{
				if (this.Def != null)
				{
					UIText.SetText(this.m_BuildingName, (this.Def != null) ? (global::Defs.Localize(this.Def.field, "name", null, null, true, true) ?? "") : "");
				}
				this.m_BuildingName.color = global::Defs.GetColor(this.state_def, "label_color", null);
				return;
			}
			UIText.SetText(this.m_BuildingName, this.state.ToString());
			this.m_BuildingName.color = global::Defs.GetColor(this.state_def, "label_color", null);
		}
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x00165ADC File Offset: 0x00163CDC
	private void UpdateUpgradeSlots()
	{
		if (this.m_UpgardeIconsContainer == null)
		{
			return;
		}
		if (!this.m_AllowUpgardeIcons || (this.state == UIBuildingSlot.State.WorkingComplated && this.Def.hide_upgrades_if_completed))
		{
			RectTransform upgardeIconsContainer = this.m_UpgardeIconsContainer;
			if (upgardeIconsContainer != null)
			{
				upgardeIconsContainer.gameObject.SetActive(false);
			}
			if (this.m_UpgardeDotsContainer != null)
			{
				this.m_UpgardeDotsContainer.gameObject.SetActive(false);
			}
			return;
		}
		if (this.m_UseUpgardeSlotIcon)
		{
			this.m_UpgardeIconsContainer.gameObject.SetActive(true);
			if (this.m_UpgardeDotsContainer != null)
			{
				this.m_UpgardeDotsContainer.gameObject.SetActive(false);
			}
			Building.Def def = this.Def;
			List<District.Def.BuildingInfo> list;
			if (def == null)
			{
				list = null;
			}
			else
			{
				District.Def upgrades = def.upgrades;
				list = ((upgrades != null) ? upgrades.buildings : null);
			}
			List<District.Def.BuildingInfo> list2 = list;
			int num = Mathf.Max((list2 != null) ? list2.Count : 0, this.m_AllicatedUpgardeSlots.Count);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				District.Def.BuildingInfo buildingInfo = (list2 != null && list2.Count > i) ? list2[i] : null;
				if (this.Kingdom != null && buildingInfo != null && !this.Kingdom.CheckReligionRequirements(buildingInfo.def))
				{
					buildingInfo = null;
				}
				if (buildingInfo == null)
				{
					if (this.m_AllicatedUpgardeSlots.Count > i)
					{
						this.m_AllicatedUpgardeSlots[i].SetDef(this.Kingdom, this.Castle, (buildingInfo != null) ? buildingInfo.def : null, null);
						this.m_AllicatedUpgardeSlots[i].gameObject.SetActive(buildingInfo != null);
						num2++;
					}
				}
				else
				{
					if (this.m_AllicatedUpgardeSlots.Count <= num2)
					{
						UIBuildingSlot uibuildingSlot = UIBuildingSlot.Create(buildingInfo.def, this.Kingdom, this.Castle, null, this.m_UpgardeIconsContainer, "small");
						uibuildingSlot.OnSelected = new Action<UIBuildingSlot, PointerEventData>(this.HandleUpgardeSlotSelected);
						this.m_AllicatedUpgardeSlots.Add(uibuildingSlot);
					}
					else
					{
						this.m_AllicatedUpgardeSlots[num2].SetDef(this.Kingdom, this.Castle, (buildingInfo != null) ? buildingInfo.def : null, null);
					}
					this.m_AllicatedUpgardeSlots[num2].gameObject.SetActive(buildingInfo != null);
					num2++;
				}
			}
			return;
		}
		if (this.m_UpgardeDotsContainer != null)
		{
			this.m_UpgardeDotsContainer.gameObject.SetActive(true);
			RectTransform upgardeIconsContainer2 = this.m_UpgardeIconsContainer;
			if (upgardeIconsContainer2 == null)
			{
				return;
			}
			upgardeIconsContainer2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x00165D50 File Offset: 0x00163F50
	private void HandleUpgardeSlotSelected(UIBuildingSlot slot, PointerEventData e)
	{
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x00165D68 File Offset: 0x00163F68
	private void UpdateReligionIncompatibility()
	{
		if (this.m_ReligionIncompatibility == null)
		{
			return;
		}
		if (this.Inst == null || this.Kingdom == null)
		{
			this.m_ReligionIncompatibility.gameObject.SetActive(false);
			return;
		}
		if (!this.Kingdom.CheckReligionRequirements(this.Inst.def))
		{
			string reqiredReligionName = this.GetReqiredReligionName(this.Inst.def, this.Kingdom);
			Sprite obj = global::Defs.GetObj<Sprite>("ResourceIconSettings", global::Religions.GetRelgionIconKey(reqiredReligionName, true), null);
			if (obj != null)
			{
				this.m_ReligionIncompatibilityIcon.sprite = obj;
			}
			this.m_ReligionIncompatibility.gameObject.SetActive(obj != null);
			return;
		}
		this.m_ReligionIncompatibility.gameObject.SetActive(false);
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x00165E28 File Offset: 0x00164028
	private string GetReqiredReligionName(Building.Def def, Logic.Kingdom k)
	{
		List<string> list = new List<string>();
		this.GetReligionFromRequirements(def, k, list);
		List<ValueTuple<string, int>> list2 = new List<ValueTuple<string, int>>();
		if (this.Castle != null && list.Count > 0)
		{
			Religion religion = this.Castle.GetRealm().religion;
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				if (text == religion.name)
				{
					list2.Add(new ValueTuple<string, int>(text, 0));
					break;
				}
				if (text == "Christian" && religion.def.christian)
				{
					list2.Add(new ValueTuple<string, int>(religion.def.name, 1));
				}
				else if (text == "Muslim" && religion.def.muslim)
				{
					list2.Add(new ValueTuple<string, int>(religion.def.name, 1));
				}
			}
		}
		list2.Sort((ValueTuple<string, int> x, ValueTuple<string, int> y) => x.Item2.CompareTo(y.Item2));
		if (list2 != null && list2.Count > 0)
		{
			return list2[0].Item1;
		}
		if (list == null || list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x00165F6C File Offset: 0x0016416C
	private void GetReligionFromRequirements(Building.Def def, Logic.Kingdom k, List<string> religionDef)
	{
		UIBuildingSlot.<>c__DisplayClass200_0 CS$<>8__locals1;
		CS$<>8__locals1.k = k;
		if (religionDef == null)
		{
			religionDef = new List<string>();
		}
		UIBuildingSlot.<GetReligionFromRequirements>g__GetReligion|200_0(def.requires, religionDef, ref CS$<>8__locals1);
		UIBuildingSlot.<GetReligionFromRequirements>g__GetReligion|200_0(def.requires_or, religionDef, ref CS$<>8__locals1);
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x00165FA8 File Offset: 0x001641A8
	public static bool WillStall(Building.Def def, Logic.Kingdom k)
	{
		if (((def != null) ? def.produces : null) == null)
		{
			return false;
		}
		if (k == null)
		{
			return false;
		}
		for (int i = 0; i < k.realms.Count; i++)
		{
			Logic.Realm realm = k.realms[i];
			if (UIBuildingSlot.WillStall(def, realm.castle))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x00166000 File Offset: 0x00164200
	public static bool WillStall(Building.Def def, Castle c)
	{
		if (((c != null) ? c.buildings : null) == null)
		{
			return false;
		}
		for (int i = 0; i < c.buildings.Count; i++)
		{
			Building b = c.buildings[i];
			if (UIBuildingSlot.WillStall(def, b))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x0016604C File Offset: 0x0016424C
	public static bool WillStall(Building.Def def, Building b)
	{
		if (((b != null) ? b.def : null) == null)
		{
			return false;
		}
		if (!b.IsBuilt())
		{
			return false;
		}
		if (b.def.requires != null)
		{
			for (int i = 0; i < b.def.requires.Count; i++)
			{
				Building.Def.RequirementInfo requirementInfo = b.def.requires[i];
				if (def.Produces(requirementInfo.key) || def.ProducesCompleted(requirementInfo.key))
				{
					return true;
				}
			}
		}
		District.Def upgrades = b.def.upgrades;
		if (((upgrades != null) ? upgrades.buildings : null) != null)
		{
			for (int j = 0; j < b.def.upgrades.buildings.Count; j++)
			{
				Building.Def def2 = b.def.upgrades.buildings[j].def;
				Castle castle = b.castle;
				Building building = (castle != null) ? castle.FindBuilding(def2) : null;
				if (building != null && UIBuildingSlot.WillStall(def, building))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x00166148 File Offset: 0x00164348
	private void HandleRemoveButton(BSGButton btn)
	{
		if (!this.m_AllowRemove)
		{
			return;
		}
		if (this.Castle.GetRealm().IsDisorder())
		{
			return;
		}
		if (this.OnRemove != null)
		{
			this.OnRemove(this);
			return;
		}
		if (this.Inst != null || this.Def != null)
		{
			if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
			{
				UIBuildingSlot.RemoveBuilding(this.Def, this.Inst, this.Castle);
				return;
			}
			this.m_RemoveBuiildingConfirmation = UIBuildingSlot.HandleRemoveBuilding(this.Def, this.Inst, this.Castle);
		}
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x001661E8 File Offset: 0x001643E8
	public static void RemoveBuilding(Building.Def Def, Building Inst, Castle Castle)
	{
		if (Castle == null)
		{
			return;
		}
		if (Castle.GetRealm().IsDisorder())
		{
			return;
		}
		Building building = Inst ?? Castle.FindBuilding(Def);
		Castle.RemoveBuilding(building, Castle.BuildingRemovalMode.Regular, true);
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x00166220 File Offset: 0x00164420
	public static MessageWnd HandleRemoveBuilding(Building.Def Def, Building Inst, Castle Castle)
	{
		if (Inst != null && Inst.state == Building.State.Planned)
		{
			UIBuildingSlot.RemoveBuilding(Def, Inst, Castle);
			return null;
		}
		Vars vars = new Vars(Def);
		Vars vars2 = vars;
		string key = "kingdom";
		Castle castle = Castle;
		vars2.Set<Logic.Kingdom>(key, (castle != null) ? castle.GetKingdom() : null);
		if (Def.ProducesResources())
		{
			Castle castle2 = Castle;
			Logic.Kingdom kingdom = (castle2 != null) ? castle2.GetKingdom() : null;
			if (kingdom != null && kingdom.GetRealmTag(Def.id) == 1)
			{
				string producesText = UIBuildingSlot.GetProducesText(Def.produces, Def, kingdom, Castle, Inst);
				vars.Set<string>("last_produces_text", producesText);
				if (UIBuildingSlot.WillStall(Def, kingdom))
				{
					vars.Set<bool>("will_stall", true);
				}
			}
		}
		return MessageWnd.Create("ConfirmRemoveBuilding", vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "confirm")
			{
				UIBuildingSlot.RemoveBuilding(Def, Inst, Castle);
			}
			wnd.Close(false);
			return true;
		});
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x00166340 File Offset: 0x00164540
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		this.UpdateHighlight();
		if (this.Def == null || this.Castle == null)
		{
			if (this.OnSelected != null)
			{
				this.OnSelected(this, e);
			}
			return;
		}
		if (this.state == UIBuildingSlot.State.Locked)
		{
			return;
		}
		int slot_idx = UICastleBuildWindow.TargetSlotIndex();
		if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
		{
			if (this.Def.IsUpgrade())
			{
				if (this.Kingdom != null && this.Kingdom.HasPlannedUpgrade(this.Def))
				{
					this.Kingdom.RemovePlanedUpgrade(this.Def);
					return;
				}
			}
			else if (this.Inst != null && this.Inst.IsPlanned())
			{
				this.Castle.RemoveBuilding(this.Inst, Castle.BuildingRemovalMode.Regular, true);
				return;
			}
			this.Castle.PlanBuilding(this.Def, slot_idx, UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false));
			return;
		}
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheatmode building select", true))
		{
			if (this.IsUnderConstruction())
			{
				this.Castle.structure_build.CancelBuild(true, true);
			}
			this.Castle.BuildBuilding(this.Def, slot_idx, true);
			return;
		}
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x001664A0 File Offset: 0x001646A0
	private void TryUnlockSlots()
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

	// Token: 0x06002A23 RID: 10787 RVA: 0x00166531 File Offset: 0x00164731
	private bool Expand(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.Castle.ExpandCity();
		}
		wnd.Close(false);
		return true;
	}

	// Token: 0x06002A24 RID: 10788 RVA: 0x00166554 File Offset: 0x00164754
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (this.OnGainFocus != null)
		{
			this.OnGainFocus(this, eventData);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002A25 RID: 10789 RVA: 0x00166578 File Offset: 0x00164778
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (this.OnLooseFocus != null)
		{
			this.OnLooseFocus(this, eventData);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x0016659C File Offset: 0x0016479C
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		string text = this.m_CurRelationHighlight;
		if (!this.ShowRelationHighlight || string.IsNullOrEmpty(text) || text == "will_be_destroyed")
		{
			text = "none";
		}
		if (!this.ShowSelection && (text == "selected" || text == "this"))
		{
			text = "none";
		}
		if (this.m_RelationHighlight != null)
		{
			this.m_RelationHighlight.gameObject.SetActive(text != "none");
		}
		if (this.m_OverGlow != null)
		{
			this.m_OverGlow.gameObject.SetActive(this.mouse_in);
		}
		if (this.m_FocusBorder != null)
		{
			this.m_FocusBorder.gameObject.SetActive(this.mouse_in);
		}
		UIBuildingSlot.Settings.Refresh();
		if (this.m_RelationHighlightImage != null)
		{
			Color color = UIBuildingSlot.Settings.RelationColor(text);
			this.m_RelationHighlightImage.color = color;
		}
		if (this.m_Selection != null)
		{
			this.m_Selection.gameObject.SetActive(this.m_Selected && this.ShowSelection);
		}
		this.UpdateInteractions();
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x001666DC File Offset: 0x001648DC
	private string GetRelation(BaseUI ui)
	{
		MessageWnd window = ui.window_dispatcher.GetWindow<MessageWnd>();
		if (window != null)
		{
			DT.Field def_field = window.def_field;
			if (((def_field != null) ? def_field.key : null) == "ConfirmRemoveBuilding")
			{
				Building.Def def = window.vars.obj.obj_val as Building.Def;
				if (this.Inst != null && this.Def.CalcRequires((def != null) ? def.id : null, this.Castle, true) > 0)
				{
					return "will_be_destroyed";
				}
				return "none";
			}
		}
		if (ui.picked_hotspot == null)
		{
			return "none";
		}
		if (ui.picked_hotspot == this)
		{
			return "this";
		}
		if (this.Def == null)
		{
			return "none";
		}
		string text = ui.picked_hotspot.GetVar("def_id", null, true).String(null);
		if (string.IsNullOrEmpty(text))
		{
			return "none";
		}
		BSGButton picked_button = ui.picked_button;
		if (((picked_button != null) ? picked_button.name : null) == "id_RemoveBuilding")
		{
			if (this.Inst != null && this.Def.CalcRequires(text, this.Castle, true) > 0)
			{
				return "will_be_destroyed";
			}
			return "none";
		}
		else
		{
			string relation = this.Def.GetRelation(this.game, text, this.Castle);
			if (relation == null)
			{
				return "none";
			}
			return relation;
		}
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x00166833 File Offset: 0x00164A33
	public static Color GetBackgrounColor(District.Def ddef)
	{
		return global::Defs.GetColor((ddef != null) ? ddef.field : null, "color", Color.clear, null);
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x00166851 File Offset: 0x00164A51
	public Color GetBackgrounColor()
	{
		return UIBuildingSlot.GetBackgrounColor(this.District);
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x00166860 File Offset: 0x00164A60
	private void OnUIPickerChanged(BaseUI ui)
	{
		string text = this.GetRelation(ui);
		if (text == this.m_CurRelationHighlight)
		{
			return;
		}
		bool flag = text == "will_be_destroyed" || this.m_CurRelationHighlight == "will_be_destroyed";
		if (text == "this" && this.mouse_in)
		{
			text = "none";
		}
		this.m_CurRelationHighlight = text;
		if (flag)
		{
			this.UpdateState();
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x001668D4 File Offset: 0x00164AD4
	private void OnActiveWndChanged(BaseUI ui, UIWindow prev)
	{
		this.OnUIPickerChanged(ui);
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x001668E0 File Offset: 0x00164AE0
	public void OnMessage(object obj, string message, object param)
	{
		if (this == null || base.gameObject == null)
		{
			this.SetCastle(null, false);
			this.SetKingdom(null);
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1248572439U)
		{
			if (num != 24823524U)
			{
				if (num != 434548633U)
				{
					if (num != 1248572439U)
					{
						return;
					}
					if (!(message == "build_canceled"))
					{
						return;
					}
				}
				else if (!(message == "build_started"))
				{
					return;
				}
			}
			else if (!(message == "structures_changed"))
			{
				return;
			}
		}
		else if (num <= 2287476291U)
		{
			if (num != 1904481269U)
			{
				if (num != 2287476291U)
				{
					return;
				}
				if (!(message == "upgrade_canceled"))
				{
					return;
				}
			}
			else if (!(message == "upgrade_started"))
			{
				return;
			}
		}
		else if (num != 2512362562U)
		{
			if (num != 3127271985U)
			{
				return;
			}
			if (!(message == "planed_upgarde_changed"))
			{
				return;
			}
		}
		else if (!(message == "build_finished"))
		{
			return;
		}
		if (base.gameObject.activeInHierarchy)
		{
			this.UpdateState();
			this.SetupTooltip();
			return;
		}
		this.m_RefreshOnEnable = true;
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x001669E8 File Offset: 0x00164BE8
	protected override void OnEnable()
	{
		base.OnEnable();
		BaseUI.on_picked_element_changed = (BaseUI.OnPickedElementChanged)Delegate.Combine(BaseUI.on_picked_element_changed, new BaseUI.OnPickedElementChanged(this.OnUIPickerChanged));
		UIWindowDispatcher.on_active_wnd_changed = (UIWindowDispatcher.OnActiveWndChanged)Delegate.Combine(UIWindowDispatcher.on_active_wnd_changed, new UIWindowDispatcher.OnActiveWndChanged(this.OnActiveWndChanged));
		if (this.m_RefreshOnEnable)
		{
			this.Refresh();
			this.m_RefreshOnEnable = false;
		}
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x00166A50 File Offset: 0x00164C50
	protected override void OnDisable()
	{
		base.OnDisable();
		BaseUI.on_picked_element_changed = (BaseUI.OnPickedElementChanged)Delegate.Remove(BaseUI.on_picked_element_changed, new BaseUI.OnPickedElementChanged(this.OnUIPickerChanged));
		UIWindowDispatcher.on_active_wnd_changed = (UIWindowDispatcher.OnActiveWndChanged)Delegate.Remove(UIWindowDispatcher.on_active_wnd_changed, new UIWindowDispatcher.OnActiveWndChanged(this.OnActiveWndChanged));
		if (this.m_RemoveBuiildingConfirmation != null)
		{
			this.m_RemoveBuiildingConfirmation.Close(false);
		}
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x00166AC0 File Offset: 0x00164CC0
	public void OnDestroy()
	{
		this.OnSelected = null;
		this.OnGainFocus = null;
		this.OnLooseFocus = null;
		this.OnRemove = null;
		this.m_Locked = false;
		this.Def = null;
		this.Inst = null;
		this.state = UIBuildingSlot.State.NotInitialzied;
		this.m_AllowUpgardeIcons = true;
		this.m_AllowNamePlates = true;
		this.m_AllowRemove = true;
		this.SetCastle(null, false);
		this.SetKingdom(null);
		if (this.m_RemoveBuiildingConfirmation != null)
		{
			this.m_RemoveBuiildingConfirmation.Close(false);
		}
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x00166B44 File Offset: 0x00164D44
	public override string ToString()
	{
		return string.Format(string.Format("UIBuildingSlot({0}) Def: {1}, Inst: {2}, Castle: {3}", new object[]
		{
			this.SlotIndex,
			this.Def,
			this.Inst,
			this.Castle
		}), Array.Empty<object>());
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x00166B94 File Offset: 0x00164D94
	public void SetSlotIdx(int slot_idx)
	{
		this.SlotIndex = slot_idx;
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x00166BA0 File Offset: 0x00164DA0
	public static UIBuildingSlot Create(Building.Def def, Logic.Kingdom kingdom, Castle castle, District.Def district, RectTransform parent, string variant = null)
	{
		if (parent == null)
		{
			return null;
		}
		string componentId = "BuildingSlot";
		if (def.IsUpgrade())
		{
			componentId = "UpgradeSlot";
		}
		GameObject prefab = UICommon.GetPrefab(componentId, variant);
		if (!(prefab != null))
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prefab, parent, false, "");
		UIBuildingSlot component = gameObject.GetComponent<UIBuildingSlot>();
		if (component != null)
		{
			gameObject.name = def.ToString();
			component.SlotIndex = -1;
			component.SetDef(kingdom, castle, def, district);
			BaseUI baseUI = BaseUI.Get();
			if (baseUI != null)
			{
				component.OnUIPickerChanged(baseUI);
			}
			return component;
		}
		UnityEngine.Object.Destroy(gameObject);
		return null;
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x00166C40 File Offset: 0x00164E40
	internal void SetOfflineState(UIBuildingSlot.State new_state, bool force = false, bool ignoreDirty = false)
	{
		DT.Field defField = global::Defs.GetDefField(this.slot_type.ToString(), null);
		if (defField == null)
		{
			Debug.Log("Fail to load BuildingSlot def");
			return;
		}
		if (this.state == new_state && !force)
		{
			return;
		}
		this.state = new_state;
		this.preview_state = new_state;
		if (defField != null)
		{
			this.state_def = defField.FindChild(this.state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				this.state_def = defField.FindChild("State", null, true, true, true, '.');
				if (this.state_def == null)
				{
					Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, this.state));
				}
			}
		}
		else
		{
			this.state_def = null;
		}
		this.UpdateOfflineState();
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x00166D08 File Offset: 0x00164F08
	internal void UpdateOfflineState()
	{
		this.Init();
		this.UpdateIcon();
		this.UpdateDistrictColor();
		this.UpdateStatusIcon();
		this.UpdateBuildProgress();
		this.UpdateRepairProgress();
		this.UpdateBuildingName();
		this.UpdateInteractions();
		this.UpdateNamePlate();
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x00166D40 File Offset: 0x00164F40
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x00166DC0 File Offset: 0x00164FC0
	[CompilerGenerated]
	internal static void <GetReligionFromRequirements>g__GetReligion|200_0(List<Building.Def.RequirementInfo> reqs, List<string> list, ref UIBuildingSlot.<>c__DisplayClass200_0 A_2)
	{
		if (reqs == null)
		{
			return;
		}
		for (int i = 0; i < reqs.Count; i++)
		{
			Building.Def.RequirementInfo requirementInfo = reqs[i];
			if (!(((requirementInfo != null) ? requirementInfo.type : null) != "Religion"))
			{
				Religion.Def def = A_2.k.game.defs.Find<Religion.Def>(requirementInfo.key);
				if (def != null)
				{
					if (!list.Contains(def.name))
					{
						list.Add(def.name);
					}
				}
				else
				{
					DT.Field defField = global::Defs.GetDefField(requirementInfo.key, null);
					if (defField != null)
					{
						if (!list.Contains(defField.key))
						{
							list.Add(defField.key);
						}
					}
					else if ((requirementInfo.key == "Muslim" || requirementInfo.key == "Christian") && !list.Contains(requirementInfo.key))
					{
						list.Add(requirementInfo.key);
					}
				}
			}
		}
	}

	// Token: 0x04001C48 RID: 7240
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001C49 RID: 7241
	[UIFieldTarget("id_DistrictType")]
	private Image m_DistrictType;

	// Token: 0x04001C4A RID: 7242
	[UIFieldTarget("id_DistrictBorder")]
	private Image m_DistrictBorder;

	// Token: 0x04001C4B RID: 7243
	[UIFieldTarget("id_StaticBorder")]
	private Image m_StaticBorder;

	// Token: 0x04001C4C RID: 7244
	[UIFieldTarget("id_FocusBorder")]
	private Image m_FocusBorder;

	// Token: 0x04001C4D RID: 7245
	[UIFieldTarget("id_CompleteBorder")]
	private Image m_CompleteBorder;

	// Token: 0x04001C4E RID: 7246
	[UIFieldTarget("id_DefKey")]
	private TextMeshProUGUI m_DefKey;

	// Token: 0x04001C4F RID: 7247
	[UIFieldTarget("id_RemoveBuilding")]
	private BSGButton Button_RemoveStructure;

	// Token: 0x04001C50 RID: 7248
	[UIFieldTarget("id_BuildingLevelLabel")]
	private TextMeshProUGUI m_BuildingLevelLabel;

	// Token: 0x04001C51 RID: 7249
	[UIFieldTarget("id_RelationHighlight")]
	private GameObject m_RelationHighlight;

	// Token: 0x04001C52 RID: 7250
	[UIFieldTarget("id_RelationHighlightImage")]
	private Image m_RelationHighlightImage;

	// Token: 0x04001C53 RID: 7251
	[UIFieldTarget("id_BuildProgress")]
	private BSGProgressBar m_BuildProgress;

	// Token: 0x04001C54 RID: 7252
	[UIFieldTarget("id_HostedBuilding")]
	private GameObject m_HostedBuilding;

	// Token: 0x04001C55 RID: 7253
	[UIFieldTarget("id_HostedBuildingProgress")]
	private BSGProgressBar m_HostedBuildingProgress;

	// Token: 0x04001C56 RID: 7254
	[UIFieldTarget("id_RepairProgress")]
	private BSGProgressBar m_RepairProgress;

	// Token: 0x04001C57 RID: 7255
	[UIFieldTarget("id_CancelBuild")]
	private BSGButton m_CancelBuild;

	// Token: 0x04001C58 RID: 7256
	[UIFieldTarget("id_Status")]
	private GameObject m_Status;

	// Token: 0x04001C59 RID: 7257
	[UIFieldTarget("id_StatusIcon")]
	private Image m_StatusIcon;

	// Token: 0x04001C5A RID: 7258
	[UIFieldTarget("id_StatusIconBorder")]
	private Image m_StatusIconBorder;

	// Token: 0x04001C5B RID: 7259
	[UIFieldTarget("id_StatusBackground")]
	private Image m_StatusBackground;

	// Token: 0x04001C5C RID: 7260
	[UIFieldTarget("id_BuildingsCount")]
	private GameObject m_BuildingsCount;

	// Token: 0x04001C5D RID: 7261
	[UIFieldTarget("id_BuildingsCountValue")]
	private TextMeshProUGUI m_BuildingsCountValue;

	// Token: 0x04001C5E RID: 7262
	[UIFieldTarget("id_BuildingName")]
	private TextMeshProUGUI m_BuildingName;

	// Token: 0x04001C5F RID: 7263
	[UIFieldTarget("id_OverGlow")]
	private Image m_OverGlow;

	// Token: 0x04001C60 RID: 7264
	[UIFieldTarget("id_EdgeLight")]
	private Image m_EdgeLight;

	// Token: 0x04001C61 RID: 7265
	[UIFieldTarget("id_InnerFrame")]
	private Image m_InnerFrame;

	// Token: 0x04001C62 RID: 7266
	[UIFieldTarget("id_Selection")]
	private Image m_Selection;

	// Token: 0x04001C63 RID: 7267
	[UIFieldTarget("id_DistrictProgress")]
	private GameObject m_DistrictProgress;

	// Token: 0x04001C64 RID: 7268
	[UIFieldTarget("id_BuildingSlotCountProtootype")]
	private GameObject m_BuildingSlotCountProtootype;

	// Token: 0x04001C65 RID: 7269
	[UIFieldTarget("id_DistrictMaxedBorder")]
	private GameObject m_DistrictMaxedBorder;

	// Token: 0x04001C66 RID: 7270
	[UIFieldTarget("id_BuildingNameContainer")]
	private GameObject m_BuildingNameContainer;

	// Token: 0x04001C67 RID: 7271
	[UIFieldTarget("id_Background_Unavailabale")]
	private GameObject m_Background_Unavailabale;

	// Token: 0x04001C68 RID: 7272
	[UIFieldTarget("id_UpgardeIconsContainer")]
	private RectTransform m_UpgardeIconsContainer;

	// Token: 0x04001C69 RID: 7273
	[UIFieldTarget("id_UpgardeDotsContainer")]
	private RectTransform m_UpgardeDotsContainer;

	// Token: 0x04001C6A RID: 7274
	[UIFieldTarget("id_ReligionIncompatibility")]
	private RectTransform m_ReligionIncompatibility;

	// Token: 0x04001C6B RID: 7275
	[UIFieldTarget("id_ReligionIncompatibilityIcon")]
	private Image m_ReligionIncompatibilityIcon;

	// Token: 0x04001C6C RID: 7276
	private bool m_RefreshOnEnable;

	// Token: 0x04001C6D RID: 7277
	public static readonly UIBuildingSlot.BuildWindowSettigs Settings = new UIBuildingSlot.BuildWindowSettigs();

	// Token: 0x04001C75 RID: 7285
	public int SlotIndex;

	// Token: 0x04001C76 RID: 7286
	public bool ShowSelection = true;

	// Token: 0x04001C77 RID: 7287
	public bool ShowRelationHighlight = true;

	// Token: 0x04001C7C RID: 7292
	private bool m_Selected;

	// Token: 0x04001C7D RID: 7293
	private bool m_AllowRemove = true;

	// Token: 0x04001C7E RID: 7294
	private bool m_AllowNamePlates = true;

	// Token: 0x04001C7F RID: 7295
	private bool m_AllowUpgardeIcons = true;

	// Token: 0x04001C80 RID: 7296
	private bool m_Initialzed;

	// Token: 0x04001C81 RID: 7297
	private List<GameObject> m_DistrictPorgressSlots = new List<GameObject>();

	// Token: 0x04001C82 RID: 7298
	private bool m_ShowDistrictProgress;

	// Token: 0x04001C85 RID: 7301
	public UIBuildingSlot.SlotType slot_type;

	// Token: 0x04001C86 RID: 7302
	public bool m_Locked;

	// Token: 0x04001C87 RID: 7303
	public string m_CurRelationHighlight = "none";

	// Token: 0x04001C88 RID: 7304
	private bool m_isCurrentlyBuilding;

	// Token: 0x04001C89 RID: 7305
	private Vars tooltipVars;

	// Token: 0x04001C8A RID: 7306
	private MessageWnd m_RemoveBuiildingConfirmation;

	// Token: 0x04001C8B RID: 7307
	[HideInInspector]
	[SerializeField]
	internal UIBuildingSlot.State preview_state;

	// Token: 0x04001C8C RID: 7308
	private static List<UIBuildingSlot.BonusText> tmp_bonus_texts = new List<UIBuildingSlot.BonusText>(16);

	// Token: 0x04001C8D RID: 7309
	private bool m_UseUpgardeSlotIcon = true;

	// Token: 0x04001C8E RID: 7310
	private List<UIBuildingSlot> m_AllicatedUpgardeSlots = new List<UIBuildingSlot>();

	// Token: 0x020007FE RID: 2046
	public enum SlotType
	{
		// Token: 0x04003D39 RID: 15673
		BuildingSlot,
		// Token: 0x04003D3A RID: 15674
		UpgradeSlot
	}

	// Token: 0x020007FF RID: 2047
	public enum State
	{
		// Token: 0x04003D3C RID: 15676
		NotInitialzied,
		// Token: 0x04003D3D RID: 15677
		Invalid,
		// Token: 0x04003D3E RID: 15678
		Empty,
		// Token: 0x04003D3F RID: 15679
		EmptyOccupied,
		// Token: 0x04003D40 RID: 15680
		Unbuildabe,
		// Token: 0x04003D41 RID: 15681
		CanNotBuildPrerequisites,
		// Token: 0x04003D42 RID: 15682
		CanNotBuildPrerequisitesGoods,
		// Token: 0x04003D43 RID: 15683
		CanNotBuild,
		// Token: 0x04003D44 RID: 15684
		CanNotBuildCost,
		// Token: 0x04003D45 RID: 15685
		CanNotBuildSlots,
		// Token: 0x04003D46 RID: 15686
		CanBuild,
		// Token: 0x04003D47 RID: 15687
		Planned,
		// Token: 0x04003D48 RID: 15688
		PlannedCanBuild,
		// Token: 0x04003D49 RID: 15689
		Building,
		// Token: 0x04003D4A RID: 15690
		BuildingOccupied,
		// Token: 0x04003D4B RID: 15691
		UpgradingElsewhere,
		// Token: 0x04003D4C RID: 15692
		Damaged,
		// Token: 0x04003D4D RID: 15693
		Repairing,
		// Token: 0x04003D4E RID: 15694
		Abandoned,
		// Token: 0x04003D4F RID: 15695
		Stalled,
		// Token: 0x04003D50 RID: 15696
		StalledOccupied,
		// Token: 0x04003D51 RID: 15697
		Working,
		// Token: 0x04003D52 RID: 15698
		WorkingComplated,
		// Token: 0x04003D53 RID: 15699
		WillBeDestroyed,
		// Token: 0x04003D54 RID: 15700
		Locked
	}

	// Token: 0x02000800 RID: 2048
	private struct BonusText
	{
		// Token: 0x04003D55 RID: 15701
		public int line_number;

		// Token: 0x04003D56 RID: 15702
		public string text;
	}

	// Token: 0x02000801 RID: 2049
	public class BuildWindowSettigs
	{
		// Token: 0x06004F40 RID: 20288 RVA: 0x00234ACD File Offset: 0x00232CCD
		public void Refresh()
		{
			if (!this.initilzed)
			{
				this.LoadSettings();
			}
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x00234AE0 File Offset: 0x00232CE0
		public Color RelationColor(string key)
		{
			Color result;
			if (this.relation_highlights.TryGetValue(key, out result))
			{
				return result;
			}
			return Color.white;
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x00234B04 File Offset: 0x00232D04
		public void LoadSettings()
		{
			DT.Field defField = global::Defs.GetDefField("BuildWindowSettings", null);
			if (defField != null)
			{
				this.ArrowSpriteNormal = global::Defs.GetObj<Sprite>(defField, "upgrade_link", null);
				this.ColorSpriteNormal = global::Defs.GetColor(defField, "upgrade_link_tint", null);
				this.ColorSpriteBuild = global::Defs.GetColor(defField, "upgrade_link_great_structure_tint", null);
				this.Slot_Border_Normal = global::Defs.GetColor(defField, "tint_slot_border_normal", null);
				this.Slot_Border_Selected = global::Defs.GetColor(defField, "tint_slot_border_selected", null);
				this.relation_highlights.Clear();
				DT.Field field = defField.FindChild("relation_highlights", null, true, true, true, '.');
				if (field != null && field.children != null)
				{
					for (int i = 0; i < field.children.Count; i++)
					{
						DT.Field field2 = field.children[i];
						if (!string.IsNullOrEmpty(field2.key))
						{
							Color color = global::Defs.GetColor(field2, Color.white, null);
							this.relation_highlights.Add(field2.key, color);
						}
					}
				}
			}
			this.initilzed = true;
		}

		// Token: 0x04003D57 RID: 15703
		public Sprite ArrowSpriteNormal;

		// Token: 0x04003D58 RID: 15704
		public Color ColorSpriteBuild;

		// Token: 0x04003D59 RID: 15705
		public Color ColorSpriteNormal;

		// Token: 0x04003D5A RID: 15706
		public Color Slot_Border_Normal = Color.black;

		// Token: 0x04003D5B RID: 15707
		public Color Slot_Border_Selected = new Color(1f, 0.5254902f, 0.04705882f, 1f);

		// Token: 0x04003D5C RID: 15708
		public Dictionary<string, Color> relation_highlights = new Dictionary<string, Color>();

		// Token: 0x04003D5D RID: 15709
		private bool initilzed;
	}
}
