using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B3 RID: 435
public class UIBattleResilience : MonoBehaviour
{
	// Token: 0x060019BA RID: 6586 RVA: 0x000FA902 File Offset: 0x000F8B02
	public void SetBattle(Logic.Battle battle, Vars vars)
	{
		this.logic = battle;
		UICommon.FindComponents(this, false);
		if (this.m_Resilience != null)
		{
			Tooltip.Get(this.m_Resilience.gameObject, true).SetText("Battle.Siege.Resilience", null, vars);
		}
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000FA93D File Offset: 0x000F8B3D
	private void Update()
	{
		this.m_Resilience.fillAmount = this.logic.resilience / this.logic.initial_resilience_pre_condition;
	}

	// Token: 0x04001088 RID: 4232
	public Logic.Battle logic;

	// Token: 0x04001089 RID: 4233
	[UIFieldTarget("id_Resilience")]
	private Image m_Resilience;
}
