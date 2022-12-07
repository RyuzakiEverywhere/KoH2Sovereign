using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000264 RID: 612
public abstract class UIPVFigureIcon : UIPVFigure
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x0600259D RID: 9629 RVA: 0x0014D0EC File Offset: 0x0014B2EC
	// (set) Token: 0x0600259E RID: 9630 RVA: 0x0014D0FF File Offset: 0x0014B2FF
	public UIPVFigureIcon.TransformParams crestParams
	{
		get
		{
			return UIPVFigureIcon.crestParamDefs[this.DefKey(false)];
		}
		set
		{
			UIPVFigureIcon.crestParamDefs[this.DefKey(false)] = value;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600259F RID: 9631 RVA: 0x0014D113 File Offset: 0x0014B313
	// (set) Token: 0x060025A0 RID: 9632 RVA: 0x0014D126 File Offset: 0x0014B326
	public UIPVFigureIcon.TransformParams iconParams
	{
		get
		{
			return UIPVFigureIcon.iconParamDefs[this.DefKey(false)];
		}
		set
		{
			UIPVFigureIcon.iconParamDefs[this.DefKey(false)] = value;
		}
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x0014D13A File Offset: 0x0014B33A
	public override void Init()
	{
		if (this.m_init)
		{
			return;
		}
		this.m_init = true;
		UICommon.FindComponents(this, true);
		base.Init();
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x0014D159 File Offset: 0x0014B359
	public virtual void GetIconSprite(out Sprite sprite, out Sprite hover)
	{
		sprite = null;
		hover = null;
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x0014D161 File Offset: 0x0014B361
	public override void RefreshDefField()
	{
		base.RefreshDefField();
		this.RefreshIconMeasurements();
		this.RefreshCrestMeasurements();
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x0014D178 File Offset: 0x0014B378
	public void RefreshIconMeasurements()
	{
		if (this.icon == null)
		{
			return;
		}
		bool active = true;
		if (!UIPVFigureIcon.iconParamDefs.ContainsKey(this.DefKey(false)))
		{
			DT.Field hierarchyChildField = base.GetHierarchyChildField("icon_measurements");
			if (hierarchyChildField != null)
			{
				float posX = hierarchyChildField.Value(0, null, true, true);
				float posY = hierarchyChildField.Value(1, null, true, true);
				Value value = hierarchyChildField.Value(2, null, true, true);
				Value value2 = hierarchyChildField.Value(3, null, true, true);
				float num = (value == Value.Unknown) ? 1f : value;
				float scaleY = (value2 == Value.Unknown) ? num : value2;
				this.iconParams = new UIPVFigureIcon.TransformParams(posX, posY, num, scaleY);
			}
			else
			{
				active = false;
			}
		}
		this.icon.gameObject.SetActive(active);
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x0014D258 File Offset: 0x0014B458
	public void RefreshIconPosition()
	{
		if (this.icon == null)
		{
			return;
		}
		Vector3 localPosition = new Vector3(this.iconParams.pos.x * (float)(this.IsFlipped() ? -1 : 1), this.iconParams.pos.y, 0f);
		Vector3 localScale = new Vector3(this.iconParams.scale.x * (float)(this.IsFlipped() ? -1 : 1), this.iconParams.scale.y, 1f);
		this.icon.transform.localPosition = localPosition;
		this.icon.transform.localScale = localScale;
	}

	// Token: 0x060025A6 RID: 9638 RVA: 0x0014D30C File Offset: 0x0014B50C
	public void RefreshCrestMeasurements()
	{
		if (this.crest == null)
		{
			return;
		}
		bool active = true;
		if (!UIPVFigureIcon.crestParamDefs.ContainsKey(this.DefKey(false)))
		{
			DT.Field hierarchyChildField = base.GetHierarchyChildField("crest_measurements");
			if (hierarchyChildField != null)
			{
				float posX = hierarchyChildField.Value(0, null, true, true);
				float posY = hierarchyChildField.Value(1, null, true, true);
				Value value = hierarchyChildField.Value(2, null, true, true);
				Value value2 = hierarchyChildField.Value(3, null, true, true);
				float num = (value == Value.Unknown) ? 1f : value;
				float scaleY = (value2 == Value.Unknown) ? num : value2;
				this.crest.gameObject.SetActive(true);
				this.crestParams = new UIPVFigureIcon.TransformParams(posX, posY, num, scaleY);
			}
			else
			{
				active = false;
			}
		}
		this.crest.gameObject.SetActive(active);
	}

	// Token: 0x060025A7 RID: 9639 RVA: 0x0014D3FC File Offset: 0x0014B5FC
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

	// Token: 0x060025A8 RID: 9640 RVA: 0x0014D4A2 File Offset: 0x0014B6A2
	public virtual void RefreshIcon(bool force = false)
	{
		this.GetIconSprite(out this.icon_sprite, out this.icon_sprite_hover);
		this.UpdateIcon();
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x0014D4BC File Offset: 0x0014B6BC
	public override void Refresh()
	{
		base.Refresh();
		this.RefreshIconPosition();
		this.RefreshCrestPosition();
		this.RefreshIcon(false);
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x0014D4D8 File Offset: 0x0014B6D8
	public void UpdateIcon()
	{
		if (this.icon == null)
		{
			return;
		}
		if (this.mouse_in || this.Selected())
		{
			this.icon.overrideSprite = this.icon_sprite_hover;
			return;
		}
		this.icon.overrideSprite = this.icon_sprite;
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x0002C538 File Offset: 0x0002A738
	protected virtual bool Selected()
	{
		return false;
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x0014D527 File Offset: 0x0014B727
	protected override bool AddToGlobalListOnAwake()
	{
		return (!(base.transform.parent != null) || !(global::Common.GetParentComponent<WorldToScreenObject>(base.transform.parent.gameObject) != null)) && base.AddToGlobalListOnAwake();
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x0014D561 File Offset: 0x0014B761
	public override void OnPointerEnter(PointerEventData e)
	{
		base.OnPointerEnter(e);
		this.UpdateIcon();
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x0014D570 File Offset: 0x0014B770
	public override void OnPointerExit(PointerEventData e)
	{
		base.OnPointerExit(e);
		this.UpdateIcon();
	}

	// Token: 0x04001999 RID: 6553
	[UIFieldTarget("id_Icon")]
	protected Image icon;

	// Token: 0x0400199A RID: 6554
	[UIFieldTarget("_Shield")]
	protected UIKingdomIcon crest;

	// Token: 0x0400199B RID: 6555
	private Sprite icon_sprite;

	// Token: 0x0400199C RID: 6556
	private Sprite icon_sprite_hover;

	// Token: 0x0400199D RID: 6557
	protected static Dictionary<string, UIPVFigureIcon.TransformParams> crestParamDefs = new Dictionary<string, UIPVFigureIcon.TransformParams>();

	// Token: 0x0400199E RID: 6558
	protected static Dictionary<string, UIPVFigureIcon.TransformParams> iconParamDefs = new Dictionary<string, UIPVFigureIcon.TransformParams>();

	// Token: 0x0400199F RID: 6559
	private bool m_init;

	// Token: 0x020007BD RID: 1981
	public struct TransformParams
	{
		// Token: 0x06004D9D RID: 19869 RVA: 0x0022F613 File Offset: 0x0022D813
		public TransformParams(float posX, float posY, float scaleX, float scaleY)
		{
			this.pos = new Vector2(posX, posY);
			this.scale = new Vector2(scaleX, scaleY);
		}

		// Token: 0x04003C2D RID: 15405
		public Vector2 pos;

		// Token: 0x04003C2E RID: 15406
		public Vector2 scale;
	}
}
