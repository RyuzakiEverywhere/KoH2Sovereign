using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
	// Token: 0x06000490 RID: 1168 RVA: 0x00035DD5 File Offset: 0x00033FD5
	private void OnEnable()
	{
		this.Recalc();
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00035DD5 File Offset: 0x00033FD5
	private void LateUpdate()
	{
		this.Recalc();
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00035DE0 File Offset: 0x00033FE0
	private void Recalc()
	{
		if (this.parent_object != null)
		{
			base.transform.position = this.parent_object.TransformPoint(this.offset);
			base.transform.rotation = this.parent_object.rotation;
			return;
		}
		if (this.cam == null || !this.cam.gameObject.activeInHierarchy)
		{
			this.cam = CameraController.MainCamera;
		}
		if (this.cam == null)
		{
			return;
		}
		if (!this.YawOnly)
		{
			base.transform.rotation = this.cam.transform.rotation;
			return;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.y = this.cam.transform.eulerAngles.y;
		base.transform.eulerAngles = eulerAngles;
	}

	// Token: 0x04000477 RID: 1143
	public bool YawOnly;

	// Token: 0x04000478 RID: 1144
	private Camera cam;

	// Token: 0x04000479 RID: 1145
	[NonSerialized]
	public Vector3 offset;

	// Token: 0x0400047A RID: 1146
	[NonSerialized]
	public Transform parent_object;
}
