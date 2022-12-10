using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D4 RID: 1236
	public class Capsule : SplinePrimitive
	{
		// Token: 0x060041B4 RID: 16820 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Bezier;
		}

		// Token: 0x060041B5 RID: 16821 RVA: 0x001F387C File Offset: 0x001F1A7C
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(7, SplinePoint.Type.SmoothMirrored);
			this.points[0].position = Vector3.right / 2f * this.radius + Vector3.forward * this.height * 0.5f;
			this.points[0].SetTangentPosition(this.points[0].position + Vector3.back * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[1].position = Vector3.forward / 2f * this.radius + Vector3.forward * this.height * 0.5f;
			this.points[1].SetTangentPosition(this.points[1].position + Vector3.right * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[2].position = Vector3.left / 2f * this.radius + Vector3.forward * this.height * 0.5f;
			this.points[2].SetTangentPosition(this.points[2].position + Vector3.forward * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[3].position = Vector3.left / 2f * this.radius + Vector3.back * this.height * 0.5f;
			this.points[3].SetTangentPosition(this.points[3].position + Vector3.forward * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[4].position = Vector3.back / 2f * this.radius + Vector3.back * this.height * 0.5f;
			this.points[4].SetTangentPosition(this.points[4].position + Vector3.left * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[5].position = Vector3.right / 2f * this.radius + Vector3.back * this.height * 0.5f;
			this.points[5].SetTangentPosition(this.points[5].position + Vector3.back * 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.radius);
			this.points[6] = this.points[0];
		}

		// Token: 0x04002DC3 RID: 11715
		public float radius = 1f;

		// Token: 0x04002DC4 RID: 11716
		public float height = 2f;
	}
}
