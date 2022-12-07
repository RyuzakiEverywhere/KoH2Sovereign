using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000199 RID: 409
public class UIArmyManpower : MonoBehaviour
{
	// Token: 0x060016E4 RID: 5860 RVA: 0x000E33E9 File Offset: 0x000E15E9
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x000E3404 File Offset: 0x000E1604
	public void AddArmy(global::Army army)
	{
		this.Init();
		for (int i = 0; i < this.armies.Count; i++)
		{
			if (this.armies[i].army == army)
			{
				return;
			}
		}
		if (army == null)
		{
			return;
		}
		UIArmyStatusBar.ArmyInfo armyInfo = new UIArmyStatusBar.ArmyInfo();
		armyInfo.army = army;
		this.armies.Add(armyInfo);
		this.Refresh();
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x000E3470 File Offset: 0x000E1670
	public void SetSettlement(Logic.Settlement settlement)
	{
		this.Init();
		if (settlement == null)
		{
			return;
		}
		this.settlement_info = new UIArmyStatusBar.ArmyInfo
		{
			settlement = settlement
		};
		this.Refresh();
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x000E34A1 File Offset: 0x000E16A1
	public void SetHealthColor(Color c)
	{
		this.Init();
		this.m_Health.color = c;
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x000E34B5 File Offset: 0x000E16B5
	public void SetArmyStrengthColor(Color c)
	{
		this.Init();
		this.m_ArmyStrength.color = c;
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x000E34CC File Offset: 0x000E16CC
	public void DelArmy(global::Army army)
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			if (this.armies[i].army == army)
			{
				this.armies.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x000E3515 File Offset: 0x000E1715
	public void Clear()
	{
		Vars vars = this.vars;
		if (vars != null)
		{
			vars.Clear();
		}
		this.vars = null;
		this.settlement_info = null;
		this.armies.Clear();
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x000E3544 File Offset: 0x000E1744
	public void Refresh()
	{
		if (this.vars == null)
		{
			this.vars = new Vars();
		}
		for (int i = 0; i < 2; i++)
		{
			this.vars.Set<Logic.Army>(string.Format("army{0}", i), null);
		}
		if (this.armies.Count > 0)
		{
			for (int j = 0; j < this.armies.Count; j++)
			{
				Logic.Army logic = this.armies[j].army.logic;
				if (logic != null)
				{
					this.vars.Set<Logic.Army>(string.Format("army{0}", j), logic);
					if (j == 0)
					{
						this.vars.Set<Logic.Kingdom>("kingdom", logic.GetKingdom());
					}
					if (logic.battle == null && logic.castle != null)
					{
						for (int k = 0; k < logic.units.Count; k++)
						{
							if (logic.units[k].damage > 0f)
							{
								this.vars.Set<bool>("army_damaged", true);
								break;
							}
						}
					}
				}
			}
		}
		Vars vars = this.vars;
		string key = "garrison";
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		Garrison val;
		if (armyInfo == null)
		{
			val = null;
		}
		else
		{
			Logic.Settlement settlement = armyInfo.settlement;
			val = ((settlement != null) ? settlement.garrison : null);
		}
		vars.Set<Garrison>(key, val);
		Tooltip.Get(base.gameObject, true).SetDef("ManpowerArmyTooltip", this.vars);
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x000E36A6 File Offset: 0x000E18A6
	protected void LateUpdate()
	{
		this.UpdateManpower();
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x000E36B0 File Offset: 0x000E18B0
	private void AddManpower(List<Logic.Unit> units, ref int alive, ref int cnt)
	{
		for (int i = 0; i < units.Count; i++)
		{
			Logic.Unit unit = units[i];
			if (unit != null)
			{
				int num = unit.max_manpower_modified_locked_in_battle();
				cnt += num;
				if (!unit.IsDefeated())
				{
					alive += unit.num_alive();
				}
			}
		}
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x000E36FC File Offset: 0x000E18FC
	private void UpdateManpower()
	{
		using (new Stat.ForceCached("UIArmyManpower.UpdateManpower"))
		{
			int num = 0;
			int num2 = 0;
			using (Game.Profile("Calc Manpower", false, 0f, null))
			{
				for (int i = 0; i < this.armies.Count; i++)
				{
					Logic.Army logic = this.armies[i].army.logic;
					if (logic != null)
					{
						this.AddManpower(logic.units, ref num, ref num2);
					}
				}
				UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
				Garrison garrison;
				if (armyInfo == null)
				{
					garrison = null;
				}
				else
				{
					Logic.Settlement settlement = armyInfo.settlement;
					garrison = ((settlement != null) ? settlement.garrison : null);
				}
				Garrison garrison2 = garrison;
				if (garrison2 != null)
				{
					this.AddManpower(garrison2.units, ref num, ref num2);
				}
			}
			using (Game.Profile("Apply Manpower UI", false, 0f, null))
			{
				if (this.m_ArmyStrength != null && (num != this.last_manpower || num2 != this.last_max_manpower))
				{
					this.last_manpower = num;
					this.last_max_manpower = num2;
					if (this.last_max_manpower <= 0 || (this.last_manpower <= 0 && this.hideIfZero))
					{
						this.m_HealthBackground.gameObject.SetActive(false);
						this.m_ArmyStrength.gameObject.SetActive(false);
					}
					else
					{
						if (this.m_Health != null && this.m_HealthBackground != null)
						{
							if (num == num2)
							{
								this.m_HealthBackground.gameObject.SetActive(false);
							}
							else
							{
								this.m_HealthBackground.gameObject.SetActive(true);
								this.m_Health.fillAmount = 1f - (float)num / (float)num2;
							}
						}
						this.m_ArmyStrength.gameObject.SetActive(true);
						UIText.SetText(this.m_ArmyStrength, this.last_manpower.ToString());
						DT.Field thresholdField = UIArmyStatusBar.GetThresholdField(global::Defs.GetDefField("ArmyStatusBar", "Manpower"), (float)this.last_manpower);
						if (thresholdField != null)
						{
							Color color = global::Defs.GetColor(thresholdField, "col", null);
							this.m_ArmyStrength.color = color;
						}
					}
				}
			}
		}
	}

	// Token: 0x04000ECA RID: 3786
	private bool m_Initialzied;

	// Token: 0x04000ECB RID: 3787
	private List<UIArmyStatusBar.ArmyInfo> armies = new List<UIArmyStatusBar.ArmyInfo>();

	// Token: 0x04000ECC RID: 3788
	private UIArmyStatusBar.ArmyInfo settlement_info;

	// Token: 0x04000ECD RID: 3789
	private int last_manpower = -1;

	// Token: 0x04000ECE RID: 3790
	private int last_max_manpower = -1;

	// Token: 0x04000ECF RID: 3791
	private Vars vars;

	// Token: 0x04000ED0 RID: 3792
	[UIFieldTarget("id_ArmyStrength")]
	private TextMeshProUGUI m_ArmyStrength;

	// Token: 0x04000ED1 RID: 3793
	[UIFieldTarget("id_Health")]
	private Image m_Health;

	// Token: 0x04000ED2 RID: 3794
	[UIFieldTarget("id_HealthBackground")]
	private GameObject m_HealthBackground;

	// Token: 0x04000ED3 RID: 3795
	[NonSerialized]
	public bool hideIfZero = true;
}
