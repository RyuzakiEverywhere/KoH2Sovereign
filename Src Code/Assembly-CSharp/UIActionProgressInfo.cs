using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000286 RID: 646
public class UIActionProgressInfo : MonoBehaviour, IListener
{
	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06002760 RID: 10080 RVA: 0x001561B6 File Offset: 0x001543B6
	// (set) Token: 0x06002761 RID: 10081 RVA: 0x001561BE File Offset: 0x001543BE
	public Action Action { protected get; set; }

	// Token: 0x06002762 RID: 10082 RVA: 0x001561C7 File Offset: 0x001543C7
	protected virtual string GetCancelActionId()
	{
		return "CancelMainAction";
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x001561D0 File Offset: 0x001543D0
	public void SetData(Action action)
	{
		this.Init();
		if (action == null)
		{
			return;
		}
		this.Action = action;
		if (this.m_CancelActionScript != null)
		{
			UIActionIcon cancelActionScript = this.m_CancelActionScript;
			cancelActionScript.OnSelect = (Action<UIActionIcon, PointerEventData>)Delegate.Remove(cancelActionScript.OnSelect, new Action<UIActionIcon, PointerEventData>(this.HandleOnCancelAction));
		}
		if (this.m_CancelAction != null)
		{
			if (action.def.can_be_canceled)
			{
				Action action2 = this.Action;
				Action action3;
				if (action2 == null)
				{
					action3 = null;
				}
				else
				{
					Logic.Character own_character = action2.own_character;
					if (own_character == null)
					{
						action3 = null;
					}
					else
					{
						Actions actions = own_character.actions;
						action3 = ((actions != null) ? actions.Find(this.GetCancelActionId()) : null);
					}
				}
				Action action4 = action3;
				if (action4 != null)
				{
					if (this.m_CancelActionScript == null)
					{
						this.m_CancelActionScript = UIActionIcon.Possess(action4.visuals as ActionVisuals, this.m_CancelAction, null);
						UIActionIcon cancelActionScript2 = this.m_CancelActionScript;
						cancelActionScript2.OnSelect = (Action<UIActionIcon, PointerEventData>)Delegate.Combine(cancelActionScript2.OnSelect, new Action<UIActionIcon, PointerEventData>(this.HandleOnCancelAction));
					}
					else
					{
						this.m_CancelActionScript.SetObject(action4, null);
					}
				}
			}
			this.m_CancelAction.SetActive(action.def.can_be_canceled);
		}
		this.Refresh();
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x001562F5 File Offset: 0x001544F5
	private void HandleOnCancelAction(UIActionIcon a, PointerEventData e)
	{
		Action onCancel = this.OnCancel;
		if (onCancel == null)
		{
			return;
		}
		onCancel();
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x00156307 File Offset: 0x00154507
	protected void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x00156320 File Offset: 0x00154520
	private void Refresh()
	{
		this.UpdateStatics();
		this.UpdateDynamics();
		this.UpdateProgress();
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x00156334 File Offset: 0x00154534
	private void Update()
	{
		this.UpdateDynamics();
		this.UpdateProgress();
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x00156344 File Offset: 0x00154544
	private void UpdateStatics()
	{
		if (this.Action == null)
		{
			return;
		}
		if (this.m_ActionName != null)
		{
			UIText.SetText(this.m_ActionName, global::Defs.Localize(this.Action.def.dt_def.field, "name", this.Action, null, true, true));
		}
		if (this.m_FlavorText != null)
		{
			UIText.SetText(this.m_FlavorText, global::Defs.Localize(this.Action.def.dt_def.field, "prepare_status_text", this.Action, null, true, true));
		}
		if (this.m_ActionIcon != null)
		{
			this.m_ActionIcon.sprite = global::Defs.GetObj<Sprite>(this.Action.def.dt_def.field, "icon", this.Action);
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x0015641A File Offset: 0x0015461A
	private void UpdateDynamics()
	{
		if (this.m_Success != null)
		{
			UIText.SetText(this.m_Success, global::Defs.Localize("ActionProgress.success_chance", this.Action, null, true, true));
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x00156448 File Offset: 0x00154648
	private void UpdateProgress()
	{
		if (this.Action == null)
		{
			return;
		}
		if (this.m_ActionProgressForegorund == null)
		{
			return;
		}
		float num;
		float num2;
		this.Action.GetProgress(out num, out num2);
		if (this.m_GroupActionProgress != null)
		{
			this.m_GroupActionProgress.gameObject.SetActive(num2 > 0f);
		}
		this.m_ActionProgressForegorund.fillAmount = ((num2 != 0f) ? (num / num2) : 0f);
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000023FD File Offset: 0x000005FD
	private void Clear()
	{
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x001564BF File Offset: 0x001546BF
	public void Close()
	{
		this.Clear();
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDestroy()
	{
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x001564D2 File Offset: 0x001546D2
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CurentActionPogressInfo", null);
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x001564E0 File Offset: 0x001546E0
	public static UIActionProgressInfo Create(Action action, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIActionProgressInfo orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UIActionProgressInfo>();
		orAddComponent.SetData(action);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x04001AC2 RID: 6850
	[UIFieldTarget("id_ActionName")]
	protected TextMeshProUGUI m_ActionName;

	// Token: 0x04001AC3 RID: 6851
	[UIFieldTarget("id_FlavorText")]
	protected TextMeshProUGUI m_FlavorText;

	// Token: 0x04001AC4 RID: 6852
	[UIFieldTarget("id_Success")]
	protected TextMeshProUGUI m_Success;

	// Token: 0x04001AC5 RID: 6853
	[UIFieldTarget("id_ActionIcon")]
	protected Image m_ActionIcon;

	// Token: 0x04001AC6 RID: 6854
	[UIFieldTarget("id_ActionProgressForegorund")]
	protected Image m_ActionProgressForegorund;

	// Token: 0x04001AC7 RID: 6855
	[UIFieldTarget("id_CancelAction")]
	protected GameObject m_CancelAction;

	// Token: 0x04001AC8 RID: 6856
	[UIFieldTarget("Group_ActionProgress")]
	protected GameObject m_GroupActionProgress;

	// Token: 0x04001AC9 RID: 6857
	public Action OnCancel;

	// Token: 0x04001ACB RID: 6859
	private UIActionIcon m_CancelActionScript;

	// Token: 0x04001ACC RID: 6860
	protected bool m_Initalized;
}
