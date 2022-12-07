using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000157 RID: 343
[ExecuteInEditMode]
public class VerticalTint : PostEffectsBase
{
	// Token: 0x06001190 RID: 4496 RVA: 0x000B84C4 File Offset: 0x000B66C4
	public override bool CheckResources()
	{
		base.CheckSupport(true);
		this.tintMaterial = base.CheckShaderAndCreateMaterial(this.tintShader, this.tintMaterial);
		if (this.tintMaterial == null)
		{
			return false;
		}
		if (Application.isPlaying)
		{
			Game game = GameLogic.Get(true);
			if (game != null && game.fow != this.showFoW)
			{
				game.fow = this.showFoW;
				WorldMap worldMap = WorldMap.Get();
				if (worldMap != null)
				{
					worldMap.ReloadView();
				}
			}
		}
		return this.isSupported;
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x000B8548 File Offset: 0x000B6748
	private void FindChildren(Transform t)
	{
		if (this.children == null)
		{
			this.children = new List<VerticalTint>();
		}
		if (t == null)
		{
			return;
		}
		VerticalTint component = t.GetComponent<VerticalTint>();
		if (component != null && component != this)
		{
			Camera component2 = component.GetComponent<Camera>();
			if (component2 == null || !component2.enabled)
			{
				this.children.Add(component);
			}
		}
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			this.FindChildren(child);
		}
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x000B85D0 File Offset: 0x000B67D0
	private VerticalTint FindChild()
	{
		if (this.children == null)
		{
			this.FindChildren(base.transform.parent);
		}
		for (int i = 0; i < this.children.Count; i++)
		{
			VerticalTint verticalTint = this.children[i];
			if (verticalTint.isActiveAndEnabled)
			{
				return verticalTint;
			}
		}
		return null;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000B8624 File Offset: 0x000B6824
	private void OnTransformChildrenChanged()
	{
		this.FindChildren(base.transform.parent);
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x000B8637 File Offset: 0x000B6837
	private void OnEnable()
	{
		this.children = null;
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x000B8640 File Offset: 0x000B6840
	private void OnPreRender()
	{
		if (this.tintMaterial == null)
		{
			return;
		}
		Camera component = base.GetComponent<Camera>();
		VerticalTint verticalTint = this.FindChild();
		if (verticalTint != null)
		{
			verticalTint.OnPreRender(component);
			return;
		}
		this.OnPreRender(component);
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x000B8684 File Offset: 0x000B6884
	private void OnPreRender(Camera camera)
	{
		Matrix4x4 cameraToWorldMatrix = camera.cameraToWorldMatrix;
		Matrix4x4 inverse = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true).inverse;
		ref Matrix4x4 ptr = ref inverse;
		ptr[1, 1] = ptr[1, 1] * -1f;
		this.tintMaterial.SetMatrix("_WorldFromView", cameraToWorldMatrix);
		this.tintMaterial.SetMatrix("_ViewFromScreen", inverse);
		this.tintMaterial.SetInt("_excludeFarPixels", this.excludeSkybox ? 1 : 0);
		this.tintMaterial.SetFloat("_terrainInfluence", this.terrainInfluence);
		this.tintMaterial.SetFloat("_tintOffsetH", this.tintOffsetH);
		this.tintMaterial.SetFloat("_tintUnderOcean", this.tintUnderOcean);
		this.oceanLevel = MapData.GetWaterLevel();
		this.tintMaterial.SetFloat("_oceanLevel", this.oceanLevel);
		this.tintMaterial.SetFloat("_tintMaxH", this.maxHeight);
		this.tintMaterial.SetFloat("_tintAlpha", this.tintAlpha);
		this.tintMaterial.SetFloat("_tintGamma", this.gradientGamma);
		this.tintMaterial.SetColor("_bottomColor", this.bottomColor);
		this.tintMaterial.SetColor("_topColor", this.topColor);
		this.tintMaterial.SetColor("_wetColor", this.wetColor);
		this.tintMaterial.SetFloat("_wetGamma", this.wetGamma);
		this.tintMaterial.SetFloat("_wetAlpha", this.wetAlpha);
		this.tintMaterial.SetFloat("_wetOffset", this.terrainOffset);
		this.tintMaterial.SetFloat("_wetHeight", this.wetHeight);
		float f = 0.017453292f * this.cloudsDir;
		Vector2 v = new Vector2(-Mathf.Cos(f), -Mathf.Sin(f));
		this.tintMaterial.SetInt("_clouds_on", this.showClouds ? 1 : 0);
		this.tintMaterial.SetTexture("_CloudsTex", this.tex_Clouds);
		this.tintMaterial.SetColor("_clouds_color", this.cloudsColor);
		this.tintMaterial.SetFloat("_clouds_speed", this.cloudsSpeed);
		this.tintMaterial.SetFloat("_clouds_size", this.cloudsSize);
		this.tintMaterial.SetFloat("_clouds_alpha", this.cloudsAlpha);
		this.tintMaterial.SetVector("_clouds_dir", v);
		this.tintMaterial.SetFloat("_clouds_gamma", this.cloudsGamma);
		this.tintMaterial.SetFloat("_clouds_dist", this.cloudsDist);
		this.tintMaterial.SetInt("_show_fow", this.showFoW ? 1 : 0);
		this.tintMaterial.SetInt("_use_pm_colors", this.usePMColors ? 1 : 0);
		this.tintMaterial.SetInt("_highlight_realms", this.highlightRealms ? 1 : 0);
		this.tintMaterial.SetInt("_show_hatch", this.showHatch ? 1 : 0);
		this.tintMaterial.SetInt("_force_hatch_colors", this.forceHatchColors ? 1 : 0);
		this.tintMaterial.SetInt("_show_SDF", this.showSDF ? 1 : 0);
		this.tintMaterial.SetTexture("_HatchTex", this.tex_Hatch);
		this.tintMaterial.SetFloat("_sdfGamma", this.sdfGamma);
		this.tintMaterial.SetFloat("_sdfPow", this.sdfPow);
		this.tintMaterial.SetColor("_sdfColor", this.sdfColor);
		this.tintMaterial.SetColor("_hatchColorR", this.hatchColorR);
		this.tintMaterial.SetColor("_hatchColorG", this.hatchColorG);
		this.tintMaterial.SetColor("_hatchColorB", this.hatchColorB);
		this.tintMaterial.SetFloat("_hatchScale", this.hatchScale);
		this.tintMaterial.SetFloat("_hatchAlpha", this.hatchAlpha);
		this.tintMaterial.SetInt("_show_vfog", this.showVfog ? 1 : 0);
		this.tintMaterial.SetColor("_vfogColorStart", this.vfogColorStart);
		this.tintMaterial.SetColor("_vfogColorEnd", this.vfogColorEnd);
		this.tintMaterial.SetFloat("_vfog_color_gamma", this.vfogColorGamma);
		this.tintMaterial.SetFloat("_vfog_bot", this.vfogBot);
		this.tintMaterial.SetFloat("_vfog_top", this.vfogTop);
		this.tintMaterial.SetFloat("_vfog_zero", this.vfogStartDistance);
		this.tintMaterial.SetFloat("_vfog_max", this.vfogMaxDistance);
		this.tintMaterial.SetFloat("_vfog_dist_gamma", this.vfogDistGamma);
		this.tintMaterial.SetFloat("_vfog_height_gamma", this.vfogHeightGamma);
		this.tintMaterial.SetInt("_show_dfog", this.showDfog ? 1 : 0);
		this.tintMaterial.SetInt("_dfog_sky", this.fogSkybox ? 0 : 1);
		this.tintMaterial.SetColor("_dfogColorStart", this.dfogColorStart);
		this.tintMaterial.SetColor("_dfogColorEnd", this.dfogColorEnd);
		this.tintMaterial.SetFloat("_dfog_color_gamma", this.dfogColorGamma);
		this.tintMaterial.SetFloat("_dfog_zero", this.dfogStartDistance);
		this.tintMaterial.SetFloat("_dfog_max", this.dfogMaxDistance);
		this.tintMaterial.SetFloat("_dfog_dist_gamma", this.dfogDistGamma);
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x000B8C44 File Offset: 0x000B6E44
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		VerticalTint verticalTint = this.FindChild();
		if (verticalTint != null)
		{
			verticalTint.OnRenderImage(source, destination);
			return;
		}
		if (!this.CheckResources() || this.tintAlpha <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		Graphics.Blit(source, destination, this.tintMaterial);
	}

	// Token: 0x04000BA2 RID: 2978
	[Space(10f)]
	[Header("Vertical Tint")]
	[Space(10f)]
	public Color bottomColor = new Color(0.31640625f, 0.34375f, 0.4609375f);

	// Token: 0x04000BA3 RID: 2979
	public Color topColor = new Color(0.99609375f, 0.91015625f, 0.74609375f);

	// Token: 0x04000BA4 RID: 2980
	[Range(0f, 64f)]
	public float maxHeight = 12f;

	// Token: 0x04000BA5 RID: 2981
	[Range(-1f, 1f)]
	public float gradientGamma;

	// Token: 0x04000BA6 RID: 2982
	[Range(0f, 1f)]
	public float tintAlpha = 0.5f;

	// Token: 0x04000BA7 RID: 2983
	[Range(0f, 1f)]
	public float terrainInfluence = 0.5f;

	// Token: 0x04000BA8 RID: 2984
	[Range(0f, 1f)]
	public float tintOffsetH = 1f;

	// Token: 0x04000BA9 RID: 2985
	[Range(0f, 1f)]
	public float tintUnderOcean = 0.5f;

	// Token: 0x04000BAA RID: 2986
	public bool excludeSkybox = true;

	// Token: 0x04000BAB RID: 2987
	[Space(10f)]
	[Header("Ocean Wetness")]
	public float oceanLevel;

	// Token: 0x04000BAC RID: 2988
	[Space(10f)]
	public Color wetColor = new Color(0.31640625f, 0.34375f, 0.4609375f);

	// Token: 0x04000BAD RID: 2989
	[Range(0f, 3f)]
	public float wetHeight = 1.25f;

	// Token: 0x04000BAE RID: 2990
	[Range(0.1f, 4f)]
	public float wetGamma = 3f;

	// Token: 0x04000BAF RID: 2991
	[Range(0f, 1f)]
	public float wetAlpha = 0.8f;

	// Token: 0x04000BB0 RID: 2992
	[Range(0f, 1f)]
	public float terrainOffset = 0.4f;

	// Token: 0x04000BB1 RID: 2993
	[Space(10f)]
	[Header("Cloud Shadows")]
	[Space(10f)]
	public bool showClouds = true;

	// Token: 0x04000BB2 RID: 2994
	[SerializeField]
	public Texture2D tex_Clouds;

	// Token: 0x04000BB3 RID: 2995
	public Color cloudsColor = Color.black;

	// Token: 0x04000BB4 RID: 2996
	[Range(0f, 360f)]
	public float cloudsDir;

	// Token: 0x04000BB5 RID: 2997
	[Range(1f, 10f)]
	public float cloudsSpeed = 1f;

	// Token: 0x04000BB6 RID: 2998
	[Range(1f, 10f)]
	public float cloudsSize = 5f;

	// Token: 0x04000BB7 RID: 2999
	[Range(-1f, 1f)]
	public float cloudsGamma;

	// Token: 0x04000BB8 RID: 3000
	[Range(0f, 1f)]
	public float cloudsAlpha = 0.5f;

	// Token: 0x04000BB9 RID: 3001
	[Range(0f, 2000f)]
	public float cloudsDist = 300f;

	// Token: 0x04000BBA RID: 3002
	[Space(10f)]
	[Header("Fog of War")]
	[Space(10f)]
	public bool showFoW;

	// Token: 0x04000BBB RID: 3003
	public bool usePMColors;

	// Token: 0x04000BBC RID: 3004
	public bool highlightRealms;

	// Token: 0x04000BBD RID: 3005
	[SerializeField]
	public bool showSDF;

	// Token: 0x04000BBE RID: 3006
	public Color sdfColor = Color.red;

	// Token: 0x04000BBF RID: 3007
	[Range(0f, 1f)]
	public float sdfGamma = 1f;

	// Token: 0x04000BC0 RID: 3008
	[Range(0.01f, 10f)]
	public float sdfPow = 1f;

	// Token: 0x04000BC1 RID: 3009
	public bool showHatch;

	// Token: 0x04000BC2 RID: 3010
	public bool forceHatchColors;

	// Token: 0x04000BC3 RID: 3011
	public Texture2D tex_Hatch;

	// Token: 0x04000BC4 RID: 3012
	[Range(100f, 1500f)]
	public float hatchScale = 300f;

	// Token: 0x04000BC5 RID: 3013
	public Color hatchColorR = Color.red;

	// Token: 0x04000BC6 RID: 3014
	public Color hatchColorG = Color.green;

	// Token: 0x04000BC7 RID: 3015
	public Color hatchColorB = Color.blue;

	// Token: 0x04000BC8 RID: 3016
	[Range(0f, 1f)]
	public float hatchAlpha = 0.5f;

	// Token: 0x04000BC9 RID: 3017
	[Space(10f)]
	[Header("Vertical & Distance Fog")]
	[Space(10f)]
	public bool showVfog = true;

	// Token: 0x04000BCA RID: 3018
	public Color vfogColorStart = new Color(0.99609375f, 0.91015625f, 0.74609375f);

	// Token: 0x04000BCB RID: 3019
	public Color vfogColorEnd = new Color(0.99609375f, 0.91015625f, 0.74609375f);

	// Token: 0x04000BCC RID: 3020
	[Range(-1f, 1f)]
	public float vfogColorGamma;

	// Token: 0x04000BCD RID: 3021
	[Range(0f, 32f)]
	public float vfogBot;

	// Token: 0x04000BCE RID: 3022
	[Range(0f, 128f)]
	public float vfogTop = 10f;

	// Token: 0x04000BCF RID: 3023
	[Range(0f, 300f)]
	public float vfogStartDistance = 80f;

	// Token: 0x04000BD0 RID: 3024
	[Range(0f, 1000f)]
	public float vfogMaxDistance = 200f;

	// Token: 0x04000BD1 RID: 3025
	[Range(-1f, 1f)]
	public float vfogDistGamma;

	// Token: 0x04000BD2 RID: 3026
	[Range(-1f, 1f)]
	public float vfogHeightGamma;

	// Token: 0x04000BD3 RID: 3027
	[Space(10f)]
	public bool showDfog = true;

	// Token: 0x04000BD4 RID: 3028
	public Color dfogColorStart = new Color(0.99609375f, 0.91015625f, 0.74609375f);

	// Token: 0x04000BD5 RID: 3029
	public Color dfogColorEnd = new Color(0.99609375f, 0.91015625f, 0.74609375f);

	// Token: 0x04000BD6 RID: 3030
	[Range(-1f, 1f)]
	public float dfogColorGamma;

	// Token: 0x04000BD7 RID: 3031
	[Range(0f, 300f)]
	public float dfogStartDistance = 80f;

	// Token: 0x04000BD8 RID: 3032
	[Range(0f, 1000f)]
	public float dfogMaxDistance = 200f;

	// Token: 0x04000BD9 RID: 3033
	[Range(-1f, 1f)]
	public float dfogDistGamma;

	// Token: 0x04000BDA RID: 3034
	public bool fogSkybox;

	// Token: 0x04000BDB RID: 3035
	[Space(20f)]
	public Shader tintShader;

	// Token: 0x04000BDC RID: 3036
	private Material tintMaterial;

	// Token: 0x04000BDD RID: 3037
	private List<VerticalTint> children;
}
