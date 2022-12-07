using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
[ExecuteInEditMode]
public class MicroSplatBlendableObject : MonoBehaviour
{
	// Token: 0x06000240 RID: 576 RVA: 0x000211E1 File Offset: 0x0001F3E1
	private void OnEnable()
	{
		this.Sync();
	}

	// Token: 0x06000241 RID: 577 RVA: 0x000211E1 File Offset: 0x0001F3E1
	private void Start()
	{
		this.Sync();
	}

	// Token: 0x06000242 RID: 578 RVA: 0x000211EC File Offset: 0x0001F3EC
	public Bounds TransformBounds(Bounds localBounds)
	{
		Vector3 center = base.transform.TransformPoint(localBounds.center);
		Vector3 extents = localBounds.extents;
		Vector3 vector = base.transform.TransformVector(extents.x, 0f, 0f);
		Vector3 vector2 = base.transform.TransformVector(0f, extents.y, 0f);
		Vector3 vector3 = base.transform.TransformVector(0f, 0f, extents.z);
		extents.x = Mathf.Abs(vector.x) + Mathf.Abs(vector2.x) + Mathf.Abs(vector3.x);
		extents.y = Mathf.Abs(vector.y) + Mathf.Abs(vector2.y) + Mathf.Abs(vector3.y);
		extents.z = Mathf.Abs(vector.z) + Mathf.Abs(vector2.z) + Mathf.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00021304 File Offset: 0x0001F504
	public void Sync()
	{
		Material blendMatInstance = this.msObject.GetBlendMatInstance();
		if (this.msObject == null && blendMatInstance)
		{
			return;
		}
		Renderer component = base.GetComponent<Renderer>();
		Material[] sharedMaterials = component.sharedMaterials;
		bool flag = false;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			if (sharedMaterials[i] == blendMatInstance && blendMatInstance != null)
			{
				flag = true;
			}
			else if (sharedMaterials[i] == null || sharedMaterials[i].shader == null || sharedMaterials[i].shader.name.Contains("_BlendWithTerrain"))
			{
				flag = true;
				sharedMaterials[i] = blendMatInstance;
				component.sharedMaterials = sharedMaterials;
			}
		}
		if (!flag)
		{
			Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
			sharedMaterials[sharedMaterials.Length - 1] = blendMatInstance;
			component.sharedMaterials = sharedMaterials;
		}
		if (MicroSplatBlendableObject.props == null)
		{
			MicroSplatBlendableObject.props = new MaterialPropertyBlock();
		}
		MicroSplatBlendableObject.props.Clear();
		MicroSplatBlendableObject.props.SetVector("_TerrainBlendParams", new Vector4(this.blendDistance, this.blendContrast, this.msObject.transform.position.y, this.blendCurve));
		MicroSplatBlendableObject.props.SetVector("_SlopeBlendParams", new Vector4(this.slopeFilter, this.slopeContrast, this.slopeNoise, this.normalBlendDistance));
		MicroSplatBlendableObject.props.SetVector("_SnowBlendParams", new Vector4(this.snowWidth, 0f, 0f, 0f));
		MicroSplatBlendableObject.props.SetFloat("_TBNoiseScale", this.noiseScale);
		MicroSplatBlendableObject.props.SetVector("_FeatureFilters", new Vector4(this.doTerrainBlend ? 0f : 1f, this.doSnow ? 0f : 1f, 0f, 0f));
		if (this.normalFromObject != null)
		{
			MicroSplatBlendableObject.props.SetTexture("_NormalOriginal", this.normalFromObject);
		}
		component.SetPropertyBlock(MicroSplatBlendableObject.props);
	}

	// Token: 0x0400035C RID: 860
	[HideInInspector]
	public MicroSplatObject msObject;

	// Token: 0x0400035D RID: 861
	public float blendDistance = 1f;

	// Token: 0x0400035E RID: 862
	public float normalBlendDistance = 1f;

	// Token: 0x0400035F RID: 863
	[Range(0.0001f, 1f)]
	public float blendContrast;

	// Token: 0x04000360 RID: 864
	[Range(0.25f, 4f)]
	public float blendCurve = 1f;

	// Token: 0x04000361 RID: 865
	[Range(0f, 1f)]
	public float slopeFilter = 1f;

	// Token: 0x04000362 RID: 866
	[Range(1f, 40f)]
	public float slopeContrast = 20f;

	// Token: 0x04000363 RID: 867
	[Range(0f, 1f)]
	public float slopeNoise = 0.35f;

	// Token: 0x04000364 RID: 868
	private static MaterialPropertyBlock props;

	// Token: 0x04000365 RID: 869
	[Range(0f, 1f)]
	public float snowDampening;

	// Token: 0x04000366 RID: 870
	[Range(0f, 1f)]
	public float snowWidth;

	// Token: 0x04000367 RID: 871
	public float noiseScale = 1f;

	// Token: 0x04000368 RID: 872
	public bool doSnow = true;

	// Token: 0x04000369 RID: 873
	public bool doTerrainBlend = true;

	// Token: 0x0400036A RID: 874
	public Texture2D normalFromObject;
}
