using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000481 RID: 1153
	public static class TextureExtensions
	{
		// Token: 0x06003C5E RID: 15454 RVA: 0x001CC074 File Offset: 0x001CA274
		public static bool IsReadable(this Texture2D tex)
		{
			bool result;
			try
			{
				tex.GetPixel(0, 0);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06003C5F RID: 15455 RVA: 0x0002C538 File Offset: 0x0002A738
		public static bool IsLinear(this Texture tex)
		{
			return false;
		}

		// Token: 0x06003C60 RID: 15456 RVA: 0x001CC0A4 File Offset: 0x001CA2A4
		public static Texture2D ReadableClone(this Texture2D tex)
		{
			Texture2D texture2D = new Texture2D(tex.width, tex.height, tex.format, true, tex.IsLinear());
			Graphics.CopyTexture(tex, texture2D);
			texture2D.Apply(false);
			return texture2D;
		}

		// Token: 0x06003C61 RID: 15457 RVA: 0x001CC0E0 File Offset: 0x001CA2E0
		public static Texture2D UncompressedClone(this Texture2D tex)
		{
			int mipmapCount = tex.mipmapCount;
			Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, mipmapCount != 1, tex.IsLinear());
			for (int i = 0; i < mipmapCount; i++)
			{
				Color[] pixels = tex.GetPixels(i);
				texture2D.SetPixels(pixels, i);
			}
			texture2D.Apply(false);
			return texture2D;
		}

		// Token: 0x06003C62 RID: 15458 RVA: 0x001CC138 File Offset: 0x001CA338
		public static Texture2D ResizedClone(this Texture2D tex, int newWidth, int newHeight)
		{
			Texture2D texture2D = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, true, tex.IsLinear());
			texture2D.name = tex.name;
			Color[] array = tex.GetPixels();
			array = array.ResizeColorArray(tex.width, tex.height, newWidth, newHeight);
			texture2D.SetPixels(array);
			texture2D.Apply(true);
			return texture2D;
		}

		// Token: 0x06003C63 RID: 15459 RVA: 0x001CC18C File Offset: 0x001CA38C
		public static Color[] ResizeColorArray(this Color[] srcColors, int oldWidth, int oldHeight, int newWidth, int newHeight)
		{
			Color[] array = new Color[newWidth * newHeight];
			Matrix matrix = new Matrix(new CoordRect(0, 0, oldWidth, oldHeight), null);
			Matrix matrix2 = new Matrix(new CoordRect(0, 0, newWidth, newHeight), null);
			for (int i = 0; i < srcColors.Length; i++)
			{
				matrix.array[i] = srcColors[i].r;
			}
			matrix2 = matrix.Resize(matrix2.rect, matrix2);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].r = matrix2.array[j];
			}
			for (int k = 0; k < srcColors.Length; k++)
			{
				matrix.array[k] = srcColors[k].g;
			}
			matrix2 = matrix.Resize(matrix2.rect, matrix2);
			for (int l = 0; l < array.Length; l++)
			{
				array[l].g = matrix2.array[l];
			}
			for (int m = 0; m < srcColors.Length; m++)
			{
				matrix.array[m] = srcColors[m].b;
			}
			matrix2 = matrix.Resize(matrix2.rect, matrix2);
			for (int n = 0; n < array.Length; n++)
			{
				array[n].b = matrix2.array[n];
			}
			for (int num = 0; num < srcColors.Length; num++)
			{
				matrix.array[num] = srcColors[num].a;
			}
			matrix2 = matrix.Resize(matrix2.rect, matrix2);
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				array[num2].a = matrix2.array[num2];
			}
			return array;
		}

		// Token: 0x06003C64 RID: 15460 RVA: 0x001CC338 File Offset: 0x001CA538
		public static Texture2D ColorTexture(int width, int height, Color color, bool linear = true)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, true, linear);
			Color[] pixels = texture2D.GetPixels(0, 0, width, height);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = color;
			}
			texture2D.SetPixels(0, 0, width, height, pixels);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x06003C65 RID: 15461 RVA: 0x001CC384 File Offset: 0x001CA584
		public static Texture2D Clone(this Texture2D src)
		{
			Texture2D texture2D = new Texture2D(src.width, src.height, src.format, src.mipmapCount != 1);
			Graphics.CopyTexture(src, texture2D);
			texture2D.Apply(false);
			return texture2D;
		}

		// Token: 0x06003C66 RID: 15462 RVA: 0x001CC3C4 File Offset: 0x001CA5C4
		public static void ClearAlpha(this Texture2D tex)
		{
			Color[] pixels = tex.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = 1f;
			}
			tex.SetPixels(pixels);
			tex.Apply();
		}

		// Token: 0x06003C67 RID: 15463 RVA: 0x001CC404 File Offset: 0x001CA604
		public static void ApplyGamma(this Texture2D tex, float gamma = 2.2f)
		{
			float p = 1f / gamma;
			Color[] pixels = tex.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = new Color(Mathf.Pow(pixels[i].r, p), Mathf.Pow(pixels[i].g, p), Mathf.Pow(pixels[i].b, p), pixels[i].a);
			}
			tex.SetPixels(pixels);
			tex.Apply();
		}

		// Token: 0x06003C68 RID: 15464 RVA: 0x001CC48C File Offset: 0x001CA68C
		public static void RestoreNormalmap(this Texture2D tex)
		{
			Color[] pixels = tex.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				Vector2 v = new Vector2(pixels[i].g * 2f - 1f, pixels[i].a * 2f - 1f);
				float num = Mathf.Sqrt(1f - Mathf.Clamp01(Vector3.Dot(v, v)));
				pixels[i] = new Color(pixels[i].g, pixels[i].a, num / 2f + 0.5f, 1f);
			}
			tex.SetPixels(pixels);
			tex.Apply();
		}

		// Token: 0x06003C69 RID: 15465 RVA: 0x001CC550 File Offset: 0x001CA750
		public static void SaveAsPNG(this Texture2D origTex, string savePath, bool linear = false, bool normal = false)
		{
			Texture2D texture2D = origTex;
			if (!texture2D.IsReadable())
			{
				texture2D = texture2D.ReadableClone();
			}
			if (texture2D.format.IsCompressed())
			{
				texture2D = texture2D.UncompressedClone();
			}
			if (linear)
			{
				if (texture2D == origTex)
				{
					texture2D = texture2D.Clone();
				}
				texture2D.ApplyGamma(0.45454547f);
				texture2D.Apply(false);
			}
			if (normal)
			{
				if (texture2D == origTex)
				{
					texture2D = texture2D.Clone();
				}
				texture2D.RestoreNormalmap();
				texture2D.Apply(false);
			}
			savePath = savePath.Replace(Application.dataPath, "Assets");
			File.WriteAllBytes(savePath, texture2D.EncodeToPNG());
		}

		// Token: 0x06003C6A RID: 15466 RVA: 0x001CC5E8 File Offset: 0x001CA7E8
		public static Hash128 GetHash(this Texture2D tex)
		{
			return default(Hash128);
		}

		// Token: 0x06003C6B RID: 15467 RVA: 0x001CC5FE File Offset: 0x001CA7FE
		public static bool IsCompressed(this TextureFormat format)
		{
			return !TextureExtensions.uncompressedFormats.Contains(format);
		}

		// Token: 0x06003C6C RID: 15468 RVA: 0x001CC610 File Offset: 0x001CA810
		public static TextureFormat AutoFormat(TextureExtensions.TextureType type, bool compressed)
		{
			if (compressed)
			{
				return TextureFormat.DXT5;
			}
			switch (type)
			{
			case TextureExtensions.TextureType.RGB:
				return TextureFormat.RGB24;
			case TextureExtensions.TextureType.Normal:
				return TextureFormat.RG16;
			case TextureExtensions.TextureType.Monochrome:
				return TextureFormat.R8;
			case TextureExtensions.TextureType.MonochromeFloat:
				return TextureFormat.RFloat;
			default:
				return TextureFormat.RGBA32;
			}
		}

		// Token: 0x06003C6D RID: 15469 RVA: 0x001CC63E File Offset: 0x001CA83E
		// Note: this type is marked as 'beforefieldinit'.
		static TextureExtensions()
		{
			TextureFormat[] array = new TextureFormat[16];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.8E4AB413F4E1222F40D127291AF9BD9B5C876E02).FieldHandle);
			TextureExtensions.uncompressedFormats = new HashSet<TextureFormat>(array);
		}

		// Token: 0x04002B76 RID: 11126
		public static readonly HashSet<TextureFormat> uncompressedFormats;

		// Token: 0x0200095F RID: 2399
		public enum TextureType
		{
			// Token: 0x0400437D RID: 17277
			RGBA,
			// Token: 0x0400437E RID: 17278
			RGB,
			// Token: 0x0400437F RID: 17279
			Normal,
			// Token: 0x04004380 RID: 17280
			Monochrome,
			// Token: 0x04004381 RID: 17281
			MonochromeFloat,
			// Token: 0x04004382 RID: 17282
			Manual
		}
	}
}
