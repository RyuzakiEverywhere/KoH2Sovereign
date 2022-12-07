using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C9 RID: 713
public class UITableCell : Hotspot
{
	// Token: 0x06002CB7 RID: 11447 RVA: 0x0014F481 File Offset: 0x0014D681
	public override void Awake()
	{
		base.Awake();
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x00174DE6 File Offset: 0x00172FE6
	public Image GetCellBackground()
	{
		return this.cellColor;
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x00174DF0 File Offset: 0x00172FF0
	public void SetBorderSize(float left, float right, float top, float bottom)
	{
		if (this.cell == null)
		{
			return;
		}
		RectTransform component = this.cell.GetComponent<RectTransform>();
		if (component != null)
		{
			component.offsetMin = new Vector2(left, top);
			component.offsetMax = new Vector2(-right, -bottom);
		}
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x00174E3E File Offset: 0x0017303E
	public void SetCellBackground(Color c)
	{
		if (this.cellColor)
		{
			this.cellColor.color = c;
		}
	}

	// Token: 0x04001E79 RID: 7801
	public Color mouseOverColor;

	// Token: 0x04001E7A RID: 7802
	public Color selectedColor;

	// Token: 0x04001E7B RID: 7803
	public Color normalColor;

	// Token: 0x04001E7C RID: 7804
	[UIFieldTarget("id_Cell")]
	protected Image cell;

	// Token: 0x04001E7D RID: 7805
	[UIFieldTarget("id_CellColor")]
	protected Image cellColor;

	// Token: 0x04001E7E RID: 7806
	[UIFieldTarget("id_Content")]
	protected GameObject Content;
}
