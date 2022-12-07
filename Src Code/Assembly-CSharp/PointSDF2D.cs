using System;
using Unity.Mathematics;

// Token: 0x0200014B RID: 331
public class PointSDF2D : SDF2D
{
	// Token: 0x0600114F RID: 4431 RVA: 0x000B6E86 File Offset: 0x000B5086
	public PointSDF2D(float2 position)
	{
		this.position = position;
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x000B6E95 File Offset: 0x000B5095
	public float SDF2D(float2 point)
	{
		return math.distance(point, this.position);
	}

	// Token: 0x04000B82 RID: 2946
	public float2 position;
}
