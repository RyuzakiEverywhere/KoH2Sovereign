using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000470 RID: 1136
	[Serializable]
	public class BoneRotation
	{
		// Token: 0x04002B35 RID: 11061
		public Transform bone;

		// Token: 0x04002B36 RID: 11062
		public Vector3 offset = new Vector3(0f, -90f, -90f);

		// Token: 0x04002B37 RID: 11063
		[Range(0f, 1f)]
		public float weight = 1f;

		// Token: 0x04002B38 RID: 11064
		internal Quaternion initialRotation;
	}
}
