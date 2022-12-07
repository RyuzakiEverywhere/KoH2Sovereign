using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class UIPopupWindow : MonoBehaviour
{
	// Token: 0x06003011 RID: 12305 RVA: 0x000023FD File Offset: 0x000005FD
	public void FitContent()
	{
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000448AF File Offset: 0x00042AAF
	public static UIPopupWindow Create(GameObject payload)
	{
		return null;
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x0018791C File Offset: 0x00185B1C
	public static UIPopupWindow CreateEmpty(GameObject host, Rect size)
	{
		GameObject gameObject = new GameObject("PopupHost", new Type[]
		{
			typeof(RectTransform)
		});
		gameObject.transform.SetParent(host.transform, false);
		UIPopupWindow uipopupWindow = gameObject.AddComponent<UIPopupWindow>();
		uipopupWindow.m_RectTransform = gameObject.GetComponent<RectTransform>();
		uipopupWindow.m_RectTransform.sizeDelta = new Vector2(size.width, size.height);
		return uipopupWindow;
	}

	// Token: 0x04002060 RID: 8288
	private RectTransform m_RectTransform;
}
