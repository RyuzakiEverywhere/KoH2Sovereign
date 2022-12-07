using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class MicroSplatObject : MonoBehaviour
{
	// Token: 0x06000211 RID: 529 RVA: 0x0001FC70 File Offset: 0x0001DE70
	protected void ApplyMaps(Material m)
	{
		if (m.HasProperty("_GeoTex") && this.geoTextureOverride != null)
		{
			m.SetTexture("_GeoTex", this.geoTextureOverride);
		}
		if (m.HasProperty("_GeoCurve") && this.propData != null)
		{
			m.SetTexture("_GeoCurve", this.propData.GetGeoCurve());
		}
		if (m.HasProperty("_AlphaHoleTexture") && this.clipMap != null)
		{
			m.SetTexture("_AlphaHoleTexture", this.clipMap);
		}
		if (m.HasProperty("_PerPixelNormal"))
		{
			m.SetTexture("_PerPixelNormal", this.perPixelNormal);
		}
		if (m.HasProperty("_GlobalTintTex") && this.tintMapOverride != null)
		{
			m.SetTexture("_GlobalTintTex", this.tintMapOverride);
		}
		if (m.HasProperty("_GlobalNormalTex") && this.globalNormalOverride != null)
		{
			m.SetTexture("_GlobalNormalTex", this.globalNormalOverride);
		}
		if (m.HasProperty("_VSGrassMap") && this.vsGrassMap != null)
		{
			m.SetTexture("_VSGrassMap", this.vsGrassMap);
		}
		if (m.HasProperty("_VSShadowMap") && this.vsShadowMap != null)
		{
			m.SetTexture("_VSShadowMap", this.vsShadowMap);
		}
		if (m.HasProperty("_StreamControl"))
		{
			m.SetTexture("_StreamControl", this.streamTexture);
		}
		if (m.HasProperty("_AdvDetailControl"))
		{
			m.SetTexture("_AdvDetailControl", this.advDetailControl);
		}
		if (this.propData != null)
		{
			m.SetTexture("_PerTexProps", this.propData.GetTexture());
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0001FE34 File Offset: 0x0001E034
	protected void ApplyControlTextures(Texture2D[] controls, Material m)
	{
		m.SetTexture("_Control0", (controls.Length != 0) ? controls[0] : Texture2D.blackTexture);
		m.SetTexture("_Control1", (controls.Length > 1) ? controls[1] : Texture2D.blackTexture);
		m.SetTexture("_Control2", (controls.Length > 2) ? controls[2] : Texture2D.blackTexture);
		m.SetTexture("_Control3", (controls.Length > 3) ? controls[3] : Texture2D.blackTexture);
		m.SetTexture("_Control4", (controls.Length > 4) ? controls[4] : Texture2D.blackTexture);
		m.SetTexture("_Control5", (controls.Length > 5) ? controls[5] : Texture2D.blackTexture);
		m.SetTexture("_Control6", (controls.Length > 6) ? controls[6] : Texture2D.blackTexture);
		m.SetTexture("_Control7", (controls.Length > 7) ? controls[7] : Texture2D.blackTexture);
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0001FF18 File Offset: 0x0001E118
	protected void SyncBlendMat(Vector3 size)
	{
		if (this.blendMatInstance != null && this.matInstance != null)
		{
			this.blendMatInstance.CopyPropertiesFromMaterial(this.matInstance);
			Vector4 value = default(Vector4);
			value.z = size.x;
			value.w = size.z;
			value.x = base.transform.position.x;
			value.y = base.transform.position.z;
			this.blendMatInstance.SetVector("_TerrainBounds", value);
			this.blendMatInstance.SetTexture("_TerrainDesc", this.terrainDesc);
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0001FFD0 File Offset: 0x0001E1D0
	public virtual Bounds GetBounds()
	{
		return default(Bounds);
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0001FFE8 File Offset: 0x0001E1E8
	public Material GetBlendMatInstance()
	{
		if (this.blendMat != null && this.terrainDesc != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
				this.SyncBlendMat(this.GetBounds().size);
			}
			if (this.blendMatInstance.shader != this.blendMat.shader)
			{
				this.blendMatInstance.shader = this.blendMat.shader;
				this.SyncBlendMat(this.GetBounds().size);
			}
		}
		return this.blendMatInstance;
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00020094 File Offset: 0x0001E294
	protected void ApplyBlendMap()
	{
		if (this.blendMat != null && this.terrainDesc != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
			}
			this.SyncBlendMat(this.GetBounds().size);
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x000023FD File Offset: 0x000005FD
	public void RevisionFromMat()
	{
	}

	// Token: 0x0400031C RID: 796
	[HideInInspector]
	public Material templateMaterial;

	// Token: 0x0400031D RID: 797
	[HideInInspector]
	[NonSerialized]
	public Material matInstance;

	// Token: 0x0400031E RID: 798
	[HideInInspector]
	public Texture2D terrainDesc;

	// Token: 0x0400031F RID: 799
	[HideInInspector]
	public Texture2D cavityMap;

	// Token: 0x04000320 RID: 800
	[HideInInspector]
	public Material blendMat;

	// Token: 0x04000321 RID: 801
	[HideInInspector]
	public Material blendMatInstance;

	// Token: 0x04000322 RID: 802
	[HideInInspector]
	public MicroSplatKeywords keywordSO;

	// Token: 0x04000323 RID: 803
	[HideInInspector]
	public Texture2D perPixelNormal;

	// Token: 0x04000324 RID: 804
	[HideInInspector]
	public Texture2D tintMapOverride;

	// Token: 0x04000325 RID: 805
	[HideInInspector]
	public Texture2D globalNormalOverride;

	// Token: 0x04000326 RID: 806
	[HideInInspector]
	public Texture2D geoTextureOverride;

	// Token: 0x04000327 RID: 807
	[HideInInspector]
	public Texture2D streamTexture;

	// Token: 0x04000328 RID: 808
	[HideInInspector]
	public Texture2D vsGrassMap;

	// Token: 0x04000329 RID: 809
	[HideInInspector]
	public Texture2D vsShadowMap;

	// Token: 0x0400032A RID: 810
	[HideInInspector]
	public Texture2D advDetailControl;

	// Token: 0x0400032B RID: 811
	[HideInInspector]
	public Texture2D clipMap;

	// Token: 0x0400032C RID: 812
	[HideInInspector]
	public Texture2D customControl0;

	// Token: 0x0400032D RID: 813
	[HideInInspector]
	public Texture2D customControl1;

	// Token: 0x0400032E RID: 814
	[HideInInspector]
	public Texture2D customControl2;

	// Token: 0x0400032F RID: 815
	[HideInInspector]
	public Texture2D customControl3;

	// Token: 0x04000330 RID: 816
	[HideInInspector]
	public Texture2D customControl4;

	// Token: 0x04000331 RID: 817
	[HideInInspector]
	public Texture2D customControl5;

	// Token: 0x04000332 RID: 818
	[HideInInspector]
	public Texture2D customControl6;

	// Token: 0x04000333 RID: 819
	[HideInInspector]
	public Texture2D customControl7;

	// Token: 0x04000334 RID: 820
	[HideInInspector]
	public MicroSplatPropData propData;
}
