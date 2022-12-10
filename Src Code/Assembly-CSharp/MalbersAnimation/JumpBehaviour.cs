using System;
using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003D5 RID: 981
	public class JumpBehaviour : StateMachineBehaviour
	{
		// Token: 0x0600375C RID: 14172 RVA: 0x001B6600 File Offset: 0x001B4800
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal = animator.GetComponent<Animal>();
			this.rb = animator.GetComponent<Rigidbody>();
			animator.applyRootMotion = true;
			this.rb.constraints = RigidbodyConstraints.FreezeRotation;
			this.jumpPoint = animator.transform.position.y;
			this.animal.InAir(true);
			this.animal.SetIntID(0);
			this.animal.OnJump.Invoke();
			this.Rb_Y_Speed = 0f;
			Vector3 rawDirection = this.animal.RawDirection;
			rawDirection.y = 0f;
			this.animal.AirControlDir = rawDirection;
			this.Can_Add_ExtraJump = ((this.JumpMultiplier > 0f && this.animal.JumpHeightMultiplier > 0f) || (this.ForwardMultiplier > 0f && this.animal.AirForwardMultiplier > 0f));
			this.ExtraJump = Vector3.up * this.JumpMultiplier * this.animal.JumpHeightMultiplier + animator.transform.forward * this.ForwardMultiplier * this.animal.AirForwardMultiplier;
			this.JumpSmoothPressed = 1f;
			this.JumpPressed = true;
			if (this.animal.JumpPress)
			{
				this.Can_Add_ExtraJump = (this.JumpPressed = this.animal.Jump);
			}
			this.JumpEnd = false;
			animator.SetFloat(this.animal.hash_IDFloat, 1f);
		}

		// Token: 0x0600375D RID: 14173 RVA: 0x001B6798 File Offset: 0x001B4998
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			bool flag = animator.IsInTransition(layerIndex);
			bool flag2 = flag && stateInfo.normalizedTime > 0.5f;
			if (this.JumpPressed)
			{
				this.JumpPressed = this.animal.Jump;
			}
			if (!flag && this.Can_Add_ExtraJump && !this.JumpEnd)
			{
				if (this.animal.JumpPress)
				{
					int num = this.JumpPressed ? 1 : 0;
					this.JumpSmoothPressed = Mathf.Lerp(this.JumpSmoothPressed, (float)num, Time.deltaTime * 5f);
				}
				this.animal.DeltaPosition += this.ExtraJump * Time.deltaTime * this.JumpSmoothPressed;
			}
			if (this.animal.FrameCounter % this.animal.FallRayInterval == 0)
			{
				this.Can_Fall(stateInfo.normalizedTime);
				this.Can_Jump_on_Cliff(stateInfo.normalizedTime);
			}
			if (this.rb && flag2 && animator.GetNextAnimatorStateInfo(layerIndex).tagHash == AnimTag.Fly)
			{
				float normalizedTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
				Vector3 velocity = this.rb.velocity;
				if (this.Rb_Y_Speed < velocity.y)
				{
					this.Rb_Y_Speed = velocity.y;
				}
				velocity.y = Mathf.Lerp(this.Rb_Y_Speed, 0f, normalizedTime);
				this.rb.velocity = velocity;
			}
		}

		// Token: 0x0600375E RID: 14174 RVA: 0x001B690C File Offset: 0x001B4B0C
		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.animal.AirControl)
			{
				this.AirControl();
			}
		}

		// Token: 0x0600375F RID: 14175 RVA: 0x001B6924 File Offset: 0x001B4B24
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animal.SetIntID(0);
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			if (currentAnimatorStateInfo.tagHash == AnimTag.Fly)
			{
				this.rb.velocity = new Vector3(this.rb.velocity.x, 0f, this.rb.velocity.z);
			}
			else if (currentAnimatorStateInfo.tagHash != AnimTag.Fall)
			{
				this.animal.IsInAir = false;
			}
			this.JumpEnd = true;
		}

		// Token: 0x06003760 RID: 14176 RVA: 0x001B69AC File Offset: 0x001B4BAC
		private void Can_Fall(float normalizedTime)
		{
			Debug.DrawRay(this.animal.Pivot_fall, -this.animal.transform.up * this.animal.Pivot_Multiplier * this.fallRay, Color.red);
			if (!Physics.Raycast(this.animal.Pivot_fall, -this.animal.transform.up, out this.JumpRay, this.animal.Pivot_Multiplier * this.fallRay, this.animal.GroundLayer))
			{
				if (normalizedTime > this.willFall)
				{
					this.animal.SetIntID(111);
				}
				MalbersTools.DebugPlane(this.animal.Pivot_fall - this.animal.transform.up * this.animal.Pivot_Multiplier * this.fallRay, 0.1f, Color.red, false);
				return;
			}
			if (this.jumpPoint - this.JumpRay.point.y <= this.stepHeight * this.animal.ScaleFactor && Vector3.Angle(this.JumpRay.normal, Vector3.up) < this.animal.maxAngleSlope)
			{
				this.animal.SetIntID(0);
				MalbersTools.DebugTriangle(this.JumpRay.point, 0.1f, Color.red);
				return;
			}
			if (normalizedTime > this.willFall)
			{
				this.animal.SetIntID(111);
			}
			MalbersTools.DebugTriangle(this.JumpRay.point, 0.1f, Color.yellow);
		}

		// Token: 0x06003761 RID: 14177 RVA: 0x001B6B58 File Offset: 0x001B4D58
		private void Can_Jump_on_Cliff(float normalizedTime)
		{
			if (normalizedTime >= this.Cliff.minValue && normalizedTime <= this.Cliff.maxValue)
			{
				if (Physics.Raycast(this.animal.Main_Pivot_Point, -this.animal.transform.up, out this.JumpRay, this.CliffRay * this.animal.ScaleFactor, this.animal.GroundLayer))
				{
					if (Vector3.Angle(this.JumpRay.normal, Vector3.up) < this.animal.maxAngleSlope)
					{
						if (this.animal.debug)
						{
							Debug.DrawLine(this.animal.Main_Pivot_Point, this.JumpRay.point, Color.black);
							MalbersTools.DebugTriangle(this.JumpRay.point, 0.1f, Color.black);
						}
						this.animal.SetIntID(110);
						return;
					}
				}
				else if (this.animal.debug)
				{
					Debug.DrawRay(this.animal.Main_Pivot_Point, -this.animal.transform.up * this.CliffRay * this.animal.ScaleFactor, Color.black);
					MalbersTools.DebugPlane(this.animal.Main_Pivot_Point - this.animal.transform.up * this.CliffRay * this.animal.ScaleFactor, 0.1f, Color.black, false);
				}
			}
		}

		// Token: 0x06003762 RID: 14178 RVA: 0x001B6CF0 File Offset: 0x001B4EF0
		private void AirControl()
		{
			float deltaTime = Time.deltaTime;
			float y = this.rb.velocity.y;
			Vector3 rawDirection = this.animal.RawDirection;
			rawDirection.y = 0f;
			this.animal.AirControlDir = Vector3.Lerp(this.animal.AirControlDir, rawDirection * this.ForwardMultiplier, deltaTime * this.animal.airSmoothness);
			Debug.DrawRay(this.animal.transform.position, this.animal.AirControlDir, Color.yellow);
			Vector3 vector = this.animal.AirControlDir * this.animal.airMaxSpeed;
			if (!this.animal.DirectionalMovement)
			{
				vector = this.animal.transform.TransformDirection(vector);
			}
			vector.y = y;
			this.rb.velocity = vector;
		}

		// Token: 0x0400274D RID: 10061
		[Header("Checking Fall")]
		[Tooltip("Ray Length to check if the ground is at the same level all the time")]
		public float fallRay = 1.7f;

		// Token: 0x0400274E RID: 10062
		[Tooltip("Terrain difference to be sure the animal will fall ")]
		public float stepHeight = 0.1f;

		// Token: 0x0400274F RID: 10063
		[Tooltip("Animation normalized time to change to fall animation if the ray checks if the animal is falling ")]
		[Range(0f, 1f)]
		public float willFall = 0.7f;

		// Token: 0x04002750 RID: 10064
		[Header("Jump on Higher Ground")]
		[Tooltip("Range to Calcultate if we can land on Higher ground")]
		[MinMaxRange(0f, 1f)]
		public RangedFloat Cliff = new RangedFloat(0.5f, 0.65f);

		// Token: 0x04002751 RID: 10065
		public float CliffRay = 0.6f;

		// Token: 0x04002752 RID: 10066
		[Space]
		[Header("Add more Height and Distance to the Jump")]
		public float JumpMultiplier = 1f;

		// Token: 0x04002753 RID: 10067
		public float ForwardMultiplier = 1f;

		// Token: 0x04002754 RID: 10068
		private Animal animal;

		// Token: 0x04002755 RID: 10069
		private Rigidbody rb;

		// Token: 0x04002756 RID: 10070
		private bool Can_Add_ExtraJump;

		// Token: 0x04002757 RID: 10071
		private Vector3 ExtraJump;

		// Token: 0x04002758 RID: 10072
		private bool JumpPressed;

		// Token: 0x04002759 RID: 10073
		private float jumpPoint;

		// Token: 0x0400275A RID: 10074
		private float Rb_Y_Speed;

		// Token: 0x0400275B RID: 10075
		private RaycastHit JumpRay;

		// Token: 0x0400275C RID: 10076
		private float JumpSmoothPressed = 1f;

		// Token: 0x0400275D RID: 10077
		private bool JumpEnd;
	}
}
