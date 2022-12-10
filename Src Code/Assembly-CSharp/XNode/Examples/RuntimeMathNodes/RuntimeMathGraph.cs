using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode.Examples.MathNodes;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000363 RID: 867
	public class RuntimeMathGraph : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060033D8 RID: 13272 RVA: 0x001A0F26 File Offset: 0x0019F126
		// (set) Token: 0x060033D9 RID: 13273 RVA: 0x001A0F2E File Offset: 0x0019F12E
		public ScrollRect scrollRect { get; private set; }

		// Token: 0x060033DA RID: 13274 RVA: 0x001A0F38 File Offset: 0x0019F138
		private void Awake()
		{
			this.graph = (this.graph.Copy() as MathGraph);
			this.scrollRect = base.GetComponentInChildren<ScrollRect>();
			UGUIContextMenu uguicontextMenu = this.graphContextMenu;
			uguicontextMenu.onClickSpawn = (Action<Type, Vector2>)Delegate.Remove(uguicontextMenu.onClickSpawn, new Action<Type, Vector2>(this.SpawnNode));
			UGUIContextMenu uguicontextMenu2 = this.graphContextMenu;
			uguicontextMenu2.onClickSpawn = (Action<Type, Vector2>)Delegate.Combine(uguicontextMenu2.onClickSpawn, new Action<Type, Vector2>(this.SpawnNode));
		}

		// Token: 0x060033DB RID: 13275 RVA: 0x001A0FB5 File Offset: 0x0019F1B5
		private void Start()
		{
			this.SpawnGraph();
		}

		// Token: 0x060033DC RID: 13276 RVA: 0x001A0FBD File Offset: 0x0019F1BD
		public void Refresh()
		{
			this.Clear();
			this.SpawnGraph();
		}

		// Token: 0x060033DD RID: 13277 RVA: 0x001A0FCC File Offset: 0x0019F1CC
		public void Clear()
		{
			for (int i = this.nodes.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.nodes[i].gameObject);
			}
			this.nodes.Clear();
		}

		// Token: 0x060033DE RID: 13278 RVA: 0x001A1014 File Offset: 0x0019F214
		public void SpawnGraph()
		{
			if (this.nodes != null)
			{
				this.nodes.Clear();
			}
			else
			{
				this.nodes = new List<UGUIMathBaseNode>();
			}
			for (int i = 0; i < this.graph.nodes.Count; i++)
			{
				Node node = this.graph.nodes[i];
				UGUIMathBaseNode uguimathBaseNode = null;
				if (node is MathNode)
				{
					uguimathBaseNode = Object.Instantiate<UGUIMathNode>(this.runtimeMathNodePrefab);
				}
				else if (node is Vector)
				{
					uguimathBaseNode = Object.Instantiate<UGUIVector>(this.runtimeVectorPrefab);
				}
				else if (node is DisplayValue)
				{
					uguimathBaseNode = Object.Instantiate<UGUIDisplayValue>(this.runtimeDisplayValuePrefab);
				}
				uguimathBaseNode.transform.SetParent(this.scrollRect.content);
				uguimathBaseNode.node = node;
				uguimathBaseNode.graph = this;
				this.nodes.Add(uguimathBaseNode);
			}
		}

		// Token: 0x060033DF RID: 13279 RVA: 0x001A10E8 File Offset: 0x0019F2E8
		public UGUIMathBaseNode GetRuntimeNode(Node node)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (this.nodes[i].node == node)
				{
					return this.nodes[i];
				}
			}
			return null;
		}

		// Token: 0x060033E0 RID: 13280 RVA: 0x001A1132 File Offset: 0x0019F332
		public void SpawnNode(Type type, Vector2 position)
		{
			Node node = this.graph.AddNode(type);
			node.name = type.Name;
			node.position = position;
			this.Refresh();
		}

		// Token: 0x060033E1 RID: 13281 RVA: 0x001A1158 File Offset: 0x0019F358
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Right)
			{
				return;
			}
			this.graphContextMenu.OpenAt(eventData.position);
		}

		// Token: 0x040022DC RID: 8924
		[Header("Graph")]
		public MathGraph graph;

		// Token: 0x040022DD RID: 8925
		[Header("Prefabs")]
		public UGUIMathNode runtimeMathNodePrefab;

		// Token: 0x040022DE RID: 8926
		public UGUIVector runtimeVectorPrefab;

		// Token: 0x040022DF RID: 8927
		public UGUIDisplayValue runtimeDisplayValuePrefab;

		// Token: 0x040022E0 RID: 8928
		public Connection runtimeConnectionPrefab;

		// Token: 0x040022E1 RID: 8929
		[Header("References")]
		public UGUIContextMenu graphContextMenu;

		// Token: 0x040022E2 RID: 8930
		public UGUIContextMenu nodeContextMenu;

		// Token: 0x040022E3 RID: 8931
		public UGUITooltip tooltip;

		// Token: 0x040022E5 RID: 8933
		private List<UGUIMathBaseNode> nodes;
	}
}
