using System;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B3 RID: 947
	[Node.CreateNodeMenuAttribute("BSG Nodes/Terrain/TerrainComposer")]
	public class TerrainComposerNode : Node
	{
		// Token: 0x0600359E RID: 13726 RVA: 0x001AE27C File Offset: 0x001AC47C
		public override object GetValue(NodePort port)
		{
			this.heights = base.GetInputValue<GetFloat2DCB>("heights", default(GetFloat2DCB));
			this.splats = base.GetInputValue<GetSplatCB>("splats", default(GetSplatCB));
			this.trees = base.GetInputValue<GetTreesCB>("trees", default(GetTreesCB));
			this.objects = base.GetInputValue<ObjectsPackage>("objects", default(ObjectsPackage));
			if (this.heights.data == null || this.splats.data == null || this.trees.data == null || this.objects.objects.data == null)
			{
				Debug.LogError("(TerrainComposerNode) One of more of inputs is not connected, please connect those inputs.");
				return null;
			}
			return new GetTerrainCB
			{
				heights = this.heights,
				splats = this.splats,
				trees = this.trees,
				objects = this.objects
			};
		}

		// Token: 0x04002511 RID: 9489
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB heights;

		// Token: 0x04002512 RID: 9490
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB splats;

		// Token: 0x04002513 RID: 9491
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB trees;

		// Token: 0x04002514 RID: 9492
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage objects;

		// Token: 0x04002515 RID: 9493
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTerrainCB terrain;
	}
}
