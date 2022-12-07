using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Logic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public static class Troops
{
	// Token: 0x06000A44 RID: 2628 RVA: 0x00075A3C File Offset: 0x00073C3C
	public unsafe static void StartAllJobs(float dt)
	{
		Troops.pdata->dt = dt;
		Game batte_view_game = BattleMap.battle.batte_view_game;
		if (!Troops.path_data_assigned)
		{
			if (batte_view_game != null && batte_view_game.path_finding != null && batte_view_game.path_finding.data != null && batte_view_game.path_finding.data.initted && *batte_view_game.path_finding.data.pointers.Initted)
			{
				Troops.path_data = batte_view_game.path_finding.data.pointers;
				Troops.path_data_assigned = true;
				BattleMap.battle.InitAI();
			}
			else if (!SettlementBV.generating)
			{
				BattleMap battleMap = BattleMap.Get();
				PassableAreaManager passableAreaManager = (battleMap != null) ? battleMap.paManager : null;
				if (passableAreaManager != null)
				{
					passableAreaManager.RecalculateAreas();
				}
				return;
			}
		}
		Troops.pdata->winner = BattleMap.battle.winner;
		HeightsGrid.Data* data = batte_view_game.heights.data;
		if (Troops.NotReady)
		{
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.InitAction));
			Troops.ScheduleAction(new Action(Troops.UpdateSquadFormations));
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.CalcFormationAction));
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.CalcTroopsHoldFormationVec));
			Troops.CompleteAllJobs();
			Troops.SchedulePerTroopJob_PushOffUpdateJob(true, Troops.AllSquads, Troops.PushedOffTroops, new Troops.PushOffUpdateJob
			{
				pointers = Troops.path_data,
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_AdjustTroopTargetsJob(true, Troops.AllSquads, Troops.AliveNotPushedOffNotClimbingTroops, new Troops.AdjustTroopTargetsJob
			{
				pointers = Troops.path_data
			});
			Troops.SchedulePerTroopJob_CalcTroopVelocities(true, Troops.AllSquads, Troops.AliveNotPushedOffNotClimbingTroops, new Troops.CalcTroopVelocities
			{
				pointers = Troops.path_data,
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_TroopsObstacleAvoidanceJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.TroopsObstacleAvoidanceJob
			{
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_MoveTroopsJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.MoveTroopsJob
			{
				collisions = Troops.collisions.read_only,
				pointers = Troops.path_data
			});
			Troops.SchedulePerTroopJob_SnapTroopsJob(true, Troops.VisibleSquads, Troops.AllTroops, new Troops.SnapTroopsJob
			{
				heights = data,
				pointers = Troops.path_data
			});
			Troops.ScheduleJob<Troops.ClearCollisionsGridJob>(new Troops.ClearCollisionsGridJob
			{
				collisions = Troops.collisions
			});
			Troops.SchedulePerTroopJob_RebuildCollisionsGridJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.RebuildCollisionsGridJob
			{
				collisions = Troops.collisions.concurrent
			});
			Troops.SchedulePerTroopJob_UpdateTroopRotationsJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.UpdateTroopRotationsJob
			{
				pointers = Troops.path_data
			});
			Troops.SchedulePerTroopJob_PushOffJob(true, Troops.ActiveSquads, Troops.ChargingTroops, new Troops.PushOffJob
			{
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerArrowJob_MoveArrowsJob(Troops.MovingSalvos, Troops.MovingArrows, new Troops.MoveArrowsJob
			{
				collisions = Troops.collisions.read_only,
				fort_collisions = Troops.fort_collisions.read_only,
				pointers = Troops.path_data
			});
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.FinishAction));
		}
		else if (!BattleMap.battle.IsFinishing() && !BattleMap.battle.battle_map_finished)
		{
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.InitAction));
			Troops.ScheduleAction(new Action(Troops.UpdateSquadFormations));
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.CalcFormationAction));
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.CalcTroopsHoldFormationVec));
			Troops.SchedulePerSquadAction(true, Troops.ActiveSquads, new Troops.SquadAction(Troops.FindAccessibleEnemySquadsInRange));
			Troops.CompleteAllJobs();
			Troops.SchedulePerTroopJob_ClimbLadderProgressJob(true, Troops.AllSquads, Troops.ClimbingTroops, default(Troops.ClimbLadderProgressJob));
			Troops.SchedulePerTroopJob_FindTroopEnemiesJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.FindTroopEnemiesJob
			{
				pointers = Troops.path_data,
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_PushOffUpdateJob(true, Troops.AllSquads, Troops.PushedOffTroops, new Troops.PushOffUpdateJob
			{
				pointers = Troops.path_data,
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_AdjustTroopTargetsJob(true, Troops.AllSquads, Troops.AliveNotPushedOffNotClimbingTroops, new Troops.AdjustTroopTargetsJob
			{
				pointers = Troops.path_data
			});
			Troops.SchedulePerTroopJob_CalcTroopVelocities(true, Troops.AllSquads, Troops.AliveNotPushedOffNotClimbingTroops, new Troops.CalcTroopVelocities
			{
				pointers = Troops.path_data,
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_TroopsObstacleAvoidanceJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.TroopsObstacleAvoidanceJob
			{
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_MoveTroopsJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.MoveTroopsJob
			{
				collisions = Troops.collisions.read_only,
				pointers = Troops.path_data
			});
			Troops.SchedulePerTroopJob_SnapTroopsJob(true, Troops.VisibleSquads, Troops.AllTroops, new Troops.SnapTroopsJob
			{
				heights = data,
				pointers = Troops.path_data
			});
			Troops.ScheduleJob<Troops.ClearCollisionsGridJob>(new Troops.ClearCollisionsGridJob
			{
				collisions = Troops.collisions
			});
			Troops.SchedulePerTroopJob_RebuildCollisionsGridJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.RebuildCollisionsGridJob
			{
				collisions = Troops.collisions.concurrent
			});
			Troops.SchedulePerTroopJob_UpdateTroopRotationsJob(true, Troops.AllSquads, Troops.AliveTroops, new Troops.UpdateTroopRotationsJob
			{
				pointers = Troops.path_data,
				heights = data
			});
			Troops.SchedulePerTroopJob_PushOffJob(true, Troops.ActiveSquads, Troops.ChargingTroops, new Troops.PushOffJob
			{
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerTroopJob_ChargeJob(true, Troops.ActiveSquads, Troops.ChargingTroops, new Troops.ChargeJob
			{
				collisions = Troops.collisions.read_only
			});
			Troops.SchedulePerArrowJob_MoveArrowsJob(Troops.MovingSalvos, Troops.MovingArrows, new Troops.MoveArrowsJob
			{
				collisions = Troops.collisions.read_only,
				fort_collisions = Troops.fort_collisions.read_only,
				pointers = Troops.path_data
			});
			Troops.SchedulePerSquadJob_CalcKilledTroopsJob(true, Troops.AllSquads, default(Troops.CalcKilledTroopsJob));
			Troops.SchedulePerSquadAction(true, Troops.AllSquads, new Troops.SquadAction(Troops.FinishAction));
		}
		else
		{
			Troops.CompleteAllJobs();
			Troops.SchedulePerArrowJob_MoveArrowsJob(Troops.MovingSalvos, Troops.MovingArrows, new Troops.MoveArrowsJob
			{
				collisions = Troops.collisions.read_only,
				fort_collisions = Troops.fort_collisions.read_only,
				pointers = Troops.path_data
			});
		}
		Troops.SchedulePerSquadAction(true, Troops.ActiveSquads, new Troops.SquadAction(Troops.CalcAnimationCounts));
		Troops.SchedulePerTroopJob_UpdateTroopAnimationsJob(true, Troops.AllSquads, Troops.AllTroops, new Troops.UpdateTroopAnimationsJob
		{
			heights = data
		});
		Troops.SchedulePerArrowJob_ArrowTrailsJob(Troops.AllSalvos, Troops.AllArrows, default(Troops.ArrowTrailsJob));
		Troops.ScheduleAction(new Action(Troops.ClearModelDataBuffer));
		Troops.SchedulePerTroopJob_DustParticlesJob(true, Troops.AllSquads, Troops.AllTroops, default(Troops.DustParticlesJob));
		Troops.SchedulePerTroopJob_FillTroopRenderDataJob(true, Troops.AllSquads, Troops.AllTroops, new Troops.FillTroopRenderDataJob
		{
			f_planes = (float4*)Troops.f_planes.GetUnsafePtr<float4>()
		});
		Troops.SchedulePerArrowJob_FillArrowRenderDataJob(Troops.StartedSalvos, Troops.StartedArrows, new Troops.FillArrowRenderDataJob
		{
			f_planes = (float4*)Troops.f_planes.GetUnsafePtr<float4>()
		});
		Troops.StartScheduledJobs();
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00076204 File Offset: 0x00074404
	public static void OnUpdate()
	{
		using (Game.Profile("Troops.OnUpdate", false, 0f, null))
		{
			if (Troops.Initted)
			{
				Troops.StartAllJobs(UnityEngine.Time.deltaTime);
			}
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00076258 File Offset: 0x00074458
	public unsafe static void OnLateUpdate()
	{
		if (!Troops.Initted)
		{
			return;
		}
		Troops.CompleteAllJobs();
		Troops.UpdateCameraStats();
		bool flag = !Troops.NotReady;
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			global::Squad squad = Troops.squads[i];
			if (((squad != null) ? squad.logic : null) != null && squad.logic.IsValid())
			{
				Troops.CalcStats(squad);
				squad.UpdateInsideWallsValues();
				squad.RefreshVisibility();
				squad.UpdateArrowRangeGO();
				squad.UpdateSoundLoop(false);
				if (flag)
				{
					Troops.UpdateEnemySquads(squad);
					Troops.SpawnRagdolls(squad);
					if (Troops.CheckDefeated(squad, true))
					{
						goto IL_E3;
					}
					Troops.CheckSplitSquad(squad);
					if (Troops.CheckSubSquad(squad))
					{
						goto IL_E3;
					}
				}
				Troops.StopSquad(squad);
				Troops.MoveSquad(squad);
				Troops.RotateSquad(squad);
				squad.cur_salvo = squad.data->cur_salvo;
				if (squad.data != null)
				{
					squad.logic.move_history = squad.data->move_history.ToArray(squad.logic.move_history);
				}
			}
			IL_E3:;
		}
		Troops.DrawSquads();
		Troops.UpdateDecals();
		Troops.UpdateLadders();
		for (int j = 0; j < Troops.pdata->NumSalvos; j++)
		{
			Troops.SalvoData* salvo = Troops.pdata->GetSalvo(j);
			if (salvo->HasFlags(Troops.SalvoData.Flags.Active))
			{
				bool flag2 = true;
				Troops.Arrow arrow = salvo->FirstArrow;
				while (arrow <= salvo->LastArrow)
				{
					if (!arrow.HasFlags(Troops.Arrow.Flags.Destroyed))
					{
						flag2 = false;
						break;
					}
					arrow = ++arrow;
				}
				if (flag2)
				{
					salvo->SetFlags(Troops.SalvoData.Flags.Destroyed);
				}
				else
				{
					Troops.DrawSalvo(salvo);
				}
			}
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x000763E4 File Offset: 0x000745E4
	private static void UpdateSquadFormations()
	{
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			global::Squad squad = Troops.squads[i];
			if (!(squad == null) && squad.logic != null && squad.logic.IsValid() && (squad.data_formation == null || squad.data_formation.def != squad.logic.formation.def))
			{
				FormationPool.Return(ref squad.data_formation);
				squad.data_formation = FormationPool.Get(squad.logic.formation.def, squad.logic);
				squad.data_formation.allowed_areas = squad.logic.movement.allowed_non_ladder_areas;
				squad.data_formation.battle_side = squad.logic.battle_side;
				squad.data_formation.is_inside_wall = squad.logic.is_inside_walls_or_on_walls;
			}
		}
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x000764D0 File Offset: 0x000746D0
	private static void DrawSquads()
	{
		GameCamera gameCamera = CameraController.GameCamera;
		if (((gameCamera != null) ? gameCamera.Camera : null) == null)
		{
			return;
		}
		TextureBaker.InstancedDecalDrawerBatched decal_drawer = Troops.texture_baker.decal_drawer;
		if (decal_drawer != null)
		{
			decal_drawer.FillBuffer(gameCamera.f_planes);
		}
		Troops.texture_baker.Draw(gameCamera.Camera);
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x00076523 File Offset: 0x00074723
	private static void UpdateCameraStats()
	{
		Troops.CalcFrustrum();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0007652C File Offset: 0x0007472C
	private unsafe static bool CheckSubSquad(global::Squad squad)
	{
		if (squad.GetMainSquadID() == -1 || squad.logic == null || squad.logic.climbing || squad.data_formation == null || (squad.def != null && squad.def.size == 1))
		{
			return false;
		}
		global::Squad squad2 = Troops.GetSquad(squad.GetMainSquadID());
		PPos ppos;
		if (squad2.logic.position.Dist(squad.logic.position) < squad2.data_formation.cur_radius && !squad2.logic.climbing && Troops.path_data.Trace(squad2.logic.position, squad.logic.position, out ppos, false, true, false, squad.logic.movement.allowed_area_types, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls) && ppos.paID == squad.logic.position.paID)
		{
			squad.MergeWithMainSquad();
			return true;
		}
		PPos pos = squad.data->pos;
		for (int i = 0; i < squad2.subsquads_data_ids.Count; i++)
		{
			if (squad2.subsquads_data_ids[i] != squad.GetID())
			{
				global::Squad squad3 = Troops.GetSquad(squad2.subsquads_data_ids[i]);
				if (!(squad3 == null) && squad3.logic != null && squad3.data_formation != null && squad3.logic.IsValid())
				{
					PPos pos2 = squad3.data->pos;
					float num = math.max(squad.data_formation.cur_radius, 4f);
					PPos ppos2;
					if (pos2.Dist(pos) < num && Troops.path_data.Trace(pos, pos2, out ppos2, false, true, false, squad.logic.movement.allowed_non_ladder_areas, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls) && ppos2.paID == pos2.paID && !squad3.logic.climbing)
					{
						squad.MergeSubSquads(squad3);
					}
				}
			}
		}
		if (squad.GetMainSquadID() != -1 && squad.data->stuck_recalc_attempts > 1 && squad.logic.movement.path != null && squad.logic.movement.pf_path == null)
		{
			Troops.Troop troop = squad.data->FirstTroop;
			while (troop <= squad.data->LastTroop)
			{
				if (troop.squad_id == squad.GetID() && !troop.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Thrown | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed | Troops.Troop.Flags.PushedOff) && !troop.squad->HasFlags(Troops.SquadData.Flags.Fled))
				{
					squad.logic.SetPosition(troop.pos);
					break;
				}
				troop = ++troop;
			}
			squad.logic.Stop(true);
		}
		return false;
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x000767FC File Offset: 0x000749FC
	private unsafe static void CheckSplitSquad(global::Squad squad)
	{
		using (Game.Profile("Troops.CheckSplitSquad", false, 0f, null))
		{
			if (squad.GetMainSquadID() == -1 && squad.data_formation != null && (squad.def == null || squad.def.size != 1))
			{
				if (squad.logic != null && !squad.logic.climbing)
				{
					if (!squad.logic.battle.IsFinishing())
					{
						Troops.SquadData* data = squad.data;
						if (!data->HasFlags(Troops.SquadData.Flags.Teleport))
						{
							Logic.Squad logic = squad.logic;
							Troops.SquadData.Ptr* ptr = Troops.pdata->squad + data->offset;
							List<int> list = new List<int>();
							PPos ppos = default(PPos);
							Troops.Troop troop = default(Troops.Troop);
							float num = math.max(squad.data_formation.cur_radius, 4f);
							int num2 = squad.GetSquadAliveTroops();
							bool flag = logic.command == Logic.Squad.Command.Disengage;
							if (data->logic_alive == 1)
							{
								Troops.Troop troop2 = data->FirstTroop;
								while (troop2 <= data->LastTroop)
								{
									if (!troop2.HasFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Thrown | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed | Troops.Troop.Flags.PushedOff) && !troop2.squad->HasFlags(Troops.SquadData.Flags.HasTroopClimbedLadder) && troop2.HasFlags(Troops.Troop.Flags.Stuck))
									{
										logic.Teleport(troop2.pos);
										return;
									}
									troop2 = ++troop2;
									ptr++;
								}
							}
							float num3 = 4f;
							Troops.Troop troop3 = data->FirstTroop;
							while (troop3 <= data->LastTroop)
							{
								if (!troop3.HasFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Thrown | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed | Troops.Troop.Flags.PushedOff) && !troop3.squad->HasFlags(Troops.SquadData.Flags.HasTroopClimbedLadder))
								{
									global::Squad squad2 = Troops.GetSquad(troop3.squad_id);
									if (((squad2 != null) ? squad2.logic : null) != null && squad2.logic.simulation.state != BattleSimulation.Squad.State.Stuck)
									{
										if (troop3.HasFlags(Troops.Troop.Flags.Stuck))
										{
											PPos ppos2;
											if (((!Troops.path_data.Trace(troop3.tgt_pos, troop3.pos, out ppos2, false, true, false, squad.logic.movement.allowed_non_ladder_areas, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls) || ppos2.paID != troop3.pa_id) && troop3.pos.Dist(troop3.tgt_pos) >= num) || troop3.squad->stuck_recalc_attempts > 1 || troop3.pos.Dist(squad.logic.position) > num * num3)
											{
												bool flag2 = true;
												if (squad.subsquads_data_ids.Count > 0)
												{
													flag2 = Troops.ValidateTroopSplitToSubSquad(squad, ptr, troop3, ref num2);
												}
												if (flag2)
												{
													if (troop3.squad_id == squad.GetID())
													{
														if (num2 <= 1)
														{
															goto IL_499;
														}
														num2--;
													}
													if (list.Count == 0)
													{
														if (troop3.pos.SqrDist(ppos2) < 0.1f && troop3.pos.paID == 0 && ppos2.paID != 0 && Troops.path_data.PointInArea(troop3.pos, ppos2.paID, PathData.PassableArea.Type.All))
														{
															troop3.pa_id = troop3.tgt_pos.paID;
														}
														ppos = troop3.pos;
														troop = troop3;
														list.Add(troop3.id);
													}
													else if (Troops.path_data.Trace(troop3.pos, ppos, out ppos2, false, true, false, squad.logic.movement.allowed_area_types, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls))
													{
														list.Add(troop3.id);
													}
												}
											}
										}
										else if (flag && troop3.HasFlags(Troops.Troop.Flags.Fighting) && data->troop_too_far && troop3.pos.Dist(squad.logic.position) > num * num3)
										{
											bool flag3 = true;
											if (squad.subsquads_data_ids.Count > 0)
											{
												flag3 = Troops.ValidateTroopSplitToSubSquad(squad, ptr, troop3, ref num2);
											}
											if (flag3)
											{
												if (troop3.squad_id == squad.GetID())
												{
													if (num2 <= 1)
													{
														goto IL_499;
													}
													num2--;
												}
												PPos ppos2;
												if (list.Count == 0)
												{
													ppos = troop3.pos;
													troop = troop3;
													list.Add(troop3.id);
												}
												else if (Troops.path_data.Trace(troop3.pos, ppos, out ppos2, false, true, false, squad.logic.movement.allowed_area_types, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls))
												{
													list.Add(troop3.id);
												}
											}
										}
									}
								}
								IL_499:
								troop3 = ++troop3;
								ptr++;
							}
							PPos ppos3;
							if (list.Count > 0 && (!Troops.path_data.Trace(squad.logic.position, ppos, out ppos3, false, true, false, squad.logic.movement.allowed_non_ladder_areas, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls) || ppos3.paID != ppos.paID || (troop.squad->stuck_recalc_attempts > 1 && troop.squad->HasFlags(Troops.SquadData.Flags.Moving)) || (ppos.paID > 0 && !Troops.path_data.PointInArea(ppos, ppos.paID, PathData.PassableArea.Type.All)) || (flag && data->troop_too_far)))
							{
								if (ppos3.paID > 0 && ppos.paID != ppos3.paID && Troops.path_data.PointInArea(ppos, ppos3.paID, PathData.PassableArea.Type.All))
								{
									ppos.paID = ppos3.paID;
								}
								squad.CreateSubSquad(list.ToArray(), ppos);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00076DF4 File Offset: 0x00074FF4
	private unsafe static bool ValidateTroopSplitToSubSquad(global::Squad main_squad, Troops.SquadData.Ptr* pSquad, Troops.Troop troop, ref int main_squad_troops_count)
	{
		bool result = true;
		for (int i = 0; i < main_squad.subsquads_data_ids.Count; i++)
		{
			global::Squad squad = Troops.GetSquad(main_squad.subsquads_data_ids[i]);
			if (squad.logic != null && !squad.logic.IsDefeated() && squad.data_formation != null)
			{
				if (troop.squad == squad.data && troop.squad->logic_alive <= 1)
				{
					result = false;
					break;
				}
				float num = math.max(squad.data_formation.cur_radius, 3f);
				PPos ppos;
				if (troop.pos.Dist(squad.logic.position) <= num && Troops.path_data.Trace(troop.pos, squad.logic.position, out ppos, false, true, false, squad.logic.movement.allowed_area_types, squad.logic.battle_side, squad.logic.is_inside_walls_or_on_walls))
				{
					if (troop.squad != main_squad.data)
					{
						troop.squad->logic_alive--;
						global::Squad squad2 = Troops.GetSquad(troop.squad->id);
						if (squad2 != null && squad2.data_formation != null)
						{
							squad2.data_formation.dirty = true;
						}
					}
					else
					{
						if (main_squad_troops_count <= 1)
						{
							result = false;
							break;
						}
						main_squad_troops_count--;
					}
					pSquad->ptr = squad.data;
					troop.squad->logic_alive++;
					squad.data_formation.dirty = true;
					result = false;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00076F94 File Offset: 0x00075194
	public unsafe static void CalcStats(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Logic.Squad logic = squad.logic;
		if (logic == null || !logic.IsValid())
		{
			return;
		}
		data->CTH_final = logic.battle.simulation.def.global_cth_mod * logic.simulation.CTH_Modified(null);
		data->defense_final = logic.simulation.defense_Modified(null);
		data->defense_against_ranged_final = logic.simulation.defense_against_ranged_Modified();
		data->trample_chance_final = logic.simulation.unit.trample_chance_modified();
		data->CTH_against_me_mod = logic.simulation.CTH_against_me_Modified();
		data->resiliance_total = logic.simulation.resilience_total();
		data->shock_damage_base = logic.simulation.unit.shock_damage_base();
		data->shock_damage_bonus_trample = logic.simulation.unit.shock_damage_bonus_trample();
		data->shock_chance = logic.simulation.chance_to_shock_Modified();
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0007707C File Offset: 0x0007527C
	public unsafe static void InitAction(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Logic.Squad logic = squad.logic;
		if (logic == null || !logic.IsValid())
		{
			return;
		}
		data->selected = squad.Selected;
		data->highlighted = (squad.Highlighted || squad.subsquad_highlighted);
		if (squad.GetMainSquadID() != -1 && Troops.GetSquad(squad.GetMainSquadID()).subsquad_highlighted)
		{
			data->highlighted = true;
		}
		data->in_drag = squad.InDrag;
		squad.InDrag = false;
		data->target_previewed = squad.TargetPreviewed;
		data->previewed = squad.Previewed;
		RelationUtils.Stance stance = logic.GetStance(BaseUI.LogicKingdom());
		if ((stance & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
		{
			bool flag;
			if (logic == null)
			{
				flag = (null != null);
			}
			else
			{
				BattleSimulation.Squad simulation = logic.simulation;
				if (simulation == null)
				{
					flag = (null != null);
				}
				else
				{
					Logic.Unit unit = simulation.unit;
					if (unit == null)
					{
						flag = (null != null);
					}
					else
					{
						Logic.Army army = unit.army;
						flag = (((army != null) ? army.mercenary : null) != null);
					}
				}
			}
			if (!flag)
			{
				if (data->highlighted && !data->selected)
				{
					data->stance_to_player = TextureBaker.StanceColors.OwnHighlighted;
					goto IL_13A;
				}
				data->stance_to_player = TextureBaker.StanceColors.Own;
				goto IL_13A;
			}
		}
		if ((stance & RelationUtils.Stance.War) != RelationUtils.Stance.None)
		{
			if (data->highlighted && !data->selected)
			{
				data->stance_to_player = TextureBaker.StanceColors.EnemyHighlighted;
			}
			else
			{
				data->stance_to_player = TextureBaker.StanceColors.Enemy;
			}
		}
		else if (data->highlighted && !data->selected)
		{
			data->stance_to_player = TextureBaker.StanceColors.AllyHighlighted;
		}
		else
		{
			data->stance_to_player = TextureBaker.StanceColors.Ally;
		}
		IL_13A:
		data->is_inside_wall = logic.is_inside_walls_or_on_walls;
		if (logic.simulation.army != null && logic.simulation.army.realm_in != null)
		{
			int id = logic.simulation.army.realm_in.id;
		}
		data->defense_against_trample_mod = logic.battle.simulation.def.defense_against_trample_mod;
		if (logic.spacing == Logic.Squad.Spacing.Shrinken)
		{
			data->avoid_trample_tight_formation_mod = logic.battle.simulation.def.avoid_trample_tight_formation_mod;
		}
		else
		{
			data->avoid_trample_tight_formation_mod = 1f;
		}
		data->CTH_against_cav_charge_mod = logic.battle.simulation.def.CTH_against_cav_charge_mod;
		data->aggressive_base_flank_angle = logic.battle.simulation.def.aggressive_base_flank_angle;
		data->aggressive_additional_flank_angle_cavalry = logic.battle.simulation.def.aggressive_additional_flank_angle_cavalry;
		data->aggressive_additional_flank_angle_infantry = logic.battle.simulation.def.aggressive_additional_flank_angle_infantry;
		data->defensive_base_flank_angle = logic.battle.simulation.def.defensive_base_flank_angle;
		data->defensive_additional_flank_angle_cavalry = logic.battle.simulation.def.defensive_additional_flank_angle_cavalry;
		data->defensive_additional_flank_angle_infantry = logic.battle.simulation.def.defensive_additional_flank_angle_infantry;
		if (data->shock_chance != logic.last_chance_to_shock)
		{
			logic.last_chance_to_shock = data->shock_chance;
			logic.stats_dirty = true;
		}
		data->salvos_left = logic.salvos_left;
		data->hold_fire = !logic.can_auto_fire;
		PPos pt;
		PPos dir;
		logic.CalcPos(out pt, out dir, 0f, 0f, false);
		PPos pos = data->pos;
		PPos ppos;
		if (logic.movement.path != null && !Troops.path_data.Trace(pos, pt, out ppos, false, true, false, logic.movement.allowed_area_types, logic.battle_side, logic.is_inside_walls_or_on_walls))
		{
			float moveTime = logic.GetMoveTime();
			int num = logic.movement.path.FindSegment(logic.movement.path.t);
			int num2 = logic.movement.path.FindSegment(moveTime);
			Path.Segment segment = logic.movement.path.segments[num2];
			while (num2 > num && !Troops.path_data.Trace(pos, segment.pt, out ppos, false, true, false, logic.movement.allowed_area_types, logic.battle_side, logic.is_inside_walls_or_on_walls))
			{
				num2--;
				segment = logic.movement.path.segments[num2];
			}
			pt = segment.pt;
			dir = segment.dir;
		}
		bool flag2 = pt != pos && !data->HasFlags(Troops.SquadData.Flags.Teleport);
		data->pos = pt;
		data->tgtPos = logic.destination;
		if (logic.movement.path != null)
		{
			data->path_len = logic.movement.path.path_len;
		}
		data->move_speed = logic.last_move_speed;
		if (logic.def.is_siege_eq)
		{
			logic.siege_health = data->health;
		}
		Formation.Line line = default(Formation.Line);
		if (squad.data_formation != null && squad.data_formation.lines.Count > 0)
		{
			line = squad.data_formation.lines[0];
		}
		if (!logic.game.path_finding.data.processing)
		{
			Troops.UpdateInvalidMoveHistoryPoints(data, logic, pt, logic.GetMoveTime(), data->move_history.Get(1), data->move_history.GetPathT(1), line);
			data->move_history.Add(pt, dir, logic.GetMoveTime(), logic.game.passability.data, logic.game.path_finding.data.pointers, line, false);
		}
		Logic.Fortification enemy_melee_fortification = logic.enemy_melee_fortification;
		if (enemy_melee_fortification != null)
		{
			global::Fortification fortification = enemy_melee_fortification.visuals as global::Fortification;
			data->enemy_fortification.ptr = fortification.data;
		}
		else
		{
			data->enemy_fortification.ptr = null;
		}
		data->SetFlags(Troops.SquadData.Flags.Visible);
		bool flag3 = data->HasFlags(Troops.SquadData.Flags.Fighting);
		data->SetFlags(Troops.SquadData.Flags.Fled, logic.simulation.state == BattleSimulation.Squad.State.Fled || logic.simulation.state == BattleSimulation.Squad.State.Stuck);
		data->SetFlags(Troops.SquadData.Flags.Moving, flag2 || logic.IsMoving());
		data->SetFlags(Troops.SquadData.Flags.Alert, flag3 || logic.enemy_squad != null);
		data->SetFlags(Troops.SquadData.Flags.HadActiveTroops, data->HasFlags(Troops.SquadData.Flags.HasActiveTroops));
		data->ClrFlags(Troops.SquadData.Flags.HasActiveTroops);
		ref Troops.SquadData ptr = ref *data;
		Troops.SquadData.Flags mask = Troops.SquadData.Flags.Retreating;
		bool flag4;
		if (logic == null)
		{
			flag4 = (null != null);
		}
		else
		{
			Movement movement = logic.movement;
			flag4 = (((movement != null) ? movement.path : null) != null);
		}
		ptr.SetFlags(mask, flag4 && logic.movement.path.flee);
		data->SetFlags(Troops.SquadData.Flags.Charging, logic != null && logic.HasCharge());
		if (data->HasFlags(Troops.SquadData.Flags.Moving))
		{
			data->last_moving_time = logic.game.time_f;
		}
		data->was_moving_lately = (logic.game.time_f - data->last_moving_time < 3f);
		logic.is_fighting_target = data->is_Fighting_Target;
		data->was_Fighting_Target = data->is_Fighting_Target;
		data->is_Fighting_Target = false;
		if (data->is_Fighting)
		{
			if (data->HasFlags(Troops.SquadData.Flags.Fled) || logic.command == Logic.Squad.Command.Disengage)
			{
				logic.is_fighting = false;
				data->is_Fighting = false;
			}
			else if (flag3)
			{
				data->last_Fight_Time = logic.game.time_f;
			}
			else if (data->last_Fight_Time + 1f < logic.game.time_f)
			{
				logic.is_fighting = flag3;
				data->is_Fighting = flag3;
			}
		}
		else if (flag3 && !data->HasFlags(Troops.SquadData.Flags.Fled) && logic.command != Logic.Squad.Command.Disengage)
		{
			logic.is_fighting = flag3;
			data->is_Fighting = flag3;
			data->last_Fight_Time = logic.game.time_f;
		}
		data->command = logic.command;
		if (data->stance != logic.stance)
		{
			data->stance = logic.stance;
			data->SetControlZone(logic.formation.cur_width, logic.formation.cur_height, logic.formation.def.defensive_stance_control_zone, logic.formation.def.aggressive_mode_ground_control_zone, logic.formation.def.attacking_control_zone);
		}
		data->wedgeFormation = (logic.formation is TriangleFormation);
		if (data->HasFlags(Troops.SquadData.Flags.HasFinishedShot) && data->def->is_siege_eq)
		{
			squad.logic.last_shoot_time = squad.logic.game.time.seconds - squad.def.shoot_interval;
		}
		data->ClrFlags(Troops.SquadData.Flags.Fighting | Troops.SquadData.Flags.Shooting | Troops.SquadData.Flags.HasFinishedShot);
		data->cur_salvo = squad.cur_salvo;
		object obj;
		if (logic == null)
		{
			obj = null;
		}
		else
		{
			MapObject target = logic.target;
			obj = ((target != null) ? target.visuals : null);
		}
		global::Squad squad2 = obj as global::Squad;
		if (squad2 == null)
		{
			object obj2;
			if (logic == null)
			{
				obj2 = null;
			}
			else
			{
				Logic.Squad enemy_squad = logic.enemy_squad;
				obj2 = ((enemy_squad != null) ? enemy_squad.visuals : null);
			}
			squad2 = (obj2 as global::Squad);
			data->is_attacking = false;
		}
		else
		{
			data->is_attacking = ((logic != null && logic.command == Logic.Squad.Command.Attack) || (logic != null && logic.command == Logic.Squad.Command.Fight) || (logic != null && logic.command == Logic.Squad.Command.Charge));
		}
		if (squad2 == null)
		{
			data->target_id = -1;
		}
		else
		{
			data->target_id = squad2.GetID();
		}
		data->are_selection_circles_enabled = squad.areSelectionCirclesVisibleFilter;
		BattleSimulation.Squad simulation2 = logic.simulation;
		if (simulation2 != null && !simulation2.IsDefeated() && simulation2.state < BattleSimulation.Squad.State.Disengaging)
		{
			BattleSimulation.Squad.State state;
			if (flag3)
			{
				state = BattleSimulation.Squad.State.Attacking;
			}
			else if (data->HasFlags(Troops.SquadData.Flags.Moving))
			{
				if (data->command == Logic.Squad.Command.Charge)
				{
					state = BattleSimulation.Squad.State.Charging;
				}
				else
				{
					state = BattleSimulation.Squad.State.Moving;
				}
			}
			else
			{
				state = BattleSimulation.Squad.State.Idle;
			}
			simulation2.SetState(state, null, -1f);
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00077984 File Offset: 0x00075B84
	public unsafe static void FinishAction(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Logic.Squad logic = squad.logic;
		if (logic == null)
		{
			return;
		}
		Logic.Squad logic2 = squad.logic;
		if (((logic2 != null) ? logic2.simulation : null) != null)
		{
			int num = logic.NumTroops();
			if (squad.GetMainSquadID() == -1 && squad.subsquads_data_ids.Count > 0)
			{
				int allSquadsAliveTroops = squad.GetAllSquadsAliveTroops();
				if (allSquadsAliveTroops != num)
				{
					num = allSquadsAliveTroops;
					logic.SetNumTroops(allSquadsAliveTroops, true);
					data->logic_alive = allSquadsAliveTroops;
				}
			}
			else if (squad.GetMainSquadID() != -1)
			{
				num = squad.GetSquadAliveTroops();
			}
			int logic_alive = data->logic_alive;
			if (logic_alive < num)
			{
				logic.SetNumTroops(logic_alive, true);
			}
			else if (num < logic_alive)
			{
				data->logic_alive = num;
			}
			data->SetFlags(Troops.SquadData.Flags.Dead, num == 0);
			if (num > 0 && data->shock_morale_dmg_acc > 0f)
			{
				logic.simulation.AddMorale(-data->shock_morale_dmg_acc, true, false);
				data->shock_morale_dmg_acc = 0f;
			}
		}
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00077A68 File Offset: 0x00075C68
	private unsafe static void UpdateInvalidMoveHistoryPoints(Troops.SquadData* data, Logic.Squad logic, PPos pos, float pos_t, PPos old_pos, float old_pos_t, Formation.Line mid_line)
	{
		if (logic.movement.path != null)
		{
			PathData.DataPointers pointers = logic.game.path_finding.data.pointers;
			PPos ppos = old_pos;
			PPos ppos2 = default(PPos);
			float num = old_pos_t;
			float num2 = pos_t;
			int num3 = 0;
			PPos ppos3;
			while (ppos != pos && (!pointers.Trace(ppos, pos, out ppos3, false, true, false, logic.movement.allowed_area_types, logic.battle_side, logic.is_inside_walls_or_on_walls) || pos.paID != ppos3.paID) && num3 < 100)
			{
				num3++;
				while (ppos2 != pos)
				{
					if (num2 - num < 0.001f)
					{
						ppos = pos;
						break;
					}
					float num4 = (num2 + num) / 2f;
					PPos ppos4;
					PPos ppos5;
					logic.movement.path.GetPathPoint(num4, out ppos4, out ppos5, false, 0f);
					PPos ppos6;
					if (pointers.Trace(ppos, ppos4, out ppos6, false, true, false, logic.movement.allowed_area_types, logic.battle_side, logic.is_inside_walls_or_on_walls))
					{
						ppos2 = ppos4;
						num = num4;
					}
					else if (ppos2 != PPos.Zero)
					{
						PPos normalized = (ppos - ppos2).GetNormalized();
						data->move_history.Add(ppos2, normalized, num4, logic.game.passability.data, logic.game.path_finding.data.pointers, mid_line, true);
						ppos = ppos2;
						ppos2 = PPos.Zero;
					}
					else
					{
						num2 = num4;
					}
				}
			}
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00077BF0 File Offset: 0x00075DF0
	private static void GetTempTroopProjLists(int thread_id, out List<Troops.TroopProj> lst1, out List<Troops.TroopProj> lst2)
	{
		if (Troops.temp_troop_proj == null)
		{
			Troops.temp_troop_proj = new List<Troops.TroopProj>[128 * 2];
		}
		if (thread_id < 0)
		{
			thread_id = 0;
		}
		lst1 = Troops.temp_troop_proj[thread_id * 2];
		if (lst1 == null)
		{
			List<Troops.TroopProj>[] array = Troops.temp_troop_proj;
			int num = thread_id * 2;
			List<Troops.TroopProj> list;
			lst1 = (list = new List<Troops.TroopProj>(Troops.mem_data.TroopsPerSquad));
			array[num] = list;
		}
		lst2 = Troops.temp_troop_proj[thread_id * 2 + 1];
		if (lst2 == null)
		{
			List<Troops.TroopProj>[] array2 = Troops.temp_troop_proj;
			int num2 = thread_id * 2 + 1;
			List<Troops.TroopProj> list;
			lst2 = (list = new List<Troops.TroopProj>(Troops.mem_data.TroopsPerSquad));
			array2[num2] = list;
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00077C78 File Offset: 0x00075E78
	private static void RecalcFormation(global::Squad squad, ref Troops.SquadData data)
	{
		List<Troops.TroopProj> list;
		List<Troops.TroopProj> list2;
		Troops.GetTempTroopProjLists(data.thread_id, out list, out list2);
		Troops.Troop troop = data.FirstTroop;
		while (troop <= data.LastTroop)
		{
			if (troop.squad_id == squad.GetID())
			{
				troop.form_idx = -1;
			}
			troop = ++troop;
		}
		Formation data_formation = squad.data_formation;
		Point pt = data.pos;
		Point pt2 = data.dir;
		Point pt3 = pt2.Right(0f);
		Troops.Troop troop2 = data.FirstTroop;
		while (troop2 <= data.LastTroop)
		{
			if (!troop2.HasFlags(Troops.Troop.Flags.Dead) && troop2.squad_id == squad.GetID())
			{
				Point point = troop2.pos - pt;
				float proj = point.Dot(pt3);
				float dist = -point.Dot(pt2);
				list.Add(new Troops.TroopProj
				{
					troop = troop2,
					proj = proj,
					dist = dist
				});
			}
			troop2 = ++troop2;
		}
		list.Sort((Troops.TroopProj p1, Troops.TroopProj p2) => p1.dist.CompareTo(p2.dist));
		int num = 0;
		for (int i = 0; i < data_formation.lines.Count; i++)
		{
			int count = data_formation.lines[i].count;
			for (int j = 0; j < count; j++)
			{
				Troops.TroopProj item = list[num + j];
				list2.Add(item);
			}
			list2.Sort((Troops.TroopProj p1, Troops.TroopProj p2) => p1.proj.CompareTo(p2.proj));
			for (int k = 0; k < count; k++)
			{
				list2[k].troop.form_idx = num++;
			}
			list2.Clear();
		}
		list.Clear();
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00077E74 File Offset: 0x00076074
	private static void RecalcFormationHungarianMethod(global::Squad squad, Formation formation, ref Troops.SquadData data, Troops.Troop.Flags flags = Troops.Troop.Flags.None)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		formation.dirty = false;
		int num = 0;
		Troops.Troop troop = data.FirstTroop;
		while (troop <= data.LastTroop)
		{
			if (troop.squad_id == squad.GetID() && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				num++;
			}
			troop = ++troop;
		}
		int num2 = math.min(num, formation.count);
		int[,] array = new int[num2, num2];
		int[] array2 = new int[num2];
		int num3 = 0;
		Troops.Troop troop2 = data.FirstTroop;
		while (troop2 <= data.LastTroop)
		{
			if (troop2.squad_id == squad.GetID() && !troop2.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && (flags == Troops.Troop.Flags.None || troop2.HasFlags(flags)))
			{
				dictionary.Add(troop2.id, num3);
				num3++;
				for (int i = 0; i < num2; i++)
				{
					float num4 = formation.positions[i].SqrDist(troop2.pos);
					array[dictionary[troop2.id], i] = (int)(num4 * 1000f);
				}
				if (num3 >= num2)
				{
					break;
				}
			}
			troop2 = ++troop2;
		}
		array2 = new global::HungarianAlgorithm(array).Run();
		Troops.Troop troop3 = data.FirstTroop;
		while (troop3 <= data.LastTroop)
		{
			if (troop3.squad_id == squad.GetID())
			{
				if (troop3.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
				{
					troop3.form_idx = -1;
				}
				else if ((flags == Troops.Troop.Flags.None || troop3.HasFlags(flags)) && dictionary.ContainsKey(troop3.id) && dictionary[troop3.id] < array2.Length)
				{
					troop3.form_idx = array2[dictionary[troop3.id]];
				}
			}
			troop3 = ++troop3;
		}
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00078050 File Offset: 0x00076250
	public static Formation troop_formation(Troops.Troop troop)
	{
		int squad_id = troop.squad_id;
		global::Squad squad = Troops.squads[squad_id];
		if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished))
		{
			return squad.wall_climb_dest_formation;
		}
		if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadderWaiting))
		{
			return squad.wall_climb_start_formation;
		}
		return squad.data_formation;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x000780A0 File Offset: 0x000762A0
	public unsafe static void CalcAnimationCounts(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		if (squad.logic == null)
		{
			return;
		}
		for (int i = 0; i < 30; i++)
		{
			*(ref data->animation_counts.FixedElementField + (IntPtr)i * 4) = 0;
		}
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == squad.GetID())
			{
				(*(ref data->animation_counts.FixedElementField + (IntPtr)troop.cur_anim_state * 4))++;
			}
			troop = ++troop;
		}
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x00078134 File Offset: 0x00076334
	public unsafe static void CalcFormationAction(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Logic.Squad logic = squad.logic;
		if (logic == null || !logic.IsValid())
		{
			return;
		}
		Logic.Unit.Def def = squad.def;
		Point point = data->dir;
		PPos ppos = point;
		bool flag = false;
		Formation data_formation = squad.data_formation;
		bool flag2 = data->HasFlags(Troops.SquadData.Flags.Moving | Troops.SquadData.Flags.Teleport);
		bool flag3 = false;
		if (squad.previewFormationConfirmed != null && !squad.data->in_drag)
		{
			data->preview_formation_dir = squad.previewFormationConfirmed.dir.Heading();
		}
		else if (squad.previewFormation != null)
		{
			data->preview_formation_dir = squad.previewFormation.dir.Heading();
		}
		PPos pos;
		if (logic.climbing)
		{
			Troops.ClimbTroops(squad);
			pos = data->pos;
		}
		else
		{
			flag = data->HasFlags(Troops.SquadData.Flags.Teleport);
			float speed = logic.movement.speed;
			logic.CalcPos(out pos, out ppos, speed, def.dir_look_ahead, false);
			if (ppos != point)
			{
				data->dir = ppos;
				data->rot = 90f - ppos.Heading();
				float to = 90f - point.Heading();
				float num = math.abs(Angle.Diff(data->rot, data->form_rot));
				float num2 = math.abs(Angle.Diff(data->rot, to));
				flag3 = (num > 90f);
				float num3 = num2 / Troops.pdata->dt;
			}
			logic.formation.SetCount(data->logic_alive);
			if (data_formation.CopyParamsFrom(logic.formation))
			{
				flag2 = true;
			}
			data_formation.SetCount(data->logic_alive);
			if (data_formation.dirty)
			{
				flag3 = true;
			}
			if (flag)
			{
				data->ClrFlags(Troops.SquadData.Flags.Teleport);
				flag3 = true;
				logic.move_history = null;
			}
			if (Troops.RecalcStuckSquad(data, flag2))
			{
				flag3 = true;
			}
			if (flag2 || flag3)
			{
				logic.CalcFormation(data_formation, speed, true);
			}
			if (flag3)
			{
				Troops.RecalcFormationHungarianMethod(squad, squad.data_formation, ref *squad.data, Troops.Troop.Flags.None);
				data->form_rot = data->rot;
				Troops.RecalcDeform(squad);
				data->SetControlZone(data_formation.cur_width, data_formation.cur_height, logic.formation.def.defensive_stance_control_zone, logic.formation.def.aggressive_mode_ground_control_zone, logic.formation.def.attacking_control_zone);
			}
			if (flag2 || flag3)
			{
				Troops.ApplyDeform(squad);
			}
		}
		float num4 = 0f;
		if (squad.logic.position.paID > 0)
		{
			Troops.Troop troop = data->FirstTroop;
			while (troop <= data->LastTroop)
			{
				if (!troop.HasFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == squad.GetID())
				{
					Formation formation = Troops.troop_formation(troop);
					bool flag4;
					if (formation == null)
					{
						flag4 = true;
					}
					else
					{
						NativeArrayList<PPos> positions = formation.positions;
						flag4 = false;
					}
					if (!flag4 && formation.positions.Count > troop.form_idx)
					{
						float num5 = formation.positions[troop.form_idx].Dist(pos);
						if (num5 > num4)
						{
							num4 = num5;
						}
					}
				}
				troop = ++troop;
			}
		}
		float num6 = math.max(logic.normal_move_speed * def.run_speed_mul * def.max_speed_mul, num4);
		float num7 = num6 * 4f;
		if (def.is_siege_eq)
		{
			num6 += def.radius;
			num7 += def.radius;
		}
		float num8 = num6 * num6;
		float num9 = num7 * num7;
		num9 *= (data->troop_too_far ? 0.64000005f : 1f);
		bool flag5 = false;
		bool flag6 = false;
		int num10 = 0;
		int num11 = 0;
		float2 @float = 0;
		float2 float2 = 0;
		float3 float3 = 0;
		int num12 = 0;
		float num13 = 0f;
		float num14 = data->rearrange_max_safe_dist * data->rearrange_max_safe_dist;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = logic.command == Logic.Squad.Command.Attack || logic.command == Logic.Squad.Command.Fight;
		Troops.Troop troop2 = data->FirstTroop;
		while (troop2 <= data->LastTroop)
		{
			if (!troop2.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop2.squad_id == squad.GetID())
			{
				if (num12 == 0)
				{
					float2 = (@float = pos);
				}
				PPos pos2 = troop2.pos;
				PPos ppos2;
				if (troop2.HasFlags(Troops.Troop.Flags.ClimbingLadder))
				{
					ppos2 = troop2.tgt_pos;
				}
				else
				{
					Formation formation2 = Troops.troop_formation(troop2);
					bool flag10;
					if (formation2 == null)
					{
						flag10 = true;
					}
					else
					{
						NativeArrayList<PPos> positions2 = formation2.positions;
						flag10 = false;
					}
					if (flag10 || formation2.positions.Count <= troop2.form_idx)
					{
						ppos2 = troop2.pos;
					}
					else
					{
						ppos2 = Troops.troop_formation(troop2).positions[troop2.form_idx];
					}
				}
				if (flag)
				{
					PPos ppos3 = troop2.pos = (troop2.tgt_pos = ppos2);
				}
				else
				{
					if (pos2.x + troop2.def->radius > float2.x)
					{
						float2 = new float2(pos2.x + troop2.def->radius, float2.y);
					}
					if (pos2.y + troop2.def->radius > float2.y)
					{
						float2 = new float2(float2.x, pos2.y + troop2.def->radius);
					}
					if (pos2.x - troop2.def->radius < @float.x)
					{
						@float = new float2(pos2.x - troop2.def->radius, @float.y);
					}
					if (pos2.y - troop2.def->radius < @float.y)
					{
						@float = new float2(@float.x, pos2.y - troop2.def->radius);
					}
					float3 += troop2.pos3d;
					num12++;
					if (!troop2.squad->def->is_cavalry && troop2.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger) && !troop2.squad->HasFlags(Troops.SquadData.Flags.Fled))
					{
						ppos2 = troop2.tgt_pos;
					}
					if (def.is_siege_eq && logic.IsPacking())
					{
						ppos2 = troop2.pos;
					}
					else if (logic.ranged_enemy != null && logic.CanShoot(logic.ranged_enemy, true) && logic.arrow_path_is_clear && !troop2.squad->def->is_cavalry)
					{
						if (!def.is_siege_eq)
						{
							goto IL_78A;
						}
						ppos2 = troop2.pos;
					}
					if (troop2.cur_anim_state == UnitAnimation.State.ShieldSetup || troop2.cur_anim_state == UnitAnimation.State.ShieldPack)
					{
						ppos2 = troop2.pos;
					}
					if (logic.normal_move_speed == 0f)
					{
						ppos2 = pos2;
					}
					float num15 = pos2.SqrDist(ppos2);
					float num16 = troop2.HasFlags(Troops.Troop.Flags.Fighting) ? 0.4f : 1f;
					if (num15 < num9 * num16)
					{
						if (num15 < num8)
						{
							flag5 = true;
							num10++;
							troop2.ClrFlags(Troops.Troop.Flags.FarFromSquad);
						}
						troop2.tgt_pos = ppos2;
					}
					else if (pos2.SqrDist(data->pos) >= num8 * 0.5f)
					{
						flag6 = true;
						num11++;
						data->troop_too_far = flag6;
						troop2.SetFlags(Troops.Troop.Flags.FarFromSquad);
						troop2.tgt_pos = ppos2;
					}
					else
					{
						troop2.tgt_pos = ppos2;
					}
					if (num15 > num13)
					{
						num13 = num15;
					}
					if (!flag7 && (flag9 || logic.overlap_with_ally || troop2.HasFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.Collided | Troops.Troop.Flags.Stuck | Troops.Troop.Flags.MovingDown)))
					{
						flag7 = true;
						flag8 = false;
					}
					if (!flag7 && data->rearrange_troops_cooldown == 0f && !flag2 && !troop2.HasFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.Collided | Troops.Troop.Flags.Stuck | Troops.Troop.Flags.MovingDown) && !logic.climbing && !flag9 && num15 > num14 && !data->IsRearranging())
					{
						flag8 = true;
					}
				}
			}
			IL_78A:
			troop2 = ++troop2;
		}
		if (flag8)
		{
			data->rearrange_troops_cooldown = data->rearrange_troops_delay;
		}
		if (num12 > 0)
		{
			float3 /= (float)num12;
		}
		data->bb_lower_left_corner = @float;
		data->bb_upper_right_corner = float2;
		float2 float4 = (@float + float2) * 0.5f - float2;
		data->sqr_radius = float4.x * float4.x + float4.y * float4.y;
		int num17 = data->closest_troop_banner_id;
		Troops.Troop troop3 = default(Troops.Troop);
		float num18 = float.MaxValue;
		if (num17 >= 0)
		{
			troop3 = Troops.pdata->GetTroop(0, num17);
			num18 = math.distance(troop3.pos3d, float3);
		}
		Troops.Troop troop4 = data->FirstTroop;
		while (troop4 <= data->LastTroop)
		{
			if (!troop4.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop4.squad_id == squad.GetID() && troop4.id != data->closest_troop_banner_id)
			{
				float num19 = math.distance(troop4.pos3d, float3);
				if (num19 < num18 * 0.5f)
				{
					num18 = num19;
					troop3 = troop4;
					num17 = troop4.id;
				}
			}
			troop4 = ++troop4;
		}
		if (num17 >= 0)
		{
			data->closest_troop_banner_id = troop3.id;
			data->banner_pos = troop3.pos3d;
		}
		else
		{
			data->banner_pos = float3;
		}
		logic.avg_troops_pos = float3;
		int squadAliveTroops = squad.GetSquadAliveTroops();
		float num20 = (float)num10 / (float)squadAliveTroops;
		float num21 = (float)num11 / (float)squadAliveTroops;
		bool flag11 = (logic.def.is_siege_eq && ((logic.CanBePacked() && !logic.is_packed) || logic.IsPacking())) || logic.climbing || logic.paused || (logic.command != Logic.Squad.Command.Charge && !flag5 && !data->def->is_siege_eq) || (flag6 && logic.command != Logic.Squad.Command.Attack && num21 > 0.9f && logic.command != Logic.Squad.Command.Disengage) || (logic.command == Logic.Squad.Command.Fight && num20 < 0.2f);
		Troops.RearrangeTroops(data, logic, flag2, flag11);
		logic.movement.Pause(flag11, true);
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00078B00 File Offset: 0x00076D00
	private unsafe static void RearrangeTroops(Troops.SquadData* data, Logic.Squad logic, bool moving, bool pause_movement)
	{
		if ((data->rearrange_troops_cooldown > 0f && !moving && !pause_movement) || logic.rearrange)
		{
			data->rearrange_troops_cooldown = data->rearrange_troops_cooldown - Troops.pdata->dt;
			if (data->rearrange_troops_cooldown < 0f || logic.rearrange)
			{
				data->rearrange_troops_cooldown = 0f;
				if (!data->IsRearranging() || logic.rearrange)
				{
					data->rearrange_safe_dist = data->rearrange_min_safe_dist;
					data->rearrange_troops_cooldown = data->rearrange_troops_time;
				}
				else
				{
					data->rearrange_safe_dist = data->rearrange_max_safe_dist;
				}
				if (logic.rearrange)
				{
					logic.rearrange = false;
					return;
				}
			}
		}
		else if (moving || pause_movement)
		{
			data->rearrange_troops_cooldown = 0f;
			data->rearrange_safe_dist = data->rearrange_max_safe_dist;
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00078BC0 File Offset: 0x00076DC0
	public unsafe static void ClimbTroops(global::Squad squad)
	{
		Logic.Squad logic = squad.logic;
		Troops.SquadData* data = squad.data;
		Logic.Ladder ladder = null;
		bool flag = logic.cur_ladder.Area_valid();
		if (!logic.battle.ladders.TryGetValue(data->climb_ladder_paid, out ladder))
		{
			flag = false;
		}
		if (!flag)
		{
			logic.StopClimb();
			return;
		}
		logic.movement.Pause(true, true);
		bool flag2 = squad.wall_climb_start_formation.dirty || squad.wall_climb_dest_formation.dirty;
		if (data->climb_cooldown <= 0f)
		{
			if (ladder.cur_squad == logic)
			{
				Troops.StartClimbTroop(squad);
				flag2 = true;
			}
		}
		else
		{
			if (data->HasFlags(Troops.SquadData.Flags.HasTroopClimbedLadder))
			{
				data->ClrFlags(Troops.SquadData.Flags.HasTroopClimbedLadder);
				flag2 = true;
			}
			data->climb_cooldown -= Troops.pdata->dt;
		}
		if (flag2)
		{
			squad.RecalcClimbFormations();
			Troops.RecalcFormationHungarianMethod(squad, squad.wall_climb_start_formation, ref *squad.data, Troops.Troop.Flags.ClimbingLadderWaiting);
			Troops.RecalcFormationHungarianMethod(squad, squad.wall_climb_dest_formation, ref *squad.data, Troops.Troop.Flags.ClimbingLadderFinished);
		}
		if (!squad.HasClimbingTroops())
		{
			logic.StopClimb();
			logic.RecalcPath(squad.wall_climb_dest_formation.pos);
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00078CDC File Offset: 0x00076EDC
	public static void UpdateLadders()
	{
		using (Game.Profile("Troops.UpdateLadders", false, 0f, null))
		{
			for (int i = 0; i < Troops.squads.Length; i++)
			{
				global::Squad squad = Troops.squads[i];
				if (!(squad == null))
				{
					squad.logic.CheckNeedsLadder();
				}
			}
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00078D4C File Offset: 0x00076F4C
	private unsafe static void StartClimbTroop(global::Squad squad)
	{
		Logic.Squad logic = squad.logic;
		Troops.SquadData* data = squad.data;
		Point pt = new Point(data->climb_start_pt.x, data->climb_start_pt.z);
		float num = float.MaxValue;
		int num2 = -1;
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (troop.squad_id == squad.GetID() && !troop.HasFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed | Troops.Troop.Flags.ClimbingLadderFinished))
			{
				float num3 = troop.pos.SqrDist(pt);
				if (num3 < num)
				{
					num2 = troop.id;
					num = num3;
				}
			}
			troop = ++troop;
		}
		if (num2 == -1)
		{
			return;
		}
		data->climb_cooldown = logic.def.climb_cooldown;
		Troops.Troop troop2 = new Troops.Troop(Troops.pdata, data->thread_id, num2);
		troop2.SetFlags(Troops.Troop.Flags.ClimbingLadder);
		troop2.ClrFlags(Troops.Troop.Flags.ClimbingLadderWaiting);
		troop2.climb_progress = 0f;
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00078E40 File Offset: 0x00077040
	private unsafe static bool RecalcStuckSquad(Troops.SquadData* data, bool moving)
	{
		if (!moving)
		{
			float num = 3f;
			int num2 = 3;
			if (data->stuck_recalc_attempts <= num2)
			{
				Troops.Troop troop = data->FirstTroop;
				while (troop <= data->LastTroop)
				{
					if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == data->id && troop.HasFlags(Troops.Troop.Flags.Stuck))
					{
						if (data->stuck_recalc_attempts == num2)
						{
							troop.ClrFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.Stuck);
						}
						else
						{
							data->stuck_recalc_time = data->stuck_recalc_time + troop.data->dt;
							if (data->stuck_recalc_time >= num)
							{
								data->stuck_recalc_attempts = data->stuck_recalc_attempts + 1;
								data->stuck_recalc_time = 0f;
								return true;
							}
							break;
						}
					}
					troop = ++troop;
				}
				if (data->stuck_recalc_attempts == num2)
				{
					data->stuck_recalc_attempts = data->stuck_recalc_attempts + 1;
				}
			}
		}
		else
		{
			data->stuck_recalc_time = 0f;
			data->stuck_recalc_attempts = 0;
		}
		return false;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00078F24 File Offset: 0x00077124
	public unsafe static void RecalcDeform(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		if (data->logic_alive == 1)
		{
			data->FirstTroop.deform = 0;
			return;
		}
		Formation data_formation = squad.data_formation;
		float num = data->def->radius * 2f;
		Point spacing = data_formation.spacing;
		spacing.x -= num;
		spacing.y -= num;
		spacing.x *= data_formation.def.deform_x;
		spacing.y *= data_formation.def.deform_y1;
		Point point = spacing * 0.5f;
		Unity.Mathematics.Random rand = Troops.pdata->rand;
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (troop.form_idx >= 0 && troop.squad_id == squad.GetID())
			{
				float x = rand.NextFloat(spacing.x) - point.x;
				float y = rand.NextFloat(spacing.y) - point.y;
				troop.deform = new float2(x, y);
			}
			troop = ++troop;
		}
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00079050 File Offset: 0x00077250
	public unsafe static void ApplyDeform(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Point pt = data->dir;
		Point pt2 = pt.Right(0f);
		Formation data_formation = squad.data_formation;
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (troop.form_idx >= 0)
			{
				bool flag;
				if (data_formation == null)
				{
					flag = true;
				}
				else
				{
					NativeArrayList<PPos> positions = data_formation.positions;
					flag = false;
				}
				if (!flag && data_formation.positions.Count > troop.form_idx && troop.squad_id == squad.GetID())
				{
					Point point = troop.deform;
					if (!(point == Point.Zero))
					{
						PPos ppos = data_formation.positions[troop.form_idx];
						if (ppos.paID <= 0 || squad.logic.game.path_finding.data.pointers.GetPA(ppos.paID - 1).IsGround())
						{
							ppos += point.x * pt2 + point.y * pt;
							if (Troops.path_data.IsPassable(ppos))
							{
								data_formation.positions[troop.form_idx] = ppos;
							}
						}
					}
				}
			}
			troop = ++troop;
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000791B0 File Offset: 0x000773B0
	public unsafe static void CalcTroopsHoldFormationVec(global::Squad squad)
	{
		if (squad == null || squad.logic == null || !squad.logic.IsValid())
		{
			return;
		}
		Formation data_formation = squad.data_formation;
		Troops.Troop firstTroop = squad.data->FirstTroop;
		float4 @float = default(float4);
		if (squad.data->is_attacking)
		{
			Troops.Troop troop = firstTroop;
			while (troop <= squad.data->LastTroop)
			{
				if (!troop.HasFlags(Troops.Troop.Flags.FarFromSquad | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == squad.GetID())
				{
					@float.xyz += troop.pos;
					@float.w += 1f;
				}
				troop = ++troop;
			}
		}
		float2 rhs = new float2(0f, 0f);
		if (@float.w > 0f)
		{
			@float.xyz /= @float.w;
			rhs = @float.xz - data_formation.pos;
		}
		Troops.Troop troop2 = firstTroop;
		while (troop2 <= squad.data->LastTroop)
		{
			if (!troop2.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop2.squad_id == squad.GetID() && troop2.form_idx >= 0 && troop2.form_idx < data_formation.positions.Count)
			{
				float2 float2 = data_formation.positions[troop2.form_idx] - troop2.pos2d + rhs;
				if (float2.x == 0f && float2.y == 0f)
				{
					troop2.hold_formation_vec = float2;
				}
				else
				{
					float num = math.length(float2);
					num = ((num < 1f) ? num : 1f);
					troop2.hold_formation_vec = math.normalize(float2) * num;
					troop2.pos_in_formation = data_formation.positions[troop2.form_idx];
				}
			}
			troop2 = ++troop2;
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000793D0 File Offset: 0x000775D0
	public unsafe static void FindAccessibleEnemySquadsInRange(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		data->has_accessible_enemies = false;
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			global::Squad squad2 = Troops.squads[i];
			if (squad2 && !squad2.data->HasFlags(Troops.SquadData.Flags.Dead | Troops.SquadData.Flags.Destroyed) && squad2.data->battle_side != data->battle_side)
			{
				PPos ppos = squad2.data->pos - data->pos;
				float num = math.lengthsq(data->controlZone + squad2.data->controlZone);
				if (ppos.SqrLength() <= num)
				{
					data->has_accessible_enemies = true;
					return;
				}
			}
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00079480 File Offset: 0x00077680
	public static void ClearModelDataBuffer()
	{
		Troops.texture_baker.troop_selection_arrows_drawer.model_data.Clear();
		Troops.texture_baker.troop_selection_circles_drawer.model_data.Clear();
		Troops.texture_baker.dust_drawer.model_data.Clear();
		foreach (KeyValuePair<string, Troops.SquadModelData> keyValuePair in Troops.sharedSkinningData)
		{
			for (int i = 0; i < keyValuePair.Value.per_model_data.Count; i++)
			{
				keyValuePair.Value.per_model_data[i].model_data_buffer.Clear();
			}
		}
		Troops.texture_baker.trail_drawer.model_data.Clear();
		foreach (KeyValuePair<string, TextureBaker.PerArrowModelData> keyValuePair2 in Troops.texture_baker.arrow_drawers)
		{
			keyValuePair2.Value.model_data_buffer.Clear();
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x000795A4 File Offset: 0x000777A4
	public unsafe static void DisableStuckAreas(global::Squad squad)
	{
		List<int> list = new List<int>();
		Troops.Troop troop = squad.data->FirstTroop;
		while (troop <= squad.data->LastTroop)
		{
			if (troop.squad_id == squad.GetID() && !troop.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				int pa_id = troop.pa_id;
				if (pa_id != 0 && !list.Contains(pa_id))
				{
					list.Add(pa_id);
				}
			}
			troop = ++troop;
		}
		Logic.PathFinding path_finding = squad.logic.game.path_finding;
		while (list.Count > 0)
		{
			int num = list[0];
			list.RemoveAt(0);
			PathData.PassableArea pa = path_finding.data.pointers.GetPA(num - 1);
			pa.NobodyCanEnter = true;
			path_finding.data.pointers.SetPA(num - 1, pa);
			PathData.HighPassableAreaNode highPassableAreaNode = path_finding.data.highPassableAreaNodes[num - 1];
			for (int i = 0; i < highPassableAreaNode.ribs.Count; i++)
			{
				int paID = highPassableAreaNode.ribs[i].GetOther(highPassableAreaNode).pos.paID;
				PathData.PassableArea pa2 = path_finding.data.pointers.GetPA(paID - 1);
				if (pa2.enabled && !pa2.NobodyCanEnter && !list.Contains(paID))
				{
					list.Add(paID);
				}
			}
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0007970C File Offset: 0x0007790C
	public static bool CheckSwapMainSquad(global::Squad squad)
	{
		if (squad.logic == null || squad.data == null)
		{
			return false;
		}
		if (squad.subsquads_data_ids.Count > 0 && (squad.GetSquadAliveTroops() <= 0 || squad.logic.IsDefeated()))
		{
			global::Squad nextMainSquad = squad.GetNextMainSquad();
			if (nextMainSquad != null)
			{
				nextMainSquad.SwitchToMainSquad();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0007976C File Offset: 0x0007796C
	public unsafe static bool CheckDefeated(global::Squad squad, bool apply_results = true)
	{
		if (squad.logic == null || squad.data == null)
		{
			return true;
		}
		bool flag = false;
		if (squad.subsquads_data_ids.Count > 0)
		{
			if (squad.GetSquadAliveTroops() <= 0)
			{
				flag = true;
			}
		}
		else if (squad.data->logic_alive <= 0)
		{
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		if (apply_results)
		{
			if (squad.logic.simulation.state != BattleSimulation.Squad.State.Dead)
			{
				squad.logic.simulation.SetState(BattleSimulation.Squad.State.Dead, null, -1f);
				squad.logic.simulation.simulation.ThinkSurrender(squad.logic.battle_side);
			}
			if (squad.logic.simulation.state == BattleSimulation.Squad.State.Dead && squad.logic.IsValid())
			{
				squad.logic.Destroy(false);
			}
		}
		return true;
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0007983C File Offset: 0x00077A3C
	public unsafe static void UpdateEnemySquads(global::Squad squad)
	{
		using (Game.Profile("Troops.UpdateEnemySquads", false, 0f, null))
		{
			if (!(squad == null) && squad.logic != null && squad.logic.IsValid())
			{
				Troops.Troop troop = squad.data->FirstTroop;
				while (troop <= squad.data->LastTroop)
				{
					if (troop.squad_id == squad.GetID())
					{
						if (troop.enemy_id >= 0 && troop.enemy.valid && troop.HasFlags(Troops.Troop.Flags.Fighting))
						{
							global::Squad squad2 = Troops.squads[troop.enemy.squad_id];
							if (((squad2 != null) ? squad2.logic : null) != null)
							{
								squad.logic.AddMeleeSquad(squad2.logic);
								squad2.logic.AddMeleeSquad(squad.logic);
							}
						}
						if (troop.range_enemy_squad_id >= 0)
						{
							global::Squad squad3 = Troops.squads[troop.range_enemy_squad_id];
							if (((squad3 != null) ? squad3.logic : null) != null)
							{
								squad.logic.AddShootingSquad(squad3.logic);
							}
							troop.range_enemy_squad_id = -1;
						}
					}
					troop = ++troop;
				}
			}
		}
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0007999C File Offset: 0x00077B9C
	private unsafe static void StopSquad(global::Squad squad)
	{
		using (Game.Profile("Troops.StopSquad", false, 0f, null))
		{
			if (squad.data != null && squad.logic != null)
			{
				if (!squad.logic.IsMoving() && squad.def.is_siege_eq && squad.logic.NumTroops() == 1)
				{
					Troops.Troop troop = squad.data->FirstTroop;
					while (troop <= squad.data->LastTroop)
					{
						troop.ClrFlags(Troops.Troop.Flags.Blocked);
						troop = ++troop;
					}
				}
				else if (squad.def.is_siege_eq && squad.logic.NumTroops() == 1)
				{
					bool flag = true;
					PPos ppos = default(PPos);
					Troops.Troop troop2 = squad.data->FirstTroop;
					while (troop2 <= squad.data->LastTroop)
					{
						if (!troop2.HasFlags(Troops.Troop.Flags.Blocked))
						{
							flag = false;
						}
						else
						{
							troop2.ClrFlags(Troops.Troop.Flags.Blocked);
							troop2.ClrFlags(Troops.Troop.Flags.Moving);
							ppos = troop2.pos;
						}
						troop2 = ++troop2;
					}
					if (flag)
					{
						squad.logic.SetPosition(ppos);
						squad.data_formation.Calc(ppos, squad.logic.direction, true, false);
						squad.logic.Stop(true);
						squad.logic.SetCommand(Logic.Squad.Command.Hold, null);
						squad.data->pos = squad.logic.position;
						squad.data->tgtPos = squad.logic.position;
					}
				}
			}
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00079B54 File Offset: 0x00077D54
	private unsafe static void MoveSquad(global::Squad squad)
	{
		using (Game.Profile("Troops.MoveSquad", false, 0f, null))
		{
			if (squad.data != null)
			{
				PPos pos = squad.data->pos;
				squad.transform.position = global::Common.SnapToTerrain(new Vector3(pos.x, 0f, pos.y), 0f, null, -1f, false);
				if (pos.paID != 0 && !BattleMap.battle.batte_view_game.path_finding.data.processing)
				{
					PathData.DataPointers dataPointers = Troops.path_data;
					PathData.PassableArea pa = dataPointers.GetPA(pos.paID - 1);
					float angle = pa.Angle;
					bool flag = squad.logic.climbing;
					if (!flag)
					{
						bool flag2 = false;
						Troops.Troop troop = squad.data->FirstTroop;
						while (troop <= squad.data->LastTroop)
						{
							if (troop.squad_id == squad.GetID())
							{
								if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadder))
								{
									flag = true;
								}
								if (troop.pa_id != 0 || troop.HasFlags(Troops.Troop.Flags.FarFromSquad))
								{
									if (!dataPointers.IsNeighbour(troop.pa_id, pos.paID))
									{
										flag2 = true;
									}
									if (flag && flag2)
									{
										break;
									}
								}
							}
							troop = ++troop;
						}
					}
					if (pa.type == PathData.PassableArea.Type.Ladder || flag)
					{
						squad.logic.movement.speed = 0.3f;
					}
					else if (pos.paID != 0)
					{
						squad.logic.movement.speed = squad.logic.last_move_speed * math.max(1f - angle / 135f, 0.25f);
					}
				}
			}
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00079D30 File Offset: 0x00077F30
	private unsafe static void RotateSquad(global::Squad squad)
	{
		if (squad.data == null)
		{
			return;
		}
		squad.transform.rotation = Quaternion.Euler(0f, 90f - squad.data->rot, 0f);
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00079D68 File Offset: 0x00077F68
	private unsafe static void SpawnRagdoll(Troops.Troop troop)
	{
		global::Squad squad = Troops.GetSquad(troop.squad_id);
		if (squad.model_data == null)
		{
			troop.Destroy();
			return;
		}
		TextureBaker.PerModelData perModelData = squad.model_data.per_model_data[troop.drawer_model_id];
		GameObject gameObject = (perModelData != null) ? perModelData.ragdoll_prefab : null;
		if (gameObject == null)
		{
			troop.Destroy();
			return;
		}
		UnityEngine.Object component = gameObject.GetComponent<Animator>();
		Troops.BoneSettingType boneSettingType = Troops.BoneSettingType.Animator;
		if (component == null)
		{
			boneSettingType = Troops.BoneSettingType.BakedTexture;
		}
		GameObject gameObject2 = global::Common.Spawn(gameObject, false, false);
		global::Common.SetObjectParent(gameObject2, GameLogic.instance.transform, "Ragdolls");
		gameObject2.transform.position = troop.pos3d + new float3(0f, 0.1f, 0f);
		gameObject2.transform.eulerAngles = troop.rot3d;
		gameObject2.transform.localScale = Vector3.one;
		gameObject2.SetLayer(LayerMask.NameToLayer("Physics"), true);
		Troops.RagdollInfo ragdollInfo = new Troops.RagdollInfo();
		if (troop.pa_id > 0 && !Troops.path_data.GetPA(troop.pa_id - 1).IsGround())
		{
			ragdollInfo.was_off_ground = true;
		}
		ragdollInfo.go = gameObject2;
		ragdollInfo.sink_depth = squad.def.sink_depth;
		ragdollInfo.max_ragdoll_velocity = squad.def.max_ragdoll_velocity;
		ragdollInfo.spawn_time = UnityEngine.Time.time;
		ragdollInfo.despawn_time = ragdollInfo.spawn_time + UserSettings.MaxRagdollDuration;
		ragdollInfo.sink_time = squad.def.ragdoll_sink_time;
		ragdollInfo.inert = false;
		ragdollInfo.decal_scale = UnityEngine.Random.Range(squad.def.death_decal_min_scale, squad.def.death_decal_max_scale);
		ragdollInfo.target_alpha = UnityEngine.Random.Range(squad.def.death_decal_min_alpha, squad.def.death_decal_max_alpha);
		Troops.ragdolls.Add(ragdollInfo);
		float3 anim_result = troop.anim_result;
		Texture2D texture = perModelData.drawers[0].baked_data.Texture;
		troop.Destroy();
		Rigidbody[] componentsInChildren = gameObject2.GetComponentsInChildren<Rigidbody>();
		ragdollInfo.rigidbodies = componentsInChildren;
		Vector3 vector = new Vector3(troop.vel_spd_t.x, 0f, troop.vel_spd_t.y).normalized * troop.vel_spd_t.z;
		if (troop.thrown_dir.x != 0f && troop.thrown_dir.y != 0f)
		{
			float d = troop.def->is_cavalry ? 1f : 2f;
			float d2 = troop.def->is_cavalry ? 3.5f : 6.5f;
			vector += (new Vector3(troop.thrown_dir.x, 0f, troop.thrown_dir.y).normalized + Vector3.up * d).normalized * d2;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].velocity = vector;
		}
		Renderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<Renderer>();
		if (componentsInChildren2.Length != 0)
		{
			Color value = BattleMap.Get().unit_colors[troop.squad->kingdom_color_id];
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", value);
			foreach (Renderer renderer in componentsInChildren2)
			{
				if (renderer.sharedMaterial != null)
				{
					renderer.sharedMaterial = SilhuetteUtility.GetNoSilhuetteMaterial(renderer.sharedMaterial);
				}
				renderer.SetPropertyBlock(materialPropertyBlock);
				SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.updateWhenOffscreen = true;
				}
			}
		}
		if (boneSettingType == Troops.BoneSettingType.BakedTexture)
		{
			SkinnedMeshRenderer componentInChildren = gameObject2.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren)
			{
				BakedTextureBoneAnimator.SetupBonesFromAnimationTexture(componentInChildren, anim_result, texture);
				return;
			}
		}
		else if (boneSettingType == Troops.BoneSettingType.Animator)
		{
			Animator component2 = gameObject2.GetComponent<Animator>();
			if (component2 == null)
			{
				return;
			}
			component2.fireEvents = false;
			UnitAnimation.Index cur_anim_idx = (UnitAnimation.Index)troop.cur_anim_idx;
			KeyframeTextureBaker.AnimationClipDataBaked cur_anim_info = troop.cur_anim_info;
			component2.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			component2.Play(cur_anim_idx.ToString(), 0, troop.cur_anim_time / cur_anim_info.AnimationLength);
			component2.Update(0f);
			component2.enabled = false;
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0007A1DC File Offset: 0x000783DC
	private unsafe static void SpawnRagdolls(global::Squad squad)
	{
		using (Game.Profile("Troops.SpawnRagdolls", false, 0f, null))
		{
			if (squad.data != null)
			{
				Troops.Troop troop = squad.data->FirstTroop;
				while (troop <= squad.data->LastTroop)
				{
					if (troop.data != null && troop.squad_id == squad.GetID() && troop.valid && troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						Troops.SpawnRagdoll(troop);
					}
					troop = ++troop;
				}
			}
		}
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0007A288 File Offset: 0x00078488
	private static void UpdateDecals()
	{
		using (Game.Profile("Troops.UpdateRagdolls", false, 0f, null))
		{
			if (Troops.ragdolls.Count > UserSettings.MaxRagdollCount)
			{
				for (int i = 0; i < Troops.ragdolls.Count - UserSettings.MaxRagdollCount; i++)
				{
					Troops.ragdolls[0].EnableDecal(true);
					Troops.ragdolls.RemoveAt(0);
				}
			}
			for (int j = Troops.ragdolls.Count - 1; j >= 0; j--)
			{
				Troops.ragdolls[j].UpdateRigidBodies();
			}
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0007A338 File Offset: 0x00078538
	private unsafe static void DrawSalvo(Troops.SalvoData* salvo)
	{
		using (Game.Profile("Troops.DrawSalvo", false, 0f, null))
		{
			if (salvo->id < Troops.salvos.Count && !salvo->HasFlags(Troops.SalvoData.Flags.Destroyed))
			{
				bool flag = false;
				Troops.Arrow arrow = salvo->FirstArrow;
				while (arrow <= salvo->LastActiveArrow)
				{
					if (arrow.HasFlags(Troops.Arrow.Flags.Moving | Troops.Arrow.Flags.AboutToShoot | Troops.Arrow.Flags.BeingShot))
					{
						flag = true;
					}
					arrow = ++arrow;
				}
				if (salvo->HasFlags(Troops.SalvoData.Flags.Moving) && !flag)
				{
					salvo->ClrFlags(Troops.SalvoData.Flags.Moving);
					salvo->SetFlags(Troops.SalvoData.Flags.Landed);
					if (salvo->squad != null && salvo->squad->cur_salvo == salvo->id)
					{
						salvo->squad->cur_salvo = -1;
					}
					if (salvo->fortification != null && salvo->fortification->cur_salvo == salvo->id)
					{
						salvo->fortification->cur_salvo = -1;
					}
				}
			}
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0007A434 File Offset: 0x00078634
	private static void CalcFrustrum()
	{
		if (UnityEngine.Time.frameCount == Troops._frustrum_last_frame)
		{
			return;
		}
		Troops._frustrum_last_frame = UnityEngine.Time.frameCount;
		GameCamera gameCamera = CameraController.GameCamera;
		if (gameCamera == null)
		{
			return;
		}
		if (gameCamera != null)
		{
			Troops.main_cam_rot = gameCamera.transform.rotation;
		}
		for (int i = 0; i < gameCamera.f_planes.Length; i++)
		{
			Troops.f_planes[i] = gameCamera.f_planes[i];
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0007A4AC File Offset: 0x000786AC
	public unsafe static void InitFortifications(global::Fortification[] new_f)
	{
		Troops.fortifications = new_f;
		Troops.mem_data.NumFortifications = Troops.fortifications.Length;
		if (Troops.mem_data.fortifications.IsCreated)
		{
			Troops.mem_data.fortifications.Dispose();
		}
		Troops.mem_data.fortifications = new NativeArray<Troops.FortificationData>(Troops.fortifications.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		Troops.pdata->fortifications = (Troops.FortificationData*)Troops.mem_data.fortifications.GetUnsafePtr<Troops.FortificationData>();
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0007A521 File Offset: 0x00078721
	public static void SetPathData(PathData.DataPointers path_data)
	{
		Troops.path_data = path_data;
		Troops.collisions.path_data = path_data;
		Troops.fort_collisions.path_data = path_data;
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000A6F RID: 2671 RVA: 0x0007A53F File Offset: 0x0007873F
	// (set) Token: 0x06000A70 RID: 2672 RVA: 0x0007A546 File Offset: 0x00078746
	public static bool running { get; private set; }

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000A71 RID: 2673 RVA: 0x0007A54E File Offset: 0x0007874E
	public static bool Initted
	{
		get
		{
			return Troops.pdata != null;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x0007A55C File Offset: 0x0007875C
	public static bool NotReady
	{
		get
		{
			return BattleMap.battle == null || !BattleMap.battle.IsValid() || BattleMap.battle.stage < Logic.Battle.Stage.Ongoing;
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0007A580 File Offset: 0x00078780
	public static void Init(Game game = null)
	{
		if (Troops.Initted)
		{
			return;
		}
		FormationBurst.Compile();
		Troops.texture_baker = new TextureBaker();
		Troops.texture_baker.SetKingdomColors(BattleMap.Get().unit_colors);
		DT.Field field = global::Defs.Get(false).dt.Find("Unit", null);
		if (field != null)
		{
			Material material = field.GetRandomValue("dead_decals", null, true, true, true, '.').Get<Material>();
			if (material != null)
			{
				Troops.texture_baker.decal_drawer = new TextureBaker.InstancedDecalDrawerBatched(material, field.GetFloat("death_decal_appear_time", null, 0f, true, true, true, '.'));
			}
		}
		Troops.mem_data = new Troops.TroopsMemData(10000, 100);
		Troops.squads = new global::Squad[Troops.mem_data.SquadsCapacity];
		Troops.salvos = new List<Troops.Salvo>();
		Troops.sharedSkinningData = new Dictionary<string, Troops.SquadModelData>();
		Troops.cur_job = default(JobHandle);
		Troops.running = false;
		Troops.path_data_assigned = false;
		Troops.ragdolls = new List<Troops.RagdollInfo>();
		Troops.old_ragdolls = new List<Troops.RagdollInfo>();
		Troops.f_planes = new NativeArray<float4>(6, Allocator.Persistent, NativeArrayOptions.ClearMemory);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0007A68C File Offset: 0x0007888C
	public unsafe static void InitPointers()
	{
		Troops.pdata = (Troops.TroopsPtrData*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<Troops.TroopsPtrData>(), UnsafeUtility.AlignOf<Troops.TroopsPtrData>(), Allocator.Persistent);
		Troops.pdata->This = Troops.pdata;
		Troops.pdata->CopyFrom(ref Troops.mem_data);
		Troops.collisions = new TroopCollisions(Troops.pdata, Troops.mem_data.TroopsPerSquad * Troops.mem_data.SquadsCapacity, Troops.path_data);
		Troops.fort_collisions = new TroopCollisions(Troops.pdata, 200000, Troops.path_data);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0007A710 File Offset: 0x00078910
	public unsafe static void Done()
	{
		if (!Troops.Initted)
		{
			return;
		}
		Troops.CompleteAllJobs();
		Troops.squads = null;
		Troops.mem_data.Dispose();
		Troops.f_planes.Dispose();
		UnsafeUtility.Free((void*)Troops.pdata, Allocator.Persistent);
		Troops.pdata = null;
		Troops.collisions.Dispose();
		Troops.fort_collisions.Dispose();
		Troops.ragdolls = null;
		Troops.old_ragdolls = null;
		TextureBaker.InstancedArrowDrawer.ClearMeshes();
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0007A77B File Offset: 0x0007897B
	public static global::Squad GetSquad(int squad_id)
	{
		return Troops.squads[squad_id];
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0007A784 File Offset: 0x00078984
	public static Troops.Salvo GetSalvo(int salvo_id)
	{
		return Troops.salvos[salvo_id];
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0007A791 File Offset: 0x00078991
	public static void ClearSkinningCache()
	{
		if (Troops.sharedSkinningData == null)
		{
			return;
		}
		Troops.sharedSkinningData.Clear();
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0007A7A8 File Offset: 0x000789A8
	public unsafe static void AddSquad(global::Squad squad_visual)
	{
		if (!Troops.Initted)
		{
			Debug.LogError("Troops.AddSquad() while Troops is not initialized");
			return;
		}
		Logic.Unit.Def def = squad_visual.def;
		Troops.CompleteAllJobs();
		if (squad_visual == null)
		{
			throw new Exception("No visuals for squad");
		}
		if (squad_visual.logic == null)
		{
			throw new Exception("No logic for squad " + ((def != null) ? def.field.key : null));
		}
		int num = -1;
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			if (Troops.squads[i] == null)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			Troops.DelSquad(squad_visual);
			throw new OverflowException("No free slot for squad " + ((def != null) ? def.field.key : null));
		}
		Troops.DefData* def2 = Troops.pdata->GetDef(squad_visual.def.troops_def_idx);
		Troops.squads[num] = squad_visual;
		float cth = squad_visual.def.CTH;
		float defense = squad_visual.def.defense;
		float resilience = squad_visual.def.resilience;
		float chance_to_shock = squad_visual.def.chance_to_shock;
		float shock_damage_base = squad_visual.logic.simulation.unit.shock_damage_base();
		float shock_damage_trample = squad_visual.logic.simulation.unit.shock_damage_bonus_trample();
		float max_health = squad_visual.logic.simulation.unit.max_health_modified();
		int j = squad_visual.logic.NumTroops();
		float friendly_fire_mod = 0f;
		int kingdom_color_id = 0;
		BattleMap battleMap = BattleMap.Get();
		Color rhs;
		if (battleMap.kingdom_colors != null && battleMap.kingdom_colors.TryGetValue(squad_visual.kingdom, out rhs))
		{
			for (int k = 0; k < battleMap.unit_colors.Length; k++)
			{
				if (battleMap.unit_colors[k] == rhs)
				{
					kingdom_color_id = k;
					break;
				}
			}
		}
		Troops.mem_data.squads[num] = new Troops.SquadData(Troops.pdata, squad_visual.logic.battle_side, def2, num, -1, num * Troops.mem_data.TroopsPerSquad, j, cth, defense, resilience, chance_to_shock, shock_damage_base, shock_damage_trample, friendly_fire_mod, squad_visual.logic.movement.allowed_non_ladder_areas, kingdom_color_id, max_health);
		Troops.SquadData* squad = Troops.pdata->GetSquad(num);
		squad_visual.SetID(num);
		Troops.pdata->CopyCounts(ref Troops.mem_data);
		Troops.SquadData.Ptr* ptr = Troops.pdata->squad + squad->offset;
		int* ptr2 = Troops.pdata->form_idx + squad->offset;
		int* ptr3 = Troops.pdata->enemy_id + squad->offset;
		byte* ptr4 = Troops.pdata->fight_status + squad->offset;
		bool2* ptr5 = Troops.pdata->flanking_status + squad->offset;
		float4* ptr6 = Troops.pdata->cooldowns + squad->offset;
		int num2 = 0;
		while (j > 0)
		{
			ptr->ptr = squad;
			ptr++;
			*(ptr2++) = num2++;
			*(ptr3++) = -1;
			*(ptr4++) = 0;
			*(ptr5++) = false;
			*(ptr6++) = 0;
			j--;
		}
		Troops.Troop troop = squad->FirstTroop;
		while (troop <= squad->LastTroop)
		{
			troop.ClrFlags((Troops.Troop.Flags)(-1));
			troop.thrown_dir = 0;
			troop = ++troop;
		}
		Troops.RefreshDrawers(squad_visual);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0007AB38 File Offset: 0x00078D38
	public unsafe static void RefreshDrawers(global::Squad squad_visual)
	{
		if (squad_visual.data == null)
		{
			return;
		}
		Troops.SquadData* data = squad_visual.data;
		Logic.Unit.Def visualDef = Logic.Squad.GetVisualDef(squad_visual.logic);
		bool flag = squad_visual.logic.visual_def != visualDef;
		global::Squad.LoadBakedSkinningData(visualDef);
		Troops.SquadModelData model_data;
		if (!Troops.sharedSkinningData.TryGetValue(visualDef.field.key, out model_data))
		{
			throw new Exception("Missing model data for " + ((visualDef != null) ? visualDef.field.key : null));
		}
		squad_visual.model_data = model_data;
		squad_visual.logic.visual_def = visualDef;
		if (flag)
		{
			squad_visual.SpawnBanner(visualDef);
			data->def = Troops.pdata->GetDef(visualDef.troops_def_idx);
			squad_visual.logic.RecalcMoveSpeed();
			squad_visual.logic.SetMoveSpeed(squad_visual.logic.normal_move_speed);
			squad_visual.logic.last_shoot_time = 0f;
		}
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (troop.squad_id == squad_visual.GetID())
			{
				troop.SetDrawerInfo(squad_visual, flag);
			}
			troop = ++troop;
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0007AC54 File Offset: 0x00078E54
	public unsafe static void AddSubSquad(global::Squad squad_visual, int main_squad_id, int[] subSquadTroopsId)
	{
		if (!Troops.Initted)
		{
			Debug.LogError("Troops.AddSquad() while Troops is not initialized");
			return;
		}
		Logic.Unit.Def def = squad_visual.def;
		Troops.CompleteAllJobs();
		if (subSquadTroopsId == null || subSquadTroopsId.Length == 0)
		{
			throw new Exception("No Subsquad troops IDs");
		}
		if (squad_visual == null)
		{
			throw new Exception("No visuals for squad");
		}
		if (squad_visual.logic == null)
		{
			throw new Exception("No logic for squad " + ((def != null) ? def.field.key : null));
		}
		int emptySquad = Troops.GetEmptySquad();
		if (emptySquad < 0)
		{
			Troops.DelSquad(squad_visual);
			throw new OverflowException("No free slot for squad " + ((def != null) ? def.field.key : null));
		}
		Troops.DefData* def2 = Troops.pdata->GetDef(squad_visual.def.troops_def_idx);
		Troops.squads[emptySquad] = squad_visual;
		float cth = squad_visual.def.CTH;
		float defense = squad_visual.def.defense;
		float resilience = squad_visual.def.resilience;
		float chance_to_shock = squad_visual.def.chance_to_shock;
		float shock_damage_base = squad_visual.logic.simulation.unit.shock_damage_base();
		float shock_damage_trample = squad_visual.logic.simulation.unit.shock_damage_bonus_trample();
		float max_health = squad_visual.logic.simulation.unit.max_health_modified();
		Troops.SquadData* squad = Troops.pdata->GetSquad(main_squad_id);
		int size = squad->size;
		float friendly_fire_mod = 0f;
		Troops.mem_data.squads[emptySquad] = new Troops.SquadData(Troops.pdata, squad_visual.logic.battle_side, def2, emptySquad, main_squad_id, main_squad_id * Troops.mem_data.TroopsPerSquad, size, cth, defense, resilience, chance_to_shock, shock_damage_base, shock_damage_trample, friendly_fire_mod, squad_visual.logic.movement.allowed_non_ladder_areas, squad->kingdom_color_id, max_health);
		Troops.SquadData* squad2 = Troops.pdata->GetSquad(emptySquad);
		squad2->logic_alive = subSquadTroopsId.Length;
		squad2->ClrFlags(Troops.SquadData.Flags.Teleport);
		squad_visual.SetID(emptySquad);
		squad_visual.SetMainSquadID(main_squad_id);
		PPos pos;
		PPos pt;
		squad_visual.logic.CalcPos(out pos, out pt, 0f, 0f, false);
		squad2->pos = pos;
		squad2->dir = pt;
		Troops.pdata->CopyCounts(ref Troops.mem_data);
		int num = subSquadTroopsId.Length;
		int num2 = subSquadTroopsId.Length;
		Troops.SquadData.Ptr* ptr = Troops.pdata->squad + squad2->offset;
		Troops.Troop troop = squad2->FirstTroop;
		while (troop <= squad2->LastTroop)
		{
			for (int i = 0; i < subSquadTroopsId.Length; i++)
			{
				if (troop.id == subSquadTroopsId[i])
				{
					if (troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						num--;
						if (troop.HasFlags(Troops.Troop.Flags.Destroyed))
						{
							num2--;
						}
					}
					ptr->ptr = squad2;
					break;
				}
			}
			ptr++;
			troop = ++troop;
		}
		if (squad2->logic_alive != num)
		{
			squad2->logic_alive = num;
		}
		Troops.RefreshDrawers(squad_visual);
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0007AF44 File Offset: 0x00079144
	public static int GetEmptySquad()
	{
		int result = -1;
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			if (Troops.squads[i] == null)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0007AF7C File Offset: 0x0007917C
	public unsafe static void DelSquad(global::Squad squad)
	{
		if (squad == null)
		{
			return;
		}
		if (Troops.squads == null)
		{
			return;
		}
		int id = squad.GetID();
		if (id >= 0)
		{
			Troops.squads[id] = null;
		}
		if (squad.logic != null && squad.logic.IsValid())
		{
			squad.logic.Destroy(false);
		}
		if (!Troops.Initted || id < 0)
		{
			return;
		}
		if (squad.data != null && !squad.data->HasFlags(Troops.SquadData.Flags.Destroyed))
		{
			squad.data->SetFlags(Troops.SquadData.Flags.Destroyed);
		}
		squad.SetID(-1);
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x0007B00E File Offset: 0x0007920E
	public static void ScheduleJob<T>(T job) where T : struct, IJob
	{
		if (Troops.debug_jobs)
		{
			string name = job.ToString();
			Profile.BeginSection(name);
			job.Run<T>();
			Profile.EndSection(name);
			return;
		}
		Troops.running = true;
		Troops.cur_job = job.Schedule(Troops.cur_job);
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0007B04C File Offset: 0x0007924C
	public static void ScheduleJob<T>(T job, int count, int batch) where T : struct, IJobParallelFor
	{
		if (Troops.debug_jobs)
		{
			string name = job.ToString();
			Profile.BeginSection(name);
			job.Run(count);
			Profile.EndSection(name);
			return;
		}
		Troops.running = true;
		Troops.cur_job = job.Schedule(count, batch, Troops.cur_job);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0007B098 File Offset: 0x00079298
	public static void SchedulePerSquadPFJob<T>(T job) where T : struct, IJobParallelFor
	{
		Troops.ScheduleJob<T>(job, Troops.mem_data.SquadsCapacity, 1);
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0007B0AB File Offset: 0x000792AB
	public static void SchedulePerSalvoPFJob<T>(T job) where T : struct, IJobParallelFor
	{
		Troops.ScheduleJob<T>(job, Troops.mem_data.NumSalvos, 1);
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0007B0C0 File Offset: 0x000792C0
	public static void ScheduleAction(Action action)
	{
		Troops.running = true;
		Troops.ActionWrapper actionWrapper = new Troops.ActionWrapper(action);
		Troops.DisposeGCHandleJob job = new Troops.DisposeGCHandleJob(actionWrapper.action_handle);
		Troops.ScheduleJob<Troops.ActionWrapper>(actionWrapper);
		Troops.ScheduleJob<Troops.DisposeGCHandleJob>(job);
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0007B0F4 File Offset: 0x000792F4
	public static void SchedulePerSquadAction(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.SquadAction action)
	{
		Troops.SquadActionWrapper squadActionWrapper = new Troops.SquadActionWrapper(action, squads_filter, call_subsquads);
		Troops.DisposeGCHandleJob job = new Troops.DisposeGCHandleJob(squadActionWrapper.action_handle);
		Troops.SchedulePerSquadPFJob<Troops.SquadActionWrapper>(squadActionWrapper);
		Troops.ScheduleJob<Troops.DisposeGCHandleJob>(job);
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0007B121 File Offset: 0x00079321
	public static void SchedulePerTroopJob_FindTroopEnemiesJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.FindTroopEnemiesJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.FindTroopEnemiesJob>>(new Troops.TroopJobWrapper<Troops.FindTroopEnemiesJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0007B136 File Offset: 0x00079336
	private static void SchedulePerSquadJob_CalcKilledTroopsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.CalcKilledTroopsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.SquadJobWrapper<Troops.CalcKilledTroopsJob>>(new Troops.SquadJobWrapper<Troops.CalcKilledTroopsJob>(Troops.pdata, job, squads_filter, call_subsquads));
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0007B14A File Offset: 0x0007934A
	private static void SchedulePerTroopJob_ChargeJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.ChargeJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.ChargeJob>>(new Troops.TroopJobWrapper<Troops.ChargeJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0007B15F File Offset: 0x0007935F
	private static void SchedulePerTroopJob_ClimbLadderProgressJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.ClimbLadderProgressJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.ClimbLadderProgressJob>>(new Troops.TroopJobWrapper<Troops.ClimbLadderProgressJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0007B174 File Offset: 0x00079374
	private static void SchedulePerArrowJob_MoveArrowsJob(Troops.SalvosFilter salvos_filter, Troops.ArrowsFilter arrows_filter, Troops.MoveArrowsJob job)
	{
		Troops.SchedulePerSalvoPFJob<Troops.ArrowJobWrapper<Troops.MoveArrowsJob>>(new Troops.ArrowJobWrapper<Troops.MoveArrowsJob>(Troops.pdata, job, salvos_filter, arrows_filter));
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0007B188 File Offset: 0x00079388
	private static void SchedulePerArrowJob_ArrowTrailsJob(Troops.SalvosFilter salvos_filter, Troops.ArrowsFilter arrows_filter, Troops.ArrowTrailsJob job)
	{
		Troops.SchedulePerSalvoPFJob<Troops.ArrowJobWrapper<Troops.ArrowTrailsJob>>(new Troops.ArrowJobWrapper<Troops.ArrowTrailsJob>(Troops.pdata, job, salvos_filter, arrows_filter));
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0007B19C File Offset: 0x0007939C
	private static void SchedulePerTroopJob_PushOffJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.PushOffJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.PushOffJob>>(new Troops.TroopJobWrapper<Troops.PushOffJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0007B1B1 File Offset: 0x000793B1
	private static void SchedulePerTroopJob_UpdateTroopRotationsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.UpdateTroopRotationsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.UpdateTroopRotationsJob>>(new Troops.TroopJobWrapper<Troops.UpdateTroopRotationsJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0007B1C6 File Offset: 0x000793C6
	private static void SchedulePerTroopJob_RebuildCollisionsGridJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.RebuildCollisionsGridJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.RebuildCollisionsGridJob>>(new Troops.TroopJobWrapper<Troops.RebuildCollisionsGridJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0007B1DB File Offset: 0x000793DB
	private static void SchedulePerTroopJob_SnapTroopsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.SnapTroopsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.SnapTroopsJob>>(new Troops.TroopJobWrapper<Troops.SnapTroopsJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0007B1F0 File Offset: 0x000793F0
	private static void SchedulePerTroopJob_MoveTroopsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.MoveTroopsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.MoveTroopsJob>>(new Troops.TroopJobWrapper<Troops.MoveTroopsJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0007B205 File Offset: 0x00079405
	private static void SchedulePerTroopJob_TroopsObstacleAvoidanceJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.TroopsObstacleAvoidanceJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.TroopsObstacleAvoidanceJob>>(new Troops.TroopJobWrapper<Troops.TroopsObstacleAvoidanceJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0007B21A File Offset: 0x0007941A
	private static void SchedulePerTroopJob_CalcTroopVelocities(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.CalcTroopVelocities job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.CalcTroopVelocities>>(new Troops.TroopJobWrapper<Troops.CalcTroopVelocities>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0007B22F File Offset: 0x0007942F
	private static void SchedulePerTroopJob_AdjustTroopTargetsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.AdjustTroopTargetsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.AdjustTroopTargetsJob>>(new Troops.TroopJobWrapper<Troops.AdjustTroopTargetsJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0007B244 File Offset: 0x00079444
	private static void SchedulePerTroopJob_PushOffUpdateJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.PushOffUpdateJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.PushOffUpdateJob>>(new Troops.TroopJobWrapper<Troops.PushOffUpdateJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0007B259 File Offset: 0x00079459
	private static void SchedulePerArrowJob_FillArrowRenderDataJob(Troops.SalvosFilter salvos_filter, Troops.ArrowsFilter arrows_filter, Troops.FillArrowRenderDataJob job)
	{
		Troops.SchedulePerSalvoPFJob<Troops.ArrowJobWrapper<Troops.FillArrowRenderDataJob>>(new Troops.ArrowJobWrapper<Troops.FillArrowRenderDataJob>(Troops.pdata, job, salvos_filter, arrows_filter));
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0007B26D File Offset: 0x0007946D
	private static void SchedulePerTroopJob_FillTroopRenderDataJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.FillTroopRenderDataJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.FillTroopRenderDataJob>>(new Troops.TroopJobWrapper<Troops.FillTroopRenderDataJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0007B282 File Offset: 0x00079482
	private static void SchedulePerTroopJob_DustParticlesJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.DustParticlesJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.DustParticlesJob>>(new Troops.TroopJobWrapper<Troops.DustParticlesJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0007B297 File Offset: 0x00079497
	private static void SchedulePerTroopJob_UpdateTroopAnimationsJob(bool call_subsquads, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, Troops.UpdateTroopAnimationsJob job)
	{
		Troops.SchedulePerSquadPFJob<Troops.TroopJobWrapper<Troops.UpdateTroopAnimationsJob>>(new Troops.TroopJobWrapper<Troops.UpdateTroopAnimationsJob>(Troops.pdata, job, squads_filter, troops_filter, call_subsquads));
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0007B2AC File Offset: 0x000794AC
	public static void SchedulePerArrowJob<T>(Troops.SalvosFilter salvos_filter, Troops.ArrowsFilter arrows_filter, T job) where T : struct, Troops.IArrowJob
	{
		Troops.SchedulePerSalvoPFJob<Troops.ArrowJobWrapper<T>>(new Troops.ArrowJobWrapper<T>(Troops.pdata, job, salvos_filter, arrows_filter));
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0007B2C0 File Offset: 0x000794C0
	public static void StartScheduledJobs()
	{
		JobHandle.ScheduleBatchedJobs();
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0007B2C8 File Offset: 0x000794C8
	public static void CompleteAllJobs()
	{
		using (Game.Profile("Troops.CompleteAllJobs", false, 0f, null))
		{
			Troops.cur_job.Complete();
			Troops.cur_job = default(JobHandle);
			Troops.running = false;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x0007B324 File Offset: 0x00079524
	// (set) Token: 0x06000A9B RID: 2715 RVA: 0x0007B337 File Offset: 0x00079537
	public static bool debug_jobs
	{
		get
		{
			return JobsUtility.JobDebuggerEnabled && !JobsUtility.JobCompilerEnabled;
		}
		set
		{
			JobsUtility.JobDebuggerEnabled = value;
			JobsUtility.JobCompilerEnabled = !value;
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0007B348 File Offset: 0x00079548
	public unsafe static Troops.SalvoData* AddSalvo(Troops.SalvoDefData* def)
	{
		Troops.CompleteAllJobs();
		int numSalvos = Troops.mem_data.NumSalvos;
		Troops.mem_data.NumSalvos = Troops.mem_data.NumSalvos + 1;
		Troops.mem_data.salvos[numSalvos] = new Troops.SalvoData(Troops.pdata, numSalvos, numSalvos * 100, def);
		Troops.SalvoData* salvo = Troops.pdata->GetSalvo(numSalvos);
		Troops.pdata->CopyCounts(ref Troops.mem_data);
		Troops.SalvoData.Ptr* ptr = Troops.pdata->salvo + salvo->offset;
		for (int i = 100; i > 0; i--)
		{
			ptr->ptr = salvo;
			ptr++;
		}
		return salvo;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0007B3E8 File Offset: 0x000795E8
	public static TextureBaker.PerArrowModelData LoadArrowDrawer(Logic.SalvoData.Def def)
	{
		foreach (KeyValuePair<string, TextureBaker.PerArrowModelData> keyValuePair in Troops.texture_baker.arrow_drawers)
		{
			if (keyValuePair.Key == def.field.key)
			{
				return keyValuePair.Value;
			}
		}
		return Troops.texture_baker.AddArrowDrawer(def.field);
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0007B470 File Offset: 0x00079670
	public unsafe static Troops.SalvoData* RecycleSalvo(Logic.SalvoData.Def salvo_def, out int newIndex, float CTH_final, float CTH_cavalry_mod, float friendly_fire_reduction, int battle_side, Troops.SquadData* squad, float3 end)
	{
		Troops.SalvoDefData* salvoDef = Troops.pdata->GetSalvoDef(salvo_def.troops_def_idx);
		TextureBaker.PerArrowModelData perArrowModelData = Troops.LoadArrowDrawer(salvo_def);
		if (Troops.mem_data.NumSalvos < 100)
		{
			Troops.SalvoData* ptr = Troops.AddSalvo(salvoDef);
			Troops.salvos.Add(new Troops.Salvo
			{
				data = ptr
			});
			newIndex = ptr->id;
		}
		else
		{
			newIndex = (Troops.mem_data.NextSalvo + 1) % 100;
			Troops.GetSalvo(newIndex);
			Troops.mem_data.NextSalvo = newIndex;
			Troops.SalvoData* salvo = Troops.pdata->GetSalvo(newIndex);
			salvo->ClrFlags(Troops.SalvoData.Flags.Destroyed);
			salvo->squad = null;
			salvo->fortification = null;
			salvo->def = salvoDef;
			Troops.Arrow arrow = salvo->FirstArrow;
			while (arrow <= salvo->LastActiveArrow)
			{
				arrow.Reset();
				arrow = ++arrow;
			}
		}
		Troops.SalvoData* salvo2 = Troops.pdata->GetSalvo(newIndex);
		salvo2->CTH_final = CTH_final;
		salvo2->CTH_cavalry_mod = CTH_cavalry_mod;
		salvo2->friendly_fire_mod = salvo_def.friendly_fire_mod - friendly_fire_reduction;
		salvo2->ClrFlags(Troops.SalvoData.Flags.Active);
		salvo2->battle_side = battle_side;
		salvo2->squad = squad;
		salvo2->end = end;
		salvo2->model_data_buffer = (void*)perArrowModelData.model_data_buffer.data;
		return salvo2;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0007B5A0 File Offset: 0x000797A0
	public unsafe static int ShootSalvo(Logic.Unit.Def def, Logic.SalvoData.Def salvo_def, float CTH_final, float friendly_fire_reduction, float3 end, int battle_side, Troops.SquadData* squad, Troops.SquadData* enemy = null, bool high_angle = false)
	{
		int num = -1;
		Troops.SalvoData* ptr = Troops.RecycleSalvo(salvo_def, out num, CTH_final, def.CTH_cavalry_mod, friendly_fire_reduction, battle_side, squad, end);
		float max_random_shoot_offset = def.max_random_shoot_offset;
		int num2 = 0;
		if (enemy != null)
		{
			float2 rhs = enemy->pos - end.xz;
			end.xz = enemy->BoundingBoxCenter - rhs;
			ptr->end = end;
			num2 = enemy->pos.paID;
		}
		Troops.Troop troop = squad->FirstTroop;
		int num3 = 0;
		int num4 = 0;
		int i = 0;
		while (i < ptr->size)
		{
			bool flag = false;
			Troops.Troop troop2 = troop;
			while (troop2 <= squad->LastTroop)
			{
				if (!troop2.HasFlags(Troops.Troop.Flags.Fighting | Troops.Troop.Flags.Attacking | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop2.squad_id == squad->id)
				{
					troop = troop2;
					flag = true;
					break;
				}
				troop2 = ++troop2;
			}
			if (!flag)
			{
				if (num4 <= 0)
				{
					squad->cur_salvo = -1;
					return -1;
				}
				break;
			}
			else
			{
				troop.arrow_count = 0;
				for (int j = 0; j < salvo_def.arrows_per_troop; j++)
				{
					Troops.Arrow arrow = new Troops.Arrow(Troops.pdata, 0, ptr->offset + num4);
					num4++;
					arrow.Reset();
					arrow.cur_arrow_troop = troop.id;
					troop.SetArrow(j, arrow.id);
					int arrow_count = troop.arrow_count;
					troop.arrow_count = arrow_count + 1;
					arrow.SetFlags(Troops.Arrow.Flags.AboutToShoot);
					float3 @float = new Vector3(0f, 0.5f, 0f);
					int num5 = num2;
					float y = 0f;
					if (enemy == null)
					{
						@float += ptr->end;
					}
					else
					{
						for (int k = 0; k < Troops.mem_data.TroopsPerSquad; k++)
						{
							int num6 = (k + num3) % enemy->size;
							Troops.Troop troop3 = Troops.pdata->GetTroop(enemy->thread_id, enemy->offset + num6);
							if (!troop3.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop3.squad_id == enemy->id)
							{
								num3 = num6 + 1;
								float4 vel_spd_t = troop3.vel_spd_t;
								@float += troop3.pos + vel_spd_t.xy * vel_spd_t.z * math.length(troop3.pos2d - troop.pos2d) / (0.707f * salvo_def.min_shoot_speed);
								num5 = troop3.pos.paID;
								y = troop3.pos3d.y;
								break;
							}
						}
					}
					@float += math.normalize(new float3(arrow.data->rand.NextFloat(-1f, 1f), 0f, arrow.data->rand.NextFloat(-1f, 1f))) * arrow.data->rand.NextFloat(-max_random_shoot_offset, max_random_shoot_offset);
					if (num5 == 0)
					{
						@float = global::Common.SnapToTerrain(@float, 0f, null, -1f, false);
					}
					float3 rhs2 = math.normalize(new float3(arrow.data->rand.NextFloat(-1f, 1f), 0f, arrow.data->rand.NextFloat(-1f, 1f))) * arrow.data->rand.NextFloat(0f, salvo_def.max_end_position_offset);
					@float += rhs2;
					@float = global::Common.SnapToTerrain(@float, 0f, null, -1f, false);
					arrow.target_paid = 0;
					if (num5 != 0 && !Troops.path_data.GetPA(num5 - 1).IsGround())
					{
						arrow.target_paid = num5;
					}
					if (num2 != 0)
					{
						PathData.PassableArea pa = Troops.path_data.GetPA(num2 - 1);
						if (!pa.IsGround())
						{
							if (pa.Contains(@float.xz))
							{
								@float.y = math.max(end.y, pa.GetHeight(end));
							}
							else if (num5 != 0)
							{
								@float.y = math.max(end.y, y);
							}
						}
					}
					if (high_angle)
					{
						@float.y += 0.6f;
					}
					else
					{
						@float.y += 0.35f;
					}
					arrow.endPos = @float;
					arrow.is_high_angle = high_angle;
					arrow.t = -troop.data->rand.NextFloat(0f, salvo_def.random_shoot_time_offset);
					arrow.scale = 0f;
				}
				Troops.Arrow firstArrow = ptr->FirstArrow;
				float num7;
				if (Troops.CalcRot(troop.pos, firstArrow.endPos.xz, 4f, out num7))
				{
					if (num7 >= 360f)
					{
						num7 -= 360f;
					}
					if (num7 < 0f)
					{
						num7 += 360f;
					}
					troop.tgt_arrow_rot = num7;
				}
				troop.SetFlags(Troops.Troop.Flags.ShootTrigger);
				troop = ++troop;
				i += salvo_def.arrows_per_troop;
			}
		}
		if (num4 == 0)
		{
			num = -1;
		}
		squad->cur_salvo = num;
		ptr->SetFlags(Troops.SalvoData.Flags.Moving);
		ptr->num_arrows = num4;
		return num;
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0007BB18 File Offset: 0x00079D18
	public unsafe static int ShootSalvo(Logic.Fortification logic, Logic.SalvoData.Def salvo_def, float CTH_final, float friendly_fire_reduction, float3 start, float3 end, int battle_side, Troops.FortificationData* fortification, Troops.SquadData* enemy = null)
	{
		Logic.Fortification.Def def = logic.def;
		int result = -1;
		Troops.SalvoData* ptr = Troops.RecycleSalvo(salvo_def, out result, CTH_final, 1f, friendly_fire_reduction, battle_side, null, end);
		float num = 5f;
		int num2 = 0;
		if (enemy != null)
		{
			float2 rhs = enemy->pos - end.xz;
			end.xz = enemy->BoundingBoxCenter - rhs;
			ptr->end = end;
			num = math.max(enemy->BoundingBoxRadius, 5f);
			num2 = enemy->pos.paID;
		}
		int num3 = 0;
		int num4 = 0;
		Point pt = (end - start).GetNormalized().Right(0f);
		Troops.Arrow arrow = ptr->FirstArrow;
		while (arrow <= ptr->LastArrow && num4 < logic.num_arrows)
		{
			num4++;
			arrow.Reset();
			float3 @float = new Vector3(0f, 0.5f, 0f);
			int num5 = num2;
			float y = 0f;
			if (enemy == null)
			{
				@float += global::Common.SnapToTerrain(ptr->end + math.normalize(new float3(arrow.data->rand.NextFloat(-1f, 1f), 0f, arrow.data->rand.NextFloat(-1f, 1f))) * arrow.data->rand.NextFloat(-num, num), 0f, null, -1f, false);
			}
			else
			{
				for (int i = 0; i < Troops.mem_data.TroopsPerSquad; i++)
				{
					int num6 = (i + num3) % enemy->size;
					Troops.Troop troop = Troops.pdata->GetTroop(enemy->thread_id, enemy->offset + num6);
					if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == enemy->id)
					{
						num3 = num6 + 1;
						float4 vel_spd_t = troop.vel_spd_t;
						@float += global::Common.SnapToTerrain(troop.pos + vel_spd_t.xy * vel_spd_t.z * math.length(troop.pos2d - start.xz) / salvo_def.min_shoot_speed, 0f, null, -1f, false);
						num5 = troop.pos.paID;
						y = troop.pos3d.y;
						break;
					}
				}
			}
			float3 rhs2 = math.normalize(new float3(arrow.data->rand.NextFloat(-1f, 1f), 0f, arrow.data->rand.NextFloat(-1f, 1f))) * arrow.data->rand.NextFloat(0f, salvo_def.max_end_position_offset);
			@float += rhs2;
			arrow.target_paid = 0;
			if (num5 != 0 && !Troops.path_data.GetPA(num5 - 1).IsGround())
			{
				arrow.target_paid = num5;
			}
			if (num2 != 0)
			{
				PathData.PassableArea pa = Troops.path_data.GetPA(num2 - 1);
				if (!pa.IsGround())
				{
					if (pa.Contains(@float.xz))
					{
						@float.y = math.max(end.y, pa.GetHeight(end));
					}
					else if (num5 != 0)
					{
						@float.y = math.max(end.y, y);
					}
				}
			}
			@float.y += 0.35f;
			arrow.endPos = @float;
			float3 float2 = start;
			float shoot_height = salvo_def.shoot_height;
			float2.y += shoot_height;
			float3 rhs3 = arrow.data->rand.NextFloat(-1f, 1f) * salvo_def.width_randomized_offset * pt;
			float2 += rhs3;
			float num7 = arrow.data->rand.NextFloat(0f, 1f) * salvo_def.height_randomized_offset;
			float2.y += num7;
			arrow.startPos = float2;
			arrow.endPos = @float;
			arrow.pos = float2;
			float num8 = 10f;
			float num9 = math.length(math.float2(@float.x, @float.z) - math.float2(float2.x, float2.z));
			float num10 = float2.y - @float.y;
			float num11 = Mathf.Sqrt(num9 * num8);
			float min_shoot_speed = arrow.salvo->def->min_shoot_speed;
			if (num11 < min_shoot_speed)
			{
				num11 = min_shoot_speed;
			}
			num11 += arrow.data->rand.NextFloat() * arrow.salvo->def->shoot_speed_randomization_mod * num11;
			float num15;
			float num16;
			if (num10 >= 0f)
			{
				float num12 = num11 * num11;
				float num13 = math.atan((num12 - math.sqrt(num12 * num12 - 2f * num12 * -num10 * num8 - num8 * num8 * num9 * num9)) / (num8 * num9));
				float num14;
				if (num13 > -1.5707964f && num13 < 1.5707964f)
				{
					num14 = num13;
				}
				else
				{
					num14 = 0.7853982f;
					num11 = math.sqrt(num9 * num8);
				}
				num14 += Troops.pdata->rand.NextFloat() * num14 * 0.1f;
				num11 += Troops.pdata->rand.NextFloat() * 0.5f;
				num15 = math.cos(num14) * num11;
				num16 = math.sin(num14) * num11;
			}
			else
			{
				num10 *= -1f;
				float num17 = num11 * num11;
				float num18 = math.atan((num17 - math.sqrt(num17 * num17 - 2f * num17 * -num10 * num8 - num8 * num8 * num9 * num9)) / (num8 * num9));
				float num14;
				if (num18 > -1.5707964f && num18 < 1.5707964f)
				{
					num14 = num18;
				}
				else
				{
					num14 = 0.7853982f;
					num11 = math.sqrt(num9 * num8);
				}
				num11 += Troops.pdata->rand.NextFloat() * 0.5f;
				num14 += Troops.pdata->rand.NextFloat() * num14 * 0.1f;
				num15 = math.cos(num14) * num11;
				float num19 = num9 / num15;
				num16 = -(math.sin(num14) * num11 - num8 * num19);
				num14 = Mathf.Atan(num16 / num15);
			}
			arrow.startVelocity = new float2(num15, num16);
			float num20 = num9 / num15 + 0.2f;
			arrow.duration = num20;
			float num21 = num16 * 0.1f;
			float y2 = num10 + num16 * num16 * 0.5f * 0.1f;
			float2 float3 = math.lerp(new float2(float2.x, float2.z), new float2(@float.x, @float.z), num21 / num20);
			arrow.midPoint = new float3(float3.x, y2, float3.y);
			arrow.SetFlags(Troops.Arrow.Flags.Moving);
			arrow.t = -Troops.pdata->rand.NextFloat(0f, salvo_def.random_shoot_time_offset);
			arrow.scale = 0f;
			arrow = ++arrow;
		}
		ptr->SetFlags(Troops.SalvoData.Flags.Moving);
		ptr->num_arrows = num4;
		return result;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0007C2E0 File Offset: 0x0007A4E0
	public unsafe static int ShootSalvo(Logic.SalvoData.Def salvo_def, float CTH_final, float friendly_fire_reduction, float3 start, float3 end, int battle_side, Troops.SquadData* enemy = null, float random_offset_mult = 1f)
	{
		int result = -1;
		Troops.SalvoData* ptr = Troops.RecycleSalvo(salvo_def, out result, CTH_final, 1f, friendly_fire_reduction, battle_side, null, end);
		float num = 5f;
		int num2 = 0;
		if (enemy != null)
		{
			float2 rhs = enemy->pos - end.xz;
			end.xz = enemy->BoundingBoxCenter - rhs;
			ptr->end = end;
			num = math.max(enemy->BoundingBoxRadius, 5f);
			num2 = enemy->pos.paID;
		}
		num *= random_offset_mult;
		int num3 = 0;
		int num4 = 0;
		Troops.Arrow arrow = ptr->FirstArrow;
		while (arrow <= ptr->LastArrow && num4 < 1)
		{
			num4++;
			arrow.Reset();
			float3 @float = new Vector3(0f, 0.5f, 0f);
			if (enemy == null)
			{
				@float += global::Common.SnapToTerrain(ptr->end + math.normalize(new float3(arrow.data->rand.NextFloat(-1f, 1f), 0f, arrow.data->rand.NextFloat(-1f, 1f))) * arrow.data->rand.NextFloat(-num, num), 0f, null, -1f, false);
			}
			else
			{
				for (int i = 0; i < Troops.mem_data.TroopsPerSquad; i++)
				{
					int num5 = (i + num3) % enemy->size;
					Troops.Troop troop = Troops.pdata->GetTroop(enemy->thread_id, enemy->offset + num5);
					if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
					{
						num3 = num5 + 1;
						float4 vel_spd_t = troop.vel_spd_t;
						@float += global::Common.SnapToTerrain(troop.pos + vel_spd_t.xy * vel_spd_t.z * 2f, 0f, null, -1f, false);
						break;
					}
				}
			}
			if (num2 != 0)
			{
				PathData.PassableArea pa = Troops.path_data.GetPA(num2 - 1);
				if (pa.Contains(@float.xz))
				{
					@float.y = math.max(end.y, pa.GetHeight(end));
				}
			}
			arrow.endPos = @float;
			float3 float2 = start;
			float shoot_height = salvo_def.shoot_height;
			float2.y += shoot_height;
			arrow.startPos = float2;
			arrow.endPos = @float;
			arrow.pos = float2;
			float num6 = 1000f + Troops.pdata->rand.NextFloat() * 5f;
			float num7 = 10f;
			float num8 = math.length(math.float2(@float.x, @float.z) - math.float2(float2.x, float2.z));
			float num9 = float2.y - @float.y;
			float num12;
			float num13;
			if (num9 >= 0f)
			{
				float num10 = num6 * num6;
				float num11 = math.atan((num10 - math.sqrt(num10 * num10 - 2f * num10 * -num9 * num7 - num7 * num7 * num8 * num8)) / (num7 * num8));
				float x;
				if (num11 > -1.5707964f && num11 < 1.5707964f)
				{
					x = num11;
				}
				else
				{
					x = 0.7853982f;
					num6 = math.sqrt(num8 * num7);
				}
				num12 = math.cos(x) * num6;
				num13 = math.sin(x) * num6;
			}
			else
			{
				num9 *= -1f;
				float num14 = num6 * num6;
				float num15 = math.atan((num14 - math.sqrt(num14 * num14 - 2f * num14 * -num9 * num7 - num7 * num7 * num8 * num8)) / (num7 * num8));
				float x;
				if (num15 > -1.5707964f && num15 < 1.5707964f)
				{
					x = num15;
				}
				else
				{
					x = 0.7853982f;
					num6 = math.sqrt(num8 * num7);
				}
				num12 = math.cos(x) * num6;
				float num16 = num8 / num12;
				num13 = -(math.sin(x) * num6 - num7 * num16);
				x = Mathf.Atan(num13 / num12);
			}
			arrow.startVelocity = new float2(num12, num13);
			float num17 = num8 / num12 + 0.2f;
			arrow.duration = num17;
			float num18 = num13 * 0.1f;
			float y = num9 + num13 * num13 * 0.5f * 0.1f;
			float2 float3 = math.lerp(new float2(float2.x, float2.z), new float2(@float.x, @float.z), num18 / num17);
			arrow.midPoint = new float3(float3.x, y, float3.y);
			arrow.SetFlags(Troops.Arrow.Flags.Moving);
			arrow.t = Troops.pdata->rand.NextFloat(-0.5f, 0f);
			arrow.scale = 0f;
			arrow = ++arrow;
		}
		ptr->SetFlags(Troops.SalvoData.Flags.Moving);
		ptr->num_arrows = num4;
		return result;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0007C838 File Offset: 0x0007AA38
	private static bool CalcRot(float2 from, float2 to, float d2min, out float rot)
	{
		float2 @float = to - from;
		if (math.lengthsq(@float) <= d2min)
		{
			rot = 0f;
			return false;
		}
		rot = 90f - math.degrees(math.atan2(@float.y, @float.x));
		return true;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0007C880 File Offset: 0x0007AA80
	public unsafe static int AddFortification(global::Fortification fortificationObj)
	{
		if (!Troops.Initted)
		{
			Debug.LogError("Troops.AddFortification() while Troops is not initialized");
			return -1;
		}
		Troops.CompleteAllJobs();
		int num = -1;
		for (int i = 0; i < Troops.mem_data.fortifications.Length; i++)
		{
			if (!Troops.mem_data.fortifications[i].HasFlags(Troops.FortificationData.Flags.Active))
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return -1;
		}
		Troops.pdata->CopyCounts(ref Troops.mem_data);
		Vector3 attack_position_a = fortificationObj.transform.TransformPoint(fortificationObj.attack_position_a);
		Vector3 attack_position_b = fortificationObj.transform.TransformPoint(fortificationObj.attack_position_b);
		int index = num;
		Troops.TroopsPtrData* ptr = Troops.pdata;
		int id = num;
		float health = fortificationObj.logic.health;
		bool flag;
		if (fortificationObj == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Fortification logic = fortificationObj.logic;
			flag = (((logic != null) ? logic.gate : null) != null);
		}
		Troops.mem_data.fortifications[index] = new Troops.FortificationData(ptr, id, health, (!flag) ? 0f : fortificationObj.logic.gate.health, fortificationObj.logic.position, fortificationObj.logic.def.type, attack_position_a, attack_position_b);
		Troops.FortificationData* fortification = Troops.pdata->GetFortification(num);
		fortification->ClrFlags((Troops.FortificationData.Flags)(-1));
		fortification->SetFlags(Troops.FortificationData.Flags.Started);
		fortificationObj.SetID(fortification->id);
		TroopCollisions.Concurrent concurrent = Troops.fort_collisions.concurrent;
		float2 @float = new float2(fortificationObj.bounds.center.x, fortificationObj.bounds.center.z);
		for (float num2 = @float.x - fortificationObj.bounds.size.x; num2 <= @float.x + fortificationObj.bounds.size.x; num2 += 1f)
		{
			for (float num3 = @float.y - fortificationObj.bounds.size.z; num3 <= @float.y + fortificationObj.bounds.size.z; num3 += 1f)
			{
				int2 tile = TroopCollisions.WorldToGrid(new float2(num2, num3));
				concurrent.Add(tile, num);
			}
		}
		return num;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0007CA90 File Offset: 0x0007AC90
	public static void DelFortification(Troops.ITroopObject fortificationObj)
	{
		if (!Troops.mem_data.fortifications.IsCreated)
		{
			return;
		}
		int id = fortificationObj.GetID();
		if (id <= 0)
		{
			return;
		}
		Troops.mem_data.fortifications[id].ClrFlags((Troops.FortificationData.Flags)(-1));
		fortificationObj.SetID(-1);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0007CADC File Offset: 0x0007ACDC
	public unsafe static void OnRestart()
	{
		TextureBaker textureBaker = Troops.texture_baker;
		if (textureBaker != null)
		{
			TextureBaker.InstancedDecalDrawerBatched decal_drawer = textureBaker.decal_drawer;
			if (decal_drawer != null)
			{
				decal_drawer.Empty();
			}
		}
		for (int i = Troops.ragdolls.Count - 1; i >= 0; i--)
		{
			Troops.RagdollInfo ragdollInfo = Troops.ragdolls[i];
			if (ragdollInfo.go != null)
			{
				global::Common.DestroyObj(ragdollInfo.go);
			}
		}
		Troops.ragdolls.Clear();
		for (int j = Troops.old_ragdolls.Count - 1; j >= 0; j--)
		{
			Troops.RagdollInfo ragdollInfo2 = Troops.old_ragdolls[j];
			if (ragdollInfo2.go != null)
			{
				global::Common.DestroyObj(ragdollInfo2.go);
			}
		}
		Troops.old_ragdolls.Clear();
		for (int k = 0; k < Troops.salvos.Count; k++)
		{
			Troops.salvos[k].data->SetFlags(Troops.SalvoData.Flags.Destroyed);
		}
	}

	// Token: 0x04000840 RID: 2112
	private static List<Troops.TroopProj>[] temp_troop_proj = null;

	// Token: 0x04000841 RID: 2113
	public static bool DrawDustParticles = true;

	// Token: 0x04000842 RID: 2114
	public static GameObject death_decal_prefab;

	// Token: 0x04000843 RID: 2115
	public static Dictionary<string, Troops.SquadModelData> sharedSkinningData = null;

	// Token: 0x04000844 RID: 2116
	public static global::Squad[] squads = null;

	// Token: 0x04000845 RID: 2117
	public static global::Fortification[] fortifications = null;

	// Token: 0x04000846 RID: 2118
	public static List<Troops.Salvo> salvos = null;

	// Token: 0x04000847 RID: 2119
	public static Troops.TroopsMemData mem_data;

	// Token: 0x04000848 RID: 2120
	public unsafe static Troops.TroopsPtrData* pdata = null;

	// Token: 0x04000849 RID: 2121
	public static TroopCollisions collisions;

	// Token: 0x0400084A RID: 2122
	public static TroopCollisions fort_collisions;

	// Token: 0x0400084B RID: 2123
	public static bool path_data_assigned = false;

	// Token: 0x0400084C RID: 2124
	public static PathData.DataPointers path_data;

	// Token: 0x0400084D RID: 2125
	public static List<Troops.RagdollInfo> ragdolls = null;

	// Token: 0x0400084E RID: 2126
	public static List<Troops.RagdollInfo> old_ragdolls = null;

	// Token: 0x0400084F RID: 2127
	private static Quaternion main_cam_rot;

	// Token: 0x04000850 RID: 2128
	public static List<Logic.Unit.Def> valid_defs = null;

	// Token: 0x04000851 RID: 2129
	public static List<Troops.SquadDrawCallInfo> draw_calls = new List<Troops.SquadDrawCallInfo>();

	// Token: 0x04000852 RID: 2130
	private static int _frustrum_last_frame;

	// Token: 0x04000853 RID: 2131
	public static NativeArray<float4> f_planes;

	// Token: 0x04000854 RID: 2132
	public static TextureBaker texture_baker;

	// Token: 0x04000855 RID: 2133
	private static JobHandle cur_job;

	// Token: 0x04000857 RID: 2135
	public const int num_troops = 10000;

	// Token: 0x04000858 RID: 2136
	public const int num_squads = 100;

	// Token: 0x04000859 RID: 2137
	public static readonly Troops.SquadsFilter AllSquads = new Troops.SquadsFilter
	{
		any_of = Troops.SquadData.Flags.Active,
		none_of = Troops.SquadData.Flags.Destroyed
	};

	// Token: 0x0400085A RID: 2138
	public static readonly Troops.SquadsFilter ActiveSquads = new Troops.SquadsFilter
	{
		any_of = Troops.SquadData.Flags.Active,
		none_of = (Troops.SquadData.Flags.Fled | Troops.SquadData.Flags.Destroyed)
	};

	// Token: 0x0400085B RID: 2139
	public static readonly Troops.SquadsFilter MovingSquads = new Troops.SquadsFilter
	{
		any_of = Troops.SquadData.Flags.Moving,
		none_of = Troops.SquadData.Flags.Destroyed
	};

	// Token: 0x0400085C RID: 2140
	public static readonly Troops.SquadsFilter VisibleSquads = new Troops.SquadsFilter
	{
		any_of = Troops.SquadData.Flags.Visible,
		none_of = Troops.SquadData.Flags.Destroyed
	};

	// Token: 0x0400085D RID: 2141
	public static readonly Troops.SquadsFilter DamagedSquads = new Troops.SquadsFilter
	{
		any_of = Troops.SquadData.Flags.HasKilledTroops,
		none_of = Troops.SquadData.Flags.Destroyed
	};

	// Token: 0x0400085E RID: 2142
	public static Troops.TroopsFilter AllTroops = new Troops.TroopsFilter
	{
		none_of = Troops.Troop.Flags.Destroyed
	};

	// Token: 0x0400085F RID: 2143
	public static Troops.TroopsFilter ClimbingTroops = new Troops.TroopsFilter
	{
		any_of = Troops.Troop.Flags.ClimbingLadder,
		none_of = (Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)
	};

	// Token: 0x04000860 RID: 2144
	public static Troops.TroopsFilter ChargingTroops = new Troops.TroopsFilter
	{
		any_of = Troops.Troop.Flags.Charging,
		none_of = (Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)
	};

	// Token: 0x04000861 RID: 2145
	public static Troops.TroopsFilter AliveTroops = new Troops.TroopsFilter
	{
		none_of = (Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)
	};

	// Token: 0x04000862 RID: 2146
	public static Troops.TroopsFilter MovableTroops = new Troops.TroopsFilter
	{
		none_of = (Troops.Troop.Flags.Attacking | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)
	};

	// Token: 0x04000863 RID: 2147
	public static Troops.TroopsFilter PushedOffTroops = new Troops.TroopsFilter
	{
		any_of = Troops.Troop.Flags.PushedOff,
		none_of = (Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)
	};

	// Token: 0x04000864 RID: 2148
	public static Troops.TroopsFilter AliveNotPushedOffNotClimbingTroops = new Troops.TroopsFilter
	{
		none_of = (Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed | Troops.Troop.Flags.PushedOff)
	};

	// Token: 0x04000865 RID: 2149
	public static Troops.SalvosFilter AllSalvos = new Troops.SalvosFilter
	{
		none_of = Troops.SalvoData.Flags.Destroyed
	};

	// Token: 0x04000866 RID: 2150
	public static Troops.SalvosFilter MovingSalvos = new Troops.SalvosFilter
	{
		any_of = Troops.SalvoData.Flags.Moving,
		none_of = Troops.SalvoData.Flags.Destroyed
	};

	// Token: 0x04000867 RID: 2151
	public static Troops.SalvosFilter StartedSalvos = new Troops.SalvosFilter
	{
		any_of = (Troops.SalvoData.Flags.Moving | Troops.SalvoData.Flags.Landed),
		none_of = Troops.SalvoData.Flags.Destroyed
	};

	// Token: 0x04000868 RID: 2152
	public static Troops.ArrowsFilter AllArrows = new Troops.ArrowsFilter
	{
		none_of = Troops.Arrow.Flags.Destroyed
	};

	// Token: 0x04000869 RID: 2153
	public static Troops.ArrowsFilter MovingArrows = new Troops.ArrowsFilter
	{
		any_of = (Troops.Arrow.Flags.Moving | Troops.Arrow.Flags.BeingShot),
		none_of = (Troops.Arrow.Flags.Landed | Troops.Arrow.Flags.Destroyed)
	};

	// Token: 0x0400086A RID: 2154
	public static Troops.ArrowsFilter StartedArrows = new Troops.ArrowsFilter
	{
		any_of = (Troops.Arrow.Flags.Moving | Troops.Arrow.Flags.Landed),
		none_of = Troops.Arrow.Flags.Destroyed
	};

	// Token: 0x020005C4 RID: 1476
	private struct TroopProj
	{
		// Token: 0x06004506 RID: 17670 RVA: 0x00204AB4 File Offset: 0x00202CB4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.troop.id,
				": proj: ",
				this.proj,
				", dist: ",
				this.dist
			});
		}

		// Token: 0x040031A6 RID: 12710
		public Troops.Troop troop;

		// Token: 0x040031A7 RID: 12711
		public float proj;

		// Token: 0x040031A8 RID: 12712
		public float dist;
	}

	// Token: 0x020005C5 RID: 1477
	public struct ClimbLadderProgressJob : Troops.ITroopJob
	{
		// Token: 0x06004507 RID: 17671 RVA: 0x00204B0C File Offset: 0x00202D0C
		public unsafe void Execute(Troops.Troop troop)
		{
			troop.pos2d = math.lerp(troop.squad->climb_start_pt, troop.squad->climb_dest_pt, troop.climb_progress).xz;
			if (troop.climb_progress > 1f)
			{
				troop.pa_id = troop.squad->climb_dest_paid;
				troop.ClrFlags(Troops.Troop.Flags.ClimbingLadder);
				troop.SetFlags(Troops.Troop.Flags.ClimbingLadderFinished);
				troop.squad->SetFlags(Troops.SquadData.Flags.HasTroopClimbedLadder);
				return;
			}
			troop.pa_id = troop.squad->climb_ladder_paid;
			float num = math.length(troop.squad->climb_start_pt - troop.squad->climb_dest_pt);
			troop.climb_progress += troop.data->dt / num;
		}
	}

	// Token: 0x020005C6 RID: 1478
	public struct AdjustTroopTargetsJob : Troops.ITroopJob
	{
		// Token: 0x06004508 RID: 17672 RVA: 0x00204BE8 File Offset: 0x00202DE8
		public unsafe void Execute(Troops.Troop troop)
		{
			PPos pos = troop.pos;
			PPos tgt_pos = troop.tgt_pos;
			if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished))
			{
				return;
			}
			PPos ppos;
			if (this.pointers.Trace(pos, tgt_pos, out ppos, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.pointers.IsNeighbour(tgt_pos.paID, ppos.paID))
			{
				troop.tgt_pa_id = ppos.paID;
				troop.ClrFlags(Troops.Troop.Flags.Stuck);
				troop.stuck_tgt_pos = PPos.Zero;
				return;
			}
			if (pos.paID == 0 && tgt_pos.paID > 0 && this.pointers.PointInArea(pos, tgt_pos.paID, troop.squad->allowed_areas))
			{
				troop.pa_id = tgt_pos.paID;
			}
			else if (pos.paID > 0 && tgt_pos.paID == 0 && !this.pointers.PointInArea(pos, pos.paID, troop.squad->allowed_areas) && this.pointers.GetPA(pos.paID - 1).IsGround())
			{
				troop.pa_id = 0;
			}
			troop.SetFlags(Troops.Troop.Flags.Stuck);
			PPos ppos2;
			if (troop.squad->move_history.Trace(pos, troop.stuck_tgt_pos, out ppos2, this.pointers))
			{
				troop.stuck_tgt_pos = ppos2;
			}
			troop.tgt_pos = ppos2;
		}

		// Token: 0x06004509 RID: 17673 RVA: 0x00204D64 File Offset: 0x00202F64
		private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
		{
			float num = Vector3.Dot(Vector3.Cross(fwd, targetDir), up);
			if (num > 0f)
			{
				return 1f;
			}
			if (num < 0f)
			{
				return -1f;
			}
			return 0f;
		}

		// Token: 0x0600450A RID: 17674 RVA: 0x00204DA0 File Offset: 0x00202FA0
		private unsafe bool IgnoreReachability(Troops.Troop troop, PPos resultPos)
		{
			bool flag = troop.pos.Dist(resultPos) < troop.def->radius + 0.05f;
			return troop.HasFlags(Troops.Troop.Flags.Charging) && !troop.HasFlags(Troops.Troop.Flags.Stuck) && troop.squad->is_attacking && !flag;
		}

		// Token: 0x040031A9 RID: 12713
		public PathData.DataPointers pointers;

		// Token: 0x040031AA RID: 12714
		private const float corner_radius = 0.5f;
	}

	// Token: 0x020005C7 RID: 1479
	public struct FindTroopEnemiesJob : Troops.ITroopJob
	{
		// Token: 0x0600450B RID: 17675 RVA: 0x00204E00 File Offset: 0x00203000
		public unsafe void Execute(Troops.Troop troop)
		{
			if ((troop.squad->HasFlags(Troops.SquadData.Flags.Fled | Troops.SquadData.Flags.Retreating) && !troop.HasFlags(Troops.Troop.Flags.Attacking)) || !troop.def->can_attack_melee)
			{
				this.SetEnemy(troop, -1, 0f);
				troop.fight_status = 0;
				return;
			}
			if (troop.enemy_id >= 0)
			{
				Troops.Troop enemy = troop.enemy;
				if (enemy.squad->HasFlags(Troops.SquadData.Flags.Destroyed) || enemy.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
				{
					troop.enemy_id = -1;
					troop.fight_status = 0;
					troop.find_enemy_cd = 0f;
				}
			}
			Troops.SquadData* squad = troop.squad;
			bool has_accessible_enemies = squad->has_accessible_enemies;
			float num = (squad->controlZone.x > squad->controlZone.y) ? squad->controlZone.x : squad->controlZone.y;
			if (troop.HasFlags(Troops.Troop.Flags.Attacking))
			{
				this.SetEnemy(troop, troop.enemy_id, 0f);
				if (troop.enemy_id < 0)
				{
					troop.tgt_pos = troop.pos;
				}
				return;
			}
			if (troop.enemy_id >= 0 && !this.IsFromTargetSquad(troop, troop.enemy) && (math.lengthsq(troop.pos_in_formation - troop.enemy.pos2d) > num * num * 0.25f || troop.squad->command != Logic.Squad.Command.Fight))
			{
				PPos pt = troop.enemy.pos - troop.pos;
				PPos normalized = (troop.pos_in_formation - troop.pos).GetNormalized();
				pt = pt.GetNormalized();
				float num2 = normalized.Dot(pt);
				if (troop.HasFlags(Troops.Troop.Flags.FarFromSquad) || num2 <= 0f)
				{
					this.SetEnemy(troop, -1, -1f);
					troop.fight_status = 0;
					return;
				}
			}
			else if (troop.enemy_id < 0 && !troop.face_enemies && troop.squad->command == Logic.Squad.Command.Disengage)
			{
				this.SetEnemy(troop, -1, -1f);
				troop.fight_status = 0;
				return;
			}
			Troops.DefData* def = squad->def;
			float2 pos2d = troop.pos2d;
			if (troop.enemy_id >= 0 && troop.fight_status > 0)
			{
				Troops.Troop enemy2 = new Troops.Troop(troop.data, troop.cur_thread_id, troop.enemy_id);
				if (this.IsValidForFurtherProcessing(enemy2))
				{
					float num3 = def->attack_range + def->radius + enemy2.def->radius;
					float num4 = num3 * num3;
					float num5 = this.EvalEnemy(troop, enemy2, num4, num3);
					if (num5 >= 0f && num5 < num4)
					{
						this.SetEnemy(troop, troop.enemy_id, 0f);
						return;
					}
				}
				else
				{
					troop.find_enemy_cd = 0f;
					if (!has_accessible_enemies)
					{
						this.SetEnemy(troop, -1, -1f);
						troop.fight_status = 0;
					}
				}
			}
			bool flag = troop.HasFlags(Troops.Troop.Flags.Charging) || troop.charged_lastly_cd < 0.5f || (troop.def->is_cavalry && troop.charged_lastly_cd < 1f) || troop.squad->command == Logic.Squad.Command.Charge || troop.squad->command == Logic.Squad.Command.Attack;
			if (troop.find_enemy_cd <= 0f)
			{
				float straightnessWeight = 0f;
				float distanceWeight = 1f;
				if ((troop.squad->command == Logic.Squad.Command.Charge || troop.squad->command == Logic.Squad.Command.Attack) && troop.HasFlags(Troops.Troop.Flags.Charging))
				{
					if (troop.def->is_cavalry || troop.squad->command == Logic.Squad.Command.Charge)
					{
						distanceWeight = 0.2f;
						straightnessWeight = 1f;
					}
					else
					{
						distanceWeight = 0.25f;
						straightnessWeight = 1f;
					}
				}
				float num6 = troop.data->rand.NextFloat(0.5f, 1f);
				float num7 = troop.data->rand.NextFloat(2f, 4f);
				float num8 = troop.data->rand.NextFloat(0.1f, 0.2f);
				int num9 = (squad->is_Fighting || flag) ? 1 : 3;
				TempList<Troops.FindTroopEnemiesJob.possibleEnemy> tempList = this.FindSuitableEnemies(troop, pos2d, num, squad, straightnessWeight, distanceWeight, num9);
				int num10 = 0;
				while (num10 < num9 && tempList[num10].id >= 0)
				{
					Troops.Troop enemy3 = new Troops.Troop(troop.data, troop.cur_thread_id, tempList[num10].id);
					float num11 = def->attack_range + def->radius + enemy3.def->radius;
					float num12 = this.CalcAggroRange(troop, ref *squad, ref *def, enemy3, num11);
					if (this.IsValidEnemyForAttack(troop, enemy3, num11 * num11, tempList[num10].d2, num12 * num12))
					{
						float num13 = flag ? 1.5f : 2f;
						float2 @float = math.normalize(enemy3.pos2d - pos2d);
						if (troop.def->radius <= 1f)
						{
							@float *= troop.def->radius * num13;
						}
						else
						{
							@float *= num13;
						}
						float find_enemy_cd = flag ? num8 : num6;
						troop.find_enemy_cd = find_enemy_cd;
						if (!squad->is_Fighting || flag || this.collisions.CheckFuturePosition(troop, pos2d + @float, true))
						{
							this.SetEnemy(troop, tempList[num10].id, tempList[num10].d2);
							tempList.Dispose();
							return;
						}
					}
					num10++;
				}
				if (tempList[0].id >= 0 && squad->is_Fighting)
				{
					if (!flag)
					{
						this.SetEnemy(troop, tempList[0].id, -1f);
						troop.fight_status = 2;
						troop.tgt_pos = troop.pos;
						tempList.Dispose();
						return;
					}
					if (squad->command == Logic.Squad.Command.Hold)
					{
						Troops.Troop enemy4 = new Troops.Troop(troop.data, troop.cur_thread_id, tempList[0].id);
						float ar = def->attack_range + def->radius + enemy4.def->radius;
						this.ShiftTowardsEnemy(troop, enemy4, ar);
					}
				}
				float cooldown = num7;
				if (squad->command == Logic.Squad.Command.Charge || squad->command == Logic.Squad.Command.Attack)
				{
					cooldown = num6;
				}
				this.SetNullEnemy(troop, cooldown, squad, flag);
				troop.enemy_id = tempList[0].id;
				tempList.Dispose();
				return;
			}
			if (troop.enemy_id < 0)
			{
				troop.fight_status = 0;
			}
			troop.find_enemy_cd -= troop.data->dt;
			float num14 = flag ? 1.5f : 2f;
			if (troop.fight_status == 1)
			{
				float2 float2 = math.normalize(troop.enemy.pos2d - pos2d);
				if (troop.def->radius <= 1f)
				{
					float2 *= troop.def->radius * num14;
				}
				else
				{
					float2 *= num14;
				}
				if (this.collisions.CheckFuturePosition(troop, pos2d + float2, true))
				{
					this.SetEnemy(troop, troop.enemy_id, troop.pos.SqrDist(troop.enemy.pos));
					troop.flanking_left = false;
					troop.flanking_right = false;
					return;
				}
				troop.tgt_pos = troop.pos;
				troop.fight_status = 2;
			}
			if (troop.fight_status == 2)
			{
				if (math.lengthsq(troop.tgt_pos - pos2d) != 0f)
				{
					float2 float3 = math.normalize(troop.tgt_pos - pos2d);
					bool flag2 = false;
					float num15 = 1f;
					if (troop.enemy_id >= 0)
					{
						float num16 = def->attack_range + def->radius + troop.enemy.def->radius;
						float2 x = troop.enemy.pos2d - troop.pos2d;
						flag2 = (math.length(x) < num16 * 1.5f);
						if (!flag2)
						{
							num15 = math.dot(float3, math.normalize(x));
						}
					}
					if (!flag2)
					{
						if (num15 > 0f)
						{
							if (troop.def->radius <= 1f)
							{
								float3 *= troop.def->radius * num14;
							}
							else
							{
								float3 *= num14;
							}
							if (math.lengthsq(float3) != 0f && this.collisions.CheckFuturePosition(troop, pos2d + float3, true))
							{
								return;
							}
						}
						else
						{
							troop.fight_status = 3;
						}
					}
				}
				troop.fight_status = 4;
			}
			if (troop.fight_status == 3)
			{
				if (math.lengthsq(troop.tgt_pos - pos2d) != 0f)
				{
					float2 lhs = math.normalize(troop.tgt_pos - pos2d);
					if (troop.enemy_id >= 0)
					{
						float2 x2 = troop.enemy.pos2d - troop.pos2d;
						if (math.lengthsq(x2) != 0f)
						{
							float2 float4 = troop.pos2d + math.normalize(lhs + math.normalize(x2)) * math.length(x2);
							PPos tgt_pos;
							if (this.pointers.Trace(troop.pos, float4, out tgt_pos, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.collisions.CheckFuturePosition(troop, troop.pos2d + math.normalize(float4 - troop.pos2d) * num14 * troop.def->radius, true))
							{
								troop.tgt_pos = tgt_pos;
								return;
							}
						}
					}
				}
				troop.fight_status = 4;
			}
			float num17 = (troop.squad->stance == Logic.Squad.Stance.Aggressive) ? troop.squad->aggressive_base_flank_angle : troop.squad->defensive_base_flank_angle;
			if (troop.fight_status == 4)
			{
				if (num17 <= 0f)
				{
					troop.fight_status = 8;
				}
				else
				{
					float2 rhs = (troop.enemy.pos2d - troop.pos2d).GetRotated(num17);
					float2 float5 = troop.pos2d + rhs;
					PPos tgt_pos2;
					if (this.pointers.Trace(troop.pos, float5, out tgt_pos2, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.collisions.CheckFuturePosition(troop, troop.pos2d + math.normalize(float5 - troop.pos2d) * num14 * troop.def->radius, true))
					{
						troop.tgt_pos = tgt_pos2;
						troop.flanking_left = false;
						troop.flanking_right = false;
						return;
					}
					troop.fight_status = 5;
				}
			}
			if (troop.fight_status == 5)
			{
				float2 rhs2 = (troop.enemy.pos2d - troop.pos2d).GetRotated(-num17);
				float2 float6 = troop.pos2d + rhs2;
				PPos tgt_pos3;
				if (this.pointers.Trace(troop.pos, float6, out tgt_pos3, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.collisions.CheckFuturePosition(troop, troop.pos2d + math.normalize(float6 - troop.pos2d) * num14 * troop.def->radius, true))
				{
					troop.tgt_pos = tgt_pos3;
					troop.flanking_left = false;
					troop.flanking_right = false;
					return;
				}
				troop.fight_status = 6;
				troop.tgt_pos = troop.pos;
				return;
			}
			else
			{
				float num18 = (troop.squad->stance == Logic.Squad.Stance.Aggressive) ? troop.squad->aggressive_additional_flank_angle_cavalry : troop.squad->defensive_additional_flank_angle_cavalry;
				float num19 = (troop.squad->stance == Logic.Squad.Stance.Aggressive) ? troop.squad->aggressive_additional_flank_angle_infantry : troop.squad->defensive_additional_flank_angle_infantry;
				if (troop.fight_status == 6)
				{
					if ((!troop.def->is_cavalry && (num19 <= 0f || num19 < num17)) || (troop.def->is_cavalry && (num18 <= 0f || num18 < num17)))
					{
						troop.fight_status = 8;
					}
					else
					{
						PPos ppos = troop.enemy.pos2d - troop.pos2d;
						float2 rhs3 = troop.def->is_cavalry ? ppos.GetRotated(num18) : ppos.GetRotated(num19);
						float2 float7 = troop.pos2d + rhs3;
						PPos tgt_pos4;
						if (!troop.flanking_right && this.pointers.Trace(troop.pos, float7, out tgt_pos4, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.collisions.CheckFuturePosition(troop, troop.pos2d + math.normalize(float7 - troop.pos2d) * num14 * troop.def->radius, true))
						{
							troop.tgt_pos = tgt_pos4;
							troop.flanking_left = true;
							return;
						}
						if (troop.flanking_left)
						{
							troop.fight_status = 8;
						}
						else
						{
							troop.fight_status = 7;
						}
					}
				}
				if (troop.fight_status != 7)
				{
					if (troop.fight_status == 8)
					{
						troop.tgt_pos = troop.pos;
					}
					return;
				}
				PPos ppos2 = troop.enemy.pos2d - troop.pos2d;
				float2 rhs4 = troop.def->is_cavalry ? ppos2.GetRotated(-num18) : ppos2.GetRotated(-num19);
				float2 float8 = troop.pos2d + rhs4;
				PPos tgt_pos5;
				if (!troop.flanking_left && this.pointers.Trace(troop.pos, float8, out tgt_pos5, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall) && this.collisions.CheckFuturePosition(troop, troop.pos2d + math.normalize(float8 - troop.pos2d) * num14 * troop.def->radius, true))
				{
					troop.tgt_pos = tgt_pos5;
					troop.flanking_right = true;
					return;
				}
				troop.fight_status = 8;
				troop.tgt_pos = troop.pos;
				return;
			}
		}

		// Token: 0x0600450C RID: 17676 RVA: 0x00205D90 File Offset: 0x00203F90
		private unsafe TempList<Troops.FindTroopEnemiesJob.possibleEnemy> FindSuitableEnemies(Troops.Troop troop, float2 pos, float r, Troops.SquadData* squad, float straightnessWeight = 1f, float distanceWeight = 1f, int count = 1)
		{
			float num = r * r;
			float num2 = num + float.Epsilon;
			float value = num2 + 2f;
			TempList<Troops.FindTroopEnemiesJob.possibleEnemy> result = new TempList<Troops.FindTroopEnemiesJob.possibleEnemy>(true, count);
			for (int i = 0; i < count; i++)
			{
				result[i] = new Troops.FindTroopEnemiesJob.possibleEnemy
				{
					id = -1,
					value = value,
					d2 = num2
				};
			}
			if (count <= 0)
			{
				return result;
			}
			if (distanceWeight < 0f)
			{
				distanceWeight = 0f;
			}
			if (straightnessWeight < 0f)
			{
				straightnessWeight = 0f;
			}
			float num3 = -1f;
			float num4 = -1f;
			TempList<int> tempList = new TempList<int>(false, 0);
			Troops.FindTroopEnemiesJob.possibleEnemy possibleEnemy = new Troops.FindTroopEnemiesJob.possibleEnemy
			{
				id = -1,
				value = value,
				d2 = num2
			};
			this.collisions.EnumTroops(pos, r, ref tempList, 1 - squad->battle_side, false);
			for (int j = 0; j < tempList.Length; j++)
			{
				int num5 = tempList[j];
				if (num5 >= 0)
				{
					Troops.Troop enemy = new Troops.Troop(troop.data, troop.cur_thread_id, num5);
					if (this.IsValidForFurtherProcessing(enemy))
					{
						float ar = troop.def->attack_range + troop.def->radius + enemy.def->radius;
						float num6 = this.EvalEnemy(troop, enemy, num, ar);
						float num7 = (straightnessWeight == 0f) ? 0f : this.EvalStraightestEnemy(troop, enemy);
						float num8 = num6 * distanceWeight + (2f - (num7 + 1f)) * straightnessWeight;
						if (this.IsFromTargetSquad(troop, enemy))
						{
							if (num7 > num4)
							{
								num4 = num7;
							}
							for (int k = 0; k < count; k++)
							{
								if (num6 >= 0f && result[k].value > num8)
								{
									for (int l = count - 1; l > k; l--)
									{
										result[l] = result[l - 1];
									}
									result[k] = new Troops.FindTroopEnemiesJob.possibleEnemy
									{
										id = num5,
										value = num8,
										d2 = num6
									};
									if (k == 0)
									{
										num2 = num6;
										num3 = num7;
									}
								}
							}
						}
						else if (num6 >= 0f && possibleEnemy.value > num8 && math.lengthsq(troop.pos_in_formation - enemy.pos2d) <= num * 0.25f)
						{
							possibleEnemy = new Troops.FindTroopEnemiesJob.possibleEnemy
							{
								id = num5,
								value = num8,
								d2 = num6
							};
						}
					}
				}
			}
			if (troop.squad->command == Logic.Squad.Command.Charge && num4 - num3 > 0.05f && num2 > troop.def->attack_range + troop.def->radius + 0.5f)
			{
				tempList.Dispose();
				result[0] = new Troops.FindTroopEnemiesJob.possibleEnemy
				{
					id = -1,
					value = value,
					d2 = num2
				};
				return result;
			}
			if (possibleEnemy.d2 < troop.def->attack_range + troop.def->radius + 0.5f)
			{
				for (int m = 0; m < count; m++)
				{
					if (result[m].id < 0)
					{
						result[m] = possibleEnemy;
					}
					else if (m == count - 1 && possibleEnemy.value < result[m].value)
					{
						result[m] = possibleEnemy;
					}
				}
			}
			tempList.Dispose();
			return result;
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x00206130 File Offset: 0x00204330
		private unsafe bool IsValidForFurtherProcessing(Troops.Troop enemy)
		{
			return enemy.valid && !enemy.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && !enemy.squad->HasFlags(Troops.SquadData.Flags.Fled);
		}

		// Token: 0x0600450E RID: 17678 RVA: 0x00206160 File Offset: 0x00204360
		private unsafe float EvalEnemy(Troops.Troop troop, Troops.Troop enemy, float r2, float ar)
		{
			PPos pos = troop.pos;
			PPos pos2 = enemy.pos;
			float num = ar + enemy.def->attack_range;
			float num2 = num * num;
			float num3 = pos.SqrDist(pos2);
			if (num3 > r2)
			{
				return -1f;
			}
			if (num2 < num3 && !this.IsInSquadRectangle(troop, enemy) && (!enemy.HasFlags(Troops.Troop.Flags.Attacking) || enemy.enemy_id != troop.id) && troop.pa_id == 0)
			{
				return -1f;
			}
			return num3;
		}

		// Token: 0x0600450F RID: 17679 RVA: 0x002061DC File Offset: 0x002043DC
		private unsafe float EvalStraightestEnemy(Troops.Troop troop, Troops.Troop enemy)
		{
			float2 rhs = troop.pos;
			float2 lhs = enemy.pos;
			PPos ppos = troop.squad->dir;
			PPos pt = math.normalize(lhs - rhs);
			return ppos.GetNormalized().Dot(pt);
		}

		// Token: 0x06004510 RID: 17680 RVA: 0x00206238 File Offset: 0x00204438
		private unsafe float CalcAggroRange(Troops.Troop troop, ref Troops.SquadData squad, ref Troops.DefData def, Troops.Troop enemy, float ar)
		{
			float num = enemy.def->attack_range + enemy.def->radius + troop.def->radius;
			float num2 = ar + num;
			float num3 = (ar > num) ? (ar + troop.squad->def->radius + enemy.def->radius) : num;
			num3 += 0.01f;
			num2 += ar;
			Logic.Squad.Command command = squad.command;
			if (enemy.HasFlags(Troops.Troop.Flags.Attacking) && enemy.enemy_id == troop.id)
			{
				return float.MaxValue;
			}
			if (command == Logic.Squad.Command.Disengage)
			{
				return ar;
			}
			if (!squad.is_attacking)
			{
				if (squad.is_Fighting || (squad.stance == Logic.Squad.Stance.Aggressive && (!def.is_ranged || squad.salvos_left <= 0 || squad.hold_fire)))
				{
					return float.MaxValue;
				}
				return num3;
			}
			else if (this.IsFromTargetSquad(troop, enemy))
			{
				if (squad.command == Logic.Squad.Command.Charge)
				{
					return num2 * 2f;
				}
				return float.MaxValue;
			}
			else
			{
				if (squad.was_Fighting_Target)
				{
					return 2f * num2;
				}
				if (squad.is_Fighting)
				{
					return num2;
				}
				return num3;
			}
		}

		// Token: 0x06004511 RID: 17681 RVA: 0x00206350 File Offset: 0x00204550
		private unsafe bool IsValidEnemyForAttack(Troops.Troop troop, Troops.Troop enemy, float ar2, float d2, float aggroRange2)
		{
			PPos ppos;
			return (ar2 >= d2 || troop.squad->is_Fighting || !troop.squad->is_attacking || this.IsFromTargetSquad(troop, enemy)) && this.pointers.Trace(troop.pos, enemy.pos, out ppos, false, true, false, PathData.PassableArea.Type.All, troop.squad->battle_side, troop.squad->is_inside_wall) && d2 <= aggroRange2;
		}

		// Token: 0x06004512 RID: 17682 RVA: 0x002063D0 File Offset: 0x002045D0
		private void ShiftTowardsEnemy(Troops.Troop troop, Troops.Troop enemy, float ar)
		{
			float2 rhs = math.normalize(enemy.pos - troop.pos_in_formation);
			troop.tgt_pos = troop.pos_in_formation + ar * rhs;
		}

		// Token: 0x06004513 RID: 17683 RVA: 0x0020641C File Offset: 0x0020461C
		private unsafe void SetEnemy(Troops.Troop troop, int enemy_id, float d2 = -1f)
		{
			float num = troop.def->attack_range + troop.def->radius;
			troop.enemy_id = enemy_id;
			troop.fight_status = 1;
			if (enemy_id < 0 || d2 < 0f)
			{
				if (troop.enemy_fortification != null)
				{
					PPos pos = troop.pos;
					PPos ppos = new PPos(troop.enemy_fortification->attack_position_a, pos.paID);
					PPos ppos2 = new PPos(troop.enemy_fortification->attack_position_b, pos.paID);
					float num2 = ppos.SqrDist(pos);
					float num3 = ppos2.SqrDist(pos);
					PPos tgt_pos;
					if (num2 < num3)
					{
						tgt_pos = ppos;
						d2 = num2;
					}
					else
					{
						tgt_pos = ppos2;
						d2 = num3;
					}
					if (d2 <= num * num * 2f)
					{
						if ((double)d2 > 0.01)
						{
							troop.tgt_pos = tgt_pos;
						}
						troop.ClrFlags(Troops.Troop.Flags.AttackMove);
						troop.SetFlags(Troops.Troop.Flags.Fighting);
						troop.squad->SetFlags(Troops.SquadData.Flags.Fighting);
						troop.squad->is_Fighting_Target = true;
						return;
					}
				}
				troop.fight_status = 0;
				troop.ClrFlags(Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.Fighting);
				return;
			}
			if (troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger))
			{
				return;
			}
			Troops.Troop enemy = troop.enemy;
			num += enemy.def->radius;
			if (d2 > num * num)
			{
				if (!troop.HasFlags(Troops.Troop.Flags.Stuck))
				{
					if (troop.HasFlags(Troops.Troop.Flags.Charging) && troop.def->is_cavalry)
					{
						PPos normalized = (enemy.pos - troop.pos).GetNormalized();
						troop.tgt_pos = enemy.pos + new PPos(normalized * (num + 4f), 0);
						troop.ClrFlags(Troops.Troop.Flags.Fighting);
						troop.SetFlags(Troops.Troop.Flags.AttackMove);
						return;
					}
					troop.tgt_pos = enemy.pos;
					troop.ClrFlags(Troops.Troop.Flags.Fighting);
					troop.SetFlags(Troops.Troop.Flags.AttackMove);
				}
				return;
			}
			if (troop.HasFlags(Troops.Troop.Flags.Charging) && troop.def->is_cavalry)
			{
				PPos normalized2 = (enemy.pos - troop.pos).GetNormalized();
				troop.tgt_pos = enemy.pos + new PPos(normalized2 * (num + 2f), 0);
				troop.ClrFlags(Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.ShootTrigger);
				troop.SetFlags(Troops.Troop.Flags.Fighting);
				return;
			}
			troop.tgt_pos = troop.pos;
			troop.ClrFlags(Troops.Troop.Flags.AttackMove | Troops.Troop.Flags.ShootTrigger);
			troop.SetFlags(Troops.Troop.Flags.Fighting);
			troop.squad->SetFlags(Troops.SquadData.Flags.Fighting);
			if (this.IsFromTargetSquad(troop, enemy))
			{
				troop.squad->is_Fighting_Target = true;
			}
			enemy.SetFlags(Troops.Troop.Flags.Fighting);
			enemy.squad->SetFlags(Troops.SquadData.Flags.Fighting);
		}

		// Token: 0x06004514 RID: 17684 RVA: 0x002066EC File Offset: 0x002048EC
		private unsafe bool IsInSquadRectangle(Troops.Troop troop, Troops.Troop enemy)
		{
			return troop.squad->IsInSquadRectangle(enemy.pos);
		}

		// Token: 0x06004515 RID: 17685 RVA: 0x00206704 File Offset: 0x00204904
		private unsafe bool IsFromTargetSquad(Troops.Troop troop, Troops.Troop enemy)
		{
			return troop.squad->target_id == enemy.squad_id || (enemy.squad->main_squad_id != -1 && enemy.squad->main_squad_id == troop.squad->target_id);
		}

		// Token: 0x06004516 RID: 17686 RVA: 0x00206754 File Offset: 0x00204954
		private unsafe void SetNullEnemy(Troops.Troop troop, float cooldown, Troops.SquadData* squad, bool isCharging)
		{
			troop.find_enemy_cd = cooldown;
			float2 @float = math.normalize(math.float2(troop.tgt_pos) - troop.pos2d);
			if (troop.def->radius <= 1f)
			{
				@float *= troop.def->radius * 2f;
			}
			else
			{
				@float *= 2f;
			}
			if (math.lengthsq(@float) != 0f && !this.collisions.CheckFuturePosition(troop, troop.pos2d + @float, true) && squad->is_Fighting && !isCharging)
			{
				troop.tgt_pos = troop.pos;
				this.SetEnemy(troop, -2, -1f);
				troop.fight_status = 0;
				return;
			}
			this.SetEnemy(troop, -1, -1f);
		}

		// Token: 0x040031AB RID: 12715
		public TroopCollisions.ReadOnly collisions;

		// Token: 0x040031AC RID: 12716
		public PathData.DataPointers pointers;

		// Token: 0x040031AD RID: 12717
		private const float check_collision_distance_mul = 2f;

		// Token: 0x040031AE RID: 12718
		private const float charge_check_collision_distance_mul = 1.5f;

		// Token: 0x020009DA RID: 2522
		private struct possibleEnemy
		{
			// Token: 0x0400457F RID: 17791
			public int id;

			// Token: 0x04004580 RID: 17792
			public float value;

			// Token: 0x04004581 RID: 17793
			public float d2;
		}
	}

	// Token: 0x020005C8 RID: 1480
	public struct CalcTroopVelocities : Troops.ITroopJob
	{
		// Token: 0x06004517 RID: 17687 RVA: 0x00206830 File Offset: 0x00204A30
		public unsafe void Execute(Troops.Troop troop)
		{
			float2 @float = troop.tgt_pos - troop.pos;
			float radius = troop.def->radius;
			float num = math.length(@float);
			float4 vel_spd_t = troop.vel_spd_t;
			float dt = troop.data->dt;
			int pa_id = troop.pa_id;
			if (num <= 1E-05f || troop.def->walk_anim_speed <= 0f)
			{
				troop.pa_id = troop.tgt_pa_id;
				troop.vel_spd_t = 0;
				troop.ClrFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.AttackMove);
				return;
			}
			if (radius <= 1f)
			{
				float num2 = troop.HasFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.ClimbingLadderFinished | Troops.Troop.Flags.ClimbingLadderWaiting) ? 1f : 4f;
				if (troop.squad->rearrange_troops_cooldown > 0f && troop.squad->rearrange_safe_dist < num2)
				{
					num2 = troop.squad->rearrange_safe_dist;
				}
				if (((num <= num2 && (!troop.squad->is_Fighting || troop.fight_status == 2)) || (num < 0.9f * (troop.def->attack_range + troop.def->radius + 0.5f) && troop.HasFlags(Troops.Troop.Flags.Fighting))) && !troop.HasFlags(Troops.Troop.Flags.Stuck) && troop.fight_status != 8)
				{
					if (troop.squad->HasFlags(Troops.SquadData.Flags.Moving))
					{
						troop.vel_spd_t = new float4(0f, 0f, 0f, vel_spd_t.w - dt / 4f);
					}
					else
					{
						troop.vel_spd_t = 0;
					}
					troop.ClrFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.AttackMove);
					return;
				}
			}
			else if (num <= 0.5f)
			{
				if (troop.squad->HasFlags(Troops.SquadData.Flags.Moving))
				{
					troop.vel_spd_t = new float4(0f, 0f, 0f, vel_spd_t.w - dt / 4f);
				}
				else
				{
					troop.vel_spd_t = 0;
				}
				troop.ClrFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.AttackMove);
				return;
			}
			troop.SetFlags(Troops.Troop.Flags.Moving);
			troop.ActivateSquad();
			Troops.DefData* def = troop.def;
			float num3 = troop.squad->move_speed * def->max_speed_mul;
			if (def->is_siege_eq)
			{
				num3 = troop.squad->move_speed;
			}
			float num4 = math.length(@float);
			PPos to = troop.pos2d + @float;
			PPos ppos;
			if (!troop.HasFlags(Troops.Troop.Flags.Stuck) && !this.pointers.Trace(troop.pos, to, out ppos, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall))
			{
				float num5 = ppos.pos.Dist(troop.pos);
				if (num4 > 0f && num5 > 0f)
				{
					@float = @float / num4 * num5;
					num4 = math.max(num5, num4 * 0.5f);
				}
			}
			float num6 = math.min(troop.squad->move_speed * def->max_speed_mul, num4);
			if (troop.squad->IsRearranging())
			{
				num6 = ((num6 >= 1f) ? num6 : 1f);
			}
			float max = dt * def->max_acceleration;
			float num7 = dt * def->max_deceleration;
			if (pa_id != 0)
			{
				PathData.PassableArea pa = this.pointers.GetPA(pa_id - 1);
				if (pa.type == PathData.PassableArea.Type.Ladder)
				{
					vel_spd_t.z = 0.25f;
					vel_spd_t.w = math.min(num4 / vel_spd_t.z, 1f);
					vel_spd_t.xy = @float / num4;
					if (vel_spd_t.z > num3)
					{
						vel_spd_t.z = num3;
					}
					troop.vel_spd_t = vel_spd_t;
					return;
				}
				float angle = pa.Angle;
				num6 *= math.max(1f - angle / 135f, 0.25f);
			}
			if (troop.HasFlags(Troops.Troop.Flags.Stuck))
			{
				float rot_within_bounds = troop.rot_within_bounds;
				float num8;
				if (Troops.CalcRot(0, @float, 0.1f, out num8))
				{
					if (num8 < 0f)
					{
						num8 += 360f;
					}
					float num9 = math.max(0f, 1f - math.abs(num8 - rot_within_bounds) / 360f);
					if (this.pointers.IsPassable(troop.pos))
					{
						vel_spd_t.z = 2f * num9;
					}
					else
					{
						vel_spd_t.z = num4 * num9;
					}
					if (vel_spd_t.z > 0f)
					{
						vel_spd_t.w = math.min(num4 / vel_spd_t.z, 1f);
					}
					else
					{
						vel_spd_t.w = 0f;
					}
					if (num4 > 0f)
					{
						vel_spd_t.xy = @float / num4;
					}
					if (vel_spd_t.z > num3)
					{
						vel_spd_t.z = num3;
					}
					troop.vel_spd_t = vel_spd_t;
					return;
				}
			}
			float num10 = Mathf.Clamp(num6 - vel_spd_t.z, -num7, max);
			vel_spd_t.z += num10;
			if (vel_spd_t.z < 0f)
			{
				return;
			}
			vel_spd_t.xy = @float / num4;
			if (troop.squad->HasFlags(Troops.SquadData.Flags.Moving) || troop.squad->is_Fighting)
			{
				vel_spd_t.x = Mathf.Lerp(troop.vel_spd_t.x, vel_spd_t.x, dt * 3f);
				vel_spd_t.y = Mathf.Lerp(troop.vel_spd_t.y, vel_spd_t.y, dt * 3f);
				float num11 = math.length(vel_spd_t.xy);
				if (num11 > 0.01f)
				{
					vel_spd_t.z *= num11;
					vel_spd_t.xy = math.normalize(vel_spd_t.xy);
					if (num4 > 1f)
					{
						float2 rhs = new PPos(0f, 1f, 0).GetRotated(troop.rot_y);
						float2 float2 = vel_spd_t.xy + rhs;
						if (float2.x != 0f || float2.y != 0f)
						{
							vel_spd_t.xy = math.normalize(float2);
						}
					}
				}
			}
			if (!troop.HasFlags(Troops.Troop.Flags.Attacking) && troop.squad->command != Logic.Squad.Command.Move && !troop.def->is_siege_eq)
			{
				float2 float3 = vel_spd_t.xy * vel_spd_t.z;
				@float = float3;
				float2 float4 = 0;
				float2 lhs = 0;
				float num12 = 8f;
				float num13 = 0.5f;
				if (troop.pa_id != 0)
				{
					num13 = 0f;
					num12 = 1f;
				}
				else
				{
					if (troop.squad->command == Logic.Squad.Command.Hold || troop.squad->command == Logic.Squad.Command.Attack)
					{
						num12 = 2f;
					}
					if (troop.squad->command == Logic.Squad.Command.Charge)
					{
						num12 = 0f;
					}
					else if (troop.fight_status == 4 || troop.fight_status == 5 || troop.fight_status == 6 || troop.fight_status == 7 || troop.fight_status == 8)
					{
						num12 = 0f;
					}
					else if (troop.fight_status == 1 || troop.fight_status == 2)
					{
						num12 *= 0.5f;
					}
					if (troop.def->is_cavalry)
					{
						num13 = 0f;
					}
				}
				num12 *= 0.25f;
				num13 *= 0.25f;
				if (num12 > 0f && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
				{
					this.collisions.CalculateSeparation(troop);
				}
				if (troop.squad->is_Fighting || troop.squad->command == Logic.Squad.Command.Attack)
				{
					float4 += num12 * troop.separation.xy;
					if (troop.squad->pos.paID == 0 && !troop.enemy.valid && troop.squad->command != Logic.Squad.Command.Fight)
					{
						lhs += num13 * troop.hold_formation_vec;
					}
				}
				else if (troop.HasFlags(Troops.Troop.Flags.AttackMove))
				{
					float4 += num12 * troop.separation.xy;
					if (troop.squad->pos.paID == 0)
					{
						lhs += num13 * troop.hold_formation_vec;
					}
				}
				if (math.lengthsq(float4) > 0f)
				{
					float2 float5 = new PPos(0f, 1f, 0).GetRotated(troop.rot_y + 90f);
					float4 = math.dot(float5, float4) * float5;
				}
				troop.separation = new float4(float4, troop.separation.zw);
				@float += float4;
				if (troop.tgt_pos == troop.pos)
				{
					troop.tgt_pos += new PPos(float4, 0);
				}
				float num14 = math.lengthsq(@float);
				float num15 = math.lengthsq(float3);
				if (num14 != 0f && num15 != 0f)
				{
					math.dot(math.normalize(float3), math.normalize(@float));
					if (math.dot(math.normalize(@float), math.normalize(float3)) < 0f)
					{
						@float = float3;
					}
				}
				if (math.length(@float) > 0.01f)
				{
					vel_spd_t.z = math.length(@float);
					vel_spd_t.xy = math.normalize(@float);
				}
			}
			if (vel_spd_t.z < def->min_speed)
			{
				vel_spd_t.z = def->min_speed;
			}
			if (vel_spd_t.z > 0f)
			{
				vel_spd_t.w = math.min(num4 / vel_spd_t.z, 1f);
			}
			if (vel_spd_t.z > num3)
			{
				vel_spd_t.z = num3;
			}
			troop.vel_spd_t = vel_spd_t;
		}

		// Token: 0x040031AF RID: 12719
		public PathData.DataPointers pointers;

		// Token: 0x040031B0 RID: 12720
		public TroopCollisions.ReadOnly collisions;
	}

	// Token: 0x020005C9 RID: 1481
	public struct TroopsObstacleAvoidanceJob : Troops.ITroopJob
	{
		// Token: 0x06004518 RID: 17688 RVA: 0x0020724C File Offset: 0x0020544C
		public unsafe void Execute(Troops.Troop troop)
		{
			this.collisions.AvoidCollisions(troop);
			float4 steer;
			if (troop.steer.w == 0f && !this.collisions.ValidatePosition(troop, troop.pos.pos + troop.vel_spd_t.xy, out steer))
			{
				troop.steer = steer;
			}
			if (troop.squad->HasFlags(Troops.SquadData.Flags.Charging) && troop.HasFlags(Troops.Troop.Flags.Moving) && troop.vel_spd_t.z > 1f)
			{
				troop.SetFlags(Troops.Troop.Flags.Charging, true);
				troop.charged_lastly_cd = 0f;
				return;
			}
			troop.SetFlags(Troops.Troop.Flags.Charging, false);
			troop.charged_lastly_cd += troop.data->dt;
		}

		// Token: 0x040031B1 RID: 12721
		public TroopCollisions.ReadOnly collisions;
	}

	// Token: 0x020005CA RID: 1482
	public struct PushOffUpdateJob : Troops.ITroopJob
	{
		// Token: 0x06004519 RID: 17689 RVA: 0x00207328 File Offset: 0x00205528
		public unsafe void Execute(Troops.Troop troop)
		{
			if (troop.data->dt == 0f)
			{
				return;
			}
			float dt = troop.data->dt;
			float2 xy = troop.pushOff_args.xy;
			float2 x = xy * this.CalcSpeed(troop.pushOff_time, dt);
			PPos to = troop.pos + new PPos(x.xy, 0) * dt;
			PPos tgt_pos;
			if (!this.pointers.Trace(troop.pos, to, out tgt_pos, false, true, false, troop.squad->allowed_areas, troop.squad->battle_side, troop.squad->is_inside_wall))
			{
				troop.ClrFlags(Troops.Troop.Flags.PushedOff);
				return;
			}
			troop.vel_spd_t = new float4(math.normalize(xy), math.length(x), dt);
			troop.tgt_pos = tgt_pos;
			troop.pushOff_time += dt;
			float num = 2f - troop.pushOff_time;
			if (num <= 0f)
			{
				troop.ClrFlags(Troops.Troop.Flags.PushedOff);
				return;
			}
			TempList<int> tempList = new TempList<int>(false, 0);
			float num2 = troop.def->radius * 2f;
			float num3 = num2 * num2;
			if (this.collisions.EnumTroops(troop.pos, num2, ref tempList, troop.squad->battle_side, true) > 0)
			{
				for (int i = 0; i < tempList.Length; i++)
				{
					Troops.Troop troop2 = troop.data->GetTroop(troop.cur_thread_id, tempList[i]);
					float num4 = troop2.def->defense * 0.01f;
					if (troop.data->rand.NextFloat() >= num4 && troop.id != troop2.id)
					{
						float2 x2 = troop2.pos2d - troop.pos2d;
						if (math.lengthsq(x2) <= num3 && math.dot(math.normalize(x2), math.normalize(xy)) >= 0.5f && !troop2.def->is_cavalry && !troop2.HasFlags(Troops.Troop.Flags.Attacking | Troops.Troop.Flags.Killed | Troops.Troop.Flags.Thrown | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
						{
							troop.pushOff_time += 0.1f;
							troop2.PushOff(math.normalize(x2), num);
						}
					}
				}
			}
		}

		// Token: 0x0600451A RID: 17690 RVA: 0x0020757C File Offset: 0x0020577C
		private float CalcSpeed(float t, float dt)
		{
			if (dt == 0f)
			{
				return 0f;
			}
			return (this.EaseOutQuad((t + dt) / 2f) - this.EaseOutQuad(t / 2f)) / dt;
		}

		// Token: 0x0600451B RID: 17691 RVA: 0x002075AB File Offset: 0x002057AB
		private float EaseOutQuad(float t)
		{
			return t * (2f - t);
		}

		// Token: 0x040031B2 RID: 12722
		public TroopCollisions.ReadOnly collisions;

		// Token: 0x040031B3 RID: 12723
		public PathData.DataPointers pointers;
	}

	// Token: 0x020005CB RID: 1483
	public struct MoveTroopsJob : Troops.ITroopJob
	{
		// Token: 0x0600451C RID: 17692 RVA: 0x002075B8 File Offset: 0x002057B8
		public unsafe void Execute(Troops.Troop troop)
		{
			float2 pos2d = troop.pos2d;
			if (!troop.squad->HasFlags(Troops.SquadData.Flags.Fled) && !troop.def->is_cavalry && troop.HasFlags(Troops.Troop.Flags.Shooting))
			{
				return;
			}
			if (troop.def->walk_anim_speed <= 0f)
			{
				return;
			}
			if (troop.def->is_siege_eq && !troop.HasFlags(Troops.Troop.Flags.Moving))
			{
				return;
			}
			float4 @float = troop.vel_spd_t;
			float dt = troop.data->dt;
			troop.previous_pos3d = troop.pos3d;
			if (dt <= 0f)
			{
				return;
			}
			float4 steer = troop.steer;
			float2 x = default(float2);
			float2 float2;
			if (steer.z < 1f && !troop.HasFlags(Troops.Troop.Flags.Stuck))
			{
				troop.ClrFlags(Troops.Troop.Flags.AttackMove);
				if (steer.z < 0f)
				{
					if (troop.HasFlags(Troops.Troop.Flags.Blocked))
					{
						@float = 0;
						x = 0;
					}
					else
					{
						float2 = steer.xy;
						x = float2;
						float num = dt;
						float num2 = math.lengthsq(float2);
						if (num2 > num * num)
						{
							float2 *= num * math.rsqrt(num2);
						}
						this.Move(troop, float2, false, false);
						if (!troop.def->is_siege_eq)
						{
							@float.xy += float2;
						}
					}
				}
				else if (!troop.def->is_cavalry || !troop.HasFlags(Troops.Troop.Flags.Charging))
				{
					@float.xy = math.lerp(@float.xy, steer.xy, dt);
				}
				if ((troop.enemy_id >= 0 && troop.fight_status != 0) || (troop.squad->command == Logic.Squad.Command.Disengage && !troop.squad->HasFlags(Troops.SquadData.Flags.Fled)))
				{
					float num3 = math.length(@float.xy);
					float max = dt * troop.def->max_acceleration;
					float num4 = dt * troop.def->max_deceleration;
					float num5 = Mathf.Clamp(math.min(troop.squad->move_speed * troop.def->max_speed_mul, num3) - @float.z, -num4, max);
					@float.z += num5;
					if (@float.z > 0f)
					{
						if (@float.z < troop.def->min_speed)
						{
							@float.z = troop.def->min_speed;
						}
						@float.w = math.min(num3 / @float.z, 1f);
						@float.xy /= num3;
						troop.vel_spd_t = @float;
					}
				}
				else
				{
					troop.vel_spd_t = new float4(@float.xy, troop.vel_spd_t.zw);
				}
			}
			float num6 = @float.z;
			if (troop.def->is_siege_eq)
			{
				num6 -= math.length(x);
				if (num6 < 0f)
				{
					num6 = 0f;
				}
			}
			float num7 = math.min(@float.w, dt);
			float2 = @float.xy * (num6 * num7);
			this.Move(troop, float2, true, false);
			if (troop.squad->is_Fighting)
			{
				float2 float3 = troop.pos2d - pos2d;
				num6 = math.length(float3) / troop.data->dt;
				if (num6 != 0f)
				{
					float3 = math.normalize(float3);
				}
				troop.vel_spd_t = new float4(float3, num6, troop.vel_spd_t.w);
			}
		}

		// Token: 0x0600451D RID: 17693 RVA: 0x00207950 File Offset: 0x00205B50
		public unsafe void Move(Troops.Troop troop, Point v, bool check_collisions, bool saveVelocity = false)
		{
			if (v.SqrLength() > 1f)
			{
				v = v.GetNormalized();
			}
			float v_mag = v.Length();
			PPos pos = troop.pos;
			PPos ppos = pos + v;
			PPos ppos2;
			int num;
			if (troop.HasFlags(Troops.Troop.Flags.Stuck))
			{
				if (this.pointers.Trace(pos, ppos, out ppos2, out num, true, true, false, troop.squad->allowed_areas, troop.squad->battle_side, true, troop.squad->is_inside_wall))
				{
					troop.pos = ppos2;
				}
				else if (troop.pa_id > 0 && ppos2.paID > 0 && num > 0 && this.pointers.PointInArea(ppos2, ppos2.paID, troop.squad->allowed_areas))
				{
					troop.pos = this.TrySlideOnEdge(troop, v, v_mag, pos, ppos2, num, troop.squad->is_inside_wall);
				}
				if (pos.paID != 0 && pos.paID == ppos.paID && ppos.paID == ppos2.paID && !this.pointers.PointInArea(pos, ppos2.paID, troop.squad->allowed_areas))
				{
					troop.pos = ppos;
				}
				if (pos.paID == 0 && troop.tgt_pos.paID != 0 && this.pointers.PointInArea(pos, troop.tgt_pos.paID, troop.squad->allowed_areas))
				{
					troop.pa_id = troop.tgt_pos.paID;
				}
				return;
			}
			float4 @float;
			if (check_collisions && !this.collisions.ValidatePosition(troop, ppos, out @float))
			{
				troop.ClrFlags(Troops.Troop.Flags.Moving | Troops.Troop.Flags.AttackMove);
				if (saveVelocity)
				{
					troop.vel_spd_t = new float4(troop.vel_spd_t.xy, 0);
				}
				return;
			}
			if (this.pointers.Trace(pos, ppos, out ppos2, out num, pos.paID != 0, true, false, troop.squad->allowed_areas, troop.squad->battle_side, true, troop.squad->is_inside_wall))
			{
				if (saveVelocity)
				{
					float2 float2 = ppos2 - troop.pos;
					troop.vel_spd_t = new float4(float2, math.length(float2) / troop.data->dt, troop.vel_spd_t.w);
				}
				troop.pos = ppos2;
			}
			else if (troop.pa_id > 0 && ppos2.paID > 0 && num > 0 && this.pointers.PointInArea(ppos2, ppos2.paID, troop.squad->allowed_areas))
			{
				troop.pos = this.TrySlideOnEdge(troop, v, v_mag, pos, ppos2, num, troop.squad->is_inside_wall);
			}
			if (pos.paID == 0 && ppos2.paID != 0 && this.pointers.PointInArea(pos, ppos2.paID, troop.squad->allowed_areas))
			{
				troop.pa_id = ppos2.paID;
			}
		}

		// Token: 0x0600451E RID: 17694 RVA: 0x00207C5C File Offset: 0x00205E5C
		private unsafe PPos TrySlideOnEdge(Troops.Troop troop, Point v, float v_mag, PPos pos, PPos result_pos, int idx_hit, bool was_inside_wall = false)
		{
			float num = 0.02f;
			float num2 = (troop.pos - result_pos).Length();
			float f = v_mag - num2;
			PathData.PassableArea pa = this.pointers.GetPA(result_pos.paID - 1);
			Point xz = pa.GetCornerVertex(idx_hit).xz;
			Point xz2 = pa.GetCornerVertex((idx_hit + 1) % PathData.PassableArea.numNodes).xz;
			Point normalized = (xz - xz2).GetNormalized();
			Point pt = normalized.Right(0f);
			float num3 = v.Dot(normalized);
			float f2 = (v.Dot(pt) > 0f) ? (-num) : num;
			PPos to;
			if (num3 > 0f)
			{
				to = result_pos + normalized * f + pt * f2;
			}
			else
			{
				to = result_pos - normalized * f + pt * f2;
			}
			PPos ppos;
			int num4;
			this.pointers.Trace(pos, to, out ppos, out num4, pos.paID != 0, true, false, troop.squad->allowed_areas, troop.squad->battle_side, false, troop.squad->is_inside_wall);
			if (ppos.paID > 0 && !this.pointers.GetPA(ppos.paID - 1).CanEnter(troop.squad->allowed_areas, troop.squad->battle_side, this.pointers, ppos.paID, 0, was_inside_wall))
			{
				return pos;
			}
			return ppos;
		}

		// Token: 0x040031B4 RID: 12724
		public PathData.DataPointers pointers;

		// Token: 0x040031B5 RID: 12725
		public TroopCollisions.ReadOnly collisions;
	}

	// Token: 0x020005CC RID: 1484
	public struct SnapTroopsJob : Troops.ITroopJob
	{
		// Token: 0x0600451F RID: 17695 RVA: 0x00207DF0 File Offset: 0x00205FF0
		public unsafe void Execute(Troops.Troop troop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Thrown) && troop.HasFlags(Troops.Troop.Flags.Dead))
			{
				return;
			}
			int pa_id = troop.pa_id;
			float3 pos3d = troop.pos3d;
			float num = (this.heights == null) ? 2f : this.GetHeight(ref *this.heights, pos3d.xz);
			float num6;
			if (troop.HasFlags(Troops.Troop.Flags.Thrown))
			{
				float num2 = troop.thrown_time;
				float dt = troop.data->dt;
				float3 thrown_pos = troop.thrown_pos;
				float y = thrown_pos.y;
				float num3 = troop.def->is_cavalry ? 0.1f : 0.35f;
				float num4 = troop.def->is_cavalry ? 1f : 3.5f;
				float num5 = math.clamp((y - num) / 5f, num3, 1f);
				if (num2 <= num3)
				{
					float t = num2 / num3;
					num6 = y + num4 * this.EaseOutQuad(t);
				}
				else
				{
					float num7 = math.max((num2 - num3) / num5, 0f);
					if (num7 >= 1f)
					{
						troop.ClrFlags(Troops.Troop.Flags.Thrown);
					}
					num6 = y + num4 - (y + num4 - num) * this.EaseInQuad(num7);
				}
				float num8 = num5 + num3;
				pos3d.xz = math.lerp(thrown_pos.xz, thrown_pos.xz + troop.thrown_dir, num2 / num8);
				num2 += dt;
				troop.thrown_time = num2;
			}
			else if (pa_id == 0)
			{
				num6 = num;
			}
			else
			{
				PathData.PassableArea pa = this.pointers.GetPA(pa_id - 1);
				if (pa.IsGround())
				{
					num6 = num;
				}
				else
				{
					float num9 = 0f;
					if (pa.type == PathData.PassableArea.Type.Ladder && troop.HasFlags(Troops.Troop.Flags.ClimbingLadder))
					{
						num9 = 0.75f;
						if (troop.squad->climb_dest_pt.y > troop.squad->climb_start_pt.y)
						{
							num6 = math.lerp(num, math.max(pa.GetHeight(pos3d) + num9, num), math.min(1f, troop.climb_progress / 0.25f));
						}
						else
						{
							num6 = math.lerp(num, math.max(pa.GetHeight(pos3d) + num9, num), math.min(1f, (1f - troop.climb_progress) / 0.25f));
						}
					}
					else
					{
						num6 = math.max(pa.GetHeight(pos3d) + num9, num);
					}
				}
			}
			troop.pos3d = new float3(pos3d.x, num6, pos3d.z);
			float num10 = num6 - pos3d.y;
			troop.SetFlags(Troops.Troop.Flags.MovingDown, num10 < 0f || (num10 == 0f && troop.HasFlags(Troops.Troop.Flags.MovingDown)));
		}

		// Token: 0x06004520 RID: 17696 RVA: 0x002080D9 File Offset: 0x002062D9
		private float GetHeight(ref HeightsGrid.Data heights, float2 pos)
		{
			return heights.GetInterpolatedHeight(pos.x, pos.y);
		}

		// Token: 0x06004521 RID: 17697 RVA: 0x002075AB File Offset: 0x002057AB
		private float EaseOutQuad(float t)
		{
			return t * (2f - t);
		}

		// Token: 0x06004522 RID: 17698 RVA: 0x002080ED File Offset: 0x002062ED
		private float EaseInQuad(float t)
		{
			return t * t;
		}

		// Token: 0x040031B6 RID: 12726
		[NativeDisableUnsafePtrRestriction]
		public unsafe HeightsGrid.Data* heights;

		// Token: 0x040031B7 RID: 12727
		public PathData.DataPointers pointers;
	}

	// Token: 0x020005CD RID: 1485
	public struct ClearCollisionsGridJob : IJob
	{
		// Token: 0x06004523 RID: 17699 RVA: 0x002080F2 File Offset: 0x002062F2
		public void Execute()
		{
			this.collisions.Clear();
		}

		// Token: 0x040031B8 RID: 12728
		public TroopCollisions collisions;
	}

	// Token: 0x020005CE RID: 1486
	public struct RebuildCollisionsGridJob : Troops.ITroopJob
	{
		// Token: 0x06004524 RID: 17700 RVA: 0x002080FF File Offset: 0x002062FF
		public void Execute(Troops.Troop troop)
		{
			this.collisions.Add(troop);
		}

		// Token: 0x040031B9 RID: 12729
		public TroopCollisions.Concurrent collisions;
	}

	// Token: 0x020005CF RID: 1487
	public struct UpdateTroopAnimationsJob : Troops.ITroopJob
	{
		// Token: 0x06004525 RID: 17701 RVA: 0x0020810D File Offset: 0x0020630D
		public static bool IsSpecialAttack(UnitAnimation.State state)
		{
			return state == UnitAnimation.State.SpecialAttack || state == UnitAnimation.State.SpecialAttackMovingFront || state == UnitAnimation.State.SpecialAttackMovingRight || state == UnitAnimation.State.SpecialAttackMovingLeft || state == UnitAnimation.State.SpecialAttackMovingRear;
		}

		// Token: 0x06004526 RID: 17702 RVA: 0x0020812C File Offset: 0x0020632C
		public unsafe void Execute(Troops.Troop troop)
		{
			float dt = troop.data->dt;
			if (troop.attack_cd > 0f)
			{
				troop.attack_cd = math.max(troop.attack_cd - dt, 0f);
			}
			UnitAnimation.State cur_anim_state = troop.cur_anim_state;
			UnitAnimation.State cur_anim_state2;
			float cur_anim_speed;
			this.CalcAnim(troop, out cur_anim_state2, out cur_anim_speed);
			float forced_offset;
			this.CalcAltAnim(troop, ref cur_anim_state2, ref cur_anim_speed, out forced_offset);
			if (Troops.UpdateTroopAnimationsJob.IsSpecialAttack(cur_anim_state) && !Troops.UpdateTroopAnimationsJob.IsSpecialAttack(cur_anim_state2) && troop.cur_anim_time < troop.action_frame)
			{
				cur_anim_state2 = troop.cur_anim_state;
				cur_anim_speed = troop.cur_anim_speed;
			}
			if (cur_anim_state == UnitAnimation.State.Attack && (cur_anim_state2 == UnitAnimation.State.Death || cur_anim_state2 == UnitAnimation.State.SpecialDeath) && troop.cur_anim_time < troop.action_frame && !troop.HasFlags(Troops.Troop.Flags.Thrown))
			{
				cur_anim_state2 = troop.cur_anim_state;
				cur_anim_speed = troop.cur_anim_speed;
			}
			if (!this.TryBegin(troop, cur_anim_state2, dt, cur_anim_speed, forced_offset))
			{
				this.OnUpdateAnimation(troop, troop.cur_anim_state, dt, cur_anim_speed);
			}
			troop.blend_time = math.clamp(troop.blend_time + dt, 0f, troop.cur_anim_blend_time);
			troop.cur_anim_speed = cur_anim_speed;
			this.SetResult(troop);
		}

		// Token: 0x06004527 RID: 17703 RVA: 0x00208244 File Offset: 0x00206444
		private bool TryBegin(Troops.Troop troop, UnitAnimation.State new_state, float dt, float anim_speed, float forced_offset = -1f)
		{
			UnitAnimation.State cur_anim_state = troop.cur_anim_state;
			if (new_state == cur_anim_state && cur_anim_state != UnitAnimation.State.None)
			{
				return false;
			}
			UnitAnimation.Index index = this.StateToIdx(troop, new_state);
			if (forced_offset >= 0f)
			{
				this.OnBeginAnimation(troop, new_state, index, anim_speed < 0f, forced_offset);
				return true;
			}
			if (this.IsAnimShoot(new_state) && this.IsAnimShoot(cur_anim_state))
			{
				return false;
			}
			if (index == (UnitAnimation.Index)troop.cur_anim_idx)
			{
				return false;
			}
			KeyframeTextureBaker.AnimationClipDataBaked animInfo = troop.GetAnimInfo((int)index);
			KeyframeTextureBaker.AnimationClipDataBaked cur_anim_info = troop.cur_anim_info;
			if (troop.blend_time < cur_anim_info.BlendTime)
			{
				return false;
			}
			float action = cur_anim_info.GetAction(KeyframeTextureBaker.ActionFrame.ActionType.LeftFoot);
			float action2 = animInfo.GetAction(KeyframeTextureBaker.ActionFrame.ActionType.LeftFoot);
			float cur_anim_time = troop.cur_anim_time;
			bool flag = false;
			if (action > -1f && action2 > -1f)
			{
				flag = true;
				if (cur_anim_time > action && cur_anim_time < action + dt)
				{
					this.OnBeginAnimation(troop, new_state, index, anim_speed < 0f, action2);
					return true;
				}
			}
			float action3 = cur_anim_info.GetAction(KeyframeTextureBaker.ActionFrame.ActionType.RightFoot);
			float action4 = animInfo.GetAction(KeyframeTextureBaker.ActionFrame.ActionType.RightFoot);
			if (action3 > -1f && action4 > -1f)
			{
				flag = true;
				if (cur_anim_time > action3 && cur_anim_time < action3 + dt)
				{
					this.OnBeginAnimation(troop, new_state, index, anim_speed < 0f, action4);
					return true;
				}
			}
			if (!flag)
			{
				this.OnBeginAnimation(troop, new_state, index, anim_speed < 0f, 0f);
				return true;
			}
			return false;
		}

		// Token: 0x06004528 RID: 17704 RVA: 0x00208390 File Offset: 0x00206590
		private void SetResult(Troops.Troop troop)
		{
			float x = math.clamp(troop.cur_anim_time / troop.cur_anim_length, 0f, 1f) * troop.cur_anim_range + troop.cur_anim_offset;
			float y = math.clamp(troop.prev_anim_time / troop.prev_anim_length, 0f, 1f) * troop.prev_anim_range + troop.prev_anim_offset;
			troop.anim_result = new float3(x, y, troop.blend_time / troop.cur_anim_blend_time);
		}

		// Token: 0x06004529 RID: 17705 RVA: 0x00208418 File Offset: 0x00206618
		private unsafe void OnBeginAnimation(Troops.Troop troop, UnitAnimation.State anim_state, UnitAnimation.Index anim_idx, bool backwards = false, float ofs = 0f)
		{
			if (Troops.UpdateTroopAnimationsJob.IsSpecialAttack(troop.cur_anim_state) && !Troops.UpdateTroopAnimationsJob.IsSpecialAttack(anim_state))
			{
				troop.squad->SetFlags(Troops.SquadData.Flags.HasFinishedShot);
				if (troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger))
				{
					troop.CancelArrows();
				}
			}
			if (anim_idx != (UnitAnimation.Index)troop.cur_anim_idx)
			{
				if (!this.IsAnimNoBlend(troop.cur_anim_state) && !troop.def->is_siege_eq)
				{
					troop.blend_time = 0f;
				}
				troop.prev_anim_time = troop.cur_anim_time;
				troop.prev_anim_idx = troop.cur_anim_idx;
				troop.cur_anim_idx = (int)anim_idx;
				UnitAnimation.State prev_anim_state = troop.prev_anim_state;
				if (anim_state != troop.prev_anim_state && ((anim_state >= UnitAnimation.State.Idle && anim_state <= UnitAnimation.State.Ready) || (anim_state >= UnitAnimation.State.Move && anim_state <= UnitAnimation.State.Charge && (prev_anim_state < UnitAnimation.State.Move || prev_anim_state > UnitAnimation.State.Charge)) || anim_state == UnitAnimation.State.Ready_To_Surrender_01 || anim_state == UnitAnimation.State.Ready_To_Surrender_02 || anim_state == UnitAnimation.State.ShieldPack || anim_state == UnitAnimation.State.ShieldSetup))
				{
					ofs = troop.data->rand.NextFloat(0f, math.min(troop.cur_anim_length * 0.5f, 2f));
				}
			}
			troop.cur_anim_time = ofs;
			if (anim_state == UnitAnimation.State.Attack)
			{
				troop.SetFlags(Troops.Troop.Flags.Attacking);
				troop.attack_cd = troop.def->attack_interval;
				return;
			}
			troop.ClrFlags(Troops.Troop.Flags.Attacking);
			if (Troops.UpdateTroopAnimationsJob.IsSpecialAttack(anim_state))
			{
				troop.ClrFlags(Troops.Troop.Flags.ShootTrigger);
				troop.attack_cd = troop.def->attack_interval;
			}
		}

		// Token: 0x0600452A RID: 17706 RVA: 0x00208584 File Offset: 0x00206784
		private unsafe void OnActionFrame(Troops.Troop troop, UnitAnimation.State anim_state)
		{
			if (anim_state == UnitAnimation.State.Attack)
			{
				Troops.FortificationData* enemy_fortification = troop.enemy_fortification;
				if (enemy_fortification != null)
				{
					if (enemy_fortification->HasFlags(Troops.FortificationData.Flags.Demolished | Troops.FortificationData.Flags.GateHit))
					{
						return;
					}
					enemy_fortification->SetFlags(Troops.FortificationData.Flags.GateHit);
					float num = troop.squad->CTH_final * troop.squad->def->CTH_shoot_mod;
					enemy_fortification->gate_health -= num;
					enemy_fortification->last_attacker = troop.squad;
				}
				if (troop.enemy_id >= 0)
				{
					Troops.SquadData* squad = troop.squad;
					Unity.Mathematics.Random rand = squad->pdata->rand;
					Troops.Troop enemy = troop.enemy;
					if (enemy.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
					{
						return;
					}
					if (enemy.valid && !enemy.def->is_cavalry && !enemy.def->is_siege_eq && !enemy.HasFlags(Troops.Troop.Flags.Charging | Troops.Troop.Flags.PushedOff))
					{
						float num2 = 0.33f;
						bool flag = troop.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished);
						if (flag)
						{
							num2 = 1f;
						}
						if (rand.NextFloat(1f) < num2)
						{
							float num3 = rand.NextFloat(0.5f, 2f);
							if (flag)
							{
								num3 *= 2f;
							}
							float2 @float = (enemy.pos - troop.pos).GetNormalized();
							if (flag)
							{
								@float *= 2f;
							}
							enemy.PushOff(@float, num3);
							troop.PushOff(@float, 0.5f);
						}
					}
					bool flag2 = false;
					Troops.SquadData* squad2 = enemy.squad;
					if (squad2 == null)
					{
						return;
					}
					float num4;
					if (squad->def->is_siege_eq && enemy.def->is_siege_eq)
					{
						num4 = squad->def->CTH_siege_vs_siege;
					}
					else
					{
						num4 = squad->CTH_final;
					}
					num4 *= 1f + squad2->CTH_against_me_mod / 100f;
					float defense_final = squad2->defense_final;
					if (enemy.squad->def->max_health > 0f)
					{
						enemy.GetHit(num4 / (100f + defense_final));
						return;
					}
					if (!flag2)
					{
						if (enemy.def->is_cavalry)
						{
							num4 *= squad->def->CTH_cavalry_mod;
						}
						flag2 = (rand.NextFloat(1f) < num4 / (100f + defense_final));
					}
					this.TryShock(squad, enemy.squad, false);
					if (!flag2)
					{
						return;
					}
					enemy.Kill();
					return;
				}
			}
			else if (Troops.UpdateTroopAnimationsJob.IsSpecialAttack(anim_state))
			{
				this.ApplyShoot(troop);
			}
		}

		// Token: 0x0600452B RID: 17707 RVA: 0x00208800 File Offset: 0x00206A00
		private unsafe void TryShock(Troops.SquadData* squad_data, Troops.SquadData* enemy_data, bool is_trample = false)
		{
			float num = squad_data->shock_chance;
			float resiliance_total = enemy_data->resiliance_total;
			if (resiliance_total < 0f)
			{
				num -= resiliance_total;
			}
			Unity.Mathematics.Random rand = squad_data->pdata->rand;
			if (rand.NextFloat(100f) < num)
			{
				float num2 = squad_data->shock_damage_base;
				if (is_trample)
				{
					num2 += squad_data->shock_damage_bonus_trample;
				}
				enemy_data->shock_morale_dmg_acc = enemy_data->shock_morale_dmg_acc + num2 / (float)enemy_data->logic_alive;
			}
		}

		// Token: 0x0600452C RID: 17708 RVA: 0x00208868 File Offset: 0x00206A68
		private unsafe void ApplyShoot(Troops.Troop troop)
		{
			for (int i = 0; i < troop.arrow_count; i++)
			{
				int arrowID = troop.GetArrowID(i);
				if (arrowID != -1)
				{
					Troops.Arrow arrow = troop.data->GetArrow(troop.cur_thread_id, arrowID);
					arrow.ClrFlags(Troops.Arrow.Flags.AboutToShoot);
					arrow.SetFlags(Troops.Arrow.Flags.BeingShot);
					troop.SetArrow(i, -1);
				}
			}
		}

		// Token: 0x0600452D RID: 17709 RVA: 0x002088C4 File Offset: 0x00206AC4
		private void OnUpdateAnimation(Troops.Troop troop, UnitAnimation.State anim_state, float dt, float anim_speed)
		{
			float cur_anim_time = troop.cur_anim_time;
			float action_frame = troop.action_frame;
			troop.cur_anim_time += dt * anim_speed;
			bool flag = troop.cur_anim_time >= troop.cur_anim_length;
			bool flag2 = troop.cur_anim_time < 0f;
			if ((cur_anim_time <= action_frame || (troop.HasFlags(Troops.Troop.Flags.ShootTrigger) && Troops.UpdateTroopAnimationsJob.IsSpecialAttack(troop.cur_anim_state))) && troop.cur_anim_time > action_frame)
			{
				this.OnActionFrame(troop, anim_state);
			}
			if (flag || flag2)
			{
				if (anim_state == UnitAnimation.State.Death || anim_state == UnitAnimation.State.SpecialDeath)
				{
					if (troop.HasFlags(Troops.Troop.Flags.Thrown))
					{
						return;
					}
					float num = troop.cur_anim_time - troop.cur_anim_length;
					if (num >= 5f)
					{
						float3 pos3d = troop.pos3d;
						pos3d.y -= dt * 0.1f;
						troop.pos3d = pos3d;
						if (num > 55f)
						{
							troop.Destroy();
						}
					}
					return;
				}
				else
				{
					if (anim_state == UnitAnimation.State.ShieldSetup)
					{
						anim_state = UnitAnimation.State.SpecialReady;
					}
					if (anim_state == UnitAnimation.State.ShieldPack)
					{
						anim_state = UnitAnimation.State.Idle;
					}
					if (anim_state == UnitAnimation.State.Attack)
					{
						troop.ClrFlags(Troops.Troop.Flags.Attacking);
						anim_state = UnitAnimation.State.Ready;
					}
					if (Troops.UpdateTroopAnimationsJob.IsSpecialAttack(anim_state))
					{
						troop.ClrFlags(Troops.Troop.Flags.Shooting);
						anim_state = UnitAnimation.State.SpecialReady;
					}
					if (anim_state == UnitAnimation.State.Ready_To_Surrender_01)
					{
						anim_state = UnitAnimation.State.Idle_Surrender_01;
					}
					else if (anim_state == UnitAnimation.State.Ready_To_Surrender_02)
					{
						anim_state = UnitAnimation.State.Idle_Surrender_02;
					}
					else
					{
						if (flag)
						{
							troop.cur_anim_time -= troop.cur_anim_length;
						}
						if (flag2)
						{
							troop.cur_anim_time += troop.cur_anim_length;
						}
					}
					UnitAnimation.Index anim_idx = this.StateToIdx(troop, anim_state);
					this.OnBeginAnimation(troop, anim_state, anim_idx, anim_speed < 0f, troop.cur_anim_time);
				}
			}
		}

		// Token: 0x0600452E RID: 17710 RVA: 0x00208A58 File Offset: 0x00206C58
		private unsafe void CalcMoveAnim(Troops.Troop troop, float move_speed, bool attack_move, out UnitAnimation.State anim_state, out float speed)
		{
			Troops.DefData* def = troop.def;
			float walk_anim_speed = def->walk_anim_speed;
			float trot_anim_speed = def->trot_anim_speed;
			float num = attack_move ? def->charge_anim_speed : def->run_anim_speed;
			float sprint_anim_speed = def->sprint_anim_speed;
			float num2 = (walk_anim_speed + trot_anim_speed) * def->walk_to_trot_ratio;
			float num3 = math.lerp(trot_anim_speed, num, def->trot_to_run_ratio);
			float num4 = (num + sprint_anim_speed) * def->run_to_sprint_ratio;
			if (attack_move)
			{
				speed = move_speed / num;
				anim_state = UnitAnimation.State.Charge;
				return;
			}
			if (move_speed >= num4 && sprint_anim_speed > 0f)
			{
				speed = move_speed / sprint_anim_speed;
				anim_state = UnitAnimation.State.Sprint;
				return;
			}
			if (move_speed >= num3)
			{
				speed = move_speed / num;
				anim_state = UnitAnimation.State.Run;
				return;
			}
			if (move_speed >= num2 && trot_anim_speed > 0f)
			{
				speed = move_speed / trot_anim_speed;
				anim_state = UnitAnimation.State.Trot;
				return;
			}
			speed = move_speed / walk_anim_speed;
			anim_state = UnitAnimation.State.Move;
		}

		// Token: 0x0600452F RID: 17711 RVA: 0x00208B18 File Offset: 0x00206D18
		private Point ReverseRot(float rot)
		{
			float x = math.radians(90f - rot);
			float x2 = math.cos(x);
			float y = math.sin(x);
			return new Point(x2, y);
		}

		// Token: 0x06004530 RID: 17712 RVA: 0x00208B48 File Offset: 0x00206D48
		private unsafe void CalcAnim(Troops.Troop troop, out UnitAnimation.State anim_state, out float anim_speed)
		{
			anim_speed = 1f;
			bool flag = troop.squad->HasFlags(Troops.SquadData.Flags.Fled);
			if (troop.squad->battle_side == troop.data->winner && !flag)
			{
				anim_state = UnitAnimation.State.Cheer;
				return;
			}
			if (troop.data->winner != -1)
			{
				anim_state = UnitAnimation.State.Idle;
				return;
			}
			if (!flag)
			{
				if (troop.HasFlags(Troops.Troop.Flags.PushedOff) && troop.HasValidAnim(UnitAnimation.Index.Hit))
				{
					anim_state = UnitAnimation.State.Hit;
					return;
				}
				if (troop.HasFlags(Troops.Troop.Flags.Attacking))
				{
					anim_state = UnitAnimation.State.Attack;
					return;
				}
				if (troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger))
				{
					if (troop.HasFlags(Troops.Troop.Flags.ShootTrigger))
					{
						if (troop.HasFlags(Troops.Troop.Flags.Fighting))
						{
							troop.CancelArrows();
						}
						else if (troop.squad->def->is_cavalry || (!troop.HasFlags(Troops.Troop.Flags.Moving) && math.abs(troop.rot_y - troop.tgt_arrow_rot) < 10f))
						{
							troop.SetFlags(Troops.Troop.Flags.Shooting);
						}
					}
					if (troop.HasFlags(Troops.Troop.Flags.Shooting))
					{
						anim_state = UnitAnimation.State.SpecialAttack;
						troop.squad->SetFlags(Troops.SquadData.Flags.Shooting);
						return;
					}
				}
			}
			if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadder))
			{
				float interpolatedHeight = this.heights->GetInterpolatedHeight(troop.pos.x, troop.pos.y);
				if (troop.pos3d.y - interpolatedHeight <= 0.3f)
				{
					anim_state = UnitAnimation.State.Move;
				}
				anim_state = UnitAnimation.State.LadderClimb;
				if (troop.HasFlags(Troops.Troop.Flags.MovingDown))
				{
					anim_speed = -1f;
				}
				return;
			}
			if (troop.vel_spd_t.z > 0.2f && (troop.HasFlags(Troops.Troop.Flags.Moving) || troop.vel_spd_t.w > 0.25f) && (!troop.HasFlags(Troops.Troop.Flags.Collided) || troop.squad->HasFlags(Troops.SquadData.Flags.Moving)))
			{
				float move_speed = math.max(troop.vel_spd_t.z, troop.def->min_speed);
				bool attack_move = troop.HasFlags(Troops.Troop.Flags.AttackMove) || troop.squad->has_accessible_enemies;
				if (troop.vel_spd_t.z < 0.5f || troop.squad->HasFlags(Troops.SquadData.Flags.Retreating))
				{
					attack_move = false;
				}
				this.CalcMoveAnim(troop, move_speed, attack_move, out anim_state, out anim_speed);
				return;
			}
			if (!flag)
			{
				if (troop.HasFlags(Troops.Troop.Flags.Fighting))
				{
					if (troop.attack_cd > 0f)
					{
						anim_state = UnitAnimation.State.Ready;
						return;
					}
					if ((troop.enemy_fortification != null || troop.def->can_attack_melee) && (!troop.HasFlags(Troops.Troop.Flags.PushedOff) || troop.cur_anim_state == UnitAnimation.State.Attack))
					{
						anim_state = UnitAnimation.State.Attack;
						return;
					}
				}
				if (troop.cur_anim_state == UnitAnimation.State.SpecialReady)
				{
					anim_state = UnitAnimation.State.SpecialReady;
					return;
				}
			}
			if (troop.squad->is_Fighting || troop.squad->has_accessible_enemies)
			{
				anim_state = UnitAnimation.State.Ready;
				return;
			}
			anim_state = UnitAnimation.State.Idle;
		}

		// Token: 0x06004531 RID: 17713 RVA: 0x00208E1C File Offset: 0x0020701C
		private bool IsAnimShoot(UnitAnimation.State state)
		{
			for (int i = 0; i < Troops.UpdateTroopAnimationsJob.shoot_animations.Length; i++)
			{
				if (state == Troops.UpdateTroopAnimationsJob.shoot_animations[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004532 RID: 17714 RVA: 0x00208E48 File Offset: 0x00207048
		private bool IsAnimNoBlend(UnitAnimation.State state)
		{
			for (int i = 0; i < Troops.UpdateTroopAnimationsJob.no_blend_states.Length; i++)
			{
				if (state == Troops.UpdateTroopAnimationsJob.no_blend_states[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004533 RID: 17715 RVA: 0x00208E74 File Offset: 0x00207074
		private unsafe void CalcAltAnim(Troops.Troop troop, ref UnitAnimation.State anim_state, ref float anim_speed, out float forced_offset)
		{
			forced_offset = -1f;
			UnitAnimation.State cur_anim_state = troop.cur_anim_state;
			if (cur_anim_state == UnitAnimation.State.ShieldSetup || cur_anim_state == UnitAnimation.State.ShieldPack)
			{
				anim_state = cur_anim_state;
				return;
			}
			if (anim_state == UnitAnimation.State.SpecialAttack)
			{
				UnitAnimation.State state = UnitAnimation.State.SpecialAttack;
				if (troop.HasFlags(Troops.Troop.Flags.Moving) && troop.def->is_cavalry)
				{
					Point pt = this.ReverseRot(troop.rot_y);
					Point pt2 = this.ReverseRot(troop.tgt_arrow_rot);
					float num = math.acos(math.dot(pt, pt2));
					float3 y = math.cross(pt, pt2);
					num *= math.sign(math.dot(Vector3.up, y));
					float num2 = math.degrees(num);
					if (num2 >= -22.5f && num2 < 22.5f)
					{
						state = UnitAnimation.State.SpecialAttackMovingFront;
					}
					else if (num2 >= -112.5f && num2 < -22.5f)
					{
						state = UnitAnimation.State.SpecialAttackMovingLeft;
					}
					else if (num2 >= -202.5f && num2 < -112.5f)
					{
						state = UnitAnimation.State.SpecialAttackMovingRear;
					}
					else
					{
						state = UnitAnimation.State.SpecialAttackMovingRight;
					}
				}
				if (cur_anim_state != state)
				{
					anim_state = state;
					if (this.IsAnimShoot(troop.cur_anim_state))
					{
						forced_offset = troop.cur_anim_time;
					}
				}
			}
			if (!this.IsAnimShoot(cur_anim_state) && this.IsAnimShoot(UnitAnimation.State.SpecialAttack) && troop.HasValidAnim(UnitAnimation.Index.ShieldSetup))
			{
				anim_state = UnitAnimation.State.ShieldSetup;
				return;
			}
			if (cur_anim_state == UnitAnimation.State.SpecialReady && anim_state != UnitAnimation.State.SpecialReady && anim_state != UnitAnimation.State.SpecialAttack && anim_state != UnitAnimation.State.SpecialDeath && troop.HasValidAnim(UnitAnimation.Index.ShieldPack))
			{
				anim_state = UnitAnimation.State.ShieldPack;
				return;
			}
		}

		// Token: 0x06004534 RID: 17716 RVA: 0x00208FE0 File Offset: 0x002071E0
		private unsafe UnitAnimation.Index StateToIdx(Troops.Troop troop, UnitAnimation.State state)
		{
			switch (state)
			{
			case UnitAnimation.State.Idle:
			{
				float num = troop.data->rand.NextFloat();
				float num2 = 0.1f;
				if (troop.cur_anim_idx != 1)
				{
					num2 = 0f;
				}
				if (num < num2)
				{
					return UnitAnimation.Index.IdleAlt_01;
				}
				if (num < num2 * 2f)
				{
					return UnitAnimation.Index.IdleAlt_02;
				}
				if (num < num2 * 3f)
				{
					return UnitAnimation.Index.IdleAlt_03;
				}
				return UnitAnimation.Index.Idle;
			}
			case UnitAnimation.State.Ready:
			{
				float num3 = troop.data->rand.NextFloat();
				float num4 = 0.05f;
				if (troop.squad->HasFlags(Troops.SquadData.Flags.Fighting))
				{
					num4 = 0.3f;
				}
				if (troop.cur_anim_idx != 2)
				{
					num4 = 0f;
				}
				if (num3 < num4)
				{
					return UnitAnimation.Index.ReadyAlt_01;
				}
				if (num3 < num4 * 2f)
				{
					return UnitAnimation.Index.ReadyAlt_02;
				}
				return UnitAnimation.Index.Ready;
			}
			case UnitAnimation.State.Attack:
				if (troop.data->rand.NextFloat() < 0.4f)
				{
					return UnitAnimation.Index.Attack_02;
				}
				return UnitAnimation.Index.Attack;
			case UnitAnimation.State.Death:
			{
				float num5 = troop.data->rand.NextFloat();
				if (num5 < 0.3f)
				{
					return UnitAnimation.Index.Death_03;
				}
				if (num5 < 0.6f)
				{
					return UnitAnimation.Index.Death_02;
				}
				return UnitAnimation.Index.Death;
			}
			case UnitAnimation.State.SpecialReady:
			{
				float num6 = troop.data->rand.NextFloat();
				float num7 = 0.01f;
				if (troop.cur_anim_idx != 23)
				{
					num7 = 0f;
				}
				if (num6 < num7)
				{
					return UnitAnimation.Index.SpecialReadyAlt_01;
				}
				if (num6 < num7 * 2f)
				{
					return UnitAnimation.Index.SpecialReadyAlt_02;
				}
				return UnitAnimation.Index.SpecialReady;
			}
			case UnitAnimation.State.SpecialDeath:
				if (troop.data->rand.NextFloat() < 0.5f)
				{
					return UnitAnimation.Index.SpecialDeath_01;
				}
				return UnitAnimation.Index.SpecialDeath_02;
			case UnitAnimation.State.LadderClimb:
				return UnitAnimation.Index.Ladder_Climb_Up;
			case UnitAnimation.State.LadderIdle:
				return UnitAnimation.Index.Ladder_Idle;
			case UnitAnimation.State.LadderClimbDown:
				return UnitAnimation.Index.Ladder_Climb_Down;
			case UnitAnimation.State.Ready_To_Surrender_01:
				return UnitAnimation.Index.Ready_To_Surrender_01;
			case UnitAnimation.State.Ready_To_Surrender_02:
				return UnitAnimation.Index.Ready_To_Surrender_02;
			case UnitAnimation.State.Idle_Surrender_01:
				return UnitAnimation.Index.Idle_Surrender_01;
			case UnitAnimation.State.Idle_Surrender_02:
				return UnitAnimation.Index.Idle_Surrender_02;
			case UnitAnimation.State.Cheer:
			{
				float num8 = troop.data->rand.NextFloat();
				if (num8 < 0.09f)
				{
					return UnitAnimation.Index.Idle_Cheer_01;
				}
				if (num8 < 0.18f)
				{
					return UnitAnimation.Index.Idle_Cheer_02;
				}
				return UnitAnimation.Index.Idle_Cheer;
			}
			case UnitAnimation.State.ShieldSetup:
				return UnitAnimation.Index.ShieldSetup;
			case UnitAnimation.State.ShieldPack:
				return UnitAnimation.Index.ShieldPack;
			case UnitAnimation.State.Hit:
				return UnitAnimation.Index.Hit;
			case UnitAnimation.State.HitSpecial:
				return UnitAnimation.Index.HitSpecial;
			case UnitAnimation.State.SpecialAttackMovingFront:
				return UnitAnimation.Index.SpecialAttackMovingFront;
			case UnitAnimation.State.SpecialAttackMovingLeft:
				return UnitAnimation.Index.SpecialAttackMovingLeft;
			case UnitAnimation.State.SpecialAttackMovingRight:
				return UnitAnimation.Index.SpecialAttackMovingRight;
			case UnitAnimation.State.SpecialAttackMovingRear:
				return UnitAnimation.Index.SpecialAttackMovingRear;
			}
			return (UnitAnimation.Index)state;
		}

		// Token: 0x040031BA RID: 12730
		[NativeDisableUnsafePtrRestriction]
		public unsafe HeightsGrid.Data* heights;

		// Token: 0x040031BB RID: 12731
		public static readonly UnitAnimation.State[] no_blend_states = new UnitAnimation.State[]
		{
			UnitAnimation.State.ShieldPack,
			UnitAnimation.State.ShieldSetup
		};

		// Token: 0x040031BC RID: 12732
		public static readonly UnitAnimation.State[] shoot_animations = new UnitAnimation.State[]
		{
			UnitAnimation.State.SpecialAttack,
			UnitAnimation.State.SpecialAttackMovingFront,
			UnitAnimation.State.SpecialAttackMovingRight,
			UnitAnimation.State.SpecialAttackMovingLeft,
			UnitAnimation.State.SpecialAttackMovingRear
		};
	}

	// Token: 0x020005D0 RID: 1488
	public struct FillTroopRenderDataJob : Troops.ITroopJob
	{
		// Token: 0x06004536 RID: 17718 RVA: 0x00209240 File Offset: 0x00207440
		private unsafe bool Visible(float3 pos)
		{
			for (int i = 0; i < 6; i++)
			{
				if (math.dot(pos, this.f_planes[i].xyz) + this.f_planes[i].w + 20f < 0f)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004537 RID: 17719 RVA: 0x0020929C File Offset: 0x0020749C
		public unsafe void Execute(Troops.Troop troop)
		{
			float3 pos3d = troop.pos3d;
			for (int i = 0; i < 5; i++)
			{
				int num = troop.id * 5 + i;
				float4 @float = troop.data->dust_particle_positions[num];
				if (@float.w > 0f && this.Visible(@float.xyz))
				{
					troop.dust_data_buffer.Add(new TextureBaker.InstancedDustDrawerBatched.DrawCallData
					{
						pos_progress = @float
					});
				}
			}
			if (troop.squad->target_previewed && !troop.squad->in_drag && this.Visible(troop.preview_position))
			{
				GrowBuffer<TextureBaker.InstancedSelectionDrawerBatched.DrawCallData> selection_data_buffer = troop.selection_data_buffer;
				TextureBaker.InstancedSelectionDrawerBatched.DrawCallData item = default(TextureBaker.InstancedSelectionDrawerBatched.DrawCallData);
				item.pos = troop.preview_position;
				if (troop.squad->selected)
				{
					item.color_id = 0;
				}
				else
				{
					item.color_id = 6;
				}
				item.rot = -troop.squad->preview_formation_dir * 0.017453292f;
				item.scale = troop.def->selection_radius;
				selection_data_buffer.Add(item);
			}
			if (this.Visible(pos3d))
			{
				troop.model_data_buffer.Add(new TextureBaker.InstancedSkinningDrawerBatched.DrawCallData
				{
					pos = troop.pos3d,
					prevPos = troop.previous_pos3d,
					rot = troop.rot3d,
					anim_data = troop.anim_result,
					kingdom_color_id = troop.squad->kingdom_color_id,
					stance_color_id = (int)troop.squad->stance_to_player
				});
				if ((!troop.squad->selected && !troop.squad->highlighted && !troop.squad->previewed) || !troop.squad->are_selection_circles_enabled)
				{
					return;
				}
				troop.selection_data_buffer.Add(new TextureBaker.InstancedSelectionDrawerBatched.DrawCallData
				{
					pos = troop.pos3d,
					color_id = (int)troop.squad->stance_to_player,
					scale = troop.def->selection_radius,
					rot = (troop.rot_y - 90f) * 0.017453292f
				});
			}
			if (troop.squad->in_drag)
			{
				if (this.Visible(troop.preview_position))
				{
					troop.selection_data_buffer.Add(new TextureBaker.InstancedSelectionDrawerBatched.DrawCallData
					{
						pos = troop.preview_position,
						color_id = 6,
						scale = troop.def->selection_radius,
						rot = -troop.squad->preview_formation_dir * 0.017453292f
					});
					return;
				}
			}
			else if (!troop.squad->target_previewed)
			{
				troop.preview_position = troop.pos3d;
			}
		}

		// Token: 0x040031BD RID: 12733
		[NativeDisableUnsafePtrRestriction]
		public unsafe float4* f_planes;
	}

	// Token: 0x020005D1 RID: 1489
	public struct FillArrowRenderDataJob : Troops.IArrowJob
	{
		// Token: 0x06004538 RID: 17720 RVA: 0x00209584 File Offset: 0x00207784
		private unsafe bool Visible(float3 pos)
		{
			for (int i = 0; i < 6; i++)
			{
				if (math.dot(pos, this.f_planes[i].xyz) + this.f_planes[i].w + 20f < 0f)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004539 RID: 17721 RVA: 0x002095E0 File Offset: 0x002077E0
		public unsafe void Execute(Troops.Arrow arrow)
		{
			float3 pos = arrow.pos;
			if (this.Visible(pos) && !arrow.HasFlags(Troops.Arrow.Flags.FloatingInAir))
			{
				arrow.model_data_buffer.Add(new TextureBaker.InstancedArrowDrawerBatched.DrawCallData
				{
					pos = new float4(pos, arrow.scale),
					rot = arrow.rot
				});
			}
			if (arrow.salvo->def->draw_trail && (this.Visible(arrow.startPos) || this.Visible(arrow.endPos)))
			{
				arrow.trail_data_buffer.Add(new TextureBaker.InstancedTrailDrawerBatched.DrawCallData
				{
					start = arrow.startPos,
					end = arrow.endPos,
					dur = arrow.duration,
					t = arrow.trail_progress,
					startVelocity = arrow.startVelocity,
					gravity = arrow.salvo->def->gravity
				});
			}
		}

		// Token: 0x040031BE RID: 12734
		[NativeDisableUnsafePtrRestriction]
		public unsafe float4* f_planes;
	}

	// Token: 0x020005D2 RID: 1490
	public struct UpdateTroopRotationsJob : Troops.ITroopJob
	{
		// Token: 0x0600453A RID: 17722 RVA: 0x002096F0 File Offset: 0x002078F0
		public unsafe void Execute(Troops.Troop troop)
		{
			float to = this.CalcTgtRot(troop);
			float rot_y = troop.rot_y;
			float num = Angle.Diff(rot_y, to);
			float num2 = troop.def->turn_speed;
			if (troop.def->is_cavalry)
			{
				num2 *= 0.666f;
			}
			float num3 = num2 * troop.data->dt;
			num = math.clamp(num, -num3, num3);
			troop.rot_y = rot_y + num;
			if (troop.rot_y >= 360f)
			{
				troop.rot_y -= 360f;
			}
			if (troop.rot_y < 0f)
			{
				troop.rot_y += 360f;
			}
			if (troop.squad->HasFlags(Troops.SquadData.Flags.Fled))
			{
				troop.rot_y += 360f;
			}
			bool flag = troop.pa_id == 0;
			if (!flag && this.pointers.GetPA(troop.pa_id - 1).IsGround())
			{
				flag = true;
			}
			float rot_y2 = troop.rot_y;
			if (troop.def->terrain_normal_checks <= 0 || !flag || this.heights == null)
			{
				troop.rot3d = new float3(0f, rot_y2, 0f);
				return;
			}
			float3 @float = this.CalcTerrainNormal(troop, this.heights);
			if (float.IsNaN(@float.x))
			{
				troop.rot3d = new float3(0f, rot_y2, 0f);
				return;
			}
			float3 float2 = @float;
			float x = math.radians(90f - rot_y2);
			float x2 = math.cos(x);
			float z = math.sin(x);
			float3 float3 = new float3(x2, 0f, z);
			float3 v = float3 - math.dot(float3, @float) * @float;
			if (v.Equals(float2))
			{
				troop.rot3d = new float3(0f, rot_y2, 0f);
				return;
			}
			Vector3 eulerAngles = Quaternion.LookRotation(v, float2).eulerAngles;
			eulerAngles.x = math.clamp(eulerAngles.x, -troop.def->max_rotation_x, troop.def->max_rotation_x);
			eulerAngles.z = math.clamp(eulerAngles.z, -troop.def->max_rotation_z, troop.def->max_rotation_z);
			troop.rot3d = eulerAngles;
		}

		// Token: 0x0600453B RID: 17723 RVA: 0x0020995C File Offset: 0x00207B5C
		private unsafe float3 CalcTerrainNormal(Troops.Troop troop, HeightsGrid.Data* heights)
		{
			Troops.DefData defData = *troop.squad->def;
			float3 @float = 0;
			float3 float2 = 0;
			float3 lhs = 0;
			float3 rhs = 0;
			float rot_y = troop.rot_y;
			for (int i = 0; i < troop.def->terrain_normal_checks; i++)
			{
				Point pt;
				switch (i)
				{
				case 0:
					pt = troop.def->terrain_normal_point_1;
					break;
				case 1:
					pt = troop.def->terrain_normal_point_2;
					break;
				case 2:
					pt = troop.def->terrain_normal_point_3;
					break;
				case 3:
					pt = troop.def->terrain_normal_point_4;
					break;
				default:
					return new float3(0f, 1f, 0f);
				}
				pt *= troop.def->radius;
				float2 float3 = troop.pos.pos + pt.GetRotated(rot_y);
				float height = this.GetHeight(ref *heights, float3);
				float3 float4 = new float3(float3.x, height, float3.y);
				switch (i)
				{
				case 0:
					@float = float4;
					break;
				case 1:
					float2 = float4;
					break;
				case 2:
					lhs = float4;
					break;
				case 3:
					rhs = float4;
					break;
				default:
					return new float3(0f, 1f, 0f);
				}
			}
			if (troop.def->terrain_normal_checks == 2)
			{
				float3 float5 = math.normalize(@float - float2);
				float3 y = new float3(float5.z, float5.y, -float5.x);
				return math.normalize(math.cross(float5, y));
			}
			float3 x = float2 - @float;
			float3 y2 = lhs - @float;
			float3 float6 = math.cross(x, y2);
			if (float6.y < 0f)
			{
				float6 *= -1f;
			}
			float6 = math.normalize(float6);
			if (troop.def->terrain_normal_checks == 4)
			{
				float3 x2 = float2 - rhs;
				float3 y3 = lhs - rhs;
				float3 float7 = math.cross(x2, y3);
				if (float7.y < 0f)
				{
					float7 *= -1f;
				}
				float7 = math.normalize(float7);
				return (float6 + float7) / 2f;
			}
			return float6;
		}

		// Token: 0x0600453C RID: 17724 RVA: 0x002080D9 File Offset: 0x002062D9
		private float GetHeight(ref HeightsGrid.Data heights, float2 pos)
		{
			return heights.GetInterpolatedHeight(pos.x, pos.y);
		}

		// Token: 0x0600453D RID: 17725 RVA: 0x00209BB4 File Offset: 0x00207DB4
		private unsafe float CalcTgtRot(Troops.Troop troop)
		{
			PPos pos = troop.pos;
			float result;
			if (troop.HasFlags(Troops.Troop.Flags.Attacking))
			{
				if (troop.enemy_id < 0)
				{
					result = troop.rot_y;
				}
				else
				{
					this.CalcRot(pos, troop.enemy.pos2d, 0f, out result);
				}
				return result;
			}
			if (troop.def->is_siege_eq)
			{
				if (troop.squad->HasFlags(Troops.SquadData.Flags.Moving) && troop.vel_spd_t.z > 0.1f)
				{
					this.CalcRot(default(float2), troop.vel_spd_t.xy + troop.steer.xy, 0f, out result);
				}
				else
				{
					if ((troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger) || troop.cur_anim_state == UnitAnimation.State.SpecialAttack || troop.cur_anim_state == UnitAnimation.State.SpecialReady) && !troop.HasFlags(Troops.Troop.Flags.Moving))
					{
						return troop.tgt_arrow_rot;
					}
					result = troop.squad->rot;
				}
				return result;
			}
			if (pos.paID > 0)
			{
				PathData.PassableArea pa = this.pointers.GetPA(pos.paID - 1);
				if (pa.type == PathData.PassableArea.Type.Ladder && this.CalcRot(0, -pa.GetNormalUp().xz, 0f, out result))
				{
					return result;
				}
			}
			Troops.FortificationData* enemy_fortification = troop.enemy_fortification;
			if (enemy_fortification != null)
			{
				PPos tgt_pos = troop.tgt_pos;
				float num = troop.def->attack_range + troop.def->radius;
				if (pos.SqrDist(enemy_fortification->pos) < num * num && this.CalcRot(tgt_pos, enemy_fortification->pos, 0f, out result))
				{
					return result;
				}
			}
			if ((troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger) || troop.cur_anim_state == UnitAnimation.State.SpecialAttack || troop.cur_anim_state == UnitAnimation.State.SpecialReady) && !troop.HasFlags(Troops.Troop.Flags.Moving))
			{
				return troop.tgt_arrow_rot;
			}
			float d2min;
			if (troop.fight_status == 0)
			{
				d2min = (troop.HasFlags(Troops.Troop.Flags.Moving) ? 1f : 4f);
				if (troop.squad->IsRearranging())
				{
					d2min = troop.squad->rearrange_min_safe_dist;
				}
			}
			else
			{
				d2min = 9f;
			}
			if (this.CalcRot(pos, troop.tgt_pos, d2min, out result))
			{
				return result;
			}
			if (troop.enemy_id >= 0 && troop.fight_status > 0 && troop.squad->command != Logic.Squad.Command.Disengage && troop.squad->command != Logic.Squad.Command.Move && this.CalcRot(pos, troop.enemy.pos2d, 0f, out result))
			{
				return result;
			}
			result = (troop.squad->HasFlags(Troops.SquadData.Flags.Moving) ? troop.rot_y : troop.squad->rot);
			return result;
		}

		// Token: 0x0600453E RID: 17726 RVA: 0x00209EA4 File Offset: 0x002080A4
		private bool CalcRot(float2 from, float2 to, float d2min, out float rot)
		{
			float2 @float = to - from;
			if (math.lengthsq(@float) <= d2min)
			{
				rot = 0f;
				return false;
			}
			rot = 90f - math.degrees(math.atan2(@float.y, @float.x));
			return true;
		}

		// Token: 0x040031BF RID: 12735
		public PathData.DataPointers pointers;

		// Token: 0x040031C0 RID: 12736
		[NativeDisableUnsafePtrRestriction]
		public unsafe HeightsGrid.Data* heights;
	}

	// Token: 0x020005D3 RID: 1491
	public struct CalcKilledTroopsJob : Troops.ISquadJob
	{
		// Token: 0x0600453F RID: 17727 RVA: 0x00209EEC File Offset: 0x002080EC
		public void Execute(ref Troops.SquadData squad)
		{
			if (squad.HasFlags(Troops.SquadData.Flags.HasKilledTroops))
			{
				Troops.Troop troop = squad.FirstTroop;
				while (troop <= squad.LastTroop)
				{
					if (troop.HasFlags(Troops.Troop.Flags.Killed) && !troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						if (troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger))
						{
							troop.CancelArrows();
						}
						troop.SetFlags(Troops.Troop.Flags.Dead);
						troop.fight_status = 0;
						troop.enemy_id = -1;
						troop.range_enemy_squad_id = -1;
						troop.find_enemy_cd = 0f;
						troop.rot_y += 360f;
					}
					troop = ++troop;
				}
				squad.ClrFlags(Troops.SquadData.Flags.HasKilledTroops);
			}
			int num = 0;
			Troops.Troop troop2 = squad.FirstTroop;
			while (troop2 <= squad.LastTroop)
			{
				if (!troop2.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && (squad.main_squad_id == -1 || troop2.squad_id == squad.id))
				{
					num++;
				}
				troop2 = ++troop2;
			}
			squad.logic_alive = num;
		}
	}

	// Token: 0x020005D4 RID: 1492
	private enum BoneSettingType
	{
		// Token: 0x040031C2 RID: 12738
		Animator,
		// Token: 0x040031C3 RID: 12739
		BakedTexture
	}

	// Token: 0x020005D5 RID: 1493
	public struct MoveArrowsJob : Troops.IArrowJob
	{
		// Token: 0x06004540 RID: 17728 RVA: 0x00209FFC File Offset: 0x002081FC
		public unsafe void Execute(Troops.Arrow arrow)
		{
			float gravity = arrow.salvo->def->gravity;
			if (arrow.HasFlags(Troops.Arrow.Flags.BeingShot))
			{
				Troops.Troop troop = arrow.data->GetTroop(arrow.cur_thread_id, arrow.cur_arrow_troop);
				float3 pos3d = troop.pos3d;
				float4 vel_spd_t = troop.vel_spd_t;
				if (vel_spd_t.z > 0.1f)
				{
					pos3d.xz += vel_spd_t.xy * vel_spd_t.z / 2f;
				}
				float3 endPos = arrow.endPos;
				float3 @float = endPos - pos3d;
				float shoot_height = arrow.salvo->def->shoot_height;
				float shoot_offset = arrow.salvo->def->shoot_offset;
				pos3d.y += shoot_height;
				pos3d.xz += shoot_offset * math.normalize(@float.xz);
				arrow.startPos = pos3d;
				arrow.pos = pos3d;
				float num = 0f;
				float f = 0f;
				float num2 = math.length(math.float2(endPos.x, endPos.z) - math.float2(pos3d.x, pos3d.z));
				float num3 = pos3d.y - endPos.y;
				float num4 = Mathf.Sqrt(num2 * gravity);
				float min_shoot_speed = arrow.salvo->def->min_shoot_speed;
				if (num4 < min_shoot_speed)
				{
					num4 = min_shoot_speed;
				}
				float num5 = 0f;
				float num6 = 0f;
				num4 += troop.data->rand.NextFloat() * arrow.salvo->def->shoot_speed_randomization_mod * num4;
				if (num3 >= 0f)
				{
					float num7 = num4 * num4;
					float num8 = Mathf.Sqrt(num7 * num7 - 2f * num7 * -num3 * gravity - gravity * gravity * num2 * num2);
					float num9 = Mathf.Atan((num7 + num8) / (gravity * num2));
					bool flag = arrow.is_high_angle && num9 > -1.5707964f && num9 < 1.5707964f;
					if (arrow.is_high_angle && flag)
					{
						f = num9;
						flag = true;
					}
					if (!arrow.is_high_angle && !flag)
					{
						float num10 = Mathf.Atan((num7 - num8) / (gravity * num2));
						flag = (num10 > -1.5707964f && num10 < 1.5707964f);
						if (flag)
						{
							f = num10;
							flag = true;
						}
					}
					if (!flag)
					{
						f = 0.7853982f;
						num4 = Mathf.Sqrt(num2 * gravity);
					}
					num5 = Mathf.Cos(f) * num4;
					num6 = Mathf.Sin(f) * num4;
					num = num2 / num5;
				}
				else
				{
					num3 *= -1f;
					float num11 = num4 * num4;
					float num12 = Mathf.Sqrt(num11 * num11 - 2f * num11 * -num3 * gravity - gravity * gravity * num2 * num2);
					float num13 = Mathf.Atan((num11 + num12) / (gravity * num2));
					bool flag2 = arrow.is_high_angle && num13 > -1.5707964f && num13 < 1.5707964f;
					if (arrow.is_high_angle && flag2)
					{
						f = num13;
						num5 = Mathf.Cos(f) * num4;
						num = num2 / num5;
						num6 = -(Mathf.Sin(f) * num4 - gravity * num);
						f = Mathf.Atan(num6 / num5);
						flag2 = true;
					}
					if (!arrow.is_high_angle && !flag2)
					{
						float num14 = Mathf.Atan((num11 - num12) / (gravity * num2));
						flag2 = (num14 > -1.5707964f && num14 < 1.5707964f);
						if (flag2)
						{
							f = num14;
							num5 = Mathf.Cos(f) * num4;
							num = num2 / num5;
							num6 = -(Mathf.Sin(f) * num4 - gravity * num);
							f = Mathf.Atan(num6 / num5);
						}
					}
				}
				arrow.startVelocity = new float2(num5, num6);
				float num15 = num6 / gravity;
				float y = num3 + num6 * num6 * 0.5f / gravity;
				float2 float2 = math.lerp(new float2(pos3d.x, pos3d.z), new float2(endPos.x, endPos.z), num15 / num);
				arrow.midPoint = new float3(float2.x, y, float2.y);
				arrow.duration = num;
				arrow.ClrFlags(Troops.Arrow.Flags.BeingShot);
				arrow.SetFlags(Troops.Arrow.Flags.Moving);
			}
			float dt = arrow.data->dt;
			if (arrow.t < 0f)
			{
				arrow.scale = 0f;
				arrow.t += dt;
				return;
			}
			arrow.t += dt;
			if (arrow.t < arrow.duration)
			{
				arrow.scale = 1f;
			}
			float num16 = arrow.duration - 0.1f;
			if ((arrow.t < num16 && arrow.t + dt >= num16) || (arrow.t > num16 && !arrow.salvo->HasFlags(Troops.SalvoData.Flags.HasFinishedActionFrameArrow)))
			{
				arrow.salvo->SetFlags(Troops.SalvoData.Flags.HasFinishedActionFrameArrow);
				TempList<int> tempList = new TempList<int>(false, 0);
				float projectile_radius = arrow.salvo->def->projectile_radius;
				float friendly_fire_mod = arrow.salvo->friendly_fire_mod;
				if (this.collisions.EnumTroops(arrow.endPos.xz, projectile_radius, ref tempList, -1, true) > 0)
				{
					for (int i = 0; i < tempList.Length; i++)
					{
						Troops.Troop troop2 = arrow.data->GetTroop(arrow.cur_thread_id, tempList[i]);
						bool flag3 = troop2.squad->battle_side == arrow.salvo->battle_side;
						if (arrow.salvo->squad != null && !flag3)
						{
							troop2.range_enemy_squad_id = arrow.salvo->squad->id;
						}
						float num17 = flag3 ? friendly_fire_mod : 1f;
						float defense_against_ranged_final = troop2.squad->defense_against_ranged_final;
						float num18;
						if (arrow.salvo->squad != null && arrow.salvo->squad->def->is_siege_eq && troop2.def->is_siege_eq)
						{
							num18 = arrow.salvo->squad->def->CTH_siege_vs_siege;
						}
						else
						{
							num18 = arrow.salvo->CTH_final;
							if (troop2.def->is_cavalry)
							{
								num18 *= arrow.salvo->CTH_cavalry_mod;
							}
						}
						num18 *= 1f + troop2.squad->CTH_against_me_mod / 100f;
						if (troop2.squad->def->max_health > 0f)
						{
							troop2.GetHit(num18 / (100f + defense_against_ranged_final));
						}
						else if (arrow.data->rand.NextFloat() < num17 * num18 / (100f + defense_against_ranged_final) && !troop2.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
						{
							troop2.Kill();
							if (arrow.salvo->def->explosion_force > 0f)
							{
								float2 direction = troop2.pos3d.xz - arrow.pos.xz;
								troop2.Throw(direction, arrow.salvo->def->explosion_force);
							}
							if (!arrow.salvo->def->splash_damage)
							{
								break;
							}
						}
					}
				}
				tempList.Clear();
				if (arrow.salvo->def->can_hit_fortification && this.fort_collisions.EnumTroops(arrow.endPos.xz, projectile_radius, ref tempList, -1, true) > 0)
				{
					for (int j = 0; j < tempList.Length; j++)
					{
						Troops.FortificationData* fortification = arrow.data->GetFortification(tempList[j]);
						if (!fortification->HasFlags(Troops.FortificationData.Flags.Hit | Troops.FortificationData.Flags.Demolished))
						{
							fortification->SetFlags(Troops.FortificationData.Flags.Hit);
							float cth_final = arrow.salvo->CTH_final;
							fortification->health -= cth_final;
						}
					}
				}
				tempList.Dispose();
			}
			float2 float3 = arrow.startPos.xz + arrow.startVelocity.x * arrow.t * math.normalize(arrow.endPos.xz - arrow.startPos.xz);
			float3 float4 = new float3(float3.x, arrow.startPos.y + arrow.startVelocity.y * arrow.t - arrow.t * arrow.t * gravity * 0.5f, float3.y);
			float3 float5 = math.normalize(float4 - arrow.pos);
			if (math.length(float5) > 0.01f)
			{
				quaternion quaternion = Quaternion.LookRotation(float5);
				arrow.rot = quaternion.value;
			}
			arrow.pos = float4;
			if (arrow.t >= arrow.duration)
			{
				arrow.ClrFlags(Troops.Arrow.Flags.Moving);
				arrow.SetFlags(Troops.Arrow.Flags.Landed);
				arrow.salvo->SetFlags(Troops.SalvoData.Flags.HasLandedArrow);
				if (arrow.target_paid > 0 && !this.pointers.PointInArea(arrow.pos.xz, arrow.target_paid, PathData.PassableArea.Type.All))
				{
					arrow.SetFlags(Troops.Arrow.Flags.FloatingInAir);
				}
			}
		}

		// Token: 0x040031C4 RID: 12740
		public TroopCollisions.ReadOnly collisions;

		// Token: 0x040031C5 RID: 12741
		public TroopCollisions.ReadOnly fort_collisions;

		// Token: 0x040031C6 RID: 12742
		public PathData.DataPointers pointers;
	}

	// Token: 0x020005D6 RID: 1494
	public struct ArrowTrailsJob : Troops.IArrowJob
	{
		// Token: 0x06004541 RID: 17729 RVA: 0x0020A970 File Offset: 0x00208B70
		public unsafe void Execute(Troops.Arrow arrow)
		{
			if (!arrow.salvo->def->draw_trail && !arrow.salvo->def->draw_after_landing)
			{
				if (arrow.HasFlags(Troops.Arrow.Flags.Landed))
				{
					arrow.SetFlags(Troops.Arrow.Flags.Destroyed);
				}
				return;
			}
			float dt = arrow.data->dt;
			if (arrow.HasFlags(Troops.Arrow.Flags.Moving))
			{
				arrow.trail_progress = arrow.t;
				return;
			}
			if (arrow.HasFlags(Troops.Arrow.Flags.Landed))
			{
				arrow.trail_progress += dt;
				if (arrow.trail_progress > 10f)
				{
					arrow.SetFlags(Troops.Arrow.Flags.Destroyed);
					return;
				}
			}
			else
			{
				arrow.trail_progress = 0f;
			}
		}
	}

	// Token: 0x020005D7 RID: 1495
	public struct ChargeJob : Troops.ITroopJob
	{
		// Token: 0x06004542 RID: 17730 RVA: 0x0020AA18 File Offset: 0x00208C18
		public unsafe void Execute(Troops.Troop troop)
		{
			if (!troop.squad->is_Fighting && !troop.face_enemies)
			{
				troop.SetFlags(Troops.Troop.Flags.Trampling);
				troop.slow_down_cd = 0f;
			}
			TempList<int> tempList = new TempList<int>(false, 0);
			float num = troop.def->attack_range + troop.def->radius + 0.5f;
			float num2 = num * num;
			if (this.collisions.EnumTroops(troop.pos, num, ref tempList, 1 - troop.squad->battle_side, true) > 0)
			{
				float2 @float = math.normalize(troop.vel_spd_t.xy);
				bool flag = false;
				for (int i = 0; i < tempList.Length; i++)
				{
					Troops.Troop troop2 = troop.data->GetTroop(troop.cur_thread_id, tempList[i]);
					float2 x = troop2.pos - troop.pos;
					float num3 = math.dot(@float, math.normalize(x));
					if (math.lengthsq(x) <= num2 && !troop2.def->is_cavalry && !troop2.def->is_siege_eq && !troop2.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && num3 >= 0f)
					{
						if (num3 > 0.7f && troop.def->is_cavalry && troop.HasFlags(Troops.Troop.Flags.Trampling))
						{
							float num4 = 1f;
							float num5 = math.dot(new PPos(0f, 1f, 0).GetRotated(troop2.rot_y), @float);
							float num6 = 1f + troop2.squad->CTH_against_me_mod / 100f;
							float defense_against_trample_mod = troop2.squad->defense_against_trample_mod;
							if (num5 > 0f)
							{
								num4 += num5;
							}
							float num7 = troop2.squad->avoid_trample_tight_formation_mod * math.clamp(troop.squad->trample_chance_final * troop.squad->CTH_final * num4 * num6 / (100f + troop2.squad->defense_final * defense_against_trample_mod), 0.05f, 0.95f);
							if (troop.data->rand.NextFloat(1f) < num7)
							{
								troop2.Throw(math.float2(troop2.pos - troop.pos), troop.vel_spd_t.z);
								troop2.Kill();
							}
							else
							{
								troop.ClrFlags(Troops.Troop.Flags.Trampling);
								if (troop2.def->is_spearman && math.dot(new PPos(0f, 1f, 0).GetRotated(troop2.rot_y), @float) <= -0.5f)
								{
									num7 = math.clamp(troop2.def->CTH_cavalry_mod * troop2.squad->CTH_final * troop2.squad->CTH_against_cav_charge_mod / (100f + troop.squad->defense_final), 0.05f, 0.95f);
									if (troop.data->rand.NextFloat(1f) < num7)
									{
										troop.Kill();
										troop.Throw(math.float2(troop2.pos - troop.pos), troop.vel_spd_t.z * 0.2f);
										return;
									}
								}
							}
							this.TryShock(troop.squad, troop2.squad, true);
						}
						if (!troop.HasFlags(Troops.Troop.Flags.Trampling))
						{
							flag = true;
							float num8 = 1f;
							troop.slow_down_cd = math.clamp(troop.slow_down_cd + troop.data->dt, 0f, num8) / num8;
							float num9 = 1f;
							if (troop.def->is_heavy)
							{
								num9 *= 0.25f;
							}
							if (troop2.def->is_heavy)
							{
								num9 *= 2f;
							}
							if (troop2.def->is_spearman && troop.def->is_cavalry)
							{
								num9 *= 2f;
							}
							if (troop.squad->wedgeFormation)
							{
								num9 *= 0.5f;
							}
							num9 = math.clamp(num9, 1f, 4f);
							float num10 = troop.vel_spd_t.z - num9 * troop.data->dt;
							float num11 = troop.squad->move_speed * troop.def->max_speed_mul;
							num11 *= 1f - num9 * 0.125f * troop.slow_down_cd;
							num10 = math.clamp(num10, 0f, num11);
							troop.vel_spd_t = new float4(troop.vel_spd_t.xy, num10, troop.vel_spd_t.w);
						}
					}
				}
				if (flag)
				{
					troop.ClrFlags(Troops.Troop.Flags.Trampling);
					troop.face_enemies = true;
					return;
				}
			}
			else
			{
				troop.face_enemies = false;
			}
		}

		// Token: 0x06004543 RID: 17731 RVA: 0x0020AF3C File Offset: 0x0020913C
		private unsafe void TryShock(Troops.SquadData* squad_data, Troops.SquadData* enemy_data, bool is_trample = false)
		{
			float num = squad_data->shock_chance;
			float resiliance_total = enemy_data->resiliance_total;
			if (resiliance_total < 0f)
			{
				num -= resiliance_total;
			}
			Unity.Mathematics.Random rand = squad_data->pdata->rand;
			if (rand.NextFloat(100f) < num)
			{
				float num2 = squad_data->shock_damage_base;
				if (is_trample)
				{
					num2 += squad_data->shock_damage_bonus_trample;
				}
				enemy_data->shock_morale_dmg_acc = enemy_data->shock_morale_dmg_acc + num2 / (float)enemy_data->logic_alive;
			}
		}

		// Token: 0x040031C7 RID: 12743
		public TroopCollisions.ReadOnly collisions;
	}

	// Token: 0x020005D8 RID: 1496
	public struct PushOffJob : Troops.ITroopJob
	{
		// Token: 0x06004544 RID: 17732 RVA: 0x0020AFA4 File Offset: 0x002091A4
		public unsafe void Execute(Troops.Troop troop)
		{
			if (troop.HasFlags(Troops.Troop.Flags.Fighting))
			{
				return;
			}
			TempList<int> tempList = new TempList<int>(false, 0);
			if (this.collisions.EnumTroops(troop.pos, troop.def->radius, ref tempList, 1 - troop.squad->battle_side, false) > 0)
			{
				for (int i = 0; i < tempList.Length; i++)
				{
					Troops.Troop troop2 = troop.data->GetTroop(troop.cur_thread_id, tempList[i]);
					float2 @float = troop2.pos - troop.pos;
					if (!troop2.def->is_cavalry && !troop2.def->is_siege_eq && !troop2.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
					{
						float num = troop2.def->defense * 0.01f;
						num = math.clamp(num, 0.05f, 0.95f);
						if (troop.data->rand.NextFloat() >= num)
						{
							float2 float2 = @float;
							if (float2.x == 0f && float2.y == 0f)
							{
								return;
							}
							float rhs = troop.def->is_heavy ? 1.5f : 1f;
							float rhs2 = troop.def->is_cavalry ? 1.5f : 1f;
							bool wedgeFormation = troop.squad->wedgeFormation;
							troop2.PushOff(math.normalize(float2) * rhs * rhs2, 2f);
						}
					}
				}
			}
		}

		// Token: 0x040031C8 RID: 12744
		public TroopCollisions.ReadOnly collisions;
	}

	// Token: 0x020005D9 RID: 1497
	public struct DustParticlesJob : Troops.ITroopJob
	{
		// Token: 0x06004545 RID: 17733 RVA: 0x0020B144 File Offset: 0x00209344
		public unsafe void Execute(Troops.Troop troop)
		{
			ref float4 vel_spd_t = troop.vel_spd_t;
			float dt = troop.data->dt;
			float run_anim_speed = troop.def->run_anim_speed;
			bool flag = vel_spd_t.z > run_anim_speed && troop.squad->command == Logic.Squad.Command.Charge;
			float num = 4f;
			for (int i = 0; i < 5; i++)
			{
				int num2 = troop.id * 5 + i;
				float4 @float = troop.data->dust_particle_positions[num2];
				bool flag2 = false;
				if (flag)
				{
					if (@float.w < -num)
					{
						@float.w += num;
					}
					if (@float.w <= 0f && @float.w + dt > 0f)
					{
						float3 pos3d = troop.pos3d;
						float radius = troop.def->radius;
						@float.xyz = pos3d + new float3(troop.data->rand.NextFloat(-radius, radius), 0f, troop.data->rand.NextFloat(-radius, radius));
					}
					flag2 = true;
				}
				else if (@float.w > 0f)
				{
					flag2 = true;
				}
				if (flag2)
				{
					@float.y += dt;
					@float.w += dt;
					if (@float.w > num)
					{
						@float.w = -num - troop.data->rand.NextFloat(0f, num);
					}
				}
				troop.data->dust_particle_positions[num2] = @float;
			}
		}
	}

	// Token: 0x020005DA RID: 1498
	public interface ISquadJob
	{
		// Token: 0x06004546 RID: 17734
		void Execute(ref Troops.SquadData squad);
	}

	// Token: 0x020005DB RID: 1499
	public interface ITroopJob
	{
		// Token: 0x06004547 RID: 17735
		void Execute(Troops.Troop troop);
	}

	// Token: 0x020005DC RID: 1500
	public interface IArrowJob
	{
		// Token: 0x06004548 RID: 17736
		void Execute(Troops.Arrow arrow);
	}

	// Token: 0x020005DD RID: 1501
	public interface ISalvoJob
	{
		// Token: 0x06004549 RID: 17737
		void Execute(ref Troops.SalvoData salvo);
	}

	// Token: 0x020005DE RID: 1502
	// (Invoke) Token: 0x0600454B RID: 17739
	public delegate void SquadAction(global::Squad squad);

	// Token: 0x020005DF RID: 1503
	// (Invoke) Token: 0x0600454F RID: 17743
	public delegate void SalvoAction(Troops.Salvo salvo);

	// Token: 0x020005E0 RID: 1504
	[DebuggerDisplay("[{DebugText}]")]
	public struct Troop
	{
		// Token: 0x06004552 RID: 17746 RVA: 0x0020B2E2 File Offset: 0x002094E2
		public unsafe Troop(Troops.TroopsPtrData* data, int cur_thread_id, int troop_id)
		{
			this.data = data;
			this.cur_thread_id = cur_thread_id;
			this.id = troop_id;
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06004553 RID: 17747 RVA: 0x0020B2F9 File Offset: 0x002094F9
		public bool valid
		{
			get
			{
				return this.data != null && this.id >= 0 && !this.HasFlags(Troops.Troop.Flags.Destroyed);
			}
		}

		// Token: 0x06004554 RID: 17748 RVA: 0x0020B320 File Offset: 0x00209520
		public void SetDrawerInfo(global::Squad squad_visual, bool reset_model)
		{
			Troops.SquadModelData model_data = squad_visual.model_data;
			if (reset_model)
			{
				this.drawer_model_id = UnityEngine.Random.Range(0, model_data.per_model_data.Count);
				TextureBaker.PerModelData perModelData = model_data.per_model_data[this.drawer_model_id];
				this.drawer_anim_id = perModelData.baked_data_id;
				this.attack_cd = 0f;
			}
			this.drawer_object_id = squad_visual.GetID();
			if (model_data != null)
			{
				this.model_data_buffer = model_data.per_model_data[this.drawer_model_id].model_data_buffer;
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06004555 RID: 17749 RVA: 0x0020B3A2 File Offset: 0x002095A2
		public unsafe Troops.SquadData* squad
		{
			get
			{
				return this.data->squad[this.id].ptr;
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06004556 RID: 17750 RVA: 0x0020B3C3 File Offset: 0x002095C3
		public unsafe int thread_id
		{
			get
			{
				return this.squad->thread_id;
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06004557 RID: 17751 RVA: 0x0020B3D0 File Offset: 0x002095D0
		public unsafe Troops.DefData* def
		{
			get
			{
				return this.squad->def;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06004558 RID: 17752 RVA: 0x0020B3DD File Offset: 0x002095DD
		public unsafe int squad_id
		{
			get
			{
				return this.squad->id;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06004559 RID: 17753 RVA: 0x0020B3EA File Offset: 0x002095EA
		public unsafe int type
		{
			get
			{
				return this.def->type;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x0600455A RID: 17754 RVA: 0x0020B3F7 File Offset: 0x002095F7
		private unsafe int* mt_flags
		{
			get
			{
				return this.data->flags + this.id;
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x0600455B RID: 17755 RVA: 0x0020B40E File Offset: 0x0020960E
		public unsafe Troops.Troop.Flags flags
		{
			get
			{
				return (Troops.Troop.Flags)(*this.mt_flags);
			}
		}

		// Token: 0x0600455C RID: 17756 RVA: 0x0020B417 File Offset: 0x00209617
		public unsafe bool HasFlags(Troops.Troop.Flags mask)
		{
			return Troops.MTFlags.get(*this.mt_flags, (int)mask);
		}

		// Token: 0x0600455D RID: 17757 RVA: 0x0020B426 File Offset: 0x00209626
		public unsafe void SetFlags(Troops.Troop.Flags mask)
		{
			Troops.MTFlags.set(ref *this.mt_flags, (int)mask);
		}

		// Token: 0x0600455E RID: 17758 RVA: 0x0020B434 File Offset: 0x00209634
		public unsafe void ClrFlags(Troops.Troop.Flags mask)
		{
			Troops.MTFlags.clear(ref *this.mt_flags, (int)mask);
		}

		// Token: 0x0600455F RID: 17759 RVA: 0x0020B442 File Offset: 0x00209642
		public unsafe void SetFlags(Troops.Troop.Flags mask, bool set)
		{
			if (set)
			{
				Troops.MTFlags.set(ref *this.mt_flags, (int)mask);
				return;
			}
			Troops.MTFlags.clear(ref *this.mt_flags, (int)mask);
		}

		// Token: 0x06004560 RID: 17760 RVA: 0x0020B460 File Offset: 0x00209660
		public void Throw()
		{
			this.Throw(new float2(0f, 0f), 1f);
		}

		// Token: 0x06004561 RID: 17761 RVA: 0x0020B47C File Offset: 0x0020967C
		public unsafe void Throw(float2 direction, float force = 1f)
		{
			this.thrown_args = new float4(this.pos3d, 0f);
			if (direction.Equals(new float2(0f, 0f)))
			{
				this.thrown_dir = math.normalize(new float2(this.data->rand.NextFloat(-1f, 1f), this.data->rand.NextFloat(-1f, 1f))) * force;
			}
			else
			{
				this.thrown_dir = math.normalize(direction) * force;
			}
			this.SetFlags(Troops.Troop.Flags.Thrown);
		}

		// Token: 0x06004562 RID: 17762 RVA: 0x0020B520 File Offset: 0x00209720
		public unsafe void Destroy()
		{
			float num = 4f;
			for (int i = 0; i < 5; i++)
			{
				int num2 = this.id * 5 + i;
				float4 @float = this.data->dust_particle_positions[num2];
				@float.xyz = new float3(0f, -1000f, 0f);
				@float.w = -num - this.data->rand.NextFloat(0f, num);
				this.data->dust_particle_positions[num2] = @float;
			}
			this.SetFlags(Troops.Troop.Flags.Destroyed);
			this.pos3d = new float3(0f, -1000f, 0f);
		}

		// Token: 0x06004563 RID: 17763 RVA: 0x0020B5E8 File Offset: 0x002097E8
		public unsafe void PushOff(float2 direction, float duration)
		{
			if (duration > 2f)
			{
				duration = 2f;
			}
			else if (duration < 0f)
			{
				duration = 0f;
			}
			float rhs = this.def->is_heavy ? 0.67f : 1f;
			this.pushOff_args = new float3(direction * 0.75f * rhs, 2f - duration);
			this.SetFlags(Troops.Troop.Flags.PushedOff);
		}

		// Token: 0x06004564 RID: 17764 RVA: 0x0020B65D File Offset: 0x0020985D
		public unsafe void Kill()
		{
			this.SetFlags(Troops.Troop.Flags.Killed);
			this.squad->SetFlags(Troops.SquadData.Flags.HasKilledTroops);
		}

		// Token: 0x06004565 RID: 17765 RVA: 0x0020B67A File Offset: 0x0020987A
		public unsafe void GetHit(float damage)
		{
			this.squad->health = math.max(0f, this.squad->health - damage);
			if (this.squad->health <= 0f)
			{
				this.Kill();
			}
		}

		// Token: 0x06004566 RID: 17766 RVA: 0x0020B6B6 File Offset: 0x002098B6
		public unsafe void ActivateSquad()
		{
			this.squad->SetFlags(Troops.SquadData.Flags.HasActiveTroops);
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06004567 RID: 17767 RVA: 0x0020B6C8 File Offset: 0x002098C8
		// (set) Token: 0x06004568 RID: 17768 RVA: 0x0020B6E9 File Offset: 0x002098E9
		public unsafe float3 pos3d
		{
			get
			{
				return this.data->pos_rot[this.id].xyz;
			}
			set
			{
				this.data->pos_rot[this.id] = new float4(value, this.rot_y);
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06004569 RID: 17769 RVA: 0x0020B716 File Offset: 0x00209916
		// (set) Token: 0x0600456A RID: 17770 RVA: 0x0020B737 File Offset: 0x00209937
		public unsafe float3 previous_pos3d
		{
			get
			{
				return this.data->prev_pos_rot[this.id].xyz;
			}
			set
			{
				this.data->prev_pos_rot[this.id] = new float4(value, this.rot_y);
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x0600456B RID: 17771 RVA: 0x0020B764 File Offset: 0x00209964
		// (set) Token: 0x0600456C RID: 17772 RVA: 0x0020B785 File Offset: 0x00209985
		public unsafe float3 rot3d
		{
			get
			{
				return this.data->rot_3d[this.id];
			}
			set
			{
				this.data->rot_3d[this.id] = value;
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600456D RID: 17773 RVA: 0x0020B7A7 File Offset: 0x002099A7
		// (set) Token: 0x0600456E RID: 17774 RVA: 0x0020B7C8 File Offset: 0x002099C8
		public unsafe float3 preview_position
		{
			get
			{
				return this.data->previewPositions[this.id];
			}
			set
			{
				this.data->previewPositions[this.id] = value;
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600456F RID: 17775 RVA: 0x0020B7EA File Offset: 0x002099EA
		// (set) Token: 0x06004570 RID: 17776 RVA: 0x0020B80B File Offset: 0x00209A0B
		public unsafe float4 thrown_args
		{
			get
			{
				return this.data->throw_time[this.id];
			}
			set
			{
				this.data->throw_time[this.id] = value;
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06004571 RID: 17777 RVA: 0x0020B82D File Offset: 0x00209A2D
		// (set) Token: 0x06004572 RID: 17778 RVA: 0x0020B84E File Offset: 0x00209A4E
		public unsafe float2 thrown_dir
		{
			get
			{
				return this.data->thrown_dir[this.id];
			}
			set
			{
				this.data->thrown_dir[this.id] = value;
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06004573 RID: 17779 RVA: 0x0020B870 File Offset: 0x00209A70
		// (set) Token: 0x06004574 RID: 17780 RVA: 0x0020B880 File Offset: 0x00209A80
		public float thrown_time
		{
			get
			{
				return this.thrown_args.w;
			}
			set
			{
				this.thrown_args = new float4(this.thrown_args.xyz, value);
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06004575 RID: 17781 RVA: 0x0020B8A7 File Offset: 0x00209AA7
		// (set) Token: 0x06004576 RID: 17782 RVA: 0x0020B8C8 File Offset: 0x00209AC8
		public unsafe float3 pushOff_args
		{
			get
			{
				return this.data->pushOff_args[this.id];
			}
			set
			{
				this.data->pushOff_args[this.id] = value;
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06004577 RID: 17783 RVA: 0x0020B8EA File Offset: 0x00209AEA
		// (set) Token: 0x06004578 RID: 17784 RVA: 0x0020B8F8 File Offset: 0x00209AF8
		public float pushOff_time
		{
			get
			{
				return this.pushOff_args.z;
			}
			set
			{
				this.pushOff_args = new float3(this.pushOff_args.xy, value);
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06004579 RID: 17785 RVA: 0x0020B920 File Offset: 0x00209B20
		// (set) Token: 0x0600457A RID: 17786 RVA: 0x0020B93B File Offset: 0x00209B3B
		public float3 thrown_pos
		{
			get
			{
				return this.thrown_args.xyz;
			}
			set
			{
				this.thrown_args = new float4(value, this.thrown_time);
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x0600457B RID: 17787 RVA: 0x0020B94F File Offset: 0x00209B4F
		// (set) Token: 0x0600457C RID: 17788 RVA: 0x0020B970 File Offset: 0x00209B70
		public unsafe int pa_id
		{
			get
			{
				return this.data->pa_id[this.id].x;
			}
			set
			{
				this.data->pa_id[this.id].x = value;
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x0600457D RID: 17789 RVA: 0x0020B992 File Offset: 0x00209B92
		// (set) Token: 0x0600457E RID: 17790 RVA: 0x0020B9B3 File Offset: 0x00209BB3
		public unsafe int tgt_pa_id
		{
			get
			{
				return this.data->pa_id[this.id].y;
			}
			set
			{
				this.data->pa_id[this.id].y = value;
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x0600457F RID: 17791 RVA: 0x0020B9D5 File Offset: 0x00209BD5
		// (set) Token: 0x06004580 RID: 17792 RVA: 0x0020B9ED File Offset: 0x00209BED
		public unsafe int stuck_tgt_pa_id
		{
			get
			{
				return this.data->stuck_tgt_pa_id[this.id];
			}
			set
			{
				this.data->stuck_tgt_pa_id[this.id] = value;
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06004581 RID: 17793 RVA: 0x0020BA06 File Offset: 0x00209C06
		// (set) Token: 0x06004582 RID: 17794 RVA: 0x0020BA37 File Offset: 0x00209C37
		public unsafe PPos pos
		{
			get
			{
				return new PPos(this.data->pos_rot[this.id].xz, this.pa_id);
			}
			set
			{
				this.data->pos_rot[this.id].xz = new float2(value.x, value.y);
				this.pa_id = value.paID;
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06004583 RID: 17795 RVA: 0x0020BA78 File Offset: 0x00209C78
		// (set) Token: 0x06004584 RID: 17796 RVA: 0x0020BAA6 File Offset: 0x00209CA6
		public float2 pos2d
		{
			get
			{
				return new float2(this.pos.x, this.pos.y);
			}
			set
			{
				this.pos = new PPos(value.x, value.y, this.pos.paID);
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06004585 RID: 17797 RVA: 0x0020BACA File Offset: 0x00209CCA
		// (set) Token: 0x06004586 RID: 17798 RVA: 0x0020BAEB File Offset: 0x00209CEB
		public unsafe float rot_y
		{
			get
			{
				return this.data->pos_rot[this.id].w;
			}
			set
			{
				this.data->pos_rot[this.id] = new float4(this.pos3d, value);
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06004587 RID: 17799 RVA: 0x0020BB18 File Offset: 0x00209D18
		public float rot_within_bounds
		{
			get
			{
				float num = this.rot_y % 360f;
				if (num < 0f)
				{
					num += 360f;
				}
				return num;
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06004588 RID: 17800 RVA: 0x0020BB43 File Offset: 0x00209D43
		// (set) Token: 0x06004589 RID: 17801 RVA: 0x0020BB5B File Offset: 0x00209D5B
		public unsafe float tgt_arrow_rot
		{
			get
			{
				return this.data->tgt_arrow_rot[this.id];
			}
			set
			{
				this.data->tgt_arrow_rot[this.id] = value;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x0600458A RID: 17802 RVA: 0x0020BB74 File Offset: 0x00209D74
		// (set) Token: 0x0600458B RID: 17803 RVA: 0x0020BB98 File Offset: 0x00209D98
		public unsafe float4 vel_spd_t
		{
			get
			{
				return this.data->vel_spd_t[this.id];
			}
			set
			{
				if (!float.IsNaN(value.x) && !float.IsNaN(value.y) && !float.IsNaN(value.z))
				{
					float.IsNaN(value.w);
				}
				this.data->vel_spd_t[this.id] = value;
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x0600458C RID: 17804 RVA: 0x0020BBF8 File Offset: 0x00209DF8
		// (set) Token: 0x0600458D RID: 17805 RVA: 0x0020BC05 File Offset: 0x00209E05
		public unsafe float move_speed
		{
			get
			{
				return this.vel_spd_t.z;
			}
			set
			{
				this.data->vel_spd_t[this.id].z = value;
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x0600458E RID: 17806 RVA: 0x0020BC27 File Offset: 0x00209E27
		// (set) Token: 0x0600458F RID: 17807 RVA: 0x0020BC48 File Offset: 0x00209E48
		public unsafe float4 steer
		{
			get
			{
				return this.data->steer[this.id];
			}
			set
			{
				this.data->steer[this.id] = value;
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06004590 RID: 17808 RVA: 0x0020BC6A File Offset: 0x00209E6A
		// (set) Token: 0x06004591 RID: 17809 RVA: 0x0020BC8B File Offset: 0x00209E8B
		public unsafe float4 separation
		{
			get
			{
				return this.data->separation[this.id];
			}
			set
			{
				this.data->separation[this.id] = value;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06004592 RID: 17810 RVA: 0x0020BCAD File Offset: 0x00209EAD
		// (set) Token: 0x06004593 RID: 17811 RVA: 0x0020BCCE File Offset: 0x00209ECE
		public unsafe float2 hold_formation_vec
		{
			get
			{
				return this.data->holdFormationVec[this.id];
			}
			set
			{
				this.data->holdFormationVec[this.id] = value;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06004594 RID: 17812 RVA: 0x0020BCF0 File Offset: 0x00209EF0
		// (set) Token: 0x06004595 RID: 17813 RVA: 0x0020BD08 File Offset: 0x00209F08
		public unsafe int form_idx
		{
			get
			{
				return this.data->form_idx[this.id];
			}
			set
			{
				this.data->form_idx[this.id] = value;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06004596 RID: 17814 RVA: 0x0020BD21 File Offset: 0x00209F21
		// (set) Token: 0x06004597 RID: 17815 RVA: 0x0020BD42 File Offset: 0x00209F42
		public unsafe float2 pos_in_formation
		{
			get
			{
				return this.data->posInFormation[this.id];
			}
			set
			{
				this.data->posInFormation[this.id] = value;
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06004598 RID: 17816 RVA: 0x0020BD64 File Offset: 0x00209F64
		// (set) Token: 0x06004599 RID: 17817 RVA: 0x0020BD85 File Offset: 0x00209F85
		public unsafe float2 deform
		{
			get
			{
				return this.data->deform[this.id];
			}
			set
			{
				this.data->deform[this.id] = value;
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x0600459A RID: 17818 RVA: 0x0020BDA7 File Offset: 0x00209FA7
		// (set) Token: 0x0600459B RID: 17819 RVA: 0x0020BDBF File Offset: 0x00209FBF
		public unsafe float climb_progress
		{
			get
			{
				return this.data->climb_progress[this.id];
			}
			set
			{
				this.data->climb_progress[this.id] = value;
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x0600459C RID: 17820 RVA: 0x0020BDD8 File Offset: 0x00209FD8
		// (set) Token: 0x0600459D RID: 17821 RVA: 0x0020BE2E File Offset: 0x0020A02E
		public unsafe PPos tgt_pos
		{
			get
			{
				return new PPos(this.data->tgt_pos[this.id].x, this.data->tgt_pos[this.id].y, this.tgt_pa_id);
			}
			set
			{
				this.data->tgt_pos[this.id] = new float2(value.x, value.y);
				this.tgt_pa_id = value.paID;
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600459E RID: 17822 RVA: 0x0020BE70 File Offset: 0x0020A070
		// (set) Token: 0x0600459F RID: 17823 RVA: 0x0020BEC6 File Offset: 0x0020A0C6
		public unsafe PPos stuck_tgt_pos
		{
			get
			{
				return new PPos(this.data->stuck_tgt_pos[this.id].x, this.data->stuck_tgt_pos[this.id].y, this.stuck_tgt_pa_id);
			}
			set
			{
				this.data->stuck_tgt_pos[this.id] = new float2(value.x, value.y);
				this.stuck_tgt_pa_id = value.paID;
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x060045A0 RID: 17824 RVA: 0x0020BF06 File Offset: 0x0020A106
		// (set) Token: 0x060045A1 RID: 17825 RVA: 0x0020BF1E File Offset: 0x0020A11E
		public unsafe float cur_speed
		{
			get
			{
				return this.data->cur_speed[this.id];
			}
			set
			{
				this.data->cur_speed[this.id] = value;
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x060045A2 RID: 17826 RVA: 0x0020BF37 File Offset: 0x0020A137
		// (set) Token: 0x060045A3 RID: 17827 RVA: 0x0020BF58 File Offset: 0x0020A158
		public unsafe int2 drawer_id
		{
			get
			{
				return this.data->drawer_ids[this.id];
			}
			set
			{
				this.data->drawer_ids[this.id] = value;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x060045A4 RID: 17828 RVA: 0x0020BF7A File Offset: 0x0020A17A
		// (set) Token: 0x060045A5 RID: 17829 RVA: 0x0020BF87 File Offset: 0x0020A187
		public int drawer_object_id
		{
			get
			{
				return this.drawer_id.x;
			}
			set
			{
				this.drawer_id = new int2(value, this.drawer_id.y);
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x060045A6 RID: 17830 RVA: 0x0020BFA0 File Offset: 0x0020A1A0
		// (set) Token: 0x060045A7 RID: 17831 RVA: 0x0020BFC2 File Offset: 0x0020A1C2
		public unsafe GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData> model_data_buffer
		{
			get
			{
				return new GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>(*(IntPtr*)(this.data->model_data_buffer + (IntPtr)this.id * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)));
			}
			set
			{
				*(IntPtr*)(this.data->model_data_buffer + (IntPtr)this.id * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) = value.data;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x060045A8 RID: 17832 RVA: 0x0020BFE5 File Offset: 0x0020A1E5
		public unsafe GrowBuffer<TextureBaker.InstancedSelectionDrawerBatched.DrawCallData> selection_data_buffer
		{
			get
			{
				return new GrowBuffer<TextureBaker.InstancedSelectionDrawerBatched.DrawCallData>(this.def->is_siege_eq ? this.data->selection_circle_data_buffer : this.data->selection_arrow_data_buffer);
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x060045A9 RID: 17833 RVA: 0x0020C011 File Offset: 0x0020A211
		public unsafe GrowBuffer<TextureBaker.InstancedDustDrawerBatched.DrawCallData> dust_data_buffer
		{
			get
			{
				return new GrowBuffer<TextureBaker.InstancedDustDrawerBatched.DrawCallData>(this.data->dust_data_buffer);
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x060045AA RID: 17834 RVA: 0x0020C023 File Offset: 0x0020A223
		// (set) Token: 0x060045AB RID: 17835 RVA: 0x0020C030 File Offset: 0x0020A230
		public int drawer_model_id
		{
			get
			{
				return this.drawer_id.y;
			}
			set
			{
				this.drawer_id = new int2(this.drawer_id.x, value);
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x060045AC RID: 17836 RVA: 0x0020C049 File Offset: 0x0020A249
		// (set) Token: 0x060045AD RID: 17837 RVA: 0x0020C061 File Offset: 0x0020A261
		public unsafe int drawer_anim_id
		{
			get
			{
				return this.data->drawer_anim_ids[this.id];
			}
			set
			{
				this.data->drawer_anim_ids[this.id] = value;
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x060045AE RID: 17838 RVA: 0x0020C07A File Offset: 0x0020A27A
		// (set) Token: 0x060045AF RID: 17839 RVA: 0x0020C09B File Offset: 0x0020A29B
		public unsafe int2 anim_idx
		{
			get
			{
				return this.data->anim_idx[this.id];
			}
			set
			{
				this.data->anim_idx[this.id] = value;
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x060045B0 RID: 17840 RVA: 0x0020C0BD File Offset: 0x0020A2BD
		// (set) Token: 0x060045B1 RID: 17841 RVA: 0x0020C0DE File Offset: 0x0020A2DE
		public unsafe int cur_anim_idx
		{
			get
			{
				return this.data->anim_idx[this.id].x;
			}
			set
			{
				this.data->anim_idx[this.id] = new int2(value, this.prev_anim_idx);
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x060045B2 RID: 17842 RVA: 0x0020C10B File Offset: 0x0020A30B
		public UnitAnimation.State cur_anim_state
		{
			get
			{
				return UnitAnimation.IdxToState((UnitAnimation.Index)this.cur_anim_idx);
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x060045B3 RID: 17843 RVA: 0x0020C118 File Offset: 0x0020A318
		public UnitAnimation.State prev_anim_state
		{
			get
			{
				return UnitAnimation.IdxToState((UnitAnimation.Index)this.prev_anim_idx);
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x060045B4 RID: 17844 RVA: 0x0020C125 File Offset: 0x0020A325
		// (set) Token: 0x060045B5 RID: 17845 RVA: 0x0020C146 File Offset: 0x0020A346
		public unsafe int prev_anim_idx
		{
			get
			{
				return this.data->anim_idx[this.id].y;
			}
			set
			{
				this.data->anim_idx[this.id] = new int2(this.cur_anim_idx, value);
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x060045B6 RID: 17846 RVA: 0x0020C173 File Offset: 0x0020A373
		// (set) Token: 0x060045B7 RID: 17847 RVA: 0x0020C194 File Offset: 0x0020A394
		public unsafe float4 anim_time
		{
			get
			{
				return this.data->anim_time[this.id];
			}
			set
			{
				this.data->anim_time[this.id] = value;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x060045B8 RID: 17848 RVA: 0x0020C1B6 File Offset: 0x0020A3B6
		// (set) Token: 0x060045B9 RID: 17849 RVA: 0x0020C1D8 File Offset: 0x0020A3D8
		public unsafe float cur_anim_time
		{
			get
			{
				return this.data->anim_time[this.id].x;
			}
			set
			{
				this.data->anim_time[this.id] = new float4(value, this.anim_time.yzw);
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x060045BA RID: 17850 RVA: 0x0020C218 File Offset: 0x0020A418
		// (set) Token: 0x060045BB RID: 17851 RVA: 0x0020C23C File Offset: 0x0020A43C
		public unsafe float prev_anim_time
		{
			get
			{
				return this.data->anim_time[this.id].y;
			}
			set
			{
				this.data->anim_time[this.id] = new float4(this.anim_time.x, value, this.anim_time.zw);
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x060045BC RID: 17852 RVA: 0x0020C287 File Offset: 0x0020A487
		// (set) Token: 0x060045BD RID: 17853 RVA: 0x0020C2A8 File Offset: 0x0020A4A8
		public unsafe float blend_time
		{
			get
			{
				return this.data->anim_time[this.id].z;
			}
			set
			{
				this.data->anim_time[this.id] = new float4(this.anim_time.xy, value, this.anim_time.w);
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x060045BE RID: 17854 RVA: 0x0020C2F3 File Offset: 0x0020A4F3
		// (set) Token: 0x060045BF RID: 17855 RVA: 0x0020C314 File Offset: 0x0020A514
		public unsafe float cur_anim_speed
		{
			get
			{
				return this.data->anim_time[this.id].w;
			}
			set
			{
				this.data->anim_time[this.id] = new float4(this.anim_time.xyz, value);
			}
		}

		// Token: 0x060045C0 RID: 17856 RVA: 0x0020C354 File Offset: 0x0020A554
		public unsafe KeyframeTextureBaker.AnimationClipDataBaked GetAnimInfo(int index)
		{
			int num = this.IndexToBakedID(index);
			return this.data->anim_data[this.drawer_anim_id * this.data->NumBakedAnims + num];
		}

		// Token: 0x060045C1 RID: 17857 RVA: 0x0020C396 File Offset: 0x0020A596
		public unsafe int IndexToBakedID(int index)
		{
			return this.data->index_to_baked_id[index];
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x0020C3AC File Offset: 0x0020A5AC
		public bool HasValidAnim(UnitAnimation.Index anim_index)
		{
			int num = this.IndexToBakedID((int)anim_index);
			if (num == -1)
			{
				return false;
			}
			int baked_anim_count = this.baked_anim_count;
			return num < baked_anim_count && this.GetAnimInfo((int)anim_index).valid;
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x060045C3 RID: 17859 RVA: 0x0020C3E0 File Offset: 0x0020A5E0
		// (set) Token: 0x060045C4 RID: 17860 RVA: 0x0020C3F8 File Offset: 0x0020A5F8
		public unsafe int baked_anim_count
		{
			get
			{
				return this.data->baked_anim_count[this.drawer_anim_id];
			}
			set
			{
				this.data->baked_anim_count[this.drawer_anim_id] = value;
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x060045C5 RID: 17861 RVA: 0x0020C411 File Offset: 0x0020A611
		public KeyframeTextureBaker.AnimationClipDataBaked cur_anim_info
		{
			get
			{
				return this.GetAnimInfo(this.cur_anim_idx);
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x060045C6 RID: 17862 RVA: 0x0020C41F File Offset: 0x0020A61F
		public KeyframeTextureBaker.AnimationClipDataBaked prev_anim_info
		{
			get
			{
				return this.GetAnimInfo(this.prev_anim_idx);
			}
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x060045C7 RID: 17863 RVA: 0x0020C42D File Offset: 0x0020A62D
		public float cur_anim_length
		{
			get
			{
				return this.cur_anim_info.AnimationLength;
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x060045C8 RID: 17864 RVA: 0x0020C43A File Offset: 0x0020A63A
		public float cur_anim_blend_time
		{
			get
			{
				return this.cur_anim_info.BlendTime;
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x060045C9 RID: 17865 RVA: 0x0020C447 File Offset: 0x0020A647
		public float cur_anim_range
		{
			get
			{
				return this.cur_anim_info.TextureRange;
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x060045CA RID: 17866 RVA: 0x0020C454 File Offset: 0x0020A654
		public float cur_anim_offset
		{
			get
			{
				return this.cur_anim_info.TextureStart;
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x060045CB RID: 17867 RVA: 0x0020C461 File Offset: 0x0020A661
		public float prev_anim_length
		{
			get
			{
				return this.prev_anim_info.AnimationLength;
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x060045CC RID: 17868 RVA: 0x0020C46E File Offset: 0x0020A66E
		public float prev_anim_blend_time
		{
			get
			{
				return this.prev_anim_info.BlendTime;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x060045CD RID: 17869 RVA: 0x0020C47B File Offset: 0x0020A67B
		public float prev_anim_range
		{
			get
			{
				return this.prev_anim_info.TextureRange;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x060045CE RID: 17870 RVA: 0x0020C488 File Offset: 0x0020A688
		public float prev_anim_offset
		{
			get
			{
				return this.prev_anim_info.TextureStart;
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x060045CF RID: 17871 RVA: 0x0020C495 File Offset: 0x0020A695
		public unsafe float texture_width
		{
			get
			{
				return this.data->texture_width[this.squad->def->type];
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x060045D0 RID: 17872 RVA: 0x0020C4B8 File Offset: 0x0020A6B8
		public float action_frame
		{
			get
			{
				return this.cur_anim_info.GetAction(KeyframeTextureBaker.ActionFrame.ActionType.Attack);
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x060045D1 RID: 17873 RVA: 0x0020C4D4 File Offset: 0x0020A6D4
		// (set) Token: 0x060045D2 RID: 17874 RVA: 0x0020C4F5 File Offset: 0x0020A6F5
		public unsafe float3 anim_result
		{
			get
			{
				return this.data->anim_result[this.id];
			}
			set
			{
				this.data->anim_result[this.id] = value;
			}
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x0020C517 File Offset: 0x0020A717
		public unsafe void SetArrow(int i, int arrow_id)
		{
			this.data->cur_arrows[this.id * this.data->NumTroopsPerSquad + i] = arrow_id;
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x0020C53E File Offset: 0x0020A73E
		public unsafe int GetArrowID(int i)
		{
			return this.data->cur_arrows[this.id * this.data->NumTroopsPerSquad + i];
		}

		// Token: 0x060045D5 RID: 17877 RVA: 0x0020C564 File Offset: 0x0020A764
		public unsafe void CancelArrows()
		{
			for (int i = 0; i < this.arrow_count; i++)
			{
				int arrowID = this.GetArrowID(i);
				if (arrowID != -1)
				{
					Troops.Arrow arrow = this.data->GetArrow(this.thread_id, arrowID);
					if (arrow.HasFlags(Troops.Arrow.Flags.AboutToShoot))
					{
						arrow.Reset();
						this.SetArrow(i, -1);
					}
				}
			}
			this.ClrFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger);
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x060045D6 RID: 17878 RVA: 0x0020C5C5 File Offset: 0x0020A7C5
		// (set) Token: 0x060045D7 RID: 17879 RVA: 0x0020C5DD File Offset: 0x0020A7DD
		public unsafe int arrow_count
		{
			get
			{
				return this.data->arrow_count[this.id];
			}
			set
			{
				this.data->arrow_count[this.id] = value;
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x060045D8 RID: 17880 RVA: 0x0020C5F6 File Offset: 0x0020A7F6
		// (set) Token: 0x060045D9 RID: 17881 RVA: 0x0020C60E File Offset: 0x0020A80E
		public unsafe float attack_cd
		{
			get
			{
				return this.data->attack_cd[this.id];
			}
			set
			{
				this.data->attack_cd[this.id] = value;
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x060045DA RID: 17882 RVA: 0x0020C627 File Offset: 0x0020A827
		// (set) Token: 0x060045DB RID: 17883 RVA: 0x0020C648 File Offset: 0x0020A848
		public unsafe float find_enemy_cd
		{
			get
			{
				return this.data->cooldowns[this.id].x;
			}
			set
			{
				this.data->cooldowns[this.id].x = value;
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x060045DC RID: 17884 RVA: 0x0020C66A File Offset: 0x0020A86A
		// (set) Token: 0x060045DD RID: 17885 RVA: 0x0020C68B File Offset: 0x0020A88B
		public unsafe float movement_blocked_cd
		{
			get
			{
				return this.data->cooldowns[this.id].y;
			}
			set
			{
				this.data->cooldowns[this.id].y = value;
			}
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x060045DE RID: 17886 RVA: 0x0020C6AD File Offset: 0x0020A8AD
		// (set) Token: 0x060045DF RID: 17887 RVA: 0x0020C6CE File Offset: 0x0020A8CE
		public unsafe float charged_lastly_cd
		{
			get
			{
				return this.data->cooldowns[this.id].z;
			}
			set
			{
				this.data->cooldowns[this.id].z = value;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060045E0 RID: 17888 RVA: 0x0020C6F0 File Offset: 0x0020A8F0
		// (set) Token: 0x060045E1 RID: 17889 RVA: 0x0020C711 File Offset: 0x0020A911
		public unsafe float slow_down_cd
		{
			get
			{
				return this.data->cooldowns[this.id].w;
			}
			set
			{
				this.data->cooldowns[this.id].w = value;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060045E2 RID: 17890 RVA: 0x0020C733 File Offset: 0x0020A933
		// (set) Token: 0x060045E3 RID: 17891 RVA: 0x0020C748 File Offset: 0x0020A948
		public unsafe bool face_enemies
		{
			get
			{
				return this.data->face_enemies[this.id];
			}
			set
			{
				this.data->face_enemies[this.id] = value;
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060045E4 RID: 17892 RVA: 0x0020C75E File Offset: 0x0020A95E
		// (set) Token: 0x060045E5 RID: 17893 RVA: 0x0020C776 File Offset: 0x0020A976
		public unsafe int enemy_id
		{
			get
			{
				return this.data->enemy_id[this.id];
			}
			set
			{
				this.data->enemy_id[this.id] = value;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060045E6 RID: 17894 RVA: 0x0020C78F File Offset: 0x0020A98F
		// (set) Token: 0x060045E7 RID: 17895 RVA: 0x0020C7A7 File Offset: 0x0020A9A7
		public unsafe int range_enemy_squad_id
		{
			get
			{
				return this.data->ranged_enemy_squad_id[this.id];
			}
			set
			{
				this.data->ranged_enemy_squad_id[this.id] = value;
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060045E8 RID: 17896 RVA: 0x0020C7C0 File Offset: 0x0020A9C0
		// (set) Token: 0x060045E9 RID: 17897 RVA: 0x0020C7D5 File Offset: 0x0020A9D5
		public unsafe bool is_under_fire
		{
			get
			{
				return this.data->is_under_fire[this.id];
			}
			set
			{
				this.data->is_under_fire[this.id] = value;
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060045EA RID: 17898 RVA: 0x0020C7EB File Offset: 0x0020A9EB
		// (set) Token: 0x060045EB RID: 17899 RVA: 0x0020C800 File Offset: 0x0020AA00
		public unsafe byte fight_status
		{
			get
			{
				return this.data->fight_status[this.id];
			}
			set
			{
				this.data->fight_status[this.id] = value;
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x060045EC RID: 17900 RVA: 0x0020C816 File Offset: 0x0020AA16
		// (set) Token: 0x060045ED RID: 17901 RVA: 0x0020C837 File Offset: 0x0020AA37
		public unsafe bool flanking_left
		{
			get
			{
				return this.data->flanking_status[this.id].x;
			}
			set
			{
				this.data->flanking_status[this.id].x = value;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x060045EE RID: 17902 RVA: 0x0020C859 File Offset: 0x0020AA59
		// (set) Token: 0x060045EF RID: 17903 RVA: 0x0020C87A File Offset: 0x0020AA7A
		public unsafe bool flanking_right
		{
			get
			{
				return this.data->flanking_status[this.id].y;
			}
			set
			{
				this.data->flanking_status[this.id].y = value;
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060045F0 RID: 17904 RVA: 0x0020C89C File Offset: 0x0020AA9C
		// (set) Token: 0x060045F1 RID: 17905 RVA: 0x0020C8B5 File Offset: 0x0020AAB5
		public unsafe Troops.Troop enemy
		{
			get
			{
				return new Troops.Troop(this.data, this.cur_thread_id, this.enemy_id);
			}
			set
			{
				this.data->enemy_id[this.id] = ((!this.enemy) ? -1 : this.enemy.id);
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x060045F2 RID: 17906 RVA: 0x0020C8E8 File Offset: 0x0020AAE8
		public unsafe Troops.FortificationData* enemy_fortification
		{
			get
			{
				return this.squad->enemy_fortification.ptr;
			}
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x0020C8FA File Offset: 0x0020AAFA
		public static bool operator !(Troops.Troop troop)
		{
			return !troop.valid;
		}

		// Token: 0x060045F4 RID: 17908 RVA: 0x0020C906 File Offset: 0x0020AB06
		public static Troops.Troop operator ++(Troops.Troop troop)
		{
			troop.id++;
			return troop;
		}

		// Token: 0x060045F5 RID: 17909 RVA: 0x0020C915 File Offset: 0x0020AB15
		public static bool operator <=(Troops.Troop t1, Troops.Troop t2)
		{
			return t1.id <= t2.id;
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x0020C928 File Offset: 0x0020AB28
		public static bool operator >=(Troops.Troop t1, Troops.Troop t2)
		{
			return t1.id >= t2.id;
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x060045F7 RID: 17911 RVA: 0x0020C93C File Offset: 0x0020AB3C
		public string DebugText
		{
			[BurstDiscard]
			get
			{
				return string.Concat(new object[]
				{
					"[",
					this.thread_id,
					"/",
					this.cur_thread_id,
					"] ",
					this.id,
					":",
					this.squad_id
				});
			}
		}

		// Token: 0x040031C9 RID: 12745
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x040031CA RID: 12746
		public int cur_thread_id;

		// Token: 0x040031CB RID: 12747
		public int id;

		// Token: 0x040031CC RID: 12748
		public const float pushOff_duration_min = 0.5f;

		// Token: 0x040031CD RID: 12749
		public const float pushOff_duration_max = 2f;

		// Token: 0x040031CE RID: 12750
		public const float pushOff_duration_climb_mod = 2f;

		// Token: 0x040031CF RID: 12751
		public const float pushOff_force = 0.75f;

		// Token: 0x040031D0 RID: 12752
		public const byte fight_status_no_enemy = 0;

		// Token: 0x040031D1 RID: 12753
		public const byte fight_status_has_accessible_enemy = 1;

		// Token: 0x040031D2 RID: 12754
		public const byte fight_status_go_towards_position_in_formation = 2;

		// Token: 0x040031D3 RID: 12755
		public const byte fight_status_go_towards_position_between_formation_pos_and_enemy_pos = 3;

		// Token: 0x040031D4 RID: 12756
		public const byte fight_status_slightly_flank_left = 4;

		// Token: 0x040031D5 RID: 12757
		public const byte fight_status_slightly_flank_right = 5;

		// Token: 0x040031D6 RID: 12758
		public const byte fight_status_flank_left = 6;

		// Token: 0x040031D7 RID: 12759
		public const byte fight_status_flank_right = 7;

		// Token: 0x040031D8 RID: 12760
		public const byte fight_status_wait = 8;

		// Token: 0x020009DB RID: 2523
		[Flags]
		public enum Flags
		{
			// Token: 0x04004583 RID: 17795
			None = 0,
			// Token: 0x04004584 RID: 17796
			Moving = 1,
			// Token: 0x04004585 RID: 17797
			AttackMove = 2,
			// Token: 0x04004586 RID: 17798
			Collided = 4,
			// Token: 0x04004587 RID: 17799
			Stuck = 8,
			// Token: 0x04004588 RID: 17800
			FarFromSquad = 16,
			// Token: 0x04004589 RID: 17801
			Fighting = 32,
			// Token: 0x0400458A RID: 17802
			Attacking = 64,
			// Token: 0x0400458B RID: 17803
			MovingDown = 128,
			// Token: 0x0400458C RID: 17804
			ClimbingLadder = 256,
			// Token: 0x0400458D RID: 17805
			Killed = 512,
			// Token: 0x0400458E RID: 17806
			Charging = 1024,
			// Token: 0x0400458F RID: 17807
			Shooting = 2048,
			// Token: 0x04004590 RID: 17808
			Thrown = 4096,
			// Token: 0x04004591 RID: 17809
			ShootTrigger = 8192,
			// Token: 0x04004592 RID: 17810
			Dead = 16384,
			// Token: 0x04004593 RID: 17811
			Destroyed = 32768,
			// Token: 0x04004594 RID: 17812
			PushedOff = 65536,
			// Token: 0x04004595 RID: 17813
			Trampling = 131072,
			// Token: 0x04004596 RID: 17814
			ClimbingLadderFinished = 262144,
			// Token: 0x04004597 RID: 17815
			ClimbingLadderWaiting = 524288,
			// Token: 0x04004598 RID: 17816
			Blocked = 1048576
		}
	}

	// Token: 0x020005E1 RID: 1505
	[DebuggerDisplay("[{DebugText}]")]
	public struct DefData
	{
		// Token: 0x060045F8 RID: 17912 RVA: 0x0020C9AC File Offset: 0x0020ABAC
		public DefData(int type, Logic.Unit.Def def)
		{
			this.type = type;
			def.troops_def_idx = type;
			this.radius = def.radius * def.battle_scale;
			this.selection_radius = def.selection_radius;
			this.attack_range = def.attack_range;
			this.attack_interval = def.attack_interval;
			this.min_speed = def.min_speed;
			this.max_speed_mul = def.max_speed_mul;
			this.max_acceleration = def.max_acceleration;
			this.max_deceleration = def.max_deceleration;
			this.walk_anim_speed = def.walk_anim_speed * def.battle_scale;
			this.trot_anim_speed = def.trot_anim_speed * def.battle_scale;
			this.run_anim_speed = def.run_anim_speed * def.battle_scale;
			this.sprint_anim_speed = def.sprint_anim_speed * def.battle_scale;
			this.charge_anim_speed = def.charge_anim_speed * def.battle_scale;
			this.walk_to_trot_ratio = def.walk_to_trot_ratio;
			this.trot_to_run_ratio = def.trot_to_run_ratio;
			this.run_to_sprint_ratio = def.run_to_sprint_ratio;
			this.shoot_interval = def.shoot_interval;
			this.CTH = def.CTH;
			this.CTH_cavalry_mod = def.CTH_cavalry_mod;
			this.CTH_shoot_mod = def.CTH_shoot_mod;
			this.CTH_siege_vs_siege = def.CTH_siege_vs_siege;
			this.defense = def.defense;
			this.is_cavalry = def.is_cavalry;
			this.is_ranged = def.is_ranged;
			this.is_spearman = (def.type == Logic.Unit.Type.Defense);
			this.is_heavy = (def.is_heavy || def.type == Logic.Unit.Type.Noble);
			this.is_siege_eq = (def.type == Logic.Unit.Type.InventoryItem);
			this.trample_chance = def.trample_chance;
			this.turn_speed = def.turn_speed;
			this.max_health = def.max_health;
			this.can_attack_melee = def.can_attack_melee;
			this.terrain_normal_point_1 = (this.terrain_normal_point_2 = (this.terrain_normal_point_3 = (this.terrain_normal_point_4 = Point.Zero)));
			this.max_rotation_x = def.max_rotation_x;
			this.max_rotation_z = def.max_rotation_z;
			if (def.terrain_normal_points == null)
			{
				this.terrain_normal_checks = 0;
				return;
			}
			this.terrain_normal_checks = def.terrain_normal_points.Length;
			if (def.terrain_normal_points.Length != 0)
			{
				this.terrain_normal_point_1 = def.terrain_normal_points[0];
			}
			if (def.terrain_normal_points.Length > 1)
			{
				this.terrain_normal_point_2 = def.terrain_normal_points[1];
			}
			if (def.terrain_normal_points.Length > 2)
			{
				this.terrain_normal_point_3 = def.terrain_normal_points[2];
			}
			if (def.terrain_normal_points.Length > 3)
			{
				this.terrain_normal_point_4 = def.terrain_normal_points[3];
			}
		}

		// Token: 0x040031D9 RID: 12761
		public int type;

		// Token: 0x040031DA RID: 12762
		public float radius;

		// Token: 0x040031DB RID: 12763
		public float selection_radius;

		// Token: 0x040031DC RID: 12764
		public float attack_range;

		// Token: 0x040031DD RID: 12765
		public float attack_interval;

		// Token: 0x040031DE RID: 12766
		public float min_speed;

		// Token: 0x040031DF RID: 12767
		public float max_speed_mul;

		// Token: 0x040031E0 RID: 12768
		public float max_acceleration;

		// Token: 0x040031E1 RID: 12769
		public float max_deceleration;

		// Token: 0x040031E2 RID: 12770
		public float walk_anim_speed;

		// Token: 0x040031E3 RID: 12771
		public float trot_anim_speed;

		// Token: 0x040031E4 RID: 12772
		public float run_anim_speed;

		// Token: 0x040031E5 RID: 12773
		public float charge_anim_speed;

		// Token: 0x040031E6 RID: 12774
		public float sprint_anim_speed;

		// Token: 0x040031E7 RID: 12775
		public float walk_to_trot_ratio;

		// Token: 0x040031E8 RID: 12776
		public float trot_to_run_ratio;

		// Token: 0x040031E9 RID: 12777
		public float run_to_sprint_ratio;

		// Token: 0x040031EA RID: 12778
		public float turn_speed;

		// Token: 0x040031EB RID: 12779
		public float shoot_interval;

		// Token: 0x040031EC RID: 12780
		public float CTH;

		// Token: 0x040031ED RID: 12781
		public float CTH_cavalry_mod;

		// Token: 0x040031EE RID: 12782
		public float CTH_shoot_mod;

		// Token: 0x040031EF RID: 12783
		public float CTH_siege_vs_siege;

		// Token: 0x040031F0 RID: 12784
		public float defense;

		// Token: 0x040031F1 RID: 12785
		public bool is_cavalry;

		// Token: 0x040031F2 RID: 12786
		public bool is_ranged;

		// Token: 0x040031F3 RID: 12787
		public bool is_spearman;

		// Token: 0x040031F4 RID: 12788
		public bool is_heavy;

		// Token: 0x040031F5 RID: 12789
		public bool is_siege_eq;

		// Token: 0x040031F6 RID: 12790
		public float trample_chance;

		// Token: 0x040031F7 RID: 12791
		public float max_health;

		// Token: 0x040031F8 RID: 12792
		public bool can_attack_melee;

		// Token: 0x040031F9 RID: 12793
		public int terrain_normal_checks;

		// Token: 0x040031FA RID: 12794
		public Point terrain_normal_point_1;

		// Token: 0x040031FB RID: 12795
		public Point terrain_normal_point_2;

		// Token: 0x040031FC RID: 12796
		public Point terrain_normal_point_3;

		// Token: 0x040031FD RID: 12797
		public Point terrain_normal_point_4;

		// Token: 0x040031FE RID: 12798
		public float max_rotation_x;

		// Token: 0x040031FF RID: 12799
		public float max_rotation_z;
	}

	// Token: 0x020005E2 RID: 1506
	[DebuggerDisplay("[{DebugText}]")]
	public struct SalvoDefData
	{
		// Token: 0x060045F9 RID: 17913 RVA: 0x0020CC48 File Offset: 0x0020AE48
		public SalvoDefData(int type, Logic.SalvoData.Def def)
		{
			this.type = type;
			def.troops_def_idx = type;
			this.projectile_radius = def.projectile_radius;
			this.splash_damage = def.splash_damage;
			this.explosion_force = def.explosion_force;
			this.min_shoot_speed = def.min_shoot_speed;
			this.shoot_speed_randomization_mod = def.shoot_speed_randomization_mod;
			this.collision_check_offset = def.collision_check_offset;
			this.max_end_position_offset = def.max_end_position_offset;
			this.min_shoot_angle = def.min_shoot_angle;
			this.max_shoot_angle = def.max_shoot_angle;
			this.min_shoot_range = def.min_shoot_range;
			this.max_shoot_range = def.max_shoot_range;
			this.friendly_fire_mod = def.friendly_fire_mod;
			this.shoot_height = def.shoot_height;
			this.shoot_offset = def.shoot_offset;
			this.gravity = def.gravity;
			this.can_hit_fortification = def.can_hit_fortification;
			this.draw_trail = (def.field.GetRandomValue("arrow_trail_texture", null, true, true, true, '.').Get<Texture2D>() != null);
			this.draw_after_landing = def.draw_after_landing;
		}

		// Token: 0x04003200 RID: 12800
		public int type;

		// Token: 0x04003201 RID: 12801
		public float projectile_radius;

		// Token: 0x04003202 RID: 12802
		public bool splash_damage;

		// Token: 0x04003203 RID: 12803
		public float explosion_force;

		// Token: 0x04003204 RID: 12804
		public float min_shoot_speed;

		// Token: 0x04003205 RID: 12805
		public float shoot_speed_randomization_mod;

		// Token: 0x04003206 RID: 12806
		public float collision_check_offset;

		// Token: 0x04003207 RID: 12807
		public float max_end_position_offset;

		// Token: 0x04003208 RID: 12808
		public float min_shoot_range;

		// Token: 0x04003209 RID: 12809
		public float max_shoot_range;

		// Token: 0x0400320A RID: 12810
		public float min_shoot_angle;

		// Token: 0x0400320B RID: 12811
		public float max_shoot_angle;

		// Token: 0x0400320C RID: 12812
		public float gravity;

		// Token: 0x0400320D RID: 12813
		public float friendly_fire_mod;

		// Token: 0x0400320E RID: 12814
		public bool can_hit_fortification;

		// Token: 0x0400320F RID: 12815
		public float shoot_height;

		// Token: 0x04003210 RID: 12816
		public float shoot_offset;

		// Token: 0x04003211 RID: 12817
		public bool draw_trail;

		// Token: 0x04003212 RID: 12818
		public bool draw_after_landing;
	}

	// Token: 0x020005E3 RID: 1507
	[DebuggerDisplay("[{DebugText}]")]
	public struct SquadData
	{
		// Token: 0x060045FA RID: 17914 RVA: 0x0020CD59 File Offset: 0x0020AF59
		public bool IsRearranging()
		{
			return this.rearrange_safe_dist == this.rearrange_min_safe_dist;
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x0020CD6C File Offset: 0x0020AF6C
		public unsafe SquadData(Troops.TroopsPtrData* pdata, int battle_side, Troops.DefData* def, int id, int main_squad_id, int offset, int size, float CTH_final, float defense_final, float resiliance_total, float shock_chance, float shock_damage_base, float shock_damage_trample, float friendly_fire_mod, PathData.PassableArea.Type allowed_areas, int kingdom_color_id, float max_health)
		{
			this.pdata = pdata;
			this.thread_id = -1;
			this.battle_side = battle_side;
			this.def = def;
			this.id = id;
			this.main_squad_id = main_squad_id;
			this.offset = offset;
			this.size = size;
			this.CTH_final = CTH_final;
			this.defense_final = defense_final;
			this.defense_against_ranged_final = defense_final;
			this.trample_chance_final = 0f;
			this.CTH_against_me_mod = 1f;
			this.defense_against_trample_mod = 2f;
			this.avoid_trample_tight_formation_mod = 1f;
			this.CTH_against_cav_charge_mod = 2f;
			this.resiliance_total = resiliance_total;
			this.shock_chance = shock_chance;
			this.shock_damage_base = shock_damage_base;
			this.shock_damage_bonus_trample = shock_damage_trample;
			this.shock_morale_dmg_acc = 0f;
			this.min_attack_move_range = 8f;
			this.max_attack_move_range = 20f;
			this.friendly_fire_reduction = friendly_fire_mod;
			this.kingdom_color_id = kingdom_color_id;
			this.selected = false;
			this.highlighted = false;
			this.in_drag = false;
			this.target_previewed = false;
			this.previewed = false;
			this.stance_to_player = TextureBaker.StanceColors.Own;
			this.mt_flags = 0;
			this.logic_alive = size;
			this.pos = new PPos(0f, 0f, 0);
			this.tgtPos = new PPos(0f, 0f, 0);
			this.path_len = 0f;
			this.dir = 0;
			this.rot = 0f;
			this.form_rot = 0f;
			this.preview_formation_dir = 0f;
			this.move_speed = 0f;
			this.modified_move_speed = 0f;
			this.troop_too_far = false;
			this.command = Logic.Squad.Command.Hold;
			this.stance = Logic.Squad.Stance.Defensive;
			this.is_attacking = false;
			this.wedgeFormation = false;
			this.target_id = -1;
			this.controlZone = 0;
			this.bb_lower_left_corner = new float2(0f, 0f);
			this.bb_upper_right_corner = new float2(0f, 0f);
			this.sqr_radius = 0f;
			this.banner_pos = 0;
			this.closest_troop_banner_id = -1;
			this.cur_salvo = -1;
			this.salvos_left = 0;
			this.hold_fire = false;
			this.enemy_fortification = default(Troops.FortificationData.Ptr);
			this.stuck_recalc_attempts = 0;
			this.stuck_recalc_time = 0f;
			this.is_Fighting = false;
			this.is_Fighting_Target = false;
			this.was_Fighting_Target = false;
			this.last_Fight_Time = 0f;
			this.last_moving_time = 0f;
			this.was_moving_lately = false;
			this.has_accessible_enemies = false;
			this.health = max_health;
			this.climb_cooldown = 0f;
			this.climb_start_pt = 0;
			this.climb_dest_pt = 0;
			this.climb_start_paid = 0;
			this.climb_dest_paid = 0;
			this.climb_ladder_paid = 0;
			this.allowed_areas = allowed_areas;
			this.is_inside_wall = false;
			this.rearrange_max_safe_dist = 1.75f;
			this.rearrange_min_safe_dist = 0.1f;
			this.rearrange_safe_dist = this.rearrange_max_safe_dist;
			this.rearrange_troops_delay = 7f;
			this.rearrange_troops_time = 5f;
			this.rearrange_troops_cooldown = 0f;
			this.aggressive_base_flank_angle = 45f;
			this.aggressive_additional_flank_angle_cavalry = 85f;
			this.aggressive_additional_flank_angle_infantry = 75f;
			this.defensive_base_flank_angle = 0f;
			this.defensive_additional_flank_angle_cavalry = 0f;
			this.defensive_additional_flank_angle_infantry = 0f;
			this.are_selection_circles_enabled = true;
			float num = 4f;
			Troops.Troop troop = this.FirstTroop;
			while (troop <= this.LastTroop)
			{
				for (int i = 0; i < 5; i++)
				{
					int num2 = troop.id * 5 + i;
					float4 @float = troop.data->dust_particle_positions[num2];
					@float.w = -num - troop.data->rand.NextFloat(0f, num);
					troop.data->dust_particle_positions[num2] = @float;
				}
				troop = ++troop;
			}
			this.move_history = new Troops.MoveHistory(ref this);
			this.SetFlags(Troops.SquadData.Flags.Visible | Troops.SquadData.Flags.Teleport);
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060045FC RID: 17916 RVA: 0x0020D17A File Offset: 0x0020B37A
		public Troops.Troop FirstTroop
		{
			get
			{
				return new Troops.Troop(this.pdata, this.thread_id, this.offset);
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060045FD RID: 17917 RVA: 0x0020D193 File Offset: 0x0020B393
		public Troops.Troop LastTroop
		{
			get
			{
				return new Troops.Troop(this.pdata, this.thread_id, this.offset + this.size - 1);
			}
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060045FE RID: 17918 RVA: 0x0020D1B5 File Offset: 0x0020B3B5
		public Troops.SquadData.Flags flags
		{
			get
			{
				return (Troops.SquadData.Flags)this.mt_flags;
			}
		}

		// Token: 0x060045FF RID: 17919 RVA: 0x0020D1BD File Offset: 0x0020B3BD
		public bool HasFlags(Troops.SquadData.Flags mask)
		{
			return Troops.MTFlags.get(this.mt_flags, (int)mask);
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x0020D1CB File Offset: 0x0020B3CB
		public void SetFlags(Troops.SquadData.Flags mask)
		{
			Troops.MTFlags.set(ref this.mt_flags, (int)mask);
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x0020D1D9 File Offset: 0x0020B3D9
		public void ClrFlags(Troops.SquadData.Flags mask)
		{
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x0020D1E7 File Offset: 0x0020B3E7
		public void SetFlags(Troops.SquadData.Flags mask, bool set)
		{
			if (set)
			{
				Troops.MTFlags.set(ref this.mt_flags, (int)mask);
				return;
			}
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x0020D208 File Offset: 0x0020B408
		public void SetControlZone(float width, float height, float defensive_range, float aggresive_range, float attacking_range)
		{
			this.min_attack_move_range = defensive_range;
			this.max_attack_move_range = aggresive_range;
			float num;
			if (this.is_attacking)
			{
				num = attacking_range;
			}
			else
			{
				num = ((this.stance == Logic.Squad.Stance.Defensive) ? this.min_attack_move_range : this.max_attack_move_range);
			}
			this.controlZone = new float2(width + num, height + num);
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x0020D25C File Offset: 0x0020B45C
		public bool IsInSquadRectangle(PPos enemyPos)
		{
			PPos rotated = this.pos.GetRotated(this.rot);
			PPos rotated2 = enemyPos.GetRotated(this.rot);
			return rotated2.x < rotated.x + 0.5f * this.controlZone.x && rotated2.x > rotated.x - 0.5f * this.controlZone.x && rotated2.y < rotated.y + 0.5f * this.controlZone.y && rotated2.y > rotated.y - 0.5f * this.controlZone.y;
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06004605 RID: 17925 RVA: 0x0020D313 File Offset: 0x0020B513
		public string DebugText
		{
			[BurstDiscard]
			get
			{
				return string.Concat(new object[]
				{
					"[",
					this.thread_id,
					"] ",
					this.id
				});
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06004606 RID: 17926 RVA: 0x0020D34C File Offset: 0x0020B54C
		public float2 BoundingBoxCenter
		{
			get
			{
				return math.lerp(this.bb_lower_left_corner, this.bb_upper_right_corner, 0.5f);
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06004607 RID: 17927 RVA: 0x0020D364 File Offset: 0x0020B564
		public float2 BoundingBoxSize
		{
			get
			{
				return this.bb_upper_right_corner - this.bb_lower_left_corner;
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06004608 RID: 17928 RVA: 0x0020D377 File Offset: 0x0020B577
		public float BoundingBoxRadiusSqr
		{
			get
			{
				return math.pow(this.BoundingBoxSize.x / 2f, 2f) + math.pow(this.BoundingBoxSize.y / 2f, 2f);
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06004609 RID: 17929 RVA: 0x0020D3B0 File Offset: 0x0020B5B0
		public float BoundingBoxRadius
		{
			get
			{
				return math.sqrt(this.BoundingBoxRadiusSqr);
			}
		}

		// Token: 0x04003213 RID: 12819
		public unsafe Troops.TroopsPtrData* pdata;

		// Token: 0x04003214 RID: 12820
		public int thread_id;

		// Token: 0x04003215 RID: 12821
		public int battle_side;

		// Token: 0x04003216 RID: 12822
		public unsafe Troops.DefData* def;

		// Token: 0x04003217 RID: 12823
		public int id;

		// Token: 0x04003218 RID: 12824
		public int main_squad_id;

		// Token: 0x04003219 RID: 12825
		public int offset;

		// Token: 0x0400321A RID: 12826
		public int size;

		// Token: 0x0400321B RID: 12827
		private int mt_flags;

		// Token: 0x0400321C RID: 12828
		public int logic_alive;

		// Token: 0x0400321D RID: 12829
		public PPos pos;

		// Token: 0x0400321E RID: 12830
		public PPos tgtPos;

		// Token: 0x0400321F RID: 12831
		public float path_len;

		// Token: 0x04003220 RID: 12832
		public float2 dir;

		// Token: 0x04003221 RID: 12833
		public float rot;

		// Token: 0x04003222 RID: 12834
		public bool selected;

		// Token: 0x04003223 RID: 12835
		public bool highlighted;

		// Token: 0x04003224 RID: 12836
		public bool in_drag;

		// Token: 0x04003225 RID: 12837
		public bool target_previewed;

		// Token: 0x04003226 RID: 12838
		public bool previewed;

		// Token: 0x04003227 RID: 12839
		public TextureBaker.StanceColors stance_to_player;

		// Token: 0x04003228 RID: 12840
		public float preview_formation_dir;

		// Token: 0x04003229 RID: 12841
		public int kingdom_color_id;

		// Token: 0x0400322A RID: 12842
		public float form_rot;

		// Token: 0x0400322B RID: 12843
		public float move_speed;

		// Token: 0x0400322C RID: 12844
		public float modified_move_speed;

		// Token: 0x0400322D RID: 12845
		public bool troop_too_far;

		// Token: 0x0400322E RID: 12846
		public Logic.Squad.Command command;

		// Token: 0x0400322F RID: 12847
		public Logic.Squad.Stance stance;

		// Token: 0x04003230 RID: 12848
		public bool is_attacking;

		// Token: 0x04003231 RID: 12849
		public bool wedgeFormation;

		// Token: 0x04003232 RID: 12850
		public int target_id;

		// Token: 0x04003233 RID: 12851
		public float CTH_final;

		// Token: 0x04003234 RID: 12852
		public float defense_final;

		// Token: 0x04003235 RID: 12853
		public float defense_against_ranged_final;

		// Token: 0x04003236 RID: 12854
		public float trample_chance_final;

		// Token: 0x04003237 RID: 12855
		public float CTH_against_me_mod;

		// Token: 0x04003238 RID: 12856
		public float defense_against_trample_mod;

		// Token: 0x04003239 RID: 12857
		public float avoid_trample_tight_formation_mod;

		// Token: 0x0400323A RID: 12858
		public float CTH_against_cav_charge_mod;

		// Token: 0x0400323B RID: 12859
		public float resiliance_total;

		// Token: 0x0400323C RID: 12860
		public float shock_chance;

		// Token: 0x0400323D RID: 12861
		public float shock_damage_base;

		// Token: 0x0400323E RID: 12862
		public float shock_damage_bonus_trample;

		// Token: 0x0400323F RID: 12863
		public float shock_morale_dmg_acc;

		// Token: 0x04003240 RID: 12864
		public float friendly_fire_reduction;

		// Token: 0x04003241 RID: 12865
		public float min_attack_move_range;

		// Token: 0x04003242 RID: 12866
		public float max_attack_move_range;

		// Token: 0x04003243 RID: 12867
		public float2 controlZone;

		// Token: 0x04003244 RID: 12868
		public float2 bb_lower_left_corner;

		// Token: 0x04003245 RID: 12869
		public float2 bb_upper_right_corner;

		// Token: 0x04003246 RID: 12870
		public float sqr_radius;

		// Token: 0x04003247 RID: 12871
		public float3 banner_pos;

		// Token: 0x04003248 RID: 12872
		public int closest_troop_banner_id;

		// Token: 0x04003249 RID: 12873
		public Troops.FortificationData.Ptr enemy_fortification;

		// Token: 0x0400324A RID: 12874
		public int cur_salvo;

		// Token: 0x0400324B RID: 12875
		public int salvos_left;

		// Token: 0x0400324C RID: 12876
		public bool hold_fire;

		// Token: 0x0400324D RID: 12877
		[FixedBuffer(typeof(int), 30)]
		public Troops.SquadData.<animation_counts>e__FixedBuffer animation_counts;

		// Token: 0x0400324E RID: 12878
		public Troops.MoveHistory move_history;

		// Token: 0x0400324F RID: 12879
		public int stuck_recalc_attempts;

		// Token: 0x04003250 RID: 12880
		public float stuck_recalc_time;

		// Token: 0x04003251 RID: 12881
		public bool is_Fighting;

		// Token: 0x04003252 RID: 12882
		public bool is_Fighting_Target;

		// Token: 0x04003253 RID: 12883
		public bool was_Fighting_Target;

		// Token: 0x04003254 RID: 12884
		public float last_Fight_Time;

		// Token: 0x04003255 RID: 12885
		public float last_moving_time;

		// Token: 0x04003256 RID: 12886
		public bool was_moving_lately;

		// Token: 0x04003257 RID: 12887
		public bool has_accessible_enemies;

		// Token: 0x04003258 RID: 12888
		public float health;

		// Token: 0x04003259 RID: 12889
		public float climb_cooldown;

		// Token: 0x0400325A RID: 12890
		public float3 climb_start_pt;

		// Token: 0x0400325B RID: 12891
		public float3 climb_dest_pt;

		// Token: 0x0400325C RID: 12892
		public int climb_start_paid;

		// Token: 0x0400325D RID: 12893
		public int climb_dest_paid;

		// Token: 0x0400325E RID: 12894
		public int climb_ladder_paid;

		// Token: 0x0400325F RID: 12895
		public PathData.PassableArea.Type allowed_areas;

		// Token: 0x04003260 RID: 12896
		public bool is_inside_wall;

		// Token: 0x04003261 RID: 12897
		public float rearrange_max_safe_dist;

		// Token: 0x04003262 RID: 12898
		public float rearrange_min_safe_dist;

		// Token: 0x04003263 RID: 12899
		public float rearrange_safe_dist;

		// Token: 0x04003264 RID: 12900
		public float rearrange_troops_delay;

		// Token: 0x04003265 RID: 12901
		public float rearrange_troops_time;

		// Token: 0x04003266 RID: 12902
		public float rearrange_troops_cooldown;

		// Token: 0x04003267 RID: 12903
		public float aggressive_base_flank_angle;

		// Token: 0x04003268 RID: 12904
		public float aggressive_additional_flank_angle_cavalry;

		// Token: 0x04003269 RID: 12905
		public float aggressive_additional_flank_angle_infantry;

		// Token: 0x0400326A RID: 12906
		public float defensive_base_flank_angle;

		// Token: 0x0400326B RID: 12907
		public float defensive_additional_flank_angle_cavalry;

		// Token: 0x0400326C RID: 12908
		public float defensive_additional_flank_angle_infantry;

		// Token: 0x0400326D RID: 12909
		public bool are_selection_circles_enabled;

		// Token: 0x020009DC RID: 2524
		public struct Ptr
		{
			// Token: 0x04004599 RID: 17817
			public unsafe Troops.SquadData* ptr;
		}

		// Token: 0x020009DD RID: 2525
		[Flags]
		public enum Flags
		{
			// Token: 0x0400459B RID: 17819
			None = 0,
			// Token: 0x0400459C RID: 17820
			Visible = 1,
			// Token: 0x0400459D RID: 17821
			Moving = 2,
			// Token: 0x0400459E RID: 17822
			Alert = 4,
			// Token: 0x0400459F RID: 17823
			Fighting = 8,
			// Token: 0x040045A0 RID: 17824
			Dead = 16,
			// Token: 0x040045A1 RID: 17825
			Fled = 32,
			// Token: 0x040045A2 RID: 17826
			HasTroopClimbedLadder = 64,
			// Token: 0x040045A3 RID: 17827
			Teleport = 128,
			// Token: 0x040045A4 RID: 17828
			HasActiveTroops = 256,
			// Token: 0x040045A5 RID: 17829
			HadActiveTroops = 512,
			// Token: 0x040045A6 RID: 17830
			HasKilledTroops = 8192,
			// Token: 0x040045A7 RID: 17831
			Shooting = 16384,
			// Token: 0x040045A8 RID: 17832
			Destroyed = 32768,
			// Token: 0x040045A9 RID: 17833
			Retreating = 65536,
			// Token: 0x040045AA RID: 17834
			Charging = 131072,
			// Token: 0x040045AB RID: 17835
			HasFinishedShot = 262144,
			// Token: 0x040045AC RID: 17836
			Active = 527
		}

		// Token: 0x020009DE RID: 2526
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 120)]
		public struct <animation_counts>e__FixedBuffer
		{
			// Token: 0x040045AD RID: 17837
			public int FixedElementField;
		}
	}

	// Token: 0x020005E4 RID: 1508
	public struct TroopsMemData
	{
		// Token: 0x0600460A RID: 17930 RVA: 0x0020D3C0 File Offset: 0x0020B5C0
		public TroopsMemData(int troops_capacity, int squads_capacity)
		{
			this.SquadsCapacity = squads_capacity;
			this.TroopsPerSquad = troops_capacity / squads_capacity;
			this.NumDefs = 0;
			this.NumSalvoDefs = 0;
			this.NumFortifications = 0;
			this.NumBakedAnims = 0;
			this.defs = default(NativeArray<Troops.DefData>);
			this.salvo_defs = default(NativeArray<Troops.SalvoDefData>);
			this.squads = new NativeArray<Troops.SquadData>(squads_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.winner = -1;
			this.index_to_baked_id = new NativeArray<int>(43, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.squad = new NativeArray<Troops.SquadData.Ptr>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.drawer_ids = new NativeArray<int2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.drawer_anim_ids = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.model_data_buffer = new NativeArray<GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.flags = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.pa_id = new NativeArray<int2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.pos_rot = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.prev_pos_rot = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.rot_3d = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.previewPositions = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.tgt_arrow_rot = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.vel_spd_t = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.steer = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.separation = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.holdFormationVec = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.form_idx = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.posInFormation = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.deform = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.tgt_pos = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.anim_idx = new NativeArray<int2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.anim_time = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.attack_cd = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.cooldowns = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.enemy_id = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.ranged_enemy_squad_id = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.is_under_fire = new NativeArray<bool>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.fight_status = new NativeArray<byte>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.flanking_status = new NativeArray<bool2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.face_enemies = new NativeArray<bool>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.throw_args = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.throw_dir = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.pushOff_args = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.anim_data = default(NativeArray<KeyframeTextureBaker.AnimationClipDataBaked>);
			this.baked_anim_count = default(NativeArray<int>);
			this.texture_width = default(NativeArray<float>);
			this.anim_result = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.climb_progress = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.stuck_tgt_pos = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.stuck_tgt_pa_id = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.NumArrows = troops_capacity;
			this.NumSalvos = 0;
			this.NextSalvo = 0;
			this.salvos = new NativeArray<Troops.SalvoData>(squads_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.salvo = new NativeArray<Troops.SalvoData.Ptr>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_flags = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_pos = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_rot = new NativeArray<float4>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.end_pos = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.start_pos = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.mid_pos = new NativeArray<float3>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.is_high_angle = new NativeArray<bool>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.target_paid = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.start_v = new NativeArray<float2>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_t = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.cur_arrows = new NativeArray<int>(troops_capacity * this.TroopsPerSquad, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_count = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.cur_arrow_troop = new NativeArray<int>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.arrow_duration = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.trail_progress = new NativeArray<float>(troops_capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.dust_particle_positions = new NativeArray<float4>(troops_capacity * 5, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.fortifications = default(NativeArray<Troops.FortificationData>);
		}

		// Token: 0x0600460B RID: 17931 RVA: 0x0020D778 File Offset: 0x0020B978
		public void InitAnimData(int model_count)
		{
			this.NumBakedAnims = UnitAnimation.IndexBakedCount;
			this.anim_data = new NativeArray<KeyframeTextureBaker.AnimationClipDataBaked>(model_count * this.NumBakedAnims, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.baked_anim_count = new NativeArray<int>(model_count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.texture_width = new NativeArray<float>(model_count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = -1;
			for (int i = 0; i < 43; i++)
			{
				if (KeyframeTextureBaker.Skip((UnitAnimation.Index)i))
				{
					this.index_to_baked_id[i] = -1;
				}
				else
				{
					num++;
					this.index_to_baked_id[i] = num;
				}
			}
			Troops.death_decal_prefab = global::Defs.Get(false).dt.Find("Unit", null).GetRandomValue("dead_decal_prefab", null, true, true, true, '.').Get<GameObject>();
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x0020D82C File Offset: 0x0020BA2C
		public void Dispose()
		{
			this.defs.Dispose();
			this.salvo_defs.Dispose();
			this.squads.Dispose();
			this.index_to_baked_id.Dispose();
			this.squad.Dispose();
			this.drawer_ids.Dispose();
			this.drawer_anim_ids.Dispose();
			this.model_data_buffer.Dispose();
			this.flags.Dispose();
			this.pa_id.Dispose();
			this.pos_rot.Dispose();
			this.prev_pos_rot.Dispose();
			this.rot_3d.Dispose();
			this.previewPositions.Dispose();
			this.tgt_arrow_rot.Dispose();
			this.vel_spd_t.Dispose();
			this.steer.Dispose();
			this.separation.Dispose();
			this.holdFormationVec.Dispose();
			this.form_idx.Dispose();
			this.posInFormation.Dispose();
			this.deform.Dispose();
			this.tgt_pos.Dispose();
			this.anim_idx.Dispose();
			this.anim_time.Dispose();
			this.attack_cd.Dispose();
			this.cooldowns.Dispose();
			this.ranged_enemy_squad_id.Dispose();
			this.is_under_fire.Dispose();
			this.enemy_id.Dispose();
			this.fight_status.Dispose();
			this.flanking_status.Dispose();
			this.face_enemies.Dispose();
			this.throw_args.Dispose();
			this.throw_dir.Dispose();
			this.pushOff_args.Dispose();
			this.anim_data.Dispose();
			this.baked_anim_count.Dispose();
			this.anim_result.Dispose();
			this.climb_progress.Dispose();
			this.texture_width.Dispose();
			this.stuck_tgt_pos.Dispose();
			this.stuck_tgt_pa_id.Dispose();
			this.salvos.Dispose();
			this.salvo.Dispose();
			this.arrow_flags.Dispose();
			this.arrow_pos.Dispose();
			this.arrow_rot.Dispose();
			this.end_pos.Dispose();
			this.start_pos.Dispose();
			this.mid_pos.Dispose();
			this.is_high_angle.Dispose();
			this.target_paid.Dispose();
			this.start_v.Dispose();
			this.arrow_t.Dispose();
			this.cur_arrows.Dispose();
			this.arrow_count.Dispose();
			this.cur_arrow_troop.Dispose();
			this.arrow_duration.Dispose();
			this.trail_progress.Dispose();
			this.dust_particle_positions.Dispose();
			if (this.fortifications.IsCreated)
			{
				this.fortifications.Dispose();
			}
			Troops.texture_baker.Dispose();
		}

		// Token: 0x0400326E RID: 12910
		public int NumDefs;

		// Token: 0x0400326F RID: 12911
		public int NumSalvoDefs;

		// Token: 0x04003270 RID: 12912
		public int NumBakedAnims;

		// Token: 0x04003271 RID: 12913
		public NativeArray<int> index_to_baked_id;

		// Token: 0x04003272 RID: 12914
		public NativeArray<Troops.DefData> defs;

		// Token: 0x04003273 RID: 12915
		public NativeArray<Troops.SalvoDefData> salvo_defs;

		// Token: 0x04003274 RID: 12916
		public NativeArray<Troops.SquadData> squads;

		// Token: 0x04003275 RID: 12917
		public int winner;

		// Token: 0x04003276 RID: 12918
		public NativeArray<Troops.SquadData.Ptr> squad;

		// Token: 0x04003277 RID: 12919
		public NativeArray<int2> drawer_ids;

		// Token: 0x04003278 RID: 12920
		public NativeArray<int> drawer_anim_ids;

		// Token: 0x04003279 RID: 12921
		public NativeArray<GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>> model_data_buffer;

		// Token: 0x0400327A RID: 12922
		public NativeArray<int> flags;

		// Token: 0x0400327B RID: 12923
		public NativeArray<int2> pa_id;

		// Token: 0x0400327C RID: 12924
		public NativeArray<float4> pos_rot;

		// Token: 0x0400327D RID: 12925
		public NativeArray<float4> prev_pos_rot;

		// Token: 0x0400327E RID: 12926
		public NativeArray<float3> rot_3d;

		// Token: 0x0400327F RID: 12927
		public NativeArray<float3> previewPositions;

		// Token: 0x04003280 RID: 12928
		public NativeArray<float> tgt_arrow_rot;

		// Token: 0x04003281 RID: 12929
		public NativeArray<float4> vel_spd_t;

		// Token: 0x04003282 RID: 12930
		public NativeArray<float4> steer;

		// Token: 0x04003283 RID: 12931
		public NativeArray<float4> separation;

		// Token: 0x04003284 RID: 12932
		public NativeArray<float2> holdFormationVec;

		// Token: 0x04003285 RID: 12933
		public NativeArray<int> form_idx;

		// Token: 0x04003286 RID: 12934
		public NativeArray<float2> posInFormation;

		// Token: 0x04003287 RID: 12935
		public NativeArray<float2> deform;

		// Token: 0x04003288 RID: 12936
		public NativeArray<float2> tgt_pos;

		// Token: 0x04003289 RID: 12937
		public NativeArray<int2> anim_idx;

		// Token: 0x0400328A RID: 12938
		public NativeArray<float4> anim_time;

		// Token: 0x0400328B RID: 12939
		public NativeArray<float> attack_cd;

		// Token: 0x0400328C RID: 12940
		public NativeArray<float4> cooldowns;

		// Token: 0x0400328D RID: 12941
		public NativeArray<int> enemy_id;

		// Token: 0x0400328E RID: 12942
		public NativeArray<int> ranged_enemy_squad_id;

		// Token: 0x0400328F RID: 12943
		public NativeArray<bool> is_under_fire;

		// Token: 0x04003290 RID: 12944
		public NativeArray<byte> fight_status;

		// Token: 0x04003291 RID: 12945
		public NativeArray<bool2> flanking_status;

		// Token: 0x04003292 RID: 12946
		public NativeArray<bool> face_enemies;

		// Token: 0x04003293 RID: 12947
		public NativeArray<float4> throw_args;

		// Token: 0x04003294 RID: 12948
		public NativeArray<float2> throw_dir;

		// Token: 0x04003295 RID: 12949
		public NativeArray<float3> pushOff_args;

		// Token: 0x04003296 RID: 12950
		public NativeArray<float> climb_progress;

		// Token: 0x04003297 RID: 12951
		public NativeArray<float2> stuck_tgt_pos;

		// Token: 0x04003298 RID: 12952
		public NativeArray<int> stuck_tgt_pa_id;

		// Token: 0x04003299 RID: 12953
		public NativeArray<KeyframeTextureBaker.AnimationClipDataBaked> anim_data;

		// Token: 0x0400329A RID: 12954
		public NativeArray<int> baked_anim_count;

		// Token: 0x0400329B RID: 12955
		public NativeArray<float> texture_width;

		// Token: 0x0400329C RID: 12956
		public NativeArray<float3> anim_result;

		// Token: 0x0400329D RID: 12957
		public int NumArrows;

		// Token: 0x0400329E RID: 12958
		public int NumSalvos;

		// Token: 0x0400329F RID: 12959
		public int NumFortifications;

		// Token: 0x040032A0 RID: 12960
		public int NextSalvo;

		// Token: 0x040032A1 RID: 12961
		public int TroopsPerSquad;

		// Token: 0x040032A2 RID: 12962
		public int SquadsCapacity;

		// Token: 0x040032A3 RID: 12963
		public NativeArray<Troops.SalvoData> salvos;

		// Token: 0x040032A4 RID: 12964
		public NativeArray<Troops.SalvoData.Ptr> salvo;

		// Token: 0x040032A5 RID: 12965
		public NativeArray<int> arrow_flags;

		// Token: 0x040032A6 RID: 12966
		public NativeArray<float> trail_progress;

		// Token: 0x040032A7 RID: 12967
		public NativeArray<float4> arrow_pos;

		// Token: 0x040032A8 RID: 12968
		public NativeArray<float4> arrow_rot;

		// Token: 0x040032A9 RID: 12969
		public NativeArray<float3> end_pos;

		// Token: 0x040032AA RID: 12970
		public NativeArray<float3> start_pos;

		// Token: 0x040032AB RID: 12971
		public NativeArray<float3> mid_pos;

		// Token: 0x040032AC RID: 12972
		public NativeArray<float2> start_v;

		// Token: 0x040032AD RID: 12973
		public NativeArray<bool> is_high_angle;

		// Token: 0x040032AE RID: 12974
		public NativeArray<int> target_paid;

		// Token: 0x040032AF RID: 12975
		public NativeArray<float> arrow_t;

		// Token: 0x040032B0 RID: 12976
		public NativeArray<int> cur_arrows;

		// Token: 0x040032B1 RID: 12977
		public NativeArray<int> arrow_count;

		// Token: 0x040032B2 RID: 12978
		public NativeArray<int> cur_arrow_troop;

		// Token: 0x040032B3 RID: 12979
		public NativeArray<float> arrow_duration;

		// Token: 0x040032B4 RID: 12980
		public const int particles_per_troop = 5;

		// Token: 0x040032B5 RID: 12981
		public NativeArray<float4> dust_particle_positions;

		// Token: 0x040032B6 RID: 12982
		public NativeArray<Troops.FortificationData> fortifications;
	}

	// Token: 0x020005E5 RID: 1509
	public struct TroopsPtrData
	{
		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x0600460D RID: 17933 RVA: 0x0020DAFA File Offset: 0x0020BCFA
		// (set) Token: 0x0600460E RID: 17934 RVA: 0x0020DB02 File Offset: 0x0020BD02
		public int NumArrows { get; private set; }

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x0600460F RID: 17935 RVA: 0x0020DB0B File Offset: 0x0020BD0B
		// (set) Token: 0x06004610 RID: 17936 RVA: 0x0020DB13 File Offset: 0x0020BD13
		public int NumSalvos { get; private set; }

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06004611 RID: 17937 RVA: 0x0020DB1C File Offset: 0x0020BD1C
		// (set) Token: 0x06004612 RID: 17938 RVA: 0x0020DB24 File Offset: 0x0020BD24
		public int NumDefs { get; private set; }

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06004613 RID: 17939 RVA: 0x0020DB2D File Offset: 0x0020BD2D
		// (set) Token: 0x06004614 RID: 17940 RVA: 0x0020DB35 File Offset: 0x0020BD35
		public int NumBakedAnims { get; private set; }

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06004615 RID: 17941 RVA: 0x0020DB3E File Offset: 0x0020BD3E
		// (set) Token: 0x06004616 RID: 17942 RVA: 0x0020DB46 File Offset: 0x0020BD46
		public int NumTroopsPerSquad { get; private set; }

		// Token: 0x06004617 RID: 17943 RVA: 0x0020DB50 File Offset: 0x0020BD50
		public unsafe void CopyFrom(ref Troops.TroopsMemData data)
		{
			this.CopyCounts(ref data);
			this.defs = (Troops.DefData*)data.defs.GetUnsafePtr<Troops.DefData>();
			this.salvo_defs = (Troops.SalvoDefData*)data.salvo_defs.GetUnsafePtr<Troops.SalvoDefData>();
			this.squads = (Troops.SquadData*)data.squads.GetUnsafePtr<Troops.SquadData>();
			this.fortifications = null;
			this.winner = data.winner;
			this.index_to_baked_id = (int*)data.index_to_baked_id.GetUnsafePtr<int>();
			this.squad = (Troops.SquadData.Ptr*)data.squad.GetUnsafePtr<Troops.SquadData.Ptr>();
			this.drawer_ids = (int2*)data.drawer_ids.GetUnsafePtr<int2>();
			this.drawer_anim_ids = (int*)data.drawer_anim_ids.GetUnsafePtr<int>();
			this.dust_data_buffer = (void*)Troops.texture_baker.dust_drawer.model_data.data;
			this.selection_arrow_data_buffer = (void*)Troops.texture_baker.troop_selection_arrows_drawer.model_data.data;
			this.selection_circle_data_buffer = (void*)Troops.texture_baker.troop_selection_circles_drawer.model_data.data;
			this.model_data_buffer = (void**)data.model_data_buffer.GetUnsafePtr<GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>>();
			this.flags = (int*)data.flags.GetUnsafePtr<int>();
			this.pa_id = (int2*)data.pa_id.GetUnsafePtr<int2>();
			this.pos_rot = (float4*)data.pos_rot.GetUnsafePtr<float4>();
			this.prev_pos_rot = (float4*)data.prev_pos_rot.GetUnsafePtr<float4>();
			this.rot_3d = (float3*)data.rot_3d.GetUnsafePtr<float3>();
			this.previewPositions = (float3*)data.previewPositions.GetUnsafePtr<float3>();
			this.tgt_arrow_rot = (float*)data.tgt_arrow_rot.GetUnsafePtr<float>();
			this.vel_spd_t = (float4*)data.vel_spd_t.GetUnsafePtr<float4>();
			this.steer = (float4*)data.steer.GetUnsafePtr<float4>();
			this.separation = (float4*)data.separation.GetUnsafePtr<float4>();
			this.holdFormationVec = (float2*)data.holdFormationVec.GetUnsafePtr<float2>();
			this.form_idx = (int*)data.form_idx.GetUnsafePtr<int>();
			this.posInFormation = (float2*)data.posInFormation.GetUnsafePtr<float2>();
			this.deform = (float2*)data.deform.GetUnsafePtr<float2>();
			this.tgt_pos = (float2*)data.tgt_pos.GetUnsafePtr<float2>();
			this.anim_idx = (int2*)data.anim_idx.GetUnsafePtr<int2>();
			this.anim_time = (float4*)data.anim_time.GetUnsafePtr<float4>();
			this.attack_cd = (float*)data.attack_cd.GetUnsafePtr<float>();
			this.cooldowns = (float4*)data.cooldowns.GetUnsafePtr<float4>();
			this.enemy_id = (int*)data.enemy_id.GetUnsafePtr<int>();
			this.ranged_enemy_squad_id = (int*)data.ranged_enemy_squad_id.GetUnsafePtr<int>();
			this.is_under_fire = (bool*)data.is_under_fire.GetUnsafePtr<bool>();
			this.fight_status = (byte*)data.fight_status.GetUnsafePtr<byte>();
			this.flanking_status = (bool2*)data.flanking_status.GetUnsafePtr<bool2>();
			this.face_enemies = (bool*)data.face_enemies.GetUnsafePtr<bool>();
			this.throw_time = (float4*)data.throw_args.GetUnsafePtr<float4>();
			this.thrown_dir = (float2*)data.throw_dir.GetUnsafePtr<float2>();
			this.pushOff_args = (float3*)data.pushOff_args.GetUnsafePtr<float3>();
			this.anim_data = (KeyframeTextureBaker.AnimationClipDataBaked*)data.anim_data.GetUnsafePtr<KeyframeTextureBaker.AnimationClipDataBaked>();
			this.baked_anim_count = (int*)data.baked_anim_count.GetUnsafePtr<int>();
			this.texture_width = (float*)data.texture_width.GetUnsafePtr<float>();
			this.anim_result = (float3*)data.anim_result.GetUnsafePtr<float3>();
			this.climb_progress = (float*)data.climb_progress.GetUnsafePtr<float>();
			this.stuck_tgt_pos = (float2*)data.stuck_tgt_pos.GetUnsafePtr<float2>();
			this.stuck_tgt_pa_id = (int*)data.stuck_tgt_pa_id.GetUnsafePtr<int>();
			this.rand = new Unity.Mathematics.Random(1U);
			this.salvos = (Troops.SalvoData*)data.salvos.GetUnsafePtr<Troops.SalvoData>();
			this.salvo = (Troops.SalvoData.Ptr*)data.salvo.GetUnsafePtr<Troops.SalvoData.Ptr>();
			this.arrow_flags = (int*)data.arrow_flags.GetUnsafePtr<int>();
			this.arrow_pos = (float4*)data.arrow_pos.GetUnsafePtr<float4>();
			this.arrow_rot = (float4*)data.arrow_rot.GetUnsafePtr<float4>();
			this.start_pos = (float3*)data.start_pos.GetUnsafePtr<float3>();
			this.mid_pos = (float3*)data.mid_pos.GetUnsafePtr<float3>();
			this.end_pos = (float3*)data.end_pos.GetUnsafePtr<float3>();
			this.is_high_angle = (bool*)data.is_high_angle.GetUnsafePtr<bool>();
			this.target_paid = (int*)data.target_paid.GetUnsafePtr<int>();
			this.start_v = (float2*)data.start_v.GetUnsafePtr<float2>();
			this.arrow_t = (float*)data.arrow_t.GetUnsafePtr<float>();
			this.cur_arrows = (int*)data.cur_arrows.GetUnsafePtr<int>();
			this.arrow_count = (int*)data.arrow_count.GetUnsafePtr<int>();
			this.cur_arrow_troop = (int*)data.cur_arrow_troop.GetUnsafePtr<int>();
			this.arrow_duration = (float*)data.arrow_duration.GetUnsafePtr<float>();
			this.trail_progress = (float*)data.trail_progress.GetUnsafePtr<float>();
			this.dust_particle_positions = (float4*)data.dust_particle_positions.GetUnsafePtr<float4>();
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x0020DFDF File Offset: 0x0020C1DF
		public void CopyCounts(ref Troops.TroopsMemData data)
		{
			this.NumArrows = data.NumArrows;
			this.NumSalvos = data.NumSalvos;
			this.NumDefs = data.NumDefs;
			this.NumBakedAnims = data.NumBakedAnims;
			this.NumTroopsPerSquad = data.TroopsPerSquad;
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x0020E01D File Offset: 0x0020C21D
		public unsafe Troops.DefData* GetDef(int type)
		{
			return this.defs + type;
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x0020E02F File Offset: 0x0020C22F
		public unsafe Troops.SalvoDefData* GetSalvoDef(int type)
		{
			return this.salvo_defs + type;
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x0020E041 File Offset: 0x0020C241
		public unsafe Troops.SquadData* GetSquad(int squad_id)
		{
			return this.squads + squad_id;
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x0020E053 File Offset: 0x0020C253
		public unsafe Troops.FortificationData* GetFortification(int fortification_id)
		{
			return this.fortifications + fortification_id;
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x0020E065 File Offset: 0x0020C265
		public Troops.Troop GetTroop(int thread_id, int troop_id)
		{
			return new Troops.Troop(this.This, thread_id, troop_id);
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x0020E074 File Offset: 0x0020C274
		public unsafe Troops.SalvoData* GetSalvo(int salvo_id)
		{
			return this.salvos + salvo_id;
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x0020E086 File Offset: 0x0020C286
		public Troops.Arrow GetArrow(int thread_id, int arrow_id)
		{
			return new Troops.Arrow(this.This, thread_id, arrow_id);
		}

		// Token: 0x040032BC RID: 12988
		public unsafe Troops.TroopsPtrData* This;

		// Token: 0x040032BD RID: 12989
		public float dt;

		// Token: 0x040032BE RID: 12990
		public unsafe Troops.DefData* defs;

		// Token: 0x040032BF RID: 12991
		public unsafe Troops.SalvoDefData* salvo_defs;

		// Token: 0x040032C0 RID: 12992
		public unsafe Troops.SquadData* squads;

		// Token: 0x040032C1 RID: 12993
		public unsafe Troops.FortificationData* fortifications;

		// Token: 0x040032C2 RID: 12994
		public int winner;

		// Token: 0x040032C3 RID: 12995
		public unsafe int* index_to_baked_id;

		// Token: 0x040032C4 RID: 12996
		public unsafe Troops.SquadData.Ptr* squad;

		// Token: 0x040032C5 RID: 12997
		public unsafe int2* drawer_ids;

		// Token: 0x040032C6 RID: 12998
		public unsafe int* drawer_anim_ids;

		// Token: 0x040032C7 RID: 12999
		public unsafe void* dust_data_buffer;

		// Token: 0x040032C8 RID: 13000
		public unsafe void* selection_arrow_data_buffer;

		// Token: 0x040032C9 RID: 13001
		public unsafe void* selection_circle_data_buffer;

		// Token: 0x040032CA RID: 13002
		public unsafe void** model_data_buffer;

		// Token: 0x040032CB RID: 13003
		public unsafe int* flags;

		// Token: 0x040032CC RID: 13004
		public unsafe int2* pa_id;

		// Token: 0x040032CD RID: 13005
		public unsafe float4* pos_rot;

		// Token: 0x040032CE RID: 13006
		public unsafe float4* prev_pos_rot;

		// Token: 0x040032CF RID: 13007
		public unsafe float3* rot_3d;

		// Token: 0x040032D0 RID: 13008
		public unsafe float3* previewPositions;

		// Token: 0x040032D1 RID: 13009
		public unsafe float* tgt_arrow_rot;

		// Token: 0x040032D2 RID: 13010
		public unsafe float4* vel_spd_t;

		// Token: 0x040032D3 RID: 13011
		public unsafe float4* steer;

		// Token: 0x040032D4 RID: 13012
		public unsafe float4* separation;

		// Token: 0x040032D5 RID: 13013
		public unsafe float2* holdFormationVec;

		// Token: 0x040032D6 RID: 13014
		public unsafe int* form_idx;

		// Token: 0x040032D7 RID: 13015
		public unsafe float2* posInFormation;

		// Token: 0x040032D8 RID: 13016
		public unsafe float2* deform;

		// Token: 0x040032D9 RID: 13017
		public unsafe float2* tgt_pos;

		// Token: 0x040032DA RID: 13018
		public unsafe float* cur_speed;

		// Token: 0x040032DB RID: 13019
		public unsafe int2* anim_idx;

		// Token: 0x040032DC RID: 13020
		public unsafe float4* anim_time;

		// Token: 0x040032DD RID: 13021
		public unsafe float* attack_cd;

		// Token: 0x040032DE RID: 13022
		public unsafe float4* cooldowns;

		// Token: 0x040032DF RID: 13023
		public unsafe int* enemy_id;

		// Token: 0x040032E0 RID: 13024
		public unsafe int* ranged_enemy_squad_id;

		// Token: 0x040032E1 RID: 13025
		public unsafe bool* is_under_fire;

		// Token: 0x040032E2 RID: 13026
		public unsafe byte* fight_status;

		// Token: 0x040032E3 RID: 13027
		public unsafe bool2* flanking_status;

		// Token: 0x040032E4 RID: 13028
		public unsafe bool* face_enemies;

		// Token: 0x040032E5 RID: 13029
		public unsafe float4* throw_time;

		// Token: 0x040032E6 RID: 13030
		public unsafe float2* thrown_dir;

		// Token: 0x040032E7 RID: 13031
		public unsafe float3* pushOff_args;

		// Token: 0x040032E8 RID: 13032
		public unsafe KeyframeTextureBaker.AnimationClipDataBaked* anim_data;

		// Token: 0x040032E9 RID: 13033
		public unsafe int* baked_anim_count;

		// Token: 0x040032EA RID: 13034
		public unsafe float* texture_width;

		// Token: 0x040032EB RID: 13035
		public unsafe float3* anim_result;

		// Token: 0x040032EC RID: 13036
		public unsafe float* climb_progress;

		// Token: 0x040032ED RID: 13037
		public unsafe float2* stuck_tgt_pos;

		// Token: 0x040032EE RID: 13038
		public unsafe int* stuck_tgt_pa_id;

		// Token: 0x040032EF RID: 13039
		public Unity.Mathematics.Random rand;

		// Token: 0x040032F0 RID: 13040
		public unsafe Troops.SalvoData* salvos;

		// Token: 0x040032F1 RID: 13041
		public unsafe Troops.SalvoData.Ptr* salvo;

		// Token: 0x040032F2 RID: 13042
		public unsafe int* arrow_flags;

		// Token: 0x040032F3 RID: 13043
		public unsafe float4* arrow_pos;

		// Token: 0x040032F4 RID: 13044
		public unsafe float4* arrow_rot;

		// Token: 0x040032F5 RID: 13045
		public unsafe float3* start_pos;

		// Token: 0x040032F6 RID: 13046
		public unsafe float3* mid_pos;

		// Token: 0x040032F7 RID: 13047
		public unsafe float3* end_pos;

		// Token: 0x040032F8 RID: 13048
		public unsafe bool* is_high_angle;

		// Token: 0x040032F9 RID: 13049
		public unsafe int* target_paid;

		// Token: 0x040032FA RID: 13050
		public unsafe float2* start_v;

		// Token: 0x040032FB RID: 13051
		public unsafe float* arrow_t;

		// Token: 0x040032FC RID: 13052
		public unsafe int* cur_arrows;

		// Token: 0x040032FD RID: 13053
		public unsafe int* arrow_count;

		// Token: 0x040032FE RID: 13054
		public unsafe int* cur_arrow_troop;

		// Token: 0x040032FF RID: 13055
		public unsafe float* arrow_duration;

		// Token: 0x04003300 RID: 13056
		public unsafe float* trail_progress;

		// Token: 0x04003301 RID: 13057
		public unsafe float4* dust_particle_positions;

		// Token: 0x04003302 RID: 13058
		public unsafe float2* dust_particle_velocity;
	}

	// Token: 0x020005E6 RID: 1510
	public class SquadModelData
	{
		// Token: 0x04003303 RID: 13059
		public List<TextureBaker.PerModelData> per_model_data = new List<TextureBaker.PerModelData>();
	}

	// Token: 0x020005E7 RID: 1511
	public class RagdollInfo
	{
		// Token: 0x06004621 RID: 17953 RVA: 0x0020E0A8 File Offset: 0x0020C2A8
		public void UpdateRigidBodies()
		{
			if (!this.inert && UnityEngine.Time.time - this.spawn_time > 4f)
			{
				Vector3 a = Vector3.zero;
				for (int i = 0; i < this.rigidbodies.Length; i++)
				{
					Rigidbody rigidbody = this.rigidbodies[i];
					Vector3 velocity = rigidbody.velocity;
					a += velocity;
					float magnitude = velocity.magnitude;
					if (magnitude > 0.05f && magnitude > this.max_ragdoll_velocity)
					{
						rigidbody.velocity = this.max_ragdoll_velocity * rigidbody.velocity / magnitude;
					}
				}
				if ((a / (float)this.rigidbodies.Length).magnitude <= 0.5f)
				{
					this.EnableDecal(false);
				}
			}
		}

		// Token: 0x06004622 RID: 17954 RVA: 0x0020E168 File Offset: 0x0020C368
		public void EnableDecal(bool instant)
		{
			if (this.spawned)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			if (this.rigidbodies.Length != 0)
			{
				for (int i = 0; i < this.rigidbodies.Length; i++)
				{
					vector += this.rigidbodies[i].transform.position;
				}
				vector /= (float)this.rigidbodies.Length;
			}
			else
			{
				vector = this.go.transform.position;
			}
			if (!this.was_off_ground)
			{
				TextureBaker.InstancedDecalDrawerBatched decal_drawer = Troops.texture_baker.decal_drawer;
				if (decal_drawer != null)
				{
					decal_drawer.AddDecal(vector, UnityEngine.Random.Range(0f, 360f), this.decal_scale, this.target_alpha, UnityEngine.Random.Range(0, Troops.texture_baker.decal_drawer.atlas_count));
				}
			}
			BattleMap.Get().StartCoroutine(this.RagdollSink(instant || this.was_off_ground));
			this.spawned = true;
		}

		// Token: 0x06004623 RID: 17955 RVA: 0x0020E254 File Offset: 0x0020C454
		public void SetInert()
		{
			this.inert = true;
			for (int i = 0; i < this.rigidbodies.Length; i++)
			{
				this.rigidbodies[i].isKinematic = true;
			}
		}

		// Token: 0x06004624 RID: 17956 RVA: 0x0020E289 File Offset: 0x0020C489
		private IEnumerator RagdollSink(bool instant)
		{
			if (!this.inert)
			{
				this.SetInert();
			}
			if (!instant)
			{
				Troops.old_ragdolls.Add(this);
				float sink_end_time = UnityEngine.Time.time + this.sink_time;
				Vector3 start_pos = this.go.transform.position;
				Vector3 end_pos = start_pos - Vector3.up * this.sink_depth;
				while (UnityEngine.Time.time < sink_end_time)
				{
					yield return new WaitForEndOfFrame();
					if (this.go == null)
					{
						break;
					}
					this.go.transform.position = Vector3.Lerp(start_pos, end_pos, 1f - (sink_end_time - UnityEngine.Time.time) / this.sink_time);
				}
				start_pos = default(Vector3);
				end_pos = default(Vector3);
			}
			if (this.go != null)
			{
				global::Common.DestroyObj(this.go);
				this.go = null;
			}
			Troops.old_ragdolls.Remove(this);
			yield return null;
			yield break;
		}

		// Token: 0x04003304 RID: 13060
		public GameObject go;

		// Token: 0x04003305 RID: 13061
		public Rigidbody[] rigidbodies;

		// Token: 0x04003306 RID: 13062
		public float despawn_time;

		// Token: 0x04003307 RID: 13063
		public float spawn_time;

		// Token: 0x04003308 RID: 13064
		public float sink_depth;

		// Token: 0x04003309 RID: 13065
		public float max_ragdoll_velocity;

		// Token: 0x0400330A RID: 13066
		public bool inert;

		// Token: 0x0400330B RID: 13067
		public bool spawned;

		// Token: 0x0400330C RID: 13068
		public float target_alpha;

		// Token: 0x0400330D RID: 13069
		public float sink_time = 2f;

		// Token: 0x0400330E RID: 13070
		public float decal_appear_time = 2f;

		// Token: 0x0400330F RID: 13071
		public float decal_scale;

		// Token: 0x04003310 RID: 13072
		public bool was_off_ground;
	}

	// Token: 0x020005E8 RID: 1512
	public class SquadDrawCallInfo
	{
		// Token: 0x04003311 RID: 13073
		public TextureBaker.InstancedSkinningDrawer drawer;

		// Token: 0x04003312 RID: 13074
		public TextureBaker.UnitTypeData data;

		// Token: 0x04003313 RID: 13075
		public int local_id;

		// Token: 0x04003314 RID: 13076
		public global::Squad squad;
	}

	// Token: 0x020005E9 RID: 1513
	[BurstCompile]
	private struct SquadJobWrapper<T> : IJobParallelFor where T : struct, Troops.ISquadJob
	{
		// Token: 0x06004627 RID: 17959 RVA: 0x0020E2BD File Offset: 0x0020C4BD
		public unsafe SquadJobWrapper(Troops.TroopsPtrData* data, T job, Troops.SquadsFilter squads_filter, bool call_subsquads)
		{
			this.data = data;
			this.thread_id = -1;
			this.job = job;
			this.squads_filter = squads_filter;
			this.call_subsquads = call_subsquads;
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x0020E2E4 File Offset: 0x0020C4E4
		public unsafe void Execute(int index)
		{
			Troops.SquadData* ptr = this.data->squads + index;
			if (!this.squads_filter.check(ref *ptr) || (!this.call_subsquads && ptr->main_squad_id != -1))
			{
				return;
			}
			ptr->thread_id = this.thread_id;
			this.job.Execute(ref *ptr);
			ptr->thread_id = -1;
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x0020E34C File Offset: 0x0020C54C
		public override string ToString()
		{
			return "SquadJob: " + this.job.GetType().Name;
		}

		// Token: 0x04003315 RID: 13077
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x04003316 RID: 13078
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x04003317 RID: 13079
		public T job;

		// Token: 0x04003318 RID: 13080
		public Troops.SquadsFilter squads_filter;

		// Token: 0x04003319 RID: 13081
		private bool call_subsquads;
	}

	// Token: 0x020005EA RID: 1514
	[BurstCompile]
	private struct TroopJobWrapper<T> : IJobParallelFor where T : struct, Troops.ITroopJob
	{
		// Token: 0x0600462A RID: 17962 RVA: 0x0020E36E File Offset: 0x0020C56E
		public unsafe TroopJobWrapper(Troops.TroopsPtrData* data, T job, Troops.SquadsFilter squads_filter, Troops.TroopsFilter troops_filter, bool call_subsquads)
		{
			this.data = data;
			this.thread_id = -1;
			this.job = job;
			this.squads_filter = squads_filter;
			this.troops_filter = troops_filter;
			this.call_subsquads = call_subsquads;
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x0020E39C File Offset: 0x0020C59C
		public unsafe void Execute(int index)
		{
			Troops.SquadData* ptr = this.data->squads + index;
			if (!this.squads_filter.check(ref *ptr) || (!this.call_subsquads && ptr->main_squad_id != -1))
			{
				return;
			}
			ptr->thread_id = this.thread_id;
			Troops.Troop troop = ptr->FirstTroop;
			while (troop <= ptr->LastTroop)
			{
				if ((!this.call_subsquads || troop.squad_id == ptr->id) && this.troops_filter.check(troop))
				{
					this.job.Execute(troop);
				}
				troop = ++troop;
			}
			ptr->thread_id = -1;
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x0020E447 File Offset: 0x0020C647
		public override string ToString()
		{
			return "TroopJob: " + this.job.GetType().Name;
		}

		// Token: 0x0400331A RID: 13082
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x0400331B RID: 13083
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x0400331C RID: 13084
		public T job;

		// Token: 0x0400331D RID: 13085
		private Troops.SquadsFilter squads_filter;

		// Token: 0x0400331E RID: 13086
		private Troops.TroopsFilter troops_filter;

		// Token: 0x0400331F RID: 13087
		private bool call_subsquads;
	}

	// Token: 0x020005EB RID: 1515
	[BurstCompile]
	private struct SalvoJobWrapper<T> : IJobParallelFor where T : struct, Troops.ISalvoJob
	{
		// Token: 0x0600462D RID: 17965 RVA: 0x0020E469 File Offset: 0x0020C669
		public unsafe SalvoJobWrapper(Troops.TroopsPtrData* data, T job, Troops.SalvosFilter salvos_filter)
		{
			this.data = data;
			this.thread_id = -1;
			this.job = job;
			this.salvos_filter = salvos_filter;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x0020E488 File Offset: 0x0020C688
		public unsafe void Execute(int index)
		{
			Troops.SalvoData* ptr = this.data->salvos + index;
			if (!this.salvos_filter.check(ref *ptr))
			{
				return;
			}
			ptr->thread_id = this.thread_id;
			this.job.Execute(ref *ptr);
			ptr->thread_id = -1;
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x0020E4DF File Offset: 0x0020C6DF
		public override string ToString()
		{
			return "SalvoJob: " + this.job.GetType().Name;
		}

		// Token: 0x04003320 RID: 13088
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x04003321 RID: 13089
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x04003322 RID: 13090
		public T job;

		// Token: 0x04003323 RID: 13091
		public Troops.SalvosFilter salvos_filter;
	}

	// Token: 0x020005EC RID: 1516
	[BurstCompile]
	private struct ArrowJobWrapper<T> : IJobParallelFor where T : struct, Troops.IArrowJob
	{
		// Token: 0x06004630 RID: 17968 RVA: 0x0020E501 File Offset: 0x0020C701
		public unsafe ArrowJobWrapper(Troops.TroopsPtrData* data, T job, Troops.SalvosFilter salvos_filter, Troops.ArrowsFilter arrows_filter)
		{
			this.data = data;
			this.thread_id = -1;
			this.job = job;
			this.salvos_filter = salvos_filter;
			this.arrows_filter = arrows_filter;
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x0020E528 File Offset: 0x0020C728
		public unsafe void Execute(int index)
		{
			Troops.SalvoData* ptr = this.data->salvos + index;
			if (!this.salvos_filter.check(ref *ptr))
			{
				return;
			}
			ptr->thread_id = this.thread_id;
			Troops.Arrow arrow = ptr->FirstArrow;
			while (arrow <= ptr->LastArrow)
			{
				if (this.arrows_filter.check(arrow))
				{
					this.job.Execute(arrow);
				}
				arrow = ++arrow;
			}
			ptr->thread_id = -1;
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x0020E5AB File Offset: 0x0020C7AB
		public override string ToString()
		{
			return "ArrowJob: " + this.job.GetType().Name;
		}

		// Token: 0x04003324 RID: 13092
		[NativeDisableUnsafePtrRestriction]
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x04003325 RID: 13093
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x04003326 RID: 13094
		public T job;

		// Token: 0x04003327 RID: 13095
		private Troops.SalvosFilter salvos_filter;

		// Token: 0x04003328 RID: 13096
		private Troops.ArrowsFilter arrows_filter;
	}

	// Token: 0x020005ED RID: 1517
	private struct DisposeGCHandleJob : IJob
	{
		// Token: 0x06004633 RID: 17971 RVA: 0x0020E5CD File Offset: 0x0020C7CD
		public DisposeGCHandleJob(GCHandle handle)
		{
			this.handle = handle;
		}

		// Token: 0x06004634 RID: 17972 RVA: 0x0020E5D6 File Offset: 0x0020C7D6
		public void Execute()
		{
			this.handle.Free();
		}

		// Token: 0x04003329 RID: 13097
		public GCHandle handle;
	}

	// Token: 0x020005EE RID: 1518
	private struct ActionWrapper : IJob
	{
		// Token: 0x06004635 RID: 17973 RVA: 0x0020E5E3 File Offset: 0x0020C7E3
		public ActionWrapper(Action action)
		{
			this.action_handle = GCHandle.Alloc(action);
			this.thread_id = -1;
		}

		// Token: 0x06004636 RID: 17974 RVA: 0x0020E5F8 File Offset: 0x0020C7F8
		public void Execute()
		{
			((Action)this.action_handle.Target)();
		}

		// Token: 0x06004637 RID: 17975 RVA: 0x0020E60F File Offset: 0x0020C80F
		public override string ToString()
		{
			return "Action: " + ((Action)this.action_handle.Target).Method.ToString();
		}

		// Token: 0x0400332A RID: 13098
		public GCHandle action_handle;

		// Token: 0x0400332B RID: 13099
		[NativeSetThreadIndex]
		public int thread_id;
	}

	// Token: 0x020005EF RID: 1519
	private struct SquadActionWrapper : IJobParallelFor
	{
		// Token: 0x06004638 RID: 17976 RVA: 0x0020E635 File Offset: 0x0020C835
		public SquadActionWrapper(Troops.SquadAction action, Troops.SquadsFilter squads_filter, bool call_subsquads)
		{
			this.action_handle = GCHandle.Alloc(action);
			this.squads_filter = squads_filter;
			this.thread_id = -1;
			this.call_subsquads = call_subsquads;
		}

		// Token: 0x06004639 RID: 17977 RVA: 0x0020E658 File Offset: 0x0020C858
		public unsafe void Execute(int index)
		{
			Troops.SquadData* ptr = Troops.pdata->squads + index;
			if (!this.squads_filter.check(ref *ptr) || (!this.call_subsquads && ptr->main_squad_id != -1))
			{
				return;
			}
			Troops.SquadAction squadAction = (Troops.SquadAction)this.action_handle.Target;
			global::Squad squad = Troops.GetSquad(index);
			if (((squad != null) ? squad.logic : null) == null)
			{
				return;
			}
			if (!squad.logic.IsValid())
			{
				return;
			}
			ptr->thread_id = this.thread_id;
			squadAction(squad);
			ptr->thread_id = -1;
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x0020E6E9 File Offset: 0x0020C8E9
		public override string ToString()
		{
			return "SquadAction: " + ((Troops.SquadAction)this.action_handle.Target).Method.ToString();
		}

		// Token: 0x0400332C RID: 13100
		public GCHandle action_handle;

		// Token: 0x0400332D RID: 13101
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x0400332E RID: 13102
		private Troops.SquadsFilter squads_filter;

		// Token: 0x0400332F RID: 13103
		private bool call_subsquads;
	}

	// Token: 0x020005F0 RID: 1520
	private struct SalvoActionWrapper : IJobParallelFor
	{
		// Token: 0x0600463B RID: 17979 RVA: 0x0020E70F File Offset: 0x0020C90F
		public SalvoActionWrapper(Troops.SalvoAction action, Troops.SalvosFilter salvos_filter)
		{
			this.action_handle = GCHandle.Alloc(action);
			this.salvos_filter = salvos_filter;
			this.thread_id = -1;
		}

		// Token: 0x0600463C RID: 17980 RVA: 0x0020E72C File Offset: 0x0020C92C
		public unsafe void Execute(int index)
		{
			Troops.SalvoData* ptr = Troops.pdata->salvos + index;
			if (!this.salvos_filter.check(ref *ptr))
			{
				return;
			}
			Troops.SalvoAction salvoAction = (Troops.SalvoAction)this.action_handle.Target;
			Troops.Salvo salvo = Troops.GetSalvo(index);
			ptr->thread_id = this.thread_id;
			salvoAction(salvo);
			ptr->thread_id = -1;
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x0020E78D File Offset: 0x0020C98D
		public override string ToString()
		{
			return "SalvoAction: " + ((Troops.SalvoAction)this.action_handle.Target).Method.ToString();
		}

		// Token: 0x04003330 RID: 13104
		public GCHandle action_handle;

		// Token: 0x04003331 RID: 13105
		[NativeSetThreadIndex]
		public int thread_id;

		// Token: 0x04003332 RID: 13106
		private Troops.SalvosFilter salvos_filter;
	}

	// Token: 0x020005F1 RID: 1521
	public static class MTFlags
	{
		// Token: 0x0600463E RID: 17982 RVA: 0x0020E7B3 File Offset: 0x0020C9B3
		public static bool get(int flags, int mask)
		{
			return (flags & mask) != 0;
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x0020E7BC File Offset: 0x0020C9BC
		public static void reset(ref int flags, int value = 0)
		{
			int num;
			do
			{
				num = flags;
			}
			while (num != Interlocked.CompareExchange(ref flags, value, num));
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x0020E7D8 File Offset: 0x0020C9D8
		public static void set(ref int flags, int mask)
		{
			if ((flags & mask) == mask)
			{
				return;
			}
			int num;
			do
			{
				num = flags;
			}
			while (num != Interlocked.CompareExchange(ref flags, num | mask, num));
		}

		// Token: 0x06004641 RID: 17985 RVA: 0x0020E800 File Offset: 0x0020CA00
		public static void clear(ref int flags, int mask)
		{
			if ((flags & mask) == 0)
			{
				return;
			}
			mask = ~mask;
			int num;
			do
			{
				num = flags;
			}
			while (num != Interlocked.CompareExchange(ref flags, num & mask, num));
		}

		// Token: 0x06004642 RID: 17986 RVA: 0x0020E828 File Offset: 0x0020CA28
		public static bool check(int flags, int any_of, int all_of, int none_of)
		{
			return (flags & all_of) == all_of && (any_of == 0 || (flags & any_of) != 0) && (flags & none_of) == 0;
		}
	}

	// Token: 0x020005F2 RID: 1522
	public struct SquadsFilter
	{
		// Token: 0x06004643 RID: 17987 RVA: 0x0020E844 File Offset: 0x0020CA44
		public bool check(ref Troops.SquadData squad)
		{
			return Troops.MTFlags.check((int)squad.flags, (int)this.any_of, (int)this.all_of, (int)this.none_of);
		}

		// Token: 0x04003333 RID: 13107
		public Troops.SquadData.Flags any_of;

		// Token: 0x04003334 RID: 13108
		public Troops.SquadData.Flags all_of;

		// Token: 0x04003335 RID: 13109
		public Troops.SquadData.Flags none_of;
	}

	// Token: 0x020005F3 RID: 1523
	public struct TroopsFilter
	{
		// Token: 0x06004644 RID: 17988 RVA: 0x0020E863 File Offset: 0x0020CA63
		public bool check(Troops.Troop troop)
		{
			return Troops.MTFlags.check((int)troop.flags, (int)this.any_of, (int)this.all_of, (int)this.none_of);
		}

		// Token: 0x04003336 RID: 13110
		public Troops.Troop.Flags any_of;

		// Token: 0x04003337 RID: 13111
		public Troops.Troop.Flags all_of;

		// Token: 0x04003338 RID: 13112
		public Troops.Troop.Flags none_of;
	}

	// Token: 0x020005F4 RID: 1524
	public struct SalvosFilter
	{
		// Token: 0x06004645 RID: 17989 RVA: 0x0020E883 File Offset: 0x0020CA83
		public bool check(ref Troops.SalvoData salvo)
		{
			return Troops.MTFlags.check((int)salvo.flags, (int)this.any_of, (int)this.all_of, (int)this.none_of);
		}

		// Token: 0x04003339 RID: 13113
		public Troops.SalvoData.Flags any_of;

		// Token: 0x0400333A RID: 13114
		public Troops.SalvoData.Flags all_of;

		// Token: 0x0400333B RID: 13115
		public Troops.SalvoData.Flags none_of;
	}

	// Token: 0x020005F5 RID: 1525
	public struct ArrowsFilter
	{
		// Token: 0x06004646 RID: 17990 RVA: 0x0020E8A2 File Offset: 0x0020CAA2
		public bool check(Troops.Arrow arrow)
		{
			return Troops.MTFlags.check((int)arrow.flags, (int)this.any_of, (int)this.all_of, (int)this.none_of);
		}

		// Token: 0x0400333C RID: 13116
		public Troops.Arrow.Flags any_of;

		// Token: 0x0400333D RID: 13117
		public Troops.Arrow.Flags all_of;

		// Token: 0x0400333E RID: 13118
		public Troops.Arrow.Flags none_of;
	}

	// Token: 0x020005F6 RID: 1526
	public struct MoveHistory
	{
		// Token: 0x06004647 RID: 17991 RVA: 0x0020E8C4 File Offset: 0x0020CAC4
		public MoveHistory(ref Troops.SquadData squad)
		{
			for (int i = 0; i < 36; i++)
			{
				this.Set(i, PPos.Zero, PPos.Zero, 0f, 0f, 0f);
			}
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x0020E900 File Offset: 0x0020CB00
		public unsafe PPos Get(int idx)
		{
			idx *= 3;
			fixed (float* ptr = &this.positions.FixedElementField)
			{
				float* ptr2 = ptr;
				return new PPos(ptr2[idx], ptr2[idx + 1], (int)ptr2[idx + 2]);
			}
		}

		// Token: 0x06004649 RID: 17993 RVA: 0x0020E940 File Offset: 0x0020CB40
		public unsafe PPos GetDir(int idx)
		{
			idx *= 2;
			fixed (float* ptr = &this.directions.FixedElementField)
			{
				float* ptr2 = ptr;
				return new PPos(ptr2[idx], ptr2[idx + 1], 0);
			}
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x0020E978 File Offset: 0x0020CB78
		public unsafe float GetLeftLength(int idx)
		{
			fixed (float* ptr = &this.left.FixedElementField)
			{
				return ptr[idx];
			}
		}

		// Token: 0x0600464B RID: 17995 RVA: 0x0020E99C File Offset: 0x0020CB9C
		public unsafe float GetRightLength(int idx)
		{
			fixed (float* ptr = &this.right.FixedElementField)
			{
				return ptr[idx];
			}
		}

		// Token: 0x0600464C RID: 17996 RVA: 0x0020E9C0 File Offset: 0x0020CBC0
		public unsafe float GetPathT(int idx)
		{
			fixed (float* ptr = &this.path_t.FixedElementField)
			{
				return ptr[idx];
			}
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x0020E9E1 File Offset: 0x0020CBE1
		public void Set(int idx, PPos pt)
		{
			this.Set(idx, pt, PPos.Zero, 0f, 0f, 0f);
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x0020EA00 File Offset: 0x0020CC00
		public unsafe void Set(int idx, PPos pt, PPos dir, float t, float left_line, float right_line)
		{
			int num = idx * 3;
			fixed (float* ptr = &this.positions.FixedElementField)
			{
				float* ptr2 = ptr;
				ptr2[num] = pt.x;
				ptr2[num + 1] = pt.y;
				ptr2[num + 2] = (float)pt.paID;
			}
			num = idx * 2;
			fixed (float* ptr = &this.directions.FixedElementField)
			{
				float* ptr3 = ptr;
				ptr3[num] = dir.x;
				ptr3[num + 1] = dir.y;
			}
			fixed (float* ptr = &this.left.FixedElementField)
			{
				ptr[idx] = left_line;
			}
			fixed (float* ptr = &this.right.FixedElementField)
			{
				ptr[idx] = right_line;
			}
			fixed (float* ptr = &this.path_t.FixedElementField)
			{
				ptr[idx] = t;
			}
		}

		// Token: 0x0600464F RID: 17999 RVA: 0x0020EAD0 File Offset: 0x0020CCD0
		private unsafe void Shift()
		{
			fixed (float* ptr = &this.positions.FixedElementField)
			{
				float* ptr2 = ptr;
				UnsafeUtility.MemMove((void*)(ptr2 + 3), (void*)ptr2, 420L);
			}
			fixed (float* ptr = &this.directions.FixedElementField)
			{
				float* ptr3 = ptr;
				UnsafeUtility.MemMove((void*)(ptr3 + 2), (void*)ptr3, 280L);
			}
			fixed (float* ptr = &this.left.FixedElementField)
			{
				float* ptr4 = ptr;
				UnsafeUtility.MemMove((void*)(ptr4 + 1), (void*)ptr4, 140L);
			}
			fixed (float* ptr = &this.right.FixedElementField)
			{
				float* ptr5 = ptr;
				UnsafeUtility.MemMove((void*)(ptr5 + 1), (void*)ptr5, 140L);
			}
			fixed (float* ptr = &this.path_t.FixedElementField)
			{
				float* ptr6 = ptr;
				UnsafeUtility.MemMove((void*)(ptr6 + 1), (void*)ptr6, 140L);
			}
		}

		// Token: 0x06004650 RID: 18000 RVA: 0x0020EB90 File Offset: 0x0020CD90
		public unsafe void Add(PPos pt, PPos dir, float path_t, PassabilityGrid.Data* passability, PathData.DataPointers pointers, Formation.Line line_data, bool forceAdd = false)
		{
			PPos pt2 = this.Get(0);
			PPos pt3 = this.Get(1);
			if (pt == pt2 || pt == pt3)
			{
				return;
			}
			if (this.IsNewPointValid(pt, pointers) || forceAdd)
			{
				this.Shift();
			}
			this.Set(0, pt, dir, path_t, line_data.left, line_data.right);
		}

		// Token: 0x06004651 RID: 18001 RVA: 0x0020EBEC File Offset: 0x0020CDEC
		public bool IsNewPointValid(PPos pt, PathData.DataPointers pointers)
		{
			PPos ppos = this.Get(1);
			float num = pt.SqrDist(ppos);
			PPos ppos2;
			return !pointers.Trace(ppos, pt, out ppos2, false, true, false, PathData.PassableArea.Type.All, -1, false) || ppos == Point.Zero || (pt.paID == 0 && num >= 9f) || ppos.paID != pt.paID;
		}

		// Token: 0x06004652 RID: 18002 RVA: 0x0020EC50 File Offset: 0x0020CE50
		public unsafe bool Trace(PPos from, PPos last_stuck_pos, out PPos result, PathData.DataPointers path_data)
		{
			PPos ppos = this.Get(0);
			if (ppos == Point.Zero)
			{
				result = from;
				return false;
			}
			bool flag = path_data.IsPassable(from);
			PPos ppos2 = Point.Zero;
			float num = float.MaxValue;
			int num2 = 0;
			PPos ppos3 = ppos;
			int i = 1;
			while (i < 36)
			{
				PPos ppos4 = this.Get(i);
				if (ppos4 == Point.Zero)
				{
					break;
				}
				Point pt = ppos4.pos - ppos3.pos;
				float num3 = pt.Normalize();
				float num4 = (from - ppos3).Dot(pt);
				PPos ppos5;
				if (ppos4.paID > 0 && num3 < 2f)
				{
					ppos5 = ppos4;
				}
				else if (num4 <= 0f)
				{
					ppos5 = ppos3;
				}
				else if (num4 >= num3)
				{
					ppos5 = ppos4;
				}
				else
				{
					ppos5 = ppos3 + pt * num4;
				}
				if (from.paID != 0 || ppos5.paID <= 0 || !(ppos5 != last_stuck_pos))
				{
					goto IL_148;
				}
				int num5 = ppos5.paID - 1;
				bool flag2 = false;
				for (int j = 0; j < 4; j++)
				{
					if (path_data.paNormalNodes[num5 * 4 + j].pa_id == 0)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					goto IL_148;
				}
				IL_1E3:
				i++;
				ppos3 = ppos4;
				continue;
				IL_148:
				float num6 = from.SqrDist(ppos5);
				PPos ppos6;
				if (flag && num4 > 0f && path_data.Trace(from, ppos5, out ppos6, false, true, false, PathData.PassableArea.Type.All, -1, false) && path_data.IsNeighbour(ppos5.paID, ppos6.paID) && num6 > 0.5f)
				{
					result = ppos6;
					return true;
				}
				if (num6 < num)
				{
					if (ppos5 == last_stuck_pos)
					{
						if (num6 < 0.3f)
						{
							result = this.Get(i - 1);
							return true;
						}
						if (num6 < 20f)
						{
							result = last_stuck_pos;
							return false;
						}
					}
					ppos2 = ppos5;
					num = num6;
					num2 = i;
					goto IL_1E3;
				}
				goto IL_1E3;
			}
			if (ppos2 == Point.Zero)
			{
				result = ppos;
				return false;
			}
			if (ppos2.SqrDist(last_stuck_pos) < 0.2f || num < 0.2f)
			{
				result = this.Get(num2 - 1);
				return true;
			}
			result = ppos2;
			return true;
		}

		// Token: 0x06004653 RID: 18003 RVA: 0x0020EE9C File Offset: 0x0020D09C
		private PPos OffsetPosition(PPos position, int index, float normalized_distance_to_center)
		{
			PPos pt = this.GetDir(index).GetNormalized().Right(0f);
			float leftLength = this.GetLeftLength(index);
			float rightLength = this.GetRightLength(index);
			if (normalized_distance_to_center > 0f)
			{
				if (rightLength > 2f)
				{
					position += pt * (rightLength * normalized_distance_to_center);
				}
				else if (leftLength > 2f)
				{
					position += pt * (rightLength * -normalized_distance_to_center);
				}
			}
			else if (normalized_distance_to_center < 0f)
			{
				if (leftLength > 2f)
				{
					position += pt * (leftLength * normalized_distance_to_center);
				}
				else if (rightLength > 2f)
				{
					position += pt * (leftLength * -normalized_distance_to_center);
				}
			}
			return position;
		}

		// Token: 0x06004654 RID: 18004 RVA: 0x0020EF54 File Offset: 0x0020D154
		public PPos[] ToArray(PPos[] arrold)
		{
			PPos[] array;
			if (arrold != null)
			{
				array = arrold;
			}
			else
			{
				array = new PPos[36];
			}
			for (int i = 0; i < 36; i++)
			{
				array[i] = this.Get(i);
			}
			return array;
		}

		// Token: 0x0400333F RID: 13119
		public const int size = 36;

		// Token: 0x04003340 RID: 13120
		public const float min_length = 2f;

		// Token: 0x04003341 RID: 13121
		[FixedBuffer(typeof(float), 108)]
		private Troops.MoveHistory.<positions>e__FixedBuffer positions;

		// Token: 0x04003342 RID: 13122
		[FixedBuffer(typeof(float), 36)]
		private Troops.MoveHistory.<path_t>e__FixedBuffer path_t;

		// Token: 0x04003343 RID: 13123
		[FixedBuffer(typeof(float), 108)]
		private Troops.MoveHistory.<directions>e__FixedBuffer directions;

		// Token: 0x04003344 RID: 13124
		[FixedBuffer(typeof(float), 36)]
		private Troops.MoveHistory.<left>e__FixedBuffer left;

		// Token: 0x04003345 RID: 13125
		[FixedBuffer(typeof(float), 36)]
		private Troops.MoveHistory.<right>e__FixedBuffer right;

		// Token: 0x020009E0 RID: 2528
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 432)]
		public struct <positions>e__FixedBuffer
		{
			// Token: 0x040045B5 RID: 17845
			public float FixedElementField;
		}

		// Token: 0x020009E1 RID: 2529
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 144)]
		public struct <path_t>e__FixedBuffer
		{
			// Token: 0x040045B6 RID: 17846
			public float FixedElementField;
		}

		// Token: 0x020009E2 RID: 2530
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 432)]
		public struct <directions>e__FixedBuffer
		{
			// Token: 0x040045B7 RID: 17847
			public float FixedElementField;
		}

		// Token: 0x020009E3 RID: 2531
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 144)]
		public struct <left>e__FixedBuffer
		{
			// Token: 0x040045B8 RID: 17848
			public float FixedElementField;
		}

		// Token: 0x020009E4 RID: 2532
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 144)]
		public struct <right>e__FixedBuffer
		{
			// Token: 0x040045B9 RID: 17849
			public float FixedElementField;
		}
	}

	// Token: 0x020005F7 RID: 1527
	[DebuggerDisplay("[{DebugText}]")]
	public struct Arrow
	{
		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06004655 RID: 18005 RVA: 0x0020EF8C File Offset: 0x0020D18C
		private unsafe int* mt_flags
		{
			get
			{
				return this.data->arrow_flags + this.id;
			}
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06004656 RID: 18006 RVA: 0x0020EFA3 File Offset: 0x0020D1A3
		public unsafe Troops.SalvoData* salvo
		{
			get
			{
				return this.data->salvo[this.id].ptr;
			}
		}

		// Token: 0x06004657 RID: 18007 RVA: 0x0020EFC4 File Offset: 0x0020D1C4
		public unsafe Arrow(Troops.TroopsPtrData* data, int cur_thread_id, int troop_id)
		{
			this.data = data;
			this.cur_thread_id = cur_thread_id;
			this.id = troop_id;
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06004658 RID: 18008 RVA: 0x0020EFDB File Offset: 0x0020D1DB
		public bool valid
		{
			get
			{
				return this.data != null && this.id >= 0 && !this.HasFlags(Troops.Arrow.Flags.Destroyed);
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06004659 RID: 18009 RVA: 0x0020EFFC File Offset: 0x0020D1FC
		public unsafe Troops.Arrow.Flags flags
		{
			get
			{
				return (Troops.Arrow.Flags)(*this.mt_flags);
			}
		}

		// Token: 0x0600465A RID: 18010 RVA: 0x0020F005 File Offset: 0x0020D205
		public unsafe bool HasFlags(Troops.Arrow.Flags mask)
		{
			return Troops.MTFlags.get(*this.mt_flags, (int)mask);
		}

		// Token: 0x0600465B RID: 18011 RVA: 0x0020F014 File Offset: 0x0020D214
		public unsafe void SetFlags(Troops.Arrow.Flags mask)
		{
			Troops.MTFlags.set(ref *this.mt_flags, (int)mask);
		}

		// Token: 0x0600465C RID: 18012 RVA: 0x0020F022 File Offset: 0x0020D222
		public unsafe void ClrFlags(Troops.Arrow.Flags mask)
		{
			Troops.MTFlags.clear(ref *this.mt_flags, (int)mask);
		}

		// Token: 0x0600465D RID: 18013 RVA: 0x0020F030 File Offset: 0x0020D230
		public void Reset()
		{
			this.ClrFlags((Troops.Arrow.Flags)(-1));
		}

		// Token: 0x0600465E RID: 18014 RVA: 0x0020F039 File Offset: 0x0020D239
		public static bool operator !(Troops.Arrow arrow)
		{
			return !arrow.valid;
		}

		// Token: 0x0600465F RID: 18015 RVA: 0x0020F045 File Offset: 0x0020D245
		public static Troops.Arrow operator ++(Troops.Arrow arrow)
		{
			arrow.id++;
			return arrow;
		}

		// Token: 0x06004660 RID: 18016 RVA: 0x0020F054 File Offset: 0x0020D254
		public static bool operator <=(Troops.Arrow t1, Troops.Arrow t2)
		{
			return t1.id <= t2.id;
		}

		// Token: 0x06004661 RID: 18017 RVA: 0x0020F067 File Offset: 0x0020D267
		public static bool operator >=(Troops.Arrow t1, Troops.Arrow t2)
		{
			return t1.id >= t2.id;
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06004662 RID: 18018 RVA: 0x0020F07A File Offset: 0x0020D27A
		// (set) Token: 0x06004663 RID: 18019 RVA: 0x0020F092 File Offset: 0x0020D292
		public unsafe int cur_arrow_troop
		{
			get
			{
				return this.data->cur_arrow_troop[this.id];
			}
			set
			{
				this.data->cur_arrow_troop[this.id] = value;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06004664 RID: 18020 RVA: 0x0020F0AB File Offset: 0x0020D2AB
		// (set) Token: 0x06004665 RID: 18021 RVA: 0x0020F0CC File Offset: 0x0020D2CC
		public unsafe float3 startPos
		{
			get
			{
				return this.data->start_pos[this.id];
			}
			set
			{
				this.data->start_pos[this.id] = value;
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06004666 RID: 18022 RVA: 0x0020F0EE File Offset: 0x0020D2EE
		// (set) Token: 0x06004667 RID: 18023 RVA: 0x0020F10F File Offset: 0x0020D30F
		public unsafe float3 endPos
		{
			get
			{
				return this.data->end_pos[this.id];
			}
			set
			{
				this.data->end_pos[this.id] = value;
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06004668 RID: 18024 RVA: 0x0020F131 File Offset: 0x0020D331
		// (set) Token: 0x06004669 RID: 18025 RVA: 0x0020F152 File Offset: 0x0020D352
		public unsafe float3 midPoint
		{
			get
			{
				return this.data->mid_pos[this.id];
			}
			set
			{
				this.data->mid_pos[this.id] = value;
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x0600466A RID: 18026 RVA: 0x0020F174 File Offset: 0x0020D374
		// (set) Token: 0x0600466B RID: 18027 RVA: 0x0020F189 File Offset: 0x0020D389
		public unsafe bool is_high_angle
		{
			get
			{
				return this.data->is_high_angle[this.id];
			}
			set
			{
				this.data->is_high_angle[this.id] = value;
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x0600466C RID: 18028 RVA: 0x0020F19F File Offset: 0x0020D39F
		// (set) Token: 0x0600466D RID: 18029 RVA: 0x0020F1B7 File Offset: 0x0020D3B7
		public unsafe int target_paid
		{
			get
			{
				return this.data->target_paid[this.id];
			}
			set
			{
				this.data->target_paid[this.id] = value;
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x0600466E RID: 18030 RVA: 0x0020F1D0 File Offset: 0x0020D3D0
		// (set) Token: 0x0600466F RID: 18031 RVA: 0x0020F1F1 File Offset: 0x0020D3F1
		public unsafe float2 startVelocity
		{
			get
			{
				return this.data->start_v[this.id];
			}
			set
			{
				this.data->start_v[this.id] = value;
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06004670 RID: 18032 RVA: 0x0020F213 File Offset: 0x0020D413
		// (set) Token: 0x06004671 RID: 18033 RVA: 0x0020F22B File Offset: 0x0020D42B
		public unsafe float t
		{
			get
			{
				return this.data->arrow_t[this.id];
			}
			set
			{
				this.data->arrow_t[this.id] = value;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06004672 RID: 18034 RVA: 0x0020F244 File Offset: 0x0020D444
		// (set) Token: 0x06004673 RID: 18035 RVA: 0x0020F265 File Offset: 0x0020D465
		public unsafe float3 pos
		{
			get
			{
				return this.data->arrow_pos[this.id].xyz;
			}
			set
			{
				this.data->arrow_pos[this.id].xyz = value;
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06004674 RID: 18036 RVA: 0x0020F287 File Offset: 0x0020D487
		// (set) Token: 0x06004675 RID: 18037 RVA: 0x0020F2A8 File Offset: 0x0020D4A8
		public unsafe float scale
		{
			get
			{
				return this.data->arrow_pos[this.id].w;
			}
			set
			{
				this.data->arrow_pos[this.id].w = value;
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06004676 RID: 18038 RVA: 0x0020F2CA File Offset: 0x0020D4CA
		// (set) Token: 0x06004677 RID: 18039 RVA: 0x0020F2EB File Offset: 0x0020D4EB
		public unsafe float4 rot
		{
			get
			{
				return this.data->arrow_rot[this.id];
			}
			set
			{
				this.data->arrow_rot[this.id] = value;
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06004678 RID: 18040 RVA: 0x0020F30D File Offset: 0x0020D50D
		// (set) Token: 0x06004679 RID: 18041 RVA: 0x0020F325 File Offset: 0x0020D525
		public unsafe float duration
		{
			get
			{
				return this.data->arrow_duration[this.id];
			}
			set
			{
				this.data->arrow_duration[this.id] = value;
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x0600467A RID: 18042 RVA: 0x0020F33E File Offset: 0x0020D53E
		// (set) Token: 0x0600467B RID: 18043 RVA: 0x0020F356 File Offset: 0x0020D556
		public unsafe float trail_progress
		{
			get
			{
				return this.data->trail_progress[this.id];
			}
			set
			{
				this.data->trail_progress[this.id] = value;
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x0600467C RID: 18044 RVA: 0x0020F36F File Offset: 0x0020D56F
		public unsafe GrowBuffer<TextureBaker.InstancedArrowDrawerBatched.DrawCallData> model_data_buffer
		{
			get
			{
				return new GrowBuffer<TextureBaker.InstancedArrowDrawerBatched.DrawCallData>(this.salvo->model_data_buffer);
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x0600467D RID: 18045 RVA: 0x0020F381 File Offset: 0x0020D581
		public unsafe GrowBuffer<TextureBaker.InstancedTrailDrawerBatched.DrawCallData> trail_data_buffer
		{
			get
			{
				return new GrowBuffer<TextureBaker.InstancedTrailDrawerBatched.DrawCallData>(this.salvo->trail_data_buffer);
			}
		}

		// Token: 0x04003346 RID: 13126
		public unsafe Troops.TroopsPtrData* data;

		// Token: 0x04003347 RID: 13127
		public int cur_thread_id;

		// Token: 0x04003348 RID: 13128
		public int id;

		// Token: 0x020009E5 RID: 2533
		[Flags]
		public enum Flags
		{
			// Token: 0x040045BB RID: 17851
			None = 0,
			// Token: 0x040045BC RID: 17852
			Moving = 1,
			// Token: 0x040045BD RID: 17853
			Landed = 2,
			// Token: 0x040045BE RID: 17854
			Destroyed = 4,
			// Token: 0x040045BF RID: 17855
			AboutToShoot = 8,
			// Token: 0x040045C0 RID: 17856
			BeingShot = 16,
			// Token: 0x040045C1 RID: 17857
			FloatingInAir = 32,
			// Token: 0x040045C2 RID: 17858
			Active = 27
		}
	}

	// Token: 0x020005F8 RID: 1528
	[DebuggerDisplay("[{DebugText}]")]
	public struct SalvoData
	{
		// Token: 0x0600467E RID: 18046 RVA: 0x0020F394 File Offset: 0x0020D594
		public unsafe SalvoData(Troops.TroopsPtrData* pdata, int id, int offset, Troops.SalvoDefData* def)
		{
			this.pdata = pdata;
			this.thread_id = -1;
			this.battle_side = 0;
			this.id = id;
			this.offset = offset;
			this.size = 100;
			this.squad = null;
			this.fortification = null;
			this.num_arrows = 0;
			this.def = def;
			this.CTH_final = 0f;
			this.CTH_cavalry_mod = 0f;
			this.friendly_fire_mod = 0f;
			this.mt_flags = 0;
			this.end = 0;
			this.model_data_buffer = null;
			this.trail_data_buffer = (void*)Troops.texture_baker.trail_drawer.model_data.data;
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x0600467F RID: 18047 RVA: 0x0020F441 File Offset: 0x0020D641
		public Troops.Arrow FirstArrow
		{
			get
			{
				return new Troops.Arrow(this.pdata, this.thread_id, this.offset);
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06004680 RID: 18048 RVA: 0x0020F45A File Offset: 0x0020D65A
		public Troops.Arrow LastArrow
		{
			get
			{
				return new Troops.Arrow(this.pdata, this.thread_id, this.offset + this.size - 1);
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06004681 RID: 18049 RVA: 0x0020F47C File Offset: 0x0020D67C
		public Troops.Arrow LastActiveArrow
		{
			get
			{
				return new Troops.Arrow(this.pdata, this.thread_id, this.offset + this.num_arrows - 1);
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06004682 RID: 18050 RVA: 0x0020F49E File Offset: 0x0020D69E
		public Troops.SalvoData.Flags flags
		{
			get
			{
				return (Troops.SalvoData.Flags)this.mt_flags;
			}
		}

		// Token: 0x06004683 RID: 18051 RVA: 0x0020F4A6 File Offset: 0x0020D6A6
		public bool HasFlags(Troops.SalvoData.Flags mask)
		{
			return Troops.MTFlags.get(this.mt_flags, (int)mask);
		}

		// Token: 0x06004684 RID: 18052 RVA: 0x0020F4B4 File Offset: 0x0020D6B4
		public void SetFlags(Troops.SalvoData.Flags mask)
		{
			Troops.MTFlags.set(ref this.mt_flags, (int)mask);
		}

		// Token: 0x06004685 RID: 18053 RVA: 0x0020F4C2 File Offset: 0x0020D6C2
		public void ClrFlags(Troops.SalvoData.Flags mask)
		{
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x06004686 RID: 18054 RVA: 0x0020F4D0 File Offset: 0x0020D6D0
		public void SetFlags(Troops.SalvoData.Flags mask, bool set)
		{
			if (set)
			{
				Troops.MTFlags.set(ref this.mt_flags, (int)mask);
				return;
			}
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x04003349 RID: 13129
		public const int salvo_capacity = 100;

		// Token: 0x0400334A RID: 13130
		public unsafe Troops.TroopsPtrData* pdata;

		// Token: 0x0400334B RID: 13131
		public int thread_id;

		// Token: 0x0400334C RID: 13132
		public int battle_side;

		// Token: 0x0400334D RID: 13133
		public int id;

		// Token: 0x0400334E RID: 13134
		public int offset;

		// Token: 0x0400334F RID: 13135
		public int size;

		// Token: 0x04003350 RID: 13136
		private int mt_flags;

		// Token: 0x04003351 RID: 13137
		public int num_arrows;

		// Token: 0x04003352 RID: 13138
		public float3 end;

		// Token: 0x04003353 RID: 13139
		public float CTH_final;

		// Token: 0x04003354 RID: 13140
		public float CTH_cavalry_mod;

		// Token: 0x04003355 RID: 13141
		public float friendly_fire_mod;

		// Token: 0x04003356 RID: 13142
		public unsafe void* model_data_buffer;

		// Token: 0x04003357 RID: 13143
		public unsafe void* trail_data_buffer;

		// Token: 0x04003358 RID: 13144
		public unsafe Troops.SquadData* squad;

		// Token: 0x04003359 RID: 13145
		public unsafe Troops.FortificationData* fortification;

		// Token: 0x0400335A RID: 13146
		public unsafe Troops.SalvoDefData* def;

		// Token: 0x020009E6 RID: 2534
		public struct Ptr
		{
			// Token: 0x040045C3 RID: 17859
			public unsafe Troops.SalvoData* ptr;
		}

		// Token: 0x020009E7 RID: 2535
		[Flags]
		public enum Flags
		{
			// Token: 0x040045C5 RID: 17861
			None = 0,
			// Token: 0x040045C6 RID: 17862
			Visible = 1,
			// Token: 0x040045C7 RID: 17863
			Moving = 2,
			// Token: 0x040045C8 RID: 17864
			Landed = 4,
			// Token: 0x040045C9 RID: 17865
			HasFinishedActionFrameArrow = 8,
			// Token: 0x040045CA RID: 17866
			Destroyed = 16,
			// Token: 0x040045CB RID: 17867
			HasLandedArrow = 32,
			// Token: 0x040045CC RID: 17868
			Active = 47
		}
	}

	// Token: 0x020005F9 RID: 1529
	[DebuggerDisplay("[{DebugText}]")]
	public struct FortificationData
	{
		// Token: 0x06004687 RID: 18055 RVA: 0x0020F4F0 File Offset: 0x0020D6F0
		public unsafe FortificationData(Troops.TroopsPtrData* pdata, int id, float health, float gate_health, PPos pos, Logic.Fortification.Type type, Vector3 attack_position_a, Vector3 attack_position_b)
		{
			this.pdata = pdata;
			this.thread_id = -1;
			this.battle_side = 0;
			this.id = id;
			this.health = health;
			this.gate_health = gate_health;
			this.mt_flags = 0;
			this.pos = pos;
			this.type = type;
			this.attack_position_a = new float2(attack_position_a.x, attack_position_a.z);
			this.attack_position_b = new float2(attack_position_b.x, attack_position_b.z);
			this.cur_salvo = 0;
			this.last_attacker = null;
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06004688 RID: 18056 RVA: 0x0020F580 File Offset: 0x0020D780
		public Troops.FortificationData.Flags flags
		{
			get
			{
				return (Troops.FortificationData.Flags)this.mt_flags;
			}
		}

		// Token: 0x06004689 RID: 18057 RVA: 0x0020F588 File Offset: 0x0020D788
		public bool HasFlags(Troops.FortificationData.Flags mask)
		{
			return Troops.MTFlags.get(this.mt_flags, (int)mask);
		}

		// Token: 0x0600468A RID: 18058 RVA: 0x0020F596 File Offset: 0x0020D796
		public void SetFlags(Troops.FortificationData.Flags mask)
		{
			Troops.MTFlags.set(ref this.mt_flags, (int)mask);
		}

		// Token: 0x0600468B RID: 18059 RVA: 0x0020F5A4 File Offset: 0x0020D7A4
		public void ClrFlags(Troops.FortificationData.Flags mask)
		{
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x0600468C RID: 18060 RVA: 0x0020F5B2 File Offset: 0x0020D7B2
		public void SetFlags(Troops.FortificationData.Flags mask, bool set)
		{
			if (set)
			{
				Troops.MTFlags.set(ref this.mt_flags, (int)mask);
				return;
			}
			Troops.MTFlags.clear(ref this.mt_flags, (int)mask);
		}

		// Token: 0x0400335B RID: 13147
		public float health;

		// Token: 0x0400335C RID: 13148
		public float gate_health;

		// Token: 0x0400335D RID: 13149
		public unsafe Troops.TroopsPtrData* pdata;

		// Token: 0x0400335E RID: 13150
		public int thread_id;

		// Token: 0x0400335F RID: 13151
		public int battle_side;

		// Token: 0x04003360 RID: 13152
		public int id;

		// Token: 0x04003361 RID: 13153
		public PPos pos;

		// Token: 0x04003362 RID: 13154
		public int cur_salvo;

		// Token: 0x04003363 RID: 13155
		public Logic.Fortification.Type type;

		// Token: 0x04003364 RID: 13156
		private int mt_flags;

		// Token: 0x04003365 RID: 13157
		public float2 attack_position_a;

		// Token: 0x04003366 RID: 13158
		public float2 attack_position_b;

		// Token: 0x04003367 RID: 13159
		public unsafe Troops.SquadData* last_attacker;

		// Token: 0x020009E8 RID: 2536
		public struct Ptr
		{
			// Token: 0x040045CD RID: 17869
			public unsafe Troops.FortificationData* ptr;
		}

		// Token: 0x020009E9 RID: 2537
		[Flags]
		public enum Flags
		{
			// Token: 0x040045CF RID: 17871
			None = 0,
			// Token: 0x040045D0 RID: 17872
			Started = 1,
			// Token: 0x040045D1 RID: 17873
			Hit = 2,
			// Token: 0x040045D2 RID: 17874
			Demolished = 4,
			// Token: 0x040045D3 RID: 17875
			Destroyed = 8,
			// Token: 0x040045D4 RID: 17876
			Shooting = 16,
			// Token: 0x040045D5 RID: 17877
			GateHit = 32,
			// Token: 0x040045D6 RID: 17878
			Active = 21
		}
	}

	// Token: 0x020005FA RID: 1530
	public class Salvo
	{
		// Token: 0x04003368 RID: 13160
		public unsafe Troops.SalvoData* data;
	}

	// Token: 0x020005FB RID: 1531
	public interface ITroopObject
	{
		// Token: 0x0600468E RID: 18062
		bool ReadyToAdd();

		// Token: 0x0600468F RID: 18063
		void AddToTroops();

		// Token: 0x06004690 RID: 18064
		int GetID();

		// Token: 0x06004691 RID: 18065
		void SetID(int id);
	}
}
