using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D6 RID: 982
	public class JumpNoRootBehaviour : StateMachineBehaviour
	{
		// Token: 0x06003764 RID: 14180 RVA: 0x001B6E4C File Offset: 0x001B504C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.rb = animator.GetComponent<Rigidbody>();
			this.animal.InAir(true);
			this.animal.SetIntID(0);
			this.animal.OnJump.Invoke();
			animator.applyRootMotion = false;
			Vector3 force = Vector3.up * this.JumpMultiplier * this.animal.JumpHeightMultiplier + animator.transform.forward * this.ForwardMultiplier * this.animal.AirForwardMultiplier;
			this.rb.AddForce(force, ForceMode.VelocityChange);
		}

		// Token: 0x06003765 RID: 14181 RVA: 0x001B6EFC File Offset: 0x001B50FC
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.SetIntID(0);
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			if (this.rb && currentAnimatorStateInfo.tagHash == AnimTag.Fly)
			{
				Vector3 velocity = new Vector3(this.rb.velocity.x, 0f, this.rb.velocity.z);
				this.rb.velocity = velocity;
			}
			if (currentAnimatorStateInfo.tagHash != AnimTag.Fall && currentAnimatorStateInfo.tagHash != AnimTag.Fly)
			{
				this.animal.IsInAir = false;
			}
		}

		// Token: 0x0400275E RID: 10078
		public float JumpMultiplier = 1f;

		// Token: 0x0400275F RID: 10079
		public float ForwardMultiplier;

		// Token: 0x04002760 RID: 10080
		private Animal animal;

		// Token: 0x04002761 RID: 10081
		private Rigidbody rb;
	}
}
