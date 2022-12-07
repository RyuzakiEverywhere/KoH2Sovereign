using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class WorkerRandomWalk : StateMachineBehaviour
{
	// Token: 0x06000115 RID: 277 RVA: 0x0000AE7C File Offset: 0x0000907C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int value = Random.Range(1, 4);
		animator.SetInteger("idx", value);
	}
}
