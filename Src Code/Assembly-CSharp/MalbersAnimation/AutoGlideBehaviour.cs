using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003CD RID: 973
	public class AutoGlideBehaviour : StateMachineBehaviour
	{
		// Token: 0x0600373E RID: 14142 RVA: 0x001B5098 File Offset: 0x001B3298
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.Default_UseShift = this.animal.UseShift;
			this.animal.UseShift = false;
			this.FlyStyleTime = this.GlideChance.RandomValue;
			this.currentTime = Time.time;
		}

		// Token: 0x0600373F RID: 14143 RVA: 0x001B50EC File Offset: 0x001B32EC
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (Time.time - this.FlyStyleTime >= this.currentTime)
			{
				this.currentTime = Time.time;
				this.isGliding = !this.isGliding;
				this.FlyStyleTime = (this.isGliding ? this.GlideChance.RandomValue : this.FlapChange.RandomValue);
				this.animal.GroundSpeed = (this.isGliding ? 2f : Random.Range(1f, 1.5f));
			}
		}

		// Token: 0x06003740 RID: 14144 RVA: 0x001B5176 File Offset: 0x001B3376
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.UseShift = this.Default_UseShift;
		}

		// Token: 0x040026FC RID: 9980
		[MinMaxRange(0f, 10f)]
		public RangedFloat GlideChance = new RangedFloat(0.8f, 4f);

		// Token: 0x040026FD RID: 9981
		[MinMaxRange(0f, 10f)]
		public RangedFloat FlapChange = new RangedFloat(0.5f, 4f);

		// Token: 0x040026FE RID: 9982
		protected bool isGliding;

		// Token: 0x040026FF RID: 9983
		protected float FlyStyleTime = 1f;

		// Token: 0x04002700 RID: 9984
		protected float currentTime = 1f;

		// Token: 0x04002701 RID: 9985
		protected bool Default_UseShift;

		// Token: 0x04002702 RID: 9986
		protected Animal animal;
	}
}
