using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D3 RID: 979
	public class FlySprintBehaviour : StateMachineBehaviour
	{
		// Token: 0x06003754 RID: 14164 RVA: 0x001B5C3C File Offset: 0x001B3E3C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.BehaviourSpeed = this.animal.flySpeed;
			this.Shift = 0f;
			if (this.Speed_Param != string.Empty)
			{
				this.SpeedHash = Animator.StringToHash(this.Speed_Param);
				animator.SetFloat(this.SpeedHash, this.AnimSpeedDefault);
			}
			this.CurrentSpeedMultiplier = this.AnimSpeedDefault;
		}

		// Token: 0x06003755 RID: 14165 RVA: 0x001B5CB4 File Offset: 0x001B3EB4
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float deltaTime = Time.deltaTime;
			this.Shift = Mathf.Lerp(this.Shift, this.animal.Shift ? this.ShiftMultiplier : 1f, this.BehaviourSpeed.lerpPosition * deltaTime);
			this.CurrentSpeedMultiplier = Mathf.Lerp(this.CurrentSpeedMultiplier, (this.animal.Shift && this.animal.MovementForward > 0f) ? this.AnimSprintSpeed : this.AnimSpeedDefault, deltaTime * this.AnimSprintLerp);
			if (this.Speed_Param != string.Empty)
			{
				this.animal.Anim.SetFloat(this.SpeedHash, this.CurrentSpeedMultiplier);
			}
			if (this.IsRootMotion)
			{
				this.animal.DeltaPosition += animator.velocity * this.Shift * deltaTime;
			}
			else
			{
				this.animal.DeltaPosition += this.animal.T_Forward * this.Shift * Mathf.Clamp(this.animal.Speed, 0f, 1f) * deltaTime;
			}
			if (this.animal.Shift && this.NoGliding)
			{
				this.animal.Speed = Mathf.Lerp(this.animal.Speed, 1f, deltaTime * 6f);
			}
		}

		// Token: 0x0400272C RID: 10028
		public bool IsRootMotion;

		// Token: 0x0400272D RID: 10029
		[Tooltip("Float Parameter on the Animator to Modify When Sprint is Enabled, if this value is null it will not change the multiplier")]
		public string Speed_Param = "SpeedMultiplier";

		// Token: 0x0400272E RID: 10030
		public float ShiftMultiplier = 2f;

		// Token: 0x0400272F RID: 10031
		public float AnimSpeedDefault = 1f;

		// Token: 0x04002730 RID: 10032
		[Tooltip("Amount of Speed Multiplier  to use on the Speed Multiplier Animator parameter when 'UseSprint' is Enabled\n if this value is null it will not change the multiplier")]
		public float AnimSprintSpeed = 2f;

		// Token: 0x04002731 RID: 10033
		[Tooltip("Smoothness to use when the SpeedMultiplier changes")]
		public float AnimSprintLerp = 2f;

		// Token: 0x04002732 RID: 10034
		[Tooltip("Do not Glide while pressing shift")]
		public bool NoGliding = true;

		// Token: 0x04002733 RID: 10035
		protected int SpeedHash = Animator.StringToHash("SpeedMultiplier");

		// Token: 0x04002734 RID: 10036
		protected float CurrentSpeedMultiplier;

		// Token: 0x04002735 RID: 10037
		protected float Shift;

		// Token: 0x04002736 RID: 10038
		protected Animal animal;

		// Token: 0x04002737 RID: 10039
		protected Speeds BehaviourSpeed;
	}
}
