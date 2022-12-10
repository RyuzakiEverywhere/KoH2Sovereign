using System;

namespace MalbersAnimations
{
	// Token: 0x020003D7 RID: 983
	[Serializable]
	public class LayersActivation
	{
		// Token: 0x04002762 RID: 10082
		public string layer;

		// Token: 0x04002763 RID: 10083
		public bool activate;

		// Token: 0x04002764 RID: 10084
		public StateTransition transA;

		// Token: 0x04002765 RID: 10085
		public bool deactivate;

		// Token: 0x04002766 RID: 10086
		public StateTransition transD;
	}
}
