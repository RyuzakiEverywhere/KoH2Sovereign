using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class UISingleplayerCampaign : MonoBehaviour, RemoteVars.IListener, IVars
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06002DC3 RID: 11715 RVA: 0x0017BD07 File Offset: 0x00179F07
	// (set) Token: 0x06002DC4 RID: 11716 RVA: 0x0017BD0F File Offset: 0x00179F0F
	public Logic.Kingdom SelectedKingdom { get; private set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x0017BD18 File Offset: 0x00179F18
	// (set) Token: 0x06002DC6 RID: 11718 RVA: 0x0017BD20 File Offset: 0x00179F20
	public UICampaignKingdomBrowser SelectionBrowser { get; private set; }

	// Token: 0x06002DC7 RID: 11719 RVA: 0x0017BD2C File Offset: 0x00179F2C
	public void Init()
	{
		if (this.m_Initialzid)
		{
			return;
		}
		using (Game.Profile("UISingleplayerCampaign.Init", false, 0f, null))
		{
			UICommon.FindComponents(this, false);
			if (this.m_Back != null)
			{
				this.m_Back.onClick = new BSGButton.OnClick(this.BackToMainMenu);
			}
			if (this.m_Start != null)
			{
				this.m_Start.onClick = new BSGButton.OnClick(this.StartCampaign);
			}
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
			if (this.m_KingdomSelectionPanel != null)
			{
				this.m_KingdomInfo = this.m_KingdomSelectionPanel.GetOrAddComponent<UICampaignKingdomInfo>();
			}
			if (this.m_KingdomBrowserContianer != null)
			{
				this.SelectionBrowser = this.m_KingdomBrowserContianer.GetOrAddComponent<UICampaignKingdomBrowser>();
				this.SelectionBrowser.HandelOnClose(null);
			}
			if (this.m_KingdomList != null)
			{
				this.m_KingdomList.AllowSelection(true);
				this.m_KingdomList.onClick = new BSGButton.OnClick(this.ToggleKingdomBrowser);
			}
			this.m_Initialzid = true;
		}
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x0017BEB8 File Offset: 0x0017A0B8
	private void OnEnable()
	{
		Game game = GameLogic.Get(false);
		Campaign campaignClient = (game != null) ? game.campaign : null;
		this.SetCampaignClient(campaignClient);
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x0017BEE0 File Offset: 0x0017A0E0
	public void SetCampaignClient(Campaign campaign)
	{
		this.Init();
		Campaign campaign2 = this.m_Campaign;
		if (campaign2 != null)
		{
			campaign2.DelVarsListener(this);
		}
		this.m_Campaign = campaign;
		Campaign campaign3 = this.m_Campaign;
		if (campaign3 != null)
		{
			campaign3.AddVarsListener(this);
		}
		this.LocalizeStatics();
		this.LocalizeDynamic();
		this.SetKingdomView(null);
		if (this.m_SettingsPanel != null)
		{
			this.m_SettingsPanel.SetData(campaign);
		}
		if (this.m_KingdomInfo != null)
		{
			this.m_KingdomInfo.Show(false);
		}
		CameraController.LookAt(MapData.Get().GetTerrainBounds().center);
		this.m_InvalidatePoliticalData = true;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x0017BF84 File Offset: 0x0017A184
	private void Update()
	{
		if (this.m_SelectRandomKingdom)
		{
			this.SelectRandomKingdom();
			if (this.m_KingdomInfo != null)
			{
				this.m_KingdomInfo.Show(true);
			}
			this.m_SelectRandomKingdom = false;
		}
		if (this.m_Campaign.state < Campaign.State.Started && this.SelectedKingdom != null && (this.SelectedKingdom.destroyed || this.SelectedKingdom.IsDefeated()) && !this.m_PickRealm)
		{
			this.SelectKingdom(null, null);
		}
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x0017C000 File Offset: 0x0017A200
	private void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.UpdateButtons();
			this.UpdateLabels();
			this.LocalizeDynamic();
			this.m_Invalidate = false;
		}
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
				this.m_InvalidatePoliticalData = false;
			}
		}
		if (this.m_InvalidateKingdomSelection)
		{
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			string name = this.m_Campaign.GetVar(RemoteVars.DataType.PersistentPlayerData, 0, "kingdom_name");
			Game game2 = GameLogic.Get(true);
			Logic.Kingdom kingdom = game2.GetKingdom(name);
			bool flag = CampaignUtils.IsRealmKingdom(kingdom);
			bool flag2 = this.m_Campaign.GetVar("pick_kingdom", null, true) == "province";
			Game game3 = game2;
			string name2;
			if (titleUI == null)
			{
				name2 = null;
			}
			else
			{
				global::Realm selected_realm = titleUI.selected_realm;
				name2 = ((selected_realm != null) ? selected_realm.Name : null);
			}
			Logic.Realm realm = game3.GetRealm(name2);
			Logic.Realm realm2;
			if (flag2 && flag)
			{
				realm2 = game2.GetRealm(kingdom.Name);
			}
			else if (flag2)
			{
				realm2 = realm;
			}
			else if (flag)
			{
				realm2 = game2.GetRealm(kingdom.Name);
			}
			else if (kingdom != null && realm != null)
			{
				if (!kingdom.HasRealm((realm != null) ? realm.name : null))
				{
					realm = ((kingdom.realms.Count > 0) ? kingdom.realms[0] : null);
				}
				realm2 = realm;
			}
			else
			{
				realm2 = realm;
			}
			titleUI.SetSelectedRealm((realm2 != null) ? realm2.id : 0, true);
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
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x0017C1F0 File Offset: 0x0017A3F0
	private void LocalizeStatics()
	{
		using (Game.Profile("UISingleplayerCampaign.LocalizeStatics", false, 0f, null))
		{
			if (this.m_RealmViewLabel != null)
			{
				UIText.SetTextKey(this.m_RealmViewLabel, "CampaignsWindow.realm", null, null);
			}
			if (this.m_KingdomViewLabel != null)
			{
				UIText.SetTextKey(this.m_KingdomViewLabel, "CampaignsWindow.kingdom", null, null);
			}
			if (this.m_StartLabel != null)
			{
				UIText.SetTextKey(this.m_StartLabel, "CampaignsWindow.start", null, null);
			}
			if (this.m_GradCampaignDescription != null)
			{
				UIText.SetTextKey(this.m_GradCampaignDescription, "CampaignsWindow.grand_campaign_description", null, null);
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
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x0017C310 File Offset: 0x0017A510
	private void LocalizeDynamic()
	{
		if (this.m_RulesLabel != null)
		{
			UIText.SetTextKey(this.m_RulesLabel, "CampaignsWindow.rules", new Vars(this), null);
		}
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x0017C338 File Offset: 0x0017A538
	public void SelectKingdom(Logic.Kingdom kingdom, Logic.Realm realm)
	{
		this.Init();
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Campaign campaign = game.campaign;
		if (campaign == null)
		{
			return;
		}
		string a = this.m_Campaign.GetVar("pick_kingdom", null, true);
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
			this.m_KingdomInfo.SetKingdom(campaign, this.SelectedKingdom, realm);
		}
		Logic.Kingdom selectedKingdom = this.SelectedKingdom;
		this.SetPlayerKingdom(((selectedKingdom != null) ? selectedKingdom.Name : null) ?? string.Empty, ((realm != null) ? realm.name : null) ?? string.Empty);
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x0017C42C File Offset: 0x0017A62C
	private void SetPlayerKingdom(string kingdomName, string realmName)
	{
		Game game = GameLogic.Get(false);
		Game game2 = GameLogic.Get(false);
		Campaign campaign = (game2 != null) ? game2.campaign : null;
		if (campaign == null)
		{
			return;
		}
		if (campaign.playerDataPersistent == null)
		{
			return;
		}
		this.m_Invalidate = true;
		if (!CampaignUtils.IsPlayerEligible(game, game.GetKingdom(kingdomName)))
		{
			campaign.SetPlayerKingdomName(0, null, null, true);
			return;
		}
		campaign.SetPlayerKingdomName(0, kingdomName, realmName, true);
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x0017C48C File Offset: 0x0017A68C
	private void SelectRandomKingdom()
	{
		Game game = GameLogic.Get(false);
		List<Logic.Kingdom> list = new List<Logic.Kingdom>();
		for (int i = 0; i < game.kingdoms.Count; i++)
		{
			Logic.Kingdom kingdom = game.kingdoms[i];
			if (!kingdom.IsDefeated())
			{
				list.Add(kingdom);
			}
		}
		if (list.Count > 0)
		{
			Logic.Kingdom kingdom2 = list[Random.Range(0, list.Count)];
			BaseUI.Get<TitleUI>().SelectKingdom(kingdom2.id, true);
			CameraController.LookAt((kingdom2.visuals as global::Kingdom).CalcBounds().center);
		}
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x0017C528 File Offset: 0x0017A728
	private void SetKingdomView(BSGButton b)
	{
		using (Game.Profile("UISingleplayerCampaign.SetKingdomView", false, 0f, null))
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
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x0017C594 File Offset: 0x0017A794
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

	// Token: 0x06002DD3 RID: 11731 RVA: 0x0017C5C2 File Offset: 0x0017A7C2
	private void StartCampaign(BSGButton b)
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.StartSinglePlayerGame();
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x0017C5D4 File Offset: 0x0017A7D4
	private Campaign GetCampaign()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return null;
		}
		return game.campaign;
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x0017C5E8 File Offset: 0x0017A7E8
	private bool ValidateCampaign()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return false;
		}
		Campaign campaign = game.campaign;
		return campaign != null && campaign.ValidateSetup(game) == null;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x0017C619 File Offset: 0x0017A819
	private void BackToMainMenu(BSGButton b)
	{
		base.gameObject.SetActive(false);
		CampaignUtils.ClearSingleplayerLogic(GameLogic.Get(true), true);
		TitleUI titleUI = BaseUI.Get<TitleUI>();
		if (titleUI == null)
		{
			return;
		}
		titleUI.DisableSinglePlayerCampaignWindow();
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x0017C644 File Offset: 0x0017A844
	private void UpdateButtons()
	{
		if (this.m_Start != null)
		{
			bool enable = this.ValidateCampaign();
			this.m_Start.Enable(enable, false);
		}
		if (this.m_KingdomList != null)
		{
			this.m_KingdomList.SetSelected(this.SelectionBrowser.IsShown(), false);
		}
		this.UpdateViewButtons();
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x0017C6A0 File Offset: 0x0017A8A0
	private void UpdateLabels()
	{
		if (this.m_StartPromptLabel != null)
		{
			this.ValidateCampaign();
			string startPromptLabel = UILobby.GetStartPromptLabel(this.GetCampaign());
			Game game = GameLogic.Get(false);
			IVars vars = (game != null) ? game.GetKingdom(this.m_Campaign.GetKingdomName(false)) : null;
			UIText.SetText(this.m_StartPromptLabel, global::Defs.Localize(startPromptLabel, vars, null, true, true));
		}
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x0017C704 File Offset: 0x0017A904
	private void UpdateViewButtons()
	{
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

	// Token: 0x06002DDA RID: 11738 RVA: 0x0017C7BC File Offset: 0x0017A9BC
	private void ToggleKingdomBrowser(BSGButton b)
	{
		if (this.SelectionBrowser != null)
		{
			if (this.SelectionBrowser.IsShown())
			{
				this.SelectionBrowser.Close(false);
				return;
			}
			UICampaignKingdomBrowser selectionBrowser = this.SelectionBrowser;
			Game game = GameLogic.Get(false);
			selectionBrowser.Open((game != null) ? game.campaign : null, new Action<Logic.Kingdom>(this.OnKingdomBrowseSelected), null);
		}
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x0017C81C File Offset: 0x0017AA1C
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

	// Token: 0x06002DDC RID: 11740 RVA: 0x0017C8D0 File Offset: 0x0017AAD0
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (this == null)
		{
			Campaign campaign = this.m_Campaign;
			if (campaign == null)
			{
				return;
			}
			campaign.DelVarsListener(this);
			return;
		}
		else
		{
			this.m_Invalidate = true;
			if (key == "main_goal" || key == "realms_goal")
			{
				this.m_InvalidateKingdomSelection = true;
				return;
			}
			if (key == "pick_kingdom")
			{
				this.m_InvalidateKingdomSelection = true;
				this.m_InvalidateViewFilter = true;
				return;
			}
			if (!(key == "map_size") && !(key == "start_period") && !(key == "from_save_id"))
			{
				return;
			}
			this.m_InvalidateKingdomSelection = true;
			this.m_InvalidatePoliticalData = true;
			return;
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x0017C973 File Offset: 0x0017AB73
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "is_game_from_save")
		{
			Campaign campaign = this.m_Campaign;
			return campaign != null && campaign.IsFromSave();
		}
		return Value.Unknown;
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x0017C99F File Offset: 0x0017AB9F
	private void OnDisable()
	{
		if (this.SelectionBrowser != null)
		{
			this.SelectionBrowser.Close(false);
		}
		TooltipInstance.RemovePinnedTooltips();
	}

	// Token: 0x04001EF4 RID: 7924
	[UIFieldTarget("id_Back")]
	private BSGButton m_Back;

	// Token: 0x04001EF5 RID: 7925
	[UIFieldTarget("id_Start")]
	private BSGButton m_Start;

	// Token: 0x04001EF6 RID: 7926
	[UIFieldTarget("id_StartLabel")]
	private TextMeshProUGUI m_StartLabel;

	// Token: 0x04001EF7 RID: 7927
	[UIFieldTarget("id_KingdomView")]
	private BSGButton m_KingdomView;

	// Token: 0x04001EF8 RID: 7928
	[UIFieldTarget("id_KingdomViewLabel")]
	private TextMeshProUGUI m_KingdomViewLabel;

	// Token: 0x04001EF9 RID: 7929
	[UIFieldTarget("id_RealmView")]
	private BSGButton m_RealmView;

	// Token: 0x04001EFA RID: 7930
	[UIFieldTarget("id_RealmViewLabel")]
	private TextMeshProUGUI m_RealmViewLabel;

	// Token: 0x04001EFB RID: 7931
	[UIFieldTarget("id_KingdomSelectionPanel")]
	private GameObject m_KingdomSelectionPanel;

	// Token: 0x04001EFC RID: 7932
	[UIFieldTarget("id_KingdomList")]
	private BSGButton m_KingdomList;

	// Token: 0x04001EFD RID: 7933
	[UIFieldTarget("id_SettingsPanel")]
	private UICampaignSettings m_SettingsPanel;

	// Token: 0x04001EFE RID: 7934
	[UIFieldTarget("id_KingdomBrowser")]
	private GameObject m_KingdomBrowserContianer;

	// Token: 0x04001EFF RID: 7935
	[UIFieldTarget("id_StartPromptLabel")]
	private TextMeshProUGUI m_StartPromptLabel;

	// Token: 0x04001F00 RID: 7936
	[UIFieldTarget("id_GradCampaignDescription")]
	private TextMeshProUGUI m_GradCampaignDescription;

	// Token: 0x04001F01 RID: 7937
	[UIFieldTarget("id_GradCampaignLabel")]
	private TextMeshProUGUI m_GradCampaignLabel;

	// Token: 0x04001F02 RID: 7938
	[UIFieldTarget("id_MainGoalLabel")]
	private TextMeshProUGUI m_MainGoalLabel;

	// Token: 0x04001F03 RID: 7939
	[UIFieldTarget("id_RulesLabel")]
	private TextMeshProUGUI m_RulesLabel;

	// Token: 0x04001F04 RID: 7940
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI m_BackLabel;

	// Token: 0x04001F07 RID: 7943
	private UICampaignKingdomInfo m_KingdomInfo;

	// Token: 0x04001F08 RID: 7944
	private Campaign m_Campaign;

	// Token: 0x04001F09 RID: 7945
	private bool m_SelectRandomKingdom;

	// Token: 0x04001F0A RID: 7946
	private bool m_InvalidateViewFilter;

	// Token: 0x04001F0B RID: 7947
	private bool m_InvalidatePoliticalData;

	// Token: 0x04001F0C RID: 7948
	private bool m_InvalidateKingdomSelection;

	// Token: 0x04001F0D RID: 7949
	private bool m_Initialzid;

	// Token: 0x04001F0E RID: 7950
	private bool m_Invalidate = true;

	// Token: 0x04001F0F RID: 7951
	private bool m_PickRealm;
}
