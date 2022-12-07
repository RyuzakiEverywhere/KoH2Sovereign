using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000085 RID: 133
public static class SaveGame
{
	// Token: 0x060004F0 RID: 1264 RVA: 0x00038BCA File Offset: 0x00036DCA
	public static SaveGame.Info GetCurrent()
	{
		return SaveGame.current;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00038BD4 File Offset: 0x00036DD4
	public static SaveGame.Info FindByPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		SaveGame.ScanSavesDir(false);
		SaveGame.Info result;
		if (SaveGame.by_path.TryGetValue(path, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00038C04 File Offset: 0x00036E04
	public static SaveGame.Info FindById(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		SaveGame.ScanSavesDir(false);
		SaveGame.Info result;
		if (!SaveGame.by_id.TryGetValue(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00038C34 File Offset: 0x00036E34
	public static SaveGame.Info FindByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (SaveGame.list == null)
		{
			return null;
		}
		for (int i = 0; i < SaveGame.list.Count; i++)
		{
			SaveGame.Info info = SaveGame.list[i];
			if (info.name == name)
			{
				return info;
			}
		}
		return null;
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00038C88 File Offset: 0x00036E88
	public static SaveGame.CampaignInfo FindCampaign(string campaign_id)
	{
		if (string.IsNullOrEmpty(campaign_id))
		{
			return null;
		}
		SaveGame.ScanSavesDir(false);
		for (int i = 0; i < SaveGame.all_campaigns.Count; i++)
		{
			SaveGame.CampaignInfo campaignInfo = SaveGame.all_campaigns[i];
			Campaign campaign = campaignInfo.campaign;
			if (((campaign != null) ? campaign.id : null) == campaign_id)
			{
				return campaignInfo;
			}
		}
		return null;
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00038CE4 File Offset: 0x00036EE4
	public static Campaign GetCampaignData(string save_id)
	{
		SaveGame.Info info = SaveGame.FindById(save_id);
		if (info == null)
		{
			return null;
		}
		return info.GetCampaignData();
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00038D04 File Offset: 0x00036F04
	private static SaveGame.Info LoadSaveInfo(string path, bool is_multiplayer)
	{
		Path.GetFileName(path);
		FileInfo fileInfo = new FileInfo(path + "/savegame.txt");
		if (!fileInfo.Exists)
		{
			return null;
		}
		if (!File.Exists(path + "/world.txt"))
		{
			return null;
		}
		List<DT.Field> list = DT.Parser.ReadFile(null, path + "/savegame.txt", null);
		if (list == null || list.Count < 1)
		{
			return null;
		}
		return SaveGame.LoadSaveInfo(list[0], path, fileInfo.LastWriteTime, is_multiplayer);
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00038D7C File Offset: 0x00036F7C
	public static SaveGame.Info LoadSaveInfo(DT.Field f, string path, DateTime default_timestamp, bool is_multiplayer)
	{
		if (f.type != "save")
		{
			return null;
		}
		string text = f.String(null, "");
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string @string = f.GetString("id", null, "", true, true, true, '.');
		string key = f.key;
		string string2 = f.GetString("kingdom", null, "", true, true, true, '.');
		DateTime date_time = DateTime.Parse(f.GetString("save_date", null, default_timestamp.ToString(), true, true, true, '.'));
		int @int = f.GetInt("autoSaveNum", null, -1, true, true, true, '.');
		int int2 = f.GetInt("eventNum", null, -1, true, true, true, '.');
		int int3 = f.GetInt("session_time", null, 0, true, true, true, '.');
		DirectoryInfo parent = new DirectoryInfo(path).Parent;
		string campaign_id = (parent != null) ? parent.Name : null;
		string string3 = f.GetString("mod_id", null, "", true, true, true, '.');
		return new SaveGame.Info
		{
			id = @string,
			is_multiplayer = is_multiplayer,
			fullPath = path,
			campaign_id = campaign_id,
			name = text,
			date_time = date_time,
			map = key,
			kingdom = string2,
			autoSaveNum = @int,
			eventNum = int2,
			session_time = int3,
			mod_id = string3
		};
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00038ED4 File Offset: 0x000370D4
	private static int CompareCampaigns(SaveGame.CampaignInfo a, SaveGame.CampaignInfo b)
	{
		return b.last_save_date_time.CompareTo(a.last_save_date_time);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00038EF8 File Offset: 0x000370F8
	private static int CompareSaves(SaveGame.Info a, SaveGame.Info b)
	{
		if (a == null)
		{
			if (b == null)
			{
				return 0;
			}
			return 1;
		}
		else
		{
			if (b == null)
			{
				return -1;
			}
			if (a.is_multiplayer && a.campaign_id == b.campaign_id)
			{
				return b.session_time.CompareTo(a.session_time);
			}
			return b.date_time.CompareTo(a.date_time);
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00038F54 File Offset: 0x00037154
	private static void AddCampaign(string dir, bool is_multiplayer)
	{
		if (string.IsNullOrEmpty(dir))
		{
			return;
		}
		if (!Directory.Exists(dir))
		{
			return;
		}
		SaveGame.CampaignInfo campaignInfo = new SaveGame.CampaignInfo();
		campaignInfo.campaign = new Campaign(-2);
		campaignInfo.campaign.id = Path.GetFileName(dir);
		campaignInfo.campaign.SetMultiplayer(is_multiplayer);
		if (!campaignInfo.campaign.Load(dir))
		{
			return;
		}
		DirectoryInfo[] directories = new DirectoryInfo(dir).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			SaveGame.Info info = SaveGame.LoadSaveInfo(directories[i].FullName, is_multiplayer);
			if (info != null)
			{
				campaignInfo.saves.Add(info);
				if (SaveGame.CompareSaves(info, campaignInfo.latest_save) < 0)
				{
					campaignInfo.latest_save = info;
					if (!campaignInfo.campaign.IsMultiplayerCampaign())
					{
						SaveGame.Info a = info;
						SaveGame.CampaignInfo campaignInfo2 = SaveGame.latest_single_playr_campaign_info;
						if (SaveGame.CompareSaves(a, (campaignInfo2 != null) ? campaignInfo2.latest_save : null) < 0)
						{
							SaveGame.latest_single_playr_campaign_info = campaignInfo;
						}
					}
				}
				try
				{
					SaveGame.by_path.Add(info.fullPath, info);
					SaveGame.by_id.Add(info.id, info);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Error while adding save game for campaign ",
						info.ToString(),
						": ",
						ex
					}));
				}
				SaveGame.all_saves.Add(info);
				if (is_multiplayer)
				{
					SaveGame.multiplayer_saves.Add(info);
				}
				else
				{
					SaveGame.singleplayer_saves.Add(info);
				}
			}
		}
		campaignInfo.saves.Sort(new Comparison<SaveGame.Info>(SaveGame.CompareSaves));
		SaveGame.all_campaigns.Add(campaignInfo);
		if (is_multiplayer)
		{
			SaveGame.multiplayer_campaigns.Add(campaignInfo);
			return;
		}
		SaveGame.singleplayer_campaigns.Add(campaignInfo);
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00039100 File Offset: 0x00037300
	private static void AddCampaigns(string dir, bool is_multiplayer)
	{
		if (string.IsNullOrEmpty(dir))
		{
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(dir);
		if (!directoryInfo.Exists)
		{
			return;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			SaveGame.AddCampaign(directories[i].FullName, is_multiplayer);
		}
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0003914C File Offset: 0x0003734C
	public static void OnCampaignSaved(Campaign campaign)
	{
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign.id);
		if (campaignInfo == null)
		{
			SaveGame.saves_dir_scanned = false;
			return;
		}
		RemoteVars campaignData = campaign.campaignData;
		Vars vars = (campaignData != null) ? campaignData.vars : null;
		if (vars == null)
		{
			return;
		}
		campaignInfo.campaign.LoadFromVars(vars);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00039192 File Offset: 0x00037392
	private static void RefreshDependentUI()
	{
		InGameMenu.ForceRefresh();
		UILoadGameWindow.ForceRefresh();
		UISaveGameWindow.ForceRefresh();
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x000391A4 File Offset: 0x000373A4
	public static void ScanSavesDir(bool forced = false)
	{
		if (!forced && SaveGame.saves_dir_scanned)
		{
			return;
		}
		SaveGame.saves_dir_scanned = true;
		SaveGame.by_path.Clear();
		SaveGame.by_id.Clear();
		SaveGame.singleplayer_campaigns.Clear();
		SaveGame.multiplayer_campaigns.Clear();
		SaveGame.all_campaigns.Clear();
		SaveGame.singleplayer_saves.Clear();
		SaveGame.multiplayer_saves.Clear();
		SaveGame.all_saves.Clear();
		SaveGame.latest_single_playr_campaign_info = null;
		SaveGame.AddCampaigns(Game.GetSavesRootDir(Game.SavesRoot.Single), false);
		SaveGame.AddCampaigns(Game.GetSavesRootDir(Game.SavesRoot.Multi), true);
		SaveGame.singleplayer_campaigns.Sort(new Comparison<SaveGame.CampaignInfo>(SaveGame.CompareCampaigns));
		SaveGame.multiplayer_campaigns.Sort(new Comparison<SaveGame.CampaignInfo>(SaveGame.CompareCampaigns));
		SaveGame.all_campaigns.Sort(new Comparison<SaveGame.CampaignInfo>(SaveGame.CompareCampaigns));
		SaveGame.singleplayer_saves.Sort(new Comparison<SaveGame.Info>(SaveGame.CompareSaves));
		SaveGame.multiplayer_saves.Sort(new Comparison<SaveGame.Info>(SaveGame.CompareSaves));
		SaveGame.all_saves.Sort(new Comparison<SaveGame.Info>(SaveGame.CompareSaves));
		MPBoss mpboss = MPBoss.Get(false);
		if (mpboss != null)
		{
			mpboss.UpdateLatestSaveInfo();
		}
		SaveGame.RefreshDependentUI();
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000392CC File Offset: 0x000374CC
	public static void UpdateList(bool singleplayer_saves_only, bool list_all)
	{
		SaveGame.ScanSavesDir(false);
		if (!list_all)
		{
			Game game = GameLogic.Get(false);
			Campaign campaign = (game != null) ? game.campaign : null;
			if (campaign == null)
			{
				return;
			}
			SaveGame.tmp_campaign_list.Clear();
			SaveGame.campaigns = SaveGame.tmp_campaign_list;
			SaveGame.list = null;
			for (int i = 0; i < SaveGame.all_campaigns.Count; i++)
			{
				SaveGame.CampaignInfo campaignInfo = SaveGame.all_campaigns[i];
				if (!(campaignInfo.campaign.id != campaign.id))
				{
					SaveGame.tmp_campaign_list.Add(campaignInfo);
					SaveGame.list = campaignInfo.saves;
					break;
				}
			}
			if (SaveGame.list == null)
			{
				SaveGame.list = new List<SaveGame.Info>();
			}
			return;
		}
		else
		{
			if (singleplayer_saves_only)
			{
				SaveGame.campaigns = SaveGame.singleplayer_campaigns;
				SaveGame.list = SaveGame.singleplayer_saves;
				return;
			}
			SaveGame.campaigns = SaveGame.all_campaigns;
			SaveGame.list = SaveGame.all_saves;
			return;
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x000393A8 File Offset: 0x000375A8
	public static string GetSavePath(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		SaveGame.UpdateList(false, false);
		SaveGame.Info info = SaveGame.FindById(id);
		if (info == null)
		{
			return null;
		}
		return info.fullPath;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x000393D8 File Offset: 0x000375D8
	public static PoliticalData GetLatestPolitcalData(string campaign_id)
	{
		if (string.IsNullOrEmpty(campaign_id))
		{
			return null;
		}
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign_id);
		PoliticalData politicalData = (campaignInfo != null) ? campaignInfo.GetPoliticalData() : null;
		PoliticalData politicalData2 = null;
		Game game = GameLogic.Get(false);
		Campaign campaign = (game != null) ? game.campaign : null;
		if (((campaign != null) ? campaign.id : null) == campaign_id)
		{
			string fromSaveID = campaign.GetFromSaveID(true);
			if (!string.IsNullOrEmpty(fromSaveID))
			{
				SaveGame.Info info = SaveGame.FindById(fromSaveID);
				politicalData2 = ((info != null) ? info.GetPoliticalData() : null);
			}
		}
		if (politicalData2 == null)
		{
			PoliticalData politicalData3;
			if (campaignInfo == null)
			{
				politicalData3 = null;
			}
			else
			{
				SaveGame.Info latest_save = campaignInfo.latest_save;
				politicalData3 = ((latest_save != null) ? latest_save.GetPoliticalData() : null);
			}
			politicalData2 = politicalData3;
		}
		if (politicalData2 == null)
		{
			return politicalData;
		}
		if (politicalData == null)
		{
			return politicalData2;
		}
		if (politicalData2.session_time >= politicalData.session_time)
		{
			return politicalData2;
		}
		return politicalData;
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0003948C File Offset: 0x0003768C
	public static void DeleteCachedPoliticalData(Campaign campaign, string from_save_id)
	{
		File.Delete(campaign.Dir() + "/political_data.def");
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign((campaign != null) ? campaign.id : null);
		if (campaignInfo == null)
		{
			return;
		}
		campaignInfo.political_data = null;
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x000394CC File Offset: 0x000376CC
	public static bool UpdatePoliticalData(string campaign_id, PoliticalData political_data)
	{
		SaveGame.CampaignInfo campaignInfo = SaveGame.FindCampaign(campaign_id);
		if (campaignInfo == null)
		{
			Logic.Multiplayer.Log(string.Format("Received political data for unknown campaign {0}: {1}", campaign_id, political_data), 2, Game.LogType.Error);
			return false;
		}
		PoliticalData politicalData = campaignInfo.GetPoliticalData();
		if (politicalData != null && politicalData.session_time >= political_data.session_time)
		{
			return false;
		}
		campaignInfo.political_data = political_data;
		try
		{
			string contents = global::Defs.Save((political_data != null) ? political_data.Save() : null, "", null);
			File.WriteAllText(campaignInfo.campaign.Dir() + "/political_data.def", contents);
		}
		catch (Exception ex)
		{
			string str = "Error saving ";
			Campaign campaign = campaignInfo.campaign;
			Debug.LogError(str + ((campaign != null) ? campaign.Dir() : null) + "/political_data.def: " + ex.ToString());
		}
		return true;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00039590 File Offset: 0x00037790
	public static string UniqueFolder(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogWarning("Empty path!");
			return null;
		}
		DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories();
		int num = 1;
		for (;;)
		{
			bool flag = false;
			DirectoryInfo[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Name == num.ToString())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
			num++;
		}
		return Path.Combine(path, num.ToString());
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00039608 File Offset: 0x00037808
	public static bool CanSave(bool ignore_bv = false)
	{
		string text;
		return SaveGame.CanSave(out text, ignore_bv);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00039620 File Offset: 0x00037820
	public static bool CanSave(out string reason_key, bool ignore_bv = false)
	{
		reason_key = "";
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			reason_key = "missing_game";
			return false;
		}
		if (!ignore_bv && game.subgames.Count > 0)
		{
			reason_key = "in_battle";
			return false;
		}
		return true;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00039664 File Offset: 0x00037864
	public static bool CanLoad(bool refresh_saves = true)
	{
		string text;
		return SaveGame.CanLoad(out text, refresh_saves);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0003967C File Offset: 0x0003787C
	public static bool CanLoad(out string reason_key, bool refresh_saves = true)
	{
		reason_key = "";
		Game game = GameLogic.Get(true);
		if (game == null)
		{
			reason_key = "missing_game";
			return false;
		}
		if (game.subgames.Count > 0)
		{
			reason_key = "in_battle";
			return false;
		}
		if (SaveGame.save_thread != null)
		{
			reason_key = "save_in_progress";
			return false;
		}
		if (!refresh_saves)
		{
			return true;
		}
		bool flag = game.IsMultiplayer();
		if (flag)
		{
			reason_key = "multiplayer_game";
			return false;
		}
		SaveGame.UpdateList(!flag, !flag);
		if (SaveGame.campaigns.Count <= 0)
		{
			reason_key = "no_saved_games";
			return false;
		}
		return true;
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00039708 File Offset: 0x00037908
	public static SaveGame.Info GetValidLoadInfo(string fullPath)
	{
		if (string.IsNullOrEmpty(fullPath))
		{
			return null;
		}
		if (!SaveGame.CanLoad(false))
		{
			return null;
		}
		SaveGame.Info info = SaveGame.FindByPath(fullPath);
		if (info == null)
		{
			return null;
		}
		return info;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00039738 File Offset: 0x00037938
	private static void Save(string path, string name, List<Serialization.ObjectStates> all_states, DT.Field vis_dt, PoliticalData political_data)
	{
		Game game = GameLogic.Get(true);
		Stopwatch stopwatch = Stopwatch.StartNew();
		DTSerializeWriter dtserializeWriter = new DTSerializeWriter(new UniqueStrings());
		dtserializeWriter.SetRootKey(game.map_name, game.map_period);
		dtserializeWriter.Write7BitUInt(9, "save_version", int.MaxValue);
		for (int i = 0; i < all_states.Count; i++)
		{
			Serialization.ObjectStates objectStates = all_states[i];
			NID obj_nid = objectStates.obj_nid;
			Logic.Object obj = obj_nid.GetObj(game);
			string key = NID.ToString(obj_nid.nid);
			using (dtserializeWriter.OpenSection(obj_nid.ti.name, key, int.MaxValue, false))
			{
				dtserializeWriter.GetLastSection().comment1 = "//" + objectStates.comment;
				try
				{
					foreach (KeyValuePair<byte, Serialization.ObjectState> keyValuePair in objectStates.states)
					{
						Serialization.ObjectState value = keyValuePair.Value;
						string state_name = value.state_name;
						using (dtserializeWriter.OpenSection("", state_name, int.MaxValue, true))
						{
							value.Write(dtserializeWriter, false);
							if (value.substates != null)
							{
								for (int j = 0; j < value.substates.Count; j++)
								{
									Serialization.ObjectSubstate objectSubstate = value.substates[j];
									using (dtserializeWriter.OpenSection(objectSubstate.substate_name, objectSubstate.substate_name, objectSubstate.substate_index, true))
									{
										objectSubstate.Write(dtserializeWriter, false);
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Error saving ",
						obj.ToString(),
						": ",
						ex
					}));
				}
			}
		}
		stopwatch.Stop();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Directory.CreateDirectory(path);
		DT.Field root = dtserializeWriter.GetRoot();
		if (root != null)
		{
			stopwatch.Restart();
			string contents = global::Defs.Save(root, "", null);
			stopwatch.Stop();
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			File.WriteAllText(path + "/world.txt", contents);
			stopwatch.Stop();
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
		}
		try
		{
			string contents2 = global::Defs.Save((political_data != null) ? political_data.Save() : null, "", null);
			File.WriteAllText(path + "/political_data.def", contents2);
		}
		catch (Exception ex2)
		{
			Debug.LogError("Error saving " + path + "/political_data.def: " + ex2.ToString());
		}
		UniqueStrings unique_strings = dtserializeWriter.unique_strings;
		if (unique_strings != null)
		{
			stopwatch.Restart();
			unique_strings.SaveToCSVFile(path + "/unique_strings.csv");
			stopwatch.Stop();
			long elapsedMilliseconds4 = stopwatch.ElapsedMilliseconds;
		}
		if (vis_dt != null)
		{
			stopwatch.Restart();
			string contents3 = global::Defs.Save(vis_dt, "", null);
			File.WriteAllText(path + "/savegame.txt", contents3);
			SaveGame.current = SaveGame.LoadSaveInfo(vis_dt, path, DateTime.Now, game.campaign.IsMultiplayerCampaign());
			stopwatch.Stop();
			long elapsedMilliseconds5 = stopwatch.ElapsedMilliseconds;
		}
		WorldRadio.Save(path + "/radio.txt");
		if (!game.campaign.IsMultiplayerCampaign())
		{
			game.campaign.Save(null);
		}
		game.campaign.Save(path);
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00039B30 File Offset: 0x00037D30
	private static void SaveThread(object thread_params)
	{
		try
		{
			SaveGame.ThreadParams threadParams = thread_params as SaveGame.ThreadParams;
			SaveGame.Save(threadParams.path, threadParams.name, threadParams.all_states, threadParams.vis_dt, threadParams.political_data);
			if (threadParams.onComplete != null)
			{
				MainThreadUpdates.Schedule(threadParams.onComplete);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		SaveGame.save_thread = null;
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00039BA0 File Offset: 0x00037DA0
	private static void OnSaveComplete()
	{
		SaveGame.ScanSavesDir(true);
		UISystemEventsIcons.Hide("Saving");
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00039BB4 File Offset: 0x00037DB4
	public static void Save(string fullPath, string name, int autoSaveNum = -1, int eventNum = -1, string save_id = null)
	{
		string text;
		if (!SaveGame.CanSave(out text, true))
		{
			return;
		}
		Game game = GameLogic.Get(true);
		Logic.Multiplayer multiplayer = (game != null) ? game.multiplayer : null;
		if (multiplayer == null)
		{
			return;
		}
		UISystemEventsIcons.Show("Saving", true, "icon", null);
		Profile.BeginSection("Generate all states");
		Stopwatch stopwatch = Stopwatch.StartNew();
		List<Serialization.ObjectStates> all_states = multiplayer.GenerateAllStates();
		DT.Field field = new DT.Field(null);
		field.flags |= DT.Field.Flags.StartsAtSameLine;
		field.type = "save";
		field.key = game.map_name;
		field.value = name;
		string text2 = save_id ?? Campaign.NewGUID();
		field.SetValue("id", DT.Enquote(text2), text2);
		if (!string.IsNullOrEmpty(game.map_period))
		{
			field.SetValue("period", DT.Enquote(game.map_period), game.map_period);
		}
		field.SetValue("autoSaveNum", autoSaveNum.ToString(), autoSaveNum);
		field.SetValue("eventNum", eventNum.ToString(), eventNum);
		field.SetValue("session_time", (int)game.session_time.seconds);
		string text3 = DateTime.Now.ToString("O");
		field.SetValue("save_date", DT.Enquote(text3), text3);
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			Mod activeMod = modManager.GetActiveMod();
			field.SetValue("mod_id", (activeMod != null) ? DT.Enquote(activeMod.mod_id) : "", null);
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.Save(field);
		}
		PoliticalData politicalData = new PoliticalData();
		politicalData.InitFrom(game);
		stopwatch.Stop();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Profile.EndSection("Generate all states");
		SaveGame.WaitSaveComplete();
		if (autoSaveNum < 0 && save_id == null)
		{
			Analytics.OnSaveGame(game, name);
		}
		string text4 = (save_id != null) ? "Sync Save" : "Local Save";
		Debug.Log(string.Format("[{0}] {1}: {2} ({3})", new object[]
		{
			game.session_time,
			text4,
			Path.GetFileName(fullPath),
			text2
		}));
		if (game.campaign != null && game.campaign.IsAuthority())
		{
			game.FireEvent("sync_save", text2, Array.Empty<int>());
		}
		PersistentLogs.Save(fullPath);
		SaveGame.save_thread = new Thread(new ParameterizedThreadStart(SaveGame.SaveThread));
		SaveGame.save_thread.Name = "Save thread";
		SaveGame.ThreadParams parameter = new SaveGame.ThreadParams
		{
			path = fullPath,
			name = name,
			all_states = all_states,
			vis_dt = field,
			political_data = politicalData,
			onComplete = new Action(SaveGame.OnSaveComplete)
		};
		SaveGame.save_thread.Start(parameter);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00039E81 File Offset: 0x00038081
	public static bool IsSaving()
	{
		return SaveGame.save_thread != null || PersistentLogs.save_thread != null;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00039E96 File Offset: 0x00038096
	public static void WaitSaveComplete()
	{
		if (SaveGame.save_thread != null)
		{
			Thread thread = SaveGame.save_thread;
			if (thread != null)
			{
				thread.Join();
			}
		}
		if (PersistentLogs.save_thread != null)
		{
			Thread thread2 = PersistentLogs.save_thread;
			if (thread2 == null)
			{
				return;
			}
			thread2.Join();
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00039EC8 File Offset: 0x000380C8
	private static void LoadObjectsFromDT(DT.Field rootSection)
	{
		Game game = GameLogic.Get(true);
		DT.Field field = null;
		DTSerializeReader dtserializeReader = new DTSerializeReader(game, null);
		for (int i = 0; i < rootSection.children.Count; i++)
		{
			field = rootSection.children[i];
			if (!string.IsNullOrEmpty(field.type))
			{
				Serialization.ObjectType tid;
				try
				{
					tid = (Serialization.ObjectType)Enum.Parse(typeof(Serialization.ObjectType), field.type, true);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Invalid type while loading object " + field.type + ". " + ex.ToString());
					goto IL_3B2;
				}
				Serialization.ObjectTypeInfo objectTypeInfo = Serialization.ObjectTypeInfo.Get(tid);
				if (objectTypeInfo == null)
				{
					Debug.LogWarning("Invalid type info for type " + tid.ToString());
				}
				else
				{
					int num = NID.Decode(field.key);
					if (num <= 0)
					{
						Debug.LogWarning("Cannot parse object nid for obj " + field.ToString());
					}
					else
					{
						Logic.Object @object = game.multiplayer.objects.Get(objectTypeInfo.tid, num);
						if (@object == null || @object.destroyed)
						{
							@object = objectTypeInfo.CreateObject(game.multiplayer);
							if (@object == null)
							{
								Debug.LogError("Failed to create object " + field.ToString());
								goto IL_3B2;
							}
							@object.SetNid(num, true);
						}
						if (@object.state_acc == null)
						{
							@object.state_acc = new Serialization.ObjectStates(@object);
						}
						else
						{
							Debug.LogError("Loading an object that already has a state accumulator " + field.ToString());
						}
						if (field.children != null)
						{
							for (int j = 0; j < field.children.Count; j++)
							{
								DT.Field field2 = field.children[j];
								byte b;
								if (!objectTypeInfo.str_state_ids.TryGetValue(field2.key, out b))
								{
									Debug.LogWarning("Cannot parse state id for state " + field2.ToString() + " of obj " + field.ToString());
								}
								else
								{
									dtserializeReader.SetDTField(field2);
									Serialization.ObjectState objectState = Serialization.ObjectMessage.Read("state", dtserializeReader, b, objectTypeInfo, num, 0, 0) as Serialization.ObjectState;
									if (@object.state_acc != null)
									{
										@object.state_acc.Add(objectState.id, objectState);
										if (field2.children != null)
										{
											for (int k = 0; k < field2.children.Count; k++)
											{
												DT.Field field3 = field2.children[k];
												if (field3.children != null && field3.children.Count != 0 && !(field3.type == "data"))
												{
													string text = field3.key;
													if (text.Length > field3.type.Length && text.StartsWith(field3.type))
													{
														text = text.Substring(field3.type.Length);
													}
													int substate_index;
													if (int.TryParse(text, out substate_index))
													{
														Serialization.State state;
														byte substate_id;
														if (!objectTypeInfo.states_by_id.TryGetValue(b, out state))
														{
															Debug.LogWarning("Cannot get state type for state " + field2.ToString() + " of object " + field.ToString());
														}
														else if (!state.str_substate_ids.TryGetValue(field3.type, out substate_id))
														{
															Debug.LogWarning(string.Concat(new string[]
															{
																"Cannot get substate id for substate ",
																field3.ToString(),
																" of state ",
																field2.ToString(),
																" of object ",
																field.ToString()
															}));
														}
														else
														{
															dtserializeReader.SetDTField(field3);
															Serialization.ObjectSubstate substate = Serialization.ObjectMessage.Read("substate", dtserializeReader, b, objectTypeInfo, num, substate_id, substate_index) as Serialization.ObjectSubstate;
															objectState.AddSubstate(substate);
														}
													}
												}
											}
										}
									}
									else
									{
										Debug.LogError("Loading a state for non-accumulating object " + field.ToString());
									}
								}
							}
						}
					}
				}
			}
			IL_3B2:;
		}
		dtserializeReader.SetDTField(null);
		Serialization.PostProcessAllObjectsAfterLoad(game);
		for (Serialization.ObjectType objectType = Serialization.ObjectType.Game; objectType < Serialization.ObjectType.COUNT; objectType++)
		{
			foreach (KeyValuePair<int, Logic.Object> keyValuePair in game.multiplayer.objects.Registry(objectType).objects)
			{
				Logic.Object value = keyValuePair.Value;
				if (value.state_acc == null)
				{
					Debug.LogError("Loading an object that doesn't have a state accumulator " + value.ToString());
				}
				else
				{
					value.state_acc.LoadObject(value);
					value.state_acc = null;
				}
			}
		}
		Serialization.PostProcessAllObjectsAfterApply(game);
		Serialization.OnLoadComplete();
		Game.isLoadingSaveGame = false;
		for (Serialization.ObjectType objectType2 = Serialization.ObjectType.Game; objectType2 < Serialization.ObjectType.COUNT; objectType2++)
		{
			foreach (KeyValuePair<int, Logic.Object> keyValuePair2 in game.multiplayer.objects.Registry(objectType2).objects)
			{
				Logic.Object value2 = keyValuePair2.Value;
				if (!value2.started)
				{
					value2.Start();
				}
			}
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0003A3F4 File Offset: 0x000385F4
	public static void Delete(string fullPath)
	{
		if (Directory.Exists(fullPath))
		{
			Directory.Delete(fullPath, true);
			SaveGame.ScanSavesDir(true);
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0003A40C File Offset: 0x0003860C
	public static bool QuickLoad()
	{
		if (!SaveGame.CanLoad(true))
		{
			return false;
		}
		Game game = GameLogic.Get(false);
		Campaign campaign = (game != null) ? game.campaign : null;
		if (campaign == null)
		{
			return false;
		}
		if (campaign.IsMultiplayerCampaign())
		{
			return false;
		}
		string fullPath = (campaign != null) ? campaign.Dir("Quicksave") : null;
		if (SaveGame.GetValidLoadInfo(fullPath) == null)
		{
			return false;
		}
		game.load_game = Game.LoadedGameType.QuickLoad;
		game.StartGame(false, null, fullPath);
		return true;
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0003A474 File Offset: 0x00038674
	public static void Load(string fullPath)
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		SaveGame.Info validLoadInfo = SaveGame.GetValidLoadInfo(fullPath);
		if (validLoadInfo == null)
		{
			return;
		}
		List<DT.Field> list = DT.Parser.ReadFile(null, fullPath + "/savegame.txt", null);
		if (list == null || list.Count != 1)
		{
			Debug.LogError("Error loading savegame.txt");
			SaveGame.HideLoadingScreen();
			return;
		}
		DT.Field field = list[0];
		Game.isLoadingSaveGame = true;
		bool isMidGame = game.state == Game.State.Running;
		Game.BeginProfileSection("UnloadMap");
		Stopwatch stopwatch = Stopwatch.StartNew();
		game.UnloadMap();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Game.EndProfileSection("UnloadMap");
		string key = field.key;
		string @string = field.GetString("period", null, "", true, true, true, '.');
		string a = SceneManager.GetActiveScene().name.ToLowerInvariant();
		PersistentLogs.StoreLoadLogs(fullPath);
		if (game.state == Game.State.Running && game.campaign != null && game.campaign.IsMultiplayerCampaign() && game.multiplayer != null && game.multiplayer.type == Logic.Multiplayer.Type.Server)
		{
			game.multiplayer.SendLoadGame(key, @string);
		}
		if (a != key)
		{
			SaveGame.cur_loading_info = validLoadInfo;
			SaveGame.cur_loading_vis_root = field;
			SaveGame.cur_full_path = fullPath;
			game.map_period = @string;
			game.map_from_save_id = null;
			game.political_data_session_time = 0;
			if (GameLogic.LoadScene(key))
			{
				return;
			}
			Debug.LogError("Unknown scene for map " + key);
			SaveGame.cur_loading_info = null;
			SaveGame.cur_loading_vis_root = null;
			SaveGame.cur_full_path = null;
		}
		Game.BeginProfileSection("LoadMap");
		stopwatch.Restart();
		game.LoadMap(key, @string, false);
		if (a == key)
		{
			global::PathFinding pathFinding = UnityEngine.Object.FindObjectOfType<global::PathFinding>();
			if (pathFinding != null)
			{
				pathFinding.logic = game.path_finding;
				game.path_finding.visuals = pathFinding;
				MapData.Get().InitPAManager(game.path_finding);
			}
		}
		long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
		Game.EndProfileSection("LoadMap");
		SaveGame.FinishLoading(fullPath, field, isMidGame, validLoadInfo.is_multiplayer);
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0003A66C File Offset: 0x0003886C
	private static void OnLoadingFinished()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Game.isLoadingSaveGame = false;
		if (game.IsAuthority())
		{
			game.SetSpeed(1f, -1);
			game.OnFullGameStateReceived(true);
		}
		GameSpeed.SupressSpeedChangesByPlayer = false;
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0003A6AC File Offset: 0x000388AC
	public static void FinishLoading()
	{
		SaveGame.Info info = SaveGame.cur_loading_info;
		DT.Field field = SaveGame.cur_loading_vis_root;
		string text = SaveGame.cur_full_path;
		SaveGame.cur_loading_info = null;
		SaveGame.cur_loading_vis_root = null;
		SaveGame.cur_full_path = null;
		if (info == null || field == null || string.IsNullOrEmpty(text))
		{
			SaveGame.OnLoadingFinished();
			return;
		}
		SaveGame.FinishLoading(text, field, false, info.is_multiplayer);
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0003A700 File Offset: 0x00038900
	public static void FinishLoading(string fullPath, DT.Field vis_root, bool isMidGame, bool is_multiplayer)
	{
		Game game = GameLogic.Get(true);
		Profile.BeginSection("LoadSaveFile");
		Stopwatch stopwatch = Stopwatch.StartNew();
		SaveGame.current = SaveGame.LoadSaveInfo(vis_root, fullPath, DateTime.Now, is_multiplayer);
		List<DT.Field> list = DT.Parser.ReadFile(null, fullPath + "/world.txt", null);
		stopwatch.Stop();
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		Profile.EndSection("LoadSaveFile");
		if (list == null)
		{
			Debug.LogError("Load Game: Problem while loading game. Load file is missing.");
			SaveGame.OnLoadingFinished();
			return;
		}
		if (list.Count > 1)
		{
			Debug.LogError("Load Game: Problem while parsing DT file or file contains multiple root sections");
			SaveGame.OnLoadingFinished();
			return;
		}
		DT.Field field = list[0];
		if (string.IsNullOrEmpty(field.key))
		{
			Debug.LogError("Map is missing from the save file");
			SaveGame.OnLoadingFinished();
			return;
		}
		Game.BeginProfileSection("Serialization.PreprocessAllFields");
		Serialization.PreprocessAllFields(game, field);
		Game.EndProfileSection("Serialization.PreprocessAllFields");
		Game.BeginProfileSection("LoadObjectsFromDT");
		stopwatch.Restart();
		SaveGame.LoadObjectsFromDT(field);
		long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
		Game.EndProfileSection("LoadObjectsFromDT");
		Game.BeginProfileSection("Set Kingdom");
		SaveGame.SetLocalLogicKingdom(vis_root, isMidGame, game);
		Game.EndProfileSection("Set Kingdom");
		Game.BeginProfileSection("Load visuals");
		stopwatch.Restart();
		if (vis_root != null)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI != null)
			{
				worldUI.Load(vis_root);
			}
		}
		long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
		Game.EndProfileSection("Load visuals");
		Game.BeginProfileSection("Load radio");
		WorldRadio.Load(fullPath + "/radio.txt");
		Game.EndProfileSection("Load radio");
		game.OnGameStarted("load_game");
		game.SendState<Game.GameLoadedState>();
		Debug.Log("Game loaded");
		SaveGame.OnLoadingFinished();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0003A888 File Offset: 0x00038A88
	private static void SetLocalLogicKingdom(DT.Field vis_root, bool isMidGame, Game game)
	{
		Logic.Kingdom kingdom = NID.FromDTValue(vis_root.GetValue("kingdom", null, true, true, true, '.')).GetObj(game) as Logic.Kingdom;
		if (!isMidGame)
		{
			int num = game.IsMultiplayer() ? game.campaign.GetPlayerIndex(game.multiplayer.playerData.id, true) : 0;
			if (num >= 0 && num < 6)
			{
				string kingdomName = game.campaign.GetKingdomName(num, false);
				if (!string.IsNullOrEmpty(kingdomName))
				{
					kingdom = game.GetKingdom(kingdomName);
				}
			}
		}
		if (kingdom != null)
		{
			game.SetKingdom(kingdom, false);
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0003A916 File Offset: 0x00038B16
	private static void HideLoadingScreen()
	{
		LoadingScreen loadingScreen = LoadingScreen.Get();
		if (loadingScreen == null)
		{
			return;
		}
		loadingScreen.Show(false, true, false);
	}

	// Token: 0x040004BE RID: 1214
	private static Dictionary<string, SaveGame.Info> by_path = new Dictionary<string, SaveGame.Info>();

	// Token: 0x040004BF RID: 1215
	private static Dictionary<string, SaveGame.Info> by_id = new Dictionary<string, SaveGame.Info>();

	// Token: 0x040004C0 RID: 1216
	private static List<SaveGame.CampaignInfo> singleplayer_campaigns = new List<SaveGame.CampaignInfo>();

	// Token: 0x040004C1 RID: 1217
	private static List<SaveGame.CampaignInfo> multiplayer_campaigns = new List<SaveGame.CampaignInfo>();

	// Token: 0x040004C2 RID: 1218
	private static List<SaveGame.CampaignInfo> all_campaigns = new List<SaveGame.CampaignInfo>();

	// Token: 0x040004C3 RID: 1219
	private static List<SaveGame.Info> singleplayer_saves = new List<SaveGame.Info>();

	// Token: 0x040004C4 RID: 1220
	private static List<SaveGame.Info> multiplayer_saves = new List<SaveGame.Info>();

	// Token: 0x040004C5 RID: 1221
	private static List<SaveGame.Info> all_saves = new List<SaveGame.Info>();

	// Token: 0x040004C6 RID: 1222
	private static bool saves_dir_scanned = false;

	// Token: 0x040004C7 RID: 1223
	public static List<SaveGame.CampaignInfo> campaigns = new List<SaveGame.CampaignInfo>();

	// Token: 0x040004C8 RID: 1224
	public static SaveGame.CampaignInfo latest_single_playr_campaign_info = null;

	// Token: 0x040004C9 RID: 1225
	public static List<SaveGame.Info> list = new List<SaveGame.Info>();

	// Token: 0x040004CA RID: 1226
	public static SaveGame.Info current = null;

	// Token: 0x040004CB RID: 1227
	private static List<SaveGame.CampaignInfo> tmp_campaign_list = new List<SaveGame.CampaignInfo>(1);

	// Token: 0x040004CC RID: 1228
	public static Thread save_thread = null;

	// Token: 0x040004CD RID: 1229
	public static object Lock = new object();

	// Token: 0x040004CE RID: 1230
	private static SaveGame.Info cur_loading_info = null;

	// Token: 0x040004CF RID: 1231
	private static DT.Field cur_loading_vis_root = null;

	// Token: 0x040004D0 RID: 1232
	private static string cur_full_path = string.Empty;

	// Token: 0x02000554 RID: 1364
	public class Info
	{
		// Token: 0x0600439E RID: 17310 RVA: 0x001FE004 File Offset: 0x001FC204
		public Campaign GetCampaignData()
		{
			if (this.campaign_data == null)
			{
				this.campaign_data = new Campaign(-1);
				this.campaign_data.Load(this.fullPath);
			}
			return this.campaign_data;
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x001FE032 File Offset: 0x001FC232
		public PoliticalData GetPoliticalData()
		{
			if (this.political_data == null)
			{
				this.political_data = new PoliticalData();
				this.political_data.Load(this.fullPath + "/political_data.def");
			}
			return this.political_data;
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x001FE068 File Offset: 0x001FC268
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"#",
				this.fullPath,
				": '",
				this.name,
				"' from ",
				this.date_time.ToString(),
				", ",
				this.map,
				", ",
				this.kingdom
			});
		}

		// Token: 0x04002FF2 RID: 12274
		public string id;

		// Token: 0x04002FF3 RID: 12275
		public bool is_multiplayer;

		// Token: 0x04002FF4 RID: 12276
		public string fullPath;

		// Token: 0x04002FF5 RID: 12277
		public string campaign_id;

		// Token: 0x04002FF6 RID: 12278
		public string name;

		// Token: 0x04002FF7 RID: 12279
		public int session_time;

		// Token: 0x04002FF8 RID: 12280
		public DateTime date_time;

		// Token: 0x04002FF9 RID: 12281
		public int autoSaveNum = -1;

		// Token: 0x04002FFA RID: 12282
		public int eventNum = -1;

		// Token: 0x04002FFB RID: 12283
		public string map;

		// Token: 0x04002FFC RID: 12284
		public string kingdom;

		// Token: 0x04002FFD RID: 12285
		public string mod_id;

		// Token: 0x04002FFE RID: 12286
		private Campaign campaign_data;

		// Token: 0x04002FFF RID: 12287
		private PoliticalData political_data;
	}

	// Token: 0x02000555 RID: 1365
	public class CampaignInfo
	{
		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x060043A2 RID: 17314 RVA: 0x001FE0F2 File Offset: 0x001FC2F2
		public DateTime last_save_date_time
		{
			get
			{
				if (this.latest_save != null)
				{
					return this.latest_save.date_time;
				}
				if (this.campaign != null)
				{
					return this.campaign.GetCreationTime();
				}
				return DateTime.MinValue;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x060043A3 RID: 17315 RVA: 0x001FE121 File Offset: 0x001FC321
		public int last_save_session_time
		{
			get
			{
				if (this.latest_save != null)
				{
					return this.latest_save.session_time;
				}
				return -1;
			}
		}

		// Token: 0x060043A4 RID: 17316 RVA: 0x001FE138 File Offset: 0x001FC338
		public PoliticalData GetPoliticalData()
		{
			if (this.political_data == null)
			{
				this.political_data = new PoliticalData();
				this.political_data.Load(this.campaign.Dir() + "/political_data.def");
			}
			return this.political_data;
		}

		// Token: 0x04003000 RID: 12288
		public Campaign campaign;

		// Token: 0x04003001 RID: 12289
		public List<SaveGame.Info> saves = new List<SaveGame.Info>();

		// Token: 0x04003002 RID: 12290
		public SaveGame.Info latest_save;

		// Token: 0x04003003 RID: 12291
		public PoliticalData political_data;
	}

	// Token: 0x02000556 RID: 1366
	private class ThreadParams
	{
		// Token: 0x04003004 RID: 12292
		public string path;

		// Token: 0x04003005 RID: 12293
		public string name;

		// Token: 0x04003006 RID: 12294
		public List<Serialization.ObjectStates> all_states;

		// Token: 0x04003007 RID: 12295
		public DT.Field vis_dt;

		// Token: 0x04003008 RID: 12296
		public PoliticalData political_data;

		// Token: 0x04003009 RID: 12297
		public Action onComplete;
	}
}
