using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
[Serializable]
public class BSGTerrainMask : ScriptableObject
{
	// Token: 0x0600058D RID: 1421 RVA: 0x0003D6BC File Offset: 0x0003B8BC
	public static float CalcFadeInOut(float val, float fade_in, float min_val, float max_val, float fade_out)
	{
		float num;
		float num2;
		if (fade_in < 0f)
		{
			fade_in = -fade_in;
			num = min_val;
			num2 = min_val + fade_in;
		}
		else
		{
			num = min_val - fade_in;
			num2 = min_val;
		}
		float num3;
		float num4;
		if (fade_out < 0f)
		{
			fade_out = -fade_out;
			num3 = max_val - fade_out;
			num4 = max_val;
		}
		else
		{
			num3 = max_val;
			num4 = max_val + fade_out;
		}
		if (val >= num2 && val <= num3)
		{
			return 1f;
		}
		if (val <= num || val >= num4)
		{
			return 0f;
		}
		if (val < num2)
		{
			return (val - num) / fade_in;
		}
		return 1f - (val - num3) / fade_out;
	}
}
