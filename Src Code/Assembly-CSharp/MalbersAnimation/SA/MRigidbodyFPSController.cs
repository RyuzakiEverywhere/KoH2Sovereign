using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x0200045B RID: 1115
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class MRigidbodyFPSController : MonoBehaviour
	{
		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06003AF0 RID: 15088 RVA: 0x001C5008 File Offset: 0x001C3208
		// (set) Token: 0x06003AEF RID: 15087 RVA: 0x001C4FFF File Offset: 0x001C31FF
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

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06003AF1 RID: 15089 RVA: 0x001C5010 File Offset: 0x001C3210
		public Vector3 Velocity
		{
			get
			{
				return this.m_RigidBody.velocity;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06003AF2 RID: 15090 RVA: 0x001C501D File Offset: 0x001C321D
		public bool Grounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06003AF3 RID: 15091 RVA: 0x001C5025 File Offset: 0x001C3225
		public bool Jumping
		{
			get
			{
				return this.m_Jumping;
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06003AF4 RID: 15092 RVA: 0x001C502D File Offset: 0x001C322D
		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		// Token: 0x06003AF5 RID: 15093 RVA: 0x001C503A File Offset: 0x001C323A
		private void Start()
		{
			this.m_RigidBody = base.GetComponent<Rigidbody>();
			this.m_Capsule = base.GetComponent<CapsuleCollider>();
			Cursor.lockState = (this.LockCursor ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !this.LockCursor;
			this.RestartMouseLook();
		}

		// Token: 0x06003AF6 RID: 15094 RVA: 0x001C5079 File Offset: 0x001C3279
		private void Update()
		{
			this.RotateView();
			if (Input.GetButtonDown("Jump") && !this.m_Jump)
			{
				this.m_Jump = true;
			}
		}

		// Token: 0x06003AF7 RID: 15095 RVA: 0x001C509C File Offset: 0x001C329C
		public void RestartMouseLook()
		{
			this.mouseLook.Init(base.transform, this.cam.transform);
		}

		// Token: 0x06003AF8 RID: 15096 RVA: 0x001C50BC File Offset: 0x001C32BC
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

		// Token: 0x06003AF9 RID: 15097 RVA: 0x001C5314 File Offset: 0x001C3514
		private float SlopeMultiplier()
		{
			float time = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(time);
		}

		// Token: 0x06003AFA RID: 15098 RVA: 0x001C5344 File Offset: 0x001C3544
		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius, Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.stickToGroundHelperDistance) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, raycastHit.normal);
			}
		}

		// Token: 0x06003AFB RID: 15099 RVA: 0x001C53E0 File Offset: 0x001C35E0
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

		// Token: 0x06003AFC RID: 15100 RVA: 0x001C5428 File Offset: 0x001C3628
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

		// Token: 0x06003AFD RID: 15101 RVA: 0x001C54C4 File Offset: 0x001C36C4
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

		// Token: 0x04002AC8 RID: 10952
		public Camera cam;

		// Token: 0x04002AC9 RID: 10953
		public bool LockCursor;

		// Token: 0x04002ACA RID: 10954
		[SerializeField]
		public bool lockMovement;

		// Token: 0x04002ACB RID: 10955
		public MMovementSettings movementSettings = new MMovementSettings();

		// Token: 0x04002ACC RID: 10956
		public MMouseLook mouseLook = new MMouseLook();

		// Token: 0x04002ACD RID: 10957
		public MAdvancedSettings advancedSettings = new MAdvancedSettings();

		// Token: 0x04002ACE RID: 10958
		private Rigidbody m_RigidBody;

		// Token: 0x04002ACF RID: 10959
		private CapsuleCollider m_Capsule;

		// Token: 0x04002AD0 RID: 10960
		private Vector3 m_GroundContactNormal;

		// Token: 0x04002AD1 RID: 10961
		private bool m_Jump;

		// Token: 0x04002AD2 RID: 10962
		private bool m_PreviouslyGrounded;

		// Token: 0x04002AD3 RID: 10963
		private bool m_Jumping;

		// Token: 0x04002AD4 RID: 10964
		private bool m_IsGrounded;
	}
}
