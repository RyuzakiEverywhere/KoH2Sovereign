using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D9 RID: 985
	public class LoopBehaviour : StateMachineBehaviour
	{
		// Token: 0x0600376B RID: 14187 RVA: 0x001B716C File Offset: 0x001B536C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			if (this.animal == null)
			{
				return;
			}
			if (!this.hasEntered)
			{
				this.hasEntered = true;
				this.CurrentLoop = 1;
				this.animal.SetIntID(0);
			}
			else
			{
				this.CurrentLoop++;
			}
			if (this.CurrentLoop >= this.animal.Loops)
			{
				this.hasEntered = false;
				this.animal.SetIntID(this.IntIDExitValue);
			}
		}

		// Token: 0x04002769 RID: 10089
		[Header("This behaviour requires a transition to itself")]
		[Header("With the contidion 'IntID' != -1")]
		public int IntIDExitValue = -1;

		// Token: 0x0400276A RID: 10090
		[Header("")]
		protected int CurrentLoop;

		// Token: 0x0400276B RID: 10091
		protected int loop;

		// Token: 0x0400276C RID: 10092
		private bool hasEntered;

		// Token: 0x0400276D RID: 10093
		private Animal animal;
	}
}
