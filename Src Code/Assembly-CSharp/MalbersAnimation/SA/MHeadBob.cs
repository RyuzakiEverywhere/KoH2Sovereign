using System;
using UnityEngine;

namespace MalbersAnimations.SA
{
	// Token: 0x02000456 RID: 1110
	public class MHeadBob : MonoBehaviour
	{
		// Token: 0x06003AD4 RID: 15060 RVA: 0x001C455F File Offset: 0x001C275F
		private void Start()
		{
			this.motionBob.Setup(this.Camera, this.StrideInterval);
			this.m_OriginalCameraPosition = this.Camera.transform.localPosition;
		}

		// Token: 0x06003AD5 RID: 15061 RVA: 0x001C4590 File Offset: 0x001C2790
		private void Update()
		{
			Vector3 localPosition;
			if (this.rigidbodyFirstPersonController.Velocity.magnitude > 0f && this.rigidbodyFirstPersonController.Grounded)
			{
				this.Camera.transform.localPosition = this.motionBob.DoHeadBob(this.rigidbodyFirstPersonController.Velocity.magnitude * (this.rigidbodyFirstPersonController.Running ? this.RunningStrideLengthen : 1f));
				localPosition = this.Camera.transform.localPosition;
				localPosition.y = this.Camera.transform.localPosition.y - this.jumpAndLandingBob.Offset();
			}
			else
			{
				localPosition = this.Camera.transform.localPosition;
				localPosition.y = this.m_OriginalCameraPosition.y - this.jumpAndLandingBob.Offset();
			}
			this.Camera.transform.localPosition = localPosition;
			if (!this.m_PreviouslyGrounded && this.rigidbodyFirstPersonController.Grounded)
			{
				base.StartCoroutine(this.jumpAndLandingBob.DoBobCycle());
			}
			this.m_PreviouslyGrounded = this.rigidbodyFirstPersonController.Grounded;
		}

		// Token: 0x04002A9D RID: 10909
		public Camera Camera;

		// Token: 0x04002A9E RID: 10910
		public MCurveControlledBob motionBob = new MCurveControlledBob();

		// Token: 0x04002A9F RID: 10911
		public MLerpControlledBob jumpAndLandingBob = new MLerpControlledBob();

		// Token: 0x04002AA0 RID: 10912
		public MRigidbodyFPSController rigidbodyFirstPersonController;

		// Token: 0x04002AA1 RID: 10913
		public float StrideInterval;

		// Token: 0x04002AA2 RID: 10914
		[Range(0f, 1f)]
		public float RunningStrideLengthen;

		// Token: 0x04002AA3 RID: 10915
		private bool m_PreviouslyGrounded;

		// Token: 0x04002AA4 RID: 10916
		private Vector3 m_OriginalCameraPosition;
	}
}
