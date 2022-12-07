using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class UIBattleInitiative : MonoBehaviour
{
	// Token: 0x06001A83 RID: 6787 RVA: 0x00100E67 File Offset: 0x000FF067
	public void SetData(Logic.Battle battle)
	{
		this.battle = battle;
		this.Init();
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x00100E76 File Offset: 0x000FF076
	private void Init()
	{
		if (this._init)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.vars = new Vars();
		this._init = true;
		Tooltip.Get(base.gameObject, true).SetDef("InitiativeTooltip", this.vars);
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00100EB8 File Offset: 0x000FF0B8
	private void UpdateDynamics()
	{
		Logic.Battle battle = this.battle;
		if (((battle != null) ? battle.initiative : null) == null)
		{
			return;
		}
		float num = this.battle.initiative.Get();
		for (int i = 0; i < this.m_Initiative.Length; i++)
		{
			this.m_Initiative[i].fillAmount = num / this.battle.initiative.GetMax();
		}
		float initiative_cooldown = this.battle.initiative_cooldown;
		bool flag = num <= 0f;
		if (initiative_cooldown > 0f)
		{
			for (int j = 0; j < this.m_IdleProgress.Length; j++)
			{
				Image image = this.m_IdleProgress[j];
				if (flag)
				{
					image.gameObject.SetActive(true);
					image.fillAmount = initiative_cooldown / this.battle.simulation.def.initiative_cooldown;
				}
				else
				{
					image.gameObject.SetActive(false);
				}
			}
			this.m_InitiativeValue.gameObject.SetActive(flag);
			this.m_InitiativePause.SetActive(!flag);
			if (flag)
			{
				UIText.SetText(this.m_InitiativeValue, global::Defs.LocalizedNumber(num, null, "F0"));
			}
		}
		else
		{
			for (int k = 0; k < this.m_IdleProgress.Length; k++)
			{
				this.m_IdleProgress[k].gameObject.SetActive(false);
			}
			this.m_InitiativeValue.gameObject.SetActive(true);
			this.m_InitiativePause.SetActive(false);
			if (!flag)
			{
				UIText.SetText(this.m_InitiativeValue, global::Defs.LocalizedNumber(num, null, "F0"));
			}
		}
		this.vars.Set<float>("seconds", num);
		this.vars.Set<bool>("initiative_is_paused", this.battle.initiative.GetRate() == 0f);
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x00101082 File Offset: 0x000FF282
	private void LateUpdate()
	{
		this.UpdateDynamics();
	}

	// Token: 0x0400112F RID: 4399
	[UIFieldTarget("id_Initiative", findMultiple = true)]
	private Image[] m_Initiative;

	// Token: 0x04001130 RID: 4400
	[UIFieldTarget("id_IdleProgress", findMultiple = true)]
	private Image[] m_IdleProgress;

	// Token: 0x04001131 RID: 4401
	[UIFieldTarget("id_InitiativeValue")]
	private TextMeshProUGUI m_InitiativeValue;

	// Token: 0x04001132 RID: 4402
	[UIFieldTarget("id_InitiativePause")]
	private GameObject m_InitiativePause;

	// Token: 0x04001133 RID: 4403
	private Logic.Battle battle;

	// Token: 0x04001134 RID: 4404
	private Vars vars;

	// Token: 0x04001135 RID: 4405
	private bool _init;
}
