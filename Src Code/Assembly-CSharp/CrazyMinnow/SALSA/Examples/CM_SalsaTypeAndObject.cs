using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200048D RID: 1165
	public class CM_SalsaTypeAndObject
	{
		// Token: 0x04002BC7 RID: 11207
		public GameObject salsaGameObject;

		// Token: 0x04002BC8 RID: 11208
		public CM_SalsaTypeAndObject.SalsaTypeOf salsaType;

		// Token: 0x02000977 RID: 2423
		public enum SalsaTypeOf
		{
			// Token: 0x04004403 RID: 17411
			Salsa2D,
			// Token: 0x04004404 RID: 17412
			Salsa3D
		}
	}
}
