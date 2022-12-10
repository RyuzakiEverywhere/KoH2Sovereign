using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x02000441 RID: 1089
	[CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/String Var")]
	public class StringVar : ScriptableObject
	{
		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06003A8F RID: 14991 RVA: 0x001C3956 File Offset: 0x001C1B56
		// (set) Token: 0x06003A90 RID: 14992 RVA: 0x001C395E File Offset: 0x001C1B5E
		public virtual string Value
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

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06003A91 RID: 14993 RVA: 0x001C3989 File Offset: 0x001C1B89
		// (set) Token: 0x06003A92 RID: 14994 RVA: 0x001C3991 File Offset: 0x001C1B91
		public virtual string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x001C399A File Offset: 0x001C1B9A
		public virtual void ResetValue()
		{
			this.Value = this.DefaultValue;
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x001C39A8 File Offset: 0x001C1BA8
		public virtual void SetValue(StringVar var)
		{
			this.Value = var.Value;
			this.DefaultValue = var.DefaultValue;
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x001C39C2 File Offset: 0x001C1BC2
		public static implicit operator string(StringVar reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A63 RID: 10851
		[SerializeField]
		private string value = "";

		// Token: 0x04002A64 RID: 10852
		[SerializeField]
		private string defaultValue = "";

		// Token: 0x04002A65 RID: 10853
		public bool UseEvent = true;

		// Token: 0x04002A66 RID: 10854
		public StringEvent OnValueChanged = new StringEvent();
	}
}
