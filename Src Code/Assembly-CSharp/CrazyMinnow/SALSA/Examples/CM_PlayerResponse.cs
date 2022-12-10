using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200048E RID: 1166
	[Serializable]
	public class CM_PlayerResponse
	{
		// Token: 0x04002BC9 RID: 11209
		public GameObject player;

		// Token: 0x04002BCA RID: 11210
		public string playerText;

		// Token: 0x04002BCB RID: 11211
		public AudioClip playerAudio;

		// Token: 0x04002BCC RID: 11212
		public int npcDialogIndex;

		// Token: 0x04002BCD RID: 11213
		public bool endDialog;
	}
}
