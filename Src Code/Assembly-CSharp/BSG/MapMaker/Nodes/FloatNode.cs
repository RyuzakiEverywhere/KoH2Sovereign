using System;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000388 RID: 904
	[Node.CreateNodeMenuAttribute("BSG Nodes/Const/Float")]
	[Serializable]
	public class FloatNode : Node
	{
		// Token: 0x060034A0 RID: 13472 RVA: 0x001A4F80 File Offset: 0x001A3180
		public override object GetValue(NodePort port)
		{
			return this.value;
		}

		// Token: 0x04002392 RID: 9106
		public float value;

		// Token: 0x04002393 RID: 9107
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float res;
	}
}
