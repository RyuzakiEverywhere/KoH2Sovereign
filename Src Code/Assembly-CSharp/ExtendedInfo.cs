using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class ExtendedInfo : MonoBehaviour
{
	// Token: 0x060021A1 RID: 8609 RVA: 0x00131294 File Offset: 0x0012F494
	public void SetTooltipText(string ttText)
	{
		Tooltip.Get(base.gameObject, true).SetText(ttText, null, null);
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x001312AC File Offset: 0x0012F4AC
	public static void Build(GameObject prorotype, RectTransform container, string info)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(prorotype, container);
		ExtendedInfo extendedInfo = gameObject.AddComponent<ExtendedInfo>();
		UICommon.SetAligment(gameObject.transform as RectTransform, TextAnchor.MiddleLeft);
		gameObject.transform.Translate(-30f, 7f, 0f);
		extendedInfo.SetTooltipText(info);
	}

	// Token: 0x04001694 RID: 5780
	[UIFieldTarget("id_Tax_10")]
	private BSGButton m_ButtonTax_10;

	// Token: 0x04001695 RID: 5781
	[UIFieldTarget("id_Tax_20")]
	private BSGButton m_ButtonTax_20;
}
