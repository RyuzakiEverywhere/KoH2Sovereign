using System;
using System.IO;
using System.Runtime.InteropServices;
using Logic;
using UnityEngine;

// Token: 0x02000183 RID: 387
public static class TerrainInfo
{
	// Token: 0x1700010E RID: 270
	// (get) Token: 0x0600151F RID: 5407 RVA: 0x000D59D4 File Offset: 0x000D3BD4
	public static int width
	{
		get
		{
			if (TerrainInfo.cells != null)
			{
				return TerrainInfo.cells.GetLength(0);
			}
			return 0;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06001520 RID: 5408 RVA: 0x000D59EA File Offset: 0x000D3BEA
	public static int height
	{
		get
		{
			if (TerrainInfo.cells != null)
			{
				return TerrainInfo.cells.GetLength(1);
			}
			return 0;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06001521 RID: 5409 RVA: 0x000D5A00 File Offset: 0x000D3C00
	public static bool Innitted
	{
		get
		{
			return TerrainInfo.cells != null;
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000D5A0C File Offset: 0x000D3C0C
	public static TerrainInfo.Cell Get(TerrainInfo.Coords ptg, int update = 255)
	{
		TerrainInfo.Cell cell = TerrainInfo.cells[ptg.x, ptg.y];
		bool flag = cell.bt_height == byte.MaxValue && (update & 1) != 0;
		bool flag2 = cell.slope == byte.MaxValue && (update & 2) != 0;
		if (flag || flag2)
		{
			float x = (float)ptg.x / (float)(TerrainInfo.width - 1);
			float y = (float)ptg.y / (float)(TerrainInfo.height - 1);
			if (flag)
			{
				TerrainInfo.height_tiles++;
				float num = TerrainInfo.terrain_data.GetInterpolatedHeight(x, y);
				if (num > 32f)
				{
					num = 32f;
				}
				byte bt_height = (byte)(254f * (num / 32f));
				TerrainInfo.cells[ptg.x, ptg.y].bt_height = (cell.bt_height = bt_height);
			}
			if (flag2)
			{
				TerrainInfo.slope_tiles++;
				TerrainInfo.cells[ptg.x, ptg.y].slope = (cell.slope = (byte)TerrainInfo.terrain_data.GetSteepness(x, y));
			}
		}
		return cell;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000D5B38 File Offset: 0x000D3D38
	public static void AddPath(TerrainInfo.Coords ptg, bool bRiver)
	{
		if (bRiver)
		{
			for (int i = ptg.y - 1; i <= ptg.y + 1; i++)
			{
				for (int j = ptg.x - 1; j <= ptg.x + 1; j++)
				{
					TerrainInfo.river_tiles++;
					ref TerrainInfo.Cell ptr = ref TerrainInfo.cells[j, i];
					ptr.rivers += 1;
				}
			}
			return;
		}
		TerrainInfo.road_tiles++;
		ref TerrainInfo.Cell ptr2 = ref TerrainInfo.cells[ptg.x, ptg.y];
		ptr2.roads += 1;
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000D5BD0 File Offset: 0x000D3DD0
	public static void DelPath(TerrainInfo.Coords ptg, bool bRiver)
	{
		if (bRiver)
		{
			for (int i = ptg.y - 1; i <= ptg.y + 1; i++)
			{
				for (int j = ptg.x - 1; j <= ptg.x + 1; j++)
				{
					TerrainInfo.river_tiles--;
					ref TerrainInfo.Cell ptr = ref TerrainInfo.cells[j, i];
					ptr.rivers -= 1;
				}
			}
			return;
		}
		TerrainInfo.road_tiles--;
		ref TerrainInfo.Cell ptr2 = ref TerrainInfo.cells[ptg.x, ptg.y];
		ptr2.roads -= 1;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x000D5C68 File Offset: 0x000D3E68
	public static Vector3 GridToWorld(TerrainInfo.Coords ptg, bool randomize = false)
	{
		float x = TerrainInfo.rcWorld.xMin + (float)ptg.x * TerrainInfo.cell_size;
		float z = TerrainInfo.rcWorld.yMin + (float)ptg.y * TerrainInfo.cell_size;
		Vector3 result = new Vector3(x, 0f, z);
		if (randomize && !Application.isPlaying)
		{
			Random.InitState(100 * ptg.x + ptg.y);
			float num = Random.Range(-TerrainInfo.cell_size / 3f, TerrainInfo.cell_size / 3f);
			float num2 = Random.Range(-TerrainInfo.cell_size / 3f, TerrainInfo.cell_size / 3f);
			result.x += num;
			result.z += num2;
		}
		return result;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x000D5D2C File Offset: 0x000D3F2C
	public static TerrainInfo.Coords WorldToGrid(Vector3 ptw)
	{
		int x = (int)((ptw.x - TerrainInfo.rcWorld.xMin) / TerrainInfo.cell_size);
		int y = (int)((ptw.z - TerrainInfo.rcWorld.yMin) / TerrainInfo.cell_size);
		return new TerrainInfo.Coords(x, y);
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06001527 RID: 5415 RVA: 0x000D5D70 File Offset: 0x000D3F70
	public static int version
	{
		get
		{
			return TerrainInfo._version;
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x0002C53B File Offset: 0x0002A73B
	public static bool Update()
	{
		return true;
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000D5D78 File Offset: 0x000D3F78
	public static void Clean()
	{
		TerrainInfo._version++;
		TerrainInfo.rcWorld = Rect.zero;
		TerrainInfo.cells = null;
		TerrainInfo.height_tiles = (TerrainInfo.slope_tiles = (TerrainInfo.road_tiles = (TerrainInfo.river_tiles = 0)));
		TerrainInfo.Update();
		TerrainInfo.AddPaths();
		TerrainInfo.AddLakes(false);
		TerrainInfo.AddRivers(false);
		TerrainInfo.AddMountains(false);
		TerrainInfo.AddWalls();
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x000D5DDC File Offset: 0x000D3FDC
	public static void AddPaths()
	{
		TerrainPath terrainPath = TerrainPath.First();
		while (terrainPath != null)
		{
			if (terrainPath.Initted())
			{
				terrainPath.RegisterInTI();
			}
			terrainPath = terrainPath.Next();
		}
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x000D5E10 File Offset: 0x000D4010
	public static byte[,] RenderRivers(int width, int height, bool use_yofs = false)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return null;
		}
		Shader shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (shader == null)
		{
			return null;
		}
		Camera camera = global::Common.CreateTerrainCam(activeTerrain);
		if (camera == null)
		{
			return null;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(TerrainInfo.rt_res, TerrainInfo.rt_res, 0);
		camera.targetTexture = renderTexture;
		Shader.SetGlobalInt("_ARPass", 0);
		Shader.SetGlobalColor("_ARColor", Color.white);
		camera.clearFlags = CameraClearFlags.Color;
		camera.cullingMask = 1 << LayerMask.NameToLayer("Rivers");
		camera.RenderWithShader(shader, "");
		int i = TerrainInfo.rt_res;
		while (i > TerrainInfo.tex_res)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(renderTexture, temporary);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Texture2D texture2D = new Texture2D(TerrainInfo.tex_res, TerrainInfo.tex_res, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)TerrainInfo.tex_res, (float)TerrainInfo.tex_res), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		RenderTexture.ReleaseTemporary(renderTexture);
		File.WriteAllBytes("Assets/Test/Michael/RealmsData/rivers.png", texture2D.EncodeToPNG());
		Vector3 size = activeTerrain.terrainData.size;
		int num = use_yofs ? Mathf.RoundToInt(Mathf.Abs(size.x - size.z) * (float)(TerrainInfo.tex_res - 1) / (2f * Mathf.Max(size.x, size.z))) : 0;
		int num2 = TerrainInfo.tex_res - 2 * num;
		Color[] pixels = texture2D.GetPixels();
		byte[,] array = new byte[width, height];
		for (int j = 0; j < height; j++)
		{
			int num3 = num + j * num2 / height;
			for (int k = 0; k < width; k++)
			{
				int num4 = k * TerrainInfo.tex_res / width;
				Color color = pixels[num3 * TerrainInfo.tex_res + num4];
				array[k, j] = (byte)(color.r * 255f);
			}
		}
		return array;
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x000D6020 File Offset: 0x000D4220
	public static void AddLakes(bool use_yofs = false)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return;
		}
		Shader shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (shader == null)
		{
			return;
		}
		Camera camera = global::Common.CreateTerrainCam(activeTerrain);
		if (camera == null)
		{
			return;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(TerrainInfo.rt_res, TerrainInfo.rt_res, 0);
		camera.targetTexture = renderTexture;
		Shader.SetGlobalInt("_ARPass", 0);
		Shader.SetGlobalColor("_ARColor", Color.white);
		camera.clearFlags = CameraClearFlags.Color;
		camera.cullingMask = 1 << LayerMask.NameToLayer("Lakes");
		camera.RenderWithShader(shader, "");
		int i = TerrainInfo.rt_res;
		while (i > TerrainInfo.tex_res)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(renderTexture, temporary);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Texture2D texture2D = new Texture2D(TerrainInfo.tex_res, TerrainInfo.tex_res, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)TerrainInfo.tex_res, (float)TerrainInfo.tex_res), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		RenderTexture.ReleaseTemporary(renderTexture);
		Vector3 size = activeTerrain.terrainData.size;
		int num = use_yofs ? Mathf.RoundToInt(Mathf.Abs(size.x - size.z) * (float)(TerrainInfo.tex_res - 1) / (2f * Mathf.Max(size.x, size.z))) : 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < TerrainInfo.tex_res - num; j++)
		{
			int num2 = (j - num) * TerrainInfo.height / (TerrainInfo.tex_res - 2 * num);
			for (int k = 0; k < TerrainInfo.tex_res; k++)
			{
				if (pixels[j * TerrainInfo.tex_res + k].r > 0f)
				{
					int num3 = k * TerrainInfo.width / TerrainInfo.tex_res;
					TerrainInfo.cells[num3, num2].slope = TerrainInfo.lake_slope;
				}
			}
		}
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x000D6228 File Offset: 0x000D4428
	private static bool AddRiverCrossings()
	{
		foreach (RamSpline ramSpline in UnityEngine.Object.FindObjectsOfType<RamSpline>())
		{
			for (int j = 0; j < ramSpline.points.Count - 1; j++)
			{
				Vector3 a = ramSpline.transform.TransformPoint(ramSpline.points[j]);
				Vector3 b = ramSpline.transform.TransformPoint(ramSpline.points[j + 1]);
				float num = Vector3.Distance(a, b);
				for (float num2 = 0f; num2 < num; num2 += 3f)
				{
					Vector3 v = Vector3.Lerp(a, b, num2 / num);
					Point point = ramSpline.tangents[j];
					point = new Point(-point.y, point.x);
					Point point2 = v;
					Point pt = point2 + point * 100f;
					Coord tile = Coord.WorldToGrid(point2, 1f);
					Point point3 = Coord.WorldToLocal(tile, point2, 1f);
					Point point4 = Coord.WorldToLocal(tile, pt, 1f);
					Coord invalid = Coord.Invalid;
					Coord invalid2 = Coord.Invalid;
					Coord coord = Coord.Invalid;
					Coord coord2 = Coord.Invalid;
					Point point5;
					do
					{
						int num3 = Mathf.RoundToInt(point2.x);
						int num4 = Mathf.RoundToInt(point2.y);
						point5 = new Point((float)num3, (float)num4);
						coord = new Coord(point2);
						if (point5.x < 0f || point5.x >= (float)TerrainInfo.cells.GetLength(0) || point5.y < 0f || point5.y >= (float)TerrainInfo.cells.GetLength(1) || TerrainInfo.cells[num3, num4].rivers == 0)
						{
							break;
						}
						point2 = Coord.GridToWorld(tile, 1f);
					}
					while (Coord.RayStep(ref tile, ref point3, ref point4, 0f, out invalid, out invalid2));
					point2 = v;
					pt = point2 - point * 100f;
					tile = Coord.WorldToGrid(point2, 1f);
					point3 = Coord.WorldToLocal(tile, point2, 1f);
					point4 = Coord.WorldToLocal(tile, pt, 1f);
					Point point6;
					do
					{
						int num5 = Mathf.RoundToInt(point2.x);
						int num6 = Mathf.RoundToInt(point2.y);
						point6 = new Point((float)num5, (float)num6);
						coord2 = new Coord(point2);
						if (point6.x < 0f || point6.x >= (float)TerrainInfo.cells.GetLength(0) || point6.y < 0f || point6.y >= (float)TerrainInfo.cells.GetLength(1) || TerrainInfo.cells[num5, num6].rivers == 0)
						{
							break;
						}
						point2 = Coord.GridToWorld(tile, 1f);
					}
					while (Coord.RayStep(ref tile, ref point3, ref point4, 0f, out invalid, out invalid2));
					if (!(coord2 == coord) && point5.x >= 0f && point5.x < (float)TerrainInfo.cells.GetLength(0) && point5.y >= 0f && point5.y < (float)TerrainInfo.cells.GetLength(1) && point6.x >= 0f && point6.x < (float)TerrainInfo.cells.GetLength(0) && point6.y >= 0f && point6.y < (float)TerrainInfo.cells.GetLength(1))
					{
						Coord coord3 = (coord2 - coord) * 2 / 3;
						if (coord3.x <= 7 && coord3.y <= 7)
						{
							coord = (coord + coord2) / 2 - coord3 * 3 / 2;
							coord2 = coord + coord3 * 3;
							int num7 = Mathf.RoundToInt((float)coord.x);
							int num8 = Mathf.RoundToInt((float)coord.y);
							point5 = new Point((float)num7, (float)num8);
							num7 = Mathf.RoundToInt((float)coord2.x);
							num8 = Mathf.RoundToInt((float)coord2.y);
							point6 = new Point((float)num7, (float)num8);
							if (TerrainInfo.cells[(int)point5.x, (int)point5.y].river_offset == 0 && TerrainInfo.cells[(int)point6.x, (int)point6.y].river_offset == 0)
							{
								byte b2 = 0;
								b2 |= (byte)(((Math.Sign(coord3.x) == 1) ? 1 : 0) << 7);
								b2 |= (byte)(Math.Abs(coord3.x) << 4);
								b2 |= (byte)(((Math.Sign(coord3.y) == 1) ? 1 : 0) << 3);
								b2 |= (byte)Math.Abs(coord3.y);
								TerrainInfo.cells[(int)point5.x, (int)point5.y].river_offset = b2;
								coord3 *= -1;
								b2 = 0;
								b2 |= (byte)(((Math.Sign(coord3.x) == 1) ? 1 : 0) << 7);
								b2 |= (byte)(Math.Abs(coord3.x) << 4);
								b2 |= (byte)(((Math.Sign(coord3.y) == 1) ? 1 : 0) << 3);
								b2 |= (byte)Math.Abs(coord3.y);
								TerrainInfo.cells[(int)point6.x, (int)point6.y].river_offset = b2;
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x000D67F0 File Offset: 0x000D49F0
	public static void AddRivers(bool use_yofs = false)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return;
		}
		Shader shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (shader == null)
		{
			return;
		}
		Camera camera = global::Common.CreateTerrainCam(activeTerrain);
		if (camera == null)
		{
			return;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(TerrainInfo.rt_res, TerrainInfo.rt_res, 0);
		camera.targetTexture = renderTexture;
		Shader.SetGlobalInt("_ARPass", 0);
		Shader.SetGlobalColor("_ARColor", Color.white);
		camera.clearFlags = CameraClearFlags.Color;
		camera.cullingMask = 1 << LayerMask.NameToLayer("Rivers");
		camera.RenderWithShader(shader, "");
		int i = TerrainInfo.rt_res;
		while (i > TerrainInfo.tex_res)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(renderTexture, temporary);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Texture2D texture2D = new Texture2D(TerrainInfo.tex_res, TerrainInfo.tex_res, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)TerrainInfo.tex_res, (float)TerrainInfo.tex_res), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		RenderTexture.ReleaseTemporary(renderTexture);
		Vector3 size = activeTerrain.terrainData.size;
		int num = use_yofs ? Mathf.RoundToInt(Mathf.Abs(size.x - size.z) * (float)(TerrainInfo.tex_res - 1) / (2f * Mathf.Max(size.x, size.z))) : 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < TerrainInfo.tex_res - num; j++)
		{
			int num2 = (j - num) * TerrainInfo.height / (TerrainInfo.tex_res - 2 * num);
			for (int k = 0; k < TerrainInfo.tex_res; k++)
			{
				if (pixels[j * TerrainInfo.tex_res + k].r > 0f)
				{
					int num3 = k * TerrainInfo.width / TerrainInfo.tex_res;
					ref TerrainInfo.Cell ptr = ref TerrainInfo.cells[num3, num2];
					ptr.rivers += 1;
					TerrainInfo.river_tiles++;
				}
			}
		}
		TerrainInfo.AddRiverCrossings();
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x000D6A10 File Offset: 0x000D4C10
	public static void AddMountains(bool use_yofs = false)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return;
		}
		Shader shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (shader == null)
		{
			return;
		}
		Camera camera = global::Common.CreateTerrainCam(activeTerrain);
		if (camera == null)
		{
			return;
		}
		RenderTexture renderTexture = RenderTexture.GetTemporary(TerrainInfo.rt_res, TerrainInfo.rt_res, 0);
		camera.targetTexture = renderTexture;
		Shader.SetGlobalInt("_ARPass", 0);
		Shader.SetGlobalColor("_ARColor", Color.white);
		camera.clearFlags = CameraClearFlags.Color;
		camera.cullingMask = (1 << LayerMask.NameToLayer("Mountains") | 1 << LayerMask.NameToLayer("Rocks"));
		camera.RenderWithShader(shader, "");
		int i = TerrainInfo.rt_res;
		while (i > TerrainInfo.tex_res)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(renderTexture, temporary);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Texture2D texture2D = new Texture2D(TerrainInfo.tex_res, TerrainInfo.tex_res, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)TerrainInfo.tex_res, (float)TerrainInfo.tex_res), 0, 0);
		RenderTexture.active = null;
		camera.targetTexture = null;
		RenderTexture.ReleaseTemporary(renderTexture);
		Vector3 size = activeTerrain.terrainData.size;
		int num = use_yofs ? Mathf.RoundToInt(Mathf.Abs(size.x - size.z) * (float)(TerrainInfo.tex_res - 1) / (2f * Mathf.Max(size.x, size.z))) : 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < TerrainInfo.tex_res - num; j++)
		{
			int num2 = (j - num) * TerrainInfo.height / (TerrainInfo.tex_res - 2 * num);
			for (int k = 0; k < TerrainInfo.tex_res; k++)
			{
				if (pixels[j * TerrainInfo.tex_res + k].r > 0f)
				{
					int num3 = k * TerrainInfo.width / TerrainInfo.tex_res;
					TerrainInfo.cells[num3, num2].slope = TerrainInfo.mountain_slope;
				}
			}
		}
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000D6C28 File Offset: 0x000D4E28
	public static void AddWalls()
	{
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			Wall wall = settlement.wall;
			if (!(wall == null))
			{
				bool flag = false;
				for (int i = 0; i < wall.corners.Count; i++)
				{
					if (wall.corners[i].type == WallCorner.Type.Gate)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					for (int j = 0; j < wall.corners.Count; j++)
					{
						WallCorner wallCorner = wall.corners[j];
						WallCorner wallCorner2 = wall.corners[(j + 1) % wall.corners.Count];
						Vector3 vector;
						Vector3 v;
						wallCorner.GetExtents(out vector, out v);
						Vector3 v2;
						Vector3 vector2;
						wallCorner2.GetExtents(out v2, out vector2);
						Point point = v;
						Point pt = v2;
						Coord tile = Coord.WorldToGrid(point, 1f);
						Point point2 = Coord.WorldToLocal(tile, point, 1f);
						Point point3 = Coord.WorldToLocal(tile, pt, 1f);
						Coord invalid = Coord.Invalid;
						Coord invalid2 = Coord.Invalid;
						do
						{
							point = Coord.GridToWorld(tile, 1f);
							int num = Mathf.RoundToInt(point.x);
							int num2 = Mathf.RoundToInt(point.y);
							TerrainInfo.Cell cell = TerrainInfo.cells[num, num2];
							cell.wall = true;
							TerrainInfo.cells[num, num2] = cell;
							if (invalid.valid)
							{
								point = Coord.GridToWorld(invalid, 1f);
								num = Mathf.RoundToInt(point.x);
								num2 = Mathf.RoundToInt(point.y);
								cell = TerrainInfo.cells[num, num2];
								cell.wall = true;
								TerrainInfo.cells[num, num2] = cell;
							}
							if (invalid2.valid)
							{
								point = Coord.GridToWorld(invalid2, 1f);
								num = Mathf.RoundToInt(point.x);
								num2 = Mathf.RoundToInt(point.y);
								cell = TerrainInfo.cells[num, num2];
								cell.wall = true;
								TerrainInfo.cells[num, num2] = cell;
							}
						}
						while (Coord.RayStep(ref tile, ref point2, ref point3, 0f, out invalid, out invalid2));
					}
				}
			}
			settlement = settlement.Next();
		}
	}

	// Token: 0x04000D82 RID: 3458
	public static float cell_size = 1f;

	// Token: 0x04000D83 RID: 3459
	public static int rt_res = 16384;

	// Token: 0x04000D84 RID: 3460
	public static int tex_res = 4096;

	// Token: 0x04000D85 RID: 3461
	public static byte mountain_slope = 100;

	// Token: 0x04000D86 RID: 3462
	public static byte lake_slope = 101;

	// Token: 0x04000D87 RID: 3463
	public static byte out_of_bounds_slope = 102;

	// Token: 0x04000D88 RID: 3464
	public const int UpdateNone = 0;

	// Token: 0x04000D89 RID: 3465
	public const int UpdateHeight = 1;

	// Token: 0x04000D8A RID: 3466
	public const int UpdateSlope = 2;

	// Token: 0x04000D8B RID: 3467
	public const int UpdateAll = 255;

	// Token: 0x04000D8C RID: 3468
	public static int height_tiles = 0;

	// Token: 0x04000D8D RID: 3469
	public static int slope_tiles = 0;

	// Token: 0x04000D8E RID: 3470
	public static int road_tiles = 0;

	// Token: 0x04000D8F RID: 3471
	public static int river_tiles = 0;

	// Token: 0x04000D90 RID: 3472
	private static int _version = 0;

	// Token: 0x04000D91 RID: 3473
	private static Rect rcWorld = Rect.zero;

	// Token: 0x04000D92 RID: 3474
	private static TerrainInfo.Cell[,] cells = null;

	// Token: 0x04000D93 RID: 3475
	private static TerrainData terrain_data = null;

	// Token: 0x020006BA RID: 1722
	[Serializable]
	public struct Coords
	{
		// Token: 0x0600487E RID: 18558 RVA: 0x00217E88 File Offset: 0x00216088
		public Coords(int x = 0, int y = 0)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x0600487F RID: 18559 RVA: 0x00217E98 File Offset: 0x00216098
		public override string ToString()
		{
			return this.x + ", " + this.y;
		}

		// Token: 0x040036BD RID: 14013
		public int x;

		// Token: 0x040036BE RID: 14014
		public int y;
	}

	// Token: 0x020006BB RID: 1723
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Cell
	{
		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06004880 RID: 18560 RVA: 0x00217EBA File Offset: 0x002160BA
		public float height
		{
			get
			{
				return (float)this.bt_height * 32f / 254f;
			}
		}

		// Token: 0x06004881 RID: 18561 RVA: 0x00217ECF File Offset: 0x002160CF
		public void Reset()
		{
			this.bt_height = byte.MaxValue;
			this.slope = byte.MaxValue;
			this.roads = 0;
			this.rivers = 0;
			this.river_offset = 0;
		}

		// Token: 0x040036BF RID: 14015
		public const float MAX_HEIGHT = 32f;

		// Token: 0x040036C0 RID: 14016
		public byte bt_height;

		// Token: 0x040036C1 RID: 14017
		public byte slope;

		// Token: 0x040036C2 RID: 14018
		public byte roads;

		// Token: 0x040036C3 RID: 14019
		public byte rivers;

		// Token: 0x040036C4 RID: 14020
		public byte river_offset;

		// Token: 0x040036C5 RID: 14021
		public bool wall;
	}
}
