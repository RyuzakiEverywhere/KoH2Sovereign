using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logic;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class TitleMap : MapData
{
	// Token: 0x060015C5 RID: 5573 RVA: 0x000DDE83 File Offset: 0x000DC083
	public new static TitleMap Get()
	{
		return TitleMap.instance;
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000DDE8C File Offset: 0x000DC08C
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

	// Token: 0x060015C7 RID: 5575 RVA: 0x000DDED8 File Offset: 0x000DC0D8
	private void LoadKingdomDefs(Game game)
	{
		using (Game.Profile("TitleMap.LoadKingdomDefs", false, 0f, null))
		{
			this.Kingdoms.Clear();
			if (game != null && game.kingdoms != null)
			{
				for (int i = 0; i < game.kingdoms.Count; i++)
				{
					Logic.Kingdom kingdom = game.kingdoms[i];
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
		}
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x000DDFA8 File Offset: 0x000DC1A8
	private void LoadRealmDefs(Game game)
	{
		using (Game.Profile("TitleMap.LoadRealmDefs", false, 0f, null))
		{
			this.Realms.Clear();
			if (game != null && game.realms != null)
			{
				for (int i = 0; i < game.realms.Count; i++)
				{
					Logic.Realm realm = game.realms[i];
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
				this.LoadRealmBounds(game.map_name);
			}
		}
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000DE0A8 File Offset: 0x000DC2A8
	public bool LoadRealmBounds(string mapName)
	{
		bool result;
		using (Game.Profile("TitleMap.LoadRealmBounds", false, 0f, null))
		{
			string maps_path = Game.maps_path;
			if (File.Exists(maps_path + mapName + "/RealmBounds.bin"))
			{
				List<float> list = null;
				using (Stream stream = File.Open(maps_path + mapName + "/RealmBounds.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					list = (List<float>)new BinaryFormatter().Deserialize(stream);
				}
				if (list != null && this.Realms != null)
				{
					for (int i = 0; i < list.Count; i += 6)
					{
						Vector3 min = new Vector3(list[i], list[i + 1], list[i + 2]);
						Vector3 max = new Vector3(list[i + 3], list[i + 4], list[i + 5]);
						if (i / 6 < this.Realms.Count)
						{
							global::Realm realm = this.Realms[i / 6];
							realm.bounds = default(Bounds);
							realm.bounds.SetMinMax(min, max);
						}
					}
				}
				result = true;
			}
			else
			{
				Debug.LogError("Warning: no realmbounds found at: " + maps_path + mapName + "/RealmBounds.bin   Please regenerate and analyze the realm data");
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x000DE22C File Offset: 0x000DC42C
	public void LoadRealmsAndKingdoms(Game game)
	{
		using (Game.Profile("TitleMap.LoadRealmsAndKingdoms", false, 0f, null))
		{
			this.LoadRealmDefs(game);
			this.LoadKingdomDefs(game);
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000DE27C File Offset: 0x000DC47C
	public void LoadRealmIDMap(Game game)
	{
		using (Game.Profile("TitleMap.LoadRealmIDMap", false, 0f, null))
		{
			this.RealmIDMap = game.realm_id_map;
			this.RealmsDataResolution = ((this.RealmIDMap == null) ? 0 : this.RealmIDMap.GetLength(0));
			base.ReloadView();
		}
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x000DE2EC File Offset: 0x000DC4EC
	public override Bounds GetTerrainBounds()
	{
		return this.GetTerrainBounds(GameLogic.Get(false));
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000DE2FC File Offset: 0x000DC4FC
	public Bounds GetTerrainBounds(Game game)
	{
		if (game == null || game.dt == null)
		{
			return default(Bounds);
		}
		DT.Field defField = global::Defs.GetDefField("Maps." + game.map_name, null);
		Point point = new Point(3750f, 2625f);
		if (defField != null)
		{
			point = defField.GetPoint("size", null, true, true, true, '.');
		}
		Vector3 center = new Vector3(point.x / 2f, 20f, point.y / 2f);
		Vector3 size = new Vector3(point.x, 40f, point.y);
		return new Bounds(center, size);
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000DE39C File Offset: 0x000DC59C
	public Vector2Int WorldToRIDMap(float fx, float fz)
	{
		if (this.RealmIDMap == null)
		{
			return Vector2Int.zero;
		}
		Vector3 max = this.GetTerrainBounds().max;
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

	// Token: 0x060015CF RID: 5583 RVA: 0x000DE425 File Offset: 0x000DC625
	public Vector2Int WorldToRIDMap(Vector3 ptw)
	{
		return this.WorldToRIDMap(ptw.x, ptw.z);
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x000DE43C File Offset: 0x000DC63C
	public int RealmIDAt(float fx, float fz)
	{
		Bounds terrainBounds = this.GetTerrainBounds();
		return this.RealmIDAt(fx, fz, terrainBounds);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x000DE45C File Offset: 0x000DC65C
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

	// Token: 0x060015D2 RID: 5586 RVA: 0x000DE4E0 File Offset: 0x000DC6E0
	public global::Realm RealmAt(float fx, float fz)
	{
		return global::Realm.Get(this.RealmIDAt(fx, fz));
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000DE4F0 File Offset: 0x000DC6F0
	private void LoadTextures(string map_name)
	{
		using (Game.Profile("TitleMap.LoadTextures", false, 0f, null))
		{
			DT.Field field = GameLogic.Get(true).dt.Find("Maps." + map_name, null);
			this.RealmsDataTexture = global::Defs.GetObj<Texture2D>(field, "realms_data", null);
			if (this.RealmsDataTexture == null)
			{
				Debug.LogWarning("Fail to find realmsdata.png for " + map_name);
			}
		}
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x000DE57C File Offset: 0x000DC77C
	private void Awake()
	{
		TitleMap.instance = this;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x000DE584 File Offset: 0x000DC784
	public void Init()
	{
		if (this.Inited)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		this.LoadPrefabsDef();
		this.Inited = true;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x000DE5A4 File Offset: 0x000DC7A4
	public void LoadGame(Game game, bool ignore_labels = false)
	{
		this.Init();
		if (game == null)
		{
			Debug.Log("Missing game data on game load.");
			return;
		}
		using (Game.Profile("TitleMap.LoadGame", false, 0f, null))
		{
			this.LoadTextures(game.map_name);
			this.SpawnPoliticalMap(game);
			base.LoadMapDef(game.dt, game.map_name);
			this.LoadRealmsAndKingdoms(game);
			this.LoadRealmIDMap(game);
			base.LoadBordersFromBinary();
			BaseUI.Get<TitleUI>().SelectKingdom(0, true);
			if (!ignore_labels)
			{
				LabelUpdater.Get(true).GenerateLabels(false);
			}
		}
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x000DE64C File Offset: 0x000DC84C
	public void UpdateGamePoliticalData(Game game)
	{
		using (Game.Profile("TitleMap.UpdateGamePoliticalData", false, 0f, null))
		{
			LabelUpdater.Get(true).AbortThreads();
			this.LoadRealmsAndKingdoms(game);
			base.SetHighlighedRealm(null);
			base.SetSelectedRealm(0);
			base.SetSrcKingdom(null, true);
			TitleMap titleMap = TitleMap.Get();
			if (titleMap != null)
			{
				titleMap.ClearSelectedBorders();
			}
			base.RebuildNeighbors(game);
			this.LoadRealmIDMap(game);
			ViewMode.titleKingdomsView.Apply(true);
			this.SDF_Tex = KingdomBorderSDFGenerator.Rebuild(this.SDF_Tex, 1, false, null, false);
			base.UpdateAllRealmBorders();
			base.UpdateSelectedBorders();
			LabelUpdater labelUpdater = LabelUpdater.Get(true);
			if (labelUpdater != null)
			{
				labelUpdater.SetThreads();
			}
			LabelUpdater.Get(true).GenerateLabels(false);
			LabelUpdater.Get(true).UpdateLabels();
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x000DE728 File Offset: 0x000DC928
	public void SpawnPoliticalMap(Game game)
	{
		if (this.PoliticalMapPrefab == null)
		{
			return;
		}
		using (Game.Profile("TitleMap.SpawnPoliticalMap", false, 0f, null))
		{
			GameObject gameObject = global::Common.FindChildByName(base.gameObject, "PoliticalMap", true, true);
			if (gameObject != null)
			{
				global::Common.DestroyObj(gameObject);
			}
			this.PoliticalMapObject = global::Common.Spawn(this.PoliticalMapPrefab, false, false);
			this.PoliticalMapObject.name = "PoliticalMap";
			this.PoliticalMapObject.transform.parent = base.transform;
			DT.Field field = game.dt.Find("Maps." + game.map_name, null);
			Point point = new Point(3750f, 2625f);
			if (field != null)
			{
				point = field.GetPoint("size", null, true, true, true, '.');
			}
			this.PoliticalMapObject.transform.localScale = new Vector3(point.x, point.y, 1f);
			this.PoliticalMapObject.transform.localPosition = new Vector3(point.x / 2f, -0.1f, point.y / 2f);
			this.PoliticalMapObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
			this.PoliticalMapObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000DE8B0 File Offset: 0x000DCAB0
	private void OnEnable()
	{
		TitleMap.instance = this;
		if (Application.isPlaying)
		{
			BackgroundMusic.Reset();
		}
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x000DE8C4 File Offset: 0x000DCAC4
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x000DE8CC File Offset: 0x000DCACC
	private void Start()
	{
		if (Application.isPlaying)
		{
			SaveGame.FinishLoading();
		}
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000DE8DC File Offset: 0x000DCADC
	public static void InitBorderData(bool reload = false, bool force = false)
	{
		TitleMap titleMap = TitleMap.Get();
		if (titleMap == null)
		{
			return;
		}
		using (Game.Profile("TitleMap.InitBorderData", false, 0f, null))
		{
			if (titleMap.pv_realm_borders == null)
			{
				titleMap.pv_realm_borders = titleMap.InitBorderData("_pv_realm_borders");
			}
			if (titleMap.pv_kingdom_borders == null)
			{
				titleMap.pv_kingdom_borders = titleMap.InitBorderData("_pv_kingdom_borders");
			}
			if (titleMap.h_realm_borders == null)
			{
				titleMap.h_realm_borders = titleMap.InitBorderData("_h_realm_borders");
			}
			if (titleMap.h_kingdom_borders == null)
			{
				titleMap.h_kingdom_borders = titleMap.InitBorderData("_h_kingdom_borders");
			}
			if (titleMap.pv_realm_borders.dirty || force)
			{
				BordersBatching pv_realm_borders = titleMap.pv_realm_borders;
				if (pv_realm_borders != null)
				{
					pv_realm_borders.MarkDirty(reload, true, false, false);
				}
			}
			if (titleMap.pv_kingdom_borders.dirty || force)
			{
				BordersBatching pv_kingdom_borders = titleMap.pv_kingdom_borders;
				if (pv_kingdom_borders != null)
				{
					pv_kingdom_borders.MarkDirty(reload, true, false, false);
				}
			}
			if (titleMap.h_realm_borders.dirty || force)
			{
				BordersBatching h_realm_borders = titleMap.h_realm_borders;
				if (h_realm_borders != null)
				{
					h_realm_borders.MarkDirty(reload, true, false, false);
				}
			}
			if (titleMap.h_kingdom_borders.dirty || force)
			{
				BordersBatching h_kingdom_borders = titleMap.h_kingdom_borders;
				if (h_kingdom_borders != null)
				{
					h_kingdom_borders.MarkDirty(reload, true, false, false);
				}
			}
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x000DEA44 File Offset: 0x000DCC44
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

	// Token: 0x060015DE RID: 5598 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x04000E20 RID: 3616
	public List<global::Kingdom> Kingdoms = new List<global::Kingdom>();

	// Token: 0x04000E21 RID: 3617
	public short[,] RealmIDMap;

	// Token: 0x04000E22 RID: 3618
	[HideInInspector]
	public int RealmsDataResolution = 2048;

	// Token: 0x04000E23 RID: 3619
	public Texture2D RealmsDataTexture;

	// Token: 0x04000E24 RID: 3620
	public TitleMap.CBordersSettings BorderSettings = new TitleMap.CBordersSettings();

	// Token: 0x04000E25 RID: 3621
	[HideInInspector]
	public GameObject PoliticalMapPrefab;

	// Token: 0x04000E26 RID: 3622
	[HideInInspector]
	public GameObject PoliticalMapObject;

	// Token: 0x04000E27 RID: 3623
	private static TitleMap instance;

	// Token: 0x04000E28 RID: 3624
	private bool Inited;

	// Token: 0x020006C5 RID: 1733
	public class CBordersSettings
	{
		// Token: 0x0600488D RID: 18573 RVA: 0x00218010 File Offset: 0x00216210
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

		// Token: 0x040036E8 RID: 14056
		public Material WVRealmMaterial;

		// Token: 0x040036E9 RID: 14057
		public float WVRealmWidth = 1f;

		// Token: 0x040036EA RID: 14058
		public Material WVKingdomMaterial;

		// Token: 0x040036EB RID: 14059
		public float WVKingdomWidth = 2f;

		// Token: 0x040036EC RID: 14060
		public Material PVRealmMaterial;

		// Token: 0x040036ED RID: 14061
		public float PVRealmWidth = 2f;

		// Token: 0x040036EE RID: 14062
		public Material PVKingdomMaterial;

		// Token: 0x040036EF RID: 14063
		public float PVKingdomWidth = 4f;

		// Token: 0x040036F0 RID: 14064
		public Material HighlightedRealmMaterial;

		// Token: 0x040036F1 RID: 14065
		public float HighlightedRealmWidth = 2f;

		// Token: 0x040036F2 RID: 14066
		public Material HighlightedKingdomMaterial;

		// Token: 0x040036F3 RID: 14067
		public float HighlightedKingdomWidth = 4f;

		// Token: 0x040036F4 RID: 14068
		public Material HighlightedPVCustomMaterial;

		// Token: 0x040036F5 RID: 14069
		public float HighlightedPVCustomWidth = 4f;
	}
}
