using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C4 RID: 452
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class UIBattleViewCapturePointsContainer : MonoBehaviour
{
	// Token: 0x06001AC5 RID: 6853 RVA: 0x00102F05 File Offset: 0x00101105
	public void Initialize(float uiCapturePointWidth)
	{
		this.m_uiCapturePointWidth = uiCapturePointWidth;
		this.m_layoutGroup = base.GetComponent<HorizontalLayoutGroup>();
		this.m_currentSpacing = this.m_maxSpacing;
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x00102F26 File Offset: 0x00101126
	public void AddPoint(UICapturePoint point)
	{
		point.transform.SetParent(base.transform, false);
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x00102F3C File Offset: 0x0010113C
	public void Refresh()
	{
		float width = this.m_layoutGroup.GetComponent<RectTransform>().rect.width;
		int num = (int)(width / this.m_uiCapturePointWidth);
		int childCount = this.m_layoutGroup.transform.childCount;
		if (childCount <= num)
		{
			this.m_currentSpacing = Common.map((float)childCount, 2f, (float)num, this.m_maxSpacing, 0f, false);
		}
		else
		{
			float num2 = (float)childCount * this.m_uiCapturePointWidth;
			float num3 = width - num2;
			this.m_currentSpacing = num3 / (float)(childCount - 1);
		}
		if (this.m_currentSpacing >= this.m_maxSpacing)
		{
			this.m_currentSpacing = this.m_maxSpacing;
		}
		this.m_layoutGroup.spacing = this.m_currentSpacing;
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x00102FEC File Offset: 0x001011EC
	public void Clear()
	{
		Common.DestroyChildren(base.transform);
	}

	// Token: 0x0400116B RID: 4459
	[SerializeField]
	private float m_maxSpacing = 5f;

	// Token: 0x0400116C RID: 4460
	private float m_currentSpacing = 5f;

	// Token: 0x0400116D RID: 4461
	private float m_uiCapturePointWidth = 70f;

	// Token: 0x0400116E RID: 4462
	private HorizontalLayoutGroup m_layoutGroup;
}
