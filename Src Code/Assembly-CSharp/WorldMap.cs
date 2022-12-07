using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200031B RID: 795
public class WorldMap : MapData
{
	// Token: 0x060031C5 RID: 12741 RVA: 0x001936E9 File Offset: 0x001918E9
	public static Terrain GetTerrain()
	{
		return WorldMap.terrain;
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x001936F0 File Offset: 0x001918F0
	public new static WorldMap Get()
	{
		return WorldMap.instance;
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x001936F8 File Offset: 0x001918F8
	private void LoadPrefabsDef()
	{
		DT.Field defField = global::Defs.GetDefField("PoliticalView", null);
		if (defField == null)
		{
			return;
		}
		this.PoliticalMapPrefab = global::Defs.GetObj<GameObject>(defField, "political_map_prefab", null);
		this.BorderSettings.Load(defField.FindChild("borders", null, true, true, true, '.'));
	}

	// Token: 0x060031C8 RID: 12744 RVA: 0x00193744 File Offset: 0x00191944
	public void SetStartingKingdom()
	{
		if (string.IsNullOrEmpty(this.starting_realm))
		{
			return;
		}
		global::Realm realm = this.FindRealm(this.starting_realm);
		if (realm == null)
		{
			Debug.LogError("Starting realm not found: " + this.starting_realm);
			return;
		}
		global::Kingdom kingdom = realm.GetKingdom();
		if (kingdom == null)
		{
			Debug.LogError("Starting realm has no kingdom: " + this.starting_realm);
			return;
		}
		if (WorldUI.Get() == null)
		{
			Debug.LogError("UI not found while setting starting kingdom");
			return;
		}
		if (Application.isPlaying)
		{
			Game game = GameLogic.Get(true);
			if (game != null && game.multiplayer != null)
			{
				if (string.IsNullOrEmpty(game.multiplayer.playerData.kingdomName))
				{
					game.multiplayer.playerData.kingdomName = kingdom.Name;
					if (game.campaign != null && string.IsNullOrEmpty(game.campaign.GetKingdomName(false)))
					{
						game.campaign.SetLocalPlayerKingdomName(kingdom.Name, null, true);
					}
				}
				if (game.multiplayer.type == Logic.Multiplayer.Type.Server)
				{
					game.multiplayer.UpdatePlayerInCurrentPlayers(game.multiplayer.playerData.pid, game.multiplayer.playerData.kingdomName);
				}
			}
		}
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x00193874 File Offset: 0x00191A74
	private void CheckMisspelledName(Logic.Object obj, DT.Field f, string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogWarning(string.Format("{0}: {1}: Empty name", (f != null) ? f.Path(true, false, '.') : null, obj));
			return;
		}
		char c = name[name.Length - 1];
		if (c == 'E' || c == 'M' || c == 'L')
		{
			Debug.LogWarning(string.Format("{0}: {1}: Misspelled name: '{2}'", (f != null) ? f.Path(true, false, '.') : null, obj, name));
			return;
		}
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x001938EC File Offset: 0x00191AEC
	private void LoadKingdomDefs(Game game)
	{
		this.Kingdoms.Clear();
		if (game == null || game.kingdoms == null)
		{
			return;
		}
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			this.CheckMisspelledName(kingdom, kingdom.csv_field ?? kingdom.def, kingdom.Name);
			global::Kingdom kingdom2 = new global::Kingdom();
			kingdom2.logic = kingdom;
			kingdom.visuals = kingdom2;
			this.Kingdoms.Add(kingdom2);
		}
		for (int j = 0; j < this.Kingdoms.Count; j++)
		{
			global::Kingdom kingdom3 = this.Kingdoms[j];
			kingdom3.Load(kingdom3.logic);
		}
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x001939A0 File Offset: 0x00191BA0
	private void LoadRealmDefs(Game game)
	{
		this.Realms.Clear();
		if (game == null || game.realms == null)
		{
			return;
		}
		for (int i = 0; i < game.realms.Count; i++)
		{
			Logic.Realm realm = game.realms[i];
			this.CheckMisspelledName(realm, realm.csv_field ?? realm.def, realm.name);
			global::Realm realm2 = new global::Realm();
			realm2.id = realm.id;
			realm2.Name = realm.name;
			realm2.TownName = realm.town_name;
			realm2.kingdom = realm.kingdom_id;
			if (realm.def != null)
			{
				realm2.Load(realm.def);
			}
			if (Application.isPlaying)
			{
				realm2.logic = realm;
				realm.visuals = realm2;
			}
			this.Realms.Add(realm2);
		}
		this.LoadRealmBounds();
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x00193A84 File Offset: 0x00191C84
	public bool LoadRealmBounds()
	{
		string text = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		string maps_path = Game.maps_path;
		if (File.Exists(maps_path + text + "/RealmBounds.bin"))
		{
			List<float> list = null;
			using (Stream stream = File.Open(maps_path + text + "/RealmBounds.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				list = (List<float>)new BinaryFormatter().Deserialize(stream);
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i += 6)
				{
					Vector3 min = new Vector3(list[i], list[i + 1], list[i + 2]);
					Vector3 max = new Vector3(list[i + 3], list[i + 4], list[i + 5]);
					global::Realm realm = this.Realms[i / 6];
					realm.bounds = default(Bounds);
					realm.bounds.SetMinMax(min, max);
				}
			}
			return true;
		}
		Debug.LogError("Warning: no realmbounds found at: " + maps_path + text + "/RealmBounds.bin   Please regenerate and analyze the realm data");
		return false;
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x00193BB8 File Offset: 0x00191DB8
	public void LoadRealmsAndKingdoms(Game game)
	{
		this.LoadRealmDefs(game);
		this.LoadKingdomDefs(game);
		this.SetStartingKingdom();
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x00193BCE File Offset: 0x00191DCE
	public void LoadRealmIDMap(Game game)
	{
		this.RealmIDMap = game.realm_id_map;
		this.RealmsDataResolution = ((this.RealmIDMap == null) ? 0 : this.RealmIDMap.GetLength(0));
		base.ReloadView();
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x00193BFF File Offset: 0x00191DFF
	public void ResetUsedColors()
	{
		this.UsedColors = 0;
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x00193C08 File Offset: 0x00191E08
	public Color GetRandomColor()
	{
		if (this.UsedColors >= WorldMap.Colors.Length)
		{
			return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		}
		Color[] colors = WorldMap.Colors;
		int usedColors = this.UsedColors;
		this.UsedColors = usedColors + 1;
		return colors[usedColors];
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x00193C72 File Offset: 0x00191E72
	public void ClearAll()
	{
		this.ResetUsedColors();
		this.Kingdoms = new List<global::Kingdom>();
		this.Realms = new List<global::Realm>();
	}

	// Token: 0x060031D2 RID: 12754 RVA: 0x00193C90 File Offset: 0x00191E90
	public void ClearUnused(bool log = true)
	{
		for (int i = 0; i < this.Realms.Count; i++)
		{
			this.Realms[i].CastlePos = Vector3.zero;
		}
		int num = 0;
		int num2 = 0;
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			if (settlement.IsCastle())
			{
				global::Realm realm = this.FindRealm(settlement.Name);
				if (realm != null && realm.id > 0)
				{
					realm.CastlePos = settlement.transform.position;
				}
			}
			settlement = settlement.Next();
		}
		for (int j = this.Realms.Count - 1; j >= 0; j--)
		{
			global::Realm realm2 = this.Realms[j];
			if (!(realm2.CastlePos != Vector3.zero))
			{
				this.DelRealm(realm2, ref num, ref num2);
			}
		}
		if (log)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Deleted realms: ",
				num,
				", kingdoms: ",
				num2
			}));
		}
		this.GenerateRealmsAndKingdoms();
	}

	// Token: 0x060031D3 RID: 12755 RVA: 0x00193DA0 File Offset: 0x00191FA0
	private void DelRealm(global::Realm r, ref int deleted_realms, ref int deleted_kingdoms)
	{
		int i = Mathf.Abs(r.id) - 1;
		if (i < 0 || i >= this.Realms.Count || this.Realms[i] != r)
		{
			Debug.LogError("Realm IDs messed up");
			return;
		}
		deleted_realms++;
		this.Realms.RemoveAt(i);
		while (i < this.Realms.Count)
		{
			global::Realm realm = this.Realms[i];
			if (realm.id > 0)
			{
				realm.id--;
			}
			else
			{
				realm.id++;
			}
			i++;
		}
		global::Kingdom kingdom = global::Kingdom.Get(r.kingdom.id);
		if (kingdom != null)
		{
			kingdom.realms.Remove(r);
		}
		kingdom = this.FindKingdom(r.Name);
		if (kingdom != null)
		{
			kingdom.realms.Remove(r);
			if (kingdom.realms.Count == 0)
			{
				deleted_kingdoms++;
				this.DelKingdom(kingdom);
			}
		}
	}

	// Token: 0x060031D4 RID: 12756 RVA: 0x00193E9C File Offset: 0x0019209C
	private void DelKingdom(global::Kingdom k)
	{
		int i = k.id - 1;
		if (i < 0 || i >= this.Kingdoms.Count || this.Kingdoms[i] != k)
		{
			Debug.LogError("Kingdom IDs messed up");
			return;
		}
		this.Kingdoms.RemoveAt(i);
		for (int j = 0; j < this.Realms.Count; j++)
		{
			global::Realm realm = this.Realms[j];
			if (realm != null && realm.kingdom.id > i)
			{
				global::Realm realm2 = realm;
				realm2.kingdom.id = realm2.kingdom.id - 1;
			}
		}
		while (i < this.Kingdoms.Count)
		{
			k = this.Kingdoms[i];
			k.id--;
			i++;
		}
	}

	// Token: 0x060031D5 RID: 12757 RVA: 0x00193F60 File Offset: 0x00192160
	public global::Realm FindRealm(string name)
	{
		for (int i = 0; i < this.Realms.Count; i++)
		{
			global::Realm realm = this.Realms[i];
			if (realm.Name == name)
			{
				return realm;
			}
		}
		return null;
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x00193FA4 File Offset: 0x001921A4
	public global::Kingdom FindKingdom(string name)
	{
		for (int i = 0; i < this.Kingdoms.Count; i++)
		{
			global::Kingdom kingdom = this.Kingdoms[i];
			if (kingdom.Name == name)
			{
				return kingdom;
			}
		}
		return null;
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x00193FE8 File Offset: 0x001921E8
	public void CheckRealmsAndKingdoms()
	{
		List<string> list = new List<string>();
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			if (settlement.IsCastle())
			{
				string text = "";
				global::Realm realm = this.FindRealm(settlement.Name);
				global::Kingdom kingdom = this.FindKingdom(settlement.Name);
				if (realm == null)
				{
					text = text + "Castle " + settlement.Name + " does not have a coresponding realm. ";
				}
				else
				{
					list.Add(realm.Name);
				}
				if (realm != null && realm.IsSeaRealm())
				{
					text = text + "Realm " + realm.Name + "has a castle, but is sea realm. ";
				}
				if (kingdom == null)
				{
					if (realm == null)
					{
						text = text + "Castle " + settlement.Name + " does not have a coresponding kingdom. ";
					}
					else
					{
						text = text + "Castle " + settlement.Name + " has a coresponding realm, but not a coresponding kingdom. ";
					}
				}
				if (realm != null && kingdom != null && kingdom.id != realm.id)
				{
					text = string.Concat(new object[]
					{
						text,
						"Realm ",
						realm.Name,
						" has different id than its kingdom - (r.id:",
						realm.id,
						", k.id:",
						kingdom.id,
						"). "
					});
				}
				if (!string.IsNullOrEmpty(text))
				{
					Debug.LogWarning(text);
				}
			}
			settlement = settlement.Next();
		}
		RealmOriginPoint realmOriginPoint = RealmOriginPoint.First();
		while (realmOriginPoint != null)
		{
			string text2 = "";
			global::Realm realm2 = this.FindRealm(realmOriginPoint.realmName);
			if (realm2 == null)
			{
				text2 = text2 + "RealmOriginPoint " + realmOriginPoint.realmName + "does not have a coresponding realm";
			}
			else
			{
				list.Add(realm2.Name);
			}
			if (realm2 != null && realmOriginPoint.hasNegativeId && realm2.id > 0)
			{
				text2 = string.Concat(new object[]
				{
					text2,
					"RealmOriginPoint ",
					realmOriginPoint.realmName,
					" is marked for negative realm id, but its realm has id: ",
					realm2.id
				});
			}
			if (realm2 != null && !realmOriginPoint.hasNegativeId && realm2.id < 0)
			{
				text2 = string.Concat(new object[]
				{
					text2,
					"RealmOriginPoint ",
					realmOriginPoint.realmName,
					" is marked for positive realm id, but its realm has id: ",
					realm2.id
				});
			}
			if (realm2.kingdom != 0)
			{
				text2 = string.Concat(new object[]
				{
					text2,
					"RealmOriginPoints should not have kingdom id, but point ",
					realmOriginPoint.realmName,
					" has kingdom id: ",
					realm2.kingdom
				});
			}
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.LogWarning(text2);
			}
			realmOriginPoint = realmOriginPoint.Next();
		}
		for (int i = 0; i < this.Realms.Count; i++)
		{
			string text3 = "";
			global::Realm realm3 = this.Realms[i];
			global::Kingdom kingdom2 = this.FindKingdom(realm3.Name);
			if (!list.Contains(realm3.Name))
			{
				text3 = string.Concat(new object[]
				{
					text3,
					"Realm ",
					realm3.Name,
					"(",
					realm3.id,
					") exists without a castle or origin point."
				});
			}
			if (kingdom2 == null && !realm3.IsSeaRealm())
			{
				text3 = string.Concat(new object[]
				{
					text3,
					"Realm ",
					realm3.Name,
					"(",
					realm3.id,
					") exists without a kingdom."
				});
			}
			if (!string.IsNullOrEmpty(text3))
			{
				Debug.LogWarning(text3);
			}
		}
	}

	// Token: 0x060031D8 RID: 12760 RVA: 0x0019439C File Offset: 0x0019259C
	public void GenerateRealmsAndKingdoms()
	{
		global::Settlement settlement = global::Settlement.First();
		while (settlement != null)
		{
			if (settlement.IsCastle())
			{
				global::Realm realm = this.FindRealm(settlement.Name);
				if (realm != null)
				{
					if (realm.IsSeaRealm())
					{
						realm.id = -realm.id;
					}
					realm.CastlePos = settlement.transform.position;
				}
				else
				{
					realm = global::Realm.New(settlement.Name);
					realm.CastlePos = settlement.transform.position;
					global::Kingdom kingdom = global::Kingdom.New(settlement.Name);
					kingdom.MapColor = this.GetRandomColor();
					realm.kingdom = kingdom.id;
					realm.MapColor = kingdom.MapColor;
				}
			}
			settlement = settlement.Next();
		}
		RealmOriginPoint realmOriginPoint = RealmOriginPoint.First();
		while (realmOriginPoint != null)
		{
			global::Realm realm2 = this.FindRealm(realmOriginPoint.realmName);
			if (realm2 != null)
			{
				if (realmOriginPoint.hasNegativeId && realm2.id > 0)
				{
					realm2.id = -realm2.id;
				}
				if (!realmOriginPoint.hasNegativeId && realm2.id < 0)
				{
					realm2.id = -realm2.id;
				}
				realm2.CastlePos = realmOriginPoint.transform.position;
			}
			else
			{
				realm2 = global::Realm.New(realmOriginPoint.realmName);
				if (realmOriginPoint.hasNegativeId)
				{
					realm2.id = -realm2.id;
				}
				realm2.CastlePos = realmOriginPoint.transform.position;
				realm2.kingdom = 0;
				realm2.MapColor = this.GetRandomColor();
			}
			realmOriginPoint = realmOriginPoint.Next();
		}
		base.ReloadView();
	}

	// Token: 0x060031D9 RID: 12761 RVA: 0x000023FD File Offset: 0x000005FD
	public void GenerateRealmsData(bool fast = false)
	{
	}

	// Token: 0x060031DA RID: 12762 RVA: 0x000023FD File Offset: 0x000005FD
	public void GenerateGeographyFeatures()
	{
	}

	// Token: 0x060031DB RID: 12763 RVA: 0x00194538 File Offset: 0x00192738
	public Vector2Int WorldToRIDMap(float fx, float fz)
	{
		if (this.RealmIDMap == null)
		{
			return Vector2Int.zero;
		}
		Vector3 max = WorldMap.terrain.terrainData.bounds.max;
		int num = (int)(fx * (float)this.RealmsDataResolution / max.x);
		int num2 = (int)(fz * (float)this.RealmsDataResolution / max.z);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= this.RealmsDataResolution)
		{
			num = this.RealmsDataResolution - 1;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		else if (num2 >= this.RealmsDataResolution)
		{
			num2 = this.RealmsDataResolution - 1;
		}
		return new Vector2Int(num, num2);
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x001945CA File Offset: 0x001927CA
	public Vector2Int WorldToRIDMap(Vector3 ptw)
	{
		return this.WorldToRIDMap(ptw.x, ptw.z);
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x001945E0 File Offset: 0x001927E0
	public int RealmIDAt(float fx, float fz)
	{
		if (WorldMap.terrain == null || WorldMap.terrain.terrainData == null)
		{
			return 0;
		}
		Bounds bounds = WorldMap.terrain.terrainData.bounds;
		return this.RealmIDAt(fx, fz, bounds);
	}

	// Token: 0x060031DE RID: 12766 RVA: 0x00194628 File Offset: 0x00192828
	public int RealmIDAt(float fx, float fz, Bounds terrainBounds)
	{
		if (this.RealmIDMap == null)
		{
			return 0;
		}
		Vector3 max = terrainBounds.max;
		int num = (int)(fx * (float)this.RealmsDataResolution / max.x);
		int num2 = (int)(fz * (float)this.RealmsDataResolution / max.z);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= this.RealmsDataResolution)
		{
			num = this.RealmsDataResolution - 1;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		else if (num2 >= this.RealmsDataResolution)
		{
			num2 = this.RealmsDataResolution - 1;
		}
		return (int)this.RealmIDMap[num, num2];
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x001946AC File Offset: 0x001928AC
	public global::Realm RealmAt(float fx, float fz)
	{
		return global::Realm.Get(this.RealmIDAt(fx, fz));
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x001946BC File Offset: 0x001928BC
	private void LoadTextures()
	{
		string text = Path.GetDirectoryName(SceneManager.GetActiveScene().path).Replace('\\', '/');
		base.LoadTintTexture(text);
		this.RealmsDataTexture = Assets.Get<Texture2D>(text + "/realmsdata.png");
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x00194704 File Offset: 0x00192904
	public override Bounds GetTerrainBounds()
	{
		Terrain terrain = WorldMap.terrain;
		Bounds? bounds;
		if (terrain == null)
		{
			bounds = null;
		}
		else
		{
			TerrainData terrainData = terrain.terrainData;
			bounds = ((terrainData != null) ? new Bounds?(terrainData.bounds) : null);
		}
		Bounds? bounds2 = bounds;
		if (bounds2 == null)
		{
			return default(Bounds);
		}
		return bounds2.GetValueOrDefault();
	}

	// Token: 0x060031E2 RID: 12770 RVA: 0x0019475E File Offset: 0x0019295E
	private bool IsPV()
	{
		return ViewMode.current is PoliticalView;
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x00194770 File Offset: 0x00192970
	private void Init()
	{
		if (this.Inited)
		{
			return;
		}
		WorldMap.instance = this;
		WorldMap.terrain = Terrain.activeTerrain;
		this.Inited = true;
		try
		{
			using (Game.Profile("LoadingFMODBanks", false, 0f, null))
			{
				base.LoadFMODBanks("Title_Banks");
				base.LoadFMODBanks("WV_Banks");
				base.LoadFMODBanks("WV_BV_Banks");
			}
			Profile.BeginSection("LoadPrefabsDef");
			this.LoadPrefabsDef();
			Profile.EndSection("LoadPrefabsDef");
			Profile.BeginSection("LoadTextures");
			this.LoadTextures();
			Profile.EndSection("LoadTextures");
			Profile.BeginSection("GameLogic.Get");
			Game game = GameLogic.Get(true);
			Profile.EndSection("GameLogic.Get");
			string text = SceneManager.GetActiveScene().name.ToLowerInvariant();
			if (text != game.map_name)
			{
				Profile.BeginSection("Game.LoadMap");
				game.LoadMap(text, game.map_period, !Game.isLoadingSaveGame && !Game.isJoiningGame);
				Profile.EndSection("Game.LoadMap");
			}
			base.InitPAManager((game != null) ? game.path_finding : null);
			Profile.BeginSection("SpawnPoliticalMap");
			this.SpawnPoliticalMap();
			Profile.EndSection("SpawnPoliticalMap");
			Profile.BeginSection("LoadMapDef");
			base.LoadMapDef(game.dt, text);
			Profile.EndSection("LoadMapDef");
			Profile.BeginSection("LoadRealmsAndKingdoms");
			this.LoadRealmsAndKingdoms(game);
			Profile.EndSection("LoadRealmsAndKingdoms");
			Profile.BeginSection("LoadRealmIDMap");
			this.LoadRealmIDMap(game);
			Profile.EndSection("LoadRealmIDMap");
			Profile.BeginSection("LoadBorders");
			base.LoadBordersFromBinary();
			Profile.EndSection("LoadBorders");
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
			LoadingScreen loadingScreen = LoadingScreen.Get();
			if (loadingScreen != null)
			{
				loadingScreen.Show(false, true, false);
			}
		}
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x00194978 File Offset: 0x00192B78
	public void InitTextureBaker()
	{
		if (this.texture_baker == null)
		{
			this.texture_baker = new TextureBaker();
			this.texture_baker.SetKingdomColors(this.unit_colors);
			Traveler.LoadPrefabs(this.texture_baker);
		}
		if (this.texture_baker_ui == null)
		{
			this.texture_baker_ui = new TextureBaker();
			this.texture_baker_ui.SetKingdomColors(this.unit_colors);
		}
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x001949D8 File Offset: 0x00192BD8
	public void SpawnPoliticalMap()
	{
		if (this.PoliticalMapPrefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "PoliticalMap", true, true);
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		this.PoliticalMapObject = global::Common.Spawn(this.PoliticalMapPrefab, false, false);
		Tooltip.Get(this.PoliticalMapObject, true);
		this.PoliticalMapObject.transform.parent = base.transform;
		this.PoliticalMapObject.gameObject.SetActive(false);
	}

	// Token: 0x060031E6 RID: 12774 RVA: 0x00194A5D File Offset: 0x00192C5D
	private void OnApplicationQuit()
	{
		if (ViewMode.current != ViewMode.WorldView)
		{
			ViewMode.WorldView.Apply();
		}
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x00194A78 File Offset: 0x00192C78
	private void Update()
	{
		if (this.texture_baker != null)
		{
			GameCamera gameCamera = CameraController.GameCamera;
			this.texture_baker.Draw((gameCamera != null) ? gameCamera.Camera : null);
			this.texture_baker.ClearSkinningBuffers();
		}
		if (this.texture_baker_ui != null)
		{
			this.texture_baker_ui.Draw(UIBattleWindow.troops_camera);
			this.texture_baker_ui.ClearSkinningBuffers();
		}
	}

	// Token: 0x060031E8 RID: 12776 RVA: 0x000023FD File Offset: 0x000005FD
	public void Log(string msg)
	{
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x00194AD8 File Offset: 0x00192CD8
	private void OnEnable()
	{
		this.Log("OnEnable");
		if (!this.Inited || WorldMap.instance == null)
		{
			Profile.BeginSection("WorldMap.Init");
			this.Init();
			Profile.EndSection("WorldMap.Init");
		}
		if (Application.isPlaying)
		{
			Profile.BeginSection("WorldMap.LoadInstancedRenderers");
			this.InitTextureBaker();
			Profile.EndSection("WorldMap.LoadInstancedRenderers");
		}
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x000DE8C4 File Offset: 0x000DCAC4
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x00194B40 File Offset: 0x00192D40
	private void Start()
	{
		this.Log("Start");
		if (Application.isPlaying)
		{
			Profile.BeginSection("WorldMap.SaveGame.FinishLoading");
			SaveGame.FinishLoading();
			Profile.EndSection("WorldMap.SaveGame.FinishLoading");
			Profile.BeginSection("WorldMap.GenerateLabels");
			LabelUpdater labelUpdater = LabelUpdater.Get(true);
			if (labelUpdater != null)
			{
				labelUpdater.GenerateLabels(false);
			}
			Profile.EndSection("WorldMap.GenerateLabels");
			RichPresence.Update(RichPresence.State.InGame);
		}
		AutoSaveManager.Get();
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x00194BAC File Offset: 0x00192DAC
	public new void SetHighlightColors()
	{
		ViewMode current = ViewMode.current;
		if (current != null)
		{
			BordersBatching h_realm_borders = this.h_realm_borders;
			if (h_realm_borders != null)
			{
				LinesBatching.LineInfo line_info = h_realm_borders.line_info;
				if (line_info != null)
				{
					Material material = line_info.material;
					if (material != null)
					{
						material.SetColor("_Color", current.selected_highlight_color);
					}
				}
			}
			BordersBatching h_kingdom_borders = this.h_kingdom_borders;
			if (h_kingdom_borders == null)
			{
				return;
			}
			LinesBatching.LineInfo line_info2 = h_kingdom_borders.line_info;
			if (line_info2 == null)
			{
				return;
			}
			Material material2 = line_info2.material;
			if (material2 == null)
			{
				return;
			}
			material2.SetColor("_Color", current.selected_highlight_color);
		}
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x00194C24 File Offset: 0x00192E24
	private BordersBatching InitBorderData(string name)
	{
		Transform transform = base.transform.Find("BordersBatching");
		if (transform == null)
		{
			return null;
		}
		Transform transform2 = transform.Find(name);
		BordersBatching result;
		if (transform2 == null)
		{
			transform2 = new GameObject(name).transform;
			transform2.SetParent(transform);
			transform2.localPosition = Vector3.zero;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			result = transform2.gameObject.AddComponent<BordersBatching>();
		}
		else
		{
			result = transform2.GetComponent<BordersBatching>();
		}
		return result;
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x00194CA8 File Offset: 0x00192EA8
	private void OnDestroy()
	{
		this.Log("OnDestroy");
		base.UnloadFMODBanks("WV_Banks");
		if (this.texture_baker != null)
		{
			this.texture_baker.Dispose();
		}
		if (this.texture_baker_ui != null)
		{
			this.texture_baker_ui.Dispose();
		}
		PoliticalView politicalView;
		if ((politicalView = (ViewMode.current as PoliticalView)) != null)
		{
			politicalView.ToggleSnapshot(false);
		}
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x00194D06 File Offset: 0x00192F06
	private void OnValidate()
	{
		if (!this.Inited)
		{
			return;
		}
		base.ReloadView();
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x00194D17 File Offset: 0x00192F17
	private List<Vector3> CalcBorderPoints(global::Realm r, global::Realm.Neighbor n, int idx)
	{
		return n.BorderSegments[idx].points;
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x00194D2C File Offset: 0x00192F2C
	public static void GenerateTrapezoidData(List<Vector3> points, ref List<float> data, float right_offs = 0f)
	{
		if (data == null)
		{
			data = new List<float>();
		}
		if (points.Count < 2)
		{
			for (int i = 0; i < 3; i++)
			{
				data.Add(-1f);
			}
			return;
		}
		for (int j = 0; j < points.Count - 1; j++)
		{
			Vector3 vector = points[j];
			Vector3 vector2 = points[j + 1];
			if (right_offs > 0f)
			{
				Vector3 vector3 = vector2 - vector;
				vector3.y = 0f;
				Vector3 b = new Vector3(-vector3.y, 0f, vector3.x).normalized * right_offs;
				vector += b;
				vector2 += b;
			}
			Vector3 vector4 = (vector + vector2) / 2f;
			data.Add(vector4.x);
			data.Add(vector4.y);
			data.Add(vector4.z);
		}
		for (int k = 0; k < 3; k++)
		{
			data.Add(-1f);
		}
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x00194E44 File Offset: 0x00193044
	public static void GenerateRoadData(List<Vector3> points, ref List<float> data, out float len, float right_offs = 0f)
	{
		len = 0f;
		if (data == null)
		{
			data = new List<float>();
		}
		if (points.Count < 2)
		{
			for (int i = 0; i < 3; i++)
			{
				data.Add(-1f);
			}
			return;
		}
		for (int j = 0; j < points.Count - 1; j++)
		{
			Vector3 vector = points[j];
			Vector3 vector2 = points[j + 1];
			len += Vector3.Distance(vector, vector2);
			if (right_offs > 0f)
			{
				Vector3 vector3 = vector2 - vector;
				vector3.y = 0f;
				Vector3 b = new Vector3(-vector3.y, 0f, vector3.x).normalized * right_offs;
				vector += b;
				vector2 += b;
			}
			Vector3 vector4 = (vector + vector2) / 2f;
			data.Add(vector4.x);
			data.Add(vector4.y);
			data.Add(vector4.z);
		}
		for (int k = 0; k < 3; k++)
		{
			data.Add(-1f);
		}
	}

	// Token: 0x060031F3 RID: 12787 RVA: 0x00194F70 File Offset: 0x00193170
	public void GenerateRealmBordersBinaryData(bool check_duplicates = true)
	{
		List<float> list = new List<float>();
		List<float> list2 = new List<float>();
		int num = 0;
		List<Coord> list3 = new List<Coord>();
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm realm = this.Realms[i - 1];
			foreach (global::Realm.Neighbor neighbor in realm.Neighbors)
			{
				Coord coord = new Coord(realm.id, neighbor.rid);
				Coord item = new Coord(neighbor.rid, realm.id);
				if (check_duplicates)
				{
					if (list3.Contains(item))
					{
						continue;
					}
					list3.Add(new Coord(realm.id, neighbor.rid));
				}
				for (int j = 0; j < neighbor.BorderSegments.Count; j++)
				{
					list.Add((float)coord.x);
					list.Add((float)coord.y);
					list.Add(-1f);
					num++;
					List<Vector3> points = neighbor.BorderSegments[j].points;
					list2.Clear();
					WorldMap.GenerateTrapezoidData(points, ref list2, 0f);
					list.AddRange(list2);
				}
			}
		}
		Debug.Log("Borders processed: " + num);
		byte[] bytes = Serialization.ToBytes<float>(list.ToArray());
		string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		File.WriteAllBytes("Assets/Maps/" + str + "/RealmBordersTrapezoidData.bin", bytes);
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x00195124 File Offset: 0x00193324
	public void GenerateKingdomBordersBinaryData()
	{
		List<float> list = new List<float>();
		List<float> list2 = new List<float>();
		int num = 0;
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm realm = this.Realms[i - 1];
			foreach (global::Realm.Neighbor neighbor in realm.Neighbors)
			{
				Coord coord = new Coord(realm.id, neighbor.rid);
				for (int j = 0; j < neighbor.BorderSegments.Count; j++)
				{
					list.Add((float)coord.x);
					list.Add((float)coord.y);
					list.Add(-1f);
					num++;
					List<Vector3> points = neighbor.BorderSegments[j].points;
					list2.Clear();
					WorldMap.GenerateTrapezoidData(points, ref list2, 0f);
					list.AddRange(list2);
				}
			}
		}
		Debug.Log("Borders processed: " + num);
		byte[] bytes = Serialization.ToBytes<float>(list.ToArray());
		string str = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path).ToLowerInvariant();
		File.WriteAllBytes("Assets/Maps/" + str + "/KingdomBordersTrapezoidData.bin", bytes);
	}

	// Token: 0x060031F5 RID: 12789 RVA: 0x00195294 File Offset: 0x00193494
	public static bool IsFarFromLand(Point position, float treshold = 0.4f)
	{
		WorldMap worldMap = WorldMap.Get();
		Texture2D texture2D = (worldMap != null) ? worldMap.SDF_Tex : null;
		if (texture2D == null)
		{
			return false;
		}
		if (!global::Common.IsInWater(position, null, -1f))
		{
			return false;
		}
		Vector3 size = Terrain.activeTerrain.terrainData.size;
		return texture2D.GetPixel((int)((float)texture2D.width * position.x / size.x), (int)((float)texture2D.height * position.y / size.z)).r > treshold;
	}

	// Token: 0x04002151 RID: 8529
	public List<global::Kingdom> Kingdoms = new List<global::Kingdom>();

	// Token: 0x04002152 RID: 8530
	public short[,] RealmIDMap;

	// Token: 0x04002153 RID: 8531
	[HideInInspector]
	public int RealmsDataResolution = 2048;

	// Token: 0x04002154 RID: 8532
	public Texture2D RealmsDataTexture;

	// Token: 0x04002155 RID: 8533
	public TextureBaker texture_baker;

	// Token: 0x04002156 RID: 8534
	public TextureBaker texture_baker_ui;

	// Token: 0x04002157 RID: 8535
	public WorldMap.CBordersSettings BorderSettings = new WorldMap.CBordersSettings();

	// Token: 0x04002158 RID: 8536
	[HideInInspector]
	public GameObject PoliticalMapPrefab;

	// Token: 0x04002159 RID: 8537
	[HideInInspector]
	public GameObject PoliticalMapObject;

	// Token: 0x0400215A RID: 8538
	private static Terrain terrain = null;

	// Token: 0x0400215B RID: 8539
	private static WorldMap instance = null;

	// Token: 0x0400215C RID: 8540
	private static Color[] Colors = new Color[]
	{
		new Color32(70, 130, 180, byte.MaxValue),
		new Color32(46, 139, 87, byte.MaxValue),
		new Color32(154, 205, 50, byte.MaxValue),
		new Color32(107, 142, 35, byte.MaxValue),
		new Color32(237, 183, 61, byte.MaxValue),
		new Color32(237, 183, 61, byte.MaxValue),
		new Color32(222, 184, 135, byte.MaxValue),
		new Color32(218, 165, 32, byte.MaxValue),
		new Color32(byte.MaxValue, 160, 122, byte.MaxValue),
		new Color32(135, 206, 250, byte.MaxValue),
		new Color32(50, 205, 50, byte.MaxValue),
		new Color32(244, 164, 96, byte.MaxValue)
	};

	// Token: 0x0400215D RID: 8541
	private int UsedColors;

	// Token: 0x0400215E RID: 8542
	private bool Inited;

	// Token: 0x0400215F RID: 8543
	private static List<global::Realm> gizmo_realms = new List<global::Realm>();

	// Token: 0x02000882 RID: 2178
	[HideInInspector]
	public class CBordersSettings
	{
		// Token: 0x0600516E RID: 20846 RVA: 0x0023D4EC File Offset: 0x0023B6EC
		public void Save(DT.Field field)
		{
			field.SetValue("wv_realm_material", null, this.WVRealmMaterial).type = "material";
			field.SetValue("wv_realm_width", DT.FloatToStr(this.WVRealmWidth, int.MaxValue), null).type = "float";
			field.SetValue("wv_kingdom_material", null, this.WVKingdomMaterial).type = "material";
			field.SetValue("wv_kingdom_width", DT.FloatToStr(this.WVKingdomWidth, int.MaxValue), null).type = "float";
			field.SetValue("pv_realm_material", null, this.PVRealmMaterial).type = "material";
			field.SetValue("pv_realm_width", DT.FloatToStr(this.PVRealmWidth, int.MaxValue), null).type = "float";
			field.SetValue("pv_kingdom_material", null, this.PVKingdomMaterial).type = "material";
			field.SetValue("pv_kingdom_width", DT.FloatToStr(this.PVKingdomWidth, int.MaxValue), null).type = "float";
			field.SetValue("highlighted_realm_material", null, this.HighlightedRealmMaterial).type = "material";
			field.SetValue("highlighted_realm_width", DT.FloatToStr(this.HighlightedRealmWidth, int.MaxValue), null).type = "float";
			field.SetValue("highlighted_kingdom_material", null, this.HighlightedKingdomMaterial).type = "material";
			field.SetValue("highlighted_kingdom_width", DT.FloatToStr(this.HighlightedKingdomWidth, int.MaxValue), null).type = "float";
			field.SetValue("highlighted_PV_custom_material", null, this.HighlightedPVCustomMaterial).type = "material";
			field.SetValue("highlighted_PV_custom_width", DT.FloatToStr(this.HighlightedPVCustomWidth, int.MaxValue), null).type = "float";
		}

		// Token: 0x0600516F RID: 20847 RVA: 0x0023D6C8 File Offset: 0x0023B8C8
		public void Load(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			this.WVRealmMaterial = global::Defs.GetObj<Material>(field, "wv_realm_material", null);
			this.WVRealmWidth = field.GetFloat("wv_realm_width", null, this.WVRealmWidth, true, true, true, '.');
			this.WVKingdomMaterial = global::Defs.GetObj<Material>(field, "wv_kingdom_material", null);
			this.WVKingdomWidth = field.GetFloat("wv_kingdom_width", null, this.WVKingdomWidth, true, true, true, '.');
			this.PVRealmMaterial = global::Defs.GetObj<Material>(field, "pv_realm_material", null);
			this.PVRealmWidth = field.GetFloat("pv_realm_width", null, this.PVRealmWidth, true, true, true, '.');
			this.PVKingdomMaterial = global::Defs.GetObj<Material>(field, "pv_kingdom_material", null);
			this.PVKingdomWidth = field.GetFloat("pv_kingdom_width", null, this.PVKingdomWidth, true, true, true, '.');
			this.HighlightedRealmMaterial = global::Defs.GetObj<Material>(field, "highlighted_realm_material", null);
			this.HighlightedRealmWidth = field.GetFloat("highlighted_realm_width", null, this.HighlightedRealmWidth, true, true, true, '.');
			this.HighlightedKingdomMaterial = global::Defs.GetObj<Material>(field, "highlighted_kingdom_material", null);
			this.HighlightedKingdomWidth = field.GetFloat("highlighted_kingdom_width", null, this.HighlightedKingdomWidth, true, true, true, '.');
			this.HighlightedPVCustomMaterial = global::Defs.GetObj<Material>(field, "highlighted_PV_custom_material", null);
			this.HighlightedPVCustomWidth = field.GetFloat("highlighted_PV_custom_width", null, this.HighlightedKingdomWidth, true, true, true, '.');
		}

		// Token: 0x04003FC3 RID: 16323
		public Material WVRealmMaterial;

		// Token: 0x04003FC4 RID: 16324
		public float WVRealmWidth = 1f;

		// Token: 0x04003FC5 RID: 16325
		public Material WVKingdomMaterial;

		// Token: 0x04003FC6 RID: 16326
		public float WVKingdomWidth = 2f;

		// Token: 0x04003FC7 RID: 16327
		public Material PVRealmMaterial;

		// Token: 0x04003FC8 RID: 16328
		public float PVRealmWidth = 2f;

		// Token: 0x04003FC9 RID: 16329
		public Material PVKingdomMaterial;

		// Token: 0x04003FCA RID: 16330
		public float PVKingdomWidth = 4f;

		// Token: 0x04003FCB RID: 16331
		public Material HighlightedRealmMaterial;

		// Token: 0x04003FCC RID: 16332
		public float HighlightedRealmWidth = 2f;

		// Token: 0x04003FCD RID: 16333
		public Material HighlightedKingdomMaterial;

		// Token: 0x04003FCE RID: 16334
		public float HighlightedKingdomWidth = 4f;

		// Token: 0x04003FCF RID: 16335
		public Material HighlightedPVCustomMaterial;

		// Token: 0x04003FD0 RID: 16336
		public float HighlightedPVCustomWidth = 4f;
	}
}
