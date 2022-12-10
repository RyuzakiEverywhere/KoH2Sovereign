using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A4 RID: 1188
	public static class DuplicateUtility
	{
		// Token: 0x06003E60 RID: 15968 RVA: 0x001DCAA4 File Offset: 0x001DACA4
		public static AnimationCurve DuplicateCurve(AnimationCurve input)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.postWrapMode = input.postWrapMode;
			animationCurve.preWrapMode = input.preWrapMode;
			for (int i = 0; i < input.keys.Length; i++)
			{
				animationCurve.AddKey(input.keys[i]);
			}
			return animationCurve;
		}

		// Token: 0x06003E61 RID: 15969 RVA: 0x000448AF File Offset: 0x00042AAF
		public static Gradient DuplicateGradient(Gradient input)
		{
			return null;
		}
	}
}
