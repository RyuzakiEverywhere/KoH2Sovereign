using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002DF RID: 735
public class UITradeCenterInfo : MonoBehaviour
{
	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06002E77 RID: 11895 RVA: 0x0017FC90 File Offset: 0x0017DE90
	// (set) Token: 0x06002E78 RID: 11896 RVA: 0x0017FC98 File Offset: 0x0017DE98
	public Logic.Realm Data { get; private set; }

	// Token: 0x06002E79 RID: 11897 RVA: 0x0017FCA1 File Offset: 0x0017DEA1
	public void SetObject(Logic.Realm realm)
	{
		this.Init();
		this.Data = realm;
		if (this.m_TradeCenterIcon != null)
		{
			this.m_TradeCenterIcon.onClick = new BSGButton.OnClick(this.HandleTradeCenterSelect);
		}
		this.Refresh();
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x0017FCDC File Offset: 0x0017DEDC
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		LayoutElement component = base.GetComponent<LayoutElement>();
		if (component != null)
		{
			this.pw = component.preferredWidth;
			this.ph = component.preferredHeight;
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x0017FD28 File Offset: 0x0017DF28
	private void Refresh()
	{
		UIText.SetTextKey(this.m_Caption, "TradeCenter.caption", null, null);
		this.m_Group_Self.gameObject.gameObject.SetActive(false);
		this.m_Group_Influances.gameObject.gameObject.SetActive(false);
		this.m_Group_NoInfulance.gameObject.gameObject.SetActive(false);
		if (this.Data == null)
		{
			this.SetAsNotInfluencedByTradeCenter();
			return;
		}
		if (this.Data.IsTradeCenter())
		{
			this.SetAsTradeCenter();
			return;
		}
		this.SetAsNotInfluencedByTradeCenter();
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x0017FDB2 File Offset: 0x0017DFB2
	private void Update()
	{
		if (!this.m_Initialzied)
		{
			return;
		}
		if (UnityEngine.Time.frameCount % 10 == 0)
		{
			this.Refresh();
		}
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x0017FDD0 File Offset: 0x0017DFD0
	private void SetAsTradeCenter()
	{
		Tooltip.Get(base.gameObject, true).SetDef("TradeCenterTooltip", new Vars(this.Data.tradeCenter));
		GameObject group_Self = this.m_Group_Self;
		if (group_Self != null)
		{
			group_Self.gameObject.gameObject.SetActive(true);
		}
		UIKingdomIcon kingdomIcon = this.m_KingdomIcon;
		if (kingdomIcon != null)
		{
			kingdomIcon.SetObject(this.Data.GetKingdom(), null);
		}
		if (this.m_TradeIncome != null)
		{
			string key = (BaseUI.LogicKingdom() == this.Data.GetKingdom()) ? "TradeCenter.own.income" : "TradeCenter.enemy.income";
			UIText.SetTextKey(this.m_TradeIncome, key, new Vars(this.Data), null);
		}
		this.Show(true);
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x0017FE94 File Offset: 0x0017E094
	private void SetAsInfluencedByTradeCenter()
	{
		Vars vars = new Vars(this.Data.tradeCenter);
		vars.Set<Logic.Realm>("realm", this.Data);
		Tooltip.Get(base.gameObject, true).SetDef("TradeCenterTooltipInfluenced", vars);
		GameObject group_Influances = this.m_Group_Influances;
		if (group_Influances != null)
		{
			group_Influances.gameObject.gameObject.SetActive(true);
		}
		UIKingdomIcon kingdomIcon = this.m_KingdomIcon;
		if (kingdomIcon != null)
		{
			Logic.Realm data = this.Data;
			object obj;
			if (data == null)
			{
				obj = null;
			}
			else
			{
				TradeCenter tradeCenter = data.tradeCenter;
				if (tradeCenter == null)
				{
					obj = null;
				}
				else
				{
					Logic.Realm realm = tradeCenter.realm;
					obj = ((realm != null) ? realm.GetKingdom() : null);
				}
			}
			kingdomIcon.SetObject(obj, null);
		}
		if (this.m_TradeIncomeFromInfluance != null)
		{
			string key = (BaseUI.LogicKingdom() == this.Data.GetKingdom()) ? "TradeCenter.own.incomeFromInfluances" : "TradeCenter.enemy.incomeFromInfluances";
			UIText.SetTextKey(this.m_TradeIncomeFromInfluance, key, new Vars(this.Data), null);
		}
		this.Show(false);
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x0017FF88 File Offset: 0x0017E188
	private void SetAsNotInfluencedByTradeCenter()
	{
		Tooltip.Get(base.gameObject, true).SetDef("TradeCenterTooltipNotInfluenced", new Vars(this.Data.tradeCenter));
		GameObject group_NoInfulance = this.m_Group_NoInfulance;
		if (group_NoInfulance != null)
		{
			group_NoInfulance.gameObject.gameObject.SetActive(true);
		}
		if (this.m_NoTradeInfluance != null)
		{
			string key = (BaseUI.LogicKingdom() == this.Data.GetKingdom()) ? "TradeCenter.own.noTradeInfluances" : "TradeCenter.enemy.noTradeInfluances";
			UIText.SetTextKey(this.m_NoTradeInfluance, key, new Vars(this.Data), null);
		}
		this.Show(false);
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x00180030 File Offset: 0x0017E230
	private void Show(bool show)
	{
		LayoutElement component = base.GetComponent<LayoutElement>();
		if (component != null)
		{
			component.preferredHeight = (show ? this.pw : 0f);
			component.preferredWidth = (show ? this.ph : 0f);
		}
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x00180079 File Offset: 0x0017E279
	private void HandleTradeCenterSelect(BSGButton bnt)
	{
		this.SelectTradeCenter();
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x00180084 File Offset: 0x0017E284
	private void SelectTradeCenter()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.Data.tradeCenter == null)
		{
			return;
		}
		if (this.Data.tradeCenter.realm == null)
		{
			return;
		}
		if (this.Data.tradeCenter.realm.castle == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		global::Settlement settlement = this.Data.tradeCenter.realm.castle.visuals as global::Settlement;
		if (settlement != null)
		{
			worldUI.SelectObj(settlement.gameObject, false, true, true, true);
		}
	}

	// Token: 0x04001F6D RID: 8045
	[UIFieldTarget("id_TradeKingdom")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001F6E RID: 8046
	[UIFieldTarget("id_NoTradeLabel")]
	private TextMeshProUGUI m_NoTradeLabel;

	// Token: 0x04001F6F RID: 8047
	[UIFieldTarget("id_TradeIncome")]
	private TextMeshProUGUI m_TradeIncome;

	// Token: 0x04001F70 RID: 8048
	[UIFieldTarget("id_TradeIncomeFromInfluance")]
	private TextMeshProUGUI m_TradeIncomeFromInfluance;

	// Token: 0x04001F71 RID: 8049
	[UIFieldTarget("id_NoTradeInfluance")]
	private TextMeshProUGUI m_NoTradeInfluance;

	// Token: 0x04001F72 RID: 8050
	[UIFieldTarget("Group_Self")]
	private GameObject m_Group_Self;

	// Token: 0x04001F73 RID: 8051
	[UIFieldTarget("Group_Influances")]
	private GameObject m_Group_Influances;

	// Token: 0x04001F74 RID: 8052
	[UIFieldTarget("Group_NoInfulance")]
	private GameObject m_Group_NoInfulance;

	// Token: 0x04001F75 RID: 8053
	[UIFieldTarget("id_TradeCenterIcon")]
	private BSGButton m_TradeCenterIcon;

	// Token: 0x04001F76 RID: 8054
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001F77 RID: 8055
	private bool m_Initialzied;

	// Token: 0x04001F79 RID: 8057
	private WaitForSeconds refreshYield = new WaitForSeconds(0.5f);

	// Token: 0x04001F7A RID: 8058
	private float pw;

	// Token: 0x04001F7B RID: 8059
	private float ph;
}
