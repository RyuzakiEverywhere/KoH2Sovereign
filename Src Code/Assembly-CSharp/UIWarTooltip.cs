using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class UIWarTooltip : MonoBehaviour, Tooltip.IHandler
{
	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060020D8 RID: 8408 RVA: 0x0012B7C2 File Offset: 0x001299C2
	// (set) Token: 0x060020D9 RID: 8409 RVA: 0x0012B7CA File Offset: 0x001299CA
	public War War { get; private set; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x060020DA RID: 8410 RVA: 0x0012B7D3 File Offset: 0x001299D3
	// (set) Token: 0x060020DB RID: 8411 RVA: 0x0012B7DB File Offset: 0x001299DB
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x060020DC RID: 8412 RVA: 0x0012B7E4 File Offset: 0x001299E4
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.def = global::Defs.GetDefField("WarTooltip", null);
		this.m_Initialized = true;
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x0012B810 File Offset: 0x00129A10
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Kingdom kingdom = null;
		War war = null;
		if (tooltip.vars != null)
		{
			kingdom = BaseUI.LogicKingdom();
			war = tooltip.vars.obj.Get<War>();
			if (!war.IsMember(kingdom))
			{
				kingdom = war.GetLeader(0);
			}
		}
		return this.HandleTooltip(kingdom, war, tooltip.vars, ui, evt);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x0012B861 File Offset: 0x00129A61
	public bool HandleTooltip(Logic.Kingdom kingdom, War war, Vars vars, BaseUI ui, Tooltip.Event evt)
	{
		this.Init();
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			this.vars = vars;
			this.SetData(kingdom, war);
			this.Refresh(evt != Tooltip.Event.Update);
			return true;
		}
		return false;
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x0012B894 File Offset: 0x00129A94
	private void SetData(Logic.Kingdom k, War w)
	{
		this.Kingdom = k;
		this.War = w;
		if (this.m_WarInfo != null)
		{
			bool flag = this.Kingdom != null && this.War != null;
			UIWarsOverviewWindow.UIWarRow uiwarRow = this.m_WarInfo.AddComponent<UIWarsOverviewWindow.UIWarRow>();
			uiwarRow.gameObject.SetActive(flag);
			if (flag)
			{
				uiwarRow.SetData(this.Kingdom, this.War);
			}
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x0012B8FF File Offset: 0x00129AFF
	private void Refresh(bool full = true)
	{
		this.Init();
		if (full)
		{
			this.PoulateHeader();
		}
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x0012B910 File Offset: 0x00129B10
	private void PoulateHeader()
	{
		if (this.m_Caption != null)
		{
			UIText.SetText(this.m_Caption, this.def, "caption", this.vars, null);
		}
		if (this.m_HeaderDescription != null)
		{
			UIText.SetText(this.m_HeaderDescription, this.def, "text", this.vars, null);
		}
		if (this.m_Footer != null)
		{
			UIText.SetText(this.m_Footer, this.def, "footer", this.vars, null);
		}
		bool flag = string.IsNullOrEmpty(this.m_Footer.text);
		this.m_FooterSpacer.SetActive(!flag);
		this.m_FooterContainer.SetActive(!flag);
	}

	// Token: 0x040015DD RID: 5597
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040015DE RID: 5598
	[UIFieldTarget("id_HeaderDescription")]
	private TextMeshProUGUI m_HeaderDescription;

	// Token: 0x040015DF RID: 5599
	[UIFieldTarget("id_Footer")]
	private TextMeshProUGUI m_Footer;

	// Token: 0x040015E0 RID: 5600
	[UIFieldTarget("id_FooterContainer")]
	private GameObject m_FooterContainer;

	// Token: 0x040015E1 RID: 5601
	[UIFieldTarget("id_FooterSpacer")]
	private GameObject m_FooterSpacer;

	// Token: 0x040015E2 RID: 5602
	[UIFieldTarget("id_WarInfo")]
	private GameObject m_WarInfo;

	// Token: 0x040015E5 RID: 5605
	public DT.Field def;

	// Token: 0x040015E6 RID: 5606
	private Vars vars;

	// Token: 0x040015E7 RID: 5607
	private bool m_Initialized;
}
