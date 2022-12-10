using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E0 RID: 992
	public class SleepBehavior : StateMachineBehaviour
	{
		// Token: 0x0600377F RID: 14207 RVA: 0x001B7810 File Offset: 0x001B5A10
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.animal == null)
			{
				this.animal = animator.GetComponent<Animal>();
			}
			if (!this.animal)
			{
				return;
			}
			if (this.animal.GotoSleep == 0)
			{
				return;
			}
			if (animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == AnimTag.Idle)
			{
				Animal animal = this.animal;
				int tired = animal.Tired;
				animal.Tired = tired + 1;
				if (this.animal.Tired >= this.animal.GotoSleep)
				{
					this.animal.SetIntID(this.transitionID);
					this.animal.Tired = 0;
					return;
				}
			}
			else
			{
				this.CyclesToSleep();
			}
		}

		// Token: 0x06003780 RID: 14208 RVA: 0x001B78BC File Offset: 0x001B5ABC
		private void CyclesToSleep()
		{
			if (this.CyclesFromController)
			{
				this.Cycles = this.animal.GotoSleep;
				if (this.Cycles == 0)
				{
					return;
				}
			}
			this.currentCycle++;
			if (this.currentCycle >= this.Cycles)
			{
				this.animal.SetIntID(this.transitionID);
				this.currentCycle = 0;
			}
		}

		// Token: 0x04002793 RID: 10131
		public bool CyclesFromController;

		// Token: 0x04002794 RID: 10132
		public int Cycles;

		// Token: 0x04002795 RID: 10133
		public int transitionID;

		// Token: 0x04002796 RID: 10134
		private int currentCycle;

		// Token: 0x04002797 RID: 10135
		private Animal animal;
	}
}
