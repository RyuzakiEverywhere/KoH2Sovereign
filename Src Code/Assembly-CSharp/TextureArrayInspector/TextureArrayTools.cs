using System;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000482 RID: 1154
	public static class TextureArrayTools
	{
		// Token: 0x06003C6E RID: 15470 RVA: 0x001CC65C File Offset: 0x001CA85C
		public static void SetTexture(this Texture2DArray dstArr, Texture2D src, int dstCh, bool apply = true)
		{
			if (dstArr.depth <= dstCh)
			{
				throw new IndexOutOfRangeException(string.Concat(new object[]
				{
					"Trying to set channel (",
					dstCh,
					") >= depth (",
					dstArr.depth,
					")"
				}));
			}
			if (src.width == dstArr.width && src.height == dstArr.height && src.format == dstArr.format)
			{
				Graphics.CopyTexture(src, 0, dstArr, dstCh);
				if (apply)
				{
					dstArr.Apply(false);
				}
				return;
			}
			if (!src.IsReadable())
			{
				src = src.ReadableClone();
			}
			if (src.format.IsCompressed())
			{
				src = src.UncompressedClone();
			}
			if (src.width != dstArr.width || src.height != dstArr.height)
			{
				src = src.ResizedClone(dstArr.width, dstArr.height);
			}
			if (dstArr.format.IsCompressed())
			{
				src.Compress(true);
			}
			src.Apply(false);
			Graphics.CopyTexture(src, 0, dstArr, dstCh);
			if (apply)
			{
				dstArr.Apply(false);
			}
		}

		// Token: 0x06003C6F RID: 15471 RVA: 0x001CC774 File Offset: 0x001CA974
		public static void SetTextureAlpha(this Texture2DArray dstArr, Texture2D src, Texture2D alpha, int dstCh, bool apply = true)
		{
			if (alpha == null)
			{
				dstArr.SetTexture(src, dstCh, apply);
				return;
			}
			if (!src.IsReadable())
			{
				src = src.ReadableClone();
			}
			if (src.format.IsCompressed())
			{
				src = src.UncompressedClone();
			}
			if (src.width != dstArr.width || src.height != dstArr.height)
			{
				src = src.ResizedClone(dstArr.width, dstArr.height);
			}
			if (!alpha.IsReadable())
			{
				alpha = alpha.ReadableClone();
			}
			if (alpha.width != dstArr.width || alpha.height != dstArr.height)
			{
				alpha = alpha.ResizedClone(dstArr.width, dstArr.height);
			}
			Texture2D texture2D = new Texture2D(src.width, src.height, TextureFormat.RGBA32, true, src.IsLinear());
			int mipmapCount = src.mipmapCount;
			for (int i = 0; i < mipmapCount; i++)
			{
				Color[] pixels = src.GetPixels(i);
				Color[] pixels2 = alpha.GetPixels(i);
				for (int j = 0; j < pixels.Length; j++)
				{
					pixels[j] = new Color(pixels[j].r, pixels[j].g, pixels[j].b, pixels2[j].r * 0.3f + pixels2[j].r * 0.6f + pixels2[j].r * 0.1f);
				}
				texture2D.SetPixels(pixels, i);
			}
			texture2D.Apply(false);
			dstArr.SetTexture(texture2D, dstCh, apply);
		}

		// Token: 0x06003C70 RID: 15472 RVA: 0x001CC90C File Offset: 0x001CAB0C
		public static Texture2D GetTexture(this Texture2DArray srcArr, int srcCh, bool readable = true)
		{
			Texture2D texture2D = new Texture2D(srcArr.width, srcArr.height, srcArr.format, true, srcArr.IsLinear());
			Graphics.CopyTexture(srcArr, srcCh, texture2D, 0);
			texture2D.Apply(false, !readable);
			return texture2D;
		}

		// Token: 0x06003C71 RID: 15473 RVA: 0x001CC950 File Offset: 0x001CAB50
		public static Texture2D[] GetTextures(this Texture2DArray srcArr)
		{
			Texture2D[] array = new Texture2D[srcArr.depth];
			for (int i = 0; i < srcArr.depth; i++)
			{
				array[i] = srcArr.GetTexture(i, true);
			}
			return array;
		}

		// Token: 0x06003C72 RID: 15474 RVA: 0x001CC986 File Offset: 0x001CAB86
		public static Color GetPixel(this Texture2DArray srcArr, int x, int y, int ch)
		{
			return srcArr.GetTexture(ch, true).GetPixel(x, y);
		}

		// Token: 0x06003C73 RID: 15475 RVA: 0x001CC998 File Offset: 0x001CAB98
		public static void FillTexture(this Texture2DArray srcArr, Texture2D dst, int srcCh)
		{
			if (srcArr.depth <= srcCh)
			{
				throw new IndexOutOfRangeException(string.Concat(new object[]
				{
					"Trying to get channel (",
					srcCh,
					") >= depth (",
					srcArr.depth,
					")"
				}));
			}
			if (srcArr.format == dst.format && srcArr.width == dst.width && srcArr.height == dst.height)
			{
				Graphics.CopyTexture(srcArr, srcCh, dst, 0);
				dst.Apply(false);
				return;
			}
			Texture2D texture2D = srcArr.GetTexture(srcCh, true);
			if (texture2D.format.IsCompressed())
			{
				texture2D = texture2D.UncompressedClone();
			}
			if (texture2D.width != dst.width || texture2D.height != dst.height)
			{
				texture2D = texture2D.ResizedClone(dst.width, dst.height);
			}
			if (dst.format.IsCompressed())
			{
				texture2D.Compress(true);
			}
			texture2D.Apply(false);
			Graphics.CopyTexture(texture2D, dst);
			dst.Apply(false);
		}

		// Token: 0x06003C74 RID: 15476 RVA: 0x001CCA9E File Offset: 0x001CAC9E
		public static void CopyTexture(Texture2DArray srcArr, int srcCh, Texture2DArray dstArr, int dstCh)
		{
			TextureArrayTools.CopyTextures(srcArr, srcCh, dstArr, dstCh, 1);
		}

		// Token: 0x06003C75 RID: 15477 RVA: 0x001CCAAA File Offset: 0x001CACAA
		public static void CopyTextures(Texture2DArray srcArr, Texture2DArray dstArr, int length)
		{
			TextureArrayTools.CopyTextures(srcArr, 0, dstArr, 0, length);
		}

		// Token: 0x06003C76 RID: 15478 RVA: 0x001CCAB8 File Offset: 0x001CACB8
		public static void CopyTextures(Texture2DArray srcArr, int srcIndex, Texture2DArray dstArr, int dstIndex, int length)
		{
			if (srcArr.format == dstArr.format && srcArr.width == dstArr.width && srcArr.height == dstArr.height)
			{
				for (int i = 0; i < length; i++)
				{
					Graphics.CopyTexture(srcArr, srcIndex + i, dstArr, dstIndex + i);
				}
				dstArr.Apply(false);
				return;
			}
			Texture2D texture2D = new Texture2D(dstArr.width, dstArr.height, dstArr.format, true, srcArr.IsLinear());
			for (int j = 0; j < length; j++)
			{
				srcArr.FillTexture(texture2D, srcIndex + j);
				Graphics.CopyTexture(texture2D, 0, dstArr, dstIndex + j);
			}
			dstArr.Apply(false);
		}

		// Token: 0x06003C77 RID: 15479 RVA: 0x001CCB5C File Offset: 0x001CAD5C
		public static void Add(ref Texture2DArray texArr, Texture2D tex)
		{
			Texture2DArray newArr = TextureArrayTools.Add(texArr, tex);
			TextureArrayTools.Rewrite(ref texArr, newArr);
		}

		// Token: 0x06003C78 RID: 15480 RVA: 0x001CCB7C File Offset: 0x001CAD7C
		public static Texture2DArray Add(Texture2DArray texArr, Texture2D tex)
		{
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth + 1, texArr.format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			TextureArrayTools.CopyTextures(texArr, texture2DArray, texArr.depth);
			texture2DArray.SetTexture(tex, texArr.depth, false);
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C79 RID: 15481 RVA: 0x001CCBE0 File Offset: 0x001CADE0
		public static void Insert(ref Texture2DArray texArr, int pos, Texture2D tex)
		{
			Texture2DArray newArr = TextureArrayTools.Insert(texArr, pos, tex);
			TextureArrayTools.Rewrite(ref texArr, newArr);
		}

		// Token: 0x06003C7A RID: 15482 RVA: 0x001CCC00 File Offset: 0x001CAE00
		public static Texture2DArray Insert(Texture2DArray texArr, int pos, Texture2D tex)
		{
			bool linear = texArr.IsLinear();
			if (texArr == null || texArr.depth == 0)
			{
				texArr = new Texture2DArray(tex.width, tex.height, 1, texArr.format, true, linear);
				texArr.filterMode = FilterMode.Trilinear;
				texArr.SetTexture(tex, 0, false);
				return texArr;
			}
			if (pos > texArr.depth || pos < 0)
			{
				pos = texArr.depth;
			}
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth + 1, texArr.format, true, linear);
			texture2DArray.name = texArr.name;
			if (pos != 0)
			{
				TextureArrayTools.CopyTextures(texArr, texture2DArray, pos);
			}
			if (pos != texArr.depth)
			{
				TextureArrayTools.CopyTextures(texArr, pos, texture2DArray, pos + 1, texArr.depth - pos);
			}
			if (tex != null)
			{
				texture2DArray.SetTexture(tex, pos, false);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x001CCCD8 File Offset: 0x001CAED8
		public static Texture2DArray InsertRange(Texture2DArray texArr, int pos, Texture2DArray addArr)
		{
			if (pos > texArr.depth || pos < 0)
			{
				pos = texArr.depth;
			}
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth + addArr.depth, texArr.format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			if (pos != 0)
			{
				TextureArrayTools.CopyTextures(texArr, texture2DArray, pos);
			}
			TextureArrayTools.CopyTextures(addArr, 0, texture2DArray, pos, addArr.depth);
			if (pos != texArr.depth)
			{
				TextureArrayTools.CopyTextures(texArr, pos, texture2DArray, pos + addArr.depth, texArr.depth - pos);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x001CCD75 File Offset: 0x001CAF75
		public static void Switch(ref Texture2DArray texArr, int num1, int num2)
		{
			texArr.Switch(num1, num2);
			TextureArrayTools.Rewrite(ref texArr, texArr);
		}

		// Token: 0x06003C7D RID: 15485 RVA: 0x001CCD88 File Offset: 0x001CAF88
		public static void Switch(this Texture2DArray texArr, int num1, int num2)
		{
			if (num1 < 0 || num1 >= texArr.depth || num2 < 0 || num2 >= texArr.depth)
			{
				return;
			}
			Texture2D texture = texArr.GetTexture(num1, true);
			TextureArrayTools.CopyTexture(texArr, num2, texArr, num1);
			texArr.SetTexture(texture, num2, true);
		}

		// Token: 0x06003C7E RID: 15486 RVA: 0x001CCDCC File Offset: 0x001CAFCC
		public static void Clear(this Texture2DArray texArr, int chNum)
		{
			Texture2D src = new Texture2D(texArr.width, texArr.height, texArr.format, true, texArr.IsLinear());
			texArr.SetTexture(src, chNum, true);
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x001CCE04 File Offset: 0x001CB004
		public static void RemoveAt(ref Texture2DArray texArr, int num)
		{
			Texture2DArray newArr = TextureArrayTools.RemoveAt(texArr, num);
			TextureArrayTools.Rewrite(ref texArr, newArr);
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x001CCE24 File Offset: 0x001CB024
		public static Texture2DArray RemoveAt(Texture2DArray texArr, int num)
		{
			if (num >= texArr.depth || num < 0)
			{
				return texArr;
			}
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth - 1, texArr.format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			if (num != 0)
			{
				TextureArrayTools.CopyTextures(texArr, texture2DArray, num);
			}
			if (num != texArr.depth)
			{
				TextureArrayTools.CopyTextures(texArr, num + 1, texture2DArray, num, texture2DArray.depth - num);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C81 RID: 15489 RVA: 0x001CCEA4 File Offset: 0x001CB0A4
		public static void ChangeCount(ref Texture2DArray texArr, int newSize)
		{
			Texture2DArray newArr = TextureArrayTools.ChangeCount(texArr, newSize);
			TextureArrayTools.Rewrite(ref texArr, newArr);
		}

		// Token: 0x06003C82 RID: 15490 RVA: 0x001CCEC4 File Offset: 0x001CB0C4
		public static Texture2DArray ChangeCount(Texture2DArray texArr, int newSize)
		{
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, newSize, texArr.format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			int length = (newSize < texArr.depth) ? newSize : texArr.depth;
			TextureArrayTools.CopyTextures(texArr, texture2DArray, length);
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C83 RID: 15491 RVA: 0x001CCF20 File Offset: 0x001CB120
		public static Texture2DArray ResizedClone(this Texture2DArray texArr, int newWidth, int newHeight)
		{
			Texture2DArray texture2DArray = new Texture2DArray(newWidth, newHeight, texArr.depth, texArr.format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			for (int i = 0; i < texArr.depth; i++)
			{
				TextureArrayTools.CopyTexture(texArr, i, texture2DArray, i);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C84 RID: 15492 RVA: 0x001CCF78 File Offset: 0x001CB178
		public static Texture2DArray FormattedClone(this Texture2DArray texArr, TextureFormat format)
		{
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth, format, true, texArr.IsLinear());
			texture2DArray.name = texArr.name;
			int depth = texArr.depth;
			for (int i = 0; i < depth; i++)
			{
				TextureArrayTools.CopyTexture(texArr, i, texture2DArray, i);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x001CCFD8 File Offset: 0x001CB1D8
		public static Texture2DArray LinearClone(this Texture2DArray texArr, bool linear)
		{
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth, texArr.format, true, linear);
			texture2DArray.name = texArr.name;
			int depth = texArr.depth;
			for (int i = 0; i < depth; i++)
			{
				TextureArrayTools.CopyTexture(texArr, i, texture2DArray, i);
			}
			texture2DArray.Apply(false);
			return texture2DArray;
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x001CD038 File Offset: 0x001CB238
		public static Texture2DArray WritableClone(this Texture2DArray texArr)
		{
			Texture2DArray texture2DArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth, texArr.format, true, texArr.IsLinear());
			for (int i = 0; i < texArr.depth; i++)
			{
				TextureArrayTools.CopyTexture(texArr, i, texture2DArray, i);
			}
			texture2DArray.Apply(true);
			return texture2DArray;
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x001CD08C File Offset: 0x001CB28C
		public static int GetMipMapCount(this Texture2DArray texArr)
		{
			for (int i = 0; i < 100; i++)
			{
				try
				{
					texArr.GetPixels(0, i);
				}
				catch
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x001CD0CC File Offset: 0x001CB2CC
		public static bool IsReadWrite(this Texture2DArray texArr)
		{
			try
			{
				texArr.SetPixels(null, 0);
			}
			catch
			{
				return false;
			}
			return true;
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x000023FD File Offset: 0x000005FD
		public static void Rewrite(ref Texture2DArray texArr, Texture2DArray newArr)
		{
		}
	}
}
