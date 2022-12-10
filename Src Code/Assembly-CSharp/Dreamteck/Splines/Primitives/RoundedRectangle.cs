using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D9 RID: 1241
	public class RoundedRectangle : SplinePrimitive
	{
		// Token: 0x060041C3 RID: 16835 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Bezier;
		}

		// Token: 0x060041C4 RID: 16836 RVA: 0x001F42A4 File Offset: 0x001F24A4
		protected override void Generate()
		{
			base.Generate();
			this.closed = true;
			base.CreatePoints(9, SplinePoint.Type.Broken);
			Vector2 vector = this.size - new Vector2(this.xRadius, this.yRadius) * 2f;
			this.points[0].SetPosition(Vector3.forward / 2f * vector.y + Vector3.left / 2f * this.size.x);
			this.points[1].SetPosition(Vector3.forward / 2f * this.size.y + Vector3.left / 2f * vector.x);
			this.points[2].SetPosition(Vector3.forward / 2f * this.size.y + Vector3.right / 2f * vector.x);
			this.points[3].SetPosition(Vector3.forward / 2f * vector.y + Vector3.right / 2f * this.size.x);
			this.points[4].SetPosition(Vector3.back / 2f * vector.y + Vector3.right / 2f * this.size.x);
			this.points[5].SetPosition(Vector3.back / 2f * this.size.y + Vector3.right / 2f * vector.x);
			this.points[6].SetPosition(Vector3.back / 2f * this.size.y + Vector3.left / 2f * vector.x);
			this.points[7].SetPosition(Vector3.back / 2f * vector.y + Vector3.left / 2f * this.size.x);
			float d = 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.xRadius * 2f;
			float d2 = 2f * (Mathf.Sqrt(2f) - 1f) / 3f * this.yRadius * 2f;
			this.points[0].SetTangent2Position(this.points[0].position + Vector3.forward * d2);
			this.points[1].SetTangentPosition(this.points[1].position + Vector3.left * d);
			this.points[2].SetTangent2Position(this.points[2].position + Vector3.right * d);
			this.points[3].SetTangentPosition(this.points[3].position + Vector3.forward * d2);
			this.points[4].SetTangent2Position(this.points[4].position + Vector3.back * d2);
			this.points[5].SetTangentPosition(this.points[5].position + Vector3.right * d);
			this.points[6].SetTangent2Position(this.points[6].position + Vector3.left * d);
			this.points[7].SetTangentPosition(this.points[7].position + Vector3.back * d2);
			this.points[8] = this.points[0];
		}

		// Token: 0x04002DCD RID: 11725
		public Vector2 size = Vector2.one;

		// Token: 0x04002DCE RID: 11726
		public float xRadius = 0.25f;

		// Token: 0x04002DCF RID: 11727
		public float yRadius = 0.25f;
	}
}
