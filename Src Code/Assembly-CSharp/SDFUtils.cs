using System;
using Unity.Mathematics;

// Token: 0x0200014E RID: 334
public static class SDFUtils
{
	// Token: 0x06001154 RID: 4436 RVA: 0x000B6ED0 File Offset: 0x000B50D0
	public static float3 FindClosestSurfacePointBetween(Func<float3, float> sdf, float3 point1, float3 point2, int iterations = 10)
	{
		if (math.sign(sdf(point1)) != math.sign(sdf(point2)))
		{
			float3 x;
			float3 y;
			if ((double)sdf(point1) < 0.0)
			{
				x = point1;
				y = point2;
			}
			else
			{
				x = point2;
				y = point1;
			}
			for (int i = 0; i < iterations; i++)
			{
				float3 @float = math.lerp(x, y, 0.5f);
				if ((double)sdf(@float) < 0.0)
				{
					x = @float;
				}
				else
				{
					y = @float;
				}
			}
			return math.lerp(x, y, 0.5f);
		}
		if (math.abs(sdf(point1)) < math.abs(sdf(point2)))
		{
			return point1;
		}
		return point2;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x000B6F70 File Offset: 0x000B5170
	public static float3 GetSDFNormal(Func<float3, float> sdf, float3 position, float epsilon = 0.1f)
	{
		float num = sdf(position + math.float3(epsilon, 0f, 0f));
		float num2 = sdf(position + math.float3(-epsilon, 0f, 0f));
		float num3 = sdf(position + math.float3(0f, epsilon, 0f));
		float num4 = sdf(position + math.float3(0f, -epsilon, 0f));
		float num5 = sdf(position + math.float3(0f, 0f, epsilon));
		float num6 = sdf(position + math.float3(0f, 0f, -epsilon));
		return math.normalize(math.float3(num - num2, num3 - num4, num5 - num6));
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x000B7044 File Offset: 0x000B5244
	public static float2 GetSDFNormal(Func<float2, float> sdf, float2 position, float epsilon = 0.1f)
	{
		float num = sdf(position + math.float2(epsilon, 0f));
		float num2 = sdf(position + math.float2(-epsilon, 0f));
		float num3 = sdf(position + math.float2(0f, epsilon));
		float num4 = sdf(position + math.float2(0f, -epsilon));
		return math.normalize(math.float2(num - num2, num3 - num4));
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x000B70C4 File Offset: 0x000B52C4
	public static float3 SnapToSDF(Func<float3, float> sdf, float3 position)
	{
		float3 @float = -SDFUtils.GetSDFNormal(sdf, position, 0.01f);
		@float = math.normalize(@float);
		return position + @float * sdf(position);
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x000B7100 File Offset: 0x000B5300
	public static float2 SnapToSDF(Func<float2, float> sdf, float2 position)
	{
		float2 lhs = -SDFUtils.GetSDFNormal(sdf, position, 0.01f);
		return position + lhs * sdf(position);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x000B7134 File Offset: 0x000B5334
	public static float2 Raymarch(Func<float2, float> sdf, float2 position, float2 direction, int iterations, float step_multiplier)
	{
		for (int i = 0; i < iterations; i++)
		{
			float num = sdf(position);
			if ((double)num < 0.0)
			{
				return position;
			}
			position += direction * num * step_multiplier;
		}
		return position;
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x000B717C File Offset: 0x000B537C
	public static float2 SnapToSDFByMarching(Func<float2, float> sdf, float2 position, int iterations, float step_multiplier)
	{
		for (int i = 0; i < iterations; i++)
		{
			float2 lhs = -SDFUtils.GetSDFNormal(sdf, position, 0.01f);
			position += lhs * sdf(position) * step_multiplier;
		}
		return position;
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x000B71C4 File Offset: 0x000B53C4
	public static float3 SnapToSDFByMarching(Func<float3, float> sdf, float3 position, int iterations, float step_multiplier)
	{
		for (int i = 0; i < iterations; i++)
		{
			float3 lhs = -SDFUtils.GetSDFNormal(sdf, position, 0.01f);
			position += lhs * sdf(position) * step_multiplier;
		}
		return position;
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x000B720B File Offset: 0x000B540B
	public static Func<float3, float> To3DFunc(this SDF2D sdf)
	{
		return (float3 p) => sdf.SDF2D(p.xz);
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x000B7224 File Offset: 0x000B5424
	public static Func<float2, float> To2DFunc(this SDF2D sdf)
	{
		return (float2 p) => sdf.SDF2D(p);
	}
}
