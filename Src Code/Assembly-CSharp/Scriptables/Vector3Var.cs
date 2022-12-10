using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x02000443 RID: 1091
	[CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Vector3 Var")]
	public class Vector3Var : ScriptableObject
	{
		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06003A9E RID: 15006 RVA: 0x001C3AFD File Offset: 0x001C1CFD
		// (set) Token: 0x06003A9F RID: 15007 RVA: 0x001C3B05 File Offset: 0x001C1D05
		public virtual Vector3 Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					if (this.UseEvent)
					{
						this.OnValueChanged.Invoke(value);
					}
				}
			}
		}

		// Token: 0x06003AA0 RID: 15008 RVA: 0x001C3B30 File Offset: 0x001C1D30
		public virtual void SetValue(Vector3Var var)
		{
			this.Value = var.Value;
		}

		// Token: 0x06003AA1 RID: 15009 RVA: 0x001C3B3E File Offset: 0x001C1D3E
		public static implicit operator Vector3(Vector3Var reference)
		{
			return reference.Value;
		}

		// Token: 0x06003AA2 RID: 15010 RVA: 0x001C3B46 File Offset: 0x001C1D46
		public static implicit operator Vector2(Vector3Var reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A6B RID: 10859
		[SerializeField]
		private Vector3 value = Vector3.zero;

		// Token: 0x04002A6C RID: 10860
		public bool UseEvent = true;

		// Token: 0x04002A6D RID: 10861
		public Vector3Event OnValueChanged = new Vector3Event();
	}
}
