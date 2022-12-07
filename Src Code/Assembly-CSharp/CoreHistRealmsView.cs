using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class CoreHistRealmsView : PoliticalView
{
	// Token: 0x06000B94 RID: 2964 RVA: 0x000825D5 File Offset: 0x000807D5
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x000825E0 File Offset: 0x000807E0
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI worldUI = WorldUI.Get();
		Logic.Kingdom kingdom;
		if (worldUI == null)
		{
			kingdom = null;
		}
		else
		{
			global::Kingdom selected_kingdom = worldUI.selected_kingdom;
			kingdom = ((selected_kingdom != null) ? selected_kingdom.logic : null);
		}
		Logic.Kingdom kingdom2 = kingdom;
		List<global::Realm> realms = this.wm.Realms;
		for (int i = 0; i < realms.Count; i++)
		{
			Logic.Realm logic = realms[i].logic;
			Color color = Color.black;
			if (kingdom2 != null)
			{
				if (logic.IsCoreFor(kingdom2))
				{
					color += Color.blue;
				}
				if (logic.IsHistoricalFor(kingdom2))
				{
					color += Color.red;
				}
				if (logic.kingdom_id == kingdom2.id)
				{
					color += Color.green;
				}
			}
			this.SetRealmColor(i + 1, color);
		}
	}

	// Token: 0x04000906 RID: 2310
	public List<Color> colors;
}
