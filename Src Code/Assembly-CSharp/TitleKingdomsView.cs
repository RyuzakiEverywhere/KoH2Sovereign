using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class TitleKingdomsView : TitleView
{
	// Token: 0x06000B65 RID: 2917 RVA: 0x00080E6C File Offset: 0x0007F06C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		Campaign campaign = GameLogic.Get(true).campaign;
		TitleMap titleMap = TitleMap.Get();
		if (titleMap == null)
		{
			return;
		}
		List<global::Realm> realms = titleMap.Realms;
		for (int i = 1; i <= realms.Count; i++)
		{
			global::Realm realm = realms[i - 1];
			if (realm != null)
			{
				Color color = Color.black;
				if (this.IsHighlighted(realm))
				{
					color.r = 1f;
				}
				if (realm.IsSeaRealm())
				{
					color.g = 1f;
				}
				color.a = 1f;
				this.RealmColors[i] = color;
				color = realm.MapColor;
				if (realm.logic == null)
				{
					this.SetRealmColor(i, color);
					return;
				}
				global::Kingdom kingdom = realm.GetKingdom();
				color = ((kingdom == null) ? this.color_none : kingdom.MapColor);
				color *= (this.IsSelectedrealm(realm) ? 1.4f : 1f);
				if (CampaignUtils.IsBlackListedKingdom(campaign, (kingdom != null) ? kingdom.logic : null))
				{
					global::Common.Desaturate(ref color, 0.5f);
					color *= Color.gray;
				}
				Color color2 = color;
				global::Kingdom kingdom2 = (kingdom != null && kingdom.logic.sovereignState != null) ? titleMap.Kingdoms[kingdom.logic.sovereignState.id - 1] : null;
				if (kingdom2 != null)
				{
					color2 = kingdom2.MapColor;
					if (CampaignUtils.IsBlackListedKingdom(campaign, (kingdom2 != null) ? kingdom2.logic : null))
					{
						global::Common.Desaturate(ref color2, 0.5f);
						color2 *= Color.gray;
					}
				}
				this.blueChannel[i] = color2;
				this.redChannel[i] = this.blueChannel[i];
				this.greenChannel[i] = color;
			}
		}
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0008104C File Offset: 0x0007F24C
	private bool IsSelectedrealm(global::Realm r)
	{
		if (r == null)
		{
			return false;
		}
		TitleMap titleMap = TitleMap.Get();
		return !(titleMap == null) && r.id == titleMap.selected_realm;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00081080 File Offset: 0x0007F280
	protected override bool IsHighlighted(global::Realm r)
	{
		if (r == null)
		{
			return false;
		}
		TitleMap titleMap = TitleMap.Get();
		if (titleMap == null)
		{
			return false;
		}
		if (r.id == titleMap.highlighted_realm)
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

	// Token: 0x06000B68 RID: 2920 RVA: 0x000810DC File Offset: 0x0007F2DC
	public new int GetHighlightedKingdom()
	{
		TitleMap titleMap = TitleMap.Get();
		if (titleMap == null)
		{
			return 0;
		}
		int result = 0;
		global::Realm realm = global::Realm.Get(titleMap.highlighted_realm);
		if (realm != null)
		{
			result = realm.kingdom.id;
		}
		return result;
	}
}
