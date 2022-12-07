using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200029E RID: 670
public class UISaveGameWindow : UIWindow, IVars
{
	// Token: 0x06002977 RID: 10615 RVA: 0x00160999 File Offset: 0x0015EB99
	public override string GetDefId()
	{
		return UISaveGameWindow.def_id;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x001609A0 File Offset: 0x0015EBA0
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		Vars vars = new Vars(this);
		if (this.m_btnSave != null)
		{
			this.m_btnSave.onClick = delegate(BSGButton b)
			{
				this.OnSaveGame();
			};
			Tooltip.Get(this.m_btnSave.gameObject, true).SetDef("SaveGameTooltip", vars);
		}
		if (this.m_btnDelete != null)
		{
			this.m_btnDelete.onClick = delegate(BSGButton b)
			{
				this.OnDeleteSave();
			};
			Tooltip.Get(this.m_btnDelete.gameObject, true).SetDef("DeleteGameSaveTooltip", vars);
		}
		if (this.m_Close != null)
		{
			this.m_Close.onClick = delegate(BSGButton b)
			{
				this.Close(false);
			};
		}
		if (this.m_inputNameFieldText != null)
		{
			TMP_InputField inputNameFieldText = this.m_inputNameFieldText;
			inputNameFieldText.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(inputNameFieldText.onValidateInput, new TMP_InputField.OnValidateInput((string input, int charIndex, char addedChar) => this.ValidateSaveNameInput(addedChar)));
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x00160AA7 File Offset: 0x0015ECA7
	public override void Show()
	{
		this.Init();
		this.LocalizeStatics();
		this.InitTable();
		base.Show();
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x00160AC4 File Offset: 0x0015ECC4
	private void LocalizeStatics()
	{
		if (this.id_Title_text != null)
		{
			UIText.SetTextKey(this.id_Title_text, "SaveLoadMenuWindow.titleSave", null, null);
		}
		if (this.id_Save_text != null)
		{
			UIText.SetTextKey(this.id_Save_text, "SaveLoadMenuWindow.btnSave", null, null);
		}
		if (this.id_Delete_text != null)
		{
			UIText.SetTextKey(this.id_Delete_text, "SaveLoadMenuWindow.btnDelete", null, null);
		}
		if (this.m_SaveGameLabel != null)
		{
			UIText.SetTextKey(this.m_SaveGameLabel, "SaveLoadMenuWindow.label_savegame", null, null);
		}
		TextMeshProUGUI component = this.m_inputNameFieldText.placeholder.GetComponent<TextMeshProUGUI>();
		if (component != null)
		{
			UIText.SetTextKey(component, "SaveLoadMenuWindow.newSaveName", null, null);
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
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x00160BD8 File Offset: 0x0015EDD8
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
		base.StartCoroutine(this.Refresh());
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x00160C4C File Offset: 0x0015EE4C
	public IEnumerator Refresh()
	{
		if (this.selected != null)
		{
			this.selected.Select(false);
		}
		this.selected = null;
		if (this.TableContainer == null)
		{
			yield break;
		}
		if (this.tableRowPrefab != null && this.TableContent != null && this.TableContainer != null)
		{
			Game game = GameLogic.Get(false);
			bool singleplayer_saves_only = game == null || !game.IsMultiplayer();
			bool list_all = false;
			SaveGame.UpdateList(singleplayer_saves_only, list_all);
			List<SaveGame.Info> list = SaveGame.list;
			bool flag = this.selected == null && this.selectedSaveInfo != null;
			this.TableContainer.Clear();
			if (list.Count != this.rows.Count)
			{
				for (int i = 0; i < this.rows.Count; i++)
				{
					UnityEngine.Object.Destroy(this.rows[i].gameObject);
					this.rows.RemoveAt(i);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				SaveLoadMenuRow saveLoadMenuRow;
				if (this.rows.Count <= j)
				{
					saveLoadMenuRow = global::Common.Spawn(this.tableRowPrefab, this.TableContent.transform, false, "").GetComponent<SaveLoadMenuRow>();
					this.rows.Add(saveLoadMenuRow);
				}
				else
				{
					saveLoadMenuRow = this.rows[j];
				}
				if (saveLoadMenuRow != null)
				{
					UITableRow component = saveLoadMenuRow.GetComponent<UITableRow>();
					if (component != null)
					{
						UITableRow uitableRow = component;
						uitableRow.OnSelected = (UITableRow.OnSelectedEvent)Delegate.Combine(uitableRow.OnSelected, new UITableRow.OnSelectedEvent(this.Select));
						UITableRow uitableRow2 = component;
						uitableRow2.OnFocus = (UITableRow.OnDoubleClickEvent)Delegate.Combine(uitableRow2.OnFocus, new UITableRow.OnDoubleClickEvent(this.DoubleClick));
						this.TableContainer.AddRow(component);
					}
					SaveGame.Info info = list[j];
					saveLoadMenuRow.SetInfo(info);
					saveLoadMenuRow.Refresh();
					if (flag && this.selectedSaveInfo == info)
					{
						this.selected = component;
						this.selected.Select(true);
						flag = false;
					}
				}
			}
			yield return null;
			yield return null;
			foreach (SaveLoadMenuRow saveLoadMenuRow2 in this.rows)
			{
				saveLoadMenuRow2.RefreshHeight();
			}
		}
		if (this.TableContainer)
		{
			this.TableContainer.Refresh(true);
		}
		this.UpdateCampaignData();
		yield break;
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x00160C5B File Offset: 0x0015EE5B
	protected override void Update()
	{
		base.Update();
		this.UpdateButtonState();
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x00160C6C File Offset: 0x0015EE6C
	private void UpdateButtonState()
	{
		if (this.m_btnDelete)
		{
			SaveGame.Info info = this.GetSelectedSaveInfo();
			if (info != null)
			{
				this.m_btnDelete.Enable(this.CanDeleteSave(info), false);
				return;
			}
			this.m_btnDelete.Enable(false, false);
		}
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x00160CB4 File Offset: 0x0015EEB4
	private bool CanDeleteSave(SaveGame.Info saveInfo)
	{
		if (saveInfo == null)
		{
			return false;
		}
		if (GameLogic.Get(true) == null)
		{
			return false;
		}
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(saveInfo.campaign_id);
		return campaignInfo != null && campaignInfo.latest_save != saveInfo;
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x00160CF0 File Offset: 0x0015EEF0
	private void UpdateCampaignData()
	{
		Game game = GameLogic.Get(true);
		Campaign campaign = (game != null) ? game.campaign : null;
		if (this.m_CampaignIcon != null)
		{
			DT.Field varDef = campaign.GetVarDef("main_goal");
			DT.Field selectedOption = CampaignUtils.GetSelectedOption(campaign, varDef);
			this.m_CampaignIcon.overrideSprite = global::Defs.GetObj<Sprite>(selectedOption, "icon", null);
		}
		if (this.m_KingdomIcon != null)
		{
			this.m_KingdomIcon.SetIndex(campaign.GetCoAIndex());
		}
		if (this.m_CampaignName != null)
		{
			string text = (campaign != null) ? campaign.GetCustomNameLocalized() : null;
			if (string.IsNullOrEmpty(text))
			{
				text = ((campaign != null) ? campaign.GetNameKey() : null);
			}
			UIText.SetTextKey(this.m_CampaignName, text, campaign, null);
		}
		if (this.m_CampaignCreationDate != null)
		{
			Vars vars = new Vars();
			vars.Set<string>("date", "#" + ((campaign != null) ? campaign.GetCreationTime().ToString() : null));
			UIText.SetTextKey(this.m_CampaignCreationDate, "SaveLoadMenuWindow.campaignCretaionDate", vars, null);
		}
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x00160DFC File Offset: 0x0015EFFC
	public bool Select(UITableRow row, PointerEventData eventData)
	{
		if (this.selected == row)
		{
			return false;
		}
		if (this.selected)
		{
			this.selected.Select(false);
		}
		this.selected = row;
		this.selectedSaveInfo = this.GetSelectedSaveInfo();
		this.m_inputNameFieldText.text = this.selectedSaveInfo.name;
		UserInteractionLogger.LogNewLine(this.selectedSaveInfo.name + " - selected.");
		return true;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x00160E76 File Offset: 0x0015F076
	public bool DoubleClick(UITableRow row, PointerEventData eventData)
	{
		this.OnSaveGame();
		return true;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x00160E80 File Offset: 0x0015F080
	private void OnSaveGame()
	{
		Game game = GameLogic.Get(true);
		Campaign campaign = (game != null) ? game.campaign : null;
		UserInteractionLogger.LogNewLine("Clicked Save Game");
		SaveGame.UpdateList(!campaign.IsMultiplayerCampaign(), false);
		string name = null;
		name = this.m_inputNameFieldText.text;
		if (string.IsNullOrEmpty(name))
		{
			name = this.m_inputNameFieldText.placeholder.GetComponent<TextMeshProUGUI>().text;
		}
		SaveGame.Info si = SaveGame.FindByName(name);
		if (si == null)
		{
			SaveGame.Save(Path.Combine(campaign.Dir(), name), name, -1, -1, null);
			this.Close(false);
			return;
		}
		string value = campaign.Dir();
		if (!si.fullPath.Contains(value))
		{
			Debug.LogError("Overriding save file from another campaign! Path: " + si.fullPath);
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("saveName", "#" + name);
		MessageWnd messageWnd = MessageWnd.Create("ConfirmSaveOverwrite", vars, null, delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "confirm")
			{
				UserInteractionLogger.LogNewLine("Confirmed Save");
				SaveGame.Save(si.fullPath, name, -1, -1, null);
				this.Close(false);
			}
			UserInteractionLogger.LogNewLine("Canceled Save");
			wnd.Close(false);
			return true;
		});
		if (messageWnd != null)
		{
			messageWnd.transform.SetParent(base.transform);
			return;
		}
		SaveGame.Save(si.fullPath, name, -1, -1, null);
		this.Close(false);
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x00160FF8 File Offset: 0x0015F1F8
	private void OnDeleteSave()
	{
		UserInteractionLogger.LogNewLine("Clicked Delete Save");
		SaveGame.UpdateList(false, true);
		if (this.selected == null)
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
				this.Delete(sg.fullPath, sg.name);
			}
			UserInteractionLogger.LogNewLine("Canceled Delete");
			return true;
		});
		if (messageWnd != null)
		{
			messageWnd.transform.SetParent(base.transform);
			return;
		}
		this.Delete(sg.fullPath, sg.name);
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x001610C8 File Offset: 0x0015F2C8
	private void Delete(string fullPath, string name)
	{
		SaveGame.Delete(fullPath);
		this.m_inputNameFieldText.text = "";
		base.StartCoroutine(this.Refresh());
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

	// Token: 0x06002986 RID: 10630 RVA: 0x00161154 File Offset: 0x0015F354
	private SaveGame.Info GetSelectedSaveInfo()
	{
		if (this.selected == null)
		{
			return null;
		}
		SaveLoadMenuRow component = this.selected.GetComponent<SaveLoadMenuRow>();
		if (component == null)
		{
			return null;
		}
		return component.info;
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x00161190 File Offset: 0x0015F390
	private char ValidateSaveNameInput(char charToValidate)
	{
		if (charToValidate == '.')
		{
			return '\0';
		}
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		int num = invalidFileNameChars.Length;
		for (int i = 0; i < num; i++)
		{
			if (invalidFileNameChars[i] == charToValidate)
			{
				return '\0';
			}
		}
		return charToValidate;
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x001611C4 File Offset: 0x0015F3C4
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

	// Token: 0x06002989 RID: 10633 RVA: 0x00161224 File Offset: 0x0015F424
	public override void Close(bool silent = false)
	{
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

	// Token: 0x0600298A RID: 10634 RVA: 0x0016127C File Offset: 0x0015F47C
	public static void Hide()
	{
		if (UISaveGameWindow.current != null)
		{
			UISaveGameWindow.current.Close(false);
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x00161298 File Offset: 0x0015F498
	public static UISaveGameWindow Create(Transform parent)
	{
		UserInteractionLogger.LogNewLine("Create savemenu");
		if (UISaveGameWindow.current != null)
		{
			if (UISaveGameWindow.current.gameObject.activeSelf)
			{
				return UISaveGameWindow.current;
			}
			UISaveGameWindow.current.GetComponent<UISaveGameWindow>().Close(false);
		}
		GameObject prefab = UICommon.GetPrefab("SaveMenuWindow", null);
		if (prefab == null)
		{
			return null;
		}
		UISaveGameWindow orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prefab, parent).GetOrAddComponent<UISaveGameWindow>();
		orAddComponent.Open();
		UISaveGameWindow.current = orAddComponent;
		return orAddComponent;
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x00161312 File Offset: 0x0015F512
	public static bool IsActive()
	{
		return UISaveGameWindow.current != null;
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x0016131F File Offset: 0x0015F51F
	public static void ForceRefresh()
	{
		if (UISaveGameWindow.current == null || !UISaveGameWindow.current.gameObject.activeSelf)
		{
			return;
		}
		UISaveGameWindow.current.StartCoroutine(UISaveGameWindow.current.Refresh());
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x00161358 File Offset: 0x0015F558
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (!(key == "is_multiplayer"))
		{
			if (!(key == "can_delete_save"))
			{
				return Value.Unknown;
			}
			SaveGame.Info info = this.GetSelectedSaveInfo();
			if (info != null)
			{
				return this.CanDeleteSave(info);
			}
			return false;
		}
		else
		{
			Game game = GameLogic.Get(true);
			Campaign campaign = (game != null) ? game.campaign : null;
			if (campaign == null)
			{
				return false;
			}
			return campaign.IsMultiplayerCampaign();
		}
	}

	// Token: 0x04001C1B RID: 7195
	private static string def_id = "SaveMenuWindow";

	// Token: 0x04001C1C RID: 7196
	[UIFieldTarget("id_CampaignIcon")]
	private Image m_CampaignIcon;

	// Token: 0x04001C1D RID: 7197
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001C1E RID: 7198
	[UIFieldTarget("id_CampaignName")]
	private TextMeshProUGUI m_CampaignName;

	// Token: 0x04001C1F RID: 7199
	[UIFieldTarget("id_CampaignCreationDate")]
	private TextMeshProUGUI m_CampaignCreationDate;

	// Token: 0x04001C20 RID: 7200
	[UIFieldTarget("btn_Save")]
	private BSGButton m_btnSave;

	// Token: 0x04001C21 RID: 7201
	[UIFieldTarget("btn_Delete")]
	private BSGButton m_btnDelete;

	// Token: 0x04001C22 RID: 7202
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001C23 RID: 7203
	[UIFieldTarget("InputNameFieldText")]
	private TMP_InputField m_inputNameFieldText;

	// Token: 0x04001C24 RID: 7204
	[UIFieldTarget("id_Save_text")]
	private TextMeshProUGUI id_Save_text;

	// Token: 0x04001C25 RID: 7205
	[UIFieldTarget("id_Delete_text")]
	private TextMeshProUGUI id_Delete_text;

	// Token: 0x04001C26 RID: 7206
	[UIFieldTarget("id_Title_text")]
	private TextMeshProUGUI id_Title_text;

	// Token: 0x04001C27 RID: 7207
	[UIFieldTarget("id_SaveName_Header")]
	private TextMeshProUGUI m_SaveNameHeader;

	// Token: 0x04001C28 RID: 7208
	[UIFieldTarget("id_Playime_Header")]
	private TextMeshProUGUI m_PlayimeHeader;

	// Token: 0x04001C29 RID: 7209
	[UIFieldTarget("id_SaveDate_Header")]
	private TextMeshProUGUI m_SaveDateHeader;

	// Token: 0x04001C2A RID: 7210
	[UIFieldTarget("Content")]
	private GameObject TableContent;

	// Token: 0x04001C2B RID: 7211
	[UIFieldTarget("id_TableGrid")]
	private UITable TableContainer;

	// Token: 0x04001C2C RID: 7212
	[UIFieldTarget("id_SaveGameLabel")]
	private TextMeshProUGUI m_SaveGameLabel;

	// Token: 0x04001C2D RID: 7213
	public GameObject tableRowPrefab;

	// Token: 0x04001C2E RID: 7214
	private UITableRow selected;

	// Token: 0x04001C2F RID: 7215
	private SaveGame.Info selectedSaveInfo;

	// Token: 0x04001C30 RID: 7216
	private List<SaveLoadMenuRow> rows = new List<SaveLoadMenuRow>();

	// Token: 0x04001C31 RID: 7217
	private static UISaveGameWindow current = null;
}
