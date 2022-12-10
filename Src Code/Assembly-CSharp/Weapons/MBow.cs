using System;
using System.Collections;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x0200041C RID: 1052
	[AddComponentMenu("Malbers/Weapons/MBow")]
	public class MBow : MWeapon, IBow, IMWeapon
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x060038D4 RID: 14548 RVA: 0x001BC724 File Offset: 0x001BA924
		public Transform KNot
		{
			get
			{
				this.knot.rotation = Quaternion.LookRotation((this.arrowPoint.position - this.knot.position).normalized, -Physics.gravity);
				return this.knot;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x060038D5 RID: 14549 RVA: 0x001BC774 File Offset: 0x001BA974
		public Transform ArrowPoint
		{
			get
			{
				return this.arrowPoint;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x060038D6 RID: 14550 RVA: 0x001BC77C File Offset: 0x001BA97C
		public float HoldTime
		{
			get
			{
				return this.holdTime;
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x060038D7 RID: 14551 RVA: 0x001BC784 File Offset: 0x001BA984
		// (set) Token: 0x060038D8 RID: 14552 RVA: 0x001BC78C File Offset: 0x001BA98C
		public GameObject Arrow
		{
			get
			{
				return this.arrow;
			}
			set
			{
				this.arrow = value;
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x060038D9 RID: 14553 RVA: 0x001BC795 File Offset: 0x001BA995
		// (set) Token: 0x060038DA RID: 14554 RVA: 0x001BC79D File Offset: 0x001BA99D
		public GameObject ArrowInstance
		{
			get
			{
				return this.arrowInstance;
			}
			set
			{
				this.arrowInstance = value;
			}
		}

		// Token: 0x060038DB RID: 14555 RVA: 0x001BC7A6 File Offset: 0x001BA9A6
		private void Start()
		{
			this.InitializeWeapon();
		}

		// Token: 0x060038DC RID: 14556 RVA: 0x001BC7AE File Offset: 0x001BA9AE
		public override void InitializeWeapon()
		{
			base.InitializeWeapon();
			this.InitializeBow();
		}

		// Token: 0x060038DD RID: 14557 RVA: 0x001BC7BC File Offset: 0x001BA9BC
		public virtual void InitializeBow()
		{
			if (this.UpperBn == null || this.LowerBn == null)
			{
				this.BowIsSet = false;
				return;
			}
			if (this.UpperBn.Length == 0 || this.LowerBn.Length == 0)
			{
				this.BowIsSet = false;
				return;
			}
			this.BowTension = 0f;
			this.UpperBnInitRotation = new Quaternion[this.UpperBn.Length];
			this.LowerBnInitRotation = new Quaternion[this.LowerBn.Length];
			if (this.knot)
			{
				this.InitPosKnot = this.knot.localPosition;
			}
			for (int i = 0; i < this.UpperBn.Length; i++)
			{
				if (this.UpperBn[i] == null)
				{
					this.BowIsSet = false;
					return;
				}
				this.UpperBnInitRotation[i] = this.UpperBn[i].localRotation;
			}
			for (int j = 0; j < this.LowerBn.Length; j++)
			{
				if (this.LowerBn[j] == null)
				{
					this.BowIsSet = false;
					return;
				}
				this.LowerBnInitRotation[j] = this.LowerBn[j].localRotation;
			}
			this.BowIsSet = true;
		}

		// Token: 0x060038DE RID: 14558 RVA: 0x001BC8DC File Offset: 0x001BAADC
		public virtual void EquipArrow()
		{
			if (this.ArrowInstance != null)
			{
				Object.Destroy(this.ArrowInstance.gameObject);
			}
			this.ArrowInstance = Object.Instantiate<GameObject>(this.Arrow, this.KNot);
			this.ArrowInstance.transform.localPosition = Vector3.zero;
			this.ArrowInstance.transform.localRotation = Quaternion.identity;
			IArrow component = this.ArrowInstance.GetComponent<IArrow>();
			if (component != null)
			{
				this.ArrowInstance.transform.Translate(0f, 0f, component.TailOffset, Space.Self);
			}
			this.OnLoadArrow.Invoke(this.ArrowInstance);
		}

		// Token: 0x060038DF RID: 14559 RVA: 0x001BC989 File Offset: 0x001BAB89
		public virtual void DestroyArrow()
		{
			if (this.ArrowInstance != null)
			{
				Object.Destroy(this.ArrowInstance.gameObject);
			}
			this.ArrowInstance = null;
		}

		// Token: 0x060038E0 RID: 14560 RVA: 0x001BC9B0 File Offset: 0x001BABB0
		public virtual void BendBow(float normalizedTime)
		{
			if (!this.BowIsSet)
			{
				return;
			}
			this.BowTension = Mathf.Clamp01(normalizedTime);
			this.OnHold.Invoke(this.BowTension);
			for (int i = 0; i < this.UpperBn.Length; i++)
			{
				if (this.UpperBn[i] != null)
				{
					this.UpperBn[i].localRotation = Quaternion.Lerp(this.UpperBnInitRotation[i], Quaternion.Euler(this.RotUpperDir * this.MaxTension) * this.UpperBnInitRotation[i], this.BowTension);
				}
			}
			for (int j = 0; j < this.LowerBn.Length; j++)
			{
				if (this.LowerBn[j] != null)
				{
					this.LowerBn[j].localRotation = Quaternion.Lerp(this.LowerBnInitRotation[j], Quaternion.Euler(this.RotLowerDir * this.MaxTension) * this.LowerBnInitRotation[j], this.BowTension);
				}
			}
			if (this.knot && this.arrowPoint)
			{
				Debug.DrawRay(this.KNot.position, this.KNot.forward, Color.red);
			}
		}

		// Token: 0x060038E1 RID: 14561 RVA: 0x001BCAFC File Offset: 0x001BACFC
		public virtual void ReleaseArrow(Vector3 direction)
		{
			if (!this.releaseArrow)
			{
				this.DestroyArrow();
				return;
			}
			if (this.ArrowInstance == null)
			{
				return;
			}
			this.ArrowInstance.transform.parent = null;
			IArrow component = this.ArrowInstance.GetComponent<IArrow>();
			component.HitMask = base.HitMask;
			component.ShootArrow(Mathf.Lerp(base.MinForce, base.MaxForce, this.BowTension), direction);
			component.Damage = Mathf.Lerp(base.MinDamage, base.MaxDamage, this.BowTension);
			this.OnReleaseArrow.Invoke(this.ArrowInstance);
			this.ArrowInstance = null;
		}

		// Token: 0x060038E2 RID: 14562 RVA: 0x001BCBA1 File Offset: 0x001BADA1
		public virtual void RestoreKnot()
		{
			this.KNot.localPosition = this.InitPosKnot;
			this.DestroyArrow();
		}

		// Token: 0x060038E3 RID: 14563 RVA: 0x001BCBBC File Offset: 0x001BADBC
		public override void PlaySound(int ID)
		{
			if (ID > this.Sounds.Length - 1)
			{
				return;
			}
			if (this.Sounds[ID] != null && this.WeaponSound != null)
			{
				if (this.WeaponSound.isPlaying)
				{
					this.WeaponSound.Stop();
				}
				if (ID == 2)
				{
					this.WeaponSound.pitch = 1.03f / this.HoldTime;
					base.StartCoroutine(this.BowHoldTimePlay(ID));
					return;
				}
				this.WeaponSound.pitch = 1f;
				this.WeaponSound.PlayOneShot(this.Sounds[ID]);
			}
		}

		// Token: 0x060038E4 RID: 14564 RVA: 0x001BCC5A File Offset: 0x001BAE5A
		private IEnumerator BowHoldTimePlay(int ID)
		{
			while (this.BowTension == 0f)
			{
				yield return null;
			}
			this.WeaponSound.PlayOneShot(this.Sounds[ID]);
			yield break;
		}

		// Token: 0x040028FC RID: 10492
		public Transform knot;

		// Token: 0x040028FD RID: 10493
		public Transform arrowPoint;

		// Token: 0x040028FE RID: 10494
		public Transform[] UpperBn;

		// Token: 0x040028FF RID: 10495
		public Transform[] LowerBn;

		// Token: 0x04002900 RID: 10496
		public GameObject arrow;

		// Token: 0x04002901 RID: 10497
		protected GameObject arrowInstance;

		// Token: 0x04002902 RID: 10498
		public float MaxTension;

		// Token: 0x04002903 RID: 10499
		public float holdTime = 2f;

		// Token: 0x04002904 RID: 10500
		[Range(0f, 1f)]
		public float BowTension;

		// Token: 0x04002905 RID: 10501
		private Quaternion[] UpperBnInitRotation;

		// Token: 0x04002906 RID: 10502
		private Quaternion[] LowerBnInitRotation;

		// Token: 0x04002907 RID: 10503
		private Vector3 InitPosKnot;

		// Token: 0x04002908 RID: 10504
		public Vector3 RotUpperDir = -Vector3.forward;

		// Token: 0x04002909 RID: 10505
		public Vector3 RotLowerDir = Vector3.forward;

		// Token: 0x0400290A RID: 10506
		public GameObjectEvent OnLoadArrow;

		// Token: 0x0400290B RID: 10507
		public FloatEvent OnHold;

		// Token: 0x0400290C RID: 10508
		public GameObjectEvent OnReleaseArrow;

		// Token: 0x0400290D RID: 10509
		[Tooltip(" Does not shoot arrows when is false, useful for other controllers like Invector and ootii to let them shoot the arrow instead")]
		public bool releaseArrow = true;

		// Token: 0x0400290E RID: 10510
		public bool BowIsSet;

		// Token: 0x0400290F RID: 10511
		[HideInInspector]
		public bool BonesFoldout;

		// Token: 0x04002910 RID: 10512
		[HideInInspector]
		public bool proceduralfoldout;

		// Token: 0x04002911 RID: 10513
		[HideInInspector]
		public int LowerIndex;

		// Token: 0x04002912 RID: 10514
		[HideInInspector]
		public int UpperIndex;
	}
}
