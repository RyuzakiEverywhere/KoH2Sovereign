using System;

namespace MalbersAnimations
{
	// Token: 0x020003FE RID: 1022
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public sealed class LineAttribute : Attribute
	{
		// Token: 0x06003853 RID: 14419 RVA: 0x001BB816 File Offset: 0x001B9A16
		public LineAttribute()
		{
			this.height = 8f;
		}

		// Token: 0x06003854 RID: 14420 RVA: 0x001BB829 File Offset: 0x001B9A29
		public LineAttribute(float height)
		{
			this.height = height;
		}

		// Token: 0x0400285C RID: 10332
		public readonly float height;
	}
}
