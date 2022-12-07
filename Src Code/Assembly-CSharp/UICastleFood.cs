using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200019F RID: 415
public class UICastleFood : MonoBehaviour
{
	// Token: 0x0600178C RID: 6028 RVA: 0x000E7738 File Offset: 0x000E5938
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x000E7740 File Offset: 0x000E5940
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x000E7759 File Offset: 0x000E5959
	public void SetCastle(Logic.Settlement settlement)
	{
		this.settlement = settlement;
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x000E7762 File Offset: 0x000E5962
	public void Refresh()
	{
		this.UpdateFood();
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x000E7762 File Offset: 0x000E5962
	protected void LateUpdate()
	{
		this.UpdateFood();
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x000E776C File Offset: 0x000E596C
	private void UpdateFood()
	{
		Logic.Settlement settlement = this.settlement;
		bool flag;
		if (settlement == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Battle battle = settlement.battle;
			flag = (((battle != null) ? battle.settlement_food_copy : null) != null);
		}
		if (flag)
		{
			float num = this.settlement.battle.settlement_food_copy.Get();
			float max = this.settlement.battle.settlement_food_copy.GetMax();
			if (this.m_Food != null)
			{
				float fillAmount = num / max;
				this.m_Food.fillAmount = fillAmount;
			}
			if (this.m_CastleFoodText != null)
			{
				this.m_CastleFoodText.text = num.ToString("F0");
			}
			if (this.m_CastleFood != null)
			{
				if (this.vars == null)
				{
					this.vars = new Vars(this.settlement);
				}
				this.vars.Set<float>("cur_food", num);
				this.vars.Set<float>("max_food", max);
				Tooltip.Get(this.m_CastleFood, true).SetText("Battle.Siege.DefenderFood", null, this.vars);
			}
		}
	}

	// Token: 0x04000F22 RID: 3874
	[UIFieldTarget("id_Food")]
	private Image m_Food;

	// Token: 0x04000F23 RID: 3875
	[UIFieldTarget("id_CastleFood")]
	private GameObject m_CastleFood;

	// Token: 0x04000F24 RID: 3876
	[UIFieldTarget("id_CastleFoodLabel")]
	private TextMeshProUGUI m_CastleFoodText;

	// Token: 0x04000F25 RID: 3877
	private Logic.Settlement settlement;

	// Token: 0x04000F26 RID: 3878
	private Vars vars;

	// Token: 0x04000F27 RID: 3879
	private bool m_Initialzied;
}
