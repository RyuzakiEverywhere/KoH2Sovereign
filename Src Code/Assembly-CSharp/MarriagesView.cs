using System;
using Logic;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class MarriagesView : PoliticalView
{
	// Token: 0x06000BAE RID: 2990 RVA: 0x0008381C File Offset: 0x00081A1C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.color_selection = global::Defs.GetColor(this.def_id, "color_selection", Color.clear);
		this.color_royal_marriage = global::Defs.GetColor(this.def_id, "color_royal_marriage", Color.clear);
		this.color_unmarried_prince_or_king = global::Defs.GetColor(this.def_id, "color_unmarried_prince_or_king", Color.clear);
		this.color_unmarried_princess = global::Defs.GetColor(this.def_id, "color_unmarried_princess", Color.clear);
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0008389C File Offset: 0x00081A9C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		for (int i = 1; i <= this.wm.Realms.Count; i++)
		{
			Color color_none = this.color_none;
			global::Realm realm = this.wm.Realms[i - 1];
			Logic.Kingdom kingdom2;
			if (realm == null)
			{
				kingdom2 = null;
			}
			else
			{
				global::Kingdom kingdom3 = realm.GetKingdom();
				kingdom2 = ((kingdom3 != null) ? kingdom3.logic : null);
			}
			Logic.Kingdom kingdom4 = kingdom2;
			if (kingdom4 == null || kingdom4.royalFamily == null || kingdom4 == kingdom)
			{
				this.SetRealmColor(i, color_none);
			}
			else
			{
				if (this.kSrc != null && kingdom4 == this.kSrc.logic)
				{
					color_none = this.color_selection;
				}
				bool flag = false;
				bool flag2 = kingdom4.GetKing() != null && kingdom4.GetKing().CanMarry();
				for (int j = 0; j < kingdom4.royalFamily.Children.Count; j++)
				{
					Logic.RoyalFamily royalFamily = kingdom4.royalFamily;
					Logic.Character character = (royalFamily != null) ? royalFamily.Children[j] : null;
					if (character != null && character.CanMarry())
					{
						if (character.sex == Logic.Character.Sex.Male)
						{
							flag2 = true;
						}
						else
						{
							flag = true;
						}
					}
				}
				this.redChannel[i] = (flag ? this.color_unmarried_princess : color_none);
				this.blueChannel[i] = (flag2 ? this.color_unmarried_prince_or_king : color_none);
				this.greenChannel[i] = (kingdom4.GetRoyalMarriage(kingdom) ? this.color_royal_marriage : color_none);
			}
		}
	}

	// Token: 0x04000918 RID: 2328
	private Color color_selection;

	// Token: 0x04000919 RID: 2329
	private Color color_royal_marriage;

	// Token: 0x0400091A RID: 2330
	private Color color_unmarried_prince_or_king;

	// Token: 0x0400091B RID: 2331
	private Color color_unmarried_princess;
}
