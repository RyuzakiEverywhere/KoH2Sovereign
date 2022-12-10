using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A3 RID: 1187
	public static class DMath
	{
		// Token: 0x06003E4D RID: 15949 RVA: 0x001DC93F File Offset: 0x001DAB3F
		public static double Sin(double a)
		{
			return Math.Sin(a);
		}

		// Token: 0x06003E4E RID: 15950 RVA: 0x001DC947 File Offset: 0x001DAB47
		public static double Cos(double a)
		{
			return Math.Cos(a);
		}

		// Token: 0x06003E4F RID: 15951 RVA: 0x001DC94F File Offset: 0x001DAB4F
		public static double Tan(double a)
		{
			return Math.Tan(a);
		}

		// Token: 0x06003E50 RID: 15952 RVA: 0x001DC957 File Offset: 0x001DAB57
		public static double Pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

		// Token: 0x06003E51 RID: 15953 RVA: 0x001DC960 File Offset: 0x001DAB60
		public static double Log(double a, double newBase)
		{
			return Math.Log(a, newBase);
		}

		// Token: 0x06003E52 RID: 15954 RVA: 0x001DC969 File Offset: 0x001DAB69
		public static double Log10(double a)
		{
			return Math.Log10(a);
		}

		// Token: 0x06003E53 RID: 15955 RVA: 0x001DC971 File Offset: 0x001DAB71
		public static double Clamp01(double a)
		{
			if (a > 1.0)
			{
				return 1.0;
			}
			if (a < 0.0)
			{
				return 0.0;
			}
			return a;
		}

		// Token: 0x06003E54 RID: 15956 RVA: 0x001DC9A0 File Offset: 0x001DABA0
		public static double Clamp(double a, double min, double max)
		{
			if (a > max)
			{
				return max;
			}
			if (a < min)
			{
				return min;
			}
			return a;
		}

		// Token: 0x06003E55 RID: 15957 RVA: 0x001DC9AF File Offset: 0x001DABAF
		public static double Lerp(double a, double b, double t)
		{
			t = DMath.Clamp01(t);
			return a + (b - a) * t;
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x001DC9C0 File Offset: 0x001DABC0
		public static double InverseLerp(double a, double b, double t)
		{
			if (a == b)
			{
				return 0.0;
			}
			return DMath.Clamp01((t - a) / (b - a));
		}

		// Token: 0x06003E57 RID: 15959 RVA: 0x001DC9DC File Offset: 0x001DABDC
		public static Vector3 LerpVector3(Vector3 a, Vector3 b, double t)
		{
			t = DMath.Clamp01(t);
			Vector3 vector = b - a;
			float num = (float)((double)a.x + (double)vector.x * t);
			double num2 = (double)a.y + (double)vector.y * t;
			double num3 = (double)a.z + (double)vector.z * t;
			return new Vector3(num, (float)num2, (float)num3);
		}

		// Token: 0x06003E58 RID: 15960 RVA: 0x001DCA38 File Offset: 0x001DAC38
		public static double Round(double a)
		{
			return Math.Round(a);
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x001DCA40 File Offset: 0x001DAC40
		public static int RoundInt(double a)
		{
			return (int)Math.Round(a);
		}

		// Token: 0x06003E5A RID: 15962 RVA: 0x001DCA49 File Offset: 0x001DAC49
		public static double Ceil(double a)
		{
			return Math.Ceiling(a);
		}

		// Token: 0x06003E5B RID: 15963 RVA: 0x001DCA51 File Offset: 0x001DAC51
		public static int CeilInt(double a)
		{
			return (int)Math.Ceiling(a);
		}

		// Token: 0x06003E5C RID: 15964 RVA: 0x001DCA5A File Offset: 0x001DAC5A
		public static double Floor(double a)
		{
			return Math.Floor(a);
		}

		// Token: 0x06003E5D RID: 15965 RVA: 0x001DCA62 File Offset: 0x001DAC62
		public static int FloorInt(double a)
		{
			return (int)Math.Floor(a);
		}

		// Token: 0x06003E5E RID: 15966 RVA: 0x001DCA6B File Offset: 0x001DAC6B
		public static double Move(double current, double target, double amount)
		{
			if (target > current)
			{
				current += amount;
				if (current > target)
				{
					return target;
				}
			}
			else
			{
				current -= amount;
				if (current < target)
				{
					return target;
				}
			}
			return current;
		}

		// Token: 0x06003E5F RID: 15967 RVA: 0x001DCA88 File Offset: 0x001DAC88
		public static double Abs(double a)
		{
			if (a < 0.0)
			{
				return a * -1.0;
			}
			return a;
		}
	}
}
