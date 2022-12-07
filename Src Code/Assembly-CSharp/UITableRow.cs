using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002CA RID: 714
public class UITableRow : Hotspot
{
	// Token: 0x06002CBC RID: 11452 RVA: 0x00174E5C File Offset: 0x0017305C
	public override void Awake()
	{
		base.Awake();
		UICommon.FindComponents(this, false);
		if (this.selectedRow != null)
		{
			this.selectedRow.enabled = false;
		}
		if (this.hoverRow != null)
		{
			this.hoverRow.enabled = false;
		}
		if (this.background != null)
		{
			this.background.color = this.normalColor;
		}
		this.cells = new List<UITableCell>(base.GetComponentsInChildren<UITableCell>());
		this.UpdateHighlight();
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x00174EE0 File Offset: 0x001730E0
	public virtual void Select(bool select)
	{
		this.selected = select;
		this.UpdateHighlight();
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x00174EEF File Offset: 0x001730EF
	public void SetColor(Color normalColor, Color mouseOverColor, Color selectedColor)
	{
		this.normalColor = normalColor;
		this.mouseOverColor = mouseOverColor;
		this.selectedColor = selectedColor;
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x00174F06 File Offset: 0x00173106
	public override void OnPointerDown(PointerEventData e)
	{
		base.OnPointerDown(e);
		this.Select(true);
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
		if (this.OnPointerDownEvent != null)
		{
			this.OnPointerDownEvent.Invoke(e, this);
		}
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x00174F41 File Offset: 0x00173141
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (this.OnPointerExitEvent != null)
		{
			this.OnPointerExitEvent.Invoke(eventData, this);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x00174F65 File Offset: 0x00173165
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (this.OnPointerEnterEvent != null)
		{
			this.OnPointerEnterEvent.Invoke(eventData, this);
		}
		this.UpdateHighlight();
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x00174F8C File Offset: 0x0017318C
	private void UpdateHighlight()
	{
		Color color = this.selected ? this.selectedColor : (this.mouse_in ? this.mouseOverColor : this.normalColor);
		if (this.selectedRow != null)
		{
			this.selectedRow.enabled = this.selected;
			this.selectedRow.color = color;
		}
		if (this.hoverRow != null)
		{
			this.hoverRow.enabled = (!this.selected && this.mouse_in);
			this.hoverRow.color = color;
		}
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x00175021 File Offset: 0x00173221
	public override void OnPointerUp(PointerEventData e)
	{
		base.OnPointerUp(e);
		if (this.OnPointerUpEvent != null)
		{
			this.OnPointerUpEvent.Invoke(e, this);
		}
		if (e.clickCount > 1 && this.OnFocus != null)
		{
			this.OnFocus(this, e);
		}
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x0017505E File Offset: 0x0017325E
	public void SetBorderSize(float borderSize, bool refresh = true)
	{
		this.borderSize = borderSize;
		if (refresh)
		{
			this.RefreshBorderSize();
		}
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x00175070 File Offset: 0x00173270
	public void RefreshBorderSize()
	{
		int count = this.cells.Count;
		for (int i = 0; i < count; i++)
		{
			UITableCell uitableCell = this.cells[i];
			float num = this.borderSize / 2f;
			float left = (i == 0) ? this.borderSize : num;
			float right = (i == count - 1) ? this.borderSize : num;
			uitableCell.SetBorderSize(left, right, this.borderSize, this.borderSize);
		}
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x001750E0 File Offset: 0x001732E0
	public List<UITableCell> GetCells()
	{
		return this.cells;
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x001750E8 File Offset: 0x001732E8
	public void Refresh()
	{
		this.cells = new List<UITableCell>(base.GetComponentsInChildren<UITableCell>());
		this.RefreshBorderSize();
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x00175101 File Offset: 0x00173301
	public void AddCell(UITableCell tc, bool refresh = true)
	{
		this.cells.Add(tc);
		tc.transform.SetParent(base.transform, false);
		if (refresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x04001E7F RID: 7807
	[UIFieldTarget("id_Background")]
	protected Image background;

	// Token: 0x04001E80 RID: 7808
	[UIFieldTarget("id_SelectedRow")]
	protected Image selectedRow;

	// Token: 0x04001E81 RID: 7809
	[UIFieldTarget("id_HoverRow")]
	protected Image hoverRow;

	// Token: 0x04001E82 RID: 7810
	protected List<UITableCell> cells = new List<UITableCell>();

	// Token: 0x04001E83 RID: 7811
	protected bool selected;

	// Token: 0x04001E84 RID: 7812
	public RowEvent OnPointerDownEvent;

	// Token: 0x04001E85 RID: 7813
	public RowEvent OnPointerUpEvent;

	// Token: 0x04001E86 RID: 7814
	public RowEvent OnPointerEnterEvent;

	// Token: 0x04001E87 RID: 7815
	public RowEvent OnPointerExitEvent;

	// Token: 0x04001E88 RID: 7816
	public UITableRow.OnSelectedEvent OnSelected;

	// Token: 0x04001E89 RID: 7817
	public UITableRow.OnDoubleClickEvent OnFocus;

	// Token: 0x04001E8A RID: 7818
	public float borderSize = 1f;

	// Token: 0x04001E8B RID: 7819
	public Color mouseOverColor = new Color(0.21f, 0.21f, 0.21f, 0.42f);

	// Token: 0x04001E8C RID: 7820
	public Color selectedColor = new Color(0.21f, 0.21f, 0.21f, 0.66f);

	// Token: 0x04001E8D RID: 7821
	public Color normalColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x02000823 RID: 2083
	// (Invoke) Token: 0x06004FF2 RID: 20466
	public delegate bool OnSelectedEvent(UITableRow tr, PointerEventData eventData);

	// Token: 0x02000824 RID: 2084
	// (Invoke) Token: 0x06004FF6 RID: 20470
	public delegate bool OnDoubleClickEvent(UITableRow tr, PointerEventData eventData);
}
