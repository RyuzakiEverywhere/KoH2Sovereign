using System;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class SMBPlayAlterantiveIdle : StateMachineBehaviour
{
	// Token: 0x06003156 RID: 12630 RVA: 0x0018ED44 File Offset: 0x0018CF44
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Random.value < this.useAltAnimChance)
		{
			animator.SetInteger("RandomStateIndex", Random.Range(0, this.statesCount));
			return;
		}
		animator.SetInteger("RandomStateIndex", -1);
	}

	// Token: 0x040020F6 RID: 8438
	public int statesCount;

	// Token: 0x040020F7 RID: 8439
	public float useAltAnimChance = 0.2f;
}
