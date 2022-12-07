using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C1 RID: 449
public class UIBattleViewArmy : MonoBehaviour
{
	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06001A92 RID: 6802 RVA: 0x001015B5 File Offset: 0x000FF7B5
	private bool IsGarrisonArmy
	{
		get
		{
			return this.m_settlement != null;
		}
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x001015C0 File Offset: 0x000FF7C0
	private void LateUpdate()
	{
		if (this.m_refresh)
		{
			this.m_refresh = false;
			this.Refresh();
			this.UpdateSelection();
		}
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x001015E0 File Offset: 0x000FF7E0
	public void Initialize()
	{
		this.m_ui = BattleViewUI.Get();
		if (this.m_ui == null)
		{
			return;
		}
		this.InitializeWindowDef();
		this.InitializeComponents();
		if (BattleMap.battle != null)
		{
			this.logic = BattleMap.battle;
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (this.logic.attacker_kingdom.id != kingdom.id)
			{
				Logic.Army attacker_support = this.logic.attacker_support;
				if (((attacker_support != null) ? attacker_support.GetKingdom().id : 0) != kingdom.id)
				{
					if (this.logic.defender_kingdom.id != kingdom.id)
					{
						Logic.Army defender_support = this.logic.defender_support;
						if (((defender_support != null) ? defender_support.GetKingdom().id : 0) != kingdom.id)
						{
							goto IL_C9;
						}
					}
					this.BattleSide = 1;
					goto IL_C9;
				}
			}
			this.BattleSide = 0;
			IL_C9:
			this.Refresh();
		}
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x001016BC File Offset: 0x000FF8BC
	public void SetSquads(List<BattleSimulation.Squad> squads, Logic.Settlement settlement = null)
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.simulation == null)
		{
			return;
		}
		this.m_settlement = settlement;
		this.m_logicSquads = squads;
		this.InitializeRegularSquadSlots();
		this.Refresh();
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x001016EF File Offset: 0x000FF8EF
	public void SetInventoryItems(List<InventoryItem> armyItems)
	{
		this.m_SiegeEquipmentContainer.SetObject(armyItems);
		this.m_SiegeEquipmentContainer.gameObject.SetActive(armyItems.Count > 0);
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x00101716 File Offset: 0x000FF916
	public void Show(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x00101724 File Offset: 0x000FF924
	public void Refresh()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.simulation == null)
		{
			return;
		}
		int num = 0;
		if (this.IsGarrisonArmy)
		{
			this.SetGarrisonOnArmyOwnerField();
		}
		BattleSimulation.Squad squad = null;
		for (int i = 0; i < this.m_logicSquads.Count; i++)
		{
			BattleSimulation.Squad squad2 = this.m_logicSquads[i];
			if (squad2 != null && squad2.def != null && squad2.squad != null && squad2.squad.is_main_squad && squad2.def.type == Logic.Unit.Type.Noble && !this.IsGarrisonArmy)
			{
				squad = squad2;
			}
		}
		for (int j = 0; j < this.m_logicSquads.Count; j++)
		{
			BattleSimulation.Squad squad3 = this.m_logicSquads[j];
			if (squad3 != null && squad3.def != null && squad3.squad != null && squad3.squad.is_main_squad && !this.m_squadFrames.ContainsKey(squad3))
			{
				if (squad3.def.type == Logic.Unit.Type.Noble && this.m_armyOwnerContainer.childCount == 0 && !this.IsGarrisonArmy)
				{
					this.AddSquad(this.m_logicSquads[j], this.m_armyOwnerContainer, true);
					TMP_Text armyOwnerName = this.m_armyOwnerName;
					string key = "Character.name_only";
					Logic.Army army = this.m_logicSquads[j].army;
					UIText.SetTextKey(armyOwnerName, key, new Vars((army != null) ? army.leader : null), null);
				}
				else if (!squad3.def.is_siege_eq && this.AddRegularSquad(this.m_logicSquads[j]))
				{
					num++;
				}
			}
		}
		if (this.m_ArmySupporterContainer != null)
		{
			if (((squad != null) ? squad.squad : null) != null)
			{
				Logic.Kingdom obj = BaseUI.LogicKingdom();
				this.m_ArmySupporterContainer.SetActive(!squad.squad.IsOwnStance(obj) && !squad.squad.IsEnemy(obj));
				UIText.SetTextKey(this.m_ArmySupporterText, "Battle.BattleView.Allies", null, null);
			}
			else
			{
				this.m_ArmySupporterContainer.SetActive(false);
			}
		}
		this.m_armyOwner.gameObject.SetActive(this.m_armyOwnerContainer.childCount != 0);
		Dictionary<int, BaseUI.ControlGroup> groups = this.m_ui.groups;
		if (groups != null)
		{
			foreach (KeyValuePair<int, BaseUI.ControlGroup> keyValuePair in groups)
			{
				BaseUI.ControlGroup value = keyValuePair.Value;
				for (int k = 0; k < value.squads.Count; k++)
				{
					Logic.Squad squad4 = value.squads[k];
					UIBattleViewSquad uibattleViewSquad;
					if (squad4 != null && this.m_squadFrames.TryGetValue(squad4.simulation, out uibattleViewSquad))
					{
						this.m_battleViewSquads.Remove(uibattleViewSquad);
						this.m_battleViewSquads.Insert(0, uibattleViewSquad);
						uibattleViewSquad.transform.SetAsFirstSibling();
					}
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_squadsContainer);
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x00101A30 File Offset: 0x000FFC30
	public void UpdateSelection()
	{
		for (int i = this.m_battleViewSquads.Count - 1; i >= 0; i--)
		{
			if (this.m_battleViewSquads[i] == null)
			{
				this.m_battleViewSquads.Remove(this.m_battleViewSquads[i]);
			}
			else if (this.m_battleViewSquads[i].SimulationSquadLogic != null)
			{
				this.m_battleViewSquads[i].UpdateSelection();
			}
		}
		if (this.m_SiegeEquipmentContainer != null)
		{
			this.m_SiegeEquipmentContainer.UpdateSelection();
		}
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x00101AC0 File Offset: 0x000FFCC0
	public void Clear()
	{
		for (int i = 0; i < this.m_battleViewSquads.Count; i++)
		{
			this.m_battleViewSquads.Clear();
		}
		this.m_battleViewSquads.Clear();
		this.m_logicSquads.Clear();
		this.m_squadFrames.Clear();
		this.m_emptyRegularSlots.Clear();
		this.m_SiegeEquipmentContainer.Clear();
		this.m_settlement = null;
		global::Common.DestroyChildren(this.m_armyOwnerContainer);
		global::Common.DestroyChildren(this.m_squadsContainer);
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x00101B42 File Offset: 0x000FFD42
	private void InitializeWindowDef()
	{
		if (this.m_windowDefinition == null)
		{
			this.m_windowDefinition = global::Defs.GetDefField("BattleViewArmyWindow", null);
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x00101B5D File Offset: 0x000FFD5D
	private void InitializeComponents()
	{
		UICommon.FindComponents(this, false);
		UICommon.DeleteChildren(this.m_squadsContainer);
		UICommon.DeleteChildren(this.m_armyOwnerContainer);
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x00101B7C File Offset: 0x000FFD7C
	private void InitializeRegularSquadSlots()
	{
		if (this.IsGarrisonArmy)
		{
			this.m_numberOfSlots = this.m_windowDefinition.GetInt("garrison_number_of_unit_slots", null, 0, true, true, true, '.');
		}
		else
		{
			this.m_numberOfSlots = this.m_windowDefinition.GetInt("default_number_of_unit_slots", null, 0, true, true, true, '.');
		}
		if (this.m_squadsContainer.childCount == 0)
		{
			this.m_emptyRegularSlots = new Queue<UIBattleViewSquad>(this.m_numberOfSlots);
			for (int i = 0; i < this.m_numberOfSlots; i++)
			{
				this.AddSquad(null, this.m_squadsContainer, false);
				this.m_emptyRegularSlots.Enqueue(this.m_squadsContainer.GetChild(i).GetComponent<UIBattleViewSquad>());
			}
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x00101C28 File Offset: 0x000FFE28
	private bool AddSquad(BattleSimulation.Squad squad, RectTransform parent, bool isNormalVersion = false)
	{
		if (parent == null)
		{
			return false;
		}
		if (squad != null && squad.def.is_siege_eq)
		{
			return true;
		}
		string key = isNormalVersion ? "squad_plate_normal" : "squad_plate_small";
		GameObject obj = global::Defs.GetObj<GameObject>(this.m_windowDefinition, key, null);
		if (obj == null)
		{
			return false;
		}
		GameObject gameObject = global::Common.Spawn(obj, parent, false, "");
		UIBattleViewSquad component = gameObject.GetComponent<UIBattleViewSquad>();
		if (component == null)
		{
			return false;
		}
		component.SetData(squad);
		component.onClick += this.HandleSquadClicked;
		component.onRemoved += this.UIBattleViewSquad_OnRemoved;
		component.onSquadTypeIconClick += this.UIBattleViewSquad_OnSquadTypeIconClicked;
		if (squad != null && squad.squad.IsValid())
		{
			this.m_squadFrames[squad] = component;
		}
		this.m_battleViewSquads.Add(component);
		UICommon.FitInParent(gameObject.GetComponent<RectTransform>());
		return true;
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x00101D10 File Offset: 0x000FFF10
	private bool AddRegularSquad(BattleSimulation.Squad squad)
	{
		if (squad == null || !squad.squad.IsValid())
		{
			return false;
		}
		if (this.m_emptyRegularSlots.Count == 0)
		{
			return this.AddSquad(squad, this.m_squadsContainer, false);
		}
		UIBattleViewSquad uibattleViewSquad = this.m_emptyRegularSlots.Dequeue();
		uibattleViewSquad.SetData(squad);
		uibattleViewSquad.onClick += this.HandleSquadClicked;
		uibattleViewSquad.onRemoved += this.UIBattleViewSquad_OnRemoved;
		this.m_squadFrames[squad] = uibattleViewSquad;
		return true;
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x00101D90 File Offset: 0x000FFF90
	private void RemoveSquad(UIBattleViewSquad squad)
	{
		if (this.m_battleViewSquads == null)
		{
			return;
		}
		if (squad.SimulationSquadLogic != null)
		{
			this.m_squadFrames.Remove(squad.SimulationSquadLogic);
		}
		if (squad.transform.parent == this.m_squadsContainer && squad.transform.GetSiblingIndex() < this.m_numberOfSlots)
		{
			squad.SetData(null);
			this.m_emptyRegularSlots.Enqueue(squad);
			return;
		}
		squad.onClick -= this.HandleSquadClicked;
		squad.onRemoved -= this.UIBattleViewSquad_OnRemoved;
		this.m_battleViewSquads.Remove(squad);
		global::Common.DestroyObj(squad.gameObject);
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x00101E3C File Offset: 0x0010003C
	private void SetGarrisonOnArmyOwnerField()
	{
		if (this.m_armyOwnerContainer == null || this.m_armyOwnerContainer.childCount != 0 || this.m_settlement == null)
		{
			return;
		}
		GameObject obj = global::Defs.GetObj<GameObject>(this.m_windowDefinition, "garrison_avatar", null);
		if (obj == null)
		{
			return;
		}
		UIBattleViewGarrisonIcon component = global::Common.Spawn(obj, this.m_armyOwnerContainer, false, "").GetComponent<UIBattleViewGarrisonIcon>();
		component.SetObject(this.m_settlement, null);
		component.onClick += this.UIBattleViewGarrisonIcon_OnClick;
		UICommon.FitInParent(component.GetComponent<RectTransform>());
		UIText.SetTextKey(this.m_armyOwnerName, "BattleViewArmyWindow.local_troops", null, null);
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x00101EDC File Offset: 0x001000DC
	private void UIBattleViewGarrisonIcon_OnClick(UIBattleViewGarrisonIcon s, PointerEventData e)
	{
		if (s == null || s.m_logic == null || s.m_logic.garrison == null)
		{
			return;
		}
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		List<GameObject> list = new List<GameObject>();
		if (UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl))
		{
			List<Logic.Squad> list2 = s.m_logic.battle.squads.Get(this.BattleSide);
			for (int i = 0; i < list2.Count; i++)
			{
				global::Squad squad = list2[i].visuals as global::Squad;
				if (!(squad == null) && squad.logic.IsValid() && !squad.logic.IsDefeated() && squad.logic.simulation.garrison != null && squad.logic.simulation.garrison == s.m_logic.garrison)
				{
					list.Add(squad.gameObject);
				}
			}
		}
		battleViewUI.SelectSquads(list, true);
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x00101FD0 File Offset: 0x001001D0
	private void HandleSquadClicked(UIBattleViewSquad s, PointerEventData e)
	{
		if (s == null || s.SimulationSquadLogic == null || s.SquadLogic == null)
		{
			return;
		}
		global::Squad visuals = s.Visuals;
		if (visuals != null)
		{
			BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
			List<GameObject> list = new List<GameObject>();
			list.Add(visuals.gameObject);
			if (e.clickCount > 1 && battleViewUI.selected_obj != null)
			{
				battleViewUI.LookAt(battleViewUI.selected_obj.transform.position, false);
			}
			if (UICommon.GetModifierKey(UICommon.ModifierKey.Ctrl))
			{
				List<Logic.Squad> list2 = s.SimulationSquadLogic.simulation.battle.squads.Get(visuals.logic.battle_side);
				for (int i = 0; i < list2.Count; i++)
				{
					global::Squad squad = list2[i].visuals as global::Squad;
					if (!(squad == null) && squad.logic.IsValid() && !squad.logic.IsDefeated() && (visuals.def.type != Logic.Unit.Type.Noble || squad.logic.simulation.army == visuals.logic.simulation.army) && (visuals.def.type == Logic.Unit.Type.Noble || !(visuals.def.name != squad.def.name)))
					{
						list.Add(squad.gameObject);
					}
				}
			}
			battleViewUI.SelectSquads(list, true);
		}
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x0010214C File Offset: 0x0010034C
	private void UIBattleViewSquad_OnSquadTypeIconClicked(UIBattleViewSquad s, PointerEventData e)
	{
		if (s == null || s.SimulationSquadLogic == null || s.SquadLogic == null)
		{
			return;
		}
		global::Squad visuals = s.Visuals;
		if (visuals != null)
		{
			BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
			battleViewUI.BattleViewSquad_OnSquadTypeIconClicked(visuals.gameObject, false, true, true, true);
			if (e.clickCount > 1 && battleViewUI.selected_obj != null)
			{
				battleViewUI.LookAt(battleViewUI.selected_obj.transform.position, false);
			}
		}
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x001021C7 File Offset: 0x001003C7
	private void UIBattleViewSquad_OnRemoved(UIBattleViewSquad squad)
	{
		this.RemoveSquad(squad);
	}

	// Token: 0x04001143 RID: 4419
	public Logic.Battle logic;

	// Token: 0x04001144 RID: 4420
	public int BattleSide;

	// Token: 0x04001145 RID: 4421
	[UIFieldTarget("id_ArmyOwner")]
	private GameObject m_armyOwner;

	// Token: 0x04001146 RID: 4422
	[UIFieldTarget("id_ArmyOwnerContainer")]
	private RectTransform m_armyOwnerContainer;

	// Token: 0x04001147 RID: 4423
	[UIFieldTarget("id_ArmySupporterContainer")]
	private GameObject m_ArmySupporterContainer;

	// Token: 0x04001148 RID: 4424
	[UIFieldTarget("id_ArmySupporterText")]
	private TextMeshProUGUI m_ArmySupporterText;

	// Token: 0x04001149 RID: 4425
	[UIFieldTarget("id_ArmyOwnerName")]
	private TextMeshProUGUI m_armyOwnerName;

	// Token: 0x0400114A RID: 4426
	[UIFieldTarget("id_Squads")]
	private RectTransform m_squadsContainer;

	// Token: 0x0400114B RID: 4427
	[UIFieldTarget("id_SiegeEquipmentContainer")]
	private UIBattleViewSiegeEquipmentContainer m_SiegeEquipmentContainer;

	// Token: 0x0400114C RID: 4428
	private BattleViewUI m_ui;

	// Token: 0x0400114D RID: 4429
	private DT.Field m_windowDefinition;

	// Token: 0x0400114E RID: 4430
	private Logic.Settlement m_settlement;

	// Token: 0x0400114F RID: 4431
	private Dictionary<BattleSimulation.Squad, UIBattleViewSquad> m_squadFrames = new Dictionary<BattleSimulation.Squad, UIBattleViewSquad>();

	// Token: 0x04001150 RID: 4432
	private List<UIBattleViewSquad> m_battleViewSquads = new List<UIBattleViewSquad>();

	// Token: 0x04001151 RID: 4433
	private List<BattleSimulation.Squad> m_logicSquads = new List<BattleSimulation.Squad>();

	// Token: 0x04001152 RID: 4434
	private bool m_refresh;

	// Token: 0x04001153 RID: 4435
	private int m_numberOfSlots;

	// Token: 0x04001154 RID: 4436
	private Queue<UIBattleViewSquad> m_emptyRegularSlots;
}
