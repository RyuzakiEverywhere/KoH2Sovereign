using System;

namespace XNode.Examples.MathNodes
{
	// Token: 0x0200036B RID: 875
	public class DisplayValue : Node
	{
		// Token: 0x06003416 RID: 13334 RVA: 0x001A1D3D File Offset: 0x0019FF3D
		public object GetValue()
		{
			return base.GetInputValue<object>("input", null);
		}

		// Token: 0x04002304 RID: 8964
		[Node.InputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.None, false)]
		public DisplayValue.Anything input;

		// Token: 0x020008B0 RID: 2224
		[Serializable]
		public class Anything
		{
		}
	}
}
