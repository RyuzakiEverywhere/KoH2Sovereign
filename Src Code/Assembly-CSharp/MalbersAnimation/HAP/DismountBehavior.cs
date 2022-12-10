using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000424 RID: 1060
	public class DismountBehavior : StateMachineBehaviour
	{
		// Token: 0x06003924 RID: 14628 RVA: 0x001BD8C0 File Offset: 0x001BBAC0
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetInteger(Hash.MountSide, 0);
			this.rider = animator.GetComponent<Rider3rdPerson>();
			this.ScaleFactor = this.rider.Montura.Animal.ScaleFactor;
			this.LeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			this.RightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			this.MountPoint = this.rider.Montura.MountPoint;
			this.BottomPosition = this.MountPoint.InverseTransformPoint((this.LeftFoot.position + this.RightFoot.position) / 2f);
			this.HipPosition = this.MountPoint.InverseTransformPoint(animator.rootPosition);
			this.Fix = this.rider.MountTrigger.Adjustment;
			this.transform = animator.transform;
			this.rider.Start_Dismounting();
			this.transform.position = this.rider.Montura.MountPoint.position;
			this.transform.rotation = this.rider.Montura.MountPoint.rotation;
			this.LastRelativeRiderPosition = this.MountPoint.InverseTransformPoint(this.transform.position);
		}

		// Token: 0x06003925 RID: 14629 RVA: 0x001BDA06 File Offset: 0x001BBC06
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rider.End_Dismounting();
		}

		// Token: 0x06003926 RID: 14630 RVA: 0x001BDA14 File Offset: 0x001BBC14
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float normalizedTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
			float deltaTime = Time.deltaTime;
			this.transform.rotation = animator.rootRotation;
			this.transform.position = this.MountPoint.TransformPoint(this.LastRelativeRiderPosition);
			if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)
			{
				Vector3 position = this.MountPoint.position;
				position.y = this.MountPoint.TransformPoint(Vector3.Lerp(this.HipPosition, this.BottomPosition, normalizedTime)).y;
				this.transform.position = position;
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.FromToRotation(this.transform.up, Vector3.up) * this.transform.rotation, normalizedTime);
			}
			else
			{
				this.transform.position += animator.velocity * Time.deltaTime * this.ScaleFactor * (this.Fix ? this.Fix.delay : 1f);
			}
			if (this.rider.Montura)
			{
				this.rider.Montura.Animal.MovementAxis = this.rider.Montura.Animal.MovementAxis * (1f - stateInfo.normalizedTime);
				if (this.rider.MountTrigger && this.transform.position.y < this.rider.MountTrigger.transform.position.y)
				{
					this.transform.position = new Vector3(this.transform.position.x, this.rider.MountTrigger.transform.position.y, this.transform.position.z);
				}
				if (stateInfo.normalizedTime > 0.8f)
				{
					this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.FromToRotation(this.transform.up, Vector3.up) * this.transform.rotation, normalizedTime);
					this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, this.rider.MountTrigger.transform.position.y, this.transform.position.z), deltaTime * 5f);
				}
			}
			animator.rootPosition = this.transform.position;
			this.LastRelativeRiderPosition = this.MountPoint.InverseTransformPoint(this.transform.position);
		}

		// Token: 0x0400293D RID: 10557
		private Rider3rdPerson rider;

		// Token: 0x0400293E RID: 10558
		private Vector3 HipPosition;

		// Token: 0x0400293F RID: 10559
		private Vector3 BottomPosition;

		// Token: 0x04002940 RID: 10560
		private Transform transform;

		// Token: 0x04002941 RID: 10561
		private Transform MountPoint;

		// Token: 0x04002942 RID: 10562
		private Vector3 LastRelativeRiderPosition;

		// Token: 0x04002943 RID: 10563
		private TransformAnimation Fix;

		// Token: 0x04002944 RID: 10564
		private float ScaleFactor;

		// Token: 0x04002945 RID: 10565
		private Transform LeftFoot;

		// Token: 0x04002946 RID: 10566
		private Transform RightFoot;

		// Token: 0x04002947 RID: 10567
		private IAnimatorBehaviour animatorListener;
	}
}
