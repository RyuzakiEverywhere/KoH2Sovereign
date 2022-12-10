using System;

namespace XNode.Examples.MathNodes
{
	// Token: 0x0200036C RID: 876
	[Serializable]
	public class MathNode : Node
	{
		// Token: 0x06003418 RID: 13336 RVA: 0x001A1D4C File Offset: 0x0019FF4C
		public override object GetValue(NodePort port)
		{
			float inputValue = base.GetInputValue<float>("a", this.a);
			float inputValue2 = base.GetInputValue<float>("b", this.b);
			this.result = 0f;
			if (port.fieldName == "result")
			{
				switch (this.mathType)
				{
				default:
					this.result = inputValue + inputValue2;
					break;
				case MathNode.MathType.Subtract:
					this.result = inputValue - inputValue2;
					break;
				case MathNode.MathType.Multiply:
					this.result = inputValue * inputValue2;
					break;
				case MathNode.MathType.Divide:
					this.result = inputValue / inputValue2;
					break;
				}
			}
			return this.result;
		}

		// Token: 0x04002305 RID: 8965
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float a;

		// Token: 0x04002306 RID: 8966
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float b;

		// Token: 0x04002307 RID: 8967
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float result;

		// Token: 0x04002308 RID: 8968
		public MathNode.MathType mathType;

		// Token: 0x020008B1 RID: 2225
		public enum MathType
		{
			// Token: 0x04004094 RID: 16532
			Add,
			// Token: 0x04004095 RID: 16533
			Subtract,
			// Token: 0x04004096 RID: 16534
			Multiply,
			// Token: 0x04004097 RID: 16535
			Divide
		}
	}
}
