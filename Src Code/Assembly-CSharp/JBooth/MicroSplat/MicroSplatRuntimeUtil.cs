using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020003BA RID: 954
	public class MicroSplatRuntimeUtil
	{
		// Token: 0x060035BD RID: 13757 RVA: 0x001AF294 File Offset: 0x001AD494
		public static Vector2 UnityUVScaleToUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			uv.x = 1f / (uv.x / x);
			uv.y = 1f / (uv.y / z);
			return uv;
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x001AF2F0 File Offset: 0x001AD4F0
		public static Vector2 UVScaleToUnityUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float y = t.terrainData.size.y;
			if (uv.x < 0f)
			{
				uv.x = 0.001f;
			}
			if (uv.y < 0f)
			{
				uv.y = 0.001f;
			}
			uv.x = x / uv.x;
			uv.y = y / uv.y;
			return uv;
		}
	}
}
