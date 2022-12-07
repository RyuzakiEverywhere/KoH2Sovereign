using System;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class SMBBlockAnimationSwitch : StateMachineBehaviour
{
	// Token: 0x06003153 RID: 12627 RVA: 0x0018ECEE File Offset: 0x0018CEEE
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.unit == null)
		{
			this.unit = animator.GetComponent<Unit>();
		}
		if (this.unit == null)
		{
			return;
		}
		this.unit.BlockNewAnim(true);
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x0018ED19 File Offset: 0x0018CF19
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.unit == null)
		{
			this.unit = animator.GetComponent<Unit>();
		}
		if (this.unit == null)
		{
			return;
		}
		this.unit.BlockNewAnim(false);
	}

	// Token: 0x040020F5 RID: 8437
	private Unit unit;
}
