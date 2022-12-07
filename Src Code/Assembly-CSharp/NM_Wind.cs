using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
[ExecuteInEditMode]
public class NM_Wind : MonoBehaviour
{
	// Token: 0x0600017C RID: 380 RVA: 0x0000F50A File Offset: 0x0000D70A
	private void Start()
	{
		this.ApplySettings();
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000F50A File Offset: 0x0000D70A
	private void Update()
	{
		this.ApplySettings();
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000F50A File Offset: 0x0000D70A
	private void OnValidate()
	{
		this.ApplySettings();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000F514 File Offset: 0x0000D714
	private void ApplySettings()
	{
		Shader.SetGlobalTexture("WIND_SETTINGS_TexNoise", this.NoiseTexture);
		Shader.SetGlobalTexture("WIND_SETTINGS_TexGust", this.GustMaskTexture);
		Shader.SetGlobalVector("WIND_SETTINGS_WorldDirectionAndSpeed", this.GetDirectionAndSpeed());
		Shader.SetGlobalFloat("WIND_SETTINGS_FlexNoiseScale", 1f / Mathf.Max(0.01f, this.FlexNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_ShiverNoiseScale", 1f / Mathf.Max(0.01f, this.ShiverNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_Turbulence", this.WindSpeed * this.Turbulence);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustSpeed", this.GustSpeed);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustScale", this.GustScale);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustWorldScale", 1f / Mathf.Max(0.01f, this.GustWorldSize));
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000F5E8 File Offset: 0x0000D7E8
	private Vector4 GetDirectionAndSpeed()
	{
		Vector3 normalized = base.transform.forward.normalized;
		return new Vector4(normalized.x, normalized.y, normalized.z, this.WindSpeed * 0.2777f);
	}

	// Token: 0x0400028F RID: 655
	[Header("General Parameters")]
	[Tooltip("Wind Speed in Kilometers per hour")]
	public float WindSpeed = 30f;

	// Token: 0x04000290 RID: 656
	[Range(0f, 2f)]
	[Tooltip("Wind Turbulence in percentage of wind Speed")]
	public float Turbulence = 0.25f;

	// Token: 0x04000291 RID: 657
	[Header("Noise Parameters")]
	[Tooltip("Texture used for wind turbulence")]
	public Texture2D NoiseTexture;

	// Token: 0x04000292 RID: 658
	[Tooltip("Size of one world tiling patch of the Noise Texture, for bending trees")]
	public float FlexNoiseWorldSize = 175f;

	// Token: 0x04000293 RID: 659
	[Tooltip("Size of one world tiling patch of the Noise Texture, for leaf shivering")]
	public float ShiverNoiseWorldSize = 10f;

	// Token: 0x04000294 RID: 660
	[Header("Gust Parameters")]
	[Tooltip("Texture used for wind gusts")]
	public Texture2D GustMaskTexture;

	// Token: 0x04000295 RID: 661
	[Tooltip("Size of one world tiling patch of the Gust Texture, for leaf shivering")]
	public float GustWorldSize = 600f;

	// Token: 0x04000296 RID: 662
	[Tooltip("Wind Gust Speed in Kilometers per hour")]
	public float GustSpeed = 50f;

	// Token: 0x04000297 RID: 663
	[Tooltip("Wind Gust Influence on trees")]
	public float GustScale = 1f;
}
