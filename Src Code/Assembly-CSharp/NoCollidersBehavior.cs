using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class NoCollidersBehavior : StateMachineBehaviour
{
	// Token: 0x060001F7 RID: 503 RVA: 0x0001F72C File Offset: 0x0001D92C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.cap = animator.GetComponentsInChildren<Collider>();
		if (this.enter)
		{
			Collider[] array = this.cap;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0001F76C File Offset: 0x0001D96C
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.exit)
		{
			Collider[] array = this.cap;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
	}

	// Token: 0x0400030F RID: 783
	[Header("Deactivate Colliders on Enter")]
	public bool enter = true;

	// Token: 0x04000310 RID: 784
	[Header("Activate Colliders on Exit")]
	public bool exit = true;

	// Token: 0x04000311 RID: 785
	private Collider[] cap;
}
