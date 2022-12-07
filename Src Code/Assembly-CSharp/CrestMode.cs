using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class CrestMode
{
	// Token: 0x06000C23 RID: 3107 RVA: 0x0008858C File Offset: 0x0008678C
	public void Init()
	{
		this.rt = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
		this.rt.Create();
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x000885B4 File Offset: 0x000867B4
	public override string ToString()
	{
		return this.dir;
	}

	// Token: 0x0400097A RID: 2426
	public int atlas_width;

	// Token: 0x0400097B RID: 2427
	public int atlas_height;

	// Token: 0x0400097C RID: 2428
	public int width;

	// Token: 0x0400097D RID: 2429
	public int height;

	// Token: 0x0400097E RID: 2430
	public string dir;

	// Token: 0x0400097F RID: 2431
	public Material mat;

	// Token: 0x04000980 RID: 2432
	public RenderTexture rt;
}
