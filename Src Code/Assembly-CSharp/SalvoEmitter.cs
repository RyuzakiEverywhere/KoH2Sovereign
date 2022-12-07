using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class SalvoEmitter : MonoBehaviour
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060008F5 RID: 2293 RVA: 0x000616BC File Offset: 0x0005F8BC
	public static int physics_layer
	{
		get
		{
			if (SalvoEmitter._physics_layer == -1)
			{
				SalvoEmitter._physics_layer = LayerMask.NameToLayer("Physics");
			}
			return SalvoEmitter._physics_layer;
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x000616DC File Offset: 0x0005F8DC
	public unsafe static void SpawnSalvoEmitter(int salvo, MapObject squad, SalvoData.Def def)
	{
		if (salvo < 0)
		{
			return;
		}
		if (Troops.squads == null)
		{
			return;
		}
		if (Troops.pdata->GetSalvo(salvo)->HasFlags(Troops.SalvoData.Flags.HasFinishedActionFrameArrow))
		{
			return;
		}
		GameObject gameObject = def.field.GetRandomValue("projectile_particles", null, true, true, true, '.').Get<GameObject>();
		SalvoEmitter salvoEmitter = global::Common.SpawnTemplate<SalvoEmitter>("SalvoEmitter", "SalvoEmitter", null, true, new Type[]
		{
			typeof(SalvoEmitter)
		});
		salvoEmitter.projectile_land_particles = gameObject;
		salvoEmitter.cur_salvo = salvo;
		salvoEmitter.def = def;
		salvoEmitter.obj = squad;
		salvoEmitter.has_played_shoot_sound = false;
		if (squad != null)
		{
			salvoEmitter.transform.position = global::Common.SnapToTerrain(squad.position, 0f, null, -1f, false);
		}
		salvoEmitter.attributes.position = salvoEmitter.transform.position.ToFMODVector();
		salvoEmitter.attributes.up = Vector3.up.ToFMODVector();
		salvoEmitter.attributes.forward = Vector3.forward.ToFMODVector();
		salvoEmitter.salvo_colliders = new List<SalvoEmitter.SalvoCollider>();
		salvoEmitter.gameObject.SetLayer(SalvoEmitter.physics_layer, true);
		Logic.Squad squad2 = squad as Logic.Squad;
		if (squad2 != null)
		{
			(squad2.visuals as global::Squad).PlayReloadSound();
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00061818 File Offset: 0x0005FA18
	private unsafe void CreateParticles()
	{
		Troops.SalvoData* salvo = Troops.pdata->GetSalvo(this.cur_salvo);
		GameObject gameObject = this.def.field.GetRandomValue("particles", null, true, true, true, '.').Get<GameObject>();
		Troops.Arrow arrow = salvo->FirstArrow;
		while (arrow <= salvo->LastActiveArrow)
		{
			SalvoEmitter.SalvoCollider component = global::Common.SpawnTemplate("SalvoCollider", "SalvoCollider", base.transform, true, new Type[]
			{
				typeof(SalvoEmitter.SalvoCollider),
				typeof(SphereCollider),
				typeof(Rigidbody)
			}).GetComponent<SalvoEmitter.SalvoCollider>();
			SphereCollider component2 = component.GetComponent<SphereCollider>();
			Rigidbody component3 = component.GetComponent<Rigidbody>();
			component2.radius = this.def.projectile_radius;
			component3.isKinematic = true;
			component.parent = this;
			component.has_collided = false;
			component.gameObject.SetLayer(SalvoEmitter.physics_layer, true);
			this.salvo_colliders.Add(component);
			if (gameObject != null)
			{
				ParticleSystem component4 = global::Common.Spawn(gameObject, component.transform, false, "").GetComponent<ParticleSystem>();
				component.main_particles = component4;
				component.all_particles = component.GetComponentsInChildren<ParticleSystem>();
				component4.transform.localPosition = Vector3.zero;
				component4.transform.localRotation = Quaternion.identity;
				component4.transform.localScale = Vector3.one;
			}
			if (this.def.arrows_per_troop <= 1)
			{
				break;
			}
			arrow = ++arrow;
		}
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x0006199C File Offset: 0x0005FB9C
	private unsafe void PlayShootSound()
	{
		if (!string.IsNullOrEmpty(this.def.arrow_shoot_sound_effect))
		{
			Troops.SalvoData* salvo = Troops.pdata->GetSalvo(this.cur_salvo);
			this.shoot_sound = FMODWrapper.CreateInstance(this.def.arrow_shoot_sound_effect, false);
			this.shoot_sound.setParameterByName("NumArrows", (float)salvo->num_arrows, false);
			this.shoot_sound.set3DAttributes(this.attributes);
			this.shoot_sound.start();
		}
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00061A1A File Offset: 0x0005FC1A
	private void OnDestroy()
	{
		this.Stop();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00061A24 File Offset: 0x0005FC24
	private unsafe void Update()
	{
		if (this.cur_salvo == -1 || BattleMap.battle == null || BattleMap.battle.winner >= 0)
		{
			this.Stop();
			return;
		}
		Troops.SalvoData* salvo = Troops.pdata->GetSalvo(this.cur_salvo);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		Troops.Arrow arrow = salvo->FirstArrow;
		Troops.Arrow arrow2 = salvo->FirstArrow;
		while (arrow2 <= salvo->LastActiveArrow)
		{
			if (arrow2.HasFlags(Troops.Arrow.Flags.Active))
			{
				flag = true;
			}
			if (arrow2.HasFlags(Troops.Arrow.Flags.AboutToShoot))
			{
				flag2 = true;
			}
			if (arrow2.HasFlags(Troops.Arrow.Flags.Moving) && arrow2.t > 0f)
			{
				arrow = arrow2;
				flag3 = true;
			}
			arrow2 = ++arrow2;
		}
		if (!flag)
		{
			this.Stop();
			return;
		}
		if (flag2)
		{
			if (salvo->squad != null)
			{
				bool flag4 = false;
				Troops.Troop troop = salvo->squad->FirstTroop;
				while (troop <= salvo->squad->LastTroop)
				{
					if (troop.flags == Troops.Troop.Flags.None || (troop.HasFlags(Troops.Troop.Flags.Shooting | Troops.Troop.Flags.ShootTrigger) && !troop.HasFlags(Troops.Troop.Flags.Killed | Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed)))
					{
						flag4 = true;
						break;
					}
					troop = ++troop;
				}
				if (salvo->squad->HasFlags(Troops.SquadData.Flags.Dead | Troops.SquadData.Flags.Destroyed) || !flag4)
				{
					this.Stop();
					return;
				}
			}
			if (salvo->fortification != null && salvo->fortification->HasFlags(Troops.FortificationData.Flags.Demolished | Troops.FortificationData.Flags.Destroyed))
			{
				this.Stop();
				return;
			}
		}
		Troops.Arrow arrow3 = salvo->FirstArrow;
		if (salvo->HasFlags(Troops.SalvoData.Flags.HasLandedArrow))
		{
			if (this.salvo_colliders.Count == 1)
			{
				this.salvo_colliders[0].PlayLandParticles();
			}
			else
			{
				int num = 0;
				Troops.Arrow arrow4 = salvo->FirstArrow;
				while (arrow4 <= salvo->LastActiveArrow)
				{
					if (arrow4.HasFlags(Troops.Arrow.Flags.Landed) && this.salvo_colliders.Count > num && !this.salvo_colliders[num].has_landed)
					{
						this.salvo_colliders[num].PlayLandParticles();
					}
					arrow4 = ++arrow4;
					num++;
				}
			}
		}
		base.transform.position = arrow.pos;
		this.attributes.position = base.transform.position.ToFMODVector();
		if (flag3)
		{
			if (this.salvo_colliders.Count == 0)
			{
				this.CreateParticles();
			}
			if (!this.has_played_shoot_sound)
			{
				this.PlayShootSound();
				this.has_played_shoot_sound = true;
			}
		}
		this.shoot_sound.set3DAttributes(this.attributes);
		if (this.salvo_colliders != null && this.salvo_colliders.Count >= salvo->num_arrows)
		{
			int num2 = 0;
			arrow3 = salvo->FirstArrow;
			while (arrow3 <= salvo->LastActiveArrow)
			{
				SalvoEmitter.SalvoCollider salvoCollider = this.salvo_colliders[num2];
				salvoCollider.gameObject.SetActive(arrow3.HasFlags(Troops.Arrow.Flags.Moving | Troops.Arrow.Flags.Landed));
				salvoCollider.transform.position = arrow3.pos;
				arrow3 = ++arrow3;
				num2++;
			}
		}
		if ((arrow3.flags != Troops.Arrow.Flags.None && arrow3.t >= arrow3.duration && arrow3.duration > 0f) || salvo->HasFlags(Troops.SalvoData.Flags.Landed))
		{
			this.Stop();
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00061D40 File Offset: 0x0005FF40
	private void Stop()
	{
		this.shoot_sound.release();
		this.shoot_sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		if (this.salvo_colliders != null)
		{
			for (int i = 0; i < this.salvo_colliders.Count; i++)
			{
				SalvoEmitter.SalvoCollider salvoCollider = this.salvo_colliders[i];
				ParticleSystem main_particles = salvoCollider.main_particles;
				if (main_particles != null)
				{
					ParticleSystemRenderer component = main_particles.GetComponent<ParticleSystemRenderer>();
					if (component != null)
					{
						component.enabled = false;
					}
					main_particles.transform.SetParent(null);
					UnityEngine.Object.Destroy(main_particles.gameObject, main_particles.main.duration);
				}
				if (salvoCollider.all_particles != null)
				{
					for (int j = 0; j < salvoCollider.all_particles.Length; j++)
					{
						salvoCollider.all_particles[j].emission.rateOverTime = 0f;
					}
				}
				global::Common.DestroyObj(salvoCollider.gameObject);
			}
			this.salvo_colliders = null;
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x04000713 RID: 1811
	private int cur_salvo = -1;

	// Token: 0x04000714 RID: 1812
	private EventInstance shoot_sound;

	// Token: 0x04000715 RID: 1813
	private ATTRIBUTES_3D attributes;

	// Token: 0x04000716 RID: 1814
	private MapObject obj;

	// Token: 0x04000717 RID: 1815
	private SalvoData.Def def;

	// Token: 0x04000718 RID: 1816
	private List<SalvoEmitter.SalvoCollider> salvo_colliders;

	// Token: 0x04000719 RID: 1817
	private Rigidbody rig;

	// Token: 0x0400071A RID: 1818
	private GameObject projectile_land_particles;

	// Token: 0x0400071B RID: 1819
	private bool has_played_shoot_sound;

	// Token: 0x0400071C RID: 1820
	private static int _physics_layer = -1;

	// Token: 0x020005AB RID: 1451
	internal class SalvoCollider : MonoBehaviour
	{
		// Token: 0x06004482 RID: 17538 RVA: 0x002021E8 File Offset: 0x002003E8
		private unsafe void OnCollisionEnter(Collision collision)
		{
			if (this.has_collided || this.has_landed)
			{
				return;
			}
			if (!Troops.pdata->GetSalvo(this.parent.cur_salvo)->def->can_hit_fortification)
			{
				return;
			}
			this.has_collided = true;
			this.PlayCollideParticles();
		}

		// Token: 0x06004483 RID: 17539 RVA: 0x00202238 File Offset: 0x00200438
		private unsafe void OnTriggerEnter(Collider other)
		{
			if (this.has_collided || this.has_landed)
			{
				return;
			}
			if (!Troops.pdata->GetSalvo(this.parent.cur_salvo)->def->can_hit_fortification)
			{
				return;
			}
			this.has_collided = true;
			this.PlayCollideParticles();
		}

		// Token: 0x06004484 RID: 17540 RVA: 0x00202288 File Offset: 0x00200488
		private unsafe void PlayCollideParticles()
		{
			GameObject projectile_land_particles = this.parent.projectile_land_particles;
			List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>();
			Troops.SalvoData* salvo = Troops.pdata->GetSalvo(this.parent.cur_salvo);
			list.Add(new KeyValuePair<string, float>("NumArrows", (float)salvo->num_arrows));
			BaseUI.PlaySoundEvent(this.parent.def.arrow_impact_sound_effect, base.transform.position, list);
			if (projectile_land_particles != null)
			{
				GameObject gameObject = global::Common.Spawn(projectile_land_particles, false, false);
				gameObject.transform.position = base.transform.position;
				gameObject.transform.rotation = Quaternion.identity;
			}
		}

		// Token: 0x06004485 RID: 17541 RVA: 0x0020232B File Offset: 0x0020052B
		public void PlayLandParticles()
		{
			if (this.has_landed)
			{
				return;
			}
			this.has_landed = true;
			this.PlayCollideParticles();
		}

		// Token: 0x04003134 RID: 12596
		public SalvoEmitter parent;

		// Token: 0x04003135 RID: 12597
		public ParticleSystem main_particles;

		// Token: 0x04003136 RID: 12598
		public ParticleSystem[] all_particles;

		// Token: 0x04003137 RID: 12599
		public int arrow_id_local;

		// Token: 0x04003138 RID: 12600
		public bool has_landed;

		// Token: 0x04003139 RID: 12601
		public bool has_collided;
	}
}
