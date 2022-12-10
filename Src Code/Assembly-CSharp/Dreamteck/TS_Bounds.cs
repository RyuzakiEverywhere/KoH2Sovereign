using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A9 RID: 1193
	[Serializable]
	public class TS_Bounds
	{
		// Token: 0x06003E83 RID: 16003 RVA: 0x001DEC28 File Offset: 0x001DCE28
		public TS_Bounds()
		{
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x001DEC68 File Offset: 0x001DCE68
		public TS_Bounds(Bounds bounds)
		{
			this.center = bounds.center;
			this.extents = bounds.extents;
			this.max = bounds.max;
			this.min = bounds.min;
			this.size = bounds.size;
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x001DECF4 File Offset: 0x001DCEF4
		public TS_Bounds(Vector3 c, Vector3 s)
		{
			this.center = c;
			this.size = s;
			this.extents = s / 2f;
			this.max = this.center + this.extents;
			this.min = this.center - this.extents;
		}

		// Token: 0x06003E86 RID: 16006 RVA: 0x001DED8C File Offset: 0x001DCF8C
		public TS_Bounds(Vector3 min, Vector3 max, Vector3 center)
		{
			this.size = new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
			this.extents = this.size / 2f;
			this.min = min;
			this.max = max;
			this.center = center;
		}

		// Token: 0x06003E87 RID: 16007 RVA: 0x001DEE34 File Offset: 0x001DD034
		public void CreateFromMinMax(Vector3 min, Vector3 max)
		{
			this.size.x = max.x - min.x;
			this.size.y = max.y - min.y;
			this.size.z = max.z - min.z;
			this.extents = this.size / 2f;
			this.min = min;
			this.max = max;
			this.center = Vector3.Lerp(min, max, 0.5f);
		}

		// Token: 0x06003E88 RID: 16008 RVA: 0x001DEEC0 File Offset: 0x001DD0C0
		public bool Contains(Vector3 point)
		{
			return point.x >= this.min.x && point.x <= this.max.x && point.y >= this.min.y && point.y <= this.max.y && point.z >= this.min.z && point.z <= this.max.z;
		}

		// Token: 0x04002C4F RID: 11343
		public Vector3 center = Vector3.zero;

		// Token: 0x04002C50 RID: 11344
		public Vector3 extents = Vector3.zero;

		// Token: 0x04002C51 RID: 11345
		public Vector3 max = Vector3.zero;

		// Token: 0x04002C52 RID: 11346
		public Vector3 min = Vector3.zero;

		// Token: 0x04002C53 RID: 11347
		public Vector3 size = Vector3.zero;
	}
}
