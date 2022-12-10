using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x02000440 RID: 1088
	[Serializable]
	public class StringReference
	{
		// Token: 0x06003A89 RID: 14985 RVA: 0x001C387E File Offset: 0x001C1A7E
		public StringReference()
		{
			this.UseConstant = true;
			this.ConstantValue = string.Empty;
		}

		// Token: 0x06003A8A RID: 14986 RVA: 0x001C38A0 File Offset: 0x001C1AA0
		public StringReference(bool variable = false)
		{
			this.UseConstant = !variable;
			if (!variable)
			{
				this.ConstantValue = string.Empty;
				return;
			}
			this.Variable = ScriptableObject.CreateInstance<StringVar>();
			this.Variable.Value = string.Empty;
			this.Variable.DefaultValue = string.Empty;
		}

		// Token: 0x06003A8B RID: 14987 RVA: 0x001C38FE File Offset: 0x001C1AFE
		public StringReference(string value)
		{
			this.Value = value;
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06003A8C RID: 14988 RVA: 0x001C3914 File Offset: 0x001C1B14
		// (set) Token: 0x06003A8D RID: 14989 RVA: 0x001C3930 File Offset: 0x001C1B30
		public string Value
		{
			get
			{
				if (!this.UseConstant)
				{
					return this.Variable.Value;
				}
				return this.ConstantValue;
			}
			set
			{
				if (this.UseConstant)
				{
					this.ConstantValue = value;
					return;
				}
				this.Variable.Value = value;
			}
		}

		// Token: 0x06003A8E RID: 14990 RVA: 0x001C394E File Offset: 0x001C1B4E
		public static implicit operator string(StringReference reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A60 RID: 10848
		public bool UseConstant = true;

		// Token: 0x04002A61 RID: 10849
		public string ConstantValue;

		// Token: 0x04002A62 RID: 10850
		public StringVar Variable;
	}
}
