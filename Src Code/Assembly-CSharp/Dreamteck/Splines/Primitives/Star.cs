using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004DC RID: 1244
	public class Star : SplinePrimitive
	{
		// Token: 0x060041D5 RID: 16853 RVA: 0x001F3F2C File Offset: 0x001F212C
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Linear;
		}

		// Token: 0x060041D6 RID: 16854 RVA: 0x001F4E50 File Offset: 0x001F3050
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(this.sides * 2 + 1, SplinePoint.Type.SmoothMirrored);
			float num = this.radius * this.depth;
			for (int i = 0; i < this.sides * 2; i++)
			{
				float num2 = (float)i / (float)(this.sides * 2);
				Vector3 position = Quaternion.AngleAxis(180f + 360f * num2, Vector3.up) * Vector3.forward * (((float)i % 2f == 0f) ? this.radius : num);
				this.points[i].SetPosition(position);
			}
			this.points[this.points.Length - 1] = this.points[0];
		}

		// Token: 0x04002DDB RID: 11739
		public float radius = 1f;

		// Token: 0x04002DDC RID: 11740
		public float depth = 0.5f;

		// Token: 0x04002DDD RID: 11741
		public int sides = 5;
	}
}
