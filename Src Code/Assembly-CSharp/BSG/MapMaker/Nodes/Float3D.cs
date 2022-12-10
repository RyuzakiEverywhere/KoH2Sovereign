using System;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A7 RID: 935
	[Serializable]
	public class Float3D
	{
		// Token: 0x06003556 RID: 13654 RVA: 0x001ABDB4 File Offset: 0x001A9FB4
		public Float3D(IFloat3D val)
		{
			this.val = val;
		}

		// Token: 0x0400248E RID: 9358
		public IFloat3D val;
	}
}
