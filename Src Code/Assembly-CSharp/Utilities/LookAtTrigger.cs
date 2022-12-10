using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000472 RID: 1138
	public class LookAtTrigger : MonoBehaviour
	{
		// Token: 0x06003B7C RID: 15228 RVA: 0x001C73EC File Offset: 0x001C55EC
		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger)
			{
				return;
			}
			LookAt componentInParent = other.GetComponentInParent<LookAt>();
			if (!componentInParent)
			{
				return;
			}
			componentInParent.Active = true;
			componentInParent.Target = base.transform;
		}

		// Token: 0x06003B7D RID: 15229 RVA: 0x001C7428 File Offset: 0x001C5628
		private void OnTriggerExit(Collider other)
		{
			if (other.isTrigger)
			{
				return;
			}
			LookAt componentInParent = other.GetComponentInParent<LookAt>();
			if (!componentInParent)
			{
				return;
			}
			componentInParent.Target = null;
		}
	}
}
