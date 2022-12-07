using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class UICampaigns : MonoBehaviour
{
	// Token: 0x060023B8 RID: 9144 RVA: 0x00142094 File Offset: 0x00140294
	public void SetCampaigns(UIMultiplayerMenu parent)
	{
		this.parent = parent;
		this.Init();
		this.RefreshCampaigns();
		this.Select(null);
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x001420B0 File Offset: 0x001402B0
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_CampaignPrototype != null)
		{
			this.m_CampaignPrototype.gameObject.SetActive(false);
		}
		this.m_Initialized = true;
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x001420E8 File Offset: 0x001402E8
	public void RefreshCampaigns()
	{
		if (this.m_CampaignContainer == null)
		{
			return;
		}
		if (this.m_CampaignPrototype == null)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.m_CampaignContainer);
		this.m_CampaignSlots.Clear();
		MPBoss mpboss = MPBoss.Get();
		Campaign[] array = (mpboss != null) ? mpboss.multiplayerCampaigns : null;
		if (array == null)
		{
			return;
		}
		foreach (Campaign campaign in array)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_CampaignPrototype, this.m_CampaignContainer);
			UICampaign component = gameObject.GetComponent<UICampaign>();
			component.parent = this;
			component.SetCampaign(campaign);
			gameObject.SetActive(true);
			List<UICampaign> campaignSlots = this.m_CampaignSlots;
			if (campaignSlots != null)
			{
				campaignSlots.Add(component);
			}
		}
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x00142198 File Offset: 0x00140398
	public void Select(Campaign campaign)
	{
		this.selected = campaign;
		for (int i = 0; i < this.m_CampaignSlots.Count; i++)
		{
			Campaign campaign2 = this.m_CampaignSlots[i].campaign;
			if (campaign2 != null)
			{
				this.m_CampaignSlots[i].Select(campaign2 == this.selected);
			}
		}
		if (this.OnCampaiognSelect != null)
		{
			this.OnCampaiognSelect(this.selected);
		}
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x0014220C File Offset: 0x0014040C
	public void TrySelectBestOption()
	{
		if (this.m_CampaignSlots == null)
		{
			return;
		}
		if (this.m_CampaignSlots.Count == 0)
		{
			return;
		}
		if (this.IsEmpty())
		{
			this.Select(this.m_CampaignSlots[0].campaign);
			return;
		}
		Campaign campaign = this.FindLatestUnfinishedCampaign();
		if (campaign != null)
		{
			this.Select(campaign);
			return;
		}
		Campaign campaign2 = this.FindFirstFreeCampaign();
		if (campaign2 != null)
		{
			this.Select(campaign2);
			return;
		}
		this.Select(this.m_CampaignSlots[0].campaign);
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x0014228C File Offset: 0x0014048C
	private Campaign FindLatestUnfinishedCampaign()
	{
		Campaign campaign = null;
		for (int i = 0; i < this.m_CampaignSlots.Count; i++)
		{
			Campaign campaign2 = this.m_CampaignSlots[i].campaign;
			if (campaign2.state == Campaign.State.Started)
			{
				if (campaign == null)
				{
					campaign = campaign2;
				}
				else
				{
					long num = this.FindLatesGampaignSaveTime(campaign);
					long num2 = this.FindLatesGampaignSaveTime(campaign2);
					if (num <= num2)
					{
						campaign = campaign2;
					}
				}
			}
		}
		return campaign;
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x001422E8 File Offset: 0x001404E8
	private long FindLatesGampaignSaveTime(Campaign campaign)
	{
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign.id);
		if (campaignInfo == null)
		{
			return 0L;
		}
		return campaignInfo.last_save_date_time.Ticks;
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x00142318 File Offset: 0x00140518
	private Campaign FindFirstFreeCampaign()
	{
		for (int i = 0; i < this.m_CampaignSlots.Count; i++)
		{
			Campaign campaign = this.m_CampaignSlots[i].campaign;
			if (campaign.state == Campaign.State.Empty)
			{
				return campaign;
			}
		}
		return null;
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x00142358 File Offset: 0x00140558
	public void Close()
	{
		this.m_CampaignSlots.Clear();
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x00142368 File Offset: 0x00140568
	public void DoEnterLobby(Action onComplete = null)
	{
		if (this.selected == null)
		{
			return;
		}
		this.DoLeaveAll(this.selected);
		if (this.selected.state != Campaign.State.Empty && MPBoss.Get().EnterLobby(this.selected, null) && this.selected.GetLocalPlayerIndex(true) > -1)
		{
			onComplete();
		}
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x001423BF File Offset: 0x001405BF
	public void DoDelete(Campaign c)
	{
		if (c == null)
		{
			return;
		}
		MPBoss.Get().LeaveCampaign(c, "deleted");
		this.RefreshCampaigns();
		this.Select(null);
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x001423E4 File Offset: 0x001405E4
	public void DoLeaveAll(Campaign ignore_client = null)
	{
		MPBoss mpboss = MPBoss.Get();
		Campaign[] array = (mpboss != null) ? mpboss.multiplayerCampaigns : null;
		if (array == null)
		{
			return;
		}
		foreach (Campaign campaign in array)
		{
			if (campaign != ignore_client && campaign.GetLocalPlayerIndex(true) >= 0)
			{
				MPBoss.Get().LeaveLobby(campaign, true);
			}
		}
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x00142438 File Offset: 0x00140638
	public bool IsEmpty()
	{
		MPBoss mpboss = MPBoss.Get();
		Campaign[] array = (mpboss != null) ? mpboss.multiplayerCampaigns : null;
		if (array == null)
		{
			return true;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].state != Campaign.State.Empty)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04001809 RID: 6153
	[UIFieldTarget("id_Campaigns")]
	private RectTransform m_CampaignContainer;

	// Token: 0x0400180A RID: 6154
	[UIFieldTarget("id_CampaignPrototype")]
	private GameObject m_CampaignPrototype;

	// Token: 0x0400180B RID: 6155
	public Action<Campaign> OnCampaiognSelect;

	// Token: 0x0400180C RID: 6156
	public Campaign selected;

	// Token: 0x0400180D RID: 6157
	public List<UICampaign> m_CampaignSlots = new List<UICampaign>();

	// Token: 0x0400180E RID: 6158
	public UIMultiplayerMenu parent;

	// Token: 0x0400180F RID: 6159
	private bool m_Initialized;
}
