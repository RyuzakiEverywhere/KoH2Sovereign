using System;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A5 RID: 933
	[Serializable]
	public class Float2D
	{
		// Token: 0x06003554 RID: 13652 RVA: 0x001ABDA5 File Offset: 0x001A9FA5
		public Float2D(IFloat2D val)
		{
			this.val = val;
		}

		// Token: 0x0400248D RID: 9357
		public IFloat2D val;
	}
}
