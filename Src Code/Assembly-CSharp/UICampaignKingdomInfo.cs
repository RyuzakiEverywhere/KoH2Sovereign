using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002D2 RID: 722
public class UICampaignKingdomInfo : MonoBehaviour, RemoteVars.IListener
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x0017B0A4 File Offset: 0x001792A4
	// (set) Token: 0x06002DAA RID: 11690 RVA: 0x0017B0AC File Offset: 0x001792AC
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06002DAB RID: 11691 RVA: 0x0017B0B5 File Offset: 0x001792B5
	// (set) Token: 0x06002DAC RID: 11692 RVA: 0x0017B0BD File Offset: 0x001792BD
	public Logic.Realm Realm { get; private set; }

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06002DAD RID: 11693 RVA: 0x0017B0C6 File Offset: 0x001792C6
	// (set) Token: 0x06002DAE RID: 11694 RVA: 0x0017B0CE File Offset: 0x001792CE
	public Campaign Campaign { get; private set; }

	// Token: 0x06002DAF RID: 11695 RVA: 0x0017B0D7 File Offset: 0x001792D7
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x0017B0E0 File Offset: 0x001792E0
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.PopulateUnitsSlots();
		if (this.m_PickKingdom != null)
		{
			this.m_PickKingdom.onClick = new BSGButton.OnClick(this.HandleOnPickKingdom);
			Tooltip.Get(this.m_PickKingdom.gameObject, true).SetDef("PickKingdomTooltip", null);
		}
		UIKingdomIcon kingdomCrest = this.m_KingdomCrest;
		if (((kingdomCrest != null) ? kingdomCrest.GetPrimary() : null) != null)
		{
			this.m_KingdomCrest.GetPrimary().onClick = new KingdomShield.OnShieldClick(this.HandleShieldClick);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x0017B184 File Offset: 0x00179384
	public void SetKingdom(Campaign campaign, Logic.Kingdom k, Logic.Realm r)
	{
		this.Init();
		Campaign campaign2 = this.Campaign;
		if (campaign2 != null)
		{
			campaign2.DelVarsListener(this);
		}
		this.Campaign = null;
		this.Campaign = campaign;
		Campaign campaign3 = this.Campaign;
		if (campaign3 != null)
		{
			campaign3.AddVarsListener(this);
		}
		this.Kingdom = k;
		this.Realm = r;
		this.Refresh();
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x0017B1DD File Offset: 0x001793DD
	private void Refresh()
	{
		this.PopulateKingdom();
		this.PopulateKingdomUnits();
		this.PopulateVassalsOrLeage();
		this.UpdatePick();
		this.LocalzieStatics();
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x0017B200 File Offset: 0x00179400
	private void LocalzieStatics()
	{
		if (this.m_PickKingdomLabel != null)
		{
			DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.Campaign, "pick_kingdom");
			string key;
			if (selectedOption != null && selectedOption.key.Contains("province"))
			{
				key = "TitleScreen.SinglePlayer.pick_province";
			}
			else
			{
				key = "TitleScreen.SinglePlayer.pick_kingdom";
			}
			UIText.SetTextKey(this.m_PickKingdomLabel, key, null, null);
		}
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x0017B260 File Offset: 0x00179460
	private void PopulateKingdom()
	{
		if (this.Kingdom == null)
		{
			this.DoHide();
			return;
		}
		this.DoShow();
		if (this.m_KingdomCrest != null)
		{
			this.m_KingdomCrest.SetObject(this.Kingdom, null);
		}
		if (this.m_KingdomName != null)
		{
			UIText.SetTextKey(this.m_KingdomName, (this.Kingdom == null) ? "" : "TitleScreen.SinglePlayer.Kingdom.name", new Vars(this.Kingdom), null);
		}
		if (this.m_Religion != null)
		{
			UIText.SetTextKey(this.m_Religion, "TitleScreen.SinglePlayer.Kingdom.religion", this.Kingdom, null);
		}
		if (this.m_ReligionIcon != null)
		{
			Image religionIcon = this.m_ReligionIcon;
			Logic.Kingdom kingdom = this.Kingdom;
			DT.Field field;
			if (kingdom == null)
			{
				field = null;
			}
			else
			{
				Religion religion = kingdom.religion;
				if (religion == null)
				{
					field = null;
				}
				else
				{
					Religion.Def def = religion.def;
					field = ((def != null) ? def.field : null);
				}
			}
			religionIcon.overrideSprite = global::Defs.GetObj<Sprite>(field, global::Religions.GetRelgionIconKey(this.Kingdom), null);
		}
		if (this.m_Culture != null)
		{
			UIText.SetTextKey(this.m_Culture, "TitleScreen.SinglePlayer.Kingdom.culture", this.Kingdom, null);
		}
		if (this.m_Complexity != null)
		{
			UIText.SetTextKey(this.m_Complexity, "TitleScreen.SinglePlayer.Kingdom.difficulty", this.Kingdom, null);
		}
		if (this.m_Description != null)
		{
			if (global::Defs.FindTextField(this.Kingdom.Name + ".description") != null)
			{
				UIText.SetTextKey(this.m_Description, this.Kingdom.Name + ".description", this.Kingdom, null);
				return;
			}
			UIText.SetTextKey(this.m_Description, "TitleScreen.SinglePlayer.Kingdom.description_fallback", null, null);
		}
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x0017B408 File Offset: 0x00179608
	private void PopulateVassalsOrLeage()
	{
		if (this.m_VassalsGroup == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		bool flag = ((kingdom != null) ? kingdom.vassalStates : null) != null && this.Kingdom.vassalStates.Count > 0;
		Logic.Kingdom kingdom2 = this.Kingdom;
		bool flag2 = ((kingdom2 != null) ? kingdom2.sovereignState : null) != null;
		this.m_VassalsGroup.gameObject.SetActive(flag || flag2);
		if (!flag && !flag2)
		{
			return;
		}
		if (this.m_VassalsContainder != null)
		{
			UICommon.DeleteChildren(this.m_VassalsContainder.transform);
			if (flag)
			{
				for (int i = 0; i < this.Kingdom.vassalStates.Count; i++)
				{
					Vars vars = new Vars();
					vars.Set<string>("variant", "compact");
					UIKingdomIcon component = ObjectIcon.GetIcon(this.Kingdom.vassalStates[i], vars, this.m_VassalsContainder.transform as RectTransform).GetComponent<UIKingdomIcon>();
					if (((component != null) ? component.GetPrimary() : null) != null)
					{
						component.GetPrimary().onClick = new KingdomShield.OnShieldClick(this.HandleShieldClick);
					}
				}
			}
			else
			{
				Vars vars2 = new Vars();
				vars2.Set<string>("variant", "compact");
				UIKingdomIcon component2 = ObjectIcon.GetIcon(this.Kingdom.sovereignState, vars2, this.m_VassalsContainder.transform as RectTransform).GetComponent<UIKingdomIcon>();
				if (((component2 != null) ? component2.GetPrimary() : null) != null)
				{
					component2.GetPrimary().onClick = new KingdomShield.OnShieldClick(this.HandleShieldClick);
				}
			}
			this.m_VassalsContainder.Refresh();
		}
		if (this.m_VassalsLabel != null)
		{
			string key = flag ? "TitleScreen.SinglePlayer.Kingdom.vassals" : "TitleScreen.SinglePlayer.Kingdom.leage";
			UIText.SetTextKey(this.m_VassalsLabel, key, this.Kingdom, null);
		}
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x0017B5F0 File Offset: 0x001797F0
	private void PopulateLeage()
	{
		if (this.m_VassalsGroup == null)
		{
			return;
		}
		Logic.Kingdom kingdom = this.Kingdom;
		bool flag = ((kingdom != null) ? kingdom.vassalStates : null) != null && this.Kingdom.vassalStates.Count > 0;
		this.m_VassalsGroup.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		if (this.m_VassalsContainder == null)
		{
			UICommon.DeleteChildren(this.m_VassalsContainder.transform);
			for (int i = 0; i < this.Kingdom.vassalStates.Count; i++)
			{
				ObjectIcon.GetIcon(this.Kingdom.vassalStates[i], null, this.m_VassalsContainder.transform as RectTransform);
			}
		}
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x0017B6B4 File Offset: 0x001798B4
	private void UpdatePick()
	{
		bool flag = this.Campaign.IsMultiplayerCampaign();
		bool flag2 = this.Campaign.state >= Campaign.State.Started;
		int kingdomsOwningPlayerIndex = CampaignUtils.GetKingdomsOwningPlayerIndex(this.Campaign, this.Kingdom);
		bool flag3 = kingdomsOwningPlayerIndex != -1;
		bool flag4 = true;
		bool flag5 = (flag ? CampaignUtils.GetPlayerIndex(this.Campaign, THQNORequest.userId) : 0) == kingdomsOwningPlayerIndex;
		bool flag6 = CampaignUtils.IsBlackListedKingdom(this.Campaign, this.Kingdom);
		Logic.Kingdom kingdom = this.Kingdom;
		bool flag7 = CampaignUtils.IsPlayerEligible((kingdom != null) ? kingdom.game : null, this.Kingdom);
		flag7 &= !flag6;
		bool flag8 = flag && (!flag3 || flag4) && flag7 && !flag5;
		string a = this.Campaign.GetVar("pick_kingdom", null, true);
		bool flag9 = a == "random_province";
		bool flag10 = a == "random_kingdom";
		bool flag11 = this.Campaign.IsLocalPlayerReady() && !this.Campaign.IsAuthority();
		flag7 &= (!flag9 && !flag10);
		flag8 &= (!flag9 && !flag10 && !flag2 && !flag11);
		if (this.m_PickKingdom != null)
		{
			this.m_PickKingdom.gameObject.SetActive(flag8);
			bool flag12 = this.Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, this.Campaign.GetLocalPlayerID(), "start_countdown");
			this.m_PickKingdom.Enable(!flag12, false);
		}
		if (this.m_PickWarning != null)
		{
			string key;
			if (flag2)
			{
				key = "TitleScreen.Multiplayer.kingdom_picked_forbidden_after_start";
			}
			else if (flag10)
			{
				key = "TitleScreen.Multiplayer.random_kingdom";
			}
			else if (flag9)
			{
				key = "TitleScreen.Multiplayer.random_province";
			}
			else if (flag6)
			{
				key = "TitleScreen.Multiplayer.kingdom_picked_forbidden_after_start";
			}
			else if (!flag7)
			{
				key = "TitleScreen.Multiplayer.kingodm_picked_invalid";
			}
			else if (flag5 && flag)
			{
				key = "TitleScreen.Multiplayer.kingodm_picked_self";
			}
			else
			{
				key = string.Empty;
			}
			UIText.SetTextKey(this.m_PickWarning, key, null, null);
			this.m_PickWarning.gameObject.SetActive(!flag8 || !flag7 || flag6);
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x0017B8D8 File Offset: 0x00179AD8
	private void PopulateUnitsSlots()
	{
		if (this.m_SpecalUnitsContainer != null)
		{
			global::Common.FindChildrenWithComponent<UIUnitSlot>(this.m_SpecalUnitsContainer.gameObject, this.m_UnitSlots);
			for (int i = 0; i < this.m_UnitSlots.Count; i++)
			{
				this.m_UnitSlots[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x0017B938 File Offset: 0x00179B38
	private void PopulateKingdomUnits()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		this.tmp_SpecialUnits.Clear();
		this.tmp_SpecialUnits.AddRange(this.Kingdom.unit_types);
		if (this.Kingdom.units_set != null)
		{
			Logic.Defs defs = this.Kingdom.game.defs;
			Logic.Kingdom kingdom = this.Kingdom;
			AvailableUnits.Def def = defs.Get<AvailableUnits.Def>((kingdom != null) ? kingdom.units_set : null);
			if (def != null)
			{
				for (int i = 0; i < def.avaliable_types.Count; i++)
				{
					if (def.avaliable_types[i].special && !this.tmp_SpecialUnits.Contains(def.avaliable_types[i].name))
					{
						this.tmp_SpecialUnits.Add(def.avaliable_types[i].name);
					}
				}
			}
		}
		if (this.Kingdom.unit_types != null)
		{
			for (int j = 0; j < this.Kingdom.unit_types.Count; j++)
			{
				string item = this.Kingdom.unit_types[j];
				if (!this.tmp_SpecialUnits.Contains(item))
				{
					this.tmp_SpecialUnits.Add(item);
				}
			}
		}
		foreach (Logic.Realm realm in this.Kingdom.realms)
		{
			if (realm.unit_types != null)
			{
				for (int k = 0; k < realm.unit_types.Count; k++)
				{
					string item2 = realm.unit_types[k];
					if (!this.tmp_SpecialUnits.Contains(item2))
					{
						this.tmp_SpecialUnits.Add(item2);
					}
				}
			}
		}
		if (this.m_UnitSlots != null)
		{
			for (int l = 0; l < this.m_UnitSlots.Count; l++)
			{
				UIUnitSlot uiunitSlot = this.m_UnitSlots[l];
				Logic.Unit.Def def2 = (this.tmp_SpecialUnits.Count > l) ? this.Kingdom.game.defs.Find<Logic.Unit.Def>(this.tmp_SpecialUnits[l]) : null;
				if (def2 != null)
				{
					uiunitSlot.SetDef(def2, 0);
				}
				uiunitSlot.gameObject.SetActive(def2 != null);
			}
		}
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x0017BB80 File Offset: 0x00179D80
	public void Show(bool shown)
	{
		using (Game.Profile("UICampaignKingdomInfo.Show", false, 0f, null))
		{
			this.m_Shown = shown;
			if (this.m_Shown)
			{
				this.DoShow();
			}
			else
			{
				this.DoHide();
			}
		}
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x0017BBDC File Offset: 0x00179DDC
	private void DoShow()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x0017BBF3 File Offset: 0x00179DF3
	private void DoHide()
	{
		Transform transform = base.transform;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x0017BC08 File Offset: 0x00179E08
	private void ExtractRoyalFamily(Logic.Kingdom k)
	{
		k.royalFamily = new Logic.RoyalFamily(k);
		CharacterFactory.PopulateInitialRoyalFamily(k);
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x0017BC1C File Offset: 0x00179E1C
	private bool HandleShieldClick(PointerEventData e, KingdomShield s)
	{
		TitleUI titleUI = BaseUI.Get<TitleUI>();
		if (titleUI == null)
		{
			return true;
		}
		Logic.Kingdom kingdom = s.logicObject as Logic.Kingdom;
		if (kingdom == null)
		{
			return true;
		}
		titleUI.UpdateSelectedKingdom(this.Campaign, kingdom);
		return true;
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x0017BC59 File Offset: 0x00179E59
	private void HandleOnPickKingdom(BSGButton btn)
	{
		Action<Logic.Kingdom, Logic.Realm> onPickKingdom = this.OnPickKingdom;
		if (onPickKingdom == null)
		{
			return;
		}
		onPickKingdom(this.Kingdom, this.Realm);
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x0017BC78 File Offset: 0x00179E78
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (key == "kingdom_name" || key == "ready" || key == "pick_kingdom" || key == "start_countdown" || key == "from_save_id")
		{
			this.UpdatePick();
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x0017BCCC File Offset: 0x00179ECC
	private void OnDestroy()
	{
		if (this.Campaign != null)
		{
			this.Campaign.DelVarsListener(this);
		}
	}

	// Token: 0x04001EDB RID: 7899
	[UIFieldTarget("id_KingdomCrest")]
	private UIKingdomIcon m_KingdomCrest;

	// Token: 0x04001EDC RID: 7900
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x04001EDD RID: 7901
	[UIFieldTarget("id_Religion")]
	private TextMeshProUGUI m_Religion;

	// Token: 0x04001EDE RID: 7902
	[UIFieldTarget("id_ReligionIcon")]
	private Image m_ReligionIcon;

	// Token: 0x04001EDF RID: 7903
	[UIFieldTarget("id_Culture")]
	private TextMeshProUGUI m_Culture;

	// Token: 0x04001EE0 RID: 7904
	[UIFieldTarget("id_Complexity")]
	private TextMeshProUGUI m_Complexity;

	// Token: 0x04001EE1 RID: 7905
	[UIFieldTarget("id_VassalContainer")]
	private RectTransform m_VassalContainer;

	// Token: 0x04001EE2 RID: 7906
	[UIFieldTarget("id_Label_Units")]
	private TextMeshProUGUI m_LabelUnits;

	// Token: 0x04001EE3 RID: 7907
	[UIFieldTarget("id_SpecalUnitsContainer")]
	private RectTransform m_SpecalUnitsContainer;

	// Token: 0x04001EE4 RID: 7908
	[UIFieldTarget("id_Label_History")]
	private TextMeshProUGUI m_LabelHistory;

	// Token: 0x04001EE5 RID: 7909
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x04001EE6 RID: 7910
	[UIFieldTarget("id_PickKingdom")]
	private BSGButton m_PickKingdom;

	// Token: 0x04001EE7 RID: 7911
	[UIFieldTarget("id_PickKingdomLabel")]
	private TextMeshProUGUI m_PickKingdomLabel;

	// Token: 0x04001EE8 RID: 7912
	[UIFieldTarget("id_PickWarning")]
	private TextMeshProUGUI m_PickWarning;

	// Token: 0x04001EE9 RID: 7913
	[UIFieldTarget("id_VassalsGroup")]
	private RectTransform m_VassalsGroup;

	// Token: 0x04001EEA RID: 7914
	[UIFieldTarget("id_VassalsContainder")]
	private StackableIconsContainer m_VassalsContainder;

	// Token: 0x04001EEB RID: 7915
	[UIFieldTarget("id_VassalsLabel")]
	private TextMeshProUGUI m_VassalsLabel;

	// Token: 0x04001EEF RID: 7919
	private List<UIUnitSlot> m_UnitSlots = new List<UIUnitSlot>();

	// Token: 0x04001EF0 RID: 7920
	private List<string> tmp_SpecialUnits = new List<string>();

	// Token: 0x04001EF1 RID: 7921
	public Action<Logic.Kingdom, Logic.Realm> OnPickKingdom;

	// Token: 0x04001EF2 RID: 7922
	private bool m_Initialzied;

	// Token: 0x04001EF3 RID: 7923
	private bool m_Shown = true;
}
