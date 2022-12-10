using System;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038A RID: 906
	public class CreatePortal : MonoBehaviour
	{
		// Token: 0x060034AC RID: 13484 RVA: 0x001A54DF File Offset: 0x001A36DF
		public static void Create(Node srcNode, NodeGraph graph)
		{
			PortalNode portalNode = graph.AddNode<PortalNode>();
			portalNode.name = "Portal";
			portalNode.SrcNode = srcNode;
			portalNode.position = srcNode.position + new Vector2(200f, 100f);
		}
	}
}
