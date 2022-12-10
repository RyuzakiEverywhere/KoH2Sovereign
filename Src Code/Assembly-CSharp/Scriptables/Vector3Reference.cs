using System;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x02000442 RID: 1090
	[Serializable]
	public class Vector3Reference
	{
		// Token: 0x06003A97 RID: 14999 RVA: 0x001C39FA File Offset: 0x001C1BFA
		public Vector3Reference()
		{
			this.UseConstant = true;
			this.ConstantValue = Vector3.zero;
			this.DefaultValue = Vector3.zero;
		}

		// Token: 0x06003A98 RID: 15000 RVA: 0x001C3A34 File Offset: 0x001C1C34
		public Vector3Reference(bool variable = false)
		{
			this.UseConstant = !variable;
			if (!variable)
			{
				this.ConstantValue = Vector3.zero;
				return;
			}
			this.Variable = ScriptableObject.CreateInstance<Vector3Var>();
			this.Variable.Value = Vector3.zero;
		}

		// Token: 0x06003A99 RID: 15001 RVA: 0x001C3A8D File Offset: 0x001C1C8D
		public Vector3Reference(Vector3 value)
		{
			this.Value = value;
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06003A9A RID: 15002 RVA: 0x001C3AAE File Offset: 0x001C1CAE
		// (set) Token: 0x06003A9B RID: 15003 RVA: 0x001C3ACA File Offset: 0x001C1CCA
		public Vector3 Value
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

		// Token: 0x06003A9C RID: 15004 RVA: 0x001C3AE8 File Offset: 0x001C1CE8
		public static implicit operator Vector3(Vector3Reference reference)
		{
			return reference.Value;
		}

		// Token: 0x06003A9D RID: 15005 RVA: 0x001C3AF0 File Offset: 0x001C1CF0
		public static implicit operator Vector2(Vector3Reference reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A67 RID: 10855
		public bool UseConstant = true;

		// Token: 0x04002A68 RID: 10856
		public Vector3 ConstantValue = Vector3.zero;

		// Token: 0x04002A69 RID: 10857
		public Vector3 DefaultValue;

		// Token: 0x04002A6A RID: 10858
		public Vector3Var Variable;
	}
}
