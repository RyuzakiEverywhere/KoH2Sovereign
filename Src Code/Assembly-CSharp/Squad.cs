using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Logic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Token: 0x020000CF RID: 207
public class Squad : GameLogic.Behaviour, ISelectable, VisibilityDetector.IVisibilityChanged, Troops.ITroopObject
{
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600099B RID: 2459 RVA: 0x0006D532 File Offset: 0x0006B732
	public Logic.Unit.Def def
	{
		get
		{
			Logic.Squad squad = this.logic;
			if (squad == null)
			{
				return null;
			}
			return squad.def;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600099C RID: 2460 RVA: 0x0006D545 File Offset: 0x0006B745
	public SalvoData.Def salvo_def
	{
		get
		{
			Logic.Squad squad = this.logic;
			if (squad == null)
			{
				return null;
			}
			return squad.salvo_def;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x0600099D RID: 2461 RVA: 0x0006D558 File Offset: 0x0006B758
	public unsafe Troops.SquadData* data
	{
		get
		{
			if (!Troops.Initted || this.data_id < 0)
			{
				return null;
			}
			return Troops.pdata->GetSquad(this.data_id);
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x0600099E RID: 2462 RVA: 0x0006D57D File Offset: 0x0006B77D
	public unsafe Troops.SquadData* main_squad_data
	{
		get
		{
			if (!Troops.Initted || this.main_squad_data_id < 0)
			{
				return null;
			}
			return Troops.pdata->GetSquad(this.main_squad_data_id);
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0006D5A2 File Offset: 0x0006B7A2
	public bool IsVisible()
	{
		return this.visible;
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0006D5AC File Offset: 0x0006B7AC
	public bool ValidForSkirmish()
	{
		if (this.logic.IsDefeated())
		{
			return false;
		}
		if (this.logic.battle.battle_map_finished)
		{
			return false;
		}
		if (this.enemy != null)
		{
			return this.logic.is_fighting || this.logic.is_fighting_target;
		}
		if (this.skirmish != null)
		{
			for (int i = 0; i < this.skirmish.squads.Count; i++)
			{
				global::Squad squad = this.skirmish.squads[i];
				if (squad != this && squad.enemy == this)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0006D657 File Offset: 0x0006B857
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0006D660 File Offset: 0x0006B860
	public static void CreateVisuals(Logic.Object obj)
	{
		Logic.Squad squad = obj as Logic.Squad;
		if (squad == null)
		{
			return;
		}
		global::Squad squad2 = UnityEngine.Object.Instantiate<GameObject>(squad.def.field.GetRandomValue("squad_prefab", null, true, true, true, '.').Get<GameObject>()).GetComponent<global::Squad>();
		if (squad2 == null)
		{
			squad2 = new GameObject(squad.def.name).AddComponent<global::Squad>();
		}
		else
		{
			squad2.name = squad.def.name;
		}
		squad2.kingdom = squad.kingdom_id;
		global::Common.SetObjectParent(squad2.transform.gameObject, GameLogic.instance.transform, "Squads");
		squad2.logic = squad;
		squad.visuals = squad2;
		PPos pt;
		PPos ppos;
		squad.CalcPos(out pt, out ppos, 0f, 0f, false);
		squad2.SetPosition(pt);
		squad2.transform.localEulerAngles = new Vector3(0f, 90f - ppos.Heading(), 0f);
		squad2.RegisterSelectable();
		squad2.last_alive = squad.NumTroops();
		if (global::Squad.control_zone_mesh == null)
		{
			global::Squad.control_zone_mesh = MeshUtils.CreateGridMesh(new Vector3(-0.5f, 0f, -0.5f), 1, 1, Vector3.forward, Vector3.right, false, false);
		}
		squad2.control_zone_go = global::Common.SpawnTemplate("QuadRenderer", "ControlZone", null, true, new Type[]
		{
			typeof(MeshFilter),
			typeof(MeshRenderer)
		});
		global::Common.SetObjectParent(squad2.control_zone_go, GameLogic.instance.transform, "SquadAdditionalObjects");
		squad2.control_zone_go.transform.rotation = squad2.transform.rotation;
		squad2.control_zone_go.transform.position = squad2.transform.position;
		squad2.control_zone_go.GetComponent<MeshFilter>().mesh = global::Squad.control_zone_mesh;
		MeshRenderer component = squad2.control_zone_go.GetComponent<MeshRenderer>();
		component.material = squad2.control_zone_material;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		squad2.control_zone_go.layer = TreesBatching.hide_trees_layer;
		squad2.CreateArrowRangeGO();
		if (squad.def.is_siege_eq)
		{
			GameObject gameObject = squad.def.field.GetRandomValue("unpack_particles", null, true, true, true, '.').Get<GameObject>();
			if (gameObject != null)
			{
				GameObject gameObject2 = global::Common.Spawn(gameObject, squad2.transform, false, "");
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localRotation = Quaternion.identity;
				squad2.unpack_particles = gameObject2.GetComponentsInChildren<ParticleSystem>();
			}
		}
		squad2.ListenToEvents();
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0006D91C File Offset: 0x0006BB1C
	public static GameObject StatusBarPrefab()
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("UIBattleViewSquad", null).FindChild("Compact", null, true, true, true, '.'), "component_prefab", null);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0006D944 File Offset: 0x0006BB44
	public unsafe void UpdateUnpackParticles()
	{
		if (this.logic == null || this.unpack_particles == null || this.data_id < 0)
		{
			return;
		}
		bool flag = this.logic.pack_progress != null && this.logic.pack_progress.GetRate() != 0f;
		float3 banner_pos = this.data->banner_pos;
		for (int i = 0; i < this.unpack_particles.Length; i++)
		{
			ParticleSystem particleSystem = this.unpack_particles[i];
			if (!particleSystem.isPlaying && flag)
			{
				particleSystem.Play();
			}
			else if (particleSystem.isPlaying && !flag)
			{
				particleSystem.Stop();
			}
			particleSystem.transform.position = banner_pos;
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0006D9F5 File Offset: 0x0006BBF5
	public Vector3 StatusBarPos()
	{
		if (this.status_bar == null)
		{
			return base.transform.position;
		}
		return this.status_bar.GetDesiredPosition(false);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0006DA20 File Offset: 0x0006BC20
	private void UpdateStatusbar()
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		if (this.logic.is_main_squad && !this.logic.IsDefeated())
		{
			if (this.status_bar == null)
			{
				BattleViewUI battleViewUI = BattleViewUI.Get();
				RectTransform rectTransform = (battleViewUI != null) ? battleViewUI.m_statusBar : null;
				if (rectTransform == null)
				{
					return;
				}
				GameObject prefab = global::Squad.StatusBarPrefab();
				this.status_bar = global::Common.Spawn(prefab, rectTransform, false, "").GetComponent<UIBattleViewSquad>();
				this.status_bar.UpdateAsCompact(true);
				this.status_bar.SetData(this.logic.simulation);
				this.status_bar.onClick += this.HandleSquadClicked;
				this.status_bar.onRemoved += this.UIBattleViewSquad_OnRemoved;
				this.status_bar.onSquadTypeIconClick += this.UIBattleViewSquad_OnSquadTypeIconClicked;
				this.ApplyFilters();
				return;
			}
		}
		else
		{
			this.ClearStatusBar();
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0006DB24 File Offset: 0x0006BD24
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
			if (e.button == PointerEventData.InputButton.Right)
			{
				this.HandleSquadRightClicked();
				return;
			}
			battleViewUI.SelectObj(visuals.gameObject, false, true, true, true);
			if (e.clickCount > 1 && battleViewUI.selected_obj != null)
			{
				battleViewUI.LookAt(battleViewUI.selected_obj.transform.position, false);
			}
		}
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0006DBB0 File Offset: 0x0006BDB0
	private void HandleSquadRightClicked()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		battleViewUI.DoubleClickCheck();
		List<Logic.Squad> selectedSquads = battleViewUI.GetSelectedSquads();
		for (int i = 0; i < selectedSquads.Count; i++)
		{
			Logic.Squad squad = selectedSquads[i];
			global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
			if (((squad2 != null) ? squad2.logic : null) != null)
			{
				if (squad2.kingdom == battleViewUI.kingdom)
				{
					Logic.Squad squad3 = squad2.logic;
					bool flag;
					if (squad3 == null)
					{
						flag = (null != null);
					}
					else
					{
						BattleSimulation.Squad simulation = squad3.simulation;
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
						goto IL_9B;
					}
				}
				if (!BaseUI.CanControlAI())
				{
					goto IL_D6;
				}
				IL_9B:
				if (squad2.data_formation != null)
				{
					squad2.MoveTo(this.logic.position, this.logic.position.paID, squad2.data_formation.dir, true, false);
				}
			}
			IL_D6:;
		}
		battleViewUI.SpawnClickReticle(this.logic.position);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0006DCB4 File Offset: 0x0006BEB4
	private void UIBattleViewSquad_OnRemoved(UIBattleViewSquad squad)
	{
		if (squad == null)
		{
			return;
		}
		squad.onClick -= this.HandleSquadClicked;
		squad.onRemoved -= this.UIBattleViewSquad_OnRemoved;
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0006DCE4 File Offset: 0x0006BEE4
	private void UIBattleViewSquad_OnSquadTypeIconClicked(UIBattleViewSquad s, PointerEventData e)
	{
		if (s == null || s.SimulationSquadLogic == null || s.SquadLogic == null)
		{
			return;
		}
		global::Squad visuals = s.Visuals;
		if (visuals != null)
		{
			if (e.button == PointerEventData.InputButton.Right)
			{
				this.HandleSquadRightClicked();
				return;
			}
			BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
			battleViewUI.BattleViewSquad_OnSquadTypeIconClicked(visuals.gameObject, false, true, true, true);
			if (e.clickCount > 1 && battleViewUI.selected_obj != null)
			{
				battleViewUI.LookAt(battleViewUI.selected_obj.transform.position, false);
			}
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0006DD70 File Offset: 0x0006BF70
	private unsafe void UpdateControlZoneRenderer()
	{
		if (this.data == null)
		{
			return;
		}
		bool flag = !this.logic.climbing && TreesBatching.hide_camera_enabled;
		if (this.logic.position.paID > 0 && flag)
		{
			PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(this.logic.position.paID - 1);
			if (pa.type == PathData.PassableArea.Type.Wall || pa.type == PathData.PassableArea.Type.Ladder)
			{
				flag = false;
			}
			if (pa.type == PathData.PassableArea.Type.Tower && !this.logic.is_fighting)
			{
				flag = false;
			}
		}
		if (this.logic.enemy_melee_fortification != null && (this.data->is_Fighting_Target || this.data->was_Fighting_Target))
		{
			flag = false;
		}
		if (this.control_zone_go.activeInHierarchy != flag)
		{
			this.control_zone_go.SetActive(flag);
		}
		if (!flag)
		{
			return;
		}
		GameCamera gameCamera = CameraController.GameCamera;
		Vector3 eulerAngles = gameCamera.transform.eulerAngles;
		float num = Mathf.Lerp(45f, 0f, (eulerAngles.x + 20f) / 65f);
		Quaternion quaternion = gameCamera.transform.rotation;
		quaternion *= Quaternion.Euler(90f + num, 0f, 0f);
		float num2 = base.transform.rotation.eulerAngles.y;
		num2 = ((num2 > 0f) ? num2 : (180f + num2));
		num2 += 90f;
		quaternion *= Quaternion.Euler(0f, num2, 0f);
		this.control_zone_go.transform.localScale = new Vector3(this.logic.formation.cur_width + 10f, 1f, this.logic.formation.cur_height + 10f);
		this.control_zone_go.transform.position = this.data->banner_pos;
		this.control_zone_go.transform.rotation = quaternion;
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0006DF88 File Offset: 0x0006C188
	private void ClearStatusBar()
	{
		if (this.status_bar == null)
		{
			return;
		}
		this.status_bar.onClick -= this.HandleSquadClicked;
		this.status_bar.onRemoved -= this.UIBattleViewSquad_OnRemoved;
		this.status_bar.onSquadTypeIconClick -= this.UIBattleViewSquad_OnSquadTypeIconClicked;
		global::Common.DestroyObj(this.status_bar.gameObject);
		this.status_bar = null;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0006E000 File Offset: 0x0006C200
	private void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.logic != null || GameLogic.in_create_visuals)
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle != null && battle.battle_map_only)
		{
			global::Squad.Spawn((this.kingdom == 0) ? 0 : 1, this.troops_def_id, base.transform.position, true);
		}
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0006E06C File Offset: 0x0006C26C
	private void OnDestroy()
	{
		if (this.control_zone_go != null)
		{
			global::Common.DestroyObj(this.control_zone_go);
			this.control_zone_go = null;
		}
		this.StopListeningToEvents();
		this.ClearStatusBar();
		this.ClearArrowRangeGO();
		this.UpdateAliveTroops();
		this.UpdateSoundLoop(true);
		this.DelMinimap(false);
		if (this.skirmish != null)
		{
			this.skirmish.DelSquad(this, true);
		}
		if (this.main_squad_data_id != -1)
		{
			if (Troops.squads != null)
			{
				global::Squad squad = Troops.GetSquad(this.main_squad_data_id);
				if (squad != null)
				{
					this.MoveAllTroopsToSquad(squad);
					this.RefreshMainSquadTroopsCounters(squad);
					if (squad.subsquads_data_ids.Contains(this.data_id))
					{
						squad.subsquads_data_ids.Remove(this.data_id);
					}
				}
			}
		}
		else
		{
			if (Troops.squads != null)
			{
				for (int i = 0; i < this.subsquads_data_ids.Count; i++)
				{
					global::Squad squad2 = Troops.GetSquad(this.subsquads_data_ids[i]);
					if (squad2 != null && squad2.logic != null && squad2.logic.IsValid())
					{
						squad2.logic.Destroy(false);
					}
				}
			}
			this.subsquads_data_ids.Clear();
		}
		Troops.DelSquad(this);
		if (this.visibility_index >= 0)
		{
			VisibilityDetector.Del("BattleView", this.visibility_index);
			this.visibility_index = -1;
		}
		this.UnregisterSelectable();
		this.DestroyPathArrows();
		this.DestroyPathArrowsStraight();
		if (this.squad_banner != null)
		{
			UnityEngine.Object.Destroy(this.squad_banner.gameObject);
		}
		if (this.logic != null && this.logic.IsValid())
		{
			if (this.logic.simulation != null && !this.logic.simulation.IsDefeated())
			{
				this.logic.simulation.SetState(BattleSimulation.Squad.State.Left, null, -1f);
			}
			Logic.Object @object = this.logic;
			this.logic = null;
			@object.Destroy(false);
		}
		FormationPool.Return(ref this.data_formation);
		FormationPool.Return(ref this.wall_climb_dest_formation);
		FormationPool.Return(ref this.wall_climb_start_formation);
		FormationPool.Return(ref this.previewFormation);
		FormationPool.Return(ref this.previewFormationConfirmed);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0006E284 File Offset: 0x0006C484
	private void CreateArrowRangeGO()
	{
		if (this.arrow_range_go != null)
		{
			return;
		}
		if (this.arrow_range_material == null)
		{
			return;
		}
		if (!this.logic.def.is_ranged)
		{
			return;
		}
		Mesh mesh = MeshUtils.CreateCircle(this.logic.simulation.unit.GetVar("max_shoot_range", null, true), -1f, 64);
		Mesh mesh2 = MeshUtils.CreateCircle(this.salvo_def.min_shoot_range, 1f, 64);
		CombineInstance[] array = new CombineInstance[2];
		array[0].mesh = mesh;
		array[0].transform = Matrix4x4.identity;
		array[1].mesh = mesh2;
		array[1].transform = Matrix4x4.identity * Matrix4x4.Rotate(Quaternion.Euler(180f, 0f, 0f));
		Mesh mesh3 = new Mesh();
		mesh3.CombineMeshes(array);
		global::Common.DestroyObj(mesh2);
		global::Common.DestroyObj(mesh);
		this.arrow_range_go = global::Common.SpawnTemplate("arrow_range_indicator", "ArrowRange", null, true, new Type[]
		{
			typeof(MeshRenderer),
			typeof(MeshFilter)
		});
		global::Common.SetObjectParent(this.arrow_range_go, GameLogic.instance.transform, "ArrowRanges");
		MeshRenderer component = this.arrow_range_go.GetComponent<MeshRenderer>();
		component.sharedMaterial = this.arrow_range_material;
		EmissionRenderer.AddRenderer(component);
		this.arrow_range_go.GetComponent<MeshFilter>().sharedMesh = mesh3;
		this.UpdateArrowRangeGO();
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0006E406 File Offset: 0x0006C606
	private void ClearArrowRangeGO()
	{
		if (this.arrow_range_go == null)
		{
			return;
		}
		global::Common.DestroyObj(this.arrow_range_go);
		this.arrow_range_go = null;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0006E42C File Offset: 0x0006C62C
	public unsafe void UpdateArrowRangeGO()
	{
		if (this.arrow_range_go == null)
		{
			return;
		}
		this.arrow_range_go.SetActive(this.Selected && this.m_isShootingRangeIndicatorVisibleFilter && this.logic.salvos_left > 0);
		if (!this.Selected || this.GetMainSquadID() != -1)
		{
			return;
		}
		this.arrow_range_go.GetComponent<MeshFilter>();
		if (this.data != null)
		{
			this.arrow_range_go.transform.position = this.data->banner_pos;
			this.arrow_range_go.transform.rotation = Quaternion.identity;
		}
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0006E4D4 File Offset: 0x0006C6D4
	private unsafe void MoveAllTroopsToSquad(global::Squad squad)
	{
		Troops.SquadData* data = squad.data;
		Troops.SquadData.Ptr* ptr = Troops.pdata->squad + data->offset;
		Troops.Troop troop = data->FirstTroop;
		while (troop <= data->LastTroop)
		{
			if (troop.squad_id == this.GetID())
			{
				ptr->ptr = data;
				troop.SetDrawerInfo(squad, true);
			}
			ptr++;
			troop = ++troop;
		}
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0006E548 File Offset: 0x0006C748
	private unsafe void RefreshMainSquadTroopsCounters(global::Squad main_squad)
	{
		int allSquadsAliveTroops = main_squad.GetAllSquadsAliveTroops();
		main_squad.GetAllSquadsNumTroops();
		if (main_squad.data->logic_alive != allSquadsAliveTroops)
		{
			main_squad.data->logic_alive = allSquadsAliveTroops;
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0006E580 File Offset: 0x0006C780
	public unsafe override void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2810294840U)
		{
			if (num <= 1842288223U)
			{
				if (num <= 1211309691U)
				{
					if (num != 391992465U)
					{
						if (num != 1211309691U)
						{
							return;
						}
						if (!(message == "destroying"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "check_salvo_passability"))
						{
							return;
						}
						if (param == null)
						{
							this.ranged_enemy = null;
							this.logic.arrow_path_is_clear = false;
							return;
						}
						this.ranged_enemy = (param as MapObject);
						if (this.ranged_enemy == null)
						{
							this.logic.arrow_path_is_clear = false;
							return;
						}
						Logic.Squad squad = this.ranged_enemy as Logic.Squad;
						PPos position;
						if (squad != null)
						{
							float num2 = (squad.movement.speed + 2f) / squad.movement.speed;
							PPos ppos;
							squad.CalcPos(out position, out ppos, num2, num2, true);
						}
						else
						{
							position = this.ranged_enemy.position;
						}
						this.logic.arrow_path_is_clear = this.CheckSalvoPassability(position, squad, true);
						return;
					}
				}
				else if (num != 1649643086U)
				{
					if (num != 1842288223U)
					{
						return;
					}
					if (!(message == "defeated"))
					{
						return;
					}
					if (this.Selected)
					{
						BattleViewUI.Get().Deselect(this);
					}
					bool flag = true;
					Troops.RefreshDrawers(this);
					if ((bool)param)
					{
						flag = !Troops.CheckSwapMainSquad(this);
					}
					if (this.logic.simulation.state == BattleSimulation.Squad.State.Stuck)
					{
						Troops.DisableStuckAreas(this);
					}
					if (flag)
					{
						this.UnregisterSelectable();
						this.UnregisterMinimapIcon();
						if (this.IsVisible() && this.logic.simulation.state == BattleSimulation.Squad.State.Fled && this.logic.is_main_squad)
						{
							Logic.Squad squad2 = null;
							float num3 = float.MaxValue;
							List<Logic.Squad> list = this.logic.battle.squads.Get(1 - this.logic.battle_side);
							for (int i = 0; i < list.Count; i++)
							{
								Logic.Squad squad3 = list[i];
								global::Squad squad4 = squad3.visuals as global::Squad;
								if (!(squad4 == null) && squad4.IsVisible())
								{
									float num4 = squad3.position.SqrDist(this.logic.position);
									if (num4 < num3)
									{
										num3 = num4;
										squad2 = squad3;
									}
								}
							}
							if (squad2 != null)
							{
								BaseUI.PlayVoiceEvent(squad2.def.enemy_flees_voice_line, squad2, global::Common.SnapToTerrain(squad2.VisualPosition(), 0f, null, -1f, false));
							}
						}
					}
					return;
				}
				else if (!(message == "finishing"))
				{
					return;
				}
				this.UnregisterMinimapIcon();
				global::Common.DestroyObj(base.gameObject);
				return;
			}
			if (num <= 2016096567U)
			{
				if (num != 1902263116U)
				{
					if (num != 2016096567U)
					{
						return;
					}
					if (!(message == "stop_climb"))
					{
						return;
					}
					this.data->ClrFlags(Troops.SquadData.Flags.HasTroopClimbedLadder);
					Troops.Troop troop = this.data->FirstTroop;
					while (troop <= this.data->LastTroop)
					{
						troop.ClrFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.ClimbingLadderFinished | Troops.Troop.Flags.ClimbingLadderWaiting);
						troop = ++troop;
					}
					return;
				}
				else
				{
					if (!(message == "flanked"))
					{
						return;
					}
					if (!this.logic.is_main_squad)
					{
						return;
					}
					if (this.kingdom != BaseUI.LogicKingdom().id)
					{
						return;
					}
					if (!this.IsVisible())
					{
						return;
					}
					BaseUI.PlayVoiceEvent(this.def.flanked_voice_line, this.logic, global::Common.SnapToTerrain(this.logic.VisualPosition(), 0f, null, -1f, false));
					return;
				}
			}
			else if (num != 2111097090U)
			{
				if (num != 2757266137U)
				{
					if (num != 2810294840U)
					{
						return;
					}
					if (!(message == "enemy_changed"))
					{
						return;
					}
					if (this.logic.enemy_squad == null)
					{
						this.enemy = null;
						return;
					}
					this.enemy = (this.logic.enemy_squad.visuals as global::Squad);
					return;
				}
				else
				{
					if (!(message == "marked_as_target_changed"))
					{
						return;
					}
					if (this.logic.marked_as_target)
					{
						BaseUI.PlayVoiceEvent(this.def.marked_as_target_voice_line, this.logic, global::Common.SnapToTerrain(this.logic.VisualPosition(), 0f, null, -1f, false));
						return;
					}
					BaseUI.PlayVoiceEvent(this.def.unmarked_as_target_voice_line, this.logic, global::Common.SnapToTerrain(this.logic.VisualPosition(), 0f, null, -1f, false));
					return;
				}
			}
			else
			{
				if (!(message == "started_packing"))
				{
					return;
				}
				this.StartPackingSound();
				return;
			}
		}
		else if (num <= 3021313648U)
		{
			if (num <= 2863135946U)
			{
				if (num != 2850358207U)
				{
					if (num != 2863135946U)
					{
						return;
					}
					if (!(message == "under_fire"))
					{
						return;
					}
					if (this.kingdom != BaseUI.LogicKingdom().id)
					{
						return;
					}
					BaseUI.PlayVoiceEvent(this.def.under_fire_voice_line, this.logic, global::Common.SnapToTerrain(this.logic.VisualPosition(), 0f, null, -1f, false));
					return;
				}
				else
				{
					if (!(message == "teleported"))
					{
						return;
					}
					if (this.data == null)
					{
						return;
					}
					this.data->SetFlags(Troops.SquadData.Flags.Teleport);
					this.UpdateInsideWallsValues();
					return;
				}
			}
			else if (num != 2920394832U)
			{
				if (num != 3021313648U)
				{
					return;
				}
				if (!(message == "shoot"))
				{
					return;
				}
				if (param == null)
				{
					this.ranged_enemy = null;
					return;
				}
				this.ranged_enemy = (param as MapObject);
				if (this.ranged_enemy == null)
				{
					return;
				}
				Logic.Squad squad5 = this.ranged_enemy as Logic.Squad;
				PPos position2;
				if (squad5 != null)
				{
					float num5 = (squad5.movement.speed + 2f) / squad5.movement.speed;
					PPos ppos2;
					squad5.CalcPos(out position2, out ppos2, num5, num5, true);
				}
				else
				{
					position2 = this.ranged_enemy.position;
				}
				this.ShootArrows(position2, this.ranged_enemy);
				return;
			}
			else
			{
				if (!(message == "moved"))
				{
					return;
				}
				SquadPowerGrid squadPowerGrid = this.logic.battle.power_grids[this.logic.battle_side];
				if (squadPowerGrid != null)
				{
					squadPowerGrid.Recalculate(this.logic.battle, this.logic.battle_side);
				}
				Movement movement = this.logic.movement;
				object obj2;
				if (movement == null)
				{
					obj2 = null;
				}
				else
				{
					Path path = movement.path;
					obj2 = ((path != null) ? path.dst_obj : null);
				}
				Logic.Squad squad6 = obj2 as Logic.Squad;
				if (this.logic.def.is_cavalry && squad6 != null && this.IsVisible() && this.logic.is_main_squad && squad6.battle_side == global::Battle.PlayerBattleSide())
				{
					float num6 = squad6.GetRadius() + this.logic.GetRadius();
					float num7 = this.logic.position.Dist(this.logic.movement.path.dst_pt);
					if (num7 < num6 * 2f && num7 > num6 * 1f)
					{
						BaseUI.PlayVoiceEvent(squad6.def.enemy_cavalry_attacks_us_voice_line, squad6, global::Common.SnapToTerrain(squad6.VisualPosition(), 0f, null, -1f, false));
					}
				}
				return;
			}
		}
		else if (num <= 3663550303U)
		{
			if (num != 3380500069U)
			{
				if (num != 3663550303U)
				{
					return;
				}
				if (!(message == "fleeing"))
				{
					return;
				}
				if (this.logic.simulation.state == BattleSimulation.Squad.State.Fled)
				{
					if (this.logic.GetKingdom() == BaseUI.LogicKingdom())
					{
						BaseUI.PlayVoiceEvent("narrator_voice:unit_fleeing", null);
						return;
					}
				}
				else if (this.logic.simulation.state == BattleSimulation.Squad.State.Retreating)
				{
					this.PlayVoiceLine(global::Squad.VoiceCommand.Retreat, global::Common.SnapToTerrain(this.logic.VisualPosition(), 0f, null, -1f, false), "FloatingTexts.Normal");
				}
				return;
			}
			else
			{
				if (!(message == "refresh_drawers"))
				{
					return;
				}
				Troops.RefreshDrawers(this);
				return;
			}
		}
		else if (num != 3702845995U)
		{
			if (num != 3761154513U)
			{
				if (num != 3997142699U)
				{
					return;
				}
				if (!(message == "bv_loaded"))
				{
					return;
				}
				this.StopListeningToEvents();
				this.ListenToEvents();
				return;
			}
			else
			{
				if (!(message == "path_changed"))
				{
					return;
				}
				if (this.Selected || this.Previewed)
				{
					this.CreatePathVisualisation();
					this.PreviewDirty = true;
				}
				return;
			}
		}
		else
		{
			if (!(message == "finished_packing"))
			{
				return;
			}
			this.StopPackingSound();
			return;
		}
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0006EE10 File Offset: 0x0006D010
	private void ClearControlGroup()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI == null)
		{
			return;
		}
		for (int i = 0; i < this.control_groups.Count; i++)
		{
			int groupIndex = this.control_groups[i];
			battleViewUI.RemoveSquadFromGroup(this.logic, groupIndex);
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0006EE5D File Offset: 0x0006D05D
	// (set) Token: 0x060009B7 RID: 2487 RVA: 0x0006EE65 File Offset: 0x0006D065
	public bool TroopDied { get; private set; }

	// Token: 0x060009B8 RID: 2488 RVA: 0x0006EE70 File Offset: 0x0006D070
	private void UpdateAliveTroops()
	{
		this.TroopDied = false;
		if (this.logic == null)
		{
			return;
		}
		int num = this.logic.NumTroops();
		if (this.last_alive > num)
		{
			this.last_alive = num;
			this.TroopDied = true;
			this.logic.NotifyListeners("troop_died", null);
		}
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0006EEC4 File Offset: 0x0006D0C4
	private void InitSoundLoop()
	{
		if (this.initted_sound_loops)
		{
			return;
		}
		if (BattleMap.battle != null && BattleMap.battle.stage == Logic.Battle.Stage.EnteringBattle)
		{
			return;
		}
		this.initted_sound_loops = true;
		if (this.logic.IsValid() && this.marching_sound_emitter == null)
		{
			this.marching_sound_emitter = base.gameObject.AddComponent<StudioEventEmitter>();
			this.marching_sound_emitter.StopEvent = EmitterGameEvent.ObjectDestroy;
		}
		ATTRIBUTES_3D attributes = default(ATTRIBUTES_3D);
		attributes.position = base.transform.position.ToFMODVector();
		attributes.up = Vector3.up.ToFMODVector();
		attributes.forward = Vector3.forward.ToFMODVector();
		if (!string.IsNullOrEmpty(this.def.DyingSoundLoop))
		{
			this.dying_sound = FMODWrapper.CreateInstance(this.def.DyingSoundLoop, false);
			this.dying_sound.set3DAttributes(attributes);
		}
		if (this.def.is_cavalry && !string.IsNullOrEmpty(this.def.DyingSoundHorsesLoop))
		{
			this.dying_sound_horses = FMODWrapper.CreateInstance(this.def.DyingSoundHorsesLoop, false);
			this.dying_sound_horses.set3DAttributes(attributes);
		}
		if (!string.IsNullOrEmpty(this.def.ChargingSoundLoop))
		{
			this.charge_sound = FMODWrapper.CreateInstance(this.def.ChargingSoundLoop, false);
			this.charge_sound.set3DAttributes(attributes);
		}
		SalvoData.Def salvo_def = this.salvo_def;
		if (!string.IsNullOrEmpty((salvo_def != null) ? salvo_def.reload_sound_effect : null))
		{
			this.reload_sound = FMODWrapper.CreateInstance(this.salvo_def.reload_sound_effect, false);
			this.reload_sound.set3DAttributes(attributes);
		}
		SalvoData.Def salvo_def2 = this.salvo_def;
		if (!string.IsNullOrEmpty((salvo_def2 != null) ? salvo_def2.release_sound_effect : null))
		{
			this.release_sound = FMODWrapper.CreateInstance(this.salvo_def.release_sound_effect, false);
			this.release_sound.set3DAttributes(attributes);
		}
		if (!string.IsNullOrEmpty(this.def.PackingSoundLoop))
		{
			this.packing_sound = FMODWrapper.CreateInstance(this.def.PackingSoundLoop, false);
			this.packing_sound.set3DAttributes(attributes);
		}
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0006F0D0 File Offset: 0x0006D2D0
	public unsafe void PlayReloadSound()
	{
		if (this.release_sound.isValid())
		{
			ATTRIBUTES_3D attributes;
			if (this.release_sound.get3DAttributes(out attributes) == RESULT.OK)
			{
				attributes.position = this.data->banner_pos.ToFMODVector();
				this.release_sound.set3DAttributes(attributes);
			}
			this.release_sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			this.release_sound.start();
		}
		if (this.reload_sound.isValid())
		{
			this.reload_sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			this.next_reload_sound = this.salvo_def.reload_sound_delay + UnityEngine.Time.time;
		}
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0006F16C File Offset: 0x0006D36C
	private unsafe void CheckReloadSound()
	{
		if (this.next_reload_sound < 0f || UnityEngine.Time.time < this.next_reload_sound)
		{
			return;
		}
		this.next_reload_sound = -1f;
		ATTRIBUTES_3D attributes;
		if (this.reload_sound.get3DAttributes(out attributes) == RESULT.OK)
		{
			attributes.position = this.data->banner_pos.ToFMODVector();
			this.reload_sound.set3DAttributes(attributes);
		}
		this.reload_sound.start();
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0006F1E4 File Offset: 0x0006D3E4
	private void PlayDyingSound()
	{
		PLAYBACK_STATE playback_STATE;
		ATTRIBUTES_3D attributes;
		if (this.dying_sound.isValid() && this.dying_sound.getPlaybackState(out playback_STATE) == RESULT.OK && playback_STATE == PLAYBACK_STATE.STOPPED && this.dying_sound.get3DAttributes(out attributes) == RESULT.OK)
		{
			attributes.position = base.transform.position.ToFMODVector();
			this.dying_sound.set3DAttributes(attributes);
			this.dying_sound.start();
		}
		PLAYBACK_STATE playback_STATE2;
		ATTRIBUTES_3D attributes2;
		if (this.dying_sound_horses.isValid() && this.dying_sound_horses.getPlaybackState(out playback_STATE2) == RESULT.OK && playback_STATE2 == PLAYBACK_STATE.STOPPED && this.dying_sound_horses.get3DAttributes(out attributes2) == RESULT.OK)
		{
			attributes2.position = base.transform.position.ToFMODVector();
			this.dying_sound_horses.set3DAttributes(attributes2);
			this.dying_sound_horses.start();
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0006F2B0 File Offset: 0x0006D4B0
	private unsafe void PlayCharge()
	{
		PLAYBACK_STATE playback_STATE;
		ATTRIBUTES_3D attributes;
		if (this.charge_sound.isValid() && this.charge_sound.getPlaybackState(out playback_STATE) == RESULT.OK && playback_STATE == PLAYBACK_STATE.STOPPED && this.charge_sound.get3DAttributes(out attributes) == RESULT.OK)
		{
			attributes.position = this.data->banner_pos.ToFMODVector();
			this.charge_sound.set3DAttributes(attributes);
			this.charge_sound.setParameterByName("NumCharge", (float)this.last_alive, false);
			this.charge_sound.start();
		}
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0006F33C File Offset: 0x0006D53C
	public unsafe void StartPackingSound()
	{
		if (!this.packing_sound.isValid())
		{
			return;
		}
		ATTRIBUTES_3D attributes;
		if (this.packing_sound.get3DAttributes(out attributes) == RESULT.OK)
		{
			attributes.position = this.data->banner_pos.ToFMODVector();
			this.packing_sound.set3DAttributes(attributes);
		}
		this.packing_sound.setParameterByName("running", 1f, false);
		this.packing_sound.start();
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0006F3B2 File Offset: 0x0006D5B2
	public void StopPackingSound()
	{
		this.packing_sound.setParameterByName("running", 0f, false);
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0006F3CC File Offset: 0x0006D5CC
	public unsafe void UpdateSoundLoop(bool destroying = false)
	{
		if (this.data == null)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.battle == null)
		{
			return;
		}
		this.InitSoundLoop();
		if (this.marching_sound_emitter == null)
		{
			return;
		}
		if (this.TroopDied && !this.logic.battle.IsFinishing() && !this.logic.battle.battle_map_finished)
		{
			this.PlayDyingSound();
			if (this.logic.def.type == Logic.Unit.Type.Noble && this.logic.IsOwnStance(BaseUI.LogicKingdom()))
			{
				BaseUI.PlayVoiceEvent(this.logic.def.units_died_voice_line, this.logic, this.data->banner_pos);
			}
		}
		if (destroying)
		{
			if (this.dying_sound.isValid())
			{
				this.dying_sound.release();
			}
			if (this.charge_sound.isValid())
			{
				this.charge_sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.charge_sound.release();
			}
			if (this.dying_sound_horses.isValid())
			{
				this.dying_sound_horses.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.dying_sound_horses.release();
			}
			if (this.reload_sound.isValid())
			{
				this.reload_sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.reload_sound.release();
			}
			if (this.release_sound.isValid())
			{
				this.release_sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.release_sound.release();
			}
			if (this.packing_sound.isValid())
			{
				this.packing_sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.packing_sound.release();
			}
			return;
		}
		this.CheckReloadSound();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = *(ref this.data->animation_counts.FixedElementField + (IntPtr)6 * 4);
		int num5 = *(ref this.data->animation_counts.FixedElementField + (IntPtr)21 * 4);
		int num6 = 0;
		for (int i = 0; i < Troops.UpdateTroopAnimationsJob.shoot_animations.Length; i++)
		{
			UnitAnimation.State state = Troops.UpdateTroopAnimationsJob.shoot_animations[i];
			num6 += *(ref this.data->animation_counts.FixedElementField + (IntPtr)state * 4);
		}
		Logic.Squad squad = this.logic.enemy_squad;
		if (squad != null && squad.IsDefeated())
		{
			squad = null;
		}
		float trot_anim_speed = this.data->def->trot_anim_speed;
		float run_anim_speed = this.data->def->run_anim_speed;
		float sprint_anim_speed = this.data->def->sprint_anim_speed;
		float num7 = Mathf.Lerp(trot_anim_speed, run_anim_speed, this.data->def->trot_to_run_ratio);
		Troops.DefData defData = *this.data->def;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (troop.squad != null && !troop.HasFlags(Troops.Troop.Flags.Collided | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == this.GetID() && (troop.cur_anim_state == UnitAnimation.State.Move || troop.cur_anim_state == UnitAnimation.State.Trot || troop.cur_anim_state == UnitAnimation.State.Sprint || troop.cur_anim_state == UnitAnimation.State.Charge || troop.cur_anim_state == UnitAnimation.State.SpecialAttackMovingFront || troop.cur_anim_state == UnitAnimation.State.SpecialAttackMovingLeft || troop.cur_anim_state == UnitAnimation.State.SpecialAttackMovingRear || troop.cur_anim_state == UnitAnimation.State.SpecialAttackMovingRight || this.logic.IsMoving()))
			{
				num++;
				if (troop.vel_spd_t.z >= num7)
				{
					num2++;
					num--;
				}
				if (num2 > 0 && troop.HasFlags(Troops.Troop.Flags.Charging))
				{
					num3++;
					num2--;
				}
			}
			troop = ++troop;
		}
		if (this.logic.def.type == Logic.Unit.Type.InventoryItem && num == 0)
		{
			bool flag = false;
			if (this.logic.ranged_enemy != null)
			{
				Troops.Troop troop2 = this.data->FirstTroop;
				while (troop2 <= this.data->LastTroop)
				{
					if (Math.Abs(troop2.rot_within_bounds - troop2.tgt_arrow_rot) > 3f)
					{
						flag = true;
						break;
					}
					troop2 = ++troop2;
				}
			}
			num = ((flag || this.logic.movement.IsMoving(true)) ? 1 : 0);
		}
		if (this.logic.battle.stage != Logic.Battle.Stage.Ongoing || this.logic.battle.battle_map_finished)
		{
			num = 0;
			num3 = 0;
			num2 = 0;
		}
		if (num6 != 0 && this.last_shooting == 0)
		{
			SalvoEmitter.SpawnSalvoEmitter(this.cur_salvo, this.logic, this.logic.salvo_def);
		}
		this.last_shooting = num6;
		if (UnityEngine.Time.time > this.next_charge && squad != null && !this.data->HasFlags(Troops.SquadData.Flags.Fighting))
		{
			this.next_charge = UnityEngine.Time.time + 5f;
			if (this.last_enemy == null)
			{
				this.PlayCharge();
			}
		}
		this.last_enemy = squad;
		string text;
		if (this.logic.battle.winner != -1)
		{
			if (num5 > 0)
			{
				text = this.def.CheeringSoundLoop;
			}
			else
			{
				text = null;
			}
		}
		else
		{
			if (num != 0 || num2 != 0 || num3 != 0 || num4 != 0)
			{
				if (num3 > num2 && num3 > num)
				{
					text = this.def.SprintingSoundLoop;
				}
				else if (num2 > num)
				{
					text = this.def.RunningSoundLoop;
				}
				else if (num > 0)
				{
					text = this.def.MarchingSoundLoop;
				}
				else
				{
					text = null;
				}
			}
			else
			{
				text = this.def.IdleSoundLoop;
			}
			if (this.logic.IsDefeated())
			{
				text = null;
			}
		}
		if (text != this.last_march)
		{
			this.marching_sound_emitter.Stop();
			if (text != null)
			{
				if (global::Squad.DebugAudio && this.Selected)
				{
					UnityEngine.Debug.Log("Playing " + text);
				}
				this.marching_sound_emitter.Event = text;
				this.marching_sound_emitter.Play();
			}
			else
			{
				if (global::Squad.DebugAudio && this.Selected)
				{
					UnityEngine.Debug.Log("Stopping " + this.last_march);
				}
				this.marching_sound_emitter.Event = text;
			}
			this.last_march = text;
		}
		this.SendParameters(num, num2, num3, this.logic.NumTroops());
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0006F9D8 File Offset: 0x0006DBD8
	private void SendParameters(int num_walking, int num_running, int num_sprinting, int num_charging)
	{
		if (this.marching_sound_emitter != null)
		{
			this.marching_sound_emitter.EventInstance.setParameterByName("NumWalk", (float)num_walking, false);
			this.marching_sound_emitter.EventInstance.setParameterByName("NumRun", (float)num_running, false);
			this.marching_sound_emitter.EventInstance.setParameterByName("NumSprint", (float)num_sprinting, false);
			this.marching_sound_emitter.EventInstance.setParameterByName("NumCharge", (float)num_charging, false);
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0006FA64 File Offset: 0x0006DC64
	[ConsoleMethod("squad_log")]
	public static void ToggleSquadAudioDebug(int v)
	{
		global::Squad.DebugAudio = (v == 1);
		UnityEngine.Debug.Log("DebugAudio " + global::Squad.DebugAudio.ToString());
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0006FA88 File Offset: 0x0006DC88
	public void SetPosition(Vector3 pt)
	{
		base.transform.position = global::Common.SnapToTerrain(pt, 0f, null, -1f, false);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0006FAA8 File Offset: 0x0006DCA8
	public void SpawnBanner(Logic.Unit.Def def)
	{
		if (!this.logic.is_main_squad)
		{
			return;
		}
		GameObject gameObject = global::Squad.BannerPrefab(def.field.key);
		if (gameObject == null)
		{
			return;
		}
		if (this.squad_banner != null)
		{
			global::Common.DestroyObj(this.squad_banner.gameObject);
		}
		if (GameLogic.instance != null)
		{
			this.squad_banner = global::Common.Spawn(gameObject, base.transform.parent, false, "").transform;
		}
		if (this.squad_banner == null)
		{
			return;
		}
		CrestObject component = global::Common.FindChildByName(this.squad_banner.gameObject, "Plane2_2", true, true).GetComponent<CrestObject>();
		if (component == null)
		{
			return;
		}
		component.SetKingdomId(this.logic.kingdom_id);
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0006FB74 File Offset: 0x0006DD74
	public void CreateSubSquad(int[] subSquadTroopsId, PPos position)
	{
		using (Game.Profile("Troops.CreateSubSquad", false, 0f, null))
		{
			if (subSquadTroopsId != null && subSquadTroopsId.Length != 0)
			{
				Logic.Battle battle = BattleMap.battle;
				if (battle != null)
				{
					if (Troops.GetEmptySquad() >= 0)
					{
						Logic.Unit.Def def = GameLogic.Get(true).defs.Get<Logic.Unit.Def>(this.def.id);
						if (def != null)
						{
							if (((battle != null) ? battle.simulation : null) != null)
							{
								Logic.Unit unit = new Logic.Unit();
								unit.def = def;
								unit.salvo_def = this.logic.simulation.unit.salvo_def;
								unit.SetArmy(this.logic.simulation.army);
								unit.SetGarrison(this.logic.simulation.garrison);
								unit.mercenary = this.logic.simulation.unit.mercenary;
								int num = this.GetID();
								if (this.GetMainSquadID() != -1)
								{
									num = this.GetMainSquadID();
								}
								global::Squad squad = Troops.squads[num];
								int alive = subSquadTroopsId.Length;
								BattleSimulation.Squad simulation;
								if (unit.army != null)
								{
									simulation = new BattleSimulation.Squad(unit, battle.simulation, battle.simulation.def.unit_max_morale / 2f);
								}
								else
								{
									simulation = new BattleSimulation.Squad(unit, unit.garrison, battle.simulation, 1, battle.simulation.def.unit_max_morale / 2f);
								}
								Logic.Squad squad2 = new Logic.Squad(battle, simulation, position);
								squad2.SetNumTroops(alive, false);
								squad2.main_squad = squad.logic;
								squad2.Start();
								squad2.position_set = true;
								squad2.visual_def = squad.logic.visual_def;
								global::Squad squad3 = squad2.visuals as global::Squad;
								squad.logic.AddSubSquad(squad2);
								squad3.AddToTroops(num, subSquadTroopsId);
								squad.subsquads_data_ids.Add(squad3.GetID());
								this.data_formation.dirty = true;
								squad3.ShowBanners(false);
								if (squad.logic.simulation.state == BattleSimulation.Squad.State.Fled)
								{
									squad3.logic.simulation.SetState(BattleSimulation.Squad.State.Fled, null, -1f);
								}
								else
								{
									bool disengage = squad.logic.command == Logic.Squad.Command.Disengage;
									squad3.logic.MoveTo(this.logic, 0f, true, this.logic.direction, disengage, false, false, false, false, false, false);
								}
								if (squad3.logic.movement.pf_path != null)
								{
									Path pf_path = squad3.logic.movement.pf_path;
									pf_path.onPathFindingComplete = (Action)Delegate.Combine(pf_path.onPathFindingComplete, new Action(squad3.OnPathFindingComplete));
								}
								squad3.UpdateControlZoneRenderer();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0006FE58 File Offset: 0x0006E058
	public void OnPathFindingComplete()
	{
		if (this.logic != null)
		{
			Logic.Squad squad = this.logic;
			if (((squad != null) ? squad.movement : null) != null)
			{
				Logic.Squad squad2 = this.logic;
				bool flag;
				if (squad2 == null)
				{
					flag = (null != null);
				}
				else
				{
					Game game = squad2.game;
					if (game == null)
					{
						flag = (null != null);
					}
					else
					{
						Logic.PathFinding path_finding = game.path_finding;
						flag = (((path_finding != null) ? path_finding.data : null) != null);
					}
				}
				if (flag)
				{
					PathData.DataPointers pointers = this.logic.game.path_finding.data.pointers;
					global::Squad squad3 = Troops.GetSquad(this.main_squad_data_id);
					PPos ppos;
					if (squad3 != null && (this.logic.movement.path == null || this.logic.movement.path.state != Path.State.Succeeded || (((squad3 != null) ? squad3.logic : null) != null && !pointers.BurstedTrace(this.logic.movement.path.segments[this.logic.movement.path.segments.Count - 1].pt, squad3.logic.position, out ppos, false, true, false, this.logic.movement.allowed_area_types, this.logic.battle_side, this.logic.is_inside_walls_or_on_walls))))
					{
						this.subsquad_path_attempts++;
						if (this.subsquad_path_attempts >= 5)
						{
							if (this.logic.position.paID != 0)
							{
								PathData.PassableArea pa = Troops.path_data.GetPA(this.logic.position.paID - 1);
								if (pa.normal != default(Point3))
								{
									PPos[] array = new PPos[2];
									array[0] = this.logic.position + pa.normal.xz * 15f;
									array[0].paID = 0;
									array[1] = this.logic.position - pa.normal.xz * 15f;
									array[1].paID = 0;
									this.logic.ValidateIfStuck(array);
								}
							}
						}
						else
						{
							bool disengage = this.logic.main_squad != null && this.logic.main_squad.command == Logic.Squad.Command.Disengage;
							this.logic.MoveTo(squad3.logic, 0f, true, squad3.logic.direction, disengage, false, false, false, false, false, false);
							if (this.logic.movement.pf_path != null)
							{
								Path pf_path = this.logic.movement.pf_path;
								pf_path.onPathFindingComplete = (Action)Delegate.Combine(pf_path.onPathFindingComplete, new Action(this.OnPathFindingComplete));
							}
						}
					}
					else
					{
						this.subsquad_path_attempts = 0;
					}
					if (this.logic.movement.path != null)
					{
						Path path = this.logic.movement.path;
						path.onPathFindingComplete = (Action)Delegate.Remove(path.onPathFindingComplete, new Action(this.OnPathFindingComplete));
					}
					return;
				}
			}
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00070178 File Offset: 0x0006E378
	public unsafe void MergeWithMainSquad()
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		if (this.main_squad_data_id == -1)
		{
			return;
		}
		global::Squad squad = Troops.GetSquad(this.main_squad_data_id);
		if (this.data != null && this.ReadyToAdd())
		{
			Troops.SquadData* main_squad_data = this.main_squad_data;
			Troops.SquadData.Ptr* ptr = Troops.pdata->squad + main_squad_data->offset;
			Troops.Troop troop = main_squad_data->FirstTroop;
			while (troop <= main_squad_data->LastTroop)
			{
				if (troop.squad_id == this.GetID())
				{
					ptr->ptr = main_squad_data;
					troop.SetDrawerInfo(squad, this.logic.visual_def != squad.logic.visual_def);
				}
				ptr++;
				troop = ++troop;
			}
		}
		int allSquadsAliveTroops = squad.GetAllSquadsAliveTroops();
		squad.GetAllSquadsNumTroops();
		int num = squad.logic.NumTroops();
		if (allSquadsAliveTroops != num)
		{
			squad.logic.SetNumTroops(allSquadsAliveTroops, false);
			this.main_squad_data->logic_alive = allSquadsAliveTroops;
			squad.last_alive = allSquadsAliveTroops;
		}
		squad.data_formation.dirty = true;
		this.DeleteObject();
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000702A4 File Offset: 0x0006E4A4
	public unsafe void MergeSubSquads(global::Squad otherSquad)
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		if (this.GetMainSquadID() == -1)
		{
			return;
		}
		if (this.GetMainSquadID() != otherSquad.GetMainSquadID())
		{
			return;
		}
		if (this.data != null && this.ReadyToAdd())
		{
			Troops.SquadData* data = this.data;
			Troops.SquadData.Ptr* ptr = Troops.pdata->squad + data->offset;
			Troops.Troop troop = data->FirstTroop;
			while (troop <= data->LastTroop)
			{
				if (troop.squad_id == otherSquad.GetID())
				{
					troop.squad->logic_alive--;
					troop.SetDrawerInfo(this, this.logic.visual_def != otherSquad.logic.visual_def);
					ptr->ptr = data;
					if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
					{
						troop.squad->logic_alive++;
					}
				}
				ptr++;
				troop = ++troop;
			}
			int squadAliveTroops = this.GetSquadAliveTroops();
			this.GetSquadNumTroops();
			if (data->logic_alive != squadAliveTroops)
			{
				data->logic_alive = squadAliveTroops;
			}
			this.logic.SetNumTroops(squadAliveTroops, false);
			this.last_alive = squadAliveTroops;
		}
		this.data_formation.dirty = true;
		otherSquad.DeleteObject();
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x000703F0 File Offset: 0x0006E5F0
	public global::Squad GetNextMainSquad()
	{
		if (this.subsquads_data_ids.Count == 0)
		{
			return null;
		}
		global::Squad result = null;
		int num = 0;
		for (int i = 0; i < this.subsquads_data_ids.Count; i++)
		{
			global::Squad squad = Troops.GetSquad(this.subsquads_data_ids[i]);
			if (!squad.logic.simulation.IsDefeated())
			{
				int squadAliveTroops = squad.GetSquadAliveTroops();
				if (squadAliveTroops > num)
				{
					num = squadAliveTroops;
					result = squad;
				}
			}
		}
		return result;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00070460 File Offset: 0x0006E660
	public unsafe void SwitchToMainSquad()
	{
		global::Squad squad = Troops.GetSquad(this.main_squad_data->id);
		Troops.SquadData.Ptr* ptr = Troops.pdata->squad + this.main_squad_data->offset;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Troops.Troop troop = this.main_squad_data->FirstTroop;
		while (troop <= this.main_squad_data->LastTroop)
		{
			if (troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				ptr->ptr = this.main_squad_data;
				ptr++;
			}
			else
			{
				if (troop.squad_id == this.GetID())
				{
					ptr->ptr = this.main_squad_data;
					if (!troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						num++;
					}
					num2++;
				}
				else if (troop.squad_id == this.GetMainSquadID())
				{
					ptr->ptr = this.data;
					if (!troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						num3++;
					}
					num4++;
				}
				ptr++;
			}
			troop = ++troop;
		}
		PPos position = squad.logic.position;
		PPos pt = squad.logic.direction;
		squad.logic.SetPosition(this.logic.position);
		squad.logic.direction = this.logic.direction;
		squad.data_formation.dirty = true;
		if (squad.logic.movement.path != null)
		{
			squad.logic.RecalcPath(this.logic.position);
		}
		int allSquadsAliveTroops = squad.GetAllSquadsAliveTroops();
		squad.GetAllSquadsNumTroops();
		squad.logic.SetNumTroops(allSquadsAliveTroops, false);
		squad.last_alive = allSquadsAliveTroops;
		this.main_squad_data->logic_alive = allSquadsAliveTroops;
		this.logic.SetNumTroops(num3, false);
		this.last_alive = num3;
		if (this.data_formation != null)
		{
			this.data_formation.dirty = true;
		}
		this.logic.SetPosition(position);
		this.logic.direction = pt;
		this.data->logic_alive = num3;
		if (this.logic.movement.path != null)
		{
			this.logic.RecalcPath(this.logic.position);
		}
		BattleSimulation.Squad.State state = this.logic.simulation.state;
		this.logic.simulation.SetState(squad.logic.simulation.state, null, -1f);
		squad.logic.simulation.SetState(state, null, -1f);
		PPos[] array = squad.data->move_history.ToArray(null);
		squad.data->move_history = default(Troops.MoveHistory);
		if (this.logic.move_history != null)
		{
			for (int i = 0; i < this.logic.move_history.Length; i++)
			{
				squad.data->move_history.Set(i, this.logic.move_history[i]);
			}
		}
		this.data->move_history = default(Troops.MoveHistory);
		for (int j = 0; j < array.Length; j++)
		{
			this.data->move_history.Set(j, array[j]);
		}
		this.logic.Stop(true);
		this.logic.MoveTo(this.logic, 0f, true, this.logic.direction, false, false, false, false, false, false, false);
		if (this.logic.movement.pf_path != null)
		{
			Path pf_path = this.logic.movement.pf_path;
			pf_path.onPathFindingComplete = (Action)Delegate.Combine(pf_path.onPathFindingComplete, new Action(this.OnPathFindingComplete));
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00070818 File Offset: 0x0006EA18
	public unsafe int GetSquadAliveTroops()
	{
		if (this.main_squad_data_id == -1 && this.subsquads_data_ids.Count <= 0)
		{
			return this.logic.NumTroops();
		}
		int num = 0;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.squad_id == this.GetID())
			{
				num++;
			}
			troop = ++troop;
		}
		return num;
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00070894 File Offset: 0x0006EA94
	public unsafe int GetSquadNumTroops()
	{
		int num = 0;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Destroyed) && troop.squad_id == this.GetID())
			{
				num++;
			}
			troop = ++troop;
		}
		return num;
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x000708F0 File Offset: 0x0006EAF0
	public unsafe int GetAllSquadsAliveTroops()
	{
		int num = 0;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				num++;
			}
			troop = ++troop;
		}
		return num;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0007093C File Offset: 0x0006EB3C
	public unsafe int GetAllSquadsNumTroops()
	{
		int num = 0;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Destroyed))
			{
				num++;
			}
			troop = ++troop;
		}
		return num;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00070988 File Offset: 0x0006EB88
	public static void Spawn(int side, string type, Vector3 pt, bool notify = true)
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "spawn", true))
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		Logic.Army army = battle.GetArmy(side);
		Garrison garrison;
		if (side != 1)
		{
			garrison = null;
		}
		else
		{
			Logic.Settlement settlement = battle.settlement;
			garrison = ((settlement != null) ? settlement.garrison : null);
		}
		Garrison garrison2 = garrison;
		if (army == null && garrison2 == null)
		{
			return;
		}
		Logic.Unit.Def def = GameLogic.Get(true).defs.Get<Logic.Unit.Def>(type);
		if (def == null)
		{
			return;
		}
		if (((battle != null) ? battle.simulation : null) == null)
		{
			return;
		}
		SalvoData.Def salvo_def = GameLogic.Get(true).defs.Get<SalvoData.Def>(def.salvo_def);
		BattleSimulation.Squad squad;
		if (def.type != Logic.Unit.Type.InventoryItem)
		{
			Logic.Unit unit = new Logic.Unit();
			unit.def = def;
			unit.level = UnityEngine.Random.Range(0, unit.def.experience_to_next.items.Count + 1);
			unit.salvo_def = salvo_def;
			unit.SetArmy(army);
			unit.SetGarrison(garrison2);
			unit.mercenary = false;
			if (army != null)
			{
				army.AddUnit(unit, -1, true);
				squad = new BattleSimulation.Squad(unit, battle.simulation, battle.simulation.def.unit_max_morale / 2f);
			}
			else
			{
				squad = new BattleSimulation.Squad(unit, garrison2, battle.simulation, 1, battle.simulation.def.unit_max_morale / 2f);
			}
			battle.simulation.AddSquad(squad);
			squad.temporary = true;
			squad.spawned_in_bv = true;
			battle.CreateSquads(false);
		}
		else
		{
			if (army == null)
			{
				return;
			}
			InventoryItem inventoryItem = army.AddInvetoryItem(def, 0, true, false, true);
			battle.simulation.AddEquipment(army);
			battle.CreateSquads(false);
			squad = inventoryItem.simulation;
			squad.temporary = true;
			squad.spawned_in_bv = true;
			if (squad.sub_squads != null)
			{
				for (int i = 0; i < squad.sub_squads.Count; i++)
				{
					BattleSimulation.Squad squad2 = squad.sub_squads[i];
					squad2.temporary = true;
					squad2.spawned_in_bv = true;
				}
			}
		}
		for (int j = 0; j < squad.sub_squads.Count; j++)
		{
			BattleSimulation.Squad squad3 = squad.sub_squads[j];
			Logic.Squad squad4 = squad3.squad;
			squad3.spawned_in_bv = true;
			squad4.Start();
			squad4.Teleport(pt);
			squad4.SetCommand(Logic.Squad.Command.Hold, null);
			squad4.position_set = true;
		}
		if (notify)
		{
			if (army != null)
			{
				army.NotifyListeners("units_changed", null);
				return;
			}
			battle.NotifyListeners("units_changed", null);
		}
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00070BEB File Offset: 0x0006EDEB
	private bool TryCommandWhileRetreat()
	{
		if (this.logic.is_fleeing)
		{
			this.PlayVoiceLine(global::Squad.VoiceCommand.Retreat, base.transform.position, "FloatingTexts.Normal");
			return true;
		}
		return false;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x00070C14 File Offset: 0x0006EE14
	private bool TryImpossibleMove(Vector3 pt, bool can_shoot)
	{
		if (can_shoot)
		{
			return false;
		}
		bool flag = (this.logic.movement.allowed_area_types & PathData.PassableArea.Type.Ladder) > PathData.PassableArea.Type.None;
		if (flag)
		{
			return false;
		}
		bool flag2 = BattleMap.Get().IsInsideWall(pt) && !this.logic.is_inside_walls_or_on_walls;
		bool flag3 = false;
		bool fortification_destroyed = this.logic.battle.fortification_destroyed;
		if (!flag && flag2 && !fortification_destroyed)
		{
			for (int i = 0; i < this.logic.battle.gates.Count; i++)
			{
				Logic.PassableGate passableGate = this.logic.battle.gates[i];
				if (passableGate.battle_side == this.logic.battle_side || passableGate.Open)
				{
					flag3 = true;
					break;
				}
			}
		}
		if (flag2 && !flag && !flag3 && !fortification_destroyed)
		{
			this.PlayVoiceLine(global::Squad.VoiceCommand.RefuseOrder, base.transform.position, "FloatingTexts.Normal");
			return true;
		}
		return false;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00070D04 File Offset: 0x0006EF04
	public unsafe void MoveTo(Vector3 pt, int passableAreaID, Point facingDir, bool confirmation = true, bool ignore_reserve = false)
	{
		if (this.data == null)
		{
			return;
		}
		if (this.TryCommandWhileRetreat())
		{
			return;
		}
		bool dblclk = BaseUI.Get().dblclk;
		bool flag = true;
		bool key = UICommon.GetKey(KeyCode.LeftShift, false);
		bool flag2 = UICommon.GetKey(KeyCode.LeftControl, false) && key;
		if (flag2 && !Game.CheckCheatLevel(Game.CheatLevel.High, "teleport", true))
		{
			flag2 = false;
		}
		MapObject mapObject = null;
		this.logic.SetEnemy(null);
		this.logic.SetRangedEnemy(null);
		bool flag3 = this.logic.is_fighting || this.logic.command == Logic.Squad.Command.Fight;
		if (dblclk && flag)
		{
			this.DoubleTime(pt);
			return;
		}
		if (flag2)
		{
			this.logic.Stop(true);
			this.logic.Teleport(new PPos(pt, passableAreaID));
			base.transform.position = pt;
			this.logic.SetCommand(Logic.Squad.Command.Hold, null);
			return;
		}
		BattleViewUI battleViewUI = BattleViewUI.Get();
		MapObject mapObject2 = null;
		int battle_side = this.logic.battle_side;
		float equipment_melee_dist = 0f;
		bool flag4 = UnityEngine.Time.unscaledTime - battleViewUI.btn_down_time > 0.25f;
		if (battle_side >= 0 && (!battleViewUI.dragging || !flag4))
		{
			global::Squad squad = battleViewUI.picked_squads[1 - battle_side];
			mapObject2 = ((squad != null) ? squad.logic : null);
		}
		global::Fortification fortification = battleViewUI.picked_fortifications[1 - battle_side];
		Logic.Fortification fortification2 = (fortification != null) ? fortification.logic : null;
		if (fortification2 == null)
		{
			global::Fortification fortification3 = battleViewUI.picked_fortifications[battle_side];
			fortification2 = ((fortification3 != null) ? fortification3.logic : null);
		}
		if (battleViewUI.dragging && flag4)
		{
			fortification2 = null;
		}
		if (fortification2 != null && this.logic.IsEnemy(fortification2))
		{
			mapObject2 = fortification2;
			global::Fortification fortification4 = fortification2.visuals as global::Fortification;
			equipment_melee_dist = Mathf.Max(Vector3.Distance(fortification4.attack_position_a_world, fortification4.transform.position), Vector3.Distance(fortification4.attack_position_b_world, fortification4.transform.position));
		}
		else if (this.TryImpossibleMove(pt, mapObject2 != null && this.logic.CanAttack(mapObject2) && this.logic.CanShoot(mapObject2, false)))
		{
			return;
		}
		global::Squad.VoiceCommand line;
		if (mapObject2 != null)
		{
			bool key2 = UICommon.GetKey(KeyCode.LeftControl, false);
			bool key3 = UICommon.GetKey(KeyCode.LeftShift, false);
			if (this.logic.CanAttack(mapObject2))
			{
				if (!this.logic.Attack(mapObject2, dblclk, true, key2, equipment_melee_dist, true, key3, false, false))
				{
					return;
				}
				if (this.logic.CanShoot(mapObject2, false) && !key2 && !this.logic.is_fighting)
				{
					line = global::Squad.VoiceCommand.Shoot;
				}
				else if (dblclk)
				{
					line = global::Squad.VoiceCommand.Charge;
					this.logic.SetCommand(Logic.Squad.Command.Charge, mapObject2);
				}
				else
				{
					line = global::Squad.VoiceCommand.Attack;
				}
				this.PlayVoiceLine(line, global::Common.SnapToTerrain(pt, 0f, null, -1f, false), "FloatingTexts.Attacking");
				return;
			}
		}
		if (fortification2 != null)
		{
			bool flag5 = false;
			if (fortification2.def.type == Logic.Fortification.Type.Wall && fortification2.paids != null && fortification2.paids.Count > 0)
			{
				int num = -1;
				for (int i = 0; i < fortification2.paids.Count; i++)
				{
					int num2 = fortification2.paids[i];
					PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(num2 - 1);
					if (!pa.IsGround() && (this.logic.movement.allowed_area_types & pa.type) != PathData.PassableArea.Type.None && pa.type != PathData.PassableArea.Type.Ladder && pa.type != PathData.PassableArea.Type.LadderExit)
					{
						num = num2;
						break;
					}
				}
				if (num != -1)
				{
					passableAreaID = num;
					pt = this.logic.game.path_finding.data.pointers.GetPA(passableAreaID - 1).Center();
					flag5 = true;
				}
			}
			if (!flag5)
			{
				return;
			}
		}
		if (dblclk || flag3)
		{
			line = global::Squad.VoiceCommand.Run;
		}
		else if (!this.logic.IsMoving() || Vector3.SqrMagnitude(pt - this.logic.position) > this.data->BoundingBoxRadiusSqr)
		{
			line = global::Squad.VoiceCommand.Move;
		}
		else
		{
			line = global::Squad.VoiceCommand.Stop;
		}
		if (flag)
		{
			if (mapObject != null)
			{
				this.logic.MoveTo(mapObject, 0f, dblclk, facingDir, flag3, false, ignore_reserve, true, UICommon.GetKey(KeyCode.LeftShift, false), false, false);
			}
			else
			{
				this.logic.MoveTo(new PPos(pt, passableAreaID), 0f, dblclk, facingDir, flag3, false, false, false, ignore_reserve, -1f, true, UICommon.GetKey(KeyCode.LeftShift, false), false, false);
			}
		}
		this.PlayVoiceLine(line, global::Common.SnapToTerrain(pt, 0f, null, -1f, false), "FloatingTexts.Normal");
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000711AE File Offset: 0x0006F3AE
	public void MoveTo(Vector3 pt, int passableAreaID, bool confirmation = true)
	{
		this.MoveTo(pt, passableAreaID, Point.Zero, confirmation, false);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x000711C0 File Offset: 0x0006F3C0
	public void DoubleTime(Vector3 pt)
	{
		if (this.logic.SetDoubleTime(true, true))
		{
			global::Squad.VoiceCommand line;
			if (this.logic.command == Logic.Squad.Command.Attack)
			{
				line = global::Squad.VoiceCommand.Charge;
				this.PlayVoiceLine(line, pt, "FloatingTexts.Attacking");
				return;
			}
			line = global::Squad.VoiceCommand.Run;
			this.PlayVoiceLine(line, pt, "FloatingTexts.Normal");
		}
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0007120C File Offset: 0x0006F40C
	public unsafe void PlayVoiceLine(global::Squad.VoiceCommand line, Vector3 pt, string st)
	{
		if (BaseUI.SelLO() != this.logic && (line != global::Squad.VoiceCommand.Retreat || this.logic.GetKingdom() != BaseUI.LogicKingdom()))
		{
			return;
		}
		this.moveCommand = line;
		this.style = st;
		this.playPosition = global::Common.SnapToTerrain(this.data->BoundingBoxCenter, 0f, null, -1f, false);
		this.textPosition = pt;
		if (base.gameObject.activeInHierarchy)
		{
			if (this.playAudioAt == -1f)
			{
				this.playAudioAt = UnityEngine.Time.time + 0.25f;
				return;
			}
		}
		else
		{
			this.PlayVoiceLine(this.moveCommand);
		}
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x000712B8 File Offset: 0x0006F4B8
	private void PlayVoiceLine(global::Squad.VoiceCommand line)
	{
		Logic.Battle battle = BattleMap.battle;
		if (battle == null)
		{
			return;
		}
		if (battle.GetArmy(this.logic.battle_side) == null)
		{
			return;
		}
		string voiceLinePath = null;
		string text = "";
		switch (line)
		{
		case global::Squad.VoiceCommand.Move:
			voiceLinePath = this.def.walk_voice_line;
			text = "move";
			break;
		case global::Squad.VoiceCommand.Charge:
			voiceLinePath = this.def.charge_voice_line;
			text = "charge";
			break;
		case global::Squad.VoiceCommand.Attack:
			if (this.logic.enemy_melee_fortification != null)
			{
				voiceLinePath = this.def.ram_battering_voice_line;
			}
			else
			{
				voiceLinePath = (this.def.is_ranged ? this.def.melee_attack_range_voice_line : this.def.melee_attack_voice_line);
			}
			text = "attack";
			break;
		case global::Squad.VoiceCommand.Shoot:
			voiceLinePath = this.def.shoot_voice_line;
			text = "shoot";
			break;
		case global::Squad.VoiceCommand.Retreat:
			voiceLinePath = this.def.retreat_voice_line;
			text = "retreat";
			break;
		case global::Squad.VoiceCommand.Run:
			voiceLinePath = this.def.run_voice_line;
			text = "run";
			break;
		case global::Squad.VoiceCommand.Face:
			voiceLinePath = this.def.face_voice_line;
			text = "face";
			break;
		case global::Squad.VoiceCommand.Fight:
			text = "fight";
			break;
		case global::Squad.VoiceCommand.FormLine:
			voiceLinePath = this.def.line_voice_line;
			text = "form_line";
			break;
		case global::Squad.VoiceCommand.FormTriangle:
			voiceLinePath = this.def.triangle_voice_line;
			text = "form_triangle";
			break;
		case global::Squad.VoiceCommand.FormWiden:
			voiceLinePath = this.def.widen_voice_line;
			text = "form_widen";
			break;
		case global::Squad.VoiceCommand.FormShrink:
			voiceLinePath = this.def.shrink_voice_line;
			text = "form_shrink";
			break;
		case global::Squad.VoiceCommand.Stop:
			voiceLinePath = this.def.stop_voice_line;
			text = "stop";
			break;
		case global::Squad.VoiceCommand.RefuseOrder:
			voiceLinePath = this.def.refuse_order_voice_line;
			text = "refuse_order";
			break;
		}
		FloatingText.Create(this.textPosition, this.style, text, null, true);
		this.PlayVoiceLine(voiceLinePath, this.def.VoiceSoundEffectPath, this.playPosition);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x000714AC File Offset: 0x0006F6AC
	public void PlayVoiceLine(string voiceLinePath, string soundEffectPath, Vector3 position)
	{
		if (!string.IsNullOrEmpty(voiceLinePath))
		{
			BaseUI.PlayVoiceEvent(voiceLinePath, this.logic, position);
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (string.IsNullOrEmpty(soundEffectPath))
		{
			return;
		}
		EventInstance eventInstance = FMODWrapper.CreateInstance(soundEffectPath, true);
		eventInstance.set3DAttributes(position.To3DAttributes());
		eventInstance.start();
		eventInstance.release();
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0007150C File Offset: 0x0006F70C
	private void Start()
	{
		if (this.logic == null)
		{
			return;
		}
		this.SpawnBanner(this.logic.def);
		this.UpdateSelection();
		this.visibility_index = VisibilityDetector.Add("BattleView", base.transform.position, 300f, null, this, base.gameObject.layer);
		this.logic.SetSpacing(Logic.Squad.Spacing.Default, true);
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00071574 File Offset: 0x0006F774
	public void CheckClimbArea()
	{
		if (this.data_id == -1)
		{
			return;
		}
		if (this.logic.movement.path == null)
		{
			return;
		}
		if (this.logic.climbing)
		{
			this.CheckCancelClimb();
			return;
		}
		if (this.logic.recalc_path)
		{
			return;
		}
		PPos ppos;
		PPos ppos2;
		PPos dest_formation_pos;
		this.GetClimbSegments(out ppos, out ppos2, out dest_formation_pos, false);
		if (ppos == PPos.Zero || ppos2 == PPos.Zero || this.wall_climb_paid == -1)
		{
			return;
		}
		this.Climb(ppos, ppos2, dest_formation_pos);
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x000715FC File Offset: 0x0006F7FC
	private void GetClimbSegments(out PPos start_pos, out PPos dest_pos, out PPos dest_formation_pos, bool check_current_ladder = false)
	{
		int num = this.wall_climb_paid;
		this.wall_climb_paid = -1;
		start_pos = PPos.Zero;
		dest_pos = PPos.Zero;
		dest_formation_pos = PPos.Zero;
		int num2 = -1;
		Logic.Squad squad = this.logic;
		bool flag;
		if (squad == null)
		{
			flag = (null != null);
		}
		else
		{
			Movement movement = squad.movement;
			if (movement == null)
			{
				flag = (null != null);
			}
			else
			{
				Path path = movement.path;
				flag = (((path != null) ? path.segments : null) != null);
			}
		}
		if (!flag)
		{
			return;
		}
		for (float num3 = this.logic.movement.path.t; num3 <= Math.Min(this.logic.movement.path.path_len, this.logic.movement.path.t + 5f); num3 += 1f)
		{
			int num4 = this.logic.movement.path.FindSegment(num3);
			PPos pt = this.logic.movement.path.segments[num4].pt;
			if (pt.paID != 0 && this.logic.game.path_finding.data.pointers.GetPA(pt.paID - 1).type == PathData.PassableArea.Type.Ladder)
			{
				num2 = num4;
				this.wall_climb_paid = pt.paID;
				break;
			}
		}
		if (this.wall_climb_paid == -1)
		{
			if (check_current_ladder)
			{
				this.wall_climb_paid = num;
				num2 = this.logic.movement.path.segment_idx;
			}
			if (this.wall_climb_paid == -1)
			{
				return;
			}
		}
		PPos ppos = PPos.Zero;
		PPos ppos2 = PPos.Zero;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		list.Add(this.wall_climb_paid);
		list2.Add(this.wall_climb_paid);
		while (list.Count > 0)
		{
			int num5 = list[0];
			list.RemoveAt(0);
			int num6 = num5 - 1;
			PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(num6);
			for (int i = 0; i < PathData.PassableArea.numNodes; i++)
			{
				Point xz = pa.GetCornerVertex(i).xz;
				Point xz2 = pa.GetCornerVertex((i + 1) % PathData.PassableArea.numNodes).xz;
				PathData.Node panormalNode = this.logic.game.path_finding.data.pointers.GetPANormalNode(num6 * PathData.PassableArea.numNodes + i);
				if (this.logic.game.path_finding.data.pointers.GetPANode(num6 * PathData.PassableArea.numNodes + i).type == PathData.PassableAreaNode.Type.Normal)
				{
					short pa_id = panormalNode.pa_id;
					if (!list2.Contains((int)pa_id))
					{
						PathData.PassableArea pa2 = this.logic.game.path_finding.data.pointers.GetPA((int)(pa_id - 1));
						if (pa2.type != PathData.PassableArea.Type.Ladder)
						{
							Point p = (xz + xz2) / 2f;
							PPos ppos3 = new PPos(p, (int)pa_id);
							bool flag2 = false;
							if (ppos == PPos.Zero)
							{
								ppos = ppos3;
								flag2 = true;
							}
							else if (ppos2 == PPos.Zero)
							{
								ppos2 = ppos3;
								flag2 = true;
							}
							if (pa2.type == PathData.PassableArea.Type.LadderExit)
							{
								flag2 = false;
							}
							if (pa.type == PathData.PassableArea.Type.LadderExit)
							{
								dest_formation_pos = new PPos(pa2.Center(), (int)pa_id);
								flag2 = true;
							}
							if (flag2)
							{
								goto IL_36C;
							}
						}
						list.Add((int)pa_id);
						list2.Add((int)pa_id);
					}
				}
				IL_36C:;
			}
		}
		if (dest_formation_pos == PPos.Zero)
		{
			dest_formation_pos = dest_pos;
		}
		if (ppos != PPos.Zero && ppos2 != PPos.Zero)
		{
			PPos pt2 = ppos - ppos2;
			Point point = Point.Zero;
			if (this.logic.movement.path.segments.Count > num2 + 1)
			{
				while (point == Point.Zero)
				{
					point = this.logic.movement.path.segments[num2 + 1].pt - this.logic.movement.path.segments[num2].pt;
					num2++;
				}
			}
			else
			{
				point = pt2;
			}
			if (pt2.Dot(point) > 0f)
			{
				PPos ppos4 = ppos;
				ppos = ppos2;
				ppos2 = ppos4;
			}
			start_pos = ppos;
			dest_pos = ppos2;
			if (dest_formation_pos.SqrDist(dest_pos) > dest_formation_pos.SqrDist(start_pos))
			{
				dest_formation_pos = dest_pos;
			}
		}
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00071AB8 File Offset: 0x0006FCB8
	private void CheckCancelClimb()
	{
		PPos ppos;
		PPos ppos2;
		PPos ppos3;
		this.GetClimbSegments(out ppos, out ppos2, out ppos3, true);
		if (ppos.paID == this.wall_climb_start_pos.paID && ppos2.paID == this.wall_climb_dest_pos.paID)
		{
			return;
		}
		if (ppos.paID == this.wall_climb_dest_pos.paID && (ppos2.paID == -1 || this.wall_climb_paid == -1))
		{
			return;
		}
		this.CancelClimb();
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00071B28 File Offset: 0x0006FD28
	public unsafe void RecalcClimbFormations()
	{
		int num = 0;
		int num2 = 0;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (troop.squad_id == this.GetID() && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadderWaiting))
				{
					num++;
				}
				else if (troop.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished))
				{
					num2++;
				}
			}
			troop = ++troop;
		}
		this.wall_climb_start_formation.SetCount(num);
		this.wall_climb_dest_formation.SetCount(num2);
		this.wall_climb_start_formation.CopyParamsFrom(this.data_formation);
		this.wall_climb_dest_formation.CopyParamsFrom(this.data_formation);
		this.wall_climb_start_formation.Calc(this.wall_climb_start_pos_offsetted, this.wall_normal, true, false);
		this.wall_climb_dest_formation.Calc(this.wall_climb_dest_pos_offsetted, this.wall_normal, true, false);
		this.wall_climb_start_formation.dirty = true;
		this.wall_climb_dest_formation.dirty = true;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00071C2C File Offset: 0x0006FE2C
	public unsafe void Climb(PPos start_pos, PPos dest_pos, PPos dest_formation_pos)
	{
		if ((this.logic.movement.allowed_area_types & PathData.PassableArea.Type.Ladder) == PathData.PassableArea.Type.None)
		{
			UnityEngine.Debug.LogError(string.Format("{0} in climb area", this.logic));
			return;
		}
		int num = this.wall_climb_paid;
		int paID = start_pos.paID;
		this.logic.cur_ladder = this.logic.battle.ladders[num];
		this.logic.movement.Pause(true, false);
		this.wall_climb_start_pos = start_pos;
		this.wall_climb_dest_pos = dest_pos;
		this.wall_climb_start_pos_offsetted = start_pos;
		this.wall_climb_dest_pos_offsetted = dest_formation_pos;
		PPos pt = -(dest_pos - start_pos).GetNormalized();
		PPos ppos;
		if (this.logic.game.path_finding.data.pointers.TraceDir(start_pos, pt, 3f, out ppos, PathData.PassableArea.Type.All, -1, false))
		{
			this.wall_climb_start_pos_offsetted = ppos;
		}
		PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(start_pos.paID - 1);
		PathData.PassableArea pa2 = this.logic.game.path_finding.data.pointers.GetPA(dest_pos.paID - 1);
		this.wall_normal = pa2.normal.xz.GetNormalized();
		if (this.wall_climb_start_formation == null)
		{
			FormationPool.Return(ref this.wall_climb_start_formation);
			this.wall_climb_start_formation = FormationPool.Get(this.logic.formation.def, this.logic);
		}
		if (this.wall_climb_dest_formation == null)
		{
			FormationPool.Return(ref this.wall_climb_dest_formation);
			this.wall_climb_dest_formation = FormationPool.Get(this.logic.formation.def, this.logic);
		}
		this.RecalcClimbFormations();
		this.data->climb_start_pt = pa.Center();
		this.data->climb_dest_pt = pa2.Center();
		this.data->climb_start_paid = paID;
		this.data->climb_dest_paid = this.wall_climb_dest_pos.paID;
		this.data->climb_ladder_paid = num;
		this.logic.climbing_pos = this.wall_climb_dest_pos;
		Troops.CompleteAllJobs();
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (troop.squad_id == this.GetID() && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				troop.SetFlags(Troops.Troop.Flags.ClimbingLadderWaiting);
			}
			troop = ++troop;
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00071EB4 File Offset: 0x000700B4
	public unsafe bool HasClimbingTroops()
	{
		bool flag = false;
		int id = this.GetID();
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (troop.squad_id == id && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				flag |= troop.HasFlags(Troops.Troop.Flags.ClimbingLadder | Troops.Troop.Flags.ClimbingLadderWaiting);
			}
			troop = ++troop;
		}
		return flag;
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00071F1C File Offset: 0x0007011C
	public unsafe void CancelClimb()
	{
		Troops.CompleteAllJobs();
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (troop.squad_id == this.GetID() && !troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished) && troop.pa_id > 0 && this.logic.game.path_finding.data.pointers.GetPA(troop.pa_id - 1).type == PathData.PassableArea.Type.LadderExit)
			{
				Formation formation = Troops.troop_formation(troop);
				if (formation != null)
				{
					NativeArrayList<Formation.Line> lines = formation.lines;
					PPos pos = formation.pos;
					float num = float.MaxValue;
					int i = 0;
					while (i < formation.lines.Count)
					{
						PPos pt = formation.lines[i].pt;
						if (pt.paID <= 0)
						{
							goto IL_120;
						}
						PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(pt.paID - 1);
						if (pa.type != PathData.PassableArea.Type.Ladder && pa.type != PathData.PassableArea.Type.LadderExit)
						{
							goto IL_120;
						}
						IL_13E:
						i++;
						continue;
						IL_120:
						float num2 = pt.SqrDist(troop.pos);
						if (num2 < num)
						{
							num = num2;
							pos = pt;
							goto IL_13E;
						}
						goto IL_13E;
					}
					troop.pos = pos;
				}
				return;
			}
			troop = ++troop;
		}
		Formation formation2 = this.wall_climb_start_formation;
		this.wall_climb_start_formation = this.wall_climb_dest_formation;
		this.wall_climb_dest_formation = formation2;
		PPos ppos = this.wall_climb_start_pos_offsetted;
		this.wall_climb_start_pos_offsetted = this.wall_climb_dest_pos_offsetted;
		this.wall_climb_dest_pos_offsetted = ppos;
		PPos ppos2 = this.wall_climb_start_pos;
		this.wall_climb_start_pos = this.wall_climb_dest_pos;
		this.wall_climb_dest_pos = ppos2;
		float3 climb_start_pt = this.data->climb_start_pt;
		this.data->climb_start_pt = this.data->climb_dest_pt;
		this.data->climb_dest_pt = climb_start_pt;
		int climb_start_paid = this.data->climb_start_paid;
		this.data->climb_start_paid = this.data->climb_dest_paid;
		this.data->climb_dest_paid = climb_start_paid;
		Troops.Troop troop2 = this.data->FirstTroop;
		while (troop2 <= this.data->LastTroop)
		{
			if (troop2.squad_id == this.GetID() && !troop2.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed))
			{
				if (troop2.HasFlags(Troops.Troop.Flags.ClimbingLadder))
				{
					troop2.climb_progress = 1f - troop2.climb_progress;
				}
				else if (troop2.HasFlags(Troops.Troop.Flags.ClimbingLadderFinished))
				{
					troop2.ClrFlags(Troops.Troop.Flags.ClimbingLadderFinished);
					troop2.SetFlags(Troops.Troop.Flags.ClimbingLadderWaiting);
				}
				else if (troop2.HasFlags(Troops.Troop.Flags.ClimbingLadderWaiting))
				{
					troop2.ClrFlags(Troops.Troop.Flags.ClimbingLadderWaiting);
					troop2.SetFlags(Troops.Troop.Flags.ClimbingLadderFinished);
				}
			}
			troop2 = ++troop2;
		}
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x0007221C File Offset: 0x0007041C
	private void RegisterMinimapIcon()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return;
		}
		battleViewUI.minimap.AddObj(this.logic);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00072258 File Offset: 0x00070458
	private void UnregisterMinimapIcon()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return;
		}
		battleViewUI.minimap.DelObj(this.logic);
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00072294 File Offset: 0x00070494
	public virtual void AddToTroops()
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		if (this.data == null && this.ReadyToAdd())
		{
			if (this.logic.IsDefeated())
			{
				UnityEngine.Debug.LogWarning("Attempting to add squad to jobs with 0 troops");
				this.logic.Destroy(false);
				return;
			}
			BattleMap.Get().position_squads = true;
			Troops.AddSquad(this);
			this.UpdateVisibility();
			this.RegisterMinimapIcon();
			this.UpdateControlZoneRenderer();
		}
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00072310 File Offset: 0x00070510
	public virtual void AddToTroops(int main_squad_id, int[] subSquadTroopsId)
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		if (subSquadTroopsId == null || subSquadTroopsId.Length == 0)
		{
			return;
		}
		if (this.data == null && this.ReadyToAdd())
		{
			if (this.logic.IsDefeated())
			{
				UnityEngine.Debug.LogWarning("Attempting to add squad to jobs with 0 troops");
				this.logic.Destroy(false);
				return;
			}
			Troops.AddSubSquad(this, main_squad_id, subSquadTroopsId);
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x00072379 File Offset: 0x00070579
	public int GetID()
	{
		return this.data_id;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x00072381 File Offset: 0x00070581
	public void SetID(int id)
	{
		this.data_id = id;
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x0007238A File Offset: 0x0007058A
	public int GetMainSquadID()
	{
		return this.main_squad_data_id;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00072392 File Offset: 0x00070592
	public void SetMainSquadID(int id)
	{
		this.main_squad_data_id = id;
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0007239B File Offset: 0x0007059B
	public bool ReadyToAdd()
	{
		return (BattleMap.battle == null || !BattleMap.battle.IsValid() || BattleMap.battle.battle_map_only || SettlementBV.finished_generation) && Troops.Initted;
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x000723CC File Offset: 0x000705CC
	public bool CheckSalvoPassability(PPos target_pos, Logic.Squad target_squad, bool shoot_after = true)
	{
		if (this.logic.def.type == Logic.Unit.Type.InventoryItem && target_squad == null)
		{
			return true;
		}
		Vector3 vector = this.logic.avg_troops_pos;
		if (this.logic.position.paID > 0)
		{
			PathData.DataPointers pointers = BattleMap.battle.batte_view_game.path_finding.data.pointers;
			vector.y = pointers.GetPA(this.logic.position.paID - 1).GetHeight(this.logic.position);
		}
		else
		{
			vector.y = global::Common.GetTerrainHeight(vector, null, false);
		}
		Vector3 vector2 = target_pos;
		if (target_pos.paID > 0)
		{
			PathData.DataPointers pointers2 = BattleMap.battle.batte_view_game.path_finding.data.pointers;
			vector2.y = pointers2.GetPA(target_pos.paID - 1).GetHeight(target_pos);
		}
		else
		{
			vector2.y = global::Common.GetTerrainHeight(vector2, null, false);
		}
		vector2 - vector;
		float shoot_height = this.salvo_def.shoot_height;
		vector.y += shoot_height;
		vector2.y += 0.5f;
		float magnitude = (new Vector2(vector2.x, vector2.z) - new Vector2(vector.x, vector.z)).magnitude;
		float num = vector.y - vector2.y;
		float gravity = this.salvo_def.gravity;
		float num2 = Mathf.Sqrt(magnitude * gravity);
		if (num2 < this.salvo_def.min_shoot_speed)
		{
			num2 = this.salvo_def.min_shoot_speed;
		}
		float num3 = num2;
		float num4 = num2 + this.salvo_def.shoot_speed_randomization_mod * num2;
		if (num >= 0f)
		{
			num2 = num4;
			float num5 = num2 * num2;
			float num6 = Mathf.Sqrt(num5 * num5 - 2f * num5 * -num * gravity - gravity * gravity * magnitude * magnitude);
			float num7 = Mathf.Atan((num5 - num6) / (gravity * magnitude));
			bool flag = num7 > -1.5707964f && num7 < 1.5707964f;
			if (flag)
			{
				float num8 = num7;
				flag = (num8 * 180f / 3.1415927f > this.salvo_def.min_shoot_angle);
				if (shoot_after)
				{
					this.logic.use_high_angle_arrow_path = false;
				}
				if (flag)
				{
					float num9 = Mathf.Cos(num8) * num2;
					float v0y = Mathf.Sin(num8) * num2;
					float num10 = magnitude / num9;
					if (!this.RaycastSalvo(vector, vector2, num9, v0y, num10, gravity, shoot_after))
					{
						return true;
					}
					flag = false;
				}
			}
			if (!flag)
			{
				num2 = num3;
				num5 = num2 * num2;
				num6 = Mathf.Sqrt(num5 * num5 - 2f * num5 * -num * gravity - gravity * gravity * magnitude * magnitude);
				float num11 = Mathf.Atan((num5 + num6) / (gravity * magnitude));
				flag = (num11 > -1.5707964f && num11 < 1.5707964f);
				if (flag)
				{
					float num8 = num11;
					flag = (num8 * 180f / 3.1415927f < this.salvo_def.max_shoot_angle);
					if (shoot_after)
					{
						this.logic.use_high_angle_arrow_path = true;
					}
					if (flag)
					{
						float num9 = Mathf.Cos(num8) * num2;
						float v0y = Mathf.Sin(num8) * num2;
						float num10 = magnitude / num9;
						return !this.RaycastSalvo(vector, vector2, num9, v0y, num10, gravity, shoot_after);
					}
				}
			}
		}
		else
		{
			num *= -1f;
			num2 = num4;
			float num12 = num2 * num2;
			float num13 = Mathf.Sqrt(num12 * num12 - 2f * num12 * -num * gravity - gravity * gravity * magnitude * magnitude);
			float num14 = Mathf.Atan((num12 - num13) / (gravity * magnitude));
			bool flag2 = num14 > -1.5707964f && num14 < 1.5707964f;
			if (flag2)
			{
				float num8 = num14;
				float num9 = Mathf.Cos(num8) * num2;
				float num10 = magnitude / num9;
				float v0y = -(Mathf.Sin(num8) * num2 - gravity * num10);
				flag2 = (num8 * 180f / 3.1415927f > this.salvo_def.min_shoot_angle);
				if (shoot_after)
				{
					this.logic.use_high_angle_arrow_path = false;
				}
				if (flag2)
				{
					if (!this.RaycastSalvo(vector, vector2, num9, v0y, num10, gravity, shoot_after))
					{
						return true;
					}
					flag2 = false;
				}
			}
			if (!flag2)
			{
				num2 = num3;
				num12 = num2 * num2;
				num13 = Mathf.Sqrt(num12 * num12 - 2f * num12 * -num * gravity - gravity * gravity * magnitude * magnitude);
				float num15 = Mathf.Atan((num12 + num13) / (gravity * magnitude));
				flag2 = (num15 > -1.5707964f && num15 < 1.5707964f);
				if (flag2)
				{
					float num8 = num15;
					float num9 = Mathf.Cos(num8) * num2;
					float num10 = magnitude / num9;
					float v0y = -(Mathf.Sin(num8) * num2 - gravity * num10);
					flag2 = (num8 * 180f / 3.1415927f < this.salvo_def.max_shoot_angle);
					if (shoot_after)
					{
						this.logic.use_high_angle_arrow_path = true;
					}
					if (flag2)
					{
						return !this.RaycastSalvo(vector, vector2, num9, v0y, num10, gravity, shoot_after);
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00072910 File Offset: 0x00070B10
	public bool RaycastSalvo(Vector3 start_pos, Vector3 end_pos, float v0x, float v0y, float dur, float g, bool shoot_after = true)
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(start_pos + new Vector3(0f, 0.1f));
		int num = 3;
		float num2 = dur / ((float)num + 1f);
		float num3 = 0f;
		for (int i = 0; i < num; i++)
		{
			num3 += num2;
			Vector2 vector = new Vector2(start_pos.x, start_pos.z) + v0x * num3 * (new Vector2(end_pos.x, end_pos.z) - new Vector2(start_pos.x, start_pos.z)).normalized;
			Vector3 vector2 = new Vector3(vector.x, start_pos.y + v0y * num3 - num3 * num3 * g * 0.5f, vector.y);
			if (this.salvo_def.collision_check_offset != 0f)
			{
				float num4 = (num3 <= dur * 0.5f) ? (num3 / dur * 2f) : ((1f - num3 / dur) * 2f);
				float y = Vector3.Lerp(start_pos, end_pos, num3 / dur).y;
				if (y < vector2.y - this.salvo_def.collision_check_offset * num4)
				{
					vector2 += new Vector3(0f, this.salvo_def.collision_check_offset * num4, 0f);
				}
				else
				{
					vector2.y = y;
				}
			}
			list.Add(vector2);
		}
		list.Add(end_pos + new Vector3(0f, 0.1f));
		this.debugSalvoPoints = new List<Vector4>();
		for (int j = 0; j < list.Count; j++)
		{
			this.debugSalvoPoints.Add(list[j]);
		}
		int mask = LayerMask.GetMask(new string[]
		{
			"Settlements",
			"Terrain"
		});
		QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide;
		for (int k = 0; k < list.Count - 1; k++)
		{
			Vector3 direction = list[k + 1] - list[k];
			RaycastHit raycastHit;
			if (Physics.Raycast(list[k], direction, out raycastHit, direction.magnitude, mask, queryTriggerInteraction))
			{
				if (shoot_after && this.logic.def.type == Logic.Unit.Type.InventoryItem)
				{
					Transform parent = raycastHit.collider.transform.parent;
					if (parent != null)
					{
						global::Fortification componentInChildren = parent.GetComponentInChildren<global::Fortification>();
						if (componentInChildren != null && componentInChildren.kingdom.id != this.logic.kingdom_id)
						{
							this.logic.target = componentInChildren.logic;
						}
					}
				}
				return true;
			}
			List<Vector4> list2 = this.debugSalvoPoints;
			int index = k;
			list2[index] += new Vector4(0f, 0f, 0f, 1f);
		}
		return false;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00072C04 File Offset: 0x00070E04
	public unsafe void ShootArrows(Vector3 tarPos, MapObject enemy)
	{
		if (this.def == null)
		{
			UnityEngine.Debug.LogWarning(this + " has a missing def, can't shoot");
			return;
		}
		if (this.data == null)
		{
			return;
		}
		if (this.data->HasFlags(Troops.SquadData.Flags.Fighting | Troops.SquadData.Flags.Shooting))
		{
			return;
		}
		global::Squad squad = enemy.visuals as global::Squad;
		if (squad != null)
		{
			this.cur_salvo = Troops.ShootSalvo(this.def, this.logic.salvo_def, this.logic.simulation.CTH_Ranged_Modified(null), this.data->friendly_fire_reduction, tarPos, this.logic.battle_side, this.data, squad.data, this.logic.use_high_angle_arrow_path);
		}
		else
		{
			this.cur_salvo = Troops.ShootSalvo(this.def, this.logic.salvo_def, this.logic.simulation.CTH_Ranged_Modified(null), this.data->friendly_fire_reduction, tarPos, this.logic.battle_side, this.data, null, this.logic.use_high_angle_arrow_path);
		}
		if (this.cur_salvo != -1)
		{
			Point normalized = (tarPos - this.logic.position).GetNormalized();
			this.logic.direction = normalized;
			this.logic.OnSuccessfulShoot(enemy);
		}
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00072D5C File Offset: 0x00070F5C
	public void ShowBanners(bool shown)
	{
		if (this.squad_banner != null)
		{
			bool flag = shown && this.main_squad_data_id == -1 && this.m_isBannerVisibleFilter;
			if (this.squad_banner.gameObject.activeSelf != flag)
			{
				this.squad_banner.gameObject.SetActive(flag);
			}
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00072DB4 File Offset: 0x00070FB4
	private unsafe void UpdateBannerPosition()
	{
		if (this.squad_banner != null && this.data != null)
		{
			this.ShowBanners(this.IsUiCanvasActiveInHierarchy() && !this.ShouldSquadBannerBeDisplayed());
			float3 banner_pos = this.data->banner_pos;
			Vector3 vector = new Vector3(banner_pos.x, banner_pos.y, banner_pos.z);
			float terrainHeight = global::Common.GetTerrainHeight(vector, null, false);
			vector.y = Mathf.Max(terrainHeight, vector.y);
			Vector3 vector2 = vector - this.squad_banner.position;
			if (Mathf.Abs(vector2.x) > 10f || Mathf.Abs(vector2.z) > 10f)
			{
				this.squad_banner.position = vector;
				return;
			}
			this.squad_banner.position = Vector3.Lerp(this.squad_banner.position, vector, UnityEngine.Time.deltaTime * 4f);
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00072EA4 File Offset: 0x000710A4
	private bool IsUiCanvasActiveInHierarchy()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		return ((battleViewUI != null) ? battleViewUI.canvas : null) != null && battleViewUI.canvas.gameObject.activeInHierarchy;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00072EE0 File Offset: 0x000710E0
	private bool ShouldSquadBannerBeDisplayed()
	{
		return this.logic != null && this.logic.simulation != null && (this.logic.simulation.state == BattleSimulation.Squad.State.Stuck || this.logic.simulation.state == BattleSimulation.Squad.State.Fled);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00072F30 File Offset: 0x00071130
	public unsafe void DelMinimap(bool check_data = true)
	{
		if (check_data)
		{
			if (this.data == null)
			{
				return;
			}
			if (!this.data->HasFlags(Troops.SquadData.Flags.Dead) && !this.data->HasFlags(Troops.SquadData.Flags.Fled))
			{
				return;
			}
		}
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return;
		}
		battleViewUI.minimap.DelObj(this.logic);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00072F9C File Offset: 0x0007119C
	protected void Update()
	{
		if (this.logic == null)
		{
			return;
		}
		this.AddToTroops();
		this.UpdateAliveTroops();
		this.UpdateStatusbar();
		this.UpdateControlZoneRenderer();
		this.UpdateSubSquadVars();
		this.UpdateUnpackParticles();
		if (this.playAudioAt != -1f && UnityEngine.Time.time > this.playAudioAt)
		{
			this.PlayVoiceLine(this.moveCommand);
			this.playAudioAt = -1f;
		}
		this.CalcSquadCenter();
		this.UpdateBannerPosition();
		this.DelMinimap(true);
		this.CheckClimbArea();
		this.UpdatePositionHeight();
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00073028 File Offset: 0x00071228
	public unsafe void UpdateInsideWallsValues()
	{
		BattleMap battleMap = BattleMap.Get();
		if (battleMap != null)
		{
			Color wallSDFVal = battleMap.GetWallSDFVal(this.logic.position);
			this.logic.is_inside_walls_dist = battleMap.GetDistToWall(wallSDFVal);
			bool is_inside_walls = this.logic.is_inside_walls;
			bool is_inside_walls2 = battleMap.IsInsideWall(wallSDFVal);
			bool is_on_walls = false;
			bool flag = true;
			if (this.logic.position.paID > 0)
			{
				PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(this.logic.position.paID - 1);
				is_on_walls = pa.IsWall();
				if (pa.type == PathData.PassableArea.Type.Tower || pa.type == PathData.PassableArea.Type.Wall || pa.type == PathData.PassableArea.Type.Stairs)
				{
					flag = false;
				}
			}
			if (flag && Math.Abs(this.logic.is_inside_walls_dist) < 0.9f)
			{
				is_inside_walls2 = false;
			}
			this.logic.is_inside_walls = is_inside_walls2;
			this.logic.is_on_walls = is_on_walls;
			if (!this.logic.is_inside_walls && is_inside_walls && !this.data->HasFlags(Troops.SquadData.Flags.Teleport))
			{
				this.logic.battle.OnSquadExitCity(this.logic.battle_side);
			}
		}
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00073178 File Offset: 0x00071378
	private void UpdateSubSquadVars()
	{
		if (this.subsquads_data_ids.Count > 0)
		{
			this.subsquad_highlighted = false;
			for (int i = 0; i < this.subsquads_data_ids.Count; i++)
			{
				global::Squad squad = Troops.squads[this.subsquads_data_ids[i]];
				squad.Selected = this.Selected;
				squad.InDrag = this.InDrag;
				squad.TargetPreviewed = this.TargetPreviewed;
				squad.MouseOvered = this.MouseOvered;
				squad.Previewed = this.Previewed;
				if (squad.Highlighted)
				{
					this.subsquad_highlighted = true;
				}
			}
		}
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0007320C File Offset: 0x0007140C
	private void UpdatePositionHeight()
	{
		PPos position = this.logic.position;
		if (position.paID > 0)
		{
			PathData.DataPointers pointers = BattleMap.battle.batte_view_game.path_finding.data.pointers;
			this.logic.actual_position_height = pointers.GetPA(position.paID - 1).GetHeight(position);
			return;
		}
		this.logic.actual_position_height = global::Common.GetTerrainHeight(position, null, false);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0007328C File Offset: 0x0007148C
	private unsafe void CalcSquadCenter()
	{
		if (this.GetID() < 0)
		{
			this.squad_center = base.transform.position;
			return;
		}
		Vector3 vector = global::Common.SnapToTerrain(this.data->BoundingBoxCenter, 0f, null, -1f, false);
		if (this.logic.position.paID > 0)
		{
			vector.y = this.logic.position.Height(this.logic.game, vector.y, 0f);
		}
		this.squad_center = vector;
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00073322 File Offset: 0x00071522
	public int GetKingdomID()
	{
		if (this.logic == null)
		{
			return this.kingdom;
		}
		return this.logic.kingdom_id;
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00073343 File Offset: 0x00071543
	public void VisibilityChanged(bool visible)
	{
		this.inCameraView = visible;
		this.UpdateVisibility();
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00073354 File Offset: 0x00071554
	public unsafe void RefreshVisibility()
	{
		if (this.logic == null)
		{
			return;
		}
		if (this.visibility_index >= 0 && this.data_id >= 0)
		{
			if (this.logic.IsDefeated() || this.data == null)
			{
				VisibilityDetector.Move("BattleView", this.visibility_index, base.transform.position, 40f);
				return;
			}
			float2 boundingBoxCenter = this.data->BoundingBoxCenter;
			VisibilityDetector.Move("BattleView", this.visibility_index, global::Common.SnapToTerrain(new Vector3(boundingBoxCenter.x, 0f, boundingBoxCenter.y), 0f, null, -1f, false), Mathf.Max(this.data->BoundingBoxRadius, 1f));
		}
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00073414 File Offset: 0x00071614
	public void UpdateVisibility()
	{
		bool flag = this.visible;
		this.visible = true;
		this.visible &= this.inCameraView;
		this.visible |= (this.data == null);
		if (this.status_bar != null)
		{
			this.status_bar.UpdateVisibilityFromObject(this.visible);
		}
		if (base.gameObject.activeSelf != this.visible)
		{
			base.gameObject.SetActive(this.visible);
		}
		bool shown = this.visible && this.m_isBannerVisibleFilter;
		this.ShowBanners(shown);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x000734B5 File Offset: 0x000716B5
	public bool StatusBarIsHovered()
	{
		return !(this.status_bar == null) && this.status_bar.mouse_in;
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x000734D4 File Offset: 0x000716D4
	private void ApplyFilters()
	{
		this.VisibilityFilterChanged("kingdom_flags_filter", BattleViewPreferences.GetPreference("kingdom_flags_filter", false));
		this.VisibilityFilterChanged("army_nameplates_filter", BattleViewPreferences.GetPreference("army_nameplates_filter", false));
		this.VisibilityFilterChanged("health_filter", BattleViewPreferences.GetPreference("health_filter", false));
		this.VisibilityFilterChanged("stamina_filter", BattleViewPreferences.GetPreference("stamina_filter", false));
		this.VisibilityFilterChanged("army_movement_arrows_filter", BattleViewPreferences.GetPreference("army_movement_arrows_filter", false));
		this.VisibilityFilterChanged("army_selection_circles_filter", BattleViewPreferences.GetPreference("army_selection_circles_filter", false));
		this.VisibilityFilterChanged("shooting_range_indication_filter", BattleViewPreferences.GetPreference("shooting_range_indication_filter", false));
		this.VisibilityFilterChanged("experience_indications_filter", BattleViewPreferences.GetPreference("experience_indications_filter", false));
		this.VisibilityFilterChanged("nameplate_tooltips", BattleViewPreferences.GetPreference("nameplate_tooltips", false));
		this.RefreshClampedBars();
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000735AD File Offset: 0x000717AD
	private void ListenToEvents()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI == null)
		{
			return;
		}
		FiltersEventHub filtersEventHub = battleViewUI.FiltersEventHub;
		if (filtersEventHub == null)
		{
			return;
		}
		filtersEventHub.AddListener(new FiltersEventHub.FilterChanged(this.VisibilityFilterChanged));
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x000735D4 File Offset: 0x000717D4
	private void StopListeningToEvents()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI == null)
		{
			return;
		}
		FiltersEventHub filtersEventHub = battleViewUI.FiltersEventHub;
		if (filtersEventHub == null)
		{
			return;
		}
		filtersEventHub.RemoveListener(new FiltersEventHub.FilterChanged(this.VisibilityFilterChanged));
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x000735FB File Offset: 0x000717FB
	public void RefreshClampedBars()
	{
		this.VisibilityFilterChanged("clamped_filter", this.status_bar.Clamped);
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00073614 File Offset: 0x00071814
	public void VisibilityFilterChanged(string filter, bool isOn)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(filter);
		if (num <= 1193140272U)
		{
			if (num <= 552299136U)
			{
				if (num != 519004238U)
				{
					if (num != 552299136U)
					{
						return;
					}
					if (!(filter == "shooting_range_indication_filter"))
					{
						return;
					}
					this.m_isShootingRangeIndicatorVisibleFilter = isOn;
					this.UpdateArrowRangeGO();
					return;
				}
				else
				{
					if (!(filter == "army_selection_circles_filter"))
					{
						return;
					}
					this.areSelectionCirclesVisibleFilter = isOn;
					return;
				}
			}
			else if (num != 629630590U)
			{
				if (num != 723558326U)
				{
					if (num != 1193140272U)
					{
						return;
					}
					if (!(filter == "clamped_filter"))
					{
						return;
					}
					this.status_bar.ForceHideMoraleBar(!isOn);
					return;
				}
				else
				{
					if (!(filter == "army_nameplates_filter"))
					{
						return;
					}
					this.m_isArmyPlateVisibleFilter = isOn;
					if (!(this.status_bar == null))
					{
						this.status_bar.NameplatesEnabled = this.m_isArmyPlateVisibleFilter;
						this.status_bar.UpdateVisibility();
						return;
					}
				}
			}
			else
			{
				if (!(filter == "army_movement_arrows_filter"))
				{
					return;
				}
				this.m_areMovementArrowsVisibleFilter = isOn;
				if (this.Selected || this.Previewed)
				{
					this.DestroyPathArrows();
					this.DestroyPathArrowsStraight();
					this.CreatePathVisualisation();
					return;
				}
			}
		}
		else if (num <= 2591306931U)
		{
			if (num != 2422787864U)
			{
				if (num != 2591306931U)
				{
					return;
				}
				if (!(filter == "nameplate_tooltips"))
				{
					return;
				}
				this.m_NameplateTooltipFilter = isOn;
				return;
			}
			else
			{
				if (!(filter == "experience_indications_filter"))
				{
					return;
				}
				if (this.status_bar == null)
				{
					return;
				}
				this.status_bar.ForceHideLevelIndicatior(isOn);
			}
		}
		else if (num != 2815667197U)
		{
			if (num != 3513114882U)
			{
				if (num != 4129070453U)
				{
					return;
				}
				if (!(filter == "stamina_filter"))
				{
					return;
				}
				if (!(this.status_bar == null))
				{
					this.status_bar.ForceHideStaminaBar(isOn);
					return;
				}
			}
			else
			{
				if (!(filter == "health_filter"))
				{
					return;
				}
				if (!(this.status_bar == null))
				{
					this.status_bar.ForceHideHealthBar(isOn);
					return;
				}
			}
		}
		else
		{
			if (!(filter == "kingdom_flags_filter"))
			{
				return;
			}
			this.m_isBannerVisibleFilter = isOn;
			return;
		}
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00073834 File Offset: 0x00071A34
	private void RefreshPathArrowsVisbility()
	{
		if (this.path_arrows != null)
		{
			this.path_arrows.gameObject.SetActive(this.m_areMovementArrowsVisibleFilter);
		}
		if (this.path_arrows_straight != null)
		{
			this.path_arrows_straight.gameObject.SetActive(this.m_areMovementArrowsVisibleFilter);
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x06000A01 RID: 2561 RVA: 0x00073889 File Offset: 0x00071A89
	// (set) Token: 0x06000A02 RID: 2562 RVA: 0x00073891 File Offset: 0x00071A91
	public bool Previewed
	{
		get
		{
			return this.m_Previewed;
		}
		set
		{
			this.m_Previewed = value;
			this.UpdatePreview();
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000A03 RID: 2563 RVA: 0x000738A0 File Offset: 0x00071AA0
	// (set) Token: 0x06000A04 RID: 2564 RVA: 0x000738A8 File Offset: 0x00071AA8
	public bool Selected
	{
		get
		{
			return this.m_Selected;
		}
		set
		{
			if (this.m_Selected == value)
			{
				return;
			}
			this.m_Selected = value;
			this.UpdateSelection();
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x06000A05 RID: 2565 RVA: 0x000738C1 File Offset: 0x00071AC1
	// (set) Token: 0x06000A06 RID: 2566 RVA: 0x000738C9 File Offset: 0x00071AC9
	public bool MouseOvered
	{
		get
		{
			return this.m_MouseOvered;
		}
		set
		{
			this.m_MouseOvered = value;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000A07 RID: 2567 RVA: 0x000738D4 File Offset: 0x00071AD4
	public bool Highlighted
	{
		get
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			return this.PreSelected || this.MouseOvered || (battleViewUI != null && battleViewUI.picked_squads != null && (battleViewUI.picked_squads[0] == this || battleViewUI.picked_squads[1] == this));
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000A08 RID: 2568 RVA: 0x0007392C File Offset: 0x00071B2C
	// (set) Token: 0x06000A09 RID: 2569 RVA: 0x00073934 File Offset: 0x00071B34
	public bool PreSelected
	{
		get
		{
			return this.preSelected;
		}
		set
		{
			if (this.preSelected != value)
			{
				this.preSelected = value;
			}
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00073946 File Offset: 0x00071B46
	protected void DestroyPathArrows()
	{
		if (this.path_arrows == null)
		{
			return;
		}
		global::Common.DestroyObj(this.path_arrows.gameObject);
		this.path_arrows = null;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0007396E File Offset: 0x00071B6E
	protected void DestroyPathArrowsStraight()
	{
		if (this.path_arrows_straight == null)
		{
			return;
		}
		global::Common.DestroyObj(this.path_arrows_straight.gameObject);
		this.path_arrows_straight = null;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00073996 File Offset: 0x00071B96
	protected void CreatePathVisualisation()
	{
		if (global::Squad.UseNormalPathArrows)
		{
			this.CreatePathArrows();
			return;
		}
		this.CreatePathArrowsStraight();
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x000739AC File Offset: 0x00071BAC
	protected void CreatePathArrows()
	{
		this.DestroyPathArrows();
		if (!this.m_areMovementArrowsVisibleFilter)
		{
			return;
		}
		Path path = this.logic.movement.path;
		if (path == null || path.IsDone() || this.main_squad_data_id != -1)
		{
			return;
		}
		Logic.Kingdom kingdom = this.logic.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		bool dark = false;
		if (kingdom != kingdom2)
		{
			if (kingdom.IsAlly(kingdom2))
			{
				dark = true;
			}
			else if (!Game.CheckCheatLevel(Game.CheatLevel.Low, "squad_path_arrows", false))
			{
				return;
			}
		}
		this.path_arrows = PathArrows.Create(this.logic.movement, 1, dark);
		this.path_arrows.gameObject.layer = base.gameObject.layer;
		this.RefreshPathArrowsVisbility();
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00073A60 File Offset: 0x00071C60
	protected void CreatePathArrowsStraight()
	{
		this.DestroyPathArrowsStraight();
		if (!this.m_areMovementArrowsVisibleFilter)
		{
			return;
		}
		Path path = this.logic.movement.path;
		if (path == null || path.IsDone() || this.main_squad_data_id != -1)
		{
			return;
		}
		Logic.Kingdom kingdom = this.logic.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (kingdom != kingdom2 && !kingdom.IsAlly(kingdom2) && !Game.CheckCheatLevel(Game.CheatLevel.Low, "squad_path_arrows", false))
		{
			return;
		}
		float shooting_range = this.def.is_ranged ? this.logic.Max_Shoot_Dist : 0f;
		this.path_arrows_straight = PathArrowsStraight.Create(this.logic.movement, shooting_range, BattleMap.Get().texture_baker, 3);
		this.path_arrows_straight.gameObject.layer = base.gameObject.layer;
		this.RefreshPathArrowsVisbility();
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00073B37 File Offset: 0x00071D37
	public void UpdateHighlight(bool highlighted)
	{
		if (this.squad_icon == null)
		{
			return;
		}
		if (highlighted)
		{
			this.squad_icon.color = this.icon_highlighted;
			return;
		}
		this.squad_icon.color = this.icon_normal;
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00073B70 File Offset: 0x00071D70
	protected virtual void UpdateSelection()
	{
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (((battleViewUI != null) ? battleViewUI.selected_squads : null) != null)
		{
			if (this.Selected && this.logic != null && this.logic.is_main_squad)
			{
				if (!battleViewUI.selected_squads.Contains(this))
				{
					List<global::Squad> selected_squads = battleViewUI.selected_squads;
					if (selected_squads != null)
					{
						selected_squads.Add(this);
					}
				}
			}
			else
			{
				List<global::Squad> selected_squads2 = battleViewUI.selected_squads;
				if (selected_squads2 != null)
				{
					selected_squads2.Remove(this);
				}
			}
		}
		if (this.logic == null || this.logic.IsDefeated())
		{
			this.m_Selected = false;
		}
		if (!this.m_Previewed)
		{
			if (this.m_Selected)
			{
				this.CreatePathVisualisation();
				return;
			}
			this.DestroyPathArrows();
			this.DestroyPathArrowsStraight();
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00073C24 File Offset: 0x00071E24
	protected virtual void UpdatePreview()
	{
		if (this.logic == null || this.logic.IsDefeated())
		{
			this.m_Previewed = false;
		}
		if (!this.m_Selected)
		{
			if (this.m_Previewed)
			{
				this.CreatePathVisualisation();
				return;
			}
			this.DestroyPathArrows();
			this.DestroyPathArrowsStraight();
		}
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00073C70 File Offset: 0x00071E70
	public void RegisterSelectable()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI != null && battleViewUI.Selection != null)
		{
			battleViewUI.Selection.Register(this);
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00073CA0 File Offset: 0x00071EA0
	public void UnregisterSelectable()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null)
		{
			return;
		}
		if (battleViewUI != null && battleViewUI.Selection != null)
		{
			battleViewUI.Selection.Unregister(this);
			battleViewUI.Deselect(this);
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00073CE4 File Offset: 0x00071EE4
	public unsafe virtual float RayCast(BattleViewUI ui, Ray ray)
	{
		if (this.logic != null && this.logic.IsDefeated())
		{
			return -1f;
		}
		if (this.data == null)
		{
			return -1f;
		}
		if (this.squad_banner != null)
		{
			Collider componentInChildren = this.squad_banner.GetComponentInChildren<Collider>();
			RaycastHit raycastHit;
			if (componentInChildren != null && componentInChildren.Raycast(ray, out raycastHit, 300f))
			{
				return 1f;
			}
		}
		Vector3 a = this.squad_center;
		float num = Vector3.Dot(a - ray.origin, ray.direction);
		if (num < 0f)
		{
			return -1f;
		}
		Vector3 b = ray.origin + ray.direction * num;
		if (ui.picked_passable_area != 0)
		{
			b = ui.picked_passable_area_pos;
		}
		float sqrMagnitude = (a - b).sqrMagnitude;
		float num2 = (this.data_formation == null || this.data->logic_alive <= 1) ? this.def.selection_radius : Mathf.Max(new float[]
		{
			this.data_formation.spacing.x,
			this.data_formation.spacing.y,
			this.def.selection_radius
		});
		float num3 = num2 * num2;
		float num4 = Mathf.Max(num3, this.data->sqr_radius);
		if (sqrMagnitude > num4)
		{
			return -1f;
		}
		float num5 = num3 + 1f;
		Troops.Troop troop = this.data->FirstTroop;
		while (troop <= this.data->LastTroop)
		{
			if (!troop.HasFlags(Troops.Troop.Flags.Dead))
			{
				a = troop.pos3d;
				num = Vector3.Dot(a - ray.origin, ray.direction);
				if (num >= 0f)
				{
					b = ray.origin + ray.direction * num;
					sqrMagnitude = (a - b).sqrMagnitude;
					if (sqrMagnitude <= num3 && sqrMagnitude <= num5)
					{
						num5 = sqrMagnitude;
					}
				}
			}
			troop = ++troop;
		}
		if (num5 > num3)
		{
			return -1f;
		}
		return num5;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00073F09 File Offset: 0x00072109
	public static GameObject BannerPrefab(string def = "Unit")
	{
		return global::Defs.GetObj<GameObject>(def, "squad_banner", null);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00073F17 File Offset: 0x00072117
	public static GameObject IconPrefab(string def = "Unit")
	{
		return global::Defs.GetObj<GameObject>(def, "squad_icon", null);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00073F28 File Offset: 0x00072128
	public unsafe static int LoadBakedSkinningData(Logic.Unit.Def def)
	{
		int num = 0;
		string key = def.field.key;
		if (Troops.sharedSkinningData == null)
		{
			Troops.sharedSkinningData = new Dictionary<string, Troops.SquadModelData>();
		}
		foreach (KeyValuePair<string, Troops.SquadModelData> keyValuePair in Troops.sharedSkinningData)
		{
			if (keyValuePair.Key == key)
			{
				return num;
			}
			num += keyValuePair.Value.per_model_data.Count;
		}
		Troops.SquadModelData squadModelData = new Troops.SquadModelData();
		List<KeyframeTextureBaker.BakedData[]> list = new List<KeyframeTextureBaker.BakedData[]>();
		List<GameObject> list2 = new List<GameObject>();
		new List<int>();
		Troops.sharedSkinningData[key] = squadModelData;
		DT.Field field = def.field.FindChild("model", null, true, true, true, '.');
		int num2 = field.NumValues();
		if (num2 == 0)
		{
			return -1;
		}
		for (int i = 0; i < num2; i++)
		{
			string text = DT.Unquote(field.ValueStr(i));
			GameObject item = Assets.GetObject(text.Replace(".prefab", "_ragdoll.prefab"), null, 1) as GameObject;
			list2.Add(item);
			Assets.AssetInfo asset = Assets.GetAsset(text.Replace(".prefab", ".asset"), false);
			SerializedAnimData serializedAnimData = ((asset != null) ? asset.GetAsset() : null) as SerializedAnimData;
			if (!(serializedAnimData == null))
			{
				list.Add(serializedAnimData.data);
			}
		}
		List<TextureBaker.PerModelData> list3 = Troops.texture_baker.skinning_drawers[key] = new List<TextureBaker.PerModelData>();
		for (int j = 0; j < list.Count; j++)
		{
			KeyframeTextureBaker.BakedData[] array = list[j];
			KeyframeTextureBaker.BakedData bakedData = array[0];
			int num3 = num + j;
			Troops.pdata->texture_width[num3] = (float)bakedData.Texture.width;
			Troops.pdata->baked_anim_count[num3] = bakedData.Animations.Count;
			for (int k = 0; k < bakedData.Animations.Count; k++)
			{
				KeyframeTextureBaker.AnimationClipDataBaked animationClipDataBaked = bakedData.Animations[k];
				Troops.pdata->anim_data[k + num3 * Troops.pdata->NumBakedAnims] = animationClipDataBaked;
			}
			TextureBaker.PerModelData perModelData = new TextureBaker.PerModelData();
			if (list2.Count > j)
			{
				perModelData.ragdoll_prefab = list2[j];
			}
			perModelData.baked_data_id = num3;
			perModelData.model_data_buffer = new GrowBuffer<TextureBaker.InstancedSkinningDrawerBatched.DrawCallData>(Allocator.Persistent, 64);
			perModelData.model_compute_buffer = new ComputeBuffer(10000, TextureBaker.InstancedSkinningDrawerBatched.DrawCallDataSize);
			for (int l = 0; l < array.Length; l++)
			{
				TextureBaker.InstancedSkinningDrawerBatched item2 = new TextureBaker.InstancedSkinningDrawerBatched(array[l], Troops.texture_baker.kingdom_colors, def.battle_scale, true);
				perModelData.drawers.Add(item2);
			}
			squadModelData.per_model_data.Add(perModelData);
			list3.Add(perModelData);
		}
		return num;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000742D9 File Offset: 0x000724D9
	Transform ISelectable.get_transform()
	{
		return base.transform;
	}

	// Token: 0x040007CE RID: 1998
	[HideInInspector]
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x040007CF RID: 1999
	[NonSerialized]
	public Logic.Squad logic;

	// Token: 0x040007D0 RID: 2000
	public Troops.SquadModelData model_data;

	// Token: 0x040007D1 RID: 2001
	[NonSerialized]
	private int data_id = -1;

	// Token: 0x040007D2 RID: 2002
	[NonSerialized]
	private int main_squad_data_id = -1;

	// Token: 0x040007D3 RID: 2003
	private int visibility_index = -1;

	// Token: 0x040007D4 RID: 2004
	private bool visible = true;

	// Token: 0x040007D5 RID: 2005
	private bool inCameraView = true;

	// Token: 0x040007D6 RID: 2006
	[NonSerialized]
	public Formation data_formation;

	// Token: 0x040007D7 RID: 2007
	[NonSerialized]
	public Formation previewFormation;

	// Token: 0x040007D8 RID: 2008
	[NonSerialized]
	public Formation previewFormationConfirmed;

	// Token: 0x040007D9 RID: 2009
	[NonSerialized]
	public Formation last_previewed_formation;

	// Token: 0x040007DA RID: 2010
	public PPos wall_climb_start_pos;

	// Token: 0x040007DB RID: 2011
	public PPos wall_climb_dest_pos;

	// Token: 0x040007DC RID: 2012
	public PPos wall_climb_start_pos_offsetted;

	// Token: 0x040007DD RID: 2013
	public PPos wall_climb_dest_pos_offsetted;

	// Token: 0x040007DE RID: 2014
	public Point wall_normal;

	// Token: 0x040007DF RID: 2015
	[NonSerialized]
	public Formation wall_climb_start_formation;

	// Token: 0x040007E0 RID: 2016
	[NonSerialized]
	public Formation wall_climb_dest_formation;

	// Token: 0x040007E1 RID: 2017
	[NonSerialized]
	public int wall_climb_paid;

	// Token: 0x040007E2 RID: 2018
	[NonSerialized]
	public global::Squad enemy;

	// Token: 0x040007E3 RID: 2019
	[NonSerialized]
	public MapObject ranged_enemy;

	// Token: 0x040007E4 RID: 2020
	[NonSerialized]
	public List<int> control_groups = new List<int>();

	// Token: 0x040007E5 RID: 2021
	public static Mesh control_zone_mesh;

	// Token: 0x040007E6 RID: 2022
	public Material control_zone_material;

	// Token: 0x040007E7 RID: 2023
	private GameObject control_zone_go;

	// Token: 0x040007E8 RID: 2024
	public Skirmish skirmish;

	// Token: 0x040007E9 RID: 2025
	public Material arrow_range_material;

	// Token: 0x040007EA RID: 2026
	private GameObject arrow_range_go;

	// Token: 0x040007EB RID: 2027
	public Color icon_normal;

	// Token: 0x040007EC RID: 2028
	public Color icon_highlighted;

	// Token: 0x040007ED RID: 2029
	private PathArrows path_arrows;

	// Token: 0x040007EE RID: 2030
	private PathArrowsStraight path_arrows_straight;

	// Token: 0x040007EF RID: 2031
	public static bool UseNormalPathArrows = false;

	// Token: 0x040007F0 RID: 2032
	private ParticleSystem[] unpack_particles;

	// Token: 0x040007F1 RID: 2033
	[NonSerialized]
	public int cur_salvo;

	// Token: 0x040007F2 RID: 2034
	public static int debug_squad_id = -1;

	// Token: 0x040007F3 RID: 2035
	public static int debug_troop_id = -1;

	// Token: 0x040007F4 RID: 2036
	public static bool debugging_troop_anims = false;

	// Token: 0x040007F5 RID: 2037
	private StudioEventEmitter marching_sound_emitter;

	// Token: 0x040007F6 RID: 2038
	private EventInstance dying_sound;

	// Token: 0x040007F7 RID: 2039
	private EventInstance dying_sound_horses;

	// Token: 0x040007F8 RID: 2040
	private EventInstance charge_sound;

	// Token: 0x040007F9 RID: 2041
	private EventInstance reload_sound;

	// Token: 0x040007FA RID: 2042
	private EventInstance release_sound;

	// Token: 0x040007FB RID: 2043
	private EventInstance packing_sound;

	// Token: 0x040007FC RID: 2044
	private float next_reload_sound = -1f;

	// Token: 0x040007FD RID: 2045
	public static bool DebugAudio;

	// Token: 0x040007FE RID: 2046
	private bool initted_sound_loops;

	// Token: 0x040007FF RID: 2047
	private global::Squad.VoiceCommand moveCommand;

	// Token: 0x04000800 RID: 2048
	private string style;

	// Token: 0x04000801 RID: 2049
	private float playAudioAt = -1f;

	// Token: 0x04000802 RID: 2050
	private Vector3 playPosition;

	// Token: 0x04000803 RID: 2051
	private Vector3 textPosition;

	// Token: 0x04000804 RID: 2052
	public Transform squad_banner;

	// Token: 0x04000805 RID: 2053
	private Transform squad_icon_transform;

	// Token: 0x04000806 RID: 2054
	private UIBattleViewSquad status_bar;

	// Token: 0x04000807 RID: 2055
	[NonSerialized]
	public Image squad_icon;

	// Token: 0x04000808 RID: 2056
	[NonSerialized]
	public Vector3 squad_center;

	// Token: 0x04000809 RID: 2057
	public List<int> subsquads_data_ids = new List<int>();

	// Token: 0x0400080A RID: 2058
	private int subsquad_path_attempts;

	// Token: 0x0400080B RID: 2059
	private const int max_subsquad_path_attempts = 5;

	// Token: 0x0400080C RID: 2060
	private bool m_isBannerVisibleFilter = true;

	// Token: 0x0400080D RID: 2061
	private bool m_isArmyPlateVisibleFilter = true;

	// Token: 0x0400080E RID: 2062
	private bool m_areMovementArrowsVisibleFilter = true;

	// Token: 0x0400080F RID: 2063
	public bool areSelectionCirclesVisibleFilter = true;

	// Token: 0x04000810 RID: 2064
	private bool m_isShootingRangeIndicatorVisibleFilter = true;

	// Token: 0x04000811 RID: 2065
	public bool m_NameplateTooltipFilter;

	// Token: 0x04000812 RID: 2066
	private int last_alive;

	// Token: 0x04000814 RID: 2068
	private Logic.Squad last_enemy;

	// Token: 0x04000815 RID: 2069
	private string last_march;

	// Token: 0x04000816 RID: 2070
	private float next_charge;

	// Token: 0x04000817 RID: 2071
	private int last_shooting;

	// Token: 0x04000818 RID: 2072
	[HideInInspector]
	public string troops_def_id = "Militia";

	// Token: 0x04000819 RID: 2073
	private List<Vector4> debugSalvoPoints = new List<Vector4>();

	// Token: 0x0400081A RID: 2074
	public bool InDrag;

	// Token: 0x0400081B RID: 2075
	public bool TargetPreviewed;

	// Token: 0x0400081C RID: 2076
	public bool PreviewDirty;

	// Token: 0x0400081D RID: 2077
	public bool subsquad_highlighted;

	// Token: 0x0400081E RID: 2078
	protected bool m_Previewed;

	// Token: 0x0400081F RID: 2079
	protected bool m_Selected;

	// Token: 0x04000820 RID: 2080
	protected bool m_MouseOvered;

	// Token: 0x04000821 RID: 2081
	private bool preSelected;

	// Token: 0x020005C1 RID: 1473
	public enum VoiceCommand
	{
		// Token: 0x04003192 RID: 12690
		Move,
		// Token: 0x04003193 RID: 12691
		Charge,
		// Token: 0x04003194 RID: 12692
		Attack,
		// Token: 0x04003195 RID: 12693
		Shoot,
		// Token: 0x04003196 RID: 12694
		Retreat,
		// Token: 0x04003197 RID: 12695
		Run,
		// Token: 0x04003198 RID: 12696
		Face,
		// Token: 0x04003199 RID: 12697
		Fight,
		// Token: 0x0400319A RID: 12698
		FormLine,
		// Token: 0x0400319B RID: 12699
		FormTriangle,
		// Token: 0x0400319C RID: 12700
		FormWiden,
		// Token: 0x0400319D RID: 12701
		FormShrink,
		// Token: 0x0400319E RID: 12702
		Stop,
		// Token: 0x0400319F RID: 12703
		RefuseOrder,
		// Token: 0x040031A0 RID: 12704
		Count
	}
}
