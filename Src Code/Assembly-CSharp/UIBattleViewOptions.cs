using System;
using Logic;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class UIBattleViewOptions : MonoBehaviour
{
	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06001B64 RID: 7012 RVA: 0x00106058 File Offset: 0x00104258
	// (set) Token: 0x06001B65 RID: 7013 RVA: 0x00106060 File Offset: 0x00104260
	private Logic.Battle m_logic { get; set; }

	// Token: 0x06001B66 RID: 7014 RVA: 0x00106069 File Offset: 0x00104269
	private void Start()
	{
		if (BattleMap.battle != null)
		{
			this.SetData(BattleMap.battle);
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x00106080 File Offset: 0x00104280
	public void SetData(Logic.Battle battle)
	{
		if (battle == null)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_ui = BattleViewUI.Get();
		this.m_logic = battle;
		this.m_playerKingdomId = BattleMap.KingdomId;
		if (this.m_btn_Exit != null)
		{
			this.m_btn_Exit.onClick = new BSGButton.OnClick(this.IdleLeaveBattle);
			Tooltip.Get(this.m_btn_Exit.gameObject, true).SetDef("LeaveBattleButton", null);
		}
		if (this.m_btn_Retreat != null)
		{
			this.m_btn_Retreat.onClick = new BSGButton.OnClick(this.Retreat);
			Tooltip.Get(this.m_btn_Retreat.gameObject, true).SetDef("RetreatFromBattleButton", null);
		}
		if (this.m_logic != null && this.m_logic.is_siege)
		{
			this.m_btn_Exit.gameObject.SetActive(battle.initiative != null && (global::Battle.PlayerIsAttacker(this.m_logic, false) || UIBattleWindow.PlayerHasControl(this.m_logic)) && BattleMap.BattleSide == battle.initiative_side);
			this.m_btn_Retreat.gameObject.SetActive(global::Battle.PlayerIsAttacker(this.m_logic, false) || UIBattleWindow.PlayerHasControl(this.m_logic));
		}
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x001061BC File Offset: 0x001043BC
	private void Update()
	{
		if (this.m_btn_Exit != null && this.m_btn_Exit.gameObject.activeInHierarchy)
		{
			BSGButton btn_Exit = this.m_btn_Exit;
			Logic.Battle logic = this.m_logic;
			btn_Exit.Enable(((logic != null) ? logic.initiative : null) != null && this.m_logic.initiative.GetRate() != 0f, false);
		}
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x00106228 File Offset: 0x00104428
	private void Retreat(BSGButton button)
	{
		if (this.m_ui == null || this.m_logic == null)
		{
			return;
		}
		MessageWnd.Create(global::Defs.GetDefField("BattleViewRetreatMessage", null), new Vars(this.m_logic), null, new MessageWnd.OnButton(this.OnRetreatMessage));
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x0010627B File Offset: 0x0010447B
	public virtual bool OnRetreatMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.Retreat();
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x00106298 File Offset: 0x00104498
	private void Retreat()
	{
		int num = -1;
		if (this.m_playerKingdomId == this.m_logic.attacker_kingdom.id)
		{
			num = 0;
		}
		else if (this.m_playerKingdomId == this.m_logic.defender_kingdom.id)
		{
			num = 1;
		}
		if (num != -1)
		{
			this.m_logic.DoAction("retreat", num, "");
		}
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x001062F8 File Offset: 0x001044F8
	private void LeaveBattle(BSGButton button)
	{
		if (this.m_ui == null || this.m_logic == null)
		{
			return;
		}
		MessageWnd.Create(global::Defs.GetDefField("BattleViewLeaveMessage", null), new Vars(this.m_logic), null, new MessageWnd.OnButton(this.OnLeaveMessage));
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x0010634B File Offset: 0x0010454B
	public virtual bool OnLeaveMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.LeaveBattle();
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x00106368 File Offset: 0x00104568
	private void LeaveBattle()
	{
		int side = -1;
		if (this.m_playerKingdomId == this.m_logic.attacker_kingdom.id)
		{
			side = 0;
		}
		else if (this.m_playerKingdomId == this.m_logic.defender_kingdom.id)
		{
			side = 1;
		}
		this.m_logic.DoAction("leave_battle", side, "");
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x001063C4 File Offset: 0x001045C4
	private void IdleLeaveBattle(BSGButton button)
	{
		if (this.m_ui == null || this.m_logic == null)
		{
			return;
		}
		MessageWnd.Create(global::Defs.GetDefField("BattleViewLeaveMessage", null), new Vars(this.m_logic), null, new MessageWnd.OnButton(this.OnIdleLeaveMessage));
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x00106417 File Offset: 0x00104617
	public virtual bool OnIdleLeaveMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.IdleLeaveBattle();
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x00106434 File Offset: 0x00104634
	private void IdleLeaveBattle()
	{
		int side = -1;
		if (this.m_playerKingdomId == this.m_logic.attacker_kingdom.id)
		{
			side = 0;
		}
		else if (this.m_playerKingdomId == this.m_logic.defender_kingdom.id)
		{
			side = 1;
		}
		this.m_logic.DoAction("idle_leave_battle", side, "");
	}

	// Token: 0x040011D6 RID: 4566
	[UIFieldTarget("id_btn_Exit")]
	private BSGButton m_btn_Exit;

	// Token: 0x040011D7 RID: 4567
	[UIFieldTarget("id_btn_Retreat")]
	private BSGButton m_btn_Retreat;

	// Token: 0x040011D9 RID: 4569
	private int m_playerKingdomId;

	// Token: 0x040011DA RID: 4570
	private BattleViewUI m_ui;
}
