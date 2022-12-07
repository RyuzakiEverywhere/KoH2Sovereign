using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000169 RID: 361
[ExecuteInEditMode]
[RequireComponent(typeof(global::HexGroups))]
public class BattleTemplate : MonoBehaviour
{
	// Token: 0x06001241 RID: 4673 RVA: 0x000BF544 File Offset: 0x000BD744
	public Logic.Battle Battle()
	{
		if (this.battle != null)
		{
			return this.battle;
		}
		Game game = GameLogic.Get(true);
		Logic.Army defender = new Logic.Army(game, Point.Invalid, 2);
		Logic.Army attacker = new Logic.Army(game, Point.Invalid, 1);
		this.battle = Logic.Battle.StartBattle(attacker, defender, false);
		return this.battle;
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x000BF593 File Offset: 0x000BD793
	private void Start()
	{
		if (Application.isPlaying)
		{
			this.Battle();
		}
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x000BF5A4 File Offset: 0x000BD7A4
	public static BattleTemplate Get()
	{
		global::HexGrid hexGrid = global::HexGrid.Get();
		if (hexGrid == null)
		{
			return null;
		}
		return hexGrid.GetComponent<BattleTemplate>();
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x000BF5C8 File Offset: 0x000BD7C8
	public bool PaletteLoaded()
	{
		return this.palette_info != null;
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x000BF5D4 File Offset: 0x000BD7D4
	public void UnloadPalette()
	{
		this.palette_grid = null;
		this.palette_info = null;
		this.palette_heights = null;
		this.palette_splats = null;
		this.palette_trees = null;
		this.palette_objects = null;
		this.paletteMap = null;
		this.shapeCoord = null;
		this.numOfHexPerShape = null;
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x000BF620 File Offset: 0x000BD820
	public void LoadPalette(bool force_reload)
	{
		if (!force_reload && this.PaletteLoaded())
		{
			return;
		}
		this.UnloadPalette();
		if (string.IsNullOrEmpty(this.palette_path))
		{
			return;
		}
		this.LoadPaletteTerrain();
		this.LoadPaletteTrees();
		this.LoadPaletteObjects();
		this.LoadPaletteGroups();
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x000BF65C File Offset: 0x000BD85C
	public void Remember()
	{
		global::HexGroups.Get().SaveGroups(global::HexGroups.GetScenePath());
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		this.org_terrain = new BSGTerrainTools.TerrainBlock(terrainData, terrainData.bounds);
		this.org_terrain.Import(true, true, true);
		this.org_terrain.Save(this.TerrainFileName(), true, true, true);
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x000BF6C0 File Offset: 0x000BD8C0
	public void Generate()
	{
		if (global::HexGroups.Get().shapes == null)
		{
			global::HexGroups.Get().LoadDefs();
		}
		if (!this.lock_seed)
		{
			this.seed = (int)(UnityEngine.Time.unscaledTime * 1000f) % 1000;
		}
		Random.InitState(this.seed);
		this.generated = true;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		this.BeginGenerate();
		Stopwatch stopwatch2 = new Stopwatch();
		stopwatch2.Start();
		this.ImportTiles();
		stopwatch2.Stop();
		Debug.Log("Import done in " + stopwatch2.Elapsed.ToString());
		this.EndGenerate();
		stopwatch.Stop();
		Debug.Log("Map generated in " + stopwatch.Elapsed.ToString());
		global::HexGrid.Get().GenerateTexture(false);
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x000BF79C File Offset: 0x000BD99C
	public void LoadOrgTerrain(bool force_reload = false)
	{
		if (!force_reload && this.org_terrain != null)
		{
			return;
		}
		this.org_terrain = null;
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		this.org_terrain = new BSGTerrainTools.TerrainBlock(terrainData, terrainData.bounds);
		if (!this.org_terrain.Load(this.TerrainFileName(), true, true, true))
		{
			this.org_terrain = null;
		}
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x000BF7FC File Offset: 0x000BD9FC
	public void Restore()
	{
		global::HexGroups.Get().Reload();
		this.LoadOrgTerrain(false);
		if (this.org_terrain != null)
		{
			this.org_terrain.Apply(true, true, true);
			BSGTerrainTools.RebuildBasemap(this.org_terrain.td);
		}
		if (this.gen_objects != null)
		{
			for (int i = 0; i < this.gen_objects.Count; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.gen_objects[i]);
			}
			this.gen_objects = null;
		}
		this.generated = false;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x000BF880 File Offset: 0x000BDA80
	public string TerrainFileName()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		return Path.GetDirectoryName(activeScene.path) + "/" + Path.GetFileNameWithoutExtension(activeScene.path) + ".terrain";
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x000BF8C0 File Offset: 0x000BDAC0
	private TerrainData GetTerrainData()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return null;
		}
		TerrainData terrainData = activeTerrain.terrainData;
		if (terrainData == null)
		{
			return null;
		}
		return terrainData;
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x000BF8F4 File Offset: 0x000BDAF4
	private Bounds CalcBounds(Logic.HexGrid grid, Logic.HexGrid.Coord tile)
	{
		Point pt = grid.Center(tile);
		Vector3 size = new Vector3(2f * grid.tile_radius, 0f, 1.732f * grid.tile_radius);
		return new Bounds(pt, size);
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x000BF938 File Offset: 0x000BDB38
	public void LoadPaletteTerrain()
	{
		TextAsset textAsset = Assets.Get<TextAsset>(this.palette_path + ".terrain.bytes");
		if (textAsset == null)
		{
			return;
		}
		using (MemoryStream memoryStream = new MemoryStream(textAsset.bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				this.palette_grid = new Logic.HexGrid(binaryReader.ReadSingle());
				this.palette_info = BSGTerrainTools.TerrainInfo.Load(binaryReader);
				this.palette_heights = BSGTerrainTools.Float2D.Load(binaryReader);
				this.palette_heights.SetWorldRect(this.palette_info.bounds.min.x, this.palette_info.bounds.min.z, this.palette_info.bounds.size.x, this.palette_info.bounds.size.z);
				this.palette_splats = BSGTerrainTools.Float3D.Load(binaryReader);
				this.palette_splats.SetWorldRect(this.palette_info.bounds.min.x, this.palette_info.bounds.min.z, this.palette_info.bounds.size.x, this.palette_info.bounds.size.z);
			}
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000BFAB8 File Offset: 0x000BDCB8
	public void LoadPaletteTrees()
	{
		TextAsset textAsset = Assets.Get<TextAsset>(this.palette_path + ".trees.bytes");
		if (textAsset == null)
		{
			return;
		}
		using (MemoryStream memoryStream = new MemoryStream(textAsset.bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				int num = binaryReader.ReadInt32();
				if (num > 0)
				{
					this.palette_trees = new Dictionary<Logic.HexGrid.Coord, List<TreeInstance>>();
					for (int i = 0; i < num; i++)
					{
						Logic.HexGrid.Coord key = default(Logic.HexGrid.Coord);
						key.x = binaryReader.ReadInt32();
						key.y = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						List<TreeInstance> list = new List<TreeInstance>(num2);
						for (int j = 0; j < num2; j++)
						{
							TreeInstance item = BSGTerrainTools.LoadTree(binaryReader);
							list.Add(item);
						}
						this.palette_trees.Add(key, list);
					}
				}
			}
		}
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x000BFBBC File Offset: 0x000BDDBC
	public void LoadPaletteGroups()
	{
		TextAsset textAsset = Assets.Get<TextAsset>(this.palette_path + ".groups.bytes");
		if (textAsset == null)
		{
			Debug.LogError("Missing groups");
			return;
		}
		this.paletteGroups = global::HexGroups.Load(textAsset, false);
		this.BuildPaletteMap(this.paletteGroups);
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x000BFC0C File Offset: 0x000BDE0C
	private void BuildPaletteMap(Logic.HexGroups paletteGroups)
	{
		this.paletteMap = new Dictionary<string, List<HexGroup>>();
		this.shapeCoord = new Dictionary<string, List<Logic.HexGrid.Coord>>();
		this.numOfHexPerShape = new Dictionary<int, string>();
		for (int i = 0; i < paletteGroups.Count; i++)
		{
			HexGroup hexGroup = paletteGroups[i];
			if (!this.paletteMap.ContainsKey(hexGroup.originalShape))
			{
				this.paletteMap[hexGroup.originalShape] = new List<HexGroup>();
			}
			this.paletteMap[hexGroup.originalShape].Add(hexGroup);
			if (!this.shapeCoord.ContainsKey(hexGroup.originalShape))
			{
				this.shapeCoord[hexGroup.originalShape] = new List<Logic.HexGrid.Coord>();
				string[] array = hexGroup.originalShape.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(new char[]
					{
						','
					});
					this.shapeCoord[hexGroup.originalShape].Add(new Logic.HexGrid.Coord(int.Parse(array2[0]), int.Parse(array2[1])));
				}
			}
			this.numOfHexPerShape[hexGroup.Count] = hexGroup.originalShape;
		}
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x000BFD40 File Offset: 0x000BDF40
	public void LoadPaletteObjects()
	{
		this.palette_objects = Assets.Get<GameObject>(this.palette_path + ".objects.prefab");
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x000BFD60 File Offset: 0x000BDF60
	private void BeginGenerate()
	{
		TerrainData terrainData = this.GetTerrainData();
		if (terrainData == null)
		{
			return;
		}
		this.LoadPalette(false);
		this.gen_terrain = new BSGTerrainTools.TerrainBlock(terrainData, terrainData.bounds);
		this.gen_terrain.Import(this.import_heights, this.import_splats, this.import_trees);
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x000BFDB4 File Offset: 0x000BDFB4
	private void EndGenerate()
	{
		this.import_info = null;
		if (this.gen_terrain == null)
		{
			return;
		}
		this.gen_terrain.Apply(this.import_heights, this.import_splats, this.import_trees);
		this.gen_terrain = null;
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x000BFDEC File Offset: 0x000BDFEC
	private void PlaceTilesFromGroup(Logic.HexGrid.Coord start, HexGroup paletteGroup, int rotation = 0)
	{
		HexGroup hexGroup = new HexGroup(this.shapeCoord[paletteGroup.originalShape], "", 0, false, "");
		hexGroup.Rotate(rotation);
		for (int i = 0; i < hexGroup.Count; i++)
		{
			Logic.HexGrid.Coord key = start + hexGroup[i];
			Logic.HexGrid.Coord coord = paletteGroup[i];
			BattleTemplate.TileImportInfo value = new BattleTemplate.TileImportInfo
			{
				palette_tile = coord,
				edges_mask = paletteGroup.CalculateHexMask(coord),
				rot = paletteGroup.rotation
			};
			this.import_info.Add(key, value);
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x000BFE80 File Offset: 0x000BE080
	private void PlaceTile(Logic.HexGrid.Coord tc, Logic.HexGrid.Coord pc, byte edge_mask = 255, int rotation = 0)
	{
		BattleTemplate.TileImportInfo value = new BattleTemplate.TileImportInfo
		{
			palette_tile = pc,
			edges_mask = edge_mask,
			rot = rotation
		};
		this.import_info.Add(tc, value);
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x000BFEB8 File Offset: 0x000BE0B8
	private void PlaceTilesFromGroup(HexGroup templateGroup, HexGroup paletteGroup)
	{
		Logic.HexGrid.Coord pt = templateGroup[0];
		for (int i = 0; i < this.shapeCoord[templateGroup.originalShape].Count; i++)
		{
			Logic.HexGrid.Coord key = pt + this.shapeCoord[templateGroup.originalShape][i];
			Logic.HexGrid.Coord coord = (templateGroup.locked ? Logic.HexGrid.Coord.Invalid : paletteGroup[i]).Rotate(templateGroup.rotation);
			BattleTemplate.TileImportInfo value = new BattleTemplate.TileImportInfo
			{
				palette_tile = coord,
				edges_mask = paletteGroup.CalculateHexMask(coord),
				rot = templateGroup.rotation
			};
			this.import_info.Add(key, value);
		}
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000BFF6C File Offset: 0x000BE16C
	public bool CanPlace(HexGroup group, Logic.HexGrid.Coord c)
	{
		bool result = true;
		for (int i = 0; i < group.Count; i++)
		{
			Logic.HexGrid.Coord coord = c + group[i];
			if (this.import_info.ContainsKey(coord) || coord.x > this.max_template_coord.x || coord.y > this.max_template_coord.y || coord.x < 1 || coord.y < 1)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x000BFFE8 File Offset: 0x000BE1E8
	private void CalcImportInfo()
	{
		this.import_info = new Dictionary<Logic.HexGrid.Coord, BattleTemplate.TileImportInfo>();
		this.max_template_coord = BattleTemplate.CalcMaxCoord(this.gen_terrain.td.size, global::HexGrid.Radius());
		List<Logic.HexGrid.Coord> list = new List<Logic.HexGrid.Coord>();
		Logic.HexGrid.Coord coord = BattleTemplate.CalcMaxCoord(this.palette_info.bounds.size, this.palette_grid.tile_radius);
		Logic.HexGroups groups = global::HexGroups.Get().groups;
		for (int i = 1; i <= this.max_template_coord.x; i++)
		{
			for (int j = 0; j <= this.max_template_coord.y; j++)
			{
				list.Add(new Logic.HexGrid.Coord(i, j));
			}
		}
		for (int k = 0; k < groups.Count; k++)
		{
			HexGroup hexGroup = groups[k];
			if (hexGroup.locked)
			{
				for (int l = 0; l < hexGroup.Count; l++)
				{
					BattleTemplate.TileImportInfo value = new BattleTemplate.TileImportInfo
					{
						palette_tile = Logic.HexGrid.Coord.Invalid,
						edges_mask = hexGroup.CalculateHexMask(hexGroup[l]),
						rot = 0
					};
					this.import_info.Add(hexGroup[l], value);
					list.Remove(hexGroup[l]);
				}
			}
			else
			{
				List<HexGroup> list2;
				this.paletteMap.TryGetValue(hexGroup.originalShape, out list2);
				if (list2 != null && list2.Count > 0)
				{
					int index = Random.Range(0, list2.Count);
					this.PlaceTilesFromGroup(hexGroup, list2[index]);
					Logic.HexGrid.Coord pt = hexGroup[0];
					for (int m = 0; m < this.shapeCoord[hexGroup.originalShape].Count; m++)
					{
						Logic.HexGrid.Coord item = pt + this.shapeCoord[hexGroup.originalShape][m];
						list.Remove(item);
					}
				}
			}
		}
		int num = list.Count * 2;
		while (list.Count > 0 && num > 0)
		{
			num--;
			bool flag = false;
			int num2 = 10;
			List<int> list3 = new List<int>(this.numOfHexPerShape.Keys);
			while (!flag && list3.Count > 0)
			{
				num2--;
				int index2 = Random.Range(0, list.Count);
				Logic.HexGrid.Coord coord2 = list[index2];
				int index3 = Random.Range(0, list3.Count);
				string key = this.numOfHexPerShape[list3[index3]];
				int index4 = Random.Range(0, this.paletteMap[key].Count);
				HexGroup hexGroup2 = this.paletteMap[key][index4];
				HexGroup hexGroup3 = new HexGroup(this.shapeCoord[hexGroup2.originalShape], "", 0, false, "");
				if (hexGroup3.Count != 1)
				{
					List<int> list4 = new List<int>
					{
						0,
						1,
						2,
						3,
						4,
						5
					};
					for (int n = 0; n < 1; n++)
					{
						int index5 = Random.Range(0, list4.Count);
						int rotation = 0;
						list4.RemoveAt(index5);
						HexGroup hexGroup4 = hexGroup3.Rotate(rotation);
						if (this.CanPlace(hexGroup4, coord2))
						{
							flag = true;
							List<Logic.HexGrid.Coord> list5 = new List<Logic.HexGrid.Coord>();
							for (int num3 = 0; num3 < hexGroup4.Count; num3++)
							{
								if (this.import_info.ContainsKey(hexGroup4[num3] + coord2))
								{
									this.CanPlace(hexGroup4, coord2);
									hexGroup3.Rotate(rotation);
								}
								Logic.HexGrid.Coord coord3 = hexGroup4[num3] + coord2;
								list5.Add(coord3);
								this.PlaceTile(coord3, hexGroup2[num3], hexGroup4.CalculateHexMask(hexGroup4[num3]), rotation);
								list.Remove(coord3);
							}
							global::HexGroups.Get().groups.Add(new HexGroup(list5, "", 0, false, ""));
							break;
						}
					}
				}
				list3.RemoveAt(index3);
			}
		}
		if (list.Count > 0)
		{
			List<Logic.HexGrid.Coord> list6 = new List<Logic.HexGrid.Coord>();
			for (int num4 = 1; num4 <= coord.x; num4++)
			{
				for (int num5 = 1; num5 <= coord.y; num5++)
				{
					Logic.HexGrid.Coord coord4 = new Logic.HexGrid.Coord(num4, num5);
					if (!this.paletteGroups.Contains(coord4))
					{
						list6.Add(coord4);
					}
				}
			}
			if (list6.Count > 0)
			{
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					int index6 = Random.Range(0, list6.Count);
					this.PlaceTile(list[num6], list6[index6], byte.MaxValue, 0);
				}
			}
		}
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x000C04BA File Offset: 0x000BE6BA
	private void ImportTiles()
	{
		this.CalcImportInfo();
		this.ImportTerrain();
		this.ImportTrees();
		this.ImportObjects();
		this.WaitCompleted();
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x000C04DA File Offset: 0x000BE6DA
	private void ImportTerrain()
	{
		if (!this.import_heights && !this.import_splats)
		{
			return;
		}
		if (this.multithreaded)
		{
			this.ImportTerrainMultiThreaded();
			return;
		}
		this.ImportTerrainSingeThreaded();
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000C0504 File Offset: 0x000BE704
	private void ImportTerrainMultiThreaded()
	{
		int processorCount = Environment.ProcessorCount;
		if (processorCount < 2)
		{
			this.ImportTerrainSingeThreaded();
			return;
		}
		this.terrain_threads = new Thread[processorCount];
		for (int i = 0; i < processorCount; i++)
		{
			BattleTemplate.TerrainColumnImport @object = new BattleTemplate.TerrainColumnImport(this, i + 1, processorCount);
			this.terrain_threads[i] = new Thread(new ThreadStart(@object.Import));
			this.terrain_threads[i].Start();
		}
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x000C056C File Offset: 0x000BE76C
	private void ImportTerrainSingeThreaded()
	{
		Logic.HexGrid.Coord coord = default(Logic.HexGrid.Coord);
		coord.x = 1;
		while (coord.x <= this.max_template_coord.x)
		{
			int y;
			int num;
			BattleTemplate.CalcYRange(coord.x, this.max_template_coord, out y, out num);
			coord.y = y;
			while (coord.y <= num)
			{
				BattleTemplate.TileImportInfo tii;
				if (this.import_info.TryGetValue(coord, out tii))
				{
					this.ImportTerrain(coord, tii);
				}
				coord.y += 2;
			}
			coord.x++;
		}
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x000C05F8 File Offset: 0x000BE7F8
	private void ImportTrees()
	{
		if (!this.import_trees || this.palette_trees == null)
		{
			return;
		}
		Logic.HexGrid.Coord coord = default(Logic.HexGrid.Coord);
		coord.x = 1;
		while (coord.x <= this.max_template_coord.x)
		{
			int y;
			int num;
			BattleTemplate.CalcYRange(coord.x, this.max_template_coord, out y, out num);
			coord.y = y;
			while (coord.y <= num)
			{
				BattleTemplate.TileImportInfo tii;
				if (this.import_info.TryGetValue(coord, out tii))
				{
					this.ImportTrees(coord, tii);
				}
				coord.y += 2;
			}
			coord.x++;
		}
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x000C0694 File Offset: 0x000BE894
	private void ImportObjects()
	{
		if (!this.import_objects)
		{
			return;
		}
		this.gen_objects = new List<GameObject>();
		Logic.HexGrid.Coord coord = default(Logic.HexGrid.Coord);
		coord.x = 1;
		while (coord.x <= this.max_template_coord.x)
		{
			int y;
			int num;
			BattleTemplate.CalcYRange(coord.x, this.max_template_coord, out y, out num);
			coord.y = y;
			while (coord.y <= num)
			{
				BattleTemplate.TileImportInfo tii;
				if (this.import_info.TryGetValue(coord, out tii))
				{
					this.ImportObjects(coord, tii);
				}
				coord.y += 2;
			}
			coord.x++;
		}
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x000C0734 File Offset: 0x000BE934
	private void WaitCompleted()
	{
		if (this.terrain_threads == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_threads.Length; i++)
		{
			this.terrain_threads[i].Join();
		}
		this.terrain_threads = null;
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x000C0774 File Offset: 0x000BE974
	private void ImportTerrainColumn(int x)
	{
		int y;
		int num;
		BattleTemplate.CalcYRange(x, this.max_template_coord, out y, out num);
		Logic.HexGrid.Coord coord = new Logic.HexGrid.Coord(x, 0);
		coord.y = y;
		while (coord.y <= num)
		{
			BattleTemplate.TileImportInfo tii;
			if (this.import_info.TryGetValue(coord, out tii))
			{
				this.ImportTerrain(coord, tii);
			}
			coord.y += 2;
		}
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x000C07D4 File Offset: 0x000BE9D4
	private float GetVertexHeight(Logic.HexGrid.Coord palette_tile, int idx)
	{
		Point point = this.palette_grid.Vertex(palette_tile, idx);
		return this.palette_heights.GetWorld(point.x, point.y) * this.gen_terrain.ti.bounds.size.y;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000C0824 File Offset: 0x000BEA24
	private void ImportTerrain(Logic.HexGrid.Coord template_tile, BattleTemplate.TileImportInfo tii)
	{
		if (tii.palette_tile == Logic.HexGrid.Coord.Invalid)
		{
			return;
		}
		Logic.HexGrid grid = global::HexGrid.Grid();
		float edge_fade = global::HexGrid.Get().edge_fade;
		float inv_edge_fade = 1f / edge_fade;
		Bounds bounds = this.CalcBounds(this.palette_grid, tii.palette_tile);
		Bounds bounds2 = this.CalcBounds(grid, template_tile);
		BSGTerrainTools.Gen2D gen2D = new BSGTerrainTools.Gen2D(1f);
		gen2D.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			Logic.HexGrid.Coord pt;
			int num;
			float num2;
			grid.FindNearestEdge(new Point(wx, wy), out pt, out num, out num2, tii.edges_mask);
			if (pt != template_tile)
			{
				return 0f;
			}
			if (num2 >= edge_fade)
			{
				return 1f;
			}
			float num3 = num2 * inv_edge_fade;
			if (num3 < 0.001f)
			{
				num3 = 0.001f;
			}
			return Mathf.Clamp(num3, 0f, 1f);
		};
		if (this.import_heights && this.palette_heights != null)
		{
			BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(this.palette_heights.arr);
			Vector2Int vector2Int;
			Vector2Int vector2Int2;
			BSGTerrainTools.CalcGridBounds(bounds, this.palette_info.bounds, this.palette_info.heights_resolution, out vector2Int, out vector2Int2);
			float2D.SetLocalRect(vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
			Bounds bounds3 = BSGTerrainTools.SnapHeightsBounds(bounds2, this.gen_terrain.ti);
			float2D.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
			BSGTerrainTools.Gen2D gen2D2 = new BSGTerrainTools.Gen2D(float2D);
			if (this.additive)
			{
				gen2D2.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
				{
					float result = src_val + (tgt_val - 0.125f) * alpha;
					alpha = 1f;
					return result;
				};
			}
			BSGTerrainTools.ModifyHeights(this.gen_terrain, bounds2, gen2D2, gen2D);
		}
		if (this.import_splats && this.palette_splats != null)
		{
			BSGTerrainTools.Float2D[] array = new BSGTerrainTools.Float2D[this.palette_splats.layers.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new BSGTerrainTools.Float2D(this.palette_splats.layers[i].arr);
			}
			BSGTerrainTools.Float3D float3D = new BSGTerrainTools.Float3D(array);
			Vector2Int vector2Int;
			Vector2Int vector2Int2;
			BSGTerrainTools.CalcGridBounds(bounds, this.palette_info.bounds, this.palette_info.splats_resolution, out vector2Int, out vector2Int2);
			float3D.SetLocalRect(vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
			Bounds bounds4 = BSGTerrainTools.SnapSplatsBounds(bounds2, this.gen_terrain.ti);
			float3D.SetWorldRect(bounds4.min.x, bounds4.min.z, bounds4.size.x, bounds4.size.z);
			BSGTerrainTools.Gen3D gen3D = new BSGTerrainTools.Gen3D(float3D);
			if (this.cliff_texture_index >= 0 && this.cliff_texture_index < array.Length && this.cliff_slope_min <= this.cliff_slope_max)
			{
				gen3D.func = delegate(BSGTerrainTools.Gen3D gen, float wx, float wy, int layer, float src_val, float tgt_val, ref float alpha)
				{
					if (alpha <= 0f || alpha >= 1f)
					{
						return tgt_val;
					}
					alpha = 1f;
					if (layer == 0)
					{
						float num = this.gen_terrain.CalcSlope(wx, wy);
						if (num < this.cliff_slope_min)
						{
							gen.val = 0f;
						}
						else if (num < this.cliff_slope_max)
						{
							gen.val = (num - this.cliff_slope_min) / (this.cliff_slope_max - this.cliff_slope_min);
						}
						else
						{
							gen.val = 1f;
						}
					}
					if (layer == this.cliff_texture_index)
					{
						return gen.val;
					}
					return tgt_val * (1f - gen.val);
				};
			}
			BSGTerrainTools.ModifySplats(this.gen_terrain, bounds2, gen3D, gen2D);
		}
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x000C0B08 File Offset: 0x000BED08
	private void ImportTrees(Logic.HexGrid.Coord template_tile, BattleTemplate.TileImportInfo tii)
	{
		List<TreeInstance> list;
		if (!this.palette_trees.TryGetValue(tii.palette_tile, out list))
		{
			return;
		}
		Vector3 a = global::HexGrid.Center(template_tile, 0f);
		float num = global::HexGrid.Radius() / this.palette_grid.tile_radius;
		float num2 = this.gen_terrain.ti.bounds.size.y / this.palette_info.bounds.size.y;
		float f = 0.017453292f * ((float)tii.rot * 60f);
		float num3 = Mathf.Sin(f);
		float num4 = Mathf.Cos(f);
		for (int i = 0; i < list.Count; i++)
		{
			TreeInstance treeInstance = list[i];
			Vector3 a2 = new Vector3(num4 * treeInstance.position.x - num3 * treeInstance.position.z, treeInstance.position.y, num3 * treeInstance.position.x + num4 * treeInstance.position.z);
			Vector3 vector = a + a2 * num;
			treeInstance.position.x = vector.x / this.gen_terrain.td.size.x;
			treeInstance.position.z = vector.z / this.gen_terrain.td.size.z;
			treeInstance.position.y = this.gen_terrain.GetHeight(vector.x, vector.z);
			treeInstance.heightScale *= num2;
			treeInstance.widthScale *= num;
			this.gen_terrain.trees.Add(treeInstance);
		}
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x000C0CC4 File Offset: 0x000BEEC4
	private void ImportObjects(Logic.HexGrid.Coord template_tile, BattleTemplate.TileImportInfo tii)
	{
		string name = string.Concat(new object[]
		{
			"hex_",
			tii.palette_tile.x,
			"_",
			tii.palette_tile.y
		});
		GameObject gameObject = global::Common.FindChildByName(this.palette_objects, name, false, true);
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = global::Common.Spawn(gameObject, false, false);
		gameObject2.transform.parent = base.transform;
		float y = gameObject2.transform.position.y;
		gameObject2.transform.position = global::HexGrid.Center(template_tile, y);
		gameObject2.transform.localEulerAngles = new Vector3(0f, (float)(-(float)tii.rot * 60), 0f);
		float num = global::HexGrid.Radius() / this.palette_grid.tile_radius;
		float y2 = this.gen_terrain.ti.bounds.size.y / this.palette_info.bounds.size.y;
		gameObject2.transform.localScale = new Vector3(num, y2, num);
		this.gen_objects.Add(gameObject2);
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x000C0DF8 File Offset: 0x000BEFF8
	public static Logic.HexGrid.Coord CalcMaxCoord(Vector3 terrain_size, float tile_radius)
	{
		Logic.HexGrid.Coord coord = default(Logic.HexGrid.Coord);
		float num = terrain_size.x - tile_radius * 0.5f;
		float num2 = tile_radius * 3f;
		coord.x = Mathf.FloorToInt(num / num2);
		float num3 = num - num2 * (float)coord.x;
		coord.x *= 2;
		if (num3 < tile_radius * 0.5f)
		{
			coord.x--;
		}
		else if (num3 >= 2f * tile_radius)
		{
			coord.x++;
		}
		float z = terrain_size.z;
		float num4 = tile_radius * 0.866f;
		coord.y = Mathf.FloorToInt(z / num4) - 1;
		return coord;
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x000C0EA0 File Offset: 0x000BF0A0
	public static void CalcYRange(int x, Logic.HexGrid.Coord max_coord, out int min_y, out int max_y)
	{
		bool flag = (x & 1) == 1;
		bool flag2 = (max_coord.y & 1) == 1;
		min_y = (flag ? 1 : 2);
		max_y = ((flag == flag2) ? max_coord.y : (max_coord.y - 1));
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x000C0EDF File Offset: 0x000BF0DF
	public static int RndY(int min, int max)
	{
		return min + Random.Range(0, (max - min) / 2 + 1) * 2;
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x000C0EF4 File Offset: 0x000BF0F4
	public static Logic.HexGrid.Coord RndTile(Logic.HexGrid.Coord max_coord)
	{
		int x = Random.Range(1, max_coord.x + 1);
		int min;
		int max;
		BattleTemplate.CalcYRange(x, max_coord, out min, out max);
		int y = BattleTemplate.RndY(min, max);
		return new Logic.HexGrid.Coord(x, y);
	}

	// Token: 0x04000C43 RID: 3139
	public bool generated;

	// Token: 0x04000C44 RID: 3140
	public string palette_path;

	// Token: 0x04000C45 RID: 3141
	public Logic.HexGrid palette_grid;

	// Token: 0x04000C46 RID: 3142
	public BSGTerrainTools.TerrainInfo palette_info;

	// Token: 0x04000C47 RID: 3143
	public BSGTerrainTools.Float2D palette_heights;

	// Token: 0x04000C48 RID: 3144
	public BSGTerrainTools.Float3D palette_splats;

	// Token: 0x04000C49 RID: 3145
	public Dictionary<Logic.HexGrid.Coord, List<TreeInstance>> palette_trees;

	// Token: 0x04000C4A RID: 3146
	public GameObject palette_objects;

	// Token: 0x04000C4B RID: 3147
	public Logic.HexGroups paletteGroups;

	// Token: 0x04000C4C RID: 3148
	public Dictionary<string, List<HexGroup>> paletteMap;

	// Token: 0x04000C4D RID: 3149
	public Dictionary<string, List<Logic.HexGrid.Coord>> shapeCoord;

	// Token: 0x04000C4E RID: 3150
	public Dictionary<int, string> numOfHexPerShape;

	// Token: 0x04000C4F RID: 3151
	public BSGTerrainTools.TerrainBlock org_terrain;

	// Token: 0x04000C50 RID: 3152
	public BSGTerrainTools.TerrainBlock gen_terrain;

	// Token: 0x04000C51 RID: 3153
	public List<GameObject> gen_objects;

	// Token: 0x04000C52 RID: 3154
	public bool import_heights = true;

	// Token: 0x04000C53 RID: 3155
	public bool import_splats = true;

	// Token: 0x04000C54 RID: 3156
	public bool import_trees = true;

	// Token: 0x04000C55 RID: 3157
	public bool import_objects = true;

	// Token: 0x04000C56 RID: 3158
	public bool multithreaded = true;

	// Token: 0x04000C57 RID: 3159
	public int cliff_texture_index = -1;

	// Token: 0x04000C58 RID: 3160
	public float cliff_slope_min;

	// Token: 0x04000C59 RID: 3161
	public float cliff_slope_max = 40f;

	// Token: 0x04000C5A RID: 3162
	public bool lock_seed;

	// Token: 0x04000C5B RID: 3163
	public int seed;

	// Token: 0x04000C5C RID: 3164
	public bool additive = true;

	// Token: 0x04000C5D RID: 3165
	private Logic.Battle battle;

	// Token: 0x04000C5E RID: 3166
	private Dictionary<Logic.HexGrid.Coord, BattleTemplate.TileImportInfo> import_info;

	// Token: 0x04000C5F RID: 3167
	private Logic.HexGrid.Coord max_template_coord;

	// Token: 0x04000C60 RID: 3168
	private Thread[] terrain_threads;

	// Token: 0x02000690 RID: 1680
	private class TileImportInfo
	{
		// Token: 0x040035F6 RID: 13814
		public Logic.HexGrid.Coord palette_tile;

		// Token: 0x040035F7 RID: 13815
		public int rot;

		// Token: 0x040035F8 RID: 13816
		public byte edges_mask;
	}

	// Token: 0x02000691 RID: 1681
	private class TerrainColumnImport
	{
		// Token: 0x060047FE RID: 18430 RVA: 0x00216580 File Offset: 0x00214780
		public TerrainColumnImport(BattleTemplate template, int x, int step)
		{
			this.template = template;
			this.x = x;
			this.step = step;
		}

		// Token: 0x060047FF RID: 18431 RVA: 0x0021659D File Offset: 0x0021479D
		public void Import()
		{
			while (this.x <= this.template.max_template_coord.x)
			{
				this.template.ImportTerrainColumn(this.x);
				this.x += this.step;
			}
		}

		// Token: 0x040035F9 RID: 13817
		private BattleTemplate template;

		// Token: 0x040035FA RID: 13818
		private int x;

		// Token: 0x040035FB RID: 13819
		private int step;
	}
}
