using System;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043A RID: 1082
	[Serializable]
	public class BoolReference
	{
		// Token: 0x06003A6A RID: 14954 RVA: 0x001C358B File Offset: 0x001C178B
		public BoolReference()
		{
			this.UseConstant = true;
			this.ConstantValue = false;
			this.DefaultValue = false;
		}

		// Token: 0x06003A6B RID: 14955 RVA: 0x001C35AF File Offset: 0x001C17AF
		public BoolReference(bool value)
		{
			this.Value = value;
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06003A6C RID: 14956 RVA: 0x001C35C5 File Offset: 0x001C17C5
		// (set) Token: 0x06003A6D RID: 14957 RVA: 0x001C35E1 File Offset: 0x001C17E1
		public bool Value
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

		// Token: 0x06003A6E RID: 14958 RVA: 0x001C35FF File Offset: 0x001C17FF
		public static implicit operator bool(BoolReference reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A4C RID: 10828
		public bool UseConstant = true;

		// Token: 0x04002A4D RID: 10829
		public bool ConstantValue;

		// Token: 0x04002A4E RID: 10830
		public bool DefaultValue;

		// Token: 0x04002A4F RID: 10831
		public BoolVar Variable;
	}
}
