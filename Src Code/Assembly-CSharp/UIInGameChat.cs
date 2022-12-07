using System;
using System.Collections.Generic;
using Crosstales.BWF;
using Crosstales.BWF.Model;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000256 RID: 598
public class UIInGameChat : UIWindow, IListener
{
	// Token: 0x060024A3 RID: 9379 RVA: 0x0014782F File Offset: 0x00145A2F
	public override string GetDefId()
	{
		return UIInGameChat.def_id;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x00147838 File Offset: 0x00145A38
	private void Init()
	{
		if (this.m_Inited)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.in_game_chat = this;
		}
		UICommon.FindComponents(this, false);
		WorldUI worldUI = WorldUI.Get();
		GameObject gameObject = global::Common.FindChildByName((worldUI != null) ? worldUI.canvas.gameObject : null, "id_CampaignStats", true, true);
		if (gameObject != null)
		{
			this.m_UnreadMessageIcon = global::Common.FindChildByName(gameObject, "id_UnreadMessage", true, true);
		}
		this.CalcScrollbarAtBottomTreshold();
		if (this.m_MessagePrototype != null)
		{
			this.m_MessagePrototype.gameObject.SetActive(false);
		}
		if (this.m_ChannelPrototype != null)
		{
			this.m_ChannelPrototype.gameObject.SetActive(false);
		}
		if (this.m_Button_Close != null)
		{
			this.m_Button_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_ToggleChatChannels != null)
		{
			this.m_ToggleChatChannels.onClick = new BSGButton.OnClick(this.HandleOpenChatChannels);
		}
		if (this.m_SendChatMessage != null)
		{
			this.m_SendChatMessage.onClick = new BSGButton.OnClick(this.HandleSendChatMessage);
		}
		if (this.m_NewMessage != null)
		{
			this.m_NewMessage.onClick = new BSGButton.OnClick(this.HandleNewMessage);
			this.m_NewMessage.gameObject.SetActive(false);
		}
		if (this.m_ButtonExpand != null)
		{
			this.m_ButtonExpand.onClick = new BSGButton.OnClick(this.HandleOnExpand);
		}
		if (this.m_ButtonShrink != null)
		{
			this.m_ButtonShrink.onClick = new BSGButton.OnClick(this.HandleOnShrink);
		}
		if (this.m_ChatInput != null)
		{
			this.m_ChatInput.onValueChanged.AddListener(new UnityAction<string>(this.HandleInputOnValueChanged));
			this.m_ChatInput.onSubmit.AddListener(new UnityAction<string>(this.SendChatMessage));
			this.m_ChatInput.characterLimit = 255;
		}
		if (this.m_UnseenChannel != null)
		{
			this.m_UnseenChannel.gameObject.SetActive(false);
		}
		this.game = GameLogic.Get(false);
		if (this.game != null)
		{
			this.game.AddListener(this);
			if (this.game.multiplayer != null)
			{
				Logic.Multiplayer multiplayer = this.game.multiplayer;
				if (multiplayer != null)
				{
					multiplayer.AddListener(this);
				}
			}
			if (this.game.rules != null)
			{
				if (this.game.rules.team_size.is_valid && this.game.rules.team_size != 1)
				{
					this.SetChannel(Chat.Channel.Team, null);
				}
				else
				{
					this.SetChannel(Chat.Channel.All, null);
				}
			}
		}
		this.LocalizeStaticFields();
		this.RebuildChannels();
		this.UpdateLayout();
		this.ApplyChannelFilter();
		ScrollRect scrollRect = this.m_ScrollRect;
		if (scrollRect != null)
		{
			Scrollbar verticalScrollbar = scrollRect.verticalScrollbar;
			if (verticalScrollbar != null)
			{
				verticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.HandleScrollbarValueChanged));
			}
		}
		this.m_Inited = true;
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x00147B38 File Offset: 0x00145D38
	private void LocalizeStaticFields()
	{
		if (this.m_SendLabel)
		{
			UIText.SetTextKey(this.m_SendLabel, "InGameChat.send_button", null, null);
		}
		if (this.m_NewMessageLabel)
		{
			UIText.SetTextKey(this.m_NewMessageLabel, "InGameChat.new_message", null, null);
		}
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x00147B78 File Offset: 0x00145D78
	private void CalcScrollbarAtBottomTreshold()
	{
		if (this.m_MessagePrototype != null)
		{
			this.m_ScrollBarAtBottomTreshold = Mathf.RoundToInt((this.m_MessagePrototype.transform as RectTransform).rect.height * this.m_ScrollBarAtBottomPaddingCoeficient);
		}
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x00147BC2 File Offset: 0x00145DC2
	private void HandleScrollbarValueChanged(float value)
	{
		if (Mathf.Approximately(value, 0f) || value < 0f)
		{
			this.EnableNewMessageButton(false);
		}
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x00147BE0 File Offset: 0x00145DE0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		ScrollRect scrollRect = this.m_ScrollRect;
		if (scrollRect != null)
		{
			Scrollbar verticalScrollbar = scrollRect.verticalScrollbar;
			if (verticalScrollbar != null)
			{
				verticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.HandleScrollbarValueChanged));
			}
		}
		TMP_InputField chatInput = this.m_ChatInput;
		if (chatInput != null)
		{
			TMP_InputField.OnChangeEvent onValueChanged = chatInput.onValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged.RemoveListener(new UnityAction<string>(this.HandleInputOnValueChanged));
			}
		}
		TMP_InputField chatInput2 = this.m_ChatInput;
		if (chatInput2 != null)
		{
			TMP_InputField.SubmitEvent onSubmit = chatInput2.onSubmit;
			if (onSubmit != null)
			{
				onSubmit.RemoveListener(new UnityAction<string>(this.SendChatMessage));
			}
		}
		this.game = GameLogic.Get(false);
		if (this.game != null)
		{
			this.game.DelListener(this);
			if (this.game.multiplayer != null)
			{
				this.game.multiplayer.DelListener(this);
			}
		}
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x00147CB0 File Offset: 0x00145EB0
	protected override void Update()
	{
		base.Update();
		if (this.m_InvalidateScroll)
		{
			this.CheckForNewMessageButton();
			this.m_InvalidateScroll = false;
		}
		if (this.m_InvalidateChannels)
		{
			this.RebuildChannels();
			this.m_InvalidateChannels = false;
		}
		if (this.m_chatIsOpen && this.m_ChatInput.isFocused && this.m_ChatInput.stringPosition != this.m_StringPosition)
		{
			this.m_StringPosition = this.m_ChatInput.stringPosition;
			this.HandleCaretPositionChanged();
		}
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x00147D2C File Offset: 0x00145F2C
	public void SetChannel(Chat.Channel newChannel, string whisperTargetId = null)
	{
		this.CurrentActiveChannel = newChannel;
		this.m_ChannelFilter = newChannel.ToString();
		switch (newChannel)
		{
		case Chat.Channel.All:
			this.m_ChatChannelsIcon.overrideSprite = global::Defs.GetObj<Sprite>("InGameChat", "channels.All.icon", null);
			this.UpdateInputPlaceholderText(global::Defs.Localize("InGameChat.channels.All.input_text", null, null, true, true));
			if (this.m_GlobalChannel != null)
			{
				this.m_GlobalChannel.useend = false;
			}
			break;
		case Chat.Channel.Team:
			this.m_ChatChannelsIcon.overrideSprite = global::Defs.GetObj<Sprite>("InGameChat", "channels.Team.icon", null);
			this.UpdateInputPlaceholderText(global::Defs.Localize("InGameChat.channels.Team.input_text", null, null, true, true));
			if (this.m_TeamChannel != null)
			{
				this.m_TeamChannel.useend = false;
			}
			break;
		case Chat.Channel.Whisper:
			this.m_ChatChannelsIcon.overrideSprite = global::Defs.GetObj<Sprite>("InGameChat", "channels.Whisper.icon", null);
			if (string.IsNullOrEmpty(whisperTargetId))
			{
				Debug.LogError("Invalid whisper target id for whisper channel!");
			}
			this.whisperTargetId = whisperTargetId;
			this.m_ChannelFilter += whisperTargetId;
			if (this.game != null && this.game.campaign != null && this.game.campaign.playerDataPersistent != null)
			{
				string playerName = this.game.campaign.GetPlayerName(whisperTargetId);
				Vars vars = new Vars();
				vars.Set<string>("player_name", "#" + playerName);
				this.UpdateInputPlaceholderText(global::Defs.Localize("InGameChat.channels.Whisper.input_text", vars, null, true, true));
			}
			for (int i = 0; i < this.m_WhisperChannels.Count; i++)
			{
				if (this.m_WhisperChannels[i].whisperTargetId == whisperTargetId)
				{
					this.m_WhisperChannels[i].useend = false;
					break;
				}
			}
			break;
		case Chat.Channel.Lobby:
			this.m_ChatChannelsIcon.overrideSprite = global::Defs.GetObj<Sprite>("InGameChat", "channels.Lobby.icon", null);
			this.UpdateInputPlaceholderText(global::Defs.Localize("InGameChat.channels.Lobby.input_text", null, null, true, true));
			if (this.m_GlobalChannel != null)
			{
				this.m_GlobalChannel.useend = false;
			}
			break;
		}
		if (this.m_ChatChannelsWindow != null)
		{
			this.m_ChatChannelsWindow.gameObject.SetActive(false);
		}
		bool interactable = true;
		if (newChannel == Chat.Channel.Whisper)
		{
			int playerIndex = this.game.campaign.GetPlayerIndex(whisperTargetId, true);
			interactable = (this.game.campaign.GetPlayerRuntimeState(playerIndex) == Campaign.RuntimeState.Playing);
		}
		this.m_ChatInput.interactable = interactable;
		this.UpdateChannels();
		this.ApplyChannelFilter();
		this.CheckForUnreadMessageIcon();
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x00147FC2 File Offset: 0x001461C2
	private void UpdateInputPlaceholderText(string text)
	{
		if (this.m_InputPlaceholder != null)
		{
			this.m_InputPlaceholder.text = text;
		}
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x00147FE0 File Offset: 0x001461E0
	private void RebuildChannels()
	{
		if (this.game == null || this.game.campaign == null || this.game.campaign.playerDataPersistent == null)
		{
			return;
		}
		if (this.game.multiplayer == null || this.game.multiplayer.playerData == null)
		{
			return;
		}
		if (this.m_TeamChannel != null)
		{
			bool active = this.game.rules.team_size.is_valid && this.game.rules.team_size != 1;
			this.m_TeamChannel.gameObject.SetActive(active);
		}
		List<Logic.Multiplayer.PlayerData> all = Logic.Multiplayer.CurrentPlayers.GetAll();
		for (int i = 0; i < all.Count; i++)
		{
			Logic.Multiplayer.PlayerData playerData = all[i];
			if (!(playerData.id == this.game.campaign.GetLocalPlayerID()) && this.m_WhisperChannels.FindIndex((ChatChannel x) => x.whisperTargetId == playerData.id) == -1)
			{
				GameObject gameObject = global::Common.Spawn(this.m_ChannelPrototype, this.m_ChannelPrototype.transform.parent, false, "");
				gameObject.gameObject.SetActive(true);
				gameObject.name = "id_Channel";
				TMP_Text tmp_Text = global::Common.FindChildComponent<TMP_Text>(gameObject, "id_Text");
				if (tmp_Text != null)
				{
					int playerIndex = this.game.campaign.GetPlayerIndex(playerData.id, true);
					if (this.game.campaign.GetPlayerRuntimeState(playerIndex) == Campaign.RuntimeState.Playing)
					{
						tmp_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player");
					}
					else
					{
						tmp_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player_offline");
					}
					Vars vars = new Vars();
					vars.Set<string>("player_name", "#" + playerData.name);
					UIText.SetText(tmp_Text, global::Defs.Localize("InGameChat.channels.Whisper.description", vars, null, true, true));
				}
				ChatChannel component = global::Common.GetComponent<ChatChannel>(gameObject, null);
				component.whisperTargetId = playerData.id;
				this.m_WhisperChannels.Add(component);
				component.gameObject.name = component.gameObject.name + "_" + playerData.id;
				gameObject.SetActive(true);
			}
		}
		for (int j = 0; j < this.m_WhisperChannels.Count; j++)
		{
			this.m_WhisperChannels[j].Rebuild();
		}
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x0014827C File Offset: 0x0014647C
	private void UpdateChannels()
	{
		if (this.m_TeamChannel != null)
		{
			this.m_TeamChannel.UpdateState();
		}
		if (this.m_GlobalChannel != null)
		{
			this.m_GlobalChannel.UpdateState();
		}
		for (int i = 0; i < this.m_WhisperChannels.Count; i++)
		{
			this.m_WhisperChannels[i].UpdateState();
		}
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x001482E4 File Offset: 0x001464E4
	private void SendChatMessage(string message)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			return;
		}
		if (this.m_ChatInput.wasCanceled)
		{
			this.m_ChatInput.text = string.Empty;
			this.m_ChatInput.Select();
			this.m_ChatInput.ActivateInputField();
			return;
		}
		if (this.game == null || this.game.multiplayer == null || this.game.multiplayer.chat == null)
		{
			return;
		}
		message = this.ParseChannelCommand(message);
		if (string.IsNullOrWhiteSpace(message))
		{
			this.m_ChatInput.text = string.Empty;
			this.m_ChatInput.Select();
			this.m_ChatInput.ActivateInputField();
			return;
		}
		string message2 = ChatLinksUtils.ConvertLinksToMarkup(this.game, message);
		if (this.CurrentActiveChannel == Chat.Channel.Lobby)
		{
			MPBoss mpboss = MPBoss.Get();
			if (mpboss != null)
			{
				mpboss.SendLobbyChat(this.game.campaign, message);
			}
		}
		else
		{
			this.game.multiplayer.chat.SendInGameChatMessage(this.CurrentActiveChannel, message2, this.whisperTargetId);
		}
		string whisperTargetName = null;
		Vars messageVars;
		if (this.CurrentActiveChannel == Chat.Channel.Whisper)
		{
			if (this.game.campaign != null && this.game.campaign.playerDataPersistent != null)
			{
				whisperTargetName = this.game.campaign.GetPlayerName(this.whisperTargetId);
			}
			messageVars = Chat.GetMessageVars(this.game.campaign, this.whisperTargetId, message2, this.CurrentActiveChannel, whisperTargetName);
		}
		else
		{
			messageVars = Chat.GetMessageVars(this.game.campaign, this.game.multiplayer.playerData.id, message2, this.CurrentActiveChannel, whisperTargetName);
		}
		this.AddChatMessage(messageVars, false);
		this.ScrollToBottom();
		this.m_ChatInput.text = string.Empty;
		this.m_ChatInput.Select();
		this.m_ChatInput.ActivateInputField();
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x001484AC File Offset: 0x001466AC
	private string ParseChannelCommand(string message)
	{
		string result = message;
		int num = message.IndexOf(' ');
		if (num > 0)
		{
			string text = message.Substring(0, num);
			if (text[0] == '/' && text[1] != '/')
			{
				result = message.Substring(num + 1);
				text = text.Remove(0, 1);
				if (!this.SetChannel(text))
				{
					result = null;
				}
			}
		}
		else if (message[0] == '/' && message[1] != '/')
		{
			result = null;
			message = message.Remove(0, 1);
			this.SetChannel(message);
		}
		return result;
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x00148534 File Offset: 0x00146734
	private bool SetChannel(string channel)
	{
		if (channel == "all")
		{
			this.SetChannel(Chat.Channel.All, null);
			return true;
		}
		if (channel == "lobby")
		{
			this.SetChannel(Chat.Channel.Lobby, null);
			return true;
		}
		if (!(channel == "team"))
		{
			this.AddSystemMessage(global::Defs.Localize("InGameChat.invalid_command", null, null, true, true));
			return false;
		}
		if (this.game == null || this.game.rules == null)
		{
			return false;
		}
		if (this.game.rules.team_size.is_valid && this.game.rules.team_size != 1)
		{
			this.SetChannel(Chat.Channel.Team, null);
			return true;
		}
		this.AddSystemMessage(global::Defs.Localize("InGameChat.non_team_based_campaign", null, null, true, true));
		return false;
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x00148600 File Offset: 0x00146800
	private void HandleInputOnValueChanged(string message)
	{
		int stringPosition;
		this.m_ChatInput.text = ChatLinksUtils.HandleInvalidLinks(message, this.m_ChatInput.stringPosition, out stringPosition);
		this.m_ChatInput.stringPosition = stringPosition;
		this.m_ChatInput.Rebuild(CanvasUpdate.LatePreRender);
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x00148643 File Offset: 0x00146843
	private void HandleCaretPositionChanged()
	{
		this.m_ChatInput.stringPosition = ChatLinksUtils.HandleCaretPosition(this.m_ChatInput.text, this.m_ChatInput.stringPosition);
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x0014866C File Offset: 0x0014686C
	public void AddInputChatLink(Logic.Object logicObject)
	{
		if (!this.ValidateLinkObj(logicObject))
		{
			return;
		}
		this.EnableChatAndFocusInput();
		string text = global::Defs.LocalizedObjName(logicObject, null, "", true);
		text = UIText.StripNestedLinks(text);
		string colorOpeningTag = global::Defs.Localize("clr", null, "link_bright", false, true);
		text = ChatLinksUtils.ColorizeLinks(text, colorOpeningTag);
		this.AddInputChatMessage(text, this.m_ChatInput.stringPosition);
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x001486CC File Offset: 0x001468CC
	public static string ConvertMarkupToLinks(Game game, string message)
	{
		if (game == null)
		{
			return null;
		}
		int i = 0;
		while (i > -1)
		{
			i = message.IndexOf(ChatLinksUtils.MarkupOpeningTag);
			if (i == -1)
			{
				break;
			}
			int num = message.IndexOf(ChatLinksUtils.MarkupClosingTag);
			int length = num - (i + ChatLinksUtils.MarkupOpeningTag.Length);
			try
			{
				string objectInfo = message.Substring(i + ChatLinksUtils.MarkupOpeningTag.Length, length);
				Logic.Object objectFromMarkup = ChatLinksUtils.GetObjectFromMarkup(game, objectInfo);
				if (objectFromMarkup == null)
				{
					break;
				}
				message = string.Concat(new string[]
				{
					message.Substring(0, i),
					ChatLinksUtils.NoParseClosingTag,
					global::Defs.LocalizedObjName(objectFromMarkup, null, "", true),
					ChatLinksUtils.NoParseOpeningTag,
					message.Substring(num + ChatLinksUtils.MarkupClosingTag.Length)
				});
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}
		message = ChatLinksUtils.NoParseOpeningTag + message + ChatLinksUtils.NoParseClosingTag;
		return message;
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x001487BC File Offset: 0x001469BC
	private bool ValidateLinkObj(Logic.Object logicObject)
	{
		if (logicObject == null || logicObject.rtti == null || logicObject.rtti.ti == null)
		{
			return false;
		}
		switch (logicObject.rtti.ti.tid)
		{
		case Serialization.ObjectType.Kingdom:
		case Serialization.ObjectType.Castle:
		case Serialization.ObjectType.Village:
		case Serialization.ObjectType.Army:
		case Serialization.ObjectType.Character:
		case Serialization.ObjectType.Mercenary:
		case Serialization.ObjectType.Rebel:
			return true;
		}
		return false;
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x00148828 File Offset: 0x00146A28
	private void AddInputChatMessage(string message, int position = -1)
	{
		if (this.m_ChatInput == null)
		{
			return;
		}
		string text = this.m_ChatInput.text;
		if (position > -1 && position < text.Length)
		{
			try
			{
				this.m_ChatInput.text = text.Insert(position, message ?? "");
				return;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
				return;
			}
		}
		TMP_InputField chatInput = this.m_ChatInput;
		chatInput.text += message;
		this.m_ChatInput.MoveToEndOfLine(false, false);
		this.m_ChatInput.MoveToEndOfLine(false, false);
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x001488C8 File Offset: 0x00146AC8
	private void AddChatMessage(Vars messageVars, bool receivedMessage = true)
	{
		string text = this.FormatMessage(messageVars, receivedMessage);
		if (text == null)
		{
			return;
		}
		Chat.Channel channel = messageVars.Get<Chat.Channel>("channel", Chat.Channel.NONE);
		if (channel == Chat.Channel.Lobby)
		{
			channel = Chat.Channel.All;
		}
		string text2 = channel.ToString();
		string text3 = messageVars.Get<string>("player_id", null);
		if (channel == Chat.Channel.Whisper)
		{
			text2 += text3;
		}
		if (receivedMessage)
		{
			this.m_InvalidateScroll = true;
		}
		this.m_ReceivedMessageWhileClosed = !base.gameObject.activeInHierarchy;
		this.InstantiateMessagePrototype(text, text2, !receivedMessage);
		this.UpdateChannelsUnseenState(channel, text3);
		this.CheckForUnreadMessageIcon();
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x00148954 File Offset: 0x00146B54
	private void UpdateChannelsUnseenState(Chat.Channel messageChannel, string player_id)
	{
		if (this.CurrentActiveChannel == Chat.Channel.All)
		{
			this.m_GlobalChannel.useend = false;
			this.m_GlobalChannel.useend = false;
			for (int i = 0; i < this.m_WhisperChannels.Count; i++)
			{
				ChatChannel chatChannel = this.m_WhisperChannels[i];
				if (chatChannel.whisperTargetId == player_id && this.m_ChannelFilter != chatChannel.channel.ToString() + player_id)
				{
					this.m_WhisperChannels[i].useend = false;
					return;
				}
			}
			return;
		}
		if (messageChannel == this.m_GlobalChannel.channel && this.CurrentActiveChannel != messageChannel)
		{
			this.m_GlobalChannel.useend = true;
		}
		else if (messageChannel == this.m_TeamChannel.channel && this.CurrentActiveChannel != messageChannel)
		{
			this.m_TeamChannel.useend = true;
		}
		else if (messageChannel == Chat.Channel.Whisper)
		{
			for (int j = 0; j < this.m_WhisperChannels.Count; j++)
			{
				ChatChannel chatChannel2 = this.m_WhisperChannels[j];
				if (chatChannel2.whisperTargetId == player_id && this.m_ChannelFilter != chatChannel2.channel.ToString() + player_id)
				{
					this.m_WhisperChannels[j].useend = true;
					this.m_WhisperChannels[j].UpdateState();
					break;
				}
			}
		}
		this.UpdateChannels();
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x00148AC0 File Offset: 0x00146CC0
	private string FormatMessage(Vars message, bool receivedMessage)
	{
		string result = null;
		string text = message.Get<string>("player_name", null);
		int playerIndex = this.game.campaign.GetPlayerIndex(message.Get<string>("player_id", null), true);
		Chat.Channel channel = message.Get<Chat.Channel>("channel", Chat.Channel.NONE);
		string message2 = message.Get<string>("message", null);
		if (this.game != null && this.game.multiplayer != null && this.game.multiplayer.chat != null)
		{
			message2 = this.FilterProfanity(message2);
		}
		string text2 = UIInGameChat.ConvertMarkupToLinks(this.game ?? GameLogic.Get(false), message2);
		if (text2 == null)
		{
			return null;
		}
		string text3 = ColorUtility.ToHtmlStringRGBA(global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player"));
		if (channel == Chat.Channel.Team)
		{
			string text4 = ColorUtility.ToHtmlStringRGBA(global::Defs.GetColor("InGameChat", "channels.Team.color"));
			result = string.Format("<color=#{0}>{1}</color> <color=#{2}>[{3}]</color>: {4}\n", new object[]
			{
				text3,
				text,
				text4,
				channel,
				text2
			});
		}
		else if (channel == Chat.Channel.All)
		{
			string text5 = ColorUtility.ToHtmlStringRGBA(global::Defs.GetColor("InGameChat", "channels.All.color"));
			result = string.Format("<color=#{0}>{1}</color> <color=#{2}>[{3}]</color>: {4}\n", new object[]
			{
				text3,
				text,
				text5,
				channel,
				text2
			});
		}
		else if (channel == Chat.Channel.Lobby)
		{
			string text6 = ColorUtility.ToHtmlStringRGBA(global::Defs.GetColor("InGameChat", "channels.Lobby.color"));
			result = string.Format("<color=#{0}>{1}</color> <color=#{2}>[{3}]</color>: {4}\n", new object[]
			{
				text3,
				text,
				text6,
				channel,
				text2
			});
		}
		else if (channel == Chat.Channel.Whisper)
		{
			text3 = ColorUtility.ToHtmlStringRGBA(global::Defs.GetColor("InGameChat", "channels.Whisper.whisper_color"));
			if (receivedMessage)
			{
				result = string.Concat(new string[]
				{
					"<color=#",
					text3,
					">From ",
					text,
					"</color>: ",
					text2,
					"\n"
				});
			}
			else
			{
				string text7 = message.Get<string>("whisper_target_name", null);
				result = string.Concat(new string[]
				{
					"<color=#",
					text3,
					">То ",
					text7,
					"</color>: ",
					text2,
					"\n"
				});
			}
		}
		return result;
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x00148D08 File Offset: 0x00146F08
	private string FilterProfanity(string message)
	{
		string result = message;
		if (UserSettings.ProfanityFilter)
		{
			result = BWFManager.ReplaceAll(message, ManagerMask.BadWord, Array.Empty<string>());
		}
		return result;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x00148D2C File Offset: 0x00146F2C
	private void AddSystemMessage(string message)
	{
		Color color = global::Defs.GetColor("InGameChat", "system_message_color");
		this.InstantiateMessagePrototype(string.Concat(new string[]
		{
			"<b><color=#",
			ColorUtility.ToHtmlStringRGB(color),
			">",
			message,
			"</color><b>"
		}), "System", false);
		this.m_ReceivedMessageWhileClosed = !base.gameObject.activeInHierarchy;
		this.CheckForUnreadMessageIcon();
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x00148DA0 File Offset: 0x00146FA0
	private void InstantiateMessagePrototype(string message, string channelTag, bool own)
	{
		if (this.m_MessagePrototype == null)
		{
			return;
		}
		if (this.m_Content == null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(this.m_MessagePrototype, this.m_Content, false, "");
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_Text");
		if (own)
		{
			textMeshProUGUI.alignment = TextAlignmentOptions.TopRight;
		}
		else
		{
			textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
		}
		UIText.SetText(textMeshProUGUI, message);
		gameObject.SetActive(this.IsShownForCurrentChannel(channelTag));
		if (!this.m_Messages.ContainsKey(channelTag))
		{
			this.m_Messages.Add(channelTag, new List<GameObject>());
		}
		this.m_Messages[channelTag].Add(gameObject);
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x00148E4F File Offset: 0x0014704F
	private bool IsShownForCurrentChannel(string tag)
	{
		return this.m_ChannelFilter == "All" || tag == "System" || tag == this.m_ChannelFilter;
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x00148E88 File Offset: 0x00147088
	private void ApplyChannelFilter()
	{
		foreach (KeyValuePair<string, List<GameObject>> keyValuePair in this.m_Messages)
		{
			bool active = this.IsShownForCurrentChannel(keyValuePair.Key);
			int i = 0;
			int count = keyValuePair.Value.Count;
			while (i < count)
			{
				keyValuePair.Value[i].SetActive(active);
				i++;
			}
		}
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x00148F10 File Offset: 0x00147110
	public bool HasUnseenMessages()
	{
		if (this.m_ReceivedMessageWhileClosed)
		{
			return true;
		}
		if (this.m_GlobalChannel != null && this.m_GlobalChannel.useend)
		{
			return true;
		}
		if (this.m_TeamChannel != null && this.m_TeamChannel.useend)
		{
			return true;
		}
		for (int i = 0; i < this.m_WhisperChannels.Count; i++)
		{
			if (this.m_WhisperChannels[i].useend)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x00148F90 File Offset: 0x00147190
	private void CheckForUnreadMessageIcon()
	{
		bool active = this.HasUnseenMessages();
		if (this.m_UnseenChannel != null)
		{
			this.m_UnseenChannel.gameObject.SetActive(active);
		}
		if (this.m_UnreadMessageIcon != null)
		{
			this.m_UnreadMessageIcon.SetActive(active);
		}
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x00148FDD File Offset: 0x001471DD
	private void CheckForNewMessageButton()
	{
		if (this.m_chatIsOpen && !this.IsScrolledToBottom())
		{
			this.EnableNewMessageButton(true);
		}
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x00148FF8 File Offset: 0x001471F8
	private bool IsScrolledToBottom()
	{
		return this.m_ScrollRect != null && this.m_ScrollRect.verticalScrollbar != null && (!this.m_ScrollRect.verticalScrollbar.isActiveAndEnabled || (this.m_Viewport == null || this.m_Content == null) || (this.m_Viewport as RectTransform).rect.height - Mathf.Abs(this.m_Content.transform.localPosition.y) > (float)(-(float)this.m_ScrollBarAtBottomTreshold));
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x0014909A File Offset: 0x0014729A
	private void ScrollToBottom()
	{
		if (this.m_ScrollRect != null)
		{
			this.m_ScrollRect.verticalNormalizedPosition = 0f;
		}
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x001490BA File Offset: 0x001472BA
	private void HandleSendChatMessage(BSGButton button)
	{
		if (this.m_ChatInput != null)
		{
			this.SendChatMessage(this.m_ChatInput.text);
		}
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x001490DB File Offset: 0x001472DB
	public void EnableChatAndFocusInput()
	{
		if (!this.m_chatIsOpen)
		{
			base.gameObject.SetActive(true);
		}
		this.m_ChatInput.Select();
		this.m_ChatInput.ActivateInputField();
		this.m_chatIsOpen = true;
		this.CheckForUnreadMessageIcon();
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton button)
	{
		this.Close(false);
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x00149114 File Offset: 0x00147314
	public override void Show()
	{
		base.Show();
		this.Init();
		this.m_ReceivedMessageWhileClosed = false;
		base.gameObject.SetActive(true);
		this.m_chatIsOpen = true;
		this.SetChannel(this.CurrentActiveChannel, this.whisperTargetId);
		this.CheckForUnreadMessageIcon();
		this.m_ChatInput.Select();
		this.m_ChatInput.ActivateInputField();
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x00149175 File Offset: 0x00147375
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
		this.m_chatIsOpen = false;
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x001491A5 File Offset: 0x001473A5
	private void HandleOpenChatChannels(BSGButton button)
	{
		if (this.m_ChatChannelsWindow != null)
		{
			this.m_ChatChannelsWindow.gameObject.SetActive(!this.m_ChatChannelsWindow.gameObject.activeSelf);
		}
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x001491D8 File Offset: 0x001473D8
	private void HandleNewMessage(BSGButton button)
	{
		this.EnableNewMessageButton(false);
		this.ScrollToBottom();
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x001491E7 File Offset: 0x001473E7
	private void HandleOnShrink(BSGButton btn)
	{
		this.m_isShrinked = !this.m_isShrinked;
		this.UpdateLayout();
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x001491E7 File Offset: 0x001473E7
	private void HandleOnExpand(BSGButton btn)
	{
		this.m_isShrinked = !this.m_isShrinked;
		this.UpdateLayout();
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x00149200 File Offset: 0x00147400
	private void UpdateLayout()
	{
		if (this.m_ScrollRect != null)
		{
			this.m_ScrollRect.enabled = !this.m_isShrinked;
			ScrollRect scrollRect = this.m_ScrollRect;
			LayoutElement layoutElement = (scrollRect != null) ? scrollRect.GetComponent<LayoutElement>() : null;
			if (layoutElement != null)
			{
				layoutElement.preferredHeight = (this.m_isShrinked ? global::Defs.GetFloat("InGameChat", "shrink_size", null, 0f) : global::Defs.GetFloat("InGameChat", "full_size", null, 0f));
			}
			Scrollbar verticalScrollbar = this.m_ScrollRect.verticalScrollbar;
			if (verticalScrollbar != null)
			{
				verticalScrollbar.gameObject.SetActive(!this.m_isShrinked);
				if (this.m_isShrinked)
				{
					verticalScrollbar.value = 0f;
				}
			}
		}
		if (this.m_ButtonShrink != null)
		{
			this.m_ButtonShrink.gameObject.SetActive(!this.m_isShrinked);
		}
		if (this.m_ButtonExpand != null)
		{
			this.m_ButtonExpand.gameObject.SetActive(this.m_isShrinked);
		}
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x0014930F File Offset: 0x0014750F
	public void EnableNewMessageButton(bool enabled)
	{
		if (this.m_NewMessage != null)
		{
			this.m_NewMessage.gameObject.SetActive(enabled);
		}
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x00149330 File Offset: 0x00147530
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "map_loaded"))
		{
			if (!(message == "chat_message_received"))
			{
				if (message == "players_changed")
				{
					this.m_InvalidateChannels = true;
					return;
				}
				if (!(message == "chat_add_system_message"))
				{
					return;
				}
				string text_key = param as string;
				DT.Field soundsDef = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("system_chat_sound", null, "", true, true, true, '.') : null, null);
				this.AddSystemMessage(global::Defs.Localize(text_key, null, null, true, true));
				this.SetChannel(Chat.Channel.All, null);
			}
			else
			{
				DT.Field soundsDef2 = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("ingame_chat_sound", null, "", true, true, true, '.') : null, null);
				Vars vars = (Vars)param;
				if (vars.GetVar("player_id", null, true) == this.game.multiplayer.playerData.id)
				{
					return;
				}
				this.AddChatMessage(vars, true);
				return;
			}
		}
		else if (this.game != null && this.game.multiplayer != null)
		{
			this.game.multiplayer.AddListener(this);
			return;
		}
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x00149459 File Offset: 0x00147659
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIInGameChat.def_id, null);
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x060024D1 RID: 9425 RVA: 0x00149466 File Offset: 0x00147666
	// (set) Token: 0x060024D2 RID: 9426 RVA: 0x0014946D File Offset: 0x0014766D
	public static UIInGameChat current { get; private set; }

	// Token: 0x060024D3 RID: 9427 RVA: 0x00149478 File Offset: 0x00147678
	public static void ToggleOpen()
	{
		if (UIInGameChat.current != null)
		{
			if (UIInGameChat.current.IsShown())
			{
				UIInGameChat.current.Close(false);
				return;
			}
			UIInGameChat.current.Show();
			return;
		}
		else
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			GameObject prefab = UIInGameChat.GetPrefab();
			if (prefab == null)
			{
				return;
			}
			GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
			if (gameObject != null)
			{
				UICommon.DeleteChildren(gameObject.transform, typeof(UIInGameChat));
				UIInGameChat.current = UIInGameChat.Create(prefab, gameObject.transform as RectTransform);
				UIInGameChat.current.Show();
			}
			return;
		}
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x00149527 File Offset: 0x00147727
	public static bool IsActive()
	{
		return UIInGameChat.current != null;
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x00149534 File Offset: 0x00147734
	public static bool IsActiveAndShow()
	{
		return UIInGameChat.current != null && UIInGameChat.current.gameObject.activeInHierarchy;
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x00149554 File Offset: 0x00147754
	public static UIInGameChat Create(GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIInGameChat orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, parent).GetOrAddComponent<UIInGameChat>();
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x040018E1 RID: 6369
	private static string def_id = "InGameChat";

	// Token: 0x040018E2 RID: 6370
	[HideInInspector]
	public Chat.Channel CurrentActiveChannel = Chat.Channel.All;

	// Token: 0x040018E3 RID: 6371
	[HideInInspector]
	public string whisperTargetId;

	// Token: 0x040018E4 RID: 6372
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_Button_Close;

	// Token: 0x040018E5 RID: 6373
	[UIFieldTarget("btn_Channel")]
	private BSGButton m_ToggleChatChannels;

	// Token: 0x040018E6 RID: 6374
	[UIFieldTarget("id_UnseenChannel")]
	private GameObject m_UnseenChannel;

	// Token: 0x040018E7 RID: 6375
	[UIFieldTarget("btn_Channel")]
	private Image m_ChatChannelsIcon;

	// Token: 0x040018E8 RID: 6376
	[UIFieldTarget("id_ChatChannels")]
	private GameObject m_ChatChannelsWindow;

	// Token: 0x040018E9 RID: 6377
	[UIFieldTarget("btn_Send")]
	private BSGButton m_SendChatMessage;

	// Token: 0x040018EA RID: 6378
	[UIFieldTarget("id_SendLabel")]
	protected TextMeshProUGUI m_SendLabel;

	// Token: 0x040018EB RID: 6379
	[UIFieldTarget("id_Input")]
	private TMP_InputField m_ChatInput;

	// Token: 0x040018EC RID: 6380
	[UIFieldTarget("id_Placeholder")]
	private TMP_Text m_InputPlaceholder;

	// Token: 0x040018ED RID: 6381
	[UIFieldTarget("id_MessagePrototype")]
	private GameObject m_MessagePrototype;

	// Token: 0x040018EE RID: 6382
	[UIFieldTarget("id_GlobalChannel")]
	private ChatChannel m_GlobalChannel;

	// Token: 0x040018EF RID: 6383
	[UIFieldTarget("id_TeamChannel")]
	private ChatChannel m_TeamChannel;

	// Token: 0x040018F0 RID: 6384
	[UIFieldTarget("id_ChannelPrototype")]
	private GameObject m_ChannelPrototype;

	// Token: 0x040018F1 RID: 6385
	[UIFieldTarget("id_ScrollView")]
	private ScrollRect m_ScrollRect;

	// Token: 0x040018F2 RID: 6386
	[UIFieldTarget("id_Viewport")]
	private Transform m_Viewport;

	// Token: 0x040018F3 RID: 6387
	[UIFieldTarget("id_Content")]
	private Transform m_Content;

	// Token: 0x040018F4 RID: 6388
	[UIFieldTarget("btn_NewMessage")]
	private BSGButton m_NewMessage;

	// Token: 0x040018F5 RID: 6389
	[UIFieldTarget("id_NewMessageLabel")]
	protected TextMeshProUGUI m_NewMessageLabel;

	// Token: 0x040018F6 RID: 6390
	[UIFieldTarget("id_ButtonShrink")]
	private BSGButton m_ButtonShrink;

	// Token: 0x040018F7 RID: 6391
	[UIFieldTarget("id_ButtonExpand")]
	private BSGButton m_ButtonExpand;

	// Token: 0x040018F8 RID: 6392
	private string m_ChannelFilter = "All";

	// Token: 0x040018F9 RID: 6393
	private Dictionary<string, List<GameObject>> m_Messages = new Dictionary<string, List<GameObject>>();

	// Token: 0x040018FA RID: 6394
	private GameObject m_UnreadMessageIcon;

	// Token: 0x040018FB RID: 6395
	private List<ChatChannel> m_WhisperChannels = new List<ChatChannel>();

	// Token: 0x040018FC RID: 6396
	private Game game;

	// Token: 0x040018FD RID: 6397
	private bool m_chatIsOpen;

	// Token: 0x040018FE RID: 6398
	private bool m_Inited;

	// Token: 0x040018FF RID: 6399
	private bool m_isShrinked;

	// Token: 0x04001900 RID: 6400
	private bool m_InvalidateScroll;

	// Token: 0x04001901 RID: 6401
	private bool m_InvalidateChannels;

	// Token: 0x04001902 RID: 6402
	private int m_StringPosition;

	// Token: 0x04001903 RID: 6403
	private float m_ScrollBarAtBottomPaddingCoeficient = 1.1f;

	// Token: 0x04001904 RID: 6404
	private int m_ScrollBarAtBottomTreshold;

	// Token: 0x04001905 RID: 6405
	private bool m_ReceivedMessageWhileClosed;
}
