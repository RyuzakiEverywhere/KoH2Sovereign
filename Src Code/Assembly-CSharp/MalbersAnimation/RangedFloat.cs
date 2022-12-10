using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x02000409 RID: 1033
	[Serializable]
	public struct RangedFloat
	{
		// Token: 0x06003859 RID: 14425 RVA: 0x001BB908 File Offset: 0x001B9B08
		public RangedFloat(float minValue, float maxValue)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x0600385A RID: 14426 RVA: 0x001BB918 File Offset: 0x001B9B18
		public float RandomValue
		{
			get
			{
				return Random.Range(this.minValue, this.maxValue);
			}
		}

		// Token: 0x0600385B RID: 14427 RVA: 0x001BB92B File Offset: 0x001B9B2B
		public bool IsInRange(float value)
		{
			return value >= this.minValue && value <= this.maxValue;
		}

		// Token: 0x040028A4 RID: 10404
		public float minValue;

		// Token: 0x040028A5 RID: 10405
		public float maxValue;
	}
}
