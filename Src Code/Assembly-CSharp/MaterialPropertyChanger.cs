using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class MaterialPropertyChanger : MonoBehaviour
{
	// Token: 0x060010A5 RID: 4261 RVA: 0x000B1290 File Offset: 0x000AF490
	public void InitProperties()
	{
		this.properties = null;
		this.mat = null;
		this.rend = base.GetComponent<Renderer>();
		if (this.rend == null)
		{
			return;
		}
		this.mat = this.rend.sharedMaterial;
		this.mat == null;
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000B12E4 File Offset: 0x000AF4E4
	public int GetPropIdx(string name)
	{
		if (this.properties == null)
		{
			return -1;
		}
		for (int i = 0; i < this.properties.Length; i++)
		{
			if (this.properties[i].name == name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000B132C File Offset: 0x000AF52C
	public int GetChangeIdx(string name)
	{
		if (this.changes == null)
		{
			return -1;
		}
		for (int i = 0; i < this.changes.Count; i++)
		{
			if (this.changes[i].name == name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000B1378 File Offset: 0x000AF578
	public void ApplyChanges()
	{
		this.pb = null;
		int count = this.changes.Count;
		for (int i = 0; i < count; i++)
		{
			MaterialPropertyChanger.Property property = this.changes[i];
			int propIdx = this.GetPropIdx(property.name);
			if (propIdx >= 0)
			{
				if (this.properties != null)
				{
					this.properties[propIdx] = property;
				}
				if (this.pb == null)
				{
					this.pb = new MaterialPropertyBlock();
				}
				switch (property.type)
				{
				case MaterialPropertyChanger.PropType.Color:
					this.pb.SetColor(property.name, property.clr_val);
					break;
				case MaterialPropertyChanger.PropType.Vector:
					this.pb.SetVector(property.name, property.vec_val);
					break;
				case MaterialPropertyChanger.PropType.Float:
				case MaterialPropertyChanger.PropType.Range:
					this.pb.SetFloat(property.name, property.flt_val);
					break;
				case MaterialPropertyChanger.PropType.TexEnv:
					this.pb.SetTexture(property.name, property.tex_val);
					this.pb.SetVector(property.name + "_ST", new Vector4(property.tex_scale.x, property.tex_scale.y, property.tex_offset.x, property.tex_offset.y));
					break;
				}
			}
		}
		if (this.rend == null)
		{
			return;
		}
		this.rend.SetPropertyBlock(this.pb);
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x000B14EC File Offset: 0x000AF6EC
	private void OnEnable()
	{
		this.InitProperties();
		this.ApplyChanges();
	}

	// Token: 0x04000B0E RID: 2830
	[NonSerialized]
	public MaterialPropertyChanger.Property[] properties;

	// Token: 0x04000B0F RID: 2831
	[HideInInspector]
	public List<MaterialPropertyChanger.Property> changes = new List<MaterialPropertyChanger.Property>();

	// Token: 0x04000B10 RID: 2832
	[NonSerialized]
	public Renderer rend;

	// Token: 0x04000B11 RID: 2833
	[NonSerialized]
	public Material mat;

	// Token: 0x04000B12 RID: 2834
	private MaterialPropertyBlock pb;

	// Token: 0x02000656 RID: 1622
	public enum PropType
	{
		// Token: 0x04003514 RID: 13588
		Color,
		// Token: 0x04003515 RID: 13589
		Vector,
		// Token: 0x04003516 RID: 13590
		Float,
		// Token: 0x04003517 RID: 13591
		Range,
		// Token: 0x04003518 RID: 13592
		TexEnv
	}

	// Token: 0x02000657 RID: 1623
	[Serializable]
	public struct Property
	{
		// Token: 0x04003519 RID: 13593
		public bool changed;

		// Token: 0x0400351A RID: 13594
		public MaterialPropertyChanger.PropType type;

		// Token: 0x0400351B RID: 13595
		public string name;

		// Token: 0x0400351C RID: 13596
		public string descr;

		// Token: 0x0400351D RID: 13597
		public float min_val;

		// Token: 0x0400351E RID: 13598
		public float max_val;

		// Token: 0x0400351F RID: 13599
		public Color clr_val;

		// Token: 0x04003520 RID: 13600
		public Vector4 vec_val;

		// Token: 0x04003521 RID: 13601
		public float flt_val;

		// Token: 0x04003522 RID: 13602
		public Texture tex_val;

		// Token: 0x04003523 RID: 13603
		public Vector2 tex_offset;

		// Token: 0x04003524 RID: 13604
		public Vector2 tex_scale;
	}
}
