using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
[CreateAssetMenu(fileName = "SplineProfile", menuName = "SplineProfile", order = 1)]
public class SplineProfile : ScriptableObject
{
	// Token: 0x04000257 RID: 599
	public Material splineMaterial;

	// Token: 0x04000258 RID: 600
	public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000259 RID: 601
	public float minVal = 0.5f;

	// Token: 0x0400025A RID: 602
	public float maxVal = 0.5f;

	// Token: 0x0400025B RID: 603
	public int vertsInShape = 3;

	// Token: 0x0400025C RID: 604
	public float traingleDensity = 0.2f;

	// Token: 0x0400025D RID: 605
	public float uvScale = 3f;

	// Token: 0x0400025E RID: 606
	public bool uvRotation = true;

	// Token: 0x0400025F RID: 607
	public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(0.5f, 0.05f),
		new Keyframe(1f, 0.025f)
	});

	// Token: 0x04000260 RID: 608
	public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.25f),
		new Keyframe(1f, 0.25f)
	});
}
