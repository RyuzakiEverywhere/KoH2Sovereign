using System;
using MalbersAnimations.HAP;
using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000422 RID: 1058
	public class AimIKBehaviour : StateMachineBehaviour
	{
		// Token: 0x0600391F RID: 14623 RVA: 0x001BD61B File Offset: 0x001BB81B
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.RC = animator.GetComponent<RiderCombat>();
			this.IkMode = (this.RC.ActiveAbility as GunCombatIK);
			this.active = false;
			if (this.IkMode)
			{
				this.active = true;
			}
		}

		// Token: 0x06003920 RID: 14624 RVA: 0x001BD65C File Offset: 0x001BB85C
		public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this.active)
			{
				return;
			}
			bool rightHand = this.RC.Active_IMWeapon.RightHand;
			Vector3 origin = rightHand ? this.RC.RightShoulder.position : this.RC.LeftShoulder.position;
			float distance = rightHand ? this.IkMode.HandIKDistance.Evaluate(1f + this.RC.HorizontalAngle) : this.IkMode.HandIKDistance.Evaluate(1f - this.RC.HorizontalAngle);
			Vector3 vector = this.RC.Target ? this.RC.AimDirection : (this.RC.AimDot ? MalbersTools.DirectionFromCameraNoRayCast(this.RC.AimDot.position) : Camera.main.transform.forward);
			Ray ray = new Ray(origin, vector);
			Vector3 point = ray.GetPoint(distance);
			Vector3 normalized = (this.RC.AimRayCastHit.point - (rightHand ? this.RC.RightHand.position : this.RC.LeftHand.position)).normalized;
			if (this.RC.IsAiming)
			{
				this.Weight = Mathf.Lerp(this.Weight, 1f, Time.deltaTime * 10f);
				Quaternion goalRotation = Quaternion.LookRotation(vector) * Quaternion.Euler(rightHand ? this.IkMode.RightHandOffset : this.IkMode.LeftHandOffset);
				AvatarIKGoal goal = rightHand ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;
				animator.SetIKPosition(goal, point);
				animator.SetIKPositionWeight(goal, this.Weight);
				if (this.RC.WeaponAction != WeaponActions.Fire_Proyectile)
				{
					animator.SetIKRotation(goal, goalRotation);
					animator.SetIKRotationWeight(goal, this.Weight);
				}
				animator.SetLookAtWeight(1f * this.Weight, 0.3f * this.Weight);
				animator.SetLookAtPosition(ray.GetPoint(10f));
			}
		}

		// Token: 0x04002938 RID: 10552
		[Header("This is Link to the GunCombatIKMode")]
		public bool active = true;

		// Token: 0x04002939 RID: 10553
		private float Weight;

		// Token: 0x0400293A RID: 10554
		private RiderCombat RC;

		// Token: 0x0400293B RID: 10555
		private GunCombatIK IkMode;
	}
}
