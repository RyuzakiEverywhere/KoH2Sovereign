using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x0200040F RID: 1039
	[CreateAssetMenu(menuName = "Malbers Animations/Anim Transform")]
	public class TransformAnimation : ScriptableObject
	{
		// Token: 0x040028C1 RID: 10433
		public TransformAnimation.AnimTransType animTrans;

		// Token: 0x040028C2 RID: 10434
		private static Keyframe[] K = new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		};

		// Token: 0x040028C3 RID: 10435
		public float time = 1f;

		// Token: 0x040028C4 RID: 10436
		public float delay = 1f;

		// Token: 0x040028C5 RID: 10437
		public bool UsePosition;

		// Token: 0x040028C6 RID: 10438
		public Vector3 Position;

		// Token: 0x040028C7 RID: 10439
		public AnimationCurve PosCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028C8 RID: 10440
		public bool SeparateAxisPos;

		// Token: 0x040028C9 RID: 10441
		public AnimationCurve PosXCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028CA RID: 10442
		public AnimationCurve PosYCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028CB RID: 10443
		public AnimationCurve PosZCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028CC RID: 10444
		public bool UseRotation;

		// Token: 0x040028CD RID: 10445
		public Vector3 Rotation;

		// Token: 0x040028CE RID: 10446
		public AnimationCurve RotCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028CF RID: 10447
		public bool SeparateAxisRot;

		// Token: 0x040028D0 RID: 10448
		public AnimationCurve RotXCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028D1 RID: 10449
		public AnimationCurve RotYCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028D2 RID: 10450
		public AnimationCurve RotZCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x040028D3 RID: 10451
		public bool UseScale;

		// Token: 0x040028D4 RID: 10452
		public Vector3 Scale = Vector3.one;

		// Token: 0x040028D5 RID: 10453
		public AnimationCurve ScaleCurve = new AnimationCurve(TransformAnimation.K);

		// Token: 0x0200092F RID: 2351
		public enum AnimTransType
		{
			// Token: 0x040042B3 RID: 17075
			TransformAnimation,
			// Token: 0x040042B4 RID: 17076
			MountTriggerAdjustment
		}
	}
}
