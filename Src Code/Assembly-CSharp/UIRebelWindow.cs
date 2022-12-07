using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027D RID: 637
public class UIRebelWindow : ObjectWindow
{
	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060026E7 RID: 9959 RVA: 0x00153761 File Offset: 0x00151961
	// (set) Token: 0x060026E8 RID: 9960 RVA: 0x00153769 File Offset: 0x00151969
	public Logic.Rebel Rebel { get; private set; }

	// Token: 0x060026E9 RID: 9961 RVA: 0x00153774 File Offset: 0x00151974
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.UnitFramePrefab = UICommon.GetPrefab("UnitSlot", null);
		if (this.m_Icon != null)
		{
			this.m_Icon.onDoubleClick = new BSGButton.OnClick(this.HandleOnRebelionIconClick);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x001537CE File Offset: 0x001519CE
	private void Start()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x001537D6 File Offset: 0x001519D6
	private void OnEnable()
	{
		if (this.logicObject == null)
		{
			this.ExtractLogicObject();
		}
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x001537E8 File Offset: 0x001519E8
	private void ExtractLogicObject()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		if (worldUI.selected_obj == null)
		{
			return;
		}
		Logic.Rebel rebel = worldUI.selected_logic_obj as Logic.Rebel;
		if (rebel != null && rebel != this.Rebel)
		{
			this.SetObject(worldUI.selected_logic_obj, new Vars(worldUI.selected_logic_obj));
		}
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x00153848 File Offset: 0x00151A48
	public override void SetObject(Logic.Object obj, Vars vars = null)
	{
		this.Init();
		Logic.Rebel rebel = this.Rebel;
		if (rebel != null)
		{
			Logic.Army army = rebel.army;
			if (army != null)
			{
				army.DelListener(this);
			}
		}
		base.SetObject(obj, vars);
		if (obj is Logic.Rebel)
		{
			this.Rebel = (obj as Logic.Rebel);
		}
		Logic.Rebel rebel2 = this.Rebel;
		if (rebel2 != null)
		{
			Logic.Army army2 = rebel2.army;
			if (army2 != null)
			{
				army2.AddListener(this);
			}
		}
		if (this.m_BountyButton != null)
		{
			this.m_BountyButton.onClick = new BSGButton.OnClick(this.HandleBountySelect);
		}
		if (this.m_ArmyMorale != null)
		{
			this.m_ArmyMorale.Clear();
			if (this.Rebel != null)
			{
				UIArmyMorale armyMorale = this.m_ArmyMorale;
				Logic.Army army3 = this.Rebel.army;
				armyMorale.AddArmy(((army3 != null) ? army3.visuals : null) as global::Army);
			}
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
			if (this.Rebel != null)
			{
				UIArmyManpower manpower = this.m_Manpower;
				Logic.Army army4 = this.Rebel.army;
				manpower.AddArmy(((army4 != null) ? army4.visuals : null) as global::Army);
			}
		}
		if (this.m_Icon != null)
		{
			Image component = this.m_Icon.GetComponent<Image>();
			if (component != null)
			{
				Image image = component;
				Logic.Rebel rebel3 = this.Rebel;
				DT.Field field;
				if (rebel3 == null)
				{
					field = null;
				}
				else
				{
					Logic.Rebel.Def def = rebel3.def;
					field = ((def != null) ? def.field : null);
				}
				image.overrideSprite = global::Defs.GetObj<Sprite>(field, "icon", null);
			}
			Tooltip.Get(this.m_Icon.gameObject, true).SetDef("RebelTooltip", new Vars(this.Rebel));
		}
		if (this.m_ProvinceIllustration != null)
		{
			UIProvinceIllustration provinceIllustration = this.m_ProvinceIllustration;
			Logic.Rebel rebel4 = this.Rebel;
			provinceIllustration.SetObject((rebel4 != null) ? rebel4.army : null);
		}
		this.Refresh();
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x00153A14 File Offset: 0x00151C14
	public override void Refresh()
	{
		this.PopulateLeader();
		this.PopulateStats();
		this.PopulateAgenda();
		this.CreateUnitFrames();
		this.UpdateUnitFrames();
		this.UpdateBanner();
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x00153A3C File Offset: 0x00151C3C
	private void PopulateLeader()
	{
		if (this.Rebel == null)
		{
			return;
		}
		bool flag = this.Rebel.IsGeneral();
		flag |= this.Rebel.IsLeader();
		if (this.m_LeaderlessIcon != null)
		{
			this.m_LeaderlessIcon.gameObject.SetActive(!flag);
			Tooltip.Get(this.m_LeaderlessIcon.gameObject, true).SetText("Rebel.leaderless", null, null);
		}
		if (this.m_RebelLeader != null)
		{
			this.m_RebelLeader.gameObject.SetActive(flag);
			if (flag)
			{
				this.m_RebelLeader.SetObject(this.Rebel.army.leader, null);
			}
			else
			{
				this.m_RebelLeader.SetObject(null, null);
			}
		}
		if (this.m_RebeType != null)
		{
			UIText.SetText(this.m_RebeType, global::Defs.Localize(this.Rebel.def.field, "type_name", null, null, true, true));
		}
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x00153B30 File Offset: 0x00151D30
	private void CreateUnitFrames()
	{
		if (this.UnitFramePrefab == null)
		{
			return;
		}
		if (this.unitSlotsContainer == null)
		{
			return;
		}
		if (this.Rebel == null)
		{
			return;
		}
		if (this.unitSlots == null)
		{
			this.unitSlots = new List<UIUnitSlot>(30);
		}
		int num = this.Rebel.army.MaxUnits();
		if (this.unitSlots.Count < num)
		{
			int num2 = num - this.unitSlots.Count;
			for (int i = 0; i < num2; i++)
			{
				UIUnitSlot component = UnityEngine.Object.Instantiate<GameObject>(this.UnitFramePrefab, this.unitSlotsContainer).GetComponent<UIUnitSlot>();
				this.unitSlots.Add(component);
			}
		}
		for (int j = 0; j < this.unitSlots.Count; j++)
		{
			UIUnitSlot uiunitSlot = this.unitSlots[j];
			if (!(uiunitSlot == null))
			{
				if (j >= num)
				{
					uiunitSlot.gameObject.SetActive(false);
				}
				else
				{
					uiunitSlot.gameObject.SetActive(true);
					uiunitSlot.SetUnitInstance(null, j, this.Rebel.army, null);
				}
			}
		}
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x00153C40 File Offset: 0x00151E40
	private void UpdateUnitFrames()
	{
		if (this.unitSlots == null)
		{
			return;
		}
		if (this.Rebel == null)
		{
			return;
		}
		int count = this.unitSlots.Count;
		this.Rebel.army.MaxUnits();
		global::Army army = this.Rebel.army.visuals as global::Army;
		int num = 0;
		int num2 = 0;
		while (num2 + num < count)
		{
			UIUnitSlot uiunitSlot = this.unitSlots[num2 + num];
			if (!(uiunitSlot == null))
			{
				global::Unit unit = (army.units.Count > num2) ? army.units[num2] : null;
				if (unit != null && unit.logic.def.type == Logic.Unit.Type.Noble)
				{
					num--;
				}
				else if (unit != null)
				{
					uiunitSlot.SetUnitInstance(unit.logic, num2 + num, this.Rebel.army, null);
				}
			}
			num2++;
		}
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x00153D20 File Offset: 0x00151F20
	private void PopulateStats()
	{
		if (this.Rebel == null)
		{
			return;
		}
		Vars vars = new Vars(this.Rebel);
		if (this.m_RebelLevelText != null)
		{
			UIText.SetText(this.m_RebelLevelText, string.Format("{0}/{1}", this.Rebel.level, this.Rebel.def.max_level));
			Tooltip.Get(this.m_RebelLevelText.gameObject, true).SetText("Rebel.level", null, vars);
		}
		if (this.Rebel.HasBounty())
		{
			UIText.SetText(this.m_BountyText, this.Rebel.GetBounty().ToString());
			Tooltip.Get(this.m_BountyText.transform.parent.gameObject, true).SetText("Rebels.Bounty.current", null, vars);
			return;
		}
		UIText.SetTextKey(this.m_BountyText, "Rebels.Bounty.no_bounty", vars, null);
		Tooltip.Get(this.m_BountyText.transform.parent.gameObject, true).SetText("Rebels.Bounty.label_current", null, vars);
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x00153E38 File Offset: 0x00152038
	private void PopulateAgenda()
	{
		if (this.Rebel == null || this.Rebel.agenda == null)
		{
			return;
		}
		if (this.Rebel.agenda.def != null)
		{
			UIText.SetText(this.m_TextAgenda, this.Rebel.agenda.def.field, "flavor", null, null);
		}
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x00153E94 File Offset: 0x00152094
	private void UpdateBanner()
	{
		Vars vars = new Vars();
		Vars vars2 = vars;
		string key = "rebellion";
		Logic.Rebel rebel = this.Rebel;
		vars2.Set<Rebellion>(key, (rebel != null) ? rebel.rebellion : null);
		UIKingdomIcon kingdomCrest = this.m_KingdomCrest;
		if (kingdomCrest == null)
		{
			return;
		}
		kingdomCrest.SetObject(this.Rebel, vars);
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleBountySelect(BSGButton b)
	{
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x001537CE File Offset: 0x001519CE
	public override void ValidateSelectionObject()
	{
		this.ExtractLogicObject();
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x00153EDC File Offset: 0x001520DC
	private void HandleOnRebelionIconClick(BSGButton b)
	{
		if (this.Rebel == null)
		{
			return;
		}
		if (this.Rebel.IsLeader())
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		Logic.Rebel rebel = this.Rebel;
		Logic.Object obj;
		if (rebel == null)
		{
			obj = null;
		}
		else
		{
			Rebellion rebellion = rebel.rebellion;
			if (rebellion == null)
			{
				obj = null;
			}
			else
			{
				Logic.Rebel leader = rebellion.leader;
				obj = ((leader != null) ? leader.army : null);
			}
		}
		worldUI.LookAt(obj, false);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x00153F3A File Offset: 0x0015213A
	protected override void HandleLogicMessage(object obj, string message, object param)
	{
		base.HandleLogicMessage(obj, message, param);
		if (message == "units_changed" || message == "rebel_type_changed" || message == "leader_changed")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x00153F74 File Offset: 0x00152174
	public override void Release()
	{
		if (this.m_ArmyMorale != null)
		{
			this.m_ArmyMorale.Clear();
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
		}
		Logic.Rebel rebel = this.Rebel;
		if (rebel != null)
		{
			Logic.Army army = rebel.army;
			if (army != null)
			{
				army.DelListener(this);
			}
		}
		Logic.Rebel rebel2 = this.Rebel;
		if (rebel2 != null)
		{
			rebel2.DelListener(this);
		}
		this.Rebel = null;
		base.Release();
	}

	// Token: 0x04001A57 RID: 6743
	[UIFieldTarget("id_LeaderName")]
	private TextMeshProUGUI m_LeaderName;

	// Token: 0x04001A58 RID: 6744
	[UIFieldTarget("id_RebelLeader")]
	private UICharacterIcon m_RebelLeader;

	// Token: 0x04001A59 RID: 6745
	[UIFieldTarget("id_LeaderlessIcon")]
	private RectTransform m_LeaderlessIcon;

	// Token: 0x04001A5A RID: 6746
	[UIFieldTarget("id_Icon")]
	private BSGButton m_Icon;

	// Token: 0x04001A5B RID: 6747
	[UIFieldTarget("id_RebeType")]
	private TextMeshProUGUI m_RebeType;

	// Token: 0x04001A5C RID: 6748
	[UIFieldTarget("id_Origin")]
	private TextMeshProUGUI m_Origin;

	// Token: 0x04001A5D RID: 6749
	[UIFieldTarget("id_GorupUnits")]
	private RectTransform unitSlotsContainer;

	// Token: 0x04001A5E RID: 6750
	[UIFieldTarget("id_AgentaText")]
	private TextMeshProUGUI m_TextAgenda;

	// Token: 0x04001A5F RID: 6751
	[UIFieldTarget("id_ArmyMorale")]
	private UIArmyMorale m_ArmyMorale;

	// Token: 0x04001A60 RID: 6752
	[UIFieldTarget("id_RebelLevelText")]
	private TextMeshProUGUI m_RebelLevelText;

	// Token: 0x04001A61 RID: 6753
	[UIFieldTarget("id_BountyText")]
	private TextMeshProUGUI m_BountyText;

	// Token: 0x04001A62 RID: 6754
	[UIFieldTarget("id_BountyButton")]
	private BSGButton m_BountyButton;

	// Token: 0x04001A63 RID: 6755
	[UIFieldTarget("id_Banner")]
	private Image m_Banner;

	// Token: 0x04001A64 RID: 6756
	[UIFieldTarget("id_KingdomCrest")]
	private UIKingdomIcon m_KingdomCrest;

	// Token: 0x04001A65 RID: 6757
	[UIFieldTarget("id_BannerIcon")]
	private Image m_BannerIcon;

	// Token: 0x04001A66 RID: 6758
	[UIFieldTarget("id_Manpower")]
	private UIArmyManpower m_Manpower;

	// Token: 0x04001A67 RID: 6759
	[UIFieldTarget("id_ProvinceIllustration")]
	private UIProvinceIllustration m_ProvinceIllustration;

	// Token: 0x04001A68 RID: 6760
	private GameObject UnitFramePrefab;

	// Token: 0x04001A69 RID: 6761
	private List<UIUnitSlot> unitSlots;

	// Token: 0x04001A6B RID: 6763
	private bool m_Initialzied;
}
