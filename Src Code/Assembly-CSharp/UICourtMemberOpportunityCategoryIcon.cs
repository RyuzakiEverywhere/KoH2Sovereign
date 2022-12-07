using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200028E RID: 654
public class UICourtMemberOpportunityCategoryIcon : Hotspot, IListener
{
	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x0600280C RID: 10252 RVA: 0x00159045 File Offset: 0x00157245
	// (set) Token: 0x0600280D RID: 10253 RVA: 0x0015904D File Offset: 0x0015724D
	public Logic.Character Character { get; private set; }

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x0600280E RID: 10254 RVA: 0x00159056 File Offset: 0x00157256
	// (set) Token: 0x0600280F RID: 10255 RVA: 0x0015905E File Offset: 0x0015725E
	public Vars Vars { get; private set; }

	// Token: 0x06002810 RID: 10256 RVA: 0x00159067 File Offset: 0x00157267
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x00159080 File Offset: 0x00157280
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

	// Token: 0x06002812 RID: 10258 RVA: 0x001590B4 File Offset: 0x001572B4
	private void LateUpdate()
	{
		this.UpdateResourceBar();
		this.UpdateUnreadState();
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x001590C4 File Offset: 0x001572C4
	private void UpdateResourceBar()
	{
		if (this.m_ResourceBar == null || this.m_ResourceBarValue == null)
		{
			return;
		}
		if (this.Character == null)
		{
			this.m_ResourceBar.gameObject.SetActive(false);
			return;
		}
		DT.Field field = this.Character.class_def.field.FindChild("opportunities_button.bar_min", null, true, true, true, '.');
		Value value = (field != null) ? field.Value(this.Character, true, true) : Value.Unknown;
		DT.Field field2 = this.Character.class_def.field.FindChild("opportunities_button.bar_max", null, true, true, true, '.');
		Value value2 = (field2 != null) ? field2.Value(this.Character, true, true) : Value.Unknown;
		DT.Field field3 = this.Character.class_def.field.FindChild("opportunities_button.bar_cur", null, true, true, true, '.');
		Value value3 = (field3 != null) ? field3.Value(this.Character, true, true) : Value.Unknown;
		bool flag = !value3.is_unknown;
		flag &= (value2 > value);
		this.m_ResourceBar.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		this.m_ResourceBarValue.fillAmount = (value3.float_val - value.float_val) / (value2.float_val - value.float_val);
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x0015920C File Offset: 0x0015740C
	private void Refresh()
	{
		if (this.Character == null)
		{
			return;
		}
		if (this.m_Icon != null && this.m_Icon.Length != 0)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.Character.class_def.field, "opportunities_button.icon", null);
			for (int i = 0; i < this.m_Icon.Length; i++)
			{
				this.m_Icon[i].overrideSprite = obj;
			}
		}
		Tooltip.Get(this.m_Empty, true).SetDef("CharacterOpportunitiesTooltip", new Vars(this.Character));
		this.UpdateUnreadState();
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x0015929C File Offset: 0x0015749C
	private void UpdateUnreadState()
	{
		if (this.m_Unseen == null)
		{
			return;
		}
		this.m_Unseen.gameObject.SetActive(this.HaveUnseenOpportunities());
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x001592C3 File Offset: 0x001574C3
	public void Enabled(bool enabled)
	{
		if (this.m_Enabled == enabled)
		{
			return;
		}
		this.m_Enabled = enabled;
		this.UpdateHighlight();
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x001592DC File Offset: 0x001574DC
	public void Select(bool selected)
	{
		if (this.m_Selected == selected)
		{
			return;
		}
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x001592F8 File Offset: 0x001574F8
	public void DefaultAction()
	{
		if (this.Character == null)
		{
			return;
		}
		if (this.Character != null)
		{
			if (this.opportunityActions != null)
			{
				if (this.opportunityActions.Character == this.Character)
				{
					this.Select(false);
					this.CloseSubSelections();
					return;
				}
				this.opportunityActions.SetData(this.Character, null);
				this.Select(true);
				return;
			}
			else
			{
				this.opportunityActions = UICourtMemberOpportunityActions.Create(this.Character, UICourtMemberOpportunityActions.GetPrefab(), this.GetPopupContainer(), null);
				UICommon.SetAligment(this.opportunityActions.transform as RectTransform, TextAnchor.UpperCenter);
				this.Select(true);
			}
		}
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x0015939C File Offset: 0x0015759C
	public bool HaveUnseenOpportunities()
	{
		Logic.Character character = this.Character;
		List<Opportunity> list;
		if (character == null)
		{
			list = null;
		}
		else
		{
			Actions actions = character.actions;
			list = ((actions != null) ? actions.opportunities : null);
		}
		List<Opportunity> list2 = list;
		if (list2 != null && list2.Count > 0)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				Opportunity opportunity = list2[i];
				if (opportunity.active && opportunity.action != null && !opportunity.action.is_active && !(opportunity.Validate() != "ok") && !opportunity.seen)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x00159428 File Offset: 0x00157628
	public bool HaveActiveOpportunities()
	{
		return this.Character != null && this.Character.actions != null && this.Character.actions.opportunities != null && this.Character.actions.opportunities.Count > 0;
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x00159478 File Offset: 0x00157678
	public Action GetActiveOpportunityAction()
	{
		if (this.Character == null)
		{
			return null;
		}
		Logic.Character character = this.Character;
		bool flag;
		if (character == null)
		{
			flag = (null != null);
		}
		else
		{
			Actions actions = character.actions;
			if (actions == null)
			{
				flag = (null != null);
			}
			else
			{
				Action current = actions.current;
				if (current == null)
				{
					flag = (null != null);
				}
				else
				{
					Action.Def def = current.def;
					flag = (((def != null) ? def.opportunity : null) != null);
				}
			}
		}
		if (!flag)
		{
			return null;
		}
		Logic.Character character2 = this.Character;
		if (character2 == null)
		{
			return null;
		}
		Actions actions2 = character2.actions;
		if (actions2 == null)
		{
			return null;
		}
		return actions2.current;
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x001594E8 File Offset: 0x001576E8
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

	// Token: 0x0600281D RID: 10269 RVA: 0x0015953E File Offset: 0x0015773E
	public void SetPopupContainer(RectTransform m_SubSelectionContainer)
	{
		this.m_PopupContianer = m_SubSelectionContainer;
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x00159547 File Offset: 0x00157747
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x00159556 File Offset: 0x00157756
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x00159565 File Offset: 0x00157765
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		base.OnClick(e);
		if (this.OnSelected != null && !this.m_Selected)
		{
			this.OnSelected(this, e);
		}
		this.DefaultAction();
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x0015959C File Offset: 0x0015779C
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_GroupNormal != null)
		{
			this.m_GroupNormal.gameObject.SetActive(!this.m_Selected);
		}
		if (this.m_GroupSelected != null)
		{
			this.m_GroupSelected.gameObject.SetActive(this.m_Selected);
		}
		if (this.m_Glow != null && this.m_Glow.Length != 0)
		{
			for (int i = 0; i < this.m_Glow.Length; i++)
			{
				this.m_Glow[i].SetActive(this.mouse_in);
			}
		}
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x00159633 File Offset: 0x00157833
	public void Close()
	{
		this.RemoveListeners();
		this.CloseSubSelections();
		this.m_PopupContianer = null;
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x00159653 File Offset: 0x00157853
	public void CloseSubSelections()
	{
		if (this.opportunityActions != null)
		{
			this.opportunityActions.Close();
			this.opportunityActions = null;
		}
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x00159675 File Offset: 0x00157875
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

	// Token: 0x06002825 RID: 10277 RVA: 0x001596AE File Offset: 0x001578AE
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

	// Token: 0x06002826 RID: 10278 RVA: 0x001596E7 File Offset: 0x001578E7
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "new_opportunity" || message == "opportunity_lost" || message == "opportunities_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x00159716 File Offset: 0x00157916
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x0015971E File Offset: 0x0015791E
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberOpportunityCategory", null);
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x00158905 File Offset: 0x00156B05
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

	// Token: 0x0600282A RID: 10282 RVA: 0x0015972B File Offset: 0x0015792B
	public static UICourtMemberOpportunityCategoryIcon Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberOpportunityCategoryIcon orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberOpportunityCategoryIcon>();
		orAddComponent.SetData(character, vars);
		return orAddComponent;
	}

	// Token: 0x04001B2D RID: 6957
	[UIFieldTarget("id_Empty")]
	private GameObject m_Empty;

	// Token: 0x04001B2E RID: 6958
	[UIFieldTarget("id_GroupNormal")]
	private GameObject m_GroupNormal;

	// Token: 0x04001B2F RID: 6959
	[UIFieldTarget("id_GroupSelected")]
	private GameObject m_GroupSelected;

	// Token: 0x04001B30 RID: 6960
	[UIFieldTarget("id_Icon")]
	private Image[] m_Icon;

	// Token: 0x04001B31 RID: 6961
	[UIFieldTarget("id_ResourceBar")]
	private GameObject m_ResourceBar;

	// Token: 0x04001B32 RID: 6962
	[UIFieldTarget("id_ResourceBarValue")]
	private Image m_ResourceBarValue;

	// Token: 0x04001B33 RID: 6963
	[UIFieldTarget("id_Unseen")]
	private GameObject m_Unseen;

	// Token: 0x04001B34 RID: 6964
	[UIFieldTarget("id_Glow")]
	private GameObject[] m_Glow;

	// Token: 0x04001B37 RID: 6967
	private UICourtMemberOpportunityActions opportunityActions;

	// Token: 0x04001B38 RID: 6968
	private bool m_Selected;

	// Token: 0x04001B39 RID: 6969
	private bool m_Enabled = true;

	// Token: 0x04001B3A RID: 6970
	private bool m_Initalized;

	// Token: 0x04001B3B RID: 6971
	public Action<UICourtMemberOpportunityCategoryIcon, PointerEventData> OnSelected;

	// Token: 0x04001B3C RID: 6972
	private RectTransform m_PopupContianer;
}
