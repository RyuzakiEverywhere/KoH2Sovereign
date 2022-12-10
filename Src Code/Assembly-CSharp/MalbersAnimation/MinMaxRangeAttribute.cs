using System;

namespace MalbersAnimations
{
	// Token: 0x020003FD RID: 1021
	public class MinMaxRangeAttribute : Attribute
	{
		// Token: 0x0600384E RID: 14414 RVA: 0x001BB7DE File Offset: 0x001B99DE
		public MinMaxRangeAttribute(float min, float max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x0600384F RID: 14415 RVA: 0x001BB7F4 File Offset: 0x001B99F4
		// (set) Token: 0x06003850 RID: 14416 RVA: 0x001BB7FC File Offset: 0x001B99FC
		public float Min { get; private set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06003851 RID: 14417 RVA: 0x001BB805 File Offset: 0x001B9A05
		// (set) Token: 0x06003852 RID: 14418 RVA: 0x001BB80D File Offset: 0x001B9A0D
		public float Max { get; private set; }
	}
}
