using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000323 RID: 803
[ExecuteInEditMode]
public class CommandBufferStencil : MonoBehaviour
{
	// Token: 0x06003206 RID: 12806 RVA: 0x00195B4C File Offset: 0x00193D4C
	private void OnEnable()
	{
		if (this.cmdBuffer == null)
		{
			this.cmdBuffer = new CommandBuffer();
			this.cmdBuffer.name = "cmdBuffer";
			this.bufferId = Shader.PropertyToID("_Depth2");
			this.cmdBuffer.GetTemporaryRT(this.bufferId, this.bufferId - 1, -1, 24, FilterMode.Point, RenderTextureFormat.ARGB32);
			RenderTexture active = RenderTexture.active;
			this.cmdBuffer.SetRenderTarget(this.bufferId);
			this.cmdBuffer.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.None, this.stencil_is_on_mat);
			this.cmdBuffer.SetGlobalTexture("_Depth2", this.bufferId);
			this.cmdBuffer.SetRenderTarget(active);
			this.cmdBuffer.ReleaseTemporaryRT(this.bufferId);
			Camera.main.AddCommandBuffer(this.e, this.cmdBuffer);
		}
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x00195C39 File Offset: 0x00193E39
	private void OnDisable()
	{
		if (this.cmdBuffer != null)
		{
			Camera.main.RemoveCommandBuffer(this.e, this.cmdBuffer);
		}
		this.cmdBuffer = null;
	}

	// Token: 0x0400216E RID: 8558
	private CommandBuffer cmdBuffer;

	// Token: 0x0400216F RID: 8559
	public Material stencil_is_on_mat;

	// Token: 0x04002170 RID: 8560
	private int bufferId;

	// Token: 0x04002171 RID: 8561
	public CameraEvent e;
}
