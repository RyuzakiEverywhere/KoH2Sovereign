using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003FC RID: 1020
	public class LookAtCamera : MonoBehaviour
	{
		// Token: 0x0600384B RID: 14411 RVA: 0x001BB737 File Offset: 0x001B9937
		private void Start()
		{
			this.cam = Camera.main.transform;
		}

		// Token: 0x0600384C RID: 14412 RVA: 0x001BB74C File Offset: 0x001B994C
		private void Update()
		{
			Vector3 forward = this.cam.position - base.transform.position;
			forward.y = 0f;
			Quaternion quaternion = Quaternion.LookRotation(forward);
			base.transform.eulerAngles = new Vector3(this.justY ? 0f : quaternion.eulerAngles.x, quaternion.eulerAngles.y, 0f) + this.Offset;
		}

		// Token: 0x04002857 RID: 10327
		public bool justY = true;

		// Token: 0x04002858 RID: 10328
		public Vector3 Offset;

		// Token: 0x04002859 RID: 10329
		private Transform cam;
	}
}
