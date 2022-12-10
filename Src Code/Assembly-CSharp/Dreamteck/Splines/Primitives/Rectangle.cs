using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D8 RID: 1240
	public class Rectangle : SplinePrimitive
	{
		// Token: 0x060041C0 RID: 16832 RVA: 0x001F3F2C File Offset: 0x001F212C
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Linear;
		}

		// Token: 0x060041C1 RID: 16833 RVA: 0x001F40B4 File Offset: 0x001F22B4
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(5, SplinePoint.Type.SmoothMirrored);
			this.points[0].position = (this.points[0].tangent = Vector3.forward / 2f * this.size.y + Vector3.left / 2f * this.size.x);
			this.points[1].position = (this.points[1].tangent = Vector3.forward / 2f * this.size.y + Vector3.right / 2f * this.size.x);
			this.points[2].position = (this.points[2].tangent = Vector3.back / 2f * this.size.y + Vector3.right / 2f * this.size.x);
			this.points[3].position = (this.points[3].tangent = Vector3.back / 2f * this.size.y + Vector3.left / 2f * this.size.x);
			this.points[4] = this.points[0];
		}

		// Token: 0x04002DCC RID: 11724
		public Vector2 size = Vector2.one;
	}
}
