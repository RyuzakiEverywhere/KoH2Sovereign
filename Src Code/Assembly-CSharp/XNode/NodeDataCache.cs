using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XNode
{
	// Token: 0x0200035B RID: 859
	public static class NodeDataCache
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06003394 RID: 13204 RVA: 0x0019FB83 File Offset: 0x0019DD83
		private static bool Initialized
		{
			get
			{
				return NodeDataCache.portDataCache != null;
			}
		}

		// Token: 0x06003395 RID: 13205 RVA: 0x0019FB90 File Offset: 0x0019DD90
		public static void UpdatePorts(Node node, Dictionary<string, NodePort> ports)
		{
			if (!NodeDataCache.Initialized)
			{
				NodeDataCache.BuildCache();
			}
			Dictionary<string, NodePort> dictionary = new Dictionary<string, NodePort>();
			Type type = node.GetType();
			List<NodePort> list;
			if (NodeDataCache.portDataCache.TryGetValue(type, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					dictionary.Add(list[i].fieldName, NodeDataCache.portDataCache[type][i]);
				}
			}
			foreach (NodePort nodePort in ports.Values.ToList<NodePort>())
			{
				NodePort nodePort2;
				if (dictionary.TryGetValue(nodePort.fieldName, out nodePort2))
				{
					if (nodePort.connectionType != nodePort2.connectionType || nodePort.IsDynamic || nodePort.direction != nodePort2.direction || nodePort.typeConstraint != nodePort2.typeConstraint)
					{
						ports.Remove(nodePort.fieldName);
					}
					else
					{
						nodePort.ValueType = nodePort2.ValueType;
					}
				}
				else if (nodePort.IsStatic)
				{
					ports.Remove(nodePort.fieldName);
				}
			}
			foreach (NodePort nodePort3 in dictionary.Values)
			{
				if (!ports.ContainsKey(nodePort3.fieldName))
				{
					ports.Add(nodePort3.fieldName, new NodePort(nodePort3, node));
				}
			}
		}

		// Token: 0x06003396 RID: 13206 RVA: 0x0019FD28 File Offset: 0x0019DF28
		private static void BuildCache()
		{
			NodeDataCache.portDataCache = new NodeDataCache.PortDataCache();
			Type baseType = typeof(Node);
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly assembly = Assembly.GetAssembly(baseType);
			if (assembly.FullName.StartsWith("Assembly-CSharp", StringComparison.Ordinal) && !assembly.FullName.Contains("-firstpass"))
			{
				list.AddRange(from t in assembly.GetTypes()
				where !t.IsAbstract && baseType.IsAssignableFrom(t)
				select t);
			}
			else
			{
				Func<Type, bool> <>9__1;
				foreach (Assembly assembly2 in assemblies)
				{
					if (!assembly2.FullName.StartsWith("Unity", StringComparison.Ordinal) && assembly2.FullName.Contains("Version=0.0.0"))
					{
						List<Type> list2 = list;
						IEnumerable<Type> types = assembly2.GetTypes();
						Func<Type, bool> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((Type t) => !t.IsAbstract && baseType.IsAssignableFrom(t)));
						}
						list2.AddRange(types.Where(predicate).ToArray<Type>());
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				NodeDataCache.CachePorts(list[j]);
			}
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x0019FE54 File Offset: 0x0019E054
		public static List<FieldInfo> GetNodeFields(Type nodeType)
		{
			List<FieldInfo> list = new List<FieldInfo>(nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			Type type = nodeType;
			while ((type = type.BaseType) != typeof(Node))
			{
				list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
			}
			return list;
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x0019FE9C File Offset: 0x0019E09C
		private static void CachePorts(Type nodeType)
		{
			List<FieldInfo> nodeFields = NodeDataCache.GetNodeFields(nodeType);
			for (int i = 0; i < nodeFields.Count; i++)
			{
				object[] customAttributes = nodeFields[i].GetCustomAttributes(true);
				Node.InputAttribute inputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.InputAttribute) as Node.InputAttribute;
				Node.OutputAttribute outputAttribute = customAttributes.FirstOrDefault((object x) => x is Node.OutputAttribute) as Node.OutputAttribute;
				if (inputAttribute != null || outputAttribute != null)
				{
					if (inputAttribute != null && outputAttribute != null)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Field ",
							nodeFields[i].Name,
							" of type ",
							nodeType.FullName,
							" cannot be both input and output."
						}));
					}
					else
					{
						if (!NodeDataCache.portDataCache.ContainsKey(nodeType))
						{
							NodeDataCache.portDataCache.Add(nodeType, new List<NodePort>());
						}
						NodeDataCache.portDataCache[nodeType].Add(new NodePort(nodeFields[i]));
					}
				}
			}
		}

		// Token: 0x040022CB RID: 8907
		private static NodeDataCache.PortDataCache portDataCache;

		// Token: 0x020008AA RID: 2218
		[Serializable]
		private class PortDataCache : Dictionary<Type, List<NodePort>>, ISerializationCallbackReceiver
		{
			// Token: 0x060051CA RID: 20938 RVA: 0x0023EA48 File Offset: 0x0023CC48
			public void OnBeforeSerialize()
			{
				this.keys.Clear();
				this.values.Clear();
				foreach (KeyValuePair<Type, List<NodePort>> keyValuePair in this)
				{
					this.keys.Add(keyValuePair.Key);
					this.values.Add(keyValuePair.Value);
				}
			}

			// Token: 0x060051CB RID: 20939 RVA: 0x0023EACC File Offset: 0x0023CCCC
			public void OnAfterDeserialize()
			{
				base.Clear();
				if (this.keys.Count != this.values.Count)
				{
					throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", Array.Empty<object>()));
				}
				for (int i = 0; i < this.keys.Count; i++)
				{
					base.Add(this.keys[i], this.values[i]);
				}
			}

			// Token: 0x04004085 RID: 16517
			[SerializeField]
			private List<Type> keys = new List<Type>();

			// Token: 0x04004086 RID: 16518
			[SerializeField]
			private List<List<NodePort>> values = new List<List<NodePort>>();
		}
	}
}
