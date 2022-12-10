using System;
using System.Collections.Generic;
using UnityEngine;

namespace XNode
{
	// Token: 0x0200035A RID: 858
	[Serializable]
	public abstract class Node : ScriptableObject
	{
		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06003370 RID: 13168 RVA: 0x0019F791 File Offset: 0x0019D991
		[Obsolete("Use DynamicPorts instead")]
		public IEnumerable<NodePort> InstancePorts
		{
			get
			{
				return this.DynamicPorts;
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06003371 RID: 13169 RVA: 0x0019F799 File Offset: 0x0019D999
		[Obsolete("Use DynamicOutputs instead")]
		public IEnumerable<NodePort> InstanceOutputs
		{
			get
			{
				return this.DynamicOutputs;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06003372 RID: 13170 RVA: 0x0019F7A1 File Offset: 0x0019D9A1
		[Obsolete("Use DynamicInputs instead")]
		public IEnumerable<NodePort> InstanceInputs
		{
			get
			{
				return this.DynamicInputs;
			}
		}

		// Token: 0x06003373 RID: 13171 RVA: 0x0019F7A9 File Offset: 0x0019D9A9
		[Obsolete("Use AddDynamicInput instead")]
		public NodePort AddInstanceInput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			return this.AddInstanceInput(type, connectionType, typeConstraint, fieldName);
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x0019F7B6 File Offset: 0x0019D9B6
		[Obsolete("Use AddDynamicOutput instead")]
		public NodePort AddInstanceOutput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			return this.AddDynamicOutput(type, connectionType, typeConstraint, fieldName);
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x0019F7C3 File Offset: 0x0019D9C3
		[Obsolete("Use AddDynamicPort instead")]
		private NodePort AddInstancePort(Type type, NodePort.IO direction, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			return this.AddDynamicPort(type, direction, connectionType, typeConstraint, fieldName);
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x0019F7D2 File Offset: 0x0019D9D2
		[Obsolete("Use RemoveDynamicPort instead")]
		public void RemoveInstancePort(string fieldName)
		{
			this.RemoveDynamicPort(fieldName);
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x0019F7DB File Offset: 0x0019D9DB
		[Obsolete("Use RemoveDynamicPort instead")]
		public void RemoveInstancePort(NodePort port)
		{
			this.RemoveDynamicPort(port);
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x0019F7E4 File Offset: 0x0019D9E4
		[Obsolete("Use ClearDynamicPorts instead")]
		public void ClearInstancePorts()
		{
			this.ClearDynamicPorts();
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06003379 RID: 13177 RVA: 0x0019F7EC File Offset: 0x0019D9EC
		public IEnumerable<NodePort> Ports
		{
			get
			{
				foreach (NodePort nodePort in this.ports.Values)
				{
					yield return nodePort;
				}
				Dictionary<string, NodePort>.ValueCollection.Enumerator enumerator = default(Dictionary<string, NodePort>.ValueCollection.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x0600337A RID: 13178 RVA: 0x0019F7FC File Offset: 0x0019D9FC
		public IEnumerable<NodePort> Outputs
		{
			get
			{
				foreach (NodePort nodePort in this.Ports)
				{
					if (nodePort.IsOutput)
					{
						yield return nodePort;
					}
				}
				IEnumerator<NodePort> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x0600337B RID: 13179 RVA: 0x0019F80C File Offset: 0x0019DA0C
		public IEnumerable<NodePort> Inputs
		{
			get
			{
				foreach (NodePort nodePort in this.Ports)
				{
					if (nodePort.IsInput)
					{
						yield return nodePort;
					}
				}
				IEnumerator<NodePort> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x0600337C RID: 13180 RVA: 0x0019F81C File Offset: 0x0019DA1C
		public IEnumerable<NodePort> DynamicPorts
		{
			get
			{
				foreach (NodePort nodePort in this.Ports)
				{
					if (nodePort.IsDynamic)
					{
						yield return nodePort;
					}
				}
				IEnumerator<NodePort> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x0600337D RID: 13181 RVA: 0x0019F82C File Offset: 0x0019DA2C
		public IEnumerable<NodePort> DynamicOutputs
		{
			get
			{
				foreach (NodePort nodePort in this.Ports)
				{
					if (nodePort.IsDynamic && nodePort.IsOutput)
					{
						yield return nodePort;
					}
				}
				IEnumerator<NodePort> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x0600337E RID: 13182 RVA: 0x0019F83C File Offset: 0x0019DA3C
		public IEnumerable<NodePort> DynamicInputs
		{
			get
			{
				foreach (NodePort nodePort in this.Ports)
				{
					if (nodePort.IsDynamic && nodePort.IsInput)
					{
						yield return nodePort;
					}
				}
				IEnumerator<NodePort> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x0019F84C File Offset: 0x0019DA4C
		protected void OnEnable()
		{
			if (Node.graphHotfix != null)
			{
				this.graph = Node.graphHotfix;
			}
			Node.graphHotfix = null;
			this.UpdateStaticPorts();
			this.Init();
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x0019F878 File Offset: 0x0019DA78
		public void UpdateStaticPorts()
		{
			NodeDataCache.UpdatePorts(this, this.ports);
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void Init()
		{
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x0019F888 File Offset: 0x0019DA88
		public void VerifyConnections()
		{
			foreach (NodePort nodePort in this.Ports)
			{
				nodePort.VerifyConnections();
			}
		}

		// Token: 0x06003383 RID: 13187 RVA: 0x0019F8D4 File Offset: 0x0019DAD4
		public NodePort AddDynamicInput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			return this.AddDynamicPort(type, NodePort.IO.Input, connectionType, typeConstraint, fieldName);
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x0019F8E2 File Offset: 0x0019DAE2
		public NodePort AddDynamicOutput(Type type, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			return this.AddDynamicPort(type, NodePort.IO.Output, connectionType, typeConstraint, fieldName);
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x0019F8F0 File Offset: 0x0019DAF0
		private NodePort AddDynamicPort(Type type, NodePort.IO direction, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, string fieldName = null)
		{
			if (fieldName == null)
			{
				fieldName = "dynamicInput_0";
				int num = 0;
				while (this.HasPort(fieldName))
				{
					fieldName = "dynamicInput_" + ++num;
				}
			}
			else if (this.HasPort(fieldName))
			{
				Debug.LogWarning("Port '" + fieldName + "' already exists in " + base.name, this);
				return this.ports[fieldName];
			}
			NodePort nodePort = new NodePort(fieldName, type, direction, connectionType, typeConstraint, this);
			this.ports.Add(fieldName, nodePort);
			return nodePort;
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x0019F980 File Offset: 0x0019DB80
		public void RemoveDynamicPort(string fieldName)
		{
			if (this.GetPort(fieldName) == null)
			{
				throw new ArgumentException("port " + fieldName + " doesn't exist");
			}
			this.RemoveDynamicPort(this.GetPort(fieldName));
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x0019F9AE File Offset: 0x0019DBAE
		public void RemoveDynamicPort(NodePort port)
		{
			if (port == null)
			{
				throw new ArgumentNullException("port");
			}
			if (port.IsStatic)
			{
				throw new ArgumentException("cannot remove static port");
			}
			port.ClearConnections();
			this.ports.Remove(port.fieldName);
		}

		// Token: 0x06003388 RID: 13192 RVA: 0x0019F9EC File Offset: 0x0019DBEC
		[ContextMenu("Clear Dynamic Ports")]
		public void ClearDynamicPorts()
		{
			foreach (NodePort port in new List<NodePort>(this.DynamicPorts))
			{
				this.RemoveDynamicPort(port);
			}
		}

		// Token: 0x06003389 RID: 13193 RVA: 0x0019FA44 File Offset: 0x0019DC44
		public NodePort GetOutputPort(string fieldName)
		{
			NodePort port = this.GetPort(fieldName);
			if (port == null || port.direction != NodePort.IO.Output)
			{
				return null;
			}
			return port;
		}

		// Token: 0x0600338A RID: 13194 RVA: 0x0019FA68 File Offset: 0x0019DC68
		public NodePort GetInputPort(string fieldName)
		{
			NodePort port = this.GetPort(fieldName);
			if (port == null || port.direction != NodePort.IO.Input)
			{
				return null;
			}
			return port;
		}

		// Token: 0x0600338B RID: 13195 RVA: 0x0019FA8C File Offset: 0x0019DC8C
		public NodePort GetPort(string fieldName)
		{
			NodePort result;
			if (this.ports.TryGetValue(fieldName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600338C RID: 13196 RVA: 0x0019FAAC File Offset: 0x0019DCAC
		public bool HasPort(string fieldName)
		{
			return this.ports.ContainsKey(fieldName);
		}

		// Token: 0x0600338D RID: 13197 RVA: 0x0019FABC File Offset: 0x0019DCBC
		public T GetInputValue<T>(string fieldName, T fallback = default(T))
		{
			NodePort port = this.GetPort(fieldName);
			if (port != null && port.IsConnected)
			{
				return port.GetInputValue<T>();
			}
			return fallback;
		}

		// Token: 0x0600338E RID: 13198 RVA: 0x0019FAE4 File Offset: 0x0019DCE4
		public T[] GetInputValues<T>(string fieldName, params T[] fallback)
		{
			NodePort port = this.GetPort(fieldName);
			if (port != null && port.IsConnected)
			{
				return port.GetInputValues<T>();
			}
			return fallback;
		}

		// Token: 0x0600338F RID: 13199 RVA: 0x0019FB0C File Offset: 0x0019DD0C
		public virtual object GetValue(NodePort port)
		{
			Debug.LogWarning("No GetValue(NodePort port) override defined for " + base.GetType());
			return null;
		}

		// Token: 0x06003390 RID: 13200 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void OnCreateConnection(NodePort from, NodePort to)
		{
		}

		// Token: 0x06003391 RID: 13201 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void OnRemoveConnection(NodePort port)
		{
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x0019FB24 File Offset: 0x0019DD24
		public void ClearConnections()
		{
			foreach (NodePort nodePort in this.Ports)
			{
				nodePort.ClearConnections();
			}
		}

		// Token: 0x040022C7 RID: 8903
		[SerializeField]
		public NodeGraph graph;

		// Token: 0x040022C8 RID: 8904
		[SerializeField]
		public Vector2 position;

		// Token: 0x040022C9 RID: 8905
		[SerializeField]
		private Node.NodePortDictionary ports = new Node.NodePortDictionary();

		// Token: 0x040022CA RID: 8906
		public static NodeGraph graphHotfix;

		// Token: 0x0200089B RID: 2203
		public enum ShowBackingValue
		{
			// Token: 0x04004051 RID: 16465
			Never,
			// Token: 0x04004052 RID: 16466
			Unconnected,
			// Token: 0x04004053 RID: 16467
			Always
		}

		// Token: 0x0200089C RID: 2204
		public enum ConnectionType
		{
			// Token: 0x04004055 RID: 16469
			Multiple,
			// Token: 0x04004056 RID: 16470
			Override
		}

		// Token: 0x0200089D RID: 2205
		public enum TypeConstraint
		{
			// Token: 0x04004058 RID: 16472
			None,
			// Token: 0x04004059 RID: 16473
			Inherited,
			// Token: 0x0400405A RID: 16474
			Strict
		}

		// Token: 0x0200089E RID: 2206
		[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
		public class InputAttribute : Attribute
		{
			// Token: 0x1700066D RID: 1645
			// (get) Token: 0x06005186 RID: 20870 RVA: 0x0023DF74 File Offset: 0x0023C174
			// (set) Token: 0x06005187 RID: 20871 RVA: 0x0023DF7C File Offset: 0x0023C17C
			[Obsolete("Use dynamicPortList instead")]
			public bool instancePortList
			{
				get
				{
					return this.dynamicPortList;
				}
				set
				{
					this.dynamicPortList = value;
				}
			}

			// Token: 0x06005188 RID: 20872 RVA: 0x0023DF85 File Offset: 0x0023C185
			public InputAttribute(Node.ShowBackingValue backingValue = Node.ShowBackingValue.Unconnected, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, Node.TypeConstraint typeConstraint = Node.TypeConstraint.None, bool dynamicPortList = false)
			{
				this.backingValue = backingValue;
				this.connectionType = connectionType;
				this.dynamicPortList = dynamicPortList;
				this.typeConstraint = typeConstraint;
			}

			// Token: 0x0400405B RID: 16475
			public Node.ShowBackingValue backingValue;

			// Token: 0x0400405C RID: 16476
			public Node.ConnectionType connectionType;

			// Token: 0x0400405D RID: 16477
			public bool dynamicPortList;

			// Token: 0x0400405E RID: 16478
			public Node.TypeConstraint typeConstraint;
		}

		// Token: 0x0200089F RID: 2207
		[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
		public class OutputAttribute : Attribute
		{
			// Token: 0x1700066E RID: 1646
			// (get) Token: 0x06005189 RID: 20873 RVA: 0x0023DFAA File Offset: 0x0023C1AA
			// (set) Token: 0x0600518A RID: 20874 RVA: 0x0023DFB2 File Offset: 0x0023C1B2
			[Obsolete("Use dynamicPortList instead")]
			public bool instancePortList
			{
				get
				{
					return this.dynamicPortList;
				}
				set
				{
					this.dynamicPortList = value;
				}
			}

			// Token: 0x0600518B RID: 20875 RVA: 0x0023DFBB File Offset: 0x0023C1BB
			public OutputAttribute(Node.ShowBackingValue backingValue = Node.ShowBackingValue.Never, Node.ConnectionType connectionType = Node.ConnectionType.Multiple, bool dynamicPortList = false)
			{
				this.backingValue = backingValue;
				this.connectionType = connectionType;
				this.dynamicPortList = dynamicPortList;
			}

			// Token: 0x0400405F RID: 16479
			public Node.ShowBackingValue backingValue;

			// Token: 0x04004060 RID: 16480
			public Node.ConnectionType connectionType;

			// Token: 0x04004061 RID: 16481
			public bool dynamicPortList;
		}

		// Token: 0x020008A0 RID: 2208
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class CreateNodeMenuAttribute : Attribute
		{
			// Token: 0x0600518C RID: 20876 RVA: 0x0023DFD8 File Offset: 0x0023C1D8
			public CreateNodeMenuAttribute(string menuName)
			{
				this.menuName = menuName;
			}

			// Token: 0x04004062 RID: 16482
			public string menuName;
		}

		// Token: 0x020008A1 RID: 2209
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class NodeTintAttribute : Attribute
		{
			// Token: 0x0600518D RID: 20877 RVA: 0x0023DFE7 File Offset: 0x0023C1E7
			public NodeTintAttribute(float r, float g, float b)
			{
				this.color = new Color(r, g, b);
			}

			// Token: 0x0600518E RID: 20878 RVA: 0x0023DFFD File Offset: 0x0023C1FD
			public NodeTintAttribute(string hex)
			{
				ColorUtility.TryParseHtmlString(hex, out this.color);
			}

			// Token: 0x0600518F RID: 20879 RVA: 0x0023E012 File Offset: 0x0023C212
			public NodeTintAttribute(byte r, byte g, byte b)
			{
				this.color = new Color32(r, g, b, byte.MaxValue);
			}

			// Token: 0x04004063 RID: 16483
			public Color color;
		}

		// Token: 0x020008A2 RID: 2210
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class NodeWidthAttribute : Attribute
		{
			// Token: 0x06005190 RID: 20880 RVA: 0x0023E032 File Offset: 0x0023C232
			public NodeWidthAttribute(int width)
			{
				this.width = width;
			}

			// Token: 0x04004064 RID: 16484
			public int width;
		}

		// Token: 0x020008A3 RID: 2211
		[Serializable]
		private class NodePortDictionary : Dictionary<string, NodePort>, ISerializationCallbackReceiver
		{
			// Token: 0x06005191 RID: 20881 RVA: 0x0023E044 File Offset: 0x0023C244
			public void OnBeforeSerialize()
			{
				this.keys.Clear();
				this.values.Clear();
				foreach (KeyValuePair<string, NodePort> keyValuePair in this)
				{
					this.keys.Add(keyValuePair.Key);
					this.values.Add(keyValuePair.Value);
				}
			}

			// Token: 0x06005192 RID: 20882 RVA: 0x0023E0C8 File Offset: 0x0023C2C8
			public void OnAfterDeserialize()
			{
				base.Clear();
				if (this.keys.Count != this.values.Count)
				{
					throw new Exception(string.Concat(new object[]
					{
						"there are ",
						this.keys.Count,
						" keys and ",
						this.values.Count,
						" values after deserialization. Make sure that both key and value types are serializable."
					}));
				}
				for (int i = 0; i < this.keys.Count; i++)
				{
					base.Add(this.keys[i], this.values[i]);
				}
			}

			// Token: 0x04004065 RID: 16485
			[SerializeField]
			private List<string> keys = new List<string>();

			// Token: 0x04004066 RID: 16486
			[SerializeField]
			private List<NodePort> values = new List<NodePort>();
		}
	}
}
