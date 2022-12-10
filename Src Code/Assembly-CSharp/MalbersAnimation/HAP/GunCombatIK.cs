using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000431 RID: 1073
	[CreateAssetMenu(menuName = "Malbers Animations/HAP/Gun Combat IK")]
	public class GunCombatIK : GunCombat
	{
		// Token: 0x060039AD RID: 14765 RVA: 0x001C0463 File Offset: 0x001BE663
		public override void ActivateAbility()
		{
			base.ActivateAbility();
			base.EnableAimIKBehaviour(true);
		}

		// Token: 0x040029BF RID: 10687
		private static Keyframe[] KeyFrames = new Keyframe[]
		{
			new Keyframe(0f, 0.61f),
			new Keyframe(1.25f, 0.61f),
			new Keyframe(2f, 0.4f)
		};

		// Token: 0x040029C0 RID: 10688
		[Space]
		public Vector3 RightHandOffset;

		// Token: 0x040029C1 RID: 10689
		public Vector3 LeftHandOffset;

		// Token: 0x040029C2 RID: 10690
		public AnimationCurve HandIKDistance = new AnimationCurve(GunCombatIK.KeyFrames);
	}
}
