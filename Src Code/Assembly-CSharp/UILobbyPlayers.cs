using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200024D RID: 589
public class UILobbyPlayers : MonoBehaviour, RemoteVars.IListener, IListener
{
	// Token: 0x0600241A RID: 9242 RVA: 0x00144ABC File Offset: 0x00142CBC
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_PlayerPrototype != null)
		{
			this.m_PlayerPrototype.gameObject.SetActive(false);
		}
		if (this.m_AllowGameToStartWithoutMe_Toggle != null)
		{
			this.m_AllowGameToStartWithoutMe_Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleOnAllowOfflineToggle));
		}
		if (this.m_CampaignNameInputField != null)
		{
			TMP_InputField campaignNameInputField = this.m_CampaignNameInputField;
			campaignNameInputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(campaignNameInputField.onValidateInput, new TMP_InputField.OnValidateInput(this.ValidateCampaignNameInput));
			this.m_CampaignName = this.m_CampaignNameInputField.GetOrAddComponent<UILobbyPlayers.UICampaignNameField>();
		}
		if (this.m_PlayersContainer != null)
		{
			UICommon.DeleteActiveChildren(this.m_PlayersContainer.transform);
		}
		this.m_Initialized = true;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x00144B90 File Offset: 0x00142D90
	private char ValidateCampaignNameInput(string input, int charIndex, char charToValidate)
	{
		if (input.Length >= 100)
		{
			return '\0';
		}
		char[] invalidPathChars = Path.GetInvalidPathChars();
		int num = invalidPathChars.Length;
		for (int i = 0; i < num; i++)
		{
			if (invalidPathChars[i] == charToValidate)
			{
				return '\0';
			}
		}
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		num = invalidFileNameChars.Length;
		for (int j = 0; j < num; j++)
		{
			if (invalidFileNameChars[j] == charToValidate)
			{
				return '\0';
			}
		}
		return charToValidate;
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x00144BEC File Offset: 0x00142DEC
	public void SetCampaign(Campaign Client, bool fromLobby = false)
	{
		this.Init();
		Campaign campaign = this.m_Campaign;
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		MPBoss.Get().DelListener(this);
		this.m_Campaign = Client;
		Campaign campaign2 = this.m_Campaign;
		if (campaign2 != null)
		{
			campaign2.AddVarsListener(this);
		}
		MPBoss.Get().AddListener(this);
		this.m_FromLobby = fromLobby;
		this.LocalizeStatic();
		this.Populate();
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x00144C53 File Offset: 0x00142E53
	private void LocalizeStatic()
	{
		if (this.m_AllowGameToStartWithoutMe_Label != null)
		{
			UIText.SetTextKey(this.m_AllowGameToStartWithoutMe_Label, "LobbyPlayerWindow.enable_offline_start", null, null);
		}
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x00144C78 File Offset: 0x00142E78
	private void Populate()
	{
		this.UpdateLabels();
		this.UpdatePlayers();
		this.UpdateAllowOfflie();
		this.CreateTeamSizeDropDownRuleIcon();
		this.UpdateTimePlayed();
		this.UpdateMainGoalIcon();
		this.UpdateCampaignNameInteraction();
		if (this.m_ModCompatibilityIcon != null)
		{
			this.m_ModCompatibilityIcon.SetObject(this.m_Campaign, null);
		}
		if (this.m_CampaignName != null)
		{
			this.m_CampaignName.SetCampaign(this.m_Campaign);
		}
		this.m_InvalidateChat = true;
		this.m_InvalidateTeamRule = true;
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x00144CFC File Offset: 0x00142EFC
	private void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.UpdatePlayers();
			this.UpdateAllowOfflie();
			this.UpdateMainGoalIcon();
			this.UpdateCampaignNameInteraction();
			this.m_Invalidate = false;
		}
		if (this.m_InvalidateChat)
		{
			this.UpdateChat();
			this.m_InvalidateChat = false;
		}
		if (this.m_InvalidateTeamRule)
		{
			if (this.m_teamSizeRule != null)
			{
				this.m_teamSizeRule.Refresh();
			}
			this.m_InvalidateTeamRule = false;
		}
		if (this.m_InvalidateCampaignName)
		{
			if (this.m_CampaignName != null)
			{
				this.m_CampaignName.Refresh();
			}
			this.m_InvalidateCampaignName = false;
		}
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x00144D98 File Offset: 0x00142F98
	private void UpdateLabels()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_GameOwner != null)
		{
			string ownerName = this.m_Campaign.GetOwnerName();
			UIText.SetText(this.m_GameOwner, ownerName);
			this.m_GameOwner.gameObject.SetActive(!string.IsNullOrEmpty(ownerName));
		}
		if (this.m_OutcomeLabel != null)
		{
			bool flag = this.m_Campaign.state == Campaign.State.Closed;
			this.m_OutcomeLabel.gameObject.SetActive(flag);
			if (flag)
			{
				bool flag2 = false;
				bool flag3 = false;
				int localPlayerIndex = this.m_Campaign.GetLocalPlayerIndex(true);
				if (localPlayerIndex >= 0)
				{
					flag2 = this.m_Campaign.playerDataPersistent[localPlayerIndex].GetVar("victor", null, true).Bool();
					flag3 = this.m_Campaign.playerDataPersistent[localPlayerIndex].GetVar("looser", null, true).Bool();
				}
				if (flag2)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.victory_label", null, null);
				}
				else if (flag3)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.defeated_label", null, null);
				}
				else if (this.m_Campaign.state == Campaign.State.Closed)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.finshed_label", null, null);
				}
				else
				{
					this.m_OutcomeLabel.gameObject.SetActive(false);
				}
			}
		}
		if (this.m_CampaigName != null)
		{
			UIText.SetText(this.m_CampaigName, global::Defs.Localize(this.m_Campaign.GetNameKey(), this.m_Campaign, null, true, true));
		}
		if (this.m_CampaignNameInputField != null)
		{
			TextMeshProUGUI component = this.m_CampaignNameInputField.placeholder.GetComponent<TextMeshProUGUI>();
			if (component != null)
			{
				UIText.SetText(component, global::Defs.Localize("LobbyPlayerWindow.campaign_name_prompt", this.m_Campaign, null, true, true));
			}
		}
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x00144F5A File Offset: 0x0014315A
	private void UpdateAllowOfflie()
	{
		if (this.m_AllowGameToStartWithoutMe_Toggle != null)
		{
			this.m_AllowGameToStartWithoutMe_Toggle.SetIsOnWithoutNotify(CampaignUtils.GetAllowOffline(this.m_Campaign, THQNORequest.userId));
		}
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x00144F88 File Offset: 0x00143188
	private void UpdateMainGoalIcon()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		DT.Field varsDef = this.m_Campaign.GetVarsDef();
		if (varsDef == null || varsDef.children == null)
		{
			return;
		}
		DT.Field field = varsDef.FindChild("main_goal", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.m_Campaign, field);
		if (this.m_CampaignIcon != null)
		{
			this.m_CampaignIcon.overrideSprite = global::Defs.GetObj<Sprite>(selectedOption, "icon", null);
		}
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x00145000 File Offset: 0x00143200
	private void UpdateTimePlayed()
	{
		if (this.m_TimePlayed == null)
		{
			return;
		}
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_Campaign.state >= Campaign.State.Started)
		{
			int latestSaveSessionTime = this.m_Campaign.GetLatestSaveSessionTime();
			this.m_TimePlayed.gameObject.SetActive(latestSaveSessionTime >= 0);
			if (latestSaveSessionTime >= 0)
			{
				Vars vars = new Vars();
				TimeSpan timeSpan = new TimeSpan((long)latestSaveSessionTime * 10000000L);
				vars.Set<string>("time_hours", "#" + Mathf.FloorToInt((float)((int)timeSpan.TotalHours)).ToString());
				vars.Set<string>("time_minutes", "#" + timeSpan.Minutes.ToString("D2"));
				vars.Set<string>("time_seconds", "#" + timeSpan.Seconds.ToString("D2"));
				UIText.SetTextKey(this.m_TimePlayed, "LobbyPlayerWindow.play_time", vars, null);
				return;
			}
		}
		else
		{
			UIText.SetTextKey(this.m_TimePlayed, "LobbyPlayerWindow.new_campaign", null, null);
		}
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x00145120 File Offset: 0x00143320
	private void UpdatePlayers()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_Campaign.playerDataPersistent == null)
		{
			return;
		}
		if (this.m_PlayersContainer == null)
		{
			return;
		}
		if (this.m_PlayerPrototype == null)
		{
			return;
		}
		int num = this.m_PlayerSlots.Count;
		while (this.m_PlayerSlots.Count < this.m_Campaign.playerDataPersistent.Length)
		{
			UILobbyPlayers.UIPlayerTeamSlot uiplayerTeamSlot = UILobbyPlayers.UIPlayerTeamSlot.Create(this.m_Campaign, num, this.m_PlayerPrototype, this.m_PlayersContainer.transform);
			this.m_PlayerSlots.Add(uiplayerTeamSlot);
			if (uiplayerTeamSlot != null && !this.m_FromLobby)
			{
				uiplayerTeamSlot.DisableKingdomTooltip(true);
			}
			num++;
		}
		for (int i = 0; i < this.m_PlayerSlots.Count; i++)
		{
			this.m_PlayerSlots[i].SetData(this.m_Campaign, i);
		}
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x00145200 File Offset: 0x00143400
	private void UpdateCampaignNameInteraction()
	{
		if (this.m_CampaignNameInputField == null)
		{
			return;
		}
		if (this.m_Campaign == null)
		{
			this.m_CampaignNameInputField.interactable = false;
			return;
		}
		bool interactable = this.m_Campaign.GetHostID(false, true) == THQNORequest.userId || this.m_Campaign.GetOwnerID() == THQNORequest.userId;
		this.m_CampaignNameInputField.interactable = interactable;
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x0014526F File Offset: 0x0014346F
	private void UpdateChat()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		if (this.m_UILobbyChat == null)
		{
			return;
		}
		this.m_UILobbyChat.SetCampaignClient(this.m_Campaign);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x0014529A File Offset: 0x0014349A
	private void RefreshSettings()
	{
		if (this.m_teamSizeRule != null)
		{
			this.m_teamSizeRule.Refresh();
		}
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x001452B8 File Offset: 0x001434B8
	private void HandleOnAllowOfflineToggle(bool v)
	{
		int playerIndex = CampaignUtils.GetPlayerIndex(this.m_Campaign, THQNORequest.userId);
		if (playerIndex == -1)
		{
			return;
		}
		this.m_Campaign.playerDataPersistent[playerIndex].Set("allow_offline", v, true);
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x001452FC File Offset: 0x001434FC
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
			if (key == "runtime_state")
			{
				this.m_InvalidateChat = true;
				return;
			}
			if (key == "team_size" || key == "main_goal")
			{
				if (!this.m_InvalidateTeamRule)
				{
					UICampaignSettings.LocalNotifySettingChanged(this.m_teamSizeRule, old_val, new_val);
				}
				this.m_InvalidateTeamRule = true;
				return;
			}
			if (key == "start_countdown")
			{
				this.RefreshSettings();
				return;
			}
			if (!(key == "name"))
			{
				this.m_Invalidate = true;
				return;
			}
			this.m_InvalidateCampaignName = true;
			return;
		}
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x001453A4 File Offset: 0x001435A4
	public void CreateTeamSizeDropDownRuleIcon()
	{
		if (this.m_Campaign == null)
		{
			return;
		}
		DT.Field varsDef = this.m_Campaign.GetVarsDef();
		if (varsDef == null || varsDef.children == null)
		{
			return;
		}
		DT.Field field = varsDef.FindChild("team_size", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		this.m_teamSizeRule = UICampaignSettings.SettingsRule.Possess(this.m_TeamSizeRuleHost, this.m_Campaign, field, null);
		if (this.m_LockTeams != null)
		{
			this.m_LockTeams.SetActive(this.m_Campaign.state >= Campaign.State.Started);
		}
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x0014542B File Offset: 0x0014362B
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "player_name_changed")
		{
			this.m_Invalidate = true;
		}
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x00145441 File Offset: 0x00143641
	private void OnDestroy()
	{
		this.m_PlayerSlots.Clear();
		Campaign campaign = this.m_Campaign;
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.DelListener(this);
	}

	// Token: 0x04001861 RID: 6241
	[UIFieldTarget("id_CampaignIcon")]
	private Image m_CampaignIcon;

	// Token: 0x04001862 RID: 6242
	[UIFieldTarget("id_CampaigName")]
	private TextMeshProUGUI m_CampaigName;

	// Token: 0x04001863 RID: 6243
	[UIFieldTarget("id_AllowGameToStartWithoutMe_Toggle")]
	private Toggle m_AllowGameToStartWithoutMe_Toggle;

	// Token: 0x04001864 RID: 6244
	[UIFieldTarget("id_AllowGameToStartWithoutMe_Label")]
	private TextMeshProUGUI m_AllowGameToStartWithoutMe_Label;

	// Token: 0x04001865 RID: 6245
	[UIFieldTarget("id_CampaignNameInputField")]
	private TMP_InputField m_CampaignNameInputField;

	// Token: 0x04001866 RID: 6246
	[UIFieldTarget("id_TimePlayed")]
	private TextMeshProUGUI m_TimePlayed;

	// Token: 0x04001867 RID: 6247
	[UIFieldTarget("id_PlayersContainer")]
	private GameObject m_PlayersContainer;

	// Token: 0x04001868 RID: 6248
	[UIFieldTarget("id_PlayerPrototype")]
	private GameObject m_PlayerPrototype;

	// Token: 0x04001869 RID: 6249
	[UIFieldTarget("id_LobbyChat")]
	private UILobbyChat m_UILobbyChat;

	// Token: 0x0400186A RID: 6250
	[UIFieldTarget("id_TeamSizeRule")]
	private GameObject m_TeamSizeRuleHost;

	// Token: 0x0400186B RID: 6251
	[UIFieldTarget("id_GameOwner")]
	private TextMeshProUGUI m_GameOwner;

	// Token: 0x0400186C RID: 6252
	[UIFieldTarget("id_OutcomeLabel")]
	private TextMeshProUGUI m_OutcomeLabel;

	// Token: 0x0400186D RID: 6253
	[UIFieldTarget("id_LockTeams")]
	private GameObject m_LockTeams;

	// Token: 0x0400186E RID: 6254
	[UIFieldTarget("id_ModCompatibilityIcon")]
	private UIModCompatibilityIcon m_ModCompatibilityIcon;

	// Token: 0x0400186F RID: 6255
	private UICampaignSettings.SettingsRule m_teamSizeRule;

	// Token: 0x04001870 RID: 6256
	private UILobbyPlayers.UICampaignNameField m_CampaignName;

	// Token: 0x04001871 RID: 6257
	private Campaign m_Campaign;

	// Token: 0x04001872 RID: 6258
	private bool m_FromLobby;

	// Token: 0x04001873 RID: 6259
	private bool m_InvalidateChat;

	// Token: 0x04001874 RID: 6260
	private bool m_InvalidateTeamRule;

	// Token: 0x04001875 RID: 6261
	private bool m_Invalidate;

	// Token: 0x04001876 RID: 6262
	private bool m_InvalidateCampaignName;

	// Token: 0x04001877 RID: 6263
	private bool m_Initialized;

	// Token: 0x04001878 RID: 6264
	private List<UILobbyPlayers.UIPlayerTeamSlot> m_PlayerSlots = new List<UILobbyPlayers.UIPlayerTeamSlot>();

	// Token: 0x020007A8 RID: 1960
	internal class UIPlayerTeamSlot : MonoBehaviour, IVars, RemoteVars.IListener, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06004CFD RID: 19709 RVA: 0x0022BC80 File Offset: 0x00229E80
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Team != null)
			{
				this.m_Team.onClick = new BSGButton.OnClick(this.CycleTeams);
			}
			if (this.m_PlayerActionsContainer != null)
			{
				this.m_Actions = this.m_PlayerActionsContainer.GetOrAddComponent<UILobbyPlayers.UISlotActions>();
			}
			if (this.m_Actions != null)
			{
				this.m_Actions.gameObject.SetActive(false);
			}
			if (this.m_PlayerNameHitBox != null)
			{
				this.m_PlayerNameHitBox.onClick = new BSGButton.OnClick(this.HandleOnPlayerNameClick);
			}
			UIKingdomIcon playerKingdomIcon = this.m_PlayerKingdomIcon;
			if (((playerKingdomIcon != null) ? playerKingdomIcon.GetPrimary() : null) != null)
			{
				this.m_PlayerKingdomIcon.GetPrimary().onClick = new KingdomShield.OnShieldClick(this.HandleShieldClick);
			}
			this.m_Initialized = true;
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x0022BD64 File Offset: 0x00229F64
		public void SetData(Campaign campaign, int playerIndex)
		{
			this.Init();
			if (campaign == this.m_Campaign && playerIndex == this.m_PlayerIndex)
			{
				return;
			}
			this.RemoveListeners();
			this.m_Campaign = campaign;
			this.m_PlayerIndex = playerIndex;
			this.AddListeners();
			this.m_mode = this.GetMode();
			if (this.m_Actions != null)
			{
				this.m_Actions.SetIndex(playerIndex, campaign);
			}
			this.AddTooltips();
			this.Refresh();
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x0022BDD8 File Offset: 0x00229FD8
		private void AddListeners()
		{
			if (this.m_Campaign == null)
			{
				return;
			}
			this.m_Campaign.AddVarsListener(this);
			if (this.m_PlayerIndex >= 0 && this.m_Campaign.playerDataPersistent.Length > this.m_PlayerIndex)
			{
				this.m_Campaign.playerDataPersistent[this.m_PlayerIndex].AddListener(this);
			}
		}

		// Token: 0x06004D00 RID: 19712 RVA: 0x0022BE30 File Offset: 0x0022A030
		private void RemoveListeners()
		{
			if (this.m_Campaign == null)
			{
				return;
			}
			this.m_Campaign.DelVarsListener(this);
			if (this.m_PlayerIndex >= 0 && this.m_Campaign.playerDataPersistent.Length > this.m_PlayerIndex)
			{
				this.m_Campaign.playerDataPersistent[this.m_PlayerIndex].DelListener(this);
			}
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x0022BE88 File Offset: 0x0022A088
		private void LateUpdate()
		{
			if (this.m_Invalidate)
			{
				this.m_Invalidate = false;
				this.Refresh();
			}
		}

		// Token: 0x06004D02 RID: 19714 RVA: 0x0022BE9F File Offset: 0x0022A09F
		public void DisableKingdomTooltip(bool disabled)
		{
			this.m_DisableKingdomIconTooltip = disabled;
			this.m_PlayerKingdomIcon.GetPrimary().DisbaleTooltip(this.m_DisableKingdomIconTooltip);
		}

		// Token: 0x06004D03 RID: 19715 RVA: 0x0022BEC0 File Offset: 0x0022A0C0
		private UILobbyPlayers.UIPlayerTeamSlot.Mode GetMode()
		{
			int localPlayerIndex = this.m_Campaign.GetLocalPlayerIndex(true);
			if (this.m_Campaign.GetPlayerRuntimeState(localPlayerIndex) <= Campaign.RuntimeState.Offline)
			{
				return UILobbyPlayers.UIPlayerTeamSlot.Mode.CampaignInstect;
			}
			return UILobbyPlayers.UIPlayerTeamSlot.Mode.Lobby;
		}

		// Token: 0x06004D04 RID: 19716 RVA: 0x0022BEEC File Offset: 0x0022A0EC
		private void AddTooltips()
		{
			Vars vars = new Vars(this);
			if (this.m_Campaign != null)
			{
				string playerID = this.m_Campaign.GetPlayerID(this.m_PlayerIndex, true);
				string str = this.m_Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, playerID, "client_version");
				vars.Set<string>("player_game_version", "#" + str);
			}
			if (this.m_GameOwnerIcon != null)
			{
				Tooltip.Get(this.m_GameOwnerIcon.gameObject, true).SetDef("GameOwnedTooltip", vars);
			}
			if (this.m_IconAllowGameToStartWithoutMe != null)
			{
				Tooltip.Get(this.m_IconAllowGameToStartWithoutMe.gameObject, true).SetDef("AllowOfflineTooltip", vars);
			}
			if (this.m_Host != null)
			{
				Tooltip.Get(this.m_Host.gameObject, true).SetDef("GameHostTooltip", vars);
			}
			if (this.m_PlayerNameHitBox != null)
			{
				Tooltip.Get(this.m_PlayerNameHitBox.gameObject, true).SetDef("PlayerLobbyInfoTooltip", vars);
			}
			if (this.m_Team != null)
			{
				Tooltip.Get(this.m_Team.gameObject, true).SetDef("TeamIconTooltip", vars);
			}
			if (this.m_VerisonMismatch != null)
			{
				Tooltip.Get(this.m_VerisonMismatch, true).SetDef("PlayerVersionIncompatibleTooltip", vars);
			}
		}

		// Token: 0x06004D05 RID: 19717 RVA: 0x0022C040 File Offset: 0x0022A240
		private void Refresh()
		{
			if (this.m_Campaign == null)
			{
				return;
			}
			Campaign.SlotState slotState = this.m_Campaign.GetSlotState(this.m_PlayerIndex);
			string playerName = this.m_Campaign.GetPlayerName(this.m_PlayerIndex);
			bool flag = slotState == Campaign.SlotState.Empty || slotState == Campaign.SlotState.Closed;
			bool flag2 = !flag && this.m_Campaign.GetPlayerRuntimeState(this.m_PlayerIndex) > Campaign.RuntimeState.Offline;
			bool flag3 = slotState == Campaign.SlotState.Eliminated;
			string kingdomName = this.m_Campaign.GetKingdomName(this.m_PlayerIndex, false);
			Game game = GameLogic.Get(false);
			Logic.Kingdom kingdom = (game != null) ? game.GetKingdom(kingdomName) : null;
			int num = -1;
			if (kingdom == null)
			{
				num = this.m_Campaign.GetPlayerKingdomCoAIndexByKingdomName(kingdomName);
			}
			bool flag4 = this.m_Campaign.IsPlayerReady(this.m_PlayerIndex);
			string playerID = this.m_Campaign.GetPlayerID(this.m_PlayerIndex, false);
			string authorityPlayerID = this.m_Campaign.GetAuthorityPlayerID();
			bool flag5 = authorityPlayerID != null && authorityPlayerID == playerID;
			string ownerID = this.m_Campaign.GetOwnerID();
			bool active = ownerID != null && ownerID == playerID;
			bool flag6 = this.m_Campaign.IsInCampaign(this.m_PlayerIndex);
			bool flag7 = this.m_PlayerIndex == this.m_Campaign.GetLocalPlayerIndex(true);
			if (this.m_GameOwnerIcon != null)
			{
				this.m_GameOwnerIcon.gameObject.SetActive(active);
			}
			if (this.m_IconAllowGameToStartWithoutMe != null)
			{
				bool flag8 = flag6;
				flag8 &= !this.m_Campaign.playerDataPersistent[this.m_PlayerIndex].GetVar("allow_offline", null, true);
				if (flag8)
				{
					this.m_IconAllowGameToStartWithoutMe.overrideSprite = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", flag2 ? "icon_enable_offline_start_online" : "icon_enabdle_offline_start_offline", null);
				}
				this.m_IconAllowGameToStartWithoutMe.gameObject.SetActive(flag8);
			}
			if (this.m_Host != null)
			{
				this.m_Host.gameObject.SetActive(flag5);
			}
			if (this.m_PlayerName != null)
			{
				if (slotState == Campaign.SlotState.Closed)
				{
					UIText.SetTextKey(this.m_PlayerName, "LobbyPlayerWindow.closed_slot", null, null);
				}
				else if (string.IsNullOrEmpty(playerName) || (!flag6 && slotState != Campaign.SlotState.Reserved && this.m_Campaign.state == Campaign.State.Created))
				{
					UIText.SetTextKey(this.m_PlayerName, "LobbyPlayerWindow.empty_slot", null, null);
				}
				else
				{
					UIText.SetText(this.m_PlayerName, playerName);
				}
				string key;
				if (flag2 && flag3)
				{
					key = "player_name_eliminated_online";
				}
				else if (flag2 && !flag3 && flag4)
				{
					key = "player_name_normal_online_ready";
				}
				else if (flag2 && !flag3 && !flag4)
				{
					key = "player_name_normal_online_not_ready";
				}
				else if (!flag2 && flag3)
				{
					key = "player_name_eliminated_offline";
				}
				else if (flag)
				{
					key = "player_name_empty";
				}
				else
				{
					key = "player_name_normal_offline";
				}
				this.m_PlayerName.color = global::Defs.GetColor("LobbyPlayerWindow", key);
				bool flag9 = slotState == Campaign.SlotState.Left;
				this.m_PlayerName.fontStyle = (flag9 ? FontStyles.Strikethrough : FontStyles.Normal);
			}
			if (this.m_PlayerKingdomIcon != null)
			{
				this.m_PlayerKingdomIcon.GetPrimary().DisbaleTooltip(!this.m_DisableKingdomIconTooltip);
				if (kingdom != null)
				{
					this.m_PlayerKingdomIcon.SetObject(kingdom, null);
				}
				else if (num != -1)
				{
					this.m_PlayerKingdomIcon.SetIndex(num);
				}
				if (kingdomName == "random")
				{
					this.m_PlayerKingdomIcon.gameObject.SetActive(false);
					this.id_EmptyPlayerKingdomIcon.gameObject.SetActive(false);
				}
				else
				{
					bool flag10 = kingdom != null || num >= 0;
					this.m_PlayerKingdomIcon.gameObject.SetActive(flag10);
					this.id_EmptyPlayerKingdomIcon.gameObject.SetActive(!flag10);
				}
			}
			if (this.m_PlayerKingdom != null)
			{
				this.m_PlayerKingdom.gameObject.SetActive(!string.IsNullOrEmpty(kingdomName));
				if (string.IsNullOrEmpty(kingdomName))
				{
					UIText.SetText(this.m_PlayerKingdom, global::Defs.Localize("TitleScreen.Multiplayer.Settings.kingdom_not_selected", null, null, true, true));
				}
				else if (kingdomName == "random")
				{
					UIText.SetText(this.m_PlayerKingdom, global::Defs.Localize("TitleScreen.Multiplayer.Settings.random_kingdom", null, null, true, true));
				}
				else if (kingdom != null)
				{
					UIText.SetText(this.m_PlayerKingdom, global::Defs.Localize("TitleScreen.Multiplayer.player_kingdom", new Vars(kingdom), null, true, true));
				}
				else
				{
					UIText.SetText(this.m_PlayerKingdom, global::Defs.Localize("tn_" + kingdomName, null, null, true, true));
				}
				string key2;
				if (flag2 && flag3)
				{
					key2 = "kingdom_name_eliminated_online";
				}
				else if (flag2 && !flag3 && flag4)
				{
					key2 = "kingdom_name_normal_online_ready";
				}
				else if (flag2 && !flag3 && !flag4)
				{
					key2 = "kingdom_name_normal_online_not_ready";
				}
				else if (!flag2 && flag3)
				{
					key2 = "kingdom_name_eliminated_offline";
				}
				else
				{
					key2 = "kingdom_name_normal_offline";
				}
				this.m_PlayerKingdom.color = global::Defs.GetColor("LobbyPlayerWindow", key2);
				bool flag11 = slotState == Campaign.SlotState.Left;
				this.m_PlayerKingdom.fontStyle = (flag11 ? FontStyles.Strikethrough : FontStyles.Normal);
			}
			if (this.m_ReligionIcon != null)
			{
				this.m_ReligionIcon.gameObject.SetActive(kingdom != null);
				if (kingdom != null)
				{
					this.m_ReligionIcon.overrideSprite = global::Defs.GetObj<Sprite>(kingdom.religion.def.field, global::Religions.GetRelgionIconKey(kingdom), null);
				}
			}
			if (this.m_Background_Ready != null)
			{
				this.m_Background_Ready.gameObject.SetActive(flag4 && !flag5);
			}
			if (this.m_Background_NotRead != null)
			{
				this.m_Background_NotRead.gameObject.SetActive(!flag5 && !flag5);
			}
			if (this.m_Background != null)
			{
				this.m_Background.gameObject.SetActive(base.transform.GetSiblingIndex() % 2 == 1);
			}
			this.ValidateCurrentTeam();
			this.UpdateTeam();
			if (this.m_PlayerNameHitBox != null)
			{
				this.m_PlayerNameHitBox.Enable(!flag5, false);
			}
			if (this.m_VerisonMismatch != null)
			{
				Value var = this.m_Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, playerID, "client_version");
				bool flag12 = var != Value.Unknown;
				bool active2 = !flag7 && flag12 && !flag && Title.Version(true) != var;
				this.m_VerisonMismatch.gameObject.SetActive(active2);
			}
			this.UpdateHighlight();
		}

		// Token: 0x06004D06 RID: 19718 RVA: 0x0022C694 File Offset: 0x0022A894
		private void UpdateTeam()
		{
			Campaign.SlotState slotState = this.m_Campaign.GetSlotState(this.m_PlayerIndex);
			bool flag = slotState == Campaign.SlotState.Empty || slotState == Campaign.SlotState.Closed;
			bool flag2 = CampaignUtils.IsAutoTeams(this.m_Campaign);
			int team = CampaignUtils.GetTeam(this.m_Campaign, this.m_PlayerIndex);
			string text = (!flag2 && team >= 0 && !flag) ? UILobbyPlayers.UIPlayerTeamSlot.IntToRomanNumber(team + 1) : "";
			if (this.m_TeamIndex)
			{
				this.m_TeamIndex.gameObject.SetActive(!flag2);
				if (!flag2)
				{
					UIText.SetText(this.m_TeamIndex, text);
				}
			}
			if (this.m_TeamBackground != null)
			{
				string text2;
				if (flag)
				{
					text2 = "team.empty";
				}
				else if (team == -1)
				{
					text2 = "team.none";
				}
				else
				{
					text2 = "team.team_" + (team + 1);
				}
				this.m_TeamBackground.normalImage = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", text2, null);
				this.m_TeamBackground.rolloverImage = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", text2 + ".over", null);
				this.m_TeamBackground.pressedImage = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", text2 + ".selected", null);
				this.m_TeamBackground.selectedImage = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", text2 + ".selected", null);
				this.m_TeamBackground.disabledImage = global::Defs.GetObj<Sprite>("LobbyPlayerWindow", text2 + ".disabled", null);
			}
			if (this.m_Team != null)
			{
				this.m_Team.UpdateState(true);
			}
		}

		// Token: 0x06004D07 RID: 19719 RVA: 0x000023FD File Offset: 0x000005FD
		private void UpdateHighlight()
		{
		}

		// Token: 0x06004D08 RID: 19720 RVA: 0x0022C820 File Offset: 0x0022AA20
		private void ValidateCurrentTeam()
		{
			if (this.m_Campaign == null)
			{
				return;
			}
			if (this.m_Campaign.playerDataPersistent == null)
			{
				return;
			}
			bool disable = CampaignUtils.IsAutoTeams(this.m_Campaign);
			this.DisableTeam(disable);
		}

		// Token: 0x06004D09 RID: 19721 RVA: 0x0022C857 File Offset: 0x0022AA57
		private void DisableTeam(bool disable)
		{
			if (this.m_Team != null)
			{
				this.m_Team.Enable(!disable, false);
			}
		}

		// Token: 0x06004D0A RID: 19722 RVA: 0x0022C878 File Offset: 0x0022AA78
		private bool IsReady()
		{
			return this.m_Campaign != null && this.m_Campaign.playerDataPersistent != null && this.m_Campaign.playerDataPersistent.Length > this.m_PlayerIndex && this.m_Campaign.playerDataPersistent[this.m_PlayerIndex].GetVar("ready", null, true).Bool();
		}

		// Token: 0x06004D0B RID: 19723 RVA: 0x0022C8DC File Offset: 0x0022AADC
		private void CycleTeams(BSGButton b)
		{
			UserInteractionLogger.LogNewLine(b, null);
			if (!this.CanCycleTeam())
			{
				return;
			}
			int num = CampaignUtils.GetTeam(this.m_Campaign, this.m_PlayerIndex);
			int teamsCount = CampaignUtils.GetTeamsCount(this.m_Campaign);
			num++;
			if (num >= teamsCount)
			{
				num = 0;
			}
			CampaignUtils.SetTeam(this.m_Campaign, this.m_PlayerIndex, num);
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x0022C934 File Offset: 0x0022AB34
		private bool CanCycleTeam()
		{
			return this.m_Campaign != null && this.m_Campaign.state < Campaign.State.Started && this.m_Campaign.playerDataPersistent != null && (this.m_Campaign.IsAuthority() || !this.m_Campaign.IsLocalPlayerReady()) && !CampaignUtils.IsAutoTeams(this.m_Campaign) && !(this.m_Campaign.playerDataPersistent[this.m_PlayerIndex].GetPlayerID() != THQNORequest.userId) && !this.m_Campaign.GetVar(RemoteVars.DataType.NonPersistentPlayerData, this.m_Campaign.GetLocalPlayerID(), "start_countdown");
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x0022C9E0 File Offset: 0x0022ABE0
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x0022C9E8 File Offset: 0x0022ABE8
		private void HandleOnPlayerNameClick(BSGButton b)
		{
			if (this.m_Actions.HasValidActions())
			{
				this.m_Actions.Show(true);
			}
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x0022CA04 File Offset: 0x0022AC04
		private bool HandleShieldClick(PointerEventData e, KingdomShield s)
		{
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			if (titleUI == null)
			{
				return true;
			}
			titleUI.UpdateSelectedKingdom(this.m_Campaign, this.m_PlayerIndex);
			return true;
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x0022CA35 File Offset: 0x0022AC35
		public virtual void OnPointerEnter(PointerEventData e)
		{
			this.UpdateHighlight();
		}

		// Token: 0x06004D11 RID: 19729 RVA: 0x0022CA35 File Offset: 0x0022AC35
		public virtual void OnPointerExit(PointerEventData e)
		{
			this.UpdateHighlight();
		}

		// Token: 0x06004D12 RID: 19730 RVA: 0x0022CA3D File Offset: 0x0022AC3D
		public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
		{
			if (!(this == null))
			{
				this.m_Invalidate = true;
				return;
			}
			Campaign campaign = this.m_Campaign;
			if (campaign == null)
			{
				return;
			}
			campaign.DelVarsListener(this);
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x0022CA64 File Offset: 0x0022AC64
		public static UILobbyPlayers.UIPlayerTeamSlot Create(Campaign campaign, int playerIndex, GameObject prototype, Transform parent)
		{
			if (campaign == null)
			{
				return null;
			}
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			UILobbyPlayers.UIPlayerTeamSlot uiplayerTeamSlot = gameObject.AddComponent<UILobbyPlayers.UIPlayerTeamSlot>();
			uiplayerTeamSlot.SetData(campaign, playerIndex);
			gameObject.SetActive(true);
			return uiplayerTeamSlot;
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x0022CAB0 File Offset: 0x0022ACB0
		public static string IntToRomanNumber(int number)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int[] array = new int[]
			{
				1,
				4,
				5,
				9,
				10,
				40,
				50,
				90,
				100,
				400,
				500,
				900,
				1000
			};
			string[] array2 = new string[]
			{
				"I",
				"IV",
				"V",
				"IX",
				"X",
				"XL",
				"L",
				"XC",
				"C",
				"CD",
				"D",
				"CM",
				"M"
			};
			while (number > 0)
			{
				for (int i = array.Length - 1; i >= 0; i--)
				{
					if (number / array[i] >= 1)
					{
						number -= array[i];
						stringBuilder.Append(array2[i]);
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x0022CB84 File Offset: 0x0022AD84
		public string GetConnectionStatusKey()
		{
			Campaign.SlotState slotState = this.m_Campaign.GetSlotState(this.m_PlayerIndex);
			bool flag = slotState == Campaign.SlotState.Empty;
			bool flag2 = !flag && this.m_Campaign.GetPlayerRuntimeState(this.m_PlayerIndex) > Campaign.RuntimeState.Offline;
			bool flag3 = this.m_Campaign.IsInCampaign(this.m_PlayerIndex);
			bool flag4 = !flag && !flag3;
			bool flag5 = this.m_Campaign.state >= Campaign.State.Started;
			switch (slotState)
			{
			case Campaign.SlotState.Empty:
				return "LobbyPlayerWindow.empty";
			case Campaign.SlotState.Eliminated:
				return "LobbyPlayerWindow.eliminated";
			case Campaign.SlotState.Kicked:
				if (!flag5)
				{
					return "LobbyPlayerWindow.empty";
				}
				return "LobbyPlayerWindow.kicked";
			case Campaign.SlotState.Reserved:
				return "LobbyPlayerWindow.reserved";
			case Campaign.SlotState.Closed:
				return "LobbyPlayerWindow.closed";
			}
			if (flag2)
			{
				return "LobbyPlayerWindow.in_lobby";
			}
			if (flag4)
			{
				if (!flag5)
				{
					return "LobbyPlayerWindow.empty";
				}
				return "LobbyPlayerWindow.left";
			}
			else
			{
				if (CampaignUtils.IsPlayerLoaded(GameLogic.Get(false), this.m_PlayerIndex))
				{
					return "LobbyPlayerWindow.playing";
				}
				return "LobbyPlayerWindow.offline";
			}
		}

		// Token: 0x06004D16 RID: 19734 RVA: 0x0022CC78 File Offset: 0x0022AE78
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1864414919U)
			{
				if (num <= 1063586346U)
				{
					if (num != 748274432U)
					{
						if (num == 1063586346U)
						{
							if (key == "has_options")
							{
								return MPBoss.Get().GetValidSlotActions(this.m_PlayerIndex).Count > 0;
							}
						}
					}
					else if (key == "player")
					{
						return "#" + this.m_Campaign.GetPlayerName(this.m_PlayerIndex);
					}
				}
				else if (num != 1130061116U)
				{
					if (num == 1864414919U)
					{
						if (key == "coop_game")
						{
							return CampaignUtils.IsCoop(this.m_Campaign);
						}
					}
				}
				else if (key == "connection_status")
				{
					return this.GetConnectionStatusKey();
				}
			}
			else if (num <= 2734521612U)
			{
				if (num != 1909552382U)
				{
					if (num == 2734521612U)
					{
						if (key == "team")
						{
							int team = CampaignUtils.GetTeam(this.m_Campaign, this.m_PlayerIndex);
							if (team < 0)
							{
								return "";
							}
							return "#" + UILobbyPlayers.UIPlayerTeamSlot.IntToRomanNumber(team + 1);
						}
					}
				}
				else if (key == "can_change_team")
				{
					return this.CanCycleTeam();
				}
			}
			else if (num != 2959219945U)
			{
				if (num == 3024911855U)
				{
					if (key == "team_based_game")
					{
						return !CampaignUtils.IsAutoTeams(this.m_Campaign);
					}
				}
			}
			else if (key == "is_hosted")
			{
				return this.m_Campaign.GetRuntimeState() > Campaign.RuntimeState.Offline;
			}
			return this.m_Campaign.GetVar(key, vars, as_value);
		}

		// Token: 0x04003B88 RID: 15240
		[UIFieldTarget("id_GameOwnerIcon")]
		private Image m_GameOwnerIcon;

		// Token: 0x04003B89 RID: 15241
		[UIFieldTarget("id_IconAllowGameToStartWithoutMe")]
		private Image m_IconAllowGameToStartWithoutMe;

		// Token: 0x04003B8A RID: 15242
		[UIFieldTarget("id_Host")]
		private Image m_Host;

		// Token: 0x04003B8B RID: 15243
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003B8C RID: 15244
		[UIFieldTarget("id_PlayerName")]
		private TMP_Text m_PlayerName;

		// Token: 0x04003B8D RID: 15245
		[UIFieldTarget("id_PlayerKingdom")]
		private TMP_Text m_PlayerKingdom;

		// Token: 0x04003B8E RID: 15246
		[UIFieldTarget("id_Team")]
		private BSGButton m_Team;

		// Token: 0x04003B8F RID: 15247
		[UIFieldTarget("id_TeamIndex")]
		private TMP_Text m_TeamIndex;

		// Token: 0x04003B90 RID: 15248
		[UIFieldTarget("id_PlayerKingdomIcon")]
		private UIKingdomIcon m_PlayerKingdomIcon;

		// Token: 0x04003B91 RID: 15249
		[UIFieldTarget("id_EmptyPlayerKingdomIcon")]
		private Image id_EmptyPlayerKingdomIcon;

		// Token: 0x04003B92 RID: 15250
		[UIFieldTarget("id_Background")]
		private GameObject m_Background;

		// Token: 0x04003B93 RID: 15251
		[UIFieldTarget("id_Background_Ready")]
		private GameObject m_Background_Ready;

		// Token: 0x04003B94 RID: 15252
		[UIFieldTarget("id_Background_NotReady")]
		private GameObject m_Background_NotRead;

		// Token: 0x04003B95 RID: 15253
		[UIFieldTarget("id_ReligionIcon")]
		private Image m_ReligionIcon;

		// Token: 0x04003B96 RID: 15254
		[UIFieldTarget("id_EliminatedIndication")]
		private Image m_EliminatedIndication;

		// Token: 0x04003B97 RID: 15255
		[UIFieldTarget("id_TeamBackground")]
		private BSGButtonImage m_TeamBackground;

		// Token: 0x04003B98 RID: 15256
		[UIFieldTarget("id_PlayerActions")]
		private GameObject m_PlayerActionsContainer;

		// Token: 0x04003B99 RID: 15257
		[UIFieldTarget("id_PlayerNameHitBox")]
		private BSGButton m_PlayerNameHitBox;

		// Token: 0x04003B9A RID: 15258
		[UIFieldTarget("id_VerisonMismatch")]
		private GameObject m_VerisonMismatch;

		// Token: 0x04003B9B RID: 15259
		private Campaign m_Campaign;

		// Token: 0x04003B9C RID: 15260
		private UILobbyPlayers.UISlotActions m_Actions;

		// Token: 0x04003B9D RID: 15261
		private UILobbyPlayers.UIPlayerTeamSlot.Mode m_mode;

		// Token: 0x04003B9E RID: 15262
		private int m_PlayerIndex;

		// Token: 0x04003B9F RID: 15263
		private bool m_DisableKingdomIconTooltip = true;

		// Token: 0x04003BA0 RID: 15264
		private bool m_Initialized;

		// Token: 0x04003BA1 RID: 15265
		private bool m_Invalidate;

		// Token: 0x02000A27 RID: 2599
		private enum Mode
		{
			// Token: 0x0400469B RID: 18075
			Lobby,
			// Token: 0x0400469C RID: 18076
			CampaignInstect
		}
	}

	// Token: 0x020007A9 RID: 1961
	internal class UISlotActions : MonoBehaviour, RemoteVars.IListener, IListener
	{
		// Token: 0x06004D18 RID: 19736 RVA: 0x0022CE94 File Offset: 0x0022B094
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_ActionIconPrototype != null)
			{
				this.m_ActionIconPrototype.SetActive(false);
			}
			if (this.m_Close != null)
			{
				this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
			}
			this.m_Initialized = true;
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x0022CEF7 File Offset: 0x0022B0F7
		public void SetIndex(int index, Campaign campaign)
		{
			this.Init();
			Campaign campaign2 = this.m_Campaign;
			if (campaign2 != null)
			{
				campaign2.DelVarsListener(this);
			}
			this.slotIndex = index;
			this.m_Campaign = campaign;
			Campaign campaign3 = this.m_Campaign;
			if (campaign3 != null)
			{
				campaign3.AddVarsListener(this);
			}
			this.BuildActions();
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x0022CF38 File Offset: 0x0022B138
		private void BuildActions()
		{
			UICommon.DeleteActiveChildren(this.m_ActionsContainer);
			MPBoss mpboss = MPBoss.Get();
			List<string> list = (mpboss != null) ? mpboss.GetValidSlotActions(this.slotIndex) : null;
			if (list == null || list.Count == 0)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				UILobbyPlayers.UIActionIcon.Create(list[i], this.slotIndex, this.m_ActionIconPrototype, this.m_ActionsContainer);
			}
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x0022CFA4 File Offset: 0x0022B1A4
		private void Update()
		{
			if (this.m_Invalidate)
			{
				this.BuildActions();
				this.m_Invalidate = false;
			}
			this.CheckOutOfBoundsClick();
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x0022CFC1 File Offset: 0x0022B1C1
		private void CheckOutOfBoundsClick()
		{
			if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)) && !RectTransformUtility.RectangleContainsScreenPoint(this.m_RectTransform, Input.mousePosition, null))
			{
				this.Show(false);
			}
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x0022CFFC File Offset: 0x0022B1FC
		public bool HasValidActions()
		{
			MPBoss mpboss = MPBoss.Get();
			List<string> list = (mpboss != null) ? mpboss.GetValidSlotActions(this.slotIndex) : null;
			return list != null && list.Count > 0;
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x00101716 File Offset: 0x000FF916
		public void Show(bool shown)
		{
			base.gameObject.SetActive(shown);
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x0022D02F File Offset: 0x0022B22F
		private void HandleOnClose(BSGButton btn)
		{
			this.Show(false);
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x0022D038 File Offset: 0x0022B238
		public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
		{
			if (key == "slot_state")
			{
				this.m_Invalidate = true;
			}
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnMessage(object obj, string message, object param)
		{
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0022D04E File Offset: 0x0022B24E
		private void OnDestroy()
		{
			Campaign campaign = this.m_Campaign;
			if (campaign == null)
			{
				return;
			}
			campaign.DelVarsListener(this);
		}

		// Token: 0x04003BA2 RID: 15266
		[UIFieldTarget("id_ActionIconPrototype")]
		private GameObject m_ActionIconPrototype;

		// Token: 0x04003BA3 RID: 15267
		[UIFieldTarget("id_ActionsContainer")]
		private RectTransform m_ActionsContainer;

		// Token: 0x04003BA4 RID: 15268
		[UIFieldTarget("id_Close")]
		private BSGButton m_Close;

		// Token: 0x04003BA5 RID: 15269
		private Campaign m_Campaign;

		// Token: 0x04003BA6 RID: 15270
		private int slotIndex;

		// Token: 0x04003BA7 RID: 15271
		private bool m_Invalidate;

		// Token: 0x04003BA8 RID: 15272
		private bool m_Initialized;

		// Token: 0x04003BA9 RID: 15273
		private RectTransform m_RectTransform;
	}

	// Token: 0x020007AA RID: 1962
	internal class UIActionIcon : Hotspot
	{
		// Token: 0x06004D24 RID: 19748 RVA: 0x0022D061 File Offset: 0x0022B261
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialized = true;
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x0022D07C File Offset: 0x0022B27C
		public void SetData(string key, int slot_index)
		{
			this.Init();
			this.m_ActionKey = key;
			this.m_Index = slot_index;
			DT.Field defField = global::Defs.GetDefField("LobbyPlayerWindow", null);
			this.def = ((defField != null) ? defField.FindChild(this.m_ActionKey, null, true, true, true, '.') : null);
			Tooltip.Get(base.gameObject, true).SetDef("PlayerSlotActionTooltip", new Vars(this.def));
			this.Refresh();
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x0022D0ED File Offset: 0x0022B2ED
		private void Refresh()
		{
			if (this.m_Icon != null)
			{
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>(this.def, "icon", null);
			}
		}

		// Token: 0x06004D27 RID: 19751 RVA: 0x0022D119 File Offset: 0x0022B319
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			MPBoss mpboss = MPBoss.Get();
			if (mpboss == null)
			{
				return;
			}
			mpboss.PerformSlotAction(this.m_Index, this.m_ActionKey);
		}

		// Token: 0x06004D28 RID: 19752 RVA: 0x0022D13E File Offset: 0x0022B33E
		public static UILobbyPlayers.UIActionIcon Create(string action_key, int slot_index, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.SetActive(true);
			UILobbyPlayers.UIActionIcon orAddComponent = gameObject.GetOrAddComponent<UILobbyPlayers.UIActionIcon>();
			orAddComponent.SetData(action_key, slot_index);
			return orAddComponent;
		}

		// Token: 0x04003BAA RID: 15274
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x04003BAB RID: 15275
		private string m_ActionKey;

		// Token: 0x04003BAC RID: 15276
		private int m_Index;

		// Token: 0x04003BAD RID: 15277
		private DT.Field def;

		// Token: 0x04003BAE RID: 15278
		private bool m_Initialized;
	}

	// Token: 0x020007AB RID: 1963
	internal class UICampaignNameField : MonoBehaviour
	{
		// Token: 0x06004D2A RID: 19754 RVA: 0x0022D177 File Offset: 0x0022B377
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_CampaignNameInputField = base.gameObject.GetComponent<TMP_InputField>();
			this.AddListeners();
			this.m_Initialized = true;
		}

		// Token: 0x06004D2B RID: 19755 RVA: 0x0022D1A7 File Offset: 0x0022B3A7
		public void SetCampaign(Campaign campaign)
		{
			this.Init();
			this.m_Campaign = campaign;
			this.Refresh();
		}

		// Token: 0x06004D2C RID: 19756 RVA: 0x0022D1BC File Offset: 0x0022B3BC
		public void Refresh()
		{
			if (this.m_Campaign == null)
			{
				return;
			}
			if (this.m_CampaignNameInputField == null)
			{
				return;
			}
			string text = this.m_Campaign.GetCustomName();
			if (string.IsNullOrEmpty(text))
			{
				text = global::Defs.Localize(this.m_Campaign.GetNameKey(), this.m_Campaign, null, true, true);
			}
			this.m_CampaignNameInputField.text = text;
		}

		// Token: 0x06004D2D RID: 19757 RVA: 0x0022D21C File Offset: 0x0022B41C
		private void AddListeners()
		{
			UnityAction<string> call = delegate(string val)
			{
				if (!this.m_CampaignNameInputField.wasCanceled)
				{
					this.OnFieldChanged(val);
				}
				this.m_OriginalValue = null;
			};
			this.m_CampaignNameInputField.onSubmit.AddListener(call);
			UnityAction<string> call2 = delegate(string val)
			{
				this.m_OriginalValue = this.m_CampaignNameInputField.text;
			};
			this.m_CampaignNameInputField.onSelect.AddListener(call2);
			UnityAction<string> call3 = delegate(string val)
			{
				if (this.m_OriginalValue != this.m_CampaignNameInputField.text)
				{
					this.OnFieldChanged(val);
				}
				this.m_OriginalValue = null;
			};
			this.m_CampaignNameInputField.onDeselect.AddListener(call3);
		}

		// Token: 0x06004D2E RID: 19758 RVA: 0x0022D283 File Offset: 0x0022B483
		private void OnFieldChanged(Value val)
		{
			this.m_Campaign.campaignData.Set("name", val, true);
		}

		// Token: 0x04003BAF RID: 15279
		private TMP_InputField m_CampaignNameInputField;

		// Token: 0x04003BB0 RID: 15280
		private Campaign m_Campaign;

		// Token: 0x04003BB1 RID: 15281
		private bool m_Initialized;

		// Token: 0x04003BB2 RID: 15282
		private string m_OriginalValue;
	}
}
