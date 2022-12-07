using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000219 RID: 537
public class UIPactTooltip : MonoBehaviour, Tooltip.IHandler
{
	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x0600208D RID: 8333 RVA: 0x00129A9D File Offset: 0x00127C9D
	// (set) Token: 0x0600208E RID: 8334 RVA: 0x00129AA5 File Offset: 0x00127CA5
	public Pact Pact { get; private set; }

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x0600208F RID: 8335 RVA: 0x00129AAE File Offset: 0x00127CAE
	// (set) Token: 0x06002090 RID: 8336 RVA: 0x00129AB6 File Offset: 0x00127CB6
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x06002091 RID: 8337 RVA: 0x00129ABF File Offset: 0x00127CBF
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.def = global::Defs.GetDefField("PactTooltip", null);
		this.m_Initialized = true;
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x00129AEC File Offset: 0x00127CEC
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		Logic.Kingdom kingdom = null;
		Pact war = null;
		if (tooltip.vars != null)
		{
			kingdom = BaseUI.LogicKingdom();
			war = tooltip.vars.obj.Get<Pact>();
		}
		return this.HandleTooltip(kingdom, war, tooltip.vars, ui, evt);
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x00129B2C File Offset: 0x00127D2C
	public bool HandleTooltip(Logic.Kingdom kingdom, Pact war, Vars vars, BaseUI ui, Tooltip.Event evt)
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

	// Token: 0x06002094 RID: 8340 RVA: 0x00129B60 File Offset: 0x00127D60
	private void SetData(Logic.Kingdom k, Pact w)
	{
		this.Kingdom = k;
		this.Pact = w;
		if (this.m_PactInfo != null)
		{
			bool flag = this.Kingdom != null && this.Pact != null;
			UIWarsOverviewWindow.UIPackInfo uipackInfo = this.m_PactInfo.AddComponent<UIWarsOverviewWindow.UIPackInfo>();
			uipackInfo.gameObject.SetActive(flag);
			if (flag)
			{
				uipackInfo.SetData(this.Kingdom, this.Pact);
			}
		}
		if (this.m_LeaderCrest != null)
		{
			this.m_LeaderCrest.SetObject(this.Pact.leader, null);
		}
		if (this.m_TargetCrest != null)
		{
			UIKingdomIcon targetCrest = this.m_TargetCrest;
			if (targetCrest == null)
			{
				return;
			}
			targetCrest.SetObject(this.Pact.target, null);
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x00129C1A File Offset: 0x00127E1A
	private void Refresh(bool full = true)
	{
		this.Init();
		if (full)
		{
			this.PoulateHeader();
		}
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x00129C2C File Offset: 0x00127E2C
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
	}

	// Token: 0x040015AE RID: 5550
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040015AF RID: 5551
	[UIFieldTarget("id_HeaderDescription")]
	private TextMeshProUGUI m_HeaderDescription;

	// Token: 0x040015B0 RID: 5552
	[UIFieldTarget("id_PactInfo")]
	private GameObject m_PactInfo;

	// Token: 0x040015B1 RID: 5553
	[UIFieldTarget("id_LeaderCrest")]
	private UIKingdomIcon m_LeaderCrest;

	// Token: 0x040015B2 RID: 5554
	[UIFieldTarget("id_TargetCrest")]
	private UIKingdomIcon m_TargetCrest;

	// Token: 0x040015B5 RID: 5557
	public DT.Field def;

	// Token: 0x040015B6 RID: 5558
	private Vars vars;

	// Token: 0x040015B7 RID: 5559
	private bool m_Initialized;
}
