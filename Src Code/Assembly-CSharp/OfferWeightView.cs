using System;
using Logic;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class OfferWeightView : PoliticalView
{
	// Token: 0x06000BB6 RID: 2998 RVA: 0x00083C54 File Offset: 0x00081E54
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.clr_worst = global::Defs.GetColor(field, "clr_worst", Color.red, null);
		this.clr_neutral = global::Defs.GetColor(field, "clr_neutral", Color.yellow, null);
		this.clr_best = global::Defs.GetColor(field, "clr_best", Color.green, null);
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x00083CB0 File Offset: 0x00081EB0
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (WorldUI.Get() != null)
		{
			global::Kingdom.ID kingdom = WorldUI.Get().kingdom;
		}
		int[] array = new int[1000];
		for (int i = 0; i < 1000; i++)
		{
			array[i] = -1;
		}
		for (int j = 1; j <= this.realms.Count; j++)
		{
			global::Kingdom kingdom2 = this.realms[j - 1].GetKingdom();
			if (this.kSrc == null || this.kSrc.logic == null || this.kSrc.logic.ai == null)
			{
				this.SetRealmColor(j, Color.black);
			}
			else if (kingdom2 == null || kingdom2.logic == null)
			{
				this.SetRealmColor(j, Color.red);
			}
			else if (kingdom2 == this.kSrc)
			{
				this.SetRealmColor(j, Color.blue);
			}
			else
			{
				Logic.Kingdom logic = this.kSrc.logic;
				Logic.Kingdom logic2 = kingdom2.logic;
				int num = array[logic2.id];
				if (num == -1)
				{
					if (logic.ai != null)
					{
						num = logic.ai.CalcDiplomaticImportance(logic2);
					}
					else
					{
						num = 0;
					}
					array[logic2.id] = num;
				}
				Color newColor;
				if (num == 0)
				{
					newColor = Color.clear;
				}
				else
				{
					float t = 0.1f;
					if (num > 200)
					{
						t = 0.2f;
					}
					if (num > 1000)
					{
						t = 0.4f;
					}
					if (num > 2000)
					{
						t = 0.5f;
					}
					if (num > 5000)
					{
						t = 0.7f;
					}
					if (num > 8000)
					{
						t = 0.8f;
					}
					if (num > 12000)
					{
						t = 1f;
					}
					newColor = Color.Lerp(this.clr_worst, this.clr_best, t);
				}
				this.SetRealmColor(j, newColor);
			}
		}
	}

	// Token: 0x04000921 RID: 2337
	public Color clr_worst;

	// Token: 0x04000922 RID: 2338
	public Color clr_neutral;

	// Token: 0x04000923 RID: 2339
	public Color clr_best;
}
