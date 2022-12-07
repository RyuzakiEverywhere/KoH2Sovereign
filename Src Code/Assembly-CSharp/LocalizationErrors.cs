using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class LocalizationErrors
{
	// Token: 0x06000428 RID: 1064 RVA: 0x00032A49 File Offset: 0x00030C49
	public override string ToString()
	{
		return string.Format("{0}: {1} + {2}", this.lang_key, this.num_errors, this.num_warnings);
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00032A74 File Offset: 0x00030C74
	public static string StatsText(out int total_errors, out int total_warnings)
	{
		total_errors = 0;
		total_warnings = 0;
		string text = "";
		foreach (KeyValuePair<string, LocalizationErrors> keyValuePair in LocalizationErrors.per_language)
		{
			LocalizationErrors value = keyValuePair.Value;
			if (value.num_errors != 0 || value.num_warnings != 0)
			{
				total_errors += value.num_errors;
				total_warnings += value.num_warnings;
				if (text != "")
				{
					text += ", ";
				}
				text += string.Format("{0}: {1} + {2}", value.lang_key, value.num_errors, value.num_warnings);
			}
		}
		return text;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00032B44 File Offset: 0x00030D44
	public static void LogStats(bool force_log)
	{
		int num;
		int num2;
		string text = LocalizationErrors.StatsText(out num, out num2);
		if (num == 0 && num2 == 0)
		{
			if (force_log)
			{
				Debug.Log("No localization errors or warnings!");
			}
			return;
		}
		if (LocalizationErrors.log_to_files)
		{
			Debug.LogError(string.Format("Localization errors: {0} + {1}: {2}\nSee {3}/ for details.", new object[]
			{
				num,
				num2,
				text,
				"TextsCache/Localization Errors"
			}));
			return;
		}
		if (num > 0)
		{
			Debug.LogError(string.Format("Localization errors: {0} + {1}: {2}\nRun the texts verification tool for details.", num, num2, text));
		}
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00032BCC File Offset: 0x00030DCC
	public static void ClearAll()
	{
		foreach (KeyValuePair<string, LocalizationErrors> keyValuePair in LocalizationErrors.per_language)
		{
			LocalizationErrors value = keyValuePair.Value;
			value.num_errors = 0;
			value.num_warnings = 0;
			StringBuilder stringBuilder = value.text;
			if (stringBuilder != null)
			{
				stringBuilder.Clear();
			}
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00032C40 File Offset: 0x00030E40
	public static void Log(string lang_key, string message, bool new_error = false, bool is_warning = false)
	{
		if (string.IsNullOrEmpty(lang_key))
		{
			lang_key = "any";
		}
		LocalizationErrors localizationErrors;
		if (!LocalizationErrors.per_language.TryGetValue(lang_key, out localizationErrors))
		{
			localizationErrors = new LocalizationErrors();
			localizationErrors.lang_key = lang_key;
			LocalizationErrors.per_language.Add(lang_key, localizationErrors);
		}
		if (new_error)
		{
			if (is_warning)
			{
				localizationErrors.num_warnings++;
			}
			else
			{
				localizationErrors.num_errors++;
			}
		}
		if (LocalizationErrors.log_to_files)
		{
			if (localizationErrors.text == null)
			{
				localizationErrors.text = new StringBuilder();
			}
			localizationErrors.text.AppendLine(message);
		}
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00032CD0 File Offset: 0x00030ED0
	public static void SaveAll()
	{
		if (!LocalizationErrors.log_to_files)
		{
			return;
		}
		Directory.CreateDirectory("TextsCache/Localization Errors");
		LocalizationErrors.Save("any");
		global::Defs defs = global::Defs.Get(false);
		List<DT.Field> list = (defs != null) ? defs.languages : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field = list[i];
			LocalizationErrors.Save((field != null) ? field.key : null);
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00032D3C File Offset: 0x00030F3C
	private static bool Save(string lang_key)
	{
		if (string.IsNullOrEmpty(lang_key))
		{
			return false;
		}
		string path = "TextsCache/Localization Errors/" + lang_key + ".wiki";
		LocalizationErrors localizationErrors;
		if (!LocalizationErrors.per_language.TryGetValue(lang_key, out localizationErrors) || (localizationErrors.num_errors == 0 && localizationErrors.num_warnings == 0) || localizationErrors.text == null)
		{
			File.Delete(path);
			return false;
		}
		string contents = localizationErrors.text.ToString();
		File.WriteAllText(path, contents, Encoding.UTF8);
		return true;
	}

	// Token: 0x0400043C RID: 1084
	public string lang_key;

	// Token: 0x0400043D RID: 1085
	public int num_errors;

	// Token: 0x0400043E RID: 1086
	public int num_warnings;

	// Token: 0x0400043F RID: 1087
	public StringBuilder text;

	// Token: 0x04000440 RID: 1088
	public static bool log_to_files = false;

	// Token: 0x04000441 RID: 1089
	public const string localization_errors_dir = "TextsCache/Localization Errors";

	// Token: 0x04000442 RID: 1090
	public static Dictionary<string, LocalizationErrors> per_language = new Dictionary<string, LocalizationErrors>();
}
