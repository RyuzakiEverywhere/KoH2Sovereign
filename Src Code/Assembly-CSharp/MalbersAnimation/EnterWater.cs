using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003FB RID: 1019
	public class EnterWater : MonoBehaviour
	{
		// Token: 0x06003848 RID: 14408 RVA: 0x001BB6FB File Offset: 0x001B98FB
		private void OnTriggerEnter(Collider other)
		{
			other.transform.root.SendMessage("EnterWater", true, SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x06003849 RID: 14409 RVA: 0x001BB719 File Offset: 0x001B9919
		private void OnTriggerExit(Collider other)
		{
			other.transform.root.SendMessage("EnterWater", false, SendMessageOptions.DontRequireReceiver);
		}
	}
}
