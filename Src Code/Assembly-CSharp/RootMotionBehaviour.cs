using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class RootMotionBehaviour : StateMachineBehaviour
{
	// Token: 0x060001FA RID: 506 RVA: 0x0001F7B5 File Offset: 0x0001D9B5
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.OnEnter)
		{
			animator.applyRootMotion = this.RootMotionOnEnter;
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0001F7CB File Offset: 0x0001D9CB
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.OnExit)
		{
			animator.applyRootMotion = this.RootMotionOnExit;
		}
	}

	// Token: 0x04000312 RID: 786
	public bool OnEnter;

	// Token: 0x04000313 RID: 787
	public bool RootMotionOnEnter;

	// Token: 0x04000314 RID: 788
	[Space]
	public bool OnExit;

	// Token: 0x04000315 RID: 789
	public bool RootMotionOnExit;
}
