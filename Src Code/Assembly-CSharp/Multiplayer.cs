using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class Multiplayer : IListener
{
	// Token: 0x060011CD RID: 4557 RVA: 0x000BB6A8 File Offset: 0x000B98A8
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Multiplayer multiplayer = logic_obj as Logic.Multiplayer;
		if (multiplayer == null)
		{
			return;
		}
		multiplayer.visuals = new global::Multiplayer
		{
			logic = multiplayer
		};
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x000BB6D4 File Offset: 0x000B98D4
	public void Init()
	{
		if (this.UI != null && this.title != null)
		{
			return;
		}
		this.UI = GameObject.Find("UI");
		if (this.UI != null)
		{
			this.title = this.UI.GetComponent<Title>();
		}
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x000BB730 File Offset: 0x000B9930
	public void OnMessage(object obj, string message, object param)
	{
		this.Init();
		if (message == "game_started")
		{
			try
			{
				Logic.Multiplayer multiplayer = this.logic;
				global::Multiplayer.<>c__DisplayClass5_0 CS$<>8__locals1;
				CS$<>8__locals1.game = ((multiplayer != null) ? multiplayer.game : null);
				if (CS$<>8__locals1.game == null)
				{
					return;
				}
				List<string> list = param as List<string>;
				if (list.Count < 3)
				{
					Debug.LogError("Invalid parameters of 'game_started'!");
					return;
				}
				string text = list[0];
				string map_name = list[1];
				string text2 = list[2];
				bool flag;
				if (!bool.TryParse(text, out flag))
				{
					Debug.LogError("Can't parse newGameStr: " + text);
					return;
				}
				bool flag2 = CS$<>8__locals1.game.campaign != null && CS$<>8__locals1.game.campaign.IsMultiplayerCampaign();
				bool flag3 = true;
				if (flag2)
				{
					string text3 = CS$<>8__locals1.game.campaign.CalcHostID();
					if (string.IsNullOrEmpty(text3))
					{
						Debug.LogError("OnMessage(game_started) with empty host id");
						return;
					}
					flag3 = (text3 == THQNORequest.userId);
				}
				RuntimeManager.StudioSystem.setParameterByName("TitleMusicState", 2f, false);
				if (flag || !flag3)
				{
					this.<OnMessage>g__LoadMap|5_0(map_name, ref CS$<>8__locals1);
				}
				else
				{
					if (string.IsNullOrEmpty(text2))
					{
						Debug.LogError("Start game saves path is empty. Starting new game");
						this.<OnMessage>g__LoadMap|5_0(map_name, ref CS$<>8__locals1);
						return;
					}
					SaveGame.Info validLoadInfo = SaveGame.GetValidLoadInfo(text2);
					if (validLoadInfo != null)
					{
						CS$<>8__locals1.game.save_name = validLoadInfo.name;
						if (CS$<>8__locals1.game.campaign == null || (!CS$<>8__locals1.game.campaign.IsMultiplayerCampaign() && validLoadInfo.id != CS$<>8__locals1.game.campaign.GetFromSaveID(false)))
						{
							CS$<>8__locals1.game.campaign = new Campaign(0);
							CS$<>8__locals1.game.campaign.Load(text2);
						}
						if (CS$<>8__locals1.game.campaign.state != Campaign.State.Started)
						{
							CS$<>8__locals1.game.campaign.SetState(Campaign.State.Started);
						}
						SaveGame.Load(text2);
					}
					else
					{
						Debug.LogError("Start game saves path " + text2 + " is not valid. Starting new game");
						this.<OnMessage>g__LoadMap|5_0(map_name, ref CS$<>8__locals1);
					}
				}
				if (this.logic == null)
				{
					Debug.LogWarning("logic is null");
				}
				string str = flag ? "New" : "Load";
				string format = " game started - {0}, {1}";
				Game game = CS$<>8__locals1.game;
				object arg;
				if (game == null)
				{
					arg = null;
				}
				else
				{
					Campaign campaign = game.campaign;
					arg = ((campaign != null) ? campaign.ToString() : null);
				}
				Debug.Log(str + string.Format(format, arg, this.logic));
				return;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				LoadingScreen loadingScreen = LoadingScreen.Get();
				if (loadingScreen != null)
				{
					loadingScreen.Show(false, true, false);
				}
				return;
			}
		}
		if (message == "show_loading_screen")
		{
			if (MPBoss.is_migrating_host)
			{
				return;
			}
			LoadingScreen loadingScreen2 = LoadingScreen.Get();
			if (loadingScreen2 == null)
			{
				return;
			}
			loadingScreen2.Show(true, (bool)param, false);
			return;
		}
		else
		{
			if (message == "show_network_error_window")
			{
				UINetworkErrorWindow.ShowNetworkMessageHeaderErrorWindow(param as string);
				return;
			}
			if (message == "show_error_window")
			{
				string[] array = param as string[];
				string a = array[0];
				string captionKey = "System.TitleError.error";
				string message1Key = "";
				string message2Key = "error code: " + array[1];
				if (a == "eliminated")
				{
					captionKey = "System.TitleError.eliminated_caption";
					message1Key = "System.TitleError.eliminated";
				}
				else if (a == "join_fail")
				{
					message1Key = "System.TitleError.join_fail";
				}
				TitleErrorWindow.Get().Show(true, captionKey, message1Key, message2Key);
				return;
			}
			if (message == "destroying" || message == "finishing")
			{
				Logic.Multiplayer multiplayer2 = this.logic;
				if (multiplayer2 != null)
				{
					multiplayer2.DelListener(this);
				}
				this.logic = null;
				return;
			}
			if (message == "client_disconnected")
			{
				Logic.Multiplayer multiplayer3 = this.logic;
				if (multiplayer3 != null && multiplayer3.connectionReason == Logic.Multiplayer.ConnectionReason.InGame)
				{
					Logic.Multiplayer multiplayer4 = this.logic;
					Game game2 = (multiplayer4 != null) ? multiplayer4.game : null;
					Vars vars = param as Vars;
					bool flag4 = false;
					string text4 = null;
					string text5 = THQNORequest.userId;
					int num = this.logic.playerData.pid;
					if (vars != null)
					{
						flag4 = vars.GetVar("isUngraceful", null, true).Bool();
						text4 = vars.GetVar("disconnectionType", null, true).String(null);
						text5 = vars.GetVar("player_id", null, true).String(null);
						num = vars.GetVar("pid", null, true).Int(0);
					}
					Debug.Log(string.Format("Client disconnected: pid: {0}, id: {1}, disconnection type: {2}, ungraceful: {3}", new object[]
					{
						num,
						text5,
						text4,
						flag4
					}));
					AutoSaveManager.SaveOnDisconnect();
					if (game2.state != Game.State.Quitting)
					{
						Action btnAction = delegate()
						{
							MPBoss mpboss2 = MPBoss.Get();
							if (mpboss2 != null)
							{
								mpboss2.CancelHostMigration();
							}
							LoadingScreen loadingScreen3 = LoadingScreen.Get();
							if (loadingScreen3 != null)
							{
								loadingScreen3.Show(false, true, false);
							}
							UIFallbackWindow.DefaultButtonBehaviour();
						};
						if (flag4)
						{
							if (this.logic != null && this.logic.type == Logic.Multiplayer.Type.Client)
							{
								string caption = "System.Network.ungracefully_disconnected";
								string body = "System.Network.reconnecting";
								if (THQNORequest.loadingScreenShown)
								{
									body = "System.Network.disconnected";
								}
								UIFallbackWindow.ShowDisconnectedWindow(caption, body, null, btnAction);
							}
						}
						else if (text4 == "host")
						{
							string caption2 = "System.Network.host_disconnected";
							string body2 = "System.Network.reconnecting";
							UIFallbackWindow.ShowDisconnectedWindow(caption2, body2, null, btnAction);
						}
						if (flag4)
						{
							game2.pause.AddRequest("DisconnectPause", num);
						}
						else
						{
							game2.pause.AddRequest("QuitPause", num);
						}
						if (this.logic != null && this.logic.type == Logic.Multiplayer.Type.Client)
						{
							MPBoss mpboss = MPBoss.Get();
							if (mpboss != null)
							{
								mpboss.MigrateHost();
							}
						}
					}
					if (vars != null)
					{
						string text6 = vars.Get("target", true);
						Value value = vars.Get("kingdom", true);
						if (text4 == "you")
						{
							Debug.Log("You disconnected!");
						}
						else if (text4 == "anotherPlayer")
						{
							Debug.Log(string.Format("Another player disconnected! - {0} as {1}", text6, value));
							MessageIcon.CreateAnotherPlayerDisconnected(vars, true);
						}
						else if (text4 == "host")
						{
							Debug.Log(string.Format("{0}: Host disconnected! - {1} as {2}", this.logic, text6, value));
						}
						Logic.Multiplayer multiplayer5 = this.logic;
						if (multiplayer5 != null && multiplayer5.type == Logic.Multiplayer.Type.Client && text4 != "anotherPlayer")
						{
							game2.campaign.SetLocalPlayerGameLoaded(false, true);
						}
						Logic.Multiplayer multiplayer6 = this.logic;
						if (multiplayer6 != null && multiplayer6.type == Logic.Multiplayer.Type.ServerClient && text4 != "anotherPlayer")
						{
							game2.campaign.SetLocalPlayerGameLoaded(false, true);
							return;
						}
					}
				}
			}
			else if (message == "full_game_state_received")
			{
				Logic.Coroutine.Start("HideLoadingScreenCoro", global::Multiplayer.<OnMessage>g__HideLoadingScreenCoro|5_2(), null);
				Logic.Multiplayer multiplayer7 = this.logic;
				Game game3 = (multiplayer7 != null) ? multiplayer7.game : null;
				if (game3 != null && game3.state != Game.State.Quitting)
				{
					if (MPBoss.is_migrating_host && game3.IsAuthority())
					{
						Pause pause = game3.pause;
						if (pause != null)
						{
							pause.AddRequest("HostMigratedPause", -2);
						}
					}
					UIFallbackWindow.HideDisconnectedWindow();
					return;
				}
			}
			else if (message == "reconnect_fail")
			{
				Action btnAction2 = delegate()
				{
					LoadingScreen loadingScreen3 = LoadingScreen.Get();
					if (loadingScreen3 != null)
					{
						loadingScreen3.Show(false, true, false);
					}
					UIFallbackWindow.DefaultButtonBehaviour();
				};
				string text7 = param as string;
				string caption3 = "System.Network.reconnect_failed";
				string body3 = text7;
				UIFallbackWindow.ShowDisconnectedWindow(caption3, body3, null, btnAction2);
			}
		}
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x000BBE9C File Offset: 0x000BA09C
	[CompilerGenerated]
	private void <OnMessage>g__LoadMap|5_0(string map_name, ref global::Multiplayer.<>c__DisplayClass5_0 A_2)
	{
		if (A_2.game != null)
		{
			A_2.game.ai.enabled = true;
			Game game = A_2.game;
			CampaignUtils.ClearMultiplayerRegistry((game != null) ? game.multiplayer : null);
			CampaignUtils.UnloadMap(A_2.game);
		}
		if (!string.IsNullOrEmpty(map_name))
		{
			Title title = this.title;
			if (title != null)
			{
				title.OnLoadScene(map_name);
			}
			GameLogic.LoadScene(map_name);
			return;
		}
		Debug.LogError("Map name is null!");
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x000BBF10 File Offset: 0x000BA110
	[CompilerGenerated]
	internal static IEnumerator <OnMessage>g__HideLoadingScreenCoro|5_2()
	{
		WorldUI worldUI;
		global::Kingdom kingdom;
		for (;;)
		{
			yield return null;
			worldUI = WorldUI.Get();
			if (!(worldUI == null))
			{
				int num = worldUI.kingdom;
				if (num != 0)
				{
					kingdom = global::Kingdom.Get(num);
					if (((kingdom != null) ? kingdom.logic : null) != null && (kingdom.logic.royalFamily != null || kingdom.logic.IsDefeated()) && (kingdom.logic.royalFamily.Sovereign != null || kingdom.logic.IsDefeated()) && CameraController.IsReady())
					{
						break;
					}
				}
			}
		}
		if (!WorldUI.cam_pos_loaded)
		{
			worldUI.LookAtCapital(true);
		}
		else
		{
			WorldUI.cam_pos_loaded = false;
		}
		LoadingScreen loadingScreen = LoadingScreen.Get();
		if (loadingScreen != null)
		{
			loadingScreen.Show(false, true, false);
		}
		if (kingdom.logic.IsDefeated())
		{
			kingdom.logic.NotifyListeners("defeat", "KingdomDefeated");
		}
		kingdom.logic.game.OnStartedAnalytics();
		if (kingdom.logic.IsAuthority())
		{
			List<Logic.Multiplayer.PlayerData> all = Logic.Multiplayer.CurrentPlayers.GetAll();
			for (int i = 0; i < all.Count; i++)
			{
				Logic.Multiplayer.PlayerData playerData = all[i];
				Logic.Kingdom kingdom2 = kingdom.logic.game.GetKingdom(playerData.kingdomId);
				if (kingdom2 != null)
				{
					kingdom2.NotifyListeners("game_started", null);
				}
			}
		}
		worldUI.CreateGameStartedMessageIcons();
		yield break;
	}

	// Token: 0x04000BFD RID: 3069
	public Logic.Multiplayer logic;

	// Token: 0x04000BFE RID: 3070
	private GameObject UI;

	// Token: 0x04000BFF RID: 3071
	private Title title;
}
