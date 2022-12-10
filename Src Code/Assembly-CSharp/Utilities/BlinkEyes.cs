using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200046B RID: 1131
	public class BlinkEyes : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x06003B5F RID: 15199 RVA: 0x001C6E4C File Offset: 0x001C504C
		public virtual void Eyes(int ID)
		{
			if (this.animator)
			{
				this.animator.SetInteger(this.parameter, ID);
			}
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x04002B27 RID: 11047
		public Animator animator;

		// Token: 0x04002B28 RID: 11048
		public string parameter;
	}
}
