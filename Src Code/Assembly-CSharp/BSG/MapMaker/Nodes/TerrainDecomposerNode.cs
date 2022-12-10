using System;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B4 RID: 948
	[Node.CreateNodeMenuAttribute("BSG Nodes/Terrain/TerrainDecomposer")]
	public class TerrainDecomposerNode : Node
	{
		// Token: 0x060035A0 RID: 13728 RVA: 0x001AE384 File Offset: 0x001AC584
		public override object GetValue(NodePort port)
		{
			this.terrain = base.GetInputValue<GetTerrainCB>("terrain", default(GetTerrainCB));
			string fieldName = port.fieldName;
			if (!(fieldName == "heights"))
			{
				if (!(fieldName == "splats"))
				{
					if (!(fieldName == "trees"))
					{
						if (!(fieldName == "objects"))
						{
							return null;
						}
						if (this.terrain.objects.objects.data == null)
						{
							Debug.LogError("No heights data found");
							return null;
						}
						return this.terrain.objects;
					}
					else
					{
						if (this.terrain.trees.data == null)
						{
							Debug.LogError("No trees data found");
							return null;
						}
						return this.terrain.trees;
					}
				}
				else
				{
					if (this.terrain.splats.data == null)
					{
						Debug.LogError("No trees data found");
						return null;
					}
					return this.terrain.splats;
				}
			}
			else
			{
				if (this.terrain.heights.data == null)
				{
					Debug.LogError("No heights data found");
					return null;
				}
				return this.terrain.heights;
			}
		}

		// Token: 0x04002516 RID: 9494
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTerrainCB terrain;

		// Token: 0x04002517 RID: 9495
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB heights;

		// Token: 0x04002518 RID: 9496
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB splats;

		// Token: 0x04002519 RID: 9497
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB trees;

		// Token: 0x0400251A RID: 9498
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage objects;
	}
}
