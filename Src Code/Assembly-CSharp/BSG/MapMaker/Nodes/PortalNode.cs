using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003AA RID: 938
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Portal")]
	[Serializable]
	public class PortalNode : Node
	{
		// Token: 0x06003559 RID: 13657 RVA: 0x001ABDD4 File Offset: 0x001A9FD4
		public void RefreshConnections()
		{
			if (this.SrcNode == null)
			{
				return;
			}
			NodePort inputPort = base.GetInputPort("refInput");
			NodePort nodePort;
			if (string.IsNullOrEmpty(this.OutputName))
			{
				nodePort = null;
				using (IEnumerator<NodePort> enumerator = this.SrcNode.Outputs.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						goto IL_6C;
					}
					nodePort = enumerator.Current;
					goto IL_6C;
				}
			}
			nodePort = this.SrcNode.GetOutputPort(this.OutputName);
			IL_6C:
			if (inputPort.Connection != nodePort)
			{
				inputPort.Disconnect(inputPort.Connection);
			}
			if (!inputPort.IsConnected && nodePort != null)
			{
				inputPort.Connect(nodePort);
			}
		}

		// Token: 0x0600355A RID: 13658 RVA: 0x001ABE84 File Offset: 0x001AA084
		public override object GetValue(NodePort port)
		{
			NodePort inputPort = base.GetInputPort("refInput");
			if (!inputPort.IsConnected)
			{
				this.RefreshConnections();
			}
			if (!inputPort.IsConnected)
			{
				Debug.LogError("Portal sorce node not specified");
				return null;
			}
			return inputPort.GetInputValue();
		}

		// Token: 0x0600355B RID: 13659 RVA: 0x001ABEC8 File Offset: 0x001AA0C8
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			base.OnCreateConnection(from, to);
			if (to != base.GetInputPort("refInput"))
			{
				return;
			}
			if (to.IsConnected)
			{
				to.Disconnect(to.Connection);
			}
			this.SrcNode = from.node;
			this.OutputName = from.fieldName;
		}

		// Token: 0x0600355C RID: 13660 RVA: 0x001ABF18 File Offset: 0x001AA118
		public override void OnRemoveConnection(NodePort port)
		{
			base.OnRemoveConnection(port);
			if (port != base.GetInputPort("refInput"))
			{
				return;
			}
			this.SrcNode = null;
		}

		// Token: 0x04002490 RID: 9360
		public string OutputName;

		// Token: 0x04002491 RID: 9361
		public Node SrcNode;

		// Token: 0x04002492 RID: 9362
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public PortalNode.Any refInput;

		// Token: 0x04002493 RID: 9363
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public PortalNode.Any res;

		// Token: 0x020008EF RID: 2287
		[Serializable]
		public struct Any
		{
		}
	}
}
