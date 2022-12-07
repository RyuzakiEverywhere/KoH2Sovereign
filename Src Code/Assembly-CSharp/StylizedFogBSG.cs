using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Stylized Fog BSG")]
public class StylizedFogBSG : MonoBehaviour
{
	// Token: 0x0600016B RID: 363 RVA: 0x0000EE5C File Offset: 0x0000D05C
	private void Start()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0000EE5C File Offset: 0x0000D05C
	private void OnEnable()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000EE70 File Offset: 0x0000D070
	private void OnDisable()
	{
		this.clearResources();
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000EE78 File Offset: 0x0000D078
	public void UpdateTextures()
	{
		this.setGradient();
		this.SetKeywords();
		this.updateValues();
	}

	// Token: 0x0600016F RID: 367 RVA: 0x0000EE8C File Offset: 0x0000D08C
	private void updateValues()
	{
		if (this.fogMat == null || this.fogShader == null)
		{
			this.createResources();
		}
		if (this.mainRamp != null)
		{
			this.fogMat.SetTexture("_SF_MainRamp", this.mainRamp);
		}
		if (this.useBlend && this.blendRamp != null)
		{
			this.fogMat.SetTexture("_SF_BlendRamp", this.blendRamp);
			this.fogMat.SetFloat("_SF_Blend", this.blend);
		}
		if (this.useNoise && this.noiseTexture != null)
		{
			this.fogMat.SetTexture("_SF_NoiseTex", this.noiseTexture);
			this.fogMat.SetVector("_SF_NoiseSpeed", this.noiseSpeed);
			this.fogMat.SetVector("_SF_NoiseTiling", this.noiseTiling);
		}
		if (this.AdvancedMode)
		{
			this.fogMat.SetFloat("_SF_Near", this.NearRange);
			this.fogMat.SetFloat("_SF_Far", this.FarRange);
		}
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000EFB0 File Offset: 0x0000D1B0
	private void setGradient()
	{
		if (this.gradientSource == StylizedFogBSG.StylizedFogGradient.Textures)
		{
			this.mainRamp = this.rampTexture;
			if (this.useBlend)
			{
				this.blendRamp = this.rampBlendTexture;
				return;
			}
		}
		else if (this.gradientSource == StylizedFogBSG.StylizedFogGradient.Gradients)
		{
			if (this.mainRamp != null)
			{
				Object.DestroyImmediate(this.mainRamp);
			}
			this.mainRamp = this.GenerateGradient(this.rampGradient, 256, 8);
			if (this.useBlend)
			{
				if (this.blendRamp != null)
				{
					Object.DestroyImmediate(this.blendRamp);
				}
				this.blendRamp = this.GenerateGradient(this.rampBlendGradient, 256, 8);
			}
		}
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000F05C File Offset: 0x0000D25C
	private Texture2D GenerateGradient(Gradient gradient, int gWidth, int gHeight)
	{
		Texture2D texture2D = new Texture2D(gWidth, gHeight, TextureFormat.ARGB32, false);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		Color color = Color.white;
		if (gradient != null)
		{
			for (int i = 0; i < gWidth; i++)
			{
				color = gradient.Evaluate((float)i / (float)gWidth);
				for (int j = 0; j < gHeight; j++)
				{
					texture2D.SetPixel(i, j, color);
				}
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06000172 RID: 370 RVA: 0x0000F0C0 File Offset: 0x0000D2C0
	private void createResources()
	{
		if (this.fogShader == null)
		{
			this.fogShader = Shader.Find("Hidden/StylizedFog_BSG");
		}
		if (this.fogMat == null && this.fogShader != null)
		{
			this.fogMat = new Material(this.fogShader);
			this.fogMat.hideFlags = HideFlags.HideAndDontSave;
		}
		if (this.mainRamp == null || this.blendRamp == null)
		{
			this.setGradient();
		}
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
			this.cam.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000F174 File Offset: 0x0000D374
	private void clearResources()
	{
		if (this.fogMat != null)
		{
			Object.DestroyImmediate(this.fogMat);
		}
		this.disableKeywords();
		this.cam.depthTextureMode = DepthTextureMode.None;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0000F1A4 File Offset: 0x0000D3A4
	public void SetKeywords()
	{
		switch (this.fogMode)
		{
		case StylizedFogBSG.StylizedFogMode.Blend:
			Shader.EnableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFogBSG.StylizedFogMode.Additive:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.EnableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFogBSG.StylizedFogMode.Multiply:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.EnableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFogBSG.StylizedFogMode.Screen:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.EnableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFogBSG.StylizedFogMode.Overlay:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.EnableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFogBSG.StylizedFogMode.Dodge:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.EnableKeyword("_FOG_DODGE");
			break;
		}
		if (this.useBlend)
		{
			Shader.EnableKeyword("_FOG_BLEND_ON");
		}
		else
		{
			Shader.DisableKeyword("_FOG_BLEND_ON");
		}
		if (this.useNoise)
		{
			Shader.EnableKeyword("_FOG_NOISE_ON");
		}
		else
		{
			Shader.DisableKeyword("_FOG_NOISE_ON");
		}
		if (this.AdvancedMode)
		{
			Shader.EnableKeyword("_ADVANCED_ON");
		}
		else
		{
			Shader.DisableKeyword("_ADVANCED_ON");
		}
		if (this.ExcludeSkybox)
		{
			Shader.EnableKeyword("_SKYBOX");
			return;
		}
		Shader.DisableKeyword("_SKYBOX");
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000F3D0 File Offset: 0x0000D5D0
	private void disableKeywords()
	{
		Shader.DisableKeyword("_FOG_BLEND");
		Shader.DisableKeyword("_FOG_ADDITIVE");
		Shader.DisableKeyword("_FOG_MULTIPLY");
		Shader.DisableKeyword("_FOG_SCREEN");
		Shader.DisableKeyword("_FOG_BLEND_OFF");
		Shader.DisableKeyword("_FOG_BLEND_ON");
		Shader.DisableKeyword("_FOG_NOISE_OFF");
		Shader.DisableKeyword("_FOG_NOISE_ON");
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000F42D File Offset: 0x0000D62D
	private bool isSupported()
	{
		return this.fogShader.isSupported && !(this.fogShader == null);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000F44D File Offset: 0x0000D64D
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.isSupported())
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.updateValues();
		Graphics.Blit(source, destination, this.fogMat);
	}

	// Token: 0x04000276 RID: 630
	public StylizedFogBSG.StylizedFogMode fogMode;

	// Token: 0x04000277 RID: 631
	public bool ExcludeSkybox;

	// Token: 0x04000278 RID: 632
	[Header("Blend")]
	[Tooltip("Use a second ramp for transition")]
	[SerializeField]
	private bool useBlend;

	// Token: 0x04000279 RID: 633
	[Tooltip("Amount of blend between 2 gradients")]
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x0400027A RID: 634
	[Header("Gradients")]
	[Tooltip("Use ramp from textures or gradient fields")]
	public StylizedFogBSG.StylizedFogGradient gradientSource;

	// Token: 0x0400027B RID: 635
	public Gradient rampGradient;

	// Token: 0x0400027C RID: 636
	public Gradient rampBlendGradient;

	// Token: 0x0400027D RID: 637
	public Texture2D rampTexture;

	// Token: 0x0400027E RID: 638
	public Texture2D rampBlendTexture;

	// Token: 0x0400027F RID: 639
	[Header("AdvancedMode")]
	[Tooltip("Experimental: Will remap the depth to custom values. \nUseful when you have big camera range")]
	public bool AdvancedMode;

	// Token: 0x04000280 RID: 640
	public float NearRange;

	// Token: 0x04000281 RID: 641
	public float FarRange = 1000f;

	// Token: 0x04000282 RID: 642
	[Header("Noise Texture")]
	[SerializeField]
	private bool useNoise;

	// Token: 0x04000283 RID: 643
	public Texture2D noiseTexture;

	// Token: 0x04000284 RID: 644
	[Space(5f)]
	[Tooltip("XY: Speed1 XY | WH: Speed2 XY")]
	public Vector4 noiseSpeed;

	// Token: 0x04000285 RID: 645
	[Space(5f)]
	[Tooltip("XY: Tiling1 XY | WH: Tiling2 XY")]
	public Vector4 noiseTiling = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04000286 RID: 646
	private Camera cam;

	// Token: 0x04000287 RID: 647
	private Texture2D mainRamp;

	// Token: 0x04000288 RID: 648
	private Texture2D blendRamp;

	// Token: 0x04000289 RID: 649
	private Shader fogShader;

	// Token: 0x0400028A RID: 650
	private Material fogMat;

	// Token: 0x020004FC RID: 1276
	public enum StylizedFogMode
	{
		// Token: 0x04002E69 RID: 11881
		Blend,
		// Token: 0x04002E6A RID: 11882
		Additive,
		// Token: 0x04002E6B RID: 11883
		Multiply,
		// Token: 0x04002E6C RID: 11884
		Screen,
		// Token: 0x04002E6D RID: 11885
		Overlay,
		// Token: 0x04002E6E RID: 11886
		Dodge
	}

	// Token: 0x020004FD RID: 1277
	public enum StylizedFogGradient
	{
		// Token: 0x04002E70 RID: 11888
		Textures,
		// Token: 0x04002E71 RID: 11889
		Gradients
	}
}
