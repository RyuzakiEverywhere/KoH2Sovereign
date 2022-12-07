using System;
using Logic;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class WorldView : ViewMode
{
	// Token: 0x06000BF1 RID: 3057 RVA: 0x0008623C File Offset: 0x0008443C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.clr_hatch_Enemy1 = global::Defs.GetColor(field, "clr_hatch_Enemy1", Color.red, null);
		this.clr_hatch_Enemy2 = global::Defs.GetColor(field, "clr_hatch_Enemy2", Color.red, null);
		this.clr_hatch_EnemyRebel1 = global::Defs.GetColor(field, "clr_hatch_EnemyRebel1", Color.red, null);
		this.clr_hatch_EnemyRebel2 = global::Defs.GetColor(field, "clr_hatch_EnemyRebel2", Color.red, null);
		this.clr_hatch_Neutral1 = global::Defs.GetColor(field, "clr_hatch_Neutral1", Color.red, null);
		this.clr_hatch_Neutral2 = global::Defs.GetColor(field, "clr_hatch_Neutral2", Color.red, null);
		this.clr_hatch_Ally1 = global::Defs.GetColor(field, "clr_hatch_Ally1", Color.red, null);
		this.clr_hatch_Ally2 = global::Defs.GetColor(field, "clr_hatch_Ally2", Color.red, null);
		this.clr_hatch_AllyLoyalist1 = global::Defs.GetColor(field, "clr_hatch_AllyLoyalist1", Color.red, null);
		this.clr_hatch_AllyLoyalist2 = global::Defs.GetColor(field, "clr_hatch_AllyLoyalist2", Color.red, null);
		this.clr_hatch_Own1 = global::Defs.GetColor(field, "clr_hatch_Own1", Color.red, null);
		this.clr_hatch_Own2 = global::Defs.GetColor(field, "clr_hatch_Own2", Color.red, null);
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00086364 File Offset: 0x00084564
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.wm == null)
		{
			return;
		}
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			Logic.Realm logic = realm.logic;
			Color mapColor = realm.MapColor;
			if (this.uik != null && logic != null && (logic.IsOccupied() || logic.IsDisorder()))
			{
				this.greenChannel[i] = new Color(0f, 0f, 0f, 0f);
				if (logic.IsDisorder())
				{
					this.redChannel[i] = this.clr_hatch_Enemy1;
					this.blueChannel[i] = new Color(0f, 0f, 0f, 0f);
				}
				else if (logic.IsEnemy(this.uik))
				{
					Logic.Kingdom kingdom = logic.controller.GetKingdom();
					if (kingdom != null && (kingdom.type == Logic.Kingdom.Type.RebelFaction || kingdom.type == Logic.Kingdom.Type.LoyalistsFaction))
					{
						this.redChannel[i] = this.clr_hatch_EnemyRebel2;
						this.blueChannel[i] = this.clr_hatch_EnemyRebel1;
					}
					else
					{
						this.redChannel[i] = this.clr_hatch_Enemy2;
						this.blueChannel[i] = this.clr_hatch_Enemy1;
					}
				}
				else if (logic.IsAllyOrVassal(this.uik))
				{
					if (logic.controller.GetKingdom().type == Logic.Kingdom.Type.LoyalistsFaction)
					{
						this.redChannel[i] = this.clr_hatch_AllyLoyalist2;
						this.blueChannel[i] = this.clr_hatch_AllyLoyalist1;
					}
					else
					{
						this.redChannel[i] = this.clr_hatch_Ally2;
						this.blueChannel[i] = this.clr_hatch_Ally1;
					}
				}
				else if (logic.IsOwnStance(this.uik))
				{
					this.redChannel[i] = this.clr_hatch_Own2;
					this.blueChannel[i] = this.clr_hatch_Own1;
				}
				else
				{
					this.redChannel[i] = this.clr_hatch_Neutral2;
					this.blueChannel[i] = this.clr_hatch_Neutral1;
				}
			}
			else
			{
				this.redChannel[i] = new Color(0f, 0f, 0f, 0f);
				this.greenChannel[i] = new Color(0f, 0f, 0f, 0f);
				this.blueChannel[i] = new Color(0f, 0f, 0f, 0f);
			}
		}
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0008660B File Offset: 0x0008480B
	public override void SetStripeTextures(bool secondary)
	{
		if (secondary)
		{
			base.SetStripeTextures(secondary);
			return;
		}
		Shader.SetGlobalTexture("_WVStripeR", this.StripeR);
		Shader.SetGlobalTexture("_WVStripeG", this.StripeG);
		Shader.SetGlobalTexture("_WVStripeB", this.StripeB);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00080D0C File Offset: 0x0007EF0C
	public WorldView() : base(null, null)
	{
	}

	// Token: 0x04000952 RID: 2386
	protected Color clr_hatch_Enemy1;

	// Token: 0x04000953 RID: 2387
	protected Color clr_hatch_Enemy2;

	// Token: 0x04000954 RID: 2388
	protected Color clr_hatch_EnemyRebel1;

	// Token: 0x04000955 RID: 2389
	protected Color clr_hatch_EnemyRebel2;

	// Token: 0x04000956 RID: 2390
	protected Color clr_hatch_Neutral1;

	// Token: 0x04000957 RID: 2391
	protected Color clr_hatch_Neutral2;

	// Token: 0x04000958 RID: 2392
	protected Color clr_hatch_Ally1;

	// Token: 0x04000959 RID: 2393
	protected Color clr_hatch_Ally2;

	// Token: 0x0400095A RID: 2394
	protected Color clr_hatch_AllyLoyalist1;

	// Token: 0x0400095B RID: 2395
	protected Color clr_hatch_AllyLoyalist2;

	// Token: 0x0400095C RID: 2396
	protected Color clr_hatch_Own1;

	// Token: 0x0400095D RID: 2397
	protected Color clr_hatch_Own2;
}
