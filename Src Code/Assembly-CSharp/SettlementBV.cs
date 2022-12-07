using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x020000CC RID: 204
public class SettlementBV : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x0600090F RID: 2319 RVA: 0x0006240A File Offset: 0x0006060A
	public static int total_generation_steps
	{
		get
		{
			return SettlementBV.generationFunctions.Count;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000910 RID: 2320 RVA: 0x00062416 File Offset: 0x00060616
	public static string cur_generation_description
	{
		get
		{
			if (SettlementBV.generationFunctions.Count == 0)
			{
				return "Battle.BattleviewGeneration.GeneratingTerrain";
			}
			if (SettlementBV.cur_generation_step == SettlementBV.generationFunctions.Count)
			{
				return "Battle.BattleviewGeneration.CreatingLogic";
			}
			return SettlementBV.generationFunctions[SettlementBV.cur_generation_step].Item3;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00062455 File Offset: 0x00060655
	private void Update()
	{
		if ((SettlementBV.finished_generation || (BattleMap.battle != null && BattleMap.battle.IsValid() && BattleMap.battle.battle_map_only)) && SettlementBV.OnGenerationComplete != null)
		{
			SettlementBV.OnGenerationComplete();
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0006248E File Offset: 0x0006068E
	public void ResetSeed()
	{
		UnityEngine.Random.InitState(SettlementBV.seed);
		this.available_info = new WeightedRandom<PrefabGrid.Info>(5, SettlementBV.seed);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000624AC File Offset: 0x000606AC
	public void Load()
	{
		if (this.id <= 0)
		{
			this.field = null;
			return;
		}
		this.field = this.Load(this.id, false);
		this.wv_pos = this.field.GetPoint("position", null, true, true, true, '.');
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x000624FC File Offset: 0x000606FC
	public DT.Field Load(int id, bool allow_null = false)
	{
		if (id <= 0 && !allow_null)
		{
			return null;
		}
		List<DT.Field> list = DT.Parser.ReadFile(null, Game.maps_path + "europe/settlements.def", null);
		DT.Field field = null;
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i];
			if (field2.key == "Settlements" && field2.type == "extend")
			{
				for (int j = 0; j < field2.children.Count; j++)
				{
					DT.Field field3 = field2.children[j];
					if (field3.key == "europe")
					{
						field = field3;
						break;
					}
				}
			}
		}
		if (id == -1 && ((field != null) ? field.children : null) != null)
		{
			for (int k = 0; k < field.children.Count; k++)
			{
				DT.Field field4 = field.children[k];
				if (field4.type == "def" && field4.base_path == "Castle")
				{
					return field4;
				}
			}
		}
		if (field == null)
		{
			return null;
		}
		return field.FindChild(id.ToString(), null, true, true, true, '.');
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00062628 File Offset: 0x00060828
	private void LoadSettlementInfo(bool reload_defs)
	{
		if (BattleMap.battle != null && !BattleMap.battle.IsValid())
		{
			BattleMap.SetBattle(null, 0);
		}
		BattleMap battleMap = BattleMap.Get();
		this.pdb = ((battleMap != null) ? battleMap.pdb : null);
		RuntimePathDataBuilder runtimePathDataBuilder = this.pdb;
		if (runtimePathDataBuilder != null)
		{
			runtimePathDataBuilder.Prebake(false);
		}
		battleMap.LoadSDF();
		this.t = global::Common.GetBiggestTerrain();
		if (reload_defs)
		{
			Profile.BeginSection("SettlementBV.LoadField");
			this.Load();
			Profile.EndSection("SettlementBV.LoadField");
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x000626A7 File Offset: 0x000608A7
	public static void ResetGenerationVars()
	{
		SettlementBV.generating = false;
		SettlementBV.finished_generation = false;
		SettlementBV.generationFunctions.Clear();
		SettlementBV.cur_generation_step = 0;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000626C8 File Offset: 0x000608C8
	private void BeginGeneration()
	{
		SettlementBV.generating = true;
		SettlementBV.finished_generation = false;
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		if (this.field != null)
		{
			this.rot = this.field.GetFloat("rotation", null, 0f, true, true, true, '.');
		}
		else
		{
			this.rot = 0f;
		}
		base.transform.eulerAngles = new Vector3(0f, this.rot, 0f);
		this.spawned_grids.Clear();
		this.clusters.Clear();
		SettlementBV.waypoints.Clear();
		this.connected_waypoints.Clear();
		this.outer_roads.Clear();
		this.roads.Clear();
		this.spawned_outer_houses.Clear();
		this.outside_town_connection_paths.Clear();
		this.outside_town_connectors.Clear();
		this.shore_line_pgs.Clear();
		this.ResetSeed();
		SettlementBV.generationFunctions.Clear();
		this.south_camps.Clear();
		this.north_camps.Clear();
		this.highway_roads.Clear();
		this.wall = null;
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(BattleMap.Get().OnGenerationComplete));
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0006282E File Offset: 0x00060A2E
	private void AddGenerationStep(string step_name, SettlementBV.GenerateFunc func, string description = "")
	{
		SettlementBV.generationFunctions.Add(new ValueTuple<string, SettlementBV.GenerateFunc, string>(step_name, func, description));
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00062842 File Offset: 0x00060A42
	private IEnumerator RunGenerationSteps()
	{
		PrefabGrid.UseBattleView(true);
		SettlementBV.cur_generation_step = 0;
		while (SettlementBV.cur_generation_step < SettlementBV.generationFunctions.Count && this.RunGenerationStep(SettlementBV.cur_generation_step))
		{
			if (Application.isPlaying)
			{
				yield return new WaitForEndOfFrame();
			}
			SettlementBV.cur_generation_step++;
		}
		if (Application.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		else
		{
			yield return null;
		}
		this.FinishGeneration();
		yield break;
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00062854 File Offset: 0x00060A54
	private void FinishGeneration()
	{
		SettlementBV.generating = false;
		SettlementBV.finished_generation = true;
		if (Application.isPlaying)
		{
			BattleMap battleMap = BattleMap.Get();
			if (battleMap != null)
			{
				PassableAreaManager paManager = battleMap.paManager;
				if (paManager != null)
				{
					paManager.Clear();
				}
			}
		}
		else
		{
			BattleMap.refresh_graph_settlements = true;
		}
		PrefabGrid.UseBattleView(false);
		if (SettlementBV.OnGenerationComplete != null)
		{
			SettlementBV.OnGenerationComplete();
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x000628B0 File Offset: 0x00060AB0
	private bool RunGenerationStep(int generate_step)
	{
		string item = SettlementBV.generationFunctions[generate_step].Item1;
		SettlementBV.GenerateFunc item2 = SettlementBV.generationFunctions[generate_step].Item2;
		Profile.BeginSection("SettlementBV." + item);
		item2();
		Profile.EndSection("SettlementBV." + item);
		return true;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00062904 File Offset: 0x00060B04
	private DT.Field GetBattleDef()
	{
		Logic.Battle battle = BattleMap.battle;
		bool flag;
		if (battle == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Battle.Def def = battle.def;
			flag = (((def != null) ? def.field : null) != null);
		}
		if (flag)
		{
			return BattleMap.battle.def.field;
		}
		if (this.field == null || !(this.field.base_path == "Castle"))
		{
			return global::Defs.Get(false).dt.Find("OpenField", null);
		}
		if (BattleMap.Get().assault)
		{
			return global::Defs.Get(false).dt.Find("Assault", null);
		}
		return global::Defs.Get(false).dt.Find("BreakSiege", null);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000629B0 File Offset: 0x00060BB0
	public void LoadGenerationSteps(DT.Field battle_def)
	{
		Vars vars = new Vars();
		vars.Set<bool>("is_town", this.field != null && this.field.base_path == "Castle");
		DT.Field field = battle_def.FindChild("BattleviewGeneration", vars, true, true, true, '.');
		field.FindChild("Parameters", vars, true, true, true, '.');
		DT.Field steps_field = field.FindChild("Steps.MandatoryAtStart", vars, true, true, true, '.');
		DT.Field steps_field2 = field.FindChild("Steps.VariableSteps", vars, true, true, true, '.');
		DT.Field steps_field3 = field.FindChild("Steps.MandatoryAtEnd", vars, true, true, true, '.');
		this.AddGenerationSteps(steps_field);
		this.AddGenerationSteps(steps_field2);
		this.AddGenerationSteps(steps_field3);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00062A5C File Offset: 0x00060C5C
	private void AddGenerationSteps(DT.Field steps_field)
	{
		DT.Field field = steps_field;
		DT.Field field2;
		if ((field2 = (field.value.obj_val as DT.Field)) != null)
		{
			field = field2;
		}
		List<DT.Field> list = field.Children();
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field3 = list[i];
			if (!(field3.type != "step"))
			{
				DT.Field field4 = field3.FindChild("loading_screen_tip", null, true, true, true, '.');
				string @string = field3.GetString("func", null, "", true, true, true, '.');
				SettlementBV.GenerateFunc generateFunc = this.FindGenerationFunction(@string);
				if (generateFunc == null)
				{
					Debug.LogError("Missing function " + @string);
				}
				else
				{
					this.AddGenerationStep(field3.key, generateFunc, field4.Path(false, false, '.'));
				}
			}
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00062B24 File Offset: 0x00060D24
	private SettlementBV.GenerateFunc FindGenerationFunction(string key)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num <= 2578356394U)
		{
			if (num <= 1327866616U)
			{
				if (num <= 654563346U)
				{
					if (num <= 135637716U)
					{
						if (num != 85825219U)
						{
							if (num == 135637716U)
							{
								if (key == "Refresh")
								{
									return new SettlementBV.GenerateFunc(this.Refresh);
								}
							}
						}
						else if (key == "SpawnArmyCampOutsideWall")
						{
							return new SettlementBV.GenerateFunc(this.SpawnArmyCampOutsideWall);
						}
					}
					else if (num != 453063929U)
					{
						if (num == 654563346U)
						{
							if (key == "ReduceTerrainLayers")
							{
								return new SettlementBV.GenerateFunc(BattleMap.Get().ReduceTerrainLayers);
							}
						}
					}
					else if (key == "GenerateRoads")
					{
						return new SettlementBV.GenerateFunc(this.GenerateRoads);
					}
				}
				else if (num <= 695775757U)
				{
					if (num != 676498961U)
					{
						if (num == 695775757U)
						{
							if (key == "GenerateMapBounds")
							{
								return new SettlementBV.GenerateFunc(this.GenerateMapBounds);
							}
						}
					}
					else if (key == "Scale")
					{
						return new SettlementBV.GenerateFunc(this.Scale);
					}
				}
				else if (num != 751293055U)
				{
					if (num != 891503710U)
					{
						if (num == 1327866616U)
						{
							if (key == "CreateRandomRoad")
							{
								return new SettlementBV.GenerateFunc(this.CreateRandomRoad);
							}
						}
					}
					else if (key == "SpawnRandomHouses")
					{
						return new SettlementBV.GenerateFunc(this.SpawnRandomHouses);
					}
				}
				else if (key == "LoadCitadel")
				{
					return new SettlementBV.GenerateFunc(this.LoadCitadel);
				}
			}
			else if (num <= 2150849505U)
			{
				if (num <= 1532272002U)
				{
					if (num != 1331092504U)
					{
						if (num == 1532272002U)
						{
							if (key == "SpawnTowers")
							{
								return new SettlementBV.GenerateFunc(this.SpawnAdditionalTowers);
							}
						}
					}
					else if (key == "SpawnUniqueBuildings")
					{
						return new SettlementBV.GenerateFunc(this.SpawnUniqueBuildings);
					}
				}
				else if (num != 1555113691U)
				{
					if (num != 2110707972U)
					{
						if (num == 2150849505U)
						{
							if (key == "SpawnRemoveTrees")
							{
								return new SettlementBV.GenerateFunc(this.SpawnTrees);
							}
						}
					}
					else if (key == "LoadHouses")
					{
						return new SettlementBV.GenerateFunc(this.LoadHouses);
					}
				}
				else if (key == "GenerateNeighbourhood")
				{
					return new SettlementBV.GenerateFunc(this.GenerateNeighbourhoodRoads);
				}
			}
			else if (num <= 2318836513U)
			{
				if (num != 2278275637U)
				{
					if (num == 2318836513U)
					{
						if (key == "LoadWall")
						{
							return new SettlementBV.GenerateFunc(this.LoadWall);
						}
					}
				}
				else if (key == "BuildMasksAllSettlements")
				{
					return new SettlementBV.GenerateFunc(this.BuildMasksAllSettlements);
				}
			}
			else if (num != 2481388584U)
			{
				if (num != 2567505030U)
				{
					if (num == 2578356394U)
					{
						if (key == "CalcLevel")
						{
							return new SettlementBV.GenerateFunc(this.CalcLevel);
						}
					}
				}
				else if (key == "SpawnOpenFieldCamps")
				{
					return new SettlementBV.GenerateFunc(this.CalcOpenFieldCampsPositions);
				}
			}
			else if (key == "AddTerrainSplats")
			{
				return new SettlementBV.GenerateFunc(this.ChangeTerrain);
			}
		}
		else if (num <= 3356470675U)
		{
			if (num <= 3029256377U)
			{
				if (num <= 2788599756U)
				{
					if (num != 2608986513U)
					{
						if (num == 2788599756U)
						{
							if (key == "CalcNorthReinforcementPositions")
							{
								return new SettlementBV.GenerateFunc(this.CalcNorthReinforcementPositions);
							}
						}
					}
					else if (key == "ConnectRoadsHouses")
					{
						return new SettlementBV.GenerateFunc(this.ConnectRoadsHouses);
					}
				}
				else if (num != 2970795517U)
				{
					if (num != 3026806235U)
					{
						if (num == 3029256377U)
						{
							if (key == "LoadUniqueContainer")
							{
								return new SettlementBV.GenerateFunc(this.LoadUniqueContainer);
							}
						}
					}
					else if (key == "SnapHeights")
					{
						return new SettlementBV.GenerateFunc(this.SnapHeights);
					}
				}
				else if (key == "SpawnCapturePoints")
				{
					return new SettlementBV.GenerateFunc(this.SpawnCapturePoints);
				}
			}
			else if (num <= 3137170331U)
			{
				if (num != 3037601765U)
				{
					if (num == 3137170331U)
					{
						if (key == "GrowPG")
						{
							return new SettlementBV.GenerateFunc(this.GrowPG);
						}
					}
				}
				else if (key == "GenerateShoreline")
				{
					return new SettlementBV.GenerateFunc(this.GenerateShorelineDecorations);
				}
			}
			else if (num != 3180612482U)
			{
				if (num != 3207810000U)
				{
					if (num == 3356470675U)
					{
						if (key == "CalcNorthBreakSiegePositions")
						{
							return new SettlementBV.GenerateFunc(this.CalcNorthBreakSiegePositions);
						}
					}
				}
				else if (key == "BuildMasksWithoutCitadel")
				{
					return new SettlementBV.GenerateFunc(this.BuildMasksWithoutCitadel);
				}
			}
			else if (key == "CalcSouthReinforcementPositions")
			{
				return new SettlementBV.GenerateFunc(this.CalcSouthReinforcementPositions);
			}
		}
		else if (num <= 3791116325U)
		{
			if (num <= 3473398483U)
			{
				if (num != 3390704116U)
				{
					if (num == 3473398483U)
					{
						if (key == "SetLevel")
						{
							return new SettlementBV.GenerateFunc(this.SetLevel);
						}
					}
				}
				else if (key == "SpawnGates")
				{
					return new SettlementBV.GenerateFunc(this.SpawnGates);
				}
			}
			else if (num != 3490972742U)
			{
				if (num != 3666769387U)
				{
					if (num == 3791116325U)
					{
						if (key == "LoadArchitecturesOpenfield")
						{
							return new SettlementBV.GenerateFunc(this.LoadArchitecturesOpenfield);
						}
					}
				}
				else if (key == "LoadPathfinding")
				{
					return new SettlementBV.GenerateFunc(this.pdb.LoadPathfinding);
				}
			}
			else if (key == "ValidatePrefabGrids")
			{
				return new SettlementBV.GenerateFunc(this.ValidatePrefabGrids);
			}
		}
		else if (num <= 3919138463U)
		{
			if (num != 3900029272U)
			{
				if (num == 3919138463U)
				{
					if (key == "ConnectRandomHouses")
					{
						return new SettlementBV.GenerateFunc(this.ConnectRandomHouses);
					}
				}
			}
			else if (key == "LoadSiegeArchitectures")
			{
				return new SettlementBV.GenerateFunc(this.LoadSiegeArchitectures);
			}
		}
		else if (num != 4076963241U)
		{
			if (num != 4267441508U)
			{
				if (num == 4282638157U)
				{
					if (key == "ScaleWallCorners")
					{
						return new SettlementBV.GenerateFunc(this.ScaleWallCorners);
					}
				}
			}
			else if (key == "CalcTreeGrid")
			{
				return new SettlementBV.GenerateFunc(this.pdb.BuildTreeArr);
			}
		}
		else if (key == "ClearRoads")
		{
			return new SettlementBV.GenerateFunc(this.pdb.ClearRoads);
		}
		return null;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x000632F5 File Offset: 0x000614F5
	public void Generate(bool reload_defs = true)
	{
		this.LoadSettlementInfo(reload_defs);
		this.BeginGeneration();
		this.LoadGenerationSteps(this.GetBattleDef());
		base.StartCoroutine(this.RunGenerationSteps());
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00063320 File Offset: 0x00061520
	private void ConnectRandomHouses()
	{
		SettlementBV.<>c__DisplayClass175_0 CS$<>8__locals1 = new SettlementBV.<>c__DisplayClass175_0();
		CS$<>8__locals1.<>4__this = this;
		PathUtils.AStarPathfinder astarPathfinder = this.CreateRoadPathfinder(30f, 0f);
		float waterLevel = MapData.GetWaterLevel();
		CS$<>8__locals1.terrain = Terrain.activeTerrain;
		Bounds bounds = CS$<>8__locals1.terrain.terrainData.bounds;
		CS$<>8__locals1.terrain_bounds_sdf = new BoxSDF2D(new float3(bounds.center).xz, new float3(bounds.extents).xz);
		foreach (PrefabGrid prefabGrid in this.spawned_outer_houses)
		{
			astarPathfinder.AddObstacle(prefabGrid.transform.position.xz, astarPathfinder.grid_cell_size * 0.5f);
		}
		foreach (List<float2> source in this.outside_town_connection_paths)
		{
			astarPathfinder.AddObstacle(source.First<float2>(), astarPathfinder.grid_cell_size * 0.6f);
			astarPathfinder.AddObstacle(source.Last<float2>(), astarPathfinder.grid_cell_size * 0.6f);
		}
		List<PrefabGrid> list = new List<PrefabGrid>();
		list.AddRange(this.spawned_outer_houses);
		list.Sort((PrefabGrid a, PrefabGrid b) => -(int)math.sign(base.<ConnectRandomHouses>g__ClosestRoad|1(a) - base.<ConnectRandomHouses>g__ClosestRoad|1(b)));
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			PrefabGrid prefabGrid2 = list[index];
			list.RemoveAt(index);
			float house_radius = prefabGrid2.ScaledRadius();
			float2 house_pos = math.float3(prefabGrid2.transform.position).xz;
			List<float2> path = new List<float2>();
			List<float2> path2 = new List<float2>();
			astarPathfinder.RemoveObstacle(house_pos, astarPathfinder.grid_cell_size);
			bool flag = astarPathfinder.TryFindPath(house_pos, new Func<float2, float>(CS$<>8__locals1.<ConnectRandomHouses>g__ClosestRoadSDF|0), ref path2, false);
			astarPathfinder.AddObstacle(house_pos, astarPathfinder.grid_cell_size);
			if (flag)
			{
				path = path2;
				if (path.Count == 1)
				{
					path.Add(path[0]);
				}
				path[0] = house_pos;
				path[path.Count - 1] = SDFUtils.SnapToSDFByMarching(new Func<float2, float>(CS$<>8__locals1.<ConnectRandomHouses>g__ClosestRoadSDF|0), path.Last<float2>(), 10, 0.9f);
				PathUtils.MakePathDense(path, this.average_road_segment);
				PathUtils.SmoothPath((float2 p) => math.smoothstep(2f, 30f, base.<ConnectRandomHouses>g__DistanceToEnds|5(p)), path, 2, 5);
				PathUtils.MakePathDense(path, this.average_road_segment);
				PathUtils.SmoothPath((float2 p) => 1f, path, 2, 1);
				for (int i = 1; i < path.Count - 1; i++)
				{
					path[i] = DirtRoadGraph.PointDistortion(path[i].xxy, 0.5f, 2f).xz;
				}
				path[0] = house_pos;
				path[path.Count - 1] = SDFUtils.SnapToSDFByMarching(new Func<float2, float>(CS$<>8__locals1.<ConnectRandomHouses>g__ClosestRoadSDF|0), path.Last<float2>(), 10, 0.9f);
				PathUtils.SmoothPath((float2 p) => math.smoothstep(2f, 3f, base.<ConnectRandomHouses>g__DistanceToEnds|5(p)), path, 1, 1);
				IEnumerable<float2> range = path.GetRange(2, path.Count - 4);
				Func<float2, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__9) == null)
				{
					predicate = (CS$<>8__locals1.<>9__9 = ((float2 p) => CS$<>8__locals1.terrain_bounds_sdf.SDF2D(p) + 20f > 0f));
				}
				if (!range.Any(predicate))
				{
					IEnumerable<float2> source2 = path.SkipWhile((float2 p) => math.distance(house_pos, p) < house_radius * 0.6f);
					Func<float2, float3> selector;
					if ((selector = CS$<>8__locals1.<>9__11) == null)
					{
						selector = (CS$<>8__locals1.<>9__11 = ((float2 p) => global::Common.SnapToTerrain(math.float3(p.x, 0f, p.y), 0f, CS$<>8__locals1.terrain, 0f, false)));
					}
					List<float3> list2 = source2.Select(selector).ToList<float3>();
					if (list2.Count >= 3)
					{
						if (list2.Count > 4)
						{
							if (list2.GetRange(2, list2.Count - 4).Min((float3 p) => p.y) < waterLevel)
							{
								continue;
							}
						}
						float2 first = path.First<float2>();
						float2 last = path.Last<float2>();
						List<float2> list3 = (from p in path
						where math.min(math.distance(p, first), math.distance(p, last)) > 60f
						select p).ToList<float2>();
						if (list3.Count > 0)
						{
							this.outside_town_connection_paths.Add(list3);
							this.outside_town_connectors.Add(new PolygonalChainSDF2D(list3));
						}
						SettlementBV.RoadBV roadBV = this.CreateRoadDecal(list2, null, false, SettlementBV.RoadBV.Type.Subdivision, this.subdivisionPrefab);
						if (roadBV != null)
						{
							this.roads.Add(roadBV);
						}
					}
				}
			}
		}
		this.pdb.BuildRoads((from r in this.roads
		select r.path).ToList<TerrainPath>());
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000638D8 File Offset: 0x00061AD8
	private void ConnectRoadsHouses()
	{
		RuntimePathDataBuilder runtimePathDataBuilder = BattleMap.Get().pdb;
		if (runtimePathDataBuilder == null)
		{
			return;
		}
		if (runtimePathDataBuilder.lpf == null)
		{
			return;
		}
		float num = this.min_road_distance * this.min_road_distance;
		float num2 = this.max_road_distance * this.max_road_distance;
		List<ValueTuple<RoadWaypoint, RoadWaypoint, float, List<RoadWaypoint>>> list = new List<ValueTuple<RoadWaypoint, RoadWaypoint, float, List<RoadWaypoint>>>();
		new Dictionary<List<RoadWaypoint>, int>();
		for (int i = 0; i < SettlementBV.waypoints.Count; i++)
		{
			RoadWaypoint roadWaypoint = SettlementBV.waypoints[i];
			Vector3 position = roadWaypoint.transform.position;
			for (int j = i + 1; j < SettlementBV.waypoints.Count; j++)
			{
				RoadWaypoint roadWaypoint2 = SettlementBV.waypoints[j];
				Vector3 position2 = roadWaypoint2.transform.position;
				float num3 = Vector3.SqrMagnitude(position - position2);
				if (num3 <= num2 && num3 >= num)
				{
					list.Add(new ValueTuple<RoadWaypoint, RoadWaypoint, float, List<RoadWaypoint>>(roadWaypoint, roadWaypoint2, num3, null));
				}
			}
		}
		list.Sort((ValueTuple<RoadWaypoint, RoadWaypoint, float, List<RoadWaypoint>> a, ValueTuple<RoadWaypoint, RoadWaypoint, float, List<RoadWaypoint>> b) => -a.Item3.CompareTo(b.Item3));
		for (int k = 0; k < list.Count; k++)
		{
			RoadWaypoint item = list[k].Item1;
			RoadWaypoint item2 = list[k].Item2;
			int num4;
			if (!this.connected_waypoints.TryGetValue(item, out num4))
			{
				num4 = 0;
				this.connected_waypoints[item] = num4;
			}
			int num5;
			if (!this.connected_waypoints.TryGetValue(item2, out num5))
			{
				num5 = 0;
				this.connected_waypoints[item2] = num5;
			}
			if (num4 < this.max_roads_per_waypoint && num5 < this.max_roads_per_waypoint)
			{
				Vector3 position3 = item.transform.position;
				Vector3 position4 = item2.transform.position;
				if (this.CreateRoad(position3, position4, false, SettlementBV.RoadBV.Type.Waypoints, this.waypointPrefab) != null)
				{
					Dictionary<RoadWaypoint, int> dictionary = this.connected_waypoints;
					RoadWaypoint key = item;
					int num6 = dictionary[key];
					dictionary[key] = num6 + 1;
					Dictionary<RoadWaypoint, int> dictionary2 = this.connected_waypoints;
					key = item2;
					num6 = dictionary2[key];
					dictionary2[key] = num6 + 1;
				}
			}
		}
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x00063AF8 File Offset: 0x00061CF8
	private void CreateRandomRoad()
	{
		SettlementBV.<>c__DisplayClass177_0 CS$<>8__locals1 = new SettlementBV.<>c__DisplayClass177_0();
		if (this.roads_obj == null)
		{
			this.roads_obj = new GameObject("Roads").transform;
			this.roads_obj.transform.SetParent(base.transform);
		}
		else
		{
			global::Common.DestroyChildren(this.roads_obj.gameObject);
		}
		CS$<>8__locals1.terrain_bounds = Terrain.activeTerrain.terrainData.bounds;
		float x = UnityEngine.Random.Range(0f, 3.1415927f);
		float2 @float = math.mul(new float2x2(math.cos(x), math.sin(x), -math.sin(x), math.cos(x)), new float2(1f, 0f));
		-@float;
		CS$<>8__locals1.terrain_bounds_sdf = new BoxSDF2D(math.float3(CS$<>8__locals1.terrain_bounds.center).xz, math.float3(CS$<>8__locals1.terrain_bounds.extents).xz);
		CS$<>8__locals1.search_direction = math.normalize(@float);
		float2 xz = math.float3(CS$<>8__locals1.terrain_bounds.center).xz;
		PathUtils.AStarPathfinder astarPathfinder = this.CreateRoadPathfinder(30f, 0.05f);
		astarPathfinder.RemoveObstacle(xz, astarPathfinder.grid_cell_size);
		foreach (PrefabGrid prefabGrid in this.spawned_outer_houses)
		{
			astarPathfinder.AddObstacle(new float3(prefabGrid.transform.position).xz, prefabGrid.ScaledRadius());
		}
		float2 start_pos = xz;
		float2 end_pos = xz;
		List<float2> list = new List<float2>();
		if (astarPathfinder.TryFindPath(xz, new Func<float2, float>(CS$<>8__locals1.<CreateRandomRoad>g__PathfindingTargetFunction|0), ref list, true))
		{
			start_pos = SDFUtils.SnapToSDFByMarching((float2 p) => -CS$<>8__locals1.terrain_bounds_sdf.SDF2D(p) - 20f, list.Last<float2>(), 10, 0.6f);
		}
		CS$<>8__locals1.search_direction = -CS$<>8__locals1.search_direction;
		if (astarPathfinder.TryFindPath(xz, new Func<float2, float>(CS$<>8__locals1.<CreateRandomRoad>g__PathfindingTargetFunction|0), ref list, true))
		{
			end_pos = SDFUtils.SnapToSDFByMarching((float2 p) => -CS$<>8__locals1.terrain_bounds_sdf.SDF2D(p) - 20f, list.Last<float2>(), 10, 0.6f);
		}
		if (astarPathfinder.TryFindPath(start_pos, end_pos, ref list, false))
		{
			PathUtils.MakePathDense(list, this.average_road_segment);
			PathUtils.SmoothPath((float2 p) => 1f, list, 7, 2);
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = DirtRoadGraph.PointDistortion(list[i], 1f, 1f);
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (CS$<>8__locals1.terrain_bounds_sdf.SDF2D(list[j]) + 20f > 0f)
				{
					List<float2> list2 = list;
					int index = j;
					Func<float2, float> sdf;
					if ((sdf = CS$<>8__locals1.<>9__6) == null)
					{
						sdf = (CS$<>8__locals1.<>9__6 = ((float2 p) => CS$<>8__locals1.terrain_bounds_sdf.SDF2D(p) + 20f));
					}
					list2[index] = SDFUtils.SnapToSDFByMarching(sdf, list[j], 5, 1f);
				}
			}
			this.outside_town_connection_paths.Add(list);
			this.outside_town_connectors.Add(new PolygonalChainSDF2D(list));
			List<float3> path3D = (from p in list
			select global::Common.SnapToTerrain(p.xxy, 0f, null, -1f, false)).ToList<float3>();
			List<Color> colors = path3D.Select(delegate(float3 p)
			{
				float x2 = math.min(math.distance(p, path3D.First<float3>()), math.distance(p, path3D.Last<float3>()));
				return new Color(1f, 1f, 1f, math.smoothstep(0f, 10f, x2));
			}).ToList<Color>();
			SettlementBV.RoadBV roadBV = this.CreateRoadDecal(path3D, colors, false, SettlementBV.RoadBV.Type.Subdivision, this.subdivisionPrefab);
			if (roadBV != null)
			{
				this.roads.Add(roadBV);
			}
		}
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00063EE0 File Offset: 0x000620E0
	private void LoadArchitecturesHouses()
	{
		DT.Field field = this.field;
		if (field == null)
		{
			int num = -1;
			if (Application.isPlaying)
			{
				Logic.Battle battle = BattleMap.battle;
				if (battle == null)
				{
					return;
				}
				Logic.Realm realm = battle.GetRealm();
				if (((realm != null) ? realm.castle : null) == null)
				{
					return;
				}
				num = realm.castle.GetNid(true);
			}
			field = this.Load(num, true);
		}
		DT.Field field2 = (field != null) ? field.FindChild("houses", null, true, true, true, '.') : null;
		if (field2 != null)
		{
			this.houses_architecture = DT.Unquote(field2.value_str);
			this.house_infos.Clear();
			for (int i = 0; i < this.house_types.Count; i++)
			{
				string type = this.house_types[i];
				string text = PrefabGrid.ResolveArchitecture(type, this.houses_architecture);
				if (text != null)
				{
					this.house_infos.Add(PrefabGrid.Info.Get(type, text, false));
				}
			}
			float num2 = float.MaxValue;
			for (int j = 0; j < this.house_infos.Count; j++)
			{
				PrefabGrid.Info info = this.house_infos[j];
				float num3 = (info.rect_width + info.rect_height) / 2f;
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			this.smallest_avg_house_size = num2;
		}
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00064020 File Offset: 0x00062220
	private void LoadSiegeArchitectures()
	{
		this.LoadArchitecturesHouses();
		DT.Field field = this.field.FindChild("citadel", null, true, true, true, '.');
		if (field != null)
		{
			this.citadel_architecture = PrefabGrid.ResolveArchitecture("Citadels", field.GetString("architecture", null, "", true, true, true, '.'));
		}
		DT.Field field2 = this.field.FindChild("wall", null, true, true, true, '.');
		if (field2 != null)
		{
			string @string = field2.GetString("towers_architecture", null, "", true, true, true, '.');
			this.towers_architecture = PrefabGrid.ResolveArchitecture("Towers", @string);
			this.gates_architecture = PrefabGrid.ResolveArchitecture("Gates", @string);
			this.walls_architecture = PrefabGrid.ResolveArchitecture("Walls", field2.GetString("segments_architecture", null, "", true, true, true, '.'));
		}
		this.road_infos.Clear();
		for (int i = 0; i < this.road_splats.Length; i++)
		{
			string text = PrefabGrid.ResolveArchitecture(this.road_splats[i], this.houses_architecture);
			if (text != null)
			{
				PrefabGrid.Info info = PrefabGrid.Info.Get(this.road_splats[i], text, false);
				if (info != null)
				{
					this.road_infos.Add(info);
				}
			}
		}
		this.LoadArchitecturesOpenfield();
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0006414C File Offset: 0x0006234C
	private void LoadArchitecturesOpenfield()
	{
		this.LoadArchitectureCapturePoint();
		this.LoadArchitecturesHouses();
		this.LoadArchitectureDock();
		string text = PrefabGrid.ResolveArchitecture("ArmyCamp", "Common");
		if (text != null)
		{
			this.army_camp_info = PrefabGrid.Info.Get("ArmyCamp", text, false);
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00064190 File Offset: 0x00062390
	private void LoadArchitectureDock()
	{
		string text = PrefabGrid.ResolveArchitecture("Dock", this.houses_architecture);
		if (text != null)
		{
			PrefabGrid.Info info = PrefabGrid.Info.Get("Dock", text, false);
			this.dock_info = info;
		}
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x000641C8 File Offset: 0x000623C8
	private void LoadArchitectureCapturePoint()
	{
		PrefabGrid.Info.Get("CapturePoints", "Common", false);
		Dictionary<string, PrefabGrid.Info> registry = PrefabGrid.Info.GetRegistry();
		if (registry == null)
		{
			return;
		}
		List<PrefabGrid.Info> list = new List<PrefabGrid.Info>();
		foreach (KeyValuePair<string, PrefabGrid.Info> keyValuePair in registry)
		{
			PrefabGrid.Info value = keyValuePair.Value;
			if (value.capture_point == PrefabGrid.Info.CapturePoint.Yes)
			{
				list.Add(value);
			}
		}
		this.capture_point_info = list;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00064254 File Offset: 0x00062454
	private void BuildMasksWithoutCitadel()
	{
		BattleMap battleMap = BattleMap.Get();
		RuntimePathDataBuilder runtimePathDataBuilder = (battleMap != null) ? battleMap.pdb : null;
		if (this.citadel != null)
		{
			this.citadel.gameObject.SetActive(false);
		}
		runtimePathDataBuilder.BuildMasks(false, true, true);
		if (this.citadel != null)
		{
			this.citadel.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x000642B9 File Offset: 0x000624B9
	private void BuildMasksAllSettlements()
	{
		BattleMap battleMap = BattleMap.Get();
		((battleMap != null) ? battleMap.pdb : null).BuildMasks(false, true, false);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x000642D8 File Offset: 0x000624D8
	private void SnapHeights()
	{
		for (int i = 0; i < this.spawned_grids.Count; i++)
		{
			global::Common.SnapToTerrain(this.spawned_grids[i].gameObject, 0f, null, -1f);
		}
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0006431C File Offset: 0x0006251C
	private void ChangeTerrain()
	{
		if (this.src_terrain == null)
		{
			return;
		}
		TerrainData terrainData = global::Common.GetBiggestTerrain().terrainData;
		BSGTerrainTools.ApplySplatMaps(this.wall, this.src_terrain, terrainData);
		this.highway_roads.Clear();
		BSGTerrainTools.ApplySplatMaps(this.spawned_grids, this.src_terrain, terrainData);
		List<TerrainPath> list = new List<TerrainPath>();
		for (int i = 0; i < this.roads.Count; i++)
		{
			SettlementBV.RoadBV roadBV = this.roads[i];
			if (!(((roadBV != null) ? roadBV.path : null) == null))
			{
				list.Add(this.roads[i].path);
			}
		}
		this.pdb.BuildRoads(list);
		for (int j = 0; j < terrainData.detailPrototypes.Length; j++)
		{
			int[,] detailLayer = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, j);
			for (int k = 0; k < this.pdb.cells.Length; k++)
			{
				if ((this.pdb.cells[k].bits & 1) != 0)
				{
					float num = (float)(k % this.pdb.width);
					int num2 = k / this.pdb.width;
					int num3 = (int)(num * ((float)terrainData.detailWidth / (float)this.pdb.width));
					int num4 = (int)((float)num2 * ((float)terrainData.detailHeight / (float)this.pdb.height));
					detailLayer[num4, num3] = 0;
				}
			}
			terrainData.SetDetailLayer(0, 0, j, detailLayer);
		}
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x000644A8 File Offset: 0x000626A8
	private void SpawnTrees()
	{
		Profile.BeginSection("SettlementBV.SpawnTrees.ImportTrees");
		BSGTreesGrid bsgtreesGrid = new BSGTreesGrid();
		bsgtreesGrid.ImportTrees(this.t, 16f);
		Profile.EndSection("SettlementBV.SpawnTrees.ImportTrees");
		List<float> list = new List<float>();
		float num = 0f;
		for (int i = 0; i < this.t.terrainData.treePrototypes.Length; i++)
		{
			float num2 = Vector3.Magnitude(this.t.terrainData.treePrototypes[i].prefab.GetComponentInChildren<Renderer>().bounds.extents);
			if (num2 > 10f)
			{
				num2 = 10f;
			}
			list.Add(num2);
			if (num2 > num)
			{
				num = num2;
			}
		}
		num *= 2f;
		Profile.BeginSection("SettlementBV.SpawnTrees.ClearFromPGs");
		for (int j = 0; j < this.spawned_grids.Count; j++)
		{
			PrefabGrid prefabGrid = this.spawned_grids[j];
			float num3 = (prefabGrid.ScaledRadiusInner() + prefabGrid.ScaledRadius()) / 2f;
			this.DelTrees(prefabGrid.transform.position, null, num3 + num, bsgtreesGrid, list);
		}
		Profile.EndSection("SettlementBV.SpawnTrees.ClearFromPGs");
		if (this.wall != null)
		{
			Profile.BeginSection("SettlementBV.SpawnTrees.ClearFromWall");
			Transform transform = this.wall.transform.Find("_segments");
			if (transform != null)
			{
				for (int k = 0; k < transform.childCount; k++)
				{
					Transform child = transform.GetChild(k);
					this.DelTrees(child.position, null, 10f + num, bsgtreesGrid, list);
				}
			}
			for (int l = 0; l < this.wall.corners.Count; l++)
			{
				WallCorner wallCorner = this.wall.corners[l];
				MeshRenderer[] componentsInChildren = wallCorner.gameObject.GetComponentsInChildren<MeshRenderer>();
				Bounds bounds = new Bounds(wallCorner.transform.position, Vector3.zero);
				for (int m = 0; m < componentsInChildren.Length; m++)
				{
					bounds.Encapsulate(componentsInChildren[m].bounds);
				}
				this.DelTrees(wallCorner.transform.position, null, Mathf.Max(bounds.extents.x, bounds.extents.z) + num, bsgtreesGrid, list);
			}
			Profile.EndSection("SettlementBV.SpawnTrees.ClearFromWall");
		}
		Profile.BeginSection("SettlementBV.SpawnTrees.RemoveFromRoads");
		for (int n = 0; n < this.roads.Count; n++)
		{
			TerrainPath path = this.roads[n].path;
			float num4 = 0f;
			do
			{
				Vector3 ptw;
				Vector3 vector;
				TerrainPathFinder.GetPathPoint(path.path_points, num4, out ptw, out vector, path.step);
				this.DelTrees(ptw, null, path.width + num, bsgtreesGrid, list);
				num4 += path.width;
			}
			while (num4 < path.path_len);
		}
		Profile.EndSection("SettlementBV.SpawnTrees.RemoveFromRoads");
		if (this.trees_on_roads)
		{
			Profile.BeginSection("SettlementBV.SpawnTrees.SpawnOnRoads");
			if (this.trees_linear)
			{
				this.SpawnTreesLinear(bsgtreesGrid);
			}
			else
			{
				this.SpawnTreesViaRadius(bsgtreesGrid);
			}
			Profile.EndSection("SettlementBV.SpawnTrees.SpawnOnRoads");
		}
		Profile.BeginSection("SettlementBV.SpawnTrees.Apply");
		bsgtreesGrid.ApplyToUnity(this.t);
		bsgtreesGrid.ApplyToTresBatching(this.t);
		Profile.EndSection("SettlementBV.SpawnTrees.Apply");
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x000647F4 File Offset: 0x000629F4
	private void SpawnTreesViaRadius(BSGTreesGrid grid)
	{
		int max = this.t.terrainData.treePrototypes.Length;
		RuntimePathDataBuilder runtimePathDataBuilder = BattleMap.Get().pdb;
		for (int i = 0; i < this.roads.Count; i++)
		{
			if (this.roads[i].type != SettlementBV.RoadBV.Type.Waypoints || this.trees_on_waypoint_roads)
			{
				TerrainPath path = this.roads[i].path;
				float num = 0f;
				do
				{
					Vector3 vector;
					Vector3 a;
					TerrainPathFinder.GetPathPoint(path.path_points, num, out vector, out a, path.step);
					Vector3 vector2 = a - vector;
					vector2.y = 0f;
					vector2 = vector2.normalized;
					vector2 = Vector3.Cross(vector2, Vector3.up);
					num += UnityEngine.Random.Range(this.tree_dist_min, this.tree_dist_max);
					Vector3 vector3 = vector + vector2 * path.width / 2f;
					int num2 = UnityEngine.Random.Range(this.tree_min_count, this.tree_max_count);
					for (int j = 0; j < num2; j++)
					{
						Point pt = global::Common.RandomOnUnitCircle();
						Vector3 point = global::Common.SnapToTerrain(vector3 + pt * UnityEngine.Random.Range(this.tree_min_radius, this.tree_max_radius), 0f, null, -1f, false);
						if ((runtimePathDataBuilder.GetCell(point).bits & 33) == 0)
						{
							float num3 = UnityEngine.Random.Range(this.min_tree_scale, this.max_tree_scale);
							BSGTreesGrid.TreeInfo ti = new BSGTreesGrid.TreeInfo(UnityEngine.Random.Range(0, max), vector3, UnityEngine.Random.Range(0f, 360f), num3, num3);
							grid.AddTree(ti);
						}
					}
				}
				while (num < path.path_len);
				num = 0f;
				do
				{
					Vector3 vector4;
					Vector3 a2;
					TerrainPathFinder.GetPathPoint(path.path_points, num, out vector4, out a2, path.step);
					Vector3 vector5 = a2 - vector4;
					vector5.y = 0f;
					vector5 = vector5.normalized;
					vector5 = Vector3.Cross(vector5, Vector3.up);
					num += UnityEngine.Random.Range(this.tree_dist_min, this.tree_dist_max);
					Vector3 a3 = vector4 - vector5 * path.width / 2f;
					int num4 = UnityEngine.Random.Range(this.tree_min_count, this.tree_max_count);
					for (int k = 0; k < num4; k++)
					{
						Point pt2 = global::Common.RandomOnUnitCircle();
						Vector3 vector6 = global::Common.SnapToTerrain(a3 + pt2 * UnityEngine.Random.Range(this.tree_min_radius, this.tree_max_radius), 0f, null, -1f, false);
						if ((runtimePathDataBuilder.GetCell(vector6).bits & 33) == 0)
						{
							float num5 = UnityEngine.Random.Range(this.min_tree_scale, this.max_tree_scale);
							BSGTreesGrid.TreeInfo ti2 = new BSGTreesGrid.TreeInfo(UnityEngine.Random.Range(0, max), vector6, UnityEngine.Random.Range(0f, 360f), num5, num5);
							grid.AddTree(ti2);
						}
					}
				}
				while (num < path.path_len);
			}
		}
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00064B00 File Offset: 0x00062D00
	private void SpawnTreesLinear(BSGTreesGrid grid)
	{
		int max = this.t.terrainData.treePrototypes.Length;
		RuntimePathDataBuilder runtimePathDataBuilder = BattleMap.Get().pdb;
		for (int i = 0; i < this.roads.Count; i++)
		{
			if (this.roads[i].type != SettlementBV.RoadBV.Type.Waypoints || this.trees_on_waypoint_roads)
			{
				TerrainPath path = this.roads[i].path;
				float num = 0f;
				do
				{
					int num2 = UnityEngine.Random.Range(this.tree_min_count, this.tree_max_count);
					for (int j = 0; j < num2; j++)
					{
						float num3 = UnityEngine.Random.Range(this.tree_linear_offset_min, this.tree_linear_offset_max);
						Vector3 vector;
						Vector3 a;
						TerrainPathFinder.GetPathPoint(path.path_points, num + num3, out vector, out a, path.step);
						Vector3 vector2 = a - vector;
						vector2.y = 0f;
						vector2 = vector2.normalized;
						vector2 = Vector3.Cross(vector2, Vector3.up);
						Vector3 vector3 = vector + vector2 * (5f + path.width / 2f);
						if ((runtimePathDataBuilder.GetCell(vector3).bits & 33) == 0 && !this.TooCloseToWall(vector3, 1f, 1f, false, 0f))
						{
							float num4 = UnityEngine.Random.Range(this.min_tree_scale, this.max_tree_scale);
							BSGTreesGrid.TreeInfo ti = new BSGTreesGrid.TreeInfo(UnityEngine.Random.Range(0, max), vector3, UnityEngine.Random.Range(0f, 360f), num4, num4);
							grid.AddTree(ti);
						}
					}
					num += UnityEngine.Random.Range(this.tree_dist_min, this.tree_dist_max);
				}
				while (num < path.path_len);
				num = 0f;
				do
				{
					int num5 = UnityEngine.Random.Range(this.tree_min_count, this.tree_max_count);
					for (int k = 0; k < num5; k++)
					{
						float num6 = UnityEngine.Random.Range(this.tree_linear_offset_min, this.tree_linear_offset_max);
						Vector3 vector4;
						Vector3 a2;
						TerrainPathFinder.GetPathPoint(path.path_points, num + num6, out vector4, out a2, path.step);
						Vector3 vector5 = a2 - vector4;
						vector5.y = 0f;
						vector5 = vector5.normalized;
						vector5 = Vector3.Cross(vector5, Vector3.up);
						Vector3 vector6 = vector4 - vector5 * (5f + path.width / 2f);
						if ((runtimePathDataBuilder.GetCell(vector6).bits & 33) == 0 && !this.TooCloseToWall(vector6, 1f, 1f, false, 0f))
						{
							float num7 = UnityEngine.Random.Range(this.min_tree_scale, this.max_tree_scale);
							BSGTreesGrid.TreeInfo ti2 = new BSGTreesGrid.TreeInfo(UnityEngine.Random.Range(0, max), vector6, UnityEngine.Random.Range(0f, 360f), num7, num7);
							grid.AddTree(ti2);
						}
					}
					num += UnityEngine.Random.Range(this.tree_dist_min, this.tree_dist_max);
				}
				while (num < path.path_len);
			}
		}
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00064DFC File Offset: 0x00062FFC
	public void DelTrees(Vector3 ptw, List<int> tree_type, float range, BSGTreesGrid trees, List<float> tree_radiuses)
	{
		Vector3 ptw2 = new Vector3(ptw.x - range, ptw.y - range, ptw.z - range);
		Vector3 ptw3 = new Vector3(ptw.x + range, ptw.y + range, ptw.z + range);
		Vector2Int vector2Int = trees.WorldToGrid(ptw2);
		Vector2Int vector2Int2 = trees.WorldToGrid(ptw3);
		global::Common.GetBiggestTerrain();
		for (int i = vector2Int.x; i <= vector2Int2.x; i++)
		{
			for (int j = vector2Int.y; j <= vector2Int2.y; j++)
			{
				if (j >= 0 && i >= 0 && j < trees.Grid_Size.y && i < trees.Grid_Size.x)
				{
					List<BSGTreesGrid.TreeInfo> grid = trees.GetGrid(j, i);
					if (grid != null)
					{
						List<BSGTreesGrid.TreeInfo> list = new List<BSGTreesGrid.TreeInfo>();
						for (int k = grid.Count - 1; k >= 0; k--)
						{
							BSGTreesGrid.TreeInfo treeInfo = grid[k];
							float num = tree_radiuses[treeInfo.type] * treeInfo.data.sx;
							Vector3 b = new Vector3(treeInfo.data.x, treeInfo.data.y, treeInfo.data.z);
							if ((ptw - b).magnitude <= range + num && (tree_type == null || tree_type.Contains(treeInfo.type)))
							{
								list.Add(treeInfo);
							}
						}
						trees.DelTrees(grid, list);
					}
				}
			}
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00064FAC File Offset: 0x000631AC
	public void LoadWall()
	{
		if (this.wall != null)
		{
			UnityEngine.Object.DestroyImmediate(this.wall.gameObject);
		}
		DT.Field field = this.field.FindChild("wall", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		this.wall = new GameObject("Wall").AddComponent<Wall>();
		this.wall.transform.SetParent(base.transform);
		this.wall.transform.localPosition = Vector3.zero;
		this.wall.transform.localRotation = Quaternion.identity;
		this.wall.Load(field);
		this.wall.FixDoorHeight = true;
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			this.wall.corners[i].AutoRotate = true;
		}
		this.wall.CheckFortificationLinks = true;
		this.wall.Curveness = 0f;
		this.wall.segments_architecture = this.walls_architecture;
		this.wall.towers_architecture = this.towers_architecture;
		this.wall.LoadArchitectureSet();
		this.wall_level = Mathf.Min(this.wall.GetMaxLevel(), this.wall_level);
		this.wall.segments_variant = Mathf.Min(this.wall.GetMaxVariant(), this.wall.segments_variant);
		this.wall.towers_variant = Mathf.Min(this.wall.GetMaxTowersVariant(), this.wall.towers_variant);
		this.wall.level = this.wall_level;
		this.wall.SnapCorners();
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00065160 File Offset: 0x00063360
	private void GetWallCorners()
	{
		this.wall_corners.Clear();
		if (this.wall == null)
		{
			return;
		}
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			WallCorner wallCorner = this.wall.corners[i];
			wallCorner.HideWall = false;
			wallCorner.AutoCurve = false;
			this.wall_corners.Add(wallCorner.transform.position);
		}
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x000651E0 File Offset: 0x000633E0
	public void LoadCitadel()
	{
		if (this.citadel != null)
		{
			UnityEngine.Object.DestroyImmediate(this.citadel.gameObject);
		}
		DT.Field field = this.field.FindChild("citadel", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		this.citadel = PrefabGrid.Load(field, base.transform, "Citadels", "", 0, 0, true, true);
		if (this.citadel != null)
		{
			this.citadel.name = "Citadel";
			this.citadel.transform.localRotation = Quaternion.identity;
			this.citadel.set_level = this.citadel_level;
			this.citadel.cur_variant = 1;
			this.citadel.SetParent(this, false);
			this.citadel.architecture = this.citadel_architecture;
			this.citadel.UpdateInfo(false);
			this.spawned_grids.Add(this.citadel);
			this.clusters[this.citadel] = this.citadel;
		}
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x000652EC File Offset: 0x000634EC
	private void CalcLevel()
	{
		if (BattleMap.battle != null && BattleMap.battle.IsValid() && !BattleMap.battle.battle_map_only)
		{
			Castle c = BattleMap.battle.settlement as Castle;
			this.level = this.CalcLevel(c, out this.logic_level, out this.logic_max_level, out this.wall_level, out this.wall_max_level, out this.citadel_level, out this.citadel_max_level);
			if (this.citadel != null)
			{
				int cur_level = global::Common.map(this.citadel_level, 1, this.citadel_max_level, 1, this.citadel.GetMaxLevel(), false);
				this.citadel.cur_level = cur_level;
			}
			if (this.wall != null)
			{
				int num = global::Common.map(this.wall_level, 1, this.wall_max_level, 1, this.wall.GetMaxLevel(), false);
				this.wall.level = num;
				return;
			}
		}
		else
		{
			if (this.citadel != null)
			{
				this.citadel.cur_level = this.citadel_level;
			}
			if (this.wall != null)
			{
				this.wall.level = this.wall_level;
			}
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00065415 File Offset: 0x00063615
	public int GetLevel()
	{
		return this.level;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0002C53B File Offset: 0x0002A73B
	public int GetMinLevel()
	{
		return 1;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00065420 File Offset: 0x00063620
	public int GetMaxLevel()
	{
		int num = 1;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.houses[i];
			if (!(prefabGrid == null) && prefabGrid.set_level <= 0)
			{
				int maxLevel = prefabGrid.GetMaxLevel();
				if (maxLevel > 0)
				{
					num += maxLevel - 1;
				}
			}
		}
		return num;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00065476 File Offset: 0x00063676
	public void SetLevel()
	{
		this.UpdateHouseLevels(this.level, true);
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00065488 File Offset: 0x00063688
	public void ValidatePrefabGrids()
	{
		for (int i = 0; i < this.spawned_grids.Count; i++)
		{
			PrefabGrid prefabGrid = this.spawned_grids[i];
			PrefabGrid.Info info = prefabGrid.info;
			int cur_variant = prefabGrid.cur_variant;
			int cur_level = prefabGrid.cur_level;
			if (info.GetPrefab(cur_variant, cur_level) == null)
			{
				int num = info.max_variant + 1;
				for (int j = 1; j <= info.max_variant; j++)
				{
					if (j != cur_variant && info.GetPrefab(j, cur_level) == null)
					{
						num = j;
						break;
					}
				}
				if (num > 0)
				{
					prefabGrid.set_variant = UnityEngine.Random.Range(1, num);
					prefabGrid.Refresh(true, true);
				}
				else
				{
					for (int k = 1; k < info.max_level; k++)
					{
						if (k != cur_level)
						{
							num = info.max_variant + 1;
							for (int l = 1; l <= info.max_variant; l++)
							{
								if (l != cur_variant && info.GetPrefab(l, cur_level) == null)
								{
									num = l;
									break;
								}
							}
							if (num > 0)
							{
								prefabGrid.set_variant = UnityEngine.Random.Range(1, num);
								prefabGrid.set_level = k;
								prefabGrid.Refresh(true, true);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x000655BC File Offset: 0x000637BC
	private void SpawnUniqueBuildings()
	{
		if (this.wall_corners == null || this.wall_corners.Count == 0)
		{
			return;
		}
		if (PrefabGrid.Info.GetRegistry() == null)
		{
			return;
		}
		string text = PrefabGrid.ResolveArchitecture("SpecialBuildings", this.houses_architecture);
		if (text != null)
		{
			PrefabGrid.Info info = PrefabGrid.Info.Get("SpecialBuildings", text, false);
			int num = UnityEngine.Random.Range(this.min_unique_pgs, this.max_unique_pgs);
			Point point = base.transform.position;
			if (this.citadel != null)
			{
				point = this.citadel.transform.position;
			}
			for (int i = 0; i < num; i++)
			{
				float pgscale = this.GetPGScale(info);
				List<Point> list = new List<Point>();
				for (int j = 0; j < 360; j += 10)
				{
					Point rotated = Point.UnitUp.GetRotated((float)j);
					Point point2 = point;
					Coord tile = Coord.WorldToGrid(point2, 1f);
					Point point3 = Coord.WorldToLocal(tile, point2, 1f);
					Point point4 = Coord.WorldToLocal(tile, point + rotated * this.WallDistance * 200f, 1f);
					while (BattleMap.Get().GetDistToWall(point2) <= -45f)
					{
						if (!this.IntersectsPG(point2, info.rect_width * pgscale, info.rect_height * pgscale, 0f))
						{
							list.Add(point2);
							break;
						}
						point2 = Coord.GridToWorld(tile, 1f);
						if (!Coord.RayStep(ref tile, ref point3, ref point4))
						{
							break;
						}
					}
				}
				if (list.Count != 0)
				{
					this.SpawnHouse(info, list[UnityEngine.Random.Range(0, list.Count)], this.unique_container, 0f, false, true, "");
				}
			}
			return;
		}
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x0006578C File Offset: 0x0006398C
	private void SpawnCapturePoints()
	{
		if (this.wall_corners != null && this.wall_corners.Count != 0)
		{
			Wall wall = this.wall;
			if (((wall != null) ? wall.corners : null) != null)
			{
				List<PrefabGrid.Info> list = this.capture_point_info;
				if (list.Count == 0)
				{
					return;
				}
				int num = this.control_point_count;
				SettlementBV.<>c__DisplayClass202_0 CS$<>8__locals1;
				CS$<>8__locals1.gate_positions = from c in this.wall.corners
				where c.type == WallCorner.Type.Gate
				select c.transform.position;
				Point point = base.transform.position;
				if (this.citadel != null)
				{
					point = this.citadel.transform.position;
				}
				float num2 = this.WallDistance;
				if (this.wall_corners != null)
				{
					for (int i = 0; i < this.wall_corners.Count; i++)
					{
						float num3 = this.wall_corners[i].Dist(point);
						if (num3 > num2)
						{
							num2 = num3;
						}
					}
				}
				for (int j = 0; j < num; j++)
				{
					PrefabGrid.Info info = list[UnityEngine.Random.Range(0, list.Count)];
					float pgscale = this.GetPGScale(info);
					List<Point> list2 = new List<Point>();
					for (int k = 0; k < 360; k += 10)
					{
						Point rotated = Point.UnitUp.GetRotated((float)k);
						Point point2 = point;
						Coord tile = Coord.WorldToGrid(point2, 1f);
						Point point3 = Coord.WorldToLocal(tile, point2, 1f);
						Point point4 = Coord.WorldToLocal(tile, point + rotated * this.WallDistance * 200f, 1f);
						while (BattleMap.Get().IsInsideWall(point2) && SettlementBV.<SpawnCapturePoints>g__DistanceToGate|202_2(point2, ref CS$<>8__locals1) >= 45f)
						{
							float num4 = float.MaxValue;
							for (int l = 0; l < this.spawned_grids.Count; l++)
							{
								PrefabGrid prefabGrid = this.spawned_grids[l];
								if (prefabGrid.capture_point == PrefabGrid.Info.CapturePoint.Yes)
								{
									float num5 = point2.Dist(prefabGrid.transform.position);
									if (num5 < num4)
									{
										num4 = num5;
									}
								}
							}
							if (num4 > this.min_dist_between_control_point && !this.IntersectsPG(point2, info.rect_width * pgscale, info.rect_height * pgscale, 0f))
							{
								list2.Add(point2);
								break;
							}
							point2 = Coord.GridToWorld(tile, 1f);
							if (!Coord.RayStep(ref tile, ref point3, ref point4))
							{
								break;
							}
						}
					}
					if (list2.Count != 0)
					{
						Point pos = list2[UnityEngine.Random.Range(0, list2.Count)];
						PrefabGrid prefabGrid2 = this.SpawnHouse(info, pos, this.unique_container, 0f, false, false, "");
						global::CapturePoint capturePoint = prefabGrid2.gameObject.GetComponentInChildren<global::CapturePoint>(true);
						if (capturePoint == null)
						{
							capturePoint = prefabGrid2.gameObject.AddComponent<global::CapturePoint>();
						}
						float num6 = Mathf.Sqrt(info.rect_width * pgscale * info.rect_width * pgscale + info.rect_height * pgscale * info.rect_height * pgscale);
						capturePoint.radius = num6 / 2f;
					}
				}
				return;
			}
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00065AF4 File Offset: 0x00063CF4
	private bool TooCloseToWall(Point pos, float width, float height, bool check_inside, float min_value)
	{
		float num = BattleMap.Get().GetDistToWall(pos);
		float num2 = Mathf.Sqrt(width * width + height * height) / 2f;
		if (num < num2 + min_value)
		{
			return true;
		}
		if (check_inside && num < 0f)
		{
			return true;
		}
		for (int i = 0; i < this.wall_corners.Count - 1; i++)
		{
			float2 @float;
			num = this.GetDistanceToLine(this.wall_corners[i], this.wall_corners[i + 1], pos, out @float);
			if (num < num2 + min_value)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00065B94 File Offset: 0x00063D94
	private float GetDistanceToLine(float2 a, float2 b, float2 p, out float2 vec)
	{
		float2 @float = a - b;
		float num = @float.x * @float.x + @float.y * @float.y;
		if ((double)num == 0.0)
		{
			vec = 0;
			return math.distance(p, a);
		}
		float lhs = math.max(0f, math.min(1f, math.dot(p - a, b - a) / num));
		float2 float2 = a + lhs * (b - a);
		vec = float2 - p;
		return math.distance(p, float2);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00065C3A File Offset: 0x00063E3A
	private bool TooCloseToShore(Point pos, float width, float height)
	{
		return this.TooCloseToShore(pos, width, height, this.min_army_camp_dist_to_coast);
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00065C4C File Offset: 0x00063E4C
	private bool TooCloseToShore(Point pos, float width, float height, float min_dist)
	{
		float num = Mathf.Sqrt(width * width + height * height) / 2f;
		float distToShore = BattleMap.Get().GetDistToShore(pos);
		if (distToShore < 0f)
		{
			return true;
		}
		float num2 = Math.Abs(distToShore);
		return num2 < num + min_dist && num2 < BattleMap.Get().wall_sdf_max_circle;
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00065CA5 File Offset: 0x00063EA5
	private bool TooCloseToOtherGrids(Point pos, float width, float height, float min_dist_to_city)
	{
		return this.TooCloseToOtherGrids(pos, width, height, min_dist_to_city, this.spawned_grids);
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x00065CB8 File Offset: 0x00063EB8
	private bool TooCloseToOtherGrids(Point pos, float width, float height, float min_dist_to_city, List<PrefabGrid> spawned_grids)
	{
		float num = Mathf.Sqrt(width * width + height * height) / 2f;
		float num2 = float.MaxValue;
		for (int i = 0; i < spawned_grids.Count; i++)
		{
			float num3 = pos.Dist(spawned_grids[i].transform.position);
			if (num3 < num2)
			{
				num2 = num3;
			}
		}
		return num2 != float.MaxValue && num2 < num + min_dist_to_city;
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x00065D2C File Offset: 0x00063F2C
	private bool TooCloseToBreakSiegePos(Point pos)
	{
		if (this.citadel == null)
		{
			return false;
		}
		Point point = new Point(this.break_siege_position_x, this.break_siege_position_y);
		float num = point.SqrDist(pos);
		float num2 = this.min_between_camps_dist;
		float num3 = num2 * num2;
		return num < num3;
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00065D70 File Offset: 0x00063F70
	private bool TooCloseToEnemyCamp(Point pos, List<PrefabGrid> enemyCamps, float width, float height, float custom_test_dist = -1f)
	{
		if (enemyCamps == null || enemyCamps.Count == 0)
		{
			return false;
		}
		float num = Mathf.Sqrt(width * width + height * height) / 2f;
		float num2 = float.MaxValue;
		for (int i = 0; i < enemyCamps.Count; i++)
		{
			float num3 = pos.Dist(enemyCamps[i].transform.position);
			if (num3 < num2)
			{
				num2 = num3;
			}
		}
		if (num2 == 3.4028235E+38f)
		{
			return false;
		}
		if (custom_test_dist == -1f)
		{
			if (num2 < this.min_between_camps_dist - 2f * num)
			{
				return true;
			}
		}
		else if (num2 < custom_test_dist - 2f * num)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00065E10 File Offset: 0x00064010
	private void SpawnArmyCampOutsideWall()
	{
		PrefabGrid.Info info = this.army_camp_info;
		float pgscale = this.GetPGScale(info);
		float num = pgscale * info.hex_size;
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		Point point = new Point(x / 2f, z / 2f);
		List<Point> list = new List<Point>();
		float num2 = pgscale * info.rect_width;
		float num3 = pgscale * info.rect_height;
		int count;
		if (BattleMap.battle == null || BattleMap.battle.battle_map_only)
		{
			count = this.max_army_camp_squad_count;
		}
		else
		{
			count = BattleMap.battle.squads.Get(0).Count;
		}
		int camp_count = global::Common.map(count, this.min_army_camp_squad_count, this.max_army_camp_squad_count, this.min_army_camps, this.max_army_camps, true);
		float num4 = this.min_camp_dist_to_city;
		while (num4 > 0f && list.Count == 0)
		{
			for (float num5 = 0f; num5 <= this.outside_camp_additional_range; num5 += num)
			{
				for (int i = 0; i < this.outside_camp_offsets.Count; i++)
				{
					Point point2 = default(Point);
					Point point3 = default(Point);
					Point point4 = new Point(this.t.terrainData.size.x / 2f + this.outside_camp_offsets[i].x, this.t.terrainData.size.z / 2f + this.outside_camp_offsets[i].z);
					Point point5 = new Point(point4.x - this.outside_camp_max_width - num5, point4.y - this.outside_camp_max_height - num5);
					Point point6 = new Point(point4.x + this.outside_camp_max_width + num5, point4.y + this.outside_camp_max_height + num5);
					if (num5 > 0f)
					{
						point2 = new Point(point5.x + num5, point5.y + num5);
						point3 = new Point(point6.x - num5, point6.y - num5);
					}
					for (float num6 = point5.x; num6 < point6.x; num6 += num)
					{
						for (float num7 = point5.y; num7 < point6.y; num7 += num)
						{
							if (num5 <= 0f || num7 < point2.y || num7 > point3.y || num6 < point2.x || num6 > point3.x)
							{
								Point point7 = new Point(num6, num7);
								if ((!(this.wall != null) || !this.wall.gameObject.activeInHierarchy || !this.TooCloseToWall(point7, num2, pgscale * num3, true, num4)) && !this.TooCloseToShore(point7, num2, pgscale * num3) && !this.TooCloseToOtherGrids(point7, num2, pgscale * num3, num4))
								{
									float y = Quaternion.LookRotation(point - point7).eulerAngles.y;
									if (!this.IsTerrainInvalidForPrefabGrid(point7, num2, num3, y) && !this.IntersectsPG(point7, num2, num3, y) && !list.Contains(point7))
									{
										list.Add(point7);
									}
								}
							}
						}
					}
					if (i != 0 && list.Count > 0)
					{
						break;
					}
				}
				if (list.Count > 0)
				{
					break;
				}
			}
			num4 -= this.camp_dist_to_city_decrease_step;
		}
		if (list.Count == 0)
		{
			return;
		}
		this.SpawnArmyCamps(ref this.south_camps, camp_count, point, list, num2, num3, info);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x000661C0 File Offset: 0x000643C0
	private List<Point> GetReinfPoints(bool check_break_siege_pos = true, bool ignore_north_camps = false, bool ignore_south_camps = false)
	{
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		List<Point> list = new List<Point>();
		float num = 20f;
		float break_siege_distance_to_camps = this.min_between_camps_dist;
		float num2 = (float)this.pdb.lpf.settings.map_bounds_width + 2f * num;
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			break_siege_distance_to_camps = BattleMap.battle.simulation.def.break_siege_distance_to_camps;
		}
		for (float num3 = 0f; num3 < x; num3 += num)
		{
			for (float num4 = 0f; num4 < z; num4 += num)
			{
				Point point = new Point(num3, num4);
				if ((!(this.wall != null) || !this.wall.gameObject.activeInHierarchy || !this.TooCloseToWall(point, num, num, true, num)) && (!check_break_siege_pos || !this.TooCloseToBreakSiegePos(point)) && !this.TooCloseToShore(point, num, num, 0f) && !this.TooCloseToOtherGrids(point, num, num, num) && (ignore_north_camps || !this.TooCloseToEnemyCamp(point, this.north_camps, num, num, break_siege_distance_to_camps)) && (ignore_south_camps || !this.TooCloseToEnemyCamp(point, this.south_camps, num, num, break_siege_distance_to_camps)) && this.IsTerrainValidForSquad(point, num, num, 0f) && (this.TooCloseToShore(point, num, num, num) || num3 < num2 || num3 >= x - num2 || num4 < num2 || num4 >= z - num2) && !list.Contains(point))
				{
					list.Add(point);
				}
			}
		}
		return list;
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0006636C File Offset: 0x0006456C
	private SettlementBV.ReinfPoint[] CalcReinforcementPositions(Point camp_pos, Point enemy_camp_pos, bool can_be_near_break_siege_pos = true, bool ignore_north_camps = false, bool ignore_south_camps = false)
	{
		List<Point> reinfPoints = this.GetReinfPoints(!can_be_near_break_siege_pos, ignore_north_camps, ignore_south_camps);
		SettlementBV.ReinfPoint[] array = new SettlementBV.ReinfPoint[reinfPoints.Count];
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		float num = x * x + z * z;
		for (int i = 0; i < reinfPoints.Count; i++)
		{
			Point pos = reinfPoints[i];
			array[i] = new SettlementBV.ReinfPoint
			{
				pos = pos,
				dist_to_enemy_camp = num - pos.SqrDist(enemy_camp_pos),
				dist_to_my_camp = num - pos.SqrDist(camp_pos)
			};
		}
		return array;
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00066424 File Offset: 0x00064624
	private void CalcSouthReinforcementPositions()
	{
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		Point point = new Point(x / 2f, z / 2f);
		Point camp_pos;
		if (this.south_camps != null && this.south_camps.Count > 0)
		{
			camp_pos = this.south_camps[0].transform.position;
		}
		else
		{
			camp_pos = point;
		}
		Point enemy_camp_pos;
		if (this.north_camps != null && this.north_camps.Count > 0)
		{
			enemy_camp_pos = this.north_camps[0].transform.position;
		}
		else
		{
			enemy_camp_pos = point;
		}
		this.south_reinforcement_points = this.CalcReinforcementPositions(camp_pos, enemy_camp_pos, false, false, true);
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x000664EC File Offset: 0x000646EC
	private void CalcNorthReinforcementPositions()
	{
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		Point point = new Point(x / 2f, z / 2f);
		Point camp_pos;
		if (this.north_camps != null && this.north_camps.Count > 0)
		{
			camp_pos = this.north_camps[0].transform.position;
		}
		else
		{
			camp_pos = point;
		}
		Point enemy_camp_pos;
		if (this.south_camps != null && this.south_camps.Count > 0)
		{
			enemy_camp_pos = this.south_camps[0].transform.position;
		}
		else
		{
			enemy_camp_pos = point;
		}
		this.north_reinforcement_points = this.CalcReinforcementPositions(camp_pos, enemy_camp_pos, true, true, false);
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x000665B4 File Offset: 0x000647B4
	private void CalcNorthBreakSiegePositions()
	{
		if (this.citadel == null)
		{
			return;
		}
		if (this.south_camps == null)
		{
			return;
		}
		float num = 15f;
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x2 = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		Point point = new Point(x2 / 2f, z / 2f);
		Point point2 = this.south_camps[0].gameObject.transform.position;
		Point point3 = point2 - point;
		float num2 = (point3.x < 0f) ? num : (-num);
		float num3 = (point3.y < 0f) ? num : (-num);
		List<Point> list = new List<Point>();
		float break_siege_distance_to_camps = this.max_between_camps_dist;
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			break_siege_distance_to_camps = BattleMap.battle.simulation.def.break_siege_distance_to_camps;
		}
		float num4 = ((num2 > 0f) ? break_siege_distance_to_camps : (-break_siege_distance_to_camps)) + point2.x;
		float num5 = (num3 > 0f) ? break_siege_distance_to_camps : (-break_siege_distance_to_camps);
		num5 += point2.y;
		float num6 = num4;
		while (num6 < x2 - 150f && num6 > 150f)
		{
			for (float num7 = point2.y - 2f * num; num7 <= point2.y + 2f * num; num7 += num * 0.5f)
			{
				Point point4 = new Point(num6, num7);
				if (this.IsWithinBounds(point4, 1f) && (!(this.wall != null) || !this.wall.gameObject.activeInHierarchy || !this.TooCloseToWall(point4, num, num, true, 0f)) && !this.TooCloseToShore(point4, num, num, num) && !this.TooCloseToOtherGrids(point4, num, num, num) && !this.TooCloseToEnemyCamp(point4, this.south_camps, num, num, break_siege_distance_to_camps))
				{
					float y2 = Quaternion.LookRotation(point2 - point4).eulerAngles.y;
					if (!this.IntersectsPG(point4, num, num, y2) && this.IsTerrainValidForSquad(point4, num, num, 0f) && !list.Contains(point4))
					{
						list.Add(point4);
					}
				}
			}
			num6 += num2;
		}
		float num8 = num5;
		while (num8 < z - 150f && num8 > 150f)
		{
			for (float num9 = point2.x - 2f * num; num9 <= point2.x + 2f * num; num9 += num * 0.5f)
			{
				Point point5 = new Point(num9, num8);
				if (this.IsWithinBounds(point5, 1f) && (!(this.wall != null) || !this.wall.gameObject.activeInHierarchy || !this.TooCloseToWall(point5, num, num, true, 0f)) && !this.TooCloseToShore(point5, num, num) && !this.TooCloseToOtherGrids(point5, num, num, num) && !this.TooCloseToEnemyCamp(point5, this.south_camps, num, num, break_siege_distance_to_camps))
				{
					float y2 = Quaternion.LookRotation(point2 - point5).eulerAngles.y;
					if (!this.IntersectsPG(point5, num, num, y2) && this.IsTerrainValidForSquad(point5, num, num, 0f) && !list.Contains(point5))
					{
						list.Add(point5);
					}
				}
			}
			num8 += num3;
		}
		if (list.Count < 1)
		{
			Point camp_pos;
			if (this.north_camps != null && this.north_camps.Count > 0)
			{
				camp_pos = this.north_camps[0].transform.position;
			}
			else
			{
				camp_pos = point;
			}
			Point enemy_camp_pos;
			if (this.south_camps != null && this.south_camps.Count > 0)
			{
				enemy_camp_pos = this.south_camps[0].transform.position;
			}
			else
			{
				enemy_camp_pos = point;
			}
			SettlementBV.ReinfPoint[] array = this.CalcReinforcementPositions(camp_pos, enemy_camp_pos, true, false, false);
			WeightedRandom<Point> weightedRandom = new WeightedRandom<Point>(array.Length, -1);
			foreach (SettlementBV.ReinfPoint reinfPoint in array)
			{
				Point pos = reinfPoint.pos;
				float weight = reinfPoint.dist_to_enemy_camp + reinfPoint.dist_to_my_camp;
				weightedRandom.AddOption(pos, weight);
			}
			if (weightedRandom.options.Count >= 2)
			{
				weightedRandom.options.Sort((WeightedRandom<Point>.Option x, WeightedRandom<Point>.Option y) => y.weight.CompareTo(x.weight));
				int count = weightedRandom.options.Count;
				for (int j = count - 1; j >= count / 2; j--)
				{
					weightedRandom.DelOptionAtIndex(j);
				}
			}
			Point item = weightedRandom.Choose(default(Point), false);
			list.Add(item);
		}
		if (list.Count == 0)
		{
			this.break_siege_position_x = point.x;
			this.break_siege_position_y = point.y;
			return;
		}
		Point point6 = list[UnityEngine.Random.Range(0, list.Count)];
		this.break_siege_position_x = point6.x;
		this.break_siege_position_y = point6.y;
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00066AF4 File Offset: 0x00064CF4
	private void SpawnArmyCamps(ref List<PrefabGrid> camps, int camp_count, Point center, List<Point> points, float scaled_width, float scaled_height, PrefabGrid.Info info, Point direction)
	{
		Point point = points[UnityEngine.Random.Range(0, points.Count)];
		this.rot = Quaternion.LookRotation(direction).eulerAngles.y;
		PrefabGrid item = this.SpawnHouse(info, point, this.unique_container, this.rot, false, false, "");
		camps.Add(item);
		float num = Mathf.Sqrt(scaled_width * scaled_width + scaled_height * scaled_height) / 2f;
		for (int i = 1; i < camp_count; i++)
		{
			float num2 = this.rot + (float)(90 * (1 + (i - 1) * 2));
			bool flag = i % 2 == 0;
			for (float num3 = 0f; num3 < 360f; num3 += 10f)
			{
				float num4 = num2;
				num4 += num3 * (float)(flag ? -1 : 1);
				if (num4 > 360f)
				{
					num4 -= 360f;
				}
				bool flag2 = false;
				Point point2 = point;
				for (int j = 0; j < 3; j++)
				{
					Point pt = PPos.UnitUp.GetRotated(num4) * (num * 2f + UnityEngine.Random.Range(10f, (float)(50 - j * 20)));
					point2 = point + pt;
					if (!this.pdb.lpf.data.OutOfMapBounds(point2) && BattleMap.Get().IsInsideWall(point) == BattleMap.Get().IsInsideWall(point2) && !this.IsTerrainInvalidForPrefabGrid(point2, scaled_width, scaled_height, this.rot) && !this.IntersectsPG(point2, scaled_width, scaled_height, this.rot))
					{
						List<PrefabGrid> enemyCamps = (camps != this.south_camps) ? this.south_camps : this.north_camps;
						if (!this.TooCloseToEnemyCamp(point2, enemyCamps, scaled_width, scaled_height, -1f) && !this.TooCloseToShore(point2, scaled_width, scaled_height, 0f))
						{
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					PrefabGrid item2 = this.SpawnHouse(info, point2, this.unique_container, this.rot, false, false, "");
					camps.Add(item2);
					break;
				}
			}
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00066D28 File Offset: 0x00064F28
	private void SpawnArmyCamps(ref List<PrefabGrid> camps, int camp_count, Point center, List<Point> points, float scaled_width, float scaled_height, PrefabGrid.Info info)
	{
		Point point = points[UnityEngine.Random.Range(0, points.Count)];
		Point direction = center - point;
		points = new List<Point>
		{
			point
		};
		this.SpawnArmyCamps(ref camps, camp_count, center, points, scaled_width, scaled_height, info, direction);
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00066D74 File Offset: 0x00064F74
	private void CalcOpenFieldCampsPositions()
	{
		PrefabGrid.Info info = this.army_camp_info;
		float pgscale = this.GetPGScale(info);
		float hex_size = info.hex_size;
		Terrain biggestTerrain = global::Common.GetBiggestTerrain();
		float x = biggestTerrain.terrainData.size.x;
		float z = biggestTerrain.terrainData.size.z;
		Point point = new Point(x / 2f, z / 2f);
		float num = pgscale * info.rect_width;
		float num2 = pgscale * info.rect_height;
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			this.min_between_camps_dist = BattleMap.battle.simulation.def.min_distance_between_camps;
			this.max_between_camps_dist = BattleMap.battle.simulation.def.max_distance_between_camps;
		}
		float num3 = this.min_between_camps_dist + 2f * this.min_army_camp_dist_to_coast;
		float d = 1.4142f * biggestTerrain.terrainData.size.x;
		float num4 = 22.5f;
		float num5 = 50f;
		float num6 = 300f;
		float num7 = 50f;
		float tile_size = 1f;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = true;
		List<Point> list = new List<Point>();
		List<Point> list2 = new List<Point>();
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		while (num10 <= 90f && (!flag3 || !flag5 || num10 != num4))
		{
			float num11 = 0f;
			while (num11 <= num6 && (!flag3 || num11 <= num7))
			{
				float num12 = 0f;
				while (num12 <= num6 && (!flag3 || num12 <= num7))
				{
					int i = 0;
					while (i < 2)
					{
						if (i != 1 || num11 != 0f)
						{
							int j = 0;
							while (j < 2)
							{
								if (j != 1 || num12 != 0f)
								{
									int k = 0;
									while (k < 2)
									{
										if (k != 1 || (num10 != 0f && num10 != -90f))
										{
											Vector2 vector = Quaternion.Euler(0f, 0f, num10) * new Vector2(0f, -1f);
											Point point2 = point + new Point(num12, num11);
											Point point3 = point2;
											Vector2 v = point2 + vector * d;
											Coord tile = Coord.WorldToGrid(point3, tile_size);
											Point point4 = Coord.WorldToLocal(tile, point3, tile_size);
											Point point5 = Coord.WorldToLocal(tile, v, tile_size);
											while (!this.pdb.lpf.data.OutOfMapBounds(point3))
											{
												if (this.pdb.lpf.data.GetNode(point3).water)
												{
													flag5 = false;
													break;
												}
												point3 = Coord.GridToWorld(tile, tile_size);
												if (!Coord.RayStep(ref tile, ref point4, ref point5))
												{
													break;
												}
											}
											Point point6 = point3;
											point3 = point2;
											vector = -vector;
											v = point2 + vector * d;
											tile = Coord.WorldToGrid(point3, tile_size);
											point4 = Coord.WorldToLocal(tile, point3, tile_size);
											point5 = Coord.WorldToLocal(tile, v, tile_size);
											while (!this.pdb.lpf.data.OutOfMapBounds(point3))
											{
												if (this.pdb.lpf.data.GetNode(point3).water)
												{
													flag5 = false;
													break;
												}
												point3 = Coord.GridToWorld(tile, tile_size);
												if (!Coord.RayStep(ref tile, ref point4, ref point5))
												{
													break;
												}
											}
											Point point7 = point3;
											float num13 = point6.Dist(point7);
											if ((!flag3 && !flag && !flag4) || num13 >= num3)
											{
												if (num13 < num3)
												{
													float f = Mathf.Sqrt(num * num + num2 * num2) / 2f;
													this.camp_position_south = point6 + new Point(vector.x, vector.y) * f;
													bool flag6 = this.CheckCampPoint(this.camp_position_south, point, pgscale, num, num2);
													if (flag6 || (!flag2 && !flag))
													{
														this.camp_position_north = point7 - new Point(vector.x, vector.y) * f;
														bool flag7 = this.CheckCampPoint(this.camp_position_north, point, pgscale, num, num2);
														if (flag7 || (!flag2 && !flag))
														{
															if (flag6 && flag7 && !flag2)
															{
																flag2 = true;
																num9 = 0f;
															}
															if (num13 > num9)
															{
																num9 = num13;
																list.Clear();
																list2.Clear();
																list.Add(this.camp_position_south);
																list2.Add(this.camp_position_north);
															}
														}
													}
												}
												else
												{
													float num14 = point2.Dist(point6);
													float num15 = point2.Dist(point7);
													float num16 = point6.Dist(point7) - 2f * this.min_army_camp_dist_to_coast;
													float max = (num16 > this.max_between_camps_dist) ? this.max_between_camps_dist : num16;
													float num17 = UnityEngine.Random.Range(this.min_between_camps_dist, max);
													if (num14 < num15)
													{
														if (num14 >= num17 * 0.5f)
														{
															this.camp_position_south = point2 - new Point(vector.x, vector.y) * (num17 * 0.5f);
															bool flag8 = this.CheckCampPoint(this.camp_position_south, point, pgscale, num, num2);
															if (!flag8 && flag2)
															{
																goto IL_903;
															}
															this.camp_position_north = point2 + new Point(vector.x, vector.y) * num17 * 0.5f;
															bool flag9 = this.CheckCampPoint(this.camp_position_north, point, pgscale, num, num2);
															if (!flag9 && flag2)
															{
																goto IL_903;
															}
															if (flag8 && flag9)
															{
																if (num12 < num7 && num11 < num7)
																{
																	if (!flag3)
																	{
																		flag3 = true;
																		num8 = 0f;
																	}
																}
																else
																{
																	flag4 = true;
																}
																flag2 = true;
															}
															else
															{
																flag = true;
															}
														}
														else
														{
															this.camp_position_south = point2 - new Point(vector.x, vector.y) * (num14 - this.min_army_camp_dist_to_coast);
															bool flag10 = this.CheckCampPoint(this.camp_position_south, point, pgscale, num, num2);
															if (!flag10 && flag2)
															{
																goto IL_903;
															}
															this.camp_position_north = this.camp_position_south + new Point(vector.x, vector.y) * num17;
															bool flag11 = this.CheckCampPoint(this.camp_position_north, point, pgscale, num, num2);
															if (!flag11 && flag2)
															{
																goto IL_903;
															}
															if (flag10 && flag11)
															{
																if (num12 < num7 && num11 < num7)
																{
																	if (!flag3)
																	{
																		flag3 = true;
																		num8 = 0f;
																	}
																}
																else
																{
																	flag4 = true;
																}
																flag2 = true;
															}
															else
															{
																flag = true;
															}
														}
													}
													else if (num15 >= num17 * 0.5f)
													{
														this.camp_position_south = point2 - new Point(vector.x, vector.y) * (num17 * 0.5f);
														bool flag12 = this.CheckCampPoint(this.camp_position_south, point, pgscale, num, num2);
														if (!flag12 && flag2)
														{
															goto IL_903;
														}
														this.camp_position_north = point2 + new Point(vector.x, vector.y) * num17 * 0.5f;
														bool flag13 = this.CheckCampPoint(this.camp_position_north, point, pgscale, num, num2);
														if (!flag13 && flag2)
														{
															goto IL_903;
														}
														if (flag12 && flag13)
														{
															if (num12 < num7 && num11 < num7)
															{
																if (!flag3)
																{
																	flag3 = true;
																	num8 = 0f;
																}
															}
															else
															{
																flag4 = true;
															}
															flag2 = true;
														}
														else
														{
															flag = true;
														}
													}
													else
													{
														this.camp_position_north = point2 + new Point(vector.x, vector.y) * (num15 - this.min_army_camp_dist_to_coast);
														bool flag14 = this.CheckCampPoint(this.camp_position_north, point, pgscale, num, num2);
														if (!flag14 && flag2)
														{
															goto IL_903;
														}
														this.camp_position_south = this.camp_position_north - new Point(vector.x, vector.y) * num17;
														bool flag15 = this.CheckCampPoint(this.camp_position_south, point, pgscale, num, num2);
														if (!flag15 && flag2)
														{
															goto IL_903;
														}
														if (flag14 && flag15)
														{
															if (num12 < num7 && num11 < num7)
															{
																if (!flag3)
																{
																	flag3 = true;
																	num8 = 0f;
																}
															}
															else
															{
																flag4 = true;
															}
															flag2 = true;
														}
														else
														{
															flag = true;
														}
													}
													if (flag3)
													{
														if (num16 >= num8)
														{
															if (num16 > num8)
															{
																list.Clear();
																list2.Clear();
																num8 = num16;
															}
															list.Add(this.camp_position_south);
															list2.Add(this.camp_position_north);
														}
													}
													else if (flag4)
													{
														if (num16 >= num8)
														{
															if (num16 > num8)
															{
																list.Clear();
																list2.Clear();
																num8 = num16;
															}
															list.Add(this.camp_position_south);
															list2.Add(this.camp_position_north);
														}
													}
													else if (num13 > num9)
													{
														num9 = num13;
														list.Clear();
														list2.Clear();
														list.Add(this.camp_position_south);
														list2.Add(this.camp_position_north);
													}
												}
											}
										}
										IL_903:
										k++;
										num10 = -num10;
									}
								}
								j++;
								num12 = -num12;
							}
						}
						i++;
						num11 = -num11;
					}
					num12 += num5;
				}
				num11 += num5;
			}
			num10 += num4;
		}
		if (!flag3)
		{
			if (!flag2)
			{
				if (!flag)
				{
					Debug.LogWarning("Correct position for camps not found, using not checked position with smaller distance");
				}
				else
				{
					Debug.LogWarning("Correct position for camps not found, using not checked position");
				}
			}
			else
			{
				Debug.LogWarning("Correct position for camps not found, using checked position with smaller distance");
			}
		}
		if (list.Count == 0 || list2.Count == 0)
		{
			Debug.LogError("Correct position for camps not found");
			return;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		this.camp_position_south = list[index];
		this.camp_position_north = list2[index];
		bool flag16 = false;
		if (BattleMap.battle == null || BattleMap.battle.battle_map_only)
		{
			flag16 = true;
		}
		else
		{
			for (int l = 0; l < BattleMap.battle.attackers.Count; l++)
			{
				if (BattleMap.battle.attackers[l].GetKingdom().is_player)
				{
					flag16 = true;
					break;
				}
			}
		}
		int v2;
		if (BattleMap.battle == null || BattleMap.battle.battle_map_only)
		{
			v2 = this.max_army_camp_squad_count;
		}
		else
		{
			v2 = (flag16 ? BattleMap.battle.squads.Get(0).Count : BattleMap.battle.squads.Get(1).Count);
		}
		int camp_count = global::Common.map(v2, this.min_army_camp_squad_count, this.max_army_camp_squad_count, this.min_army_camps, this.max_army_camps, true);
		this.SpawnArmyCamps(ref this.south_camps, camp_count, point, new List<Point>
		{
			this.camp_position_south
		}, num, num2, info, this.camp_position_north - this.camp_position_south);
		if (BattleMap.battle == null || BattleMap.battle.battle_map_only)
		{
			v2 = this.max_army_camp_squad_count;
		}
		else
		{
			v2 = (flag16 ? BattleMap.battle.squads.Get(1).Count : BattleMap.battle.squads.Get(0).Count);
		}
		camp_count = global::Common.map(v2, this.min_army_camp_squad_count, this.max_army_camp_squad_count, this.min_army_camps, this.max_army_camps, true);
		this.SpawnArmyCamps(ref this.north_camps, camp_count, point, new List<Point>
		{
			this.camp_position_north
		}, num, num2, info, this.camp_position_south - this.camp_position_north);
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00067914 File Offset: 0x00065B14
	private bool CheckCampPoint(Point pos, Point center, float scale, float scaled_width, float scaled_height)
	{
		if (this.wall != null && this.wall.gameObject.activeInHierarchy && this.TooCloseToWall(pos, scaled_width, scale * scaled_height, true, this.min_camp_dist_to_city))
		{
			return false;
		}
		if (this.TooCloseToOtherGrids(pos, scaled_width, scale * scaled_height, this.min_camp_dist_to_city))
		{
			return false;
		}
		float y = Quaternion.LookRotation(center - pos).eulerAngles.y;
		return !this.IsTerrainInvalidForPrefabGrid(pos, scaled_width, scaled_height, y) && !this.IntersectsPG(pos, scaled_width, scaled_height, y);
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x000679B0 File Offset: 0x00065BB0
	private Point[] PickRandomTriangle(List<SettlementBV.Triangle> _triangles, float _areaSum)
	{
		float num = UnityEngine.Random.Range(0f, _areaSum);
		for (int i = 0; i < _triangles.Count; i++)
		{
			if (num < _triangles[i].area)
			{
				return _triangles[i].points;
			}
			num -= _triangles[i].area;
		}
		return _triangles[_triangles.Count - 1].points;
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00067A18 File Offset: 0x00065C18
	private static Vector2 RandomWithinTriangle(Point[] t)
	{
		float num = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
		float num2 = UnityEngine.Random.Range(0f, 1f);
		float f = 1f - num;
		float f2 = num * (1f - num2);
		float f3 = num2 * num;
		Point pt = t[0];
		Point pt2 = t[1];
		Point pt3 = t[2];
		return f * pt + f2 * pt2 + f3 * pt3;
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00067AA0 File Offset: 0x00065CA0
	public float AreaOfTriangle(Point pt1, Point pt2, Point pt3)
	{
		float num = pt1.Dist(pt2);
		float num2 = pt2.Dist(pt3);
		float num3 = pt3.Dist(pt1);
		float num4 = (num + num2 + num3) / 2f;
		return Mathf.Sqrt(num4 * (num4 - num) * (num4 - num2) * (num4 - num3));
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00067AE8 File Offset: 0x00065CE8
	private void GenerateRoads()
	{
		SettlementBV.<>c__DisplayClass225_0 CS$<>8__locals1 = new SettlementBV.<>c__DisplayClass225_0();
		CS$<>8__locals1.<>4__this = this;
		BattleMap battleMap = BattleMap.Get();
		RuntimePathDataBuilder runtimePathDataBuilder = (battleMap != null) ? battleMap.pdb : null;
		CS$<>8__locals1.lpf = runtimePathDataBuilder.lpf;
		if (CS$<>8__locals1.lpf == null)
		{
			return;
		}
		if (this.roads_obj == null)
		{
			this.roads_obj = new GameObject("Roads").transform;
			this.roads_obj.transform.SetParent(base.transform);
		}
		else
		{
			global::Common.DestroyChildren(this.roads_obj.gameObject);
		}
		Wall wall = this.wall;
		if (((wall != null) ? wall.corners : null) == null || this.wall.corners.Count == 0)
		{
			return;
		}
		if (this.citadel == null)
		{
			return;
		}
		if (this.roadPrefab == null)
		{
			return;
		}
		runtimePathDataBuilder.LoadPathfinding();
		this.citadel.transform.position;
		CS$<>8__locals1.pathfinder = this.CreateRoadPathfinder(20f, 0.05f);
		CS$<>8__locals1.special_buildings_sdf = new CombinedSDF2DObject();
		CS$<>8__locals1.capture_points_sdf = new SmoothCombinedSDF2DObject();
		for (int i = 0; i < this.spawned_grids.Count; i++)
		{
			PrefabGrid prefabGrid = this.spawned_grids[i];
			if (prefabGrid.cur_type == "SpecialBuildings")
			{
				Vector3[] rectCornersInWorldSpace = prefabGrid.GetRectCornersInWorldSpace();
				CircleSDF2D item = new CircleSDF2D(prefabGrid.transform.position.xz, math.distance(rectCornersInWorldSpace[0], rectCornersInWorldSpace[2]) * 0.5f);
				CS$<>8__locals1.special_buildings_sdf.sdf2DObjects.Add(item);
			}
			else if (prefabGrid.cur_type == "CapturePoints")
			{
				Vector3[] rectCornersInWorldSpace2 = prefabGrid.GetRectCornersInWorldSpace();
				CircleSDF2D item2 = new CircleSDF2D(prefabGrid.transform.position.xz, math.distance(rectCornersInWorldSpace2[0], rectCornersInWorldSpace2[2]) * 0.25f);
				CS$<>8__locals1.capture_points_sdf.sdf2DObjects.Add(item2);
				CS$<>8__locals1.special_buildings_sdf.sdf2DObjects.Add(item2);
			}
		}
		CS$<>8__locals1.wall_sdf = new PolygonSDF2D((from c in this.wall.corners
		select c.transform.position.xz).ToArray<float2>());
		List<float3> list = new List<float3>();
		List<float3> list2 = new List<float3>();
		for (int j = 0; j < this.wall.corners.Count; j++)
		{
			float3 @float = this.wall.corners[j].transform.position;
			float3 float2 = @float;
			Func<float2, float> sdf;
			if ((sdf = CS$<>8__locals1.<>9__12) == null)
			{
				sdf = (CS$<>8__locals1.<>9__12 = ((float2 p) => CS$<>8__locals1.wall_sdf.SDF2D(p) + CS$<>8__locals1.<>4__this.inner_ring_min_dist_to_walls));
			}
			float2.xz = SDFUtils.SnapToSDFByMarching(sdf, @float.xz, 20, 0.5f);
			float2 = global::Common.SnapToTerrain(float2, 0f, null, -1f, false);
			float3 float3 = math.lerp(float2, this.citadel.transform.position, 0.45f);
			float3 = global::Common.SnapToTerrain(float3, 0f, null, -1f, false);
			list.Add(float2);
			list2.Add(float3);
		}
		List<SettlementBV.RoadBV> list3 = new List<SettlementBV.RoadBV>();
		for (int k = 0; k < this.wall.corners.Count; k++)
		{
			if (this.wall.corners[k].type == WallCorner.Type.Gate)
			{
				WallCorner wallCorner = this.wall.corners[k];
				List<SettlementBV.RoadBV> list4 = CS$<>8__locals1.<GenerateRoads>g__CreateGateRoad|11(this.citadel.transform.position, wallCorner.transform.position, -wallCorner.transform.forward, CS$<>8__locals1.wall_sdf, CS$<>8__locals1.pathfinder);
				if (list4 != null)
				{
					list3.AddRange(list4);
				}
			}
		}
		CombinedSDF2DObject combinedSDF2DObject = new CombinedSDF2DObject();
		combinedSDF2DObject.sdf2DObjects.AddRange(from r in list3
		select new PolygonalChainSDF2D(r.path.path_points.ToArray()));
		PathUtils.MakePathDense(list, this.road_segment_size);
		PathUtils.SmoothLoopedPath((float3 p) => 1f, list, 3, 1);
		for (int l = 0; l < list.Count; l++)
		{
			list[l] = DirtRoadGraph.PointDistortion(list[l], this.outer_ring_road_noise_strength, this.outer_ring_road_noise_scale);
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m] = global::Common.SnapToTerrain(list[m], 0f, null, -1f, false);
		}
		list.Add(list[0]);
		list.Add(list[1]);
		CS$<>8__locals1.parts_count = list.Count<float3>() / this.road_max_segment_count_in_road_part + 1;
		List<SettlementBV.RoadBV> list5 = this.CreatePartitionedRoadDecal(list, null, CS$<>8__locals1.parts_count, true, SettlementBV.RoadBV.Type.Highway, this.roadPrefab);
		this.outer_roads.AddRange(list5);
		this.roads.AddRange(list5);
		foreach (SettlementBV.RoadBV road_to_clone in list5)
		{
			this.CloneRoadObject(road_to_clone, this.roadBackgroundPrefab);
		}
		PathUtils.MakePathDense(list2, this.road_segment_size);
		for (int n = 0; n < list2.Count; n++)
		{
			float3 float4 = list2[n];
			float3 float5 = math.normalize((float4 - this.citadel.transform.position) * math.float3(1f, 0f, 1f));
			int num = 0;
			while (num < 10 && CS$<>8__locals1.special_buildings_sdf.SDF2D(float4.xz) - 5f < 0f)
			{
				float4.xz += float5.xz * 3f;
				num++;
			}
			list2[n] = float4;
		}
		list2.RemoveAll((float3 p) => CS$<>8__locals1.special_buildings_sdf.SDF2D(p.xz) - 5f < 0f);
		list2.Add(list2.First<float3>());
		list2 = CS$<>8__locals1.<GenerateRoads>g__BuildPathThroughPoints|8(list2, this.road_segment_size, this.roadPrefab.width * 0.5f);
		PathUtils.SmoothLoopedPath((float3 p) => math.saturate((CS$<>8__locals1.special_buildings_sdf.SDF2D(p.xz) - 15f) / 20f), list2, 7, 2);
		PathUtils.SmoothLoopedPath((float3 p) => 1f, list2, 1, 1);
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			float3 point = list2[num2];
			float distortion_strength = math.saturate((CS$<>8__locals1.special_buildings_sdf.SDF2D(point.xz) - 10f) / 30f) * this.inner_ring_road_noise_strength;
			list2[num2] = DirtRoadGraph.PointDistortion(point, distortion_strength, this.inner_ring_road_noise_scale);
		}
		for (int num3 = 0; num3 < list2.Count; num3++)
		{
			float3 float6 = list2[num3];
			float num4 = 20f;
			float num5 = combinedSDF2DObject.SDF2D(float6.xz);
			if (num5 < num4)
			{
				float3 y = SDFUtils.SnapToSDFByMarching(combinedSDF2DObject.To3DFunc(), float6, 4, 0.5f);
				float6 = math.lerp(float6, y, math.smoothstep(num4, num4 * 0.4f, num5));
				list2[num3] = float6;
			}
		}
		PathUtils.ReduceLoopsInLoopedPath(list2);
		PathUtils.SmoothLoopedPath((float3 p) => 1f, list2, 2, 1);
		list2.Add(list2.First<float3>());
		for (int num6 = 0; num6 < list2.Count; num6++)
		{
			list2[num6] = global::Common.SnapToTerrain(list2[num6], 0f, null, -1f, false);
		}
		PolygonSDF2D polygonSDF2D = new PolygonSDF2D((from p in list2
		select p.xz).ToArray<float2>());
		Vector3[] rectCornersInWorldSpace3 = this.citadel.GetRectCornersInWorldSpace();
		float num7 = math.distance(rectCornersInWorldSpace3[0], rectCornersInWorldSpace3[2]) * 0.4f;
		if (polygonSDF2D.SDF2D(this.citadel.transform.position.xz) < -num7)
		{
			CS$<>8__locals1.parts_count = list2.Count<float3>() / this.road_max_segment_count_in_road_part + 1;
			list5 = this.CreatePartitionedRoadDecal(list2, null, CS$<>8__locals1.parts_count, true, SettlementBV.RoadBV.Type.Highway, this.roadPrefab);
			this.roads.AddRange(list5);
			foreach (SettlementBV.RoadBV road_to_clone2 in list5)
			{
				this.CloneRoadObject(road_to_clone2, this.roadBackgroundPrefab);
			}
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0006845C File Offset: 0x0006665C
	private PathUtils.AStarPathfinder CreateRoadPathfinder(float max_angle = 20f, float margin = 0.05f)
	{
		Bounds bounds = Terrain.activeTerrain.terrainData.bounds;
		float3 x = bounds.min;
		float3 y = bounds.max;
		int num = 32;
		PathUtils.AStarPathfinder astarPathfinder = new PathUtils.AStarPathfinder(bounds.size.x / (float)num, SettlementBV.seed * 321 + 7864738);
		int num2 = 400;
		float num3 = math.cos(math.radians(max_angle));
		Terrain activeTerrain = Terrain.activeTerrain;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				float2 @float = math.float2((float)j, (float)i) / ((float)num2 - 1f);
				@float = math.clamp(@float, margin, 1f - margin);
				float3 float2 = math.normalize(activeTerrain.terrainData.GetInterpolatedNormal(@float.x, @float.y));
				float interpolatedHeight = activeTerrain.terrainData.GetInterpolatedHeight(@float.x, @float.y);
				float3 v = math.lerp(x, y, math.float3(@float, 0f).xzy);
				bool flag = (double)BattleMap.Get().GetDistToWall(v) < (double)astarPathfinder.grid_cell_size - 0.5;
				if (float2.y < num3 || interpolatedHeight < 20f || flag)
				{
					astarPathfinder.AddObstacle(math.lerp(x.xz, y.xz, @float));
				}
			}
		}
		for (int k = 0; k < this.south_camps.Count; k++)
		{
			float2 xz = math.float3(this.south_camps[k].transform.position).xz;
			astarPathfinder.AddObstacle(xz);
			astarPathfinder.AddObstacle(xz + math.float2(0f, -1f) * astarPathfinder.grid_cell_size);
			astarPathfinder.AddObstacle(xz + math.float2(0f, 1f) * astarPathfinder.grid_cell_size);
			astarPathfinder.AddObstacle(xz + math.float2(-1f, 0f) * astarPathfinder.grid_cell_size);
			astarPathfinder.AddObstacle(xz + math.float2(1f, 0f) * astarPathfinder.grid_cell_size);
		}
		return astarPathfinder;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x000686F8 File Offset: 0x000668F8
	private List<SettlementBV.RoadBV> CreatePartitionedRoadDecal(List<float3> path, List<Color> colors, int parts_count, bool spawn_waypoints, SettlementBV.RoadBV.Type type, TerrainPath decal_prefab)
	{
		List<SettlementBV.RoadBV> list = new List<SettlementBV.RoadBV>();
		float num = (float)path.Count / (float)parts_count;
		for (int i = 0; i < parts_count; i++)
		{
			int num2 = (int)(num * (float)i);
			int num3 = math.clamp((int)(num * (float)(i + 1) + 1f), 0, path.Count - 1);
			List<float3> path2 = path.GetRange(num2, num3 - num2 + 1).ToList<float3>();
			List<Color> colors2 = null;
			if (colors != null)
			{
				colors2 = colors.GetRange(num2, num3 - num2 + 1).ToList<Color>();
			}
			SettlementBV.RoadBV roadBV = this.CreateRoadDecal(path2, colors2, spawn_waypoints, type, decal_prefab);
			if (roadBV != null)
			{
				list.Add(roadBV);
			}
		}
		return list;
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00068794 File Offset: 0x00066994
	private SettlementBV.RoadBV CreateRoadDecal(List<float3> path, List<Color> colors, bool spawn_waypoints, SettlementBV.RoadBV.Type type, TerrainPath decal_prefab)
	{
		if (path.Count <= 1)
		{
			return null;
		}
		float num = 0f;
		for (int i = 0; i < path.Count - 1; i++)
		{
			num += math.distance(path[i], path[i + 1]);
		}
		SettlementBV.RoadBV roadBV = new SettlementBV.RoadBV();
		float3 @float = path[1] - path[0];
		@float = math.normalize(math.float3(-@float.z, 0f, @float.x));
		roadBV.normal = @float;
		roadBV.type = SettlementBV.RoadBV.Type.Highway;
		TerrainPath terrainPath = UnityEngine.Object.Instantiate<TerrainPath>(decal_prefab);
		terrainPath.name = type.ToString() + " -  " + this.roads.Count;
		terrainPath.transform.position = path[0];
		terrainPath.transform.rotation = Quaternion.identity;
		terrainPath.transform.SetParent(this.roads_obj);
		terrainPath.disable_pathfinding = true;
		terrainPath.path_points = (from p in path
		select p).ToList<Vector3>();
		terrainPath.path_colors = colors;
		terrainPath.CreateLines();
		if (spawn_waypoints)
		{
			List<RoadWaypoint> list = new List<RoadWaypoint>();
			foreach (float3 v in path)
			{
				GameObject gameObject = global::Common.SpawnTemplate("road_waypoint", "waypoint", terrainPath.transform, true, new Type[]
				{
					typeof(RoadWaypoint)
				});
				RoadWaypoint component = gameObject.GetComponent<RoadWaypoint>();
				list.Add(component);
				gameObject.transform.position = v;
			}
			for (int j = 0; j < list.Count; j++)
			{
				this.connected_waypoints[list[j]] = list.Count;
			}
		}
		roadBV.path = terrainPath;
		roadBV.path.path_len = num;
		roadBV.pt_len = roadBV.path.path_len;
		return roadBV;
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x000689C8 File Offset: 0x00066BC8
	[ContextMenu("Render debug SDF")]
	private void RenderDebugSDF()
	{
		if (this.debug_sdf_2D == null)
		{
			return;
		}
		Bounds bounds = Terrain.activeTerrain.terrainData.bounds;
		float3 @float = bounds.min;
		float3 float2 = bounds.max;
		int num = 512;
		Texture2D texture2D = new Texture2D(num, num, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		texture2D.name = "SettlementBV_RenderDebugSDF";
		Color[] pixels = texture2D.GetPixels(0, 0, num, num, 0);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num2 = j * num + i;
				float2 s = math.float2((float)i, (float)j) / (float)(num - 1);
				float2 point = math.lerp(@float.xz, float2.xz, s);
				pixels[num2] = Color.black;
				pixels[num2].r = this.debug_sdf_2D.SDF2D(point) / (bounds.size.x / 32f);
			}
		}
		texture2D.SetPixels(pixels);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes("Dbg/Debug_SDF_2D.png", bytes);
		global::Common.DestroyObj(texture2D);
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00068AF8 File Offset: 0x00066CF8
	private void CloneRoadObject(SettlementBV.RoadBV road_to_clone, TerrainPath prefab)
	{
		if (road_to_clone == null)
		{
			return;
		}
		TerrainPath terrainPath = UnityEngine.Object.Instantiate<TerrainPath>(prefab);
		terrainPath.name = road_to_clone.path.gameObject.name + "(clone)";
		terrainPath.step = road_to_clone.path.step;
		terrainPath.disable_pathfinding = road_to_clone.path.disable_pathfinding;
		terrainPath.transform.position = road_to_clone.path.transform.position;
		terrainPath.transform.rotation = Quaternion.identity;
		terrainPath.transform.SetParent(this.roads_obj);
		terrainPath.path_points.Clear();
		terrainPath.path_points.AddRange(road_to_clone.path.path_points);
		terrainPath.path_colors = road_to_clone.path.path_colors;
		terrainPath.CreateLines();
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00068BC4 File Offset: 0x00066DC4
	[ContextMenu("GenerateNeighbourRoads")]
	private void GenerateNeighbourhoodRoads()
	{
		Bounds bounds = Terrain.activeTerrain.terrainData.bounds;
		DirtRoadGraph dirtRoadGraph = new DirtRoadGraph(VoronoiGraph.Generate(new VoronoiGraph.GenerationParameters
		{
			resolution = (int)this.dirt_roads_density,
			center_offset = Mathf.Lerp(0.5f, 0f, this.cell_size_equality),
			iterations = 4,
			merge_distance = math.clamp(this.merge_distance, 0.001f, 0.4f),
			seed = 1865488763 - SettlementBV.seed * 213
		}), bounds);
		float2[] array = this.outer_roads.SelectMany((SettlementBV.RoadBV r) => from p in r.path.path_points
		select p.xz).ToArray<float2>();
		if (array.Length == 0)
		{
			return;
		}
		PolygonSDF2D outer_roads_sdf = new PolygonSDF2D(array);
		IEnumerable<PrefabGrid> enumerable = from g in this.spawned_grids
		where g.cur_type == "SpecialBuildings" || g.cur_type == "ArmyCamp" || g.cur_type == "CapturePoints" || g.cur_type == "Citadels"
		select g;
		CombinedSDF2DObject special_buildings_sdf = new CombinedSDF2DObject();
		foreach (PrefabGrid prefabGrid in enumerable)
		{
			float multiplier = 1f;
			if (prefabGrid.cur_type == "CapturePoints")
			{
				multiplier = 0.3f;
			}
			Vector3[] rectCornersInWorldSpace = prefabGrid.GetRectCornersInWorldSpace();
			Vector3 center = rectCornersInWorldSpace.Aggregate((Vector3 c1, Vector3 c2) => c1 + c2) / 4f;
			PolygonSDF2D item = new PolygonSDF2D((from v in rectCornersInWorldSpace
			select math.lerp(center, v, multiplier).xz).ToArray<float2>());
			special_buildings_sdf.sdf2DObjects.Add(item);
		}
		CombinedSDF2DObject pabbleroad_sdf = new CombinedSDF2DObject();
		pabbleroad_sdf.sdf2DObjects.AddRange(from r in this.roads
		select new PolygonalChainSDF2D((from p in r.path.path_points
		select p.xz).ToArray<float2>()));
		dirtRoadGraph.IntersectWithSDF((float3 p) => outer_roads_sdf.SDF2D(p.xz) + 1f);
		dirtRoadGraph.SnapVertivesToSDFSurface((float3 p) => pabbleroad_sdf.SDF2D(p.xz), this.dirt_roads_snap_distance);
		dirtRoadGraph.RemoveEdgesThatIntersectSDFObject((float3 p) => special_buildings_sdf.SDF2D(p.xz), 0f);
		dirtRoadGraph.RemoveEdgesNearSDFObject((float3 p) => pabbleroad_sdf.SDF2D(p.xz), 4f, this.min_road_to_highway_distance);
		this.debug_dirt_road_graph = dirtRoadGraph;
		foreach (DirtRoadGraph.RoadPath roadPath in dirtRoadGraph.GenerateRoadPaths((float3 p) => pabbleroad_sdf.SDF2D(p.xz) / 12f, this.average_road_segment, 0.8f, 3f))
		{
			SettlementBV.RoadBV roadBV = new SettlementBV.RoadBV();
			roadBV.normal = roadPath.GetNormal();
			roadBV.type = SettlementBV.RoadBV.Type.Subdivision;
			TerrainPath terrainPath = UnityEngine.Object.Instantiate<TerrainPath>(this.subdivisionPrefab);
			terrainPath.name = "Subdivision - " + this.roads.Count;
			terrainPath.transform.position = roadPath.points.First<float3>();
			terrainPath.transform.rotation = Quaternion.identity;
			terrainPath.transform.SetParent(this.roads_obj);
			terrainPath.disable_pathfinding = true;
			terrainPath.path_points.AddRange(from p in roadPath.points
			select p);
			terrainPath.path_colors = null;
			terrainPath.CreateLines();
			this.roads.Add(roadBV);
			roadBV.path = terrainPath;
			roadBV.path.path_len = roadPath.GetLength();
			roadBV.pt_len = roadPath.GetLength();
		}
		this.pdb.BuildRoads((from r in this.roads
		select r.path).ToList<TerrainPath>());
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00069024 File Offset: 0x00067224
	private bool TrySpawnHouse(TerrainPath path, ref float t, float norm_mult = 1f, float max_size = -1f, bool can_spawn_far_from_wall = true)
	{
		this.available_info.Clear();
		for (int i = 0; i < this.house_infos.Count; i++)
		{
			PrefabGrid.Info info = this.house_infos[i];
			this.available_info.AddOption(info, info.rect_width * info.rect_height);
		}
		while (this.available_info.options.Count > 0)
		{
			PrefabGrid.Info info2 = this.available_info.Choose(null, true);
			float pgscale = this.GetPGScale(info2);
			float num = info2.rect_width * pgscale;
			float num2 = info2.rect_height * pgscale;
			float num3 = (path.width + num2) / 2f;
			Vector3 vector;
			Vector3 vector2;
			TerrainPathFinder.GetPathPoint(path.path_points, t + num / 2f, out vector, out vector2, path.step);
			Vector3 vector3 = Vector3.Cross(Vector3.up, new Vector3(vector2.x - vector.x, 0f, vector2.z - vector.z)).normalized * norm_mult;
			if (vector3 == Vector3.zero)
			{
				t += num + 0.5f;
				return false;
			}
			float y = Quaternion.LookRotation(vector3).eulerAngles.y;
			Vector3 vector4 = vector + vector3 * num3;
			int num4 = BattleMap.Get().IsInsideWall(vector4) ? 1 : 0;
			bool flag = BattleMap.Get().IsInsideWall(vector + vector3 * num3 * -1f);
			if (num4 != 0 || !flag)
			{
				if (max_size > 0f)
				{
					float num5 = (info2.rect_width + info2.rect_height) / 2f;
					if (num5 > max_size && num5 != this.smallest_avg_house_size)
					{
						continue;
					}
				}
				if (!this.IsTerrainInvalidForPrefabGrid(vector4, num, num2, y) && !this.IntersectsPG(vector4, num, num2, y) && (can_spawn_far_from_wall || !this.IsTooFarFromWall(vector4, num, num2)))
				{
					num3 += this.distance_to_roads;
					vector4 = vector + vector3 * num3;
					PrefabGrid prefabGrid = this.SpawnHouse(info2, vector4, this.house_container, y, true, true, " - " + path.name);
					Vector3 eulerAngles = prefabGrid.transform.eulerAngles;
					eulerAngles.y = y;
					prefabGrid.transform.eulerAngles = eulerAngles;
					if (this.clusters != null)
					{
						this.clusters[prefabGrid] = prefabGrid;
					}
					t += num + 0.5f;
					return true;
				}
			}
		}
		t += Mathf.Max(1f, this.distance_between_grids);
		return false;
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000692DC File Offset: 0x000674DC
	private PrefabGrid TrySpawnHouse(Point pos, float rot, float norm_mult = 1f, PrefabGrid parent = null, float max_size = -1f, bool can_spawn_far_from_wall = true)
	{
		this.available_info.Clear();
		int i = 0;
		while (i < this.house_infos.Count)
		{
			PrefabGrid.Info info = this.house_infos[i];
			if (max_size <= 0f)
			{
				goto IL_47;
			}
			float num = (info.rect_width + info.rect_height) / 2f;
			if (num <= max_size || num == this.smallest_avg_house_size)
			{
				goto IL_47;
			}
			IL_61:
			i++;
			continue;
			IL_47:
			this.available_info.AddOption(info, info.rect_width + info.rect_height);
			goto IL_61;
		}
		while (this.available_info.options.Count > 0)
		{
			PrefabGrid.Info info2 = this.available_info.Choose(null, true);
			float pgscale = this.GetPGScale(info2);
			float width = info2.rect_width * pgscale;
			float height = info2.rect_height * pgscale;
			if (!this.IsTerrainInvalidForPrefabGrid(pos, width, height, rot) && !this.IntersectsPG(pos, width, height, rot) && (can_spawn_far_from_wall || !this.IsTooFarFromWall(pos, width, height)))
			{
				PrefabGrid prefabGrid = this.SpawnHouse(info2, pos, this.house_container, rot, true, true, " - waves from " + ((parent != null) ? parent.name : null));
				Vector3 eulerAngles = prefabGrid.transform.eulerAngles;
				eulerAngles.y = rot;
				prefabGrid.transform.eulerAngles = eulerAngles;
				if (this.clusters != null)
				{
					this.clusters[prefabGrid] = prefabGrid;
				}
				return prefabGrid;
			}
		}
		return null;
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00069440 File Offset: 0x00067640
	private void SpawnPGsOnRoads()
	{
		for (int i = 0; i < this.roads.Count; i++)
		{
			SettlementBV.RoadBV roadBV = this.roads[i];
			TerrainPath path = roadBV.path;
			float num = 0f;
			do
			{
				this.TrySpawnHouse(path, ref num, 1f, -1f, false);
			}
			while (num < roadBV.pt_len);
			num = 0f;
			do
			{
				this.TrySpawnHouse(path, ref num, -1f, -1f, false);
			}
			while (num < roadBV.pt_len);
		}
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x000694C0 File Offset: 0x000676C0
	private SettlementBV.RoadBV CreateRoadNoPathfinding(Point start_pos, Point end, SettlementBV.RoadBV.Type type, TerrainPath prefab)
	{
		Point pt = end - start_pos;
		float num = pt.Length();
		pt /= num;
		for (float num2 = num / 2f; num2 >= 0f; num2 -= 1f)
		{
			Point point = start_pos + pt * num2;
			if (!this.pdb.lpf.data.IsPassable(point, 0f))
			{
				start_pos = point;
				break;
			}
		}
		for (float num3 = num / 2f; num3 < 2f; num3 += 1f)
		{
			Point point2 = start_pos + pt * num3;
			if (!this.pdb.lpf.data.IsPassable(point2, 0f))
			{
				end = point2;
				break;
			}
		}
		pt = end - start_pos;
		num = pt.Length();
		if (num <= prefab.step * 2f)
		{
			return null;
		}
		pt /= num;
		int num4 = 0;
		while ((float)num4 < num)
		{
			Point v = start_pos + pt * (float)num4;
			if (!this.pdb.lpf.data.IsPassable(v, 0f))
			{
				return null;
			}
			num4++;
		}
		SettlementBV.RoadBV roadBV = new SettlementBV.RoadBV();
		roadBV.normal = end - start_pos;
		roadBV.normal.y = 0f;
		roadBV.normal = Vector3.Cross(Vector3.up, roadBV.normal).normalized;
		roadBV.type = type;
		TerrainPath component = UnityEngine.Object.Instantiate<GameObject>(prefab.gameObject).GetComponent<TerrainPath>();
		component.name = type.ToString() + " -  " + this.roads.Count;
		component.transform.position = start_pos;
		component.transform.rotation = Quaternion.identity;
		component.transform.SetParent(this.roads_obj);
		component.disable_pathfinding = true;
		component.path_points.Clear();
		component.path_points.Add(start_pos);
		component.path_points.Add((start_pos + end) / 2f);
		component.path_points.Add(end);
		component.CreateLines();
		this.roads.Add(roadBV);
		roadBV.path = component;
		roadBV.path.path_len = num;
		roadBV.pt_len = num;
		return roadBV;
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00069748 File Offset: 0x00067948
	private SettlementBV.RoadBV CreateRoadPathfinding(Point start_pos, Point end, bool isMainRoad, SettlementBV.RoadBV.Type type, TerrainPath prefab)
	{
		Point pt = end - start_pos;
		pt.y = 0f;
		float num = pt.Length();
		pt /= num;
		Path path = new Path(null, start_pos, PathData.PassableArea.Type.All, false);
		path.use_max_steps_possible = true;
		path.can_enter_water = false;
		bool flag = !BattleMap.Get().IsInsideWall(start_pos);
		int num2 = 0;
		Logic.PathFinding lpf = this.pdb.lpf;
		for (;;)
		{
			bool flag2 = !BattleMap.Get().IsInsideWall(end);
			if (!isMainRoad && flag != flag2)
			{
				if (num2 >= 3)
				{
					break;
				}
				num *= 0.6f;
				end = start_pos + pt * num;
				num2++;
			}
			else
			{
				path.Find(end, 0f, false);
				path.state = Path.State.Pending;
				lpf.pending.Insert(0, path);
				while (path.state == Path.State.Pending)
				{
					lpf.Process(true, false);
				}
				if (path.state == Path.State.Succeeded)
				{
					goto IL_121;
				}
				if (num2 >= 3)
				{
					goto Block_6;
				}
				num *= 0.6f;
				end = start_pos + pt * num;
				num2++;
			}
		}
		return null;
		Block_6:
		return null;
		IL_121:
		int num3 = 6;
		for (int i = 0; i < 5; i++)
		{
			List<Path.Segment> list = new List<Path.Segment>(path.segments);
			for (int j = 1; j < list.Count - 1; j++)
			{
				PPos pt2 = new PPos(0f, 0f, 0);
				for (int k = -num3; k <= num3; k++)
				{
					pt2.pos += path.segments[math.clamp(j + k, 0, list.Count - 1)].pt.pos;
				}
				Path.Segment value = list[j];
				value.pt.pos = pt2 / ((float)num3 * 2f + 1f);
				list[j] = value;
			}
			for (int l = 1; l < list.Count - 1; l++)
			{
				path.segments[l] = list[l];
			}
		}
		SettlementBV.RoadBV roadBV = new SettlementBV.RoadBV();
		roadBV.normal = end - start_pos;
		roadBV.normal.y = 0f;
		roadBV.normal = Vector3.Cross(Vector3.up, roadBV.normal).normalized;
		roadBV.type = type;
		float num4 = 0f;
		float num5 = 1f;
		do
		{
			PPos pt3;
			PPos ppos;
			path.GetPathPoint(num4, out pt3, out ppos, false, 0f);
			this.pdb.PartialSavePF(pt3, 1);
			num4 += num5;
		}
		while (num4 < path.path_len);
		TerrainPath component = UnityEngine.Object.Instantiate<GameObject>(prefab.gameObject).GetComponent<TerrainPath>();
		component.name = type.ToString() + " -  " + this.roads.Count;
		num5 = component.step;
		component.transform.position = start_pos;
		component.transform.rotation = Quaternion.identity;
		component.transform.SetParent(this.roads_obj);
		component.disable_pathfinding = true;
		component.path_points.Clear();
		num4 = 0f;
		List<RoadWaypoint> list2 = new List<RoadWaypoint>();
		PPos pt4;
		PPos ppos2;
		Vector3 vector;
		do
		{
			path.GetPathPoint(num4, out pt4, out ppos2, false, 0f);
			vector = global::Common.SnapToTerrain(pt4, 0f, null, -1f, false);
			component.path_points.Add(vector);
			if (isMainRoad)
			{
				GameObject gameObject = global::Common.SpawnTemplate("road_waypoint", "waypoint", component.transform, true, new Type[]
				{
					typeof(RoadWaypoint)
				});
				RoadWaypoint component2 = gameObject.GetComponent<RoadWaypoint>();
				list2.Add(component2);
				gameObject.transform.position = vector;
			}
			num4 += num5;
		}
		while (num4 < path.path_len);
		num4 = path.path_len;
		path.GetPathPoint(num4, out pt4, out ppos2, false, 0f);
		vector = global::Common.SnapToTerrain(pt4, 0f, null, -1f, false);
		component.path_points.Add(vector);
		if (isMainRoad)
		{
			GameObject gameObject2 = global::Common.SpawnTemplate("road_waypoint", "waypoint", component.transform, true, new Type[]
			{
				typeof(RoadWaypoint)
			});
			RoadWaypoint component3 = gameObject2.GetComponent<RoadWaypoint>();
			list2.Add(component3);
			gameObject2.transform.position = vector;
		}
		for (int m = 0; m < list2.Count; m++)
		{
			this.connected_waypoints[list2[m]] = list2.Count;
		}
		component.CreateLines();
		this.roads.Add(roadBV);
		roadBV.path = component;
		roadBV.path.path_len = path.path_len;
		roadBV.pt_len = path.path_len;
		return roadBV;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00069C64 File Offset: 0x00067E64
	private SettlementBV.RoadBV CreateRoad(Point start_pos, Point end, bool isMainRoad, SettlementBV.RoadBV.Type type, TerrainPath prefab)
	{
		float height = global::Common.GetHeight(start_pos, null, -1f, false);
		float height2 = global::Common.GetHeight(end, null, -1f, false);
		float waterLevel = MapData.GetWaterLevel();
		if (height < waterLevel || height2 < waterLevel || (type != SettlementBV.RoadBV.Type.Waypoints && (!this.pdb.IsPassable(start_pos) || !this.pdb.IsPassable(end))))
		{
			return null;
		}
		if (type == SettlementBV.RoadBV.Type.Waypoints)
		{
			return this.CreateRoadNoPathfinding(start_pos, end, type, prefab);
		}
		return this.CreateRoadPathfinding(start_pos, end, isMainRoad, type, prefab);
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x00069CF4 File Offset: 0x00067EF4
	private void SpawnRandomHouses()
	{
		if (this.house_infos == null || this.house_infos.Count == 0)
		{
			return;
		}
		BattleMap battleMap = BattleMap.Get();
		List<Point> list = new List<Point>();
		int num = 0;
		while ((float)num < this.t.terrainData.size.x)
		{
			int num2 = 0;
			while ((float)num2 < this.t.terrainData.size.z)
			{
				Point point = new Point((float)num, (float)num2);
				if (!this.IsTerrainInvalid(point, 30f, 15) && !this.TooCloseToOtherGrids(point, 0f, 0f, this.min_camp_dist_to_city) && (!(this.wall != null) || !battleMap.IsInsideWall(point)))
				{
					list.Add(point);
				}
				num2 += 30;
			}
			num += 30;
		}
		int num3 = UnityEngine.Random.Range(this.min_random_houses, this.max_random_houses);
		bool flag = false;
		for (int i = 0; i < num3; i++)
		{
			this.available_info.Clear();
			for (int j = 0; j < this.house_infos.Count; j++)
			{
				PrefabGrid.Info info = this.house_infos[j];
				this.available_info.AddOption(info, info.rect_width + info.rect_height);
			}
			while (this.available_info.options.Count > 0)
			{
				List<Point> list2 = new List<Point>();
				PrefabGrid.Info info2 = this.available_info.Choose(null, true);
				if (info2 != null)
				{
					int num4 = UnityEngine.Random.Range(0, 360);
					for (int k = 0; k < list.Count; k++)
					{
						Point point2 = list[k];
						if (!this.TooCloseToWall(point2, info2.rect_width * this.houseScale, info2.rect_height * this.houseScale, false, this.min_dist_house_to_wall_outside_wall) && !this.TooCloseToOtherGrids(point2, info2.rect_width * this.houseScale, info2.rect_height * this.houseScale, this.min_camp_dist_to_city) && !this.TooCloseToShore(point2, info2.rect_width * this.houseScale, info2.rect_height * this.houseScale, this.min_army_camp_dist_to_coast))
						{
							list2.Add(point2);
						}
					}
					if (list2.Count == 0)
					{
						flag = true;
						break;
					}
					PrefabGrid item = this.SpawnHouse(info2, list2[UnityEngine.Random.Range(0, list2.Count)], this.house_container, (float)num4, true, true, string.Format("RandomHouse_{0}", i));
					this.spawned_outer_houses.Add(item);
				}
			}
			if (flag)
			{
				break;
			}
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00069F9C File Offset: 0x0006819C
	private void GrowPG()
	{
		if (this.citadel != null)
		{
			this.SpawnPGsOnRoads();
		}
		if (this.house_wave_count_inner == 0)
		{
			return;
		}
		this.FindInnerOuterHouses();
		List<PrefabGrid> list = new List<PrefabGrid>();
		for (int i = 0; i < this.inner_houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.inner_houses[i];
			prefabGrid.UpdateInfo(false);
			list.Add(prefabGrid);
			this.clusters[prefabGrid] = prefabGrid;
		}
		for (int j = this.house_wave_count_inner - 1; j >= 0; j--)
		{
			int count = list.Count;
			for (int k = 0; k < count; k++)
			{
				PrefabGrid prefabGrid2 = list[0];
				list.RemoveAt(0);
				float num = (prefabGrid2.rect_width + prefabGrid2.rect_height) / 2f;
				this.GrowPG(prefabGrid2.transform.position, prefabGrid2, list, num * (float)(j + 1) / (float)(this.house_wave_count_inner + 1), false);
				if (list.Count == 0)
				{
					break;
				}
			}
		}
		list.Clear();
		for (int l = 0; l < this.outer_houses.Count; l++)
		{
			PrefabGrid prefabGrid3 = this.outer_houses[l];
			prefabGrid3.UpdateInfo(false);
			list.Add(prefabGrid3);
			this.clusters[prefabGrid3] = prefabGrid3;
		}
		for (int m = this.house_wave_count_outer - 1; m >= 0; m--)
		{
			int count2 = list.Count;
			for (int n = 0; n < count2; n++)
			{
				PrefabGrid prefabGrid4 = list[0];
				list.RemoveAt(0);
				float num2 = (prefabGrid4.rect_width + prefabGrid4.rect_height) / 2f;
				this.GrowPG(prefabGrid4.transform.position, prefabGrid4, list, num2 * (float)(m + 1) / (float)(this.house_wave_count_outer + 1), true);
				if (list.Count == 0)
				{
					break;
				}
			}
		}
		list.Clear();
		for (int num3 = 0; num3 < this.shore_line_pgs.Count; num3++)
		{
			PrefabGrid prefabGrid5 = this.shore_line_pgs[num3];
			prefabGrid5.UpdateInfo(false);
			list.Add(prefabGrid5);
			this.clusters[prefabGrid5] = prefabGrid5;
		}
		for (int num4 = this.house_wave_count_shoreline - 1; num4 >= 0; num4--)
		{
			int count3 = list.Count;
			for (int num5 = 0; num5 < count3; num5++)
			{
				PrefabGrid prefabGrid6 = list[0];
				list.RemoveAt(0);
				float num6 = (prefabGrid6.rect_width + prefabGrid6.rect_height) / 2f;
				this.GrowPG(prefabGrid6.transform.position, prefabGrid6, list, num6 * (float)(num4 + 1) / (float)(this.house_wave_count_shoreline + 1), true);
				if (list.Count == 0)
				{
					break;
				}
			}
		}
		for (int num7 = this.houses.Count - 1; num7 >= 0; num7--)
		{
			PrefabGrid prefabGrid7 = this.houses[num7];
			GameObject gameObject = (prefabGrid7 != null) ? prefabGrid7.gameObject : null;
			if (!gameObject.activeInHierarchy)
			{
				this.houses.RemoveAt(num7);
				this.spawned_grids.Remove(prefabGrid7);
				global::Common.DestroyObj(gameObject);
			}
		}
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0006A2BC File Offset: 0x000684BC
	private bool GrowPG(Point original_pos, PrefabGrid grid, List<PrefabGrid> frontier, float max_size, bool can_spawn_far_from_wall = true)
	{
		if (grid == null)
		{
			return false;
		}
		bool result = false;
		float num = grid.transform.forward.Heading();
		float num2 = grid.ScaledRadiusInner();
		for (float num3 = 0f; num3 < 360f; num3 += this.rotation_angle)
		{
			float num4 = num + num3;
			if (num4 > 360f)
			{
				num4 -= 360f;
			}
			Point pt = PPos.UnitRight.GetRotated(num4) * (num2 * 2f + this.distance_between_grids);
			Point point = original_pos + pt;
			if (BattleMap.Get().IsInsideWall(original_pos) == BattleMap.Get().IsInsideWall(point))
			{
				PrefabGrid prefabGrid = this.TrySpawnHouse(point, num4, 1f, grid, max_size, can_spawn_far_from_wall);
				if (prefabGrid != null)
				{
					result = true;
					frontier.Add(prefabGrid);
				}
			}
		}
		return result;
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x0006A3B4 File Offset: 0x000685B4
	private PrefabGrid SpawnHouse(PrefabGrid parent, PrefabGrid.Info info, Point pos, PrefabGrid grid)
	{
		string obj_name = info.architecture + " house";
		if (this.clusters != null)
		{
			obj_name = this.clusters[parent].name + " " + info.type + " (Clone)";
		}
		PrefabGrid prefabGrid = global::Common.SpawnTemplate<PrefabGrid>("PrefabGrid", obj_name, grid.transform.parent, false, new Type[]
		{
			typeof(PrefabGrid)
		});
		GameObject gameObject = prefabGrid.gameObject;
		gameObject.transform.position = global::Common.SnapToTerrain(pos, 0f, null, -1f, false);
		float pgscale = this.GetPGScale(info);
		gameObject.transform.localScale = Vector3.one * pgscale;
		if (grid != null)
		{
			gameObject.transform.rotation = grid.transform.rotation;
		}
		if (this.clusters != null)
		{
			this.clusters[prefabGrid] = parent;
		}
		this.spawned_grids.Add(prefabGrid);
		prefabGrid.architecture = info.architecture;
		prefabGrid.type = info.type;
		prefabGrid.info = info;
		prefabGrid.UpdateInfo(false);
		this.RandomRotate(prefabGrid);
		prefabGrid.set_variant = -1;
		prefabGrid.set_level = -1;
		prefabGrid.cur_variant = -1;
		prefabGrid.cur_level = -1;
		prefabGrid.DecideVariant();
		gameObject.SetActive(true);
		prefabGrid.Refresh(false, true);
		this.houses.Add(prefabGrid);
		return prefabGrid;
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0006A524 File Offset: 0x00068724
	private PrefabGrid SpawnHouse(PrefabGrid.Info info, Point pos, Transform parent, float heading, bool is_house = true, bool random_rotate = true, string suff = "")
	{
		string obj_name = info.architecture + " " + info.type + suff;
		PrefabGrid prefabGrid = global::Common.SpawnTemplate<PrefabGrid>("PrefabGrid", obj_name, parent, false, new Type[]
		{
			typeof(PrefabGrid)
		});
		GameObject gameObject = prefabGrid.gameObject;
		gameObject.transform.position = global::Common.SnapToTerrain(pos, 0f, null, -1f, false);
		float pgscale = this.GetPGScale(info);
		gameObject.transform.localScale = Vector3.one * pgscale;
		Vector3 eulerAngles = gameObject.transform.eulerAngles;
		eulerAngles.y = heading;
		gameObject.transform.eulerAngles = eulerAngles;
		this.spawned_grids.Add(prefabGrid);
		prefabGrid.architecture = info.architecture;
		prefabGrid.type = info.type;
		prefabGrid.info = info;
		prefabGrid.UpdateInfo(false);
		if (random_rotate)
		{
			this.RandomRotate(prefabGrid);
		}
		prefabGrid.set_variant = -1;
		prefabGrid.set_level = -1;
		prefabGrid.cur_variant = -1;
		prefabGrid.cur_level = -1;
		prefabGrid.DecideVariant();
		gameObject.SetActive(true);
		prefabGrid.Refresh(true, true);
		if (is_house)
		{
			this.houses.Add(prefabGrid);
		}
		return prefabGrid;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0006A650 File Offset: 0x00068850
	private bool IsTerrainValidForSquad(Point pos, float width, float height, float rotation)
	{
		return this.pdb.lpf.data.IsPassable(pos, 0f);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0006A674 File Offset: 0x00068874
	private bool IsTerrainInvalidForPrefabGrid(Point pos, float width, float height, float rotation)
	{
		RuntimePathDataBuilder runtimePathDataBuilder = BattleMap.Get().pdb;
		if (runtimePathDataBuilder != null)
		{
			int x;
			int y;
			runtimePathDataBuilder.WorldToGrid(pos, out x, out y);
			if (!runtimePathDataBuilder.IsInBounds(x, y))
			{
				return true;
			}
			if ((runtimePathDataBuilder.GetCell(pos).bits & 1) != 0)
			{
				return true;
			}
			if (!runtimePathDataBuilder.lpf.data.IsPassable(pos, 0f))
			{
				return true;
			}
		}
		float waterLevel = MapData.GetWaterLevel();
		if (global::Common.SnapToTerrain(pos, 0f, null, -1f, false).y <= waterLevel)
		{
			return true;
		}
		if (this.TooCloseToShore(pos, width, height, this.min_dist_to_shore))
		{
			return true;
		}
		TerrainData terrainData = this.t.terrainData;
		SettlementBV.PGRect rotated = SettlementBV.PGRect.GetRotated(pos, rotation, width / 2f, height / 2f);
		Point point = Point.Zero;
		for (int i = 0; i < rotated.Points.Length; i++)
		{
			Point pt = rotated.Points[i];
			point += pt;
		}
		point /= (float)rotated.Points.Length;
		for (int j = 0; j < rotated.Points.Length; j++)
		{
			Point point2 = rotated.Points[j];
			if (global::Common.SnapToTerrain(point2, 0f, null, -1f, false).y <= waterLevel)
			{
				return true;
			}
			if (runtimePathDataBuilder != null)
			{
				Point b = rotated.Points[(j + 1) % rotated.Points.Length];
				bool result = false;
				if (this.IsTerrainInvalid(point2, b, terrainData, out result))
				{
					return result;
				}
				if (this.IsTerrainInvalid(point2, point, terrainData, out result))
				{
					return result;
				}
			}
		}
		return false;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0006A81A File Offset: 0x00068A1A
	private bool IsPassable(Point pos)
	{
		RuntimePathDataBuilder runtimePathDataBuilder = this.pdb;
		return ((runtimePathDataBuilder != null) ? runtimePathDataBuilder.lpf : null) == null || this.pdb.lpf.data.IsPassable(pos, 0f);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0006A854 File Offset: 0x00068A54
	private bool IsWithinBounds(Point pos, float r)
	{
		RuntimePathDataBuilder runtimePathDataBuilder = this.pdb;
		if (((runtimePathDataBuilder != null) ? runtimePathDataBuilder.lpf : null) == null)
		{
			return true;
		}
		float num = (float)this.pdb.lpf.settings.map_bounds_width + r;
		return pos.x > num && pos.x <= (float)this.pdb.lpf.data.width - num && pos.y > num && pos.y <= (float)this.pdb.lpf.data.height - num;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0006A8E8 File Offset: 0x00068AE8
	private bool IsTerrainInvalid(Point a, Point b, TerrainData data, out bool res)
	{
		Point point = a;
		float tile_size = 1f;
		Coord tile = Coord.WorldToGrid(point, tile_size);
		Point point2 = Coord.WorldToLocal(tile, point, tile_size);
		Point point3 = Coord.WorldToLocal(tile, b, tile_size);
		res = false;
		while (data.GetSteepness(point.x / data.size.x, point.y / data.size.z) <= this.max_slope)
		{
			if (point.x < 0f || point.x >= this.t.terrainData.size.x || point.y < 0f || point.y >= this.t.terrainData.size.z)
			{
				res = false;
				return true;
			}
			if ((this.pdb.GetCell(point).bits & 1) != 0)
			{
				res = true;
				return true;
			}
			if (!this.IsPassable(point))
			{
				res = true;
				return true;
			}
			point = Coord.GridToWorld(tile, tile_size);
			if (!Coord.RayStep(ref tile, ref point2, ref point3))
			{
				return false;
			}
		}
		res = true;
		return true;
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0006A9FC File Offset: 0x00068BFC
	private bool IsTerrainInvalid(Point pos, float r, int angle_delta = 45)
	{
		RuntimePathDataBuilder runtimePathDataBuilder = BattleMap.Get().pdb;
		if (runtimePathDataBuilder != null && (runtimePathDataBuilder.GetCell(pos).bits & 1) != 0)
		{
			return true;
		}
		if (!this.IsPassable(pos))
		{
			return true;
		}
		TerrainData terrainData = this.t.terrainData;
		for (int i = 0; i < 360; i += angle_delta)
		{
			float num = (float)i;
			if (num < 0f)
			{
				num += 360f;
			}
			if (num > 360f)
			{
				num -= 360f;
			}
			Point pt = PPos.UnitRight.GetRotated(num) * r;
			Point point = pos + pt;
			Vector3 vector = global::Common.SnapToTerrain(point, 0f, null, -1f, false);
			float steepness = terrainData.GetSteepness(vector.x / terrainData.size.x, vector.z / terrainData.size.z);
			if (!this.IsPassable(point) || steepness > this.max_slope)
			{
				return true;
			}
			if (runtimePathDataBuilder != null && (runtimePathDataBuilder.GetCell(point).bits & 1) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0006AB20 File Offset: 0x00068D20
	private static bool IsPolygonsIntersecting(SettlementBV.PGRect a, SettlementBV.PGRect b)
	{
		foreach (SettlementBV.PGRect pgrect in new SettlementBV.PGRect[]
		{
			a,
			b
		})
		{
			int j = 0;
			while (j < pgrect.Points.Length)
			{
				int num = (j + 1) % pgrect.Points.Length;
				Point point = pgrect.Points[j];
				Point point2 = pgrect.Points[num];
				Point point3 = new Point(point2.y - point.y, point.x - point2.x);
				double? num2 = null;
				double? num3 = null;
				Point[] points = a.Points;
				int k = 0;
				double? num6;
				while (k < points.Length)
				{
					Point point4 = points[k];
					float num4 = point3.x * point4.x + point3.y * point4.y;
					if (num2 == null)
					{
						goto IL_EC;
					}
					double num5 = (double)num4;
					num6 = num2;
					if (num5 < num6.GetValueOrDefault() & num6 != null)
					{
						goto IL_EC;
					}
					IL_F6:
					if (num3 == null)
					{
						goto IL_119;
					}
					double num7 = (double)num4;
					num6 = num3;
					if (num7 > num6.GetValueOrDefault() & num6 != null)
					{
						goto IL_119;
					}
					IL_123:
					k++;
					continue;
					IL_119:
					num3 = new double?((double)num4);
					goto IL_123;
					IL_EC:
					num2 = new double?((double)num4);
					goto IL_F6;
				}
				double? num8 = null;
				double? num9 = null;
				points = b.Points;
				k = 0;
				while (k < points.Length)
				{
					Point point5 = points[k];
					float num10 = point3.x * point5.x + point3.y * point5.y;
					if (num8 == null)
					{
						goto IL_1A3;
					}
					double num11 = (double)num10;
					num6 = num8;
					if (num11 < num6.GetValueOrDefault() & num6 != null)
					{
						goto IL_1A3;
					}
					IL_1AD:
					if (num9 == null)
					{
						goto IL_1D0;
					}
					double num12 = (double)num10;
					num6 = num9;
					if (num12 > num6.GetValueOrDefault() & num6 != null)
					{
						goto IL_1D0;
					}
					IL_1DA:
					k++;
					continue;
					IL_1D0:
					num9 = new double?((double)num10);
					goto IL_1DA;
					IL_1A3:
					num8 = new double?((double)num10);
					goto IL_1AD;
				}
				num6 = num3;
				double? num13 = num8;
				if (!(num6.GetValueOrDefault() < num13.GetValueOrDefault() & (num6 != null & num13 != null)))
				{
					num13 = num9;
					num6 = num2;
					if (!(num13.GetValueOrDefault() < num6.GetValueOrDefault() & (num13 != null & num6 != null)))
					{
						j++;
						continue;
					}
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0006AD90 File Offset: 0x00068F90
	private float GetPGScale(PrefabGrid.Info go)
	{
		if (go.type == "Citadels")
		{
			return this.citadelScale * this.globalScale;
		}
		if (go.type == "ArmyCamp")
		{
			return this.globalScale;
		}
		if (go.type == "Dock")
		{
			return this.globalScale;
		}
		return this.houseScale * this.globalScale;
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0006ADFC File Offset: 0x00068FFC
	public bool IsTooFarFromWall(Point pos, float width, float height)
	{
		if (this.wall != null && this.wall.gameObject.activeInHierarchy)
		{
			float num = Mathf.Sqrt(width * width + height * height) / 2f;
			return BattleMap.Get().GetDistToWall(pos) > this.max_dist_house_to_wall_outside_wall + num;
		}
		return false;
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0006AE58 File Offset: 0x00069058
	public bool IntersectsPG(Point pos, float width, float height, float rotation)
	{
		if (this.wall != null && this.wall.gameObject.activeInHierarchy)
		{
			float num = Mathf.Sqrt(width * width + height * height) / 2f;
			float distToWall = BattleMap.Get().GetDistToWall(pos);
			float num2 = Math.Abs(distToWall);
			float num3 = this.min_dist_house_to_wall_inside_wall;
			if (distToWall > 0f)
			{
				num3 = this.min_dist_house_to_wall_outside_wall;
			}
			if (num2 < num + num3 && num2 < BattleMap.Get().wall_sdf_max_circle)
			{
				return true;
			}
		}
		SettlementBV.PGRect rotated = SettlementBV.PGRect.GetRotated(pos, rotation, width / 2f, height / 2f);
		for (int i = 0; i < this.spawned_grids.Count; i++)
		{
			PrefabGrid prefabGrid = this.spawned_grids[i];
			float pgscale = this.GetPGScale(prefabGrid.info);
			SettlementBV.PGRect rotated2 = SettlementBV.PGRect.GetRotated(prefabGrid.transform.position, prefabGrid.transform.eulerAngles.y, prefabGrid.rect_width * pgscale / 2f, prefabGrid.rect_height * pgscale / 2f);
			if (SettlementBV.IsPolygonsIntersecting(rotated, rotated2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0006AF84 File Offset: 0x00069184
	public int CalcLevel(Castle c, out int logic_level, out int logic_max_level, out int logic_wall_level, out int logic_wall_max_level, out int logic_citadel_level, out int Logic_citadel_max_level)
	{
		if (c == null)
		{
			logic_level = 0;
			logic_max_level = 0;
			logic_wall_level = 0;
			logic_wall_max_level = 0;
			logic_citadel_level = 0;
			Logic_citadel_max_level = 0;
			return 0;
		}
		logic_wall_level = Mathf.Max(1, c.GetWallLevel());
		logic_wall_max_level = Mathf.Max(logic_wall_level, c.GetMaxWallLevel());
		logic_citadel_level = c.GetCitadelLevel();
		Logic_citadel_max_level = c.GetCitadelMaxLevel();
		logic_level = c.CalcLevel();
		logic_max_level = c.GetMaxLevel();
		int maxLevel = this.GetMaxLevel();
		return global::Common.map(logic_level, 1, logic_max_level, 1, maxLevel, true);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x0006B004 File Offset: 0x00069204
	public void LoadHouses()
	{
		if (this.house_container != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.house_container.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.house_container.gameObject);
			}
		}
		this.houses.Clear();
		if (this.house_infos == null || this.house_infos.Count == 0)
		{
			return;
		}
		this.house_container = new GameObject("Houses").transform;
		this.house_container.SetParent(base.transform);
		this.house_container.transform.localRotation = Quaternion.identity;
		this.house_container.localPosition = Vector3.zero;
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0006B0B4 File Offset: 0x000692B4
	public void LoadUniqueContainer()
	{
		this.unique_container = new GameObject("UniqueBuildings").transform;
		this.unique_container.SetParent(base.transform);
		this.unique_container.transform.localRotation = Quaternion.identity;
		this.unique_container.localPosition = Vector3.zero;
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0006B10C File Offset: 0x0006930C
	private void SpawnGates()
	{
		Wall wall = this.wall;
		if (((wall != null) ? wall.corners : null) == null || this.wall.corners.Count == 0)
		{
			this.GetWallCorners();
			return;
		}
		List<int> list = new List<int>();
		List<WallCorner> list2 = new List<WallCorner>();
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			list2.Add(this.wall.corners[i]);
		}
		int num = list2.Count - 1;
		int num2 = this.min_gates;
		if (this.min_gates < num)
		{
			num2 = UnityEngine.Random.Range(this.min_gates, num);
		}
		for (int j = 0; j < num2; j++)
		{
			for (int k = 0; k < num; k++)
			{
				int num3 = UnityEngine.Random.Range(0, num);
				WallCorner wallCorner = list2[num3];
				if (wallCorner.type != WallCorner.Type.Gate && !list.Contains(num3))
				{
					int index = (num3 + 1) % list2.Count;
					WallCorner wallCorner2 = list2[index];
					if (wallCorner2.type != WallCorner.Type.Gate)
					{
						list.Add(num3);
						list.Add((num3 - 1 < 0) ? list2.Count : (num3 - 1));
						list.Add((num3 + 1) % list2.Count);
						WallCorner component = global::Common.SpawnTemplate("WallCorner", "Gate", this.wall.transform, true, new Type[]
						{
							typeof(WallCorner),
							typeof(PrefabGrid)
						}).GetComponent<WallCorner>();
						component.type = WallCorner.Type.Gate;
						component.transform.position = Vector3.Lerp(wallCorner.transform.position, wallCorner2.transform.position, 0.5f);
						component.GetComponent<PrefabGrid>().architecture = this.gates_architecture;
						this.wall.corners.Add(component);
						break;
					}
				}
			}
		}
		if (this.south_camps.Count > 0)
		{
			Point point = this.south_camps[0].transform.position;
			Point pt = point;
			float num4 = float.MaxValue;
			float num5 = float.MaxValue;
			for (int l = 0; l < this.wall.corners.Count; l++)
			{
				WallCorner wallCorner3 = this.wall.corners[l];
				WallCorner wallCorner4 = this.wall.corners[(l + 1) % this.wall.corners.Count];
				if (wallCorner3.type == WallCorner.Type.Gate)
				{
					float num6 = point.SqrDist(wallCorner3.transform.position);
					if (num6 < num5)
					{
						num5 = num6;
					}
				}
				else if (wallCorner4.type == WallCorner.Type.Gate)
				{
					float num7 = point.SqrDist(wallCorner4.transform.position);
					if (num7 < num5)
					{
						num5 = num7;
					}
				}
				else
				{
					Vector3 v = (wallCorner3.transform.position + wallCorner4.transform.position) / 2f;
					float num8 = point.SqrDist(v);
					if (num8 < num4)
					{
						num4 = num8;
						pt = v;
					}
				}
			}
			if (num4 < 3.4028235E+38f && num4 < num5 * 0.95f)
			{
				WallCorner component2 = global::Common.SpawnTemplate("WallCorner", "Gate_Additive", this.wall.transform, true, new Type[]
				{
					typeof(WallCorner),
					typeof(PrefabGrid)
				}).GetComponent<WallCorner>();
				component2.type = WallCorner.Type.Gate;
				component2.transform.position = pt;
				component2.GetComponent<PrefabGrid>().architecture = this.gates_architecture;
			}
		}
		this.wall.Refresh(true);
		this.GetWallCorners();
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0006B4DC File Offset: 0x000696DC
	private void SpawnAdditionalTowers()
	{
		Wall wall = this.wall;
		if (((wall != null) ? wall.corners : null) == null || this.wall.corners.Count == 0)
		{
			return;
		}
		List<WallCorner> list = new List<WallCorner>();
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			list.Add(this.wall.corners[i]);
		}
		for (int j = 0; j < list.Count; j++)
		{
			WallCorner wallCorner = list[j];
			UnityEngine.Component component = list[(j + 1) % list.Count];
			Point pt = wallCorner.transform.position;
			Point point = component.transform.position;
			if (pt.Dist(point) > this.max_dist_between_towers)
			{
				WallCorner component2 = global::Common.SpawnTemplate("WallCorner", "Tower_Additive", this.wall.transform, true, new Type[]
				{
					typeof(WallCorner),
					typeof(PrefabGrid)
				}).GetComponent<WallCorner>();
				component2.type = WallCorner.Type.Tower;
				component2.transform.position = (pt + point) / 2f;
				component2.GetComponent<PrefabGrid>().architecture = this.towers_architecture;
			}
		}
		this.wall.Refresh(true);
		this.GetWallCorners();
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0006B637 File Offset: 0x00069837
	private void Scale()
	{
		this.ScaleCitadel();
		this.ScaleHouses();
		this.ScaleWall();
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0006B64C File Offset: 0x0006984C
	public void ScaleCitadel()
	{
		if (this.citadel == null)
		{
			return;
		}
		float pgscale = this.GetPGScale(this.citadel.info);
		this.citadel.transform.localScale = Vector3.one * pgscale;
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0006B698 File Offset: 0x00069898
	public void ScaleWall()
	{
		if (this.wall == null)
		{
			return;
		}
		Point point = base.transform.position;
		if (this.citadel != null)
		{
			point = this.citadel.transform.position;
		}
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			WallCorner wallCorner = this.wall.corners[i];
			Point pt = wallCorner.transform.position - point;
			wallCorner.transform.position = global::Common.SnapToTerrain(point + pt * this.WallDistance, 0f, null, -1f, false);
		}
		this.wall.transform.localRotation = Quaternion.identity;
		this.wall.transform.localScale = Vector3.one;
		this.wall.HeightScale = this.wallHeight * this.wallScale * this.globalScale;
		this.wall.ThicknessScale = this.wallThickness * this.wallScale * this.globalScale;
		this.wall.MaxLengthScale = this.wallMaxLength * this.wallScale * this.globalScale;
		this.GetWallCorners();
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0006B7EC File Offset: 0x000699EC
	public void ScaleWallCorners()
	{
		Wall wall = this.wall;
		if (((wall != null) ? wall.corners : null) == null)
		{
			return;
		}
		for (int i = 0; i < this.wall.corners.Count; i++)
		{
			WallCorner wallCorner = this.wall.corners[i];
			if (wallCorner.type == WallCorner.Type.Gate)
			{
				wallCorner.transform.localScale = new Vector3(this.towerThickness, this.gateHeight, this.towerThickness) * this.wallScale * this.globalScale * this.gateScale;
			}
			else
			{
				wallCorner.transform.localScale = new Vector3(this.towerThickness, this.towerHeight, this.towerThickness) * this.wallScale * this.globalScale * this.towerScale;
			}
		}
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0006B8D4 File Offset: 0x00069AD4
	public void ScaleHouses()
	{
		this.FindInnerOuterHouses();
		if (this.wall != null && this.citadel != null)
		{
			Point point = this.citadel.transform.position;
			for (int i = 0; i < this.outer_houses.Count; i++)
			{
				PrefabGrid prefabGrid = this.outer_houses[i];
				Vector3 a = prefabGrid.transform.position - point;
				prefabGrid.transform.position = point + a * this.HouseOuterDistance;
				this.outer_houses.Add(prefabGrid);
			}
			for (int j = 0; j < this.inner_houses.Count; j++)
			{
				PrefabGrid prefabGrid2 = this.inner_houses[j];
				Vector3 a2 = prefabGrid2.transform.position - point;
				prefabGrid2.transform.position = point + a2 * this.HouseInnerDistance;
				this.outer_houses.Add(prefabGrid2);
			}
		}
		for (int k = 0; k < this.houses.Count; k++)
		{
			PrefabGrid prefabGrid3 = this.houses[k];
			float pgscale = this.GetPGScale(prefabGrid3.info);
			prefabGrid3.transform.localScale = Vector3.one * pgscale;
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0006BA54 File Offset: 0x00069C54
	private void FindInnerOuterHouses()
	{
		this.inner_houses.Clear();
		this.outer_houses.Clear();
		if (this.wall != null && this.citadel != null)
		{
			this.citadel.transform.position;
			for (int i = 0; i < this.houses.Count; i++)
			{
				PrefabGrid prefabGrid = this.houses[i];
				Point pt = prefabGrid.transform.position;
				if (BattleMap.Get().IsInsideWall(pt))
				{
					this.inner_houses.Add(prefabGrid);
				}
				else
				{
					this.outer_houses.Add(prefabGrid);
				}
			}
			return;
		}
		this.outer_houses.AddRange(this.houses);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0006BB20 File Offset: 0x00069D20
	public void Refresh()
	{
		if (this.wall != null)
		{
			this.wall.Refresh(false);
		}
		if (this.citadel != null)
		{
			this.citadel.Refresh(false, true);
		}
		for (int i = 0; i < this.houses.Count; i++)
		{
			this.houses[i].Refresh(false, true);
		}
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0006BB8C File Offset: 0x00069D8C
	private void RandomRotate(PrefabGrid house)
	{
		float num = (float)UnityEngine.Random.Range(0, 6) * this.rotation_angle;
		num = global::Common.NormalizeAngle360(house.transform.eulerAngles.y + num);
		house.transform.eulerAngles = new Vector3(0f, num, 0f);
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0006BBDC File Offset: 0x00069DDC
	private void FindPrefabGrids(Transform parent, List<PrefabGrid> pgs)
	{
		if (parent == null)
		{
			return;
		}
		if (!parent.gameObject.activeSelf)
		{
			return;
		}
		PrefabGrid component = parent.GetComponent<PrefabGrid>();
		if (component != null)
		{
			pgs.Add(component);
			return;
		}
		foreach (object obj in parent)
		{
			Transform parent2 = (Transform)obj;
			this.FindPrefabGrids(parent2, pgs);
		}
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0006BC64 File Offset: 0x00069E64
	public void CalcLevelFromHouses()
	{
		if (this.houses.Count <= 0)
		{
			return;
		}
		this.level = 1;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid = this.houses[i];
			prefabGrid.UpdateInfo(false);
			prefabGrid.DecideLevel();
			if (prefabGrid.set_level <= 0 && prefabGrid.cur_level > 1)
			{
				this.level += prefabGrid.cur_level - 1;
			}
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0006BCE0 File Offset: 0x00069EE0
	public PrefabGrid ChooseHouseToUpgrade()
	{
		int num = -1;
		PrefabGrid prefabGrid = null;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid2 = this.houses[i];
			if (prefabGrid2.set_level <= 0)
			{
				int num2 = prefabGrid2.cur_level;
				if (num2 < prefabGrid2.GetMaxLevel())
				{
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (!(prefabGrid != null) || num > num2)
					{
						prefabGrid = prefabGrid2;
						num = num2;
					}
				}
			}
		}
		return prefabGrid;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x0006BD4C File Offset: 0x00069F4C
	public PrefabGrid ChooseHouseToDegrade()
	{
		int num = -1;
		PrefabGrid prefabGrid = null;
		for (int i = 0; i < this.houses.Count; i++)
		{
			PrefabGrid prefabGrid2 = this.houses[i];
			if (prefabGrid2.set_level <= 0)
			{
				int num2 = prefabGrid2.cur_level;
				if (num2 > 1)
				{
					int maxLevel = prefabGrid2.GetMaxLevel();
					if (num2 > maxLevel)
					{
						num2 = maxLevel;
					}
					if (!(prefabGrid != null) || num < num2)
					{
						prefabGrid = prefabGrid2;
						num = num2;
					}
				}
			}
		}
		return prefabGrid;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x0006BDBC File Offset: 0x00069FBC
	public void UpdateHouseLevels(int lvl, bool refresh)
	{
		if (this.houses.Count <= 0)
		{
			return;
		}
		this.CalcLevelFromHouses();
		while (this.level < lvl)
		{
			PrefabGrid prefabGrid = this.ChooseHouseToUpgrade();
			if (prefabGrid == null)
			{
				return;
			}
			prefabGrid.cur_level++;
			if (refresh)
			{
				prefabGrid.Refresh(true, true);
			}
			this.level++;
		}
		while (this.level > lvl)
		{
			PrefabGrid prefabGrid2 = this.ChooseHouseToDegrade();
			if (prefabGrid2 == null)
			{
				return;
			}
			prefabGrid2.cur_level--;
			if (refresh)
			{
				prefabGrid2.Refresh(true, true);
			}
			this.level--;
		}
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x0006BE64 File Offset: 0x0006A064
	private void GenerateMapBounds()
	{
		if (this.map_bounds != null)
		{
			global::Common.DestroyObj(this.map_bounds.gameObject);
			this.map_bounds = null;
		}
		if (!(this.map_bounds_material == null))
		{
			RuntimePathDataBuilder runtimePathDataBuilder = this.pdb;
			bool flag;
			if (runtimePathDataBuilder == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.PathFinding lpf = runtimePathDataBuilder.lpf;
				flag = (((lpf != null) ? lpf.settings : null) != null);
			}
			if (flag)
			{
				int map_bounds_width = this.pdb.lpf.settings.map_bounds_width;
				this.map_bounds = global::Common.SpawnTemplate("MapBounds", "MapBounds", base.transform, true, new Type[]
				{
					typeof(MapBorderMesh)
				}).GetComponent<MapBorderMesh>();
				this.map_bounds.mat = this.map_bounds_material;
				this.map_bounds.transform.position = new Vector3((float)map_bounds_width, 0f, (float)map_bounds_width);
				this.map_bounds.transform.rotation = Quaternion.identity;
				Vector3 size = this.t.terrainData.size;
				this.map_bounds.width = size.x - (float)(map_bounds_width * 2);
				this.map_bounds.length = size.z - (float)(map_bounds_width * 2);
				this.map_bounds.UpdateMesh();
				return;
			}
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x0006BF9C File Offset: 0x0006A19C
	private void GenerateShorelineDecorations()
	{
		this.shore_line_pgs.Clear();
		if (this.dock_info == null)
		{
			return;
		}
		MakeMap component = BattleMap.Get().GetComponent<MakeMap>();
		Texture2D texture2D = ((component != null) ? component.graph : null).generated_textures["shore_normals"];
		if (texture2D == null)
		{
			return;
		}
		List<float4> list = new List<float4>();
		float r = (this.dock_info.rect_width + this.dock_info.rect_height) / 2f;
		for (int i = 0; i < texture2D.width; i++)
		{
			for (int j = 0; j < texture2D.height; j++)
			{
				if (this.IsCoast(i, j, texture2D))
				{
					int num;
					int num2;
					this.ShoreTexToPDB(i, j, texture2D, out num, out num2);
					if (this.IsWithinBounds(new Point((float)num, (float)num2), r))
					{
						Vector3 vector = Vector3.zero;
						for (int k = i - 1; k <= i + 1; k++)
						{
							for (int l = j - 1; l <= j + 1; l++)
							{
								if (k >= 0 && k < texture2D.width && l >= 0 && l < texture2D.height && this.IsCoast(k, l, texture2D))
								{
									Color pixel = texture2D.GetPixel(k, l);
									vector -= new Vector3((pixel.g - 0.5f) * 2f, 0f, (pixel.b - 0.5f) * 2f);
								}
							}
						}
						vector.Normalize();
						vector *= 5f;
						list.Add(new float4((float)num, (float)num2, vector.x, vector.z));
					}
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		float pgscale = this.GetPGScale(this.dock_info);
		float width = pgscale * this.dock_info.rect_width;
		float height = pgscale * this.dock_info.rect_height;
		while (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			float4 @float = list[index];
			list.RemoveAt(index);
			Point pos = new Point(@float.x, @float.y);
			if (!this.TooCloseToOtherGrids(pos, width, height, this.min_dist_between_docks, this.shore_line_pgs))
			{
				Point pt = new Point(@float.z, @float.w);
				PrefabGrid prefabGrid = this.SpawnHouse(this.dock_info, pos, this.unique_container, Quaternion.LookRotation(pt).eulerAngles.y, false, false, "");
				this.spawned_outer_houses.Add(prefabGrid);
				if (!(prefabGrid == null))
				{
					this.shore_line_pgs.Add(prefabGrid);
				}
			}
		}
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0006C25A File Offset: 0x0006A45A
	private void ShoreTexToPDB(int x, int y, Texture2D shore, out int x_correct, out int y_correct)
	{
		x_correct = (int)((float)this.pdb.width * ((float)x / (float)shore.width));
		y_correct = (int)((float)this.pdb.height * ((float)y / (float)shore.height));
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x0006C292 File Offset: 0x0006A492
	private void PDBToShoreTex(int x, int y, Texture2D shore, out int x_correct, out int y_correct)
	{
		x_correct = (int)((float)shore.width * ((float)x / (float)this.pdb.width));
		y_correct = (int)((float)shore.height * ((float)y / (float)this.pdb.height));
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0006C2CC File Offset: 0x0006A4CC
	private bool IsCoast(int x, int y, Texture2D shore)
	{
		int num;
		int num2;
		this.ShoreTexToPDB(x, y, shore, out num, out num2);
		if ((this.pdb.GetCell(num, num2).bits & 8) != 0)
		{
			bool flag = false;
			for (int i = num - 1; i <= num + 1; i++)
			{
				for (int j = num2 - 1; j <= num2 + 1; j++)
				{
					if (i >= 0 && i < this.pdb.width && j >= 0 && j < this.pdb.height && (this.pdb.GetCell(i, j).bits & 8) == 0)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}
		return false;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0006C880 File Offset: 0x0006AA80
	[CompilerGenerated]
	internal static float <SpawnCapturePoints>g__DistanceToGate|202_2(float3 pos, ref SettlementBV.<>c__DisplayClass202_0 A_1)
	{
		return A_1.gate_positions.Min((float3 p) => math.distance(p.xz, pos.xz));
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0006C8B4 File Offset: 0x0006AAB4
	[CompilerGenerated]
	internal static List<float3> <GenerateRoads>g__CreateEqualStepPath|225_9(Path lp, float step_size)
	{
		List<float3> list = new List<float3>();
		int num = math.max(2, (int)(lp.path_len / step_size));
		step_size = lp.path_len / (float)num;
		for (int i = 0; i <= num; i++)
		{
			float num2 = (float)i / (float)num;
			PPos ppos;
			PPos ppos2;
			lp.GetPathPoint(step_size * (float)i, out ppos, out ppos2, false, 0f);
			list.Add(math.float3(ppos.pos.x, 0f, ppos.pos.y));
		}
		return list;
	}

	// Token: 0x04000728 RID: 1832
	[HideInInspector]
	public int id = 48;

	// Token: 0x04000729 RID: 1833
	public DT.Field field;

	// Token: 0x0400072A RID: 1834
	public static int seed = 0;

	// Token: 0x0400072B RID: 1835
	[Header("Scale")]
	public float globalScale = 3f;

	// Token: 0x0400072C RID: 1836
	public float citadelScale = 1f;

	// Token: 0x0400072D RID: 1837
	public float houseScale = 1f;

	// Token: 0x0400072E RID: 1838
	public float wallScale = 1f;

	// Token: 0x0400072F RID: 1839
	[Header("Wall Properties")]
	public float wallThickness = 1f;

	// Token: 0x04000730 RID: 1840
	public float wallHeight = 1f;

	// Token: 0x04000731 RID: 1841
	public float wallMaxLength = 2f;

	// Token: 0x04000732 RID: 1842
	public float towerHeight = 1f;

	// Token: 0x04000733 RID: 1843
	public float towerThickness = 1f;

	// Token: 0x04000734 RID: 1844
	public float towerScale = 1f;

	// Token: 0x04000735 RID: 1845
	public float gateHeight = 1f;

	// Token: 0x04000736 RID: 1846
	public float gateScale = 1f;

	// Token: 0x04000737 RID: 1847
	public int min_gates = 2;

	// Token: 0x04000738 RID: 1848
	public float max_dist_between_towers = 80f;

	// Token: 0x04000739 RID: 1849
	[Header("Translation")]
	public float HouseInnerDistance = 1f;

	// Token: 0x0400073A RID: 1850
	public float WallDistance = 1.4f;

	// Token: 0x0400073B RID: 1851
	public float HouseOuterDistance = 1.6f;

	// Token: 0x0400073C RID: 1852
	[Header("Army Camps - Overall")]
	public int min_army_camps = 1;

	// Token: 0x0400073D RID: 1853
	public int max_army_camps = 3;

	// Token: 0x0400073E RID: 1854
	public int min_army_camp_squad_count = 3;

	// Token: 0x0400073F RID: 1855
	public int max_army_camp_squad_count = 12;

	// Token: 0x04000740 RID: 1856
	public float min_army_camp_dist_to_coast = 50f;

	// Token: 0x04000741 RID: 1857
	public float min_camp_dist_to_city = 150f;

	// Token: 0x04000742 RID: 1858
	public float camp_dist_to_city_decrease_step = 20f;

	// Token: 0x04000743 RID: 1859
	[Header("Army Camps - Siege")]
	public float outside_camp_max_width = 50f;

	// Token: 0x04000744 RID: 1860
	public float outside_camp_max_height = 50f;

	// Token: 0x04000745 RID: 1861
	public float outside_camp_additional_range = 75f;

	// Token: 0x04000746 RID: 1862
	public List<Vector3> outside_camp_offsets = new List<Vector3>
	{
		new Vector3(266f, 0f, -266f),
		new Vector3(-266f, 0f, -266f),
		new Vector3(0f, 0f, -266f),
		new Vector3(266f, 0f, 266f),
		new Vector3(-266f, 0f, 266f),
		new Vector3(0f, 0f, 266f),
		new Vector3(-266f, 0f, 0f),
		new Vector3(266f, 0f, 0f)
	};

	// Token: 0x04000747 RID: 1863
	[Header("Army Camps - Open Field (SET FROM BattleSimulation.def IF EXISTS!)")]
	public float min_between_camps_dist = 300f;

	// Token: 0x04000748 RID: 1864
	public float max_between_camps_dist = 500f;

	// Token: 0x04000749 RID: 1865
	[Header("Growth")]
	public float min_dist_house_to_wall_inside_wall = 10f;

	// Token: 0x0400074A RID: 1866
	public float min_dist_house_to_wall_outside_wall = 5f;

	// Token: 0x0400074B RID: 1867
	public float max_dist_house_to_wall_outside_wall = 60f;

	// Token: 0x0400074C RID: 1868
	public int house_wave_count_inner = 4;

	// Token: 0x0400074D RID: 1869
	public int house_wave_count_outer = 2;

	// Token: 0x0400074E RID: 1870
	public int house_wave_count_shoreline = 2;

	// Token: 0x0400074F RID: 1871
	public float distance_between_grids;

	// Token: 0x04000750 RID: 1872
	public float distance_between_grid_cluster;

	// Token: 0x04000751 RID: 1873
	public float min_dist_to_shore;

	// Token: 0x04000752 RID: 1874
	[Range(0f, 90f)]
	public float rotation_angle = 60f;

	// Token: 0x04000753 RID: 1875
	[Range(0f, 90f)]
	public float max_slope = 3f;

	// Token: 0x04000754 RID: 1876
	public int min_unique_pgs;

	// Token: 0x04000755 RID: 1877
	public int max_unique_pgs;

	// Token: 0x04000756 RID: 1878
	public int control_point_count = 4;

	// Token: 0x04000757 RID: 1879
	public float min_dist_between_control_point = 50f;

	// Token: 0x04000758 RID: 1880
	public int min_random_houses = 3;

	// Token: 0x04000759 RID: 1881
	public int max_random_houses = 6;

	// Token: 0x0400075A RID: 1882
	public float min_dist_between_docks = 20f;

	// Token: 0x0400075B RID: 1883
	[Header("Roads")]
	public TerrainPath roadPrefab;

	// Token: 0x0400075C RID: 1884
	public TerrainPath roadBackgroundPrefab;

	// Token: 0x0400075D RID: 1885
	public TerrainPath subdivisionPrefab;

	// Token: 0x0400075E RID: 1886
	public TerrainPath waypointPrefab;

	// Token: 0x0400075F RID: 1887
	public int max_roads_per_waypoint = 3;

	// Token: 0x04000760 RID: 1888
	public string[] road_splats = new string[0];

	// Token: 0x04000761 RID: 1889
	[Tooltip("How far a road can go after exiting a gate")]
	public float road_out_gate_distance = 30f;

	// Token: 0x04000762 RID: 1890
	[Tooltip("For connected waypoints")]
	public float min_road_distance = 10f;

	// Token: 0x04000763 RID: 1891
	[Tooltip("For connected waypoints")]
	public float max_road_distance = 30f;

	// Token: 0x04000764 RID: 1892
	[Tooltip("Minimum lenght of subdivided roads")]
	public float min_road_length = 20f;

	// Token: 0x04000765 RID: 1893
	[Tooltip("Distance between houses and roads")]
	public float distance_to_roads = 4f;

	// Token: 0x04000766 RID: 1894
	[Tooltip("Min distance between wall and main road")]
	public float inner_ring_min_dist_to_walls = 15f;

	// Token: 0x04000767 RID: 1895
	[Tooltip("Scale of the noise that distorts the road")]
	public float inner_ring_road_noise_scale = 0.5f;

	// Token: 0x04000768 RID: 1896
	public float outer_ring_road_noise_scale = 2f;

	// Token: 0x04000769 RID: 1897
	public float gate_road_noise_scale = 1f;

	// Token: 0x0400076A RID: 1898
	[Tooltip("Strength of the noise that distorts the road")]
	public float inner_ring_road_noise_strength = 0.8f;

	// Token: 0x0400076B RID: 1899
	public float outer_ring_road_noise_strength = 0.1f;

	// Token: 0x0400076C RID: 1900
	public float gate_road_noise_strength = 0.8f;

	// Token: 0x0400076D RID: 1901
	[Tooltip("Average road decal segment size")]
	public float road_segment_size = 3f;

	// Token: 0x0400076E RID: 1902
	[Tooltip("How many segments should be in a single road decal")]
	public int road_max_segment_count_in_road_part = 10;

	// Token: 0x0400076F RID: 1903
	[Header("Roads for neighbourhood")]
	[Tooltip("What distance to highway road should make the dirt road to snap to it")]
	public float dirt_roads_snap_distance = 20f;

	// Token: 0x04000770 RID: 1904
	[Tooltip("How dense dirt roads should be")]
	public float dirt_roads_density = 15f;

	// Token: 0x04000771 RID: 1905
	[Tooltip("How equal the parallel dirt road distances should be. 0.0 - should vary a lot, 1.0 - shoud be almost equal")]
	public float cell_size_equality = 0.5f;

	// Token: 0x04000772 RID: 1906
	[Tooltip("Crossroads merge distance. If there are 2 different crossroads near each other, they will merge and create new crossroad. value should be in about 0.001-0.4 range")]
	public float merge_distance = 0.1f;

	// Token: 0x04000773 RID: 1907
	[Tooltip("Average length of straight road segments")]
	public float average_road_segment = 6f;

	// Token: 0x04000774 RID: 1908
	[Tooltip("Minimum dirt road distance to highway. All dirtroads that are parallel to highways will be removed if distance to highway is lower than this value")]
	public float min_road_to_highway_distance = 14f;

	// Token: 0x04000775 RID: 1909
	[Header("Trees")]
	public bool trees_on_roads;

	// Token: 0x04000776 RID: 1910
	public bool trees_on_waypoint_roads;

	// Token: 0x04000777 RID: 1911
	public bool trees_linear;

	// Token: 0x04000778 RID: 1912
	public float tree_linear_offset_min = 1f;

	// Token: 0x04000779 RID: 1913
	public float tree_linear_offset_max = 6f;

	// Token: 0x0400077A RID: 1914
	public float min_tree_scale = 0.5f;

	// Token: 0x0400077B RID: 1915
	public float max_tree_scale = 1.25f;

	// Token: 0x0400077C RID: 1916
	[Tooltip("Random distance between trees")]
	public float tree_dist_min = 1f;

	// Token: 0x0400077D RID: 1917
	[Tooltip("Random distance between trees")]
	public float tree_dist_max = 10f;

	// Token: 0x0400077E RID: 1918
	public float tree_min_radius = 3f;

	// Token: 0x0400077F RID: 1919
	public float tree_max_radius = 6f;

	// Token: 0x04000780 RID: 1920
	public int tree_min_count = 1;

	// Token: 0x04000781 RID: 1921
	public int tree_max_count = 5;

	// Token: 0x04000782 RID: 1922
	public List<string> house_types = new List<string>();

	// Token: 0x04000783 RID: 1923
	[Header("Map Bounds")]
	public Material map_bounds_material;

	// Token: 0x04000784 RID: 1924
	[HideInInspector]
	public int level;

	// Token: 0x04000785 RID: 1925
	public static int cur_generation_step;

	// Token: 0x04000786 RID: 1926
	public static bool generating = false;

	// Token: 0x04000787 RID: 1927
	public static bool finished_generation = false;

	// Token: 0x04000788 RID: 1928
	private int logic_level = 2;

	// Token: 0x04000789 RID: 1929
	private int logic_max_level;

	// Token: 0x0400078A RID: 1930
	private int wall_max_level;

	// Token: 0x0400078B RID: 1931
	private int house_level;

	// Token: 0x0400078C RID: 1932
	private int house_max_level;

	// Token: 0x0400078D RID: 1933
	private int citadel_max_level;

	// Token: 0x0400078E RID: 1934
	private RuntimePathDataBuilder pdb;

	// Token: 0x0400078F RID: 1935
	private PrefabGrid.Info house_info;

	// Token: 0x04000790 RID: 1936
	private List<PrefabGrid.Info> house_infos = new List<PrefabGrid.Info>();

	// Token: 0x04000791 RID: 1937
	private PrefabGrid.Info dock_info;

	// Token: 0x04000792 RID: 1938
	private float smallest_avg_house_size = float.MaxValue;

	// Token: 0x04000793 RID: 1939
	public static List<RoadWaypoint> waypoints = new List<RoadWaypoint>();

	// Token: 0x04000794 RID: 1940
	private Dictionary<RoadWaypoint, int> connected_waypoints = new Dictionary<RoadWaypoint, int>();

	// Token: 0x04000795 RID: 1941
	private List<SettlementBV.RoadBV> roads = new List<SettlementBV.RoadBV>();

	// Token: 0x04000796 RID: 1942
	private List<SettlementBV.RoadBV> highway_roads = new List<SettlementBV.RoadBV>();

	// Token: 0x04000797 RID: 1943
	private List<List<float2>> outside_town_connection_paths = new List<List<float2>>();

	// Token: 0x04000798 RID: 1944
	private List<SDF2D> outside_town_connectors = new List<SDF2D>();

	// Token: 0x04000799 RID: 1945
	private List<PrefabGrid.Info> capture_point_info;

	// Token: 0x0400079A RID: 1946
	private PrefabGrid.Info army_camp_info;

	// Token: 0x0400079B RID: 1947
	private List<PrefabGrid.Info> road_infos = new List<PrefabGrid.Info>();

	// Token: 0x0400079C RID: 1948
	public static SettlementBV.OnGenerationEvent OnGenerationComplete;

	// Token: 0x0400079D RID: 1949
	[NonSerialized]
	public Terrain t;

	// Token: 0x0400079E RID: 1950
	[HideInInspector]
	public int citadel_level = 4;

	// Token: 0x0400079F RID: 1951
	[HideInInspector]
	public int wall_level = 4;

	// Token: 0x040007A0 RID: 1952
	public Point wv_pos;

	// Token: 0x040007A1 RID: 1953
	private float rot;

	// Token: 0x040007A2 RID: 1954
	[HideInInspector]
	public Wall wall;

	// Token: 0x040007A3 RID: 1955
	[HideInInspector]
	public PrefabGrid citadel;

	// Token: 0x040007A4 RID: 1956
	[HideInInspector]
	public Transform roads_obj;

	// Token: 0x040007A5 RID: 1957
	[HideInInspector]
	public List<PrefabGrid> houses = new List<PrefabGrid>();

	// Token: 0x040007A6 RID: 1958
	private List<PrefabGrid> inner_houses = new List<PrefabGrid>();

	// Token: 0x040007A7 RID: 1959
	private List<PrefabGrid> outer_houses = new List<PrefabGrid>();

	// Token: 0x040007A8 RID: 1960
	private List<PrefabGrid> shore_line_pgs = new List<PrefabGrid>();

	// Token: 0x040007A9 RID: 1961
	private List<PrefabGrid> spawned_outer_houses = new List<PrefabGrid>();

	// Token: 0x040007AA RID: 1962
	private Transform house_container;

	// Token: 0x040007AB RID: 1963
	private Transform unique_container;

	// Token: 0x040007AC RID: 1964
	[HideInInspector]
	public List<PrefabGrid> north_camps = new List<PrefabGrid>();

	// Token: 0x040007AD RID: 1965
	[HideInInspector]
	public List<PrefabGrid> south_camps = new List<PrefabGrid>();

	// Token: 0x040007AE RID: 1966
	[HideInInspector]
	public float break_siege_position_x;

	// Token: 0x040007AF RID: 1967
	[HideInInspector]
	public float break_siege_position_y;

	// Token: 0x040007B0 RID: 1968
	[HideInInspector]
	public SettlementBV.ReinfPoint[] north_reinforcement_points;

	// Token: 0x040007B1 RID: 1969
	[HideInInspector]
	public SettlementBV.ReinfPoint[] south_reinforcement_points;

	// Token: 0x040007B2 RID: 1970
	[HideInInspector]
	public MapBorderMesh map_bounds;

	// Token: 0x040007B3 RID: 1971
	[NonSerialized]
	public string walls_architecture;

	// Token: 0x040007B4 RID: 1972
	[NonSerialized]
	public string towers_architecture;

	// Token: 0x040007B5 RID: 1973
	[NonSerialized]
	public string gates_architecture;

	// Token: 0x040007B6 RID: 1974
	[NonSerialized]
	public string citadel_architecture;

	// Token: 0x040007B7 RID: 1975
	[NonSerialized]
	public string houses_architecture;

	// Token: 0x040007B8 RID: 1976
	public TerrainData src_terrain;

	// Token: 0x040007B9 RID: 1977
	[HideInInspector]
	public List<Point> wall_corners = new List<Point>();

	// Token: 0x040007BA RID: 1978
	[NonSerialized]
	public List<PrefabGrid> spawned_grids = new List<PrefabGrid>();

	// Token: 0x040007BB RID: 1979
	private Dictionary<PrefabGrid, PrefabGrid> clusters = new Dictionary<PrefabGrid, PrefabGrid>();

	// Token: 0x040007BC RID: 1980
	private Point camp_position_south;

	// Token: 0x040007BD RID: 1981
	private Point camp_position_north;

	// Token: 0x040007BE RID: 1982
	private static List<ValueTuple<string, SettlementBV.GenerateFunc, string>> generationFunctions = new List<ValueTuple<string, SettlementBV.GenerateFunc, string>>();

	// Token: 0x040007BF RID: 1983
	private List<SettlementBV.RoadBV> outer_roads = new List<SettlementBV.RoadBV>();

	// Token: 0x040007C0 RID: 1984
	private DirtRoadGraph debug_dirt_road_graph;

	// Token: 0x040007C1 RID: 1985
	public SDF2D debug_sdf_2D;

	// Token: 0x040007C2 RID: 1986
	private WeightedRandom<PrefabGrid.Info> available_info = new WeightedRandom<PrefabGrid.Info>(5, -1);

	// Token: 0x020005AC RID: 1452
	// (Invoke) Token: 0x06004488 RID: 17544
	public delegate void OnGenerationEvent();

	// Token: 0x020005AD RID: 1453
	[Serializable]
	public struct ReinfPoint
	{
		// Token: 0x0400313A RID: 12602
		public Point pos;

		// Token: 0x0400313B RID: 12603
		public float dist_to_enemy_camp;

		// Token: 0x0400313C RID: 12604
		public float dist_to_my_camp;
	}

	// Token: 0x020005AE RID: 1454
	[Serializable]
	public class RoadBV
	{
		// Token: 0x0400313D RID: 12605
		public int depth;

		// Token: 0x0400313E RID: 12606
		public TerrainPath path;

		// Token: 0x0400313F RID: 12607
		public float pt_len;

		// Token: 0x04003140 RID: 12608
		public Vector3 normal;

		// Token: 0x04003141 RID: 12609
		public SettlementBV.RoadBV.Type type;

		// Token: 0x020009D9 RID: 2521
		public enum Type
		{
			// Token: 0x0400457B RID: 17787
			Highway,
			// Token: 0x0400457C RID: 17788
			InnerRing,
			// Token: 0x0400457D RID: 17789
			Subdivision,
			// Token: 0x0400457E RID: 17790
			Waypoints
		}
	}

	// Token: 0x020005AF RID: 1455
	// (Invoke) Token: 0x0600448D RID: 17549
	private delegate void GenerateFunc();

	// Token: 0x020005B0 RID: 1456
	private struct Triangle
	{
		// Token: 0x04003142 RID: 12610
		public Point[] points;

		// Token: 0x04003143 RID: 12611
		public float area;
	}

	// Token: 0x020005B1 RID: 1457
	public struct PGRect
	{
		// Token: 0x06004490 RID: 17552 RVA: 0x00202344 File Offset: 0x00200544
		public static SettlementBV.PGRect GetRotated(Point pos, float angle, float width, float height)
		{
			SettlementBV.PGRect pgrect = new SettlementBV.PGRect
			{
				Points = new Point[4]
			};
			pgrect.Points[0] = new Point(-width, -height).GetRotated(angle) + pos;
			pgrect.Points[1] = new Point(width, -height).GetRotated(angle) + pos;
			pgrect.Points[2] = new Point(width, height).GetRotated(angle) + pos;
			pgrect.Points[3] = new Point(-width, height).GetRotated(angle) + pos;
			return pgrect;
		}

		// Token: 0x04003144 RID: 12612
		public Point[] Points;
	}
}
