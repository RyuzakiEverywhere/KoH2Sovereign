using System;
using MalbersAnimations.HAP;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000421 RID: 1057
	public abstract class MWeapon : MonoBehaviour, IMWeapon
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06003901 RID: 14593 RVA: 0x001BD3A9 File Offset: 0x001BB5A9
		public int WeaponID
		{
			get
			{
				return this.weaponID;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06003902 RID: 14594 RVA: 0x001BD3B1 File Offset: 0x001BB5B1
		// (set) Token: 0x06003903 RID: 14595 RVA: 0x001BD3B9 File Offset: 0x001BB5B9
		public WeaponHolder Holder
		{
			get
			{
				return this.holder;
			}
			set
			{
				this.holder = value;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06003904 RID: 14596 RVA: 0x001BD3C2 File Offset: 0x001BB5C2
		// (set) Token: 0x06003905 RID: 14597 RVA: 0x001BD3CC File Offset: 0x001BB5CC
		public bool IsEquiped
		{
			get
			{
				return this.isEquiped;
			}
			set
			{
				this.isEquiped = value;
				if (this.isEquiped)
				{
					this.OnEquiped.Invoke(this);
					return;
				}
				this.Owner = null;
				this.HitMask = default(LayerMask);
				this.OnUnequiped.Invoke(this);
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06003907 RID: 14599 RVA: 0x001BD420 File Offset: 0x001BB620
		// (set) Token: 0x06003906 RID: 14598 RVA: 0x001BD417 File Offset: 0x001BB617
		public float MinDamage
		{
			get
			{
				return this.minDamage;
			}
			set
			{
				this.minDamage = value;
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06003909 RID: 14601 RVA: 0x001BD431 File Offset: 0x001BB631
		// (set) Token: 0x06003908 RID: 14600 RVA: 0x001BD428 File Offset: 0x001BB628
		public float MaxDamage
		{
			get
			{
				return this.maxDamage;
			}
			set
			{
				this.maxDamage = value;
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x0600390B RID: 14603 RVA: 0x001BD442 File Offset: 0x001BB642
		// (set) Token: 0x0600390A RID: 14602 RVA: 0x001BD439 File Offset: 0x001BB639
		public bool RightHand
		{
			get
			{
				return this.rightHand;
			}
			set
			{
				this.rightHand = value;
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600390C RID: 14604 RVA: 0x001BD44A File Offset: 0x001BB64A
		public Vector3 PositionOffset
		{
			get
			{
				return this.positionOffset;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600390D RID: 14605 RVA: 0x001BD452 File Offset: 0x001BB652
		public Vector3 RotationOffset
		{
			get
			{
				return this.rotationOffset;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600390E RID: 14606 RVA: 0x001BD45A File Offset: 0x001BB65A
		// (set) Token: 0x0600390F RID: 14607 RVA: 0x001BD462 File Offset: 0x001BB662
		public float MinForce
		{
			get
			{
				return this.minForce;
			}
			set
			{
				this.minForce = value;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06003910 RID: 14608 RVA: 0x001BD46B File Offset: 0x001BB66B
		// (set) Token: 0x06003911 RID: 14609 RVA: 0x001BD473 File Offset: 0x001BB673
		public float MaxForce
		{
			get
			{
				return this.maxForce;
			}
			set
			{
				this.maxForce = value;
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06003912 RID: 14610 RVA: 0x001BD47C File Offset: 0x001BB67C
		// (set) Token: 0x06003913 RID: 14611 RVA: 0x001BD484 File Offset: 0x001BB684
		public Rider Owner { get; set; }

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06003914 RID: 14612 RVA: 0x001BD48D File Offset: 0x001BB68D
		// (set) Token: 0x06003915 RID: 14613 RVA: 0x001BD495 File Offset: 0x001BB695
		public LayerMask HitMask { get; set; }

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06003916 RID: 14614 RVA: 0x001BD49E File Offset: 0x001BB69E
		// (set) Token: 0x06003917 RID: 14615 RVA: 0x001BD4A6 File Offset: 0x001BB6A6
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		// Token: 0x06003918 RID: 14616 RVA: 0x001BD4AF File Offset: 0x001BB6AF
		public override bool Equals(object a)
		{
			return a is IMWeapon && this.weaponID == (a as IMWeapon).WeaponID;
		}

		// Token: 0x06003919 RID: 14617 RVA: 0x001BD4CF File Offset: 0x001BB6CF
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600391A RID: 14618 RVA: 0x001BD4D7 File Offset: 0x001BB6D7
		public virtual void Equiped()
		{
			Debug.Log(base.name + this.Owner);
			this.OnEquiped.Invoke(this);
		}

		// Token: 0x0600391B RID: 14619 RVA: 0x001BD4FB File Offset: 0x001BB6FB
		public virtual void Unequiped()
		{
			this.OnUnequiped.Invoke(this);
			this.Owner = null;
			Debug.Log(base.name + this.Owner);
		}

		// Token: 0x0600391C RID: 14620 RVA: 0x001BD528 File Offset: 0x001BB728
		public virtual void InitializeWeapon()
		{
			this.WeaponSound = base.GetComponent<AudioSource>();
			this.isEquiped = false;
			if (!this.WeaponSound)
			{
				this.WeaponSound = base.gameObject.AddComponent<AudioSource>();
			}
			this.WeaponSound.spatialBlend = 1f;
		}

		// Token: 0x0600391D RID: 14621 RVA: 0x001BD576 File Offset: 0x001BB776
		public virtual void PlaySound(int ID)
		{
			if (ID > this.Sounds.Length - 1)
			{
				return;
			}
			if (this.Sounds[ID] != null && this.WeaponSound)
			{
				this.WeaponSound.PlayOneShot(this.Sounds[ID]);
			}
		}

		// Token: 0x04002925 RID: 10533
		public int weaponID;

		// Token: 0x04002926 RID: 10534
		[SerializeField]
		private bool active = true;

		// Token: 0x04002927 RID: 10535
		[SerializeField]
		private float minDamage = 10f;

		// Token: 0x04002928 RID: 10536
		[SerializeField]
		private float maxDamage = 20f;

		// Token: 0x04002929 RID: 10537
		[SerializeField]
		private float minForce = 500f;

		// Token: 0x0400292A RID: 10538
		[SerializeField]
		private float maxForce = 1000f;

		// Token: 0x0400292B RID: 10539
		private bool isEquiped;

		// Token: 0x0400292C RID: 10540
		public bool rightHand = true;

		// Token: 0x0400292D RID: 10541
		public WeaponHolder holder;

		// Token: 0x0400292E RID: 10542
		public Vector3 positionOffset;

		// Token: 0x0400292F RID: 10543
		public Vector3 rotationOffset;

		// Token: 0x04002930 RID: 10544
		protected DamageValues DV;

		// Token: 0x04002931 RID: 10545
		public AudioClip[] Sounds;

		// Token: 0x04002932 RID: 10546
		public AudioSource WeaponSound;

		// Token: 0x04002935 RID: 10549
		public WeaponEvent OnEquiped = new WeaponEvent();

		// Token: 0x04002936 RID: 10550
		public WeaponEvent OnUnequiped = new WeaponEvent();

		// Token: 0x04002937 RID: 10551
		[HideInInspector]
		public bool ShowEventEditor;
	}
}
