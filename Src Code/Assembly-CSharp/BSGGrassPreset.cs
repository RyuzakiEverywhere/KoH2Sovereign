using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
[Serializable]
public class BSGGrassPreset : ScriptableObject
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600046F RID: 1135 RVA: 0x0003488F File Offset: 0x00032A8F
	public static BSGGrassPreset Default
	{
		get
		{
			if (!(BSGGrassPreset._default != null))
			{
				return BSGGrassPreset._default = ScriptableObject.CreateInstance<BSGGrassPreset>();
			}
			return BSGGrassPreset._default;
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x000348B0 File Offset: 0x00032AB0
	public void Validate()
	{
		this.minWidth = Mathf.Max(0f, this.minWidth);
		this.maxWidth = Mathf.Max(this.minWidth, this.maxWidth);
		this.minHeight = Mathf.Max(0f, this.minHeight);
		this.maxHeight = Mathf.Max(this.minHeight, this.maxHeight);
		this.minPitch = Mathf.Clamp01(this.minPitch);
		this.maxPitch = Mathf.Clamp01(this.maxPitch);
		this.minPitch = Mathf.Min(this.minPitch, this.maxPitch);
		this.basemapBlend = Mathf.Clamp01(this.basemapBlend);
		this.randomizeColor = Mathf.Clamp01(this.randomizeColor);
		this.noiseSpread = Mathf.Max(0.0001f, this.noiseSpread);
		this.wind = Mathf.Clamp01(this.wind);
		this.closeCutout = Mathf.Clamp01(this.closeCutout);
		this.distanceCutout = Mathf.Clamp01(this.distanceCutout);
		this.endFadeDistance = Mathf.Clamp01(this.endFadeDistance);
		this.startFadeDistance = Mathf.Clamp(this.startFadeDistance, 0f, this.endFadeDistance - 1E-05f);
		this.translucency = Mathf.Clamp01(this.translucency);
		this.fakeAO = Mathf.Clamp01(this.fakeAO);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00034A10 File Offset: 0x00032C10
	public DetailPrototype CreatePrototype()
	{
		return new DetailPrototype
		{
			prototype = null,
			prototypeTexture = this.texture,
			minWidth = this.minWidth,
			maxWidth = this.maxWidth,
			minHeight = this.minHeight,
			maxHeight = this.maxHeight,
			noiseSpread = 10f,
			healthyColor = BSGGrassData.PresetIndexToColor(this.index),
			dryColor = BSGGrassData.PresetIndexToColor(this.index),
			renderMode = DetailRenderMode.GrassBillboard,
			usePrototypeMesh = false,
			bendFactor = 0f
		};
	}

	// Token: 0x0400045C RID: 1116
	private static BSGGrassPreset _default;

	// Token: 0x0400045D RID: 1117
	public int index;

	// Token: 0x0400045E RID: 1118
	public Texture2D texture;

	// Token: 0x0400045F RID: 1119
	public float minWidth = 0.75f;

	// Token: 0x04000460 RID: 1120
	public float maxWidth = 1.5f;

	// Token: 0x04000461 RID: 1121
	public float minHeight = 0.75f;

	// Token: 0x04000462 RID: 1122
	public float maxHeight = 1.5f;

	// Token: 0x04000463 RID: 1123
	public Color firstColor = Color.gray;

	// Token: 0x04000464 RID: 1124
	public Color secondColor = Color.gray;

	// Token: 0x04000465 RID: 1125
	[Range(0f, 1f)]
	public float minPitch;

	// Token: 0x04000466 RID: 1126
	[Range(0f, 1f)]
	public float maxPitch = 0.5f;

	// Token: 0x04000467 RID: 1127
	public float heightOffset;

	// Token: 0x04000468 RID: 1128
	[Range(0f, 1f)]
	public float basemapBlend = 0.5f;

	// Token: 0x04000469 RID: 1129
	[Range(0f, 1f)]
	public float randomizeColor = 0.5f;

	// Token: 0x0400046A RID: 1130
	public float noiseSpread = 0.1f;

	// Token: 0x0400046B RID: 1131
	[Range(0f, 1f)]
	public float wind = 0.3f;

	// Token: 0x0400046C RID: 1132
	[Range(0f, 1f)]
	public float closeCutout = 0.3f;

	// Token: 0x0400046D RID: 1133
	[Range(0f, 1f)]
	public float distanceCutout = 0.3f;

	// Token: 0x0400046E RID: 1134
	[Range(0f, 1f)]
	public float startFadeDistance = 0.4f;

	// Token: 0x0400046F RID: 1135
	[Range(0f, 1f)]
	public float endFadeDistance = 1f;

	// Token: 0x04000470 RID: 1136
	[Range(0f, 1f)]
	public float translucency = 0.2f;

	// Token: 0x04000471 RID: 1137
	[Range(0f, 1f)]
	public float fakeAO = 0.5f;
}
