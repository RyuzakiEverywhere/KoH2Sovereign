using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public static class TerrainTypesRender
{
	// Token: 0x060019AF RID: 6575 RVA: 0x000FA0FC File Offset: 0x000F82FC
	public static RenderTexture Render(Vector2Int origin, TerrainType[,] tt_grid, RenderTexture rt, int width = 256, int height = 256, int overhang = 64, bool show_grid = false)
	{
		if (tt_grid == null)
		{
			return null;
		}
		TerrainTypesRender.origin = origin;
		TerrainTypesRender.sprites_field = global::Defs.GetDefField("TerrainTypes", "sprites");
		if (TerrainTypesRender.sprites_field == null)
		{
			return null;
		}
		bool sRGBWrite = GL.sRGBWrite;
		rt = TexRender.Begin(rt, width, height + overhang);
		if (rt == null)
		{
			return null;
		}
		TerrainTypesRender.grid = tt_grid;
		TerrainTypesRender.grid_size = new Vector2Int(tt_grid.GetLength(0), tt_grid.GetLength(1));
		TerrainTypesRender.tile_size = new Vector2((float)width / (float)TerrainTypesRender.grid_size.x, (float)height / (float)TerrainTypesRender.grid_size.y);
		TexRender.Clear(new Color32(206, 192, 176, 0));
		TerrainTypesRender.sprites = new List<TerrainTypesRender.SpriteInfo>();
		for (int i = TerrainTypesRender.grid_size.y - 1; i >= 0; i--)
		{
			for (int j = 0; j < TerrainTypesRender.grid_size.x; j++)
			{
				TerrainTypesRender.Draw(j, i);
			}
		}
		TerrainTypesRender.sprites.Sort(new Comparison<TerrainTypesRender.SpriteInfo>(TerrainTypesRender.SpriteInfo.Compare));
		for (int k = 0; k < TerrainTypesRender.sprites.Count; k++)
		{
			TerrainTypesRender.SpriteInfo spriteInfo = TerrainTypesRender.sprites[k];
			TexRender.Draw(new TexRender.Quad(spriteInfo.sprite, null)
			{
				pos = spriteInfo.pos,
				scale = spriteInfo.scale,
				pivot = spriteInfo.pivot
			});
		}
		if (show_grid)
		{
			TexRender.Draw(new TexRender.Grid(TerrainTypesRender.tile_size));
		}
		TexRender.End();
		GL.sRGBWrite = sRGBWrite;
		TerrainTypesRender.grid = null;
		TerrainTypesRender.sprites_field = null;
		TerrainTypesRender.sprites = null;
		return rt;
	}

	// Token: 0x060019B0 RID: 6576 RVA: 0x000FA294 File Offset: 0x000F8494
	private static Sprite GetSprite(DT.Field sf)
	{
		if (sf == null)
		{
			return null;
		}
		List<Sprite> list = new List<Sprite>();
		Sprite sprite = sf.value.obj_val as Sprite;
		if (sprite != null)
		{
			list.Add(sprite);
		}
		if (sf.children != null)
		{
			for (int i = 0; i < sf.children.Count; i++)
			{
				sprite = (sf.children[i].value.obj_val as Sprite);
				if (sprite != null)
				{
					list.Add(sprite);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x000FA335 File Offset: 0x000F8535
	private static bool Same(int x, int y, TerrainType tt)
	{
		return x < 0 || y < 0 || x >= TerrainTypesRender.grid_size.x || y >= TerrainTypesRender.grid_size.y || TerrainTypesRender.grid[x, y] == tt;
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x000FA36C File Offset: 0x000F856C
	private static string DecideTL(int x, int y, TerrainType tt)
	{
		bool flag = TerrainTypesRender.Same(x - 1, y, tt);
		bool flag2 = TerrainTypesRender.Same(x, y + 1, tt);
		bool flag3 = TerrainTypesRender.Same(x - 1, y + 1, tt);
		if (flag2 && flag)
		{
			if (!flag3)
			{
				return "TopLeftIn";
			}
			return "FullTopLeft";
		}
		else
		{
			if (flag)
			{
				return "TopLeft";
			}
			if (flag2)
			{
				return "LeftTop";
			}
			return "TopLeftOut";
		}
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x000FA3C8 File Offset: 0x000F85C8
	private static string DecideTR(int x, int y, TerrainType tt)
	{
		bool flag = TerrainTypesRender.Same(x + 1, y, tt);
		bool flag2 = TerrainTypesRender.Same(x, y + 1, tt);
		bool flag3 = TerrainTypesRender.Same(x + 1, y + 1, tt);
		if (flag2 && flag)
		{
			if (!flag3)
			{
				return "TopRightIn";
			}
			return "FullTopRight";
		}
		else
		{
			if (flag)
			{
				return "TopRight";
			}
			if (flag2)
			{
				return "RightTop";
			}
			return "TopRightOut";
		}
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x000FA424 File Offset: 0x000F8624
	private static string DecideBL(int x, int y, TerrainType tt)
	{
		bool flag = TerrainTypesRender.Same(x - 1, y, tt);
		bool flag2 = TerrainTypesRender.Same(x, y - 1, tt);
		bool flag3 = TerrainTypesRender.Same(x - 1, y - 1, tt);
		if (flag2 && flag)
		{
			if (!flag3)
			{
				return "BotLeftIn";
			}
			return "FullBotLeft";
		}
		else
		{
			if (flag)
			{
				return "BotLeft";
			}
			if (flag2)
			{
				return "LeftBot";
			}
			return "BotLeftOut";
		}
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x000FA480 File Offset: 0x000F8680
	private static string DecideBR(int x, int y, TerrainType tt)
	{
		bool flag = TerrainTypesRender.Same(x + 1, y, tt);
		bool flag2 = TerrainTypesRender.Same(x, y - 1, tt);
		bool flag3 = TerrainTypesRender.Same(x + 1, y - 1, tt);
		if (flag2 && flag)
		{
			if (!flag3)
			{
				return "BotRightIn";
			}
			return "FullBotRight";
		}
		else
		{
			if (flag)
			{
				return "BotRight";
			}
			if (flag2)
			{
				return "RightBot";
			}
			return "BotRightOut";
		}
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x000FA4DC File Offset: 0x000F86DC
	private static void DrawWaterTile(int x, int y, DT.Field sf, string key, Vector2 pivot)
	{
		Sprite sprite = TerrainTypesRender.GetSprite(sf.FindChild(key, null, true, true, true, '.'));
		if (sprite == null)
		{
			return;
		}
		Vector2 size = sprite.rect.size;
		Vector2 scale = new Vector2(TerrainTypesRender.tile_size.x / 2f / size.x, TerrainTypesRender.tile_size.y / 2f / size.y);
		Vector2 pos = new Vector2((pivot.x + (float)x) * TerrainTypesRender.tile_size.x, (pivot.y + (float)y) * TerrainTypesRender.tile_size.y);
		TerrainTypesRender.sprites.Add(new TerrainTypesRender.SpriteInfo(sprite, 1, pos, pivot, scale));
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x000FA594 File Offset: 0x000F8794
	private static void DrawWater(int x, int y, TerrainType tt, DT.Field sf)
	{
		string key = TerrainTypesRender.DecideTL(x, y, tt);
		string key2 = TerrainTypesRender.DecideTR(x, y, tt);
		string key3 = TerrainTypesRender.DecideBL(x, y, tt);
		string key4 = TerrainTypesRender.DecideBR(x, y, tt);
		TerrainTypesRender.DrawWaterTile(x, y, sf, key, Vector2.up);
		TerrainTypesRender.DrawWaterTile(x, y, sf, key2, Vector2.one);
		TerrainTypesRender.DrawWaterTile(x, y, sf, key3, Vector2.zero);
		TerrainTypesRender.DrawWaterTile(x, y, sf, key4, Vector2.right);
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x000FA600 File Offset: 0x000F8800
	private static void Draw(int x, int y)
	{
		TerrainType terrainType = TerrainTypesRender.grid[x, y];
		DT.Field field = TerrainTypesRender.sprites_field.FindChild(terrainType.ToString(), null, true, true, true, '.');
		if (field == null && terrainType == TerrainType.Lake)
		{
			field = TerrainTypesRender.sprites_field.FindChild("Ocean", null, true, true, true, '.');
		}
		if (field == null)
		{
			return;
		}
		if (TerrainTypesRender.origin != Vector2Int.zero)
		{
			Random.InitState(TerrainTypesRender.origin.y + y << 18 | TerrainTypesRender.origin.x + x << 4 | (int)terrainType);
		}
		if (terrainType == TerrainType.Ocean || terrainType == TerrainType.Lake || terrainType == TerrainType.River)
		{
			TerrainTypesRender.DrawWater(x, y, terrainType, field);
			return;
		}
		Point point = field.GetPoint("grow", null, true, true, true, '.');
		int @int = field.GetInt("cols_min", null, 1, true, true, true, '.');
		int int2 = field.GetInt("cols_max", null, @int, true, true, true, '.');
		int int3 = field.GetInt("rows_min", null, 1, true, true, true, '.');
		int int4 = field.GetInt("rows_max", null, int3, true, true, true, '.');
		int num = Random.Range(int3, int4 + 1);
		float num2 = point.x * TerrainTypesRender.tile_size.x;
		float num3 = (int2 > 1) ? (num2 / (float)(int2 - 1)) : 0f;
		float num4 = 0f;
		float num5 = (num > 1) ? (point.y * TerrainTypesRender.tile_size.y / (float)(num - 1)) : 0f;
		for (int i = 0; i < num; i++)
		{
			float num6 = -num2 / 2f;
			int num7 = Random.Range(@int, int2 + 1);
			float num8 = (int2 > 1) ? ((float)(num7 - 1) * num3) : 0f;
			float num9 = num2 - num8;
			if (num9 > 0f)
			{
				num6 += Random.Range(0f, num9);
			}
			for (int j = 0; j < num7; j++)
			{
				Sprite sprite = TerrainTypesRender.GetSprite(field);
				if (sprite == null)
				{
					return;
				}
				Vector2 size = sprite.rect.size;
				Vector2 vector = new Vector2(sprite.pivot.x / size.x, sprite.pivot.y / size.y);
				float a = TerrainTypesRender.tile_size.x / size.x;
				float b = TerrainTypesRender.tile_size.y / size.y;
				float num10 = Mathf.Min(a, b);
				Vector2 scale = new Vector2(num10, num10);
				Vector2 pos = new Vector2((vector.x + (float)x) * TerrainTypesRender.tile_size.x, (vector.y + (float)y) * TerrainTypesRender.tile_size.y);
				pos.x += num6;
				pos.y += num4;
				TerrainTypesRender.sprites.Add(new TerrainTypesRender.SpriteInfo(sprite, 2, pos, vector, scale));
				num6 += num3;
			}
			num4 += num5;
		}
	}

	// Token: 0x04001082 RID: 4226
	private static Vector2Int origin = Vector2Int.zero;

	// Token: 0x04001083 RID: 4227
	private static TerrainType[,] grid = null;

	// Token: 0x04001084 RID: 4228
	private static Vector2Int grid_size;

	// Token: 0x04001085 RID: 4229
	private static Vector2 tile_size;

	// Token: 0x04001086 RID: 4230
	private static DT.Field sprites_field = null;

	// Token: 0x04001087 RID: 4231
	private static List<TerrainTypesRender.SpriteInfo> sprites = null;

	// Token: 0x0200070E RID: 1806
	private struct SpriteInfo
	{
		// Token: 0x0600495B RID: 18779 RVA: 0x0021B0F9 File Offset: 0x002192F9
		public SpriteInfo(Sprite sprite, int layer, Vector2 pos, Vector2 pivot, Vector2 scale)
		{
			this.sprite = sprite;
			this.layer = layer;
			this.pos = pos;
			this.pivot = pivot;
			this.scale = scale;
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x0021B120 File Offset: 0x00219320
		public static int Compare(TerrainTypesRender.SpriteInfo a, TerrainTypesRender.SpriteInfo b)
		{
			if (a.layer < b.layer)
			{
				return -1;
			}
			if (a.layer > b.layer)
			{
				return 1;
			}
			if (a.pos.y > b.pos.y)
			{
				return -1;
			}
			if (a.pos.y < b.pos.y)
			{
				return 1;
			}
			if (a.pos.x < b.pos.x)
			{
				return -1;
			}
			if (a.pos.x > b.pos.x)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x04003808 RID: 14344
		public Sprite sprite;

		// Token: 0x04003809 RID: 14345
		public int layer;

		// Token: 0x0400380A RID: 14346
		public Vector2 pos;

		// Token: 0x0400380B RID: 14347
		public Vector2 pivot;

		// Token: 0x0400380C RID: 14348
		public Vector2 scale;
	}
}
