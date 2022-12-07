using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000BF RID: 191
public class RuntimePathDataBuilder
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x0600085D RID: 2141 RVA: 0x0005A308 File Offset: 0x00058508
	private bool debug_textures
	{
		get
		{
			return BattleMap.Get().DebugGeneration;
		}
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0005A314 File Offset: 0x00058514
	public void Prebake(bool force = false)
	{
		if (this.built_at_least_once && !force)
		{
			return;
		}
		this.built_at_least_once = true;
		this.Build(false, false, true, true, false);
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0005A334 File Offset: 0x00058534
	public RuntimePathDataBuilder(global::PathFinding pf)
	{
		this.path_finding = pf;
		Scene scene;
		if (pf != null)
		{
			scene = pf.gameObject.scene;
		}
		else
		{
			scene = SceneManager.GetActiveScene();
		}
		string map_name = scene.name.ToLowerInvariant();
		this.lpf = new Logic.PathFinding(null, map_name);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0005A48C File Offset: 0x0005868C
	public void LoadPFSettings()
	{
		Scene scene;
		if (this.path_finding != null)
		{
			scene = this.path_finding.gameObject.scene;
		}
		else
		{
			scene = SceneManager.GetActiveScene();
		}
		string str = scene.name.ToLowerInvariant();
		DT.Field defField = global::Defs.GetDefField("PathFindings." + str, null);
		this.lpf.settings = new Logic.PathFinding.Settings();
		this.lpf.settings.Load(defField);
		this.lpf.settings.multithreaded = false;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x000023FD File Offset: 0x000005FD
	private void EditorStep(string text1, string text2)
	{
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0005A514 File Offset: 0x00058714
	public void Build(bool also_high_pf, bool also_roads, bool disable_houses, bool passable_settlements, bool tree_grid = true)
	{
		bool flag = false;
		try
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			this.EditorStep("Analyzing map", "Initializing");
			flag = !this.Init();
			this.EditorStep("Analyzing map", "Disabling houses");
			if (!flag && disable_houses)
			{
				flag = !this.SetActiveHouses(false);
			}
			this.EditorStep("Analyzing map", "Building Terrain");
			if (!flag)
			{
				flag = !this.BuildTerrain();
			}
			this.EditorStep("Analyzing map", "Building masks");
			if (!flag)
			{
				flag = (this.BuildMasks(true, true, passable_settlements) == null);
			}
			this.EditorStep("Analyzing map", "Building Roads");
			if (also_roads && !flag)
			{
				flag = (this.BuildRoads(null) == null);
			}
			this.EditorStep("Analyzing map", "Building Mountains");
			if (!flag)
			{
				flag = (this.BuildMountains() == null);
			}
			this.EditorStep("Analyzing map", "Generating Heights");
			if (!flag)
			{
				if (this.heights_grid != null)
				{
					this.heights_grid.Dispose();
				}
				this.heights_grid = this.GenerateHeights();
				flag = (this.heights_grid == null);
			}
			this.EditorStep("Analyzing map", "Generating Passability");
			if (!flag)
			{
				flag = !this.GeneratePassability();
			}
			this.EditorStep("Analyzing map", "Building Coast");
			if (!flag)
			{
				flag = !this.buildCoast();
			}
			this.EditorStep("Analyzing map", "Loading Pathfinding");
			if (!flag)
			{
				this.LoadPathfinding();
			}
			this.EditorStep("Analyzing map", "Enabling Houses");
			if (!flag && disable_houses)
			{
				flag = !this.SetActiveHouses(true);
			}
			this.EditorStep("Analyzing map", "Building tree grid");
			if (!flag && tree_grid)
			{
				this.BuildTreeArr();
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

	// Token: 0x06000863 RID: 2147 RVA: 0x0005A754 File Offset: 0x00058954
	public void BuildTreeArr()
	{
		this.tree_count_grid_width = (int)this.terrain.terrainData.size.x / 20;
		this.tree_count_grid_height = (int)this.terrain.terrainData.size.z / 20;
		this.tree_count_grid = new byte[this.tree_count_grid_width, this.tree_count_grid_height];
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		for (int i = 0; i < this.terrain.terrainData.treePrototypes.Length; i++)
		{
			MeshRenderer[] componentsInChildren = this.terrain.terrainData.treePrototypes[i].prefab.GetComponentsInChildren<MeshRenderer>();
			bool flag = false;
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].sharedMaterial.shader.name.ToLowerInvariant().Contains("grass"))
				{
					flag = true;
					break;
				}
			}
			dictionary[i] = !flag;
		}
		for (int k = 0; k < this.terrain.terrainData.treeInstances.Length; k++)
		{
			TreeInstance treeInstance = this.terrain.terrainData.treeInstances[k];
			if (dictionary[treeInstance.prototypeIndex])
			{
				Vector3 v = Vector3.Scale(treeInstance.position, this.terrain.terrainData.size);
				Coord treeCoords = this.GetTreeCoords(v);
				ref byte ptr = ref this.tree_count_grid[treeCoords.x, treeCoords.y];
				ptr += 1;
			}
		}
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0005A8D8 File Offset: 0x00058AD8
	public Coord GetTreeCoords(Point pos)
	{
		Coord coord = new Coord((int)pos.x / 20, (int)pos.y / 20);
		if (coord.x < 0)
		{
			coord.x = 0;
		}
		if (coord.x >= this.tree_count_grid_width)
		{
			coord.x = this.tree_count_grid_width - 1;
		}
		if (coord.y < 0)
		{
			coord.y = 0;
		}
		if (coord.y >= this.tree_count_grid_height)
		{
			coord.y = this.tree_count_grid_height - 1;
		}
		return coord;
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0005A960 File Offset: 0x00058B60
	private bool SetActiveHouses(bool val)
	{
		SettlementBV sbv = BattleMap.Get().sbv;
		if (sbv == null)
		{
			return true;
		}
		int layer;
		if (!val)
		{
			layer = LayerMask.NameToLayer("Facegen");
		}
		else
		{
			layer = LayerMask.NameToLayer("Settlements");
		}
		if (sbv.citadel != null)
		{
			sbv.citadel.gameObject.SetLayer(layer, true);
		}
		for (int i = 0; i < sbv.houses.Count; i++)
		{
			if (!(sbv.houses[i] == null))
			{
				sbv.houses[i].gameObject.SetLayer(layer, true);
			}
		}
		return true;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0005AA01 File Offset: 0x00058C01
	public bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < this.width && y >= 0 && y < this.height;
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0005AA1F File Offset: 0x00058C1F
	public void WorldToGrid(Vector3 point, out int x, out int y)
	{
		x = (int)(point.x / this.path_finding.settings.tile_size);
		y = (int)(point.z / this.path_finding.settings.tile_size);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0005AA55 File Offset: 0x00058C55
	public PathData.Cell GetCell(int x, int y)
	{
		return this.cells[this.GetIdx(x, y)];
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0005AA6C File Offset: 0x00058C6C
	public PathData.Cell GetCell(Vector3 point)
	{
		int x;
		int y;
		this.WorldToGrid(point, out x, out y);
		return this.GetCell(x, y);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0005AA8C File Offset: 0x00058C8C
	public void SetCell(PathData.Cell cell, int x, int y)
	{
		this.cells[this.GetIdx(x, y)] = cell;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0005AAA4 File Offset: 0x00058CA4
	public void SetCell(PathData.Cell cell, Vector3 point)
	{
		int x;
		int y;
		this.WorldToGrid(point, out x, out y);
		this.SetCell(cell, x, y);
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0005AAC5 File Offset: 0x00058CC5
	public int GetIdx(int x, int y)
	{
		return y * this.width + x;
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0005AAD4 File Offset: 0x00058CD4
	private bool Init()
	{
		this.LoadPFSettings();
		this.terrain = global::Common.GetBiggestTerrain();
		if (this.terrain == null)
		{
			return false;
		}
		TerrainHeightsRenderer terrainHeightsRenderer = this.terrain.GetComponent<TerrainHeightsRenderer>();
		if (terrainHeightsRenderer == null)
		{
			terrainHeightsRenderer = UnityEngine.Object.FindObjectOfType<TerrainHeightsRenderer>();
		}
		terrainHeightsRenderer.Render(true, false);
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
		Vector3 size = global::Common.GetBiggestTerrain().terrainData.size;
		this.width = (int)(size.x / this.path_finding.settings.tile_size);
		this.height = (int)(size.z / this.path_finding.settings.tile_size);
		this.cells = new PathData.Cell[this.width * this.height];
		this.water_level = MapData.GetWaterLevel();
		return true;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0005ABD8 File Offset: 0x00058DD8
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

	// Token: 0x0600086F RID: 2159 RVA: 0x0005AC28 File Offset: 0x00058E28
	private bool BuildTerrain()
	{
		Debug.Log("Building terrain info");
		TerrainData terrainData = this.terrain.terrainData;
		int heightmapResolution = terrainData.heightmapResolution;
		for (int i = 0; i < this.height; i++)
		{
			float num = (float)i / (float)(this.height - 1);
			int num2 = (int)(num * (float)heightmapResolution);
			for (int j = 0; j < this.width; j++)
			{
				float num3 = (float)j / (float)(this.width - 1);
				int num4 = (int)(num3 * (float)heightmapResolution);
				float num5 = terrainData.GetHeight(num4, num2);
				if (num5 < 0.01f)
				{
					num5 = 0.01f;
				}
				byte b;
				if (num2 == 0 || num2 == heightmapResolution - 1 || num4 == 0 || num4 == heightmapResolution - 1)
				{
					b = 100;
				}
				else
				{
					b = (byte)terrainData.GetSteepness(num3, num);
				}
				int idx = this.GetIdx(j, i);
				PathData.Cell cell = this.cells[idx];
				if (cell.max_height == 0f || num5 < cell.min_height)
				{
					cell.min_height = num5;
				}
				if (num5 > cell.max_height)
				{
					cell.max_height = num5;
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

	// Token: 0x06000870 RID: 2160 RVA: 0x0005AD84 File Offset: 0x00058F84
	private bool BuildLSA()
	{
		this.lsa_level = 2;
		this.lpf.data.highPFLSACount = new Dictionary<byte, int>();
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				Coord pos = new Coord(i, j);
				Point v = this.lpf.GridCoordToWorld(pos, true);
				this.SetLSA(v, byte.MaxValue);
			}
		}
		if (this.lpf.data.highPFAdditionalNodes != null)
		{
			for (int k = 0; k < this.lpf.data.highPFAdditionalNodes.Length; k++)
			{
				Coord coord = this.lpf.data.highPFAdditionalNodes[k].coord;
				Point v2 = this.lpf.GridCoordToWorld(coord, false);
				this.SetLSA(v2, byte.MaxValue);
			}
		}
		for (int l = 0; l < this.width; l++)
		{
			for (int m = 0; m < this.height; m++)
			{
				Coord pos2 = new Coord(l, m);
				Point v3 = this.lpf.GridCoordToWorld(pos2, true);
				int x;
				int y;
				this.lpf.data.WorldToGrid(v3, out x, out y);
				PathData.Node node = this.lpf.data.GetNode(x, y);
				if (node.lsa == 255)
				{
					if (this.lsa_level == 255)
					{
						Debug.LogError("LSA LEVEL OVERFLOW!!!");
						break;
					}
					if (this.LSACheck(v3))
					{
						this.LSAWave(v3);
					}
					node = this.lpf.data.GetNode(x, y);
					if (node.lsa == 255)
					{
						node.lsa = 0;
						this.lpf.data.ModifyNode(x, y, node);
					}
				}
			}
		}
		this.has_lsa_generated = true;
		return true;
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0005AF78 File Offset: 0x00059178
	public void SetLSA(PPos pos, byte lsa)
	{
		int x;
		int y;
		this.lpf.data.WorldToGrid(pos.pos, out x, out y);
		PathData.Node node = this.lpf.data.GetNode(x, y);
		node.lsa = lsa;
		this.lpf.data.ModifyNode(x, y, node);
		if (lsa != 0 && lsa != 255)
		{
			if (!this.lpf.data.highPFLSACount.ContainsKey(lsa))
			{
				this.lpf.data.highPFLSACount[lsa] = 0;
			}
			Dictionary<byte, int> highPFLSACount = this.lpf.data.highPFLSACount;
			int num = highPFLSACount[lsa];
			highPFLSACount[lsa] = num + 1;
		}
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0005B034 File Offset: 0x00059234
	private bool LSACheck(PPos pos)
	{
		PathData.Node node = this.lpf.data.GetNode(pos.pos);
		return this.lpf.data.IsPassable(pos.pos, 0f) && node.lsa == byte.MaxValue;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x0005B090 File Offset: 0x00059290
	private void Que(PPos pos, Queue<PPos> queue)
	{
		Coord coord = this.lpf.WorldToGridCoord(pos);
		if (coord.x == 22 && coord.y == 20)
		{
			int paID = pos.paID;
		}
		queue.Enqueue(pos);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0005B0D4 File Offset: 0x000592D4
	private void LSAWave(PPos start_pos)
	{
		Queue<PPos> queue = new Queue<PPos>();
		queue.Enqueue(start_pos);
		while (queue.Count > 0)
		{
			PPos ppos = queue.Dequeue();
			if (this.LSACheck(ppos))
			{
				this.SetLSA(ppos, this.lsa_level);
				Coord coord = this.lpf.WorldToGridCoord(ppos);
				if (ppos.paID < 0)
				{
					PathData.HighAdditionalNode additionalNode = this.lpf.GetAdditionalNode(ppos.paID);
					for (int i = 0; i < additionalNode.ribs.Count; i++)
					{
						PathData.HighAdditionalNode other = additionalNode.ribs[i].GetOther(additionalNode);
						Point point = this.lpf.GridCoordToWorld(other.coord, false);
						if (this.LSACheck(point))
						{
							this.Que(new PPos(point, -other.id), queue);
						}
					}
					for (int j = 0; j < additionalNode.terrain_weights.Length; j++)
					{
						if (additionalNode.terrain_weights[j] < 65535)
						{
							Coord closestReachableTerrain = additionalNode.GetClosestReachableTerrain(this.lpf, (byte)j);
							if (!(closestReachableTerrain == Coord.Zero))
							{
								Point v = this.lpf.GridCoordToWorld(closestReachableTerrain, true);
								if (this.lpf.data.HighCoordsInBounds(closestReachableTerrain.x, closestReachableTerrain.y) && this.LSACheck(v))
								{
									this.Que(v, queue);
								}
							}
						}
					}
				}
				else
				{
					int num = -1;
					for (int k = -1; k <= 1; k++)
					{
						for (int l = -1; l <= 1; l++)
						{
							if (l != 0 || k != 0)
							{
								num++;
								Coord coord2 = new Coord(coord.x + l, coord.y + k);
								Point v2 = this.lpf.GridCoordToWorld(coord2, true);
								if (this.lpf.data.GetHighNodeWeight(coord.x, coord.y, num) != 65535 && this.lpf.data.HighCoordsInBounds(coord2.x, coord2.y) && this.LSACheck(v2))
								{
									this.Que(v2, queue);
								}
							}
						}
					}
					Coord coord3 = this.lpf.WorldToGridCoord(ppos);
					int num2;
					NativeMultiHashMapIterator<Coord> nativeMultiHashMapIterator;
					if (this.lpf.data.GetHighGridNode(coord3.x, coord3.y).HasAdditionalNodes && this.lpf.data.highPFAdditionalGrid.TryGetFirstValue(coord3, out num2, out nativeMultiHashMapIterator))
					{
						PathData.HighAdditionalNode additionalNode2 = this.lpf.GetAdditionalNode(num2);
						Coord coord4 = additionalNode2.coord - coord3;
						byte dir = Logic.PathFinding.GetDir(coord4.x, coord4.y);
						if (additionalNode2.terrain_weights[(int)dir] != 65535)
						{
							Point point2 = this.lpf.GridCoordToWorld(additionalNode2.coord, false);
							if (this.LSACheck(point2))
							{
								this.Que(new PPos(point2, -num2), queue);
							}
						}
					}
				}
			}
		}
		this.lsa_level += 1;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0005B408 File Offset: 0x00059608
	private void Render(string layer, Color clr, bool clear = false, bool check_height = true)
	{
		Shader.SetGlobalInt("_ARPass", check_height ? 0 : 3);
		Shader.SetGlobalColor("_ARColor", clr);
		this.cam.clearFlags = (clear ? CameraClearFlags.Color : CameraClearFlags.Nothing);
		this.cam.cullingMask = 1 << LayerMask.NameToLayer(layer);
		this.cam.RenderWithShader(this.shader, "");
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0005B470 File Offset: 0x00059670
	private unsafe HeightsGrid GenerateHeights()
	{
		Debug.Log("Building heights");
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return null;
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
		return heightsGrid;
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0005B51C File Offset: 0x0005971C
	private unsafe bool GeneratePassability()
	{
		Debug.Log("Building passability");
		if (this.pg != null)
		{
			this.pg.Dispose();
		}
		this.pg = new PassabilityGrid();
		this.pg.Alloc(this.width, this.height, this.path_finding.settings.tile_size);
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				bool passable = this.IsPassable(j, i);
				this.pg.data->SetPassable(j, i, passable);
			}
		}
		return true;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0005B5B8 File Offset: 0x000597B8
	public Texture2D BuildMasks(bool rivers = true, bool settlements = true, bool passable_settlements = true)
	{
		Debug.Log("Rendering " + (rivers ? "rivers" : "") + " " + (settlements ? "settlements" : ""));
		global::Settlement.SpawnAll();
		this.rt = RenderTexture.GetTemporary(2048, 2048, 0);
		if (this.cam == null)
		{
			this.cam = global::Common.CreateTerrainCam(global::Common.GetBiggestTerrain());
		}
		this.cam.targetTexture = this.rt;
		if (rivers)
		{
			this.Render("Rivers", new Color(0f, 0f, 1f), true, true);
		}
		if (settlements)
		{
			this.Render("Settlements", new Color(0f, 1f, 0f), !rivers, true);
		}
		if (passable_settlements)
		{
			this.Render("PassableSettlement", new Color(1f, 0f, 0f), !rivers && !settlements, false);
		}
		if (settlements)
		{
			this.Render("ImpassableSettlement", new Color(0f, 1f, 0f), false, false);
		}
		Texture2D texture2D = new Texture2D(2048, 2048, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 2048f, 2048f), 0, 0);
		texture2D.Apply(false);
		RenderTexture.active = null;
		this.cam.targetTexture = null;
		RenderTexture.ReleaseTemporary(this.rt);
		this.rt = null;
		Vector3 size = this.terrain.terrainData.size;
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < this.height; i++)
		{
			int num = (int)((float)i / (float)this.height * 2048f);
			for (int j = 0; j < this.width; j++)
			{
				int num2 = (int)((float)j / (float)this.width * 2048f);
				Color color = pixels[num * 2048 + num2];
				int idx = this.GetIdx(j, i);
				PathData.Cell cell = this.cells[idx];
				if (color.r > 0f)
				{
					if (passable_settlements)
					{
						cell.bits = (byte)((int)cell.bits & -33);
					}
				}
				else if (settlements)
				{
					if (color.g > 0f)
					{
						cell.bits |= 32;
						if (this.path_finding.settings.expand_towns || this.path_finding.settings.towns_passable)
						{
							if (i > 0)
							{
								PathData.Cell[] array = this.cells;
								int num3 = idx - this.width;
								array[num3].bits = (array[num3].bits | 32);
							}
							if (j > 0)
							{
								PathData.Cell[] array2 = this.cells;
								int num4 = idx - 1;
								array2[num4].bits = (array2[num4].bits | 32);
							}
							if (j + 1 < this.width)
							{
								PathData.Cell[] array3 = this.cells;
								int num5 = idx + 1;
								array3[num5].bits = (array3[num5].bits | 32);
							}
							if (i + 1 < this.height)
							{
								PathData.Cell[] array4 = this.cells;
								int num6 = idx + this.width;
								array4[num6].bits = (array4[num6].bits | 32);
							}
						}
					}
					else
					{
						cell.bits = (byte)((int)cell.bits & -33);
					}
				}
				if (color.b > 0f)
				{
					if (rivers)
					{
						cell.bits = (byte)((int)cell.bits & -9);
						cell.bits |= 2;
					}
				}
				else
				{
					cell.bits = (byte)((int)cell.bits & -3);
				}
				this.cells[idx] = cell;
			}
		}
		return texture2D;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0005B958 File Offset: 0x00059B58
	public void ClearRoads()
	{
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				int idx = this.GetIdx(j, i);
				PathData.Cell cell = this.cells[idx];
				cell.bits = (byte)((int)this.cells[idx].bits & -2);
				this.cells[idx] = cell;
			}
		}
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0005B9C8 File Offset: 0x00059BC8
	public Texture2D BuildRoads(List<TerrainPath> roads = null)
	{
		Debug.Log("Rendering road masks");
		if (roads != null)
		{
			for (int i = 0; i < roads.Count; i++)
			{
				roads[i].width /= 2f;
				roads[i].CreateLines();
			}
		}
		this.rt = RenderTexture.GetTemporary(2048, 2048, 0);
		this.cam.targetTexture = this.rt;
		if (roads != null)
		{
			for (int j = 0; j < roads.Count; j++)
			{
				roads[j].CreateLines();
			}
		}
		this.Render("Roads", new Color(1f, 0f, 0f), true, false);
		Texture2D texture2D = new Texture2D(2048, 2048, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 2048f, 2048f), 0, 0);
		texture2D.Apply(false);
		RenderTexture.active = null;
		this.cam.targetTexture = null;
		RenderTexture.ReleaseTemporary(this.rt);
		this.rt = null;
		Color[] pixels = texture2D.GetPixels();
		for (int k = 0; k < this.height; k++)
		{
			int num = (int)((float)k / (float)this.height * 2048f);
			for (int l = 0; l < this.width; l++)
			{
				int num2 = (int)((float)l / (float)this.width * 2048f);
				ref Color ptr = pixels[num * 2048 + num2];
				int idx = this.GetIdx(l, k);
				PathData.Cell cell = this.cells[idx];
				if (ptr.r > 0f)
				{
					cell.bits |= 1;
				}
				else
				{
					cell.bits = (byte)((int)this.cells[idx].bits & -2);
				}
				this.cells[idx] = cell;
			}
		}
		if (roads != null)
		{
			for (int m = 0; m < roads.Count; m++)
			{
				roads[m].width *= 2f;
				roads[m].CreateLines();
			}
		}
		return texture2D;
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0005BC00 File Offset: 0x00059E00
	private Texture2D BuildMountains()
	{
		Debug.Log("Rendering mountains");
		this.rt = RenderTexture.GetTemporary(2048, 2048, 0);
		this.cam.targetTexture = this.rt;
		this.Render("Rocks", new Color(0f, 0f, 1f), true, true);
		this.Render("Mountains", new Color(1f, 0f, 0f), false, true);
		Texture2D texture2D = new Texture2D(2048, 2048, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 2048f, 2048f), 0, 0);
		RenderTexture.active = null;
		this.cam.targetTexture = null;
		RenderTexture.ReleaseTemporary(this.rt);
		this.rt = null;
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < this.height; i++)
		{
			int num = (int)((float)i / (float)this.height * 2048f);
			for (int j = 0; j < this.width; j++)
			{
				int num2 = (int)((float)j / (float)this.width * 2048f);
				Color color = pixels[num * 2048 + num2];
				if (!(color == Color.black))
				{
					int idx = this.GetIdx(j, i);
					PathData.Cell cell = this.cells[idx];
					if (color.r > 0f || color.b > 0f)
					{
						cell.slope = 100;
						this.cells[idx] = cell;
					}
				}
			}
		}
		return texture2D;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0005BDB0 File Offset: 0x00059FB0
	private bool IsPassable(int idx)
	{
		if (idx < 0 || idx >= this.cells.Length)
		{
			return false;
		}
		PathData.Cell cell = this.cells[idx];
		return (cell.bits & 26) == 0 && ((cell.bits & 32) == 0 || this.path_finding.settings.towns_passable) && (cell.slope <= this.path_finding.settings.max_slope || (cell.bits & 1) != 0);
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0005BE2C File Offset: 0x0005A02C
	public bool IsPassable(Vector3 point)
	{
		int cx;
		int cy;
		this.WorldToGrid(point, out cx, out cy);
		return this.IsPassable(cx, cy);
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0005BE4C File Offset: 0x0005A04C
	public bool IsPassable(int cx, int cy)
	{
		int idx = this.GetIdx(cx, cy);
		return this.IsPassable(idx);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0005BE6C File Offset: 0x0005A06C
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

	// Token: 0x06000880 RID: 2176 RVA: 0x0005BE98 File Offset: 0x0005A098
	private void TraceClearance(int idx, int ofs, int cnt, float ts, ref float min)
	{
		float num = (float)this.TraceClearance(idx, ofs, cnt) * ts;
		if (num < min)
		{
			min = num;
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0005BEC0 File Offset: 0x0005A0C0
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

	// Token: 0x06000882 RID: 2178 RVA: 0x0005BFB4 File Offset: 0x0005A1B4
	private bool buildCoast()
	{
		Debug.Log("Building coast");
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				int idx = this.GetIdx(i, j);
				PathData.Cell cell = this.cells[idx];
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

	// Token: 0x06000883 RID: 2179 RVA: 0x0005C0C8 File Offset: 0x0005A2C8
	private void GenerateHighPFData()
	{
		float grid_tile_size = this.lpf.settings.grid_tile_size;
		int num = (int)(this.terrain.terrainData.size.x / grid_tile_size);
		int num2 = (int)(this.terrain.terrainData.size.z / grid_tile_size);
		PathData.HighGridNode[] highPFGrid = new PathData.HighGridNode[num * num2];
		this.lpf.data.highPFGrid = highPFGrid;
		this.lpf.data.highPFGridClosed = new ushort[num * num2];
		this.lpf.data.highPFGrid_width = num;
		this.lpf.data.highPFGrid_height = num2;
		this.lpf.data.highPFGridWeights = new ushort[num * num2 * 4];
		if (!global::Common.EditorProgress("Build Path Data", "Building nodes", 0f, true))
		{
			return;
		}
		for (int i = 0; i < this.lpf.data.highPFGridWeights.Length; i++)
		{
			this.lpf.data.highPFGridWeights[i] = ushort.MaxValue;
		}
		byte b = (byte)(Math.Min(8f, grid_tile_size / 2f) - 1f);
		float num3 = (float)b;
		float num4 = (float)Math.Sqrt((double)(num3 * num3 * 2f));
		for (int j = 0; j < num; j++)
		{
			for (int k = 0; k < num2; k++)
			{
				PathData.HighGridNode node = PathData.HighGridNode.Create();
				Point point = this.lpf.GridCoordToWorld(new Coord(j, k), false);
				bool flag = this.IsPassable(point);
				Coord zero = Coord.Zero;
				float num5 = float.MaxValue;
				bool flag2 = false;
				for (int l = 0; l <= 8; l++)
				{
					Coord dir = this.lpf.GetDir(l);
					int x = dir.x;
					int y = dir.y;
					if (x != 0 || y != 0)
					{
						float num6 = num3;
						Point pt = point + new Point((float)x, (float)y) * num6;
						if (x != 0 && y != 0)
						{
							num6 = num4;
						}
						Point normalized = (pt - point).GetNormalized();
						float num7 = this.lpf.data.TraceDir(point, normalized, num6, 0f, flag);
						if (num7 < num5 && num7 > 0f)
						{
							Point point2 = normalized * (num7 + 1f);
							num5 = num7;
							zero = new Coord((int)Math.Truncate((double)point2.x), (int)Math.Truncate((double)point2.y));
						}
						if (flag && num7 < num6)
						{
							flag2 = true;
						}
					}
				}
				bool flag3 = false;
				if (flag2 || !flag)
				{
					byte b2 = 0;
					Point pt2 = Point.Zero;
					int num8 = (int)(point.x - (float)b);
					while ((float)num8 <= point.x + (float)b)
					{
						int num9 = (int)(point.y - (float)b);
						while ((float)num9 <= point.y + (float)b)
						{
							Point point3 = new Point((float)num8, (float)num9);
							if (this.lpf.data.IsInBounds((int)point3.x, (int)point3.y))
							{
								PathData.Node node2 = this.lpf.data.GetNode(point3);
								if (node2.weight != 0 && node2.clearance > b2)
								{
									b2 = node2.clearance;
									pt2 = point3;
								}
							}
							num9++;
						}
						num8++;
					}
					if (pt2 != Point.Zero)
					{
						Point point4 = pt2 - point;
						byte b3 = (byte)((byte)point4.x + 8 << 4);
						byte b4 = (byte)point4.y + 8;
						node.cell_offset = (b3 | b4);
						flag3 = true;
					}
				}
				if (!flag3 && zero != Coord.Zero && !flag)
				{
					byte b5 = (byte)(zero.x + 8 << 4);
					byte b6 = (byte)(zero.y + 8);
					node.cell_offset = (b5 | b6);
				}
				this.lpf.data.SetHighNode(j, k, node);
			}
		}
		for (int m = 0; m < num; m++)
		{
			for (int n = 0; n < num2; n++)
			{
				Point point5 = this.lpf.GridCoordToWorld(new Coord(m, n), true);
				if (this.lpf.data.IsPassable(point5, 0f))
				{
					int num10 = -1;
					for (int num11 = n - 1; num11 <= n + 1; num11++)
					{
						for (int num12 = m - 1; num12 <= m + 1; num12++)
						{
							if (num12 != m || num11 != n)
							{
								num10++;
								if (num12 >= 0 && num11 >= 0 && num12 < num && num11 < num2)
								{
									Point point6 = this.lpf.GridCoordToWorld(new Coord(num12, num11), true);
									if (this.lpf.data.IsPassable(point6, 0f))
									{
										float num13;
										if (!this.lpf.data.Trace(point5, point6, 0f, true))
										{
											Path path = new Path(null, point5, PathData.PassableArea.Type.All, false);
											path.min_radius = 0f;
											path.max_radius = 10f;
											path.low_level_only = true;
											path.Find(point6, 0f, false);
											path.state = Path.State.Pending;
											this.lpf.pending.Add(path);
											while (path.state == Path.State.Pending)
											{
												this.lpf.Process(true, true);
												if (this.lpf.total_steps > 100 && this.lpf.state == Logic.PathFinding.State.LowSteps)
												{
													this.lpf.Del(path);
													this.lpf.Process(true, true);
													break;
												}
											}
											if (path.state == Path.State.Failed || path.state == Path.State.Stopped)
											{
												goto IL_5F9;
											}
											num13 = path.path_eval;
										}
										else
										{
											num13 = Logic.PathFinding.CalcGroundWeight(this.lpf, point5, point6, 0f, 10f);
										}
										this.lpf.data.SetHighNodeWeight(m, n, num10, (ushort)num13);
									}
								}
							}
							IL_5F9:;
						}
					}
				}
			}
		}
		this.BuildLSA();
		if (BattleMap.Get().DebugGeneration)
		{
			this.SaveTex();
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0005C728 File Offset: 0x0005A928
	private void SaveTex()
	{
		Color[] array = new Color[this.width * this.height];
		int num = 0;
		for (int i = 0; i < this.height; i++)
		{
			if (i % 100 == 0 && !global::Common.EditorProgress("Build Path Data", "Saving texture", (float)i / (float)this.height, true))
			{
				return;
			}
			for (int j = 0; j < this.width; j++)
			{
				PathData.Cell cell = this.cells[num];
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
		File.WriteAllBytes("dbg/pathfinding.png", texture2D.EncodeToPNG());
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0005C960 File Offset: 0x0005AB60
	private PathData.HighAdditionalRib CreateAdditionalRibs(Logic.PathFinding lpf, int node_1_id, int node_2_id, Coord neighbor_2_coord)
	{
		if (node_1_id == node_2_id)
		{
			return null;
		}
		PathData.HighAdditionalNode additionalNode = lpf.GetAdditionalNode(node_1_id);
		PathData.HighAdditionalNode additionalNode2 = lpf.GetAdditionalNode(node_2_id);
		if (additionalNode2.coord != neighbor_2_coord)
		{
			return null;
		}
		for (int i = 0; i < additionalNode.ribs.Count; i++)
		{
			if (additionalNode.ribs[i].GetOther(additionalNode) == additionalNode2)
			{
				return null;
			}
		}
		Point point = lpf.GridCoordToWorld(additionalNode.coord, false);
		Point point2 = lpf.GridCoordToWorld(additionalNode2.coord, false);
		if (!lpf.data.IsPassable(point, 0f))
		{
			return null;
		}
		if (!lpf.data.IsPassable(point2, 0f))
		{
			return null;
		}
		if (!lpf.data.Trace(point, point2, 0f, true))
		{
			return null;
		}
		PathData.HighAdditionalRib highAdditionalRib = new PathData.HighAdditionalRib();
		highAdditionalRib.node1 = additionalNode;
		highAdditionalRib.node2 = additionalNode2;
		additionalNode.ribs.Add(highAdditionalRib);
		additionalNode2.ribs.Add(highAdditionalRib);
		highAdditionalRib.weight = (ushort)Math.Min(Logic.PathFinding.CalcGroundWeight(lpf, point, point2, 0f, 0f), 65535U);
		return highAdditionalRib;
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0005CA8E File Offset: 0x0005AC8E
	public void LoadPathfinding()
	{
		Debug.Log("Setting pathfinding data");
		this.lpf.LoadPFData(this.width, this.height, this.cells);
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0005CAB8 File Offset: 0x0005ACB8
	public void PartialSavePF(Vector3 pos, byte mask)
	{
		PathData.Cell cell = this.GetCell(pos);
		cell.bits |= mask;
		this.SetCell(cell, pos);
		int num = (int)(pos.x / (float)this.width * (float)this.lpf.data.width);
		int num2 = (int)(pos.z / (float)this.height * (float)this.lpf.data.height);
		int num3 = num + num2 * this.lpf.data.width;
		PathData.Node node = default(PathData.Node);
		node.slope = cell.slope;
		node.bits = cell.bits;
		node.lsa = cell.lsa_level;
		node.river_offset = 0;
		node.pa_id = 0;
		if (num2 < 1 || num2 + 1 >= this.height || num < 1 || num + 1 >= this.width)
		{
			node.weight = 0;
		}
		else
		{
			this.lpf.data.CalcWeight(ref node);
		}
		this.lpf.data.nodes[num3] = node;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0005CBCC File Offset: 0x0005ADCC
	public bool GenerateHighPFDataStep()
	{
		Debug.Log("Generating high level pathfinding data");
		BattleMap.Get().ConnectPFLogic();
		Logic.PathFinding logic = this.path_finding.logic;
		if (logic.game.heights != this.heights_grid)
		{
			HeightsGrid heights = logic.game.heights;
			if (heights != null)
			{
				heights.Dispose();
			}
		}
		logic.game.heights = this.heights_grid;
		if (logic.game.passability != this.pg)
		{
			PassabilityGrid passability = logic.game.passability;
			if (passability != null)
			{
				passability.Dispose();
			}
		}
		logic.game.passability = this.pg;
		logic.data = this.lpf.data;
		this.GenerateHighPFData();
		return true;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0005CC85 File Offset: 0x0005AE85
	public void Dispose()
	{
		HeightsGrid heightsGrid = this.heights_grid;
		if (heightsGrid != null)
		{
			heightsGrid.Dispose();
		}
		PassabilityGrid passabilityGrid = this.pg;
		if (passabilityGrid == null)
		{
			return;
		}
		passabilityGrid.Dispose();
	}

	// Token: 0x040006A2 RID: 1698
	private global::PathFinding path_finding;

	// Token: 0x040006A3 RID: 1699
	public int width;

	// Token: 0x040006A4 RID: 1700
	public int height;

	// Token: 0x040006A5 RID: 1701
	private Terrain terrain;

	// Token: 0x040006A6 RID: 1702
	private float water_level;

	// Token: 0x040006A7 RID: 1703
	private Camera cam;

	// Token: 0x040006A8 RID: 1704
	private RenderTexture rt;

	// Token: 0x040006A9 RID: 1705
	private const int rt_res = 2048;

	// Token: 0x040006AA RID: 1706
	private const int tex_res = 2048;

	// Token: 0x040006AB RID: 1707
	private Shader shader;

	// Token: 0x040006AC RID: 1708
	private Color clrOcean = new Color(0f, 0f, 1f);

	// Token: 0x040006AD RID: 1709
	private Color clrCoast = new Color(0f, 1f, 0f);

	// Token: 0x040006AE RID: 1710
	private Color clrCoastOcean = new Color(1f, 1f, 1f);

	// Token: 0x040006AF RID: 1711
	private Color clrLakes = new Color(0f, 0.5f, 0.5f);

	// Token: 0x040006B0 RID: 1712
	private Color clrRivers = new Color(0f, 1f, 1f);

	// Token: 0x040006B1 RID: 1713
	private Color clrRoads = new Color(1f, 1f, 0f);

	// Token: 0x040006B2 RID: 1714
	private Color clrTowns = new Color(1f, 0f, 1f);

	// Token: 0x040006B3 RID: 1715
	private Color clrSlope = new Color(1f, 0.5f, 0f);

	// Token: 0x040006B4 RID: 1716
	private Color clrPassable = new Color(0f, 0f, 0f);

	// Token: 0x040006B5 RID: 1717
	private Color clrImpassable = new Color(1f, 0f, 0f);

	// Token: 0x040006B6 RID: 1718
	public PassabilityGrid pg;

	// Token: 0x040006B7 RID: 1719
	public PathData.Cell[] cells;

	// Token: 0x040006B8 RID: 1720
	public byte[,] tree_count_grid;

	// Token: 0x040006B9 RID: 1721
	public int tree_count_grid_width;

	// Token: 0x040006BA RID: 1722
	public int tree_count_grid_height;

	// Token: 0x040006BB RID: 1723
	public const int tree_grid_size = 20;

	// Token: 0x040006BC RID: 1724
	public HeightsGrid heights_grid;

	// Token: 0x040006BD RID: 1725
	public Logic.PathFinding lpf;

	// Token: 0x040006BE RID: 1726
	private bool built_at_least_once;

	// Token: 0x040006BF RID: 1727
	private byte lsa_level;

	// Token: 0x040006C0 RID: 1728
	public bool has_lsa_generated;
}
