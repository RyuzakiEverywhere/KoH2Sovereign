using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000064 RID: 100
[RequireComponent(typeof(Camera))]
public class SSRT : MonoBehaviour
{
	// Token: 0x06000252 RID: 594 RVA: 0x00021A48 File Offset: 0x0001FC48
	private void GenerateCommandBuffers()
	{
		this.ssrtBuffer.Clear();
		this.storeAmbientBuffer.Clear();
		this.clearBuffer.Clear();
		this.clearBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
		this.clearBuffer.ClearRenderTarget(false, true, Color.black);
		this.storeAmbientBuffer.Blit(BuiltinRenderTextureType.CameraTarget, this.ambientTexture);
		this.storeAmbientBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
		this.storeAmbientBuffer.ClearRenderTarget(false, true, Color.black);
		int nameID = Shader.PropertyToID("_CameraTexture");
		this.ssrtBuffer.GetTemporaryRT(nameID, (int)this.renderResolution.x, (int)this.renderResolution.y, 0, FilterMode.Point, RenderTextureFormat.DefaultHDR);
		this.ssrtBuffer.Blit(BuiltinRenderTextureType.CameraTarget, nameID);
		int nameID2 = Shader.PropertyToID("_LightmaskTexture");
		this.ssrtBuffer.GetTemporaryRT(nameID2, (int)((SSRT.ResolutionDownscale)this.renderResolution.x / this.lightBufferResolution), (int)((SSRT.ResolutionDownscale)this.renderResolution.y / this.lightBufferResolution), 0, FilterMode.Point, this.lightBufferHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32);
		this.ssrtBuffer.SetRenderTarget(nameID2);
		this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 10);
		RenderTargetIdentifier[] colors = new RenderTargetIdentifier[]
		{
			this.ssrtMrt[0],
			this.ssrtMrt[1]
		};
		this.ssrtBuffer.SetRenderTarget(colors, this.ssrtMrt[1]);
		this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 0);
		int nameID3 = Shader.PropertyToID("_FilterTexture1");
		this.ssrtBuffer.GetTemporaryRT(nameID3, (int)this.renderResolution.x, (int)this.renderResolution.y, 0, FilterMode.Point, RenderTextureFormat.DefaultHDR);
		int nameID4 = Shader.PropertyToID("_FilterTexture2");
		this.ssrtBuffer.GetTemporaryRT(nameID4, (int)this.renderResolution.x, (int)this.renderResolution.y, 0, FilterMode.Point, RenderTextureFormat.DefaultHDR);
		if (this.resolutionDownscale != SSRT.ResolutionDownscale.Full)
		{
			int nameID5 = Shader.PropertyToID("_CurrentDepth");
			this.ssrtBuffer.GetTemporaryRT(nameID5, (int)((SSRT.ResolutionDownscale)this.renderResolution.x / this.resolutionDownscale), (int)((SSRT.ResolutionDownscale)this.renderResolution.y / this.resolutionDownscale), 0, FilterMode.Point, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
			int nameID6 = Shader.PropertyToID("_CurrentNormal");
			this.ssrtBuffer.GetTemporaryRT(nameID6, (int)((SSRT.ResolutionDownscale)this.renderResolution.x / this.resolutionDownscale), (int)((SSRT.ResolutionDownscale)this.renderResolution.y / this.resolutionDownscale), 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
			this.ssrtBuffer.SetRenderTarget(nameID5);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 8);
			this.ssrtBuffer.SetRenderTarget(nameID6);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 9);
			this.ssrtBuffer.SetRenderTarget(nameID3);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 1);
		}
		else
		{
			this.ssrtBuffer.Blit(this.ssrtMrt[1], nameID3);
		}
		if (this.reuseCount > 1)
		{
			this.ssrtBuffer.SetRenderTarget(nameID4);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 2);
			this.ssrtBuffer.CopyTexture(nameID4, nameID3);
		}
		if (this.temporalEnabled)
		{
			this.ssrtBuffer.SetRenderTarget(nameID4);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 3);
			this.ssrtBuffer.Blit(nameID4, this.previousFrameTexture);
			this.ssrtBuffer.SetRenderTarget(this.previousDepthTexture);
			this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, 8);
		}
		else
		{
			this.ssrtBuffer.Blit(nameID3, nameID4);
		}
		this.ssrtBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
		this.ssrtBuffer.DrawMesh(this.mesh, Matrix4x4.identity, this.ssrtMaterial, 0, (int)this.debugMode);
		this.lastFrameViewProjectionMatrix = this.viewProjectionMatrix;
		this.lastFrameInverseViewProjectionMatrix = this.viewProjectionMatrix.inverse;
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00021EF4 File Offset: 0x000200F4
	private void UpdateVariables()
	{
		Matrix4x4 worldToCameraMatrix = this.cam.worldToCameraMatrix;
		this.ssrtMaterial.SetMatrix("_CameraToWorldMatrix", worldToCameraMatrix.inverse);
		Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(this.cam.projectionMatrix, false);
		this.ssrtMaterial.SetMatrix("_InverseProjectionMatrix", gpuprojectionMatrix.inverse);
		this.viewProjectionMatrix = gpuprojectionMatrix * worldToCameraMatrix;
		this.ssrtMaterial.SetMatrix("_InverseViewProjectionMatrix", this.viewProjectionMatrix.inverse);
		this.ssrtMaterial.SetMatrix("_LastFrameViewProjectionMatrix", this.lastFrameViewProjectionMatrix);
		this.ssrtMaterial.SetMatrix("_LastFrameInverseViewProjectionMatrix", this.lastFrameInverseViewProjectionMatrix);
		this.ssrtMaterial.SetInt("_RotationCount", this.rotationCount);
		this.ssrtMaterial.SetInt("_StepCount", this.stepCount);
		this.ssrtMaterial.SetFloat("_GIBoost", this.GIBoost);
		this.ssrtMaterial.SetFloat("_LnDlOffset", this.LnDlOffset);
		this.ssrtMaterial.SetFloat("_NDlOffset", this.nDlOffset);
		this.ssrtMaterial.SetFloat("_Radius", this.radius);
		this.ssrtMaterial.SetFloat("_ExpStart", this.expStart);
		this.ssrtMaterial.SetFloat("_ExpFactor", this.expFactor);
		this.ssrtMaterial.SetFloat("_Thickness", this.thickness);
		this.ssrtMaterial.SetFloat("_Falloff", this.falloff);
		this.ssrtMaterial.SetFloat("_Power", this.power);
		this.ssrtMaterial.SetFloat("_TemporalResponse", this.temporalResponse);
		this.ssrtMaterial.SetInt("_MultiBounceAO", this.multiBounceAO ? 1 : 0);
		this.ssrtMaterial.SetFloat("_DirectLightingAO", (float)(this.directLightingAO ? 1 : 0));
		this.ssrtMaterial.SetTexture("_CubemapFallback", this.cubemapFallback);
		this.ssrtMaterial.SetInt("_FallbackMethod", (int)this.fallbackMethod);
		this.ssrtMaterial.SetInt("_LightOnly", this.lightOnly ? 1 : 0);
		this.ssrtMaterial.SetInt("_ReuseCount", this.reuseCount);
		this.ssrtMaterial.SetInt("_JitterSamples", this.jitterSamples ? 1 : 0);
		float value = this.renderResolution.y / (Mathf.Tan(this.cam.fieldOfView * 0.017453292f * 0.5f) * 2f) * 0.5f;
		this.ssrtMaterial.SetFloat("_HalfProjScale", value);
		this.ssrtMaterial.SetInt("_ResolutionDownscale", (int)this.resolutionDownscale);
		float num = SSRT.temporalRotations[Time.frameCount % 6];
		float value2 = SSRT.spatialOffsets[Time.frameCount % ((this.resolutionDownscale == SSRT.ResolutionDownscale.Full) ? 4 : 2)];
		this.ssrtMaterial.SetFloat("_TemporalDirections", num / 360f);
		this.ssrtMaterial.SetFloat("_TemporalOffsets", value2);
		if (this.cameraSize != this.renderResolution / (float)this.resolutionDownscale)
		{
			this.cameraSize = this.renderResolution / (float)this.resolutionDownscale;
			if (this.ssrtMrt[0] != null)
			{
				this.ssrtMrt[0].Release();
			}
			this.ssrtMrt[0] = new RenderTexture((int)((SSRT.ResolutionDownscale)this.renderResolution.x / this.resolutionDownscale), (int)((SSRT.ResolutionDownscale)this.renderResolution.y / this.resolutionDownscale), 0, RenderTextureFormat.ARGBHalf);
			this.ssrtMrt[0].filterMode = FilterMode.Point;
			this.ssrtMrt[0].Create();
			if (this.ssrtMrt[1] != null)
			{
				this.ssrtMrt[1].Release();
			}
			this.ssrtMrt[1] = new RenderTexture((int)((SSRT.ResolutionDownscale)this.renderResolution.x / this.resolutionDownscale), (int)((SSRT.ResolutionDownscale)this.renderResolution.y / this.resolutionDownscale), 0, RenderTextureFormat.DefaultHDR);
			this.ssrtMrt[1].filterMode = FilterMode.Point;
			this.ssrtMrt[1].Create();
			if (this.ambientTexture != null)
			{
				this.ambientTexture.Release();
			}
			this.ambientTexture = new RenderTexture((int)this.renderResolution.x, (int)this.renderResolution.y, 0, RenderTextureFormat.DefaultHDR);
			if (this.previousFrameTexture != null)
			{
				this.previousFrameTexture.Release();
			}
			this.previousFrameTexture = new RenderTexture((int)this.renderResolution.x, (int)this.renderResolution.y, 0, RenderTextureFormat.DefaultHDR);
			this.previousFrameTexture.filterMode = FilterMode.Point;
			this.previousFrameTexture.Create();
			if (this.previousDepthTexture != null)
			{
				this.previousDepthTexture.Release();
			}
			this.previousDepthTexture = new RenderTexture((int)this.renderResolution.x, (int)this.renderResolution.y, 0, RenderTextureFormat.RFloat);
			this.previousDepthTexture.filterMode = FilterMode.Point;
			this.previousDepthTexture.Create();
		}
		this.ssrtMaterial.SetTexture("_BentNormalTexture", this.ssrtMrt[0]);
		this.ssrtMaterial.SetTexture("_GIOcclusionTexture", this.ssrtMrt[1]);
		this.ssrtMaterial.SetTexture("_AmbientTexture", this.ambientTexture);
		this.ssrtMaterial.SetTexture("_PreviousColor", this.previousFrameTexture);
		this.ssrtMaterial.SetTexture("_PreviousDepth", this.previousDepthTexture);
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00022484 File Offset: 0x00020684
	private void RenderCubemap()
	{
		if (this.cubemapCamera == null)
		{
			GameObject gameObject = new GameObject("CubemapCamera", new Type[]
			{
				typeof(Camera)
			});
			gameObject.transform.SetParent(this.cam.transform);
			this.cubemapCamera = gameObject.GetComponent<Camera>();
			this.cubemapCamera.CopyFrom(this.cam);
			this.cubemapCamera.enabled = false;
			this.cubemapCamera.renderingPath = RenderingPath.Forward;
		}
		if (this.cubemapFallback)
		{
			this.cubemapCamera.RenderToCubemap(this.cubemapFallback, 1 << Time.frameCount % 6);
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00022534 File Offset: 0x00020734
	private void Awake()
	{
		this.cam = base.gameObject.GetComponent<Camera>();
		this.cam.depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.MotionVectors);
		this.ssrtMaterial = new Material(Shader.Find("Hidden/SSRT"));
		this.mesh = new Mesh();
		this.mesh.vertices = new Vector3[]
		{
			new Vector3(-1f, -1f, 1f),
			new Vector3(-1f, 1f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, -1f, 1f)
		};
		this.mesh.uv = new Vector2[]
		{
			new Vector2(0f, 1f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f)
		};
		this.mesh.SetIndices(new int[]
		{
			0,
			1,
			2,
			3
		}, MeshTopology.Quads, 0);
		if (this.fallbackMethod == SSRT.FallbackMethod.DynamicCubemap)
		{
			this.cubemapFallback = new Cubemap(32, TextureFormat.RGB24, true);
			this.cubemapFallback.Apply(true);
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x000226B0 File Offset: 0x000208B0
	private void OnPreRender()
	{
		this.renderResolution = new Vector2((float)this.cam.pixelWidth, (float)this.cam.pixelHeight);
		if (this.ssrtBuffer != null)
		{
			if (this.fallbackMethod == SSRT.FallbackMethod.DynamicCubemap && Application.isPlaying)
			{
				this.RenderCubemap();
			}
			this.UpdateVariables();
			this.GenerateCommandBuffers();
		}
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0002270C File Offset: 0x0002090C
	private void OnEnable()
	{
		this.ssrtBuffer = new CommandBuffer();
		this.ssrtBuffer.name = "SSRT";
		this.storeAmbientBuffer = new CommandBuffer();
		this.storeAmbientBuffer.name = "StoreAmbient";
		this.clearBuffer = new CommandBuffer();
		this.clearBuffer.name = "ClearBuffer";
		this.cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.ssrtBuffer);
		this.cam.AddCommandBuffer(CameraEvent.BeforeLighting, this.storeAmbientBuffer);
		this.cam.AddCommandBuffer(CameraEvent.BeforeGBuffer, this.clearBuffer);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x000227A4 File Offset: 0x000209A4
	private void OnDisable()
	{
		if (this.ssrtBuffer != null)
		{
			this.cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.ssrtBuffer);
			this.ssrtBuffer = null;
		}
		if (this.storeAmbientBuffer != null)
		{
			this.cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.storeAmbientBuffer);
			this.storeAmbientBuffer = null;
		}
		if (this.clearBuffer != null)
		{
			this.cam.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, this.clearBuffer);
			this.clearBuffer = null;
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00022818 File Offset: 0x00020A18
	private void OnDestroy()
	{
		if (this.ssrtMrt[0] != null)
		{
			this.ssrtMrt[0].Release();
			this.ssrtMrt[0] = null;
		}
		if (this.ssrtMrt[1] != null)
		{
			this.ssrtMrt[1].Release();
			this.ssrtMrt[1] = null;
		}
		if (this.ambientTexture != null)
		{
			this.ambientTexture.Release();
			this.ambientTexture = null;
		}
		if (this.previousFrameTexture != null)
		{
			this.previousFrameTexture.Release();
			this.previousFrameTexture = null;
		}
		if (this.previousDepthTexture != null)
		{
			this.previousDepthTexture.Release();
			this.previousDepthTexture = null;
		}
		if (this.ssrtBuffer != null)
		{
			this.ssrtBuffer.Dispose();
			this.ssrtBuffer = null;
		}
		if (this.storeAmbientBuffer != null)
		{
			this.storeAmbientBuffer.Dispose();
			this.storeAmbientBuffer = null;
		}
		if (this.clearBuffer != null)
		{
			this.clearBuffer.Dispose();
			this.clearBuffer = null;
		}
	}

	// Token: 0x0400037D RID: 893
	public readonly string version = "1.0.0";

	// Token: 0x0400037E RID: 894
	[Header("Sampling")]
	[Tooltip("Number of directionnal rotations applied during sampling.")]
	[Range(1f, 4f)]
	public int rotationCount = 4;

	// Token: 0x0400037F RID: 895
	[Tooltip("Number of samples taken along one edge of the current conic slice.")]
	[Range(1f, 16f)]
	public int stepCount = 8;

	// Token: 0x04000380 RID: 896
	[Tooltip("Effective sampling radius in world space. AO and GI can only have influence within that radius.")]
	[Range(1f, 25f)]
	public float radius = 3.5f;

	// Token: 0x04000381 RID: 897
	[Tooltip("Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.")]
	[Range(0.1f, 1f)]
	public float expStart = 1f;

	// Token: 0x04000382 RID: 898
	[Tooltip("Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.")]
	[Range(1f, 2f)]
	public float expFactor = 1f;

	// Token: 0x04000383 RID: 899
	[Tooltip("Applies some noise on sample positions to hide the banding artifacts that can occur when there is undersampling.")]
	public bool jitterSamples = true;

	// Token: 0x04000384 RID: 900
	[Header("GI")]
	[Tooltip("Intensity of the indirect diffuse light.")]
	[Range(0f, 75f)]
	public float GIBoost = 20f;

	// Token: 0x04000385 RID: 901
	[Tooltip("Using an HDR light buffer gives more accurate lighting but have an impact on performances.")]
	public bool lightBufferHDR;

	// Token: 0x04000386 RID: 902
	[Tooltip("Using lower resolution light buffer can help performances but can accentuate aliasing.")]
	public SSRT.ResolutionDownscale lightBufferResolution = SSRT.ResolutionDownscale.Half;

	// Token: 0x04000387 RID: 903
	[Tooltip("Bypass the dot(lightNormal, lightDirection) weighting.")]
	[Range(0f, 1f)]
	public float LnDlOffset;

	// Token: 0x04000388 RID: 904
	[Tooltip("Bypass the dot(normal, lightDirection) weighting.")]
	[Range(0f, 1f)]
	public float nDlOffset;

	// Token: 0x04000389 RID: 905
	[Header("Occlusion")]
	[Tooltip("Power function applied to AO to make it appear darker/lighter.")]
	[Range(1f, 8f)]
	public float power = 1.5f;

	// Token: 0x0400038A RID: 906
	[Tooltip("Constant thickness value of objects on the screen in world space. Is used to ignore occlusion past that thickness level, as if light can travel behind the object.")]
	[Range(0.1f, 10f)]
	public float thickness = 10f;

	// Token: 0x0400038B RID: 907
	[Tooltip("Occlusion falloff relative to distance.")]
	[Range(1f, 50f)]
	public float falloff = 1f;

	// Token: 0x0400038C RID: 908
	[Tooltip("Multi-Bounce analytic approximation from GTAO.")]
	public bool multiBounceAO;

	// Token: 0x0400038D RID: 909
	[Tooltip("Composite AO also on direct lighting.")]
	public bool directLightingAO;

	// Token: 0x0400038E RID: 910
	[Header("Offscreen Fallback")]
	[Tooltip("Ambient lighting to use. Off uses the Unity ambient lighting, but it's possible to use instead a static irradiance cubemap (pre-convolved), or render a cubemap around camera every frame (expensive).")]
	public SSRT.FallbackMethod fallbackMethod;

	// Token: 0x0400038F RID: 911
	[Tooltip("Static irradiance cubemap to use if it's the chosen fallback.")]
	public Cubemap cubemapFallback;

	// Token: 0x04000390 RID: 912
	[Header("Filters")]
	[Tooltip("The resolution at which SSRT is computed. If lower than fullscreen the effect will be upscaled to fullscreen afterwards. Lower resolution can help performances but can also introduce more flickering/aliasing.")]
	public SSRT.ResolutionDownscale resolutionDownscale = SSRT.ResolutionDownscale.Half;

	// Token: 0x04000391 RID: 913
	[Tooltip("Number of neighbor pixel to reuse (helps reduce noise).")]
	[Range(1f, 8f)]
	public int reuseCount = 5;

	// Token: 0x04000392 RID: 914
	[Tooltip("Enable/Disable temporal reprojection")]
	public bool temporalEnabled = true;

	// Token: 0x04000393 RID: 915
	[Tooltip("Controls the speed of the accumulation, slower accumulation is more effective at removing noise but can introduce ghosting.")]
	[Range(0f, 1f)]
	public float temporalResponse = 0.35f;

	// Token: 0x04000394 RID: 916
	[Header("Debug Mode")]
	[Tooltip("View of the different SSRT buffers for debug purposes.")]
	public SSRT.DebugMode debugMode = SSRT.DebugMode.Combined;

	// Token: 0x04000395 RID: 917
	[Tooltip("If enabled will show only the radiance that affects the surface, if unchecked radiance will be multiplied by surface albedo.")]
	public bool lightOnly;

	// Token: 0x04000396 RID: 918
	private Camera cam;

	// Token: 0x04000397 RID: 919
	private Camera cubemapCamera;

	// Token: 0x04000398 RID: 920
	private Material ssrtMaterial;

	// Token: 0x04000399 RID: 921
	private CommandBuffer ssrtBuffer;

	// Token: 0x0400039A RID: 922
	private CommandBuffer storeAmbientBuffer;

	// Token: 0x0400039B RID: 923
	private CommandBuffer clearBuffer;

	// Token: 0x0400039C RID: 924
	private Mesh mesh;

	// Token: 0x0400039D RID: 925
	private Matrix4x4 lastFrameViewProjectionMatrix;

	// Token: 0x0400039E RID: 926
	private Matrix4x4 viewProjectionMatrix;

	// Token: 0x0400039F RID: 927
	private Matrix4x4 lastFrameInverseViewProjectionMatrix;

	// Token: 0x040003A0 RID: 928
	private Vector2 cameraSize;

	// Token: 0x040003A1 RID: 929
	private Vector2 renderResolution;

	// Token: 0x040003A2 RID: 930
	private RenderTexture ambientTexture;

	// Token: 0x040003A3 RID: 931
	private RenderTexture previousFrameTexture;

	// Token: 0x040003A4 RID: 932
	private RenderTexture previousDepthTexture;

	// Token: 0x040003A5 RID: 933
	private RenderTexture[] ssrtMrt = new RenderTexture[2];

	// Token: 0x040003A6 RID: 934
	private static readonly float[] temporalRotations = new float[]
	{
		60f,
		300f,
		180f,
		240f,
		120f,
		0f
	};

	// Token: 0x040003A7 RID: 935
	private static readonly float[] spatialOffsets = new float[]
	{
		0f,
		0.5f,
		0.25f,
		0.75f
	};

	// Token: 0x02000516 RID: 1302
	public enum DebugMode
	{
		// Token: 0x04002ED9 RID: 11993
		AO = 4,
		// Token: 0x04002EDA RID: 11994
		BentNormal,
		// Token: 0x04002EDB RID: 11995
		GI,
		// Token: 0x04002EDC RID: 11996
		Combined
	}

	// Token: 0x02000517 RID: 1303
	public enum FallbackMethod
	{
		// Token: 0x04002EDE RID: 11998
		Off,
		// Token: 0x04002EDF RID: 11999
		StaticIrradianceCubemap,
		// Token: 0x04002EE0 RID: 12000
		DynamicCubemap
	}

	// Token: 0x02000518 RID: 1304
	public enum ResolutionDownscale
	{
		// Token: 0x04002EE2 RID: 12002
		Full = 1,
		// Token: 0x04002EE3 RID: 12003
		Half,
		// Token: 0x04002EE4 RID: 12004
		Quarter = 4,
		// Token: 0x04002EE5 RID: 12005
		Eight = 8
	}

	// Token: 0x02000519 RID: 1305
	public enum RenderPass
	{
		// Token: 0x04002EE7 RID: 12007
		SSRT,
		// Token: 0x04002EE8 RID: 12008
		Upsample,
		// Token: 0x04002EE9 RID: 12009
		SampleReuse,
		// Token: 0x04002EEA RID: 12010
		TemporalReproj,
		// Token: 0x04002EEB RID: 12011
		DebugModeAO,
		// Token: 0x04002EEC RID: 12012
		DebugModeBentNormal,
		// Token: 0x04002EED RID: 12013
		DebugModeGI,
		// Token: 0x04002EEE RID: 12014
		DebugModeCombined,
		// Token: 0x04002EEF RID: 12015
		GetDepth,
		// Token: 0x04002EF0 RID: 12016
		GetNormal,
		// Token: 0x04002EF1 RID: 12017
		GetLightmask,
		// Token: 0x04002EF2 RID: 12018
		CopyLightmask
	}
}
