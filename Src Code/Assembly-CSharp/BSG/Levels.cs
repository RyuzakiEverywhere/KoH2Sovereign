using System;
using Unity.Mathematics;

namespace BSG
{
	// Token: 0x0200036F RID: 879
	public static class Levels
	{
		// Token: 0x0600341E RID: 13342 RVA: 0x001A1EF8 File Offset: 0x001A00F8
		public static float3 InputRange(float3 color, float minInput, float maxInput)
		{
			return math.min(math.max(color - math.float3(minInput, minInput, minInput), math.float3(0f, 0f, 0f)) / (math.float3(maxInput, maxInput, maxInput) - math.float3(minInput, minInput, minInput)), math.float3(1f, 1f, 1f));
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x001A1F60 File Offset: 0x001A0160
		public static float3 Gamma(float3 color, float gamma)
		{
			if (gamma < 0f)
			{
				gamma = math.lerp(1f, 0.01f, -gamma);
			}
			else
			{
				gamma = math.lerp(1f, 10f, gamma);
			}
			return math.pow(color, 1f / gamma);
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x001A1FAE File Offset: 0x001A01AE
		public static float3 OutRange(float3 color, float minOutput, float maxOutput)
		{
			return math.lerp(math.float3(minOutput, minOutput, minOutput), math.float3(maxOutput, maxOutput, maxOutput), color);
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x001A1FC6 File Offset: 0x001A01C6
		public static float3 Calc(float3 color, float minInput, float gamma, float maxInput)
		{
			return Levels.Gamma(Levels.InputRange(color, minInput, maxInput), gamma);
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x001A1FD6 File Offset: 0x001A01D6
		public static float3 Calc(float3 color, float minInput, float gamma, float maxInput, float minOutput, float maxOutput)
		{
			return Levels.OutRange(Levels.Calc(color, minInput, gamma, maxInput), minOutput, maxOutput);
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x001A1FEA File Offset: 0x001A01EA
		public static float InputRange(float value, float minInput, float maxInput)
		{
			return math.min(math.max(value - minInput, 0f) / (maxInput - minInput), 1f);
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x001A2007 File Offset: 0x001A0207
		public static float Gamma(float value, float gamma)
		{
			if (gamma < 0f)
			{
				gamma = math.lerp(1f, 0.01f, -gamma);
			}
			else
			{
				gamma = math.lerp(1f, 10f, gamma);
			}
			return math.pow(value, 1f / gamma);
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x001A2045 File Offset: 0x001A0245
		public static float OutRange(float value, float minOutput, float maxOutput)
		{
			return math.lerp(minOutput, maxOutput, value);
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x001A204F File Offset: 0x001A024F
		public static float Calc(float value, float minInput, float gamma, float maxInput)
		{
			return Levels.Gamma(Levels.InputRange(value, minInput, maxInput), gamma);
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x001A205F File Offset: 0x001A025F
		public static float Calc(float value, float minInput, float gamma, float maxInput, float minOutput, float maxOutput)
		{
			return Levels.OutRange(Levels.Calc(value, minInput, gamma, maxInput), minOutput, maxOutput);
		}
	}
}
