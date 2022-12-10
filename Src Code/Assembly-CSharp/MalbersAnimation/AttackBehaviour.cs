using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003CC RID: 972
	public class AttackBehaviour : StateMachineBehaviour
	{
		// Token: 0x0600373A RID: 14138 RVA: 0x001B4F30 File Offset: 0x001B3130
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.animal.IsAttacking = true;
			this.animal.Attack1 = false;
			this.isOn = (this.isOff = false);
			this.attackDelay = this.animal.attackDelay;
			this.startAttackTime = Time.time;
		}

		// Token: 0x0600373B RID: 14139 RVA: 0x001B4F90 File Offset: 0x001B3190
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.IsAttacking = true;
			if (!this.isOn && stateInfo.normalizedTime % 1f >= this.On)
			{
				this.animal.AttackTrigger(this.AttackTrigger);
				this.isOn = true;
			}
			if (!this.isOff && stateInfo.normalizedTime % 1f >= this.Off)
			{
				this.animal.AttackTrigger(0);
				this.isOff = true;
			}
			if (this.attackDelay > 0f && Time.time - this.startAttackTime >= this.attackDelay)
			{
				this.animal.IsAttacking = false;
			}
		}

		// Token: 0x0600373C RID: 14140 RVA: 0x001B503C File Offset: 0x001B323C
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.AttackTrigger(0);
			this.isOn = (this.isOff = false);
			this.animal.IsAttacking = false;
		}

		// Token: 0x040026F4 RID: 9972
		public int AttackTrigger = 1;

		// Token: 0x040026F5 RID: 9973
		[Range(0f, 1f)]
		public float On = 0.3f;

		// Token: 0x040026F6 RID: 9974
		[Range(0f, 1f)]
		public float Off = 0.6f;

		// Token: 0x040026F7 RID: 9975
		private bool isOn;

		// Token: 0x040026F8 RID: 9976
		private bool isOff;

		// Token: 0x040026F9 RID: 9977
		private Animal animal;

		// Token: 0x040026FA RID: 9978
		private float startAttackTime;

		// Token: 0x040026FB RID: 9979
		private float attackDelay;
	}
}
