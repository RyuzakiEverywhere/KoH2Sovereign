using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000428 RID: 1064
	public class WagonController : MonoBehaviour
	{
		// Token: 0x06003934 RID: 14644 RVA: 0x001BE8CC File Offset: 0x001BCACC
		private void Start()
		{
			this._rigidbody = base.GetComponent<Rigidbody>();
			if (this.HorseRigidBody)
			{
				this.DHorses = this.HorseRigidBody.transform.GetComponent<PullingHorses>();
			}
			if (this.Body && this.BodyCollider)
			{
				this.Body.parent = this.BodyCollider;
			}
			if (this.StearMesh && this.StearCollider)
			{
				this.StearMesh.parent = this.StearCollider;
			}
			if (this.HorseRigidBody && this.HorseJoint)
			{
				this.HorseJoint.connectedBody = this.HorseRigidBody;
			}
		}

		// Token: 0x06003935 RID: 14645 RVA: 0x001BE98C File Offset: 0x001BCB8C
		private void UpdateWheelMeshes()
		{
			for (int i = 0; i < this.WheelColliders.Length; i++)
			{
				if (this.WheelColliders[i])
				{
					if (this.DHorses)
					{
						this.StopWheels(this.DHorses.RightHorse.Stand, i);
					}
					Vector3 position;
					Quaternion rotation;
					this.WheelColliders[i].GetWorldPose(out position, out rotation);
					this.WheelMeshes[i].position = position;
					this.WheelMeshes[i].rotation = rotation;
				}
			}
		}

		// Token: 0x06003936 RID: 14646 RVA: 0x001BEA0C File Offset: 0x001BCC0C
		private void StopWheels(bool stop, int Index)
		{
			if (stop)
			{
				this.WheelColliders[Index].brakeTorque = 1f;
				return;
			}
			this.WheelColliders[Index].brakeTorque = 0f;
		}

		// Token: 0x06003937 RID: 14647 RVA: 0x001BEA36 File Offset: 0x001BCC36
		private void Update()
		{
			this.UpdateWheelMeshes();
			this.GetStearAngle();
		}

		// Token: 0x06003938 RID: 14648 RVA: 0x001BEA44 File Offset: 0x001BCC44
		protected virtual void GetStearAngle()
		{
			if (!this.DHorses)
			{
				return;
			}
			if (!this.StearCollider || !this.BodyCollider)
			{
				return;
			}
			Vector3 forward = this.BodyCollider.forward;
			Vector3 forward2 = this.StearCollider.forward;
			forward.y = (forward2.y = 0f);
			this.currentAngle = Vector3.Angle(forward, forward2);
			float num = Vector3.Dot(forward, this.StearCollider.right);
			this.currentAngle *= (float)((num > 0f) ? 1 : -1);
			this.DHorses.CurrentAngleSide = (num > 0f);
			if (this.DHorses)
			{
				if ((this.currentAngle >= this.MaxTurnAngle && this.DHorses.RightHorse.MovementAxis.x <= 0f) || (this.currentAngle <= -this.MaxTurnAngle && this.DHorses.RightHorse.MovementAxis.x >= 0f))
				{
					this.DHorses.CanRotateInPlace = false;
				}
				else
				{
					this.DHorses.CanRotateInPlace = true;
				}
				if (this._rigidbody.velocity.magnitude < 0.01f)
				{
					this._rigidbody.velocity = this.DHorses.PullingDirection;
				}
			}
		}

		// Token: 0x0400295F RID: 10591
		[Header("Horse")]
		public Rigidbody HorseRigidBody;

		// Token: 0x04002960 RID: 10592
		public ConfigurableJoint HorseJoint;

		// Token: 0x04002961 RID: 10593
		public float MaxTurnAngle = 45f;

		// Token: 0x04002962 RID: 10594
		protected PullingHorses DHorses;

		// Token: 0x04002963 RID: 10595
		[Header("Colliders")]
		public Transform BodyCollider;

		// Token: 0x04002964 RID: 10596
		public Transform StearCollider;

		// Token: 0x04002965 RID: 10597
		[Space]
		public WheelCollider[] WheelColliders;

		// Token: 0x04002966 RID: 10598
		[Space]
		[Header("Meshes")]
		public Transform Body;

		// Token: 0x04002967 RID: 10599
		public Transform StearMesh;

		// Token: 0x04002968 RID: 10600
		public Transform[] WheelMeshes;

		// Token: 0x04002969 RID: 10601
		protected Rigidbody _rigidbody;

		// Token: 0x0400296A RID: 10602
		protected float currentAngle;

		// Token: 0x0400296B RID: 10603
		[Space]
		public bool debug;

		// Token: 0x0400296C RID: 10604
		public Color DebugColor;
	}
}
