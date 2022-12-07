using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Logic;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class PathDataBuilder
{
	// Token: 0x0600154F RID: 5455 RVA: 0x000D7F64 File Offset: 0x000D6164
	public PathDataBuilder(global::PathFinding pf)
	{
		this.path_finding = pf;
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000D8084 File Offset: 0x000D6284
	public void Build(string path, bool save_tex, bool build_LSA)
	{
		this.path = path;
		this.save_tex = save_tex;
		bool flag = false;
		try
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			flag = !this.Init();
			if (!flag)
			{
				flag = !this.BuildTerrain();
			}
			if (!flag)
			{
				flag = !this.BuildMasks();
			}
			if (!flag)
			{
				flag = !this.BuildRoads();
			}
			if (!flag)
			{
				flag = !this.BuildWalls();
			}
			if (!flag)
			{
				flag = !this.BuildMountains();
			}
			if (!flag)
			{
				flag = !this.BuildRivers();
			}
			if (!flag)
			{
				flag = !this.buildCoast();
			}
			if (!flag && build_LSA)
			{
				flag = !this.BuildLSA();
			}
			if (!flag)
			{
				flag = !this.GenerateHeights();
			}
			if (!flag)
			{
				flag = !this.GeneratePassability();
			}
			if (!flag)
			{
				this.Save(build_LSA);
			}
			if (!flag)
			{
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				Debug.Log("Pathfinding data build in " + (float)elapsedMilliseconds / 1000f + " seconds");
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			flag = true;
		}
		global::Common.EditorProgress(null, null, 0f, false);
		BuildTools.cancelled = flag;
		if (this.cam != null)
		{
			this.cam.targetTexture = null;
		}
		if (this.rt != null)
		{
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = null;
		}
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x000D81DC File Offset: 0x000D63DC
	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < this.width && y >= 0 && y < this.height;
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x000D81FA File Offset: 0x000D63FA
	public void WorldToGrid(Vector3 point, out int x, out int y)
	{
		x = (int)(point.x / this.path_finding.settings.tile_size);
		y = (int)(point.z / this.path_finding.settings.tile_size);
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x000D8230 File Offset: 0x000D6430
	public PathDataBuilder.Cell GetCell(int x, int y)
	{
		return this.cells[this.GetIdx(x, y)];
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x000D8248 File Offset: 0x000D6448
	public PathDataBuilder.Cell GetCell(Vector3 point)
	{
		int x;
		int y;
		this.WorldToGrid(point, out x, out y);
		return this.GetCell(x, y);
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x000D8268 File Offset: 0x000D6468
	public int GetIdx(int x, int y)
	{
		return y * this.width + x;
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000D8274 File Offset: 0x000D6474
	private bool Init()
	{
		this.terrain = Terrain.activeTerrain;
		if (this.terrain == null)
		{
			return false;
		}
		this.shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (this.shader == null)
		{
			return false;
		}
		this.cam = global::Common.CreateTerrainCam(this.terrain);
		if (this.cam == null)
		{
			return false;
		}
		Vector3 size = this.terrain.terrainData.size;
		this.width = (int)(size.x / this.path_finding.settings.tile_size);
		this.height = (int)(size.z / this.path_finding.settings.tile_size);
		this.cells = new PathDataBuilder.Cell[this.width * this.height];
		this.water_level = MapData.GetWaterLevel();
		return true;
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x000D8350 File Offset: 0x000D6550
	private void Cleanup()
	{
		if (this.cam != null)
		{
			this.cam.targetTexture = null;
		}
		if (this.rt != null)
		{
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = null;
		}
		this.cells = null;
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x000D83A0 File Offset: 0x000D65A0
	private bool BuildTerrain()
	{
		TerrainData terrainData = this.terrain.terrainData;
		int heightmapResolution = terrainData.heightmapResolution;
		for (int i = 0; i < heightmapResolution; i++)
		{
			if (i % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Processing terrain", (float)i / (float)heightmapResolution, true))
			{
				return false;
			}
			int y = i * this.height / heightmapResolution;
			float y2 = (float)i / (float)(heightmapResolution - 1);
			for (int j = 0; j < heightmapResolution; j++)
			{
				int x = j * this.width / heightmapResolution;
				float x2 = (float)j / (float)(heightmapResolution - 1);
				float num = terrainData.GetHeight(j, i);
				if (num < 0.01f)
				{
					num = 0.01f;
				}
				byte b;
				if (i == 0 || i == heightmapResolution - 1 || j == 0 || j == heightmapResolution - 1)
				{
					b = TerrainInfo.out_of_bounds_slope;
				}
				else
				{
					b = (byte)terrainData.GetSteepness(x2, y2);
				}
				int idx = this.GetIdx(x, y);
				PathDataBuilder.Cell cell = this.cells[idx];
				if (cell.max_height == 0f || num < cell.min_height)
				{
					cell.min_height = num;
				}
				if (num > cell.max_height)
				{
					cell.max_height = num;
				}
				if (b > cell.slope)
				{
					cell.slope = b;
				}
				if (cell.min_height <= this.water_level)
				{
					cell.bits |= 8;
				}
				this.cells[idx] = cell;
			}
		}
		return true;
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x000D8508 File Offset: 0x000D6708
	private bool BuildRoads()
	{
		float num = 0f;
		AutoRoads autoRoads = UnityEngine.Object.FindObjectOfType<AutoRoads>();
		if (autoRoads != null)
		{
			LinesBatching component = autoRoads.GetComponent<LinesBatching>();
			if (component != null)
			{
				bool flag = component.pos_rots_binary != null && component.pos_rots_binary.Count > 0;
				if (!flag)
				{
					flag = component.LoadPoints(null, true);
				}
				if (flag)
				{
					for (int i = 0; i < component.pos_rots_binary.Count; i++)
					{
						Vector3 vector = new Vector3(component.pos_rots_binary[i].x, component.pos_rots_binary[i].y, component.pos_rots_binary[i].z);
						int x = Mathf.RoundToInt(vector.x / this.path_finding.settings.tile_size);
						int y = Mathf.RoundToInt(vector.z / this.path_finding.settings.tile_size);
						int idx = this.GetIdx(x, y);
						if (idx >= 0 && idx < this.cells.Length)
						{
							this.cells[idx].bits = (byte)((int)this.cells[idx].bits & -3);
							PathDataBuilder.Cell[] array = this.cells;
							int num2 = idx;
							array[num2].bits = (array[num2].bits | 1);
						}
					}
					return true;
				}
			}
		}
		TerrainPath terrainPath = TerrainPath.First();
		while (terrainPath != null)
		{
			num += 1f;
			if (!terrainPath.settings.river)
			{
				if (!global::Common.EditorProgress("Build Path Data", "Processing roads", num / (float)TerrainPath.Count(), true))
				{
					return false;
				}
				PathWalker pathWalker = new PathWalker();
				pathWalker.SetPath(terrainPath.path_points, -1f);
				float num3 = this.path_finding.settings.tile_size / 2f;
				float num4 = 0f;
				for (;;)
				{
					Vector3 vector2;
					Vector3 vector3;
					bool flag2 = !pathWalker.GetPathPoint(num4, out vector2, out vector3, true, 0f);
					int x2 = Mathf.RoundToInt(vector2.x / this.path_finding.settings.tile_size);
					int y2 = Mathf.RoundToInt(vector2.z / this.path_finding.settings.tile_size);
					int idx2 = this.GetIdx(x2, y2);
					if (idx2 >= 0 && idx2 < this.cells.Length)
					{
						this.cells[idx2].bits = (byte)((int)this.cells[idx2].bits & -3);
						PathDataBuilder.Cell[] array2 = this.cells;
						int num5 = idx2;
						array2[num5].bits = (array2[num5].bits | 1);
					}
					if (flag2)
					{
						break;
					}
					num4 += num3;
				}
			}
			terrainPath = terrainPath.Next();
		}
		return true;
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x000D87B4 File Offset: 0x000D69B4
	private bool BuildWalls()
	{
		if (!global::Common.EditorProgress("Build Wall passability", "Processing walls", 0f, true))
		{
			return false;
		}
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
							int x = Mathf.RoundToInt(point.x / this.path_finding.settings.tile_size);
							int y = Mathf.RoundToInt(point.y / this.path_finding.settings.tile_size);
							int idx = this.GetIdx(x, y);
							PathDataBuilder.Cell cell = this.cells[idx];
							cell.bits |= 64;
							this.cells[idx] = cell;
							if (invalid.valid)
							{
								point = Coord.GridToWorld(invalid, 1f);
								x = Mathf.RoundToInt(point.x / this.path_finding.settings.tile_size);
								y = Mathf.RoundToInt(point.y / this.path_finding.settings.tile_size);
								idx = this.GetIdx(x, y);
								cell = this.cells[idx];
								cell.bits |= 64;
								this.cells[idx] = cell;
							}
							if (invalid2.valid)
							{
								point = Coord.GridToWorld(invalid2, 1f);
								x = Mathf.RoundToInt(point.x / this.path_finding.settings.tile_size);
								y = Mathf.RoundToInt(point.y / this.path_finding.settings.tile_size);
								idx = this.GetIdx(x, y);
								cell = this.cells[idx];
								cell.bits |= 64;
								this.cells[idx] = cell;
							}
						}
						while (Coord.RayStep(ref tile, ref point2, ref point3, 0f, out invalid, out invalid2));
					}
				}
			}
			settlement = settlement.Next();
		}
		return true;
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x000D8AAC File Offset: 0x000D6CAC
	private bool BuildRivers()
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
					int idx;
					do
					{
						int x = Mathf.RoundToInt(point2.x / this.path_finding.settings.tile_size);
						int y = Mathf.RoundToInt(point2.y / this.path_finding.settings.tile_size);
						idx = this.GetIdx(x, y);
						coord = new Coord(point2);
						if (idx < 0 || idx >= this.cells.Length || (this.cells[idx].bits & 2) == 0)
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
					int idx2;
					do
					{
						int x2 = Mathf.RoundToInt(point2.x / this.path_finding.settings.tile_size);
						int y2 = Mathf.RoundToInt(point2.y / this.path_finding.settings.tile_size);
						idx2 = this.GetIdx(x2, y2);
						coord2 = new Coord(point2);
						if (idx2 < 0 || idx2 >= this.cells.Length || (this.cells[idx2].bits & 2) == 0)
						{
							break;
						}
						point2 = Coord.GridToWorld(tile, 1f);
					}
					while (Coord.RayStep(ref tile, ref point3, ref point4, 0f, out invalid, out invalid2));
					if (!(coord2 == coord) && idx >= 0 && idx < this.cells.Length && idx2 >= 0 && idx2 < this.cells.Length)
					{
						Coord coord3 = (coord2 - coord) * 2 / 3;
						if (Math.Abs(coord3.x) <= 7 && Math.Abs(coord3.y) <= 7)
						{
							coord = (coord + coord2) / 2 - coord3 * 3 / 2;
							coord2 = coord + coord3 * 3;
							int x3 = Mathf.RoundToInt((float)coord.x / this.path_finding.settings.tile_size);
							int y3 = Mathf.RoundToInt((float)coord.y / this.path_finding.settings.tile_size);
							idx = this.GetIdx(x3, y3);
							x3 = Mathf.RoundToInt((float)coord2.x / this.path_finding.settings.tile_size);
							y3 = Mathf.RoundToInt((float)coord2.y / this.path_finding.settings.tile_size);
							idx2 = this.GetIdx(x3, y3);
							if (this.cells[idx].river_offset == 0 && this.cells[idx2].river_offset == 0 && (this.cells[idx].bits & 24) == 0 && (this.cells[idx].bits & 32) == 0 && (this.cells[idx2].bits & 24) == 0 && (this.cells[idx2].bits & 32) == 0)
							{
								byte b2 = 0;
								b2 |= (byte)(((Math.Sign(coord3.x) == 1) ? 1 : 0) << 7);
								b2 |= (byte)(Math.Abs(coord3.x) << 4);
								b2 |= (byte)(((Math.Sign(coord3.y) == 1) ? 1 : 0) << 3);
								b2 |= (byte)Math.Abs(coord3.y);
								this.cells[idx].river_offset = b2;
								coord3 *= -1;
								b2 = 0;
								b2 |= (byte)(((Math.Sign(coord3.x) == 1) ? 1 : 0) << 7);
								b2 |= (byte)(Math.Abs(coord3.x) << 4);
								b2 |= (byte)(((Math.Sign(coord3.y) == 1) ? 1 : 0) << 3);
								b2 |= (byte)Math.Abs(coord3.y);
								this.cells[idx2].river_offset = b2;
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x000D905C File Offset: 0x000D725C
	private void Render(string layer, Color clr, bool clear = false)
	{
		Shader.SetGlobalInt("_ARPass", 0);
		Shader.SetGlobalColor("_ARColor", clr);
		this.cam.clearFlags = (clear ? CameraClearFlags.Color : CameraClearFlags.Nothing);
		this.cam.cullingMask = 1 << LayerMask.NameToLayer(layer);
		this.cam.RenderWithShader(this.shader, "");
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000D90C0 File Offset: 0x000D72C0
	private unsafe bool GenerateHeights()
	{
		if (!global::Common.EditorProgress("Build Path Data", "Generating heights", 0f, true))
		{
			return false;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return true;
		}
		TerrainData terrainData = activeTerrain.terrainData;
		HeightsGrid heightsGrid = new HeightsGrid();
		int num = terrainData.heightmapResolution - 1;
		heightsGrid.Alloc(num, num, terrainData.size.x, terrainData.size.z, terrainData.size.y, 0f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float num2 = terrainData.GetHeight(j, i);
				heightsGrid.data->SetHeight(j, i, num2);
			}
		}
		heightsGrid.Save(this.path + "/heights.bin");
		heightsGrid.Dispose();
		return true;
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000D9198 File Offset: 0x000D7398
	private unsafe bool GeneratePassability()
	{
		if (!global::Common.EditorProgress("Build Path Data", "Generating passability", 0f, true))
		{
			return false;
		}
		PassabilityGrid passabilityGrid = new PassabilityGrid();
		passabilityGrid.Alloc(this.width, this.height, this.path_finding.settings.tile_size);
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				bool passable = this.IsPassable(j, i);
				passabilityGrid.data->SetPassable(j, i, passable);
			}
		}
		passabilityGrid.Save(this.path + "/passability.bin");
		if (this.save_tex)
		{
			Color[] array = new Color[this.width * this.height];
			int num = 0;
			for (int k = 0; k < this.height; k++)
			{
				for (int l = 0; l < this.width; l++)
				{
					bool flag = passabilityGrid.data->IsPassable(new Coord(l, k));
					array[num++] = (flag ? Color.green : Color.red);
				}
			}
			Texture2D texture2D = new Texture2D(this.width, this.height);
			texture2D.SetPixels(array);
			File.WriteAllBytes(this.path + "/passability.png", texture2D.EncodeToPNG());
		}
		passabilityGrid.Dispose();
		return true;
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x000D92F4 File Offset: 0x000D74F4
	private bool LSACheck(int idx, int oceanMask)
	{
		PathDataBuilder.Cell cell = this.cells[idx];
		return (cell.slope <= this.path_finding.settings.max_slope || (cell.bits & 5) != 0) && cell.lsa_level == byte.MaxValue && (int)(cell.bits & 8) == oceanMask;
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x000D934C File Offset: 0x000D754C
	private void LSAWave(int idx, int oceanMask)
	{
		Queue<int> queue = new Queue<int>();
		queue.Enqueue(idx);
		bool flag = oceanMask > 0;
		while (queue.Count > 0)
		{
			int num = queue.Dequeue();
			int num2 = num % this.width;
			int num3 = num / this.width;
			PathDataBuilder.Cell cell = this.cells[num];
			if (this.LSACheck(num, oceanMask))
			{
				if (oceanMask > 0)
				{
					this.cells[num].lsa_level = 1;
				}
				else
				{
					this.cells[num].lsa_level = this.lsa_level;
				}
				if ((cell.bits & 32) != 0)
				{
					flag = true;
				}
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if ((i != 0 || j != 0) && this.IsInBounds(num2 + j, num3 + i) && this.LSACheck(this.GetIdx(num2 + j, num3 + i), oceanMask))
						{
							queue.Enqueue(this.GetIdx(num2 + j, num3 + i));
						}
					}
				}
			}
		}
		if (oceanMask > 0)
		{
			return;
		}
		if (!flag)
		{
			queue.Enqueue(idx);
			while (queue.Count > 0)
			{
				int num4 = queue.Dequeue();
				int num5 = num4 % this.width;
				int num6 = num4 / this.width;
				if (this.cells[num4].lsa_level == this.lsa_level)
				{
					this.cells[num4].lsa_level = 0;
					for (int k = -1; k <= 1; k++)
					{
						for (int l = -1; l <= 1; l++)
						{
							if ((k != 0 || l != 0) && this.IsInBounds(num5 + l, num6 + k) && this.cells[this.GetIdx(num5 + l, num6 + k)].lsa_level == this.lsa_level)
							{
								queue.Enqueue(this.GetIdx(num5 + l, num6 + k));
							}
						}
					}
				}
			}
		}
		if (flag)
		{
			this.lsa_level += 1;
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000D9554 File Offset: 0x000D7754
	private bool BuildLSA()
	{
		this.lsa_level = 2;
		for (int i = 0; i < this.cells.Length; i++)
		{
			if (i % 10000 == 0 && !global::Common.EditorProgress("Build Path Data", "LSA preparation", (float)i / (float)this.cells.Length, true))
			{
				return false;
			}
			PathDataBuilder.Cell cell = this.cells[i];
			cell.lsa_level = byte.MaxValue;
			this.cells[i] = cell;
		}
		for (int j = 0; j < this.cells.Length; j++)
		{
			if (j % 10000 == 0 && !global::Common.EditorProgress("Build Path Data", "LSA", (float)j / (float)this.cells.Length, true))
			{
				return false;
			}
			PathDataBuilder.Cell cell2 = this.cells[j];
			int oceanMask = (int)(cell2.bits & 8);
			if (cell2.lsa_level == 255)
			{
				if (this.lsa_level == 255)
				{
					Debug.LogError("LSA LEVEL OVERFLOW!!!");
					break;
				}
				if (this.LSACheck(j, oceanMask))
				{
					this.LSAWave(j, oceanMask);
				}
				if (cell2.lsa_level == 255)
				{
					this.cells[j].lsa_level = 0;
				}
			}
		}
		this.has_lsa_generated = true;
		return true;
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000D9684 File Offset: 0x000D7884
	private bool BuildMasks()
	{
		if (!global::Common.EditorProgress("Build Path Data", "Rendering masks", 0f, true))
		{
			return false;
		}
		global::Settlement.SpawnAll();
		this.rt = RenderTexture.GetTemporary(16384, 16384, 0);
		this.cam.targetTexture = this.rt;
		this.Render("Rivers", new Color(0f, 0f, 1f), true);
		this.Render("Settlements", new Color(0f, 1f, 0f), false);
		this.Render("Lakes", new Color(1f, 0f, 0f), false);
		int i = 16384;
		while (i > 4096)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(this.rt, temporary);
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = temporary;
		}
		if (!global::Common.EditorProgress("Build Path Data", "Downloading masks texture", 0f, true))
		{
			this.cam.targetTexture = null;
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = null;
			return false;
		}
		Texture2D texture2D = new Texture2D(4096, 4096, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 4096f, 4096f), 0, 0);
		RenderTexture.active = null;
		this.cam.targetTexture = null;
		RenderTexture.ReleaseTemporary(this.rt);
		this.rt = null;
		if (this.save_tex)
		{
			if (!global::Common.EditorProgress("Build Path Data", "Saving masks texture", 0f, true))
			{
				return false;
			}
			File.WriteAllBytes(this.path + "/pathmasks.png", texture2D.EncodeToPNG());
		}
		Vector3 size = this.terrain.terrainData.size;
		int num = 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < 4096 - num; j++)
		{
			int num2 = (j - num) * this.height / (4096 - 2 * num);
			if (num2 % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Processing rivers, lakes, settlements", (float)num2 / (float)this.height, true))
			{
				return false;
			}
			for (int k = 0; k < 4096; k++)
			{
				Color color = pixels[j * 4096 + k];
				if (!(color == Color.black))
				{
					int num3 = k * this.width / 4096;
					int idx = this.GetIdx(num3, num2);
					PathDataBuilder.Cell cell = this.cells[idx];
					if (color.r > 0f)
					{
						cell.bits |= 16;
					}
					if (color.g > 0f)
					{
						cell.bits |= 32;
						if (this.path_finding.settings.expand_towns || this.path_finding.settings.towns_passable)
						{
							if (num2 > 0)
							{
								PathDataBuilder.Cell[] array = this.cells;
								int num4 = idx - this.width;
								array[num4].bits = (array[num4].bits | 32);
							}
							if (num3 > 0)
							{
								PathDataBuilder.Cell[] array2 = this.cells;
								int num5 = idx - 1;
								array2[num5].bits = (array2[num5].bits | 32);
							}
							if (num3 + 1 < this.width)
							{
								PathDataBuilder.Cell[] array3 = this.cells;
								int num6 = idx + 1;
								array3[num6].bits = (array3[num6].bits | 32);
							}
							if (num2 + 1 < this.height)
							{
								PathDataBuilder.Cell[] array4 = this.cells;
								int num7 = idx + this.width;
								array4[num7].bits = (array4[num7].bits | 32);
							}
						}
					}
					if (color.b > 0f && (cell.bits & 8) == 0)
					{
						cell.bits |= 2;
					}
					this.cells[idx] = cell;
				}
			}
		}
		return true;
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000D9A58 File Offset: 0x000D7C58
	private bool BuildMountains()
	{
		if (!global::Common.EditorProgress("Build Path Data", "Rendering rocks and mountains", 0f, true))
		{
			return false;
		}
		this.rt = RenderTexture.GetTemporary(16384, 16384, 0);
		this.cam.targetTexture = this.rt;
		this.Render("Rocks", new Color(0f, 0f, 1f), true);
		this.Render("Mountains", new Color(1f, 0f, 0f), false);
		int i = 16384;
		while (i > 4096)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(this.rt, temporary);
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = temporary;
		}
		if (!global::Common.EditorProgress("Build Path Data", "Downloading masks texture", 0f, true))
		{
			this.cam.targetTexture = null;
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = null;
			return false;
		}
		Texture2D texture2D = new Texture2D(4096, 4096, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 4096f, 4096f), 0, 0);
		RenderTexture.active = null;
		this.cam.targetTexture = null;
		RenderTexture.ReleaseTemporary(this.rt);
		this.rt = null;
		if (this.save_tex)
		{
			if (!global::Common.EditorProgress("Build Path Data", "Saving masks texture", 0f, true))
			{
				return false;
			}
			File.WriteAllBytes(this.path + "/mountains.png", texture2D.EncodeToPNG());
		}
		Vector3 size = this.terrain.terrainData.size;
		int num = 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < 4096 - num; j++)
		{
			int num2 = (j - num) * this.height / (4096 - 2 * num);
			if (num2 % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Processing rocks and mountains", (float)num2 / (float)this.height, true))
			{
				return false;
			}
			for (int k = 0; k < 4096; k++)
			{
				Color color = pixels[j * 4096 + k];
				if (!(color == Color.black))
				{
					int x = k * this.width / 4096;
					int idx = this.GetIdx(x, num2);
					PathDataBuilder.Cell cell = this.cells[idx];
					if (color.r > 0f || color.b > 0f)
					{
						cell.slope = TerrainInfo.mountain_slope;
						this.cells[idx] = cell;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x000D9D0C File Offset: 0x000D7F0C
	private bool IsPassable(int idx)
	{
		if (idx < 0 || idx >= this.cells.Length)
		{
			return false;
		}
		PathDataBuilder.Cell cell = this.cells[idx];
		return cell.lsa_level != byte.MaxValue && cell.lsa_level != 0 && (cell.bits & 26) == 0 && ((cell.bits & 32) == 0 || this.path_finding.settings.towns_passable) && (cell.slope <= this.path_finding.settings.max_slope || (cell.bits & 1) != 0);
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x000D9DA0 File Offset: 0x000D7FA0
	public bool IsPassable(int cx, int cy)
	{
		int idx = this.GetIdx(cx, cy);
		return this.IsPassable(idx);
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x000D9DC0 File Offset: 0x000D7FC0
	private int TraceClearance(int idx, int ofs, int cnt)
	{
		for (int i = 1; i <= cnt; i++)
		{
			if (!this.IsPassable(idx + i * ofs))
			{
				return i - 1;
			}
		}
		return cnt;
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x000D9DEC File Offset: 0x000D7FEC
	private void TraceClearance(int idx, int ofs, int cnt, float ts, ref float min)
	{
		float num = (float)this.TraceClearance(idx, ofs, cnt) * ts;
		if (num < min)
		{
			min = num;
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x000D9E14 File Offset: 0x000D8014
	private float CalcClearance(int idx)
	{
		float max_radius = this.path_finding.settings.max_radius;
		float num = this.path_finding.settings.tile_size;
		int cnt = Mathf.CeilToInt(this.path_finding.settings.max_radius / num);
		this.TraceClearance(idx, 1, cnt, num, ref max_radius);
		this.TraceClearance(idx, -1, cnt, num, ref max_radius);
		this.TraceClearance(idx, this.width, cnt, num, ref max_radius);
		this.TraceClearance(idx, -this.width, cnt, num, ref max_radius);
		num *= 1.4142135f;
		cnt = Mathf.CeilToInt(this.path_finding.settings.max_radius / num);
		this.TraceClearance(idx, this.width - 1, cnt, num, ref max_radius);
		this.TraceClearance(idx, this.width + 1, cnt, num, ref max_radius);
		this.TraceClearance(idx, -this.width - 1, cnt, num, ref max_radius);
		this.TraceClearance(idx, -this.width + 1, cnt, num, ref max_radius);
		return max_radius;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000D9F08 File Offset: 0x000D8108
	private bool buildCoast()
	{
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				int idx = this.GetIdx(i, j);
				PathDataBuilder.Cell cell = this.cells[idx];
				if ((cell.bits & 8) != 0)
				{
					bool flag = false;
					for (int k = i - 1; k <= i + 1; k++)
					{
						for (int l = j - 1; l <= j + 1; l++)
						{
							if (k >= 0 && k < this.width && l >= 0 && l < this.height && (k != i || l != j))
							{
								int idx2 = this.GetIdx(k, l);
								if ((this.cells[idx2].bits & 14) == 0)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (flag)
					{
						cell.bits = (byte)((int)cell.bits & -9);
						cell.bits |= 4;
						this.cells[idx] = cell;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000DA014 File Offset: 0x000D8214
	private bool SaveTex()
	{
		if (!this.save_tex)
		{
			return true;
		}
		Color[] array = new Color[this.width * this.height];
		int num = 0;
		for (int i = 0; i < this.height; i++)
		{
			if (i % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Saving texture", (float)i / (float)this.height, true))
			{
				return false;
			}
			for (int j = 0; j < this.width; j++)
			{
				PathDataBuilder.Cell cell = this.cells[num];
				Color color;
				if ((cell.bits & 4) != 0 && (cell.bits & 8) != 0)
				{
					color = this.clrCoastOcean;
				}
				else if ((cell.bits & 4) != 0)
				{
					color = this.clrCoast;
				}
				else if ((cell.bits & 16) != 0)
				{
					color = this.clrLakes;
				}
				else if ((cell.bits & 2) != 0)
				{
					color = this.clrRivers;
				}
				else if ((cell.bits & 8) != 0)
				{
					color = this.clrOcean;
				}
				else if ((cell.bits & 1) != 0)
				{
					color = this.clrRoads;
				}
				else if ((cell.bits & 64) != 0)
				{
					color = this.clrImpassable;
				}
				else if (cell.slope > this.path_finding.settings.max_slope)
				{
					color = this.clrImpassable;
				}
				else if (cell.lsa_level == 0)
				{
					color = this.clrImpassable;
				}
				else if ((cell.bits & 32) != 0)
				{
					color = this.clrTowns;
				}
				else if (this.path_finding.settings.max_radius <= 0f)
				{
					float num2 = (float)cell.slope / (float)this.path_finding.settings.max_slope;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					color = Color.Lerp(this.clrPassable, this.clrSlope, num2);
				}
				else
				{
					float t = this.CalcClearance(num) / this.path_finding.settings.max_radius;
					color = Color.Lerp(this.clrSlope, this.clrPassable, t);
				}
				array[num] = color;
				num++;
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height);
		texture2D.SetPixels(array);
		File.WriteAllBytes(this.path + "/pathfinding.png", texture2D.EncodeToPNG());
		return true;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000DA27C File Offset: 0x000D847C
	private bool SaveLSATex()
	{
		Color[] array = new Color[this.width * this.height];
		Color[] array2 = new Color[(int)this.lsa_level];
		for (int i = 0; i < (int)this.lsa_level; i++)
		{
			array2[i] = new Color(Random.value, Random.value, Random.value);
		}
		int num = 0;
		for (int j = 0; j < this.height; j++)
		{
			if (j % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Saving LSA texture", (float)j / (float)this.height, true))
			{
				return false;
			}
			for (int k = 0; k < this.width; k++)
			{
				PathDataBuilder.Cell cell = this.cells[this.GetIdx(k, j)];
				Color color = (cell.lsa_level == 0) ? Color.red : ((cell.lsa_level == 1) ? Color.blue : array2[(int)(cell.lsa_level - 1)]);
				array[num++] = color;
			}
		}
		string text = this.path + "/pathfinding_LSA.png";
		Texture2D texture2D = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
		texture2D.SetPixels(array);
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(text, bytes);
		return true;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000DA3C0 File Offset: 0x000D85C0
	private void Save(bool LSA)
	{
		TerrainData terrainData = this.terrain.terrainData;
		using (FileStream fileStream = File.OpenWrite(this.path + "/pathfinding.bin"))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				binaryWriter.Write(this.width);
				binaryWriter.Write(this.height);
				for (int i = 0; i < this.cells.Length; i++)
				{
					if (i % 100000 == 0)
					{
						global::Common.EditorProgress("Build Path Data", "Saving path data", (float)i / (float)this.cells.Length, false);
					}
					PathDataBuilder.Cell cell = this.cells[i];
					binaryWriter.Write(cell.slope);
					binaryWriter.Write(cell.bits);
					binaryWriter.Write(cell.lsa_level);
					binaryWriter.Write(cell.river_offset);
				}
			}
		}
		this.SaveTex();
		if (LSA)
		{
			this.SaveLSATex();
		}
		global::Common.EditorProgress("Build Path Data", "Refreshing assets database", 1f, false);
	}

	// Token: 0x04000DA4 RID: 3492
	private global::PathFinding path_finding;

	// Token: 0x04000DA5 RID: 3493
	private string path;

	// Token: 0x04000DA6 RID: 3494
	private bool save_tex;

	// Token: 0x04000DA7 RID: 3495
	private byte lsa_level;

	// Token: 0x04000DA8 RID: 3496
	public bool has_lsa_generated;

	// Token: 0x04000DA9 RID: 3497
	public int width;

	// Token: 0x04000DAA RID: 3498
	public int height;

	// Token: 0x04000DAB RID: 3499
	private PathDataBuilder.Cell[] cells;

	// Token: 0x04000DAC RID: 3500
	private Terrain terrain;

	// Token: 0x04000DAD RID: 3501
	private float water_level;

	// Token: 0x04000DAE RID: 3502
	private Camera cam;

	// Token: 0x04000DAF RID: 3503
	private RenderTexture rt;

	// Token: 0x04000DB0 RID: 3504
	private const int rt_res = 16384;

	// Token: 0x04000DB1 RID: 3505
	private const int tex_res = 4096;

	// Token: 0x04000DB2 RID: 3506
	private Shader shader;

	// Token: 0x04000DB3 RID: 3507
	private Color clrOcean = new Color(0f, 0f, 1f);

	// Token: 0x04000DB4 RID: 3508
	private Color clrCoast = new Color(0f, 1f, 0f);

	// Token: 0x04000DB5 RID: 3509
	private Color clrCoastOcean = new Color(1f, 1f, 1f);

	// Token: 0x04000DB6 RID: 3510
	private Color clrLakes = new Color(0f, 0.5f, 0.5f);

	// Token: 0x04000DB7 RID: 3511
	private Color clrRivers = new Color(0f, 1f, 1f);

	// Token: 0x04000DB8 RID: 3512
	private Color clrRoads = new Color(1f, 1f, 0f);

	// Token: 0x04000DB9 RID: 3513
	private Color clrTowns = new Color(1f, 0f, 1f);

	// Token: 0x04000DBA RID: 3514
	private Color clrSlope = new Color(1f, 0.5f, 0f);

	// Token: 0x04000DBB RID: 3515
	private Color clrPassable = new Color(0f, 0f, 0f);

	// Token: 0x04000DBC RID: 3516
	private Color clrImpassable = new Color(1f, 0f, 0f);

	// Token: 0x020006C0 RID: 1728
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Cell
	{
		// Token: 0x040036D8 RID: 14040
		public float min_height;

		// Token: 0x040036D9 RID: 14041
		public float max_height;

		// Token: 0x040036DA RID: 14042
		public byte slope;

		// Token: 0x040036DB RID: 14043
		public byte bits;

		// Token: 0x040036DC RID: 14044
		public byte lsa_level;

		// Token: 0x040036DD RID: 14045
		public byte river_offset;
	}
}
