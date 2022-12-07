using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class UIStatusList : MonoBehaviour, IListener
{
	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001EA4 RID: 7844 RVA: 0x0011C6F7 File Offset: 0x0011A8F7
	// (set) Token: 0x06001EA5 RID: 7845 RVA: 0x0011C6FF File Offset: 0x0011A8FF
	public Logic.Object Data { get; private set; }

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06001EA6 RID: 7846 RVA: 0x0011C708 File Offset: 0x0011A908
	// (remove) Token: 0x06001EA7 RID: 7847 RVA: 0x0011C740 File Offset: 0x0011A940
	public event Action<Logic.Status, string> OnStatusButton;

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06001EA8 RID: 7848 RVA: 0x0011C778 File Offset: 0x0011A978
	// (remove) Token: 0x06001EA9 RID: 7849 RVA: 0x0011C7B0 File Offset: 0x0011A9B0
	public event Action<Logic.Status> OnStatusSelected;

	// Token: 0x06001EAA RID: 7850 RVA: 0x0011C7E5 File Offset: 0x0011A9E5
	private void Awake()
	{
		this.SetData(null);
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x0011C7EE File Offset: 0x0011A9EE
	private void OnDestroy()
	{
		this.Clean();
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x0011C7F6 File Offset: 0x0011A9F6
	private void Clean()
	{
		if (this.m_InfoWindow != null)
		{
			UnityEngine.Object.Destroy(this.m_InfoWindow);
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.m_Icons.Clear();
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x0011C830 File Offset: 0x0011AA30
	public void SetData(Logic.Object data)
	{
		UICommon.FindComponents(this, false);
		this.Data = data;
		this.Clean();
		if (this.Data == null)
		{
			return;
		}
		this.Data.AddListener(this);
		this.Refresh(true);
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x0011C862 File Offset: 0x0011AA62
	public void SetBlackList(List<string> keys)
	{
		this.blacklistedStatuses.Clear();
		this.blacklistedStatuses.AddRange(keys);
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x0011C87B File Offset: 0x0011AA7B
	public void SupressAutoSelect(bool supressed)
	{
		this.m_autoSelectStatus = !supressed;
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x0011C887 File Offset: 0x0011AA87
	public void ShowMainStatus(bool shown)
	{
		this.m_ShowMainStatus = shown;
		this.Refresh(true);
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x0011C898 File Offset: 0x0011AA98
	public void Refresh(bool autoSelect = true)
	{
		if (this.m_Container == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_Container);
		this.m_Icons.Clear();
		if (this.m_InfoWindow != null)
		{
			UnityEngine.Object.Destroy(this.m_InfoWindow);
		}
		Statuses statuses = this.Data.statuses;
		if (statuses != null)
		{
			for (int i = 0; i < statuses.Count; i++)
			{
				Logic.Status status = statuses[i];
				if (status != null && (this.m_ShowMainStatus || status != this.Data.statuses.main) && !this.blacklistedStatuses.Contains(status.def.id) && status.def.show_in_window)
				{
					this.AddStatus(statuses[i]);
				}
			}
		}
		if (this.m_autoSelectStatus && !this.TryReselectPreviousStatus())
		{
			this.SelectDefaultStatus();
		}
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x0011C97C File Offset: 0x0011AB7C
	private bool TryReselectPreviousStatus()
	{
		if (this.m_CurrnetStatus != null)
		{
			Logic.Status status = this.Data.FindStatus(this.m_CurrnetStatus);
			if (status != null)
			{
				this.SelectStatus(status);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x0011C9B0 File Offset: 0x0011ABB0
	public void SelectDefaultStatus()
	{
		Logic.Status status = this.Data.GetStatus();
		if (status != null)
		{
			this.SelectStatus(status);
			return;
		}
		Logic.Status status2 = this.Data.GetStatus(1);
		if (status2 != null)
		{
			this.SelectStatus(status2);
			for (int i = 0; i < this.m_Icons.Count; i++)
			{
				if (this.m_Icons[i].Data == status2)
				{
					this.m_Icons[i].Select(true);
				}
			}
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x0011CA28 File Offset: 0x0011AC28
	public void SelectStatus(Logic.Status status)
	{
		if (this.m_InfoWindow != null)
		{
			UnityEngine.Object.Destroy(this.m_InfoWindow);
		}
		this.m_CurrnetStatus = ((status != null) ? status.def : null);
		Transform statusInfoContainer = this.GetStatusInfoContainer();
		UICommon.DeleteChildren(statusInfoContainer);
		this.m_InfoWindow = UIStatusWindow.GetWindow(status, null, statusInfoContainer as RectTransform);
		if (this.m_InfoWindow != null)
		{
			this.m_InfoWindow.transform.SetAsFirstSibling();
			UIStatusWindow component = this.m_InfoWindow.GetComponent<UIStatusWindow>();
			if (component != null)
			{
				component.OnButtonAction += this.HandleOnCharacterStatusButtonAction;
			}
		}
		this.UpdateSelection();
		if (this.OnStatusSelected != null)
		{
			this.OnStatusSelected(status);
		}
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x0011CAE0 File Offset: 0x0011ACE0
	private void UpdateSelection()
	{
		for (int i = 0; i < this.m_Icons.Count; i++)
		{
			if (!(this.m_Icons[i] == null) && this.m_Icons[i].Data != null)
			{
				this.m_Icons[i].Select(this.m_Icons[i].Data.def == this.m_CurrnetStatus);
			}
		}
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x0011CB59 File Offset: 0x0011AD59
	private void HandleOnCharacterStatusButtonAction(Logic.Status status, string action)
	{
		if (this.OnStatusButton != null)
		{
			this.OnStatusButton(status, action);
			return;
		}
		global::Status status2 = ((status != null) ? status.visuals : null) as global::Status;
		if (status2 == null)
		{
			return;
		}
		status2.OnButton(status, action);
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x0011CB90 File Offset: 0x0011AD90
	private void AddStatus(Logic.Status status)
	{
		if (status == null)
		{
			return;
		}
		if (this.Status_Prototype == null)
		{
			Debug.Log("Missing prorortype!");
			return;
		}
		if (this.m_Container == null)
		{
			Debug.Log("Missing Container!");
			return;
		}
		UIStatusIcon component = UnityEngine.Object.Instantiate<GameObject>(this.Status_Prototype, Vector3.zero, Quaternion.identity, this.m_Container).GetComponent<UIStatusIcon>();
		if (component != null)
		{
			component.SetObject(status, null);
			UIStatusIcon uistatusIcon = component;
			uistatusIcon.OnSelect = (Action<UIStatusIcon>)Delegate.Combine(uistatusIcon.OnSelect, new Action<UIStatusIcon>(this.HanldeSelect));
			this.m_Icons.Add(component);
		}
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x0011CC34 File Offset: 0x0011AE34
	private void HanldeSelect(UIStatusIcon obj)
	{
		for (int i = 0; i < this.m_Icons.Count; i++)
		{
			this.m_Icons[i].Select(false);
		}
		obj.Select(true);
		this.SelectStatus(obj.Data);
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x0011CC7C File Offset: 0x0011AE7C
	private Transform GetStatusInfoContainer()
	{
		return base.transform.parent.parent;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x0011CC90 File Offset: 0x0011AE90
	public void OnMessage(object obj, string message, object param)
	{
		if ((this == null || base.gameObject == null) && this.Data != null)
		{
			this.Data.DelListener(this);
		}
		if (message == "status_changed")
		{
			this.Refresh(true);
		}
		if (message == "statuses_changed")
		{
			this.Refresh(true);
		}
	}

	// Token: 0x04001413 RID: 5139
	[SerializeField]
	private GameObject Status_Prototype;

	// Token: 0x04001414 RID: 5140
	[UIFieldTarget("id_Container")]
	private Transform m_Container;

	// Token: 0x04001416 RID: 5142
	private Logic.Status.Def m_CurrnetStatus;

	// Token: 0x04001417 RID: 5143
	private GameObject m_InfoWindow;

	// Token: 0x0400141A RID: 5146
	private List<UIStatusIcon> m_Icons = new List<UIStatusIcon>();

	// Token: 0x0400141B RID: 5147
	public List<string> blacklistedStatuses = new List<string>();

	// Token: 0x0400141C RID: 5148
	private bool m_ShowMainStatus = true;

	// Token: 0x0400141D RID: 5149
	private bool m_autoSelectStatus = true;
}
