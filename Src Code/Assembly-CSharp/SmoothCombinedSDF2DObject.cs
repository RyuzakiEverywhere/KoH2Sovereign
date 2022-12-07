using System;
using System.Collections.Generic;
using Unity.Mathematics;

// Token: 0x02000149 RID: 329
public class SmoothCombinedSDF2DObject : SDF2D
{
	// Token: 0x06001147 RID: 4423 RVA: 0x000B6C98 File Offset: 0x000B4E98
	public float SDF2D(float2 point)
	{
		float num = this.sdf2DObjects[0].SDF2D(point);
		for (int i = 1; i < this.sdf2DObjects.Count; i++)
		{
			num = this.SmoothCombine(num, this.sdf2DObjects[i].SDF2D(point), this.smooth_factor);
		}
		return num;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x000B6CF0 File Offset: 0x000B4EF0
	private float SmoothCombine(float d1, float d2, float k)
	{
		float num = math.max(k - math.abs(d1 - d2), 0f);
		return math.min(d1, d2) - num * num * 0.25f / k;
	}

	// Token: 0x04000B7F RID: 2943
	public List<SDF2D> sdf2DObjects = new List<SDF2D>();

	// Token: 0x04000B80 RID: 2944
	public float smooth_factor = 5f;
}
