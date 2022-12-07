using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
[ExecuteInEditMode]
public class PostProcessExample : MonoBehaviour
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002381 File Offset: 0x00000581
	private void Awake()
	{
		if (this.PostProcessMat == null)
		{
			base.enabled = false;
			return;
		}
		this.PostProcessMat.mainTexture = this.PostProcessMat.mainTexture;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000023AF File Offset: 0x000005AF
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, this.PostProcessMat);
	}

	// Token: 0x0400000C RID: 12
	public Material PostProcessMat;
}
