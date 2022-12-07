using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class InGameMenu : MonoBehaviour
{
	// Token: 0x06001FB5 RID: 8117 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x00124EFC File Offset: 0x001230FC
	private void OnEnable()
	{
		GameLogic.Get(true).pause.AddRequest("MenuPause", -2);
		this.btnResume = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_Resume");
		this.btnResume.onClick = new BSGButton.OnClick(this.OnResume);
		this.lblResume = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_ResumeLabel");
		UIText.SetTextKey(this.lblResume, "Menu.resume", null, null);
		this.btnQuit = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_Quit");
		this.btnQuit.onClick = new BSGButton.OnClick(this.OnQuitToTitle);
		this.lblQuit = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_QuitLabel");
		UIText.SetTextKey(this.lblQuit, "Menu.return_to_main_game", null, null);
		this.btnSave = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_Save");
		this.btnSave.onClick = new BSGButton.OnClick(this.OnSaveGame);
		this.lblSave = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_SaveLabel");
		UIText.SetTextKey(this.lblSave, "Menu.save_game", null, null);
		this.btnLoad = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_Load");
		this.btnLoad.onClick = new BSGButton.OnClick(this.OnLoadGame);
		this.lblLoad = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_LoadLabel");
		UIText.SetTextKey(this.lblLoad, "Menu.load_game", null, null);
		this.btnPreferences = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_Preferences");
		this.btnPreferences.onClick = new BSGButton.OnClick(this.OnPreferences);
		this.lblPreferences = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_PreferencesLabel");
		UIText.SetTextKey(this.lblPreferences, "Menu.preferences", null, null);
		this.btnRetunToWorldView = global::Common.FindChildComponent<BSGButton>(base.gameObject, "btn_ToWorldView");
		this.btnRetunToWorldView.onClick = new BSGButton.OnClick(this.OnReturnToWorldView);
		this.lblRetunToWorldView = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_ToWorldViewLabel");
		UIText.SetTextKey(this.lblRetunToWorldView, "Menu.return_to_world_view", null, null);
		this.mainFrame = global::Common.FindChildByName(base.gameObject, "id_SelectOptions", true, false);
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Preferences", true, false);
		this.preferences = ((gameObject != null) ? gameObject.GetComponent<UIPreferences>() : null);
		this.RebuildLayot();
		InGameMenu.current = this;
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x00125166 File Offset: 0x00123366
	private void OnDisable()
	{
		GameLogic.Get(true).pause.DelRequest("MenuPause", -2);
		InGameMenu.current = null;
		this.ResetLayout();
		UserSettings.OnUIClosed();
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x00125190 File Offset: 0x00123390
	private void RebuildLayot()
	{
		bool flag = BattleViewLoader.CurrentBattleView.IsValid() && BattleViewLoader.CurrentBattleView.isLoaded;
		flag |= (BattleMap.battle != null && BattleMap.battle.battle_map_only);
		this.btnRetunToWorldView.gameObject.SetActive(false);
		this.btnQuit.gameObject.SetActive(!flag);
		string val;
		this.btnSave.Enable(SaveGame.CanSave(out val, false), false);
		Vars vars = new Vars();
		vars.Set<string>("reason", val);
		Tooltip.Get(this.btnSave.gameObject, true).SetDef("MenuSaveGameTooltip", vars);
		string val2;
		this.btnLoad.Enable(SaveGame.CanLoad(out val2, true), false);
		Vars vars2 = new Vars();
		vars2.Set<string>("reason", val2);
		Tooltip.Get(this.btnLoad.gameObject, true).SetDef("MenuLoadGameTooltip", vars2);
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x0012527C File Offset: 0x0012347C
	public void ResetLayout()
	{
		if (InGameMenu.current != null && this.mainFrame != null)
		{
			this.mainFrame.SetActive(true);
		}
		if (this.preferences != null && this.preferences.IsShown())
		{
			this.preferences.Close(false);
		}
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x001252D8 File Offset: 0x001234D8
	private void OnResume(BSGButton btn)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI)
		{
			baseUI.OnMenu();
		}
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x001252F9 File Offset: 0x001234F9
	public void OnPreferencesClosed()
	{
		this.currentOpenPanel = null;
		if (InGameMenu.current != null && this.mainFrame != null)
		{
			this.mainFrame.SetActive(true);
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x0012532C File Offset: 0x0012352C
	public void OnPreferences(BSGButton btn)
	{
		if (this.preferences != null)
		{
			if (this.preferences.IsShown())
			{
				this.preferences.Close(false);
			}
			else
			{
				this.preferences.Open();
				this.currentOpenPanel = this.preferences.gameObject;
			}
		}
		if (this.mainFrame != null)
		{
			this.mainFrame.SetActive(false);
		}
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x00125398 File Offset: 0x00123598
	private void OnSaveGame(BSGButton btn)
	{
		UISaveGameWindow uisaveGameWindow = UISaveGameWindow.Create(base.transform as RectTransform);
		if (uisaveGameWindow != null)
		{
			this.currentOpenPanel = uisaveGameWindow.gameObject;
			UISaveGameWindow uisaveGameWindow2 = uisaveGameWindow;
			uisaveGameWindow2.on_close = (UIWindow.OnClose)Delegate.Combine(uisaveGameWindow2.on_close, new UIWindow.OnClose(delegate(UIWindow wnd)
			{
				this.ResetLayout();
			}));
			if (this.mainFrame != null)
			{
				this.mainFrame.SetActive(false);
			}
		}
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x00125408 File Offset: 0x00123608
	private void OnLoadGame(BSGButton btn)
	{
		UILoadGameWindow uiloadGameWindow = UILoadGameWindow.Create(base.GetComponent<RectTransform>(), false);
		if (uiloadGameWindow != null)
		{
			this.currentOpenPanel = uiloadGameWindow.gameObject;
			UILoadGameWindow uiloadGameWindow2 = uiloadGameWindow;
			uiloadGameWindow2.on_close = (UIWindow.OnClose)Delegate.Combine(uiloadGameWindow2.on_close, new UIWindow.OnClose(delegate(UIWindow wnd)
			{
				this.ResetLayout();
			}));
			if (this.mainFrame != null)
			{
				this.mainFrame.SetActive(false);
			}
		}
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x00125474 File Offset: 0x00123674
	private void OnReturnToWorldView(BSGButton btn)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.OnMenu();
		}
		if (BattleViewLoader.WorldView.IsValid() && BattleViewLoader.WorldView.isLoaded)
		{
			MessageWnd.Create(global::Defs.GetDefField("BattleViewLeaveMessage", null), new Vars(BattleMap.battle), null, new MessageWnd.OnButton(this.OnLeaveMessage));
			return;
		}
		GameLogic.LoadScene("title");
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x001254E4 File Offset: 0x001236E4
	public virtual bool OnLeaveMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			Logic.Battle battle = BattleMap.battle;
			if (battle != null)
			{
				int kingdomId = BattleMap.KingdomId;
				int side = -1;
				if (kingdomId == battle.attacker_kingdom.id)
				{
					side = 0;
				}
				else if (kingdomId == battle.defender_kingdom.id)
				{
					side = 1;
				}
				battle.DoAction("leave_battle", side, "");
			}
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x0012554A File Offset: 0x0012374A
	private void OnQuitToTitle(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, "Quit To Main Menu");
		GameLogic.QuitToTitle();
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x0012555C File Offset: 0x0012375C
	internal void HandleEscapeAction()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (this.currentOpenPanel != null)
			{
				UISaveGameWindow component = this.currentOpenPanel.GetComponent<UISaveGameWindow>();
				if (component != null)
				{
					this.currentOpenPanel = null;
					component.Close(false);
					if (this.mainFrame != null)
					{
						this.mainFrame.gameObject.SetActive(true);
					}
					return;
				}
				UILoadGameWindow component2 = this.currentOpenPanel.GetComponent<UILoadGameWindow>();
				if (component2 != null)
				{
					this.currentOpenPanel = null;
					component2.Close(false);
					if (this.mainFrame != null)
					{
						this.mainFrame.gameObject.SetActive(true);
					}
					return;
				}
				UIPreferences component3 = this.currentOpenPanel.GetComponent<UIPreferences>();
				if (component3 != null)
				{
					component3.Close(false);
					if (this.mainFrame != null)
					{
						this.mainFrame.gameObject.SetActive(true);
					}
					return;
				}
			}
			base.gameObject.SetActive(false);
			return;
		}
		this.currentOpenPanel = null;
		base.gameObject.SetActive(true);
		base.gameObject.transform.SetAsLastSibling();
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		GameObject system_message_container = baseUI.system_message_container;
		if (system_message_container == null)
		{
			return;
		}
		system_message_container.transform.SetAsLastSibling();
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x00125697 File Offset: 0x00123897
	public static void ForceRefresh()
	{
		if (InGameMenu.current == null || !InGameMenu.current.gameObject.activeSelf)
		{
			return;
		}
		InGameMenu.current.RebuildLayot();
	}

	// Token: 0x04001502 RID: 5378
	public static InGameMenu current;

	// Token: 0x04001503 RID: 5379
	private BSGButton btnResume;

	// Token: 0x04001504 RID: 5380
	private BSGButton btnQuit;

	// Token: 0x04001505 RID: 5381
	private BSGButton btnSave;

	// Token: 0x04001506 RID: 5382
	private BSGButton btnLoad;

	// Token: 0x04001507 RID: 5383
	private BSGButton btnPreferences;

	// Token: 0x04001508 RID: 5384
	private BSGButton btnRetunToWorldView;

	// Token: 0x04001509 RID: 5385
	private TextMeshProUGUI lblResume;

	// Token: 0x0400150A RID: 5386
	private TextMeshProUGUI lblQuit;

	// Token: 0x0400150B RID: 5387
	private TextMeshProUGUI lblSave;

	// Token: 0x0400150C RID: 5388
	private TextMeshProUGUI lblLoad;

	// Token: 0x0400150D RID: 5389
	private TextMeshProUGUI lblPreferences;

	// Token: 0x0400150E RID: 5390
	private TextMeshProUGUI lblRetunToWorldView;

	// Token: 0x0400150F RID: 5391
	private UIPreferences preferences;

	// Token: 0x04001510 RID: 5392
	private GameObject mainFrame;

	// Token: 0x04001511 RID: 5393
	private GameObject currentOpenPanel;
}
