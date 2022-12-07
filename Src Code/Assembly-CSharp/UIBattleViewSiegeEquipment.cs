using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001D3 RID: 467
public class UIBattleViewSiegeEquipment : Hotspot, IListener
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06001B8A RID: 7050 RVA: 0x00107334 File Offset: 0x00105534
	// (remove) Token: 0x06001B8B RID: 7051 RVA: 0x0010736C File Offset: 0x0010556C
	public event Action<UIBattleViewSiegeEquipment, PointerEventData> onClick;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06001B8C RID: 7052 RVA: 0x001073A4 File Offset: 0x001055A4
	// (remove) Token: 0x06001B8D RID: 7053 RVA: 0x001073DC File Offset: 0x001055DC
	public event Action<UIBattleViewSiegeEquipment> onRemoved;

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06001B8E RID: 7054 RVA: 0x00107411 File Offset: 0x00105611
	public bool IsSelectable
	{
		get
		{
			return this.Visuals != null;
		}
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x0010741F File Offset: 0x0010561F
	private void Update()
	{
		this.Refresh();
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00107427 File Offset: 0x00105627
	public void SetInventoryItem(InventoryItem item)
	{
		this.Clear();
		this.logic = item;
		this.Initialize();
		this.Setup();
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x00107442 File Offset: 0x00105642
	public void Refresh()
	{
		UIBattleViewSiegeEquipment.SelectionState selectionState = this.m_selectionState;
		this.UpdateState();
		if (selectionState != this.m_selectionState)
		{
			this.UpdateBackground();
			this.UpdateItemsVisibility();
			this.UpdateIcon();
		}
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x0010746A File Offset: 0x0010566A
	public void UpdateSelection()
	{
		if (this.Visuals != null)
		{
			this.Select(this.Visuals.Selected);
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x0010748B File Offset: 0x0010568B
	public void Select(bool selected)
	{
		this.m_isSelected = selected;
		this.Refresh();
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x0010749A File Offset: 0x0010569A
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (!this.IsSelectable)
		{
			return;
		}
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
		this.Refresh();
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x001074C7 File Offset: 0x001056C7
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		if (!this.IsSelectable)
		{
			return;
		}
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x001074F0 File Offset: 0x001056F0
	public void OnMessage(object obj, string message, object param)
	{
		if (this == null)
		{
			return;
		}
		if (base.gameObject == null)
		{
			return;
		}
		if (message == "defeated")
		{
			this.Refresh();
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			if (this.SquadLogic.simulation.spawned_in_bv)
			{
				Action<UIBattleViewSiegeEquipment> action = this.onRemoved;
				if (action != null)
				{
					action(this);
				}
				this.Clear();
				this.Refresh();
			}
			return;
		}
		if (!(message == "troop_died"))
		{
			return;
		}
		this.Refresh();
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00107589 File Offset: 0x00105789
	private void Clear()
	{
		if (this.SquadLogic != null)
		{
			this.SquadLogic.DelListener(this);
		}
		this.m_isSelected = false;
		this.logic = null;
		this.SquadLogic = null;
		this.Visuals = null;
		this.onClick = null;
		this.onRemoved = null;
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x001075C9 File Offset: 0x001057C9
	private void Initialize()
	{
		this.InitializeDefinitions();
		this.InitializeComponents();
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x001075D7 File Offset: 0x001057D7
	private void InitializeDefinitions()
	{
		if (this.m_windowDefinition == null)
		{
			this.m_windowDefinition = global::Defs.GetDefField("UIBattleViewSiegeEquipment", null);
		}
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void InitializeComponents()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x001075F4 File Offset: 0x001057F4
	private void Setup()
	{
		if (this.logic != null)
		{
			this.m_PopulatedSlot.SetActive(true);
			if (this.logic.simulation != null && this.logic.simulation.sub_squads != null && this.logic.simulation.sub_squads.Count > 0 && this.logic.simulation.sub_squads[0] != null)
			{
				this.sim_squad = this.logic.simulation.sub_squads[0];
				this.SquadLogic = this.sim_squad.squad;
				this.Visuals = (this.SquadLogic.visuals as global::Squad);
				if (this.m_SquadStatusBar != null)
				{
					this.m_SquadStatusBar.SetSquad(this.SquadLogic);
					this.m_SquadStatusBar.gameObject.SetActive(true);
				}
				this.SquadLogic.AddListener(this);
			}
			else if (this.m_SquadStatusBar != null)
			{
				this.m_SquadStatusBar.gameObject.SetActive(false);
			}
			if (this.m_Icon != null)
			{
				this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.logic.def.dt_def.field, "icon", null);
			}
			if (this.sim_squad != null)
			{
				Tooltip.Get(base.gameObject, true).SetDef("SquadBVTooltip", new Vars(this.sim_squad));
			}
		}
		else
		{
			this.m_PopulatedSlot.SetActive(false);
		}
		this.Refresh();
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x0010778C File Offset: 0x0010598C
	private void UpdateState()
	{
		UIBattleViewSiegeEquipment.SelectionState state = this.DecideState();
		this.SetState(state);
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x001077A8 File Offset: 0x001059A8
	private void SetState(UIBattleViewSiegeEquipment.SelectionState state)
	{
		if (this.m_selectionState == state && this.m_stateDefinition != null)
		{
			return;
		}
		this.m_selectionState = state;
		if (this.m_windowDefinition != null)
		{
			this.m_stateDefinition = this.m_windowDefinition.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.m_stateDefinition == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.m_stateDefinition = this.m_windowDefinition.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.m_stateDefinition = null;
		}
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x0010783C File Offset: 0x00105A3C
	private UIBattleViewSiegeEquipment.SelectionState DecideState()
	{
		if (this.logic == null)
		{
			return UIBattleViewSiegeEquipment.SelectionState.Empty;
		}
		if (this.Visuals == null || this.SquadLogic == null)
		{
			return UIBattleViewSiegeEquipment.SelectionState.NotInteractive;
		}
		this.Visuals.MouseOvered = false;
		Logic.Squad squadLogic = this.SquadLogic;
		if (squadLogic == null || squadLogic.IsDefeated())
		{
			return UIBattleViewSiegeEquipment.SelectionState.Disabled;
		}
		if (this.m_isSelected)
		{
			return UIBattleViewSiegeEquipment.SelectionState.Selected;
		}
		if (this.mouse_in || (this.Visuals != null && this.Visuals.Highlighted))
		{
			this.Visuals.MouseOvered = true;
			return UIBattleViewSiegeEquipment.SelectionState.Over;
		}
		return UIBattleViewSiegeEquipment.SelectionState.Default;
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x001078C9 File Offset: 0x00105AC9
	private void UpdateBackground()
	{
		if (this.m_stateDefinition == null || this.m_Background == null)
		{
			return;
		}
		this.m_Background.sprite = global::Defs.GetObj<Sprite>(this.m_stateDefinition, "background", null);
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x001078FE File Offset: 0x00105AFE
	private void UpdateIcon()
	{
		if (this.m_stateDefinition == null || this.m_Icon == null)
		{
			return;
		}
		this.m_Icon.color = global::Defs.GetColor(this.m_stateDefinition, "icon_color", null);
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00107934 File Offset: 0x00105B34
	private void UpdateItemsVisibility()
	{
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.gameObject.SetActive(this.m_selectionState != UIBattleViewSiegeEquipment.SelectionState.Disabled && this.m_selectionState != UIBattleViewSiegeEquipment.SelectionState.NotInteractive && this.m_selectionState != UIBattleViewSiegeEquipment.SelectionState.Empty && this.SquadLogic != null && this.Visuals != null);
		}
	}

	// Token: 0x040011EB RID: 4587
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x040011EC RID: 4588
	[UIFieldTarget("id_PopulatedSlot")]
	private GameObject m_PopulatedSlot;

	// Token: 0x040011ED RID: 4589
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040011EE RID: 4590
	[UIFieldTarget("id_SquadStatusBar")]
	private UISquadStatusBar m_SquadStatusBar;

	// Token: 0x040011EF RID: 4591
	public InventoryItem logic;

	// Token: 0x040011F0 RID: 4592
	public Logic.Squad SquadLogic;

	// Token: 0x040011F1 RID: 4593
	public BattleSimulation.Squad sim_squad;

	// Token: 0x040011F2 RID: 4594
	public global::Squad Visuals;

	// Token: 0x040011F3 RID: 4595
	private DT.Field m_windowDefinition;

	// Token: 0x040011F4 RID: 4596
	private DT.Field m_stateDefinition;

	// Token: 0x040011F5 RID: 4597
	private UIBattleViewSiegeEquipment.SelectionState m_selectionState;

	// Token: 0x040011F6 RID: 4598
	private bool m_isSelected;

	// Token: 0x0200071A RID: 1818
	protected enum SelectionState
	{
		// Token: 0x04003851 RID: 14417
		Default,
		// Token: 0x04003852 RID: 14418
		Over,
		// Token: 0x04003853 RID: 14419
		Selected,
		// Token: 0x04003854 RID: 14420
		Disabled,
		// Token: 0x04003855 RID: 14421
		NotInteractive,
		// Token: 0x04003856 RID: 14422
		Empty
	}
}
