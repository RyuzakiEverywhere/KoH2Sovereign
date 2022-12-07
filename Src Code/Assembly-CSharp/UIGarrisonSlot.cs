using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B4 RID: 692
public class UIGarrisonSlot : Hotspot
{
	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06002B60 RID: 11104 RVA: 0x0016F0B6 File Offset: 0x0016D2B6
	// (set) Token: 0x06002B61 RID: 11105 RVA: 0x0016F0BE File Offset: 0x0016D2BE
	public Logic.Unit.Def UnitDef { get; private set; }

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06002B62 RID: 11106 RVA: 0x0016F0C7 File Offset: 0x0016D2C7
	// (set) Token: 0x06002B63 RID: 11107 RVA: 0x0016F0CF File Offset: 0x0016D2CF
	public int SlotIndex { get; private set; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06002B64 RID: 11108 RVA: 0x0016F0D8 File Offset: 0x0016D2D8
	// (set) Token: 0x06002B65 RID: 11109 RVA: 0x0016F0E0 File Offset: 0x0016D2E0
	public Castle Castle { get; private set; }

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06002B66 RID: 11110 RVA: 0x0016F0E9 File Offset: 0x0016D2E9
	// (set) Token: 0x06002B67 RID: 11111 RVA: 0x0016F0F1 File Offset: 0x0016D2F1
	public Logic.Army Army { get; private set; }

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06002B68 RID: 11112 RVA: 0x0016F0FA File Offset: 0x0016D2FA
	// (set) Token: 0x06002B69 RID: 11113 RVA: 0x0016F102 File Offset: 0x0016D302
	public DT.Field state_def { get; private set; }

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06002B6A RID: 11114 RVA: 0x0016F10B File Offset: 0x0016D30B
	// (set) Token: 0x06002B6B RID: 11115 RVA: 0x0016F113 File Offset: 0x0016D313
	public DT.Field slot_def { get; private set; }

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06002B6C RID: 11116 RVA: 0x0016F11C File Offset: 0x0016D31C
	// (set) Token: 0x06002B6D RID: 11117 RVA: 0x0016F124 File Offset: 0x0016D324
	public UIGarrisonSlot.State state { get; private set; }

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x06002B6E RID: 11118 RVA: 0x0016F130 File Offset: 0x0016D330
	// (remove) Token: 0x06002B6F RID: 11119 RVA: 0x0016F168 File Offset: 0x0016D368
	public event Action<UIGarrisonSlot> OnSelected;

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06002B70 RID: 11120 RVA: 0x0016F1A0 File Offset: 0x0016D3A0
	// (remove) Token: 0x06002B71 RID: 11121 RVA: 0x0016F1D8 File Offset: 0x0016D3D8
	public event Action<UIGarrisonSlot> OnFocused;

	// Token: 0x06002B72 RID: 11122 RVA: 0x0016F20D File Offset: 0x0016D40D
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initiazlied = false;
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x0016F228 File Offset: 0x0016D428
	public void SetData(Logic.Unit.Def unit, int slotIndex, Castle castle = null, Logic.Army army = null)
	{
		this.Init();
		this.SlotIndex = slotIndex;
		this.UnitDef = unit;
		this.Castle = castle;
		this.Army = army;
		Vars vars = new Vars(this.UnitDef);
		vars.Set<DT.Field>("name", this.UnitDef.field.FindChild("name", null, true, true, true, '.'));
		vars.Set<Castle>("castle", this.Castle);
		vars.Set<Logic.Army>("army", this.Army);
		vars.Set<bool>("is_bayer_a_army", this.Army != null);
		Tooltip.Get(base.gameObject, true).SetDef("UnitTooltip", vars);
		if (this.UnitDef != null && this.m_UnitIcon != null)
		{
			this.m_UnitIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.UnitDef.dt_def.field, "icon", null);
		}
		this.UpdateState();
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x0016F31E File Offset: 0x0016D51E
	public void Select(bool selected)
	{
		if (this.m_Selected != selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlight();
		}
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x0016F336 File Offset: 0x0016D536
	private void Update()
	{
		if (this.UnitDef == null)
		{
			base.enabled = false;
			return;
		}
		this.UpdateState();
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x0016F350 File Offset: 0x0016D550
	public void UpdateState()
	{
		UIGarrisonSlot.State state = this.DecideState();
		this.SetState(state);
		if (this.m_UnitIcon != null)
		{
			this.m_UnitIcon.gameObject.SetActive(this.state_def.GetBool("show_icon", null, false, true, true, true, '.'));
			this.m_UnitIcon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x0016F3C4 File Offset: 0x0016D5C4
	public void SetState(UIGarrisonSlot.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		Tooltip.Get(base.gameObject, true).Refresh();
		this.state = state;
		if (this.slot_def == null)
		{
			this.slot_def = global::Defs.GetDefField("UnitHireSlot", null);
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

	// Token: 0x06002B78 RID: 11128 RVA: 0x0016F484 File Offset: 0x0016D684
	public UIGarrisonSlot.State DecideState()
	{
		Logic.Unit.Def unitDef = this.UnitDef;
		if (unitDef == null)
		{
			return UIGarrisonSlot.State.Empty;
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
			num = ((available_units != null) ? new bool?(available_units.CanBuildUnit(this.UnitDef, true)) : null);
		}
		int num2 = num ?? 0;
		bool flag = num2 != 0 && this.Castle.CheckUnitCost(unitDef, this.Castle.GetUnitCost(unitDef, this.Castle.army));
		if (num2 == 0)
		{
			return UIGarrisonSlot.State.Unavailable;
		}
		if (!flag)
		{
			return UIGarrisonSlot.State.CannotHire;
		}
		return UIGarrisonSlot.State.Available;
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x0016F51F File Offset: 0x0016D71F
	public override void OnClick(PointerEventData e)
	{
		this.UpdateHighlight();
		if (this.OnSelected != null)
		{
			this.OnSelected(this);
		}
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x0016F53B File Offset: 0x0016D73B
	public override void OnDoubleClick(PointerEventData e)
	{
		this.UpdateHighlight();
		if (this.OnFocused != null)
		{
			this.OnFocused(this);
		}
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x0016F557 File Offset: 0x0016D757
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x0016F566 File Offset: 0x0016D766
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000023FD File Offset: 0x000005FD
	public void UpdateHighlight()
	{
	}

	// Token: 0x04001D9D RID: 7581
	[UIFieldTarget("id_Background")]
	private Image m_BackgroundImage;

	// Token: 0x04001D9E RID: 7582
	[UIFieldTarget("id_Icon")]
	private Image m_UnitIcon;

	// Token: 0x04001DA8 RID: 7592
	private bool m_Selected;

	// Token: 0x04001DA9 RID: 7593
	private bool m_Initiazlied;

	// Token: 0x02000810 RID: 2064
	public enum State
	{
		// Token: 0x04003D9C RID: 15772
		Empty,
		// Token: 0x04003D9D RID: 15773
		Available,
		// Token: 0x04003D9E RID: 15774
		Unavailable,
		// Token: 0x04003D9F RID: 15775
		CannotHire
	}
}
