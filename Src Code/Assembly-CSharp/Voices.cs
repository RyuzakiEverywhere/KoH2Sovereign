using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x0200017F RID: 383
public static class Voices
{
	// Token: 0x1700010D RID: 269
	// (get) Token: 0x060014BF RID: 5311 RVA: 0x000D03AF File Offset: 0x000CE5AF
	// (set) Token: 0x060014C0 RID: 5312 RVA: 0x000D03EC File Offset: 0x000CE5EC
	public static string Language
	{
		get
		{
			if (Voices.language == null)
			{
				if (global::Defs.IsPlaying())
				{
					Voices.language = UserSettings.AudioLanguage;
				}
				else
				{
					Voices.language = global::Defs.def_language;
				}
			}
			if (string.IsNullOrEmpty(Voices.language))
			{
				return global::Defs.Language;
			}
			return Voices.language;
		}
		set
		{
			Voices.language = value;
		}
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x000D03F4 File Offset: 0x000CE5F4
	public static void LoadDefs()
	{
		Voices.Clear();
		if (global::Defs.Get(false) == null)
		{
			return;
		}
		Voices.voices_def_field = global::Defs.GetDefField("Voices", null);
		if (Voices.voices_def_field == null)
		{
			AudioLog.Error("'Voices' def not found");
			return;
		}
		Voices.roles_field = Voices.voices_def_field.FindChild("roles", null, true, true, true, '.');
		Voices.conditions_field = Voices.voices_def_field.FindChild("conditions", null, true, true, true, '.');
		Voices.events_field = Voices.voices_def_field.FindChild("events", null, true, true, true, '.');
		Voices.types_field = Voices.voices_def_field.FindChild("types", null, true, true, true, '.');
		for (Voices.VoiceLine.Type type = Voices.VoiceLine.Type.Narrator; type < Voices.VoiceLine.Type.COUNT; type++)
		{
			string path = type.ToString();
			DT.Field field = Voices.roles_field;
			DT.Field field2 = (field != null) ? field.FindChild(path, null, true, true, true, '.') : null;
			Voices.role_fields[(int)type] = field2;
			DT.Field field3 = Voices.conditions_field;
			DT.Field field4 = (field3 != null) ? field3.FindChild(path, null, true, true, true, '.') : null;
			Voices.condition_fields[(int)type] = field4;
			DT.Field field5 = Voices.events_field;
			DT.Field field6 = (field5 != null) ? field5.FindChild(path, null, true, true, true, '.') : null;
			Voices.event_fields[(int)type] = field6;
			DT.Field field7 = Voices.types_field;
			DT.Field field8 = (field7 != null) ? field7.FindChild(path, null, true, true, true, '.') : null;
			Voices.type_fields[(int)type] = field8;
		}
		List<DT.SubValue> list = Voices.voices_def_field.GetValue("group_cooldowns", null, true, true, true, '.').obj_val as List<DT.SubValue>;
		if (list != null)
		{
			foreach (DT.SubValue subValue in list)
			{
				List<DT.SubValue> list2 = subValue.value.obj_val as List<DT.SubValue>;
				if (list2 != null && list2.Count > 1)
				{
					Voices.group_cooldowns[list2[0].value.String(null)] = list2[1].value.Float(0f);
				}
			}
		}
		Voices.priority_queue_size = Voices.voices_def_field.GetInt("priority_queue_size", null, Voices.priority_queue_size, true, true, true, '.');
		Voices.weight_default = Voices.voices_def_field.GetInt("weight_default", null, Voices.weight_default, true, true, true, '.');
		Voices.weight_invalid = Voices.voices_def_field.GetInt("weight_invalid", null, Voices.weight_invalid, true, true, true, '.');
		DT.Field field9 = Voices.voices_def_field;
		List<DT.Field> list3;
		if (field9 == null)
		{
			list3 = null;
		}
		else
		{
			DT.Field field10 = field9.FindChild("suppress", null, true, true, true, '.');
			list3 = ((field10 != null) ? field10.Children() : null);
		}
		List<DT.Field> list4 = list3;
		if (list4 != null)
		{
			Profile.BeginSection("Load suppressors");
			for (int i = 0; i < list4.Count; i++)
			{
				DT.Field field11 = list4[i];
				List<DT.Field> list5 = field11.Children();
				if (!string.IsNullOrEmpty(field11.key) && list5 != null)
				{
					List<string> list6 = new List<string>(list5.Count);
					for (int j = 0; j < list5.Count; j++)
					{
						if (!string.IsNullOrEmpty(list5[j].key))
						{
							list6.Add(list5[j].key);
						}
					}
					Voices.suppression_data[field11.key] = list6;
				}
			}
			Profile.EndSection("Load suppressors");
		}
		Voices.ParsePropertiesCSV();
		Profile.BeginSection("Load CSVs");
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator voice lines", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator battle introduction", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Character, "Knights voice lines", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Unit, "Units voice lines", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator voice lines ADDITIONS", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Character, "Knights voice lines ADDITIONS", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator voice lines ADDITIONS 2", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator voice lines ADDITIONS 3", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator battle introduction ADDITIONS", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Unit, "Units voice lines ADDITIONS", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "Narrator voice lines ADDITIONS 4", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Narrator, "VIRTUAL Narrator voice lines", Voices.voices_def_field);
		Voices.LoadCSV(Voices.VoiceLine.Type.Character, "VIRTUAL Knights voice lines", Voices.voices_def_field);
		Profile.EndSection("Load CSVs");
		Profile.BeginSection("LoadActorsDef");
		Voices.LoadActorsDef(Voices.Language);
		Profile.EndSection("LoadActorsDef");
		if (Application.isPlaying && Voices.Language == "dev")
		{
			Profile.BeginSection("LoadSpokenByFromManifest");
			int num;
			Voices.LoadSpokenByFromManifest(out num, null);
			Profile.EndSection("LoadSpokenByFromManifest");
		}
		AudioLog.Info("Loaded voices for language '" + Voices.Language + "'");
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x000D0894 File Offset: 0x000CEA94
	public static bool PlayNarratorVoiceLine(string event_name, IVars vars = null)
	{
		return Voices.Play(Voices.GetNarratorVoiceLine(event_name, vars), 0, event_name, "narrator", Vector3.zero);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x000D08B0 File Offset: 0x000CEAB0
	public static void FillPropertiesCSV()
	{
		using (StreamWriter streamWriter = new StreamWriter(global::Defs.texts_path + "Voices/properties.csv", false))
		{
			streamWriter.WriteLine("event,condition,type,cooldown,que_clearing,fmod_event,priority,bv_priority,timeout,skip_queue,allow_default_condition");
			Dictionary<string, Voices.Event>[] array = Voices.events;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<string, Voices.Event> keyValuePair in array[i])
				{
					string b = "garbage_condition";
					foreach (Voices.VoiceLine voiceLine in keyValuePair.Value.voice_lines)
					{
						if (voiceLine.condition_key != b)
						{
							streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", new object[]
							{
								keyValuePair.Key,
								voiceLine.condition_key,
								voiceLine.type,
								voiceLine.cooldown,
								voiceLine.que_clearing,
								voiceLine.fmod_event,
								voiceLine.priority,
								(voiceLine.bv_priority == 0) ? "" : voiceLine.bv_priority.ToString(),
								voiceLine.timeout,
								voiceLine.skip_queue,
								voiceLine.allow_default_condition
							});
						}
						b = voiceLine.condition_key;
					}
				}
			}
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x000D0AA0 File Offset: 0x000CECA0
	public static void ParsePropertiesCSV()
	{
		string text = global::Defs.texts_path + "Voices/properties.csv";
		Table table = Table.FromFile(text, '\0');
		if (table == null)
		{
			AudioLog.Error(text + " not found");
			return;
		}
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int col = -1;
		int col2 = -1;
		int col3 = -1;
		int col4 = -1;
		int col5 = -1;
		int col6 = -1;
		int col7 = -1;
		int col8 = -1;
		for (int i = 0; i < table.NumCols; i++)
		{
			string text2 = table.Get(0, i);
			uint num4 = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num4 <= 2245568488U)
			{
				if (num4 <= 116600886U)
				{
					if (num4 != 50064974U)
					{
						if (num4 == 116600886U)
						{
							if (text2 == "allow_default_condition")
							{
								col8 = i;
							}
						}
					}
					else if (text2 == "skip_queue")
					{
						col7 = i;
					}
				}
				else if (num4 != 957270816U)
				{
					if (num4 != 1361572173U)
					{
						if (num4 == 2245568488U)
						{
							if (text2 == "cooldown")
							{
								col3 = i;
							}
						}
					}
					else if (text2 == "type")
					{
						num3 = i;
					}
				}
				else if (text2 == "fmod_event")
				{
					col5 = i;
				}
			}
			else if (num4 <= 2633252776U)
			{
				if (num4 != 2498028297U)
				{
					if (num4 != 2549515144U)
					{
						if (num4 == 2633252776U)
						{
							if (text2 == "que_clearing")
							{
								col4 = i;
							}
						}
					}
					else if (text2 == "timeout")
					{
						col6 = i;
					}
				}
				else if (text2 == "priority")
				{
					col = i;
				}
			}
			else if (num4 != 3110511478U)
			{
				if (num4 != 3524459198U)
				{
					if (num4 == 4264611999U)
					{
						if (text2 == "event")
						{
							num = i;
						}
					}
				}
				else if (text2 == "condition")
				{
					num2 = i;
				}
			}
			else if (text2 == "bv_priority")
			{
				col2 = i;
			}
		}
		if (num < 0)
		{
			AudioLog.Error(text + ": No 'event' column found");
			return;
		}
		if (num2 < 0)
		{
			AudioLog.Error(text + ": No 'condition' column found");
			return;
		}
		if (num3 < 0)
		{
			AudioLog.Error(text + ": No 'type' column found");
			return;
		}
		for (int j = 1; j < table.NumRows; j++)
		{
			Voices.VoiceLineProperties voiceLineProperties = new Voices.VoiceLineProperties();
			voiceLineProperties.event_name = table.Get(j, num);
			voiceLineProperties.condition = table.Get(j, num2);
			string a = table.Get(j, num3);
			for (int k = 0; k < 3; k++)
			{
				Voices.VoiceLine.Type type = (Voices.VoiceLine.Type)k;
				if (a == type.ToString())
				{
					voiceLineProperties.type = type;
				}
			}
			voiceLineProperties.priority = table.GetInt(j, col, voiceLineProperties.priority);
			voiceLineProperties.bv_priority = table.GetInt(j, col2, voiceLineProperties.bv_priority);
			voiceLineProperties.cooldown = table.GetFloat(j, col3, voiceLineProperties.cooldown);
			voiceLineProperties.que_clearing = table.GetBool(j, col4, voiceLineProperties.que_clearing);
			string text3 = table.Get(j, col5);
			if (!string.IsNullOrEmpty(text3))
			{
				voiceLineProperties.fmod_event = text3;
			}
			voiceLineProperties.timeout = table.GetFloat(j, col6, voiceLineProperties.timeout);
			voiceLineProperties.skip_queue = table.GetBool(j, col7, voiceLineProperties.skip_queue);
			string a2 = table.Get(j, col8);
			for (int l = 0; l <= 2; l++)
			{
				Voices.VoiceLine.AllowDefaultCondition allow_default_condition = (Voices.VoiceLine.AllowDefaultCondition)l;
				if (a2 == allow_default_condition.ToString())
				{
					voiceLineProperties.allow_default_condition = allow_default_condition;
				}
			}
			Voices.VoiceLineProperties.Add(voiceLineProperties);
		}
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x000D0E90 File Offset: 0x000CF090
	public static bool PlayCharacterVoiceLine(Logic.Character character, string event_name, IVars vars = null, bool no_character_allowed = false)
	{
		Voices.Event @event = Voices.GetEvent(Voices.VoiceLine.Type.Character, event_name, false, 0, null);
		if (@event == null)
		{
			AudioLog.Error("Failed to get character event: " + event_name);
			return false;
		}
		string role = Voices.GetCharacterRole(character, @event);
		int actor_id = Voices.GetCharacterActorID(character, role);
		if (character == null && no_character_allowed)
		{
			role = "marshal";
			actor_id = 1;
		}
		return Voices.Play(Voices.GetCharacterVoiceLine(character, actor_id, event_name, role, vars), actor_id, event_name, role, Vector3.zero);
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x000D0EF4 File Offset: 0x000CF0F4
	public static bool PlayUnitVoiceLine(Logic.Unit unit, string event_name)
	{
		Logic.Unit.Def def = (unit != null) ? unit.def : null;
		string unitRole = Voices.GetUnitRole(def);
		return Voices.Play(Voices.GetUnitVoiceLine(def, -1, event_name, unitRole), -1, event_name, unitRole, Vector3.zero);
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x000D0F2C File Offset: 0x000CF12C
	public static bool PlaySquadVoiceLine(Logic.Squad squad, string event_name, Vector3 position)
	{
		Logic.Unit.Def def = (squad != null) ? squad.def : null;
		string unitRole = Voices.GetUnitRole(def);
		return Voices.Play(Voices.GetUnitVoiceLine(def, -1, event_name, unitRole), -1, event_name, unitRole, position);
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x000D0F5D File Offset: 0x000CF15D
	public static Voices.VoiceLine GetNarratorVoiceLine(string event_name, IVars vars = null)
	{
		return Voices.GetVoiceLine(Voices.VoiceLine.Type.Narrator, 0, event_name, "narrator", vars);
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x000D0F70 File Offset: 0x000CF170
	public static Voices.VoiceLine GetCharacterVoiceLine(Logic.Character character, int actor_id, string event_name, string role, IVars vars = null)
	{
		Voices.VoiceLine voiceLine = Voices.GetVoiceLine(Voices.VoiceLine.Type.Character, actor_id, event_name, role, (vars != null) ? vars : character);
		if (voiceLine != null)
		{
			return voiceLine;
		}
		if (role == "marshal")
		{
			return null;
		}
		if (((character != null) ? character.GetArmy() : null) == null)
		{
			return null;
		}
		return Voices.GetVoiceLine(Voices.VoiceLine.Type.Character, actor_id, event_name, "marshal", character);
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000D0FC6 File Offset: 0x000CF1C6
	public static Voices.VoiceLine GetUnitVoiceLine(Logic.Unit.Def def, int actor_id, string event_name, string role)
	{
		return Voices.GetVoiceLine(Voices.VoiceLine.Type.Unit, actor_id, event_name, role, def);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000D0FD4 File Offset: 0x000CF1D4
	public static void FillBSGToTHQEthnicityMapping(DT.Field field)
	{
		for (int i = 0; i < 7; i++)
		{
			Logic.Character.Ethnicity ethnicity = (Logic.Character.Ethnicity)i;
			List<string> list = (field != null) ? field.GetListOfStrings(ethnicity.ToString()) : null;
			Voices.BSGToTHQEthnicityMapping[i] = list;
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000D1014 File Offset: 0x000CF214
	private static int GetCharacterActorID(Logic.Character character, string role)
	{
		if (character == null)
		{
			return -2;
		}
		if (string.IsNullOrEmpty(role))
		{
			return -1;
		}
		Voices.Role role2 = Voices.GetRole(Voices.VoiceLine.Type.Character, role, false);
		if (role2 == null)
		{
			return -1;
		}
		Voices.RefreshActorIDEntries();
		int num = Voices.ChooseActorID(character, role2);
		character.VO_actor_id = num;
		Voices.RegisterCharacterActorID(character, num, Voices.Language);
		return num;
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000D1060 File Offset: 0x000CF260
	private static int ChooseActorID(Logic.Character character, Voices.Role r)
	{
		if (r.spoken_by.Count == 0)
		{
			return -1;
		}
		if (r.spoken_by.Count == 1)
		{
			return r.spoken_by[0];
		}
		if (character.VO_actor_id > 0 && character.VO_actor_id <= Voices.max_actor_id && r.spoken_by.Contains(character.VO_actor_id))
		{
			return character.VO_actor_id;
		}
		for (int i = 0; i < Voices.tmp_actor_ids.Length; i++)
		{
			Voices.tmp_actor_ids[i].Clear();
		}
		for (int j = 0; j < r.spoken_by.Count; j++)
		{
			int num = r.spoken_by[j];
			int num2 = Voices.PrioritizeActorID(num, character.ethnicity);
			if (num2 >= 0 && num2 < Voices.tmp_actor_ids.Length)
			{
				Voices.tmp_actor_ids[num2].Add(num);
			}
		}
		int k = 0;
		while (k < Voices.tmp_actor_ids.Length)
		{
			List<int> list = Voices.tmp_actor_ids[k];
			if (list.Count != 0)
			{
				if (list.Count == 1)
				{
					return list[0];
				}
				int index = character.game.Random(0, list.Count);
				return list[index];
			}
			else
			{
				k++;
			}
		}
		return -1;
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000D118C File Offset: 0x000CF38C
	private static int PrioritizeActorID(int actor_id, Logic.Character.Ethnicity ethnicity)
	{
		if (actor_id <= 0 || actor_id > Voices.max_actor_id)
		{
			return -1;
		}
		Voices.AllocatedActorIDCharacters[] array;
		if (!Voices.allocated_character_actor_ids.TryGetValue(Voices.Language, out array))
		{
			return 0;
		}
		Voices.Actor actor = Voices.actors[actor_id - 1];
		List<string> list = Voices.BSGToTHQEthnicityMapping[(int)ethnicity];
		if (list != null && list.Count > 0 && !list.Contains(actor.ethnicity))
		{
			return -1;
		}
		Voices.AllocatedActorIDCharacters allocatedActorIDCharacters = array[actor_id];
		if (!allocatedActorIDCharacters.IsValid())
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < allocatedActorIDCharacters.in_court.Count; i++)
		{
			if (Voices.IsInPlayerCourt(allocatedActorIDCharacters.in_court[i].character))
			{
				return 3;
			}
			num = 2;
		}
		if (num == 2)
		{
			return num;
		}
		for (int j = 0; j < allocatedActorIDCharacters.others.Count; j++)
		{
			if (Voices.IsReachable(allocatedActorIDCharacters.others[j].character, false))
			{
				return 2;
			}
			num = 1;
		}
		return num;
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000D1280 File Offset: 0x000CF480
	private static int FindCharacterInfoIndex(List<Voices.AllocatedActorIDCharacters.CharacterInfo> lst, int nid)
	{
		if (lst == null)
		{
			return -1;
		}
		for (int i = 0; i < lst.Count; i++)
		{
			if (lst[i].nid == nid)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x000D12B8 File Offset: 0x000CF4B8
	private static void RemoveObsoletteActorIDEntries(List<Voices.AllocatedActorIDCharacters.CharacterInfo> lst, Logic.Time now)
	{
		if (lst == null)
		{
			return;
		}
		float num = 900f;
		for (int i = lst.Count - 1; i >= 0; i--)
		{
			Voices.AllocatedActorIDCharacters.CharacterInfo characterInfo = lst[i];
			bool in_court = Voices.IsInPlayerCourt(characterInfo.character);
			if (!Voices.IsReachable(characterInfo.character, in_court) && now - characterInfo.timestamp >= num)
			{
				lst.RemoveAt(i);
			}
		}
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x000D131C File Offset: 0x000CF51C
	public static void RefreshActorIDEntries()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		Logic.Time time = game.time;
		foreach (KeyValuePair<string, Voices.AllocatedActorIDCharacters[]> keyValuePair in Voices.allocated_character_actor_ids)
		{
			foreach (Voices.AllocatedActorIDCharacters allocatedActorIDCharacters in keyValuePair.Value)
			{
				if (allocatedActorIDCharacters.IsValid())
				{
					Voices.RemoveObsoletteActorIDEntries(allocatedActorIDCharacters.in_court, time);
					Voices.RemoveObsoletteActorIDEntries(allocatedActorIDCharacters.others, time);
					Voices.CheckMovedInCourt(allocatedActorIDCharacters);
				}
			}
		}
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x000D13CC File Offset: 0x000CF5CC
	private static void CheckMovedInCourt(Voices.AllocatedActorIDCharacters id_info)
	{
		if (id_info.in_court == null || id_info.others == null)
		{
			return;
		}
		for (int i = id_info.others.Count - 1; i >= 0; i--)
		{
			Voices.AllocatedActorIDCharacters.CharacterInfo characterInfo = id_info.others[i];
			Logic.Character character = characterInfo.character;
			if (character != null && character.IsValid() && Voices.IsInPlayerCourt(character))
			{
				if (Voices.FindCharacterInfoIndex(id_info.in_court, characterInfo.nid) < 0)
				{
					id_info.in_court.Add(characterInfo);
				}
				id_info.others.RemoveAt(i);
			}
		}
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x000D1455 File Offset: 0x000CF655
	private static bool IsReachable(Logic.Character character, bool in_court)
	{
		return in_court || (character != null && character.IsValid() && ((character.IsRebel() && character.GetArmy() != null) || (character.prison_kingdom != null && character.prison_kingdom == BaseUI.LogicKingdom())));
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x000D1495 File Offset: 0x000CF695
	private static bool IsInPlayerCourt(Logic.Character character)
	{
		return character != null && (character.IsInLocalPlayerCourt() || character.IsInLocalPlayerSpecialCourt());
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x000D14B4 File Offset: 0x000CF6B4
	private static void RegisterCharacterActorID(Logic.Character character, int actor_id, string language)
	{
		if (actor_id <= 0)
		{
			return;
		}
		if (actor_id > Voices.max_actor_id)
		{
			AudioLog.Error(string.Format("Attempting to assing actor id {0} / {1} for character {2}", actor_id, Voices.max_actor_id, character));
			return;
		}
		Voices.AllocatedActorIDCharacters[] array;
		if (!Voices.allocated_character_actor_ids.TryGetValue(language, out array))
		{
			Voices.allocated_character_actor_ids[language] = new Voices.AllocatedActorIDCharacters[Voices.max_actor_id + 1];
			array = Voices.allocated_character_actor_ids[language];
		}
		Voices.AllocatedActorIDCharacters allocatedActorIDCharacters = array[actor_id];
		if (!allocatedActorIDCharacters.IsValid())
		{
			allocatedActorIDCharacters.actor_id = actor_id;
			allocatedActorIDCharacters.in_court = new List<Voices.AllocatedActorIDCharacters.CharacterInfo>();
			allocatedActorIDCharacters.others = new List<Voices.AllocatedActorIDCharacters.CharacterInfo>();
			array[actor_id] = allocatedActorIDCharacters;
		}
		bool flag = Voices.IsInPlayerCourt(character);
		int nid = character.GetNid(false);
		Logic.Time time = character.game.time;
		int num = Voices.FindCharacterInfoIndex(allocatedActorIDCharacters.in_court, nid);
		int num2 = Voices.FindCharacterInfoIndex(allocatedActorIDCharacters.others, nid);
		if (num >= 0)
		{
			flag = true;
		}
		List<Voices.AllocatedActorIDCharacters.CharacterInfo> list = flag ? allocatedActorIDCharacters.in_court : allocatedActorIDCharacters.others;
		int num3 = flag ? num : num2;
		if (num3 < 0)
		{
			Voices.AllocatedActorIDCharacters.CharacterInfo item = new Voices.AllocatedActorIDCharacters.CharacterInfo
			{
				character = character,
				nid = nid,
				timestamp = time
			};
			list.Add(item);
			return;
		}
		Voices.AllocatedActorIDCharacters.CharacterInfo value = list[num3];
		value.timestamp = time;
		list[num3] = value;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x000D160C File Offset: 0x000CF80C
	private static Voices.Role GetRole(Voices.VoiceLine.Type type, string id, bool create_if_not_found)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		Dictionary<string, Voices.Role> dictionary = Voices.roles[(int)type];
		if (dictionary == null)
		{
			if (!create_if_not_found)
			{
				return null;
			}
			dictionary = new Dictionary<string, Voices.Role>();
			Voices.roles[(int)type] = dictionary;
		}
		Voices.Role role;
		if (dictionary.TryGetValue(id, out role))
		{
			return role;
		}
		if (!create_if_not_found)
		{
			return null;
		}
		Profile.BeginSection("Add Role");
		DT.Field field = Voices.role_fields[(int)type];
		DT.Field field2 = (field != null) ? field.FindChild(id, null, true, true, true, '.') : null;
		string description = (field2 != null) ? field2.GetString("description", null, "", true, true, true, '.') : null;
		role = new Voices.Role
		{
			type = type,
			id = id,
			def = field2,
			description = description
		};
		dictionary.Add(id, role);
		Profile.EndSection("Add Role");
		return role;
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x000D16CC File Offset: 0x000CF8CC
	private static Voices.VoiceLine GetVoiceLine(Voices.VoiceLine.Type type, int actor_id, string event_name, string role, IVars vars)
	{
		Voices.Event @event = Voices.GetEvent(type, event_name, false, 0, null);
		if (@event == null)
		{
			return null;
		}
		bool res_default = @event.EvalDefaultCondition(vars);
		bool flag = false;
		Voices.valid_voice_lines.Clear();
		Voices.conditions_cache.Clear();
		for (int i = 0; i < @event.voice_lines.Count; i++)
		{
			Voices.VoiceLine voiceLine = @event.voice_lines[i];
			if (!(voiceLine.role_name != role) && (actor_id < 0 || voiceLine.SpokenBy.Contains(actor_id)) && Voices.CheckCondition(voiceLine, vars, res_default))
			{
				if (string.IsNullOrEmpty(voiceLine.condition_key))
				{
					if (voiceLine.allow_default_condition == Voices.VoiceLine.AllowDefaultCondition.Never)
					{
						goto IL_99;
					}
				}
				else
				{
					flag = true;
				}
				Voices.valid_voice_lines.Add(voiceLine);
			}
			IL_99:;
		}
		WeightedRandom<Voices.VoiceLine> temp = WeightedRandom<Voices.VoiceLine>.GetTemp(32);
		for (int j = 0; j < Voices.valid_voice_lines.Count; j++)
		{
			if ((Voices.valid_voice_lines[j].allow_default_condition != Voices.VoiceLine.AllowDefaultCondition.WhenAllConditionsFail || !flag || !string.IsNullOrEmpty(Voices.valid_voice_lines[j].condition_key)) && (Voices.valid_voice_lines[j].variant != @event.last_variant_played || Voices.valid_voice_lines.Count <= 1))
			{
				temp.AddOption(Voices.valid_voice_lines[j], (float)Voices.valid_voice_lines[j].weight);
			}
		}
		if (temp.options.Count == 0)
		{
			return null;
		}
		return temp.Choose(null, false);
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x000D1840 File Offset: 0x000CFA40
	private static bool CheckCondition(Voices.VoiceLine voice_line, IVars vars, bool res_default = true)
	{
		if (voice_line.condition_field == null || string.IsNullOrEmpty(voice_line.condition_key))
		{
			return res_default;
		}
		bool flag;
		if (Voices.conditions_cache.TryGetValue(voice_line.condition_key, out flag))
		{
			return flag;
		}
		flag = voice_line.condition_field.Bool(vars, false);
		Voices.conditions_cache.Add(voice_line.condition_key, flag);
		return flag;
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x000D189C File Offset: 0x000CFA9C
	private static string GetCharacterRole(Logic.Character character, Voices.Event evt)
	{
		if (character == null)
		{
			return null;
		}
		DT.Field field = Voices.role_fields[1];
		if (((field != null) ? field.children : null) != null)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				if (!string.IsNullOrEmpty(field2.key))
				{
					bool? flag;
					if (evt == null)
					{
						flag = null;
					}
					else
					{
						List<string> ignore_roles = evt.ignore_roles;
						flag = ((ignore_roles != null) ? new bool?(ignore_roles.Contains(field2.key)) : null);
					}
					if (!(flag ?? false) && field2.Bool(character, false))
					{
						return field2.key;
					}
				}
			}
			return null;
		}
		string class_name = character.class_name;
		if (class_name == null)
		{
			return null;
		}
		return class_name.ToLowerInvariant();
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x000D1968 File Offset: 0x000CFB68
	private static string GetUnitRole(Logic.Unit.Def def)
	{
		DT.Field field;
		if (def == null)
		{
			field = null;
		}
		else
		{
			DT.Field field2 = def.field;
			field = ((field2 != null) ? field2.FindChild("VO_role", null, true, true, true, '.') : null);
		}
		DT.Field field3 = field;
		if (field3 == null)
		{
			return null;
		}
		return field3.String(null, "");
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x000D19AC File Offset: 0x000CFBAC
	private static bool Play(Voices.VoiceLine voice_line, int actor_id, string event_name, string role, Vector3 position)
	{
		if (voice_line == null)
		{
			string str = ((actor_id >= 0) ? string.Format("{0:D2}_", actor_id) : "") + role + "_" + event_name;
			AudioLog.Warning("Failed to find a suitable voice line: " + str);
			return false;
		}
		string str2;
		if (actor_id >= 0)
		{
			str2 = string.Format("{0:D2}/", actor_id);
		}
		else
		{
			str2 = role + "/";
		}
		BaseUI.PlayVoiceLine(str2 + Voices.VoiceLineSoundID(actor_id, (voice_line.refers_to != null) ? voice_line.refers_to.id : voice_line.id), position, voice_line);
		return true;
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x000D1A49 File Offset: 0x000CFC49
	private static int CalcUnitActorID(string role, int max_actor_id)
	{
		return (int)(1UL + (ulong)role.GetHashCode() % (ulong)((long)max_actor_id));
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x000D1A5C File Offset: 0x000CFC5C
	private static void CalcSpokenBy(Voices.VoiceLine.Type type, string role, List<int> spoken_by)
	{
		switch (type)
		{
		case Voices.VoiceLine.Type.Narrator:
			spoken_by.Add(0);
			return;
		case Voices.VoiceLine.Type.Character:
			for (int i = 1; i <= Voices.max_actor_id; i++)
			{
				spoken_by.Add(i);
			}
			return;
		case Voices.VoiceLine.Type.Unit:
		{
			int item = Voices.CalcUnitActorID(role, Voices.max_actor_id);
			spoken_by.Add(item);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x000D1AAF File Offset: 0x000CFCAF
	private static string VoiceLineSoundID(int actor_id, string voice_line_id)
	{
		return ((actor_id >= 0) ? string.Format("{0:D2}_", actor_id) : "") + voice_line_id;
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x000D1AD4 File Offset: 0x000CFCD4
	public static void Clear()
	{
		Voices.all_voice_lines.Clear();
		Voices.voice_lines_in_order.Clear();
		Voices.suppression_data.Clear();
		Voices.VoiceLineProperties.Clear();
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			Dictionary<string, Voices.Role> dictionary = Voices.roles[i];
			if (dictionary != null)
			{
				dictionary.Clear();
			}
			Dictionary<string, Dictionary<string, int>> dictionary2 = Voices.unique_event_variants[i];
			if (dictionary2 != null)
			{
				dictionary2.Clear();
			}
			Dictionary<string, Voices.Event> dictionary3 = Voices.events[i];
			if (dictionary3 != null)
			{
				dictionary3.Clear();
			}
		}
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000D1B48 File Offset: 0x000CFD48
	private static void LoadCSV(Voices.VoiceLine.Type type, string filename, DT.Field voices_def_field)
	{
		bool flag = filename.ToLowerInvariant().Contains("virtual");
		string text = global::Defs.texts_path + "Voices/" + filename + ".csv";
		Table table = Table.FromFile(text, '\0');
		if (table == null)
		{
			AudioLog.Error(text + " not found");
			return;
		}
		DT.Field field = (voices_def_field != null) ? voices_def_field.FindChild(string.Format("conditions.{0}", type), null, true, true, true, '.') : null;
		DT.Field field2 = (voices_def_field != null) ? voices_def_field.FindChild(string.Format("events.{0}", type), null, true, true, true, '.') : null;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		int num6 = 0;
		for (int i = 0; i < table.NumCols; i++)
		{
			string text2 = table.Get(0, i);
			if (!(text2 == "event"))
			{
				if (!(text2 == "condition"))
				{
					if (!(text2 == "default"))
					{
						if (!(text2 == "comments"))
						{
							if (!(text2 == "weight"))
							{
								if (Voices.IsRoleColumn(text2))
								{
									if (type == Voices.VoiceLine.Type.Narrator && text2 != "narrator")
									{
										AudioLog.Error(text + ": invalid colum: '" + text2 + "', only 'narrator' is allowed");
									}
									num6++;
								}
							}
							else
							{
								num5 = i;
							}
						}
						else
						{
							num4 = i;
						}
					}
					else
					{
						num3 = i;
					}
				}
				else
				{
					num2 = i;
				}
			}
			else
			{
				num = i;
			}
		}
		if (num < 0)
		{
			AudioLog.Error(text + ": No 'event' column found");
			return;
		}
		if (num6 == 0)
		{
			AudioLog.Error(text + ": No role columns found");
			return;
		}
		for (int j = 1; j < table.NumRows; j++)
		{
			string text3 = table.Get(j, num);
			if (!string.IsNullOrEmpty(text3))
			{
				if (text3 == "//comment")
				{
					Voices.ParseUnitRoleComments(type, table, j);
				}
				else if (text3 == "//unit_ids")
				{
					Voices.ParseUnitRoles(filename, table, j);
				}
				else if (text3 == "//old_role")
				{
					Voices.ParseOldRole(type, table, j);
				}
				else if (!text3.StartsWith("//", StringComparison.Ordinal))
				{
					Voices.VoiceLineCreationParams voiceLineCreationParams = new Voices.VoiceLineCreationParams();
					voiceLineCreationParams.filename = filename;
					voiceLineCreationParams.line_number = j;
					voiceLineCreationParams.evt = Voices.GetEvent(type, text3, true, j, field2);
					voiceLineCreationParams.condition_key = ((num2 == -1) ? null : table.Get(j, num2));
					voiceLineCreationParams.weight = ((num5 == -1) ? Voices.weight_invalid : table.GetInt(j, num5, Voices.weight_invalid));
					voiceLineCreationParams.variant = Voices.GetVariant(voiceLineCreationParams.evt, voiceLineCreationParams.condition_key);
					voiceLineCreationParams.condition_field = Voices.ResolveCondition(text, j, voiceLineCreationParams.condition_key, field);
					voiceLineCreationParams.default_text = ((num3 < -1) ? null : table.Get(j, num3));
					voiceLineCreationParams.comment = ((num4 < -1) ? null : table.Get(j, num4));
					Voices.VoiceLine voiceLine = null;
					for (int k = 0; k < table.NumCols; k++)
					{
						if (k != num && k != num2 && k != num4)
						{
							voiceLineCreationParams.role_name = table.Get(0, k);
							voiceLineCreationParams.dev_text = table.Get(j, k);
							if (voiceLineCreationParams.role_name == "//fmod event")
							{
								if (!string.IsNullOrEmpty(voiceLineCreationParams.dev_text) && !(voiceLineCreationParams.dev_text == "not implemented") && !(voiceLineCreationParams.dev_text == "NOT IMPLEMENTED"))
								{
									if (voiceLine != null)
									{
										voiceLine.old_fmod_event = voiceLineCreationParams.dev_text;
									}
									else
									{
										AudioLog.Error(string.Format("{0}({1}:{2}): fmod event defined for missing voice line ({3})", new object[]
										{
											text,
											j + 1,
											65 + k,
											text3
										}));
									}
								}
							}
							else if (voiceLineCreationParams.role_name == "//audio file old")
							{
								if (!string.IsNullOrEmpty(voiceLineCreationParams.dev_text) && !(voiceLineCreationParams.dev_text == "TBR") && !(voiceLineCreationParams.dev_text == "RENAMED_ALREADY"))
								{
									if (voiceLine != null)
									{
										voiceLine.old_audio_file = voiceLineCreationParams.dev_text;
									}
									else
									{
										AudioLog.Error(string.Format("{0}({1}:{2}): old audio file defined for missing voice line ({3})", new object[]
										{
											text,
											j + 1,
											65 + k,
											text3
										}));
									}
								}
							}
							else if (Voices.IsRoleColumn(voiceLineCreationParams.role_name))
							{
								if (voiceLineCreationParams.dev_text == "x" || voiceLineCreationParams.dev_text == "X")
								{
									voiceLine = null;
								}
								else if (flag)
								{
									Voices.VoiceLine voiceLine2;
									if (string.IsNullOrEmpty(voiceLineCreationParams.dev_text))
									{
										AudioLog.Error("Empty field in virtual voice line csv!" + string.Format("\n\tFilename: {0}\n\tRow: {1}\n\tColumn: {2}\n\tEvent: {3}", new object[]
										{
											filename,
											j + 1,
											k + 1,
											voiceLineCreationParams.evt.id
										}));
										voiceLine = null;
									}
									else if (Voices.all_voice_lines.TryGetValue(voiceLineCreationParams.dev_text, out voiceLine2))
									{
										voiceLineCreationParams.role_name = voiceLine2.role_name;
										voiceLineCreationParams.dev_text = voiceLine2.dev_text;
										voiceLineCreationParams.is_default_text = voiceLine2.is_default_text;
										voiceLineCreationParams.refers_to = voiceLine2;
										voiceLine = Voices.AddVoiceLine(voiceLineCreationParams, filename, j + 1, k + 1);
									}
									else
									{
										AudioLog.Error("Failed to find voice line: " + voiceLineCreationParams.dev_text);
										voiceLine = null;
									}
								}
								else
								{
									voiceLineCreationParams.is_default_text = false;
									if (string.IsNullOrEmpty(voiceLineCreationParams.dev_text))
									{
										if (k == num3)
										{
											goto IL_5CD;
										}
										voiceLineCreationParams.is_default_text = true;
										voiceLineCreationParams.dev_text = voiceLineCreationParams.default_text;
									}
									voiceLine = Voices.AddVoiceLine(voiceLineCreationParams, filename, j + 1, k + 1);
								}
							}
						}
						IL_5CD:;
					}
				}
			}
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x000D2148 File Offset: 0x000D0348
	private static bool IsRoleColumn(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		char c = name[0];
		return c != '_' && c != '/';
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x000D2178 File Offset: 0x000D0378
	private static void ParseUnitRoleComments(Voices.VoiceLine.Type type, Table tbl, int row)
	{
		for (int i = 1; i < tbl.NumCols; i++)
		{
			string text = tbl.Get(row, i);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = tbl.Get(0, i);
				if (Voices.IsRoleColumn(text2))
				{
					Voices.Role role = Voices.GetRole(type, text2, true);
					if (!string.IsNullOrEmpty(role.description))
					{
						AudioLog.Warning("Unit role '" + text2 + "' description overriden in defs");
					}
					else
					{
						role.description = text;
					}
				}
			}
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000D21EC File Offset: 0x000D03EC
	private static void ParseUnitRoles(string filename, Table tbl, int row)
	{
		for (int i = 1; i < tbl.NumCols; i++)
		{
			string text = tbl.Get(row, i);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = tbl.Get(0, i);
				if (string.IsNullOrEmpty(text2))
				{
					AudioLog.Error(string.Format("{0}({1}): No role defined in column {2}", filename, row + 1, 65 + i));
				}
				else
				{
					int num = 0;
					while (num < text.Length && DT.Parser.IsBlank(text[num]))
					{
						num++;
					}
					for (;;)
					{
						string text3 = Expression.ReadIdentifier(text, ref num);
						while (num < text.Length && DT.Parser.IsBlank(text[num]))
						{
							num++;
						}
						if (string.IsNullOrEmpty(text3))
						{
							break;
						}
						DT.Field defField = global::Defs.GetDefField(text3, null);
						string a;
						if (defField == null)
						{
							a = null;
						}
						else
						{
							DT.Field field = defField.BaseRoot();
							a = ((field != null) ? field.key : null);
						}
						if (a != "Unit")
						{
							AudioLog.Error(string.Format("{0}({1}): Unknown unit def: {2}", filename, row + 1, text3));
						}
						else
						{
							DT.Field field2 = defField.FindChild("VO_role", null, false, true, true, '.');
							if (field2 != null)
							{
								string text4 = field2.String(null, "");
								if (text4 != text2)
								{
									AudioLog.Error(string.Format("{0}({1}): Unit {2} already has an assigned VO role '{3}'", new object[]
									{
										filename,
										row + 1,
										text3,
										text4
									}));
								}
							}
							else
							{
								field2 = defField.AddChild("VO_role");
								field2.flags |= DT.Field.Flags.DontSave;
								field2.value = text2;
							}
						}
					}
					if (num < text.Length)
					{
						AudioLog.Error(string.Format("{0}({1}): Non-identifier character found", filename, row + 1));
					}
				}
			}
		}
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000D23B4 File Offset: 0x000D05B4
	private static void ParseOldRole(Voices.VoiceLine.Type type, Table tbl, int row)
	{
		for (int i = 1; i < tbl.NumCols; i++)
		{
			string text = tbl.Get(row, i);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = tbl.Get(0, i);
				if (Voices.IsRoleColumn(text2))
				{
					Voices.GetRole(type, text2, true).old_role = text;
				}
			}
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000D2404 File Offset: 0x000D0604
	private static void ValidateUnitRoles()
	{
		DT.Field defField = global::Defs.GetDefField("Unit", null);
		bool flag;
		if (defField == null)
		{
			flag = (null != null);
		}
		else
		{
			DT.Def def = defField.def;
			flag = (((def != null) ? def.defs : null) != null);
		}
		if (!flag)
		{
			return;
		}
		StringBuilder stringBuilder = null;
		int num = 0;
		for (int i = 0; i < defField.def.defs.Count; i++)
		{
			DT.Def def2 = defField.def.defs[i];
			bool flag2;
			if (def2 == null)
			{
				flag2 = (null != null);
			}
			else
			{
				DT.Field field = def2.field;
				flag2 = (((field != null) ? field.FindChild("VO_role", null, true, true, true, '.') : null) != null);
			}
			if (!flag2)
			{
				num++;
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder();
				}
				stringBuilder.AppendLine(def2.path);
			}
		}
		if (stringBuilder == null)
		{
			return;
		}
		string arg = stringBuilder.ToString();
		AudioLog.Warning(string.Format("Units with missing VO role: {0}\n{1}", num, arg));
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x000D24D4 File Offset: 0x000D06D4
	private static void LoadActorsDef(string language)
	{
		DT.Field field = null;
		DT.Field field2 = null;
		string text = Voices.ManifestsDir + "/" + language + "_actors.def";
		List<DT.Field> list = DT.Parser.ReadFile(null, text, null);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field3 = list[i];
				string key = field3.key;
				if (!(key == "actors"))
				{
					if (key == "BSGToTHQEthnicityMapping")
					{
						field2 = field3;
					}
				}
				else
				{
					field = field3;
				}
			}
			if (field == null)
			{
				AudioLog.Error("No 'actors' field in " + text);
			}
		}
		else
		{
			AudioLog.Error(text + " not found");
		}
		Voices.LoadActors(field);
		Voices.FillBSGToTHQEthnicityMapping(field2);
		Voices.LoadRolesSpokenBy();
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x000D258C File Offset: 0x000D078C
	private static void LoadActors(DT.Field actors_field)
	{
		Voices.actors.Clear();
		if (actors_field == null)
		{
			Voices.max_actor_id = 15;
		}
		else
		{
			Voices.max_actor_id = actors_field.Int(null, 0);
			if (Voices.max_actor_id <= 0)
			{
				AudioLog.Error(string.Concat(new string[]
				{
					"Invalid value for 'actors' field in ",
					Voices.ManifestsDir,
					"/",
					Voices.Language,
					"_actors.def"
				}));
				Voices.max_actor_id = 15;
			}
		}
		for (int i = 1; i <= Voices.max_actor_id; i++)
		{
			string path = string.Format("{0:D2}", i);
			DT.Field field = (actors_field != null) ? actors_field.FindChild(path, null, true, true, true, '.') : null;
			Voices.Actor item = new Voices.Actor
			{
				id = i,
				name = ((field != null) ? field.GetString("name", null, "", true, true, true, '.') : null),
				ethnicity = ((field != null) ? field.GetString("ethnicity", null, "", true, true, true, '.') : null)
			};
			Voices.actors.Add(item);
		}
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000D269C File Offset: 0x000D089C
	private static void LoadRolesSpokenBy()
	{
		for (Voices.VoiceLine.Type type = Voices.VoiceLine.Type.Narrator; type < Voices.VoiceLine.Type.COUNT; type++)
		{
			if (type != Voices.VoiceLine.Type.Unit)
			{
				Dictionary<string, Voices.Role> dictionary = Voices.roles[(int)type];
				if (dictionary == null)
				{
					AudioLog.Error(string.Format("No roles for {0} voices", type));
				}
				else
				{
					foreach (KeyValuePair<string, Voices.Role> keyValuePair in dictionary)
					{
						Voices.Role value = keyValuePair.Value;
						value.spoken_by.Clear();
						if (!(value.id == "default"))
						{
							if (type != Voices.VoiceLine.Type.Narrator)
							{
								if (type == Voices.VoiceLine.Type.Character)
								{
									for (int i = 1; i <= Voices.max_actor_id; i++)
									{
										value.spoken_by.Add(i);
									}
								}
							}
							else
							{
								value.spoken_by.Add(0);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x000D2784 File Offset: 0x000D0984
	private static Voices.VoiceLine AddVoiceLine(Voices.VoiceLineCreationParams param, string csv_file, int csv_row, int csv_col)
	{
		Profile.BeginSection("AddVoiceLine");
		string text = Voices.MakeID(param.evt.id, param.condition_key, param.role_name, param.variant);
		Voices.Role role = Voices.GetRole(param.evt.type, param.role_name, true);
		string localized_text = param.dev_text;
		if (!global::Defs.IsDevLanguage())
		{
			string text_key;
			if (param.is_default_text)
			{
				text_key = "voice_" + Voices.MakeID(param.evt.id, param.condition_key, "default", param.variant);
			}
			else
			{
				text_key = "voice_" + text;
			}
			string text2 = global::Defs.Localize(text_key, null, null, false, true);
			if (!string.IsNullOrEmpty(text2))
			{
				localized_text = text2;
			}
			else
			{
				localized_text = param.dev_text;
			}
		}
		Voices.VoiceLine voiceLine;
		if (Voices.all_voice_lines.TryGetValue(text, out voiceLine))
		{
			AudioLog.Error(string.Format("Duplicated voice line id: {0}\n{1}:{2}\n{3}:{4}", new object[]
			{
				text,
				voiceLine.filename,
				voiceLine.line_number,
				param.filename,
				param.line_number
			}));
		}
		else
		{
			voiceLine = new Voices.VoiceLine
			{
				id = text
			};
			Voices.all_voice_lines.Add(text, voiceLine);
			Voices.voice_lines_in_order.Add(voiceLine);
			if (role != null)
			{
				role.voice_lines.Add(voiceLine);
			}
			param.evt.voice_lines.Add(voiceLine);
			if (!string.IsNullOrEmpty(voiceLine.condition_key))
			{
				param.evt.has_conditions = true;
			}
		}
		voiceLine.filename = param.filename;
		voiceLine.line_number = param.line_number;
		voiceLine.type = param.evt.type;
		voiceLine.event_name = param.evt.id;
		voiceLine.condition_key = param.condition_key;
		voiceLine.variant = param.variant;
		voiceLine.role_name = param.role_name;
		voiceLine.dev_text = param.dev_text;
		voiceLine.localized_text = localized_text;
		voiceLine.is_default_text = param.is_default_text;
		voiceLine.comment = param.comment;
		voiceLine.evt = param.evt;
		voiceLine.role = role;
		voiceLine.condition_field = param.condition_field;
		voiceLine.spoken_by = ((role != null) ? role.spoken_by : null);
		voiceLine.refers_to = param.refers_to;
		voiceLine.weight = ((param.weight == Voices.weight_invalid) ? voiceLine.GetInt("weight", Voices.weight_default) : param.weight);
		Voices.VoiceLineProperties voiceLineProperties = Voices.VoiceLineProperties.Get(voiceLine);
		voiceLine.priority = voiceLineProperties.priority;
		voiceLine.bv_priority = voiceLineProperties.bv_priority;
		voiceLine.cooldown = voiceLineProperties.cooldown;
		voiceLine.que_clearing = voiceLineProperties.que_clearing;
		voiceLine.timeout = voiceLineProperties.timeout;
		voiceLine.fmod_event = voiceLineProperties.fmod_event;
		voiceLine.skip_queue = voiceLineProperties.skip_queue;
		voiceLine.allow_default_condition = voiceLineProperties.allow_default_condition;
		Profile.EndSection("AddVoiceLine");
		return voiceLine;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x000D2A6C File Offset: 0x000D0C6C
	public static void Save(DT.Field field)
	{
		if (field == null)
		{
			return;
		}
		if (GameLogic.Get(false) == null)
		{
			return;
		}
		if (Voices.allocated_character_actor_ids.Count == 0)
		{
			return;
		}
		DT.Field field2 = field.CreateChild("allocated_character_actor_ids", '.');
		if (field2 == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Voices.AllocatedActorIDCharacters[]> keyValuePair in Voices.allocated_character_actor_ids)
		{
			DT.Field field3 = field2.CreateChild(keyValuePair.Key, '.');
			foreach (Voices.AllocatedActorIDCharacters allocatedActorIDCharacters in keyValuePair.Value)
			{
				if (allocatedActorIDCharacters.IsValid())
				{
					for (int j = 0; j < allocatedActorIDCharacters.in_court.Count; j++)
					{
						Voices.AllocatedActorIDCharacters.CharacterInfo characterInfo = allocatedActorIDCharacters.in_court[j];
						if (characterInfo.character != null)
						{
							field3.SetValue(characterInfo.nid.ToString(), allocatedActorIDCharacters.actor_id);
						}
					}
					for (int k = 0; k < allocatedActorIDCharacters.others.Count; k++)
					{
						Voices.AllocatedActorIDCharacters.CharacterInfo characterInfo2 = allocatedActorIDCharacters.others[k];
						if (characterInfo2.character != null)
						{
							field3.SetValue(characterInfo2.nid.ToString(), allocatedActorIDCharacters.actor_id);
						}
					}
				}
			}
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x000D2BE4 File Offset: 0x000D0DE4
	public static void Load(DT.Field field)
	{
		if (field == null)
		{
			return;
		}
		Voices.<>c__DisplayClass83_0 CS$<>8__locals1;
		CS$<>8__locals1.game = GameLogic.Get(false);
		if (CS$<>8__locals1.game == null)
		{
			return;
		}
		Voices.allocated_character_actor_ids.Clear();
		DT.Field field2 = field.FindChild("allocated_character_actor_ids", null, true, true, true, '.');
		List<DT.Field> list;
		if ((list = ((field2 != null) ? field2.Children() : null)) == null)
		{
			return;
		}
		foreach (DT.Field field3 in list)
		{
			string key = field3.key;
			if (!string.IsNullOrEmpty(key))
			{
				List<DT.Field> list2 = field3.Children();
				if (list2 != null && list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						Voices.<Load>g__ParseCharacterField|83_0(list2[i], key, ref CS$<>8__locals1);
					}
				}
			}
		}
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000D2CC0 File Offset: 0x000D0EC0
	private static Voices.Event GetEvent(Voices.VoiceLine.Type type, string event_name, bool create, int row = 0, DT.Field events_field = null)
	{
		Dictionary<string, Voices.Event> dictionary = Voices.events[(int)type];
		if (dictionary == null)
		{
			dictionary = new Dictionary<string, Voices.Event>();
			Voices.events[(int)type] = dictionary;
		}
		Voices.Event @event;
		if (!dictionary.TryGetValue(event_name, out @event))
		{
			if (!create)
			{
				return null;
			}
			Profile.BeginSection("Add Voice Event");
			DT.Field field = (events_field != null) ? events_field.FindChild(event_name, null, true, true, true, '.') : null;
			string description = (field != null) ? field.GetString("description", null, "", true, true, true, '.') : null;
			DT.Field default_condition = (field != null) ? field.FindChild("default_condition", null, true, true, true, '.') : null;
			List<string> list = (field != null) ? field.GetListOfStrings("ignore_roles") : null;
			@event = new Voices.Event
			{
				type = type,
				id = event_name,
				row = row,
				def = field,
				description = description,
				default_condition = default_condition
			};
			if (list != null)
			{
				@event.ignore_roles = list;
			}
			dictionary.Add(event_name, @event);
			Profile.EndSection("Add Voice Event");
		}
		return @event;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000D2DB4 File Offset: 0x000D0FB4
	private static DT.Field ResolveCondition(string path, int row, string condition_key, DT.Field conditions_field)
	{
		if (string.IsNullOrEmpty(condition_key))
		{
			if (conditions_field == null)
			{
				return null;
			}
			condition_key = "no_condition";
		}
		DT.Field field = (conditions_field != null) ? conditions_field.FindChild(condition_key, null, true, true, true, '.') : null;
		if (field == null)
		{
			AudioLog.Error(string.Format("{0}({1}): Unknown condition: '{2}'", path, row + 1, condition_key));
		}
		return field;
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000D2E04 File Offset: 0x000D1004
	private static int GetVariant(Voices.Event evt, string condition_key)
	{
		if (evt == null)
		{
			return 0;
		}
		if (condition_key == null)
		{
			condition_key = "";
		}
		int num;
		evt.unique_variants.TryGetValue(condition_key, out num);
		num++;
		evt.unique_variants[condition_key] = num;
		return num;
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000D2E44 File Offset: 0x000D1044
	private static string MakeID(string event_name, string condition_key, string role, int variant)
	{
		string text = role + "_" + event_name;
		if (!string.IsNullOrEmpty(condition_key))
		{
			text = text + "_" + condition_key;
		}
		if (variant >= 0)
		{
			text += string.Format("_{0:D2}", variant);
		}
		return text;
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000D2E90 File Offset: 0x000D1090
	public static Voices.Manifest ScanDirectory(string path)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return null;
		}
		Voices.Manifest manifest = new Voices.Manifest();
		manifest.root = Voices.ScanDirectory(manifest, path, directoryInfo, null);
		return manifest;
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000D2EC4 File Offset: 0x000D10C4
	private static Voices.Manifest.DirInfo ScanDirectory(Voices.Manifest manifest, string path, DirectoryInfo di, Voices.Manifest.DirInfo parent)
	{
		Voices.Manifest.DirInfo dirInfo = Voices.AddDir(manifest, di.Name, parent);
		if (parent == null)
		{
			dirInfo.path = path;
		}
		foreach (FileInfo fi in di.GetFiles())
		{
			Voices.AddFile(manifest, dirInfo, fi);
		}
		foreach (DirectoryInfo di2 in di.GetDirectories())
		{
			Voices.ScanDirectory(manifest, dirInfo.path, di2, dirInfo);
		}
		return dirInfo;
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x000D2F40 File Offset: 0x000D1140
	private static Voices.Manifest.FileInfo AddFile(Voices.Manifest manifest, Voices.Manifest.DirInfo dir, FileInfo fi)
	{
		string extension = fi.Extension;
		if (extension != ".wav" && extension != ".ogg" && extension != ".mp3")
		{
			AudioLog.Warning(string.Concat(new string[]
			{
				"Unknown file extension '",
				extension,
				"': ",
				dir.path,
				"/",
				fi.Name
			}));
			return null;
		}
		return Voices.AddFile(manifest, dir, fi.Name);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x000D2FC8 File Offset: 0x000D11C8
	private static Voices.Manifest.DirInfo AddDir(Voices.Manifest manifest, string name, Voices.Manifest.DirInfo parent)
	{
		Voices.Manifest.DirInfo dirInfo = new Voices.Manifest.DirInfo();
		dirInfo.name = name.ToLowerInvariant();
		if (parent != null)
		{
			dirInfo.path = parent.path + "/" + name;
		}
		else
		{
			dirInfo.path = name;
		}
		dirInfo.parent = parent;
		if (parent != null)
		{
			parent.subdirs.Add(dirInfo);
		}
		return dirInfo;
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000D3024 File Offset: 0x000D1224
	private static Voices.Manifest.FileInfo AddFile(Voices.Manifest manifest, Voices.Manifest.DirInfo dir, string name)
	{
		Voices.Manifest.FileInfo fileInfo = new Voices.Manifest.FileInfo();
		fileInfo.name = Path.GetFileNameWithoutExtension(name).ToLowerInvariant();
		fileInfo.path = dir.path + "/" + name;
		fileInfo.directory = dir;
		Voices.ParseFileName(fileInfo);
		if (fileInfo.variant <= 0)
		{
			fileInfo.variant = 1;
		}
		if (fileInfo.actor_id > manifest.max_actor_id)
		{
			manifest.max_actor_id = fileInfo.actor_id;
		}
		dir.files.Add(fileInfo);
		Voices.Manifest.FileInfo fileInfo2;
		if (!manifest.files.TryGetValue(fileInfo.name, out fileInfo2))
		{
			manifest.files.Add(fileInfo.name, fileInfo);
		}
		return fileInfo;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000D30CC File Offset: 0x000D12CC
	private static int ReadInt(string s, ref int idx)
	{
		int num = 0;
		while (idx < s.Length)
		{
			char c = s[idx];
			if (c < '0' || c > '9')
			{
				break;
			}
			num = 10 * num + (int)c - 48;
			idx++;
		}
		return num;
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000D310C File Offset: 0x000D130C
	private static string Trim(string s)
	{
		int i;
		for (i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (c != ' ' && c != '_')
			{
				break;
			}
		}
		int j;
		for (j = s.Length; j > i; j--)
		{
			char c2 = s[j - 1];
			if (c2 != ' ' && c2 != '_')
			{
				break;
			}
		}
		return s.Substring(i, j - i);
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000D316C File Offset: 0x000D136C
	private static bool IsNarrator(Voices.Manifest.FileInfo file)
	{
		for (Voices.Manifest.DirInfo dirInfo = file.directory; dirInfo != null; dirInfo = dirInfo.parent)
		{
			if (dirInfo.name == "narrator")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000D31A4 File Offset: 0x000D13A4
	private static bool IsUnit(Voices.Manifest.FileInfo file)
	{
		for (Voices.Manifest.DirInfo dirInfo = file.directory; dirInfo != null; dirInfo = dirInfo.parent)
		{
			if (dirInfo.name == "units")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x000D31DC File Offset: 0x000D13DC
	private static void ParseFileName(Voices.Manifest.FileInfo file)
	{
		int i = file.name.Length - 1;
		while (i >= 0)
		{
			char c = file.name[i];
			if (c < '0' || c > '9')
			{
				i++;
				break;
			}
			i--;
			if (i == 0)
			{
				break;
			}
		}
		if (i < file.name.Length)
		{
			int num = i;
			file.variant = Voices.ReadInt(file.name, ref num);
		}
		int j;
		for (j = 0; j < file.name.Length; j++)
		{
			char c2 = file.name[j];
			if (c2 >= '0' && c2 <= '9')
			{
				break;
			}
		}
		int num2 = j;
		if (j >= i)
		{
			j = 0;
			num2 = 0;
			if (Voices.IsNarrator(file))
			{
				file.actor_id = 0;
			}
			else if (!int.TryParse(file.directory.name, out file.actor_id))
			{
				file.actor_id = -1;
			}
		}
		else
		{
			file.actor_id = Voices.ReadInt(file.name, ref num2);
		}
		file.role_event_condition = Voices.Trim(file.name.Substring(num2, i - num2));
		if (j > 0)
		{
			string str = Voices.Trim(file.name.Substring(0, j));
			file.role_event_condition = str + "_" + file.role_event_condition;
		}
		file.role_event_condition = file.role_event_condition.Replace(' ', '_');
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000D3328 File Offset: 0x000D1528
	public static Voices.Manifest LoadManifest(string name)
	{
		string text = Voices.ManifestsDir + "/" + name + ".manifest";
		string[] array;
		try
		{
			array = File.ReadAllLines(text);
		}
		catch
		{
			AudioLog.Error("Voices manifest not found: " + text);
			return null;
		}
		Voices.Manifest manifest = new Voices.Manifest();
		manifest.root = new Voices.Manifest.DirInfo
		{
			name = name,
			path = name
		};
		Voices.Manifest.DirInfo dirInfo = manifest.root;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			int j = 0;
			while (j < text2.Length && text2[j] == '\t')
			{
				j++;
			}
			if (j >= text2.Length)
			{
				AudioLog.Error(string.Format("{0}:{1}: Empty line", text, i));
			}
			else
			{
				bool flag;
				string name2;
				if (text2[text2.Length - 1] == '/')
				{
					flag = true;
					name2 = text2.Substring(j, text2.Length - j - 1);
				}
				else
				{
					flag = false;
					name2 = text2.Substring(j);
				}
				while (j < num)
				{
					dirInfo = ((dirInfo != null) ? dirInfo.parent : null);
					num--;
				}
				if (j != num)
				{
					AudioLog.Error(string.Format("{0}:{1}: Wrong identation: {2} instead of {3}", new object[]
					{
						text,
						i,
						j,
						num
					}));
				}
				if (flag)
				{
					dirInfo = Voices.AddDir(manifest, name2, dirInfo);
					num++;
				}
				else
				{
					Voices.AddFile(manifest, dirInfo, name2);
				}
			}
		}
		return manifest;
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000D34C8 File Offset: 0x000D16C8
	public static void ExportVOTexts()
	{
		global::Defs defs = global::Defs.Get(false);
		Voices.<>c__DisplayClass99_0 CS$<>8__locals1;
		CS$<>8__locals1.sb = new StringBuilder();
		CS$<>8__locals1.sb.Append("\"id\";\"comment\";\"en\"");
		foreach (DT.Field field in defs.languages)
		{
			string key = field.key;
			if (!global::Defs.IsDevLanguage(key) && !(key == "en") && field.GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
			{
				CS$<>8__locals1.sb.Append(";\"" + key + "\"");
			}
		}
		CS$<>8__locals1.sb.AppendLine();
		int num = 0;
		foreach (Voices.VoiceLine voiceLine in Voices.voice_lines_in_order)
		{
			if (!voiceLine.is_default_text && voiceLine.refers_to == null)
			{
				num++;
				string text = "voice_" + voiceLine.id;
				string comment = voiceLine.comment;
				string text2 = (comment != null) ? comment.Replace("\"", "\"\"") : null;
				CS$<>8__locals1.sb.Append(string.Concat(new string[]
				{
					"\"",
					text,
					"\";\"",
					text2,
					"\""
				}));
				global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text, false, true);
				Voices.<ExportVOTexts>g__AddText|99_0(voiceLine, textInfo, "en", true, ref CS$<>8__locals1);
				foreach (DT.Field field2 in defs.languages)
				{
					string key2 = field2.key;
					if (!global::Defs.IsDevLanguage(key2) && !(key2 == "en") && field2.GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
					{
						Voices.<ExportVOTexts>g__AddText|99_0(voiceLine, textInfo, key2, false, ref CS$<>8__locals1);
					}
				}
				CS$<>8__locals1.sb.AppendLine();
			}
		}
		string contents = CS$<>8__locals1.sb.ToString();
		string text3 = "Assets/texts/VO.csv";
		File.WriteAllText(text3, contents, Encoding.UTF8);
		Debug.Log(string.Format("Exported {0} VO texts to {1}", num, text3));
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000D3770 File Offset: 0x000D1970
	public static void ExportRoleToDo(Voices.Role role)
	{
		if (role.id == "default")
		{
			return;
		}
		new List<int>();
		StringBuilder stringBuilder = new StringBuilder(16384);
		stringBuilder.AppendLine("File Name,Text,Comments");
		stringBuilder.AppendLine(",,\"" + role.description + "\"");
		stringBuilder.AppendLine();
		string b = null;
		string b2 = null;
		for (int i = 0; i < role.voice_lines.Count; i++)
		{
			Voices.VoiceLine voiceLine = role.voice_lines[i];
			if (voiceLine.refers_to == null)
			{
				if (voiceLine.event_name != b)
				{
					b = voiceLine.event_name;
					b2 = null;
					stringBuilder.Append(",,\"EVENT (" + voiceLine.event_name + ")");
					if (!string.IsNullOrEmpty(voiceLine.evt.description))
					{
						stringBuilder.Append(": " + voiceLine.evt.description);
					}
					stringBuilder.AppendLine("\"");
				}
				if (voiceLine.evt.has_conditions && voiceLine.condition_field != null && voiceLine.condition_field.key != b2)
				{
					b2 = voiceLine.condition_field.key;
					stringBuilder.Append(",,\"Condition (" + voiceLine.condition_field.key + ")");
					string @string = voiceLine.condition_field.GetString("description", null, "", true, true, true, '.');
					if (!string.IsNullOrEmpty(@string))
					{
						stringBuilder.Append(": " + @string);
					}
					stringBuilder.AppendLine("\"");
				}
				if (!string.IsNullOrEmpty(voiceLine.comment))
				{
					stringBuilder.AppendLine(",,\"" + voiceLine.comment + "\"");
				}
				string str;
				switch (role.type)
				{
				case Voices.VoiceLine.Type.Narrator:
					str = "00_";
					break;
				case Voices.VoiceLine.Type.Character:
					str = "??_";
					break;
				case Voices.VoiceLine.Type.Unit:
					str = "";
					break;
				default:
					Debug.LogError(string.Format("Unknown role type: {0}", role.type));
					str = "??_";
					break;
				}
				string text = str + voiceLine.id;
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"\"",
					text,
					"\",\"",
					voiceLine.localized_text,
					"\""
				}));
			}
		}
		string contents = stringBuilder.ToString();
		string text2 = string.Format("Design Docs/Misc/Audio/Actors ToDo/{0}/Per Role/{1}", Voices.Language, role.type);
		Directory.CreateDirectory(text2);
		File.WriteAllText(text2 + "/" + role.id + ".csv", contents, Encoding.UTF8);
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000D3A3C File Offset: 0x000D1C3C
	public static void ExportRolesToDo()
	{
		for (int i = 0; i < 3; i++)
		{
			foreach (KeyValuePair<string, Voices.Role> keyValuePair in Voices.roles[i])
			{
				Voices.ExportRoleToDo(keyValuePair.Value);
			}
		}
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000D3AA4 File Offset: 0x000D1CA4
	public static void ExportDefaultTextUnitVoiceLines()
	{
		global::Defs defs = global::Defs.Get(false);
		Voices.<>c__DisplayClass102_0 CS$<>8__locals1;
		CS$<>8__locals1.sb = new StringBuilder();
		CS$<>8__locals1.sb.Append("\"id\";\"comment\";\"en\"");
		foreach (DT.Field field in defs.languages)
		{
			string key = field.key;
			if (!global::Defs.IsDevLanguage(key) && !(key == "en") && field.GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
			{
				CS$<>8__locals1.sb.Append(";\"" + key + "\"");
			}
		}
		CS$<>8__locals1.sb.AppendLine();
		int num = 0;
		foreach (KeyValuePair<string, Voices.Role> keyValuePair in Voices.roles[2])
		{
			Voices.Role value = keyValuePair.Value;
			CS$<>8__locals1.sb.AppendLine();
			CS$<>8__locals1.sb.AppendLine("\"" + value.id + "/\"");
			CS$<>8__locals1.sb.AppendLine();
			for (int i = 0; i < value.voice_lines.Count; i++)
			{
				Voices.VoiceLine voiceLine = value.voice_lines[i];
				if (voiceLine.refers_to == null && voiceLine.is_default_text)
				{
					num++;
					string comment = voiceLine.comment;
					string text = (comment != null) ? comment.Replace("\"", "\"\"") : null;
					CS$<>8__locals1.sb.Append(string.Concat(new string[]
					{
						"\"",
						voiceLine.id,
						"\";\"",
						text,
						"\""
					}));
					global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(string.Format("voice_default_{0}_{1:D2}", voiceLine.event_name, voiceLine.variant), false, true);
					Voices.<ExportDefaultTextUnitVoiceLines>g__AddText|102_0(voiceLine, textInfo, "en", true, ref CS$<>8__locals1);
					foreach (DT.Field field2 in defs.languages)
					{
						string key2 = field2.key;
						if (!global::Defs.IsDevLanguage(key2) && !(key2 == "en") && field2.GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
						{
							Voices.<ExportDefaultTextUnitVoiceLines>g__AddText|102_0(voiceLine, textInfo, key2, false, ref CS$<>8__locals1);
						}
					}
					CS$<>8__locals1.sb.AppendLine();
				}
			}
		}
		string contents = CS$<>8__locals1.sb.ToString();
		string text2 = "Design Docs/Misc/Audio/Actors ToDo/DefaultTextUnitVO.csv";
		File.WriteAllText(text2, contents, Encoding.UTF8);
		Debug.Log(string.Format("Exported {0} default-text VO lines to {1}", num, text2));
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000D3DD0 File Offset: 0x000D1FD0
	public static Voices.Manifest ExportManifest(string src_path, string name)
	{
		Voices.Manifest manifest = Voices.ScanDirectory(src_path);
		if (manifest == null)
		{
			AudioLog.Error("Directory not found: " + src_path);
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < manifest.root.subdirs.Count; i++)
		{
			Voices.ExportManifestDir(manifest.root.subdirs[i], stringBuilder, "");
		}
		string contents = stringBuilder.ToString();
		File.WriteAllText(Voices.ManifestsDir + "/" + name + ".manifest", contents);
		return manifest;
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000D3E5C File Offset: 0x000D205C
	private static void ExportManifestDir(Voices.Manifest.DirInfo dir, StringBuilder sb, string ident)
	{
		sb.AppendLine(ident + dir.name + "/");
		string text = ident + "\t";
		for (int i = 0; i < dir.subdirs.Count; i++)
		{
			Voices.ExportManifestDir(dir.subdirs[i], sb, text);
		}
		for (int j = 0; j < dir.files.Count; j++)
		{
			Voices.Manifest.FileInfo fileInfo = dir.files[j];
			sb.AppendLine(text + fileInfo.name);
		}
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000D3EEC File Offset: 0x000D20EC
	private static Dictionary<string, Voices.RECInfo> BuildRecInfo(Voices.Manifest manifest)
	{
		if (manifest == null)
		{
			return null;
		}
		Dictionary<string, Voices.RECInfo> dictionary = new Dictionary<string, Voices.RECInfo>();
		foreach (KeyValuePair<string, Voices.Manifest.FileInfo> keyValuePair in manifest.files)
		{
			Voices.Manifest.FileInfo value = keyValuePair.Value;
			Voices.RECInfo recinfo;
			if (!dictionary.TryGetValue(value.role_event_condition, out recinfo))
			{
				recinfo = new Voices.RECInfo
				{
					key = value.role_event_condition
				};
				dictionary.Add(value.role_event_condition, recinfo);
			}
			Container.AddUnique<int>(ref recinfo.spoken_by, value.actor_id, 0);
			recinfo.files.Add(value);
			if (value.variant > recinfo.max_variant)
			{
				recinfo.max_variant = value.variant;
			}
		}
		return dictionary;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000D3FC0 File Offset: 0x000D21C0
	private static void DecideVariants(Voices.Event evt, Voices.Role role, Voices.RECInfo rec)
	{
		for (int i = 0; i < evt.voice_lines.Count; i++)
		{
			Voices.VoiceLine voiceLine = evt.voice_lines[i];
			if (voiceLine.role == role && voiceLine.old_variant == 0)
			{
				voiceLine.old_variant = -1;
			}
		}
		int j = 0;
		if (rec.max_variant <= j)
		{
			return;
		}
		for (int k = 0; k < evt.voice_lines.Count; k++)
		{
			Voices.VoiceLine voiceLine2 = evt.voice_lines[k];
			if (voiceLine2.role == role && voiceLine2.old_variant <= 0 && string.IsNullOrEmpty(voiceLine2.condition_key))
			{
				j++;
				voiceLine2.old_variant = j;
				if (j >= rec.max_variant)
				{
					return;
				}
			}
		}
		while (j < rec.max_variant)
		{
			bool flag = false;
			foreach (KeyValuePair<string, int> keyValuePair in evt.unique_variants)
			{
				string key = keyValuePair.Key;
				if (!string.IsNullOrEmpty(key))
				{
					int l = 0;
					while (l < evt.voice_lines.Count)
					{
						Voices.VoiceLine voiceLine3 = evt.voice_lines[l];
						if (voiceLine3.role == role && voiceLine3.old_variant <= 0 && !(voiceLine3.condition_key != key))
						{
							flag = true;
							j++;
							voiceLine3.old_variant = j;
							if (j >= rec.max_variant)
							{
								return;
							}
							break;
						}
						else
						{
							l++;
						}
					}
				}
			}
			if (!flag)
			{
				break;
			}
		}
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000D4148 File Offset: 0x000D2348
	private static void Copy(string src_path, string tgt_dir, string tgt_name)
	{
		string text = tgt_dir + "/" + tgt_name + ".wav";
		if (File.Exists(text))
		{
			return;
		}
		Voices.sb_copy.AppendLine(src_path + " -> " + text);
		Voices.copied++;
		if (Voices.simulate_copy)
		{
			return;
		}
		Directory.CreateDirectory(tgt_dir);
		File.Copy(src_path, text, true);
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x000D41AC File Offset: 0x000D23AC
	private static int ExportOldAudioFiles(Voices.VoiceLine voice_line, Voices.RECInfo rec)
	{
		if (voice_line.old_variant == 0)
		{
			Voices.DecideVariants(voice_line.evt, voice_line.role, rec);
		}
		if (voice_line.old_variant <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < rec.files.Count; i++)
		{
			Voices.Manifest.FileInfo fileInfo = rec.files[i];
			if (fileInfo.variant == voice_line.old_variant)
			{
				int num2 = fileInfo.actor_id;
				if (num2 < 0)
				{
					if (voice_line.type != Voices.VoiceLine.Type.Unit)
					{
						goto IL_BE;
					}
					num2 = Voices.CalcUnitActorID(voice_line.role_name, 15);
				}
				num++;
				string tgt_dir = (num2 >= 0) ? string.Format("{0}/dev/{1:D2}", Voices.SrcFilesDir, num2) : (Voices.SrcFilesDir + "/dev/" + voice_line.role_name);
				string tgt_name = Voices.VoiceLineSoundID(num2, voice_line.id);
				Voices.Copy(fileInfo.path, tgt_dir, tgt_name);
			}
			IL_BE:;
		}
		rec.exported = true;
		return num;
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x000D4294 File Offset: 0x000D2494
	private static int ExportOldNarratorAudioFile(Voices.VoiceLine voice_line, Voices.Manifest.FileInfo file)
	{
		string tgt_dir = Voices.SrcFilesDir + "/dev/00/";
		string tgt_name = Voices.VoiceLineSoundID(0, voice_line.id);
		Voices.Copy(file.path, tgt_dir, tgt_name);
		return 1;
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x000D42CC File Offset: 0x000D24CC
	public static void ExportOldVoices(bool simulated)
	{
		Voices.sb_copy.Clear();
		Voices.simulate_copy = simulated;
		Voices.copied = 0;
		Voices.Manifest manifest = Voices.ExportManifest(Voices.OldFilesDir, "old");
		Dictionary<string, Voices.RECInfo> dictionary = Voices.BuildRecInfo(manifest);
		int num = 0;
		int num2 = 0;
		DT.Field defField = global::Defs.GetDefField("Voices", "roles.Character");
		DT.Field defField2 = global::Defs.GetDefField("Voices", "events.Character");
		foreach (Voices.VoiceLine voiceLine in Voices.voice_lines_in_order)
		{
			if (voiceLine.old_audio_file != null && voiceLine.type == Voices.VoiceLine.Type.Narrator)
			{
				Voices.Manifest.FileInfo file;
				if (manifest.files.TryGetValue(voiceLine.old_audio_file, out file))
				{
					num += Voices.ExportOldNarratorAudioFile(voiceLine, file);
				}
				else
				{
					AudioLog.Error(string.Format("Old audio file '{0}' not found: {1}", voiceLine.old_audio_file, voiceLine));
				}
			}
			else
			{
				string str = voiceLine.role_name;
				if (!string.IsNullOrEmpty(voiceLine.role.old_role))
				{
					str = voiceLine.role.old_role;
				}
				if (voiceLine.type == Voices.VoiceLine.Type.Character)
				{
					DT.Field field = (defField != null) ? defField.FindChild(voiceLine.role_name + ".old_role", null, true, true, true, '.') : null;
					if (field != null)
					{
						str = field.String(null, "");
						DT.Field field2 = field.FindChild(voiceLine.event_name, null, true, true, true, '.');
						if (field2 != null)
						{
							str = field2.String(null, "");
						}
					}
				}
				string str2 = voiceLine.event_name;
				if (voiceLine.type == Voices.VoiceLine.Type.Character)
				{
					DT.Field field3 = (defField2 != null) ? defField2.FindChild(voiceLine.event_name + ".old_event", null, true, true, true, '.') : null;
					if (field3 != null)
					{
						str2 = field3.String(null, "");
						DT.Field field4 = field3.FindChild(voiceLine.role_name, null, true, true, true, '.');
						if (field4 != null)
						{
							str2 = field4.String(null, "");
						}
					}
				}
				string key = (str + "_" + str2).Replace(' ', '_');
				Voices.RECInfo rec;
				if (dictionary.TryGetValue(key, out rec))
				{
					num2 += Voices.ExportOldAudioFiles(voiceLine, rec);
				}
			}
		}
		AudioLog.Info(string.Format("Exported files: {0} ({1} manual + {2} auto), {3} new", new object[]
		{
			num + num2,
			num,
			num2,
			Voices.copied
		}));
		File.WriteAllText(Voices.ManifestsDir + "/copied.log", Voices.sb_copy.ToString());
		StringBuilder stringBuilder = new StringBuilder();
		int num3 = 0;
		foreach (KeyValuePair<string, Voices.RECInfo> keyValuePair in dictionary)
		{
			Voices.RECInfo value = keyValuePair.Value;
			if (!value.exported)
			{
				num3++;
				stringBuilder.AppendLine(value.ToString());
			}
		}
		if (num3 > 0)
		{
			AudioLog.Warning(string.Format("Not exported: {0}\n", num3));
			File.WriteAllText(Voices.ManifestsDir + "/not copied.log", stringBuilder.ToString());
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x000D461C File Offset: 0x000D281C
	public static void ExportManifest(bool all_languages)
	{
		global::Defs defs = global::Defs.Get(false);
		for (int i = 0; i < defs.languages.Count; i++)
		{
			DT.Field field = defs.languages[i];
			if (all_languages || field.key == Voices.Language)
			{
				Voices.ExportManifest(Voices.SrcFilesDir + "/" + field.key, field.key);
			}
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000D468C File Offset: 0x000D288C
	private static Voices.Manifest LoadSpokenByFromManifest(out int num_missing_lines, StringBuilder sb_missing_lines = null)
	{
		num_missing_lines = 0;
		Voices.Manifest manifest = Voices.LoadManifest(Voices.Language);
		if (manifest == null)
		{
			AudioLog.Error(Voices.Language + ".manifest not found");
			return null;
		}
		for (Voices.VoiceLine.Type type = Voices.VoiceLine.Type.Narrator; type < Voices.VoiceLine.Type.COUNT; type++)
		{
			Dictionary<string, Voices.Role> dictionary = Voices.roles[(int)type];
			if (dictionary == null)
			{
				AudioLog.Error(string.Format("No roles for {0} voices", type));
			}
			else
			{
				foreach (KeyValuePair<string, Voices.Role> keyValuePair in dictionary)
				{
					Voices.Role value = keyValuePair.Value;
					value.spoken_by.Clear();
					for (int i = 0; i < value.voice_lines.Count; i++)
					{
						value.voice_lines[i].spoken_by = new List<int>();
					}
					if (!(value.id == "default"))
					{
						for (int j = 0; j <= manifest.max_actor_id; j++)
						{
							int num = 0;
							for (int k = 0; k < value.voice_lines.Count; k++)
							{
								Voices.VoiceLine voiceLine = value.voice_lines[k];
								string key = Voices.VoiceLineSoundID(j, voiceLine.id);
								Voices.Manifest.FileInfo fileInfo;
								if (manifest.files.TryGetValue(key, out fileInfo))
								{
									fileInfo.used = true;
									num++;
									voiceLine.spoken_by.Add(j);
								}
							}
							if (num != 0)
							{
								value.spoken_by.Add(j);
								if (num != value.voice_lines.Count)
								{
									for (int l = 0; l < value.voice_lines.Count; l++)
									{
										Voices.VoiceLine voiceLine2 = value.voice_lines[l];
										if (!voiceLine2.spoken_by.Contains(j))
										{
											num_missing_lines++;
											if (sb_missing_lines != null)
											{
												string arg = Voices.VoiceLineSoundID(j, voiceLine2.id);
												sb_missing_lines.AppendLine(string.Format("{0:D2}/{1}", j, arg));
												sb_missing_lines.AppendLine(voiceLine2.dev_text);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return manifest;
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000D48CC File Offset: 0x000D2ACC
	public static void ExportActorsDef()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num;
		Voices.Manifest manifest = Voices.LoadSpokenByFromManifest(out num, stringBuilder);
		if (num > 0)
		{
			AudioLog.Error(string.Format("Missing voice lines: {0}, check Tools/TTSExporter/TTS.txt", num));
		}
		stringBuilder.Clear();
		int num2 = 0;
		foreach (KeyValuePair<string, Voices.Manifest.FileInfo> keyValuePair in manifest.files)
		{
			Voices.Manifest.FileInfo value = keyValuePair.Value;
			if (!value.used)
			{
				num2++;
				stringBuilder.AppendLine(value.path);
			}
		}
		string text;
		string contents;
		if (num2 > 0)
		{
			text = Voices.ManifestsDir + "/unused.txt";
			contents = stringBuilder.ToString();
			File.WriteAllText(text, contents, Encoding.UTF8);
			AudioLog.Error(string.Format("Unused voice files: {0}, check {1}", num2, text));
		}
		stringBuilder.Clear();
		stringBuilder.AppendLine(string.Format("actors = {0}", manifest.max_actor_id));
		stringBuilder.AppendLine("{");
		for (int i = 0; i < manifest.max_actor_id; i++)
		{
			Voices.Actor actor = (i < Voices.actors.Count) ? Voices.actors[i] : null;
			stringBuilder.AppendLine(string.Format("\t{0:D2}", i + 1));
			stringBuilder.AppendLine("\t{");
			stringBuilder.AppendLine("\t\tname = \"" + ((actor != null) ? actor.name : null) + "\"");
			stringBuilder.AppendLine("\t\tethnicity = \"" + ((actor != null) ? actor.ethnicity : null) + "\"");
			stringBuilder.AppendLine("\t}");
		}
		stringBuilder.AppendLine("}");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("unit_roles_spoken_by");
		stringBuilder.AppendLine("{");
		foreach (KeyValuePair<string, Voices.Role> keyValuePair2 in Voices.roles[2])
		{
			Voices.Role value2 = keyValuePair2.Value;
			if (!(value2.id == "default"))
			{
				stringBuilder.Append("\t" + value2.id + " = ");
				if (value2.spoken_by.Count > 0)
				{
					for (int j = 0; j < value2.spoken_by.Count; j++)
					{
						int num3 = value2.spoken_by[j];
						if (j > 0)
						{
							stringBuilder.Append(" / ");
						}
						stringBuilder.Append(string.Format("{0:D2}", num3));
					}
				}
				else
				{
					int num4 = Voices.CalcUnitActorID(value2.id, Voices.max_actor_id);
					stringBuilder.Append(string.Format("{0:D2} //!!! randomly assigned", num4));
				}
				stringBuilder.AppendLine();
			}
		}
		stringBuilder.AppendLine("}");
		text = Voices.ManifestsDir + "/" + Voices.Language + "_actors.def";
		contents = stringBuilder.ToString();
		File.WriteAllText(text, contents, Encoding.UTF8);
		AudioLog.Info(string.Format("Exported {0}, actors: {1}", text, manifest.max_actor_id));
		Voices.LoadDefs();
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x000D4C34 File Offset: 0x000D2E34
	public static void ExportTTSData()
	{
		List<int> list = new List<int>();
		StringBuilder stringBuilder = new StringBuilder(16384);
		foreach (Voices.VoiceLine voiceLine in Voices.voice_lines_in_order)
		{
			if (!(voiceLine.role_name == "default"))
			{
				if (voiceLine.type == Voices.VoiceLine.Type.Unit)
				{
					stringBuilder.AppendLine(voiceLine.role_name + "/" + voiceLine.id);
					stringBuilder.AppendLine(voiceLine.dev_text);
				}
				else
				{
					list.Clear();
					Voices.CalcSpokenBy(voiceLine.type, voiceLine.role_name, list);
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i];
						if (num >= 0)
						{
							stringBuilder.AppendLine(string.Format("{0:D2}/{1:D2}_{2}", num, num, voiceLine.id));
							stringBuilder.AppendLine(voiceLine.dev_text);
						}
					}
				}
			}
		}
		string contents = stringBuilder.ToString();
		File.WriteAllText("Tools/TTSExporter/TTS.txt", contents, Encoding.UTF8);
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x000D4D70 File Offset: 0x000D2F70
	public static Dictionary<string, HashSet<string>> GetVoiceLineFiles(bool ignoreVLWithMissingDefaultText, string language)
	{
		Voices.LoadActorsDef(language.ToLowerInvariant());
		List<int> list = new List<int>();
		StringBuilder stringBuilder = new StringBuilder(16384);
		foreach (Voices.VoiceLine voiceLine in Voices.voice_lines_in_order)
		{
			if (voiceLine.refers_to == null && !(voiceLine.role_name == "default") && (!voiceLine.is_default_text || !ignoreVLWithMissingDefaultText))
			{
				if (voiceLine.type == Voices.VoiceLine.Type.Unit)
				{
					stringBuilder.AppendLine(voiceLine.role_name + "/" + voiceLine.id);
				}
				else
				{
					list.Clear();
					Voices.CalcSpokenBy(voiceLine.type, voiceLine.role_name, list);
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i];
						if (num >= 0)
						{
							stringBuilder.AppendLine(string.Format("{0:D2}/{1:D2}_{2}", num, num, voiceLine.id));
						}
					}
				}
			}
		}
		Voices.LoadActorsDef(Voices.Language);
		string[] array = stringBuilder.ToString().Split(new string[]
		{
			Environment.NewLine
		}, StringSplitOptions.None);
		Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
		for (int j = 0; j < array.Length; j++)
		{
			string[] array2 = array[j].Split(new char[]
			{
				'/'
			});
			if (array2.Length == 2)
			{
				if (dictionary.ContainsKey(array2[0]))
				{
					dictionary[array2[0]].Add(array2[1] + ".wav");
				}
				else
				{
					dictionary[array2[0]] = new HashSet<string>
					{
						array2[1] + ".wav"
					};
				}
			}
		}
		return dictionary;
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x000D4F4C File Offset: 0x000D314C
	public static string DetectMisnamedVoiceFiles(Dictionary<string, HashSet<string>> voiceFilesByDirectory, string voicesRootDir, string language)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(voicesRootDir, language));
		if (!directoryInfo.Exists)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("### Misnamed voice files ###");
		stringBuilder.AppendLine("Language: " + language);
		stringBuilder.AppendLine();
		foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
		{
			bool flag = false;
			HashSet<string> hashSet;
			if (voiceFilesByDirectory.TryGetValue(directoryInfo2.Name, out hashSet))
			{
				foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
				{
					if (!hashSet.Contains(fileInfo.Name))
					{
						if (!flag)
						{
							stringBuilder.AppendLine(directoryInfo2.Name + "/");
							flag = true;
						}
						stringBuilder.AppendLine("\t" + fileInfo.Name);
					}
				}
				foreach (DirectoryInfo directoryInfo3 in directoryInfo2.GetDirectories())
				{
					if (!flag)
					{
						stringBuilder.AppendLine(directoryInfo2.Name + "/");
						flag = true;
					}
					stringBuilder.AppendLine("\t" + directoryInfo3.Name + "/");
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x000D50A8 File Offset: 0x000D32A8
	public static string DetectMissingVoiceFiles(Dictionary<string, HashSet<string>> voiceFilesByDirectory, string voicesRootDir, string language, bool ignoreMissingDefaultText)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(voicesRootDir, language));
		if (!directoryInfo.Exists)
		{
			return "";
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			string name = directoryInfo2.Name;
			dictionary.Add(name, new HashSet<string>());
			FileInfo[] files = directoryInfo2.GetFiles();
			for (int j = 0; j < files.Length; j++)
			{
				dictionary[name].Add(files[j].Name);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("### Missing voice files ###");
		stringBuilder.AppendLine("Language: " + language);
		stringBuilder.AppendLine("Ignore missing voice lines with default text: " + (ignoreMissingDefaultText ? "yes" : "no"));
		stringBuilder.AppendLine();
		foreach (KeyValuePair<string, HashSet<string>> keyValuePair in voiceFilesByDirectory)
		{
			bool flag = dictionary.ContainsKey(keyValuePair.Key);
			if (!flag)
			{
				stringBuilder.AppendLine(keyValuePair.Key + "/");
				using (HashSet<string>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string str = enumerator2.Current;
						stringBuilder.AppendLine("\t" + str);
					}
					continue;
				}
			}
			HashSet<string> hashSet = flag ? dictionary[keyValuePair.Key] : null;
			List<string> list = new List<string>();
			foreach (string item in keyValuePair.Value)
			{
				if (!hashSet.Contains(item))
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				list.Sort();
				stringBuilder.AppendLine(keyValuePair.Key + "/");
				foreach (string str2 in list)
				{
					stringBuilder.AppendLine("\t" + str2);
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000D5360 File Offset: 0x000D3560
	public static string DetectEmptyVoiceFiles(Dictionary<string, HashSet<string>> voiceFilesByDirectory, string voicesRootDir, string language)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(voicesRootDir, language));
		if (!directoryInfo.Exists)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("### Empty voice files ###");
		stringBuilder.AppendLine("Language: " + language);
		stringBuilder.AppendLine();
		foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
		{
			bool flag = false;
			HashSet<string> hashSet;
			if (voiceFilesByDirectory.TryGetValue(directoryInfo2.Name, out hashSet))
			{
				foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
				{
					if (fileInfo.Length == 0L)
					{
						if (!flag)
						{
							stringBuilder.AppendLine(directoryInfo2.Name + "/");
							flag = true;
						}
						stringBuilder.AppendLine("\t" + fileInfo.Name);
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x000D5580 File Offset: 0x000D3780
	[CompilerGenerated]
	internal static void <Load>g__ParseCharacterField|83_0(DT.Field character_field, string language, ref Voices.<>c__DisplayClass83_0 A_2)
	{
		if (character_field == null)
		{
			return;
		}
		int nid;
		if (!DT.ParseInt(character_field.key, out nid))
		{
			return;
		}
		Logic.Character character = new NID(Serialization.ObjectType.Character, nid).GetObj(A_2.game) as Logic.Character;
		if (character == null)
		{
			return;
		}
		int num = character_field.Int(null, -100);
		if (language == Voices.Language)
		{
			character.VO_actor_id = num;
		}
		Voices.RegisterCharacterActorID(character, num, language);
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x000D55E8 File Offset: 0x000D37E8
	[CompilerGenerated]
	internal static void <ExportVOTexts>g__AddText|99_0(Voices.VoiceLine voice_line, global::Defs.TextInfo ti, string lang_key, bool fall_back_to_dev, ref Voices.<>c__DisplayClass99_0 A_4)
	{
		string text = null;
		if (ti != null)
		{
			DT.Field localized_field = ti.GetLanguageInfo(lang_key, false).localized_field;
			text = ((localized_field != null) ? localized_field.String(null, "") : null);
		}
		if (string.IsNullOrEmpty(text) && fall_back_to_dev)
		{
			text = voice_line.dev_text;
		}
		if (text == null)
		{
			text = "";
		}
		text = text.Replace("\"", "\"\"");
		A_4.sb.Append(";\"" + text + "\"");
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x000D5664 File Offset: 0x000D3864
	[CompilerGenerated]
	internal static void <ExportDefaultTextUnitVoiceLines>g__AddText|102_0(Voices.VoiceLine voice_line, global::Defs.TextInfo ti, string lang_key, bool fall_back_to_dev, ref Voices.<>c__DisplayClass102_0 A_4)
	{
		string text = null;
		if (ti != null)
		{
			DT.Field localized_field = ti.GetLanguageInfo(lang_key, false).localized_field;
			text = ((localized_field != null) ? localized_field.String(null, "") : null);
		}
		if (string.IsNullOrEmpty(text) && fall_back_to_dev)
		{
			text = voice_line.dev_text;
		}
		if (text == null)
		{
			text = "";
		}
		text = text.Replace("\"", "\"\"");
		A_4.sb.Append(";\"" + text + "\"");
	}

	// Token: 0x04000D60 RID: 3424
	private static string ManifestsDir = "texts/Voices/Manifests";

	// Token: 0x04000D61 RID: 3425
	private static string OldFilesDir = "FMOD/Assets/Voices";

	// Token: 0x04000D62 RID: 3426
	public static string SrcFilesDir = "E:/BSG/Sound Source Files/voices";

	// Token: 0x04000D63 RID: 3427
	public static int priority_queue_size = 3;

	// Token: 0x04000D64 RID: 3428
	public static int weight_default = 100;

	// Token: 0x04000D65 RID: 3429
	public static int weight_invalid = -1;

	// Token: 0x04000D66 RID: 3430
	public static Dictionary<string, List<string>> suppression_data = new Dictionary<string, List<string>>();

	// Token: 0x04000D67 RID: 3431
	public static Dictionary<string, float> group_cooldowns = new Dictionary<string, float>();

	// Token: 0x04000D68 RID: 3432
	private static DT.Field voices_def_field;

	// Token: 0x04000D69 RID: 3433
	private static DT.Field roles_field;

	// Token: 0x04000D6A RID: 3434
	private static DT.Field[] role_fields = new DT.Field[3];

	// Token: 0x04000D6B RID: 3435
	private static DT.Field conditions_field;

	// Token: 0x04000D6C RID: 3436
	private static DT.Field[] condition_fields = new DT.Field[3];

	// Token: 0x04000D6D RID: 3437
	private static DT.Field events_field;

	// Token: 0x04000D6E RID: 3438
	private static DT.Field[] event_fields = new DT.Field[3];

	// Token: 0x04000D6F RID: 3439
	private static DT.Field types_field;

	// Token: 0x04000D70 RID: 3440
	private static DT.Field[] type_fields = new DT.Field[3];

	// Token: 0x04000D71 RID: 3441
	private static string language = null;

	// Token: 0x04000D72 RID: 3442
	public static List<string>[] BSGToTHQEthnicityMapping = new List<string>[7];

	// Token: 0x04000D73 RID: 3443
	private static Dictionary<string, Voices.AllocatedActorIDCharacters[]> allocated_character_actor_ids = new Dictionary<string, Voices.AllocatedActorIDCharacters[]>();

	// Token: 0x04000D74 RID: 3444
	private static List<int>[] tmp_actor_ids = new List<int>[]
	{
		new List<int>(),
		new List<int>(),
		new List<int>(),
		new List<int>()
	};

	// Token: 0x04000D75 RID: 3445
	private static Dictionary<string, bool> conditions_cache = new Dictionary<string, bool>();

	// Token: 0x04000D76 RID: 3446
	private static List<Voices.VoiceLine> valid_voice_lines = new List<Voices.VoiceLine>();

	// Token: 0x04000D77 RID: 3447
	private static Dictionary<string, Voices.VoiceLine> all_voice_lines = new Dictionary<string, Voices.VoiceLine>();

	// Token: 0x04000D78 RID: 3448
	private static List<Voices.VoiceLine> voice_lines_in_order = new List<Voices.VoiceLine>();

	// Token: 0x04000D79 RID: 3449
	private static Dictionary<string, Voices.Role>[] roles = new Dictionary<string, Voices.Role>[3];

	// Token: 0x04000D7A RID: 3450
	private static Dictionary<string, Dictionary<string, int>>[] unique_event_variants = new Dictionary<string, Dictionary<string, int>>[3];

	// Token: 0x04000D7B RID: 3451
	private static Dictionary<string, Voices.Event>[] events = new Dictionary<string, Voices.Event>[3];

	// Token: 0x04000D7C RID: 3452
	private static int max_actor_id = -1;

	// Token: 0x04000D7D RID: 3453
	private static List<Voices.Actor> actors = new List<Voices.Actor>();

	// Token: 0x04000D7E RID: 3454
	private static StringBuilder sb_copy = new StringBuilder();

	// Token: 0x04000D7F RID: 3455
	private static bool simulate_copy;

	// Token: 0x04000D80 RID: 3456
	private static int copied = 0;

	// Token: 0x020006AE RID: 1710
	public class VoiceLine
	{
		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x0600485D RID: 18525 RVA: 0x0021778B File Offset: 0x0021598B
		// (set) Token: 0x0600485E RID: 18526 RVA: 0x002177A9 File Offset: 0x002159A9
		public List<int> SpokenBy
		{
			get
			{
				Voices.VoiceLine voiceLine = this.refers_to;
				return ((voiceLine != null) ? voiceLine.SpokenBy : null) ?? this.spoken_by;
			}
			set
			{
				this.spoken_by = value;
			}
		}

		// Token: 0x0600485F RID: 18527 RVA: 0x002177B2 File Offset: 0x002159B2
		public int GetCurrentPriority()
		{
			if (this.bv_priority <= 0 || !(BattleMap.Get() != null))
			{
				return this.priority;
			}
			return this.bv_priority;
		}

		// Token: 0x06004860 RID: 18528 RVA: 0x002177D8 File Offset: 0x002159D8
		public DT.Field GetField(string key)
		{
			DT.Field result;
			using (Game.Profile("VoiceLine.GetField", false, 0f, null))
			{
				Voices.Event @event = this.evt;
				if (((@event != null) ? @event.def : null) != null)
				{
					DT.Field field = this.evt.def.FindChild("condition", null, true, true, true, '.');
					DT.Field field2 = (field != null) ? field.FindChild("role", null, true, true, true, '.') : null;
					DT.Field field3 = (field2 != null) ? field2.FindChild("variant", null, true, true, true, '.') : null;
					DT.Field field4 = (field3 != null) ? field3.FindChild(key, null, true, true, true, '.') : null;
					if (field4 != null)
					{
						return field4;
					}
					DT.Field field5 = (field2 != null) ? field2.FindChild(key, null, true, true, true, '.') : null;
					if (field5 != null)
					{
						return field5;
					}
					DT.Field field6 = (field != null) ? field.FindChild(key, null, true, true, true, '.') : null;
					if (field6 != null)
					{
						return field6;
					}
					DT.Field field7 = this.evt.def.FindChild(key, null, true, true, true, '.');
					if (field7 != null)
					{
						return field7;
					}
				}
				Voices.Role role = this.role;
				DT.Field field8;
				if (role == null)
				{
					field8 = null;
				}
				else
				{
					DT.Field def = role.def;
					field8 = ((def != null) ? def.FindChild(key, null, true, true, true, '.') : null);
				}
				DT.Field field9 = field8;
				if (field9 != null)
				{
					result = field9;
				}
				else
				{
					DT.Field field10 = Voices.type_fields[(int)this.evt.type];
					DT.Field field11 = (field10 != null) ? field10.FindChild(key, null, true, true, true, '.') : null;
					if (field11 != null)
					{
						result = field11;
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x06004861 RID: 18529 RVA: 0x00217974 File Offset: 0x00215B74
		public float GetFloat(string key, float def_val = 0f)
		{
			DT.Field field = this.GetField(key);
			if (field == null)
			{
				return def_val;
			}
			return field.Float(null, def_val);
		}

		// Token: 0x06004862 RID: 18530 RVA: 0x00217998 File Offset: 0x00215B98
		public int GetInt(string key, int def_val = 0)
		{
			DT.Field field = this.GetField(key);
			if (field == null)
			{
				return def_val;
			}
			return field.Int(null, def_val);
		}

		// Token: 0x06004863 RID: 18531 RVA: 0x002179BC File Offset: 0x00215BBC
		public string GetString(string key, string def_val = null)
		{
			DT.Field field = this.GetField(key);
			if (field == null)
			{
				return def_val;
			}
			return field.String(null, def_val);
		}

		// Token: 0x06004864 RID: 18532 RVA: 0x002179E0 File Offset: 0x00215BE0
		public bool GetBool(string key, bool def_val = false)
		{
			DT.Field field = this.GetField(key);
			if (field == null)
			{
				return def_val;
			}
			return field.Bool(null, def_val);
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x00217A04 File Offset: 0x00215C04
		public override string ToString()
		{
			int num = (this.SpokenBy == null) ? -1 : this.SpokenBy.Count;
			return string.Format("[{0}, x{1}] {2}: '{3}'", new object[]
			{
				this.type,
				num,
				this.id,
				this.dev_text
			});
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x00217A64 File Offset: 0x00215C64
		public string GetCooldownID()
		{
			string text = this.event_name ?? "";
			if (!string.IsNullOrEmpty(this.condition_key))
			{
				text = text + "_" + this.condition_key;
			}
			return text;
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x00217AA4 File Offset: 0x00215CA4
		public string GetSuppressionCheckString()
		{
			string text = string.Concat(new string[]
			{
				this.type.ToString(),
				"_",
				this.role_name,
				"_",
				this.event_name
			});
			if (!string.IsNullOrEmpty(this.condition_key))
			{
				text = text + "_" + this.condition_key;
			}
			return text;
		}

		// Token: 0x04003663 RID: 13923
		public Voices.VoiceLine.Type type;

		// Token: 0x04003664 RID: 13924
		public string id;

		// Token: 0x04003665 RID: 13925
		public string filename;

		// Token: 0x04003666 RID: 13926
		public int line_number;

		// Token: 0x04003667 RID: 13927
		public string event_name;

		// Token: 0x04003668 RID: 13928
		public string condition_key;

		// Token: 0x04003669 RID: 13929
		public int variant;

		// Token: 0x0400366A RID: 13930
		public string role_name;

		// Token: 0x0400366B RID: 13931
		public string dev_text;

		// Token: 0x0400366C RID: 13932
		public string localized_text;

		// Token: 0x0400366D RID: 13933
		public bool is_default_text;

		// Token: 0x0400366E RID: 13934
		public string comment;

		// Token: 0x0400366F RID: 13935
		public string old_fmod_event;

		// Token: 0x04003670 RID: 13936
		public string old_audio_file;

		// Token: 0x04003671 RID: 13937
		public int old_variant;

		// Token: 0x04003672 RID: 13938
		public Voices.Event evt;

		// Token: 0x04003673 RID: 13939
		public Voices.Role role;

		// Token: 0x04003674 RID: 13940
		public DT.Field condition_field;

		// Token: 0x04003675 RID: 13941
		public int weight;

		// Token: 0x04003676 RID: 13942
		public Voices.VoiceLine.AllowDefaultCondition allow_default_condition;

		// Token: 0x04003677 RID: 13943
		public int priority;

		// Token: 0x04003678 RID: 13944
		public int bv_priority;

		// Token: 0x04003679 RID: 13945
		public float cooldown;

		// Token: 0x0400367A RID: 13946
		public float timeout;

		// Token: 0x0400367B RID: 13947
		public bool que_clearing;

		// Token: 0x0400367C RID: 13948
		public bool skip_queue;

		// Token: 0x0400367D RID: 13949
		public string fmod_event;

		// Token: 0x0400367E RID: 13950
		public Voices.VoiceLine refers_to;

		// Token: 0x0400367F RID: 13951
		public List<int> spoken_by;

		// Token: 0x02000A05 RID: 2565
		public enum Type
		{
			// Token: 0x0400462F RID: 17967
			Narrator,
			// Token: 0x04004630 RID: 17968
			Character,
			// Token: 0x04004631 RID: 17969
			Unit,
			// Token: 0x04004632 RID: 17970
			COUNT
		}

		// Token: 0x02000A06 RID: 2566
		public enum AllowDefaultCondition
		{
			// Token: 0x04004634 RID: 17972
			Always,
			// Token: 0x04004635 RID: 17973
			Never,
			// Token: 0x04004636 RID: 17974
			WhenAllConditionsFail
		}
	}

	// Token: 0x020006AF RID: 1711
	public class VoiceLineCreationParams
	{
		// Token: 0x04003680 RID: 13952
		public string filename;

		// Token: 0x04003681 RID: 13953
		public int line_number;

		// Token: 0x04003682 RID: 13954
		public Voices.Event evt;

		// Token: 0x04003683 RID: 13955
		public string condition_key;

		// Token: 0x04003684 RID: 13956
		public int variant = -1;

		// Token: 0x04003685 RID: 13957
		public DT.Field condition_field;

		// Token: 0x04003686 RID: 13958
		public string role_name;

		// Token: 0x04003687 RID: 13959
		public string dev_text;

		// Token: 0x04003688 RID: 13960
		public string default_text;

		// Token: 0x04003689 RID: 13961
		public bool is_default_text;

		// Token: 0x0400368A RID: 13962
		public string comment;

		// Token: 0x0400368B RID: 13963
		public int weight;

		// Token: 0x0400368C RID: 13964
		public Voices.VoiceLine refers_to;
	}

	// Token: 0x020006B0 RID: 1712
	public class Role
	{
		// Token: 0x0600486A RID: 18538 RVA: 0x00217B24 File Offset: 0x00215D24
		public override string ToString()
		{
			return string.Format("[{0}] {1}: lines: {2}, spoken by: {3}", new object[]
			{
				this.type,
				this.id,
				this.voice_lines.Count,
				this.spoken_by.Count
			});
		}

		// Token: 0x0400368D RID: 13965
		public Voices.VoiceLine.Type type;

		// Token: 0x0400368E RID: 13966
		public string id;

		// Token: 0x0400368F RID: 13967
		public string description;

		// Token: 0x04003690 RID: 13968
		public string old_role;

		// Token: 0x04003691 RID: 13969
		public DT.Field def;

		// Token: 0x04003692 RID: 13970
		public List<Voices.VoiceLine> voice_lines = new List<Voices.VoiceLine>();

		// Token: 0x04003693 RID: 13971
		public List<int> spoken_by = new List<int>();
	}

	// Token: 0x020006B1 RID: 1713
	public class Event
	{
		// Token: 0x0600486C RID: 18540 RVA: 0x00217B9C File Offset: 0x00215D9C
		public override string ToString()
		{
			return string.Format("[{0}] {1}, lines: {2}", this.type, this.id, this.voice_lines.Count);
		}

		// Token: 0x0600486D RID: 18541 RVA: 0x00217BC9 File Offset: 0x00215DC9
		public bool EvalDefaultCondition(IVars vars)
		{
			DT.Field field = this.default_condition;
			return field == null || field.Bool(vars, true);
		}

		// Token: 0x04003694 RID: 13972
		public Voices.VoiceLine.Type type;

		// Token: 0x04003695 RID: 13973
		public string id;

		// Token: 0x04003696 RID: 13974
		public int row;

		// Token: 0x04003697 RID: 13975
		public string description;

		// Token: 0x04003698 RID: 13976
		public DT.Field def;

		// Token: 0x04003699 RID: 13977
		public List<Voices.VoiceLine> voice_lines = new List<Voices.VoiceLine>();

		// Token: 0x0400369A RID: 13978
		public Dictionary<string, int> unique_variants = new Dictionary<string, int>();

		// Token: 0x0400369B RID: 13979
		public bool has_conditions;

		// Token: 0x0400369C RID: 13980
		public DT.Field default_condition;

		// Token: 0x0400369D RID: 13981
		public List<string> ignore_roles = new List<string>();

		// Token: 0x0400369E RID: 13982
		public int last_variant_played = -1;
	}

	// Token: 0x020006B2 RID: 1714
	public class Actor
	{
		// Token: 0x0600486F RID: 18543 RVA: 0x00217C0E File Offset: 0x00215E0E
		public override string ToString()
		{
			return string.Format("[{0}] {1} ({2})", this.id, this.name, this.ethnicity);
		}

		// Token: 0x0400369F RID: 13983
		public int id;

		// Token: 0x040036A0 RID: 13984
		public string name;

		// Token: 0x040036A1 RID: 13985
		public string ethnicity;
	}

	// Token: 0x020006B3 RID: 1715
	public class Manifest
	{
		// Token: 0x040036A2 RID: 13986
		public Dictionary<string, Voices.Manifest.FileInfo> files = new Dictionary<string, Voices.Manifest.FileInfo>();

		// Token: 0x040036A3 RID: 13987
		public Voices.Manifest.DirInfo root;

		// Token: 0x040036A4 RID: 13988
		public int max_actor_id;

		// Token: 0x02000A07 RID: 2567
		public class FileInfo
		{
			// Token: 0x0600553A RID: 21818 RVA: 0x00248A8C File Offset: 0x00246C8C
			public override string ToString()
			{
				return this.path;
			}

			// Token: 0x04004637 RID: 17975
			public string name;

			// Token: 0x04004638 RID: 17976
			public string path;

			// Token: 0x04004639 RID: 17977
			public Voices.Manifest.DirInfo directory;

			// Token: 0x0400463A RID: 17978
			public int actor_id;

			// Token: 0x0400463B RID: 17979
			public string role_event_condition;

			// Token: 0x0400463C RID: 17980
			public int variant;

			// Token: 0x0400463D RID: 17981
			public bool used;
		}

		// Token: 0x02000A08 RID: 2568
		public class DirInfo
		{
			// Token: 0x0600553C RID: 21820 RVA: 0x00248A94 File Offset: 0x00246C94
			public override string ToString()
			{
				return string.Format("{0}: files: {1}, subdirs: {2}", this.path, this.files.Count, this.subdirs.Count);
			}

			// Token: 0x0400463E RID: 17982
			public string name;

			// Token: 0x0400463F RID: 17983
			public string path;

			// Token: 0x04004640 RID: 17984
			public Voices.Manifest.DirInfo parent;

			// Token: 0x04004641 RID: 17985
			public List<Voices.Manifest.FileInfo> files = new List<Voices.Manifest.FileInfo>();

			// Token: 0x04004642 RID: 17986
			public List<Voices.Manifest.DirInfo> subdirs = new List<Voices.Manifest.DirInfo>();
		}
	}

	// Token: 0x020006B4 RID: 1716
	private class VoiceLineProperties
	{
		// Token: 0x06004872 RID: 18546 RVA: 0x00217C44 File Offset: 0x00215E44
		public string GetHashString()
		{
			return this.event_name + this.condition + this.type.ToString();
		}

		// Token: 0x06004873 RID: 18547 RVA: 0x00217C68 File Offset: 0x00215E68
		public static string GetHashString(Voices.VoiceLine vl)
		{
			return vl.event_name + vl.condition_key + vl.type.ToString();
		}

		// Token: 0x06004874 RID: 18548 RVA: 0x00217C8C File Offset: 0x00215E8C
		public static Voices.VoiceLineProperties Get(Voices.VoiceLine vl)
		{
			string hashString = Voices.VoiceLineProperties.GetHashString(vl);
			Voices.VoiceLineProperties result;
			if (Voices.VoiceLineProperties.properties_dictionary.TryGetValue(hashString, out result))
			{
				return result;
			}
			AudioLog.Warning(string.Format("Failed to find voice entry in properties.csv: {0}, condition: {1}, type: {2}", vl.event_name, vl.condition_key, vl.type));
			Voices.VoiceLineProperties voiceLineProperties = new Voices.VoiceLineProperties();
			Voices.VoiceLineProperties.properties_dictionary.Add(hashString, voiceLineProperties);
			return voiceLineProperties;
		}

		// Token: 0x06004875 RID: 18549 RVA: 0x00217CEC File Offset: 0x00215EEC
		public static void Add(Voices.VoiceLineProperties vlp)
		{
			string hashString = vlp.GetHashString();
			if (Voices.VoiceLineProperties.properties_dictionary.ContainsKey(hashString))
			{
				AudioLog.Error("Duplicated voice line properties key: " + hashString);
				return;
			}
			Voices.VoiceLineProperties.properties_dictionary.Add(hashString, vlp);
		}

		// Token: 0x06004876 RID: 18550 RVA: 0x00217D2A File Offset: 0x00215F2A
		public static void Clear()
		{
			Voices.VoiceLineProperties.properties_dictionary.Clear();
		}

		// Token: 0x040036A5 RID: 13989
		private static Dictionary<string, Voices.VoiceLineProperties> properties_dictionary = new Dictionary<string, Voices.VoiceLineProperties>();

		// Token: 0x040036A6 RID: 13990
		public string event_name;

		// Token: 0x040036A7 RID: 13991
		public string condition;

		// Token: 0x040036A8 RID: 13992
		public Voices.VoiceLine.Type type;

		// Token: 0x040036A9 RID: 13993
		public int priority;

		// Token: 0x040036AA RID: 13994
		public int bv_priority;

		// Token: 0x040036AB RID: 13995
		public float cooldown;

		// Token: 0x040036AC RID: 13996
		public bool que_clearing;

		// Token: 0x040036AD RID: 13997
		public string fmod_event = FMODVoiceProvider.default_voice_event;

		// Token: 0x040036AE RID: 13998
		public float timeout = 10f;

		// Token: 0x040036AF RID: 13999
		public bool skip_queue;

		// Token: 0x040036B0 RID: 14000
		public Voices.VoiceLine.AllowDefaultCondition allow_default_condition;
	}

	// Token: 0x020006B5 RID: 1717
	private struct AllocatedActorIDCharacters
	{
		// Token: 0x06004879 RID: 18553 RVA: 0x00217D60 File Offset: 0x00215F60
		public bool IsValid()
		{
			return this.actor_id > 0 && this.in_court != null && this.others != null;
		}

		// Token: 0x0600487A RID: 18554 RVA: 0x00217D80 File Offset: 0x00215F80
		public override string ToString()
		{
			if (this.actor_id <= 0)
			{
				return "null";
			}
			return string.Format("actor {0:D2}: in court: {1}, others: {2}", this.actor_id, this.in_court.Count, this.others.Count);
		}

		// Token: 0x040036B1 RID: 14001
		public static Dictionary<string, int> language_id = new Dictionary<string, int>
		{
			{
				"en",
				0
			},
			{
				"fr",
				1
			},
			{
				"de",
				2
			},
			{
				"zh",
				3
			}
		};

		// Token: 0x040036B2 RID: 14002
		public int actor_id;

		// Token: 0x040036B3 RID: 14003
		public List<Voices.AllocatedActorIDCharacters.CharacterInfo> in_court;

		// Token: 0x040036B4 RID: 14004
		public List<Voices.AllocatedActorIDCharacters.CharacterInfo> others;

		// Token: 0x02000A09 RID: 2569
		public struct CharacterInfo
		{
			// Token: 0x0600553E RID: 21822 RVA: 0x00248AE4 File Offset: 0x00246CE4
			public override string ToString()
			{
				if (this.character == null)
				{
					return string.Format("[{0}] {1} (deleted)", this.timestamp, NID.ToString(this.nid));
				}
				return string.Format("[{0}] {1}", this.timestamp, this.character);
			}

			// Token: 0x04004643 RID: 17987
			public Logic.Character character;

			// Token: 0x04004644 RID: 17988
			public int nid;

			// Token: 0x04004645 RID: 17989
			public Logic.Time timestamp;
		}
	}

	// Token: 0x020006B6 RID: 1718
	private class RECInfo
	{
		// Token: 0x0600487C RID: 18556 RVA: 0x00217E10 File Offset: 0x00216010
		public override string ToString()
		{
			return string.Format("{0}: spoken by: {1}, max variant: {2}, files: {3}", new object[]
			{
				this.key,
				this.spoken_by.Count,
				this.max_variant,
				this.files.Count
			});
		}

		// Token: 0x040036B5 RID: 14005
		public string key;

		// Token: 0x040036B6 RID: 14006
		public List<int> spoken_by = new List<int>();

		// Token: 0x040036B7 RID: 14007
		public List<Voices.Manifest.FileInfo> files = new List<Voices.Manifest.FileInfo>();

		// Token: 0x040036B8 RID: 14008
		public int max_variant;

		// Token: 0x040036B9 RID: 14009
		public bool exported;
	}
}
