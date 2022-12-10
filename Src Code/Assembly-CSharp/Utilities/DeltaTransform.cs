using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200046D RID: 1133
	[Serializable]
	public struct DeltaTransform
	{
		// Token: 0x06003B63 RID: 15203 RVA: 0x001C6E70 File Offset: 0x001C5070
		public void StoreTransform(Transform transform)
		{
			if (transform == null)
			{
				return;
			}
			this.Position = transform.position;
			this.LocalPosition = transform.localPosition;
			this.EulerAngles = transform.eulerAngles;
			this.Rotation = transform.rotation;
			this.LocalEulerAngles = transform.localEulerAngles;
			this.LocalRotation = transform.localRotation;
			this.lossyScale = transform.lossyScale;
			this.LocalScale = transform.localScale;
		}

		// Token: 0x06003B64 RID: 15204 RVA: 0x001C6EE7 File Offset: 0x001C50E7
		public void RestoreTransform(Transform transform)
		{
			transform.position = this.Position;
			transform.rotation = this.Rotation;
			transform.localScale = this.LocalScale;
		}

		// Token: 0x06003B65 RID: 15205 RVA: 0x001C6F0D File Offset: 0x001C510D
		public void RestoreLocalTransform(Transform transform)
		{
			transform.localPosition = this.LocalPosition;
			transform.localRotation = this.LocalRotation;
			transform.localScale = this.LocalScale;
		}

		// Token: 0x04002B2A RID: 11050
		public Vector3 LocalPosition;

		// Token: 0x04002B2B RID: 11051
		public Vector3 LocalEulerAngles;

		// Token: 0x04002B2C RID: 11052
		public Vector3 Position;

		// Token: 0x04002B2D RID: 11053
		public Vector3 EulerAngles;

		// Token: 0x04002B2E RID: 11054
		public Quaternion Rotation;

		// Token: 0x04002B2F RID: 11055
		public Quaternion LocalRotation;

		// Token: 0x04002B30 RID: 11056
		public Vector3 lossyScale;

		// Token: 0x04002B31 RID: 11057
		public Vector3 LocalScale;
	}
}
