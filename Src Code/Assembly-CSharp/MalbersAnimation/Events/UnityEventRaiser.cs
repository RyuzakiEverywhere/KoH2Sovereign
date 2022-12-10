using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Events
{
	// Token: 0x02000452 RID: 1106
	public class UnityEventRaiser : MonoBehaviour
	{
		// Token: 0x06003AC7 RID: 15047 RVA: 0x001C3FE7 File Offset: 0x001C21E7
		public void OnEnable()
		{
			this.OnEnableEvent.Invoke();
		}

		// Token: 0x04002A81 RID: 10881
		public UnityEvent OnEnableEvent;
	}
}
