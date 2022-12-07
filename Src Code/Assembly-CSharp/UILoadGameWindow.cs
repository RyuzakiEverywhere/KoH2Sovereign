using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200029D RID: 669
public class UILoadGameWindow : UIWindow, IVars
{
	// Token: 0x06002948 RID: 10568 RVA: 0x0015F72A File Offset: 0x0015D92A
	public override string GetDefId()
	{
		return UILoadGameWindow.def_id;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x0015F734 File Offset: 0x0015D934
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.vars = new Vars(this);
		if (this.m_btnLoad != null)
		{
			this.m_btnLoad.onClick = new BSGButton.OnClick(this.HandleOnLoadButtonClick);
			Tooltip.Get(this.m_btnLoad.gameObject, true).SetDef("LoadGameSaveTooltip", this.vars);
		}
		if (this.m_btnDelete != null)
		{
			this.m_btnDelete.onClick = delegate(BSGButton b)
			{
				this.OnDeleteSave();
			};
			Tooltip.Get(this.m_btnDelete.gameObject, true).SetDef("DeleteGameSaveTooltip", this.vars);
		}
		if (this.m_DeleteCampaign != null)
		{
			this.m_DeleteCampaign.onClick = delegate(BSGButton b)
			{
				this.OnDeleteCampaign();
			};
			Tooltip.Get(this.m_DeleteCampaign.gameObject, true).SetDef("DeleteCampaignTooltip", this.vars);
		}
		if (this.m_ToggleMultiplayerSaves != null)
		{
			this.m_ToggleMultiplayerSaves.onClick = delegate(BSGButton b)
			{
				this.HandleMultiplayerCampaign();
			};
		}
		if (this.m_ToggleSingleplayerSaves != null)
		{
			this.m_ToggleSingleplayerSaves.onClick = delegate(BSGButton b)
			{
				this.HandleSingleplayerCampaign();
			};
		}
		if (this.m_CampaignLabelPrototype != null)
		{
			this.m_CampaignLabelPrototype.gameObject.SetActive(false);
		}
		if (this.m_Close != null)
		{
			this.m_Close.onClick = delegate(BSGButton b)
			{
				this.Close(false);
			};
		}
		this.m_Initialized = true;
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x0015F8C4 File Offset: 0x0015DAC4
	public override void Show()
	{
		this.Init();
		this.LocalizeStatics();
		this.InitTable();
		this.UpdateSaveList(false);
		this.UpdateTabs();
		this.SelectCampaign(this.GetDefaultCampaign(), true);
		bool active = this.m_Mode == UILoadGameWindow.Mode.CampaignFromSave || !(BaseUI.Get() is TitleUI);
		this.m_PointerBlocker.gameObject.SetActive(active);
		base.Show();
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x0015F931 File Offset: 0x0015DB31
	public void SetLocalPosition(Vector2 pos)
	{
		if (this.m_Menu != null)
		{
			this.m_Menu.transform.localPosition = pos;
		}
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x0015F958 File Offset: 0x0015DB58
	private void LocalizeStatics()
	{
		if (this.m_Title != null)
		{
			string key = (this.m_Mode == UILoadGameWindow.Mode.CampaignFromSave) ? "SaveLoadMenuWindow.titleCreateFromSave" : "SaveLoadMenuWindow.titleLoad";
			UIText.SetTextKey(this.m_Title, key, null, null);
		}
		if (this.m_SaveNameHeader != null)
		{
			UIText.SetTextKey(this.m_SaveNameHeader, "SaveLoadMenuWindow.colNameName", null, null);
		}
		if (this.m_PlayimeHeader != null)
		{
			UIText.SetTextKey(this.m_PlayimeHeader, "SaveLoadMenuWindow.colNamePlayTime", null, null);
		}
		if (this.m_SaveDateHeader != null)
		{
			UIText.SetTextKey(this.m_SaveDateHeader, "SaveLoadMenuWindow.colNameSaveData", null, null);
		}
		if (this.m_LoadLabel != null)
		{
			string key2 = (this.m_Mode == UILoadGameWindow.Mode.CampaignFromSave) ? "SaveLoadMenuWindow.btnCreate" : "SaveLoadMenuWindow.btnLoad";
			UIText.SetTextKey(this.m_LoadLabel, key2, null, null);
		}
		if (this.m_DeleteLabel != null)
		{
			UIText.SetTextKey(this.m_DeleteLabel, "SaveLoadMenuWindow.btnDelete", null, null);
		}
		if (this.m_DeleteCampaignLabel != null)
		{
			UIText.SetTextKey(this.m_DeleteCampaignLabel, "SaveLoadMenuWindow.btnDeleteCampaign", null, null);
		}
		if (this.m_CampaignsLabel != null)
		{
			UIText.SetTextKey(this.m_CampaignsLabel, "SaveLoadMenuWindow.campaignsLabel", null, null);
		}
		if (this.m_ToggleSingleplayerSavesLabel != null)
		{
			UIText.SetTextKey(this.m_ToggleSingleplayerSavesLabel, "SaveLoadMenuWindow.toggleSingleplayer", null, null);
		}
		if (this.m_ToggleMultiplayerSavesLabel != null)
		{
			UIText.SetTextKey(this.m_ToggleMultiplayerSavesLabel, "SaveLoadMenuWindow.toggleMultiplayer", null, null);
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x0015FACC File Offset: 0x0015DCCC
	private void InitTable()
	{
		if (this.TableContainer != null)
		{
			List<Comparison<UITableRow>> list = new List<Comparison<UITableRow>>();
			list.Add(new Comparison<UITableRow>(this.CompareBySaveName));
			this.TableContainer.SetBorderSize(1f, true);
			this.TableContainer.SetComparisonFunctions(list);
			this.TableContainer.Init();
			this.TableContainer.Refresh(false);
		}
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x0015FB33 File Offset: 0x0015DD33
	private void SetFilter(UILoadGameWindow.CampaignFilter newFilter)
	{
		if (newFilter == this.m_CurrentFilter)
		{
			return;
		}
		this.m_CurrentFilter = newFilter;
		this.UpdateTabs();
		this.UpdateSaveList(false);
		this.SelectCampaign(this.GetDefaultCampaign(), true);
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x0015FB60 File Offset: 0x0015DD60
	private void SetMode(UILoadGameWindow.Mode mode)
	{
		this.m_Mode = mode;
		this.LocalizeStatics();
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x0015FB70 File Offset: 0x0015DD70
	private void UpdateTabs()
	{
		if (this.m_TabSingleplayerBackground != null)
		{
			this.m_TabSingleplayerBackground.SetActive(this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer);
		}
		if (this.m_TabMultiplayerBackground != null)
		{
			this.m_TabMultiplayerBackground.SetActive(this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Multiplayer);
		}
		if (this.m_ToggleMultiplayerSavesLabel != null)
		{
			Color color = global::Defs.GetColor("SaveLoadMenuWindow", (this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Multiplayer) ? "tab_label_active" : "tab_label_normal");
			this.m_ToggleMultiplayerSavesLabel.color = color;
		}
		if (this.m_ToggleSingleplayerSavesLabel != null)
		{
			Color color2 = global::Defs.GetColor("SaveLoadMenuWindow", (this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer) ? "tab_label_active" : "tab_label_normal");
			this.m_ToggleSingleplayerSavesLabel.color = color2;
		}
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x0015FC34 File Offset: 0x0015DE34
	private void SelectCampaign(SaveGame.CampaignInfo campaign, bool select_first_save = false)
	{
		this.m_SelectedCamaign = campaign;
		this.UpdateSaveList(false);
		base.StartCoroutine(this.Refresh(select_first_save));
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x0015FC52 File Offset: 0x0015DE52
	protected override void Update()
	{
		base.Update();
		this.UpdateButtonState();
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x0015FC60 File Offset: 0x0015DE60
	private void UpdateButtonState()
	{
		if (this.m_btnDelete)
		{
			this.m_btnDelete.Enable(this.CanDeleteSave(this.m_SelectedSaveInfo), false);
		}
		if (this.m_btnLoad)
		{
			bool enable = this.CanLoadSelectedSave();
			this.m_btnLoad.Enable(enable, false);
		}
		if (this.m_DeleteCampaign)
		{
			this.m_DeleteCampaign.Enable(this.CanDeleteCampaign(this.m_SelectedCamaign), false);
		}
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x0015FCD8 File Offset: 0x0015DED8
	private bool CanLoadSelectedSave()
	{
		return !(this.m_SelectedSaveRow == null) && (this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer || this.m_Mode == UILoadGameWindow.Mode.CampaignFromSave);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x0015FD00 File Offset: 0x0015DF00
	private bool CanDeleteSave(SaveGame.Info saveInfo)
	{
		return saveInfo != null && GameLogic.Get(true) != null && this.m_SelectedCamaign != null && !(this.m_SelectedCamaign.campaign.id != saveInfo.campaign_id) && (this.m_SelectedCamaign.latest_save != saveInfo || this.CanDeleteCampaign(this.m_SelectedCamaign));
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x0015FD64 File Offset: 0x0015DF64
	private bool CanDeleteCampaign(SaveGame.CampaignInfo campaignInfo)
	{
		if (campaignInfo == null)
		{
			return false;
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return false;
		}
		if (game.IsRunning())
		{
			string id = this.m_SelectedCamaign.campaign.id;
			Game game2 = GameLogic.Get(false);
			string b;
			if (game2 == null)
			{
				b = null;
			}
			else
			{
				Campaign campaign = game2.campaign;
				b = ((campaign != null) ? campaign.id : null);
			}
			if (id == b)
			{
				return false;
			}
		}
		if (this.m_SelectedCamaign.campaign.IsMultiplayerCampaign())
		{
			MPBoss mpboss = MPBoss.Get();
			if (mpboss != null && mpboss.GetCampaignIndex(this.m_SelectedCamaign.campaign.id) >= 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x0015FDFF File Offset: 0x0015DFFF
	private void HandleOnCampaingSelect(UILoadGameWindow.CampaignLabel label)
	{
		this.SelectCampaign(label.Data, true);
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x0015FE10 File Offset: 0x0015E010
	private void PopulateCampaigns()
	{
		if (this.m_CampaignsContainer == null)
		{
			return;
		}
		if (this.m_CampaignLabelPrototype == null)
		{
			return;
		}
		this.m_CampaingLabels.Clear();
		UICommon.DeleteActiveChildren(this.m_CampaignsContainer);
		List<SaveGame.CampaignInfo> campaigns = SaveGame.campaigns;
		for (int i = 0; i < campaigns.Count; i++)
		{
			SaveGame.CampaignInfo campaignInfo = campaigns[i];
			if ((this.m_CurrentFilter != UILoadGameWindow.CampaignFilter.Singleplayer || !campaignInfo.campaign.IsMultiplayerCampaign()) && (this.m_CurrentFilter != UILoadGameWindow.CampaignFilter.Multiplayer || campaignInfo.campaign.IsMultiplayerCampaign()))
			{
				UILoadGameWindow.CampaignLabel campaignLabel = UILoadGameWindow.CampaignLabel.Create(campaignInfo, this.m_CampaignLabelPrototype, this.m_CampaignsContainer);
				campaignLabel.OnSelect = new Action<UILoadGameWindow.CampaignLabel>(this.HandleOnCampaingSelect);
				campaignLabel.Select(this.m_SelectedCamaign == campaignInfo);
				this.m_CampaingLabels.Add(campaignLabel);
			}
		}
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x0015FEDC File Offset: 0x0015E0DC
	private void UpdateSaveList(bool forced = false)
	{
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.IsMultiplayer();
		}
		bool singleplayer_saves_only = this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer;
		bool list_all = true;
		if (forced)
		{
			SaveGame.ScanSavesDir(true);
		}
		SaveGame.UpdateList(singleplayer_saves_only, list_all);
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x0015FF17 File Offset: 0x0015E117
	public IEnumerator Refresh(bool select_first_save = false)
	{
		if (this.m_SelectedSaveRow != null)
		{
			this.m_SelectedSaveRow.Select(false);
		}
		this.m_SelectedSaveRow = null;
		if (this.TableContainer == null)
		{
			yield break;
		}
		this.PopulateCampaigns();
		if (this.tableRowPrefab != null && this.TableContent != null && this.TableContainer != null && this.m_SelectedCamaign != null)
		{
			List<SaveGame.Info> saves = this.m_SelectedCamaign.saves;
			if (select_first_save)
			{
				this.m_SelectedSaveInfo = ((saves != null && saves.Count > 0) ? saves[0] : null);
			}
			bool flag = this.m_SelectedSaveRow == null && this.m_SelectedSaveInfo != null;
			this.TableContainer.Clear();
			if (saves.Count != this.m_SaveRows.Count)
			{
				for (int i = 0; i < this.m_SaveRows.Count; i++)
				{
					UnityEngine.Object.Destroy(this.m_SaveRows[i].gameObject);
				}
				this.m_SaveRows.Clear();
			}
			for (int j = 0; j < saves.Count; j++)
			{
				SaveLoadMenuRow saveLoadMenuRow;
				if (this.m_SaveRows.Count <= j)
				{
					saveLoadMenuRow = global::Common.Spawn(this.tableRowPrefab, this.TableContent.transform, false, "").GetComponent<SaveLoadMenuRow>();
					this.m_SaveRows.Add(saveLoadMenuRow);
				}
				else
				{
					saveLoadMenuRow = this.m_SaveRows[j];
				}
				if (saveLoadMenuRow != null)
				{
					UITableRow component = saveLoadMenuRow.GetComponent<UITableRow>();
					if (component != null)
					{
						UITableRow uitableRow = component;
						uitableRow.OnSelected = (UITableRow.OnSelectedEvent)Delegate.Combine(uitableRow.OnSelected, new UITableRow.OnSelectedEvent(this.HandleOnSaveSelect));
						UITableRow uitableRow2 = component;
						uitableRow2.OnFocus = (UITableRow.OnDoubleClickEvent)Delegate.Combine(uitableRow2.OnFocus, new UITableRow.OnDoubleClickEvent(this.HandleOnSaveRowDoubleClick));
						this.TableContainer.AddRow(component);
					}
					SaveGame.Info info = saves[j];
					saveLoadMenuRow.SetInfo(info);
					saveLoadMenuRow.Refresh();
					if (flag && this.m_SelectedSaveInfo == info)
					{
						this.m_SelectedSaveRow = component;
						this.m_SelectedSaveRow.Select(true);
						flag = false;
					}
					Tooltip.Get(saveLoadMenuRow.gameObject, true).SetDef("LoadGameSaveTooltip", this.vars);
				}
			}
			yield return null;
			yield return null;
			using (List<SaveLoadMenuRow>.Enumerator enumerator = this.m_SaveRows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SaveLoadMenuRow saveLoadMenuRow2 = enumerator.Current;
					saveLoadMenuRow2.RefreshHeight();
				}
				goto IL_31C;
			}
		}
		for (int k = 0; k < this.m_SaveRows.Count; k++)
		{
			UnityEngine.Object.Destroy(this.m_SaveRows[k].gameObject);
		}
		this.m_SaveRows.Clear();
		IL_31C:
		if (this.TableContainer != null)
		{
			this.TableContainer.Refresh(true);
		}
		if (this.m_SelectedCamaign != null)
		{
			for (int l = 0; l < this.m_CampaingLabels.Count; l++)
			{
				UILoadGameWindow.CampaignLabel campaignLabel = this.m_CampaingLabels[l];
				campaignLabel.Select(campaignLabel.Data.campaign.id == this.m_SelectedCamaign.campaign.id);
			}
		}
		this.UpdateCampaignData();
		yield break;
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x0015FF30 File Offset: 0x0015E130
	private void UpdateCampaignData()
	{
		bool flag = this.m_SelectedCamaign != null;
		if (this.m_CampaignIcon != null)
		{
			if (flag)
			{
				DT.Field varDef = this.m_SelectedCamaign.campaign.GetVarDef("main_goal");
				DT.Field selectedOption = CampaignUtils.GetSelectedOption(this.m_SelectedCamaign.campaign, varDef);
				this.m_CampaignIcon.overrideSprite = global::Defs.GetObj<Sprite>(selectedOption, "icon", null);
			}
			else
			{
				this.m_CampaignIcon.overrideSprite = null;
			}
		}
		if (this.m_KingdomIcon != null)
		{
			if (flag)
			{
				this.m_KingdomIcon.SetIndex(this.m_SelectedCamaign.campaign.GetCoAIndex());
			}
			this.m_KingdomIcon.gameObject.SetActive(flag);
		}
		if (this.m_CampaignName != null)
		{
			SaveGame.CampaignInfo selectedCamaign = this.m_SelectedCamaign;
			if (((selectedCamaign != null) ? selectedCamaign.campaign : null) != null)
			{
				UIText.SetTextKey(this.m_CampaignName, this.m_SelectedCamaign.campaign.GetNameKey(), this.m_SelectedCamaign.campaign, null);
			}
			else
			{
				UIText.SetTextKey(this.m_CampaignName, "SaveLoadMenuWindow.empty_selection", null, null);
			}
		}
		if (this.m_CampaignCreationDate != null)
		{
			if (flag)
			{
				Vars vars = new Vars();
				Vars vars2 = vars;
				string key = "date";
				string str = "#";
				SaveGame.CampaignInfo selectedCamaign2 = this.m_SelectedCamaign;
				vars2.Set<string>(key, str + ((selectedCamaign2 != null) ? selectedCamaign2.campaign.GetCreationTime().ToString() : null));
				UIText.SetTextKey(this.m_CampaignCreationDate, "SaveLoadMenuWindow.campaignCretaionDate", vars, null);
			}
			this.m_CampaignCreationDate.gameObject.SetActive(flag);
		}
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x001600B0 File Offset: 0x0015E2B0
	private SaveGame.CampaignInfo GetDefaultCampaign()
	{
		SaveGame.CampaignInfo campaignInfo = null;
		SaveGame.CampaignInfo campaignInfo2 = null;
		Game game = GameLogic.Get(true);
		Campaign campaign = (game != null) ? game.campaign : null;
		foreach (SaveGame.CampaignInfo campaignInfo3 in SaveGame.campaigns)
		{
			if (campaignInfo2 == null)
			{
				if (this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer && !campaignInfo3.campaign.IsMultiplayerCampaign())
				{
					campaignInfo2 = campaignInfo3;
				}
				if (this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Multiplayer && campaignInfo3.campaign.IsMultiplayerCampaign())
				{
					campaignInfo2 = campaignInfo3;
				}
			}
			if (campaign != null && (this.m_CurrentFilter != UILoadGameWindow.CampaignFilter.Multiplayer || campaignInfo3.campaign.IsMultiplayerCampaign()) && (this.m_CurrentFilter != UILoadGameWindow.CampaignFilter.Singleplayer || !campaignInfo3.campaign.IsMultiplayerCampaign()) && campaign != null && campaignInfo3.campaign.id == campaign.id)
			{
				campaignInfo = campaignInfo3;
				break;
			}
		}
		if (campaignInfo == null && this.m_CurrentFilter == UILoadGameWindow.CampaignFilter.Singleplayer)
		{
			campaignInfo = SaveGame.latest_single_playr_campaign_info;
		}
		if (campaignInfo == null)
		{
			campaignInfo = campaignInfo2;
		}
		return campaignInfo;
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x001601B8 File Offset: 0x0015E3B8
	public bool HandleOnSaveSelect(UITableRow row, PointerEventData eventData)
	{
		if (this.m_SelectedSaveRow == row)
		{
			return false;
		}
		if (this.m_SelectedSaveRow)
		{
			this.m_SelectedSaveRow.Select(false);
		}
		this.m_SelectedSaveRow = row;
		this.m_SelectedSaveInfo = this.GetSelectedSaveInfo();
		UserInteractionLogger.LogNewLine(this.m_SelectedSaveInfo.name + " - selected.");
		return true;
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x0016021C File Offset: 0x0015E41C
	private void HandleOnLoadButtonClick(BSGButton b)
	{
		if (this.m_Mode == UILoadGameWindow.Mode.Normal)
		{
			this.OnLoadGame();
			return;
		}
		this.OnLoadGameAsCampaign();
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x00160233 File Offset: 0x0015E433
	public bool HandleOnSaveRowDoubleClick(UITableRow row, PointerEventData eventData)
	{
		if (!this.CanLoadSelectedSave())
		{
			return true;
		}
		if (this.m_Mode == UILoadGameWindow.Mode.Normal)
		{
			this.OnLoadGame();
		}
		else
		{
			this.OnLoadGameAsCampaign();
		}
		return true;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x00160258 File Offset: 0x0015E458
	private void OnLoadGameAsCampaign()
	{
		if (this.m_SelectedSaveInfo == null)
		{
			return;
		}
		string id = this.m_SelectedSaveInfo.id;
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.m_SelectedSaveInfo.name))
		{
			return;
		}
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.LoadCampaignFromSave(id);
		}
		Action onSaveLoaded = this.OnSaveLoaded;
		if (onSaveLoaded != null)
		{
			onSaveLoaded();
		}
		this.Close(false);
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x001602C0 File Offset: 0x0015E4C0
	private void OnLoadGame()
	{
		UserInteractionLogger.LogNewLine("Clicked Load Game");
		if (!SaveGame.CanLoad(true))
		{
			return;
		}
		if (this.m_SelectedSaveInfo == null)
		{
			return;
		}
		string id = this.m_SelectedSaveInfo.id;
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		string name = this.m_SelectedSaveInfo.name;
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		Game game = GameLogic.Get(false);
		bool flag = game.IsMultiplayer();
		SaveGame.UpdateList(!flag, !flag);
		SaveGame.Info sg = SaveGame.FindById(id);
		if (sg == null)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("saveName", "#" + name);
		if (GameLogic.Get(true).state != Game.State.Running)
		{
			game.load_game = Game.LoadedGameType.LoadFromMainMenu;
			Game game4 = game;
			if (game4 != null)
			{
				game4.StartGame(false, null, sg.fullPath);
			}
			this.Close(false);
			return;
		}
		MessageWnd messageWnd = MessageWnd.Create("ConfirmSaveLoad", vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "confirm")
			{
				wnd.Close(false);
				this.Close(false);
				UserInteractionLogger.LogNewLine("Confirmed Load");
				game.load_game = Game.LoadedGameType.LoadFromInGameMenu;
				Game game3 = game;
				if (game3 != null)
				{
					game3.StartGame(false, null, sg.fullPath);
				}
				Transform transform2 = this.parent;
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(false);
				}
			}
			else
			{
				UserInteractionLogger.LogNewLine("Canceled Load");
			}
			wnd.Close(false);
			return true;
		});
		if (messageWnd != null)
		{
			messageWnd.transform.SetParent(base.transform);
			return;
		}
		this.Close(false);
		game.load_game = Game.LoadedGameType.LoadFromInGameMenu;
		Game game2 = game;
		if (game2 != null)
		{
			game2.StartGame(false, null, sg.fullPath);
		}
		Transform transform = this.parent;
		if (transform == null)
		{
			return;
		}
		transform.gameObject.SetActive(false);
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x00160437 File Offset: 0x0015E637
	private void HandleSingleplayerCampaign()
	{
		this.SetFilter(UILoadGameWindow.CampaignFilter.Singleplayer);
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x00160440 File Offset: 0x0015E640
	private void HandleMultiplayerCampaign()
	{
		this.SetFilter(UILoadGameWindow.CampaignFilter.Multiplayer);
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x0016044C File Offset: 0x0015E64C
	private void OnDeleteCampaign()
	{
		Vars vars = new Vars(this.m_SelectedCamaign.campaign);
		string name = global::Defs.Localize(this.m_SelectedCamaign.campaign.GetNameKey(), this.m_SelectedCamaign.campaign, null, true, true);
		vars.Set<string>("campaign_name", name);
		MessageWnd.Create(global::Defs.GetDefField("DeleteCampaingSavesMessage", null), vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				this.m_SelectedCamaign.campaign.DeleteSavedGame();
				this.UpdateSaveList(true);
				this.SelectCampaign(this.GetDefaultCampaign(), false);
				Analytics instance = Analytics.instance;
				if (instance != null)
				{
					instance.OnSaveDeleted(name, "delete_campaign");
				}
			}
			wnd.Close(false);
			return true;
		});
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x001604D0 File Offset: 0x0015E6D0
	private void OnDeleteSave()
	{
		UserInteractionLogger.LogNewLine("Clicked Delete Save");
		SaveGame.UpdateList(false, true);
		if (this.m_SelectedSaveRow == null)
		{
			return;
		}
		SaveGame.Info sg = this.GetSelectedSaveInfo();
		if (sg == null)
		{
			return;
		}
		string name = sg.name;
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("saveName", "#" + name);
		MessageWnd messageWnd = MessageWnd.Create("ConfirmSaveDelete", vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			wnd.Close(false);
			if (btn_id == "confirm")
			{
				UserInteractionLogger.LogNewLine("Confirmed Delete");
				this.Delete(sg.fullPath, name);
			}
			UserInteractionLogger.LogNewLine("Canceled Delete");
			return true;
		});
		if (messageWnd != null)
		{
			messageWnd.transform.SetParent(base.transform);
			return;
		}
		this.Delete(sg.fullPath, name);
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x001605AC File Offset: 0x0015E7AC
	private void Delete(string fullPath, string name)
	{
		SaveGame.Delete(fullPath);
		SaveGame.ScanSavesDir(true);
		base.StartCoroutine(this.Refresh(false));
		if (SaveGame.list.Count <= 0)
		{
			TitleUI titleUI = BaseUI.Get<TitleUI>();
			if (titleUI != null)
			{
				BSGButton btnContinue = titleUI.btnContinue;
				if (btnContinue != null)
				{
					btnContinue.Enable(false, false);
				}
			}
			TitleUI titleUI2 = BaseUI.Get<TitleUI>();
			if (titleUI2 != null)
			{
				BSGButton btnLoad = titleUI2.btnLoad;
				if (btnLoad != null)
				{
					btnLoad.Enable(false, false);
				}
			}
		}
		Analytics instance = Analytics.instance;
		if (instance == null)
		{
			return;
		}
		instance.OnSaveDeleted(name, "delete_save");
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x00160630 File Offset: 0x0015E830
	private SaveGame.Info GetSelectedSaveInfo()
	{
		if (this.m_SelectedSaveRow == null)
		{
			return null;
		}
		SaveLoadMenuRow component = this.m_SelectedSaveRow.GetComponent<SaveLoadMenuRow>();
		if (component == null)
		{
			return null;
		}
		return component.info;
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x0016066C File Offset: 0x0015E86C
	public int CompareBySaveName(UITableRow t1, UITableRow t2)
	{
		if (t1 == null || t2 == null)
		{
			return 0;
		}
		SaveLoadMenuRow component = t1.GetComponent<SaveLoadMenuRow>();
		SaveLoadMenuRow component2 = t2.GetComponent<SaveLoadMenuRow>();
		if (component == null || component2 == null)
		{
			return 0;
		}
		return component.info.name.CompareTo(component2.info.name);
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x001606CA File Offset: 0x0015E8CA
	public static void Hide()
	{
		if (UILoadGameWindow.current != null)
		{
			UILoadGameWindow.current.Close(false);
		}
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x001606E4 File Offset: 0x0015E8E4
	public static GameObject GetPrefab(string variant)
	{
		return UICommon.GetPrefab(UILoadGameWindow.def_id, variant);
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x001606F4 File Offset: 0x0015E8F4
	public static UILoadGameWindow Create(Transform parent, bool create_from_load = false)
	{
		UserInteractionLogger.LogNewLine("Create loadmenu");
		if (UILoadGameWindow.current != null)
		{
			if (UILoadGameWindow.current.gameObject.activeSelf)
			{
				return UILoadGameWindow.current;
			}
			UILoadGameWindow.current.GetComponent<UILoadGameWindow>().Close(false);
		}
		Game game = GameLogic.Get(false);
		bool flag = ((game != null) ? game.campaign : null) != null && game.campaign.IsMultiplayerCampaign();
		GameObject prefab = UILoadGameWindow.GetPrefab((!create_from_load && flag) ? "multiplayer" : null);
		if (prefab == null)
		{
			return null;
		}
		UILoadGameWindow orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prefab, parent).GetOrAddComponent<UILoadGameWindow>();
		orAddComponent.parent = parent;
		orAddComponent.Open();
		orAddComponent.SetMode(create_from_load ? UILoadGameWindow.Mode.CampaignFromSave : UILoadGameWindow.Mode.Normal);
		orAddComponent.SetFilter(flag ? UILoadGameWindow.CampaignFilter.Multiplayer : UILoadGameWindow.CampaignFilter.Singleplayer);
		UILoadGameWindow.current = orAddComponent;
		return orAddComponent;
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x001607BD File Offset: 0x0015E9BD
	public static bool IsActive()
	{
		return UILoadGameWindow.current != null;
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x001607CC File Offset: 0x0015E9CC
	public static void ForceRefresh()
	{
		if (UILoadGameWindow.current == null || !UILoadGameWindow.current.gameObject.activeSelf)
		{
			return;
		}
		UILoadGameWindow.current.SelectCampaign(SaveGame.campaigns.Find((SaveGame.CampaignInfo c) => c.campaign.id == UILoadGameWindow.current.m_SelectedCamaign.campaign.id) ?? UILoadGameWindow.current.GetDefaultCampaign(), false);
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x0016083C File Offset: 0x0015EA3C
	public override void Close(bool silent = false)
	{
		if (!base.IsShown())
		{
			return;
		}
		UserInteractionLogger.LogNewLine("Close save menu.");
		GameLogic.Get(true);
		CameraController cameraController = CameraController.Get();
		List<GameCamera> list = (cameraController != null) ? cameraController.AllCameras : null;
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Lock(false);
			}
		}
		base.Close(silent);
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x001608A0 File Offset: 0x0015EAA0
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "can_delete_campaign")
		{
			return this.CanDeleteCampaign(this.m_SelectedCamaign);
		}
		if (!(key == "is_multiplayer"))
		{
			if (key == "can_delete_save")
			{
				return this.CanDeleteSave(this.m_SelectedSaveInfo);
			}
			if (!(key == "can_load_save"))
			{
				return Value.Unknown;
			}
			return this.CanLoadSelectedSave();
		}
		else
		{
			SaveGame.CampaignInfo selectedCamaign = this.m_SelectedCamaign;
			if (((selectedCamaign != null) ? selectedCamaign.campaign : null) != null)
			{
				return this.m_SelectedCamaign.campaign.IsMultiplayerCampaign();
			}
			return false;
		}
	}

	// Token: 0x04001BF2 RID: 7154
	private static string def_id = "LoadMenuWindow";

	// Token: 0x04001BF3 RID: 7155
	private static UILoadGameWindow current = null;

	// Token: 0x04001BF4 RID: 7156
	[UIFieldTarget("id_Menu")]
	private GameObject m_Menu;

	// Token: 0x04001BF5 RID: 7157
	[UIFieldTarget("id_CampaignIcon")]
	private Image m_CampaignIcon;

	// Token: 0x04001BF6 RID: 7158
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001BF7 RID: 7159
	[UIFieldTarget("id_CampaignName")]
	private TextMeshProUGUI m_CampaignName;

	// Token: 0x04001BF8 RID: 7160
	[UIFieldTarget("id_CampaignCreationDate")]
	private TextMeshProUGUI m_CampaignCreationDate;

	// Token: 0x04001BF9 RID: 7161
	[UIFieldTarget("btn_Load")]
	private BSGButton m_btnLoad;

	// Token: 0x04001BFA RID: 7162
	[UIFieldTarget("id_LoadLabel")]
	private TextMeshProUGUI m_LoadLabel;

	// Token: 0x04001BFB RID: 7163
	[UIFieldTarget("btn_Delete")]
	private BSGButton m_btnDelete;

	// Token: 0x04001BFC RID: 7164
	[UIFieldTarget("id_DeleteLabel")]
	private TextMeshProUGUI m_DeleteLabel;

	// Token: 0x04001BFD RID: 7165
	[UIFieldTarget("btn_DeleteCampaign")]
	private BSGButton m_DeleteCampaign;

	// Token: 0x04001BFE RID: 7166
	[UIFieldTarget("id_DeleteCampaignLabel")]
	private TextMeshProUGUI m_DeleteCampaignLabel;

	// Token: 0x04001BFF RID: 7167
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001C00 RID: 7168
	[UIFieldTarget("id_Title_text")]
	private TextMeshProUGUI m_Title;

	// Token: 0x04001C01 RID: 7169
	[UIFieldTarget("id_SaveName_Header")]
	private TextMeshProUGUI m_SaveNameHeader;

	// Token: 0x04001C02 RID: 7170
	[UIFieldTarget("id_Playime_Header")]
	private TextMeshProUGUI m_PlayimeHeader;

	// Token: 0x04001C03 RID: 7171
	[UIFieldTarget("id_SaveDate_Header")]
	private TextMeshProUGUI m_SaveDateHeader;

	// Token: 0x04001C04 RID: 7172
	[UIFieldTarget("Content")]
	private GameObject TableContent;

	// Token: 0x04001C05 RID: 7173
	[UIFieldTarget("id_TableGrid")]
	private UITable TableContainer;

	// Token: 0x04001C06 RID: 7174
	[UIFieldTarget("id_TabSingleplayerBackground")]
	private GameObject m_TabSingleplayerBackground;

	// Token: 0x04001C07 RID: 7175
	[UIFieldTarget("id_TabMultiplayerBackground")]
	private GameObject m_TabMultiplayerBackground;

	// Token: 0x04001C08 RID: 7176
	[UIFieldTarget("id_CampaignsLabel")]
	private TextMeshProUGUI m_CampaignsLabel;

	// Token: 0x04001C09 RID: 7177
	[UIFieldTarget("id_ToggleSingleplayerSaves")]
	private BSGButton m_ToggleSingleplayerSaves;

	// Token: 0x04001C0A RID: 7178
	[UIFieldTarget("id_ToggleSingleplayerSavesLabel")]
	private TextMeshProUGUI m_ToggleSingleplayerSavesLabel;

	// Token: 0x04001C0B RID: 7179
	[UIFieldTarget("id_ToggleMultiplayerSaves")]
	private BSGButton m_ToggleMultiplayerSaves;

	// Token: 0x04001C0C RID: 7180
	[UIFieldTarget("id_ToggleMultiplayerSavesLabel")]
	private TextMeshProUGUI m_ToggleMultiplayerSavesLabel;

	// Token: 0x04001C0D RID: 7181
	[UIFieldTarget("id_CampaignsContainer")]
	private RectTransform m_CampaignsContainer;

	// Token: 0x04001C0E RID: 7182
	[UIFieldTarget("id_CampaignLabelPrototype")]
	private GameObject m_CampaignLabelPrototype;

	// Token: 0x04001C0F RID: 7183
	[UIFieldTarget("id_PointerBlocker")]
	private GameObject m_PointerBlocker;

	// Token: 0x04001C10 RID: 7184
	private Transform parent;

	// Token: 0x04001C11 RID: 7185
	public GameObject tableRowPrefab;

	// Token: 0x04001C12 RID: 7186
	public Action OnSaveLoaded;

	// Token: 0x04001C13 RID: 7187
	private UILoadGameWindow.CampaignFilter m_CurrentFilter;

	// Token: 0x04001C14 RID: 7188
	private UITableRow m_SelectedSaveRow;

	// Token: 0x04001C15 RID: 7189
	private SaveGame.CampaignInfo m_SelectedCamaign;

	// Token: 0x04001C16 RID: 7190
	private SaveGame.Info m_SelectedSaveInfo;

	// Token: 0x04001C17 RID: 7191
	private List<SaveLoadMenuRow> m_SaveRows = new List<SaveLoadMenuRow>();

	// Token: 0x04001C18 RID: 7192
	private List<UILoadGameWindow.CampaignLabel> m_CampaingLabels = new List<UILoadGameWindow.CampaignLabel>();

	// Token: 0x04001C19 RID: 7193
	private UILoadGameWindow.Mode m_Mode;

	// Token: 0x04001C1A RID: 7194
	private Vars vars;

	// Token: 0x020007F1 RID: 2033
	private enum CampaignFilter
	{
		// Token: 0x04003D10 RID: 15632
		Singleplayer,
		// Token: 0x04003D11 RID: 15633
		Multiplayer
	}

	// Token: 0x020007F2 RID: 2034
	private enum Mode
	{
		// Token: 0x04003D13 RID: 15635
		Normal,
		// Token: 0x04003D14 RID: 15636
		CampaignFromSave
	}

	// Token: 0x020007F3 RID: 2035
	internal class CampaignLabel : Hotspot
	{
		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06004F17 RID: 20247 RVA: 0x00233FAE File Offset: 0x002321AE
		// (set) Token: 0x06004F18 RID: 20248 RVA: 0x00233FB6 File Offset: 0x002321B6
		public bool Selected { get; private set; }

		// Token: 0x06004F19 RID: 20249 RVA: 0x00233FBF File Offset: 0x002321BF
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initialzied = true;
		}

		// Token: 0x06004F1A RID: 20250 RVA: 0x00233FD8 File Offset: 0x002321D8
		public void Select(bool selected)
		{
			this.Selected = selected;
			this.UpdateHighlight();
		}

		// Token: 0x06004F1B RID: 20251 RVA: 0x00233FE7 File Offset: 0x002321E7
		public void SetData(SaveGame.CampaignInfo info)
		{
			this.Init();
			this.Data = info;
			this.Refresh();
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x00233FFC File Offset: 0x002321FC
		private void Refresh()
		{
			if (this.m_CampaingName != null)
			{
				UIText.SetTextKey(this.m_CampaingName, this.Data.campaign.GetNameKey(), this.Data.campaign, null);
			}
			if (this.m_CampaingKIngdomIcon != null)
			{
				this.m_CampaingKIngdomIcon.SetIndex(this.Data.campaign.GetCoAIndex());
			}
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x00234067 File Offset: 0x00232267
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			Action<UILoadGameWindow.CampaignLabel> onSelect = this.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x00234081 File Offset: 0x00232281
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004F1F RID: 20255 RVA: 0x00234090 File Offset: 0x00232290
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004F20 RID: 20256 RVA: 0x002340A0 File Offset: 0x002322A0
		public void UpdateHighlight()
		{
			if (this.m_Normal != null)
			{
				this.m_Normal.gameObject.SetActive(!this.Selected && !this.mouse_in);
			}
			if (this.m_Highlight != null)
			{
				this.m_Highlight.gameObject.SetActive(!this.Selected && this.mouse_in);
			}
			if (this.m_Selected != null)
			{
				this.m_Selected.gameObject.SetActive(this.Selected);
			}
		}

		// Token: 0x06004F21 RID: 20257 RVA: 0x00234134 File Offset: 0x00232334
		public static UILoadGameWindow.CampaignLabel Create(SaveGame.CampaignInfo info, GameObject prototype, Transform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
			UILoadGameWindow.CampaignLabel orAddComponent = gameObject.GetOrAddComponent<UILoadGameWindow.CampaignLabel>();
			orAddComponent.SetData(info);
			gameObject.SetActive(true);
			return orAddComponent;
		}

		// Token: 0x04003D15 RID: 15637
		[UIFieldTarget("id_CampaingKIngdomIcon")]
		private UIKingdomIcon m_CampaingKIngdomIcon;

		// Token: 0x04003D16 RID: 15638
		[UIFieldTarget("id_CampaingName")]
		private TextMeshProUGUI m_CampaingName;

		// Token: 0x04003D17 RID: 15639
		[UIFieldTarget("id_Normal")]
		private GameObject m_Normal;

		// Token: 0x04003D18 RID: 15640
		[UIFieldTarget("id_Highlight")]
		private GameObject m_Highlight;

		// Token: 0x04003D19 RID: 15641
		[UIFieldTarget("id_Selected")]
		private GameObject m_Selected;

		// Token: 0x04003D1A RID: 15642
		public SaveGame.CampaignInfo Data;

		// Token: 0x04003D1C RID: 15644
		public Action<UILoadGameWindow.CampaignLabel> OnSelect;

		// Token: 0x04003D1D RID: 15645
		private bool m_Initialzied;
	}
}
