using System;
using Unity.Mathematics;

// Token: 0x0200014C RID: 332
public class CircleSDF2D : SDF2D
{
	// Token: 0x06001151 RID: 4433 RVA: 0x000B6EA3 File Offset: 0x000B50A3
	public CircleSDF2D(float2 position, float radius)
	{
		this.position = position;
		this.radius = radius;
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x000B6EB9 File Offset: 0x000B50B9
	public float SDF2D(float2 point)
	{
		return math.distance(point, this.position) - this.radius;
	}

	// Token: 0x04000B83 RID: 2947
	public float2 position;

	// Token: 0x04000B84 RID: 2948
	public float radius;
}
