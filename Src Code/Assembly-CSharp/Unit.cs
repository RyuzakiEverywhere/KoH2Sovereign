using System;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class Unit
{
	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06003131 RID: 12593 RVA: 0x0018DDD5 File Offset: 0x0018BFD5
	// (set) Token: 0x06003132 RID: 12594 RVA: 0x0018DDDD File Offset: 0x0018BFDD
	public bool enabled { get; private set; }

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06003133 RID: 12595 RVA: 0x0018DDE6 File Offset: 0x0018BFE6
	public static int unit_layer
	{
		get
		{
			if (global::Unit._unit_layer == -1)
			{
				global::Unit._unit_layer = LayerMask.NameToLayer("Armies");
			}
			return global::Unit._unit_layer;
		}
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x0018DE04 File Offset: 0x0018C004
	public Unit(TextureBaker texture_baker, Transform parent)
	{
		this.texture_baker = texture_baker;
		this.parent = parent;
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x0018DE6C File Offset: 0x0018C06C
	public void Enable(bool enabled, bool force = false)
	{
		if (this.enabled == enabled && !force)
		{
			return;
		}
		this.enabled = enabled;
		if (enabled && !this.can_move && this.logic.IsDefeated())
		{
			this.UpdateAnimation(true);
			this.instancer.Play(this.current_anim.ToString(), false, true, 0f);
		}
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x0018DED0 File Offset: 0x0018C0D0
	public static global::Unit Get(Logic.Unit lu)
	{
		if (lu == null)
		{
			return null;
		}
		global::Army army = global::Army.Get(lu.army);
		if (army == null)
		{
			return null;
		}
		return army.GetUnit(lu);
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x0018DF00 File Offset: 0x0018C100
	public global::Army GetArmy()
	{
		if (this.logic != null && this.logic.army != null)
		{
			return this.logic.army.visuals as global::Army;
		}
		return null;
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0018DF2E File Offset: 0x0018C12E
	public global::Settlement GetSettlement()
	{
		if (this.logic != null && this.logic.garrison != null)
		{
			return this.logic.garrison.settlement.visuals as global::Settlement;
		}
		return null;
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x0018DF64 File Offset: 0x0018C164
	public global::Battle GetBattle()
	{
		global::Army army = this.GetArmy();
		if (army != null)
		{
			Logic.Army army2 = army.logic;
			object obj;
			if (army2 == null)
			{
				obj = null;
			}
			else
			{
				Logic.Battle battle = army2.battle;
				obj = ((battle != null) ? battle.visuals : null);
			}
			return obj as global::Battle;
		}
		global::Settlement settlement = this.GetSettlement();
		object obj2;
		if (settlement == null)
		{
			obj2 = null;
		}
		else
		{
			Logic.Settlement settlement2 = settlement.logic;
			if (settlement2 == null)
			{
				obj2 = null;
			}
			else
			{
				Logic.Battle battle2 = settlement2.battle;
				obj2 = ((battle2 != null) ? battle2.visuals : null);
			}
		}
		return obj2 as global::Battle;
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x0018DFD4 File Offset: 0x0018C1D4
	public bool IsMoving()
	{
		return !this.dest_pos.Equals(float3.zero);
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600313B RID: 12603 RVA: 0x0018DFEC File Offset: 0x0018C1EC
	public float scale
	{
		get
		{
			Logic.Unit unit = this.logic;
			if (((unit != null) ? unit.def : null) == null || !this.can_move)
			{
				return 1f;
			}
			if (this.logic.def.type != Logic.Unit.Type.Noble)
			{
				return WV_Scale.GetSize(WV_Scale.Object_Type.Unit);
			}
			return WV_Scale.GetSize(WV_Scale.Object_Type.Marshal);
		}
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x0018E03C File Offset: 0x0018C23C
	public void UpdateModel()
	{
		DT.Field field = this.logic.def.field;
		string text = field.key;
		if (this.logic.simulation != null && this.logic.simulation.state == BattleSimulation.Squad.State.Fled)
		{
			text = this.logic.def.surrender_def_key;
		}
		if (this.instancer == null || text != this.instancer.model_key)
		{
			if (field.key != text)
			{
				field = global::Defs.Get(false).dt.Find(text, null);
			}
			float scale = this.scale;
			if (this.instancer == null)
			{
				this.instancer = new AnimatorInstancer(this.texture_baker, field, null, global::Unit.unit_layer, scale);
				return;
			}
			this.instancer.SetData(this.texture_baker, field, null, global::Unit.unit_layer, scale);
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x0018E110 File Offset: 0x0018C310
	public void Refresh()
	{
		if (this.texture_baker == null)
		{
			return;
		}
		this.UpdateModel();
		this.GetCollisionRadius();
		this.dest_pos = Vector3.zero;
		this.block_new_anim = false;
		this.UpdateColors();
		if (this.logic != null)
		{
			this.last_damage = this.logic.damage;
		}
		if (this.hit_particles != null)
		{
			this.hit_particles.SetActive(false);
		}
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x0018E184 File Offset: 0x0018C384
	public void UpdateColors()
	{
		global::Army army = this.GetArmy();
		Logic.Kingdom kingdom;
		if (army == null)
		{
			kingdom = null;
		}
		else
		{
			Logic.Army army2 = army.logic;
			kingdom = ((army2 != null) ? army2.GetKingdom() : null);
		}
		Logic.Kingdom kingdom2 = kingdom;
		if (kingdom2 == null)
		{
			Logic.Unit unit = this.logic;
			object obj;
			if (unit == null)
			{
				obj = null;
			}
			else
			{
				Garrison garrison = unit.garrison;
				obj = ((garrison != null) ? garrison.settlement : null);
			}
			object obj2 = obj;
			kingdom2 = ((obj2 != null) ? obj2.GetKingdom() : null);
			if (kingdom2 == null)
			{
				return;
			}
		}
		global::Kingdom kingdom3 = kingdom2.visuals as global::Kingdom;
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		int color_id;
		if (kingdom3 != null)
		{
			color_id = kingdom3.unitColorID;
		}
		else
		{
			color_id = global::Kingdom.PickClosestColorID(worldMap.unit_colors, global::Defs.GetColor(kingdom2.def, "primary_color", null));
		}
		this.instancer.UpdateKingdomColor(color_id);
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x000023FD File Offset: 0x000005FD
	public void SetColors(Color primary)
	{
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x0018E238 File Offset: 0x0018C438
	public float GetCollisionRadius()
	{
		if (this.collision_radius > 0f)
		{
			Logic.Unit unit = this.logic;
			if (((unit != null) ? unit.def : null) != null && this.collision_radius == this.logic.def.radius)
			{
				return this.collision_radius * this.scale;
			}
		}
		Logic.Unit unit2 = this.logic;
		this.collision_radius = ((((unit2 != null) ? unit2.def : null) != null) ? this.logic.def.radius : 1f);
		return this.collision_radius * this.scale;
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x0018E2CC File Offset: 0x0018C4CC
	private void OnHit(ref UnitAnimation.State new_anim)
	{
		if (this.hit_particles == null && this.logic != null)
		{
			GameObject prefab = this.logic.def.field.GetRandomValue("hit_particles", null, true, true, true, '.').Get<GameObject>();
			this.hit_particles = global::Common.Spawn(prefab, GameLogic.instance.transform, false, "Particles");
			this.hit_particles.transform.position = this.instancer.Position;
			this.hit_particles.transform.rotation = Quaternion.identity;
			this.hit_particles.transform.localScale = Vector3.one;
			this.hit_particles.SetLayer(global::Unit.unit_layer, true);
		}
		if (this.hit_particles != null)
		{
			this.hit_particles.transform.position = this.instancer.Position;
			this.hit_particles.SetActive(true);
			foreach (ParticleSystem particleSystem in this.hit_particles.GetComponentsInChildren<ParticleSystem>())
			{
				particleSystem.time = 0f;
				particleSystem.Play();
			}
		}
		new_anim = this.current_anim;
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x0018E404 File Offset: 0x0018C604
	public void BlockNewAnim(bool val)
	{
		this.block_new_anim = val;
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x0018E410 File Offset: 0x0018C610
	public void UpdateAnimation(bool add_anim = true)
	{
		if (this.block_new_anim || !this.enabled)
		{
			return;
		}
		bool loop = true;
		UnitAnimation.State state = UnitAnimation.State.Idle;
		float offset = -1f;
		float3 position = this.instancer.Position;
		if (this.logic != null && this.logic.army != null)
		{
			if (this.logic.army.movement.IsMoving(false))
			{
				if ((this.IsMoving() && this.dest_speed > 0f) || (UnityEngine.Time.time < this.next_pause_time && this.current_anim == UnitAnimation.State.Move))
				{
					state = UnitAnimation.State.Move;
				}
				else
				{
					this.next_pause_time = UnityEngine.Time.time + 0.5f;
				}
			}
			else if (this.can_move && this.IsMoving() && this.dest_pos.x != position.x && this.dest_pos.z != position.z && this.dest_speed > 0f)
			{
				state = UnitAnimation.State.Move;
			}
		}
		float num = this.last_damage;
		this.last_damage = this.logic.damage;
		if (this.last_damage > num)
		{
			this.OnHit(ref state);
		}
		else if (state != UnitAnimation.State.Move)
		{
			Logic.Unit unit = this.logic;
			if (((unit != null) ? unit.simulation : null) != null)
			{
				switch (this.logic.simulation.state)
				{
				case BattleSimulation.Squad.State.Idle:
					if (this.current_anim == UnitAnimation.State.SpecialReady || this.current_anim == UnitAnimation.State.SpecialAttack)
					{
						state = UnitAnimation.State.SpecialReady;
						goto IL_1B8;
					}
					state = UnitAnimation.State.Ready;
					goto IL_1B8;
				case BattleSimulation.Squad.State.Moving:
					state = UnitAnimation.State.Move;
					goto IL_1B8;
				case BattleSimulation.Squad.State.Attacking:
					state = UnitAnimation.State.Attack;
					goto IL_1B8;
				case BattleSimulation.Squad.State.Shooting:
					state = UnitAnimation.State.SpecialAttack;
					goto IL_1B8;
				case BattleSimulation.Squad.State.Fled:
					if (this.onFlee != null)
					{
						this.onFlee();
					}
					state = UnitAnimation.State.Idle;
					goto IL_1B8;
				case BattleSimulation.Squad.State.Dead:
					state = UnitAnimation.State.Death;
					loop = false;
					goto IL_1B8;
				}
				state = UnitAnimation.State.Ready;
			}
		}
		IL_1B8:
		if (this.logic != null && this.logic.army != null)
		{
			float anim_speed = 1f;
			if (this.can_move && state == UnitAnimation.State.Move)
			{
				WV_Scale wv_Scale = WV_Scale.Get();
				anim_speed = this.logic.speed_mod * (1f + (this.scale - 1f) * wv_Scale.def.unit_speed_bias);
			}
			this.instancer.anim_speed = anim_speed;
			offset = this.logic.army.game.Random(0f, 0.6f);
		}
		if (state != this.current_anim)
		{
			this.current_anim = state;
			this.instancer.Play(state.ToString(), loop, false, offset);
		}
		if (add_anim)
		{
			GameCamera gameCamera = CameraController.GameCamera;
			this.instancer.Update(UnityEngine.Time.deltaTime, gameCamera.f_planes, !this.can_move);
		}
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x0018E6B8 File Offset: 0x0018C8B8
	public void SetFacing(float angle, bool instant = false)
	{
		if (instant && angle >= 0f)
		{
			Quaternion rotation = this.instancer.Rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.y = angle;
			rotation.eulerAngles = eulerAngles;
			this.instancer.Rotation = rotation;
			this.tgt_facing = -1f;
			return;
		}
		this.tgt_facing = angle;
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x0018E714 File Offset: 0x0018C914
	public void SetFacing(Vector3 v, bool instant = false)
	{
		v.y = 0f;
		if (v.magnitude < 0.001f)
		{
			return;
		}
		float num = Mathf.Atan2(v.z, v.x) * 57.29578f;
		num = 90f - num;
		if (num < 0f)
		{
			num += 360f;
		}
		this.SetFacing(num, instant);
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x0018E774 File Offset: 0x0018C974
	public void LookAt(float3 pt, bool instant = false)
	{
		if (this.instancer == null)
		{
			return;
		}
		Vector3 v = pt - this.instancer.Position;
		this.SetFacing(v, instant);
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x0018E7AC File Offset: 0x0018C9AC
	private void UpdateFacing()
	{
		if (this.tgt_facing < 0f)
		{
			return;
		}
		global::Army army = this.GetArmy();
		Quaternion rotation = this.instancer.Rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		float num = eulerAngles.y - this.tgt_facing;
		if (num < -180f)
		{
			num += 360f;
		}
		else if (num > 180f)
		{
			num -= 360f;
		}
		Logic.Unit unit = this.logic;
		float num2 = (((unit != null) ? unit.def : null) == null) ? 90f : this.logic.def.rotation_speed;
		if (this.logic.simulation != null)
		{
			num2 = 360f;
		}
		if ((num < -120f || num > 120f) && (army == null || army.units.Count == 1 || (army.GetFormation() != global::Army.Formation.Thread && this.logic.army.movement.path != null)))
		{
			eulerAngles.y = this.tgt_facing;
		}
		else
		{
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, this.tgt_facing, Mathf.Clamp(num2 - (num2 - Mathf.Abs(num)), num2 / 1.5f, num2 * 1.25f) * UnityEngine.Time.deltaTime);
		}
		rotation.eulerAngles = eulerAngles;
		this.instancer.Rotation = rotation;
		if (eulerAngles.y == this.tgt_facing)
		{
			this.tgt_facing = -1f;
		}
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x0018E915 File Offset: 0x0018CB15
	public void SetPosition(Vector3 pos)
	{
		this.instancer.Position = (this.can_move ? global::Common.SnapToTerrain(pos, 0f, null, -1f, false) : pos);
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x0018E944 File Offset: 0x0018CB44
	public void SetRotation(Quaternion rot)
	{
		this.instancer.Rotation = rot;
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x0018E954 File Offset: 0x0018CB54
	public bool IsLeader()
	{
		global::Army army = this.GetArmy();
		return ((army != null) ? army.units : null) != null && army.units.Count != 0 && army.units[0] == this;
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x0018E994 File Offset: 0x0018CB94
	public void MoveTowards(Vector3 dst, float speed, bool look_at_dst)
	{
		if (this.logic.simulation == null)
		{
			Logic.Unit unit = this.logic;
			bool flag;
			if (unit == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.Army army = unit.army;
				if (army == null)
				{
					flag = (null != null);
				}
				else
				{
					Game game = army.game;
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
			}
			if (flag && !this.IsLeader() && !this.logic.army.game.path_finding.data.IsPassable(dst, 0f))
			{
				this.dest_pos = this.instancer.Position;
				return;
			}
		}
		this.dest_pos = dst;
		this.dest_speed = speed;
		if (this.logic.simulation != null)
		{
			this.dest_speed /= this.logic.simulation.simulation.def.duration_mod;
		}
		if (look_at_dst)
		{
			this.LookAt(dst, false);
		}
		if (speed == float.PositiveInfinity)
		{
			this.SetPosition(dst);
			this.dest_speed = 0f;
		}
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x0018EAA0 File Offset: 0x0018CCA0
	public void UpdateFinalFacing()
	{
		Logic.Unit unit = this.logic;
		bool flag;
		if (unit == null)
		{
			flag = (null != null);
		}
		else
		{
			BattleSimulation.Squad simulation = unit.simulation;
			flag = (((simulation != null) ? simulation.target : null) != null);
		}
		if (flag)
		{
			this.LookAt(this.logic.simulation.target.world_pos, false);
			return;
		}
		global::Battle battle = this.GetBattle();
		if (!(battle != null))
		{
			return;
		}
		if (!battle.logic.is_plunder)
		{
			this.LookAt(battle.transform.position, false);
			return;
		}
		int side = 1 - this.battle_side;
		Logic.Army army = battle.logic.GetArmy(side);
		global::Army army2 = ((army != null) ? army.visuals : null) as global::Army;
		if (army2 != null)
		{
			this.LookAt(army2.transform.position, false);
			return;
		}
		this.LookAt(battle.transform.position, false);
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x0018EB88 File Offset: 0x0018CD88
	private void UpdateMoveTowards()
	{
		float3 position = this.instancer.Position;
		float3 @float = this.dest_pos - position;
		@float.y = 0f;
		float num = math.length(@float);
		float num2 = this.dest_speed * UnityEngine.Time.deltaTime;
		bool flag = num <= num2;
		if (!flag)
		{
			@float *= num2 / num;
		}
		Vector3 position2 = position + @float;
		if (flag)
		{
			this.UpdateFinalFacing();
			if (num <= 0f)
			{
				this.dest_pos = Vector3.zero;
			}
		}
		this.SetPosition(position2);
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x0018EC18 File Offset: 0x0018CE18
	public void SetLogic(Logic.Unit logic)
	{
		this.logic = logic;
		if (logic != null)
		{
			this.type = logic.def.id;
			global::Army army = this.GetArmy();
			if (((army != null) ? army.logic : null) != null)
			{
				this.battle_side = army.logic.battle_side;
			}
			else
			{
				global::Settlement settlement = this.GetSettlement();
				if (((settlement != null) ? settlement.logic : null) != null)
				{
					this.battle_side = 1;
				}
			}
			this.Refresh();
		}
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x0018EC89 File Offset: 0x0018CE89
	private void UpdateMovement()
	{
		if (!this.dest_pos.Equals(float3.zero) && this.dest_speed > 0f)
		{
			this.UpdateMoveTowards();
			return;
		}
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x0018ECB1 File Offset: 0x0018CEB1
	public void Update()
	{
		if (this.logic == null)
		{
			return;
		}
		this.UpdateModel();
		if (!this.enabled)
		{
			return;
		}
		if (this.can_move)
		{
			this.UpdateMovement();
			this.UpdateFacing();
		}
		this.UpdateAnimation(true);
	}

	// Token: 0x040020E1 RID: 8417
	private static int _unit_layer = -1;

	// Token: 0x040020E2 RID: 8418
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x040020E3 RID: 8419
	public TextureBaker texture_baker;

	// Token: 0x040020E4 RID: 8420
	public Logic.Unit logic;

	// Token: 0x040020E5 RID: 8421
	public Transform parent;

	// Token: 0x040020E6 RID: 8422
	private int battle_side;

	// Token: 0x040020E7 RID: 8423
	public string type = "";

	// Token: 0x040020E8 RID: 8424
	public bool can_move = true;

	// Token: 0x040020E9 RID: 8425
	private float collision_radius = -1f;

	// Token: 0x040020EA RID: 8426
	[NonSerialized]
	public GameObject hit_particles;

	// Token: 0x040020EB RID: 8427
	public AnimatorInstancer instancer;

	// Token: 0x040020EC RID: 8428
	public float3 dest_pos = Vector3.zero;

	// Token: 0x040020ED RID: 8429
	public float dest_speed;

	// Token: 0x040020EE RID: 8430
	public float tgt_facing = -1f;

	// Token: 0x040020EF RID: 8431
	private float next_pause_time;

	// Token: 0x040020F0 RID: 8432
	private const float min_pause_time = 0.5f;

	// Token: 0x040020F1 RID: 8433
	private float last_damage;

	// Token: 0x040020F2 RID: 8434
	private bool block_new_anim;

	// Token: 0x040020F3 RID: 8435
	private UnitAnimation.State current_anim;

	// Token: 0x040020F4 RID: 8436
	public global::Unit.OnFlee onFlee;

	// Token: 0x02000877 RID: 2167
	// (Invoke) Token: 0x0600514B RID: 20811
	public delegate void OnFlee();
}
