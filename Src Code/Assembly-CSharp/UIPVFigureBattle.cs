using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000263 RID: 611
public class UIPVFigureBattle : UIPVFigure
{
	// Token: 0x0600258F RID: 9615 RVA: 0x0014CDCE File Offset: 0x0014AFCE
	public override void Init()
	{
		base.Init();
		if (this.settlement == null)
		{
			this.settlement = global::Common.FindChildComponent<UIPVFigureSettlement>(base.gameObject, "id_settlement");
			UIPVFigureSettlement uipvfigureSettlement = this.settlement;
			if (uipvfigureSettlement == null)
			{
				return;
			}
			uipvfigureSettlement.SetParent(this);
		}
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x0014CE0C File Offset: 0x0014B00C
	public void SetBattle(global::Battle battle)
	{
		base.SetAllowedType(ViewMode.AllowedFigures.Battle);
		this.battle = battle;
		this._g = battle.gameObject;
		battle.UpdateVisibility();
		Vector3 position = battle.transform.position;
		base.gameObject.transform.position = new Vector3(position.x, (float)base.transform.GetSiblingIndex() * 0.001f, position.z);
		this.Init();
		this.Refresh();
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x0014CE84 File Offset: 0x0014B084
	public override void Refresh()
	{
		base.Refresh();
		global::Battle battle = this.battle;
		if (((battle != null) ? battle.logic : null) != null)
		{
			float num = float.MaxValue;
			int num2 = 0;
			for (int i = 0; i <= 1; i++)
			{
				Logic.Army army = this.battle.logic.GetArmy(i);
				if (army != null)
				{
					float x = army.position.x;
					if (x < num)
					{
						num = x;
						num2 = i;
					}
				}
			}
			this.UpdateSides(num2 != 0);
		}
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x0014CEF5 File Offset: 0x0014B0F5
	public override void RefreshDefField()
	{
		this.field = global::Defs.GetDefField("PoliticalView", "pv_figures.Battle");
		base.RefreshDefField();
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x0014CF12 File Offset: 0x0014B112
	protected override string DefKey(bool refresh = false)
	{
		if (refresh || this.def_key == null)
		{
			this.def_key = "battle";
		}
		return base.DefKey(refresh);
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x0014CF31 File Offset: 0x0014B131
	public void UpdateStatus()
	{
		this.battle == null;
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x0014CF40 File Offset: 0x0014B140
	public void UpdateSides(bool swap)
	{
		if (this.battle == null)
		{
			return;
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

	// Token: 0x06002596 RID: 9622 RVA: 0x0014CFB3 File Offset: 0x0014B1B3
	public override void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFiguresFromViewMode)
	{
		base.UpdateVisibilityFromView(allowedFiguresFromViewMode);
		if (this.settlement != null)
		{
			this.settlement.UpdateVisibilityFromView(allowedFiguresFromViewMode);
		}
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x0014CFD6 File Offset: 0x0014B1D6
	public override void UpdateVisibilityFilter()
	{
		base.UpdateVisibilityFilter();
		if (this.settlement != null)
		{
			this.settlement.UpdateVisibilityFilter();
		}
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x0014CFF8 File Offset: 0x0014B1F8
	public override void OnClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableBattlePVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Battle battle = this.battle;
			baseUI2.SelectObjFromLogic((battle != null) ? battle.logic : null, false, true);
			base.OnClick(e);
		}
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x0014D04C File Offset: 0x0014B24C
	public override void OnDoubleClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableBattlePVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Battle battle = this.battle;
			baseUI2.SelectObjFromLogic((battle != null) ? battle.logic : null, false, true);
			BaseUI baseUI3 = baseUI;
			global::Battle battle2 = this.battle;
			baseUI3.LookAt((battle2 != null) ? battle2.logic : null, false);
			base.OnDoubleClick(e);
		}
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x0014D0B7 File Offset: 0x0014B2B7
	protected override bool Clickable()
	{
		return UserSettings.ClickableBattlePVFigures;
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x0014D0BE File Offset: 0x0014B2BE
	public void UpdateIcon()
	{
		if (this.settlement != null)
		{
			this.settlement.UpdateIcon();
		}
	}

	// Token: 0x04001995 RID: 6549
	public global::Battle battle;

	// Token: 0x04001996 RID: 6550
	private UIPVFigureSettlement settlement;

	// Token: 0x04001997 RID: 6551
	public float swordsFieldYPos = -30f;

	// Token: 0x04001998 RID: 6552
	public float swordsSettlementYPos;
}
