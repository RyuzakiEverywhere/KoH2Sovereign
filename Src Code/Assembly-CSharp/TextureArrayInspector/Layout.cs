using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000483 RID: 1155
	public class Layout
	{
		// Token: 0x06003C8A RID: 15498 RVA: 0x001CD0FC File Offset: 0x001CB2FC
		public static Rect GetInspectorRect()
		{
			return default(Rect);
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x001CD112 File Offset: 0x001CB312
		public static void SetInspectorRect(Rect rect)
		{
			if (Event.current.type == EventType.Layout)
			{
				GUILayoutUtility.GetRect(1f, rect.height, "TextField");
			}
		}

		// Token: 0x06003C8C RID: 15500 RVA: 0x001CD140 File Offset: 0x001CB340
		public void Par(Layout.Val height = default(Layout.Val), Layout.Val margin = default(Layout.Val), Layout.Val padding = default(Layout.Val))
		{
			int num = height.ovd ? ((int)height.val) : this.lineHeight;
			int num2 = margin.ovd ? ((int)margin.val) : this.margin;
			int num3 = padding.ovd ? ((int)padding.val) : this.verticalPadding;
			this.cursor = new Rect(this.field.x + (float)num2, this.cursor.y + this.cursor.height + (float)num3, 0f, (float)(num - num3));
			this.field = new Rect(this.field.x, this.field.y, this.field.width, Mathf.Max(this.field.height, this.cursor.y + this.cursor.height));
			this.bottomPoint = Mathf.Max(this.bottomPoint, this.cursor.y + this.cursor.height);
		}

		// Token: 0x06003C8D RID: 15501 RVA: 0x001CD24C File Offset: 0x001CB44C
		public Rect Inset(Layout.Val width = default(Layout.Val), Layout.Val margin = default(Layout.Val), Layout.Val rightMargin = default(Layout.Val), Layout.Val padding = default(Layout.Val))
		{
			int num = margin.ovd ? ((int)margin.val) : this.margin;
			int num2 = rightMargin.ovd ? ((int)rightMargin.val) : this.rightMargin;
			int num3 = 0;
			float num4 = width.ovd ? width.val : 1f;
			if (num4 < 1.0001f)
			{
				num4 *= this.field.width - (float)num - (float)num2;
			}
			this.cursor.x = this.cursor.x + num4;
			this.lastRect = new Rect(this.cursor.x - num4, this.cursor.y + this.field.y, num4 - (float)num3, this.cursor.height);
			return this.lastRect;
		}

		// Token: 0x06003C8E RID: 15502 RVA: 0x001CD314 File Offset: 0x001CB514
		public Rect ParInset(Layout.Val height = default(Layout.Val), Layout.Val width = default(Layout.Val), Layout.Val margin = default(Layout.Val), Layout.Val rightMargin = default(Layout.Val), Layout.Val verticalPadding = default(Layout.Val), Layout.Val horizontalPadding = default(Layout.Val))
		{
			this.Par(height, margin, verticalPadding);
			return this.Inset(width, margin, rightMargin, horizontalPadding);
		}

		// Token: 0x06003C8F RID: 15503 RVA: 0x001CD32C File Offset: 0x001CB52C
		public void Repaint(int numTimes)
		{
			this.repaintsLeft = Mathf.Max(this.repaintsLeft, numTimes);
			this.Repaint();
			this.repaintsLeft--;
		}

		// Token: 0x06003C90 RID: 15504 RVA: 0x000023FD File Offset: 0x000005FD
		public void Repaint()
		{
		}

		// Token: 0x06003C91 RID: 15505 RVA: 0x001CD354 File Offset: 0x001CB554
		public void Zoom()
		{
			if (Event.current == null)
			{
				return;
			}
			bool control = Event.current.control;
			float num = 0f;
			if (Event.current.type == EventType.ScrollWheel)
			{
				num = Event.current.delta.y / 3f;
			}
			else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && control)
			{
				num = Event.current.delta.y / 15f;
			}
			if (Mathf.Abs(num) < 0.001f)
			{
				return;
			}
			float num2 = -this.zoom * this.zoomStep * num;
			if (this.zoom + num2 > this.maxZoom)
			{
				num2 = this.maxZoom - this.zoom;
			}
			if (this.zoom + num2 < this.minZoom)
			{
				num2 = this.minZoom - this.zoom;
			}
			Vector2 a = (Event.current.mousePosition - this.scroll) / this.zoom;
			this.zoom += num2;
			if (this.zoom >= this.minZoom && this.zoom <= this.maxZoom)
			{
				this.scroll -= a * num2;
			}
		}

		// Token: 0x06003C92 RID: 15506 RVA: 0x001CD494 File Offset: 0x001CB694
		public void Scroll()
		{
			if (Event.current == null || Event.current.type != EventType.MouseDrag)
			{
				return;
			}
			if (Event.current.button != this.scrollButton && (Event.current.button != 0 || !Event.current.alt))
			{
				return;
			}
			this.scroll += Event.current.delta;
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x001CD4FC File Offset: 0x001CB6FC
		public void ScrollWheel(int step = 3)
		{
			float num = 0f;
			if (Event.current.type == EventType.ScrollWheel)
			{
				num = Event.current.delta.y / 3f;
			}
			this.scroll.y = this.scroll.y - num * (float)this.lineHeight * (float)step;
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x001CD550 File Offset: 0x001CB750
		public Rect ToDisplay(Rect rect)
		{
			return new Rect(rect.x * this.zoom + this.scroll.x, rect.y * this.zoom + this.scroll.y, rect.width * this.zoom, rect.height * this.zoom);
		}

		// Token: 0x06003C95 RID: 15509 RVA: 0x001CD5B4 File Offset: 0x001CB7B4
		public Rect ToInternal(Rect rect)
		{
			return new Rect((rect.x - this.scroll.x) / this.zoom, (rect.y - this.scroll.y) / this.zoom, rect.width / this.zoom, rect.height / this.zoom);
		}

		// Token: 0x06003C96 RID: 15510 RVA: 0x001CD616 File Offset: 0x001CB816
		public Vector2 ToInternal(Vector2 pos)
		{
			return (pos - this.scroll) / this.zoom;
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x001CD62F File Offset: 0x001CB82F
		public Vector2 ToDisplay(Vector2 pos)
		{
			return pos * this.zoom + this.scroll;
		}

		// Token: 0x06003C98 RID: 15512 RVA: 0x001CD648 File Offset: 0x001CB848
		public void Focus(Vector2 pos)
		{
			pos *= this.zoom;
			this.scroll = -pos;
			this.scroll += new Vector2(this.field.width / 2f, this.field.height / 2f);
		}

		// Token: 0x06003C99 RID: 15513 RVA: 0x000023FD File Offset: 0x000005FD
		public void CheckStyles()
		{
		}

		// Token: 0x06003C9A RID: 15514 RVA: 0x001CD6A8 File Offset: 0x001CB8A8
		public Texture2D GetIcon(string textureName)
		{
			Texture2D texture2D;
			if (!this.icons.ContainsKey(textureName))
			{
				texture2D = (Resources.Load(textureName) as Texture2D);
				if (texture2D == null)
				{
					texture2D = (Resources.Load(textureName) as Texture2D);
				}
				this.icons.Add(textureName, texture2D);
			}
			else
			{
				texture2D = this.icons[textureName];
			}
			return texture2D;
		}

		// Token: 0x06003C9B RID: 15515 RVA: 0x001CD708 File Offset: 0x001CB908
		public bool Icon(string textureName, Rect rect, Layout.IconAligment horizontalAlign = Layout.IconAligment.resize, Layout.IconAligment verticalAlign = Layout.IconAligment.resize, int animationFrames = 0, bool frame = false, bool tile = false, bool clickable = false)
		{
			if (animationFrames != 0)
			{
				DateTime now = DateTime.Now;
				int num = (int)(((float)now.Second * 5f + (float)now.Millisecond * 5f / 1000f) % (float)animationFrames);
				string str = ((num + 1 < 10) ? "0" : "") + (num + 1).ToString();
				return this.Icon(textureName + str, rect, horizontalAlign, verticalAlign, 0, frame, tile, clickable);
			}
			return this.Icon(this.GetIcon(textureName), rect, horizontalAlign, verticalAlign, frame, tile, clickable, true);
		}

		// Token: 0x06003C9C RID: 15516 RVA: 0x001CD7A0 File Offset: 0x001CB9A0
		public bool Icon(Texture2D texture, Rect rect, Layout.IconAligment horizontalAlign = Layout.IconAligment.resize, Layout.IconAligment verticalAlign = Layout.IconAligment.resize, bool frame = false, bool tile = false, bool clickable = false, bool alphaBlend = true)
		{
			if (texture == null)
			{
				return false;
			}
			if (rect.width > (float)texture.width)
			{
				switch (horizontalAlign)
				{
				case Layout.IconAligment.min:
					rect.width = (float)texture.width;
					break;
				case Layout.IconAligment.max:
					rect.x += rect.width;
					rect.x -= (float)texture.width;
					rect.width = (float)texture.width;
					break;
				case Layout.IconAligment.center:
					rect.x += rect.width / 2f;
					rect.x -= (float)(texture.width / 2);
					rect.width = (float)texture.width;
					break;
				}
			}
			if (rect.height > (float)texture.height)
			{
				switch (verticalAlign)
				{
				case Layout.IconAligment.min:
					rect.height = (float)texture.height;
					break;
				case Layout.IconAligment.max:
					rect.y += rect.height;
					rect.y -= (float)texture.height;
					rect.height = (float)texture.height;
					break;
				case Layout.IconAligment.center:
					rect.y += rect.height / 2f;
					rect.y -= (float)(texture.height / 2);
					rect.height = (float)texture.height;
					break;
				}
			}
			bool result = false;
			if (!tile)
			{
				GUI.DrawTexture(this.ToDisplay(rect), texture, ScaleMode.ScaleAndCrop);
			}
			else
			{
				Rect rect2 = this.ToDisplay(rect);
				for (float num = 0f; num < rect.width; num += (float)texture.width * this.zoom)
				{
					for (float num2 = 0f; num2 < rect.height; num2 += (float)texture.height * this.zoom)
					{
						GUI.DrawTexture(new Rect(num + rect2.x, num2 + rect2.y, (float)texture.width * this.zoom, (float)texture.height * this.zoom), texture, ScaleMode.StretchToFill);
					}
				}
			}
			return result;
		}

		// Token: 0x06003C9D RID: 15517 RVA: 0x001CD9C7 File Offset: 0x001CBBC7
		public void TextureIcon(Texture2D texture, Rect rect)
		{
			if (Layout.iconMat == null)
			{
				Layout.iconMat = new Material(Shader.Find("Hidden/DPLayout/TextureIcon"));
			}
		}

		// Token: 0x06003C9E RID: 15518 RVA: 0x001CD9EC File Offset: 0x001CBBEC
		public void Texture(Texture2D texture, Rect rect, ref int mode)
		{
			if (Layout.textureMat == null)
			{
				Layout.textureMat = new Material(Shader.Find("Hidden/DPLayout/Texture"));
			}
			Layout.textureMat.SetInt("_Mode", mode);
			int num = 20;
			for (int i = 0; i < Layout.textureSimpleIcons.Length; i++)
			{
				Rect rect2 = new Rect(rect.x + (float)(i * num), rect.y, (float)num, (float)num);
				if (this.Button(null, rect2, default(Layout.Val), default(Layout.Val), Layout.textureSimpleIcons[i], false, 0, GUIStyle.none, null))
				{
					mode = i;
				}
			}
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x001CDA8C File Offset: 0x001CBC8C
		public void TextureTool(ref int mode, ref int channel, Rect rect = default(Rect))
		{
			int num = 20;
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(num, default(Layout.Val), 0);
				rect = this.Inset(1, 0, default(Layout.Val), 0);
			}
			Rect rect2 = new Rect(rect.x, rect.y, (float)(Layout.textureModeIcons.Length * num), rect.height);
			for (int i = 0; i < Layout.textureModeIcons.Length; i++)
			{
				Rect rect3 = new Rect(rect.x + (float)(i * num), rect.y, (float)num, rect.height);
				if (this.Button(null, rect3, default(Layout.Val), default(Layout.Val), Layout.textureModeIcons[i], false, 0, GUIStyle.none, null))
				{
					mode = i;
				}
			}
			Rect rect4 = new Rect(rect2.x + rect2.width, rect.y, (float)(num / 3), rect.height);
			Rect rect5 = new Rect(rect4.x + rect4.width, rect.y, (float)(Layout.textureChannelsIcons.Length * num), rect.height);
			for (int j = 0; j < Layout.textureChannelsIcons.Length; j++)
			{
				Rect rect6 = new Rect(rect5.x + (float)(j * num), rect.y, (float)num, rect.height);
				if (this.Button(null, rect6, default(Layout.Val), default(Layout.Val), Layout.textureChannelsIcons[j], false, 0, GUIStyle.none, null))
				{
					mode = j;
				}
			}
		}

		// Token: 0x06003CA0 RID: 15520 RVA: 0x001CDC50 File Offset: 0x001CBE50
		public void Element(string textureName, Rect rect, RectOffset borders, RectOffset offset)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			GUIStyle guistyle = this.elementStyles.CheckGet(textureName);
			if (guistyle == null || guistyle.normal.background == null || guistyle.hover.background == null)
			{
				guistyle = new GUIStyle();
				guistyle.normal.background = this.GetIcon(textureName);
				guistyle.hover.background = this.GetIcon(textureName + "_pro");
				this.elementStyles.CheckAdd(textureName, guistyle, true);
			}
			if (borders != null)
			{
				guistyle.border = borders;
			}
			Rect rect2 = this.ToDisplay(rect);
			if (offset != null)
			{
				rect2 = new Rect(rect2.x - (float)offset.left, rect2.y - (float)offset.top, rect2.width + (float)offset.left + (float)offset.right, rect2.height + (float)offset.top + (float)offset.bottom);
			}
		}

		// Token: 0x06003CA1 RID: 15521 RVA: 0x001CDD54 File Offset: 0x001CBF54
		private float StepRound(float src)
		{
			if (src < 1f)
			{
				src = (float)((int)(src * 1000f)) / 1000f;
			}
			else if (src < 10f)
			{
				src = (float)((int)(src * 100f)) / 100f;
			}
			else if (src < 100f)
			{
				src = (float)((int)(src * 10f)) / 10f;
			}
			else
			{
				src = (float)((int)src);
			}
			return src;
		}

		// Token: 0x06003CA2 RID: 15522 RVA: 0x001CDDB8 File Offset: 0x001CBFB8
		public float DragChangeField(float val, Rect sliderRect, float min = 0f, float max = 0f, float minStep = 0.2f)
		{
			sliderRect = this.ToDisplay(sliderRect);
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition))
			{
				this.sliderClickPos = Event.current.mousePosition;
				this.sliderOriginalValue = val;
				this.sliderDraggingId = controlID;
			}
			if (this.sliderDraggingId == controlID)
			{
				int num = (int)((Event.current.mousePosition.x - this.sliderClickPos.x) / 5f);
				val = this.sliderOriginalValue;
				for (int i = 0; i < Mathf.Abs(num); i++)
				{
					object obj = (val >= 0f) ? val : (-val);
					float num2 = 0.01f;
					object obj2 = obj;
					if (obj2 != 0.99f)
					{
						num2 = 0.02f;
					}
					if (obj2 != 1.99f)
					{
						num2 = 0.1f;
					}
					if (obj2 != 4.999f)
					{
						num2 = 0.2f;
					}
					if (obj2 != 9.999f)
					{
						num2 = 0.5f;
					}
					if (obj2 != 39.999f)
					{
						num2 = 1f;
					}
					if (obj2 != 99.999f)
					{
						num2 = 2f;
					}
					if (obj2 != 199.999f)
					{
						num2 = 5f;
					}
					if (obj2 != 499.999f)
					{
						num2 = 10f;
					}
					if (num2 < minStep)
					{
						num2 = minStep;
					}
					val = ((num > 0) ? (val + num2) : (val - num2));
					val = Mathf.Round(val * 10000f) / 10000f;
					if (Mathf.Abs(min) > 0.001f && val < min)
					{
						val = min;
					}
					if (Mathf.Abs(max) > 0.001f && val > max)
					{
						val = max;
					}
				}
			}
			if (Event.current.rawType == EventType.MouseUp)
			{
				this.sliderDraggingId = -20000000;
			}
			if (Event.current.isMouse && this.sliderDraggingId == controlID)
			{
				Event.current.Use();
			}
			return val;
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06003CA3 RID: 15523 RVA: 0x001CDF70 File Offset: 0x001CC170
		// (remove) Token: 0x06003CA4 RID: 15524 RVA: 0x001CDFA8 File Offset: 0x001CC1A8
		public event Layout.ChangeAction OnBeforeChange;

		// Token: 0x06003CA5 RID: 15525 RVA: 0x001CDFDD File Offset: 0x001CC1DD
		public void SetChange(bool change)
		{
			if (change)
			{
				this.change = true;
				this.lastChange = true;
				if (this.OnBeforeChange != null)
				{
					this.OnBeforeChange();
					return;
				}
			}
			else
			{
				this.lastChange = false;
			}
		}

		// Token: 0x06003CA6 RID: 15526 RVA: 0x001CE00C File Offset: 0x001CC20C
		public void Field<T>(ref T src, string label = null, Rect rect = default(Rect), float min = -200000000f, float max = 200000000f, bool limit = true, Layout.Val fieldSize = default(Layout.Val), Layout.Val sliderSize = default(Layout.Val), Layout.Val monitorChange = default(Layout.Val), Layout.Val useEvent = default(Layout.Val), Layout.Val disabled = default(Layout.Val), Layout.Val dragChange = default(Layout.Val), Layout.Val slider = default(Layout.Val), Layout.Val quadratic = default(Layout.Val), Layout.Val allowSceneObject = default(Layout.Val), Layout.Val delayed = default(Layout.Val), GUIStyle style = null, string tooltip = null)
		{
			src = this.Field<T>(src, label, rect, min, max, limit, fieldSize, sliderSize, monitorChange, this.markup, useEvent, disabled, dragChange, slider, quadratic, allowSceneObject, delayed, style, tooltip);
		}

		// Token: 0x06003CA7 RID: 15527 RVA: 0x001CE058 File Offset: 0x001CC258
		public T Field<T>(T src, string label = null, Rect rect = default(Rect), float min = -200000000f, float max = 200000000f, bool limit = true, Layout.Val fieldSize = default(Layout.Val), Layout.Val sliderSize = default(Layout.Val), Layout.Val monitorChange = default(Layout.Val), Layout.Val markup = default(Layout.Val), Layout.Val useEvent = default(Layout.Val), Layout.Val disabled = default(Layout.Val), Layout.Val dragChange = default(Layout.Val), Layout.Val slider = default(Layout.Val), Layout.Val quadratic = default(Layout.Val), Layout.Val allowSceneObject = default(Layout.Val), Layout.Val delayed = default(Layout.Val), GUIStyle style = null, string tooltip = null)
		{
			fieldSize.Verify(this.fieldSize);
			sliderSize.Verify(this.sliderSize);
			useEvent.Verify(this.useEvent);
			disabled.Verify(this.disabled);
			markup.Verify(this.markup);
			dragChange.Verify(this.dragChange);
			slider.Verify(this.slider);
			delayed.Verify(this.delayed);
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (markup)
			{
				return src;
			}
			disabled.Verify(this.disabled);
			if (label == null)
			{
				fieldSize = 1;
			}
			Rect rect2 = rect.Clamp(1f - fieldSize);
			Rect r = rect.ClampFromLeft(fieldSize);
			Rect r2 = r.Clamp(sliderSize);
			r2 = r2.Clamp((int)r2.width - 4);
			if (slider)
			{
				r = r.ClampFromLeft(1f - sliderSize);
			}
			if (label != null && this.zoom > 0.3f)
			{
				this.Label(label, rect2, null, false, 0, default(Layout.Val), default(Layout.Val), FontStyle.Normal, TextAnchor.UpperLeft, false, null, tooltip);
			}
			T t = (T)((object)default(T));
			monitorChange.Verify(this.monitorChange);
			if (monitorChange && !EqualityComparer<T>.Default.Equals(src, t))
			{
				this.SetChange(true);
			}
			else
			{
				this.SetChange(false);
			}
			return t;
		}

		// Token: 0x06003CA8 RID: 15528 RVA: 0x001CE248 File Offset: 0x001CC448
		public void Curve(AnimationCurve src, Rect rect, Rect ranges = default(Rect), Color color = default(Color), string tooltip = null)
		{
			if (ranges.width < Mathf.Epsilon && ranges.height < Mathf.Epsilon)
			{
				ranges.width = 1f;
				ranges.height = 1f;
			}
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (color.a < 0.001f)
			{
				color = Color.white;
			}
			this.lastChange = false;
			bool flag = this.markup;
		}

		// Token: 0x06003CA9 RID: 15529 RVA: 0x001CE318 File Offset: 0x001CC518
		public void Label(string label = null, Rect rect = default(Rect), string url = null, bool helpbox = false, int messageType = 0, Layout.Val fontSize = default(Layout.Val), Layout.Val disabled = default(Layout.Val), FontStyle fontStyle = FontStyle.Normal, TextAnchor textAnchor = TextAnchor.UpperLeft, bool prefix = false, string icon = null, string tooltip = null)
		{
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return;
			}
			disabled.Verify(this.disabled);
			this.CheckStyles();
			GUIStyle guistyle = this.labelStyle;
			if (url != null)
			{
				guistyle = this.urlStyle;
			}
			fontSize.Verify(this.fontSize);
			if (helpbox)
			{
				fontSize /= 1.2f;
			}
			fontSize = Mathf.RoundToInt(fontSize * this.zoom);
			if (guistyle.fontSize != fontSize)
			{
				guistyle.fontSize = fontSize;
			}
			if (guistyle.alignment != textAnchor)
			{
				this.labelStyle.alignment = textAnchor;
			}
			if (guistyle.fontStyle != fontStyle)
			{
				this.labelStyle.fontStyle = fontStyle;
			}
			if (helpbox)
			{
				this.labelStyle.wordWrap = true;
			}
			else
			{
				this.labelStyle.wordWrap = false;
			}
			if (icon != null)
			{
				this.Icon(icon, new Rect(rect.x + 4f, rect.y, rect.width - 8f, rect.height), Layout.IconAligment.min, Layout.IconAligment.center, 0, false, false, false);
			}
			new GUIContent(label, tooltip);
		}

		// Token: 0x06003CAA RID: 15530 RVA: 0x001CE4AC File Offset: 0x001CC6AC
		public string EditableLabel(string label = null, Rect rect = default(Rect), Layout.Val fontSize = default(Layout.Val), Layout.Val disabled = default(Layout.Val), FontStyle fontStyle = FontStyle.Normal, TextAnchor textAnchor = TextAnchor.UpperLeft, string tooltip = null)
		{
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return label;
			}
			this.CheckStyles();
			GUIStyle guistyle = this.labelStyle;
			fontSize.Verify(this.fontSize);
			fontSize = Mathf.RoundToInt(fontSize * this.zoom);
			if (guistyle.fontSize != fontSize)
			{
				guistyle.fontSize = fontSize;
			}
			if (guistyle.alignment != textAnchor)
			{
				this.labelStyle.alignment = textAnchor;
			}
			if (guistyle.fontStyle != fontStyle)
			{
				this.labelStyle.fontStyle = fontStyle;
			}
			float num = 20f;
			Rect rect2 = new Rect(rect.x + rect.width - num, rect.y, num, rect.height);
			this.Icon("DPLayout_EditableLabel", rect2, Layout.IconAligment.center, Layout.IconAligment.center, 0, false, false, false);
			if (GUI.Button(this.ToDisplay(rect2), "", GUIStyle.none))
			{
				Layout.currentlyFocusedRect = rect;
			}
			CoordinatesExtensions.Approximately(rect, Layout.currentlyFocusedRect);
			new Rect(rect.x, rect.y, rect.width - num, rect.height);
			return label;
		}

		// Token: 0x06003CAB RID: 15531 RVA: 0x001CE638 File Offset: 0x001CC838
		public bool Button(string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string icon = null, bool iconCenter = false, int iconAnimFrames = 0, GUIStyle style = null, string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			GUIContent content = new GUIContent(label, tooltip);
			if (this.markup)
			{
				return false;
			}
			disabled.Verify(this.disabled);
			bool flag;
			if (style == null)
			{
				flag = GUI.Button(this.ToDisplay(rect), content, this.buttonStyle);
			}
			else
			{
				flag = GUI.Button(this.ToDisplay(rect), content, style);
			}
			monitorChange.Verify(this.monitorChange);
			if (monitorChange)
			{
				if (flag)
				{
					this.SetChange(true);
				}
				else
				{
					this.SetChange(false);
				}
			}
			if (icon != null && !iconCenter)
			{
				this.Icon(icon, new Rect(rect.x + 4f, rect.y, rect.width - 8f, rect.height), Layout.IconAligment.min, Layout.IconAligment.center, iconAnimFrames, false, false, false);
			}
			if (icon != null && iconCenter)
			{
				this.Icon(icon, rect, Layout.IconAligment.center, Layout.IconAligment.center, iconAnimFrames, false, false, false);
			}
			return flag;
		}

		// Token: 0x06003CAC RID: 15532 RVA: 0x001CE78C File Offset: 0x001CC98C
		public void CheckButton(ref bool src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string icon = null, string tooltip = null)
		{
			src = this.CheckButton(src, label, rect, monitorChange, disabled, icon, tooltip);
		}

		// Token: 0x06003CAD RID: 15533 RVA: 0x001CE7B0 File Offset: 0x001CC9B0
		public bool CheckButton(bool src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string icon = null, string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return src;
			}
			GUIContent content = new GUIContent(label, tooltip);
			bool flag = GUI.Toggle(this.ToDisplay(rect), src, content, this.buttonStyle);
			monitorChange.Verify(this.monitorChange);
			if (monitorChange)
			{
				if (flag != src)
				{
					this.SetChange(true);
				}
				else
				{
					this.SetChange(false);
				}
			}
			if (icon != null)
			{
				this.Icon(icon, new Rect(rect.x + 4f, rect.y, rect.width - 8f, rect.height), Layout.IconAligment.min, Layout.IconAligment.center, 0, false, false, false);
			}
			return flag;
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x001CE8C0 File Offset: 0x001CCAC0
		public void Toggle(ref bool src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string onIcon = null, string offIcon = null, string tooltip = null)
		{
			src = this.Toggle(src, label, rect, monitorChange, disabled, onIcon, offIcon, tooltip);
		}

		// Token: 0x06003CAF RID: 15535 RVA: 0x001CE8E4 File Offset: 0x001CCAE4
		public bool Toggle(bool src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string onIcon = null, string offIcon = null, string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return src;
			}
			disabled.Verify(this.disabled);
			new Rect(rect.x, rect.y, 20f, rect.height);
			Rect rect2 = new Rect(rect.x + 20f, rect.y, rect.width - 20f, rect.height);
			if (label != null)
			{
				this.Label(label, rect2, null, false, 0, default(Layout.Val), default(Layout.Val), FontStyle.Normal, TextAnchor.UpperLeft, false, null, null);
			}
			monitorChange.Verify(this.monitorChange);
			if (monitorChange)
			{
				if (src != src)
				{
					this.SetChange(true);
				}
				else
				{
					this.SetChange(false);
				}
			}
			return src;
		}

		// Token: 0x06003CB0 RID: 15536 RVA: 0x001CEA18 File Offset: 0x001CCC18
		public void LayersField(ref int src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string onIcon = null, string offIcon = null, string tooltip = null)
		{
			src = this.LayersField(src, label, rect, monitorChange, disabled, onIcon, offIcon, tooltip);
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x001CEA3C File Offset: 0x001CCC3C
		public int LayersField(int src, string label = null, Rect rect = default(Rect), Layout.Val monitorChange = default(Layout.Val), Layout.Val disabled = default(Layout.Val), string onIcon = null, string offIcon = null, string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return src;
			}
			disabled.Verify(this.disabled);
			if (label == null)
			{
				this.fieldSize = 1f;
			}
			Rect rect2 = rect.Clamp(1f - this.fieldSize);
			rect.ClampFromLeft(this.fieldSize);
			if (label != null)
			{
				this.Label(label, rect2, null, false, 0, default(Layout.Val), default(Layout.Val), FontStyle.Normal, TextAnchor.UpperLeft, false, null, null);
			}
			monitorChange.Verify(this.monitorChange);
			if (monitorChange)
			{
				if (src != src)
				{
					this.SetChange(true);
				}
				else
				{
					this.SetChange(false);
				}
			}
			return src;
		}

		// Token: 0x06003CB2 RID: 15538 RVA: 0x001CEB50 File Offset: 0x001CCD50
		public void Foldout(ref bool src, string label = null, Rect rect = default(Rect), Layout.Val disabled = default(Layout.Val), string tooltip = null, bool bold = true)
		{
			src = this.Foldout(src, label, rect, disabled, tooltip, bold);
		}

		// Token: 0x06003CB3 RID: 15539 RVA: 0x001CEB64 File Offset: 0x001CCD64
		public bool Foldout(bool src, string label = null, Rect rect = default(Rect), Layout.Val disabled = default(Layout.Val), string tooltip = null, bool bold = true)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return src;
			}
			new GUIContent(label, tooltip);
			if (bold)
			{
				this.foldoutStyle.fontStyle = FontStyle.Bold;
			}
			else
			{
				this.foldoutStyle.fontStyle = FontStyle.Normal;
			}
			rect.x += 12f;
			rect.width -= 12f;
			return false;
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x001CEC38 File Offset: 0x001CCE38
		public void ToggleFoldout(ref bool unfolded, ref bool enabled, string label = null, Rect rect = default(Rect), Layout.Val disabled = default(Layout.Val), string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return;
			}
			label = "     " + label;
			new GUIContent(label, tooltip);
			this.foldoutStyle.fontStyle = FontStyle.Normal;
		}

		// Token: 0x06003CB5 RID: 15541 RVA: 0x001CECE0 File Offset: 0x001CCEE0
		public void Gauge(float progress, string label, Rect rect = default(Rect), Layout.Val disabled = default(Layout.Val), string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return;
			}
			new GUIContent(label, tooltip);
		}

		// Token: 0x06003CB6 RID: 15542 RVA: 0x001CED70 File Offset: 0x001CCF70
		public int Popup(int selected, string[] displayedOptions, string label = null, Rect rect = default(Rect), Layout.Val disabled = default(Layout.Val), string tooltip = null)
		{
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return selected;
			}
			new GUIContent(label, tooltip);
			if (selected != selected)
			{
				this.SetChange(true);
				return selected;
			}
			this.SetChange(false);
			return selected;
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x001CEE14 File Offset: 0x001CD014
		public T ScriptableAssetField<T>(T asset, Func<T> construct, string savePath = null, Layout.Val fieldSize = default(Layout.Val)) where T : ScriptableObject, ISerializationCallbackReceiver
		{
			fieldSize.Verify(this.fieldSize);
			bool flag = this.change;
			this.change = false;
			this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
			this.Label("Data", this.Inset(1f - fieldSize.val, default(Layout.Val), default(Layout.Val), default(Layout.Val)), null, false, 0, default(Layout.Val), default(Layout.Val), FontStyle.Normal, TextAnchor.UpperLeft, false, null, null);
			asset = this.Field<T>(asset, null, this.Inset(fieldSize.val, default(Layout.Val), default(Layout.Val), default(Layout.Val)), -200000000f, 200000000f, true, default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), null, null);
			if (this.lastChange && asset != null)
			{
				asset.OnAfterDeserialize();
			}
			if (this.change)
			{
				this.lastChange = true;
			}
			this.change = (flag || this.lastChange);
			return asset;
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x001CEF9F File Offset: 0x001CD19F
		public void AssetNewSaveField<T>(ref T asset, string label, Rect rect = default(Rect), string saveFilename = "Data.asset", string saveType = "asset", Func<T> create = null) where T : Object
		{
			asset = this.AssetNewSaveField<T>(asset, label, rect, saveFilename, saveType, create);
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x001CEFBC File Offset: 0x001CD1BC
		public T AssetNewSaveField<T>(T asset, string label, Rect rect = default(Rect), string saveFilename = "Data", string saveType = "asset", Func<T> create = null) where T : Object
		{
			bool flag = this.change;
			this.change = false;
			this.CheckStyles();
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return asset;
			}
			Rect rect2 = rect;
			rect2.width *= 0.62f;
			asset = this.Field<T>(asset, label, rect2, -200000000f, 200000000f, true, 0.55f, default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val), null, null);
			Rect rect3 = rect;
			rect3.width = rect3.width * 0.19f - (float)this.horizontalPadding;
			rect3.x += rect2.width + (float)this.horizontalPadding;
			this.Button("New", rect3, default(Layout.Val), default(Layout.Val), null, false, 0, null, null);
			if (this.change)
			{
				this.lastChange = true;
			}
			this.change = (this.change || flag);
			return asset;
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x000F3767 File Offset: 0x000F1967
		public T SaveAsset<T>(T asset, string savePath = null, string filename = "Data", string type = "asset", string caption = "Save Data as Unity Asset") where T : Object
		{
			return asset;
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x000023FD File Offset: 0x000005FD
		public void SaveRawBytes(byte[] bytes, string savePath = null, string filename = "Data", string type = "asset")
		{
		}

		// Token: 0x06003CBC RID: 15548 RVA: 0x000F3767 File Offset: 0x000F1967
		private T ReleaseAsset<T>(T asset, string savePath = null) where T : ScriptableObject, ISerializationCallbackReceiver
		{
			return asset;
		}

		// Token: 0x06003CBD RID: 15549 RVA: 0x001CF160 File Offset: 0x001CD360
		public T LoadAsset<T>(string label = "Load Unity Asset", string[] filters = null) where T : Object
		{
			return default(T);
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x001CF178 File Offset: 0x001CD378
		public bool DragDrop(Rect initialRect, int id, Action<Vector2, Rect> onDrag = null, Action<Vector2, Rect> onPress = null, Action<Vector2, Rect> onRelease = null)
		{
			Vector2 vector = this.ToInternal(Event.current.mousePosition);
			if (id == this.dragId)
			{
				this.dragState = Layout.DragState.Drag;
			}
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && initialRect.Contains(vector))
			{
				this.dragOffset = new Vector2(initialRect.x, initialRect.y) - vector;
				this.dragId = id;
				this.dragState = Layout.DragState.Pressed;
			}
			if (Event.current.rawType == EventType.MouseUp && id == this.dragId)
			{
				this.dragState = Layout.DragState.Released;
			}
			if (id != this.dragId)
			{
				return false;
			}
			this.dragDelta = vector - this.dragPos;
			this.dragPos = vector;
			this.dragRect = new Rect(vector.x + this.dragOffset.x, vector.y + this.dragOffset.y, initialRect.width, initialRect.height);
			switch (this.dragState)
			{
			case Layout.DragState.Pressed:
				if (onPress != null)
				{
					onPress(this.dragPos, this.dragRect);
				}
				break;
			case Layout.DragState.Drag:
				if (onDrag != null)
				{
					onDrag(this.dragPos, this.dragRect);
				}
				break;
			case Layout.DragState.Released:
				if (onRelease != null)
				{
					onRelease(this.dragPos, this.dragRect);
				}
				break;
			}
			if (this.dragState == Layout.DragState.Released)
			{
				this.dragId = -2000000000;
			}
			return true;
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x001CF2EC File Offset: 0x001CD4EC
		public Rect ResizeRect(Rect rectBase, int id, int border = 6, bool sideResize = true)
		{
			Rect rect = this.ToDisplay(rectBase);
			Rect rect2 = new Rect(rect.x + rect.width - (float)(border / 2), rect.y, (float)border, rect.height);
			Rect rect3 = new Rect(rect.x - (float)(border / 2), rect.y, (float)border, rect.height);
			Rect rect4 = new Rect(rect.x, rect.y - (float)(border / 2), rect.width, (float)border);
			Rect rect5 = new Rect(rect.x, rect.y + rect.height - (float)(border / 2), rect.width, (float)border);
			Rect rect6 = new Rect(rect.x + rect.width - (float)border, rect.y - (float)border, (float)(border * 2), (float)(border * 2));
			Rect rect7 = new Rect(rect.x - (float)border, rect.y - (float)border, (float)(border * 2), (float)(border * 2));
			Rect rect8 = new Rect(rect.x + rect.width - (float)border, rect.y + rect.height - (float)border, (float)(border * 2), (float)(border * 2));
			Rect rect9 = new Rect(rect.x - (float)border, rect.y + rect.height - (float)border, (float)(border * 2), (float)(border * 2));
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = rect6.Contains(mousePosition) || rect7.Contains(mousePosition) || rect8.Contains(mousePosition) || rect9.Contains(mousePosition);
			if (sideResize)
			{
				flag = (flag || rect2.Contains(mousePosition) || rect3.Contains(mousePosition) || rect4.Contains(mousePosition) || rect5.Contains(mousePosition));
			}
			if (Event.current.type == EventType.MouseDown && flag)
			{
				this.dragId = id;
				this.dragPos = Event.current.mousePosition;
				this.dragInitialRect = rect;
				if (sideResize)
				{
					if (rect2.Contains(mousePosition))
					{
						this.dragSide = Layout.DragSide.right;
					}
					else if (rect3.Contains(mousePosition))
					{
						this.dragSide = Layout.DragSide.left;
					}
					else if (rect4.Contains(mousePosition))
					{
						this.dragSide = Layout.DragSide.top;
					}
					else if (rect5.Contains(mousePosition))
					{
						this.dragSide = Layout.DragSide.bottom;
					}
				}
				if (rect6.Contains(mousePosition))
				{
					this.dragSide = Layout.DragSide.rightTop;
				}
				if (rect7.Contains(mousePosition))
				{
					this.dragSide = Layout.DragSide.leftTop;
				}
				if (rect8.Contains(mousePosition))
				{
					this.dragSide = Layout.DragSide.rightBottom;
				}
				if (rect9.Contains(mousePosition))
				{
					this.dragSide = Layout.DragSide.leftBottom;
				}
			}
			if (id == this.dragId)
			{
				Vector2 vector = Event.current.mousePosition - this.dragPos;
				if (this.dragSide == Layout.DragSide.right || this.dragSide == Layout.DragSide.rightTop || this.dragSide == Layout.DragSide.rightBottom)
				{
					rect.width = this.dragInitialRect.width + vector.x;
				}
				if (this.dragSide == Layout.DragSide.left || this.dragSide == Layout.DragSide.leftTop || this.dragSide == Layout.DragSide.leftBottom)
				{
					rect.width = this.dragInitialRect.width - vector.x;
					rect.x = this.dragInitialRect.x + vector.x;
				}
				if (this.dragSide == Layout.DragSide.top || this.dragSide == Layout.DragSide.leftTop || this.dragSide == Layout.DragSide.rightTop)
				{
					rect.height = this.dragInitialRect.height - vector.y;
					rect.y = this.dragInitialRect.y + vector.y;
				}
				if (this.dragSide == Layout.DragSide.bottom || this.dragSide == Layout.DragSide.leftBottom || this.dragSide == Layout.DragSide.rightBottom)
				{
					rect.height = this.dragInitialRect.height + vector.y;
				}
			}
			if (Event.current.rawType == EventType.MouseUp && id == this.dragId)
			{
				this.dragId = -2000000000;
			}
			if (id == this.dragId)
			{
				return this.ToInternal(rect);
			}
			return rectBase;
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x001CF6E8 File Offset: 0x001CD8E8
		public Rect GetBackgroundRect(Action<Layout> onGUI, bool fullWidth = true)
		{
			int num = this.margin;
			int num2 = this.rightMargin;
			this.Par(0, default(Layout.Val), 0);
			Rect rect = this.cursor;
			bool flag = this.markup;
			this.markup = true;
			onGUI(this);
			this.markup = flag;
			this.Par(0, default(Layout.Val), 0);
			Rect rect2 = this.cursor;
			Rect result = new Rect(rect.x, rect.y, rect2.x - rect.x, rect2.y - rect.y);
			result.y += this.field.y;
			if (fullWidth)
			{
				result.x = this.field.x;
				result.width = this.field.width;
			}
			this.cursor = rect;
			this.margin = num;
			this.rightMargin = num2;
			return result;
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x001CF7F0 File Offset: 0x001CD9F0
		public void Layer(Layout.ILayered obj, int num, Layout.Val disabled = default(Layout.Val), bool displayFoldout = true, int expandHeaderBackground = 0, string tooltip = null)
		{
			this.CheckStyles();
			if (this.markup)
			{
				return;
			}
			bool flag = obj.selected == num;
			bool flag2 = obj.expanded && flag;
			Rect backgroundRect;
			if (flag2)
			{
				backgroundRect = this.GetBackgroundRect(delegate(Layout tmp)
				{
					obj.OnLayerHeader(this, num);
					obj.OnLayerGUI(this, num);
				}, true);
			}
			else
			{
				backgroundRect = this.GetBackgroundRect(delegate(Layout tmp)
				{
					obj.OnLayerHeader(this, num);
				}, true);
			}
			backgroundRect.x = this.field.x + (float)this.margin;
			backgroundRect.width = this.field.width - (float)this.margin - (float)this.rightMargin;
			backgroundRect.height += (float)expandHeaderBackground;
			obj.OnLayerHeader(this, num);
			float f = (float)(displayFoldout ? 18 : 0);
			if (displayFoldout)
			{
				bool flag3 = flag2;
				Rect rect = this.Inset(f, default(Layout.Val), default(Layout.Val), default(Layout.Val));
				this.Icon(flag2 ? "DPLayout_ChevronDown" : "DPLayout_ChevronLeft", rect, Layout.IconAligment.center, Layout.IconAligment.center, 0, false, false, false);
				if (Event.current.type == EventType.MouseDown && rect.Contains(this.ToInternal(Event.current.mousePosition)))
				{
					flag3 = !flag2;
				}
				if (flag3 != flag2)
				{
					if (!flag && flag3)
					{
						obj.selected = num;
					}
					obj.expanded = flag3;
					this.Repaint(3);
				}
			}
			this.cursor.y = this.cursor.y + (float)expandHeaderBackground;
			if (flag2)
			{
				bool flag4 = this.change;
				this.change = false;
				obj.OnLayerGUI(this, num);
				if (this.change)
				{
					this.lastChange = true;
				}
				this.change = (this.change || flag4);
			}
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x001CF9EC File Offset: 0x001CDBEC
		public void LayerButtons(Layout.ILayered obj, int count, string label = null, Rect rect = default(Rect), bool addBeforeSelected = false, string tooltip = null)
		{
			if (rect.width < 0.9f && rect.height < 0.9f)
			{
				this.Par(default(Layout.Val), default(Layout.Val), default(Layout.Val));
				rect = this.Inset(default(Layout.Val), default(Layout.Val), default(Layout.Val), default(Layout.Val));
			}
			if (this.markup)
			{
				return;
			}
			bool flag = this.change;
			this.change = false;
			float num = (label != null) ? (rect.width - 100f) : 0f;
			if (label != null)
			{
				Rect rect2 = new Rect(rect.x, rect.y, num, rect.height);
				this.Label(label, rect2, null, false, 0, default(Layout.Val), default(Layout.Val), FontStyle.Normal, TextAnchor.UpperLeft, false, null, null);
			}
			Rect rect3 = new Rect(rect.x + num, rect.y, 25f, rect.height);
			if (this.Button(null, rect3, default(Layout.Val), default(Layout.Val), "DPLayout_Add", true, 0, null, "Add new array element"))
			{
				int num2;
				if (!addBeforeSelected)
				{
					num2 = obj.selected + 1;
					if (obj.selected < 0)
					{
						num2 = count;
					}
				}
				else
				{
					num2 = obj.selected;
					if (obj.selected < 0)
					{
						num2 = 0;
					}
				}
				obj.AddLayer(num2);
				int selected = obj.selected;
				obj.selected = selected + 1;
				if (obj.selected >= 0)
				{
					if (addBeforeSelected)
					{
						selected = obj.selected;
						obj.selected = selected + 1;
					}
					obj.selected = Mathf.Clamp(obj.selected, 0, count - 1);
				}
				this.change = true;
			}
			rect3.x += 25f;
			if (this.Button(null, rect3, default(Layout.Val), default(Layout.Val), "DPLayout_Remove", true, 0, null, "Remove element") && obj.selected < count && obj.selected >= 0)
			{
				obj.RemoveLayer(obj.selected);
				obj.selected = Mathf.Clamp(obj.selected, 0, count - 1);
				this.change = true;
			}
			rect3.x += 25f;
			if (this.Button(null, rect3, default(Layout.Val), default(Layout.Val), "DPLayout_Up", true, 0, null, "Move selected up") && obj.selected < count && obj.selected >= 1)
			{
				obj.SwitchLayers(obj.selected, obj.selected - 1);
				int selected = obj.selected;
				obj.selected = selected - 1;
				obj.selected = Mathf.Clamp(obj.selected, 0, count - 1);
				this.change = true;
			}
			rect3.x += 25f;
			if (this.Button(null, rect3, default(Layout.Val), default(Layout.Val), "DPLayout_Down", true, 0, null, "Move selected up") && obj.selected < count - 1 && obj.selected >= 0)
			{
				obj.SwitchLayers(obj.selected, obj.selected + 1);
				int selected = obj.selected;
				obj.selected = selected + 1;
				obj.selected = Mathf.Clamp(obj.selected, 0, count - 1);
				this.change = true;
			}
			this.lastChange = this.change;
			this.change = (flag || this.change);
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x001CFD60 File Offset: 0x001CDF60
		public void Foreground(Rect startAnchor, Rect endAnchor, int padding = 3)
		{
			Vector2 position = startAnchor.position;
			Vector2 vector = endAnchor.position + endAnchor.size;
			this.Element("DPLayout_FoldoutBackground", new Rect(position.x - (float)padding, position.y - (float)padding, vector.x - position.x + (float)(padding * 2), vector.y - position.y + (float)(padding * 2)), new RectOffset(6, 6, 6, 6), null);
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x001CFDDC File Offset: 0x001CDFDC
		public void Foreground(Rect startAnchor, int padding = 3)
		{
			this.Par(0, default(Layout.Val), 0);
			this.Inset(1, default(Layout.Val), default(Layout.Val), default(Layout.Val));
			Rect rect = this.lastRect;
			Vector2 position = startAnchor.position;
			Vector2 vector = rect.position + rect.size;
			this.Element("DPLayout_FoldoutBackground", new Rect(position.x - (float)padding, position.y - (float)padding, vector.x - position.x + (float)(padding * 2), vector.y - position.y + (float)(padding * 2)), new RectOffset(6, 6, 6, 6), null);
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x001CFEA0 File Offset: 0x001CE0A0
		public void MatKeyword(Material mat, string keyword, string label)
		{
			bool flag = mat.IsKeywordEnabled(keyword);
			this.Toggle(ref flag, label, default(Rect), default(Layout.Val), default(Layout.Val), null, null, null);
			if (this.lastChange)
			{
				if (flag)
				{
					mat.EnableKeyword(keyword);
					return;
				}
				mat.DisableKeyword(keyword);
			}
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x001CFEF8 File Offset: 0x001CE0F8
		public void MatField<T>(Material mat, string name, string label = null, Rect rect = default(Rect), float min = -200000000f, float max = 200000000f, bool limit = true, Layout.Val fieldSize = default(Layout.Val), Layout.Val sliderSize = default(Layout.Val), Layout.Val monitorChange = default(Layout.Val), Layout.Val useEvent = default(Layout.Val), Layout.Val disabled = default(Layout.Val), Layout.Val dragChange = default(Layout.Val), Layout.Val slider = default(Layout.Val), Layout.Val quadratic = default(Layout.Val), Layout.Val allowSceneObject = default(Layout.Val), Layout.Val delayed = default(Layout.Val), GUIStyle style = null, string tooltip = null, bool zwOfVector4 = false)
		{
			if (mat == null || !mat.HasProperty(name))
			{
				return;
			}
			Vector4 vector = default(Vector4);
			T t = default(T);
			if (typeof(T) == typeof(float))
			{
				t = (T)((object)mat.GetFloat(name));
			}
			else if (typeof(T) == typeof(int))
			{
				t = (T)((object)mat.GetInt(name));
			}
			else if (typeof(T) == typeof(Color))
			{
				t = (T)((object)mat.GetColor(name));
			}
			else if (typeof(T) == typeof(Vector2))
			{
				vector = mat.GetVector(name);
				if (!zwOfVector4)
				{
					t = (T)((object)new Vector2(vector.x, vector.y));
				}
				else
				{
					t = (T)((object)new Vector2(vector.z, vector.w));
				}
			}
			else if (typeof(T) == typeof(Vector4))
			{
				t = (T)((object)mat.GetVector(name));
			}
			else if (typeof(T) == typeof(Texture))
			{
				t = (T)((object)mat.GetTexture(name));
			}
			t = this.Field<T>(t, label, rect, min, max, limit, fieldSize, sliderSize, monitorChange, this.markup, useEvent, disabled, dragChange, slider, quadratic, allowSceneObject, delayed, style, tooltip);
			if (this.lastChange)
			{
				if (typeof(T) == typeof(float))
				{
					mat.SetFloat(name, (float)((object)t));
					return;
				}
				if (typeof(T) == typeof(int))
				{
					mat.SetInt(name, (int)((object)t));
					return;
				}
				if (typeof(T) == typeof(Color))
				{
					mat.SetColor(name, (Color)((object)t));
					return;
				}
				if (typeof(T) == typeof(Vector2))
				{
					Vector2 vector2 = (Vector2)((object)t);
					if (!zwOfVector4)
					{
						vector.x = vector2.x;
						vector.y = vector2.y;
					}
					else
					{
						vector.z = vector2.x;
						vector.w = vector2.y;
					}
					mat.SetVector(name, vector);
					return;
				}
				if (typeof(T) == typeof(Vector4) || typeof(T) == typeof(Vector2))
				{
					mat.SetVector(name, (Vector4)((object)t));
					return;
				}
				if (typeof(T) == typeof(Texture))
				{
					mat.SetTexture(name, (Texture)((object)t));
				}
			}
		}

		// Token: 0x04002B77 RID: 11127
		public Rect field;

		// Token: 0x04002B78 RID: 11128
		public Rect cursor;

		// Token: 0x04002B79 RID: 11129
		public Rect lastRect;

		// Token: 0x04002B7A RID: 11130
		public int margin = 10;

		// Token: 0x04002B7B RID: 11131
		public int rightMargin = 10;

		// Token: 0x04002B7C RID: 11132
		public int lineHeight = 18;

		// Token: 0x04002B7D RID: 11133
		public float bottomPoint;

		// Token: 0x04002B7E RID: 11134
		public int verticalPadding = 2;

		// Token: 0x04002B7F RID: 11135
		public int horizontalPadding = 3;

		// Token: 0x04002B80 RID: 11136
		public Object undoObject;

		// Token: 0x04002B81 RID: 11137
		public string undoName = "";

		// Token: 0x04002B82 RID: 11138
		public int repaintsLeft;

		// Token: 0x04002B83 RID: 11139
		public Vector2 scroll = new Vector2(0f, 0f);

		// Token: 0x04002B84 RID: 11140
		public float zoom = 1f;

		// Token: 0x04002B85 RID: 11141
		public float zoomStep = 0.0625f;

		// Token: 0x04002B86 RID: 11142
		public float minZoom = 0.25f;

		// Token: 0x04002B87 RID: 11143
		public float maxZoom = 2f;

		// Token: 0x04002B88 RID: 11144
		public int scrollButton = 2;

		// Token: 0x04002B89 RID: 11145
		[NonSerialized]
		public GUIStyle labelStyle;

		// Token: 0x04002B8A RID: 11146
		[NonSerialized]
		public GUIStyle boldLabelStyle;

		// Token: 0x04002B8B RID: 11147
		[NonSerialized]
		public GUIStyle foldoutStyle;

		// Token: 0x04002B8C RID: 11148
		[NonSerialized]
		public GUIStyle fieldStyle;

		// Token: 0x04002B8D RID: 11149
		[NonSerialized]
		public GUIStyle dragFieldStyle;

		// Token: 0x04002B8E RID: 11150
		[NonSerialized]
		public GUIStyle buttonStyle;

		// Token: 0x04002B8F RID: 11151
		[NonSerialized]
		public GUIStyle enumZoomStyle;

		// Token: 0x04002B90 RID: 11152
		[NonSerialized]
		public GUIStyle urlStyle;

		// Token: 0x04002B91 RID: 11153
		[NonSerialized]
		public GUIStyle toolbarStyle;

		// Token: 0x04002B92 RID: 11154
		[NonSerialized]
		public GUIStyle toolbarButtonStyle;

		// Token: 0x04002B93 RID: 11155
		[NonSerialized]
		public GUIStyle helpBoxStyle;

		// Token: 0x04002B94 RID: 11156
		[NonSerialized]
		private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

		// Token: 0x04002B95 RID: 11157
		private static Material iconMat;

		// Token: 0x04002B96 RID: 11158
		private static Material textureMat;

		// Token: 0x04002B97 RID: 11159
		private static readonly string[] textureSimpleIcons = new string[]
		{
			"DPLayout_TexCh_Color",
			"DPLayout_TexCh_Alpha",
			"DPLayout_TexCh_Normal"
		};

		// Token: 0x04002B98 RID: 11160
		private static readonly string[] textureModeIcons = new string[]
		{
			"DPLayout_TexCh_Color",
			"DPLayout_TexCh_Linear",
			"DPLayout_TexCh_Normal"
		};

		// Token: 0x04002B99 RID: 11161
		private static readonly string[] textureChannelsIcons = new string[]
		{
			"DPLayout_TexCh_RGB",
			"DPLayout_TexCh_Red",
			"DPLayout_TexCh_Green",
			"DPLayout_TexCh_Blue",
			"DPLayout_TexCh_Alpha"
		};

		// Token: 0x04002B9A RID: 11162
		[NonSerialized]
		private Dictionary<string, GUIStyle> elementStyles = new Dictionary<string, GUIStyle>();

		// Token: 0x04002B9B RID: 11163
		private Vector2 sliderClickPos;

		// Token: 0x04002B9C RID: 11164
		private int sliderDraggingId = -20000000;

		// Token: 0x04002B9D RID: 11165
		private float sliderOriginalValue;

		// Token: 0x04002B9F RID: 11167
		private static Rect currentlyFocusedRect;

		// Token: 0x04002BA0 RID: 11168
		public bool change;

		// Token: 0x04002BA1 RID: 11169
		public bool lastChange;

		// Token: 0x04002BA2 RID: 11170
		public float fieldSize = 0.5f;

		// Token: 0x04002BA3 RID: 11171
		public float sliderSize = 0.5f;

		// Token: 0x04002BA4 RID: 11172
		public bool monitorChange = true;

		// Token: 0x04002BA5 RID: 11173
		public bool markup;

		// Token: 0x04002BA6 RID: 11174
		public bool useEvent;

		// Token: 0x04002BA7 RID: 11175
		public bool disabled;

		// Token: 0x04002BA8 RID: 11176
		public int fontSize = 11;

		// Token: 0x04002BA9 RID: 11177
		public int iconOffset = 4;

		// Token: 0x04002BAA RID: 11178
		public bool dragChange;

		// Token: 0x04002BAB RID: 11179
		public bool slider;

		// Token: 0x04002BAC RID: 11180
		public bool delayed;

		// Token: 0x04002BAD RID: 11181
		private Type curveWindowType;

		// Token: 0x04002BAE RID: 11182
		private AnimationCurve windowCurveRef;

		// Token: 0x04002BAF RID: 11183
		public Layout.DragState dragState;

		// Token: 0x04002BB0 RID: 11184
		public Rect dragRect;

		// Token: 0x04002BB1 RID: 11185
		public Vector2 dragPos;

		// Token: 0x04002BB2 RID: 11186
		public Vector2 dragDelta;

		// Token: 0x04002BB3 RID: 11187
		public Vector2 dragOffset;

		// Token: 0x04002BB4 RID: 11188
		public int dragId = -2000000000;

		// Token: 0x04002BB5 RID: 11189
		public Layout.DragSide dragSide;

		// Token: 0x04002BB6 RID: 11190
		public Rect dragInitialRect;

		// Token: 0x02000960 RID: 2400
		public struct Val
		{
			// Token: 0x0600539B RID: 21403 RVA: 0x00243C70 File Offset: 0x00241E70
			public static implicit operator Layout.Val(bool b)
			{
				return new Layout.Val
				{
					val = (float)(b ? 1 : 0),
					ovd = true
				};
			}

			// Token: 0x0600539C RID: 21404 RVA: 0x00243CA0 File Offset: 0x00241EA0
			public static implicit operator Layout.Val(float f)
			{
				return new Layout.Val
				{
					val = f,
					ovd = true
				};
			}

			// Token: 0x0600539D RID: 21405 RVA: 0x00243CC8 File Offset: 0x00241EC8
			public static implicit operator Layout.Val(int i)
			{
				return new Layout.Val
				{
					val = (float)i,
					ovd = true
				};
			}

			// Token: 0x0600539E RID: 21406 RVA: 0x00243CEF File Offset: 0x00241EEF
			public static implicit operator bool(Layout.Val v)
			{
				return v.val > 0.5f;
			}

			// Token: 0x0600539F RID: 21407 RVA: 0x00243D01 File Offset: 0x00241F01
			public static implicit operator float(Layout.Val v)
			{
				return v.val;
			}

			// Token: 0x060053A0 RID: 21408 RVA: 0x00243D09 File Offset: 0x00241F09
			public static implicit operator int(Layout.Val v)
			{
				return (int)v.val;
			}

			// Token: 0x060053A1 RID: 21409 RVA: 0x00243D12 File Offset: 0x00241F12
			public void Verify(float def)
			{
				if (!this.ovd)
				{
					this.val = def;
				}
			}

			// Token: 0x060053A2 RID: 21410 RVA: 0x00243D23 File Offset: 0x00241F23
			public void Verify(int def)
			{
				if (!this.ovd)
				{
					this.val = (float)def;
				}
			}

			// Token: 0x060053A3 RID: 21411 RVA: 0x00243D35 File Offset: 0x00241F35
			public void Verify(bool def)
			{
				if (!this.ovd)
				{
					this.val = (float)(def ? 1 : 0);
				}
			}

			// Token: 0x04004383 RID: 17283
			public float val;

			// Token: 0x04004384 RID: 17284
			public bool ovd;
		}

		// Token: 0x02000961 RID: 2401
		public enum IconAligment
		{
			// Token: 0x04004386 RID: 17286
			resize,
			// Token: 0x04004387 RID: 17287
			min,
			// Token: 0x04004388 RID: 17288
			max,
			// Token: 0x04004389 RID: 17289
			center
		}

		// Token: 0x02000962 RID: 2402
		// (Invoke) Token: 0x060053A5 RID: 21413
		public delegate void ChangeAction();

		// Token: 0x02000963 RID: 2403
		public enum HelpboxType
		{
			// Token: 0x0400438B RID: 17291
			off,
			// Token: 0x0400438C RID: 17292
			empty,
			// Token: 0x0400438D RID: 17293
			info,
			// Token: 0x0400438E RID: 17294
			warning,
			// Token: 0x0400438F RID: 17295
			error
		}

		// Token: 0x02000964 RID: 2404
		public enum DragState
		{
			// Token: 0x04004391 RID: 17297
			Pressed,
			// Token: 0x04004392 RID: 17298
			Drag,
			// Token: 0x04004393 RID: 17299
			Released
		}

		// Token: 0x02000965 RID: 2405
		public enum DragSide
		{
			// Token: 0x04004395 RID: 17301
			right,
			// Token: 0x04004396 RID: 17302
			left,
			// Token: 0x04004397 RID: 17303
			top,
			// Token: 0x04004398 RID: 17304
			bottom,
			// Token: 0x04004399 RID: 17305
			rightTop,
			// Token: 0x0400439A RID: 17306
			leftTop,
			// Token: 0x0400439B RID: 17307
			rightBottom,
			// Token: 0x0400439C RID: 17308
			leftBottom
		}

		// Token: 0x02000966 RID: 2406
		public interface ILayered
		{
			// Token: 0x060053A8 RID: 21416
			void OnLayerHeader(Layout layout, int num);

			// Token: 0x060053A9 RID: 21417
			void OnLayerGUI(Layout layout, int num);

			// Token: 0x060053AA RID: 21418
			void AddLayer(int num);

			// Token: 0x060053AB RID: 21419
			void RemoveLayer(int num);

			// Token: 0x060053AC RID: 21420
			void SwitchLayers(int n1, int n2);

			// Token: 0x170006D5 RID: 1749
			// (get) Token: 0x060053AD RID: 21421
			// (set) Token: 0x060053AE RID: 21422
			int selected { get; set; }

			// Token: 0x170006D6 RID: 1750
			// (get) Token: 0x060053AF RID: 21423
			// (set) Token: 0x060053B0 RID: 21424
			bool expanded { get; set; }
		}
	}
}
