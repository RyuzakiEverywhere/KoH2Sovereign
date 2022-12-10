using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043B RID: 1083
	[CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Bool Var")]
	public class BoolVar : ScriptableObject
	{
		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06003A6F RID: 14959 RVA: 0x001C3607 File Offset: 0x001C1807
		// (set) Token: 0x06003A70 RID: 14960 RVA: 0x001C360F File Offset: 0x001C180F
		public virtual bool Value
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

		// Token: 0x06003A71 RID: 14961 RVA: 0x001C3635 File Offset: 0x001C1835
		public virtual void SetValue(BoolVar var)
		{
			this.Value = var.Value;
		}

		// Token: 0x06003A72 RID: 14962 RVA: 0x001C3643 File Offset: 0x001C1843
		public static implicit operator bool(BoolVar reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A50 RID: 10832
		[SerializeField]
		private bool value;

		// Token: 0x04002A51 RID: 10833
		public bool UseEvent = true;

		// Token: 0x04002A52 RID: 10834
		public BoolEvent OnValueChanged = new BoolEvent();
	}
}
