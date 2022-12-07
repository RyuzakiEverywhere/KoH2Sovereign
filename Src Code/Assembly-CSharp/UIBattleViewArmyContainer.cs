using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class UIBattleViewArmyContainer : MonoBehaviour, IListener
{
	// Token: 0x06001AB5 RID: 6837 RVA: 0x0010263A File Offset: 0x0010083A
	private void Start()
	{
		this.Initialize();
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x00102642 File Offset: 0x00100842
	private void Update()
	{
		if (this.m_refresh && !this.m_isFinishing)
		{
			this.Refresh();
		}
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x0010265A File Offset: 0x0010085A
	public void Refresh()
	{
		if (!this.m_isInitialized)
		{
			this.Initialize();
		}
		if (this.m_logic == null)
		{
			return;
		}
		this.CreateArmyViews();
		if (this.m_ControlGroupsControler != null)
		{
			this.m_ControlGroupsControler.Refresh();
		}
		this.m_refresh = false;
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x0010269C File Offset: 0x0010089C
	public void UpdateSelection()
	{
		for (int i = 0; i < this.m_armyViews.Count; i++)
		{
			this.m_armyViews[i].UpdateSelection();
		}
		if (this.m_ControlGroupsControler != null)
		{
			this.m_ControlGroupsControler.Refresh();
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x001026EC File Offset: 0x001008EC
	public void CycleSelection()
	{
		if (this.m_armySquads.Count == 0)
		{
			return;
		}
		int num = -1;
		int num2 = -1;
		if (this.m_armySquads.Count > 1)
		{
			int num3 = 0;
			for (int i = 0; i < this.m_armySquads.Count; i++)
			{
				global::Squad squad = this.m_armySquads[i];
				if (!(squad == null))
				{
					if (squad.Selected)
					{
						if (num == -1)
						{
							num = i;
						}
						num3++;
						if (num3 > 1)
						{
							return;
						}
					}
					if (num2 == -1 && !squad.logic.IsDefeated())
					{
						num2 = i;
					}
				}
			}
			if (num != -1)
			{
				for (int j = num + 1; j < num + this.m_armySquads.Count; j++)
				{
					int num4 = j % this.m_armySquads.Count;
					global::Squad squad2 = this.m_armySquads[num4];
					BattleSimulation.Squad squad3 = (squad2 != null) ? squad2.logic.simulation : null;
					if (squad3 != null && !squad3.IsDefeated())
					{
						num = num4;
						break;
					}
				}
			}
		}
		if (this.m_armySquads.Count > 0)
		{
			if (num == -1)
			{
				if (num2 == -1)
				{
					return;
				}
				num = num2;
			}
			global::Squad squad4 = this.m_armySquads[num];
			BattleViewUI.Get().SelectObjFromLogic(squad4.logic, false, true);
		}
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x0010281C File Offset: 0x00100A1C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "finishing" || message == "leave_battle")
		{
			this.m_refresh = false;
			this.m_isFinishing = true;
			return;
		}
		if (message == "armies_changed" || message == "units_changed" || message == "battle_updated")
		{
			this.m_refresh = true;
			return;
		}
		if (!(message == "Restart"))
		{
			return;
		}
		this.Clear();
		this.m_refresh = true;
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x0010289C File Offset: 0x00100A9C
	public void ToggleArmyWindowsVisibility()
	{
		this.m_Container.gameObject.SetActive(!this.m_Container.gameObject.activeSelf);
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x001028C4 File Offset: 0x00100AC4
	private void Initialize()
	{
		this.m_ui = BattleViewUI.Get();
		if (this.m_ui == null)
		{
			return;
		}
		if (BattleMap.battle != null)
		{
			this.m_logic = BattleMap.battle;
			this.m_logic.AddListener(this);
			this.InitializeComponents();
			this.UpdateBattleSide();
			this.InitializeListeners();
			this.CreateArmyViews();
		}
		this.m_isInitialized = true;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x00102928 File Offset: 0x00100B28
	private void InitializeComponents()
	{
		UICommon.FindComponents(this, false);
		this.m_OwnArmyWindow.Initialize();
		this.m_SupportingArmyWindow.Initialize();
		this.m_GarrisonArmyWindow.Initialize();
		this.m_armyViews = new List<UIBattleViewArmy>
		{
			this.m_OwnArmyWindow,
			this.m_SupportingArmyWindow,
			this.m_GarrisonArmyWindow
		};
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x0010298C File Offset: 0x00100B8C
	private void UpdateBattleSide()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.m_logic.attacker_kingdom.id != kingdom.id)
		{
			Logic.Army attacker_support = this.m_logic.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom().id : 0) != kingdom.id)
			{
				if (this.m_logic.defender_kingdom.id != kingdom.id)
				{
					Logic.Army defender_support = this.m_logic.defender_support;
					if (((defender_support != null) ? defender_support.GetKingdom().id : 0) != kingdom.id)
					{
						return;
					}
				}
				this.m_battleSide = 1;
				return;
			}
		}
		this.m_battleSide = 0;
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x00102A28 File Offset: 0x00100C28
	private void InitializeListeners()
	{
		List<Logic.Army> armies = this.m_logic.GetArmies(this.m_battleSide);
		if (armies != null)
		{
			for (int i = 0; i < armies.Count; i++)
			{
				armies[i].AddListener(this);
			}
		}
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x00102A68 File Offset: 0x00100C68
	private void CreateArmyViews()
	{
		if (this.m_logic == null || this.m_isFinishing)
		{
			return;
		}
		this.m_armySquads.Clear();
		List<BattleSimulation.Squad> list = new List<BattleSimulation.Squad>();
		List<BattleSimulation.Squad> list2 = new List<BattleSimulation.Squad>();
		List<BattleSimulation.Squad> list3 = new List<BattleSimulation.Squad>();
		List<InventoryItem> list4 = new List<InventoryItem>();
		List<InventoryItem> list5 = new List<InventoryItem>();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		List<BattleSimulation.Squad> squads = this.m_logic.simulation.GetSquads(this.m_battleSide);
		if (this.m_logic.settlement != null && kingdom.IsAllyOrOwn(this.m_logic.settlement))
		{
			for (int i = 0; i < squads.Count; i++)
			{
				if (squads[i] != null && squads[i].sub_squads != null && squads[i].sub_squads.Count != 0 && squads[i].garrison != null)
				{
					if (squads[i].def.type == Logic.Unit.Type.Noble)
					{
						list3.Insert(0, squads[i].sub_squads[0]);
					}
					else
					{
						list3.Add(squads[i].sub_squads[0]);
					}
				}
			}
		}
		if (this.m_logic.GetArmies(this.m_battleSide).Count > 0)
		{
			Logic.Army army = this.m_logic.GetArmies(this.m_battleSide)[0];
			for (int j = 0; j < squads.Count; j++)
			{
				if (squads[j] != null && squads[j].sub_squads != null && squads[j].sub_squads.Count != 0 && !list3.Contains(squads[j].sub_squads[0]))
				{
					if (squads[j].army != army)
					{
						if (squads[j].def.type == Logic.Unit.Type.Noble)
						{
							list2.Insert(0, squads[j].sub_squads[0]);
						}
						else
						{
							list2.Add(squads[j].sub_squads[0]);
						}
					}
					else if (squads[j].def.type == Logic.Unit.Type.Noble)
					{
						list.Insert(0, squads[j].sub_squads[0]);
					}
					else
					{
						list.Add(squads[j].sub_squads[0]);
					}
				}
			}
			List<InventoryItem> equipment = this.m_logic.simulation.GetEquipment(this.m_battleSide);
			for (int k = 0; k < equipment.Count; k++)
			{
				if (equipment[k] != null)
				{
					if (equipment[k].army != army)
					{
						list5.Add(equipment[k]);
					}
					else
					{
						list4.Add(equipment[k]);
					}
				}
			}
		}
		this.UpdateArmyWindow(this.m_OwnArmyWindow, list, list4, null);
		this.UpdateArmyWindow(this.m_SupportingArmyWindow, list2, list5, null);
		this.UpdateArmyWindow(this.m_GarrisonArmyWindow, list3, new List<InventoryItem>(), this.m_logic.settlement);
		this.AddSquadsToSelectionList(list);
		this.AddSquadsToSelectionList(list2);
		this.AddSquadsToSelectionList(list3);
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x00102DB6 File Offset: 0x00100FB6
	private void UpdateArmyWindow(UIBattleViewArmy armyWindow, List<BattleSimulation.Squad> squads, List<InventoryItem> armyItems, Logic.Settlement settlement = null)
	{
		armyWindow.SetSquads(squads, settlement);
		armyWindow.SetInventoryItems(armyItems);
		armyWindow.Show(squads.Count != 0 || armyItems.Count != 0);
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x00102DE4 File Offset: 0x00100FE4
	private void AddSquadsToSelectionList(List<BattleSimulation.Squad> squads)
	{
		for (int i = 0; i < squads.Count; i++)
		{
			Logic.Squad squad = squads[i].squad;
			global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
			if (!(squad2 == null))
			{
				this.m_armySquads.Add(squad2);
			}
		}
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x00102E38 File Offset: 0x00101038
	private void Clear()
	{
		foreach (UIBattleViewArmy uibattleViewArmy in this.m_armyViews)
		{
			uibattleViewArmy.Clear();
		}
		List<Logic.Army> armies = this.m_logic.GetArmies(this.m_battleSide);
		if (armies != null)
		{
			for (int i = 0; i < armies.Count; i++)
			{
				armies[i].DelListener(this);
			}
		}
		this.m_armySquads.Clear();
		this.m_logic.DelListener(this);
		this.m_logic = null;
		this.m_isInitialized = false;
	}

	// Token: 0x0400115E RID: 4446
	[UIFieldTarget("id_Container")]
	private RectTransform m_Container;

	// Token: 0x0400115F RID: 4447
	[UIFieldTarget("id_OwnArmyWindow")]
	private UIBattleViewArmy m_OwnArmyWindow;

	// Token: 0x04001160 RID: 4448
	[UIFieldTarget("id_SupportingArmyWindow")]
	private UIBattleViewArmy m_SupportingArmyWindow;

	// Token: 0x04001161 RID: 4449
	[UIFieldTarget("id_GarrisonArmyWindow")]
	private UIBattleViewArmy m_GarrisonArmyWindow;

	// Token: 0x04001162 RID: 4450
	[UIFieldTarget("id_ControlGroupsControler")]
	private UIBattleViewGroupsControler m_ControlGroupsControler;

	// Token: 0x04001163 RID: 4451
	public Logic.Battle m_logic;

	// Token: 0x04001164 RID: 4452
	private int m_battleSide;

	// Token: 0x04001165 RID: 4453
	private BattleViewUI m_ui;

	// Token: 0x04001166 RID: 4454
	private List<UIBattleViewArmy> m_armyViews = new List<UIBattleViewArmy>();

	// Token: 0x04001167 RID: 4455
	private List<global::Squad> m_armySquads = new List<global::Squad>();

	// Token: 0x04001168 RID: 4456
	private bool m_refresh = true;

	// Token: 0x04001169 RID: 4457
	private bool m_isInitialized;

	// Token: 0x0400116A RID: 4458
	private bool m_isFinishing;
}
