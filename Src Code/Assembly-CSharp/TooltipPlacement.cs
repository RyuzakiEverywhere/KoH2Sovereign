using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DD RID: 733
public static class TooltipPlacement
{
	// Token: 0x06002E57 RID: 11863 RVA: 0x0017F05A File Offset: 0x0017D25A
	public static void AddBlocker(GameObject go, Transform parent = null)
	{
		TooltipPlacement.AddBlocker(go.GetComponent<RectTransform>(), parent);
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x0017F068 File Offset: 0x0017D268
	public static void AddBlocker(RectTransform rt, Transform parent = null)
	{
		if (rt == null)
		{
			return;
		}
		TooltipPlacement.Blocker blocker = new TooltipPlacement.Blocker
		{
			rt = rt,
			parent = parent
		};
		int num = TooltipPlacement.FindBlockerIdx(rt);
		if (num < 0)
		{
			TooltipPlacement.blockers.Add(blocker);
		}
		else
		{
			TooltipPlacement.blockers[num] = blocker;
		}
		TooltipPlacement.dirty = true;
		if (TooltipPlacement.log)
		{
			Debug.Log(string.Format("Added tooltip blocker '{0}'", blocker));
		}
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x0017F0E0 File Offset: 0x0017D2E0
	public static void DelBlocker(RectTransform rt)
	{
		int num = TooltipPlacement.FindBlockerIdx(rt);
		if (num < 0)
		{
			return;
		}
		if (TooltipPlacement.log)
		{
			Debug.Log(string.Format("Deleted tooltip blocker '{0}'", TooltipPlacement.blockers[num]));
		}
		TooltipPlacement.blockers.RemoveAt(num);
		TooltipPlacement.dirty = true;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x0017F130 File Offset: 0x0017D330
	public static void DelBlocker(GameObject go)
	{
		TooltipPlacement.DelBlocker(go.GetComponent<RectTransform>());
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x0017F140 File Offset: 0x0017D340
	public static bool PlaceTooltip(RectTransform rt, Tooltip tooltip, Canvas canvas)
	{
		TooltipPlacement.dirty = false;
		RectTransform rectTransform = (canvas != null) ? canvas.GetComponent<RectTransform>() : null;
		if (rectTransform == null)
		{
			return false;
		}
		TooltipPlacement.rc_canvas = TooltipPlacement.GetRTRect(rectTransform);
		TooltipPlacement.spacing = 8f * rectTransform.lossyScale.x;
		TooltipPlacement.tt_src = tooltip;
		TooltipPlacement.rt_src = tooltip.GetComponent<RectTransform>();
		if (TooltipPlacement.rt_src == null)
		{
			return false;
		}
		TooltipPlacement.rc_src = TooltipPlacement.GetRTRect(TooltipPlacement.rt_src);
		TooltipPlacement.rc_inst = TooltipPlacement.GetRTRect(rt);
		TooltipPlacement.options.Clear();
		TooltipPlacement.processed.Clear();
		TooltipPlacement.RefreshBlockers();
		TooltipPlacement.blockers.Insert(0, new TooltipPlacement.Blocker
		{
			rt = TooltipPlacement.rt_src,
			parent = null,
			active = true,
			rc = TooltipPlacement.rc_src
		});
		if (TooltipPlacement.log)
		{
			string text = string.Format("Placing tooltip, blockers: {0}", TooltipPlacement.blockers.Count);
			for (int i = 0; i < TooltipPlacement.blockers.Count; i++)
			{
				text += string.Format("\n  {0}", TooltipPlacement.blockers[i]);
			}
			Debug.Log(text);
		}
		Vector2 vector = new Vector2(TooltipPlacement.rc_src.center.x - TooltipPlacement.rc_inst.width / 2f, TooltipPlacement.rc_src.center.y - TooltipPlacement.rc_inst.height / 2f);
		TooltipPlacement.rc_inst = new Rect(vector, TooltipPlacement.rc_inst.size);
		TooltipPlacement.Open(vector);
		Rect rect = TooltipPlacement.DecidePlacement();
		TooltipPlacement.blockers.RemoveAt(0);
		TooltipPlacement.options.Clear();
		TooltipPlacement.processed.Clear();
		Vector3 position = rect.min;
		position.z = rt.position.z;
		rt.pivot = Vector2.zero;
		rt.transform.position = position;
		TooltipPlacement.dirty = false;
		return true;
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x0017F346 File Offset: 0x0017D546
	public static void Update()
	{
		if (!TooltipPlacement.dirty)
		{
			TooltipPlacement.RefreshBlockers();
		}
		if (TooltipPlacement.dirty)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null)
			{
				return;
			}
			baseUI.PlaceTooltip();
		}
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x0017F36C File Offset: 0x0017D56C
	private static Rect DecidePlacement()
	{
		Rect rect;
		while (TooltipPlacement.Pop(out rect))
		{
			if (TooltipPlacement.Process(rect))
			{
				return rect;
			}
		}
		return TooltipPlacement.Fit(TooltipPlacement.rc_inst, TooltipPlacement.rc_canvas);
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x0017F39C File Offset: 0x0017D59C
	private static int FindBlockerIdx(RectTransform rt)
	{
		if (rt == null)
		{
			return -1;
		}
		for (int i = 0; i < TooltipPlacement.blockers.Count; i++)
		{
			if (TooltipPlacement.blockers[i].rt == rt)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x0017F3E4 File Offset: 0x0017D5E4
	private static void RefreshBlockers()
	{
		for (int i = TooltipPlacement.blockers.Count - 1; i >= 0; i--)
		{
			TooltipPlacement.Blocker blocker = TooltipPlacement.blockers[i];
			if (blocker.rt == null)
			{
				TooltipPlacement.blockers.RemoveAt(i);
				TooltipPlacement.dirty = true;
			}
			else
			{
				bool activeInHierarchy = blocker.rt.gameObject.activeInHierarchy;
				bool flag = activeInHierarchy != blocker.active;
				blocker.active = activeInHierarchy;
				if (activeInHierarchy)
				{
					Rect rtrect = TooltipPlacement.GetRTRect(blocker.rt);
					if (rtrect != blocker.rc)
					{
						flag = true;
						blocker.rc = rtrect;
					}
				}
				if (flag)
				{
					TooltipPlacement.dirty = true;
					TooltipPlacement.blockers[i] = blocker;
				}
			}
		}
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x0017F4A0 File Offset: 0x0017D6A0
	private static Rect GetRTRect(RectTransform rt)
	{
		rt.GetWorldCorners(TooltipPlacement.s_corners);
		float x = TooltipPlacement.s_corners[0].x;
		float y = TooltipPlacement.s_corners[0].y;
		float width = TooltipPlacement.s_corners[2].x - x;
		float height = TooltipPlacement.s_corners[2].y - y;
		return new Rect(x, y, width, height);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x0017F50C File Offset: 0x0017D70C
	private static float EvalDist(Rect rc1, Rect rc2)
	{
		float num;
		if (rc1.xMax <= rc2.xMin)
		{
			num = rc2.xMin - rc1.xMax;
		}
		else if (rc1.xMin >= rc2.xMax)
		{
			num = rc1.xMin - rc2.xMax;
		}
		else
		{
			num = 0f;
		}
		float num2;
		if (rc1.yMax <= rc2.yMin)
		{
			num2 = rc2.yMin - rc1.yMax;
		}
		else if (rc1.yMin >= rc2.yMax)
		{
			num2 = rc1.yMin - rc2.yMax;
		}
		else
		{
			num2 = 0f;
		}
		float num3 = num + num2;
		if (TooltipPlacement.tt_src.prefer_aligned > 0f)
		{
			Vector2 vector = rc2.center - rc1.center;
			num3 += (Mathf.Abs(vector.x) + Mathf.Abs(vector.y)) * TooltipPlacement.tt_src.prefer_aligned;
		}
		if (num > 0f)
		{
			num3 *= 1f + TooltipPlacement.tt_src.prefer_vertical;
		}
		Vector2 vector2 = rc2.center - rc1.center;
		if ((vector2.y < 0f && TooltipPlacement.tt_src.prefer_top > 0f) || (vector2.y > 0f && TooltipPlacement.tt_src.prefer_top < 0f))
		{
			num3 *= 1f + Mathf.Abs(TooltipPlacement.tt_src.prefer_top);
		}
		if ((vector2.x < 0f && TooltipPlacement.tt_src.prefer_right > 0f) || (vector2.x > 0f && TooltipPlacement.tt_src.prefer_right < 0f))
		{
			num3 *= 1f + Mathf.Abs(TooltipPlacement.tt_src.prefer_right);
		}
		return num3;
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x0017F6D8 File Offset: 0x0017D8D8
	private static bool Intersects(Rect rc1, Rect rc2)
	{
		return rc1.xMax >= rc2.xMin && rc1.xMin <= rc2.xMax && rc1.yMax >= rc2.yMin && rc1.yMin <= rc2.yMax;
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x0017F72E File Offset: 0x0017D92E
	private static Rect MoveUp(Rect rc, Rect rc_from)
	{
		return new Rect(rc.x, rc_from.yMax + TooltipPlacement.spacing, rc.width, rc.height);
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x0017F757 File Offset: 0x0017D957
	private static Rect MoveDown(Rect rc, Rect rc_from)
	{
		return new Rect(rc.x, rc_from.yMin - rc.height - TooltipPlacement.spacing, rc.width, rc.height);
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x0017F788 File Offset: 0x0017D988
	private static Rect MoveRight(Rect rc, Rect rc_from)
	{
		return new Rect(rc_from.xMax + TooltipPlacement.spacing, rc.y, rc.width, rc.height);
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x0017F7B1 File Offset: 0x0017D9B1
	private static Rect MoveLeft(Rect rc, Rect rc_from)
	{
		return new Rect(rc_from.xMin - rc.width - TooltipPlacement.spacing, rc.y, rc.width, rc.height);
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x0017F7E4 File Offset: 0x0017D9E4
	private static Rect Fit(Rect rc, Rect rc_into)
	{
		float x;
		if (rc.xMin < rc_into.xMin || rc.width >= rc_into.width)
		{
			x = rc_into.xMin;
		}
		else if (rc.xMax > rc_into.xMax)
		{
			x = rc_into.xMax - rc.width;
		}
		else
		{
			x = rc.x;
		}
		float y;
		if (rc.yMax > rc_into.yMax || rc.height >= rc_into.height)
		{
			y = rc_into.yMax - rc.height;
		}
		else if (rc.yMin < rc_into.yMin)
		{
			y = rc_into.yMin;
		}
		else
		{
			y = rc.y;
		}
		return new Rect(x, y, rc.width, rc.height);
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x0017F8B0 File Offset: 0x0017DAB0
	private static bool Intersects(Rect rc, TooltipPlacement.Blocker b)
	{
		return b.active && TooltipPlacement.Intersects(rc, b.rc) && (b.parent == null || Common.IsParent(b.parent.gameObject, TooltipPlacement.rt_src.gameObject));
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x0017F908 File Offset: 0x0017DB08
	private static void Open(Vector2 pt)
	{
		if (TooltipPlacement.processed.Contains(pt))
		{
			return;
		}
		TooltipPlacement.processed.Add(pt);
		Rect rc = new Rect(pt, TooltipPlacement.rc_inst.size);
		float dist = TooltipPlacement.EvalDist(TooltipPlacement.rc_src, rc);
		TooltipPlacement.Option item = new TooltipPlacement.Option
		{
			pt = pt,
			dist = dist
		};
		TooltipPlacement.options.Add(item);
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x0017F972 File Offset: 0x0017DB72
	private static void Open(Rect rc)
	{
		rc = TooltipPlacement.Fit(rc, TooltipPlacement.rc_canvas);
		TooltipPlacement.Open(rc.position);
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x0017F990 File Offset: 0x0017DB90
	private static bool Pop(out Rect rc)
	{
		int num = -1;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < TooltipPlacement.options.Count; i++)
		{
			TooltipPlacement.Option option = TooltipPlacement.options[i];
			if (option.dist < num2)
			{
				num = i;
				num2 = option.dist;
			}
		}
		if (num < 0)
		{
			rc = default(Rect);
			return false;
		}
		Vector2 pt = TooltipPlacement.options[num].pt;
		TooltipPlacement.options.RemoveAt(num);
		rc = new Rect(pt, TooltipPlacement.rc_inst.size);
		return true;
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x0017FA1C File Offset: 0x0017DC1C
	private static bool Process(Rect rc)
	{
		bool result = true;
		for (int i = 0; i < TooltipPlacement.blockers.Count; i++)
		{
			TooltipPlacement.Blocker blocker = TooltipPlacement.blockers[i];
			if (TooltipPlacement.Intersects(rc, blocker))
			{
				result = false;
				TooltipPlacement.Open(TooltipPlacement.MoveUp(rc, blocker.rc));
				TooltipPlacement.Open(TooltipPlacement.MoveDown(rc, blocker.rc));
				TooltipPlacement.Open(TooltipPlacement.MoveRight(rc, blocker.rc));
				TooltipPlacement.Open(TooltipPlacement.MoveLeft(rc, blocker.rc));
			}
		}
		return result;
	}

	// Token: 0x04001F5D RID: 8029
	public static bool dirty = false;

	// Token: 0x04001F5E RID: 8030
	private static bool log = false;

	// Token: 0x04001F5F RID: 8031
	private static Rect rc_canvas;

	// Token: 0x04001F60 RID: 8032
	private static Tooltip tt_src;

	// Token: 0x04001F61 RID: 8033
	private static RectTransform rt_src;

	// Token: 0x04001F62 RID: 8034
	private static Rect rc_src;

	// Token: 0x04001F63 RID: 8035
	private static Rect rc_inst;

	// Token: 0x04001F64 RID: 8036
	private static float spacing;

	// Token: 0x04001F65 RID: 8037
	private static List<TooltipPlacement.Blocker> blockers = new List<TooltipPlacement.Blocker>();

	// Token: 0x04001F66 RID: 8038
	private static List<TooltipPlacement.Option> options = new List<TooltipPlacement.Option>();

	// Token: 0x04001F67 RID: 8039
	private static HashSet<Vector2> processed = new HashSet<Vector2>();

	// Token: 0x04001F68 RID: 8040
	private static Vector3[] s_corners = new Vector3[4];

	// Token: 0x02000852 RID: 2130
	private struct Blocker
	{
		// Token: 0x060050B8 RID: 20664 RVA: 0x0023A990 File Offset: 0x00238B90
		public override string ToString()
		{
			string text = string.Format("{0}: ({1}) - ({2}) [{3} x {4}]", new object[]
			{
				Common.ObjPath(this.rt.gameObject),
				this.rc.min,
				this.rc.max,
				this.rc.width,
				this.rc.height
			});
			if (!this.active)
			{
				text = "[Inactive] " + text;
			}
			return text;
		}

		// Token: 0x04003EB7 RID: 16055
		public RectTransform rt;

		// Token: 0x04003EB8 RID: 16056
		public Transform parent;

		// Token: 0x04003EB9 RID: 16057
		public bool active;

		// Token: 0x04003EBA RID: 16058
		public Rect rc;
	}

	// Token: 0x02000853 RID: 2131
	private struct Option
	{
		// Token: 0x060050B9 RID: 20665 RVA: 0x0023AA22 File Offset: 0x00238C22
		public override string ToString()
		{
			return string.Format("[{0}] {1}", this.dist, this.pt);
		}

		// Token: 0x04003EBB RID: 16059
		public Vector2 pt;

		// Token: 0x04003EBC RID: 16060
		public float dist;
	}
}
