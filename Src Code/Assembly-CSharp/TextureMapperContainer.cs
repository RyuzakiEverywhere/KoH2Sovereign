using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000106 RID: 262
[Serializable]
public class TextureMapperContainer : Dictionary<Color, TextureMapper>
{
	// Token: 0x06000C26 RID: 3110 RVA: 0x000885BC File Offset: 0x000867BC
	public TextureMapperContainer()
	{
		base[Color.red] = new TextureMapper(Color.red, "red");
		base[Color.green] = new TextureMapper(Color.green, "green");
		base[Color.blue] = new TextureMapper(Color.blue, "blue");
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00088620 File Offset: 0x00086820
	public TextureMapperContainer(List<Color> colors, TextureMapperContainer container)
	{
		bool flag = container != null;
		for (int i = 0; i < colors.Count; i++)
		{
			Color color = colors[i];
			if (flag && container.ContainsKey(color))
			{
				base[color] = container[color];
			}
			else
			{
				string text = CoatOfArmsUtility.KeyByValue(CoatOfArms.Instance.paletteColors, color);
				if (text == null)
				{
					CoatOfArmsUtility.colorToLetter.TryGetValue(color, out text);
				}
				if (text == null)
				{
					text = CoatOfArmsUtility.GetNearestPaletteColor(CoatOfArms.Instance.paletteColors, color);
				}
				base[color] = new TextureMapper(color, text);
			}
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x000886B4 File Offset: 0x000868B4
	public TextureMapperContainer(List<Color> colors)
	{
		for (int i = 0; i < colors.Count; i++)
		{
			Color color = colors[i];
			string text = CoatOfArmsUtility.KeyByValue(CoatOfArms.Instance.paletteColors, color);
			if (text == null)
			{
				CoatOfArmsUtility.colorToLetter.TryGetValue(color, out text);
			}
			if (text == null)
			{
				text = CoatOfArmsUtility.GetNearestPaletteColor(CoatOfArms.Instance.paletteColors, color);
			}
			base[color] = new TextureMapper(color, text);
		}
	}
}
