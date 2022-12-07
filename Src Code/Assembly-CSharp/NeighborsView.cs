using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class NeighborsView : PoliticalView
{
	// Token: 0x06000BCB RID: 3019 RVA: 0x000848C0 File Offset: 0x00082AC0
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			Kingdom.ID kingdom = worldUI.kingdom;
		}
		Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<Settlement>() : null;
		for (int i = 1; i <= this.realms.Count; i++)
		{
			Realm realm = this.realms[i - 1];
			Color newColor = Color.black;
			if (settlement != null)
			{
				if (settlement.GetRealmID() == realm.id)
				{
					newColor = Color.cyan;
				}
				else if (settlement.GetRealm().logic.logicNeighborsAll.Contains(realm.logic))
				{
					newColor = Color.green;
				}
				else if (settlement.GetRealm().logic.logicNeighborsRestricted.Contains(realm.logic))
				{
					newColor = Color.yellow;
				}
			}
			this.SetRealmColor(i, newColor);
		}
	}
}
