using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200046E RID: 1134
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class FlagAttribute : PropertyAttribute
	{
		// Token: 0x06003B66 RID: 15206 RVA: 0x0003E12E File Offset: 0x0003C32E
		public FlagAttribute()
		{
		}

		// Token: 0x06003B67 RID: 15207 RVA: 0x001C6F33 File Offset: 0x001C5133
		public FlagAttribute(string name)
		{
			this.enumName = name;
		}

		// Token: 0x04002B32 RID: 11058
		public string enumName;
	}
}
