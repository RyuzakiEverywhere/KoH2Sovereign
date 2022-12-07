using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class CoatOfArmsUtility
{
	// Token: 0x06000C38 RID: 3128 RVA: 0x0000B82A File Offset: 0x00009A2A
	private CoatOfArmsUtility()
	{
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x000897D0 File Offset: 0x000879D0
	public static Texture2D[] ReadInAllTextures(string[] paths)
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < paths.Length; i++)
		{
			paths[i] = paths[i].Replace("\\", "/");
			Texture2D texture2D = Assets.Get<Texture2D>(paths[i]);
			if (texture2D != null)
			{
				Texture2D item = texture2D;
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x00089824 File Offset: 0x00087A24
	public static List<Color> GetAvailableColors(Texture2D t)
	{
		List<Color> list = new List<Color>();
		Color[] pixels = t.GetPixels();
		for (int i = 0; i < pixels.Length; i += 32)
		{
			Color item = pixels[i];
			if (CoatOfArmsUtility.usableColors.Contains(item) && !list.Contains(item))
			{
				list.Add(item);
			}
			if (list.Count == CoatOfArmsUtility.usableColors.Count)
			{
				break;
			}
		}
		return list;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00089888 File Offset: 0x00087A88
	public static List<Color> GetAllColors(Texture2D t)
	{
		Dictionary<Color, int> od = new Dictionary<Color, int>();
		foreach (Color32 c in t.GetPixels32())
		{
			if (od.ContainsKey(c))
			{
				Dictionary<Color, int> od2 = od;
				Color key = c;
				od2[key] += 50;
			}
			else
			{
				od[c] = 1;
			}
		}
		List<Color> list = new List<Color>();
		foreach (KeyValuePair<Color, int> keyValuePair in od)
		{
			if (CoatOfArmsUtility.usableColors.Contains(keyValuePair.Key))
			{
				list.Add(keyValuePair.Key);
			}
		}
		list.Sort(delegate(Color c1, Color c2)
		{
			if (od[c1] < od[c2])
			{
				return -1;
			}
			return 1;
		});
		return list;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00089990 File Offset: 0x00087B90
	public static void Transform(Texture2D t)
	{
		List<Color> allColors = CoatOfArmsUtility.GetAllColors(t);
		float num = 0.15f;
		Color[] pixels = t.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color color = pixels[i];
			int num2 = allColors.IndexOf(color);
			if (num2 >= 0)
			{
				pixels[i] = CoatOfArmsUtility.usableColors[num2];
			}
			else
			{
				for (int j = 0; j < allColors.Count; j++)
				{
					Color color2 = allColors[j];
					if (Mathf.Abs(color.r - color2.r) < num && Mathf.Abs(color.g - color2.g) < num && Mathf.Abs(color.b - color2.b) < num)
					{
						pixels[i] = color2;
					}
				}
			}
		}
		t.SetPixels(pixels);
		t.Apply();
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00089A6C File Offset: 0x00087C6C
	public static RenderTexture MergeTextures(Texture t1, Texture t2, RenderTexture returnT, Material mat)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(t2.width, t2.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		mat.SetTexture("_Tex2", t2);
		Graphics.Blit(t1, temporary, mat);
		Graphics.Blit(temporary, returnT);
		RenderTexture.ReleaseTemporary(temporary);
		temporary.Release();
		return returnT;
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00089AB8 File Offset: 0x00087CB8
	public static string KeyByValue(Dictionary<string, Color> dict, Color val)
	{
		string result = null;
		foreach (KeyValuePair<string, Color> keyValuePair in dict)
		{
			if (keyValuePair.Value == val)
			{
				result = keyValuePair.Key;
				break;
			}
		}
		return result;
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x00089B1C File Offset: 0x00087D1C
	public static string GetNearestPaletteColor(Dictionary<string, Color> palette, Color c)
	{
		if (palette == null || palette.Count == 0)
		{
			Debug.LogWarning("Cant find palette. Choosing backup color - white");
			return "w";
		}
		float num = 3f;
		KeyValuePair<string, Color> keyValuePair2;
		foreach (KeyValuePair<string, Color> keyValuePair in palette)
		{
			float num2 = Mathf.Sqrt(Mathf.Pow(c.r - keyValuePair.Value.r, 2f) + Mathf.Pow(c.g - keyValuePair.Value.g, 2f) + Mathf.Pow(c.b - keyValuePair.Value.b, 2f));
			if (num2 < num)
			{
				keyValuePair2 = keyValuePair;
				num = num2;
			}
		}
		return keyValuePair2.Key;
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00089BF8 File Offset: 0x00087DF8
	public static DT.Field FindInChildren(DT.Field f, string key)
	{
		DT.Field field = null;
		List<DT.Field> children = f.children;
		field = ((children != null) ? children.Find((DT.Field x) => x.key == key) : null);
		if (field == null && f.children != null)
		{
			foreach (DT.Field f2 in f.children)
			{
				field = CoatOfArmsUtility.FindInChildren(f2, key);
				if (field != null)
				{
					break;
				}
			}
		}
		return field;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00089C90 File Offset: 0x00087E90
	public static CoATransform ConvertChargeTransform(CoATransform transform, CrestMode fromMode, CrestMode toMode)
	{
		CoATransform coATransform = new CoATransform(transform);
		if (fromMode != null && toMode != null)
		{
			float f = (float)toMode.width / (float)fromMode.width;
			coATransform.scale *= f;
		}
		return coATransform;
	}

	// Token: 0x04000995 RID: 2453
	public static readonly List<Color> usableColors = new List<Color>
	{
		Color.red,
		Color.green,
		Color.blue
	};

	// Token: 0x04000996 RID: 2454
	public static readonly Dictionary<string, Color> letterToColor = new Dictionary<string, Color>
	{
		{
			"r",
			Color.red
		},
		{
			"g",
			Color.green
		},
		{
			"b",
			Color.blue
		},
		{
			"c",
			Color.cyan
		},
		{
			"m",
			Color.magenta
		},
		{
			"y",
			Color.yellow
		},
		{
			"k",
			Color.black
		},
		{
			"w",
			Color.white
		}
	};

	// Token: 0x04000997 RID: 2455
	public static readonly Dictionary<Color, string> colorToLetter = new Dictionary<Color, string>
	{
		{
			Color.red,
			"r"
		},
		{
			Color.green,
			"g"
		},
		{
			Color.blue,
			"b"
		},
		{
			Color.cyan,
			"c"
		},
		{
			Color.magenta,
			"m"
		},
		{
			Color.yellow,
			"y"
		},
		{
			Color.black,
			"k"
		},
		{
			Color.white,
			"w"
		}
	};

	// Token: 0x04000998 RID: 2456
	public static Material replaceColorMat;

	// Token: 0x04000999 RID: 2457
	public static Material mergeMat;

	// Token: 0x0400099A RID: 2458
	public static Material coaMat;

	// Token: 0x0400099B RID: 2459
	public static Material coaMat2;
}
