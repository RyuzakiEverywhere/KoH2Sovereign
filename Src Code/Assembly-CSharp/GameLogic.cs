using System;
using System.Reflection;
using FMODUnity;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000071 RID: 113
public class GameLogic : MonoBehaviour, IEngine, IListener
{
	// Token: 0x06000431 RID: 1073 RVA: 0x00032DC0 File Offset: 0x00030FC0
	static GameLogic()
	{
		Reflection.fn_resolve_type = new Func<string, Type>(GameLogic.ResolveType);
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00032E0C File Offset: 0x0003100C
	public static Type ResolveType(string type_name)
	{
		return Type.GetType(type_name);
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00032E14 File Offset: 0x00031014
	public static Game Get(bool create = true)
	{
		if (GameLogic.instance == null)
		{
			if (!create)
			{
				return null;
			}
			if (!Application.isPlaying)
			{
				Debug.LogError("Attempting to create game logic in editor!");
				return null;
			}
			if (GameLogic.destroying)
			{
				Debug.LogError("Attempting to create game logic while it is being destroyed!");
			}
			UnityEngine.Time.fixedDeltaTime = 0.05f;
			Physics.autoSimulation = false;
			MainThreadUpdates.Init();
			Profile.Init();
			GameObject gameObject = new GameObject("GameLogic");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			GameLogic.instance = gameObject.AddComponent<GameLogic>();
			GameLogic.instance.logic = new Game("world_view", GameLogic.instance);
			global::SuccessAndFail.Init();
			GameLogic.instance.SetupVars();
			Analytics.Init(GameLogic.instance.logic);
			GameLogic.instance.logic.tutorial_listener = new Tutorial.RulesListener();
		}
		return GameLogic.instance.logic;
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00032EE4 File Offset: 0x000310E4
	public static Reflection.TypeInfo.CreateVisuals CreateCreateVisualsDelegate(Type logic_type)
	{
		Type type = Type.GetType(logic_type.Name);
		while (type == null)
		{
			if (!logic_type.IsClass)
			{
				return Reflection.TypeInfo.no_create_visuals;
			}
			logic_type = logic_type.BaseType;
			if (logic_type == null)
			{
				return Reflection.TypeInfo.no_create_visuals;
			}
			type = Type.GetType(logic_type.Name);
		}
		MethodInfo method = type.GetMethod("CreateVisuals", new Type[]
		{
			typeof(Logic.Object)
		});
		if (method == null || !method.IsStatic || !method.IsPublic || method.ReturnType != typeof(void))
		{
			return Reflection.TypeInfo.no_create_visuals;
		}
		try
		{
			Reflection.TypeInfo.CreateVisuals createVisuals = Delegate.CreateDelegate(typeof(Reflection.TypeInfo.CreateVisuals), method) as Reflection.TypeInfo.CreateVisuals;
			if (createVisuals != null)
			{
				return createVisuals;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error creating CreateVisuals delegate for " + logic_type.FullName + ": " + ex.ToString());
		}
		return Reflection.TypeInfo.no_create_visuals;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00032FEC File Offset: 0x000311EC
	public static Reflection.TypeInfo.CreateVisuals GetCreateVisualsDelegate(Type logic_type)
	{
		Reflection.TypeInfo typeInfo = Reflection.GetTypeInfo(logic_type, null);
		if (typeInfo.create_visuals == null)
		{
			typeInfo.create_visuals = GameLogic.CreateCreateVisualsDelegate(logic_type);
		}
		return typeInfo.create_visuals;
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0003301C File Offset: 0x0003121C
	public static void CreateVisuals(Logic.Object obj)
	{
		if (obj == null)
		{
			return;
		}
		Reflection.TypeInfo.CreateVisuals createVisualsDelegate = GameLogic.GetCreateVisualsDelegate(obj.rtti.type);
		if (createVisualsDelegate == Reflection.TypeInfo.no_create_visuals)
		{
			return;
		}
		string name = "Create " + obj.rtti.name;
		Profile.BeginSection(name);
		GameLogic.in_create_visuals = true;
		try
		{
			createVisualsDelegate(obj);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error creating visuals for " + obj.ToString() + ": " + ex.ToString());
		}
		GameLogic.in_create_visuals = false;
		Profile.EndSection(name);
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x000330B8 File Offset: 0x000312B8
	public static bool LoadScene(string mapName)
	{
		string text = null;
		if (mapName.Equals("title", StringComparison.OrdinalIgnoreCase))
		{
			text = "Title/title";
			RuntimeManager.StudioSystem.setParameterByName("TitleMusicState", 1f, false);
		}
		else if (mapName.Equals("europe", StringComparison.OrdinalIgnoreCase))
		{
			text = "Europe/europe";
		}
		else if (mapName.Equals("test", StringComparison.OrdinalIgnoreCase))
		{
			text = "Test/Michael/Test";
		}
		if (text != null)
		{
			SceneManager.LoadSceneAsync(text);
			return true;
		}
		if (mapName.ToLowerInvariant() == "openfield")
		{
			return false;
		}
		Debug.LogError("LoadScene: unknown map '" + mapName + "'");
		return false;
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00033158 File Offset: 0x00031358
	public static void QuitToTitle()
	{
		MPBoss mpboss = MPBoss.Get();
		if (mpboss != null)
		{
			mpboss.CancelHostMigration();
		}
		LoadingScreen loadingScreen = LoadingScreen.Get();
		if (loadingScreen != null)
		{
			loadingScreen.Show(true, true, true);
		}
		GameLogic gameLogic = GameLogic.instance;
		if (gameLogic != null)
		{
			Game game = gameLogic.logic;
			if (game != null)
			{
				game.Quit();
			}
		}
		GameLogic.LoadScene("title");
		GameLogic.ResetDifficultyToGlobal();
		RichPresence.Update(RichPresence.State.InTitle);
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x000331BA File Offset: 0x000313BA
	private static void ResetDifficultyToGlobal()
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("difficulty");
		setting.LoadSetting();
		setting.value = setting.new_value;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000331D8 File Offset: 0x000313D8
	private static void SetDifficultyTo(string key)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("difficulty");
		if (setting != null)
		{
			setting.value = (setting.new_value = key);
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00033208 File Offset: 0x00031408
	public void OnMessage(object obj, string message, object param)
	{
		Game game = obj as Game;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 2395901619U)
		{
			if (num <= 1078171722U)
			{
				if (num <= 663797166U)
				{
					if (num != 351880791U)
					{
						if (num != 663797166U)
						{
							return;
						}
						if (!(message == "trigger_autosave"))
						{
							return;
						}
						AutoSaveManager.Save(AutoSaveManager.Type.Event, null, param as string);
						return;
					}
					else
					{
						if (!(message == "on_event"))
						{
							return;
						}
						MessageIcon.OnEvent(param as Logic.Event);
						return;
					}
				}
				else if (num != 1022769518U)
				{
					if (num != 1078171722U)
					{
						return;
					}
					if (!(message == "sync_save"))
					{
						return;
					}
					string save_id = param as string;
					if (game.campaign != null && !game.campaign.IsAuthority())
					{
						AutoSaveManager.OnSyncSave(save_id);
					}
					return;
				}
				else
				{
					if (!(message == "loading_map"))
					{
						return;
					}
					string text = param as string;
					if (!SceneManager.GetActiveScene().name.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						GameLogic.LoadScene(text);
					}
					return;
				}
			}
			else if (num <= 1887199645U)
			{
				if (num != 1497642878U)
				{
					if (num != 1887199645U)
					{
						return;
					}
					if (!(message == "game_pause_changed"))
					{
						return;
					}
					GameSpeed.Pause(this.logic.IsPaused());
					this.RefreshMasterVolume();
					return;
				}
				else
				{
					if (!(message == "ai_difficulty_changed"))
					{
						return;
					}
					GameLogic.SetDifficultyTo((string)param);
					return;
				}
			}
			else if (num != 2040518624U)
			{
				if (num != 2385280586U)
				{
					if (num != 2395901619U)
					{
						return;
					}
					if (!(message == "unloading_map"))
					{
						return;
					}
					Tutorial.OnUnloadMap();
					TooltipInstance.RemovePinnedTooltips();
					if (game.type == "world_view")
					{
						SaveGame.current = null;
						VisibilityDetector.ClearAll("WorldView");
					}
					else if (game.type == "battle_view")
					{
						VisibilityDetector.ClearAll("BattleView");
						Troops.ClearSkinningCache();
					}
					else
					{
						VisibilityDetector.ClearAll("");
					}
					if (game.IsMain())
					{
						ViewMode current = ViewMode.current;
						if (current != null)
						{
							current.Deactivate();
						}
						ViewMode.current = null;
					}
					BaseUI baseUI = BaseUI.Get();
					if (baseUI != null)
					{
						baseUI.ClosePanels();
					}
					return;
				}
				else
				{
					if (!(message == "defs_processed"))
					{
						return;
					}
					if (game.defs_map == null && !Title.allLoaded)
					{
						return;
					}
					global::Defs.Get(true).PostProcess();
					return;
				}
			}
			else
			{
				if (!(message == "game_speed_changed"))
				{
					return;
				}
				GameSpeed.OnSetSpeed((float)param);
				return;
			}
		}
		else if (num <= 3272608042U)
		{
			if (num <= 2711999362U)
			{
				if (num != 2633743066U)
				{
					if (num != 2711999362U)
					{
						return;
					}
					if (!(message == "dt_processed"))
					{
						return;
					}
					global::Defs.Get(true).OnDTProcessed();
					return;
				}
				else
				{
					if (!(message == "character_portrait_refresh"))
					{
						return;
					}
					Vars vars = param as Vars;
					Logic.Character character = vars.Get<Logic.Character>("obj", null);
					if (character == null)
					{
						return;
					}
					bool flag = vars.Get<bool>("check_old_valid", false);
					if (DynamicIconBuilder.sm_FaceMap == null || DynamicIconBuilder.sm_FaceMap.Count == 0)
					{
						DynamicIconBuilder.Instance.Init();
					}
					if (character.portrait_variantID != -1 && character.portraitID != -1 && flag)
					{
						DynamicIconBuilder.CharacterData.VariantData variant = DynamicIconBuilder.Instance.GetVariant(character);
						if (variant != null && variant.Validate(character, true))
						{
							return;
						}
					}
					if (vars.Get<bool>("base_too", false) || character.portraitID == -1)
					{
						character.portraitID = DynamicIconBuilder.FindDefaultVariants(character, true);
						if (character.portraitID < 0)
						{
							character.portraitID = DynamicIconBuilder.FindDefaultVariants(character, false);
						}
					}
					character.portrait_variantID = DynamicIconBuilder.FindVariant(character);
					character.portrait_age = PresetRecipes.CharacterToAges(character);
					return;
				}
			}
			else if (num != 2872055667U)
			{
				if (num != 3272608042U)
				{
					return;
				}
				if (!(message == "quitting"))
				{
					return;
				}
				if (game.campaign == null)
				{
					return;
				}
				if (game.campaign.IsMultiplayerCampaign())
				{
					bool flag2;
					if (game == null)
					{
						flag2 = (null != null);
					}
					else
					{
						Logic.Multiplayer multiplayer = game.multiplayer;
						flag2 = (((multiplayer != null) ? multiplayer.playerData : null) != null);
					}
					if (flag2)
					{
						game.pause.AddRequest("QuitPause", game.multiplayer.playerData.pid);
					}
					AutoSaveManager.SaveOnDisconnect();
				}
				else
				{
					AutoSaveManager.Save(AutoSaveManager.Type.Event, null, "quitting");
				}
				SaveGame.WaitSaveComplete();
				return;
			}
			else
			{
				if (!(message == "map_loaded"))
				{
					return;
				}
				WorldUI worldUI = WorldUI.Get();
				if (worldUI != null)
				{
					UILogger uilogger = global::Common.FindChildComponent<UILogger>(worldUI.gameObject, "id_EventLogger");
					if (uilogger == null)
					{
						return;
					}
					uilogger.Clear();
				}
				return;
			}
		}
		else if (num <= 3515634967U)
		{
			if (num != 3448757682U)
			{
				if (num != 3515634967U)
				{
					return;
				}
				if (!(message == "defs_loaded"))
				{
					return;
				}
				global::Defs.Get(true).OnDefsLoaded(game.dt);
				return;
			}
			else
			{
				if (!(message == "joining_map_loaded"))
				{
					return;
				}
				WorldUI worldUI2 = WorldUI.Get();
				if (worldUI2 != null)
				{
					worldUI2.OnJoinGame();
				}
				ViewMode.WorldView.Apply(true);
				return;
			}
		}
		else if (num != 3569334022U)
		{
			if (num != 3639315131U)
			{
				if (num != 4116088480U)
				{
					return;
				}
				if (!(message == "clear_portrait"))
				{
					return;
				}
				Logic.Character character2 = param as Logic.Character;
				if (character2.last_portraitID != -1 && character2.last_variantID != -1 && DynamicIconBuilder.sm_FaceMap != null && DynamicIconBuilder.sm_FaceMap.Count > 0)
				{
					DynamicIconBuilder.CharacterData.VariantData lastVariant = DynamicIconBuilder.Instance.GetLastVariant(character2);
					if (lastVariant != null)
					{
						lastVariant.used_by--;
						if (lastVariant.used_by <= 0)
						{
							lastVariant.used_by = 0;
							lastVariant.ClearSprites();
						}
					}
				}
				return;
			}
			else
			{
				if (!(message == "create_visuals"))
				{
					return;
				}
				GameLogic.CreateVisuals(param as Logic.Object);
				return;
			}
		}
		else
		{
			if (!(message == "updates_per_type"))
			{
				return;
			}
			string text2 = param as string;
			if (text2 == null || (text2 != null && text2.Length == 0) || text2 == "dump")
			{
				Debug.Log(string.Format("Hickup frames: {0}, total wasted time: {1}", GameLogic.hickup_frames, GameLogic.hickup_total_wasted_time));
				this.logic.scheduler.DumpUpdatesPerTypeStats();
				return;
			}
			if (!(text2 == "start"))
			{
				return;
			}
			GameLogic.hickup_frames = 0;
			GameLogic.hickup_total_wasted_time = 0f;
			this.logic.scheduler.StartUpdatesPerTypeStats();
			return;
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x0003384C File Offset: 0x00031A4C
	public void Log(Logic.Object obj, string msg, Game.LogType type)
	{
		UnityEngine.Object context = null;
		if (obj != null)
		{
			msg = "[" + obj.ToString() + "]: " + msg;
			context = (obj.visuals as UnityEngine.Object);
		}
		if (type == Game.LogType.Error)
		{
			Debug.LogError(msg, context);
			return;
		}
		if (type == Game.LogType.Warning)
		{
			Debug.LogWarning(msg, context);
			return;
		}
		Debug.Log(msg, context);
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x000338A4 File Offset: 0x00031AA4
	public string GetTextFile(string name)
	{
		TextAsset textAsset = Assets.Get<TextAsset>(name);
		if (textAsset == null)
		{
			return null;
		}
		return textAsset.text;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x000338CC File Offset: 0x00031ACC
	public byte[] GetBinaryFile(string name)
	{
		TextAsset textAsset = Assets.Get<TextAsset>(name);
		if (textAsset == null)
		{
			return null;
		}
		return textAsset.bytes;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x000338F4 File Offset: 0x00031AF4
	private void OnApplicationQuit()
	{
		if (this.logic != null)
		{
			this.logic.state = Game.State.Quitting;
		}
		AutoSaveManager.Save(AutoSaveManager.Type.Event, null, "quitting");
		MPBoss.OnQuit();
		SaveGame.WaitSaveComplete();
		THQNORequest.Disconnect();
		Logic.Coroutine.TerminateAll(null);
		if (this.logic == null)
		{
			return;
		}
		this.logic.Quit();
		Analytics.OnAppQuit();
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00033950 File Offset: 0x00031B50
	private void OnDestroy()
	{
		GameLogic.destroying = true;
		if (this.logic != null)
		{
			this.logic.Destroy(false);
			this.logic = null;
		}
		if (GameLogic.instance == this)
		{
			GameLogic.instance = null;
		}
		GameLogic.destroying = false;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0003398C File Offset: 0x00031B8C
	private void Start()
	{
		if (!global::Defs.InTitle())
		{
			this.logic.Start();
		}
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000339A0 File Offset: 0x00031BA0
	private void Update()
	{
		if (!GameLogic.developer_console_enabled && Debug.developerConsoleVisible)
		{
			Debug.developerConsoleVisible = false;
		}
		float unscaledTime = UnityEngine.Time.unscaledTime;
		Shader.SetGlobalVector("_UnscaledTime", new Vector4(unscaledTime / 20f, unscaledTime, unscaledTime * 2f, unscaledTime * 3f));
		MPBoss.OnUpdate();
		MainThreadUpdates.Update();
		if (UnityEngine.Time.deltaTime > UnityEngine.Time.maximumDeltaTime * UnityEngine.Time.timeScale * 0.99f)
		{
			float num = UnityEngine.Time.unscaledDeltaTime * UnityEngine.Time.timeScale - UnityEngine.Time.deltaTime;
			GameLogic.hickup_frames++;
			GameLogic.hickup_total_wasted_time += num;
		}
		if (RelationUtils.validators.Count != 0)
		{
			Debug.LogError("Relation validators count over 0: " + RelationUtils.validators.Count);
			RelationUtils.validators.Clear();
		}
		if (this.logic != null)
		{
			this.logic.Update(UnityEngine.Time.deltaTime, UnityEngine.Time.unscaledDeltaTime);
		}
		Analytics.OnUpdate();
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00033A8E File Offset: 0x00031C8E
	private void OnApplicationFocus(bool hasFocus)
	{
		this.HasFocus = hasFocus;
		this.RefreshMasterVolume();
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00033A9D File Offset: 0x00031C9D
	private void OnApplicationPause(bool pauseStatus)
	{
		this.OnApplicationFocus(!pauseStatus);
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000445 RID: 1093 RVA: 0x00033AA9 File Offset: 0x00031CA9
	// (set) Token: 0x06000446 RID: 1094 RVA: 0x00033AB1 File Offset: 0x00031CB1
	public bool HasFocus { get; protected set; }

	// Token: 0x06000447 RID: 1095 RVA: 0x00033ABC File Offset: 0x00031CBC
	public void RefreshMasterVolume()
	{
		if (!GameLogic.duckAudioOnFocusLost)
		{
			return;
		}
		if (this.HasFocus)
		{
			UserSettings.ForceTemporaryMasterVolume(UserSettings.MasterVolume);
			return;
		}
		float b = UserSettings.MasterOn ? UserSettings.MasterVolume : 0f;
		float num = UserSettings.VolumeOutOfFocusPausedOn ? Mathf.Min(UserSettings.VolumeOutOfFocusPaused, b) : 0f;
		float num2 = UserSettings.VolumeOutOfFocusOn ? Mathf.Min(UserSettings.VolumeOutOfFocus, b) : 0f;
		float value = num;
		if (this.logic != null && this.logic.state == Game.State.Running && !this.logic.IsPaused())
		{
			value = num2;
		}
		UserSettings.ForceTemporaryMasterVolume(value);
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00033B58 File Offset: 0x00031D58
	private object krel()
	{
		Logic.Kingdom k = BaseUI.LogicKingdom();
		Logic.Kingdom k2 = BaseUI.SelKingdom();
		return KingdomAndKingdomRelation.Get(k, k2, true, false);
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00033B78 File Offset: 0x00031D78
	private void SetupVars()
	{
		Vars vars = this.logic.vars;
		vars.Set<Vars.Func0>("ui", new Vars.Func0(BaseUI.Get));
		vars.Set<Vars.Func0>("world_ui", new Vars.Func0(WorldUI.Get));
		vars.Set<Vars.Func0>("plr_kingdom", new Vars.Func0(BaseUI.LogicKingdom));
		vars.Set<Vars.Func0>("sel", new Vars.Func0(BaseUI.SelLO));
		vars.Set<Vars.Func0>("ttobj", new Vars.Func0(BaseUI.TTObj));
		vars.Set<Vars.Func0>("ttvars", new Vars.Func0(BaseUI.TTVars));
		vars.Set<Vars.Func0>("link_obj", new Vars.Func0(BaseUI.LinkObj));
		vars.Set<Vars.Func0>("link_vars", new Vars.Func0(BaseUI.LinkVars));
		vars.Set<Vars.Func0>("alt", new Vars.Func0(BaseUI.Alt));
		vars.Set<Vars.Func0>("ctrl", new Vars.Func0(BaseUI.Ctrl));
		vars.Set<Vars.Func0>("sel_kingdom", new Vars.Func0(BaseUI.SelKingdom));
		vars.Set<Vars.Func0>("sel_char", new Vars.Func0(BaseUI.SelChar));
		vars.Set<Vars.Func0>("map", new Vars.Func0(MapData.Get));
		vars.Set<Vars.Func0>("world_map", new Vars.Func0(WorldMap.Get));
		vars.Set<Vars.Func0>("pope", new Vars.Func0(global::Religions.GetPope));
		vars.Set<Vars.Func0>("patriarch", new Vars.Func0(global::Religions.GetPatriarch));
		vars.Set<Vars.Func0>("crusade", new Vars.Func0(global::Religions.GetCrusade));
		vars.Set<Vars.Func0>("piety_icon", new Vars.Func0(BaseUI.GetPietyIcon));
		vars.Set<Vars.Func1>("fgo", new Vars.Func1(DevCheats.FindGameObjectByName));
		vars.Set<Vars.Func0>("cur_save", new Vars.Func0(SaveGame.GetCurrent));
		vars.Set<Vars.Func0>("mpboss", new Vars.Func0(MPBoss.Get));
		vars.Set<Vars.Func0>("keybindings", new Vars.Func0(KeyBindings.DefsLocalizer.Get));
		vars.Set<Vars.Func0>("krel", new Vars.Func0(this.krel));
	}

	// Token: 0x04000443 RID: 1091
	public static GameLogic instance = null;

	// Token: 0x04000444 RID: 1092
	public Game logic;

	// Token: 0x04000445 RID: 1093
	public static bool in_create_visuals = false;

	// Token: 0x04000446 RID: 1094
	public static bool developer_console_enabled = false;

	// Token: 0x04000447 RID: 1095
	private static bool destroying = false;

	// Token: 0x04000448 RID: 1096
	public static int hickup_frames = 0;

	// Token: 0x04000449 RID: 1097
	public static float hickup_total_wasted_time = 0f;

	// Token: 0x0400044B RID: 1099
	public static bool duckAudioOnFocusLost = true;

	// Token: 0x0200053F RID: 1343
	public abstract class Behaviour : MonoBehaviour, IListener
	{
		// Token: 0x06004365 RID: 17253
		public abstract Logic.Object GetLogic();

		// Token: 0x06004366 RID: 17254 RVA: 0x000448AF File Offset: 0x00042AAF
		public static GameLogic.Behaviour Get(int nid)
		{
			return null;
		}

		// Token: 0x06004367 RID: 17255 RVA: 0x001FD6F0 File Offset: 0x001FB8F0
		public virtual void DeleteObject()
		{
			Logic.Object logic = this.GetLogic();
			if (logic is Logic.Army)
			{
				if (!logic.IsAuthority())
				{
					logic.SendEvent(new Logic.Object.DeleteObjectCustomEvent(logic));
					return;
				}
				this.GetLogic().Destroy(false);
			}
			if (logic is Logic.Squad)
			{
				logic.Destroy(false);
			}
		}

		// Token: 0x06004368 RID: 17256
		public abstract void OnMessage(object obj, string message, object param);

		// Token: 0x04002F9E RID: 12190
		public int nid;
	}
}
