using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000195 RID: 405
public class UIArmyFood : MonoBehaviour, IVars
{
	// Token: 0x06001690 RID: 5776 RVA: 0x000E1CE0 File Offset: 0x000DFEE0
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x000E1CE8 File Offset: 0x000DFEE8
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x000E1D04 File Offset: 0x000DFF04
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
		this.CalcArmies();
		this.Refresh();
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x000E1D78 File Offset: 0x000DFF78
	private void CalcArmies()
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			Logic.Army logic = this.armies[i].army.logic;
			if (logic != null && logic.supplies != null)
			{
				if (this.main_army == null)
				{
					this.main_army = logic;
				}
				else
				{
					this.second_army = logic;
				}
			}
		}
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x000E1DD8 File Offset: 0x000DFFD8
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
		this.CalcArmies();
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x000E1E27 File Offset: 0x000E0027
	public void Clear()
	{
		this.armies.Clear();
		this.main_army = null;
		this.second_army = null;
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x000E1E42 File Offset: 0x000E0042
	public void Refresh()
	{
		if (this.vars == null)
		{
			this.vars = new Vars(this);
		}
		this.UpdateSupplies();
		Tooltip.Get(base.gameObject, true).SetDef("ArmySuppliesTooltip", this.vars);
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x000E1E7A File Offset: 0x000E007A
	protected void LateUpdate()
	{
		this.UpdateSupplies();
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x000E1E84 File Offset: 0x000E0084
	private void UpdateSupplies()
	{
		this.total_max = 0f;
		this.total_supplies = 0f;
		this.rate = 0f;
		for (int i = 0; i < this.armies.Count; i++)
		{
			Logic.Army logic = this.armies[i].army.logic;
			if (logic != null && logic.supplies != null)
			{
				this.total_supplies += (float)Math.Ceiling((double)logic.supplies.Get());
				this.total_max += logic.supplies.GetMax();
				this.rate += logic.supplies.GetRate();
			}
		}
		float fillAmount = Mathf.Clamp01(this.total_supplies / this.total_max);
		if (this.m_ArmyFoodLabel != null)
		{
			UIText.SetText(this.m_ArmyFoodLabel, ((int)this.total_supplies).ToString());
		}
		if (this.m_ArmyFoodProgress != null)
		{
			this.m_ArmyFoodProgress.fillAmount = fillAmount;
		}
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x000E1F94 File Offset: 0x000E0194
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "multiple")
		{
			return this.main_army != null && this.second_army != null;
		}
		if (key == "army")
		{
			return this.main_army;
		}
		if (key == "second_army")
		{
			return this.second_army;
		}
		if (key == "supplies")
		{
			return this.total_supplies;
		}
		if (key == "supplies_max")
		{
			return this.total_max;
		}
		if (!(key == "supplies_rate"))
		{
			return Value.Unknown;
		}
		if (this.rate != 0f)
		{
			return new Value(this.rate);
		}
		return Value.Unknown;
	}

	// Token: 0x04000E8E RID: 3726
	[UIFieldTarget("id_ArmyFoodProgressBar")]
	private Image m_ArmyFoodProgress;

	// Token: 0x04000E8F RID: 3727
	[UIFieldTarget("id_ArmyFoodLabel")]
	private TextMeshProUGUI m_ArmyFoodLabel;

	// Token: 0x04000E90 RID: 3728
	private List<UIArmyStatusBar.ArmyInfo> armies = new List<UIArmyStatusBar.ArmyInfo>();

	// Token: 0x04000E91 RID: 3729
	private Vars vars;

	// Token: 0x04000E92 RID: 3730
	private Logic.Army main_army;

	// Token: 0x04000E93 RID: 3731
	private Logic.Army second_army;

	// Token: 0x04000E94 RID: 3732
	private float total_max;

	// Token: 0x04000E95 RID: 3733
	private float total_supplies;

	// Token: 0x04000E96 RID: 3734
	private float rate;

	// Token: 0x04000E97 RID: 3735
	private bool m_Initialzied;
}
