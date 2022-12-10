using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043E RID: 1086
	[Serializable]
	public class IntReference
	{
		// Token: 0x06003A7E RID: 14974 RVA: 0x001C3777 File Offset: 0x001C1977
		public IntReference()
		{
			this.UseConstant = true;
			this.ConstantValue = 0;
		}

		// Token: 0x06003A7F RID: 14975 RVA: 0x001C3794 File Offset: 0x001C1994
		public IntReference(bool variable = false)
		{
			this.UseConstant = !variable;
			if (!variable)
			{
				this.ConstantValue = 0;
				return;
			}
			this.Variable = ScriptableObject.CreateInstance<IntVar>();
			this.Variable.Value = 0;
		}

		// Token: 0x06003A80 RID: 14976 RVA: 0x001C37CF File Offset: 0x001C19CF
		public IntReference(int value)
		{
			this.Value = value;
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06003A81 RID: 14977 RVA: 0x001C37E5 File Offset: 0x001C19E5
		// (set) Token: 0x06003A82 RID: 14978 RVA: 0x001C3801 File Offset: 0x001C1A01
		public int Value
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

		// Token: 0x06003A83 RID: 14979 RVA: 0x001C381F File Offset: 0x001C1A1F
		public static implicit operator int(IntReference reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A59 RID: 10841
		public bool UseConstant = true;

		// Token: 0x04002A5A RID: 10842
		public int ConstantValue;

		// Token: 0x04002A5B RID: 10843
		public int ResetValue;

		// Token: 0x04002A5C RID: 10844
		public IntVar Variable;
	}
}
