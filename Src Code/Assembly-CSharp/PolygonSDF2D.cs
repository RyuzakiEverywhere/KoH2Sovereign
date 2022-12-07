using System;
using Unity.Mathematics;

// Token: 0x02000146 RID: 326
public class PolygonSDF2D : SDF2D
{
	// Token: 0x06001140 RID: 4416 RVA: 0x000B6A44 File Offset: 0x000B4C44
	public PolygonSDF2D(float2[] vertices)
	{
		this.vertices = vertices;
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x000B6A53 File Offset: 0x000B4C53
	private float cross2d(float2 v0, float2 v1)
	{
		return v0.x * v1.y - v0.y * v1.x;
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x000B6A70 File Offset: 0x000B4C70
	public float SDF2D(float2 point)
	{
		int num = this.vertices.Length;
		float x = math.dot(point - this.vertices[0], point - this.vertices[0]);
		float num2 = 1f;
		for (int i = 0; i < num; i++)
		{
			int num3 = (i + 1) % num;
			float2 @float = this.vertices[num3] - this.vertices[i];
			float2 float2 = point - this.vertices[i];
			float2 float3 = float2 - @float * math.clamp(math.dot(float2, @float) / math.dot(@float, @float), 0f, 1f);
			x = math.min(x, math.dot(float3, float3));
			float2 float4 = point - this.vertices[num3];
			float num4 = this.cross2d(@float, float2);
			bool3 @bool = math.bool3((double)float2.y >= 0.0, (double)float4.y < 0.0, (double)num4 > 0.0);
			if (math.all(@bool) || math.all(!@bool))
			{
				num2 *= -1f;
			}
		}
		return math.sqrt(x) * num2;
	}

	// Token: 0x04000B7B RID: 2939
	public float2[] vertices;
}
