using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000462 RID: 1122
	public class EffectModifier : ScriptableObject
	{
		// Token: 0x06003B23 RID: 15139 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void AwakeEffect(Effect effect)
		{
		}

		// Token: 0x06003B24 RID: 15140 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void StartEffect(Effect effect)
		{
		}

		// Token: 0x06003B25 RID: 15141 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void StopEffect(Effect effect)
		{
		}

		// Token: 0x04002B0A RID: 11018
		[TextArea]
		public string Description = string.Empty;
	}
}
