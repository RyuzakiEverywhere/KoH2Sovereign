using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000207 RID: 519
public class IconsBar : MonoBehaviour
{
	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001F90 RID: 8080 RVA: 0x0012372A File Offset: 0x0012192A
	public int NumIcons
	{
		get
		{
			List<IconsBar.IconInfo> list = this.icons;
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x00123740 File Offset: 0x00121940
	public GameObject AddIcon(MessageIcon.CreateParams pars)
	{
		GameObject gameObject = pars.def_field.GetValue("icon_prefab", null, true, true, true, '.').obj_val as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		if (pars.playSound)
		{
			BaseUI.PlaySoundEvent(pars.def_field.GetString("sound_effect", pars.vars, "", true, true, true, '.'), null);
			string @string = pars.def_field.GetString("voice_line", pars.vars, "", true, true, true, '.');
			if (!string.IsNullOrEmpty(@string))
			{
				BaseUI.PlayVoiceEvent(@string, pars.voiceVars ?? pars.vars);
			}
		}
		this.Init();
		GameObject gameObject2 = global::Common.Spawn(gameObject, false, false);
		RectTransform component = gameObject2.GetComponent<RectTransform>();
		if (component == null)
		{
			UnityEngine.Object.DestroyImmediate(gameObject2);
			return null;
		}
		IconsBar.IconInfo iconInfo = new IconsBar.IconInfo();
		iconInfo.pars = pars;
		iconInfo.rt = component;
		iconInfo.stack_group = pars.def_field.GetString("stack_group", null, "", true, true, true, '.');
		iconInfo.separate_stack = pars.def_field.GetBool("separate_stack", null, false, true, true, true, '.');
		iconInfo.timestamp = (long)UnityEngine.Time.frameCount;
		iconInfo.stack_priority = pars.def_field.GetInt("stack_priority", null, 0, true, true, true, '.');
		IconsBar.IconInfo iconInfo2 = iconInfo;
		DT.Field soundsDef = BaseUI.soundsDef;
		iconInfo2.sound_effect = ((soundsDef != null) ? soundsDef.GetString("notification_message", null, "", true, true, true, '.') : null);
		int i;
		for (i = 0; i < this.icons.Count; i++)
		{
			IconsBar.IconInfo icon = this.icons[i];
			if (this.CanStack(iconInfo, icon))
			{
				while (i < this.icons.Count)
				{
					IconsBar.IconInfo iconInfo3 = this.icons[i];
					if (!this.CanStack(iconInfo, iconInfo3) || iconInfo.stack_priority < iconInfo3.stack_priority)
					{
						break;
					}
					i++;
				}
				break;
			}
		}
		if (i >= this.icons.Count)
		{
			this.icons.Add(iconInfo);
		}
		else
		{
			this.icons.Insert(i, iconInfo);
		}
		this.InitIcon(iconInfo);
		this.Recalc();
		return gameObject2;
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x00123964 File Offset: 0x00121B64
	private int FindIndex(GameObject obj)
	{
		for (int i = 0; i < this.icons.Count; i++)
		{
			if (this.icons[i].rt.gameObject == obj)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x001239A8 File Offset: 0x00121BA8
	public void DelIcon(GameObject obj, bool open_next_in_stack)
	{
		int num = this.FindIndex(obj);
		if (num < 0)
		{
			return;
		}
		this.icons.RemoveAt(num);
		if (open_next_in_stack && num < this.icons.Count)
		{
			IconsBar.IconInfo iconInfo = this.icons[num];
			if (iconInfo.stack > 0)
			{
				MessageIcon component = iconInfo.rt.GetComponent<MessageIcon>();
				if (component != null)
				{
					component.OnClick(null);
				}
			}
		}
		this.Recalc();
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x00123A18 File Offset: 0x00121C18
	public void DelStack(GameObject obj, bool open_non_dismissed)
	{
		int num = this.FindIndex(obj);
		if (num < 0)
		{
			return;
		}
		int num2 = num - this.icons[num].stack;
		while (num + 1 < this.icons.Count)
		{
			IconsBar.IconInfo iconInfo = this.icons[num + 1];
			if (iconInfo.stack == 0)
			{
				break;
			}
			if (iconInfo.pars.def_field != null && !iconInfo.pars.def_field.GetBool("allow_stack_dismiss", null, true, true, true, true, '.'))
			{
				if (!open_non_dismissed)
				{
					break;
				}
				MessageIcon component = iconInfo.rt.GetComponent<MessageIcon>();
				if (component == null)
				{
					break;
				}
				component.OnClick(null);
				break;
			}
			else
			{
				num++;
			}
		}
		for (int i = num2; i <= num; i++)
		{
			global::Common.DestroyObj(this.icons[i].rt.gameObject);
		}
		this.icons.RemoveRange(num2, num - num2 + 1);
		this.Recalc();
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x00123AF8 File Offset: 0x00121CF8
	public void Clear(bool destroy_objects)
	{
		if (destroy_objects)
		{
			for (int i = 0; i < this.icons.Count; i++)
			{
				IconsBar.IconInfo iconInfo = this.icons[i];
				if (!(iconInfo.rt == null))
				{
					UnityEngine.Object.DestroyImmediate(iconInfo.rt.gameObject);
				}
			}
		}
		this.icons.Clear();
		this.Recalc();
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x00123B5C File Offset: 0x00121D5C
	public bool IsVertical()
	{
		IconsBar.Flow flow = this.flow;
		return flow <= IconsBar.Flow.BottomToBottom || (flow - IconsBar.Flow.LeftoToRight > 3 && false);
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x00123B84 File Offset: 0x00121D84
	private RectTransform.Edge StartEdge()
	{
		switch (this.flow)
		{
		case IconsBar.Flow.TopToBottom:
		case IconsBar.Flow.TopToTop:
			return RectTransform.Edge.Top;
		case IconsBar.Flow.BottomToTop:
		case IconsBar.Flow.BottomToBottom:
			return RectTransform.Edge.Bottom;
		case IconsBar.Flow.LeftoToRight:
		case IconsBar.Flow.LeftToLeft:
			return RectTransform.Edge.Left;
		case IconsBar.Flow.RightToLeft:
		case IconsBar.Flow.RightToRight:
			return RectTransform.Edge.Right;
		default:
			return RectTransform.Edge.Top;
		}
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x00123BCC File Offset: 0x00121DCC
	private RectTransform.Edge TargetEdge()
	{
		switch (this.flow)
		{
		case IconsBar.Flow.TopToBottom:
		case IconsBar.Flow.BottomToBottom:
			return RectTransform.Edge.Bottom;
		case IconsBar.Flow.TopToTop:
		case IconsBar.Flow.BottomToTop:
			return RectTransform.Edge.Top;
		case IconsBar.Flow.LeftoToRight:
		case IconsBar.Flow.RightToRight:
			return RectTransform.Edge.Right;
		case IconsBar.Flow.LeftToLeft:
		case IconsBar.Flow.RightToLeft:
			return RectTransform.Edge.Left;
		default:
			return RectTransform.Edge.Top;
		}
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x00123C14 File Offset: 0x00121E14
	private Vector2 PreOfs(IconsBar.IconInfo icon)
	{
		switch (this.TargetEdge())
		{
		case RectTransform.Edge.Left:
			return Vector2.zero;
		case RectTransform.Edge.Right:
			return new Vector2(-(icon.width + this.spacing), 0f);
		case RectTransform.Edge.Top:
			return new Vector2(0f, -(icon.height + this.spacing));
		case RectTransform.Edge.Bottom:
			return Vector2.zero;
		default:
			return Vector2.zero;
		}
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x00123C84 File Offset: 0x00121E84
	private Vector2 PostOfs(IconsBar.IconInfo icon)
	{
		switch (this.TargetEdge())
		{
		case RectTransform.Edge.Left:
			return new Vector2(icon.width + this.spacing, 0f);
		case RectTransform.Edge.Right:
			return Vector2.zero;
		case RectTransform.Edge.Top:
			return Vector2.zero;
		case RectTransform.Edge.Bottom:
			return new Vector2(0f, icon.height + this.spacing);
		default:
			return Vector2.zero;
		}
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x00123CF4 File Offset: 0x00121EF4
	private Vector2 Pos(IconsBar.IconInfo icon, RectTransform.Edge edge, float ofs)
	{
		Rect rect = this.rt.rect;
		float width = rect.width;
		float height = rect.height;
		switch (edge)
		{
		case RectTransform.Edge.Left:
			return new Vector2(ofs, 0f);
		case RectTransform.Edge.Right:
			return new Vector2(width - icon.width - ofs, 0f);
		case RectTransform.Edge.Top:
			return new Vector2(0f, height - icon.height - ofs);
		case RectTransform.Edge.Bottom:
			return new Vector2(0f, ofs);
		default:
			return Vector2.zero;
		}
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x00123D7C File Offset: 0x00121F7C
	private void InitIcon(IconsBar.IconInfo icon)
	{
		icon.rt.SetParent(base.transform, false);
		Rect rect = this.rt.rect;
		float width = rect.width;
		float height = rect.height;
		rect = icon.rt.rect;
		icon.width = rect.width;
		icon.height = rect.height;
		icon.aspectRatio = rect.width / rect.height;
		icon.rt.pivot = Vector3.zero;
		icon.rt.anchorMin = Vector2.zero;
		icon.rt.anchorMax = Vector2.zero;
		float num;
		if (this.IsVertical())
		{
			num = width / icon.width;
		}
		else
		{
			num = height / icon.height;
		}
		icon.width *= num;
		icon.height *= num;
		icon.rt.sizeDelta = new Vector2(icon.width, icon.height);
		Vector2 pos = this.Pos(icon, this.StartEdge(), 0f);
		this.MoveIcon(icon, pos);
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x00123EA0 File Offset: 0x001220A0
	public void RecalcIconsSize()
	{
		if (this.icons == null || this.icons.Count == 0)
		{
			return;
		}
		Rect rect = this.rt.rect;
		float width = rect.width;
		float height = rect.height;
		bool flag = this.IsVertical();
		for (int i = 0; i < this.icons.Count; i++)
		{
			IconsBar.IconInfo iconInfo = this.icons[i];
			if (flag)
			{
				iconInfo.width = width;
				iconInfo.height = width / iconInfo.aspectRatio;
			}
			else
			{
				iconInfo.width = height * iconInfo.aspectRatio;
				iconInfo.height = height;
			}
			iconInfo.rt.sizeDelta = new Vector2(iconInfo.width, iconInfo.height);
		}
		this.Recalc();
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x00123F69 File Offset: 0x00122169
	private void MoveIcon(IconsBar.IconInfo icon, Vector2 pos)
	{
		if (this.scaler != null)
		{
			pos /= this.scaler.scaleFactor;
		}
		icon.rt.anchoredPosition = pos;
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x00123F98 File Offset: 0x00122198
	private bool CanStack(IconsBar.IconInfo icon1, IconsBar.IconInfo icon2)
	{
		return (icon1.timestamp == icon2.timestamp && this.stack_same_frame_messages && !icon1.separate_stack && !icon2.separate_stack) || (!string.IsNullOrEmpty(icon1.stack_group) && !(icon1.stack_group != icon2.stack_group));
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x00123FF4 File Offset: 0x001221F4
	private void CalcTransitionDuration(IconsBar.IconInfo icon)
	{
		Vector2 anchoredPosition = icon.rt.anchoredPosition;
		Vector2 tgt_pos = icon.tgt_pos;
		float a = Mathf.Abs(tgt_pos.x - anchoredPosition.x);
		float b = Mathf.Abs(tgt_pos.y - anchoredPosition.y);
		float num = Mathf.Max(a, b);
		icon.transition_duration = num / this.speed;
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x00124050 File Offset: 0x00122250
	private void CalcTransition(IconsBar.IconInfo icon)
	{
		if (icon.transition_duration <= 0f)
		{
			return;
		}
		if (!icon.in_transition)
		{
			if ((double)(UnityEngine.Time.unscaledTime - (this.transition_end - icon.transition_duration)) < -0.01)
			{
				this.needs_animate = true;
				return;
			}
			icon.in_transition = true;
		}
		Vector2 anchoredPosition = icon.rt.anchoredPosition;
		Vector2 tgt_pos = icon.tgt_pos;
		if (anchoredPosition == tgt_pos)
		{
			icon.in_transition = false;
			return;
		}
		float maxDelta = UnityEngine.Time.unscaledDeltaTime * this.speed;
		anchoredPosition.x = Mathf.MoveTowards(anchoredPosition.x, tgt_pos.x, maxDelta);
		anchoredPosition.y = Mathf.MoveTowards(anchoredPosition.y, tgt_pos.y, maxDelta);
		icon.rt.anchoredPosition = anchoredPosition;
		if (anchoredPosition != tgt_pos)
		{
			this.needs_animate = true;
			return;
		}
		icon.in_transition = false;
		if (icon.first_fall)
		{
			BaseUI.PlaySoundEvent(icon.sound_effect, null);
			icon.first_fall = false;
		}
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x00124144 File Offset: 0x00122344
	public void Recalc()
	{
		this.picked_idx = -1;
		if (this.icons.Count < 1)
		{
			this.needs_animate = false;
			this.transition_end = 0f;
			return;
		}
		this.needs_animate = true;
		float num = this.spacing;
		int num2 = 0;
		bool flag = this.IsVertical();
		float num3 = flag ? this.icons[0].height : this.icons[0].width;
		for (int i = 1; i < this.icons.Count; i++)
		{
			IconsBar.IconInfo iconInfo = this.icons[i];
			if (!this.CanStack(iconInfo, this.icons[i - 1]))
			{
				num2++;
				num3 += (flag ? iconInfo.height : iconInfo.width);
			}
		}
		num3 += (float)num2 * this.spacing;
		Rect rect = this.rt.rect;
		float num4 = flag ? rect.height : rect.width;
		if (num3 > num4 && num2 > 0)
		{
			this.spacing -= (num3 - num4) / (float)num2;
		}
		Vector2 vector = this.Pos(this.icons[0], this.TargetEdge(), 0f);
		this.icons[0].tgt_pos = vector;
		this.icons[0].stack = 0;
		for (int j = 1; j < this.icons.Count; j++)
		{
			IconsBar.IconInfo iconInfo2 = this.icons[j - 1];
			IconsBar.IconInfo iconInfo3 = this.icons[j];
			Vector2 b = Vector3.zero;
			if (this.CanStack(iconInfo3, iconInfo2))
			{
				iconInfo3.stack = iconInfo2.stack + 1;
				b = (float)Mathf.Min(iconInfo3.stack, this.maxStacks) * this.stackOffset;
			}
			else
			{
				iconInfo3.stack = 0;
				vector += this.PostOfs(iconInfo2);
				vector += this.PreOfs(iconInfo3);
			}
			iconInfo3.tgt_pos = vector + b;
		}
		float num5 = 0f;
		for (int k = 0; k < this.icons.Count; k++)
		{
			IconsBar.IconInfo iconInfo4 = this.icons[k];
			iconInfo4.rt.SetSiblingIndex(this.icons.Count - 1 - k);
			this.CalcTransitionDuration(iconInfo4);
			if (iconInfo4.transition_duration > num5)
			{
				num5 = iconInfo4.transition_duration;
			}
		}
		this.transition_end = UnityEngine.Time.unscaledTime + num5;
		this.spacing = num;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x001243E4 File Offset: 0x001225E4
	private void Animate()
	{
		if (!this.needs_animate)
		{
			return;
		}
		this.needs_animate = false;
		for (int i = 0; i < this.icons.Count; i++)
		{
			IconsBar.IconInfo icon = this.icons[i];
			this.CalcTransition(icon);
		}
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x0012442C File Offset: 0x0012262C
	private void SetPicked(int idx)
	{
		if (idx == this.picked_idx)
		{
			return;
		}
		if (this.picked_idx >= 0)
		{
			this.icons[this.picked_idx].rt.SetSiblingIndex(this.icons.Count - 1 - this.picked_idx);
		}
		this.picked_idx = idx;
		if (this.picked_idx >= 0)
		{
			this.icons[this.picked_idx].rt.SetAsLastSibling();
		}
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x001244A8 File Offset: 0x001226A8
	private void UpdatePicked()
	{
		Vector2 a;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rt, Input.mousePosition, null, out a))
		{
			this.SetPicked(-1);
			return;
		}
		int num = -1;
		float num2 = 0f;
		for (int i = 0; i < this.icons.Count; i++)
		{
			IconsBar.IconInfo iconInfo = this.icons[i];
			if (iconInfo.stack <= 0)
			{
				Vector2 anchoredPosition = iconInfo.rt.anchoredPosition;
				Vector2 vector = a - anchoredPosition;
				if (vector.x >= 0f && vector.y >= 0f && vector.x <= iconInfo.width && vector.y <= iconInfo.height)
				{
					vector.x -= iconInfo.width / 2f;
					vector.y -= iconInfo.height / 2f;
					float sqrMagnitude = vector.sqrMagnitude;
					if (num < 0 || sqrMagnitude < num2)
					{
						num = i;
						num2 = sqrMagnitude;
					}
				}
			}
		}
		this.SetPicked(num);
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x001245B8 File Offset: 0x001227B8
	private void Init()
	{
		if (this.initted)
		{
			return;
		}
		this.initted = true;
		this.scaler = base.GetComponentInParent<CanvasScaler>();
		this.rt = base.GetComponent<RectTransform>();
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x001245E2 File Offset: 0x001227E2
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x001245EA File Offset: 0x001227EA
	private void Update()
	{
		this.floodControl.OnUpdate();
		this.UpdatePicked();
		this.Animate();
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x00124603 File Offset: 0x00122803
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.Recalc();
	}

	// Token: 0x040014EE RID: 5358
	public IconsBar.Flow flow;

	// Token: 0x040014EF RID: 5359
	public float spacing = 10f;

	// Token: 0x040014F0 RID: 5360
	public float speed = 2000f;

	// Token: 0x040014F1 RID: 5361
	public Vector2 stackOffset = new Vector2(4f, -4f);

	// Token: 0x040014F2 RID: 5362
	public int maxStacks = 2;

	// Token: 0x040014F3 RID: 5363
	public IconsFloodControl floodControl = new IconsFloodControl();

	// Token: 0x040014F4 RID: 5364
	public string TestDef1 = "";

	// Token: 0x040014F5 RID: 5365
	public string TestDef2 = "";

	// Token: 0x040014F6 RID: 5366
	public string TestDef3 = "";

	// Token: 0x040014F7 RID: 5367
	private CanvasScaler scaler;

	// Token: 0x040014F8 RID: 5368
	private RectTransform rt;

	// Token: 0x040014F9 RID: 5369
	public List<IconsBar.IconInfo> icons = new List<IconsBar.IconInfo>();

	// Token: 0x040014FA RID: 5370
	private float transition_end;

	// Token: 0x040014FB RID: 5371
	private bool needs_animate;

	// Token: 0x040014FC RID: 5372
	private int picked_idx = -1;

	// Token: 0x040014FD RID: 5373
	[NonSerialized]
	public bool stack_same_frame_messages;

	// Token: 0x040014FE RID: 5374
	private bool initted;

	// Token: 0x02000742 RID: 1858
	public enum Flow
	{
		// Token: 0x04003943 RID: 14659
		TopToBottom,
		// Token: 0x04003944 RID: 14660
		TopToTop,
		// Token: 0x04003945 RID: 14661
		BottomToTop,
		// Token: 0x04003946 RID: 14662
		BottomToBottom,
		// Token: 0x04003947 RID: 14663
		LeftoToRight,
		// Token: 0x04003948 RID: 14664
		LeftToLeft,
		// Token: 0x04003949 RID: 14665
		RightToLeft,
		// Token: 0x0400394A RID: 14666
		RightToRight
	}

	// Token: 0x02000743 RID: 1859
	public class IconInfo
	{
		// Token: 0x0400394B RID: 14667
		public MessageIcon.CreateParams pars;

		// Token: 0x0400394C RID: 14668
		public RectTransform rt;

		// Token: 0x0400394D RID: 14669
		public float width;

		// Token: 0x0400394E RID: 14670
		public float height;

		// Token: 0x0400394F RID: 14671
		public float aspectRatio = 1f;

		// Token: 0x04003950 RID: 14672
		public string stack_group;

		// Token: 0x04003951 RID: 14673
		public bool separate_stack;

		// Token: 0x04003952 RID: 14674
		public long timestamp;

		// Token: 0x04003953 RID: 14675
		public int stack_priority;

		// Token: 0x04003954 RID: 14676
		public Vector2 tgt_pos;

		// Token: 0x04003955 RID: 14677
		public int stack;

		// Token: 0x04003956 RID: 14678
		public float transition_duration;

		// Token: 0x04003957 RID: 14679
		public bool in_transition;

		// Token: 0x04003958 RID: 14680
		public bool first_fall = true;

		// Token: 0x04003959 RID: 14681
		public string sound_effect;
	}
}
