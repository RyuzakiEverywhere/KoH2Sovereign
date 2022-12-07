using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class BSGShieldImageEncoder : MonoBehaviour
{
	// Token: 0x060018C4 RID: 6340 RVA: 0x000F29AC File Offset: 0x000F0BAC
	private static int F2I(float f)
	{
		return (int)(f * 255f + 0.5f);
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x000F29BC File Offset: 0x000F0BBC
	private static float I2F(int i)
	{
		return (float)i / 255f;
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x000F2A58 File Offset: 0x000F0C58
	public static Color CalcEncodedColor(int kID, BSGShieldImageEncoder.Frame fr)
	{
		Color black = Color.black;
		black.g = BSGShieldImageEncoder.I2F((int)fr);
		int num = kID / 255;
		int num2 = kID % 255;
		black.r = (float)num / 255f;
		black.b = (float)num2 / 255f;
		black.a = 1f;
		return black;
	}

	// Token: 0x0400100C RID: 4108
	public int kingdomID;

	// Token: 0x0400100D RID: 4109
	public BSGShieldImageEncoder.Frame frame;

	// Token: 0x0400100E RID: 4110
	public string result;

	// Token: 0x02000703 RID: 1795
	public enum Frame
	{
		// Token: 0x040037D9 RID: 14297
		Normal,
		// Token: 0x040037DA RID: 14298
		NormalGP,
		// Token: 0x040037DB RID: 14299
		Vassal,
		// Token: 0x040037DC RID: 14300
		VassalGP
	}
}
