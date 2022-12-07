using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002FC RID: 764
public class BSGScrollRect : ScrollRect
{
	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06002FFE RID: 12286 RVA: 0x00187803 File Offset: 0x00185A03
	// (set) Token: 0x06002FFF RID: 12287 RVA: 0x0018780B File Offset: 0x00185A0B
	public bool DisableDrag
	{
		get
		{
			return this.m_DisableDrag;
		}
		set
		{
			this.m_DisableDrag = value;
		}
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x00187814 File Offset: 0x00185A14
	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (this.DisableDrag)
		{
			return;
		}
		base.OnBeginDrag(eventData);
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x00187826 File Offset: 0x00185A26
	public override void OnDrag(PointerEventData eventData)
	{
		if (this.DisableDrag)
		{
			return;
		}
		base.OnDrag(eventData);
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x00187838 File Offset: 0x00185A38
	public override void OnEndDrag(PointerEventData eventData)
	{
		if (this.DisableDrag)
		{
			return;
		}
		base.OnEndDrag(eventData);
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x0018784A File Offset: 0x00185A4A
	public override void OnScroll(PointerEventData data)
	{
		Action<PointerEventData> onMouseScroll = this.OnMouseScroll;
		if (onMouseScroll != null)
		{
			onMouseScroll(data);
		}
		base.OnScroll(data);
	}

	// Token: 0x0400205A RID: 8282
	[SerializeField]
	private bool m_DisableDrag;

	// Token: 0x0400205B RID: 8283
	public Action<PointerEventData> OnMouseScroll;
}
