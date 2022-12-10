using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x0200045E RID: 1118
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public class MThirdPersonCharacter : MonoBehaviour, ICharacterMove
	{
		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06003B03 RID: 15107 RVA: 0x001C572F File Offset: 0x001C392F
		// (set) Token: 0x06003B04 RID: 15108 RVA: 0x001C5737 File Offset: 0x001C3937
		public bool Jump { get; set; }

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06003B05 RID: 15109 RVA: 0x001C5740 File Offset: 0x001C3940
		// (set) Token: 0x06003B06 RID: 15110 RVA: 0x001C5748 File Offset: 0x001C3948
		public bool Shift { get; set; }

		// Token: 0x06003B07 RID: 15111 RVA: 0x001C5751 File Offset: 0x001C3951
		private void Start()
		{
			this.m_Animator = base.GetComponent<Animator>();
			this.m_Rigidbody = base.GetComponent<Rigidbody>();
			this.m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			this.m_OrigGroundCheckDistance = this.m_GroundCheckDistance;
		}

		// Token: 0x06003B08 RID: 15112 RVA: 0x001C5784 File Offset: 0x001C3984
		public void Move(Vector3 move, bool directional)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.Shift)
			{
				move *= 0.5f;
			}
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			move = base.transform.InverseTransformDirection(move);
			this.CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, this.m_GroundNormal);
			this.m_TurnAmount = Mathf.Atan2(move.x, move.z);
			this.m_ForwardAmount = move.z;
			this.ApplyExtraTurnRotation();
			if (this.m_IsGrounded)
			{
				this.HandleGroundedMovement(this.Jump);
			}
			else
			{
				this.HandleAirborneMovement();
			}
			this.UpdateAnimator(move);
		}

		// Token: 0x06003B09 RID: 15113 RVA: 0x001C5834 File Offset: 0x001C3A34
		private void UpdateAnimator(Vector3 move)
		{
			if (!this.m_Animator.isActiveAndEnabled)
			{
				return;
			}
			this.m_Animator.SetFloat("Forward", this.m_ForwardAmount, 0.1f, Time.deltaTime);
			this.m_Animator.SetFloat("Turn", this.m_TurnAmount, 0.1f, Time.deltaTime);
			this.m_Animator.SetBool("OnGround", this.m_IsGrounded);
			if (!this.m_IsGrounded)
			{
				this.m_Animator.SetFloat("Jump", this.m_Rigidbody.velocity.y);
			}
			float value = (float)((Mathf.Repeat(this.m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + this.m_RunCycleLegOffset, 1f) < 0.5f) ? 1 : -1) * this.m_ForwardAmount;
			if (this.m_IsGrounded)
			{
				this.m_Animator.SetFloat("JumpLeg", value);
			}
			if (this.m_IsGrounded && move.magnitude > 0f)
			{
				this.m_Animator.speed = this.m_AnimSpeedMultiplier;
				return;
			}
			this.m_Animator.speed = 1f;
		}

		// Token: 0x06003B0A RID: 15114 RVA: 0x001C5958 File Offset: 0x001C3B58
		private void HandleAirborneMovement()
		{
			Vector3 force = Physics.gravity * this.m_GravityMultiplier - Physics.gravity;
			this.m_Rigidbody.AddForce(force);
			this.m_GroundCheckDistance = ((this.m_Rigidbody.velocity.y < 0f) ? this.m_OrigGroundCheckDistance : 0.01f);
		}

		// Token: 0x06003B0B RID: 15115 RVA: 0x001C59B8 File Offset: 0x001C3BB8
		private void HandleGroundedMovement(bool jump)
		{
			if (jump && this.m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				this.m_Rigidbody.velocity = new Vector3(this.m_Rigidbody.velocity.x, this.m_JumpPower, this.m_Rigidbody.velocity.z);
				this.m_IsGrounded = false;
				this.m_Animator.applyRootMotion = false;
				this.m_GroundCheckDistance = 0.1f;
			}
		}

		// Token: 0x06003B0C RID: 15116 RVA: 0x001C5A38 File Offset: 0x001C3C38
		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(this.m_StationaryTurnSpeed, this.m_MovingTurnSpeed, this.m_ForwardAmount);
			base.transform.Rotate(0f, this.m_TurnAmount * num * Time.deltaTime, 0f);
		}

		// Token: 0x06003B0D RID: 15117 RVA: 0x001C5A80 File Offset: 0x001C3C80
		public void OnAnimatorMove()
		{
			if (this.m_IsGrounded && Time.deltaTime > 0f)
			{
				Vector3 velocity = this.m_Animator.deltaPosition * this.m_MoveSpeedMultiplier / Time.deltaTime;
				velocity.y = this.m_Rigidbody.velocity.y;
				this.m_Rigidbody.velocity = velocity;
			}
		}

		// Token: 0x06003B0E RID: 15118 RVA: 0x001C5AE8 File Offset: 0x001C3CE8
		private void CheckGroundStatus()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out raycastHit, this.m_GroundCheckDistance))
			{
				this.m_GroundNormal = raycastHit.normal;
				this.m_IsGrounded = true;
				this.m_Animator.applyRootMotion = true;
				return;
			}
			this.m_IsGrounded = false;
			this.m_GroundNormal = Vector3.up;
			this.m_Animator.applyRootMotion = false;
		}

		// Token: 0x04002AE2 RID: 10978
		[SerializeField]
		private float m_MovingTurnSpeed = 360f;

		// Token: 0x04002AE3 RID: 10979
		[SerializeField]
		private float m_StationaryTurnSpeed = 180f;

		// Token: 0x04002AE4 RID: 10980
		[SerializeField]
		private float m_JumpPower = 12f;

		// Token: 0x04002AE5 RID: 10981
		[Range(1f, 4f)]
		[SerializeField]
		private float m_GravityMultiplier = 2f;

		// Token: 0x04002AE6 RID: 10982
		[SerializeField]
		private float m_RunCycleLegOffset = 0.2f;

		// Token: 0x04002AE7 RID: 10983
		[SerializeField]
		private float m_MoveSpeedMultiplier = 1f;

		// Token: 0x04002AE8 RID: 10984
		[SerializeField]
		private float m_AnimSpeedMultiplier = 1f;

		// Token: 0x04002AE9 RID: 10985
		[SerializeField]
		private float m_GroundCheckDistance = 0.1f;

		// Token: 0x04002AEA RID: 10986
		private Rigidbody m_Rigidbody;

		// Token: 0x04002AEB RID: 10987
		private Animator m_Animator;

		// Token: 0x04002AEC RID: 10988
		private bool m_IsGrounded;

		// Token: 0x04002AED RID: 10989
		private float m_OrigGroundCheckDistance;

		// Token: 0x04002AEE RID: 10990
		private const float k_Half = 0.5f;

		// Token: 0x04002AEF RID: 10991
		private float m_TurnAmount;

		// Token: 0x04002AF0 RID: 10992
		private float m_ForwardAmount;

		// Token: 0x04002AF1 RID: 10993
		private Vector3 m_GroundNormal;
	}
}
