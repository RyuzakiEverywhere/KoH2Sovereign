using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000247 RID: 583
public class UICampaign : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, RemoteVars.IListener
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x0600238A RID: 9098 RVA: 0x0014072B File Offset: 0x0013E92B
	// (set) Token: 0x0600238B RID: 9099 RVA: 0x00140733 File Offset: 0x0013E933
	public Campaign campaign { get; private set; }

	// Token: 0x0600238C RID: 9100 RVA: 0x0014073C File Offset: 0x0013E93C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Empty != null)
		{
			this.m_Empty.onClick = new BSGButton.OnClick(this.HandleOnEmpty);
			this.m_Empty.AllowSelection(true);
		}
		if (this.m_Populated != null)
		{
			this.m_Populated.onClick = new BSGButton.OnClick(this.HandleOnEmpty);
		}
		if (this.btn_CreateNew != null)
		{
			this.btn_CreateNew.onClick = new BSGButton.OnClick(this.HandleOnCreateNew);
			Tooltip.Get(this.btn_CreateNew.gameObject, true).SetDef("CreateNewCampaignTooltip", null);
		}
		if (this.btn_Search != null)
		{
			this.btn_Search.onClick = new BSGButton.OnClick(this.HandleOnSearch);
			Tooltip.Get(this.btn_Search.gameObject, true).SetDef("FindCampaignTooltip", null);
		}
		if (this.m_PlayerStausPrototype != null)
		{
			this.m_PlayerStausPrototype.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x00140858 File Offset: 0x0013EA58
	public void SetCampaign(Campaign c)
	{
		this.Init();
		Campaign campaign = this.campaign;
		if (campaign != null)
		{
			campaign.DelVarsListener(this);
		}
		this.campaign = c;
		Campaign campaign2 = this.campaign;
		if (campaign2 != null)
		{
			campaign2.AddVarsListener(this);
		}
		if (this.m_ModCompatibility != null)
		{
			this.m_ModCompatibility.SetObject(this.campaign, null);
		}
		this.PopulateStatics();
		this.Refresh();
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x001408C4 File Offset: 0x0013EAC4
	private void PopulateStatics()
	{
		if (this.m_CreateNewLabel != null)
		{
			UIText.SetTextKey(this.m_CreateNewLabel, "MultiplayerMenu.new_campaign", null, null);
		}
		if (this.m_SearchLabel != null)
		{
			UIText.SetTextKey(this.m_SearchLabel, "MultiplayerMenu.search", null, null);
		}
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x00140911 File Offset: 0x0013EB11
	private void OnDisable()
	{
		this.m_StartNewGameTransition = false;
		Campaign campaign = this.campaign;
		if (campaign == null)
		{
			return;
		}
		campaign.DelVarsListener(this);
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x0014092B File Offset: 0x0013EB2B
	private void Update()
	{
		this.UpdateState();
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x00140934 File Offset: 0x0013EB34
	private void UpdateState()
	{
		if (this.m_Populated != null)
		{
			this.m_Populated.transform.localPosition = (this.m_Selected ? new Vector3(0f, -5f, 0f) : new Vector3(0f, 0f, 0f));
		}
		if (this.m_BackgroundSelected != null)
		{
			this.m_BackgroundSelected.gameObject.SetActive(this.m_Selected);
		}
		if (this.m_Finished != null)
		{
			this.m_Finished.gameObject.SetActive(this.campaign.state == Campaign.State.Closed);
		}
		if (this.m_MainGoalIcon != null)
		{
			this.m_MainGoalIcon.sprite = this.GetGameModeIcon(this.campaign);
		}
		if (this.m_MainGoalName != null)
		{
			UIText.SetText(this.m_MainGoalName, this.GetGameModeName(this.campaign));
			this.m_MainGoalName.gameObject.SetActive(!this.m_StartNewGameTransition);
		}
		if (this.m_CreationDate != null)
		{
			UIText.SetText(this.m_CreationDate, this.campaign.GetCreationTime().ToString("d"));
			this.m_CreationDate.gameObject.SetActive(this.m_Selected);
		}
		this.isEmpty = (this.campaign == null || this.campaign.state == Campaign.State.Empty);
		BSGButton populated = this.m_Populated;
		if (populated != null)
		{
			populated.gameObject.SetActive(!this.isEmpty);
		}
		if (this.m_Empty != null)
		{
			this.m_Empty.gameObject.SetActive(this.isEmpty);
			this.m_Empty.SetSelected(this.isEmpty && this.m_Selected, false);
			this.m_Empty.transform.localPosition = (this.m_Selected ? new Vector3(0f, -5f, 0f) : new Vector3(0f, 0f, 0f));
		}
		if (this.m_HostedIcon != null)
		{
			string key = (this.campaign.GetRuntimeState() >= Campaign.RuntimeState.Lobby) ? "hosted" : "icon_not_hosted";
			this.m_HostedIcon.overrideSprite = global::Defs.GetObj<Sprite>("MultiplayerMenu", key, null);
			this.m_HostedIcon.gameObject.SetActive(this.campaign.state != Campaign.State.Closed);
		}
		if (this.m_OutcomeLabel != null)
		{
			bool flag = this.campaign.state == Campaign.State.Closed;
			this.m_OutcomeLabel.gameObject.SetActive(flag);
			if (flag)
			{
				bool flag2 = false;
				bool flag3 = false;
				int localPlayerIndex = this.campaign.GetLocalPlayerIndex(true);
				if (localPlayerIndex >= 0)
				{
					flag2 = this.campaign.playerDataPersistent[localPlayerIndex].GetVar("victor", null, true).Bool();
					flag3 = this.campaign.playerDataPersistent[localPlayerIndex].GetVar("looser", null, true).Bool();
				}
				if (flag2)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.victory_label", null, null);
				}
				else if (flag3)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.defeated_label", null, null);
				}
				else if (this.campaign.state == Campaign.State.Closed)
				{
					UIText.SetTextKey(this.m_OutcomeLabel, "LobbyPlayerWindow.finshed_label", null, null);
				}
				else
				{
					this.m_OutcomeLabel.gameObject.SetActive(false);
				}
			}
		}
		if (this.m_OwnderKingdomIcon != null)
		{
			int playerKingdomCoAIndex = this.campaign.GetPlayerKingdomCoAIndex(this.campaign.GetOwnerID());
			this.m_OwnderKingdomIcon.SetIndex(playerKingdomCoAIndex);
			this.m_OwnderKingdomIcon.gameObject.SetActive(!this.m_StartNewGameTransition);
		}
		if (this.m_ButtonsEmpty != null)
		{
			this.m_ButtonsEmpty.SetActive(this.m_Selected);
		}
		if (this.m_UnreadMessages)
		{
			this.m_UnreadMessages.SetActive(this.m_HasUnseenChatMessage);
		}
		this.PopulatePlayerStates();
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x00140D3C File Offset: 0x0013EF3C
	private void PopulatePlayerStates()
	{
		if (this.m_Players != null)
		{
			this.m_Players.gameObject.SetActive(!this.m_StartNewGameTransition);
		}
		if (this.m_PlayerIconsContainer == null)
		{
			return;
		}
		if (this.m_PlayerStausPrototype == null)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.m_PlayerIconsContainer.transform);
		if (this.campaign == null)
		{
			return;
		}
		if (this.m_StartNewGameTransition)
		{
			return;
		}
		for (int i = 0; i < this.campaign.playerDataPersistent.Length; i++)
		{
			RemoteVars remoteVars = this.campaign.playerDataPersistent[i];
			string kingdomName = this.campaign.GetKingdomName(i, false);
			bool flag = string.IsNullOrEmpty(kingdomName);
			GameObject gameObject = global::Common.Spawn(this.m_PlayerStausPrototype, this.m_PlayerIconsContainer.transform, false, "");
			gameObject.SetActive(true);
			UIKingdomIcon uikingdomIcon = global::Common.FindChildComponent<UIKingdomIcon>(gameObject, "id_KingdomIcon");
			if (uikingdomIcon != null)
			{
				uikingdomIcon.gameObject.SetActive(!flag);
				if (!flag)
				{
					int playerKingdomCoAIndexByKingdomName = this.campaign.GetPlayerKingdomCoAIndexByKingdomName(kingdomName);
					uikingdomIcon.SetIndex(playerKingdomCoAIndexByKingdomName);
					uikingdomIcon.GetPrimary().DisbaleTooltip(true);
				}
			}
			UIKingdomIcon uikingdomIcon2 = global::Common.FindChildComponent<UIKingdomIcon>(gameObject, "id_EmptyShield");
			if (uikingdomIcon2 != null)
			{
				uikingdomIcon2.gameObject.SetActive(flag);
			}
			Image image = global::Common.FindChildComponent<Image>(gameObject, "id_StateIcon");
			if (image != null)
			{
				if (!flag)
				{
					string key = (this.campaign.GetPlayerRuntimeState(i) == Campaign.RuntimeState.Lobby) ? "icon_player_online" : "icon_player_offline";
					image.overrideSprite = global::Defs.GetObj<Sprite>("MultiplayerMenu", key, null);
				}
				image.gameObject.SetActive(!flag);
			}
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x00140EDB File Offset: 0x0013F0DB
	private GameObject GetSlotUIElement()
	{
		if (this.isEmpty)
		{
			BSGButton empty = this.m_Empty;
			if (empty == null)
			{
				return null;
			}
			return empty.gameObject;
		}
		else
		{
			BSGButton populated = this.m_Populated;
			if (populated == null)
			{
				return null;
			}
			return populated.gameObject;
		}
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x00140F08 File Offset: 0x0013F108
	private void HandleOnCreateNew(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		if (this.campaign == null || this.campaign.state != Campaign.State.Empty)
		{
			return;
		}
		this.m_StartNewGameTransition = true;
		BaseUI.Get().AddInteractionBlocker("enter_new_game_loby");
		Logic.Coroutine.Start("CreateNewCampaign", MPBoss.Get().CreateNewCampaign(this.campaign, delegate(Campaign x)
		{
			this.OpenLobby();
		}), null);
		this.Select(false);
		this.UpdateState();
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x00140F7D File Offset: 0x0013F17D
	public void OpenLobby()
	{
		this.m_StartNewGameTransition = false;
		BaseUI.Get<TitleUI>().EnableLobbyScreen(this.campaign);
		BaseUI.Get().RemoveInteractionBlocker("enter_new_game_loby");
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x00140FA5 File Offset: 0x0013F1A5
	private void HandleOnSearch(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		BaseUI.Get<TitleUI>().EnableLobbyBrowserScreen();
		this.Select(false);
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x00140FBF File Offset: 0x0013F1BF
	private void HandleOnEmpty(BSGButton b)
	{
		if (this.parent == null)
		{
			return;
		}
		this.parent.Select(this.campaign);
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x00140FE1 File Offset: 0x0013F1E1
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("WTF");
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x00140FED File Offset: 0x0013F1ED
	public void Select(bool select)
	{
		if (this.m_Selected == select)
		{
			return;
		}
		this.m_Selected = select;
		if (select)
		{
			this.m_HasUnseenChatMessage = false;
		}
		this.UpdateState();
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x00141010 File Offset: 0x0013F210
	public void Refresh()
	{
		this.UpdateState();
		if (this.campaign != null)
		{
			string text = (this.campaign.campaignData == null) ? "null" : (this.campaign.campaignData.GetVar("name", null, true).String(null) ?? "no-name");
			UIText.SetText(this.m_CampaignName, text);
			UIText.SetText(this.m_Owner, this.GetOwnerName(this.campaign));
		}
		if (this.m_HostedIcon != null)
		{
			Vars vars = new Vars();
			string authorityPlayerID = this.campaign.GetAuthorityPlayerID();
			string playerName = this.campaign.GetPlayerName(authorityPlayerID);
			vars.Set<string>("player", "#" + playerName);
			vars.Set<bool>("is_hosted", this.campaign.GetRuntimeState() > Campaign.RuntimeState.Offline);
			if (this.m_HostedIcon != null)
			{
				Tooltip.Get(this.m_HostedIcon.gameObject, true).SetDef("GameHostTooltip", vars);
			}
		}
		if (this.m_ModCompatibility != null)
		{
			this.m_ModCompatibility.SetObject(this.campaign, null);
		}
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x00141134 File Offset: 0x0013F334
	private Sprite GetGameModeIcon(Campaign campaign)
	{
		DT.Field varsDef = campaign.GetVarsDef();
		if (varsDef == null || varsDef.children == null)
		{
			return null;
		}
		DT.Field field = varsDef.FindChild("main_goal", null, true, true, true, '.');
		if (field == null)
		{
			return null;
		}
		DT.Field selectedOption = this.GetSelectedOption(campaign, field);
		if (selectedOption == null)
		{
			return null;
		}
		return global::Defs.GetObj<Sprite>(selectedOption, "icon_wide", null);
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x00141188 File Offset: 0x0013F388
	private string GetGameModeName(Campaign campaign)
	{
		DT.Field varsDef = campaign.GetVarsDef();
		if (varsDef == null || varsDef.children == null)
		{
			return null;
		}
		DT.Field field = varsDef.FindChild("main_goal", null, true, true, true, '.');
		if (field == null)
		{
			return null;
		}
		DT.Field selectedOption = this.GetSelectedOption(campaign, field);
		if (selectedOption == null)
		{
			return null;
		}
		string key = (selectedOption.key != "None") ? "name" : "secondary_name";
		return global::Defs.Localize(selectedOption, key, null, null, true, true);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x001411F8 File Offset: 0x0013F3F8
	private DT.Field GetSelectedOption(Campaign campaign, DT.Field rule)
	{
		if (campaign == null)
		{
			return null;
		}
		if (campaign.campaignData == null)
		{
			return null;
		}
		if (rule == null)
		{
			return null;
		}
		Value var = campaign.campaignData.GetVar(rule.key, null, true);
		DT.Field varOptions = campaign.GetVarOptions(rule.key);
		for (int i = 0; i < varOptions.children.Count; i++)
		{
			DT.Field field = varOptions.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				Value optionValue = campaign.GetOptionValue(field);
				if (optionValue.is_valid && optionValue == var)
				{
					return field;
				}
			}
		}
		return null;
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x0014128C File Offset: 0x0013F48C
	private string GetOwnerName(Campaign campaign)
	{
		if (campaign == null)
		{
			return "";
		}
		string ownerID = campaign.GetOwnerID();
		return campaign.GetPlayerName(ownerID) ?? "";
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x001412B9 File Offset: 0x0013F4B9
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (key == "history" && !this.m_Selected)
		{
			this.m_HasUnseenChatMessage = true;
		}
		this.Refresh();
	}

	// Token: 0x040017CE RID: 6094
	[HideInInspector]
	public UICampaigns parent;

	// Token: 0x040017CF RID: 6095
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040017D0 RID: 6096
	[UIFieldTarget("id_CampaignName")]
	private TMP_Text m_CampaignName;

	// Token: 0x040017D1 RID: 6097
	[UIFieldTarget("id_OwnerName")]
	private TMP_Text m_Owner;

	// Token: 0x040017D2 RID: 6098
	[UIFieldTarget("id_MainGoalIcon")]
	private Image m_MainGoalIcon;

	// Token: 0x040017D3 RID: 6099
	[UIFieldTarget("id_MainGoalName")]
	private TMP_Text m_MainGoalName;

	// Token: 0x040017D4 RID: 6100
	[UIFieldTarget("id_CreationDate")]
	private TMP_Text m_CreationDate;

	// Token: 0x040017D5 RID: 6101
	[UIFieldTarget("id_Empty")]
	private BSGButton m_Empty;

	// Token: 0x040017D6 RID: 6102
	[UIFieldTarget("id_Populated")]
	private BSGButton m_Populated;

	// Token: 0x040017D7 RID: 6103
	[UIFieldTarget("id_Finished")]
	private GameObject m_Finished;

	// Token: 0x040017D8 RID: 6104
	[UIFieldTarget("id_OutcomeLabel")]
	private TMP_Text m_OutcomeLabel;

	// Token: 0x040017D9 RID: 6105
	[UIFieldTarget("id_BackgroundSelected")]
	private GameObject m_BackgroundSelected;

	// Token: 0x040017DA RID: 6106
	[UIFieldTarget("id_ButtonsEmpty")]
	private GameObject m_ButtonsEmpty;

	// Token: 0x040017DB RID: 6107
	[UIFieldTarget("id_CreateNew")]
	private BSGButton btn_CreateNew;

	// Token: 0x040017DC RID: 6108
	[UIFieldTarget("id_CreateNewLabel")]
	private TextMeshProUGUI m_CreateNewLabel;

	// Token: 0x040017DD RID: 6109
	[UIFieldTarget("id_Search")]
	private BSGButton btn_Search;

	// Token: 0x040017DE RID: 6110
	[UIFieldTarget("id_SearchLabel")]
	private TextMeshProUGUI m_SearchLabel;

	// Token: 0x040017DF RID: 6111
	[UIFieldTarget("id_HostedIcon")]
	private Image m_HostedIcon;

	// Token: 0x040017E0 RID: 6112
	[UIFieldTarget("id_UnreadMessages")]
	private GameObject m_UnreadMessages;

	// Token: 0x040017E1 RID: 6113
	[UIFieldTarget("id_Players")]
	private GameObject m_Players;

	// Token: 0x040017E2 RID: 6114
	[UIFieldTarget("id_PlayerIconsContainer")]
	private GameObject m_PlayerIconsContainer;

	// Token: 0x040017E3 RID: 6115
	[UIFieldTarget("id_PlayerStausPrototype")]
	private GameObject m_PlayerStausPrototype;

	// Token: 0x040017E4 RID: 6116
	[UIFieldTarget("id_OwnderKingdomIcon")]
	private UIKingdomIcon m_OwnderKingdomIcon;

	// Token: 0x040017E5 RID: 6117
	[UIFieldTarget("id_ModCompatibility")]
	private UIModCompatibilityIcon m_ModCompatibility;

	// Token: 0x040017E6 RID: 6118
	private bool isEmpty = true;

	// Token: 0x040017E7 RID: 6119
	private bool m_Selected;

	// Token: 0x040017E8 RID: 6120
	private bool m_HasUnseenChatMessage;

	// Token: 0x040017E9 RID: 6121
	private bool m_StartNewGameTransition;

	// Token: 0x040017EA RID: 6122
	private bool m_Initialzied;
}
