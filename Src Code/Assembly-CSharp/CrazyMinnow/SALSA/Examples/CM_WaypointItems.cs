using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200049D RID: 1181
	[Serializable]
	public class CM_WaypointItems
	{
		// Token: 0x04002C26 RID: 11302
		public GameObject waypoint;

		// Token: 0x04002C27 RID: 11303
		public CM_WaypointItems.Direction direction;

		// Token: 0x04002C28 RID: 11304
		public int delay;

		// Token: 0x0200097B RID: 2427
		public enum Direction
		{
			// Token: 0x04004413 RID: 17427
			Left,
			// Token: 0x04004414 RID: 17428
			Right
		}
	}
}
