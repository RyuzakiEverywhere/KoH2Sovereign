using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000369 RID: 873
	public class UGUIPort : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x06003403 RID: 13315 RVA: 0x001A1718 File Offset: 0x0019F918
		private void Start()
		{
			this.port = this.node.GetPort(this.fieldName);
			this.graph = base.GetComponentInParent<RuntimeMathGraph>();
			if (this.port.IsOutput && this.port.IsConnected)
			{
				for (int i = 0; i < this.port.ConnectionCount; i++)
				{
					this.AddConnection();
				}
			}
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x001A177E File Offset: 0x0019F97E
		private void Reset()
		{
			this.fieldName = base.name;
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x001A178C File Offset: 0x0019F98C
		private void OnDestroy()
		{
			for (int i = this.connections.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.connections[i].gameObject);
			}
			this.connections.Clear();
		}

		// Token: 0x06003406 RID: 13318 RVA: 0x001A17D4 File Offset: 0x0019F9D4
		public void UpdateConnectionTransforms()
		{
			if (this.port.IsInput)
			{
				return;
			}
			while (this.connections.Count < this.port.ConnectionCount)
			{
				this.AddConnection();
			}
			while (this.connections.Count > this.port.ConnectionCount)
			{
				Object.Destroy(this.connections[0].gameObject);
				this.connections.RemoveAt(0);
			}
			for (int i = 0; i < this.port.ConnectionCount; i++)
			{
				NodePort connection = this.port.GetConnection(i);
				UGUIMathBaseNode runtimeNode = this.graph.GetRuntimeNode(connection.node);
				if (!runtimeNode)
				{
					Debug.LogWarning(connection.node.name + " node not found", this);
				}
				Transform transform = runtimeNode.GetPort(connection.fieldName).transform;
				if (!transform)
				{
					Debug.LogWarning(connection.fieldName + " not found", this);
				}
				this.connections[i].SetPosition(base.transform.position, transform.position);
			}
		}

		// Token: 0x06003407 RID: 13319 RVA: 0x001A1904 File Offset: 0x0019FB04
		private void AddConnection()
		{
			Connection connection = Object.Instantiate<Connection>(this.graph.runtimeConnectionPrefab);
			connection.transform.SetParent(this.graph.scrollRect.content);
			this.connections.Add(connection);
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x001A194C File Offset: 0x0019FB4C
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this.port.IsOutput)
			{
				this.tempConnection = Object.Instantiate<Connection>(this.graph.runtimeConnectionPrefab);
				this.tempConnection.transform.SetParent(this.graph.scrollRect.content);
				this.tempConnection.SetPosition(base.transform.position, eventData.position);
				this.startPos = base.transform.position;
				this.startPort = this.port;
				return;
			}
			if (this.port.IsConnected)
			{
				NodePort connection = this.port.Connection;
				Debug.Log("has " + this.port.ConnectionCount + " connections");
				Debug.Log(this.port.GetConnection(0));
				UGUIPort uguiport = this.graph.GetRuntimeNode(connection.node).GetPort(connection.fieldName);
				Debug.Log("Disconnect");
				connection.Disconnect(this.port);
				this.tempConnection = Object.Instantiate<Connection>(this.graph.runtimeConnectionPrefab);
				this.tempConnection.transform.SetParent(this.graph.scrollRect.content);
				this.tempConnection.SetPosition(uguiport.transform.position, eventData.position);
				this.startPos = uguiport.transform.position;
				this.startPort = uguiport.port;
				this.graph.GetRuntimeNode(this.node).UpdateGUI();
			}
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x001A1AF4 File Offset: 0x0019FCF4
		public void OnDrag(PointerEventData eventData)
		{
			if (this.tempConnection == null)
			{
				return;
			}
			UGUIPort uguiport = this.FindPortInStack(eventData.hovered);
			this.tempHovered = uguiport;
			this.tempConnection.SetPosition(this.startPos, eventData.position);
		}

		// Token: 0x0600340A RID: 13322 RVA: 0x001A1B3C File Offset: 0x0019FD3C
		public void OnEndDrag(PointerEventData eventData)
		{
			if (this.tempConnection == null)
			{
				return;
			}
			if (this.tempHovered)
			{
				this.startPort.Connect(this.tempHovered.port);
				this.graph.GetRuntimeNode(this.tempHovered.node).UpdateGUI();
			}
			Object.Destroy(this.tempConnection.gameObject);
		}

		// Token: 0x0600340B RID: 13323 RVA: 0x001A1BA8 File Offset: 0x0019FDA8
		public UGUIPort FindPortInStack(List<GameObject> stack)
		{
			for (int i = 0; i < stack.Count; i++)
			{
				UGUIPort component = stack[i].GetComponent<UGUIPort>();
				if (component)
				{
					return component;
				}
			}
			return null;
		}

		// Token: 0x0600340C RID: 13324 RVA: 0x001A1BE0 File Offset: 0x0019FDE0
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.graph.tooltip.Show();
			object inputValue = this.node.GetInputValue<object>(this.port.fieldName, null);
			if (inputValue != null)
			{
				this.graph.tooltip.label.text = inputValue.ToString();
				return;
			}
			this.graph.tooltip.label.text = "n/a";
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x001A1C4E File Offset: 0x0019FE4E
		public void OnPointerExit(PointerEventData eventData)
		{
			this.graph.tooltip.Hide();
		}

		// Token: 0x040022F7 RID: 8951
		public string fieldName;

		// Token: 0x040022F8 RID: 8952
		[HideInInspector]
		public Node node;

		// Token: 0x040022F9 RID: 8953
		private NodePort port;

		// Token: 0x040022FA RID: 8954
		private Connection tempConnection;

		// Token: 0x040022FB RID: 8955
		private NodePort startPort;

		// Token: 0x040022FC RID: 8956
		private UGUIPort tempHovered;

		// Token: 0x040022FD RID: 8957
		private RuntimeMathGraph graph;

		// Token: 0x040022FE RID: 8958
		private Vector2 startPos;

		// Token: 0x040022FF RID: 8959
		private List<Connection> connections = new List<Connection>();
	}
}
