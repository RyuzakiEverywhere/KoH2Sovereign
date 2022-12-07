using System;
using Logic;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class ReligiousPowerView : PoliticalView
{
	// Token: 0x06000BD2 RID: 3026 RVA: 0x00084D5C File Offset: 0x00082F5C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colorTexture = global::Defs.GetObj<Texture2D>(field, "religious_power_color_texture", null);
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00084D78 File Offset: 0x00082F78
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.colorTexture == null)
		{
			Debug.Log("Realm Gold Color Texture is not set");
			return;
		}
		int height = this.colorTexture.height;
		WorldUI worldUI = WorldUI.Get();
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm = this.wm.Realms[i];
			int num = (int)(((realm.logic != null) ? (realm.logic.religiousPower.amount / realm.logic.religiousPower.maxAmount) : 0f) * (float)height);
			if (num == height)
			{
				num--;
			}
			Color newColor = (settlement != null && settlement.IsCastle() && settlement.GetRealmID() == realm.id) ? Color.green : this.colorTexture.GetPixel(1, height - num);
			this.SetRealmColor(i + 1, newColor);
		}
	}

	// Token: 0x0400093C RID: 2364
	public Texture2D colorTexture;
}
