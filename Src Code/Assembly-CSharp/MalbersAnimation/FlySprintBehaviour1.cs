using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003CE RID: 974
	public class FlySprintBehaviour1 : StateMachineBehaviour
	{
		// Token: 0x06003742 RID: 14146 RVA: 0x001B51E0 File Offset: 0x001B33E0
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.BehaviourSpeed = this.animal.flySpeed;
			if (this.Speed_Param != string.Empty)
			{
				this.SpeedHash = Animator.StringToHash(this.Speed_Param);
				animator.SetFloat(this.SpeedHash, this.AnimSpeedDefault);
			}
			this.CurrentSpeedMultiplier = this.AnimSpeedDefault;
		}

		// Token: 0x06003743 RID: 14147 RVA: 0x001B524C File Offset: 0x001B344C
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float deltaTime = Time.deltaTime;
			this.Shift = Mathf.Lerp(this.Shift, this.animal.Shift ? this.ShiftMultiplier : 1f, this.BehaviourSpeed.lerpPosition * deltaTime);
			this.CurrentSpeedMultiplier = Mathf.Lerp(this.CurrentSpeedMultiplier, this.animal.Shift ? this.AnimSprintSpeed : this.AnimSpeedDefault, deltaTime * this.AnimSprintLerp);
			if (this.Speed_Param != string.Empty)
			{
				this.animal.Anim.SetFloat(this.SpeedHash, this.CurrentSpeedMultiplier);
			}
			this.animal.DeltaPosition += this.animal.T_Forward * this.Shift * deltaTime;
		}

		// Token: 0x04002703 RID: 9987
		public bool UseSprint = true;

		// Token: 0x04002704 RID: 9988
		[Tooltip("Float Parameter on the Animator to Modify When Sprint is Enabled, if this value is null it will not change the multiplier")]
		public string Speed_Param = "SpeedMultiplier";

		// Token: 0x04002705 RID: 9989
		public float ShiftMultiplier = 2f;

		// Token: 0x04002706 RID: 9990
		public float AnimSpeedDefault = 1f;

		// Token: 0x04002707 RID: 9991
		[Tooltip("Amount of Speed Multiplier  to use on the Speed Multiplier Animator parameter when 'UseSprint' is Enabled\n if this value is null it will not change the multiplier")]
		public float AnimSprintSpeed = 2f;

		// Token: 0x04002708 RID: 9992
		[Tooltip("Smoothness to use when the SpeedMultiplier changes")]
		public float AnimSprintLerp = 2f;

		// Token: 0x04002709 RID: 9993
		protected int SpeedHash = Animator.StringToHash("SpeedMultiplier");

		// Token: 0x0400270A RID: 9994
		protected float CurrentSpeedMultiplier;

		// Token: 0x0400270B RID: 9995
		protected float Shift;

		// Token: 0x0400270C RID: 9996
		protected Animal animal;

		// Token: 0x0400270D RID: 9997
		protected Speeds BehaviourSpeed;
	}
}
