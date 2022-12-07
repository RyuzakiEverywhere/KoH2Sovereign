using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020002E7 RID: 743
public class UITabButton : MonoBehaviour
{
	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06002F0C RID: 12044 RVA: 0x00183717 File Offset: 0x00181917
	// (set) Token: 0x06002F0D RID: 12045 RVA: 0x0018371F File Offset: 0x0018191F
	public bool Selected { get; private set; }

	// Token: 0x06002F0E RID: 12046 RVA: 0x00183728 File Offset: 0x00181928
	public void Init(UITabController c, UITabButton.Data data, Action callback)
	{
		this.data = data;
		UICommon.FindComponents(this, false);
		this.m_Button = base.GetComponentInChildren<BSGButton>(true);
		this.m_Button.SetAudioSet("DefaultAudioSetMetal");
		if (this.m_Button != null)
		{
			this.m_Button.onClick = new BSGButton.OnClick(this.HandleClick);
		}
		this.m_Controller = c;
		this.m_Callback = callback;
		this.m_Controller.Add(this);
		if (data != null)
		{
			Tooltip.Get(base.gameObject, true).SetText(data.TooltipBody, data.TooltipTitle, data.Vars);
		}
		this.Refresh();
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x001837CB File Offset: 0x001819CB
	public void Enable(bool e)
	{
		this.m_Enabled = e;
		this.UpdateState();
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x001837DA File Offset: 0x001819DA
	private void OnDestroy()
	{
		if (this.m_Controller != null)
		{
			this.m_Controller.Remove(this);
			this.m_Callback = null;
			this.m_Button.onClick = null;
			this.m_Controller = null;
		}
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x00183810 File Offset: 0x00181A10
	private void HandleClick(BSGButton btn)
	{
		if (this.m_Controller != null)
		{
			this.m_Controller.Select(this, true);
		}
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x0018382D File Offset: 0x00181A2D
	public void Select(bool selected, bool force = false)
	{
		if (this.Selected == selected && !force)
		{
			return;
		}
		this.Selected = selected;
		if (this.Selected)
		{
			Action callback = this.m_Callback;
			if (callback != null)
			{
				callback();
			}
		}
		this.UpdateState();
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x00183862 File Offset: 0x00181A62
	public UITabButton.Data GetData()
	{
		return this.data;
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x0018386A File Offset: 0x00181A6A
	public void UpdateVars()
	{
		if (this.data == null)
		{
			return;
		}
		if (this.data.UpdateVars == null)
		{
			return;
		}
		this.data.UpdateVars();
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x00183894 File Offset: 0x00181A94
	public void Refresh()
	{
		if (this.data != null)
		{
			if (this.m_Label != null && this.m_Label.Length != 0)
			{
				for (int i = 0; i < this.m_Label.Length; i++)
				{
					UIText.SetTextKey(this.m_Label[i], this.data.Text, this.data.Vars, null);
				}
			}
			if (this.m_Icon)
			{
				this.m_Icon.overrideSprite = this.data.Icon;
			}
		}
		this.UpdateState();
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x0018391C File Offset: 0x00181B1C
	private void UpdateState()
	{
		if (this.Selected)
		{
			this.OnSelect.Invoke();
		}
		else
		{
			this.OnDeselect.Invoke();
		}
		if (this.Selected)
		{
			if (this.m_Active != null)
			{
				this.m_Active.gameObject.SetActive(true);
			}
			if (this.m_Inactive != null)
			{
				this.m_Inactive.gameObject.SetActive(false);
			}
			if (this.m_Disabled != null)
			{
				this.m_Disabled.gameObject.SetActive(false);
				return;
			}
		}
		else if (this.m_Enabled)
		{
			if (this.m_Active != null)
			{
				this.m_Active.gameObject.SetActive(false);
			}
			if (this.m_Inactive != null)
			{
				this.m_Inactive.gameObject.SetActive(true);
			}
			if (this.m_Disabled != null)
			{
				this.m_Disabled.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			if (this.m_Active != null)
			{
				this.m_Active.gameObject.SetActive(false);
			}
			if (this.m_Inactive != null)
			{
				this.m_Inactive.gameObject.SetActive(false);
			}
			if (this.m_Disabled != null)
			{
				this.m_Disabled.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x00183A78 File Offset: 0x00181C78
	public static UITabButton Possses(UITabController c, GameObject go, UITabButton.Data data)
	{
		if (c == null || c.container == null)
		{
			return null;
		}
		if (go == null)
		{
			return null;
		}
		if (data == null)
		{
			return null;
		}
		UITabButton uitabButton = go.GetComponent<UITabButton>();
		if (uitabButton == null)
		{
			uitabButton = go.AddComponent<UITabButton>();
		}
		uitabButton.Init(c, data, data.Callback);
		return uitabButton;
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x00183AD4 File Offset: 0x00181CD4
	public static UITabButton Create(UITabController c, GameObject prototype, UITabButton.Data data)
	{
		if (c == null || c.container == null)
		{
			return null;
		}
		if (prototype == null)
		{
			return null;
		}
		if (data == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, c.container);
		UITabButton uitabButton = gameObject.GetComponent<UITabButton>();
		if (uitabButton == null)
		{
			uitabButton = gameObject.AddComponent<UITabButton>();
		}
		uitabButton.Init(c, data, data.Callback);
		return uitabButton;
	}

	// Token: 0x04001FC0 RID: 8128
	private UITabController m_Controller;

	// Token: 0x04001FC1 RID: 8129
	private Action m_Callback;

	// Token: 0x04001FC2 RID: 8130
	[UIFieldTarget("id_Label")]
	private TextMeshProUGUI[] m_Label;

	// Token: 0x04001FC3 RID: 8131
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001FC4 RID: 8132
	[UIFieldTarget("id_Active")]
	private GameObject m_Active;

	// Token: 0x04001FC5 RID: 8133
	[UIFieldTarget("id_Inactive")]
	private GameObject m_Inactive;

	// Token: 0x04001FC6 RID: 8134
	[UIFieldTarget("id_Disabled")]
	private GameObject m_Disabled;

	// Token: 0x04001FC8 RID: 8136
	private BSGButton m_Button;

	// Token: 0x04001FC9 RID: 8137
	private UITabButton.Data data;

	// Token: 0x04001FCA RID: 8138
	public UnityEvent OnSelect;

	// Token: 0x04001FCB RID: 8139
	public UnityEvent OnDeselect;

	// Token: 0x04001FCC RID: 8140
	private bool m_Enabled = true;

	// Token: 0x02000863 RID: 2147
	public class Data
	{
		// Token: 0x04003F01 RID: 16129
		public string Text;

		// Token: 0x04003F02 RID: 16130
		public Sprite Icon;

		// Token: 0x04003F03 RID: 16131
		public string TooltipTitle;

		// Token: 0x04003F04 RID: 16132
		public string TooltipBody;

		// Token: 0x04003F05 RID: 16133
		public Vars Vars;

		// Token: 0x04003F06 RID: 16134
		public Action Callback;

		// Token: 0x04003F07 RID: 16135
		public Action UpdateVars;
	}
}
