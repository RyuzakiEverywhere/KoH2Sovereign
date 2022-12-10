using System;
using UnityEngine;

namespace XNode.Examples.MathNodes
{
	// Token: 0x0200036E RID: 878
	public class Vector : Node
	{
		// Token: 0x0600341C RID: 13340 RVA: 0x001A1E8C File Offset: 0x001A008C
		public override object GetValue(NodePort port)
		{
			this.vector.x = base.GetInputValue<float>("x", this.x);
			this.vector.y = base.GetInputValue<float>("y", this.y);
			this.vector.z = base.GetInputValue<float>("z", this.z);
			return this.vector;
		}

		// Token: 0x0400230E RID: 8974
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float x;

		// Token: 0x0400230F RID: 8975
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float y;

		// Token: 0x04002310 RID: 8976
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float z;

		// Token: 0x04002311 RID: 8977
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public Vector3 vector;
	}
}
