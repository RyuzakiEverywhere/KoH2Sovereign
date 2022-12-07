using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class WanderPeasant : GameLogic.Behaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x060011AF RID: 4527 RVA: 0x000BA350 File Offset: 0x000B8550
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x000BA358 File Offset: 0x000B8558
	public static GameObject Prefab(Logic.WanderPeasant.Def def)
	{
		return def.field.GetRandomValue("model", null, true, true, true, '.').Get<GameObject>();
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x000BA384 File Offset: 0x000B8584
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.WanderPeasant wanderPeasant = logic_obj as Logic.WanderPeasant;
		if (wanderPeasant == null)
		{
			return;
		}
		if (global::WanderPeasant.Prefab(wanderPeasant.def) == null)
		{
			return;
		}
		GameObject gameObject = global::Common.SpawnTemplate("WanderPeasant", "WanderPeasant", null, true, new Type[]
		{
			typeof(global::WanderPeasant)
		});
		global::Common.SetObjectParent(gameObject, GameLogic.instance.transform, "Peasants");
		global::WanderPeasant orAddComponent = gameObject.GetOrAddComponent<global::WanderPeasant>();
		if (orAddComponent == null)
		{
			return;
		}
		orAddComponent.instancer = new AnimatorInstancer(BattleMap.Get().texture_baker, wanderPeasant.def.field, null, 0, 1f);
		orAddComponent.cur_target = null;
		orAddComponent.cur_anim = Logic.WanderPeasant.AttractorFlags.Move;
		orAddComponent.last_target = null;
		orAddComponent.spawn_target = null;
		orAddComponent.logic = wanderPeasant;
		wanderPeasant.visuals = orAddComponent;
		orAddComponent.nid = wanderPeasant.GetNid(true);
		orAddComponent.kingdom.id = wanderPeasant.kingdom_id;
		orAddComponent.transform.position = global::Common.SnapToTerrain(wanderPeasant.position, 0f, null, -1f, false);
		int color_id = 0;
		BattleMap battleMap = BattleMap.Get();
		Color rhs;
		if (battleMap.kingdom_colors != null && battleMap.kingdom_colors.TryGetValue(wanderPeasant.kingdom_id, out rhs))
		{
			for (int i = 0; i < battleMap.unit_colors.Length; i++)
			{
				if (battleMap.unit_colors[i] == rhs)
				{
					color_id = i;
					break;
				}
			}
		}
		orAddComponent.instancer.UpdateKingdomColor(color_id);
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x000BA4F4 File Offset: 0x000B86F4
	private void Awake()
	{
		if (Application.isPlaying)
		{
			if (this.logic == null)
			{
				return;
			}
			this.kingdom.id = BattleMap.battle.defender_kingdom.id;
			if (this.logic != null)
			{
				global::Kingdom kingdom = global::Kingdom.Get(this.kingdom.id);
				if (kingdom == null)
				{
					return;
				}
				Color primaryArmyColor = kingdom.PrimaryArmyColor;
				Color secondaryArmyColor = kingdom.SecondaryArmyColor;
				this.rend.sharedMaterial.SetColor("_Color1", primaryArmyColor);
				this.rend.sharedMaterial.SetColor("_Color2", secondaryArmyColor);
			}
		}
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x000BA584 File Offset: 0x000B8784
	public override void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 669907077U)
		{
			if (num != 114379217U)
			{
				if (num != 530911862U)
				{
					if (num != 669907077U)
					{
						return;
					}
					if (!(message == "finished_wait"))
					{
						return;
					}
					this.MoveToNextState();
					return;
				}
				else
				{
					if (!(message == "reached_destination"))
					{
						return;
					}
					this.MoveToNextState();
					return;
				}
			}
			else
			{
				if (!(message == "run_away"))
				{
					return;
				}
				this.PickDestination(true);
				return;
			}
		}
		else
		{
			if (num <= 1649643086U)
			{
				if (num != 1211309691U)
				{
					if (num != 1649643086U)
					{
						return;
					}
					if (!(message == "finishing"))
					{
						return;
					}
				}
				else if (!(message == "destroying"))
				{
					return;
				}
				this.logic.DelListener(this);
				BattleMap battleMap = BattleMap.Get();
				if (battleMap != null)
				{
					List<Logic.WanderPeasant> peasants = battleMap.peasants;
					if (peasants != null)
					{
						peasants.Remove(this.logic);
					}
				}
				this.logic = null;
				global::Common.DestroyObj(base.gameObject);
				return;
			}
			if (num != 2920394832U)
			{
				if (num != 3761154513U)
				{
					return;
				}
				if (!(message == "path_changed"))
				{
					return;
				}
				if (this.logic.movement.path != null && this.logic.movement.path.state == Path.State.Failed)
				{
					this.logic.movement.Stop(true, true, true);
					this.PickDestination(this.logic.running_away);
				}
				return;
			}
			else
			{
				if (!(message == "moved"))
				{
					return;
				}
				this.Moved();
				return;
			}
		}
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x000BA715 File Offset: 0x000B8915
	private void Start()
	{
		if (Application.isPlaying)
		{
			this.visibility_index = VisibilityDetector.Add("BattleView", base.transform.position, 1.75f, null, this, base.gameObject.layer);
		}
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x000BA74C File Offset: 0x000B894C
	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			if (this.visibility_index >= 0)
			{
				VisibilityDetector.Del("BattleView", this.visibility_index);
				this.visibility_index = -1;
			}
			if (this.logic != null)
			{
				Logic.WanderPeasant wanderPeasant = this.logic;
				BattleMap battleMap = BattleMap.Get();
				if (battleMap != null)
				{
					List<Logic.WanderPeasant> peasants = battleMap.peasants;
					if (peasants != null)
					{
						peasants.Remove(wanderPeasant);
					}
				}
				this.logic = null;
				wanderPeasant.Destroy(false);
			}
		}
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x000BA7BC File Offset: 0x000B89BC
	public void VisibilityChanged(bool visible)
	{
		bool flag = this.visible;
		this.visible = visible;
		base.enabled = visible;
		base.gameObject.SetActive(visible);
		if (visible && !flag)
		{
			this.UpdateAnimation(true);
			this.UpdateStateMachine();
		}
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x000BA800 File Offset: 0x000B8A00
	private void Moved()
	{
		if (this.logic == null)
		{
			return;
		}
		this.Moved(this.logic.position, (this.logic.movement.path == null) ? 0f : this.logic.movement.path.t);
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x000BA85C File Offset: 0x000B8A5C
	private void Moved(Point pt, float path_t)
	{
		if (this.logic == null)
		{
			return;
		}
		Vector3 vector = global::Common.SnapToTerrain(pt, 0f, null, -1f, false);
		if (this.logic.position.paID != 0)
		{
			vector.y = this.logic.game.path_finding.data.pointers.GetPA(this.logic.position.paID - 1).GetHeight(vector);
		}
		Vector3 vector2 = vector - base.transform.position;
		vector2.y = 0f;
		base.transform.position = vector;
		if (this.visibility_index >= 0)
		{
			VisibilityDetector.Move("BattleView", this.visibility_index, vector, -1f);
		}
		if (this.visible)
		{
			if (vector2 == Vector3.zero)
			{
				return;
			}
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector2), 90f * UnityEngine.Time.deltaTime);
		}
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x000BA970 File Offset: 0x000B8B70
	public void UpdateStateMachine()
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		bool flag = false;
		switch (this.logic.state)
		{
		case Logic.WanderPeasant.State.None:
			flag = true;
			break;
		case Logic.WanderPeasant.State.MoveToMainPoint:
		case Logic.WanderPeasant.State.MoveToMainPointAfterIdle:
			flag = !this.MoveToTransform(this.cur_target.transform, false);
			break;
		case Logic.WanderPeasant.State.MoveToDestination:
			flag = !this.logic.movement.IsMoving(true);
			break;
		case Logic.WanderPeasant.State.MoveToControlPoint:
			if (this.cur_target.control_transform != null && this.cur_target != this.spawn_target)
			{
				flag = !this.MoveToTransform(this.cur_target.control_transform, true);
			}
			else
			{
				flag = !this.MoveToTransform(this.cur_target.transform, true);
			}
			break;
		}
		if (flag)
		{
			this.MoveToNextState();
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x000BAA58 File Offset: 0x000B8C58
	private bool MoveToTransform(Transform dest, bool rotate)
	{
		return !(dest == null) && this.MoveToTransform(dest.position, dest.rotation, rotate);
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x000BAA88 File Offset: 0x000B8C88
	private bool MoveToTransform(Vector3 dest, Quaternion rot, bool rotate)
	{
		Vector3 vector = dest - base.transform.position;
		bool flag = true;
		if (this.cur_target != null && this.logic.state != Logic.WanderPeasant.State.MoveToDestination && !this.cur_target.snap_to_terrain)
		{
			flag = false;
		}
		if (flag)
		{
			vector.y = 0f;
		}
		float magnitude = vector.magnitude;
		if (magnitude <= 0.01f && this.CheckDespawn())
		{
			return false;
		}
		if (magnitude > 0.01f || (rotate && Quaternion.Angle(base.transform.rotation, rot) > 1f))
		{
			if (magnitude > 0.01f)
			{
				vector /= magnitude;
				if (!flag)
				{
					base.transform.position = base.transform.position + vector * Mathf.Min(magnitude, this.logic.def.move_speed * UnityEngine.Time.deltaTime);
				}
				else
				{
					base.transform.position = global::Common.SnapToTerrain(base.transform.position + vector * Mathf.Min(magnitude, this.logic.def.move_speed * UnityEngine.Time.deltaTime), 0f, null, -1f, false);
				}
				this.logic.SetPosition(base.transform.position);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector), UnityEngine.Time.deltaTime * 360f);
				VisibilityDetector.Move("BattleView", this.visibility_index, base.transform.position, -1f);
			}
			else if (rotate)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, rot, UnityEngine.Time.deltaTime * 90f);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x000BAC60 File Offset: 0x000B8E60
	public void MoveToNextState()
	{
		if (this.logic == null || !this.logic.IsValid())
		{
			return;
		}
		int num = (int)this.logic.state;
		num = (num + 1) % 6;
		if (num < 2)
		{
			num = 2;
		}
		if (this.logic.state == Logic.WanderPeasant.State.MoveToDestination)
		{
			this.logic.movement.Stop(true, true, true);
		}
		this.logic.state = (Logic.WanderPeasant.State)num;
		switch (this.logic.state)
		{
		case Logic.WanderPeasant.State.Idle:
		{
			this.UpdateAnimation(true);
			float duration = Random.Range(this.logic.def.idle_duration_min, this.logic.def.idle_duration_max);
			Attractor.AnimationSettings animationSettings;
			if (Attractor.animation_settings.TryGetValue(this.cur_anim.ToString().ToLowerInvariant(), out animationSettings) && animationSettings.idle_duration_max > 0f)
			{
				duration = Random.Range(animationSettings.idle_duration_min, animationSettings.idle_duration_max);
			}
			this.logic.WaitForAnimation(duration, false);
			return;
		}
		case Logic.WanderPeasant.State.MoveToMainPointAfterIdle:
			this.HandlePickup();
			return;
		case Logic.WanderPeasant.State.MoveToDestination:
			this.PickDestination(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x000BAD78 File Offset: 0x000B8F78
	private void Update()
	{
		if (this.logic.movement.path != null)
		{
			PPos pt;
			float path_t;
			this.logic.movement.CalcPosition(out pt, out path_t);
			this.Moved(pt, path_t);
		}
		this.UpdateAnimation(false);
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x000BADC0 File Offset: 0x000B8FC0
	public void UpdateInstancer(float dt)
	{
		if (this.instancer == null)
		{
			return;
		}
		GameCamera gameCamera = CameraController.GameCamera;
		if (gameCamera == null)
		{
			return;
		}
		this.instancer.Update(dt, base.transform.position, base.transform.rotation, gameCamera.f_planes, false);
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x000BAE10 File Offset: 0x000B9010
	public void UpdateAnimation(bool force_schedule = false)
	{
		Logic.WanderPeasant wanderPeasant = this.logic;
		if (((wanderPeasant != null) ? wanderPeasant.movement : null) != null)
		{
			Logic.WanderPeasant wanderPeasant2 = this.logic;
			if (((wanderPeasant2 != null) ? wanderPeasant2.def : null) != null)
			{
				Logic.WanderPeasant.AttractorFlags attractorFlags = this.cur_anim;
				switch (this.logic.state)
				{
				case Logic.WanderPeasant.State.MoveToMainPoint:
				case Logic.WanderPeasant.State.MoveToMainPointAfterIdle:
				case Logic.WanderPeasant.State.MoveToDestination:
				case Logic.WanderPeasant.State.MoveToControlPoint:
					attractorFlags = this.logic.move_flag;
					break;
				case Logic.WanderPeasant.State.Idle:
					if (force_schedule)
					{
						attractorFlags = this.PickAttractorAnimation();
						if (attractorFlags == Logic.WanderPeasant.AttractorFlags.None)
						{
							this.MoveToNextState();
							return;
						}
					}
					else
					{
						attractorFlags = this.cur_anim;
						if (this.cur_target.radius > 0f)
						{
							bool flag = this.idle_anim_next_pos == Vector3.zero;
							if (UnityEngine.Time.time > this.idle_anim_end_time && flag)
							{
								Vector3 vector;
								if (this.cur_target.control_transform != null)
								{
									vector = this.cur_target.control_transform.position;
								}
								else
								{
									vector = this.cur_target.transform.position;
								}
								vector += Point.RandomOnUnitCircle(this.logic.game) * Random.Range(0f, this.cur_target.radius);
								if (!this.cur_target.snap_to_terrain)
								{
									this.idle_anim_next_pos = vector;
								}
								else
								{
									this.idle_anim_next_pos = global::Common.SnapToTerrain(vector, 0f, null, -1f, false);
								}
							}
							if (!flag)
							{
								if (this.MoveToTransform(this.idle_anim_next_pos, base.transform.rotation, true))
								{
									attractorFlags = Logic.WanderPeasant.AttractorFlags.Move;
								}
								else
								{
									this.idle_anim_next_pos = Vector3.zero;
									attractorFlags = this.PickAttractorAnimation();
									if (attractorFlags == Logic.WanderPeasant.AttractorFlags.None)
									{
										this.MoveToNextState();
										return;
									}
									Attractor.AnimationSettings animationSettings;
									if (Attractor.animation_settings.TryGetValue(attractorFlags.ToString().ToLowerInvariant(), out animationSettings))
									{
										this.idle_anim_end_time = UnityEngine.Time.time + Random.Range(animationSettings.radius_idle_duration_min, animationSettings.radius_idle_duration_max);
									}
									else
									{
										this.idle_anim_end_time = UnityEngine.Time.time + Random.Range(this.logic.def.idle_duration_min, this.logic.def.idle_duration_max);
									}
								}
							}
						}
					}
					break;
				}
				if ((attractorFlags != this.cur_anim || force_schedule) && base.gameObject.activeInHierarchy)
				{
					this.instancer.Play(attractorFlags.ToString(), true, false, 0f);
					this.cur_anim = attractorFlags;
				}
				float anim_speed = 1f;
				if (attractorFlags == Logic.WanderPeasant.AttractorFlags.Move)
				{
					anim_speed = this.logic.movement.modified_speed / this.logic.def.move_speed;
				}
				this.instancer.anim_speed = anim_speed;
				return;
			}
		}
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x000BB0B4 File Offset: 0x000B92B4
	public Logic.WanderPeasant.AttractorFlags PickAttractorAnimation()
	{
		if (this.cur_target == null)
		{
			return this.cur_anim;
		}
		List<Logic.WanderPeasant.AttractorFlags> list = new List<Logic.WanderPeasant.AttractorFlags>();
		for (int i = 4; i <= 8192; i <<= 1)
		{
			Logic.WanderPeasant.AttractorFlags attractorFlags = (Logic.WanderPeasant.AttractorFlags)i;
			if (this.cur_target.HasFlag(attractorFlags) && this.logic.HasFlag(attractorFlags))
			{
				list.Add(attractorFlags);
			}
		}
		if (list.Count == 0)
		{
			return Logic.WanderPeasant.AttractorFlags.None;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x000BB130 File Offset: 0x000B9330
	private bool CheckDespawn()
	{
		if (this.cur_target != null && (this.cur_target != this.spawn_target || this.logic.running_away) && this.cur_target.HasFlag(Logic.WanderPeasant.AttractorFlags.Despawn))
		{
			this.Despawn();
			return true;
		}
		return false;
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x000BB182 File Offset: 0x000B9382
	private void Despawn()
	{
		this.logic.Destroy(false);
		this.cur_target.cur_peasant = null;
		this.cur_target = null;
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x000BB1A4 File Offset: 0x000B93A4
	public void PickDestination(bool run_away)
	{
		if (!run_away || !(this.cur_target != null) || !this.cur_target.HasFlag(Logic.WanderPeasant.AttractorFlags.Despawn))
		{
			BattleMap battleMap = BattleMap.Get();
			Attractor attractor = this.WeightedRandomAttractor(battleMap.attractors, battleMap.IsInsideWall(base.transform.position), run_away);
			if (attractor == null)
			{
				bool flag = true;
				if (this.cur_target == this.spawn_target)
				{
					this.spawn_target = null;
					attractor = this.WeightedRandomAttractor(battleMap.attractors, battleMap.IsInsideWall(base.transform.position), run_away);
					if (attractor != null)
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateAnimation(true);
					return;
				}
			}
			this.SetTarget(attractor);
			this.logic.running_away = run_away;
			if (run_away)
			{
				this.logic.move_flag = Logic.WanderPeasant.AttractorFlags.Run;
			}
			this.spawn_target = null;
			return;
		}
		if (this.cur_target == this.spawn_target)
		{
			this.Despawn();
			return;
		}
		this.logic.running_away = run_away;
		this.logic.move_flag = Logic.WanderPeasant.AttractorFlags.Run;
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x000BB2B4 File Offset: 0x000B94B4
	private float WeightAttractor(Attractor attractor, bool is_inside_wall, bool run_away, bool can_repick_last = false, Logic.WanderPeasant.AttractorFlags move_flag = Logic.WanderPeasant.AttractorFlags.None)
	{
		if (move_flag == Logic.WanderPeasant.AttractorFlags.None)
		{
			move_flag = this.logic.move_flag;
		}
		float num = 0f;
		float num2 = Vector3.Distance(attractor.transform.position, base.transform.position);
		if (num2 > Attractor.max_wander_distance)
		{
			return 0f;
		}
		if (attractor.is_inside_wall != is_inside_wall)
		{
			return 0f;
		}
		if (run_away)
		{
			if (!attractor.HasFlag(Logic.WanderPeasant.AttractorFlags.Despawn))
			{
				return 0f;
			}
			return num + 10f / num2;
		}
		else
		{
			if (attractor == this.cur_target || (!can_repick_last && attractor == this.last_target))
			{
				return 0f;
			}
			if (attractor.cur_peasant != null)
			{
				return 0f;
			}
			if (!attractor.HasFlag(this.logic.def.allowed_animations))
			{
				return 0f;
			}
			if (!attractor.HasLeaveFlag(move_flag))
			{
				return 0f;
			}
			return num + 10f / num2;
		}
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x000BB39C File Offset: 0x000B959C
	private Attractor WeightedRandomAttractor(List<Attractor> attractors, bool is_inside_wall, bool run_away)
	{
		WeightedRandom<Attractor> weightedRandom = new WeightedRandom<Attractor>(attractors.Count, -1);
		for (int i = attractors.Count - 1; i >= 0; i--)
		{
			Attractor attractor = attractors[i];
			weightedRandom.AddOption(attractor, this.WeightAttractor(attractor, is_inside_wall, run_away, false, Logic.WanderPeasant.AttractorFlags.None));
		}
		Attractor attractor2 = weightedRandom.Choose(null, false);
		if (attractor2 != null)
		{
			return attractor2;
		}
		weightedRandom.Clear();
		for (int j = attractors.Count - 1; j >= 0; j--)
		{
			Attractor attractor3 = attractors[j];
			weightedRandom.AddOption(attractor3, this.WeightAttractor(attractor3, is_inside_wall, run_away, true, Logic.WanderPeasant.AttractorFlags.None));
		}
		return weightedRandom.Choose(null, false);
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x000BB43C File Offset: 0x000B963C
	private void HandlePickup()
	{
		Logic.WanderPeasant.AttractorFlags move_flag = Logic.WanderPeasant.AttractorFlags.Move;
		bool flag = true;
		this.idle_anim_next_pos = Vector3.zero;
		if (this.logic.move_flag != Logic.WanderPeasant.AttractorFlags.Move && this.cur_target.HasLeaveFlag(this.logic.move_flag))
		{
			flag = false;
		}
		bool is_inside_wall = BattleMap.Get().IsInsideWall(this.logic.position);
		if (this.cur_target.pickup_flags != Logic.WanderPeasant.AttractorFlags.None)
		{
			BattleMap battleMap = BattleMap.Get();
			List<Logic.WanderPeasant.AttractorFlags> list = new List<Logic.WanderPeasant.AttractorFlags>();
			for (int i = 4; i <= 8192; i <<= 1)
			{
				Logic.WanderPeasant.AttractorFlags attractorFlags = (Logic.WanderPeasant.AttractorFlags)i;
				if ((attractorFlags != this.logic.move_flag || flag) && this.cur_target.HasPickupFlag(attractorFlags) && this.logic.HasMoveFlag(attractorFlags))
				{
					bool flag2 = false;
					for (int j = 0; j < battleMap.attractors.Count; j++)
					{
						Attractor attractor = battleMap.attractors[j];
						if (!(attractor == this.cur_target) && this.WeightAttractor(attractor, is_inside_wall, false, false, attractorFlags) != 0f)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						list.Add(attractorFlags);
					}
				}
			}
			if (list.Count != 0)
			{
				move_flag = list[Random.Range(0, list.Count)];
			}
		}
		this.logic.move_flag = move_flag;
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x000BB591 File Offset: 0x000B9791
	private void LeaveCurTarget()
	{
		if (this.cur_target == null)
		{
			return;
		}
		this.cur_target.cur_peasant = null;
		this.last_target = this.cur_target;
		this.cur_target = null;
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x000BB5C4 File Offset: 0x000B97C4
	private void SetTarget(Attractor new_target)
	{
		this.LeaveCurTarget();
		this.cur_target = new_target;
		new_target.cur_peasant = this.logic;
		this.logic.movement.MoveTo(new_target.transform.position, 1.75f, false, true, false, false);
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x000BB613 File Offset: 0x000B9813
	public int GetKingdomID()
	{
		if (this.logic == null)
		{
			return this.kingdom;
		}
		return this.logic.kingdom_id;
	}

	// Token: 0x04000BEF RID: 3055
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x04000BF0 RID: 3056
	public const float Radius = 1.75f;

	// Token: 0x04000BF1 RID: 3057
	public Renderer rend;

	// Token: 0x04000BF2 RID: 3058
	private int visibility_index = -1;

	// Token: 0x04000BF3 RID: 3059
	private bool visible = true;

	// Token: 0x04000BF4 RID: 3060
	private float idle_anim_end_time;

	// Token: 0x04000BF5 RID: 3061
	private Vector3 idle_anim_next_pos;

	// Token: 0x04000BF6 RID: 3062
	public AnimatorInstancer instancer;

	// Token: 0x04000BF7 RID: 3063
	[EventRef]
	public string MarchingSound;

	// Token: 0x04000BF8 RID: 3064
	public Logic.WanderPeasant logic;

	// Token: 0x04000BF9 RID: 3065
	[NonSerialized]
	public Attractor spawn_target;

	// Token: 0x04000BFA RID: 3066
	[NonSerialized]
	public Attractor cur_target;

	// Token: 0x04000BFB RID: 3067
	[NonSerialized]
	private Attractor last_target;

	// Token: 0x04000BFC RID: 3068
	private Logic.WanderPeasant.AttractorFlags cur_anim = Logic.WanderPeasant.AttractorFlags.Move;
}
