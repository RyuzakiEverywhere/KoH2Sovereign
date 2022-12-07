using System;
using Logic;
using UnityEngine.UI;

// Token: 0x020002D4 RID: 724
public class UICampaignVerisonIcon : ObjectIcon
{
	// Token: 0x06002DE0 RID: 11744 RVA: 0x0017C9CF File Offset: 0x0017ABCF
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		Tooltip.Get(base.gameObject, true).SetDef("CampaignVersionIncompatibleTooltip", new Vars(this));
		this.m_Initiazlied = false;
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x0017CA04 File Offset: 0x0017AC04
	public override void SetObject(object campaign_verison, Vars vars = null)
	{
		base.SetObject(campaign_verison, vars);
		this.SetData(campaign_verison);
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x0017CA15 File Offset: 0x0017AC15
	private void SetData(object verison)
	{
		this.Init();
		this.m_CampaignVerson = (string)verison;
		this.Refresh();
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x0017CA30 File Offset: 0x0017AC30
	private void Refresh()
	{
		base.gameObject.SetActive(true);
		if (this.m_Icon != null)
		{
			bool active = this.IsVerisonMismatch();
			this.m_Icon.gameObject.SetActive(active);
		}
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x0017CA6F File Offset: 0x0017AC6F
	public bool IsVerisonMismatch()
	{
		return Title.Version(true) != this.m_CampaignVerson;
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x0017CA84 File Offset: 0x0017AC84
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "campaign_game_version")
		{
			return "#" + this.m_CampaignVerson;
		}
		if (!(key == "player_game_version"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return "#" + Title.Version(true);
	}

	// Token: 0x04001F10 RID: 7952
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001F11 RID: 7953
	private bool m_Initiazlied;

	// Token: 0x04001F12 RID: 7954
	private string m_CampaignVerson;
}
