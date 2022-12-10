using System;
using UnityEngine;

namespace XNode.Examples.StateGraph
{
	// Token: 0x0200035F RID: 863
	public class StateNode : Node
	{
		// Token: 0x060033CA RID: 13258 RVA: 0x001A0C50 File Offset: 0x0019EE50
		public void MoveNext()
		{
			if ((this.graph as StateGraph).current != this)
			{
				Debug.LogWarning("Node isn't active");
				return;
			}
			NodePort outputPort = base.GetOutputPort("exit");
			if (!outputPort.IsConnected)
			{
				Debug.LogWarning("Node isn't connected");
				return;
			}
			(outputPort.Connection.node as StateNode).OnEnter();
		}

		// Token: 0x060033CB RID: 13259 RVA: 0x001A0CB4 File Offset: 0x0019EEB4
		public void OnEnter()
		{
			(this.graph as StateGraph).current = this;
		}

		// Token: 0x060033CC RID: 13260 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x040022D6 RID: 8918
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public StateNode.Empty enter;

		// Token: 0x040022D7 RID: 8919
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public StateNode.Empty exit;

		// Token: 0x020008AF RID: 2223
		[Serializable]
		public class Empty
		{
		}
	}
}
