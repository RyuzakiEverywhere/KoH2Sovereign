using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class TradePowerView : PoliticalView
{
	// Token: 0x06000BDF RID: 3039 RVA: 0x0008565E File Offset: 0x0008385E
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colorTexture = global::Defs.GetObj<Texture2D>(field, "trade_power_color_texture", null);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0008567C File Offset: 0x0008387C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		if (this.colorTexture == null)
		{
			Debug.Log("Realm Gold Color Texture is not set");
			return;
		}
		this.rankingValue.Clear();
		int height = this.colorTexture.height;
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			global::Kingdom.ID kingdom = worldUI.kingdom;
		}
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		Logic.Kingdom kingdom2 = null;
		if (worldUI.selected_logic_obj != null)
		{
			kingdom2 = worldUI.selected_logic_obj.GetKingdom();
		}
		KingdomTradeRanking component = game.economy.GetComponent<KingdomTradeRanking>();
		Color color = new Color(0.13f, 0.3f, 1f);
		new Color(0.36f, 0f, 0.768f);
		bool flag = component != null && component.def != null;
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm = this.wm.Realms[i];
			float num = 0f;
			bool flag2 = realm.logic != null;
			int num2 = 0;
			if (flag2 && flag)
			{
				num2 = realm.logic.kingdom_id;
				float baseGold = component.GetBaseGold(num2);
				num = (baseGold - component.def.min) / (component.def.max - component.def.min);
				if (!this.rankingValue.ContainsKey(realm.logic.kingdom_id))
				{
					this.rankingValue.Add(realm.logic.kingdom_id, baseGold);
				}
			}
			int num3 = (int)(num * (float)height);
			if (num3 == height)
			{
				num3--;
			}
			Color color2 = (settlement != null && settlement.IsCastle() && settlement.GetKingdomID() == num2) ? Color.blue : this.colorTexture.GetPixel(1, height - num3);
			this.blueChannel[i + 1] = color2;
			this.redChannel[i + 1] = color2;
			if (kingdom2 != null && realm.id > 0)
			{
				global::Kingdom kingdom3 = realm.GetKingdom();
				if (kingdom3 != null)
				{
					if (kingdom3.logic.HasTradeAgreement(kingdom2))
					{
						this.blueChannel[i + 1] = color;
					}
					if (kingdom2.tradeRouteWith.Contains(kingdom3.logic))
					{
						this.redChannel[i + 1] = this.blueChannel[i + 1];
					}
				}
			}
			this.greenChannel[i + 1] = color2;
		}
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0008591C File Offset: 0x00083B1C
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
		if (realm != null && this.rankingValue != null)
		{
			int highlightedKingdom = base.GetHighlightedKingdom();
			global::Kingdom kingdom = global::Kingdom.Get(highlightedKingdom);
			float num = 0f;
			if (realm.id >= 0)
			{
				this.rankingValue.TryGetValue(highlightedKingdom, out num);
			}
			tooltip.text = "Trade value: " + num.ToString();
			if (kingdom != null && kingdom.logic != null)
			{
				tooltip.text = tooltip.text + "\nTrade power: " + kingdom.logic.GetStat(Stats.ks_commerce, true).ToString();
			}
		}
		return false;
	}

	// Token: 0x04000945 RID: 2373
	public Texture2D colorTexture;

	// Token: 0x04000946 RID: 2374
	private Dictionary<int, float> rankingValue = new Dictionary<int, float>();
}
