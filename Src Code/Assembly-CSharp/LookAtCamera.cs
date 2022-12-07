using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class LookAtCamera : MonoBehaviour
{
	// Token: 0x0600007E RID: 126 RVA: 0x00004BB2 File Offset: 0x00002DB2
	public void Start()
	{
		if (this.lookAtCamera == null)
		{
			this.lookAtCamera = Camera.main;
		}
		if (this.lookOnlyOnAwake)
		{
			this.LookCam();
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004BDB File Offset: 0x00002DDB
	public void Update()
	{
		if (!this.lookOnlyOnAwake)
		{
			this.LookCam();
		}
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004BEB File Offset: 0x00002DEB
	public void LookCam()
	{
		base.transform.LookAt(this.lookAtCamera.transform);
	}

	// Token: 0x040000AC RID: 172
	public Camera lookAtCamera;

	// Token: 0x040000AD RID: 173
	public bool lookOnlyOnAwake;
}
