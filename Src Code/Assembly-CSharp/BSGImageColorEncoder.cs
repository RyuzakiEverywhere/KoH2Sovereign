using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class BSGImageColorEncoder : MonoBehaviour
{
	// Token: 0x060018C0 RID: 6336 RVA: 0x000F29AC File Offset: 0x000F0BAC
	private static int F2I(float f)
	{
		return (int)(f * 255f + 0.5f);
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x000F29BC File Offset: 0x000F0BBC
	private static float I2F(int i)
	{
		return (float)i / 255f;
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x000F29C8 File Offset: 0x000F0BC8
	public static Color CalcEncodedColor(BSGImageColorEncoder.EffectType effectType, BSGImageColorEncoder.MaskChannel maskChannel, Color color)
	{
		Color color2 = color;
		int num = BSGImageColorEncoder.F2I(color2.a) >> 3;
		int num2 = (int)((int)effectType << 5);
		color2.a = BSGImageColorEncoder.I2F(num | num2);
		int num3 = BSGImageColorEncoder.F2I(color2.b);
		if (num3 < 4)
		{
			num3 = 4;
		}
		else if (num3 > 250)
		{
			num3 = 250;
		}
		int num4 = num3 >> 2;
		int num5 = (int)((int)maskChannel << 6);
		color2.b = BSGImageColorEncoder.I2F(num4 | num5);
		return color2;
	}

	// Token: 0x04001008 RID: 4104
	public BSGImageColorEncoder.EffectType effectType = BSGImageColorEncoder.EffectType.Multiply;

	// Token: 0x04001009 RID: 4105
	public BSGImageColorEncoder.MaskChannel maskChannel = BSGImageColorEncoder.MaskChannel.Alpha;

	// Token: 0x0400100A RID: 4106
	public Color color = Color.clear;

	// Token: 0x0400100B RID: 4107
	public string result;

	// Token: 0x02000701 RID: 1793
	public enum EffectType
	{
		// Token: 0x040037CD RID: 14285
		ColorizeWithDarken,
		// Token: 0x040037CE RID: 14286
		HardLight,
		// Token: 0x040037CF RID: 14287
		ColorizeIgnoreAlpha,
		// Token: 0x040037D0 RID: 14288
		Overlay,
		// Token: 0x040037D1 RID: 14289
		Screen,
		// Token: 0x040037D2 RID: 14290
		Multiply
	}

	// Token: 0x02000702 RID: 1794
	public enum MaskChannel
	{
		// Token: 0x040037D4 RID: 14292
		Red,
		// Token: 0x040037D5 RID: 14293
		Green,
		// Token: 0x040037D6 RID: 14294
		Blue,
		// Token: 0x040037D7 RID: 14295
		Alpha
	}
}
