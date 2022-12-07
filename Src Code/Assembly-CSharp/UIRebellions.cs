using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200027E RID: 638
public class UIRebellions : UIWindow, IListener
{
	// Token: 0x060026FB RID: 9979 RVA: 0x00153FEF File Offset: 0x001521EF
	public override string GetDefId()
	{
		return UIRebellions.def_id;
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060026FC RID: 9980 RVA: 0x00153FF6 File Offset: 0x001521F6
	// (set) Token: 0x060026FD RID: 9981 RVA: 0x00153FFE File Offset: 0x001521FE
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x060026FE RID: 9982 RVA: 0x00154007 File Offset: 0x00152207
	// (set) Token: 0x060026FF RID: 9983 RVA: 0x0015400F File Offset: 0x0015220F
	public Rebellion SelectedRebellion { get; private set; }

	// Token: 0x06002700 RID: 9984 RVA: 0x00154018 File Offset: 0x00152218
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_RebellionlPrototype != null)
		{
			LayoutElement component = this.m_RebellionlPrototype.GetComponent<LayoutElement>();
			this.m_RowHeight = ((component != null) ? component.preferredHeight : this.m_RowHeight);
			this.m_RebellionlPrototype.gameObject.SetActive(false);
		}
		if (this.m_RebellionsContainer != null)
		{
			VerticalLayoutGroup component2 = this.m_RebellionsContainer.GetComponent<VerticalLayoutGroup>();
			this.m_RowSpacing = ((component2 != null) ? component2.spacing : this.m_RowSpacing);
		}
		RectTransform viewport = this.m_BodyScrollRect.viewport;
		this.m_ViewportLayoutElement = ((viewport != null) ? viewport.GetComponent<LayoutElement>() : null);
		if (this.m_ViewportLayoutElement != null)
		{
			this.m_ViewportLayoutElement.minHeight = this.m_RowHeight;
		}
		if (this.m_Button_Close != null)
		{
			this.m_Button_Close.onClick = new BSGButton.OnClick(this.HandleOnCloseButtonClick);
		}
		this.m_Vars = new Vars();
		this.m_Initialized = true;
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x0015411C File Offset: 0x0015231C
	public void SetData(Logic.Kingdom kingdom, Rebellion r)
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
			UIText.SetTextKey(this.m_Caption, "RebellionsWindow.caption", this.m_Vars, null);
		}
		if (this.m_EmptyLabel != null)
		{
			UIText.SetTextKey(this.m_EmptyLabel, "MercenariesWindow.empty", this.m_Vars, null);
		}
		this.RefreshRebellionsList();
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x001541C0 File Offset: 0x001523C0
	private void LateUpdate()
	{
		this.AnalizeSelection();
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x001541C8 File Offset: 0x001523C8
	private void AnalizeSelection()
	{
		WorldUI worldUI = WorldUI.Get();
		Logic.Rebel rebel = UIRebellions.<AnalizeSelection>g__FindRebel|26_0((worldUI != null) ? worldUI.selected_obj : null);
		Rebellion rebellion = (rebel != null) ? rebel.rebellion : null;
		if (rebellion == null)
		{
			this.SelectRebellion(null);
			return;
		}
		if (rebellion != this.SelectedRebellion)
		{
			this.SelectRebellion(rebellion);
		}
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x00154214 File Offset: 0x00152414
	private void RefreshRebellionsList()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_RebellionsContainer == null)
		{
			return;
		}
		if (this.m_RebellionlPrototype == null)
		{
			return;
		}
		int count = this.Data.rebellions.Count;
		while (this.m_Rebbelions.Count < count)
		{
			UIKingdomStabilityRebellionRow uikingdomStabilityRebellionRow = UIKingdomStabilityRebellionRow.Create(null, this.m_RebellionlPrototype, this.m_RebellionsContainer, null);
			if (uikingdomStabilityRebellionRow == null)
			{
				break;
			}
			UIKingdomStabilityRebellionRow uikingdomStabilityRebellionRow2 = uikingdomStabilityRebellionRow;
			uikingdomStabilityRebellionRow2.OnSelect = (Action<UIKingdomStabilityRebellionRow, PointerEventData>)Delegate.Combine(uikingdomStabilityRebellionRow2.OnSelect, new Action<UIKingdomStabilityRebellionRow, PointerEventData>(this.HandleOnRowSelected));
			this.m_Rebbelions.Add(uikingdomStabilityRebellionRow);
		}
		int num = 0;
		for (int i = 0; i < this.m_Rebbelions.Count; i++)
		{
			Rebellion rebellion = (this.Data.rebellions.Count > i) ? this.Data.rebellions[i] : null;
			this.m_Rebbelions[i].SetObject(rebellion);
			this.m_Rebbelions[i].gameObject.SetActive(rebellion != null);
			if (rebellion != null)
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

	// Token: 0x06002705 RID: 9989 RVA: 0x001543A4 File Offset: 0x001525A4
	private void SelectRebellion(Rebellion r)
	{
		if (this.SelectedRebellion == r)
		{
			return;
		}
		this.SelectedRebellion = r;
		for (int i = 0; i < this.m_Rebbelions.Count; i++)
		{
			this.m_Rebbelions[i].Select(this.m_Rebbelions[i].rebellion == this.SelectedRebellion);
		}
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x00154404 File Offset: 0x00152604
	private void HandleOnRowSelected(UIKingdomStabilityRebellionRow r, PointerEventData e)
	{
		object obj;
		if (r == null)
		{
			obj = null;
		}
		else
		{
			Rebellion rebellion = r.rebellion;
			if (rebellion == null)
			{
				obj = null;
			}
			else
			{
				Logic.Army army = rebellion.leader.army;
				obj = ((army != null) ? army.visuals : null);
			}
		}
		global::Army army2 = obj as global::Army;
		if (army2 != null)
		{
			if (WorldUI.Get().GetSelectionObj(army2.gameObject) != army2.gameObject)
			{
				return;
			}
			if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false) || e.clickCount > 1)
			{
				WorldUI.Get().LookAt(army2.transform.position, false);
			}
			WorldUI.Get().SelectObj(army2.gameObject, false, true, true, true);
		}
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnCloseButtonClick(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x001544B4 File Offset: 0x001526B4
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "rebellion_leader_started" || message == "rebellion_zone_joined" || message == "rebellions_changed" || message == "rebel_type_changed" || message == "rebellion_ended" || message == "rebellion_new_leader")
		{
			this.RefreshRebellionsList();
		}
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x00154515 File Offset: 0x00152715
	protected override void OnDestroy()
	{
		if (UIRebellions.current == this)
		{
			UIRebellions.current = null;
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		base.OnDestroy();
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x00154544 File Offset: 0x00152744
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIRebellions.def_id, null);
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x00154554 File Offset: 0x00152754
	public static void ToggleOpen(Logic.Kingdom kingdom, Rebellion rebellion)
	{
		if (kingdom == null)
		{
			if (UIRebellions.current != null)
			{
				UIRebellions.current.Close(false);
				UIRebellions.current = null;
			}
			return;
		}
		if (UIRebellions.current != null)
		{
			UIRebellions uirebellions = UIRebellions.current;
			if (((uirebellions != null) ? uirebellions.SelectedRebellion : null) == rebellion)
			{
				UIRebellions.current.Close(false);
				UIRebellions.current = null;
				return;
			}
			UIRebellions.current.SetData(kingdom, rebellion);
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIRebellions.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIRebellions));
				UIRebellions.current = UIRebellions.Create(kingdom, rebellion, prefab, gameObject.transform as RectTransform);
			}
			return;
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x0015462D File Offset: 0x0015282D
	public static bool IsActive()
	{
		return UIRebellions.current != null;
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x0015463A File Offset: 0x0015283A
	public static UIRebellions Create(Logic.Kingdom kingdom, Rebellion rebellion, GameObject prototype, RectTransform parent)
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
		UIRebellions orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIRebellions>();
		orAddComponent.SetData(kingdom, rebellion);
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x001546B0 File Offset: 0x001528B0
	[CompilerGenerated]
	internal static Logic.Rebel <AnalizeSelection>g__FindRebel|26_0(GameObject so)
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
					Logic.Rebel rebel = (army != null) ? army.rebel : null;
					if (rebel != null)
					{
						return rebel;
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
			return army2.rebel;
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
			return logic.rebel;
		}
	}

	// Token: 0x04001A6C RID: 6764
	private static string def_id = "RebellionsWindow";

	// Token: 0x04001A6D RID: 6765
	private const string PREFRED_CONTAINER = "id_MessageContainer";

	// Token: 0x04001A6E RID: 6766
	[UIFieldTarget("id_RebellionlPrototype")]
	private GameObject m_RebellionlPrototype;

	// Token: 0x04001A6F RID: 6767
	[UIFieldTarget("id_RebellionsContainer")]
	private RectTransform m_RebellionsContainer;

	// Token: 0x04001A70 RID: 6768
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001A71 RID: 6769
	[UIFieldTarget("id_BodyScrollRect")]
	private ScrollRect m_BodyScrollRect;

	// Token: 0x04001A72 RID: 6770
	[UIFieldTarget("id_EmptyLabel")]
	private TextMeshProUGUI m_EmptyLabel;

	// Token: 0x04001A73 RID: 6771
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_Button_Close;

	// Token: 0x04001A75 RID: 6773
	private List<UIKingdomStabilityRebellionRow> m_Rebbelions = new List<UIKingdomStabilityRebellionRow>();

	// Token: 0x04001A76 RID: 6774
	private Vars m_Vars;

	// Token: 0x04001A77 RID: 6775
	private int m_MaxVisibleRows = 4;

	// Token: 0x04001A78 RID: 6776
	private float m_RowHeight = 86f;

	// Token: 0x04001A79 RID: 6777
	private float m_RowSpacing = -2f;

	// Token: 0x04001A7B RID: 6779
	private LayoutElement m_ViewportLayoutElement;

	// Token: 0x04001A7C RID: 6780
	private static UIRebellions current;
}
