using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200049B RID: 1179
	[Serializable]
	public class CM_SalsaWaypointTriggers
	{
		// Token: 0x04002C19 RID: 11289
		public CM_SalsaWaypointTriggers.Trigger trigger;

		// Token: 0x04002C1A RID: 11290
		public AudioClip audioClip;

		// Token: 0x04002C1B RID: 11291
		public float movementSpeed = 10f;

		// Token: 0x04002C1C RID: 11292
		public int waypointIndex;

		// Token: 0x0200097A RID: 2426
		public enum Trigger
		{
			// Token: 0x04004410 RID: 17424
			Start,
			// Token: 0x04004411 RID: 17425
			End
		}
	}
}
