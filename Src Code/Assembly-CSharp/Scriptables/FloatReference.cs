using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043C RID: 1084
	[Serializable]
	public class FloatReference
	{
		// Token: 0x06003A74 RID: 14964 RVA: 0x001C3665 File Offset: 0x001C1865
		public FloatReference()
		{
			this.UseConstant = true;
			this.ConstantValue = 0f;
		}

		// Token: 0x06003A75 RID: 14965 RVA: 0x001C3688 File Offset: 0x001C1888
		public FloatReference(bool variable = false)
		{
			this.UseConstant = !variable;
			if (!variable)
			{
				this.ConstantValue = 0f;
				return;
			}
			this.Variable = ScriptableObject.CreateInstance<FloatVar>();
			this.Variable.Value = 0f;
		}

		// Token: 0x06003A76 RID: 14966 RVA: 0x001C36D6 File Offset: 0x001C18D6
		public FloatReference(float value)
		{
			this.Value = value;
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06003A77 RID: 14967 RVA: 0x001C36EC File Offset: 0x001C18EC
		// (set) Token: 0x06003A78 RID: 14968 RVA: 0x001C3708 File Offset: 0x001C1908
		public float Value
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

		// Token: 0x06003A79 RID: 14969 RVA: 0x001C3726 File Offset: 0x001C1926
		public static implicit operator float(FloatReference reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A53 RID: 10835
		public bool UseConstant = true;

		// Token: 0x04002A54 RID: 10836
		public float ConstantValue;

		// Token: 0x04002A55 RID: 10837
		public FloatVar Variable;
	}
}
