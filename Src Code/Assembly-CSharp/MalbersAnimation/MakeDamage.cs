using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003CB RID: 971
	public class MakeDamage : MonoBehaviour
	{
		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06003736 RID: 14134 RVA: 0x001B4E4C File Offset: 0x001B304C
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

		// Token: 0x06003737 RID: 14135 RVA: 0x001B4E6D File Offset: 0x001B306D
		private void Start()
		{
			if (this.Collider)
			{
				this.Collider.isTrigger = true;
				return;
			}
			Debug.LogWarning(base.name + " needs a Collider so 'AttackTrigger' can function correctly");
		}

		// Token: 0x06003738 RID: 14136 RVA: 0x001B4EA0 File Offset: 0x001B30A0
		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.root == base.transform.root)
			{
				return;
			}
			DamageValues dv = new DamageValues(-other.bounds.center + this.Collider.bounds.center, this.damageMultiplier);
			if (other.isTrigger)
			{
				return;
			}
			IMDamagable componentInParent = other.GetComponentInParent<IMDamagable>();
			if (componentInParent != null)
			{
				componentInParent.getDamaged(dv);
			}
		}

		// Token: 0x040026F2 RID: 9970
		public float damageMultiplier = 1f;

		// Token: 0x040026F3 RID: 9971
		private Collider _collider;
	}
}
