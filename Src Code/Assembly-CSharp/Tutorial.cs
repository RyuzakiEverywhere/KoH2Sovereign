using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public static class Tutorial
{
	// Token: 0x06002ECE RID: 11982 RVA: 0x00181D64 File Offset: 0x0017FF64
	public static void LoadDefs()
	{
		Tutorial.LoadHotspotDefs();
		Tutorial.LoadMessageDefs();
		Tutorial.Rule.LoadDefs();
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x00181D75 File Offset: 0x0017FF75
	public static void Save(DT.Field f)
	{
		Tutorial.SaveSeenHotspots(f);
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x00181D7D File Offset: 0x0017FF7D
	public static void Load(DT.Field f)
	{
		Tutorial.LoadSeenHotspots(f);
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06002ED1 RID: 11985 RVA: 0x00181D85 File Offset: 0x0017FF85
	public static bool active
	{
		get
		{
			return Tutorial.enabled && (Tutorial.alt_held || Tutorial.force_active) && !Tutorial.suppres_tutorials;
		}
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x00181DA8 File Offset: 0x0017FFA8
	public static Tutorial.HotspotDef FindHotspotDef(string id)
	{
		for (int i = 0; i < Tutorial.hotspot_defs.Count; i++)
		{
			Tutorial.HotspotDef hotspotDef = Tutorial.hotspot_defs[i];
			if (hotspotDef.id == id)
			{
				return hotspotDef;
			}
		}
		return null;
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x00181DE8 File Offset: 0x0017FFE8
	public static Tutorial.HotspotDef ResolveHotspotDef(string path, string tt_def = null, IVars match_vars = null)
	{
		for (int i = 0; i < Tutorial.hotspot_defs.Count; i++)
		{
			Tutorial.HotspotDef hotspotDef = Tutorial.hotspot_defs[i];
			if (hotspotDef.field != null && Game.Match(path, hotspotDef.path_mask, false, '*') && Game.Match(tt_def, hotspotDef.tooltip_def_mask, false, '*'))
			{
				if (hotspotDef.match_field != null)
				{
					Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
					bool flag = hotspotDef.match_field.Bool(match_vars, true);
					Vars.PopReflectionMode(old_mode);
					if (!flag)
					{
						goto IL_60;
					}
				}
				return hotspotDef;
			}
			IL_60:;
		}
		return null;
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x00181E68 File Offset: 0x00180068
	public static Tutorial.HotspotDef ResolveHotspotDef(Tooltip hs)
	{
		if (hs.is_tutorial_tooltip)
		{
			return hs.tutorial_hotspot_def;
		}
		if (string.IsNullOrEmpty(hs.TutorialHotspotDef))
		{
			hs.path = global::Common.ObjPath(hs.gameObject);
			string text;
			if (hs == null)
			{
				text = null;
			}
			else
			{
				DT.Def def = hs.def;
				text = ((def != null) ? def.path : null);
			}
			string tt_def = text;
			return Tutorial.ResolveHotspotDef(hs.path, tt_def, hs);
		}
		Tutorial.HotspotDef hotspotDef = Tutorial.FindHotspotDef(hs.TutorialHotspotDef);
		if (hotspotDef != null && hotspotDef.field != null)
		{
			return hotspotDef;
		}
		return null;
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x00181EE4 File Offset: 0x001800E4
	private static void LoadHotspotDefs()
	{
		for (int i = 0; i < Tutorial.hotspot_defs.Count; i++)
		{
			Tutorial.hotspot_defs[i].Unload();
		}
		Tutorial.hotspot_defs.Clear();
		Tutorial.green_hotspot_defs.Clear();
		Tutorial.base_hotspot_def = null;
		DT.Field defField = global::Defs.GetDefField("TutorialHotspot", null);
		if (((defField != null) ? defField.def : null) == null)
		{
			Tutorial.RefreshHotspots(true);
			return;
		}
		Tutorial.hold_alt_time = defField.GetFloat("hold_alt_time", null, Tutorial.hold_alt_time, true, true, true, '.');
		Tutorial.base_hotspot_def = new Tutorial.HotspotDef();
		Tutorial.base_hotspot_def.Load(defField.def);
		if (defField.def.defs != null)
		{
			for (int j = 0; j < defField.def.defs.Count; j++)
			{
				Tutorial.LoadHotspotDef(defField.def.defs[j]);
			}
		}
		Tutorial.RefreshHotspots(true);
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x00181FCC File Offset: 0x001801CC
	private static void LoadHotspotDef(DT.Def dt_def)
	{
		Tutorial.HotspotDef hotspotDef = Tutorial.FindHotspotDef(dt_def.path);
		if (hotspotDef != null)
		{
			hotspotDef.Load(dt_def);
			return;
		}
		hotspotDef = new Tutorial.HotspotDef();
		hotspotDef.Load(dt_def);
		Tutorial.hotspot_defs.Add(hotspotDef);
		if (hotspotDef.HasGreenHighlight())
		{
			Tutorial.green_hotspot_defs.Add(hotspotDef);
		}
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x0018201C File Offset: 0x0018021C
	private static void SaveSeenHotspots(DT.Field f)
	{
		List<DT.SubValue> list = new List<DT.SubValue>();
		for (int i = 0; i < Tutorial.hotspot_defs.Count; i++)
		{
			Tutorial.HotspotDef hotspotDef = Tutorial.hotspot_defs[i];
			if (hotspotDef.seen)
			{
				list.Add(new DT.SubValue
				{
					value = hotspotDef.id
				});
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		f.SetValue("seen_tutorial_hotspots", new Value(list));
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x00182090 File Offset: 0x00180290
	private static void LoadSeenHotspots(DT.Field f)
	{
		List<DT.SubValue> list;
		if ((list = (f.GetValue("seen_tutorial_hotspots", null, true, true, true, '.').obj_val as List<DT.SubValue>)) == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i].value.String(null);
			if (!string.IsNullOrEmpty(text))
			{
				Tutorial.HotspotDef hotspotDef = Tutorial.FindHotspotDef(text);
				if (hotspotDef != null)
				{
					hotspotDef.seen = true;
				}
			}
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x001820FC File Offset: 0x001802FC
	public static void OnUnloadMap()
	{
		for (int i = 0; i < Tutorial.hotspot_defs.Count; i++)
		{
			Tutorial.hotspot_defs[i].seen = false;
		}
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x0018212F File Offset: 0x0018032F
	public static void OnHotspotEnabled(Tooltip hs)
	{
		Container.AddUniqueUnsafe_Class<Tooltip>(Tutorial.hotspots, hs);
		Tutorial.Rule.OnMessage(hs.gameObject, "ui_hotspot_shown", hs.path);
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x00182153 File Offset: 0x00180353
	public static void OnHotspotDisabled(Tooltip hs)
	{
		Tutorial.hotspots.Remove(hs);
		Tutorial.Rule.OnMessage(hs.gameObject, "ui_hotspot_hidden", hs.path);
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x00182178 File Offset: 0x00180378
	public static void RefreshHotspots(bool resolve_defs)
	{
		for (int i = 0; i < Tutorial.hotspots.Count; i++)
		{
			Tutorial.hotspots[i].RefreshTutorialHotspot(resolve_defs);
		}
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x001821AC File Offset: 0x001803AC
	public static void RefreshMouseBlocker()
	{
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject = (baseUI != null) ? baseUI.tutorial_mouse_blocker : null;
		if (gameObject == null)
		{
			return;
		}
		bool activeSelf = gameObject.activeSelf;
		bool flag = Tutorial.active || Tutorial.cur_message_wnd != null;
		if (flag == activeSelf)
		{
			return;
		}
		gameObject.SetActive(flag);
		UIWikiWindow.UpdateParent(true);
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (game.IsMultiplayer())
		{
			return;
		}
		if (game.multiplayer != null && game.multiplayer.playerData != null)
		{
			int pid = game.multiplayer.playerData.pid;
		}
		if (flag)
		{
			game.pause.AddRequest("TutorialPause", -2);
			return;
		}
		game.pause.DelRequest("TutorialPause", -2);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x00182268 File Offset: 0x00180468
	public static void Update()
	{
		if (Tutorial.cur_message_wnd == null)
		{
			Tutorial.ShowNextInQueue();
		}
		bool flag = Tutorial.alt_held;
		bool flag2 = UICommon.GetKey(KeyCode.RightAlt, false);
		bool flag3 = UICommon.GetKey(KeyCode.LeftAlt, false) || (flag2 && UICommon.GetKey(KeyCode.LeftControl, false));
		if (!Application.isFocused)
		{
			flag3 = (flag2 = false);
		}
		float unscaledTime = UnityEngine.Time.unscaledTime;
		if (flag3)
		{
			if (Tutorial.alt_down_time < 0f)
			{
				Tutorial.alt_down_time = unscaledTime;
			}
		}
		else if (Tutorial.alt_down_time >= 0f)
		{
			Tutorial.alt_down_time = -1f;
		}
		Tutorial.alt_held = (Tutorial.alt_down_time >= 0f && unscaledTime - Tutorial.alt_down_time >= Tutorial.hold_alt_time);
		bool flag4 = false;
		if (Tutorial.alt_held != flag)
		{
			flag4 = true;
		}
		if (flag2 && Input.GetKeyDown(KeyCode.Slash) && Game.CheckCheatLevel(Game.CheatLevel.Low, "lock tutorial hotspots", true))
		{
			Tutorial.force_active = !Tutorial.force_active;
			flag4 = true;
		}
		if (flag4 && !Tutorial.suppres_tutorials)
		{
			UIText.AltChanged();
			Tutorial.RefreshMouseBlocker();
			Tutorial.RefreshHotspots(false);
		}
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x00182372 File Offset: 0x00180572
	public static void SupressTutorials(bool supress)
	{
		Tutorial.suppres_tutorials = supress;
		UIText.AltChanged();
		Tutorial.RefreshMouseBlocker();
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x00182384 File Offset: 0x00180584
	public static void CheckMouseClick(string path)
	{
		for (int i = 0; i < Tutorial.green_hotspot_defs.Count; i++)
		{
			Tutorial.HotspotDef hotspotDef = Tutorial.green_hotspot_defs[i];
			if (Game.Match(path, hotspotDef.show_green_highlight_until_clicked, false, '*'))
			{
				hotspotDef.Seen();
			}
		}
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x001823CC File Offset: 0x001805CC
	private static void LoadMessageDefs()
	{
		Tutorial.Topic.ClearAll();
		DT.Field defField = global::Defs.GetDefField("UIWindow", null);
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.defs : null) != null);
		}
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < defField.def.defs.Count; i++)
		{
			DT.Def message_def = defField.def.defs[i];
			Tutorial.Topic topic = Tutorial.Topic.Get(message_def, true);
			if (topic != null)
			{
				topic.AddMessageDef(message_def);
			}
		}
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x00182444 File Offset: 0x00180644
	public static MessageWnd ShowMessage(DT.Def message_def)
	{
		if (((message_def != null) ? message_def.field : null) == null)
		{
			return null;
		}
		Vars vars = new Vars();
		vars.Set<bool>("in_battleview", BattleMap.Get() != null);
		MessageWnd messageWnd = MessageWnd.Create(message_def.field, vars, null, new MessageWnd.OnButton(Tutorial.OnMessageButton));
		if (messageWnd == null)
		{
			return null;
		}
		messageWnd.validate_button = new MessageWnd.ValidateButton(Tutorial.ValidateMessageButton);
		messageWnd.on_close = new UIWindow.OnClose(Tutorial.OnMessageClosed);
		Tutorial.cur_message_wnd = messageWnd;
		Tutorial.Topic topic = Tutorial.Topic.Get(message_def, false);
		if (topic != null)
		{
			topic.SetCurrent(message_def.path);
		}
		Tutorial.RefreshMouseBlocker();
		Tutorial.RefreshHotspots(false);
		Analytics.OnTutorialMessage(GameLogic.Get(false), topic, message_def);
		return messageWnd;
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x001824FC File Offset: 0x001806FC
	public static MessageWnd ShowMessage(string id)
	{
		DT.Field defField = global::Defs.GetDefField(id, null);
		if (((defField != null) ? defField.def : null) == null)
		{
			return null;
		}
		return Tutorial.ShowMessage(defField.def);
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x0018252C File Offset: 0x0018072C
	public static MessageWnd ShowTopic(string id, int forced = 0)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return null;
		}
		if (game.IsMultiplayer())
		{
			return null;
		}
		if (forced <= 0)
		{
			bool flag = BattleMap.Get() != null;
			if ((!flag && !UserSettings.TutorialMessages) || (flag && !UserSettings.TutorialMessagesBattleView))
			{
				Tutorial.DelFromQueue(id, true);
				return null;
			}
			Tutorial.AddToQueue(id);
			return null;
		}
		else
		{
			if (Tutorial.cur_message_wnd != null)
			{
				Tutorial.disable_queue = true;
				Tutorial.cur_message_wnd.Close(false);
				Tutorial.disable_queue = false;
			}
			Tutorial.DelFromQueue(id, true);
			Tutorial.Topic topic = Tutorial.Topic.Get(id, false);
			if (topic == null)
			{
				return null;
			}
			string text = topic.cur_message;
			if (string.IsNullOrEmpty(text))
			{
				text = "first";
			}
			else if (forced >= 2 && text == "skipped")
			{
				text = "first";
			}
			DT.Def def = topic.FindMessageDef(text);
			if (def == null)
			{
				return null;
			}
			return Tutorial.ShowMessage(def);
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x00182604 File Offset: 0x00180804
	public static bool HideTopic(string id)
	{
		if (!Tutorial.DelFromQueue(id, false))
		{
			return false;
		}
		if (Tutorial.cur_message_wnd == null)
		{
			return false;
		}
		DT.Field def_field = Tutorial.cur_message_wnd.def_field;
		Tutorial.Topic topic = Tutorial.Topic.Get((def_field != null) ? def_field.def : null, false);
		if (topic == null)
		{
			return false;
		}
		if (topic.id != id)
		{
			return false;
		}
		Tutorial.cur_message_wnd.Close(false);
		return true;
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x0018266C File Offset: 0x0018086C
	private static int FindQueuedIdx(string id)
	{
		for (int i = 0; i < Tutorial.queued_topics.Count; i++)
		{
			if (Tutorial.queued_topics[i].id == id)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x001826AC File Offset: 0x001808AC
	private static void AddToQueue(string id)
	{
		MessageWnd messageWnd = Tutorial.cur_message_wnd;
		DT.Def message_def;
		if (messageWnd == null)
		{
			message_def = null;
		}
		else
		{
			DT.Field def_field = messageWnd.def_field;
			message_def = ((def_field != null) ? def_field.def : null);
		}
		Tutorial.Topic topic = Tutorial.Topic.Get(message_def, false);
		if (((topic != null) ? topic.id : null) == id)
		{
			return;
		}
		int num = Tutorial.FindQueuedIdx(id);
		if (num < 0)
		{
			Tutorial.queued_topics.Add(new Tutorial.QueuedTopic
			{
				id = id,
				count = 1
			});
			return;
		}
		Tutorial.QueuedTopic value = Tutorial.queued_topics[num];
		value.count++;
		Tutorial.queued_topics[num] = value;
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x00182744 File Offset: 0x00180944
	private static bool DelFromQueue(string id, bool forced)
	{
		int num = Tutorial.FindQueuedIdx(id);
		if (num < 0)
		{
			return true;
		}
		Tutorial.QueuedTopic queuedTopic = Tutorial.queued_topics[num];
		queuedTopic.count--;
		if (forced || queuedTopic.count <= 0)
		{
			Tutorial.queued_topics.RemoveAt(num);
			return true;
		}
		Tutorial.queued_topics[num] = queuedTopic;
		return false;
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x0018279C File Offset: 0x0018099C
	private static bool ShowNextInQueue()
	{
		return !Tutorial.disable_queue && Tutorial.queued_topics.Count != 0 && !(Tutorial.ShowTopic(Tutorial.queued_topics[0].id, 1) == null);
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x001827D6 File Offset: 0x001809D6
	private static void OnMessageClosed(UIWindow wnd)
	{
		if (wnd != Tutorial.cur_message_wnd)
		{
			return;
		}
		Tutorial.cur_message_wnd = null;
		Tutorial.RefreshMouseBlocker();
		Tutorial.RefreshHotspots(false);
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x001827F8 File Offset: 0x001809F8
	public static bool OnBackInputAction(MessageWnd wnd)
	{
		if (wnd != Tutorial.cur_message_wnd)
		{
			return false;
		}
		DT.Field def_field = wnd.def_field;
		Tutorial.Topic topic = Tutorial.Topic.Get((def_field != null) ? def_field.def : null, false);
		DT.Def def = (topic != null) ? topic.GetNext(wnd.def_field.def) : null;
		if (def == null)
		{
			return false;
		}
		Tutorial.disable_queue = true;
		wnd.Close(false);
		Tutorial.ShowMessage(def);
		Tutorial.disable_queue = false;
		return true;
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x00182864 File Offset: 0x00180A64
	private static string ValidateMessageButton(MessageWnd wnd, string btn_id)
	{
		DT.Field def_field = wnd.def_field;
		Tutorial.Topic topic = Tutorial.Topic.Get((def_field != null) ? def_field.def : null, false);
		if (!(btn_id == "prev") && !(btn_id == "_prev") && !(btn_id == "_empty"))
		{
			if (!(btn_id == "next") && !(btn_id == "skip"))
			{
				if (!(btn_id == "done"))
				{
					return "ok";
				}
				if (((topic != null) ? topic.GetNext(wnd.def_field.def) : null) == null)
				{
					return "ok";
				}
				return "not_last";
			}
			else
			{
				if (((topic != null) ? topic.GetNext(wnd.def_field.def) : null) == null)
				{
					return "last";
				}
				return "ok";
			}
		}
		else
		{
			if (((topic != null) ? topic.GetPrev(wnd.def_field.def) : null) != null)
			{
				return "ok";
			}
			if (((topic != null) ? topic.GetNext(wnd.def_field.def) : null) == null)
			{
				return "single";
			}
			if (btn_id == "prev")
			{
				return "_first";
			}
			return "first";
		}
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x00182988 File Offset: 0x00180B88
	private static bool OnMessageButton(MessageWnd wnd, string btn_id)
	{
		DT.Field def_field = wnd.def_field;
		DT.Def message_def = (def_field != null) ? def_field.def : null;
		Tutorial.Topic topic = Tutorial.Topic.Get(message_def, false);
		if (topic == null)
		{
			return false;
		}
		if (btn_id == "skip" || btn_id == "done")
		{
			topic.SetCurrent("skipped");
			wnd.Close(false);
			return true;
		}
		if (btn_id == "disable")
		{
			if (BattleMap.Get() != null)
			{
				UserSettings.GetSetting("tutorial_messages_battles").ApplyValue(false);
			}
			else
			{
				UserSettings.GetSetting("tutorial_messages").ApplyValue(false);
			}
			Tutorial.queued_topics.Clear();
			wnd.Close(false);
			return true;
		}
		if (!(btn_id == "prev") && !(btn_id == "_prev"))
		{
			if (!(btn_id == "next"))
			{
				return false;
			}
			DT.Def next = topic.GetNext(message_def);
			if (next == null)
			{
				return false;
			}
			Tutorial.disable_queue = true;
			wnd.Close(false);
			Tutorial.ShowMessage(next);
			Tutorial.disable_queue = false;
			return true;
		}
		else
		{
			DT.Def prev = topic.GetPrev(message_def);
			if (prev == null)
			{
				return false;
			}
			Tutorial.disable_queue = true;
			wnd.Close(false);
			Tutorial.ShowMessage(prev);
			Tutorial.disable_queue = false;
			return true;
		}
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x00182AC4 File Offset: 0x00180CC4
	public static void Start()
	{
		Game game = GameLogic.Get(false);
		if (game == null || game.state != Game.State.Running)
		{
			return;
		}
		Tutorial.enabled = true;
		bool flag = BattleMap.Get() != null;
		if (!flag && !UserSettings.TutorialMessages)
		{
			return;
		}
		if (flag && !UserSettings.TutorialMessagesBattleView)
		{
			return;
		}
		Tutorial.ShowTopic(flag ? "battleview_tutorial" : "welcome", 1);
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x00182B24 File Offset: 0x00180D24
	public static void Reset()
	{
		Tutorial.Topic.ResetAll();
		bool flag = BattleMap.Get() != null;
		if (!flag && !UserSettings.TutorialMessages)
		{
			return;
		}
		if (flag && !UserSettings.TutorialMessagesBattleView)
		{
			return;
		}
		for (int i = 0; i < Tutorial.hotspots.Count; i++)
		{
			Tooltip tooltip = Tutorial.hotspots[i];
			if (tooltip.isActiveAndEnabled)
			{
				Tutorial.OnHotspotEnabled(tooltip);
			}
		}
		BaseUI baseUI = BaseUI.Get();
		GameObject gameObject;
		if (baseUI == null)
		{
			gameObject = null;
		}
		else
		{
			BaseUI.ISelectionPanel selectionPanel = baseUI.SelectionPanel;
			gameObject = ((selectionPanel != null) ? selectionPanel.gameObject : null);
		}
		GameObject gameObject2 = gameObject;
		if (gameObject2 != null)
		{
			string param = global::Common.ObjPath(gameObject2);
			Tutorial.Rule.OnMessage(gameObject2, "ui_window_shown", param);
		}
		BaseUI baseUI2 = BaseUI.Get();
		List<UIWindow> list;
		if (baseUI2 == null)
		{
			list = null;
		}
		else
		{
			UIWindowDispatcher window_dispatcher = baseUI2.window_dispatcher;
			list = ((window_dispatcher != null) ? window_dispatcher.active_windows_stack : null);
		}
		List<UIWindow> list2 = list;
		if (list2 != null)
		{
			for (int j = 0; j < list2.Count; j++)
			{
				UIWindow uiwindow = list2[j];
				string param2 = global::Common.ObjPath(uiwindow.gameObject);
				Tutorial.Rule.OnMessage(uiwindow.gameObject, "ui_window_shown", param2);
			}
		}
		Tutorial.Start();
	}

	// Token: 0x04001FAA RID: 8106
	public static bool enabled = false;

	// Token: 0x04001FAB RID: 8107
	public static float hold_alt_time = 0f;

	// Token: 0x04001FAC RID: 8108
	public static float alt_down_time = -1f;

	// Token: 0x04001FAD RID: 8109
	public static bool alt_held = false;

	// Token: 0x04001FAE RID: 8110
	public static bool force_active = false;

	// Token: 0x04001FAF RID: 8111
	public static Tutorial.HotspotDef base_hotspot_def;

	// Token: 0x04001FB0 RID: 8112
	public static List<Tutorial.HotspotDef> hotspot_defs = new List<Tutorial.HotspotDef>();

	// Token: 0x04001FB1 RID: 8113
	public static List<Tutorial.HotspotDef> green_hotspot_defs = new List<Tutorial.HotspotDef>();

	// Token: 0x04001FB2 RID: 8114
	private static List<Tooltip> hotspots = new List<Tooltip>();

	// Token: 0x04001FB3 RID: 8115
	private static bool suppres_tutorials = false;

	// Token: 0x04001FB4 RID: 8116
	public static MessageWnd cur_message_wnd = null;

	// Token: 0x04001FB5 RID: 8117
	public static List<Tutorial.QueuedTopic> queued_topics = new List<Tutorial.QueuedTopic>();

	// Token: 0x04001FB6 RID: 8118
	private static bool disable_queue = false;

	// Token: 0x02000859 RID: 2137
	public class HotspotDef
	{
		// Token: 0x060050C4 RID: 20676 RVA: 0x0023ADC4 File Offset: 0x00238FC4
		public void Load(DT.Def dt_def)
		{
			this.id = dt_def.path;
			this.field = dt_def.field;
			this.path_mask = this.field.GetString("path_mask", null, "", true, true, true, '.');
			this.tooltip_def_mask = this.field.GetString("tooltip_def_mask", null, "", true, true, true, '.');
			this.match_field = this.field.FindChild("match", null, true, true, true, '.');
			this.seen_delay = this.field.GetFloat("seen_delay", null, this.seen_delay, true, true, true, '.');
			this.visible_field = this.field.FindChild("visible", null, true, true, true, '.');
			this.force_visible_field = this.field.FindChild("force_visible", null, true, true, true, '.');
			this.show_green_highlight_until_clicked = this.field.GetString("show_green_highlight_until_clicked", null, "", true, true, true, '.');
			this.highlight_prefab = global::Defs.GetObj<GameObject>(this.field, "highlight_prefab", null);
			this.highlight_prefab_seen = global::Defs.GetObj<GameObject>(this.field, "highlight_prefab_seen", null);
			this.highlight_prefab_green = global::Defs.GetObj<GameObject>(this.field, "highlight_prefab_green", null);
			this.mouse_transparent = this.field.GetBool("mouse_transparent", null, this.highlight_prefab == null && this.highlight_prefab_seen == null, true, true, true, '.');
			DT.Field field = this.field.FindChild("tooltip", null, true, true, true, '.');
			if (field != null)
			{
				this.tooltip_def = field.def;
				if (this.tooltip_def == null)
				{
					string text = field.String(null, "");
					if (!string.IsNullOrEmpty(text))
					{
						DT.Field defField = global::Defs.GetDefField(text, null);
						this.tooltip_def = ((defField != null) ? defField.def : null);
						if (this.tooltip_def == null)
						{
							Debug.LogError(field.Path(true, false, '.') + ": Unknown tooltip def: " + text);
						}
					}
				}
			}
		}

		// Token: 0x060050C5 RID: 20677 RVA: 0x0023AFC0 File Offset: 0x002391C0
		public void Unload()
		{
			this.field = null;
			this.path_mask = "";
			this.tooltip_def_mask = "";
			this.match_field = null;
			this.visible_field = null;
			this.force_visible_field = null;
			this.show_green_highlight_until_clicked = null;
			this.highlight_prefab = null;
			this.highlight_prefab_seen = null;
			this.highlight_prefab_green = null;
			this.tooltip_def = null;
		}

		// Token: 0x060050C6 RID: 20678 RVA: 0x0023B022 File Offset: 0x00239222
		public void Seen()
		{
			if (this.seen)
			{
				return;
			}
			this.seen = true;
			Tutorial.RefreshHotspots(false);
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x0023B03A File Offset: 0x0023923A
		public bool HasGreenHighlight()
		{
			return !string.IsNullOrEmpty(this.show_green_highlight_until_clicked);
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x0023B04C File Offset: 0x0023924C
		public bool CheckShowGreenHighlighth()
		{
			bool flag = BattleMap.Get() != null;
			return (flag || UserSettings.TutorialMessages) && (!flag || UserSettings.TutorialMessagesBattleView) && this.HasGreenHighlight() && !this.seen;
		}

		// Token: 0x060050C9 RID: 20681 RVA: 0x0023B090 File Offset: 0x00239290
		public override string ToString()
		{
			string text = this.id;
			if (this.seen)
			{
				text = "[seen] " + text;
			}
			if (this.field == null)
			{
				text = "[INVALID] " + text;
			}
			return text;
		}

		// Token: 0x04003ED0 RID: 16080
		public string id;

		// Token: 0x04003ED1 RID: 16081
		public DT.Field field;

		// Token: 0x04003ED2 RID: 16082
		public string path_mask;

		// Token: 0x04003ED3 RID: 16083
		public string tooltip_def_mask;

		// Token: 0x04003ED4 RID: 16084
		public DT.Field match_field;

		// Token: 0x04003ED5 RID: 16085
		public float seen_delay = 1f;

		// Token: 0x04003ED6 RID: 16086
		public DT.Field visible_field;

		// Token: 0x04003ED7 RID: 16087
		public DT.Field force_visible_field;

		// Token: 0x04003ED8 RID: 16088
		public string show_green_highlight_until_clicked;

		// Token: 0x04003ED9 RID: 16089
		public GameObject highlight_prefab;

		// Token: 0x04003EDA RID: 16090
		public GameObject highlight_prefab_seen;

		// Token: 0x04003EDB RID: 16091
		public GameObject highlight_prefab_green;

		// Token: 0x04003EDC RID: 16092
		public bool mouse_transparent;

		// Token: 0x04003EDD RID: 16093
		public DT.Def tooltip_def;

		// Token: 0x04003EDE RID: 16094
		public bool seen;
	}

	// Token: 0x0200085A RID: 2138
	public class Topic
	{
		// Token: 0x060050CB RID: 20683 RVA: 0x0023B0E0 File Offset: 0x002392E0
		public override string ToString()
		{
			return string.Format("{0} ({1}), current: {2}", this.id, this.message_defs.Count, this.cur_message);
		}

		// Token: 0x060050CC RID: 20684 RVA: 0x0023B108 File Offset: 0x00239308
		public static void ClearAll()
		{
			Tutorial.Topic.registry.Clear();
		}

		// Token: 0x060050CD RID: 20685 RVA: 0x0023B114 File Offset: 0x00239314
		public static void ResetAll()
		{
			foreach (KeyValuePair<string, Tutorial.Topic> keyValuePair in Tutorial.Topic.registry)
			{
				keyValuePair.Value.SetCurrent(null);
			}
		}

		// Token: 0x060050CE RID: 20686 RVA: 0x0023B16C File Offset: 0x0023936C
		public static Tutorial.Topic Get(string id, bool create = false)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			Tutorial.Topic topic;
			if (Tutorial.Topic.registry.TryGetValue(id, out topic))
			{
				return topic;
			}
			if (!create)
			{
				return null;
			}
			topic = new Tutorial.Topic();
			topic.id = id;
			Tutorial.Topic.registry.Add(id, topic);
			return topic;
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x0023B1B3 File Offset: 0x002393B3
		public static Tutorial.Topic Get(DT.Def message_def, bool create = false)
		{
			if (((message_def != null) ? message_def.field : null) == null)
			{
				return null;
			}
			return Tutorial.Topic.Get(message_def.field.GetString("tutorial_topic", null, "", true, true, true, '.'), create);
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x0023B1E6 File Offset: 0x002393E6
		public void AddMessageDef(DT.Def message_def)
		{
			this.message_defs.Add(message_def);
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x0023B1F4 File Offset: 0x002393F4
		public int FindMessageIndex(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return -1;
			}
			if (id == "skipped")
			{
				return -2;
			}
			for (int i = 0; i < this.message_defs.Count; i++)
			{
				if (this.message_defs[i].path == id)
				{
					return i;
				}
			}
			return -3;
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x0023B250 File Offset: 0x00239450
		public DT.Def GetPrev(DT.Def message_def)
		{
			int num = this.FindMessageIndex(message_def.path);
			if (num <= 0)
			{
				return null;
			}
			return this.message_defs[num - 1];
		}

		// Token: 0x060050D3 RID: 20691 RVA: 0x0023B280 File Offset: 0x00239480
		public DT.Def GetNext(DT.Def message_def)
		{
			int num = this.FindMessageIndex(message_def.path);
			if (num < 0 || num + 1 >= this.message_defs.Count)
			{
				return null;
			}
			return this.message_defs[num + 1];
		}

		// Token: 0x060050D4 RID: 20692 RVA: 0x0023B2BE File Offset: 0x002394BE
		public void SetCurrent(string id)
		{
			this.cur_message = id;
		}

		// Token: 0x060050D5 RID: 20693 RVA: 0x0023B2C8 File Offset: 0x002394C8
		public DT.Def FindMessageDef(string id)
		{
			if (this.message_defs == null || this.message_defs.Count == 0)
			{
				return null;
			}
			if (id == "first")
			{
				return this.message_defs[0];
			}
			if (id == "skipped")
			{
				return null;
			}
			for (int i = 0; i < this.message_defs.Count; i++)
			{
				DT.Def def = this.message_defs[i];
				if (def.path == id)
				{
					return def;
				}
			}
			return null;
		}

		// Token: 0x04003EDF RID: 16095
		public string id;

		// Token: 0x04003EE0 RID: 16096
		public List<DT.Def> message_defs = new List<DT.Def>();

		// Token: 0x04003EE1 RID: 16097
		public string cur_message;

		// Token: 0x04003EE2 RID: 16098
		private static Dictionary<string, Tutorial.Topic> registry = new Dictionary<string, Tutorial.Topic>();
	}

	// Token: 0x0200085B RID: 2139
	public struct QueuedTopic
	{
		// Token: 0x060050D8 RID: 20696 RVA: 0x0023B368 File Offset: 0x00239568
		public override string ToString()
		{
			return string.Format("{0} x{1}", this.id, this.count);
		}

		// Token: 0x04003EE3 RID: 16099
		public string id;

		// Token: 0x04003EE4 RID: 16100
		public int count;
	}

	// Token: 0x0200085C RID: 2140
	public class Rule : IVars
	{
		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x060050D9 RID: 20697 RVA: 0x0023B385 File Offset: 0x00239585
		public string id
		{
			get
			{
				return this.dt_def.path;
			}
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x060050DA RID: 20698 RVA: 0x0023B392 File Offset: 0x00239592
		public DT.Field field
		{
			get
			{
				return this.dt_def.field;
			}
		}

		// Token: 0x060050DB RID: 20699 RVA: 0x0023B3A0 File Offset: 0x002395A0
		public bool Load(DT.Def dt_def)
		{
			this.dt_def = dt_def;
			DT.Field field = this.field;
			this.log = field.GetInt("log", null, 0, true, true, true, '.');
			DT.Field field2 = field.FindChild("triggers", null, true, true, true, '.');
			List<DT.Field> list = (field2 != null) ? field2.Children() : null;
			if (list != null)
			{
				this.triggers = new List<Trigger.Def>(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					DT.Field field3 = list[i];
					if (!string.IsNullOrEmpty(field3.key))
					{
						Trigger.Def def = Trigger.Def.Load(field3);
						if (def != null)
						{
							this.triggers.Add(def);
							this.AddListener(def);
						}
					}
				}
			}
			else
			{
				Tutorial.Rule.Warning(this.id + " has no triggers");
				this.triggers = null;
			}
			DT.Field field4 = field.FindChild("args", null, true, true, true, '.');
			List<DT.Field> list2 = (field4 != null) ? field4.Children() : null;
			if (list2 != null)
			{
				this.args = new List<DT.Field>(list2.Count);
				for (int j = 0; j < list2.Count; j++)
				{
					DT.Field field5 = list2[j];
					if (!string.IsNullOrEmpty(field5.key))
					{
						this.args.Add(field5);
					}
				}
			}
			else
			{
				this.args = null;
			}
			return true;
		}

		// Token: 0x060050DC RID: 20700 RVA: 0x0023B4E4 File Offset: 0x002396E4
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			for (DT.Field field = this.field; field != null; field = field.parent)
			{
				Value value = field.GetValue(key, vars, true, true, true, '.');
				if (!value.is_unknown)
				{
					return value;
				}
			}
			return Value.Unknown;
		}

		// Token: 0x060050DD RID: 20701 RVA: 0x0023B522 File Offset: 0x00239722
		public override string ToString()
		{
			return "Tutorial Rule '" + this.id + "'";
		}

		// Token: 0x060050DE RID: 20702 RVA: 0x0023B539 File Offset: 0x00239739
		private static void Log(string msg)
		{
			Debug.Log("TutorialRules: " + msg);
		}

		// Token: 0x060050DF RID: 20703 RVA: 0x0023B54B File Offset: 0x0023974B
		private static void Warning(string msg)
		{
			Debug.LogWarning("TutorialRules: " + msg);
		}

		// Token: 0x060050E0 RID: 20704 RVA: 0x0023B55D File Offset: 0x0023975D
		private static void Error(string msg)
		{
			Debug.LogError("TutorialRules: " + msg);
		}

		// Token: 0x060050E1 RID: 20705 RVA: 0x0023B570 File Offset: 0x00239770
		private void AddListener(Trigger.Def tdef)
		{
			if (tdef.messages == null)
			{
				return;
			}
			if (Tutorial.Rule.listeners == null)
			{
				Tutorial.Rule.listeners = new Dictionary<string, List<Tutorial.Rule.Listener>>();
			}
			Tutorial.Rule.Listener item = new Tutorial.Rule.Listener
			{
				rule = this,
				tdef = tdef
			};
			for (int i = 0; i < tdef.messages.Count; i++)
			{
				string key = tdef.messages[i];
				List<Tutorial.Rule.Listener> list;
				if (!Tutorial.Rule.listeners.TryGetValue(key, out list))
				{
					list = new List<Tutorial.Rule.Listener>();
					Tutorial.Rule.listeners.Add(key, list);
				}
				list.Add(item);
			}
		}

		// Token: 0x060050E2 RID: 20706 RVA: 0x0023B600 File Offset: 0x00239800
		public static void LoadDefs()
		{
			Tutorial.Rule.root_dt_def = null;
			Tutorial.Rule.rules.Clear();
			Tutorial.Rule.listeners = null;
			DT.Field defField = global::Defs.GetDefField("TutorialRule", null);
			Tutorial.Rule.root_dt_def = ((defField != null) ? defField.def : null);
			if (Tutorial.Rule.root_dt_def == null)
			{
				Tutorial.Rule.Error("Root def not found");
				return;
			}
			if (Tutorial.Rule.root_dt_def.defs == null)
			{
				Tutorial.Rule.Warning("No defs found");
				return;
			}
			Tutorial.Rule.AddTargetTypes();
			for (int i = 0; i < Tutorial.Rule.root_dt_def.defs.Count; i++)
			{
				DT.Def def = Tutorial.Rule.root_dt_def.defs[i];
				Tutorial.Rule rule = new Tutorial.Rule();
				if (rule.Load(def))
				{
					Tutorial.Rule.rules.Add(rule);
				}
			}
		}

		// Token: 0x060050E3 RID: 20707 RVA: 0x0023B6B2 File Offset: 0x002398B2
		private static void AddTargetTypes()
		{
			if (TargetType.Find("ui_element") == null)
			{
				TargetType.Add("ui_element", typeof(GameObject), null, Array.Empty<string>());
			}
		}

		// Token: 0x060050E4 RID: 20708 RVA: 0x0023B6DC File Offset: 0x002398DC
		public static void OnMessage(object obj, string message, object param)
		{
			if (Tutorial.Rule.listeners == null)
			{
				return;
			}
			Game game = GameLogic.Get(false);
			if (game == null)
			{
				return;
			}
			if (game.IsMultiplayer())
			{
				return;
			}
			using (Game.Profile("TutorialRules.OnMessage", false, 10f, null))
			{
				List<Tutorial.Rule.Listener> list;
				if (Tutorial.Rule.listeners.TryGetValue(message, out list))
				{
					try
					{
						for (int i = 0; i < list.Count; i++)
						{
							Tutorial.Rule.ProcessTrigger(list[i], obj, message, param);
						}
					}
					catch (Exception arg)
					{
						Tutorial.Rule.Error(string.Format("Error in OnMessage({0}.{1}): ", obj, message) + arg);
					}
				}
			}
		}

		// Token: 0x060050E5 RID: 20709 RVA: 0x0023B794 File Offset: 0x00239994
		private static void ProcessTrigger(Tutorial.Rule.Listener listener, object sender, string message, object param)
		{
			if (listener.rule.log >= 2)
			{
				Tutorial.Rule.Log(string.Format("Processing {0}: {1}.{2}({3})", new object[]
				{
					listener,
					sender,
					message,
					param
				}));
			}
			using (Game.Profile("TutorialRules.ProcessTrigger", false, 10f, null))
			{
				Game game = GameLogic.Get(false);
				Tutorial.Rule.tmp_vars.obj = new Value(listener.rule);
				if (listener.tdef.sender_type != null)
				{
					object obj = listener.tdef.sender_type.Resolve(sender);
					if (obj == null)
					{
						return;
					}
					Tutorial.Rule.tmp_vars.Set<object>("target", obj);
				}
				else
				{
					Tutorial.Rule.tmp_vars.Set<Value>("target", Value.Null);
				}
				Tutorial.Rule.tmp_trigger.Set(listener.tdef, sender, message, param, 0, Tutorial.Rule.tmp_vars);
				if (!Tutorial.Rule.tmp_trigger.Validate(game, Tutorial.Rule.tmp_vars))
				{
					Tutorial.Rule.tmp_trigger.Clear();
				}
				else
				{
					Vars vars = new Vars(listener.rule);
					vars.Set<Trigger>("trigger", Tutorial.Rule.tmp_trigger);
					if (listener.rule.args != null)
					{
						Tutorial.Rule.tmp_vars.Set<Trigger>("trigger", Tutorial.Rule.tmp_trigger);
						for (int i = 0; i < listener.rule.args.Count; i++)
						{
							DT.Field field = listener.rule.args[i];
							string key = field.key;
							Value val = field.Value(Tutorial.Rule.tmp_vars, true, true);
							if (val.is_unknown && Tutorial.Rule.tmp_trigger.named_vars != null)
							{
								val = Tutorial.Rule.tmp_trigger.named_vars.Get(key, true);
							}
							if (val.is_unknown)
							{
								Tutorial.Rule.Warning(string.Format("Could not resolve {0}.{1}", listener.rule, key));
							}
							else
							{
								vars.Set<Value>(key, val);
							}
						}
					}
					if (listener.rule.log > 0)
					{
						Tutorial.Rule.Log(string.Format("Activating {0}: {1})", listener.rule, Tutorial.Rule.tmp_trigger));
					}
					listener.rule.Activate(Tutorial.Rule.tmp_trigger, vars);
				}
			}
		}

		// Token: 0x060050E6 RID: 20710 RVA: 0x0023B9D8 File Offset: 0x00239BD8
		private void Activate(Trigger trigger, Vars args)
		{
			string text = args.Get("hide_topic", true).String(null);
			if (!string.IsNullOrEmpty(text))
			{
				Tutorial.HideTopic(text);
			}
			string text2 = args.Get("show_topic", true).String(null);
			if (!string.IsNullOrEmpty(text2))
			{
				Tutorial.ShowTopic(text2, 0);
			}
		}

		// Token: 0x04003EE5 RID: 16101
		public DT.Def dt_def;

		// Token: 0x04003EE6 RID: 16102
		public int log;

		// Token: 0x04003EE7 RID: 16103
		public List<Trigger.Def> triggers;

		// Token: 0x04003EE8 RID: 16104
		public List<DT.Field> args;

		// Token: 0x04003EE9 RID: 16105
		public static DT.Def root_dt_def;

		// Token: 0x04003EEA RID: 16106
		public static List<Tutorial.Rule> rules = new List<Tutorial.Rule>();

		// Token: 0x04003EEB RID: 16107
		public static Dictionary<string, List<Tutorial.Rule.Listener>> listeners;

		// Token: 0x04003EEC RID: 16108
		private static Trigger tmp_trigger = new Trigger(null, null, null, null, 0, null);

		// Token: 0x04003EED RID: 16109
		private static Vars tmp_vars = new Vars();

		// Token: 0x02000A3E RID: 2622
		public struct Listener
		{
			// Token: 0x060055F2 RID: 22002 RVA: 0x0024C1E2 File Offset: 0x0024A3E2
			public override string ToString()
			{
				return this.rule.id + "." + this.tdef.name;
			}

			// Token: 0x0400470F RID: 18191
			public Tutorial.Rule rule;

			// Token: 0x04004710 RID: 18192
			public Trigger.Def tdef;
		}
	}

	// Token: 0x0200085D RID: 2141
	public class RulesListener : IListener
	{
		// Token: 0x060050E9 RID: 20713 RVA: 0x0023BA56 File Offset: 0x00239C56
		public void OnMessage(object obj, string message, object param)
		{
			Tutorial.Rule.OnMessage(obj, message, param);
		}
	}
}
