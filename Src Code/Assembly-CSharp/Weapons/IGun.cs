using System;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x0200041A RID: 1050
	public interface IGun : IMWeapon
	{
		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060038B9 RID: 14521
		// (set) Token: 0x060038BA RID: 14522
		bool IsAutomatic { get; set; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060038BB RID: 14523
		// (set) Token: 0x060038BC RID: 14524
		bool IsAiming { get; set; }

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060038BD RID: 14525
		// (set) Token: 0x060038BE RID: 14526
		int TotalAmmo { get; set; }

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060038BF RID: 14527
		// (set) Token: 0x060038C0 RID: 14528
		int AmmoInChamber { get; set; }

		// Token: 0x060038C1 RID: 14529
		void FireProyectile(RaycastHit Direction);

		// Token: 0x060038C2 RID: 14530
		bool Reload();
	}
}
