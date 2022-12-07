using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200006A RID: 106
public class AutoSaveManager : MonoBehaviour
{
	// Token: 0x060002BF RID: 703 RVA: 0x000259E0 File Offset: 0x00023BE0
	public static AutoSaveManager Get()
	{
		if (!(AutoSaveManager.instance == null))
		{
			return AutoSaveManager.instance;
		}
		GameObject gameObject = GameObject.Find("AutoSaveManager");
		if (gameObject == null)
		{
			GameObject obj = global::Defs.GetObj<GameObject>("AutoSaveSettings", "singletonHolder", null);
			if (obj == null)
			{
				gameObject = new GameObject("AutoSave");
			}
			else
			{
				gameObject = global::Common.Spawn(obj, false, false);
			}
		}
		if (gameObject == null)
		{
			return null;
		}
		gameObject.hideFlags = HideFlags.DontSave;
		AutoSaveManager.instance = gameObject.GetComponent<AutoSaveManager>();
		if (AutoSaveManager.instance == null)
		{
			AutoSaveManager.instance = gameObject.AddComponent<AutoSaveManager>();
		}
		AutoSaveManager.instance.Init();
		return AutoSaveManager.instance;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00025A8C File Offset: 0x00023C8C
	public static void EnablePeriodicAutosave(bool enable = true, bool forceChange = false)
	{
		if (AutoSaveManager.instance == null && AutoSaveManager.Get() == null)
		{
			return;
		}
		if (forceChange)
		{
			AutoSaveManager.instance.enabled = enable;
		}
		else
		{
			AutoSaveManager.instance.enabled = (enable && AutoSaveManager.instance.IsAutoSaveAllowed(AutoSaveManager.Type.Periodic));
		}
		AutoSaveManager.instance.timer = 0f;
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00025AF0 File Offset: 0x00023CF0
	public static void Save(AutoSaveManager.Type type, string save_id = null, string reason = null)
	{
		if (AutoSaveManager.instance == null && AutoSaveManager.Get() == null)
		{
			return;
		}
		if (!AutoSaveManager.instance.IsAutoSaveAllowed(type))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Campaign campaign = (game != null) ? game.campaign : null;
		if (campaign == null)
		{
			Debug.LogError("Campaign is null on Autosave");
			return;
		}
		int num = (int)game.session_time.seconds;
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign.id);
		if (((campaignInfo != null) ? campaignInfo.latest_save : null) != null && campaignInfo.latest_save.session_time == num)
		{
			return;
		}
		string path = campaign.Dir();
		int num2 = -1;
		int num3 = -1;
		string name;
		string subfolder;
		if (type == AutoSaveManager.Type.Periodic)
		{
			num2 = AutoSaveManager.instance.GetNextIncrementalNumber(path, type);
			Vars vars = new Vars();
			vars.Set<int>("number", num2);
			name = global::Defs.Localize("AutoSaveSettings.autoSaveName", vars, null, true, true);
			subfolder = string.Format("Autosave {0}", num2);
		}
		else
		{
			num3 = AutoSaveManager.instance.GetNextIncrementalNumber(path, type);
			AutoSaveManager.instance.DelExcessEventSave(path);
			Vars vars2 = new Vars();
			vars2.Set<int>("number", num3);
			vars2.Set<string>("reason", "#" + global::Defs.Localize("AutoSaveSettings.Event." + reason, null, null, true, true));
			name = global::Defs.Localize("AutoSaveSettings.EventName", vars2, null, true, true);
			subfolder = string.Format("Event {0}", num3);
		}
		SaveGame.Save((campaign != null) ? campaign.Dir(subfolder) : null, name, num2, num3, save_id);
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00025C83 File Offset: 0x00023E83
	public static void OnSyncSave(string save_id)
	{
		AutoSaveManager.Save(AutoSaveManager.Type.Event, save_id, "on_sync");
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00025C91 File Offset: 0x00023E91
	public static void SaveOnDisconnect()
	{
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.campaign : null) == null)
		{
			return;
		}
		AutoSaveManager.Save(AutoSaveManager.Type.Event, null, "on_disconnect");
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00025CB4 File Offset: 0x00023EB4
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		if (this.allowedScenes == null)
		{
			this.allowedScenes = new List<string>();
		}
		DT.Field defField = global::Defs.GetDefField("AutoSaveSettings", null);
		DT.Field field = (defField != null) ? defField.FindChild("allowedScenes", null, true, true, true, '.') : null;
		if (field == null)
		{
			return;
		}
		int num = field.NumValues();
		for (int i = 0; i < num; i++)
		{
			string item = field.String(i, null, "");
			this.allowedScenes.Add(item);
		}
		this.timeBetweenAutoSaves = global::Defs.GetFloat("AutoSaveSettings", "timeBetweenSaves", null, 0f);
		this.numOfIncrementalAutoSaves = global::Defs.GetInt("AutoSaveSettings", "numOfIncrementalAutoSaves", null, 0);
		this.numOfIncrementalEvents = global::Defs.GetInt("AutoSaveSettings", "numOfIncrementalEvents", null, 0);
		this.inited = true;
		AutoSaveManager.EnablePeriodicAutosave(UserSettings.AutoSave, false);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00025D8C File Offset: 0x00023F8C
	private void Awake()
	{
		AutoSaveManager.instance = this;
		this.Init();
		SceneManager.activeSceneChanged += this.ChangedActiveScene;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00025DB8 File Offset: 0x00023FB8
	public void Update()
	{
		this.Init();
		Game game = GameLogic.Get(false);
		if (game == null || game.IsPaused())
		{
			return;
		}
		if (!this.IsAutoSaveAllowedInScene(SceneManager.GetActiveScene()))
		{
			this.timer = 0f;
			return;
		}
		this.timer += UnityEngine.Time.unscaledDeltaTime;
		if (this.timer >= this.timeBetweenAutoSaves)
		{
			AutoSaveManager.Save(AutoSaveManager.Type.Periodic, null, null);
			this.timer = 0f;
		}
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00025E2C File Offset: 0x0002402C
	public int GetNextIncrementalNumber(string path, AutoSaveManager.Type type)
	{
		if (string.IsNullOrEmpty(path))
		{
			return 1;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return 1;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		if (directories.Length == 0)
		{
			return 1;
		}
		SaveGame.Info info = null;
		DirectoryInfo[] array = directories;
		for (int i = 0; i < array.Length; i++)
		{
			SaveGame.Info info2 = SaveGame.FindByPath(array[i].FullName);
			if (info2 != null && (type != AutoSaveManager.Type.Periodic || info2.autoSaveNum != -1) && (type != AutoSaveManager.Type.Event || info2.eventNum != -1) && (info == null || info2.date_time > info.date_time))
			{
				info = info2;
			}
		}
		if (info == null)
		{
			return 1;
		}
		if (type == AutoSaveManager.Type.Periodic)
		{
			return info.autoSaveNum % this.numOfIncrementalAutoSaves + 1;
		}
		return info.eventNum % this.numOfIncrementalEvents + 1;
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00025EE8 File Offset: 0x000240E8
	private void DelExcessEventSave(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		if (directories.Length == 0)
		{
			return;
		}
		SaveGame.Info info = null;
		int num = 0;
		DirectoryInfo[] array = directories;
		for (int i = 0; i < array.Length; i++)
		{
			SaveGame.Info info2 = SaveGame.FindByPath(array[i].FullName);
			if (info2 != null && info2.eventNum != -1)
			{
				if (info == null || info2.date_time <= info.date_time)
				{
					info = info2;
				}
				num++;
			}
		}
		if (num < this.numOfIncrementalEvents)
		{
			return;
		}
		if (info == null)
		{
			return;
		}
		SaveGame.Delete(info.fullPath);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00025F8C File Offset: 0x0002418C
	private bool IsAutoSaveAllowed(AutoSaveManager.Type type = AutoSaveManager.Type.Periodic)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		return this.IsAutoSaveAllowed(type, activeScene);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00025FA8 File Offset: 0x000241A8
	private bool IsAutoSaveAllowed(AutoSaveManager.Type type, Scene scene)
	{
		AutoSaveManager autoSaveManager = AutoSaveManager.instance;
		if (autoSaveManager != null)
		{
			autoSaveManager.Init();
		}
		if (type == AutoSaveManager.Type.Periodic && !UserSettings.AutoSave)
		{
			return false;
		}
		Game game = GameLogic.Get(false);
		if (game == null || game.state < Game.State.Running)
		{
			return false;
		}
		if (type == AutoSaveManager.Type.Periodic && !AutoSaveManager.instance.IsAutoSaveAllowedInScene(scene))
		{
			return false;
		}
		if (!SaveGame.CanSave(false))
		{
			return false;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		return kingdom != null && !kingdom.IsDefeated() && (game.rules == null || (!game.rules.early_end_triggered && !game.rules.end_game_triggered));
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0002603A File Offset: 0x0002423A
	private bool IsAutoSaveAllowedInScene(Scene scene)
	{
		this.Init();
		return this.allowedScenes.Contains(scene.name);
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00026054 File Offset: 0x00024254
	private void ChangedActiveScene(Scene current, Scene next)
	{
		AutoSaveManager.EnablePeriodicAutosave(this.IsAutoSaveAllowed(AutoSaveManager.Type.Periodic, next), true);
	}

	// Token: 0x040003E2 RID: 994
	private static AutoSaveManager instance;

	// Token: 0x040003E3 RID: 995
	private float timer;

	// Token: 0x040003E4 RID: 996
	private float timeBetweenAutoSaves = 300f;

	// Token: 0x040003E5 RID: 997
	private int numOfIncrementalAutoSaves = 5;

	// Token: 0x040003E6 RID: 998
	private int numOfIncrementalEvents = 5;

	// Token: 0x040003E7 RID: 999
	private List<string> allowedScenes = new List<string>();

	// Token: 0x040003E8 RID: 1000
	private bool inited;

	// Token: 0x02000527 RID: 1319
	public enum Type
	{
		// Token: 0x04002F20 RID: 12064
		Periodic,
		// Token: 0x04002F21 RID: 12065
		Event
	}
}
