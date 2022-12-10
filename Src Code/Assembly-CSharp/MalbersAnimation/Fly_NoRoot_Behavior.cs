using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D4 RID: 980
	public class Fly_NoRoot_Behavior : StateMachineBehaviour
	{
		// Token: 0x06003757 RID: 14167 RVA: 0x001B5E9C File Offset: 0x001B409C
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.ResetAllValues();
			this.rb = animator.GetComponent<Rigidbody>();
			this.animal = animator.GetComponent<Animal>();
			this.BehaviourSpeed = this.animal.flySpeed;
			animator.applyRootMotion = true;
			this.transform = animator.transform;
			this.DeltaRotation = this.transform.rotation;
			this.acceleration = 0f;
			this.vertical = this.animal.Speed;
			this.FallVector = ((this.animal.CurrentAnimState == AnimTag.Fall || this.animal.CurrentAnimState == AnimTag.Jump) ? this.rb.velocity : Vector3.zero);
			this.rb.constraints = RigidbodyConstraints.FreezeRotation;
			this.rb.useGravity = false;
			this.rb.drag = this.Drag;
		}

		// Token: 0x06003758 RID: 14168 RVA: 0x001B5F80 File Offset: 0x001B4180
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float num = 1f;
			if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)
			{
				num = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
			}
			if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime > 0.5f)
			{
				num = 1f - animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
			}
			this.deltaTime = Time.deltaTime;
			this.transform.rotation = this.DeltaRotation;
			float num2 = (float)((this.animal.MovementAxis.z >= 0f) ? 1 : -1);
			this.Direction = Mathf.Lerp(this.Direction, Mathf.Clamp(this.animal.Direction, -1f, 1f), this.deltaTime * this.BehaviourSpeed.lerpRotation);
			Quaternion rhs = Quaternion.Euler(this.transform.InverseTransformDirection(0f, this.Direction * this.BehaviourSpeed.rotation * num2, 0f));
			this.DeltaRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up) * this.rb.rotation * rhs;
			float movementUp = this.animal.MovementUp;
			this.vertical = Mathf.Lerp(this.vertical, Mathf.Clamp(this.animal.Speed, -1f, 1f), this.deltaTime * 6f);
			Vector3 vector = Vector3.zero;
			Vector3 a = this.animal.T_Forward;
			if (this.animal.DirectionalMovement)
			{
				vector = this.animal.RawDirection;
				if (this.animal.IgnoreYDir)
				{
					vector.y = 0f;
				}
				vector.Normalize();
				vector += this.transform.up * movementUp;
				if (vector.magnitude > 1f)
				{
					vector.Normalize();
				}
			}
			else
			{
				vector = this.transform.forward * this.vertical + this.transform.up * movementUp;
				if (vector.magnitude > 1f)
				{
					vector.Normalize();
				}
				if (this.animal.MovementAxis.z < 0f)
				{
				}
				a = vector;
			}
			this.forwardAceleration = Mathf.Lerp(this.forwardAceleration, vector.magnitude, this.deltaTime * this.BehaviourSpeed.lerpPosition);
			Vector3 vector2 = a * this.forwardAceleration * this.BehaviourSpeed.position * ((this.animal.Speed < 0f) ? 0.5f : 1f) * this.deltaTime;
			vector2 = Vector3.Lerp(Vector3.zero, vector2, num);
			if (this.CanNotSwim)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.animal.Main_Pivot_Point, -Vector2.up, out raycastHit, this.animal.Pivot_Multiplier * this.animal.ScaleFactor * this.animal.FallRayMultiplier, 16))
				{
					this.foundWater = true;
				}
				else
				{
					this.foundWater = false;
				}
			}
			if (this.foundWater && vector2.y < 0f)
			{
				vector2.y = 0.001f;
				this.animal.DeltaPosition.y = 0f;
				this.animal.MovementUp = 0f;
			}
			this.animal.DeltaPosition += vector2;
			if (this.animal.debug)
			{
				Debug.DrawRay(this.transform.position, vector * 2f, Color.yellow);
			}
			if ((double)vector.magnitude > 0.001)
			{
				float num3 = 90f - Vector3.Angle(Vector3.up, vector);
				float num4 = Mathf.Max(Mathf.Abs(this.animal.MovementAxis.y), Mathf.Abs(this.vertical));
				num3 = Mathf.Clamp(-num3, -this.Ylimit, this.Ylimit);
				this.PitchAngle = Mathf.Lerp(this.PitchAngle, num3, this.deltaTime * this.animal.upDownSmoothness * 2f);
				this.animal.DeltaRotation *= Quaternion.Euler(this.PitchAngle * num4 * num, 0f, 0f);
			}
			this.animal.DeltaRotation *= Quaternion.Euler(0f, 0f, -this.Bank * this.Direction);
			if (this.foundWater)
			{
				return;
			}
			if (this.FallVector != Vector3.zero)
			{
				this.animal.DeltaPosition += this.FallVector * this.deltaTime;
				this.FallVector = Vector3.Lerp(this.FallVector, Vector3.zero, this.deltaTime * this.FallRecovery);
			}
			if (this.UseDownAcceleration)
			{
				this.GravityAcceleration(vector);
			}
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x001B64C4 File Offset: 0x001B46C4
		private void GravityAcceleration(Vector3 DirectionVector)
		{
			if ((double)this.animal.MovementAxis.y < -0.1)
			{
				this.acceleration = Mathf.Lerp(this.acceleration, this.acceleration + this.DownAcceleration, this.deltaTime);
			}
			else
			{
				float num = this.acceleration - this.DownAcceleration;
				if (num < 0f)
				{
					num = 0f;
				}
				this.acceleration = Mathf.Lerp(this.acceleration, num, this.deltaTime);
			}
			this.animal.DeltaPosition += DirectionVector * (this.acceleration * this.deltaTime);
		}

		// Token: 0x0600375A RID: 14170 RVA: 0x001B6570 File Offset: 0x001B4770
		private void ResetAllValues()
		{
			this.deltaTime = (this.acceleration = (this.forwardAceleration = (this.PitchAngle = (this.Direction = 0f))));
		}

		// Token: 0x04002738 RID: 10040
		[Range(0f, 90f)]
		[Tooltip("Adds Banking to the Fly animation when turning")]
		public float Bank = 30f;

		// Token: 0x04002739 RID: 10041
		[Range(0f, 90f)]
		[Tooltip("Top Angle the Animal Can go UP or Down ")]
		public float Ylimit = 80f;

		// Token: 0x0400273A RID: 10042
		public float Drag = 5f;

		// Token: 0x0400273B RID: 10043
		[Space]
		public bool UseDownAcceleration = true;

		// Token: 0x0400273C RID: 10044
		public float DownAcceleration = 3f;

		// Token: 0x0400273D RID: 10045
		public float FallRecovery = 1.5f;

		// Token: 0x0400273E RID: 10046
		[Space]
		public bool CanNotSwim;

		// Token: 0x0400273F RID: 10047
		protected float acceleration;

		// Token: 0x04002740 RID: 10048
		protected Rigidbody rb;

		// Token: 0x04002741 RID: 10049
		protected Animal animal;

		// Token: 0x04002742 RID: 10050
		protected Transform transform;

		// Token: 0x04002743 RID: 10051
		protected Quaternion DeltaRotation;

		// Token: 0x04002744 RID: 10052
		protected float Shift;

		// Token: 0x04002745 RID: 10053
		protected float Direction;

		// Token: 0x04002746 RID: 10054
		protected float deltaTime;

		// Token: 0x04002747 RID: 10055
		private Vector3 FallVector;

		// Token: 0x04002748 RID: 10056
		protected float forwardAceleration;

		// Token: 0x04002749 RID: 10057
		protected Speeds BehaviourSpeed;

		// Token: 0x0400274A RID: 10058
		private float PitchAngle;

		// Token: 0x0400274B RID: 10059
		private float vertical;

		// Token: 0x0400274C RID: 10060
		private bool foundWater;
	}
}
