using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200022E RID: 558
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x060021CB RID: 8651 RVA: 0x00131E50 File Offset: 0x00130050
	public static LoadingScreen Get()
	{
		if (LoadingScreen.sm_Instance == null)
		{
			DT.Field defField = global::Defs.GetDefField("LoadingScreen", null);
			if (defField != null)
			{
				GameObject obj = global::Defs.GetObj<GameObject>(defField, "prefab", null);
				if (obj != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj);
					LoadingScreen component = gameObject.GetComponent<LoadingScreen>();
					if (component != null)
					{
						LoadingScreen.sm_Instance = component;
						LoadingScreen.sm_Instance.gameObject.SetActive(false);
						UnityEngine.Object.DontDestroyOnLoad(LoadingScreen.sm_Instance.gameObject);
						if (LoadingScreen.snapshot == null)
						{
							LoadingScreen.snapshot = new FMODWrapper.Snapshot("loading_screen_snapshot");
						}
					}
					else
					{
						Debug.Log("LoadingScreen: provided prefab is not eligable");
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				else
				{
					Debug.Log("LoadingScreen: fail to find def field \"LoadingScreen\"");
				}
			}
		}
		return LoadingScreen.sm_Instance;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x00131F0C File Offset: 0x0013010C
	public void Show(bool shown, bool new_game = true, bool isQuitting = false)
	{
		if (Logic.Multiplayer.LogEnabled(2))
		{
			Logic.Multiplayer.Log(string.Format("LoadingScreen.Show({0}); new_game = {1}; isQuitting = {2}", shown, new_game, isQuitting), 2, Game.LogType.Message);
		}
		THQNORequest.loadingScreenShown = shown;
		if (GameLogic.Get(false) == null)
		{
			LoadingScreen.sm_Instance.gameObject.SetActive(false);
			FMODWrapper.Snapshot snapshot = LoadingScreen.snapshot;
			if (snapshot == null)
			{
				return;
			}
			snapshot.EndSnapshot();
			return;
		}
		else
		{
			LoadingScreen.sm_Instance.gameObject.SetActive(shown);
			if (!shown)
			{
				FMODWrapper.Snapshot snapshot2 = LoadingScreen.snapshot;
				if (snapshot2 != null)
				{
					snapshot2.EndSnapshot();
				}
				GameLogic instance = GameLogic.instance;
				if (instance != null)
				{
					instance.RefreshMasterVolume();
				}
				BackgroundMusic.Reset();
				return;
			}
			Image component = global::Common.GetComponent<Image>(base.gameObject, "id_Illustration");
			if (component != null)
			{
				Sprite randomObj;
				if (BattleViewLoader.sm_InTrasition)
				{
					randomObj = global::Defs.GetRandomObj<Sprite>("LoadingScreen", "background.battleview", null);
				}
				else
				{
					randomObj = global::Defs.GetRandomObj<Sprite>("LoadingScreen", "background.worldview", null);
				}
				if (randomObj != null)
				{
					component.overrideSprite = randomObj;
				}
			}
			FMODVoiceProvider.ClearAllVoices();
			FMODWrapper.Snapshot snapshot3 = LoadingScreen.snapshot;
			if (snapshot3 == null)
			{
				return;
			}
			snapshot3.StartSnapshot();
			return;
		}
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x00132018 File Offset: 0x00130218
	private void ShowTitleText(bool shown, bool new_game, bool isQuitting)
	{
		Game game = GameLogic.Get(false);
		if (shown)
		{
			string text = string.Empty;
			if (isQuitting)
			{
				text = "LoadingScreen.return_to_title";
			}
			else if (!game.campaign.IsMultiplayerCampaign())
			{
				if (new_game)
				{
					text = "LoadingScreen.new_game";
				}
				else
				{
					text = "LoadingScreen.load_game";
				}
			}
			else if (new_game)
			{
				text = "LoadingScreen.multiplayer_new_game";
			}
			else
			{
				text = "LoadingScreen.multiplayer_load_game";
			}
			this.SetText(text);
			return;
		}
		LoadingScreen.sm_Instance.gameObject.SetActive(shown);
		if (Game.fullGameStateReceived && game.campaign.IsMultiplayerCampaign())
		{
			CampaignUtils.SetCampaignVarGameLoaded(game);
		}
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x001320A4 File Offset: 0x001302A4
	public void SetText(string key)
	{
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(LoadingScreen.sm_Instance.gameObject, "id_Text");
		if (textMeshProUGUI != null)
		{
			UIText.SetTextKey(textMeshProUGUI, key, null, null);
		}
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x001320D8 File Offset: 0x001302D8
	public static bool IsShown()
	{
		return !(LoadingScreen.sm_Instance == null) && LoadingScreen.sm_Instance.gameObject.activeInHierarchy;
	}

	// Token: 0x040016AC RID: 5804
	private static LoadingScreen sm_Instance;

	// Token: 0x040016AD RID: 5805
	private static FMODWrapper.Snapshot snapshot;
}
