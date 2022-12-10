using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200033C RID: 828
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(AudioSource))]
	public class FirstPersonController : MonoBehaviour
	{
		// Token: 0x06003275 RID: 12917 RVA: 0x00199B98 File Offset: 0x00197D98
		private void Start()
		{
			this.m_CharacterController = base.GetComponent<CharacterController>();
			this.m_Camera = Camera.main;
			this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
			this.m_FovKick.Setup(this.m_Camera);
			this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
			this.m_StepCycle = 0f;
			this.m_NextStep = this.m_StepCycle / 2f;
			this.m_Jumping = false;
			this.m_AudioSource = base.GetComponent<AudioSource>();
			this.m_MouseLook.Init(base.transform, this.m_Camera.transform);
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x00199C48 File Offset: 0x00197E48
		private void Update()
		{
			this.RotateView();
			if (!this.m_Jump)
			{
				this.m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			}
			if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
			{
				base.StartCoroutine(this.m_JumpBob.DoBobCycle());
				this.PlayLandingSound();
				this.m_MoveDir.y = 0f;
				this.m_Jumping = false;
			}
			if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
			{
				this.m_MoveDir.y = 0f;
			}
			this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
		}

		// Token: 0x06003277 RID: 12919 RVA: 0x00199CF5 File Offset: 0x00197EF5
		private void PlayLandingSound()
		{
			this.m_AudioSource.clip = this.m_LandSound;
			this.m_AudioSource.Play();
			this.m_NextStep = this.m_StepCycle + 0.5f;
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x00199D28 File Offset: 0x00197F28
		private void FixedUpdate()
		{
			float num;
			this.GetInput(out num);
			Vector3 vector = base.transform.forward * this.m_Input.y + base.transform.right * this.m_Input.x;
			RaycastHit raycastHit;
			Physics.SphereCast(base.transform.position, this.m_CharacterController.radius, Vector3.down, out raycastHit, this.m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
			vector = Vector3.ProjectOnPlane(vector, raycastHit.normal).normalized;
			this.m_MoveDir.x = vector.x * num;
			this.m_MoveDir.z = vector.z * num;
			if (this.m_CharacterController.isGrounded)
			{
				this.m_MoveDir.y = -this.m_StickToGroundForce;
				if (this.m_Jump)
				{
					this.m_MoveDir.y = this.m_JumpSpeed;
					this.PlayJumpSound();
					this.m_Jump = false;
					this.m_Jumping = true;
				}
			}
			else
			{
				this.m_MoveDir += Physics.gravity * this.m_GravityMultiplier * Time.fixedDeltaTime;
			}
			this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir * Time.fixedDeltaTime);
			this.ProgressStepCycle(num);
			this.UpdateCameraPosition(num);
			this.m_MouseLook.UpdateCursorLock();
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x00199E9E File Offset: 0x0019809E
		private void PlayJumpSound()
		{
			this.m_AudioSource.clip = this.m_JumpSound;
			this.m_AudioSource.Play();
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x00199EBC File Offset: 0x001980BC
		private void ProgressStepCycle(float speed)
		{
			if (this.m_CharacterController.velocity.sqrMagnitude > 0f && (this.m_Input.x != 0f || this.m_Input.y != 0f))
			{
				this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten)) * Time.fixedDeltaTime;
			}
			if (this.m_StepCycle <= this.m_NextStep)
			{
				return;
			}
			this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
			this.PlayFootStepAudio();
		}

		// Token: 0x0600327B RID: 12923 RVA: 0x00199F70 File Offset: 0x00198170
		private void PlayFootStepAudio()
		{
			if (!this.m_CharacterController.isGrounded)
			{
				return;
			}
			int num = Random.Range(1, this.m_FootstepSounds.Length);
			this.m_AudioSource.clip = this.m_FootstepSounds[num];
			this.m_AudioSource.PlayOneShot(this.m_AudioSource.clip);
			this.m_FootstepSounds[num] = this.m_FootstepSounds[0];
			this.m_FootstepSounds[0] = this.m_AudioSource.clip;
		}

		// Token: 0x0600327C RID: 12924 RVA: 0x00199FE8 File Offset: 0x001981E8
		private void UpdateCameraPosition(float speed)
		{
			if (!this.m_UseHeadBob)
			{
				return;
			}
			Vector3 localPosition;
			if (this.m_CharacterController.velocity.magnitude > 0f && this.m_CharacterController.isGrounded)
			{
				this.m_Camera.transform.localPosition = this.m_HeadBob.DoHeadBob(this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten));
				localPosition = this.m_Camera.transform.localPosition;
				localPosition.y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset();
			}
			else
			{
				localPosition = this.m_Camera.transform.localPosition;
				localPosition.y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset();
			}
			this.m_Camera.transform.localPosition = localPosition;
		}

		// Token: 0x0600327D RID: 12925 RVA: 0x0019A0EC File Offset: 0x001982EC
		private void GetInput(out float speed)
		{
			float axis = CrossPlatformInputManager.GetAxis("Horizontal");
			float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
			bool isWalking = this.m_IsWalking;
			this.m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
			speed = (this.m_IsWalking ? this.m_WalkSpeed : this.m_RunSpeed);
			this.m_Input = new Vector2(axis, axis2);
			if (this.m_Input.sqrMagnitude > 1f)
			{
				this.m_Input.Normalize();
			}
			if (this.m_IsWalking != isWalking && this.m_UseFovKick && this.m_CharacterController.velocity.sqrMagnitude > 0f)
			{
				base.StopAllCoroutines();
				base.StartCoroutine((!this.m_IsWalking) ? this.m_FovKick.FOVKickUp() : this.m_FovKick.FOVKickDown());
			}
		}

		// Token: 0x0600327E RID: 12926 RVA: 0x0019A1C3 File Offset: 0x001983C3
		private void RotateView()
		{
			this.m_MouseLook.LookRotation(base.transform, this.m_Camera.transform);
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x0019A1E4 File Offset: 0x001983E4
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (this.m_CollisionFlags == CollisionFlags.Below)
			{
				return;
			}
			if (attachedRigidbody == null || attachedRigidbody.isKinematic)
			{
				return;
			}
			attachedRigidbody.AddForceAtPosition(this.m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
		}

		// Token: 0x040021EE RID: 8686
		[SerializeField]
		private bool m_IsWalking;

		// Token: 0x040021EF RID: 8687
		[SerializeField]
		private float m_WalkSpeed;

		// Token: 0x040021F0 RID: 8688
		[SerializeField]
		private float m_RunSpeed;

		// Token: 0x040021F1 RID: 8689
		[SerializeField]
		[Range(0f, 1f)]
		private float m_RunstepLenghten;

		// Token: 0x040021F2 RID: 8690
		[SerializeField]
		private float m_JumpSpeed;

		// Token: 0x040021F3 RID: 8691
		[SerializeField]
		private float m_StickToGroundForce;

		// Token: 0x040021F4 RID: 8692
		[SerializeField]
		private float m_GravityMultiplier;

		// Token: 0x040021F5 RID: 8693
		[SerializeField]
		private MouseLook m_MouseLook;

		// Token: 0x040021F6 RID: 8694
		[SerializeField]
		private bool m_UseFovKick;

		// Token: 0x040021F7 RID: 8695
		[SerializeField]
		private FOVKick m_FovKick = new FOVKick();

		// Token: 0x040021F8 RID: 8696
		[SerializeField]
		private bool m_UseHeadBob;

		// Token: 0x040021F9 RID: 8697
		[SerializeField]
		private CurveControlledBob m_HeadBob = new CurveControlledBob();

		// Token: 0x040021FA RID: 8698
		[SerializeField]
		private LerpControlledBob m_JumpBob = new LerpControlledBob();

		// Token: 0x040021FB RID: 8699
		[SerializeField]
		private float m_StepInterval;

		// Token: 0x040021FC RID: 8700
		[SerializeField]
		private AudioClip[] m_FootstepSounds;

		// Token: 0x040021FD RID: 8701
		[SerializeField]
		private AudioClip m_JumpSound;

		// Token: 0x040021FE RID: 8702
		[SerializeField]
		private AudioClip m_LandSound;

		// Token: 0x040021FF RID: 8703
		private Camera m_Camera;

		// Token: 0x04002200 RID: 8704
		private bool m_Jump;

		// Token: 0x04002201 RID: 8705
		private float m_YRotation;

		// Token: 0x04002202 RID: 8706
		private Vector2 m_Input;

		// Token: 0x04002203 RID: 8707
		private Vector3 m_MoveDir = Vector3.zero;

		// Token: 0x04002204 RID: 8708
		private CharacterController m_CharacterController;

		// Token: 0x04002205 RID: 8709
		private CollisionFlags m_CollisionFlags;

		// Token: 0x04002206 RID: 8710
		private bool m_PreviouslyGrounded;

		// Token: 0x04002207 RID: 8711
		private Vector3 m_OriginalCameraPosition;

		// Token: 0x04002208 RID: 8712
		private float m_StepCycle;

		// Token: 0x04002209 RID: 8713
		private float m_NextStep;

		// Token: 0x0400220A RID: 8714
		private bool m_Jumping;

		// Token: 0x0400220B RID: 8715
		private AudioSource m_AudioSource;
	}
}
