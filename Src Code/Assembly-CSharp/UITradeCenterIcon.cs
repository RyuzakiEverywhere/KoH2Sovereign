using System;
using Logic;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class UITradeCenterIcon : ObjectIcon
{
	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06002E6E RID: 11886 RVA: 0x0017FAD3 File Offset: 0x0017DCD3
	// (set) Token: 0x06002E6F RID: 11887 RVA: 0x0017FADB File Offset: 0x0017DCDB
	public Logic.Realm Data { get; private set; }

	// Token: 0x06002E70 RID: 11888 RVA: 0x0017FAE4 File Offset: 0x0017DCE4
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.Init();
		if (obj is Logic.Realm)
		{
			this.Data = (obj as Logic.Realm);
		}
		if (obj is TradeCenter)
		{
			this.Data = ((obj as TradeCenter).obj as Logic.Realm);
		}
		this.Refresh();
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x0017FB37 File Offset: 0x0017DD37
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_TradeCenterIcon != null)
		{
			this.m_TradeCenterIcon.onClick = new BSGButton.OnClick(this.HandleTradeCenterSelect);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x0017FB78 File Offset: 0x0017DD78
	private void Refresh()
	{
		if (this.Data == null)
		{
			return;
		}
		Tooltip.Get(base.gameObject, true).SetDef("TradeCenterTooltip", new Vars(this.Data.tradeCenter));
		UIKingdomIcon kingdomIcon = this.m_KingdomIcon;
		if (kingdomIcon == null)
		{
			return;
		}
		kingdomIcon.SetObject(this.Data.GetKingdom(), null);
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x0017FBD5 File Offset: 0x0017DDD5
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

	// Token: 0x06002E74 RID: 11892 RVA: 0x0017FBF0 File Offset: 0x0017DDF0
	private void HandleTradeCenterSelect(BSGButton bnt)
	{
		this.SelectTradeCenter();
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x0017FBF8 File Offset: 0x0017DDF8
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

	// Token: 0x04001F69 RID: 8041
	[UIFieldTarget("id_TradeKingdom")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001F6A RID: 8042
	[UIFieldTarget("id_TradeCenterIcon")]
	private BSGButton m_TradeCenterIcon;

	// Token: 0x04001F6B RID: 8043
	private bool m_Initialzied;
}
