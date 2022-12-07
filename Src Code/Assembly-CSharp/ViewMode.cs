using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public abstract class ViewMode
{
	// Token: 0x06000B6A RID: 2922 RVA: 0x00081120 File Offset: 0x0007F320
	public static void SetFilter(ViewMode.AllowedFigures filter)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("pv_filter");
		if (setting == null)
		{
			return;
		}
		setting.ApplyValue((int)filter);
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0008113C File Offset: 0x0007F33C
	public static void ApplyFilter(ViewMode.AllowedFigures filter)
	{
		ViewMode.figuresFilter = filter;
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (game.kingdoms != null)
		{
			for (int i = 0; i < game.kingdoms.Count; i++)
			{
				Logic.Kingdom kingdom = game.kingdoms[i];
				if (!kingdom.IsDefeated() || kingdom.type != Logic.Kingdom.Type.Regular)
				{
					for (int j = 0; j < kingdom.armies.Count; j++)
					{
						Logic.Army army = kingdom.armies[j];
						global::Army army2 = army.visuals as global::Army;
						if (army2 != null)
						{
							army2.UpdateVisibility(false);
						}
						if (army.battle != null)
						{
							global::Battle battle = army.battle.visuals as global::Battle;
							if (battle != null)
							{
								battle.UpdateVisibility();
							}
						}
					}
				}
			}
		}
		if (game.realms != null)
		{
			for (int k = 0; k < game.realms.Count; k++)
			{
				Logic.Realm realm = game.realms[k];
				for (int l = 0; l < realm.settlements.Count; l++)
				{
					global::Settlement settlement = realm.settlements[l].visuals as global::Settlement;
					if (settlement != null)
					{
						UIPVFigureSettlement ui_pvFigure = settlement.ui_pvFigure;
						if (ui_pvFigure != null)
						{
							ui_pvFigure.UpdateVisibilityFilter();
						}
					}
				}
			}
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0008128C File Offset: 0x0007F48C
	public virtual void LoadDef(DT.Field field)
	{
		this.def = field;
		this.terrain_material = global::Defs.GetObj<Material>(field, "terrain_material", null);
		this.filter = global::Defs.GetObj<GameObject>(field, "filter", null);
		if (field != null)
		{
			this.highlightTarget = field.GetBool("highlight", null, false, true, true, true, '.');
			this.allowedFigures = ViewMode.AllowedFigures.None;
			if (field.GetBool("allowed_figures_army", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Army;
			}
			if (field.GetBool("allowed_figures_mercenary", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Mercenary;
			}
			if (field.GetBool("allowed_figures_town", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Castle;
			}
			if (field.GetBool("allowed_figures_battle", null, false, true, true, true, '.'))
			{
				this.allowedFigures |= ViewMode.AllowedFigures.Battle;
			}
			this.selectionRules.LoadDef(field.FindChild("selection_rules", null, true, true, true, '.'));
			this.current_sub_mode = this.selectionRules.preferred_sub_mode;
			this.selected_highlight_color = global::Defs.GetColor(field, "selection_highlight_color", null);
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000813B0 File Offset: 0x0007F5B0
	public virtual ViewMode.SubMode GetSubMode()
	{
		return this.current_sub_mode;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x000813B8 File Offset: 0x0007F5B8
	public virtual void SetSubMode(ViewMode.SubMode mode)
	{
		if (this.current_sub_mode == mode)
		{
			return;
		}
		this.current_sub_mode = mode;
		ViewMode.current.Apply(true);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x000813D6 File Offset: 0x0007F5D6
	public virtual bool ShowRealmLabels()
	{
		return this.current_sub_mode == ViewMode.SubMode.Realm;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000813E1 File Offset: 0x0007F5E1
	public virtual ViewMode.SelectionRules.Rule GetSelectionRule()
	{
		return this.selectionRules.GetSelectioMode(this.current_sub_mode);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x000813F4 File Offset: 0x0007F5F4
	public ViewMode(string name = null, string def_id = null)
	{
		if (name == null)
		{
			name = base.GetType().Name;
			if (name.EndsWith("View", StringComparison.Ordinal))
			{
				name = name.Substring(0, name.Length - 4);
			}
		}
		this.name = name;
		if (def_id == null)
		{
			def_id = name;
			if (!def_id.EndsWith("View", StringComparison.Ordinal))
			{
				def_id += "View";
			}
		}
		this.def_id = def_id;
		ViewMode.all.Add(this);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000814C4 File Offset: 0x0007F6C4
	public static void LoadDefs()
	{
		for (int i = 0; i < ViewMode.all.Count; i++)
		{
			ViewMode viewMode = ViewMode.all[i];
			DT.Field defField = global::Defs.GetDefField(viewMode.def_id, null);
			if (defField != null)
			{
				viewMode.LoadDef(defField);
			}
			else
			{
				Debug.LogWarning("View mode def not found: " + viewMode.def_id);
			}
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00081520 File Offset: 0x0007F720
	public static ViewMode Get(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		for (int i = 0; i < ViewMode.all.Count; i++)
		{
			ViewMode viewMode = ViewMode.all[i];
			if (viewMode.name == name)
			{
				return viewMode;
			}
		}
		return null;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00081569 File Offset: 0x0007F769
	public static bool IsPoliticalView()
	{
		return ViewMode.current is PoliticalView || ViewMode.current is TitleView;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x00081588 File Offset: 0x0007F788
	public virtual void OnApply(bool secondary)
	{
		this.wm = WorldMap.Get();
		WorldMap worldMap = this.wm;
		this.realms = ((worldMap != null) ? worldMap.Realms : null);
		this.kSrc = ((this.wm == null) ? null : global::Kingdom.Get(this.wm.SrcKingdom));
		this.uik = BaseUI.LogicKingdom();
		if (this.RealmColorsTexture == null)
		{
			this.RealmColorsTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			this.RealmColorsTexture.filterMode = FilterMode.Point;
			this.RealmColorsTexture.wrapMode = TextureWrapMode.Clamp;
		}
		this.RealmColors[0] = Color.gray * 0.5f;
		if (this.realms != null)
		{
			for (int i = 1; i <= this.realms.Count; i++)
			{
				global::Realm realm = this.realms[i - 1];
				if (realm != null)
				{
					Color black = Color.black;
					if (this.IsHighlighted(realm))
					{
						black.r = 1f;
					}
					if (realm.IsSeaRealm())
					{
						black.g = 1f;
					}
					black.a = this.GetRealmVisibilityAlpha(realm);
					this.RealmColors[i] = black;
					if (!secondary)
					{
						this.UpdateArmies(i);
					}
				}
			}
		}
		if (this.StripeR == null)
		{
			this.StripeR = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			this.StripeR.filterMode = FilterMode.Point;
			this.StripeR.wrapMode = TextureWrapMode.Clamp;
		}
		if (this.StripeG == null)
		{
			this.StripeG = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			this.StripeG.filterMode = FilterMode.Point;
			this.StripeG.wrapMode = TextureWrapMode.Clamp;
		}
		if (this.StripeB == null)
		{
			this.StripeB = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			this.StripeB.filterMode = FilterMode.Point;
			this.StripeB.wrapMode = TextureWrapMode.Clamp;
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00081770 File Offset: 0x0007F970
	protected virtual bool IsHighlighted(global::Realm r)
	{
		if (r == null)
		{
			return false;
		}
		if (this.wm == null)
		{
			return false;
		}
		if (r.id == this.wm.highlighted_realm)
		{
			return true;
		}
		if (this.ShowRealmLabels())
		{
			return false;
		}
		int highlightedKingdom = this.GetHighlightedKingdom();
		return highlightedKingdom > 0 && r.kingdom.id == highlightedKingdom;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x000817D0 File Offset: 0x0007F9D0
	protected virtual void SetRealmColor(int id, Color newColor)
	{
		if (this.realms == null || id <= 0 || id > this.realms.Count)
		{
			return;
		}
		if (this.realms[id - 1].IsSeaRealm())
		{
			if (this.drawSeaRealms)
			{
				newColor = this.RealmColors[id];
			}
			else
			{
				newColor = Color.clear;
			}
		}
		this.redChannel[id] = (this.greenChannel[id] = (this.blueChannel[id] = newColor));
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00081858 File Offset: 0x0007FA58
	protected virtual void SetRealmColors(int id, Color clrR, Color clrG, Color clrB)
	{
		if (this.realms == null || id <= 0 || id > this.realms.Count)
		{
			return;
		}
		if (this.realms[id - 1].IsSeaRealm())
		{
			if (this.drawSeaRealms)
			{
				clrG = (clrR = (clrB = this.RealmColors[id]));
			}
			else
			{
				clrG = (clrR = (clrB = Color.clear));
			}
		}
		this.redChannel[id] = clrR.linear;
		this.greenChannel[id] = clrG.linear;
		this.blueChannel[id] = clrB.linear;
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000818FC File Offset: 0x0007FAFC
	public void RefreshPVFigures()
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		for (int i = 1; i <= worldMap.Realms.Count; i++)
		{
			this.UpdateArmies(i);
		}
		ViewMode.refresh_armies = false;
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0008193C File Offset: 0x0007FB3C
	protected virtual void UpdateStatusBars(int rid)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		Logic.Realm logic = worldMap.Realms[rid - 1].logic;
		if (((logic != null) ? logic.armies : null) != null)
		{
			for (int i = 0; i < logic.armies.Count; i++)
			{
				Logic.Army army = logic.armies[i];
				global::Army army2 = army.visuals as global::Army;
				if (army2 != null)
				{
					army2.UpdateStatusBarVisiblity(army2.IsVisible() && army.battle == null);
				}
				if (army.battle != null)
				{
					global::Battle battle = army.battle.visuals as global::Battle;
					if (battle != null)
					{
						battle.UpdateStatusBarVisibility(battle.IsVisible());
					}
				}
			}
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00081A00 File Offset: 0x0007FC00
	protected virtual void UpdatePVFigures(int rid)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		Logic.Realm logic = worldMap.Realms[rid - 1].logic;
		if (((logic != null) ? logic.armies : null) != null)
		{
			for (int i = 0; i < logic.armies.Count; i++)
			{
				Logic.Army army = logic.armies[i];
				global::Army army2 = army.visuals as global::Army;
				if (army2 != null)
				{
					army2.UpdatePVFigureVisiblity(this.allowedFigures);
				}
				if (army.battle != null)
				{
					global::Battle battle = army.battle.visuals as global::Battle;
					if (battle != null)
					{
						battle.UpdatePVFigureVisiblity(this.allowedFigures);
					}
				}
			}
		}
		object obj;
		if (logic == null)
		{
			obj = null;
		}
		else
		{
			Castle castle = logic.castle;
			obj = ((castle != null) ? castle.visuals : null);
		}
		global::Settlement settlement = obj as global::Settlement;
		if (settlement == null)
		{
			return;
		}
		settlement.UpdatePVFigureVisiblity(this.allowedFigures);
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00081AD4 File Offset: 0x0007FCD4
	protected virtual void UpdateArmies(int rid)
	{
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		Logic.Realm logic = worldMap.Realms[rid - 1].logic;
		if (((logic != null) ? logic.armies : null) != null)
		{
			for (int i = 0; i < logic.armies.Count; i++)
			{
				Logic.Army army = logic.armies[i];
				global::Army army2 = army.visuals as global::Army;
				if (army2 != null)
				{
					if (ViewMode.refresh_armies)
					{
						army2.UpdateVisibility(false);
					}
					army2.UpdatePVFigureVisiblity(this.allowedFigures);
				}
				if (army.battle != null)
				{
					global::Battle battle = army.battle.visuals as global::Battle;
					if (battle != null)
					{
						if (ViewMode.refresh_armies)
						{
							battle.UpdateVisibility();
						}
						battle.UpdatePVFigureVisiblity(this.allowedFigures);
					}
				}
			}
		}
		object obj;
		if (logic == null)
		{
			obj = null;
		}
		else
		{
			Castle castle = logic.castle;
			obj = ((castle != null) ? castle.visuals : null);
		}
		global::Settlement settlement = obj as global::Settlement;
		if (settlement == null)
		{
			return;
		}
		settlement.UpdatePVFigureVisiblity(this.allowedFigures);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00081BE0 File Offset: 0x0007FDE0
	public int GetHighlightedKingdom()
	{
		if (this.wm == null)
		{
			return 0;
		}
		int result = 0;
		global::Realm realm = global::Realm.Get(this.wm.highlighted_realm);
		if (realm != null)
		{
			result = realm.kingdom.id;
		}
		return result;
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00081C20 File Offset: 0x0007FE20
	public Color ChangeColorBrightness(Color c, float amount)
	{
		return c * 1.5f;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00081C2D File Offset: 0x0007FE2D
	public virtual void SetStripeTextures(bool secondary)
	{
		Shader.SetGlobalTexture("_StripeR", this.StripeR);
		Shader.SetGlobalTexture("_StripeG", this.StripeG);
		Shader.SetGlobalTexture("_StripeB", this.StripeB);
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00081C60 File Offset: 0x0007FE60
	public virtual void SetShaderGlobals(bool secondary)
	{
		if (this.RealmColorsTexture == null)
		{
			return;
		}
		MapData mapData = MapData.Get();
		Shader.SetGlobalTexture("_TintTex", (mapData == null || mapData.TintTexture == null) ? Texture2D.whiteTexture : mapData.TintTexture);
		Shader.SetGlobalColor("_TintColor", (mapData == null || mapData.TintTexture == null) ? Color.gray : Color.white);
		this.RealmColorsTexture.SetPixels(this.RealmColors);
		this.RealmColorsTexture.Apply();
		this.StripeR.SetPixels(this.redChannel);
		this.StripeR.Apply();
		this.StripeG.SetPixels(this.greenChannel);
		this.StripeG.Apply();
		this.StripeB.SetPixels(this.blueChannel);
		this.StripeB.Apply();
		Shader.SetGlobalTexture("_RealmColors", this.RealmColorsTexture);
		this.SetStripeTextures(secondary);
		if (this.wm != null)
		{
			Shader.SetGlobalTexture("_RealmsData", this.wm.RealmsDataTexture);
		}
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00081D87 File Offset: 0x0007FF87
	public bool IsActive()
	{
		return ViewMode.current == this;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00081D91 File Offset: 0x0007FF91
	protected virtual void OnActivate()
	{
		ViewMode.refresh_armies = true;
		this.RefreshPVFigures();
		ViewMode.ViewChanged viewChanged = ViewMode.view_changed;
		if (viewChanged == null)
		{
			return;
		}
		viewChanged(this);
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x000023FD File Offset: 0x000005FD
	protected virtual void OnDeactivate()
	{
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00081DAF File Offset: 0x0007FFAF
	public void Deactivate()
	{
		this.OnDeactivate();
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00081DB8 File Offset: 0x0007FFB8
	public virtual bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt != Tooltip.Event.Place)
		{
			return false;
		}
		RectTransform component = tooltip.instance.GetComponent<RectTransform>();
		if (component == null)
		{
			return false;
		}
		Vector3 position = new Vector3(10f, (float)(Screen.height / 2), component.position.z);
		component.pivot = new Vector2(0f, 0.5f);
		component.transform.position = position;
		return true;
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00081E23 File Offset: 0x00080023
	public virtual float GetRealmVisibilityAlpha(global::Realm r)
	{
		if (r.logic == null)
		{
			return 0f;
		}
		if (r.IsSeaRealm())
		{
			return 1f;
		}
		return (float)r.logic.CalcVisibleBy(this.uik, true) * 0.5f;
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00081E5A File Offset: 0x0008005A
	public void ApplySecondary()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		ViewMode.secondary = this;
		ViewMode.RefreshSecondary();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00081E70 File Offset: 0x00080070
	public static void RefreshSecondary()
	{
		if (ViewMode.secondary == null)
		{
			ViewMode.secondary = ViewMode.Kingdoms;
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		global::Kingdom.ID srcKingdom = worldMap.SrcKingdom;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		global::Kingdom k = ((kingdom != null) ? kingdom.visuals : null) as global::Kingdom;
		worldMap.SetSrcKingdom(k, false);
		ViewMode.secondary.OnApply(true);
		ViewMode.secondary.SetShaderGlobals(true);
		worldMap.SrcKingdom = srcKingdom;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00081EE1 File Offset: 0x000800E1
	public void Apply()
	{
		this.Apply(false);
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00081EEC File Offset: 0x000800EC
	public void Apply(bool force_reapply)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		using (Game.Profile("ViewMode.Apply", false, 0f, null))
		{
			try
			{
				bool flag = force_reapply;
				if (!this.IsActive())
				{
					if (ViewMode.current != null)
					{
						flag = true;
						ViewMode.previousMode = ViewMode.current;
						ViewMode.current.OnDeactivate();
					}
					ViewMode.current = this;
					this.OnActivate();
				}
				if (!ViewMode.IsPoliticalView())
				{
					ViewMode.RefreshSecondary();
				}
				MapData.UpdateSrcKingdom(false);
				this.OnApply(false);
				this.SetShaderGlobals(false);
				if (this.wm != null && flag)
				{
					this.wm.ViewModeChanged();
				}
				CameraController cameraController = CameraController.Get();
				if (cameraController != null && flag)
				{
					cameraController.ViewModeChanged();
				}
				WorldUI worldUI = WorldUI.Get();
				if (worldUI != null)
				{
					if (flag)
					{
						worldUI.ViewModeChanged();
					}
					worldUI.RefreshTooltip(this.tt, true);
				}
				GarbageCollectHack.Collect();
			}
			catch (Exception ex)
			{
				Debug.LogError("Camera error: " + ex.ToString());
			}
		}
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00082008 File Offset: 0x00080208
	protected void OnDestroy()
	{
		if (this.RealmColorsTexture != null)
		{
			UnityEngine.Object.Destroy(this.RealmColorsTexture);
		}
		if (this.StripeR == null)
		{
			UnityEngine.Object.Destroy(this.StripeR);
		}
		if (this.StripeG == null)
		{
			UnityEngine.Object.Destroy(this.StripeG);
		}
		if (this.StripeB == null)
		{
			UnityEngine.Object.Destroy(this.StripeB);
		}
	}

	// Token: 0x040008C6 RID: 2246
	public static ViewMode.AllowedFigures figuresFilter = ViewMode.AllowedFigures.Army | ViewMode.AllowedFigures.Mercenary | ViewMode.AllowedFigures.Castle | ViewMode.AllowedFigures.Battle;

	// Token: 0x040008C7 RID: 2247
	private ViewMode.SubMode current_sub_mode;

	// Token: 0x040008C8 RID: 2248
	public Material terrain_material;

	// Token: 0x040008C9 RID: 2249
	protected Texture2D RealmColorsTexture;

	// Token: 0x040008CA RID: 2250
	protected Texture2D StripeR;

	// Token: 0x040008CB RID: 2251
	protected Texture2D StripeG;

	// Token: 0x040008CC RID: 2252
	protected Texture2D StripeB;

	// Token: 0x040008CD RID: 2253
	protected Color[] RealmColors = new Color[1024];

	// Token: 0x040008CE RID: 2254
	protected Color[] redChannel = new Color[1024];

	// Token: 0x040008CF RID: 2255
	protected Color[] greenChannel = new Color[1024];

	// Token: 0x040008D0 RID: 2256
	protected Color[] blueChannel = new Color[1024];

	// Token: 0x040008D1 RID: 2257
	protected WorldMap wm;

	// Token: 0x040008D2 RID: 2258
	protected List<global::Realm> realms;

	// Token: 0x040008D3 RID: 2259
	protected global::Kingdom kSrc;

	// Token: 0x040008D4 RID: 2260
	protected Logic.Kingdom uik;

	// Token: 0x040008D5 RID: 2261
	public bool drawSeaRealms;

	// Token: 0x040008D6 RID: 2262
	public static ViewMode.ViewChanged view_changed = null;

	// Token: 0x040008D7 RID: 2263
	public static ViewMode previousMode;

	// Token: 0x040008D8 RID: 2264
	public Tooltip tt;

	// Token: 0x040008D9 RID: 2265
	public GameObject filter;

	// Token: 0x040008DA RID: 2266
	public DT.Field def;

	// Token: 0x040008DB RID: 2267
	public bool highlightTarget = true;

	// Token: 0x040008DC RID: 2268
	public ViewMode.AllowedFigures allowedFigures;

	// Token: 0x040008DD RID: 2269
	public ViewMode.SelectionRules selectionRules = new ViewMode.SelectionRules();

	// Token: 0x040008DE RID: 2270
	public Color selected_highlight_color;

	// Token: 0x040008DF RID: 2271
	public string name;

	// Token: 0x040008E0 RID: 2272
	public string def_id;

	// Token: 0x040008E1 RID: 2273
	public static List<ViewMode> all = new List<ViewMode>();

	// Token: 0x040008E2 RID: 2274
	public static ViewMode current = null;

	// Token: 0x040008E3 RID: 2275
	public static ViewMode secondary = null;

	// Token: 0x040008E4 RID: 2276
	public static WorldView WorldView = new WorldView();

	// Token: 0x040008E5 RID: 2277
	public static OverlayView Overlay = new OverlayView();

	// Token: 0x040008E6 RID: 2278
	public static PoliticalView PoliticalView = new PoliticalView();

	// Token: 0x040008E7 RID: 2279
	public static KingdomsView Kingdoms = new KingdomsView();

	// Token: 0x040008E8 RID: 2280
	public static StancesView Stances = new StancesView();

	// Token: 0x040008E9 RID: 2281
	public static RelationsView Relations = new RelationsView();

	// Token: 0x040008EA RID: 2282
	public static ReligionView Religion = new ReligionView();

	// Token: 0x040008EB RID: 2283
	public static AuthoritiesView Authorities = new AuthoritiesView();

	// Token: 0x040008EC RID: 2284
	public static GoldIncomeView GoldIncome = new GoldIncomeView();

	// Token: 0x040008ED RID: 2285
	public static TradePowerView TradePower = new TradePowerView();

	// Token: 0x040008EE RID: 2286
	public static TradeZonesView TradeZones = new TradeZonesView();

	// Token: 0x040008EF RID: 2287
	public static CulturesView CulturesView = new CulturesView();

	// Token: 0x040008F0 RID: 2288
	public static CulturePowerView Culturepower = new CulturePowerView();

	// Token: 0x040008F1 RID: 2289
	public static ResourcesView Resources = new ResourcesView();

	// Token: 0x040008F2 RID: 2290
	public static CoreHistRealmsView CoreHistRealms = new CoreHistRealmsView();

	// Token: 0x040008F3 RID: 2291
	public static ProvinceFeaturesView ProvinceFeatures = new ProvinceFeaturesView();

	// Token: 0x040008F4 RID: 2292
	public static WarPeacePointsView WarPeacePoints = new WarPeacePointsView();

	// Token: 0x040008F5 RID: 2293
	public static RebellionRiskView RebelRisk = new RebellionRiskView();

	// Token: 0x040008F6 RID: 2294
	public static MercenariesView Mercenaries = new MercenariesView();

	// Token: 0x040008F7 RID: 2295
	public static OfferWeightView OfferWeight = new OfferWeightView();

	// Token: 0x040008F8 RID: 2296
	public static NeighborsView Neighbors = new NeighborsView();

	// Token: 0x040008F9 RID: 2297
	public static BuildingsView Buildings = new BuildingsView();

	// Token: 0x040008FA RID: 2298
	public static ProducedResourcesView ProducedResources = new ProducedResourcesView();

	// Token: 0x040008FB RID: 2299
	public static DebugPoliticalView DebugView = new DebugPoliticalView();

	// Token: 0x040008FC RID: 2300
	public static MarriagesView MarriagesView = new MarriagesView();

	// Token: 0x040008FD RID: 2301
	public static TitleView titleView = new TitleView();

	// Token: 0x040008FE RID: 2302
	public static TitleKingdomsView titleKingdomsView = new TitleKingdomsView();

	// Token: 0x040008FF RID: 2303
	public static bool refresh_armies = false;

	// Token: 0x02000600 RID: 1536
	[Flags]
	public enum AllowedFigures
	{
		// Token: 0x04003378 RID: 13176
		None = 0,
		// Token: 0x04003379 RID: 13177
		Army = 1,
		// Token: 0x0400337A RID: 13178
		Mercenary = 2,
		// Token: 0x0400337B RID: 13179
		Castle = 4,
		// Token: 0x0400337C RID: 13180
		Battle = 8
	}

	// Token: 0x02000601 RID: 1537
	public enum SubMode
	{
		// Token: 0x0400337E RID: 13182
		Kingdom,
		// Token: 0x0400337F RID: 13183
		Realm
	}

	// Token: 0x02000602 RID: 1538
	public class SelectionRules
	{
		// Token: 0x0600469E RID: 18078 RVA: 0x0020F6F8 File Offset: 0x0020D8F8
		public void LoadDef(DT.Field def)
		{
			this.rules.Clear();
			if (def == null)
			{
				return;
			}
			this.field = def;
			if (!Enum.TryParse<ViewMode.SubMode>(this.field.GetString("preferred_sub_mode", null, "", true, true, true, '.'), true, out this.preferred_sub_mode))
			{
				this.preferred_sub_mode = ViewMode.SubMode.Kingdom;
			}
			List<DT.Field> list = this.field.Children();
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field = list[i];
				ViewMode.SubMode subMode;
				if (!string.IsNullOrEmpty(field.key) && !(field.type == "template") && string.IsNullOrEmpty(field.type) && Enum.TryParse<ViewMode.SubMode>(field.key, out subMode))
				{
					ViewMode.SelectionRules.Rule rule = new ViewMode.SelectionRules.Rule();
					rule.type = subMode;
					rule.pick_target = field.GetString("pick_target", null, "", true, true, true, '.');
					rule.show_kingdom_border = field.GetBool("show_kingdom_border", null, false, true, true, true, '.');
					rule.show_realm_border = field.GetBool("show_realm_border", null, false, true, true, true, '.');
					this.rules.Add(subMode, rule);
				}
			}
		}

		// Token: 0x0600469F RID: 18079 RVA: 0x0020F828 File Offset: 0x0020DA28
		public ViewMode.SelectionRules.Rule GetSelectioMode(ViewMode.SubMode m)
		{
			ViewMode.SelectionRules.Rule result;
			if (!this.rules.TryGetValue(m, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x04003380 RID: 13184
		public ViewMode.SubMode preferred_sub_mode;

		// Token: 0x04003381 RID: 13185
		public DT.Field field;

		// Token: 0x04003382 RID: 13186
		private Dictionary<ViewMode.SubMode, ViewMode.SelectionRules.Rule> rules = new Dictionary<ViewMode.SubMode, ViewMode.SelectionRules.Rule>();

		// Token: 0x020009EA RID: 2538
		public class Rule
		{
			// Token: 0x040045D7 RID: 17879
			public ViewMode.SubMode type;

			// Token: 0x040045D8 RID: 17880
			public string pick_target;

			// Token: 0x040045D9 RID: 17881
			public bool show_realm_border;

			// Token: 0x040045DA RID: 17882
			public bool show_kingdom_border;
		}
	}

	// Token: 0x02000603 RID: 1539
	// (Invoke) Token: 0x060046A2 RID: 18082
	public delegate void ViewChanged(ViewMode view);
}
