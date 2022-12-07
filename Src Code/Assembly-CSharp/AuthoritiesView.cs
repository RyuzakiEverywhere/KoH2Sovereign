using System;
using Logic;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class AuthoritiesView : PoliticalView
{
	// Token: 0x06000B8D RID: 2957 RVA: 0x000821C0 File Offset: 0x000803C0
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.AuthorityColorTexture = global::Defs.GetObj<Texture2D>(field, "authority_color_texture", null);
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x000821DC File Offset: 0x000803DC
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.AuthorityColorTexture == null)
		{
			Debug.Log("Authority Color Texture is not set");
			return;
		}
		int height = this.AuthorityColorTexture.height;
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Kingdom kingdom = this.realms[i - 1].GetKingdom();
			float num;
			if (this.kSrc == null || kingdom == null)
			{
				num = 0f;
			}
			else if (kingdom == this.kSrc)
			{
				num = 100f;
			}
			else
			{
				num = this.kSrc.logic.GetInfluenceIn(kingdom.logic);
			}
			int y;
			if (num <= 0f)
			{
				y = height - 1;
			}
			else if (num >= 100f)
			{
				y = 0;
			}
			else
			{
				y = (int)((float)(height - 2) * (1f - num / 100f));
			}
			Color color = this.AuthorityColorTexture.GetPixel(1, y);
			color *= 0.6f;
			this.SetRealmColor(i, color);
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x000822DC File Offset: 0x000804DC
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
		global::Kingdom kingdom = global::Kingdom.Get(worldMap.SrcKingdom);
		if (kingdom == null || kingdom.logic == null)
		{
			return false;
		}
		global::Kingdom kingdom2 = global::Kingdom.Get(base.GetHighlightedKingdom());
		if (kingdom2 == null || kingdom2.logic == null)
		{
			tooltip.text = "";
			return false;
		}
		tooltip.text = kingdom.logic.GetInfluenceIn(kingdom2.logic).ToString();
		return false;
	}

	// Token: 0x04000900 RID: 2304
	public Texture2D AuthorityColorTexture;
}
