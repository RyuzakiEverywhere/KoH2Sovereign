using System;
using System.Collections;
using System.IO;
using System.Threading;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002CC RID: 716
public class Title : MonoBehaviour
{
	// Token: 0x06002CCB RID: 11467 RVA: 0x001751BA File Offset: 0x001733BA
	public static Title Get()
	{
		return Title.instance;
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x001751C4 File Offset: 0x001733C4
	private void Start()
	{
		Title.instance = this;
		UserInteractionLogger.StartLogging();
		GameSpeed.OnSetSpeed(1f);
		UserSettings.InitSettings();
		WindowGhosting.Disable();
		this.BuildBadWordsFilter();
		this.intro = global::Common.FindChildByName(base.gameObject, "id_Intro", true, true);
		if (this.intro != null)
		{
			this.intro.SetActive(false);
		}
		this.loadingProgress = global::Common.FindChildByName(base.gameObject, "id_Progress", true, true);
		this.loadingProgressBar = global::Common.FindChildByName(this.loadingProgress, "id_ProgressBar", true, true);
		this.loadingProgressText = global::Common.FindChildComponent<TextMeshProUGUI>(this.loadingProgress, "id_ProgressText");
		LoadingScreen loadingScreen = LoadingScreen.Get();
		if (loadingScreen != null)
		{
			loadingScreen.Show(false, true, false);
		}
		this.SetProgress(0f);
		this.SetStatus(null);
		if (BackgroundMusic.Instance != null)
		{
			UnityEngine.Object.Destroy(BackgroundMusic.Instance.gameObject);
		}
		if (Title.showLogos)
		{
			base.StartCoroutine(this.ShowLogos());
			this.PrepGame();
			return;
		}
		base.StartCoroutine(this.Load(true));
		this.PrepGame();
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x001752DE File Offset: 0x001734DE
	private IEnumerator ShowLogos()
	{
		Title.showLogos = false;
		this.intro.SetActive(true);
		yield return null;
		Animator animator = this.intro.GetComponent<Animator>();
		AnimationEventListener component = this.intro.GetComponent<AnimationEventListener>();
		bool canLoadDefs = false;
		bool canLoadAssets = false;
		bool introDone = false;
		component.OnAnimationEvent = (Action<string>)Delegate.Combine(component.OnAnimationEvent, new Action<string>(delegate(string r)
		{
			if (r == "LoadDefs")
			{
				canLoadDefs = true;
			}
			if (r == "LoadAssets")
			{
				canLoadAssets = true;
			}
			if (r == "Done")
			{
				introDone = true;
			}
		}));
		while (!canLoadDefs)
		{
			yield return null;
		}
		animator.speed = 0f;
		base.StartCoroutine(this.Load(false));
		while (!this.defsLoaded)
		{
			yield return null;
		}
		animator.speed = 1f;
		while (!canLoadAssets)
		{
			yield return null;
		}
		animator.speed = 0f;
		Assets.status_changed = (Assets.StatusChanged)Delegate.Combine(Assets.status_changed, new Assets.StatusChanged(this.OnAssetsStatusChanged));
		Assets.progress_changed = (Assets.ProgressChanged)Delegate.Combine(Assets.progress_changed, new Assets.ProgressChanged(this.OnAssetsProgressChanged));
		Assets.Init(false);
		while (!Title.allLoaded)
		{
			yield return null;
		}
		animator.speed = 1f;
		while (!introDone)
		{
			yield return null;
		}
		this.intro.SetActive(false);
		yield break;
	}

	// Token: 0x06002CCE RID: 11470 RVA: 0x001752F0 File Offset: 0x001734F0
	private void PrepGame()
	{
		Game.isComingFromTitle = true;
		Game.isJoiningGame = false;
		Game game = GameLogic.Get(true);
		game.state = Game.State.InLobby;
		game.campaign = null;
		this.mpBoss = MPBoss.Get(true);
		this.mpBoss.InitOnlineService();
		this.mpBoss.CancelHostMigration();
		MPBoss mpboss = this.mpBoss;
		if (((mpboss != null) ? mpboss.currently_entered_campaign : null) != null)
		{
			this.mpBoss.LeaveLobby(this.mpBoss.currently_entered_campaign, true);
		}
	}

	// Token: 0x06002CCF RID: 11471 RVA: 0x0017536B File Offset: 0x0017356B
	private IEnumerator Load(bool load_asset)
	{
		yield return null;
		yield return null;
		GameLogic.Get(true);
		yield return null;
		ModManager modManager = ModManager.Get(true);
		modManager.AreBaseDefsValid();
		UserSettings.SettingData setting = UserSettings.GetSetting("active_mod");
		if (setting != null)
		{
			modManager.LoadActiveModData(setting.value.String(null));
		}
		if (global::Defs.Get(true).dt == null)
		{
			DynamicIconBuilder dynamicIconBuilder = DynamicIconBuilder.Instance;
			new Thread(new ThreadStart(this.LoadDefsThread)).Start();
			yield return null;
		}
		else
		{
			this.defsLoaded = true;
		}
		new Thread(new ThreadStart(this.LoadModsThread)).Start();
		yield return null;
		if (load_asset)
		{
			Assets.status_changed = (Assets.StatusChanged)Delegate.Combine(Assets.status_changed, new Assets.StatusChanged(this.OnAssetsStatusChanged));
			Assets.progress_changed = (Assets.ProgressChanged)Delegate.Combine(Assets.progress_changed, new Assets.ProgressChanged(this.OnAssetsProgressChanged));
			Assets.Init(false);
		}
		yield break;
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x00175384 File Offset: 0x00173584
	private void LoadDefsThread()
	{
		Thread.CurrentThread.Name = "Load Defs";
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return;
		}
		defs.Load(null, true, false);
		DynamicIconBuilder.Instance.Init();
		this.defsLoaded = true;
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x001753CC File Offset: 0x001735CC
	private void LoadModsThread()
	{
		Thread.CurrentThread.Name = "Load Mods";
		ModManager modManager = ModManager.Get(false);
		if (modManager == null)
		{
			return;
		}
		modManager.LoadModList();
		this.modsLoaded = true;
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x00175400 File Offset: 0x00173600
	private void OnDisable()
	{
		Assets.status_changed = (Assets.StatusChanged)Delegate.Remove(Assets.status_changed, new Assets.StatusChanged(this.OnAssetsStatusChanged));
		Assets.progress_changed = (Assets.ProgressChanged)Delegate.Remove(Assets.progress_changed, new Assets.ProgressChanged(this.OnAssetsProgressChanged));
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x00175450 File Offset: 0x00173650
	private void OnGUI()
	{
		string text = Title.Version(false);
		if (text == "")
		{
			return;
		}
		GUI.color = Color.white;
		GUI.Label(new Rect(0f, 0f, 400f, 20f), "Build " + text);
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x001754A8 File Offset: 0x001736A8
	public static string Version(bool only_version = false)
	{
		if (Title.version_and_branch != null && !only_version)
		{
			return Title.version_and_branch;
		}
		if (Title.version == null)
		{
			Title.version = "";
			string path = Application.streamingAssetsPath + "/version.txt";
			try
			{
				Title.version = File.ReadAllText(path);
				Debug.Log("Build " + Title.version);
			}
			catch
			{
			}
		}
		string text = Title.BranchName();
		if (string.IsNullOrEmpty(text))
		{
			return Title.version;
		}
		Title.version_and_branch = Title.version + " (" + text + ")";
		if (only_version)
		{
			return Title.version;
		}
		return Title.version_and_branch;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x00175558 File Offset: 0x00173758
	public static string BranchName()
	{
		return Game.BranchName();
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x0017555F File Offset: 0x0017375F
	public static bool IsInternalBranch()
	{
		return Game.IsInternalBranch();
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x00175566 File Offset: 0x00173766
	private void OnAssetsStatusChanged(string status)
	{
		if (status == "Done")
		{
			this.loadingProgress.SetActive(false);
			this.OnAssetsDone();
			return;
		}
		if (Assets.available)
		{
			this.assetBundlesLoaded = true;
		}
		this.SetStatus(status);
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x001755A0 File Offset: 0x001737A0
	private void SetStatus(string status)
	{
		if (string.IsNullOrEmpty(status) || status == "Done")
		{
			UIText.SetText(this.loadingProgressText, "");
			this.loadingProgress.SetActive(false);
			return;
		}
		UIText.SetText(this.loadingProgressText, global::Defs.Localize(status, null, null, true, true));
		this.loadingProgress.SetActive(true);
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x00175600 File Offset: 0x00173800
	private void OnAssetsProgressChanged(float progress)
	{
		this.SetProgress(progress);
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x00175609 File Offset: 0x00173809
	private void SetProgress(float progress)
	{
		this.loadingProgressBar.transform.localScale = new Vector3(progress * 0.9f, 1f, 1f);
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x00175634 File Offset: 0x00173834
	private void Update()
	{
		if (Assets.IsLoadingWholeBundles())
		{
			Assets.CalcProgress();
		}
		if (this.loadingOperation != null)
		{
			this.SetProgress(this.loadingOperation.progress);
		}
		if (this.defsLoaded)
		{
			MPBoss mpboss = this.mpBoss;
			if (mpboss != null && mpboss.GetSystemState() == MPBoss.SystemState.Default)
			{
				this.mpBoss.Start(false);
			}
		}
		if (this.defsLoaded && this.assetBundlesLoaded && !Title.allLoaded)
		{
			this.OnAllLoaded();
		}
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x001756AE File Offset: 0x001738AE
	private void OnAssetsDone()
	{
		this.assetBundlesLoaded = true;
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x001756B8 File Offset: 0x001738B8
	private void OnAllLoaded()
	{
		Title.allLoaded = true;
		global::Defs defs = global::Defs.Get(false);
		if (defs != null)
		{
			defs.PostProcess();
		}
		using (Game.Profile("LoadingFMODBanks", false, 0f, null))
		{
			MapData.Get().LoadFMODBanks("Title_Banks");
		}
		this.SetStatus(null);
		Game game = GameLogic.Get(true);
		if (!game.started)
		{
			game.Start();
		}
		DT.Field defField = global::Defs.GetDefField("DevSettings", null);
		if (defField != null)
		{
			int @int = defField.GetInt("cheat_level", null, -1, true, true, true, '.');
			if (@int != -1)
			{
				Game.cheat_level = (Game.CheatLevel)@int;
			}
		}
		if (!string.IsNullOrEmpty(THQNORequest.lobbyIdToJoin))
		{
			MPBoss mpboss = MPBoss.Get();
			if (mpboss != null)
			{
				mpboss.OnCallbackMessage("on_invite_response", null);
			}
		}
		DataCollectionController component = base.GetComponent<DataCollectionController>();
		if (component != null)
		{
			component.OnAllLoaded();
		}
		RichPresence.Update(RichPresence.State.InTitle);
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x001757A8 File Offset: 0x001739A8
	public void OnLoadScene(string scene)
	{
		this.loadingProgress.SetActive(true);
		this.SetStatus("Starting " + Path.GetFileName(scene));
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x001757CC File Offset: 0x001739CC
	public void StartTestMapSinglePlayer()
	{
		GameLogic.Get(false).StartGame(true, "test", null);
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x001757E0 File Offset: 0x001739E0
	public void ContinueGame()
	{
		SaveGame.UpdateList(true, true);
		if (SaveGame.latest_single_playr_campaign_info == null)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		game.campaign = SaveGame.latest_single_playr_campaign_info.campaign;
		game.load_game = Game.LoadedGameType.Continue;
		if (game == null)
		{
			return;
		}
		game.StartGame(false, null, SaveGame.latest_single_playr_campaign_info.latest_save.fullPath);
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x00175834 File Offset: 0x00173A34
	public void StopCampaignsLogic()
	{
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.DestroyMultiplayer();
		}
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x00175854 File Offset: 0x00173A54
	private void BuildBadWordsFilter()
	{
		if (GameObject.Find("BWF") == null)
		{
			if (this.badWordsFilterPrefab != null)
			{
				global::Common.Spawn(this.badWordsFilterPrefab, false, false).name = "BWF";
				return;
			}
			Debug.LogError("Bad words filter prefab is not attached to the script!");
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x001758A3 File Offset: 0x00173AA3
	public void Quit()
	{
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.OnQuitGameAnalytics("quit_to_desktop");
		}
		Assets.Shutdown();
		Debug.Log("Application quitting, reason: Title.Quit");
		Application.Quit();
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x001758CF File Offset: 0x00173ACF
	public void BuildSingleplayerGameData(Game game, string map_name)
	{
		CampaignUtils.InitForSingleplayer(game);
		CampaignUtils.PopulateDummyGame(game, map_name);
		TitleMap titleMap = TitleMap.Get();
		if (titleMap == null)
		{
			return;
		}
		titleMap.LoadGame(game, true);
	}

	// Token: 0x04001E8E RID: 7822
	public const string title_scene = "Title/title";

	// Token: 0x04001E8F RID: 7823
	public const string europe_scene = "Europe/europe";

	// Token: 0x04001E90 RID: 7824
	public const string test_scene = "Test/Michael/Test";

	// Token: 0x04001E91 RID: 7825
	public GameObject badWordsFilterPrefab;

	// Token: 0x04001E92 RID: 7826
	[HideInInspector]
	public bool assetBundlesLoaded;

	// Token: 0x04001E93 RID: 7827
	[HideInInspector]
	public bool defsLoaded;

	// Token: 0x04001E94 RID: 7828
	[HideInInspector]
	public bool modsLoaded;

	// Token: 0x04001E95 RID: 7829
	[HideInInspector]
	public static bool allLoaded = false;

	// Token: 0x04001E96 RID: 7830
	[HideInInspector]
	public bool multiplayerForgotPasswordRequestSent;

	// Token: 0x04001E97 RID: 7831
	private GameObject intro;

	// Token: 0x04001E98 RID: 7832
	private GameObject loadingProgress;

	// Token: 0x04001E99 RID: 7833
	private GameObject loadingProgressBar;

	// Token: 0x04001E9A RID: 7834
	private TextMeshProUGUI loadingProgressText;

	// Token: 0x04001E9B RID: 7835
	private AsyncOperation loadingOperation;

	// Token: 0x04001E9C RID: 7836
	private MPBoss mpBoss;

	// Token: 0x04001E9D RID: 7837
	private static bool showLogos = true;

	// Token: 0x04001E9E RID: 7838
	private static Title instance;

	// Token: 0x04001E9F RID: 7839
	private static string version = null;

	// Token: 0x04001EA0 RID: 7840
	private static string version_and_branch = null;

	// Token: 0x04001EA1 RID: 7841
	private static string svn_branch = null;
}
