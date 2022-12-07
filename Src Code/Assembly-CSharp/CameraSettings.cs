using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
[Serializable]
public class CameraSettings
{
	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000B50 RID: 2896 RVA: 0x0008090D File Offset: 0x0007EB0D
	public float GetScrollSpeed
	{
		get
		{
			return this.scrollSpeed * UserSettings.PanSpeed;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0008091B File Offset: 0x0007EB1B
	public float GetScrollSpeedFar
	{
		get
		{
			return this.scrollSpeedFar * UserSettings.PanSpeed;
		}
	}

	// Token: 0x040008B5 RID: 2229
	public float scrollSpeed = 6f;

	// Token: 0x040008B6 RID: 2230
	public float scrollSpeedFar = -1f;

	// Token: 0x040008B7 RID: 2231
	public float zoomSpeed = 2000f;

	// Token: 0x040008B8 RID: 2232
	public float lookAtHeight = 15f;

	// Token: 0x040008B9 RID: 2233
	public float minHeight = 25f;

	// Token: 0x040008BA RID: 2234
	public Vector2 dist = new Vector2(10f, 65f);

	// Token: 0x040008BB RID: 2235
	public float initialDist = 65f;

	// Token: 0x040008BC RID: 2236
	public Vector2 pitch = new Vector2(20f, 55f);

	// Token: 0x040008BD RID: 2237
	public Vector2 farPlane = new Vector2(10000f, 10000f);

	// Token: 0x040008BE RID: 2238
	public bool hasBounds = true;

	// Token: 0x040008BF RID: 2239
	public bool snapToTerrain;
}
