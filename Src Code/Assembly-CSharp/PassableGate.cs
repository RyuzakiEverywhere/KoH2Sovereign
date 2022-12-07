using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class PassableGate : GameLogic.Behaviour
{
	// Token: 0x060008EB RID: 2283 RVA: 0x00061158 File Offset: 0x0005F358
	public void CreateLogic(Logic.Battle battle)
	{
		if (this.logic != null && this.logic.IsValid())
		{
			return;
		}
		this.logic = new Logic.PassableGate(battle, battle.batte_view_game, base.transform.position, BattleMap.battle.defender, 1);
		this.logic.visuals = this;
		PassableAreaManager paManager = BattleMap.Get().paManager;
		paManager.onFinishPathfinding = (PassableAreaManager.OnFinishPathfinding)Delegate.Combine(paManager.onFinishPathfinding, new PassableAreaManager.OnFinishPathfinding(this.SetAreaPaids));
		paManager.RecalculateAreas();
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x000611E5 File Offset: 0x0005F3E5
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x000611F0 File Offset: 0x0005F3F0
	public void SetAreaPaids()
	{
		PassableAreaManager.SetAreaPaids(base.gameObject, this.logic.game.path_finding, this.logic.paids, this.logic.middle_paids);
		this.logic.ResetAreas();
		PassableAreaManager paManager = BattleMap.Get().paManager;
		paManager.onFinishPathfinding = (PassableAreaManager.OnFinishPathfinding)Delegate.Remove(paManager.onFinishPathfinding, new PassableAreaManager.OnFinishPathfinding(this.SetAreaPaids));
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00061264 File Offset: 0x0005F464
	private unsafe void OnHit()
	{
		if (this.open_go != null && this.logic != null)
		{
			this.open_go.SetBool("Destroyed", this.logic.IsDefeated());
		}
		Logic.PassableGate passableGate = this.logic;
		Logic.Fortification fortification = (passableGate != null) ? passableGate.MainFortification() : null;
		Logic.Squad squad = (fortification != null) ? fortification.last_attacker : null;
		global::Fortification fortification2 = fortification.visuals as global::Fortification;
		if (fortification2 == null)
		{
			return;
		}
		if (((squad != null) ? squad.def : null) != null)
		{
			global::Fortification.GateType gate_material = fortification2.gate_material;
			if (gate_material != global::Fortification.GateType.Wood)
			{
				if (gate_material == global::Fortification.GateType.Metal)
				{
					BaseUI.PlaySoundEvent(squad.def.HitMetalGateSound, base.transform.position, null);
				}
			}
			else
			{
				BaseUI.PlaySoundEvent(squad.def.HitWoodGateSound, base.transform.position, null);
			}
		}
		if (this.logic.IsDefeated() && fortification2.data->HasFlags(Troops.FortificationData.Flags.Hit | Troops.FortificationData.Flags.GateHit) && !this.was_destroyed)
		{
			global::Fortification.GateType gate_material = fortification2.gate_material;
			if (gate_material != global::Fortification.GateType.Wood)
			{
				if (gate_material == global::Fortification.GateType.Metal)
				{
					BaseUI.PlaySoundEvent(fortification.def.GateDestroyedMetalSound, base.transform.position, null);
				}
			}
			else
			{
				BaseUI.PlaySoundEvent(fortification.def.GateDestroyedWoodSound, base.transform.position, null);
			}
			int num = global::Battle.PlayerBattleSide();
			string str = (num == 0) ? "we" : "enemy";
			Logic.Army army = this.logic.battle.GetArmy(num);
			BaseUI.PlayCharacterlessVoiceEvent((army != null) ? army.leader : null, "battle_" + str + "_broke_gate");
		}
		this.was_destroyed = this.logic.IsDefeated();
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00061408 File Offset: 0x0005F608
	public override void OnMessage(object obj, string message, object param)
	{
		if (message == "Hit")
		{
			this.OnHit();
			return;
		}
		if (!(message == "ToggleOpen"))
		{
			message == "ToggleLocked";
			return;
		}
		if (this.open_go != null)
		{
			this.open_go.SetBool("Open", this.logic.Open);
		}
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x0006146C File Offset: 0x0005F66C
	private bool HasSquadsPlanningToMoveThrough(int battle_side)
	{
		if (this.logic.middle_paids == null || this.logic.middle_paids.Count == 0)
		{
			return false;
		}
		List<Logic.Squad> list = this.logic.battle.squads.Get(battle_side);
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad = list[i];
			if (squad.movement.PlanningToMoveThroughPAIDs(this.logic.middle_paids, squad.formation.cur_height * 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x000614F8 File Offset: 0x0005F6F8
	private unsafe bool HasTroopsUnder(int battle_side)
	{
		if (this.logic.middle_paids == null || this.logic.middle_paids.Count == 0)
		{
			return false;
		}
		List<Logic.Squad> list = this.logic.battle.squads.Get(battle_side);
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad = list[i];
			if (!squad.IsDefeated())
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				if (squad2.GetID() >= 0)
				{
					Troops.SquadData* data = squad2.data;
					Troops.Troop troop = data->FirstTroop;
					while (troop <= data->LastTroop)
					{
						if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && this.logic.middle_paids.Contains(troop.pa_id))
						{
							return true;
						}
						troop = ++troop;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x000615CC File Offset: 0x0005F7CC
	private void CheckSquadControl()
	{
		if (UnityEngine.Time.time < this.next_control_check_time)
		{
			return;
		}
		this.next_control_check_time = UnityEngine.Time.time + 2f;
		this.logic.has_friendly_squad_under = this.HasSquadsPlanningToMoveThrough(this.logic.battle_side);
		this.logic.has_enemy_squad_under = this.HasSquadsPlanningToMoveThrough(1 - this.logic.battle_side);
		if (!this.logic.has_friendly_squad_under)
		{
			this.logic.has_friendly_squad_under = this.HasTroopsUnder(this.logic.battle_side);
		}
		if (!this.logic.has_enemy_squad_under)
		{
			this.logic.has_enemy_squad_under = this.HasTroopsUnder(1 - this.logic.battle_side);
		}
		this.logic.RefreshOpen();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00061691 File Offset: 0x0005F891
	private void Update()
	{
		if (this.logic == null || this.logic.IsDefeated() || !this.logic.IsValid())
		{
			return;
		}
		this.CheckSquadControl();
	}

	// Token: 0x0400070E RID: 1806
	public Logic.PassableGate logic;

	// Token: 0x0400070F RID: 1807
	public Animator open_go;

	// Token: 0x04000710 RID: 1808
	public Animator closed_go;

	// Token: 0x04000711 RID: 1809
	private float next_control_check_time;

	// Token: 0x04000712 RID: 1810
	private bool was_destroyed;
}
