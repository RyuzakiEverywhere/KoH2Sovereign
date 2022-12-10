using System;
using System.IO;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000489 RID: 1161
	[Serializable]
	public class Matrix : Matrix2<float>
	{
		// Token: 0x06003D72 RID: 15730 RVA: 0x001D43A4 File Offset: 0x001D25A4
		public float GetInterpolatedValue(Vector2 pos)
		{
			int num = Mathf.FloorToInt(pos.x);
			int num2 = Mathf.FloorToInt(pos.y);
			float num3 = pos.x - (float)num;
			float num4 = pos.y - (float)num2;
			float num5 = base[num, num2];
			float num6 = base[num + 1, num2];
			float num7 = num5 * (1f - num3) + num6 * num3;
			float num8 = base[num, num2 + 1];
			float num9 = base[num + 1, num2 + 1];
			float num10 = num8 * (1f - num3) + num9 * num3;
			return num7 * (1f - num4) + num10 * num4;
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x001D4434 File Offset: 0x001D2634
		public float GetAveragedValue(int x, int z, int steps)
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < steps; i++)
			{
				for (int j = 0; j < steps; j++)
				{
					if (x + i < this.rect.offset.x + this.rect.size.x && z + j < this.rect.offset.z + this.rect.size.z)
					{
						num += base[x + i, z + j];
						num2++;
					}
				}
			}
			return num / (float)num2;
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x001D44C4 File Offset: 0x001D26C4
		public Matrix()
		{
			this.array = new float[0];
			this.rect = new CoordRect(0, 0, 0, 0);
			this.count = 0;
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x001D44F0 File Offset: 0x001D26F0
		public Matrix(CoordRect rect, float[] array = null)
		{
			this.rect = rect;
			this.count = rect.size.x * rect.size.z;
			if (array != null && array.Length < this.count)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Array length: ",
					array.Length,
					" is lower then matrix capacity: ",
					this.count
				}));
			}
			if (array != null && array.Length >= this.count)
			{
				this.array = array;
				return;
			}
			this.array = new float[this.count];
		}

		// Token: 0x06003D76 RID: 15734 RVA: 0x001D4598 File Offset: 0x001D2798
		public Matrix(Coord offset, Coord size, float[] array = null)
		{
			this.rect = new CoordRect(offset, size);
			this.count = this.rect.size.x * this.rect.size.z;
			if (array != null && array.Length < this.count)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Array length: ",
					array.Length,
					" is lower then matrix capacity: ",
					this.count
				}));
			}
			if (array != null && array.Length >= this.count)
			{
				this.array = array;
				return;
			}
			this.array = new float[this.count];
		}

		// Token: 0x06003D77 RID: 15735 RVA: 0x001D4650 File Offset: 0x001D2850
		public Matrix(Texture2D texture)
		{
			this.rect = new CoordRect(0, 0, texture.width, texture.height);
			this.count = texture.width * texture.height;
			this.array = new float[this.count];
			this.FromTexture(texture);
		}

		// Token: 0x06003D78 RID: 15736 RVA: 0x001D46A8 File Offset: 0x001D28A8
		public override object Clone()
		{
			Matrix matrix = new Matrix(this.rect, null);
			matrix.rect = this.rect;
			matrix.count = this.count;
			if (matrix.array.Length != this.array.Length)
			{
				matrix.array = new float[this.array.Length];
			}
			Array.Copy(this.array, matrix.array, this.array.Length);
			return matrix;
		}

		// Token: 0x06003D79 RID: 15737 RVA: 0x001D471C File Offset: 0x001D291C
		public Matrix Copy(Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(this.rect, null);
			}
			result.rect = this.rect;
			result.count = this.count;
			if (result.array.Length != this.array.Length)
			{
				result.array = new float[this.array.Length];
			}
			Array.Copy(this.array, result.array, this.array.Length);
			return result;
		}

		// Token: 0x06003D7A RID: 15738 RVA: 0x001D4794 File Offset: 0x001D2994
		public bool[] InRect(CoordRect area = default(CoordRect))
		{
			Matrix2<bool> matrix = new Matrix2<bool>(this.rect, null);
			CoordRect coordRect = CoordRect.Intersect(this.rect, area);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					matrix[i, j] = true;
				}
			}
			return matrix.array;
		}

		// Token: 0x06003D7B RID: 15739 RVA: 0x001D4810 File Offset: 0x001D2A10
		public Texture2D ToTexture()
		{
			return this.ToTexture(this.rect.size.x, this.rect.size.z);
		}

		// Token: 0x06003D7C RID: 15740 RVA: 0x001D4838 File Offset: 0x001D2A38
		public Texture2D ToTexture(int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height);
			this.WriteTexture(texture2D);
			return texture2D;
		}

		// Token: 0x06003D7D RID: 15741 RVA: 0x001D4855 File Offset: 0x001D2A55
		public void WriteTexture(Texture2D texture)
		{
			this.WriteIntersectingTexture(texture, this.rect.offset.x, this.rect.offset.z, 0f, 1f);
		}

		// Token: 0x06003D7E RID: 15742 RVA: 0x001D4888 File Offset: 0x001D2A88
		public void WriteIntersectingTexture(Texture2D texture, int textureOffsetX, int textureOffsetZ, float rangeMin = 0f, float rangeMax = 1f)
		{
			CoordRect c = new CoordRect(textureOffsetX, textureOffsetZ, texture.width, texture.height);
			CoordRect coordRect = CoordRect.Intersect(this.rect, c);
			Color[] array = new Color[coordRect.size.x * coordRect.size.z];
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = base[i, j];
					num -= rangeMin;
					num /= rangeMax - rangeMin;
					array[(j - min.z) * (max.x - min.x) + (i - min.x)] = new Color(num, num, num);
				}
			}
			texture.SetPixels(coordRect.offset.x - textureOffsetX, coordRect.offset.z - textureOffsetZ, coordRect.size.x, coordRect.size.z, array);
			texture.Apply();
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x001D49A8 File Offset: 0x001D2BA8
		public void WriteTextureInterpolated(Texture2D texture, CoordRect textureRect, Matrix.WrapMode wrap = Matrix.WrapMode.Once, float rangeMin = 0f, float rangeMax = 1f)
		{
			float num = 1f * (float)textureRect.size.x / (float)texture.width;
			float num2 = 1f * (float)textureRect.size.z / (float)texture.height;
			Rect r = new Rect(0f, 0f, (float)texture.width, (float)texture.height);
			Rect r2 = new Rect((float)(textureRect.offset.x - this.rect.offset.x) / num, (float)(textureRect.offset.z - this.rect.offset.z) / num2, (float)this.rect.size.x / num, (float)this.rect.size.z / num2);
			Rect rect = CoordinatesExtensions.Intersect(r, r2);
			CoordRect coordRect = new CoordRect(Mathf.CeilToInt(rect.x), Mathf.CeilToInt(rect.y), Mathf.FloorToInt(rect.width), Mathf.FloorToInt(rect.height));
			Color[] array = new Color[coordRect.size.x * coordRect.size.z];
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float x = (float)i * num - (float)textureRect.offset.x + (float)(this.rect.offset.x * 2);
					float z = (float)j * num2 - (float)textureRect.offset.z + (float)(this.rect.offset.z * 2);
					float num3 = this.GetInterpolated(x, z, Matrix.WrapMode.Once);
					num3 -= rangeMin;
					num3 /= rangeMax - rangeMin;
					array[(j - min.z) * (max.x - min.x) + (i - min.x)] = new Color(num3, num3, num3);
				}
			}
			texture.SetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z, array);
			texture.Apply();
		}

		// Token: 0x06003D80 RID: 15744 RVA: 0x001D4C0C File Offset: 0x001D2E0C
		public void ReadArrayDirect(float[,] array)
		{
			Coord coord = new Coord(Mathf.Min(array.GetLength(0), this.rect.size.x), Mathf.Min(array.GetLength(1), this.rect.size.z));
			for (int i = 0; i < coord.x; i++)
			{
				for (int j = 0; j < coord.z; j++)
				{
					this.array[j * this.rect.size.x + i] = array[j, i];
				}
			}
		}

		// Token: 0x06003D81 RID: 15745 RVA: 0x001D4CA0 File Offset: 0x001D2EA0
		public void ReadArrayResize(float[,] array)
		{
			Coord coord = new Coord(array.GetLength(0), array.GetLength(1));
			if (coord.x == this.rect.size.x && coord.z == this.rect.size.z)
			{
				this.ReadArrayDirect(array);
				return;
			}
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					double num = 1.0 * (double)(i - this.rect.offset.x) / (double)this.rect.size.x;
					double num2 = 1.0 * (double)(j - this.rect.offset.z) / (double)this.rect.size.z;
					float interpolated = array.GetInterpolated((float)(num * (double)coord.x), (float)(num2 * (double)coord.z));
					this.array[j * this.rect.size.x + i] = interpolated;
				}
			}
		}

		// Token: 0x06003D82 RID: 15746 RVA: 0x001D4DF0 File Offset: 0x001D2FF0
		public void Fill(float[,] array, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = array[j - arrayRect.offset.z, i - arrayRect.offset.x];
				}
			}
		}

		// Token: 0x06003D83 RID: 15747 RVA: 0x001D4E74 File Offset: 0x001D3074
		public void Pour(float[,] array, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					array[j - arrayRect.offset.z, i - arrayRect.offset.x] = base[i, j];
				}
			}
		}

		// Token: 0x06003D84 RID: 15748 RVA: 0x001D4EF8 File Offset: 0x001D30F8
		public void Pour(float[,,] array, int channel, CoordRect arrayRect)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, arrayRect);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					array[j - arrayRect.offset.z, i - arrayRect.offset.x, channel] = base[i, j];
				}
			}
		}

		// Token: 0x06003D85 RID: 15749 RVA: 0x001D4F80 File Offset: 0x001D3180
		public float[,] ReadHeighmap(TerrainData data, float height = 1f)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
			float[,] heights = data.GetHeights(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = heights[j - min.z, i - min.x] * height;
				}
			}
			base.RemoveBorders(coordRect);
			return heights;
		}

		// Token: 0x06003D86 RID: 15750 RVA: 0x001D5050 File Offset: 0x001D3250
		public void WriteHeightmap(TerrainData data, float[,] array = null, float brushFallof = 0.5f, bool delayLod = false)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.heightmapResolution, data.heightmapResolution));
			if (array == null || array.Length != coordRect.size.x * coordRect.size.z)
			{
				array = new float[coordRect.size.z, coordRect.size.x];
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = this.Fallof(i, j, brushFallof);
					if (!Mathf.Approximately(num, 0f))
					{
						array[j - min.z, i - min.x] = base[i, j] * num + array[j - min.z, i - min.x] * (1f - num);
					}
				}
			}
			if (delayLod)
			{
				data.SetHeightsDelayLOD(coordRect.offset.x, coordRect.offset.z, array);
				return;
			}
			data.SetHeights(coordRect.offset.x, coordRect.offset.z, array);
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x001D519C File Offset: 0x001D339C
		public float[,,] ReadSplatmap(TerrainData data, int channel, float[,,] array = null)
		{
			CoordRect coordRect = CoordRect.Intersect(this.rect, new CoordRect(0, 0, data.alphamapResolution, data.alphamapResolution));
			if (array == null)
			{
				array = data.GetAlphamaps(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					base[i, j] = array[j - min.z, i - min.x, channel];
				}
			}
			base.RemoveBorders(coordRect);
			return array;
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x001D5268 File Offset: 0x001D3468
		public static void AddSplatmaps(TerrainData data, Matrix[] matrices, int[] channels, float[] opacity, float[,,] array = null, float brushFallof = 0.5f)
		{
			int alphamapLayers = data.alphamapLayers;
			bool[] array2 = new bool[alphamapLayers];
			for (int i = 0; i < channels.Length; i++)
			{
				array2[channels[i]] = true;
			}
			float[] array3 = new float[alphamapLayers];
			Coord size = new Coord(data.alphamapResolution, data.alphamapResolution);
			CoordRect coordRect = CoordRect.Intersect(new CoordRect(new Coord(0, 0), size), matrices[0].rect);
			if (array == null)
			{
				array = data.GetAlphamaps(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int j = min.x; j < max.x; j++)
			{
				for (int k = min.z; k < max.z; k++)
				{
					float num = matrices[0].Fallof(j, k, brushFallof);
					if (!Mathf.Approximately(num, 0f))
					{
						for (int l = 0; l < alphamapLayers; l++)
						{
							array3[l] = array[k - min.z, j - min.x, l];
						}
						for (int m = 0; m < matrices.Length; m++)
						{
							matrices[m][j, k] = Mathf.Max(0f, matrices[m][j, k] - array3[channels[m]]);
						}
						for (int n = 0; n < matrices.Length; n++)
						{
							Matrix matrix = matrices[n];
							int num2 = j;
							int num3 = k;
							matrix[num2, num3] *= num * opacity[n];
						}
						float num4 = 0f;
						for (int num5 = 0; num5 < matrices.Length; num5++)
						{
							num4 += matrices[num5][j, k];
						}
						if (num4 > 1f)
						{
							foreach (Matrix matrix in matrices)
							{
								int num3 = j;
								int num2 = k;
								matrix[num3, num2] /= num4;
							}
							num4 = 1f;
						}
						float num7 = 1f - num4;
						for (int num8 = 0; num8 < alphamapLayers; num8++)
						{
							array3[num8] *= num7;
						}
						for (int num9 = 0; num9 < matrices.Length; num9++)
						{
							array3[channels[num9]] += matrices[num9][j, k];
						}
						for (int num10 = 0; num10 < alphamapLayers; num10++)
						{
							array[k - min.z, j - min.x, num10] = array3[num10];
						}
					}
				}
			}
			data.SetAlphamaps(coordRect.offset.x, coordRect.offset.z, array);
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x001D554C File Offset: 0x001D374C
		public void FillTexture(Texture2D texture = null, Color[] colors = null, float rangeMin = 0f, float rangeMax = 1f, bool resizeTexture = false)
		{
			if (texture == null)
			{
				texture = new Texture2D(this.rect.size.x, this.rect.size.z);
			}
			if (resizeTexture)
			{
				texture.Resize(this.rect.size.x, this.rect.size.z);
			}
			Coord size = new Coord(texture.width, texture.height);
			CoordRect coordRect = CoordRect.Intersect(new CoordRect(new Coord(0, 0), size), this.rect);
			if (colors == null || colors.Length != coordRect.size.x * coordRect.size.z)
			{
				colors = new Color[coordRect.size.x * coordRect.size.z];
			}
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = (base[i, j] - rangeMin) / (rangeMax - rangeMin) * 256f;
					int num2 = (int)num;
					float num3 = num - (float)num2;
					float num4 = (float)num2 / 256f;
					float num5 = (float)(num2 + 1) / 256f;
					int num6 = i - min.x;
					int num7 = j - min.z;
					colors[num7 * (max.x - min.x) + num6] = new Color(num4, (num3 > 0.333f) ? num5 : num4, (num3 > 0.666f) ? num5 : num4);
				}
			}
			texture.SetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z, colors);
			texture.Apply();
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x001D5734 File Offset: 0x001D3934
		public void FromTexture(Texture2D texture)
		{
			CoordRect coordRect = CoordRect.Intersect(new CoordRect(0, 0, texture.width, texture.height), this.rect);
			Color[] pixels = texture.GetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = i - min.x;
					int num2 = j - min.z;
					Color color = pixels[num2 * (max.x - min.x) + num];
					base[i, j] = (color.r + color.g + color.b) / 3f;
				}
			}
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x001D5834 File Offset: 0x001D3A34
		public void FromTextureAlpha(Texture2D texture)
		{
			CoordRect coordRect = CoordRect.Intersect(new CoordRect(0, 0, texture.width, texture.height), this.rect);
			Color[] pixels = texture.GetPixels(coordRect.offset.x, coordRect.offset.z, coordRect.size.x, coordRect.size.z);
			Coord min = coordRect.Min;
			Coord max = coordRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = i - min.x;
					int num2 = j - min.z;
					Color color = pixels[num2 * (max.x - min.x) + num];
					base[i, j] = (color.r + color.g + color.b + color.a) / 4f;
				}
			}
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x001D5940 File Offset: 0x001D3B40
		public void FromTextureTiled(Texture2D texture)
		{
			Color[] pixels = texture.GetPixels();
			int width = texture.width;
			int height = texture.height;
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = i % width;
					if (num < 0)
					{
						num += width;
					}
					int num2 = j % height;
					if (num2 < 0)
					{
						num2 += height;
					}
					Color color = pixels[num2 * width + num];
					base[i, j] = (color.r + color.g + color.b) / 3f;
				}
			}
		}

		// Token: 0x06003D8D RID: 15757 RVA: 0x001D5A0C File Offset: 0x001D3C0C
		public Texture2D SimpleToTexture(Texture2D texture = null, Color[] colors = null, float rangeMin = 0f, float rangeMax = 1f, string savePath = null)
		{
			if (texture == null)
			{
				texture = new Texture2D(this.rect.size.x, this.rect.size.z);
			}
			if (texture.width != this.rect.size.x || texture.height != this.rect.size.z)
			{
				texture.Resize(this.rect.size.x, this.rect.size.z);
			}
			if (colors == null || colors.Length != this.rect.size.x * this.rect.size.z)
			{
				colors = new Color[this.rect.size.x * this.rect.size.z];
			}
			for (int i = 0; i < this.count; i++)
			{
				float num = this.array[i];
				num -= rangeMin;
				num /= rangeMax - rangeMin;
				colors[i] = new Color(num, num, num);
			}
			texture.SetPixels(colors);
			texture.Apply();
			return texture;
		}

		// Token: 0x06003D8E RID: 15758 RVA: 0x001D5B34 File Offset: 0x001D3D34
		public void SimpleFromTexture(Texture2D texture)
		{
			base.ChangeRect(new CoordRect(this.rect.offset.x, this.rect.offset.z, texture.width, texture.height), false);
			Color[] pixels = texture.GetPixels();
			for (int i = 0; i < this.count; i++)
			{
				Color color = pixels[i];
				this.array[i] = (color.r + color.g + color.b) / 3f;
			}
		}

		// Token: 0x06003D8F RID: 15759 RVA: 0x001D5BBC File Offset: 0x001D3DBC
		public void ImportRaw(string path)
		{
			FileStream fileStream = new FileInfo(path).Open(FileMode.Open, FileAccess.Read);
			int num = (int)Mathf.Sqrt((float)(fileStream.Length / 2L));
			byte[] array = new byte[num * num * 2];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			base.ChangeRect(new CoordRect(0, 0, num, num), false);
			int num2 = 0;
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = max.z - 1; i >= min.z; i--)
			{
				for (int j = min.x; j < max.x; j++)
				{
					base[j, i] = ((float)array[num2 + 1] * 256f + (float)array[num2]) / 65535f;
					num2 += 2;
				}
			}
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x001D5C8C File Offset: 0x001D3E8C
		public void Replicate(Matrix source, bool tile = false)
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					if (source.rect.CheckInRange(i, j))
					{
						base[i, j] = source[i, j];
					}
					else if (tile)
					{
						int num = i - source.rect.offset.x;
						int num2 = j - source.rect.offset.z;
						int num3 = num % source.rect.size.x;
						int num4 = num2 % source.rect.size.z;
						if (num3 < 0)
						{
							num3 += source.rect.size.x;
						}
						if (num4 < 0)
						{
							num4 += source.rect.size.z;
						}
						int x = num3 + source.rect.offset.x;
						int z = num4 + source.rect.offset.z;
						base[i, j] = source[x, z];
					}
				}
			}
		}

		// Token: 0x06003D91 RID: 15761 RVA: 0x001D5DD4 File Offset: 0x001D3FD4
		public float GetArea(int x, int z, int range)
		{
			if (range == 0)
			{
				if (x < this.rect.offset.x)
				{
					x = this.rect.offset.x;
				}
				if (x >= this.rect.offset.x + this.rect.size.x)
				{
					x = this.rect.offset.x + this.rect.size.x - 1;
				}
				if (z < this.rect.offset.z)
				{
					z = this.rect.offset.z;
				}
				if (z >= this.rect.offset.z + this.rect.size.z)
				{
					z = this.rect.offset.z + this.rect.size.z - 1;
				}
				return this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x];
			}
			float num = 0f;
			int num2 = 0;
			for (int i = x - range; i <= x + range; i++)
			{
				if (i >= this.rect.offset.x && i < this.rect.offset.x + this.rect.size.x)
				{
					for (int j = z - range; j <= z + range; j++)
					{
						if (j >= this.rect.offset.z && j < this.rect.offset.z + this.rect.size.z)
						{
							num += this.array[(j - this.rect.offset.z) * this.rect.size.x + i - this.rect.offset.x];
							num2++;
						}
					}
				}
			}
			return num / (float)num2;
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x001D5FF0 File Offset: 0x001D41F0
		public float GetInterpolated(float x, float z, Matrix.WrapMode wrap = Matrix.WrapMode.Once)
		{
			if (wrap == Matrix.WrapMode.Once && (x < (float)this.rect.offset.x || x >= (float)(this.rect.offset.x + this.rect.size.x) || z < (float)this.rect.offset.z || z >= (float)(this.rect.offset.z + this.rect.size.z)))
			{
				return 0f;
			}
			int num = (int)x;
			if (x < 0f)
			{
				num--;
			}
			int num2 = num + 1;
			int num3 = (int)z;
			if (z < 0f)
			{
				num3--;
			}
			int num4 = num3 + 1;
			int num5 = num - this.rect.offset.x;
			int num6 = num2 - this.rect.offset.x;
			int num7 = num3 - this.rect.offset.z;
			int num8 = num4 - this.rect.offset.z;
			if (wrap == Matrix.WrapMode.Clamp || wrap == Matrix.WrapMode.Once)
			{
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num5 >= this.rect.size.x)
				{
					num5 = this.rect.size.x - 1;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num6 >= this.rect.size.x)
				{
					num6 = this.rect.size.x - 1;
				}
				if (num7 < 0)
				{
					num7 = 0;
				}
				if (num7 >= this.rect.size.z)
				{
					num7 = this.rect.size.z - 1;
				}
				if (num8 < 0)
				{
					num8 = 0;
				}
				if (num8 >= this.rect.size.z)
				{
					num8 = this.rect.size.z - 1;
				}
			}
			else if (wrap == Matrix.WrapMode.Tile)
			{
				num5 %= this.rect.size.x;
				if (num5 < 0)
				{
					num5 = this.rect.size.x + num5;
				}
				num7 %= this.rect.size.z;
				if (num7 < 0)
				{
					num7 = this.rect.size.z + num7;
				}
				num6 %= this.rect.size.x;
				if (num6 < 0)
				{
					num6 = this.rect.size.x + num6;
				}
				num8 %= this.rect.size.z;
				if (num8 < 0)
				{
					num8 = this.rect.size.z + num8;
				}
			}
			else if (wrap == Matrix.WrapMode.PingPong)
			{
				num5 %= this.rect.size.x * 2;
				if (num5 < 0)
				{
					num5 = this.rect.size.x * 2 + num5;
				}
				if (num5 >= this.rect.size.x)
				{
					num5 = this.rect.size.x * 2 - num5 - 1;
				}
				num7 %= this.rect.size.z * 2;
				if (num7 < 0)
				{
					num7 = this.rect.size.z * 2 + num7;
				}
				if (num7 >= this.rect.size.z)
				{
					num7 = this.rect.size.z * 2 - num7 - 1;
				}
				num6 %= this.rect.size.x * 2;
				if (num6 < 0)
				{
					num6 = this.rect.size.x * 2 + num6;
				}
				if (num6 >= this.rect.size.x)
				{
					num6 = this.rect.size.x * 2 - num6 - 1;
				}
				num8 %= this.rect.size.z * 2;
				if (num8 < 0)
				{
					num8 = this.rect.size.z * 2 + num8;
				}
				if (num8 >= this.rect.size.z)
				{
					num8 = this.rect.size.z * 2 - num8 - 1;
				}
			}
			float num9 = this.array[num7 * this.rect.size.x + num5];
			float num10 = this.array[num7 * this.rect.size.x + num6];
			float num11 = this.array[num8 * this.rect.size.x + num5];
			float num12 = this.array[num8 * this.rect.size.x + num6];
			float num13 = x - (float)num;
			float num14 = z - (float)num3;
			float num15 = num9 * (1f - num13) + num10 * num13;
			float num16 = num11 * (1f - num13) + num12 * num13;
			return num15 * (1f - num14) + num16 * num14;
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x001D64B4 File Offset: 0x001D46B4
		public Matrix Resize(CoordRect newRect, Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(newRect, null);
			}
			else
			{
				result.ChangeRect(newRect, false);
			}
			Coord min = result.rect.Min;
			Coord max = result.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float x = 1f * (float)(i - result.rect.offset.x) / (float)result.rect.size.x * (float)this.rect.size.x + (float)this.rect.offset.x;
					float z = 1f * (float)(j - result.rect.offset.z) / (float)result.rect.size.z * (float)this.rect.size.z + (float)this.rect.offset.z;
					result[i, j] = this.GetInterpolated(x, z, Matrix.WrapMode.Once);
				}
			}
			return result;
		}

		// Token: 0x06003D94 RID: 15764 RVA: 0x001D65DD File Offset: 0x001D47DD
		public Matrix Downscale(int factor, Matrix result = null)
		{
			return this.Resize(this.rect / factor, result);
		}

		// Token: 0x06003D95 RID: 15765 RVA: 0x001D65F2 File Offset: 0x001D47F2
		public Matrix Upscale(int factor, Matrix result = null)
		{
			return this.Resize(this.rect * factor, result);
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x001D6608 File Offset: 0x001D4808
		public Matrix BlurredUpscale(int factor)
		{
			Matrix matrix = new Matrix(this.rect, new float[this.count * factor]);
			Matrix matrix2 = new Matrix(this.rect, new float[this.count * factor]);
			matrix.Fill(this, false);
			int num = Mathf.RoundToInt(Mathf.Sqrt((float)factor));
			for (int i = 0; i < num; i++)
			{
				matrix.Resize(matrix.rect * 2, matrix2);
				matrix.ChangeRect(matrix2.rect, false);
				matrix.Fill(matrix2, false);
				matrix.Blur(null, 0.5f, false, false, true, true, null);
			}
			return matrix;
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x001D66A4 File Offset: 0x001D48A4
		public Matrix OutdatedResize(CoordRect newRect, float smoothness = 1f, Matrix result = null)
		{
			int num = newRect.size.x / this.rect.size.x;
			int num2 = this.rect.size.x / newRect.size.x;
			if (num > 1 && !newRect.Divisible((float)num))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Matrix rect ",
					this.rect,
					" could not be upscaled to ",
					newRect,
					" with factor ",
					num
				}));
			}
			if (num2 > 1 && !this.rect.Divisible((float)num2))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Matrix rect ",
					this.rect,
					" could not be downscaled to ",
					newRect,
					" with factor ",
					num2
				}));
			}
			if (num > 1)
			{
				result = this.OutdatedUpscale(num, result);
			}
			if (num2 > 1)
			{
				result = this.OutdatedDownscale(num2, smoothness, result);
			}
			if (num <= 1 && num2 <= 1)
			{
				return this.Copy(result);
			}
			return result;
		}

		// Token: 0x06003D98 RID: 15768 RVA: 0x001D67CC File Offset: 0x001D49CC
		public Matrix OutdatedUpscale(int factor, Matrix result = null)
		{
			if (result == null)
			{
				result = new Matrix(this.rect * factor, null);
			}
			result.ChangeRect(this.rect * factor, false);
			if (factor == 1)
			{
				return this.Copy(result);
			}
			Coord min = this.rect.Min;
			Coord coord = this.rect.Max - 1;
			float num = 1f / (float)factor;
			for (int i = min.x; i < coord.x; i++)
			{
				for (int j = min.z; j < coord.z; j++)
				{
					float a = base[i, j];
					float a2 = base[i + 1, j];
					float b = base[i, j + 1];
					float b2 = base[i + 1, j + 1];
					for (int k = 0; k < factor; k++)
					{
						for (int l = 0; l < factor; l++)
						{
							float t = (float)k * num;
							float t2 = (float)l * num;
							float a3 = Mathf.Lerp(a, b, t2);
							float b3 = Mathf.Lerp(a2, b2, t2);
							result[i * factor + k, j * factor + l] = Mathf.Lerp(a3, b3, t);
						}
					}
				}
			}
			result.RemoveBorders(0, 0, factor + 1, factor + 1);
			return result;
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x001D691C File Offset: 0x001D4B1C
		public float OutdatedGetInterpolated(float x, float z)
		{
			int num = (int)x;
			int num2 = (int)(x + 1f);
			if (num2 >= this.rect.offset.x + this.rect.size.x)
			{
				num2 = this.rect.offset.x + this.rect.size.x - 1;
			}
			int num3 = (int)z;
			int num4 = (int)(z + 1f);
			if (num4 >= this.rect.offset.z + this.rect.size.z)
			{
				num4 = this.rect.offset.z + this.rect.size.z - 1;
			}
			float num5 = x - (float)num;
			float num6 = z - (float)num3;
			int num7 = (num3 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x;
			float num8 = this.array[num7];
			float num9 = this.array[(num3 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
			float num10 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num - this.rect.offset.x];
			float num11 = this.array[(num4 - this.rect.offset.z) * this.rect.size.x + num2 - this.rect.offset.x];
			float num12 = num8 * (1f - num5) + num9 * num5;
			float num13 = num10 * (1f - num5) + num11 * num5;
			return num12 * (1f - num6) + num13 * num6;
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x001D6B0C File Offset: 0x001D4D0C
		public Matrix TestResize(CoordRect newRect)
		{
			Matrix matrix = new Matrix(newRect, null);
			Coord min = matrix.rect.Min;
			Coord max = matrix.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float x = 1f * (float)(i - matrix.rect.offset.x) / (float)matrix.rect.size.x * (float)this.rect.size.x + (float)this.rect.offset.x;
					float z = 1f * (float)(j - matrix.rect.offset.z) / (float)matrix.rect.size.z * (float)this.rect.size.z + (float)this.rect.offset.z;
					matrix[i, j] = this.OutdatedGetInterpolated(x, z);
				}
			}
			return matrix;
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x001D6C2C File Offset: 0x001D4E2C
		public Matrix OutdatedDownscale(int factor = 2, float smoothness = 1f, Matrix result = null)
		{
			if (!this.rect.Divisible((float)factor))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Matrix rect ",
					this.rect,
					" could not be downscaled with factor ",
					factor
				}));
			}
			if (result == null)
			{
				result = new Matrix(this.rect / factor, null);
			}
			result.ChangeRect(this.rect / factor, false);
			if (factor == 1)
			{
				return this.Copy(result);
			}
			Coord min = this.rect.Min;
			Coord min2 = result.rect.Min;
			Coord max = result.rect.Max;
			if (smoothness < 0.0001f)
			{
				for (int i = min2.x; i < max.x; i++)
				{
					for (int j = min2.z; j < max.z; j++)
					{
						int x = (i - min2.x) * factor + min.x;
						int z = (j - min2.z) * factor + min.z;
						result[i, j] = base[x, z];
					}
				}
			}
			else
			{
				for (int k = min2.x; k < max.x; k++)
				{
					for (int l = min2.z; l < max.z; l++)
					{
						int num = (k - min2.x) * factor + min.x;
						int num2 = (l - min2.z) * factor + min.z;
						float num3 = 0f;
						for (int m = num; m < num + factor; m++)
						{
							for (int n = num2; n < num2 + factor; n++)
							{
								num3 += base[m, n];
							}
						}
						result[k, l] = num3 / (float)(factor * factor) * smoothness + base[num, num2] * (1f - smoothness);
					}
				}
			}
			return result;
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x001D6E20 File Offset: 0x001D5020
		public void Spread(float strength = 0.5f, int iterations = 4, Matrix copy = null)
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] = Mathf.Clamp(this.array[i], -1f, 1f);
			}
			if (copy == null)
			{
				copy = this.Copy(null);
			}
			else
			{
				for (int j = 0; j < this.count; j++)
				{
					copy.array[j] = this.array[j];
				}
			}
			for (int k = 0; k < iterations; k++)
			{
				for (int l = min.x; l < max.x; l++)
				{
					float num = base[l, min.z];
					base.SetPos(l, min.z);
					for (int m = min.z + 1; m < max.z; m++)
					{
						num = (num + this.array[this.pos]) / 2f;
						this.array[this.pos] = num;
						this.pos += this.rect.size.x;
					}
					num = base[l, max.z - 1];
					base.SetPos(l, max.z - 1);
					for (int n = max.z - 2; n >= min.z; n--)
					{
						num = (num + this.array[this.pos]) / 2f;
						this.array[this.pos] = num;
						this.pos -= this.rect.size.x;
					}
				}
				for (int num2 = min.z; num2 < max.z; num2++)
				{
					float num = base[min.x, num2];
					base.SetPos(min.x, num2);
					for (int num3 = min.x + 1; num3 < max.x; num3++)
					{
						num = (num + this.array[this.pos]) / 2f;
						this.array[this.pos] = num;
						this.pos++;
					}
					num = base[max.x - 1, num2];
					base.SetPos(max.x - 1, num2);
					for (int num4 = max.x - 2; num4 >= min.x; num4--)
					{
						num = (num + this.array[this.pos]) / 2f;
						this.array[this.pos] = num;
						this.pos--;
					}
				}
			}
			for (int num5 = 0; num5 < this.count; num5++)
			{
				this.array[num5] = copy.array[num5] + this.array[num5] * 2f * strength;
			}
			float num6 = Mathf.Sqrt((float)iterations);
			for (int num7 = 0; num7 < this.count; num7++)
			{
				this.array[num7] /= num6;
			}
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x001D7154 File Offset: 0x001D5354
		public void Spread(Func<float, float, float> spreadFn = null, int iterations = 4)
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = 0; i < iterations; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					float num = base[j, min.z];
					base.SetPos(j, min.z);
					for (int k = min.z + 1; k < max.z; k++)
					{
						num = spreadFn(num, this.array[this.pos]);
						this.array[this.pos] = num;
						this.pos += this.rect.size.x;
					}
					num = base[j, max.z - 1];
					base.SetPos(j, max.z - 1);
					for (int l = max.z - 2; l >= min.z; l--)
					{
						num = spreadFn(num, this.array[this.pos]);
						this.array[this.pos] = num;
						this.pos -= this.rect.size.x;
					}
				}
				for (int m = min.z; m < max.z; m++)
				{
					float num = base[min.x, m];
					base.SetPos(min.x, m);
					for (int n = min.x + 1; n < max.x; n++)
					{
						num = spreadFn(num, this.array[this.pos]);
						this.array[this.pos] = num;
						this.pos++;
					}
					num = base[max.x - 1, m];
					base.SetPos(max.x - 1, m);
					for (int num2 = max.x - 2; num2 >= min.x; num2--)
					{
						num = spreadFn(num, this.array[this.pos]);
						this.array[this.pos] = num;
						this.pos--;
					}
				}
			}
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x001D73A0 File Offset: 0x001D55A0
		public void SimpleBlur(int iterations, float strength)
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = 0; i < iterations; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					float num = base[min.x, j];
					for (int k = min.x + 1; k < max.x - 1; k++)
					{
						int num2 = (j - this.rect.offset.z) * this.rect.size.x + k - this.rect.offset.x;
						float num3 = this.array[num2];
						float num4 = this.array[num2 + 1];
						float num5 = (num + num4) / 2f * strength + num3 * (1f - strength);
						this.array[num2] = num5;
						num = num5;
					}
				}
				for (int l = min.x; l < max.x; l++)
				{
					float num6 = base[l, min.z];
					for (int m = min.z + 1; m < max.z - 1; m++)
					{
						int num7 = (m - this.rect.offset.z) * this.rect.size.x + l - this.rect.offset.x;
						float num8 = this.array[num7];
						float num9 = this.array[num7 + this.rect.size.x];
						float num10 = (num6 + num9) / 2f * strength + num8 * (1f - strength);
						this.array[num7] = num10;
						num6 = num10;
					}
				}
			}
		}

		// Token: 0x06003D9F RID: 15775 RVA: 0x001D7580 File Offset: 0x001D5780
		public void Blur(Func<float, float, float, float> blurFn = null, float intensity = 0.666f, bool additive = false, bool takemax = false, bool horizontal = true, bool vertical = true, Matrix reference = null)
		{
			if (reference == null)
			{
				reference = this;
			}
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			if (horizontal)
			{
				for (int i = min.z; i < max.z; i++)
				{
					int num = (i - this.rect.offset.z) * this.rect.size.x + min.x - this.rect.offset.x;
					float num2 = reference[min.x, i];
					float num3 = num2;
					float num4 = num2;
					for (int j = min.x; j < max.x; j++)
					{
						num2 = num3;
						num3 = num4;
						if (j < max.x - 1)
						{
							num4 = reference.array[num + 1];
						}
						float num5;
						if (blurFn == null)
						{
							num5 = (num2 + num4) / 2f;
						}
						else
						{
							num5 = blurFn(num2, num3, num4);
						}
						num5 = num3 * (1f - intensity) + num5 * intensity;
						if (additive)
						{
							this.array[num] += num5;
						}
						else
						{
							this.array[num] = num5;
						}
						num++;
					}
				}
			}
			if (vertical)
			{
				for (int k = min.x; k < max.x; k++)
				{
					int num6 = (min.z - this.rect.offset.z) * this.rect.size.x + k - this.rect.offset.x;
					float num7 = reference[k, min.z];
					float num8 = num7;
					for (int l = min.z; l < max.z; l++)
					{
						float num9 = num8;
						num8 = num7;
						if (l < max.z - 1)
						{
							num7 = reference.array[num6 + this.rect.size.x];
						}
						float num10;
						if (blurFn == null)
						{
							num10 = (num9 + num7) / 2f;
						}
						else
						{
							num10 = blurFn(num9, num8, num7);
						}
						num10 = num8 * (1f - intensity) + num10 * intensity;
						if (additive)
						{
							this.array[num6] += num10;
						}
						else if (takemax)
						{
							if (num10 > this.array[num6])
							{
								this.array[num6] = num10;
							}
						}
						else
						{
							this.array[num6] = num10;
						}
						num6 += this.rect.size.x;
					}
				}
			}
		}

		// Token: 0x06003DA0 RID: 15776 RVA: 0x001D7818 File Offset: 0x001D5A18
		public void LossBlur(int step = 2, bool horizontal = true, bool vertical = true, Matrix reference = null)
		{
			if (reference == null)
			{
				reference = this;
			}
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			int num = step + step / 2;
			if (horizontal)
			{
				for (int i = min.z; i < max.z; i++)
				{
					base.SetPos(min.x, i);
					float num2 = 0f;
					int num3 = 0;
					float num4 = this.array[this.pos];
					float num5 = this.array[this.pos];
					for (int j = min.x; j < max.x + num; j++)
					{
						if (j < max.x)
						{
							num2 += reference.array[this.pos];
						}
						num3++;
						if (j % step == 0)
						{
							num5 = num4;
							if (j < max.x)
							{
								num4 = num2 / (float)num3;
							}
							num2 = 0f;
							num3 = 0;
						}
						if (j - num >= min.x)
						{
							float num6 = 1f * (float)(j % step) / (float)step;
							if (num6 < 0f)
							{
								num6 += 1f;
							}
							this.array[this.pos - num] = num4 * num6 + num5 * (1f - num6);
						}
						this.pos++;
					}
				}
			}
			if (vertical)
			{
				for (int k = min.x; k < max.x; k++)
				{
					base.SetPos(k, min.z);
					float num7 = 0f;
					int num8 = 0;
					float num9 = this.array[this.pos];
					float num10 = this.array[this.pos];
					for (int l = min.z; l < max.z + num; l++)
					{
						if (l < max.z)
						{
							num7 += reference.array[this.pos];
						}
						num8++;
						if (l % step == 0)
						{
							num10 = num9;
							if (l < max.z)
							{
								num9 = num7 / (float)num8;
							}
							num7 = 0f;
							num8 = 0;
						}
						if (l - num >= min.z)
						{
							float num11 = 1f * (float)(l % step) / (float)step;
							if (num11 < 0f)
							{
								num11 += 1f;
							}
							this.array[this.pos - num * this.rect.size.x] = num9 * num11 + num10 * (1f - num11);
						}
						this.pos += this.rect.size.x;
					}
				}
			}
		}

		// Token: 0x06003DA1 RID: 15777 RVA: 0x001D7AB0 File Offset: 0x001D5CB0
		public static void BlendLayers(Matrix[] matrices, float[] opacity = null)
		{
			int num = -1;
			for (int i = 0; i < matrices.Length; i++)
			{
				if (matrices[i] != null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				Debug.LogError("No matrices were found to blend " + matrices.Length);
				return;
			}
			CoordRect rect = matrices[num].rect;
			int count = rect.Count;
			for (int j = 0; j < count; j++)
			{
				float num2 = 0f;
				for (int k = matrices.Length - 1; k >= 0; k--)
				{
					if (matrices[k] != null)
					{
						float num3 = matrices[k].array[j];
						if (opacity != null)
						{
							num3 *= opacity[k];
						}
						float num4 = num2 + num3 - 1f;
						if (num4 < 0f)
						{
							num4 = 0f;
						}
						if (num4 > 1f)
						{
							num4 = 1f;
						}
						matrices[k].array[j] = num3 - num4;
						num2 += num3 - num4;
					}
				}
			}
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x001D7B9C File Offset: 0x001D5D9C
		public static void NormalizeLayers(Matrix[] matrices, float[] opacity)
		{
			int num = -1;
			for (int i = 0; i < matrices.Length; i++)
			{
				if (matrices[i] != null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				Debug.LogError("No matrices were found to blend " + matrices.Length);
				return;
			}
			CoordRect rect = matrices[num].rect;
			int count = rect.Count;
			for (int j = 0; j < count; j++)
			{
				for (int k = 0; k < matrices.Length; k++)
				{
					matrices[k].array[j] *= opacity[k];
				}
				float num2 = 0f;
				for (int l = 0; l < matrices.Length; l++)
				{
					num2 += matrices[l].array[j];
				}
				if (num2 > 1f)
				{
					for (int m = 0; m < matrices.Length; m++)
					{
						matrices[m].array[j] /= num2;
					}
				}
			}
		}

		// Token: 0x06003DA3 RID: 15779 RVA: 0x001D7C8C File Offset: 0x001D5E8C
		public float Fallof(int x, int z, float fallof)
		{
			if (fallof < 0f)
			{
				return 1f;
			}
			float num = (float)this.rect.size.x / 2f - 1f;
			float num2 = ((float)x - ((float)this.rect.offset.x + num)) / num;
			float num3 = (float)this.rect.size.z / 2f - 1f;
			float num4 = ((float)z - ((float)this.rect.offset.z + num3)) / num3;
			float num5 = Mathf.Sqrt(num2 * num2 + num4 * num4);
			float num6 = Mathf.Clamp01((1f - num5) / (1f - fallof));
			return 3f * num6 * num6 - 2f * num6 * num6 * num6;
		}

		// Token: 0x06003DA4 RID: 15780 RVA: 0x001D7D50 File Offset: 0x001D5F50
		public void FillEmpty()
		{
			Coord min = this.rect.Min;
			Coord max = this.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				float num = 0f;
				for (int j = min.z; j < max.z; j++)
				{
					float num2 = this.array[(j - this.rect.offset.z) * this.rect.size.x + i - this.rect.offset.x];
					if (num2 > 0.0001f)
					{
						num = num2;
					}
					else if (num > 0.0001f)
					{
						this.array[(j - this.rect.offset.z) * this.rect.size.x + i - this.rect.offset.x] = num;
					}
				}
			}
			for (int k = min.z; k < max.z; k++)
			{
				float num = 0f;
				for (int l = min.x; l < max.x; l++)
				{
					float num3 = this.array[(k - this.rect.offset.z) * this.rect.size.x + l - this.rect.offset.x];
					if (num3 > 0.0001f)
					{
						num = num3;
					}
					else if (num > 0.0001f)
					{
						this.array[(k - this.rect.offset.z) * this.rect.size.x + l - this.rect.offset.x] = num;
					}
				}
			}
			for (int m = min.x; m < max.x; m++)
			{
				float num = 0f;
				for (int n = max.z - 1; n > min.z; n--)
				{
					float num4 = this.array[(n - this.rect.offset.z) * this.rect.size.x + m - this.rect.offset.x];
					if (num4 > 0.0001f)
					{
						num = num4;
					}
					else if (num > 0.0001f)
					{
						this.array[(n - this.rect.offset.z) * this.rect.size.x + m - this.rect.offset.x] = num;
					}
				}
			}
			for (int num5 = min.z; num5 < max.z; num5++)
			{
				float num = 0f;
				for (int num6 = max.x - 1; num6 >= min.x; num6--)
				{
					float num7 = this.array[(num5 - this.rect.offset.z) * this.rect.size.x + num6 - this.rect.offset.x];
					if (num7 > 0.0001f)
					{
						num = num7;
					}
					else if (num > 0.0001f)
					{
						this.array[(num5 - this.rect.offset.z) * this.rect.size.x + num6 - this.rect.offset.x] = num;
					}
				}
			}
		}

		// Token: 0x06003DA5 RID: 15781 RVA: 0x001D80F0 File Offset: 0x001D62F0
		public static void Blend(Matrix src, Matrix dst, float factor)
		{
			if (dst.rect != src.rect)
			{
				Debug.LogError("Matrix Blend: maps have different sizes");
			}
			for (int i = 0; i < dst.count; i++)
			{
				dst.array[i] = dst.array[i] * factor + src.array[i] * (1f - factor);
			}
		}

		// Token: 0x06003DA6 RID: 15782 RVA: 0x001D8150 File Offset: 0x001D6350
		public static void Mask(Matrix src, Matrix dst, Matrix mask)
		{
			if (src != null && (dst.rect != src.rect || dst.rect != mask.rect))
			{
				Debug.LogError("Matrix Mask: maps have different sizes");
			}
			for (int i = 0; i < dst.count; i++)
			{
				float num = mask.array[i];
				if (num <= 1f && num >= 0f)
				{
					dst.array[i] = dst.array[i] * num + ((src == null) ? 0f : (src.array[i] * (1f - num)));
				}
			}
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x001D81E8 File Offset: 0x001D63E8
		public static void SafeBorders(Matrix src, Matrix dst, int safeBorders)
		{
			Coord min = dst.rect.Min;
			Coord max = dst.rect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					int num = Mathf.Min(Mathf.Min(i - min.x, max.x - i), Mathf.Min(j - min.z, max.z - j));
					float num2 = 1f * (float)num / (float)safeBorders;
					if (num2 <= 1f)
					{
						dst[i, j] = dst[i, j] * num2 + ((src == null) ? 0f : (src[i, j] * (1f - num2)));
					}
				}
			}
		}

		// Token: 0x06003DA8 RID: 15784 RVA: 0x001D82BC File Offset: 0x001D64BC
		public void Add(Matrix add)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] += add.array[i];
			}
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x001D82F4 File Offset: 0x001D64F4
		public void Add(Matrix add, Matrix mask)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] += add.array[i] * mask.array[i];
			}
		}

		// Token: 0x06003DAA RID: 15786 RVA: 0x001D8334 File Offset: 0x001D6534
		public void Add(float add)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] += add;
			}
		}

		// Token: 0x06003DAB RID: 15787 RVA: 0x001D8364 File Offset: 0x001D6564
		public void Subtract(Matrix m)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] -= m.array[i];
			}
		}

		// Token: 0x06003DAC RID: 15788 RVA: 0x001D839C File Offset: 0x001D659C
		public void InvSubtract(Matrix m)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] = m.array[i] - this.array[i];
			}
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x001D83D4 File Offset: 0x001D65D4
		public void Multiply(Matrix m)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] *= m.array[i];
			}
		}

		// Token: 0x06003DAE RID: 15790 RVA: 0x001D840C File Offset: 0x001D660C
		public void Multiply(float m)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] *= m;
			}
		}

		// Token: 0x06003DAF RID: 15791 RVA: 0x001D843C File Offset: 0x001D663C
		public void Max(Matrix m)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (m.array[i] > this.array[i])
				{
					this.array[i] = m.array[i];
				}
			}
		}

		// Token: 0x06003DB0 RID: 15792 RVA: 0x001D847C File Offset: 0x001D667C
		public bool CheckRange(float min, float max)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i] < min || this.array[i] > max)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x001D84B4 File Offset: 0x001D66B4
		public void Invert()
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] = -this.array[i];
			}
		}

		// Token: 0x06003DB2 RID: 15794 RVA: 0x001D84E4 File Offset: 0x001D66E4
		public void InvertOne()
		{
			for (int i = 0; i < this.count; i++)
			{
				this.array[i] = 1f - this.array[i];
			}
		}

		// Token: 0x06003DB3 RID: 15795 RVA: 0x001D8518 File Offset: 0x001D6718
		public void Clamp01()
		{
			for (int i = 0; i < this.count; i++)
			{
				float num = this.array[i];
				if (num > 1f)
				{
					this.array[i] = 1f;
				}
				else if (num < 0f)
				{
					this.array[i] = 0f;
				}
			}
		}

		// Token: 0x06003DB4 RID: 15796 RVA: 0x001D856C File Offset: 0x001D676C
		public void ClampSubtract(Matrix m)
		{
			for (int i = 0; i < this.count; i++)
			{
				float num = this.array[i] - m.array[i];
				if (num > 1f)
				{
					num = 1f;
				}
				else if (num < 0f)
				{
					num = 0f;
				}
				this.array[i] = num;
			}
		}

		// Token: 0x06003DB5 RID: 15797 RVA: 0x001D85C4 File Offset: 0x001D67C4
		public bool IsEmpty(float delta = 0.0001f)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i] > delta)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003DB6 RID: 15798 RVA: 0x001D85F0 File Offset: 0x001D67F0
		public float MaxValue()
		{
			float num = -20000000f;
			for (int i = 0; i < this.count; i++)
			{
				float num2 = this.array[i];
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x02000974 RID: 2420
		public enum WrapMode
		{
			// Token: 0x040043F4 RID: 17396
			Once,
			// Token: 0x040043F5 RID: 17397
			Clamp,
			// Token: 0x040043F6 RID: 17398
			Tile,
			// Token: 0x040043F7 RID: 17399
			PingPong
		}

		// Token: 0x02000975 RID: 2421
		public class Stacker
		{
			// Token: 0x060053FA RID: 21498 RVA: 0x00244F88 File Offset: 0x00243188
			public Stacker(CoordRect smallRect, CoordRect bigRect)
			{
				this.smallRect = smallRect;
				this.bigRect = bigRect;
				this.isDownscaled = false;
				if (bigRect == smallRect)
				{
					this.upscaled = (this.downscaled = new Matrix(bigRect, null));
					return;
				}
				this.downscaled = new Matrix(smallRect, null);
				this.upscaled = new Matrix(bigRect, null);
				this.difference = new Matrix(bigRect, null);
			}

			// Token: 0x170006E5 RID: 1765
			// (get) Token: 0x060053FB RID: 21499 RVA: 0x00244FFE File Offset: 0x002431FE
			public Matrix matrix
			{
				get
				{
					if (this.isDownscaled)
					{
						return this.downscaled;
					}
					return this.upscaled;
				}
			}

			// Token: 0x060053FC RID: 21500 RVA: 0x00245018 File Offset: 0x00243218
			public void ToSmall()
			{
				if (this.bigRect == this.smallRect)
				{
					return;
				}
				this.downscaled = this.upscaled.OutdatedResize(this.smallRect, 1f, this.downscaled);
				if (this.preserveDetail)
				{
					this.difference = this.downscaled.OutdatedResize(this.bigRect, 1f, this.difference);
					this.difference.Blur(null, 0.666f, false, false, true, true, null);
					this.difference.InvSubtract(this.upscaled);
				}
				this.isDownscaled = true;
			}

			// Token: 0x060053FD RID: 21501 RVA: 0x002450B4 File Offset: 0x002432B4
			public void ToBig()
			{
				if (this.bigRect == this.smallRect)
				{
					return;
				}
				this.upscaled = this.downscaled.OutdatedResize(this.bigRect, 1f, this.upscaled);
				this.upscaled.Blur(null, 0.666f, false, false, true, true, null);
				if (this.preserveDetail)
				{
					this.upscaled.Add(this.difference);
				}
				this.isDownscaled = false;
			}

			// Token: 0x040043F8 RID: 17400
			public CoordRect smallRect;

			// Token: 0x040043F9 RID: 17401
			public CoordRect bigRect;

			// Token: 0x040043FA RID: 17402
			public bool preserveDetail = true;

			// Token: 0x040043FB RID: 17403
			private Matrix downscaled;

			// Token: 0x040043FC RID: 17404
			private Matrix upscaled;

			// Token: 0x040043FD RID: 17405
			private Matrix difference;

			// Token: 0x040043FE RID: 17406
			private bool isDownscaled;
		}
	}
}
