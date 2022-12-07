using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001B5 RID: 437
public class UIBattleStatusBar : WorldToScreenObject, IListener, IPoolable
{
	// Token: 0x060019C0 RID: 6592 RVA: 0x000FA9C0 File Offset: 0x000F8BC0
	public void SetBattle(Logic.Battle battle)
	{
		if (battle != this.logic && this.logic != null)
		{
			this.Release();
		}
		this.logic = battle;
		if (this.logic == null)
		{
			return;
		}
		global::Battle battle2 = battle.visuals as global::Battle;
		if (battle2 == null)
		{
			return;
		}
		this._g = battle2.gameObject;
		this.SetAllowedType(ViewMode.AllowedFigures.Battle);
		battle2.UpdateVisibility();
		this.Init();
		UIKingdomIcon kingdomLeft = this.m_KingdomLeft;
		if (kingdomLeft != null)
		{
			kingdomLeft.SetObject(null, null);
		}
		UIKingdomIcon kingdomRight = this.m_KingdomRight;
		if (kingdomRight != null)
		{
			kingdomRight.SetObject(null, null);
		}
		if (this.m_EstimationBar != null)
		{
			this.m_EstimationBar.use_x_location = true;
			this.m_EstimationBar.SetObject(this.logic);
		}
		this.AddListeners(battle);
		this.Refresh();
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x0002C538 File Offset: 0x0002A738
	protected override bool AddToGlobalListOnAwake()
	{
		return false;
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000E4719 File Offset: 0x000E2919
	public void SetAllowedType(ViewMode.AllowedFigures allowedType = ViewMode.AllowedFigures.None)
	{
		this.allowedType = allowedType;
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x000FAA87 File Offset: 0x000F8C87
	public bool IsPreparing()
	{
		return this.logic.stage == Logic.Battle.Stage.Preparing;
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x000FAA97 File Offset: 0x000F8C97
	public bool IsOngoing()
	{
		return this.logic.stage == Logic.Battle.Stage.Ongoing && this.logic.type != Logic.Battle.Type.Plunder;
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x000FAABA File Offset: 0x000F8CBA
	public bool IsOngoingPlunder()
	{
		return this.logic.stage == Logic.Battle.Stage.Ongoing && this.logic.type == Logic.Battle.Type.Plunder;
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x000FAADC File Offset: 0x000F8CDC
	private void RefreshSpriteSheets()
	{
		if (this.m_PreparationBattleSwords != null)
		{
			this.m_PreparationBattleSwords.SetActive(this.IsPreparing());
		}
		if (this.m_OngoingBattleSwords != null)
		{
			this.m_OngoingBattleSwords.SetActive(this.IsOngoing());
		}
		if (this.m_PlunderTorch != null)
		{
			this.m_PlunderTorch.SetActive(this.IsOngoingPlunder());
		}
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x000FAB46 File Offset: 0x000F8D46
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.RefreshDefField();
		this.m_Initialized = true;
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x000FAB68 File Offset: 0x000F8D68
	public override void RefreshDefField()
	{
		if (base.InitDef())
		{
			DT.Field defField = global::Defs.GetDefField("BattleStatusBar", null);
			this.LoadDefs(defField);
			base.RefreshDefField();
		}
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x000FAB98 File Offset: 0x000F8D98
	private void RefreshArmies()
	{
		Logic.Object attacker = this.logic.attacker;
		Logic.Object @object = this.logic.defender;
		if (@object is Logic.Settlement)
		{
			@object = (@object as Logic.Settlement).GetController();
		}
		bool active = this.NameplatesVisible();
		this.m_ArmiesLeft.gameObject.SetActive(active);
		this.m_ArmiesLeft.gameObject.SetActive(active);
		if (this.side_left == 1)
		{
			this.m_KingdomRight.SetObject(attacker, null);
			this.m_KingdomLeft.SetObject(@object, null);
		}
		else
		{
			this.m_KingdomLeft.SetObject(attacker, null);
			this.m_KingdomRight.SetObject(@object, null);
		}
		this.was_nameplate_visible = this.NameplatesVisible();
		if (this.was_nameplate_visible)
		{
			if (this.side_left == 1)
			{
				this.attacker_armies = this.m_ArmiesRight;
				this.defender_armies = this.m_ArmiesLeft;
			}
			else
			{
				this.attacker_armies = this.m_ArmiesLeft;
				this.defender_armies = this.m_ArmiesRight;
			}
			if (this.logic != null)
			{
				bool is_plunder = this.logic.is_plunder;
				if (this.m_ArmiesLeft.m_Manpower != null)
				{
					this.m_ArmiesLeft.m_Manpower.hideIfZero = is_plunder;
				}
				if (this.m_ArmiesRight.m_Manpower != null)
				{
					this.m_ArmiesRight.m_Manpower.hideIfZero = is_plunder;
				}
			}
			for (int i = 0; i < this.logic.attackers.Count; i++)
			{
				this.attacker_armies.AddArmy(this.logic.attackers[i].visuals as global::Army);
			}
			this.attacker_armies.SetSettlement(null);
			this.attacker_armies.SetBattle(this.logic);
			this.attacker_armies.SetParent(this);
			this.attacker_armies.SetAllowedType(ViewMode.AllowedFigures.Army);
			if (this.logic.defenders.Count > 0)
			{
				for (int j = 0; j < this.logic.defenders.Count; j++)
				{
					this.defender_armies.AddArmy(this.logic.defenders[j].visuals as global::Army);
				}
			}
			this.defender_armies.SetSettlement(this.logic.settlement);
			this.defender_armies.SetBattle(this.logic);
			this.defender_armies.SetParent(this);
			this.defender_armies.SetAllowedType(ViewMode.AllowedFigures.Army);
		}
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x000FADFC File Offset: 0x000F8FFC
	public void Refresh()
	{
		if (this.m_ArmiesLeft != null)
		{
			this.m_ArmiesLeft.Release(false);
		}
		if (this.m_ArmiesRight != null)
		{
			this.m_ArmiesRight.Release(false);
		}
		this.Init();
		float num = float.MaxValue;
		this.side_left = 0;
		for (int i = 0; i <= 1; i++)
		{
			Logic.Army army = this.logic.GetArmy(i);
			if (army != null)
			{
				float x = army.position.x;
				if (x < num)
				{
					num = x;
					this.side_left = i;
				}
			}
		}
		this.RefreshArmies();
		this.RefreshSpriteSheets();
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x000FAE91 File Offset: 0x000F9091
	public void AddListeners(Logic.Battle logic)
	{
		if (logic != null)
		{
			logic.AddListener(this);
		}
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x000FAE9D File Offset: 0x000F909D
	public void RemoveListeners(Logic.Battle logic)
	{
		if (logic != null)
		{
			this.m_ArmiesLeft.Release(true);
			this.m_ArmiesRight.Release(true);
			logic.DelListener(this);
		}
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x000FAEC1 File Offset: 0x000F90C1
	public void Release()
	{
		this.RemoveListeners(this.logic);
		base.DelFromWTS();
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x000E5417 File Offset: 0x000E3617
	public void OnPoolSpawned()
	{
		base.AddToWTS();
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x000E5417 File Offset: 0x000E3617
	public void OnPoolActivated()
	{
		base.AddToWTS();
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x000FAED5 File Offset: 0x000F90D5
	public void OnPoolDeactivated()
	{
		this.Release();
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x000FAED5 File Offset: 0x000F90D5
	protected override void OnDestroy()
	{
		this.Release();
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x000FAEDD File Offset: 0x000F90DD
	public override bool IsVisible()
	{
		return this.visible;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000FAEE5 File Offset: 0x000F90E5
	private bool NameplatesVisible()
	{
		if (ViewMode.IsPoliticalView())
		{
			return UserSettings.NameplatesEnabledPV;
		}
		return UserSettings.NameplatesEnabledWV;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x000FAEFC File Offset: 0x000F90FC
	public override void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFigures)
	{
		bool flag = this.IsVisible();
		this.visibility_from_view = ((allowedFigures & this.allowedType) > ViewMode.AllowedFigures.None);
		this.visibility_from_filter = (!ViewMode.IsPoliticalView() || (ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		if (this.m_ArmiesLeft != null)
		{
			this.m_ArmiesLeft.UpdateVisibilityFromView(allowedFigures);
		}
		if (this.m_ArmiesRight != null)
		{
			this.m_ArmiesRight.UpdateVisibilityFromView(allowedFigures);
		}
		this.visible = (this.visibility_from_object && this.visibility_from_view && this.visibility_from_filter);
		base.UpdateVisibility();
		bool flag2 = this.NameplatesVisible();
		if ((this.IsVisible() && !flag) || (flag2 && !this.was_nameplate_visible))
		{
			this.Refresh();
		}
		this.UpdateEstimationBar();
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x000FAFC4 File Offset: 0x000F91C4
	public override void UpdateVisibilityFromObject(bool visible_from_object)
	{
		bool flag = this.IsVisible();
		this.visibility_from_object = visible_from_object;
		this.visibility_from_filter = ((ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		if (this.m_ArmiesLeft != null)
		{
			this.m_ArmiesLeft.UpdateVisibilityFromObject(visible_from_object);
		}
		if (this.m_ArmiesRight != null)
		{
			this.m_ArmiesRight.UpdateVisibilityFromObject(visible_from_object);
		}
		this.visible = (visible_from_object && this.visibility_from_view && this.visibility_from_filter);
		base.UpdateVisibility();
		bool flag2 = this.NameplatesVisible();
		if ((this.IsVisible() && !flag) || (this.was_nameplate_visible && !flag2))
		{
			this.Refresh();
		}
		this.UpdateEstimationBar();
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000FB074 File Offset: 0x000F9274
	public static void SetEstimationColor(Image obj, float estimation)
	{
		if (obj == null)
		{
			return;
		}
		Color estimationBarColor = global::Battle.GetEstimationBarColor(global::Battle.GetEstimationKey(estimation));
		obj.color = estimationBarColor;
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x000FB0A0 File Offset: 0x000F92A0
	private bool HasDefenders()
	{
		Logic.Battle battle = this.logic;
		bool flag;
		if (battle == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Settlement settlement = battle.settlement;
			flag = (((settlement != null) ? settlement.garrison : null) != null);
		}
		if (flag)
		{
			for (int i = 0; i < this.logic.settlement.garrison.units.Count; i++)
			{
				if (this.logic.settlement.garrison.units[i] != null && !this.logic.settlement.garrison.units[i].IsDefeated())
				{
					return true;
				}
			}
		}
		List<Logic.Army> armies = this.logic.GetArmies(1);
		for (int j = 0; j < armies.Count; j++)
		{
			Logic.Army army = armies[j];
			if (army != null && !army.IsDefeated())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x000FB168 File Offset: 0x000F9368
	public void UpdateEstimationBar()
	{
		if (this.logic == null)
		{
			return;
		}
		if (!(((this.logic.stage == Logic.Battle.Stage.Ongoing || this.logic.stage == Logic.Battle.Stage.Preparing) && this.HasDefenders()) & (this.visibility_from_object && this.visibility_from_view && this.visibility_from_filter)))
		{
			if (this.m_EstimationBar != null)
			{
				this.m_EstimationBar.gameObject.SetActive(false);
			}
			return;
		}
		if (this.m_EstimationBar != null)
		{
			this.m_EstimationBar.gameObject.SetActive(true);
		}
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x000FB1FD File Offset: 0x000F93FD
	private void LateUpdate()
	{
		this.UpdateEstimationBar();
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x000FB205 File Offset: 0x000F9405
	protected override string DefKey(bool refresh = false)
	{
		if (refresh)
		{
			this.def_key = "battle_status_bar";
		}
		return base.DefKey(refresh);
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x000FB21C File Offset: 0x000F941C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "stage_changed" || message == "type_changed" || message == "armies_changed")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x000DF448 File Offset: 0x000DD648
	public override float BaseSortOrder()
	{
		return -1f;
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x000FB24C File Offset: 0x000F944C
	public override void OnClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (ViewMode.IsPoliticalView() && !UserSettings.ClickableBattlePVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			baseUI.SelectObjFromLogic(this.logic, false, true);
			base.OnClick(e);
		}
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x000FB29C File Offset: 0x000F949C
	public override void OnDoubleClick(PointerEventData e)
	{
		if (e.button == PointerEventData.InputButton.Left)
		{
			BaseUI baseUI = BaseUI.Get();
			if (ViewMode.IsPoliticalView() && !UserSettings.ClickableBattlePVFigures && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
			{
				baseUI.Select();
				return;
			}
			baseUI.SelectObjFromLogic(this.logic, false, true);
			baseUI.LookAt(this.logic, false);
			base.OnDoubleClick(e);
		}
	}

	// Token: 0x0400108C RID: 4236
	private bool visible;

	// Token: 0x0400108D RID: 4237
	private bool was_nameplate_visible;

	// Token: 0x0400108E RID: 4238
	private bool m_Initialized;

	// Token: 0x0400108F RID: 4239
	public Logic.Battle logic;

	// Token: 0x04001090 RID: 4240
	public float offset_y = 9f;

	// Token: 0x04001091 RID: 4241
	private int side_left;

	// Token: 0x04001092 RID: 4242
	[UIFieldTarget("id_OngoingBattleSwords")]
	private GameObject m_OngoingBattleSwords;

	// Token: 0x04001093 RID: 4243
	[UIFieldTarget("id_PreparationBattleSwords")]
	private GameObject m_PreparationBattleSwords;

	// Token: 0x04001094 RID: 4244
	[UIFieldTarget("id_PlunderTorch")]
	private GameObject m_PlunderTorch;

	// Token: 0x04001095 RID: 4245
	[UIFieldTarget("id_ArmiesLeft")]
	private UIArmyStatusBar m_ArmiesLeft;

	// Token: 0x04001096 RID: 4246
	[UIFieldTarget("id_ArmiesRight")]
	private UIArmyStatusBar m_ArmiesRight;

	// Token: 0x04001097 RID: 4247
	[UIFieldTarget("id_KingdomLeft")]
	private UIKingdomIcon m_KingdomLeft;

	// Token: 0x04001098 RID: 4248
	[UIFieldTarget("id_KingdomRight")]
	private UIKingdomIcon m_KingdomRight;

	// Token: 0x04001099 RID: 4249
	[UIFieldTarget("id_estimation")]
	private UIBattleEstimationBar m_EstimationBar;

	// Token: 0x0400109A RID: 4250
	private UIArmyStatusBar attacker_armies;

	// Token: 0x0400109B RID: 4251
	private UIArmyStatusBar defender_armies;
}
