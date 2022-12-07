using System;
using Logic;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class RealmsView : PoliticalView
{
	// Token: 0x06000BC3 RID: 3011 RVA: 0x00084458 File Offset: 0x00082658
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		Texture2D obj = global::Defs.GetObj<Texture2D>(field, "realm_color_texture", null);
		if (obj == null)
		{
			return;
		}
		Color[] pixels = obj.GetPixels();
		if (pixels.Length < 52900)
		{
			Debug.Log("wrong realm color texture");
			return;
		}
		this.colors = new Color[100];
		int num = 0;
		for (int i = 0; i < 230; i += 23)
		{
			for (int j = 0; j < 52900; j += 5290)
			{
				this.colors[num] = pixels[i + j];
				num++;
			}
		}
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x000844F4 File Offset: 0x000826F4
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI worldUI = WorldUI.Get();
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			Color newColor = (settlement != null && settlement.GetRealmID() == realm.id) ? Color.clear : ((this.colors == null) ? realm.MapColor : this.colors[(i - 1) % this.colors.Length]);
			this.SetRealmColor(i, newColor);
		}
	}

	// Token: 0x04000930 RID: 2352
	public Color[] colors;
}
