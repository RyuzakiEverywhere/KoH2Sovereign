using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class PolygonalChainSDF2D : SDF2D
{
	// Token: 0x0600114A RID: 4426 RVA: 0x000B6D43 File Offset: 0x000B4F43
	public PolygonalChainSDF2D(float2[] vertices)
	{
		this.vertices = vertices;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x000B6D52 File Offset: 0x000B4F52
	public PolygonalChainSDF2D(IEnumerable<float2> vertices)
	{
		this.vertices = vertices.ToArray<float2>();
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x000B6D66 File Offset: 0x000B4F66
	public PolygonalChainSDF2D(Vector3[] vertices)
	{
		this.vertices = (from v in vertices
		select math.float3(v).xz).ToArray<float2>();
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x000B6DA0 File Offset: 0x000B4FA0
	private float SegmentSDF2D(in float2 p, in float2 a, in float2 b)
	{
		float2 @float = b - a;
		float2 float2 = p - a;
		float lhs = math.clamp(math.dot(float2, @float) / math.dot(@float, @float), 0f, 1f);
		return math.length(float2 - lhs * @float);
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x000B6E00 File Offset: 0x000B5000
	public float SDF2D(float2 point)
	{
		int num = this.vertices.Length;
		if (num == 1)
		{
			return math.distance(point, this.vertices[0]);
		}
		float num2 = this.SegmentSDF2D(point, this.vertices[0], this.vertices[1]);
		for (int i = 1; i < num - 1; i++)
		{
			num2 = math.min(num2, this.SegmentSDF2D(point, this.vertices[i], this.vertices[i + 1]));
		}
		return num2;
	}

	// Token: 0x04000B81 RID: 2945
	public float2[] vertices;
}
