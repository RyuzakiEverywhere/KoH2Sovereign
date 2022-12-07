using System;
using Logic;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class RelationsView : PoliticalView
{
	// Token: 0x06000BCD RID: 3021 RVA: 0x000849B8 File Offset: 0x00082BB8
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.clr_worst = global::Defs.GetColor(field, "clr_worst", Color.red * 0.7f, null);
		this.clr_neutral_neg = global::Defs.GetColor(field, "clr_neutral_neg", Color.yellow * 0.7f, null);
		this.clr_neutral_pos = global::Defs.GetColor(field, "clr_neutral_pos", Color.yellow * 0.7f, null);
		this.clr_best = global::Defs.GetColor(field, "clr_best", Color.green * 0.7f, null);
		this.clr_seleciton = global::Defs.GetColor(field, "clr_seleciton", Color.green * 0.7f, null);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00084A74 File Offset: 0x00082C74
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			global::Kingdom kingdom = realm.GetKingdom();
			if (!realm.IsSeaRealm())
			{
				Logic.Realm selecteRelam = base.GetSelecteRelam();
				Logic.Kingdom kingdom2 = (this.kSrc != null) ? this.kSrc.logic : ((selecteRelam != null) ? selecteRelam.GetKingdom() : null);
				float num = 0f;
				if (kingdom2 != null && kingdom != null && kingdom.logic != null)
				{
					num = kingdom2.GetRelationship(kingdom.logic);
				}
				Color newColor;
				if (kingdom == this.kSrc || (kingdom2 != null && kingdom != null && kingdom2.id == kingdom.id))
				{
					newColor = this.clr_seleciton;
				}
				else if (num > -50f && num < 50f)
				{
					newColor = this.color_none;
				}
				else if (num < 0f)
				{
					newColor = Color.Lerp(this.clr_neutral_neg, this.clr_worst, num / RelationUtils.Def.minRelationship);
				}
				else
				{
					newColor = Color.Lerp(this.clr_neutral_pos, this.clr_best, num / RelationUtils.Def.maxRelationship);
				}
				this.SetRealmColor(i, newColor);
			}
		}
	}

	// Token: 0x04000937 RID: 2359
	public Color clr_worst;

	// Token: 0x04000938 RID: 2360
	public Color clr_neutral_neg;

	// Token: 0x04000939 RID: 2361
	public Color clr_neutral_pos;

	// Token: 0x0400093A RID: 2362
	public Color clr_best;

	// Token: 0x0400093B RID: 2363
	public Color clr_seleciton;
}
