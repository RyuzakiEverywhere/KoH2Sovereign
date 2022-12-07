using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025E RID: 606
internal class UIPlayerRanking : MonoBehaviour
{
	// Token: 0x0600254A RID: 9546 RVA: 0x0014B9E1 File Offset: 0x00149BE1
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		this.Kingdom = k;
		this.game = k.game;
		this.Populate();
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x0014BA02 File Offset: 0x00149C02
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
		this.m_initialzied = true;
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x000023FD File Offset: 0x000005FD
	private void Update()
	{
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x000023FD File Offset: 0x000005FD
	private void RefreshData()
	{
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x0014BA3C File Offset: 0x00149C3C
	private void Populate()
	{
		for (int i = 0; i < this.m_Teams.Count; i++)
		{
			if (this.m_Teams[i] != null && this.m_Teams[i].gameObject != null && this.m_Teams[i].gameObject != null)
			{
				UnityEngine.Object.Destroy(this.m_Teams[i].gameObject);
			}
		}
		this.m_Teams.Clear();
		if (this.m_TeamPrototype == null)
		{
			return;
		}
		if (this.game == null)
		{
			return;
		}
		if (this.game.teams == null || this.game.teams.teams.Count == 0)
		{
			return;
		}
		foreach (Game.Team team in this.game.teams.teams)
		{
			UIPlayerRanking.Team team2 = UIPlayerRanking.Team.Create(team.id, this.m_TeamPrototype, base.transform as RectTransform);
			if (!(team2 == null))
			{
				this.m_Teams.Add(team2);
			}
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x0014BB7C File Offset: 0x00149D7C
	public void ToggleShow()
	{
		base.gameObject.SetActive(!base.gameObject.activeSelf);
		this.Populate();
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x0014BB9D File Offset: 0x00149D9D
	public void HideWindow()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001962 RID: 6498
	[UIFieldTarget("id_TeamPrototype")]
	private GameObject m_TeamPrototype;

	// Token: 0x04001963 RID: 6499
	private bool m_initialzied;

	// Token: 0x04001964 RID: 6500
	public Logic.Kingdom Kingdom;

	// Token: 0x04001965 RID: 6501
	private Game game;

	// Token: 0x04001966 RID: 6502
	private List<UIPlayerRanking.Team> m_Teams = new List<UIPlayerRanking.Team>();

	// Token: 0x020007B8 RID: 1976
	internal class Team : MonoBehaviour
	{
		// Token: 0x06004D87 RID: 19847 RVA: 0x0022EEC3 File Offset: 0x0022D0C3
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

		// Token: 0x06004D88 RID: 19848 RVA: 0x0022EEF8 File Offset: 0x0022D0F8
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.time)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.time;
			}
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x0022EF20 File Offset: 0x0022D120
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
				UIPlayerRanking.Player player = UIPlayerRanking.Player.Create(players[i].id, this.m_PlayerInfoProtoype, this.m_PlayersContianer);
				if (!(player == null))
				{
					this.m_Players.Add(player);
				}
			}
			UIText.SetText(this.m_TeamName, "Team " + (this.m_Team + 1));
			this.CheckScoreTypeIcon();
			this.RefreshData();
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x0022F034 File Offset: 0x0022D234
		private void RefreshData()
		{
			if (this.m_TeamPrimaryScore != null)
			{
				UIText.SetText(this.m_TeamPrimaryScore, Mathf.Round(this.GetTeamScore()).ToString());
			}
			if (this.m_TeamSecondaryScore != null)
			{
				UIText.SetText(this.m_TeamSecondaryScore, "-");
			}
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x0022F08C File Offset: 0x0022D28C
		private void CheckScoreTypeIcon()
		{
			Game game = GameLogic.Get(true);
			if (game == null || game.rules != null)
			{
				return;
			}
			if (this.m_ScoreTypeIcon != null)
			{
				this.m_ScoreTypeIcon.SetActive(game.rules.main_goal == "HaveXGold");
			}
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x0022F0DC File Offset: 0x0022D2DC
		private bool IsFFA()
		{
			Game game = GameLogic.Get(true);
			return game.rules == null || game.rules.team_size.is_unknown || game.rules.team_size == 1;
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x0022F124 File Offset: 0x0022D324
		private float GetTeamScore()
		{
			Game game = GameLogic.Get(false);
			if (game == null || game.rules == null)
			{
				return 0f;
			}
			return game.rules.GetTeamScore(this.m_Team, null);
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x0022F15B File Offset: 0x0022D35B
		public static UIPlayerRanking.Team Create(int id, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
			gameObject.gameObject.SetActive(true);
			UIPlayerRanking.Team team = gameObject.AddComponent<UIPlayerRanking.Team>();
			team.SetData(id);
			return team;
		}

		// Token: 0x04003C07 RID: 15367
		[UIFieldTarget("id_TeamName")]
		private TMP_Text m_TeamName;

		// Token: 0x04003C08 RID: 15368
		[UIFieldTarget("id_TeamPrimaryScore")]
		private TMP_Text m_TeamPrimaryScore;

		// Token: 0x04003C09 RID: 15369
		[UIFieldTarget("id_TeamSecondaryScore")]
		private TMP_Text m_TeamSecondaryScore;

		// Token: 0x04003C0A RID: 15370
		[UIFieldTarget("id_ScoreTypeIcon")]
		private GameObject m_ScoreTypeIcon;

		// Token: 0x04003C0B RID: 15371
		[UIFieldTarget("id_PlayerInfoProtoype")]
		private GameObject m_PlayerInfoProtoype;

		// Token: 0x04003C0C RID: 15372
		[UIFieldTarget("id_PlayersContianer")]
		private RectTransform m_PlayersContianer;

		// Token: 0x04003C0D RID: 15373
		[UIFieldTarget("id_TeamData")]
		private RectTransform m_TeamData;

		// Token: 0x04003C0E RID: 15374
		private int m_Team;

		// Token: 0x04003C0F RID: 15375
		private List<UIPlayerRanking.Player> m_Players = new List<UIPlayerRanking.Player>();

		// Token: 0x04003C10 RID: 15376
		private float m_LastUpdateTime;

		// Token: 0x04003C11 RID: 15377
		private float m_UpdateInterval = 5f;
	}

	// Token: 0x020007B9 RID: 1977
	internal class Player : MonoBehaviour
	{
		// Token: 0x06004D90 RID: 19856 RVA: 0x0022F1B0 File Offset: 0x0022D3B0
		public void SetMemeber(string playerId)
		{
			UICommon.FindComponents(this, false);
			this.playerId = playerId;
			this.Populate();
			this.RefreshData();
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x0022F1CC File Offset: 0x0022D3CC
		private void Update()
		{
			if (this.m_LastUpdateTime + this.m_UpdateInterval < UnityEngine.Time.unscaledTime)
			{
				this.RefreshData();
				this.m_LastUpdateTime = UnityEngine.Time.unscaledTime;
			}
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x0022F1F4 File Offset: 0x0022D3F4
		private void Populate()
		{
			if (this.m_PlayerName != null)
			{
				UIText.SetText(this.m_PlayerName, this.ExtractPlayerName());
			}
			if (this.m_KingdomIcon != null)
			{
				Logic.Kingdom kingdom = this.ExtractPlayerKingdom();
				if (kingdom != null)
				{
					this.m_KingdomIcon.SetObject(kingdom, null);
				}
			}
			if (this.m_KingdomDefeatedIcon != null)
			{
				this.m_KingdomDefeatedIcon.sprite = global::Defs.GetObj<Sprite>("MultiplayerStats", "player_defeated_icon", null);
			}
			this.CheckScoreTypeIcon();
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x0022F274 File Offset: 0x0022D474
		private void RefreshData()
		{
			if (this.n_PrimaryScore != null)
			{
				UIText.SetText(this.n_PrimaryScore, ((int)this.GetMainScore()).ToString());
			}
			if (this.m_SecondaryScore != null)
			{
				UIText.SetText(this.m_SecondaryScore, "-");
			}
			this.m_PlayerName.color = (this.IsOffline() ? Color.gray : Color.white);
			this.m_ConnectionStatus.sprite = global::Defs.GetObj<Sprite>("PlayerConnectionStatusIconSettings", this.GetStatus().ToString(), null);
			this.m_KingdomDefeatedIcon.gameObject.SetActive(this.IsDefeated());
		}

		// Token: 0x06004D94 RID: 19860 RVA: 0x0022F328 File Offset: 0x0022D528
		private void CheckScoreTypeIcon()
		{
			Game game = GameLogic.Get(false);
			if (game == null || game.rules != null)
			{
				return;
			}
			if (this.m_ScoreTypeIcon != null)
			{
				this.m_ScoreTypeIcon.SetActive(game.rules.main_goal == "HaveXGold");
			}
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x0022F378 File Offset: 0x0022D578
		private UIPlayerRanking.Player.Status GetStatus()
		{
			UIPlayerRanking.Player.Status result;
			if (this.IsOffline())
			{
				result = UIPlayerRanking.Player.Status.Not_Connected;
			}
			else if (CampaignUtils.IsPlayerLoaded(GameLogic.Get(false), this.playerId))
			{
				result = UIPlayerRanking.Player.Status.Playing;
			}
			else
			{
				result = UIPlayerRanking.Player.Status.Loading;
			}
			return result;
		}

		// Token: 0x06004D96 RID: 19862 RVA: 0x0022F3AC File Offset: 0x0022D5AC
		private bool IsOffline()
		{
			Game game = GameLogic.Get(false);
			return game == null || game.multiplayer == null || (game.multiplayer.type != Logic.Multiplayer.Type.Server && !game.multiplayer.IsOnline()) || Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(this.playerId) == null;
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x0022F3F8 File Offset: 0x0022D5F8
		public bool IsDefeated()
		{
			Game game = GameLogic.Get(false);
			if (game == null)
			{
				return true;
			}
			Logic.Kingdom kingdom = Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(this.playerId);
			if (kingdom != null)
			{
				return kingdom.IsDefeated();
			}
			for (int i = 0; i < game.campaign.playerDataPersistent.Length; i++)
			{
				if (!(game.campaign.playerDataPersistent[i].GetVar("id", null, true) != this.playerId))
				{
					string kingdomName = game.campaign.GetKingdomName(i, false);
					if (!string.IsNullOrEmpty(kingdomName))
					{
						kingdom = game.GetKingdom(kingdomName);
						if (kingdom != null)
						{
							return kingdom.IsDefeated();
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x0022F494 File Offset: 0x0022D694
		public float GetMainScore()
		{
			Game game = GameLogic.Get(true);
			if (game == null || game.rules == null)
			{
				return 0f;
			}
			if (string.IsNullOrEmpty(this.playerId))
			{
				return 0f;
			}
			Logic.Kingdom kingdomByGUID = Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(this.playerId);
			if (kingdomByGUID == null)
			{
				return 0f;
			}
			return game.rules.GetKingdomScore(kingdomByGUID, game.rules.mainGoalScoreModifiers);
		}

		// Token: 0x06004D99 RID: 19865 RVA: 0x0022F4F8 File Offset: 0x0022D6F8
		private string ExtractPlayerName()
		{
			string playerName = PlayerInfo.GetPlayerName(this.playerId);
			if (!string.IsNullOrEmpty(playerName))
			{
				return playerName;
			}
			return "unknown";
		}

		// Token: 0x06004D9A RID: 19866 RVA: 0x0022F520 File Offset: 0x0022D720
		private Logic.Kingdom ExtractPlayerKingdom()
		{
			Logic.Kingdom kingdom = Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(this.playerId);
			if (kingdom == null)
			{
				Game game = GameLogic.Get(true);
				if (game != null && game.campaign != null && game.campaign.playerDataPersistent != null)
				{
					for (int i = 0; i < game.campaign.playerDataPersistent.Length; i++)
					{
						if (game.campaign.playerDataPersistent[i].GetVar("id", null, true).String(null) == this.playerId)
						{
							string kingdomName = game.campaign.GetKingdomName(i, false);
							if (!string.IsNullOrEmpty(kingdomName))
							{
								kingdom = game.GetKingdom(kingdomName);
								break;
							}
						}
					}
				}
			}
			return kingdom;
		}

		// Token: 0x06004D9B RID: 19867 RVA: 0x0022F5C9 File Offset: 0x0022D7C9
		public static UIPlayerRanking.Player Create(string id, GameObject prototype, RectTransform parent)
		{
			if (prototype == null)
			{
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, parent);
			gameObject.gameObject.SetActive(true);
			gameObject.AddComponent<UIPlayerRanking.Player>().SetMemeber(id);
			return null;
		}

		// Token: 0x04003C12 RID: 15378
		[UIFieldTarget("id_KingdomDefeated")]
		private Image m_KingdomDefeatedIcon;

		// Token: 0x04003C13 RID: 15379
		[UIFieldTarget("id_Kingdom")]
		private UIKingdomIcon m_KingdomIcon;

		// Token: 0x04003C14 RID: 15380
		[UIFieldTarget("id_ConnectionStatus")]
		private Image m_ConnectionStatus;

		// Token: 0x04003C15 RID: 15381
		[UIFieldTarget("id_PlayerName")]
		private TMP_Text m_PlayerName;

		// Token: 0x04003C16 RID: 15382
		[UIFieldTarget("id_PrimaryScore")]
		private TMP_Text n_PrimaryScore;

		// Token: 0x04003C17 RID: 15383
		[UIFieldTarget("id_ScoreTypeIcon")]
		private GameObject m_ScoreTypeIcon;

		// Token: 0x04003C18 RID: 15384
		[UIFieldTarget("id_SecondaryScore")]
		private TMP_Text m_SecondaryScore;

		// Token: 0x04003C19 RID: 15385
		public string playerId;

		// Token: 0x04003C1A RID: 15386
		private float m_LastUpdateTime;

		// Token: 0x04003C1B RID: 15387
		private float m_UpdateInterval = 2f;

		// Token: 0x02000A2B RID: 2603
		private enum Status
		{
			// Token: 0x040046A4 RID: 18084
			Not_Connected,
			// Token: 0x040046A5 RID: 18085
			Loading,
			// Token: 0x040046A6 RID: 18086
			Playing
		}
	}
}
