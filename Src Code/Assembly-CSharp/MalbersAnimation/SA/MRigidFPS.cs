using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000458 RID: 1112
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class MRigidFPS : MonoBehaviour
	{
		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06003ADC RID: 15068 RVA: 0x001C48DA File Offset: 0x001C2ADA
		// (set) Token: 0x06003ADB RID: 15067 RVA: 0x001C48D1 File Offset: 0x001C2AD1
		public bool LockMovement
		{
			get
			{
				return this.lockMovement;
			}
			set
			{
				this.lockMovement = value;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06003ADD RID: 15069 RVA: 0x001C48E2 File Offset: 0x001C2AE2
		public Vector3 Velocity
		{
			get
			{
				return this.m_RigidBody.velocity;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06003ADE RID: 15070 RVA: 0x001C48EF File Offset: 0x001C2AEF
		public bool Grounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06003ADF RID: 15071 RVA: 0x001C48F7 File Offset: 0x001C2AF7
		public bool Jumping
		{
			get
			{
				return this.m_Jumping;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06003AE0 RID: 15072 RVA: 0x001C48FF File Offset: 0x001C2AFF
		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		// Token: 0x06003AE1 RID: 15073 RVA: 0x001C490C File Offset: 0x001C2B0C
		private void Start()
		{
			this.m_RigidBody = base.GetComponent<Rigidbody>();
			this.m_Capsule = base.GetComponent<CapsuleCollider>();
			Cursor.lockState = (this.LockCursor ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !this.LockCursor;
			this.RestartMouseLook();
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x001C494B File Offset: 0x001C2B4B
		private void Update()
		{
			this.RotateView();
			if (Input.GetButtonDown("Jump") && !this.m_Jump)
			{
				this.m_Jump = true;
			}
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x001C496E File Offset: 0x001C2B6E
		public void RestartMouseLook()
		{
			this.mouseLook.Init(base.transform, this.cam.transform);
		}

		// Token: 0x06003AE4 RID: 15076 RVA: 0x001C498C File Offset: 0x001C2B8C
		private void FixedUpdate()
		{
			if (this.lockMovement)
			{
				return;
			}
			this.GroundCheck();
			Vector2 input = this.GetInput();
			if ((Mathf.Abs(input.x) > 1E-45f || Mathf.Abs(input.y) > 1E-45f) && (this.advancedSettings.airControl || this.m_IsGrounded))
			{
				Vector3 vector = this.cam.transform.forward * input.y + this.cam.transform.right * input.x;
				vector = Vector3.ProjectOnPlane(vector, this.m_GroundContactNormal).normalized;
				vector.x *= this.movementSettings.CurrentTargetSpeed;
				vector.z *= this.movementSettings.CurrentTargetSpeed;
				vector.y *= this.movementSettings.CurrentTargetSpeed;
				if (this.m_RigidBody.velocity.sqrMagnitude < this.movementSettings.CurrentTargetSpeed * this.movementSettings.CurrentTargetSpeed)
				{
					this.m_RigidBody.AddForce(vector * this.SlopeMultiplier(), ForceMode.Impulse);
				}
			}
			if (this.m_IsGrounded)
			{
				this.m_RigidBody.drag = 5f;
				if (this.m_Jump)
				{
					this.m_RigidBody.drag = 0f;
					this.m_RigidBody.velocity = new Vector3(this.m_RigidBody.velocity.x, 0f, this.m_RigidBody.velocity.z);
					this.m_RigidBody.AddForce(new Vector3(0f, this.movementSettings.JumpForce, 0f), ForceMode.Impulse);
					this.m_Jumping = true;
				}
				if (!this.m_Jumping && Mathf.Abs(input.x) < 1E-45f && Mathf.Abs(input.y) < 1E-45f && this.m_RigidBody.velocity.magnitude < 1f)
				{
					this.m_RigidBody.Sleep();
				}
			}
			else
			{
				this.m_RigidBody.drag = 0f;
				if (this.m_PreviouslyGrounded && !this.m_Jumping)
				{
					this.StickToGroundHelper();
				}
			}
			this.m_Jump = false;
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x001C4BE4 File Offset: 0x001C2DE4
		private float SlopeMultiplier()
		{
			float time = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(time);
		}

		// Token: 0x06003AE6 RID: 15078 RVA: 0x001C4C14 File Offset: 0x001C2E14
		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius, Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.stickToGroundHelperDistance) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, raycastHit.normal);
			}
		}

		// Token: 0x06003AE7 RID: 15079 RVA: 0x001C4CB0 File Offset: 0x001C2EB0
		private Vector2 GetInput()
		{
			Vector2 vector = new Vector2
			{
				x = Input.GetAxis("Horizontal"),
				y = Input.GetAxis("Vertical")
			};
			this.movementSettings.UpdateDesiredTargetSpeed(vector);
			return vector;
		}

		// Token: 0x06003AE8 RID: 15080 RVA: 0x001C4CF8 File Offset: 0x001C2EF8
		public virtual void RotateView()
		{
			if (Mathf.Abs(Time.timeScale) < 1E-45f)
			{
				return;
			}
			float y = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.cam.transform);
			if (this.m_IsGrounded || this.advancedSettings.airControl)
			{
				Quaternion rotation = Quaternion.AngleAxis(base.transform.eulerAngles.y - y, Vector3.up);
				this.m_RigidBody.velocity = rotation * this.m_RigidBody.velocity;
			}
		}

		// Token: 0x06003AE9 RID: 15081 RVA: 0x001C4D94 File Offset: 0x001C2F94
		private void GroundCheck()
		{
			this.m_PreviouslyGrounded = this.m_IsGrounded;
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius, Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.groundCheckDistance))
			{
				this.m_IsGrounded = true;
				this.m_GroundContactNormal = raycastHit.normal;
			}
			else
			{
				this.m_IsGrounded = false;
				this.m_GroundContactNormal = Vector3.up;
			}
			if (!this.m_PreviouslyGrounded && this.m_IsGrounded && this.m_Jumping)
			{
				this.m_Jumping = false;
			}
		}

		// Token: 0x04002AAE RID: 10926
		public Camera cam;

		// Token: 0x04002AAF RID: 10927
		public bool LockCursor;

		// Token: 0x04002AB0 RID: 10928
		[SerializeField]
		public bool lockMovement;

		// Token: 0x04002AB1 RID: 10929
		public MMovementSettings movementSettings = new MMovementSettings();

		// Token: 0x04002AB2 RID: 10930
		public MMouseLook mouseLook = new MMouseLook();

		// Token: 0x04002AB3 RID: 10931
		public MAdvancedSettings advancedSettings = new MAdvancedSettings();

		// Token: 0x04002AB4 RID: 10932
		private Rigidbody m_RigidBody;

		// Token: 0x04002AB5 RID: 10933
		private CapsuleCollider m_Capsule;

		// Token: 0x04002AB6 RID: 10934
		private Vector3 m_GroundContactNormal;

		// Token: 0x04002AB7 RID: 10935
		private bool m_Jump;

		// Token: 0x04002AB8 RID: 10936
		private bool m_PreviouslyGrounded;

		// Token: 0x04002AB9 RID: 10937
		private bool m_Jumping;

		// Token: 0x04002ABA RID: 10938
		private bool m_IsGrounded;
	}
}
