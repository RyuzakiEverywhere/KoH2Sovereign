using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x0200033D RID: 829
	public class HeadBob : MonoBehaviour
	{
		// Token: 0x06003281 RID: 12929 RVA: 0x0019A26F File Offset: 0x0019846F
		private void Start()
		{
			this.motionBob.Setup(this.Camera, this.StrideInterval);
			this.m_OriginalCameraPosition = this.Camera.transform.localPosition;
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x0019A2A0 File Offset: 0x001984A0
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

		// Token: 0x0400220C RID: 8716
		public Camera Camera;

		// Token: 0x0400220D RID: 8717
		public CurveControlledBob motionBob = new CurveControlledBob();

		// Token: 0x0400220E RID: 8718
		public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();

		// Token: 0x0400220F RID: 8719
		public RigidbodyFirstPersonController rigidbodyFirstPersonController;

		// Token: 0x04002210 RID: 8720
		public float StrideInterval;

		// Token: 0x04002211 RID: 8721
		[Range(0f, 1f)]
		public float RunningStrideLengthen;

		// Token: 0x04002212 RID: 8722
		private bool m_PreviouslyGrounded;

		// Token: 0x04002213 RID: 8723
		private Vector3 m_OriginalCameraPosition;
	}
}
