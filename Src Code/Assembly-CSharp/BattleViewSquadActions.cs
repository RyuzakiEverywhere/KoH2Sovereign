using System;
using System.Collections.Generic;
using Logic;

// Token: 0x020001BB RID: 443
public class BattleViewSquadActions : IListener
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06001A38 RID: 6712 RVA: 0x000FD8B0 File Offset: 0x000FBAB0
	// (remove) Token: 0x06001A39 RID: 6713 RVA: 0x000FD8E8 File Offset: 0x000FBAE8
	public event Action<string> SquadActionPerformed;

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06001A3A RID: 6714 RVA: 0x000FD920 File Offset: 0x000FBB20
	// (remove) Token: 0x06001A3B RID: 6715 RVA: 0x000FD958 File Offset: 0x000FBB58
	public event Action SquadStatesChanged;

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06001A3C RID: 6716 RVA: 0x000FD98D File Offset: 0x000FBB8D
	public int SelectedUnits
	{
		get
		{
			return this.m_squads.Count;
		}
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x000FD99A File Offset: 0x000FBB9A
	public void SetSquads(List<Logic.Squad> squads)
	{
		this.RemoveListeners();
		this.m_squads = squads;
		if (this.m_squads == null)
		{
			return;
		}
		if (!this.m_isInitialized)
		{
			this.CreateSquadsStatesLookup();
		}
		else
		{
			this.ResetSquadsStatesLookup();
		}
		this.UpdateSquadStates();
		this.AddListeners();
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x000FD9D4 File Offset: 0x000FBBD4
	public void CallSquadActions(string action)
	{
		if (!this.m_isInitialized || this.m_squads.Count == 0)
		{
			return;
		}
		if (this.AreOurUnitsSelected)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(action);
			if (num <= 1557350270U)
			{
				if (num <= 84037765U)
				{
					if (num != 3119156U)
					{
						if (num == 84037765U)
						{
							if (action == "triangle")
							{
								this.TriangleFormation();
							}
						}
					}
					else if (action == "reset_formation")
					{
						this.ResetFormation();
					}
				}
				else if (num != 400234023U)
				{
					if (num != 1025155545U)
					{
						if (num == 1557350270U)
						{
							if (action == "deploy")
							{
								this.Deploy();
							}
						}
					}
					else if (action == "expand_formation")
					{
						this.ExpandFormation();
					}
				}
				else if (action == "line")
				{
					this.LineFormation();
				}
			}
			else if (num <= 2937586116U)
			{
				if (num != 2650158521U)
				{
					if (num == 2937586116U)
					{
						if (action == "shrink_formation")
						{
							this.ShrinkFormation();
						}
					}
				}
				else if (action == "hold_fire")
				{
					this.HoldFire();
				}
			}
			else if (num != 3234472817U)
			{
				if (num != 3242729310U)
				{
					if (num == 3411225317U)
					{
						if (action == "stop")
						{
							this.Stop();
						}
					}
				}
				else if (action == "hold_ground")
				{
					this.HoldGround();
				}
			}
			else if (action == "charge")
			{
				this.Charge();
			}
		}
		else if (action == "mark_as_target")
		{
			this.MarkAsTarget();
		}
		Action<string> squadActionPerformed = this.SquadActionPerformed;
		if (squadActionPerformed == null)
		{
			return;
		}
		squadActionPerformed(action);
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x000FDBC9 File Offset: 0x000FBDC9
	public Dictionary<string, bool> GetStates()
	{
		return this.m_squadsState;
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x000FDBD4 File Offset: 0x000FBDD4
	public bool GetActionState(string action)
	{
		bool result = false;
		this.m_squadsState.TryGetValue(action, out result);
		return result;
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x000FDBF4 File Offset: 0x000FBDF4
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "fleeing"))
		{
			if (!(message == "fleeing_finished"))
			{
				if (message == "defeated")
				{
					if (this.m_squads.Count == 1)
					{
						this.IsSquadRetreating = false;
						this.IsAnySquadAlive = false;
						Action<string> squadActionPerformed = this.SquadActionPerformed;
						if (squadActionPerformed != null)
						{
							squadActionPerformed("defeated");
						}
					}
					this.UpdateSquadStates();
					return;
				}
				if (!(message == "started_packing") && !(message == "finished_packing"))
				{
					if (!(message == "buffs_changed"))
					{
						return;
					}
					this.UpdateSquadStates();
					Action squadStatesChanged = this.SquadStatesChanged;
					if (squadStatesChanged == null)
					{
						return;
					}
					squadStatesChanged();
				}
				else
				{
					this.UpdateSquadStates();
					Action<string> squadActionPerformed2 = this.SquadActionPerformed;
					if (squadActionPerformed2 == null)
					{
						return;
					}
					squadActionPerformed2("deploy");
					return;
				}
			}
			else if (this.m_squads.Count == 1)
			{
				this.IsSquadRetreating = false;
				Action<string> squadActionPerformed3 = this.SquadActionPerformed;
				if (squadActionPerformed3 == null)
				{
					return;
				}
				squadActionPerformed3("retreating_finished");
				return;
			}
		}
		else if (this.m_squads.Count == 1)
		{
			this.IsSquadRetreating = true;
			Action<string> squadActionPerformed4 = this.SquadActionPerformed;
			if (squadActionPerformed4 == null)
			{
				return;
			}
			squadActionPerformed4("retreating");
			return;
		}
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x000FDD24 File Offset: 0x000FBF24
	private void CreateSquadsStatesLookup()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		this.m_squadsState.Add("shrink_formation", false);
		this.m_squadsState.Add("reset_formation", false);
		this.m_squadsState.Add("expand_formation", false);
		this.m_squadsState.Add("line", false);
		this.m_squadsState.Add("triangle", false);
		this.m_squadsState.Add("hold_fire", false);
		this.m_squadsState.Add("hold_ground", false);
		this.m_squadsState.Add("deploy", false);
		this.m_squadsState.Add("mark_as_target", false);
		this.m_isInitialized = true;
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x000FDDDC File Offset: 0x000FBFDC
	private void ResetSquadsStatesLookup()
	{
		this.m_squadsState["shrink_formation"] = false;
		this.m_squadsState["reset_formation"] = false;
		this.m_squadsState["expand_formation"] = false;
		this.m_squadsState["line"] = false;
		this.m_squadsState["triangle"] = false;
		this.m_squadsState["hold_fire"] = false;
		this.m_squadsState["hold_ground"] = false;
		this.m_squadsState["deploy"] = false;
		this.m_squadsState["mark_as_target"] = false;
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x000FDE84 File Offset: 0x000FC084
	private void AddListeners()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null)
			{
				squad.AddListener(this);
			}
		}
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x000FDEDC File Offset: 0x000FC0DC
	private void RemoveListeners()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null)
			{
				squad.DelListener(this);
			}
		}
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x000FDF34 File Offset: 0x000FC134
	private void UpdateSquadStates()
	{
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = false;
		bool flag11 = false;
		bool flag12 = true;
		bool flag13 = false;
		bool flag14 = false;
		bool flag15 = false;
		bool flag16 = false;
		this.AreRangedUnitsSelected = false;
		this.AreSiegeUnitsOnlySelected = true;
		this.ArePackableUnitsOnlySelected = true;
		this.IsDeployBlocked = false;
		this.IsPackingInProgress = false;
		this.IsDeployingInProgress = false;
		this.AreOurUnitsSelected = true;
		this.AreEnemyUnitsSelected = false;
		this.CanSquadsDeploy = true;
		if (this.m_squads.Count == 1)
		{
			this.IsAnySquadAlive = !this.m_squads[0].IsDefeated();
			this.IsSquadRetreating = (this.m_squads[0].simulation.state == BattleSimulation.Squad.State.Retreating);
		}
		else
		{
			this.IsAnySquadAlive = true;
			this.IsSquadRetreating = false;
		}
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad.spacing == Logic.Squad.Spacing.Shrinken)
			{
				flag = true;
				if (flag2 || flag3)
				{
					flag16 = true;
				}
			}
			else if (squad.spacing == Logic.Squad.Spacing.Default)
			{
				flag2 = true;
				if (flag || flag3)
				{
					flag16 = true;
				}
			}
			else
			{
				flag3 = true;
				if (flag || flag2)
				{
					flag16 = true;
				}
			}
			if (squad.formation is RectFormation)
			{
				flag4 = true;
			}
			else if (squad.formation is TriangleFormation)
			{
				flag5 = true;
			}
			if (squad.can_auto_fire)
			{
				flag7 = true;
			}
			else
			{
				flag6 = true;
			}
			if (squad.stance == Logic.Squad.Stance.Defensive)
			{
				flag8 = true;
			}
			else
			{
				flag9 = true;
			}
			if (squad.double_time)
			{
				flag13 = true;
			}
			else
			{
				flag12 = true;
			}
			if (squad.marked_as_target)
			{
				flag14 = true;
			}
			else
			{
				flag15 = true;
			}
			if (squad.CanBePacked())
			{
				if ((!squad.is_packed && !squad.IsPacking()) || (squad.is_packed && squad.IsPacking()))
				{
					flag10 = true;
				}
				else
				{
					flag11 = true;
				}
				if (squad.is_packed && squad.IsPacking())
				{
					this.IsDeployingInProgress = true;
				}
				else if (!squad.is_packed && squad.IsPacking())
				{
					this.IsPackingInProgress = true;
				}
			}
			if (squad.def.is_ranged)
			{
				this.AreRangedUnitsSelected = true;
			}
			if (!squad.def.is_siege_eq)
			{
				this.AreSiegeUnitsOnlySelected = false;
			}
			if (!squad.CanBePacked())
			{
				this.ArePackableUnitsOnlySelected = false;
			}
			if (!squad.IsOwnStance(obj) && !squad.IsAlly(obj))
			{
				this.AreOurUnitsSelected = false;
			}
			if (squad.IsEnemy(obj))
			{
				this.AreEnemyUnitsSelected = true;
			}
			if (!squad.is_packed)
			{
				this.CanSquadsDeploy = false;
			}
			if (this.AreSiegeUnitsOnlySelected && this.CanSquadsDeploy && !squad.CanShootInTrees())
			{
				this.IsDeployBlocked = true;
			}
		}
		if (!flag16)
		{
			this.m_squadsState["shrink_formation"] = flag;
			this.m_squadsState["reset_formation"] = flag2;
			this.m_squadsState["expand_formation"] = flag3;
		}
		if (flag4 != flag5)
		{
			this.m_squadsState["line"] = flag4;
			this.m_squadsState["triangle"] = flag5;
		}
		if (flag7 != flag6)
		{
			this.m_squadsState["hold_fire"] = flag6;
		}
		if (flag8 != flag9)
		{
			this.m_squadsState["hold_ground"] = flag8;
		}
		if (flag11 != flag10)
		{
			this.m_squadsState["deploy"] = flag10;
		}
		if (flag13 != flag12)
		{
			this.m_squadsState["charge"] = flag13;
		}
		if (flag14 != flag15)
		{
			this.m_squadsState["mark_as_target"] = flag14;
		}
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x000FE2D4 File Offset: 0x000FC4D4
	private void Stop()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating && squad.command != Logic.Squad.Command.Hold)
			{
				squad.Stop(true);
				squad.SetCommand(Logic.Squad.Command.Hold, null);
				global::Squad squad2 = squad.visuals as global::Squad;
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.Stop, squad2.transform.position, "FloatingTexts.Normal");
			}
		}
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x000FE374 File Offset: 0x000FC574
	private void ShrinkFormation()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				float min_spacing = squad2.logic.formation.min_spacing;
				squad2.logic.SetSpacing(Logic.Squad.Spacing.Shrinken, false);
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormShrink, squad2.transform.position, "FloatingTexts.Normal");
			}
		}
		this.m_squadsState["shrink_formation"] = true;
		this.m_squadsState["reset_formation"] = false;
		this.m_squadsState["expand_formation"] = false;
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x000FE450 File Offset: 0x000FC650
	private void ResetFormation()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				squad2.logic.SetSpacing(Logic.Squad.Spacing.Default, false);
				if (this.m_squadsState["expand_formation"])
				{
					squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormShrink, squad2.transform.position, "FloatingTexts.Normal");
				}
				if (this.m_squadsState["shrink_formation"])
				{
					squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormWiden, squad2.transform.position, "FloatingTexts.Normal");
				}
			}
		}
		this.m_squadsState["shrink_formation"] = false;
		this.m_squadsState["reset_formation"] = true;
		this.m_squadsState["expand_formation"] = false;
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x000FE560 File Offset: 0x000FC760
	private void ExpandFormation()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				squad2.logic.SetSpacing(Logic.Squad.Spacing.Expanded, false);
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormWiden, squad2.transform.position, "FloatingTexts.Normal");
			}
		}
		this.m_squadsState["shrink_formation"] = false;
		this.m_squadsState["reset_formation"] = false;
		this.m_squadsState["expand_formation"] = true;
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000FE62C File Offset: 0x000FC82C
	private void LineFormation()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				if (!squad2.def.is_defense)
				{
					squad2.logic.SetFormation("Rect");
				}
				else
				{
					squad2.logic.SetFormation("Checkerboard");
				}
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormLine, squad2.transform.position, "FloatingTexts.Normal");
				FormationPool.Return(ref squad2.previewFormation);
				FormationPool.Return(ref squad2.previewFormationConfirmed);
				squad2.previewFormation = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
				squad2.previewFormationConfirmed = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
			}
		}
		this.m_squadsState["line"] = true;
		this.m_squadsState["triangle"] = false;
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000FE76C File Offset: 0x000FC96C
	private void TriangleFormation()
	{
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				squad2.logic.SetFormation("Triangle");
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.FormTriangle, squad2.transform.position, "FloatingTexts.Normal");
				FormationPool.Return(ref squad2.previewFormation);
				FormationPool.Return(ref squad2.previewFormationConfirmed);
				squad2.previewFormation = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
				squad2.previewFormationConfirmed = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
			}
		}
		this.m_squadsState["line"] = false;
		this.m_squadsState["triangle"] = true;
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000FE890 File Offset: 0x000FCA90
	private void HoldFire()
	{
		if (!this.m_isInitialized || this.m_squads.Count == 0)
		{
			return;
		}
		bool flag = this.m_squadsState["hold_fire"];
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				squad.can_auto_fire = flag;
			}
		}
		if (!flag)
		{
			BaseUI.PlayVoiceEvent(this.m_squads[0].def.hold_fire_voice_line, this.m_squads[0], global::Common.SnapToTerrain(this.m_squads[0].VisualPosition(), 0f, null, -1f, false));
		}
		else
		{
			BaseUI.PlayVoiceEvent(this.m_squads[0].def.allow_fire_voice_line, this.m_squads[0], global::Common.SnapToTerrain(this.m_squads[0].VisualPosition(), 0f, null, -1f, false));
		}
		this.m_squadsState["hold_fire"] = !flag;
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000FE9D8 File Offset: 0x000FCBD8
	private void HoldGround()
	{
		bool flag = this.m_squadsState["hold_ground"];
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				if (!flag)
				{
					squad.SetStance(Logic.Squad.Stance.Defensive);
				}
				else
				{
					squad.SetStance(Logic.Squad.Stance.Aggressive);
				}
			}
		}
		if (!flag)
		{
			BaseUI.PlayVoiceEvent(this.m_squads[0].def.stand_ground_voice_line, this.m_squads[0], global::Common.SnapToTerrain(this.m_squads[0].VisualPosition(), 0f, null, -1f, false));
		}
		else
		{
			BaseUI.PlayVoiceEvent(this.m_squads[0].def.at_ease_voice_line, this.m_squads[0], global::Common.SnapToTerrain(this.m_squads[0].VisualPosition(), 0f, null, -1f, false));
		}
		this.m_squadsState["hold_ground"] = !flag;
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000FEB18 File Offset: 0x000FCD18
	private void Deploy()
	{
		bool flag = !this.m_squadsState["deploy"];
		bool flag2 = false;
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.CanBePacked() && !squad.IsPacking() && squad.simulation.state != BattleSimulation.Squad.State.Retreating)
			{
				flag2 = true;
				if (squad.is_packed == flag)
				{
					squad.StartPack();
					if (flag)
					{
						this.IsDeployingInProgress = true;
					}
					else
					{
						this.IsPackingInProgress = true;
					}
				}
			}
		}
		if (flag2)
		{
			this.m_squadsState["deploy"] = flag;
		}
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000FEBDC File Offset: 0x000FCDDC
	private void Charge()
	{
		if (this.m_squads.Count == 0)
		{
			return;
		}
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.simulation.state != BattleSimulation.Squad.State.Retreating && squad.IsMoving() && squad.SetDoubleTime(true, true))
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				squad2.PlayVoiceLine(global::Squad.VoiceCommand.Charge, squad2.transform.position, "FloatingTexts.Normal");
			}
		}
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000FEC84 File Offset: 0x000FCE84
	private void MarkAsTarget()
	{
		if (this.m_squads.Count == 0)
		{
			return;
		}
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		bool flag = !this.m_squadsState["mark_as_target"];
		foreach (Logic.Squad squad in this.m_squads)
		{
			if (squad != null && squad.visuals != null && squad.IsEnemy(obj))
			{
				squad.marked_as_target = flag;
			}
		}
		this.m_squadsState["mark_as_target"] = flag;
	}

	// Token: 0x040010D0 RID: 4304
	public bool AreRangedUnitsSelected;

	// Token: 0x040010D1 RID: 4305
	public bool AreSiegeUnitsOnlySelected;

	// Token: 0x040010D2 RID: 4306
	public bool ArePackableUnitsOnlySelected;

	// Token: 0x040010D3 RID: 4307
	public bool CanSquadsDeploy;

	// Token: 0x040010D4 RID: 4308
	public bool IsDeployBlocked;

	// Token: 0x040010D5 RID: 4309
	public bool IsPackingInProgress;

	// Token: 0x040010D6 RID: 4310
	public bool IsDeployingInProgress;

	// Token: 0x040010D7 RID: 4311
	public bool AreOurUnitsSelected;

	// Token: 0x040010D8 RID: 4312
	public bool AreEnemyUnitsSelected;

	// Token: 0x040010D9 RID: 4313
	public bool IsAnySquadAlive;

	// Token: 0x040010DA RID: 4314
	public bool IsSquadRetreating;

	// Token: 0x040010DB RID: 4315
	private List<Logic.Squad> m_squads = new List<Logic.Squad>();

	// Token: 0x040010DC RID: 4316
	private Dictionary<string, bool> m_squadsState = new Dictionary<string, bool>();

	// Token: 0x040010DD RID: 4317
	private bool m_isInitialized;
}
