using System;
using Logic;
using UnityEngine;

// Token: 0x020002D1 RID: 721
public static class RichPresence
{
	// Token: 0x06002DA0 RID: 11680 RVA: 0x0017AA10 File Offset: 0x00178C10
	public static bool Set(string key, Vars vars)
	{
		RichPresence.state_key = key;
		RichPresence.discord_state = "";
		RichPresence.discord_details = "";
		RichPresence.text = global::Defs.Localize("RichPresence." + key, vars, null, false, true);
		if (!string.IsNullOrEmpty(RichPresence.text))
		{
			Debug.Log("Rich presence: " + RichPresence.text + "\n---- Context:\n" + ((vars != null) ? vars.Dump() : null));
			RichPresence.discord_state = RichPresence.text;
			RichPresence.discord_details = "";
			int i = RichPresence.text.IndexOf('\n', 1);
			if (i > 0)
			{
				RichPresence.discord_state = RichPresence.text.Substring(0, i);
				for (i++; i < RichPresence.text.Length; i++)
				{
					char c = RichPresence.text[i];
					if (c != '\n' && c != '\r')
					{
						break;
					}
				}
				RichPresence.discord_details = RichPresence.text.Substring(i);
			}
		}
		if (!RichPresence.enabled)
		{
			return false;
		}
		if (!THQNORequest.signed_in)
		{
			return false;
		}
		THQNO_Wrapper.SetRichPresence("discord_state", RichPresence.discord_state);
		THQNO_Wrapper.SetRichPresence("discord_details", RichPresence.discord_details);
		vars.EnumerateAll(delegate(string k, Value v)
		{
			string text = DT.String(v, null);
			if (!string.IsNullOrEmpty(text) && text[0] == '#')
			{
				text = text.Substring(1);
			}
			THQNO_Wrapper.SetRichPresence(k, text ?? "");
		});
		THQNO_Wrapper.SetRichPresence("steam_display", "#" + key);
		return true;
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x0017AB68 File Offset: 0x00178D68
	private static string LocalizeKey(string text_key)
	{
		string text = global::Defs.Localize(text_key, null, null, false, true);
		if (string.IsNullOrEmpty(text))
		{
			return "---";
		}
		return text;
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x0017AB8F File Offset: 0x00178D8F
	private static string LocalizeKingdomName(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "---";
		}
		if (!key.StartsWith("tn_", StringComparison.InvariantCulture))
		{
			key = "tn_" + key;
		}
		return RichPresence.LocalizeKey(key);
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x0017ABC0 File Offset: 0x00178DC0
	private static string LocalizeStartingPeriod(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "---";
		}
		return RichPresence.LocalizeKey("CampaignVars.start_period.options." + key + ".name");
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x0017ABE5 File Offset: 0x00178DE5
	private static string LocalizeAIDifficulty(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "---";
		}
		return RichPresence.LocalizeKey("CampaignVars.ai_difficulty.options." + key + ".name");
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x0017AC0C File Offset: 0x00178E0C
	private static string LocalizeGameMode(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "---";
		}
		string text_key;
		if (key != "None")
		{
			text_key = "CampaignVars.main_goal.options." + key + ".name";
		}
		else
		{
			text_key = "CampaignVars.main_goal.options." + key + ".secondary_name";
		}
		return RichPresence.LocalizeKey(text_key);
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x0017AC5E File Offset: 0x00178E5E
	private static string LocalizeTeamSetup(int key)
	{
		return RichPresence.LocalizeKey(string.Format("CampaignVars.team_size.options.{0}.name", key));
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x0017AC78 File Offset: 0x00178E78
	public static bool Update(RichPresence.State state)
	{
		bool result;
		try
		{
			Game game = GameLogic.Get(true);
			Campaign campaign = (game != null) ? game.campaign : null;
			RichPresence.vars.Clear();
			string key;
			switch (state)
			{
			case RichPresence.State.InTitle:
				key = "InMenus";
				break;
			case RichPresence.State.InLobby:
				if (campaign == null)
				{
					return false;
				}
				if (campaign.IsMultiplayerCampaign())
				{
					if (campaign.state >= Campaign.State.Started)
					{
						key = "ContinuingMultiplayerGame";
						string str = RichPresence.LocalizeKingdomName(campaign.GetKingdomName(false));
						RichPresence.vars.Set<string>("kingdom", "#" + str);
						string str2 = RichPresence.LocalizeStartingPeriod(campaign.GetPeriod());
						RichPresence.vars.Set<string>("starting_period", "#" + str2);
						string str3 = RichPresence.LocalizeAIDifficulty(campaign.GetVar("ai_difficulty", null, true).String(null));
						RichPresence.vars.Set<string>("difficulty", "#" + str3);
						string str4 = RichPresence.LocalizeGameMode(campaign.GetVar("main_goal", null, true).String(null));
						RichPresence.vars.Set<string>("game_mode", "#" + str4);
						string str5 = RichPresence.LocalizeTeamSetup(campaign.GetVar("team_size", null, true).Int(0));
						RichPresence.vars.Set<string>("team_setup", "#" + str5);
					}
					else
					{
						key = "InMultiplayerLobby";
						RichPresence.vars.Set<int>("current_players", campaign.GetNumPlayers(false));
						RichPresence.vars.Set<int>("max_players", campaign.GetMaxPlayers());
					}
				}
				else
				{
					key = "InSingleplayerLobby";
				}
				break;
			case RichPresence.State.InGame:
			{
				if (campaign == null)
				{
					return false;
				}
				if (campaign.IsMultiplayerCampaign())
				{
					key = "InMultiplayerGame";
				}
				else
				{
					key = "InSingleplayerGame";
				}
				Logic.Kingdom kingdom = (game != null) ? game.GetLocalPlayerKingdom() : null;
				string str6 = RichPresence.LocalizeKingdomName((kingdom != null) ? kingdom.GetNameKey(null, "") : null);
				RichPresence.vars.Set<string>("kingdom", "#" + str6);
				string str7 = RichPresence.LocalizeStartingPeriod((game != null) ? game.map_period : null);
				RichPresence.vars.Set<string>("starting_period", "#" + str7);
				string key2 = null;
				if (((game != null) ? game.rules : null) != null)
				{
					switch (game.rules.ai_difficulty)
					{
					case 0:
						key2 = "easy";
						break;
					case 1:
						key2 = "normal";
						break;
					case 2:
						key2 = "hard";
						break;
					case 3:
						key2 = "very_hard";
						break;
					}
				}
				string str8 = RichPresence.LocalizeAIDifficulty(key2);
				RichPresence.vars.Set<string>("difficulty", "#" + str8);
				string key3;
				if (game == null)
				{
					key3 = null;
				}
				else
				{
					Game.CampaignRules rules = game.rules;
					key3 = ((rules != null) ? rules.main_goal : null);
				}
				string str9 = RichPresence.LocalizeGameMode(key3);
				RichPresence.vars.Set<string>("game_mode", "#" + str9);
				if (campaign.IsMultiplayerCampaign())
				{
					string str10 = RichPresence.LocalizeTeamSetup((((game != null) ? game.rules : null) == null) ? 0 : game.rules.team_size.Int(0));
					RichPresence.vars.Set<string>("team_setup", "#" + str10);
				}
				break;
			}
			default:
				return false;
			}
			if (campaign != null && campaign.IsMultiplayerCampaign())
			{
				RichPresence.vars.Set<string>("steam_player_group", campaign.id);
				RichPresence.vars.Set<int>("steam_player_group_size", campaign.GetNumPlayers(false));
			}
			else
			{
				RichPresence.vars.Set<Value>("steam_player_group", Value.Null);
				RichPresence.vars.Set<Value>("steam_player_group_size", Value.Null);
			}
			RichPresence.state = state;
			RichPresence.Set(key, RichPresence.vars);
			result = true;
		}
		catch (Exception arg)
		{
			Debug.LogError(string.Format("Error updating Rich Presesence: {0}", arg));
			result = false;
		}
		return result;
	}

	// Token: 0x04001ED3 RID: 7891
	public static bool enabled = true;

	// Token: 0x04001ED4 RID: 7892
	public static RichPresence.State state = RichPresence.State.InTitle;

	// Token: 0x04001ED5 RID: 7893
	public static string state_key = null;

	// Token: 0x04001ED6 RID: 7894
	public static Vars vars = new Vars();

	// Token: 0x04001ED7 RID: 7895
	public static string text = null;

	// Token: 0x04001ED8 RID: 7896
	public static string discord_state = null;

	// Token: 0x04001ED9 RID: 7897
	public static string discord_details = null;

	// Token: 0x04001EDA RID: 7898
	private const string NO_VALUE = "---";

	// Token: 0x02000849 RID: 2121
	public enum State
	{
		// Token: 0x04003EA4 RID: 16036
		InTitle,
		// Token: 0x04003EA5 RID: 16037
		InLobby,
		// Token: 0x04003EA6 RID: 16038
		InGame
	}
}
