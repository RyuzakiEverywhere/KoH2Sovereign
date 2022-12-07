using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000287 RID: 647
public class UICourtMemberActions : UIWindow, IListener
{
	// Token: 0x06002772 RID: 10098 RVA: 0x00156531 File Offset: 0x00154731
	public override string GetDefId()
	{
		return UICourtMemberActions.def_id;
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06002773 RID: 10099 RVA: 0x00156538 File Offset: 0x00154738
	// (set) Token: 0x06002774 RID: 10100 RVA: 0x00156540 File Offset: 0x00154740
	public Logic.Character Data { get; private set; }

	// Token: 0x06002775 RID: 10101 RVA: 0x00156549 File Offset: 0x00154749
	protected override void Awake()
	{
		base.Awake();
		this.Init();
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x00156558 File Offset: 0x00154758
	private void Init()
	{
		if (this.m_initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_StatusActionsContainer != null)
		{
			this.m_StatusActionsContainer.gameObject.SetActive(false);
			this.m_StatusActionsContainer.gameObject.GetOrAddComponent<TooltipBlocker>();
		}
		if (this.m_OpportunityActionsContianer != null)
		{
			this.opportunityActions = this.m_OpportunityActionsContianer.GetOrAddComponent<UICourtMemberActions.OpportunityActions>();
		}
		this.m_initialzed = true;
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x001565CB File Offset: 0x001547CB
	public override void Hide(bool silent = false)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		base.gameObject.SetActive(false);
		this.DeselectAll();
		base.Hide(silent);
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x001565F4 File Offset: 0x001547F4
	public override void Show()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.Show();
		base.gameObject.SetActive(true);
		this.LateUpdate();
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000EDE65 File Offset: 0x000EC065
	public override bool OnBackInputAction()
	{
		this.Hide(false);
		return true;
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x0015661C File Offset: 0x0015481C
	private void LateUpdate()
	{
		this.Init();
		if (!this.m_InvalidateSelection && !this.m_InvalidateStatuses)
		{
			this.UpdateSelection();
			this.UpdateSpacings();
			return;
		}
		if (this.Data == null)
		{
			this.Hide(false);
			return;
		}
		if (this.m_InvalidateCurrentAction)
		{
			this.DeselectAll();
			if (this.m_CurrentActionWindow)
			{
				UIActions currentActionWindow = this.m_CurrentActionWindow;
				currentActionWindow.OnActionIconSeleced = (Action)Delegate.Remove(currentActionWindow.OnActionIconSeleced, new Action(this.HandleOnActionIconSelected));
			}
			this.m_CurrentActionWindow = UIActions.Possess(this.Data, this.m_ActionsContainer, null);
			if (this.m_CurrentActionWindow)
			{
				UIActions currentActionWindow2 = this.m_CurrentActionWindow;
				currentActionWindow2.OnActionIconSeleced = (Action)Delegate.Combine(currentActionWindow2.OnActionIconSeleced, new Action(this.HandleOnActionIconSelected));
			}
			this.m_InvalidateCurrentAction = false;
		}
		this.m_InvalidateSelection |= !this.HasActiveSubActions(this.m_SelectedCategoryIcon);
		if (this.m_InvalidateSelection)
		{
			this.DeselectAll();
			if (this.m_CurrentActionWindow)
			{
				UIActions currentActionWindow3 = this.m_CurrentActionWindow;
				currentActionWindow3.OnActionIconSeleced = (Action)Delegate.Remove(currentActionWindow3.OnActionIconSeleced, new Action(this.HandleOnActionIconSelected));
			}
			this.m_CurrentActionWindow = UIActions.Possess(this.Data, this.m_ActionsContainer, null);
			if (this.m_CurrentActionWindow)
			{
				UIActions currentActionWindow4 = this.m_CurrentActionWindow;
				currentActionWindow4.OnActionIconSeleced = (Action)Delegate.Combine(currentActionWindow4.OnActionIconSeleced, new Action(this.HandleOnActionIconSelected));
			}
			if (this.m_CurAncor != null)
			{
				WorldUI worldUI = WorldUI.Get();
				Canvas canvas;
				if (worldUI == null)
				{
					canvas = null;
				}
				else
				{
					Transform tCanvas = worldUI.tCanvas;
					canvas = ((tCanvas != null) ? tCanvas.GetComponent<Canvas>() : null);
				}
				Canvas canvas2 = canvas;
				float num = (canvas2 != null) ? canvas2.scaleFactor : 1f;
				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, this.m_CurAncor.transform.position);
				Vector3 vector;
				RectTransformUtility.ScreenPointToWorldPointInRectangle(base.transform as RectTransform, screenPoint, null, out vector);
				vector.Set((float)((int)vector.x), (float)((int)vector.y) + this.yOffset * num, 0f);
				base.transform.position = vector;
			}
			this.m_SelectedCategoryIcon = null;
		}
		this.PopulateStatuses(this.Data);
		this.PopulateCustomActions(this.Data);
		this.PopulateOpportunities(this.Data);
		this.PopulateMerchantImportActions(this.Data);
		this.PolulateExileButton();
		this.PolulateRecallButton();
		this.UpdateSelection();
		this.UpdateSpacings();
		this.m_InvalidateSelection = false;
		this.m_InvalidateStatuses = false;
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x001568A4 File Offset: 0x00154AA4
	public void UpdateViusals(Logic.Character character, RectTransform anchor, bool force_refresh = false)
	{
		this.Init();
		if (this.Data != character || force_refresh)
		{
			Logic.Character data = this.Data;
			if (data != null)
			{
				data.DelListener(this);
			}
			this.Data = character;
			Logic.Character data2 = this.Data;
			if (data2 != null)
			{
				data2.AddListener(this);
			}
			this.m_InvalidateSelection = true;
			this.m_SelectedCategoryIcon = null;
		}
		this.m_CurAncor = anchor;
		if (this.Data != null)
		{
			this.Show();
		}
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x00156915 File Offset: 0x00154B15
	private void SelectCathegoryIcon(MonoBehaviour icon)
	{
		if (this.m_SelectedCategoryIcon == icon)
		{
			this.m_SelectedCategoryIcon = null;
		}
		else
		{
			this.m_SelectedCategoryIcon = icon;
		}
		this.UpdateSelection();
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x0015693C File Offset: 0x00154B3C
	private void UpdateSelection()
	{
		bool active = !(this.m_SelectedCategoryIcon != null) && !this.CalcActionBarIsEmpty();
		if (this.m_ActionsAndOpportunities != null)
		{
			this.m_ActionsAndOpportunities.gameObject.SetActive(active);
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x00156984 File Offset: 0x00154B84
	private bool HasActiveSubActions(MonoBehaviour icon)
	{
		if (icon == null)
		{
			return false;
		}
		if (!(icon is UIStatusIcon))
		{
			return true;
		}
		UIStatusIcon uistatusIcon = icon as UIStatusIcon;
		if (uistatusIcon.Data == null)
		{
			return false;
		}
		UICourtMemberActions.tempDefList.Clear();
		uistatusIcon.Data.GetActions(UICourtMemberActions.tempDefList);
		if (UICourtMemberActions.tempDefList.Count > 0)
		{
			bool @bool = uistatusIcon.Data.def.field.GetBool("show_singleton_action", uistatusIcon.Data, false, true, true, true, '.');
			return UICourtMemberActions.tempDefList.Count != 1 || @bool;
		}
		return false;
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x00156A1C File Offset: 0x00154C1C
	private bool CalcActionBarIsEmpty()
	{
		return (!(this.m_CurrentActionWindow != null) || !this.m_CurrentActionWindow.HasActiveActions()) && !this.opportunityActions.HasActiveOpportunities() && (!(this.m_RecallAction != null) || !this.m_RecallAction.gameObject.activeSelf);
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x00156A78 File Offset: 0x00154C78
	private void UpdateSpacings()
	{
		RectTransform customActionsContainer = this.m_CustomActionsContainer;
		if (customActionsContainer != null)
		{
			customActionsContainer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_CustomActionsContainer.gameObject));
		}
		RectTransform statusesContainer = this.m_statusesContainer;
		if (statusesContainer != null)
		{
			statusesContainer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_statusesContainer.gameObject));
		}
		RectTransform merchantImportActionsContainer = this.m_MerchantImportActionsContainer;
		if (merchantImportActionsContainer != null)
		{
			merchantImportActionsContainer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_MerchantImportActionsContainer.gameObject));
		}
		RectTransform bribeStatusContianer = this.m_BribeStatusContianer;
		if (bribeStatusContianer != null)
		{
			bribeStatusContianer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_BribeStatusContianer.gameObject));
		}
		RectTransform mainStatusContainer = this.m_MainStatusContainer;
		if (mainStatusContainer != null)
		{
			mainStatusContainer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_MainStatusContainer.gameObject));
		}
		RectTransform opportunityActionsContianer = this.m_OpportunityActionsContianer;
		if (opportunityActionsContianer != null)
		{
			opportunityActionsContianer.gameObject.SetActive(UICommon.HasActiveChildren(this.m_OpportunityActionsContianer.gameObject));
		}
		GameObject actionsContainer = this.m_ActionsContainer;
		if (actionsContainer == null)
		{
			return;
		}
		actionsContainer.gameObject.SetActive(this.m_CurrentActionWindow != null && this.m_CurrentActionWindow.HasActiveActions());
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x00156B9C File Offset: 0x00154D9C
	private void PopulateCustomActions(Logic.Character character)
	{
		if (this.m_CustomActionsContainer == null)
		{
			return;
		}
		if (this.m_RankActionIcon == null)
		{
			this.m_RankActionIcon = UICourtMemberRankIcon.Create(this.Data, UICourtMemberRankIcon.GetPrefab(), this.m_CustomActionsContainer, null);
			this.m_RankActionIcon.SetPopupContainer(this.m_SubSelectionContainer);
			UICourtMemberRankIcon rankActionIcon = this.m_RankActionIcon;
			rankActionIcon.OnSelected = (Action<UICourtMemberRankIcon, PointerEventData>)Delegate.Combine(rankActionIcon.OnSelected, new Action<UICourtMemberRankIcon, PointerEventData>(this.HandleOnRankActionSelected));
		}
		if (this.m_RankActionIcon != null)
		{
			this.m_RankActionIcon.SetData(character, null);
			this.m_RankActionIcon.gameObject.SetActive(this.Data.IsAlive() && !this.Data.IsRebel());
		}
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x00156C64 File Offset: 0x00154E64
	private void PopulateOpportunities(Logic.Character character)
	{
		if (this.opportunityActions == null)
		{
			return;
		}
		this.opportunityActions.SetData(character, null);
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x00156C84 File Offset: 0x00154E84
	private void PopulateMerchantImportActions(Logic.Character character)
	{
		if (this.m_MerchantImportActionsContainer == null)
		{
			return;
		}
		if (character == null)
		{
			return;
		}
		if (!character.IsMerchant() || character.IsRebel() || character.mission_kingdom == null || character.cur_action is RecallAction)
		{
			UICourtMemberMerchantImportIcon merchantImportAction = this.m_MerchantImportAction;
			if (merchantImportAction == null)
			{
				return;
			}
			merchantImportAction.gameObject.SetActive(false);
			return;
		}
		else
		{
			UICourtMemberMerchantImportIcon merchantImportAction2 = this.m_MerchantImportAction;
			if (merchantImportAction2 != null)
			{
				merchantImportAction2.gameObject.SetActive(true);
			}
			if (this.m_MerchantImportAction == null)
			{
				this.m_MerchantImportAction = UICourtMemberMerchantImportIcon.Create(this.Data, UICourtMemberMerchantImportIcon.GetPrefab(), this.m_MerchantImportActionsContainer, null);
				this.m_MerchantImportAction.SetPopupContainer(this.m_SubSelectionContainer);
				UICourtMemberMerchantImportIcon merchantImportAction3 = this.m_MerchantImportAction;
				merchantImportAction3.OnSelected = (Action<UICourtMemberMerchantImportIcon, PointerEventData>)Delegate.Combine(merchantImportAction3.OnSelected, new Action<UICourtMemberMerchantImportIcon, PointerEventData>(this.HandleMerchantImportSelected));
				return;
			}
			this.m_MerchantImportAction.SetData(this.Data, null);
			return;
		}
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x00156D70 File Offset: 0x00154F70
	private void HandleMerchantImportSelected(UICourtMemberMerchantImportIcon icon, PointerEventData e)
	{
		this.DeselectAll();
		if (icon != null)
		{
			this.SelectCathegoryIcon(icon);
			bool flag = this.m_SelectedCategoryIcon == icon;
			icon.Select(flag);
			icon.ToggleImportActions(flag);
			Vector3 position = this.m_SubSelectionContainer.transform.position;
			position.x = icon.transform.position.x;
			this.m_SubSelectionContainer.position = position;
		}
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x00156DE4 File Offset: 0x00154FE4
	private void HandleOnRankActionSelected(UICourtMemberRankIcon icon, PointerEventData e)
	{
		this.DeselectAll();
		if (icon != null)
		{
			this.SelectCathegoryIcon(icon);
			bool flag = this.m_SelectedCategoryIcon == icon;
			icon.Select(flag);
			icon.ToggleSkillWindow(flag);
			Vector3 localPosition = this.m_SubSelectionContainer.transform.localPosition;
			localPosition.x = 0f;
			this.m_SubSelectionContainer.localPosition = localPosition;
		}
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x00156E4B File Offset: 0x0015504B
	private void HandleOnActionIconSelected()
	{
		this.DeselectAll();
		this.UpdateSelection();
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x00156E5C File Offset: 0x0015505C
	private void DeselectAll()
	{
		if (this.m_RankActionIcon != null)
		{
			this.m_RankActionIcon.Select(false);
			this.m_RankActionIcon.CloseSubSelections();
		}
		if (this.m_MerchantImportAction != null)
		{
			this.m_MerchantImportAction.Select(false);
			this.m_MerchantImportAction.CloseSubSelections();
		}
		if (this.m_MainStatusIcon != null)
		{
			this.m_MainStatusIcon.Select(false);
		}
		if (this.m_StatusIcons != null && this.m_StatusIcons.Count > 0)
		{
			for (int i = 0; i < this.m_StatusIcons.Count; i++)
			{
				UIStatusIcon uistatusIcon = this.m_StatusIcons[i];
				if (uistatusIcon != null)
				{
					uistatusIcon.Select(false);
				}
			}
		}
		if (this.m_BribleStatusIcons != null && this.m_BribleStatusIcons.Count > 0)
		{
			for (int j = 0; j < this.m_BribleStatusIcons.Count; j++)
			{
				UIStatusIcon uistatusIcon2 = this.m_BribleStatusIcons[j];
				if (uistatusIcon2 != null)
				{
					uistatusIcon2.Select(false);
				}
			}
		}
		if (this.m_BribeStatusActionsContainer != null)
		{
			UICommon.DeleteChildren(this.m_BribeStatusActionsContainer);
		}
		if (this.m_SubSelectionContainer != null)
		{
			UICommon.DeleteChildren(this.m_SubSelectionContainer);
		}
		if (this.m_StatusActionsContainer != null)
		{
			for (int k = 0; k < this.m_StatusActionsContainer.childCount; k++)
			{
				Transform child = this.m_StatusActionsContainer.GetChild(k);
				if (child != null)
				{
					UIActionIcon component = child.GetComponent<UIActionIcon>();
					if (component != null)
					{
						component.Disable();
					}
				}
			}
			this.m_StatusActionsContainer.gameObject.SetActive(false);
		}
		if (this.m_CurrentActionInfoContainer != null)
		{
			UICommon.DeleteChildren(this.m_CurrentActionInfoContainer);
		}
		UITargetSelectWindow.Instance.Show(false);
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x00157014 File Offset: 0x00155214
	private void PopulateStatuses(Logic.Character character)
	{
		if (this.m_MainStatusContainer != null)
		{
			Logic.Status status = character.GetStatus();
			if (status != null)
			{
				string val = (character.cur_action != null) ? "ongoing_action" : null;
				if (this.m_MainStatusIcon == null || this.m_MainStatusIcon.vars.Get("variant", true) != val)
				{
					if (this.m_MainStatusIcon != null)
					{
						global::Common.DestroyObj(this.m_MainStatusIcon.gameObject);
					}
					Vars vars = new Vars();
					if (character.cur_action != null)
					{
						vars.Set<string>("variant", "ongoing_action");
					}
					GameObject icon = ObjectIcon.GetIcon(status, vars, this.m_MainStatusContainer);
					this.m_MainStatusIcon = ((icon != null) ? icon.GetComponent<UIStatusIcon>() : null);
					UIStatusIcon mainStatusIcon = this.m_MainStatusIcon;
					mainStatusIcon.OnSelect = (Action<UIStatusIcon>)Delegate.Combine(mainStatusIcon.OnSelect, new Action<UIStatusIcon>(this.HandleOnStatusSelect));
				}
				this.m_MainStatusIcon.SetObject(status, null);
				this.m_MainStatusIcon.gameObject.SetActive(true);
			}
			else if (this.m_MainStatusIcon != null)
			{
				this.m_MainStatusIcon.gameObject.SetActive(false);
			}
		}
		if (this.m_statusesContainer != null)
		{
			for (int i = this.m_statusesContainer.childCount - 1; i >= 0; i--)
			{
				this.m_statusesContainer.GetChild(i).gameObject.SetActive(false);
			}
			if (character == null)
			{
				return;
			}
			Statuses statuses = character.statuses;
			if (statuses != null)
			{
				int num = 0;
				for (int j = 1; j < statuses.Count; j++)
				{
					Logic.Status status2 = statuses[j];
					if (status2 != null && !(status2 is HasPuppetStatus) && status2.def.field.GetBool("show_in_action_bar", status2, false, true, true, true, '.'))
					{
						if (this.m_StatusIcons.Count <= num)
						{
							GameObject icon2 = ObjectIcon.GetIcon(status2, null, this.m_statusesContainer);
							UIStatusIcon uistatusIcon = (icon2 != null) ? icon2.GetComponent<UIStatusIcon>() : null;
							uistatusIcon.KeepAlive(true);
							UIStatusIcon uistatusIcon2 = uistatusIcon;
							uistatusIcon2.OnSelect = (Action<UIStatusIcon>)Delegate.Combine(uistatusIcon2.OnSelect, new Action<UIStatusIcon>(this.HandleOnStatusSelect));
							this.m_StatusIcons.Add(uistatusIcon);
						}
						else
						{
							this.m_StatusIcons[num].SetObject(status2, null);
						}
						this.m_StatusIcons[num].gameObject.SetActive(true);
						num++;
					}
				}
			}
		}
		if (this.m_BribeStatusContianer != null)
		{
			for (int k = this.m_BribeStatusContianer.childCount - 1; k >= 0; k--)
			{
				this.m_BribeStatusContianer.GetChild(k).gameObject.SetActive(false);
			}
			if (character == null)
			{
				return;
			}
			Statuses statuses2 = character.statuses;
			if (statuses2 != null)
			{
				int num2 = 0;
				for (int l = 1; l < statuses2.Count; l++)
				{
					HasPuppetStatus hasPuppetStatus = statuses2[l] as HasPuppetStatus;
					if (hasPuppetStatus != null && hasPuppetStatus.def.field.GetBool("show_in_action_bar", hasPuppetStatus, false, true, true, true, '.'))
					{
						if (this.m_BribleStatusIcons.Count <= num2)
						{
							GameObject icon3 = ObjectIcon.GetIcon(hasPuppetStatus, null, this.m_BribeStatusContianer);
							UIStatusIcon uistatusIcon3 = (icon3 != null) ? icon3.GetComponent<UIStatusIcon>() : null;
							uistatusIcon3.KeepAlive(true);
							UIStatusIcon uistatusIcon4 = uistatusIcon3;
							uistatusIcon4.OnSelect = (Action<UIStatusIcon>)Delegate.Combine(uistatusIcon4.OnSelect, new Action<UIStatusIcon>(this.HandleOnStatusSelect));
							this.m_BribleStatusIcons.Add(uistatusIcon3);
						}
						else
						{
							this.m_BribleStatusIcons[num2].SetObject(hasPuppetStatus, null);
						}
						this.m_BribleStatusIcons[num2].gameObject.SetActive(true);
						num2++;
					}
				}
			}
		}
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x001573EC File Offset: 0x001555EC
	private void HandleOnBribeStatusSelect(UIStatusIcon statusIcon)
	{
		this.DeselectAll();
		if (this.m_BribeStatusActionsContainer == null)
		{
			return;
		}
		this.SelectCathegoryIcon(statusIcon);
		bool flag = this.m_SelectedCategoryIcon == statusIcon;
		statusIcon.Select(flag);
		if (this.m_BribleInfo != null && !flag)
		{
			this.m_BribleInfo.Close();
			this.m_BribleInfo = null;
			return;
		}
		if (flag)
		{
			this.m_BribleInfo = UICourtMemberBribeInfo.Create(statusIcon.Data, UICourtMemberBribeInfo.GetPrefab(), this.m_BribeStatusActionsContainer, null);
		}
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x00157470 File Offset: 0x00155670
	private void HandleOnStatusSelect(UIStatusIcon statusIcon)
	{
		UnityEngine.Object selectedCategoryIcon = this.m_SelectedCategoryIcon;
		this.DeselectAll();
		if (selectedCategoryIcon == statusIcon)
		{
			this.m_InvalidateSelection = true;
			return;
		}
		if (this.m_StatusActionsContainer == null)
		{
			return;
		}
		if (statusIcon == null)
		{
			return;
		}
		if (statusIcon.Data == null)
		{
			return;
		}
		OngoingActionStatus ongoingActionStatus = statusIcon.Data as OngoingActionStatus;
		Action action = ((ongoingActionStatus != null) ? ongoingActionStatus.GetAction() : null) ?? null;
		if (action != null && this.m_CurrentActionInfoContainer != null)
		{
			this.SelectCathegoryIcon(statusIcon);
			if (this.m_SelectedCategoryIcon == statusIcon)
			{
				if (action is RecallAction)
				{
					this.m_InvalidateSelection = true;
					return;
				}
				GameObject obj = global::Defs.GetObj<GameObject>(action.def.field, "progress_window", null);
				if (obj != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity, this.m_CurrentActionInfoContainer);
					this.m_PlotProgressInfo = gameObject.GetOrAddComponent<UIActionProgressInfo>();
					this.m_PlotProgressInfo.SetData(action);
					statusIcon.Select(true);
					this.m_CurrentActionInfoContainer.gameObject.SetActive(true);
					Vector3 position = this.m_CurrentActionInfoContainer.transform.position;
					position.x = statusIcon.transform.position.x;
					this.m_CurrentActionInfoContainer.position = position;
					return;
				}
			}
		}
		UICommon.DeleteChildren(this.m_StatusActionsContainer, typeof(ObjectIcon));
		bool value = false;
		UICourtMemberActions.tempDefList.Clear();
		statusIcon.Data.GetActions(UICourtMemberActions.tempDefList);
		if (UICourtMemberActions.tempDefList.Count > 0)
		{
			bool @bool = statusIcon.Data.def.field.GetBool("show_singleton_action", statusIcon.Data, false, true, true, true, '.');
			if (UICourtMemberActions.tempDefList.Count == 1 && !@bool)
			{
				Actions actions = this.Data.actions;
				Action action2 = (actions != null) ? actions.Find(UICourtMemberActions.tempDefList[0]) : null;
				if (action2 == null || !(action2.Validate(true) == "ok"))
				{
					DT.Field soundsDef = BaseUI.soundsDef;
					BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("action_inactive", null, "", true, true, true, '.') : null, null);
					this.m_InvalidateSelection = true;
					return;
				}
				if (statusIcon.Data is TradeExpeditionStatus)
				{
					action2.target = (statusIcon.Data as TradeExpeditionStatus);
				}
				ActionVisuals.ExecuteAction(action2);
			}
			else if (UICourtMemberActions.tempDefList.Count > 0)
			{
				this.SelectCathegoryIcon(statusIcon);
				bool flag = false;
				for (int i = 0; i < UICourtMemberActions.tempDefList.Count; i++)
				{
					Actions actions2 = this.Data.actions;
					Action action3 = (actions2 != null) ? actions2.Find(UICourtMemberActions.tempDefList[i]) : null;
					if (action3 != null)
					{
						HasPuppetStatus hasPuppetStatus;
						if ((hasPuppetStatus = (statusIcon.Data as HasPuppetStatus)) != null)
						{
							action3.target = hasPuppetStatus.puppet;
						}
						string text = action3.Validate(false);
						if (action3.def.opportunity == null && (!(text != "ok") || text.StartsWith("_", StringComparison.Ordinal)))
						{
							ActionVisuals actionVisuals = action3.visuals as ActionVisuals;
							if (actionVisuals != null)
							{
								actionVisuals.logic.AddListener(this);
								GameObject icon = ObjectIcon.GetIcon(actionVisuals.logic, null, this.m_StatusActionsContainer);
								UIActionIcon uiactionIcon = (icon != null) ? icon.GetComponent<UIActionIcon>() : null;
								if (uiactionIcon != null)
								{
									uiactionIcon.ShowIfNotActive = true;
								}
								flag = true;
							}
						}
					}
				}
				if (flag)
				{
					this.m_StatusActionsContainer.gameObject.SetActive(true);
					Vector3 position2 = this.m_StatusActionsContainer.transform.position;
					position2.x = statusIcon.transform.position.x;
					this.m_StatusActionsContainer.position = position2;
					value = true;
				}
			}
			statusIcon.Select(value);
			return;
		}
		this.m_InvalidateSelection = true;
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x0015783C File Offset: 0x00155A3C
	private void PolulateExileButton()
	{
		Logic.Character data = this.Data;
		if (((data != null) ? data.actions : null) == null)
		{
			return;
		}
		if (this.m_Exile == null)
		{
			return;
		}
		Action action = this.Data.actions.Find("ExileAction");
		if (action != null && action.Validate(true) == "ok")
		{
			this.m_Exile.gameObject.SetActive(true);
			if (this.m_ExlieActionIcon == null)
			{
				this.m_ExlieActionIcon = UIActionIcon.Possess(action.visuals as ActionVisuals, this.m_Exile.gameObject, new Vars(this.Data));
				return;
			}
			if (this.m_ExlieActionIcon.Data != action.visuals)
			{
				this.m_ExlieActionIcon.SetObject(action, new Vars(this.Data));
				return;
			}
		}
		else
		{
			this.m_Exile.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x00157934 File Offset: 0x00155B34
	private Action GetValidRecallAction()
	{
		for (int i = 0; i < this.Data.actions.Count; i++)
		{
			Action action = this.Data.actions[i];
			if (action is RecallAction)
			{
				if (this.Data.cur_action == action)
				{
					return action;
				}
				if (action.def.field.GetBool("show_in_action_bar", action, false, true, true, true, '.') && !(action.Validate(true) != "ok"))
				{
					return action;
				}
			}
		}
		return null;
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x001579BC File Offset: 0x00155BBC
	private void PolulateRecallButton()
	{
		Logic.Character data = this.Data;
		if (((data != null) ? data.actions : null) == null)
		{
			return;
		}
		Action validRecallAction = this.GetValidRecallAction();
		if (validRecallAction != null)
		{
			this.m_RecallAction.gameObject.SetActive(true);
			if (this.m_RecallAction == null)
			{
				this.m_RecallAction = UIActionIcon.Possess(validRecallAction.visuals as ActionVisuals, this.m_RecallAction.gameObject, new Vars(this.Data));
				return;
			}
			if (this.m_RecallAction.Data != validRecallAction.visuals)
			{
				this.m_RecallAction.SetObject(validRecallAction, new Vars(this.Data));
				return;
			}
		}
		else
		{
			this.m_RecallAction.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x00157A7D File Offset: 0x00155C7D
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "statuses_changed" || message == "status_changed")
		{
			this.m_InvalidateStatuses = true;
			return;
		}
		if (!(message == "cur_action_changed"))
		{
			return;
		}
		this.m_InvalidateCurrentAction = true;
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x00157AB6 File Offset: 0x00155CB6
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UICourtMemberActions.def_id, null);
	}

	// Token: 0x04001ACD RID: 6861
	private static string def_id = "CourtMemberActions";

	// Token: 0x04001ACF RID: 6863
	[UIFieldTarget("id_ActionsContainer")]
	private GameObject m_ActionsContainer;

	// Token: 0x04001AD0 RID: 6864
	[UIFieldTarget("id_StatusContainer")]
	private RectTransform m_statusesContainer;

	// Token: 0x04001AD1 RID: 6865
	[UIFieldTarget("id_MerchantImportActionsContainerContainer")]
	private RectTransform m_MerchantImportActionsContainer;

	// Token: 0x04001AD2 RID: 6866
	[UIFieldTarget("id_BribeStatusContianer")]
	private RectTransform m_BribeStatusContianer;

	// Token: 0x04001AD3 RID: 6867
	[UIFieldTarget("id_CustomActionsContainer")]
	private RectTransform m_CustomActionsContainer;

	// Token: 0x04001AD4 RID: 6868
	[UIFieldTarget("id_MainStatusContainer")]
	private RectTransform m_MainStatusContainer;

	// Token: 0x04001AD5 RID: 6869
	[UIFieldTarget("id_SubSelectionContainer")]
	private RectTransform m_SubSelectionContainer;

	// Token: 0x04001AD6 RID: 6870
	[UIFieldTarget("id_StatusActionsContainer")]
	private RectTransform m_StatusActionsContainer;

	// Token: 0x04001AD7 RID: 6871
	[UIFieldTarget("id_BribeStatusContainer")]
	private RectTransform m_BribeStatusContainer;

	// Token: 0x04001AD8 RID: 6872
	[UIFieldTarget("id_BribeStatusActionsContainer")]
	private RectTransform m_BribeStatusActionsContainer;

	// Token: 0x04001AD9 RID: 6873
	[UIFieldTarget("id_CurrentActionInfoContainer")]
	private RectTransform m_CurrentActionInfoContainer;

	// Token: 0x04001ADA RID: 6874
	private UIActions m_CurrentActionWindow;

	// Token: 0x04001ADB RID: 6875
	private RectTransform m_CurAncor;

	// Token: 0x04001ADC RID: 6876
	private UICharacter m_currentChartacterSelection;

	// Token: 0x04001ADD RID: 6877
	private UICourtMemberRankIcon m_RankActionIcon;

	// Token: 0x04001ADE RID: 6878
	[UIFieldTarget("id_CourtMemberOpportunityActions")]
	private RectTransform m_OpportunityActionsContianer;

	// Token: 0x04001ADF RID: 6879
	private UICourtMemberActions.OpportunityActions opportunityActions;

	// Token: 0x04001AE0 RID: 6880
	[UIFieldTarget("id_Exile")]
	private GameObject m_Exile;

	// Token: 0x04001AE1 RID: 6881
	[UIFieldTarget("id_RecallAction")]
	private UIActionIcon m_RecallAction;

	// Token: 0x04001AE2 RID: 6882
	private UICourtMemberMerchantImportIcon m_MerchantImportAction;

	// Token: 0x04001AE3 RID: 6883
	[UIFieldTarget("id_ActionsAndOpportunities")]
	private GameObject m_ActionsAndOpportunities;

	// Token: 0x04001AE4 RID: 6884
	private UIStatusIcon m_MainStatusIcon;

	// Token: 0x04001AE5 RID: 6885
	private List<UIStatusIcon> m_StatusIcons = new List<UIStatusIcon>();

	// Token: 0x04001AE6 RID: 6886
	private List<UIStatusIcon> m_BribleStatusIcons = new List<UIStatusIcon>();

	// Token: 0x04001AE7 RID: 6887
	private bool m_InvalidateStatuses;

	// Token: 0x04001AE8 RID: 6888
	private bool m_InvalidateCurrentAction;

	// Token: 0x04001AE9 RID: 6889
	private bool m_InvalidateSelection;

	// Token: 0x04001AEA RID: 6890
	private bool m_initialzed;

	// Token: 0x04001AEB RID: 6891
	public float yOffset = -80f;

	// Token: 0x04001AEC RID: 6892
	private MonoBehaviour m_SelectedCategoryIcon;

	// Token: 0x04001AED RID: 6893
	private UICourtMemberBribeInfo m_BribleInfo;

	// Token: 0x04001AEE RID: 6894
	private UIActionProgressInfo m_PlotProgressInfo;

	// Token: 0x04001AEF RID: 6895
	private static List<Action.Def> tempDefList = new List<Action.Def>();

	// Token: 0x04001AF0 RID: 6896
	private UIActionIcon m_ExlieActionIcon;

	// Token: 0x020007E3 RID: 2019
	internal class OpportunityActions : MonoBehaviour, IListener
	{
		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06004E9D RID: 20125 RVA: 0x00232A96 File Offset: 0x00230C96
		// (set) Token: 0x06004E9E RID: 20126 RVA: 0x00232A9E File Offset: 0x00230C9E
		public Logic.Character Character { get; private set; }

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06004E9F RID: 20127 RVA: 0x00232AA7 File Offset: 0x00230CA7
		// (set) Token: 0x06004EA0 RID: 20128 RVA: 0x00232AAF File Offset: 0x00230CAF
		public Logic.Kingdom kingdom { get; private set; }

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06004EA1 RID: 20129 RVA: 0x00232AB8 File Offset: 0x00230CB8
		// (set) Token: 0x06004EA2 RID: 20130 RVA: 0x00232AC0 File Offset: 0x00230CC0
		public Vars Vars { get; private set; }

		// Token: 0x06004EA3 RID: 20131 RVA: 0x00232ACC File Offset: 0x00230CCC
		public void SetData(Logic.Character character, Vars vars)
		{
			this.Init();
			Logic.Character character2 = this.Character;
			if (character2 != null)
			{
				character2.DelListener(this);
			}
			Logic.Kingdom kingdom = this.kingdom;
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
			this.Character = character;
			Logic.Character character3 = this.Character;
			this.kingdom = ((character3 != null) ? character3.GetKingdom() : null);
			Logic.Character character4 = this.Character;
			if (character4 != null)
			{
				character4.AddListener(this);
			}
			Logic.Kingdom kingdom2 = this.kingdom;
			if (kingdom2 != null)
			{
				kingdom2.AddListener(this);
			}
			this.Vars = vars;
			this.Refresh();
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x00232B53 File Offset: 0x00230D53
		private void Init()
		{
			if (this.m_Initalized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.SetupIcons();
			this.m_Initalized = true;
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x00232B72 File Offset: 0x00230D72
		private void Refresh()
		{
			this.PopulateIcons();
		}

		// Token: 0x06004EA6 RID: 20134 RVA: 0x00232B7C File Offset: 0x00230D7C
		private void SetupIcons()
		{
			global::Common.FindChildrenWithComponent<UIOpportunityIcon>(base.gameObject, this.m_Icons);
			for (int i = 0; i < this.m_Icons.Count; i++)
			{
				this.m_Icons[i].SetObject(null, null);
			}
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x00232BC4 File Offset: 0x00230DC4
		private void PopulateIcons()
		{
			if (this.Character == null)
			{
				return;
			}
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
			int[] array = new int[3];
			int num = 0;
			CharacterClass.Def class_def = this.Character.class_def;
			int? num2;
			if (class_def == null)
			{
				num2 = null;
			}
			else
			{
				Opportunity.ClassDef opportunities = class_def.opportunities;
				num2 = ((opportunities != null) ? new int?(opportunities.max_count) : null);
			}
			array[num] = (num2 ?? 8);
			array[1] = this.m_Icons.Count;
			array[2] = ((list2 != null) ? list2.Count : 0);
			int i = Mathf.Max(array) + 1;
			while (i > this.m_Icons.Count)
			{
				GameObject icon = ObjectIcon.GetIcon("Opportunity", null, base.transform as RectTransform);
				UIOpportunityIcon uiopportunityIcon = (icon != null) ? icon.GetComponent<UIOpportunityIcon>() : null;
				if (uiopportunityIcon == null)
				{
					break;
				}
				this.m_Icons.Add(uiopportunityIcon);
			}
			int j = 0;
			int num3 = 0;
			while (j < this.m_Icons.Count)
			{
				if (list2 == null || num3 >= list2.Count)
				{
					this.m_Icons[j].SetObject(null, null);
					this.m_Icons[j].gameObject.SetActive(false);
					j++;
				}
				else
				{
					Opportunity opportunity = list2[num3];
					num3++;
					if (opportunity != null && (opportunity.active || opportunity.IsRunning()))
					{
						string text = opportunity.Validate() ?? "null";
						bool flag = opportunity.forced || text == "ok" || text.StartsWith("_", StringComparison.Ordinal);
						flag |= (opportunity.action.state == Action.State.Preparing);
						if (flag)
						{
							this.m_Icons[j].SetObject(opportunity, null);
						}
						else
						{
							this.m_Icons[j].SetObject(null, null);
						}
						this.m_Icons[j].gameObject.SetActive(flag);
						j++;
					}
				}
			}
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x00232DDB File Offset: 0x00230FDB
		private void Clear()
		{
			Logic.Character character = this.Character;
			if (character != null)
			{
				character.DelListener(this);
			}
			Logic.Kingdom kingdom = this.kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.DelListener(this);
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x00232E00 File Offset: 0x00231000
		private void OnDestroy()
		{
			this.Clear();
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x00232E08 File Offset: 0x00231008
		public void OnMessage(object obj, string message, object param)
		{
			if (message == "new_opportunity" || message == "opportunity_lost" || message == "opportunities_changed")
			{
				this.PopulateIcons();
				return;
			}
			if (!(message == "new_tradition"))
			{
				return;
			}
			foreach (UIOpportunityIcon uiopportunityIcon in this.m_Icons)
			{
				uiopportunityIcon.RefreshTooltip();
			}
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x00232E94 File Offset: 0x00231094
		public bool HasActiveOpportunities()
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
			if (list2 == null)
			{
				return false;
			}
			for (int i = 0; i < list2.Count; i++)
			{
				Opportunity opportunity = list2[i];
				if (opportunity != null && opportunity.active)
				{
					string text = opportunity.Validate() ?? "null";
					if ((opportunity.forced || text == "ok" || text.StartsWith("_", StringComparison.Ordinal)) | opportunity.action.state == Action.State.Preparing)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04003CCC RID: 15564
		private List<UIOpportunityIcon> m_Icons = new List<UIOpportunityIcon>();

		// Token: 0x04003CCD RID: 15565
		private bool m_Initalized;
	}
}
