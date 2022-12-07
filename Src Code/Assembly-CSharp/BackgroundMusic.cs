using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class BackgroundMusic : MonoBehaviour
{
	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000D80 RID: 3456 RVA: 0x0009801D File Offset: 0x0009621D
	// (set) Token: 0x06000D81 RID: 3457 RVA: 0x00098045 File Offset: 0x00096245
	public static BackgroundMusic Instance
	{
		get
		{
			if (BackgroundMusic._instance == null)
			{
				BackgroundMusic._instance = new GameObject("BackgroundMusic").AddComponent<BackgroundMusic>();
			}
			return BackgroundMusic._instance;
		}
		set
		{
			BackgroundMusic._instance = value;
		}
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0009804D File Offset: 0x0009624D
	private void OnEnable()
	{
		if (BackgroundMusic._instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		BackgroundMusic.Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(BackgroundMusic.Instance);
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00098078 File Offset: 0x00096278
	public void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		BackgroundMusic.start_time = 0f;
		BackgroundMusic.finish_time = 0f;
		BackgroundMusic.resume_time = 0f;
		BackgroundMusic.cur_track_counter = 0;
		if (BackgroundMusic.eventEmitter == null)
		{
			Transform transform = base.transform;
			BackgroundMusic.eventEmitter = ((transform != null) ? transform.GetComponentInChildren<StudioEventEmitter>() : null);
			if (BackgroundMusic.eventEmitter == null)
			{
				BackgroundMusic.eventEmitter = base.gameObject.AddComponent<StudioEventEmitter>();
				BackgroundMusic.eventEmitter.PlayEvent = EmitterGameEvent.None;
				BackgroundMusic.eventEmitter.StopEvent = EmitterGameEvent.ObjectDisable;
				BackgroundMusic.eventEmitter.AllowFadeout = true;
			}
		}
		else
		{
			BackgroundMusic.eventEmitter.Stop();
		}
		BackgroundMusic.ReadMusicGroups();
		BackgroundMusic.PickRandomMusicGroup();
		BackgroundMusic.cur_group = BackgroundMusic.default_group;
		BackgroundMusic.PickRandomPlaylist();
		BackgroundMusic.Toggle(BackgroundMusic.CanPlayMusic());
		BackgroundMusic.PlayRandomTrack(false);
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00098151 File Offset: 0x00096351
	public static void Reset()
	{
		if (BackgroundMusic._instance == null)
		{
			return;
		}
		BackgroundMusic._instance.init = false;
		BackgroundMusic._instance.Init();
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00098178 File Offset: 0x00096378
	public static BackgroundMusic.Playlist LoadPlaylist(DT.Field f)
	{
		BackgroundMusic.Playlist playlist = new BackgroundMusic.Playlist();
		playlist.key = f.key;
		playlist.can_be_interrupted = f.GetBool("can_be_interrupted", null, true, true, true, true, '.');
		playlist.max_tracks_to_play = f.GetInt("tracks_to_play", null, 0, true, true, true, '.');
		playlist.min_pause_time = f.GetFloat("min_pause_time", null, 0f, true, true, true, '.');
		playlist.max_pause_time = f.GetFloat("max_pause_time", null, 0f, true, true, true, '.');
		playlist.works_bv = f.GetBool("works_bv", null, playlist.works_bv, true, true, true, '.');
		DT.Field field = f.FindChild("tracks", null, true, true, true, '.');
		if (field != null)
		{
			int num = field.NumValues();
			for (int i = 0; i < num; i++)
			{
				playlist.track_paths.Add(f.GetValue(i, "tracks", null, true, true, true, '.').String(null));
			}
		}
		DT.Field field2 = f.FindChild("religions", null, true, true, true, '.');
		if (((field2 != null) ? field2.children : null) != null)
		{
			List<string> list = new List<string>();
			for (int j = 0; j < field2.children.Count; j++)
			{
				string text = field2.children[j].key.ToLowerInvariant();
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			playlist.religions = list.ToArray();
		}
		playlist.Reset();
		return playlist;
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000982F0 File Offset: 0x000964F0
	public static BackgroundMusic.MusicTrigger LoadTrigger(DT.Field f)
	{
		string key = f.key.ToLowerInvariant();
		string @string = f.GetString("priority", null, "", true, true, true, '.');
		bool @bool = f.GetBool("interrupt", null, false, true, true, true, '.');
		DT.Field field = f.FindChild("cooldown", null, true, true, true, '.');
		float cooldown_min = 0f;
		float cooldown_max = 0f;
		if (field != null)
		{
			cooldown_min = field.Float(0, null, 0f);
			cooldown_max = field.Float(1, null, 0f);
		}
		BackgroundMusic.MusicTrigger musicTrigger = new BackgroundMusic.MusicTrigger
		{
			key = key,
			priority = @string,
			interrupt = @bool,
			cooldown_min = cooldown_min,
			cooldown_max = cooldown_max
		};
		List<DT.Field> list = f.Children();
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				DT.Field field2 = list[i];
				if (field2.type == "playlist")
				{
					BackgroundMusic.Playlist item = BackgroundMusic.LoadPlaylist(field2);
					musicTrigger.playlists.Add(item);
				}
			}
		}
		return musicTrigger;
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00098404 File Offset: 0x00096604
	public static BackgroundMusic.MusicGroup LoadMusicGroup(DT.Field f)
	{
		DT.Field field = f.FindChild("religions", null, true, true, true, '.');
		BackgroundMusic.MusicGroup musicGroup = new BackgroundMusic.MusicGroup();
		string key = f.key.ToLowerInvariant();
		if (((field != null) ? field.children : null) != null)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < field.children.Count; i++)
			{
				string text = field.children[i].key.ToLowerInvariant();
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			musicGroup.religions = list.ToArray();
		}
		musicGroup.key = key;
		musicGroup.snippet_percentage = f.GetFloat("snippet_percentage", null, musicGroup.snippet_percentage, true, true, true, '.');
		BackgroundMusic.LoadPlaylists(musicGroup, f);
		return musicGroup;
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000984CC File Offset: 0x000966CC
	private static void LoadPlaylists(BackgroundMusic.MusicGroup group, DT.Field m_def_field)
	{
		for (int i = 0; i < m_def_field.children.Count; i++)
		{
			DT.Field field = m_def_field.children[i];
			if (!(field.type != "playlist"))
			{
				bool flag = false;
				for (int j = 0; j < group.playlists.Count; j++)
				{
					if (group.playlists[j].key == field.key)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					BackgroundMusic.Playlist item = BackgroundMusic.LoadPlaylist(field);
					group.playlists.Add(item);
				}
			}
		}
		if (m_def_field.based_on != null)
		{
			BackgroundMusic.LoadPlaylists(group, m_def_field.based_on);
		}
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x0009857C File Offset: 0x0009677C
	public static void ReadMusicGroups()
	{
		BackgroundMusic.music_groups.Clear();
		DT dt = GameLogic.Get(true).dt;
		DT.Field field = dt.Find("MusicGroup", null);
		List<string> list = new List<string>();
		for (int i = 0; i < field.def.defs.Count; i++)
		{
			BackgroundMusic.MusicGroup musicGroup = BackgroundMusic.LoadMusicGroup(field.def.defs[i].field);
			list.Add(musicGroup.key);
			BackgroundMusic.music_groups.Add(musicGroup);
		}
		DT.Field field2 = dt.Find("MusicTrigger", null);
		for (int j = 0; j < field2.def.defs.Count; j++)
		{
			DT.Field field3 = field2.def.defs[j].field;
			BackgroundMusic.triggers[field3.key.ToLowerInvariant()] = BackgroundMusic.LoadTrigger(field3);
		}
		DT.Field field4 = dt.Find("TriggerPriorities", null);
		for (int k = 0; k < field4.children.Count; k++)
		{
			DT.Field field5 = field4.children[k];
			BackgroundMusic.priorities[field5.key] = field5.value;
		}
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x000986C0 File Offset: 0x000968C0
	private static void PickRandomMusicGroup()
	{
		if (BackgroundMusic.logging_enabled)
		{
			Debug.Log("Picking random music group");
		}
		BackgroundMusic.default_group = null;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Religion religion = (kingdom != null) ? kingdom.religion : null;
		string text;
		if (religion == null)
		{
			text = null;
		}
		else
		{
			string name = religion.name;
			text = ((name != null) ? name.ToLowerInvariant() : null);
		}
		string b = text;
		for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
		{
			BackgroundMusic.MusicGroup musicGroup = BackgroundMusic.music_groups[i];
			if (musicGroup.religions != null)
			{
				for (int j = 0; j < musicGroup.religions.Length; j++)
				{
					if (musicGroup.religions[j] == b)
					{
						BackgroundMusic.default_group = musicGroup;
						break;
					}
				}
			}
		}
		if (BackgroundMusic.default_group == null && BackgroundMusic.music_groups.Count > 0)
		{
			BackgroundMusic.default_group = BackgroundMusic.music_groups[Random.Range(0, BackgroundMusic.music_groups.Count)];
		}
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00098798 File Offset: 0x00096998
	public BackgroundMusic.Playlist GetPlaylist(List<BackgroundMusic.Playlist> groups, string key)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].key == key)
			{
				return groups[i];
			}
		}
		return null;
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x000987D3 File Offset: 0x000969D3
	private static bool CanPlayMusic()
	{
		return UserSettings.MusicOn && UserSettings.MasterOn;
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x000987E3 File Offset: 0x000969E3
	public static void OnSettingsChanged()
	{
		BackgroundMusic.Toggle(BackgroundMusic.CanPlayMusic());
		if (!UserSettings.ShortSnippets)
		{
			BackgroundMusic.Playlist playlist = BackgroundMusic.cur_playlist;
			if (((playlist != null) ? playlist.key : null) == "ambience")
			{
				BackgroundMusic.PickRandomPlaylist();
				BackgroundMusic.PlayRandomTrack(false);
			}
		}
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x0009881E File Offset: 0x00096A1E
	public static void Stop()
	{
		StudioEventEmitter studioEventEmitter = BackgroundMusic.eventEmitter;
		if (studioEventEmitter != null)
		{
			studioEventEmitter.Stop();
		}
		BackgroundMusic.cur_playlist = null;
		BackgroundMusic.cur_group = null;
		BackgroundMusic.cur_track_counter = 0;
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00098842 File Offset: 0x00096A42
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x0009884A File Offset: 0x00096A4A
	public static void ResetDefaultMusicGroup()
	{
		BackgroundMusic.PickRandomMusicGroup();
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00098851 File Offset: 0x00096A51
	private void Update()
	{
		StudioEventEmitter studioEventEmitter = BackgroundMusic.eventEmitter;
		bool flag;
		if (studioEventEmitter == null)
		{
			flag = true;
		}
		else
		{
			EventInstance eventInstance = studioEventEmitter.EventInstance;
			flag = false;
		}
		if (flag)
		{
			return;
		}
		BackgroundMusic.OnUpdate();
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00098870 File Offset: 0x00096A70
	public static void OnUpdate()
	{
		if (BackgroundMusic.music_enabled)
		{
			BackgroundMusic.eventEmitter.EventInstance.getPlaybackState(out BackgroundMusic.playbackState);
			string @event = BackgroundMusic.eventEmitter.Event;
			if (BackgroundMusic.cur_playlist != null && !BackgroundMusic.cur_playlist.Validate())
			{
				BackgroundMusic.Stop();
				return;
			}
			if (BackgroundMusic.playbackState == PLAYBACK_STATE.SUSTAINING || BackgroundMusic.playbackState == PLAYBACK_STATE.STOPPED)
			{
				if (BackgroundMusic.cur_group == null)
				{
					BackgroundMusic.cur_group = BackgroundMusic.default_group;
				}
				if (BackgroundMusic.cur_group != null && UnityEngine.Time.unscaledTime > BackgroundMusic.resume_time)
				{
					BackgroundMusic.MusicGroup musicGroup = null;
					BackgroundMusic.MusicGroup musicGroup2 = BackgroundMusic.FindObligatoryGroup();
					if (musicGroup2 != null)
					{
						musicGroup = musicGroup2;
						if (BackgroundMusic.logging_enabled)
						{
							Debug.Log("Playing " + musicGroup.key + " because priority is obligatory");
						}
					}
					else if (BackgroundMusic.cur_group != BackgroundMusic.default_group)
					{
						musicGroup = BackgroundMusic.default_group;
					}
					if (musicGroup != null && musicGroup != BackgroundMusic.cur_group)
					{
						BackgroundMusic.cur_group = musicGroup;
						if (BackgroundMusic.logging_enabled)
						{
							Debug.Log("Picking random playlist from " + BackgroundMusic.cur_group.key);
						}
						BackgroundMusic.PickRandomPlaylist(BackgroundMusic.cur_group.playlists);
					}
					if (BackgroundMusic.cur_playlist == null || BackgroundMusic.cur_playlist.max_tracks_to_play <= BackgroundMusic.cur_track_counter)
					{
						if (BackgroundMusic.logging_enabled)
						{
							Debug.Log("Picking random playlist from " + BackgroundMusic.cur_group.key);
						}
						BackgroundMusic.PickRandomPlaylist(BackgroundMusic.cur_group.playlists);
					}
					if (BackgroundMusic.logging_enabled)
					{
						Debug.Log("Time to resume, playing random track");
					}
					BackgroundMusic.PlayRandomTrack(false);
				}
			}
			if (BackgroundMusic.playbackState != BackgroundMusic.last_state)
			{
				BackgroundMusic.last_state = BackgroundMusic.playbackState;
				if (BackgroundMusic.playbackState == PLAYBACK_STATE.STOPPING)
				{
					BackgroundMusic.finish_time = UnityEngine.Time.unscaledTime;
					BackgroundMusic.resume_time = BackgroundMusic.finish_time + Random.Range(BackgroundMusic.cur_playlist.min_pause_time, BackgroundMusic.cur_playlist.max_pause_time);
					if (BackgroundMusic.logging_enabled)
					{
						Debug.Log("Stopping " + @event);
					}
				}
			}
		}
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00098A44 File Offset: 0x00096C44
	private static BackgroundMusic.MusicGroup FindObligatoryGroup()
	{
		for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
		{
			BackgroundMusic.MusicGroup musicGroup = BackgroundMusic.music_groups[i];
			if (musicGroup.priority == "Obligatory")
			{
				return musicGroup;
			}
		}
		return null;
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00098A87 File Offset: 0x00096C87
	public static void PickRandomPlaylist()
	{
		if (BackgroundMusic.cur_group != null)
		{
			BackgroundMusic.PickRandomPlaylist(BackgroundMusic.cur_group.playlists);
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00098AA0 File Offset: 0x00096CA0
	public static void PickRandomPlaylist(List<BackgroundMusic.Playlist> cur_groups)
	{
		cur_groups = new List<BackgroundMusic.Playlist>(cur_groups);
		for (int i = cur_groups.Count - 1; i >= 0; i--)
		{
			if (!cur_groups[i].Validate())
			{
				cur_groups.RemoveAt(i);
			}
		}
		if (cur_groups.Count > 1)
		{
			BackgroundMusic.Playlist playlist = BackgroundMusic.cur_playlist;
			string b = (playlist != null) ? playlist.key : null;
			int index;
			string key;
			do
			{
				index = Random.Range(0, cur_groups.Count);
				key = cur_groups[index].key;
			}
			while (key == b);
			BackgroundMusic.SetPlaylist(cur_groups[index], null);
			return;
		}
		if (cur_groups.Count > 0)
		{
			BackgroundMusic.SetPlaylist(cur_groups[0], null);
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00098B44 File Offset: 0x00096D44
	public static void SetPlaylist(BackgroundMusic.Playlist pl, string src = null)
	{
		if (BackgroundMusic.cur_playlist == pl)
		{
			return;
		}
		BackgroundMusic.cur_track_counter = 0;
		BackgroundMusic.cur_playlist = pl;
		if (BackgroundMusic.logging_enabled && pl != null)
		{
			if (src != null)
			{
				Debug.Log("New playlist - " + src + " " + pl.key);
				return;
			}
			Debug.Log("New playlist - " + BackgroundMusic.cur_group.key + " " + pl.key);
		}
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00098BB4 File Offset: 0x00096DB4
	public static void SetMusicGroup(string key)
	{
		for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
		{
			if (BackgroundMusic.music_groups[i].key == key)
			{
				BackgroundMusic.cur_group = BackgroundMusic.music_groups[i];
				break;
			}
		}
		BackgroundMusic.PickRandomPlaylist();
		BackgroundMusic.PlayRandomTrack(false);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00098C0C File Offset: 0x00096E0C
	public static void PlayRandomTrack(bool no_group = false)
	{
		BackgroundMusic.Playlist playlist = BackgroundMusic.cur_playlist;
		if (((playlist != null) ? playlist.available_tracks : null) == null)
		{
			return;
		}
		if (BackgroundMusic.cur_playlist.available_tracks.Count == 0)
		{
			return;
		}
		if (BackgroundMusic.cur_playlist.available_tracks.Count > 1)
		{
			string b = null;
			if (BackgroundMusic.cur_playlist != null)
			{
				b = BackgroundMusic.cur_playlist.cur_track;
			}
			string text;
			if (BackgroundMusic.cur_playlist.available_tracks.Count > 1)
			{
				do
				{
					text = BackgroundMusic.cur_playlist.available_tracks[Random.Range(0, BackgroundMusic.cur_playlist.available_tracks.Count)];
				}
				while (text == b);
			}
			else
			{
				text = BackgroundMusic.cur_playlist.available_tracks[0];
			}
			if (no_group)
			{
				BackgroundMusic.PlayTrackAlone(text);
			}
			else
			{
				BackgroundMusic.PlayTrack(text);
			}
		}
		else
		{
			if (no_group)
			{
				BackgroundMusic.PlayTrackAlone(BackgroundMusic.cur_playlist.available_tracks[0]);
			}
			else
			{
				BackgroundMusic.PlayTrack(BackgroundMusic.cur_playlist.available_tracks[0]);
			}
			BackgroundMusic.cur_playlist.Reset();
		}
		BackgroundMusic.cur_track_counter++;
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00098D12 File Offset: 0x00096F12
	public static void PlayTrack(int new_track)
	{
		BackgroundMusic.PlayTrack(BackgroundMusic.cur_playlist.track_paths[new_track]);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00098D2C File Offset: 0x00096F2C
	public static void PlayTrack(string new_track)
	{
		for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
		{
			BackgroundMusic.music_groups[i].tracks_played_since++;
		}
		BackgroundMusic.cur_group.tracks_played_since = 0;
		BackgroundMusic.cur_playlist.cur_track = new_track;
		BackgroundMusic.eventEmitter.Stop();
		BackgroundMusic.eventEmitter.Event = new_track;
		BackgroundMusic.cur_playlist.available_tracks.Remove(new_track);
		BackgroundMusic.eventEmitter.Play();
		BackgroundMusic.start_time = UnityEngine.Time.unscaledTime;
		if (BackgroundMusic.logging_enabled)
		{
			Debug.Log("Playing " + new_track);
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x00098DD0 File Offset: 0x00096FD0
	public static void PlayTrackAlone(string new_track)
	{
		for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
		{
			BackgroundMusic.music_groups[i].tracks_played_since++;
		}
		BackgroundMusic.eventEmitter.Stop();
		BackgroundMusic.eventEmitter.Event = new_track;
		BackgroundMusic.eventEmitter.Play();
		BackgroundMusic.start_time = UnityEngine.Time.unscaledTime;
		if (BackgroundMusic.logging_enabled)
		{
			Debug.Log("Playing " + new_track);
		}
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00098E4A File Offset: 0x0009704A
	public static void Toggle(bool enable)
	{
		if (enable == BackgroundMusic.music_enabled)
		{
			return;
		}
		BackgroundMusic.music_enabled = enable;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00098E5B File Offset: 0x0009705B
	[ConsoleMethod("play_next_track", "")]
	public static void PlayNext()
	{
		BackgroundMusic.PlayRandomTrack(false);
		BackgroundMusic.OnUpdate();
		Debug.Log(BackgroundMusic.cur_playlist.cur_track);
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x00098E78 File Offset: 0x00097078
	[ConsoleMethod("bgm_track", "Get current music track")]
	public void GetCurrentTrack()
	{
		BackgroundMusic.Playlist playlist = BackgroundMusic.cur_playlist;
		string text = (playlist != null) ? playlist.cur_track : null;
		if (text != null)
		{
			Debug.Log(text);
			return;
		}
		Debug.Log("No music playing");
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00098EAC File Offset: 0x000970AC
	[ConsoleMethod("bgm_subgroup", "Sets which playlist the tracks should be picked from")]
	public void SetSubgroup(string group)
	{
		BackgroundMusic.Playlist playlist = this.GetPlaylist(BackgroundMusic.cur_group.playlists, group);
		if (playlist != null)
		{
			BackgroundMusic.SetPlaylist(playlist, null);
			BackgroundMusic.PlayNext();
		}
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00098EDC File Offset: 0x000970DC
	public static void OnTrigger(string key, string religion = null)
	{
		if (!BackgroundMusic.music_enabled)
		{
			return;
		}
		if (TitleMap.Get() != null)
		{
			return;
		}
		BackgroundMusic.Instance.Init();
		if (BackgroundMusic.logging_enabled)
		{
			Debug.Log("Trigger " + key + ", religion " + religion);
		}
		key = key.ToLowerInvariant();
		BackgroundMusic.MusicTrigger musicTrigger;
		if (!BackgroundMusic.triggers.TryGetValue(key, out musicTrigger))
		{
			if (BackgroundMusic.logging_enabled)
			{
				Debug.Log("Trigger failed: invalid trigger");
			}
			return;
		}
		if (UnityEngine.Time.unscaledTime < musicTrigger.next_play_time)
		{
			if (BackgroundMusic.logging_enabled)
			{
				Debug.Log(string.Format("Trigger failed: on cooldown for another {0} seconds", musicTrigger.next_play_time - UnityEngine.Time.unscaledTime));
			}
			return;
		}
		if (((BackgroundMusic.cur_playlist != null && !BackgroundMusic.cur_playlist.can_be_interrupted) || !musicTrigger.interrupt) && BackgroundMusic.playbackState == PLAYBACK_STATE.PLAYING)
		{
			if (BackgroundMusic.logging_enabled)
			{
				Debug.Log("Trigger failed: still playing music");
			}
			return;
		}
		float num = UnityEngine.Time.unscaledTime - BackgroundMusic.finish_time;
		float num2 = BackgroundMusic.resume_time - BackgroundMusic.finish_time;
		if (BackgroundMusic.cur_group != null && !musicTrigger.interrupt && num / num2 < BackgroundMusic.cur_group.snippet_percentage)
		{
			if (BackgroundMusic.logging_enabled)
			{
				Debug.Log("Trigger failed: too early after pause");
			}
			return;
		}
		if (religion != null)
		{
			religion = religion.ToLowerInvariant();
			if (musicTrigger.playlist == null)
			{
				BackgroundMusic.MusicGroup musicGroup = null;
				for (int i = 0; i < BackgroundMusic.music_groups.Count; i++)
				{
					BackgroundMusic.MusicGroup musicGroup2 = BackgroundMusic.music_groups[i];
					bool flag = false;
					for (int j = 0; j < musicGroup2.religions.Length; j++)
					{
						if (musicGroup2.religions[j] == religion)
						{
							flag = true;
							break;
						}
					}
					if (flag && BackgroundMusic.priorities[musicGroup2.priority] >= BackgroundMusic.priorities[musicTrigger.priority])
					{
						musicGroup = musicGroup2;
						break;
					}
				}
				if (musicGroup == null)
				{
					if (BackgroundMusic.logging_enabled)
					{
						Debug.Log("Trigger failed: no music group with high enough priority at the moment");
					}
					return;
				}
				BackgroundMusic.cur_group = musicGroup;
				BackgroundMusic.SetPlaylist(null, null);
				for (int k = 0; k < BackgroundMusic.cur_group.playlists.Count; k++)
				{
					BackgroundMusic.Playlist playlist = BackgroundMusic.cur_group.playlists[k];
					if (playlist.key == "ambience")
					{
						BackgroundMusic.SetPlaylist(playlist, null);
						break;
					}
				}
				if (BackgroundMusic.cur_playlist == null)
				{
					BackgroundMusic.PickRandomPlaylist();
				}
				BackgroundMusic.PlayRandomTrack(false);
			}
			else
			{
				for (int l = 0; l < musicTrigger.playlists.Count; l++)
				{
					bool flag2 = false;
					BackgroundMusic.Playlist playlist2 = musicTrigger.playlists[l];
					if (playlist2.religions != null)
					{
						for (int m = 0; m < playlist2.religions.Length; m++)
						{
							if (playlist2.religions[m] == religion)
							{
								flag2 = true;
								break;
							}
						}
					}
					else
					{
						flag2 = true;
					}
					if (flag2)
					{
						BackgroundMusic.SetPlaylist(playlist2, null);
						BackgroundMusic.PlayRandomTrack(true);
						break;
					}
				}
			}
		}
		else
		{
			if (musicTrigger.playlist == null)
			{
				return;
			}
			BackgroundMusic.SetPlaylist(musicTrigger.playlist, musicTrigger.key);
			BackgroundMusic.PlayRandomTrack(true);
		}
		if (musicTrigger.cooldown_max != 0f)
		{
			musicTrigger.next_play_time = UnityEngine.Time.unscaledTime + Random.Range(musicTrigger.cooldown_min, musicTrigger.cooldown_max);
			return;
		}
		musicTrigger.next_play_time = UnityEngine.Time.unscaledTime + musicTrigger.cooldown_min;
	}

	// Token: 0x04000A5B RID: 2651
	public static StudioEventEmitter eventEmitter;

	// Token: 0x04000A5C RID: 2652
	private static int cur_track_counter = 0;

	// Token: 0x04000A5D RID: 2653
	private static BackgroundMusic.MusicGroup default_group = null;

	// Token: 0x04000A5E RID: 2654
	private static BackgroundMusic.MusicGroup cur_group = null;

	// Token: 0x04000A5F RID: 2655
	private static float start_time = 0f;

	// Token: 0x04000A60 RID: 2656
	private static float finish_time = 0f;

	// Token: 0x04000A61 RID: 2657
	private static float resume_time = 0f;

	// Token: 0x04000A62 RID: 2658
	private static BackgroundMusic.Playlist cur_playlist = null;

	// Token: 0x04000A63 RID: 2659
	private static bool music_enabled = false;

	// Token: 0x04000A64 RID: 2660
	public static bool logging_enabled;

	// Token: 0x04000A65 RID: 2661
	public static Dictionary<string, float> priorities = new Dictionary<string, float>();

	// Token: 0x04000A66 RID: 2662
	private static BackgroundMusic _instance;

	// Token: 0x04000A67 RID: 2663
	private bool init;

	// Token: 0x04000A68 RID: 2664
	private static Dictionary<string, BackgroundMusic.MusicTrigger> triggers = new Dictionary<string, BackgroundMusic.MusicTrigger>();

	// Token: 0x04000A69 RID: 2665
	public static List<BackgroundMusic.MusicGroup> music_groups = new List<BackgroundMusic.MusicGroup>();

	// Token: 0x04000A6A RID: 2666
	private static PLAYBACK_STATE last_state = PLAYBACK_STATE.PLAYING;

	// Token: 0x04000A6B RID: 2667
	private static PLAYBACK_STATE playbackState = PLAYBACK_STATE.PLAYING;

	// Token: 0x02000634 RID: 1588
	public class Playlist
	{
		// Token: 0x06004729 RID: 18217 RVA: 0x00212BC4 File Offset: 0x00210DC4
		public void Reset()
		{
			this.available_tracks.Clear();
			this.available_tracks.AddRange(this.track_paths);
		}

		// Token: 0x0600472A RID: 18218 RVA: 0x00212BE4 File Offset: 0x00210DE4
		public bool Validate()
		{
			if (BattleViewLoader.sm_InTrasition || LoadingScreen.IsShown())
			{
				return false;
			}
			bool flag = BattleMap.Get() != null;
			bool flag2 = BattleMap.battle != null && BattleMap.battle.IsValid() && BattleMap.battle.stage != Logic.Battle.Stage.EnteringBattle && !BattleMap.battle.battle_map_finished;
			string a = this.key;
			if (a == "battleview")
			{
				return flag && flag2;
			}
			if (!(a == "ambience"))
			{
				return !flag || this.works_bv;
			}
			return UserSettings.ShortSnippets && !flag;
		}

		// Token: 0x0400348A RID: 13450
		public string key;

		// Token: 0x0400348B RID: 13451
		public bool can_be_interrupted = true;

		// Token: 0x0400348C RID: 13452
		public List<string> track_paths = new List<string>();

		// Token: 0x0400348D RID: 13453
		public List<string> available_tracks = new List<string>();

		// Token: 0x0400348E RID: 13454
		public string cur_track;

		// Token: 0x0400348F RID: 13455
		public int max_tracks_to_play = 2;

		// Token: 0x04003490 RID: 13456
		public float min_pause_time = 40f;

		// Token: 0x04003491 RID: 13457
		public float max_pause_time = 40f;

		// Token: 0x04003492 RID: 13458
		public bool works_bv;

		// Token: 0x04003493 RID: 13459
		public string[] religions;
	}

	// Token: 0x02000635 RID: 1589
	public class MusicGroup
	{
		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x0600472C RID: 18220 RVA: 0x00212CD4 File Offset: 0x00210ED4
		public string priority
		{
			get
			{
				string result = "";
				foreach (KeyValuePair<string, float> keyValuePair in BackgroundMusic.priorities)
				{
					result = keyValuePair.Key;
					if ((float)this.tracks_played_since <= keyValuePair.Value)
					{
						return keyValuePair.Key;
					}
				}
				return result;
			}
		}

		// Token: 0x04003494 RID: 13460
		public int tracks_played_since;

		// Token: 0x04003495 RID: 13461
		public string key;

		// Token: 0x04003496 RID: 13462
		public string[] religions;

		// Token: 0x04003497 RID: 13463
		public float snippet_percentage = 0.75f;

		// Token: 0x04003498 RID: 13464
		public List<BackgroundMusic.Playlist> playlists = new List<BackgroundMusic.Playlist>();
	}

	// Token: 0x02000636 RID: 1590
	public class MusicTrigger
	{
		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x0600472E RID: 18222 RVA: 0x00212D6A File Offset: 0x00210F6A
		public BackgroundMusic.Playlist playlist
		{
			get
			{
				if (this.playlists.Count > 0)
				{
					return this.playlists[0];
				}
				return null;
			}
		}

		// Token: 0x04003499 RID: 13465
		public string key;

		// Token: 0x0400349A RID: 13466
		public string priority;

		// Token: 0x0400349B RID: 13467
		public bool interrupt;

		// Token: 0x0400349C RID: 13468
		public List<BackgroundMusic.Playlist> playlists = new List<BackgroundMusic.Playlist>();

		// Token: 0x0400349D RID: 13469
		public float cooldown_min;

		// Token: 0x0400349E RID: 13470
		public float cooldown_max;

		// Token: 0x0400349F RID: 13471
		public float next_play_time;
	}
}
