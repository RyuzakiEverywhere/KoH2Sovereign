using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D1 RID: 977
	public class FlyBehavior : StateMachineBehaviour
	{
		// Token: 0x0600374E RID: 14158 RVA: 0x001B590C File Offset: 0x001B3B0C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rb = animator.GetComponent<Rigidbody>();
			this.animal = animator.GetComponent<Animal>();
			this.acceleration = 0f;
			this.animal.IsInAir = true;
			animator.applyRootMotion = true;
			this.FallVector = ((this.animal.CurrentAnimState == AnimTag.Fall) ? this.rb.velocity : Vector3.zero);
			this.rb.constraints = RigidbodyConstraints.FreezeRotation;
			this.rb.velocity = new Vector3(this.rb.velocity.x, 0f, this.rb.velocity.z);
			this.rb.useGravity = false;
			this.rb.drag = this.Drag;
		}

		// Token: 0x0600374F RID: 14159 RVA: 0x001B59D8 File Offset: 0x001B3BD8
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.deltaTime = Time.deltaTime;
			if (this.FallVector != Vector3.zero)
			{
				this.animal.DeltaPosition += this.FallVector * this.deltaTime;
				this.FallVector = Vector3.Lerp(this.FallVector, Vector3.zero, this.deltaTime * this.FallRecovery);
			}
			if ((double)this.animal.MovementAxis.y < -0.1)
			{
				this.acceleration = Mathf.Lerp(this.acceleration, this.acceleration + this.DownAcceleration, this.deltaTime);
			}
			else if ((double)this.animal.MovementAxis.y > -0.1 || this.animal.MovementReleased)
			{
				float num = this.acceleration - this.DownInertia;
				if (num < 0f)
				{
					num = 0f;
				}
				this.acceleration = Mathf.Lerp(this.acceleration, num, this.deltaTime * 2f);
			}
			this.animal.DeltaPosition += animator.velocity * (this.acceleration / 2f) * this.deltaTime;
			if (this.animal.LockUp)
			{
				this.animal.DeltaPosition += Physics.gravity * this.LockUpDownForce * this.deltaTime * this.deltaTime;
			}
		}

		// Token: 0x0400271D RID: 10013
		public float Drag = 5f;

		// Token: 0x0400271E RID: 10014
		public float DownAcceleration = 4f;

		// Token: 0x0400271F RID: 10015
		[Tooltip("If is changing from ")]
		public float DownInertia = 2f;

		// Token: 0x04002720 RID: 10016
		[Tooltip("If is changing from fall to fly this will smoothly ")]
		public float FallRecovery = 1.5f;

		// Token: 0x04002721 RID: 10017
		[Tooltip("If Lock up is Enabled this apply to the dragon an extra Down Force")]
		public float LockUpDownForce = 4f;

		// Token: 0x04002722 RID: 10018
		private float acceleration;

		// Token: 0x04002723 RID: 10019
		private Rigidbody rb;

		// Token: 0x04002724 RID: 10020
		private Animal animal;

		// Token: 0x04002725 RID: 10021
		private float deltaTime;

		// Token: 0x04002726 RID: 10022
		private Vector3 FallVector;
	}
}
