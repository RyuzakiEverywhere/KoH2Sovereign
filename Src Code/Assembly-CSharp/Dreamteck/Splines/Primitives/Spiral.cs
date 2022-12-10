using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004DA RID: 1242
	public class Spiral : SplinePrimitive
	{
		// Token: 0x060041C6 RID: 16838 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Bezier;
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x001F478C File Offset: 0x001F298C
		protected override void Generate()
		{
			base.Generate();
			this.closed = false;
			base.CreatePoints(this.iterations * 4 + 1, SplinePoint.Type.SmoothMirrored);
			float num = Mathf.Abs(this.endRadius - this.startRadius) / Mathf.Max(Mathf.Abs(this.endRadius), Mathf.Abs(this.startRadius));
			float num2 = 1f;
			if (this.endRadius > this.startRadius)
			{
				num2 = -1f;
			}
			float num3 = 0f;
			float num4 = 0f;
			float num5 = this.clockwise ? 1f : -1f;
			for (int i = 0; i <= this.iterations * 4; i++)
			{
				float num6 = this.curve.Evaluate((float)i / (float)(this.iterations * 4));
				float d = Mathf.Lerp(this.startRadius, this.endRadius, num6);
				Quaternion quaternion = Quaternion.AngleAxis(num3, Vector3.up);
				this.points[i].position = quaternion * Vector3.forward / 2f * d + Vector3.up * num4;
				Quaternion lhs = Quaternion.identity;
				if (num2 > 0f)
				{
					lhs = Quaternion.AngleAxis(Mathf.Lerp(0f, 14.4f * num5, num * num6), Vector3.up);
				}
				else
				{
					lhs = Quaternion.AngleAxis(Mathf.Lerp(0f, -14.4f * num5, (1f - num6) * num), Vector3.up);
				}
				if (this.clockwise)
				{
					this.points[i].tangent = this.points[i].position - (lhs * quaternion * Vector3.right * d + Vector3.up * this.stretch / 4f) * 2f * (Mathf.Sqrt(2f) - 1f) / 3f;
				}
				else
				{
					this.points[i].tangent = this.points[i].position + (lhs * quaternion * Vector3.right * d - Vector3.up * this.stretch / 4f) * 2f * (Mathf.Sqrt(2f) - 1f) / 3f;
				}
				this.points[i].tangent2 = this.points[i].position - (this.points[i].tangent - this.points[i].position);
				num4 += this.stretch / 4f;
				num3 += 90f * num5;
			}
		}

		// Token: 0x04002DD0 RID: 11728
		public float startRadius = 1f;

		// Token: 0x04002DD1 RID: 11729
		public float endRadius = 1f;

		// Token: 0x04002DD2 RID: 11730
		public float stretch = 1f;

		// Token: 0x04002DD3 RID: 11731
		public int iterations = 3;

		// Token: 0x04002DD4 RID: 11732
		public bool clockwise = true;

		// Token: 0x04002DD5 RID: 11733
		public AnimationCurve curve = new AnimationCurve();
	}
}
