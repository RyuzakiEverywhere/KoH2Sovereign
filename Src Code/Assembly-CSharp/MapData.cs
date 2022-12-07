using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class MapData : MonoBehaviour
{
	// Token: 0x06000D2A RID: 3370 RVA: 0x00095A18 File Offset: 0x00093C18
	public static MapData Get()
	{
		BattleMap battleMap = BattleMap.Get();
		if (battleMap != null)
		{
			return battleMap;
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			return worldMap;
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			return titleMap;
		}
		return null;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00095A5C File Offset: 0x00093C5C
	public static MapData GetPoliticalOnly()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			return worldMap;
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			return titleMap;
		}
		return null;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00095A8C File Offset: 0x00093C8C
	public void ResetSDFShaders(Texture2D tex)
	{
		if (tex != null)
		{
			this.SDF_Tex = tex;
		}
		Shader.SetGlobalTexture("_SDF", this.SDF_Tex);
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00095AAE File Offset: 0x00093CAE
	public PassableAreaManager InitPAManager(Logic.PathFinding pf)
	{
		this.paManager = base.GetComponent<PassableAreaManager>();
		if (this.paManager == null)
		{
			this.paManager = base.gameObject.AddComponent<PassableAreaManager>();
		}
		this.paManager.pf = pf;
		return this.paManager;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00095AF0 File Offset: 0x00093CF0
	public void LoadFMODBanks(string key)
	{
		AudioLog.Info("Loading fmod bank " + key);
		DT.Field field = global::Defs.Get(false).dt.Find(key, null);
		if (field == null || field.children == null || field.children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			string key2 = field.children[i].key;
			if (!RuntimeManager.Instance.GetLoadedBanks().ContainsKey(key2))
			{
				RuntimeManager.LoadBank(key2, field.children[i].Bool(null, false));
			}
		}
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00095B8C File Offset: 0x00093D8C
	protected void UnloadFMODBanks(string key)
	{
		AudioLog.Info("Unloading fmod bank samples " + key);
		global::Defs defs = global::Defs.Get(false);
		DT dt = (defs != null) ? defs.dt : null;
		if (dt == null)
		{
			return;
		}
		DT.Field field = dt.Find(key, null);
		if (field == null || field.children == null || field.children.Count == 0)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			RuntimeManager.UnloadBankSamples(field.children[i].key);
		}
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00095C10 File Offset: 0x00093E10
	public static float GetWaterLevel()
	{
		MapData mapData = MapData.Get();
		if (mapData == null)
		{
			return 0f;
		}
		return mapData.TerrainHeights.WaterLevel;
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00095C3D File Offset: 0x00093E3D
	protected void LoadTintTexture(string dir)
	{
		this.TintTexture = Assets.Get<Texture2D>(dir + "/tint.png");
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00095C58 File Offset: 0x00093E58
	public void SetOverlayTexture(int idx)
	{
		PassableArea[] array = UnityEngine.Object.FindObjectsOfType<PassableArea>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetVisible(idx == 1);
		}
		if (idx <= 0)
		{
			ViewMode.WorldView.Apply();
			return;
		}
		ViewMode.Overlay.idx = idx - 1;
		ViewMode.Overlay.Apply();
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00095CAB File Offset: 0x00093EAB
	public void ReloadView()
	{
		if (ViewMode.current != null)
		{
			this.reload_view = true;
		}
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00095CBB File Offset: 0x00093EBB
	public void RegenerateSDF()
	{
		this.regenerate_sdf = true;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00095CC4 File Offset: 0x00093EC4
	protected void LoadMapDef(DT dt, string scene_name)
	{
		using (Game.Profile("MapData.LoadMapDef", false, 0f, null))
		{
			DT.Field field = dt.Find("Maps." + scene_name, null);
			if (field != null)
			{
				this.TerrainHeights.Load(field.FindChild("heights", null, true, true, true, '.'));
				this.starting_realm = field.GetString("starting_realm", null, "", true, true, true, '.');
				Profile.BeginSection("LoadUnitColors");
				this.LoadUnitColors();
				Profile.EndSection("LoadUnitColors");
			}
		}
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00095D70 File Offset: 0x00093F70
	public virtual Bounds GetTerrainBounds()
	{
		return default(Bounds);
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00095D88 File Offset: 0x00093F88
	public static List<global::Realm> GetRealms()
	{
		MapData mapData = MapData.Get();
		if (mapData is WorldMap)
		{
			return (mapData as WorldMap).Realms;
		}
		if (mapData is TitleMap)
		{
			return (mapData as TitleMap).Realms;
		}
		BattleMap battleMap = mapData as BattleMap;
		return null;
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00095DCC File Offset: 0x00093FCC
	public static List<global::Kingdom> GetKingdoms()
	{
		MapData mapData = MapData.Get();
		if (mapData is WorldMap)
		{
			return (mapData as WorldMap).Kingdoms;
		}
		if (mapData is TitleMap)
		{
			return (mapData as TitleMap).Kingdoms;
		}
		BattleMap battleMap = mapData as BattleMap;
		return null;
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00095E10 File Offset: 0x00094010
	private void LoadUnitColors()
	{
		DT.Field defField = global::Defs.GetDefField("Kingdom", null);
		if (defField == null)
		{
			return;
		}
		DT.Field field = defField.FindChild("unit_colors_texture", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		Texture2D texture2D = field.Value(null, true, true).Get<Texture2D>();
		if (texture2D == null)
		{
			return;
		}
		this.unit_colors = texture2D.GetPixels();
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00095E6C File Offset: 0x0009406C
	private void LateUpdate()
	{
		if (ViewMode.current != null && this.reload_view)
		{
			ViewMode.refresh_armies = true;
			ViewMode.current.Apply();
			ViewMode.refresh_armies = false;
			this.reload_view = false;
		}
		if (this.regenerate_sdf)
		{
			this.SDF_Tex = KingdomBorderSDFGenerator.Rebuild(this.SDF_Tex, 1, false, null, false);
			this.regenerate_sdf = false;
		}
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00095ECC File Offset: 0x000940CC
	public void RebuildNeighbors(Game game)
	{
		using (Game.Profile("TitleMap.RebuildNeighbors", false, 0f, null))
		{
			Transform transform = base.transform.Find("BordersBatching");
			BordersBatching bordersBatching;
			if (transform == null)
			{
				bordersBatching = null;
			}
			else
			{
				Transform transform2 = transform.Find("_h_realm_borders");
				bordersBatching = ((transform2 != null) ? transform2.GetComponent<BordersBatching>() : null);
			}
			BordersBatching bordersBatching2 = bordersBatching;
			if (!(bordersBatching2 == null))
			{
				bordersBatching2.LoadNeighbors(game);
				MapData.MarkDirtyBorders(true, true, true, false);
			}
		}
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00095F54 File Offset: 0x00094154
	private void OnEnable()
	{
		Profile.BeginSection("WorldMap.UpdateAllRealmBorders");
		this.UpdateAllRealmBorders();
		this.SDF_Tex = KingdomBorderSDFGenerator.Rebuild(this.SDF_Tex, 1, false, null, false);
		Profile.EndSection("WorldMap.UpdateAllRealmBorders");
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00095F85 File Offset: 0x00094185
	protected virtual void OnDisable()
	{
		KingdomBorderSDFGenerator.Dispose();
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00095F8C File Offset: 0x0009418C
	public void UpdateWVKingdomBorderColors(global::Realm r1, global::Realm r2, GameObject goBorder)
	{
		global::Kingdom kingdom = r1.GetKingdom();
		global::Kingdom kingdom2 = r2.GetKingdom();
		foreach (Renderer renderer in goBorder.GetComponentsInChildren<Renderer>())
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetColor("_TopColor", kingdom2.MapColor);
			materialPropertyBlock.SetColor("_BotColor", kingdom.MapColor);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00095FF9 File Offset: 0x000941F9
	public void ViewModeChanged()
	{
		this.UpdateAllRealmBorders();
		LabelUpdater.Get(true).UpdateLabels();
		this.SetHighlightColors();
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00096014 File Offset: 0x00094214
	public void SetHighlightColors()
	{
		ViewMode current = ViewMode.current;
		if (current != null)
		{
			BordersBatching bordersBatching = this.h_realm_borders;
			if (bordersBatching != null)
			{
				bordersBatching.mat.SetColor("_Color", current.selected_highlight_color);
			}
			BordersBatching bordersBatching2 = this.h_kingdom_borders;
			if (bordersBatching2 == null)
			{
				return;
			}
			bordersBatching2.mat.SetColor("_Color", current.selected_highlight_color);
		}
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0009606C File Offset: 0x0009426C
	public void LoadBordersFromBinary()
	{
		BordersBatching.FindBorderRenderers();
		BordersBatching bordersBatching = BordersBatching.FindBorderRenderer(base.transform, BordersBatching.BorderType.H_Realm_Border);
		if (bordersBatching == null)
		{
			return;
		}
		bordersBatching.LoadNeighbors(null);
		MapData.MarkDirtyBorders(true, true, true, false);
		this.UpdateAllRealmBorders();
		MapData.MarkDirtyBorders(false, true, false, true);
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x000960B4 File Offset: 0x000942B4
	public void UpdateAllRealmBorders()
	{
		bool bPV = ViewMode.IsPoliticalView();
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm r = this.Realms[i - 1];
			this.UpdateRealmBorders(r, bPV);
		}
		MapData.MarkDirtyBorders(false, false, false, false);
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x000960FD File Offset: 0x000942FD
	public void UpdateRealmBorders(global::Realm r)
	{
		this.UpdateRealmBorders(r, ViewMode.IsPoliticalView());
		MapData.MarkDirtyBorders(false, false, false, false);
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00096114 File Offset: 0x00094314
	public void UpdateRealmBorders(global::Realm r, bool bPV)
	{
		if (r.IsSeaRealm())
		{
			return;
		}
		ViewMode current = ViewMode.current;
		ViewMode.SelectionRules.Rule rule = ((current != null) ? current.GetSelectionRule() : null) ?? null;
		global::Kingdom kingdom = r.GetKingdom();
		bool flag = false;
		if (ViewMode.current != null)
		{
			flag = ViewMode.current.ShowRealmLabels();
		}
		foreach (global::Realm.Neighbor neighbor in r.Neighbors)
		{
			global::Realm realm = global::Realm.Get(neighbor.rid);
			if (realm != null)
			{
				bool flag2 = realm.IsSeaRealm();
				global::Kingdom kingdom2 = realm.GetKingdom();
				bool flag3 = r.id == this.selected_realm;
				bool flag4 = kingdom.id == this.SrcKingdom.id && (kingdom2 == null || kingdom2.id != kingdom.id);
				bool flag5 = kingdom != null && kingdom2 != null && kingdom2.id != kingdom.id;
				bool enabled = !bPV && !flag5;
				bool enabled2 = !bPV && flag5;
				bool enabled3 = bPV && !flag5;
				bool flag6 = bPV && flag5;
				bool flag7 = bPV && flag3 && flag;
				bool flag8 = bPV && flag4 && (rule == null || rule.show_kingdom_border);
				BordersBatching bordersBatching = this.wv_realm_borders;
				if (bordersBatching != null)
				{
					bordersBatching.SetActiveBorder(r, realm, enabled, flag2);
				}
				BordersBatching bordersBatching2 = this.wv_kingdom_borders;
				if (bordersBatching2 != null)
				{
					bordersBatching2.SetActiveBorder(r, realm, enabled2, flag2);
				}
				this.pv_realm_borders.SetActiveBorder(r, realm, enabled3, flag2);
				this.pv_kingdom_borders.SetActiveBorder(r, realm, flag6 || flag8 || (flag7 && flag2), flag2 && !flag7 && !flag8);
				this.h_realm_borders.SetActiveBorder(r, realm, flag7, flag2 && !flag7);
				this.h_kingdom_borders.SetActiveBorder(r, realm, flag8, flag2 && !flag8);
			}
		}
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00096320 File Offset: 0x00094520
	public void UpdateSelectedBorders()
	{
		ViewMode current = ViewMode.current;
		ViewMode.SelectionRules.Rule rule = ((current != null) ? current.GetSelectionRule() : null) ?? null;
		bool flag = ViewMode.IsPoliticalView();
		bool flag2 = false;
		if (ViewMode.current != null)
		{
			flag2 = ViewMode.current.ShowRealmLabels();
		}
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm realm = this.Realms[i - 1];
			if (!realm.IsSeaRealm())
			{
				global::Kingdom kingdom = realm.GetKingdom();
				foreach (global::Realm.Neighbor neighbor in realm.Neighbors)
				{
					global::Realm realm2 = global::Realm.Get(neighbor.rid);
					if (realm2 != null)
					{
						bool flag3 = realm2.IsSeaRealm();
						global::Kingdom kingdom2 = realm2.GetKingdom();
						bool flag4 = realm.id == this.selected_realm;
						bool flag5 = kingdom.id == this.SrcKingdom.id && (kingdom2 == null || kingdom2.id != kingdom.id);
						bool flag6 = kingdom != null && kingdom2 != null && kingdom2.id != kingdom.id;
						bool enabled = flag && !flag6;
						bool flag7 = flag && flag6;
						bool flag8 = flag && flag4 && (flag2 || (rule != null && rule.show_realm_border));
						bool flag9 = flag && flag5 && (rule == null || rule.show_kingdom_border);
						this.pv_realm_borders.SetActiveBorder(realm, realm2, enabled, flag3);
						this.pv_kingdom_borders.SetActiveBorder(realm, realm2, flag7 || flag9 || (flag8 && flag3), flag3 && !flag8 && !flag9);
						this.h_realm_borders.SetActiveBorder(realm, realm2, flag8, flag3 && !flag8);
						this.h_kingdom_borders.SetActiveBorder(realm, realm2, flag9, flag3 && !flag9);
					}
				}
			}
		}
		if (this.h_kingdom_borders == null || this.h_realm_borders == null)
		{
			MapData.MarkDirtyBorders(false, false, false, false);
			return;
		}
		this.pv_kingdom_borders.MarkDirty(false, true, false, false);
		this.pv_realm_borders.MarkDirty(false, true, false, false);
		this.h_kingdom_borders.MarkDirty(false, true, false, false);
		this.h_realm_borders.MarkDirty(false, true, false, false);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x000965A0 File Offset: 0x000947A0
	public void ClearSelectedBorders()
	{
		for (int i = 1; i <= this.Realms.Count; i++)
		{
			global::Realm realm = this.Realms[i - 1];
			foreach (global::Realm.Neighbor neighbor in realm.Neighbors)
			{
				global::Realm realm2 = global::Realm.Get(neighbor.rid);
				bool enabled_water = realm2.IsSeaRealm();
				this.h_realm_borders.SetActiveBorder(realm, realm2, false, enabled_water);
				this.h_kingdom_borders.SetActiveBorder(realm, realm2, false, enabled_water);
			}
		}
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00096648 File Offset: 0x00094848
	public void SetHighlighedRealm(global::Realm r)
	{
		int num = (r == null) ? 0 : r.id;
		this.prev_highlighted_realm = this.highlighted_realm;
		if (this.highlighted_realm == num)
		{
			return;
		}
		this.highlighted_realm = num;
		Shader.SetGlobalInt("_HighlightedRealm", this.highlighted_realm);
		if (ViewMode.IsPoliticalView())
		{
			this.ReloadView();
		}
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x0009669C File Offset: 0x0009489C
	public void SetSelectedRealm(int rid)
	{
		if (this.selected_realm == rid)
		{
			return;
		}
		this.selected_realm = rid;
		GameLogic.Get(true);
		Shader.SetGlobalInt("_SelectedRealm", this.selected_realm);
		if (ViewMode.IsPoliticalView())
		{
			this.ReloadView();
			this.UpdateSelectedBorders();
		}
		if (rid != 0)
		{
			Logic.Realm realm = GameLogic.Get(true).GetRealm(rid);
			if (realm.GetKingdom() == BaseUI.LogicKingdom())
			{
				string key = "SelOwnRealmTrigger";
				Religion religion = realm.religion;
				BackgroundMusic.OnTrigger(key, (religion != null) ? religion.name : null);
				return;
			}
			string key2 = "SelOtherRealmTrigger";
			Religion religion2 = realm.religion;
			BackgroundMusic.OnTrigger(key2, (religion2 != null) ? religion2.name : null);
		}
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0009673C File Offset: 0x0009493C
	public void SetSrcKingdom(global::Kingdom k, bool reload_view = true)
	{
		int num = (k == null) ? 0 : k.id;
		bool flag = this.SrcKingdom == num;
		this.SrcKingdom = num;
		if (reload_view)
		{
			this.UpdateSelectedBorders();
		}
		if (flag)
		{
			return;
		}
		if (reload_view)
		{
			this.ReloadView();
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00096788 File Offset: 0x00094988
	public static void UpdateSrcKingdom(bool reload_view = true)
	{
		MapData mapData = MapData.Get();
		if (mapData == null)
		{
			return;
		}
		if (!ViewMode.IsPoliticalView())
		{
			mapData.SetSrcKingdom(null, reload_view);
			return;
		}
		if (!reload_view && mapData.SrcKingdom == 0)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.SelKingdom() ?? BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		global::Kingdom kingdom2 = kingdom.visuals as global::Kingdom;
		if (kingdom2 == null)
		{
			return;
		}
		mapData.SetSrcKingdom(kingdom2, reload_view);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x000967F4 File Offset: 0x000949F4
	public static void MarkDirtyBorders(bool reload = false, bool force = false, bool instant = false, bool recreate_buffers = false)
	{
		MapData mapData = MapData.Get();
		if (mapData == null)
		{
			return;
		}
		if (((mapData != null) ? mapData.wv_realm_borders : null) != null && (mapData.wv_realm_borders.dirty || force))
		{
			mapData.wv_realm_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
		if (((mapData != null) ? mapData.wv_kingdom_borders : null) != null && (mapData.wv_kingdom_borders.dirty || force))
		{
			mapData.wv_kingdom_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
		if (((mapData != null) ? mapData.pv_realm_borders : null) != null && (mapData.pv_realm_borders.dirty || force))
		{
			mapData.pv_realm_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
		if (((mapData != null) ? mapData.pv_kingdom_borders : null) != null && (mapData.pv_kingdom_borders.dirty || force))
		{
			mapData.pv_kingdom_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
		if (((mapData != null) ? mapData.h_realm_borders : null) != null && (mapData.h_realm_borders.dirty || force))
		{
			mapData.h_realm_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
		if (((mapData != null) ? mapData.h_kingdom_borders : null) != null && (mapData.h_kingdom_borders.dirty || force))
		{
			mapData.h_kingdom_borders.MarkDirty(reload, true, instant, recreate_buffers);
		}
	}

	// Token: 0x04000A24 RID: 2596
	[HideInInspector]
	public global::Kingdom.ID SrcKingdom = 1;

	// Token: 0x04000A25 RID: 2597
	public Texture2D TintTexture;

	// Token: 0x04000A26 RID: 2598
	public Texture2D[] OverlayTextures = new Texture2D[3];

	// Token: 0x04000A27 RID: 2599
	[Range(0f, 1f)]
	public float OverlayAlpha = 1f;

	// Token: 0x04000A28 RID: 2600
	public PassableAreaManager paManager;

	// Token: 0x04000A29 RID: 2601
	public Texture2D SDF_Tex;

	// Token: 0x04000A2A RID: 2602
	private bool reload_view;

	// Token: 0x04000A2B RID: 2603
	private bool regenerate_sdf;

	// Token: 0x04000A2C RID: 2604
	public List<global::Realm> Realms = new List<global::Realm>();

	// Token: 0x04000A2D RID: 2605
	public int highlighted_realm;

	// Token: 0x04000A2E RID: 2606
	public int prev_highlighted_realm;

	// Token: 0x04000A2F RID: 2607
	[HideInInspector]
	public Color[] unit_colors;

	// Token: 0x04000A30 RID: 2608
	public MapData.CTerrainHeights TerrainHeights = new MapData.CTerrainHeights();

	// Token: 0x04000A31 RID: 2609
	public string starting_realm;

	// Token: 0x04000A32 RID: 2610
	public int selected_realm;

	// Token: 0x04000A33 RID: 2611
	[NonSerialized]
	public BordersBatching wv_realm_borders;

	// Token: 0x04000A34 RID: 2612
	[NonSerialized]
	public BordersBatching wv_kingdom_borders;

	// Token: 0x04000A35 RID: 2613
	[NonSerialized]
	public BordersBatching pv_realm_borders;

	// Token: 0x04000A36 RID: 2614
	[NonSerialized]
	public BordersBatching pv_kingdom_borders;

	// Token: 0x04000A37 RID: 2615
	[NonSerialized]
	public BordersBatching h_realm_borders;

	// Token: 0x04000A38 RID: 2616
	[NonSerialized]
	public BordersBatching h_kingdom_borders;

	// Token: 0x02000631 RID: 1585
	[Serializable]
	public class CTerrainHeights
	{
		// Token: 0x0600471E RID: 18206 RVA: 0x00212860 File Offset: 0x00210A60
		public void Save(DT.Field field)
		{
			field.SetValue("water_level", DT.FloatToStr(this.WaterLevel, int.MaxValue), null);
			field.SetValue("beach_min", DT.FloatToStr(this.BeachMin, int.MaxValue), null);
			field.SetValue("beach_max", DT.FloatToStr(this.BeachMax, int.MaxValue), null);
			field.SetValue("hills_min_delta", DT.FloatToStr(this.HillsDeltaMin, int.MaxValue), null);
			field.SetValue("hills_max_delta", DT.FloatToStr(this.HillsDeltaMax, int.MaxValue), null);
			field.SetValue("mountains_min", DT.FloatToStr(this.MountainsMin, int.MaxValue), null);
			field.SetValue("mountains_max", DT.FloatToStr(this.MountainsMax, int.MaxValue), null);
		}

		// Token: 0x0600471F RID: 18207 RVA: 0x00212938 File Offset: 0x00210B38
		public void Load(DT.Field field)
		{
			if (field == null)
			{
				return;
			}
			this.WaterLevel = field.GetFloat("water_level", null, this.WaterLevel, true, true, true, '.');
			this.BeachMin = field.GetFloat("beach_min", null, this.BeachMin, true, true, true, '.');
			this.BeachMax = field.GetFloat("beach_max", null, this.BeachMax, true, true, true, '.');
			this.HillsDeltaMin = field.GetFloat("hills_min_delta", null, this.HillsDeltaMin, true, true, true, '.');
			this.HillsDeltaMax = field.GetFloat("hills_max_delta", null, this.HillsDeltaMax, true, true, true, '.');
			this.MountainsMin = field.GetFloat("mountains_min", null, this.MountainsMin, true, true, true, '.');
			this.MountainsMax = field.GetFloat("mountains_max", null, this.MountainsMax, true, true, true, '.');
		}

		// Token: 0x04003477 RID: 13431
		public float WaterLevel = 9f;

		// Token: 0x04003478 RID: 13432
		public float BeachMin = 9.3f;

		// Token: 0x04003479 RID: 13433
		public float BeachMax = 9.7f;

		// Token: 0x0400347A RID: 13434
		public float HillsDeltaMin = 0.2f;

		// Token: 0x0400347B RID: 13435
		public float HillsDeltaMax = 1f;

		// Token: 0x0400347C RID: 13436
		public float MountainsMin = 30f;

		// Token: 0x0400347D RID: 13437
		public float MountainsMax = 32f;
	}
}
