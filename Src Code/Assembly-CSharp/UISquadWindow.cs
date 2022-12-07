using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001A6 RID: 422
public class UISquadWindow : ObjectWindow, IListener
{
	// Token: 0x1700013A RID: 314
	// (get) Token: 0x060017FA RID: 6138 RVA: 0x000EA321 File Offset: 0x000E8521
	// (set) Token: 0x060017FB RID: 6139 RVA: 0x000EA329 File Offset: 0x000E8529
	public Logic.Squad Data { get; private set; }

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060017FC RID: 6140 RVA: 0x000EA332 File Offset: 0x000E8532
	// (set) Token: 0x060017FD RID: 6141 RVA: 0x000EA33A File Offset: 0x000E853A
	public global::Squad Visuals { get; private set; }

	// Token: 0x060017FE RID: 6142 RVA: 0x000EA343 File Offset: 0x000E8543
	private void Start()
	{
		this.ExtractLogicObject();
		this.Initialize();
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x000EA351 File Offset: 0x000E8551
	private void OnEnable()
	{
		if (this.logicObject == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x000EA361 File Offset: 0x000E8561
	protected override void OnDestroy()
	{
		this.Clear();
		if (this.m_SquadFrame != null)
		{
			this.m_SquadFrame.onSquadTypeIconClick -= this.UIBattleViewSquad_OnSquadTypeIconClicked;
		}
		base.OnDestroy();
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x000EA394 File Offset: 0x000E8594
	protected override void Update()
	{
		if (this.Data != null)
		{
			this.RefreshDynamicStats();
		}
		base.Update();
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x000EA3AC File Offset: 0x000E85AC
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.vars = vars;
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		if (obj is Logic.Squad)
		{
			this.Data = (obj as Logic.Squad);
		}
		UICommon.FindComponents(this, false);
		if (this.Data != null && this.Data.visuals != null)
		{
			this.Visuals = (this.Data.visuals as global::Squad);
			this.Data.AddListener(this);
		}
		this.Refresh();
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x000EA434 File Offset: 0x000E8634
	public void BuildAsEmpty()
	{
		Debug.Log("Set As Epmty");
		this.m_GroupEmpty.gameObject.SetActive(true);
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000EA451 File Offset: 0x000E8651
	public void BuildAsPopulated()
	{
		this.m_GroupPopulated.gameObject.SetActive(true);
		this.RefreshStaticStats();
		this.RefreshDynamicStats();
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000EA470 File Offset: 0x000E8670
	public override void Refresh()
	{
		if (this.Data == null)
		{
			this.BuildAsEmpty();
			return;
		}
		this.BuildAsPopulated();
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000EA488 File Offset: 0x000E8688
	public void OnMessage(object obj, string message, object param)
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (message == "buffs_changed")
		{
			if (this.m_SquadStats != null)
			{
				this.m_SquadStats.Refresh();
			}
			return;
		}
		if (message == "defeated")
		{
			this.Refresh();
			this.Clear();
			return;
		}
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		this.Clear();
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null)
		{
			return;
		}
		battleViewUI.SelectObj(null, false, true, true, true);
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x000EA51A File Offset: 0x000E871A
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000EA522 File Offset: 0x000E8722
	public override void Release()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.Data = null;
		base.Release();
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x000EA548 File Offset: 0x000E8748
	private void ExtractLogicObject()
	{
		Logic.Squad squad = null;
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null && baseUI.selected_obj != null && !baseUI.selected_obj.Equals("null"))
		{
			global::Squad component = baseUI.selected_obj.GetComponent<global::Squad>();
			if (component != null)
			{
				squad = component.logic;
			}
		}
		if (squad != null && squad != this.Data)
		{
			this.SetObject(squad, null);
		}
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000EA5B6 File Offset: 0x000E87B6
	private void Initialize()
	{
		if (this.m_SquadFrame != null)
		{
			this.m_SquadFrame.onSquadTypeIconClick += this.UIBattleViewSquad_OnSquadTypeIconClicked;
		}
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x000EA5DD File Offset: 0x000E87DD
	private void Clear()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
			this.Data = null;
		}
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x000EA5FC File Offset: 0x000E87FC
	private void RefreshStaticStats()
	{
		if (this.m_SquadName != null)
		{
			UIText.SetText(this.m_SquadName, global::Defs.Localize(this.Data.def.field, "name", null, null, true, true));
		}
		if (this.m_SquadTypeName != null)
		{
			UIText.SetText(this.m_SquadTypeName, global::Defs.Localize(this.Data.def.field, "militaryUnitType", null, null, true, true));
		}
		if (this.m_SquadFrame != null)
		{
			this.m_SquadFrame.SetInteraction(false);
			this.m_SquadFrame.SetData(this.Data.simulation);
		}
		if (this.m_SquadStats != null)
		{
			this.m_SquadStats.SetSquad(this.Data.simulation, null);
		}
		if (this.m_SquadCounters != null)
		{
			this.m_SquadCounters.SetSquad(this.Data.simulation);
		}
		if (this.m_SquadActions != null)
		{
			Vars vars = new Vars();
			this.m_SquadActions.Setup(vars);
		}
		if (this.m_MarkTargetPanel != null)
		{
			Vars vars2 = new Vars();
			this.m_MarkTargetPanel.Setup(vars2);
		}
		RelationUtils.Stance stance = RelationUtils.Stance.None;
		if (this.logicObject != null)
		{
			stance = this.logicObject.GetStance(BaseUI.LogicKingdom());
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
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x000EA7BC File Offset: 0x000E89BC
	private unsafe void RefreshDynamicStats()
	{
		if (this.Data.def.is_siege_eq)
		{
			if (this.m_Manpower == null)
			{
				return;
			}
			if (this.Data.visuals == null)
			{
				return;
			}
			global::Squad squad = this.Data.visuals as global::Squad;
			Vars vars = new Vars();
			vars.SetVar("health", squad.data->health * 10f);
			vars.SetVar("max_health", squad.logic.simulation.unit.max_health_modified() * 10f);
			UIText.SetTextKey(this.m_Manpower, "UISquadWindow.health", vars, null);
			return;
		}
		else
		{
			if (this.m_Manpower == null)
			{
				return;
			}
			Vars vars2 = new Vars();
			vars2.SetVar("manpower", this.Data.simulation.NumTroops());
			UIText.SetTextKey(this.m_Manpower, "UISquadWindow.manpower", vars2, null);
			return;
		}
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000EA8B8 File Offset: 0x000E8AB8
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

	// Token: 0x04000F7E RID: 3966
	[UIFieldTarget("id_Populated")]
	private GameObject m_GroupPopulated;

	// Token: 0x04000F7F RID: 3967
	[UIFieldTarget("id_Empty")]
	private GameObject m_GroupEmpty;

	// Token: 0x04000F80 RID: 3968
	[UIFieldTarget("id_SquadName")]
	private TextMeshProUGUI m_SquadName;

	// Token: 0x04000F81 RID: 3969
	[UIFieldTarget("id_SquadTypeName")]
	private TextMeshProUGUI m_SquadTypeName;

	// Token: 0x04000F82 RID: 3970
	[UIFieldTarget("id_Manpower")]
	private TextMeshProUGUI m_Manpower;

	// Token: 0x04000F83 RID: 3971
	[UIFieldTarget("id_SquadFrame")]
	private UIBattleViewSquad m_SquadFrame;

	// Token: 0x04000F84 RID: 3972
	[UIFieldTarget("id_SquadStats")]
	private UISquadCombatStats m_SquadStats;

	// Token: 0x04000F85 RID: 3973
	[UIFieldTarget("id_SquadCounters")]
	private UISquadCounters m_SquadCounters;

	// Token: 0x04000F86 RID: 3974
	[UIFieldTarget("id_ArmyActions")]
	private UIBattleViewSquadActionsPanel m_SquadActions;

	// Token: 0x04000F87 RID: 3975
	[UIFieldTarget("id_MarkTarget")]
	private UIBattleViewMarkTargetPanel m_MarkTargetPanel;

	// Token: 0x04000F88 RID: 3976
	[UIFieldTarget("id_Background")]
	private GameObject m_Background;

	// Token: 0x04000F89 RID: 3977
	[UIFieldTarget("id_BackgroundEnemy")]
	private GameObject m_BackgroundEnemy;

	// Token: 0x04000F8A RID: 3978
	[UIFieldTarget("id_BackgroundAlly")]
	private GameObject m_BackgroundAlly;
}
