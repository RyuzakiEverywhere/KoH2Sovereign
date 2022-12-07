using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000257 RID: 599
internal class UIInGameObjectivesAndRules : UIWindow, IListener
{
	// Token: 0x060024D9 RID: 9433 RVA: 0x001495C5 File Offset: 0x001477C5
	public override string GetDefId()
	{
		return UIInGameObjectivesAndRules.def_id;
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x001495CC File Offset: 0x001477CC
	private void Init()
	{
		if (this.m_initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ButtonClose != null)
		{
			this.m_ButtonClose.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		if (this.m_RulesContainer != null)
		{
			this.m_Rules = this.m_RulesContainer.GetOrAddComponent<UIInGameObjectivesAndRules.Rules>();
		}
		if (this.m_ToggleObjective != null)
		{
			this.m_ToggleObjective.onClick = new BSGButton.OnClick(this.HandleOnToggleObjectives);
		}
		if (this.m_ToggleRules != null)
		{
			this.m_ToggleRules.onClick = new BSGButton.OnClick(this.HandleOnToggleRules);
		}
		if (this.m_ObjectivePrototype != null)
		{
			this.m_ObjectivePrototype.gameObject.SetActive(false);
		}
		this.m_Game = GameLogic.Get(false);
		this.m_initialzied = true;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x001496A9 File Offset: 0x001478A9
	private void Start()
	{
		this.Init();
		this.CreateTimeTooltip();
		this.SetupTabs();
		this.RebuildObjectives();
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x001496C3 File Offset: 0x001478C3
	private void RebuildObjectives()
	{
		this.BuildObjectives();
		UIInGameObjectivesAndRules.Rules rules = this.m_Rules;
		if (rules != null)
		{
			rules.Populate();
		}
		this.UpdateLayout();
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x001496E4 File Offset: 0x001478E4
	private void CreateTimeTooltip()
	{
		if (this.m_Time == null)
		{
			return;
		}
		Game game = this.m_Game;
		Game.CampaignRules campaignRules = (game != null) ? game.rules : null;
		if (campaignRules == null)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<string>("type", campaignRules.time_limits.type.ToString());
		Tooltip.Get(this.m_Time, true).SetDef("MultiplyerTimeLimitTooltip", vars);
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x00149758 File Offset: 0x00147958
	private void SetupTabs()
	{
		if (this.m_ToggleObjectiveLabel != null)
		{
			UIText.SetTextKey(this.m_ToggleObjectiveLabel, "MultiplyerObjectivesAndRulesWindow.toggle_objective", null, null);
		}
		if (this.m_ToggleRulesLabel != null)
		{
			UIText.SetTextKey(this.m_ToggleRulesLabel, "MultiplyerObjectivesAndRulesWindow.toggle_rules", null, null);
		}
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x001497A8 File Offset: 0x001479A8
	private void UpdateLayout()
	{
		if (this.m_Objectives != null)
		{
			this.m_Objectives.SetActive(this.m_isToggleObjectives);
		}
		if (this.m_RulesContainer != null)
		{
			this.m_RulesContainer.SetActive(!this.m_isToggleObjectives);
		}
		Color color = global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "text_color_toggle_on");
		Color color2 = global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "text_color_toggle_off");
		if (this.m_ToggleObjectiveLabel != null)
		{
			this.m_ToggleObjectiveLabel.color = (this.m_isToggleObjectives ? color : color2);
		}
		if (this.m_ToggleRulesLabel != null)
		{
			this.m_ToggleRulesLabel.color = ((!this.m_isToggleObjectives) ? color : color2);
		}
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x00149860 File Offset: 0x00147A60
	private void BuildObjectives()
	{
		this.m_ActibeObjectives.Clear();
		UICommon.DeleteActiveChildren(this.m_ObjectivesContainer);
		List<DT.Field> list = this.ExtractObjectives();
		for (int i = 0; i < list.Count; i++)
		{
			UIInGameObjectivesAndRules.Objective objective = UIInGameObjectivesAndRules.Objective.Create(list[i], this.m_ObjectivePrototype, this.m_ObjectivesContainer);
			if (!(objective == null))
			{
				this.m_ActibeObjectives.Add(objective);
				UIInGameObjectivesAndRules.Objective objective2 = objective;
				objective2.OnSelect = (Action<UIInGameObjectivesAndRules.Objective>)Delegate.Combine(objective2.OnSelect, new Action<UIInGameObjectivesAndRules.Objective>(this.SelectObjectove));
			}
		}
		this.SelectObjectove(this.m_ActibeObjectives[0]);
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x00149900 File Offset: 0x00147B00
	private void SelectObjectove(UIInGameObjectivesAndRules.Objective o)
	{
		for (int i = 0; i < this.m_ActibeObjectives.Count; i++)
		{
			this.m_ActibeObjectives[i].Select(this.m_ActibeObjectives[i] == o);
		}
		if (this.m_CurrentObjectiveDescription != null)
		{
			TMP_Text currentObjectiveDescription = this.m_CurrentObjectiveDescription;
			DT.Field objectiveDef = o.objectiveDef;
			string key = "description";
			Game game = GameLogic.Get(false);
			UIText.SetText(currentObjectiveDescription, global::Defs.Localize(objectiveDef, key, (game != null) ? game.rules : null, null, true, true));
		}
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x00149984 File Offset: 0x00147B84
	protected override void Update()
	{
		base.Update();
		if (this.m_LastUpdate + this.m_RefreshRate < UnityEngine.Time.unscaledTime)
		{
			this.UpdateStats();
			this.m_LastUpdate += this.m_RefreshRate;
		}
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x001499BC File Offset: 0x00147BBC
	private void UpdateStats()
	{
		if (this.m_Game == null)
		{
			return;
		}
		string str;
		Color color;
		Game.CampaignRules.TimeLimits.Type type;
		bool active = UIMultiplayerUtils.TryGetTimeLimitString(out str, out color, out type);
		if (this.m_Time != null)
		{
			this.m_Time.gameObject.SetActive(active);
		}
		if (this.m_TimeLabel != null)
		{
			this.m_TimeLabel.color = color;
			this.m_TimeLimitVars.Set<string>("time_limit", "#" + str);
			UIText.SetText(this.m_TimeLabel, global::Defs.Localize("MultiplyerObjectivesAndRulesWindow.time_limit", this.m_TimeLimitVars, null, true, true));
		}
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x00149A50 File Offset: 0x00147C50
	private void HandleOnToggleRules(BSGButton btn)
	{
		this.m_isToggleObjectives = false;
		this.UpdateLayout();
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x00149A5F File Offset: 0x00147C5F
	private void HandleOnToggleObjectives(BSGButton btn)
	{
		this.m_isToggleObjectives = true;
		this.UpdateLayout();
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleOnClose(BSGButton bnt)
	{
		this.Close(false);
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x00149A6E File Offset: 0x00147C6E
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIInGameObjectivesAndRules.def_id, null);
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x00149A7C File Offset: 0x00147C7C
	public static void ToggleOpen()
	{
		if (UIInGameObjectivesAndRules.current != null)
		{
			UIInGameObjectivesAndRules.current.Close(false);
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		GameObject prefab = UIInGameObjectivesAndRules.GetPrefab();
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		if (gameObject != null)
		{
			UICommon.DeleteChildren(gameObject.transform, typeof(UIInGameObjectivesAndRules));
			UIInGameObjectivesAndRules.current = UIInGameObjectivesAndRules.Create(prefab, gameObject.transform as RectTransform);
		}
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x00149B0C File Offset: 0x00147D0C
	private List<DT.Field> ExtractObjectives()
	{
		List<DT.Field> list = new List<DT.Field>();
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return list;
		}
		DT.Field ruleOption = UIMultiplayerUtils.GetRuleOption(game.campaign, "main_goal");
		if (ruleOption != null)
		{
			list.Add(ruleOption);
		}
		DT.Field varDef = game.campaign.GetVarDef("goal_prestige_victory");
		if (varDef != null)
		{
			list.Add(varDef);
		}
		DT.Field varDef2 = game.campaign.GetVarDef("goal_kingdom_advantages");
		if (varDef2 != null)
		{
			list.Add(varDef2);
		}
		return list;
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x00149B80 File Offset: 0x00147D80
	public static bool IsActive()
	{
		return UIInGameObjectivesAndRules.current != null;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x00149B90 File Offset: 0x00147D90
	public static UIInGameObjectivesAndRules Create(GameObject prototype, RectTransform parent)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UIInGameObjectivesAndRules orAddComponent = global::Common.Spawn(prototype, parent, false, "").GetOrAddComponent<UIInGameObjectivesAndRules>();
		orAddComponent.on_close = (UIWindow.OnClose)Delegate.Combine(orAddComponent.on_close, new UIWindow.OnClose(delegate(UIWindow _)
		{
			UIInGameObjectivesAndRules.current = null;
		}));
		orAddComponent.Open();
		return orAddComponent;
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x00149C00 File Offset: 0x00147E00
	public static Value SetupHTObjectiveScores(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		if (((ht != null) ? ht.vars : null) == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht2 = arg.ht;
		Vars vars = ((ht2 != null) ? ht2.vars : null) as Vars;
		UIInGameObjectivesAndRules.Objective objective = vars.obj.obj_val as UIInGameObjectivesAndRules.Objective;
		vars.Set<List<UIInGameObjectivesAndRules.Objective.ObjectiveScore>>("score_types", objective.GetScoreTypes());
		return true;
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x00149C65 File Offset: 0x00147E65
	private void OnEnable()
	{
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			game.AddListener(this);
		}
		if (this.m_initialzied)
		{
			this.RebuildObjectives();
		}
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x00149C87 File Offset: 0x00147E87
	private void OnDisable()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.DelListener(this);
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x00149C9A File Offset: 0x00147E9A
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "local_player_repicked" || message == "local_player_continue_playing")
		{
			this.BuildObjectives();
			UIInGameObjectivesAndRules.Rules rules = this.m_Rules;
			if (rules != null)
			{
				rules.Populate();
			}
			this.UpdateLayout();
		}
	}

	// Token: 0x04001907 RID: 6407
	private static string def_id = "MultiplyerObjectivesAndRulesWindow";

	// Token: 0x04001908 RID: 6408
	[UIFieldTarget("id_Button_Close")]
	private BSGButton m_ButtonClose;

	// Token: 0x04001909 RID: 6409
	[UIFieldTarget("id_Objectives")]
	private GameObject m_Objectives;

	// Token: 0x0400190A RID: 6410
	[UIFieldTarget("id_ObjectivesContainer")]
	private RectTransform m_ObjectivesContainer;

	// Token: 0x0400190B RID: 6411
	[UIFieldTarget("id_ObjectivePrototype")]
	private GameObject m_ObjectivePrototype;

	// Token: 0x0400190C RID: 6412
	[UIFieldTarget("id_CurrentObjectiveDescription")]
	private TextMeshProUGUI m_CurrentObjectiveDescription;

	// Token: 0x0400190D RID: 6413
	[UIFieldTarget("id_Time")]
	private GameObject m_Time;

	// Token: 0x0400190E RID: 6414
	[UIFieldTarget("id_TimeLabel")]
	private TextMeshProUGUI m_TimeLabel;

	// Token: 0x0400190F RID: 6415
	[UIFieldTarget("id_Rules")]
	private GameObject m_RulesContainer;

	// Token: 0x04001910 RID: 6416
	private UIInGameObjectivesAndRules.Rules m_Rules;

	// Token: 0x04001911 RID: 6417
	[UIFieldTarget("id_ToggleObjective")]
	private BSGButton m_ToggleObjective;

	// Token: 0x04001912 RID: 6418
	[UIFieldTarget("id_ToggleObjectiveLabel")]
	private TextMeshProUGUI m_ToggleObjectiveLabel;

	// Token: 0x04001913 RID: 6419
	[UIFieldTarget("id_ToggleRules")]
	private BSGButton m_ToggleRules;

	// Token: 0x04001914 RID: 6420
	[UIFieldTarget("id_ToggleRulesLabel")]
	private TextMeshProUGUI m_ToggleRulesLabel;

	// Token: 0x04001915 RID: 6421
	private bool m_isToggleObjectives = true;

	// Token: 0x04001916 RID: 6422
	private bool m_initialzied;

	// Token: 0x04001917 RID: 6423
	private Game m_Game;

	// Token: 0x04001918 RID: 6424
	private List<UIInGameObjectivesAndRules.Team> m_Teams = new List<UIInGameObjectivesAndRules.Team>();

	// Token: 0x04001919 RID: 6425
	private static UIInGameObjectivesAndRules current;

	// Token: 0x0400191A RID: 6426
	private float m_LastUpdate;

	// Token: 0x0400191B RID: 6427
	private float m_RefreshRate = 1f;

	// Token: 0x0400191C RID: 6428
	private List<UIInGameObjectivesAndRules.Objective> m_ActibeObjectives = new List<UIInGameObjectivesAndRules.Objective>();

	// Token: 0x0400191D RID: 6429
	private Vars m_TimeLimitVars = new Vars();

	// Token: 0x020007AE RID: 1966
	internal class Team : MonoBehaviour
	{
		// Token: 0x06004D37 RID: 19767 RVA: 0x0022D343 File Offset: 0x0022B543
		public void SetData(int id, DT.Field objective_def)
		{
			UICommon.FindComponents(this, false);
			this.m_Objective = objective_def;
			if (this.m_PlayerInfoProtoype != null)
			{
				this.m_PlayerInfoProtoype.gameObject.SetActive(false);
			}
			this.m_Team = id;
			this.Populate();
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x0022D37F File Offset: 0x0022B57F
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.time)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.time;
			}
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x0022D3A8 File Offset: 0x0022B5A8
		private void Populate()
		{
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return;
			}
			this.m_Players.Clear();
			UICommon.DeleteActiveChildren(this.m_PlayersContianer);
			if (game.teams[this.m_Team] == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			List<Game.Player> players = game.teams[this.m_Team].players;
			if (players == null || players.Count == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.IsFFA() && this.m_TeamData != null)
			{
				this.m_TeamData.gameObject.SetActive(false);
			}
			for (int i = 0; i < players.Count; i++)
			{
				UIInGameObjectivesAndRules.Player player = UIInGameObjectivesAndRules.Player.Create(players[i].id, this.m_Objective, this.m_PlayerInfoProtoype, this.m_PlayersContianer);
				if (!(player == null))
				{
					this.m_Players.Add(player);
				}
			}
			UIText.SetText(this.m_TeamName, global::Defs.LocalizedObjName(game.teams[this.m_Team], null, "", true));
			this.CheckScoreTypeIcon();
			this.RefreshData();
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x0022D4C8 File Offset: 0x0022B6C8
		private void RefreshData()
		{
			if (this.m_TeamPrimaryScore != null)
			{
				UIText.SetText(this.m_TeamPrimaryScore, Mathf.Round(this.GetTeamScore(this.m_Objective.key)).ToString());
			}
			if (this.m_TeamSecondaryScore != null)
			{
				UIText.SetText(this.m_TeamSecondaryScore, "-");
			}
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x0022D52C File Offset: 0x0022B72C
		private void CheckScoreTypeIcon()
		{
			Game game = GameLogic.Get(true);
			if (game == null || game.rules == null)
			{
				return;
			}
			if (this.m_ScoreTypeIcon != null)
			{
				this.m_ScoreTypeIcon.SetActive(this.m_Objective.key == "HaveXGold");
			}
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x0022D57C File Offset: 0x0022B77C
		private bool IsFFA()
		{
			Game game = GameLogic.Get(true);
			return game.rules == null || game.rules.team_size.is_unknown || game.rules.team_size == 1;
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x0022D5C4 File Offset: 0x0022B7C4
		private float GetTeamScore(string rule_name)
		{
			Game game = GameLogic.Get(false);
			if (game == null || game.rules == null)
			{
				return 0f;
			}
			return game.rules.GetTeamScore(this.m_Team, rule_name);
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x0022D5FB File Offset: 0x0022B7FB
		public static UIInGameObjectivesAndRules.Team Create(int id, DT.Field objective_def, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.gameObject.SetActive(true);
			UIInGameObjectivesAndRules.Team team = gameObject.AddComponent<UIInGameObjectivesAndRules.Team>();
			team.SetData(id, objective_def);
			return team;
		}

		// Token: 0x04003BB6 RID: 15286
		[UIFieldTarget("id_TeamName")]
		private TMP_Text m_TeamName;

		// Token: 0x04003BB7 RID: 15287
		[UIFieldTarget("id_TeamPrimaryScore")]
		private TMP_Text m_TeamPrimaryScore;

		// Token: 0x04003BB8 RID: 15288
		[UIFieldTarget("id_TeamSecondaryScore")]
		private TMP_Text m_TeamSecondaryScore;

		// Token: 0x04003BB9 RID: 15289
		[UIFieldTarget("id_ScoreTypeIcon")]
		private GameObject m_ScoreTypeIcon;

		// Token: 0x04003BBA RID: 15290
		[UIFieldTarget("id_PlayerInfoProtoype")]
		private GameObject m_PlayerInfoProtoype;

		// Token: 0x04003BBB RID: 15291
		[UIFieldTarget("id_PlayersContianer")]
		private RectTransform m_PlayersContianer;

		// Token: 0x04003BBC RID: 15292
		[UIFieldTarget("id_TeamData")]
		private RectTransform m_TeamData;

		// Token: 0x04003BBD RID: 15293
		private int m_Team;

		// Token: 0x04003BBE RID: 15294
		private DT.Field m_Objective;

		// Token: 0x04003BBF RID: 15295
		private List<UIInGameObjectivesAndRules.Player> m_Players = new List<UIInGameObjectivesAndRules.Player>();

		// Token: 0x04003BC0 RID: 15296
		private float m_LastUpdateTime;

		// Token: 0x04003BC1 RID: 15297
		private float m_UpdateInterval = 5f;
	}

	// Token: 0x020007AF RID: 1967
	internal class Player : MonoBehaviour
	{
		// Token: 0x06004D40 RID: 19776 RVA: 0x0022D657 File Offset: 0x0022B857
		public void SetMemeber(string playerId, DT.Field objetive_def)
		{
			UICommon.FindComponents(this, false);
			this.playerId = playerId;
			this.m_Objective = objetive_def;
			this.Populate();
			this.RefreshData();
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x0022D67A File Offset: 0x0022B87A
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
			}
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x0022D6A4 File Offset: 0x0022B8A4
		private void Populate()
		{
			if (this.m_PlayerName != null)
			{
				UIText.SetText(this.m_PlayerName, UIMultiplayerUtils.ExtractPlayerName(this.playerId));
			}
			if (this.m_KingdomIcon != null)
			{
				Logic.Kingdom playerKingdom = UIMultiplayerUtils.GetPlayerKingdom(this.playerId);
				if (playerKingdom != null)
				{
					this.m_KingdomIcon.SetObject(playerKingdom, null);
				}
			}
			if (this.m_KingdomDefeatedIcon != null)
			{
				this.m_KingdomDefeatedIcon.sprite = global::Defs.GetObj<Sprite>("MultiplayerStats", "player_defeated_icon", null);
			}
			this.CheckScoreTypeIcon();
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x0022D730 File Offset: 0x0022B930
		private void RefreshData()
		{
			if (this.m_PrimaryScore != null)
			{
				UIText.SetText(this.m_PrimaryScore, ((int)UIMultiplayerUtils.GetScore(this.playerId, this.m_Objective.key)).ToString());
			}
			if (this.m_SecondaryScore != null)
			{
				UIText.SetText(this.m_SecondaryScore, "-");
			}
			if (this.m_PlayerName != null)
			{
				Color color = UIMultiplayerUtils.IsOffline(this.playerId) ? global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "player_name_color_offline") : global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "player_name_color_online");
				this.m_PlayerName.color = color;
			}
			if (this.m_ConnectionStatus != null)
			{
				this.m_ConnectionStatus.sprite = global::Defs.GetObj<Sprite>("PlayerConnectionStatusIconSettings", UIMultiplayerUtils.GetStatus(this.playerId).ToString(), null);
			}
			if (this.m_KingdomDefeatedIcon != null)
			{
				this.m_KingdomDefeatedIcon.gameObject.SetActive(UIMultiplayerUtils.IsDefeated(this.playerId));
			}
		}

		// Token: 0x06004D44 RID: 19780 RVA: 0x0022D838 File Offset: 0x0022BA38
		private void CheckScoreTypeIcon()
		{
			Game game = GameLogic.Get(false);
			if (game == null || game.rules == null)
			{
				return;
			}
			if (this.m_ScoreTypeIcon != null)
			{
				this.m_ScoreTypeIcon.SetActive(this.m_Objective.key == "HaveXGold");
			}
		}

		// Token: 0x06004D45 RID: 19781 RVA: 0x0022D886 File Offset: 0x0022BA86
		public static UIInGameObjectivesAndRules.Player Create(string id, DT.Field objetive_def, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.gameObject.SetActive(true);
			gameObject.AddComponent<UIInGameObjectivesAndRules.Player>().SetMemeber(id, objetive_def);
			return null;
		}

		// Token: 0x04003BC2 RID: 15298
		[UIFieldTarget("id_KingdomDefeated")]
		private Image m_KingdomDefeatedIcon;

		// Token: 0x04003BC3 RID: 15299
		[UIFieldTarget("id_Kingdom")]
		private UIKingdomIcon m_KingdomIcon;

		// Token: 0x04003BC4 RID: 15300
		[UIFieldTarget("id_ConnectionStatus")]
		private Image m_ConnectionStatus;

		// Token: 0x04003BC5 RID: 15301
		[UIFieldTarget("id_PlayerName")]
		private TMP_Text m_PlayerName;

		// Token: 0x04003BC6 RID: 15302
		[UIFieldTarget("id_PrimaryScore")]
		private TMP_Text m_PrimaryScore;

		// Token: 0x04003BC7 RID: 15303
		[UIFieldTarget("id_ScoreTypeIcon")]
		private GameObject m_ScoreTypeIcon;

		// Token: 0x04003BC8 RID: 15304
		[UIFieldTarget("id_SecondaryScore")]
		private TMP_Text m_SecondaryScore;

		// Token: 0x04003BC9 RID: 15305
		public string playerId;

		// Token: 0x04003BCA RID: 15306
		private DT.Field m_Objective;

		// Token: 0x04003BCB RID: 15307
		private float m_LastUpdateTime;

		// Token: 0x04003BCC RID: 15308
		private float m_UpdateInterval = 2f;
	}

	// Token: 0x020007B0 RID: 1968
	internal class Objective : Hotspot
	{
		// Token: 0x06004D47 RID: 19783 RVA: 0x0022D8D8 File Offset: 0x0022BAD8
		private void Init()
		{
			if (this.m_initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_TeamPrototype != null)
			{
				this.m_TeamPrototype.gameObject.SetActive(false);
			}
			this.m_Game = GameLogic.Get(true);
			this.m_initialzied = true;
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x0022D927 File Offset: 0x0022BB27
		public void SefDef(DT.Field def)
		{
			this.objectiveDef = def;
			this.Populate();
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x0022D936 File Offset: 0x0022BB36
		private void Populate()
		{
			this.PopulateTeams();
			this.PopulateObjective();
			this.SetupLayout();
		}

		// Token: 0x06004D4A RID: 19786 RVA: 0x0022D94A File Offset: 0x0022BB4A
		private void Update()
		{
			if (this.m_Game.IsMultiplayer())
			{
				return;
			}
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
			{
				this.UpdateTotalScore();
				this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
			}
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x0022D980 File Offset: 0x0022BB80
		private void UpdateTotalScore()
		{
			if (this.m_TotalScore == null)
			{
				return;
			}
			UIText.SetText(this.m_TotalScore, this.GetTotalScore(0, this.objectiveDef.key).ToString());
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x0022D9C1 File Offset: 0x0022BBC1
		private float GetTotalScore(int teamId, string ruleKey)
		{
			if (this.m_Game == null || this.m_Game.rules == null)
			{
				return 0f;
			}
			return Mathf.Round(this.m_Game.rules.GetTeamScore(teamId, ruleKey));
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x0022D9F8 File Offset: 0x0022BBF8
		private void PopulateTeams()
		{
			this.Init();
			for (int i = 0; i < this.m_Teams.Count; i++)
			{
				if (this.m_Teams[i] != null && this.m_Teams[i].gameObject != null && this.m_Teams[i].gameObject != null)
				{
					global::Common.DestroyObj(this.m_Teams[i].gameObject);
				}
			}
			this.m_Teams.Clear();
			if (this.m_TeamPrototype == null)
			{
				return;
			}
			if (this.m_Game == null)
			{
				return;
			}
			if (this.m_Game.teams == null || this.m_Game.teams.teams.Count == 0)
			{
				return;
			}
			List<Game.Team> list = new List<Game.Team>(this.m_Game.teams.teams);
			list.Sort((Game.Team x, Game.Team y) => x.id.CompareTo(y.id));
			foreach (Game.Team team in list)
			{
				UIInGameObjectivesAndRules.Team team2 = UIInGameObjectivesAndRules.Team.Create(team.id, this.objectiveDef, this.m_TeamPrototype, this.m_TeamsContainer);
				if (!(team2 == null))
				{
					this.m_Teams.Add(team2);
				}
			}
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x0022DB6C File Offset: 0x0022BD6C
		private void SetupLayout()
		{
			if (this.m_TotalScoreContainer != null)
			{
				this.m_TotalScoreContainer.SetActive(!this.m_Game.IsMultiplayer());
			}
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x0022DB98 File Offset: 0x0022BD98
		private void PopulateObjective()
		{
			Vars vars = new Vars(GameLogic.Get(false).campaign);
			if (this.m_ObjectiveIcon != null)
			{
				this.m_ObjectiveIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.objectiveDef, "icon", null);
			}
			if (this.m_RuleLabel != null)
			{
				string key = (this.objectiveDef.key != "None") ? "name" : "secondary_name";
				UIText.SetText(this.m_RuleLabel, global::Defs.Localize(this.objectiveDef, key, vars, null, true, true));
			}
			Tooltip.Get(this.m_Caption.gameObject, true).SetDef("ObjectiveScoreTooltip", new Vars(this));
		}

		// Token: 0x06004D50 RID: 19792 RVA: 0x0022DC50 File Offset: 0x0022BE50
		private void UpdateHighlight()
		{
			if (this.m_Caption != null)
			{
				string key;
				if (this.m_Selected)
				{
					key = "caption_selected";
				}
				else if (this.mouse_in)
				{
					key = "caption_over";
				}
				else
				{
					key = "caption_normal";
				}
				this.m_Caption.overrideSprite = global::Defs.GetObj<Sprite>("MultiplyerObjectivesAndRulesWindow", key, null);
			}
			if (this.m_ObjectiveIconBorder != null)
			{
				string key2;
				if (this.m_Selected)
				{
					key2 = "objective_border_selected";
				}
				else if (this.mouse_in)
				{
					key2 = "objective_border_over";
				}
				else
				{
					key2 = "objective_border_normal";
				}
				this.m_ObjectiveIconBorder.overrideSprite = global::Defs.GetObj<Sprite>("MultiplyerObjectivesAndRulesWindow", key2, null);
			}
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x0022DCF3 File Offset: 0x0022BEF3
		public override void OnPointerEnter(PointerEventData e)
		{
			base.OnPointerEnter(e);
			this.UpdateHighlight();
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x0022DD02 File Offset: 0x0022BF02
		public override void OnPointerExit(PointerEventData e)
		{
			base.OnPointerExit(e);
			this.UpdateHighlight();
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x0022DD14 File Offset: 0x0022BF14
		public void Select(bool selected)
		{
			this.m_Selected = selected;
			this.UpdateHighlight();
			bool v = this.m_Selected && this.m_Game.IsMultiplayer();
			this.Expand(v);
			this.UpdateHighlight();
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x0022DD52 File Offset: 0x0022BF52
		private void Expand(bool v)
		{
			if (this.m_TeamsContainer != null)
			{
				this.m_TeamsContainer.gameObject.SetActive(v);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_TeamsContainer.transform as RectTransform);
			}
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x0022DD88 File Offset: 0x0022BF88
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			this.OnSelect(this);
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x0022DD9D File Offset: 0x0022BF9D
		private int GetLocalPlayerScorePerType(string type)
		{
			return 42;
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x0022DD9D File Offset: 0x0022BF9D
		private int GetLocalPlayerTeamScorePerType(string type)
		{
			return 42;
		}

		// Token: 0x06004D58 RID: 19800 RVA: 0x0022DDA4 File Offset: 0x0022BFA4
		public override Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			if (key == "rule_name")
			{
				string key2 = (this.objectiveDef.key != "None") ? "name" : "secondary_name";
				return "#" + global::Defs.Localize(this.objectiveDef, key2, vars, null, true, true);
			}
			if (key == "rule")
			{
				return new Value(this.objectiveDef);
			}
			if (key == "is_multiplyer")
			{
				Game game = GameLogic.Get(true);
				return game != null && game.game.IsMultiplayer();
			}
			if (key == "player_has_team")
			{
				return this.PlayerHasTeammates();
			}
			if (key == "total_score_player")
			{
				return this.GetTotalScorePlayer();
			}
			if (!(key == "total_score_team"))
			{
				return base.GetVar(key, vars, as_value);
			}
			return this.GetTotalScoreTeam();
		}

		// Token: 0x06004D59 RID: 19801 RVA: 0x0022DEAD File Offset: 0x0022C0AD
		private int GetTotalScorePlayer()
		{
			return (int)GameLogic.Get(true).rules.GetKingdomScore(BaseUI.LogicKingdom(), this.objectiveDef.key);
		}

		// Token: 0x06004D5A RID: 19802 RVA: 0x0022DED0 File Offset: 0x0022C0D0
		private int GetTotalScoreTeam()
		{
			Game game = GameLogic.Get(true);
			Game.Team team = game.teams.Get(BaseUI.LogicKingdom());
			return (int)game.rules.GetTeamScore(team.id, this.objectiveDef.key);
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x0022DF10 File Offset: 0x0022C110
		public List<UIInGameObjectivesAndRules.Objective.ObjectiveScore> GetScoreTypes()
		{
			List<UIInGameObjectivesAndRules.Objective.ObjectiveScore> list = new List<UIInGameObjectivesAndRules.Objective.ObjectiveScore>();
			DT.Field field = this.objectiveDef.FindChild("scoring", null, true, true, true, '.');
			List<DT.Field> list2 = (field != null) ? field.Children() : null;
			for (int i = 0; i < list2.Count; i++)
			{
				DT.Field field2 = list2[i];
				if (!string.IsNullOrEmpty(field2.key))
				{
					list.Add(new UIInGameObjectivesAndRules.Objective.ObjectiveScore(field2, this.objectiveDef));
				}
			}
			return list;
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x0022DF80 File Offset: 0x0022C180
		private bool PlayerHasTeammates()
		{
			Game game = GameLogic.Get(true);
			if (game == null)
			{
				return false;
			}
			if (!game.IsMultiplayer())
			{
				return false;
			}
			Game.Team team = game.teams.Get(BaseUI.LogicKingdom());
			return team != null && team.players.Count > 1;
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x0022DFC7 File Offset: 0x0022C1C7
		public static UIInGameObjectivesAndRules.Objective Create(DT.Field def, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent, false, "");
			gameObject.gameObject.SetActive(true);
			UIInGameObjectivesAndRules.Objective orAddComponent = gameObject.GetOrAddComponent<UIInGameObjectivesAndRules.Objective>();
			orAddComponent.SefDef(def);
			return orAddComponent;
		}

		// Token: 0x04003BCD RID: 15309
		[UIFieldTarget("id_TeamsContainer")]
		private RectTransform m_TeamsContainer;

		// Token: 0x04003BCE RID: 15310
		[UIFieldTarget("id_TeamPrototype")]
		private GameObject m_TeamPrototype;

		// Token: 0x04003BCF RID: 15311
		[UIFieldTarget("id_RuleLabel")]
		private TextMeshProUGUI m_RuleLabel;

		// Token: 0x04003BD0 RID: 15312
		[UIFieldTarget("id_ObjectiveIcon")]
		private Image m_ObjectiveIcon;

		// Token: 0x04003BD1 RID: 15313
		[UIFieldTarget("id_ObjectiveIconBorder")]
		private Image m_ObjectiveIconBorder;

		// Token: 0x04003BD2 RID: 15314
		[UIFieldTarget("is_Caption")]
		private Image m_Caption;

		// Token: 0x04003BD3 RID: 15315
		[UIFieldTarget("id_TotalScoreContainer")]
		private GameObject m_TotalScoreContainer;

		// Token: 0x04003BD4 RID: 15316
		[UIFieldTarget("id_TotalScore")]
		private TextMeshProUGUI m_TotalScore;

		// Token: 0x04003BD5 RID: 15317
		public DT.Field objectiveDef;

		// Token: 0x04003BD6 RID: 15318
		public Action<UIInGameObjectivesAndRules.Objective> OnSelect;

		// Token: 0x04003BD7 RID: 15319
		private Game m_Game;

		// Token: 0x04003BD8 RID: 15320
		private List<UIInGameObjectivesAndRules.Team> m_Teams = new List<UIInGameObjectivesAndRules.Team>();

		// Token: 0x04003BD9 RID: 15321
		private float m_LastUpdateTime;

		// Token: 0x04003BDA RID: 15322
		private float m_UpdateInterval = 2f;

		// Token: 0x04003BDB RID: 15323
		private bool m_Selected;

		// Token: 0x04003BDC RID: 15324
		private bool m_initialzied;

		// Token: 0x02000A28 RID: 2600
		public class ObjectiveScore : IVars
		{
			// Token: 0x06005599 RID: 21913 RVA: 0x00249D47 File Offset: 0x00247F47
			public ObjectiveScore(DT.Field def, DT.Field objective_def)
			{
				this.def = def;
				this.objectiveDef = objective_def;
			}

			// Token: 0x0600559A RID: 21914 RVA: 0x00249D5D File Offset: 0x00247F5D
			private int GetPlayerScore()
			{
				return (int)GameLogic.Get(true).rules.GetKingdomScorePerType(BaseUI.LogicKingdom(), this.objectiveDef.key, this.def.key);
			}

			// Token: 0x0600559B RID: 21915 RVA: 0x00249D8C File Offset: 0x00247F8C
			private int GetTeamScore()
			{
				Game game = GameLogic.Get(true);
				Game.Team team = game.teams.Get(BaseUI.LogicKingdom());
				return (int)game.rules.GetTeamScorePerType(team.id, this.objectiveDef.key, this.def.key);
			}

			// Token: 0x0600559C RID: 21916 RVA: 0x00249DD8 File Offset: 0x00247FD8
			public Value GetVar(string key, IVars vars = null, bool as_value = true)
			{
				if (key == "name")
				{
					return new Value(this.def.FindChild("name", null, true, true, true, '.'));
				}
				if (key == "player_score")
				{
					return this.GetPlayerScore();
				}
				if (!(key == "team_score"))
				{
					return Value.Unknown;
				}
				return this.GetTeamScore();
			}

			// Token: 0x0400469D RID: 18077
			public DT.Field def;

			// Token: 0x0400469E RID: 18078
			public DT.Field objectiveDef;
		}
	}

	// Token: 0x020007B1 RID: 1969
	internal class Rules : MonoBehaviour
	{
		// Token: 0x06004D5F RID: 19807 RVA: 0x0022E022 File Offset: 0x0022C222
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_RulePrototype != null)
			{
				this.m_RulePrototype.SetActive(false);
			}
			this.m_Initialzied = true;
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x0022E055 File Offset: 0x0022C255
		public void Populate()
		{
			this.Init();
			if (this.m_Decription != null)
			{
				UIText.SetTextKey(this.m_Decription, "MultiplyerObjectivesAndRulesWindow.rules.description", null, null);
			}
			this.PopulateRules();
		}

		// Token: 0x06004D61 RID: 19809 RVA: 0x0022E084 File Offset: 0x0022C284
		private void PopulateRules()
		{
			if (this.m_RulesContainer == null || this.m_RulePrototype == null)
			{
				return;
			}
			Campaign campaign = GameLogic.Get(false).campaign;
			if (campaign == null)
			{
				return;
			}
			DT.Field varsDef = campaign.GetVarsDef();
			if (varsDef == null || varsDef.children == null)
			{
				return;
			}
			List<DT.Field> list = new List<DT.Field>();
			for (int i = 0; i < varsDef.children.Count; i++)
			{
				DT.Field field = varsDef.children[i];
				if (!string.IsNullOrEmpty(field.key) && field.GetBool("show_in_rules", campaign, false, true, true, true, '.') && campaign.FindMatchingOption(field.key) != null)
				{
					list.Add(field);
				}
			}
			UICommon.DeleteActiveChildren(this.m_RulesContainer.transform);
			for (int j = 0; j < list.Count; j++)
			{
				DT.Field field2 = list[j];
				GameObject gameObject = global::Common.Spawn(this.m_RulePrototype, this.m_RulesContainer.transform, false, "");
				gameObject.SetActive(true);
				TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_RuleName");
				TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject, "id_RuleValue");
				string text = global::Defs.Localize(field2, "name", campaign, null, false, true);
				if (string.IsNullOrEmpty(text))
				{
					text = "{" + field2.key + "}";
				}
				Value var = campaign.GetVar(field2.key, null, true);
				string text2 = null;
				DT.Field field3 = campaign.FindMatchingOption(field2.key);
				if (field3 != null)
				{
					text2 = global::Defs.Localize(field3, "name", campaign, null, false, true);
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = DT.Unquote(var.ToString());
				}
				if (textMeshProUGUI != null)
				{
					UIText.SetText(textMeshProUGUI, text);
				}
				if (textMeshProUGUI2 != null)
				{
					UIText.SetText(textMeshProUGUI2, text2);
				}
			}
		}

		// Token: 0x04003BDD RID: 15325
		[UIFieldTarget("id_Decription")]
		private TextMeshProUGUI m_Decription;

		// Token: 0x04003BDE RID: 15326
		[UIFieldTarget("id_RulesContainer")]
		private GameObject m_RulesContainer;

		// Token: 0x04003BDF RID: 15327
		[UIFieldTarget("id_RulePrototype")]
		private GameObject m_RulePrototype;

		// Token: 0x04003BE0 RID: 15328
		private bool m_Initialzied;
	}
}
