using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200049A RID: 1178
	public class CM_SalsaBroadcastEventTester : MonoBehaviour
	{
		// Token: 0x06003E03 RID: 15875 RVA: 0x001DB330 File Offset: 0x001D9530
		private void Salsa_OnTalkStatusChanged(SalsaStatus status)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Salsa_OnTalkStatusChanged: instance(",
				status.instance.GetType(),
				"), talkerName(",
				status.talkerName,
				"),",
				status.isTalking ? "started" : "finished",
				" saying ",
				status.clipName
			}));
		}
	}
}
