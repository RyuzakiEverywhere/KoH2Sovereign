using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B4 RID: 436
public class UIBattleSiegeDefense : MonoBehaviour
{
	// Token: 0x060019BD RID: 6589 RVA: 0x000FA961 File Offset: 0x000F8B61
	public void SetBattle(Logic.Battle battle, Vars vars)
	{
		this.logic = battle;
		UICommon.FindComponents(this, false);
		if (this.m_SiegeDefense != null)
		{
			Tooltip.Get(this.m_SiegeDefense.gameObject, true).SetText("Battle.Siege.Defense", null, vars);
		}
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000FA99C File Offset: 0x000F8B9C
	private void Update()
	{
		this.m_SiegeDefense.fillAmount = this.logic.siege_defense / this.logic.initial_siege_defense_pre_condition;
	}

	// Token: 0x0400108A RID: 4234
	public Logic.Battle logic;

	// Token: 0x0400108B RID: 4235
	[UIFieldTarget("id_SiegeDefense")]
	private Image m_SiegeDefense;
}
