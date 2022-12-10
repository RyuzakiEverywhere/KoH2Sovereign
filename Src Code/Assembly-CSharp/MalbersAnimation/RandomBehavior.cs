using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003DD RID: 989
	public class RandomBehavior : StateMachineBehaviour
	{
		// Token: 0x06003775 RID: 14197 RVA: 0x001B7560 File Offset: 0x001B5760
		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			int num = Random.Range(1, this.Range + 1);
			animator.SetInteger(this.Parameter, num);
			Animal component = animator.GetComponent<Animal>();
			if (component && this.Parameter == "IDInt")
			{
				component.SetIntID(num);
			}
		}

		// Token: 0x0400277D RID: 10109
		public string Parameter = "IDInt";

		// Token: 0x0400277E RID: 10110
		public int Range;
	}
}
