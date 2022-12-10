using System;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000386 RID: 902
	[Node.CreateNodeMenuAttribute("BSG Nodes/Const/Float2")]
	[Serializable]
	public class Float2Node : Node
	{
		// Token: 0x0600349B RID: 13467 RVA: 0x001A4ECD File Offset: 0x001A30CD
		public override object GetValue(NodePort port)
		{
			return this.res;
		}

		// Token: 0x0400238E RID: 9102
		[Node.OutputAttribute(Node.ShowBackingValue.Always, Node.ConnectionType.Multiple, false)]
		public float2 res;
	}
}
