using System;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B1 RID: 945
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/SubRect")]
	[Serializable]
	public class SubRectNode : Node
	{
		// Token: 0x0600358E RID: 13710 RVA: 0x001AD811 File Offset: 0x001ABA11
		public override object GetValue(NodePort port)
		{
			return this.subRect;
		}

		// Token: 0x040024E7 RID: 9447
		public SubRect subRect;

		// Token: 0x040024E8 RID: 9448
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public SubRect res;
	}
}
