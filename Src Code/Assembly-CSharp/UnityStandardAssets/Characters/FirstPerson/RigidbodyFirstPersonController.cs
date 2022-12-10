using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200033F RID: 831
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class RigidbodyFirstPersonController : MonoBehaviour
	{
		// Token: 0x1700026B RID: 619
		// (get) Token: 0x0600328B RID: 12939 RVA: 0x0019A67B File Offset: 0x0019887B
		public Vector3 Velocity
		{
			get
			{
				return this.m_RigidBody.velocity;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x0600328C RID: 12940 RVA: 0x0019A688 File Offset: 0x00198888
		public bool Grounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x0600328D RID: 12941 RVA: 0x0019A690 File Offset: 0x00198890
		public bool Jumping
		{
			get
			{
				return this.m_Jumping;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600328E RID: 12942 RVA: 0x0019A698 File Offset: 0x00198898
		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		// Token: 0x0600328F RID: 12943 RVA: 0x0019A6A5 File Offset: 0x001988A5
		private void Start()
		{
			this.m_RigidBody = base.GetComponent<Rigidbody>();
			this.m_Capsule = base.GetComponent<CapsuleCollider>();
			this.mouseLook.Init(base.transform, this.cam.transform);
		}

		// Token: 0x06003290 RID: 12944 RVA: 0x0019A6DB File Offset: 0x001988DB
		private void Update()
		{
			this.RotateView();
			if (CrossPlatformInputManager.GetButtonDown("Jump") && !this.m_Jump)
			{
				this.m_Jump = true;
			}
		}

		// Token: 0x06003291 RID: 12945 RVA: 0x0019A700 File Offset: 0x00198900
		private void FixedUpdate()
		{
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

		// Token: 0x06003292 RID: 12946 RVA: 0x0019A950 File Offset: 0x00198B50
		private float SlopeMultiplier()
		{
			float time = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(time);
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x0019A980 File Offset: 0x00198B80
		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.stickToGroundHelperDistance, -1, QueryTriggerInteraction.Ignore) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, raycastHit.normal);
			}
		}

		// Token: 0x06003294 RID: 12948 RVA: 0x0019AA30 File Offset: 0x00198C30
		private Vector2 GetInput()
		{
			Vector2 vector = new Vector2
			{
				x = CrossPlatformInputManager.GetAxis("Horizontal"),
				y = CrossPlatformInputManager.GetAxis("Vertical")
			};
			this.movementSettings.UpdateDesiredTargetSpeed(vector);
			return vector;
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x0019AA78 File Offset: 0x00198C78
		private void RotateView()
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

		// Token: 0x06003296 RID: 12950 RVA: 0x0019AB14 File Offset: 0x00198D14
		private void GroundCheck()
		{
			this.m_PreviouslyGrounded = this.m_IsGrounded;
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.groundCheckDistance, -1, QueryTriggerInteraction.Ignore))
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

		// Token: 0x0400221F RID: 8735
		public Camera cam;

		// Token: 0x04002220 RID: 8736
		public RigidbodyFirstPersonController.MovementSettings movementSettings = new RigidbodyFirstPersonController.MovementSettings();

		// Token: 0x04002221 RID: 8737
		public MouseLook mouseLook = new MouseLook();

		// Token: 0x04002222 RID: 8738
		public RigidbodyFirstPersonController.AdvancedSettings advancedSettings = new RigidbodyFirstPersonController.AdvancedSettings();

		// Token: 0x04002223 RID: 8739
		private Rigidbody m_RigidBody;

		// Token: 0x04002224 RID: 8740
		private CapsuleCollider m_Capsule;

		// Token: 0x04002225 RID: 8741
		private float m_YRotation;

		// Token: 0x04002226 RID: 8742
		private Vector3 m_GroundContactNormal;

		// Token: 0x04002227 RID: 8743
		private bool m_Jump;

		// Token: 0x04002228 RID: 8744
		private bool m_PreviouslyGrounded;

		// Token: 0x04002229 RID: 8745
		private bool m_Jumping;

		// Token: 0x0400222A RID: 8746
		private bool m_IsGrounded;

		// Token: 0x0200088F RID: 2191
		[Serializable]
		public class MovementSettings
		{
			// Token: 0x06005182 RID: 20866 RVA: 0x0023DDEC File Offset: 0x0023BFEC
			public void UpdateDesiredTargetSpeed(Vector2 input)
			{
				if (input == Vector2.zero)
				{
					return;
				}
				if (input.x > 0f || input.x < 0f)
				{
					this.CurrentTargetSpeed = this.StrafeSpeed;
				}
				if (input.y < 0f)
				{
					this.CurrentTargetSpeed = this.BackwardSpeed;
				}
				if (input.y > 0f)
				{
					this.CurrentTargetSpeed = this.ForwardSpeed;
				}
				if (Input.GetKey(this.RunKey))
				{
					this.CurrentTargetSpeed *= this.RunMultiplier;
					this.m_Running = true;
					return;
				}
				this.m_Running = false;
			}

			// Token: 0x1700066C RID: 1644
			// (get) Token: 0x06005183 RID: 20867 RVA: 0x0023DE8E File Offset: 0x0023C08E
			public bool Running
			{
				get
				{
					return this.m_Running;
				}
			}

			// Token: 0x0400401B RID: 16411
			public float ForwardSpeed = 8f;

			// Token: 0x0400401C RID: 16412
			public float BackwardSpeed = 4f;

			// Token: 0x0400401D RID: 16413
			public float StrafeSpeed = 4f;

			// Token: 0x0400401E RID: 16414
			public float RunMultiplier = 2f;

			// Token: 0x0400401F RID: 16415
			public KeyCode RunKey = KeyCode.LeftShift;

			// Token: 0x04004020 RID: 16416
			public float JumpForce = 30f;

			// Token: 0x04004021 RID: 16417
			public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(-90f, 1f),
				new Keyframe(0f, 1f),
				new Keyframe(90f, 0f)
			});

			// Token: 0x04004022 RID: 16418
			[HideInInspector]
			public float CurrentTargetSpeed = 8f;

			// Token: 0x04004023 RID: 16419
			private bool m_Running;
		}

		// Token: 0x02000890 RID: 2192
		[Serializable]
		public class AdvancedSettings
		{
			// Token: 0x04004024 RID: 16420
			public float groundCheckDistance = 0.01f;

			// Token: 0x04004025 RID: 16421
			public float stickToGroundHelperDistance = 0.5f;

			// Token: 0x04004026 RID: 16422
			public float slowDownRate = 20f;

			// Token: 0x04004027 RID: 16423
			public bool airControl;

			// Token: 0x04004028 RID: 16424
			[Tooltip("set it to 0.1 or more if you get stuck in wall")]
			public float shellOffset;
		}
	}
}
