using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043F RID: 1087
	[CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Int Var")]
	public class IntVar : ScriptableObject
	{
		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06003A84 RID: 14980 RVA: 0x001C3827 File Offset: 0x001C1A27
		// (set) Token: 0x06003A85 RID: 14981 RVA: 0x001C382F File Offset: 0x001C1A2F
		public virtual int Value
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

		// Token: 0x06003A86 RID: 14982 RVA: 0x001C3855 File Offset: 0x001C1A55
		public virtual void SetValue(IntVar var)
		{
			this.Value = var.Value;
		}

		// Token: 0x06003A87 RID: 14983 RVA: 0x001C3863 File Offset: 0x001C1A63
		public static implicit operator int(IntVar reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A5D RID: 10845
		[SerializeField]
		private int value;

		// Token: 0x04002A5E RID: 10846
		public bool UseEvent;

		// Token: 0x04002A5F RID: 10847
		public IntEvent OnValueChanged = new IntEvent();
	}
}
