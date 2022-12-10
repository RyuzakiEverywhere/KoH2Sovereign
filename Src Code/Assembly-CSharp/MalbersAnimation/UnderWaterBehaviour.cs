using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E2 RID: 994
	public class UnderWaterBehaviour : StateMachineBehaviour
	{
		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06003787 RID: 14215 RVA: 0x001B7B5F File Offset: 0x001B5D5F
		// (set) Token: 0x06003788 RID: 14216 RVA: 0x001B7B67 File Offset: 0x001B5D67
		public float PitchAngle { get; private set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06003789 RID: 14217 RVA: 0x001B7B70 File Offset: 0x001B5D70
		// (set) Token: 0x0600378A RID: 14218 RVA: 0x001B7B78 File Offset: 0x001B5D78
		public bool Default_UseShift { get; private set; }

		// Token: 0x0600378B RID: 14219 RVA: 0x001B7B84 File Offset: 0x001B5D84
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.rb = animator.GetComponent<Rigidbody>();
			this.animal = animator.GetComponent<Animal>();
			animator.applyRootMotion = true;
			this.transform = animator.transform;
			this.DeltaRotation = this.transform.rotation;
			this.rb.constraints = RigidbodyConstraints.FreezeRotation;
			this.rb.useGravity = false;
			this.Default_UseShift = this.animal.UseShift;
			this.animal.UseShift = false;
			this.WaterLayer = LayerMask.GetMask(new string[]
			{
				"Water"
			});
			this.BehaviourSpeed = this.animal.underWaterSpeed;
		}

		// Token: 0x0600378C RID: 14220 RVA: 0x001B7C30 File Offset: 0x001B5E30
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this.animal.CanGoUnderWater || !this.animal.Underwater)
			{
				return;
			}
			float t = 1f;
			if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)
			{
				t = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
			}
			if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime > 0.5f)
			{
				t = 1f - animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
			}
			this.deltaTime = Time.deltaTime;
			if (this.useShift)
			{
				this.Shift = Mathf.Lerp(this.Shift, this.animal.Shift ? this.ShiftMultiplier : 1f, this.BehaviourSpeed.lerpPosition * this.deltaTime);
			}
			if (this.animal.Up)
			{
				this.animal.Down = false;
			}
			this.transform.rotation = this.DeltaRotation;
			float num = (float)((this.animal.MovementAxis.z >= 0f) ? 1 : -1);
			this.Direction = Mathf.Lerp(this.Direction, Mathf.Clamp(this.animal.Direction, -1f, 1f), this.deltaTime * this.BehaviourSpeed.lerpRotation);
			Quaternion rhs = Quaternion.Euler(this.transform.InverseTransformDirection(0f, this.Direction * this.BehaviourSpeed.rotation * num, 0f));
			this.animal.DeltaRotation *= rhs;
			this.DeltaRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up) * this.rb.rotation * rhs;
			float movementUp = this.animal.MovementUp;
			float num2 = Mathf.Clamp(this.animal.Speed, -1f, 1f);
			Vector3 vector = Vector3.zero;
			Vector3 a = this.animal.T_Forward;
			if (this.animal.DirectionalMovement)
			{
				vector = this.animal.RawDirection;
				vector += this.transform.up * movementUp;
			}
			else
			{
				vector = this.transform.forward * num2 + this.transform.up * movementUp;
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
			Vector3 b = a * this.forwardAceleration * this.BehaviourSpeed.position * this.Shift * ((this.animal.Speed < 0f) ? 0.5f : 1f) * this.deltaTime;
			b = Vector3.Lerp(Vector3.zero, b, t);
			this.animal.DeltaPosition += b;
			if ((double)vector.magnitude > 0.001)
			{
				float num3 = 90f - Vector3.Angle(Vector3.up, vector);
				float num4 = Mathf.Max(Mathf.Abs(this.animal.MovementAxis.y), Mathf.Abs(num2));
				num3 = Mathf.Clamp(-num3, -this.Ylimit, this.Ylimit);
				this.PitchAngle = Mathf.Lerp(this.PitchAngle, num3, this.deltaTime * this.animal.upDownSmoothness * 2f);
				this.transform.Rotate(Mathf.Clamp(this.PitchAngle, -this.Ylimit, this.Ylimit) * num4, 0f, 0f, Space.Self);
			}
			if (this.animal.debug)
			{
				Debug.DrawRay(this.transform.position, vector * 2f, Color.yellow);
			}
			this.transform.Rotate(0f, 0f, -this.Bank * Mathf.Clamp(this.Direction, -1f, 1f), Space.Self);
			this.CheckExitUnderWater();
		}

		// Token: 0x0600378D RID: 14221 RVA: 0x001B80A1 File Offset: 0x001B62A1
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.UseShift = this.Default_UseShift;
		}

		// Token: 0x0600378E RID: 14222 RVA: 0x001B80B4 File Offset: 0x001B62B4
		protected void CheckExitUnderWater()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.transform.position + new Vector3(0f, (this.animal.height - this.animal.waterLine) * this.animal.ScaleFactor, 0f), -Vector3.up, out raycastHit, this.animal.ScaleFactor, this.WaterLayer) && !this.animal.Down)
			{
				this.animal.Underwater = false;
				this.animal.Anim.applyRootMotion = true;
				this.rb.useGravity = true;
				this.rb.drag = 0f;
				this.rb.constraints = Animal.Still_Constraints;
				this.animal.MovementAxis = new Vector3(this.animal.MovementAxis.x, 0f, this.animal.MovementAxis.z);
			}
		}

		// Token: 0x040027A7 RID: 10151
		[Range(0f, 90f)]
		public float Bank;

		// Token: 0x040027A8 RID: 10152
		[Range(0f, 90f)]
		public float Ylimit = 87f;

		// Token: 0x040027A9 RID: 10153
		[Space]
		public bool useShift = true;

		// Token: 0x040027AA RID: 10154
		public float ShiftMultiplier = 2f;

		// Token: 0x040027AB RID: 10155
		[Space]
		protected Rigidbody rb;

		// Token: 0x040027AC RID: 10156
		protected Animal animal;

		// Token: 0x040027AD RID: 10157
		protected Transform transform;

		// Token: 0x040027AE RID: 10158
		protected Quaternion DeltaRotation;

		// Token: 0x040027AF RID: 10159
		protected float Shift;

		// Token: 0x040027B0 RID: 10160
		protected float deltaTime;

		// Token: 0x040027B1 RID: 10161
		private Speeds BehaviourSpeed;

		// Token: 0x040027B2 RID: 10162
		private int WaterLayer;

		// Token: 0x040027B3 RID: 10163
		private float Direction;

		// Token: 0x040027B4 RID: 10164
		private float forwardAceleration;
	}
}
