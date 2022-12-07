using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002CF RID: 719
public class MPBoss : IVars, IListener, RemoteVars.IListener
{
	// Token: 0x06002CFC RID: 11516 RVA: 0x00175E15 File Offset: 0x00174015
	public static MPBoss Get()
	{
		return MPBoss.Get(false);
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x00175E1D File Offset: 0x0017401D
	public static MPBoss Get(bool create_if_null)
	{
		if (MPBoss.instance == null && create_if_null)
		{
			MPBoss.instance = new MPBoss();
		}
		return MPBoss.instance;
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x00175E3C File Offset: 0x0017403C
	private MPBoss()
	{
		Logic.Multiplayer.DeleteLogFile();
		this.game = GameLogic.Get(true);
		this.game.AddListener(this);
		RemoteVars.data_changed = new RemoteVars.DataChangedCallback(this.DataChangedCallback);
		PlayerInfo.LoadNamesCache();
		PlayerInfo.on_player_name_changed = new Action<string, string>(this.PlayerNameChangedCallback);
		this.CreateMPCampaigns();
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x00175EC8 File Offset: 0x001740C8
	public void Start(bool manuallyCalled)
	{
		THQNORequest.on_sign_out_response = new THQNORequest.OnSignOutResponseCallback(this.OnSignOutCallback);
		THQNORequest.on_leave_lobby_callback = new THQNORequest.OnLeaveLobbyCallbackDelegate(this.OnLeaveLobbyCallback);
		this.meta_server = new Logic.Multiplayer(this.game, Logic.Multiplayer.Type.Server);
		this.meta_server.connectionReason = Logic.Multiplayer.ConnectionReason.Meta;
		this.meta_server.playerData.name = "MetaServer";
		this.pool = new MPBossMultiplayerPool(this);
		this.SetSystemState(MPBoss.SystemState.ConnectingToTHQ);
		this.signInCoro = Logic.Coroutine.Start("THQNOInitSignInAndCampaignsSetup", this.THQNOInitSignInAndCampaignsSetup(manuallyCalled), null);
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x00175F58 File Offset: 0x00174158
	private void OnSignInSuccess()
	{
		if (this.meta_server != null && this.meta_server.playerData != null)
		{
			this.meta_server.playerData.id = THQNORequest.userId;
		}
		MPBoss.StartCheckingMultiplayerEnabledRemotely();
		this.SetSystemState(MPBoss.SystemState.InitalizingCampaigns);
		Logic.Multiplayer multiplayer = this.meta_server;
		if (multiplayer != null)
		{
			multiplayer.StartServer(-1);
		}
		this.SetSystemState(MPBoss.SystemState.GettingCampaignData);
		this.LoadMPCampaignsFromDisk();
		this.LoadTHQNOCampaignsAndConnectAll();
		this.CheckForLateJoin();
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x00175FC8 File Offset: 0x001741C8
	public void OnSignOutCallback()
	{
		if (this.systemState != MPBoss.SystemState.SigningOut)
		{
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log(string.Format("OnSignOutCallback called with MPBoss of system state: {0}", this.systemState), 2);
			}
			THQNORequest.connected = false;
			if (this.game.state < Game.State.LoadingMap)
			{
				TitleUI titleUI = BaseUI.Get<TitleUI>();
				MessageWnd.OnUpdate updateCallback = delegate(MessageWnd w)
				{
					if (THQNORequest.signed_in)
					{
						w.Close(false);
					}
				};
				if (titleUI.IsInMultiplayerRelatedMenu())
				{
					titleUI.CloseAnyOpenMenus();
					string message_caption = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.error_caption", null, null, true, true);
					string message_body = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.thqno_disconnected", null, null, true, true);
					MPBoss mpboss = MPBoss.Get();
					if (mpboss != null)
					{
						mpboss.ShowUISystemMessageWindow(message_caption, message_body, updateCallback);
					}
				}
				else
				{
					string message_caption2 = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.error_caption", null, null, true, true);
					string message_body2 = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.thqno_disconnected_achievements", null, null, true, true);
					MPBoss mpboss2 = MPBoss.Get();
					if (mpboss2 != null)
					{
						mpboss2.ShowUISystemMessageWindow(message_caption2, message_body2, updateCallback);
					}
				}
			}
			if (this.IsInGame())
			{
				if (this.game.campaign != null && !this.game.campaign.IsMultiplayerCampaign())
				{
					string caption = "System.Network.ungracefully_disconnected";
					string body = "System.Network.achievements_disabled";
					string button = "System.Network.Disconnect.continue";
					Action btnAction = delegate()
					{
					};
					UIFallbackWindow.ShowDisconnectedWindow(caption, body, button, btnAction);
				}
				else
				{
					string caption2 = "System.Network.ungracefully_disconnected";
					string body2 = "TitleScreen.Multiplayer.Error.thqno_disconnected";
					Action btnAction2 = delegate()
					{
						LoadingScreen loadingScreen = LoadingScreen.Get();
						if (loadingScreen != null)
						{
							loadingScreen.Show(false, true, false);
						}
						UIFallbackWindow.DefaultButtonBehaviour();
					};
					UIFallbackWindow.ShowDisconnectedWindow(caption2, body2, null, btnAction2);
				}
			}
			Logic.Coroutine.Start("OnSignOut", this.OnSignOut(false, true), null);
		}
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x00176197 File Offset: 0x00174397
	private IEnumerator OnSignOut(bool runSignOut, bool runSignIn)
	{
		MPBoss.StopCheckingMultiplayerEnabledRemotely();
		PlayerInfo.registry.Clear();
		if (runSignOut)
		{
			this.SetSystemState(MPBoss.SystemState.SigningOut);
			yield return THQNORequest.SignOutCoro();
			this.SetSystemState(MPBoss.SystemState.SignedOut);
		}
		this.Stop();
		if (runSignIn)
		{
			this.Start(false);
		}
		yield break;
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x001761B4 File Offset: 0x001743B4
	public static void OnQuit()
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.CleanUp(true);
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x001761C8 File Offset: 0x001743C8
	private void CleanUp(bool quitting)
	{
		Game game = this.game;
		if (game != null)
		{
			game.DelListener(this);
		}
		MPBossMultiplayerPool mpbossMultiplayerPool = this.pool;
		if (mpbossMultiplayerPool != null)
		{
			mpbossMultiplayerPool.CleanUp(quitting);
		}
		this.pool = null;
		if (quitting)
		{
			Logic.Multiplayer multiplayer = this.meta_server;
			if (multiplayer != null)
			{
				multiplayer.OnQuit();
			}
		}
		else
		{
			Logic.Multiplayer multiplayer2 = this.meta_server;
			if (multiplayer2 != null)
			{
				multiplayer2.StopServer();
			}
		}
		this.meta_server = null;
		this.currently_joining_campaign = null;
		this.currently_entered_campaign = null;
		Game game2 = this.game;
		if (game2 != null)
		{
			game2.DestroyMultiplayer();
		}
		MPBoss.instance = null;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x00176253 File Offset: 0x00174453
	private void Stop()
	{
		MPBossMultiplayerPool mpbossMultiplayerPool = this.pool;
		if (mpbossMultiplayerPool != null)
		{
			mpbossMultiplayerPool.CleanUp(false);
		}
		Logic.Multiplayer multiplayer = this.meta_server;
		if (multiplayer != null)
		{
			multiplayer.StopServer();
		}
		this.meta_server = null;
		this.currently_joining_campaign = null;
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x00176286 File Offset: 0x00174486
	public static void OnUpdate()
	{
		MPBoss mpboss = MPBoss.instance;
		if (mpboss == null)
		{
			return;
		}
		MPBossMultiplayerPool mpbossMultiplayerPool = mpboss.pool;
		if (mpbossMultiplayerPool == null)
		{
			return;
		}
		mpbossMultiplayerPool.OnUpdate();
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x001762A1 File Offset: 0x001744A1
	public bool InitOnlineService()
	{
		THQNORequest.Connect();
		if (this.CheckForMajorTHQNOInitError(THQNORequest.lastConnectToOnlineServiceResult))
		{
			Debug.Log(string.Format("Application quitting, reason: {0}", THQNORequest.lastConnectToOnlineServiceResult));
			Application.Quit();
			return false;
		}
		return true;
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x001762D8 File Offset: 0x001744D8
	private bool CheckForMajorTHQNOInitError(Logic.Common.APIResult result)
	{
		bool result2 = false;
		if (result == Logic.Common.APIResult.Init_RestartRequired || result - Logic.Common.APIResult.Init_RequiredPluginNotLoaded <= 1)
		{
			result2 = true;
		}
		return result2;
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x001762F6 File Offset: 0x001744F6
	public void SetSystemState(MPBoss.SystemState state)
	{
		this.systemState = state;
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("MPBOSS system state: {0}", state), 2);
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x0017631D File Offset: 0x0017451D
	public MPBoss.SystemState GetSystemState()
	{
		return this.systemState;
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x00176325 File Offset: 0x00174525
	private IEnumerator THQNOInitSignInAndCampaignsSetup(bool manuallyCalled)
	{
		if (this.api_version_mismatch)
		{
			yield return Logic.Coroutine.Return(Value.Null, "Major API version mismatch");
		}
		yield return Logic.Coroutine.Start("THQNOConnect", THQNORequest.ConnectCoro(), null);
		string playerName = null;
		while (string.IsNullOrEmpty(playerName))
		{
			playerName = THQNO_Wrapper.GetPlatformPlayerName();
			yield return new Logic.Coroutine.WaitForMillis(100f);
		}
		string userChoice = string.Empty;
		Action<string> onUserChoice = delegate(string x)
		{
			userChoice = x;
		};
		if (!THQNORequest.connected)
		{
			if (manuallyCalled)
			{
				string reason = "not_connected";
				if (THQNORequest.lastConnectToOnlineServiceResult == Logic.Common.APIResult.Init_RequiredPluginNotLoaded)
				{
					reason = "no_platform";
				}
				TitleUI titleUI = BaseUI.Get<TitleUI>();
				if (titleUI != null)
				{
					titleUI.EnableOfflineWarningWindow(onUserChoice, reason);
				}
				while (string.IsNullOrEmpty(userChoice))
				{
					yield return null;
				}
			}
			MPBoss mpboss = MPBoss.Get();
			if (mpboss != null)
			{
				mpboss.CheckForSignInCoroAndTerminate();
			}
		}
		else
		{
			Logic.Common.ServiceAPIVersionMismatch apiVersionMismatchResult = THQNO_Wrapper.GetServiceAPIVersionMismatch();
			Logic.Common.ServiceAPIVersionMismatch serviceAPIVersionMismatch = apiVersionMismatchResult;
			if (serviceAPIVersionMismatch > Logic.Common.ServiceAPIVersionMismatch.UnableToCheck)
			{
				if (serviceAPIVersionMismatch == Logic.Common.ServiceAPIVersionMismatch.MajorVersionsMismatch)
				{
					TitleUI titleUI2 = BaseUI.Get<TitleUI>();
					if (titleUI2 != null)
					{
						titleUI2.EnableAPIVersionMismatchWindow(onUserChoice);
					}
					while (string.IsNullOrEmpty(userChoice))
					{
						yield return null;
					}
					Logic.Coroutine.current.Terminate("Major API version mismatch");
					this.api_version_mismatch = true;
					if (this.game != null && this.game.campaign != null && this.game.campaign.IsMultiplayerCampaign())
					{
						Game.State state = this.game.state;
						TitleUI titleUI3 = BaseUI.Get<TitleUI>();
						if (titleUI3 != null)
						{
							titleUI3.CloseAnyOpenMenus();
						}
						string message_caption = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.error_caption", null, null, true, true);
						string message_body = "#" + global::Defs.Localize("TitleScreen.Multiplayer.Error.api_version_mismatch", null, null, true, true);
						MPBoss mpboss2 = MPBoss.Get();
						if (mpboss2 != null)
						{
							mpboss2.ShowUISystemMessageWindow(message_caption, message_body, null);
						}
					}
				}
			}
			else if (manuallyCalled)
			{
				TitleUI titleUI4 = BaseUI.Get<TitleUI>();
				if (titleUI4 != null)
				{
					titleUI4.EnableAPINoConnectionWindow(onUserChoice);
				}
				while (string.IsNullOrEmpty(userChoice))
				{
					yield return null;
				}
			}
			else
			{
				string error = "THQNOInitSignInAndCampaignsSetup failed, no connection to THQNO. Attempting it again.";
				MPBoss.Error(error);
				yield return new Logic.Coroutine.WaitForMillis(1000f);
				this.signInCoro = Logic.Coroutine.Start("THQNOInitSignInAndCampaignsSetup", this.THQNOInitSignInAndCampaignsSetup(manuallyCalled), null);
				Logic.Coroutine.current.Terminate(error);
				error = null;
			}
			if (apiVersionMismatchResult != Logic.Common.ServiceAPIVersionMismatch.UnableToCheck && apiVersionMismatchResult != Logic.Common.ServiceAPIVersionMismatch.MajorVersionsMismatch)
			{
				if (manuallyCalled)
				{
					yield return Logic.Coroutine.Start("SignInOnButtonCoro", this.SignInOnButtonCoro(), null);
				}
				else
				{
					yield return Logic.Coroutine.Start("SignInOnStartupCoro", this.SignInOnStartupCoro(), null);
				}
			}
		}
		if (THQNORequest.signed_in)
		{
			this.OnSignInSuccess();
		}
		this.signInCoro = null;
		yield break;
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x0017633B File Offset: 0x0017453B
	public IEnumerator SignInOnStartupCoro()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		this.SetSystemState(MPBoss.SystemState.SigningIn);
		if (THQNORequest.platformType != Logic.Common.PlatformType.THQNO && THQNORequest.platformType != Logic.Common.PlatformType.NoDRM)
		{
			yield return Logic.Coroutine.Start("SignInPlatformCoro", this.SignInPlatformCoro(), null);
			if (THQNORequest.signed_in && THQNORequest.networkingAvailable)
			{
				string userId = THQNORequest.userId;
				string empty3 = string.Empty;
			}
			else
			{
				MPBoss.Error(string.Format("SignInPlatformCoro Fail. signed_in: {0}, networkingAvailable: {1}", THQNORequest.signed_in, THQNORequest.networkingAvailable));
			}
		}
		else
		{
			yield return THQNORequest.SignInWithCredentialsFile();
		}
		if (THQNORequest.signed_in && THQNORequest.networkingAvailable)
		{
			yield return THQNORequest.GetOwnPlayerNameCoro();
			string text = (string)Logic.Coroutine.Result.obj_val;
			if (string.IsNullOrEmpty(text))
			{
				text = THQNORequest.Utility.ReadTHQNODisplayName();
			}
			if (!string.IsNullOrEmpty(text))
			{
				THQNORequest.playerName = text;
				THQNORequest.Utility.WriteTHQNODisplayName(text);
				THQNORequest.onCallbackMessage = new THQNORequest.OnCallbackMessage(this.OnCallbackMessage);
			}
			else
			{
				yield return this.SignOut();
			}
		}
		else
		{
			yield return new Logic.Coroutine.WaitForMillis(1000f);
			yield return Logic.Coroutine.Start("SignInOnStartupCoro", this.SignInOnStartupCoro(), null);
		}
		yield break;
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x0017634A File Offset: 0x0017454A
	public IEnumerator SignInOnButtonCoro()
	{
		string status_message = string.Empty;
		string email = string.Empty;
		string password = string.Empty;
		string userChoice = string.Empty;
		string userParam1 = string.Empty;
		string userParam2 = string.Empty;
		Action<string, string, string> onUserChoice = delegate(string choice, string arg1, string arg2)
		{
			userChoice = choice;
			userParam1 = arg1;
			userParam2 = arg2;
		};
		bool signed = false;
		bool invalid_THQNO_response = false;
		if (THQNORequest.platformType != Logic.Common.PlatformType.THQNO && THQNORequest.platformType != Logic.Common.PlatformType.NoDRM)
		{
			yield return Logic.Coroutine.Start("SignInPlatformCoro", this.SignInPlatformCoro(), null);
			if (THQNORequest.signed_in)
			{
				signed = true;
				email = THQNORequest.userId;
				password = string.Empty;
			}
		}
		else
		{
			yield return THQNORequest.SignInWithCredentialsFile();
			if (Logic.Coroutine.Result.obj_val != null)
			{
				SignInResultCD signInResultCD = (SignInResultCD)Logic.Coroutine.Result.obj_val;
				if ((signInResultCD.result == Logic.Common.APIResult.Success && signInResultCD.networkingAvaliable) || signInResultCD.result == Logic.Common.APIResult.SignIn_AlreadySignedIn)
				{
					signed = true;
				}
				else if (!signInResultCD.result.ToString().StartsWith("SignIn_", StringComparison.Ordinal))
				{
					invalid_THQNO_response = true;
				}
			}
		}
		if (!signed && !invalid_THQNO_response)
		{
			THQNORequest.Utility.ReadTHQNOCredentials(out email, out password);
			while (!signed)
			{
				userChoice = string.Empty;
				userParam1 = string.Empty;
				userParam2 = string.Empty;
				TitleUI titleUI = BaseUI.Get<TitleUI>();
				if (titleUI != null)
				{
					titleUI.EnableLoginWindow(email, onUserChoice, status_message);
				}
				while (string.IsNullOrEmpty(userChoice))
				{
					yield return null;
				}
				email = userParam1;
				password = userParam2;
				status_message = string.Empty;
				if (userChoice == "login")
				{
					if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
					{
						THQNORequest.Utility.WriteTHQNOEMail(email);
						yield return THQNORequest.SignInEmailPWCoro(email, password);
						SignInResultCD signInResultCD2 = (Logic.Coroutine.Result.obj_val == null) ? default(SignInResultCD) : ((SignInResultCD)Logic.Coroutine.Result.obj_val);
						if ((signInResultCD2.result == Logic.Common.APIResult.Success && signInResultCD2.networkingAvaliable) || signInResultCD2.result == Logic.Common.APIResult.SignIn_AlreadySignedIn)
						{
							signed = true;
							THQNORequest.Utility.WriteTHQNOPassword(password);
							break;
						}
						if (signInResultCD2.result != Logic.Common.APIResult.Success || !signInResultCD2.networkingAvaliable)
						{
							status_message = UILogin.SignInResultToMessage(signInResultCD2);
							this.NotifyListeners("login_respose", status_message);
						}
					}
				}
				else if (userChoice == "register")
				{
					userChoice = string.Empty;
					TitleUI titleUI2 = BaseUI.Get<TitleUI>();
					if (titleUI2 != null)
					{
						titleUI2.DisableLoginWindow();
					}
					for (;;)
					{
						userChoice = string.Empty;
						TitleUI titleUI3 = BaseUI.Get<TitleUI>();
						if (titleUI3 != null)
						{
							titleUI3.EnableRegisterWindow(string.Empty, onUserChoice, status_message);
						}
						while (string.IsNullOrEmpty(userChoice))
						{
							yield return null;
						}
						email = userParam1;
						password = userParam2;
						if (!(userChoice == "register"))
						{
							goto IL_514;
						}
						yield return THQNORequest.RegisterEmailPWCoro(email, password, true, false);
						RegisterResultCD registerResultCD = (RegisterResultCD)Logic.Coroutine.Result.obj_val;
						if (registerResultCD.result == Logic.Common.APIResult.Success)
						{
							break;
						}
						status_message = UIRegister.RegisterResultToMessage(registerResultCD.result);
						this.NotifyListeners("register_response", status_message);
					}
					status_message = global::Defs.Localize("TitleScreen.Multiplayer.Login.StatusMessage.check_email", null, null, true, true);
					this.NotifyListeners("register_response", status_message);
					TitleUI titleUI4 = BaseUI.Get<TitleUI>();
					if (titleUI4 == null)
					{
						continue;
					}
					titleUI4.DisableRegisterWindow();
					continue;
					IL_514:
					status_message = string.Empty;
					this.NotifyListeners("register_response", status_message);
				}
				else if (userChoice == "forgot_password")
				{
					TitleUI titleUI5 = BaseUI.Get<TitleUI>();
					if (titleUI5 != null)
					{
						titleUI5.DisableLoginWindow();
					}
					for (;;)
					{
						userChoice = string.Empty;
						if (this.forgotPasswordRequestSent)
						{
							TitleUI titleUI6 = BaseUI.Get<TitleUI>();
							if (titleUI6 != null)
							{
								titleUI6.EnableForgotPasswordVerificationWindow(true, onUserChoice, status_message);
							}
						}
						else
						{
							TitleUI titleUI7 = BaseUI.Get<TitleUI>();
							if (titleUI7 != null)
							{
								titleUI7.EnableForgotPasswordRequestWindow(true, email, onUserChoice, status_message);
							}
						}
						while (string.IsNullOrEmpty(userChoice))
						{
							yield return null;
						}
						if (userChoice == "reset_password")
						{
							email = userParam1;
							yield return THQNORequest.StartPasswordRecovery(email);
							PasswordRecoveryStartResultCD passwordRecoveryStartResultCD = (PasswordRecoveryStartResultCD)Logic.Coroutine.Result.obj_val;
							if (passwordRecoveryStartResultCD.result == Logic.Common.APIResult.Success)
							{
								this.forgotPasswordRequestSent = true;
								TitleUI titleUI8 = BaseUI.Get<TitleUI>();
								if (titleUI8 != null)
								{
									titleUI8.EnableAutoForgotPasswordWindow(onUserChoice, email, status_message);
								}
							}
							else
							{
								status_message = UIForgotPasswordRequest.PasswordRecoveryResultToMessage(passwordRecoveryStartResultCD.result);
								this.NotifyListeners("forgot_password_response", status_message);
							}
						}
						else if (userChoice == "set_new_password")
						{
							string userParam = userParam1;
							string userParam3 = userParam2;
							yield return THQNORequest.ConfirmPasswordRecovery(userParam, userParam3);
							PasswordRecoveryResultCD passwordRecoveryResultCD = (PasswordRecoveryResultCD)Logic.Coroutine.Result.obj_val;
							if (passwordRecoveryResultCD.result == Logic.Common.APIResult.Success)
							{
								break;
							}
							status_message = UIForgotPasswordVerification.ConfirmPasswordRecoveryResultToMessage(passwordRecoveryResultCD.result);
							this.NotifyListeners("set_new_password_response", status_message);
						}
						else if (userChoice == "send_another_code")
						{
							this.forgotPasswordRequestSent = false;
						}
						else if (userChoice == "back")
						{
							goto Block_36;
						}
					}
					this.forgotPasswordRequestSent = false;
					status_message = string.Empty;
					BaseUI.Get<TitleUI>().EnableForgotPasswordVerificationWindow(false, null, null);
					continue;
					Block_36:
					password = string.Empty;
					BaseUI.Get<TitleUI>().EnableForgotPasswordVerificationWindow(false, null, null);
					BaseUI.Get<TitleUI>().EnableForgotPasswordRequestWindow(false, null, null, null);
				}
				else if (userChoice == "back")
				{
					TitleUI titleUI9 = BaseUI.Get<TitleUI>();
					if (titleUI9 == null)
					{
						break;
					}
					titleUI9.DisableLoginWindow();
					break;
				}
			}
		}
		if (signed && !invalid_THQNO_response)
		{
			TitleUI titleUI10 = BaseUI.Get<TitleUI>();
			if (titleUI10 != null)
			{
				titleUI10.DisableLoginWindow();
			}
			yield return THQNORequest.GetOwnPlayerNameCoro();
			string text = (string)Logic.Coroutine.Result.obj_val;
			if (string.IsNullOrEmpty(text))
			{
				text = THQNORequest.Utility.ReadTHQNODisplayName();
			}
			if (string.IsNullOrEmpty(text))
			{
				userChoice = string.Empty;
				userParam1 = string.Empty;
				userParam2 = string.Empty;
				status_message = string.Empty;
				string displayName;
				for (;;)
				{
					TitleUI titleUI11 = BaseUI.Get<TitleUI>();
					if (titleUI11 != null)
					{
						titleUI11.EnableChooseDisplayNameWindow(onUserChoice, status_message);
					}
					while (string.IsNullOrEmpty(userChoice))
					{
						yield return null;
					}
					if (userChoice == "choose_display_name")
					{
						displayName = userParam1;
						yield return THQNORequest.PlayerDataSetPersonalPlayerName(displayName);
						if (Logic.Coroutine.Result.obj_val == null)
						{
							string text2 = "PlayerDataSetPersonalPlayerName: Coroutine.Result.obj_val is null";
							if (Logic.Multiplayer.LogEnabled(2))
							{
								Logic.Multiplayer.Log(text2, 2, Game.LogType.Message);
							}
							yield return Logic.Coroutine.Return(default(Value), text2);
						}
						PlayerDataSetPersonalPlayerNameResultCD playerDataSetPersonalPlayerNameResultCD = (PlayerDataSetPersonalPlayerNameResultCD)Logic.Coroutine.Result.obj_val;
						if (playerDataSetPersonalPlayerNameResultCD.result == Logic.Common.APIResult.Success)
						{
							break;
						}
						Vars vars = new Vars();
						vars.Set<Logic.Common.APIResult>("result", playerDataSetPersonalPlayerNameResultCD.result);
						status_message = global::Defs.Localize("TitleScreen.Multiplayer.Register.StatusMessage.choose_display_name_fail", vars, null, true, true);
						this.NotifyListeners("choose_display_name_response", status_message);
						displayName = null;
					}
					else if (userChoice == "back")
					{
						goto Block_51;
					}
				}
				THQNORequest.playerName = displayName;
				THQNORequest.Utility.WriteTHQNODisplayName(displayName);
				TitleUI titleUI12 = BaseUI.Get<TitleUI>();
				if (titleUI12 == null)
				{
					goto IL_A52;
				}
				titleUI12.DisableChooseDisplayNameWindow();
				goto IL_A52;
				Block_51:
				TitleUI titleUI13 = BaseUI.Get<TitleUI>();
				if (titleUI13 != null)
				{
					titleUI13.DisableChooseDisplayNameWindow();
				}
			}
			else
			{
				THQNORequest.playerName = text;
				THQNORequest.Utility.WriteTHQNODisplayName(text);
			}
		}
		IL_A52:
		if (!signed || string.IsNullOrEmpty(THQNORequest.playerName) || invalid_THQNO_response)
		{
			string displayName = string.Empty;
			if (invalid_THQNO_response)
			{
				displayName = "server_error";
			}
			else if (!signed)
			{
				displayName = "not_signed_in";
			}
			else if (string.IsNullOrEmpty(THQNORequest.playerName))
			{
				displayName = "player_name_not_set";
			}
			yield return this.SignOut();
			userChoice = string.Empty;
			TitleUI titleUI14 = BaseUI.Get<TitleUI>();
			if (titleUI14 != null)
			{
				titleUI14.EnableOfflineWarningWindow(delegate(string choice)
				{
					userChoice = choice;
				}, displayName);
			}
			while (string.IsNullOrEmpty(userChoice))
			{
				yield return null;
			}
			displayName = null;
		}
		yield break;
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x00176359 File Offset: 0x00174559
	private IEnumerator SignInPlatformCoro()
	{
		if (THQNORequest.platformType != Logic.Common.PlatformType.THQNO && THQNORequest.platformType != Logic.Common.PlatformType.NoDRM)
		{
			yield return THQNORequest.RegisterPlatform();
			yield return THQNORequest.SignInPlatformCoro();
			if (!THQNORequest.signed_in || !THQNORequest.networkingAvailable)
			{
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log("SignInPlatformCoro() didn't sign in. Calling register and sign in again.", 2);
				}
				yield return THQNORequest.RegisterPlatform();
				yield return THQNORequest.SignInPlatformCoro();
			}
		}
		yield return Logic.Coroutine.Return(THQNORequest.signed_in, null);
		yield break;
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x00176361 File Offset: 0x00174561
	public IEnumerator SignOut()
	{
		Logic.Coroutine.OnFinish(delegate(Value result, string err)
		{
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			if (titleUI == null)
			{
				return;
			}
			titleUI.EnableOfflineWarningWindow(null, "not_signed_in");
		});
		yield return this.OnSignOut(true, false);
		string credentialsFilePath = THQNORequest.Utility.GetCredentialsFilePath();
		if (!string.IsNullOrEmpty(credentialsFilePath) && File.Exists(credentialsFilePath))
		{
			THQNORequest.Utility.WriteTHQNOPassword(string.Empty);
			THQNORequest.Utility.WriteTHQNODisplayName(string.Empty);
		}
		yield break;
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x00176370 File Offset: 0x00174570
	public void CheckForSignInCoroAndTerminate()
	{
		if (this.signInCoro != null)
		{
			if (this.signInCoro.name != "THQNOInitSignInAndCampaignsSetup")
			{
				MPBoss.Error("CheckForSignInCoroAndTerminate terminating a coroutine with name: " + this.signInCoro.name);
			}
			this.signInCoro.Terminate("Sign-out button clicked while signing in.");
			this.signInCoro = null;
			this.SetSystemState(MPBoss.SystemState.SignedOut);
		}
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x001763D4 File Offset: 0x001745D4
	public static bool MultiplayerEnabledRemotely()
	{
		return MPBoss.multiplayer_enabled_remotely > 0;
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x001763E0 File Offset: 0x001745E0
	public static int IsTHQDocumentMultiplayerEnabled(string document)
	{
		if (string.IsNullOrEmpty(document))
		{
			MPBoss.Error("IsTHQDocumentMultiplayerEnabled called with empty document");
			return -1;
		}
		foreach (string text in document.Split(new char[]
		{
			','
		}))
		{
			if (!string.IsNullOrEmpty(text))
			{
				string text2;
				string text3;
				MPBoss.ExtractKeyAndValue(text, out text2, out text3);
				if (string.IsNullOrEmpty(text2))
				{
					MPBoss.Error("IsTHQDocumentMultiplayerEnabled couldn't extract key from document " + document);
				}
				if (string.IsNullOrEmpty(text3))
				{
					MPBoss.Error("IsTHQDocumentMultiplayerEnabled couldn't extract value from document " + document);
				}
				if (text2 == "enable_multiplayer")
				{
					if (!(text3 == "true"))
					{
						return 0;
					}
					return 1;
				}
			}
		}
		return -1;
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x0017648C File Offset: 0x0017468C
	private static void ExtractKeyAndValue(string input, out string key, out string value)
	{
		key = null;
		value = null;
		if (string.IsNullOrEmpty(input))
		{
			MPBoss.Error("ExtractKeyAndValue called with empty document");
			return;
		}
		string[] array = input.Split(new char[]
		{
			':'
		});
		if (array.Length != 2)
		{
			MPBoss.Error("ExtractKeyAndValue called with parameter document in invalid JSON format! " + input);
			return;
		}
		key = array[0];
		value = array[1];
		char[] trimChars = new char[]
		{
			'{',
			'}',
			'"',
			'\'',
			' ',
			'\t'
		};
		key = key.Trim(trimChars);
		value = value.Trim(trimChars);
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x0017650B File Offset: 0x0017470B
	public static void StartCheckingMultiplayerEnabledRemotely()
	{
		if (!THQNORequest.signed_in)
		{
			return;
		}
		if (MPBoss.check_multiplayer_enabled_remotely_coro != null)
		{
			return;
		}
		if (Title.IsInternalBranch())
		{
			MPBoss.multiplayer_enabled_remotely = 2;
			return;
		}
		MPBoss.multiplayer_enabled_remotely = -2;
		MPBoss.check_multiplayer_enabled_remotely_coro = Logic.Coroutine.Start("CheckMultiplayerEnabledRemotely", MPBoss.<StartCheckingMultiplayerEnabledRemotely>g__Coro|43_0(), null);
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x00176547 File Offset: 0x00174747
	public static void StopCheckingMultiplayerEnabledRemotely()
	{
		MPBoss.multiplayer_enabled_remotely = -4;
		if (MPBoss.check_multiplayer_enabled_remotely_coro != null)
		{
			MPBoss.check_multiplayer_enabled_remotely_coro.Terminate(null);
			MPBoss.check_multiplayer_enabled_remotely_coro = null;
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x00176568 File Offset: 0x00174768
	public IEnumerator StartSingleplayer(Action on_complete)
	{
		Game.isJoiningGame = false;
		if (THQNORequest.signed_in && !string.IsNullOrEmpty(THQNORequest.playerName))
		{
			if (on_complete != null)
			{
				on_complete();
			}
			yield return Logic.Coroutine.Return(true, null);
		}
		else
		{
			MPBoss.<>c__DisplayClass45_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass45_0();
			CS$<>8__locals1.userChoice = string.Empty;
			Action<string> onUserChoice = delegate(string x)
			{
				CS$<>8__locals1.userChoice = x;
			};
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			if (titleUI != null)
			{
				titleUI.EnableSingleplayerLoginWindow(onUserChoice);
			}
			while (string.IsNullOrEmpty(CS$<>8__locals1.userChoice))
			{
				yield return null;
			}
			if (CS$<>8__locals1.userChoice == "login")
			{
				yield return this.SignInOnButtonCoro();
				if (THQNORequest.signed_in && !string.IsNullOrEmpty(THQNORequest.playerName))
				{
					if (on_complete != null)
					{
						on_complete();
					}
					yield return Logic.Coroutine.Return(true, null);
				}
			}
			else if (CS$<>8__locals1.userChoice == "continue_offline")
			{
				if (on_complete != null)
				{
					on_complete();
				}
				yield return Logic.Coroutine.Return(true, null);
			}
			else
			{
				CS$<>8__locals1.userChoice == "back";
			}
			CS$<>8__locals1 = null;
		}
		yield break;
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x00176580 File Offset: 0x00174780
	public void OnCallbackMessage(string msg, object param)
	{
		if (msg == "on_invite_response")
		{
			MPBoss.<>c__DisplayClass47_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass47_0();
			CS$<>8__locals1.<>4__this = this;
			if (!string.IsNullOrEmpty(MPBoss.currently_joining_lobby_id))
			{
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log("OnCallbackMessage() called while already joining a lobby.", 2);
				}
				THQNORequest.lobbyIdToJoin = string.Empty;
				MPBoss.currently_joining_lobby_id = string.Empty;
				return;
			}
			if (this.IsInGame())
			{
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log("OnCallbackMessage() called while in game. Aborting joining of a new campaign.", 2);
				}
				THQNORequest.lobbyIdToJoin = string.Empty;
				MPBoss.currently_joining_lobby_id = string.Empty;
				return;
			}
			if (!Title.allLoaded)
			{
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log("OnCallbackMessage() called while loading assets and defs. Aborting joining of a new campaign directly trough the callback. The Title.OnAllLoaded() will attempt again.", 2);
				}
				return;
			}
			CS$<>8__locals1.lobbyId = THQNORequest.lobbyIdToJoin;
			THQNORequest.lobbyIdToJoin = string.Empty;
			MPBoss.currently_joining_lobby_id = CS$<>8__locals1.lobbyId;
			Logic.Coroutine.Start("JoinCoro", CS$<>8__locals1.<OnCallbackMessage>g__JoinCoro|0(), null);
		}
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x0017665C File Offset: 0x0017485C
	public string GenerateInviteCode(Campaign campaign)
	{
		if (campaign == null)
		{
			MPBoss.Error("GenerateInviteCode() called with null campaign");
			return null;
		}
		if (campaign.state == Campaign.State.Empty || campaign.state > Campaign.State.Created)
		{
			MPBoss.Error(string.Format("GenerateInviteCode() called for campaign with invalid state: {0}", campaign));
			return null;
		}
		if (campaign.GetRuntimeState() < Campaign.RuntimeState.Lobby)
		{
			MPBoss.Error(string.Format("GenerateInviteCode() called for campaign with invalid runtime state: {0}", campaign));
			return null;
		}
		return campaign.id + "@" + campaign.GetOwnerID();
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x001766CC File Offset: 0x001748CC
	public Logic.Coroutine JoinCampaignTroughInviteCode(Campaign campaign, string invite_code)
	{
		MPBoss.<>c__DisplayClass49_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass49_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.invite_code = invite_code;
		CS$<>8__locals1.campaign = campaign;
		if (string.IsNullOrEmpty(CS$<>8__locals1.invite_code))
		{
			MPBoss.Error("JoinLobbyTroughInviteCode() called with invalid invite code");
			return null;
		}
		return Logic.Coroutine.Start("JoinCampaignCoro", CS$<>8__locals1.<JoinCampaignTroughInviteCode>g__JoinCampaignCoro|0(), null);
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x00176720 File Offset: 0x00174920
	private void ExtractCampaignAndOwnerId(string invite_code, out string campaign_id, out string owner_id)
	{
		campaign_id = null;
		owner_id = null;
		if (string.IsNullOrEmpty(invite_code))
		{
			MPBoss.Error("ExtractCampaignAndOwnerId() called with invalid invite code");
			return;
		}
		string[] array = invite_code.Split(new char[]
		{
			'@'
		});
		if (array.Length != 2)
		{
			MPBoss.Error("ExtractCampaignAndOwnerId() called with invalid invite code: " + invite_code);
			return;
		}
		campaign_id = array[0];
		owner_id = array[1];
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x00176779 File Offset: 0x00174979
	public void AddListener(IListener listener)
	{
		this.listeners.Add(listener);
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x00176787 File Offset: 0x00174987
	public void DelListener(IListener listener)
	{
		this.listeners.Remove(listener);
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x00176798 File Offset: 0x00174998
	public void NotifyListeners(string message, object param)
	{
		for (int i = this.listeners.Count - 1; i >= 0; i--)
		{
			this.listeners[i].OnMessage(this, message, param);
		}
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x001767D1 File Offset: 0x001749D1
	public void ClearMultiplayerLogic()
	{
		Game game = this.game;
		if (game == null)
		{
			return;
		}
		game.DestroyMultiplayer();
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x001767E3 File Offset: 0x001749E3
	public IEnumerator DestroyAllMultiplayers()
	{
		List<Logic.Multiplayer> mps = THQNORequest.GetAllMultiplayers();
		if (mps == null)
		{
			goto IL_B7;
		}
		List<Logic.Multiplayer> list = new List<Logic.Multiplayer>();
		foreach (Logic.Multiplayer item in mps)
		{
			list.Add(item);
		}
		using (List<Logic.Multiplayer>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Logic.Multiplayer multiplayer = enumerator.Current;
				if (multiplayer != null)
				{
					multiplayer.ShutDown();
				}
			}
			goto IL_B7;
		}
		IL_A0:
		yield return null;
		IL_B7:
		if (mps.Count <= 0)
		{
			yield break;
		}
		goto IL_A0;
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x001767EB File Offset: 0x001749EB
	public static bool LogEnabled(int level)
	{
		return Logic.Multiplayer.LogEnabled(level);
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x001767F3 File Offset: 0x001749F3
	public static void LogDataChange(string msg)
	{
		if (Logic.Multiplayer.LogEnabled(2))
		{
			Logic.Multiplayer.Log(msg, 2, Game.LogType.Message);
		}
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x00176805 File Offset: 0x00174A05
	public static void Log(string msg, int level)
	{
		if (Logic.Multiplayer.LogEnabled(level))
		{
			Logic.Multiplayer.Log(msg, level, Game.LogType.Message);
		}
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x00176817 File Offset: 0x00174A17
	public static void Warning(string msg)
	{
		Logic.Multiplayer.Warning(msg);
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x0017681F File Offset: 0x00174A1F
	public static void Error(string msg)
	{
		Logic.Multiplayer.Error(msg);
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x00176828 File Offset: 0x00174A28
	public List<string> GetRelevantPlayers()
	{
		this.relevant_players.Clear();
		if (this.multiplayerCampaigns == null)
		{
			return this.relevant_players;
		}
		for (int i = 0; i < this.multiplayerCampaigns.Length; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (campaign != null && campaign.state != Campaign.State.Empty)
			{
				if (campaign.state < Campaign.State.Started)
				{
					string ownerID = campaign.GetOwnerID();
					if (!string.IsNullOrEmpty(ownerID) && ownerID != THQNORequest.userId)
					{
						Container.AddUniqueUnsafe_Class<string>(this.relevant_players, ownerID);
					}
				}
				for (int j = 0; j < 6; j++)
				{
					string playerID = campaign.GetPlayerID(j, false);
					if (!string.IsNullOrEmpty(playerID) && !(playerID == THQNORequest.userId))
					{
						Container.AddUniqueUnsafe_Class<string>(this.relevant_players, playerID);
					}
				}
			}
		}
		return this.relevant_players;
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x001768F0 File Offset: 0x00174AF0
	public void UpdateRelevantPlayers()
	{
		if (this.GetSystemState() != MPBoss.SystemState.Initialized)
		{
			return;
		}
		List<string> relevantPlayers = this.GetRelevantPlayers();
		for (int i = 0; i < relevantPlayers.Count; i++)
		{
			string player_id = relevantPlayers[i];
			PlayerInfo playerInfo = PlayerInfo.Get(player_id, true);
			if (playerInfo.name == null)
			{
				playerInfo.name = "";
				Logic.Coroutine.Start("UpdateCampaignDataFromTHQ", this.UpdateCampaignDataFromTHQ(player_id), null);
			}
		}
		MPBossMultiplayerPool mpbossMultiplayerPool = this.pool;
		if (mpbossMultiplayerPool != null)
		{
			mpbossMultiplayerPool.UpdateMetaConnections(relevantPlayers);
		}
		this.RequestPoliticalDataIfNeeded();
	}

	// Token: 0x06002D27 RID: 11559 RVA: 0x0017696D File Offset: 0x00174B6D
	public Logic.Coroutine CheckForLateJoin()
	{
		return Logic.Coroutine.Start("CheckForLateJoin", this.<CheckForLateJoin>g__coro|68_0(), null);
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x00176980 File Offset: 0x00174B80
	public Logic.Coroutine LoadTHQNOCampaignsAndConnectAll()
	{
		return Logic.Coroutine.Start("LoadTHQNOCampaignsAndConnectAll", this.<LoadTHQNOCampaignsAndConnectAll>g__coro|69_0(), null);
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x00176994 File Offset: 0x00174B94
	public void CreateMPCampaigns()
	{
		this.multiplayerCampaigns = new Campaign[5];
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = new Campaign(i + 1);
			campaign.SetMultiplayer(true);
			campaign.AddVarsListener(this);
			this.multiplayerCampaigns[i] = campaign;
		}
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x001769DC File Offset: 0x00174BDC
	public void LoadMPCampaignsFromDisk()
	{
		DT.Field field = new DT.Field(null);
		DT.Parser.ReadFile(null, Campaign.MultiplayerRootDir() + "/Campaigns.def", field);
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null)
			{
				string @string = field.GetString(i.ToString(), null, "", true, true, true, '.');
				if (string.IsNullOrEmpty(@string))
				{
					campaign.Reset();
				}
				else
				{
					campaign.id = @string;
					campaign.Load(campaign.Dir());
				}
			}
		}
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x00176A5C File Offset: 0x00174C5C
	public void UpdateCampaignsListInGlobalData(List<string> send_data_changed_to = null)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(THQNORequest.userId, true);
		bool flag;
		if (playerInfo == null)
		{
			flag = (null != null);
		}
		else
		{
			RemoteVars persistent_global_vars = playerInfo.persistent_global_vars;
			flag = (((persistent_global_vars != null) ? persistent_global_vars.vars : null) != null);
		}
		if (!flag)
		{
			return;
		}
		playerInfo.name = THQNORequest.playerName;
		playerInfo.online_state = PlayerInfo.OnlineState.Online;
		playerInfo.persistent_global_vars.vars.Set<string>("name", playerInfo.name);
		Vars vars = playerInfo.persistent_global_vars.vars.Get<Vars>("campaigns", null);
		if (vars == null)
		{
			vars = new Vars();
			playerInfo.persistent_global_vars.vars.Set<Vars>("campaigns", vars);
		}
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			vars.Set<string>(i.ToString(), (campaign != null) ? campaign.id : null);
		}
		if (send_data_changed_to == null)
		{
			return;
		}
		string text = null;
		for (int j = 0; j < send_data_changed_to.Count; j++)
		{
			string text2 = send_data_changed_to[j];
			if (!string.IsNullOrEmpty(text2) && !(text2 == THQNORequest.userId) && this.IsPlayerOnline(text2))
			{
				if (text == null)
				{
					text = playerInfo.persistent_global_vars.vars.ToDTString();
				}
				this.SendDataChangedTo(text2, "", RemoteVars.DataType.GlobalPersistentPlayerData, THQNORequest.userId, text);
			}
		}
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x00176B92 File Offset: 0x00174D92
	private IEnumerator LoadCampaignsFromTHQNO()
	{
		if (!MPBoss.LOAD_CAMPAIGNS_FROM_THQNO)
		{
			yield break;
		}
		if (!THQNORequest.connected || !THQNORequest.signed_in || !THQNORequest.networkingAvailable)
		{
			yield break;
		}
		if (string.IsNullOrEmpty(THQNORequest.userId))
		{
			yield break;
		}
		yield return THQNORequest.PlayerDataGetCustomDataCoro(THQNORequest.userId);
		string text = Logic.Coroutine.Result;
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log("Loaded own campaigns data from THQNO:\n" + text, 2);
		}
		Vars vars = Vars.FromDTString(text);
		Vars vars2 = vars.Get<Vars>("campaigns", null);
		if (vars2 != null)
		{
			for (int j = 1; j <= 5; j++)
			{
				Campaign campaign = this.GetCampaign(j, false);
				if (campaign != null)
				{
					campaign.Reset();
					Vars vars3 = vars2.Get<Vars>(j.ToString(), null);
					if (vars3 != null)
					{
						campaign.LoadFromVars(vars3);
					}
				}
			}
		}
		if (vars.Get("name", true).String(null) != THQNORequest.playerName)
		{
			this.SaveCampaignsToTHQNO();
		}
		List<string> relevant_players = this.GetRelevantPlayers();
		int num;
		for (int i = 0; i < relevant_players.Count; i = num)
		{
			string text2 = relevant_players[i];
			if (!(text2 == THQNORequest.userId))
			{
				yield return this.UpdateCampaignDataFromTHQ(text2);
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x00176BA1 File Offset: 0x00174DA1
	private IEnumerator UpdateCampaignDataFromTHQ(string player_id)
	{
		yield return THQNORequest.PlayerDataGetCustomDataCoro(player_id);
		string text = Logic.Coroutine.Result;
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log("Loaded campaigns data of player " + player_id + " from THQNO:\n" + text, 2);
		}
		Vars vars = Vars.FromDTString(text);
		Vars vars2 = vars.Get<Vars>("campaigns", null);
		if (vars2 == null)
		{
			yield break;
		}
		PlayerInfo playerInfo = PlayerInfo.Get(player_id, true);
		Vars vars3;
		if (playerInfo == null)
		{
			vars3 = null;
		}
		else
		{
			RemoteVars persistent_global_vars = playerInfo.persistent_global_vars;
			if (persistent_global_vars == null)
			{
				vars3 = null;
			}
			else
			{
				Vars vars4 = persistent_global_vars.vars;
				vars3 = ((vars4 != null) ? vars4.Copy() : null);
			}
		}
		Vars vars5 = vars3 ?? new Vars();
		string text2 = vars.Get<string>("name", null);
		if (!string.IsNullOrEmpty(text2))
		{
			vars5.Set<string>("name", text2);
			playerInfo.name = text2;
		}
		Vars vars6 = new Vars();
		vars5.Set<Vars>("campaigns", vars6);
		for (int i = 1; i <= 5; i++)
		{
			string key = i.ToString();
			Vars vars7 = vars2.Get<Vars>(key, null);
			if (vars7 != null)
			{
				string text3 = vars7.Get<string>("id", null);
				if (!string.IsNullOrEmpty(text3))
				{
					vars6.Set<string>(key, text3);
					Campaign campaign = this.GetCampaign(text3);
					if (campaign != null)
					{
						this.OnFullCampaignData(campaign, vars7, player_id);
					}
				}
			}
		}
		this.OnDataChanged(player_id, null, RemoteVars.DataType.GlobalPersistentPlayerData, player_id, vars5, true);
		yield break;
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x00176BB8 File Offset: 0x00174DB8
	private void LeaveNotStartedCampaigns()
	{
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null && campaign.state != Campaign.State.Empty && campaign.state < Campaign.State.Started)
			{
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log(string.Format("Cleaning up non-started campaign {0}", campaign), 2);
				}
				this.LeaveCampaign(campaign, "not started");
			}
		}
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x00176C14 File Offset: 0x00174E14
	public void SaveCampaignsData(RemoteVars rvars)
	{
		if (rvars == null)
		{
			return;
		}
		if (this.multiplayerCampaigns == null)
		{
			return;
		}
		if (rvars.campaign == null)
		{
			return;
		}
		if (rvars.campaign.idx <= 0)
		{
			return;
		}
		if (!rvars.IsPersistent())
		{
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Saving campaigns data on vars changed: {0}", rvars), 2);
		}
		this.SaveCampaignsToDisk(rvars.campaign);
		this.SaveCampaignsToTHQNO();
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x00176C7C File Offset: 0x00174E7C
	public void SaveCampaignsToDisk(Campaign save_campaign)
	{
		if (this.multiplayerCampaigns == null)
		{
			MPBoss.Error("multiplayerCampaigns is null while trying to save campaigns to disk");
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (!string.IsNullOrEmpty((campaign != null) ? campaign.id : null))
			{
				stringBuilder.AppendLine(string.Format("{0} = \"{1}\"", i + 1, campaign.id));
				if (save_campaign == campaign)
				{
					campaign.Save(null);
					SaveGame.OnCampaignSaved(campaign);
				}
			}
		}
		File.WriteAllText(Campaign.MultiplayerRootDir() + "/Campaigns.def", stringBuilder.ToString());
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x00176D14 File Offset: 0x00174F14
	public void SaveCampaignsToTHQNO()
	{
		if (string.IsNullOrEmpty(THQNORequest.userId))
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("name", THQNORequest.playerName);
		Vars vars2 = new Vars();
		vars.Set<Vars>("campaigns", vars2);
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null)
			{
				Vars saveVars = campaign.GetSaveVars();
				if (saveVars != null)
				{
					vars2.Set<Vars>(i.ToString(), saveVars);
				}
			}
		}
		THQNORequest.PlayerDataSetCustomData(vars.ToDTString());
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x00176D98 File Offset: 0x00174F98
	public void ClearSaves(bool clear_thqno = true, bool clear_local = true, bool reset_campaigns = true)
	{
		if (reset_campaigns)
		{
			for (int i = 1; i <= 5; i++)
			{
				Campaign campaign = this.GetCampaign(i, false);
				if (campaign != null)
				{
					campaign.Reset();
				}
			}
		}
		if (clear_thqno)
		{
			THQNORequest.PlayerDataSetCustomData("");
		}
		if (clear_local)
		{
			string text = Campaign.MultiplayerRootDir();
			if (!string.IsNullOrEmpty(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				try
				{
					FileInfo[] files = directoryInfo.GetFiles();
					for (int j = 0; j < files.Length; j++)
					{
						files[j].Delete();
					}
					DirectoryInfo[] directories = directoryInfo.GetDirectories();
					for (int j = 0; j < directories.Length; j++)
					{
						directories[j].Delete(true);
					}
				}
				catch (Exception ex)
				{
					MPBoss.Error("Can't delete save files: " + ex.Message);
					return;
				}
			}
			SaveGame.ScanSavesDir(true);
		}
		if (reset_campaigns && !clear_local)
		{
			MPBoss.Log("Multiplayer campaigns reset.", 1);
			this.SaveCampaignsToDisk(null);
			this.UpdateRelevantPlayers();
			return;
		}
		if (clear_thqno && !reset_campaigns && !clear_local)
		{
			MPBoss.Log("Multiplayer campaigns reloaded from disk.", 1);
			this.LoadMPCampaignsFromDisk();
			this.ResendAllPersistentDataToOthers();
			this.LoadTHQNOCampaignsAndConnectAll();
			return;
		}
		if (clear_thqno || clear_local)
		{
			MPBoss.Log("Multiplayer saves cleared.", 1);
		}
		this.UpdateRelevantPlayers();
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x00176ECC File Offset: 0x001750CC
	private void ResendAllPersistentDataToOthers()
	{
		foreach (KeyValuePair<string, PlayerInfo> keyValuePair in PlayerInfo.registry)
		{
			string key = keyValuePair.Key;
			if (keyValuePair.Value.online)
			{
				this.SendRelevantDataTo(key);
			}
		}
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x00176F34 File Offset: 0x00175134
	private Campaign GetSinglePlayerCampaign()
	{
		Game game = this.game;
		Campaign campaign = (game != null) ? game.campaign : null;
		if (campaign == null)
		{
			return null;
		}
		if (campaign.IsMultiplayerCampaign())
		{
			return null;
		}
		return campaign;
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x00176F64 File Offset: 0x00175164
	private Campaign GetCampaign(int campaign_idx, bool allow_single_player = false)
	{
		if (campaign_idx == 0)
		{
			if (allow_single_player)
			{
				return this.GetSinglePlayerCampaign();
			}
			return null;
		}
		else
		{
			if (this.multiplayerCampaigns == null)
			{
				MPBoss.Error(string.Format("multiplayerCampaigns is null while getting campaign ({0})", campaign_idx));
				return null;
			}
			campaign_idx--;
			if (campaign_idx < 0 || campaign_idx >= this.multiplayerCampaigns.Length)
			{
				return null;
			}
			return this.multiplayerCampaigns[campaign_idx];
		}
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x00176FC0 File Offset: 0x001751C0
	private Campaign GetCampaign(string campaign_id)
	{
		if (string.IsNullOrEmpty(campaign_id))
		{
			return null;
		}
		Campaign singlePlayerCampaign = this.GetSinglePlayerCampaign();
		if (((singlePlayerCampaign != null) ? singlePlayerCampaign.id : null) == campaign_id)
		{
			return singlePlayerCampaign;
		}
		if (this.multiplayerCampaigns == null)
		{
			return null;
		}
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (((campaign != null) ? campaign.id : null) == campaign_id)
			{
				return campaign;
			}
		}
		return null;
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x0017702C File Offset: 0x0017522C
	public int GetCampaignIndex(string campaign_id)
	{
		if (string.IsNullOrEmpty(campaign_id))
		{
			MPBoss.Error("GetCampaignIndex() called with empty campaign_id");
			return -2;
		}
		Campaign singlePlayerCampaign = this.GetSinglePlayerCampaign();
		if (((singlePlayerCampaign != null) ? singlePlayerCampaign.id : null) == campaign_id)
		{
			return singlePlayerCampaign.idx;
		}
		if (this.multiplayerCampaigns == null)
		{
			MPBoss.Error("multiplayerCampaigns is null");
			return -1;
		}
		for (int i = 0; i < this.multiplayerCampaigns.Length; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (((campaign != null) ? campaign.id : null) == campaign_id)
			{
				return campaign.idx;
			}
		}
		return -1;
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x001770BC File Offset: 0x001752BC
	public Campaign FindFirstEmptyCampaign()
	{
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (campaign != null && campaign.state == Campaign.State.Empty)
			{
				return campaign;
			}
		}
		return null;
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x001770EC File Offset: 0x001752EC
	public int GetPlayerIndex(int campaign_idx, string player_id)
	{
		Campaign campaign = this.GetCampaign(campaign_idx, false);
		if (campaign == null)
		{
			return -2;
		}
		return campaign.GetPlayerIndex(player_id, true);
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x00177110 File Offset: 0x00175310
	public int GetLocalPlayerIndex(int campaign_idx)
	{
		Campaign campaign = this.GetCampaign(campaign_idx, true);
		if (campaign == null)
		{
			return -2;
		}
		return campaign.GetLocalPlayerIndex(true);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x00177134 File Offset: 0x00175334
	public RemoteVars GetRemoteVars(int campaign_idx, RemoteVars.DataType data_type, string player_id)
	{
		Campaign campaign = this.GetCampaign(campaign_idx, true);
		if (campaign == null)
		{
			return null;
		}
		return campaign.GetRemoteVars(data_type, player_id);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x00177158 File Offset: 0x00175358
	public RemoteVars GetLocalPlayerRemoteVars(int campaign_idx, RemoteVars.DataType data_type)
	{
		Campaign campaign = this.GetCampaign(campaign_idx, true);
		if (campaign == null)
		{
			return null;
		}
		return campaign.GetLocalPlayerRemoteVars(data_type);
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x0017717A File Offset: 0x0017537A
	public Campaign.RuntimeState GetLocalPlayerRuntimeState(Campaign campaign)
	{
		if (campaign == null)
		{
			MPBoss.Error("GetLocalPlayerRuntimeState() called with null campaign");
			return Campaign.RuntimeState.None;
		}
		return campaign.GetLocalPlayerRuntimeState();
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x00177194 File Offset: 0x00175394
	public void SetLocalPlayerRuntimeState(Campaign campaign, Campaign.RuntimeState runtime_state, string host_id)
	{
		if (campaign == null)
		{
			MPBoss.Error("SetLocalPlayerRuntimeState() called with null campaign");
			return;
		}
		if (runtime_state == Campaign.RuntimeState.Offline)
		{
			if (this.currently_entered_campaign == campaign)
			{
				this.currently_entered_campaign.thqno_lobby_id = null;
				this.currently_entered_campaign = null;
				this.game.campaign = null;
			}
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "ready", Value.Unknown, false);
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "start_countdown", Value.Unknown, false);
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "game_loaded", Value.Unknown, false);
		}
		else
		{
			this.currently_entered_campaign = campaign;
			this.game.campaign = campaign;
		}
		if (runtime_state == Campaign.RuntimeState.Lobby)
		{
			campaign.SetLocalPlayerReady(false, false);
		}
		if (string.IsNullOrEmpty(host_id))
		{
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "host_id", Value.Unknown, false);
		}
		else
		{
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "host_id", host_id, false);
		}
		campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "runtime_state", runtime_state.ToString(), true);
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x00177286 File Offset: 0x00175486
	public bool IsInGame()
	{
		return this.currently_entered_campaign != null && this.GetLocalPlayerRuntimeState(this.currently_entered_campaign) > Campaign.RuntimeState.Lobby;
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x001772A4 File Offset: 0x001754A4
	private void SetChangedVarInTHQNOLobby(RemoteVars rvars, string key, Value val)
	{
		if (rvars.data_type != RemoteVars.DataType.CampaignData)
		{
			return;
		}
		Campaign campaign = rvars.campaign;
		if (string.IsNullOrEmpty((campaign != null) ? campaign.thqno_lobby_id : null))
		{
			return;
		}
		if (key == null)
		{
			this.SetAllVarsInTHQNOLobby(campaign);
			return;
		}
		if (key == "players")
		{
			this.SetNumPlayersInTHQNOLobby(campaign);
		}
		DT.Field varDef = campaign.GetVarDef(key);
		if (varDef == null)
		{
			return;
		}
		if (!varDef.GetBool("set_in_thq", null, false, true, true, true, '.'))
		{
			return;
		}
		string a = varDef.Type();
		if (a == "string")
		{
			THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, varDef.key, val);
			return;
		}
		if (a == "int")
		{
			THQNORequest.SetLobbyIntData(campaign.thqno_lobby_id, varDef.key, val);
		}
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x00177368 File Offset: 0x00175568
	public void SetAllVarsInTHQNOLobby(Campaign campaign)
	{
		if (string.IsNullOrEmpty((campaign != null) ? campaign.thqno_lobby_id : null))
		{
			return;
		}
		THQNORequest.SetLobbyStringData(THQNORequest.currentlyEnteredTHQNOLobbyId, "campaign_id", campaign.id);
		THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "owner_name", THQNORequest.playerName);
		THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "owner_id", THQNORequest.userId);
		string value = Title.Version(true);
		if (!string.IsNullOrEmpty(value))
		{
			THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "client_version", value);
		}
		else
		{
			MPBoss.Error("Could not extract version while setting up THQNO lobby data!");
		}
		string value2 = Title.BranchName();
		if (!string.IsNullOrEmpty(value2))
		{
			THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "client_branch", value2);
		}
		else
		{
			MPBoss.Error("Could not extract branch name while setting up THQNO lobby data!");
		}
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "mod_id", modManager.activeModID);
		}
		THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, "creation_time", campaign.GetCreationTime().ToString("O"));
		this.SetNumPlayersInTHQNOLobby(campaign);
		DT.Field field = (campaign != null) ? campaign.GetVarsDef() : null;
		if (((field != null) ? field.children : null) == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field2 = field.children[i];
			if (!string.IsNullOrEmpty(field2.key) && field2.GetBool("set_in_thq", null, false, true, true, true, '.'))
			{
				Value val = campaign.campaignData.vars.Get(field2.key, true);
				string a = field2.Type();
				if (a == "string")
				{
					THQNORequest.SetLobbyStringData(campaign.thqno_lobby_id, field2.key, val);
				}
				else if (a == "int")
				{
					THQNORequest.SetLobbyIntData(campaign.thqno_lobby_id, field2.key, val);
				}
			}
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x00177558 File Offset: 0x00175758
	private void SetNumPlayersInTHQNOLobby(Campaign campaign)
	{
		if (string.IsNullOrEmpty((campaign != null) ? campaign.thqno_lobby_id : null))
		{
			return;
		}
		int numPlayers = campaign.GetNumPlayers(true);
		THQNORequest.SetLobbyIntData(campaign.thqno_lobby_id, "player_cnt", numPlayers);
		int maxPlayers = campaign.GetMaxPlayers();
		THQNORequest.SetLobbyIntData(campaign.thqno_lobby_id, "max_players", maxPlayers);
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x001775AC File Offset: 0x001757AC
	private void PlayerNameChangedCallback(string player_id, string player_name)
	{
		string[] param = new string[]
		{
			player_id,
			player_name
		};
		this.NotifyListeners("player_name_changed", param);
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x001775D4 File Offset: 0x001757D4
	private void DataChangedCallback(RemoteVars rvars, string key, Value val)
	{
		this.SetChangedVarInTHQNOLobby(rvars, key, val);
		this.SendDataChanged(rvars);
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x001775E8 File Offset: 0x001757E8
	public void SendDataChanged(RemoteVars rvars)
	{
		if (((rvars != null) ? rvars.campaign : null) == null)
		{
			return;
		}
		if (rvars.campaign.idx <= 0)
		{
			return;
		}
		if (((rvars != null) ? rvars.vars : null) == null)
		{
			MPBoss.Error("SendDataChanged() called with null vars");
			return;
		}
		string text = rvars.campaign.GetPlayerID(rvars.player_idx, false);
		if (string.IsNullOrEmpty(text))
		{
			text = Campaign.EncodePlayerIndexAsId(rvars.player_idx);
		}
		if (rvars.data_type == RemoteVars.DataType.CampaignData)
		{
			this.UpdateLatestSaveInfo(rvars.campaign);
		}
		this.SaveCampaignsData(rvars);
		string data = rvars.vars.ToDTString();
		this.SendDataChanged(rvars.campaign, rvars.data_type, text, data);
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x00177690 File Offset: 0x00175890
	private void SendFullCampaignData(Campaign campaign, string to_player_id)
	{
		if (string.IsNullOrEmpty((campaign != null) ? campaign.id : null))
		{
			MPBoss.Error("SendDataChanged() called with null campaign");
			return;
		}
		string dtsaveString = campaign.GetDTSaveString();
		this.SendDataChangedTo(to_player_id, campaign.id, RemoteVars.DataType.AllPersistentData, THQNORequest.userId, dtsaveString);
		RemoteVars localPlayerRemoteVars = campaign.GetLocalPlayerRemoteVars(RemoteVars.DataType.NonPersistentPlayerData);
		string text;
		if (localPlayerRemoteVars == null)
		{
			text = null;
		}
		else
		{
			Vars vars = localPlayerRemoteVars.vars;
			text = ((vars != null) ? vars.ToDTString() : null);
		}
		string data = text;
		this.SendDataChangedTo(to_player_id, campaign.id, RemoteVars.DataType.NonPersistentPlayerData, THQNORequest.userId, data);
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x0017770C File Offset: 0x0017590C
	private void SendDataChanged(Campaign campaign, RemoteVars.DataType data_type, string player_id, string data)
	{
		if (string.IsNullOrEmpty((campaign != null) ? campaign.id : null))
		{
			MPBoss.Error("SendDataChanged() called with null campaign");
			return;
		}
		if (campaign.playerIDs == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			string playerID = campaign.GetPlayerID(i, false);
			if (!string.IsNullOrEmpty(playerID) && !(playerID == THQNORequest.userId))
			{
				this.SendDataChangedTo(playerID, campaign.id, data_type, player_id, data);
			}
		}
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x0017777C File Offset: 0x0017597C
	private void SendDataChangedTo(string to_player_id, string campaign_id, RemoteVars.DataType data_type, string player_id, string data)
	{
		Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(to_player_id, Logic.Multiplayer.ConnectionReason.Meta);
		if (multiplayer == null)
		{
			return;
		}
		MPBoss.data_id++;
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.LogDataChange(string.Format("Sending [{0}]{1}({2}) to {3} for campaign {4}:\n{5}", new object[]
			{
				MPBoss.data_id,
				data_type,
				player_id,
				to_player_id,
				campaign_id,
				data
			}));
		}
		multiplayer.SendData(campaign_id, (int)data_type, player_id, data, MPBoss.data_id);
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x00177800 File Offset: 0x00175A00
	public void SendLobbyChat(Campaign campaign, string message)
	{
		if (((campaign != null) ? campaign.playerIDs : null) == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			string text = campaign.playerIDs[i];
			if (!string.IsNullOrEmpty(text) && !(text == THQNORequest.userId))
			{
				this.SendLobbyChatTo(text, campaign.id, message);
			}
		}
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x00177854 File Offset: 0x00175A54
	public void SendLobbyChatTo(string to_player_id, string campaign_id, string message)
	{
		Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(to_player_id, Logic.Multiplayer.ConnectionReason.Meta);
		if (multiplayer == null)
		{
			return;
		}
		multiplayer.SendChatMessage(THQNORequest.userId, message, Chat.Channel.Lobby, null, campaign_id);
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x00177884 File Offset: 0x00175A84
	private bool SendJoinCampaignRequest(int campaign_index, Logic.Multiplayer.JoinReason joinReason)
	{
		Campaign campaign = this.GetCampaign(campaign_index, false);
		if (campaign == null)
		{
			MPBoss.Error(string.Format("SendJoinRequest - cannot find a campaign for index: {0}", campaign_index));
			return false;
		}
		string id = campaign.id;
		if (string.IsNullOrEmpty(id))
		{
			MPBoss.Error("SendJoinRequest called with a campaign with null id");
			return false;
		}
		string ownerID = campaign.GetOwnerID();
		if (string.IsNullOrEmpty(ownerID))
		{
			MPBoss.Error(string.Format("SendJoinRequest - owner_id for campaign[{0}] is empty", campaign_index));
			return false;
		}
		return this.SendJoinCampaignRequest(id, ownerID, joinReason);
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x00177900 File Offset: 0x00175B00
	private bool SendJoinCampaignRequest(string campaign_id, string player_id, Logic.Multiplayer.JoinReason joinReason)
	{
		Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(player_id, Logic.Multiplayer.ConnectionReason.Meta);
		if (multiplayer == null)
		{
			MPBoss.Error("No connected multiplayer for join request " + player_id + ":" + campaign_id);
			return false;
		}
		multiplayer.SendJoinCampaignRequest(campaign_id, joinReason);
		return true;
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x00177940 File Offset: 0x00175B40
	private bool SendJoinCampaignRequestFailRеsponse(string response, string player_id)
	{
		Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(player_id, Logic.Multiplayer.ConnectionReason.Meta);
		if (multiplayer == null)
		{
			MPBoss.Error("No connected multiplayer for join request fail response " + player_id + ":" + response);
			return false;
		}
		multiplayer.SendJoinCampaignRequestFailResponse(response);
		return true;
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x0017797E File Offset: 0x00175B7E
	public IEnumerator CreateNewCampaign(Campaign campaign, Action<Campaign> onComplete = null)
	{
		if (campaign.state != Campaign.State.Empty)
		{
			yield break;
		}
		campaign.CreateNew(null, THQNORequest.userId, THQNORequest.playerName, true);
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Created new campaign: {0}", campaign), 2);
		}
		this.SaveCampaignsData(campaign.campaignData);
		this.UpdateCampaignsListInGlobalData(null);
		yield return this.CreateTHQNOLobby(campaign);
		if (onComplete != null)
		{
			onComplete(campaign);
		}
		yield break;
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x0017799C File Offset: 0x00175B9C
	public Logic.Coroutine CreateTHQNOLobby(Campaign campaign)
	{
		MPBoss.<>c__DisplayClass108_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass108_0();
		CS$<>8__locals1.campaign = campaign;
		CS$<>8__locals1.<>4__this = this;
		return Logic.Coroutine.Start("CreateTHQNOLobby", CS$<>8__locals1.<CreateTHQNOLobby>g__CreateTHQNOLobby|0(), null);
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x001779D0 File Offset: 0x00175BD0
	public void SetLobbyType(Campaign campaign, bool publish)
	{
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Publishing campaign {0}", campaign), 2);
		}
		Logic.Common.LobbyType lobbyType;
		if (publish)
		{
			lobbyType = Logic.Common.LobbyType.Public;
		}
		else
		{
			lobbyType = Logic.Common.LobbyType.Private;
		}
		THQNORequest.SetLobbyType(THQNORequest.currentlyEnteredTHQNOLobbyId, lobbyType);
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x00177A0C File Offset: 0x00175C0C
	public void LoadCampaignFromSave(string save_id)
	{
		if (this.game == null)
		{
			MPBoss.Error("LoadCampaignFromSave() called but game is null");
			return;
		}
		if (this.game.campaign == null)
		{
			MPBoss.Error("LoadCampaignFromSave() called but game.campaign is null");
			return;
		}
		if (string.IsNullOrEmpty(save_id))
		{
			MPBoss.Error("LoadCampaignFromSave() called with invalid save_id");
			return;
		}
		if (this.game.campaign.state != Campaign.State.Created)
		{
			MPBoss.Error(string.Format("LoadCampaignFromSave() called for campaign of state: {0}", this.game.campaign.state));
			return;
		}
		Campaign campaignData = SaveGame.GetCampaignData(save_id);
		this.game.campaign.CopySettings(campaignData, Campaign.ReasonForCopying.NewCampaign);
		this.game.campaign.CopyPlayerData(campaignData);
		this.game.campaign.SetFromSave(save_id);
		this.UpdateRelevantPlayers();
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x00177AD0 File Offset: 0x00175CD0
	public Campaign GetSourceCampaignForCurrentCampaign()
	{
		Campaign campaign = this.game.campaign;
		if (campaign == null)
		{
			return null;
		}
		string fromSaveID = campaign.GetFromSaveID(false);
		if (string.IsNullOrEmpty(fromSaveID))
		{
			return null;
		}
		return SaveGame.GetCampaignData(fromSaveID);
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x00177B08 File Offset: 0x00175D08
	public bool SwitchCampaignToOriginalMinorVictoryCondition()
	{
		Campaign campaign = this.game.campaign;
		if (campaign == null)
		{
			return false;
		}
		Campaign sourceCampaignForCurrentCampaign = this.GetSourceCampaignForCurrentCampaign();
		if (sourceCampaignForCurrentCampaign == null)
		{
			return false;
		}
		campaign.CopySettings(sourceCampaignForCurrentCampaign, Campaign.ReasonForCopying.RevertMinorVictoryConditionFromSave);
		return true;
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x00177B3C File Offset: 0x00175D3C
	public bool EnterLobby(Campaign campaign, Action<Campaign> onComplete = null)
	{
		if (campaign == null)
		{
			MPBoss.Error("EnterLobby called with a null campaign");
			return false;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Entering lobby for campaign {0}", campaign), 2);
		}
		string text = Title.Version(true);
		if (text == null)
		{
			MPBoss.Error("Could not extract client version on enter lobby!");
		}
		else
		{
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "client_version", text, false);
		}
		string text2 = Game.BranchName();
		if (text2 == null)
		{
			MPBoss.Error("Could not extract client branch on enter lobby!");
		}
		else
		{
			campaign.SetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "client_branch", text2, false);
		}
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.Lobby, null);
		if (onComplete != null)
		{
			onComplete(campaign);
		}
		return true;
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x00177BD8 File Offset: 0x00175DD8
	public Logic.Coroutine LeaveLobby(Campaign campaign, bool check_leave_campaign = true)
	{
		if (campaign == null)
		{
			MPBoss.Error("LeaveLobby called with a null campaign");
			return null;
		}
		if (check_leave_campaign && campaign.state < Campaign.State.Started)
		{
			this.LeaveCampaign(campaign, "leave lobby");
			return null;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Leaving lobby for campaign {0}", campaign), 2);
		}
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.Offline, null);
		return Logic.Coroutine.Start("LeaveLobby", MPBoss.<LeaveLobby>g__LeaveLobby|114_0(), null);
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x00177C40 File Offset: 0x00175E40
	public void OnLeaveLobbyCallback(string lobbyId, string memberChangedId, string memberIdMakingChange, Logic.Common.MemberLeftReason memberLeftReason)
	{
		if (Logic.Multiplayer.LogEnabled(2))
		{
			Logic.Multiplayer.Log(string.Format("lobbyId: {0}, memberChangedId: {1}, memberIdMakingChange: {2}, memberLeftReason: {3}", new object[]
			{
				lobbyId,
				memberChangedId,
				memberIdMakingChange,
				memberLeftReason
			}), 2, Game.LogType.Message);
		}
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x00177C78 File Offset: 0x00175E78
	public Logic.Coroutine JoinCampaign(Campaign campaign, string lobby_id, Logic.Multiplayer.JoinReason joinReason, int lobbyMaxPlayers = 0, Action<Campaign> onComplete = null)
	{
		MPBoss.<>c__DisplayClass116_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass116_0();
		CS$<>8__locals1.lobby_id = lobby_id;
		CS$<>8__locals1.lobbyMaxPlayers = lobbyMaxPlayers;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.campaign = campaign;
		CS$<>8__locals1.joinReason = joinReason;
		CS$<>8__locals1.onComplete = onComplete;
		if (CS$<>8__locals1.campaign == null)
		{
			MPBoss.Error("JoinCampaign called with a null campaign");
			return null;
		}
		if (CS$<>8__locals1.campaign.state != Campaign.State.Empty)
		{
			MPBoss.Error(string.Format("JoinCampaign called with a campaign of state {0}", CS$<>8__locals1.campaign.state));
			return null;
		}
		return Logic.Coroutine.Start("JoinCampaignCoro", CS$<>8__locals1.<JoinCampaign>g__JoinCampaignCoro|0(), null);
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x00177D0C File Offset: 0x00175F0C
	public Logic.Coroutine JoinCampaign(Campaign campaign, string campaign_id, string owner_id, Logic.Multiplayer.JoinReason joinReason, string lobby_id = null, Action<Campaign> onComplete = null)
	{
		MPBoss.<>c__DisplayClass117_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass117_0();
		CS$<>8__locals1.campaign = campaign;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.campaign_id = campaign_id;
		CS$<>8__locals1.owner_id = owner_id;
		CS$<>8__locals1.joinReason = joinReason;
		CS$<>8__locals1.lobby_id = lobby_id;
		CS$<>8__locals1.onComplete = onComplete;
		if (CS$<>8__locals1.campaign == null)
		{
			MPBoss.Error("JoinCampaign called with a null campaign");
			return null;
		}
		if (string.IsNullOrEmpty(CS$<>8__locals1.campaign_id))
		{
			MPBoss.Error("JoinCampaign called with invalid campaign_id");
			return null;
		}
		if (string.IsNullOrEmpty(CS$<>8__locals1.owner_id))
		{
			MPBoss.Error("JoinCampaign called with invalid owner_id");
			return null;
		}
		return Logic.Coroutine.Start("JoinCampaignCoro", CS$<>8__locals1.<JoinCampaign>g__JoinCampaignCoro|0(), null);
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x00177DAC File Offset: 0x00175FAC
	public void LeaveCampaign(Campaign campaign, string reason)
	{
		if (string.IsNullOrEmpty(campaign.id))
		{
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Leaving campaign ({0}): {1} ", reason, campaign), 2);
		}
		if (this.currently_joining_campaign == campaign)
		{
			this.currently_joining_campaign = null;
		}
		this.LeaveLobby(campaign, false);
		List<string> send_data_changed_to = null;
		if (campaign.playerIDs != null)
		{
			send_data_changed_to = new List<string>(campaign.playerIDs);
		}
		int localPlayerIndex = campaign.GetLocalPlayerIndex(true);
		if (localPlayerIndex >= 0)
		{
			Campaign.SlotState slotState = campaign.GetSlotState(localPlayerIndex);
			if (campaign.state >= Campaign.State.Started)
			{
				if (slotState != Campaign.SlotState.Kicked)
				{
					campaign.SetSlotState(localPlayerIndex, Campaign.SlotState.Left, true);
				}
			}
			else
			{
				campaign.SetSlotState(localPlayerIndex, Campaign.SlotState.Empty, true);
			}
		}
		if (campaign.state < Campaign.State.Started)
		{
			campaign.DeleteSavedGame();
		}
		campaign.Reset();
		this.SaveCampaignsData(campaign.campaignData);
		this.UpdateCampaignsListInGlobalData(send_data_changed_to);
		this.UpdateRelevantPlayers();
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x00177E74 File Offset: 0x00176074
	public void KickPlayer(Campaign campaign, int player_index)
	{
		if (!campaign.IsOwner())
		{
			MPBoss.Error(string.Format("Attempting to kick player while not an authority for campaign {0}", campaign));
			return;
		}
		string playerID = campaign.GetPlayerID(player_index, false);
		if (string.IsNullOrEmpty(playerID))
		{
			MPBoss.Error(string.Format("Attempting to kick unknown player {0} from campaign {1}", player_index, campaign));
			return;
		}
		PlayerInfo playerInfo = PlayerInfo.Get(playerID, true);
		Campaign.SlotState slotState = campaign.GetSlotState(player_index);
		if (!campaign.IsInCampaign(slotState) && slotState != Campaign.SlotState.Reserved)
		{
			MPBoss.Error(string.Format("Attempting to kick {0} player {1} ({2}) from campaign {3}", new object[]
			{
				slotState,
				player_index,
				playerInfo,
				campaign
			}));
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Kicking player {0} ({1}) from campaign {2}", player_index, playerInfo, campaign), 2);
		}
		Campaign.SlotState slot_state = (campaign.state >= Campaign.State.Started) ? Campaign.SlotState.Kicked : Campaign.SlotState.Empty;
		campaign.SetSlotState(player_index, slot_state, false);
		campaign.SetPlayerID(player_index, null, true);
		string data = campaign.campaignData.vars.ToDTString();
		this.SendDataChangedTo(playerID, campaign.id, RemoteVars.DataType.CampaignData, playerID, data);
		this.OnPlayerLeft(campaign, playerInfo);
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x00177F7C File Offset: 0x0017617C
	public void ToggleReady(Campaign campaign)
	{
		bool flag = campaign.IsLocalPlayerReady();
		campaign.SetLocalPlayerReady(!flag, true);
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x00177F9C File Offset: 0x0017619C
	public List<string> GetValidSlotActions(int player_index)
	{
		List<string> list = this.tmp_slot_actions;
		list.Clear();
		for (int i = 0; i < MPBoss.possible_slot_actions.Count; i++)
		{
			string text = MPBoss.possible_slot_actions[i];
			if (!(this.ValidateSlotAction(player_index, text) != "ok"))
			{
				list.Add(text);
			}
		}
		return list;
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x00177FF4 File Offset: 0x001761F4
	public string ValidateSlotAction(int player_index, string action)
	{
		Campaign campaign = this.currently_entered_campaign;
		if (campaign == null)
		{
			return "no_current_campaign";
		}
		int localPlayerIndex = campaign.GetLocalPlayerIndex(true);
		if (localPlayerIndex < 0)
		{
			return "not_in_campaign";
		}
		bool flag = campaign.GetOwnerIndex() == localPlayerIndex;
		bool flag2 = campaign.state >= Campaign.State.Started;
		Campaign.SlotState slotState = campaign.GetSlotState(player_index);
		string playerID = campaign.GetPlayerID(player_index, false);
		if (!(action == "open"))
		{
			if (!(action == "close"))
			{
				if (!(action == "kick"))
				{
					if (!(action == "invite"))
					{
						return "unknown_action";
					}
					if (campaign.state >= Campaign.State.Started)
					{
						return "campaign_started";
					}
					if (slotState == Campaign.SlotState.Closed)
					{
						return "slot_closed";
					}
					if (slotState == Campaign.SlotState.Joined || slotState == Campaign.SlotState.Eliminated || slotState == Campaign.SlotState.Left)
					{
						return "not_empty";
					}
					if (campaign.IsFromSave() && slotState == Campaign.SlotState.Reserved)
					{
						if (string.IsNullOrEmpty(playerID))
						{
							return "no_player_id";
						}
						if (string.IsNullOrEmpty(campaign.thqno_lobby_id))
						{
							return "no_lobby_id";
						}
					}
					return "ok";
				}
				else
				{
					if (!campaign.IsInCampaign(slotState) && slotState != Campaign.SlotState.Reserved)
					{
						return "not_in_campaign";
					}
					if (player_index == localPlayerIndex)
					{
						return "cant_kick_self";
					}
					if (!flag)
					{
						return "not_owner";
					}
					return "ok";
				}
			}
			else
			{
				if (campaign.IsInCampaign(slotState))
				{
					return "not_empty";
				}
				if (slotState == Campaign.SlotState.Closed)
				{
					return "already_closed";
				}
				if (flag2)
				{
					return "campaign_started";
				}
				if (!flag)
				{
					return "not_owner";
				}
				return "ok";
			}
		}
		else
		{
			if (slotState != Campaign.SlotState.Closed)
			{
				return "not_closed";
			}
			if (flag2)
			{
				return "campaign_started";
			}
			if (!flag)
			{
				return "not_owner";
			}
			return "ok";
		}
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x00178178 File Offset: 0x00176378
	public string PerformSlotAction(int player_index, string action)
	{
		string text = this.ValidateSlotAction(player_index, action);
		if (text != "ok")
		{
			return text;
		}
		Campaign campaign = this.currently_entered_campaign;
		if (campaign == null)
		{
			return "no_current_campaign";
		}
		if (action == "open")
		{
			campaign.SetSlotState(player_index, Campaign.SlotState.Empty, true);
			return "ok";
		}
		if (action == "close")
		{
			campaign.SetSlotState(player_index, Campaign.SlotState.Closed, true);
			return "ok";
		}
		if (action == "kick")
		{
			this.KickPlayer(campaign, player_index);
			RichPresence.Update(RichPresence.State.InLobby);
			return "ok";
		}
		if (!(action == "invite"))
		{
			return "unknown_action";
		}
		if (campaign.GetSlotState(player_index) == Campaign.SlotState.Reserved)
		{
			string playerID = campaign.GetPlayerID(player_index, false);
			THQNORequest.InviteUserToLobby(campaign.thqno_lobby_id, playerID);
			return "ok";
		}
		THQNO_Wrapper.ActivateInviteOverlay();
		return "ok";
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x0017824C File Offset: 0x0017644C
	private void ClearPlayerSlotData(Campaign campaign, int player_index)
	{
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "id", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "version", 0, true);
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "political_data_session_time", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "origin_realm", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "kingdom_name", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.PersistentPlayerData, player_index, "team", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, player_index, "id", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, player_index, "client_version", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, player_index, "client_branch", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, player_index, "ready", Value.Null, true);
		campaign.SetVar(RemoteVars.DataType.NonPersistentPlayerData, player_index, "runtime_state", Value.Null, true);
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x00178338 File Offset: 0x00176538
	public bool StartSinglePlayerGame()
	{
		Campaign campaign = this.game.campaign;
		if (campaign == null)
		{
			MPBoss.Error("missing campaign");
			return false;
		}
		string text = campaign.ValidateSetup(this.game);
		if (text != null)
		{
			MPBoss.Error("invalid campaign setup: " + text);
			return false;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Starting single player game for campaign {0}", campaign), 2);
		}
		Logic.Multiplayer multiplayer = this.game.multiplayer;
		if (multiplayer == null)
		{
			multiplayer = this.game.CreateMultiplayer(Logic.Multiplayer.Type.Server, 0);
		}
		else
		{
			MPBoss.Error(string.Format("Reusing multiplayer {0} for single player", multiplayer));
		}
		DT.Field selectedOption = CampaignUtils.GetSelectedOption(campaign, "pick_kingdom");
		CampaignUtils.PickRandomKingdoms(this.game, campaign, selectedOption);
		multiplayer.playerData.name = "SinglePlayer";
		multiplayer.playerData.kingdomName = campaign.GetKingdomName(true);
		multiplayer.playerData.team = 0;
		this.game.map_name = campaign.GetMapName();
		this.game.map_period = campaign.GetPeriod();
		this.game.map_from_save_id = null;
		this.game.political_data_session_time = 0;
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.Playing, campaign.GetLocalPlayerID());
		bool new_game = true;
		string fullPath = null;
		if (campaign.IsFromSave())
		{
			new_game = false;
			fullPath = SaveGame.GetSavePath(campaign.GetFromSaveID(false));
		}
		this.game.StartGame(new_game, this.game.map_name, fullPath);
		return true;
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x00178494 File Offset: 0x00176694
	public void StartGame(Campaign campaign)
	{
		if (campaign == null)
		{
			MPBoss.Error("StartGame called with a null campaign");
			return;
		}
		if (!campaign.IsMultiplayerCampaign())
		{
			this.StartSinglePlayerGame();
			return;
		}
		if (campaign.state != Campaign.State.Created && campaign.state != Campaign.State.Started)
		{
			MPBoss.Error(string.Format("StartGame called with a campaign of state {0}", campaign.state));
			return;
		}
		string text = campaign.CalcHostID();
		if (string.IsNullOrEmpty(text))
		{
			MPBoss.Error("StartGame called but cannot find host");
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Starting game for campaign {0}", campaign), 2);
		}
		if (text == THQNORequest.userId)
		{
			this.HostGame(campaign);
			return;
		}
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.PreparingToStart, text);
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x00178540 File Offset: 0x00176740
	private void HostGame(Campaign campaign)
	{
		if (this.pool == null)
		{
			MPBoss.Error("Critical: HostGame() called with null pool");
			return;
		}
		if (campaign == null)
		{
			MPBoss.Error("HostGame() called with null campaign");
			return;
		}
		RemoteVars remoteVars = campaign.GetRemoteVars(RemoteVars.DataType.CampaignData, -1);
		if (remoteVars == null)
		{
			MPBoss.Error(string.Format("Cannot find campaign data for campaign {0}", campaign));
			return;
		}
		string value = remoteVars.GetVar("state", null, true).String(null);
		if (string.IsNullOrEmpty(value))
		{
			MPBoss.Error("Empty campaign data \"state\"");
			return;
		}
		Campaign.State state = Campaign.State.Empty;
		Enum.TryParse<Campaign.State>(value, out state);
		if (state != Campaign.State.Created && state != Campaign.State.Started)
		{
			MPBoss.Error(string.Format("HostGame() called with campaign of state {0}", state));
			return;
		}
		if (!MPBoss.is_migrating_host)
		{
			string text = campaign.ValidateSetup(this.game);
			if (text != null)
			{
				MPBoss.Error("Invalid campaign settings: " + text);
				return;
			}
		}
		int playerIndex = campaign.GetPlayerIndex(THQNORequest.userId, true);
		if (playerIndex < 0 || playerIndex > 6)
		{
			MPBoss.Error(string.Format("Invalid player index {0} for campaign {1}", playerIndex, campaign));
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Hosting game for campaign {0}", campaign), 2);
		}
		Logic.Multiplayer multiplayer = this.game.CreateMultiplayer(Logic.Multiplayer.Type.Server, playerIndex);
		multiplayer.playerData.kingdomName = campaign.GetKingdomName(playerIndex, false);
		multiplayer.playerData.team = CampaignUtils.GetTeam(campaign, playerIndex);
		multiplayer.AddListener(this.pool);
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.StartingGame, THQNORequest.userId);
		Game.isJoiningGame = false;
		bool flag = campaign.state == Campaign.State.Created;
		string fullPath;
		if (campaign.IsFromSave())
		{
			flag = false;
			fullPath = SaveGame.GetSavePath(campaign.GetFromSaveID(false));
		}
		else
		{
			SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign.id);
			string text2;
			if (campaignInfo == null)
			{
				text2 = null;
			}
			else
			{
				SaveGame.Info latest_save = campaignInfo.latest_save;
				text2 = ((latest_save != null) ? latest_save.fullPath : null);
			}
			fullPath = text2;
		}
		this.SetLocalPlayerRuntimeState(campaign, Campaign.RuntimeState.Playing, THQNORequest.userId);
		campaign.SetState(Campaign.State.Started);
		if (multiplayer.playerData.kingdomName == "random")
		{
			DT.Field selectedOption = CampaignUtils.GetSelectedOption(campaign, "pick_kingdom");
			CampaignUtils.PickRandomKingdoms(this.game, campaign, selectedOption);
			multiplayer.playerData.kingdomName = campaign.GetKingdomName(playerIndex, false);
		}
		if (!flag)
		{
			this.game.load_game = Game.LoadedGameType.ContinueMultiplayerHost;
		}
		this.game.StartGame(flag, campaign.GetMapName(), fullPath);
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x00178770 File Offset: 0x00176970
	private Logic.Coroutine JoinGame(Campaign campaign, string host_id)
	{
		MPBoss.<>c__DisplayClass130_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass130_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.host_id = host_id;
		CS$<>8__locals1.campaign = campaign;
		if (CS$<>8__locals1.campaign == null)
		{
			MPBoss.Error("JoinGame() called with null campaign");
			return null;
		}
		if (string.IsNullOrEmpty(CS$<>8__locals1.host_id))
		{
			MPBoss.Error("JoinGame() called with empty host_id");
			return null;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Joining game for campaign {0}", CS$<>8__locals1.campaign), 2);
		}
		this.SetLocalPlayerRuntimeState(CS$<>8__locals1.campaign, Campaign.RuntimeState.StartingGame, CS$<>8__locals1.host_id);
		return Logic.Coroutine.Start("JoinGame", CS$<>8__locals1.<JoinGame>g__JoinGame|0(), null);
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x00178807 File Offset: 0x00176A07
	public IEnumerator JoinRunningGame(Campaign campaign)
	{
		if (campaign == null)
		{
			string text = "JoinRunningGame called with null campaign";
			MPBoss.Error(text);
			yield return Logic.Coroutine.Return(Value.Null, text);
		}
		Campaign.RuntimeState runtimeState = campaign.GetRuntimeState();
		if (runtimeState < Campaign.RuntimeState.PreparingToStart)
		{
			string text2 = string.Format("JoinRunningGame called for campaign of state {0}", runtimeState);
			MPBoss.Error(text2);
			yield return Logic.Coroutine.Return(Value.Null, text2);
		}
		int localPlayerIndex = campaign.GetLocalPlayerIndex(true);
		if (campaign.GetSlotState(localPlayerIndex) == Campaign.SlotState.Eliminated)
		{
			string text3 = "JoinRunningGame() called but local player is eliminated";
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log(text3, 2);
			}
			yield return Logic.Coroutine.Return(text3, Value.Null);
		}
		string host_id = campaign.GetHostID(true, false);
		if (string.IsNullOrEmpty(host_id))
		{
			string error = "Empty host_id";
			yield return Logic.Coroutine.Return(Value.Null, error);
		}
		int localPlayerIndex2 = campaign.GetLocalPlayerIndex(true);
		if (localPlayerIndex2 < 0)
		{
			string text4 = string.Format("Invalid local player index {0}", localPlayerIndex2);
			MPBoss.Error(text4);
			yield return Logic.Coroutine.Return(Value.Null, text4);
		}
		yield return this.JoinGame(campaign, host_id);
		yield break;
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x00178820 File Offset: 0x00176A20
	public void ReconnectToGame(Campaign campaign)
	{
		if (this.currentlyReconnecting)
		{
			return;
		}
		this.currentlyReconnecting = true;
		if (campaign == null)
		{
			MPBoss.Error("ReconnectToGame() called with null campaign");
			this.currentlyReconnecting = false;
			return;
		}
		string mapName = campaign.GetMapName();
		campaign.GetPeriod();
		this.game.LoadMap(mapName, this.game.map_period, !Game.isLoadingSaveGame && !Game.isJoiningGame);
		Logic.Coroutine.Start("ConnectAndJoinRunningGame", this.ConnectAndJoinRunningGame(campaign), null);
		this.currentlyReconnecting = false;
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x001788A3 File Offset: 0x00176AA3
	private IEnumerator ConnectAndJoinRunningGame(Campaign campaign)
	{
		do
		{
			this.UpdateRelevantPlayers();
			yield return new Logic.Coroutine.WaitForMillis(1000f);
			yield return this.JoinRunningGame(campaign);
		}
		while (Logic.Coroutine.Error != null);
		yield break;
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x001788BC File Offset: 0x00176ABC
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "player_presence_changed")
		{
			Value[] array = param as Value[];
			if (array == null || array.Length != 2)
			{
				MPBoss.Error("Invalid parameter on player_presence_changed");
				return;
			}
			string player_id = array[0];
			bool online = array[1];
			this.OnPlayerPresence(player_id, online);
			return;
		}
		else
		{
			if (message == "data_changed")
			{
				Value[] array2 = param as Value[];
				Value value = array2[0];
				Value value2 = array2[1];
				RemoteVars.DataType dataType = (RemoteVars.DataType)array2[2];
				string text = array2[3];
				Value value3 = array2[4];
				Value value4 = array2[5];
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.LogDataChange(string.Format("Received [{0}] {1}({2}) from {3} for campaign {4}:\n{5}", new object[]
					{
						value4,
						dataType,
						text,
						value2,
						value,
						value3
					}));
				}
				Vars new_vars = Vars.FromDTString(value3);
				this.OnDataChanged(value2, value, dataType, text, new_vars, true);
				return;
			}
			if (message == "lobby_chat_received")
			{
				Vars vars = param as Vars;
				if (vars == null)
				{
					return;
				}
				string campaign_id = vars.Get<string>("campaign_id", null);
				Campaign campaign = this.GetCampaign(campaign_id);
				Chat.ResolveCampaignVars(vars, campaign, null);
				UILobbyChat.AddChatMessage(campaign, vars);
				Game game = this.game;
				if (game == null)
				{
					return;
				}
				Logic.Multiplayer multiplayer = game.multiplayer;
				if (multiplayer == null)
				{
					return;
				}
				multiplayer.NotifyListeners("chat_message_received", vars);
				return;
			}
			else if (message == "join_request")
			{
				Value[] array3 = param as Value[];
				string text2 = array3[0].String(null);
				string text3 = array3[1].String(null);
				Logic.Multiplayer.JoinReason join_reason = (Logic.Multiplayer.JoinReason)((byte)array3[2]);
				if (string.IsNullOrEmpty(text2))
				{
					MPBoss.Error("Received join request with empty campaign id");
					return;
				}
				if (string.IsNullOrEmpty(text3))
				{
					MPBoss.Error("Received join request with empty player id");
					return;
				}
				Campaign campaign2 = this.GetCampaign(text2);
				if (campaign2 == null)
				{
					MPBoss.Error("Received join request for invalid campaign " + text2);
					return;
				}
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log(string.Format("Received join request from {0} for campaign {1}", text3, campaign2), 2);
				}
				string text4 = campaign2.ValidatePlayerJoin(text3, join_reason);
				if (text4 == "ok")
				{
					campaign2.JoinPlayer(text3, true);
					this.SendFullCampaignData(campaign2, text3);
					this.OnPlayerJoined(campaign2, text3);
					return;
				}
				this.SendJoinCampaignRequestFailRеsponse(text4, text3);
				return;
			}
			else if (message == "join_request_fail_response")
			{
				string text5 = (param as Value[])[0].String(null);
				if (this.currently_joining_campaign == null)
				{
					MPBoss.Error("Received join request fail response with empty request");
					return;
				}
				this.currently_joining_campaign = null;
				this.currently_joining_campaign_fail_response = text5;
				return;
			}
			else if (message == "on_request_political_data")
			{
				string text6 = param as string;
				PoliticalData latestPolitcalData = SaveGame.GetLatestPolitcalData(text6);
				if (latestPolitcalData == null)
				{
					MPBoss.Error("Requested missing political data for campaign " + text6);
					return;
				}
				(obj as Logic.Multiplayer).SendPoliticalData(text6, latestPolitcalData);
				return;
			}
			else if (message == "on_political_data")
			{
				object[] array4 = param as object[];
				string text7 = array4[0] as string;
				PoliticalData politicalData = array4[1] as PoliticalData;
				Campaign campaign3 = this.GetCampaign(text7);
				if (campaign3 == null)
				{
					return;
				}
				if (campaign3.requested_political_data_session_time <= politicalData.session_time)
				{
					campaign3.requested_political_data_from_player_id = null;
					campaign3.requested_political_data_session_time = 0;
				}
				if (SaveGame.UpdatePoliticalData(text7, politicalData))
				{
					this.UpdatePoliticalDataInfo(campaign3);
					this.NotifyListeners("political_data_changed", text7);
				}
				return;
			}
			else
			{
				if (!(message == "interupt_countdown"))
				{
					if (message == "player_connected")
					{
						Game game2 = GameLogic.Get(false);
						Vars vars2 = param as Vars;
						if (vars2 != null && game2 != null)
						{
							int pid = vars2.Get("pid", true);
							game2.pause.DelRequest("DisconnectPause", pid);
							game2.pause.DelRequest("QuitPause", pid);
							if (game2.multiplayer != null && vars2.Get("player_id", true) != game2.multiplayer.playerData.id)
							{
								string arg = vars2.Get("target", true);
								Value value5 = vars2.Get("kingdom", true);
								Debug.Log(string.Format("Player reconnected! - {0} as {1}", arg, value5));
								MessageIcon.CreatePlayerReconnected(vars2, true);
							}
						}
					}
					message == "ungraceful_disconnect";
					return;
				}
				Value[] array5 = param as Value[];
				string campaign_id2 = array5[0].String(null);
				string by_player_id = array5[1].String(null);
				Campaign campaign4 = this.GetCampaign(campaign_id2);
				if (campaign4 == null)
				{
					return;
				}
				UILobbyChat.AddInteruptCountdownMessage(campaign4, by_player_id);
				return;
			}
		}
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x00178D6C File Offset: 0x00176F6C
	private void CheckKicked(Campaign campaign)
	{
		if (campaign == null)
		{
			return;
		}
		Campaign campaign2 = this.currently_joining_campaign;
		if (((campaign2 != null) ? campaign2.id : null) == campaign.id)
		{
			return;
		}
		if (campaign.GetIndexInPlayers(THQNORequest.userId) >= 0)
		{
			return;
		}
		this.LeaveCampaign(campaign, "kicked");
		this.NotifyListeners("kicked_from_campaign", campaign);
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x00178DC4 File Offset: 0x00176FC4
	private void CheckJoinAccepted(Campaign campaign)
	{
		if (this.currently_joining_campaign == null)
		{
			return;
		}
		if (campaign == null)
		{
			return;
		}
		if (campaign != this.currently_joining_campaign)
		{
			return;
		}
		int indexInPlayerData = campaign.GetIndexInPlayerData(THQNORequest.userId);
		if (indexInPlayerData < 0)
		{
			return;
		}
		if (campaign.GetSlotState(indexInPlayerData) != Campaign.SlotState.Joined)
		{
			return;
		}
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log("Join request accepted.", 2);
		}
		this.currently_joining_campaign = null;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x00178E20 File Offset: 0x00177020
	private void CheckGameStarting(Campaign campaign, string player_id)
	{
		if (campaign == null || string.IsNullOrEmpty(player_id))
		{
			return;
		}
		if (this.currently_entered_campaign != campaign)
		{
			return;
		}
		if (campaign.GetLocalPlayerRuntimeState() >= Campaign.RuntimeState.StartingGame)
		{
			return;
		}
		Campaign.RuntimeState playerRuntimeState = campaign.GetPlayerRuntimeState(player_id);
		if (playerRuntimeState < Campaign.RuntimeState.PreparingToStart)
		{
			return;
		}
		string text = campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, player_id, "host_id");
		if (string.IsNullOrEmpty(text))
		{
			MPBoss.Error(string.Format("Ignored {0} without host_id", playerRuntimeState));
			return;
		}
		if (text == THQNORequest.userId)
		{
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log("Received host request from " + player_id, 2);
			}
			this.HostGame(campaign);
			return;
		}
		if (player_id == text)
		{
			if (MPBoss.LogEnabled(2))
			{
				MPBoss.Log("Received host notification from " + player_id, 2);
			}
			this.JoinGame(campaign, text);
		}
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x00178EE4 File Offset: 0x001770E4
	public void UpdateLatestSaveInfo()
	{
		if (this.multiplayerCampaigns == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			this.UpdateLatestSaveInfo(campaign);
		}
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x00178F18 File Offset: 0x00177118
	private void UpdateLatestSaveInfo(Campaign campaign)
	{
		if (campaign == null)
		{
			return;
		}
		RemoteVars localPlayerRemoteVars = campaign.GetLocalPlayerRemoteVars(RemoteVars.DataType.PersistentPlayerData);
		if (localPlayerRemoteVars == null)
		{
			return;
		}
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign.id);
		if (((campaignInfo != null) ? campaignInfo.latest_save : null) == null)
		{
			localPlayerRemoteVars.Set("latest_save_session_time", Value.Unknown, true);
		}
		else
		{
			localPlayerRemoteVars.Set("latest_save_session_time", campaignInfo.latest_save.session_time, true);
		}
		this.UpdatePoliticalDataInfo(campaign);
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x00178F88 File Offset: 0x00177188
	public void UpdatePoliticalDataInfo(Campaign campaign)
	{
		RemoteVars remoteVars = (campaign != null) ? campaign.GetLocalPlayerRemoteVars(RemoteVars.DataType.PersistentPlayerData) : null;
		if (remoteVars == null)
		{
			return;
		}
		PoliticalData latestPolitcalData = SaveGame.GetLatestPolitcalData(campaign.id);
		if (latestPolitcalData == null)
		{
			remoteVars.Set("political_data_session_time", Value.Unknown, true);
			return;
		}
		remoteVars.Set("political_data_session_time", latestPolitcalData.session_time, true);
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x00178FE0 File Offset: 0x001771E0
	public void RequestPoliticalDataIfNeeded()
	{
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null)
			{
				this.RequestPoliticalDataIfNeeded(campaign);
			}
		}
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x0017900C File Offset: 0x0017720C
	private string FindAnotherPlayerWithNewerPoliticalData(Campaign campaign, bool online_only, out int latest_session_time)
	{
		latest_session_time = 0;
		if (campaign == null)
		{
			return null;
		}
		if (campaign.state == Campaign.State.Empty)
		{
			return null;
		}
		if (campaign.state < Campaign.State.Started && campaign.IsOwner())
		{
			return null;
		}
		if (campaign.GetRuntimeState() > Campaign.RuntimeState.Lobby)
		{
			return null;
		}
		PoliticalData latestPolitcalData = SaveGame.GetLatestPolitcalData(campaign.id);
		latest_session_time = ((latestPolitcalData == null) ? 0 : latestPolitcalData.session_time);
		string result = null;
		for (int i = 0; i < 6; i++)
		{
			string playerID = campaign.GetPlayerID(i, false);
			if (!string.IsNullOrEmpty(playerID) && !(playerID == THQNORequest.userId) && (!online_only || this.IsPlayerOnline(playerID)))
			{
				int num = campaign.GetVar(RemoteVars.DataType.PersistentPlayerData, i, "political_data_session_time");
				if (num > latest_session_time)
				{
					result = playerID;
					latest_session_time = num;
				}
			}
		}
		return result;
	}

	// Token: 0x06002D70 RID: 11632 RVA: 0x001790BC File Offset: 0x001772BC
	private void RequestPoliticalDataIfNeeded(Campaign campaign)
	{
		int session_time;
		string text = this.FindAnotherPlayerWithNewerPoliticalData(campaign, true, out session_time);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.RequestPoliticalData(campaign, text, session_time);
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x001790E8 File Offset: 0x001772E8
	private void RequestPoliticalData(Campaign campaign, string from_player_id, int session_time)
	{
		if (campaign.requested_political_data_session_time >= session_time)
		{
			return;
		}
		Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(from_player_id, Logic.Multiplayer.ConnectionReason.Meta);
		if (multiplayer == null || !multiplayer.IsConnected())
		{
			return;
		}
		campaign.requested_political_data_from_player_id = from_player_id;
		campaign.requested_political_data_session_time = session_time;
		multiplayer.RequestPoliticalData(campaign.id);
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x00179133 File Offset: 0x00177333
	private void CancelPoliticalDataRequest(Campaign campaign, string player_id)
	{
		if (campaign.requested_political_data_from_player_id != player_id)
		{
			return;
		}
		campaign.requested_political_data_from_player_id = null;
		campaign.requested_political_data_session_time = 0;
		this.RequestPoliticalDataIfNeeded(campaign);
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06002D73 RID: 11635 RVA: 0x00179159 File Offset: 0x00177359
	public static bool is_migrating_host
	{
		get
		{
			return MPBoss.migrate_host_coro != null;
		}
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x00179164 File Offset: 0x00177364
	public void MigrateHost()
	{
		MPBoss.<>c__DisplayClass150_0 CS$<>8__locals1 = new MPBoss.<>c__DisplayClass150_0();
		CS$<>8__locals1.<>4__this = this;
		MPBoss.<>c__DisplayClass150_0 CS$<>8__locals2 = CS$<>8__locals1;
		Game game = this.game;
		CS$<>8__locals2.campaign = ((game != null) ? game.campaign : null);
		if (CS$<>8__locals1.campaign == null)
		{
			return;
		}
		if (CS$<>8__locals1.campaign.GetRuntimeState() < Campaign.RuntimeState.Playing)
		{
			return;
		}
		if (this.game.state == Game.State.Quitting)
		{
			return;
		}
		MPBoss.Log("HostMigration: Starting host migration ...", 2);
		CS$<>8__locals1.screenhot_tex = null;
		GameObject system_message_container = BaseUI.Get().system_message_container;
		CS$<>8__locals1.screenshot_img = global::Common.FindChildComponent<RawImage>(system_message_container, "id_Screenshot");
		MPBoss.migrate_host_coro = Logic.Coroutine.Start("MigrateHost", CS$<>8__locals1.<MigrateHost>g__MigrateHost|0(), null);
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x00179201 File Offset: 0x00177401
	public void CancelHostMigration()
	{
		if (MPBoss.migrate_host_coro == null)
		{
			return;
		}
		MPBoss.migrate_host_coro.Terminate("cancelled");
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x0017921C File Offset: 0x0017741C
	public bool IsPlayerOnline(string player_id)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(player_id, false);
		return playerInfo != null && playerInfo.online;
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x0017923C File Offset: 0x0017743C
	private void OnPlayerPresence(string player_id, bool online)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(player_id, true);
		if (playerInfo == null)
		{
			return;
		}
		playerInfo.online_state = (online ? PlayerInfo.OnlineState.Online : PlayerInfo.OnlineState.Offline);
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Presence changed: {0}", playerInfo), 2);
		}
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null)
			{
				int playerIndex = campaign.GetPlayerIndex(player_id, true);
				if (playerIndex >= 0 && campaign.GetRemoteVars(RemoteVars.DataType.NonPersistentPlayerData, playerIndex) != null && !online)
				{
					this.OnDataChanged(THQNORequest.userId, campaign.id, RemoteVars.DataType.NonPersistentPlayerData, player_id, MPBoss.empty_vars, false);
					this.CancelPoliticalDataRequest(campaign, player_id);
				}
			}
		}
		if (online)
		{
			this.SendRelevantDataTo(player_id);
		}
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x001792D8 File Offset: 0x001774D8
	private void SendGlobalPersistentDataTo(string to_player_id)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(THQNORequest.userId, false);
		if (playerInfo == null)
		{
			this.UpdateCampaignsListInGlobalData(null);
			playerInfo = PlayerInfo.Get(THQNORequest.userId, false);
		}
		bool flag;
		if (playerInfo == null)
		{
			flag = (null != null);
		}
		else
		{
			RemoteVars persistent_global_vars = playerInfo.persistent_global_vars;
			flag = (((persistent_global_vars != null) ? persistent_global_vars.vars : null) != null);
		}
		if (!flag)
		{
			return;
		}
		string data = playerInfo.persistent_global_vars.vars.ToDTString();
		this.SendDataChangedTo(to_player_id, "", RemoteVars.DataType.GlobalPersistentPlayerData, THQNORequest.userId, data);
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x00179348 File Offset: 0x00177548
	private void SendRelevantDataTo(string player_id)
	{
		this.SendGlobalPersistentDataTo(player_id);
		if (this.multiplayerCampaigns == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			if (campaign != null && campaign.GetIndexInPlayers(player_id) >= 0)
			{
				this.SendFullCampaignData(campaign, player_id);
			}
		}
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x00179390 File Offset: 0x00177590
	private void OnFullCampaignData(Campaign campaign, Vars new_vars, string sender_player_id)
	{
		if (string.IsNullOrEmpty((campaign != null) ? campaign.id : null))
		{
			return;
		}
		if (new_vars == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			string key = string.Format("player{0}", i);
			Vars vars = new_vars.Get<Vars>(key, null);
			this.tmp_plr_vars[i] = vars;
			new_vars.Del(key);
		}
		this.OnDataChanged(sender_player_id, campaign.id, RemoteVars.DataType.CampaignData, sender_player_id, new_vars, true);
		for (int j = 0; j < 6; j++)
		{
			Vars vars2 = this.tmp_plr_vars[j];
			string text = (vars2 != null) ? vars2.Get<string>("id", null) : null;
			if (string.IsNullOrEmpty(text))
			{
				text = Campaign.EncodePlayerIndexAsId(j);
			}
			this.OnDataChanged(sender_player_id, campaign.id, RemoteVars.DataType.PersistentPlayerData, text, vars2, sender_player_id != text);
		}
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x00179454 File Offset: 0x00177654
	private List<string> GetPlayerCampaignIDs(PlayerInfo pi)
	{
		MPBoss.tmp_player_campaign_ids.Clear();
		Vars vars;
		if (pi == null)
		{
			vars = null;
		}
		else
		{
			RemoteVars persistent_global_vars = pi.persistent_global_vars;
			if (persistent_global_vars == null)
			{
				vars = null;
			}
			else
			{
				Vars vars2 = persistent_global_vars.vars;
				vars = ((vars2 != null) ? vars2.Get<Vars>("campaigns", null) : null);
			}
		}
		Vars vars3 = vars;
		if (vars3 == null)
		{
			return MPBoss.tmp_player_campaign_ids;
		}
		for (int i = 1; i <= 5; i++)
		{
			string text = vars3.Get<string>(i.ToString(), null);
			if (!string.IsNullOrEmpty(text))
			{
				MPBoss.tmp_player_campaign_ids.Add(text);
			}
		}
		return MPBoss.tmp_player_campaign_ids;
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x001794D4 File Offset: 0x001776D4
	private void CheckPlayerLeft(PlayerInfo pi)
	{
		List<string> playerCampaignIDs = this.GetPlayerCampaignIDs(pi);
		for (int i = 1; i <= 5; i++)
		{
			Campaign campaign = this.GetCampaign(i, false);
			if (campaign != null && campaign.GetPlayerIndex(pi.id, true) >= 0 && !playerCampaignIDs.Contains(campaign.id))
			{
				this.OnPlayerLeft(campaign, pi);
			}
		}
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x00179528 File Offset: 0x00177728
	private void OnPlayerLeft(Campaign campaign, PlayerInfo pi)
	{
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Player {0} left campaign {1}", pi, campaign), 2);
		}
		this.OnDataChanged(THQNORequest.userId, campaign.id, RemoteVars.DataType.NonPersistentPlayerData, pi.id, MPBoss.empty_vars, false);
		int indexInPlayers = campaign.GetIndexInPlayers(pi.id);
		if (indexInPlayers >= 0)
		{
			Campaign.SlotState slot_state = (campaign.state >= Campaign.State.Started) ? Campaign.SlotState.Left : Campaign.SlotState.Empty;
			campaign.SetSlotState(indexInPlayers, slot_state, true);
			campaign.SetPlayerID(indexInPlayers, null, true);
		}
		if (campaign.state < Campaign.State.Started && pi.id == campaign.GetOwnerID())
		{
			this.LeaveCampaign(campaign, "owner left");
			this.NotifyListeners("owner_deleted_campaign", campaign);
		}
		this.UpdateRelevantPlayers();
		if (this.currently_entered_campaign == campaign && !this.IsInGame())
		{
			RichPresence.Update(RichPresence.State.InLobby);
		}
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x001795F0 File Offset: 0x001777F0
	private void OnPlayerGlobalData(string player_id, Vars new_vars)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(player_id, true);
		if (playerInfo == null)
		{
			return;
		}
		string text = new_vars.Get("name", true).String(null);
		if (!string.IsNullOrEmpty(text))
		{
			playerInfo.name = text;
		}
		RemoteVars persistent_global_vars = playerInfo.persistent_global_vars;
		if (persistent_global_vars != null)
		{
			persistent_global_vars.SetAll(new_vars, false);
		}
		this.CheckPlayerLeft(playerInfo);
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x00179648 File Offset: 0x00177848
	private void CheckPlayerJoined(Campaign campaign, string player_id, RemoteVars cur_vars, Vars new_vars)
	{
		if (cur_vars.data_type != RemoteVars.DataType.PersistentPlayerData)
		{
			return;
		}
		string a = cur_vars.GetVar("slot_state", null, true).String(null);
		string a2 = new_vars.GetVar("slot_state", null, true).String(null);
		if (a == "Joined" || a2 != "Joined")
		{
			return;
		}
		this.OnPlayerJoined(campaign, player_id);
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x001796B0 File Offset: 0x001778B0
	private void OnPlayerJoined(Campaign campaign, string player_id)
	{
		PlayerInfo playerInfo = PlayerInfo.Get(player_id, true);
		if (MPBoss.LogEnabled(2))
		{
			MPBoss.Log(string.Format("Player {0} joined campaign {1}", playerInfo, campaign), 2);
		}
		if (playerInfo != null && playerInfo.online_state == PlayerInfo.OnlineState.Online && player_id != THQNORequest.userId)
		{
			RemoteVars localPlayerRemoteVars = campaign.GetLocalPlayerRemoteVars(RemoteVars.DataType.NonPersistentPlayerData);
			string text;
			if (localPlayerRemoteVars == null)
			{
				text = null;
			}
			else
			{
				Vars vars = localPlayerRemoteVars.vars;
				text = ((vars != null) ? vars.ToDTString() : null);
			}
			string data = text;
			this.SendDataChangedTo(player_id, campaign.id, RemoteVars.DataType.NonPersistentPlayerData, THQNORequest.userId, data);
		}
		if (this.currently_entered_campaign == campaign && !this.IsInGame())
		{
			RichPresence.Update(RichPresence.State.InLobby);
		}
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x00179744 File Offset: 0x00177944
	private void OnDataChanged(string sender_player_id, string campaign_id, RemoteVars.DataType data_type, string player_id, Vars new_vars, bool check_version = true)
	{
		if (data_type == RemoteVars.DataType.GlobalPersistentPlayerData)
		{
			this.OnPlayerGlobalData(player_id, new_vars);
			return;
		}
		Campaign campaign = this.GetCampaign(campaign_id);
		if (campaign == null)
		{
			MPBoss.Error("Received data_changed for unknown campaign: " + campaign_id);
			return;
		}
		if (data_type == RemoteVars.DataType.AllPersistentData)
		{
			this.OnFullCampaignData(campaign, new_vars, player_id);
			return;
		}
		RemoteVars remoteVars = campaign.GetRemoteVars(data_type, player_id);
		if (remoteVars == null)
		{
			MPBoss.Error(string.Format("Received data_changed for campaign {0} with invalid remote vars: {1}({2})", campaign.idx, data_type, player_id));
			return;
		}
		if (check_version && remoteVars.IsPersistent())
		{
			int version = remoteVars.GetVersion();
			if (((new_vars == null) ? 0 : new_vars.Get("version", true).Int(0)) <= version)
			{
				return;
			}
		}
		this.CheckPlayerJoined(campaign, player_id, remoteVars, new_vars);
		if (data_type == RemoteVars.DataType.NonPersistentPlayerData)
		{
			string text = new_vars.Get("runtime_state", true);
			if (string.IsNullOrEmpty(text) || text == "Offline")
			{
				new_vars = MPBoss.empty_vars;
			}
		}
		Campaign.copying_settings = true;
		try
		{
			remoteVars.SetAll(new_vars, false);
		}
		catch (Exception ex)
		{
			MPBoss.Error(ex.ToString());
		}
		Campaign.copying_settings = false;
		this.CheckKicked(campaign);
		this.CheckJoinAccepted(campaign);
		this.UpdateLatestSaveInfo(campaign);
		if (data_type == RemoteVars.DataType.NonPersistentPlayerData)
		{
			this.CheckGameStarting(campaign, player_id);
		}
		this.SaveCampaignsData(remoteVars);
		this.UpdateRelevantPlayers();
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x00179894 File Offset: 0x00177A94
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (!(key == "slot_state"))
		{
			if (!(key == "from_save_id"))
			{
				return;
			}
			SaveGame.DeleteCachedPoliticalData(vars.campaign, old_val.String(null));
			this.UpdatePoliticalDataInfo(vars.campaign);
			this.RequestPoliticalDataIfNeeded();
			return;
		}
		else
		{
			if (vars.campaign != this.currently_entered_campaign)
			{
				return;
			}
			if (!this.IsInGame())
			{
				RichPresence.Update(RichPresence.State.InLobby);
			}
			if (!vars.campaign.IsOwner())
			{
				return;
			}
			this.SetNumPlayersInTHQNOLobby(vars.campaign);
			return;
		}
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x0017991C File Offset: 0x00177B1C
	public bool SendInteruptCountdown(Campaign campaign, string interupted_by_player_id)
	{
		if (((campaign != null) ? campaign.playerIDs : null) == null)
		{
			return false;
		}
		for (int i = 0; i < 6; i++)
		{
			string text = campaign.playerIDs[i];
			if (!string.IsNullOrEmpty(text))
			{
				Logic.Multiplayer multiplayer = this.pool.GetMultiplayer(text, Logic.Multiplayer.ConnectionReason.Meta);
				if (multiplayer != null)
				{
					multiplayer.SendInteruptCountdown(campaign.id, interupted_by_player_id);
				}
			}
		}
		return true;
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x00179978 File Offset: 0x00177B78
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num <= 1286315866U)
		{
			if (num <= 564676416U)
			{
				if (num != 168538109U)
				{
					if (num != 449857922U)
					{
						if (num == 564676416U)
						{
							if (key == "campaigns")
							{
								return new Value(this.multiplayerCampaigns);
							}
						}
					}
					else if (key == "joining_campaign")
					{
						return new Value(this.currently_joining_campaign);
					}
				}
				else if (key == "player_info")
				{
					return new Value(PlayerInfo.Get(THQNORequest.userId, false));
				}
			}
			else if (num != 886072328U)
			{
				if (num != 1274325182U)
				{
					if (num == 1286315866U)
					{
						if (key == "players_info")
						{
							return new Value(PlayerInfo.registry);
						}
					}
				}
				else if (key == "debug_text")
				{
					return this.Dump();
				}
			}
			else if (key == "multiplayers")
			{
				return new Value(THQNORequest.GetAllMultiplayers());
			}
		}
		else if (num <= 2447210883U)
		{
			if (num != 1988002311U)
			{
				if (num != 2276502158U)
				{
					if (num == 2447210883U)
					{
						if (key == "pool")
						{
							return new Value(this.pool);
						}
					}
				}
				else if (key == "player_name")
				{
					return THQNORequest.playerName;
				}
			}
			else if (key == "entered_campaign")
			{
				return new Value(this.currently_entered_campaign);
			}
		}
		else if (num != 2765661720U)
		{
			if (num != 3156083493U)
			{
				if (num == 4130653598U)
				{
					if (key == "connections")
					{
						MPBossMultiplayerPool mpbossMultiplayerPool = this.pool;
						return new Value((mpbossMultiplayerPool != null) ? mpbossMultiplayerPool.virtualConnections : null);
					}
				}
			}
			else if (key == "sp_campaign")
			{
				return new Value(this.GetSinglePlayerCampaign());
			}
		}
		else if (key == "player_id")
		{
			return THQNORequest.userId;
		}
		return Value.Unknown;
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x00179BD0 File Offset: 0x00177DD0
	public void ShowUISystemMessageWindow(string message_caption, string message_body, MessageWnd.OnUpdate updateCallback = null)
	{
		Vars vars = new Vars();
		vars.Set<string>("message_caption", message_caption);
		vars.Set<string>("message_body", message_body);
		MessageWnd.Create(global::Defs.GetDefField("SystemMessage", null), vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			wnd.Close(false);
			return true;
		}).on_update = updateCallback;
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x00179C32 File Offset: 0x00177E32
	public override string ToString()
	{
		return string.Format("MPBoss({0})", this.systemState);
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x00179C4C File Offset: 0x00177E4C
	public string Dump()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("#[{0}]", this.systemState));
		stringBuilder.AppendLine("Multiplayer Campaigns:");
		for (int i = 0; i < 5; i++)
		{
			Campaign campaign = this.multiplayerCampaigns[i];
			string arg = ((campaign != null) ? campaign.ToString() : null) ?? "null";
			stringBuilder.AppendLine(string.Format("  {0}: {1}", i + 1, arg));
		}
		stringBuilder.AppendLine("Local Player: [" + THQNORequest.userId + "] " + THQNORequest.playerName);
		stringBuilder.AppendLine("Players Info:");
		foreach (KeyValuePair<string, PlayerInfo> keyValuePair in PlayerInfo.registry)
		{
			PlayerInfo value = keyValuePair.Value;
			stringBuilder.AppendLine(string.Format("  {0}", value));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x00179DD8 File Offset: 0x00177FD8
	[CompilerGenerated]
	internal static IEnumerator <StartCheckingMultiplayerEnabledRemotely>g__Coro|43_0()
	{
		while (THQNORequest.signed_in)
		{
			if (BaseUI.Get<TitleUI>() == null)
			{
				MPBoss.check_multiplayer_enabled_remotely_coro = null;
				yield break;
			}
			int enable = -1;
			yield return THQNORequest.GetPublicReadDocument("Settings");
			if (Logic.Coroutine.Result.obj_val != null)
			{
				string document = Logic.Coroutine.Result.obj_val.ToString();
				enable = MPBoss.IsTHQDocumentMultiplayerEnabled(document);
			}
			else
			{
				string msg = string.Format("DocumentReadTestCoro obj_val is null. Coroutine.Result is {0}", Logic.Coroutine.Result);
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log(msg, 2);
				}
			}
			if (enable != MPBoss.multiplayer_enabled_remotely)
			{
				MPBoss.multiplayer_enabled_remotely = enable;
				if (MPBoss.LogEnabled(2))
				{
					MPBoss.Log(string.Format("Multiplayer enabled remotely: {0}", MPBoss.multiplayer_enabled_remotely), 2);
				}
			}
			yield return new Logic.Coroutine.WaitForMillis(30000f);
		}
		MPBoss.check_multiplayer_enabled_remotely_coro = null;
		yield break;
		yield break;
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x00179DE0 File Offset: 0x00177FE0
	[CompilerGenerated]
	private IEnumerator <CheckForLateJoin>g__coro|68_0()
	{
		if (this.IsInGame() && this.currently_entered_campaign.IsMultiplayerCampaign())
		{
			string text = this.currently_entered_campaign.GetLocalPlayerVar(RemoteVars.DataType.NonPersistentPlayerData, "host_id");
			if (text == THQNORequest.userId)
			{
				yield break;
			}
			yield return this.JoinGame(this.currently_entered_campaign, text);
		}
		yield break;
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x00179DEF File Offset: 0x00177FEF
	[CompilerGenerated]
	private IEnumerator <LoadTHQNOCampaignsAndConnectAll>g__coro|69_0()
	{
		yield return this.LoadCampaignsFromTHQNO();
		this.LeaveNotStartedCampaigns();
		this.UpdateLatestSaveInfo();
		this.UpdateCampaignsListInGlobalData(null);
		this.SetSystemState(MPBoss.SystemState.Initialized);
		this.UpdateRelevantPlayers();
		yield break;
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x00179DFE File Offset: 0x00177FFE
	[CompilerGenerated]
	internal static IEnumerator <LeaveLobby>g__LeaveLobby|114_0()
	{
		if (!string.IsNullOrEmpty(THQNORequest.currentlyEnteredTHQNOLobbyId))
		{
			yield return Logic.Coroutine.Start("THQNORequest.LeaveCurrentLobby", THQNORequest.LeaveCurrentLobbyCoro(), null);
		}
		yield break;
	}

	// Token: 0x04001EB3 RID: 7859
	public const int MAX_CAMPAIGNS = 5;

	// Token: 0x04001EB4 RID: 7860
	public static bool LOAD_CAMPAIGNS_FROM_THQNO = true;

	// Token: 0x04001EB5 RID: 7861
	public const char INVITE_CODE_SEPARATOR = '@';

	// Token: 0x04001EB6 RID: 7862
	public const string THQ_DOCUMENT_NAME = "Settings";

	// Token: 0x04001EB7 RID: 7863
	public const string THQ_DOCUMENT_ENABLE_MULTIPLAYER_KEY = "enable_multiplayer";

	// Token: 0x04001EB8 RID: 7864
	public static int data_id = -1;

	// Token: 0x04001EB9 RID: 7865
	private static MPBoss instance;

	// Token: 0x04001EBA RID: 7866
	public Game game;

	// Token: 0x04001EBB RID: 7867
	public Logic.Multiplayer meta_server;

	// Token: 0x04001EBC RID: 7868
	public MPBossMultiplayerPool pool;

	// Token: 0x04001EBD RID: 7869
	public Campaign[] multiplayerCampaigns;

	// Token: 0x04001EBE RID: 7870
	private List<IListener> listeners = new List<IListener>();

	// Token: 0x04001EBF RID: 7871
	public bool forgotPasswordRequestSent;

	// Token: 0x04001EC0 RID: 7872
	private MPBoss.SystemState systemState;

	// Token: 0x04001EC1 RID: 7873
	public Logic.Coroutine signInCoro;

	// Token: 0x04001EC2 RID: 7874
	private bool api_version_mismatch;

	// Token: 0x04001EC3 RID: 7875
	private static int multiplayer_enabled_remotely = -3;

	// Token: 0x04001EC4 RID: 7876
	private static Logic.Coroutine check_multiplayer_enabled_remotely_coro = null;

	// Token: 0x04001EC5 RID: 7877
	private static string currently_joining_lobby_id = string.Empty;

	// Token: 0x04001EC6 RID: 7878
	public Campaign currently_entered_campaign;

	// Token: 0x04001EC7 RID: 7879
	private Campaign currently_joining_campaign;

	// Token: 0x04001EC8 RID: 7880
	private string currently_joining_campaign_fail_response;

	// Token: 0x04001EC9 RID: 7881
	private string currently_cloning_save_id;

	// Token: 0x04001ECA RID: 7882
	private List<string> relevant_players = new List<string>();

	// Token: 0x04001ECB RID: 7883
	private List<string> tmp_slot_actions = new List<string>();

	// Token: 0x04001ECC RID: 7884
	public static readonly List<string> possible_slot_actions = new List<string>
	{
		"open",
		"kick",
		"invite",
		"close"
	};

	// Token: 0x04001ECD RID: 7885
	private bool currentlyReconnecting;

	// Token: 0x04001ECE RID: 7886
	private static Logic.Coroutine migrate_host_coro = null;

	// Token: 0x04001ECF RID: 7887
	private static Vars empty_vars = new Vars();

	// Token: 0x04001ED0 RID: 7888
	private Vars[] tmp_plr_vars = new Vars[6];

	// Token: 0x04001ED1 RID: 7889
	private static List<string> tmp_player_campaign_ids = new List<string>();

	// Token: 0x0200082B RID: 2091
	public enum SystemState
	{
		// Token: 0x04003E26 RID: 15910
		Default,
		// Token: 0x04003E27 RID: 15911
		ConnectingToTHQ,
		// Token: 0x04003E28 RID: 15912
		SigningIn,
		// Token: 0x04003E29 RID: 15913
		InitalizingCampaigns,
		// Token: 0x04003E2A RID: 15914
		GettingCampaignData,
		// Token: 0x04003E2B RID: 15915
		Initialized,
		// Token: 0x04003E2C RID: 15916
		SigningOut,
		// Token: 0x04003E2D RID: 15917
		SignedOut
	}
}
