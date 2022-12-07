using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class KingdomsView : PoliticalView
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x00083710 File Offset: 0x00081910
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI worldUI = WorldUI.Get();
		for (int i = 1; i <= this.realms.Count; i++)
		{
			Realm realm = this.realms[i - 1];
			Color color = realm.MapColor;
			if (realm.logic == null)
			{
				this.SetRealmColor(i, color);
				return;
			}
			base.GetHighlightedKingdom();
			Kingdom kingdom = realm.GetKingdom();
			color = ((kingdom == null) ? this.color_none : kingdom.MapColor);
			if (worldUI != null)
			{
				Kingdom.ID kingdom2 = worldUI.kingdom;
			}
			Kingdom kingdom3 = (kingdom != null && kingdom.logic.sovereignState != null) ? this.wm.Kingdoms[kingdom.logic.sovereignState.id - 1] : null;
			this.blueChannel[i] = ((kingdom3 != null) ? kingdom3.MapColor : color);
			this.redChannel[i] = this.blueChannel[i];
			this.greenChannel[i] = color;
		}
	}
}
