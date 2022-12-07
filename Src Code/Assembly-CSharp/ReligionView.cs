using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class ReligionView : PoliticalView
{
	// Token: 0x06000BD0 RID: 3024 RVA: 0x00084B9C File Offset: 0x00082D9C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		for (int i = 1; i <= this.wm.Realms.Count; i++)
		{
			Color color = this.color_none;
			Realm realm = this.wm.Realms[i - 1];
			Kingdom kingdom = (realm != null) ? realm.GetKingdom() : null;
			if (kingdom == null || kingdom.logic == null || kingdom.logic.religion == null)
			{
				this.SetRealmColor(i, color);
			}
			else
			{
				string key;
				if (kingdom.logic.is_catholic && kingdom.logic.excommunicated)
				{
					key = "map_color.excommunicated";
				}
				else if (kingdom.logic.is_orthodox && kingdom.logic.subordinated)
				{
					key = "map_color.subordinated";
				}
				else if (kingdom.logic.is_orthodox && !kingdom.logic.is_ecumenical_patriarchate)
				{
					key = "map_color.independent";
				}
				else
				{
					key = "map_color";
				}
				color = Defs.GetColor(kingdom.logic.religion.def.field, key, null);
				if (realm.logic != null && realm.logic.religion != kingdom.logic.religion)
				{
					this.blueChannel[i] = Defs.GetColor(realm.logic.religion.def.field, "map_color", null);
				}
				else
				{
					this.blueChannel[i] = color;
				}
				this.redChannel[i] = color;
				if (kingdom.logic.caliphate)
				{
					this.greenChannel[i] = Defs.GetColor(kingdom.logic.religion.def.field, "map_color.caliphate", null);
				}
				else
				{
					this.greenChannel[i] = color;
				}
			}
		}
	}
}
