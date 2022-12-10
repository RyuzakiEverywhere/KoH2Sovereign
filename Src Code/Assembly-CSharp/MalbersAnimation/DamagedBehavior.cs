using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003CF RID: 975
	public class DamagedBehavior : StateMachineBehaviour
	{
		// Token: 0x06003745 RID: 14149 RVA: 0x001B5390 File Offset: 0x001B3590
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			Animal component = animator.GetComponent<Animal>();
			component.Damaged = false;
			animator.SetBool(Hash.Damaged, false);
			if (!this.DirectionalDamage)
			{
				return;
			}
			Vector3 hitDirection = component.HitDirection;
			Vector3 forward = animator.transform.forward;
			hitDirection.y = 0f;
			forward.y = 0f;
			float num = Vector3.Angle(forward, hitDirection);
			if (Vector3.Dot(component.T_Right, component.HitDirection) < 0f)
			{
				if (num > 0f && num <= 60f)
				{
					this.Side = 3;
				}
				else if (num > 60f && num <= 120f)
				{
					this.Side = 2;
				}
				else if (num > 120f && num <= 180f)
				{
					this.Side = 1;
				}
			}
			else if (num > 0f && num <= 60f)
			{
				this.Side = -3;
			}
			else if (num > 60f && num <= 120f)
			{
				this.Side = -2;
			}
			else if (num > 120f && num <= 180f)
			{
				this.Side = -1;
			}
			animator.SetInteger(Hash.IDInt, this.Side);
		}

		// Token: 0x0400270E RID: 9998
		private int Side;

		// Token: 0x0400270F RID: 9999
		public bool DirectionalDamage = true;
	}
}
