using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class UIKingdomIncomeWindow : UIWindow, IListener
{
	// Token: 0x06002188 RID: 8584 RVA: 0x00130A85 File Offset: 0x0012EC85
	public override string GetDefId()
	{
		return UIKingdomIncomeWindow.def_id;
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06002189 RID: 8585 RVA: 0x00130A8C File Offset: 0x0012EC8C
	// (set) Token: 0x0600218A RID: 8586 RVA: 0x00130A94 File Offset: 0x0012EC94
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x0600218B RID: 8587 RVA: 0x00130AA0 File Offset: 0x0012ECA0
	public void SetObject(Logic.Kingdom kingdom)
	{
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = kingdom;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
		}
		UICommon.FindComponents(this, false);
		UIIncomePanel uiincomePanel = this.incomes_panel;
		if (uiincomePanel != null)
		{
			uiincomePanel.SetObject(this.Kingdom.incomes[ResourceType.Gold]);
		}
		UIIncomePanel uiincomePanel2 = this.upkeeps_panel;
		if (uiincomePanel2 != null)
		{
			uiincomePanel2.SetObject(this.Kingdom.upkeeps[ResourceType.Gold]);
		}
		this.RefreshStatics();
		this.RefreshDynamics();
		this.SetupTaxRates();
		this.RefreshTaxRateButtons();
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x00130B40 File Offset: 0x0012ED40
	private void RefreshStatics()
	{
		if (this.m_CloseButtons != null)
		{
			for (int i = 0; i < this.m_CloseButtons.Length; i++)
			{
				this.m_CloseButtons[i].onClick = new BSGButton.OnClick(this.Close);
				this.m_CloseButtons[i].SetAudioSet("DefaultAudioSetPaper");
			}
		}
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_KingdomIcon != null)
		{
			this.m_KingdomIcon.SetObject(this.Kingdom, null);
		}
		UIText.SetTextKey(this.m_KingdomName, "KingdomIncomeWindow.Kingdom", new Vars(this.Kingdom), null);
		if (this.m_WarTaxesButton != null)
		{
			Tooltip.Get(this.m_WarTaxesButton.gameObject, true).SetText("KingdomIncomeWindow.WarTaxes", null, new Vars(this.WarTaxes));
			this.SetFieldLabel(this.m_WarTaxesButton.transform.parent.gameObject, "id_Button_Wartaxes", "KingdomIncomeWindow.Buttons.WarTax", null);
			this.m_WarTaxesButton.onClick = new BSGButton.OnClick(this.HandleWarTaxes);
		}
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x00130C54 File Offset: 0x0012EE54
	private void RefreshDynamics()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		this.PopulateIncome();
		this.PopulateExpenses();
		this.PopulateBalance();
		if (this.m_CurrentTaxRate != null)
		{
			UIText.SetTextKey(this.m_CurrentTaxRate, "KingdomIncomeWindow.CurrentTax", new Vars(this.Kingdom.GetVar("tax_rate", null, true)), null);
		}
		this.RefreshTaxRateButtons();
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x00130CB8 File Offset: 0x0012EEB8
	private Vars CreateValueVars(float value)
	{
		return new Vars((value == 0f) ? "#<color=#353535>--</color>" : ("#" + DT.FloatToStr(value, 1)));
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x00130CE4 File Offset: 0x0012EEE4
	private void PopulateIncome()
	{
		UIIncomePanel uiincomePanel = this.incomes_panel;
		if (uiincomePanel != null)
		{
			GameObject gameObject = uiincomePanel.gameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
		UIIncomePanel uiincomePanel2 = this.incomes_panel;
		if (uiincomePanel2 == null)
		{
			return;
		}
		uiincomePanel2.Refresh();
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x00130D13 File Offset: 0x0012EF13
	private void PopulateExpenses()
	{
		UIIncomePanel uiincomePanel = this.upkeeps_panel;
		if (uiincomePanel != null)
		{
			GameObject gameObject = uiincomePanel.gameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
		UIIncomePanel uiincomePanel2 = this.upkeeps_panel;
		if (uiincomePanel2 == null)
		{
			return;
		}
		uiincomePanel2.Refresh();
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x00130D44 File Offset: 0x0012EF44
	private void PopulateBalance()
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "Group_Balance", true, true);
		if (gameObject == null)
		{
			return;
		}
		float num = this.ClampGoldValue(this.Kingdom.income[ResourceType.Gold] - this.Kingdom.expenses[ResourceType.Gold]);
		string val = (num < 0f) ? string.Format("#<color=#93270E>{0}</color>", num) : string.Format("#<color=#31610A>{0}</color>", num);
		this.SetFieldValue(gameObject, "id_Total", "KingdomIncomeWindow.Value", new Vars(val));
		this.SetFieldLabel(gameObject, "id_Total", "KingdomIncomeWindow.Balance.Label", null);
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x00130DF4 File Offset: 0x0012EFF4
	private void SetFieldValue(GameObject root, string field, string key, Vars v)
	{
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(global::Common.FindChildByName(root, field, true, true), "Value");
		if (textMeshProUGUI != null)
		{
			UIText.SetTextKey(textMeshProUGUI, key, v, null);
		}
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x00130E28 File Offset: 0x0012F028
	private void SetFieldLabel(GameObject root, string field, string key, Vars v)
	{
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(global::Common.FindChildByName(root, field, true, true), "Label");
		if (textMeshProUGUI != null)
		{
			UIText.SetTextKey(textMeshProUGUI, key, v, null);
		}
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x00130E5C File Offset: 0x0012F05C
	private void HandleTaxChange(int idx)
	{
		if (this.Kingdom.taxLevel < idx)
		{
			MessageWnd.Create(global::Defs.GetDefField("ConfirmIncreaseTaxMessage", null), new Vars(this.Kingdom), null, delegate(MessageWnd wnd, string btn_id)
			{
				if (btn_id == "ok")
				{
					this.ChnageTaxRate(idx);
				}
				wnd.Close(false);
				return true;
			});
			return;
		}
		this.ChnageTaxRate(idx);
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x00130ECB File Offset: 0x0012F0CB
	private void ChnageTaxRate(int idx)
	{
		if (!this.Kingdom.IsAuthority())
		{
			this.Kingdom.SendEvent(new Logic.Kingdom.ChangeTaxRateEvent(idx));
			return;
		}
		this.Kingdom.SetTaxRate(idx, true);
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x00130EF9 File Offset: 0x0012F0F9
	private float ClampGoldValue(float orig)
	{
		if (orig <= 10f)
		{
			return (float)Math.Round((double)orig, 1);
		}
		return (float)Math.Round((double)orig, MidpointRounding.AwayFromZero);
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x00130F18 File Offset: 0x0012F118
	private void SetupTaxRates()
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "Group_Taxes", true, true);
		if (gameObject == null)
		{
			return;
		}
		this.m_TaxContainer = global::Common.FindChildByName(gameObject, "id_TaxContainer", true, true);
		if (this.m_TaxContainer == null)
		{
			return;
		}
		this.m_TaxButtonPrototype = global::Common.FindChildByName(this.m_TaxContainer, "id_TaxButton", true, true);
		if (this.m_TaxButtonPrototype == null)
		{
			return;
		}
		DT.Field field = this.Kingdom.def.FindChild("tax_rates", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		int num = field.NumValues();
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_TaxButtonPrototype, this.m_TaxContainer.transform);
			BSGButton component = gameObject2.GetComponent<BSGButton>();
			if (component != null)
			{
				int idx = i;
				BSGButton bsgbutton = component;
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(delegate(BSGButton b)
				{
					this.HandleTaxChange(idx);
				}));
				component.AllowSelection(true);
			}
			TextMeshProUGUI componentInChildren = gameObject2.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren != null)
			{
				componentInChildren.text = field.Value(i, null, true, true).Int(0) + "%";
			}
			this.m_TaxButtons.Add(gameObject2);
		}
		this.m_TaxButtonPrototype.gameObject.SetActive(false);
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x00131090 File Offset: 0x0012F290
	private void RefreshTaxRateButtons()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_TaxButtons == null || this.m_TaxButtons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_TaxButtons.Count; i++)
		{
			BSGButton component = this.m_TaxButtons[i].GetComponent<BSGButton>();
			if (component != null)
			{
				component.SetSelected(this.Kingdom.taxLevel == i, false);
			}
		}
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x00131102 File Offset: 0x0012F302
	private void HandleWarTaxes(BSGButton btn)
	{
		Debug.Log(">> HandleWarTaxes <<");
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("collect_war_tax", null, "", true, true, true, '.') : null, null);
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x00131138 File Offset: 0x0012F338
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "tax_rate_changed")
		{
			this.RefreshTaxRateButtons();
			if ((bool)param)
			{
				DT.Field soundsDef = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("tax_rate_increased", null, "", true, true, true, '.') : null, null);
			}
			else
			{
				DT.Field soundsDef2 = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("tax_rate_decreased", null, "", true, true, true, '.') : null, null);
			}
		}
		if (message == "income_changed")
		{
			this.RefreshDynamics();
		}
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void Close(BSGButton btn)
	{
		this.Close(false);
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x001311C3 File Offset: 0x0012F3C3
	protected override void OnDestroy()
	{
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x001311DF File Offset: 0x0012F3DF
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIKingdomIncomeWindow.def_id, null);
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x001311EC File Offset: 0x0012F3EC
	public static UIKingdomIncomeWindow Create(Logic.Kingdom kingdom, RectTransform parent, UIWindow.OnClose onClose)
	{
		if (kingdom == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject prefab = UICommon.GetPrefab("KingdomIncomeWindow", null);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prefab, parent, false, "");
		UIKingdomIncomeWindow uikingdomIncomeWindow = gameObject.GetComponent<UIKingdomIncomeWindow>();
		if (uikingdomIncomeWindow == null)
		{
			uikingdomIncomeWindow = gameObject.AddComponent<UIKingdomIncomeWindow>();
		}
		uikingdomIncomeWindow.SetObject(kingdom);
		UIKingdomIncomeWindow uikingdomIncomeWindow2 = uikingdomIncomeWindow;
		uikingdomIncomeWindow2.on_close = (UIWindow.OnClose)Delegate.Combine(uikingdomIncomeWindow2.on_close, onClose);
		uikingdomIncomeWindow.Open();
		return uikingdomIncomeWindow;
	}

	// Token: 0x0400166F RID: 5743
	private static string def_id = "KingdomIncomeWindow";

	// Token: 0x04001670 RID: 5744
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001671 RID: 5745
	[UIFieldTarget("id_Button_Wartaxes")]
	private BSGButton m_WarTaxesButton;

	// Token: 0x04001672 RID: 5746
	[UIFieldTarget("id_Button_Close")]
	private BSGButton[] m_CloseButtons;

	// Token: 0x04001673 RID: 5747
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x04001674 RID: 5748
	[UIFieldTarget("id_Value_TotalBalance")]
	private TextMeshProUGUI m_TotalBalance;

	// Token: 0x04001675 RID: 5749
	[UIFieldTarget("id_Incomes")]
	private UIIncomePanel incomes_panel;

	// Token: 0x04001676 RID: 5750
	[UIFieldTarget("id_Upkeeps")]
	private UIIncomePanel upkeeps_panel;

	// Token: 0x04001677 RID: 5751
	[UIFieldTarget("id_Value_Income")]
	private TextMeshProUGUI m_Income;

	// Token: 0x04001678 RID: 5752
	[UIFieldTarget("id_Value_Taxes")]
	private TextMeshProUGUI m_taxes;

	// Token: 0x04001679 RID: 5753
	[UIFieldTarget("id_Value_TownTribute")]
	private TextMeshProUGUI m_TownTribute;

	// Token: 0x0400167A RID: 5754
	[UIFieldTarget("id_Value_RoyalLands")]
	private TextMeshProUGUI m_RoyalLands;

	// Token: 0x0400167B RID: 5755
	[UIFieldTarget("id_Label_Taxes")]
	private TextMeshProUGUI m_Label_Taxes;

	// Token: 0x0400167C RID: 5756
	[UIFieldTarget("id_Value_RoyalTrade")]
	private TextMeshProUGUI m_RoyalTrade;

	// Token: 0x0400167D RID: 5757
	[UIFieldTarget("id_Value_VassalTributes")]
	private TextMeshProUGUI m_VassalIncome;

	// Token: 0x0400167E RID: 5758
	[UIFieldTarget("id_Value_KnightSkills")]
	private TextMeshProUGUI m_KnightSkill;

	// Token: 0x0400167F RID: 5759
	[UIFieldTarget("id_Value_VassalTributes")]
	private TextMeshProUGUI m_VassalTributes;

	// Token: 0x04001680 RID: 5760
	[UIFieldTarget("id_Value_Religion")]
	private TextMeshProUGUI m_Religion;

	// Token: 0x04001681 RID: 5761
	[UIFieldTarget("id_Value_KingdomTrade")]
	private TextMeshProUGUI m_Trade;

	// Token: 0x04001682 RID: 5762
	[UIFieldTarget("id_Value_LocalTrade")]
	private TextMeshProUGUI m_LocalTrade;

	// Token: 0x04001683 RID: 5763
	[UIFieldTarget("id_Value_ForeignTrade")]
	private TextMeshProUGUI m_ForeignTrade;

	// Token: 0x04001684 RID: 5764
	[UIFieldTarget("id_Value_Expenses")]
	private TextMeshProUGUI m_Expenses;

	// Token: 0x04001685 RID: 5765
	[UIFieldTarget("id_Value_Knights")]
	private TextMeshProUGUI m_Knights;

	// Token: 0x04001686 RID: 5766
	[UIFieldTarget("id_Value_VassalTributesExpense")]
	private TextMeshProUGUI m_VassalTributesExpense;

	// Token: 0x04001687 RID: 5767
	[UIFieldTarget("id_Value_Inflation")]
	private TextMeshProUGUI m_Inflation;

	// Token: 0x04001688 RID: 5768
	[UIFieldTarget("id_Value_SovereignTax")]
	private TextMeshProUGUI m_Sovereign;

	// Token: 0x04001689 RID: 5769
	[UIFieldTarget("id_Value_WarTax")]
	private TextMeshProUGUI m_WarTax;

	// Token: 0x0400168A RID: 5770
	[UIFieldTarget("id_Value_CrownAuthorityIncome")]
	private TextMeshProUGUI m_CrownAuthorityIncome;

	// Token: 0x0400168B RID: 5771
	[UIFieldTarget("id_Value_CrownAuthorityExpense")]
	private TextMeshProUGUI m_CrownAuthorityExpense;

	// Token: 0x0400168C RID: 5772
	[UIFieldTarget("id_CurrentTaxRate")]
	private TextMeshProUGUI m_CurrentTaxRate;

	// Token: 0x0400168D RID: 5773
	[SerializeField]
	private GameObject ExtendedInfoPrototype;

	// Token: 0x0400168F RID: 5775
	private float WarTaxes = 1567f;

	// Token: 0x04001690 RID: 5776
	private const string noneValue = "#<color=#353535>--</color>";

	// Token: 0x04001691 RID: 5777
	private GameObject m_TaxButtonPrototype;

	// Token: 0x04001692 RID: 5778
	private GameObject m_TaxContainer;

	// Token: 0x04001693 RID: 5779
	private List<GameObject> m_TaxButtons = new List<GameObject>();
}
