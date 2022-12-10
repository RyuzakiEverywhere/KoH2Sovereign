using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A5 RID: 1189
	public static class LinearAlgebraUtility
	{
		// Token: 0x06003E62 RID: 15970 RVA: 0x001DCAF8 File Offset: 0x001DACF8
		public static Vector3 ProjectOnLine(Vector3 fromPoint, Vector3 toPoint, Vector3 project)
		{
			Vector3 vector = Vector3.Project(project - fromPoint, toPoint - fromPoint) + fromPoint;
			Vector3 rhs = toPoint - fromPoint;
			Vector3 lhs = vector - fromPoint;
			if (Vector3.Dot(lhs, rhs) <= 0f)
			{
				return fromPoint;
			}
			if (lhs.sqrMagnitude <= rhs.sqrMagnitude)
			{
				return vector;
			}
			return toPoint;
		}

		// Token: 0x06003E63 RID: 15971 RVA: 0x001DCB54 File Offset: 0x001DAD54
		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 vector = b - a;
			return Vector3.Dot(value - a, vector) / Vector3.Dot(vector, vector);
		}
	}
}
