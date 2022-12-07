using System;
using System.Collections.Generic;
using Unity.Mathematics;

// Token: 0x02000148 RID: 328
public class CombinedSDF2DObject : SDF2D
{
	// Token: 0x06001145 RID: 4421 RVA: 0x000B6C40 File Offset: 0x000B4E40
	public float SDF2D(float2 point)
	{
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.sdf2DObjects.Count; i++)
		{
			num = math.min(num, this.sdf2DObjects[i].SDF2D(point));
		}
		return num;
	}

	// Token: 0x04000B7E RID: 2942
	public List<SDF2D> sdf2DObjects = new List<SDF2D>();
}
