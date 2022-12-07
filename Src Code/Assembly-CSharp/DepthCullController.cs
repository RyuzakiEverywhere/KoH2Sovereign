using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000324 RID: 804
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthCullController : MonoBehaviour
{
	// Token: 0x06003209 RID: 12809 RVA: 0x00195C60 File Offset: 0x00193E60
	private void OnEnable()
	{
		Camera component = base.GetComponent<Camera>();
		component.depthTextureMode |= DepthTextureMode.Depth;
		component.depthTextureMode |= DepthTextureMode.DepthNormals;
		this.CMD3();
	}

	// Token: 0x0600320A RID: 12810 RVA: 0x000023FD File Offset: 0x000005FD
	private void Update()
	{
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x00195C8C File Offset: 0x00193E8C
	private void CopyDepth()
	{
		this.cmdBuffer = new CommandBuffer();
		this.cmdBuffer.name = "commandBufferStencil";
		this.bufferCopyID = Shader.PropertyToID(this._name);
		Camera component = base.GetComponent<Camera>();
		RenderTextureFormat format = component.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
		switch (this.ttype)
		{
		case BuiltinRenderTextureType.Depth:
			format = RenderTextureFormat.Depth;
			break;
		case BuiltinRenderTextureType.DepthNormals:
			format = RenderTextureFormat.ARGB32;
			break;
		case BuiltinRenderTextureType.ResolvedDepth:
			format = RenderTextureFormat.Depth;
			break;
		case BuiltinRenderTextureType.GBuffer2:
			format = RenderTextureFormat.ARGB2101010;
			break;
		case BuiltinRenderTextureType.GBuffer3:
			format = RenderTextureFormat.ARGBHalf;
			break;
		}
		this.cmdBuffer.GetTemporaryRT(this.bufferCopyID, -1, -1, 24, FilterMode.Point, format);
		if (this.rend != null)
		{
			this.cmdBuffer.SetRenderTarget(this.bufferCopyID);
			this.cmdBuffer.ClearRenderTarget(true, true, Color.clear, 1f);
			this.cmdBuffer.DrawRenderer(this.rend, this.mat);
		}
		this.cmdBuffer.Blit(this.ttype, this.bufferCopyID);
		this.cmdBuffer.SetGlobalTexture(this._name, this.bufferCopyID);
		component.AddCommandBuffer(this.e, this.cmdBuffer);
		this.cmdBuffer.ReleaseTemporaryRT(this.bufferCopyID);
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnPreRender()
	{
	}

	// Token: 0x0600320D RID: 12813 RVA: 0x00195DF4 File Offset: 0x00193FF4
	private void CMD3()
	{
		this.cmdBuffer = new CommandBuffer();
		this.cmdBuffer.name = "commandBufferStencil";
		this.bufferCopyID = Shader.PropertyToID("_Depth2");
		this.cmdBuffer.GetTemporaryRT(this.bufferCopyID, -1, -1, 24);
		this.cmdBuffer.SetRenderTarget(this.bufferCopyID, BuiltinRenderTextureType.CurrentActive);
		this.cmdBuffer.ClearRenderTarget(true, true, Color.clear);
		Camera component = base.GetComponent<Camera>();
		Matrix4x4 localToWorldMatrix = this.rend.transform.localToWorldMatrix;
		Mesh sharedMesh = this.rend.GetComponent<MeshFilter>().sharedMesh;
		this.cmdBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.rend.sharedMaterial, 0, 0);
		this.cmdBuffer.SetGlobalTexture("_Depth2", this.bufferCopyID);
		component.AddCommandBuffer(this.e, this.cmdBuffer);
		this.cmdBuffer.ReleaseTemporaryRT(this.bufferCopyID);
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x00195EF0 File Offset: 0x001940F0
	private void CMD2()
	{
		Camera component = base.GetComponent<Camera>();
		if (this.cmdBuffer == null)
		{
			this.cmdBuffer = new CommandBuffer();
			this.cmdBuffer.name = "DEFERRED CUSTOM";
			component.AddCommandBuffer(CameraEvent.AfterGBuffer, this.cmdBuffer);
		}
		int nameID = Shader.PropertyToID("_NormalsCopy");
		this.cmdBuffer.GetTemporaryRT(nameID, -1, -1);
		this.cmdBuffer.Blit(BuiltinRenderTextureType.GBuffer2, nameID);
		int nameID2 = Shader.PropertyToID("_DepthCopy");
		this.cmdBuffer.GetTemporaryRT(nameID2, -1, -1, 24);
		this.cmdBuffer.Blit(BuiltinRenderTextureType.Depth, nameID2);
		RenderTargetIdentifier[] colors = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.GBuffer1,
			BuiltinRenderTextureType.GBuffer2,
			BuiltinRenderTextureType.GBuffer3
		};
		this.cmdBuffer.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
		this.cmdBuffer.DrawMesh(this.mesh, base.transform.localToWorldMatrix, this.mat);
		this.cmdBuffer.SetGlobalTexture("_AAADepth", nameID2);
		this.cmdBuffer.SetGlobalTexture("_AAANormals", nameID);
		this.cmdBuffer.ReleaseTemporaryRT(nameID2);
		this.cmdBuffer.ReleaseTemporaryRT(nameID);
	}

	// Token: 0x0600320F RID: 12815 RVA: 0x00196050 File Offset: 0x00194250
	private void CMD()
	{
		this.cmdBuffer = new CommandBuffer();
		this.cmdBuffer.name = "commandBufferStencil";
		this.bufferCopyID = Shader.PropertyToID("_StencilBufferCopy");
		this.rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		this.cmdBuffer.SetRenderTarget(this.rt.colorBuffer, BuiltinRenderTextureType.CameraTarget);
		Shader shader = Shader.Find("StencilToBlackAndWhite");
		this.mat = new Material(shader);
		Camera component = base.GetComponent<Camera>();
		Matrix4x4 localToWorldMatrix = this.rend.transform.localToWorldMatrix;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		this.cmdBuffer.DrawMesh(mesh, localToWorldMatrix, this.mat);
		this.cmdBuffer.SetGlobalTexture("_UnitsOutline", this.rt);
		component.AddCommandBuffer(this.e, this.cmdBuffer);
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x00196148 File Offset: 0x00194348
	private void BuildCommandBuffer()
	{
		Camera component = base.GetComponent<Camera>();
		if (this.cmdBuffer == null)
		{
			this.cmdBuffer = new CommandBuffer
			{
				name = "StencilBufferCopy"
			};
		}
		this.bufferCopyID = Shader.PropertyToID("_BufferCopy");
		this.cmdBuffer.GetTemporaryRT(this.bufferCopyID, -1, -1, 24);
		Material material = new Material(Shader.Find("Hidden/BlitDepth"));
		material.hideFlags = HideFlags.HideAndDontSave;
		this.cmdBuffer.Blit(BuiltinRenderTextureType.CameraTarget, this.bufferCopyID, material);
		this.cmdBuffer.SetGlobalTexture("_UnitsOutline", this.bufferCopyID);
		this.cmdBuffer.Blit(this.bufferCopyID, BuiltinRenderTextureType.CameraTarget);
		this.cmdBuffer.ReleaseTemporaryRT(this.bufferCopyID);
		component.AddCommandBuffer(this.e, this.cmdBuffer);
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x0019622C File Offset: 0x0019442C
	private void OnDisable()
	{
		Object.DestroyImmediate(this.rt);
		base.GetComponent<Camera>().RemoveAllCommandBuffers();
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x00196244 File Offset: 0x00194444
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		this.rt = RenderTexture.GetTemporary(src.width, src.height, 24);
		Graphics.Blit(src, dest, this.mat2);
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x0019626C File Offset: 0x0019446C
	private void OnPostRender()
	{
		RenderTexture.ReleaseTemporary(this.rt);
	}

	// Token: 0x04002172 RID: 8562
	public CommandBuffer cmdBuffer;

	// Token: 0x04002173 RID: 8563
	public Renderer rend;

	// Token: 0x04002174 RID: 8564
	private CommandBuffer cmdBuffer2;

	// Token: 0x04002175 RID: 8565
	public Mesh mesh;

	// Token: 0x04002176 RID: 8566
	public Material mat;

	// Token: 0x04002177 RID: 8567
	public Material mat2;

	// Token: 0x04002178 RID: 8568
	public RenderTexture rt;

	// Token: 0x04002179 RID: 8569
	public CameraEvent e;

	// Token: 0x0400217A RID: 8570
	public BuiltinRenderTextureType ttype;

	// Token: 0x0400217B RID: 8571
	public int bufferCopyID;

	// Token: 0x0400217C RID: 8572
	public string _name;
}
