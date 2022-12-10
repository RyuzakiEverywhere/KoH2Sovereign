using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003FA RID: 1018
	public class DodgeKeys : MonoBehaviour
	{
		// Token: 0x06003843 RID: 14403 RVA: 0x001BB60F File Offset: 0x001B980F
		private void Start()
		{
			this.animal = base.GetComponent<Animal>();
			this.animal.OnMovementReleased.AddListener(new UnityAction<bool>(this.OnMovementReleased));
		}

		// Token: 0x06003844 RID: 14404 RVA: 0x001BB63C File Offset: 0x001B983C
		private void OnMovementReleased(bool released)
		{
			if (!released)
			{
				if (this.animal.Direction != 0f && !this.DodgePressOne)
				{
					this.DodgePressOne = true;
					base.Invoke("ResetDodgeKeys", this.DoubleKeyTime);
					return;
				}
				if (this.animal.Direction != 0f && this.DodgePressOne)
				{
					this.animal.Dodge = true;
					base.Invoke("ResetDodgeKeys", 0.1f);
				}
			}
		}

		// Token: 0x06003845 RID: 14405 RVA: 0x001BB6B5 File Offset: 0x001B98B5
		private void ResetDodgeKeys()
		{
			this.DodgePressOne = false;
			this.animal.Dodge = false;
		}

		// Token: 0x06003846 RID: 14406 RVA: 0x001BB6CA File Offset: 0x001B98CA
		private void OnDisable()
		{
			this.animal.OnMovementReleased.RemoveListener(new UnityAction<bool>(this.OnMovementReleased));
		}

		// Token: 0x04002854 RID: 10324
		private Animal animal;

		// Token: 0x04002855 RID: 10325
		public float DoubleKeyTime = 0.3f;

		// Token: 0x04002856 RID: 10326
		private bool DodgePressOne;
	}
}
