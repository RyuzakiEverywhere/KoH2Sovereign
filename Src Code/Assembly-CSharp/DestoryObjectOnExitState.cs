using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class DestoryObjectOnExitState : StateMachineBehaviour
{
	// Token: 0x06000191 RID: 401 RVA: 0x0000FC4D File Offset: 0x0000DE4D
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Object.Destroy(animator.gameObject);
	}
}
