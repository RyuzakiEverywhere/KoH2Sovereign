using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class MercenariesView : PoliticalView
{
	// Token: 0x06000BB1 RID: 2993 RVA: 0x00083A06 File Offset: 0x00081C06
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colorTexture = global::Defs.GetObj<Texture2D>(field, "color_texture", null);
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00083A24 File Offset: 0x00081C24
	protected override void OnActivate()
	{
		this.realmToMerc.Clear();
		if (this.mercTypes == null)
		{
			Game game = GameLogic.Get(true);
			if (game != null)
			{
				List<Mercenary.Def> defs = game.defs.GetDefs<Mercenary.Def>();
				for (int i = 0; i < defs.Count; i++)
				{
					Mercenary.Def def = defs[i];
					if (!def.IsBase() && def.max_mercs_per_realm > this.max)
					{
						this.max = def.max_mercs_per_realm;
					}
				}
			}
		}
		base.OnActivate();
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00083A9C File Offset: 0x00081C9C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (!Application.isPlaying)
		{
			return;
		}
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		if (this.colorTexture == null)
		{
			Debug.Log("Mercenaries count Texture is not set");
			return;
		}
		int height = this.colorTexture.height;
		this.realmToMerc.Clear();
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm = this.wm.Realms[i];
			float num = 0f;
			if (realm.logic != null)
			{
				int num2 = 0;
				using (List<Logic.Army>.Enumerator enumerator = realm.logic.armies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.mercenary != null)
						{
							num2++;
						}
					}
				}
				num = (float)(num2 - this.min) / (float)(this.max - this.min);
				this.realmToMerc.Add(realm.logic.id, num2);
			}
			int y = (int)(num * (float)height);
			Color pixel = this.colorTexture.GetPixel(1, y);
			this.SetRealmColor(i + 1, pixel);
		}
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00083BDC File Offset: 0x00081DDC
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
			int num = 0;
			this.realmToMerc.TryGetValue(realm.id, out num);
			tooltip.text = num.ToString();
		}
		return false;
	}

	// Token: 0x0400091C RID: 2332
	public Texture2D colorTexture;

	// Token: 0x0400091D RID: 2333
	public List<Mercenary.Def> mercTypes;

	// Token: 0x0400091E RID: 2334
	public Dictionary<int, int> realmToMerc = new Dictionary<int, int>();

	// Token: 0x0400091F RID: 2335
	public int min;

	// Token: 0x04000920 RID: 2336
	public int max;
}
