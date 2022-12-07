using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200019B RID: 411
public class UIArmyStatusBar : WorldToScreenObject, IListener, IPoolable
{
	// Token: 0x06001700 RID: 5888 RVA: 0x000E45A0 File Offset: 0x000E27A0
	public void AddArmy(global::Army army)
	{
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
		armyInfo.kingdom = army.logic.GetKingdom();
		this.armies.Add(armyInfo);
		if (army.logic.mercenary != null)
		{
			this.SetAllowedType(ViewMode.AllowedFigures.Mercenary);
		}
		else
		{
			this.SetAllowedType(ViewMode.AllowedFigures.Army);
		}
		this.Init();
		if (this.m_Morale != null)
		{
			this.m_Morale.AddArmy(army);
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.AddArmy(army);
		}
		this.RefreshListeners();
		army.UpdateVisibility(false);
		this.m_Invalidate = true;
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x000E467C File Offset: 0x000E287C
	public override RelationUtils.Stance Stance()
	{
		Logic.Kingdom k = BaseUI.LogicKingdom();
		for (int i = 0; i < this.armies.Count; i++)
		{
			UIArmyStatusBar.ArmyInfo armyInfo = this.armies[i];
			Logic.Army army;
			if (armyInfo == null)
			{
				army = null;
			}
			else
			{
				global::Army army2 = armyInfo.army;
				army = ((army2 != null) ? army2.logic : null);
			}
			Logic.Army army3 = army;
			if (army3 != null)
			{
				return army3.GetStance(k);
			}
		}
		return RelationUtils.Stance.None;
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x000E46D6 File Offset: 0x000E28D6
	protected override string DefKey(bool refresh = false)
	{
		if (refresh)
		{
			this.def_key = "army_status_bar";
		}
		return base.DefKey(refresh);
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x000E46ED File Offset: 0x000E28ED
	public void SetBattle(Logic.Battle battle)
	{
		this.Init();
		if (this.m_Morale != null)
		{
			this.m_Morale.SetBattle(battle);
		}
		if (battle == null)
		{
			base.SetParent(null);
		}
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x0002C538 File Offset: 0x0002A738
	protected override bool AddToGlobalListOnAwake()
	{
		return false;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x000E4719 File Offset: 0x000E2919
	public void SetAllowedType(ViewMode.AllowedFigures allowedType = ViewMode.AllowedFigures.None)
	{
		this.allowedType = allowedType;
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x000E4724 File Offset: 0x000E2924
	public void SetSettlement(Logic.Settlement settlement)
	{
		if (this.settlement_info != null && this.settlement_info.settlement == settlement)
		{
			this.RemoveListeners(this.settlement_info.settlement);
			this.settlement_info = null;
		}
		if (settlement != null)
		{
			this.settlement_info = new UIArmyStatusBar.ArmyInfo
			{
				settlement = settlement,
				kingdom = settlement.GetKingdom()
			};
			this.Init();
			if (this.m_Manpower != null)
			{
				this.m_Manpower.SetSettlement(settlement);
			}
			if (this.m_Morale != null)
			{
				this.m_Morale.SetSettlement(settlement);
			}
			this.m_Invalidate = true;
		}
		this.RefreshListeners();
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x000E47CC File Offset: 0x000E29CC
	private void UpdateArmies()
	{
		if (this.armies.Count > 0)
		{
			UIArmyStatusBar.ArmyInfo armyInfo = this.armies[0];
			if (((armyInfo != null) ? armyInfo.army : null) == null)
			{
				this._g = null;
			}
			else
			{
				this._g = this.armies[0].army.gameObject;
			}
		}
		bool active = this.HasSupporter();
		if (this.m_LeaderBackgroundSupporter != null)
		{
			this.m_LeaderBackgroundSupporter.gameObject.SetActive(active);
		}
		if (this.m_FrameSupporter != null)
		{
			this.m_FrameSupporter.gameObject.SetActive(active);
		}
		if (this.m_LeaderLevelSupporter != null)
		{
			this.m_LeaderLevelSupporter.gameObject.SetActive(active);
		}
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x000E4890 File Offset: 0x000E2A90
	public void DelArmy(global::Army army)
	{
		if (this.m_Morale != null)
		{
			this.m_Morale.DelArmy(army);
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.DelArmy(army);
		}
		for (int i = 0; i < this.armies.Count; i++)
		{
			if (this.armies[i].army == army)
			{
				this.RemoveListeners(army.logic);
				this.armies.RemoveAt(i);
				break;
			}
		}
		this.RefreshListeners();
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000E4920 File Offset: 0x000E2B20
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.RefreshDefField();
		this.SetAllowedType(ViewMode.AllowedFigures.Army);
		this.m_Initialzied = true;
		if (this.button != null)
		{
			this.button.onClick = new BSGButton.OnClick(this.OnClick);
		}
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x000E4978 File Offset: 0x000E2B78
	public override void RefreshDefField()
	{
		if (base.InitDef())
		{
			DT.Field defField = global::Defs.GetDefField("ArmyStatusBar", null);
			this.LoadDefs(defField);
			base.RefreshDefField();
		}
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x000E49A6 File Offset: 0x000E2BA6
	public void Refresh()
	{
		this.UpdateStance();
		this.UpdateLevels();
		this.UpdateFrames();
		this.UpdateArmies();
		this.UpdateProgress();
		this.UpdateTier();
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x000E49CC File Offset: 0x000E2BCC
	public bool HasDefenders()
	{
		return this.HasArmies() || this.HasGarrison();
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x000E49E0 File Offset: 0x000E2BE0
	public bool HasArmies()
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			UIArmyStatusBar.ArmyInfo armyInfo = this.armies[i];
			bool flag;
			if (armyInfo == null)
			{
				flag = (null != null);
			}
			else
			{
				global::Army army = armyInfo.army;
				flag = (((army != null) ? army.logic : null) != null);
			}
			if (flag && !this.armies[i].army.logic.IsDefeated())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x000E4A4C File Offset: 0x000E2C4C
	public bool HasSupporter()
	{
		if (this.armies.Count == 2)
		{
			UIArmyStatusBar.ArmyInfo armyInfo = this.armies[1];
			bool flag;
			if (armyInfo == null)
			{
				flag = (null != null);
			}
			else
			{
				global::Army army = armyInfo.army;
				flag = (((army != null) ? army.logic : null) != null);
			}
			if (flag)
			{
				return !this.armies[1].army.logic.IsDefeated();
			}
		}
		return false;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x000E4AB0 File Offset: 0x000E2CB0
	public bool HasGarrison()
	{
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		Logic.Battle battle;
		if (armyInfo == null)
		{
			battle = null;
		}
		else
		{
			Logic.Settlement settlement = armyInfo.settlement;
			battle = ((settlement != null) ? settlement.battle : null);
		}
		Logic.Battle battle2 = battle;
		object obj;
		if (battle2 == null)
		{
			obj = null;
		}
		else
		{
			Logic.Settlement settlement2 = battle2.settlement;
			obj = ((settlement2 != null) ? settlement2.garrison : null);
		}
		if (obj == null)
		{
			return false;
		}
		for (int i = 0; i < battle2.settlement.garrison.units.Count; i++)
		{
			if (battle2.settlement.garrison.units[i] != null && !battle2.settlement.garrison.units[i].IsDefeated())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x000E4B54 File Offset: 0x000E2D54
	private void UpdateProgress()
	{
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		Logic.Battle battle;
		if (armyInfo == null)
		{
			battle = null;
		}
		else
		{
			Logic.Settlement settlement = armyInfo.settlement;
			battle = ((settlement != null) ? settlement.battle : null);
		}
		Logic.Battle battle2 = battle;
		if (battle2 == null)
		{
			if (this.m_PlunderProgress != null)
			{
				this.m_PlunderProgress.gameObject.SetActive(false);
			}
			if (this.m_Morale != null)
			{
				this.m_Morale.gameObject.SetActive(true);
			}
			return;
		}
		if (battle2.type != Logic.Battle.Type.Plunder)
		{
			if (this.m_PlunderProgress != null)
			{
				this.m_PlunderProgress.gameObject.SetActive(false);
			}
			if (this.m_Morale != null)
			{
				this.m_Morale.gameObject.SetActive(true);
			}
			return;
		}
		if (this.m_PlunderProgress != null)
		{
			this.m_PlunderProgress.gameObject.SetActive(true);
		}
		if (this.m_Morale != null)
		{
			this.m_Morale.gameObject.SetActive(false);
		}
		if (this.m_PlunderProgress == null)
		{
			return;
		}
		if (battle2.def.duration <= 0f)
		{
			this.m_PlunderProgress.gameObject.SetActive(false);
			return;
		}
		this.m_PlunderProgress.fillAmount = Mathf.Clamp01(battle2.PlunderProgress());
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x000E4C94 File Offset: 0x000E2E94
	private void UpdateStance()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		string text = null;
		Logic.Object @object;
		if (this.armies.Count > 0)
		{
			Logic.Army logic = this.armies[0].army.logic;
			if (((logic != null) ? logic.battle : null) != null)
			{
				Logic.Kingdom sideKingdom = logic.battle.GetSideKingdom(logic.battle_side);
				if (sideKingdom == null)
				{
					this.m_Invalidate = true;
					return;
				}
				if (sideKingdom.type == Logic.Kingdom.Type.LoyalistsFaction)
				{
					Logic.Rebel rebel = logic.rebel;
					@object = ((rebel != null) ? rebel.rebellion : null);
				}
				else
				{
					@object = sideKingdom;
				}
			}
			else
			{
				@object = logic;
			}
		}
		else
		{
			if (this.settlement_info == null)
			{
				return;
			}
			Logic.Settlement settlement = this.settlement_info.settlement;
			if (((settlement != null) ? settlement.battle : null) != null)
			{
				@object = settlement.battle.GetSideKingdom(1);
			}
			else
			{
				@object = settlement;
			}
		}
		if (@object == null)
		{
			this.m_Invalidate = true;
			return;
		}
		if (kingdom.IsEnemy(@object))
		{
			Logic.Kingdom kingdom2 = @object.GetKingdom();
			War war = kingdom.FindWarWith(@object.GetKingdom());
			if (war != null && !war.IsLeader(kingdom2))
			{
				text = "EnemySupporter";
			}
			else
			{
				text = "EnemyLeader";
			}
		}
		else if (kingdom.IsOwnStance(@object))
		{
			text = "Mine";
		}
		else if (kingdom.IsNeutral(@object))
		{
			text = "Neutral";
		}
		else if (kingdom.IsAlly(@object))
		{
			text = "Ally";
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("ArmyStatusBar", text);
		if (this.m_Background != null)
		{
			this.m_Background.sprite = defField.GetRandomValue("Background", null, true, true, true, '.').Get<Sprite>();
		}
		if (this.m_LeaderBackground != null)
		{
			this.m_LeaderBackground.sprite = defField.GetRandomValue("LeaderBackground", null, true, true, true, '.').Get<Sprite>();
			if (this.m_LeaderBackgroundSupporter != null)
			{
				this.m_LeaderBackgroundSupporter.sprite = this.m_LeaderBackground.sprite;
			}
		}
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x000E4E80 File Offset: 0x000E3080
	public static DT.Field GetThresholdField(DT.Field levels_field, float val)
	{
		if (((levels_field != null) ? levels_field.children : null) == null || levels_field.children.Count == 0)
		{
			return null;
		}
		DT.Field field = null;
		int num = int.MinValue;
		for (int i = 0; i < levels_field.children.Count; i++)
		{
			DT.Field field2 = levels_field.children[i];
			if (field2.children != null && field2.children.Count != 0)
			{
				int @int = field2.GetInt("threshold", null, 0, true, true, true, '.');
				if ((float)@int <= val && @int > num)
				{
					num = @int;
					field = field2;
				}
			}
		}
		if (field == null)
		{
			field = levels_field.children[0];
		}
		return field;
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x000E4F20 File Offset: 0x000E3120
	private void UpdateLevels()
	{
		bool active = this.armies.Count == 0 && this.HasGarrison();
		if (this.m_LeaderBackground != null)
		{
			this.m_LeaderBackground.gameObject.SetActive(this.HasDefenders());
		}
		if (this.m_GarrisonIcon != null)
		{
			this.m_GarrisonIcon.SetActive(active);
		}
		if (this.m_LeaderLevel != null)
		{
			this.m_LeaderLevel.gameObject.SetActive(this.armies.Count > 0);
		}
		if (this.armies.Count > 0 && this.CheckLevelChange(this.armies[0]))
		{
			this.UpdateLevel(this.armies[0], this.m_LeaderLevel);
		}
		if (this.armies.Count > 1 && this.CheckLevelChange(this.armies[1]))
		{
			this.UpdateLevel(this.armies[1], this.m_LeaderLevelSupporter);
		}
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x000E5024 File Offset: 0x000E3224
	private void UpdateLevel(UIArmyStatusBar.ArmyInfo info, TextMeshProUGUI text_field)
	{
		int last_level = info.last_level;
		if (text_field == null)
		{
			return;
		}
		DT.Field thresholdField = UIArmyStatusBar.GetThresholdField(global::Defs.GetDefField("ArmyStatusBar", "Levels"), (float)last_level);
		if (thresholdField == null)
		{
			return;
		}
		Color color = global::Defs.GetColor(thresholdField, "col", null);
		text_field.color = color;
		if (last_level <= 0)
		{
			UIText.SetText(text_field, "-");
			return;
		}
		UIText.SetText(text_field, last_level.ToString());
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000E5090 File Offset: 0x000E3290
	private void UpdateFrames()
	{
		if (this.armies.Count < 1)
		{
			if (this.settlement_info != null)
			{
				this.UpdateFrame(this.settlement_info, this.m_Frame);
			}
			return;
		}
		this.UpdateFrame(this.armies[0], this.m_Frame);
		if (this.armies.Count < 2)
		{
			return;
		}
		this.UpdateFrame(this.armies[1], this.m_FrameSupporter);
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x000E5108 File Offset: 0x000E3308
	private void UpdateFrame(UIArmyStatusBar.ArmyInfo info, Image image)
	{
		if (image == null)
		{
			return;
		}
		global::Army army = info.army;
		List<Logic.Unit> list;
		if (army == null)
		{
			list = null;
		}
		else
		{
			Logic.Army logic = army.logic;
			list = ((logic != null) ? logic.units : null);
		}
		List<Logic.Unit> list2 = list;
		if (list2 == null)
		{
			Logic.Settlement settlement = info.settlement;
			list2 = ((settlement != null) ? settlement.garrison.units : null);
		}
		if (list2 == null)
		{
			return;
		}
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < list2.Count; i++)
		{
			Logic.Unit unit = list2[i];
			if (((unit != null) ? unit.def : null) != null && unit.def.type != Logic.Unit.Type.Noble)
			{
				num2 += (float)unit.level;
				num++;
			}
		}
		if (num > 0)
		{
			num2 /= (float)num;
		}
		if (info.last_unit_level == num2)
		{
			return;
		}
		info.last_unit_level = num2;
		DT.Field thresholdField = UIArmyStatusBar.GetThresholdField(global::Defs.GetDefField("ArmyStatusBar", "Frames"), num2);
		if (thresholdField == null)
		{
			return;
		}
		Sprite sprite = thresholdField.GetRandomValue("Frame", null, true, true, true, '.').Get<Sprite>();
		if (sprite != null)
		{
			image.sprite = sprite;
		}
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x000E5214 File Offset: 0x000E3414
	public void AddListeners(Logic.Army logic)
	{
		if (logic == null)
		{
			return;
		}
		if (logic.leader != null)
		{
			logic.leader.AddListener(this);
		}
		Logic.Kingdom kingdom = logic.GetKingdom();
		if (kingdom != null)
		{
			kingdom.AddListener(this);
		}
		logic.AddListener(this);
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x000E5254 File Offset: 0x000E3454
	public void AddListeners(Logic.Settlement logic)
	{
		Logic.Kingdom kingdom = logic.GetKingdom();
		if (kingdom != null)
		{
			kingdom.AddListener(this);
		}
		logic.AddListener(this);
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x000E527C File Offset: 0x000E347C
	public void RemoveListeners(Logic.Army logic)
	{
		if (logic != null)
		{
			if (logic.leader != null)
			{
				logic.leader.DelListener(this);
			}
			Logic.Kingdom kingdom = logic.GetKingdom();
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
			logic.DelListener(this);
		}
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x000E52B8 File Offset: 0x000E34B8
	public void RemoveListeners(Logic.Settlement logic)
	{
		if (logic == null)
		{
			return;
		}
		Logic.Kingdom kingdom = logic.GetKingdom();
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		logic.DelListener(this);
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x000E52E4 File Offset: 0x000E34E4
	public void RefreshListeners()
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			global::Army army = this.armies[i].army;
			Logic.Army logic = (army != null) ? army.logic : null;
			this.RemoveListeners(logic);
			this.AddListeners(logic);
		}
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		if (((armyInfo != null) ? armyInfo.settlement : null) != null)
		{
			this.RemoveListeners(this.settlement_info.settlement);
			this.AddListeners(this.settlement_info.settlement);
		}
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x000E536C File Offset: 0x000E356C
	public void Release(bool set_visible = true)
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			this.RemoveListeners(this.armies[i].army.logic);
		}
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		this.RemoveListeners((armyInfo != null) ? armyInfo.settlement : null);
		if (this.m_Morale != null)
		{
			this.m_Morale.Clear();
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
		}
		this.armies.Clear();
		this.settlement_info = null;
		if (set_visible)
		{
			this.visible = false;
		}
		base.DelFromWTS();
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x000E5417 File Offset: 0x000E3617
	public void OnPoolSpawned()
	{
		base.AddToWTS();
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x000E5417 File Offset: 0x000E3617
	public void OnPoolActivated()
	{
		base.AddToWTS();
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000E541F File Offset: 0x000E361F
	public void OnPoolDeactivated()
	{
		this.Release(true);
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x000E541F File Offset: 0x000E361F
	protected override void OnDestroy()
	{
		this.Release(true);
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x000E5428 File Offset: 0x000E3628
	public override int SortOrderTieBreaker()
	{
		if (this.armies.Count > 0)
		{
			global::Army army = this.armies[0].army;
			if (((army != null) ? army.logic : null) != null)
			{
				return this.armies[0].army.logic.GetNid(true);
			}
		}
		return base.SortOrderTieBreaker();
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x000E5485 File Offset: 0x000E3685
	public override bool IsVisible()
	{
		if (!this.visible)
		{
			return false;
		}
		if (ViewMode.IsPoliticalView())
		{
			return UserSettings.NameplatesEnabledPV;
		}
		return UserSettings.NameplatesEnabledWV;
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x000E54A4 File Offset: 0x000E36A4
	public override void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFigures)
	{
		bool flag = this.IsVisible();
		this.visibility_from_filter = (!ViewMode.IsPoliticalView() || (ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		this.visibility_from_view = ((allowedFigures & this.allowedType) > ViewMode.AllowedFigures.None);
		this.visible = (this.visibility_from_object && this.visibility_from_view && this.visibility_from_filter);
		base.UpdateVisibility();
		if (this.IsVisible() && !flag)
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x000E5520 File Offset: 0x000E3720
	public override void UpdateVisibilityFromObject(bool visible_from_object)
	{
		bool flag = this.IsVisible();
		this.visibility_from_object = visible_from_object;
		this.visibility_from_filter = (!ViewMode.IsPoliticalView() || (ViewMode.figuresFilter & this.allowedType) > ViewMode.AllowedFigures.None);
		this.visible = (visible_from_object && this.visibility_from_view && this.visibility_from_filter);
		base.UpdateVisibility();
		if (this.IsVisible() && !flag)
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x000E558C File Offset: 0x000E378C
	protected void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
		this.UpdateDynamics();
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x000E55A9 File Offset: 0x000E37A9
	public void UpdateDynamics()
	{
		this.UpdateProgress();
		this.UpdateLevels();
		this.UpdateFrames();
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x000E55C0 File Offset: 0x000E37C0
	private void UpdateTier()
	{
		if (this.m_Manpower == null)
		{
			return;
		}
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < this.armies.Count; i++)
		{
			global::Army army = this.armies[i].army;
			List<Logic.Unit> list;
			if (army == null)
			{
				list = null;
			}
			else
			{
				Logic.Army logic = army.logic;
				list = ((logic != null) ? logic.units : null);
			}
			List<Logic.Unit> list2 = list;
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					Logic.Unit unit = list2[j];
					if (((unit != null) ? unit.def : null) != null && unit.def.type != Logic.Unit.Type.Noble)
					{
						num += (float)unit.def.tier;
						num2++;
					}
				}
			}
		}
		UIArmyStatusBar.ArmyInfo armyInfo = this.settlement_info;
		bool flag;
		if (armyInfo == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Settlement settlement = armyInfo.settlement;
			flag = (((settlement != null) ? settlement.garrison : null) != null);
		}
		if (flag)
		{
			List<Logic.Unit> units = this.settlement_info.settlement.garrison.units;
			for (int k = 0; k < units.Count; k++)
			{
				Logic.Unit unit2 = units[k];
				if (((unit2 != null) ? unit2.def : null) != null && unit2.def.type != Logic.Unit.Type.Noble)
				{
					num += (float)unit2.def.tier;
					num2++;
				}
			}
		}
		if (num2 != 0)
		{
			num /= (float)num2;
		}
		DT.Field thresholdField = UIArmyStatusBar.GetThresholdField(global::Defs.GetDefField("ArmyStatusBar", "Tiers"), num);
		if (thresholdField == null)
		{
			return;
		}
		this.m_Manpower.SetArmyStrengthColor(global::Defs.GetColor(thresholdField, "col", null));
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x000E5748 File Offset: 0x000E3948
	private bool CheckLevelChange(UIArmyStatusBar.ArmyInfo info)
	{
		Logic.Army logic = info.army.logic;
		if (logic == null)
		{
			return false;
		}
		int num = 0;
		if (logic.leader != null)
		{
			num = logic.leader.GetClassLevel();
			if (logic.leader.skills == null || logic.leader.skills.Count <= 0)
			{
				num = 0;
			}
		}
		if (num != info.last_level)
		{
			info.last_level = num;
			return true;
		}
		return false;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x000E57B4 File Offset: 0x000E39B4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "units_changed")
		{
			this.UpdateTier();
			return;
		}
		if (message == "armies_changed" || message == "players_changed" || message == "kingdom_changed" || message == "stance_changed")
		{
			this.UpdateStance();
			return;
		}
		if (!(message == "leader_changed"))
		{
			return;
		}
		this.UpdateLevels();
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x000E5824 File Offset: 0x000E3A24
	private void OnClick(BSGButton b)
	{
		if (ViewMode.IsPoliticalView() && !UserSettings.ClickableNameplates && !UICommon.GetModifierKey(UICommon.ModifierKey.Shift))
		{
			return;
		}
		if (this.armies.Count > 0)
		{
			BaseUI.Get().SelectObjFromLogic(this.armies[0].army.logic, false, true);
			return;
		}
		if (this.settlement_info != null)
		{
			BaseUI.Get().SelectObjFromLogic(this.settlement_info.settlement, false, true);
		}
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x000DF448 File Offset: 0x000DD648
	public override float BaseSortOrder()
	{
		return -1f;
	}

	// Token: 0x04000EE1 RID: 3809
	[UIFieldTarget("id_LeaderLevel")]
	private TextMeshProUGUI m_LeaderLevel;

	// Token: 0x04000EE2 RID: 3810
	[UIFieldTarget("id_Background")]
	private Image m_Background;

	// Token: 0x04000EE3 RID: 3811
	[UIFieldTarget("id_Frame")]
	private Image m_Frame;

	// Token: 0x04000EE4 RID: 3812
	[UIFieldTarget("id_LeaderLevelSupporter")]
	private TextMeshProUGUI m_LeaderLevelSupporter;

	// Token: 0x04000EE5 RID: 3813
	[UIFieldTarget("id_LeaderBackground")]
	private Image m_LeaderBackground;

	// Token: 0x04000EE6 RID: 3814
	[UIFieldTarget("id_LeaderBackgroundSupporter")]
	private Image m_LeaderBackgroundSupporter;

	// Token: 0x04000EE7 RID: 3815
	[UIFieldTarget("id_FrameSupporter")]
	private Image m_FrameSupporter;

	// Token: 0x04000EE8 RID: 3816
	[UIFieldTarget("id_GarrisonIcon")]
	private GameObject m_GarrisonIcon;

	// Token: 0x04000EE9 RID: 3817
	[UIFieldTarget("id_PlunderProgress")]
	private Image m_PlunderProgress;

	// Token: 0x04000EEA RID: 3818
	[UIFieldTarget("id_Morale")]
	private UIArmyMorale m_Morale;

	// Token: 0x04000EEB RID: 3819
	[UIFieldTarget("id_Manpower")]
	public UIArmyManpower m_Manpower;

	// Token: 0x04000EEC RID: 3820
	public BSGButton button;

	// Token: 0x04000EED RID: 3821
	private List<UIArmyStatusBar.ArmyInfo> armies = new List<UIArmyStatusBar.ArmyInfo>();

	// Token: 0x04000EEE RID: 3822
	private UIArmyStatusBar.ArmyInfo settlement_info;

	// Token: 0x04000EEF RID: 3823
	private bool visible;

	// Token: 0x04000EF0 RID: 3824
	private bool m_Invalidate;

	// Token: 0x04000EF1 RID: 3825
	private bool m_Initialzied;

	// Token: 0x020006D0 RID: 1744
	public class ArmyInfo
	{
		// Token: 0x04003729 RID: 14121
		public Logic.Settlement settlement;

		// Token: 0x0400372A RID: 14122
		public Logic.Kingdom kingdom;

		// Token: 0x0400372B RID: 14123
		public global::Army army;

		// Token: 0x0400372C RID: 14124
		public int last_level = -1;

		// Token: 0x0400372D RID: 14125
		public float last_unit_level = -1f;
	}
}
