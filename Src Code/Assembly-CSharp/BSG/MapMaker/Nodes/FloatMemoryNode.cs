using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000387 RID: 903
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Float Memory")]
	[Serializable]
	public class FloatMemoryNode : Node, IDebugNode
	{
		// Token: 0x0600349D RID: 13469 RVA: 0x001A4EDC File Offset: 0x001A30DC
		public override void Build()
		{
			if (this.debugNode && !MapMakerGraph.Debug_Mode)
			{
				return;
			}
			Terrain tgt_terrain = base.mm.tgt_terrain;
			if (((tgt_terrain != null) ? tgt_terrain.terrainData : null) == null)
			{
				return;
			}
			this.val = base.GetInputValue<float>("val", this.val);
			if (base.mm.generated_floats == null)
			{
				base.mm.generated_floats = new Dictionary<string, float>();
			}
			base.mm.generated_floats[this.key] = this.val;
		}

		// Token: 0x0600349E RID: 13470 RVA: 0x001A4F69 File Offset: 0x001A3169
		public bool IsDebugMode()
		{
			return this.debugNode;
		}

		// Token: 0x0400238F RID: 9103
		public string key;

		// Token: 0x04002390 RID: 9104
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float val;

		// Token: 0x04002391 RID: 9105
		[Space]
		public bool debugNode = true;
	}
}
