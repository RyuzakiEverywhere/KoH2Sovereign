using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003E7 RID: 999
	public class MFreeLookCamera : MonoBehaviour
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x060037A3 RID: 14243 RVA: 0x001B89B1 File Offset: 0x001B6BB1
		public Transform Target
		{
			get
			{
				return this.m_Target;
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x060037A4 RID: 14244 RVA: 0x001B89B9 File Offset: 0x001B6BB9
		// (set) Token: 0x060037A5 RID: 14245 RVA: 0x001B89C1 File Offset: 0x001B6BC1
		public Transform Cam
		{
			get
			{
				return this.cam;
			}
			set
			{
				this.cam = value;
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x060037A6 RID: 14246 RVA: 0x001B89CA File Offset: 0x001B6BCA
		// (set) Token: 0x060037A7 RID: 14247 RVA: 0x001B89D2 File Offset: 0x001B6BD2
		public Transform Pivot
		{
			get
			{
				return this.pivot;
			}
			set
			{
				this.pivot = value;
			}
		}

		// Token: 0x060037A8 RID: 14248 RVA: 0x001B89DC File Offset: 0x001B6BDC
		protected void Awake()
		{
			this.Cam = base.GetComponentInChildren<Camera>().transform;
			this.Pivot = this.Cam.parent;
			if (this.manager)
			{
				this.manager.SetCamera(this);
			}
			if (this.startState)
			{
				this.SetState(this.startState);
			}
			Cursor.lockState = (this.m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !this.m_LockCursor;
			this.m_PivotEulers = this.Pivot.rotation.eulerAngles;
			this.m_PivotTargetRot = this.Pivot.transform.localRotation;
			this.m_TransformTargetRot = base.transform.localRotation;
			this.inputSystem = DefaultInput.GetInputSystem(this.PlayerID);
			this.Horizontal.InputSystem = (this.Vertical.InputSystem = this.inputSystem);
		}

		// Token: 0x060037A9 RID: 14249 RVA: 0x001B8ACC File Offset: 0x001B6CCC
		public virtual void SetState(FreeLookCameraState profile)
		{
			this.Pivot.localPosition = profile.PivotPos;
			this.Cam.localPosition = profile.CamPos;
			this.Cam.GetComponent<Camera>().fieldOfView = profile.CamFOV;
		}

		// Token: 0x060037AA RID: 14250 RVA: 0x001B8B06 File Offset: 0x001B6D06
		protected void FollowTarget(float deltaTime)
		{
			if (this.m_Target == null)
			{
				return;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, this.m_Target.position, deltaTime * this.m_MoveSpeed);
		}

		// Token: 0x060037AB RID: 14251 RVA: 0x001B8B48 File Offset: 0x001B6D48
		private void HandleRotationMovement()
		{
			if (Time.timeScale < 1E-45f)
			{
				return;
			}
			this.x = this.Horizontal.GetAxis;
			this.y = this.Vertical.GetAxis;
			this.m_LookAngle += this.x * this.m_TurnSpeed;
			this.m_TransformTargetRot = Quaternion.Euler(0f, this.m_LookAngle, 0f);
			this.m_TiltAngle -= this.y * this.m_TurnSpeed;
			this.m_TiltAngle = Mathf.Clamp(this.m_TiltAngle, -this.m_TiltMin, this.m_TiltMax);
			this.m_PivotTargetRot = Quaternion.Euler(this.m_TiltAngle, this.m_PivotEulers.y, this.m_PivotEulers.z);
			if (this.m_TurnSmoothing > 0f)
			{
				this.Pivot.localRotation = Quaternion.Slerp(this.Pivot.localRotation, this.m_PivotTargetRot, this.m_TurnSmoothing * Time.deltaTime);
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, this.m_TransformTargetRot, this.m_TurnSmoothing * Time.deltaTime);
				return;
			}
			this.Pivot.localRotation = this.m_PivotTargetRot;
			base.transform.localRotation = this.m_TransformTargetRot;
		}

		// Token: 0x060037AC RID: 14252 RVA: 0x001B8CA2 File Offset: 0x001B6EA2
		private void Update()
		{
			this.HandleRotationMovement();
			if (this.updateType == MFreeLookCamera.UpdateType.Update)
			{
				this.FollowTarget(Time.deltaTime);
			}
		}

		// Token: 0x060037AD RID: 14253 RVA: 0x001B8CBE File Offset: 0x001B6EBE
		private void FixedUpdate()
		{
			if (this.updateType == MFreeLookCamera.UpdateType.FixedUpdate)
			{
				this.FollowTarget(Time.fixedDeltaTime);
			}
		}

		// Token: 0x060037AE RID: 14254 RVA: 0x001B8CD3 File Offset: 0x001B6ED3
		private void LateUpdate()
		{
			if (this.updateType == MFreeLookCamera.UpdateType.LateUpdate)
			{
				this.FollowTarget(Time.deltaTime);
			}
		}

		// Token: 0x060037AF RID: 14255 RVA: 0x001B8CE9 File Offset: 0x001B6EE9
		public virtual void SetTarget(Transform newTransform)
		{
			this.m_Target = newTransform;
		}

		// Token: 0x060037B0 RID: 14256 RVA: 0x001B8CF2 File Offset: 0x001B6EF2
		public virtual void SetTarget(GameObject newGO)
		{
			this.m_Target = newGO.transform;
		}

		// Token: 0x040027DA RID: 10202
		[HideInInspector]
		public string PlayerID = "Player0";

		// Token: 0x040027DB RID: 10203
		[Space]
		public Transform m_Target;

		// Token: 0x040027DC RID: 10204
		public MFreeLookCamera.UpdateType updateType;

		// Token: 0x040027DD RID: 10205
		private Transform cam;

		// Token: 0x040027DE RID: 10206
		private Transform pivot;

		// Token: 0x040027DF RID: 10207
		public float m_MoveSpeed = 10f;

		// Token: 0x040027E0 RID: 10208
		[Range(0f, 10f)]
		public float m_TurnSpeed = 10f;

		// Token: 0x040027E1 RID: 10209
		public float m_TurnSmoothing = 10f;

		// Token: 0x040027E2 RID: 10210
		public float m_TiltMax = 75f;

		// Token: 0x040027E3 RID: 10211
		public float m_TiltMin = 45f;

		// Token: 0x040027E4 RID: 10212
		public InputAxis Vertical = new InputAxis("Mouse Y", true, false);

		// Token: 0x040027E5 RID: 10213
		public InputAxis Horizontal = new InputAxis("Mouse X", true, false);

		// Token: 0x040027E6 RID: 10214
		[Space]
		public bool m_LockCursor;

		// Token: 0x040027E7 RID: 10215
		[Space]
		public FreeLockCameraManager manager;

		// Token: 0x040027E8 RID: 10216
		public FreeLookCameraState startState;

		// Token: 0x040027E9 RID: 10217
		private float m_LookAngle;

		// Token: 0x040027EA RID: 10218
		private float m_TiltAngle;

		// Token: 0x040027EB RID: 10219
		private const float k_LookDistance = 100f;

		// Token: 0x040027EC RID: 10220
		private Vector3 m_PivotEulers;

		// Token: 0x040027ED RID: 10221
		private Quaternion m_PivotTargetRot;

		// Token: 0x040027EE RID: 10222
		private Quaternion m_TransformTargetRot;

		// Token: 0x040027EF RID: 10223
		private float x;

		// Token: 0x040027F0 RID: 10224
		private float y;

		// Token: 0x040027F1 RID: 10225
		private IInputSystem inputSystem;

		// Token: 0x0200091D RID: 2333
		public enum UpdateType
		{
			// Token: 0x04004279 RID: 17017
			FixedUpdate,
			// Token: 0x0400427A RID: 17018
			LateUpdate,
			// Token: 0x0400427B RID: 17019
			Update
		}
	}
}
