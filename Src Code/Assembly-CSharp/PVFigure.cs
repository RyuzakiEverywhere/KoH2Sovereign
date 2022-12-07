using System;
using Logic;
using UnityEngine;

// Token: 0x0200015F RID: 351
public abstract class PVFigure : MonoBehaviour
{
	// Token: 0x060011D3 RID: 4563 RVA: 0x000BBF18 File Offset: 0x000BA118
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

	// Token: 0x060011D4 RID: 4564 RVA: 0x000BBF8E File Offset: 0x000BA18E
	public void SetAllowedType(ViewMode.AllowedFigures allowedType = ViewMode.AllowedFigures.None)
	{
		this.allowedType = allowedType;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x000BBF97 File Offset: 0x000BA197
	public void SetParent(PVFigure parent)
	{
		this.parent = parent;
		this.UpdateVisibility();
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x000BBFA8 File Offset: 0x000BA1A8
	protected void UpdateTransfrom()
	{
		Vector3 vector = Vector3.one * this.defScale;
		if (this.visible_from_view && this.parent == null && this.pvCamera != null && this.gameCamera != null)
		{
			GameMode gameMode = this.gameCamera.currentControllScheme as GameMode;
			if (gameMode != null)
			{
				float distanceToAimPoint = gameMode.GetDistanceToAimPoint();
				float x = this.gameCamera.Settings.dist.x;
				float y = this.gameCamera.Settings.dist.y;
				Vector3 forward = this.pvCamera.transform.forward;
				Vector3 a = this.pvCamera.transform.position + forward * (distanceToAimPoint - x) / Mathf.Abs(forward.y);
				Vector3 a2 = this.pvCamera.transform.position + forward * (distanceToAimPoint - y) / Mathf.Abs(forward.y);
				float magnitude = (a - base.transform.position).magnitude;
				float magnitude2 = (a2 - base.transform.position).magnitude;
				float rmin = magnitude * this.FixedSizeToCamera * this.pvCamera.fieldOfView * this.defScaleCameraZoomedIn;
				float rmax = magnitude2 * this.FixedSizeToCamera * this.pvCamera.fieldOfView * this.defScaleOnCameraZoomedOut;
				vector *= global::Common.map(distanceToAimPoint, x, y, rmin, rmax, false);
			}
			else
			{
				float d = (this.pvCamera.transform.position - base.transform.position).magnitude * this.FixedSizeToCamera * this.pvCamera.fieldOfView;
				vector *= d;
			}
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.y = this.pvCamera.transform.eulerAngles.y;
			base.transform.eulerAngles = eulerAngles;
		}
		base.transform.localScale = vector;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x000BC1D5 File Offset: 0x000BA3D5
	public virtual bool IsVisible()
	{
		return this.visbility_from_object && this.visible_from_view && this.visbility_from_filter;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x000BC1EF File Offset: 0x000BA3EF
	public void UpdateVisibility()
	{
		base.gameObject.SetActive(this.IsVisible());
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x000BC202 File Offset: 0x000BA402
	public void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFiguresFromViewMode)
	{
		this.visible_from_view = ((allowedFiguresFromViewMode & this.allowedType) > ViewMode.AllowedFigures.None);
		this.UpdateVisibility();
		this.UpdateTransfrom();
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000BC221 File Offset: 0x000BA421
	public void UpdateVisibilityFromObject(bool visible_from_object)
	{
		this.visbility_from_object = visible_from_object;
		this.UpdateVisibility();
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x000BC230 File Offset: 0x000BA430
	public void UpdateVisibilityFilter()
	{
		this.visbility_from_filter = ((ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		this.UpdateVisibility();
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0002C538 File Offset: 0x0002A738
	public virtual bool IsFlipped()
	{
		return false;
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x000BC250 File Offset: 0x000BA450
	public DT.Field GetHierarchyChildField(string child_name)
	{
		DT.Field field = this.field;
		DT.Field field2 = null;
		while (field2 == null && field != null)
		{
			field2 = field.FindChild(child_name, null, true, true, true, '.');
			if (field2 == null)
			{
				field = field.parent;
			}
		}
		return field2;
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x000BC288 File Offset: 0x000BA488
	public virtual void RefreshDefField()
	{
		if (this.field != null)
		{
			DT.Field field = this.field;
			DT.Field hierarchyChildField = this.GetHierarchyChildField("figure_scale");
			Value value = (hierarchyChildField != null) ? hierarchyChildField.Value(0, null, true, true) : Value.Unknown;
			if (value == Value.Unknown)
			{
				this.defScale = 1f;
			}
			else
			{
				this.defScale = value;
			}
			DT.Field hierarchyChildField2 = this.GetHierarchyChildField("figure_scale_to_camera_zoom");
			Value value2 = (hierarchyChildField2 != null) ? hierarchyChildField2.Value(0, null, true, true) : Value.Unknown;
			Value value3 = (hierarchyChildField2 != null) ? hierarchyChildField2.Value(1, null, true, true) : Value.Unknown;
			if (value2 == Value.Unknown)
			{
				this.defScaleCameraZoomedIn = 1f;
			}
			else
			{
				this.defScaleCameraZoomedIn = value2;
			}
			if (value3 == Value.Unknown)
			{
				this.defScaleOnCameraZoomedOut = 1f;
			}
			else
			{
				this.defScaleOnCameraZoomedOut = value3;
			}
			this.UpdateTransfrom();
		}
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x000BC375 File Offset: 0x000BA575
	public virtual void Refresh()
	{
		if (ViewMode.IsPoliticalView())
		{
			this.UpdateVisibilityFromView(ViewMode.current.allowedFigures);
		}
	}

	// Token: 0x04000C00 RID: 3072
	private bool visbility_from_filter = true;

	// Token: 0x04000C01 RID: 3073
	private bool visbility_from_object;

	// Token: 0x04000C02 RID: 3074
	private bool visible_from_view;

	// Token: 0x04000C03 RID: 3075
	protected ViewMode.AllowedFigures allowedType;

	// Token: 0x04000C04 RID: 3076
	public float FixedSizeToCamera = 0.002f;

	// Token: 0x04000C05 RID: 3077
	private float defScale = 1f;

	// Token: 0x04000C06 RID: 3078
	private float defScaleCameraZoomedIn = 1f;

	// Token: 0x04000C07 RID: 3079
	private float defScaleOnCameraZoomedOut = 1f;

	// Token: 0x04000C08 RID: 3080
	protected DT.Field field;

	// Token: 0x04000C09 RID: 3081
	protected PVFigure parent;

	// Token: 0x04000C0A RID: 3082
	protected Camera pvCamera;

	// Token: 0x04000C0B RID: 3083
	protected GameCamera gameCamera;
}
