using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004AC RID: 1196
	public static class TransformUtility
	{
		// Token: 0x06003EAD RID: 16045 RVA: 0x001E09B6 File Offset: 0x001DEBB6
		public static Vector3 GetPosition(Matrix4x4 m)
		{
			return m.GetColumn(3);
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x001E09C5 File Offset: 0x001DEBC5
		public static Quaternion GetRotation(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x001E09E8 File Offset: 0x001DEBE8
		public static Vector3 GetScale(Matrix4x4 m)
		{
			return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x001E0A2A File Offset: 0x001DEC2A
		public static void SetPosition(ref Matrix4x4 m, ref Vector3 p)
		{
			m.SetColumn(3, new Vector4(p.x, p.y, p.z, 1f));
		}

		// Token: 0x06003EB1 RID: 16049 RVA: 0x001E0A50 File Offset: 0x001DEC50
		public static void GetChildCount(Transform parent, ref int count)
		{
			foreach (object obj in parent)
			{
				Transform parent2 = (Transform)obj;
				count++;
				TransformUtility.GetChildCount(parent2, ref count);
			}
		}

		// Token: 0x06003EB2 RID: 16050 RVA: 0x001E0AA8 File Offset: 0x001DECA8
		public static bool IsParent(Transform child, Transform parent)
		{
			Transform transform = child;
			while (transform.parent != null)
			{
				transform = transform.parent;
				if (transform == parent)
				{
					return true;
				}
			}
			return false;
		}
	}
}
