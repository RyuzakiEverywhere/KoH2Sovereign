using System;
using Logic;
using UnityEngine;

// Token: 0x0200025B RID: 603
public static class UIMultiplayerUtils
{
	// Token: 0x0600252B RID: 9515 RVA: 0x0014B118 File Offset: 0x00149318
	public static DT.Field GetRuleOption(Campaign campaign, string rule_key)
	{
		if (campaign == null)
		{
			return null;
		}
		if (campaign.campaignData == null)
		{
			return null;
		}
		if (string.IsNullOrWhiteSpace(rule_key))
		{
			return null;
		}
		Value var = campaign.campaignData.GetVar(rule_key, null, true);
		DT.Field varOptions = campaign.GetVarOptions(rule_key);
		if (varOptions != null && varOptions.children != null)
		{
			for (int i = 0; i < varOptions.children.Count; i++)
			{
				DT.Field field = varOptions.children[i];
				if (!string.IsNullOrEmpty(field.key))
				{
					Value optionValue = campaign.GetOptionValue(field);
					if (optionValue.is_valid && optionValue == var)
					{
						return field;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x0014B1AF File Offset: 0x001493AF
	public static string GetStatus(string playerId)
	{
		if (UIMultiplayerUtils.IsOffline(playerId))
		{
			return "not_connected";
		}
		if (CampaignUtils.IsPlayerLoaded(GameLogic.Get(false), playerId))
		{
			return "playing";
		}
		return "loading";
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x0014B1D8 File Offset: 0x001493D8
	public static bool IsOffline(string playerId)
	{
		Game game = GameLogic.Get(false);
		return game == null || game.multiplayer == null || (game.multiplayer.type != Logic.Multiplayer.Type.Server && !game.multiplayer.IsOnline()) || Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(playerId) == null;
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x0014B220 File Offset: 0x00149420
	public static bool IsDefeated(string playerId)
	{
		Logic.Kingdom playerKingdom = UIMultiplayerUtils.GetPlayerKingdom(playerId);
		return playerKingdom == null || playerKingdom.IsDefeated();
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x0014B240 File Offset: 0x00149440
	public static bool IsEleminated(string playerId)
	{
		Game game = GameLogic.Get(false);
		return game != null && game.campaign != null && game.campaign.GetSlotState(playerId) == Campaign.SlotState.Eliminated;
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x0014B270 File Offset: 0x00149470
	public static float GetScore(string playerId, string rule_name = null)
	{
		Game game = GameLogic.Get(true);
		if (game == null || game.rules == null)
		{
			return 0f;
		}
		if (string.IsNullOrEmpty(playerId))
		{
			return 0f;
		}
		Logic.Kingdom playerKingdom = UIMultiplayerUtils.GetPlayerKingdom(playerId);
		return game.rules.GetKingdomScore(playerKingdom, rule_name);
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x0014B2B8 File Offset: 0x001494B8
	public static string ExtractPlayerName(string playerId)
	{
		string playerName = PlayerInfo.GetPlayerName(playerId);
		if (!string.IsNullOrEmpty(playerName))
		{
			return playerName;
		}
		return global::Defs.Localize("Player.default_player_name", null, null, true, true);
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x0014B2E4 File Offset: 0x001494E4
	public static Logic.Kingdom GetPlayerKingdom(string playerId)
	{
		Logic.Kingdom kingdom = Logic.Multiplayer.CurrentPlayers.GetKingdomByGUID(playerId);
		if (kingdom != null)
		{
			return kingdom;
		}
		Game game = GameLogic.Get(true);
		if (game == null || game.campaign == null || game.campaign.playerDataPersistent == null)
		{
			return kingdom;
		}
		for (int i = 0; i < game.campaign.playerDataPersistent.Length; i++)
		{
			if (game.campaign.playerDataPersistent[i].GetVar("id", null, true).String(null) == playerId)
			{
				string kingdomName = game.campaign.GetKingdomName(i, false);
				if (!string.IsNullOrEmpty(kingdomName))
				{
					kingdom = game.GetKingdom(kingdomName);
					break;
				}
			}
		}
		return kingdom;
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x0014B384 File Offset: 0x00149584
	public static bool HasGenerationLimit()
	{
		Game game = GameLogic.Get(false);
		Game.CampaignRules campaignRules = (game != null) ? game.rules : null;
		return campaignRules != null && campaignRules.time_limits.type == Game.CampaignRules.TimeLimits.Type.Generations;
	}

	// Token: 0x06002534 RID: 9524 RVA: 0x0014B3B8 File Offset: 0x001495B8
	public static bool TryGetTimeLimitString(out string time, out Color color, out Game.CampaignRules.TimeLimits.Type type)
	{
		time = string.Empty;
		color = Color.white;
		type = Game.CampaignRules.TimeLimits.Type.None;
		Game game = GameLogic.Get(true);
		bool campaign = game.campaign != null;
		Game.CampaignRules rules = game.rules;
		if (!campaign)
		{
			return false;
		}
		type = rules.time_limits.type;
		if (rules.time_limits.type == Game.CampaignRules.TimeLimits.Type.None)
		{
			return false;
		}
		time = "-";
		bool result = false;
		if (rules.time_limits.type == Game.CampaignRules.TimeLimits.Type.Time)
		{
			long num = (long)(rules.time_limits.value * 60) - game.session_time.milliseconds / 1000L;
			float num2 = (float)num / (float)(rules.time_limits.value * 60);
			time = UIMultiplayerUtils.SecoundsToShortString((int)num);
			color = UIMultiplayerUtils.GetTimeColor(1f - num2);
			result = true;
		}
		if (rules.time_limits.type == Game.CampaignRules.TimeLimits.Type.Generations)
		{
			int num3 = -1;
			for (int i = 0; i < game.campaign.playerDataPersistent.Length; i++)
			{
				string kingdomName = game.campaign.GetKingdomName(i, false);
				if (!string.IsNullOrEmpty(kingdomName))
				{
					Logic.Kingdom kingdom = game.GetKingdom(kingdomName);
					if (kingdom != null)
					{
						if (num3 == -1)
						{
							num3 = kingdom.generationsPassed;
						}
						else if (kingdom.generationsPassed < num3)
						{
							num3 = kingdom.generationsPassed;
						}
					}
				}
			}
			Vars vars = new Vars();
			vars.Set<int>("genrations_passes", Mathf.Min(num3, rules.time_limits.value));
			vars.Set<int>("generations_total", rules.time_limits.value);
			time = global::Defs.Localize("MultiplyerObjectivesAndRulesWindow.generations", vars, null, true, true);
			float num2 = (float)num3 / (float)rules.time_limits.value;
			color = UIMultiplayerUtils.GetTimeColor(num2);
			result = true;
		}
		return result;
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x0014B56C File Offset: 0x0014976C
	private static Color GetTimeColor(float percent)
	{
		if (percent >= 0.9f)
		{
			return global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "time_limit_soon");
		}
		if ((double)percent > 0.5)
		{
			return global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "time_limit_half");
		}
		return global::Defs.GetColor("MultiplyerObjectivesAndRulesWindow", "time_limit_normal");
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x0014B5C0 File Offset: 0x001497C0
	public static string SecoundsToShortString(int secoundsLeft)
	{
		int num = secoundsLeft % 60;
		secoundsLeft /= 60;
		int num2 = secoundsLeft % 60;
		secoundsLeft /= 60;
		int num3 = secoundsLeft;
		return string.Concat(new string[]
		{
			num3.ToString("D2"),
			":",
			num2.ToString("D2"),
			":",
			num.ToString("D2")
		});
	}
}
