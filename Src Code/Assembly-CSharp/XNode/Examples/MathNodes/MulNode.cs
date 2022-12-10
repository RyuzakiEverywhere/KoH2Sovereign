using System;
using UnityEngine;

namespace XNode.Examples.MathNodes
{
	// Token: 0x0200036D RID: 877
	[Serializable]
	public class MulNode : Node
	{
		// Token: 0x0600341A RID: 13338 RVA: 0x001A1DEC File Offset: 0x0019FFEC
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
				case MulNode.MathType.Subtract:
					this.result = inputValue - inputValue2;
					break;
				case MulNode.MathType.Multiply:
					this.result = inputValue * inputValue2;
					break;
				case MulNode.MathType.Divide:
					this.result = inputValue / inputValue2;
					break;
				}
			}
			return this.result;
		}

		// Token: 0x04002309 RID: 8969
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float a;

		// Token: 0x0400230A RID: 8970
		[Range(0f, 10f)]
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float b;

		// Token: 0x0400230B RID: 8971
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Texture2D tex;

		// Token: 0x0400230C RID: 8972
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float result;

		// Token: 0x0400230D RID: 8973
		public MulNode.MathType mathType;

		// Token: 0x020008B2 RID: 2226
		public enum MathType
		{
			// Token: 0x04004099 RID: 16537
			Add,
			// Token: 0x0400409A RID: 16538
			Subtract,
			// Token: 0x0400409B RID: 16539
			Multiply,
			// Token: 0x0400409C RID: 16540
			Divide
		}
	}
}
