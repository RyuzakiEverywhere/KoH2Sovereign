using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class Filters
{
	// Token: 0x06000570 RID: 1392 RVA: 0x0003CA2C File Offset: 0x0003AC2C
	public unsafe static float BicubicFilter(float* arr, int layers, int2 cell, float2 frac, int layer, bool wrap, int2 resolution)
	{
		Matrix4x4 matrix4x = default(Matrix4x4);
		int num = cell.x - 1;
		int num2 = cell.y - 1;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num3 = j + num;
				if (wrap)
				{
					num3 %= resolution.x;
					if ((float)num3 < 0f)
					{
						num3 += resolution.x;
					}
				}
				else
				{
					num3 = Mathf.Clamp(num3, 0, resolution.x - 1);
				}
				int num4 = i + num2;
				if (wrap)
				{
					num4 %= resolution.y;
					if ((float)num4 < 0f)
					{
						num4 += resolution.y;
					}
				}
				else
				{
					num4 = Mathf.Clamp(num4, 0, resolution.y - 1);
				}
				int num5 = num4 * resolution.y * layers;
				int num6 = num3 * layers;
				matrix4x[j, i] = arr[num5 + num6 + layer];
			}
		}
		float a = Filters.CubicInterpolate(matrix4x[0], matrix4x[1], matrix4x[2], matrix4x[3], frac.x);
		float b = Filters.CubicInterpolate(matrix4x[4], matrix4x[5], matrix4x[6], matrix4x[7], frac.x);
		float c = Filters.CubicInterpolate(matrix4x[8], matrix4x[9], matrix4x[10], matrix4x[11], frac.x);
		float d = Filters.CubicInterpolate(matrix4x[12], matrix4x[13], matrix4x[14], matrix4x[15], frac.x);
		return Filters.CubicInterpolate(a, b, c, d, frac.y);
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0003CBF8 File Offset: 0x0003ADF8
	private static float CubicInterpolate(float A, float B, float C, float D, float t)
	{
		return B + 0.5f * t * (C - A + t * (2f * A - 5f * B + 4f * C - D + t * (3f * (B - C) + D - A)));
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0003CC44 File Offset: 0x0003AE44
	public unsafe static float BilinearFilter(float* arr, int layers, int2 cell, float2 frac, int layer, bool wrap, int2 resolution)
	{
		int2 @int = cell + 1;
		if (@int.x >= resolution.x)
		{
			@int.x = (wrap ? 0 : cell.x);
		}
		if (@int.y >= resolution.y)
		{
			@int.y = (wrap ? 0 : cell.y);
		}
		float v = arr[cell.y * resolution.y * layers + cell.x * layers + layer];
		float v2 = arr[cell.y * resolution.y * layers + @int.x * layers + layer];
		float v3 = arr[@int.y * resolution.y * layers + cell.x * layers + layer];
		float v4 = arr[@int.y * resolution.y * layers + @int.x * layers + layer];
		return Filters.BilinearFilter(v, v2, v3, v4, frac);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0003CD34 File Offset: 0x0003AF34
	private static float BilinearFilter(float v00, float v10, float v01, float v11, float2 frac)
	{
		return (1f - frac.x) * (1f - frac.y) * v00 + frac.x * (1f - frac.y) * v10 + (1f - frac.x) * frac.y * v01 + frac.x * frac.y * v11;
	}
}
