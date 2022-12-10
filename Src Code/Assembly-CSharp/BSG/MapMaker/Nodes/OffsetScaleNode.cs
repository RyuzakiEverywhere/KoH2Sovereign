using System;
using Unity.Burst;
using Unity.Mathematics;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000399 RID: 921
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Offset Scaler")]
	[Serializable]
	public class OffsetScaleNode : Node
	{
		// Token: 0x06003515 RID: 13589 RVA: 0x001A9B9A File Offset: 0x001A7D9A
		public override void CleanUp()
		{
			this.isDataFilled = false;
		}

		// Token: 0x06003516 RID: 13590 RVA: 0x001A9BA4 File Offset: 0x001A7DA4
		private void FillData()
		{
			this.input = base.GetInputValue<float2>("input", this.input);
			float rhs = this.size / this.world_sub_rect_size;
			if (this.operation == OffsetScaleNode.OperationType.Multiply)
			{
				this.res = this.input * rhs;
			}
			else
			{
				this.res = this.input / rhs;
			}
			this.isDataFilled = true;
		}

		// Token: 0x06003517 RID: 13591 RVA: 0x001A9C0B File Offset: 0x001A7E0B
		public override object GetValue(NodePort port)
		{
			if (!this.isDataFilled)
			{
				this.FillData();
			}
			return this.res;
		}

		// Token: 0x04002445 RID: 9285
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float2 input;

		// Token: 0x04002446 RID: 9286
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float2 res;

		// Token: 0x04002447 RID: 9287
		public OffsetScaleNode.OperationType operation;

		// Token: 0x04002448 RID: 9288
		public float size;

		// Token: 0x04002449 RID: 9289
		public float world_sub_rect_size = 60f;

		// Token: 0x0400244A RID: 9290
		private bool isDataFilled;

		// Token: 0x020008DA RID: 2266
		public enum OperationType
		{
			// Token: 0x04004142 RID: 16706
			Multiply,
			// Token: 0x04004143 RID: 16707
			Divide
		}
	}
}
