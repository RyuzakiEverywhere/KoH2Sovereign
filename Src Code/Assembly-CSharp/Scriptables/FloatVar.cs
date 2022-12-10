using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
	// Token: 0x0200043D RID: 1085
	[CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Float Var")]
	public class FloatVar : ScriptableObject
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06003A7A RID: 14970 RVA: 0x001C372E File Offset: 0x001C192E
		// (set) Token: 0x06003A7B RID: 14971 RVA: 0x001C3736 File Offset: 0x001C1936
		public virtual float Value
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

		// Token: 0x06003A7C RID: 14972 RVA: 0x001C375C File Offset: 0x001C195C
		public static implicit operator float(FloatVar reference)
		{
			return reference.Value;
		}

		// Token: 0x04002A56 RID: 10838
		[SerializeField]
		private float value;

		// Token: 0x04002A57 RID: 10839
		public bool UseEvent;

		// Token: 0x04002A58 RID: 10840
		public FloatEvent OnValueChanged = new FloatEvent();
	}
}
