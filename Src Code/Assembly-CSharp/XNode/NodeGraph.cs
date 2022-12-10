using System;
using System.Collections.Generic;
using UnityEngine;

namespace XNode
{
	// Token: 0x0200035C RID: 860
	[Serializable]
	public abstract class NodeGraph : ScriptableObject
	{
		// Token: 0x06003399 RID: 13209 RVA: 0x0019FFB3 File Offset: 0x0019E1B3
		public T AddNode<T>() where T : Node
		{
			return this.AddNode(typeof(T)) as T;
		}

		// Token: 0x0600339A RID: 13210 RVA: 0x0019FFD0 File Offset: 0x0019E1D0
		public virtual Node AddNode(Type type)
		{
			Node.graphHotfix = this;
			Node node = ScriptableObject.CreateInstance(type) as Node;
			node.graph = this;
			this.nodes.Add(node);
			return node;
		}

		// Token: 0x0600339B RID: 13211 RVA: 0x001A0004 File Offset: 0x0019E204
		public virtual Node CopyNode(Node original)
		{
			Node.graphHotfix = this;
			Node node = Object.Instantiate<Node>(original);
			node.graph = this;
			node.ClearConnections();
			this.nodes.Add(node);
			return node;
		}

		// Token: 0x0600339C RID: 13212 RVA: 0x001A0038 File Offset: 0x0019E238
		public virtual void RemoveNode(Node node)
		{
			node.ClearConnections();
			this.nodes.Remove(node);
			if (Application.isPlaying)
			{
				Object.Destroy(node);
			}
		}

		// Token: 0x0600339D RID: 13213 RVA: 0x001A005C File Offset: 0x0019E25C
		public virtual void Clear()
		{
			if (Application.isPlaying)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					Object.Destroy(this.nodes[i]);
				}
			}
			this.nodes.Clear();
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x001A00A4 File Offset: 0x0019E2A4
		public virtual NodeGraph Copy()
		{
			NodeGraph nodeGraph = Object.Instantiate<NodeGraph>(this);
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (!(this.nodes[i] == null))
				{
					Node.graphHotfix = nodeGraph;
					Node node = Object.Instantiate<Node>(this.nodes[i]);
					node.graph = nodeGraph;
					nodeGraph.nodes[i] = node;
				}
			}
			for (int j = 0; j < nodeGraph.nodes.Count; j++)
			{
				if (!(nodeGraph.nodes[j] == null))
				{
					foreach (NodePort nodePort in nodeGraph.nodes[j].Ports)
					{
						nodePort.Redirect(this.nodes, nodeGraph.nodes);
					}
				}
			}
			return nodeGraph;
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x001A0194 File Offset: 0x0019E394
		protected virtual void OnDestroy()
		{
			this.Clear();
		}

		// Token: 0x040022CC RID: 8908
		[SerializeField]
		public List<Node> nodes = new List<Node>();
	}
}
