using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000176 RID: 374
public class AudioController
{
	// Token: 0x06001314 RID: 4884 RVA: 0x000C71E8 File Offset: 0x000C53E8
	static AudioController()
	{
		AudioController.Init();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x000C71F9 File Offset: 0x000C53F9
	private static void Init()
	{
		AudioController.m_MasterMixer = (Resources.Load("NonVrAudioMixer") as AudioMixer);
		AudioController.ExtractMixers();
		AudioController.SetSoundEfectsVolume(UserSettings.VoiceVolume);
		AudioController.SetMusicEfectsVolume(UserSettings.MusicVolume);
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x000C7228 File Offset: 0x000C5428
	public static void SetSoundEfectsVolume(float newValue)
	{
		float value = Mathf.Lerp(-50f, 10f, newValue);
		AudioController.m_MasterMixer.SetFloat("MasterSoundEffectsVolume", value);
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x000C7257 File Offset: 0x000C5457
	public static float GetSoundEfectsVolume()
	{
		return UserSettings.VoiceVolume;
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000C7260 File Offset: 0x000C5460
	public static void SetMusicEfectsVolume(float newValue)
	{
		float value = Mathf.Lerp(-50f, 10f, newValue);
		AudioController.m_MasterMixer.SetFloat("MusicVolume", value);
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x000C728F File Offset: 0x000C548F
	public static float GetMusicVolume()
	{
		return UserSettings.MusicVolume;
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x000C7298 File Offset: 0x000C5498
	public static void SetUIEfectsVolume(float newValue)
	{
		float value = Mathf.Lerp(-50f, 10f, newValue);
		AudioController.m_MasterMixer.SetFloat("UIVolume", value);
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000C72C7 File Offset: 0x000C54C7
	public static float GetUIEfectsVolume()
	{
		return 1f;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x000C72CE File Offset: 0x000C54CE
	public static AudioMixerGroup GetMixerGroup(AudioController.MixerGroup mixerGroup)
	{
		if (AudioController.m_AudioMixerGroup.ContainsKey(mixerGroup))
		{
			return AudioController.m_AudioMixerGroup[mixerGroup];
		}
		return null;
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x000023FD File Offset: 0x000005FD
	internal static void SaveAudioSettings()
	{
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000C72EC File Offset: 0x000C54EC
	private static void ExtractMixers()
	{
		foreach (object obj in Enum.GetValues(typeof(AudioController.MixerGroup)))
		{
			AudioController.MixerGroup key = (AudioController.MixerGroup)obj;
			string text = key.ToString();
			foreach (AudioMixerGroup audioMixerGroup in AudioController.m_MasterMixer.FindMatchingGroups(text))
			{
				if (audioMixerGroup.name == text)
				{
					AudioController.m_AudioMixerGroup[key] = audioMixerGroup;
				}
			}
		}
	}

	// Token: 0x04000CB3 RID: 3251
	private static Dictionary<AudioController.MixerGroup, AudioMixerGroup> m_AudioMixerGroup = new Dictionary<AudioController.MixerGroup, AudioMixerGroup>();

	// Token: 0x04000CB4 RID: 3252
	private static AudioMixer m_MasterMixer;

	// Token: 0x04000CB5 RID: 3253
	private const float MIN_VOLUME_DB = -50f;

	// Token: 0x04000CB6 RID: 3254
	private const float MAX_VOLUME_DB = 10f;

	// Token: 0x020006A0 RID: 1696
	public enum MixerGroup
	{
		// Token: 0x04003632 RID: 13874
		Game,
		// Token: 0x04003633 RID: 13875
		Ambient,
		// Token: 0x04003634 RID: 13876
		Music,
		// Token: 0x04003635 RID: 13877
		UI
	}
}
