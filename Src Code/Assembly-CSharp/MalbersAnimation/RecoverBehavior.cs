using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003DE RID: 990
	public class RecoverBehavior : StateMachineBehaviour
	{
		// Token: 0x06003777 RID: 14199 RVA: 0x001B75C4 File Offset: 0x001B57C4
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.rb = animator.GetComponent<Rigidbody>();
			animator.applyRootMotion = false;
			if (this.RigidY)
			{
				this.rb.constraints = Animal.Still_Constraints;
			}
			if (this.Landing)
			{
				this.animal.IsInAir = false;
				return;
			}
			this.rb.useGravity = false;
		}

		// Token: 0x06003778 RID: 14200 RVA: 0x001B7629 File Offset: 0x001B5829
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rb.drag = Mathf.Lerp(this.rb.drag, this.MaxDrag, Time.deltaTime * this.smoothness);
		}

		// Token: 0x06003779 RID: 14201 RVA: 0x001B7658 File Offset: 0x001B5858
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rb.drag = 0f;
		}

		// Token: 0x0400277F RID: 10111
		public float smoothness = 10f;

		// Token: 0x04002780 RID: 10112
		public float MaxDrag = 3f;

		// Token: 0x04002781 RID: 10113
		public bool stillContraints = true;

		// Token: 0x04002782 RID: 10114
		public bool Landing = true;

		// Token: 0x04002783 RID: 10115
		public bool RigidY = true;

		// Token: 0x04002784 RID: 10116
		private Animal animal;

		// Token: 0x04002785 RID: 10117
		private Rigidbody rb;

		// Token: 0x04002786 RID: 10118
		private float deltatime;
	}
}
