using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D2 RID: 978
	public class FlyDodgeBehaviour : StateMachineBehaviour
	{
		// Token: 0x06003751 RID: 14161 RVA: 0x001B5BB1 File Offset: 0x001B3DB1
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rb = animator.GetComponent<Rigidbody>();
			this.animal = animator.GetComponent<Animal>();
			this.momentum = (this.InPlace ? this.rb.velocity : animator.velocity);
		}

		// Token: 0x06003752 RID: 14162 RVA: 0x001B5BEC File Offset: 0x001B3DEC
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.time = ((animator.updateMode == AnimatorUpdateMode.AnimatePhysics) ? Time.fixedDeltaTime : Time.deltaTime);
			this.animal.DeltaPosition += this.momentum * this.time;
		}

		// Token: 0x04002727 RID: 10023
		public bool InPlace;

		// Token: 0x04002728 RID: 10024
		private Vector3 momentum;

		// Token: 0x04002729 RID: 10025
		private Rigidbody rb;

		// Token: 0x0400272A RID: 10026
		private Animal animal;

		// Token: 0x0400272B RID: 10027
		private float time;
	}
}
