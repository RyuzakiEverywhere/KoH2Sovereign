using System;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000418 RID: 1048
	public interface IBow : IMWeapon
	{
		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060038A8 RID: 14504
		// (set) Token: 0x060038A9 RID: 14505
		GameObject ArrowInstance { get; set; }

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060038AA RID: 14506
		Transform KNot { get; }

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060038AB RID: 14507
		Transform ArrowPoint { get; }

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060038AC RID: 14508
		float HoldTime { get; }

		// Token: 0x060038AD RID: 14509
		void ReleaseArrow(Vector3 Direction);

		// Token: 0x060038AE RID: 14510
		void EquipArrow();

		// Token: 0x060038AF RID: 14511
		void DestroyArrow();

		// Token: 0x060038B0 RID: 14512
		void BendBow(float normalizedTime);

		// Token: 0x060038B1 RID: 14513
		void RestoreKnot();
	}
}
