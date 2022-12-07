using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class BillBoard : MonoBehaviour
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600006B RID: 107 RVA: 0x00004780 File Offset: 0x00002980
	private Camera cam
	{
		get
		{
			return CameraController.MainCamera;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004787 File Offset: 0x00002987
	private void LateUpdate()
	{
		if (this.cam != null)
		{
			base.transform.rotation = this.cam.transform.rotation;
		}
	}
}
