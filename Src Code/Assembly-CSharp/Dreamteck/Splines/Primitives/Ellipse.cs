using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D5 RID: 1237
	public class Ellipse : SplinePrimitive
	{
		// Token: 0x060041B7 RID: 16823 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Bezier;
		}

		// Token: 0x060041B8 RID: 16824 RVA: 0x001F3CD0 File Offset: 0x001F1ED0
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(5, SplinePoint.Type.SmoothMirrored);
			this.points[0].position = Vector3.forward * this.yRadius;
			this.points[0].SetTangentPosition(this.points[0].position + Vector3.right * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * this.xRadius);
			this.points[1].position = Vector3.left * this.xRadius;
			this.points[1].SetTangentPosition(this.points[1].position + Vector3.forward * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * this.yRadius);
			this.points[2].position = Vector3.back * this.yRadius;
			this.points[2].SetTangentPosition(this.points[2].position + Vector3.left * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * this.xRadius);
			this.points[3].position = Vector3.right * this.xRadius;
			this.points[3].SetTangentPosition(this.points[3].position + Vector3.back * 2f * (Mathf.Sqrt(2f) - 1f) / 1.5f * this.yRadius);
			this.points[4] = this.points[0];
		}

		// Token: 0x04002DC5 RID: 11717
		public float xRadius = 1f;

		// Token: 0x04002DC6 RID: 11718
		public float yRadius = 1f;
	}
}
