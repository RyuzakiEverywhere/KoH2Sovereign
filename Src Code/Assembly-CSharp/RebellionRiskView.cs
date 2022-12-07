using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class RebellionRiskView : PoliticalView
{
	// Token: 0x06000BC6 RID: 3014 RVA: 0x000845AC File Offset: 0x000827AC
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colorTexture = global::Defs.GetObj<Texture2D>(field, "color_texture", null);
		this.color_rebel_presence = global::Defs.GetColor(field, "color_rebel_presence", Color.clear, null);
		this.color_rebel_occupation = global::Defs.GetColor(field, "color_rebel_occupation", Color.clear, null);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x00084600 File Offset: 0x00082800
	protected override void OnActivate()
	{
		this.realmToRisk.Clear();
		if (this.risk_categories == null)
		{
			Game game = GameLogic.Get(true);
			if (game != null)
			{
				this.risk_categories = new List<RebellionRiskCategory.Def>();
				DT.Def def = game.dt.FindDef("RebellionRiskCategory");
				this.risk_categories.Add(game.defs.Find<RebellionRiskCategory.Def>(def.field.key));
				for (int i = 0; i < def.defs.Count; i++)
				{
					this.risk_categories.Add(game.defs.Find<RebellionRiskCategory.Def>(def.defs[i].field.key));
				}
			}
		}
		base.OnActivate();
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x000846B4 File Offset: 0x000828B4
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (this.colorTexture == null)
		{
			Debug.Log("Realm Gold Color Texture is not set");
			return;
		}
		int height = this.colorTexture.height;
		if (worldUI != null)
		{
			global::Kingdom.ID kingdom = worldUI.kingdom;
		}
		this.realmToRisk.Clear();
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm = this.wm.Realms[i];
			float num = 0f;
			bool flag = realm.logic != null;
			if (flag)
			{
				float num2 = 0f;
				float num3 = -100f;
				float totalRebellionRisk = realm.logic.GetTotalRebellionRisk();
				num = 1f - (flag ? ((totalRebellionRisk - num3) / (num2 - num3)) : 0f);
				this.realmToRisk.Add(realm.logic.id, totalRebellionRisk);
			}
			int y = (int)(num * (float)height);
			Color pixel;
			Color clrR;
			Color clrG = clrR = (pixel = this.colorTexture.GetPixel(0, y));
			Logic.Realm logic = realm.logic;
			if (logic != null)
			{
				List<Logic.Army> armies = logic.armies;
				for (int j = 0; j < armies.Count; j++)
				{
					if (armies[j].rebel != null)
					{
						pixel = this.color_rebel_presence;
						break;
					}
				}
				if (logic.IsOccupied())
				{
					clrR = this.color_rebel_occupation;
				}
			}
			this.SetRealmColors(i + 1, clrR, clrG, pixel);
		}
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0008483C File Offset: 0x00082A3C
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
		global::Realm realm = global::Realm.Get(worldMap.highlighted_realm);
		if (realm != null)
		{
			float num = 0f;
			this.realmToRisk.TryGetValue(realm.id, out num);
			tooltip.text = num.ToString();
		}
		return false;
	}

	// Token: 0x04000931 RID: 2353
	public Texture2D colorTexture;

	// Token: 0x04000932 RID: 2354
	public int selectedIndex = -1;

	// Token: 0x04000933 RID: 2355
	public List<RebellionRiskCategory.Def> risk_categories;

	// Token: 0x04000934 RID: 2356
	public Dictionary<int, float> realmToRisk = new Dictionary<int, float>();

	// Token: 0x04000935 RID: 2357
	public Color color_rebel_presence;

	// Token: 0x04000936 RID: 2358
	public Color color_rebel_occupation;
}
