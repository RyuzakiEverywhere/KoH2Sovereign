using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000230 RID: 560
public class UIHiredMercenaries : UIWindow, IListener
{
	// Token: 0x060021D5 RID: 8661 RVA: 0x001321D3 File Offset: 0x001303D3
	public override string GetDefId()
	{
		return UIHiredMercenaries.def_id;
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060021D6 RID: 8662 RVA: 0x001321DA File Offset: 0x001303DA
	// (set) Token: 0x060021D7 RID: 8663 RVA: 0x001321E2 File Offset: 0x001303E2
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060021D8 RID: 8664 RVA: 0x001321EC File Offset: 0x001303EC
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_MercenaryPrototype != null)
		{
			LayoutElement component = this.m_MercenaryPrototype.GetComponent<LayoutElement>();
			this.m_RowHeight = ((component != null) ? component.preferredHeight : this.m_RowHeight);
			this.m_MercenaryPrototype.gameObject.SetActive(false);
		}
		if (this.m_MercenariesContainer != null)
		{
			VerticalLayoutGroup component2 = this.m_MercenariesContainer.GetComponent<VerticalLayoutGroup>();
			this.m_RowSpacing = ((component2 != null) ? component2.spacing : this.m_RowSpacing);
		}
		if (this.m_Button_Close != null)
		{
			this.m_Button_Close.onClick = new BSGButton.OnClick(this.HandleOnCloseButtonClick);
		}
		RectTransform viewport = this.m_BodyScrollRect.viewport;
		this.m_ViewportLayoutElement = ((viewport != null) ? viewport.GetComponent<LayoutElement>() : null);
		if (this.m_ViewportLayoutElement != null)
		{
			this.m_ViewportLayoutElement.minHeight = this.m_RowHeight;
		}
		this.m_Vars = new Vars();
		this.m_Initialized = true;
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x001322F0 File Offset: 0x001304F0
	public void SetData(Logic.Kingdom kingdom)
	{
		this.Init();
		Logic.Kingdom data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		this.Data = kingdom;
		Logic.Kingdom data2 = this.Data;
		if (data2 != null)
		{
			data2.AddListener(this);
		}
		this.m_Vars.obj = this.Data;
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "MercenariesWindow.caption", this.m_Vars, null);
		}
		if (this.m_EmptyLabel != null)
		{
			UIText.SetTextKey(this.m_EmptyLabel, "MercenariesWindow.empty", this.m_Vars, null);
		}
		this.RefreshMercList();
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x00132394 File Offset: 0x00130594
	private void LateUpdate()
	{
		this.AnalizeSelection();
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x0013239C File Offset: 0x0013059C
	private void AnalizeSelection()
	{
		WorldUI worldUI = WorldUI.Get();
		Mercenary mercenary = UIHiredMercenaries.<AnalizeSelection>g__FindMerc|24_0((worldUI != null) ? worldUI.selected_obj : null);
		if (mercenary == null)
		{
			this.SelectMercenary(null);
			return;
		}
		if (mercenary != this.m_SelectedMercenary)
		{
			this.SelectMercenary(mercenary);
		}
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x001323DC File Offset: 0x001305DC
	private void RefreshMercList()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_MercenariesContainer == null)
		{
			return;
		}
		if (this.m_MercenaryPrototype == null)
		{
			return;
		}
		Mercenary.GetLoyalMercenaries(this.Data, this.m_TmpMercList);
		int count = this.m_TmpMercList.Count;
		while (this.m_Mercenaries.Count < count)
		{
			UIHiredMercenaries.MercenaryRow mercenaryRow = UIHiredMercenaries.MercenaryRow.Create(null, this.m_MercenaryPrototype, this.m_MercenariesContainer);
			if (mercenaryRow == null)
			{
				break;
			}
			UIHiredMercenaries.MercenaryRow mercenaryRow2 = mercenaryRow;
			mercenaryRow2.OnSelect = (Action<UIHiredMercenaries.MercenaryRow, PointerEventData>)Delegate.Combine(mercenaryRow2.OnSelect, new Action<UIHiredMercenaries.MercenaryRow, PointerEventData>(this.HandleOnRowSelected));
			this.m_Mercenaries.Add(mercenaryRow);
		}
		int num = 0;
		for (int i = 0; i < this.m_Mercenaries.Count; i++)
		{
			Mercenary mercenary = (this.m_TmpMercList.Count > i) ? this.m_TmpMercList[i] : null;
			this.m_Mercenaries[i].SetData(mercenary);
			this.m_Mercenaries[i].gameObject.SetActive(mercenary != null);
			if (mercenary != null)
			{
				num++;
			}
		}
		if (this.m_BodyScrollRect != null)
		{
			num = Mathf.Min(num, this.m_MaxVisibleRows);
			if (this.m_ViewportLayoutElement != null)
			{
				this.m_ViewportLayoutElement.preferredHeight = (float)num * this.m_RowHeight + ((num > 1) ? (this.m_RowSpacing * (float)(num - 1)) : 0f);
			}
		}
		if (this.m_EmptyLabel != null)
		{
			this.m_EmptyLabel.gameObject.SetActive(num == 0);
		}
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x0013256C File Offset: 0x0013076C
	private void SelectMercenary(Mercenary m)
	{
		if (this.m_SelectedMercenary == m)
		{
			return;
		}
		this.m_SelectedMercenary = m;
		for (int i = 0; i < this.m_Mercenaries.Count; i++)
		{
			this.m_Mercenaries[i].Select(this.m_Mercenaries[i].Data == this.m_SelectedMercenary);
		}
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x001325CC File Offset: 0x001307CC
	private void HandleOnRowSelected(UIHiredMercenaries.MercenaryRow r, PointerEventData e)
	{
		object obj;
		if (r == null)
		{
			obj = null;
		}
		else
		{
			Mercenary data = r.Data;
			if (data == null)
			{
				obj = null;
			}
			else
			{
				Logic.Army army = data.army;
				obj = ((army != null) ? army.visuals : null);
			}
		}
		global::Army army2 = obj as global::Army;
		if (army2 != null)
		{
			if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false) || e.clickCount > 1)
			{
				WorldUI.Get().LookAt(army2.transform.position, false);
			}
			WorldUI.Get().SelectObj(army2.gameObject, false, true, true, true);
		}
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButtonClick(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x00132659 File Offset: 0x00130859
	protected override void OnDestroy()
	{
		if (UIHiredMercenaries.current == this)
		{
			UIHiredMercenaries.current = null;
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x00132688 File Offset: 0x00130888
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIHiredMercenaries.def_id, null);
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x00132698 File Offset: 0x00130898
	public static void ToggleOpen(Logic.Kingdom kingdom, Mercenary mercenary)
	{
		if (kingdom == null)
		{
			if (UIHiredMercenaries.current != null)
			{
				UIHiredMercenaries.current.Close(false);
				UIHiredMercenaries.current = null;
			}
			return;
		}
		if (UIHiredMercenaries.current != null)
		{
			UIHiredMercenaries uihiredMercenaries = UIHiredMercenaries.current;
			if (((uihiredMercenaries != null) ? uihiredMercenaries.Data : null) == kingdom)
			{
				UIHiredMercenaries.current.Close(false);
				UIHiredMercenaries.current = null;
				return;
			}
			UIHiredMercenaries.current.SetData(kingdom);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIHiredMercenaries.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIHiredMercenaries));
				UIHiredMercenaries.current = UIHiredMercenaries.Create(kingdom, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x0013276F File Offset: 0x0013096F
	public static bool IsActive()
	{
		return UIHiredMercenaries.current != null;
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x0013277C File Offset: 0x0013097C
	public static void UpdateMercList()
	{
		UIHiredMercenaries uihiredMercenaries = UIHiredMercenaries.current;
		if (uihiredMercenaries == null)
		{
			return;
		}
		uihiredMercenaries.RefreshMercList();
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x0013278D File Offset: 0x0013098D
	public static UIHiredMercenaries Create(Logic.Kingdom kingdom, GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		if (kingdom == null)
		{
			return null;
		}
		UIHiredMercenaries orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIHiredMercenaries>();
		orAddComponent.SetData(kingdom);
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x0013280C File Offset: 0x00130A0C
	[CompilerGenerated]
	internal static Mercenary <AnalizeSelection>g__FindMerc|24_0(GameObject so)
	{
		if (so == null)
		{
			return null;
		}
		global::Battle battle = (so != null) ? so.GetComponent<global::Battle>() : null;
		if (battle != null)
		{
			for (int i = 0; i < 2; i++)
			{
				List<Logic.Army> armies = battle.logic.GetArmies(i);
				for (int j = 0; j < armies.Count; j++)
				{
					Logic.Army army = armies[j];
					Mercenary mercenary = (army != null) ? army.mercenary : null;
					if (mercenary != null)
					{
						return mercenary;
					}
				}
			}
		}
		global::Settlement component = so.GetComponent<global::Settlement>();
		if (component != null)
		{
			Castle castle = component.logic as Castle;
			if (castle == null)
			{
				return null;
			}
			Logic.Army army2 = castle.army;
			if (army2 == null)
			{
				return null;
			}
			return army2.mercenary;
		}
		else
		{
			global::Army army3 = (so != null) ? so.GetComponent<global::Army>() : null;
			if (!(army3 != null))
			{
				return null;
			}
			if (army3 == null)
			{
				return null;
			}
			Logic.Army logic = army3.logic;
			if (logic == null)
			{
				return null;
			}
			return logic.mercenary;
		}
	}

	// Token: 0x040016B1 RID: 5809
	private static string def_id = "HiredMercenaries";

	// Token: 0x040016B2 RID: 5810
	private const string PREFRED_CONTAINER = "id_MessageContainer";

	// Token: 0x040016B3 RID: 5811
	[UIFieldTarget("id_MercenaryPrototype")]
	private GameObject m_MercenaryPrototype;

	// Token: 0x040016B4 RID: 5812
	[UIFieldTarget("id_MercenariesContainer")]
	private RectTransform m_MercenariesContainer;

	// Token: 0x040016B5 RID: 5813
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040016B6 RID: 5814
	[UIFieldTarget("id_BodyScrollRect")]
	private ScrollRect m_BodyScrollRect;

	// Token: 0x040016B7 RID: 5815
	[UIFieldTarget("id_EmptyLabel")]
	private TextMeshProUGUI m_EmptyLabel;

	// Token: 0x040016B8 RID: 5816
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_Button_Close;

	// Token: 0x040016BA RID: 5818
	private List<UIHiredMercenaries.MercenaryRow> m_Mercenaries = new List<UIHiredMercenaries.MercenaryRow>();

	// Token: 0x040016BB RID: 5819
	private Vars m_Vars;

	// Token: 0x040016BC RID: 5820
	private int m_MaxVisibleRows = 4;

	// Token: 0x040016BD RID: 5821
	private float m_RowHeight = 86f;

	// Token: 0x040016BE RID: 5822
	private float m_RowSpacing = -2f;

	// Token: 0x040016BF RID: 5823
	private List<Mercenary> m_TmpMercList = new List<Mercenary>(0);

	// Token: 0x040016C0 RID: 5824
	private Mercenary m_SelectedMercenary;

	// Token: 0x040016C1 RID: 5825
	private LayoutElement m_ViewportLayoutElement;

	// Token: 0x040016C2 RID: 5826
	private static UIHiredMercenaries current;

	// Token: 0x02000784 RID: 1924
	internal class MercenaryRow : Hotspot, IListener
	{
		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06004C64 RID: 19556 RVA: 0x0022996C File Offset: 0x00227B6C
		// (set) Token: 0x06004C65 RID: 19557 RVA: 0x00229974 File Offset: 0x00227B74
		public Mercenary Data { get; private set; }

		// Token: 0x06004C66 RID: 19558 RVA: 0x0022997D File Offset: 0x00227B7D
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Leader != null)
			{
				this.m_Leader.ShowArmyBanner(false);
			}
			this.vars = new Vars();
			this.m_Initialzied = true;
		}

		// Token: 0x06004C67 RID: 19559 RVA: 0x002299BC File Offset: 0x00227BBC
		public void SetData(Mercenary merc)
		{
			this.Init();
			Mercenary data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			Mercenary data2 = this.Data;
			if (data2 != null)
			{
				Logic.Army army = data2.army;
				if (army != null)
				{
					army.DelListener(this);
				}
			}
			this.Data = merc;
			Mercenary data3 = this.Data;
			if (data3 != null)
			{
				data3.AddListener(this);
			}
			Mercenary data4 = this.Data;
			if (data4 != null)
			{
				Logic.Army army2 = data4.army;
				if (army2 != null)
				{
					army2.AddListener(this);
				}
			}
			this.vars.obj = this.Data;
			if (this.m_Manpower != null)
			{
				this.m_Manpower.Clear();
				UIArmyManpower manpower = this.m_Manpower;
				object obj;
				if (merc == null)
				{
					obj = null;
				}
				else
				{
					Logic.Army army3 = merc.army;
					obj = ((army3 != null) ? army3.visuals : null);
				}
				manpower.AddArmy(obj as global::Army);
			}
			this.Refresh();
		}

		// Token: 0x06004C68 RID: 19560 RVA: 0x00229A91 File Offset: 0x00227C91
		private void Update()
		{
			if (this.m_NextRefresh < UnityEngine.Time.unscaledTime)
			{
				this.UpdateDescription();
				this.m_NextRefresh = UnityEngine.Time.unscaledTime + this.m_RefreshInterval;
			}
		}

		// Token: 0x06004C69 RID: 19561 RVA: 0x00229AB8 File Offset: 0x00227CB8
		public void Select(bool selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlight();
		}

		// Token: 0x06004C6A RID: 19562 RVA: 0x00229AC8 File Offset: 0x00227CC8
		private void Refresh()
		{
			Mercenary data = this.Data;
			Logic.Character character = (data != null) ? data.army.leader : null;
			if (character != null)
			{
				UICharacterIcon leader = this.m_Leader;
				if (leader != null)
				{
					leader.SetObject(character, null);
				}
			}
			GameObject leaderless = this.m_Leaderless;
			if (leaderless != null)
			{
				leaderless.gameObject.SetActive(character == null);
			}
			UICharacterIcon leader2 = this.m_Leader;
			if (leader2 != null)
			{
				leader2.gameObject.SetActive(character != null);
			}
			if (this.m_Name != null)
			{
				if (character != null)
				{
					UIText.SetTextKey(this.m_Name, "Character.title_name", new Vars(character), null);
				}
				else
				{
					UIText.SetTextKey(this.m_Name, "MercenariesWindow.leaderless", this.vars, null);
				}
			}
			this.UpdateDescription();
			this.UpdateUpkeep();
			this.UpdateHighlight();
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x00229B90 File Offset: 0x00227D90
		private void UpdateUpkeep()
		{
			if (this.m_Upkeep == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Vars vars = new Vars();
			vars.Set<Resource>("upkeep", this.Data.army.GetUpkeep());
			UIText.SetTextKey(this.m_Upkeep, "Mercenary.upkeep_text", vars, null);
		}

		// Token: 0x06004C6C RID: 19564 RVA: 0x00229BE8 File Offset: 0x00227DE8
		private void UpdateDescription()
		{
			if (this.m_Description == null)
			{
				return;
			}
			if (this.Data == null)
			{
				return;
			}
			Logic.Kingdom kingdom = this.Data.army.GetKingdom();
			string key;
			if (this.Data.ValidForHireAsUnit())
			{
				key = ((this.Data.former_owner_id == BaseUI.LogicKingdom().id) ? "Mercenary.description_own_army" : "Mercenary.description");
			}
			else if (kingdom == BaseUI.LogicKingdom())
			{
				key = "Mercenary.description_current_action";
			}
			else
			{
				key = "Mercenary.description";
			}
			UIText.SetTextKey(this.m_Description, key, this.vars, null);
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x00229C7A File Offset: 0x00227E7A
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x00229C89 File Offset: 0x00227E89
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x00229C98 File Offset: 0x00227E98
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			if (BaseUI.IgnoreLeftClick())
			{
				return;
			}
			if (this.OnSelect != null)
			{
				this.OnSelect(this, e);
			}
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x00229CBE File Offset: 0x00227EBE
		public override void OnDoubleClick(PointerEventData e)
		{
			base.OnDoubleClick(e);
			if (BaseUI.IgnoreLeftClick())
			{
				return;
			}
			if (this.OnSelect != null)
			{
				this.OnSelect(this, e);
			}
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x00229CE4 File Offset: 0x00227EE4
		private void UpdateHighlight()
		{
			if (this.m_SelectedBackground != null)
			{
				this.m_SelectedBackground.SetActive(this.m_Selected);
			}
		}

		// Token: 0x06004C72 RID: 19570 RVA: 0x00229D05 File Offset: 0x00227F05
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "command_changed" || message == "battle_changed" || message == "became_regular")
			{
				this.UpdateDescription();
			}
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x00229D34 File Offset: 0x00227F34
		private void OnDestroy()
		{
			Mercenary data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			this.Data = null;
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x00229D4F File Offset: 0x00227F4F
		public static UIHiredMercenaries.MercenaryRow Create(Mercenary merc, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			UIHiredMercenaries.MercenaryRow orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIHiredMercenaries.MercenaryRow>();
			orAddComponent.gameObject.SetActive(true);
			orAddComponent.SetData(merc);
			return orAddComponent;
		}

		// Token: 0x04003AE0 RID: 15072
		[UIFieldTarget("id_Leader")]
		private UICharacterIcon m_Leader;

		// Token: 0x04003AE1 RID: 15073
		[UIFieldTarget("id_Leaderless")]
		private GameObject m_Leaderless;

		// Token: 0x04003AE2 RID: 15074
		[UIFieldTarget("id_Name")]
		private TextMeshProUGUI m_Name;

		// Token: 0x04003AE3 RID: 15075
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x04003AE4 RID: 15076
		[UIFieldTarget("id_Manpower")]
		private UIArmyManpower m_Manpower;

		// Token: 0x04003AE5 RID: 15077
		[UIFieldTarget("id_Upkeep")]
		private TextMeshProUGUI m_Upkeep;

		// Token: 0x04003AE6 RID: 15078
		[UIFieldTarget("id_SelectedBackground")]
		private GameObject m_SelectedBackground;

		// Token: 0x04003AE8 RID: 15080
		public Action<UIHiredMercenaries.MercenaryRow, PointerEventData> OnSelect;

		// Token: 0x04003AE9 RID: 15081
		private Vars vars;

		// Token: 0x04003AEA RID: 15082
		private bool m_Selected;

		// Token: 0x04003AEB RID: 15083
		private bool m_Initialzied;

		// Token: 0x04003AEC RID: 15084
		private float m_NextRefresh;

		// Token: 0x04003AED RID: 15085
		private float m_RefreshInterval = 0.5f;
	}
}
