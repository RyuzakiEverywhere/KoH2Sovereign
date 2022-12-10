using System;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x0200047D RID: 1149
	public static class AssetsExtensions
	{
		// Token: 0x06003BF1 RID: 15345 RVA: 0x001C959B File Offset: 0x001C779B
		public static string GUID(this Object obj)
		{
			Debug.LogError("GUID does not work in build");
			return "";
		}

		// Token: 0x06003BF2 RID: 15346 RVA: 0x001C95AC File Offset: 0x001C77AC
		public static T GUIDtoObj<T>(this string guid) where T : Object
		{
			Debug.LogError("GUIDtObj does not work in build");
			return default(T);
		}

		// Token: 0x06003BF3 RID: 15347 RVA: 0x001C95CC File Offset: 0x001C77CC
		public static string[] GetUserData(this Object obj, string param)
		{
			Debug.LogError("GetUserData does not work in build");
			return null;
		}

		// Token: 0x06003BF4 RID: 15348 RVA: 0x001C95CC File Offset: 0x001C77CC
		public static string[] GetUserData(string guid, string param)
		{
			Debug.LogError("GetUserData does not work in build");
			return null;
		}

		// Token: 0x06003BF5 RID: 15349 RVA: 0x001C95D9 File Offset: 0x001C77D9
		public static void SetUserData(this Object obj, string param, string[] data, bool reload = false)
		{
			Debug.LogError("SetUserData does not work in build");
		}

		// Token: 0x06003BF6 RID: 15350 RVA: 0x001C95D9 File Offset: 0x001C77D9
		public static void SetUserData(string guid, string param, string[] data, bool reload = false)
		{
			Debug.LogError("SetUserData does not work in build");
		}

		// Token: 0x06003BF7 RID: 15351 RVA: 0x001C95E5 File Offset: 0x001C77E5
		public static void Reimport(this Object obj)
		{
			Debug.LogError("Reimport does not work in build");
		}
	}
}
