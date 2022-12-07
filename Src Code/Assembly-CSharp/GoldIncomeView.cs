using System;
using Logic;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class GoldIncomeView : PoliticalView
{
	// Token: 0x06000BA9 RID: 2985 RVA: 0x0008352E File Offset: 0x0008172E
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.incomeColorTexture = global::Defs.GetObj<Texture2D>(field, "income_color_texture", null);
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0008354C File Offset: 0x0008174C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.incomeColorTexture == null)
		{
			Debug.Log("Income Color Texture is not set");
			return;
		}
		int height = this.incomeColorTexture.height;
		if (this.kSrc != null && this.kSrc.logic != null)
		{
			this.kSrc.logic.CalcNormalizedGoldIncome();
		}
		LabelUpdater labelUpdater = LabelUpdater.Get(true);
		if (labelUpdater != null)
		{
			labelUpdater.UpdateIncomeLabels();
		}
		if (WorldUI.Get() != null)
		{
			global::Kingdom.ID kingdom = WorldUI.Get().kingdom;
		}
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			Color newColor = Color.clear;
			if (this.kSrc != null && realm.GetKingdom() == this.kSrc && realm.logic != null)
			{
				float num = realm.logic.normalizedGoldIncome;
				if (realm.logic.income[ResourceType.Gold] < 3f)
				{
					num = 0.9f;
				}
				if (realm.logic.income[ResourceType.Gold] < 2.5f)
				{
					num = 0.8f;
				}
				if (realm.logic.income[ResourceType.Gold] < 1.5f)
				{
					num = 0.7f;
				}
				if (realm.logic.income[ResourceType.Gold] < 1f)
				{
					num = 0.4f;
				}
				if (realm.logic.income[ResourceType.Gold] < 0.5f)
				{
					num = 0.2f;
				}
				int num2 = (int)(num * (float)height);
				if (num2 == height)
				{
					num2--;
				}
				newColor = this.incomeColorTexture.GetPixel(1, height - num2);
			}
			newColor.a = 1f;
			this.SetRealmColor(i, newColor);
		}
	}

	// Token: 0x04000917 RID: 2327
	public Texture2D incomeColorTexture;
}
