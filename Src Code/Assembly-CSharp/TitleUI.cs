using System;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002D9 RID: 729
public class TitleUI : BaseUI
{
	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06002E03 RID: 11779 RVA: 0x0017D00E File Offset: 0x0017B20E
	// (set) Token: 0x06002E04 RID: 11780 RVA: 0x0017D016 File Offset: 0x0017B216
	public bool showSignButtons { get; private set; }

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06002E05 RID: 11781 RVA: 0x0017D01F File Offset: 0x0017B21F
	private DT.Field def
	{
		get
		{
			if (this.window_def == null)
			{
				this.window_def = global::Defs.GetDefField("Title", null);
			}
			return this.window_def;
		}
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x0017D040 File Offset: 0x0017B240
	private void Init()
	{
		if (this.m_Initiazled)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.title = base.GetComponent<Title>();
		if (this.m_MainMenuButtons != null)
		{
			this.m_MainMenuButtons.gameObject.SetActive(Title.allLoaded && UserSettings.DataCollectionAsked);
		}
		if (this.btnNewGame != null)
		{
			this.btnNewGame.onClick = new BSGButton.OnClick(this.OnNewGame);
			this.btnNewGame.Enable(false, false);
			this.btnNewGame.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnContinue != null)
		{
			this.btnContinue.onClick = new BSGButton.OnClick(this.OnContinue);
			this.btnContinue.Enable(false, false);
			this.btnContinue.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnLoad != null)
		{
			this.btnLoad.onClick = new BSGButton.OnClick(this.OnLoadGame);
			this.btnLoad.Enable(false, false);
			this.btnLoad.AllowSelection(true);
			this.btnLoad.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnMultiplayer != null)
		{
			this.btnMultiplayer.onClick = new BSGButton.OnClick(this.OnMultiplayer);
			this.btnMultiplayer.AllowSelection(true);
			this.btnMultiplayer.Enable(false, false);
			this.btnMultiplayer.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnCredits != null)
		{
			this.btnCredits.onClick = new BSGButton.OnClick(this.OnCredits);
			this.btnCredits.Enable(false, false);
			this.btnCredits.AllowSelection(true);
			this.btnCredits.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnSettings != null)
		{
			this.btnSettings.onClick = new BSGButton.OnClick(this.OnSettings);
			this.btnSettings.Enable(false, false);
			this.btnSettings.AllowSelection(true);
			this.btnSettings.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnQuit != null)
		{
			this.btnQuit.onClick = new BSGButton.OnClick(this.OnQuit);
			this.btnQuit.Enable(true, false);
			this.btnQuit.AllowSelection(true);
			this.btnQuit.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnSignOut != null)
		{
			this.btnSignOut.onClick = new BSGButton.OnClick(this.OnSignOut);
			this.btnSignOut.Enable(false, false);
			this.btnSignOut.SetAudioSet("DefaultTitleAudioSet");
		}
		if (this.btnSignIn != null)
		{
			this.btnSignIn.onClick = new BSGButton.OnClick(this.OnSignIn);
			this.btnSignIn.Enable(false, false);
			this.btnSignIn.SetAudioSet("DefaultTitleAudioSet");
		}
		this.LocalizeStatic();
		this.m_Initiazled = true;
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x0017D334 File Offset: 0x0017B534
	private void LocalizeStatic()
	{
		if (this.m_CopyrightLabel != null)
		{
			UIText.SetTextKey(this.m_CopyrightLabel, "Title.copyright", null, null);
		}
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnContinue, "continue");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnNewGame, "new_game");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnLoad, "load_game");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnMultiplayer, "multiplayer");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnSettings, "settings");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnCredits, "credits");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnQuit, "quit");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnSignIn, "signin");
		TitleUI.<LocalizeStatic>g__LocalzieButton|43_0(this.btnSignOut, "signot");
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x0017D3F1 File Offset: 0x0017B5F1
	public void OnLanguageChange()
	{
		this.LocalizeStatic();
		this.EnableSettingsWindow();
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x0017D3FF File Offset: 0x0017B5FF
	protected override void Start()
	{
		this.Init();
		PersistentLogs.RemoveLoadedLogs();
		base.Start();
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x0017D412 File Offset: 0x0017B612
	protected override void Update()
	{
		base.Update();
		this.UpdateMainMenuButtons();
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x0017D420 File Offset: 0x0017B620
	public void EnableSinglePlayerCampaignWindow()
	{
		this.CloseAnyOpenMenus();
		GameObject singleplayerCampaign = this.m_SingleplayerCampaign;
		if (singleplayerCampaign != null)
		{
			singleplayerCampaign.SetActive(true);
		}
		GameObject mainMenu = this.m_MainMenu;
		if (mainMenu != null)
		{
			mainMenu.gameObject.SetActive(false);
		}
		GameObject backgroundImage = this.m_BackgroundImage;
		if (backgroundImage != null)
		{
			backgroundImage.gameObject.SetActive(false);
		}
		GameObject logosAndText = this.m_LogosAndText;
		if (logosAndText != null)
		{
			logosAndText.gameObject.SetActive(false);
		}
		RichPresence.Update(RichPresence.State.InLobby);
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x0017D494 File Offset: 0x0017B694
	public void DisableSinglePlayerCampaignWindow()
	{
		this.CloseAnyOpenMenus();
		GameObject singleplayerCampaign = this.m_SingleplayerCampaign;
		if (singleplayerCampaign != null)
		{
			singleplayerCampaign.SetActive(false);
		}
		GameObject mainMenu = this.m_MainMenu;
		if (mainMenu != null)
		{
			mainMenu.gameObject.SetActive(true);
		}
		GameObject backgroundImage = this.m_BackgroundImage;
		if (backgroundImage != null)
		{
			backgroundImage.gameObject.SetActive(true);
		}
		RichPresence.Update(RichPresence.State.InTitle);
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x0017D4F0 File Offset: 0x0017B6F0
	public void EnableLoadGameMenu()
	{
		this.CloseAnyOpenMenus();
		UILoadGameWindow uiloadGameWindow = UILoadGameWindow.Create(base.message_container.transform, false);
		if (uiloadGameWindow != null)
		{
			uiloadGameWindow.SetLocalPosition(new Vector2(390f, -44f));
		}
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x0017D533 File Offset: 0x0017B733
	public void EnableMultiplayerMenu(Campaign campaign = null)
	{
		this.CloseAnyOpenMenus();
		UIMultiplayerMenu uimultiplayerMenu = this.m_MultiplayerMenu;
		if (uimultiplayerMenu != null)
		{
			uimultiplayerMenu.Open();
		}
		if (campaign != null)
		{
			UIMultiplayerMenu uimultiplayerMenu2 = this.m_MultiplayerMenu;
			if (uimultiplayerMenu2 == null)
			{
				return;
			}
			UICampaigns componentInChildren = uimultiplayerMenu2.GetComponentInChildren<UICampaigns>();
			if (componentInChildren == null)
			{
				return;
			}
			componentInChildren.Select(campaign);
		}
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x0017D56A File Offset: 0x0017B76A
	public void EnableLobbyBrowserScreen()
	{
		this.CloseAnyOpenMenus();
		this.m_MainMenu.SetActive(false);
		this.m_LobbyBrowser.SetActive(true);
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x0017D58C File Offset: 0x0017B78C
	public void EnableLobbyScreen(Campaign campaign = null)
	{
		this.CloseAnyOpenMenus();
		if (campaign == null)
		{
			Debug.LogError("Enabling Lobby screen with invalid CampaignClient!");
			return;
		}
		this.m_Lobby.SetActive(true);
		GameObject logosAndText = this.m_LogosAndText;
		if (logosAndText != null)
		{
			logosAndText.SetActive(false);
		}
		UILobby uilobby = this.lobby;
		if (uilobby != null)
		{
			uilobby.SetCampaign(campaign);
		}
		this.m_MainMenu.gameObject.SetActive(false);
		this.m_BackgroundImage.gameObject.SetActive(false);
		RichPresence.Update(RichPresence.State.InLobby);
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x0017D606 File Offset: 0x0017B806
	public void DisableLobbyScreen()
	{
		this.CloseAnyOpenMenus();
		this.m_Lobby.SetActive(false);
		RichPresence.Update(RichPresence.State.InTitle);
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x0017D621 File Offset: 0x0017B821
	public void EnableAutoForgotPasswordWindow(Action<string, string, string> onUserChoice = null, string email = null, string status_message = null)
	{
		if (this.title.multiplayerForgotPasswordRequestSent)
		{
			this.EnableForgotPasswordVerificationWindow(true, onUserChoice, status_message);
			this.EnableForgotPasswordRequestWindow(false, null, null, null);
			return;
		}
		this.EnableForgotPasswordRequestWindow(true, email, onUserChoice, status_message);
		this.EnableForgotPasswordVerificationWindow(false, null, null);
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x0017D658 File Offset: 0x0017B858
	public void EnableForgotPasswordRequestWindow(bool enabled, string email = null, Action<string, string, string> onUserChoice = null, string status_message = null)
	{
		if (this.m_ForgotPasswordRequest == null)
		{
			Debug.LogWarning("Forgot Password Request window is missing!");
			return;
		}
		UIForgotPasswordRequest component = global::Common.GetComponent<UIForgotPasswordRequest>(this.m_ForgotPasswordRequest, null);
		if (component != null)
		{
			component.email = email;
			component.onUserChoice = onUserChoice;
			UIText.SetText(component.statusMessage, status_message);
		}
		this.m_ForgotPasswordRequest.SetActive(enabled);
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x0017D6BC File Offset: 0x0017B8BC
	public void EnableForgotPasswordVerificationWindow(bool enabled, Action<string, string, string> onUserChoice = null, string status_message = null)
	{
		if (this.m_ForgotPasswordVerification == null)
		{
			Debug.LogWarning("Forgot Password Verification window is missing!");
			return;
		}
		UIForgotPasswordVerification component = global::Common.GetComponent<UIForgotPasswordVerification>(this.m_ForgotPasswordVerification, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
			UIText.SetText(component.statusMessage, status_message);
		}
		this.m_ForgotPasswordVerification.SetActive(enabled);
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x0017D718 File Offset: 0x0017B918
	public void EnableAPINoConnectionWindow(Action<string> onUserChoice = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_APINoConnection == null)
		{
			Debug.LogWarning("APINoConnection window is missing!");
			return;
		}
		UIApiNoConnection component = global::Common.GetComponent<UIApiNoConnection>(this.m_APINoConnection, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
		}
		this.m_APINoConnection.SetActive(true);
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x0017D770 File Offset: 0x0017B970
	public void EnableAPIVersionMismatchWindow(Action<string> onUserChoice = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_APIVersionMismatch == null)
		{
			Debug.LogWarning("APIVersionMismatchWindow window is missing!");
			return;
		}
		UIAPIVersionMismatch component = global::Common.GetComponent<UIAPIVersionMismatch>(this.m_APIVersionMismatch, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
		}
		this.m_APIVersionMismatch.SetActive(true);
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x0017D7C8 File Offset: 0x0017B9C8
	public void EnableRegisterWindow(string email = "", Action<string, string, string> onUserChoice = null, string status_message = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_Register == null)
		{
			Debug.LogWarning("Register window is missing!");
			return;
		}
		this.m_Register.SetActive(true);
		UIRegister component = global::Common.GetComponent<UIRegister>(this.m_Register, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
			UIText.SetText(component.statusMessage, status_message);
			if (!string.IsNullOrEmpty(email))
			{
				component.email = email;
			}
		}
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x0017D838 File Offset: 0x0017BA38
	public void DisableRegisterWindow()
	{
		this.CloseAnyOpenMenus();
		this.m_Register.SetActive(false);
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x0017D84C File Offset: 0x0017BA4C
	public void EnableLoginWindow(string email = "", Action<string, string, string> onUserChoice = null, string status_message = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_Login == null)
		{
			Debug.LogWarning("Login window is missing!");
			return;
		}
		this.m_Login.SetActive(true);
		UILogin component = global::Common.GetComponent<UILogin>(this.m_Login, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
			UIText.SetText(component.statusMessage, status_message);
			if (!string.IsNullOrEmpty(email))
			{
				component.email = email;
			}
		}
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x0017D8BC File Offset: 0x0017BABC
	public void DisableLoginWindow()
	{
		this.CloseAnyOpenMenus();
		this.m_Login.SetActive(false);
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x0017D8D0 File Offset: 0x0017BAD0
	public void EnableChooseDisplayNameWindow(Action<string, string, string> onUserChoice = null, string status_message = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_ChooseDisplayName == null)
		{
			Debug.LogWarning("Choose Display Name window is missing!");
			return;
		}
		UIChooseDisplayName component = global::Common.GetComponent<UIChooseDisplayName>(this.m_ChooseDisplayName, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
			UIText.SetText(component.statusMessage, status_message);
		}
		this.m_ChooseDisplayName.SetActive(true);
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x0017D931 File Offset: 0x0017BB31
	public void DisableChooseDisplayNameWindow()
	{
		this.CloseAnyOpenMenus();
		this.m_ChooseDisplayName.SetActive(false);
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x0017D948 File Offset: 0x0017BB48
	public void EnableOfflineWarningWindow(Action<string> onUserChoice = null, string reason = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_OfflineWarning == null)
		{
			Debug.LogWarning("Offline Warning window is missing!");
			return;
		}
		UIOfflineWarning component = global::Common.GetComponent<UIOfflineWarning>(this.m_OfflineWarning, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
			component.SetReasonText(reason);
			component.Show();
		}
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x0017D9A0 File Offset: 0x0017BBA0
	public void EnableSingleplayerLoginWindow(Action<string> onUserChoice = null)
	{
		this.CloseAnyOpenMenus();
		if (this.m_SingleplayerLogin == null)
		{
			Debug.LogWarning("Singleplayer login window is missing!");
			return;
		}
		UISingleplayerLogin component = global::Common.GetComponent<UISingleplayerLogin>(this.m_SingleplayerLogin, null);
		if (component != null)
		{
			component.onUserChoice = onUserChoice;
		}
		this.m_SingleplayerLogin.SetActive(base.enabled);
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x0017D9FA File Offset: 0x0017BBFA
	public void EnableSettingsWindow()
	{
		this.CloseAnyOpenMenus();
		this.m_Preferences.Open();
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x0017DA0D File Offset: 0x0017BC0D
	public void EnableCreditsWindow()
	{
		this.CloseAnyOpenMenus();
		this.m_MainMenu.SetActive(false);
		if (this.m_Credits != null)
		{
			this.m_Credits.Show();
		}
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x0017DA3A File Offset: 0x0017BC3A
	public void DisableCreditsWindow()
	{
		this.m_MainMenu.SetActive(true);
		this.CloseAnyOpenMenus();
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x0017DA50 File Offset: 0x0017BC50
	public void CloseAnyOpenMenus()
	{
		UIMultiplayerMenu uimultiplayerMenu = this.m_MultiplayerMenu;
		if (uimultiplayerMenu != null)
		{
			uimultiplayerMenu.Close(false);
		}
		GameObject gameObject = this.m_Lobby;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		GameObject lobbyBrowser = this.m_LobbyBrowser;
		if (lobbyBrowser != null)
		{
			lobbyBrowser.SetActive(false);
		}
		GameObject forgotPasswordRequest = this.m_ForgotPasswordRequest;
		if (forgotPasswordRequest != null)
		{
			forgotPasswordRequest.SetActive(false);
		}
		GameObject forgotPasswordVerification = this.m_ForgotPasswordVerification;
		if (forgotPasswordVerification != null)
		{
			forgotPasswordVerification.SetActive(false);
		}
		GameObject singleplayerCampaign = this.m_SingleplayerCampaign;
		if (singleplayerCampaign != null)
		{
			singleplayerCampaign.SetActive(false);
		}
		GameObject mainMenu = this.m_MainMenu;
		if (mainMenu != null)
		{
			mainMenu.gameObject.SetActive(true);
		}
		GameObject backgroundImage = this.m_BackgroundImage;
		if (backgroundImage != null)
		{
			backgroundImage.gameObject.SetActive(true);
		}
		GameObject apinoConnection = this.m_APINoConnection;
		if (apinoConnection != null)
		{
			apinoConnection.SetActive(false);
		}
		GameObject apiversionMismatch = this.m_APIVersionMismatch;
		if (apiversionMismatch != null)
		{
			apiversionMismatch.SetActive(false);
		}
		GameObject register = this.m_Register;
		if (register != null)
		{
			register.SetActive(false);
		}
		GameObject login = this.m_Login;
		if (login != null)
		{
			login.SetActive(false);
		}
		GameObject chooseDisplayName = this.m_ChooseDisplayName;
		if (chooseDisplayName != null)
		{
			chooseDisplayName.SetActive(false);
		}
		GameObject offlineWarning = this.m_OfflineWarning;
		if (offlineWarning != null)
		{
			offlineWarning.SetActive(false);
		}
		if (this.m_SingleplayerLogin != null)
		{
			this.m_SingleplayerLogin.SetActive(false);
		}
		UIPreferences preferences = this.m_Preferences;
		if (preferences != null)
		{
			preferences.Close(false);
		}
		UICredits credits = this.m_Credits;
		if (credits != null)
		{
			credits.gameObject.SetActive(false);
		}
		GameObject logosAndText = this.m_LogosAndText;
		if (logosAndText != null)
		{
			logosAndText.gameObject.SetActive(true);
		}
		UILoadGameWindow.Hide();
		UISaveGameWindow.Hide();
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x0017DBC7 File Offset: 0x0017BDC7
	private void UpdateSignButtons()
	{
		Title.BranchName();
		if (THQNORequest.platformType == Logic.Common.PlatformType.Steam || THQNORequest.platformType == Logic.Common.PlatformType.EpicStore)
		{
			this.showSignButtons = false;
			return;
		}
		this.showSignButtons = true;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x0017DBEE File Offset: 0x0017BDEE
	public bool MultiplayerEnabled()
	{
		return Title.IsInternalBranch() || MPBoss.MultiplayerEnabledRemotely();
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x0017DC04 File Offset: 0x0017BE04
	private void UpdateMainMenuButtons()
	{
		this.UpdateSignButtons();
		if (this.m_MainMenuButtons == null)
		{
			return;
		}
		bool allLoaded = Title.allLoaded;
		if (!this.m_MainMenuButtons.activeSelf && allLoaded && UserSettings.DataCollectionAsked)
		{
			BaseUI.PlayVoiceEvent("narrator_voice:greetings", null);
			this.m_MainMenuButtons.gameObject.SetActive(true);
		}
		if (!this.m_MainMenuButtons.activeInHierarchy)
		{
			return;
		}
		if (this.btnNewGame != null)
		{
			this.btnNewGame.Enable(Title.allLoaded, false);
		}
		if (this.btnContinue != null)
		{
			SaveGame.UpdateList(false, true);
			this.btnContinue.Enable(allLoaded && SaveGame.latest_single_playr_campaign_info != null, false);
		}
		if (this.btnLoad != null)
		{
			this.btnLoad.Enable(allLoaded && SaveGame.campaigns.Count > 0, false);
			this.btnLoad.SetSelected(UILoadGameWindow.IsActive(), false);
		}
		if (this.btnMultiplayer != null)
		{
			this.btnMultiplayer.Enable(allLoaded && this.MultiplayerEnabled(), false);
			this.btnMultiplayer.SetSelected(this.m_MultiplayerMenu.IsShown(), false);
		}
		if (this.btnCredits != null)
		{
			this.btnCredits.Enable(allLoaded, false);
			this.btnCredits.SetSelected(this.m_Credits.gameObject.activeInHierarchy, false);
		}
		if (this.btnSettings != null)
		{
			this.btnSettings.Enable(allLoaded, false);
			this.btnSettings.SetSelected(this.m_Preferences.gameObject.activeInHierarchy, false);
		}
		if (this.showSignButtons)
		{
			bool flag = THQNORequest.signed_in && !string.IsNullOrEmpty(THQNORequest.playerName);
			if (this.btnSignIn != null)
			{
				this.btnSignIn.Enable(allLoaded, false);
				this.btnSignIn.SetSelected(false, false);
				this.btnSignIn.gameObject.SetActive(!flag);
			}
			if (this.btnSignOut != null)
			{
				this.btnSignOut.Enable(allLoaded, false);
				this.btnSignOut.gameObject.SetActive(flag);
				return;
			}
		}
		else
		{
			BSGButton bsgbutton = this.btnSignIn;
			if (bsgbutton != null)
			{
				GameObject gameObject = bsgbutton.gameObject;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			BSGButton bsgbutton2 = this.btnSignOut;
			if (bsgbutton2 != null)
			{
				GameObject gameObject2 = bsgbutton2.gameObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			GameObject signInBackground = this.SignInBackground;
			if (signInBackground == null)
			{
				return;
			}
			signInBackground.SetActive(false);
		}
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x0017DE7C File Offset: 0x0017C07C
	private void OnNewGame(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		Action OnComplete = delegate()
		{
			if (!UICommon.GetKey(KeyCode.LeftShift, false))
			{
				this.title.BuildSingleplayerGameData(GameLogic.Get(true), "europe");
				this.EnableSinglePlayerCampaignWindow();
				return;
			}
			this.title.StartTestMapSinglePlayer();
		};
		this.CheckForUnApplyedSettings(delegate
		{
			Logic.Coroutine.Start("StartSingleplayer", MPBoss.Get().StartSingleplayer(OnComplete), null);
		});
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x0017DEC4 File Offset: 0x0017C0C4
	private void OnContinue(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		Action OnComplete = delegate()
		{
			Title title = this.title;
			if (title == null)
			{
				return;
			}
			title.ContinueGame();
		};
		this.CheckForUnApplyedSettings(delegate
		{
			Logic.Coroutine.Start("StartSingleplayer", MPBoss.Get().StartSingleplayer(OnComplete), null);
		});
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x0017DF0C File Offset: 0x0017C10C
	private void OnLoadGame(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		if (UILoadGameWindow.IsActive())
		{
			UILoadGameWindow.Hide();
			return;
		}
		Action OnComplete = delegate()
		{
			UserInteractionLogger.LogNewLine(btn, null);
			this.EnableLoadGameMenu();
		};
		this.CheckForUnApplyedSettings(delegate
		{
			Logic.Coroutine.Start("StartSingleplayer", MPBoss.Get().StartSingleplayer(OnComplete), null);
		});
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x0017DF6A File Offset: 0x0017C16A
	private void OnCredits(BSGButton btn)
	{
		this.CheckForUnApplyedSettings(delegate
		{
			this.EnableCreditsWindow();
		});
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x0017DF80 File Offset: 0x0017C180
	private void OnMultiplayer(BSGButton btn)
	{
		if (!btn.IsEnabled())
		{
			return;
		}
		if (!this.MultiplayerEnabled())
		{
			return;
		}
		UIMultiplayerMenu uimultiplayerMenu = this.m_MultiplayerMenu;
		if (uimultiplayerMenu != null && uimultiplayerMenu.IsShown())
		{
			this.m_MultiplayerMenu.Close(false);
			return;
		}
		UserInteractionLogger.LogNewLine(btn, "Multiplayer");
		Action onMessage = delegate()
		{
			THQNORequest.CheckOnlineServices();
			if (THQNORequest.connected && THQNORequest.signed_in && !string.IsNullOrEmpty(THQNORequest.playerName))
			{
				this.EnableMultiplayerMenu(null);
				return;
			}
			string empty = string.Empty;
			string reason = "not_connected";
			if (THQNORequest.lastConnectToOnlineServiceResult == Logic.Common.APIResult.Init_RequiredPluginNotLoaded)
			{
				reason = "no_platform";
			}
			this.EnableOfflineWarningWindow(null, reason);
		};
		this.CheckForUnApplyedSettings(delegate
		{
			onMessage();
		});
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x0017DFFC File Offset: 0x0017C1FC
	private void OnSettings(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		if (this.m_Preferences != null)
		{
			if (this.m_Preferences.IsShown())
			{
				this.m_Preferences.Close(false);
				return;
			}
			this.EnableSettingsWindow();
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x0017E033 File Offset: 0x0017C233
	private void OnQuit(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		this.title.Quit();
		this.CheckForUnApplyedSettings(delegate
		{
			this.title.Quit();
		});
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x0017E05C File Offset: 0x0017C25C
	private void OnSignOut(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		Action onMessage = delegate()
		{
			MPBoss mpboss = MPBoss.Get();
			if (mpboss != null)
			{
				mpboss.CheckForSignInCoroAndTerminate();
			}
			Logic.Coroutine.Start("SignOut", MPBoss.Get().SignOut(), null);
		};
		this.CheckForUnApplyedSettings(delegate
		{
			onMessage();
		});
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x0017E0B0 File Offset: 0x0017C2B0
	private void OnSignIn(BSGButton btn)
	{
		Action onMessage = delegate()
		{
			UserInteractionLogger.LogNewLine(btn, null);
			MPBoss mpboss = MPBoss.Get();
			if (mpboss == null)
			{
				return;
			}
			mpboss.Start(true);
		};
		this.CheckForUnApplyedSettings(delegate
		{
			onMessage();
		});
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x0017E0F0 File Offset: 0x0017C2F0
	private void OnClearSaves(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		bool modifierKey = UICommon.GetModifierKey(UICommon.ModifierKey.Shift);
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.ClearSaves(true, !modifierKey, !modifierKey);
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x0017E123 File Offset: 0x0017C323
	private void CheckForUnApplyedSettings(Action callback)
	{
		if (this.m_Preferences != null && this.m_Preferences.HasUnappledChangesInSelectedCategory())
		{
			this.m_Preferences.ShowApplyChangesMessage(callback);
			return;
		}
		callback();
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x0017E154 File Offset: 0x0017C354
	protected override void OnEnable()
	{
		base.OnEnable();
		MPBoss.StartCheckingMultiplayerEnabledRemotely();
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			Shader.SetGlobalVector("_WVBattlePos", Vector4.zero);
			titleMap.SetHighlighedRealm(null);
			titleMap.SetSelectedRealm(0);
		}
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x0017E198 File Offset: 0x0017C398
	protected override void OnDisable()
	{
		MPBoss.StopCheckingMultiplayerEnabledRemotely();
		base.OnDisable();
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x0017E1A8 File Offset: 0x0017C3A8
	public override void OnMouseMove(Vector2 pts, int btn)
	{
		base.OnMouseMove(pts, btn);
		if (!EventSystem.current.IsPointerOverGameObject() && !IMGUIHandler.IsPointerOverIMGUI())
		{
			global::Realm highlighedRealm = global::Realm.At(this.picked_terrain_point);
			TitleMap.Get().SetHighlighedRealm(highlighedRealm);
		}
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x0017E1EA File Offset: 0x0017C3EA
	public override void OnMouseUp(Vector2 pts, int btn)
	{
		base.OnMouseUp(pts, btn);
		if (btn == 0 && UnityEngine.Time.unscaledTime - this.btn_down_time <= 0.25f)
		{
			this.Select();
		}
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x0017E214 File Offset: 0x0017C414
	protected override void UpdatePicker()
	{
		if (Input.mousePosition != this.ptLastMousePos)
		{
			this.tmLastMouseMove = UnityEngine.Time.unscaledTime;
			this.ptLastMousePos = Input.mousePosition;
		}
		this.picked_map_object = null;
		this.picked_settlement = null;
		this.picked_army = null;
		this.picked_unit = null;
		this.picked_terrain_point = Vector3.zero;
		this.picked_passable_area = 0;
		this.picked_passable_area_pos = Vector3.zero;
		Tooltip tooltip = null;
		Camera camera = CameraController.MainCamera;
		if (camera == null)
		{
			camera = this.canvas.worldCamera;
		}
		if (camera != null)
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			int num = Physics.RaycastNonAlloc(ray, this.hits);
			for (int i = 0; i < num; i++)
			{
				Transform transform = this.hits[i].transform;
				if (!(transform == null))
				{
					if (tooltip == null)
					{
						tooltip = Tooltip.FindInParents(transform);
					}
					ViewMode.IsPoliticalView();
					global::Realm realm = global::Realm.At(this.hits[i].point);
					if (realm != null && realm.IsSeaRealm())
					{
						this.picked_terrain_point = this.hits[i].point + ray.direction / ray.direction.y * (MapData.GetWaterLevel() - this.hits[i].point.y);
					}
					else
					{
						this.picked_terrain_point = this.hits[i].point;
					}
				}
			}
		}
		base.UpdateTooltip(tooltip);
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x0017E3B0 File Offset: 0x0017C5B0
	public bool IsInMultiplayerRelatedMenu()
	{
		return this.m_MultiplayerMenu.IsShown() || this.m_Lobby.activeInHierarchy || this.m_LobbyBrowser.activeInHierarchy || this.m_ChooseDisplayName.activeInHierarchy;
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x0017E3F0 File Offset: 0x0017C5F0
	public override void Select()
	{
		if (!ViewMode.IsPoliticalView())
		{
			return;
		}
		global::Realm realm = global::Realm.At(this.picked_terrain_point);
		global::Kingdom kingdom = (realm == null) ? null : realm.GetKingdom();
		ViewMode current = ViewMode.current;
		this.selected_realm = realm;
		if (realm != null && realm.IsSeaRealm())
		{
			this.SelectObj(null, false, true, true, true);
			return;
		}
		if (realm != null)
		{
			TitleMap titleMap = TitleMap.Get();
			if (titleMap != null)
			{
				titleMap.SetSelectedRealm(realm.id);
			}
		}
		if (realm != null && current.ShowRealmLabels())
		{
			this.SetSelectedRealm(realm.id, true);
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("select_kingdom_pv", null, "", true, true, true, '.') : null, null);
			return;
		}
		if (kingdom != null)
		{
			this.SelectKingdom(kingdom.id, true);
			DT.Field soundsDef2 = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("select_kingdom_pv", null, "", true, true, true, '.') : null, null);
		}
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x0017E4D8 File Offset: 0x0017C6D8
	public void SetSelectedRealm(int rid, bool reload_view = true)
	{
		global::Realm realm = global::Realm.Get(rid);
		this.SelectObj(null, false, false, true, true);
		this.selected_realm = realm;
		global::Realm realm2 = this.selected_realm;
		this.selected_logic_obj = ((realm2 != null) ? realm2.logic : null);
		TitleMap titleMap = TitleMap.Get();
		if (titleMap != null)
		{
			Logic.Realm realm3 = GameLogic.Get(true).GetRealm(rid);
			Logic.Kingdom kingdom = (realm3 != null) ? realm3.GetKingdom() : null;
			titleMap.SrcKingdom = ((kingdom != null) ? kingdom.id : 0);
			bool flag = titleMap.selected_realm == rid;
			titleMap.SetSelectedRealm(rid);
			if (flag)
			{
				titleMap.ReloadView();
				titleMap.UpdateSelectedBorders();
			}
		}
		UISingleplayerCampaign componentInChildren = base.GetComponentInChildren<UISingleplayerCampaign>();
		if (componentInChildren != null)
		{
			UISingleplayerCampaign uisingleplayerCampaign = componentInChildren;
			Logic.Kingdom kingdom2 = null;
			global::Realm realm4 = this.selected_realm;
			uisingleplayerCampaign.SelectKingdom(kingdom2, (realm4 != null) ? realm4.logic : null);
		}
		UILobby componentInChildren2 = base.GetComponentInChildren<UILobby>();
		if (componentInChildren2 != null)
		{
			UILobby uilobby = componentInChildren2;
			Logic.Kingdom kingdom3 = null;
			global::Realm realm5 = this.selected_realm;
			uilobby.SelectKingdom(kingdom3, (realm5 != null) ? realm5.logic : null);
		}
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x0017E5CC File Offset: 0x0017C7CC
	public void SelectKingdom(int kid, bool reload_view = true)
	{
		global::Kingdom kingdom = global::Kingdom.Get(kid);
		if (kingdom != null && kingdom.logic != null && kingdom.logic.type != Logic.Kingdom.Type.Regular)
		{
			return;
		}
		this.SelectObj(null, false, false, true, true);
		this.selected_kingdom = kingdom;
		if (this.selected_kingdom == null)
		{
			this.selected_logic_obj = null;
			if (reload_view)
			{
				MapData.UpdateSrcKingdom(true);
			}
			return;
		}
		this.selected_logic_obj = this.selected_kingdom.logic;
		if (reload_view)
		{
			MapData.UpdateSrcKingdom(true);
		}
		UISingleplayerCampaign componentInChildren = base.GetComponentInChildren<UISingleplayerCampaign>();
		if (componentInChildren != null)
		{
			UISingleplayerCampaign uisingleplayerCampaign = componentInChildren;
			Logic.Kingdom logic = this.selected_kingdom.logic;
			global::Realm realm = this.selected_realm;
			uisingleplayerCampaign.SelectKingdom(logic, (realm != null) ? realm.logic : null);
		}
		UILobby componentInChildren2 = base.GetComponentInChildren<UILobby>();
		if (componentInChildren2 != null)
		{
			UILobby uilobby = componentInChildren2;
			Logic.Kingdom logic2 = this.selected_kingdom.logic;
			global::Realm realm2 = this.selected_realm;
			uilobby.SelectKingdom(logic2, (realm2 != null) ? realm2.logic : null);
		}
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x0017E6A4 File Offset: 0x0017C8A4
	public void UpdateSelectedKingdom(Campaign campaign, int playerIndex)
	{
		if (campaign == null)
		{
			return;
		}
		string kingdomName = campaign.GetKingdomName(playerIndex, false);
		Game game = GameLogic.Get(false);
		Logic.Kingdom kingdom = (game != null) ? game.GetKingdom(kingdomName) : null;
		if (kingdom == null)
		{
			return;
		}
		if (!CampaignUtils.GetSelectedOption(campaign, "pick_kingdom").key.Contains("province"))
		{
			if (this.selected_kingdom == null || this.selected_kingdom.id != kingdom.id)
			{
				this.SelectKingdom(kingdom.id, true);
				return;
			}
		}
		else
		{
			Logic.Realm realm = GameLogic.Get(true).GetRealm(kingdom.Name);
			if (realm != null)
			{
				this.SetSelectedRealm(realm.id, true);
			}
		}
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x0017E740 File Offset: 0x0017C940
	public void UpdateSelectedKingdom(Campaign campaign, Logic.Kingdom kingdom)
	{
		if (campaign == null || kingdom == null)
		{
			return;
		}
		if (!(campaign.GetVar("pick_kingdom", null, true) == "province"))
		{
			if (this.selected_kingdom == null || this.selected_kingdom.id != kingdom.id)
			{
				this.SelectKingdom(kingdom.id, true);
				return;
			}
		}
		else
		{
			Logic.Realm realm = GameLogic.Get(true).GetRealm(kingdom.Name);
			if (realm != null)
			{
				this.SetSelectedRealm(realm.id, true);
			}
		}
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x0017E7BD File Offset: 0x0017C9BD
	public Campaign GetSelectedCampaignClient()
	{
		if (this.multiplayerMenu == null)
		{
			Debug.LogError("multiplayerMenu is null!");
			return null;
		}
		return this.multiplayerMenu.GetSelectedCampaign();
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x0017E7EC File Offset: 0x0017C9EC
	[CompilerGenerated]
	internal static void <LocalizeStatic>g__LocalzieButton|43_0(BSGButton b, string key)
	{
		if (b == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(b.gameObject, "id_Label");
		if (textMeshProUGUI == null)
		{
			return;
		}
		UIText.SetTextKey(textMeshProUGUI, "Title.buttons." + key, null, null);
	}

	// Token: 0x04001F27 RID: 7975
	[UIFieldTarget("id_MainMenuButtons")]
	public GameObject m_MainMenuButtons;

	// Token: 0x04001F28 RID: 7976
	[UIFieldTarget("btn_NewGame")]
	public BSGButton btnNewGame;

	// Token: 0x04001F29 RID: 7977
	[UIFieldTarget("btn_Continue")]
	public BSGButton btnContinue;

	// Token: 0x04001F2A RID: 7978
	[UIFieldTarget("btn_Load")]
	public BSGButton btnLoad;

	// Token: 0x04001F2B RID: 7979
	[UIFieldTarget("btn_Multiplayer")]
	public BSGButton btnMultiplayer;

	// Token: 0x04001F2C RID: 7980
	[UIFieldTarget("btn_Credits")]
	public BSGButton btnCredits;

	// Token: 0x04001F2D RID: 7981
	[UIFieldTarget("btn_Settings")]
	public BSGButton btnSettings;

	// Token: 0x04001F2E RID: 7982
	[UIFieldTarget("btn_Quit")]
	public BSGButton btnQuit;

	// Token: 0x04001F2F RID: 7983
	[UIFieldTarget("btn_SignOut")]
	public BSGButton btnSignOut;

	// Token: 0x04001F30 RID: 7984
	[UIFieldTarget("btn_SignIn")]
	public BSGButton btnSignIn;

	// Token: 0x04001F31 RID: 7985
	[UIFieldTarget("id_BackgroundImage")]
	private GameObject m_BackgroundImage;

	// Token: 0x04001F32 RID: 7986
	[UIFieldTarget("id_MainMenu")]
	private GameObject m_MainMenu;

	// Token: 0x04001F33 RID: 7987
	[UIFieldTarget("id_Preferences")]
	private UIPreferences m_Preferences;

	// Token: 0x04001F34 RID: 7988
	[UIFieldTarget("id_MultiplayerMenuWindow")]
	private UIMultiplayerMenu m_MultiplayerMenu;

	// Token: 0x04001F35 RID: 7989
	[UIFieldTarget("id_Lobby")]
	private GameObject m_Lobby;

	// Token: 0x04001F36 RID: 7990
	[UIFieldTarget("id_LobbyBrowser")]
	private GameObject m_LobbyBrowser;

	// Token: 0x04001F37 RID: 7991
	[UIFieldTarget("id_SingleplayerCampaign")]
	public GameObject m_SingleplayerCampaign;

	// Token: 0x04001F38 RID: 7992
	[UIFieldTarget("id_Login")]
	public GameObject m_Login;

	// Token: 0x04001F39 RID: 7993
	[UIFieldTarget("id_Register")]
	public GameObject m_Register;

	// Token: 0x04001F3A RID: 7994
	[UIFieldTarget("id_ForgotPasswordRequest")]
	public GameObject m_ForgotPasswordRequest;

	// Token: 0x04001F3B RID: 7995
	[UIFieldTarget("id_ForgotPasswordVerification")]
	public GameObject m_ForgotPasswordVerification;

	// Token: 0x04001F3C RID: 7996
	[UIFieldTarget("id_ChooseDisplayName")]
	public GameObject m_ChooseDisplayName;

	// Token: 0x04001F3D RID: 7997
	[UIFieldTarget("id_OfflineWarning")]
	public GameObject m_OfflineWarning;

	// Token: 0x04001F3E RID: 7998
	[UIFieldTarget("id_SingleplayerLogin")]
	public GameObject m_SingleplayerLogin;

	// Token: 0x04001F3F RID: 7999
	[UIFieldTarget("id_APINoConnection")]
	public GameObject m_APINoConnection;

	// Token: 0x04001F40 RID: 8000
	[UIFieldTarget("id_APIVersionMismatch")]
	public GameObject m_APIVersionMismatch;

	// Token: 0x04001F41 RID: 8001
	[UIFieldTarget("id_MultiplayerMenuWindow")]
	public UIMultiplayerMenu multiplayerMenu;

	// Token: 0x04001F42 RID: 8002
	[UIFieldTarget("id_Lobby")]
	public UILobby lobby;

	// Token: 0x04001F43 RID: 8003
	[UIFieldTarget("id_Credits")]
	public UICredits m_Credits;

	// Token: 0x04001F44 RID: 8004
	[UIFieldTarget("id_LogosAndText")]
	public GameObject m_LogosAndText;

	// Token: 0x04001F45 RID: 8005
	[UIFieldTarget("id_CopyrightLabel")]
	private TextMeshProUGUI m_CopyrightLabel;

	// Token: 0x04001F46 RID: 8006
	[UIFieldTarget("SignInBackground")]
	private GameObject SignInBackground;

	// Token: 0x04001F48 RID: 8008
	public global::Realm selected_realm;

	// Token: 0x04001F49 RID: 8009
	private Title title;

	// Token: 0x04001F4A RID: 8010
	private DT.Field window_def;

	// Token: 0x04001F4B RID: 8011
	private bool m_Initiazled;
}
