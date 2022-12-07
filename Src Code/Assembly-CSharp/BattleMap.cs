using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BSG.MapMaker;
using BSG.MapMaker.Nodes;
using FMODUnity;
using Logic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using XNode;

// Token: 0x020000BE RID: 190
public class BattleMap : MapData, IListener
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000819 RID: 2073 RVA: 0x00055A2C File Offset: 0x00053C2C
	// (set) Token: 0x0600081A RID: 2074 RVA: 0x00055A33 File Offset: 0x00053C33
	public static Logic.Battle battle { get; private set; }

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x0600081B RID: 2075 RVA: 0x00055A3B File Offset: 0x00053C3B
	// (set) Token: 0x0600081C RID: 2076 RVA: 0x00055A42 File Offset: 0x00053C42
	public static int KingdomId { get; private set; }

	// Token: 0x0600081D RID: 2077 RVA: 0x00055A4A File Offset: 0x00053C4A
	public new static BattleMap Get()
	{
		return BattleMap.instance;
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00055A54 File Offset: 0x00053C54
	public void AssignColors()
	{
		this.kingdom_colors = new Dictionary<int, Color>();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		List<Color> colors = new List<Color>(this.unit_colors);
		global::Kingdom kingdom2 = kingdom.visuals as global::Kingdom;
		Color color;
		if (kingdom2 != null)
		{
			color = kingdom2.PrimaryArmyColor;
		}
		else
		{
			color = global::Defs.GetColor(kingdom.def, "primary_color", null);
		}
		this.SetKingdomColor(kingdom.id, global::Kingdom.PickClosestColor(this.unit_colors, color), colors);
		Logic.Kingdom kingdom3;
		Logic.Kingdom kingdom4;
		Logic.Kingdom kingdom5;
		if (global::Battle.PlayerIsAttacker(BattleMap.battle, true))
		{
			kingdom3 = BattleMap.battle.defender_kingdom;
			Logic.Army defender_support = BattleMap.battle.defender_support;
			kingdom4 = ((defender_support != null) ? defender_support.GetKingdom() : null);
			Logic.Army attacker_support = BattleMap.battle.attacker_support;
			kingdom5 = ((attacker_support != null) ? attacker_support.GetKingdom() : null);
		}
		else
		{
			kingdom3 = BattleMap.battle.attacker_kingdom;
			Logic.Army attacker_support2 = BattleMap.battle.attacker_support;
			kingdom4 = ((attacker_support2 != null) ? attacker_support2.GetKingdom() : null);
			Logic.Army defender_support2 = BattleMap.battle.defender_support;
			kingdom5 = ((defender_support2 != null) ? defender_support2.GetKingdom() : null);
		}
		kingdom2 = (kingdom3.visuals as global::Kingdom);
		if (kingdom2 != null)
		{
			color = kingdom2.PrimaryArmyColor;
		}
		else
		{
			color = global::Defs.GetColor(kingdom3.def, "primary_color", null);
		}
		this.SetKingdomColor(kingdom3.id, global::Kingdom.PickMidColor(colors, color, new Color[]
		{
			this.kingdom_colors[kingdom.id]
		}), colors);
		if (kingdom4 != null)
		{
			this.SetKingdomColor(kingdom4.id, global::Kingdom.PickMidColor(colors, this.kingdom_colors[kingdom3.id], new Color[]
			{
				this.kingdom_colors[kingdom.id]
			}), colors);
		}
		if (kingdom5 != null)
		{
			kingdom2 = (kingdom5.visuals as global::Kingdom);
			if (kingdom2 != null)
			{
				color = kingdom2.PrimaryArmyColor;
			}
			else
			{
				color = global::Defs.GetColor(kingdom5.def, "primary_color", null);
			}
			this.SetKingdomColor(kingdom5.id, global::Kingdom.PickMidColor(colors, color, new Color[]
			{
				this.kingdom_colors[kingdom.id],
				this.kingdom_colors[kingdom3.id]
			}), colors);
		}
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00055C74 File Offset: 0x00053E74
	private void SetKingdomColor(int id, Color col, List<Color> colors)
	{
		if (this.kingdom_colors.ContainsKey(id))
		{
			return;
		}
		this.kingdom_colors[id] = col;
		colors.Remove(col);
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000820 RID: 2080 RVA: 0x00055C9A File Offset: 0x00053E9A
	public static int BattleSide
	{
		get
		{
			if (BattleMap.battle == null)
			{
				return -1;
			}
			if (BattleMap.KingdomId == BattleMap.battle.attacker_kingdom.id)
			{
				return 0;
			}
			if (BattleMap.KingdomId == BattleMap.battle.defender_kingdom.id)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00055CD6 File Offset: 0x00053ED6
	public static void SetBattle(Logic.Battle battle, int kingdom_id)
	{
		BattleMap.battle = battle;
		BattleMap.KingdomId = kingdom_id;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00055CE4 File Offset: 0x00053EE4
	public void LoadSDF()
	{
		this.LoadWallSDF();
		this.LoadShoreSDF();
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00055CF4 File Offset: 0x00053EF4
	public void LoadWallSDF()
	{
		MakeMap component = base.GetComponent<MakeMap>();
		NativeArray<float2> nativeArray = this.wall_segs;
		if (this.wall_segs.Length == 0)
		{
			this.wall_sdf = new Texture2D(1, 1);
			this.wall_sdf.SetPixel(0, 0, Color.red);
			this.wall_sdf.Apply();
		}
		else
		{
			bool flag;
			if (component == null)
			{
				flag = (null != null);
			}
			else
			{
				MapMakerGraph graph = component.graph;
				flag = (((graph != null) ? graph.generated_textures : null) != null);
			}
			if ((!flag || !component.graph.generated_textures.TryGetValue("wall_sdf", out this.wall_sdf)) && this.wall_sdf == null)
			{
				this.wall_sdf = new Texture2D(1, 1);
				this.wall_sdf.SetPixel(0, 0, Color.red);
				this.wall_sdf.Apply();
			}
		}
		bool flag2;
		if (component == null)
		{
			flag2 = (null != null);
		}
		else
		{
			MapMakerGraph graph2 = component.graph;
			flag2 = (((graph2 != null) ? graph2.generated_floats : null) != null);
		}
		if (!flag2 || !component.graph.generated_floats.TryGetValue("wall_sdf_max_circle", out this.wall_sdf_max_circle))
		{
			this.wall_sdf_max_circle = 30f;
		}
		Shader.SetGlobalTexture("_wall_sdf_tex", this.wall_sdf);
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00055E0C File Offset: 0x0005400C
	public void LoadShoreSDF()
	{
		MakeMap component = base.GetComponent<MakeMap>();
		bool flag;
		if (component == null)
		{
			flag = (null != null);
		}
		else
		{
			MapMakerGraph graph = component.graph;
			flag = (((graph != null) ? graph.generated_textures : null) != null);
		}
		if ((!flag || !component.graph.generated_textures.TryGetValue("shore_sdf", out this.shore_sdf)) && this.shore_sdf == null)
		{
			this.shore_sdf = new Texture2D(1, 1);
			this.shore_sdf.SetPixel(0, 0, Color.red);
			this.shore_sdf.Apply();
		}
		bool flag2;
		if (component == null)
		{
			flag2 = (null != null);
		}
		else
		{
			MapMakerGraph graph2 = component.graph;
			flag2 = (((graph2 != null) ? graph2.generated_floats : null) != null);
		}
		if (!flag2 || !component.graph.generated_floats.TryGetValue("shore_sdf_max_circle", out this.shore_sdf_max_circle))
		{
			this.shore_sdf_max_circle = 30f;
		}
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00055ED4 File Offset: 0x000540D4
	public Color GetWallSDFVal(Vector3 pos)
	{
		if (this.wall_sdf == null)
		{
			return Color.white;
		}
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		pos.x /= terrainData.size.x;
		pos.z /= terrainData.size.z;
		return this.wall_sdf.GetPixelBilinear(pos.x, pos.z);
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x00055F49 File Offset: 0x00054149
	public bool IsInsideWall(Color c)
	{
		return c.g > c.r;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00055F5C File Offset: 0x0005415C
	public bool IsInsideWall(Vector3 pos)
	{
		Color wallSDFVal = this.GetWallSDFVal(pos);
		return this.IsInsideWall(wallSDFVal);
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00055F78 File Offset: 0x00054178
	public float GetDistToWall(Color c)
	{
		if (c.r > c.g)
		{
			return c.r * this.wall_sdf_max_circle;
		}
		return -c.g * this.wall_sdf_max_circle;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00055FA4 File Offset: 0x000541A4
	public float GetDistToWall(Vector3 pos)
	{
		Color wallSDFVal = this.GetWallSDFVal(pos);
		return this.GetDistToWall(wallSDFVal);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x00055FC0 File Offset: 0x000541C0
	public float GetDistToShore(Vector3 pos)
	{
		if (this.wall_sdf == null)
		{
			return 0f;
		}
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		pos.x /= terrainData.size.x;
		pos.z /= terrainData.size.z;
		Color pixelBilinear = this.shore_sdf.GetPixelBilinear(pos.x, pos.z);
		if (pixelBilinear.r > pixelBilinear.g)
		{
			return pixelBilinear.r * this.shore_sdf_max_circle;
		}
		return -pixelBilinear.g * this.shore_sdf_max_circle;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00056060 File Offset: 0x00054260
	private IEnumerator EnemyHidingBehindWallsVoiceCoroutine()
	{
		yield return new WaitForSecondsRealtime(3f);
		if (global::Battle.PlayerBattleSide() == 0)
		{
			Logic.Army army = BattleMap.battle.GetArmy(0);
			BaseUI.PlayCharacterlessVoiceEvent((army != null) ? army.leader : null, "battle_enemy_hiding_behind_walls");
		}
		yield break;
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00056068 File Offset: 0x00054268
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1395975622U)
		{
			if (num <= 793046412U)
			{
				if (num != 97277286U)
				{
					if (num != 793046412U)
					{
						return;
					}
					if (!(message == "Restart"))
					{
						return;
					}
					Troops.OnRestart();
					BackgroundMusic.Reset();
					BattleViewLoader.InitBattleLoad(BattleMap.battle, BattleViewLoader.currentBatte.KingdomId, BattleViewLoader.currentBatte.Side, BattleViewLoader.currentBatte.MapName);
					this.ClearSkirmishes();
					return;
				}
				else
				{
					if (!(message == "initiative_lost"))
					{
						return;
					}
					if (BattleMap.battle.initiative_side == global::Battle.PlayerBattleSide())
					{
						if (BattleMap.battle.type == Logic.Battle.Type.BreakSiege)
						{
							if (BattleMap.battle.defenders.Count == 0 || BattleMap.battle.defenders[0].castle == BattleMap.battle.settlement)
							{
								BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_our_break_siege_inside", null, true, true, true, '.'), null);
								return;
							}
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_our_break_siege_outside", null, true, true, true, '.'), null);
							return;
						}
						else if (BattleMap.battle.type == Logic.Battle.Type.Assault)
						{
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_our_assault", null, true, true, true, '.'), null);
							return;
						}
					}
					else if (BattleMap.battle.type == Logic.Battle.Type.BreakSiege)
					{
						if (BattleMap.battle.defenders.Count == 0 || BattleMap.battle.defenders[0].castle == BattleMap.battle.settlement)
						{
							BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_enemy_break_siege_inside", null, true, true, true, '.'), null);
							return;
						}
						BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_enemy_break_siege_outside", null, true, true, true, '.'), null);
						return;
					}
					else if (BattleMap.battle.type == Logic.Battle.Type.Assault)
					{
						BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("lost_initiative_their_assault", null, true, true, true, '.'), null);
					}
					return;
				}
			}
			else if (num != 1199480501U)
			{
				if (num != 1395975622U)
				{
					return;
				}
				if (!(message == "losing_initiative"))
				{
					return;
				}
				if (BattleMap.battle.initiative_side == global::Battle.PlayerBattleSide() && BattleMap.battle.stage == Logic.Battle.Stage.Ongoing && BattleMap.battle.initiative.Get() < BattleMap.battle.initiative.GetMax())
				{
					BaseUI.PlayVoiceEvent(BaseUI.soundsDef.FindChild("losing_initiative", null, true, true, true, '.'), null);
				}
				return;
			}
			else
			{
				if (!(message == "finished_enter_battle"))
				{
					return;
				}
				foreach (KeyValuePair<int, BaseUI.ControlGroup> keyValuePair in BattleViewUI.Get().groups)
				{
					BaseUI.ControlGroup value = keyValuePair.Value;
					List<Logic.Squad> list = (value != null) ? value.squads : null;
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							Logic.Squad squad = list[i];
							if (squad != null && !squad.IsValid())
							{
								List<Logic.Squad> list2 = BattleMap.battle.squads.Get(squad.battle_side);
								for (int j = 0; j < list2.Count; j++)
								{
									Logic.Squad squad2 = list2[j];
									if (squad2.simulation.main_squad == squad.simulation.main_squad)
									{
										list[i] = squad2;
										break;
									}
								}
							}
						}
					}
				}
				this.SetVisibleSquadsFighting(false);
				return;
			}
		}
		else if (num <= 2168251663U)
		{
			if (num != 1654031222U)
			{
				if (num != 2168251663U)
				{
					return;
				}
				if (!(message == "leave_battle"))
				{
					return;
				}
				int num2 = (int)param;
				WorldUI worldUI = WorldUI.Get();
				if (worldUI == null || worldUI.kingdom != num2)
				{
					return;
				}
				BattleViewLoader.ExitBatte(BattleMap.battle);
				foreach (KeyValuePair<int, BaseUI.ControlGroup> keyValuePair2 in BattleViewUI.Get().groups)
				{
					BaseUI.ControlGroup value2 = keyValuePair2.Value;
					List<Logic.Squad> list3 = (value2 != null) ? value2.squads : null;
					if (list3 != null)
					{
						list3.Clear();
					}
				}
				return;
			}
			else
			{
				if (!(message == "squad_ladder_placed"))
				{
					return;
				}
				int num3 = global::Battle.PlayerBattleSide();
				if (num3 == 0 && (int)param == 0)
				{
					Logic.Army army = BattleMap.battle.GetArmy(0);
					BaseUI.PlayCharacterlessVoiceEvent((army != null) ? army.leader : null, "battle_our_ladders_placed");
					return;
				}
				if (num3 == 1 && (int)param == 0)
				{
					Logic.Army army2 = BattleMap.battle.GetArmy(1);
					BaseUI.PlayCharacterlessVoiceEvent((army2 != null) ? army2.leader : null, "battle_enemy_ladders_placed");
				}
				return;
			}
		}
		else if (num != 2313807619U)
		{
			if (num != 3326472242U)
			{
				if (num != 4293422616U)
				{
					return;
				}
				if (!(message == "squad_exit_city"))
				{
					return;
				}
				if (global::Battle.PlayerBattleSide() == 0 && (int)param == 1)
				{
					Logic.Army army3 = BattleMap.battle.GetArmy(0);
					BaseUI.PlayCharacterlessVoiceEvent((army3 != null) ? army3.leader : null, "battle_enemy_squads_leaving_city");
				}
				return;
			}
			else
			{
				if (!(message == "stage_changed"))
				{
					return;
				}
				if (BattleMap.battle.stage == Logic.Battle.Stage.Ongoing && BattleMap.battle.type == Logic.Battle.Type.Assault && !Troops.NotReady && Troops.Initted && SettlementBV.finished_generation)
				{
					base.StartCoroutine(this.EnemyHidingBehindWallsVoiceCoroutine());
				}
				return;
			}
		}
		else
		{
			if (!(message == "armies_changed"))
			{
				return;
			}
			Logic.Army army4 = param as Logic.Army;
			if (army4 != null)
			{
				if (army4.battle_side == 0)
				{
					this.attacker_reinforcement_position = this.CalcReinfPos(0);
				}
				else
				{
					this.defender_reinforcement_position = this.CalcReinfPos(1);
				}
			}
			this.position_squads = true;
			return;
		}
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x00056608 File Offset: 0x00054808
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x00056610 File Offset: 0x00054810
	public void Init()
	{
		if (BattleMap.instance == null)
		{
			BattleMap.instance = this;
			if (this.pf == null)
			{
				this.pf = base.GetComponent<global::PathFinding>();
			}
			if (this.pf == null)
			{
				this.pf = global::PathFinding.Get(false);
			}
			if (this.pdb == null)
			{
				this.pdb = new RuntimePathDataBuilder(this.pf);
			}
			if (!Application.isPlaying)
			{
				PrefabGrid.Info.Clear();
			}
			return;
		}
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0005668C File Offset: 0x0005488C
	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.ConnectPFLogic();
		Scene activeScene = SceneManager.GetActiveScene();
		this.battlePhysics = activeScene.GetPhysicsScene();
		if (BattleMap.battle != null && BattleMap.battle.simulation != null)
		{
			BattleMap.battle.simulation.RestartTotals();
		}
		this.GenerateMapOnStart();
		GameObject gameObject = global::Common.FindChildByName(GameLogic.instance.gameObject, "Ragdolls", true, true);
		if (gameObject != null)
		{
			global::Common.DestroyChildren(gameObject);
		}
		this.SetVisibleSquadsFighting(false);
		CameraPath.LoadAll(null);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00056718 File Offset: 0x00054918
	public void OnGenerationComplete()
	{
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.OnGenerationComplete));
		TreesBatching treesBatching = UnityEngine.Object.FindObjectOfType<TreesBatching>();
		if (treesBatching != null)
		{
			treesBatching.do_rebuild = true;
		}
		this.GenerateSDF();
		if (!Application.isPlaying)
		{
			return;
		}
		if (BattleMap.battle != null)
		{
			BattleMap.battle.SetTreeData(this.pdb.tree_count_grid, 20, this.pdb.tree_count_grid_width, this.pdb.tree_count_grid_height);
			SettlementBV settlementBV = this.sbv;
			if (((settlementBV != null) ? settlementBV.citadel : null) != null)
			{
				BattleMap.battle.citadel_position = new PPos(this.sbv.citadel.transform.position.x, this.sbv.citadel.transform.position.z, 0);
			}
			if (this.sbv.wall != null)
			{
				BattleMap.battle.wall_corners = this.sbv.wall_corners;
				BattleMap.battle.wall_center = this.sbv.transform.position;
			}
		}
		SettlementBV settlementBV2 = this.sbv;
		if (((settlementBV2 != null) ? settlementBV2.wall : null) != null && this.sbv.wall.gameObject.activeInHierarchy)
		{
			for (int i = 0; i < this.sbv.wall.corners.Count; i++)
			{
				WallCorner wallCorner = this.sbv.wall.corners[i];
				if (wallCorner.type == WallCorner.Type.Gate)
				{
					global::PassableGate componentInChildren = wallCorner.GetComponentInChildren<global::PassableGate>();
					if (!(componentInChildren == null))
					{
						componentInChildren.CreateLogic(BattleMap.battle);
						GameObject gameObject = global::Common.FindChildByName(wallCorner.gameObject, "_GateObj", true, true);
						if (gameObject != null)
						{
							componentInChildren.open_go = gameObject.GetComponent<Animator>();
						}
						componentInChildren.logic.Open = false;
						foreach (global::Fortification fortification in wallCorner.GetComponentsInChildren<global::Fortification>())
						{
							if (fortification != null)
							{
								fortification.CreateLogic();
								if (fortification.logic.def.type == Logic.Fortification.Type.Gate)
								{
									fortification.logic.gate = componentInChildren.logic;
									componentInChildren.logic.fortifications.Add(fortification.logic);
								}
							}
						}
						Logic.Fortification fortification2 = componentInChildren.logic.MainFortification();
						if (fortification2 != null)
						{
							fortification2.Reset();
						}
					}
				}
			}
		}
		this.pdb.LoadPathfinding();
		this.pdb.GenerateHighPFDataStep();
		if (this.sbv != null)
		{
			this.AssignCampSides();
			if (BattleMap.battle.assault_gate_action_succeeded && this.attacker_camps.Count > 0)
			{
				Point pt = this.attacker_camps[0].transform.position;
				Logic.PassableGate closest_gate_to_camps = null;
				float num = float.MaxValue;
				for (int k = 0; k < BattleMap.battle.gates.Count; k++)
				{
					Logic.PassableGate passableGate = BattleMap.battle.gates[k];
					float num2 = passableGate.position.SqrDist(pt);
					if (num2 < num)
					{
						num = num2;
						closest_gate_to_camps = passableGate;
					}
				}
				BattleMap.battle.closest_gate_to_camps = closest_gate_to_camps;
				BattleMap.battle.OpenAssaultedGate();
			}
		}
		BattleMap.battle.reinforcements_at_start_of_battleview = new Logic.Battle.Reinforcement[BattleMap.battle.reinforcements.Length];
		for (int l = 0; l < BattleMap.battle.reinforcements.Length; l++)
		{
			Logic.Battle.Reinforcement reinforcement = BattleMap.battle.reinforcements[l];
			if (reinforcement.army != null)
			{
				reinforcement.estimate_time = BattleMap.battle.CalcReinforcementTime(reinforcement.army);
			}
			BattleMap.battle.reinforcements_at_start_of_battleview[l] = new Logic.Battle.Reinforcement(reinforcement);
		}
		BattleViewAudioRenderer battleViewAudioRenderer = UnityEngine.Object.FindObjectOfType<BattleViewAudioRenderer>();
		if (battleViewAudioRenderer != null)
		{
			battleViewAudioRenderer.ExtractData();
		}
		for (int m = 0; m < 2; m++)
		{
			List<Logic.Squad> list = BattleMap.battle.squads.Get(m);
			for (int n = 0; n < list.Count; n++)
			{
				list[n].NotifyListeners("bv_loaded", null);
			}
		}
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x00056B60 File Offset: 0x00054D60
	public void ConnectPFLogic()
	{
		Logic.Battle battle = BattleMap.battle;
		Game game = (battle != null) ? battle.batte_view_game : null;
		if (this.pf != null)
		{
			this.pf.logic = ((game != null) ? game.path_finding : null);
		}
		base.InitPAManager(this.pf.logic);
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00056BB6 File Offset: 0x00054DB6
	public void ReduceTerrainLayers()
	{
		SettlementBV settlementBV = this.sbv;
		new RuntimeFixTexturesTool((settlementBV != null) ? settlementBV.t : null).Run();
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00056BD4 File Offset: 0x00054DD4
	public void GenerateMapOnStart()
	{
		if (base.GetComponent<MakeMap>() != null && BattleMap.battle != null)
		{
			if (!BattleMap.battle.battle_map_only)
			{
				this.GenerateMap();
				return;
			}
			this.pdb.Build(true, true, false, false, true);
			SettlementBV.finished_generation = true;
			this.OnGenerationComplete();
		}
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x00056C28 File Offset: 0x00054E28
	public void PositionSquads()
	{
		using (Game.Profile("BattleMap.PositionSquads", false, 0f, null))
		{
			this.position_squads = false;
			if (this.sbv != null)
			{
				if (!this.reinforcement_positions_initialized)
				{
					this.attacker_reinforcement_position = this.CalcReinfPos(0);
					this.defender_reinforcement_position = this.CalcReinfPos(1);
					this.reinforcement_positions_initialized = true;
				}
				this.PositionAttackers();
				this.PositionDefenders();
			}
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00056CB4 File Offset: 0x00054EB4
	private void PositionMobileSiege(Point army_center, Point enemy_center, int battle_side, bool inside_walls, Logic.Army army, bool garrison = false)
	{
		List<Logic.Squad> list = BattleMap.battle.squads.Get(battle_side);
		float num = float.MaxValue;
		Point point = Point.Zero;
		int i = 0;
		while (i < list.Count)
		{
			Logic.Squad squad = list[i];
			if (garrison)
			{
				goto IL_48;
			}
			BattleSimulation.Squad simulation = squad.simulation;
			if (((simulation != null) ? simulation.garrison : null) == null)
			{
				goto IL_48;
			}
			IL_AD:
			i++;
			continue;
			IL_48:
			BattleSimulation.Squad simulation2 = squad.simulation;
			if ((((simulation2 != null) ? simulation2.army : null) != null && squad.simulation.army != army) || !squad.position_set || squad.def.move_speed <= 0f)
			{
				goto IL_AD;
			}
			float num2 = squad.position.SqrDist(enemy_center);
			if (num2 < num)
			{
				num = num2;
				point = squad.position;
				goto IL_AD;
			}
			goto IL_AD;
		}
		if (num == 3.4028235E+38f)
		{
			point = army_center;
		}
		int num3 = 20;
		int max_size_y = 60;
		Point center = point + (enemy_center - point).GetNormalized() * (float)(num3 + 8);
		List<Logic.Fortification> closeEnemyTowers = this.GetCloseEnemyTowers(battle_side, point);
		this.PositionSquads(list, center, enemy_center, inside_walls, true, num3, max_size_y, army, closeEnemyTowers, null, null, garrison, false);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x00056DD8 File Offset: 0x00054FD8
	private List<Logic.Fortification> GetCloseEnemyTowers(int battle_side, Point closest_pos)
	{
		float num = 3f;
		List<Logic.Fortification> list = new List<Logic.Fortification>();
		if (BattleMap.battle.fortifications != null)
		{
			for (int i = 0; i < BattleMap.battle.fortifications.Count; i++)
			{
				Logic.Fortification fortification = BattleMap.battle.fortifications[i];
				if ((fortification.def.type == Logic.Fortification.Type.Tower || fortification.def.type == Logic.Fortification.Type.Gate) && !fortification.IsDefeated() && fortification.battle_side != battle_side && fortification.shoot_comp != null && closest_pos.Dist(fortification.position) <= fortification.shoot_comp.salvo_def.max_shoot_range * num)
				{
					list.Add(fortification);
				}
			}
		}
		return list;
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x00056E94 File Offset: 0x00055094
	public Point CalcReinfPos(int reinf_side)
	{
		SettlementBV.ReinfPoint[] array;
		if (reinf_side == 0)
		{
			array = this.attacker_reinforcement_points;
		}
		else
		{
			array = this.defender_reinforcement_points;
		}
		WeightedRandom<Point> weightedRandom = new WeightedRandom<Point>(array.Length, -1);
		float num = this.sbv.min_between_camps_dist * this.sbv.min_between_camps_dist;
		foreach (SettlementBV.ReinfPoint reinfPoint in array)
		{
			Point pos = reinfPoint.pos;
			float weight = reinfPoint.dist_to_enemy_camp + reinfPoint.dist_to_my_camp;
			bool flag = true;
			for (int j = 0; j <= 1; j++)
			{
				List<Logic.Squad> list = BattleMap.battle.squads.Get(j);
				for (int k = 0; k < list.Count; k++)
				{
					Logic.Squad squad = list[k];
					if (squad.position_set && squad.position.SqrDist(pos) < num)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			flag &= this.IsPassableWithLSA(pos, 0f);
			if (flag)
			{
				weightedRandom.AddOption(pos, weight);
			}
		}
		if (weightedRandom.options.Count == 0)
		{
			foreach (SettlementBV.ReinfPoint reinfPoint2 in array)
			{
				Point pos2 = reinfPoint2.pos;
				float weight2 = reinfPoint2.dist_to_enemy_camp + reinfPoint2.dist_to_my_camp;
				if (this.IsPassableWithLSA(pos2, 0f))
				{
					weightedRandom.AddOption(pos2, weight2);
				}
			}
		}
		if (weightedRandom.options.Count >= 2)
		{
			weightedRandom.options.Sort((WeightedRandom<Point>.Option x, WeightedRandom<Point>.Option y) => y.weight.CompareTo(x.weight));
			int count = weightedRandom.options.Count;
			for (int m = count - 1; m >= count / 2; m--)
			{
				weightedRandom.DelOptionAtIndex(m);
			}
		}
		return weightedRandom.Choose(default(Point), false);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00057074 File Offset: 0x00055274
	private void AssignCampSides()
	{
		int num = int.MinValue;
		foreach (KeyValuePair<byte, int> keyValuePair in BattleMap.battle.batte_view_game.path_finding.data.highPFLSACount)
		{
			if (keyValuePair.Value > num)
			{
				num = keyValuePair.Value;
				this.main_lsa = keyValuePair.Key;
			}
		}
		bool flag = false;
		for (int i = 0; i < BattleMap.battle.attackers.Count; i++)
		{
			if (BattleMap.battle.attackers[i].GetKingdom().is_player)
			{
				flag = true;
				break;
			}
		}
		if (flag || this.sbv.north_camps == null || this.sbv.north_camps.Count == 0)
		{
			this.attacker_camps = this.sbv.south_camps;
			this.defender_camps = this.sbv.north_camps;
			this.attacker_reinforcement_points = this.sbv.south_reinforcement_points;
			this.defender_reinforcement_points = this.sbv.north_reinforcement_points;
		}
		else
		{
			this.attacker_camps = this.sbv.north_camps;
			this.defender_camps = this.sbv.south_camps;
			this.attacker_reinforcement_points = this.sbv.north_reinforcement_points;
			this.defender_reinforcement_points = this.sbv.south_reinforcement_points;
		}
		if (this.attacker_camps != null)
		{
			for (int j = 0; j < this.attacker_camps.Count; j++)
			{
				PrefabGrid prefabGrid = this.attacker_camps[j];
				if (!(prefabGrid == null))
				{
					global::CapturePoint componentInChildren = prefabGrid.GetComponentInChildren<global::CapturePoint>();
					if (!(componentInChildren == null))
					{
						componentInChildren.battle_side = 0;
					}
				}
			}
		}
		if (this.defender_camps != null)
		{
			for (int k = 0; k < this.defender_camps.Count; k++)
			{
				PrefabGrid prefabGrid2 = this.defender_camps[k];
				if (!(prefabGrid2 == null))
				{
					global::CapturePoint componentInChildren2 = prefabGrid2.GetComponentInChildren<global::CapturePoint>();
					if (!(componentInChildren2 == null))
					{
						componentInChildren2.battle_side = 1;
					}
				}
			}
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x00057294 File Offset: 0x00055494
	private bool IsPassableWithLSA(PPos pos, float r = 0f)
	{
		if (!this.pf.logic.data.IsPassable(pos, r))
		{
			return false;
		}
		Coord coord = this.pf.logic.WorldToGridCoord(pos);
		if (!this.pf.logic.data.HighCoordsInBounds(coord.x, coord.y))
		{
			return false;
		}
		Point v = this.pf.logic.GridCoordToWorld(coord, true);
		return this.pf.logic.data.GetNode(v).lsa == this.main_lsa;
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00057334 File Offset: 0x00055534
	private void PositionSquads(List<Logic.Squad> squads, Point center, Point enemy_center, bool inside_wall, bool siege_equipment, int max_size_x = 50, int max_size_y = 50, Logic.Army army = null, List<Logic.Fortification> fortifications = null, Logic.Army supporter_army = null, StandardArmyFormation formation = null, bool garrison = false, bool using_break_siege_pos = false)
	{
		global::PathFinding pathFinding = this.pf;
		bool flag;
		if (pathFinding == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.PathFinding logic = pathFinding.logic;
			flag = (((logic != null) ? logic.data : null) != null);
		}
		if (!flag)
		{
			return;
		}
		List<Logic.Squad> list = new List<Logic.Squad>();
		foreach (Logic.Squad squad in squads)
		{
			if (squad.def.type == Logic.Unit.Type.InventoryItem == siege_equipment && !squad.position_set && (garrison || squad.simulation.garrison == null) && (army == null || squad.simulation.army == army || (supporter_army != null && squad.simulation.army == supporter_army) || (garrison && squad.simulation.garrison != null)))
			{
				list.Add(squad);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		if (!inside_wall && !siege_equipment)
		{
			Dictionary<Logic.Squad, PPos> dictionary = new Dictionary<Logic.Squad, PPos>();
			Point normalized = (enemy_center - center).GetNormalized();
			float num = normalized.Heading();
			Point point = center;
			if (!BattleMap.battle.IsReinforcement(army) && !using_break_siege_pos)
			{
				point += BattleMap.battle.simulation.def.army_offset_from_camp * normalized;
			}
			formation.CreateFormationWithWallCheck(point, num, list, out dictionary, new Func<PPos, float, bool>(this.IsPassableWithLSA), new Func<Vector3, bool>(this.IsInsideWall), inside_wall, false);
			foreach (Logic.Squad squad2 in dictionary.Keys)
			{
				squad2.direction = normalized;
				Point v = point + dictionary[squad2].GetRotated(-num);
				squad2.Teleport(v);
				squad2.SetCommand(Logic.Squad.Command.Hold, null);
				squad2.position_set = true;
				list.Remove(squad2);
			}
			if (army != null)
			{
				army.battleview_army_formation = formation;
			}
			if (list.Count == 0)
			{
				return;
			}
		}
		List<Point> list2 = new List<Point>();
		Point normalized2 = (enemy_center - center).GetNormalized();
		for (int i = -max_size_x; i <= max_size_x; i += 10)
		{
			for (int j = -max_size_y; j <= max_size_y; j += 10)
			{
				Point point2 = center + new Point((float)i, (float)j).GetRotated(-normalized2.Heading());
				if (this.pf.logic.data.IsPassable(point2, 0f) && this.IsInsideWall(point2) == inside_wall)
				{
					if (fortifications != null)
					{
						bool flag2 = false;
						for (int k = 0; k < fortifications.Count; k++)
						{
							Logic.Fortification fortification = fortifications[k];
							float num2 = point2.Dist(fortification.position);
							if (fortification.shoot_comp != null && num2 < fortification.shoot_comp.salvo_def.max_shoot_range)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							goto IL_2ED;
						}
					}
					list2.Add(point2);
				}
				IL_2ED:;
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			Logic.Squad squad3 = list[l];
			squad3.direction = normalized2;
			float num3 = 5f;
			float num4 = num3 * num3;
			List<Point> list3 = new List<Point>();
			for (int m = 0; m < list2.Count; m++)
			{
				float num5 = float.MaxValue;
				Point point3 = list2[m];
				if (this.pf.logic.data.IsPassable(point3, num3 + 1f))
				{
					for (int n = 0; n < squads.Count; n++)
					{
						if (n != l)
						{
							Logic.Squad squad4 = squads[n];
							if (squad4.position_set)
							{
								float num6 = point3.SqrDist(squad4.position);
								if (num6 < num5)
								{
									num5 = num6;
								}
							}
						}
					}
					if (num5 >= num4)
					{
						list3.Add(point3);
					}
				}
			}
			if (list3.Count == 0)
			{
				for (int num7 = 0; num7 < list2.Count; num7++)
				{
					Point point4 = list2[num7];
					if (this.pf.logic.data.IsPassable(point4, num3 + 1f))
					{
						list3.Add(point4);
					}
				}
			}
			squad3.position_set = true;
			if (list3.Count == 0)
			{
				squad3.Teleport(center);
				squad3.SetCommand(Logic.Squad.Command.Hold, null);
			}
			else
			{
				WeightedRandom<Point> weightedRandom = new WeightedRandom<Point>(list3.Count, -1);
				for (int num8 = 0; num8 < list3.Count; num8++)
				{
					Point val = list3[num8];
					weightedRandom.AddOption(val, (float)Math.Pow((double)(100f / (1f + val.SqrDist(enemy_center))), 3.0));
				}
				squad3.Teleport(weightedRandom.Choose(list3[0], false));
				squad3.SetCommand(Logic.Squad.Command.Hold, null);
			}
		}
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00057860 File Offset: 0x00055A60
	private Point PositionAttackers()
	{
		if (this.sbv == null)
		{
			return Point.Zero;
		}
		List<Logic.Squad> squads = BattleMap.battle.squads.Get(0);
		if (this.sbv.north_camps == null)
		{
			return Point.Zero;
		}
		List<PrefabGrid> list = this.attacker_camps;
		List<PrefabGrid> list2 = this.defender_camps;
		Point point = this.attacker_reinforcement_position;
		if (list != null && list.Count > 0 && list[0] != null)
		{
			Point point2 = list[0].transform.position;
			Point enemy_center;
			if (list2 != null && list2.Count > 0 && list2[0] != null)
			{
				enemy_center = list2[0].transform.position;
			}
			else
			{
				if (!(this.sbv.citadel != null))
				{
					return Point.Zero;
				}
				enemy_center = this.sbv.citadel.transform.position;
			}
			List<Logic.Army> armies = BattleMap.battle.GetArmies(0);
			Logic.Army army = null;
			Logic.Army army2 = null;
			Point army_center = point2;
			Point army_center2 = point2;
			if (armies.Count > 0)
			{
				army = armies[0];
			}
			if (armies.Count > 1)
			{
				army2 = armies[1];
			}
			if (army2 == null)
			{
				StandardArmyFormation standardArmyFormation = new StandardArmyFormation();
				this.PositionSquads(squads, point2, enemy_center, false, false, 50, 50, army, null, null, standardArmyFormation, false, false);
				army.act_separately = true;
			}
			else if (army2 == BattleMap.battle.reinforcements[0].army || army2 == BattleMap.battle.reinforcements[2].army)
			{
				StandardArmyFormation standardArmyFormation = new StandardArmyFormation();
				StandardArmyFormation standardArmyFormation2 = new StandardArmyFormation();
				this.PositionSquads(squads, point2, enemy_center, false, false, 50, 50, army, null, null, standardArmyFormation, false, false);
				this.PositionSquads(squads, point, enemy_center, false, false, 50, 50, army2, null, null, standardArmyFormation2, false, false);
				army_center2 = point;
				army.act_separately = true;
				army2.act_separately = (BattleMap.battle.type != Logic.Battle.Type.BreakSiege || !(this.sbv.citadel != null) || army2.kingdom_id != army.kingdom_id);
			}
			else if (army2.kingdom_id == army.kingdom_id)
			{
				StandardArmyFormation standardArmyFormation;
				if (BattleMap.battle.type == Logic.Battle.Type.BreakSiege && this.sbv.citadel != null)
				{
					standardArmyFormation = new StandardArmyFormation();
				}
				else
				{
					standardArmyFormation = new StandardArmyFormation(20f, 1f, 1f, 5f, false, false, StandardArmyFormation.ArmyFormationType.Default);
				}
				this.PositionSquads(squads, point2, enemy_center, false, false, 50, 50, army, null, army2, standardArmyFormation, false, false);
				army.act_separately = true;
				army2.act_separately = false;
			}
			else
			{
				StandardArmyFormation standardArmyFormation;
				StandardArmyFormation standardArmyFormation2;
				if (BattleMap.battle.type == Logic.Battle.Type.BreakSiege && this.sbv.citadel != null)
				{
					standardArmyFormation = new StandardArmyFormation(20f, 20f, 1f, 1f, true, false, StandardArmyFormation.ArmyFormationType.Default);
					standardArmyFormation2 = new StandardArmyFormation(20f, 20f, 1f, 1f, true, true, StandardArmyFormation.ArmyFormationType.Default);
					standardArmyFormation.settings.reserve_line_offset = -10f;
					standardArmyFormation.settings.center_row_limit = 11;
					standardArmyFormation2.settings.reserve_line_offset = -10f;
					standardArmyFormation2.settings.center_row_limit = 11;
				}
				else
				{
					standardArmyFormation = new StandardArmyFormation(StandardArmyFormation.ArmyFormationType.Default, true, false);
					standardArmyFormation2 = new StandardArmyFormation(StandardArmyFormation.ArmyFormationType.Default, true, true);
				}
				this.PositionSquads(squads, point2, enemy_center, false, false, 50, 50, army, null, null, standardArmyFormation, false, false);
				this.PositionSquads(squads, point2, enemy_center, false, false, 50, 50, army2, null, null, standardArmyFormation2, false, false);
				army.act_separately = true;
				army2.act_separately = true;
			}
			if (army != null)
			{
				this.PositionMobileSiege(army_center, enemy_center, 0, false, army, false);
			}
			if (army2 != null)
			{
				this.PositionMobileSiege(army_center2, enemy_center, 0, false, army2, false);
			}
			return point2;
		}
		return Point.Zero;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00057C34 File Offset: 0x00055E34
	private Point PositionDefenders()
	{
		if (this.sbv == null)
		{
			return Point.Zero;
		}
		List<Logic.Squad> squads = BattleMap.battle.squads.Get(1);
		Point point = Point.Zero;
		List<PrefabGrid> list = this.defender_camps;
		List<PrefabGrid> list2 = this.attacker_camps;
		Point point2 = this.defender_reinforcement_position;
		if (list != null && list.Count > 0 && list[0] != null)
		{
			point = list[0].transform.position;
		}
		else
		{
			if (!(this.sbv.citadel != null))
			{
				return Point.Zero;
			}
			point = this.sbv.citadel.transform.position;
		}
		Point enemy_center = Point.Zero;
		if (list2 != null && list2.Count > 0 && list2[0] != null)
		{
			enemy_center = list2[0].transform.position;
			Bounds bounds = new Bounds(point, Vector3.one * 50f);
			if (this.sbv.wall != null)
			{
				for (int i = 0; i < this.sbv.wall.corners.Count; i++)
				{
					WallCorner wallCorner = this.sbv.wall.corners[i];
					bounds.Encapsulate(wallCorner.transform.position);
				}
			}
			Vector3 size = bounds.size;
			size.y = 0f;
			bounds.size = size;
			List<Logic.Army> armies = BattleMap.battle.GetArmies(1);
			Logic.Army army = null;
			Logic.Army army2 = null;
			if (armies.Count > 0)
			{
				army = armies[0];
			}
			if (armies.Count > 1)
			{
				army2 = armies[1];
			}
			Point point3 = new Point(this.sbv.break_siege_position_x, this.sbv.break_siege_position_y);
			bool flag = this.sbv.wall != null && this.sbv.wall.gameObject.activeInHierarchy;
			Point point4 = point;
			bool flag2 = flag;
			bool flag3 = true;
			bool one_sided = false;
			Point point5 = point;
			bool flag4 = flag;
			bool flag5 = army != null && (BattleMap.battle.is_siege || BattleMap.battle.is_plunder);
			bool flag6 = army2 != null && (BattleMap.battle.is_siege || BattleMap.battle.is_plunder);
			if (this.sbv.citadel != null)
			{
				if (army != null && army.castle == null)
				{
					point4 = point3;
					flag2 = false;
					flag5 = false;
				}
				if (army2 != null && army2.castle == null)
				{
					point5 = point3;
					flag4 = false;
					flag6 = false;
				}
			}
			if (army != null && BattleMap.battle.IsReinforcement(army))
			{
				point4 = point2;
				flag2 = false;
				flag4 = false;
				flag5 = false;
				flag3 = false;
				one_sided = false;
			}
			if (army2 != null && BattleMap.battle.IsReinforcement(army2))
			{
				point5 = point2;
				flag4 = false;
				flag6 = false;
				flag3 = false;
				one_sided = false;
			}
			if (point4 == point5 && flag3)
			{
				if (army2 != null && army != null && army2.kingdom_id != army.kingdom_id)
				{
					flag3 = false;
					one_sided = true;
				}
			}
			else
			{
				flag3 = false;
			}
			if (flag5 && BattleMap.battle.defender.GetKingdom() != army.GetKingdom())
			{
				flag5 = false;
			}
			if (flag5 || (flag6 && BattleMap.battle.defender.GetKingdom() != army2.GetKingdom()))
			{
				flag6 = false;
			}
			if (flag3)
			{
				if (army != null)
				{
					StandardArmyFormation formation = new StandardArmyFormation();
					this.PositionSquads(squads, point4, enemy_center, flag2, false, 50, 50, army, null, army2, formation, flag5, point4 == point3);
					if ((flag2 && army.GetKingdom() == BattleMap.battle.defender_kingdom) || flag5)
					{
						army.act_separately = false;
					}
					else
					{
						army.act_separately = true;
					}
					if (army2 != null)
					{
						army2.act_separately = false;
					}
				}
			}
			else if (army != null)
			{
				StandardArmyFormation formation = new StandardArmyFormation(StandardArmyFormation.ArmyFormationType.Default, one_sided, false);
				this.PositionSquads(squads, point4, enemy_center, flag2, false, 50, 50, army, null, null, formation, flag5, point4 == point3);
				if ((flag2 && army.GetKingdom() == BattleMap.battle.defender_kingdom) || flag5)
				{
					army.act_separately = false;
				}
				else
				{
					army.act_separately = true;
				}
				if (army2 != null)
				{
					StandardArmyFormation formation2 = new StandardArmyFormation(StandardArmyFormation.ArmyFormationType.Default, one_sided, true);
					this.PositionSquads(squads, point5, enemy_center, flag4, false, 50, 50, army2, null, null, formation2, flag6, point5 == point3);
					if ((flag4 && army2.GetKingdom() == BattleMap.battle.defender_kingdom) || flag6)
					{
						army2.act_separately = false;
					}
					else
					{
						army2.act_separately = true;
					}
				}
			}
			if (army != null)
			{
				this.PositionMobileSiege(point4, enemy_center, 1, flag2, army, false);
			}
			if (army2 != null)
			{
				this.PositionMobileSiege(point5, enemy_center, 1, flag4, army2, false);
			}
			if (BattleMap.battle.is_siege || BattleMap.battle.is_plunder)
			{
				if (!flag5 && !flag6)
				{
					this.PositionSquads(squads, point, enemy_center, flag, false, 50, 50, null, null, null, null, true, false);
				}
				this.PositionMobileSiege(point, enemy_center, 1, flag, null, true);
			}
			return point;
		}
		return Point.Zero;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0005815C File Offset: 0x0005635C
	public DT.Field ApplyClimateDef()
	{
		MakeMap component = base.GetComponent<MakeMap>();
		if (((component != null) ? component.graph : null) == null)
		{
			return null;
		}
		Point wv_pos = this.sbv.wv_pos;
		Logic.Battle battle = BattleMap.battle;
		ClimateZoneInfo climateZoneInfo;
		if (((battle != null) ? battle.game : null) != null && !BattleMap.battle.battle_map_only && BattleMap.battle.IsValid())
		{
			climateZoneInfo = BattleMap.battle.game.climate_zones;
		}
		else
		{
			DT dt = global::Defs.Get(false).dt;
			List<DT.Field> list = DT.Parser.ReadFile(null, Game.maps_path + "europe/map.def", null);
			DT.Field field = null;
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field2 = list[i];
				if (field2.key == "Maps" && field2.type == "extend")
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
			climateZoneInfo = new ClimateZoneInfo();
			climateZoneInfo.SetWorldSize(field.GetPoint("size", null, true, true, true, '.'));
			climateZoneInfo.Load(dt, "europe");
		}
		if (wv_pos == Point.Zero)
		{
			return null;
		}
		this.climate_type = climateZoneInfo.GetZoneType(wv_pos);
		DT.Field defField = global::Defs.GetDefField(this.climate_type.ToString() + "ClimateZone", null);
		if (defField == null)
		{
			return null;
		}
		List<string> list2 = defField.Keys(true, true);
		for (int k = 0; k < list2.Count; k++)
		{
			DT.Field field4 = defField.FindChild(list2[k], null, true, true, true, '.');
			if (field4.Type() == "terrain")
			{
				TerrainData terrainData = field4.Value(null, true, true).Get<TerrainData>();
				if (!(terrainData == null))
				{
					int l = 0;
					while (l < component.graph.nodes.Count)
					{
						XNode.Node node = component.graph.nodes[l];
						if (node.name == field4.key)
						{
							InputTerrainNode inputTerrainNode = node as InputTerrainNode;
							if (inputTerrainNode != null)
							{
								inputTerrainNode.td = terrainData;
								break;
							}
							InputHeightsNode inputHeightsNode = node as InputHeightsNode;
							if (inputHeightsNode != null)
							{
								inputHeightsNode.td = terrainData;
								break;
							}
							break;
						}
						else
						{
							l++;
						}
					}
				}
			}
			else if (field4.Type() == "prefab")
			{
				if (field4.key == "RoadBacgroundDecal")
				{
					this.sbv.roadBackgroundPrefab = field4.Value(null, true, true).Get<GameObject>().GetComponent<TerrainPath>();
				}
				else if (field4.key == "RoadForegroundDecal")
				{
					this.sbv.roadPrefab = field4.Value(null, true, true).Get<GameObject>().GetComponent<TerrainPath>();
				}
			}
		}
		if (this.sbv == null)
		{
			return null;
		}
		this.sbv.src_terrain = defField.GetRandomValue("Architectures", null, true, true, true, '.').Get<TerrainData>();
		return defField;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x000584BC File Offset: 0x000566BC
	public void GenerateSDF()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		Texture2D value = new SDFGenerator(activeTerrain.terrainData.GetHeights(0, 0, activeTerrain.terrainData.heightmapResolution, activeTerrain.terrainData.heightmapResolution), 512, MapData.GetWaterLevel(), activeTerrain.terrainData.size.y, 10f).GenerateGPU();
		GameObject gameObject = GameObject.Find("Ocean");
		if (gameObject == null)
		{
			return;
		}
		gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_SDF", value);
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x00058548 File Offset: 0x00056748
	public void SetSettlementGraphParams()
	{
		MakeMap makeMap = base.GetComponent<MakeMap>();
		if (makeMap == null)
		{
			makeMap = UnityEngine.Object.FindObjectOfType<MakeMap>();
		}
		if (((makeMap != null) ? makeMap.graph : null) == null)
		{
			return;
		}
		List<TownSDFNode> list = new List<TownSDFNode>();
		for (int i = 0; i < makeMap.graph.nodes.Count; i++)
		{
			TownSDFNode townSDFNode = makeMap.graph.nodes[i] as TownSDFNode;
			if (!(townSDFNode == null))
			{
				list.Add(townSDFNode);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		if (this.wall_segs.IsCreated)
		{
			this.wall_segs.Dispose();
		}
		float4 bounds;
		this.SetSettlementGraphParams(out this.wall_segs, out bounds);
		for (int j = 0; j < list.Count; j++)
		{
			TownSDFNode townSDFNode2 = list[j];
			townSDFNode2.corner_points = this.wall_segs;
			townSDFNode2.bounds = bounds;
		}
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0005862C File Offset: 0x0005682C
	public void SetSettlementGraphParams(out NativeArray<float2> wall_segs, out float4 bounds)
	{
		if (this.sbv == null)
		{
			this.sbv = UnityEngine.Object.FindObjectOfType<SettlementBV>();
		}
		wall_segs = default(NativeArray<float2>);
		bounds = 0;
		if (this.sbv == null)
		{
			return;
		}
		if (this.sbv.field == null || this.sbv.field.key != this.sbv.id.ToString())
		{
			this.sbv.Load();
		}
		if (this.sbv.field == null)
		{
			return;
		}
		DT.Field field = this.sbv.field.FindChild("wall.corners", null, true, true, true, '.');
		if (field == null || field.children == null || field.children.Count == 0)
		{
			return;
		}
		DT.Field field2 = this.sbv.field.FindChild("wall", null, true, true, true, '.');
		DT.Field field3 = this.sbv.field.FindChild("citadel", null, true, true, true, '.');
		if (field3 == null)
		{
			return;
		}
		float2 rhs = field2.Point(null);
		float2 v = (Point)field3.value.obj_val;
		float2[] array = new float2[field.children.Count];
		float[] array2 = new float[field.children.Count];
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field4 = field.children[i];
			if (!string.IsNullOrEmpty(field4.key) && !(field4.key == "prefab"))
			{
				if (field4.value.obj_val is Point)
				{
					array[i] = (Point)field4.value.obj_val;
				}
				else
				{
					array[i] = 0;
				}
			}
		}
		float @float = this.sbv.field.GetFloat("rotation", null, 0f, true, true, true, '.');
		Terrain activeTerrain = Terrain.activeTerrain;
		float2 float2 = new float2(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.z);
		Point point = new Point(float2.x / 2f, float2.y / 2f);
		Point point2 = v.GetRotated(@float) + point;
		bounds = new float4(point2.x, point2.x, point2.y, point2.y);
		wall_segs = new NativeArray<float2>(array.Length * 2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		for (int j = 0; j < array.Length; j++)
		{
			float2 float3 = array[j];
			Point point3 = float3 + rhs - v;
			float3 = point + point3.GetRotated(@float) * this.sbv.WallDistance;
			array[j] = float3;
		}
		for (int k = 0; k < array.Length; k++)
		{
			float2 float4 = array[k];
			float2 lhs = array[(k + 1 >= array.Length) ? 0 : (k + 1)];
			float2 rhs2 = array[(k == 0) ? (array.Length - 1) : (k - 1)];
			float2 float5 = float4 - rhs2;
			float5 = math.normalize(float5);
			float2 float6 = lhs - float4;
			float6 = math.normalize(float6);
			array2[k] = Quaternion.LookRotation(new Vector3(float5.x + float6.x, 0f, float5.y + float6.y)).eulerAngles.y + 270f;
		}
		for (int l = 0; l < array.Length; l++)
		{
			float2 lhs2 = array[l];
			float angle = array2[l];
			float2 lhs3 = Point.UnitRight.GetRotated(angle);
			wall_segs[l * 2] = lhs2 - lhs3 * 8.4f;
			wall_segs[l * 2 + 1] = lhs2 + lhs3 * 8.4f;
		}
		for (int m = 0; m < wall_segs.Length; m++)
		{
			float2 float7 = wall_segs[m];
			if (float7.x < bounds.x)
			{
				bounds.x = float7.x;
			}
			if (float7.x > bounds.y)
			{
				bounds.y = float7.x;
			}
			if (float7.y < bounds.z)
			{
				bounds.z = float7.y;
			}
			if (float7.y > bounds.w)
			{
				bounds.w = float7.y;
			}
		}
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00058B0C File Offset: 0x00056D0C
	public void SetEuropeHeights(Point pos_world)
	{
		MakeMap makeMap = base.GetComponent<MakeMap>();
		if (makeMap == null)
		{
			makeMap = UnityEngine.Object.FindObjectOfType<MakeMap>();
		}
		if (((makeMap != null) ? makeMap.graph : null) == null)
		{
			return;
		}
		Point point = Point.Zero;
		List<InputHeightsNode> list = new List<InputHeightsNode>();
		List<Float2Node> list2 = new List<Float2Node>();
		List<TransformationNode> list3 = new List<TransformationNode>();
		List<LevelsNode> list4 = new List<LevelsNode>();
		for (int i = 0; i < makeMap.graph.nodes.Count; i++)
		{
			XNode.Node node = makeMap.graph.nodes[i];
			if (node.name == "EuropeHeights")
			{
				InputHeightsNode inputHeightsNode = node as InputHeightsNode;
				if (inputHeightsNode != null)
				{
					list.Add(inputHeightsNode);
				}
			}
			else if (node.name == "EuropePos")
			{
				Float2Node float2Node = node as Float2Node;
				if (float2Node != null)
				{
					list2.Add(float2Node);
				}
				else
				{
					TransformationNode transformationNode = node as TransformationNode;
					if (transformationNode != null)
					{
						list3.Add(transformationNode);
					}
				}
			}
			if (node.name == "ForestDensity")
			{
				LevelsNode levelsNode = node as LevelsNode;
				if (levelsNode != null)
				{
					list4.Add(levelsNode);
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			InputHeightsNode inputHeightsNode2 = list[j];
			if (point == Point.Zero)
			{
				point = inputHeightsNode2.td.size;
			}
			float2 center = new float2(pos_world.x / point.x, pos_world.y / point.y);
			inputHeightsNode2.subRect.center = center;
		}
		float2 @float = pos_world;
		for (int k = 0; k < list2.Count; k++)
		{
			list2[k].res = @float;
		}
		for (int l = 0; l < list3.Count; l++)
		{
			list3[l].transformation.Offset = @float;
		}
		DT dt = global::Defs.Get(false).dt;
		TerrainType[,] array;
		if (BattleMap.battle != null && BattleMap.battle.IsValid())
		{
			array = BattleMap.battle.tt_grid;
		}
		else
		{
			TerrainTypesInfo terrainTypesInfo = new TerrainTypesInfo();
			terrainTypesInfo.SetWorldSize(point);
			terrainTypesInfo.Load(dt, "Europe");
			DT.Field field = dt.Find("Battle", null);
			int @int = field.GetInt("tt_grid_width", null, 10, true, true, true, '.');
			int int2 = field.GetInt("tt_grid_height", null, 10, true, true, true, '.');
			int num;
			int num2;
			array = Logic.Battle.BuildTTGrid(terrainTypesInfo, @int, int2, pos_world, out num, out num2);
		}
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		int num3 = length * length2;
		float num4 = 0f;
		for (int m = 0; m < length; m++)
		{
			for (int n = 0; n < length2; n++)
			{
				if (array[m, n] == TerrainType.Forest)
				{
					num4 += 1f;
				}
			}
		}
		DT.Field field2 = this.ApplyClimateDef();
		float s = num4 / (float)num3;
		float float2 = field2.GetFloat("tree_density.min.in_gamma", null, -0.92f, true, true, true, '.');
		float float3 = field2.GetFloat("tree_density.max.in_gamma", null, -0.7f, true, true, true, '.');
		float float4 = field2.GetFloat("tree_density.min.in_max", null, 0.9f, true, true, true, '.');
		float float5 = field2.GetFloat("tree_density.max.in_max", null, 0.6f, true, true, true, '.');
		float in_gamma = math.lerp(float2, float3, s);
		float in_max = math.lerp(float4, float5, s);
		for (int num5 = 0; num5 < list4.Count; num5++)
		{
			LevelsNode levelsNode2 = list4[num5];
			levelsNode2.in_gamma = in_gamma;
			levelsNode2.in_max = in_max;
		}
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00058EAC File Offset: 0x000570AC
	public void GenerateTownSDF()
	{
		NativeArray<float2> wall_points;
		float4 @float;
		this.SetSettlementGraphParams(out wall_points, out @float);
		Terrain activeTerrain = Terrain.activeTerrain;
		new TownSDFGenerator(1024, 30f, wall_points, new float2(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.z)).GenerateGPU();
		wall_points.Dispose();
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00058F0B File Offset: 0x0005710B
	public static void ValidateBattle()
	{
		if (BattleMap.battle != null && !BattleMap.battle.IsValid())
		{
			BattleMap.battle = null;
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00058F28 File Offset: 0x00057128
	public void GenerateMap()
	{
		BattleMap.ValidateBattle();
		MakeMap component = base.GetComponent<MakeMap>();
		if (component != null)
		{
			if (component.graph == null)
			{
				return;
			}
			if (this.sbv == null)
			{
				this.sbv = UnityEngine.Object.FindObjectOfType<SettlementBV>();
			}
			BattleMap.GenerateMap(BattleMap.settlement_id, BattleMap.wv_battle_pos, false);
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00058F84 File Offset: 0x00057184
	private void OnEnable()
	{
		this.SetSettlementGraphParams();
		this.LoadSDF();
		BackgroundMusic.Reset();
		if (GameLogic.Get(false) != null)
		{
			using (Game.Profile("LoadingFMODBanks", false, 0f, null))
			{
				base.LoadFMODBanks("Title_Banks");
				base.LoadFMODBanks("BV_Banks");
				base.LoadFMODBanks("WV_BV_Banks");
			}
		}
		this.Init();
		if (Application.isPlaying)
		{
			Troops.Init(null);
			this.LoadTroopsDefs();
			Troops.InitPointers();
			this.texture_baker = new TextureBaker();
			this.texture_baker.SetKingdomColors(BattleMap.Get().unit_colors);
		}
		Scene activeScene = SceneManager.GetActiveScene();
		string directoryName = Path.GetDirectoryName(activeScene.path);
		string scene_name = Path.GetFileNameWithoutExtension(activeScene.path).ToLowerInvariant();
		base.LoadTintTexture(directoryName);
		if (BattleMap.battle != null)
		{
			scene_name = BattleMap.battle.batte_view_game.map_name;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			Game.Log("WorldView game is null!", Game.LogType.Error);
			return;
		}
		if (game.multiplayer == null)
		{
			game.CreateMultiplayer(Logic.Multiplayer.Type.Server, 0);
		}
		base.LoadMapDef(game.dt, scene_name);
		if (BattleMap.battle == null)
		{
			Shader.SetGlobalVector("_WVBattlePos", new Vector4(0f, 0f, 0f, 1f));
			this.CreateBattle();
			if (BattleMap.battle == null)
			{
				return;
			}
		}
		else
		{
			Shader.SetGlobalVector("_WVBattlePos", new Vector4(BattleMap.battle.position.x, 0f, BattleMap.battle.position.y, 1f));
		}
		this.AssignColors();
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.SetHighlighedRealm(null);
			worldMap.SetSelectedRealm(0);
		}
		BattleMap.battle.AddListener(this);
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x00059154 File Offset: 0x00057354
	private void LoadTroopsDefs()
	{
		BattleMap.defs.Clear();
		BattleMap.salvo_defs.Clear();
		global::Defs defs = global::Defs.Get(false);
		DT.Field field = defs.dt.Find("Unit", null);
		DT.Field field2 = defs.dt.Find("SalvoData", null);
		if (field != null)
		{
			for (int i = 0; i < field.def.defs.Count; i++)
			{
				Logic.Unit.Def def = field.def.defs[i].def as Logic.Unit.Def;
				if (def != null)
				{
					BattleMap.defs.Add(def);
				}
			}
		}
		if (field2 != null)
		{
			for (int j = 0; j < field2.def.defs.Count; j++)
			{
				SalvoData.Def def2 = field2.def.defs[j].def as SalvoData.Def;
				if (def2 != null)
				{
					BattleMap.salvo_defs.Add(def2);
				}
			}
		}
		Troops.mem_data.NumDefs = BattleMap.defs.Count;
		Troops.valid_defs = new List<Logic.Unit.Def>();
		Troops.mem_data.defs = new NativeArray<Troops.DefData>(Troops.mem_data.NumDefs, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		int num = 0;
		this.SetTroopDefs();
		for (int k = 0; k < BattleMap.defs.Count; k++)
		{
			Logic.Unit.Def def3 = BattleMap.defs[k];
			if (!def3.field.GetBool("fake_unit", null, false, false, true, true, '.'))
			{
				DT.Field field3 = def3.field.FindChild("model", null, true, true, true, '.');
				if (field3 != null)
				{
					int num2 = field3.NumValues();
					if (num2 > 0)
					{
						num += num2;
						for (int l = 0; l < num2; l++)
						{
							if (field3.Value(l, null, true, true).Get<GameObject>() != null)
							{
								Troops.valid_defs.Add(def3);
							}
						}
					}
				}
			}
		}
		Troops.mem_data.InitAnimData(num);
		Troops.mem_data.NumSalvoDefs = BattleMap.salvo_defs.Count;
		Troops.mem_data.salvo_defs = new NativeArray<Troops.SalvoDefData>(Troops.mem_data.NumSalvoDefs, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		for (int m = 0; m < BattleMap.salvo_defs.Count; m++)
		{
			Troops.mem_data.salvo_defs[m] = new Troops.SalvoDefData(m, BattleMap.salvo_defs[m]);
		}
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x000593A4 File Offset: 0x000575A4
	public unsafe void SetTroopDefs()
	{
		for (int i = 0; i < BattleMap.defs.Count; i++)
		{
			Logic.Unit.Def def = BattleMap.defs[i];
			Troops.mem_data.defs[i] = new Troops.DefData(i, def);
		}
		for (int j = 0; j < Troops.squads.Length; j++)
		{
			global::Squad squad = Troops.squads[j];
			if (!(squad == null))
			{
				Troops.DefData* def2 = Troops.pdata->GetDef(squad.def.troops_def_idx);
				squad.data->def = def2;
			}
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00059430 File Offset: 0x00057630
	public bool IsReady()
	{
		return true & BattleMap.battle != null;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0005943C File Offset: 0x0005763C
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.wall_segs.IsCreated)
		{
			this.wall_segs.Dispose();
		}
		this.Clear();
		Shader.SetGlobalVector("_WVBattlePos", Vector4.zero);
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00059474 File Offset: 0x00057674
	private void OnDestroy()
	{
		if (BattleMap.instance == this)
		{
			BattleMap.instance = null;
		}
		RuntimePathDataBuilder runtimePathDataBuilder = this.pdb;
		if (runtimePathDataBuilder != null)
		{
			runtimePathDataBuilder.Dispose();
		}
		if (Application.isPlaying)
		{
			Troops.Done();
			if (this.texture_baker != null)
			{
				this.texture_baker.Dispose();
			}
		}
		else
		{
			PrefabGrid.Info.Clear();
		}
		base.UnloadFMODBanks("BV_Banks");
		BattleMap.defs.Clear();
		BattleMap.salvo_defs.Clear();
		GameLogic gameLogic = GameLogic.instance;
		Transform transform;
		if (gameLogic == null)
		{
			transform = null;
		}
		else
		{
			Transform transform2 = gameLogic.transform;
			transform = ((transform2 != null) ? transform2.Find("Ragdolls") : null);
		}
		Transform transform3 = transform;
		if (transform3 != null)
		{
			UnityEngine.Object.Destroy(transform3.gameObject);
		}
		this.ClearSkirmishes();
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00059528 File Offset: 0x00057728
	private void ClearSkirmishes()
	{
		for (int i = BattleMap.skirmishes.Count - 1; i >= 0; i--)
		{
			BattleMap.skirmishes[i].Destroy();
		}
		BattleMap.skirmishes.Clear();
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00059568 File Offset: 0x00057768
	public void UpdateSkirmishes()
	{
		if (BattleMap.battle == null || BattleMap.battle.stage != Logic.Battle.Stage.Ongoing)
		{
			return;
		}
		using (Game.Profile("BattleMap.UpdateSkirmishSounds", false, 0f, null))
		{
			for (int i = BattleMap.skirmishes.Count - 1; i >= 0; i--)
			{
				if (BattleMap.skirmishes[i].squads.Count == 0)
				{
					BattleMap.skirmishes[i].Destroy();
				}
				else
				{
					BattleMap.skirmishes[i].SetParameters();
				}
			}
			for (int j = 0; j < Troops.squads.Length; j++)
			{
				global::Squad squad = Troops.squads[j];
				if (!(squad == null))
				{
					bool flag = squad.ValidForSkirmish();
					if (!flag && squad.skirmish != null)
					{
						squad.skirmish.DelSquad(squad, true);
					}
					if (!Troops.CheckDefeated(squad, false) && !squad.logic.IsDefeated())
					{
						global::Squad enemy = squad.enemy;
						if (((enemy != null) ? enemy.logic : null) != null && !squad.enemy.logic.IsDefeated() && flag)
						{
							if (squad.skirmish == null)
							{
								Skirmish.CreateSkirmish(squad, squad.enemy);
							}
						}
						else if (squad.skirmish != null && !flag)
						{
							bool flag2 = false;
							for (int k = 0; k < squad.skirmish.squads.Count; k++)
							{
								if (squad.skirmish.squads[k].enemy == squad)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								squad.skirmish.DelSquad(squad, true);
							}
						}
					}
				}
			}
			if (!this.has_visible_squads_fighting)
			{
				bool flag3 = false;
				for (int l = BattleMap.skirmishes.Count - 1; l >= 0; l--)
				{
					Skirmish skirmish = BattleMap.skirmishes[l];
					bool flag4 = false;
					for (int m = 0; m < skirmish.squads.Count; m++)
					{
						if (skirmish.squads[m].IsVisible())
						{
							flag4 = true;
							break;
						}
					}
					if (flag4)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					this.SetVisibleSquadsFighting(true);
				}
			}
		}
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x000597AC File Offset: 0x000579AC
	private void SetVisibleSquadsFighting(bool val)
	{
		this.has_visible_squads_fighting = val;
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x000597B5 File Offset: 0x000579B5
	public void RefreshBattleMusicSwitch()
	{
		if (BattleFieldOverview.InProgress() || BattleMap.battle.stage == Logic.Battle.Stage.Preparing || BattleMap.battle.stage == Logic.Battle.Stage.EnteringBattle)
		{
			this.SetBattleMusicSwitch(0);
			return;
		}
		this.SetBattleMusicSwitch(this.has_visible_squads_fighting ? 2 : 1);
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x000597F4 File Offset: 0x000579F4
	private void SetBattleMusicSwitch(int val)
	{
		RuntimeManager.StudioSystem.setParameterByName("BattleMusicSwitch", (float)val, false);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x00059818 File Offset: 0x00057A18
	private void Update()
	{
		if (Application.isPlaying)
		{
			Logic.Battle battle = BattleMap.battle;
			if (((battle != null) ? battle.batte_view_game : null) != null)
			{
				BattleMap.battle.simulation.OnUpdate();
				BattleMap.battle.UpdateInitiative(UnityEngine.Time.deltaTime);
				BattleMap.battle.CheckVictory(true, false);
				Troops.OnUpdate();
				this.UpdateSkirmishes();
				this.UpdateWanderers();
				this.UpdatePhysics();
				this.RefreshBattleMusicSwitch();
				if (BattleMap.battle.stage == Logic.Battle.Stage.Ongoing)
				{
					BattleMap.battle.CheckReinforcementTimers();
				}
				GameCamera gameCamera = CameraController.GameCamera;
				this.texture_baker.Draw((gameCamera != null) ? gameCamera.Camera : null);
				this.texture_baker.path_arrows_straight_drawer.model_data.Clear();
				this.texture_baker.path_arrows_shooting_range_straight_drawer.model_data.Clear();
				if (BattleMap.battle.stage == Logic.Battle.Stage.Ongoing)
				{
					for (int i = 0; i < 2; i++)
					{
						List<Logic.Character> list = BattleMap.battle.FindValidReinforcements(i);
						if (list != null && list.Count > 0)
						{
							for (int j = 0; j < list.Count; j++)
							{
								Logic.Army army = list[j].GetArmy();
								Logic.Kingdom kingdom = army.GetKingdom();
								if (kingdom.ai != null && kingdom.ai.Enabled(KingdomAI.EnableFlags.Armies, true))
								{
									KingdomAI ai = kingdom.ai;
									if (ai != null)
									{
										ai.ThinkFight(army, BattleMap.battle);
									}
								}
							}
						}
					}
				}
				if (BattleMap.refresh_graph_settlements)
				{
					this.SetSettlementGraphParams();
					BattleMap.refresh_graph_settlements = false;
				}
				if (this.position_squads && SettlementBV.finished_generation)
				{
					this.PositionSquads();
				}
			}
		}
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x000599A0 File Offset: 0x00057BA0
	private void UpdatePhysics()
	{
		using (Game.Profile("BattleMap.UpdatePhysics", false, 0f, null))
		{
			if (!Physics.autoSimulation)
			{
				float num = UnityEngine.Time.fixedDeltaTime * Mathf.Clamp01(UnityEngine.Time.timeScale);
				if (num != 0f)
				{
					this.battle_timer += UnityEngine.Time.deltaTime;
					while (this.battle_timer >= num)
					{
						this.battle_timer -= num;
						this.battlePhysics.Simulate(num);
					}
				}
			}
		}
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x00059A3C File Offset: 0x00057C3C
	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			Logic.Battle battle = BattleMap.battle;
			if (((battle != null) ? battle.batte_view_game : null) != null)
			{
				Troops.OnLateUpdate();
			}
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00059A5D File Offset: 0x00057C5D
	private void Clear()
	{
		if (BattleMap.battle == null)
		{
			return;
		}
		BattleMap.battle.DelListener(this);
		BattleMap.refresh_graph_settlements = false;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00059A78 File Offset: 0x00057C78
	private void CreateWVRealmsAndKingdoms(Game game, string map_name)
	{
		game.state = Game.State.Running;
		game.LoadRealms(map_name, null);
		game.LoadKingdoms(map_name, null);
		if (game.realms.Count == 0)
		{
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(1);
		Logic.Kingdom kingdom2 = game.GetKingdom(2);
		Logic.Kingdom kingdom3 = game.GetKingdom(3);
		Logic.Kingdom kingdom4 = game.GetKingdom(4);
		if (string.IsNullOrEmpty(game.multiplayer.playerData.kingdomName))
		{
			game.multiplayer.playerData.kingdomName = kingdom.Name;
		}
		game.multiplayer.UpdatePlayerInCurrentPlayers(game.multiplayer.playerData.pid, game.multiplayer.playerData.kingdomName);
		kingdom.ai.enabled = KingdomAI.EnableFlags.Disabled;
		kingdom2.ai.enabled = KingdomAI.EnableFlags.Disabled;
		kingdom3.ai.enabled = KingdomAI.EnableFlags.Disabled;
		kingdom4.ai.enabled = KingdomAI.EnableFlags.Disabled;
		if (kingdom != null && kingdom2 != null)
		{
			War war = kingdom.StartWarWith(kingdom2, War.InvolvementReason.InternalPurposes, null, null, true);
			war.Join(kingdom3, kingdom, War.InvolvementReason.InternalPurposes);
			war.Join(kingdom4, kingdom2, War.InvolvementReason.InternalPurposes);
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00059B7C File Offset: 0x00057D7C
	private Logic.Army CreateArmy(Logic.Kingdom k, Point pos)
	{
		if (k == null)
		{
			return null;
		}
		Logic.Character c = CharacterFactory.CreateCourtCandidate(k.game, k.id, "Marshal");
		k.AddCourtMember(c, -1, false, true, false, true);
		Logic.Army army = new Logic.Army(k.game, pos, k.id);
		Logic.Realm realm = k.game.GetRealm(2);
		army.SetLeader(c, true);
		army.Start();
		realm.AddArmy(army);
		army.realm_in = realm;
		return army;
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00059BF0 File Offset: 0x00057DF0
	private Castle CreateCastle(Logic.Kingdom k, Point pos)
	{
		if (k == null)
		{
			return null;
		}
		Castle castle = new Castle(k.game, pos);
		castle.SetKingdom(k.id);
		k.realms[0].castle = castle;
		castle.realm_id = k.realms[0].id;
		return castle;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00059C48 File Offset: 0x00057E48
	private void CreateBattle()
	{
		BattleMap.creating_fake_battle = true;
		Scene activeScene = SceneManager.GetActiveScene();
		string text = activeScene.IsValid() ? activeScene.name.ToLowerInvariant() : "battleview";
		Game game = GameLogic.Get(true);
		if (game.campaign == null)
		{
			game.campaign = Campaign.CreateSinglePlayerCampaign("europe", "mid");
		}
		if (game.rules == null)
		{
			game.rules = new Game.CampaignRules(game);
		}
		game.map_name = "europe";
		game.rules.Load();
		this.CreateWVRealmsAndKingdoms(game, text);
		if (game.kingdoms.Count == 0)
		{
			return;
		}
		game.LoadCoAIndicesAndColors("europe", "mid");
		Logic.Kingdom kingdom = game.GetKingdom(1);
		Logic.Kingdom kingdom2 = game.GetKingdom(2);
		Logic.Kingdom kingdom3 = game.GetKingdom(3);
		Logic.Kingdom kingdom4 = game.GetKingdom(4);
		Logic.Army attacker = this.CreateArmy(kingdom, new Point(0f, 0f));
		Logic.Army army = this.CreateArmy(kingdom2, new Point(1f, 1f));
		Logic.Army army2 = this.CreateArmy(kingdom3, new Point(100f, 100f));
		Logic.Army army3 = this.CreateArmy(kingdom4, new Point(100f, 100f));
		army2.FillWithRandomUnits();
		army3.FillWithRandomUnits();
		SettlementBV settlementBV = this.sbv;
		Logic.Battle battle;
		if (((settlementBV != null) ? settlementBV.field : null) == null)
		{
			battle = Logic.Battle.StartBattle(attacker, army, true);
		}
		else
		{
			Castle castle = this.CreateCastle(game.GetKingdom(2), new Point(100f, 150f));
			castle.Start();
			castle.keep_effects.siege_defense_condition.Set(this.starting_siege_defense, true);
			army.EnterCastle(castle, true);
			battle = Logic.Battle.StartBattle(attacker, castle, true);
			if (this.assault)
			{
				battle.Assault();
			}
			else
			{
				battle.BreakSiege(Logic.Battle.BreakSiegeFrom.Inside, true);
			}
		}
		battle.realm_id = 2;
		battle.Start();
		BattleMap.SetBattle(battle, 1);
		battle.DoAction("enter_battle", 0, text);
		BattleMap.creating_fake_battle = false;
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00059E40 File Offset: 0x00058040
	public static void GenerateMap(int id, Point wv_pos, bool no_settlement = false)
	{
		BattleMap battleMap = BattleMap.Get();
		battleMap.sbv.id = id;
		if (wv_pos != Point.Zero)
		{
			battleMap.sbv.wv_pos = wv_pos;
		}
		battleMap.sbv.Load();
		if (no_settlement)
		{
			id = 0;
			battleMap.sbv.id = 0;
			battleMap.sbv.field = null;
		}
		BattleMap.Get().SetSettlementGraphParams();
		MakeMap makeMap = battleMap.GetComponent<MakeMap>();
		if (makeMap == null)
		{
			makeMap = UnityEngine.Object.FindObjectOfType<MakeMap>();
		}
		battleMap.SetEuropeHeights(battleMap.sbv.wv_pos);
		SettlementBV.seed = (int)(battleMap.sbv.wv_pos.x + battleMap.sbv.wv_pos.y);
		battleMap.sbv.ResetSeed();
		if (makeMap != null)
		{
			makeMap.Run();
		}
		battleMap.pdb.Build(false, false, true, true, false);
		battleMap.sbv.Generate(true);
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x00059F30 File Offset: 0x00058130
	public void UpdateWanderers()
	{
		using (Game.Profile("BattleMap.UpdateWanderers", false, 0f, null))
		{
			if (!Troops.NotReady && Troops.Initted)
			{
				if (this.attractors != null && this.attractors.Count != 0)
				{
					if (this.texture_baker != null)
					{
						for (int i = 0; i < Attractor.defs.Count; i++)
						{
							Logic.WanderPeasant.Def def = Attractor.defs[i];
							List<TextureBaker.PerModelData> list;
							if (this.texture_baker.skinning_drawers.TryGetValue(def.field.key, out list))
							{
								for (int j = 0; j < list.Count; j++)
								{
									list[j].model_data_buffer.Clear();
								}
							}
						}
					}
					if (this.peasants != null)
					{
						float deltaTime = UnityEngine.Time.deltaTime;
						for (int k = 0; k < this.peasants.Count; k++)
						{
							global::WanderPeasant wanderPeasant = this.peasants[k].visuals as global::WanderPeasant;
							wanderPeasant.UpdateStateMachine();
							wanderPeasant.UpdateInstancer(deltaTime);
						}
					}
					if (UnityEngine.Time.time >= this.next_attractor_spawn_time)
					{
						if (Attractor.defs != null && Attractor.defs.Count != 0)
						{
							this.next_attractor_spawn_time = UnityEngine.Time.time + Attractor.time_between_spawns;
							if (this.peasants == null)
							{
								this.peasants = new List<Logic.WanderPeasant>();
							}
							int max_peasants = Attractor.max_peasants;
							if (max_peasants > this.spawn_attractors)
							{
								max_peasants = this.spawn_attractors;
							}
							if (this.peasants.Count < max_peasants)
							{
								List<Attractor> list2 = new List<Attractor>();
								for (int l = 0; l < this.attractors.Count; l++)
								{
									Attractor attractor = this.attractors[l];
									if (attractor.cur_peasant == null && attractor.HasFlag(Logic.WanderPeasant.AttractorFlags.Spawn) && !Logic.WanderPeasant.EnemiesNearby(BattleMap.battle, 1, attractor.transform.position, Attractor.defs[0].squad_run_away_threshold_dist))
									{
										list2.Add(attractor);
									}
								}
								if (list2.Count != 0)
								{
									for (int m = this.peasants.Count; m < max_peasants; m++)
									{
										Attractor sp = list2[UnityEngine.Random.Range(0, list2.Count)];
										this.SpawnWanderer(sp);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0005A198 File Offset: 0x00058398
	private void SpawnWanderer(Attractor sp)
	{
		Logic.WanderPeasant.Def def = Attractor.defs[UnityEngine.Random.Range(0, Attractor.defs.Count)];
		Vector3 position = sp.transform.position;
		if (sp.control_transform != null)
		{
			position = sp.control_transform.position;
		}
		Logic.WanderPeasant wanderPeasant = new Logic.WanderPeasant(BattleMap.battle.batte_view_game, position, BattleMap.battle.defender_kingdom.id, def, BattleMap.battle, 1);
		wanderPeasant.Start();
		global::WanderPeasant wanderPeasant2 = wanderPeasant.visuals as global::WanderPeasant;
		if (wanderPeasant2 == null)
		{
			return;
		}
		wanderPeasant2.spawn_target = sp;
		wanderPeasant2.cur_target = sp;
		wanderPeasant2.UpdateAnimation(true);
		wanderPeasant2.UpdateStateMachine();
		wanderPeasant2.transform.rotation = sp.transform.rotation;
		this.peasants.Add(wanderPeasant);
	}

	// Token: 0x04000676 RID: 1654
	private static BattleMap instance;

	// Token: 0x04000677 RID: 1655
	public static List<Logic.Unit.Def> defs = new List<Logic.Unit.Def>();

	// Token: 0x04000678 RID: 1656
	public static List<SalvoData.Def> salvo_defs = new List<SalvoData.Def>();

	// Token: 0x04000679 RID: 1657
	public static List<Skirmish> skirmishes = new List<Skirmish>();

	// Token: 0x0400067A RID: 1658
	private NativeArray<float2> wall_segs;

	// Token: 0x0400067B RID: 1659
	public static bool refresh_graph_settlements = false;

	// Token: 0x0400067C RID: 1660
	public static bool creating_fake_battle = false;

	// Token: 0x0400067D RID: 1661
	public global::PathFinding pf;

	// Token: 0x04000680 RID: 1664
	private PhysicsScene battlePhysics;

	// Token: 0x04000681 RID: 1665
	private float battle_timer;

	// Token: 0x04000682 RID: 1666
	public static int settlement_id = -1;

	// Token: 0x04000683 RID: 1667
	public static Point wv_battle_pos;

	// Token: 0x04000684 RID: 1668
	[HideInInspector]
	public SettlementBV sbv;

	// Token: 0x04000685 RID: 1669
	public RuntimePathDataBuilder pdb;

	// Token: 0x04000686 RID: 1670
	[HideInInspector]
	public Texture2D wall_sdf;

	// Token: 0x04000687 RID: 1671
	[HideInInspector]
	public float wall_sdf_max_circle;

	// Token: 0x04000688 RID: 1672
	[HideInInspector]
	public Texture2D shore_sdf;

	// Token: 0x04000689 RID: 1673
	public Texture2D Dither_Tex;

	// Token: 0x0400068A RID: 1674
	public Color capture_point_friendly = new Color(0f, 1f, 0f, 1f);

	// Token: 0x0400068B RID: 1675
	public Color capture_point_enemy = new Color(1f, 0f, 0f, 1f);

	// Token: 0x0400068C RID: 1676
	[HideInInspector]
	public float shore_sdf_max_circle;

	// Token: 0x0400068D RID: 1677
	public bool DebugGeneration;

	// Token: 0x0400068E RID: 1678
	public bool position_squads;

	// Token: 0x0400068F RID: 1679
	public bool reinforcement_positions_initialized;

	// Token: 0x04000690 RID: 1680
	public byte main_lsa;

	// Token: 0x04000691 RID: 1681
	private List<PrefabGrid> attacker_camps;

	// Token: 0x04000692 RID: 1682
	private List<PrefabGrid> defender_camps;

	// Token: 0x04000693 RID: 1683
	private SettlementBV.ReinfPoint[] attacker_reinforcement_points;

	// Token: 0x04000694 RID: 1684
	private SettlementBV.ReinfPoint[] defender_reinforcement_points;

	// Token: 0x04000695 RID: 1685
	private Point attacker_reinforcement_position;

	// Token: 0x04000696 RID: 1686
	private Point defender_reinforcement_position;

	// Token: 0x04000697 RID: 1687
	private Point defender_break_siege_position;

	// Token: 0x04000698 RID: 1688
	private bool has_visible_squads_fighting;

	// Token: 0x04000699 RID: 1689
	[NonSerialized]
	public List<Attractor> attractors;

	// Token: 0x0400069A RID: 1690
	public int spawn_attractors;

	// Token: 0x0400069B RID: 1691
	private float next_attractor_spawn_time;

	// Token: 0x0400069C RID: 1692
	[NonSerialized]
	public List<Logic.WanderPeasant> peasants;

	// Token: 0x0400069D RID: 1693
	[Tooltip("If true, battle counts as assault. Otherwise a break siege")]
	public bool assault = true;

	// Token: 0x0400069E RID: 1694
	[Range(0f, 100f)]
	public float starting_siege_defense = 100f;

	// Token: 0x0400069F RID: 1695
	public ClimateZoneType climate_type = ClimateZoneType.Mediterranean;

	// Token: 0x040006A0 RID: 1696
	public Dictionary<int, Color> kingdom_colors;

	// Token: 0x040006A1 RID: 1697
	public TextureBaker texture_baker;
}
