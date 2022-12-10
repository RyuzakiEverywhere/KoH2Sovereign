using System;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Weapons
{
	// Token: 0x0200041D RID: 1053
	public class MGun : MWeapon, IGun, IMWeapon
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x060038E6 RID: 14566 RVA: 0x001BCCA5 File Offset: 0x001BAEA5
		// (set) Token: 0x060038E7 RID: 14567 RVA: 0x001BCCAD File Offset: 0x001BAEAD
		public int TotalAmmo
		{
			get
			{
				return this.Ammo;
			}
			set
			{
				this.Ammo = value;
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x060038E8 RID: 14568 RVA: 0x001BCCB6 File Offset: 0x001BAEB6
		// (set) Token: 0x060038E9 RID: 14569 RVA: 0x001BCCBE File Offset: 0x001BAEBE
		public int AmmoInChamber
		{
			get
			{
				return this.ammoInChamber;
			}
			set
			{
				this.ammoInChamber = value;
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x060038EA RID: 14570 RVA: 0x001BCCC7 File Offset: 0x001BAEC7
		// (set) Token: 0x060038EB RID: 14571 RVA: 0x001BCCCF File Offset: 0x001BAECF
		public bool IsAutomatic
		{
			get
			{
				return this.isAutomatic;
			}
			set
			{
				this.isAutomatic = value;
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x060038EC RID: 14572 RVA: 0x001BCCD8 File Offset: 0x001BAED8
		// (set) Token: 0x060038ED RID: 14573 RVA: 0x001BCCE0 File Offset: 0x001BAEE0
		public bool IsAiming
		{
			get
			{
				return this.isAiming;
			}
			set
			{
				if (this.isAiming != value)
				{
					this.isAiming = value;
					this.OnAiming.Invoke(this.IsAiming);
				}
			}
		}

		// Token: 0x060038EE RID: 14574 RVA: 0x001BCCAD File Offset: 0x001BAEAD
		public void SetTotalAmmo(int value)
		{
			this.Ammo = value;
		}

		// Token: 0x060038EF RID: 14575 RVA: 0x001BC7A6 File Offset: 0x001BA9A6
		private void Awake()
		{
			this.InitializeWeapon();
		}

		// Token: 0x060038F0 RID: 14576 RVA: 0x001BCD03 File Offset: 0x001BAF03
		public virtual void ReduceAmmo(int amount)
		{
			this.ammoInChamber -= amount;
		}

		// Token: 0x060038F1 RID: 14577 RVA: 0x001BCD14 File Offset: 0x001BAF14
		public virtual void FireProyectile(RaycastHit AimRay)
		{
			float t = (float)Random.Range(0, 1);
			Vector3 normalized = (AimRay.point - base.transform.position).normalized;
			this.ReduceAmmo(1);
			this.OnFire.Invoke(normalized);
			DamageValues value = new DamageValues(AimRay.normal, Mathf.Lerp(base.MinDamage, base.MaxDamage, t));
			if (AimRay.transform)
			{
				AimRay.transform.root.SendMessage("getDamaged", value, SendMessageOptions.DontRequireReceiver);
				if (AimRay.rigidbody)
				{
					AimRay.rigidbody.AddForceAtPosition(normalized * Mathf.Lerp(base.MinForce, base.MaxForce, t), AimRay.point);
				}
				this.BulletHole(AimRay);
				this.OnHit.Invoke(AimRay.transform);
			}
		}

		// Token: 0x060038F2 RID: 14578 RVA: 0x001BCDF4 File Offset: 0x001BAFF4
		public virtual bool Reload()
		{
			if (this.Ammo == 0)
			{
				return false;
			}
			int num = this.ClipSize - this.ammoInChamber;
			if (num == 0)
			{
				return false;
			}
			int num2 = this.TotalAmmo - num;
			if (num2 >= 0)
			{
				this.ammoInChamber += num;
				this.TotalAmmo = num2;
			}
			else
			{
				this.ammoInChamber += this.TotalAmmo;
				this.TotalAmmo = 0;
			}
			this.OnReload.Invoke();
			return true;
		}

		// Token: 0x060038F3 RID: 14579 RVA: 0x001BCE68 File Offset: 0x001BB068
		public virtual void BulletHole(RaycastHit hit)
		{
			if (!this.bulletHole)
			{
				return;
			}
			Vector3 lossyScale = hit.transform.lossyScale;
			lossyScale.x = 1f / Mathf.Max(lossyScale.x, 0.0001f);
			lossyScale.y = 1f / Mathf.Max(lossyScale.y, 0.0001f);
			lossyScale.z = 1f / Mathf.Max(lossyScale.z, 0.0001f);
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = hit.collider.transform;
			gameObject.transform.localScale = lossyScale;
			gameObject.transform.localPosition = hit.collider.transform.InverseTransformPoint(hit.point);
			gameObject.transform.localRotation = Quaternion.identity;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.bulletHole);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			gameObject2.transform.localRotation = gameObject2.transform.localRotation * Quaternion.Euler(0f, (float)Random.Range(-90, 90), 0f);
			Object.Destroy(gameObject, this.BulletHoleTime);
			Object.Destroy(gameObject, this.BulletHoleTime);
		}

		// Token: 0x04002913 RID: 10515
		public int Ammo;

		// Token: 0x04002914 RID: 10516
		public int ammoInChamber;

		// Token: 0x04002915 RID: 10517
		public int ClipSize;

		// Token: 0x04002916 RID: 10518
		public bool isAutomatic;

		// Token: 0x04002917 RID: 10519
		public GameObject bulletHole;

		// Token: 0x04002918 RID: 10520
		public float BulletHoleTime = 10f;

		// Token: 0x04002919 RID: 10521
		public Vector3Event OnFire;

		// Token: 0x0400291A RID: 10522
		public UnityEvent OnReload;

		// Token: 0x0400291B RID: 10523
		public TransformEvent OnHit;

		// Token: 0x0400291C RID: 10524
		public BoolEvent OnAiming;

		// Token: 0x0400291D RID: 10525
		protected bool isAiming;
	}
}
