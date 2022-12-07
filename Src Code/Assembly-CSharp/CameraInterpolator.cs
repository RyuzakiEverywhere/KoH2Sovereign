using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
[Serializable]
public class CameraInterpolator
{
	// Token: 0x04000898 RID: 2200
	public bool expanded = true;

	// Token: 0x04000899 RID: 2201
	public bool enabled = true;

	// Token: 0x0400089A RID: 2202
	public GameObject obj;

	// Token: 0x0400089B RID: 2203
	public string obj_name = "";

	// Token: 0x0400089C RID: 2204
	public string component = "";

	// Token: 0x0400089D RID: 2205
	public string variable = "";

	// Token: 0x0400089E RID: 2206
	public float min;

	// Token: 0x0400089F RID: 2207
	public float max;
}
