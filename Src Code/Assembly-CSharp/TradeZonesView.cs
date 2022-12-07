using System;
using Logic;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class TradeZonesView : PoliticalView
{
	// Token: 0x06000BE3 RID: 3043 RVA: 0x000859FC File Offset: 0x00083BFC
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.color_non_tc_selection = global::Defs.GetColor(this.def_id, "color_non_tc_selection", Color.clear);
		this.color_tc_selection = global::Defs.GetColor(this.def_id, "color_tc_selection", Color.clear);
		this.color_tc_stripe = global::Defs.GetColor(this.def_id, "color_tc_stripe", Color.clear);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x00085A64 File Offset: 0x00083C64
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			global::Kingdom.ID kingdom = worldUI.kingdom;
		}
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		if (worldUI.selected_logic_obj != null)
		{
			worldUI.selected_logic_obj.GetKingdom();
		}
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm = this.wm.Realms[i];
			Color color;
			if (settlement != null && settlement.IsCastle() && settlement.GetRealmID() == realm.id)
			{
				if (realm.logic.IsTradeCenter())
				{
					color = this.color_tc_selection;
				}
				else
				{
					color = this.color_non_tc_selection;
				}
			}
			else if (realm.logic.IsInfluencedByTradeCenter() || realm.logic.IsTradeCenter())
			{
				int idx = game.economy.tradeCenterRealms.IndexOf(game.economy.tradeCenterRealms.Find((Logic.Realm pf) => pf.castle.realm_id == realm.logic.tradeCenter.realm.id));
				DT.Field field = realm.logic.tradeCenter.def.field.FindChild("map_colors", null, true, true, true, '.');
				object obj_val;
				if ((obj_val = field.Value(idx, null, true, true).obj_val) is Color)
				{
					Color color2 = (Color)obj_val;
					color = color2;
				}
				else
				{
					color = global::Defs.ColorFromString(field.String(idx, null, ""), Color.clear);
				}
			}
			else
			{
				color = this.color_none;
			}
			Color color3;
			if (realm.logic.IsTradeCenter())
			{
				color3 = this.color_tc_stripe;
			}
			else
			{
				color3 = color;
			}
			this.SetRealmColors(i + 1, color3, color, color3);
		}
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00085C6E File Offset: 0x00083E6E
	public override bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		return base.HandleTooltip(ui, tooltip, evt);
	}

	// Token: 0x04000947 RID: 2375
	private Color color_non_tc_selection;

	// Token: 0x04000948 RID: 2376
	private Color color_tc_selection;

	// Token: 0x04000949 RID: 2377
	private Color color_tc_stripe;
}
