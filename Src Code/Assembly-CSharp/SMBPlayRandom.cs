using System;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class SMBPlayRandom : StateMachineBehaviour
{
	// Token: 0x06003158 RID: 12632 RVA: 0x0018ED8A File Offset: 0x0018CF8A
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger("RandomStateIndex", Random.Range(0, this.statesCount));
	}

	// Token: 0x040020F8 RID: 8440
	public int statesCount;
}
