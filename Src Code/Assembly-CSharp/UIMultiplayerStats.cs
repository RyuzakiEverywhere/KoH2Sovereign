using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025A RID: 602
public class UIMultiplayerStats : MonoBehaviour, IListener
{
	// Token: 0x06002518 RID: 9496 RVA: 0x0014A9E7 File Offset: 0x00148BE7
	public static void Rebuild()
	{
		UIMultiplayerStats.sm_ForceRebuild = true;
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x0014A9EF File Offset: 0x00148BEF
	private void Start()
	{
		base.StartCoroutine("WaitForValidGameState");
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x0014A9FD File Offset: 0x00148BFD
	private IEnumerator WaitForValidGameState()
	{
		yield return null;
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			if (GameLogic.Get(false) == null)
			{
				yield return null;
			}
			Game game = GameLogic.Get(false);
			if (((game != null) ? game.campaign : null) == null)
			{
				yield return null;
			}
			if (!Game.fullGameStateReceived)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		Game game2 = GameLogic.Get(true);
		bool? flag2;
		if (game2 == null)
		{
			flag2 = null;
		}
		else
		{
			Campaign campaign = game2.campaign;
			flag2 = ((campaign != null) ? new bool?(campaign.IsMultiplayerCampaign()) : null);
		}
		if ((flag2 ?? false) && !UIInGameChat.IsActive())
		{
			UIInGameChat.ToggleOpen();
			UIInGameChat.current.Close(true);
		}
		this.m_IgnoreUpdate = false;
		yield break;
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x0014AA0C File Offset: 0x00148C0C
	private void Init()
	{
		if (this.m_Initiazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Expand != null)
		{
			this.m_Expand.onClick = new BSGButton.OnClick(this.HandleExpand);
		}
		if (this.m_ToggleChat != null)
		{
			Tooltip.Get(this.m_ToggleChat.gameObject, true).SetDef("ChatToggleTooltip", null);
			this.m_ToggleChat.onClick = new BSGButton.OnClick(this.HandleOpenChat);
			this.m_ToggleChat.AllowSelection(true);
		}
		if (this.m_ToggleObjectivesAndRules != null)
		{
			Tooltip.Get(this.m_ToggleObjectivesAndRules.gameObject, true).SetDef("PlayerStatsToggleTooltip", null);
			this.m_ToggleObjectivesAndRules.onClick = new BSGButton.OnClick(this.HandleToggleObjectivesAndRules);
			this.m_ToggleObjectivesAndRules.AllowSelection(true);
		}
		if (this.m_TargetKingdom != null)
		{
			this.m_TargetKingdom.gameObject.SetActive(false);
			this.m_TargetKingdom.onClick = new BSGButton.OnClick(this.HandleTargetKingodmSelect);
		}
		if (this.m_PlayersContainer != null)
		{
			this.m_Players = this.m_PlayersContainer.GetOrAddComponent<UIMultiplayerStats.Players>();
		}
		UIMultiplayerStats.Players players = this.m_Players;
		if (players != null)
		{
			players.Hide();
		}
		this.m_Initiazlied = true;
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x0014AB55 File Offset: 0x00148D55
	private void Activate()
	{
		this.m_IgnoreUpdate = false;
		base.gameObject.SetActive(true);
		base.enabled = true;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x0014AB74 File Offset: 0x00148D74
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		Game game = this.m_Game;
		if (game != null)
		{
			game.DelListener(this);
		}
		Game game2 = this.m_Game;
		if (game2 != null)
		{
			Logic.Multiplayer multiplayer = game2.multiplayer;
			if (multiplayer != null)
			{
				multiplayer.DelListener(this);
			}
		}
		if (k != null)
		{
			this.m_Kingdom = k;
			this.m_Game = k.game;
		}
		else
		{
			this.m_Kingdom = null;
		}
		Game game3 = this.m_Game;
		if (game3 != null)
		{
			game3.AddListener(this);
		}
		Game game4 = this.m_Game;
		if (game4 != null)
		{
			Logic.Multiplayer multiplayer2 = game4.multiplayer;
			if (multiplayer2 != null)
			{
				multiplayer2.AddListener(this);
			}
		}
		if (this.m_Game.campaign == null)
		{
			return;
		}
		this.Refresh();
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x0014AC18 File Offset: 0x00148E18
	private void OnDestroy()
	{
		Game game = this.m_Game;
		if (game != null)
		{
			game.DelListener(this);
		}
		Game game2 = this.m_Game;
		if (game2 == null)
		{
			return;
		}
		Logic.Multiplayer multiplayer = game2.multiplayer;
		if (multiplayer == null)
		{
			return;
		}
		multiplayer.DelListener(this);
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x0014AC48 File Offset: 0x00148E48
	private void Update()
	{
		if (this.m_IgnoreUpdate)
		{
			return;
		}
		if (UIMultiplayerStats.sm_ForceRebuild)
		{
			this.SetKingdom(BaseUI.LogicKingdom());
			UIMultiplayerStats.sm_ForceRebuild = false;
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.m_Kingdom != kingdom && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
		}
		if (this.m_LastUpdate + this.m_RefreshRate < UnityEngine.Time.unscaledTime)
		{
			this.UpdateStats();
			this.m_LastUpdate = UnityEngine.Time.unscaledTime;
		}
		if (this.m_ToggleObjectivesAndRules != null)
		{
			this.m_ToggleObjectivesAndRules.SetSelected(UIInGameObjectivesAndRules.IsActive(), false);
		}
		if (this.m_ToggleChat != null)
		{
			this.m_ToggleChat.SetSelected(UIInGameChat.IsActiveAndShow(), false);
		}
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x0014ACFC File Offset: 0x00148EFC
	private void UpdateStats()
	{
		if (this.m_Game == null)
		{
			return;
		}
		string text;
		Color color;
		Game.CampaignRules.TimeLimits.Type type;
		bool flag = UIMultiplayerUtils.TryGetTimeLimitString(out text, out color, out type);
		if (this.m_Time != null)
		{
			this.m_Time.gameObject.SetActive(flag);
		}
		if (this.m_Background_Timer)
		{
			this.m_Background_Timer.gameObject.SetActive(flag);
		}
		if (this.m_Background_NoTimer)
		{
			this.m_Background_NoTimer.gameObject.SetActive(!flag);
		}
		if (this.m_TimeLeft != null)
		{
			this.m_TimeLeft.color = color;
			UIText.SetText(this.m_TimeLeft, text);
		}
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x0014ADA4 File Offset: 0x00148FA4
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

	// Token: 0x06002522 RID: 9506 RVA: 0x0014AE18 File Offset: 0x00149018
	private void Refresh()
	{
		if (this.m_Game.campaign == null || this.m_Game.rules == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Game game = this.m_Game;
		bool? flag;
		if (game == null)
		{
			flag = null;
		}
		else
		{
			Campaign campaign = game.campaign;
			flag = ((campaign != null) ? new bool?(campaign.IsMultiplayerCampaign()) : null);
		}
		bool flag2 = flag ?? false;
		if (this.m_Expand != null)
		{
			this.m_Expand.gameObject.SetActive(flag2);
		}
		if (this.m_Players != null)
		{
			if (flag2)
			{
				this.m_Players.Show();
				this.m_Players.Populate();
			}
			else
			{
				this.m_Players.gameObject.SetActive(false);
			}
		}
		if (this.m_ToggleChat != null)
		{
			this.m_ToggleChat.gameObject.SetActive(flag2);
		}
		this.CreateTimeTooltip();
		this.UpdateStats();
		this.RefreshTargetKingdom();
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x0014AF20 File Offset: 0x00149120
	private void RefreshTargetKingdom()
	{
		if (this.m_Game == null || this.m_Game.rules == null)
		{
			return;
		}
		if (this.m_Game.rules.main_goal != "DestroyKingdom")
		{
			BSGButton targetKingdom = this.m_TargetKingdom;
			if (targetKingdom == null)
			{
				return;
			}
			targetKingdom.gameObject.SetActive(false);
			return;
		}
		else
		{
			if (this.m_Game.rules.targetKingdom == null)
			{
				return;
			}
			Logic.Kingdom targetKingdom2 = this.m_Game.rules.targetKingdom;
			if (targetKingdom2 == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.m_Game.rules.main_goal))
			{
				return;
			}
			if (this.m_TargetKingdom != null)
			{
				this.m_TargetKingdom.gameObject.SetActive(true);
				if (this.m_TargetKingdomName != null)
				{
					UIText.SetText(this.m_TargetKingdomName, global::Defs.Localize(targetKingdom2.GetNameKey(null, ""), null, null, true, true));
				}
			}
			if (this.m_TargetKingdomIcon != null)
			{
				this.m_TargetKingdomIcon.SetObject(targetKingdom2, null);
			}
			return;
		}
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x0014B020 File Offset: 0x00149220
	private void HandleTargetKingodmSelect(BSGButton b)
	{
		Logic.Kingdom targetKingdom = this.m_Game.rules.targetKingdom;
		if (targetKingdom == null)
		{
			return;
		}
		WorldUI.Get().SelectObjFromLogic(targetKingdom, false, true);
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x0014B04F File Offset: 0x0014924F
	private void HandleExpand(BSGButton b)
	{
		UIMultiplayerStats.Players players = this.m_Players;
		if (players != null)
		{
			players.Show();
		}
		this.m_Expand.gameObject.SetActive(false);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x0014B073 File Offset: 0x00149273
	private void HandleOpenChat(BSGButton b)
	{
		UIInGameChat.ToggleOpen();
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x0014B07A File Offset: 0x0014927A
	private void HandleToggleObjectivesAndRules(BSGButton b)
	{
		UIInGameObjectivesAndRules.ToggleOpen();
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x0014B084 File Offset: 0x00149284
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "players_changed"))
		{
			if (message == "destroy_kingdom_target_selected")
			{
				this.RefreshTargetKingdom();
				return;
			}
			if (!(message == "main_goal_changed"))
			{
				return;
			}
			this.Refresh();
			this.RefreshTargetKingdom();
		}
		else
		{
			Game game = GameLogic.Get(false);
			if (game != null && game.campaign != null && game.campaign.IsMultiplayerCampaign())
			{
				this.Activate();
				this.SetKingdom(game.GetKingdom());
				return;
			}
		}
	}

	// Token: 0x0400193E RID: 6462
	[UIFieldTarget("id_Time")]
	private GameObject m_Time;

	// Token: 0x0400193F RID: 6463
	[UIFieldTarget("id_TimeLeft")]
	private TMP_Text m_TimeLeft;

	// Token: 0x04001940 RID: 6464
	[UIFieldTarget("id_Background_Timer")]
	private GameObject m_Background_Timer;

	// Token: 0x04001941 RID: 6465
	[UIFieldTarget("id_Background_NoTimer")]
	private GameObject m_Background_NoTimer;

	// Token: 0x04001942 RID: 6466
	[UIFieldTarget("id_ToggleChat")]
	private BSGButton m_ToggleChat;

	// Token: 0x04001943 RID: 6467
	[UIFieldTarget("id_ToggleObjectivesAndRules")]
	private BSGButton m_ToggleObjectivesAndRules;

	// Token: 0x04001944 RID: 6468
	[UIFieldTarget("id_TargetKingdom")]
	private BSGButton m_TargetKingdom;

	// Token: 0x04001945 RID: 6469
	[UIFieldTarget("id_TargetKingdomName")]
	private TMP_Text m_TargetKingdomName;

	// Token: 0x04001946 RID: 6470
	[UIFieldTarget("id_TargetKingdomIcon")]
	private UIKingdomIcon m_TargetKingdomIcon;

	// Token: 0x04001947 RID: 6471
	[UIFieldTarget("id_Expand")]
	private BSGButton m_Expand;

	// Token: 0x04001948 RID: 6472
	[UIFieldTarget("id_Players")]
	private GameObject m_PlayersContainer;

	// Token: 0x04001949 RID: 6473
	private UIMultiplayerStats.Players m_Players;

	// Token: 0x0400194A RID: 6474
	private Game m_Game;

	// Token: 0x0400194B RID: 6475
	private Logic.Kingdom m_Kingdom;

	// Token: 0x0400194C RID: 6476
	private float m_LastUpdate;

	// Token: 0x0400194D RID: 6477
	private float m_RefreshRate = 1f;

	// Token: 0x0400194E RID: 6478
	private bool m_IgnoreUpdate = true;

	// Token: 0x0400194F RID: 6479
	private bool m_Initiazlied;

	// Token: 0x04001950 RID: 6480
	private static bool sm_ForceRebuild;

	// Token: 0x020007B3 RID: 1971
	internal class Players : MonoBehaviour
	{
		// Token: 0x06004D66 RID: 19814 RVA: 0x0022E26C File Offset: 0x0022C46C
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
			if (this.m_SeparatorPrototype != null)
			{
				this.m_SeparatorPrototype.gameObject.SetActive(false);
			}
			if (this.m_EmptySlotPrototype != null)
			{
				this.m_EmptySlotPrototype.gameObject.SetActive(false);
			}
			if (this.m_ButtonHide != null)
			{
				this.m_ButtonHide.onClick = new BSGButton.OnClick(this.HandleOnHide);
			}
			this.m_Game = GameLogic.Get(false);
			this.m_initialzied = true;
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x0022E320 File Offset: 0x0022C520
		public void Populate()
		{
			this.Init();
			int num = 6;
			for (int i = 0; i < this.m_Teams.Count; i++)
			{
				if (this.m_Teams[i] != null && this.m_Teams[i].gameObject != null && this.m_Teams[i].gameObject != null)
				{
					global::Common.DestroyObj(this.m_Teams[i].gameObject);
				}
			}
			for (int j = 0; j < this.m_Separators.Count; j++)
			{
				global::Common.DestroyObj(this.m_Separators[j]);
			}
			for (int k = 0; k < this.m_EmptySlots.Count; k++)
			{
				global::Common.DestroyObj(this.m_EmptySlots[k]);
			}
			this.m_Teams.Clear();
			this.m_Separators.Clear();
			this.m_EmptySlots.Clear();
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
			int l = 0;
			List<Game.Team> list = new List<Game.Team>(this.m_Game.teams.teams);
			list.Sort((Game.Team x, Game.Team y) => x.id.CompareTo(y.id));
			int m = 0;
			int count = this.m_Game.teams.teams.Count;
			while (m < count)
			{
				Game.Team team = list[m];
				UIMultiplayerStats.Team team2 = UIMultiplayerStats.Team.Create(team.id, this.m_TeamPrototype, this.m_TeamsContainer);
				if (!(team2 == null))
				{
					if (m < count - 1)
					{
						GameObject gameObject = global::Common.Spawn(this.m_SeparatorPrototype, this.m_TeamsContainer, false, "");
						if (gameObject != null)
						{
							this.m_Separators.Add(gameObject);
							gameObject.gameObject.SetActive(true);
						}
					}
					l += team.players.Count;
					this.m_Teams.Add(team2);
				}
				m++;
			}
			while (l < num)
			{
				l++;
				GameObject gameObject2 = global::Common.Spawn(this.m_EmptySlotPrototype, this.m_EmptySlotsContainer, false, "");
				Tooltip.Get(gameObject2, true).SetDef("InvateFriendTooltip", null);
				gameObject2.gameObject.SetActive(true);
				this.m_EmptySlots.Add(gameObject2);
			}
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x0022E5A7 File Offset: 0x0022C7A7
		private void HandleOnHide(BSGButton b)
		{
			this.Hide();
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x0022E5AF File Offset: 0x0022C7AF
		public void Show()
		{
			base.gameObject.SetActive(true);
			UIMultiplayerStats component = base.transform.parent.GetComponent<UIMultiplayerStats>();
			if (component == null)
			{
				return;
			}
			component.m_Expand.gameObject.SetActive(false);
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x0022E5E2 File Offset: 0x0022C7E2
		public void Hide()
		{
			base.gameObject.SetActive(false);
			UIMultiplayerStats component = base.transform.parent.GetComponent<UIMultiplayerStats>();
			if (component == null)
			{
				return;
			}
			component.m_Expand.gameObject.SetActive(true);
		}

		// Token: 0x04003BE3 RID: 15331
		[UIFieldTarget("id_TeamPrototype")]
		private GameObject m_TeamPrototype;

		// Token: 0x04003BE4 RID: 15332
		[UIFieldTarget("id_TeamsContainer")]
		private RectTransform m_TeamsContainer;

		// Token: 0x04003BE5 RID: 15333
		[UIFieldTarget("id_SeparatorPrototype")]
		private GameObject m_SeparatorPrototype;

		// Token: 0x04003BE6 RID: 15334
		[UIFieldTarget("id_EmptySlotsContainer")]
		private RectTransform m_EmptySlotsContainer;

		// Token: 0x04003BE7 RID: 15335
		[UIFieldTarget("id_EmptySlotPrototype")]
		private GameObject m_EmptySlotPrototype;

		// Token: 0x04003BE8 RID: 15336
		[UIFieldTarget("id_ButtonHide")]
		private BSGButton m_ButtonHide;

		// Token: 0x04003BE9 RID: 15337
		private List<UIMultiplayerStats.Team> m_Teams = new List<UIMultiplayerStats.Team>();

		// Token: 0x04003BEA RID: 15338
		private List<GameObject> m_Separators = new List<GameObject>();

		// Token: 0x04003BEB RID: 15339
		private List<GameObject> m_EmptySlots = new List<GameObject>();

		// Token: 0x04003BEC RID: 15340
		private Game m_Game;

		// Token: 0x04003BED RID: 15341
		private bool m_initialzied;
	}

	// Token: 0x020007B4 RID: 1972
	internal class Team : MonoBehaviour
	{
		// Token: 0x06004D6C RID: 19820 RVA: 0x0022E63E File Offset: 0x0022C83E
		public void SetData(int id)
		{
			UICommon.FindComponents(this, false);
			if (this.m_PlayerInfoProtoype != null)
			{
				this.m_PlayerInfoProtoype.gameObject.SetActive(false);
			}
			this.m_Team = id;
			this.Populate();
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x0022E674 File Offset: 0x0022C874
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
			}
			WorldUI worldUI = WorldUI.Get();
			bool flag = worldUI != null && worldUI.menu.gameObject.activeInHierarchy;
			if (KeyBindings.GetBindDown("toggle_chat") && !flag)
			{
				if (UIInGameChat.current == null)
				{
					UIInGameChat.ToggleOpen();
				}
				else
				{
					UIInGameChat.current.Show();
				}
			}
			if (KeyBindings.GetBindDown("toggle_score") && !flag)
			{
				UIInGameObjectivesAndRules.ToggleOpen();
			}
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x0022E70C File Offset: 0x0022C90C
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
				global::Common.DestroyObj(base.gameObject);
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
				UIMultiplayerStats.Player player = UIMultiplayerStats.Player.Create(players[i].id, this.m_PlayerInfoProtoype, this.m_PlayersContianer);
				if (!(player == null))
				{
					this.m_Players.Add(player);
				}
			}
			this.RefreshData();
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x0022E7F8 File Offset: 0x0022C9F8
		private void RefreshData()
		{
			if (this.m_TeamPrimaryScore != null)
			{
				UIText.SetText(this.m_TeamPrimaryScore, Mathf.Round(this.GetTeamScore()).ToString());
			}
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x0022E834 File Offset: 0x0022CA34
		private bool IsFFA()
		{
			Game game = GameLogic.Get(true);
			return game.rules == null || game.rules.team_size.is_unknown || game.rules.team_size == 1;
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x0022E87C File Offset: 0x0022CA7C
		private float GetTeamScore()
		{
			Game game = GameLogic.Get(false);
			if (game == null || game.rules == null)
			{
				return 0f;
			}
			return game.rules.GetTeamScore(this.m_Team, null);
		}

		// Token: 0x06004D72 RID: 19826 RVA: 0x0022E8B4 File Offset: 0x0022CAB4
		public static UIMultiplayerStats.Team Create(int id, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent.transform, false, "");
			gameObject.gameObject.SetActive(true);
			UIMultiplayerStats.Team team = gameObject.AddComponent<UIMultiplayerStats.Team>();
			team.SetData(id);
			return team;
		}

		// Token: 0x04003BEE RID: 15342
		[UIFieldTarget("id_TeamPrimaryScore")]
		private TMP_Text m_TeamPrimaryScore;

		// Token: 0x04003BEF RID: 15343
		[UIFieldTarget("id_PlayerIconPrototype")]
		private GameObject m_PlayerInfoProtoype;

		// Token: 0x04003BF0 RID: 15344
		[UIFieldTarget("id_PlayersContianer")]
		private RectTransform m_PlayersContianer;

		// Token: 0x04003BF1 RID: 15345
		[UIFieldTarget("id_TeamData")]
		private RectTransform m_TeamData;

		// Token: 0x04003BF2 RID: 15346
		private int m_Team;

		// Token: 0x04003BF3 RID: 15347
		private List<UIMultiplayerStats.Player> m_Players = new List<UIMultiplayerStats.Player>();

		// Token: 0x04003BF4 RID: 15348
		private float m_LastUpdateTime;

		// Token: 0x04003BF5 RID: 15349
		private float m_UpdateInterval = 2f;
	}

	// Token: 0x020007B5 RID: 1973
	internal class Player : MonoBehaviour
	{
		// Token: 0x06004D74 RID: 19828 RVA: 0x0022E920 File Offset: 0x0022CB20
		public void SetMemeber(string playerId)
		{
			UICommon.FindComponents(this, false);
			BSGButton component = base.GetComponent<BSGButton>();
			if (component != null)
			{
				component.onClick = new BSGButton.OnClick(this.HandleOnClock);
			}
			this.playerId = playerId;
			this.Populate();
			this.RefreshData();
		}

		// Token: 0x06004D75 RID: 19829 RVA: 0x0022E969 File Offset: 0x0022CB69
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
			}
		}

		// Token: 0x06004D76 RID: 19830 RVA: 0x0022E990 File Offset: 0x0022CB90
		private void Populate()
		{
			Game game = GameLogic.Get(true);
			Game.Player playerById = game.teams.GetPlayerById(this.playerId);
			Vars vars = new Vars(playerById);
			if (!CampaignUtils.IsFFA(game.campaign))
			{
				vars.Set<string>("team_color", "#" + ColorUtility.ToHtmlStringRGB(global::Defs.GetColor(playerById.GetTeamId(), "LobbyPlayerWindow", "team_color")));
			}
			Tooltip.Get(base.gameObject, true).SetDef("PlayerInfoTooltip", vars);
			if (this.m_Generations != null)
			{
				this.m_Generations.gameObject.SetActive(UIMultiplayerUtils.HasGenerationLimit());
			}
		}

		// Token: 0x06004D77 RID: 19831 RVA: 0x0022EA34 File Offset: 0x0022CC34
		private void RefreshData()
		{
			Logic.Kingdom playerKingdom = UIMultiplayerUtils.GetPlayerKingdom(this.playerId);
			bool flag = UIMultiplayerUtils.IsDefeated(this.playerId);
			bool flag2 = UIMultiplayerUtils.IsEleminated(this.playerId);
			if (this.m_ConnectionStatus)
			{
				this.m_ConnectionStatus.sprite = global::Defs.GetObj<Sprite>("PlayerConnectionStatusIconSettings", UIMultiplayerUtils.GetStatus(this.playerId), null);
			}
			if (this.m_KingdomDefeatedIcon != null)
			{
				this.m_KingdomDefeatedIcon.gameObject.SetActive(flag);
			}
			if (this.m_PlayerEleminated != null)
			{
				this.m_PlayerEleminated.gameObject.SetActive(flag2);
			}
			if (this.m_KingsIcon != null)
			{
				this.m_KingsIcon.gameObject.SetActive(!flag && !flag2);
				this.m_KingsIcon.SetObject((playerKingdom != null) ? playerKingdom.GetKing() : null, null);
			}
			if (this.m_GenerationsLabel != null)
			{
				if (playerKingdom != null)
				{
					playerKingdom.generationsPassed.ToString();
				}
				UIText.SetText(this.m_GenerationsLabel, (playerKingdom != null) ? playerKingdom.generationsPassed.ToString() : null);
			}
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x0022EB4C File Offset: 0x0022CD4C
		private void HandleOnClock(BSGButton b)
		{
			Logic.Kingdom playerKingdom = UIMultiplayerUtils.GetPlayerKingdom(this.playerId);
			WorldUI worldUI = WorldUI.Get();
			if (worldUI != null)
			{
				worldUI.DoubleClickCheck();
				if (worldUI.selected_kingdom == null || worldUI.selected_kingdom.id != playerKingdom.id)
				{
					worldUI.SelectKingdom(playerKingdom.id, true);
					return;
				}
				if (worldUI.dblclk && worldUI.selected_kingdom != null && worldUI.selected_kingdom.logic != null)
				{
					Logic.Realm capital = worldUI.selected_kingdom.logic.GetCapital();
					if (capital != null)
					{
						worldUI.LookAt(capital.castle.position, false);
					}
				}
			}
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x0022EBEC File Offset: 0x0022CDEC
		public static UIMultiplayerStats.Player Create(string id, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = global::Common.Spawn(prototype, parent.transform, false, "");
			gameObject.gameObject.SetActive(true);
			gameObject.AddComponent<UIMultiplayerStats.Player>().SetMemeber(id);
			return null;
		}

		// Token: 0x04003BF6 RID: 15350
		[UIFieldTarget("id_KingdomDefeated")]
		private Image m_KingdomDefeatedIcon;

		// Token: 0x04003BF7 RID: 15351
		[UIFieldTarget("id_PlayerEleminated")]
		private Image m_PlayerEleminated;

		// Token: 0x04003BF8 RID: 15352
		[UIFieldTarget("id_ConnectionStatus")]
		private Image m_ConnectionStatus;

		// Token: 0x04003BF9 RID: 15353
		[UIFieldTarget("id_KingsIcon")]
		private UICharacterIcon m_KingsIcon;

		// Token: 0x04003BFA RID: 15354
		[UIFieldTarget("id_Generations")]
		private GameObject m_Generations;

		// Token: 0x04003BFB RID: 15355
		[UIFieldTarget("id_GenerationsLabel")]
		private TextMeshProUGUI m_GenerationsLabel;

		// Token: 0x04003BFC RID: 15356
		public string playerId;

		// Token: 0x04003BFD RID: 15357
		private float m_LastUpdateTime;

		// Token: 0x04003BFE RID: 15358
		private float m_UpdateInterval = 2f;
	}
}
