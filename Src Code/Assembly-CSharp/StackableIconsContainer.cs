using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C1 RID: 705
[RequireComponent(typeof(RectTransform))]
public class StackableIconsContainer : MonoBehaviour
{
	// Token: 0x06002C3B RID: 11323 RVA: 0x0017285E File Offset: 0x00170A5E
	public void Init()
	{
		if (this.initted)
		{
			return;
		}
		this.initted = true;
		this.rt = base.GetComponent<RectTransform>();
		Canvas componentInParent = base.GetComponentInParent<Canvas>();
		this.canvas_cam = ((componentInParent != null) ? componentInParent.worldCamera : null);
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x00172894 File Offset: 0x00170A94
	public void Refresh()
	{
		this.Init();
		this.SetPicked(-1);
		this.size = this.GetSize(this.rt, default(Vector2));
		this.UpdateChildren();
		this.BuildLayout();
		this.UpdatePicker();
		this.dirty = false;
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x001728E4 File Offset: 0x00170AE4
	private void UpdateChildren()
	{
		this.items.Clear();
		this.total_size = default(Vector2);
		int childCount = this.rt.childCount;
		for (int i = 0; i < childCount; i++)
		{
			RectTransform component = this.rt.GetChild(i).GetComponent<RectTransform>();
			if (!(component == null) && !(component.gameObject == null) && component.gameObject.activeSelf)
			{
				Vector2 vector = this.GetSize(component, this.ForceChildSize);
				if (vector.x != 0f)
				{
					this.items.Add(new StackableIconsContainer.Item
					{
						rt = component,
						size = vector,
						sibling_idx = i
					});
					this.total_size.x = this.total_size.x + vector.x;
					if (vector.y > this.total_size.y)
					{
						this.total_size.y = vector.y;
					}
				}
			}
		}
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x001729E8 File Offset: 0x00170BE8
	private void BuildLayout()
	{
		int count = this.items.Count;
		if (count == 0)
		{
			return;
		}
		float num = this.size.x - this.total_size.x;
		int num2 = count - 1;
		if (num2 == 0)
		{
			this.cur_spacing = 0f;
		}
		else
		{
			this.cur_spacing = num / (float)num2;
			if (this.cur_spacing > (float)this.MaxSpacing)
			{
				this.cur_spacing = (float)this.MaxSpacing;
			}
		}
		float num3 = this.CalcHOfs();
		for (int i = 0; i < count; i++)
		{
			StackableIconsContainer.Item item = this.items[i];
			if (this.RightToLeft)
			{
				num3 -= item.size.x;
			}
			this.Place(item, num3);
			if (this.RightToLeft)
			{
				num3 -= this.cur_spacing;
			}
			else
			{
				num3 += item.size.x + this.cur_spacing;
			}
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x00172AC8 File Offset: 0x00170CC8
	private void Place(StackableIconsContainer.Item item, float hofs)
	{
		RectTransform rectTransform = item.rt;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.pivot = Vector2.zero;
		float num = this.CalcVOfs(item.size.y);
		rectTransform.offsetMin = new Vector2(hofs, num);
		rectTransform.offsetMax = new Vector2(hofs + item.size.x, num + item.size.y);
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x00172B40 File Offset: 0x00170D40
	private float CalcHOfs()
	{
		if (this.items.Count == 0)
		{
			return 0f;
		}
		if (this.total_size.x < this.size.x)
		{
			int num = this.items.Count - 1;
			float num2 = this.size.x - this.total_size.x - (float)num * this.cur_spacing;
			switch (this.HorizontalAlignment)
			{
			case StackableIconsContainer.HAlign.Left:
				if (!this.RightToLeft)
				{
					return 0f;
				}
				return this.size.x - num2;
			case StackableIconsContainer.HAlign.Center:
				if (!this.RightToLeft)
				{
					return num2 * 0.5f;
				}
				return this.size.x - num2 * 0.5f;
			case StackableIconsContainer.HAlign.Right:
				if (!this.RightToLeft)
				{
					return num2;
				}
				return this.size.x;
			default:
				return 0f;
			}
		}
		else
		{
			if (!this.RightToLeft)
			{
				return 0f;
			}
			return this.size.x;
		}
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x00172C3C File Offset: 0x00170E3C
	private float CalcVOfs(float height)
	{
		switch (this.VerticalAlignment)
		{
		case StackableIconsContainer.VAlign.Top:
			return this.size.y - height;
		case StackableIconsContainer.VAlign.Center:
			return (this.size.y - height) * 0.5f;
		case StackableIconsContainer.VAlign.Bottom:
			return 0f;
		default:
			return 0f;
		}
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x00172C94 File Offset: 0x00170E94
	private Vector2 GetSize(RectTransform rt, Vector2 force_size = default(Vector2))
	{
		Vector2 result = rt.rect.size;
		LayoutElement component = rt.GetComponent<LayoutElement>();
		if (component != null)
		{
			if (component.ignoreLayout)
			{
				result.Set(0f, 0f);
				return result;
			}
			if (component.preferredWidth > 0f)
			{
				result.x = component.preferredWidth;
			}
			if (component.preferredHeight > 0f)
			{
				result.y = component.preferredHeight;
			}
		}
		if (force_size.x > 0f)
		{
			result.x = force_size.x;
		}
		if (force_size.y > 0f)
		{
			result.y = force_size.y;
		}
		return result;
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x00172D44 File Offset: 0x00170F44
	private Vector2 GetMousePos()
	{
		if (Application.isPlaying)
		{
			Vector2 screenPoint = Input.mousePosition;
			Vector2 vector;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rt, screenPoint, this.canvas_cam, out vector))
			{
				Vector2 vector2 = this.rt.pivot;
				if (vector2 == Vector2.zero)
				{
					return vector;
				}
				Vector2 b = this.rt.rect.size;
				vector2 *= b;
				vector += vector2;
				return vector;
			}
		}
		return new Vector2(-1000f, -1000f);
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x00172DCC File Offset: 0x00170FCC
	private void SetPicked(int idx)
	{
		if (this.picked_idx == idx)
		{
			return;
		}
		if (this.picked_idx >= 0 && this.picked_idx < this.items.Count)
		{
			StackableIconsContainer.Item item = this.items[this.picked_idx];
			if (item.rt != null && item.rt.parent == this.rt)
			{
				item.rt.SetSiblingIndex(item.sibling_idx);
			}
		}
		this.picked_idx = idx;
		if (idx < 0)
		{
			this.picked_rt = null;
			return;
		}
		this.picked_rt = this.items[this.picked_idx].rt;
		this.picked_rt.SetAsLastSibling();
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x00172E84 File Offset: 0x00171084
	private void UpdatePicker()
	{
		if (!this.PopOutHovered || this.cur_spacing >= 0f)
		{
			this.SetPicked(-1);
			return;
		}
		this.mouse_pos = this.GetMousePos();
		if (this.mouse_pos.x < 0f || this.mouse_pos.y < 0f || this.mouse_pos.x > this.size.x || this.mouse_pos.y > this.size.y)
		{
			this.SetPicked(-1);
			return;
		}
		int picked = -1;
		float num = float.MaxValue;
		for (int i = 0; i < this.items.Count; i++)
		{
			StackableIconsContainer.Item item = this.items[i];
			Vector2 offsetMin = item.rt.offsetMin;
			Vector2 offsetMax = item.rt.offsetMax;
			if (this.mouse_pos.x >= offsetMin.x && this.mouse_pos.y >= offsetMin.y && this.mouse_pos.x <= offsetMax.x && this.mouse_pos.y <= offsetMax.y)
			{
				float num2 = (offsetMin.x + offsetMax.x) * 0.5f - this.mouse_pos.x;
				if (num2 < 0f)
				{
					num2 = -num2;
				}
				if (num2 < num)
				{
					picked = i;
					num = num2;
				}
			}
		}
		this.SetPicked(picked);
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x00172FF0 File Offset: 0x001711F0
	private bool CheckSizeChanged()
	{
		return this.GetSize(this.rt, default(Vector2)) != this.size;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x00173022 File Offset: 0x00171222
	private int RemapIdx(int idx)
	{
		if (this.picked_idx < 0)
		{
			return idx;
		}
		if (idx < this.picked_idx)
		{
			return idx;
		}
		if (idx == this.items.Count - 1)
		{
			return this.picked_idx;
		}
		return idx + 1;
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x00173054 File Offset: 0x00171254
	private bool CheckChildrenChanged()
	{
		int childCount = this.rt.childCount;
		int num = 0;
		for (int i = 0; i < childCount; i++)
		{
			RectTransform component = this.rt.GetChild(i).GetComponent<RectTransform>();
			if (!(component == null) && !(component.gameObject == null) && component.gameObject.activeSelf)
			{
				int num2 = this.RemapIdx(num);
				if (num2 < 0 || num2 >= this.items.Count)
				{
					return true;
				}
				StackableIconsContainer.Item item = this.items[num2];
				if (component != item.rt)
				{
					return true;
				}
				if (this.GetSize(component, this.ForceChildSize) != item.size)
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x00173118 File Offset: 0x00171318
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDisable()
	{
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x00173120 File Offset: 0x00171320
	private void OnValidate()
	{
		this.dirty = true;
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x00173129 File Offset: 0x00171329
	private void Update()
	{
		if (!this.dirty)
		{
			this.dirty = this.CheckSizeChanged();
		}
		if (!this.dirty)
		{
			this.dirty = this.CheckChildrenChanged();
		}
		if (!this.dirty)
		{
			this.UpdatePicker();
			return;
		}
		this.Refresh();
	}

	// Token: 0x04001E2D RID: 7725
	public int MaxSpacing;

	// Token: 0x04001E2E RID: 7726
	public Vector2 ForceChildSize = Vector2.zero;

	// Token: 0x04001E2F RID: 7727
	public StackableIconsContainer.VAlign VerticalAlignment = StackableIconsContainer.VAlign.Center;

	// Token: 0x04001E30 RID: 7728
	public StackableIconsContainer.HAlign HorizontalAlignment;

	// Token: 0x04001E31 RID: 7729
	public bool RightToLeft;

	// Token: 0x04001E32 RID: 7730
	public bool PopOutHovered = true;

	// Token: 0x04001E33 RID: 7731
	private List<StackableIconsContainer.Item> items = new List<StackableIconsContainer.Item>();

	// Token: 0x04001E34 RID: 7732
	private bool initted;

	// Token: 0x04001E35 RID: 7733
	private RectTransform rt;

	// Token: 0x04001E36 RID: 7734
	private Vector2 size;

	// Token: 0x04001E37 RID: 7735
	private Vector2 total_size;

	// Token: 0x04001E38 RID: 7736
	private float cur_spacing;

	// Token: 0x04001E39 RID: 7737
	private bool dirty = true;

	// Token: 0x04001E3A RID: 7738
	public Vector2 mouse_pos;

	// Token: 0x04001E3B RID: 7739
	public Camera canvas_cam;

	// Token: 0x04001E3C RID: 7740
	public int picked_idx = -1;

	// Token: 0x04001E3D RID: 7741
	public RectTransform picked_rt;

	// Token: 0x04001E3E RID: 7742
	private static Vector3[] corners = new Vector3[4];

	// Token: 0x02000815 RID: 2069
	public enum HAlign
	{
		// Token: 0x04003DAE RID: 15790
		Left,
		// Token: 0x04003DAF RID: 15791
		Center,
		// Token: 0x04003DB0 RID: 15792
		Right
	}

	// Token: 0x02000816 RID: 2070
	public enum VAlign
	{
		// Token: 0x04003DB2 RID: 15794
		Top,
		// Token: 0x04003DB3 RID: 15795
		Center,
		// Token: 0x04003DB4 RID: 15796
		Bottom
	}

	// Token: 0x02000817 RID: 2071
	[Serializable]
	public struct Item
	{
		// Token: 0x06004F89 RID: 20361 RVA: 0x00235CD4 File Offset: 0x00233ED4
		public override string ToString()
		{
			return string.Format("[{0}] {1} ({2}, {3})", new object[]
			{
				this.sibling_idx,
				this.rt,
				this.size.x,
				this.size.y
			});
		}

		// Token: 0x04003DB5 RID: 15797
		public RectTransform rt;

		// Token: 0x04003DB6 RID: 15798
		public Vector2 size;

		// Token: 0x04003DB7 RID: 15799
		public int sibling_idx;
	}
}
