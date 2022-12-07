using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class UIKingdomFood : MonoBehaviour
{
	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06001FD6 RID: 8150 RVA: 0x00125895 File Offset: 0x00123A95
	// (set) Token: 0x06001FD7 RID: 8151 RVA: 0x0012589D File Offset: 0x00123A9D
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x06001FD8 RID: 8152 RVA: 0x001258A6 File Offset: 0x00123AA6
	private void Start()
	{
		this.m_FoodValue = global::Common.GetComponent<TextMeshProUGUI>(this, "Label/Num");
		this.UpdateTexts();
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x001258C0 File Offset: 0x00123AC0
	private void Update()
	{
		if (this.Data != BaseUI.LogicKingdom())
		{
			this.SetKingdom(BaseUI.LogicKingdom());
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		float food = this.Data.GetFood();
		if (this.m_CurrentFood != food)
		{
			this.m_CurrentFood = food;
			this.UpdateTexts();
		}
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x00125911 File Offset: 0x00123B11
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		this.Data = kingdom;
		this.BuildTooltip();
		this.UpdateTexts();
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x00125926 File Offset: 0x00123B26
	public void BuildTooltip()
	{
		Tooltip.Get(base.gameObject, true).SetDef("KingdomFoodIncome", new Vars(this.Data));
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x00125950 File Offset: 0x00123B50
	private void UpdateTexts()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_FoodValue == null)
		{
			return;
		}
		Vars vars = new Vars(this.Data);
		vars.Set<float>("food", Mathf.Round(this.Data.GetFood()));
		UIText.SetTextKey(this.m_FoodValue, "Kingdom.Food", vars, null);
	}

	// Token: 0x0400151B RID: 5403
	private TextMeshProUGUI m_FoodValue;

	// Token: 0x0400151C RID: 5404
	private float m_CurrentFood;
}
