using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
[CreateAssetMenu(menuName = "Benchmarking/CPU Benchmark Assets", fileName = "CPUBenchmarkAssets")]
public class CPUBenchmarkAssets : ScriptableObject
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060001C6 RID: 454 RVA: 0x0001DC56 File Offset: 0x0001BE56
	public AnimationCurve RatingCurve
	{
		get
		{
			return this.rating_curve;
		}
	}

	// Token: 0x040002C7 RID: 711
	[SerializeField]
	private AnimationCurve rating_curve;
}
