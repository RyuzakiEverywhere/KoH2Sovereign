using System;
using Logic;
using UnityEngine;

// Token: 0x02000261 RID: 609
public abstract class UIPVFigure : WorldToScreenObject
{
	// Token: 0x0600256E RID: 9582 RVA: 0x0014C4AC File Offset: 0x0014A6AC
	public virtual void Init()
	{
		this.RefreshDefField();
		if (this.pvCamera == null || this.gameCamera == null)
		{
			GameObject go = GameObject.Find("Cameras");
			GameObject gameObject = global::Common.FindChildByName(go, "PVCamera", true, true);
			this.pvCamera = ((gameObject != null) ? gameObject.GetComponent<Camera>() : null);
			Camera camera = this.pvCamera;
			this.gameCamera = ((camera != null) ? camera.gameObject.GetComponent<GameCamera>() : null);
		}
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000E4719 File Offset: 0x000E2919
	public void SetAllowedType(ViewMode.AllowedFigures allowedType = ViewMode.AllowedFigures.None)
	{
		this.allowedType = allowedType;
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool IsFlipped()
	{
		return false;
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x0014C524 File Offset: 0x0014A724
	public DT.Field GetHierarchyChildField(string child_name)
	{
		DT.Field parent = this.field;
		DT.Field field = null;
		while (field == null && parent != null)
		{
			field = parent.FindChild(child_name, null, true, true, true, '.');
			if (field == null)
			{
				parent = parent.parent;
			}
		}
		return field;
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x0014C55B File Offset: 0x0014A75B
	public override void RefreshDefField()
	{
		if (this.field != null && base.InitDef())
		{
			this.LoadDefs(null);
		}
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x0014C574 File Offset: 0x0014A774
	protected override void LoadDefs(DT.Field def)
	{
		base.LoadFigureScale(this.GetHierarchyChildField("figure_scale_to_camera_zoom"));
		base.LoadPVDef(this.GetHierarchyChildField("figure_scale_to_camera_pv_zoom"));
		base.LoadOffset3d(this.GetHierarchyChildField("offset_3d"));
		base.LoadOffset2d(this.GetHierarchyChildField("offset_2d"));
		base.LoadOffset2dAlternative(this.GetHierarchyChildField("offset_2d_alternative"));
		base.LoadClampToScreen(this.GetHierarchyChildField("clamp_to_screen"));
		base.LoadClampToScreenStance(this.GetHierarchyChildField("clamp_stance"));
		this.LoadRaycastTargetSize(this.GetHierarchyChildField("raycast_target_anchor"));
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x0014C60C File Offset: 0x0014A80C
	protected void LoadRaycastTargetSize(DT.Field field)
	{
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		worldToScreenScaleParams.raycast_target_min_x = (worldToScreenScaleParams.raycast_target_min_y = 0f);
		worldToScreenScaleParams.raycast_target_max_x = (worldToScreenScaleParams.raycast_target_max_y = 1f);
		if (field != null && field.NumValues() == 4)
		{
			worldToScreenScaleParams.raycast_target_min_x = field.Float(0, null, 0f);
			worldToScreenScaleParams.raycast_target_min_y = field.Float(1, null, 0f);
			worldToScreenScaleParams.raycast_target_max_x = field.Float(2, null, 0f);
			worldToScreenScaleParams.raycast_target_max_y = field.Float(3, null, 0f);
		}
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x0014C6AB File Offset: 0x0014A8AB
	public virtual void Refresh()
	{
		if (ViewMode.IsPoliticalView())
		{
			this.UpdateVisibilityFromView(ViewMode.current.allowedFigures);
		}
		this.RefreshRaycastTarget();
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x0014C6CC File Offset: 0x0014A8CC
	public void RefreshRaycastTarget()
	{
		if (this.m_RaycastTarget != null)
		{
			WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
			this.m_RaycastTarget.anchorMin = new Vector2(worldToScreenScaleParams.raycast_target_min_x, worldToScreenScaleParams.raycast_target_min_y);
			this.m_RaycastTarget.anchorMax = new Vector2(worldToScreenScaleParams.raycast_target_max_x, worldToScreenScaleParams.raycast_target_max_y);
		}
	}

	// Token: 0x06002577 RID: 9591 RVA: 0x0002C53B File Offset: 0x0002A73B
	protected virtual bool Clickable()
	{
		return true;
	}

	// Token: 0x04001987 RID: 6535
	protected DT.Field field;

	// Token: 0x04001988 RID: 6536
	protected Camera pvCamera;

	// Token: 0x04001989 RID: 6537
	protected GameCamera gameCamera;

	// Token: 0x0400198A RID: 6538
	[UIFieldTarget("id_RaycastTarget")]
	private RectTransform m_RaycastTarget;

	// Token: 0x0400198B RID: 6539
	private bool was_clickable;
}
