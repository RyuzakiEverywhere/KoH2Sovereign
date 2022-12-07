using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200024C RID: 588
public class UILobbyBrowser : MonoBehaviour
{
	// Token: 0x060023FA RID: 9210 RVA: 0x00143E64 File Offset: 0x00142064
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Back != null)
		{
			this.m_Back.onClick = new BSGButton.OnClick(this.HandleOnBack);
			Tooltip.Get(this.m_Back.gameObject, true).SetDef("GameBrowserBackTooltip", null);
		}
		if (this.m_EnterCode != null)
		{
			this.m_EnterCode.onClick = new BSGButton.OnClick(this.HandleOnEnterCode);
			this.m_EnterCode.AllowSelection(true);
			Tooltip.Get(this.m_EnterCode.gameObject, true).SetDef("GameBrowserEnterCodeTooltip", null);
		}
		if (this.m_JoinGame != null)
		{
			this.m_JoinGame.onClick = new BSGButton.OnClick(this.HandleOnJoinGame);
		}
		if (this.m_Search != null)
		{
			this.m_Search.onClick = new BSGButton.OnClick(this.HandleOnSearch);
			Tooltip.Get(this.m_Search.gameObject, true).SetDef("GameBrowserSearchGamesTooltip", null);
		}
		if (this.m_Refresh != null)
		{
			this.m_Refresh.onClick = new BSGButton.OnClick(this.HandleOnRefresh);
			Tooltip.Get(this.m_Refresh.gameObject, true).SetDef("GameBrowserRefreshGamesTooltip", null);
		}
		if (this.m_ResetSorting != null)
		{
			this.m_ResetSorting.onClick = new BSGButton.OnClick(this.HandleOnResetSort);
			Tooltip.Get(this.m_ResetSorting.gameObject, true).SetDef("GameBrowserResetSortingTooltip", null);
		}
		if (this.m_SearchFitlerField != null)
		{
			this.m_SearchFitlerField.onSubmit.AddListener(new UnityAction<string>(this.HandleOnSearchSubmitValue));
		}
		if (this.m_Table)
		{
			UIGenericTable table = this.m_Table;
			table.OnRowSelected = (Action<UIGenericTable.Row, int>)Delegate.Combine(table.OnRowSelected, new Action<UIGenericTable.Row, int>(this.HandleOnTableRowSelect));
			UIGenericTable table2 = this.m_Table;
			table2.OnRowFocused = (Action<UIGenericTable.Row, int>)Delegate.Combine(table2.OnRowFocused, new Action<UIGenericTable.Row, int>(this.HandleOnTableRowDoubleClick));
		}
		this.m_IsGettingLobbyList = false;
		this.m_FailToGetLobbyList = false;
		this.m_Initalized = true;
		this.ShowLoadingLobbiesNorification(false);
		this.UpdateJoinButtonTooltip();
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x001440A0 File Offset: 0x001422A0
	private void LocalizeStatics()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "TitleScreen.Multiplayer.GameBrowser.title", null, null);
		}
		if (this.m_BackLabel != null)
		{
			UIText.SetTextKey(this.m_BackLabel, "TitleScreen.Multiplayer.GameBrowser.back", null, null);
		}
		if (this.m_EnterCodeLabel != null)
		{
			UIText.SetTextKey(this.m_EnterCodeLabel, "TitleScreen.Multiplayer.GameBrowser.enter_code", null, null);
		}
		if (this.m_JoinGameLabel != null)
		{
			UIText.SetTextKey(this.m_JoinGameLabel, "TitleScreen.Multiplayer.GameBrowser.join_game", null, null);
		}
		if (this.m_SearchLabel != null)
		{
			UIText.SetTextKey(this.m_SearchLabel, "TitleScreen.Multiplayer.GameBrowser.search", null, null);
		}
		if (this.m_ResetSortingLabel != null)
		{
			UIText.SetTextKey(this.m_ResetSortingLabel, "TitleScreen.Multiplayer.GameBrowser.reset_sorting", null, null);
		}
		if (this.m_RefreshLabel != null)
		{
			UIText.SetTextKey(this.m_RefreshLabel, "TitleScreen.Multiplayer.GameBrowser.refresh", null, null);
		}
		if (this.m_SearchFitlerField != null)
		{
			UIText.SetTextKey(this.m_SearchFitlerField.placeholder.GetComponent<TextMeshProUGUI>(), "TitleScreen.Multiplayer.GameBrowser.search_prompt", null, null);
		}
		if (this.m_LoadingGamesNotificationDescription != null)
		{
			UIText.SetTextKey(this.m_LoadingGamesNotificationDescription, "TitleScreen.Multiplayer.GameBrowser.loading_games_list", null, null);
		}
		if (this.m_EmptyGameListNotificationDescription != null)
		{
			UIText.SetTextKey(this.m_EmptyGameListNotificationDescription, "TitleScreen.Multiplayer.GameBrowser.no_games_list", null, null);
		}
		if (this.m_ErrorOnGameListLoadNotificationDescription != null)
		{
			UIText.SetTextKey(this.m_ErrorOnGameListLoadNotificationDescription, "TitleScreen.Multiplayer.GameBrowser.failed_loading_games_list", null, null);
		}
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x00144218 File Offset: 0x00142418
	private void UpdateJoinButtonTooltip()
	{
		if (this.m_JoinGame == null)
		{
			return;
		}
		if (this.m_SelectedLobby != null && !this.IsSameVersion(this.m_SelectedLobby))
		{
			Tooltip.Get(this.m_JoinGame.gameObject, true).SetDef("CampaignVersionIncompatibleTooltip", null);
			return;
		}
		Tooltip.Get(this.m_JoinGame.gameObject, true).SetDef("GameBrowserJoinGameTooltip", null);
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x00144283 File Offset: 0x00142483
	private void OnEnable()
	{
		this.Init();
		this.LocalizeStatics();
		this.ClearLobbyBrowser();
		this.GetLobbyList();
		BSGButton joinGame = this.m_JoinGame;
		if (joinGame == null)
		{
			return;
		}
		joinGame.Enable(true, false);
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x001442AF File Offset: 0x001424AF
	private void OnDisable()
	{
		this.ClearLobbyBrowser();
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x001442B8 File Offset: 0x001424B8
	private void Update()
	{
		this.UpdateForEmptyGameListNotification();
		if (this.m_InvalidateLobbyData)
		{
			this.GetLobbyList();
			this.m_InvalidateLobbyData = false;
		}
		if (this.m_EnterCode != null)
		{
			this.m_EnterCode.SetSelected(UILobbyBrowser.EnterInvateCodeWindow.IsActive(), false);
		}
		if (this.m_JoinGame != null)
		{
			this.m_JoinGame.Enable(this.m_SelectedLobby != null && this.IsSameVersion(this.m_SelectedLobby), false);
		}
		if (this.m_Refresh != null)
		{
			this.m_Refresh.Enable(!this.m_IsGettingLobbyList, false);
		}
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x00144354 File Offset: 0x00142554
	private void UpdateForEmptyGameListNotification()
	{
		bool flag = this.m_LobbiesData != null && this.m_LobbiesData.Count > 0;
		if (this.m_EmptyGameListNotificationDescription != null)
		{
			this.m_EmptyGameListNotificationDescription.gameObject.SetActive(!this.m_IsGettingLobbyList && !flag && !this.m_FailToGetLobbyList);
		}
		if (this.m_ErrorOnGameListLoadNotificationDescription != null)
		{
			this.m_ErrorOnGameListLoadNotificationDescription.gameObject.SetActive(!this.m_IsGettingLobbyList && this.m_FailToGetLobbyList);
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x001443DF File Offset: 0x001425DF
	private void ShowLoadingLobbiesNorification(bool shown)
	{
		if (this.m_LoadingGamesNotification != null)
		{
			this.m_LoadingGamesNotification.gameObject.SetActive(shown);
		}
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x00144400 File Offset: 0x00142600
	private void ClearLobbyBrowser()
	{
		this.m_IsGettingLobbyList = false;
		this.m_FailToGetLobbyList = false;
		this.m_LobbiesData.Clear();
		if (UILobbyBrowser.EnterInvateCodeWindow.current != null)
		{
			UILobbyBrowser.EnterInvateCodeWindow.current.Close(true);
		}
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x00144434 File Offset: 0x00142634
	private void PopulateTable()
	{
		if (this.m_Table == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("MultiplayerBrowserTable", null);
		if (defField == null)
		{
			return;
		}
		List<KeyValuePair<Value, Vars>> list = new List<KeyValuePair<Value, Vars>>();
		this.m_LobbiesData.Sort((UILobbyBrowser.LobbyData g1, UILobbyBrowser.LobbyData g2) => g2.creationDate.CompareTo(g1.creationDate));
		for (int i = 0; i < this.m_LobbiesData.Count; i++)
		{
			UILobbyBrowser.LobbyData lobbyData = this.m_LobbiesData[i];
			if (lobbyData != null && this.MatchFilter(lobbyData))
			{
				list.Add(new KeyValuePair<Value, Vars>(new Value(this.m_LobbiesData[i]), null));
			}
		}
		UIGenericTable.ColumnHeader currentSortColumn = this.m_Table.CurrentSortColumn;
		UIGenericTable.SortOrder currentSortOrder = this.m_Table.CurrentSortOrder;
		UILobbyBrowser.LobbyData currentSelectedLobby = this.GetCurrentSelectedLobby();
		TableDataProvider dp = TableDataProvider.CreateFromDef(defField, list);
		this.m_Table.Build(dp);
		this.m_Table.ApplySort(currentSortColumn, currentSortOrder);
		this.TryReselectLobby(currentSelectedLobby);
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x00144530 File Offset: 0x00142730
	private bool MatchFilter(UILobbyBrowser.LobbyData data)
	{
		return data != null && (string.IsNullOrEmpty(this.m_CurrentFilter) || data.GetVar("name", null, true).ToLowerInvariant().Contains(this.m_CurrentFilter));
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x00144570 File Offset: 0x00142770
	private UILobbyBrowser.LobbyData GetCurrentSelectedLobby()
	{
		if (this.m_Table == null)
		{
			return null;
		}
		Value selectedRowData = this.m_Table.GetSelectedRowData();
		if (selectedRowData == Value.Null)
		{
			return null;
		}
		if (selectedRowData.obj_val is UILobbyBrowser.LobbyData)
		{
			return selectedRowData.obj_val as UILobbyBrowser.LobbyData;
		}
		return null;
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x001445C4 File Offset: 0x001427C4
	private void TryReselectLobby(UILobbyBrowser.LobbyData lobbyData)
	{
		if (lobbyData == null)
		{
			return;
		}
		if (this.m_Table == null)
		{
			return;
		}
		string val = lobbyData.Data.Get("lobby_id", true);
		int num = -1;
		int rowCount = this.m_Table.GetRowCount();
		for (int i = 0; i < rowCount; i++)
		{
			Value value = this.m_Table.GeDataAtIndex(i);
			if (value.obj_val is UILobbyBrowser.LobbyData && val == (value.obj_val as UILobbyBrowser.LobbyData).Data.Get("lobby_id", true))
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		this.m_Table.SelectRow(num);
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x00144670 File Offset: 0x00142870
	private bool ShouldLobbyBeVisible(UILobbyBrowser.LobbyData lobbyData)
	{
		if (!Game.isDefaultLobbyFilterEnabled)
		{
			return true;
		}
		if (lobbyData == null)
		{
			Debug.Log("ShouldLobbyBeVisible() called with null lobby data!");
			return false;
		}
		if (lobbyData.Data.Get("owner_id", true) == THQNORequest.userId)
		{
			return false;
		}
		Value v = lobbyData.Data.Get("client_branch", true);
		return !(v != Value.Null) || !(v != Title.BranchName());
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x001446F0 File Offset: 0x001428F0
	private void ApplySeacrhFilter()
	{
		this.m_CurrentFilter = this.m_SearchFitlerField.text;
		if (this.m_CurrentFilter.Length < 3)
		{
			this.m_CurrentFilter = "";
		}
		else
		{
			this.m_CurrentFilter = this.m_CurrentFilter.ToLowerInvariant();
		}
		this.PopulateTable();
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x00144740 File Offset: 0x00142940
	private bool IsSameVersion(UILobbyBrowser.LobbyData lobbyData)
	{
		return lobbyData != null && lobbyData.Data.Get("client_version", true) == Title.Version(true);
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x00144768 File Offset: 0x00142968
	private void HandleOnTableRowSelect(UIGenericTable.Row r, int index)
	{
		if (this.m_IsJoiningCampaign)
		{
			return;
		}
		if (index == -1)
		{
			return;
		}
		if (this.m_LobbiesData == null)
		{
			return;
		}
		if (this.m_LobbiesData.Count <= index)
		{
			return;
		}
		this.m_SelectedLobby = this.m_LobbiesData[index];
		this.UpdateJoinButtonTooltip();
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x001447A8 File Offset: 0x001429A8
	private void HandleOnTableRowDoubleClick(UIGenericTable.Row r, int index)
	{
		if (this.m_IsJoiningCampaign)
		{
			return;
		}
		if (index == -1)
		{
			return;
		}
		if (this.m_LobbiesData == null)
		{
			return;
		}
		if (this.m_LobbiesData.Count <= index)
		{
			return;
		}
		this.m_SelectedLobby = this.m_LobbiesData[index];
		this.UpdateJoinButtonTooltip();
		if (this.IsSameVersion(this.m_SelectedLobby))
		{
			this.RefreshLobbyDataAndJoin(this.m_SelectedLobby);
		}
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x0014480D File Offset: 0x00142A0D
	private void HandleOnSearchSubmitValue(string input)
	{
		if (this.m_SearchFitlerField.wasCanceled)
		{
			return;
		}
		this.ApplySeacrhFilter();
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x00144823 File Offset: 0x00142A23
	private void HandleOnBack(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		TitleUI titleUI = BaseUI.Get<TitleUI>();
		if (titleUI == null)
		{
			return;
		}
		titleUI.EnableMultiplayerMenu(null);
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x0014483C File Offset: 0x00142A3C
	private void HandleOnEnterCode(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		BaseUI baseUI = BaseUI.Get();
		if (!UILobbyBrowser.EnterInvateCodeWindow.IsActive())
		{
			UILobbyBrowser.EnterInvateCodeWindow.Create(baseUI.message_container.transform as RectTransform);
		}
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x00144873 File Offset: 0x00142A73
	private void HandleOnJoinGame(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		this.RefreshLobbyDataAndJoin(this.m_SelectedLobby);
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x00144888 File Offset: 0x00142A88
	private void HandleOnSearch(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		this.ApplySeacrhFilter();
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x00144897 File Offset: 0x00142A97
	private void HandleOnResetSort(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		this.m_Table.ClearSort();
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x001448AB File Offset: 0x00142AAB
	private void HandleOnRefresh(BSGButton b)
	{
		UserInteractionLogger.LogNewLine(b, null);
		if (this.m_IsGettingLobbyList)
		{
			return;
		}
		this.GetLobbyList();
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x001448C3 File Offset: 0x00142AC3
	public void GetLobbyList()
	{
		if (this.m_IsGettingLobbyList)
		{
			return;
		}
		Logic.Coroutine.Start("ClientCampaigns.GetLobbyList", this.<GetLobbyList>g__coro|52_0(), null);
		this.ShowLoadingLobbiesNorification(true);
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x001448E8 File Offset: 0x00142AE8
	private void OnLobbyListData(string[] data)
	{
		this.ShowLoadingLobbiesNorification(false);
		if (MPBoss.Get() == null)
		{
			return;
		}
		this.m_LobbiesData.Clear();
		if (data == null)
		{
			return;
		}
		Array.Sort<string>(data);
		for (int i = 0; i < data.Length; i++)
		{
			UILobbyBrowser.LobbyData lobbyData = new UILobbyBrowser.LobbyData(data[i]);
			string value = lobbyData.Data.Get("campaign_id", true);
			bool flag = lobbyData.Data.Get("is_playing", true);
			if (!string.IsNullOrEmpty(value) && !flag && this.ShouldLobbyBeVisible(lobbyData))
			{
				this.m_LobbiesData.Add(lobbyData);
			}
		}
		this.PopulateTable();
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x00144984 File Offset: 0x00142B84
	public void RefreshLobbyDataAndJoin(UILobbyBrowser.LobbyData lobby_data)
	{
		UILobbyBrowser.<>c__DisplayClass54_0 CS$<>8__locals1 = new UILobbyBrowser.<>c__DisplayClass54_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.lobby_id = lobby_data.Data.Get("lobby_id", true);
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.AddInteractionBlocker("join_lobby");
		}
		Logic.Coroutine.Start("RefreshLobbyDataAndJoin", CS$<>8__locals1.<RefreshLobbyDataAndJoin>g__coro|0(), null);
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x001449DC File Offset: 0x00142BDC
	public void DoJoin(string lobby_id, int lobby_max_players)
	{
		UILobbyBrowser.<>c__DisplayClass55_0 CS$<>8__locals1 = new UILobbyBrowser.<>c__DisplayClass55_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.lobby_id = lobby_id;
		CS$<>8__locals1.lobby_max_players = lobby_max_players;
		if (string.IsNullOrEmpty(CS$<>8__locals1.lobby_id))
		{
			return;
		}
		MPBoss mpboss = MPBoss.Get();
		if (((mpboss != null) ? mpboss.multiplayerCampaigns : null) == null)
		{
			return;
		}
		this.m_IsJoiningCampaign = true;
		BSGButton joinGame = this.m_JoinGame;
		if (joinGame != null)
		{
			joinGame.Enable(false, false);
		}
		Logic.Coroutine.Start("Join with first free slot", CS$<>8__locals1.<DoJoin>g__JoinCoro|0(), null);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x00144A54 File Offset: 0x00142C54
	private Campaign GetFirstFreeSlot()
	{
		MPBoss mpboss = MPBoss.Get();
		Campaign[] array = (mpboss != null) ? mpboss.multiplayerCampaigns : null;
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].state == Campaign.State.Empty)
			{
				return array[i];
			}
		}
		return null;
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x00144AAD File Offset: 0x00142CAD
	[CompilerGenerated]
	private IEnumerator <GetLobbyList>g__coro|52_0()
	{
		this.m_FailToGetLobbyList = false;
		if (!THQNORequest.initted || !THQNORequest.connected || !THQNORequest.signed_in)
		{
			yield return Logic.Coroutine.Return(Value.Null, "THQNO not initted, connected or signed in!");
		}
		this.m_IsGettingLobbyList = true;
		yield return Logic.Coroutine.Start("THQNOGetLobbyList", THQNORequest.RequestLobbyListCoro(true), null);
		if (string.IsNullOrEmpty(Logic.Coroutine.Error))
		{
			string[] data = (string[])Logic.Coroutine.Result.obj_val;
			this.OnLobbyListData(data);
		}
		else
		{
			this.m_FailToGetLobbyList = true;
			this.OnLobbyListData(new string[0]);
		}
		this.m_IsGettingLobbyList = false;
		yield break;
	}

	// Token: 0x04001846 RID: 6214
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001847 RID: 6215
	[UIFieldTarget("id_Refresh")]
	private BSGButton m_Refresh;

	// Token: 0x04001848 RID: 6216
	[UIFieldTarget("id_RefreshLabel")]
	private TextMeshProUGUI m_RefreshLabel;

	// Token: 0x04001849 RID: 6217
	[UIFieldTarget("id_Back")]
	private BSGButton m_Back;

	// Token: 0x0400184A RID: 6218
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI m_BackLabel;

	// Token: 0x0400184B RID: 6219
	[UIFieldTarget("id_EnterCode")]
	private BSGButton m_EnterCode;

	// Token: 0x0400184C RID: 6220
	[UIFieldTarget("id_EnterCodeLabel")]
	private TextMeshProUGUI m_EnterCodeLabel;

	// Token: 0x0400184D RID: 6221
	[UIFieldTarget("id_JoinGame")]
	private BSGButton m_JoinGame;

	// Token: 0x0400184E RID: 6222
	[UIFieldTarget("id_JoinGameLabel")]
	private TextMeshProUGUI m_JoinGameLabel;

	// Token: 0x0400184F RID: 6223
	[UIFieldTarget("id_Search")]
	private BSGButton m_Search;

	// Token: 0x04001850 RID: 6224
	[UIFieldTarget("id_SearchLabel")]
	private TextMeshProUGUI m_SearchLabel;

	// Token: 0x04001851 RID: 6225
	[UIFieldTarget("id_ResetSorting")]
	private BSGButton m_ResetSorting;

	// Token: 0x04001852 RID: 6226
	[UIFieldTarget("id_ResetSortingLabel")]
	private TextMeshProUGUI m_ResetSortingLabel;

	// Token: 0x04001853 RID: 6227
	[UIFieldTarget("id_SearchFitlerField")]
	private TMP_InputField m_SearchFitlerField;

	// Token: 0x04001854 RID: 6228
	[UIFieldTarget("id_Table")]
	private UIGenericTable m_Table;

	// Token: 0x04001855 RID: 6229
	[UIFieldTarget("id_LoadingGamesNotification")]
	private GameObject m_LoadingGamesNotification;

	// Token: 0x04001856 RID: 6230
	[UIFieldTarget("id_LoadingGamesNotificationDescription")]
	private TextMeshProUGUI m_LoadingGamesNotificationDescription;

	// Token: 0x04001857 RID: 6231
	[UIFieldTarget("id_EmptyGameListNotificationDescription")]
	private TextMeshProUGUI m_EmptyGameListNotificationDescription;

	// Token: 0x04001858 RID: 6232
	[UIFieldTarget("id_ErrorOnGameListLoadNotificationDescription")]
	private TextMeshProUGUI m_ErrorOnGameListLoadNotificationDescription;

	// Token: 0x04001859 RID: 6233
	private List<UILobbyBrowser.LobbyData> m_LobbiesData = new List<UILobbyBrowser.LobbyData>();

	// Token: 0x0400185A RID: 6234
	private bool m_Initalized;

	// Token: 0x0400185B RID: 6235
	private bool m_InvalidateLobbyData;

	// Token: 0x0400185C RID: 6236
	private bool m_IsGettingLobbyList;

	// Token: 0x0400185D RID: 6237
	private bool m_FailToGetLobbyList;

	// Token: 0x0400185E RID: 6238
	private UILobbyBrowser.LobbyData m_SelectedLobby;

	// Token: 0x0400185F RID: 6239
	private bool m_IsJoiningCampaign;

	// Token: 0x04001860 RID: 6240
	private string m_CurrentFilter;

	// Token: 0x020007A2 RID: 1954
	public class LobbyData : IVars
	{
		// Token: 0x06004CD7 RID: 19671 RVA: 0x0022B004 File Offset: 0x00229204
		public LobbyData(string lobby_id)
		{
			this.Data = this.GetLobbyData(lobby_id);
			DateTime dateTime;
			if (DateTime.TryParse(this.Data.Get("creation_time", true), out dateTime))
			{
				this.creationDate = dateTime.Ticks;
			}
			this.Data.Set<int>("main_goal_index", this.GetIndexDef("main_goal", this.Data.Get<string>("main_goal", null), false));
			this.Data.Set<int>("team_size_index", this.GetIndexDef("team_size", this.Data.Get<string>("team_size", null), false));
			this.Data.Set<int>("start_period_index", this.GetIndexDef("start_period", this.Data.Get<string>("start_period", null), false));
			this.Data.Set<int>("pick_kingdom_index", this.GetIndexDef("pick_kingdom", this.Data.Get<string>("pick_kingdom", null), true));
			this.Data.Set<int>("kingdom_size_index", this.GetIndexDef("kingdom_size", this.Data.Get<string>("kingdom_size", null), true));
			this.Data.Set<int>("ai_difficulty_index", this.GetIndexDef("ai_difficulty", this.Data.Get<string>("ai_difficulty", null), false));
			this.Data.Set<int>("time_limit_index", this.GetIndexDef("time_limit", this.Data.Get<string>("time_limit", null), false));
		}

		// Token: 0x06004CD8 RID: 19672 RVA: 0x0022B19C File Offset: 0x0022939C
		private int GetIndexDef(string rule_key, string option_key, bool inverse = false)
		{
			DT.Field campaign_vars_def = Campaign.campaign_vars_def;
			if (campaign_vars_def == null)
			{
				return 0;
			}
			DT.Field field = campaign_vars_def.FindChild(rule_key, null, true, true, true, '.');
			if (field == null)
			{
				return 0;
			}
			DT.Field field2 = field.FindChild("options", null, true, true, true, '.');
			if (field2 == null)
			{
				return 0;
			}
			List<DT.Field> list = field2.Children();
			if (list == null || list.Count == 0)
			{
				return 0;
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(list[i].key))
				{
					list.RemoveAt(i);
				}
			}
			int j = 0;
			int count = list.Count;
			while (j < count)
			{
				if (list[j].key == option_key)
				{
					if (!inverse)
					{
						return j;
					}
					return count - j - 1;
				}
				else
				{
					j++;
				}
			}
			return 0;
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x0022B264 File Offset: 0x00229464
		private Vars GetLobbyData(string lobby_id)
		{
			Vars vars = new Vars();
			vars.Set<string>("lobby_id", lobby_id);
			this.GetStringVar(vars, lobby_id, "owner_name");
			this.GetStringVar(vars, lobby_id, "owner_id");
			this.GetStringVar(vars, lobby_id, "name");
			this.GetStringVar(vars, lobby_id, "main_goal");
			this.GetStringVar(vars, lobby_id, "host_message");
			this.GetStringVar(vars, lobby_id, "campaign_id");
			this.GetStringVar(vars, lobby_id, "team_size");
			this.GetStringVar(vars, lobby_id, "start_period");
			this.GetStringVar(vars, lobby_id, "pick_kingdom");
			this.GetStringVar(vars, lobby_id, "kingdom_size");
			this.GetStringVar(vars, lobby_id, "ai_difficulty");
			this.GetStringVar(vars, lobby_id, "time_limit");
			this.GetStringVar(vars, lobby_id, "mod_id");
			this.GetIntVar(vars, lobby_id, "player_cnt");
			this.GetIntVar(vars, lobby_id, "max_players");
			this.GetIntVar(vars, lobby_id, "is_from_save");
			this.GetStringVar(vars, lobby_id, "client_version");
			this.GetStringVar(vars, lobby_id, "client_branch");
			this.GetStringVar(vars, lobby_id, "creation_time");
			return vars;
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x0022B37C File Offset: 0x0022957C
		private void GetStringVar(Vars var, string lobby_id, string key)
		{
			this.strBuilder.Clear();
			if (THQNORequest.GetLobbyStringData(lobby_id, key, this.strBuilder, 128U).error != null)
			{
				var.Set<string>(key, "");
				return;
			}
			string text = this.strBuilder.ToString();
			if (string.IsNullOrEmpty(text))
			{
				var.Set<string>(key, null);
				return;
			}
			var.Set<string>(key, text);
		}

		// Token: 0x06004CDB RID: 19675 RVA: 0x0022B3E0 File Offset: 0x002295E0
		private void GetIntVar(Vars var, string lobby_id, string key)
		{
			THQNORequest lobbyIntData = THQNORequest.GetLobbyIntData(lobby_id, key);
			if (lobbyIntData.error != null)
			{
				var.Set<int>(key, 0);
				return;
			}
			var.Set<int>(key, lobbyIntData.result.Int(0));
		}

		// Token: 0x06004CDC RID: 19676 RVA: 0x0022B41C File Offset: 0x0022961C
		private void GetBoolVar(Vars var, string lobby_id, string key)
		{
			THQNORequest lobbyIntData = THQNORequest.GetLobbyIntData(lobby_id, key);
			if (lobbyIntData.error != null)
			{
				var.Set<bool>(key, false);
				return;
			}
			var.Set<bool>(key, lobbyIntData.result.Int(0) != 0);
		}

		// Token: 0x06004CDD RID: 19677 RVA: 0x0022B458 File Offset: 0x00229658
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 2369371622U)
			{
				if (num <= 441230752U)
				{
					if (num != 120167649U)
					{
						if (num != 228355902U)
						{
							if (num != 441230752U)
							{
								goto IL_36F;
							}
							if (!(key == "time_limit"))
							{
								goto IL_36F;
							}
						}
						else if (!(key == "kingdom_size"))
						{
							goto IL_36F;
						}
					}
					else if (!(key == "ai_difficulty"))
					{
						goto IL_36F;
					}
				}
				else if (num <= 793338472U)
				{
					if (num != 443087763U)
					{
						if (num != 793338472U)
						{
							goto IL_36F;
						}
						if (!(key == "player_cnt"))
						{
							goto IL_36F;
						}
						return this.Data.Get(key, true);
					}
					else
					{
						if (!(key == "map_size"))
						{
							goto IL_36F;
						}
						goto IL_2CE;
					}
				}
				else if (num != 1515052130U)
				{
					if (num != 2369371622U)
					{
						goto IL_36F;
					}
					if (!(key == "name"))
					{
						goto IL_36F;
					}
					return "#" + this.Data.Get(key, true);
				}
				else if (!(key == "pick_kingdom"))
				{
					goto IL_36F;
				}
				return this.GetLocalzedRule(key, this.Data.Get(key, true));
			}
			if (num <= 3106606041U)
			{
				if (num <= 2616089560U)
				{
					if (num != 2615209153U)
					{
						if (num != 2616089560U)
						{
							goto IL_36F;
						}
						if (!(key == "team_size"))
						{
							goto IL_36F;
						}
					}
					else if (!(key == "start_period"))
					{
						goto IL_36F;
					}
				}
				else if (num != 2697281674U)
				{
					if (num != 3106606041U)
					{
						goto IL_36F;
					}
					if (!(key == "mod_id"))
					{
						goto IL_36F;
					}
					return this.Data.Get(key, true);
				}
				else
				{
					if (!(key == "owner_name"))
					{
						goto IL_36F;
					}
					return "#" + this.Data.Get("owner_name", true);
				}
			}
			else if (num <= 3613359752U)
			{
				if (num != 3344463765U)
				{
					if (num != 3613359752U)
					{
						goto IL_36F;
					}
					if (!(key == "max_players"))
					{
						goto IL_36F;
					}
					return this.Data.Get(key, true);
				}
				else
				{
					if (!(key == "custom_campaign_name"))
					{
						goto IL_36F;
					}
					if (string.IsNullOrEmpty(this.Data.Get("name", true)))
					{
						return Value.Null;
					}
					return "#" + this.Data.Get("name", true);
				}
			}
			else if (num != 3639724134U)
			{
				if (num != 3811882199U)
				{
					goto IL_36F;
				}
				if (!(key == "minor_victory"))
				{
					goto IL_36F;
				}
				return this.GetMinorVictoryDef();
			}
			else
			{
				if (!(key == "main_goal"))
				{
					goto IL_36F;
				}
				return this.GetLocalziedMainGoal();
			}
			IL_2CE:
			if (this.Data.Get<int>("is_from_save", 0) > 0)
			{
				return "#" + global::Defs.Localize("TitleScreen.Multiplayer.GameBrowser.starting_period_custom", null, null, true, true);
			}
			return this.GetLocalzedRule(key, this.Data.Get(key, true));
			IL_36F:
			return this.Data.Get(key, true);
		}

		// Token: 0x06004CDE RID: 19678 RVA: 0x0022B7E4 File Offset: 0x002299E4
		private Value GetMinorVictoryDef()
		{
			string path = "main_goal.options." + this.Data.Get("main_goal", true);
			return new Value(Campaign.campaign_vars_def.FindChild(path, null, true, true, true, '.'));
		}

		// Token: 0x06004CDF RID: 19679 RVA: 0x0022B828 File Offset: 0x00229A28
		private string GetLocalzedRule(string rule, string optionKey)
		{
			if (string.IsNullOrEmpty(optionKey))
			{
				return "";
			}
			DT.Field field = Campaign.campaign_vars_def.FindChild(rule + ".options." + optionKey, null, true, true, true, '.');
			return "#" + global::Defs.Localize(field, "name", null, null, true, true);
		}

		// Token: 0x06004CE0 RID: 19680 RVA: 0x0022B87C File Offset: 0x00229A7C
		private string GetLocalziedMainGoal()
		{
			string path = "main_goal.options." + this.Data.Get("main_goal", true);
			DT.Field field = Campaign.campaign_vars_def.FindChild(path, null, true, true, true, '.');
			return "#" + global::Defs.Localize(field, "name", null, null, true, true);
		}

		// Token: 0x04003B72 RID: 15218
		public Vars Data;

		// Token: 0x04003B73 RID: 15219
		private StringBuilder strBuilder = new StringBuilder(128);

		// Token: 0x04003B74 RID: 15220
		public long creationDate;
	}

	// Token: 0x020007A3 RID: 1955
	internal class EnterInvateCodeWindow : UIWindow
	{
		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06004CE1 RID: 19681 RVA: 0x0022B8D5 File Offset: 0x00229AD5
		// (set) Token: 0x06004CE2 RID: 19682 RVA: 0x0022B8DC File Offset: 0x00229ADC
		public static UILobbyBrowser.EnterInvateCodeWindow current { get; private set; }

		// Token: 0x06004CE3 RID: 19683 RVA: 0x0022B8E4 File Offset: 0x00229AE4
		private void Init()
		{
			if (this.m_Initialized)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Confirm != null)
			{
				this.m_Confirm.onClick = new BSGButton.OnClick(this.HandleOnConfirm);
			}
			if (this.m_Close != null)
			{
				this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
			}
			this.m_Initialized = true;
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x0022B954 File Offset: 0x00229B54
		private void LocalizeStatics()
		{
			if (this.m_Caption != null)
			{
				UIText.SetTextKey(this.m_Caption, "EnterInvateCodePopup.caption", null, null);
			}
			if (this.m_Description != null)
			{
				UIText.SetTextKey(this.m_Description, "EnterInvateCodePopup.description", null, null);
			}
			if (this.m_ConfirmLabel != null)
			{
				UIText.SetTextKey(this.m_ConfirmLabel, "EnterInvateCodePopup.confirm", null, null);
			}
			if (this.m_CodeInputPlaceholder != null)
			{
				UIText.SetTextKey(this.m_CodeInputPlaceholder, "EnterInvateCodePopup.input_hint", null, null);
			}
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x0022B9E1 File Offset: 0x00229BE1
		private void OnEnable()
		{
			this.Init();
			this.LocalizeStatics();
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x0022B9F0 File Offset: 0x00229BF0
		private void Join(string invite_code)
		{
			UILobbyBrowser.EnterInvateCodeWindow.<>c__DisplayClass15_0 CS$<>8__locals1 = new UILobbyBrowser.EnterInvateCodeWindow.<>c__DisplayClass15_0();
			CS$<>8__locals1.invite_code = invite_code;
			CS$<>8__locals1.mpBoss = MPBoss.Get();
			if (CS$<>8__locals1.mpBoss == null)
			{
				Debug.Log("JoinCampaignInviteCode failed. MPBoss is null!");
				return;
			}
			CS$<>8__locals1.campaign = CS$<>8__locals1.mpBoss.FindFirstEmptyCampaign();
			if (CS$<>8__locals1.campaign == null)
			{
				return;
			}
			Logic.Coroutine.Start("ClientCampaigns.GetLobbyList", CS$<>8__locals1.<Join>g__coro|0(), null);
			this.Close(false);
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x0022BA5B File Offset: 0x00229C5B
		private void HandleOnConfirm(BSGButton b)
		{
			if (this.m_CodeInput == null)
			{
				return;
			}
			this.Join(this.m_CodeInput.text);
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
		private void HandleOnClose(BSGButton b)
		{
			this.Close(false);
		}

		// Token: 0x06004CE9 RID: 19689 RVA: 0x0022BA7D File Offset: 0x00229C7D
		public override void Close(bool silent = false)
		{
			base.Close(silent);
			UILobbyBrowser.EnterInvateCodeWindow.current = null;
		}

		// Token: 0x06004CEA RID: 19690 RVA: 0x0022BA8C File Offset: 0x00229C8C
		public static GameObject GetPrefab()
		{
			return UICommon.GetPrefab(UILobbyBrowser.EnterInvateCodeWindow.def_id, null);
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x0022BA9C File Offset: 0x00229C9C
		public static UILobbyBrowser.EnterInvateCodeWindow Create(RectTransform parent)
		{
			if (parent == null)
			{
				return null;
			}
			GameObject prefab = UILobbyBrowser.EnterInvateCodeWindow.GetPrefab();
			if (prefab == null)
			{
				return null;
			}
			UILobbyBrowser.EnterInvateCodeWindow orAddComponent = global::Common.Spawn(prefab, parent, false, "").GetOrAddComponent<UILobbyBrowser.EnterInvateCodeWindow>();
			orAddComponent.gameObject.SetActive(true);
			orAddComponent.Open();
			UILobbyBrowser.EnterInvateCodeWindow.current = orAddComponent;
			return orAddComponent;
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x0022BAEF File Offset: 0x00229CEF
		public static bool IsActive()
		{
			return UILobbyBrowser.EnterInvateCodeWindow.current != null;
		}

		// Token: 0x04003B75 RID: 15221
		private static string def_id = "EnterInvateCodePopup";

		// Token: 0x04003B76 RID: 15222
		[UIFieldTarget("id_Button_Close")]
		private BSGButton m_Close;

		// Token: 0x04003B77 RID: 15223
		[UIFieldTarget("id_Caption")]
		private TextMeshProUGUI m_Caption;

		// Token: 0x04003B78 RID: 15224
		[UIFieldTarget("id_Description")]
		private TextMeshProUGUI m_Description;

		// Token: 0x04003B79 RID: 15225
		[UIFieldTarget("id_ConfirmLabel")]
		private TextMeshProUGUI m_ConfirmLabel;

		// Token: 0x04003B7A RID: 15226
		[UIFieldTarget("id_Confirm")]
		private BSGButton m_Confirm;

		// Token: 0x04003B7B RID: 15227
		[UIFieldTarget("id_CodeInput")]
		private TMP_InputField m_CodeInput;

		// Token: 0x04003B7C RID: 15228
		[UIFieldTarget("id_CodeInputPlaceholder")]
		private TextMeshProUGUI m_CodeInputPlaceholder;
	}
}
