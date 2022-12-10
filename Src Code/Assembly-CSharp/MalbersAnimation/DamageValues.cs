using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003BF RID: 959
	public class DamageValues
	{
		// Token: 0x060036A3 RID: 13987 RVA: 0x001B3233 File Offset: 0x001B1433
		public DamageValues(Vector3 dir, float amount = 0f)
		{
			this.Direction = dir;
			this.Amount = amount;
		}

		// Token: 0x04002668 RID: 9832
		public Vector3 Direction;

		// Token: 0x04002669 RID: 9833
		public float Amount;
	}
}
