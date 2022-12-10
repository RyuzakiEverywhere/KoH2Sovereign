using System;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000417 RID: 1047
	public interface IMelee : IMWeapon
	{
		// Token: 0x060038A5 RID: 14501
		void CanDoDamage(bool value);

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060038A6 RID: 14502
		// (set) Token: 0x060038A7 RID: 14503
		bool CanCauseDamage { get; set; }
	}
}
