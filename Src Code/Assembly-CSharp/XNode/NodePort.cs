using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XNode
{
	// Token: 0x0200035D RID: 861
	[Serializable]
	public class NodePort
	{
		// Token: 0x170002BA RID: 698
		// (get) Token: 0x060033A1 RID: 13217 RVA: 0x001A01AF File Offset: 0x0019E3AF
		public int ConnectionCount
		{
			get
			{
				return this.connections.Count;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x060033A2 RID: 13218 RVA: 0x001A01BC File Offset: 0x0019E3BC
		public NodePort Connection
		{
			get
			{
				for (int i = 0; i < this.connections.Count; i++)
				{
					if (this.connections[i] != null)
					{
						return this.connections[i].Port;
					}
				}
				return null;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x060033A3 RID: 13219 RVA: 0x001A0200 File Offset: 0x0019E400
		public NodePort.IO direction
		{
			get
			{
				return this._direction;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x060033A4 RID: 13220 RVA: 0x001A0208 File Offset: 0x0019E408
		public Node.ConnectionType connectionType
		{
			get
			{
				return this._connectionType;
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060033A5 RID: 13221 RVA: 0x001A0210 File Offset: 0x0019E410
		public Node.TypeConstraint typeConstraint
		{
			get
			{
				return this._typeConstraint;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060033A6 RID: 13222 RVA: 0x001A0218 File Offset: 0x0019E418
		public bool IsConnected
		{
			get
			{
				return this.connections.Count != 0;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x060033A7 RID: 13223 RVA: 0x001A0228 File Offset: 0x0019E428
		public bool IsInput
		{
			get
			{
				return this.direction == NodePort.IO.Input;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060033A8 RID: 13224 RVA: 0x001A0233 File Offset: 0x0019E433
		public bool IsOutput
		{
			get
			{
				return this.direction == NodePort.IO.Output;
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x060033A9 RID: 13225 RVA: 0x001A023E File Offset: 0x0019E43E
		public string fieldName
		{
			get
			{
				return this._fieldName;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060033AA RID: 13226 RVA: 0x001A0246 File Offset: 0x0019E446
		public Node node
		{
			get
			{
				return this._node;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060033AB RID: 13227 RVA: 0x001A024E File Offset: 0x0019E44E
		public bool IsDynamic
		{
			get
			{
				return this._dynamic;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060033AC RID: 13228 RVA: 0x001A0256 File Offset: 0x0019E456
		public bool IsStatic
		{
			get
			{
				return !this._dynamic;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060033AD RID: 13229 RVA: 0x001A0261 File Offset: 0x0019E461
		// (set) Token: 0x060033AE RID: 13230 RVA: 0x001A0296 File Offset: 0x0019E496
		public Type ValueType
		{
			get
			{
				if (this.valueType == null && !string.IsNullOrEmpty(this._typeQualifiedName))
				{
					this.valueType = Type.GetType(this._typeQualifiedName, false);
				}
				return this.valueType;
			}
			set
			{
				this.valueType = value;
				if (value != null)
				{
					this._typeQualifiedName = value.AssemblyQualifiedName;
				}
			}
		}

		// Token: 0x060033AF RID: 13231 RVA: 0x001A02B4 File Offset: 0x0019E4B4
		public NodePort(FieldInfo fieldInfo)
		{
			this._fieldName = fieldInfo.Name;
			this.ValueType = fieldInfo.FieldType;
			this._dynamic = false;
			object[] customAttributes = fieldInfo.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is Node.InputAttribute)
				{
					this._direction = NodePort.IO.Input;
					this._connectionType = (customAttributes[i] as Node.InputAttribute).connectionType;
					this._typeConstraint = (customAttributes[i] as Node.InputAttribute).typeConstraint;
				}
				else if (customAttributes[i] is Node.OutputAttribute)
				{
					this._direction = NodePort.IO.Output;
					this._connectionType = (customAttributes[i] as Node.OutputAttribute).connectionType;
				}
			}
		}

		// Token: 0x060033B0 RID: 13232 RVA: 0x001A0364 File Offset: 0x0019E564
		public NodePort(NodePort nodePort, Node node)
		{
			this._fieldName = nodePort._fieldName;
			this.ValueType = nodePort.valueType;
			this._direction = nodePort.direction;
			this._dynamic = nodePort._dynamic;
			this._connectionType = nodePort._connectionType;
			this._typeConstraint = nodePort._typeConstraint;
			this._node = node;
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x001A03D4 File Offset: 0x0019E5D4
		public NodePort(string fieldName, Type type, NodePort.IO direction, Node.ConnectionType connectionType, Node.TypeConstraint typeConstraint, Node node)
		{
			this._fieldName = fieldName;
			this.ValueType = type;
			this._direction = direction;
			this._node = node;
			this._dynamic = true;
			this._connectionType = connectionType;
			this._typeConstraint = typeConstraint;
		}

		// Token: 0x060033B2 RID: 13234 RVA: 0x001A0428 File Offset: 0x0019E628
		public void VerifyConnections()
		{
			for (int i = this.connections.Count - 1; i >= 0; i--)
			{
				if (!(this.connections[i].node != null) || string.IsNullOrEmpty(this.connections[i].fieldName) || this.connections[i].node.GetPort(this.connections[i].fieldName) == null)
				{
					this.connections.RemoveAt(i);
				}
			}
		}

		// Token: 0x060033B3 RID: 13235 RVA: 0x001A04B3 File Offset: 0x0019E6B3
		public object GetOutputValue()
		{
			if (this.direction == NodePort.IO.Input)
			{
				return null;
			}
			return this.node.GetValue(this);
		}

		// Token: 0x060033B4 RID: 13236 RVA: 0x001A04CC File Offset: 0x0019E6CC
		public object GetInputValue()
		{
			NodePort connection = this.Connection;
			if (connection == null)
			{
				return null;
			}
			return connection.GetOutputValue();
		}

		// Token: 0x060033B5 RID: 13237 RVA: 0x001A04EC File Offset: 0x0019E6EC
		public object[] GetInputValues()
		{
			object[] array = new object[this.ConnectionCount];
			for (int i = 0; i < this.ConnectionCount; i++)
			{
				NodePort port = this.connections[i].Port;
				if (port == null)
				{
					this.connections.RemoveAt(i);
					i--;
				}
				else
				{
					array[i] = port.GetOutputValue();
				}
			}
			return array;
		}

		// Token: 0x060033B6 RID: 13238 RVA: 0x001A0548 File Offset: 0x0019E748
		public T GetInputValue<T>()
		{
			object inputValue = this.GetInputValue();
			if (!(inputValue is T))
			{
				return default(T);
			}
			return (T)((object)inputValue);
		}

		// Token: 0x060033B7 RID: 13239 RVA: 0x001A0574 File Offset: 0x0019E774
		public T[] GetInputValues<T>()
		{
			object[] inputValues = this.GetInputValues();
			T[] array = new T[inputValues.Length];
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is T)
				{
					array[i] = (T)((object)inputValues[i]);
				}
			}
			return array;
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x001A05BC File Offset: 0x0019E7BC
		public bool TryGetInputValue<T>(out T value)
		{
			object inputValue = this.GetInputValue();
			if (inputValue is T)
			{
				value = (T)((object)inputValue);
				return true;
			}
			value = default(T);
			return false;
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x001A05F0 File Offset: 0x0019E7F0
		public float GetInputSum(float fallback)
		{
			object[] inputValues = this.GetInputValues();
			if (inputValues.Length == 0)
			{
				return fallback;
			}
			float num = 0f;
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is float)
				{
					num += (float)inputValues[i];
				}
			}
			return num;
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x001A0634 File Offset: 0x0019E834
		public int GetInputSum(int fallback)
		{
			object[] inputValues = this.GetInputValues();
			if (inputValues.Length == 0)
			{
				return fallback;
			}
			int num = 0;
			for (int i = 0; i < inputValues.Length; i++)
			{
				if (inputValues[i] is int)
				{
					num += (int)inputValues[i];
				}
			}
			return num;
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x001A0674 File Offset: 0x0019E874
		public void Connect(NodePort port)
		{
			if (this.connections == null)
			{
				this.connections = new List<NodePort.PortConnection>();
			}
			if (port == null)
			{
				Debug.LogWarning("Cannot connect to null port");
				return;
			}
			if (port == this)
			{
				Debug.LogWarning("Cannot connect port to self.");
				return;
			}
			if (this.IsConnectedTo(port))
			{
				Debug.LogWarning("Port already connected. ");
				return;
			}
			if (this.direction == port.direction)
			{
				Debug.LogWarning("Cannot connect two " + ((this.direction == NodePort.IO.Input) ? "input" : "output") + " connections");
				return;
			}
			if (port.connectionType == Node.ConnectionType.Override && port.ConnectionCount != 0)
			{
				port.ClearConnections();
			}
			if (this.connectionType == Node.ConnectionType.Override && this.ConnectionCount != 0)
			{
				this.ClearConnections();
			}
			this.connections.Add(new NodePort.PortConnection(port));
			if (port.connections == null)
			{
				port.connections = new List<NodePort.PortConnection>();
			}
			if (!port.IsConnectedTo(this))
			{
				port.connections.Add(new NodePort.PortConnection(this));
			}
			this.node.OnCreateConnection(this, port);
			port.node.OnCreateConnection(this, port);
		}

		// Token: 0x060033BC RID: 13244 RVA: 0x001A0784 File Offset: 0x0019E984
		public List<NodePort> GetConnections()
		{
			List<NodePort> list = new List<NodePort>();
			for (int i = 0; i < this.connections.Count; i++)
			{
				NodePort connection = this.GetConnection(i);
				if (connection != null)
				{
					list.Add(connection);
				}
			}
			return list;
		}

		// Token: 0x060033BD RID: 13245 RVA: 0x001A07C0 File Offset: 0x0019E9C0
		public NodePort GetConnection(int i)
		{
			if (this.connections[i].node == null || string.IsNullOrEmpty(this.connections[i].fieldName))
			{
				this.connections.RemoveAt(i);
				return null;
			}
			NodePort port = this.connections[i].node.GetPort(this.connections[i].fieldName);
			if (port == null)
			{
				this.connections.RemoveAt(i);
				return null;
			}
			return port;
		}

		// Token: 0x060033BE RID: 13246 RVA: 0x001A0848 File Offset: 0x0019EA48
		public int GetConnectionIndex(NodePort port)
		{
			for (int i = 0; i < this.ConnectionCount; i++)
			{
				if (this.connections[i].Port == port)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060033BF RID: 13247 RVA: 0x001A0880 File Offset: 0x0019EA80
		public bool IsConnectedTo(NodePort port)
		{
			for (int i = 0; i < this.connections.Count; i++)
			{
				if (this.connections[i].Port == port)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060033C0 RID: 13248 RVA: 0x001A08BC File Offset: 0x0019EABC
		public bool CanConnectTo(NodePort port)
		{
			NodePort nodePort = null;
			NodePort nodePort2 = null;
			if (this.IsInput)
			{
				nodePort = this;
			}
			else
			{
				nodePort2 = this;
			}
			if (port.IsInput)
			{
				nodePort = port;
			}
			else
			{
				nodePort2 = port;
			}
			return nodePort != null && nodePort2 != null && (nodePort.typeConstraint != Node.TypeConstraint.Inherited || nodePort.ValueType.IsAssignableFrom(nodePort2.ValueType)) && (nodePort.typeConstraint != Node.TypeConstraint.Strict || !(nodePort.ValueType != nodePort2.ValueType));
		}

		// Token: 0x060033C1 RID: 13249 RVA: 0x001A0930 File Offset: 0x0019EB30
		public void Disconnect(NodePort port)
		{
			for (int i = this.connections.Count - 1; i >= 0; i--)
			{
				if (this.connections[i].Port == port)
				{
					this.connections.RemoveAt(i);
				}
			}
			if (port != null)
			{
				for (int j = 0; j < port.connections.Count; j++)
				{
					if (port.connections[j].Port == this)
					{
						port.connections.RemoveAt(j);
					}
				}
			}
			this.node.OnRemoveConnection(this);
			if (port != null)
			{
				port.node.OnRemoveConnection(port);
			}
		}

		// Token: 0x060033C2 RID: 13250 RVA: 0x001A09CC File Offset: 0x0019EBCC
		public void Disconnect(int i)
		{
			NodePort port = this.connections[i].Port;
			if (port != null)
			{
				for (int j = 0; j < port.connections.Count; j++)
				{
					if (port.connections[j].Port == this)
					{
						port.connections.RemoveAt(i);
					}
				}
			}
			this.connections.RemoveAt(i);
			this.node.OnRemoveConnection(this);
			if (port != null)
			{
				port.node.OnRemoveConnection(port);
			}
		}

		// Token: 0x060033C3 RID: 13251 RVA: 0x001A0A4B File Offset: 0x0019EC4B
		public void ClearConnections()
		{
			while (this.connections.Count > 0)
			{
				this.Disconnect(this.connections[0].Port);
			}
		}

		// Token: 0x060033C4 RID: 13252 RVA: 0x001A0A74 File Offset: 0x0019EC74
		public List<Vector2> GetReroutePoints(int index)
		{
			return this.connections[index].reroutePoints;
		}

		// Token: 0x060033C5 RID: 13253 RVA: 0x001A0A88 File Offset: 0x0019EC88
		public void SwapConnections(NodePort targetPort)
		{
			int count = this.connections.Count;
			int count2 = targetPort.connections.Count;
			List<NodePort> list = new List<NodePort>();
			List<NodePort> list2 = new List<NodePort>();
			for (int i = 0; i < count; i++)
			{
				list.Add(this.connections[i].Port);
			}
			for (int j = 0; j < count2; j++)
			{
				list2.Add(targetPort.connections[j].Port);
			}
			this.ClearConnections();
			targetPort.ClearConnections();
			for (int k = 0; k < list.Count; k++)
			{
				targetPort.Connect(list[k]);
			}
			for (int l = 0; l < list2.Count; l++)
			{
				this.Connect(list2[l]);
			}
		}

		// Token: 0x060033C6 RID: 13254 RVA: 0x001A0B5C File Offset: 0x0019ED5C
		public void AddConnections(NodePort targetPort)
		{
			int connectionCount = targetPort.ConnectionCount;
			for (int i = 0; i < connectionCount; i++)
			{
				NodePort port = targetPort.connections[i].Port;
				this.Connect(port);
			}
		}

		// Token: 0x060033C7 RID: 13255 RVA: 0x001A0B98 File Offset: 0x0019ED98
		public void MoveConnections(NodePort targetPort)
		{
			int count = this.connections.Count;
			for (int i = 0; i < count; i++)
			{
				NodePort port = targetPort.connections[i].Port;
				this.Connect(port);
			}
			this.ClearConnections();
		}

		// Token: 0x060033C8 RID: 13256 RVA: 0x001A0BDC File Offset: 0x0019EDDC
		public void Redirect(List<Node> oldNodes, List<Node> newNodes)
		{
			foreach (NodePort.PortConnection portConnection in this.connections)
			{
				int num = oldNodes.IndexOf(portConnection.node);
				if (num >= 0)
				{
					portConnection.node = newNodes[num];
				}
			}
		}

		// Token: 0x040022CD RID: 8909
		private Type valueType;

		// Token: 0x040022CE RID: 8910
		[SerializeField]
		private string _fieldName;

		// Token: 0x040022CF RID: 8911
		[SerializeField]
		private Node _node;

		// Token: 0x040022D0 RID: 8912
		[SerializeField]
		private string _typeQualifiedName;

		// Token: 0x040022D1 RID: 8913
		[SerializeField]
		private List<NodePort.PortConnection> connections = new List<NodePort.PortConnection>();

		// Token: 0x040022D2 RID: 8914
		[SerializeField]
		private NodePort.IO _direction;

		// Token: 0x040022D3 RID: 8915
		[SerializeField]
		private Node.ConnectionType _connectionType;

		// Token: 0x040022D4 RID: 8916
		[SerializeField]
		private Node.TypeConstraint _typeConstraint;

		// Token: 0x040022D5 RID: 8917
		[SerializeField]
		private bool _dynamic;

		// Token: 0x020008AD RID: 2221
		public enum IO
		{
			// Token: 0x0400408D RID: 16525
			Input,
			// Token: 0x0400408E RID: 16526
			Output
		}

		// Token: 0x020008AE RID: 2222
		[Serializable]
		private class PortConnection
		{
			// Token: 0x1700067B RID: 1659
			// (get) Token: 0x060051D4 RID: 20948 RVA: 0x0023EB98 File Offset: 0x0023CD98
			public NodePort Port
			{
				get
				{
					if (this.port == null)
					{
						return this.port = this.GetPort();
					}
					return this.port;
				}
			}

			// Token: 0x060051D5 RID: 20949 RVA: 0x0023EBC3 File Offset: 0x0023CDC3
			public PortConnection(NodePort port)
			{
				this.port = port;
				this.node = port.node;
				this.fieldName = port.fieldName;
			}

			// Token: 0x060051D6 RID: 20950 RVA: 0x0023EBF5 File Offset: 0x0023CDF5
			private NodePort GetPort()
			{
				if (this.node == null || string.IsNullOrEmpty(this.fieldName))
				{
					return null;
				}
				return this.node.GetPort(this.fieldName);
			}

			// Token: 0x0400408F RID: 16527
			[SerializeField]
			public string fieldName;

			// Token: 0x04004090 RID: 16528
			[SerializeField]
			public Node node;

			// Token: 0x04004091 RID: 16529
			[NonSerialized]
			private NodePort port;

			// Token: 0x04004092 RID: 16530
			[SerializeField]
			public List<Vector2> reroutePoints = new List<Vector2>();
		}
	}
}
