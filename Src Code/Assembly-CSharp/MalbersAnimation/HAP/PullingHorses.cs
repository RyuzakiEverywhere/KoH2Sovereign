using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000427 RID: 1063
	public class PullingHorses : MonoBehaviour
	{
		// Token: 0x06003931 RID: 14641 RVA: 0x001BE328 File Offset: 0x001BC528
		private void Start()
		{
			if (!this.RightHorse)
			{
				return;
			}
			if (!this.LeftHorse)
			{
				this.LeftHorse = this.RightHorse;
			}
			this.RHorseInitialPos = this.RightHorse.transform.localPosition;
			this._rigidbody = base.GetComponent<Rigidbody>();
			this.RightHorse.transform.parent = base.transform;
			this.LeftHorse.transform.parent = base.transform;
			this.LeftHorse.GetComponent<Rigidbody>().isKinematic = true;
			this.RightHorse.GetComponent<Rigidbody>().isKinematic = true;
			this.LeftHorse.Anim.applyRootMotion = false;
			this.RightHorse.Anim.applyRootMotion = false;
			switch (this.RightHorse.StartSpeed)
			{
			case Animal.Ground.walk:
				this.CurrentTurnSpeed = this.TurnSpeed1;
				return;
			case Animal.Ground.trot:
				this.CurrentTurnSpeed = this.TurnSpeed2;
				return;
			case Animal.Ground.run:
				this.CurrentTurnSpeed = this.TurnSpeed3;
				return;
			default:
				return;
			}
		}

		// Token: 0x06003932 RID: 14642 RVA: 0x001BE438 File Offset: 0x001BC638
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (!this.RightHorse)
			{
				return;
			}
			if (this.RightHorse.Speed1)
			{
				this.CurrentTurnSpeed = this.TurnSpeed1;
			}
			else if (this.RightHorse.Speed2)
			{
				this.CurrentTurnSpeed = this.TurnSpeed2;
			}
			else if (this.RightHorse.Speed3)
			{
				this.CurrentTurnSpeed = this.TurnSpeed3;
			}
			this.LeftHorse.Anim.applyRootMotion = false;
			this.RightHorse.Anim.applyRootMotion = false;
			if (this.RightHorse.Speed == 0f)
			{
				this.RightHorse.MovementAxis = new Vector3(this.RightHorse.MovementAxis.x * 2f, this.RightHorse.MovementAxis.y, this.RightHorse.MovementAxis.z);
				if (this.CanRotateInPlace)
				{
					base.transform.RotateAround(this.RotationPivot.position, Vector3.up, this.RightHorse.MovementAxis.x * fixedDeltaTime * this.TurnSpeed0);
				}
				else if ((this.CurrentAngleSide && this.RightHorse.MovementAxis.x < 0f) || (!this.CurrentAngleSide && this.RightHorse.MovementAxis.x > 0f))
				{
					this.RightHorse.MovementAxis = (this.LeftHorse.MovementAxis = Vector3.zero);
				}
				this.PullingDirection = Vector3.Lerp(this._rigidbody.velocity, Vector3.zero, fixedDeltaTime * 25f);
				this._rigidbody.velocity = Vector3.Lerp(this._rigidbody.velocity, Vector3.zero, fixedDeltaTime * 5f);
			}
			else
			{
				base.transform.RotateAround(this.RotationPivot.position, Vector3.up, this.RightHorse.MovementAxis.x * fixedDeltaTime * this.CurrentTurnSpeed);
				this.PullingDirection = Vector3.Lerp(this.PullingDirection, base.transform.forward * this.RightHorse.Anim.velocity.magnitude * (float)((this.RightHorse.Speed >= 0f) ? 1 : -1), fixedDeltaTime * 15f);
				this._rigidbody.velocity = this.PullingDirection;
			}
			base.transform.position = new Vector3(base.transform.position.x, (this.RightHorse.transform.position.y + this.LeftHorse.transform.position.y) / 2f, base.transform.position.z);
			if (this.RightHorse)
			{
				this.RightHorse.transform.rotation = base.transform.rotation;
				this.RightHorse.transform.rotation = Quaternion.FromToRotation(this.RightHorse.transform.up, this.RightHorse.SurfaceNormal) * this.RightHorse.transform.rotation;
			}
			if (this.LeftHorse)
			{
				this.LeftHorse.transform.rotation = base.transform.rotation;
				this.LeftHorse.transform.rotation = Quaternion.FromToRotation(this.LeftHorse.transform.up, this.LeftHorse.SurfaceNormal) * this.LeftHorse.transform.rotation;
			}
			if (this.LeftHorse && this.LeftHorse != this.RightHorse)
			{
				this.LeftHorse.MovementAxis = this.RightHorse.MovementAxis;
				this.LeftHorse.GroundSpeed = this.RightHorse.GroundSpeed;
				this.LeftHorse.SetIntID(this.RightHorse.IDInt);
			}
			this.RightHorse.transform.localPosition = new Vector3(this.RHorseInitialPos.x, this.RightHorse.transform.localPosition.y, this.RHorseInitialPos.z);
		}

		// Token: 0x04002951 RID: 10577
		[Header("Horses")]
		public Animal RightHorse;

		// Token: 0x04002952 RID: 10578
		public Animal LeftHorse;

		// Token: 0x04002953 RID: 10579
		[Header("Turn Speed")]
		public float TurnSpeed0 = 10f;

		// Token: 0x04002954 RID: 10580
		public float TurnSpeed1 = 25f;

		// Token: 0x04002955 RID: 10581
		public float TurnSpeed2 = 25f;

		// Token: 0x04002956 RID: 10582
		public float TurnSpeed3 = 35f;

		// Token: 0x04002957 RID: 10583
		[HideInInspector]
		public float CurrentTurnSpeed = 25f;

		// Token: 0x04002958 RID: 10584
		protected Rigidbody _rigidbody;

		// Token: 0x04002959 RID: 10585
		[HideInInspector]
		public Vector3 PullingDirection;

		// Token: 0x0400295A RID: 10586
		[HideInInspector]
		public bool CurrentAngleSide;

		// Token: 0x0400295B RID: 10587
		[HideInInspector]
		public bool CanRotateInPlace;

		// Token: 0x0400295C RID: 10588
		public Transform RotationPivot;

		// Token: 0x0400295D RID: 10589
		private Vector3 RHorseInitialPos;

		// Token: 0x0400295E RID: 10590
		private Vector3 LHorseInitialPos;
	}
}
