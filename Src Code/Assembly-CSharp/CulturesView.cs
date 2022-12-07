using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class CulturesView : PoliticalView
{
	// Token: 0x06000B9C RID: 2972 RVA: 0x000829DC File Offset: 0x00080BDC
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.defaultColor = global::Defs.GetColor(field, "defaultColor", Color.black, null);
		this.selectedColor = global::Defs.GetColor(field, "selectedColor", Color.magenta, null);
		this.useSelectionColor = field.GetBool("useSelectionColor", null, false, true, true, true, '.');
		this.color_map_min = field.GetFloat("color_map_min", null, 0f, true, true, true, '.');
		this.color_map_max = field.GetFloat("color_map_max", null, 0f, true, true, true, '.');
		this.non_selection_desaturate_perc = field.GetFloat("non_selection_desaturate_perc", null, 0f, true, true, true, '.');
		DT.Field field2 = field.FindChild("colors_per_culture", null, true, true, true, '.');
		int count = field2.children.Count;
		for (int i = 0; i < count; i++)
		{
			DT.Field field3 = field2.children[i];
			if (field3.type.ToLowerInvariant() == "color")
			{
				this.colorsPerCulture[field3.key] = (Color)field3.Value(null, true, true).obj_val;
			}
		}
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x00082AFC File Offset: 0x00080CFC
	public float map(float v, float vmin, float vmax, float rmin, float rmax)
	{
		if (v < vmin)
		{
			v = vmin;
		}
		if (v > vmax)
		{
			v = vmax;
		}
		return rmin + (v - vmin) * (rmax - rmin) / (vmax - vmin);
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x00082B1C File Offset: 0x00080D1C
	protected Color GetColor(string culture, bool fade, Game game)
	{
		string group = game.cultures.GetGroup(culture);
		if (string.IsNullOrEmpty(group))
		{
			return this.defaultColor;
		}
		Color color;
		if (!this.colorsPerCulture.TryGetValue(group, out color))
		{
			color = this.defaultColor;
		}
		if (!fade)
		{
			return color;
		}
		int num;
		if (!this.activeCultureIngroupIndex.TryGetValue(culture, out num))
		{
			num = -1;
		}
		int num2;
		if (!this.activeGroupSubculturesCount.TryGetValue(group, out num2))
		{
			num2 = -1;
		}
		if (num == -1 || num2 <= 1)
		{
			return color;
		}
		color *= this.map((float)num, 0f, (float)(num2 - 1), this.color_map_min, this.color_map_max);
		return color;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00082BB4 File Offset: 0x00080DB4
	public void RecalcActiveCultures(WorldMap wm)
	{
		this.activeGroupSubculturesCount.Clear();
		this.activeCultureIngroupIndex.Clear();
		for (int i = 0; i < wm.Realms.Count; i++)
		{
			global::Realm realm = wm.Realms[i];
			Logic.Realm realm2 = (realm != null) ? realm.logic : null;
			if (!realm2.IsSeaRealm())
			{
				string culture = realm2.pop_majority.kingdom.culture;
				if (!string.IsNullOrEmpty(culture) && !this.activeCultureIngroupIndex.ContainsKey(culture))
				{
					string key = realm2.game.cultures.GetGroup(culture) ?? "";
					int num;
					if (!this.activeGroupSubculturesCount.TryGetValue(key, out num))
					{
						num = 0;
					}
					this.activeCultureIngroupIndex[culture] = num;
					this.activeGroupSubculturesCount[key] = num + 1;
				}
			}
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00082C88 File Offset: 0x00080E88
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI worldUI = WorldUI.Get();
		this.RecalcActiveCultures(this.wm);
		global::Settlement settlement;
		if (worldUI == null)
		{
			settlement = null;
		}
		else
		{
			GameObject selected_obj = worldUI.selected_obj;
			settlement = ((selected_obj != null) ? selected_obj.GetComponent<global::Settlement>() : null);
		}
		global::Settlement settlement2 = settlement;
		int num = (settlement2 == null) ? 0 : settlement2.GetRealmID();
		global::Realm realm = global::Realm.Get(num);
		Logic.Realm realm2 = (realm != null) ? realm.logic : null;
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm3 = this.wm.Realms[i];
			Logic.Realm realm4 = (realm3 != null) ? realm3.logic : null;
			if (!realm4.IsSeaRealm())
			{
				Logic.Kingdom kingdom = (realm4 != null) ? realm4.GetKingdom() : null;
				string culture = realm4.pop_majority.kingdom.culture;
				string culture2 = kingdom.culture;
				string group = realm4.game.cultures.GetGroup(culture);
				Cultures cultures = realm4.game.cultures;
				string text;
				if (realm2 == null)
				{
					text = null;
				}
				else
				{
					Logic.Kingdom kingdom2 = realm2.pop_majority.kingdom;
					text = ((kingdom2 != null) ? kingdom2.culture : null);
				}
				bool flag = group == cultures.GetGroup(text ?? "");
				Color color = this.GetColor(culture, flag, realm4.game);
				if (!flag && realm2 != null)
				{
					color *= this.non_selection_desaturate_perc / 100f;
				}
				this.redChannel[i + 1] = color;
				this.greenChannel[i + 1] = ((this.useSelectionColor && realm4.id == num) ? this.selectedColor : color);
				this.blueChannel[i + 1] = color;
			}
		}
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00082E2C File Offset: 0x0008102C
	public override bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return base.HandleTooltip(ui, tooltip, evt);
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return false;
		}
		global::Kingdom kingdom = global::Kingdom.Get(worldMap.SrcKingdom);
		if (kingdom == null || kingdom.logic == null)
		{
			return false;
		}
		global::Realm realm = global::Realm.Get(worldMap.highlighted_realm);
		if (realm == null || realm.logic == null || realm.IsSeaRealm())
		{
			tooltip.SetText("", null, null);
			return false;
		}
		Vars vars = tooltip.vars;
		if (vars == null)
		{
			vars = new Vars(realm.logic);
		}
		else
		{
			vars.obj = realm.logic;
		}
		tooltip.SetDef("CulturesTooltip", vars);
		return false;
	}

	// Token: 0x0400090B RID: 2315
	private Color defaultColor = Color.black;

	// Token: 0x0400090C RID: 2316
	private Color selectedColor = Color.black;

	// Token: 0x0400090D RID: 2317
	private bool useSelectionColor;

	// Token: 0x0400090E RID: 2318
	private float color_map_min = 0.5f;

	// Token: 0x0400090F RID: 2319
	private float color_map_max = 1.5f;

	// Token: 0x04000910 RID: 2320
	private float non_selection_desaturate_perc = 0.5f;

	// Token: 0x04000911 RID: 2321
	public Dictionary<string, Color> colorsPerCulture = new Dictionary<string, Color>();

	// Token: 0x04000912 RID: 2322
	public Dictionary<string, int> activeGroupSubculturesCount = new Dictionary<string, int>();

	// Token: 0x04000913 RID: 2323
	public Dictionary<string, int> activeCultureIngroupIndex = new Dictionary<string, int>();
}
