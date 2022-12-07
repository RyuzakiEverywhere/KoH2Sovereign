using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001FE RID: 510
public class UIEndGameWindow : MonoBehaviour
{
	// Token: 0x06001F1E RID: 7966 RVA: 0x00120744 File Offset: 0x0011E944
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.window_def = global::Defs.GetDefField("EndGameWindow", null);
		if (this.m_QuitToMainMenu != null)
		{
			this.m_QuitToMainMenu.onClick = new BSGButton.OnClick(this.HanldeOnQuitToTitle);
		}
		if (this.m_PickKingdom != null)
		{
			this.m_PickKingdom.onClick = new BSGButton.OnClick(this.HandleOnPickKingdom);
		}
		if (this.m_ContinuePlaying != null)
		{
			this.m_ContinuePlaying.onClick = new BSGButton.OnClick(this.HandleOnContinuePlaying);
		}
		if (this.m_AdmitDefeat != null)
		{
			this.m_AdmitDefeat.onClick = new BSGButton.OnClick(this.HandleOnAdmitDefeat);
		}
		if (this.m_QuitAndRepickLater != null)
		{
			this.m_QuitAndRepickLater.onClick = new BSGButton.OnClick(this.HanldeOnQuitAndRepickLater);
		}
		this.snapshot = new FMODWrapper.Snapshot("victory_defeat_screen_snapshot");
		this.m_Initialzied = true;
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x00120842 File Offset: 0x0011EA42
	private void Awake()
	{
		if (UIEndGameWindow.instance != null)
		{
			UnityEngine.Object.Destroy(UIEndGameWindow.instance.gameObject);
		}
		UIEndGameWindow.instance = this;
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x00120868 File Offset: 0x0011EA68
	private void OnDestroy()
	{
		if (this.m_QuitToMainMenu != null)
		{
			this.m_QuitToMainMenu.onClick = null;
		}
		if (this.m_PickKingdom != null)
		{
			this.m_PickKingdom.onClick = null;
		}
		if (this.m_ContinuePlaying != null)
		{
			this.m_ContinuePlaying.onClick = null;
		}
		if (this.m_AdmitDefeat != null)
		{
			this.m_AdmitDefeat.onClick = null;
		}
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x001208DD File Offset: 0x0011EADD
	private void OnEnable()
	{
		Tutorial.SupressTutorials(true);
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000FC3D8 File Offset: 0x000FA5D8
	private void OnDisable()
	{
		Tutorial.SupressTutorials(false);
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x001208E8 File Offset: 0x0011EAE8
	private void Start()
	{
		GameObject gameObject = GameObject.Find("id_Player");
		if (gameObject != null)
		{
			this.multiplayer = gameObject.GetComponent<global::Multiplayer>();
		}
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x00120915 File Offset: 0x0011EB15
	public static bool EndGameShown()
	{
		return !(UIEndGameWindow.instance == null) && !(UIEndGameWindow.instance.gameObject == null) && UIEndGameWindow.instance.gameObject.activeInHierarchy;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x0012094C File Offset: 0x0011EB4C
	private void Show(bool show)
	{
		base.gameObject.SetActive(show);
		base.transform.SetAsLastSibling();
		if (show)
		{
			FMODWrapper.Snapshot snapshot = this.snapshot;
			if (snapshot != null)
			{
				snapshot.StartSnapshot();
			}
			TooltipInstance.RemovePinnedTooltips();
			return;
		}
		FMODWrapper.Snapshot snapshot2 = this.snapshot;
		if (snapshot2 != null)
		{
			snapshot2.EndSnapshot();
		}
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			return;
		}
		Pause pause = game.pause;
		if (pause == null)
		{
			return;
		}
		pause.DelRequest("EndGamePause", -2);
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x001209BC File Offset: 0x0011EBBC
	private static void LoadPrefab()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			GameObject gameObject = global::Common.FindChildByName(worldUI.tCanvas.gameObject, "id_MessageContainer", true, true);
			UIEndGameWindow.instance = UICommon.LoadPrefab("EndGameWindow", gameObject.transform).GetComponent<UIEndGameWindow>();
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x00120A0C File Offset: 0x0011EC0C
	public static bool ShowDialog(string reason, DT.Field voice_line, bool is_victory)
	{
		if (UIEndGameWindow.instance == null)
		{
			UIEndGameWindow.LoadPrefab();
		}
		if (UIEndGameWindow.instance != null)
		{
			UIEndGameWindow.instance.Init();
			UIEndGameWindow.instance.RebuildLayout(is_victory, reason);
			UIEndGameWindow.instance.Show(true);
			if (voice_line != null)
			{
				Vars vars = new Vars();
				vars.Set<string>("reason", reason);
				vars.Set<string>("result", is_victory ? "victory" : "defeat");
				BaseUI.PlayVoiceEvent(voice_line, vars);
			}
			if (UIEndGameWindow.onShow != null)
			{
				UIEndGameWindow.onShow(UIEndGameWindow.instance);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x00120AA8 File Offset: 0x0011ECA8
	private void RebuildLayout(bool isVictory, string reason)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		if (this.m_TitleLabel != null)
		{
			UIText.SetText(this.m_TitleLabel, global::Defs.Localize(this.window_def, isVictory ? "caption.victory" : "caption.defeat", null, null, true, true));
		}
		if (this.m_PickKingdomLabel != null)
		{
			UIText.SetText(this.m_PickKingdomLabel, global::Defs.Localize(this.window_def, "pick_kingdom", null, null, true, true));
		}
		if (this.m_ContinuePlayingLabel != null)
		{
			UIText.SetText(this.m_ContinuePlayingLabel, global::Defs.Localize(this.window_def, "continue", null, null, true, true));
		}
		Logic.Multiplayer multiplayer = game.multiplayer;
		Logic.Kingdom kingdom = (multiplayer != null) ? multiplayer.GetPlayerKingdom() : null;
		if (this.m_QuitToMainMenuLabel != null)
		{
			UIText.SetText(this.m_QuitToMainMenuLabel, global::Defs.Localize(this.window_def, "quit_sp", null, null, true, true));
		}
		if (this.m_QuitAndRepickLaterLabel != null)
		{
			UIText.SetText(this.m_QuitAndRepickLaterLabel, global::Defs.Localize(this.window_def, "quit_mp", null, null, true, true));
		}
		if (this.m_AdmitDefeatLabel != null)
		{
			UIText.SetText(this.m_AdmitDefeatLabel, global::Defs.Localize(this.window_def, "admit_defeat", null, null, true, true));
		}
		if (this.m_UpperBound != null)
		{
			this.m_UpperBound.overrideSprite = global::Defs.GetObj<Sprite>(this.window_def, isVictory ? "upper_border.victory" : "upper_border.defeat", null);
		}
		if (this.m_LowerBound != null)
		{
			this.m_LowerBound.overrideSprite = global::Defs.GetObj<Sprite>(this.window_def, isVictory ? "lower_border.victory" : "lower_border.defeat", null);
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = this.GetOutcomeIllustration(reason, isVictory);
		}
		if (this.m_Description != null)
		{
			UIText.SetText(this.m_Description, this.GetOutcomeText(reason, isVictory));
		}
		if (((game != null) ? game.campaign : null) != null)
		{
			bool flag = kingdom.IsDefeated();
			if (this.m_ContinuePlaying != null)
			{
				if (isVictory)
				{
					this.m_ContinuePlaying.gameObject.SetActive(reason != "Conquest");
				}
				else
				{
					this.m_ContinuePlaying.gameObject.SetActive(!flag);
				}
			}
			if (this.m_PickKingdom != null)
			{
				bool active = !isVictory && game.IsMultiplayer() && kingdom.IsDefeated() && game.rules.PlayerCanRepick(kingdom);
				BSGButton pickKingdom = this.m_PickKingdom;
				if (pickKingdom != null)
				{
					pickKingdom.gameObject.SetActive(active);
				}
			}
			if (this.m_AdmitDefeat != null)
			{
				bool active2 = !isVictory && game.IsMultiplayer();
				BSGButton admitDefeat = this.m_AdmitDefeat;
				if (admitDefeat != null)
				{
					admitDefeat.gameObject.SetActive(active2);
				}
			}
			if (this.m_QuitToMainMenu != null)
			{
				bool active3 = isVictory || !game.IsMultiplayer();
				BSGButton quitToMainMenu = this.m_QuitToMainMenu;
				if (quitToMainMenu != null)
				{
					quitToMainMenu.gameObject.SetActive(active3);
				}
			}
			if (this.m_QuitAndRepickLater != null)
			{
				bool active4 = !isVictory && game.IsMultiplayer() && kingdom.IsDefeated() && game.rules.PlayerCanRepick(kingdom);
				BSGButton quitAndRepickLater = this.m_QuitAndRepickLater;
				if (quitAndRepickLater == null)
				{
					return;
				}
				quitAndRepickLater.gameObject.SetActive(active4);
				return;
			}
		}
		else
		{
			if (this.m_PickKingdom != null)
			{
				this.m_PickKingdom.gameObject.SetActive(false);
			}
			if (this.m_ContinuePlaying != null)
			{
				this.m_ContinuePlaying.gameObject.SetActive(false);
			}
			if (this.m_AdmitDefeat == null)
			{
				this.m_AdmitDefeat.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x00120E4C File Offset: 0x0011F04C
	private Sprite GetOutcomeIllustration(string reason, bool isVictory)
	{
		string text = isVictory ? "victory" : "defeat";
		string path = string.Concat(new string[]
		{
			"outcomes.",
			reason,
			".",
			text,
			"_illustration"
		});
		DT.Field field = this.window_def.FindChild(path, null, true, true, true, '.');
		if (field == null)
		{
			path = "outcomes.Defaul." + text + "_illustration";
			field = this.window_def.FindChild(path, null, true, true, true, '.');
		}
		return global::Defs.GetObj<Sprite>(field, null);
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x00120ED8 File Offset: 0x0011F0D8
	private string GetOutcomeText(string reason, bool isVictory)
	{
		string text = isVictory ? "victory" : "defeat";
		string path = string.Concat(new string[]
		{
			"outcomes.",
			reason,
			".",
			text,
			"_description"
		});
		DT.Field field = this.window_def.FindChild(path, null, true, true, true, '.');
		if (field == null)
		{
			path = "outcomes.Defaul." + text + "_description";
			field = this.window_def.FindChild(path, null, true, true, true, '.');
		}
		return global::Defs.Localize(field, new Vars(BaseUI.LogicKingdom()), null, true, true);
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x00120F74 File Offset: 0x0011F174
	private void HandleOnPickKingdom(BSGButton but)
	{
		FMODVoiceProvider.ClearAllVoices();
		Game game = GameLogic.Get(false);
		BaseUI baseUI = BaseUI.Get();
		ViewMode.Kingdoms.Apply(true);
		Vector3 terrainSize = baseUI.GetTerrainSize();
		baseUI.LookAt(terrainSize / 2f, false);
		UITargetSelectWindow.ShowDialog(TargetPickerData.Create(game.rules.GetPickableKingdoms(), null, null), (baseUI != null) ? baseUI.selected_logic_obj : null, delegate(Value target)
		{
			Logic.Kingdom kNew = (Logic.Kingdom)target.obj_val;
			if (game.rules.IsAvailableForPicking(kNew, -1))
			{
				game.SetKingdom(kNew, true);
				game.NotifyListeners("local_player_repicked", null);
				UIEndGameWindow.Close();
			}
		}, delegate()
		{
		}, null, null, null, null, "KingdomTargetPicker", "");
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x0012102A File Offset: 0x0011F22A
	private void HandleOnContinuePlaying(BSGButton btn)
	{
		FMODVoiceProvider.ClearAllVoices();
		this.HandleClose(btn);
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.NotifyListeners("local_player_continue_playing", null);
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x0012104E File Offset: 0x0011F24E
	private void HanldeOnQuitToTitle(BSGButton but)
	{
		FMODVoiceProvider.ClearAllVoices();
		this.Show(false);
		GameLogic.QuitToTitle();
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x0012104E File Offset: 0x0011F24E
	private void HanldeOnQuitAndRepickLater(BSGButton but)
	{
		FMODVoiceProvider.ClearAllVoices();
		this.Show(false);
		GameLogic.QuitToTitle();
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x00121064 File Offset: 0x0011F264
	private void HandleOnAdmitDefeat(BSGButton but)
	{
		FMODVoiceProvider.ClearAllVoices();
		this.Show(false);
		Game game = GameLogic.Get(false);
		if (game != null && game.rules != null)
		{
			Logic.Kingdom playerKingdom = game.multiplayer.GetPlayerKingdom();
			if (playerKingdom.IsDefeated())
			{
				game.rules.SetPlayerEliminated(playerKingdom);
			}
		}
		GameLogic.QuitToTitle();
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x001210B4 File Offset: 0x0011F2B4
	private void HandleClose(BSGButton button)
	{
		GameLogic.Get(false);
		UIEndGameWindow.Close();
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x001210C2 File Offset: 0x0011F2C2
	public static void Close()
	{
		if (UIEndGameWindow.instance != null)
		{
			UIEndGameWindow.instance.Show(false);
		}
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x001210DC File Offset: 0x0011F2DC
	private static DT.Field FindVoiceLine(string path, string reason = null, bool allow_default = true)
	{
		DT.Field field = BaseUI.soundsDef.FindChild(path + "_" + reason, null, true, true, true, '.');
		if (field != null)
		{
			return field;
		}
		if (allow_default)
		{
			return BaseUI.soundsDef.FindChild(path, null, true, true, true, '.');
		}
		return null;
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x00121121 File Offset: 0x0011F321
	public static bool ShowVictoryWindow(string reason = null)
	{
		return UIEndGameWindow.ShowDialog(reason, UIEndGameWindow.FindVoiceLine("EndGame_victory", reason, true), true);
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x00121136 File Offset: 0x0011F336
	public static bool ShowDefeatWindow(string reason = null)
	{
		return UIEndGameWindow.ShowDialog(reason, UIEndGameWindow.FindVoiceLine("EndGame_defeat", reason, true), false);
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x00121136 File Offset: 0x0011F336
	public static bool ShowDrawWindow(string reason = null)
	{
		return UIEndGameWindow.ShowDialog(reason, UIEndGameWindow.FindVoiceLine("EndGame_defeat", reason, true), false);
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x0012114B File Offset: 0x0011F34B
	public static bool IsActive()
	{
		return UIEndGameWindow.instance != null && UIEndGameWindow.instance.isActiveAndEnabled;
	}

	// Token: 0x0400149B RID: 5275
	[UIFieldTarget("id_QuitToMainMenu")]
	private BSGButton m_QuitToMainMenu;

	// Token: 0x0400149C RID: 5276
	[UIFieldTarget("id_PickKingdom")]
	private BSGButton m_PickKingdom;

	// Token: 0x0400149D RID: 5277
	[UIFieldTarget("id_ContinuePlaying")]
	private BSGButton m_ContinuePlaying;

	// Token: 0x0400149E RID: 5278
	[UIFieldTarget("id_AdmitDefeat")]
	private BSGButton m_AdmitDefeat;

	// Token: 0x0400149F RID: 5279
	[UIFieldTarget("id_QuitAndRepickLater")]
	private BSGButton m_QuitAndRepickLater;

	// Token: 0x040014A0 RID: 5280
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI m_TitleLabel;

	// Token: 0x040014A1 RID: 5281
	[UIFieldTarget("id_PickKingdomLabel")]
	private TextMeshProUGUI m_PickKingdomLabel;

	// Token: 0x040014A2 RID: 5282
	[UIFieldTarget("id_ContinuePlayingLabel")]
	private TextMeshProUGUI m_ContinuePlayingLabel;

	// Token: 0x040014A3 RID: 5283
	[UIFieldTarget("id_QuitToMainMenuLabel")]
	private TextMeshProUGUI m_QuitToMainMenuLabel;

	// Token: 0x040014A4 RID: 5284
	[UIFieldTarget("id_AdmitDefeatLabel")]
	private TextMeshProUGUI m_AdmitDefeatLabel;

	// Token: 0x040014A5 RID: 5285
	[UIFieldTarget("id_QuitAndRepickLaterLabel")]
	private TextMeshProUGUI m_QuitAndRepickLaterLabel;

	// Token: 0x040014A6 RID: 5286
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x040014A7 RID: 5287
	[UIFieldTarget("id_UpperBound")]
	private Image m_UpperBound;

	// Token: 0x040014A8 RID: 5288
	[UIFieldTarget("id_LowerBound")]
	private Image m_LowerBound;

	// Token: 0x040014A9 RID: 5289
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x040014AA RID: 5290
	public static Action<UIEndGameWindow> onShow;

	// Token: 0x040014AB RID: 5291
	private static UIEndGameWindow instance;

	// Token: 0x040014AC RID: 5292
	private global::Multiplayer multiplayer;

	// Token: 0x040014AD RID: 5293
	private bool m_Initialzied;

	// Token: 0x040014AE RID: 5294
	private DT.Field window_def;

	// Token: 0x040014AF RID: 5295
	private FMODWrapper.Snapshot snapshot;
}
