using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001FB RID: 507
public class DragWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	// Token: 0x06001ED5 RID: 7893 RVA: 0x0011DAD8 File Offset: 0x0011BCD8
	private void Start()
	{
		this.m_transform = base.GetComponent<RectTransform>();
		Vector3 position;
		if (DragWindow.dict.TryGetValue(this.persist, out position))
		{
			this.m_transform.position = position;
		}
		this.EnsureInScreen();
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x0011DB17 File Offset: 0x0011BD17
	private void OnEnable()
	{
		this.m_transform = base.GetComponent<RectTransform>();
		if (this.hotspot == null)
		{
			this.hotspot = this.m_transform;
		}
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x0011DB40 File Offset: 0x0011BD40
	public void OnDrag(PointerEventData eventData)
	{
		this.m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
		this.EnsureInScreen();
		if (!string.IsNullOrEmpty(this.persist))
		{
			DragWindow.dict[this.persist] = this.m_transform.position;
		}
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x0011DBAC File Offset: 0x0011BDAC
	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Middle)
		{
			return;
		}
		if (eventData.button == PointerEventData.InputButton.Right && BaseUI.IgnoreRightClick())
		{
			return;
		}
		base.transform.SetAsLastSibling();
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x0011DBD4 File Offset: 0x0011BDD4
	private void EnsureInScreen()
	{
		Vector3[] array = new Vector3[4];
		this.m_transform.GetWorldCorners(array);
		float x = array[0].x;
		float y = array[2].y;
		float num = array[2].x - array[0].x;
		float num2 = array[2].y - array[0].y;
		float num3 = num * (1f - this.KeepInScreen);
		float num4 = num2 * (1f - this.KeepInScreen);
		float num5 = (float)Screen.height;
		float num6 = (float)Screen.width;
		Vector3 position = this.m_transform.position;
		if (x + num3 < 0f)
		{
			position.x -= x + num3;
		}
		if (y - num4 > num5)
		{
			position.y -= y - num5 - num4;
		}
		if (x + num - num3 > num6)
		{
			position.x -= x + num - num3 - num6;
		}
		if (y - num2 + num4 < 0f)
		{
			position.y -= y - num2 + num4;
		}
		this.m_transform.position = position;
	}

	// Token: 0x04001431 RID: 5169
	public RectTransform hotspot;

	// Token: 0x04001432 RID: 5170
	public string persist = "";

	// Token: 0x04001433 RID: 5171
	public float KeepInScreen = 0.75f;

	// Token: 0x04001434 RID: 5172
	private RectTransform m_transform;

	// Token: 0x04001435 RID: 5173
	private static Dictionary<string, Vector3> dict = new Dictionary<string, Vector3>();
}
