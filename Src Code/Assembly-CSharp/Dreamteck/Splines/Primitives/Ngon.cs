using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D7 RID: 1239
	public class Ngon : SplinePrimitive
	{
		// Token: 0x060041BD RID: 16829 RVA: 0x001F3F2C File Offset: 0x001F212C
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Linear;
		}

		// Token: 0x060041BE RID: 16830 RVA: 0x001F3FF8 File Offset: 0x001F21F8
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(this.sides + 1, SplinePoint.Type.SmoothMirrored);
			for (int i = 0; i < this.sides; i++)
			{
				float num = (float)i / (float)this.sides;
				Vector3 position = Quaternion.AngleAxis(360f * num, Vector3.up) * Vector3.forward * this.radius;
				this.points[i].SetPosition(position);
			}
			this.points[this.points.Length - 1] = this.points[0];
		}

		// Token: 0x04002DCA RID: 11722
		public float radius = 1f;

		// Token: 0x04002DCB RID: 11723
		public int sides = 3;
	}
}
