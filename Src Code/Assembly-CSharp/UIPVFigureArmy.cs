using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000262 RID: 610
public class UIPVFigureArmy : UIPVFigureIcon
{
	// Token: 0x06002579 RID: 9593 RVA: 0x0014C731 File Offset: 0x0014A931
	public override bool IsVisible()
	{
		return (this.parent != null || !this.inBattle) && base.IsVisible();
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x0014C751 File Offset: 0x0014A951
	public void SetArmy(global::Army army)
	{
		if (army == null)
		{
			return;
		}
		if (this.army == null)
		{
			this.Init();
		}
		this.army = army;
		this._g = ((army != null) ? army.gameObject : null);
		this.UpdateArmy();
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x0014C790 File Offset: 0x0014A990
	public override void Init()
	{
		this.last_icon_hostilityType = UIPVFigureArmy.HostilityType.Count;
		this.last_def_hostilityType = UIPVFigureArmy.HostilityType.Count;
		this.last_icon_armyType = UIPVFigureArmy.ArmyType.Count;
		this.last_def_armyType = UIPVFigureArmy.ArmyType.Count;
		base.Init();
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x0014C7B4 File Offset: 0x0014A9B4
	public void SetFaceDirection(UIPVFigureArmy.FaceDirection faceDirection)
	{
		this.faceDirection = faceDirection;
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x0014C7C0 File Offset: 0x0014A9C0
	public override bool IsFlipped()
	{
		if (this.faceDirection != UIPVFigureArmy.FaceDirection.Rotation)
		{
			return this.faceDirection != UIPVFigureArmy.FaceDirection.Left && this.faceDirection == UIPVFigureArmy.FaceDirection.Right;
		}
		global::Army army = this.army;
		bool? flag;
		if (army == null)
		{
			flag = null;
		}
		else
		{
			Logic.Army logic = army.logic;
			flag = ((logic != null) ? new bool?(logic.is_in_water) : null);
		}
		if ((flag ?? false) && this.army.ship != null)
		{
			return this.army.ship.transform.eulerAngles.y < 180f;
		}
		return this.army.transform.eulerAngles.y < 180f;
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x0014C88A File Offset: 0x0014AA8A
	private void UpdateAllowedType()
	{
		if (this.army.logic.mercenary != null)
		{
			base.SetAllowedType(ViewMode.AllowedFigures.Mercenary);
			return;
		}
		base.SetAllowedType(ViewMode.AllowedFigures.Army);
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x0014C8B0 File Offset: 0x0014AAB0
	private void UpdateArmyType()
	{
		Logic.Army logic = this.army.logic;
		if (logic == null)
		{
			return;
		}
		if (logic.is_in_water)
		{
			this.armyType = UIPVFigureArmy.ArmyType.Ship;
			return;
		}
		if (logic.leader != null && logic.leader.IsCrusader())
		{
			this.armyType = UIPVFigureArmy.ArmyType.Crusade;
			return;
		}
		if (logic.mercenary != null)
		{
			if (logic.IsHiredMercenary())
			{
				this.armyType = UIPVFigureArmy.ArmyType.HiredMercenary;
				return;
			}
			if (!logic.movement.IsMoving(true))
			{
				this.armyType = UIPVFigureArmy.ArmyType.Mercenary;
				return;
			}
			this.armyType = UIPVFigureArmy.ArmyType.Normal;
			return;
		}
		else
		{
			if (logic.rebel != null)
			{
				this.armyType = UIPVFigureArmy.ArmyType.Rebel;
				return;
			}
			this.armyType = UIPVFigureArmy.ArmyType.Normal;
			return;
		}
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x0014C948 File Offset: 0x0014AB48
	private void UpdateHostilityType()
	{
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		Logic.Army logic = this.army.logic;
		if (logic.IsEnemy(obj))
		{
			this.hostilityType = UIPVFigureArmy.HostilityType.Enemy;
			return;
		}
		if (logic.IsOwnStance(obj))
		{
			this.hostilityType = UIPVFigureArmy.HostilityType.Own;
			return;
		}
		if (logic.leader != null && logic.leader.IsRebel() && logic.IsAllyOrVassal(obj))
		{
			this.hostilityType = UIPVFigureArmy.HostilityType.Own;
			return;
		}
		if (logic.leader != null && logic.leader.IsCrusader() && logic.leader.IsOwnStance(obj))
		{
			this.hostilityType = UIPVFigureArmy.HostilityType.Own;
			return;
		}
		this.hostilityType = UIPVFigureArmy.HostilityType.Neutral;
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x0014C9E1 File Offset: 0x0014ABE1
	private void UpdateInBattle()
	{
		this.inBattle = (this.parent == null && this.army.logic.battle != null);
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x0014CA10 File Offset: 0x0014AC10
	public void UpdateArmy()
	{
		if (this.army == null)
		{
			return;
		}
		this.UpdateInBattle();
		this.UpdateAllowedType();
		this.UpdateHostilityType();
		this.UpdateArmyType();
		this.RefreshDefField();
		this.army.UpdateVisibility(false);
		if (this != this.army.ui_pvFigure)
		{
			this.UpdateVisibilityFromObject(this.army.ui_pvFigure.visibility_from_object);
		}
		if (this.armyType == UIPVFigureArmy.ArmyType.Mercenary && this.army.logic.mercenary.former_owner_id == BaseUI.LogicKingdom().id)
		{
			UIKingdomIcon crest = this.crest;
			if (crest != null)
			{
				crest.SetObject(this.army.logic.game.GetKingdom(this.army.logic.mercenary.former_owner_id), null);
			}
		}
		else
		{
			this.crest.SetObject(this.army.logic.GetKingdom(), null);
		}
		this.Refresh();
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x0014CB0C File Offset: 0x0014AD0C
	public override void GetIconSprite(out Sprite sprite, out Sprite hover)
	{
		string text = this.hostilityType.ToString().ToLowerInvariant() + "_texture";
		sprite = global::Defs.GetObj<Sprite>(this.field, text, null);
		hover = global::Defs.GetObj<Sprite>(this.field, text + ".hover", null);
		if (hover == null)
		{
			hover = sprite;
		}
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x0014CB70 File Offset: 0x0014AD70
	protected override bool Selected()
	{
		if (this.army != null)
		{
			return this.army.IsSelected();
		}
		return base.Selected();
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x0014CB92 File Offset: 0x0014AD92
	protected override string DefKey(bool refresh = false)
	{
		if (refresh || this.def_key == null)
		{
			this.def_key = "army_" + this.armyType.ToString();
		}
		return base.DefKey(refresh);
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x0014CBC8 File Offset: 0x0014ADC8
	public override void RefreshDefField()
	{
		if (this.armyType == this.last_def_armyType && this.hostilityType == this.last_def_hostilityType)
		{
			return;
		}
		this.last_def_armyType = this.armyType;
		this.last_def_hostilityType = this.hostilityType;
		DT.Field defField = global::Defs.GetDefField("PoliticalView", "pv_figures.Army");
		this.field = defField.FindChild(this.armyType.ToString(), null, true, true, true, '.');
		base.RefreshDefField();
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x0014CC44 File Offset: 0x0014AE44
	public override void RefreshIcon(bool force = false)
	{
		if (this.armyType == this.last_icon_armyType && this.hostilityType == this.last_icon_hostilityType && !force)
		{
			return;
		}
		this.last_icon_armyType = this.armyType;
		this.last_icon_hostilityType = this.hostilityType;
		base.RefreshIcon(force);
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x0014CC90 File Offset: 0x0014AE90
	private int GetCrestKingdomId()
	{
		if (this.armyType == UIPVFigureArmy.ArmyType.Mercenary && this.army.logic.mercenary.former_owner_id == BaseUI.LogicKingdom().id)
		{
			return this.army.logic.mercenary.former_owner_id;
		}
		return this.army.logic.kingdom_id;
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x0014CCED File Offset: 0x0014AEED
	public override GameObject GetVisuals()
	{
		global::Army army = this.army;
		if (army == null)
		{
			return null;
		}
		return army.gameObject;
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x000DF448 File Offset: 0x000DD648
	public override float BaseSortOrder()
	{
		return -1f;
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x0014CD00 File Offset: 0x0014AF00
	public override void OnClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableArmyPVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Army army = this.army;
			baseUI2.SelectObjFromLogic((army != null) ? army.logic : null, false, true);
			base.OnClick(e);
		}
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x0014CD54 File Offset: 0x0014AF54
	public override void OnDoubleClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (!UserSettings.ClickableArmyPVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			BaseUI baseUI2 = baseUI;
			global::Army army = this.army;
			baseUI2.SelectObjFromLogic((army != null) ? army.logic : null, false, true);
			BaseUI baseUI3 = baseUI;
			global::Army army2 = this.army;
			baseUI3.LookAt((army2 != null) ? army2.logic : null, false);
			base.OnDoubleClick(e);
		}
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x0014CDBF File Offset: 0x0014AFBF
	protected override bool Clickable()
	{
		return UserSettings.ClickableArmyPVFigures;
	}

	// Token: 0x0400198C RID: 6540
	public global::Army army;

	// Token: 0x0400198D RID: 6541
	public UIPVFigureArmy.ArmyType armyType;

	// Token: 0x0400198E RID: 6542
	public UIPVFigureArmy.HostilityType hostilityType;

	// Token: 0x0400198F RID: 6543
	public UIPVFigureArmy.FaceDirection faceDirection;

	// Token: 0x04001990 RID: 6544
	private bool inBattle;

	// Token: 0x04001991 RID: 6545
	public UIPVFigureArmy.HostilityType last_icon_hostilityType;

	// Token: 0x04001992 RID: 6546
	public UIPVFigureArmy.HostilityType last_def_hostilityType;

	// Token: 0x04001993 RID: 6547
	public UIPVFigureArmy.ArmyType last_icon_armyType;

	// Token: 0x04001994 RID: 6548
	public UIPVFigureArmy.ArmyType last_def_armyType;

	// Token: 0x020007BA RID: 1978
	public enum ArmyType
	{
		// Token: 0x04003C1D RID: 15389
		Normal,
		// Token: 0x04003C1E RID: 15390
		Crusade,
		// Token: 0x04003C1F RID: 15391
		Rebel,
		// Token: 0x04003C20 RID: 15392
		Ship,
		// Token: 0x04003C21 RID: 15393
		Mercenary,
		// Token: 0x04003C22 RID: 15394
		HiredMercenary,
		// Token: 0x04003C23 RID: 15395
		Count
	}

	// Token: 0x020007BB RID: 1979
	public enum HostilityType
	{
		// Token: 0x04003C25 RID: 15397
		Enemy,
		// Token: 0x04003C26 RID: 15398
		Own,
		// Token: 0x04003C27 RID: 15399
		Neutral,
		// Token: 0x04003C28 RID: 15400
		Count
	}

	// Token: 0x020007BC RID: 1980
	public enum FaceDirection
	{
		// Token: 0x04003C2A RID: 15402
		Rotation,
		// Token: 0x04003C2B RID: 15403
		Left,
		// Token: 0x04003C2C RID: 15404
		Right
	}
}
