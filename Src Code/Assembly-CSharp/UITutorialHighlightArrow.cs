using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002E4 RID: 740
public class UITutorialHighlightArrow : MonoBehaviour
{
	// Token: 0x06002EF1 RID: 12017 RVA: 0x00182C98 File Offset: 0x00180E98
	private void OnEnable()
	{
		if (this.rt == null)
		{
			this.rt = base.GetComponent<RectTransform>();
		}
		if (this.img == null)
		{
			this.img = base.GetComponent<Image>();
		}
		if (this.rt == null || this.img == null)
		{
			base.enabled = false;
			return;
		}
		if (this.owner_highlight == null)
		{
			this.owner_highlight = base.transform.parent.gameObject;
		}
		this.Refresh();
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x00182D27 File Offset: 0x00180F27
	private void Update()
	{
		this.Refresh();
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x00182D30 File Offset: 0x00180F30
	public void Refresh()
	{
		if (!this.owner_highlight.activeInHierarchy)
		{
			if (this.owner_highlight != null)
			{
				base.transform.SetParent(this.owner_highlight.transform);
			}
			return;
		}
		if (base.transform.parent == this.owner_highlight.transform && Tutorial.cur_message_wnd != null)
		{
			base.transform.SetParent(Tutorial.cur_message_wnd.transform);
			base.transform.SetAsFirstSibling();
		}
		if (!this.FindEndpoints())
		{
			this.img.enabled = false;
			return;
		}
		this.CalcRT();
		this.img.enabled = true;
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x00182DE0 File Offset: 0x00180FE0
	private bool FindEndpoints()
	{
		if (Tutorial.active)
		{
			return false;
		}
		MessageWnd cur_message_wnd = Tutorial.cur_message_wnd;
		this.rt_src = (((cur_message_wnd != null) ? cur_message_wnd.transform : null) as RectTransform);
		this.rt_tgt = (this.owner_highlight.transform.parent as RectTransform);
		return this.rt_src != null && this.rt_tgt != null;
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x00182E4C File Offset: 0x0018104C
	private void CalcRT()
	{
		Rect worldRect = UICommon.GetWorldRect(this.rt_src);
		Rect worldRect2 = UICommon.GetWorldRect(this.rt_tgt);
		Vector2 vector = worldRect.center;
		Vector2 vector2 = worldRect2.center;
		vector = this.ClipLineAABB(vector2, vector, worldRect);
		vector2 = this.ClipLineAABB(vector, vector2, worldRect2);
		vector = this.rt_src.InverseTransformPoint(vector);
		vector2 = this.rt_src.InverseTransformPoint(vector2);
		base.transform.localPosition = vector;
		Vector2 to = vector2 - vector;
		float z = Vector2.SignedAngle(Vector2.right, to);
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
		this.rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, to.magnitude);
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x00182F1C File Offset: 0x0018111C
	private Vector2 ClipLineAABB(Vector2 pt_src, Vector2 pt_tgt, Rect rc)
	{
		float num;
		if (pt_src.x >= rc.xMin && pt_src.x <= rc.xMax)
		{
			num = this.Intersect(pt_src.y, pt_tgt.y, rc.yMin, rc.yMax);
		}
		else if (pt_src.y >= rc.yMin && pt_src.y <= rc.yMax)
		{
			num = this.Intersect(pt_src.x, pt_tgt.x, rc.xMin, rc.xMax);
		}
		else
		{
			float val = this.Intersect(pt_src.x, pt_tgt.x, rc.xMin, rc.xMax);
			float val2 = this.Intersect(pt_src.y, pt_tgt.y, rc.yMin, rc.yMax);
			num = Math.Max(val, val2);
		}
		if (num < 0f || num > 1f)
		{
			return pt_src;
		}
		return pt_src + num * (pt_tgt - pt_src);
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x0018301B File Offset: 0x0018121B
	private float Intersect(float src, float tgt, float min, float max)
	{
		return (((src <= min) ? min : max) - src) / (tgt - src);
	}

	// Token: 0x04001FB7 RID: 8119
	private RectTransform rt;

	// Token: 0x04001FB8 RID: 8120
	private Image img;

	// Token: 0x04001FB9 RID: 8121
	private RectTransform rt_src;

	// Token: 0x04001FBA RID: 8122
	private RectTransform rt_tgt;

	// Token: 0x04001FBB RID: 8123
	private GameObject owner_highlight;
}
