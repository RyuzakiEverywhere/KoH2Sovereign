using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C5 RID: 453
public class UIBattleViewCharacterIcon : MonoBehaviour
{
	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06001ACA RID: 6858 RVA: 0x00103022 File Offset: 0x00101222
	public UICharacterIcon Icon
	{
		get
		{
			return this.m_CharacterIcon;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06001ACB RID: 6859 RVA: 0x0010302C File Offset: 0x0010122C
	public BattleSimulation.Squad SimulationSquadLogic
	{
		get
		{
			BattleSimulation.Squad squad = this._squad;
			if (((squad != null) ? squad.squad : null) == null || this._squad.squad.IsValid())
			{
				this._squad = null;
			}
			if (this._squad == null)
			{
				this._squad = this.ExtractSimulationLogic(this.Data);
				BattleSimulation.Squad squad2 = this._squad;
				object obj;
				if (squad2 == null)
				{
					obj = null;
				}
				else
				{
					Logic.Squad squad3 = squad2.squad;
					obj = ((squad3 != null) ? squad3.visuals : null);
				}
				this.m_SquadVisuals = (obj as global::Squad);
			}
			return this._squad;
		}
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x001030AF File Offset: 0x001012AF
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x001030B8 File Offset: 0x001012B8
	private void Update()
	{
		if (this.m_SquadVisuals != null && this.m_CharacterIcon != null && this.m_CharacterIcon.mouse_in)
		{
			this.m_SquadVisuals.MouseOvered = this.m_CharacterIcon.mouse_in;
		}
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x00103104 File Offset: 0x00101304
	public void SetObject(Logic.Character character, Vars vars)
	{
		this.Initialize();
		this.Data = character;
		if (this.m_CharacterIcon != null)
		{
			this.m_CharacterIcon.SetObject(character, vars);
			this.m_CharacterIcon.OnSelect += this.CharacterIcon_OnSelect;
			this.m_CharacterIcon.OnFocus += this.CharacterIcon_OnFocus;
		}
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x00103168 File Offset: 0x00101368
	private BattleSimulation.Squad ExtractSimulationLogic(Logic.Character leader)
	{
		Logic.Army army = (leader != null) ? leader.GetArmy() : null;
		Logic.Battle battle = (army != null) ? army.battle : null;
		if (battle == null)
		{
			return null;
		}
		int battle_side = army.battle_side;
		List<Logic.Squad> list = battle.squads.Get(army.battle_side);
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad = list[i];
			if (squad != null && !squad.IsDefeated() && squad.def.type == Logic.Unit.Type.Noble && squad.simulation.army == army)
			{
				return squad.simulation;
			}
		}
		return null;
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x001031FB File Offset: 0x001013FB
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_isInitialized = true;
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x00103214 File Offset: 0x00101414
	private void CharacterIcon_OnSelect(UICharacterIcon obj)
	{
		if (obj.Data == null)
		{
			return;
		}
		BattleSimulation.Squad simulationSquadLogic = this.SimulationSquadLogic;
		bool flag;
		if (simulationSquadLogic == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Squad squad = simulationSquadLogic.squad;
			flag = (((squad != null) ? squad.visuals : null) != null);
		}
		if (!flag)
		{
			return;
		}
		if (this.m_SquadVisuals == null)
		{
			this.m_SquadVisuals = (this.SimulationSquadLogic.squad.visuals as global::Squad);
		}
		this.m_SquadVisuals.Selected = true;
		BattleViewUI.Get().SelectObj(this.m_SquadVisuals.gameObject, false, true, true, true);
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x00103299 File Offset: 0x00101499
	private void CharacterIcon_OnFocus(UICharacterIcon obj)
	{
		if (obj.Data == null)
		{
			return;
		}
		BattleSimulation.Squad simulationSquadLogic = this.SimulationSquadLogic;
		bool flag;
		if (simulationSquadLogic == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Squad squad = simulationSquadLogic.squad;
			flag = (((squad != null) ? squad.visuals : null) != null);
		}
		if (!flag)
		{
			return;
		}
		BattleViewUI.Get().CenterCameraOnSelection();
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x001032D0 File Offset: 0x001014D0
	private void UpdateStateIcons()
	{
		BattleSimulation.Squad simulationSquadLogic = this.SimulationSquadLogic;
		BattleSimulation.Squad.State state = BattleSimulation.Squad.State.Idle;
		if (simulationSquadLogic != null)
		{
			state = simulationSquadLogic.state;
		}
		bool flag = (this.m_SquadVisuals != null && this.m_SquadVisuals.Highlighted) || this.m_CharacterIcon.mouse_in;
		if (state != this.prev_state || flag != this.prev_sel)
		{
			this.prev_state = state;
			this.prev_sel = flag;
			DT.Field defField;
			if (state == BattleSimulation.Squad.State.Fled)
			{
				defField = global::Defs.GetDefField("UIBattleViewSquad", "Flee");
			}
			else if (state == BattleSimulation.Squad.State.Dead)
			{
				defField = global::Defs.GetDefField("UIBattleViewSquad", "Destroyed");
			}
			else if (flag)
			{
				defField = global::Defs.GetDefField("UIBattleViewSquad", "Selected");
			}
			else
			{
				defField = global::Defs.GetDefField("UIBattleViewSquad", "Default");
			}
			this.m_CharacterIcon.variable_color = global::Defs.GetColor(defField, "icon_color", null);
			this.m_CharacterIcon.UpdateHighlight();
		}
		if (this.m_FleeingIcon != null)
		{
			this.m_FleeingIcon.gameObject.SetActive(state == BattleSimulation.Squad.State.Fled);
		}
		if (this.m_RetreatIcon != null)
		{
			this.m_RetreatIcon.gameObject.SetActive(state == BattleSimulation.Squad.State.Retreating);
		}
		if (this.m_SquadIconDead != null)
		{
			this.m_SquadIconDead.gameObject.SetActive(state == BattleSimulation.Squad.State.Dead);
		}
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x00103416 File Offset: 0x00101616
	private void LateUpdate()
	{
		this.UpdateStateIcons();
	}

	// Token: 0x0400116F RID: 4463
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon m_CharacterIcon;

	// Token: 0x04001170 RID: 4464
	[UIFieldTarget("id_SquadIconDead")]
	private Image m_SquadIconDead;

	// Token: 0x04001171 RID: 4465
	[UIFieldTarget("id_RetreatIcon")]
	private Image m_RetreatIcon;

	// Token: 0x04001172 RID: 4466
	[UIFieldTarget("id_FleeingIcon")]
	private Image m_FleeingIcon;

	// Token: 0x04001173 RID: 4467
	public Logic.Character Data;

	// Token: 0x04001174 RID: 4468
	private BattleSimulation.Squad _squad;

	// Token: 0x04001175 RID: 4469
	private global::Squad m_SquadVisuals;

	// Token: 0x04001176 RID: 4470
	private bool m_isInitialized;

	// Token: 0x04001177 RID: 4471
	private BattleSimulation.Squad.State prev_state;

	// Token: 0x04001178 RID: 4472
	private bool prev_sel;
}
