using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using SCPE;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000179 RID: 377
public static class UserSettings
{
	// Token: 0x06001334 RID: 4916 RVA: 0x000C782C File Offset: 0x000C5A2C
	public static void RecordSettingAnalyticsImmediately(string id, string old_value, string new_value)
	{
		Vars vars = new Vars();
		vars.Set<string>("settingName", id);
		vars.Set<string>("oldSettingValue", old_value);
		vars.Set<string>("newSettingValue", new_value);
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.NotifyListeners("analytics_setting_changed", vars);
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x000C787C File Offset: 0x000C5A7C
	public static void OnChangeAnalytics(string id, Value old_value, Value new_value, bool immediate)
	{
		if (immediate && old_value != new_value)
		{
			UserSettings.RecordSettingAnalyticsImmediately(id, old_value.ToString(), new_value.ToString());
			return;
		}
		UserSettings.SettingChangeData settingChangeData;
		if (!UserSettings.analyticsSettingsData.TryGetValue(id, out settingChangeData))
		{
			if (old_value != new_value)
			{
				settingChangeData = new UserSettings.SettingChangeData
				{
					new_value = new_value,
					old_value = old_value
				};
				UserSettings.analyticsSettingsData[id] = settingChangeData;
			}
			return;
		}
		if (settingChangeData.old_value == new_value)
		{
			UserSettings.analyticsSettingsData.Remove(id);
			return;
		}
		UserSettings.analyticsSettingsData[id] = new UserSettings.SettingChangeData
		{
			old_value = settingChangeData.old_value,
			new_value = new_value
		};
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06001336 RID: 4918 RVA: 0x000C7939 File Offset: 0x000C5B39
	// (set) Token: 0x06001337 RID: 4919 RVA: 0x000C7940 File Offset: 0x000C5B40
	public static int ScreenWidth { get; private set; }

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06001338 RID: 4920 RVA: 0x000C7948 File Offset: 0x000C5B48
	// (set) Token: 0x06001339 RID: 4921 RVA: 0x000C794F File Offset: 0x000C5B4F
	public static int ScreenHeight { get; private set; }

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x0600133A RID: 4922 RVA: 0x000C7957 File Offset: 0x000C5B57
	// (set) Token: 0x0600133B RID: 4923 RVA: 0x000C795E File Offset: 0x000C5B5E
	public static int ScreenRefreshRate { get; private set; }

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x0600133C RID: 4924 RVA: 0x000C7966 File Offset: 0x000C5B66
	// (set) Token: 0x0600133D RID: 4925 RVA: 0x000C796D File Offset: 0x000C5B6D
	public static FullScreenMode FullScreenMode { get; private set; }

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x0600133E RID: 4926 RVA: 0x000C7975 File Offset: 0x000C5B75
	// (set) Token: 0x0600133F RID: 4927 RVA: 0x000C797C File Offset: 0x000C5B7C
	public static string QualityLevelStr { get; private set; }

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06001340 RID: 4928 RVA: 0x000C7984 File Offset: 0x000C5B84
	// (set) Token: 0x06001341 RID: 4929 RVA: 0x000C798B File Offset: 0x000C5B8B
	public static bool PresetProcessing { get; private set; }

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06001342 RID: 4930 RVA: 0x000C7993 File Offset: 0x000C5B93
	// (set) Token: 0x06001343 RID: 4931 RVA: 0x000C799A File Offset: 0x000C5B9A
	public static string TextureSizeStr { get; private set; }

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06001344 RID: 4932 RVA: 0x000C79A2 File Offset: 0x000C5BA2
	// (set) Token: 0x06001345 RID: 4933 RVA: 0x000C79A9 File Offset: 0x000C5BA9
	public static string TextureQualityStr { get; private set; }

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06001346 RID: 4934 RVA: 0x000C79B1 File Offset: 0x000C5BB1
	// (set) Token: 0x06001347 RID: 4935 RVA: 0x000C79B8 File Offset: 0x000C5BB8
	public static string ShadowsQualityStr { get; private set; }

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06001348 RID: 4936 RVA: 0x000C79C0 File Offset: 0x000C5BC0
	// (set) Token: 0x06001349 RID: 4937 RVA: 0x000C79C7 File Offset: 0x000C5BC7
	public static string AntialiasingModeStr { get; private set; }

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x0600134A RID: 4938 RVA: 0x000C79CF File Offset: 0x000C5BCF
	// (set) Token: 0x0600134B RID: 4939 RVA: 0x000C79D6 File Offset: 0x000C5BD6
	public static string SSRTStr { get; private set; }

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x0600134C RID: 4940 RVA: 0x000C79DE File Offset: 0x000C5BDE
	// (set) Token: 0x0600134D RID: 4941 RVA: 0x000C79E5 File Offset: 0x000C5BE5
	public static bool VSync { get; private set; }

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600134E RID: 4942 RVA: 0x000C79ED File Offset: 0x000C5BED
	// (set) Token: 0x0600134F RID: 4943 RVA: 0x000C79F4 File Offset: 0x000C5BF4
	public static bool HasFpsLimit { get; private set; }

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06001350 RID: 4944 RVA: 0x000C79FC File Offset: 0x000C5BFC
	// (set) Token: 0x06001351 RID: 4945 RVA: 0x000C7A03 File Offset: 0x000C5C03
	public static string FpsLimit { get; private set; }

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06001352 RID: 4946 RVA: 0x000C7A0B File Offset: 0x000C5C0B
	// (set) Token: 0x06001353 RID: 4947 RVA: 0x000C7A12 File Offset: 0x000C5C12
	public static bool Reflecitons { get; private set; }

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06001354 RID: 4948 RVA: 0x000C7A1A File Offset: 0x000C5C1A
	// (set) Token: 0x06001355 RID: 4949 RVA: 0x000C7A21 File Offset: 0x000C5C21
	public static bool SSAO { get; private set; }

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06001356 RID: 4950 RVA: 0x000C7A29 File Offset: 0x000C5C29
	// (set) Token: 0x06001357 RID: 4951 RVA: 0x000C7A30 File Offset: 0x000C5C30
	public static bool Fog { get; private set; }

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06001358 RID: 4952 RVA: 0x000C7A38 File Offset: 0x000C5C38
	// (set) Token: 0x06001359 RID: 4953 RVA: 0x000C7A3F File Offset: 0x000C5C3F
	public static bool DepthOfField { get; private set; }

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x0600135A RID: 4954 RVA: 0x000C7A47 File Offset: 0x000C5C47
	// (set) Token: 0x0600135B RID: 4955 RVA: 0x000C7A4E File Offset: 0x000C5C4E
	public static bool Bloom { get; private set; }

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x0600135C RID: 4956 RVA: 0x000C7A56 File Offset: 0x000C5C56
	// (set) Token: 0x0600135D RID: 4957 RVA: 0x000C7A5D File Offset: 0x000C5C5D
	public static string Difficulty { get; set; }

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x0600135E RID: 4958 RVA: 0x000C7A65 File Offset: 0x000C5C65
	// (set) Token: 0x0600135F RID: 4959 RVA: 0x000C7A6C File Offset: 0x000C5C6C
	public static string RemindersLevelStr { get; set; }

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06001360 RID: 4960 RVA: 0x000C7A74 File Offset: 0x000C5C74
	// (set) Token: 0x06001361 RID: 4961 RVA: 0x000C7A7B File Offset: 0x000C5C7B
	public static string AudioConfigurationStr { get; set; }

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06001362 RID: 4962 RVA: 0x000C7A83 File Offset: 0x000C5C83
	// (set) Token: 0x06001363 RID: 4963 RVA: 0x000C7A8A File Offset: 0x000C5C8A
	public static int AudioConfigurationFMODParam { get; set; }

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06001364 RID: 4964 RVA: 0x000C7A92 File Offset: 0x000C5C92
	// (set) Token: 0x06001365 RID: 4965 RVA: 0x000C7A99 File Offset: 0x000C5C99
	public static float MasterVolume { get; private set; }

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06001366 RID: 4966 RVA: 0x000C7AA1 File Offset: 0x000C5CA1
	// (set) Token: 0x06001367 RID: 4967 RVA: 0x000C7AA8 File Offset: 0x000C5CA8
	public static float VolumeOutOfFocus { get; private set; }

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06001368 RID: 4968 RVA: 0x000C7AB0 File Offset: 0x000C5CB0
	// (set) Token: 0x06001369 RID: 4969 RVA: 0x000C7AB7 File Offset: 0x000C5CB7
	public static float VolumeOutOfFocusPaused { get; private set; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x0600136A RID: 4970 RVA: 0x000C7ABF File Offset: 0x000C5CBF
	// (set) Token: 0x0600136B RID: 4971 RVA: 0x000C7AC6 File Offset: 0x000C5CC6
	public static float MusicVolume { get; private set; }

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x0600136C RID: 4972 RVA: 0x000C7ACE File Offset: 0x000C5CCE
	// (set) Token: 0x0600136D RID: 4973 RVA: 0x000C7AD5 File Offset: 0x000C5CD5
	public static float VoiceVolume { get; private set; }

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x0600136E RID: 4974 RVA: 0x000C7ADD File Offset: 0x000C5CDD
	// (set) Token: 0x0600136F RID: 4975 RVA: 0x000C7AE4 File Offset: 0x000C5CE4
	public static float NarratorVoiceVolume { get; private set; }

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06001370 RID: 4976 RVA: 0x000C7AEC File Offset: 0x000C5CEC
	// (set) Token: 0x06001371 RID: 4977 RVA: 0x000C7AF3 File Offset: 0x000C5CF3
	public static float KnightVoiceVolume { get; private set; }

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06001372 RID: 4978 RVA: 0x000C7AFB File Offset: 0x000C5CFB
	// (set) Token: 0x06001373 RID: 4979 RVA: 0x000C7B02 File Offset: 0x000C5D02
	public static float UnitVoiceVolume { get; private set; }

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06001374 RID: 4980 RVA: 0x000C7B0A File Offset: 0x000C5D0A
	// (set) Token: 0x06001375 RID: 4981 RVA: 0x000C7B11 File Offset: 0x000C5D11
	public static float SoundEffectsVolume { get; private set; }

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06001376 RID: 4982 RVA: 0x000C7B19 File Offset: 0x000C5D19
	// (set) Token: 0x06001377 RID: 4983 RVA: 0x000C7B20 File Offset: 0x000C5D20
	public static float AmbienceVolume { get; private set; }

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06001378 RID: 4984 RVA: 0x000C7B28 File Offset: 0x000C5D28
	// (set) Token: 0x06001379 RID: 4985 RVA: 0x000C7B2F File Offset: 0x000C5D2F
	public static int MessageQueCap { get; private set; }

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x0600137A RID: 4986 RVA: 0x000C7B37 File Offset: 0x000C5D37
	// (set) Token: 0x0600137B RID: 4987 RVA: 0x000C7B3E File Offset: 0x000C5D3E
	public static bool MasterOn { get; private set; }

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x0600137C RID: 4988 RVA: 0x000C7B46 File Offset: 0x000C5D46
	// (set) Token: 0x0600137D RID: 4989 RVA: 0x000C7B4D File Offset: 0x000C5D4D
	public static bool VolumeOutOfFocusOn { get; private set; }

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x0600137E RID: 4990 RVA: 0x000C7B55 File Offset: 0x000C5D55
	// (set) Token: 0x0600137F RID: 4991 RVA: 0x000C7B5C File Offset: 0x000C5D5C
	public static bool VolumeOutOfFocusPausedOn { get; private set; }

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06001380 RID: 4992 RVA: 0x000C7B64 File Offset: 0x000C5D64
	// (set) Token: 0x06001381 RID: 4993 RVA: 0x000C7B6B File Offset: 0x000C5D6B
	public static bool MusicOn { get; private set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06001382 RID: 4994 RVA: 0x000C7B73 File Offset: 0x000C5D73
	// (set) Token: 0x06001383 RID: 4995 RVA: 0x000C7B7A File Offset: 0x000C5D7A
	public static bool VoiceOn { get; private set; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06001384 RID: 4996 RVA: 0x000C7B82 File Offset: 0x000C5D82
	// (set) Token: 0x06001385 RID: 4997 RVA: 0x000C7B89 File Offset: 0x000C5D89
	public static bool NarratorVoiceOn { get; private set; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06001386 RID: 4998 RVA: 0x000C7B91 File Offset: 0x000C5D91
	// (set) Token: 0x06001387 RID: 4999 RVA: 0x000C7B98 File Offset: 0x000C5D98
	public static bool KnightVoiceOn { get; private set; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06001388 RID: 5000 RVA: 0x000C7BA0 File Offset: 0x000C5DA0
	// (set) Token: 0x06001389 RID: 5001 RVA: 0x000C7BA7 File Offset: 0x000C5DA7
	public static bool UnitVoiceOn { get; private set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x0600138A RID: 5002 RVA: 0x000C7BAF File Offset: 0x000C5DAF
	// (set) Token: 0x0600138B RID: 5003 RVA: 0x000C7BB6 File Offset: 0x000C5DB6
	public static bool SoundEffectsOn { get; private set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x0600138C RID: 5004 RVA: 0x000C7BBE File Offset: 0x000C5DBE
	// (set) Token: 0x0600138D RID: 5005 RVA: 0x000C7BC5 File Offset: 0x000C5DC5
	public static bool AmbienceOn { get; private set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x0600138E RID: 5006 RVA: 0x000C7BCD File Offset: 0x000C5DCD
	// (set) Token: 0x0600138F RID: 5007 RVA: 0x000C7BD4 File Offset: 0x000C5DD4
	public static bool SelectionSoundsOn { get; private set; }

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06001390 RID: 5008 RVA: 0x000C7BDC File Offset: 0x000C5DDC
	// (set) Token: 0x06001391 RID: 5009 RVA: 0x000C7BE3 File Offset: 0x000C5DE3
	public static bool ShortSnippets { get; private set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06001392 RID: 5010 RVA: 0x000C7BEB File Offset: 0x000C5DEB
	// (set) Token: 0x06001393 RID: 5011 RVA: 0x000C7BF2 File Offset: 0x000C5DF2
	public static bool EdgeScroll { get; private set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06001394 RID: 5012 RVA: 0x000C7BFA File Offset: 0x000C5DFA
	// (set) Token: 0x06001395 RID: 5013 RVA: 0x000C7C01 File Offset: 0x000C5E01
	public static float EdgeScrollSpeed { get; private set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06001396 RID: 5014 RVA: 0x000C7C09 File Offset: 0x000C5E09
	// (set) Token: 0x06001397 RID: 5015 RVA: 0x000C7C10 File Offset: 0x000C5E10
	public static float PanSpeed { get; private set; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06001398 RID: 5016 RVA: 0x000C7C18 File Offset: 0x000C5E18
	// (set) Token: 0x06001399 RID: 5017 RVA: 0x000C7C1F File Offset: 0x000C5E1F
	public static bool AutoSave { get; private set; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x0600139A RID: 5018 RVA: 0x000C7C27 File Offset: 0x000C5E27
	// (set) Token: 0x0600139B RID: 5019 RVA: 0x000C7C2E File Offset: 0x000C5E2E
	public static int AutoSaveCount { get; private set; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600139C RID: 5020 RVA: 0x000C7C36 File Offset: 0x000C5E36
	// (set) Token: 0x0600139D RID: 5021 RVA: 0x000C7C3D File Offset: 0x000C5E3D
	public static bool AutoSaveInterval { get; private set; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600139E RID: 5022 RVA: 0x000C7C45 File Offset: 0x000C5E45
	// (set) Token: 0x0600139F RID: 5023 RVA: 0x000C7C4C File Offset: 0x000C5E4C
	public static bool PauseOnOutOfFocus { get; private set; }

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x060013A0 RID: 5024 RVA: 0x000C7C54 File Offset: 0x000C5E54
	// (set) Token: 0x060013A1 RID: 5025 RVA: 0x000C7C5B File Offset: 0x000C5E5B
	public static bool ProfanityFilter { get; private set; }

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x060013A2 RID: 5026 RVA: 0x000C7C63 File Offset: 0x000C5E63
	// (set) Token: 0x060013A3 RID: 5027 RVA: 0x000C7C6A File Offset: 0x000C5E6A
	public static int MaxRagdollCount { get; private set; }

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x060013A4 RID: 5028 RVA: 0x000C7C72 File Offset: 0x000C5E72
	// (set) Token: 0x060013A5 RID: 5029 RVA: 0x000C7C79 File Offset: 0x000C5E79
	public static float MaxRagdollDuration { get; private set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060013A6 RID: 5030 RVA: 0x000C7C81 File Offset: 0x000C5E81
	// (set) Token: 0x060013A7 RID: 5031 RVA: 0x000C7C88 File Offset: 0x000C5E88
	public static bool ClickableArmyPVFigures { get; private set; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060013A8 RID: 5032 RVA: 0x000C7C90 File Offset: 0x000C5E90
	// (set) Token: 0x060013A9 RID: 5033 RVA: 0x000C7C97 File Offset: 0x000C5E97
	public static bool ClickableSettlementPVFigures { get; private set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x060013AA RID: 5034 RVA: 0x000C7C9F File Offset: 0x000C5E9F
	// (set) Token: 0x060013AB RID: 5035 RVA: 0x000C7CA6 File Offset: 0x000C5EA6
	public static bool ClickableBattlePVFigures { get; private set; }

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060013AC RID: 5036 RVA: 0x000C7CAE File Offset: 0x000C5EAE
	// (set) Token: 0x060013AD RID: 5037 RVA: 0x000C7CB5 File Offset: 0x000C5EB5
	public static bool ClickableNameplates { get; private set; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060013AE RID: 5038 RVA: 0x000C7CBD File Offset: 0x000C5EBD
	// (set) Token: 0x060013AF RID: 5039 RVA: 0x000C7CC4 File Offset: 0x000C5EC4
	public static bool NameplatesEnabledWV { get; private set; }

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060013B0 RID: 5040 RVA: 0x000C7CCC File Offset: 0x000C5ECC
	// (set) Token: 0x060013B1 RID: 5041 RVA: 0x000C7CD3 File Offset: 0x000C5ED3
	public static bool NameplatesEnabledPV { get; private set; }

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060013B2 RID: 5042 RVA: 0x000C7CDB File Offset: 0x000C5EDB
	// (set) Token: 0x060013B3 RID: 5043 RVA: 0x000C7CE2 File Offset: 0x000C5EE2
	public static int PVFigureFilter { get; private set; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x060013B4 RID: 5044 RVA: 0x000C7CEA File Offset: 0x000C5EEA
	// (set) Token: 0x060013B5 RID: 5045 RVA: 0x000C7CF1 File Offset: 0x000C5EF1
	public static bool TutorialMessages { get; private set; }

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x060013B6 RID: 5046 RVA: 0x000C7CF9 File Offset: 0x000C5EF9
	// (set) Token: 0x060013B7 RID: 5047 RVA: 0x000C7D00 File Offset: 0x000C5F00
	public static bool TutorialMessagesBattleView { get; private set; }

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x060013B8 RID: 5048 RVA: 0x000C7D08 File Offset: 0x000C5F08
	// (set) Token: 0x060013B9 RID: 5049 RVA: 0x000C7D0F File Offset: 0x000C5F0F
	public static bool DataCollection { get; private set; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060013BA RID: 5050 RVA: 0x000C7D17 File Offset: 0x000C5F17
	// (set) Token: 0x060013BB RID: 5051 RVA: 0x000C7D1E File Offset: 0x000C5F1E
	public static bool DataCollectionAsked { get; private set; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060013BC RID: 5052 RVA: 0x000C7D26 File Offset: 0x000C5F26
	// (set) Token: 0x060013BD RID: 5053 RVA: 0x000C7D2D File Offset: 0x000C5F2D
	public static string ActiveMod { get; private set; }

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060013BE RID: 5054 RVA: 0x000C7D38 File Offset: 0x000C5F38
	// (remove) Token: 0x060013BF RID: 5055 RVA: 0x000C7D6C File Offset: 0x000C5F6C
	public static event Action OnSettingChange;

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060013C0 RID: 5056 RVA: 0x000C7DA0 File Offset: 0x000C5FA0
	public static string Language
	{
		get
		{
			global::Defs defs = global::Defs.Get(false);
			if (defs != null)
			{
				defs.LoadLanguagesDef(false);
			}
			if (!Application.isPlaying)
			{
				return global::Defs.def_language;
			}
			if (!string.IsNullOrEmpty(global::Defs.set_language))
			{
				return global::Defs.set_language;
			}
			UserSettings.SettingData setting = UserSettings.GetSetting("language");
			if (setting != null && UserSettings.ValidateLanguageKey(setting.value))
			{
				return setting.value;
			}
			string fallbackLanguageKey = UserSettings.GetFallbackLanguageKey();
			if (setting != null)
			{
				setting.new_value = fallbackLanguageKey;
				setting.Apply(true);
			}
			return fallbackLanguageKey;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x060013C1 RID: 5057 RVA: 0x000C7E28 File Offset: 0x000C6028
	public static string AudioLanguage
	{
		get
		{
			global::Defs defs = global::Defs.Get(false);
			if (defs != null)
			{
				defs.LoadLanguagesDef(false);
			}
			if (!Application.isPlaying)
			{
				return global::Defs.def_language;
			}
			if (!string.IsNullOrEmpty(global::Defs.set_language))
			{
				return global::Defs.set_language;
			}
			UserSettings.SettingData setting = UserSettings.GetSetting("audio_language");
			if (setting != null && UserSettings.ValidateAudioLanguageKey(setting.value))
			{
				return setting.value;
			}
			string fallbackAudioLanguageKey = UserSettings.GetFallbackAudioLanguageKey();
			if (setting != null)
			{
				setting.new_value = fallbackAudioLanguageKey;
				setting.Apply(true);
			}
			return fallbackAudioLanguageKey;
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x000C7EB0 File Offset: 0x000C60B0
	static UserSettings()
	{
		UserSettings.Init();
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x000023FD File Offset: 0x000005FD
	public static void InitSettings()
	{
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x000C7F3A File Offset: 0x000C613A
	private static void Init()
	{
		UserSettings.LoadSettings();
		UserSettings.CheckAutoDetect(false);
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x000C7F47 File Offset: 0x000C6147
	private static void RunBenchmark()
	{
		Debug.Log("Started Benchmark");
		Benchmark instance = Benchmark.Instance;
		if (instance == null)
		{
			return;
		}
		instance.Run(delegate(Benchmark.BenchmarkResult result)
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("auto_detected_gpu");
			UserSettings.SettingData setting2 = UserSettings.GetSetting("auto_detected_cpu");
			if (setting != null)
			{
				setting.ApplyValue(result.gpu_rating);
			}
			if (setting2 != null)
			{
				setting2.ApplyValue(result.cpu_rating);
			}
			Debug.Log(string.Format("Finished Bechmark. CPU:{0}, GPU:{1}", result.cpu_rating, result.gpu_rating));
		});
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x000C7F84 File Offset: 0x000C6184
	public static void CheckAutoDetect(bool force = false)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("auto_detected_gpu");
		UserSettings.SettingData setting2 = UserSettings.GetSetting("auto_detected_cpu");
		if (force || (setting != null && setting.value == -1) || (setting != null && setting.value == -1))
		{
			if (Benchmark.LastResult != null)
			{
				if (setting != null)
				{
					setting.ApplyValue(Benchmark.LastResult.gpu_rating);
				}
				if (setting2 != null)
				{
					setting2.ApplyValue(Benchmark.LastResult.cpu_rating);
					return;
				}
			}
			else
			{
				if (Benchmark.Instance != null)
				{
					UserSettings.RunBenchmark();
					return;
				}
				if (Application.isPlaying)
				{
					Debug.Log("Created Wait Benchmark Creation");
					new GameObject("wait_benchmark_creation").AddComponent<UserSettings.WaitBenchmarkCreation>();
				}
			}
		}
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x000C8044 File Offset: 0x000C6244
	public static void RevertChanges()
	{
		if (UserSettings.sm_Settings == null)
		{
			return;
		}
		foreach (KeyValuePair<string, UserSettings.SettingData> keyValuePair in UserSettings.sm_Settings)
		{
			keyValuePair.Value.Revert();
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x000C80A4 File Offset: 0x000C62A4
	public static void ApplayChanges()
	{
		if (UserSettings.sm_Settings == null)
		{
			return;
		}
		foreach (KeyValuePair<string, UserSettings.SettingData> keyValuePair in UserSettings.sm_Settings)
		{
			keyValuePair.Value.Apply(false);
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x000C8104 File Offset: 0x000C6304
	public static void OnUIClosed()
	{
		if (UserSettings.ResetTutorial)
		{
			UserSettings.ResetTutorial = false;
			Tutorial.Reset();
			UserSettings.OnChangeAnalytics("reset_tutorials", "unknown", "reset", true);
		}
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x000C8137 File Offset: 0x000C6337
	public static string GetCloudSettingsFilePath()
	{
		return Game.GetSavesRootDir(Game.SavesRoot.Root) + "/CloudUserSettings.def";
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x000C8149 File Offset: 0x000C6349
	private static bool LoadSteamCloudSettings()
	{
		UserSettings.cloud_settings = DT.Parser.LoadFieldFromFile(UserSettings.GetCloudSettingsFilePath(), "Settings");
		return UserSettings.cloud_settings != null;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x000C8167 File Offset: 0x000C6367
	private static void SaveSteamCloudSettings()
	{
		if (UserSettings.cloud_settings != null)
		{
			DT.Field field = new DT.Field(null);
			field.AddChild(UserSettings.cloud_settings);
			global::Defs.SaveFile(field, UserSettings.GetCloudSettingsFilePath());
		}
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x000C818B File Offset: 0x000C638B
	private static DT.Field GetDefFieldFromFile()
	{
		return DT.Parser.LoadFieldFromFile(Game.defs_path + "/Preferences.def", "Settings");
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x000C81A8 File Offset: 0x000C63A8
	public static void PorcesssSettingsDef(bool load_from_file = false)
	{
		DT.Field field;
		if (load_from_file)
		{
			field = UserSettings.GetDefFieldFromFile();
		}
		else
		{
			field = global::Defs.GetDefField("Settings", null);
		}
		UserSettings.sm_Settings.Clear();
		if (field == null)
		{
			return;
		}
		List<DT.Field> list = field.Children();
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i];
			if (!string.IsNullOrEmpty(field2.key))
			{
				UserSettings.SettingData settingData = new UserSettings.SettingData();
				if (settingData.LoadDef(field2) && !UserSettings.sm_Settings.ContainsKey(field2.key))
				{
					UserSettings.sm_Settings.Add(field2.key, settingData);
					settingData.Apply(true);
				}
			}
		}
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x000C8246 File Offset: 0x000C6446
	public static UserSettings.SettingData GetSetting(string key)
	{
		if (!UserSettings.sm_Settings.ContainsKey(key))
		{
			return null;
		}
		return UserSettings.sm_Settings[key];
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x000C8262 File Offset: 0x000C6462
	private static void LoadSettings()
	{
		bool flag = UserSettings.LoadSteamCloudSettings();
		UserSettings.PorcesssSettingsDef(true);
		if (!flag)
		{
			UserSettings.SaveSteamCloudSettings();
		}
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x000C8276 File Offset: 0x000C6476
	private static bool Validate(bool save = true)
	{
		if (UserSettings.OnSettingChange != null)
		{
			UserSettings.OnSettingChange();
		}
		return true;
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x000C828C File Offset: 0x000C648C
	public static void ApplyAction(UserSettings.SettingData setting)
	{
		string id = setting.id;
		if (id == "reset_tutorials")
		{
			UserSettings.ResetTutorial = true;
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x000C82B4 File Offset: 0x000C64B4
	private static void ApplySetting(UserSettings.SettingData setting)
	{
		if (setting.def.type == "keybind")
		{
			UserSettings.SetKeybind(setting, setting.value);
		}
		string id = setting.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1915855278U)
		{
			if (num <= 759496160U)
			{
				if (num <= 309859062U)
				{
					if (num <= 116977766U)
					{
						if (num != 5366943U)
						{
							if (num != 85681568U)
							{
								if (num != 116977766U)
								{
									goto IL_DB0;
								}
								if (!(id == "vsync"))
								{
									goto IL_DB0;
								}
								UserSettings.SetVSync(setting.value);
								goto IL_DB0;
							}
							else
							{
								if (!(id == "volume_sound_effects_enabled"))
								{
									goto IL_DB0;
								}
								UserSettings.SetSoundEffects(setting.value);
								goto IL_DB0;
							}
						}
						else
						{
							if (!(id == "audio_configuration_FMOD_parameter"))
							{
								goto IL_DB0;
							}
							UserSettings.SetAudioConfigurationFMODParam(setting.value);
							goto IL_DB0;
						}
					}
					else if (num <= 166796699U)
					{
						if (num != 158670241U)
						{
							if (num != 166796699U)
							{
								goto IL_DB0;
							}
							if (!(id == "volume_voices"))
							{
								goto IL_DB0;
							}
							UserSettings.SetVoiceVolume(setting.value);
							goto IL_DB0;
						}
						else
						{
							if (!(id == "volume_narrator"))
							{
								goto IL_DB0;
							}
							UserSettings.SetVoiceVolumeNarrator(setting.value);
							goto IL_DB0;
						}
					}
					else if (num != 246290131U)
					{
						if (num != 309859062U)
						{
							goto IL_DB0;
						}
						if (!(id == "volume_master"))
						{
							goto IL_DB0;
						}
						UserSettings.SetMasterVolume(setting.value);
						goto IL_DB0;
					}
					else
					{
						if (!(id == "autosave"))
						{
							goto IL_DB0;
						}
						UserSettings.SetAutoSave(setting.value);
						goto IL_DB0;
					}
				}
				else if (num <= 534143784U)
				{
					if (num != 421864438U)
					{
						if (num != 427541306U)
						{
							if (num != 534143784U)
							{
								goto IL_DB0;
							}
							if (!(id == "auto_detected_cpu"))
							{
								goto IL_DB0;
							}
						}
						else
						{
							if (!(id == "volume_knights"))
							{
								goto IL_DB0;
							}
							UserSettings.SetVoiceVolumeKnights(setting.value);
							goto IL_DB0;
						}
					}
					else
					{
						if (!(id == "volume_ambient_enabled"))
						{
							goto IL_DB0;
						}
						UserSettings.SetAmbience(setting.value);
						goto IL_DB0;
					}
				}
				else if (num <= 600724634U)
				{
					if (num != 534521492U)
					{
						if (num != 600724634U)
						{
							goto IL_DB0;
						}
						if (!(id == "clickable_army_pv_figures"))
						{
							goto IL_DB0;
						}
						UserSettings.SetClickableArmyPVFigures(setting.value);
						goto IL_DB0;
					}
					else
					{
						if (!(id == "clickable_settlement_pv_figures"))
						{
							goto IL_DB0;
						}
						UserSettings.SetClickableSettlementPVFigures(setting.value);
						goto IL_DB0;
					}
				}
				else if (num != 646822624U)
				{
					if (num != 759496160U)
					{
						goto IL_DB0;
					}
					if (!(id == "tooltip_delay"))
					{
						goto IL_DB0;
					}
					goto IL_DB0;
				}
				else
				{
					if (!(id == "edge_scroll_speed"))
					{
						goto IL_DB0;
					}
					UserSettings.SetEdgeScrollSpeed(setting.value);
					goto IL_DB0;
				}
			}
			else if (num <= 1466253711U)
			{
				if (num <= 1059201105U)
				{
					if (num != 899659625U)
					{
						if (num != 985514747U)
						{
							if (num != 1059201105U)
							{
								goto IL_DB0;
							}
							if (!(id == "volume_out_of_focus_enabled"))
							{
								goto IL_DB0;
							}
							UserSettings.SetVolumeOutOfFocusEnabled(setting.value);
							goto IL_DB0;
						}
						else
						{
							if (!(id == "autosave_count"))
							{
								goto IL_DB0;
							}
							goto IL_DB0;
						}
					}
					else
					{
						if (!(id == "depth_of_field"))
						{
							goto IL_DB0;
						}
						UserSettings.SetDepthOfFieldToggle(setting.value);
						UserSettings.UpdatePresetLevel("quality");
						goto IL_DB0;
					}
				}
				else if (num <= 1205386803U)
				{
					if (num != 1174844649U)
					{
						if (num != 1205386803U)
						{
							goto IL_DB0;
						}
						if (!(id == "battleview_ragdolls"))
						{
							goto IL_DB0;
						}
						UserSettings.SetMaxRagdolls(setting.value);
						UserSettings.UpdatePresetLevel("quality");
						goto IL_DB0;
					}
					else
					{
						if (!(id == "volume_units_enabled"))
						{
							goto IL_DB0;
						}
						UserSettings.SetUnitVoice(setting.value);
						goto IL_DB0;
					}
				}
				else if (num != 1457253560U)
				{
					if (num != 1466253711U)
					{
						goto IL_DB0;
					}
					if (!(id == "pause_on_out_of_focus"))
					{
						goto IL_DB0;
					}
					UserSettings.SetPauseOnOutOfFocus(setting.value);
					goto IL_DB0;
				}
				else
				{
					if (!(id == "edge_scroll"))
					{
						goto IL_DB0;
					}
					UserSettings.SetEdgeScroll(setting.value);
					goto IL_DB0;
				}
			}
			else if (num <= 1768246917U)
			{
				if (num <= 1559281484U)
				{
					if (num != 1543218592U)
					{
						if (num != 1559281484U)
						{
							goto IL_DB0;
						}
						if (!(id == "short_snippets"))
						{
							goto IL_DB0;
						}
						UserSettings.SetSnippets(setting.value);
						goto IL_DB0;
					}
					else
					{
						if (!(id == "volume_ambient"))
						{
							goto IL_DB0;
						}
						UserSettings.SetAmbienceVolume(setting.value);
						goto IL_DB0;
					}
				}
				else if (num != 1675789862U)
				{
					if (num != 1768246917U)
					{
						goto IL_DB0;
					}
					if (!(id == "autosave_interval"))
					{
						goto IL_DB0;
					}
					goto IL_DB0;
				}
				else
				{
					if (!(id == "profanity_filter"))
					{
						goto IL_DB0;
					}
					UserSettings.SetProfanityFilter(setting.value);
					goto IL_DB0;
				}
			}
			else if (num <= 1836661716U)
			{
				if (num != 1805204133U)
				{
					if (num != 1836661716U)
					{
						goto IL_DB0;
					}
					if (!(id == "auto_detected_gpu"))
					{
						goto IL_DB0;
					}
				}
				else
				{
					if (!(id == "volume_music_enabled"))
					{
						goto IL_DB0;
					}
					UserSettings.SetMusic(setting.value);
					goto IL_DB0;
				}
			}
			else if (num != 1894656452U)
			{
				if (num != 1915855278U)
				{
					goto IL_DB0;
				}
				if (!(id == "pv_filter"))
				{
					goto IL_DB0;
				}
				UserSettings.SetPVFigureFilter(setting.value);
				goto IL_DB0;
			}
			else
			{
				if (!(id == "fps_limit"))
				{
					goto IL_DB0;
				}
				UserSettings.SetFPSLimit(setting.value);
				goto IL_DB0;
			}
			UserSettings.ApplyAutodetect(setting, setting.value);
		}
		else if (num <= 2788866069U)
		{
			if (num <= 2607843962U)
			{
				if (num <= 2161000147U)
				{
					if (num != 1926020097U)
					{
						if (num != 2073442359U)
						{
							if (num == 2161000147U)
							{
								if (id == "volume_music")
								{
									UserSettings.SetMusicVolume(setting.value);
								}
							}
						}
						else if (id == "volume_units")
						{
							UserSettings.SetVoiceVolumeUnits(setting.value);
						}
					}
					else if (id == "fps_limit_toggle")
					{
						UserSettings.SetFPSLimitToggle(setting.value);
					}
				}
				else if (num <= 2563557940U)
				{
					if (num != 2230736652U)
					{
						if (num == 2563557940U)
						{
							if (id == "pan_speed")
							{
								UserSettings.SetPanSpeed(setting.value);
							}
						}
					}
					else if (id == "volume_out_of_focus_paused_enabled")
					{
						UserSettings.SetVolumeOutOfFocusPausedEnabled(setting.value);
					}
				}
				else if (num != 2601899499U)
				{
					if (num == 2607843962U)
					{
						if (id == "nameplates_pv")
						{
							UserSettings.SetNameplatesEnabledPV(setting.value);
						}
					}
				}
				else if (id == "ssao")
				{
					UserSettings.SetSSAOToggle(setting.value);
					UserSettings.UpdatePresetLevel("quality");
					UserSettings.UpdateCameraDepthMode();
				}
			}
			else if (num <= 2676101913U)
			{
				if (num != 2642532033U)
				{
					if (num != 2671742607U)
					{
						if (num == 2676101913U)
						{
							if (id == "message_que_cap")
							{
								UserSettings.SetMessageQueCap(setting.value);
							}
						}
					}
					else if (id == "volume_out_of_focus")
					{
						UserSettings.SetVolumeOutOfFocus(setting.value);
					}
				}
				else if (id == "nameplates_wv")
				{
					UserSettings.SetNameplatesEnabledWV(setting.value);
				}
			}
			else if (num <= 2717086271U)
			{
				if (num != 2695040084U)
				{
					if (num == 2717086271U)
					{
						if (id == "fog")
						{
							UserSettings.SetFogToggle(setting.value);
							UserSettings.UpdatePresetLevel("quality");
						}
					}
				}
				else if (id == "active_mod")
				{
					UserSettings.SetActiveMod(setting.value);
				}
			}
			else if (num != 2745962869U)
			{
				if (num == 2788866069U)
				{
					if (id == "ssrt")
					{
						UserSettings.SetSSRTQuality(setting.value);
						UserSettings.UpdatePresetLevel("quality");
						UserSettings.UpdateCameraDepthMode();
					}
				}
			}
			else if (id == "reflections")
			{
				UserSettings.SetReflectionsToggle(setting, setting.value);
				UserSettings.UpdatePresetLevel("quality");
			}
		}
		else if (num <= 3217532038U)
		{
			if (num <= 2993663101U)
			{
				if (num != 2856935256U)
				{
					if (num != 2989727388U)
					{
						if (num == 2993663101U)
						{
							if (id == "grass")
							{
								UserSettings.SetGrassToggle(setting.value);
								UserSettings.UpdatePresetLevel("quality");
							}
						}
					}
					else if (id == "tutorial_messages")
					{
						UserSettings.SetTutorialMessages(setting.value);
					}
				}
				else if (id == "tutorial_messages_battles")
				{
					UserSettings.SetTutorialMessagesBattleView(setting.value);
				}
			}
			else if (num <= 3187632402U)
			{
				if (num != 3047748091U)
				{
					if (num == 3187632402U)
					{
						if (id == "volume_out_of_focus_paused")
						{
							UserSettings.SetVolumeOutOfFocusPaused(setting.value);
						}
					}
				}
				else if (id == "clickable_battle_pv_figures")
				{
					UserSettings.SetClickableBattlePVFigures(setting.value);
				}
			}
			else if (num != 3191072300U)
			{
				if (num == 3217532038U)
				{
					if (id == "clickable_nameplates")
					{
						UserSettings.SetClickableNameplates(setting.value);
						return;
					}
				}
			}
			else if (id == "battleview_ragdolls_duration")
			{
				UserSettings.SetMaxRagdollDuration(setting.value);
			}
		}
		else if (num <= 3840843484U)
		{
			if (num <= 3340853222U)
			{
				if (num != 3268160797U)
				{
					if (num == 3340853222U)
					{
						if (id == "volume_sound_effects")
						{
							UserSettings.SetSoundEffectsVolume(setting.value);
						}
					}
				}
				else if (id == "volume_voices_enabled")
				{
					UserSettings.SetVoice(setting.value);
				}
			}
			else if (num != 3410698598U)
			{
				if (num == 3840843484U)
				{
					if (id == "bloom")
					{
						UserSettings.SetBloomToggle(setting.value);
						UserSettings.UpdatePresetLevel("quality");
					}
				}
			}
			else if (id == "data_collection")
			{
				UserSettings.SetDataCollection(setting.value);
			}
		}
		else if (num <= 4059018960U)
		{
			if (num != 3859151095U)
			{
				if (num == 4059018960U)
				{
					if (id == "volume_master_enabled")
					{
						UserSettings.SetMaster(setting.value);
					}
				}
			}
			else if (id == "volume_narrator_enabled")
			{
				UserSettings.SetNarratorVoice(setting.value);
			}
		}
		else if (num != 4165348868U)
		{
			if (num == 4168687445U)
			{
				if (id == "data_collection_asked")
				{
					UserSettings.SetDataCollectionAsked(setting.value);
				}
			}
		}
		else if (id == "volume_knights_enabled")
		{
			UserSettings.SetKnightVoice(setting.value);
		}
		IL_DB0:
		if (UserSettings.OnSettingChange != null)
		{
			UserSettings.OnSettingChange();
		}
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x000C9084 File Offset: 0x000C7284
	private static void ApplyOption(UserSettings.SettingData setting)
	{
		List<Value> options = setting.options;
		if (options != null)
		{
			bool flag = false;
			int num = setting.CalcOptionIndex(setting.value, out flag);
			if (flag && num >= 0 && num < options.Count)
			{
				setting.ApplyValue(options[num]);
				return;
			}
			UserSettings.ApplyOption(setting, num);
		}
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x000C90D4 File Offset: 0x000C72D4
	private static void ApplyOption(UserSettings.SettingData setting, int idx)
	{
		if (idx < 0)
		{
			return;
		}
		string id = setting.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 2597670950U)
		{
			if (num <= 916110480U)
			{
				if (num != 164617456U)
				{
					if (num != 557447092U)
					{
						if (num != 916110480U)
						{
							return;
						}
						if (!(id == "audio_configuration"))
						{
							return;
						}
						UserSettings.SetAudioConfiguration(setting, setting.options[idx]);
						return;
					}
					else
					{
						if (!(id == "reminders"))
						{
							return;
						}
						UserSettings.SetReminders(setting, setting.options[idx]);
						return;
					}
				}
				else
				{
					if (!(id == "difficulty"))
					{
						return;
					}
					UserSettings.SetDifficulty(setting.options[idx]);
					return;
				}
			}
			else if (num != 1149088756U)
			{
				if (num != 2086888361U)
				{
					if (num != 2597670950U)
					{
						return;
					}
					if (!(id == "quality"))
					{
						return;
					}
					UserSettings.SetQuality(setting, setting.options[idx]);
					return;
				}
				else
				{
					if (!(id == "antialiasing_mode"))
					{
						return;
					}
					UserSettings.SetAntialiasing(setting.options[idx]);
					UserSettings.UpdatePresetLevel("quality");
					UserSettings.UpdateCameraDepthMode();
					return;
				}
			}
			else
			{
				if (!(id == "resoulution"))
				{
					return;
				}
				Resolution[] resolutions = Screen.resolutions;
				idx = resolutions.Length - 1 - idx;
				Resolution resolution = resolutions[idx];
				UserSettings.SetResolution(resolution.width, resolution.height, resolution.refreshRate);
				return;
			}
		}
		else if (num <= 3119462523U)
		{
			if (num != 2825881069U)
			{
				if (num != 3080080344U)
				{
					if (num != 3119462523U)
					{
						return;
					}
					if (!(id == "language"))
					{
						return;
					}
					UserSettings.SetLanguage(setting, setting.options[idx]);
					return;
				}
				else
				{
					if (!(id == "terrain_quality"))
					{
						return;
					}
					UserSettings.SetTerrainQuality(setting.options[idx]);
					UserSettings.UpdatePresetLevel("quality");
					return;
				}
			}
			else
			{
				if (!(id == "window_mode"))
				{
					return;
				}
				UserSettings.SetFullscreen(setting.options[idx]);
				return;
			}
		}
		else if (num != 3306508096U)
		{
			if (num != 3698251680U)
			{
				if (num != 3981505984U)
				{
					return;
				}
				if (!(id == "texture_size"))
				{
					return;
				}
				UserSettings.SetTextureSize(setting.options[idx]);
				UserSettings.UpdatePresetLevel("quality");
				return;
			}
			else
			{
				if (!(id == "audio_language"))
				{
					return;
				}
				UserSettings.SetAudioLanguage(setting, setting.options[idx]);
				return;
			}
		}
		else
		{
			if (!(id == "shadows_quality"))
			{
				return;
			}
			UserSettings.SetShadowsQuality(setting.options[idx]);
			UserSettings.UpdatePresetLevel("quality");
			return;
		}
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x000C93B8 File Offset: 0x000C75B8
	private static int GetCurrentOptionIdx(UserSettings.SettingData setting)
	{
		string id = setting.id;
		return setting.options.FindIndex((Value v) => v.Equals(setting.value));
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x000C93FA File Offset: 0x000C75FA
	private static string GetResolutionStr(int width, int height, int refreshRate)
	{
		return string.Format("{0} x {1}, {2}Hz", width, height, refreshRate);
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x000C9418 File Offset: 0x000C7618
	private static List<Value> GetOptions(UserSettings.SettingData setting)
	{
		string id = setting.id;
		if (id == "resoulution")
		{
			List<Value> list = new List<Value>();
			Resolution[] resolutions = Screen.resolutions;
			int num = resolutions.Length;
			for (int i = resolutions.Length - 1; i >= 0; i--)
			{
				Resolution resolution = resolutions[i];
				if (UserSettings.ScreenWidth == resolutions[i].width && UserSettings.ScreenHeight == resolutions[i].height)
				{
					int screenRefreshRate = UserSettings.ScreenRefreshRate;
					int refreshRate = resolutions[i].refreshRate;
				}
				list.Add(new Value(UserSettings.GetResolutionStr(resolution.width, resolution.height, resolution.refreshRate)));
			}
			return list;
		}
		if (!(id == "audio_language"))
		{
			List<Value> list;
			if (!(id == "language"))
			{
				list = new List<Value>();
				DT.Field field = setting.def.FindChild("options", null, true, true, true, '.');
				if (((field != null) ? field.children : null) != null)
				{
					for (int j = 0; j < field.children.Count; j++)
					{
						DT.Field field2 = field.children[j];
						if (!(field2.type != "option"))
						{
							list.Add(field2.key);
						}
					}
				}
				return list;
			}
			global::Defs defs = global::Defs.Get(false);
			if (defs != null)
			{
				defs.LoadLanguagesDef(false);
			}
			List<DT.Field> list2 = (defs != null) ? defs.languages : null;
			if (list2 == null)
			{
				return null;
			}
			list = new List<Value>(list2.Count);
			for (int k = 0; k < list2.Count; k++)
			{
				if (!list2[k].GetBool("hidden", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
				{
					list.Add(list2[k].key);
				}
			}
			return list;
		}
		else
		{
			global::Defs defs2 = global::Defs.Get(false);
			if (defs2 != null)
			{
				defs2.LoadLanguagesDef(false);
			}
			List<DT.Field> list3 = (defs2 != null) ? defs2.languages : null;
			if (list3 == null)
			{
				return null;
			}
			List<Value> list = new List<Value>(list3.Count);
			for (int l = 0; l < list3.Count; l++)
			{
				if (list3[l].GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.'))
				{
					list.Add(list3[l].key);
				}
			}
			return list;
		}
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x000C9675 File Offset: 0x000C7875
	public static bool SetResolution(int width, int height, int refreshRate = 60)
	{
		UserSettings.ScreenWidth = width;
		UserSettings.ScreenHeight = height;
		UserSettings.ScreenRefreshRate = refreshRate;
		ScreenResolution.SetResolution(width, height, Screen.fullScreenMode, refreshRate);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x000C969C File Offset: 0x000C789C
	public static bool SetFullscreen(string mode)
	{
		UserSettings.FullScreenMode = (FullScreenMode)Enum.Parse(typeof(FullScreenMode), mode);
		ScreenResolution.SetFullScrenMode(UserSettings.FullScreenMode);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x000C96C8 File Offset: 0x000C78C8
	public static PostProcessLayer.Antialiasing GetPostProcessAntialiasinMode(string mode)
	{
		if (mode == "None")
		{
			return PostProcessLayer.Antialiasing.None;
		}
		if (mode == "FXAA")
		{
			return PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
		}
		if (mode == "SMAA_low" || mode == "SMAA_high")
		{
			return PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
		}
		if (!(mode == "TAA"))
		{
			return PostProcessLayer.Antialiasing.None;
		}
		return PostProcessLayer.Antialiasing.TemporalAntialiasing;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x000C9721 File Offset: 0x000C7921
	public static SubpixelMorphologicalAntialiasing.Quality GetSMAAQuality(string mode)
	{
		if (mode == "SMAA_high")
		{
			return SubpixelMorphologicalAntialiasing.Quality.High;
		}
		return SubpixelMorphologicalAntialiasing.Quality.Low;
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x000C9733 File Offset: 0x000C7933
	public static bool SetReminders(UserSettings.SettingData setting, string option)
	{
		UserSettings.SetPreset(setting, option);
		UserSettings.RemindersLevelStr = option;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x000C9748 File Offset: 0x000C7948
	public static bool SetVSync(bool vsync)
	{
		UserSettings.VSync = vsync;
		QualitySettings.vSyncCount = (vsync ? 1 : 0);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x000C9762 File Offset: 0x000C7962
	public static bool SetFPSLimitToggle(bool toggled)
	{
		UserSettings.HasFpsLimit = toggled;
		if (toggled && UserSettings.FpsLimit != null && UserSettings.FpsLimit != "Unlimited")
		{
			Application.targetFrameRate = int.Parse(UserSettings.FpsLimit);
		}
		else
		{
			Application.targetFrameRate = -1;
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x000C97A2 File Offset: 0x000C79A2
	public static bool SetFPSLimit(string newLimit)
	{
		UserSettings.FpsLimit = newLimit;
		if (UserSettings.FpsLimit == "Unlimited")
		{
			Application.targetFrameRate = -1;
		}
		else
		{
			Application.targetFrameRate = int.Parse(UserSettings.FpsLimit);
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x000C97D8 File Offset: 0x000C79D8
	public static bool SetQuality(UserSettings.SettingData setting, string option)
	{
		UserSettings.SetPreset(setting, option);
		UserSettings.QualityLevelStr = option;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x000C97F0 File Offset: 0x000C79F0
	public static void SetPreset(UserSettings.SettingData setting, string option)
	{
		UserSettings.PresetProcessing = true;
		DT.Field field;
		if (setting == null)
		{
			field = null;
		}
		else
		{
			DT.Field field2 = setting.def.FindChild("options", null, true, true, true, '.');
			if (field2 == null)
			{
				field = null;
			}
			else
			{
				DT.Field field3 = field2.FindChild(option, null, true, true, true, '.');
				field = ((field3 != null) ? field3.FindChild("preset_settings", null, true, true, true, '.') : null);
			}
		}
		DT.Field field4 = field;
		if (((field4 != null) ? field4.children : null) != null)
		{
			for (int i = 0; i < field4.children.Count; i++)
			{
				DT.Field field5 = field4.children[i];
				UserSettings.SettingData settingData;
				if (!string.IsNullOrEmpty(field5.key) && !(field5.type != "setting") && UserSettings.sm_Settings.TryGetValue(field5.key, out settingData))
				{
					settingData.ApplyValue(field5.Value(null, true, true));
				}
			}
		}
		UserSettings.PresetProcessing = false;
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x000C98C4 File Offset: 0x000C7AC4
	public static bool DoSettingsMatchPresetLevel(UserSettings.SettingData preset, string option)
	{
		DT.Field field = preset.def.FindChild("options", null, true, true, true, '.');
		DT.Field field2;
		if (field == null)
		{
			field2 = null;
		}
		else
		{
			DT.Field field3 = field.FindChild(option, null, true, true, true, '.');
			field2 = ((field3 != null) ? field3.FindChild("preset_settings", null, true, true, true, '.') : null);
		}
		DT.Field field4 = field2;
		if (((field4 != null) ? field4.children : null) != null)
		{
			for (int i = 0; i < field4.children.Count; i++)
			{
				DT.Field field5 = field4.children[i];
				UserSettings.SettingData settingData;
				if (!string.IsNullOrEmpty(field5.key) && !(field5.type != "setting") && UserSettings.sm_Settings.TryGetValue(field5.key, out settingData) && !settingData.value.Equals(field5.Value(null, true, true)))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x000C9990 File Offset: 0x000C7B90
	public static void UpdatePresetLevel(string preset_name)
	{
		if (UserSettings.PresetProcessing)
		{
			return;
		}
		UserSettings.SettingData settingData;
		if (!UserSettings.sm_Settings.TryGetValue(preset_name, out settingData))
		{
			return;
		}
		if (settingData.value.String(null).Contains("Custom"))
		{
			List<Value> options = settingData.GetOptions();
			for (int i = 0; i < options.Count; i++)
			{
				if (UserSettings.DoSettingsMatchPresetLevel(settingData, options[i].String(null)))
				{
					settingData.ApplyValue(options[i].String(null));
					return;
				}
			}
			return;
		}
		if (!UserSettings.DoSettingsMatchPresetLevel(settingData, settingData.value.String(null)))
		{
			settingData.ApplyValue("Custom");
		}
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x000C9A40 File Offset: 0x000C7C40
	public static void UpdateCameraDepthMode()
	{
		CameraController cameraController = CameraController.Get();
		if (cameraController == null)
		{
			return;
		}
		if (!UserSettings.SSAO)
		{
			cameraController.DisableCameraDepthMode(DepthTextureMode.DepthNormals);
		}
		else
		{
			cameraController.EnableCameraDepthMode(DepthTextureMode.DepthNormals);
		}
		if (!UserSettings.AntialiasingModeStr.ToLowerInvariant().Contains("taa") && UserSettings.SSRTStr.Equals("None"))
		{
			cameraController.DisableCameraDepthMode(DepthTextureMode.MotionVectors);
			return;
		}
		cameraController.EnableCameraDepthMode(DepthTextureMode.MotionVectors);
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x000C9AAC File Offset: 0x000C7CAC
	public static bool SetTextureSize(string mode)
	{
		if (!(mode == "Eight"))
		{
			if (!(mode == "Quarter"))
			{
				if (!(mode == "Half"))
				{
					QualitySettings.masterTextureLimit = 0;
				}
				else
				{
					QualitySettings.masterTextureLimit = 1;
				}
			}
			else
			{
				QualitySettings.masterTextureLimit = 2;
			}
		}
		else
		{
			QualitySettings.masterTextureLimit = 3;
		}
		UserSettings.TextureSizeStr = mode;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x000C9B0C File Offset: 0x000C7D0C
	public static bool SetTerrainQuality(string mode)
	{
		if (!(mode == "Low"))
		{
			if (!(mode == "Medium"))
			{
				if (mode == "High")
				{
					TerrainQualityManager.Quality = TerrainQualityManager.QualityOption.High;
				}
			}
			else
			{
				TerrainQualityManager.Quality = TerrainQualityManager.QualityOption.Medium;
			}
		}
		else
		{
			TerrainQualityManager.Quality = TerrainQualityManager.QualityOption.Low;
		}
		UserSettings.TextureQualityStr = mode;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x000C9B64 File Offset: 0x000C7D64
	private static bool SetShadowsQuality(string mode)
	{
		if (!(mode == "Low"))
		{
			if (!(mode == "Medium"))
			{
				if (mode == "High")
				{
					QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
					QualitySettings.shadowDistance = 300f;
				}
			}
			else
			{
				QualitySettings.shadowResolution = ShadowResolution.High;
				QualitySettings.shadowDistance = 275f;
			}
		}
		else
		{
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			QualitySettings.shadowDistance = 250f;
		}
		UserSettings.ShadowsQualityStr = mode;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x000C9BDC File Offset: 0x000C7DDC
	public static bool SetAntialiasing(string mode)
	{
		List<PostProcessLayer> list = new List<PostProcessLayer>();
		CameraController cameraController = CameraController.Get();
		GameObject gameObject = (cameraController != null) ? cameraController.gameObject : null;
		if (gameObject == null)
		{
			Camera main = Camera.main;
			GameObject gameObject2;
			if (main == null)
			{
				gameObject2 = null;
			}
			else
			{
				Transform parent = main.transform.parent;
				gameObject2 = ((parent != null) ? parent.gameObject : null);
			}
			gameObject = gameObject2;
		}
		global::Common.FindChildrenWithComponent<PostProcessLayer>(gameObject, list);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject3 = list[i].gameObject;
			CTAA_PC ctaa_PC = (gameObject3 != null) ? gameObject3.GetComponent<CTAA_PC>() : null;
			string mode2 = mode;
			if (ctaa_PC != null)
			{
				if (mode == "TAA")
				{
					ctaa_PC.enabled = true;
					mode2 = "None";
				}
				else
				{
					ctaa_PC.enabled = false;
				}
			}
			list[i].antialiasingMode = UserSettings.GetPostProcessAntialiasinMode(mode2);
			list[i].subpixelMorphologicalAntialiasing.quality = UserSettings.GetSMAAQuality(mode2);
		}
		UserSettings.AntialiasingModeStr = mode;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x000C9CD0 File Offset: 0x000C7ED0
	public static bool SetReflectionsToggle(UserSettings.SettingData setting, bool toggled)
	{
		if (UserSettings.goOcean == null)
		{
			return false;
		}
		MirrorReflection component = UserSettings.goOcean.GetComponent<MirrorReflection>();
		if (component == null)
		{
			return false;
		}
		if (toggled)
		{
			DT.Field field = setting.def.FindChild("mask", null, true, true, true, '.');
			int num = field.NumValues();
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = field.Value(i, null, true, true);
			}
			component.reflectionMask = LayerMask.GetMask(array);
		}
		else
		{
			component.reflectionMask = LayerMask.GetMask(Array.Empty<string>());
		}
		UserSettings.Reflecitons = toggled;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x000C9D82 File Offset: 0x000C7F82
	public static bool SetGrassToggle(bool toggled)
	{
		if (toggled)
		{
			EmissionRenderer.enable_rendering = true;
			BSGGrass.EnableGrass();
		}
		else
		{
			EmissionRenderer.enable_rendering = false;
			BSGGrass.DisableGrass();
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x000C9DA8 File Offset: 0x000C7FA8
	public static bool SetSSAOToggle(bool toggled)
	{
		List<PostProcessVolume> list = new List<PostProcessVolume>();
		global::Common.FindChildrenWithComponent<PostProcessVolume>(UserSettings.goPPVs, list);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			AmbientOcclusion ambientOcclusion;
			if (list[i].profile.TryGetSettings<AmbientOcclusion>(out ambientOcclusion))
			{
				ambientOcclusion.enabled.overrideState = true;
				ambientOcclusion.enabled.value = toggled;
			}
		}
		UserSettings.SSAO = toggled;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x000C9E18 File Offset: 0x000C8018
	public static bool SetFogToggle(bool toggled)
	{
		List<PostProcessVolume> list = new List<PostProcessVolume>();
		global::Common.FindChildrenWithComponent<PostProcessVolume>(UserSettings.goPPVs, list);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			SCPE.Fog fog;
			if (list[i].profile.TryGetSettings<SCPE.Fog>(out fog))
			{
				fog.enabled.overrideState = true;
				fog.enabled.value = toggled;
			}
		}
		UserSettings.Fog = toggled;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x000C9E88 File Offset: 0x000C8088
	public static bool SetSSRTQuality(string mode)
	{
		if (!(mode == "None"))
		{
			if (!(mode == "Low"))
			{
				if (!(mode == "Medium"))
				{
					if (mode == "High")
					{
						SSRTQualityManager.Quality = SSRTQualityManager.QualityOption.High;
					}
				}
				else
				{
					SSRTQualityManager.Quality = SSRTQualityManager.QualityOption.Medium;
				}
			}
			else
			{
				SSRTQualityManager.Quality = SSRTQualityManager.QualityOption.Low;
			}
		}
		else
		{
			SSRTQualityManager.Quality = SSRTQualityManager.QualityOption.None;
		}
		UserSettings.SSRTStr = mode;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x000C9EF8 File Offset: 0x000C80F8
	public static bool SetDepthOfFieldToggle(bool toggled)
	{
		List<PostProcessVolume> list = new List<PostProcessVolume>();
		global::Common.FindChildrenWithComponent<PostProcessVolume>(UserSettings.goPPVs, list);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DepthOfField depthOfField;
			if (list[i].profile.TryGetSettings<DepthOfField>(out depthOfField))
			{
				depthOfField.enabled.overrideState = true;
				depthOfField.enabled.value = toggled;
			}
		}
		UserSettings.DepthOfField = toggled;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x000C9F68 File Offset: 0x000C8168
	public static bool SetBloomToggle(bool toggled)
	{
		List<PostProcessVolume> list = new List<PostProcessVolume>();
		global::Common.FindChildrenWithComponent<PostProcessVolume>(UserSettings.goPPVs, list);
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Bloom bloom;
			if (list[i].profile.TryGetSettings<Bloom>(out bloom))
			{
				bloom.enabled.overrideState = true;
				bloom.enabled.value = toggled;
			}
		}
		UserSettings.Bloom = toggled;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x000C9FD8 File Offset: 0x000C81D8
	public static bool SetDifficulty(string difficulty)
	{
		Game game = GameLogic.Get(false);
		bool flag;
		if (game == null)
		{
			flag = (null != null);
		}
		else
		{
			Campaign campaign = game.campaign;
			flag = (((campaign != null) ? campaign.campaignData : null) != null);
		}
		if (!flag)
		{
			return false;
		}
		game.campaign.campaignData.Set("ai_difficulty", difficulty, true);
		game.rules.Reload();
		UserSettings.Difficulty = difficulty;
		Logic.Battle battle = BattleMap.battle;
		if (((battle != null) ? battle.ai : null) != null)
		{
			for (int i = 0; i < BattleMap.battle.ai.Length; i++)
			{
				BattleAI.Def def = BattleMap.battle.ai[i].def;
				if (def != null)
				{
					def.Load(game);
				}
			}
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x000CA084 File Offset: 0x000C8284
	public static Value GetClosestAutodetectOption(UserSettings.SettingData s, float rating)
	{
		if (s == null || s.options == null || s.options.Count == 0)
		{
			return 0;
		}
		float num = float.MaxValue;
		int index = 0;
		for (int i = 0; i < s.options.Count; i++)
		{
			float num2;
			if (float.TryParse(s.options[i].String(null), out num2))
			{
				float num3 = Math.Abs(num2 - rating);
				if (num > num3)
				{
					num = num3;
					index = i;
				}
			}
		}
		return s.options[index];
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x000CA110 File Offset: 0x000C8310
	public static void SetKeybind(UserSettings.SettingData s, string bindingsParseStr)
	{
		string actionName = (s != null) ? s.id : null;
		string text = string.Join("", bindingsParseStr.Split(null, StringSplitOptions.RemoveEmptyEntries));
		if (bindingsParseStr.Length != text.Length)
		{
			s.value = (s.new_value = text);
			s.SaveSetting();
		}
		KeyBindings.ChangeKeybinds(actionName, text);
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			worldUI.OnKeyBindsChange();
		}
		Action onSettingChange = UserSettings.OnSettingChange;
		if (onSettingChange == null)
		{
			return;
		}
		onSettingChange();
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x000CA18C File Offset: 0x000C838C
	public static void ApplyAutodetect(UserSettings.SettingData s, float rating)
	{
		if (s == null)
		{
			return;
		}
		Value closestAutodetectOption = UserSettings.GetClosestAutodetectOption(s, rating);
		UserSettings.SetPreset(s, closestAutodetectOption);
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x000CA1B1 File Offset: 0x000C83B1
	public static bool SetAudioConfiguration(UserSettings.SettingData setting, string option)
	{
		UserSettings.AudioConfigurationStr = option;
		UserSettings.SetPreset(setting, option);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000CA1C8 File Offset: 0x000C83C8
	public static bool SetAudioConfigurationFMODParam(int newValue)
	{
		UserSettings.AudioConfigurationFMODParam = newValue;
		RuntimeManager.StudioSystem.setParameterByName("AudioConfiguration", (float)newValue, false);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x000CA1F7 File Offset: 0x000C83F7
	public static bool SetClickableArmyPVFigures(bool newValue)
	{
		UserSettings.ClickableArmyPVFigures = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x000CA205 File Offset: 0x000C8405
	public static bool SetClickableBattlePVFigures(bool newValue)
	{
		UserSettings.ClickableBattlePVFigures = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x000CA213 File Offset: 0x000C8413
	public static bool SetClickableSettlementPVFigures(bool newValue)
	{
		UserSettings.ClickableSettlementPVFigures = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x000CA221 File Offset: 0x000C8421
	public static bool SetClickableNameplates(bool newValue)
	{
		UserSettings.ClickableNameplates = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x000CA22F File Offset: 0x000C842F
	public static bool SetNameplatesEnabledPV(bool newValue)
	{
		UserSettings.NameplatesEnabledPV = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x000CA23D File Offset: 0x000C843D
	public static bool SetNameplatesEnabledWV(bool newValue)
	{
		UserSettings.NameplatesEnabledWV = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x000CA24B File Offset: 0x000C844B
	public static bool SetPVFigureFilter(int filter)
	{
		UserSettings.PVFigureFilter = filter;
		ViewMode.ApplyFilter((ViewMode.AllowedFigures)UserSettings.PVFigureFilter);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x000CA263 File Offset: 0x000C8463
	public static bool SetMasterVolume(float newValue)
	{
		UserSettings.MasterVolume = newValue;
		UserSettings.ForceTemporaryMasterVolume(UserSettings.MasterVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x000CA27B File Offset: 0x000C847B
	public static bool SetVolumeOutOfFocus(float newValue)
	{
		UserSettings.VolumeOutOfFocus = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x000CA289 File Offset: 0x000C8489
	public static bool SetVolumeOutOfFocusPaused(float newValue)
	{
		UserSettings.VolumeOutOfFocusPaused = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x000CA298 File Offset: 0x000C8498
	public static void ForceTemporaryMasterVolume(float value)
	{
		value = Mathf.Clamp01(value);
		RuntimeManager.GetVCA("vca:/Master").setVolume(UserSettings.MasterOn ? value : 0f);
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x000CA2D0 File Offset: 0x000C84D0
	public static bool SetMusicVolume(float newValue)
	{
		UserSettings.MusicVolume = newValue;
		RuntimeManager.GetVCA("vca:/Music").setVolume(UserSettings.MusicOn ? UserSettings.MusicVolume : 0f);
		BackgroundMusic.OnSettingsChanged();
		return UserSettings.Validate(true);
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x000CA314 File Offset: 0x000C8514
	public static bool SetVoiceVolume(float newValue)
	{
		UserSettings.VoiceVolume = newValue;
		RuntimeManager.GetVCA("vca:/Voice").setVolume(UserSettings.VoiceOn ? UserSettings.VoiceVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x000CA354 File Offset: 0x000C8554
	public static bool SetVoiceVolumeNarrator(float newValue)
	{
		UserSettings.NarratorVoiceVolume = newValue;
		RuntimeManager.GetVCA("vca:/Narrator").setVolume(UserSettings.NarratorVoiceOn ? UserSettings.NarratorVoiceVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x000CA394 File Offset: 0x000C8594
	public static bool SetVoiceVolumeKnights(float newValue)
	{
		UserSettings.KnightVoiceVolume = newValue;
		RuntimeManager.GetVCA("vca:/Character").setVolume(UserSettings.KnightVoiceOn ? UserSettings.KnightVoiceVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x000CA3D4 File Offset: 0x000C85D4
	public static bool SetVoiceVolumeUnits(float newValue)
	{
		UserSettings.UnitVoiceVolume = newValue;
		RuntimeManager.GetVCA("vca:/Unit").setVolume(UserSettings.UnitVoiceOn ? UserSettings.UnitVoiceVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x000CA414 File Offset: 0x000C8614
	public static bool SetSoundEffectsVolume(float newValue)
	{
		UserSettings.SoundEffectsVolume = newValue;
		RuntimeManager.GetVCA("vca:/Sound Effects").setVolume(UserSettings.SoundEffectsOn ? UserSettings.SoundEffectsVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x000CA454 File Offset: 0x000C8654
	public static bool SetAmbienceVolume(float newValue)
	{
		UserSettings.AmbienceVolume = newValue;
		RuntimeManager.GetVCA("vca:/Ambience").setVolume(UserSettings.AmbienceOn ? UserSettings.AmbienceVolume : 0f);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x000CA493 File Offset: 0x000C8693
	public static bool SetAudioChanelEnabdled(string key, bool newValue)
	{
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x000CA49B File Offset: 0x000C869B
	public static bool SetMaster(bool newValue)
	{
		UserSettings.MasterOn = newValue;
		UserSettings.SetMasterVolume(UserSettings.MasterVolume);
		BackgroundMusic.OnSettingsChanged();
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x000CA4B9 File Offset: 0x000C86B9
	public static bool SetVolumeOutOfFocusEnabled(bool newValue)
	{
		UserSettings.VolumeOutOfFocusOn = newValue;
		UserSettings.SetVolumeOutOfFocus(UserSettings.VolumeOutOfFocus);
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x000CA4D2 File Offset: 0x000C86D2
	public static bool SetVolumeOutOfFocusPausedEnabled(bool newValue)
	{
		UserSettings.VolumeOutOfFocusPausedOn = newValue;
		UserSettings.SetVolumeOutOfFocusPaused(UserSettings.VolumeOutOfFocusPaused);
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x000CA4EB File Offset: 0x000C86EB
	public static bool SetMusic(bool newValue)
	{
		UserSettings.MusicOn = newValue;
		UserSettings.SetMusicVolume(UserSettings.MusicVolume);
		BackgroundMusic.OnSettingsChanged();
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x000CA509 File Offset: 0x000C8709
	public static bool SetVoice(bool newValue)
	{
		UserSettings.VoiceOn = newValue;
		UserSettings.SetVoiceVolume(UserSettings.VoiceVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x000CA522 File Offset: 0x000C8722
	public static bool SetNarratorVoice(bool newValue)
	{
		UserSettings.NarratorVoiceOn = newValue;
		UserSettings.SetVoiceVolumeNarrator(UserSettings.NarratorVoiceVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x000CA53B File Offset: 0x000C873B
	public static bool SetKnightVoice(bool newValue)
	{
		UserSettings.KnightVoiceOn = newValue;
		UserSettings.SetVoiceVolumeKnights(UserSettings.KnightVoiceVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x000CA554 File Offset: 0x000C8754
	public static bool SetUnitVoice(bool newValue)
	{
		UserSettings.UnitVoiceOn = newValue;
		UserSettings.SetVoiceVolumeUnits(UserSettings.UnitVoiceVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x000CA56D File Offset: 0x000C876D
	public static bool SetAmbience(bool newValue)
	{
		UserSettings.AmbienceOn = newValue;
		UserSettings.SetAmbienceVolume(UserSettings.VoiceVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x000CA586 File Offset: 0x000C8786
	public static bool SetSoundEffects(bool newValue)
	{
		UserSettings.SoundEffectsOn = newValue;
		UserSettings.SetSoundEffectsVolume(UserSettings.SoundEffectsVolume);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x000CA59F File Offset: 0x000C879F
	public static bool SetInterrupt(bool newValue)
	{
		UserSettings.InterruptMessages = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x000CA5AD File Offset: 0x000C87AD
	public static bool SetSnippets(bool newValue)
	{
		UserSettings.ShortSnippets = newValue;
		BackgroundMusic.OnSettingsChanged();
		return UserSettings.Validate(true);
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x000CA5C0 File Offset: 0x000C87C0
	public static bool SetMessageQueCap(int newValue)
	{
		UserSettings.MessageQueCap = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x000CA5D0 File Offset: 0x000C87D0
	public static bool SetMaxRagdolls(string newValue)
	{
		int maxRagdollCount;
		if (int.TryParse(newValue, out maxRagdollCount))
		{
			UserSettings.MaxRagdollCount = maxRagdollCount;
		}
		else
		{
			UserSettings.MaxRagdollCount = 5;
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x000CA5FB File Offset: 0x000C87FB
	public static bool SetMaxRagdollDuration(float newValue)
	{
		UserSettings.MaxRagdollDuration = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x000CA609 File Offset: 0x000C8809
	public static bool SetTutorialMessages(bool newValue)
	{
		UserSettings.TutorialMessages = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x000CA617 File Offset: 0x000C8817
	public static bool SetTutorialMessagesBattleView(bool newValue)
	{
		UserSettings.TutorialMessagesBattleView = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x000CA625 File Offset: 0x000C8825
	public static bool SetEdgeScroll(bool newValue)
	{
		UserSettings.EdgeScroll = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x000CA633 File Offset: 0x000C8833
	public static bool SetEdgeScrollSpeed(float newValue)
	{
		UserSettings.EdgeScrollSpeed = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x000CA641 File Offset: 0x000C8841
	public static bool SetPanSpeed(float newValue)
	{
		UserSettings.PanSpeed = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x000CA64F File Offset: 0x000C884F
	public static bool SetAutoSave(bool newValue)
	{
		UserSettings.AutoSave = newValue;
		AutoSaveManager.EnablePeriodicAutosave(UserSettings.AutoSave, false);
		return UserSettings.Validate(true);
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x000CA668 File Offset: 0x000C8868
	public static bool SetProfanityFilter(bool newValue)
	{
		UserSettings.ProfanityFilter = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x000CA676 File Offset: 0x000C8876
	public static bool SetDataCollection(bool newValue)
	{
		UserSettings.DataCollection = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x000CA684 File Offset: 0x000C8884
	public static bool SetDataCollectionAsked(bool newValue)
	{
		UserSettings.DataCollectionAsked = newValue;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x000CA694 File Offset: 0x000C8894
	public static bool SetActiveMod(string newValue)
	{
		UserSettings.ActiveMod = newValue;
		ModManager modManager = ModManager.Get(false);
		if (modManager != null)
		{
			bool flag = false;
			if (string.IsNullOrEmpty(newValue))
			{
				if (modManager.GetActiveMod() != null)
				{
					modManager.SetActiveMod(null);
					flag = true;
				}
			}
			else
			{
				Mod mod = modManager.GetMod(newValue);
				if (mod != null && modManager.GetActiveMod() != mod)
				{
					modManager.SetActiveMod(mod);
					flag = true;
				}
			}
			if (flag)
			{
				global::Defs defs = global::Defs.Get(false);
				if (defs != null)
				{
					defs.Reload();
				}
			}
		}
		return UserSettings.Validate(true);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x000CA70C File Offset: 0x000C890C
	public static bool SetLanguage(UserSettings.SettingData setting, string newKey)
	{
		if (!UserSettings.ValidateLanguageKey(newKey))
		{
			string fallbackLanguageKey = UserSettings.GetFallbackLanguageKey();
			if (fallbackLanguageKey != setting.value)
			{
				setting.ApplyValue(fallbackLanguageKey);
				return true;
			}
			global::Defs.Language = fallbackLanguageKey;
		}
		else
		{
			global::Defs.Language = newKey;
		}
		global::Defs.Get(false).LoadTexts();
		TitleUI titleUI = BaseUI.Get<TitleUI>();
		if (titleUI != null)
		{
			titleUI.OnLanguageChange();
		}
		UserSettings.GetSetting("audio_language").ApplyValue(global::Defs.Language);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x000CA790 File Offset: 0x000C8990
	public static bool SetAudioLanguage(UserSettings.SettingData setting, string newKey)
	{
		if (!UserSettings.ValidateAudioLanguageKey(newKey))
		{
			string fallbackAudioLanguageKey = UserSettings.GetFallbackAudioLanguageKey();
			if (fallbackAudioLanguageKey != setting.value)
			{
				setting.ApplyValue(fallbackAudioLanguageKey);
				return true;
			}
			newKey = fallbackAudioLanguageKey;
		}
		FMODVoiceProvider.Unregister();
		Voices.Language = newKey;
		FMODVoiceProvider.Register();
		Voices.LoadDefs();
		BaseUI.PlayVoiceEvent("narrator_voice:greetings", null);
		return UserSettings.Validate(true);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x000CA7F5 File Offset: 0x000C89F5
	public static bool SetPauseOnOutOfFocus(bool allowPause)
	{
		UserSettings.PauseOnOutOfFocus = allowPause;
		return UserSettings.Validate(true);
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x000CA804 File Offset: 0x000C8A04
	private static string GetFallbackLanguageKey()
	{
		string nativeLanguage = UserSettings.GetNativeLanguage();
		if (UserSettings.ValidateLanguageKey(nativeLanguage))
		{
			return nativeLanguage;
		}
		return "en";
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x000CA828 File Offset: 0x000C8A28
	private static string GetFallbackAudioLanguageKey()
	{
		string nativeLanguage = UserSettings.GetNativeLanguage();
		if (UserSettings.ValidateAudioLanguageKey(nativeLanguage))
		{
			return nativeLanguage;
		}
		return "en";
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x000CA84C File Offset: 0x000C8A4C
	public static bool ValidateLanguageKey(string langKey)
	{
		if (string.IsNullOrEmpty(langKey))
		{
			return false;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			Debug.LogError("Attempting to validate language key '" + langKey + "' before defs are created!");
			return true;
		}
		defs.LoadLanguagesDef(false);
		DT.Field field = global::Defs.FindLanguage(langKey);
		return field != null && field.GetBool("enabled", global::Defs.GetLanguageVars(true), false, true, true, true, '.');
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x000CA8BC File Offset: 0x000C8ABC
	public static bool ValidateAudioLanguageKey(string langKey)
	{
		if (string.IsNullOrEmpty(langKey))
		{
			return false;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			Debug.LogError("Attempting to validate voice language key '" + langKey + "' before defs are created!");
			return true;
		}
		defs.LoadLanguagesDef(false);
		DT.Field field = global::Defs.FindLanguage(langKey);
		return field != null && field.GetBool("voiced", global::Defs.GetLanguageVars(true), false, true, true, true, '.') && field.GetBool("voice_enabled", global::Defs.GetLanguageVars(true), false, true, true, true, '.');
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x000CA944 File Offset: 0x000C8B44
	private static string GetNativeLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage <= SystemLanguage.Korean)
		{
			if (systemLanguage != SystemLanguage.Chinese)
			{
				if (systemLanguage == SystemLanguage.English)
				{
					return "en";
				}
				switch (systemLanguage)
				{
				case SystemLanguage.French:
					return "fr";
				case SystemLanguage.German:
					return "de";
				case SystemLanguage.Greek:
				case SystemLanguage.Hebrew:
				case SystemLanguage.Hungarian:
				case SystemLanguage.Icelandic:
					goto IL_D0;
				case SystemLanguage.Indonesian:
					return "ind";
				case SystemLanguage.Italian:
					return "it";
				case SystemLanguage.Japanese:
					return "ja";
				case SystemLanguage.Korean:
					return "ko";
				default:
					goto IL_D0;
				}
			}
		}
		else if (systemLanguage <= SystemLanguage.Russian)
		{
			if (systemLanguage == SystemLanguage.Polish)
			{
				return "pl";
			}
			if (systemLanguage != SystemLanguage.Russian)
			{
				goto IL_D0;
			}
			return "ru";
		}
		else
		{
			if (systemLanguage == SystemLanguage.Spanish)
			{
				return "es";
			}
			switch (systemLanguage)
			{
			case SystemLanguage.Turkish:
				return "tr";
			case SystemLanguage.Ukrainian:
			case SystemLanguage.Vietnamese:
				goto IL_D0;
			case SystemLanguage.ChineseSimplified:
				break;
			case SystemLanguage.ChineseTraditional:
				return "zh_tw";
			default:
				goto IL_D0;
			}
		}
		return "zh";
		IL_D0:
		return null;
	}

	// Token: 0x04000CBC RID: 3260
	public static Dictionary<string, UserSettings.SettingChangeData> analyticsSettingsData = new Dictionary<string, UserSettings.SettingChangeData>();

	// Token: 0x04000CBD RID: 3261
	private static DT.Field cloud_settings = null;

	// Token: 0x04000CBE RID: 3262
	private static Dictionary<string, UserSettings.SettingData> sm_Settings = new Dictionary<string, UserSettings.SettingData>();

	// Token: 0x04000CBF RID: 3263
	public static GameObject goOcean = null;

	// Token: 0x04000CC0 RID: 3264
	public static GameObject goPPVs = null;

	// Token: 0x04000CC1 RID: 3265
	public static float DEFAULT_MASTER_VOLUME = 0.8f;

	// Token: 0x04000CC2 RID: 3266
	public static float DEFAULT_MUSIC_VOLUME = 0.8f;

	// Token: 0x04000CC3 RID: 3267
	public static float DEFAULT_VOICE_VOLUME = 0.8f;

	// Token: 0x04000CC4 RID: 3268
	public static float DEFAULT_AMBIENCE_VOLUME = 0.8f;

	// Token: 0x04000CC5 RID: 3269
	public static float DEFAULT_UISOUNDS_VOLUME = 0.8f;

	// Token: 0x04000CC6 RID: 3270
	public static float DEFAULT_SELECTIONSOUNDS_VOLUME = 0.8f;

	// Token: 0x04000CC7 RID: 3271
	public static float DEFAULT_BATTLESOUNDS_VOLUME = 0.8f;

	// Token: 0x04000CF5 RID: 3317
	public static bool InterruptMessages = false;

	// Token: 0x04000D0D RID: 3341
	private static bool ResetTutorial = false;

	// Token: 0x020006A3 RID: 1699
	public class SettingData
	{
		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06004831 RID: 18481 RVA: 0x00216E1A File Offset: 0x0021501A
		public string id
		{
			get
			{
				DT.Field field = this.def;
				if (field == null)
				{
					return null;
				}
				return field.key;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06004832 RID: 18482 RVA: 0x00216E2D File Offset: 0x0021502D
		public bool chnaged
		{
			get
			{
				return this.new_value != this.value;
			}
		}

		// Token: 0x06004833 RID: 18483 RVA: 0x00216E40 File Offset: 0x00215040
		public bool LoadDef(DT.Field defField)
		{
			this.def = defField;
			if (this.def == null)
			{
				return false;
			}
			if (this.def.value.is_unknown)
			{
				return false;
			}
			this.type = this.def.value.type;
			if (this.type == Value.Type.Object)
			{
				if (this.def.value.obj_val is List<DT.SubValue>)
				{
					this.default_value = this.def.Value(0, null, true, true);
					this.min_value = this.def.Value(1, null, true, true);
					this.max_value = this.def.Value(2, null, true, true);
					this.step_value = this.def.Value(3, null, true, true);
				}
				else
				{
					this.default_value = this.def.Value(null, true, true);
				}
			}
			else
			{
				this.default_value = this.def.value;
			}
			this.type = this.default_value.type;
			DT.Field field = this.def.FindChild("min", null, true, true, true, '.');
			if (field != null)
			{
				this.min_value = field.Value(null, true, true);
			}
			DT.Field field2 = this.def.FindChild("max", null, true, true, true, '.');
			if (field2 != null)
			{
				this.max_value = field2.Value(null, true, true);
			}
			DT.Field field3 = this.def.FindChild("step", null, true, true, true, '.');
			if (field3 != null)
			{
				this.step_value = field3.Value(null, true, true);
			}
			this.LoadOptions();
			this.LoadSetting();
			this.Apply(true);
			return true;
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x00216FC8 File Offset: 0x002151C8
		private static Value.Type GetTypeFormValue(DT.Field f)
		{
			if (f == null)
			{
				return Value.Type.Unknown;
			}
			return f.value.type;
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x00216FDC File Offset: 0x002151DC
		public void LoadSetting()
		{
			if (this.type == Value.Type.Int)
			{
				this.new_value = PlayerPrefs.GetInt(this.GetRegKey(), this.default_value.int_val);
			}
			if (this.type == Value.Type.Float)
			{
				this.new_value = PlayerPrefs.GetFloat(this.GetRegKey(), this.default_value.float_val);
			}
			if (this.type == Value.Type.String)
			{
				this.new_value = PlayerPrefs.GetString(this.GetRegKey(), this.default_value.String(null));
			}
			if (this.def.GetString("save_type", null, "", true, true, true, '.') == "steam_cloud")
			{
				if (UserSettings.cloud_settings == null)
				{
					UserSettings.cloud_settings = new DT.Field(null);
					UserSettings.cloud_settings.key = "Settings";
				}
				DT.Field field = UserSettings.cloud_settings.FindChild(this.def.key, null, true, true, true, '.');
				if (field == null)
				{
					field = UserSettings.cloud_settings.SetValue(this.def.key, this.new_value);
				}
				this.new_value = field.Value(null, true, true);
			}
		}

		// Token: 0x06004836 RID: 18486 RVA: 0x002170FC File Offset: 0x002152FC
		public void SaveSetting()
		{
			string @string = this.def.GetString("save_type", null, "local", true, true, true, '.');
			if (this.type == Value.Type.Int)
			{
				PlayerPrefs.SetInt(this.GetRegKey(), this.value.int_val);
			}
			if (this.type == Value.Type.Float)
			{
				PlayerPrefs.SetFloat(this.GetRegKey(), this.value.float_val);
			}
			if (this.type == Value.Type.String)
			{
				PlayerPrefs.SetString(this.GetRegKey(), this.value.String(null));
			}
			if (@string == "steam_cloud")
			{
				if (UserSettings.cloud_settings == null)
				{
					UserSettings.cloud_settings = new DT.Field(null);
					UserSettings.cloud_settings.key = "Settings";
				}
				UserSettings.cloud_settings.SetValue(this.def.key, this.value);
				UserSettings.SaveSteamCloudSettings();
			}
		}

		// Token: 0x06004837 RID: 18487 RVA: 0x002171D0 File Offset: 0x002153D0
		private void LoadOptions()
		{
			this.options = UserSettings.GetOptions(this);
		}

		// Token: 0x06004838 RID: 18488 RVA: 0x002171E0 File Offset: 0x002153E0
		private int DefaultOptionIdx()
		{
			if (this.options == null || this.options.Count == 0)
			{
				return -1;
			}
			int num = this.options.FindIndex((Value o) => o.Equals(this.default_value));
			if (num != -1)
			{
				return num;
			}
			string id = this.id;
			if (!(id == "resoulution"))
			{
				return 0;
			}
			Resolution[] resolutions = Screen.resolutions;
			int systemWidth = Display.main.systemWidth;
			int systemHeight = Display.main.systemHeight;
			int refreshRate = Screen.currentResolution.refreshRate;
			Resolution resolution = default(Resolution);
			int num2 = -1;
			for (int i = 0; i < resolutions.Length; i++)
			{
				if (resolutions[i].width == systemWidth && resolutions[i].height == systemHeight)
				{
					int num3 = Math.Abs(resolutions[i].refreshRate - refreshRate);
					int num4 = Math.Abs(resolution.refreshRate - refreshRate);
					if (num2 == -1 || num3 < num4)
					{
						resolution = resolutions[i];
						num2 = i;
					}
				}
			}
			if (num2 == -1)
			{
				return -1;
			}
			return resolutions.Length - 1 - num2;
		}

		// Token: 0x06004839 RID: 18489 RVA: 0x002172F8 File Offset: 0x002154F8
		public int CalcOptionIndex(Value v, out bool is_default)
		{
			is_default = false;
			if (this.options == null || this.options.Count == 0)
			{
				return -1;
			}
			int num = this.options.FindIndex((Value o) => o.Equals(this.value));
			if (num == -1)
			{
				is_default = true;
				num = this.DefaultOptionIdx();
			}
			return num;
		}

		// Token: 0x0600483A RID: 18490 RVA: 0x00217346 File Offset: 0x00215546
		public void SetValue(Value v)
		{
			this.new_value = v;
		}

		// Token: 0x0600483B RID: 18491 RVA: 0x00217350 File Offset: 0x00215550
		public void ApplyValue(Value v)
		{
			this.value = v;
			this.new_value = v;
			this.SaveSetting();
			UserSettings.ApplySetting(this);
			UserSettings.ApplyOption(this);
		}

		// Token: 0x0600483C RID: 18492 RVA: 0x00217380 File Offset: 0x00215580
		public void Apply(bool is_on_load = false)
		{
			if (this.new_value.Equals(this.value))
			{
				return;
			}
			this.value = this.new_value;
			if (is_on_load)
			{
				if (!this.def.GetBool("apply_on_load", null, true, true, true, true, '.'))
				{
					return;
				}
			}
			else
			{
				this.SaveSetting();
			}
			UserSettings.ApplySetting(this);
			UserSettings.ApplyOption(this);
		}

		// Token: 0x0600483D RID: 18493 RVA: 0x002173DC File Offset: 0x002155DC
		public void Revert()
		{
			this.new_value = this.value;
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x002173EA File Offset: 0x002155EA
		public void ApplyAction()
		{
			UserSettings.ApplyAction(this);
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x002173F2 File Offset: 0x002155F2
		public bool IsDefaultValue()
		{
			return this.value == this.default_value;
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x00217405 File Offset: 0x00215605
		public Value GetMinValue()
		{
			return this.min_value;
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x0021740D File Offset: 0x0021560D
		public Value GetMaxValue()
		{
			return this.max_value;
		}

		// Token: 0x06004842 RID: 18498 RVA: 0x00217415 File Offset: 0x00215615
		public Value GetStepValue()
		{
			return this.step_value;
		}

		// Token: 0x06004843 RID: 18499 RVA: 0x0021741D File Offset: 0x0021561D
		public string GetRegKey()
		{
			return "BSGK_" + this.id;
		}

		// Token: 0x06004844 RID: 18500 RVA: 0x0021742F File Offset: 0x0021562F
		public List<Value> GetOptions()
		{
			return UserSettings.GetOptions(this);
		}

		// Token: 0x06004845 RID: 18501 RVA: 0x00217437 File Offset: 0x00215637
		public int GetCurrentOptionIdx()
		{
			return UserSettings.GetCurrentOptionIdx(this);
		}

		// Token: 0x06004846 RID: 18502 RVA: 0x0021743F File Offset: 0x0021563F
		public override string ToString()
		{
			return string.Format("Setting {0} - {1}, default: {2}", this.def.key, this.value, this.default_value);
		}

		// Token: 0x04003636 RID: 13878
		public Value.Type type;

		// Token: 0x04003637 RID: 13879
		public Value default_value;

		// Token: 0x04003638 RID: 13880
		public Value min_value;

		// Token: 0x04003639 RID: 13881
		public Value max_value;

		// Token: 0x0400363A RID: 13882
		public Value step_value;

		// Token: 0x0400363B RID: 13883
		public Value value;

		// Token: 0x0400363C RID: 13884
		public Value new_value;

		// Token: 0x0400363D RID: 13885
		public List<Value> options;

		// Token: 0x0400363E RID: 13886
		public DT.Field def;
	}

	// Token: 0x020006A4 RID: 1700
	public struct SettingChangeData
	{
		// Token: 0x0400363F RID: 13887
		public Value old_value;

		// Token: 0x04003640 RID: 13888
		public Value new_value;
	}

	// Token: 0x020006A5 RID: 1701
	private class WaitBenchmarkCreation : MonoBehaviour
	{
		// Token: 0x0600484A RID: 18506 RVA: 0x0021748C File Offset: 0x0021568C
		private void Update()
		{
			if (this.waitedFrames > 1000)
			{
				Debug.LogError("Could not load benchmark, waited over 1000 frames");
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (Benchmark.Instance != null)
			{
				UserSettings.RunBenchmark();
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			this.waitedFrames++;
		}

		// Token: 0x04003641 RID: 13889
		private int waitedFrames;
	}
}
