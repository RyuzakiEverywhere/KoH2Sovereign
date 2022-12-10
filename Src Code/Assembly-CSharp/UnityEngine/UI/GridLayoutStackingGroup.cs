using System;

namespace UnityEngine.UI
{
	// Token: 0x02000348 RID: 840
	public class GridLayoutStackingGroup : LayoutGroup
	{
		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060032B3 RID: 12979 RVA: 0x0019B5DB File Offset: 0x001997DB
		// (set) Token: 0x060032B4 RID: 12980 RVA: 0x0019B5E3 File Offset: 0x001997E3
		public GridLayoutStackingGroup.Corner startCorner
		{
			get
			{
				return this.m_StartCorner;
			}
			set
			{
				base.SetProperty<GridLayoutStackingGroup.Corner>(ref this.m_StartCorner, value);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060032B5 RID: 12981 RVA: 0x0019B5F2 File Offset: 0x001997F2
		// (set) Token: 0x060032B6 RID: 12982 RVA: 0x0019B5FA File Offset: 0x001997FA
		public GridLayoutStackingGroup.Axis startAxis
		{
			get
			{
				return this.m_StartAxis;
			}
			set
			{
				base.SetProperty<GridLayoutStackingGroup.Axis>(ref this.m_StartAxis, value);
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060032B7 RID: 12983 RVA: 0x0019B609 File Offset: 0x00199809
		// (set) Token: 0x060032B8 RID: 12984 RVA: 0x0019B611 File Offset: 0x00199811
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

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x060032B9 RID: 12985 RVA: 0x0019B620 File Offset: 0x00199820
		// (set) Token: 0x060032BA RID: 12986 RVA: 0x0019B628 File Offset: 0x00199828
		public Vector2 maxSize
		{
			get
			{
				return this.m_MaxSize;
			}
			set
			{
				base.SetProperty<Vector2>(ref this.m_MaxSize, value);
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x060032BB RID: 12987 RVA: 0x0019B637 File Offset: 0x00199837
		// (set) Token: 0x060032BC RID: 12988 RVA: 0x0019B63F File Offset: 0x0019983F
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

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x060032BD RID: 12989 RVA: 0x0019B64E File Offset: 0x0019984E
		// (set) Token: 0x060032BE RID: 12990 RVA: 0x0019B656 File Offset: 0x00199856
		public Vector2 additionalSpacing
		{
			get
			{
				return this.m_AdditionalSpacing;
			}
			set
			{
				base.SetProperty<Vector2>(ref this.m_AdditionalSpacing, value);
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x060032BF RID: 12991 RVA: 0x0019B665 File Offset: 0x00199865
		// (set) Token: 0x060032C0 RID: 12992 RVA: 0x0019B66D File Offset: 0x0019986D
		public GridLayoutStackingGroup.Constraint constraint
		{
			get
			{
				return this.m_Constraint;
			}
			set
			{
				base.SetProperty<GridLayoutStackingGroup.Constraint>(ref this.m_Constraint, value);
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x060032C1 RID: 12993 RVA: 0x0019B67C File Offset: 0x0019987C
		// (set) Token: 0x060032C2 RID: 12994 RVA: 0x0019B684 File Offset: 0x00199884
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

		// Token: 0x060032C3 RID: 12995 RVA: 0x0019B69C File Offset: 0x0019989C
		protected GridLayoutStackingGroup()
		{
		}

		// Token: 0x060032C4 RID: 12996 RVA: 0x0019B700 File Offset: 0x00199900
		protected override void Awake()
		{
			if (this.maxSize == Vector2.zero)
			{
				RectTransform component = base.gameObject.GetComponent<RectTransform>();
				this.maxSize = component.sizeDelta;
			}
			base.Awake();
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x0019B740 File Offset: 0x00199940
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			int num;
			if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedColumnCount)
			{
				num = this.m_ConstraintCount;
			}
			else if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedRowCount)
			{
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.m_ConstraintCount - 0.001f);
			}
			else
			{
				num = 1;
				int num2 = Mathf.CeilToInt(Mathf.Sqrt((float)base.rectChildren.Count));
			}
			float num3 = (float)base.padding.horizontal + (this.cellSize.x + this.additionalSpacing.x) * (float)num - this.additionalSpacing.x;
			int horizontal = base.padding.horizontal;
			Vector2 cellSize = this.cellSize;
			Vector2 additionalSpacing = this.additionalSpacing;
			Vector2 additionalSpacing2 = this.additionalSpacing;
			float num4 = 0f;
			if (num3 > this.maxSize.x)
			{
				num4 = (num3 - this.maxSize.x) / (float)(num - 1);
				num3 = this.maxSize.x;
			}
			this.spacing = new Vector2(this.additionalSpacing.x - num4, this.spacing.y);
			base.SetLayoutInputForAxis(num3, num3, -1f, 0);
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x0019B86C File Offset: 0x00199A6C
		public override void CalculateLayoutInputVertical()
		{
			int num;
			if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedColumnCount)
			{
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)this.m_ConstraintCount - 0.001f);
			}
			else if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedRowCount)
			{
				num = this.m_ConstraintCount;
			}
			else
			{
				float width = base.rectTransform.rect.width;
				int num2 = Mathf.Max(1, Mathf.FloorToInt((width - (float)base.padding.horizontal + this.additionalSpacing.x + 0.001f) / (this.cellSize.x + this.additionalSpacing.x)));
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num2);
			}
			float num3 = (float)base.padding.vertical + (this.cellSize.y + this.additionalSpacing.y) * (float)num - this.additionalSpacing.y;
			float num4 = 0f;
			if (num3 > this.maxSize.x)
			{
				num4 = (num3 - this.maxSize.y) / (float)(num - 1);
				num3 = this.maxSize.y;
			}
			this.spacing = new Vector2(this.spacing.x, this.additionalSpacing.y - num4);
			base.SetLayoutInputForAxis(num3, num3, -1f, 1);
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x0019B9C3 File Offset: 0x00199BC3
		public override void SetLayoutHorizontal()
		{
			this.SetCellsAlongAxis(0);
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x0019B9CC File Offset: 0x00199BCC
		public override void SetLayoutVertical()
		{
			this.SetCellsAlongAxis(1);
		}

		// Token: 0x060032C9 RID: 13001 RVA: 0x0019B9D8 File Offset: 0x00199BD8
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
			if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedColumnCount)
			{
				num = this.m_ConstraintCount;
				if (base.rectChildren.Count > num)
				{
					num2 = base.rectChildren.Count / num + ((base.rectChildren.Count % num > 0) ? 1 : 0);
				}
			}
			else if (this.m_Constraint == GridLayoutStackingGroup.Constraint.FixedRowCount)
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
			int num3 = (int)(this.startCorner % GridLayoutStackingGroup.Corner.LowerLeft);
			int num4 = (int)(this.startCorner / GridLayoutStackingGroup.Corner.LowerLeft);
			int num5;
			int num6;
			int num7;
			if (this.startAxis == GridLayoutStackingGroup.Axis.Horizontal)
			{
				num5 = num;
				num6 = Mathf.Clamp(num, 1, base.rectChildren.Count);
				num7 = Mathf.Clamp(num2, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
			}
			else
			{
				num5 = num2;
				num7 = Mathf.Clamp(num2, 1, base.rectChildren.Count);
				num6 = Mathf.Clamp(num, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
			}
			Vector2 vector = new Vector2((float)num6 * this.cellSize.x + (float)(num6 - 1) * this.spacing.x, (float)num7 * this.cellSize.y + (float)(num7 - 1) * this.spacing.y);
			Vector2 vector2 = new Vector2(base.GetStartOffset(0, vector.x), base.GetStartOffset(1, vector.y));
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				int num8;
				int num9;
				if (this.startAxis == GridLayoutStackingGroup.Axis.Horizontal)
				{
					num8 = j % num5;
					num9 = j / num5;
				}
				else
				{
					num8 = j / num5;
					num9 = j % num5;
				}
				if (num3 == 1)
				{
					num8 = num6 - 1 - num8;
				}
				if (num4 == 1)
				{
					num9 = num7 - 1 - num9;
				}
				base.SetChildAlongAxis(base.rectChildren[j], 0, vector2.x + (this.cellSize[0] + this.spacing[0]) * (float)num8, this.cellSize[0]);
				base.SetChildAlongAxis(base.rectChildren[j], 1, vector2.y + (this.cellSize[1] + this.spacing[1]) * (float)num9, this.cellSize[1]);
			}
		}

		// Token: 0x0400224D RID: 8781
		[SerializeField]
		protected GridLayoutStackingGroup.Corner m_StartCorner;

		// Token: 0x0400224E RID: 8782
		[SerializeField]
		protected GridLayoutStackingGroup.Axis m_StartAxis = GridLayoutStackingGroup.Axis.Horizontal;

		// Token: 0x0400224F RID: 8783
		[SerializeField]
		protected Vector2 m_CellSize = new Vector2(100f, 100f);

		// Token: 0x04002250 RID: 8784
		[SerializeField]
		protected Vector2 m_MaxSize = new Vector2(1000f, 1000f);

		// Token: 0x04002251 RID: 8785
		[SerializeField]
		protected Vector2 m_Spacing = Vector2.zero;

		// Token: 0x04002252 RID: 8786
		[SerializeField]
		protected Vector2 m_AdditionalSpacing = Vector2.zero;

		// Token: 0x04002253 RID: 8787
		[SerializeField]
		protected GridLayoutStackingGroup.Constraint m_Constraint;

		// Token: 0x04002254 RID: 8788
		[SerializeField]
		protected int m_ConstraintCount = 2;

		// Token: 0x02000894 RID: 2196
		public enum Corner
		{
			// Token: 0x04004036 RID: 16438
			UpperLeft,
			// Token: 0x04004037 RID: 16439
			UpperRight,
			// Token: 0x04004038 RID: 16440
			LowerLeft,
			// Token: 0x04004039 RID: 16441
			LowerRight
		}

		// Token: 0x02000895 RID: 2197
		public enum Axis
		{
			// Token: 0x0400403B RID: 16443
			Horizontal = 1,
			// Token: 0x0400403C RID: 16444
			Vertical = 0
		}

		// Token: 0x02000896 RID: 2198
		public enum Constraint
		{
			// Token: 0x0400403E RID: 16446
			Flexible,
			// Token: 0x0400403F RID: 16447
			FixedColumnCount,
			// Token: 0x04004040 RID: 16448
			FixedRowCount
		}
	}
}
