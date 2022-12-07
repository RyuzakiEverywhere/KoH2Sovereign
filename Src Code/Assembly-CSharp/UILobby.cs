using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024B RID: 587
public class UILobby : MonoBehaviour, RemoteVars.IListener, IListener, IVars
{
	// Token: 0x170001BB RID: 443
	// (get) Token: 0x060023C6 RID: 9158 RVA: 0x0014248A File Offset: 0x0014068A
	// (set) Token: 0x060023C7 RID: 9159 RVA: 0x00142492 File Offset: 0x00140692
	public Logic.Kingdom SelectedKingdom { get; private set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x060023C8 RID: 9160 RVA: 0x0014249B File Offset: 0x0014069B
	// (set) Token: 0x060023C9 RID: 9161 RVA: 0x001424A3 File Offset: 0x001406A3
	public UICampaignKingdomBrowser SelectionBrowser { get; private set; }

	// Token: 0x060023CA RID: 9162 RVA: 0x001424AC File Offset: 0x001406AC
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_KingdomView != null)
		{
			this.m_KingdomView.AllowSelection(true);
			this.m_KingdomView.onClick = new BSGButton.OnClick(this.SetKingdomView);
		}
		if (this.m_RealmView != null)
		{
			this.m_RealmView.onClick = new BSGButton.OnClick(this.SetRealmView);
			this.m_RealmView.AllowSelection(true);
		}
		if (this.kingdomSelectionPanel != null)
		{
			this.m_KingdomInfo = this.kingdomSelectionPanel.AddComponent<UICampaignKingdomInfo>();
			UICampaignKingdomInfo kingdomInfo = this.m_KingdomInfo;
			kingdomInfo.OnPickKingdom = (Action<Logic.Kingdom, Logic.Realm>)Delegate.Combine(kingdomInfo.OnPickKingdom, new Action<Logic.Kingdom, Logic.Realm>(this.HandleOnPickKingdom));
		}
		if (this.btn_Back != null)
		{
			this.btn_Back.onClick = new BSGButton.OnClick(this.HandleOnBack);
			Tooltip.Get(this.btn_Back.gameObject, true).SetDef("LeaveLobbyTooltip", null);
		}
		if (this.m_Publish != null)
		{
			this.m_Publish.onClick = new BSGButton.OnClick(this.HandleOnPublish);
			Tooltip.Get(this.m_Publish.gameObject, true).SetDef("PublishCampaignTooltip", null);
		}
		if (this.m_Unpublish != null)
		{
			this.m_Unpublish.onClick = new BSGButton.OnClick(this.HandleOnUnpublish);
			Tooltip.Get(this.m_Unpublish.gameObject, true).SetDef("UnpublishCampaignTooltip", null);
		}
		if (this.btn_Ready != null)
		{
			this.btn_Ready.onClick = new BSGButton.OnClick(this.HandleOnReady);
		}
		if (this.btn_NotReady != null)
		{
			this.btn_NotReady.onClick = new BSGButton.OnClick(this.HandleOnReady);
		}
		if (this.btn_Start != null)
		{
			this.btn_Start.onClick = new BSGButton.OnClick(this.HandleOnStart);
		}
		if (this.btn_StopCountdown != null)
		{
			this.btn_StopCountdown.onClick = new BSGButton.OnClick(this.HandleOnStop);
		}
		if (this.m_KingdomList != null)
		{
			this.m_KingdomList.AllowSelection(true);
			this.m_KingdomList.onClick = new BSGButton.OnClick(this.ToggleKingdomBrowser);
		}
		if (this.m_KingdomBrowserContianer != null)
		{
			this.SelectionBrowser = this.m_KingdomBrowserContianer.GetOrAddComponent<UICampaignKingdomBrowser>();
			this.SelectionBrowser.HandelOnClose(null);
		}
		if (this.m_CopyCode != null)
		{
			this.m_CopyCode.onClick = new BSGButton.OnClick(this.HandleOnCopyCode);
			Tooltip.Get(this.m_CopyCode.gameObject, true).SetDef("CopyGameCodeTooltip", new Vars(this));
		}
		this.m_Initialized = true;
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x00142774 File Offset: 0x00140974
	private void LocalizeStatics()
	{
		if (this.m_ReadyLabel != null)
		{
			UIText.SetTextKey(this.m_ReadyLabel, "TitleScreen.Multiplayer.Settings.button_ready", null, null);
		}
		if (this.m_NotReadyLabel != null)
		{
			UIText.SetTextKey(this.m_NotReadyLabel, "TitleScreen.Multiplayer.Settings.button_ready", null, null);
		}
		if (this.m_StartLabel != null)
		{
			UIText.SetTextKey(this.m_StartLabel, "TitleScreen.Multiplayer.Settings.button_start", null, null);
		}
		if (this.m_StopCountdownLabel != null)
		{
			UIText.SetTextKey(this.m_StopCountdownLabel, "TitleScreen.Multiplayer.Settings.button_stop_countdown", null, null);
		}
		if (this.m_GradCampaignDescription != null)
		{
			UIText.SetTextKey(this.m_GradCampaignDescription, "TitleScreen.Multiplayer.Settings.button_start", null, null);
		}
		if (this.m_Countdown_Text != null)
		{
			UIText.SetTextKey(this.m_Countdown_Text, "TitleScreen.Multiplayer.Countdown.starting", null, null);
		}
		if (this.m_GradCampaignLabel != null)
		{
			UIText.SetTextKey(this.m_GradCampaignLabel, "CampaignsWindow.major_victory", null, null);
		}
		if (this.m_MainGoalLabel != null)
		{
			UIText.SetTextKey(this.m_MainGoalLabel, "CampaignsWindow.minor_victory", null, null);
		}
		if (this.m_BackLabel != null)
		{
			UIText.SetTextKey(this.m_BackLabel, "CampaignsWindow.back", null, null);
		}
		if (this.m_CreateLabel != null)
		{
			UIText.SetTextKey(this.m_CreateLabel, "CampaignsWindow.create", null, null);
		}
		if (this.m_PublicLabel != null)
		{
			UIText.SetTextKey(this.m_PublicLabel, "CampaignsWindow.public", null, null);
		}
		if (this.m_PrivateLabel != null)
		{
			UIText.SetTextKey(this.m_PrivateLabel, "CampaignsWindow.private", null, null);
		}
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x00142904 File Offset: 0x00140B04
	private void LocalizeDynamic()
	{
		if (this.m_RealmViewLabel != null)
		{
			UIText.SetTextKey(this.m_RealmViewLabel, "CampaignsWindow.realm", null, null);
		}
		if (this.m_KingdomViewLabel != null)
		{
			UIText.SetTextKey(this.m_KingdomViewLabel, "CampaignsWindow.kingdom", null, null);
		}
		if (this.m_GradCampaignDescription != null)
		{
			UIText.SetTextKey(this.m_GradCampaignDescription, "CampaignsWindow.grand_campaign_description", null, null);
		}
		if (this.m_CopyCodeLabel != null)
		{
			UIText.SetTextKey(this.m_CopyCodeLabel, "CampaignsWindow.copy_code", null, null);
		}
		if (this.m_RulesLabel != null)
		{
			UIText.SetTextKey(this.m_RulesLabel, "CampaignsWindow.rules", new Vars(this), null);
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x001429B8 File Offset: 0x00140BB8
	public void SetCampaign(Campaign campaign)
	{
		this.Init();
		this.LocalizeStatics();
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		this.m_Campaign = campaign;
		Campaign campaign2 = this.m_Campaign;
		if (campaign2 != null)
		{
			campaign2.AddVarsListener(this);
		}
		if (this.m_SettingsPanel != null)
		{
			this.m_SettingsPanel.SetData(this.m_Campaign);
		}
		if (this.m_Players != null)
		{
			this.m_Players.SetCampaign(this.m_Campaign, true);
		}
		this.countdownMaxDuration = campaign.GetVar("countdown_time", null, true);
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.AddListener(this);
		}
		this.Refresh();
		this.m_InvalidatePoliticalData = true;
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x00142A69 File Offset: 0x00140C69
	private void Refresh()
	{
		this.LocalizeDynamic();
		this.UpdateButtons();
		this.UpdateLabels();
		this.PopulateInvateCode();
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x00142A84 File Offset: 0x00140C84
	private void PopulateInvateCode()
	{
		bool flag = this.m_Campaign != null && this.m_Campaign.state < Campaign.State.Started;
		if (this.m_CopyCode != null)
		{
			this.m_CopyCode.Enable(flag, false);
		}
		if (!flag)
		{
			return;
		}
		this.m_InviteCode = MPBoss.Get().GenerateInviteCode(this.m_Campaign);
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x00142AE0 File Offset: 0x00140CE0
	private void OnEnable()
	{
		this.Init();
		Game game = GameLogic.Get(true);
		CampaignUtils.PopulateDummyGame(game, "europe");
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			titleMap.LoadGame(game, false);
		}
		this.kingdomSelectionPanel.gameObject.SetActive(false);
		this.SetKingdomView(null);
		CameraController.LookAt(MapData.Get().GetTerrainBounds().center);
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x00142B48 File Offset: 0x00140D48
	public void SelectKingdom(Logic.Kingdom kingdom, Logic.Realm realm)
	{
		this.Init();
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (this.m_Campaign == null)
		{
			return;
		}
		string a = this.m_Campaign.GetVar("pick_kingdom", null, true).String(null);
		this.m_PickRealm = (a == "province" || a == "random_province");
		if (this.m_PickRealm && realm != null)
		{
			this.SelectedKingdom = game.GetKingdom(realm.name);
		}
		else
		{
			if (kingdom == null && realm != null)
			{
				kingdom = game.GetKingdom(realm.kingdom_id);
			}
			this.SelectedKingdom = kingdom;
		}
		if (this.m_KingdomInfo != null)
		{
			this.m_KingdomInfo.SetKingdom(this.m_Campaign, this.SelectedKingdom, realm);
		}
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x00142C0C File Offset: 0x00140E0C
	private void SetPlayerKingdom(string kingdom_name, string realm_name)
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Kingdom kingdom = game.GetKingdom(kingdom_name);
		if (!CampaignUtils.IsPlayerEligible(game, kingdom))
		{
			kingdom_name = "";
			realm_name = "";
		}
		this.m_Campaign.SetLocalPlayerKingdomName(kingdom_name, realm_name, true);
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x00142C5C File Offset: 0x00140E5C
	private void UpdateCountdown()
	{
		if (!this.inCountdown)
		{
			return;
		}
		this.startCountdownTime -= UnityEngine.Time.deltaTime;
		float num = (float)Math.Ceiling((double)this.startCountdownTime);
		if (this.startCountdownTickTime != num)
		{
			this.startCountdownTickTime = num;
			UIText.SetText(this.m_Countdown_Time, this.startCountdownTickTime.ToString());
		}
		if (this.m_Countdown_RingFull != null)
		{
			this.m_Countdown_RingFull.fillAmount = Mathf.Clamp01(this.startCountdownTime / this.countdownMaxDuration);
		}
		if (this.startCountdownTime < 0f)
		{
			this.inCountdown = false;
			if (this.m_Campaign.IsAuthority())
			{
				MPBoss.Get().StartGame(this.m_Campaign);
			}
		}
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x00142D14 File Offset: 0x00140F14
	private void LateUpdate()
	{
		if (this.m_InvalidatePoliticalData)
		{
			Game game = GameLogic.Get(false);
			if (game != null && this.m_Campaign != null)
			{
				if (CampaignUtils.ChangeGamePoliticalData(game, this.m_Campaign, SaveGame.GetLatestPolitcalData(this.m_Campaign.id)))
				{
					TitleMap titleMap = TitleMap.Get();
					if (titleMap != null)
					{
						titleMap.UpdateGamePoliticalData(game);
					}
				}
				this.ValidateKingdomChoice();
				this.m_Campaign.ValidatePlayersReady(game);
				this.m_InvalidatePoliticalData = false;
			}
		}
		if (this.m_InvalidatePoliticalDataForced)
		{
			Game game2 = GameLogic.Get(false);
			if (game2 != null && this.m_Campaign != null)
			{
				CampaignUtils.ChangeGamePoliticalData(game2, this.m_Campaign, SaveGame.GetLatestPolitcalData(this.m_Campaign.id));
				TitleMap titleMap2 = TitleMap.Get();
				if (titleMap2 != null)
				{
					titleMap2.UpdateGamePoliticalData(game2);
				}
				this.ValidateKingdomChoice();
				this.m_Campaign.ValidatePlayersReady(game2);
				this.m_InvalidatePoliticalDataForced = false;
			}
		}
		if (this.m_InvalidateSelection)
		{
			this.TrySelectPlayerKingdom();
			this.m_InvalidateSelection = false;
		}
		if (this.m_Invalidate)
		{
			this.UpdateButtons();
			this.UpdateLabels();
			this.LocalizeDynamic();
			this.m_Invalidate = false;
		}
		if (this.m_InvalidateKingdomSelection)
		{
			this.ValidateKingdomChoice();
			this.m_Campaign.ValidatePlayersReady(GameLogic.Get(false));
			this.m_InvalidateKingdomSelection = false;
		}
		if (this.m_InvalidateViewFilter)
		{
			DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.m_Campaign, "pick_kingdom");
			if (selectedOption.key.Contains("kingdom"))
			{
				this.SetKingdomView(null);
			}
			if (selectedOption.key.Contains("province"))
			{
				this.SetRealmView(null);
			}
			this.m_InvalidateViewFilter = false;
		}
		this.UpdateCountdown();
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x00142E98 File Offset: 0x00141098
	private void ValidateKingdomChoice()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.SelectionBrowser != null && this.SelectionBrowser.isActiveAndEnabled)
		{
			this.SelectionBrowser.HandelOnClose(null);
		}
		if (this.m_Campaign.state >= Campaign.State.Started)
		{
			return;
		}
		Game game = GameLogic.Get(false);
		DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.m_Campaign, "pick_kingdom");
		string a = (selectedOption != null) ? selectedOption.key : null;
		int localPlayerIndex = this.m_Campaign.GetLocalPlayerIndex(true);
		string kingdomName = this.m_Campaign.GetKingdomName(localPlayerIndex, false);
		if (kingdomName == "random")
		{
			this.SelectKingdom(null, null);
			return;
		}
		if (!CampaignUtils.IsPlayerEligible(game, game.GetKingdom(kingdomName)))
		{
			this.m_Campaign.SetPlayerKingdomName(localPlayerIndex, null, null, true);
			this.SelectKingdom(null, null);
			return;
		}
		if (a == "kingdom")
		{
			Logic.Kingdom kingdom = game.GetKingdom(kingdomName);
			if (!CampaignUtils.IsHistoricalKingdom(kingdom))
			{
				this.m_Campaign.SetPlayerKingdomName(localPlayerIndex, null, null, true);
				this.SelectKingdom(null, null);
				return;
			}
			Logic.Realm realm = game.GetRealm(this.m_Campaign.GetOriginRealmName(localPlayerIndex));
			if (!kingdom.realms.Contains(realm))
			{
				this.m_Campaign.SetPlayerKingdomName(localPlayerIndex, null, null, true);
				this.SelectKingdom(null, null);
				return;
			}
		}
		if (a == "realm" && !this.IsValidRealmName(kingdomName))
		{
			this.m_Campaign.SetPlayerKingdomName(localPlayerIndex, null, null, true);
			return;
		}
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x00142FFC File Offset: 0x001411FC
	private void TrySelectPlayerKingdom()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_Campaign.playerDataPersistent == null)
		{
			return;
		}
		int localPlayerIndexInData = this.m_Campaign.GetLocalPlayerIndexInData();
		if (localPlayerIndexInData < 0 || localPlayerIndexInData >= this.m_Campaign.playerDataPersistent.Length)
		{
			return;
		}
		RemoteVars remoteVars = this.m_Campaign.playerDataPersistent[localPlayerIndexInData];
		string name = remoteVars.GetVar("kingdom_name", null, true).String(null);
		string name2 = remoteVars.GetVar("origin_realm", null, true).String(null);
		Game game = GameLogic.Get(false);
		Logic.Kingdom kingdom = (game != null) ? game.GetKingdom(name) : null;
		Game game2 = GameLogic.Get(false);
		Logic.Realm realm = (game2 != null) ? game2.GetRealm(name2) : null;
		BaseUI.Get<TitleUI>().SelectKingdom((kingdom != null) ? kingdom.id : 0, true);
		this.SelectKingdom(kingdom, realm);
		global::Kingdom kingdom2 = ((kingdom != null) ? kingdom.visuals : null) as global::Kingdom;
		if (kingdom2 != null)
		{
			CameraController.LookAt(kingdom2.CalcBounds().center);
		}
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x001430F4 File Offset: 0x001412F4
	public static string GetStartPromptLabel(Campaign campaign)
	{
		if (campaign == null)
		{
			return null;
		}
		Game game = GameLogic.Get(false);
		bool flag = campaign.IsAuthority();
		string text;
		if (flag)
		{
			text = campaign.ValidateSetup(game);
		}
		else
		{
			text = campaign.ValidateLocalPlayerReady(game, true);
		}
		string str;
		if (!campaign.IsMultiplayerCampaign())
		{
			str = "start_prompts_single_player";
		}
		else if (flag)
		{
			str = "start_prompts_host";
		}
		else
		{
			str = "ready_prompts";
			if (text == null && !campaign.IsLocalPlayerReady())
			{
				text = "not_ready";
			}
		}
		if (text == null)
		{
			text = "ok";
		}
		string text2 = "CampaignsWindow." + str + "." + text;
		if (global::Defs.FindTextField(text2) != null)
		{
			return text2;
		}
		text2 = "CampaignsWindow.commmon_prompts." + text;
		if (global::Defs.FindTextField(text2) != null)
		{
			return text2;
		}
		text2 = "CampaignsWindow.commmon_prompts.unknown_error";
		if (global::Defs.FindTextField(text2) != null)
		{
			return text2;
		}
		return "";
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x001431B8 File Offset: 0x001413B8
	private void UpdateLabels()
	{
		if (this.m_StartPromptLabel != null)
		{
			Game game = GameLogic.Get(false);
			IVars vars = null;
			if (this.m_Campaign != null && this.m_Campaign.state < Campaign.State.Started)
			{
				vars = game.GetKingdom(this.m_Campaign.GetKingdomName(false));
			}
			UIText.SetText(this.m_StartPromptLabel, global::Defs.Localize(UILobby.GetStartPromptLabel(this.m_Campaign), vars, null, true, true));
		}
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x00143224 File Offset: 0x00141424
	private void UpdateButtons()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		Game game = GameLogic.Get(false);
		this.m_Campaign.GetRuntimeState();
		bool flag = this.m_Campaign.IsAuthority();
		bool flag2 = this.m_Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, this.m_Campaign.GetLocalPlayerID(), "start_countdown");
		if (this.m_Publish != null)
		{
			bool active = flag && !this.m_Campaign.is_published && this.m_Campaign.state < Campaign.State.Started;
			this.m_Publish.gameObject.SetActive(active);
			this.m_Publish.Enable(!flag2, false);
			Vars vars = new Vars();
			Tooltip.Get(this.m_Publish.gameObject, true).SetVars(vars);
		}
		if (this.m_Unpublish != null)
		{
			bool active2 = flag && this.m_Campaign.is_published && this.m_Campaign.state < Campaign.State.Started;
			this.m_Unpublish.gameObject.SetActive(active2);
			this.m_Unpublish.Enable(!flag2, false);
			Vars vars2 = new Vars();
			Tooltip.Get(this.m_Unpublish.gameObject, true).SetVars(vars2);
		}
		if (this.btn_Ready != null && this.btn_NotReady != null)
		{
			bool flag3 = !flag;
			bool enable = this.m_Campaign.ValidateLocalPlayerReady(game, true) == null;
			bool flag4 = this.IsReady();
			this.btn_Ready.gameObject.SetActive(flag3 && flag4);
			this.btn_Ready.Enable(enable, false);
			this.btn_NotReady.gameObject.SetActive(flag3 && !flag4);
			this.btn_NotReady.Enable(enable, false);
		}
		if (this.btn_Start != null)
		{
			bool active3 = flag && !flag2;
			bool enable2 = this.m_Campaign.ValidateSetup(game) == null;
			this.btn_Start.gameObject.SetActive(active3);
			this.btn_Start.Enable(enable2, false);
		}
		if (this.btn_StopCountdown != null)
		{
			bool active4 = flag && flag2;
			this.btn_StopCountdown.gameObject.SetActive(active4);
		}
		if (this.btn_Back != null)
		{
			this.btn_Back.Enable(!flag2, false);
		}
		if (this.m_KingdomList != null)
		{
			this.m_KingdomList.SetSelected(this.SelectionBrowser.IsShown(), false);
		}
		this.UpdateViewButtons();
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x0014349F File Offset: 0x0014169F
	private void HandleOnPickKingdom(Logic.Kingdom kingdom, Logic.Realm realm)
	{
		Logic.Kingdom selectedKingdom = this.SelectedKingdom;
		this.SetPlayerKingdom(((selectedKingdom != null) ? selectedKingdom.Name : null) ?? string.Empty, ((realm != null) ? realm.name : null) ?? string.Empty);
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x001434D8 File Offset: 0x001416D8
	private void ToggleKingdomBrowser(BSGButton b)
	{
		if (this.SelectionBrowser != null)
		{
			if (this.SelectionBrowser.IsShown())
			{
				this.SelectionBrowser.Close(false);
				return;
			}
			this.SelectionBrowser.Open(this.m_Campaign, new Action<Logic.Kingdom>(this.OnKingdomBrowseSelected), new Func<Logic.Kingdom, bool>(this.ValidateKingdom));
		}
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x00143536 File Offset: 0x00141736
	private bool ValidateKingdom(Logic.Kingdom k)
	{
		return this.m_Campaign == null || (k != null && !CampaignUtils.IsBlackListedKingdom(this.m_Campaign, k));
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x00143558 File Offset: 0x00141758
	private void OnKingdomBrowseSelected(Logic.Kingdom k)
	{
		if (k == null)
		{
			return;
		}
		Logic.Realm realm;
		if (CampaignUtils.IsRealmKingdom(k))
		{
			realm = CampaignUtils.GetRealmOfRealmKingdom(k);
			if (((realm != null) ? realm.visuals : null) != null)
			{
				CameraController.LookAt((realm.visuals as global::Realm).bounds.center);
			}
		}
		else
		{
			realm = ((k.realms.Count > 0) ? k.realms[0] : null);
			if (((k != null) ? k.visuals : null) != null)
			{
				CameraController.LookAt((k.visuals as global::Kingdom).CalcBounds().center);
			}
		}
		TitleUI titleUI = BaseUI.Get<TitleUI>();
		if (titleUI == null)
		{
			return;
		}
		titleUI.SetSelectedRealm((realm != null) ? realm.id : 0, true);
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x0014360C File Offset: 0x0014180C
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "owner_deleted_campaign")
		{
			Vars vars = new Vars();
			vars.Set<string>("host", this.m_Campaign.GetOwnerName());
			MessageWnd.Create(global::Defs.GetDefField("ClientLeaveCampaingMessage", null), vars, null, null);
			this.CloseLobby();
			return;
		}
		if (message == "kicked_from_campaign")
		{
			Vars vars2 = new Vars();
			vars2.Set<string>("host", this.m_Campaign.GetOwnerName());
			MessageWnd.Create(global::Defs.GetDefField("ClientKickedFromCampaingMessage", null), vars2, null, null);
			this.CloseLobby();
			return;
		}
		if (!(message == "political_data_changed"))
		{
			return;
		}
		string a = param as string;
		Campaign campaign = this.m_Campaign;
		if (!(a != ((campaign != null) ? campaign.id : null)))
		{
			this.m_InvalidatePoliticalDataForced = true;
		}
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x001436D8 File Offset: 0x001418D8
	private void HandleOnBack(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		if (this.m_Campaign == null)
		{
			this.CloseLobby();
			return;
		}
		if (this.m_Campaign.state >= Campaign.State.Started || !this.m_Campaign.IsAuthority())
		{
			this.CloseLobby();
			return;
		}
		MessageWnd.Create(global::Defs.GetDefField("HostLeaveCampaingQuerryMessage", null), null, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				this.CloseLobby();
				this.m_Campaign.Reset();
			}
			wnd.Close(false);
			return true;
		});
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x0014373C File Offset: 0x0014193C
	private void HandleOnPublish(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.DoPublish();
		this.m_Invalidate = true;
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x00143752 File Offset: 0x00141952
	private void HandleOnUnpublish(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.DoUnpublish();
		this.m_Invalidate = true;
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x00143768 File Offset: 0x00141968
	private void HandleOnReady(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		bool selected = this.IsReady();
		this.btn_Ready.SetSelected(selected, false);
		this.DoToggleReady();
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x00143796 File Offset: 0x00141996
	private void HandleOnStop(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.DoStop();
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x001437A5 File Offset: 0x001419A5
	private void HandleOnStart(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.DoStart();
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x001437B4 File Offset: 0x001419B4
	private void HandleOnCopyCode(BSGButton btn)
	{
		Game.CopyToClipboard(this.m_InviteCode);
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x001437C1 File Offset: 0x001419C1
	private bool IsReady()
	{
		return this.m_Campaign != null && this.m_Campaign.GetRuntimeState() == Campaign.RuntimeState.Lobby && this.m_Campaign.IsLocalPlayerReady();
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x001437E8 File Offset: 0x001419E8
	public void Clear()
	{
		Game game = GameLogic.Get(true);
		if (game != null)
		{
			CampaignUtils.ClearMultiplayerRegistry((game != null) ? game.multiplayer : null);
			CampaignUtils.UnloadMap(game);
		}
		if (this.SelectionBrowser != null)
		{
			this.SelectionBrowser.Close(false);
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.DelListener(this);
		}
		TooltipInstance.RemovePinnedTooltips();
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x00143846 File Offset: 0x00141A46
	public void CloseLobby()
	{
		this.DoLeave();
		this.Clear();
		BaseUI.Get<TitleUI>().DisableLobbyScreen();
		BaseUI.Get<TitleUI>().EnableMultiplayerMenu(this.m_Campaign);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x0014386E File Offset: 0x00141A6E
	public void DoPublish()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		MPBoss.Get().SetLobbyType(this.m_Campaign, true);
		this.m_Campaign.is_published = true;
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x00143896 File Offset: 0x00141A96
	public void DoUnpublish()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		MPBoss.Get().SetLobbyType(this.m_Campaign, false);
		this.m_Campaign.is_published = false;
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x001438BE File Offset: 0x00141ABE
	public void DoLeave()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		MPBoss.Get().LeaveLobby(this.m_Campaign, true);
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x001438DB File Offset: 0x00141ADB
	public void DoToggleReady()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		MPBoss.Get().ToggleReady(this.m_Campaign);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x001438F6 File Offset: 0x00141AF6
	public void DoStart()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		this.m_Campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, this.m_Campaign.GetLocalPlayerIndex(true), "start_countdown", true, true);
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x00143926 File Offset: 0x00141B26
	public void DoStop()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		this.InteruptCountdown(this.m_Campaign.GetLocalPlayerID());
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x00143944 File Offset: 0x00141B44
	public void EnableCampaignRulesPreserveChoiceWindow(bool enabled, Action<string> onUserChoice)
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_CampaignRulesPreserveChoice", true, true);
		if (gameObject == null)
		{
			Debug.LogWarning("CampaignRulesPreserveChoice window is missing!");
			return;
		}
		UICampaignRulesPreserveChoice component = global::Common.GetComponent<UICampaignRulesPreserveChoice>(gameObject, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
		}
		gameObject.SetActive(enabled);
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x00143997 File Offset: 0x00141B97
	private void SetKingdomView(BSGButton b)
	{
		ViewMode.titleKingdomsView.Apply(true);
		ViewMode.titleKingdomsView.SetSubMode(ViewMode.SubMode.Kingdom);
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			titleMap.ViewModeChanged();
		}
		this.UpdateViewButtons();
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x001439C5 File Offset: 0x00141BC5
	private void SetRealmView(BSGButton b)
	{
		ViewMode.titleKingdomsView.Apply(true);
		ViewMode.titleKingdomsView.SetSubMode(ViewMode.SubMode.Realm);
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			titleMap.ViewModeChanged();
		}
		this.UpdateViewButtons();
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x001439F4 File Offset: 0x00141BF4
	private void UpdateViewButtons()
	{
		if (ViewMode.current == null)
		{
			return;
		}
		bool flag = ViewMode.current.GetSubMode() == ViewMode.SubMode.Kingdom;
		if (this.m_KingdomView != null)
		{
			this.m_KingdomView.SetSelected(flag, true);
		}
		if (this.m_RealmView != null)
		{
			this.m_RealmView.SetSelected(!flag, true);
		}
		if (this.m_KingdomViewLabel != null)
		{
			this.m_KingdomViewLabel.color = global::Defs.GetColor("CampaignsWindow", flag ? "filter_label_color_selected" : "filter_label_color_normal");
		}
		if (this.m_RealmViewLabel != null)
		{
			this.m_RealmViewLabel.color = global::Defs.GetColor("CampaignsWindow", (!flag) ? "filter_label_color_selected" : "filter_label_color_normal");
		}
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x00143AB4 File Offset: 0x00141CB4
	private bool IsValidKingdomName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		Game game = GameLogic.Get(false);
		return game != null && game.kingdoms_by_name != null && game.kingdoms_by_name.ContainsKey(name);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x00143AF0 File Offset: 0x00141CF0
	private bool IsValidRealmName(string name)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return false;
		}
		for (int i = 0; i < game.realms.Count; i++)
		{
			if (game.realms[i].name == name)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x00143B3C File Offset: 0x00141D3C
	public void InteruptCountdown(string by_player_id)
	{
		if (!this.inCountdown || !this.m_Campaign.IsAuthority())
		{
			return;
		}
		this.inCountdown = false;
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.SendInteruptCountdown(this.m_Campaign, by_player_id);
		}
		UILobbyChat.AddInteruptCountdownMessage(this.m_Campaign, by_player_id);
		this.m_Campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, this.m_Campaign.GetLocalPlayerIndex(true), "start_countdown", false, true);
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x00143BB0 File Offset: 0x00141DB0
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (!(this == null))
		{
			this.m_Invalidate = true;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num > 1712242932U)
			{
				if (num <= 3514014253U)
				{
					if (num != 2615209153U)
					{
						if (num != 3514014253U)
						{
							return;
						}
						if (!(key == "realms_goal"))
						{
							return;
						}
					}
					else
					{
						if (!(key == "start_period"))
						{
							return;
						}
						goto IL_115;
					}
				}
				else if (num != 3639724134U)
				{
					if (num != 4277568609U)
					{
						return;
					}
					if (!(key == "from_save_id"))
					{
						return;
					}
					this.m_InvalidatePoliticalDataForced = true;
					this.m_InvalidateKingdomSelection = true;
					return;
				}
				else if (!(key == "main_goal"))
				{
					return;
				}
				this.m_InvalidateKingdomSelection = true;
				return;
			}
			if (num <= 999807643U)
			{
				if (num != 443087763U)
				{
					if (num != 999807643U)
					{
						return;
					}
					if (!(key == "start_countdown"))
					{
						return;
					}
					this.inCountdown = new_val;
					if (this.inCountdown)
					{
						this.startCountdownTime = this.countdownMaxDuration;
						this.startCountdownTickTime = -1f;
						if (this.m_Countdown_RingFX != null)
						{
							this.m_Countdown_RingFX.duration = this.countdownMaxDuration;
							this.m_Countdown_RingFX.ResetToBeginning();
						}
					}
					this.m_Group_Countdown.SetActive(this.inCountdown);
					BSGButton bsgbutton = this.btn_Start;
					if (bsgbutton != null)
					{
						bsgbutton.gameObject.SetActive(!this.inCountdown);
					}
					BSGButton bsgbutton2 = this.btn_StopCountdown;
					if (bsgbutton2 == null)
					{
						return;
					}
					bsgbutton2.gameObject.SetActive(this.inCountdown);
					return;
				}
				else if (!(key == "map_size"))
				{
					return;
				}
			}
			else if (num != 1515052130U)
			{
				if (num != 1712242932U)
				{
					return;
				}
				if (!(key == "ready"))
				{
					return;
				}
				this.InteruptCountdown(vars.GetPlayerID());
				return;
			}
			else
			{
				if (!(key == "pick_kingdom"))
				{
					return;
				}
				this.m_InvalidateKingdomSelection = true;
				this.m_InvalidateViewFilter = true;
				return;
			}
			IL_115:
			this.m_InvalidateKingdomSelection = true;
			this.m_InvalidatePoliticalData = true;
			return;
		}
		Campaign campaign = this.m_Campaign;
		if (campaign == null)
		{
			return;
		}
		campaign.DelVarsListener(this);
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x00143DA0 File Offset: 0x00141FA0
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "game_code")
		{
			return "#" + this.m_InviteCode;
		}
		if (key == "can_enter")
		{
			return this.m_Campaign != null && this.m_Campaign.state < Campaign.State.Started;
		}
		if (!(key == "is_game_from_save"))
		{
			return Value.Unknown;
		}
		Campaign campaign = this.m_Campaign;
		return campaign != null && campaign.IsFromSave();
	}

	// Token: 0x04001810 RID: 6160
	[UIFieldTarget("id_KingdomView")]
	private BSGButton m_KingdomView;

	// Token: 0x04001811 RID: 6161
	[UIFieldTarget("id_KingdomViewLabel")]
	private TextMeshProUGUI m_KingdomViewLabel;

	// Token: 0x04001812 RID: 6162
	[UIFieldTarget("id_RealmView")]
	private BSGButton m_RealmView;

	// Token: 0x04001813 RID: 6163
	[UIFieldTarget("id_RealmViewLabel")]
	private TextMeshProUGUI m_RealmViewLabel;

	// Token: 0x04001814 RID: 6164
	[UIFieldTarget("id_KingdomList")]
	private BSGButton m_KingdomList;

	// Token: 0x04001815 RID: 6165
	[UIFieldTarget("id_KingdomBrowser")]
	private GameObject m_KingdomBrowserContianer;

	// Token: 0x04001816 RID: 6166
	[UIFieldTarget("id_KingdomSelectionPanel")]
	private GameObject kingdomSelectionPanel;

	// Token: 0x04001817 RID: 6167
	[UIFieldTarget("id_GradCampaignDescription")]
	private TextMeshProUGUI m_GradCampaignDescription;

	// Token: 0x04001818 RID: 6168
	[UIFieldTarget("id_StartPromptLabel")]
	private TextMeshProUGUI m_StartPromptLabel;

	// Token: 0x04001819 RID: 6169
	[UIFieldTarget("id_Back")]
	private BSGButton btn_Back;

	// Token: 0x0400181A RID: 6170
	[UIFieldTarget("id_Public")]
	private BSGButton m_Publish;

	// Token: 0x0400181B RID: 6171
	[UIFieldTarget("id_PublicLabel")]
	private TextMeshProUGUI m_PublicLabel;

	// Token: 0x0400181C RID: 6172
	[UIFieldTarget("id_Private")]
	private BSGButton m_Unpublish;

	// Token: 0x0400181D RID: 6173
	[UIFieldTarget("id_PrivateLabel")]
	private TextMeshProUGUI m_PrivateLabel;

	// Token: 0x0400181E RID: 6174
	[UIFieldTarget("btn_Ready")]
	private BSGButton btn_Ready;

	// Token: 0x0400181F RID: 6175
	[UIFieldTarget("id_ReadyLabel")]
	private TextMeshProUGUI m_ReadyLabel;

	// Token: 0x04001820 RID: 6176
	[UIFieldTarget("btn_NotReady")]
	private BSGButton btn_NotReady;

	// Token: 0x04001821 RID: 6177
	[UIFieldTarget("id_NotReadyLabel")]
	private TextMeshProUGUI m_NotReadyLabel;

	// Token: 0x04001822 RID: 6178
	[UIFieldTarget("btn_Start")]
	private BSGButton btn_Start;

	// Token: 0x04001823 RID: 6179
	[UIFieldTarget("id_StartLabel")]
	private TextMeshProUGUI m_StartLabel;

	// Token: 0x04001824 RID: 6180
	[UIFieldTarget("btn_StopCountdown")]
	private BSGButton btn_StopCountdown;

	// Token: 0x04001825 RID: 6181
	[UIFieldTarget("id_StopCountdownLabel")]
	private TextMeshProUGUI m_StopCountdownLabel;

	// Token: 0x04001826 RID: 6182
	[UIFieldTarget("id_SettingsPanel")]
	private UICampaignSettings m_SettingsPanel;

	// Token: 0x04001827 RID: 6183
	[UIFieldTarget("id_Players")]
	private UILobbyPlayers m_Players;

	// Token: 0x04001828 RID: 6184
	[UIFieldTarget("id_Group_Countdown")]
	private GameObject m_Group_Countdown;

	// Token: 0x04001829 RID: 6185
	[UIFieldTarget("id_Countdown_RingFull")]
	private Image m_Countdown_RingFull;

	// Token: 0x0400182A RID: 6186
	[UIFieldTarget("id_Countdown_RingFX")]
	private TweenRotation m_Countdown_RingFX;

	// Token: 0x0400182B RID: 6187
	[UIFieldTarget("id_Countdown_Time")]
	private TextMeshProUGUI m_Countdown_Time;

	// Token: 0x0400182C RID: 6188
	[UIFieldTarget("id_Countdown_Text")]
	private TextMeshProUGUI m_Countdown_Text;

	// Token: 0x0400182D RID: 6189
	[UIFieldTarget("id_GradCampaignLabel")]
	private TextMeshProUGUI m_GradCampaignLabel;

	// Token: 0x0400182E RID: 6190
	[UIFieldTarget("id_MainGoalLabel")]
	private TextMeshProUGUI m_MainGoalLabel;

	// Token: 0x0400182F RID: 6191
	[UIFieldTarget("id_RulesLabel")]
	private TextMeshProUGUI m_RulesLabel;

	// Token: 0x04001830 RID: 6192
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI m_BackLabel;

	// Token: 0x04001831 RID: 6193
	[UIFieldTarget("id_CreateLabel")]
	private TextMeshProUGUI m_CreateLabel;

	// Token: 0x04001832 RID: 6194
	[UIFieldTarget("id_PrivateCode")]
	private GameObject m_PrivateCode;

	// Token: 0x04001833 RID: 6195
	[UIFieldTarget("id_CopyCode")]
	private BSGButton m_CopyCode;

	// Token: 0x04001834 RID: 6196
	[UIFieldTarget("id_CopyCodeLabel")]
	private TextMeshProUGUI m_CopyCodeLabel;

	// Token: 0x04001837 RID: 6199
	public UICampaignKingdomInfo m_KingdomInfo;

	// Token: 0x04001838 RID: 6200
	private Campaign m_Campaign;

	// Token: 0x04001839 RID: 6201
	private bool m_PickRealm;

	// Token: 0x0400183A RID: 6202
	private bool m_Initialized;

	// Token: 0x0400183B RID: 6203
	private bool m_Invalidate;

	// Token: 0x0400183C RID: 6204
	private bool m_InvalidateKingdomSelection;

	// Token: 0x0400183D RID: 6205
	private bool m_InvalidateViewFilter;

	// Token: 0x0400183E RID: 6206
	private bool m_InvalidatePoliticalData;

	// Token: 0x0400183F RID: 6207
	private bool m_InvalidatePoliticalDataForced;

	// Token: 0x04001840 RID: 6208
	private bool m_InvalidateSelection;

	// Token: 0x04001841 RID: 6209
	private string m_InviteCode;

	// Token: 0x04001842 RID: 6210
	private float countdownMaxDuration = 10f;

	// Token: 0x04001843 RID: 6211
	private float startCountdownTime;

	// Token: 0x04001844 RID: 6212
	private float startCountdownTickTime;

	// Token: 0x04001845 RID: 6213
	private bool inCountdown;
}
