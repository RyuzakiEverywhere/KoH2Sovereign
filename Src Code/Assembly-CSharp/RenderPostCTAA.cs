using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class RenderPostCTAA : MonoBehaviour
{
	// Token: 0x060001F0 RID: 496 RVA: 0x0001F53C File Offset: 0x0001D73C
	private void LateUpdate()
	{
		base.transform.position = this.ctaaCamTransform.position;
		base.transform.rotation = this.ctaaCamTransform.rotation;
		this.MaskRenderCam.transform.position = this.ctaaCamTransform.position;
		this.MaskRenderCam.transform.rotation = this.ctaaCamTransform.rotation;
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0001F5AB File Offset: 0x0001D7AB
	private void OnDisable()
	{
		if (this.maskTexRT != null)
		{
			Object.DestroyImmediate(this.maskTexRT);
		}
		this.maskTexRT = null;
		if (this.MaskRenderCam != null)
		{
			this.MaskRenderCam.targetTexture = null;
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0001F5E8 File Offset: 0x0001D7E8
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.maskTexRT == null || this.maskTexRT.width != source.width || this.maskTexRT.height != source.height)
		{
			Object.DestroyImmediate(this.maskTexRT);
			this.maskTexRT = new RenderTexture(source.width, source.height, 16, source.format);
			this.maskTexRT.hideFlags = HideFlags.HideAndDontSave;
			this.maskTexRT.filterMode = FilterMode.Bilinear;
			this.maskTexRT.wrapMode = TextureWrapMode.Repeat;
			this.MaskRenderCam.targetTexture = this.maskTexRT;
		}
		if (this.layerMaskingEnabled)
		{
			this.MaskRenderCam.RenderWithShader(this.maskRenderShader, "");
		}
		RenderTexture ctaa_Render = this.ctaaPC.getCTAA_Render();
		if (!(ctaa_Render != null))
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.layerPostMat.SetTexture("_CTAA_RENDER", ctaa_Render);
		this.layerPostMat.SetTexture("_maskTexRT", this.maskTexRT);
		if (this.layerMaskingEnabled)
		{
			Graphics.Blit(source, destination, this.layerPostMat);
			return;
		}
		Graphics.Blit(ctaa_Render, destination);
	}

	// Token: 0x04000308 RID: 776
	public CTAA_PC ctaaPC;

	// Token: 0x04000309 RID: 777
	public Transform ctaaCamTransform;

	// Token: 0x0400030A RID: 778
	public Camera MaskRenderCam;

	// Token: 0x0400030B RID: 779
	public Shader maskRenderShader;

	// Token: 0x0400030C RID: 780
	public RenderTexture maskTexRT;

	// Token: 0x0400030D RID: 781
	public bool layerMaskingEnabled;

	// Token: 0x0400030E RID: 782
	public Material layerPostMat;
}
