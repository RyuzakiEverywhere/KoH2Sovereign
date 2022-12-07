using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000297 RID: 663
internal class TraditionSlot : Hotspot
{
	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060028D4 RID: 10452 RVA: 0x0015C84F File Offset: 0x0015AA4F
	// (set) Token: 0x060028D5 RID: 10453 RVA: 0x0015C857 File Offset: 0x0015AA57
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060028D6 RID: 10454 RVA: 0x0015C860 File Offset: 0x0015AA60
	// (set) Token: 0x060028D7 RID: 10455 RVA: 0x0015C868 File Offset: 0x0015AA68
	public Vars Vars { get; private set; }

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060028D8 RID: 10456 RVA: 0x0015C871 File Offset: 0x0015AA71
	// (set) Token: 0x060028D9 RID: 10457 RVA: 0x0015C879 File Offset: 0x0015AA79
	public Tradition.Def Override { get; private set; }

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060028DA RID: 10458 RVA: 0x0015C882 File Offset: 0x0015AA82
	// (set) Token: 0x060028DB RID: 10459 RVA: 0x0015C88A File Offset: 0x0015AA8A
	public Tradition Traditon { get; private set; }

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060028DC RID: 10460 RVA: 0x0015C893 File Offset: 0x0015AA93
	// (set) Token: 0x060028DD RID: 10461 RVA: 0x0015C89B File Offset: 0x0015AA9B
	public int Index { get; private set; }

	// Token: 0x060028DE RID: 10462 RVA: 0x0015C8A4 File Offset: 0x0015AAA4
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Icon != null)
		{
			this.m_Icon.OnSelect += this.HandleOnSelect;
		}
		if (this.m_Abandon != null)
		{
			this.m_Abandon.onClick = new BSGButton.OnClick(this.HandleOnAbandon);
			this.m_Abandon.gameObject.SetActive(false);
		}
		if (this.m_TutorialFirstFreeSlot != null)
		{
			this.m_TutorialFirstFreeSlot.gameObject.SetActive(false);
		}
		this.tooltipVars = new Vars();
		this.m_Initialized = true;
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x0015C94D File Offset: 0x0015AB4D
	private void HandleOnSelect(UIDynastyTradition icon, PointerEventData e)
	{
		Action<TraditionSlot> onSelect = this.OnSelect;
		if (onSelect == null)
		{
			return;
		}
		onSelect(this);
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x0015C960 File Offset: 0x0015AB60
	public void SetData(Logic.Kingdom k, int slotIndex, Tradition.Def def, Vars vars)
	{
		this.Init();
		this.Kingdom = k;
		this.Index = slotIndex;
		this.Vars = vars;
		this.Override = def;
		this.Traditon = this.Kingdom.GetTradition(this.Index);
		this.tooltipVars.obj = this.Kingdom;
		this.tooltipVars.Set<int>("slot_index", this.Index);
		if (this.Override == null && this.Traditon == null)
		{
			this.UpdateTooltipVars();
			Tooltip.Get(base.gameObject, true).SetDef("TraditionSlotTooltip", this.tooltipVars);
		}
		if (this.m_Abandon != null)
		{
			Action obj = this.Kingdom.actions.Find("AbandonTraditionAction");
			Tooltip tooltip = Tooltip.Get(this.m_Abandon.gameObject, true);
			tooltip.SetObj(obj, null, null);
			tooltip.vars.Set<Tradition.Def>("tradition", def);
		}
		this.Refresh();
		this.CheckViusalState(true);
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x0015CA60 File Offset: 0x0015AC60
	public void EnableTutorialHighlight(bool enable)
	{
		if (this.m_TutorialFirstFreeSlot != null)
		{
			this.m_TutorialFirstFreeSlot.SetActive(enable);
		}
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x0015CA7C File Offset: 0x0015AC7C
	private void Update()
	{
		this.UpdateTooltipVars();
		this.CheckViusalState(false);
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x0015CA8C File Offset: 0x0015AC8C
	private void CheckViusalState(bool force = false)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		bool flag = kingdom == null || kingdom.CanAffordTradition(this.Index);
		bool flag2 = this.IsTraditionSlotInUse(this.Kingdom, this.Index);
		bool flag3 = !this.IsTraditionSlotUnloked(this.Kingdom, this.Index);
		bool flag4 = false;
		if (flag != this.m_CanAfforf)
		{
			flag4 = true;
		}
		if (flag2 != this.m_InUse)
		{
			flag4 = true;
		}
		if (flag3 != this.m_IsLocked)
		{
			flag4 = true;
		}
		this.m_CanAfforf = flag;
		this.m_InUse = flag2;
		this.m_IsLocked = flag3;
		if (flag4 || force)
		{
			this.UpdateVisualState();
		}
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x0015CB1E File Offset: 0x0015AD1E
	private bool IsTraditionSlotInUse(Logic.Kingdom k, int slotIndex)
	{
		return k != null && slotIndex >= 0 && k.traditions != null && k.traditions.Count > slotIndex && k.traditions[slotIndex] != null;
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x0015CB54 File Offset: 0x0015AD54
	private bool IsTraditionSlotUnloked(Logic.Kingdom k, int slotIndex)
	{
		return k != null && k.IsTraditionSlotUnlocked(slotIndex);
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x0015CB62 File Offset: 0x0015AD62
	public bool IsLocked()
	{
		return this.m_IsLocked;
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x0015CB6A File Offset: 0x0015AD6A
	public bool IsFree()
	{
		return !this.m_InUse && !this.m_IsLocked;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x0015CB7F File Offset: 0x0015AD7F
	private void UpdateTooltipVars()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		this.tooltipVars.Set<bool>("is_player", this.Kingdom == BaseUI.LogicKingdom());
		this.tooltipVars.Set<bool>("is_locked", this.m_IsLocked);
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x0015CBC0 File Offset: 0x0015ADC0
	private void Refresh()
	{
		if (this.Override != null)
		{
			this.m_Icon.SetData(this.Kingdom, this.Override, true);
		}
		else if (this.Traditon != null)
		{
			this.m_Icon.SetData(this.Kingdom, this.Traditon, true);
		}
		else
		{
			this.m_Icon.SetData(this.Kingdom, null, true);
		}
		this.CheckViusalState(true);
		this.UpdateHighlight();
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x0015CC31 File Offset: 0x0015AE31
	public void SetFocused(bool focused)
	{
		this.m_Focused = focused;
		this.UpdateHighlight();
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x0015CC40 File Offset: 0x0015AE40
	public void UpdateVisualState()
	{
		if (this.m_Locked != null)
		{
			this.m_Locked.gameObject.SetActive(!this.m_InUse && this.m_IsLocked);
		}
		if (!this.m_IsLocked)
		{
			if (this.Traditon != null)
			{
				this.m_Icon.SetVisualState("adopted");
			}
			else if (this.Override != null)
			{
				Tradition.Type tt = this.Kingdom.tradition_slots_types[this.Index];
				if (this.Kingdom.HasTradition(this.Override))
				{
					this.m_Icon.SetVisualState("adopted");
				}
				else if (!this.Override.MatchTrditionType(tt))
				{
					this.m_Icon.SetVisualState("wrong_slot");
				}
				else if (!this.m_CanAfforf)
				{
					this.m_Icon.SetVisualState("reqierment_not_met");
				}
				else
				{
					this.m_Icon.SetVisualState("adoptable");
				}
			}
		}
		if (this.m_Free != null)
		{
			this.m_Free.gameObject.SetActive(!this.m_InUse && this.m_CanAfforf && !this.m_IsLocked);
		}
		if (this.m_ReqiermntsNotMet != null)
		{
			this.m_ReqiermntsNotMet.gameObject.SetActive(!this.m_InUse && !this.m_CanAfforf && !this.m_IsLocked);
		}
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x0015CDA5 File Offset: 0x0015AFA5
	public override void OnPointerEnter(PointerEventData e)
	{
		base.OnPointerEnter(e);
		this.UpdateHighlight();
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x0015CDB4 File Offset: 0x0015AFB4
	public override void OnPointerExit(PointerEventData e)
	{
		base.OnPointerExit(e);
		this.UpdateHighlight();
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x0015CDC4 File Offset: 0x0015AFC4
	private void UpdateHighlight()
	{
		if (this.m_Abandon != null)
		{
			this.m_Abandon.gameObject.SetActive(this.mouse_in && this.m_InUse);
		}
		if (this.m_Selected != null)
		{
			this.m_Selected.SetActive(this.m_Focused);
		}
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x0015CE20 File Offset: 0x0015B020
	private void HandleOnAbandon(BSGButton btn)
	{
		if (this.Traditon == null)
		{
			return;
		}
		if (this.Kingdom == null)
		{
			return;
		}
		Action action = this.Kingdom.actions.Find("AbandonTraditionAction");
		if (action != null)
		{
			if (action.args != null)
			{
				action.args = null;
			}
			action.AddArg(ref action.args, this.Traditon.def.id, 0);
			ActionVisuals.ExecuteAction(action);
		}
	}

	// Token: 0x04001B9C RID: 7068
	[UIFieldTarget("id_Locked")]
	private GameObject m_Locked;

	// Token: 0x04001B9D RID: 7069
	[UIFieldTarget("id_ReqiermntsNotMet")]
	private GameObject m_ReqiermntsNotMet;

	// Token: 0x04001B9E RID: 7070
	[UIFieldTarget("id_Free")]
	private GameObject m_Free;

	// Token: 0x04001B9F RID: 7071
	[UIFieldTarget("id_Selected")]
	private GameObject m_Selected;

	// Token: 0x04001BA0 RID: 7072
	[UIFieldTarget("id_TraditionIcon")]
	private UIDynastyTradition m_Icon;

	// Token: 0x04001BA1 RID: 7073
	[UIFieldTarget("id_Abandon")]
	private BSGButton m_Abandon;

	// Token: 0x04001BA2 RID: 7074
	[UIFieldTarget("tut_FirstFreeSlot")]
	private GameObject m_TutorialFirstFreeSlot;

	// Token: 0x04001BA8 RID: 7080
	public Action<TraditionSlot> OnSelect;

	// Token: 0x04001BA9 RID: 7081
	private bool m_Initialized;

	// Token: 0x04001BAA RID: 7082
	private bool m_Focused;

	// Token: 0x04001BAB RID: 7083
	private bool m_IsLocked;

	// Token: 0x04001BAC RID: 7084
	private Vars tooltipVars;

	// Token: 0x04001BAD RID: 7085
	private bool m_InUse;

	// Token: 0x04001BAE RID: 7086
	private bool m_CanAfforf;
}
