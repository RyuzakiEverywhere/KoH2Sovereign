using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000198 RID: 408
public class UIArmyItemIcon : ObjectIcon
{
	// Token: 0x1700012D RID: 301
	// (get) Token: 0x060016B5 RID: 5813 RVA: 0x000E2A14 File Offset: 0x000E0C14
	public bool Locked
	{
		get
		{
			return this.m_ShowLockIcon;
		}
	}

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x060016B6 RID: 5814 RVA: 0x000E2A1C File Offset: 0x000E0C1C
	// (remove) Token: 0x060016B7 RID: 5815 RVA: 0x000E2A54 File Offset: 0x000E0C54
	public event Action<UIArmyItemIcon, PointerEventData> OnSelected;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x060016B8 RID: 5816 RVA: 0x000E2A8C File Offset: 0x000E0C8C
	// (remove) Token: 0x060016B9 RID: 5817 RVA: 0x000E2AC4 File Offset: 0x000E0CC4
	public event Action<UIArmyItemIcon> OnDisband;

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x060016BA RID: 5818 RVA: 0x000E2AF9 File Offset: 0x000E0CF9
	// (set) Token: 0x060016BB RID: 5819 RVA: 0x000E2B01 File Offset: 0x000E0D01
	public InventoryItem UnitInstance { get; private set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x060016BC RID: 5820 RVA: 0x000E2B0A File Offset: 0x000E0D0A
	// (set) Token: 0x060016BD RID: 5821 RVA: 0x000E2B12 File Offset: 0x000E0D12
	public Logic.Unit.Def UnitDef { get; private set; }

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x060016BE RID: 5822 RVA: 0x000E2B1B File Offset: 0x000E0D1B
	// (set) Token: 0x060016BF RID: 5823 RVA: 0x000E2B23 File Offset: 0x000E0D23
	public Castle Castle { get; private set; }

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x060016C0 RID: 5824 RVA: 0x000E2B2C File Offset: 0x000E0D2C
	// (set) Token: 0x060016C1 RID: 5825 RVA: 0x000E2B34 File Offset: 0x000E0D34
	public Logic.Battle Battle { get; private set; }

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x060016C2 RID: 5826 RVA: 0x000E2B3D File Offset: 0x000E0D3D
	// (set) Token: 0x060016C3 RID: 5827 RVA: 0x000E2B45 File Offset: 0x000E0D45
	public DT.Field state_def { get; private set; }

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x060016C4 RID: 5828 RVA: 0x000E2B4E File Offset: 0x000E0D4E
	// (set) Token: 0x060016C5 RID: 5829 RVA: 0x000E2B56 File Offset: 0x000E0D56
	public DT.Field slot_def { get; private set; }

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x060016C6 RID: 5830 RVA: 0x000E2B5F File Offset: 0x000E0D5F
	// (set) Token: 0x060016C7 RID: 5831 RVA: 0x000E2B67 File Offset: 0x000E0D67
	public UIArmyItemIcon.State state { get; private set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x060016C8 RID: 5832 RVA: 0x000E2B70 File Offset: 0x000E0D70
	// (set) Token: 0x060016C9 RID: 5833 RVA: 0x000E2B78 File Offset: 0x000E0D78
	public int SlotIndex { get; private set; }

	// Token: 0x060016CA RID: 5834 RVA: 0x000E2B81 File Offset: 0x000E0D81
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_DisbandButton != null)
		{
			this.m_DisbandButton.onClick = new BSGButton.OnClick(this.HandleOnDisband);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x000E2BBF File Offset: 0x000E0DBF
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.Init();
		this.Refresh();
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x000E2BD8 File Offset: 0x000E0DD8
	public void SetDef(Logic.Unit.Def unitDef, int slotIndex, Castle castle, Logic.Battle battle = null)
	{
		this.Init();
		this.SlotIndex = slotIndex;
		this.UnitDef = unitDef;
		this.UnitInstance = null;
		this.Castle = castle;
		this.Battle = battle;
		if (unitDef == null)
		{
			this.SetEmpty();
		}
		else
		{
			this.SetAsOccupied();
		}
		this.Refresh();
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x000E2C28 File Offset: 0x000E0E28
	public void SetNumItems(int num)
	{
		if (num < 0)
		{
			if (this.m_NumItemsContainer != null)
			{
				this.m_NumItemsContainer.SetActive(false);
				return;
			}
		}
		else
		{
			if (this.m_NumItemsContainer != null)
			{
				this.m_NumItemsContainer.SetActive(true);
			}
			if (this.m_NumItems != null)
			{
				UIText.SetText(this.m_NumItems, global::Defs.LocalizedNumber(num, null, ""));
			}
		}
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x000E2C98 File Offset: 0x000E0E98
	public void SetUnitInstance(InventoryItem unit, int slotIndex, Castle castle)
	{
		this.Init();
		this.SlotIndex = slotIndex;
		this.UnitInstance = unit;
		this.Castle = castle;
		if (unit == null)
		{
			this.SetEmpty();
		}
		else
		{
			this.SetAsOccupied();
		}
		this.Refresh();
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x000E2CCC File Offset: 0x000E0ECC
	public void SetFocused(bool focused)
	{
		this.m_IsFocused = focused;
		this.UpdateHighlight();
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x000E2CDB File Offset: 0x000E0EDB
	public void ShowLockIcon(bool shown)
	{
		if (this.m_ShowLockIcon == shown)
		{
			return;
		}
		this.m_ShowLockIcon = shown;
		if (this.m_IconLock != null)
		{
			this.m_IconLock.gameObject.SetActive(this.m_ShowLockIcon);
		}
		this.BuildToolitpVars();
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x000E2D18 File Offset: 0x000E0F18
	public void ShowDisbandButton(bool shown)
	{
		this.m_ShowDisband = shown;
		this.UpdateHighlight();
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x000E2D27 File Offset: 0x000E0F27
	public void ShowAddIcon(bool shown)
	{
		if (this.m_ShowAddIcon == shown)
		{
			return;
		}
		this.m_ShowAddIcon = shown;
		if (this.m_IconAdd != null)
		{
			this.m_IconAdd.gameObject.SetActive(shown);
		}
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x000E2D59 File Offset: 0x000E0F59
	public void Select(bool selected)
	{
		if (this.m_Selected != selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlight();
		}
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x000E2D74 File Offset: 0x000E0F74
	private void SetEmpty()
	{
		if (this.m_IconAdd != null)
		{
			this.m_IconAdd.gameObject.SetActive(this.m_ShowAddIcon);
		}
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(false);
		}
		if (this.m_IconLock != null)
		{
			this.m_IconLock.gameObject.SetActive(this.m_ShowLockIcon);
		}
		this.SetNumItems(-1);
		this.BuildToolitpVars();
		Tooltip.Get(base.gameObject, true).SetDef("InvetoryItemTooltip", this.tooltipVars);
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x000E2E14 File Offset: 0x000E1014
	private void SetAsOccupied()
	{
		this.BuildToolitpVars();
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(true);
			if (this.UnitInstance != null)
			{
				this.m_UnitIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.UnitInstance.def.dt_def.field, "icon", null);
			}
			else if (this.UnitDef != null)
			{
				this.m_UnitIcon.sprite = global::Defs.GetObj<Sprite>(this.UnitDef.dt_def.field, "icon", null);
			}
		}
		this.SetNumItems(-1);
		Tooltip.Get(base.gameObject, true).SetDef("InvetoryItemTooltip", this.tooltipVars);
		if (this.m_DisbandButton != null)
		{
			Tooltip.Get(this.m_DisbandButton.gameObject, true).SetDef("RemoveInvetoryItemTooltip", this.tooltipVars);
		}
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x000E2EFB File Offset: 0x000E10FB
	private void Update()
	{
		this.UpdateState();
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x000E2F04 File Offset: 0x000E1104
	public void UpdateState()
	{
		UIArmyItemIcon.State state = this.DecideState();
		if (!this.SetState(state))
		{
			return;
		}
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(this.state_def.GetBool("show_icon", null, false, true, true, true, '.'));
			this.m_UnitIcon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		this.UpdateHighlight();
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x000E2F7C File Offset: 0x000E117C
	public bool SetState(UIArmyItemIcon.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return false;
		}
		this.state = state;
		this.tooltipVars.Set<bool>("can_purchase", state == UIArmyItemIcon.State.Available);
		if (this.slot_def == null)
		{
			this.slot_def = global::Defs.GetDefField("ArmyInvetoryItemSlot", null);
		}
		if (this.slot_def != null)
		{
			this.state_def = this.slot_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.slot_def.FindChild("State", null, true, true, true, '.');
			}
			return true;
		}
		this.state_def = null;
		return false;
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x000E3044 File Offset: 0x000E1244
	public UIArmyItemIcon.State DecideState()
	{
		Logic.Unit.Def unitDef = this.UnitDef;
		if (unitDef == null)
		{
			return UIArmyItemIcon.State.Empty;
		}
		if (this.Battle != null)
		{
			return UIArmyItemIcon.State.ExistsAsDef;
		}
		Castle castle = this.Castle;
		int? num;
		if (castle == null)
		{
			num = null;
		}
		else
		{
			AvailableUnits available_units = castle.available_units;
			num = ((available_units != null) ? new bool?(available_units.CanBuyEquipment(this.UnitDef)) : null);
		}
		int num2 = num ?? 0;
		bool flag = num2 != 0 && this.Castle.CheckUnitCost(unitDef, this.Castle.GetUnitCost(unitDef, this.Castle.army));
		if (num2 == 0)
		{
			return UIArmyItemIcon.State.Unavailable;
		}
		if (!flag)
		{
			return UIArmyItemIcon.State.CannotPurchase;
		}
		return UIArmyItemIcon.State.Available;
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x000E30E8 File Offset: 0x000E12E8
	private void BuildToolitpVars()
	{
		if (this.tooltipVars == null)
		{
			this.tooltipVars = new Vars();
		}
		else
		{
			this.tooltipVars.Clear();
		}
		if (this.Castle != null)
		{
			this.tooltipVars.Set<Castle>("castle", this.Castle);
		}
		if (this.UnitInstance != null)
		{
			this.tooltipVars.obj = new Value(this.UnitInstance);
			Vars vars = this.tooltipVars;
			string key = "army";
			Castle castle = this.Castle;
			vars.Set<Logic.Army>(key, (castle != null) ? castle.army : null);
			this.tooltipVars.Set<string>("state", "instance");
		}
		else if (this.UnitDef != null)
		{
			this.tooltipVars.obj = this.UnitDef;
			Castle castle2 = this.Castle;
			if (((castle2 != null) ? castle2.army : null) != null)
			{
				this.tooltipVars.Set<Logic.Army>("army", this.Castle.army);
			}
			if (this.Battle != null)
			{
				this.tooltipVars.Set<string>("state", "battle");
			}
			else
			{
				this.tooltipVars.Set<string>("state", "purchase");
			}
		}
		else if (this.Locked)
		{
			this.tooltipVars.Set<string>("state", "locked");
		}
		else
		{
			this.tooltipVars.Set<string>("state", "empty");
		}
		Vars vars2 = this.tooltipVars;
		string key2 = "in_disorder";
		Castle castle3 = this.Castle;
		bool? flag;
		if (castle3 == null)
		{
			flag = null;
		}
		else
		{
			Logic.Realm realm = castle3.GetRealm();
			flag = ((realm != null) ? new bool?(realm.IsDisorder()) : null);
		}
		vars2.Set<bool>(key2, flag ?? false);
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x000E329B File Offset: 0x000E149B
	private void Refresh()
	{
		if (this.m_IconAdd != null)
		{
			this.m_IconAdd.gameObject.SetActive(this.m_ShowAddIcon);
		}
		this.UpdateState();
		this.UpdateHighlight();
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x000E32CD File Offset: 0x000E14CD
	public bool IsEmpty()
	{
		return this.UnitDef == null && this.UnitInstance == null;
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x000E32E2 File Offset: 0x000E14E2
	private void HandleOnDisband(BSGButton btn)
	{
		if (this.OnDisband != null)
		{
			this.OnDisband(this);
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x000E32F8 File Offset: 0x000E14F8
	public override void OnClick(PointerEventData e)
	{
		this.UpdateHighlight();
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x000E3315 File Offset: 0x000E1515
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x000E3324 File Offset: 0x000E1524
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x000E3334 File Offset: 0x000E1534
	public void UpdateHighlight()
	{
		if (this.m_DisbandButton != null)
		{
			this.m_DisbandButton.gameObject.SetActive(this.m_ShowDisband && this.mouse_in && this.UnitInstance != null && (Game.CheckCheatLevel(Game.CheatLevel.High, "DisbandSiege", false) || (this.UnitInstance.army != null && this.UnitInstance.army.GetKingdom() == BaseUI.LogicKingdom())));
		}
		if (this.m_FocusHighlight != null)
		{
			this.m_FocusHighlight.SetActive(this.m_IsFocused);
		}
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x000E33D1 File Offset: 0x000E15D1
	private void OnDestroy()
	{
		this.OnSelected = null;
		this.OnDisband = null;
	}

	// Token: 0x04000EB2 RID: 3762
	[UIFieldTarget("id_Icon")]
	private Image m_UnitIcon;

	// Token: 0x04000EB3 RID: 3763
	[UIFieldTarget("id_Button_Disband")]
	private BSGButton m_DisbandButton;

	// Token: 0x04000EB4 RID: 3764
	[UIFieldTarget("id_Icon_Add")]
	private GameObject m_IconAdd;

	// Token: 0x04000EB5 RID: 3765
	[UIFieldTarget("id_Icon_Lock")]
	private GameObject m_IconLock;

	// Token: 0x04000EB6 RID: 3766
	[UIFieldTarget("id_FocusHighlight")]
	private GameObject m_FocusHighlight;

	// Token: 0x04000EB7 RID: 3767
	[UIFieldTarget("id_NumItemsContainer")]
	private GameObject m_NumItemsContainer;

	// Token: 0x04000EB8 RID: 3768
	[UIFieldTarget("id_NumItems")]
	private TextMeshProUGUI m_NumItems;

	// Token: 0x04000EB9 RID: 3769
	private bool m_Selected;

	// Token: 0x04000EBA RID: 3770
	private bool m_ShowDisband;

	// Token: 0x04000EBB RID: 3771
	private bool m_ShowAddIcon;

	// Token: 0x04000EBC RID: 3772
	private bool m_IsFocused;

	// Token: 0x04000EBD RID: 3773
	private bool m_ShowLockIcon;

	// Token: 0x04000EC8 RID: 3784
	private Vars tooltipVars;

	// Token: 0x04000EC9 RID: 3785
	private bool m_Initialzied;

	// Token: 0x020006CF RID: 1743
	public enum State
	{
		// Token: 0x04003724 RID: 14116
		Empty,
		// Token: 0x04003725 RID: 14117
		Available,
		// Token: 0x04003726 RID: 14118
		Unavailable,
		// Token: 0x04003727 RID: 14119
		CannotPurchase,
		// Token: 0x04003728 RID: 14120
		ExistsAsDef
	}
}
