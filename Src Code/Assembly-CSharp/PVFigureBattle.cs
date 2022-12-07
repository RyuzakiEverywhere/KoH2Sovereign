using System;
using Logic;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class PVFigureBattle : PVFigure
{
	// Token: 0x060011F3 RID: 4595 RVA: 0x000BC8C8 File Offset: 0x000BAAC8
	public override void Init()
	{
		base.Init();
		if (this.leftArmyFirst == null)
		{
			this.leftArmyFirst = global::Common.FindChildComponent<PVFigureArmy>(base.gameObject, "id_left_army_first");
			PVFigureArmy pvfigureArmy = this.leftArmyFirst;
			if (pvfigureArmy != null)
			{
				pvfigureArmy.SetFaceDirection(PVFigureArmy.FaceDirection.Right);
			}
			PVFigureArmy pvfigureArmy2 = this.leftArmyFirst;
			if (pvfigureArmy2 != null)
			{
				pvfigureArmy2.SetParent(this);
			}
		}
		if (this.leftArmySecond == null)
		{
			this.leftArmySecond = global::Common.FindChildComponent<PVFigureArmy>(base.gameObject, "id_left_army_second");
			PVFigureArmy pvfigureArmy3 = this.leftArmySecond;
			if (pvfigureArmy3 != null)
			{
				pvfigureArmy3.SetFaceDirection(PVFigureArmy.FaceDirection.Right);
			}
			PVFigureArmy pvfigureArmy4 = this.leftArmySecond;
			if (pvfigureArmy4 != null)
			{
				pvfigureArmy4.SetParent(this);
			}
		}
		if (this.rightArmyFirst == null)
		{
			this.rightArmyFirst = global::Common.FindChildComponent<PVFigureArmy>(base.gameObject, "id_right_army_first");
			PVFigureArmy pvfigureArmy5 = this.rightArmyFirst;
			if (pvfigureArmy5 != null)
			{
				pvfigureArmy5.SetFaceDirection(PVFigureArmy.FaceDirection.Left);
			}
			PVFigureArmy pvfigureArmy6 = this.rightArmyFirst;
			if (pvfigureArmy6 != null)
			{
				pvfigureArmy6.SetParent(this);
			}
		}
		if (this.rightArmySecond == null)
		{
			this.rightArmySecond = global::Common.FindChildComponent<PVFigureArmy>(base.gameObject, "id_right_army_second");
			PVFigureArmy pvfigureArmy7 = this.rightArmySecond;
			if (pvfigureArmy7 != null)
			{
				pvfigureArmy7.SetFaceDirection(PVFigureArmy.FaceDirection.Left);
			}
			PVFigureArmy pvfigureArmy8 = this.rightArmySecond;
			if (pvfigureArmy8 != null)
			{
				pvfigureArmy8.SetParent(this);
			}
		}
		if (this.settlement == null)
		{
			this.settlement = global::Common.FindChildComponent<PVFigureSettlement>(base.gameObject, "id_settlement");
			PVFigureSettlement pvfigureSettlement = this.settlement;
			if (pvfigureSettlement == null)
			{
				return;
			}
			pvfigureSettlement.SetParent(this);
		}
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x000BCA30 File Offset: 0x000BAC30
	public void SetBattle(global::Battle battle)
	{
		base.SetAllowedType(ViewMode.AllowedFigures.Battle);
		this.battle = battle;
		battle.UpdateVisibility();
		Vector3 position = battle.transform.position;
		base.gameObject.transform.position = new Vector3(position.x, (float)base.transform.GetSiblingIndex() * 0.001f, position.z);
		this.Init();
		this.Refresh();
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x000BCA9C File Offset: 0x000BAC9C
	public override void RefreshDefField()
	{
		this.field = global::Defs.GetDefField("PoliticalView", "pv_figures.Battle");
		base.RefreshDefField();
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x000BCAB9 File Offset: 0x000BACB9
	public void UpdateStatus()
	{
		this.battle == null;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x000BCAC8 File Offset: 0x000BACC8
	public void UpdateSides(bool swap)
	{
		if (this.battle == null)
		{
			return;
		}
		if (this.leftArmyFirst != null)
		{
			bool active = true;
			if (swap)
			{
				if (this.battle.logic.defenders != null && this.battle.logic.defenders.Count > 0)
				{
					this.leftArmyFirst.SetArmy(this.battle.logic.defenders[0].visuals as global::Army);
				}
				else
				{
					active = false;
				}
			}
			else
			{
				PVFigureArmy pvfigureArmy = this.leftArmyFirst;
				global::Battle battle = this.battle;
				object obj;
				if (battle == null)
				{
					obj = null;
				}
				else
				{
					Logic.Battle logic = battle.logic;
					if (logic == null)
					{
						obj = null;
					}
					else
					{
						Logic.Army attacker = logic.attacker;
						obj = ((attacker != null) ? attacker.visuals : null);
					}
				}
				pvfigureArmy.SetArmy(obj as global::Army);
			}
			this.leftArmyFirst.gameObject.SetActive(active);
		}
		if (this.leftArmySecond != null)
		{
			bool active2 = true;
			if (swap)
			{
				if (this.battle.logic.defenders != null && this.battle.logic.defenders.Count > 1)
				{
					this.leftArmySecond.SetArmy(this.battle.logic.defenders[1].visuals as global::Army);
				}
				else
				{
					active2 = false;
				}
			}
			else if (this.battle.logic.attackers != null && this.battle.logic.attackers.Count > 1)
			{
				this.leftArmySecond.SetArmy(this.battle.logic.attackers[1].visuals as global::Army);
			}
			else
			{
				active2 = false;
			}
			this.leftArmySecond.gameObject.SetActive(active2);
		}
		if (this.rightArmyFirst != null)
		{
			bool active3 = true;
			if (swap)
			{
				PVFigureArmy pvfigureArmy2 = this.rightArmyFirst;
				global::Battle battle2 = this.battle;
				object obj2;
				if (battle2 == null)
				{
					obj2 = null;
				}
				else
				{
					Logic.Battle logic2 = battle2.logic;
					if (logic2 == null)
					{
						obj2 = null;
					}
					else
					{
						Logic.Army attacker2 = logic2.attacker;
						obj2 = ((attacker2 != null) ? attacker2.visuals : null);
					}
				}
				pvfigureArmy2.SetArmy(obj2 as global::Army);
			}
			else if (this.battle.logic.defenders != null && this.battle.logic.defenders.Count > 0)
			{
				this.rightArmyFirst.SetArmy(this.battle.logic.defenders[0].visuals as global::Army);
			}
			else
			{
				active3 = false;
			}
			this.rightArmyFirst.gameObject.SetActive(active3);
		}
		if (this.rightArmySecond != null)
		{
			bool active4 = true;
			if (swap)
			{
				if (this.battle.logic.attackers != null && this.battle.logic.attackers.Count > 1)
				{
					this.rightArmySecond.SetArmy(this.battle.logic.attackers[1].visuals as global::Army);
				}
				else
				{
					active4 = false;
				}
			}
			else if (this.battle.logic.defenders != null && this.battle.logic.defenders.Count > 1)
			{
				this.rightArmySecond.SetArmy(this.battle.logic.defenders[1].visuals as global::Army);
			}
			else
			{
				active4 = false;
			}
			this.rightArmySecond.gameObject.SetActive(active4);
		}
		if (this.settlement != null)
		{
			if (this.battle.logic.settlement != null)
			{
				this.settlement.SetSettlement(this.battle.logic.settlement.visuals as global::Settlement);
				return;
			}
			this.settlement.gameObject.SetActive(false);
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x000BCE69 File Offset: 0x000BB069
	public override void Refresh()
	{
		base.Refresh();
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x04000C15 RID: 3093
	public global::Battle battle;

	// Token: 0x04000C16 RID: 3094
	private PVFigureArmy leftArmyFirst;

	// Token: 0x04000C17 RID: 3095
	private PVFigureArmy leftArmySecond;

	// Token: 0x04000C18 RID: 3096
	private PVFigureArmy rightArmyFirst;

	// Token: 0x04000C19 RID: 3097
	private PVFigureArmy rightArmySecond;

	// Token: 0x04000C1A RID: 3098
	private PVFigureSettlement settlement;

	// Token: 0x04000C1B RID: 3099
	public float swordsFieldYPos = -30f;

	// Token: 0x04000C1C RID: 3100
	public float swordsSettlementYPos;
}
