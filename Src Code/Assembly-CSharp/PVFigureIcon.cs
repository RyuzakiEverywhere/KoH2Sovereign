using System;
using Logic;
using UnityEngine;

// Token: 0x02000162 RID: 354
public abstract class PVFigureIcon : PVFigure
{
	// Token: 0x060011FB RID: 4603 RVA: 0x000BCE84 File Offset: 0x000BB084
	public override void Init()
	{
		this.icon = global::Common.FindChildByName(base.gameObject, "id_Icon", true, true);
		GameObject gameObject = this.icon;
		this.meshRenderer = ((gameObject != null) ? gameObject.GetComponent<MeshRenderer>() : null);
		this.materialBlock = new MaterialPropertyBlock();
		GameObject gameObject2 = global::Common.FindChildByName(base.gameObject, "_Shield", true, true);
		this.crest = ((gameObject2 != null) ? gameObject2.GetComponent<CrestObject>() : null);
		base.Init();
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x000448AF File Offset: 0x00042AAF
	public virtual Texture2D GetIconTexture()
	{
		return null;
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x000BCEF7 File Offset: 0x000BB0F7
	public override void RefreshDefField()
	{
		this.RefreshIconMeasurements();
		this.RefreshCrestMeasurements();
		base.RefreshDefField();
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x000BCF0C File Offset: 0x000BB10C
	public void RefreshIconMeasurements()
	{
		if (this.icon == null)
		{
			return;
		}
		DT.Field hierarchyChildField = base.GetHierarchyChildField("icon_measurements");
		if (hierarchyChildField == null)
		{
			this.icon.SetActive(false);
			return;
		}
		float posX = hierarchyChildField.Value(0, null, true, true);
		float posY = hierarchyChildField.Value(1, null, true, true);
		Value value = hierarchyChildField.Value(2, null, true, true);
		Value value2 = hierarchyChildField.Value(3, null, true, true);
		float num = (value == Value.Unknown) ? 1f : value;
		float scaleY = (value2 == Value.Unknown) ? num : value2;
		this.icon.gameObject.SetActive(true);
		this.iconParams = new PVFigureIcon.TransformParams(posX, posY, num, scaleY);
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x000BCFD8 File Offset: 0x000BB1D8
	public void RefreshIconPosition()
	{
		if (this.icon == null)
		{
			return;
		}
		Vector3 localPosition = new Vector3(this.iconParams.pos.x * (float)(this.IsFlipped() ? -1 : 1), this.iconParams.pos.y, 0f);
		Vector3 localScale = new Vector3(this.iconParams.scale.x, this.iconParams.scale.y, 1f);
		this.icon.transform.localPosition = localPosition;
		this.icon.transform.localScale = localScale;
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x000BD07C File Offset: 0x000BB27C
	public void RefreshCrestMeasurements()
	{
		if (this.crest == null)
		{
			return;
		}
		DT.Field hierarchyChildField = base.GetHierarchyChildField("crest_measurements");
		if (hierarchyChildField == null)
		{
			this.crest.gameObject.SetActive(false);
			return;
		}
		float posX = hierarchyChildField.Value(0, null, true, true);
		float posY = hierarchyChildField.Value(1, null, true, true);
		Value value = hierarchyChildField.Value(2, null, true, true);
		Value value2 = hierarchyChildField.Value(3, null, true, true);
		float num = (value == Value.Unknown) ? 1f : value;
		float scaleY = (value2 == Value.Unknown) ? num : value2;
		this.crest.gameObject.SetActive(true);
		this.crestParams = new PVFigureIcon.TransformParams(posX, posY, num, scaleY);
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x000BD14C File Offset: 0x000BB34C
	public void RefreshCrestPosition()
	{
		if (this.crest == null)
		{
			return;
		}
		float z = -0.0005f;
		Vector3 localPosition = new Vector3(this.crestParams.pos.x * (float)(this.IsFlipped() ? -1 : 1), this.crestParams.pos.y, z);
		Vector3 localScale = new Vector3(this.crestParams.scale.x, this.crestParams.scale.y, 1f);
		this.crest.transform.localPosition = localPosition;
		this.crest.transform.localScale = localScale;
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x000BD1F2 File Offset: 0x000BB3F2
	public virtual void RefreshIcon()
	{
		this.icon_texture = this.GetIconTexture();
		if (this.icon_texture == null)
		{
			return;
		}
		this.materialBlock.SetTexture("_MainTex", this.icon_texture);
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x000BD228 File Offset: 0x000BB428
	public override void Refresh()
	{
		base.Refresh();
		this.RefreshIconPosition();
		this.RefreshCrestPosition();
		if (this.meshRenderer == null)
		{
			return;
		}
		this.RefreshIcon();
		this.materialBlock.SetFloat("_Flip", this.IsFlipped() ? 1f : 0f);
		this.meshRenderer.SetPropertyBlock(this.materialBlock);
	}

	// Token: 0x04000C1D RID: 3101
	private Texture2D icon_texture;

	// Token: 0x04000C1E RID: 3102
	private MeshRenderer meshRenderer;

	// Token: 0x04000C1F RID: 3103
	private MaterialPropertyBlock materialBlock;

	// Token: 0x04000C20 RID: 3104
	public GameObject icon;

	// Token: 0x04000C21 RID: 3105
	public CrestObject crest;

	// Token: 0x04000C22 RID: 3106
	public PVFigureIcon.TransformParams crestParams;

	// Token: 0x04000C23 RID: 3107
	public PVFigureIcon.TransformParams iconParams;

	// Token: 0x02000687 RID: 1671
	public struct TransformParams
	{
		// Token: 0x060047EA RID: 18410 RVA: 0x00215E31 File Offset: 0x00214031
		public TransformParams(float posX, float posY, float scaleX, float scaleY)
		{
			this.pos = new Vector2(posX, posY);
			this.scale = new Vector2(scaleX, scaleY);
		}

		// Token: 0x040035D7 RID: 13783
		public Vector2 pos;

		// Token: 0x040035D8 RID: 13784
		public Vector2 scale;
	}
}
