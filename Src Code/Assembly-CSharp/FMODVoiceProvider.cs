using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x0200017E RID: 382
public static class FMODVoiceProvider
{
	// Token: 0x060014A9 RID: 5289 RVA: 0x000CF2EC File Offset: 0x000CD4EC
	public static void TurnOn()
	{
		if (FMODVoiceProvider.state == FMODVoiceProvider.State.Paused)
		{
			float num = UnityEngine.Time.unscaledTime - FMODVoiceProvider.timePaused;
			for (int i = FMODVoiceProvider.voiceQueue.Count - 1; i >= 0; i--)
			{
				if (!FMODVoiceProvider.voiceQueue[i].HasStarted())
				{
					FMODVoiceProvider.voiceQueue[i].enqueue_time += num;
				}
			}
		}
		FMODVoiceProvider.state = FMODVoiceProvider.State.On;
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x000CF357 File Offset: 0x000CD557
	public static void TurnOff()
	{
		FMODVoiceProvider.ClearAllInactiveVoices();
		FMODVoiceProvider.state = FMODVoiceProvider.State.Off;
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x000CF364 File Offset: 0x000CD564
	public static void Pause()
	{
		FMODVoiceProvider.timePaused = UnityEngine.Time.unscaledTime;
		FMODVoiceProvider.state = FMODVoiceProvider.State.Paused;
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x000CF376 File Offset: 0x000CD576
	public static bool IsRegistered()
	{
		return !string.IsNullOrEmpty(FMODVoiceProvider.bankLoaded) && FMODVoiceProvider.voiceOverCallback != null;
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x000CF390 File Offset: 0x000CD590
	public static void Register()
	{
		if (!FMODVoiceProvider.IsRegistered())
		{
			FMODVoiceProvider.voiceOverCallback = new EVENT_CALLBACK(FMODVoiceProvider.VoiceOverCallback);
			string text = "voices_" + Voices.Language;
			if (!RuntimeManager.HasBankLoaded(text))
			{
				RuntimeManager.LoadBank(text, false);
			}
			if (RuntimeManager.HasBankLoaded(text))
			{
				FMODVoiceProvider.bankLoaded = text;
				AudioLog.Info("Loaded voice bank: " + text);
				Voices.RefreshActorIDEntries();
				return;
			}
			AudioLog.Error("Failed to load sound bank: " + text);
		}
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x000CF408 File Offset: 0x000CD608
	public static void Unregister()
	{
		if (FMODVoiceProvider.IsRegistered())
		{
			FMODVoiceProvider.ClearAllVoices();
			FMODVoiceProvider.voiceOverCallback = null;
			RuntimeManager.UnloadBank(FMODVoiceProvider.bankLoaded);
			AudioLog.Info("Unloaded voice bank: " + FMODVoiceProvider.bankLoaded);
			FMODVoiceProvider.bankLoaded = null;
		}
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x000CF440 File Offset: 0x000CD640
	public static void PlayVoiceEvent(string eventPath, IVars vars, Vector3 position)
	{
		if (LoadingScreen.IsShown())
		{
			return;
		}
		if (FMODVoiceProvider.state == FMODVoiceProvider.State.Off)
		{
			return;
		}
		if (string.IsNullOrEmpty(eventPath))
		{
			return;
		}
		if (eventPath.StartsWith("event:", StringComparison.Ordinal))
		{
			AudioLog.Warning("Ignoring legacy voice event: " + eventPath);
		}
		int num = eventPath.IndexOf(':');
		if (num < 0)
		{
			AudioLog.Error("BaseUI.PlayVoiceEvent called with invalid event: '" + eventPath + "'");
			return;
		}
		string a = eventPath.Substring(0, num);
		string event_name = eventPath.Substring(num + 1);
		if (a == "narrator_voice")
		{
			Voices.PlayNarratorVoiceLine(event_name, vars);
			return;
		}
		if (!(a == "character_voice"))
		{
			if (!(a == "unit_voice"))
			{
				AudioLog.Error("BaseUI.PlayVoiceEvent unknown event prefix: '" + eventPath + "'");
				return;
			}
			Logic.Unit unit;
			if ((unit = (vars as Logic.Unit)) != null)
			{
				Voices.PlayUnitVoiceLine(unit, event_name);
				return;
			}
			Logic.Squad squad;
			if ((squad = (vars as Logic.Squad)) != null)
			{
				Voices.PlaySquadVoiceLine(squad, event_name, position);
				return;
			}
			AudioLog.Error(string.Format("BaseUI.PlayVoiceEvent '{0}' has no unit or squad argument: {1}", eventPath, vars));
			return;
		}
		else
		{
			Logic.Character character = vars as Logic.Character;
			if (character == null && vars != null)
			{
				character = vars.GetVar("character", null, true).Get<Logic.Character>();
			}
			Logic.Army army;
			if (character == null && (army = (vars as Logic.Army)) != null)
			{
				character = army.leader;
			}
			Vars vars2;
			if (character == null && (vars2 = (vars as Vars)) != null)
			{
				army = vars2.obj.Get<Logic.Army>();
				character = ((army != null) ? army.leader : null);
				if (character == null)
				{
					character = vars2.obj.Get<Logic.Character>();
				}
			}
			bool flag = vars != null && vars.GetVar("no_character_allowed", null, true).Bool();
			if (character == null && !flag)
			{
				AudioLog.Error(string.Format("BaseUI.PlayVoiceEvent '{0}' has no character argument: {1}", eventPath, vars));
				return;
			}
			if (character == null || character.IsAlive())
			{
				Voices.PlayCharacterVoiceLine(character, event_name, vars, flag);
				return;
			}
			return;
		}
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x000CF60C File Offset: 0x000CD80C
	[MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	private static RESULT VoiceOverCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
	{
		EventInstance eventInstance = new EventInstance(instancePtr);
		IntPtr value;
		RESULT result = eventInstance.getUserData(out value);
		if (result != RESULT.OK)
		{
			AudioLog.Error(string.Format("Failed to get user data for {0}", eventInstance));
			return result;
		}
		GCHandle gchandle = GCHandle.FromIntPtr(value);
		string text = gchandle.Target as string;
		if (type != EVENT_CALLBACK_TYPE.DESTROYED)
		{
			if (type != EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
			{
				if (type == EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND)
				{
					PROGRAMMER_SOUND_PROPERTIES programmer_SOUND_PROPERTIES = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
					Sound sound = new Sound(programmer_SOUND_PROPERTIES.sound);
					result = sound.release();
					if (result != RESULT.OK)
					{
						AudioLog.Error("Failed to release sound for " + text);
						return result;
					}
				}
			}
			else if (FMODVoiceProvider.voiceQueue.Count >= 1)
			{
				FMODVoiceProvider.VoiceOverItem voiceOverItem = FMODVoiceProvider.voiceQueue[0];
				PROGRAMMER_SOUND_PROPERTIES structure = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
				SOUND_INFO sound_INFO;
				result = RuntimeManager.StudioSystem.getSoundInfo(text, out sound_INFO);
				if (result != RESULT.OK)
				{
					AudioLog.Warning(string.Format("Couldn't find voiceover for {0}", voiceOverItem));
					return result;
				}
				Sound sound2;
				result = RuntimeManager.CoreSystem.createSound(sound_INFO.name_or_data, sound_INFO.mode, ref sound_INFO.exinfo, out sound2);
				if (result != RESULT.OK)
				{
					AudioLog.Error(string.Format("Failed to create sound for {0}", voiceOverItem));
					return result;
				}
				Sound sound3;
				result = sound2.getSubSound(sound_INFO.subsoundindex, out sound3);
				if (result != RESULT.OK)
				{
					AudioLog.Error(string.Format("Failed to get subsound for {0}", voiceOverItem));
					return result;
				}
				uint num;
				result = sound3.getLength(out num, TIMEUNIT.MS);
				if (result != RESULT.OK)
				{
					AudioLog.Error(string.Format("Failed to get voice length for {0}", voiceOverItem));
					return result;
				}
				voiceOverItem.duration = num / 1000f;
				if (voiceOverItem.duration <= 0f)
				{
					AudioLog.Warning(string.Format("Voice line has a duration of 0: {0}", voiceOverItem));
				}
				if (FMODVoiceProvider.voiceQueue.Count < 1)
				{
					UnityEngine.Debug.LogError("Voice queue is empty even though it wasn't at the beginning of the method?!");
					return RESULT.ERR_MEMORY;
				}
				FMODVoiceProvider.voiceQueue[0] = voiceOverItem;
				structure.sound = sound2.handle;
				structure.subsoundIndex = sound_INFO.subsoundindex;
				Marshal.StructureToPtr<PROGRAMMER_SOUND_PROPERTIES>(structure, parameterPtr, false);
			}
		}
		else
		{
			gchandle.Free();
		}
		return result;
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000CF83C File Offset: 0x000CDA3C
	public static void ClearAllInactiveVoices()
	{
		for (int i = FMODVoiceProvider.voiceQueue.Count - 1; i > 0; i--)
		{
			FMODVoiceProvider.RemoveQueueItem(i, true, true, "ClearAllInactiveVoices");
		}
		if (FMODVoiceProvider.voiceQueue.Count > 0 && !FMODVoiceProvider.voiceQueue[0].HasStarted())
		{
			FMODVoiceProvider.RemoveQueueItem(0, true, true, "ClearAllInactiveVoices");
		}
		FMODVoiceProvider.group_last_played.Clear();
		FMODVoiceProvider.voice_last_played.Clear();
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x000CF8B0 File Offset: 0x000CDAB0
	public static void ClearAllVoices()
	{
		FMODVoiceProvider.ClearAllInactiveVoices();
		if (FMODVoiceProvider.voiceQueue.Count > 0)
		{
			FMODVoiceProvider.RemoveQueueItem(0, true, true, "ClearAllVoices");
		}
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000CF8D4 File Offset: 0x000CDAD4
	public static void PlayVoiceLine(string key, Vector3 position, Voices.VoiceLine voiceLine)
	{
		float unscaledTime = UnityEngine.Time.unscaledTime;
		if (FMODVoiceProvider.use_voice_cooldowns && voiceLine.cooldown > 0f)
		{
			float num;
			if (FMODVoiceProvider.voice_last_played.TryGetValue(voiceLine.GetCooldownID(), out num) && unscaledTime - num < voiceLine.cooldown)
			{
				return;
			}
			float num2;
			if (FMODVoiceProvider.group_last_played.TryGetValue(voiceLine.fmod_event, out num2) && unscaledTime - num < num2)
			{
				return;
			}
		}
		string fmod_event = voiceLine.fmod_event;
		if (string.IsNullOrEmpty(fmod_event))
		{
			AudioLog.Error("No voice event provided for " + FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key) + "!");
			return;
		}
		if (FMODVoiceProvider.voiceQueue.Count > 0 && FMODVoiceProvider.voiceQueue[FMODVoiceProvider.voiceQueue.Count - 1].voice_line.que_clearing && !voiceLine.skip_queue)
		{
			AudioLog.Info(string.Format("{0}\nwas blocked by que_clearing voice: {1}!", FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key), FMODVoiceProvider.voiceQueue[FMODVoiceProvider.voiceQueue.Count - 1]));
			return;
		}
		if (!FMODVoiceProvider.ApplySuppressionFilters(voiceLine, key))
		{
			return;
		}
		int num3 = 0;
		if (voiceLine.que_clearing)
		{
			FMODVoiceProvider.ClearAllInactiveVoices();
			num3 = FMODVoiceProvider.voiceQueue.Count;
		}
		else
		{
			for (int i = 0; i < FMODVoiceProvider.voiceQueue.Count; i++)
			{
				if (FMODVoiceProvider.voiceQueue[i].voice_line.event_name == voiceLine.event_name)
				{
					AudioLog.Info(voiceLine.event_name + " already in queue! Discarding " + FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key));
					return;
				}
				if (FMODVoiceProvider.voiceQueue[i].voice_line.GetCurrentPriority() >= voiceLine.GetCurrentPriority() || FMODVoiceProvider.voiceQueue[i].HasStarted())
				{
					num3++;
				}
			}
		}
		EventInstance instance = FMODWrapper.CreateInstance(fmod_event, true);
		GCHandle value = GCHandle.Alloc(key);
		if (!value.IsAllocated)
		{
			AudioLog.Error("Failed to allocate handle for " + FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key));
			return;
		}
		RESULT result = instance.setUserData(GCHandle.ToIntPtr(value));
		if (result != RESULT.OK)
		{
			AudioLog.Error(string.Format("Failed to set user data for {0}: {1}", FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key), result));
			value.Free();
			return;
		}
		result = instance.setCallback(FMODVoiceProvider.voiceOverCallback, (EVENT_CALLBACK_TYPE)4294967295U);
		if (result != RESULT.OK)
		{
			AudioLog.Error(string.Format("Failed to set callback for {0}\nresult={1}", FMODVoiceProvider.VoiceOverItem.ToString(voiceLine, key), result));
			value.Free();
			return;
		}
		instance.set3DAttributes(new Vector3(position.x, position.y, position.z).To3DAttributes());
		FMODVoiceProvider.VoiceOverItem voiceOverItem = new FMODVoiceProvider.VoiceOverItem
		{
			voice_line = voiceLine,
			enqueue_time = unscaledTime,
			start_time = 0f,
			duration = 0f,
			name = key,
			instance = instance
		};
		if (voiceLine.skip_queue)
		{
			FMODVoiceProvider.StartVoiceItem(ref voiceOverItem);
			return;
		}
		AudioLog.Info(string.Format("Enqueue voice line (index={0}) {1}", num3, voiceOverItem));
		FMODVoiceProvider.voiceQueue.Insert(num3, voiceOverItem);
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x000CFBD0 File Offset: 0x000CDDD0
	public static void PrintQueue()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Voice queue:");
		for (int i = 0; i < FMODVoiceProvider.voiceQueue.Count; i++)
		{
			stringBuilder.AppendLine(FMODVoiceProvider.voiceQueue[i].name);
		}
		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x000CFC28 File Offset: 0x000CDE28
	public static void PrintLastVoices()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Last 20 voices:");
		for (int i = 0; i < FMODVoiceProvider.lastVoicesPlayed.Count; i++)
		{
			stringBuilder.AppendLine(FMODVoiceProvider.lastVoicesPlayed[i]);
		}
		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x000CFC7C File Offset: 0x000CDE7C
	private static bool ApplySuppressionFilters(Voices.VoiceLine line, string key)
	{
		string suppressionCheckString = line.GetSuppressionCheckString();
		FMODVoiceProvider.suppressed_queue_indices.Clear();
		foreach (string text in Voices.suppression_data.Keys)
		{
			for (int i = 0; i < FMODVoiceProvider.voiceQueue.Count; i++)
			{
				string suppressionCheckString2 = FMODVoiceProvider.voiceQueue[i].voice_line.GetSuppressionCheckString();
				if (Game.Match(suppressionCheckString2, text, false, '*'))
				{
					List<string> list = Voices.suppression_data[text];
					for (int j = 0; j < list.Count; j++)
					{
						if (Game.Match(suppressionCheckString, list[j], false, '*'))
						{
							AudioLog.Info(string.Format("{0}\nwas suppressed by {1}", FMODVoiceProvider.VoiceOverItem.ToString(line, key), FMODVoiceProvider.voiceQueue[i]));
							return false;
						}
					}
				}
				if ((i != 0 || FMODVoiceProvider.voiceQueue[i].start_time == 0f || FMODVoiceProvider.voiceQueue[i].start_time >= UnityEngine.Time.unscaledTime) && Game.Match(suppressionCheckString, text, false, '*'))
				{
					List<string> list2 = Voices.suppression_data[text];
					for (int k = 0; k < list2.Count; k++)
					{
						if (Game.Match(suppressionCheckString2, list2[k], false, '*'))
						{
							AudioLog.Info(string.Format("{0} was suppressed by {1}", FMODVoiceProvider.voiceQueue[i], FMODVoiceProvider.VoiceOverItem.ToString(line, key)));
							FMODVoiceProvider.suppressed_queue_indices.Add(i);
						}
					}
				}
			}
		}
		int num = 0;
		foreach (int num2 in FMODVoiceProvider.suppressed_queue_indices)
		{
			FMODVoiceProvider.RemoveQueueItem(num2 - num, true, true, "suppressed");
			num++;
		}
		return true;
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000CFEA4 File Offset: 0x000CE0A4
	private static void StartVoiceItem(ref FMODVoiceProvider.VoiceOverItem voi)
	{
		FMODVoiceProvider.StartVoiceInstance(voi);
		FMODVoiceProvider.ReleaseVoiceInstance(voi);
		voi.start_time = UnityEngine.Time.unscaledTime;
		if (voi.voice_line.cooldown > 0f)
		{
			FMODVoiceProvider.voice_last_played[voi.voice_line.GetCooldownID()] = voi.start_time;
		}
		if (Voices.group_cooldowns.ContainsKey(voi.voice_line.fmod_event))
		{
			FMODVoiceProvider.group_last_played[voi.voice_line.fmod_event] = voi.start_time;
		}
		voi.voice_line.evt.last_variant_played = voi.voice_line.variant;
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000CFF4C File Offset: 0x000CE14C
	private static void TryPlayVoiceFromQueue()
	{
		if (FMODVoiceProvider.voiceQueue.Count == 0)
		{
			return;
		}
		FMODVoiceProvider.VoiceOverItem value = FMODVoiceProvider.voiceQueue[0];
		FMODVoiceProvider.StartVoiceItem(ref value);
		FMODVoiceProvider.voiceQueue[0] = value;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x000CFF88 File Offset: 0x000CE188
	private static void StartVoiceInstance(FMODVoiceProvider.VoiceOverItem item)
	{
		if (!item.instance.isValid())
		{
			AudioLog.Error("Invalid FMOD instance for " + item.name);
		}
		RESULT result = item.instance.start();
		if (result == RESULT.OK)
		{
			if (FMODVoiceProvider.lastVoicesPlayed.Count >= 20)
			{
				FMODVoiceProvider.lastVoicesPlayed.RemoveAt(0);
			}
			FMODVoiceProvider.lastVoicesPlayed.Add(item.name);
			AudioLog.Info(string.Format("Started voice line:{0}\nwaited for {1} seconds", item, UnityEngine.Time.unscaledTime - item.enqueue_time));
			return;
		}
		AudioLog.Error(string.Format("Failed to start voice line: {0}\n(result: {1})", item, result));
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x000D0034 File Offset: 0x000CE234
	private static void ReleaseVoiceInstance(FMODVoiceProvider.VoiceOverItem item)
	{
		if (!item.instance.isValid())
		{
			AudioLog.Error(string.Format("Invalid FMOD instance for {0}", item));
		}
		RESULT result = item.instance.release();
		if (result == RESULT.OK)
		{
			AudioLog.Info(string.Format("Released voice line: {0}", item));
			return;
		}
		AudioLog.Error(string.Format("Failed to release voice line: {0}\n(result: {1})", item, result));
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x000D00A8 File Offset: 0x000CE2A8
	private static void StopVoiceInstance(FMODVoiceProvider.VoiceOverItem item, FMOD.Studio.STOP_MODE stop_mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		if (!item.instance.isValid())
		{
			AudioLog.Error(string.Format("Invalid FMOD instance for {0}", item));
		}
		RESULT result = item.instance.stop(stop_mode);
		if (result == RESULT.OK)
		{
			AudioLog.Info(string.Format("Stopped voice line: {0}", item));
			return;
		}
		AudioLog.Error(string.Format("Failed to stop voice line: {0}\n(result: {1})", item, result));
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000D011C File Offset: 0x000CE31C
	private static void RemoveQueueItem(int index, bool release = true, bool stop = true, string reason = "default")
	{
		if (index >= FMODVoiceProvider.voiceQueue.Count)
		{
			AudioLog.Error("Failed to remove item from queue, it doesn't exist!");
			return;
		}
		if (stop)
		{
			FMODVoiceProvider.StopVoiceInstance(FMODVoiceProvider.voiceQueue[index], FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (release)
		{
			FMODVoiceProvider.ReleaseVoiceInstance(FMODVoiceProvider.voiceQueue[index]);
		}
		AudioLog.Info(string.Format("Removing from queue (index={0}, reason={1}): {2}", index, reason, FMODVoiceProvider.voiceQueue[index]));
		FMODVoiceProvider.voiceQueue.RemoveAt(index);
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x000D019C File Offset: 0x000CE39C
	public static void Update()
	{
		if (FMODVoiceProvider.voiceQueue.Count == 0)
		{
			return;
		}
		FMODVoiceProvider.VoiceOverItem voiceOverItem = FMODVoiceProvider.voiceQueue[0];
		PLAYBACK_STATE playback_STATE;
		voiceOverItem.instance.getPlaybackState(out playback_STATE);
		if (playback_STATE != PLAYBACK_STATE.STOPPED && playback_STATE != PLAYBACK_STATE.STOPPING)
		{
			return;
		}
		voiceOverItem = FMODVoiceProvider.voiceQueue[0];
		bool flag = voiceOverItem.HasEnded();
		if (flag)
		{
			FMODVoiceProvider.RemoveQueueItem(0, false, false, "ended");
		}
		float unscaledTime = UnityEngine.Time.unscaledTime;
		for (int i = FMODVoiceProvider.voiceQueue.Count - 1; i >= 0; i--)
		{
			FMODVoiceProvider.VoiceOverItem voiceOverItem2 = FMODVoiceProvider.voiceQueue[i];
			if (i >= Voices.priority_queue_size)
			{
				FMODVoiceProvider.RemoveQueueItem(i, true, false, "overflow");
			}
			else if (voiceOverItem2.HasTimedOut() && FMODVoiceProvider.state == FMODVoiceProvider.State.On)
			{
				FMODVoiceProvider.RemoveQueueItem(i, true, false, "timeout");
			}
			else if (!voiceOverItem2.instance.isValid())
			{
				FMODVoiceProvider.RemoveQueueItem(i, true, false, "invalid");
			}
			else if (voiceOverItem2.voice_line.cooldown > 0f)
			{
				float num;
				float num2;
				if (FMODVoiceProvider.voice_last_played.TryGetValue(voiceOverItem2.voice_line.GetCooldownID(), out num) && unscaledTime - num < voiceOverItem2.voice_line.cooldown)
				{
					FMODVoiceProvider.RemoveQueueItem(i, true, false, "cooldown");
				}
				else if (FMODVoiceProvider.group_last_played.TryGetValue(voiceOverItem2.voice_line.fmod_event, out num2) && unscaledTime - num < num2)
				{
					FMODVoiceProvider.RemoveQueueItem(i, true, false, "group_cooldown");
				}
			}
		}
		if (!flag && FMODVoiceProvider.voiceQueue.Count > 0)
		{
			voiceOverItem = FMODVoiceProvider.voiceQueue[0];
			if (voiceOverItem.HasStarted())
			{
				return;
			}
		}
		if (FMODVoiceProvider.state == FMODVoiceProvider.State.On)
		{
			FMODVoiceProvider.TryPlayVoiceFromQueue();
		}
	}

	// Token: 0x04000D55 RID: 3413
	private static List<FMODVoiceProvider.VoiceOverItem> voiceQueue = new List<FMODVoiceProvider.VoiceOverItem>();

	// Token: 0x04000D56 RID: 3414
	private static EVENT_CALLBACK voiceOverCallback = null;

	// Token: 0x04000D57 RID: 3415
	private static string bankLoaded = null;

	// Token: 0x04000D58 RID: 3416
	public static List<string> lastVoicesPlayed = new List<string>();

	// Token: 0x04000D59 RID: 3417
	private static FMODVoiceProvider.State state = FMODVoiceProvider.State.On;

	// Token: 0x04000D5A RID: 3418
	private static float timePaused = 0f;

	// Token: 0x04000D5B RID: 3419
	public static string default_voice_event = "event:/voices";

	// Token: 0x04000D5C RID: 3420
	public static bool use_voice_cooldowns = true;

	// Token: 0x04000D5D RID: 3421
	private static HashSet<int> suppressed_queue_indices = new HashSet<int>();

	// Token: 0x04000D5E RID: 3422
	public static Dictionary<string, float> voice_last_played = new Dictionary<string, float>();

	// Token: 0x04000D5F RID: 3423
	public static Dictionary<string, float> group_last_played = new Dictionary<string, float>();

	// Token: 0x020006AC RID: 1708
	private struct VoiceOverItem
	{
		// Token: 0x06004858 RID: 18520 RVA: 0x002176AC File Offset: 0x002158AC
		public bool HasStarted()
		{
			return this.start_time > 0f;
		}

		// Token: 0x06004859 RID: 18521 RVA: 0x002176BB File Offset: 0x002158BB
		public bool HasEnded()
		{
			return this.HasStarted() && this.start_time + this.duration < UnityEngine.Time.unscaledTime;
		}

		// Token: 0x0600485A RID: 18522 RVA: 0x002176DB File Offset: 0x002158DB
		public bool HasTimedOut()
		{
			return !this.HasStarted() && this.voice_line.timeout >= 0f && this.voice_line.timeout <= UnityEngine.Time.unscaledTime - this.enqueue_time;
		}

		// Token: 0x0600485B RID: 18523 RVA: 0x00217715 File Offset: 0x00215915
		public override string ToString()
		{
			return "VoiceOverItem:\n\t" + FMODVoiceProvider.VoiceOverItem.ToString(this.voice_line, this.name);
		}

		// Token: 0x0600485C RID: 18524 RVA: 0x00217734 File Offset: 0x00215934
		public static string ToString(Voices.VoiceLine voice_line, string key)
		{
			return string.Concat(new string[]
			{
				"VoiceLine: ",
				voice_line.id,
				"\n\tAudioTableKey: ",
				key,
				"\n\tEvent: ",
				voice_line.event_name,
				"\n\tText: ",
				voice_line.dev_text
			});
		}

		// Token: 0x04003659 RID: 13913
		public Voices.VoiceLine voice_line;

		// Token: 0x0400365A RID: 13914
		public float enqueue_time;

		// Token: 0x0400365B RID: 13915
		public float start_time;

		// Token: 0x0400365C RID: 13916
		public float duration;

		// Token: 0x0400365D RID: 13917
		public string name;

		// Token: 0x0400365E RID: 13918
		public EventInstance instance;
	}

	// Token: 0x020006AD RID: 1709
	public enum State
	{
		// Token: 0x04003660 RID: 13920
		On,
		// Token: 0x04003661 RID: 13921
		Off,
		// Token: 0x04003662 RID: 13922
		Paused
	}
}
