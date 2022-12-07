using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class UIMultiplayerMenu : UIWindow
{
	// Token: 0x0600242E RID: 9262 RVA: 0x00145483 File Offset: 0x00143683
	public override string GetDefId()
	{
		return UIMultiplayerMenu.def_id;
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x0014548C File Offset: 0x0014368C
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleClose);
		}
		if (this.m_AbandonCampaign != null)
		{
			this.m_AbandonCampaign.onClick = new BSGButton.OnClick(this.HandleOnAbandon);
		}
		if (this.m_EnterLobby != null)
		{
			this.m_EnterLobby.onClick = new BSGButton.OnClick(this.HandleOnEnter);
		}
		if (this.m_JoinGame != null)
		{
			this.m_JoinGame.onClick = new BSGButton.OnClick(this.HandleOnJoin);
		}
		if (this.m_Campaigns != null)
		{
			this.m_Campaigns.OnCampaiognSelect = new Action<Campaign>(this.HandleOnCampaignSelected);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x0014556C File Offset: 0x0014376C
	private void LocalazeStatics()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "MultiplayerMenu.caption", null, null);
		}
		if (this.m_NoSelectionDescription != null)
		{
			UIText.SetTextKey(this.m_NoSelectionDescription, "MultiplayerMenu.not_selection_description", null, null);
		}
		if (this.m_AbandonCampaignLabel != null)
		{
			UIText.SetTextKey(this.m_AbandonCampaignLabel, "MultiplayerMenu.leave_campaign", null, null);
		}
		if (this.m_EnterLobbylabel != null)
		{
			UIText.SetTextKey(this.m_EnterLobbylabel, "MultiplayerMenu.enter_lobby", null, null);
		}
		if (this.m_JoinGameLabel != null)
		{
			UIText.SetTextKey(this.m_JoinGameLabel, "MultiplayerMenu.join_game", null, null);
		}
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x0014561C File Offset: 0x0014381C
	private void OnEnable()
	{
		this.Init();
		this.m_Campaigns.SetCampaigns(this);
		this.m_Campaigns.gameObject.SetActive(true);
		UILobbyPlayers playersInfo = this.m_PlayersInfo;
		if (playersInfo != null)
		{
			playersInfo.gameObject.SetActive(false);
		}
		this.LocalazeStatics();
		this.UpdateJoinTooltip(null, false);
		this.UpdateState();
		this.m_Campaigns.TrySelectBestOption();
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x00145682 File Offset: 0x00143882
	protected override void Update()
	{
		base.Update();
		if (this.m_LastUpdate + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
		{
			this.UpdateState();
			this.m_LastUpdate = UnityEngine.Time.unscaledTime;
		}
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x001456B0 File Offset: 0x001438B0
	private void UpdateState()
	{
		Campaign selectedCampaign = this.GetSelectedCampaign();
		bool flag = selectedCampaign == null || selectedCampaign.state == Campaign.State.Empty;
		int player_index = (selectedCampaign != null) ? selectedCampaign.GetLocalPlayerIndexInData() : -1;
		bool flag2 = this.MatchVersionWithHost(selectedCampaign);
		bool flag3 = ((selectedCampaign != null) ? selectedCampaign.GetSlotState(player_index) : Campaign.SlotState.Empty) == Campaign.SlotState.Eliminated;
		bool flag4 = selectedCampaign != null && selectedCampaign.state > Campaign.State.Empty && selectedCampaign.state < Campaign.State.Closed && !flag3;
		bool active = selectedCampaign != null && selectedCampaign.GetRuntimeState() > Campaign.RuntimeState.Lobby && flag4 && !flag3;
		if (this.m_EnterLobby != null)
		{
			this.m_EnterLobby.gameObject.SetActive(flag4);
		}
		if (this.m_JoinGame != null)
		{
			this.m_JoinGame.gameObject.SetActive(active);
			this.m_JoinGame.Enable(flag2, false);
			this.UpdateJoinTooltip(selectedCampaign, !flag2);
		}
		if (this.m_AbandonCampaign != null)
		{
			this.m_AbandonCampaign.gameObject.SetActive(!flag);
		}
		if (this.m_NoSelectionDescription != null)
		{
			this.m_NoSelectionDescription.gameObject.SetActive(flag);
		}
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x001457D4 File Offset: 0x001439D4
	private void UpdateJoinTooltip(Campaign campaign, bool showGameVersionMismatchTooltip)
	{
		if (this.m_JoinGame == null)
		{
			return;
		}
		if (!showGameVersionMismatchTooltip || campaign == null)
		{
			global::Common.DestroyObj(Tooltip.Get(this.m_JoinGame.gameObject, false));
			return;
		}
		if (Tooltip.Get(this.m_JoinGame.gameObject, false) != null)
		{
			return;
		}
		Vars vars = new Vars();
		string val = campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, campaign.GetAuthorityPlayerID(), "client_version");
		vars.Set<string>("campaign_game_version", val);
		vars.Set<string>("player_game_version", Title.Version(true));
		Tooltip.Get(this.m_JoinGame.gameObject, true).SetDef("CampaignVersionIncompatibleTooltip", null);
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x0014587C File Offset: 0x00143A7C
	private bool MatchVersionWithHost(Campaign campaign)
	{
		if (campaign == null)
		{
			return false;
		}
		string b = campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, campaign.GetAuthorityPlayerID(), "client_version");
		return Title.Version(true) == b;
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x001458B2 File Offset: 0x00143AB2
	public Campaign GetSelectedCampaign()
	{
		if (this.m_Campaigns == null)
		{
			Debug.LogError("m_Campaigns is null!");
			return null;
		}
		return this.m_Campaigns.selected;
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x001458D9 File Offset: 0x00143AD9
	private void HandleOnCampaignSelected(Campaign campaign)
	{
		UILobbyPlayers playersInfo = this.m_PlayersInfo;
		if (playersInfo != null)
		{
			playersInfo.gameObject.SetActive(campaign != null && campaign.state > Campaign.State.Empty);
		}
		UILobbyPlayers playersInfo2 = this.m_PlayersInfo;
		if (playersInfo2 != null)
		{
			playersInfo2.SetCampaign(campaign, false);
		}
		this.UpdateState();
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x0014591C File Offset: 0x00143B1C
	private void HandleOnEnter(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		if (this.m_Campaigns.selected == null)
		{
			return;
		}
		if (this.m_Campaigns.selected.GetRuntimeState() > Campaign.RuntimeState.Lobby)
		{
			this.JoinRunningGame();
			return;
		}
		this.m_Campaigns.DoEnterLobby(delegate
		{
			Campaign selectedCampaign = this.GetSelectedCampaign();
			BaseUI.Get<TitleUI>().EnableLobbyScreen(selectedCampaign);
			selectedCampaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, selectedCampaign.GetLocalPlayerID(), "start_countdown", false, true);
		});
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x0014596F File Offset: 0x00143B6F
	private void HandleOnJoin(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.JoinRunningGame();
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x00145980 File Offset: 0x00143B80
	private void JoinRunningGame()
	{
		Campaign selectedCampaign = this.GetSelectedCampaign();
		Logic.Coroutine.Start("JoinRunningGame", MPBoss.Get().JoinRunningGame(selectedCampaign), null);
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x001459AC File Offset: 0x00143BAC
	private void HandleOnAbandon(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		Campaign currentCampaign = this.m_Campaigns.selected;
		MessageWnd.Create(global::Defs.GetDefField("AbandonCampaingQuerryMessage", null), null, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				this.m_Campaigns.DoDelete(currentCampaign);
			}
			wnd.Close(false);
			return true;
		});
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleClose(BSGButton btn)
	{
		this.Close(false);
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x001459FD File Offset: 0x00143BFD
	public override void Open()
	{
		base.Open();
		Transform parent = base.gameObject.transform.parent;
		GameObject gameObject = (parent != null) ? parent.gameObject : null;
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(true);
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x00145A2C File Offset: 0x00143C2C
	public override void Close(bool silent = false)
	{
		if (!base.IsShown())
		{
			return;
		}
		base.Hide(silent);
		Transform parent = base.gameObject.transform.parent;
		GameObject gameObject = (parent != null) ? parent.gameObject : null;
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(false);
	}

	// Token: 0x04001879 RID: 6265
	private static string def_id = "MultiplayerMenuWindow";

	// Token: 0x0400187A RID: 6266
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x0400187B RID: 6267
	[UIFieldTarget("id_Campaigns")]
	private UICampaigns m_Campaigns;

	// Token: 0x0400187C RID: 6268
	[UIFieldTarget("id_SelectedCampaign")]
	private UILobbyPlayers m_PlayersInfo;

	// Token: 0x0400187D RID: 6269
	[UIFieldTarget("id_AbandonCampaign")]
	private BSGButton m_AbandonCampaign;

	// Token: 0x0400187E RID: 6270
	[UIFieldTarget("id_AbandonCampaignLabel")]
	private TextMeshProUGUI m_AbandonCampaignLabel;

	// Token: 0x0400187F RID: 6271
	[UIFieldTarget("id_EnterLobby")]
	private BSGButton m_EnterLobby;

	// Token: 0x04001880 RID: 6272
	[UIFieldTarget("id_EnterLabel")]
	private TextMeshProUGUI m_EnterLobbylabel;

	// Token: 0x04001881 RID: 6273
	[UIFieldTarget("id_JoinGame")]
	private BSGButton m_JoinGame;

	// Token: 0x04001882 RID: 6274
	[UIFieldTarget("id_JoinGameLabel")]
	private TextMeshProUGUI m_JoinGameLabel;

	// Token: 0x04001883 RID: 6275
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001884 RID: 6276
	[UIFieldTarget("id_NoSelectionDescription")]
	private TextMeshProUGUI m_NoSelectionDescription;

	// Token: 0x04001885 RID: 6277
	private float m_LastUpdate;

	// Token: 0x04001886 RID: 6278
	private float m_UpdateInterval = 0.5f;
}
