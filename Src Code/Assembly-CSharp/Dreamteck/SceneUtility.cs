using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A8 RID: 1192
	public static class SceneUtility
	{
		// Token: 0x06003E82 RID: 16002 RVA: 0x001DEBCC File Offset: 0x001DCDCC
		public static void GetChildrenRecursively(Transform current, ref List<Transform> transformList)
		{
			transformList.Add(current);
			foreach (object obj in current)
			{
				SceneUtility.GetChildrenRecursively((Transform)obj, ref transformList);
			}
		}
	}
}
