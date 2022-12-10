using System;
using System.Collections;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Weapons
{
	// Token: 0x0200041B RID: 1051
	[AddComponentMenu("Malbers/Weapons/MArrow")]
	public class MArrow : MonoBehaviour, IArrow
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060038C3 RID: 14531 RVA: 0x001BC2B4 File Offset: 0x001BA4B4
		private Rigidbody _Rigidbody
		{
			get
			{
				if (this._rigidbody == null)
				{
					this._rigidbody = base.GetComponent<Rigidbody>();
				}
				return this._rigidbody;
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x060038C4 RID: 14532 RVA: 0x001BC2D6 File Offset: 0x001BA4D6
		// (set) Token: 0x060038C5 RID: 14533 RVA: 0x001BC2DE File Offset: 0x001BA4DE
		public LayerMask HitMask
		{
			get
			{
				return this.hitmask;
			}
			set
			{
				this.hitmask = value;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060038C6 RID: 14534 RVA: 0x001BC2E7 File Offset: 0x001BA4E7
		// (set) Token: 0x060038C7 RID: 14535 RVA: 0x001BC2EF File Offset: 0x001BA4EF
		public float TailOffset
		{
			get
			{
				return this.tailOffset;
			}
			set
			{
				this.tailOffset = value;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060038C8 RID: 14536 RVA: 0x001BC2F8 File Offset: 0x001BA4F8
		// (set) Token: 0x060038C9 RID: 14537 RVA: 0x001BC300 File Offset: 0x001BA500
		public float Damage
		{
			get
			{
				return this.damage;
			}
			set
			{
				this.damage = value;
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060038CA RID: 14538 RVA: 0x001BC309 File Offset: 0x001BA509
		public bool isFlying
		{
			get
			{
				return this.isflying;
			}
		}

		// Token: 0x060038CB RID: 14539 RVA: 0x001BC314 File Offset: 0x001BA514
		public virtual void ShootArrow(float force, Vector3 direction)
		{
			this.force = force;
			this.direction = direction;
			if (this._Rigidbody)
			{
				this._Rigidbody.constraints = RigidbodyConstraints.None;
				this._Rigidbody.isKinematic = false;
				if (this.useGravity)
				{
					this._Rigidbody.useGravity = true;
				}
				else
				{
					this._Rigidbody.useGravity = false;
				}
				this._Rigidbody.AddForce(direction * force);
				this.isflying = true;
				base.StartCoroutine(this.FlyingArrow());
			}
			base.StartCoroutine(this.FlyingAge());
			this.OnFireArrow.Invoke();
		}

		// Token: 0x060038CC RID: 14540 RVA: 0x001BC3B4 File Offset: 0x001BA5B4
		private IEnumerator FlyingAge()
		{
			yield return new WaitForSeconds(this.AgeFlying);
			if (this.isflying)
			{
				this.isflying = false;
				Object.Destroy(base.gameObject);
				base.StopAllCoroutines();
			}
			yield break;
		}

		// Token: 0x060038CD RID: 14541 RVA: 0x001BC3C3 File Offset: 0x001BA5C3
		public virtual void TestFlyingArrow()
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.TestFly());
			base.StartCoroutine(this.FlyingAge());
		}

		// Token: 0x060038CE RID: 14542 RVA: 0x001BC3E5 File Offset: 0x001BA5E5
		private IEnumerator TestFly()
		{
			this.isflying = true;
			this.DeltaPos = base.transform.position;
			RaycastHit hit = default(RaycastHit);
			while (this.isflying)
			{
				float num = Mathf.Abs((base.transform.position - this.DeltaPos).magnitude) + 0.3f;
				if (Physics.Raycast(base.transform.position, this._Rigidbody.velocity.normalized, out hit, num, this.hitmask))
				{
					this.isflying = false;
				}
				else
				{
					Debug.DrawRay(base.transform.position, this._Rigidbody.velocity.normalized * num, Color.red);
					this.DeltaPos = base.transform.position;
				}
				yield return null;
			}
			this.TestHit(hit);
			yield break;
		}

		// Token: 0x060038CF RID: 14543 RVA: 0x001BC3F4 File Offset: 0x001BA5F4
		private IEnumerator FlyingArrow()
		{
			float num = this.force / this._rigidbody.mass * Time.fixedDeltaTime * Time.fixedDeltaTime;
			Debug.DrawRay(base.transform.position, this.direction * num, Color.red);
			RaycastHit other;
			if (Physics.Raycast(base.transform.position, this.direction, out other, num, this.hitmask))
			{
				this.OnHit(other);
				this.isflying = false;
			}
			yield return new WaitForEndOfFrame();
			while (this.isflying)
			{
				num = Mathf.Abs((base.transform.position - this.DeltaPos).magnitude) + 0.3f;
				if (Physics.Raycast(base.transform.position, this._Rigidbody.velocity, out other, num, this.hitmask))
				{
					this.OnHit(other);
					this.isflying = false;
				}
				else
				{
					Debug.DrawRay(base.transform.position, this._Rigidbody.velocity.normalized * num, Color.red);
					if (this._Rigidbody.velocity.magnitude > 0f)
					{
						base.transform.rotation = Quaternion.LookRotation(this._Rigidbody.velocity.normalized, base.transform.up);
					}
					yield return this.WfeoF;
					this.DeltaPos = base.transform.position;
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x060038D0 RID: 14544 RVA: 0x001BC404 File Offset: 0x001BA604
		public virtual void TestHit(RaycastHit other)
		{
			DamageValues value = new DamageValues(-base.transform.forward, this.damage);
			if (other.transform)
			{
				other.transform.SendMessageUpwards("getDamaged", value, SendMessageOptions.DontRequireReceiver);
				if (other.rigidbody && this.AffectRigidBodies)
				{
					other.rigidbody.AddForceAtPosition(base.transform.forward * this.force, other.point);
				}
			}
			this.OnHitTarget.Invoke(other);
		}

		// Token: 0x060038D1 RID: 14545 RVA: 0x001BC49C File Offset: 0x001BA69C
		public virtual void OnHit(RaycastHit other)
		{
			DamageValues value = new DamageValues(-base.transform.forward, this.damage);
			if (other.transform)
			{
				other.transform.SendMessageUpwards("getDamaged", value, SendMessageOptions.DontRequireReceiver);
				if (other.rigidbody && this.AffectRigidBodies)
				{
					other.rigidbody.AddForceAtPosition(base.transform.forward * this.force, other.point);
				}
			}
			this._Rigidbody.isKinematic = true;
			this._Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			Vector3 lossyScale = other.transform.lossyScale;
			lossyScale.x = 1f / Mathf.Max(lossyScale.x, 0.0001f);
			lossyScale.y = 1f / Mathf.Max(lossyScale.y, 0.0001f);
			lossyScale.z = 1f / Mathf.Max(lossyScale.z, 0.0001f);
			GameObject gameObject = new GameObject();
			gameObject.name = base.name + "Link";
			gameObject.transform.parent = other.collider.transform;
			gameObject.transform.localScale = lossyScale;
			gameObject.transform.position = other.point;
			gameObject.transform.localRotation = Quaternion.identity;
			base.transform.parent = gameObject.transform;
			base.transform.localScale = Vector3.one;
			base.transform.localPosition = Vector3.zero;
			base.transform.position += base.transform.forward * this.Penetration;
			Object.Destroy(gameObject, this.AgeAfterImpact);
			Object.Destroy(base.gameObject, this.AgeAfterImpact);
			this.OnHitTarget.Invoke(other);
			if (this.timeAfterImpact > 0f)
			{
				base.Invoke("InvokeAfterImpact", this.timeAfterImpact);
				return;
			}
			this.AfterImpact.Invoke();
		}

		// Token: 0x060038D2 RID: 14546 RVA: 0x001BC6B3 File Offset: 0x001BA8B3
		private void InvokeAfterImpact()
		{
			this.AfterImpact.Invoke();
		}

		// Token: 0x040028E9 RID: 10473
		[Space]
		[Tooltip("Damage this Arrow Causes")]
		public float damage = 1f;

		// Token: 0x040028EA RID: 10474
		[Tooltip("Penetration to the hitted mesh")]
		public float Penetration = 0.2f;

		// Token: 0x040028EB RID: 10475
		[Tooltip("How long the arrow is stay alive after hit Something")]
		public float AgeAfterImpact = 10f;

		// Token: 0x040028EC RID: 10476
		[Tooltip("How long the arrow is alive flying (Used for removing arrows shooted off the map)")]
		public float AgeFlying = 30f;

		// Token: 0x040028ED RID: 10477
		[Tooltip("Damage this Arrow Causes")]
		public float tailOffset = 1f;

		// Token: 0x040028EE RID: 10478
		public bool useGravity = true;

		// Token: 0x040028EF RID: 10479
		public bool AffectRigidBodies = true;

		// Token: 0x040028F0 RID: 10480
		[Space]
		[Header("Events")]
		public UnityEvent OnFireArrow;

		// Token: 0x040028F1 RID: 10481
		public RayCastHitEvent OnHitTarget;

		// Token: 0x040028F2 RID: 10482
		public float timeAfterImpact;

		// Token: 0x040028F3 RID: 10483
		public UnityEvent AfterImpact;

		// Token: 0x040028F4 RID: 10484
		protected LayerMask hitmask;

		// Token: 0x040028F5 RID: 10485
		protected float force;

		// Token: 0x040028F6 RID: 10486
		private Vector3 direction;

		// Token: 0x040028F7 RID: 10487
		protected Rigidbody _rigidbody;

		// Token: 0x040028F8 RID: 10488
		protected Vector3 DeltaPos;

		// Token: 0x040028F9 RID: 10489
		protected bool isflying;

		// Token: 0x040028FA RID: 10490
		[HideInInspector]
		public RaycastHit HitPoint;

		// Token: 0x040028FB RID: 10491
		private WaitForEndOfFrame WfeoF = new WaitForEndOfFrame();
	}
}
