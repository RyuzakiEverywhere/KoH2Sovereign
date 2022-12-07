using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Stylized Fog")]
public class StylizedFog : MonoBehaviour
{
	// Token: 0x0600015D RID: 349 RVA: 0x0000E812 File Offset: 0x0000CA12
	private void Start()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000E812 File Offset: 0x0000CA12
	private void OnEnable()
	{
		this.createResources();
		this.UpdateTextures();
		this.SetKeywords();
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000E826 File Offset: 0x0000CA26
	private void OnDisable()
	{
		this.clearResources();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000E82E File Offset: 0x0000CA2E
	public void UpdateTextures()
	{
		this.setGradient();
		this.SetKeywords();
		this.updateValues();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000E844 File Offset: 0x0000CA44
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

	// Token: 0x06000162 RID: 354 RVA: 0x0000E968 File Offset: 0x0000CB68
	private void setGradient()
	{
		if (this.gradientSource == StylizedFog.StylizedFogGradient.Textures)
		{
			this.mainRamp = this.rampTexture;
			if (this.useBlend)
			{
				this.blendRamp = this.rampBlendTexture;
				return;
			}
		}
		else if (this.gradientSource == StylizedFog.StylizedFogGradient.Gradients)
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

	// Token: 0x06000163 RID: 355 RVA: 0x0000EA14 File Offset: 0x0000CC14
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

	// Token: 0x06000164 RID: 356 RVA: 0x0000EA78 File Offset: 0x0000CC78
	private void createResources()
	{
		if (this.fogShader == null)
		{
			this.fogShader = Shader.Find("Hidden/StylizedFog");
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

	// Token: 0x06000165 RID: 357 RVA: 0x0000EB2C File Offset: 0x0000CD2C
	private void clearResources()
	{
		if (this.fogMat != null)
		{
			Object.DestroyImmediate(this.fogMat);
		}
		this.disableKeywords();
		this.cam.depthTextureMode = DepthTextureMode.None;
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000EB5C File Offset: 0x0000CD5C
	public void SetKeywords()
	{
		switch (this.fogMode)
		{
		case StylizedFog.StylizedFogMode.Blend:
			Shader.EnableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Additive:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.EnableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Multiply:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.EnableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Screen:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.EnableKeyword("_FOG_SCREEN");
			Shader.DisableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Overlay:
			Shader.DisableKeyword("_FOG_BLEND");
			Shader.DisableKeyword("_FOG_ADDITIVE");
			Shader.DisableKeyword("_FOG_MULTIPLY");
			Shader.DisableKeyword("_FOG_SCREEN");
			Shader.EnableKeyword("_FOG_OVERLAY");
			Shader.DisableKeyword("_FOG_DODGE");
			break;
		case StylizedFog.StylizedFogMode.Dodge:
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

	// Token: 0x06000167 RID: 359 RVA: 0x0000ED88 File Offset: 0x0000CF88
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

	// Token: 0x06000168 RID: 360 RVA: 0x0000EDE5 File Offset: 0x0000CFE5
	private bool isSupported()
	{
		return this.fogShader.isSupported && !(this.fogShader == null);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000EE05 File Offset: 0x0000D005
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

	// Token: 0x04000261 RID: 609
	public StylizedFog.StylizedFogMode fogMode;

	// Token: 0x04000262 RID: 610
	public bool ExcludeSkybox;

	// Token: 0x04000263 RID: 611
	[Header("Blend")]
	[Tooltip("Use a second ramp for transition")]
	[SerializeField]
	private bool useBlend;

	// Token: 0x04000264 RID: 612
	[Tooltip("Amount of blend between 2 gradients")]
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x04000265 RID: 613
	[Header("Gradients")]
	[Tooltip("Use ramp from textures or gradient fields")]
	public StylizedFog.StylizedFogGradient gradientSource;

	// Token: 0x04000266 RID: 614
	public Gradient rampGradient;

	// Token: 0x04000267 RID: 615
	public Gradient rampBlendGradient;

	// Token: 0x04000268 RID: 616
	public Texture2D rampTexture;

	// Token: 0x04000269 RID: 617
	public Texture2D rampBlendTexture;

	// Token: 0x0400026A RID: 618
	[Header("AdvancedMode")]
	[Tooltip("Experimental: Will remap the depth to custom values. \nUseful when you have big camera range")]
	public bool AdvancedMode;

	// Token: 0x0400026B RID: 619
	public float NearRange;

	// Token: 0x0400026C RID: 620
	public float FarRange = 1000f;

	// Token: 0x0400026D RID: 621
	[Header("Noise Texture")]
	[SerializeField]
	private bool useNoise;

	// Token: 0x0400026E RID: 622
	public Texture2D noiseTexture;

	// Token: 0x0400026F RID: 623
	[Space(5f)]
	[Tooltip("XY: Speed1 XY | WH: Speed2 XY")]
	public Vector4 noiseSpeed;

	// Token: 0x04000270 RID: 624
	[Space(5f)]
	[Tooltip("XY: Tiling1 XY | WH: Tiling2 XY")]
	public Vector4 noiseTiling = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04000271 RID: 625
	private Camera cam;

	// Token: 0x04000272 RID: 626
	private Texture2D mainRamp;

	// Token: 0x04000273 RID: 627
	private Texture2D blendRamp;

	// Token: 0x04000274 RID: 628
	private Shader fogShader;

	// Token: 0x04000275 RID: 629
	private Material fogMat;

	// Token: 0x020004FA RID: 1274
	public enum StylizedFogMode
	{
		// Token: 0x04002E5F RID: 11871
		Blend,
		// Token: 0x04002E60 RID: 11872
		Additive,
		// Token: 0x04002E61 RID: 11873
		Multiply,
		// Token: 0x04002E62 RID: 11874
		Screen,
		// Token: 0x04002E63 RID: 11875
		Overlay,
		// Token: 0x04002E64 RID: 11876
		Dodge
	}

	// Token: 0x020004FB RID: 1275
	public enum StylizedFogGradient
	{
		// Token: 0x04002E66 RID: 11878
		Textures,
		// Token: 0x04002E67 RID: 11879
		Gradients
	}
}
