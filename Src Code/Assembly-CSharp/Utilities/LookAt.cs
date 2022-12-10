using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000471 RID: 1137
	public class LookAt : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06003B6E RID: 15214 RVA: 0x001C6FF6 File Offset: 0x001C51F6
		// (set) Token: 0x06003B6D RID: 15213 RVA: 0x001C6FED File Offset: 0x001C51ED
		public Vector3 Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06003B6F RID: 15215 RVA: 0x001C6FFE File Offset: 0x001C51FE
		public bool IsAiming
		{
			get
			{
				return this.angle < this.LimitAngle && this.Active && this.AnimationActive && this.hasTarget;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06003B70 RID: 15216 RVA: 0x001C7026 File Offset: 0x001C5226
		// (set) Token: 0x06003B71 RID: 15217 RVA: 0x001C702E File Offset: 0x001C522E
		public RaycastHit AimHit
		{
			get
			{
				return this.aimHit;
			}
			set
			{
				this.aimHit = value;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06003B72 RID: 15218 RVA: 0x001C7037 File Offset: 0x001C5237
		// (set) Token: 0x06003B73 RID: 15219 RVA: 0x001C703F File Offset: 0x001C523F
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x001C7048 File Offset: 0x001C5248
		private void Awake()
		{
			if (Camera.main != null)
			{
				this.cam = Camera.main.transform;
			}
			Animator component = base.GetComponent<Animator>();
			this.AnimatorOnAnimatePhysics = (component && component.updateMode == AnimatorUpdateMode.AnimatePhysics);
			if (this.AnimatorOnAnimatePhysics)
			{
				return;
			}
			foreach (BoneRotation boneRotation in this.Bones)
			{
				boneRotation.initialRotation = boneRotation.bone.transform.localRotation;
			}
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x001C70CC File Offset: 0x001C52CC
		private void LateUpdate()
		{
			foreach (BoneRotation boneRotation in this.Bones)
			{
				boneRotation.initialRotation = boneRotation.bone.transform.localRotation;
			}
			this.LookAtBoneSet();
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x001C710C File Offset: 0x001C530C
		public void EnableLookAt(bool value)
		{
			this.AnimationActive = value;
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x001C7118 File Offset: 0x001C5318
		private void LookAtBoneSet()
		{
			if (!this.Target && !this.cam)
			{
				return;
			}
			this.hasTarget = false;
			if (this.UseCamera || this.Target)
			{
				this.hasTarget = true;
			}
			this.angle = Vector3.Angle(base.transform.forward, this.direction);
			this.currentSmoothness = Mathf.Lerp(this.currentSmoothness, (float)(this.IsAiming ? 1 : 0), Time.deltaTime * this.Smoothness);
			if (this.currentSmoothness > 0.9999f)
			{
				this.currentSmoothness = 1f;
			}
			if (this.currentSmoothness < 0.0001f)
			{
				this.currentSmoothness = 0f;
			}
			for (int i = 0; i < this.Bones.Length; i++)
			{
				BoneRotation boneRotation = this.Bones[i];
				if (boneRotation.bone)
				{
					Vector3 b = base.transform.forward;
					if (this.UseCamera && this.cam)
					{
						b = this.cam.forward;
						this.aimHit = MalbersTools.RayCastHitToCenter(boneRotation.bone, ~this.Ignore);
						if (this.aimHit.collider)
						{
							b = MalbersTools.DirectionTarget(boneRotation.bone.position, this.aimHit.point, true);
						}
					}
					if (this.Target)
					{
						b = MalbersTools.DirectionTarget(boneRotation.bone, this.Target, true);
					}
					this.direction = Vector3.Lerp(this.direction, b, Time.deltaTime * this.Smoothness);
					if (this.currentSmoothness == 0f)
					{
						return;
					}
					if (this.debug && i == this.Bones.Length - 1)
					{
						Debug.DrawRay(boneRotation.bone.position, this.direction * 15f, Color.green);
					}
					Quaternion b2 = Quaternion.LookRotation(this.direction, this.UpVector) * Quaternion.Euler(boneRotation.offset);
					Quaternion rotation = Quaternion.Lerp(boneRotation.bone.rotation, b2, boneRotation.weight * this.currentSmoothness);
					boneRotation.bone.rotation = rotation;
				}
			}
		}

		// Token: 0x06003B78 RID: 15224 RVA: 0x001C7359 File Offset: 0x001C5559
		public virtual void NoTarget()
		{
			this.Target = null;
		}

		// Token: 0x06003B79 RID: 15225 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x06003B7A RID: 15226 RVA: 0x001C7362 File Offset: 0x001C5562
		private void OnDrawGizmos()
		{
			if (Application.isPlaying && this.IsAiming)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(this.aimHit.point, 0.1f);
			}
		}

		// Token: 0x04002B39 RID: 11065
		[Tooltip("Global LookAt Activation")]
		[SerializeField]
		private bool active = true;

		// Token: 0x04002B3A RID: 11066
		[Tooltip("The Animations allows the LookAt to be enable/disabled")]
		public bool AnimationActive = true;

		// Token: 0x04002B3B RID: 11067
		[Space]
		[Tooltip("What layers the Look At Rays should ignore")]
		public LayerMask Ignore = 4;

		// Token: 0x04002B3C RID: 11068
		public bool UseCamera;

		// Token: 0x04002B3D RID: 11069
		public Transform Target;

		// Token: 0x04002B3E RID: 11070
		[Space]
		public float LimitAngle = 80f;

		// Token: 0x04002B3F RID: 11071
		public float Smoothness = 5f;

		// Token: 0x04002B40 RID: 11072
		public Vector3 UpVector = Vector3.up;

		// Token: 0x04002B41 RID: 11073
		private float currentSmoothness;

		// Token: 0x04002B42 RID: 11074
		[Space]
		public BoneRotation[] Bones;

		// Token: 0x04002B43 RID: 11075
		private Transform cam;

		// Token: 0x04002B44 RID: 11076
		protected float angle;

		// Token: 0x04002B45 RID: 11077
		protected Vector3 direction;

		// Token: 0x04002B46 RID: 11078
		public bool debug = true;

		// Token: 0x04002B47 RID: 11079
		private bool hasTarget;

		// Token: 0x04002B48 RID: 11080
		private RaycastHit aimHit;

		// Token: 0x04002B49 RID: 11081
		private bool AnimatorOnAnimatePhysics;
	}
}
