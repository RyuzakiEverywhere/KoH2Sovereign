using System;

// Token: 0x020001B8 RID: 440
public static class BattleViewPreferences
{
	// Token: 0x06001A19 RID: 6681 RVA: 0x000FCE30 File Offset: 0x000FB030
	public static void SavePreference(string key, bool value)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting(key);
		if (setting == null)
		{
			return;
		}
		setting.ApplyValue(value);
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x000FCE48 File Offset: 0x000FB048
	public static bool GetPreference(string key, bool defaultValue = false)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting(key);
		if (setting == null)
		{
			return defaultValue;
		}
		return setting.value;
	}
}
