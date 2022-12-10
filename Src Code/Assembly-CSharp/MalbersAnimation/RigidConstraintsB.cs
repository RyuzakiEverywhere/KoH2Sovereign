using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003DF RID: 991
	public class RigidConstraintsB : StateMachineBehaviour
	{
		// Token: 0x0600377B RID: 14203 RVA: 0x001B76A0 File Offset: 0x001B58A0
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.Amount = 0;
			this.rb = animator.GetComponent<Rigidbody>();
			if (this.PosX)
			{
				this.Amount += 2;
			}
			if (this.PosY)
			{
				this.Amount += 4;
			}
			if (this.PosZ)
			{
				this.Amount += 8;
			}
			if (this.RotX)
			{
				this.Amount += 16;
			}
			if (this.RotY)
			{
				this.Amount += 32;
			}
			if (this.RotZ)
			{
				this.Amount += 64;
			}
			if (this.OnEnter && this.rb)
			{
				this.rb.constraints = (RigidbodyConstraints)this.Amount;
			}
			this.ExitTime = false;
			this.rb.drag = this.OnEnterDrag;
		}

		// Token: 0x0600377C RID: 14204 RVA: 0x001B7785 File Offset: 0x001B5985
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this.ExitTime && this.OnExit && stateInfo.normalizedTime > 1f)
			{
				this.rb.constraints = (RigidbodyConstraints)this.Amount;
				this.ExitTime = true;
			}
		}

		// Token: 0x0600377D RID: 14205 RVA: 0x001B77BD File Offset: 0x001B59BD
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.OnExit && this.rb)
			{
				this.rb.constraints = (RigidbodyConstraints)this.Amount;
			}
		}

		// Token: 0x04002787 RID: 10119
		public bool PosX;

		// Token: 0x04002788 RID: 10120
		public bool PosY = true;

		// Token: 0x04002789 RID: 10121
		public bool PosZ;

		// Token: 0x0400278A RID: 10122
		public bool RotX = true;

		// Token: 0x0400278B RID: 10123
		public bool RotY = true;

		// Token: 0x0400278C RID: 10124
		public bool RotZ = true;

		// Token: 0x0400278D RID: 10125
		public bool OnEnter = true;

		// Token: 0x0400278E RID: 10126
		public bool OnExit;

		// Token: 0x0400278F RID: 10127
		protected int Amount;

		// Token: 0x04002790 RID: 10128
		private Rigidbody rb;

		// Token: 0x04002791 RID: 10129
		private bool ExitTime;

		// Token: 0x04002792 RID: 10130
		public float OnEnterDrag;
	}
}
