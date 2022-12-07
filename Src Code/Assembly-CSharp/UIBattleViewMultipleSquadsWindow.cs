using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D0 RID: 464
public class UIBattleViewMultipleSquadsWindow : MultipleObjectWindow
{
	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0010599A File Offset: 0x00103B9A
	// (set) Token: 0x06001B50 RID: 6992 RVA: 0x001059A2 File Offset: 0x00103BA2
	public List<Logic.Squad> Data { get; private set; }

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06001B51 RID: 6993 RVA: 0x001059AB File Offset: 0x00103BAB
	// (set) Token: 0x06001B52 RID: 6994 RVA: 0x001059B3 File Offset: 0x00103BB3
	public global::Squad Visuals { get; private set; }

	// Token: 0x06001B53 RID: 6995 RVA: 0x001059BC File Offset: 0x00103BBC
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x001059C4 File Offset: 0x00103BC4
	private void OnEnable()
	{
		if (this.logicObjects == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x001059D4 File Offset: 0x00103BD4
	protected override void OnDestroy()
	{
		this.Clear();
		base.OnDestroy();
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x001059E2 File Offset: 0x00103BE2
	protected override void Update()
	{
		this.Refresh();
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x001059EC File Offset: 0x00103BEC
	public override void SetObjects(List<Logic.Object> objects, Vars vars = null)
	{
		UICommon.FindComponents(this, false);
		this.Clear();
		this.UpdateStatsColors();
		base.SetObjects(objects, vars);
		this.Data = new List<Logic.Squad>();
		foreach (Logic.Object @object in objects)
		{
			this.Data.Add(@object as Logic.Squad);
		}
		if (this.Data != null)
		{
			this.AddListeners();
		}
		if (this.Data == null)
		{
			this.BuildAsEmpty();
			return;
		}
		this.BuildAsPopulated();
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x00105A90 File Offset: 0x00103C90
	public override void Refresh()
	{
		if (this.Data != null)
		{
			this.RefreshDynamicStats();
		}
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x001059BC File Offset: 0x00103BBC
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x00105AA0 File Offset: 0x00103CA0
	public override void Release()
	{
		if (this.Data != null)
		{
			this.RemoveListeners();
		}
		this.Data = null;
		base.Release();
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00105AC0 File Offset: 0x00103CC0
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (message == "defeated")
		{
			this.ValidateSelectionObject();
			this.Refresh();
			return;
		}
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		this.ValidateSelectionObject();
		this.Refresh();
	}

	// Token: 0x06001B5C RID: 7004 RVA: 0x00105B20 File Offset: 0x00103D20
	private void ExtractLogicObject()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI.selected_squads.Count <= 1)
		{
			battleViewUI.UpdateContexSelection();
			return;
		}
		List<Logic.Object> selectedObjects = battleViewUI.GetSelectedObjects();
		if (selectedObjects != null && selectedObjects != this.logicObjects)
		{
			this.SetObjects(selectedObjects, null);
		}
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x00105B63 File Offset: 0x00103D63
	private void BuildAsEmpty()
	{
		Debug.Log("Set As Empty");
		this.m_GroupEmpty.gameObject.SetActive(true);
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x00105B80 File Offset: 0x00103D80
	private void BuildAsPopulated()
	{
		this.m_GroupPopulated.gameObject.SetActive(true);
		this.RefreshStaticStats();
		this.RefreshDynamicStats();
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x00105B9F File Offset: 0x00103D9F
	private void Clear()
	{
		if (this.Data != null)
		{
			this.RemoveListeners();
			this.Data = null;
		}
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x00105BB8 File Offset: 0x00103DB8
	private void UpdateStatsColors()
	{
		DT.Field defField = global::Defs.GetDefField("SquadStatusColors", null);
		if (this.m_Stamina != null)
		{
			this.m_Stamina.color = global::Defs.GetColor(defField, "stamina", null);
		}
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x00105BF8 File Offset: 0x00103DF8
	private void RefreshStaticStats()
	{
		if (this.m_SquadActions != null)
		{
			this.m_SquadActions.Setup(null);
		}
		if (this.m_MarkTargetPanel != null)
		{
			this.m_MarkTargetPanel.Setup(null);
		}
		if (this.m_Morale != null)
		{
			this.m_Morale.AddSquads(this.Data);
		}
		RelationUtils.Stance stance = RelationUtils.Stance.None;
		if (this.Data.Count > 0)
		{
			stance = this.Data[0].GetStance(BaseUI.LogicKingdom());
		}
		if (this.m_Caption != null)
		{
			if (stance == RelationUtils.Stance.None || (stance & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
			{
				UIText.SetTextKey(this.m_Caption, "UIBattleViewMultipleSquadsWindow.caption", null, null);
			}
			else if ((stance & RelationUtils.Stance.War) != RelationUtils.Stance.None)
			{
				UIText.SetTextKey(this.m_Caption, "UIBattleViewMultipleSquadsWindow.caption_all_enemies", null, null);
			}
			else
			{
				UIText.SetTextKey(this.m_Caption, "UIBattleViewMultipleSquadsWindow.caption_all_allies", null, null);
			}
		}
		if (this.m_Background != null)
		{
			this.m_Background.SetActive(stance == RelationUtils.Stance.None || (stance & RelationUtils.Stance.Own) > RelationUtils.Stance.None);
		}
		if (this.m_BackgroundEnemy != null)
		{
			this.m_BackgroundEnemy.SetActive((stance & RelationUtils.Stance.War) > RelationUtils.Stance.None);
		}
		if (this.m_BackgroundAlly != null)
		{
			this.m_BackgroundAlly.SetActive(stance != RelationUtils.Stance.None && (stance & (RelationUtils.Stance.War | RelationUtils.Stance.Own)) == RelationUtils.Stance.None);
		}
		this.onlySiegeUnits = true;
		using (List<Logic.Squad>.Enumerator enumerator = this.Data.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.simulation.def.is_siege_eq)
				{
					this.onlySiegeUnits = false;
				}
			}
		}
		if (this.vars == null)
		{
			this.vars = new Vars();
		}
		if (this.onlySiegeUnits && this.m_MachineHealthContainer != null)
		{
			Tooltip.Get(this.m_MachineHealthContainer, true).SetDef("SquadHealthTooltip", this.vars);
			return;
		}
		if (this.m_HealthContainer != null)
		{
			Tooltip.Get(this.m_HealthContainer, true).SetDef("SquadHealthTooltip", this.vars);
		}
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00105E18 File Offset: 0x00104018
	private unsafe void RefreshDynamicStats()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		foreach (Logic.Squad squad in this.Data)
		{
			if (squad.visuals != null)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				num += squad2.data->health;
				num2 += squad2.logic.simulation.unit.max_health_modified();
			}
			if (!squad.def.is_siege_eq)
			{
				num5 += (float)squad.simulation.NumTroops();
				num3 += squad.GetStamina();
				num4 += squad.MaxStamina();
				num6 += (float)squad.simulation.max_troops;
			}
		}
		this.m_SquadStatsContainer.gameObject.SetActive(!this.onlySiegeUnits);
		this.m_SiegeMachineStatsContainer.gameObject.SetActive(this.onlySiegeUnits);
		if (this.onlySiegeUnits)
		{
			if (this.m_Manpower == null)
			{
				return;
			}
			this.vars.SetVar("health", num * 10f);
			this.vars.SetVar("max_health", num2 * 10f);
			UIText.SetTextKey(this.m_Manpower, "UISquadWindow.health", this.vars, null);
			this.m_MachineHealth.fillAmount = num / num2;
			return;
		}
		else
		{
			if (this.m_Manpower == null)
			{
				return;
			}
			this.vars.SetVar("manpower", num5);
			this.vars.SetVar("health", num5);
			this.vars.SetVar("max_health", num6);
			UIText.SetTextKey(this.m_Manpower, "UISquadWindow.manpower", this.vars, null);
			this.m_Stamina.fillAmount = num3 / num4;
			this.m_Health.fillAmount = num5 / num6;
			this.m_Morale.Refresh();
			return;
		}
	}

	// Token: 0x040011C2 RID: 4546
	[UIFieldTarget("id_Populated")]
	private GameObject m_GroupPopulated;

	// Token: 0x040011C3 RID: 4547
	[UIFieldTarget("id_Empty")]
	private GameObject m_GroupEmpty;

	// Token: 0x040011C4 RID: 4548
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040011C5 RID: 4549
	[UIFieldTarget("id_Manpower")]
	private TextMeshProUGUI m_Manpower;

	// Token: 0x040011C6 RID: 4550
	[UIFieldTarget("id_Morale")]
	private UIMultipleSquadsMorale m_Morale;

	// Token: 0x040011C7 RID: 4551
	[UIFieldTarget("id_Health")]
	private Image m_Health;

	// Token: 0x040011C8 RID: 4552
	[UIFieldTarget("id_HealthContainer")]
	private GameObject m_HealthContainer;

	// Token: 0x040011C9 RID: 4553
	[UIFieldTarget("id_MachineHealthContainer")]
	private GameObject m_MachineHealthContainer;

	// Token: 0x040011CA RID: 4554
	[UIFieldTarget("id_Stamina")]
	private Image m_Stamina;

	// Token: 0x040011CB RID: 4555
	[UIFieldTarget("id_MachineHealth")]
	private Image m_MachineHealth;

	// Token: 0x040011CC RID: 4556
	[UIFieldTarget("id_SquadStatsContainer")]
	private RectTransform m_SquadStatsContainer;

	// Token: 0x040011CD RID: 4557
	[UIFieldTarget("id_SiegeMachineStatsContainer")]
	private RectTransform m_SiegeMachineStatsContainer;

	// Token: 0x040011CE RID: 4558
	[UIFieldTarget("id_Background")]
	private GameObject m_Background;

	// Token: 0x040011CF RID: 4559
	[UIFieldTarget("id_BackgroundEnemy")]
	private GameObject m_BackgroundEnemy;

	// Token: 0x040011D0 RID: 4560
	[UIFieldTarget("id_BackgroundAlly")]
	private GameObject m_BackgroundAlly;

	// Token: 0x040011D1 RID: 4561
	[UIFieldTarget("id_SquadActions")]
	private UIBattleViewSquadActionsPanel m_SquadActions;

	// Token: 0x040011D2 RID: 4562
	[UIFieldTarget("id_MarkTarget")]
	private UIBattleViewMarkTargetPanel m_MarkTargetPanel;

	// Token: 0x040011D5 RID: 4565
	private bool onlySiegeUnits;
}
