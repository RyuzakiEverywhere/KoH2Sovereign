using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200048F RID: 1167
	[Serializable]
	public class CM_NPCDialog
	{
		// Token: 0x04002BCE RID: 11214
		public GameObject npc;

		// Token: 0x04002BCF RID: 11215
		public string npcText;

		// Token: 0x04002BD0 RID: 11216
		public AudioClip npcAudio;

		// Token: 0x04002BD1 RID: 11217
		public CM_PlayerResponse[] playerResponse;

		// Token: 0x04002BD2 RID: 11218
		public bool endDialog;
	}
}
