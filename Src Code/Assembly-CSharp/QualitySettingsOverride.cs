using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class QualitySettingsOverride : MonoBehaviour
{
	// Token: 0x06001125 RID: 4389 RVA: 0x000B5F1E File Offset: 0x000B411E
	private void Start()
	{
		this.UpdateAll();
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x000B5F1E File Offset: 0x000B411E
	private void OnValidate()
	{
		this.UpdateAll();
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x000B5F28 File Offset: 0x000B4128
	private void UpdateAll()
	{
		QualitySettings.shadows = this.shadowQuality;
		QualitySettings.shadowResolution = this.shadowResolution;
		QualitySettings.shadowCascades = (int)this.shadowCascades;
		QualitySettings.shadowProjection = this.shadowProjection;
		QualitySettings.shadowDistance = this.shadowDistance;
		QualitySettings.shadowmaskMode = this.shadowmaskMode;
		QualitySettings.shadowNearPlaneOffset = this.shadowNearPlaneOffset;
		QualitySettings.pixelLightCount = this.pixelLightCount;
		QualitySettings.masterTextureLimit = (int)this.textureQuality;
		QualitySettings.anisotropicFiltering = this.anisotropicFiltering;
		QualitySettings.antiAliasing = (int)this.antiAliasing;
		QualitySettings.softParticles = this.softParticles;
	}

	// Token: 0x04000B53 RID: 2899
	[Header("Render Settings")]
	public QualitySettingsOverride.TextureQuality textureQuality;

	// Token: 0x04000B54 RID: 2900
	[Range(1f, 4f)]
	public int pixelLightCount = 4;

	// Token: 0x04000B55 RID: 2901
	public AnisotropicFiltering anisotropicFiltering;

	// Token: 0x04000B56 RID: 2902
	public QualitySettingsOverride.AntiAliasing antiAliasing;

	// Token: 0x04000B57 RID: 2903
	public bool softParticles = true;

	// Token: 0x04000B58 RID: 2904
	[Header("Shadow Settings")]
	public ShadowQuality shadowQuality = ShadowQuality.All;

	// Token: 0x04000B59 RID: 2905
	public ShadowResolution shadowResolution = ShadowResolution.High;

	// Token: 0x04000B5A RID: 2906
	public QualitySettingsOverride.ShadowCascades shadowCascades = QualitySettingsOverride.ShadowCascades.Four;

	// Token: 0x04000B5B RID: 2907
	public ShadowProjection shadowProjection;

	// Token: 0x04000B5C RID: 2908
	public ShadowmaskMode shadowmaskMode;

	// Token: 0x04000B5D RID: 2909
	public float shadowDistance = 150f;

	// Token: 0x04000B5E RID: 2910
	public float shadowNearPlaneOffset = 20f;

	// Token: 0x02000663 RID: 1635
	public enum ShadowCascades
	{
		// Token: 0x0400355E RID: 13662
		None,
		// Token: 0x0400355F RID: 13663
		Two = 2,
		// Token: 0x04003560 RID: 13664
		Four = 4
	}

	// Token: 0x02000664 RID: 1636
	public enum TextureQuality
	{
		// Token: 0x04003562 RID: 13666
		Full,
		// Token: 0x04003563 RID: 13667
		Half,
		// Token: 0x04003564 RID: 13668
		Quater,
		// Token: 0x04003565 RID: 13669
		Eight
	}

	// Token: 0x02000665 RID: 1637
	public enum AntiAliasing
	{
		// Token: 0x04003567 RID: 13671
		Disabled,
		// Token: 0x04003568 RID: 13672
		MSx2 = 2,
		// Token: 0x04003569 RID: 13673
		MSx4 = 4,
		// Token: 0x0400356A RID: 13674
		MSx8 = 8
	}
}
