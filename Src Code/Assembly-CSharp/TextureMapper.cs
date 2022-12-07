using System;
using UnityEngine;

// Token: 0x02000107 RID: 263
[Serializable]
public class TextureMapper
{
	// Token: 0x06000C29 RID: 3113 RVA: 0x00088724 File Offset: 0x00086924
	public TextureMapper(Color from, string to)
	{
		this.channelColor = from;
		this.tintColorStr = to;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00088757 File Offset: 0x00086957
	public TextureMapper(Color from, Texture2D texture)
	{
		this.channelColor = from;
		this.texture = new CoatOfArmsTexture(texture, null);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00088790 File Offset: 0x00086990
	public TextureMapper(Color from, Texture2D texture, string tint)
	{
		this.channelColor = from;
		this.texture = new CoatOfArmsTexture(texture, null);
		this.tintColorStr = tint;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000887D0 File Offset: 0x000869D0
	public TextureMapper(Color from, CoatOfArmsTexture texture, string tint)
	{
		this.channelColor = from;
		this.texture = texture;
		this.tintColorStr = tint;
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0008880A File Offset: 0x00086A0A
	public void SetTintColor(string col_str)
	{
		this.tintColorStr = col_str;
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00088813 File Offset: 0x00086A13
	public void SetTintColor(Color col)
	{
		this.tintColorStr = this.GetPaletteColorString(col);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00088824 File Offset: 0x00086A24
	public Color GetTintColor()
	{
		Color result;
		if (!CoatOfArms.Instance.paletteColors.TryGetValue(this.tintColorStr, out result))
		{
			result = this.channelColor;
		}
		return result;
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x00088854 File Offset: 0x00086A54
	public string GetPaletteColorString(Color color)
	{
		string text = null;
		text = CoatOfArmsUtility.KeyByValue(CoatOfArms.Instance.paletteColors, color);
		if (text == null)
		{
			CoatOfArmsUtility.colorToLetter.TryGetValue(color, out text);
		}
		if (text == null)
		{
			text = CoatOfArmsUtility.GetNearestPaletteColor(CoatOfArms.Instance.paletteColors, color);
		}
		return text;
	}

	// Token: 0x04000981 RID: 2433
	public CoatOfArmsTexture texture = new CoatOfArmsTexture(CoatOfArms.Instance.WHITETEXTURE, null);

	// Token: 0x04000982 RID: 2434
	public Color channelColor;

	// Token: 0x04000983 RID: 2435
	public string tintColorStr;

	// Token: 0x04000984 RID: 2436
	public bool remappingTex;

	// Token: 0x04000985 RID: 2437
	public bool visible = true;
}
