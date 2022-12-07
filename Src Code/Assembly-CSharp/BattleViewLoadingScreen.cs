using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200022D RID: 557
public class BattleViewLoadingScreen : MonoBehaviour
{
	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060021BA RID: 8634 RVA: 0x00131A4B File Offset: 0x0012FC4B
	public bool InTransition
	{
		get
		{
			return this.CurrentState == BattleViewLoadingScreen.TransitionState.Completed || this.CurrentState == BattleViewLoadingScreen.TransitionState.None;
		}
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060021BB RID: 8635 RVA: 0x00131A61 File Offset: 0x0012FC61
	// (set) Token: 0x060021BC RID: 8636 RVA: 0x00131A69 File Offset: 0x0012FC69
	public BattleViewLoadingScreen.TransitionState CurrentState { get; private set; }

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x060021BD RID: 8637 RVA: 0x00131A74 File Offset: 0x0012FC74
	// (remove) Token: 0x060021BE RID: 8638 RVA: 0x00131AA8 File Offset: 0x0012FCA8
	public static event Action<BattleViewLoadingScreen.TransitionState> OnState;

	// Token: 0x060021BF RID: 8639 RVA: 0x00131ADC File Offset: 0x0012FCDC
	public static BattleViewLoadingScreen Get()
	{
		if (BattleViewLoadingScreen.sm_Instance == null)
		{
			DT.Field defField = global::Defs.GetDefField("BattleViewLoadingScreen", null);
			if (defField != null)
			{
				GameObject obj = global::Defs.GetObj<GameObject>(defField, "window_prefab", null);
				if (obj != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj);
					BattleViewLoadingScreen component = gameObject.GetComponent<BattleViewLoadingScreen>();
					if (component != null)
					{
						BattleViewLoadingScreen.sm_Instance = component;
						BattleViewLoadingScreen.sm_Instance.gameObject.SetActive(false);
						UnityEngine.Object.DontDestroyOnLoad(BattleViewLoadingScreen.sm_Instance.gameObject);
						if (BattleViewLoadingScreen.snapshot == null)
						{
							BattleViewLoadingScreen.snapshot = new FMODWrapper.Snapshot("battleview_loading_screen_snapshot");
						}
					}
					else
					{
						Debug.Log("BattleViewLoadingScreen: provided prefab is not eligable");
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				else
				{
					Debug.Log("BattleViewLoadingScreen: fail to find def field \"BattleViewLoadingScreen\"");
				}
			}
			else
			{
				Debug.Log("BattleViewLoadingScreen: fail to find key \"window_prefab\" in  \"BattleViewLoadingScreen\"");
			}
		}
		return BattleViewLoadingScreen.sm_Instance;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x00131BA4 File Offset: 0x0012FDA4
	private void Awake()
	{
		this.transtionSM = base.gameObject.AddComponent<BattleViewLoadingScreen.StateMachine>();
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x00131BB7 File Offset: 0x0012FDB7
	internal void SetState(BattleViewLoadingScreen.TransitionState state)
	{
		this.CurrentState = state;
		if (BattleViewLoadingScreen.OnState != null)
		{
			BattleViewLoadingScreen.OnState(state);
		}
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x00131BD4 File Offset: 0x0012FDD4
	public static void DoEnterBattle(Logic.Battle battle, Sprite loadingScreenSprite = null, bool areShowAnimationsDisabled = false)
	{
		BattleViewLoadingScreen battleViewLoadingScreen = BattleViewLoadingScreen.Get();
		if (battleViewLoadingScreen == null)
		{
			return;
		}
		BattleViewLoadingScreen.areShowAnimationsDisabled = areShowAnimationsDisabled;
		battleViewLoadingScreen.gameObject.SetActive(true);
		battleViewLoadingScreen.RebuildImageComposition(loadingScreenSprite);
		battleViewLoadingScreen.transtionSM.Dispose();
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.HideWorldView());
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.LoadBattleView());
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.ShowBattleView());
		battleViewLoadingScreen.transtionSM.OnComplete += BattleViewLoadingScreen.HandleEnterBattleTranstionDone;
		battleViewLoadingScreen.transtionSM.Init();
		bool has_restarted = battle.has_restarted;
		battle.SetStage(Logic.Battle.Stage.EnteringBattle, false, 0f);
		if (!has_restarted)
		{
			SettlementBV.ResetGenerationVars();
		}
		else
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			if (battleViewUI != null)
			{
				battleViewUI.Show(false, false);
			}
		}
		FMODWrapper.Snapshot snapshot = BattleViewLoadingScreen.snapshot;
		if (snapshot == null)
		{
			return;
		}
		snapshot.StartSnapshot();
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x00131CA4 File Offset: 0x0012FEA4
	private static void HandleEnterBattleTranstionDone(BattleViewLoadingScreen.StateMachine sm)
	{
		sm.Dispose();
		BattleViewLoadingScreen.sm_Instance.gameObject.SetActive(false);
		BattleViewLoadingScreen.sm_Instance.SetState(BattleViewLoadingScreen.TransitionState.Completed);
		GameLogic instance = GameLogic.instance;
		if (instance == null)
		{
			return;
		}
		instance.RefreshMasterVolume();
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x00131CD8 File Offset: 0x0012FED8
	public static void DoExitBattle(Logic.Battle battle, Sprite loadingScreenSprite = null, bool isFadeOutDisabled = false)
	{
		BattleViewLoadingScreen battleViewLoadingScreen = BattleViewLoadingScreen.Get();
		if (battleViewLoadingScreen == null)
		{
			return;
		}
		BattleViewLoadingScreen.areShowAnimationsDisabled = isFadeOutDisabled;
		battleViewLoadingScreen.RebuildImageComposition(loadingScreenSprite);
		battleViewLoadingScreen.gameObject.SetActive(true);
		FMODVoiceProvider.ClearAllInactiveVoices();
		battleViewLoadingScreen.transtionSM.Dispose();
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.UnloadBattleView());
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.ShowWorldView());
		battleViewLoadingScreen.transtionSM.OnComplete += BattleViewLoadingScreen.HandleExitBattleTranstionDone;
		battleViewLoadingScreen.transtionSM.Init();
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x00131D60 File Offset: 0x0012FF60
	private static void HandleExitBattleTranstionDone(BattleViewLoadingScreen.StateMachine sm)
	{
		sm.Dispose();
		BattleViewLoadingScreen.sm_Instance.gameObject.SetActive(false);
		BattleViewLoadingScreen.sm_Instance.SetState(BattleViewLoadingScreen.TransitionState.Completed);
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x00131D84 File Offset: 0x0012FF84
	public static void DoReturnToWorld()
	{
		BattleViewLoadingScreen battleViewLoadingScreen = BattleViewLoadingScreen.Get();
		if (battleViewLoadingScreen == null)
		{
			return;
		}
		battleViewLoadingScreen.gameObject.SetActive(true);
		battleViewLoadingScreen.transtionSM.Dispose();
		battleViewLoadingScreen.transtionSM.AddState(new BattleViewLoadingScreen.ShowWorldView());
		battleViewLoadingScreen.transtionSM.OnComplete += BattleViewLoadingScreen.HandleWorldToWorldTranstionDone;
		battleViewLoadingScreen.transtionSM.Init();
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x00131D60 File Offset: 0x0012FF60
	private static void HandleWorldToWorldTranstionDone(BattleViewLoadingScreen.StateMachine sm)
	{
		sm.Dispose();
		BattleViewLoadingScreen.sm_Instance.gameObject.SetActive(false);
		BattleViewLoadingScreen.sm_Instance.SetState(BattleViewLoadingScreen.TransitionState.Completed);
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x00131DEC File Offset: 0x0012FFEC
	private void RebuildImageComposition(Sprite loadingScreenSprite)
	{
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "id_Illustration", true, true);
		Image image = (gameObject != null) ? gameObject.GetComponent<Image>() : null;
		if (image != null)
		{
			Sprite sprite = (loadingScreenSprite != null) ? loadingScreenSprite : global::Defs.GetRandomObj<Sprite>("LoadingScreen", "background.battleview", null);
			if (sprite != null)
			{
				image.overrideSprite = sprite;
			}
		}
	}

	// Token: 0x040016A6 RID: 5798
	private static BattleViewLoadingScreen sm_Instance;

	// Token: 0x040016A9 RID: 5801
	private static bool areShowAnimationsDisabled;

	// Token: 0x040016AA RID: 5802
	private BattleViewLoadingScreen.StateMachine transtionSM;

	// Token: 0x040016AB RID: 5803
	private static FMODWrapper.Snapshot snapshot;

	// Token: 0x0200077B RID: 1915
	[Serializable]
	public class TarnsitonStateEvent : UnityEvent<BattleViewLoadingScreen.TransitionState>
	{
	}

	// Token: 0x0200077C RID: 1916
	public enum TransitionState
	{
		// Token: 0x04003ACF RID: 15055
		None,
		// Token: 0x04003AD0 RID: 15056
		Starting,
		// Token: 0x04003AD1 RID: 15057
		InProgress,
		// Token: 0x04003AD2 RID: 15058
		Finishing,
		// Token: 0x04003AD3 RID: 15059
		Completed
	}

	// Token: 0x0200077D RID: 1917
	private class StateMachine : MonoBehaviour
	{
		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06004C3A RID: 19514 RVA: 0x00228CB4 File Offset: 0x00226EB4
		// (remove) Token: 0x06004C3B RID: 19515 RVA: 0x00228CEC File Offset: 0x00226EEC
		public event Action<BattleViewLoadingScreen.StateMachine> OnComplete;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06004C3C RID: 19516 RVA: 0x00228D24 File Offset: 0x00226F24
		// (remove) Token: 0x06004C3D RID: 19517 RVA: 0x00228D5C File Offset: 0x00226F5C
		public event Action<BattleViewLoadingScreen.ITransitionState, BattleViewLoadingScreen.ITransitionState> OnStateChange;

		// Token: 0x06004C3E RID: 19518 RVA: 0x00228D91 File Offset: 0x00226F91
		public void AddState(BattleViewLoadingScreen.ITransitionState state)
		{
			this.states.Add(state);
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x00228DA0 File Offset: 0x00226FA0
		public void SetState(BattleViewLoadingScreen.ITransitionState next)
		{
			if (this.current != null)
			{
				this.current.OnExitState(this);
			}
			BattleViewLoadingScreen.ITransitionState arg = this.current;
			this.current = next;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(arg, this.current);
			}
			if (this.current == null)
			{
				if (this.OnComplete != null)
				{
					this.OnComplete(this);
				}
				return;
			}
			this.current.OnEnterState(this);
		}

		// Token: 0x06004C40 RID: 19520 RVA: 0x00228E14 File Offset: 0x00227014
		public BattleViewLoadingScreen.ITransitionState GetNext()
		{
			if (this.current == null)
			{
				if (this.states != null && this.states.Count > 0)
				{
					return this.states[0];
				}
				return null;
			}
			else
			{
				int num = this.states.IndexOf(this.current);
				if (num != -1 && num < this.states.Count - 1)
				{
					return this.states[num + 1];
				}
				return null;
			}
		}

		// Token: 0x06004C41 RID: 19521 RVA: 0x00228E84 File Offset: 0x00227084
		private void Update()
		{
			if (this.current != null)
			{
				this.current.OnUpdateState(this);
			}
		}

		// Token: 0x06004C42 RID: 19522 RVA: 0x00228E9C File Offset: 0x0022709C
		public void Init()
		{
			BattleViewLoadingScreen.ITransitionState next = this.GetNext();
			if (next != null)
			{
				this.SetState(next);
			}
		}

		// Token: 0x06004C43 RID: 19523 RVA: 0x00228EBC File Offset: 0x002270BC
		public void Dispose()
		{
			if (this.states.Count > 0)
			{
				for (int i = 0; i < this.states.Count; i++)
				{
					this.states[i].Reset(this);
				}
			}
			this.states.Clear();
			this.current = null;
		}

		// Token: 0x04003AD6 RID: 15062
		private List<BattleViewLoadingScreen.ITransitionState> states = new List<BattleViewLoadingScreen.ITransitionState>();

		// Token: 0x04003AD7 RID: 15063
		private BattleViewLoadingScreen.ITransitionState current;
	}

	// Token: 0x0200077E RID: 1918
	private interface ITransitionState
	{
		// Token: 0x06004C45 RID: 19525
		void OnEnterState(BattleViewLoadingScreen.StateMachine sm);

		// Token: 0x06004C46 RID: 19526
		void OnExitState(BattleViewLoadingScreen.StateMachine sm);

		// Token: 0x06004C47 RID: 19527
		void OnUpdateState(BattleViewLoadingScreen.StateMachine sm);

		// Token: 0x06004C48 RID: 19528
		void Reset(BattleViewLoadingScreen.StateMachine sm);
	}

	// Token: 0x0200077F RID: 1919
	private class HideWorldView : BattleViewLoadingScreen.ITransitionState
	{
		// Token: 0x06004C49 RID: 19529 RVA: 0x00228F24 File Offset: 0x00227124
		public void OnEnterState(BattleViewLoadingScreen.StateMachine sm)
		{
			GameObject gameObject = global::Common.FindChildByName(sm.gameObject, "id_LoadingScreen", true, true);
			if (gameObject != null)
			{
				CanvasGroup component = gameObject.GetComponent<CanvasGroup>();
				if (component != null)
				{
					component.alpha = (float)(BattleViewLoadingScreen.areShowAnimationsDisabled ? 1 : 0);
				}
			}
			GameObject gameObject2 = global::Common.FindChildByName(sm.gameObject, "id_FadeToBlack", true, true);
			if (gameObject2 != null && !BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				TweenAlpha twa = gameObject2.GetComponent<TweenAlpha>();
				twa.from = 0f;
				twa.to = 1f;
				twa.delay = 0f;
				twa.duration = 1.25f;
				twa.ResetToBeginning();
				twa.PlayForward();
				twa.onFinished.AddListener(delegate()
				{
					sm.SetState(sm.GetNext());
					twa.onFinished.RemoveAllListeners();
				});
				return;
			}
			if (gameObject2 != null && BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				Image component2 = gameObject2.GetComponent<Image>();
				Color color = component2.color;
				color.a = 1f;
				component2.color = color;
			}
			sm.SetState(sm.GetNext());
		}

		// Token: 0x06004C4A RID: 19530 RVA: 0x0022908D File Offset: 0x0022728D
		public void OnExitState(BattleViewLoadingScreen.StateMachine sm)
		{
			global::AudioRenderer audioRenderer = global::AudioRenderer.Get();
			if (audioRenderer != null)
			{
				audioRenderer.MuteAllChannels();
			}
			global::AudioRenderer audioRenderer2 = global::AudioRenderer.Get();
			if (audioRenderer2 == null)
			{
				return;
			}
			audioRenderer2.StopAllChannels();
		}

		// Token: 0x06004C4B RID: 19531 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnUpdateState(BattleViewLoadingScreen.StateMachine sm)
		{
		}

		// Token: 0x06004C4C RID: 19532 RVA: 0x000023FD File Offset: 0x000005FD
		public void Reset(BattleViewLoadingScreen.StateMachine sm)
		{
		}
	}

	// Token: 0x02000780 RID: 1920
	private class LoadBattleView : BattleViewLoadingScreen.ITransitionState
	{
		// Token: 0x06004C4E RID: 19534 RVA: 0x002290B0 File Offset: 0x002272B0
		public void OnEnterState(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = UnityEngine.Time.unscaledTime;
			GameObject gameObject = global::Common.FindChildByName(sm.gameObject, "id_ProgressForeground", true, true);
			if (gameObject != null)
			{
				this.progressBar = gameObject.GetComponent<Image>();
			}
			GameObject gameObject2 = global::Common.FindChildByName(sm.gameObject, "id_LoadingScreen", true, true);
			if (gameObject2 != null && !BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				TweenCanvasGroupAplha twa = gameObject2.GetComponent<TweenCanvasGroupAplha>();
				if (twa)
				{
					twa.from = 0f;
					twa.to = 1f;
					twa.ignoreTimeScale = true;
					twa.duration = 0.8f;
					twa.ResetToBeginning();
					twa.PlayForward();
					twa.onFinished.AddListener(delegate()
					{
						BattleViewLoadingScreen component2 = sm.GetComponent<BattleViewLoadingScreen>();
						if (component2 != null)
						{
							component2.SetState(BattleViewLoadingScreen.TransitionState.InProgress);
						}
						twa.onFinished.RemoveAllListeners();
					});
				}
			}
			else
			{
				BattleViewLoadingScreen.areShowAnimationsDisabled = false;
				BattleViewLoadingScreen component = sm.GetComponent<BattleViewLoadingScreen>();
				if (component != null)
				{
					component.SetState(BattleViewLoadingScreen.TransitionState.InProgress);
				}
			}
			GameObject gameObject3 = global::Common.FindChildByName(sm.gameObject, "id_GenerationDescription", true, true);
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
				this.generation_description = gameObject3.GetComponent<TextMeshProUGUI>();
			}
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x0022922E File Offset: 0x0022742E
		public void OnExitState(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = 0f;
			this.progressBar = null;
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06004C50 RID: 19536 RVA: 0x00229242 File Offset: 0x00227442
		private float progress
		{
			get
			{
				return (UnityEngine.Time.unscaledTime - this.startTime) / this.fakeTransitionLen;
			}
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x00229258 File Offset: 0x00227458
		public void OnUpdateState(BattleViewLoadingScreen.StateMachine sm)
		{
			bool flag = this.IsLoadedAndPrepaired();
			bool flag2 = true;
			float progress = this.progress;
			if (SettlementBV.generating || progress >= 1f)
			{
				if (this.generation_description != null)
				{
					UIText.SetTextKey(this.generation_description, SettlementBV.cur_generation_description, null, null);
				}
				if (this.progressBar != null)
				{
					if (SettlementBV.finished_generation)
					{
						this.progressBar.fillAmount = 1f;
					}
					else if (SettlementBV.total_generation_steps == 0)
					{
						this.progressBar.fillAmount = 0.25f * Mathf.Clamp01(this.progress);
					}
					else
					{
						this.progressBar.fillAmount = 0.25f + 0.75f * ((float)SettlementBV.cur_generation_step / (float)SettlementBV.total_generation_steps);
					}
				}
			}
			else
			{
				if (this.generation_description != null)
				{
					this.generation_description.text = "Generating terrain";
				}
				if (this.progressBar != null)
				{
					this.progressBar.fillAmount = 0.25f * Mathf.Clamp01(this.progress);
				}
			}
			if (flag && flag2)
			{
				Debug.Log("Finished Loading Battleview");
				sm.SetState(sm.GetNext());
			}
		}

		// Token: 0x06004C52 RID: 19538 RVA: 0x00229380 File Offset: 0x00227580
		private unsafe bool IsLoadedAndPrepaired()
		{
			bool flag = BattleFieldOverview.Get() != null;
			Logic.Battle battle = BattleMap.battle;
			PathData pathData;
			if (battle == null)
			{
				pathData = null;
			}
			else
			{
				Game batte_view_game = battle.batte_view_game;
				if (batte_view_game == null)
				{
					pathData = null;
				}
				else
				{
					Logic.PathFinding path_finding = batte_view_game.path_finding;
					pathData = ((path_finding != null) ? path_finding.data : null);
				}
			}
			PathData pathData2 = pathData;
			return flag & (pathData2 != null && pathData2.pointers.Initted != null && *pathData2.pointers.Initted && SettlementBV.finished_generation);
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x0022922E File Offset: 0x0022742E
		public void Reset(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = 0f;
			this.progressBar = null;
		}

		// Token: 0x04003AD8 RID: 15064
		private float startTime;

		// Token: 0x04003AD9 RID: 15065
		private Image progressBar;

		// Token: 0x04003ADA RID: 15066
		private TextMeshProUGUI generation_description;

		// Token: 0x04003ADB RID: 15067
		private float fakeTransitionLen;

		// Token: 0x04003ADC RID: 15068
		private const float max_load_progress = 0.25f;
	}

	// Token: 0x02000781 RID: 1921
	private class ShowBattleView : BattleViewLoadingScreen.ITransitionState
	{
		// Token: 0x06004C55 RID: 19541 RVA: 0x002293EC File Offset: 0x002275EC
		public void OnEnterState(BattleViewLoadingScreen.StateMachine sm)
		{
			GameObject gameObject = global::Common.FindChildByName(sm.gameObject, "id_LoadingScreen", true, true);
			if (gameObject != null)
			{
				TweenCanvasGroupAplha component = gameObject.GetComponent<TweenCanvasGroupAplha>();
				if (component)
				{
					component.from = 1f;
					component.to = 0f;
					component.delay = 0.3f;
					component.duration = 0.3f;
					component.ResetToBeginning();
					component.PlayForward();
				}
			}
			GameObject gameObject2 = global::Common.FindChildByName(sm.gameObject, "id_FadeToBlack", true, true);
			if (gameObject2 != null)
			{
				TweenAlpha twa = gameObject2.GetComponent<TweenAlpha>();
				twa.from = 1f;
				twa.to = 0f;
				twa.ResetToBeginning();
				twa.delay = 0.9f;
				twa.duration = 1.3f;
				twa.PlayForward();
				twa.onFinished.AddListener(delegate()
				{
					sm.SetState(sm.GetNext());
					twa.onFinished.RemoveAllListeners();
				});
			}
			else
			{
				sm.SetState(sm.GetNext());
			}
			FMODWrapper.Snapshot snapshot = BattleViewLoadingScreen.snapshot;
			if (snapshot == null)
			{
				return;
			}
			snapshot.EndSnapshot();
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnExitState(BattleViewLoadingScreen.StateMachine sm)
		{
		}

		// Token: 0x06004C57 RID: 19543 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnUpdateState(BattleViewLoadingScreen.StateMachine sm)
		{
		}

		// Token: 0x06004C58 RID: 19544 RVA: 0x000023FD File Offset: 0x000005FD
		public void Reset(BattleViewLoadingScreen.StateMachine sm)
		{
		}
	}

	// Token: 0x02000782 RID: 1922
	private class UnloadBattleView : BattleViewLoadingScreen.ITransitionState
	{
		// Token: 0x06004C5A RID: 19546 RVA: 0x00229550 File Offset: 0x00227750
		public void OnEnterState(BattleViewLoadingScreen.StateMachine sm)
		{
			GameObject gameObject = global::Common.FindChildByName(sm.gameObject, "id_GenerationDescription", true, true);
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			Game game = GameLogic.Get(false);
			if (game != null)
			{
				Pause pause = game.pause;
				if (pause != null)
				{
					pause.AddRequest("LoadingPause", -2);
				}
			}
			GameObject gameObject2 = global::Common.FindChildByName(sm.gameObject, "id_FadeToBlack", true, true);
			if (gameObject2 != null && !BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				TweenAlpha twa = gameObject2.GetComponent<TweenAlpha>();
				twa.from = 0f;
				twa.to = 1f;
				twa.ResetToBeginning();
				twa.delay = 0.6f;
				twa.duration = 1.3f;
				twa.PlayForward();
				twa.onFinished.AddListener(delegate()
				{
					sm.SetState(sm.GetNext());
					twa.onFinished.RemoveAllListeners();
				});
				return;
			}
			if (gameObject2 != null && BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				Image component = gameObject2.GetComponent<Image>();
				Color color = component.color;
				color.a = 1f;
				component.color = color;
			}
			sm.SetState(sm.GetNext());
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnExitState(BattleViewLoadingScreen.StateMachine sm)
		{
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x000023FD File Offset: 0x000005FD
		public void OnUpdateState(BattleViewLoadingScreen.StateMachine sm)
		{
		}

		// Token: 0x06004C5D RID: 19549 RVA: 0x000023FD File Offset: 0x000005FD
		public void Reset(BattleViewLoadingScreen.StateMachine sm)
		{
		}
	}

	// Token: 0x02000783 RID: 1923
	private class ShowWorldView : BattleViewLoadingScreen.ITransitionState
	{
		// Token: 0x06004C5F RID: 19551 RVA: 0x002296B8 File Offset: 0x002278B8
		public void OnEnterState(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = UnityEngine.Time.unscaledTime;
			GameObject gameObject = global::Common.FindChildByName(sm.gameObject, "id_ProgressForeground", true, true);
			if (gameObject != null)
			{
				this.progressBar = gameObject.GetComponent<Image>();
			}
			GameObject gameObject2 = global::Common.FindChildByName(sm.gameObject, "id_LoadingScreen", true, true);
			if (gameObject2 != null && !BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				TweenCanvasGroupAplha twa = gameObject2.GetComponent<TweenCanvasGroupAplha>();
				if (twa)
				{
					twa.from = 0f;
					twa.to = 1f;
					twa.duration = 0.6f;
					twa.ResetToBeginning();
					twa.PlayForward();
					twa.onFinished.AddListener(delegate()
					{
						BattleViewLoadingScreen component3 = sm.GetComponent<BattleViewLoadingScreen>();
						if (component3 != null)
						{
							component3.SetState(BattleViewLoadingScreen.TransitionState.InProgress);
						}
						twa.onFinished.RemoveAllListeners();
						Game game2 = GameLogic.Get(false);
						if (game2 == null)
						{
							return;
						}
						Pause pause2 = game2.pause;
						if (pause2 == null)
						{
							return;
						}
						pause2.DelRequest("LoadingPause", -2);
					});
					return;
				}
			}
			else if (BattleViewLoadingScreen.areShowAnimationsDisabled)
			{
				if (gameObject2 != null && BattleViewLoadingScreen.areShowAnimationsDisabled)
				{
					CanvasGroup component = gameObject2.GetComponent<CanvasGroup>();
					if (component != null)
					{
						component.alpha = 1f;
					}
				}
				BattleViewLoadingScreen.areShowAnimationsDisabled = false;
				BattleViewLoadingScreen component2 = sm.GetComponent<BattleViewLoadingScreen>();
				if (component2 != null)
				{
					component2.SetState(BattleViewLoadingScreen.TransitionState.InProgress);
				}
				Game game = GameLogic.Get(false);
				if (game == null)
				{
					return;
				}
				Pause pause = game.pause;
				if (pause == null)
				{
					return;
				}
				pause.DelRequest("LoadingPause", -2);
				return;
			}
			else
			{
				sm.SetState(sm.GetNext());
			}
		}

		// Token: 0x06004C60 RID: 19552 RVA: 0x00229858 File Offset: 0x00227A58
		public void OnExitState(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = 0f;
			this.progressBar = null;
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return;
			}
			MapData.Get().LoadBordersFromBinary();
			if (BattleMap.battle != null && BattleMap.battle.IsValid() && !BattleMap.battle.IsFinishing())
			{
				BattleMap.battle.CheckVictory(true, false);
			}
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom.IsDefeated())
			{
				Logic.Kingdom attacker_kingdom = BattleMap.battle.attacker_kingdom;
				game.ValidateEndGame(attacker_kingdom, kingdom);
			}
			FMODWrapper.Snapshot snapshot = BattleViewLoadingScreen.snapshot;
			if (snapshot == null)
			{
				return;
			}
			snapshot.EndSnapshot();
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x002298EC File Offset: 0x00227AEC
		public void OnUpdateState(BattleViewLoadingScreen.StateMachine sm)
		{
			if (this.progressBar != null)
			{
				this.progressBar.fillAmount = (UnityEngine.Time.unscaledTime - this.startTime) / this.fakeTransitionLen;
			}
			if (this.startTime + this.fakeTransitionLen < UnityEngine.Time.unscaledTime)
			{
				sm.SetState(sm.GetNext());
			}
		}

		// Token: 0x06004C62 RID: 19554 RVA: 0x00229945 File Offset: 0x00227B45
		public void Reset(BattleViewLoadingScreen.StateMachine sm)
		{
			this.startTime = 0f;
			this.progressBar = null;
		}

		// Token: 0x04003ADD RID: 15069
		private float startTime;

		// Token: 0x04003ADE RID: 15070
		private Image progressBar;

		// Token: 0x04003ADF RID: 15071
		private float fakeTransitionLen = 1.5f;
	}
}
