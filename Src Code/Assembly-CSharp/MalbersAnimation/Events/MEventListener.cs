using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Events
{
	// Token: 0x02000451 RID: 1105
	public class MEventListener : MonoBehaviour
	{
		// Token: 0x06003AC4 RID: 15044 RVA: 0x001C3F04 File Offset: 0x001C2104
		private void OnEnable()
		{
			foreach (MEventItemListener meventItemListener in this.Events)
			{
				if (meventItemListener.Event)
				{
					meventItemListener.Event.RegisterListener(meventItemListener);
				}
			}
		}

		// Token: 0x06003AC5 RID: 15045 RVA: 0x001C3F6C File Offset: 0x001C216C
		private void OnDisable()
		{
			foreach (MEventItemListener meventItemListener in this.Events)
			{
				if (meventItemListener.Event)
				{
					meventItemListener.Event.UnregisterListener(meventItemListener);
				}
			}
		}

		// Token: 0x04002A80 RID: 10880
		public List<MEventItemListener> Events = new List<MEventItemListener>();
	}
}
