using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class ImageProcessor : MonoBehaviour
{
	// Token: 0x06001061 RID: 4193 RVA: 0x000ADCE4 File Offset: 0x000ABEE4
	public static bool Process(Texture2D result, Texture2D source, bool save_progress = false, params ImageProcessor.Pass[] passes)
	{
		if (source == null || result == null)
		{
			return false;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(result.width, result.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		RenderTexture renderTexture2 = RenderTexture.GetTemporary(result.width, result.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		renderTexture.wrapMode = (renderTexture2.wrapMode = result.wrapMode);
		renderTexture.filterMode = (renderTexture2.filterMode = result.filterMode);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < passes.Length; i++)
		{
			ImageProcessor.Pass pass = passes[i];
			if (pass.repeats > 0 && !(pass.material == null))
			{
				if (!flag)
				{
					flag = true;
					Graphics.Blit(source, renderTexture, pass.material);
					pass.repeats--;
				}
				for (int j = 0; j < pass.repeats; j++)
				{
					float num = (float)j / (float)pass.repeats;
					string step = null;
					switch (i)
					{
					case 0:
						step = string.Format("Expanding realms - {0}%", 100f * num);
						break;
					case 1:
						step = string.Format("Expanding borders - {0}%", 100f * num);
						break;
					case 2:
						step = string.Format("Fixing areas with no realms - {0}%", 100f * num);
						break;
					}
					if (!Common.EditorProgress("Generating Realms Data", step, num, true))
					{
						BuildTools.cancelled = true;
						return false;
					}
					Graphics.Blit(renderTexture, renderTexture2, pass.material);
					RenderTexture renderTexture3 = renderTexture;
					renderTexture = renderTexture2;
					renderTexture2 = renderTexture3;
					if (save_progress)
					{
						ImageProcessor.SaveRT(renderTexture, result, string.Concat(new string[]
						{
							"testgen/",
							i.ToString(),
							"-",
							j.ToString(),
							".png"
						}));
					}
				}
				if (ImageProcessor.SaveRT(renderTexture, pass.save, null))
				{
					flag2 = true;
				}
			}
		}
		if (flag && ImageProcessor.SaveRT(renderTexture, result, null))
		{
			flag2 = true;
		}
		RenderTexture.ReleaseTemporary(renderTexture);
		RenderTexture.ReleaseTemporary(renderTexture2);
		return flag2;
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x000ADEF0 File Offset: 0x000AC0F0
	public static bool SaveRT(RenderTexture src, Texture2D dst, string path = null)
	{
		if (dst == null)
		{
			return false;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(dst.width, dst.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		Graphics.Blit(src, temporary);
		RenderTexture.active = temporary;
		dst.ReadPixels(new Rect(0f, 0f, (float)dst.width, (float)dst.height), 0, 0);
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(temporary);
		dst.Apply();
		return false;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000ADF64 File Offset: 0x000AC164
	public static Texture2D BuildHeightsTexture(Terrain t = null)
	{
		if (t == null)
		{
			t = Terrain.activeTerrain;
			if (t == null)
			{
				return null;
			}
		}
		TerrainData terrainData = t.terrainData;
		int heightmapResolution = terrainData.heightmapResolution;
		int heightmapResolution2 = terrainData.heightmapResolution;
		float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution2);
		Texture2D texture2D = new Texture2D(heightmapResolution, heightmapResolution2);
		Color[] array = new Color[heightmapResolution * heightmapResolution2];
		int num = 0;
		for (int i = 0; i < heightmapResolution2; i++)
		{
			for (int j = 0; j < heightmapResolution; j++)
			{
				float num2 = heights[i, j];
				float x = (float)j / (float)(heightmapResolution - 1);
				float y = (float)i / (float)(heightmapResolution2 - 1);
				float b = terrainData.GetSteepness(x, y) / 90f;
				array[num++] = new Color(num2, num2, b, 1f);
			}
		}
		texture2D.SetPixels(array);
		return texture2D;
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x000AE040 File Offset: 0x000AC240
	public static Texture2D BuildRealmsDataInitialTexture(float fWaterLevel, Terrain t = null)
	{
		if (t == null)
		{
			t = Terrain.activeTerrain;
			if (t == null)
			{
				return null;
			}
		}
		TerrainData terrainData = t.terrainData;
		int num = 2048;
		int num2 = 2048;
		byte[,] array = TerrainInfo.RenderRivers(num, num2, false);
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, false, true);
		Color[] array2 = new Color[num * num2];
		int num3 = 0;
		float a = 1f;
		for (int i = 0; i < num2; i++)
		{
			float num4 = (float)i / (float)(num2 - 1);
			int j = 0;
			while (j < num)
			{
				float num5 = (float)j / (float)(num - 1);
				if (terrainData.GetInterpolatedHeight(num5, num4) >= fWaterLevel)
				{
					goto IL_18C;
				}
				float num6 = terrainData.size.x * num5;
				float num7 = terrainData.size.z * num4;
				int num8 = (int)num6;
				int num9 = (int)num7;
				int num10 = (int)num6 + 1;
				int num11 = (int)num7 + 1;
				float interpolatedHeight = terrainData.GetInterpolatedHeight((float)num8 / terrainData.size.x, (float)num9 / terrainData.size.z);
				float interpolatedHeight2 = terrainData.GetInterpolatedHeight((float)num8 / terrainData.size.x, (float)num11 / terrainData.size.z);
				float interpolatedHeight3 = terrainData.GetInterpolatedHeight((float)num10 / terrainData.size.x, (float)num9 / terrainData.size.z);
				float interpolatedHeight4 = terrainData.GetInterpolatedHeight((float)num10 / terrainData.size.x, (float)num11 / terrainData.size.z);
				if (interpolatedHeight >= fWaterLevel || interpolatedHeight2 >= fWaterLevel || interpolatedHeight3 >= fWaterLevel || interpolatedHeight4 >= fWaterLevel)
				{
					goto IL_18C;
				}
				array2[num3++] = new Color(0f, 0f, 1f, a);
				IL_1F9:
				j++;
				continue;
				IL_18C:
				if (array != null && array[j, i] > 0)
				{
					array2[num3++] = new Color(0f, 0f, 0.6f, a);
					goto IL_1F9;
				}
				float num12 = terrainData.GetSteepness(num5, num4);
				num12 /= 100f;
				array2[num3++] = new Color(0f, 0f, num12, a);
				goto IL_1F9;
			}
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			Settlement settlement = Settlement.First();
			while (settlement != null)
			{
				if (settlement.IsCastle())
				{
					Realm realm = worldMap.FindRealm(settlement.Name);
					if (realm != null && realm.id > 0)
					{
						Vector3 position = settlement.transform.position;
						int num13 = (int)((position.x - t.transform.position.x) * (float)num / terrainData.bounds.size.x);
						int num14 = (int)((position.z - t.transform.position.z) * (float)num2 / terrainData.bounds.size.z);
						byte b = (byte)realm.id;
						byte b2 = (byte)(realm.id >> 8);
						int num15 = num14 * num + num13;
						Color color = array2[num15];
						color.r = (float)b / 255f;
						color.g = (float)b2 / 255f;
						color.a = (float)(128 - settlement.GrowRealmBorders) / 255f;
						array2[num15] = color;
					}
				}
				settlement = settlement.Next();
			}
			RealmOriginPoint realmOriginPoint = RealmOriginPoint.First();
			while (realmOriginPoint != null)
			{
				Realm realm2 = worldMap.FindRealm(realmOriginPoint.realmName);
				if (realm2 == null)
				{
					realm2 = Realm.New(realmOriginPoint.realmName);
					realm2.kingdom = 0;
					realm2.MapColor = worldMap.GetRandomColor();
				}
				if (realmOriginPoint.hasNegativeId)
				{
					realm2.id = -Mathf.Abs(realm2.id);
				}
				else
				{
					realm2.id = Mathf.Abs(realm2.id);
				}
				int num16 = Mathf.Abs(realm2.id);
				Vector3 position2 = realmOriginPoint.transform.position;
				int num17 = (int)((position2.x - t.transform.position.x) * (float)num / terrainData.bounds.size.x);
				int num18 = (int)((position2.z - t.transform.position.z) * (float)num2 / terrainData.bounds.size.z);
				byte b3 = (byte)num16;
				byte b4 = (byte)(num16 >> 8);
				int num19 = num18 * num + num17;
				Color color2 = array2[num19];
				color2.r = (float)b3 / 255f;
				color2.g = (float)b4 / 255f;
				array2[num19] = color2;
				realmOriginPoint = realmOriginPoint.Next();
			}
		}
		texture2D.SetPixels(array2);
		return texture2D;
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x000AE504 File Offset: 0x000AC704
	public static Texture2D BuildRoadsInitialTexture(float fWaterLevel, Terrain t = null)
	{
		if (t == null)
		{
			t = Terrain.activeTerrain;
			if (t == null)
			{
				return null;
			}
		}
		TerrainData terrainData = t.terrainData;
		int num = 2048;
		int num2 = 2048;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGB24, false, true);
		Color[] array = new Color[num * num2];
		int num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			float y = (float)i / (float)(num2 - 1);
			for (int j = 0; j < num; j++)
			{
				float x = (float)j / (float)(num - 1);
				if (terrainData.GetInterpolatedHeight(x, y) < fWaterLevel)
				{
					array[num3++] = new Color(1f, 1f, 1f, 1f);
				}
				else
				{
					float num4 = terrainData.GetSteepness(x, y);
					num4 /= 100f;
					array[num3++] = new Color(0f, 0f, num4, 1f);
				}
			}
		}
		List<Color> list = new List<Color>();
		for (int k = 31; k <= 255; k += 32)
		{
			for (int l = 31; l <= 255; l += 32)
			{
				list.Add(new Color32((byte)k, (byte)l, 0, byte.MaxValue));
			}
		}
		int num5 = 0;
		Settlement settlement = Settlement.First();
		while (settlement != null)
		{
			int num6 = (int)(settlement.transform.position.x * (float)num / terrainData.bounds.size.x);
			int num7 = (int)(settlement.transform.position.z * (float)num2 / terrainData.bounds.size.z) * num + num6;
			Color color = array[num7];
			Color color2 = list[num5 % list.Count];
			color.r = color2.r;
			color.g = color2.g;
			array[num7] = color;
			num5++;
			settlement = settlement.Next();
		}
		texture2D.SetPixels(array);
		return texture2D;
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x000AE730 File Offset: 0x000AC930
	private static bool MoveToRoadPrev(Color[] src_pixels, int w, ref int x, ref int y)
	{
		int num = y * w + x;
		Color color = src_pixels[num];
		float num2 = color.b * 255f + color.a;
		int num3 = x;
		int num4 = y;
		for (int i = y - 1; i <= y + 1; i++)
		{
			for (int j = x - 1; j <= x + 1; j++)
			{
				int num5 = i * w + j;
				Color color2 = src_pixels[num5];
				if (color2.r == color.r && color2.g == color.g)
				{
					float num6 = color2.b * 255f + color2.a;
					if (num6 < num2)
					{
						num2 = num6;
						num3 = j;
						num4 = i;
					}
				}
			}
		}
		if (num3 == x && num4 == y)
		{
			return false;
		}
		x = num3;
		y = num4;
		return true;
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x000AE800 File Offset: 0x000ACA00
	private static void PaintRoad(Color[] src_pixels, Color[] dst_pixels, int w, int x, int y, Color clr)
	{
		for (int i = 0; i < 500; i++)
		{
			int num = y * w + x;
			dst_pixels[num] = clr;
			if (!ImageProcessor.MoveToRoadPrev(src_pixels, w, ref x, ref y))
			{
				return;
			}
		}
		Debug.LogError("loop in PaintRoad");
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x000AE848 File Offset: 0x000ACA48
	public static void PostProcessRoads(Texture2D tex)
	{
		if (tex == null)
		{
			return;
		}
		Color[] pixels = tex.GetPixels();
		Color[] array = new Color[pixels.Length];
		Dictionary<int, ImageProcessor.RoadInfo> dictionary = new Dictionary<int, ImageProcessor.RoadInfo>();
		int num = 0;
		for (int i = 0; i < tex.height; i++)
		{
			int j = 0;
			while (j < tex.width)
			{
				Color color = pixels[num];
				if ((color.r != 0f || color.g != 0f) && (color.r != 1f || color.g != 1f))
				{
					int num2 = (int)(color.r * 255f) | (int)(color.g * 255f) << 8;
					for (int k = i - 1; k <= i + 1; k++)
					{
						if (k >= 0 && k < tex.height)
						{
							for (int l = j - 1; l <= j + 1; l++)
							{
								if (l >= 0 && l < tex.width)
								{
									int num3 = num + (k - i) * tex.width + (l - j);
									if (num3 != num)
									{
										Color color2 = pixels[num3];
										if ((color2.r != color.r || color2.g != color.g) && (color2.r != 0f || color2.g != 0f) && (color2.r != 1f || color2.g != 1f))
										{
											int num4 = (int)(color2.r * 255f) | (int)(color2.g * 255f) << 8;
											float num5 = color.b * 255f + color.a;
											int key = (num2 < num4) ? (num2 | num4 << 16) : (num4 | num2 << 16);
											ImageProcessor.RoadInfo roadInfo;
											if (!dictionary.TryGetValue(key, out roadInfo) || roadInfo.eval >= num5)
											{
												dictionary[key] = new ImageProcessor.RoadInfo
												{
													id1 = num2,
													id2 = num4,
													x1 = j,
													y1 = i,
													x2 = l,
													y2 = k,
													eval = num5
												};
											}
										}
									}
								}
							}
						}
					}
					color.b = 0f;
					color.a = 0.5f;
					array[num] = color;
				}
				j++;
				num++;
			}
		}
		foreach (KeyValuePair<int, ImageProcessor.RoadInfo> keyValuePair in dictionary)
		{
			ImageProcessor.RoadInfo value = keyValuePair.Value;
			ImageProcessor.PaintRoad(pixels, array, tex.width, value.x1, value.y1, Color.white);
			ImageProcessor.PaintRoad(pixels, array, tex.width, value.x2, value.y2, Color.white);
		}
		tex.SetPixels(array);
		tex.Apply();
	}

	// Token: 0x04000ACF RID: 2767
	public float WaterLevel = 8f;

	// Token: 0x04000AD0 RID: 2768
	public Texture2D source;

	// Token: 0x04000AD1 RID: 2769
	public ImageProcessor.Pass[] passes;

	// Token: 0x04000AD2 RID: 2770
	public Texture2D result;

	// Token: 0x0200064E RID: 1614
	[Serializable]
	public struct Pass
	{
		// Token: 0x06004763 RID: 18275 RVA: 0x0021381E File Offset: 0x00211A1E
		public Pass(Material material, int repeats = 1)
		{
			this.material = material;
			this.repeats = repeats;
			this.save = null;
		}

		// Token: 0x06004764 RID: 18276 RVA: 0x00213835 File Offset: 0x00211A35
		public Pass(Shader shader, int repeats = 1)
		{
			this.material = ((shader == null) ? null : new Material(shader));
			this.repeats = repeats;
			this.save = null;
		}

		// Token: 0x06004765 RID: 18277 RVA: 0x00213860 File Offset: 0x00211A60
		public Pass(string shader_name, int repeats = 1)
		{
			Shader shader = Shader.Find(shader_name);
			this.material = ((shader == null) ? null : new Material(shader));
			this.repeats = repeats;
			this.save = null;
		}

		// Token: 0x040034E7 RID: 13543
		public Material material;

		// Token: 0x040034E8 RID: 13544
		public int repeats;

		// Token: 0x040034E9 RID: 13545
		public Texture2D save;
	}

	// Token: 0x0200064F RID: 1615
	private struct RoadInfo
	{
		// Token: 0x040034EA RID: 13546
		public int id1;

		// Token: 0x040034EB RID: 13547
		public int id2;

		// Token: 0x040034EC RID: 13548
		public int x1;

		// Token: 0x040034ED RID: 13549
		public int y1;

		// Token: 0x040034EE RID: 13550
		public int x2;

		// Token: 0x040034EF RID: 13551
		public int y2;

		// Token: 0x040034F0 RID: 13552
		public float eval;
	}
}
