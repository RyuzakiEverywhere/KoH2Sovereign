using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200027C RID: 636
public class UIKingdomStabilityRebellionRow : Hotspot, IListener
{
	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060026D3 RID: 9939 RVA: 0x0015304C File Offset: 0x0015124C
	// (set) Token: 0x060026D4 RID: 9940 RVA: 0x00153054 File Offset: 0x00151254
	public Rebellion rebellion { get; private set; }

	// Token: 0x060026D5 RID: 9941 RVA: 0x0015305D File Offset: 0x0015125D
	private void OnDestroy()
	{
		if (this.rebellion != null)
		{
			this.rebellion.DelListener(this);
		}
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x00153074 File Offset: 0x00151274
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_leaderIcon)
		{
			this.m_leaderIcon.OnSelect += this.HandleOnLeaderSelected;
			this.m_leaderIcon.OnPointerEvent += this.HandleOnPointerEvents;
			this.m_leaderIcon.EnableClassLevel(true);
		}
		this.m_Initialized = true;
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x001530E0 File Offset: 0x001512E0
	public void SetObject(Rebellion rebellion)
	{
		this.Init();
		if (this.rebellion != null)
		{
			this.rebellion.DelListener(this);
		}
		this.rebellion = rebellion;
		if (this.rebellion != null)
		{
			this.rebellion.AddListener(this);
		}
		if (this.m_Manpower != null)
		{
			this.m_Manpower.Clear();
			UIArmyManpower manpower = this.m_Manpower;
			object obj;
			if (rebellion == null)
			{
				obj = null;
			}
			else
			{
				Logic.Rebel leader = rebellion.leader;
				if (leader == null)
				{
					obj = null;
				}
				else
				{
					Logic.Army army = leader.army;
					obj = ((army != null) ? army.visuals : null);
				}
			}
			manpower.AddArmy(obj as global::Army);
		}
		if (this.m_Wealth != null)
		{
			Tooltip.Get(this.m_Wealth, true).SetText("#" + global::Defs.Localize("Rebellion.wealth_description", new Vars(rebellion), null, true, true), null, null);
		}
		this.Refresh();
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x001531B8 File Offset: 0x001513B8
	public void Select(bool selected)
	{
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x001531C8 File Offset: 0x001513C8
	public void RefreshRebels()
	{
		this.m_TmpGenerals.Clear();
		this.m_TmpMemebers.Clear();
		for (int i = 0; i < this.rebellion.rebels.Count; i++)
		{
			Logic.Rebel rebel = this.rebellion.rebels[i];
			if (rebel != this.rebellion.leader)
			{
				if (rebel.IsGeneral())
				{
					this.m_TmpGenerals.Add(rebel);
				}
				else
				{
					this.m_TmpMemebers.Add(rebel);
				}
			}
		}
		Vars vars = new Vars();
		vars.Set<string>("variant", "general_compact");
		while (this.m_GeneralsIcons.Count < this.m_TmpGenerals.Count)
		{
			GameObject icon = ObjectIcon.GetIcon("Rebel", vars, this.m_RebellionMembersContainer);
			if (icon == null)
			{
				break;
			}
			UIRebelIcon component = icon.GetComponent<UIRebelIcon>();
			if (component == null)
			{
				break;
			}
			this.m_GeneralsIcons.Add(component);
		}
		vars.Set<string>("variant", "compact");
		while (this.m_MembersIcons.Count < this.m_TmpMemebers.Count)
		{
			GameObject icon2 = ObjectIcon.GetIcon("Rebel", vars, this.m_RebellionMembersContainer);
			if (icon2 == null)
			{
				break;
			}
			UIRebelIcon component2 = icon2.GetComponent<UIRebelIcon>();
			if (component2 == null)
			{
				break;
			}
			this.m_MembersIcons.Add(component2);
		}
		for (int j = 0; j < this.m_GeneralsIcons.Count; j++)
		{
			Logic.Rebel rebel2 = (this.m_TmpGenerals.Count > j) ? this.m_TmpGenerals[j] : null;
			this.m_GeneralsIcons[j].SetObject(rebel2, null);
			this.m_GeneralsIcons[j].gameObject.SetActive(rebel2 != null);
		}
		for (int k = 0; k < this.m_MembersIcons.Count; k++)
		{
			Logic.Rebel rebel3 = (this.m_TmpMemebers.Count > k) ? this.m_TmpMemebers[k] : null;
			this.m_MembersIcons[k].SetObject(rebel3, null);
			this.m_MembersIcons[k].gameObject.SetActive(rebel3 != null);
		}
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x001533F4 File Offset: 0x001515F4
	public void RefreshAllies()
	{
		for (int i = 0; i < this.m_allies.Length; i++)
		{
			if (i >= this.rebellion.allies.Count)
			{
				this.m_allies[i].gameObject.SetActive(false);
			}
			else
			{
				this.m_allies[i].gameObject.SetActive(true);
				this.m_allies[i].SetObject(this.rebellion.allies[i], null);
			}
		}
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x00153470 File Offset: 0x00151670
	private void Refresh()
	{
		Rebellion rebellion = this.rebellion;
		Logic.Rebel rebel = (rebellion != null) ? rebellion.leader : null;
		Rebellion rebellion2 = this.rebellion;
		Logic.Character character;
		if (rebellion2 == null)
		{
			character = null;
		}
		else
		{
			Logic.Rebel leader = rebellion2.leader;
			character = ((leader != null) ? leader.character : null);
		}
		Logic.Character character2 = character;
		if (this.rebellion == null)
		{
			return;
		}
		if (rebel == null || character2 == null || rebel.agenda == null)
		{
			return;
		}
		if (this.m_rebellionPower != null)
		{
			UIText.SetText(this.m_rebellionPower, this.rebellion.GetPower().ToString());
		}
		if (this.m_leaderIcon != null)
		{
			this.m_leaderIcon.ShowArmyBanner(false);
			this.m_leaderIcon.SetObject(character2, null);
		}
		if (this.m_rebellionIcon != null)
		{
			this.m_rebellionIcon.SetObject(this.rebellion, null);
			this.m_rebellionIcon.ShowPowerValue(true);
		}
		this.RefreshRebels();
		this.RefreshAllies();
		if (this.m_leaderName != null)
		{
			UIText.SetTextKey(this.m_leaderName, "Character.title_name", character2, null);
		}
		if (this.m_rebellionAgenda != null)
		{
			UIText.SetText(this.m_rebellionAgenda, this.rebellion.leader.agenda.def.field, "flavor", null, null);
		}
		if (this.m_WealthAmount != null)
		{
			UIText.SetText(this.m_WealthAmount, this.rebellion.GetWealth().ToString());
		}
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x001535D8 File Offset: 0x001517D8
	private void HandleClose(BSGButton btn)
	{
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x001535E5 File Offset: 0x001517E5
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x001535F4 File Offset: 0x001517F4
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x00153603 File Offset: 0x00151803
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnLeaderSelected(UICharacterIcon obj)
	{
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x00153621 File Offset: 0x00151821
	private void HandleOnPointerEvents(Hotspot hotspot, EventTriggerType type, PointerEventData eventData)
	{
		if (type != EventTriggerType.PointerClick)
		{
			return;
		}
		if (this.OnSelect != null)
		{
			this.OnSelect(this, eventData);
		}
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x0015363D File Offset: 0x0015183D
	private void UpdateHighlight()
	{
		if (this.m_SelectedBackground != null)
		{
			this.m_SelectedBackground.SetActive(this.m_Selected);
		}
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x00153660 File Offset: 0x00151860
	public static GameObject GetPrefab()
	{
		DT.Field defField = global::Defs.GetDefField("KingdomStabilityWindow", null);
		if (defField == null)
		{
			return null;
		}
		return global::Defs.GetObj<GameObject>(defField, "rebellion_prefab", null);
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x0015368C File Offset: 0x0015188C
	public static UIKingdomStabilityRebellionRow Create(Rebellion rebellion, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
		UIKingdomStabilityRebellionRow uikingdomStabilityRebellionRow = gameObject.GetComponent<UIKingdomStabilityRebellionRow>();
		if (uikingdomStabilityRebellionRow == null)
		{
			uikingdomStabilityRebellionRow = gameObject.AddComponent<UIKingdomStabilityRebellionRow>();
		}
		uikingdomStabilityRebellionRow.SetObject(rebellion);
		return uikingdomStabilityRebellionRow;
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x001536D8 File Offset: 0x001518D8
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rebellion_leader_started" || message == "leader_changed" || message == "rebels_changed" || message == "rebel_type_changed" || message == "wealth_changed")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x04001A44 RID: 6724
	[UIFieldTarget("id_LeaderIcon")]
	private UICharacterIcon m_leaderIcon;

	// Token: 0x04001A45 RID: 6725
	[UIFieldTarget("id_LeaderName")]
	private TextMeshProUGUI m_leaderName;

	// Token: 0x04001A46 RID: 6726
	[UIFieldTarget("id_RebellionAgenda")]
	private TextMeshProUGUI m_rebellionAgenda;

	// Token: 0x04001A47 RID: 6727
	[UIFieldTarget("id_RebellionPower")]
	private TextMeshProUGUI m_rebellionPower;

	// Token: 0x04001A48 RID: 6728
	[UIFieldTarget("id_RebellionIcon")]
	private UIRebellionIcon m_rebellionIcon;

	// Token: 0x04001A49 RID: 6729
	[UIFieldTarget("id_RebellionMembersContainer")]
	private RectTransform m_RebellionMembersContainer;

	// Token: 0x04001A4A RID: 6730
	[UIFieldTarget("id_allyKingdom", true)]
	private UIKingdomIcon[] m_allies;

	// Token: 0x04001A4B RID: 6731
	[UIFieldTarget("id_Manpower")]
	private UIArmyManpower m_Manpower;

	// Token: 0x04001A4C RID: 6732
	[UIFieldTarget("id_Wealth")]
	private GameObject m_Wealth;

	// Token: 0x04001A4D RID: 6733
	[UIFieldTarget("id_WealthAmount")]
	private TextMeshProUGUI m_WealthAmount;

	// Token: 0x04001A4E RID: 6734
	[UIFieldTarget("id_SelectedBackground")]
	private GameObject m_SelectedBackground;

	// Token: 0x04001A50 RID: 6736
	public Action<UIKingdomStabilityRebellionRow, PointerEventData> OnSelect;

	// Token: 0x04001A51 RID: 6737
	private List<UIRebelIcon> m_MembersIcons = new List<UIRebelIcon>();

	// Token: 0x04001A52 RID: 6738
	private List<UIRebelIcon> m_GeneralsIcons = new List<UIRebelIcon>();

	// Token: 0x04001A53 RID: 6739
	private bool m_Selected;

	// Token: 0x04001A54 RID: 6740
	private bool m_Initialized;

	// Token: 0x04001A55 RID: 6741
	private List<Logic.Rebel> m_TmpGenerals = new List<Logic.Rebel>();

	// Token: 0x04001A56 RID: 6742
	private List<Logic.Rebel> m_TmpMemebers = new List<Logic.Rebel>();
}
