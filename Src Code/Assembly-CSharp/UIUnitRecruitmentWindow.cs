using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class UIUnitRecruitmentWindow : UIWindow
{
	// Token: 0x06002C1B RID: 11291 RVA: 0x00171E8A File Offset: 0x0017008A
	public override string GetDefId()
	{
		return UIUnitRecruitmentWindow.def_id;
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06002C1C RID: 11292 RVA: 0x00171E91 File Offset: 0x00170091
	public int SlotIndex { get; }

	// Token: 0x06002C1D RID: 11293 RVA: 0x00171E9C File Offset: 0x0017009C
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ButtonRecruit != null)
		{
			this.m_ButtonRecruit.onClick = new BSGButton.OnClick(this.HandleOnUnitRecruit);
		}
		if (this.m_Close)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x00171F0C File Offset: 0x0017010C
	public void SetData(Castle castle, Logic.Army army, int slotIndex, Action<Logic.Unit.Def, int> OnHireCallback, bool showAllUnits = false)
	{
		this.Init();
		this.m_Castle = castle;
		this.m_Army = army;
		this.m_SlotIndex = slotIndex;
		this.onHire = OnHireCallback;
		this.m_HireTarget = null;
		this.m_ShowAllUnits = showAllUnits;
		if (!this.ValidateData())
		{
			this.Close(false);
			return;
		}
		this.Open();
		UIUnitRecruitmentWindow.current = this;
		if (this.m_Caption != null)
		{
			string key = (this.m_Army != null) ? "Castle.Recrutment.army" : "Castle.Recrutment.garrison";
			UIText.SetTextKey(this.m_Caption, key, null, null);
		}
		this.Refresh();
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x00171F9E File Offset: 0x0017019E
	public void SetCastle(Castle castle)
	{
		this.m_Castle = castle;
		if (!this.ValidateData())
		{
			this.Close(false);
			return;
		}
		this.Refresh();
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x00171FBD File Offset: 0x001701BD
	private bool ValidateData()
	{
		return (this.m_Castle == null || this.m_Castle.GetKingdom() == BaseUI.LogicKingdom()) && (this.m_Army == null || this.m_Army.GetKingdom() == BaseUI.LogicKingdom());
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton b)
	{
		this.Close(false);
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x00171FF8 File Offset: 0x001701F8
	public override void Close(bool silent = false)
	{
		UIUnitRecruitmentWindow.current = null;
		this.m_Army = null;
		this.m_Castle = null;
		base.Close(silent);
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x00172018 File Offset: 0x00170218
	public void Refresh()
	{
		UIUnitRecruitmentWindow.<>c__DisplayClass37_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.PopulateSource();
		UICommon.DeleteChildren(this.m_Militia);
		UICommon.DeleteChildren(this.m_Infantry);
		UICommon.DeleteChildren(this.m_Defense);
		UICommon.DeleteChildren(this.m_Cavalry);
		UICommon.DeleteChildren(this.m_Ranged);
		UICommon.DeleteChildren(this.m_RealmMilitia);
		UICommon.DeleteChildren(this.m_RealmInfantry);
		UICommon.DeleteChildren(this.m_RealmDefense);
		UICommon.DeleteChildren(this.m_RealmCavalry);
		UICommon.DeleteChildren(this.m_RealmRanged);
		for (int i = 0; i < this.slots.Count; i++)
		{
			UnityEngine.Object.Destroy(this.slots[i].gameObject);
		}
		this.slots.Clear();
		CS$<>8__locals1.prefab = UICommon.GetPrefab("UnitHireSlot", null);
		CS$<>8__locals1.slotIndex = 0;
		if (CS$<>8__locals1.prefab != null)
		{
			Game game = GameLogic.Get(true);
			List<Logic.Unit.Def> list = null;
			List<Logic.Unit.Def> list2 = null;
			if (this.m_ShowAllUnits)
			{
				list = new List<Logic.Unit.Def>();
				using (Dictionary<string, Def>.Enumerator enumerator = game.defs.Get(typeof(Logic.Unit.Def)).defs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, Def> keyValuePair = enumerator.Current;
						Logic.Unit.Def def = keyValuePair.Value as Logic.Unit.Def;
						if (def.type != Logic.Unit.Type.Noble)
						{
							list.Add(def);
						}
					}
					goto IL_178;
				}
			}
			list = this.m_Castle.GetAvailableUnitTypes(AvailableUnits.Source.Kingdom);
			list2 = this.m_Castle.GetAvailableUnitTypes(AvailableUnits.Source.Realm);
			IL_178:
			foreach (Logic.Unit.Def unitDef in list)
			{
				this.<Refresh>g__AddIcon|37_0(unitDef, AvailableUnits.Source.Kingdom, ref CS$<>8__locals1);
			}
			bool active = false;
			if (list2 != null)
			{
				foreach (Logic.Unit.Def def2 in list2)
				{
					if (!list.Contains(def2) && this.<Refresh>g__AddIcon|37_0(def2, AvailableUnits.Source.Realm, ref CS$<>8__locals1))
					{
						active = true;
					}
				}
			}
			if (this.m_SourceProvice != null)
			{
				this.m_SourceProvice.gameObject.SetActive(active);
			}
		}
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x0017226C File Offset: 0x0017046C
	private void PopulateSource()
	{
		bool flag = this.m_Army != null;
		if (this.m_ArmyLeader != null)
		{
			this.m_ArmyLeader.gameObject.SetActive(flag);
			if (flag)
			{
				this.m_ArmyLeader.SetObject(this.m_Army.leader, null);
			}
		}
		if (this.m_GarrisonIcon != null)
		{
			this.m_GarrisonIcon.SetActive(!flag);
		}
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x001722D9 File Offset: 0x001704D9
	private void HandleOnUnitRecruit(BSGButton btn)
	{
		if (this.m_HireTarget != null && this.onHire != null)
		{
			this.onHire(this.m_HireTarget, this.m_SlotIndex + 1);
		}
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x00172304 File Offset: 0x00170504
	private void HandleonUnitSlotSelect(UIGarrisonSlot obj)
	{
		this.m_HireTarget = obj.UnitDef;
		for (int i = 0; i < this.slots.Count; i++)
		{
			UIGarrisonSlot uigarrisonSlot = this.slots[i];
			uigarrisonSlot.Select(uigarrisonSlot == obj);
		}
		if ((this.m_ButtonRecruit == null || !this.m_ButtonRecruit.isActiveAndEnabled) && this.m_HireTarget != null && this.onHire != null)
		{
			this.onHire(this.m_HireTarget, -1);
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x00172388 File Offset: 0x00170588
	public static void CloseInstance()
	{
		if (UIUnitRecruitmentWindow.current != null)
		{
			UIUnitRecruitmentWindow.current.Close(false);
		}
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x001723A2 File Offset: 0x001705A2
	public static bool IsActive()
	{
		return !(UIUnitRecruitmentWindow.current == null) && UIUnitRecruitmentWindow.current.gameObject.activeInHierarchy;
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x001723C2 File Offset: 0x001705C2
	public static GameObject GetPrefab(string variant = null)
	{
		return UICommon.GetPrefab(UIUnitRecruitmentWindow.def_id, variant);
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x001723F0 File Offset: 0x001705F0
	[CompilerGenerated]
	private bool <Refresh>g__AddIcon|37_0(Logic.Unit.Def unitDef, AvailableUnits.Source source, ref UIUnitRecruitmentWindow.<>c__DisplayClass37_0 A_3)
	{
		if (unitDef.hide_if_unavailable && !unitDef.buildPrerqusite.Validate(this.m_Castle, null, false))
		{
			return false;
		}
		RectTransform rectTransform;
		switch (unitDef.type)
		{
		case Logic.Unit.Type.Militia:
			rectTransform = ((source == AvailableUnits.Source.Realm) ? this.m_RealmMilitia : this.m_Militia);
			break;
		case Logic.Unit.Type.Infantry:
			rectTransform = ((source == AvailableUnits.Source.Realm) ? this.m_RealmInfantry : this.m_Infantry);
			break;
		case Logic.Unit.Type.Defense:
			rectTransform = ((source == AvailableUnits.Source.Realm) ? this.m_RealmDefense : this.m_Defense);
			break;
		case Logic.Unit.Type.Cavalry:
			rectTransform = ((source == AvailableUnits.Source.Realm) ? this.m_RealmCavalry : this.m_Cavalry);
			break;
		case Logic.Unit.Type.Ranged:
			rectTransform = ((source == AvailableUnits.Source.Realm) ? this.m_RealmRanged : this.m_Ranged);
			break;
		default:
			rectTransform = null;
			break;
		}
		if (rectTransform == null)
		{
			return false;
		}
		UIGarrisonSlot component = UnityEngine.Object.Instantiate<GameObject>(A_3.prefab, Vector3.zero, Quaternion.identity, rectTransform).GetComponent<UIGarrisonSlot>();
		if (component != null)
		{
			component.SetData(unitDef, A_3.slotIndex, this.m_Castle, this.m_Army);
			component.OnSelected += this.HandleonUnitSlotSelect;
			component.OnFocused += this.HandleonUnitSlotSelect;
			component.Select(false);
			this.slots.Add(component);
		}
		return true;
	}

	// Token: 0x04001E10 RID: 7696
	private static string def_id = "UnitRecruitmentWindow";

	// Token: 0x04001E11 RID: 7697
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001E12 RID: 7698
	[UIFieldTarget("id_Militia")]
	private RectTransform m_Militia;

	// Token: 0x04001E13 RID: 7699
	[UIFieldTarget("id_Infantry")]
	private RectTransform m_Infantry;

	// Token: 0x04001E14 RID: 7700
	[UIFieldTarget("id_Defense")]
	private RectTransform m_Defense;

	// Token: 0x04001E15 RID: 7701
	[UIFieldTarget("id_Cavalry")]
	private RectTransform m_Cavalry;

	// Token: 0x04001E16 RID: 7702
	[UIFieldTarget("id_Ranged")]
	private RectTransform m_Ranged;

	// Token: 0x04001E17 RID: 7703
	[UIFieldTarget("id_Recruit")]
	private BSGButton m_ButtonRecruit;

	// Token: 0x04001E18 RID: 7704
	[UIFieldTarget("id_RealmMilitia")]
	private RectTransform m_RealmMilitia;

	// Token: 0x04001E19 RID: 7705
	[UIFieldTarget("id_RealmInfantry")]
	private RectTransform m_RealmInfantry;

	// Token: 0x04001E1A RID: 7706
	[UIFieldTarget("id_RealmDefense")]
	private RectTransform m_RealmDefense;

	// Token: 0x04001E1B RID: 7707
	[UIFieldTarget("id_RealmCavalry")]
	private RectTransform m_RealmCavalry;

	// Token: 0x04001E1C RID: 7708
	[UIFieldTarget("id_RealmRanged")]
	private RectTransform m_RealmRanged;

	// Token: 0x04001E1D RID: 7709
	[UIFieldTarget("id_Source_Kingdom")]
	private RectTransform m_SourceKingdom;

	// Token: 0x04001E1E RID: 7710
	[UIFieldTarget("id_Source_Provice")]
	private RectTransform m_SourceProvice;

	// Token: 0x04001E1F RID: 7711
	[UIFieldTarget("id_ArmyLeader")]
	private UICharacterIcon m_ArmyLeader;

	// Token: 0x04001E20 RID: 7712
	[UIFieldTarget("id_GarrisonIcon")]
	private GameObject m_GarrisonIcon;

	// Token: 0x04001E21 RID: 7713
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001E22 RID: 7714
	private static UIUnitRecruitmentWindow current;

	// Token: 0x04001E24 RID: 7716
	private Castle m_Castle;

	// Token: 0x04001E25 RID: 7717
	private Logic.Army m_Army;

	// Token: 0x04001E26 RID: 7718
	private Logic.Unit.Def m_HireTarget;

	// Token: 0x04001E27 RID: 7719
	private int m_SlotIndex;

	// Token: 0x04001E28 RID: 7720
	private bool m_ShowAllUnits;

	// Token: 0x04001E29 RID: 7721
	private List<UIGarrisonSlot> slots = new List<UIGarrisonSlot>();

	// Token: 0x04001E2A RID: 7722
	private Action<Logic.Unit.Def, int> onHire;
}
