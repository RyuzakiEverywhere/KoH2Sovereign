using System;
using Logic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000212 RID: 530
public class UIKingdomTaxRate : MonoBehaviour
{
	// Token: 0x0600201F RID: 8223 RVA: 0x00127211 File Offset: 0x00125411
	public void SetKingdom(int kingdom)
	{
		this.kingdom = kingdom;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x0012721F File Offset: 0x0012541F
	private void Start()
	{
		this.InitButtons();
		this.Shown(false);
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x0012722E File Offset: 0x0012542E
	private void Update()
	{
		this.UpdateTaxRate();
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x00127238 File Offset: 0x00125438
	public void UpdateTaxRate()
	{
		global::Kingdom kingdom = global::Kingdom.Get(this.kingdom);
		if (kingdom == null || kingdom.logic == null)
		{
			return;
		}
		this.HighlightTaxRateButton(kingdom.logic.taxLevel);
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x00127274 File Offset: 0x00125474
	private void InitButtons()
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "Button", true, true);
		if (gameObject == null)
		{
			return;
		}
		Button component = gameObject.GetComponent<Button>();
		if (component == null)
		{
			return;
		}
		this.taxRates = global::Common.FindChildByName(base.gameObject, "TaxRates", true, true);
		if (this.taxRates == null)
		{
			return;
		}
		component.onClick.AddListener(delegate()
		{
			this.taxRates.SetActive(!this.taxRates.activeSelf);
		});
		global::Kingdom k = global::Kingdom.Get(this.kingdom);
		if (k == null || k.logic == null)
		{
			return;
		}
		if (this.taxRates == null)
		{
			return;
		}
		int idx = 0;
		this.HighlightTaxRateButton(k.logic.taxLevel);
		if (WorldMap.Get() == null)
		{
			return;
		}
		UnityAction <>9__1;
		foreach (object obj in this.taxRates.transform)
		{
			Button component2 = ((Transform)obj).GetComponent<Button>();
			if (!(component2 == null))
			{
				UnityEvent onClick = component2.onClick;
				UnityAction call;
				if ((call = <>9__1) == null)
				{
					call = (<>9__1 = delegate()
					{
						this.taxRates.SetActive(false);
						if (!k.logic.IsAuthority())
						{
							k.logic.SendEvent(new Logic.Kingdom.ChangeTaxRateEvent(idx));
							return;
						}
						k.logic.SetTaxRate(idx, true);
						this.HighlightTaxRateButton(k.logic.taxLevel);
						DT.Field soundsDef = BaseUI.soundsDef;
						BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("tax_rate_changed", null, "", true, true, true, '.') : null, null);
						LabelUpdater labelUpdater = LabelUpdater.Get(true);
						if (labelUpdater == null)
						{
							return;
						}
						labelUpdater.UpdateGoldIncomeLabelsText();
					});
				}
				onClick.AddListener(call);
				int idx2 = idx;
				idx = idx2 + 1;
			}
		}
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x001273FC File Offset: 0x001255FC
	private void HighlightTaxRateButton(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.taxRates.transform.childCount; i++)
		{
			Transform child = this.taxRates.transform.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				Image component = child.GetChild(j).GetComponent<Image>();
				if (!(component == null))
				{
					if (num == idx)
					{
						component.color = new Color32(byte.MaxValue, 153, 0, byte.MaxValue);
					}
					else
					{
						component.color = new Color32(98, 86, 68, byte.MaxValue);
					}
				}
			}
			num++;
		}
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x001274AE File Offset: 0x001256AE
	public void Shown(bool shown)
	{
		if (this.taxRates != null && this.taxRates.activeSelf != shown)
		{
			this.taxRates.SetActive(shown);
		}
	}

	// Token: 0x04001550 RID: 5456
	private GameObject taxRates;

	// Token: 0x04001551 RID: 5457
	public global::Kingdom.ID kingdom = 1;
}
