using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003C4 RID: 964
	public class AttackTrigger : MonoBehaviour
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06003728 RID: 14120 RVA: 0x001B45E3 File Offset: 0x001B27E3
		public Collider Collider
		{
			get
			{
				if (!this._collider)
				{
					this._collider = base.GetComponent<Collider>();
				}
				return this._collider;
			}
		}

		// Token: 0x06003729 RID: 14121 RVA: 0x001B4604 File Offset: 0x001B2804
		public void Start()
		{
			this.myAnimal = base.transform.GetComponentInParent<Animal>();
			if (this.Collider)
			{
				this.Collider.isTrigger = true;
				this.Collider.enabled = false;
			}
			else
			{
				Debug.LogWarning(base.name + " needs a Collider so 'AttackTrigger' can function correctly");
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600372A RID: 14122 RVA: 0x001B466C File Offset: 0x001B286C
		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger)
			{
				return;
			}
			this.enemy = other.GetComponentInParent<IMDamagable>();
			if (this.enemy == null)
			{
				if (other.attachedRigidbody && this.PushForce != 0f)
				{
					other.attachedRigidbody.AddForce((other.transform.position - base.transform.position).normalized * this.PushForce, ForceMode.VelocityChange);
				}
				return;
			}
			if (this.myAnimal.GetComponent<IMDamagable>() == this.enemy)
			{
				return;
			}
			DamageValues dv = new DamageValues(this.myAnimal.transform.position - other.bounds.center, this.damageMultiplier * (this.myAnimal ? this.myAnimal.attackStrength : 1f));
			this.enemy.getDamaged(dv);
		}

		// Token: 0x0600372B RID: 14123 RVA: 0x001B475C File Offset: 0x001B295C
		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = this.DebugColor;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				if (this.Collider && this.Collider.enabled)
				{
					if (this.Collider is BoxCollider)
					{
						BoxCollider boxCollider = this.Collider as BoxCollider;
						if (!boxCollider.enabled)
						{
							return;
						}
						float x = base.transform.lossyScale.x * boxCollider.size.x;
						float y = base.transform.lossyScale.y * boxCollider.size.y;
						float z = base.transform.lossyScale.z * boxCollider.size.z;
						Gizmos.matrix = Matrix4x4.TRS(boxCollider.bounds.center, base.transform.rotation, new Vector3(x, y, z));
						Gizmos.DrawCube(Vector3.zero, Vector3.one);
						Gizmos.color = new Color(this.DebugColor.r, this.DebugColor.g, this.DebugColor.b, 1f);
						Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
						return;
					}
					else if (this.Collider is SphereCollider)
					{
						SphereCollider sphereCollider = this.Collider as SphereCollider;
						if (!sphereCollider.enabled)
						{
							return;
						}
						Gizmos.matrix = base.transform.localToWorldMatrix;
						Gizmos.DrawSphere(Vector3.zero + sphereCollider.center, sphereCollider.radius);
						Gizmos.color = new Color(this.DebugColor.r, this.DebugColor.g, this.DebugColor.b, 1f);
						Gizmos.DrawWireSphere(Vector3.zero + sphereCollider.center, sphereCollider.radius);
					}
				}
			}
		}

		// Token: 0x04002691 RID: 9873
		public int index = 1;

		// Token: 0x04002692 RID: 9874
		public float damageMultiplier = 1f;

		// Token: 0x04002693 RID: 9875
		public float PushForce;

		// Token: 0x04002694 RID: 9876
		private Animal myAnimal;

		// Token: 0x04002695 RID: 9877
		private IMDamagable enemy;

		// Token: 0x04002696 RID: 9878
		private Collider _collider;

		// Token: 0x04002697 RID: 9879
		public bool debug = true;

		// Token: 0x04002698 RID: 9880
		public Color DebugColor = new Color(1f, 0.25f, 0f, 0.15f);
	}
}
