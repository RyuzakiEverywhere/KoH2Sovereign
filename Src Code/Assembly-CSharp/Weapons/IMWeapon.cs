using System;
using MalbersAnimations.HAP;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000416 RID: 1046
	public interface IMWeapon
	{
		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06003890 RID: 14480
		// (set) Token: 0x06003891 RID: 14481
		bool Active { get; set; }

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06003892 RID: 14482
		int WeaponID { get; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06003893 RID: 14483
		// (set) Token: 0x06003894 RID: 14484
		WeaponHolder Holder { get; set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06003895 RID: 14485
		Vector3 PositionOffset { get; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06003896 RID: 14486
		Vector3 RotationOffset { get; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06003897 RID: 14487
		bool RightHand { get; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06003898 RID: 14488
		float MinDamage { get; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06003899 RID: 14489
		float MaxDamage { get; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600389A RID: 14490
		// (set) Token: 0x0600389B RID: 14491
		float MinForce { get; set; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x0600389C RID: 14492
		// (set) Token: 0x0600389D RID: 14493
		float MaxForce { get; set; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600389E RID: 14494
		// (set) Token: 0x0600389F RID: 14495
		LayerMask HitMask { get; set; }

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060038A0 RID: 14496
		// (set) Token: 0x060038A1 RID: 14497
		bool IsEquiped { get; set; }

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060038A2 RID: 14498
		// (set) Token: 0x060038A3 RID: 14499
		Rider Owner { get; set; }

		// Token: 0x060038A4 RID: 14500
		void PlaySound(int ID);
	}
}
