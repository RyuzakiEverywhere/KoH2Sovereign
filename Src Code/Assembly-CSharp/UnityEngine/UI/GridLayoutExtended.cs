using System;

namespace UnityEngine.UI
{
	// Token: 0x02000347 RID: 839
	[AddComponentMenu("Layout/Grid Layout Extended", 159)]
	public class GridLayoutExtended : LayoutGroup
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060032A1 RID: 12961 RVA: 0x0019ADE0 File Offset: 0x00198FE0
		// (set) Token: 0x060032A2 RID: 12962 RVA: 0x0019ADE8 File Offset: 0x00198FE8
		public GridLayoutExtended.Corner startCorner
		{
			get
			{
				return this.m_StartCorner;
			}
			set
			{
				base.SetProperty<GridLayoutExtended.Corner>(ref this.m_StartCorner, value);
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060032A3 RID: 12963 RVA: 0x0019ADF7 File Offset: 0x00198FF7
		// (set) Token: 0x060032A4 RID: 12964 RVA: 0x0019ADFF File Offset: 0x00198FFF
		public GridLayoutExtended.Axis startAxis
		{
			get
			{
				return this.m_StartAxis;
			}
			set
			{
				base.SetProperty<GridLayoutExtended.Axis>(ref this.m_StartAxis, value);
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060032A5 RID: 12965 RVA: 0x0019AE0E File Offset: 0x0019900E
		// (set) Token: 0x060032A6 RID: 12966 RVA: 0x0019AE16 File Offset: 0x00199016
		public Vector2 cellSize
		{
			get
			{
				return this.m_CellSize;
			}
			set
			{
				base.SetProperty<Vector2>(ref this.m_CellSize, value);
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060032A7 RID: 12967 RVA: 0x0019AE25 File Offset: 0x00199025
		// (set) Token: 0x060032A8 RID: 12968 RVA: 0x0019AE2D File Offset: 0x0019902D
		public Vector2 spacing
		{
			get
			{
				return this.m_Spacing;
			}
			set
			{
				base.SetProperty<Vector2>(ref this.m_Spacing, value);
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060032A9 RID: 12969 RVA: 0x0019AE3C File Offset: 0x0019903C
		// (set) Token: 0x060032AA RID: 12970 RVA: 0x0019AE44 File Offset: 0x00199044
		public GridLayoutExtended.Constraint constraint
		{
			get
			{
				return this.m_Constraint;
			}
			set
			{
				base.SetProperty<GridLayoutExtended.Constraint>(ref this.m_Constraint, value);
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060032AB RID: 12971 RVA: 0x0019AE53 File Offset: 0x00199053
		// (set) Token: 0x060032AC RID: 12972 RVA: 0x0019AE5B File Offset: 0x0019905B
		public int constraintCount
		{
			get
			{
				return this.m_ConstraintCount;
			}
			set
			{
				base.SetProperty<int>(ref this.m_ConstraintCount, Mathf.Max(1, value));
			}
		}

		// Token: 0x060032AD RID: 12973 RVA: 0x0019AE70 File Offset: 0x00199070
		protected GridLayoutExtended()
		{
		}

		// Token: 0x060032AE RID: 12974 RVA: 0x0019AEA0 File Offset: 0x001990A0
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			int num2;
			int num;
			if (this.m_Constraint == GridLayoutExtended.Constraint.FixedColumnCount)
			{
				num = (num2 = this.m_ConstraintCount);
			}
			else if (this.m_Constraint == GridLayoutExtended.Constraint.FixedRowCount)
			{
				num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.m_ConstraintCount - 0.001f));
			}
			else
			{
				num2 = 1;
				num = Mathf.CeilToInt(Mathf.Sqrt((float)base.rectChildren.Count));
			}
			base.SetLayoutInputForAxis((float)base.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)num2 - this.spacing.x, (float)base.padding.horizontal + (this.cellSize.x + this.spacing.x) * (float)num - this.spacing.x, -1f, 0);
		}

		// Token: 0x060032AF RID: 12975 RVA: 0x0019AF84 File Offset: 0x00199184
		public override void CalculateLayoutInputVertical()
		{
			int num;
			if (this.m_Constraint == GridLayoutExtended.Constraint.FixedColumnCount)
			{
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.m_ConstraintCount - 0.001f);
			}
			else if (this.m_Constraint == GridLayoutExtended.Constraint.FixedRowCount)
			{
				num = this.m_ConstraintCount;
			}
			else
			{
				float width = base.rectTransform.rect.width;
				int num2 = Mathf.Max(1, Mathf.FloorToInt((width - (float)base.padding.horizontal + this.spacing.x + 0.001f) / (this.cellSize.x + this.spacing.x)));
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num2);
			}
			float num3 = (float)base.padding.vertical + (this.cellSize.y + this.spacing.y) * (float)num - this.spacing.y;
			base.SetLayoutInputForAxis(num3, num3, -1f, 1);
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x0019B083 File Offset: 0x00199283
		public override void SetLayoutHorizontal()
		{
			this.SetCellsAlongAxis(0);
		}

		// Token: 0x060032B1 RID: 12977 RVA: 0x0019B08C File Offset: 0x0019928C
		public override void SetLayoutVertical()
		{
			this.SetCellsAlongAxis(1);
		}

		// Token: 0x060032B2 RID: 12978 RVA: 0x0019B098 File Offset: 0x00199298
		private void SetCellsAlongAxis(int axis)
		{
			if (axis == 0)
			{
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					this.m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
					rectTransform.anchorMin = Vector2.up;
					rectTransform.anchorMax = Vector2.up;
					rectTransform.sizeDelta = this.cellSize;
				}
				return;
			}
			float x = base.rectTransform.rect.size.x;
			float y = base.rectTransform.rect.size.y;
			int num = 1;
			int num2 = 1;
			if (this.m_Constraint == GridLayoutExtended.Constraint.FixedColumnCount)
			{
				num = this.m_ConstraintCount;
				if (base.rectChildren.Count > num)
				{
					num2 = base.rectChildren.Count / num + ((base.rectChildren.Count % num > 0) ? 1 : 0);
				}
			}
			else if (this.m_Constraint == GridLayoutExtended.Constraint.FixedRowCount)
			{
				num2 = this.m_ConstraintCount;
				if (base.rectChildren.Count > num2)
				{
					num = base.rectChildren.Count / num2 + ((base.rectChildren.Count % num2 > 0) ? 1 : 0);
				}
			}
			else
			{
				if (this.cellSize.x + this.spacing.x <= 0f)
				{
					num = int.MaxValue;
				}
				else
				{
					num = Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + this.spacing.x + 0.001f) / (this.cellSize.x + this.spacing.x)));
				}
				if (this.cellSize.y + this.spacing.y <= 0f)
				{
					num2 = int.MaxValue;
				}
				else
				{
					num2 = Mathf.Max(1, Mathf.FloorToInt((y - (float)base.padding.vertical + this.spacing.y + 0.001f) / (this.cellSize.y + this.spacing.y)));
				}
			}
			int num3 = (int)(this.startCorner % GridLayoutExtended.Corner.LowerLeft);
			int num4 = (int)(this.startCorner / GridLayoutExtended.Corner.LowerLeft);
			int num5 = 0;
			int num6 = 0;
			int num7;
			int num8;
			int num9;
			if (this.startAxis == GridLayoutExtended.Axis.Horizontal)
			{
				num7 = num;
				num8 = Mathf.Clamp(num, 1, base.rectChildren.Count);
				num9 = Mathf.Clamp(num2, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num7));
				int num10 = (num8 != 0) ? (base.rectChildren.Count % num8) : 0;
				num5 = ((num10 == 0) ? 0 : (num8 - num10));
			}
			else
			{
				num7 = num2;
				num9 = Mathf.Clamp(num2, 1, base.rectChildren.Count);
				num8 = Mathf.Clamp(num, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num7));
				int num11 = (num9 != 0) ? (base.rectChildren.Count % num9) : 0;
				num6 = ((num11 == 0) ? 0 : (num9 - num11));
			}
			Vector2 vector = new Vector2((float)num8 * this.cellSize.x + (float)(num8 - 1) * this.spacing.x, (float)num9 * this.cellSize.y + (float)(num9 - 1) * this.spacing.y);
			Vector2 vector2 = new Vector2(base.GetStartOffset(0, vector.x), base.GetStartOffset(1, vector.y));
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				Vector2 vector3 = new Vector2(0f, 0f);
				int num12;
				int num13;
				if (this.startAxis == GridLayoutExtended.Axis.Horizontal)
				{
					num12 = j % num7;
					num13 = j / num7;
					if (num13 == num9 - 1 && num5 > 0)
					{
						vector3.x = (float)num5 * this.cellSize[0] / 2f;
						vector3.x += this.spacing[0] * (float)num5 / 2f;
					}
				}
				else
				{
					num12 = j / num7;
					num13 = j % num7;
					if (num12 == num8 - 1 && num6 > 0)
					{
						vector3.y = (float)num6 * this.cellSize[1] / 2f;
						vector3.y += this.spacing[1] * (float)num6 / 2f;
					}
				}
				if (num3 == 1)
				{
					num12 = num8 - 1 - num12;
					vector3.x = -vector3.x;
				}
				if (num4 == 1)
				{
					num13 = num9 - 1 - num13;
					vector3.y = -vector3.y;
				}
				base.SetChildAlongAxis(base.rectChildren[j], 0, vector3.x + vector2.x + (this.cellSize[0] + this.spacing[0]) * (float)num12, this.cellSize[0]);
				base.SetChildAlongAxis(base.rectChildren[j], 1, vector3.y + vector2.y + (this.cellSize[1] + this.spacing[1]) * (float)num13, this.cellSize[1]);
			}
		}

		// Token: 0x04002247 RID: 8775
		[SerializeField]
		protected GridLayoutExtended.Corner m_StartCorner;

		// Token: 0x04002248 RID: 8776
		[SerializeField]
		protected GridLayoutExtended.Axis m_StartAxis;

		// Token: 0x04002249 RID: 8777
		[SerializeField]
		protected Vector2 m_CellSize = new Vector2(100f, 100f);

		// Token: 0x0400224A RID: 8778
		[SerializeField]
		protected Vector2 m_Spacing = Vector2.zero;

		// Token: 0x0400224B RID: 8779
		[SerializeField]
		protected GridLayoutExtended.Constraint m_Constraint;

		// Token: 0x0400224C RID: 8780
		[SerializeField]
		protected int m_ConstraintCount = 2;

		// Token: 0x02000891 RID: 2193
		public enum Corner
		{
			// Token: 0x0400402A RID: 16426
			UpperLeft,
			// Token: 0x0400402B RID: 16427
			UpperRight,
			// Token: 0x0400402C RID: 16428
			LowerLeft,
			// Token: 0x0400402D RID: 16429
			LowerRight
		}

		// Token: 0x02000892 RID: 2194
		public enum Axis
		{
			// Token: 0x0400402F RID: 16431
			Horizontal,
			// Token: 0x04004030 RID: 16432
			Vertical
		}

		// Token: 0x02000893 RID: 2195
		public enum Constraint
		{
			// Token: 0x04004032 RID: 16434
			Flexible,
			// Token: 0x04004033 RID: 16435
			FixedColumnCount,
			// Token: 0x04004034 RID: 16436
			FixedRowCount
		}
	}
}
