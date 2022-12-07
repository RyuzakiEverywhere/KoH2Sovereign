using System;
using Crosstales.BWF;
using Crosstales.BWF.Model;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000258 RID: 600
public class UILobbyChat : MonoBehaviour, RemoteVars.IListener
{
	// Token: 0x060024F2 RID: 9458 RVA: 0x00149D1A File Offset: 0x00147F1A
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x00149D22 File Offset: 0x00147F22
	private void OnDestroy()
	{
		this.CleanUp();
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x00149D2C File Offset: 0x00147F2C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.game = GameLogic.Get(false);
		this.window_def = global::Defs.GetDefField("TitleChat", null);
		if (this.window_def != null)
		{
			UILobbyChat.KingdomNameColor = "#" + ColorUtility.ToHtmlStringRGB(global::Defs.GetColor(this.window_def, "kingdom_name_color", null));
		}
		if (this.m_ChatTextField != null)
		{
			this.m_ChatTextField.text = string.Empty;
		}
		if (this.m_InputFieldChat != null)
		{
			this.m_InputFieldChat.onSubmit.AddListener(new UnityAction<string>(this.SendChatMessage));
			this.m_InputFieldChat.onValueChanged.AddListener(new UnityAction<string>(this.HandleInputOnValueChanged));
			this.m_InputFieldChat.characterLimit = 255;
			this.m_InputFieldChat.interactable = true;
			TMP_Text component = this.m_InputFieldChat.placeholder.GetComponent<TMP_Text>();
			if (component != null)
			{
				component.text = global::Defs.Localize(this.window_def, "new_message", null, null, true, true);
			}
		}
		if (this.m_SendButton != null)
		{
			this.m_SendButton.onClick = new BSGButton.OnClick(this.HandleSendButton);
		}
		this.SetCampaignClient(this.campaign);
		this.m_Initialzied = true;
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x00149E84 File Offset: 0x00148084
	private void LocalzieDynamics()
	{
		if (this.m_InputFieldChat != null)
		{
			TMP_Text component = this.m_InputFieldChat.placeholder.GetComponent<TMP_Text>();
			if (component != null)
			{
				component.text = global::Defs.Localize(this.window_def, "new_message", null, null, true, true);
			}
		}
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x00149ED4 File Offset: 0x001480D4
	public void Refresh()
	{
		if (this.m_ChatTextField == null)
		{
			return;
		}
		Campaign campaign = this.campaign;
		string text = (((campaign != null) ? campaign.chatData : null) == null) ? "" : this.campaign.chatData.GetVar("history", null, true).String("");
		UIText.SetText(this.m_ChatTextField, text);
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x00149F3C File Offset: 0x0014813C
	private void CleanUp()
	{
		Campaign campaign = this.campaign;
		if (campaign == null)
		{
			return;
		}
		campaign.DelVarsListener(this);
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x00149F4F File Offset: 0x0014814F
	public void SetCampaignClient(Campaign newClient)
	{
		Campaign campaign = this.campaign;
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		this.campaign = newClient;
		Campaign campaign2 = this.campaign;
		if (campaign2 != null)
		{
			campaign2.AddVarsListener(this);
		}
		this.LocalzieDynamics();
		this.Refresh();
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x00149F88 File Offset: 0x00148188
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (key == "history")
		{
			this.Refresh();
			return;
		}
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x00149FA0 File Offset: 0x001481A0
	private void HandleInputOnValueChanged(string message)
	{
		int stringPosition;
		this.m_InputFieldChat.text = ChatLinksUtils.HandleInvalidLinks(message, this.m_InputFieldChat.stringPosition, out stringPosition);
		this.m_InputFieldChat.stringPosition = stringPosition;
		this.m_InputFieldChat.Rebuild(CanvasUpdate.LatePreRender);
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x00149FE3 File Offset: 0x001481E3
	private void HandleSendButton(BSGButton b)
	{
		if (string.IsNullOrEmpty(this.m_InputFieldChat.text))
		{
			return;
		}
		this.SendChatMessage(this.m_InputFieldChat.text);
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x0014A00C File Offset: 0x0014820C
	private void SendChatMessage(string message)
	{
		if (this.campaign == null)
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(message))
		{
			return;
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.SendLobbyChat(this.campaign, message);
		}
		Vars messageVars = Chat.GetMessageVars(this.campaign, THQNORequest.userId, message, Chat.Channel.Lobby, null);
		this.AddChatMessage(messageVars);
		this.m_InputFieldChat.text = string.Empty;
		this.m_InputFieldChat.Select();
		this.m_InputFieldChat.ActivateInputField();
		this.m_InputFieldChat.textComponent.RecalculateClipping();
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x0014A094 File Offset: 0x00148294
	public static void AddChatMessage(Campaign campaign, Vars message)
	{
		string text = message.Get<string>("player_name", null);
		string text2 = message.Get<string>("kingdom_name", null);
		if (!string.IsNullOrEmpty(text2))
		{
			string text3;
			if (text2 == "random")
			{
				text3 = global::Defs.Localize("TitleScreen.Multiplayer.Settings.random_kingdom", null, null, true, true);
			}
			else
			{
				text3 = global::Defs.Localize("tn_" + text2, null, null, true, true);
			}
			text2 = string.Concat(new string[]
			{
				" <color=",
				UILobbyChat.KingdomNameColor,
				">[",
				text3,
				"]</color>"
			});
		}
		string text4 = UILobbyChat.FilterProfanity(message.Get<string>("message", null)) + "\n";
		if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
		{
			text4 = string.Concat(new string[]
			{
				"<b>",
				text,
				text2,
				":</b> ",
				text4
			});
		}
		string text5 = campaign.chatData.GetVar("history", null, true).String("");
		text5 += text4;
		campaign.chatData.Set("history", text5, true);
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x0014A1C0 File Offset: 0x001483C0
	private void AddChatMessage(Vars message)
	{
		if (message == null)
		{
			return;
		}
		string text = message.Get<string>("campaign_id", null);
		if (text != null && text != this.campaign.id)
		{
			Debug.LogError("UILobbyChat.AddChatMessage called for different campaign!");
			return;
		}
		UILobbyChat.AddChatMessage(this.campaign, message);
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x0014A20C File Offset: 0x0014840C
	public static void AddInteruptCountdownMessage(Campaign campaign, string by_player_id)
	{
		Vars vars = new Vars();
		vars.Set<string>("player", "#" + PlayerInfo.GetPlayerName(by_player_id));
		vars.Set<string>("message", global::Defs.Localize("TitleScreen.Multiplayer.Countdown.interupted", vars, null, true, true));
		UILobbyChat.AddChatMessage(campaign, vars);
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x0014A25C File Offset: 0x0014845C
	private static string FilterProfanity(string message)
	{
		string result = message;
		if (UserSettings.ProfanityFilter)
		{
			result = BWFManager.ReplaceAll(message, ManagerMask.BadWord, Array.Empty<string>());
		}
		return result;
	}

	// Token: 0x0400191E RID: 6430
	[UIFieldTarget("id_InputFieldChat")]
	private TMP_InputField m_InputFieldChat;

	// Token: 0x0400191F RID: 6431
	[UIFieldTarget("id_ChatTextField")]
	private TMP_Text m_ChatTextField;

	// Token: 0x04001920 RID: 6432
	[UIFieldTarget("id_SendButton")]
	private BSGButton m_SendButton;

	// Token: 0x04001921 RID: 6433
	private Campaign campaign;

	// Token: 0x04001922 RID: 6434
	private Game game;

	// Token: 0x04001923 RID: 6435
	private static string KingdomNameColor = "#BF8738";

	// Token: 0x04001924 RID: 6436
	private DT.Field window_def;

	// Token: 0x04001925 RID: 6437
	private bool m_Initialzied;
}
