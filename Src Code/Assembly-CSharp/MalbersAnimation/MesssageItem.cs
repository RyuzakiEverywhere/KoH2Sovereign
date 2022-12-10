using System;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations
{
	// Token: 0x020003DC RID: 988
	[Serializable]
	public class MesssageItem
	{
		// Token: 0x06003774 RID: 14196 RVA: 0x001B753C File Offset: 0x001B573C
		public MesssageItem()
		{
			this.message = string.Empty;
			this.Active = true;
		}

		// Token: 0x04002773 RID: 10099
		public string message;

		// Token: 0x04002774 RID: 10100
		public TypeMessage typeM;

		// Token: 0x04002775 RID: 10101
		public bool boolValue;

		// Token: 0x04002776 RID: 10102
		public int intValue;

		// Token: 0x04002777 RID: 10103
		public float floatValue;

		// Token: 0x04002778 RID: 10104
		public string stringValue;

		// Token: 0x04002779 RID: 10105
		public IntVar intVarValue;

		// Token: 0x0400277A RID: 10106
		public float time;

		// Token: 0x0400277B RID: 10107
		public bool sent;

		// Token: 0x0400277C RID: 10108
		public bool Active = true;
	}
}
