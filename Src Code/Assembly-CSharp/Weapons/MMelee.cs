using System;
using System.Collections.Generic;
using MalbersAnimations.Events;
using MalbersAnimations.HAP;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Weapons
{
	// Token: 0x0200041E RID: 1054
	public class MMelee : MWeapon, IMelee, IMWeapon
	{
		// Token: 0x17000387 RID: 903
		// (get) Token: 0x060038F5 RID: 14581 RVA: 0x001BCFFB File Offset: 0x001BB1FB
		// (set) Token: 0x060038F6 RID: 14582 RVA: 0x001BD003 File Offset: 0x001BB203
		public bool CanCauseDamage
		{
			get
			{
				return this.canCauseDamage;
			}
			set
			{
				if (!base.IsEquiped)
				{
					return;
				}
				this.canCauseDamage = value;
				this.AlreadyHitted = new List<Transform>();
				if (!this.meleeCollider.isTrigger)
				{
					this.meleeCollider.enabled = this.canCauseDamage;
				}
			}
		}

		// Token: 0x060038F7 RID: 14583 RVA: 0x001BD03E File Offset: 0x001BB23E
		public virtual void CanDoDamage(bool value)
		{
			this.CanCauseDamage = value;
			this.OnCauseDamage.Invoke(value);
			this.meleeCollider.enabled = value;
		}

		// Token: 0x060038F8 RID: 14584 RVA: 0x001BD05F File Offset: 0x001BB25F
		private void Start()
		{
			base.Invoke("InitializeWeapon", 0.01f);
		}

		// Token: 0x060038F9 RID: 14585 RVA: 0x001BD074 File Offset: 0x001BB274
		public override void InitializeWeapon()
		{
			base.InitializeWeapon();
			if (this.meleeCollider)
			{
				this.meleeColliderProxy = this.meleeCollider.gameObject.AddComponent<TriggerProxy>();
				if (this.meleeCollider.isTrigger)
				{
					this.meleeColliderProxy.OnTrigger_Stay.AddListener(new UnityAction<Collider>(this.WeaponTriggerStay));
				}
				else
				{
					this.meleeColliderProxy.OnCollision_Enter.AddListener(new UnityAction<Collision>(this.WeaponCollisionEnter));
				}
				this.meleeCollider.enabled = false;
			}
		}

		// Token: 0x060038FA RID: 14586 RVA: 0x001BD0FF File Offset: 0x001BB2FF
		protected virtual void WeaponCollisionEnter(Collision other)
		{
			if (!base.IsEquiped)
			{
				return;
			}
			if (other.contacts.Length == 0)
			{
				return;
			}
			this.SetDamageStuff(other.contacts[0].point, other.transform);
		}

		// Token: 0x060038FB RID: 14587 RVA: 0x001BD134 File Offset: 0x001BB334
		protected virtual void WeaponTriggerStay(Collider other)
		{
			if (!base.IsEquiped)
			{
				return;
			}
			this.SetDamageStuff(other.ClosestPointOnBounds(this.meleeCollider.bounds.center), other.transform);
		}

		// Token: 0x060038FC RID: 14588 RVA: 0x001BD170 File Offset: 0x001BB370
		internal void SetDamageStuff(Vector3 OtherHitPoint, Transform other)
		{
			if (other.root == base.transform.root)
			{
				return;
			}
			if (other.GetComponentInParent<Mountable>() == base.Owner.Montura)
			{
				return;
			}
			if (!MalbersTools.Layer_in_LayerMask(other.gameObject.layer, base.HitMask))
			{
				return;
			}
			this.DV = new DamageValues(this.meleeCollider.bounds.center - OtherHitPoint, Random.Range(base.MinDamage, base.MaxDamage));
			Debug.DrawLine(OtherHitPoint, this.meleeCollider.bounds.center, Color.red, 3f);
			if (this.canCauseDamage && !this.AlreadyHitted.Find((Transform item) => item == other.transform.root))
			{
				this.AlreadyHitted.Add(other.transform.root);
				other.transform.root.SendMessage("getDamaged", this.DV, SendMessageOptions.DontRequireReceiver);
				Rigidbody component = other.transform.root.GetComponent<Rigidbody>();
				if (component && other.gameObject.layer != 20)
				{
					component.AddExplosionForce(base.MinForce * 50f, OtherHitPoint, 20f);
				}
				this.PlaySound(3);
				this.OnHit.Invoke(other.gameObject);
				if (!this.meleeCollider.isTrigger)
				{
					this.meleeCollider.enabled = false;
				}
			}
		}

		// Token: 0x060038FD RID: 14589 RVA: 0x001BD324 File Offset: 0x001BB524
		private void OnDisable()
		{
			if (this.meleeColliderProxy)
			{
				if (this.meleeCollider.isTrigger)
				{
					this.meleeColliderProxy.OnTrigger_Stay.RemoveListener(new UnityAction<Collider>(this.WeaponTriggerStay));
					return;
				}
				this.meleeColliderProxy.OnCollision_Enter.RemoveListener(new UnityAction<Collision>(this.WeaponCollisionEnter));
			}
		}

		// Token: 0x0400291E RID: 10526
		protected bool isOnAtackingState;

		// Token: 0x0400291F RID: 10527
		protected bool canCauseDamage;

		// Token: 0x04002920 RID: 10528
		public Collider meleeCollider;

		// Token: 0x04002921 RID: 10529
		protected List<Transform> AlreadyHitted = new List<Transform>();

		// Token: 0x04002922 RID: 10530
		public GameObjectEvent OnHit;

		// Token: 0x04002923 RID: 10531
		public BoolEvent OnCauseDamage;

		// Token: 0x04002924 RID: 10532
		protected TriggerProxy meleeColliderProxy;
	}
}
