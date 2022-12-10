using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000425 RID: 1061
	public class MountBehavior : StateMachineBehaviour
	{
		// Token: 0x06003928 RID: 14632 RVA: 0x001BDD14 File Offset: 0x001BBF14
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetInteger(Hash.MountSide, 0);
			this.rider = animator.GetComponent<Rider3rdPerson>();
			this.transform = animator.transform;
			this.AnimalScaleFactor = this.rider.Montura.Animal.ScaleFactor;
			MountBehavior.ResetFloatParameters(animator);
			this.MountTrigger = this.rider.MountTrigger.transform;
			this.Fix = this.rider.MountTrigger.Adjustment;
			this.rider.Start_Mounting();
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x001BDDA0 File Offset: 0x001BBFA0
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.transform.position += animator.velocity * Time.deltaTime * this.AnimalScaleFactor * (this.Fix ? this.Fix.time : 1f);
			this.transform.rotation = animator.rootRotation;
			Vector3 position = this.rider.Montura.MountPoint.position;
			if (stateInfo.normalizedTime < 0.2f)
			{
				Vector3 b = new Vector3(this.MountTrigger.position.x, this.transform.position.y, this.MountTrigger.position.z);
				this.transform.position = Vector3.Lerp(this.transform.position, b, stateInfo.normalizedTime / 0.2f);
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(this.MountTrigger.forward), stateInfo.normalizedTime / 0.2f);
			}
			if (!this.Fix)
			{
				this.transform.position = Vector3.Lerp(this.transform.position, this.rider.Montura.MountPoint.position, this.MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.rider.Montura.MountPoint.rotation, this.MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
				return;
			}
			if (this.Fix.UsePosition)
			{
				if (!this.Fix.SeparateAxisPos)
				{
					this.transform.position = Vector3.LerpUnclamped(this.transform.position, position, this.Fix.PosCurve.Evaluate(stateInfo.normalizedTime));
				}
				else
				{
					float x = Mathf.LerpUnclamped(this.transform.position.x, position.x, this.Fix.PosXCurve.Evaluate(stateInfo.normalizedTime) * this.Fix.Position.x);
					float y = Mathf.LerpUnclamped(this.transform.position.y, position.y, this.Fix.PosYCurve.Evaluate(stateInfo.normalizedTime) * this.Fix.Position.y);
					float z = Mathf.LerpUnclamped(this.transform.position.z, position.z, this.Fix.PosZCurve.Evaluate(stateInfo.normalizedTime) * this.Fix.Position.z);
					Vector3 position2 = new Vector3(x, y, z);
					this.transform.position = position2;
				}
			}
			else
			{
				this.transform.position = Vector3.Lerp(this.transform.position, this.rider.Montura.MountPoint.position, this.MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
			}
			if (this.Fix.UseRotation)
			{
				this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.rider.Montura.MountPoint.rotation, this.Fix.RotCurve.Evaluate(stateInfo.normalizedTime));
				return;
			}
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.rider.Montura.MountPoint.rotation, this.MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x001BE17C File Offset: 0x001BC37C
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rider.End_Mounting();
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x001BE18C File Offset: 0x001BC38C
		private static void ResetFloatParameters(Animator animator)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
			{
				if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
				{
					if (animatorControllerParameter.nameHash == Hash.IKLeftFoot || animatorControllerParameter.nameHash == Hash.IKRightFoot)
					{
						break;
					}
					animator.SetFloat(animatorControllerParameter.nameHash, animatorControllerParameter.defaultFloat);
				}
			}
		}

		// Token: 0x04002948 RID: 10568
		public AnimationCurve MovetoMountPoint;

		// Token: 0x04002949 RID: 10569
		protected Rider3rdPerson rider;

		// Token: 0x0400294A RID: 10570
		protected Transform MountTrigger;

		// Token: 0x0400294B RID: 10571
		protected Transform transform;

		// Token: 0x0400294C RID: 10572
		protected Transform hip;

		// Token: 0x0400294D RID: 10573
		private const float toMountPoint = 0.2f;

		// Token: 0x0400294E RID: 10574
		private float AnimalScaleFactor;

		// Token: 0x0400294F RID: 10575
		private TransformAnimation Fix;
	}
}
