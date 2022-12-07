using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200028B RID: 651
public class UICourtMemberMerchantImportIcon : Hotspot, IListener
{
	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060027D3 RID: 10195 RVA: 0x001585C6 File Offset: 0x001567C6
	// (set) Token: 0x060027D4 RID: 10196 RVA: 0x001585CE File Offset: 0x001567CE
	public Logic.Character Character { get; private set; }

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060027D5 RID: 10197 RVA: 0x001585D7 File Offset: 0x001567D7
	// (set) Token: 0x060027D6 RID: 10198 RVA: 0x001585DF File Offset: 0x001567DF
	public Vars Vars { get; private set; }

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060027D7 RID: 10199 RVA: 0x001585E8 File Offset: 0x001567E8
	public bool Selected
	{
		get
		{
			return this.m_Selected;
		}
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x001585F0 File Offset: 0x001567F0
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x00158609 File Offset: 0x00156809
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		this.CloseSubSelections();
		this.RemoveListeners();
		this.Character = character;
		this.Vars = vars;
		this.AddListeners();
		this.Refresh();
		this.UpdateHighlight();
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x0015863D File Offset: 0x0015683D
	private void Refresh()
	{
		if (this.Character == null)
		{
			return;
		}
		Tooltip.Get(this.m_Empty, true).SetDef("MerchantImportGoodsIconTooltip", new Vars(this.Character));
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x0015866E File Offset: 0x0015686E
	public void Enabled(bool enabled)
	{
		if (this.m_Enabled == enabled)
		{
			return;
		}
		this.m_Enabled = enabled;
		this.UpdateHighlight();
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x00158687 File Offset: 0x00156887
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x001586A0 File Offset: 0x001568A0
	public void ToggleImportActions(bool open)
	{
		if (this.Character != null)
		{
			if (this.importActions != null)
			{
				if (open)
				{
					this.importActions.SetData(this.Character, null);
					return;
				}
				this.CloseSubSelections();
				return;
			}
			else if (open)
			{
				this.importActions = UICourtMemberMerchantImportActions.Create(this.Character, UICourtMemberMerchantImportActions.GetPrefab(), this.GetPopupContainer(), null);
				UICommon.SetAligment(this.importActions.transform as RectTransform, TextAnchor.UpperCenter);
			}
		}
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x00158718 File Offset: 0x00156918
	private RectTransform GetPopupContainer()
	{
		if (this.m_PopupContianer != null)
		{
			return this.m_PopupContianer;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		return ((gameObject != null) ? gameObject.transform : null) as RectTransform;
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x0015876E File Offset: 0x0015696E
	public void SetPopupContainer(RectTransform m_SubSelectionContainer)
	{
		this.m_PopupContianer = m_SubSelectionContainer;
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x00158777 File Offset: 0x00156977
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x00158786 File Offset: 0x00156986
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x00158795 File Offset: 0x00156995
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		base.OnClick(e);
		if (this.OnSelected != null)
		{
			this.OnSelected(this, e);
		}
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x001587BC File Offset: 0x001569BC
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_BorderSelected != null)
		{
			this.m_BorderSelected.SetActive(this.m_Selected);
		}
		if (this.m_Glow != null && this.m_Glow.Length != 0)
		{
			for (int i = 0; i < this.m_Glow.Length; i++)
			{
				this.m_Glow[i].SetActive(this.mouse_in);
			}
		}
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x00158827 File Offset: 0x00156A27
	public void Close()
	{
		this.RemoveListeners();
		this.CloseSubSelections();
		this.m_PopupContianer = null;
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x00158847 File Offset: 0x00156A47
	public void CloseSubSelections()
	{
		if (this.importActions != null)
		{
			this.importActions.Close();
			this.importActions = null;
		}
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x00158869 File Offset: 0x00156A69
	private void AddListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.AddListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.AddListener(this);
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x001588A2 File Offset: 0x00156AA2
	private void RemoveListeners()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return;
		}
		Logic.Kingdom kingdom = character2.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.DelListener(this);
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x001588DB File Offset: 0x00156ADB
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "importing_goods_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x001588F0 File Offset: 0x00156AF0
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x001588F8 File Offset: 0x00156AF8
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberMerchantImportIcon", null);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x00158905 File Offset: 0x00156B05
	public static UICourtMemberRankIcon Possess(Logic.Character character, GameObject gameObject, Vars vars)
	{
		if (gameObject == null)
		{
			return null;
		}
		UICourtMemberRankIcon orAddComponent = gameObject.GetOrAddComponent<UICourtMemberRankIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x00158920 File Offset: 0x00156B20
	public static UICourtMemberMerchantImportIcon Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberMerchantImportIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberMerchantImportIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x04001B0F RID: 6927
	[UIFieldTarget("id_Empty")]
	private GameObject m_Empty;

	// Token: 0x04001B10 RID: 6928
	[UIFieldTarget("id_GroupNormal")]
	private GameObject m_GroupNormal;

	// Token: 0x04001B11 RID: 6929
	[UIFieldTarget("id_BorderSelected")]
	private GameObject m_BorderSelected;

	// Token: 0x04001B12 RID: 6930
	[UIFieldTarget("id_Glow")]
	private GameObject[] m_Glow;

	// Token: 0x04001B15 RID: 6933
	private UICourtMemberMerchantImportActions importActions;

	// Token: 0x04001B16 RID: 6934
	private bool m_Selected;

	// Token: 0x04001B17 RID: 6935
	private bool m_Enabled = true;

	// Token: 0x04001B18 RID: 6936
	private bool m_Initalized;

	// Token: 0x04001B19 RID: 6937
	public Action<UICourtMemberMerchantImportIcon, PointerEventData> OnSelected;

	// Token: 0x04001B1A RID: 6938
	private RectTransform m_PopupContianer;
}
