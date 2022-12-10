using System;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B7 RID: 951
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Transformation")]
	[Serializable]
	public class TransformationNode : Node
	{
		// Token: 0x060035AF RID: 13743 RVA: 0x001AEE53 File Offset: 0x001AD053
		public override void CleanUp()
		{
			this.isDataFilled = false;
		}

		// Token: 0x060035B0 RID: 13744 RVA: 0x001AEE5C File Offset: 0x001AD05C
		private void FillData()
		{
			if (this.transformation == null)
			{
				this.transformation = new Transformation();
			}
			this.offset = base.GetInputValue<float2>("offset", this.transformation.Offset);
			this.transformation.Offset = this.offset;
			this.isDataFilled = true;
		}

		// Token: 0x060035B1 RID: 13745 RVA: 0x001AEEB0 File Offset: 0x001AD0B0
		public override object GetValue(NodePort port)
		{
			if (!this.isDataFilled)
			{
				this.FillData();
			}
			return this.transformation;
		}

		// Token: 0x0400253A RID: 9530
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float2 offset;

		// Token: 0x0400253B RID: 9531
		public Transformation transformation;

		// Token: 0x0400253C RID: 9532
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public Transformation res;

		// Token: 0x0400253D RID: 9533
		private bool isDataFilled;
	}
}
