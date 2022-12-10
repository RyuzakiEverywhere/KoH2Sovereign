using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x02000413 RID: 1043
	public class MInventory : MonoBehaviour
	{
		// Token: 0x06003881 RID: 14465 RVA: 0x001BC0A8 File Offset: 0x001BA2A8
		private void Update()
		{
			for (int i = 0; i < this.Inventory.Length; i++)
			{
				if (this.Inventory[i].input.GetInput)
				{
					this.EquipItem(i);
					return;
				}
			}
		}

		// Token: 0x06003882 RID: 14466 RVA: 0x001BC0E4 File Offset: 0x001BA2E4
		public virtual void EquipItem(int Slot)
		{
			this.OnEquipItem.Invoke(this.Inventory[Slot].item);
		}

		// Token: 0x040028E0 RID: 10464
		public InventorySlot[] Inventory;

		// Token: 0x040028E1 RID: 10465
		public GameObjectEvent OnEquipItem;
	}
}
