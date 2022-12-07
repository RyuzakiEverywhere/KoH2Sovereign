using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/LIVENDA/CTAA_PC")]
public class CTAA_PC : MonoBehaviour
{
	// Token: 0x060001E1 RID: 481 RVA: 0x0001E57C File Offset: 0x0001C77C
	private void SetCTAA_Parameters()
	{
		this.PreEnhanceEnabled = ((double)this.AdaptiveSharpness > 0.01);
		this.preEnhanceStrength = Mathf.Lerp(0.2f, 2f, this.AdaptiveSharpness);
		this.preEnhanceClamp = Mathf.Lerp(0.005f, 0.12f, this.AdaptiveSharpness);
		this.jitterScale = this.TemporalJitterScale;
		this.AdaptiveResolve = 3000f;
		this.ctaaMat.SetFloat("_AntiShimmer", this.AntiShimmerMode ? 1f : 0f);
		this.ctaaMat.SetVector("_delValues", this.delValues);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0001E62C File Offset: 0x0001C82C
	private static Material CreateMaterial(string shadername)
	{
		if (string.IsNullOrEmpty(shadername))
		{
			return null;
		}
		return new Material(Shader.Find(shadername))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0001E64B File Offset: 0x0001C84B
	private static void DestroyMaterial(Material mat)
	{
		if (mat != null)
		{
			Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0001E660 File Offset: 0x0001C860
	private void Awake()
	{
		if (this.ctaaMat == null)
		{
			this.ctaaMat = CTAA_PC.CreateMaterial("Hidden/CTAA_PC");
		}
		if (this.mat_enhance == null)
		{
			this.mat_enhance = CTAA_PC.CreateMaterial("Hidden/CTAA_Enhance_PC");
		}
		this.firstFrame = true;
		this.swap = true;
		this.frameCounter = 0;
		this.SetCTAA_Parameters();
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0001E6C4 File Offset: 0x0001C8C4
	private void OnEnable()
	{
		if (this.ctaaMat == null)
		{
			this.ctaaMat = CTAA_PC.CreateMaterial("Hidden/CTAA_PC");
		}
		if (this.mat_enhance == null)
		{
			this.mat_enhance = CTAA_PC.CreateMaterial("Hidden/CTAA_Enhance_PC");
		}
		this.firstFrame = true;
		this.swap = true;
		this.frameCounter = 0;
		this.SetCTAA_Parameters();
		Camera component = base.GetComponent<Camera>();
		component.depthTextureMode |= DepthTextureMode.Depth;
		component.depthTextureMode |= DepthTextureMode.MotionVectors;
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0001E749 File Offset: 0x0001C949
	public void ResetCTAA_CAM()
	{
		this.count = 0;
		this.moveActive = true;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0001E75C File Offset: 0x0001C95C
	private void LateUpdate()
	{
		if (this.moveActive)
		{
			if (this.count < 2)
			{
				base.transform.position += new Vector3(0f, 1f * this.speed, 0f);
				this.count++;
				return;
			}
			if (this.count < 4)
			{
				base.transform.position -= new Vector3(0f, 1f * this.speed, 0f);
				this.count++;
				return;
			}
			this.moveActive = false;
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0001E810 File Offset: 0x0001CA10
	private void OnDisable()
	{
		if (this.ctaaMat != null)
		{
			CTAA_PC.DestroyMaterial(this.ctaaMat);
		}
		if (this.mat_enhance != null)
		{
			CTAA_PC.DestroyMaterial(this.mat_enhance);
		}
		if (this.rtAccum0 != null)
		{
			Object.DestroyImmediate(this.rtAccum0);
		}
		this.rtAccum0 = null;
		if (this.rtAccum1 != null)
		{
			Object.DestroyImmediate(this.rtAccum1);
		}
		this.rtAccum1 = null;
		if (this.afterPreEnhace != null)
		{
			Object.DestroyImmediate(this.afterPreEnhace);
		}
		this.afterPreEnhace = null;
		base.GetComponent<Camera>().targetTexture = null;
		if (this.upScaleRT != null)
		{
			Object.DestroyImmediate(this.upScaleRT);
		}
		this.upScaleRT = null;
		if (this.m_LayerRenderCam != null)
		{
			this.m_LayerRenderCam.targetTexture = null;
			Object.Destroy(this.m_LayerRenderCam.gameObject);
		}
		if (this.m_LayerMaskCam != null)
		{
			this.m_LayerMaskCam.targetTexture = null;
			Object.Destroy(this.m_LayerMaskCam.gameObject);
		}
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0001E930 File Offset: 0x0001CB30
	private void OnDestroy()
	{
		if (this.ctaaMat != null)
		{
			CTAA_PC.DestroyMaterial(this.ctaaMat);
		}
		if (this.mat_enhance != null)
		{
			CTAA_PC.DestroyMaterial(this.mat_enhance);
		}
		if (this.rtAccum0 != null)
		{
			Object.DestroyImmediate(this.rtAccum0);
		}
		this.rtAccum0 = null;
		if (this.rtAccum1 != null)
		{
			Object.DestroyImmediate(this.rtAccum1);
		}
		this.rtAccum1 = null;
		if (this.afterPreEnhace != null)
		{
			Object.DestroyImmediate(this.afterPreEnhace);
		}
		this.afterPreEnhace = null;
		base.GetComponent<Camera>().targetTexture = null;
		if (this.upScaleRT != null)
		{
			Object.DestroyImmediate(this.upScaleRT);
		}
		this.upScaleRT = null;
		if (this.m_LayerRenderCam != null)
		{
			this.m_LayerRenderCam.targetTexture = null;
			Object.Destroy(this.m_LayerRenderCam.gameObject);
		}
		if (this.m_LayerMaskCam != null)
		{
			this.m_LayerMaskCam.targetTexture = null;
			Object.Destroy(this.m_LayerMaskCam.gameObject);
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0001EA50 File Offset: 0x0001CC50
	private void Start()
	{
		if (this.ExtendedFeatures)
		{
			if (this.SuperSampleMode == 0)
			{
				this.upscaleFactor = 1;
				this.resizeDownFactor = 1;
			}
			else if (this.SuperSampleMode == 1)
			{
				this.upscaleFactor = 2;
				this.resizeDownFactor = 2;
			}
			else if (this.SuperSampleMode == 2)
			{
				this.upscaleFactor = 2;
				this.resizeDownFactor = 1;
			}
			Camera component = base.GetComponent<Camera>();
			this.startResX = Screen.width;
			this.startResY = Screen.height;
			int num = this.startResX * this.upscaleFactor;
			int num2 = this.startResY * this.upscaleFactor;
			if (this.upScaleRT == null || this.upScaleRT.width != num || this.upScaleRT.height != num2)
			{
				Object.Destroy(this.upScaleRT);
				this.upScaleRT = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32);
				this.upScaleRT.filterMode = FilterMode.Bilinear;
				this.upScaleRT.wrapMode = TextureWrapMode.Repeat;
				this.upScaleRT.Create();
				base.GetComponent<Camera>().targetTexture = this.upScaleRT;
			}
			if (!this.m_LayerMaskCam)
			{
				GameObject gameObject = new GameObject("LayerMaskRenderCam");
				this.m_LayerMaskCam = gameObject.AddComponent<Camera>();
				this.m_LayerMaskCam.CopyFrom(component);
				this.m_LayerMaskCam.transform.position = base.transform.position;
				this.m_LayerMaskCam.transform.rotation = base.transform.rotation;
				LayerMask mask = -1;
				this.m_LayerMaskCam.cullingMask = mask;
				this.m_LayerMaskCam.depth = component.depth + 1f;
				this.m_LayerMaskCam.clearFlags = CameraClearFlags.Depth;
				this.m_LayerMaskCam.depthTextureMode = DepthTextureMode.None;
				this.m_LayerMaskCam.targetTexture = null;
				this.m_LayerMaskCam.allowMSAA = false;
				this.m_LayerMaskCam.enabled = false;
				this.m_LayerMaskCam.renderingPath = RenderingPath.Forward;
			}
			if (!this.m_LayerRenderCam)
			{
				GameObject gameObject2 = new GameObject("LayerRenderCam");
				this.m_LayerRenderCam = gameObject2.AddComponent<Camera>();
				this.m_LayerRenderCam.CopyFrom(component);
				this.m_LayerRenderCam.transform.position = base.transform.position;
				this.m_LayerRenderCam.transform.rotation = base.transform.rotation;
				this.m_LayerRenderCam.cullingMask = this.m_ExcludeLayers;
				this.m_LayerRenderCam.depth = component.depth + 1f;
				this.m_LayerRenderCam.clearFlags = CameraClearFlags.Depth;
				this.m_LayerRenderCam.depthTextureMode = DepthTextureMode.None;
				this.m_LayerRenderCam.targetTexture = null;
				this.m_LayerRenderCam.gameObject.AddComponent<RenderPostCTAA>();
				RenderPostCTAA component2 = this.m_LayerRenderCam.gameObject.GetComponent<RenderPostCTAA>();
				component2.ctaaPC = base.GetComponent<CTAA_PC>();
				component2.ctaaCamTransform = base.transform;
				component2.MaskRenderCam = this.m_LayerMaskCam.GetComponent<Camera>();
				component2.maskRenderShader = Shader.Find("Unlit/CtaaMaskRenderShader");
				component2.layerPostMat = new Material(Shader.Find("Hidden/CTAA_Layer_Post"));
				this.m_LayerRenderCam.enabled = true;
				component2.layerMaskingEnabled = this.m_LayerMaskingEnabled;
			}
			if (this.MSAA_Control)
			{
				QualitySettings.antiAliasing = this.m_MSAA_Level;
				return;
			}
		}
		else
		{
			MonoBehaviour.print("CTAA Standard Mode Enabled");
			this.upscaleFactor = 1;
			this.resizeDownFactor = 1;
			if (this.MSAA_Control)
			{
				QualitySettings.antiAliasing = this.m_MSAA_Level;
			}
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0001EDC8 File Offset: 0x0001CFC8
	private void OnPreCull()
	{
		if ((this.startResX != Screen.width || this.startResY != Screen.height) && this.ExtendedFeatures)
		{
			if (this.SuperSampleMode == 0)
			{
				this.upscaleFactor = 1;
				this.resizeDownFactor = 1;
			}
			else if (this.SuperSampleMode == 1)
			{
				this.upscaleFactor = 2;
				this.resizeDownFactor = 2;
			}
			else if (this.SuperSampleMode == 2)
			{
				this.upscaleFactor = 2;
				this.resizeDownFactor = 1;
			}
			base.GetComponent<Camera>();
			this.startResX = Screen.width;
			this.startResY = Screen.height;
			int num = this.startResX * this.upscaleFactor;
			int num2 = this.startResY * this.upscaleFactor;
			if (this.upScaleRT == null || this.upScaleRT.width != num || this.upScaleRT.height != num2)
			{
				Object.Destroy(this.upScaleRT);
				this.upScaleRT = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32);
				this.upScaleRT.filterMode = FilterMode.Bilinear;
				this.upScaleRT.wrapMode = TextureWrapMode.Repeat;
				this.upScaleRT.Create();
				base.GetComponent<Camera>().targetTexture = this.upScaleRT;
				Debug.Log("CTAA SuperResolution Updated");
			}
			this.moveActive = true;
			this.count = 0;
		}
		if (this.CTAA_Enabled)
		{
			this.jitterCam();
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0001EF20 File Offset: 0x0001D120
	private void jitterCam()
	{
		base.GetComponent<Camera>().ResetWorldToCameraMatrix();
		base.GetComponent<Camera>().ResetProjectionMatrix();
		base.GetComponent<Camera>().nonJitteredProjectionMatrix = base.GetComponent<Camera>().projectionMatrix;
		Matrix4x4 projectionMatrix = base.GetComponent<Camera>().projectionMatrix;
		float num = this.x_jit[this.frameCounter] * this.jitterScale;
		float num2 = this.y_jit[this.frameCounter] * this.jitterScale;
		projectionMatrix.m02 += (num * 2f - 1f) / base.GetComponent<Camera>().pixelRect.width;
		projectionMatrix.m12 += (num2 * 2f - 1f) / base.GetComponent<Camera>().pixelRect.height;
		this.frameCounter++;
		this.frameCounter %= 16;
		base.GetComponent<Camera>().projectionMatrix = projectionMatrix;
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0001F010 File Offset: 0x0001D210
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.CTAA_Enabled)
		{
			this.SetCTAA_Parameters();
			if (this.rtAccum0 == null || this.rtAccum0.width != source.width / this.resizeDownFactor || this.rtAccum0.height != source.height / this.resizeDownFactor)
			{
				Object.DestroyImmediate(this.rtAccum0);
				this.rtAccum0 = new RenderTexture(source.width / this.resizeDownFactor, source.height / this.resizeDownFactor, 0, source.format);
				this.rtAccum0.hideFlags = HideFlags.HideAndDontSave;
				this.rtAccum0.filterMode = FilterMode.Bilinear;
				this.rtAccum0.wrapMode = TextureWrapMode.Repeat;
			}
			if (this.rtAccum1 == null || this.rtAccum1.width != source.width / this.resizeDownFactor || this.rtAccum1.height != source.height / this.resizeDownFactor)
			{
				Object.DestroyImmediate(this.rtAccum1);
				this.rtAccum1 = new RenderTexture(source.width / this.resizeDownFactor, source.height / this.resizeDownFactor, 0, source.format);
				this.rtAccum1.hideFlags = HideFlags.HideAndDontSave;
				this.rtAccum1.filterMode = FilterMode.Bilinear;
				this.rtAccum1.wrapMode = TextureWrapMode.Repeat;
			}
			if (this.PreEnhanceEnabled)
			{
				if (this.afterPreEnhace == null || this.afterPreEnhace.width != source.width || this.afterPreEnhace.height != source.height)
				{
					Object.DestroyImmediate(this.afterPreEnhace);
					this.afterPreEnhace = new RenderTexture(source.width, source.height, 0, source.format);
					this.afterPreEnhace.hideFlags = HideFlags.HideAndDontSave;
					this.afterPreEnhace.filterMode = FilterMode.Point;
					this.afterPreEnhace.wrapMode = TextureWrapMode.Clamp;
				}
				this.mat_enhance.SetFloat("_AEXCTAA", 1f / (float)Screen.width);
				this.mat_enhance.SetFloat("_AEYCTAA", 1f / (float)Screen.height);
				this.mat_enhance.SetFloat("_AESCTAA", this.preEnhanceStrength);
				this.mat_enhance.SetFloat("_AEMAXCTAA", this.preEnhanceClamp);
				Graphics.Blit(source, this.afterPreEnhace, this.mat_enhance, 1);
				if (this.firstFrame)
				{
					Graphics.Blit(this.afterPreEnhace, this.rtAccum0);
					this.firstFrame = false;
				}
				this.ctaaMat.SetFloat("_AdaptiveResolve", this.AdaptiveResolve);
				this.ctaaMat.SetVector("_ControlParams", new Vector4(1f, (float)this.TemporalStability, this.HdrResponse, this.EdgeResponse));
				if (this.swap)
				{
					this.ctaaMat.SetTexture("_Accum", this.rtAccum0);
					Graphics.Blit(this.afterPreEnhace, this.rtAccum1, this.ctaaMat);
					Graphics.Blit(this.rtAccum1, destination);
				}
				else
				{
					this.ctaaMat.SetTexture("_Accum", this.rtAccum1);
					Graphics.Blit(this.afterPreEnhace, this.rtAccum0, this.ctaaMat);
					Graphics.Blit(this.rtAccum0, destination);
				}
			}
			else
			{
				if (this.firstFrame)
				{
					Graphics.Blit(source, this.rtAccum0);
					this.firstFrame = false;
				}
				this.ctaaMat.SetFloat("_AdaptiveResolve", this.AdaptiveResolve);
				this.ctaaMat.SetVector("_ControlParams", new Vector4(1f, (float)this.TemporalStability, this.HdrResponse, this.EdgeResponse));
				if (this.swap)
				{
					this.ctaaMat.SetTexture("_Accum", this.rtAccum0);
					Graphics.Blit(source, this.rtAccum1, this.ctaaMat);
					Graphics.Blit(this.rtAccum1, destination);
				}
				else
				{
					this.ctaaMat.SetTexture("_Accum", this.rtAccum1);
					Graphics.Blit(source, this.rtAccum0, this.ctaaMat);
					Graphics.Blit(this.rtAccum0, destination);
				}
			}
			this.swap = !this.swap;
			return;
		}
		Graphics.Blit(source, destination);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0001F43C File Offset: 0x0001D63C
	public RenderTexture getCTAA_Render()
	{
		return this.upScaleRT;
	}

	// Token: 0x040002E0 RID: 736
	[Space(5f)]
	public bool CTAA_Enabled = true;

	// Token: 0x040002E1 RID: 737
	[Header("CTAA Settings")]
	[Tooltip("Number of Frames to Blend via Re-Projection")]
	[Range(3f, 16f)]
	public int TemporalStability = 6;

	// Token: 0x040002E2 RID: 738
	[Space(5f)]
	[Tooltip("Anti-Aliasing Response and Strength for HDR Pixels")]
	[Range(0.001f, 4f)]
	public float HdrResponse = 1.2f;

	// Token: 0x040002E3 RID: 739
	[Space(5f)]
	[Tooltip("Amount of AA Blur in Geometric edges")]
	[Range(0f, 2f)]
	public float EdgeResponse = 0.5f;

	// Token: 0x040002E4 RID: 740
	[Space(5f)]
	[Tooltip("Amount of Automatic Sharpness added based on relative velocities")]
	[Range(0f, 1.5f)]
	public float AdaptiveSharpness = 0.2f;

	// Token: 0x040002E5 RID: 741
	[Space(5f)]
	[Tooltip("Amount sub-pixel Camera Jitter")]
	[Range(0f, 0.5f)]
	public float TemporalJitterScale = 0.475f;

	// Token: 0x040002E6 RID: 742
	[Space(5f)]
	[Tooltip("Eliminates Micro Shimmer - (No Dynamic Objects) Suitable for Architectural Visualisation, CAD, Engineering or non-moving objects. Camera can be moved.")]
	public bool AntiShimmerMode;

	// Token: 0x040002E7 RID: 743
	private int upscaleFactor = 1;

	// Token: 0x040002E8 RID: 744
	private int resizeDownFactor = 1;

	// Token: 0x040002E9 RID: 745
	public LayerMask m_ExcludeLayers;

	// Token: 0x040002EA RID: 746
	public int SuperSampleMode;

	// Token: 0x040002EB RID: 747
	public bool ExtendedFeatures;

	// Token: 0x040002EC RID: 748
	public bool MSAA_Control;

	// Token: 0x040002ED RID: 749
	public int m_MSAA_Level;

	// Token: 0x040002EE RID: 750
	public bool m_LayerMaskingEnabled = true;

	// Token: 0x040002EF RID: 751
	private Vector4 delValues = new Vector4(0.01f, 2f, 0.5f, 0.3f);

	// Token: 0x040002F0 RID: 752
	private bool PreEnhanceEnabled = true;

	// Token: 0x040002F1 RID: 753
	private float preEnhanceStrength = 1f;

	// Token: 0x040002F2 RID: 754
	private float preEnhanceClamp = 0.005f;

	// Token: 0x040002F3 RID: 755
	private float AdaptiveResolve = 3000f;

	// Token: 0x040002F4 RID: 756
	private float jitterScale = 1f;

	// Token: 0x040002F5 RID: 757
	private Material ctaaMat;

	// Token: 0x040002F6 RID: 758
	private Material mat_enhance;

	// Token: 0x040002F7 RID: 759
	private RenderTexture rtAccum0;

	// Token: 0x040002F8 RID: 760
	private RenderTexture rtAccum1;

	// Token: 0x040002F9 RID: 761
	private RenderTexture afterPreEnhace;

	// Token: 0x040002FA RID: 762
	private RenderTexture upScaleRT;

	// Token: 0x040002FB RID: 763
	private bool firstFrame;

	// Token: 0x040002FC RID: 764
	private bool swap;

	// Token: 0x040002FD RID: 765
	private int frameCounter;

	// Token: 0x040002FE RID: 766
	private Vector3 camoldpos;

	// Token: 0x040002FF RID: 767
	private float[] x_jit = new float[]
	{
		0.5f,
		-0.25f,
		0.75f,
		-0.125f,
		0.625f,
		0.575f,
		-0.875f,
		0.0625f,
		-0.3f,
		0.75f,
		-0.25f,
		-0.625f,
		0.325f,
		0.975f,
		-0.075f,
		0.625f
	};

	// Token: 0x04000300 RID: 768
	private float[] y_jit = new float[]
	{
		0.33f,
		-0.66f,
		0.51f,
		0.44f,
		-0.77f,
		0.12f,
		-0.55f,
		0.88f,
		-0.83f,
		0.14f,
		0.71f,
		-0.34f,
		0.87f,
		-0.12f,
		0.75f,
		0.08f
	};

	// Token: 0x04000301 RID: 769
	public bool moveActive = true;

	// Token: 0x04000302 RID: 770
	public float speed = 0.002f;

	// Token: 0x04000303 RID: 771
	private int count;

	// Token: 0x04000304 RID: 772
	private int startResX;

	// Token: 0x04000305 RID: 773
	private int startResY;

	// Token: 0x04000306 RID: 774
	private Camera m_LayerRenderCam;

	// Token: 0x04000307 RID: 775
	private Camera m_LayerMaskCam;
}
