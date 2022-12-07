using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000216 RID: 534
internal class DiplomaticInfo : MonoBehaviour
{
	// Token: 0x06002067 RID: 8295 RVA: 0x00129169 File Offset: 0x00127369
	public void SetKingdoms(Logic.Kingdom player_kingdom, Logic.Kingdom selected_kingdom)
	{
		this.playerKingomd = player_kingdom;
		this.selectedKingdom = selected_kingdom;
		UICommon.FindComponents(this, false);
		this.Build();
		this.Refresh();
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x0012918C File Offset: 0x0012738C
	private void Build()
	{
		UICharacterIcon trader = this.m_Trader;
		if (trader != null)
		{
			trader.ShowCrest(false);
		}
		UICharacterIcon spy = this.m_Spy;
		if (spy != null)
		{
			spy.ShowCrest(false);
		}
		UICharacterIcon diplomat = this.m_Diplomat;
		if (diplomat == null)
		{
			return;
		}
		diplomat.ShowCrest(false);
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x001291C4 File Offset: 0x001273C4
	private void Refresh()
	{
		if (this.m_Trade != null)
		{
			UIText.SetText(this.m_Trade, this.GetGoldFromTrade().ToString());
		}
		if (this.m_Trader != null)
		{
			Logic.Character trader = this.GetTrader();
			if (this.m_Trader.Data != trader)
			{
				this.m_Trader.SetObject(trader, null);
			}
		}
		if (this.m_Influence != null)
		{
			UIText.SetText(this.m_Influence, this.GetInfluance().ToString());
		}
		if (this.m_Diplomat != null)
		{
			Logic.Character diplomat = this.GetDiplomat(this.playerKingomd, this.selectedKingdom);
			if (this.m_Diplomat.Data != diplomat)
			{
				this.m_Diplomat.SetObject(diplomat, null);
			}
		}
		if (this.m_Spy != null)
		{
			Logic.Character spy = this.GetSpy(this.playerKingomd, this.selectedKingdom);
			if (this.m_Spy.Data != spy)
			{
				this.m_Spy.SetObject(spy, null);
			}
		}
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x001292C7 File Offset: 0x001274C7
	private float GetGoldFromTrade()
	{
		return (float)Random.Range(0, 100);
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000448AF File Offset: 0x00042AAF
	private Logic.Character GetTrader()
	{
		return null;
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x001292D4 File Offset: 0x001274D4
	private Logic.Character GetSpy(Logic.Kingdom k1, Logic.Kingdom k2)
	{
		if (k1 == null || k2 == null)
		{
			return null;
		}
		if (k1 == k2)
		{
			return null;
		}
		for (int i = 0; i < k1.court.Count; i++)
		{
			Logic.Character character = k1.court[i];
			if (character != null && character.IsSpy() && character.mission_kingdom != null && character.mission_kingdom == k2)
			{
				return character;
			}
		}
		return null;
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x00129331 File Offset: 0x00127531
	private float GetInfluance()
	{
		if (this.playerKingomd == null || this.selectedKingdom == null)
		{
			return 0f;
		}
		return this.playerKingomd.GetInfluenceIn(this.selectedKingdom);
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x0012935C File Offset: 0x0012755C
	private Logic.Character GetDiplomat(Logic.Kingdom k1, Logic.Kingdom k2)
	{
		if (k1 == null || k2 == null)
		{
			return null;
		}
		if (k1 == k2)
		{
			return null;
		}
		for (int i = 0; i < k1.court.Count; i++)
		{
			Logic.Character character = k1.court[i];
			if (character != null && character.IsDiplomat() && character.mission_kingdom != null && character.mission_kingdom == k2)
			{
				return character;
			}
		}
		return null;
	}

	// Token: 0x04001596 RID: 5526
	[UIFieldTarget("id_Trader")]
	private UICharacterIcon m_Trader;

	// Token: 0x04001597 RID: 5527
	[UIFieldTarget("id_Spy")]
	private UICharacterIcon m_Spy;

	// Token: 0x04001598 RID: 5528
	[UIFieldTarget("id_Diplomat")]
	private UICharacterIcon m_Diplomat;

	// Token: 0x04001599 RID: 5529
	[UIFieldTarget("id_Trade")]
	private TextMeshProUGUI m_Trade;

	// Token: 0x0400159A RID: 5530
	[UIFieldTarget("id_Influence")]
	private TextMeshProUGUI m_Influence;

	// Token: 0x0400159B RID: 5531
	private Logic.Kingdom playerKingomd;

	// Token: 0x0400159C RID: 5532
	private Logic.Kingdom selectedKingdom;
}
