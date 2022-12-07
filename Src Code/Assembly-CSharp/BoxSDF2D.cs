using System;
using Unity.Mathematics;

// Token: 0x02000147 RID: 327
public class BoxSDF2D : SDF2D
{
	// Token: 0x06001143 RID: 4419 RVA: 0x000B6BCB File Offset: 0x000B4DCB
	public BoxSDF2D(float2 center, float2 extents)
	{
		this.center = center;
		this.extents = extents;
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x000B6BE4 File Offset: 0x000B4DE4
	public float SDF2D(float2 point)
	{
		float2 @float = math.abs(point - this.center) - this.extents;
		return math.length(math.max(@float, 0f)) + math.min(math.max(@float.x, @float.y), 0f);
	}

	// Token: 0x04000B7C RID: 2940
	public float2 center;

	// Token: 0x04000B7D RID: 2941
	public float2 extents;
}
