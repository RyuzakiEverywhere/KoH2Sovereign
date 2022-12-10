using System;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
	// Token: 0x020004D6 RID: 1238
	public class Line : SplinePrimitive
	{
		// Token: 0x060041BA RID: 16826 RVA: 0x001F3F2C File Offset: 0x001F212C
		public override Spline.Type GetSplineType()
		{
			return Spline.Type.Linear;
		}

		// Token: 0x060041BB RID: 16827 RVA: 0x001F3F30 File Offset: 0x001F2130
		protected override void Generate()
		{
			base.Generate();
			this.closed = false;
			base.CreatePoints(this.segments + 1, SplinePoint.Type.SmoothMirrored);
			Vector3 a = Vector3.zero;
			if (this.mirror)
			{
				a = -Vector3.forward * this.length * 0.5f;
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				this.points[i].position = a + Vector3.forward * this.length * ((float)i / (float)(this.points.Length - 1));
			}
		}

		// Token: 0x04002DC7 RID: 11719
		public bool mirror = true;

		// Token: 0x04002DC8 RID: 11720
		public float length = 1f;

		// Token: 0x04002DC9 RID: 11721
		public int segments = 1;
	}
}
