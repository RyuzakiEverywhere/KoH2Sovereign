using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class Fortification : TroopsObject, IListener
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060008BB RID: 2235 RVA: 0x0005EA3E File Offset: 0x0005CC3E
	// (set) Token: 0x060008BC RID: 2236 RVA: 0x0005EA46 File Offset: 0x0005CC46
	public Vector3 snapped_position { get; private set; }

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060008BD RID: 2237 RVA: 0x0005EA4F File Offset: 0x0005CC4F
	public Vector3 attack_position_a_world
	{
		get
		{
			return base.transform.TransformPoint(this.attack_position_a);
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060008BE RID: 2238 RVA: 0x0005EA62 File Offset: 0x0005CC62
	public Vector3 attack_position_b_world
	{
		get
		{
			return base.transform.TransformPoint(this.attack_position_b);
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnEnable()
	{
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060008C0 RID: 2240 RVA: 0x0005EA78 File Offset: 0x0005CC78
	public unsafe Troops.FortificationData* data
	{
		get
		{
			int id = base.GetID();
			if (!Troops.Initted || id < 0)
			{
				return null;
			}
			return Troops.pdata->GetFortification(id);
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0005EAA8 File Offset: 0x0005CCA8
	public void CreateLogic()
	{
		int battle_side = 1;
		MapObject owner = null;
		if (this.kingdom == 0)
		{
			battle_side = 0;
			owner = BattleMap.battle.attacker;
		}
		else if (this.kingdom == 1)
		{
			battle_side = 1;
			owner = BattleMap.battle.defender;
		}
		if (this.RubbleLeft == null)
		{
			this.RubbleLeft = global::Common.FindChildByName(base.transform.parent.gameObject, "RubbleLeft", true, true);
		}
		if (this.RubbleRight == null)
		{
			this.RubbleRight = global::Common.FindChildByName(base.transform.parent.gameObject, "RubbleRight", true, true);
		}
		for (int i = 0; i < global::Fortification.required_disables.Length; i++)
		{
			GameObject gameObject = global::Common.FindChildByName(base.transform.parent.gameObject, global::Fortification.required_disables[i], true, true);
			if (gameObject != null && !this.disabled_objects.Contains(gameObject))
			{
				this.disabled_objects.Add(gameObject);
			}
		}
		for (int j = 0; j < global::Fortification.required_enables.Length; j++)
		{
			GameObject gameObject2 = global::Common.FindChildByName(base.transform.parent.gameObject, global::Fortification.required_enables[j], true, true);
			if (gameObject2 != null && !this.enabled_objects.Contains(gameObject2))
			{
				this.enabled_objects.Add(gameObject2);
			}
		}
		this.enabled_can_sink = new bool[this.enabled_objects.Count];
		for (int k = 0; k < this.enabled_objects.Count; k++)
		{
			if (!(this.enabled_objects[k] == null))
			{
				PassableArea componentInChildren = this.enabled_objects[k].GetComponentInChildren<PassableArea>();
				this.enabled_can_sink[k] = (componentInChildren == null);
			}
		}
		this.logic = new Logic.Fortification(BattleMap.battle, base.transform.position, owner, battle_side, GameLogic.Get(true).defs.Find<Logic.Fortification.Def>(this.fortification_def_id));
		this.logic.visuals = this;
		this.logic.attack_position_inside_wall = this.attack_position_a_world;
		this.logic.attack_position_outside_wall = this.attack_position_b_world;
		this.kingdom = this.logic.kingdom_id;
		this.FindCapturePoint();
		this.InitGateParticles();
		this.SetActive(true, true);
		PassableAreaManager paManager = BattleMap.Get().paManager;
		paManager.onFinishPathfinding = (PassableAreaManager.OnFinishPathfinding)Delegate.Combine(paManager.onFinishPathfinding, new PassableAreaManager.OnFinishPathfinding(this.SetAreaPaids));
		this.snapped_position = global::Common.SnapToTerrain(base.transform.position, 0f, null, -1f, false);
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0005ED54 File Offset: 0x0005CF54
	private void FindCapturePoint()
	{
		global::CapturePoint componentInChildren = base.GetComponentInChildren<global::CapturePoint>();
		if (((componentInChildren != null) ? componentInChildren.logic : null) != null)
		{
			if (!componentInChildren.logic.fortifications.Contains(this.logic))
			{
				componentInChildren.logic.fortifications.Add(this.logic);
			}
			this.logic.capture_point = componentInChildren.logic;
		}
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0005EDB5 File Offset: 0x0005CFB5
	private void InitGateParticles()
	{
		if (this.logic.def.type != Logic.Fortification.Type.Gate)
		{
			return;
		}
		this.gate_particle_systems = base.GetComponentsInChildren<ParticleSystem>();
		this.UpdateGateParticles(true);
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0005EDE0 File Offset: 0x0005CFE0
	private void UpdateGateParticles(bool instant = false)
	{
		if (this.gate_particle_systems == null || this.gate_particle_systems.Length == 0)
		{
			return;
		}
		bool flag = this.logic != null && this.logic.cur_attackers.Count > 0;
		if (flag)
		{
			bool flag2 = false;
			for (int i = 0; i < this.logic.cur_attackers.Count; i++)
			{
				if (this.logic.cur_attackers[i].is_fighting_target)
				{
					flag2 = true;
					break;
				}
			}
			flag = flag2;
		}
		for (int j = 0; j < this.gate_particle_systems.Length; j++)
		{
			ParticleSystem particleSystem = this.gate_particle_systems[j];
			if (flag && !particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
			else if (!flag && particleSystem.isPlaying)
			{
				particleSystem.Stop(true, instant ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
			}
		}
		if (flag && !this.gate_particles_were_enabled && global::Battle.PlayerBattleSide() == 1)
		{
			Logic.Squad squad = null;
			float num = float.MaxValue;
			List<Logic.Squad> list = this.logic.battle.squads.Get(this.logic.battle_side);
			for (int k = 0; k < list.Count; k++)
			{
				Logic.Squad squad2 = list[k];
				global::Squad squad3 = squad2.visuals as global::Squad;
				if (!(squad3 == null) && squad3.IsVisible())
				{
					float num2 = squad2.position.SqrDist(this.logic.position);
					if (num2 < num)
					{
						num = num2;
						squad = squad2;
					}
				}
			}
			if (squad != null)
			{
				float radius = squad.GetRadius();
				if (num >= radius * 2f)
				{
					Logic.Fortification fortification = this.logic;
					bool flag3;
					if (fortification == null)
					{
						flag3 = (null != null);
					}
					else
					{
						Logic.CapturePoint capture_point = fortification.capture_point;
						flag3 = (((capture_point != null) ? capture_point.ally_squads : null) != null);
					}
					if (!flag3 || !this.logic.capture_point.ally_squads.Contains(squad))
					{
						goto IL_1EF;
					}
				}
				BaseUI.PlayVoiceEvent(squad.def.gates_attacked_voice_line, squad, global::Common.SnapToTerrain(squad.VisualPosition(), 0f, null, -1f, false));
			}
		}
		IL_1EF:
		this.gate_particles_were_enabled = flag;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0005EFE4 File Offset: 0x0005D1E4
	public override bool ReadyToAdd()
	{
		bool result = base.ReadyToAdd();
		if (BattleMap.battle != null && BattleMap.battle.IsValid() && !BattleMap.battle.battle_map_only && !SettlementBV.finished_generation)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0005F024 File Offset: 0x0005D224
	public override void AddToTroops()
	{
		if (Troops.mem_data.NumFortifications == 0)
		{
			Troops.InitFortifications(UnityEngine.Object.FindObjectsOfType<global::Fortification>());
		}
		if (this.logic == null)
		{
			this.CreateLogic();
		}
		this.bounds = new Bounds(this.logic.position, new Vector3(25f, 12.13344f, 4f));
		BoxCollider parentComponent = global::Common.GetParentComponent<BoxCollider>(base.gameObject);
		if (parentComponent != null)
		{
			this.bounds.Encapsulate(parentComponent.bounds);
		}
		if (this.enabled_objects.Count > 0)
		{
			for (int i = 0; i < this.enabled_objects.Count; i++)
			{
				if (!(this.enabled_objects[i] == null))
				{
					Renderer[] componentsInChildren = this.enabled_objects[i].GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						this.bounds.Encapsulate(componentsInChildren[j].bounds);
					}
				}
			}
		}
		this.bounds.size = this.bounds.size * 0.5f;
		this.sqr_radius = this.bounds.extents.x * this.bounds.extents.x + this.bounds.extents.z * this.bounds.extents.z;
		Troops.AddFortification(this);
		this.OnHealthChanged();
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0005F188 File Offset: 0x0005D388
	public void SetActive(bool active, bool recalc_pf = true)
	{
		if (this.active == active)
		{
			return;
		}
		this.active = active;
		if (!active && this.sink_time > 0f)
		{
			this.sink_start_time = UnityEngine.Time.time;
			this.enabled_start_positions = new Vector3[this.enabled_objects.Count];
			this.enabled_end_positions = new Vector3[this.enabled_objects.Count];
			this.sinking = true;
			for (int i = 0; i < this.enabled_objects.Count; i++)
			{
				if (!(this.enabled_objects[i] == null))
				{
					if (this.enabled_can_sink[i])
					{
						this.enabled_start_positions[i] = this.enabled_objects[i].transform.position;
						this.enabled_end_positions[i] = this.enabled_objects[i].transform.position + Vector3.down * this.sink_length * base.transform.lossyScale.y;
					}
					else
					{
						this.enabled_objects[i].SetActive(false);
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < this.enabled_objects.Count; j++)
			{
				if (!(this.enabled_objects[j] == null))
				{
					this.enabled_objects[j].gameObject.SetActive(active);
					if (active && this.enabled_start_positions != null)
					{
						this.enabled_objects[j].transform.position = this.enabled_start_positions[j];
					}
				}
			}
		}
		for (int k = 0; k < this.disabled_objects.Count; k++)
		{
			GameObject gameObject = this.disabled_objects[k];
			if (!(gameObject == null))
			{
				gameObject.gameObject.SetActive(!active);
				if (!active)
				{
					ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
					for (int l = 0; l < componentsInChildren.Length; l++)
					{
						componentsInChildren[l].Play();
					}
				}
			}
		}
		this.CheckRubble();
		if (this.prev != null && this.prev.RubbleRight != null && !this.keep_neighbor_rubble)
		{
			this.prev.CheckRubble();
		}
		if (this.next != null && this.next.RubbleLeft != null && !this.keep_neighbor_rubble)
		{
			this.next.CheckRubble();
		}
		if (recalc_pf)
		{
			this.recheck_pf = true;
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0005F404 File Offset: 0x0005D604
	public void CheckRubble()
	{
		if (this.RubbleLeft != null)
		{
			this.RubbleLeft.SetActive(this.active || this.prev == null || this.prev.active || this.prev.keep_neighbor_rubble);
		}
		if (this.RubbleRight != null)
		{
			GameObject rubbleRight = this.RubbleRight;
			if (rubbleRight == null)
			{
				return;
			}
			rubbleRight.SetActive(this.active || this.next == null || this.next.active || this.next.keep_neighbor_rubble);
		}
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0005F4B0 File Offset: 0x0005D6B0
	private void OnDestroy()
	{
		Troops.DelFortification(this);
		this.DestroySelection(false);
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0005F4C0 File Offset: 0x0005D6C0
	public unsafe override void OnUpdate()
	{
		if (this.data == null)
		{
			return;
		}
		this.UpdateSelection();
		this.UpdateSoundLoop();
		Logic.Fortification fortification = this.logic;
		Logic.Squad last_attacker;
		if (this.data->last_attacker != null)
		{
			global::Squad squad = Troops.squads[this.data->last_attacker->id];
			last_attacker = ((squad != null) ? squad.logic : null);
		}
		else
		{
			last_attacker = null;
		}
		fortification.last_attacker = last_attacker;
		if (!this.active && this.sink_time > 0f && this.sinking)
		{
			float num = (UnityEngine.Time.time - this.sink_start_time) / this.sink_time;
			if (num >= 1f)
			{
				this.sinking = false;
				for (int i = 0; i < this.enabled_objects.Count; i++)
				{
					if (!(this.enabled_objects[i] == null))
					{
						this.enabled_objects[i].SetActive(false);
					}
				}
			}
			else
			{
				for (int j = 0; j < this.enabled_objects.Count; j++)
				{
					if (this.enabled_can_sink[j] && !(this.enabled_objects[j] == null))
					{
						this.enabled_objects[j].transform.position = Vector3.Lerp(this.enabled_start_positions[j], this.enabled_end_positions[j], num);
					}
				}
			}
		}
		if (this.recheck_pf)
		{
			this.recheck_pf = false;
			Logic.Battle battle = BattleMap.battle;
			bool flag;
			if (battle == null)
			{
				flag = (null != null);
			}
			else
			{
				Game batte_view_game = battle.batte_view_game;
				if (batte_view_game == null)
				{
					flag = (null != null);
				}
				else
				{
					Logic.PathFinding path_finding = batte_view_game.path_finding;
					flag = (((path_finding != null) ? path_finding.data : null) != null);
				}
			}
			if (!flag)
			{
				return;
			}
			BattleMap.Get().paManager.RecalculateAreas();
		}
		this.UpdateGateParticles(false);
		if (this.logic.def.type == Logic.Fortification.Type.Gate)
		{
			Logic.CapturePoint capture_point = this.logic.capture_point;
			if (capture_point != null)
			{
				global::CapturePoint capturePoint = capture_point.visuals as global::CapturePoint;
				if (capturePoint != null)
				{
					capturePoint.UpdateFlags();
				}
			}
		}
		Troops.FortificationData.Flags flags = this.data->flags;
		if (flags != this.cur_flags)
		{
			if ((this.cur_flags & Troops.FortificationData.Flags.Started) != Troops.FortificationData.Flags.None)
			{
				if ((this.cur_flags & Troops.FortificationData.Flags.Hit) == Troops.FortificationData.Flags.None && (flags & Troops.FortificationData.Flags.Hit) != Troops.FortificationData.Flags.None)
				{
					this.OnHealthChanged();
				}
				if ((this.cur_flags & Troops.FortificationData.Flags.GateHit) == Troops.FortificationData.Flags.None && (flags & Troops.FortificationData.Flags.GateHit) != Troops.FortificationData.Flags.None)
				{
					this.OnHealthChanged();
				}
			}
			this.cur_flags = this.data->flags;
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0005F702 File Offset: 0x0005D902
	public unsafe void SetHealth(float health)
	{
		if (this.data == null)
		{
			return;
		}
		Troops.CompleteAllJobs();
		this.data->health = health;
		this.OnHealthChanged();
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0005F726 File Offset: 0x0005D926
	public unsafe void SetGateHealth(float health)
	{
		if (this.data == null)
		{
			return;
		}
		Troops.CompleteAllJobs();
		this.data->gate_health = health;
		this.OnGateHealthChanged();
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0005F74A File Offset: 0x0005D94A
	public unsafe void OnHit(float health_loss)
	{
		Troops.CompleteAllJobs();
		this.data->health -= health_loss;
		this.OnHealthChanged();
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0005F768 File Offset: 0x0005D968
	private unsafe void OnHealthChanged()
	{
		Troops.CompleteAllJobs();
		bool flag = this.logic.IsDefeated();
		this.logic.health = this.data->health;
		this.data->SetFlags(Troops.FortificationData.Flags.Demolished, this.logic.health <= 0f);
		if (this.logic.IsDefeated())
		{
			this.SetActive(false, true);
			if (this.logic.def.type == Logic.Fortification.Type.Wall || this.logic.def.type == Logic.Fortification.Type.Gate)
			{
				this.logic.battle.fortification_destroyed = true;
			}
		}
		else
		{
			this.SetActive(true, true);
		}
		if (this.data->HasFlags(Troops.FortificationData.Flags.Hit) && this.logic.IsDefeated() && !flag)
		{
			switch (this.fort_material)
			{
			case global::Fortification.MaterialType.Wood:
				BaseUI.PlaySoundEvent(this.logic.def.DestroyedWoodSound, this.logic.position, null);
				break;
			case global::Fortification.MaterialType.Stone:
				BaseUI.PlaySoundEvent(this.logic.def.DestroyedStoneSound, this.logic.position, null);
				break;
			case global::Fortification.MaterialType.Combination:
				BaseUI.PlaySoundEvent(this.logic.def.DestroyedCombinedSound, this.logic.position, null);
				break;
			}
			int num = global::Battle.PlayerBattleSide();
			string str = (num == 0) ? "we" : "enemy";
			switch (this.logic.def.type)
			{
			case Logic.Fortification.Type.Wall:
			{
				Logic.Army army = this.logic.battle.GetArmy(num);
				BaseUI.PlayCharacterlessVoiceEvent((army != null) ? army.leader : null, "battle_" + str + "_broke_wall");
				break;
			}
			case Logic.Fortification.Type.Gate:
			{
				Logic.Army army2 = this.logic.battle.GetArmy(num);
				BaseUI.PlayCharacterlessVoiceEvent((army2 != null) ? army2.leader : null, "battle_" + str + "_broke_gate");
				break;
			}
			case Logic.Fortification.Type.Tower:
			{
				Logic.Army army3 = this.logic.battle.GetArmy(num);
				BaseUI.PlayCharacterlessVoiceEvent((army3 != null) ? army3.leader : null, "battle_" + str + "_broke_tower");
				break;
			}
			}
		}
		this.OnGateHealthChanged();
		this.data->ClrFlags(Troops.FortificationData.Flags.Hit);
		Logic.Fortification fortification = this.logic;
		if (fortification == null)
		{
			return;
		}
		Logic.Battle battle = fortification.battle;
		if (battle == null)
		{
			return;
		}
		battle.NotifyListeners("fortification_health_changed", null);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0005F9DA File Offset: 0x0005DBDA
	private unsafe void OnGateHealthChanged()
	{
		Troops.CompleteAllJobs();
		if (this.logic.gate != null)
		{
			this.logic.gate.SetHealth(this.data->gate_health);
		}
		this.data->ClrFlags(Troops.FortificationData.Flags.GateHit);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0005FA18 File Offset: 0x0005DC18
	public unsafe void OnMessage(object obj, string message, object param)
	{
		if (message == "reset")
		{
			Logic.Fortification fortification = this.logic;
			if (((fortification != null) ? fortification.gate : null) != null)
			{
				this.logic.cur_attackers.Clear();
				this.UpdateGateParticles(true);
				this.SetGateHealth(this.logic.gate.health);
			}
			if (this.logic != null)
			{
				this.SetHealth(this.logic.health);
			}
			return;
		}
		if (!(message == "check_salvo_passability"))
		{
			if (message == "shoot")
			{
				Logic.Squad squad = (Logic.Squad)param;
				this.ShootArrows(squad.position, squad);
				return;
			}
			if (!(message == "Hit"))
			{
				return;
			}
			this.OnHit((float)param);
			return;
		}
		else
		{
			MapObject mapObject = param as MapObject;
			if (mapObject == null)
			{
				this.logic.arrow_path_is_clear = false;
				return;
			}
			Logic.Squad squad2 = mapObject as Logic.Squad;
			PPos position;
			if (squad2 != null)
			{
				float num = (squad2.movement.speed + 2f) / squad2.movement.speed;
				PPos ppos;
				squad2.CalcPos(out position, out ppos, num, num, true);
			}
			else
			{
				position = mapObject.position;
			}
			Collider[] array = null;
			if (base.transform.parent != null)
			{
				global::Fortification component = base.transform.parent.GetComponent<global::Fortification>();
				Transform transform;
				if (component != null && component.data->type == Logic.Fortification.Type.Gate)
				{
					transform = component.transform.parent;
					if (transform == null)
					{
						transform = component.transform;
					}
				}
				else
				{
					transform = base.transform.parent;
				}
				array = transform.GetComponentsInChildren<Collider>();
				foreach (Collider collider in array)
				{
					if (collider != null)
					{
						collider.enabled = false;
					}
				}
			}
			this.logic.arrow_path_is_clear = this.CheckSalvoPassability(position);
			foreach (Collider collider2 in array)
			{
				if (collider2 != null)
				{
					collider2.enabled = true;
				}
			}
			return;
		}
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0005FC30 File Offset: 0x0005DE30
	public bool CheckSalvoPassability(PPos target_pos)
	{
		SalvoData.Def def = this.logic.game.defs.Get<SalvoData.Def>(this.logic.def.salvo_def);
		Vector3 vector = this.logic.position;
		if (this.logic.position.paID > 0)
		{
			PathData.DataPointers pointers = this.logic.battle.batte_view_game.path_finding.data.pointers;
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
		float num = def.shoot_height;
		vector.y += num;
		vector2.y += 0.5f;
		float magnitude = (new Vector2(vector2.x, vector2.z) - new Vector2(vector.x, vector.z)).magnitude;
		float num2 = vector.y - vector2.y;
		float gravity = def.gravity;
		float num3 = Mathf.Sqrt(magnitude * gravity);
		if (num3 < def.min_shoot_speed)
		{
			num3 = def.min_shoot_speed;
		}
		float num4 = num3 + def.shoot_speed_randomization_mod * num3;
		if (num2 >= 0f)
		{
			num3 = num4;
			float num5 = num3 * num3;
			float num6 = Mathf.Sqrt(num5 * num5 - 2f * num5 * -num2 * gravity - gravity * gravity * magnitude * magnitude);
			float num7 = Mathf.Atan((num5 - num6) / (gravity * magnitude));
			if (num7 > -1.5707964f && num7 < 1.5707964f)
			{
				float num8 = num7;
				if (num8 * 180f / 3.1415927f > def.min_shoot_angle)
				{
					float num9 = Mathf.Cos(num8) * num3;
					float num10 = Mathf.Sin(num8) * num3;
					float num11 = magnitude / num9;
					if (!this.RaycastSalvo(def, vector, vector2, num9, num10, num11, gravity))
					{
						return true;
					}
				}
			}
		}
		else
		{
			num2 *= -1f;
			num3 = num4;
			float num12 = num3 * num3;
			float num13 = Mathf.Sqrt(num12 * num12 - 2f * num12 * -num2 * gravity - gravity * gravity * magnitude * magnitude);
			float num14 = Mathf.Atan((num12 - num13) / (gravity * magnitude));
			bool flag = num14 > -1.5707964f && num14 < 1.5707964f;
			if (flag)
			{
				float num8 = num14;
				float num9 = Mathf.Cos(num8) * num3;
				float num11 = magnitude / num9;
				float num10 = -(Mathf.Sin(num8) * num3 - gravity * num11);
				num8 = Mathf.Atan(num10 / num9);
				flag = (num8 * 180f / 3.1415927f > def.min_shoot_angle);
				if (flag)
				{
					num9 = Mathf.Cos(num8) * num3;
					num11 = magnitude / num9;
					num10 = -(Mathf.Sin(num8) * num3 - gravity * num11);
					if (!this.RaycastSalvo(def, vector, vector2, num9, num10, num11, gravity))
					{
						return true;
					}
				}
			}
			else if (!flag)
			{
				float num8 = 0.7853982f;
				num3 = Mathf.Sqrt(magnitude * gravity);
				if (num3 < def.min_shoot_speed)
				{
					return false;
				}
				float num9 = Mathf.Cos(num8) * num3;
				float num11 = magnitude / num9;
				float num10 = -(Mathf.Sin(num8) * num3 - gravity * num11);
				return !this.RaycastSalvo(def, vector, vector2, num9, num10, num11, gravity);
			}
		}
		return false;
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00060038 File Offset: 0x0005E238
	public bool RaycastSalvo(SalvoData.Def salvo_def, Vector3 start_pos, Vector3 end_pos, float v0x, float v0y, float dur, float g)
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(start_pos);
		int num = 3;
		float num2 = dur / ((float)num + 1f);
		float num3 = 0f;
		for (int i = 0; i < num; i++)
		{
			num3 += num2;
			Vector2 vector = new Vector2(start_pos.x, start_pos.z) + v0x * num3 * (new Vector2(end_pos.x, end_pos.z) - new Vector2(start_pos.x, start_pos.z)).normalized;
			Vector3 vector2 = new Vector3(vector.x, start_pos.y + v0y * num3 - num3 * num3 * g * 0.5f, vector.y);
			if (salvo_def.collision_check_offset != 0f)
			{
				float num4 = (num3 <= dur * 0.5f) ? (num3 / dur * 2f) : ((1f - num3 / dur) * 2f);
				float y = Vector3.Lerp(start_pos, end_pos, num3 / dur).y;
				if (y < vector2.y - salvo_def.collision_check_offset * num4)
				{
					vector2 += new Vector3(0f, salvo_def.collision_check_offset * num4, 0f);
				}
				else
				{
					vector2.y = y;
				}
			}
			list.Add(vector2);
		}
		list.Add(end_pos);
		int mask = LayerMask.GetMask(new string[]
		{
			"Settlements",
			"Terrain"
		});
		for (int j = 0; j < list.Count - 1; j++)
		{
			Vector3 direction = list[j + 1] - list[j];
			if (Physics.Raycast(list[j], direction, direction.magnitude, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00060200 File Offset: 0x0005E400
	public unsafe void ShootArrows(Vector3 tarPos, MapObject enemy)
	{
		Logic.Fortification fortification = this.logic;
		if (((fortification != null) ? fortification.def : null) == null)
		{
			Debug.LogWarning(this + " has a missing def, can't shoot");
			return;
		}
		if (this.data == null)
		{
			return;
		}
		if (this.data->HasFlags(Troops.FortificationData.Flags.Demolished | Troops.FortificationData.Flags.Destroyed | Troops.FortificationData.Flags.Shooting))
		{
			return;
		}
		float f = 0f;
		if (base.transform.parent != null)
		{
			Collider componentInChildren = base.transform.parent.GetComponentInChildren<Collider>();
			f = 0.45f * componentInChildren.bounds.size.x;
		}
		Vector3 v = global::Common.SnapToTerrain(this.logic.position + f * (tarPos - this.logic.position).GetNormalized(), 1f, null, -1f, false) + new Vector3(0f, this.shoot_height, 0f);
		global::Squad squad = ((enemy != null) ? enemy.visuals : null) as global::Squad;
		if (squad != null)
		{
			this.cur_salvo = Troops.ShootSalvo(this.logic, this.logic.shoot_comp.salvo_def, this.logic.CTH_modified * this.logic.battle.simulation.def.global_cth_mod, 0f, v, tarPos, 0, this.data, squad.data);
		}
		else
		{
			this.cur_salvo = Troops.ShootSalvo(this.logic, this.logic.shoot_comp.salvo_def, this.logic.CTH_modified * this.logic.battle.simulation.def.global_cth_mod, 0f, v, tarPos, 0, this.data, null);
		}
		if (this.cur_salvo != -1 && this.data->pdata->GetSalvo(this.cur_salvo)->size > 0)
		{
			SalvoEmitter.SpawnSalvoEmitter(this.cur_salvo, this.logic, this.logic.shoot_comp.salvo_def);
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00060424 File Offset: 0x0005E624
	public void SetAreaPaids()
	{
		PassableAreaManager.SetAreaPaids(base.transform.parent.gameObject, this.logic.game.path_finding, this.logic.paids, null);
		for (int i = 0; i < this.logic.paids.Count; i++)
		{
			int num = this.logic.paids[i];
			PathData.PassableArea pa = this.logic.game.path_finding.data.pointers.GetPA(num - 1);
			if (pa.enabled && pa.connected_to_ground)
			{
				if (pa.Contains(this.logic.attack_position_inside_wall))
				{
					this.logic.attack_position_inside_wall.paID = num;
				}
				if (pa.Contains(this.logic.attack_position_outside_wall))
				{
					this.logic.attack_position_outside_wall.paID = num;
				}
			}
		}
		PassableAreaManager paManager = BattleMap.Get().paManager;
		paManager.onFinishPathfinding = (PassableAreaManager.OnFinishPathfinding)Delegate.Remove(paManager.onFinishPathfinding, new PassableAreaManager.OnFinishPathfinding(this.SetAreaPaids));
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060008D5 RID: 2261 RVA: 0x00060547 File Offset: 0x0005E747
	// (set) Token: 0x060008D6 RID: 2262 RVA: 0x0006054F File Offset: 0x0005E74F
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

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060008D7 RID: 2263 RVA: 0x00060558 File Offset: 0x0005E758
	public bool Highlighted
	{
		get
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			return !(battleViewUI == null) && battleViewUI.SelectionShown() && (this.MouseOvered || (battleViewUI != null && battleViewUI.picked_fortifications != null && (battleViewUI.picked_fortifications[0] == this || battleViewUI.picked_fortifications[1] == this)));
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x000605BB File Offset: 0x0005E7BB
	protected virtual void UpdateSelection()
	{
		if (this.m_Healthbar == null)
		{
			this.CreateSelection(false);
		}
		if (this.m_Healthbar != null)
		{
			this.m_Healthbar.UpdateVisibilityFromObject(this.Highlighted);
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x000605F4 File Offset: 0x0005E7F4
	public void CreateSelection(bool only_enable = false)
	{
		if (this.logic == null)
		{
			return;
		}
		GameObject gameObject = this.logic.def.field.GetRandomValue("healthbar_prefab", null, true, true, true, '.').Get<GameObject>();
		if (gameObject != null)
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			RectTransform rectTransform = (battleViewUI != null) ? battleViewUI.m_statusBar : null;
			if (rectTransform == null)
			{
				return;
			}
			this.m_Healthbar = global::Common.Spawn(gameObject, rectTransform, false, "").GetComponent<UIFortificationHealthbar>();
			this.m_Healthbar.Setup(this.logic);
			this.UpdateSelection();
		}
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00060687 File Offset: 0x0005E887
	public void DestroySelection(bool only_disable = false)
	{
		if (this.m_Healthbar != null)
		{
			if (only_disable)
			{
				this.m_Healthbar.gameObject.SetActive(false);
				return;
			}
			global::Common.DestroyObj(this.m_Healthbar.gameObject);
			this.m_Healthbar = null;
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x000606C4 File Offset: 0x0005E8C4
	public virtual float RayCast(BattleViewUI ui, Ray ray)
	{
		Vector3 snapped_position = this.snapped_position;
		float num = Vector3.Dot(snapped_position - ray.origin, ray.direction);
		if (num < 0f)
		{
			return -1f;
		}
		Vector3 b = ray.origin + ray.direction * num;
		if (ui.picked_passable_area != 0)
		{
			b = ui.picked_passable_area_pos;
		}
		float sqrMagnitude = (snapped_position - b).sqrMagnitude;
		if (sqrMagnitude > this.sqr_radius)
		{
			return -1f;
		}
		return sqrMagnitude;
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0006074C File Offset: 0x0005E94C
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
		if (!string.IsNullOrEmpty(this.logic.def.AttackingMeleeSound))
		{
			this.attacking_gate = base.gameObject.AddComponent<StudioEventEmitter>();
			this.attacking_gate.StopEvent = EmitterGameEvent.ObjectDisable;
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x000607B4 File Offset: 0x0005E9B4
	private void UpdateSoundLoop()
	{
		if (this.data == null)
		{
			return;
		}
		if (!base.isActiveAndEnabled)
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
		if (this.logic.battle.stage <= Logic.Battle.Stage.EnteringBattle)
		{
			return;
		}
		this.InitSoundLoop();
		Logic.Fortification.Def def = this.logic.def;
		string text = null;
		if (this.logic.cur_attackers.Count > 0)
		{
			for (int i = 0; i < this.logic.cur_attackers.Count; i++)
			{
				Logic.Squad squad = this.logic.cur_attackers[i];
				if (squad.is_fighting_target && !squad.def.is_siege_eq)
				{
					text = def.AttackingMeleeSound;
					break;
				}
			}
		}
		if (text != this.last_attacking_gate)
		{
			this.last_attacking_gate = text;
			this.attacking_gate.Stop();
			if (text != null)
			{
				this.attacking_gate.Event = text;
				this.attacking_gate.Play();
				return;
			}
			this.attacking_gate.Event = text;
		}
	}

	// Token: 0x040006DB RID: 1755
	public Logic.Fortification logic;

	// Token: 0x040006DC RID: 1756
	[HideInInspector]
	public string fortification_def_id = "Wall";

	// Token: 0x040006DD RID: 1757
	[Header("Segments")]
	[Tooltip("Active while the wall is active")]
	public List<GameObject> enabled_objects = new List<GameObject>();

	// Token: 0x040006DE RID: 1758
	[Tooltip("Active after the wall has been destroyed")]
	public List<GameObject> disabled_objects = new List<GameObject>();

	// Token: 0x040006DF RID: 1759
	[Tooltip("Specifically toggled depending on whether the neighbouring segments are also destroyed")]
	public GameObject RubbleLeft;

	// Token: 0x040006E0 RID: 1760
	[Tooltip("Specifically toggled depending on whether the neighbouring segments are also destroyed")]
	public GameObject RubbleRight;

	// Token: 0x040006E1 RID: 1761
	public float sink_time = 5f;

	// Token: 0x040006E2 RID: 1762
	public float sink_length = 10f;

	// Token: 0x040006E3 RID: 1763
	[NonSerialized]
	public global::Fortification prev;

	// Token: 0x040006E4 RID: 1764
	[NonSerialized]
	public global::Fortification next;

	// Token: 0x040006E5 RID: 1765
	private Troops.FortificationData.Flags cur_flags;

	// Token: 0x040006E6 RID: 1766
	private bool active;

	// Token: 0x040006E7 RID: 1767
	private bool recheck_pf;

	// Token: 0x040006E8 RID: 1768
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x040006E9 RID: 1769
	[HideInInspector]
	public global::Kingdom.ID kingdom;

	// Token: 0x040006EA RID: 1770
	public Vector3 attack_position_a;

	// Token: 0x040006EB RID: 1771
	public Vector3 attack_position_b;

	// Token: 0x040006EC RID: 1772
	public bool keep_neighbor_rubble;

	// Token: 0x040006ED RID: 1773
	public float shoot_height = 10f;

	// Token: 0x040006EE RID: 1774
	private float sqr_radius;

	// Token: 0x040006F0 RID: 1776
	private UIFortificationHealthbar m_Healthbar;

	// Token: 0x040006F1 RID: 1777
	private static string[] required_enables = new string[]
	{
		"MainRenderer"
	};

	// Token: 0x040006F2 RID: 1778
	private static string[] required_disables = new string[]
	{
		"Leftovers",
		"Explosion"
	};

	// Token: 0x040006F3 RID: 1779
	private const string RubbleLeftKey = "RubbleLeft";

	// Token: 0x040006F4 RID: 1780
	private const string RubbleRightKey = "RubbleRight";

	// Token: 0x040006F5 RID: 1781
	private float sink_start_time;

	// Token: 0x040006F6 RID: 1782
	private bool sinking;

	// Token: 0x040006F7 RID: 1783
	private bool[] enabled_can_sink;

	// Token: 0x040006F8 RID: 1784
	private Vector3[] enabled_start_positions;

	// Token: 0x040006F9 RID: 1785
	private Vector3[] enabled_end_positions;

	// Token: 0x040006FA RID: 1786
	private ParticleSystem[] gate_particle_systems;

	// Token: 0x040006FB RID: 1787
	private StudioEventEmitter attacking_gate;

	// Token: 0x040006FC RID: 1788
	private bool initted_sound_loops;

	// Token: 0x040006FD RID: 1789
	private string last_attacking_gate;

	// Token: 0x040006FE RID: 1790
	public int cur_salvo = -1;

	// Token: 0x040006FF RID: 1791
	public global::Fortification.MaterialType fort_material;

	// Token: 0x04000700 RID: 1792
	public global::Fortification.GateType gate_material;

	// Token: 0x04000701 RID: 1793
	private bool gate_particles_were_enabled;

	// Token: 0x04000702 RID: 1794
	protected bool m_MouseOvered;

	// Token: 0x020005A9 RID: 1449
	public enum MaterialType
	{
		// Token: 0x0400312E RID: 12590
		Wood,
		// Token: 0x0400312F RID: 12591
		Stone,
		// Token: 0x04003130 RID: 12592
		Combination
	}

	// Token: 0x020005AA RID: 1450
	public enum GateType
	{
		// Token: 0x04003132 RID: 12594
		Wood,
		// Token: 0x04003133 RID: 12595
		Metal
	}
}
