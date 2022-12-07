using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000128 RID: 296
public static class BattleViewLoader
{
	// Token: 0x06000DB8 RID: 3512 RVA: 0x00099C48 File Offset: 0x00097E48
	public static string GetBattleSceneName(Logic.Battle battle)
	{
		return "OpenField";
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00099C50 File Offset: 0x00097E50
	static BattleViewLoader()
	{
		SceneManager.sceneLoaded += BattleViewLoader.OnSceneLoadedHandle;
		SceneManager.sceneUnloaded += BattleViewLoader.OnSceneUnloadedHandler;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00099CDC File Offset: 0x00097EDC
	private static void OnSceneLoadedHandle(Scene scene, LoadSceneMode mode)
	{
		BattleViewLoader.BattleData battleData = BattleViewLoader.currentBatte;
		if (((battleData != null) ? battleData.MapName : null) != null)
		{
			BattleViewLoader.BattleData battleData2 = BattleViewLoader.currentBatte;
			if (((battleData2 != null) ? battleData2.MapName.ToLowerInvariant() : null) == scene.name.ToLowerInvariant())
			{
				BattleViewLoader.CurrentBattleView = scene;
				SceneManager.SetActiveScene(BattleViewLoader.CurrentBattleView);
			}
		}
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00099D36 File Offset: 0x00097F36
	private static void OnSceneUnloadedHandler(Scene scene)
	{
		if (scene == BattleViewLoader.CurrentBattleView)
		{
			BattleViewLoader.ReactiveWorldView();
		}
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00099D4C File Offset: 0x00097F4C
	public static void InitBattleLoad(Logic.Battle battleLogic, int kingdomId, int side, string map_name)
	{
		if (BattleViewLoader.sm_InTrasition)
		{
			return;
		}
		BattleViewLoader.BattleData battleData = new BattleViewLoader.BattleData();
		battleData.BattleLogic = battleLogic;
		battleData.KingdomId = kingdomId;
		battleData.Side = side;
		battleData.MapName = map_name;
		BattleViewLoader.BattleData battleData2 = BattleViewLoader.currentBatte;
		battleData.oldMapName = ((battleData2 != null) ? battleData2.MapName : null);
		BattleViewLoader.currentBatte = battleData;
		BattleViewLoader.sm_InTrasition = true;
		BattleViewLoadingScreen.OnState += BattleViewLoader.HandleEnterBattleStates;
		BattleViewLoadingScreen.DoEnterBattle(battleLogic, null, BattleViewLoader.m_isFadeOutDisabled);
		BattleViewLoader.m_isFadeOutDisabled = false;
		BattleViewLoader.m_loadingScreenSprite = null;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x00099DCD File Offset: 0x00097FCD
	public static string GetFullSceneName(string scene_name)
	{
		if (scene_name == "battleview")
		{
			return "BattleView/BattleViewScenes/BattleView";
		}
		return scene_name;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00099DE4 File Offset: 0x00097FE4
	public static void LoadBattle(Logic.Battle battleLogic, int kingdomId)
	{
		BattleMap.SetBattle(battleLogic, kingdomId);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(BattleViewLoader.GetFullSceneName(BattleViewLoader.currentBatte.MapName), LoadSceneMode.Additive);
		if (asyncOperation == null)
		{
			Debug.Log("Fail to find scene: " + BattleViewLoader.currentBatte.MapName + " returning to World View");
			BattleViewLoader.FallBackToWorldView();
			return;
		}
		Action<AsyncOperation> handler = null;
		handler = delegate(AsyncOperation op)
		{
			if (op.progress >= 1f)
			{
				BattleFieldOverview battleFieldOverview = BattleFieldOverview.Get();
				if (battleFieldOverview != null)
				{
					battleFieldOverview.Prepair();
				}
				GameCamera gameCamera = CameraController.GameCamera;
				if (gameCamera != null)
				{
					gameCamera.Camera.enabled = false;
				}
				else
				{
					Debug.LogError("BattleFieldOverview not found On Prepair Phase !!!");
				}
				op.completed -= handler;
			}
		};
		asyncOperation.completed += handler;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00099E60 File Offset: 0x00098060
	private static void HandleEnterBattleStates(BattleViewLoadingScreen.TransitionState state)
	{
		if (state == BattleViewLoadingScreen.TransitionState.InProgress)
		{
			BattleViewLoader.DisableWorldView();
			if (BattleViewLoader.currentBatte != null)
			{
				BattleViewLoader.currentBatte.BattleLogic.DoAction("enter_battle", BattleViewLoader.currentBatte.Side, BattleViewLoader.currentBatte.MapName);
			}
		}
		if (state == BattleViewLoadingScreen.TransitionState.Completed)
		{
			BattleFieldOverview battleFieldOverview = BattleFieldOverview.Get();
			if (battleFieldOverview != null)
			{
				battleFieldOverview.BeginBattleOverview();
			}
			else
			{
				Debug.LogError("BattleFieldOverview not found !!!");
				BattleViewUI battleViewUI = BattleViewUI.Get();
				if (battleViewUI != null)
				{
					battleViewUI.Show(true, false);
				}
			}
			BattleViewLoader.sm_InTrasition = false;
			BattleViewLoadingScreen.OnState -= BattleViewLoader.HandleEnterBattleStates;
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00099EF8 File Offset: 0x000980F8
	private static void FallBackToWorldView()
	{
		BattleViewLoadingScreen.OnState -= BattleViewLoader.HandleEnterBattleStates;
		BattleViewLoader.sm_InTrasition = true;
		BattleViewLoadingScreen.DoReturnToWorld();
		BattleViewLoadingScreen.OnState += BattleViewLoader.HanldeExitBattleStates;
		if (BattleViewLoader.currentBatte != null)
		{
			BattleViewLoader.currentBatte.BattleLogic.DoAction("leave_battle", BattleViewLoader.currentBatte.Side, BattleViewLoader.currentBatte.MapName);
		}
		BattleViewLoader.ReactiveWorldView();
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00099F66 File Offset: 0x00098166
	public static void SetLoadingScreenSprite(Sprite sprite)
	{
		BattleViewLoader.m_loadingScreenSprite = sprite;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00099F6E File Offset: 0x0009816E
	public static void DisableFadeOut()
	{
		BattleViewLoader.m_isFadeOutDisabled = true;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00099F76 File Offset: 0x00098176
	public static void ExitBatte()
	{
		if (BattleViewLoader.currentBatte != null)
		{
			BattleViewLoader.ExitBatte(BattleViewLoader.currentBatte.BattleLogic);
		}
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00099F90 File Offset: 0x00098190
	public static void ExitBatte(Logic.Battle battle)
	{
		if (BattleViewLoader.sm_InTrasition)
		{
			return;
		}
		if (battle == null)
		{
			return;
		}
		BattleViewLoader.sm_InTrasition = true;
		BattleViewLoader.currentBatte = null;
		if (BattleViewLoader.CurrentBattleView.IsValid() && BattleViewLoader.CurrentBattleView.isLoaded)
		{
			BattleViewLoadingScreen.OnState += BattleViewLoader.HanldeExitBattleStates;
			BattleViewLoadingScreen.DoExitBattle(battle, BattleViewLoader.m_loadingScreenSprite, BattleViewLoader.m_isFadeOutDisabled);
			BattleViewLoader.focusObject = battle;
			return;
		}
		Debug.LogWarning("There is not battle to leave!");
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x0009A000 File Offset: 0x00098200
	private static void HanldeExitBattleStates(BattleViewLoadingScreen.TransitionState state)
	{
		if (state == BattleViewLoadingScreen.TransitionState.InProgress)
		{
			SceneManager.UnloadSceneAsync(BattleViewLoader.CurrentBattleView);
			return;
		}
		if (state != BattleViewLoadingScreen.TransitionState.Completed)
		{
			return;
		}
		BattleViewLoader.sm_InTrasition = false;
		BattleViewLoadingScreen.OnState -= BattleViewLoader.HanldeExitBattleStates;
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null && BattleViewLoader.focusObject != null && worldUI != null)
		{
			worldUI.LookAt(BattleViewLoader.focusObject.position, false);
		}
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x0009A070 File Offset: 0x00098270
	private static void DisableWorldView()
	{
		if (BattleViewLoader.currentBatte.MapName == BattleViewLoader.currentBatte.oldMapName)
		{
			return;
		}
		VisibilityDetector.EnableGroup(false, "WorldView");
		BattleViewLoader.WorldView = SceneManager.GetActiveScene();
		BattleViewLoader.sm_rootObjectsActiveSelfState.Clear();
		GameObject[] rootGameObjects = BattleViewLoader.WorldView.GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			GameObject gameObject = rootGameObjects[i];
			BattleViewLoader.sm_rootObjectsActiveSelfState.Add(gameObject, gameObject.activeSelf);
			rootGameObjects[i].SetActive(false);
		}
		if (GameLogic.instance != null)
		{
			Transform transform = GameLogic.instance.transform;
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				if (BattleViewLoader.WorldObjectsContainers.Contains(child.name))
				{
					child.gameObject.SetActive(false);
				}
			}
		}
		if (ViewMode.IsPoliticalView())
		{
			PoliticalView politicalView = ViewMode.current as PoliticalView;
			if (politicalView == null)
			{
				return;
			}
			politicalView.ToggleSnapshot(true);
		}
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0009A164 File Offset: 0x00098364
	private static void ReactiveWorldView()
	{
		if (BattleViewLoader.WorldView.IsValid() && BattleViewLoader.WorldView.isLoaded)
		{
			SceneManager.SetActiveScene(BattleViewLoader.WorldView);
			GameObject[] rootGameObjects = BattleViewLoader.WorldView.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (BattleViewLoader.sm_rootObjectsActiveSelfState.ContainsKey(rootGameObjects[i]))
				{
					rootGameObjects[i].SetActive(BattleViewLoader.sm_rootObjectsActiveSelfState[rootGameObjects[i]]);
				}
			}
			BattleViewLoader.sm_rootObjectsActiveSelfState.Clear();
			if (GameLogic.instance != null)
			{
				Transform transform = GameLogic.instance.transform;
				for (int j = 0; j < transform.childCount; j++)
				{
					Transform child = transform.GetChild(j);
					if (BattleViewLoader.WorldObjectsContainers.Contains(child.name))
					{
						child.gameObject.SetActive(true);
					}
				}
			}
		}
		VisibilityDetector.EnableGroup(true, "WorldView");
		VisibilityDetector.ResyncVisibility();
		BattleViewLoader.sm_InTrasition = false;
		if (ViewMode.IsPoliticalView())
		{
			PoliticalView politicalView = ViewMode.current as PoliticalView;
			if (politicalView == null)
			{
				return;
			}
			politicalView.ToggleSnapshot(false);
		}
	}

	// Token: 0x04000A7B RID: 2683
	public static Scene WorldView;

	// Token: 0x04000A7C RID: 2684
	public static Scene CurrentBattleView;

	// Token: 0x04000A7D RID: 2685
	public static bool sm_InTrasition = false;

	// Token: 0x04000A7E RID: 2686
	private static Sprite m_loadingScreenSprite = null;

	// Token: 0x04000A7F RID: 2687
	private static bool m_isFadeOutDisabled = false;

	// Token: 0x04000A80 RID: 2688
	public static BattleViewLoader.BattleData currentBatte;

	// Token: 0x04000A81 RID: 2689
	private static MapObject focusObject;

	// Token: 0x04000A82 RID: 2690
	private static List<string> WorldObjectsContainers = new List<string>
	{
		"Settlements",
		"Armies",
		"Units",
		"Battles",
		"Reinforcements"
	};

	// Token: 0x04000A83 RID: 2691
	private static Dictionary<GameObject, bool> sm_rootObjectsActiveSelfState = new Dictionary<GameObject, bool>();

	// Token: 0x02000639 RID: 1593
	public class BattleData
	{
		// Token: 0x040034A9 RID: 13481
		public Logic.Battle BattleLogic;

		// Token: 0x040034AA RID: 13482
		public int KingdomId;

		// Token: 0x040034AB RID: 13483
		public int Side;

		// Token: 0x040034AC RID: 13484
		public string MapName;

		// Token: 0x040034AD RID: 13485
		public string oldMapName;
	}
}
