using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000265 RID: 613
public class UIPVFigureSettlement : UIPVFigureIcon
{
	// Token: 0x060025B1 RID: 9649 RVA: 0x0014D5A0 File Offset: 0x0014B7A0
	public override bool IsVisible()
	{
		if (!base.IsVisible())
		{
			return false;
		}
		if (this.parent != null)
		{
			return true;
		}
		global::Settlement settlement = this.settlement;
		Logic.Battle battle;
		if (settlement == null)
		{
			battle = null;
		}
		else
		{
			Logic.Settlement logic = settlement.logic;
			battle = ((logic != null) ? logic.battle : null);
		}
		Logic.Battle battle2 = battle;
		if (battle2 == null)
		{
			return true;
		}
		Logic.Realm realm = battle2.GetRealm();
		global::Realm realm2 = ((realm != null) ? realm.visuals : null) as global::Realm;
		if (realm2 == null)
		{
			return true;
		}
		if (realm2.visibility == -1)
		{
			realm2.UpdateFow(false, true);
		}
		return realm2.visibility <= 0 || (ViewMode.figuresFilter & ViewMode.AllowedFigures.Battle) == ViewMode.AllowedFigures.None;
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x0014D630 File Offset: 0x0014B830
	public void SetSettlement(global::Settlement settlement)
	{
		if (settlement == null)
		{
			base.SetAllowedType(ViewMode.AllowedFigures.None);
			return;
		}
		this.settlement = settlement;
		this.RefreshSettlementType();
		this.Init();
		this.UpdateVisibilityFromObject(true);
		this.UpdateVisibilityFilter();
		this._g = ((settlement != null) ? settlement.gameObject : null);
		this.Refresh();
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x0014D688 File Offset: 0x0014B888
	public void RefreshSettlementType()
	{
		if (this.settlement.IsCastle())
		{
			this.settlement.logic.GetKingdom();
			Logic.Realm realm = this.settlement.logic.GetRealm();
			if (realm.IsDisorder())
			{
				this.settlementType = UIPVFigureSettlement.SettlementType.Castle_Disorder;
			}
			else if (realm.IsReligionHQ() || realm.IsCatholicHolyLand() || realm.IsMuslimHolyLand())
			{
				this.settlementType = UIPVFigureSettlement.SettlementType.Castle_Religion_HQ;
			}
			else
			{
				this.settlementType = UIPVFigureSettlement.SettlementType.Castle;
			}
		}
		else
		{
			this.settlementType = UIPVFigureSettlement.SettlementType.Settlement;
		}
		base.SetAllowedType(ViewMode.AllowedFigures.Castle);
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x0014D70D File Offset: 0x0014B90D
	protected override string DefKey(bool refresh = false)
	{
		if (refresh || this.def_key == null)
		{
			this.def_key = "settlement_" + this.settlementType.ToString();
		}
		return base.DefKey(refresh);
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x0014D744 File Offset: 0x0014B944
	public override void RefreshDefField()
	{
		string text = this.settlementType.ToString();
		if (this.field != null && this.field.key == text)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("PoliticalView", "pv_figures.Settlement");
		this.field = defField.FindChild(text, null, true, true, true, '.');
		base.RefreshDefField();
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x0014D7A8 File Offset: 0x0014B9A8
	public override void GetIconSprite(out Sprite sprite, out Sprite hover)
	{
		string text = "texture";
		sprite = global::Defs.GetObj<Sprite>(this.field, text, null);
		hover = global::Defs.GetObj<Sprite>(this.field, text + ".hover", null);
		if (hover == null)
		{
			hover = sprite;
		}
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x0014D7F1 File Offset: 0x0014B9F1
	protected override bool Selected()
	{
		if (this.settlement != null)
		{
			return this.settlement.IsSelected();
		}
		return base.Selected();
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x0014D813 File Offset: 0x0014BA13
	public override void Refresh()
	{
		UIKingdomIcon crest = this.crest;
		if (crest != null)
		{
			crest.SetObject(this.settlement.logic.GetKingdom(), null);
		}
		base.Refresh();
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x0014D840 File Offset: 0x0014BA40
	public override void OnClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableSettlementPVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Settlement settlement = this.settlement;
			baseUI2.SelectObjFromLogic((settlement != null) ? settlement.logic : null, false, true);
			base.OnClick(e);
		}
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x0014D894 File Offset: 0x0014BA94
	public override void OnDoubleClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableSettlementPVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Settlement settlement = this.settlement;
			baseUI2.SelectObjFromLogic((settlement != null) ? settlement.logic : null, false, true);
			BaseUI baseUI3 = baseUI;
			global::Settlement settlement2 = this.settlement;
			baseUI3.LookAt((settlement2 != null) ? settlement2.logic : null, false);
			base.OnDoubleClick(e);
		}
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x0014D8FF File Offset: 0x0014BAFF
	protected override bool Clickable()
	{
		return UserSettings.ClickableSettlementPVFigures;
	}

	// Token: 0x040019A0 RID: 6560
	public global::Settlement settlement;

	// Token: 0x040019A1 RID: 6561
	public UIPVFigureSettlement.SettlementType settlementType;

	// Token: 0x020007BE RID: 1982
	public enum SettlementType
	{
		// Token: 0x04003C30 RID: 15408
		Castle,
		// Token: 0x04003C31 RID: 15409
		Settlement,
		// Token: 0x04003C32 RID: 15410
		Castle_Religion_HQ,
		// Token: 0x04003C33 RID: 15411
		Castle_Disorder,
		// Token: 0x04003C34 RID: 15412
		Count
	}
}
