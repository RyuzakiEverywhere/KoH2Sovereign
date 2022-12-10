using System;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A9 RID: 937
	[Serializable]
	public class TreesList
	{
		// Token: 0x06003558 RID: 13656 RVA: 0x001ABDC3 File Offset: 0x001A9FC3
		public TreesList(ITreesList val)
		{
			this.val = val;
		}

		// Token: 0x0400248F RID: 9359
		public ITreesList val;
	}
}
