using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VassCreatick;

// Token: 0x0200006F RID: 111
[ExecuteInEditMode]
public class Defs : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000381 RID: 897 RVA: 0x0002CA70 File Offset: 0x0002AC70
	public static string texts_path
	{
		get
		{
			ModManager modManager = ModManager.Get(false);
			Mod mod = (modManager != null) ? modManager.GetActiveMod() : null;
			if (modManager != null && mod != null && mod.has_texts)
			{
				return ModManager.GetModsDir() + "/" + mod.texts_path;
			}
			return "texts/";
		}
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0002CAB9 File Offset: 0x0002ACB9
	public static global::Defs Get(bool force_create = false)
	{
		if (global::Defs.instance != null)
		{
			return global::Defs.instance;
		}
		if (!force_create)
		{
			return null;
		}
		global::Defs.instance = new GameObject("DEFS").AddComponent<global::Defs>();
		return global::Defs.instance;
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000383 RID: 899 RVA: 0x0002CAEC File Offset: 0x0002ACEC
	public static int Version
	{
		get
		{
			if (global::Defs.instance == null)
			{
				return 0;
			}
			return global::Defs.instance.dt.num_reloads + global::Defs.instance.num_reloads + global::Defs.instance.num_changes;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000384 RID: 900 RVA: 0x0002CB22 File Offset: 0x0002AD22
	// (set) Token: 0x06000385 RID: 901 RVA: 0x0002CB4D File Offset: 0x0002AD4D
	public static string Language
	{
		get
		{
			if (global::Defs.language == null)
			{
				if (global::Defs.IsPlaying())
				{
					global::Defs.language = UserSettings.Language;
				}
				else
				{
					global::Defs.language = global::Defs.def_language;
				}
			}
			return global::Defs.language;
		}
		set
		{
			global::Defs.language = value;
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0002CB58 File Offset: 0x0002AD58
	public static DT.Field GetDefField(string def_id, string path = null)
	{
		if (def_id == null)
		{
			return null;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null || defs.dt == null)
		{
			return null;
		}
		DT.Def def = defs.dt.FindDef(def_id);
		if (def == null || def.field == null)
		{
			return null;
		}
		if (path == null)
		{
			return def.field;
		}
		return def.field.FindChild(path, null, true, true, true, '.');
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0002CBBC File Offset: 0x0002ADBC
	public static bool GetBool(string def_id, string key, IVars vars = null, bool def_val = false)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetBool(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0002CBE4 File Offset: 0x0002ADE4
	public static bool GetRandomBool(string def_id, string key, IVars vars = null, bool def_val = false)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetRandomBool(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0002CC0C File Offset: 0x0002AE0C
	public static int GetInt(string def_id, string key, IVars vars = null, int def_val = 0)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetInt(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0002CC34 File Offset: 0x0002AE34
	public static int GetRandomInt(string def_id, string key, IVars vars = null, int def_val = 0)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetRandomInt(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0002CC5C File Offset: 0x0002AE5C
	public static float GetFloat(string def_id, string key, IVars vars = null, float def_val = 0f)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetFloat(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0002CC84 File Offset: 0x0002AE84
	public static float GetRandomFloat(string def_id, string key, IVars vars = null, float def_val = 0f)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetRandomFloat(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0002CCAC File Offset: 0x0002AEAC
	public static Point GetPoint(string def_id, string key, IVars vars = null)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return Point.Invalid;
		}
		return defField.GetPoint(key, vars, true, true, true, '.');
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0002CCD8 File Offset: 0x0002AED8
	public static Point GetRandomPoint(string def_id, string key, IVars vars = null)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return Point.Invalid;
		}
		return defField.GetRandomPoint(key, vars, true, true, true, '.');
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0002CD04 File Offset: 0x0002AF04
	public static Vector3 GetVector3(string def_id, string key, IVars vars = null)
	{
		Point point = global::Defs.GetPoint(def_id, key, vars);
		if (point == Point.Invalid)
		{
			return Vector3.zero;
		}
		return global::Common.SnapToTerrain(point, 0f, null, -1f, false);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0002CD44 File Offset: 0x0002AF44
	public static Vector3 GetRandomVector3(string def_id, string key, IVars vars = null)
	{
		Point randomPoint = global::Defs.GetRandomPoint(def_id, key, vars);
		if (randomPoint == Point.Invalid)
		{
			return Vector3.zero;
		}
		return global::Common.SnapToTerrain(randomPoint, 0f, null, -1f, false);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0002CD84 File Offset: 0x0002AF84
	public static string GetString(string def_id, string key, IVars vars = null, string def_val = "")
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetString(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0002CDAC File Offset: 0x0002AFAC
	public static string GetRandomString(string def_id, string key, IVars vars = null, string def_val = "")
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return defField.GetRandomString(key, vars, def_val, true, true, true, '.');
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0002CDD4 File Offset: 0x0002AFD4
	public static Color ColorFromString(string value, Color def_val)
	{
		value = DT.Unquote(value);
		string[] array = value.Split(global::Defs.clr_separators, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 1)
		{
			string text = array[0];
			if (text.StartsWith("0x", StringComparison.Ordinal))
			{
				text = text.Substring(2);
			}
			Color black = Color.black;
			if (ColorUtility.TryParseHtmlString(text, out black))
			{
				return black;
			}
			if (ColorUtility.TryParseHtmlString("#" + text, out black))
			{
				return black;
			}
			return def_val;
		}
		else
		{
			if (array.Length != 3 && array.Length != 4)
			{
				return def_val;
			}
			byte maxValue = byte.MaxValue;
			byte r;
			byte g;
			byte b;
			if (byte.TryParse(array[0], out r) && byte.TryParse(array[1], out g) && byte.TryParse(array[2], out b) && (array.Length == 3 || byte.TryParse(array[3], out maxValue)))
			{
				return new Color32(r, g, b, maxValue);
			}
			return def_val;
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0002CE9E File Offset: 0x0002B09E
	public static Color ColorFromString(string value)
	{
		return global::Defs.ColorFromString(value, Color.black);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0002CEAC File Offset: 0x0002B0AC
	public static string ColorToString(Color clr)
	{
		byte b = (byte)(clr.r * 255f);
		byte b2 = (byte)(clr.g * 255f);
		byte b3 = (byte)(clr.b * 255f);
		byte b4 = (byte)(clr.a * 255f);
		string text = string.Concat(new object[]
		{
			b,
			",",
			b2,
			",",
			b3
		});
		if (b4 != 255)
		{
			text = text + "," + b4;
		}
		return DT.Enquote(text);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0002CF4C File Offset: 0x0002B14C
	public static Color GetColor(string def_id, string key, Color def_val)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return global::Defs.GetColor(defField, key, def_val, null);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0002CF70 File Offset: 0x0002B170
	public static Color GetRandomColor(string def_id, string key, Color def_val)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return global::Defs.GetRandomColor(defField, key, def_val, null);
	}

	// Token: 0x06000398 RID: 920 RVA: 0x0002CF93 File Offset: 0x0002B193
	public static Color GetColor(string def_id, string key)
	{
		return global::Defs.GetColor(def_id, key, Color.black);
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0002CFA1 File Offset: 0x0002B1A1
	public static Color GetRandomColor(string def_id, string key)
	{
		return global::Defs.GetRandomColor(def_id, key, Color.black);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0002CFB0 File Offset: 0x0002B1B0
	public static Color GetColor(DT.Field field, Color def_val, IVars vars = null)
	{
		if (field == null)
		{
			return def_val;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = field.Value(vars, true, true);
		Vars.PopReflectionMode(old_mode);
		object obj_val;
		if ((obj_val = value.obj_val) is Color)
		{
			return (Color)obj_val;
		}
		DT.Field field2;
		if ((field2 = (value.obj_val as DT.Field)) != null)
		{
			return global::Defs.GetColor(field2, def_val, vars);
		}
		if (value.is_string)
		{
			return global::Defs.ColorFromString(value.String(null), def_val);
		}
		return def_val;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0002D020 File Offset: 0x0002B220
	public static Color GetColor(DT.Field field, string key, Color def_val, IVars vars = null)
	{
		if (field == null)
		{
			return def_val;
		}
		Value value = field.GetValue(key, vars, true, true, true, '.');
		object obj_val;
		if ((obj_val = value.obj_val) is Color)
		{
			return (Color)obj_val;
		}
		if (value.is_string)
		{
			return global::Defs.ColorFromString(value.String(null), def_val);
		}
		return def_val;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0002D074 File Offset: 0x0002B274
	public static Color GetRandomColor(DT.Field field, string key, Color def_val, IVars vars = null)
	{
		if (field == null)
		{
			return def_val;
		}
		object obj_val = field.GetRandomValue(key, vars, true, true, true, '.').obj_val;
		if (!(obj_val is Color))
		{
			return def_val;
		}
		return (Color)obj_val;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0002D0A9 File Offset: 0x0002B2A9
	public static Color GetColor(DT.Field field, string key, IVars vars = null)
	{
		return global::Defs.GetColor(field, key, Color.black, vars);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0002D0B8 File Offset: 0x0002B2B8
	public static Color GetRandomColor(DT.Field field, string key, IVars vars = null)
	{
		return global::Defs.GetRandomColor(field, key, Color.black, vars);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0002D0C7 File Offset: 0x0002B2C7
	public static Color GetColor(int idx, string def_id, string key)
	{
		return global::Defs.GetColor(idx, def_id, key, Color.black);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x0002D0D8 File Offset: 0x0002B2D8
	public static Color GetColor(int idx, string def_id, string key, Color def_val)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		if (defField == null)
		{
			return def_val;
		}
		return global::Defs.GetColor(idx, defField, key, def_val, null);
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0002D0FC File Offset: 0x0002B2FC
	public static Color GetColor(int idx, DT.Field field, string key, Color def_val, IVars vars = null)
	{
		if (field == null)
		{
			return def_val;
		}
		object obj_val = field.GetValue(idx, key, vars, true, true, true, '.').obj_val;
		if (!(obj_val is Color))
		{
			return def_val;
		}
		return (Color)obj_val;
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0002D134 File Offset: 0x0002B334
	public static DT.Field SetColor(DT.Field field, string key, Color clr)
	{
		string value_str = global::Defs.ColorToString(clr);
		DT.Field field2 = field.SetValue(key, value_str, clr);
		if (field2 != null)
		{
			field2.comment1 = "//" + ColorUtility.ToHtmlStringRGBA(clr);
		}
		return field2;
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0002D174 File Offset: 0x0002B374
	public static T GetObj<T>(int idx, string def_id, string key, IVars vars = null) where T : UnityEngine.Object
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		return global::Defs.GetObj<T>(idx, defField, key, vars);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x0002D194 File Offset: 0x0002B394
	public static T GetObj<T>(int idx, DT.Field field, string key, IVars vars = null) where T : UnityEngine.Object
	{
		if (field == null)
		{
			return default(T);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		T result = field.GetValue(idx, key, vars, true, true, true, '.').obj_val as T;
		Vars.PopReflectionMode(old_mode);
		return result;
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0002D1D8 File Offset: 0x0002B3D8
	public static T GetRandomObj<T>(string def_id, string key, IVars vars = null) where T : UnityEngine.Object
	{
		return global::Defs.GetRandomObj<T>(global::Defs.GetDefField(def_id, null), key, vars);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0002D1E8 File Offset: 0x0002B3E8
	public static T GetRandomObj<T>(DT.Field field, string key, IVars vars = null) where T : UnityEngine.Object
	{
		if (field == null)
		{
			return default(T);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		T result = field.GetRandomValue(key, vars, true, true, true, '.').obj_val as T;
		Vars.PopReflectionMode(old_mode);
		return result;
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0002D22C File Offset: 0x0002B42C
	public static T GetRandomObj<T>(DT.Field field, IVars vars = null) where T : UnityEngine.Object
	{
		if (field == null)
		{
			return default(T);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		T result = field.RandomValue(vars, true, true).obj_val as T;
		Vars.PopReflectionMode(old_mode);
		return result;
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0002D26B File Offset: 0x0002B46B
	public static T GetObj<T>(string def_id, string key, IVars vars = null) where T : UnityEngine.Object
	{
		return global::Defs.GetObj<T>(global::Defs.GetDefField(def_id, null), key, vars);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0002D27C File Offset: 0x0002B47C
	public static T GetObj<T>(DT.Field field, IVars vars = null) where T : UnityEngine.Object
	{
		if (field == null)
		{
			return default(T);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		object obj_val = field.Value(vars, true, true).obj_val;
		T t = obj_val as T;
		DT.Field field2;
		while (t == null && (field2 = (obj_val as DT.Field)) != null)
		{
			obj_val = field2.Value(vars, true, true).obj_val;
			t = (obj_val as T);
		}
		Vars.PopReflectionMode(old_mode);
		return t;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0002D2F8 File Offset: 0x0002B4F8
	public static T GetObj<T>(DT.Field field, string key, IVars vars = null) where T : UnityEngine.Object
	{
		if (field == null)
		{
			return default(T);
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		T result = field.GetValue(key, vars, true, true, true, '.').obj_val as T;
		Vars.PopReflectionMode(old_mode);
		return result;
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0002D33C File Offset: 0x0002B53C
	public static global::Defs.TextInfo GetTextInfo(string text_key, bool create_if_not_found = false, bool look_in_defs = true)
	{
		if (string.IsNullOrEmpty(text_key))
		{
			return null;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		if (text_key.EndsWith(".fallback", StringComparison.Ordinal))
		{
			text_key = text_key.Substring(0, text_key.Length - 9);
		}
		global::Defs.TextInfo textInfo;
		if (defs.texts.TryGetValue(text_key, out textInfo))
		{
			return textInfo;
		}
		int num = text_key.LastIndexOf('.');
		if (num > 0 && num + 1 < text_key.Length && global::Defs.FindLanguage(text_key.Substring(num + 1)) != null)
		{
			string key = text_key.Substring(0, num);
			if (defs.texts.TryGetValue(key, out textInfo))
			{
				return textInfo;
			}
		}
		if (look_in_defs)
		{
			DT dt = defs.dt;
			if (((dt != null) ? dt.roots : null) != null)
			{
				Profile.BeginSection("FindTextField.DT.Find");
				DT dt2 = defs.dt;
				DT.Field field = (dt2 != null) ? dt2.Find(text_key, null) : null;
				Profile.EndSection("FindTextField.DT.Find");
				if (field != null)
				{
					global::Defs.TextInfo textInfo2 = new global::Defs.TextInfo
					{
						text_key = text_key,
						def_field = field,
						has_cases = DT.find_had_cases
					};
					defs.texts.Add(text_key, textInfo2);
					return textInfo2;
				}
			}
		}
		if (!create_if_not_found)
		{
			return null;
		}
		textInfo = new global::Defs.TextInfo
		{
			text_key = text_key
		};
		defs.texts.Add(text_key, textInfo);
		return textInfo;
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0002D474 File Offset: 0x0002B674
	public global::Defs.TextInfo AddText(string text_key, DT.Field field, string lang_key = null, bool error_if_duplicate = true)
	{
		if (field == null)
		{
			return null;
		}
		global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, true, true);
		this.AddText(textInfo, field, lang_key, error_if_duplicate);
		return textInfo;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0002D49C File Offset: 0x0002B69C
	public void AddText(global::Defs.TextInfo ti, DT.Field field, string lang_key = null, bool error_if_duplicate = true)
	{
		if (ti == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(lang_key))
		{
			lang_key = (global::Defs.loading_language ?? global::Defs.Language);
		}
		int idx;
		global::Defs.TextInfo.LangInfo languageInfo = ti.GetLanguageInfo(lang_key, out idx, true);
		if (languageInfo.localized_field != null && error_if_duplicate && !this.CheckDuplicate(languageInfo.localized_field, field))
		{
			return;
		}
		languageInfo.localized_field = field;
		ti.SetLanguageInfo(languageInfo, idx);
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0002D500 File Offset: 0x0002B700
	public static DT.Field FindLanguage(string key)
	{
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		for (int i = 0; i < defs.languages.Count; i++)
		{
			DT.Field field = defs.languages[i];
			if (!(field.key != key))
			{
				return field;
			}
		}
		return null;
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0002D554 File Offset: 0x0002B754
	public static Vars GetLanguageVars(bool force_rebuild = true)
	{
		if (global::Defs.language_vars != null && !force_rebuild)
		{
			return global::Defs.language_vars;
		}
		global::Defs.language_vars = new Vars();
		string val = Title.BranchName();
		global::Defs.language_vars.Set<string>("branch_name", val);
		bool val2 = Title.IsInternalBranch();
		global::Defs.language_vars.Set<bool>("is_dev_branch", val2);
		return global::Defs.language_vars;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0002D5AC File Offset: 0x0002B7AC
	public static int GetLanguageVersion(string key)
	{
		DT.Field field = global::Defs.FindLanguage(key);
		if (field == null)
		{
			return -1;
		}
		return field.GetInt("version", null, global::Defs.default_language_version, true, true, true, '.');
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0002D5DB File Offset: 0x0002B7DB
	public static string GetLanguageVersionSuffix(int version)
	{
		if (version <= 1)
		{
			return "";
		}
		return string.Format(";{0}", version);
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0002D5F8 File Offset: 0x0002B7F8
	private void LoadLanguagesCSV(bool load_all)
	{
		if (!load_all && global::Defs.IsDevLanguage() && global::Defs.enable_proofread_texts_in_dev <= 0)
		{
			return;
		}
		this.BuildLanguagesCSVRemap(load_all);
		global::Defs.import_forms_as_languages = true;
		this.LoadTextFile(global::Defs.texts_path + "languages.csv");
		this.LoadTextFile(global::Defs.texts_path + "VO.csv");
		global::Defs.import_forms_as_languages = false;
		Table.remap_cells = null;
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0002D65B File Offset: 0x0002B85B
	private void LoadLanguageCache(bool load_all)
	{
		if (!load_all && global::Defs.enable_proofread_texts_in_dev <= 0)
		{
			return;
		}
		global::Defs.LoadLanguageCache("dev", -1, load_all);
		global::Defs.LoadLanguageCache("en", -1, load_all);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0002D684 File Offset: 0x0002B884
	public static void LoadLanguageCache(string lang_key, int version, bool load_all)
	{
		if (version < 0)
		{
			version = global::Defs.GetLanguageVersion(lang_key);
		}
		if (version <= 0 && !(lang_key == "en") && !(lang_key == "dev"))
		{
			return;
		}
		string languageVersionSuffix = global::Defs.GetLanguageVersionSuffix(version);
		string path;
		if (!load_all && (lang_key == "en" || lang_key == "dev"))
		{
			path = global::Defs.texts_path + lang_key + ".cache";
		}
		else
		{
			path = "TextsCache/" + lang_key + languageVersionSuffix + ".csv";
		}
		string text = DT.ReadTextFile(path);
		DT.Field field = DT.ParseCSV(null, path, text, '\0', "base", "");
		if (((field != null) ? field.children : null) == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field2 = field.children[i];
			if (field2.base_path != null)
			{
				DT.Field field3 = field2;
				field3.key = field3.key + ":" + field2.base_path;
				field2.base_path = null;
			}
			if (field2.value.is_number)
			{
				field2.value = field2.value_str;
			}
			else if (field2.value.is_unknown)
			{
				field2.value = "";
			}
			int num = field2.key.IndexOf('#');
			int num2 = field2.key.IndexOf(':');
			if (num >= 0)
			{
				if (num2 >= 0)
				{
					Debug.LogError(field2.Path(true, false, '.') + ": form variants are not supported");
				}
				else
				{
					string text_key = field2.key.Substring(0, num);
					int variant;
					if (!int.TryParse(field2.key.Substring(num + 1), out variant))
					{
						Debug.LogError(field2.Path(true, false, '.') + ": invalid variant");
					}
					else
					{
						global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, false, true);
						DT.Field cached_field = ((textInfo == null) ? default(global::Defs.TextInfo.LangInfo) : textInfo.GetLanguageInfo(lang_key, false)).cached_field;
						if (cached_field == null)
						{
							Debug.LogError(field2.Path(true, false, '.') + ": base variant not found");
						}
						else
						{
							global::Defs.AddVariant(cached_field, field2, variant);
						}
					}
				}
			}
			else if (num2 >= 0)
			{
				string text_key2 = field2.key.Substring(0, num2);
				string key = field2.key.Substring(num2 + 1);
				global::Defs.TextInfo textInfo2 = global::Defs.GetTextInfo(text_key2, false, true);
				DT.Field cached_field2 = ((textInfo2 == null) ? default(global::Defs.TextInfo.LangInfo) : textInfo2.GetLanguageInfo(lang_key, false)).cached_field;
				if (cached_field2 == null)
				{
					Debug.LogError(field2.Path(true, false, '.') + ": base form not found");
				}
				else
				{
					field2.key = key;
					cached_field2.AddChild(field2);
				}
			}
			else
			{
				global::Defs.TextInfo textInfo3 = global::Defs.GetTextInfo(field2.key, true, true);
				int idx;
				global::Defs.TextInfo.LangInfo languageInfo = textInfo3.GetLanguageInfo(lang_key, out idx, true);
				languageInfo.cached_field = field2;
				textInfo3.SetLanguageInfo(languageInfo, idx);
			}
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0002D971 File Offset: 0x0002BB71
	public void LoadResolvedConflicts()
	{
		global::Defs.resolved_texts.Clear();
		global::Defs.loading_language = "resolved";
		global::Defs.store_loaded_texts_in = global::Defs.resolved_texts;
		this.LoadTextFile("TextsCache/resolved conflicts.csv");
		global::Defs.store_loaded_texts_in = null;
		global::Defs.loading_language = null;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0002D9A8 File Offset: 0x0002BBA8
	public static string RomanNumber(int val)
	{
		if (val >= 40)
		{
			return val.ToString();
		}
		int num = val / 10;
		string str = "";
		for (int i = 0; i < num; i++)
		{
			str += "X";
		}
		int num2 = val % 10;
		return str + global::Defs.RomanDigits[num2];
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0002D9FC File Offset: 0x0002BBFC
	public static string LocalizedNumber(Value val, IVars vars = null, string form = "")
	{
		string text;
		if (global::Defs.ExtractForm(ref form, "roman"))
		{
			text = global::Defs.RomanNumber(val.Int(0));
		}
		else if (val.type != Value.Type.Float)
		{
			text = DT.String(val, null);
		}
		else if (global::Defs.ExtractForm(ref form, "F"))
		{
			text = DT.FloatToStr(val.float_val, -1);
		}
		else if (global::Defs.ExtractForm(ref form, "F0"))
		{
			text = DT.FloatToStr(val.float_val, 0);
		}
		else if (global::Defs.ExtractForm(ref form, "F1"))
		{
			text = DT.FloatToStr(val.float_val, 1);
		}
		else if (global::Defs.ExtractForm(ref form, "F2"))
		{
			text = DT.FloatToStr(val.float_val, 2);
		}
		else if (global::Defs.ExtractForm(ref form, "F3"))
		{
			text = DT.FloatToStr(val.float_val, 3);
		}
		else
		{
			text = DT.FloatToStr(val.float_val, -1);
		}
		bool flag = text == "0";
		if (global::Defs.ExtractForm(ref form, "nonzero") && flag)
		{
			return null;
		}
		if (global::Defs.ExtractForm(ref form, "%"))
		{
			string text2 = global::Defs.Localize("percent_after_number", null, null, false, true);
			if (text2 == null)
			{
				text2 = "%";
			}
			text += text2;
		}
		if (string.IsNullOrEmpty(form))
		{
			return text;
		}
		Vars vars2 = new Vars(vars);
		vars2.Set<string>("value", "#" + text);
		string text3 = (text[0] == '-') ? "negative" : (flag ? "zero" : "positive");
		if (vars != null && vars.GetVar("is_tooltip", null, true))
		{
			text3 += "_tooltip";
		}
		string text4 = global::Defs.Localize(form, vars2, text3, false, false);
		if (text4 != null)
		{
			return text4;
		}
		return text;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0002DBB4 File Offset: 0x0002BDB4
	public static string LocalizeStatModifier(string stats, string stat_name, float val, Stat.Modifier.Type mod_type, IVars vars = null, bool dark = false, bool no_null = true)
	{
		DT.Field defField = global::Defs.GetDefField(stats, stat_name);
		bool flag = mod_type == Stat.Modifier.Type.Perc || stat_name.EndsWith("_perc", StringComparison.Ordinal);
		DT.Field field = (defField != null) ? defField.FindChild("mod_text", null, true, true, true, '.') : null;
		if (field == null)
		{
			field = global::Defs.GetDefField(stats, "mod_text");
		}
		if (field == null)
		{
			field = global::Defs.GetDefField("Stats", "mod_text");
		}
		if (field == null)
		{
			return global::Defs.LocalizeNull(stat_name, null, no_null);
		}
		string text = "F1";
		if (val == 0f || (vars != null && vars.GetVar("inactive", null, true).Bool()))
		{
			text += ":no_buff";
		}
		else if (defField != null && defField.GetBool("penalty", null, false, true, true, true, '.'))
		{
			text += ":penalty";
		}
		else
		{
			text += ":bonus";
		}
		if (dark)
		{
			text += "_dark";
		}
		if (defField != null && !defField.GetBool("show_plus_sign", null, true, true, true, true, '.'))
		{
			text += "_no_sign";
		}
		if (flag)
		{
			text += ":%";
		}
		DT.Field field2 = (defField != null) ? defField.FindChild("name", null, true, true, true, '.') : null;
		Vars vars2 = new Vars(vars);
		vars2.Set<string>("value", "#" + global::Defs.LocalizedNumber(val, null, text));
		vars2.Set<bool>("zero_value", val == 0f);
		if (field2 != null)
		{
			vars2.Set<DT.Field>("name", field2);
		}
		else
		{
			vars2.Set<string>("name", stat_name);
		}
		Stats.Def def = Def.Get<Stats.Def>(defField.parent);
		Stat.Def def2 = (def != null) ? def.FindStat(defField.key, false) : null;
		vars2.Set<Stat.Def>("stat_def", def2);
		if (def2 != null)
		{
			Value val2 = def2.CalcMultiplier(vars);
			vars2.Set<Value>("multiplier", val2);
		}
		return global::Defs.Localize(field, vars2, text, no_null, true);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0002DDA4 File Offset: 0x0002BFA4
	public static string LocalizedObjName(Logic.Object obj, IVars vars = null, string form = "", bool no_null = true)
	{
		if (form == "link")
		{
			return "<link=\"obj:" + (Logic.Object.IsValid(obj) ? obj.uid : 0U) + "\">";
		}
		if (form == "/link")
		{
			return "</link>";
		}
		if (obj == null)
		{
			return null;
		}
		bool flag = !obj.IsValid();
		if (form == "nolink")
		{
			flag = true;
			form = "";
		}
		else if (form != null && form.EndsWith(":nolink", StringComparison.Ordinal))
		{
			flag = true;
			form = form.Substring(0, form.Length - 7);
		}
		string nameKey = obj.GetNameKey(vars, form);
		if (nameKey != null)
		{
			Vars vars2 = new Vars(obj);
			vars2.Set<IVars>("vars", vars);
			string text = global::Defs.Localize(nameKey, vars2, form, no_null, false);
			if (!flag && !string.IsNullOrEmpty(text))
			{
				text = string.Concat(new object[]
				{
					"<link=\"obj:",
					obj.uid,
					"\">",
					text,
					"</link>"
				});
			}
			return text;
		}
		if (!no_null)
		{
			return null;
		}
		return "{" + obj.ToString() + "}";
	}

	// Token: 0x060003BA RID: 954 RVA: 0x0002DECE File Offset: 0x0002C0CE
	public static string LocalizedDefLink(Def def, IVars vars = null, string form = "", bool no_null = true)
	{
		if (def == null)
		{
			return null;
		}
		return global::Defs.LocalizedDefLink(def.field, vars, form, no_null);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0002DEE4 File Offset: 0x0002C0E4
	public static string LocalizedDefLink(DT.Field def_field, IVars vars = null, string form = "", bool no_null = true)
	{
		if (def_field == null)
		{
			return null;
		}
		string text = def_field.Path(false, false, '.');
		if (form == "link")
		{
			return "<link=\"def:" + text + "\">";
		}
		if (form == "/link")
		{
			return "</link>";
		}
		bool flag = false;
		if (form == "nolink")
		{
			flag = true;
			form = "";
		}
		else if (form != null && form.EndsWith(":nolink", StringComparison.Ordinal))
		{
			flag = true;
			form = form.Substring(0, form.Length - 7);
		}
		string text_key = text + ".name";
		Vars vars2 = new Vars(def_field);
		vars2.Set<IVars>("vars", vars);
		string text2 = global::Defs.Localize(text_key, vars2, form, no_null, false);
		if (!flag && !string.IsNullOrEmpty(text2))
		{
			text2 = string.Concat(new string[]
			{
				"<link=\"def:",
				text,
				"\">",
				text2,
				"</link>"
			});
		}
		return text2;
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0002DFD0 File Offset: 0x0002C1D0
	public static string LocalizedObjName(object obj, IVars vars = null, string form = "", bool no_null = true)
	{
		if (obj == null)
		{
			if (form == "link")
			{
				return "<link=\"obj:0\">";
			}
			if (form == "/link")
			{
				return "</link>";
			}
			return null;
		}
		else
		{
			Logic.Object @object = obj as Logic.Object;
			if (@object != null)
			{
				return global::Defs.LocalizedObjName(@object, vars, form, no_null);
			}
			IList list = obj as IList;
			if (list != null)
			{
				return global::Defs.LocalizeList(list, vars, form, null, null, false);
			}
			Def def;
			if ((def = (obj as Def)) != null)
			{
				return global::Defs.LocalizedDefLink(def, vars, form, no_null);
			}
			string text = null;
			Type type = obj.GetType();
			MethodInfo method = type.GetMethod("GetNameKey", Vars.GetNameKeyParams);
			if (method != null)
			{
				try
				{
					text = (method.Invoke(obj, new object[]
					{
						vars,
						form
					}) as string);
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in LocalizedObjName(): " + ex.ToString());
					return null;
				}
			}
			if (text == null)
			{
				FieldInfo field = type.GetField("name");
				if (field != null)
				{
					text = (field.GetValue(obj) as string);
				}
			}
			if (text != null)
			{
				Vars vars2 = new Vars(obj);
				vars2.Set<IVars>("vars", vars);
				return global::Defs.Localize(text, vars2, form, no_null, false);
			}
			if (!no_null)
			{
				return null;
			}
			return "{" + obj.ToString() + "}";
		}
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0002E124 File Offset: 0x0002C324
	private static char SkipText(string text, char context_type, ref int idx)
	{
		while (idx < text.Length)
		{
			char c = text[idx];
			if (c == '[' || c == ']' || c == '{' || c == '}' || c == '\\')
			{
				return c;
			}
			if (c == '|' && context_type == '[')
			{
				return c;
			}
			idx++;
		}
		return '\0';
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0002E174 File Offset: 0x0002C374
	private static char PopContext(string text, char context_type, ref int idx)
	{
		int num = 1;
		char c;
		for (;;)
		{
			c = global::Defs.SkipText(text, context_type, ref idx);
			if (c == '\0')
			{
				break;
			}
			if (c == '[' || c == '{')
			{
				num++;
			}
			else if ((c == ']' || c == '}') && --num == 0)
			{
				return c;
			}
			idx++;
		}
		return c;
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0002E1BC File Offset: 0x0002C3BC
	public static bool ExtractForm(ref string forms, string form)
	{
		if (string.IsNullOrEmpty(forms) || string.IsNullOrEmpty(form))
		{
			return false;
		}
		int i = 0;
		while (i < forms.Length)
		{
			i = forms.IndexOf(form, i, StringComparison.Ordinal);
			if (i < 0)
			{
				return false;
			}
			int num = i + form.Length;
			if (num < forms.Length && forms[num] != ':')
			{
				i = num;
			}
			else
			{
				if (i <= 0 || forms[i - 1] == ':')
				{
					if (i == 0)
					{
						if (forms.Length == form.Length)
						{
							forms = "";
						}
						else
						{
							forms = forms.Remove(i, form.Length + 1);
						}
					}
					else
					{
						forms = forms.Remove(i - 1, form.Length + 1);
					}
					return true;
				}
				i = num;
			}
		}
		return false;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0002E280 File Offset: 0x0002C480
	public static int FindFirstNonMarkupCharIdx(string text)
	{
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			if (text[i] != '<')
			{
				return i;
			}
			i = text.IndexOf('>', i);
			if (i < 0)
			{
				return -1;
			}
		}
		return -1;
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0002E2C0 File Offset: 0x0002C4C0
	public static string LowerCase(string text)
	{
		global::Defs.tmp_LowerCaseSB.Clear();
		int i = 0;
		int length = text.Length;
		while (i < length)
		{
			int num = text.IndexOf('<', i);
			if (num < 0)
			{
				break;
			}
			if (num > i)
			{
				global::Defs.tmp_LowerCaseSB.Append(text.Substring(i, num - i).ToLowerInvariant());
			}
			int num2 = text.IndexOf('>', num);
			if (num2 < 0)
			{
				global::Defs.tmp_LowerCaseSB.Append('<');
				i = num + 1;
			}
			else
			{
				global::Defs.tmp_LowerCaseSB.Append(text.Substring(num, num2 - num + 1));
				i = num2 + 1;
			}
		}
		if (global::Defs.tmp_LowerCaseSB == null)
		{
			return text;
		}
		if (i < length)
		{
			global::Defs.tmp_LowerCaseSB.Append(text.Substring(i, length - i).ToLowerInvariant());
		}
		return global::Defs.tmp_LowerCaseSB.ToString();
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0002E380 File Offset: 0x0002C580
	public static string Capitalize(string text)
	{
		int num = global::Defs.FindFirstNonMarkupCharIdx(text);
		if (num < 0)
		{
			return text;
		}
		string text2 = "";
		if (num > 0)
		{
			text2 += text.Substring(0, num);
		}
		text2 += char.ToUpperInvariant(text[num]).ToString();
		if (num + 1 < text.Length)
		{
			text2 += text.Substring(num + 1);
		}
		return text2;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0002E3EC File Offset: 0x0002C5EC
	private static bool PushRVStack(string key, string form, DT.Field ctx_field, IVars ctx_vars)
	{
		for (int i = 0; i < global::Defs.rv_stack.Count; i++)
		{
			global::Defs.RVStackItem rvstackItem = global::Defs.rv_stack[i];
			if (rvstackItem.key == key && rvstackItem.form == form && rvstackItem.ctx_field == ctx_field && rvstackItem.ctx_vars == ctx_vars)
			{
				Debug.LogError(string.Format("Infinite loop in ReplaceVar, key: '{0}', field: {1}", key, ctx_field));
				return false;
			}
		}
		global::Defs.rv_stack.Add(new global::Defs.RVStackItem
		{
			key = key,
			form = form,
			ctx_field = ctx_field,
			ctx_vars = ctx_vars
		});
		return true;
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0002E48F File Offset: 0x0002C68F
	private static void PopRVStack()
	{
		if (global::Defs.rv_stack.Count == 0)
		{
			return;
		}
		global::Defs.rv_stack.RemoveAt(global::Defs.rv_stack.Count - 1);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0002E4B4 File Offset: 0x0002C6B4
	public static string ReplaceVar(string text, IVars vars, bool no_null = true)
	{
		int num = text.IndexOf(':');
		string text2;
		string text3;
		if (num >= 0)
		{
			text2 = text.Substring(0, num);
			text3 = text.Substring(num + 1);
		}
		else
		{
			text2 = text;
			text3 = "";
		}
		if (text2 == "dump_context" || text2 == global::Defs.debug_replace_var)
		{
			Debug.Log(string.Format("Localizing {0}\nContext: {1}", global::Defs.currently_localizing, Logic.Object.Dump(vars)));
			if (text2 == "dump_context")
			{
				return "";
			}
		}
		if (global::Defs.PushRVStack(text2, text3, global::Defs.currently_localizing, vars))
		{
			bool flag = global::Defs.ExtractForm(ref text3, "lowercase");
			bool flag2 = global::Defs.ExtractForm(ref text3, "cap");
			bool flag3 = global::Defs.ExtractForm(ref text3, "if");
			bool flag4 = global::Defs.ExtractForm(ref text3, "if_not");
			bool flag5 = global::Defs.ExtractForm(ref text3, "if_one");
			bool as_value = global::Defs.ExtractForm(ref text3, "value");
			bool flag6 = global::Defs.ExtractForm(ref text3, "#");
			string text4 = null;
			global::Defs defs = global::Defs.Get(false);
			DefsContext defsContext;
			if (defs == null)
			{
				defsContext = null;
			}
			else
			{
				DT dt = defs.dt;
				defsContext = ((dt != null) ? dt.context : null);
			}
			DefsContext defsContext2 = defsContext;
			if (defsContext2 != null)
			{
				Profile.BeginSection("Defs.ReplaceVar.GetVar");
				Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
				DT.Field field = defsContext2.field;
				IVars vars2 = defsContext2.vars;
				defsContext2.field = global::Defs.currently_localizing;
				defsContext2.vars = vars;
				Value value = defsContext2.GetVarPath(text2, as_value);
				defsContext2.field = field;
				defsContext2.vars = vars2;
				Profile.EndSection("Defs.ReplaceVar.GetVar");
				if (global::Defs.currently_localizing != null && value.obj_val == global::Defs.currently_localizing)
				{
					value = Value.Unknown;
				}
				if (flag3)
				{
					Vars.PopReflectionMode(old_mode);
					global::Defs.PopRVStack();
					DT.Field field2;
					if ((field2 = (value.obj_val as DT.Field)) != null)
					{
						if (!field2.Bool(vars, false))
						{
							return null;
						}
						return "";
					}
					else
					{
						if (!value.Bool())
						{
							return null;
						}
						return "";
					}
				}
				else if (flag4)
				{
					Vars.PopReflectionMode(old_mode);
					global::Defs.PopRVStack();
					DT.Field field3;
					if ((field3 = (value.obj_val as DT.Field)) != null)
					{
						if (field3.Bool(vars, false))
						{
							return null;
						}
						return "";
					}
					else
					{
						if (value.Bool())
						{
							return null;
						}
						return "";
					}
				}
				else if (flag5)
				{
					Vars.PopReflectionMode(old_mode);
					global::Defs.PopRVStack();
					IList list;
					if ((list = (value.obj_val as IList)) != null)
					{
						if (list.Count != 1)
						{
							return null;
						}
						return "";
					}
					else
					{
						if (value.Int(0) != 1)
						{
							return null;
						}
						return "";
					}
				}
				else
				{
					DT.Field field4;
					Stat stat;
					if (value.type == Value.Type.String)
					{
						if (flag6)
						{
							text4 = value.String(null);
						}
						else
						{
							text4 = global::Defs.Localize(value, vars, text3, no_null, false);
						}
					}
					else if (value.is_number)
					{
						text4 = global::Defs.LocalizedNumber(value, vars, text3);
					}
					else if ((field4 = (value.obj_val as DT.Field)) != null)
					{
						if (flag6)
						{
							text4 = field4.String(vars, "");
						}
						else
						{
							string a = field4.Type();
							if (a == "def" || a == "template")
							{
								text4 = global::Defs.LocalizedDefLink(field4, vars, text3, no_null);
							}
							else
							{
								text4 = global::Defs.Localize(field4, vars, text3, no_null, false);
							}
						}
					}
					else if ((stat = (value.obj_val as Stat)) != null)
					{
						text4 = global::Defs.LocalizedNumber(stat.CalcValue(false), vars, text3);
					}
					else
					{
						text4 = global::Defs.LocalizedObjName(value.obj_val, vars, text3, no_null);
					}
					Vars.PopReflectionMode(old_mode);
				}
			}
			if (text4 == null)
			{
				text4 = global::Defs.Localize(text2, vars, text3, false, false);
				if (text4 == null)
				{
					if (!no_null)
					{
						global::Defs.PopRVStack();
						return null;
					}
					text4 = "{" + text2;
					if (text3 != "")
					{
						text4 = text4 + ":" + text3;
					}
					text4 += "}";
				}
			}
			if (flag)
			{
				text4 = global::Defs.LowerCase(text4);
			}
			if (flag2)
			{
				text4 = global::Defs.Capitalize(text4);
			}
			global::Defs.PopRVStack();
			return text4;
		}
		if (!no_null)
		{
			return null;
		}
		string str = "{" + text2;
		if (text3 != "")
		{
			str = str + ":" + text3;
		}
		return str + "}";
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0002E8C4 File Offset: 0x0002CAC4
	public static string ReplaceVars(string text, IVars vars, bool no_null = true, char context_type = '\0')
	{
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		bool flag = false;
		int num = 0;
		Profile.BeginSection("Defs.ReplaceVars");
		for (;;)
		{
			int num2 = num;
			char c = global::Defs.SkipText(text, context_type, ref num2);
			if (c == '\0' && num == 0)
			{
				break;
			}
			if (num2 > num && stringBuilder != null)
			{
				stringBuilder.Append(text, num, num2 - num);
			}
			if (c == '\0')
			{
				goto Block_5;
			}
			if (c == '\\')
			{
				num2++;
				if (num2 >= text.Length)
				{
					stringBuilder.Append('\\');
					num = num2;
				}
				else
				{
					c = text[num2];
					stringBuilder.Append(c);
					num = num2 + 1;
				}
			}
			else if (c == ']' || c == '}')
			{
				if (!no_null)
				{
					flag = true;
					stringBuilder.Clear();
				}
				if (!flag)
				{
					stringBuilder.Append(c);
				}
				num = num2 + 1;
			}
			else if (c == '[')
			{
				num2 = (num = num2 + 1);
				c = global::Defs.PopContext(text, c, ref num2);
				if (c != ']')
				{
					if (!no_null)
					{
						flag = true;
						stringBuilder.Clear();
					}
					if (!flag)
					{
						stringBuilder.Append(text, num - 1, num2 - num);
					}
					num = num2 + 1;
				}
				else
				{
					if (!flag)
					{
						string text2 = text.Substring(num, num2 - num);
						Profile.EndSection("Defs.ReplaceVars");
						string text3 = global::Defs.ReplaceVars(text2, vars, false, '[');
						Profile.BeginSection("Defs.ReplaceVars");
						if (text3 != null)
						{
							stringBuilder.Append(text3);
						}
					}
					num = num2 + 1;
				}
			}
			else if (c == '{')
			{
				num2 = (num = num2 + 1);
				c = global::Defs.PopContext(text, c, ref num2);
				if (c != '}')
				{
					if (!no_null)
					{
						flag = true;
						stringBuilder.Clear();
					}
					if (!flag)
					{
						stringBuilder.Append(text, num - 1, num2 + 1 - num);
					}
					num = num2 + 1;
				}
				else
				{
					if (!flag)
					{
						string text4 = text.Substring(num, num2 - num);
						Profile.EndSection("Defs.ReplaceVars");
						string text5 = global::Defs.ReplaceVar(text4, vars, no_null);
						Profile.BeginSection("Defs.ReplaceVars");
						if (text4 == global::Defs.debug_replace_var)
						{
							Debug.Log("'" + text4 + "' -> " + text5);
						}
						if (text5 == null)
						{
							flag = true;
							stringBuilder.Clear();
						}
						else
						{
							stringBuilder.Append(text5);
						}
					}
					num = num2 + 1;
				}
			}
			else if (c == '|')
			{
				if (!flag)
				{
					goto Block_26;
				}
				flag = false;
				stringBuilder.Clear();
				num = num2 + 1;
			}
			else
			{
				if (!no_null)
				{
					flag = true;
					stringBuilder.Clear();
				}
				if (!flag)
				{
					stringBuilder.Append(c);
				}
				num = num2 + 1;
			}
		}
		Profile.EndSection("Defs.ReplaceVars");
		return text;
		Block_5:
		if (flag)
		{
			Profile.EndSection("Defs.ReplaceVars");
			return null;
		}
		string result = stringBuilder.ToString();
		Profile.EndSection("Defs.ReplaceVars");
		return result;
		Block_26:
		string result2 = stringBuilder.ToString();
		Profile.EndSection("Defs.ReplaceVars");
		return result2;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0002EB40 File Offset: 0x0002CD40
	public static string ReplaceFormatting(string s)
	{
		s = s.Replace("<p>", "\n");
		int num = 0;
		for (;;)
		{
			num = s.IndexOf('&', num);
			if (num < 0 || num + 1 >= s.Length)
			{
				break;
			}
			s = string.Concat(new string[]
			{
				s.Substring(0, num),
				"<color=#9c190f>",
				s[num + 1].ToString(),
				"</color>",
				s.Substring(num + 2)
			});
		}
		return s;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0002EBC8 File Offset: 0x0002CDC8
	private static string GetLocalized(DT.Field field, string form, IVars vars)
	{
		string a = field.Type();
		bool flag = field.dt != null && a != "text";
		if (field.dt != null && !flag && !global::Defs.IsDevLanguage() && global::Defs.enable_fallback_texts <= 1 && !global::Defs.TextInfo.IsEmpty(field))
		{
			Debug.LogWarning(string.Format("Localizing def text: {0}", field));
		}
		if (a == "resources")
		{
			return global::Defs.LocalizedObjName(Resource.Parse(field, vars, false, true), vars, form, true);
		}
		if (a == "rel_mod")
		{
			int num = field.Int(0, null, 0);
			int num2 = field.Int(1, null, 0);
			return global::Defs.LocalizedNumber(num + num2, vars, form);
		}
		if (!string.IsNullOrEmpty(form))
		{
			DT.Field field2 = field.FindChild(form, null, true, true, true, '.');
			if (field2 != null)
			{
				field = field2;
				form = "";
			}
			else if (global::Defs.allow_wildcard_form)
			{
				field2 = field.FindChild("*", null, true, true, true, '.');
				if (field2 != null)
				{
					Vars vars2 = new Vars(vars);
					vars2.Set<string>("form", form);
					field = field2;
					form = "";
					vars = vars2;
				}
			}
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Value value = field.Value(vars, true, false);
		DT.Field field3 = value.obj_val as DT.Field;
		Value value2 = (field3 != null) ? value : field.RandomValue(vars, true, true);
		Vars.PopReflectionMode(old_mode);
		if (!value2.is_valid)
		{
			return null;
		}
		string text = null;
		if (value2.is_number)
		{
			flag = false;
			text = global::Defs.LocalizedNumber(value2, vars, form);
		}
		else if (value2.type == Value.Type.String)
		{
			text = DT.String(value2, null);
		}
		else
		{
			if (field3 != null && field3 != field)
			{
				return global::Defs.Localize(field3, vars, form, true, true);
			}
			if (value2.type == Value.Type.Object)
			{
				return global::Defs.LocalizedObjName(value2.obj_val, vars, form, false);
			}
		}
		if (text == null)
		{
			return null;
		}
		if (flag)
		{
			Debug.LogWarning("Localizing non-text field: " + field.Path(true, false, '.'));
		}
		return global::Defs.GetLocalized(text, vars);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0002ED9E File Offset: 0x0002CF9E
	public static string GetLocalized(string val, IVars vars)
	{
		if (val == null)
		{
			return null;
		}
		val = global::Defs.ReplaceVars(val, vars, true, '\0');
		if (val == null)
		{
			return null;
		}
		val = global::Defs.ReplaceFormatting(val);
		return val;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x0002EDC0 File Offset: 0x0002CFC0
	public static string LocalizeNull(string key, string form, bool no_null)
	{
		if (!no_null)
		{
			return null;
		}
		string str = "{" + key;
		if (form != null && form != "")
		{
			str = str + ":" + form;
		}
		return str + "}";
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0002EE08 File Offset: 0x0002D008
	public static DT.Field FindTextField(string text_key)
	{
		global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, false, true);
		if (textInfo == null)
		{
			return null;
		}
		DT.Field textField = textInfo.GetTextField(null, false);
		if (textField == null)
		{
			return null;
		}
		if (textField != textInfo.def_field)
		{
			return textField;
		}
		if (textField.Type() == "text")
		{
			return textField;
		}
		return null;
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0002EE51 File Offset: 0x0002D051
	public static DT.Field FindTextField(DT.Field field, string key)
	{
		if (global::Defs.Get(false) == null)
		{
			return null;
		}
		return global::Defs.FindTextField(field.Path(false, false, '.') + "." + key);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0002EE80 File Offset: 0x0002D080
	private static List<global::Defs.UnicodeRange> GetUnicodeRanges(string lang_key)
	{
		if (lang_key == "ru")
		{
			return global::Defs.cyrillic_unicode_ranges;
		}
		if (lang_key == "ko")
		{
			return global::Defs.korean_unicode_ranges;
		}
		if (lang_key == "ja" || lang_key == "zh")
		{
			return global::Defs.CJK_unicode_ranges;
		}
		if (lang_key == "zh_tw")
		{
			return global::Defs.traditional_chinese_unicode_ranges;
		}
		if (!(lang_key == "hi"))
		{
			return global::Defs.latin_unicode_ranges;
		}
		return global::Defs.hindi_unicode_ranges;
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0002EF00 File Offset: 0x0002D100
	public static string FakeTranslate(string key, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (global::Defs.fake_translation_to == null)
		{
			return text;
		}
		global::Defs.<>c__DisplayClass112_0 CS$<>8__locals1;
		CS$<>8__locals1.ranges = global::Defs.GetUnicodeRanges(global::Defs.fake_translation_to);
		CS$<>8__locals1.total_chars = 0;
		for (int i = 0; i < CS$<>8__locals1.ranges.Count; i++)
		{
			global::Defs.UnicodeRange unicodeRange = CS$<>8__locals1.ranges[i];
			CS$<>8__locals1.total_chars += unicodeRange.length;
		}
		int hashCode = key.GetHashCode();
		CS$<>8__locals1.rnd = new Random(hashCode);
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		bool flag = false;
		foreach (char c in text)
		{
			if (c == '{' || c == '<')
			{
				flag = true;
			}
			else if (c == '}' || c == '>')
			{
				flag = false;
			}
			else if (!flag && char.IsLetter(c))
			{
				c = global::Defs.<FakeTranslate>g__ChooseRandomChar|112_0(ref CS$<>8__locals1);
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0002EFFC File Offset: 0x0002D1FC
	public static string LocalizeTextInfo(string text_key, global::Defs.TextInfo ti, IVars vars = null, string form = null, bool no_null = true)
	{
		if (ti == null)
		{
			return global::Defs.LocalizeNull(text_key, form, no_null);
		}
		if (ti.def_field != null && ti.def_field.Type() == "def")
		{
			return global::Defs.LocalizedDefLink(ti.def_field, vars, form, no_null);
		}
		DT.Field textField = ti.GetTextField(vars, false);
		if (textField == null)
		{
			return global::Defs.LocalizeNull(text_key, form, no_null);
		}
		string text = global::Defs.GetLocalized(textField, form, vars);
		text = global::Defs.FakeTranslate(text_key, text);
		if (text != null)
		{
			return text;
		}
		return global::Defs.LocalizeNull(text_key, form, no_null);
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0002F07C File Offset: 0x0002D27C
	public static string Localize(string text_key, IVars vars = null, string form = null, bool no_null = true, bool profile = true)
	{
		if (text_key == null)
		{
			return null;
		}
		if (text_key == "")
		{
			return "";
		}
		if (profile)
		{
			Profile.BeginSection("Defs.Localize key");
		}
		if (text_key[0] == '#')
		{
			string result = text_key.Substring(1);
			if (profile)
			{
				Profile.EndSection("Defs.Localize key");
			}
			return result;
		}
		if (text_key[0] != '@')
		{
			global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text_key, false, true);
			string result2 = global::Defs.LocalizeTextInfo(text_key, textInfo, vars, form, no_null);
			if (profile)
			{
				Profile.EndSection("Defs.Localize key");
			}
			return result2;
		}
		string text = text_key.Substring(1);
		float val;
		if (DT.ParseFloat(text, out val))
		{
			string result3 = global::Defs.LocalizedNumber(val, vars, form);
			if (profile)
			{
				Profile.EndSection("Defs.Localize key");
			}
			return result3;
		}
		string localized = global::Defs.GetLocalized(text, vars);
		if (localized != null)
		{
			if (profile)
			{
				Profile.EndSection("Defs.Localize key");
			}
			return localized;
		}
		string result4 = global::Defs.LocalizeNull(text_key, form, no_null);
		if (profile)
		{
			Profile.EndSection("Defs.Localize key");
		}
		return result4;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0002F15C File Offset: 0x0002D35C
	public static string Localize(DT.Field field, IVars vars = null, string form = null, bool no_null = true, bool profile = true)
	{
		if (field != null)
		{
			if (profile)
			{
				Profile.BeginSection("Defs.Localize field");
			}
			string text_key = field.Path(false, false, '.');
			DT.Field field2 = global::Defs.currently_localizing;
			global::Defs.currently_localizing = field;
			string result = global::Defs.Localize(text_key, vars, form, no_null, false);
			global::Defs.currently_localizing = field2;
			if (profile)
			{
				Profile.EndSection("Defs.Localize field");
			}
			return result;
		}
		if (!no_null)
		{
			return null;
		}
		return "";
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0002F1B8 File Offset: 0x0002D3B8
	public static string Localize(DT.Field field, string key, IVars vars = null, string form = null, bool no_null = true, bool profile = true)
	{
		if (field != null)
		{
			if (profile)
			{
				Profile.BeginSection("Defs.Localize child field");
			}
			DT.Field field2 = field.FindChild(key, vars, true, true, true, '.');
			if (field2 != null)
			{
				string text = global::Defs.Localize(field2, vars, form, false, profile);
				if (text != null)
				{
					if (profile)
					{
						Profile.EndSection("Defs.Localize child field");
					}
					return text;
				}
			}
			string text_key = field.Path(false, false, '.') + "." + key;
			DT.Field field3 = global::Defs.currently_localizing;
			global::Defs.currently_localizing = field;
			string result = global::Defs.Localize(text_key, vars, form, no_null, false);
			global::Defs.currently_localizing = field3;
			if (profile)
			{
				Profile.EndSection("Defs.Localize child field");
			}
			return result;
		}
		if (!no_null)
		{
			return null;
		}
		return "";
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0002F254 File Offset: 0x0002D454
	public static void HandleListsLocalization(DT.Field f, Vars vars, params string[] keys)
	{
		foreach (string path in keys)
		{
			DT.Field field = f.FindChild(path, vars, true, true, true, '.');
			if (field != null && !field.Value(null, false, false).is_unknown)
			{
				global::Defs.HandleListsLocalization(field, vars);
			}
		}
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0002F2A0 File Offset: 0x0002D4A0
	public static void HandleListsLocalization(DT.Field f, Vars vars)
	{
		if (f == null || f.value == Value.Null || !f.key.StartsWith("list", StringComparison.Ordinal))
		{
			return;
		}
		string key = f.String(0, null, "");
		string text = global::Defs.LocalizeList(f, vars, "", false);
		if (!string.IsNullOrEmpty(text))
		{
			vars.Set<string>(key, "#" + text);
			for (int i = 0; i < f.parent.children.Count; i++)
			{
				DT.Field field = f.parent.children[i];
				if (field.key.StartsWith("list", StringComparison.Ordinal) && field.key != "list")
				{
					global::Defs.HandleListsLocalization(field, vars);
				}
			}
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x0002F368 File Offset: 0x0002D568
	public static string LocalizeList(IList lst, IVars vars, string form, string separator = null, string finalSeparator = null, bool start_hash = false)
	{
		if (lst == null || lst.Count == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (start_hash)
		{
			stringBuilder.Append("#");
		}
		Vars vars2 = new Vars(vars);
		if (separator != null)
		{
			vars2.Set<string>("list_separator", "#" + separator);
		}
		for (int i = 0; i < lst.Count; i++)
		{
			if (lst.Count > 1 && i == lst.Count - 2)
			{
				if (finalSeparator != null)
				{
					vars2.Set<string>("list_separator", "#" + finalSeparator);
				}
				else
				{
					vars2.Set<string>("list_separator", "@{list_final_separator}");
				}
			}
			if (i == lst.Count - 1)
			{
				vars2.Set<string>("list_separator", "#");
			}
			object val = lst[i];
			Value val2 = new Value(val);
			vars2.Set<Value>("item", val2);
			string value;
			if (!string.IsNullOrEmpty(form))
			{
				value = global::Defs.Localize(string.Concat(new string[]
				{
					"@{item:",
					form,
					"}{list_separator:",
					form,
					"}"
				}), vars2, null, true, false);
			}
			else
			{
				value = global::Defs.Localize("@{item}{list_separator}", vars2, null, true, false);
			}
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0002F4A8 File Offset: 0x0002D6A8
	public static string LocalizeList(DT.Field f, IVars vars, string form, bool start_hash = false)
	{
		string key = f.String(0, null, "");
		string separator = global::Defs.Localize(f, "separator", vars, null, false, false);
		string finalSeparator = global::Defs.Localize(f, "final_separator", vars, null, false, false);
		return global::Defs.LocalizeList(vars.GetVar(key, null, true).obj_val as IList, vars, form, separator, finalSeparator, start_hash);
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0002F500 File Offset: 0x0002D700
	public static bool InTitle()
	{
		return SceneManager.GetActiveScene().name == "title";
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0002F524 File Offset: 0x0002D724
	private void Awake()
	{
		Profile.Init();
		if (Application.isPlaying)
		{
			if (global::Defs.instance != null)
			{
				if (global::Defs.instance != this)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
			global::Defs.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			GameLogic.Get(true);
			this.PreloadTexts(null);
			if (!global::Defs.InTitle())
			{
				this.Load();
			}
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0002F58F File Offset: 0x0002D78F
	public static void fixnull(ref Value value)
	{
		if (value.obj_val == null)
		{
			value.obj_val = null;
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0002F5A0 File Offset: 0x0002D7A0
	private void PostProcess(DT.Field field)
	{
		this.ResolveValue(field);
		if (field.children == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			this.PostProcess(field.children[i]);
		}
	}

	// Token: 0x060003DB RID: 987 RVA: 0x0002F5E8 File Offset: 0x0002D7E8
	public void PostProcess()
	{
		if (this.dt == null)
		{
			return;
		}
		using (Game.ProfileScope profileScope = Game.Profile("Defs.PostProcess", false, 0f, null))
		{
			Profile.BeginSection("Log def errors");
			for (int i = 0; i < this.dt.errors.Count; i++)
			{
				Debug.LogError(this.dt.errors[i]);
			}
			this.dt.errors.Clear();
			Profile.EndSection("Log def errors");
			if (Assets.available)
			{
				Profile.BeginSection("Post process fields");
				for (int j = 0; j < this.dt.files.Count; j++)
				{
					try
					{
						this.PostProcess(this.dt.files[j]);
					}
					catch
					{
					}
				}
				Profile.EndSection("Post process fields");
				Profile.BeginSection("OnDefsProcessed");
				this.OnDefsProcessed();
				Profile.EndSection("OnDefsProcessed");
				Profile.BeginSection("Analytics.OnDefsLoaded");
				try
				{
					Analytics.OnDefsLoaded();
				}
				catch
				{
				}
				Profile.EndSection("Analytics.OnDefsLoaded");
				Profile.BeginSection("Tutorial.LoadDefs");
				try
				{
					Tutorial.LoadDefs();
				}
				catch
				{
				}
				Profile.EndSection("Tutorial.LoadDefs");
				Profile.BeginSection("Voices.LoadDefs");
				try
				{
					Voices.LoadDefs();
				}
				catch (Exception arg)
				{
					Game.Log(string.Format("Failed to load defs: {0}", arg), Game.LogType.Error);
				}
				Profile.EndSection("Voices.LoadDefs");
				if (!global::Defs.silent_reload)
				{
					Game.Log(string.Format("Defs postprocessed for {0:F3}sec", profileScope.Millis / 1000f), Game.LogType.Message);
				}
			}
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0002F7F8 File Offset: 0x0002D9F8
	private void LoadTextsDir(DirectoryInfo di, string path)
	{
		if (!di.Exists)
		{
			Debug.LogWarning("Texts directory not found: " + path);
			return;
		}
		FileInfo[] files = di.GetFiles();
		Array.Sort<FileInfo>(files, new Comparison<FileInfo>(DT.CompareFileNames));
		foreach (FileInfo fileInfo in files)
		{
			string path2 = path + fileInfo.Name.ToLowerInvariant();
			this.LoadTextFile(path2);
		}
		DirectoryInfo[] directories = di.GetDirectories();
		Array.Sort<DirectoryInfo>(directories, new Comparison<DirectoryInfo>(DT.CompareFileNames));
		foreach (DirectoryInfo directoryInfo in directories)
		{
			string path3 = path + directoryInfo.Name.ToLowerInvariant() + "/";
			this.LoadTextsDir(directoryInfo, path3);
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0002F8B8 File Offset: 0x0002DAB8
	private void LoadTextsLanguageDir(string lang_key)
	{
		global::Defs.loading_language = lang_key;
		string path = global::Defs.texts_path + lang_key + "/";
		DirectoryInfo di = new DirectoryInfo(path);
		this.LoadTextsDir(di, path);
		this.LoadDeletedTexts(path);
		global::Defs.loading_language = null;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0002F8F8 File Offset: 0x0002DAF8
	private void LoadDeletedTexts(string path)
	{
		string[] array;
		try
		{
			array = File.ReadAllLines(path + "deleted.txt", Encoding.UTF8);
		}
		catch
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(array[i], false, true);
			if (textInfo != null)
			{
				int idx;
				global::Defs.TextInfo.LangInfo languageInfo = textInfo.GetLanguageInfo(global::Defs.loading_language, out idx, false);
				if (languageInfo.localized_field != null)
				{
					languageInfo.localized_field = null;
					textInfo.SetLanguageInfo(languageInfo, idx);
				}
			}
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0002F974 File Offset: 0x0002DB74
	public static bool IsDevLanguage(string language)
	{
		return language == "dev" || language == "tts";
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x0002F990 File Offset: 0x0002DB90
	public static bool IsDevLanguage()
	{
		return global::Defs.IsDevLanguage(global::Defs.Language);
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0002F99C File Offset: 0x0002DB9C
	public static bool IsPlaying()
	{
		return MainThreadUpdates.main_thread != null && (!MainThreadUpdates.IsMainThread() || Application.isPlaying);
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x0002F9B8 File Offset: 0x0002DBB8
	private void PreloadTexts(string language = null)
	{
		if (language == null)
		{
			language = global::Defs.Language;
		}
		string text = language;
		if (text == "tts")
		{
			text = "dev";
		}
		string text2 = global::Defs.IsDevLanguage(language) ? "" : (language + "_");
		this.LoadTextFile(string.Concat(new string[]
		{
			global::Defs.texts_path,
			text,
			"/",
			text2,
			"preload.csv"
		}));
		if (!global::Defs.IsDevLanguage())
		{
			int languageVersion = global::Defs.GetLanguageVersion(language);
			for (int i = 2; i <= languageVersion; i++)
			{
				this.LoadTextFile(string.Format("{0}{1}/_{2};{3}/{4}preload;{5}.csv", new object[]
				{
					global::Defs.texts_path,
					text,
					language,
					i,
					text2,
					i
				}));
			}
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x0002FA88 File Offset: 0x0002DC88
	public void LoadTexts()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		bool flag = global::Defs.Language == "ALL";
		LocalizationErrors.ClearAll();
		this.texts.Clear();
		this.wiki.Clear();
		if (global::Defs.colorize_localized_texts == 1)
		{
			global::Defs.colorize_localized_texts = 0;
		}
		if (global::Defs.enable_fallback_texts == 1 || global::Defs.enable_fallback_texts == 2)
		{
			global::Defs.enable_fallback_texts = 0;
		}
		if (global::Defs.enable_proofread_texts_in_dev == 1 || global::Defs.enable_proofread_texts_in_dev == 2)
		{
			global::Defs.enable_proofread_texts_in_dev = 0;
		}
		long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		this.LoadLanguagesDef(!global::Defs.IsPlaying());
		long t = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		global::Defs.loading_language = "all";
		this.LoadTextFile(global::Defs.texts_path + "formatting.def");
		long t2 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		this.LoadTextsFromDefs(flag);
		long t3 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		long t4 = -1L;
		if (!flag && !global::Defs.IsDevLanguage() && global::Defs.enable_fallback_texts > 0)
		{
			elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			this.LoadTextsLanguageDir("dev");
			t4 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		}
		long t5 = -1L;
		if (!flag && global::Defs.Language != "en" && global::Defs.enable_proofread_texts_in_dev > 0 && (global::Defs.IsDevLanguage() || global::Defs.enable_fallback_texts > 0))
		{
			elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			this.LoadTextsLanguageDir("en");
			t5 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		}
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		if (flag)
		{
			for (int i = 0; i < this.languages.Count; i++)
			{
				DT.Field field = this.languages[i];
				if (!(field.key == "tts"))
				{
					this.LoadTextsLanguageDir(field.key);
				}
			}
		}
		else
		{
			string text = global::Defs.Language;
			if (text == "tts")
			{
				text = "dev";
			}
			this.LoadTextsLanguageDir(text);
		}
		long t6 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		global::Defs.loading_language = null;
		this.LoadLanguagesCSV(flag);
		long t7 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		this.LoadLanguageCache(flag);
		long t8 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		long t9 = -1L;
		if (flag)
		{
			elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			this.LoadResolvedConflicts();
			t9 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		}
		elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
		this.wiki.OnTextsLoaded();
		long t10 = stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
		if (!global::Defs.silent_reload)
		{
			elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			global::Defs.<>c__DisplayClass135_0 CS$<>8__locals1;
			CS$<>8__locals1.log = string.Format("{0} texts loaded in {1} ms", this.texts.Count, elapsedMilliseconds);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t, "laguages.def", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t2, "formatting.def", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t3, "def texts", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t4, "dev texts", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t5, "en texts", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t6, "texts dir", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t7, "laguages.csv", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t8, "cache", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t9, "resolved conflicts", ref CS$<>8__locals1);
			global::Defs.<LoadTexts>g__AddTiming|135_0(t10, "wiki", ref CS$<>8__locals1);
			Debug.Log(CS$<>8__locals1.log);
			LocalizationErrors.LogStats(false);
			LocalizationErrors.SaveAll();
		}
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x0002FDB4 File Offset: 0x0002DFB4
	private void LoadTextsFromDefs(bool forced)
	{
		if (!forced && !global::Defs.IsDevLanguage() && global::Defs.enable_fallback_texts <= 0)
		{
			return;
		}
		DT dt = this.dt;
		if (((dt != null) ? dt.files : null) == null)
		{
			return;
		}
		for (int i = 0; i < this.dt.files.Count; i++)
		{
			DT.Field f = this.dt.files[i];
			this.LoadTextsFromDefs(f);
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x0002FE20 File Offset: 0x0002E020
	private void LoadTextsFromDefs(DT.Field f)
	{
		if (f.Type() == "text")
		{
			this.ColorizeLocalizedText(f, false);
			global::Defs.GetTextInfo(f.Path(false, false, '.'), true, false).def_field = f;
			return;
		}
		if (f.children == null)
		{
			return;
		}
		for (int i = 0; i < f.children.Count; i++)
		{
			DT.Field f2 = f.children[i];
			this.LoadTextsFromDefs(f2);
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x0002FE94 File Offset: 0x0002E094
	public void ResolveTextFields()
	{
		Profile.BeginSection("Defs.ResolveTextFields");
		foreach (KeyValuePair<string, global::Defs.TextInfo> keyValuePair in this.texts)
		{
			keyValuePair.Value.GetTextField(null, true);
		}
		Profile.EndSection("Defs.ResolveTextFields");
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x0002FF04 File Offset: 0x0002E104
	public void LoadLanguagesDef(bool force_reload = false)
	{
		if (global::Defs.set_language != null && !force_reload)
		{
			return;
		}
		this.languages.Clear();
		global::Defs.def_language = "en";
		global::Defs.set_language = "";
		global::Defs.Language = null;
		Voices.Language = null;
		global::Defs.default_language_version = 0;
		global::Defs.fake_translation_to = null;
		try
		{
			DT.Field field = DT.Parser.LoadFieldFromFile(global::Defs.texts_path + "/languages.def", "Language");
			if (field != null)
			{
				DT.ResolveValue(field, true);
				global::Defs.set_language = field.String(null, "");
				if (!string.IsNullOrEmpty(global::Defs.set_language))
				{
					global::Defs.def_language = global::Defs.set_language;
				}
				if (field.children != null)
				{
					for (int i = 0; i < field.children.Count; i++)
					{
						DT.Field field2 = field.children[i];
						if (!string.IsNullOrEmpty(field2.key))
						{
							if (field2.key == "version")
							{
								global::Defs.default_language_version = field2.Int(null, 0);
							}
							else if (field2.key == "fake_translation")
							{
								global::Defs.fake_translation_to = field2.String(null, "");
							}
							else
							{
								this.languages.Add(field2);
							}
						}
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x0003004C File Offset: 0x0002E24C
	private void BuildLanguagesCSVRemap(bool load_all)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("id", "*");
		dictionary.Add("ID", "*");
		if (!load_all)
		{
			for (int i = 0; i < this.languages.Count; i++)
			{
				DT.Field field = this.languages[i];
				if (!(field.key == global::Defs.Language) && (!(field.key == "en") || global::Defs.enable_proofread_texts_in_dev <= 0))
				{
					dictionary.Add(field.key, null);
				}
			}
		}
		Table.remap_cells = dictionary;
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000300E4 File Offset: 0x0002E2E4
	public void OnDefsLoaded(DT dt)
	{
		this.dt = dt;
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x000300ED File Offset: 0x0002E2ED
	public void OnDTProcessed()
	{
		Profile.BeginSection("Defs.LoadTexts");
		this.LoadTexts();
		Profile.EndSection("Defs.LoadTexts");
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0003010C File Offset: 0x0002E30C
	public void Load(string defs_map, bool isPlaying, bool profile = true)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		this.num_reloads++;
		GameLogic.Get(true).LoadDefs(defs_map, profile);
		stopwatch.Stop();
		float num = (float)stopwatch.ElapsedMilliseconds / 1000f;
		Debug.Log(string.Concat(new object[]
		{
			"Defs loaded for ",
			num,
			"sec, ustrs: ",
			this.dt.unique_strings.ToString()
		}));
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00030188 File Offset: 0x0002E388
	public void Load()
	{
		string text;
		if (global::Defs.Language == "ALL")
		{
			text = null;
		}
		else
		{
			text = SceneManager.GetActiveScene().name.ToLowerInvariant();
			if (text == "title")
			{
				text = null;
			}
		}
		this.Load(text, Application.isPlaying, true);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x000301DC File Offset: 0x0002E3DC
	private void ParseTextKey(DT.Field field, out string key, out string form, out int variant)
	{
		key = field.key;
		form = null;
		variant = 0;
		int num = key.LastIndexOf('#');
		if (num >= 0)
		{
			int.TryParse(key.Substring(num + 1), out variant);
			if (variant <= 1)
			{
				Debug.LogError(field.Path(true, false, '.') + ": Invalid text variant index");
			}
			key = key.Substring(0, num);
		}
		int num2 = key.LastIndexOf(':');
		if (num2 >= 0)
		{
			form = key.Substring(num2 + 1);
			if (form == "")
			{
				Debug.LogError(field.Path(true, false, '.') + ": Invalid text form");
			}
			key = key.Substring(0, num2);
		}
		if (key.IndexOf('#') >= 0)
		{
			Debug.LogError(field.Path(true, false, '.') + ": Multiple '#'");
		}
		if (key.IndexOf(':') >= 0)
		{
			Debug.LogError(field.Path(true, false, '.') + ": Multiple ':'");
		}
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x000302D8 File Offset: 0x0002E4D8
	public static int ParseTextVersion(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			return 0;
		}
		if (filename == "languages.csv")
		{
			return 0;
		}
		int i = filename.IndexOf(';');
		if (i < 0)
		{
			return 1;
		}
		int num = 0;
		for (i++; i < filename.Length; i++)
		{
			char c = filename[i];
			if (!char.IsDigit(c))
			{
				break;
			}
			num = 10 * num + (int)c - 48;
		}
		return num;
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00030340 File Offset: 0x0002E540
	private static bool IsOverride(DT.Field org_field, DT.Field field)
	{
		string org_filename = (org_field != null) ? org_field.FileName() : null;
		string filename = (field != null) ? field.FileName() : null;
		return global::Defs.IsOverride(org_filename, filename);
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0003036C File Offset: 0x0002E56C
	private static bool IsOverride(string org_filename, string filename)
	{
		int num = global::Defs.ParseTextVersion(org_filename);
		return global::Defs.ParseTextVersion(filename) > num;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0003038C File Offset: 0x0002E58C
	private string ExtractLanguageFromField(DT.Field field)
	{
		if (field == null)
		{
			return null;
		}
		if (global::Defs.FindLanguage(field.key) != null)
		{
			return field.key;
		}
		string text = field.FilePath();
		if (text.StartsWith(global::Defs.texts_path, StringComparison.OrdinalIgnoreCase))
		{
			int num = text.IndexOf('/', global::Defs.texts_path.Length);
			if (num > global::Defs.texts_path.Length)
			{
				string text2 = text.Substring(global::Defs.texts_path.Length, num - global::Defs.texts_path.Length);
				if (global::Defs.FindLanguage(text2) != null)
				{
					return text2;
				}
			}
		}
		return null;
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00030410 File Offset: 0x0002E610
	private bool CheckDuplicate(DT.Field org_field, DT.Field field)
	{
		string text;
		if ((text = org_field.String(null, null)) == null)
		{
			text = (org_field.value_str ?? "");
		}
		string text2 = text;
		string text3;
		if ((text3 = field.String(null, null)) == null)
		{
			text3 = (field.value_str ?? "");
		}
		string text4 = text3;
		if (text2 == text4)
		{
			return false;
		}
		string filename = (field != null) ? field.FileName() : null;
		if (global::Defs.IsOverride((org_field != null) ? org_field.FileName() : null, filename))
		{
			return true;
		}
		string text5 = this.ExtractLanguageFromField(org_field) ?? this.ExtractLanguageFromField(field);
		LocalizationErrors.Log(text5, "\n//[" + text5 + "] " + org_field.Path(false, false, '.'), true, false);
		LocalizationErrors.Log(text5, "#ERROR: \"Replacing text value\"", false, false);
		LocalizationErrors.Log(text5, "//**** " + org_field.Path(true, false, '.'), false, false);
		LocalizationErrors.Log(text5, text2, false, false);
		LocalizationErrors.Log(text5, "//**** " + field.Path(true, false, '.'), false, false);
		LocalizationErrors.Log(text5, text4, false, false);
		return true;
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00030510 File Offset: 0x0002E710
	private void ReplaceValue(DT.Field org_field, DT.Field field)
	{
		if (!this.CheckDuplicate(org_field, field))
		{
			return;
		}
		if (org_field.value.obj_val is List<DT.SubValue>)
		{
			global::Defs.AddVariant(org_field, field, 1);
			return;
		}
		org_field.value_str = field.value_str;
		org_field.value = field.value;
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00030550 File Offset: 0x0002E750
	private static DT.SubValue ToSubValue(DT.Field field)
	{
		if (!field.value.is_string && (!field.value.is_unknown || field.value_str != ""))
		{
			Debug.LogError(string.Format("{0}: Adding non-string variant: '{1}' -> {2}", field.Path(true, false, '.'), field.value_str, field.value));
		}
		return new DT.SubValue
		{
			value_str = field.value_str,
			value = field.value
		};
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x000305D0 File Offset: 0x0002E7D0
	public static void AddVariant(DT.Field org_field, DT.Field field, int variant)
	{
		if (org_field == null)
		{
			Debug.LogError(field.Path(true, false, '.') + ": Trying to add variant to non-existing text");
			return;
		}
		if (variant <= 0)
		{
			Debug.LogError(string.Format("{0}: Trying to add variant {1} (must be > 0)", field.Path(true, false, '.'), variant));
			return;
		}
		List<DT.SubValue> list = org_field.value.obj_val as List<DT.SubValue>;
		if (list == null)
		{
			list = new List<DT.SubValue>();
			list.Add(global::Defs.ToSubValue(org_field));
			org_field.value = new Value(list);
		}
		DT.SubValue subValue = global::Defs.ToSubValue(field);
		if (variant != list.Count + 1)
		{
			if (global::Defs.IsOverride(org_field, field) && variant <= list.Count)
			{
				list[variant - 1] = subValue;
				return;
			}
			Debug.LogWarning(string.Format("{0}: Trying to add variant {1} to a text with {2} variants", field.Path(true, false, '.'), variant, list.Count));
		}
		list.Add(subValue);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x000306B0 File Offset: 0x0002E8B0
	private static bool FindMarkup(string text, int ofs, out int start_idx, out int end_idx, bool in_wiki)
	{
		for (;;)
		{
			start_idx = text.IndexOfAny(in_wiki ? global::Defs.wiki_markup_start_chars : global::Defs.markup_start_chars, ofs);
			end_idx = start_idx;
			if (start_idx < 0)
			{
				break;
			}
			char c = text[start_idx];
			if (c == '{')
			{
				end_idx = text.IndexOf('}', start_idx + 1);
				if (end_idx < 0)
				{
					return false;
				}
			}
			if (c == '<')
			{
				end_idx = text.IndexOf('>', start_idx + 1);
				if (end_idx < 0)
				{
					return false;
				}
			}
			if (c == '/')
			{
				end_idx = start_idx + 1;
				if (end_idx >= text.Length)
				{
					return false;
				}
				if (text[end_idx] == '/')
				{
					goto IL_8F;
				}
				ofs = start_idx + 1;
			}
			else
			{
				if (c == '\r' || c == '\n')
				{
					goto IL_B7;
				}
				if (c != '#')
				{
					goto IL_17D;
				}
				if (global::Defs.IsAtLineStart(text, start_idx))
				{
					goto IL_101;
				}
				ofs = start_idx + 1;
			}
		}
		return false;
		IL_8F:
		end_idx = text.IndexOfAny(global::Defs.eol_chars, start_idx);
		if (end_idx < 0)
		{
			end_idx = text.Length;
		}
		return true;
		IL_B7:
		for (end_idx++; end_idx < text.Length; end_idx++)
		{
			char c = text[end_idx];
			if (c != '\r' && c != '\n')
			{
				break;
			}
		}
		return true;
		IL_101:
		if (global::Defs.Match(text, start_idx, "#caption", ref end_idx))
		{
			return true;
		}
		if (global::Defs.Match(text, start_idx, "#category", ref end_idx))
		{
			end_idx = text.IndexOfAny(global::Defs.category_end_chars, end_idx);
			if (end_idx < 0)
			{
				end_idx = text.Length;
			}
			else if (end_idx < text.Length && text[end_idx] == ':')
			{
				end_idx++;
			}
			return true;
		}
		end_idx = text.IndexOfAny(global::Defs.eol_chars, start_idx);
		if (end_idx < 0)
		{
			end_idx = text.Length;
		}
		return true;
		IL_17D:
		end_idx++;
		return true;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00030844 File Offset: 0x0002EA44
	private static bool IsAtLineStart(string text, int idx)
	{
		for (idx--; idx >= 0; idx--)
		{
			char c = text[idx];
			if (c != ' ' && c != '\t')
			{
				return c == '\r' || c == '\n';
			}
		}
		return true;
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00030884 File Offset: 0x0002EA84
	private static bool Match(string text, int idx, string substring, ref int end_idx)
	{
		int length = substring.Length;
		if (idx + length > text.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			char c = text[idx + i];
			char c2 = substring[i];
			if (c != c2)
			{
				return false;
			}
		}
		end_idx = idx + length;
		if (end_idx >= text.Length)
		{
			return true;
		}
		char c3 = text[end_idx];
		return !char.IsLetter(c3) && !char.IsDigit(c3) && c3 != '_';
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x000308FC File Offset: 0x0002EAFC
	public static int CountPlainTextWords(string text, int start_idx, int end_idx)
	{
		int num = 0;
		while (start_idx < end_idx)
		{
			while (start_idx < end_idx && !char.IsLetter(text[start_idx]))
			{
				start_idx++;
			}
			if (start_idx < end_idx)
			{
				num++;
			}
			while (start_idx < end_idx && char.IsLetter(text[start_idx]))
			{
				start_idx++;
			}
		}
		return num;
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0003094C File Offset: 0x0002EB4C
	private static void CountWords(string text, global::Defs.WordStats stats, bool in_wiki)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int num = 0;
		int length = text.Length;
		int num2;
		int num3;
		while (num < length && global::Defs.FindMarkup(text, num, out num2, out num3, in_wiki))
		{
			if (num2 > num)
			{
				int num4 = global::Defs.CountPlainTextWords(text, num, num2);
				stats.num_words += num4;
				if (in_wiki)
				{
					stats.num_wiki_words += num4;
				}
			}
			char c = text[num2];
			if (c <= '/')
			{
				if (c != '#')
				{
					if (c != '/')
					{
						goto IL_C0;
					}
					stats.num_wiki_comments++;
				}
				else
				{
					stats.num_wiki_tags++;
				}
			}
			else if (c != '<')
			{
				if (c != '{')
				{
					goto IL_C0;
				}
				stats.num_vars++;
			}
			else
			{
				stats.num_tags++;
			}
			IL_CE:
			num = num3;
			continue;
			IL_C0:
			stats.num_symbols++;
			goto IL_CE;
		}
		if (num < length)
		{
			int num5 = global::Defs.CountPlainTextWords(text, num, length);
			stats.num_words += num5;
			if (in_wiki)
			{
				stats.num_wiki_words += num5;
			}
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00030A60 File Offset: 0x0002EC60
	private static void CountWords(DT.Field field, global::Defs.WordStats stats, bool in_wiki)
	{
		int num = field.NumValues();
		for (int i = 0; i < num; i++)
		{
			global::Defs.CountWords(field.String(i, null, ""), stats, in_wiki);
		}
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00030A94 File Offset: 0x0002EC94
	private static void AddWordStats(DT.Field field, global::Defs.WordStats stats, bool in_wiki)
	{
		if (string.IsNullOrEmpty(field.key))
		{
			return;
		}
		stats.num_fields++;
		stats.num_value_variants += field.NumValues();
		global::Defs.CountWords(field, stats, in_wiki);
		if (field.children == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field2 = field.children[i];
			if (!string.IsNullOrEmpty(field2.key))
			{
				stats.num_forms++;
				stats.num_form_variants += field2.NumValues();
				global::Defs.CountWords(field2, stats, in_wiki);
			}
		}
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00030B3C File Offset: 0x0002ED3C
	private static string ExtractAndColorizeText(string text, int start_idx, int end_idx, string open_tag, string close_tag)
	{
		int i;
		for (i = start_idx; i < end_idx; i++)
		{
			char c = text[i];
			if (c != '\r' && c != '\n' && c != ' ' && c != '\t')
			{
				break;
			}
		}
		int j;
		for (j = end_idx; j > i; j--)
		{
			char c2 = text[j - 1];
			if (c2 != '\r' && c2 != '\n' && c2 != ' ' && c2 != '\t')
			{
				break;
			}
		}
		string text2 = "";
		if (i > start_idx)
		{
			text2 += text.Substring(start_idx, i - start_idx);
		}
		if (j > i)
		{
			text2 = text2 + open_tag + text.Substring(i, j - i) + close_tag;
		}
		if (end_idx > j)
		{
			text2 += text.Substring(j, end_idx - j);
		}
		return text2;
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00030BEC File Offset: 0x0002EDEC
	public static string ColorizeLocalizedText(string text, string open_tag, string close_tag, bool in_wiki)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		string text2 = "";
		int num = 0;
		int length = text.Length;
		int num2;
		int num3;
		while (num < length && global::Defs.FindMarkup(text, num, out num2, out num3, in_wiki))
		{
			if (num2 > num)
			{
				text2 += global::Defs.ExtractAndColorizeText(text, num, num2, open_tag, close_tag);
			}
			text2 += text.Substring(num2, num3 - num2);
			num = num3;
		}
		if (num < length)
		{
			text2 += global::Defs.ExtractAndColorizeText(text, num, length, open_tag, close_tag);
		}
		return text2;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00030C68 File Offset: 0x0002EE68
	private static void ColorizeLocalizedText(ref Value val, bool in_wiki)
	{
		if (val.is_string)
		{
			string text = val.String(null);
			val.obj_val = global::Defs.ColorizeLocalizedText(text, "{L}", "{/L}", in_wiki);
			return;
		}
		List<DT.SubValue> list = val.obj_val as List<DT.SubValue>;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			global::Defs.ColorizeLocalizedText(ref list[i].value, in_wiki);
		}
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00030CD0 File Offset: 0x0002EED0
	public void ColorizeLocalizedText(DT.Field field, bool in_wiki)
	{
		if (global::Defs.colorize_localized_texts <= 0)
		{
			return;
		}
		string key = field.key;
		if (key == "L" || key == "/L" || key == "NL" || key == "/NL" || key == "PR" || key == "/PR")
		{
			return;
		}
		global::Defs.ColorizeLocalizedText(ref field.value, in_wiki);
		if (field.children == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field2 = field.children[i];
			this.ColorizeLocalizedText(field2, in_wiki);
		}
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00030D7C File Offset: 0x0002EF7C
	public static bool NeedsLigatureFix(string lang_key)
	{
		if (!global::Defs.IsPlaying())
		{
			return false;
		}
		if (string.IsNullOrEmpty(lang_key))
		{
			lang_key = (global::Defs.loading_language ?? global::Defs.Language);
		}
		return lang_key == "hi";
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00030DAF File Offset: 0x0002EFAF
	public static string FixRawTextLigatures(string text, string lang_key)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (string.IsNullOrEmpty(lang_key))
		{
			lang_key = (global::Defs.loading_language ?? global::Defs.Language);
		}
		if (lang_key == "hi")
		{
			return global::Defs.FixHindiLigatures(text);
		}
		return text;
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00030DE8 File Offset: 0x0002EFE8
	private static string FixHindiLigatures(string text)
	{
		return HindiTextConverter.Convert(text);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00030DF0 File Offset: 0x0002EFF0
	private static string ExtractTextAndFixLigatures(string text, int start_idx, int end_idx, string lang_key)
	{
		int i;
		for (i = start_idx; i < end_idx; i++)
		{
			char c = text[i];
			if (c != '\r' && c != '\n' && c != ' ' && c != '\t')
			{
				break;
			}
		}
		int j;
		for (j = end_idx; j > i; j--)
		{
			char c2 = text[j - 1];
			if (c2 != '\r' && c2 != '\n' && c2 != ' ' && c2 != '\t')
			{
				break;
			}
		}
		string text2 = "";
		if (i > start_idx)
		{
			text2 += text.Substring(start_idx, i - start_idx);
		}
		if (j > i)
		{
			string str = global::Defs.FixRawTextLigatures(text.Substring(i, j - i), lang_key);
			text2 += str;
		}
		if (end_idx > j)
		{
			text2 += text.Substring(j, end_idx - j);
		}
		return text2;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00030EA4 File Offset: 0x0002F0A4
	public static string FixLigatures(string text, string lang_key, bool check_needs_fix, bool in_wiki)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (check_needs_fix && !global::Defs.NeedsLigatureFix(lang_key))
		{
			return text;
		}
		string text2 = "";
		int num = 0;
		int length = text.Length;
		int num2;
		int num3;
		while (num < length && global::Defs.FindMarkup(text, num, out num2, out num3, in_wiki))
		{
			if (num2 > num)
			{
				text2 += global::Defs.ExtractTextAndFixLigatures(text, num, num2, lang_key);
			}
			text2 += text.Substring(num2, num3 - num2);
			num = num3;
		}
		if (num < length)
		{
			text2 += global::Defs.ExtractTextAndFixLigatures(text, num, length, lang_key);
		}
		return text2;
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00030F2C File Offset: 0x0002F12C
	private static void FixLigatures(ref Value val, string lang_key, bool in_wiki)
	{
		if (val.is_string)
		{
			string text = val.String(null);
			val.obj_val = global::Defs.FixLigatures(text, lang_key, false, in_wiki);
			return;
		}
		List<DT.SubValue> list = val.obj_val as List<DT.SubValue>;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			global::Defs.FixLigatures(ref list[i].value, lang_key, in_wiki);
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00030F90 File Offset: 0x0002F190
	public void FixLigatures(DT.Field field, string lang_key, bool check_needs_fix, bool in_wiki)
	{
		if (check_needs_fix && !global::Defs.NeedsLigatureFix(lang_key))
		{
			return;
		}
		global::Defs.FixLigatures(ref field.value, lang_key, in_wiki);
		if (field.children == null)
		{
			return;
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field2 = field.children[i];
			this.FixLigatures(field2, lang_key, false, in_wiki);
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00030FF0 File Offset: 0x0002F1F0
	private static Value CreateFallbackTextValue(Value val, string open_tag, string close_tag, bool in_wiki)
	{
		object obj_val = val.obj_val;
		if (obj_val != null)
		{
			string text;
			if ((text = (obj_val as string)) == null)
			{
				List<DT.SubValue> list;
				if ((list = (obj_val as List<DT.SubValue>)) != null)
				{
					List<DT.SubValue> list2 = list;
					List<DT.SubValue> list3 = new List<DT.SubValue>(list2.Count);
					for (int i = 0; i < list2.Count; i++)
					{
						Value value = global::Defs.CreateFallbackTextValue(list2[i].value, open_tag, close_tag, in_wiki);
						list3.Add(new DT.SubValue
						{
							value = value
						});
					}
					return new Value(list3);
				}
			}
			else
			{
				string text2 = text;
				if (string.IsNullOrEmpty(text2))
				{
					return text2;
				}
				return global::Defs.ColorizeLocalizedText(text2, open_tag, close_tag, in_wiki);
			}
		}
		return val;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0003109C File Offset: 0x0002F29C
	private static DT.Field CreateFallbackTextField(DT.Field field, int enable_var, string open_tag, string close_tag)
	{
		if (enable_var != 1)
		{
			return field;
		}
		if (((field != null) ? field.dt : null) != null && field.Type() != "text")
		{
			return field;
		}
		bool in_wiki = field.key.StartsWith("article.", StringComparison.Ordinal);
		DT.Field field2 = new DT.Field(null);
		field2.parent = field.parent;
		field2.line = field.line;
		field2.key = field.key + ".fallback";
		field2.value_str = field.value_str;
		field2.value = global::Defs.CreateFallbackTextValue(field.value, open_tag, close_tag, in_wiki);
		if (field.children == null)
		{
			return field2;
		}
		field2.children = new List<DT.Field>(field.children.Count);
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field3 = field.children[i];
			if (!string.IsNullOrEmpty(field3.key))
			{
				DT.Field field4 = field2.AddChild(field3.key);
				field4.value_str = field3.value_str;
				field4.value = global::Defs.CreateFallbackTextValue(field3.value, open_tag, close_tag, in_wiki);
			}
		}
		return field2;
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x000311B4 File Offset: 0x0002F3B4
	private void LoadText(string prefix, DT.Field field, DT.Field lang_field = null)
	{
		if (!(((field != null) ? field.key : null) == global::Defs.debug_load_text_key))
		{
			string a;
			if (field == null)
			{
				a = null;
			}
			else
			{
				DT.Field parent = field.parent;
				a = ((parent != null) ? parent.key : null);
			}
			if (!(a == global::Defs.debug_load_text_key))
			{
				goto IL_4C;
			}
		}
		Debug.Log(string.Format("Loading {0}", field));
		IL_4C:
		if (global::Defs.import_forms_as_languages && lang_field == null && field.children != null)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				if (!string.IsNullOrEmpty(field2.key) && !global::Defs.TextInfo.IsEmpty(field2))
				{
					this.LoadText(prefix, field, field2);
				}
			}
			return;
		}
		string lang_key;
		DT.Field field3;
		if (lang_field == null)
		{
			lang_key = (global::Defs.loading_language ?? global::Defs.Language);
			field3 = field;
		}
		else
		{
			lang_key = lang_field.key;
			field3 = lang_field;
		}
		if (field.key.StartsWith("article.", StringComparison.Ordinal) && global::Defs.loading_language != "resolved")
		{
			Debug.LogError(string.Format("Attempting to load wiki article from text field: {0}", field));
			return;
		}
		this.FixLigatures(field3, lang_key, true, false);
		this.ColorizeLocalizedText(field3, false);
		string str;
		string text;
		int num;
		this.ParseTextKey(field, out str, out text, out num);
		string text2 = prefix + str;
		global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(text2, true, true);
		int idx;
		global::Defs.TextInfo.LangInfo languageInfo = textInfo.GetLanguageInfo(lang_key, out idx, true);
		DT.Field field4 = languageInfo.localized_field;
		if (text != null)
		{
			DT.Field field5 = null;
			if (field4 == null)
			{
				field4 = new DT.Field(null);
				field4.key = text2;
				field4.parent = field.parent;
				languageInfo.localized_field = field4;
				textInfo.SetLanguageInfo(languageInfo, idx);
			}
			else
			{
				field5 = field4.FindChild(text, null, true, true, true, '.');
			}
			if (num > 0)
			{
				global::Defs.AddVariant(field5, field3, num);
				return;
			}
			if (field5 != null)
			{
				this.ReplaceValue(field5, field3);
				return;
			}
			field3.key = text;
			field4.AddChild(field3);
			return;
		}
		else
		{
			if (num > 0)
			{
				global::Defs.AddVariant(field4, field3, num);
				return;
			}
			if (field4 != null)
			{
				this.ReplaceValue(field4, field3);
				return;
			}
			languageInfo.localized_field = field3;
			textInfo.SetLanguageInfo(languageInfo, idx);
			if (global::Defs.store_loaded_texts_in != null)
			{
				global::Defs.store_loaded_texts_in.Add(textInfo);
			}
			return;
		}
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x000313C8 File Offset: 0x0002F5C8
	private void LoadTexts(List<DT.Field> fields, string prefix = "")
	{
		if (fields == null)
		{
			return;
		}
		for (int i = 0; i < fields.Count; i++)
		{
			DT.Field field = fields[i];
			if (!(field.key == ""))
			{
				if (field.children != null)
				{
					for (int j = 0; j < field.children.Count; j++)
					{
						if (field.children[j].key == "")
						{
							field.children.RemoveAt(j);
							j--;
						}
					}
					if (field.children.Count == 0)
					{
						field.children = null;
					}
				}
				if (field.key.EndsWith(".", StringComparison.Ordinal))
				{
					if (!field.FileName().EndsWith(".def", StringComparison.Ordinal))
					{
						Debug.LogError(string.Format("Non-def text key ends with '.': {0}", field));
					}
					this.LoadTexts(field.children, prefix + field.key);
				}
				else
				{
					this.LoadText(prefix, field, null);
				}
				if (global::Defs.colorize_localized_texts == 0 && field.key == "L")
				{
					global::Defs.colorize_localized_texts = 1;
				}
				if ((field.key == "NL" || field.key == "/NL") && (global::Defs.enable_fallback_texts == 0 || global::Defs.enable_fallback_texts == 1))
				{
					if (string.IsNullOrEmpty(field.String(null, "")))
					{
						global::Defs.enable_fallback_texts = 2;
					}
					else if (global::Defs.enable_fallback_texts == 0)
					{
						global::Defs.enable_fallback_texts = 1;
					}
				}
				if ((field.key == "PR" || field.key == "/PR") && (global::Defs.enable_proofread_texts_in_dev == 0 || global::Defs.enable_proofread_texts_in_dev == 1))
				{
					if (string.IsNullOrEmpty(field.String(null, "")))
					{
						global::Defs.enable_proofread_texts_in_dev = 2;
					}
					else if (global::Defs.enable_proofread_texts_in_dev == 0)
					{
						global::Defs.enable_proofread_texts_in_dev = 1;
					}
				}
			}
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0003159C File Offset: 0x0002F79C
	private void LoadDefTexts(string path, string text)
	{
		List<DT.Field> fields = new DT.Parser(null, text, 0, -1).ReadFields(new DT.Field(null)
		{
			type = "file",
			key = Path.GetFileName(path),
			value_str = DT.Enquote(path)
		}, false);
		this.LoadTexts(fields, "");
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x000315F0 File Offset: 0x0002F7F0
	private static void ReplaceFormVars(DT.Field field, DT.Field child, List<string> ignore_keys = null)
	{
		if (child == field)
		{
			return;
		}
		if (!(((field != null) ? field.key : null) == global::Defs.debug_load_text_key))
		{
			string a;
			if (field == null)
			{
				a = null;
			}
			else
			{
				DT.Field parent = field.parent;
				a = ((parent != null) ? parent.key : null);
			}
			if (!(a == global::Defs.debug_load_text_key))
			{
				goto IL_51;
			}
		}
		Debug.Log(string.Format("Replacing form vars: {0}", field));
		IL_51:
		if (ignore_keys == null)
		{
			global::Defs.tmp_ignore_keys.Clear();
			ignore_keys = global::Defs.tmp_ignore_keys;
		}
		string text = child.key;
		if (ignore_keys.IndexOf(text) >= 0)
		{
			return;
		}
		ignore_keys.Add(text);
		string value_str = child.value_str;
		global::Defs.tmp_sb.Clear();
		StringBuilder stringBuilder = global::Defs.tmp_sb;
		int num = 0;
		int i = 0;
		while (i < value_str.Length)
		{
			int num2 = value_str.IndexOf('{', i);
			if (num2 < 0)
			{
				break;
			}
			int num3 = value_str.IndexOf('}', num2);
			if (num3 < 0)
			{
				break;
			}
			text = value_str.Substring(num2 + 1, num3 - num2 - 1);
			DT.Field field2 = (text == "base") ? field : field.FindChild(text, null, true, true, true, '.');
			if (field2 == null)
			{
				i = num3 + 1;
			}
			else
			{
				global::Defs.ReplaceFormVars(field, field2, ignore_keys);
				string value_str2 = field2.value_str;
				if (num2 > num)
				{
					stringBuilder.Append(value_str, num, num2 - num);
				}
				stringBuilder.Append(value_str2);
				i = (num = num3 + 1);
			}
		}
		if (num == 0)
		{
			return;
		}
		if (num < value_str.Length)
		{
			stringBuilder.Append(value_str, num, value_str.Length - num);
		}
		child.value_str = stringBuilder.ToString();
		child.value = child.value_str;
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0003177C File Offset: 0x0002F97C
	private static void PostProcessCSVTextField(DT.Field field, DT.Field default_row)
	{
		if (!(((field != null) ? field.key : null) == global::Defs.debug_load_text_key))
		{
			string a;
			if (field == null)
			{
				a = null;
			}
			else
			{
				DT.Field parent = field.parent;
				a = ((parent != null) ? parent.key : null);
			}
			if (!(a == global::Defs.debug_load_text_key))
			{
				goto IL_4C;
			}
		}
		Debug.Log(string.Format("post-processing {0}", field));
		IL_4C:
		if (field.base_path != null)
		{
			field.key = field.key + ":" + field.base_path;
			field.base_path = null;
		}
		if (field.type != "")
		{
			Debug.LogError(field.Path(true, false, '.') + " has type '" + field.type + "' - spaces in the key are not allowed");
		}
		if (field.key == "")
		{
			return;
		}
		if (field.children == null)
		{
			return;
		}
		if (default_row != null)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				if (!(field2.key == "") && field2.value_str == "")
				{
					field2.value_str = default_row.GetValueStr(field2.key, "", true, true, true, '.');
					field2.value = Value.Null;
				}
			}
		}
		if (!global::Defs.import_forms_as_languages)
		{
			bool flag = false;
			for (int j = 0; j < field.children.Count; j++)
			{
				DT.Field field3 = field.children[j];
				if (!(field3.key == ""))
				{
					if (field3.key[0] == ':')
					{
						string text = field3.key.Substring(1);
						if (field.FindChild(text, null, true, true, true, '.') != null)
						{
							Debug.LogError(string.Concat(new string[]
							{
								field.Path(true, false, '.'),
								" has duplicated forms: '",
								text,
								"' and ':",
								text,
								"'"
							}));
						}
						field3.key = text;
						if (field.children_by_key != null)
						{
							field.children_by_key = null;
							flag = true;
						}
					}
					global::Defs.ReplaceFormVars(field, field3, null);
				}
			}
			if (flag)
			{
				field.BuildChildrenIndex();
			}
		}
		for (int k = 0; k < field.children.Count; k++)
		{
			DT.Field field4 = field.children[k];
			if (!(field4.key == "") && !field4.value.is_valid)
			{
				field4.value = DT.ResolveValue(field4, field4.value_str, false, false);
				if (!field4.value.is_valid && !string.IsNullOrEmpty(field4.value_str))
				{
					field4.value = field4.value_str;
				}
			}
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00031A40 File Offset: 0x0002FC40
	public static void PostProcessCSVTexts(DT.Field root)
	{
		if (root.children.Count == 0)
		{
			return;
		}
		DT.Field default_row = null;
		DT.Field field = root.children[0];
		if (field.comment2 == "default")
		{
			default_row = field;
			root.children.RemoveAt(0);
		}
		for (int i = 0; i < root.children.Count; i++)
		{
			global::Defs.PostProcessCSVTextField(root.children[i], default_row);
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00031AB4 File Offset: 0x0002FCB4
	private void LoadCSVTexts(string path, string text)
	{
		DT.Field field = DT.ParseCSV(null, path, text, '\0', "base", "");
		if (field == null || field.children == null)
		{
			return;
		}
		global::Defs.PostProcessCSVTexts(field);
		this.LoadTexts(field.children, "");
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00031AF8 File Offset: 0x0002FCF8
	private void LoadWikiTexts(string path, string text)
	{
		this.wiki.ExtractTexts(text, path);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00031B08 File Offset: 0x0002FD08
	private void LoadTexts(string path, string text, string ext)
	{
		if (ext == ".def")
		{
			this.LoadDefTexts(path, text);
			return;
		}
		if (ext == ".csv")
		{
			this.LoadCSVTexts(path, text);
			return;
		}
		if (ext == ".wiki")
		{
			this.LoadWikiTexts(path, text);
			return;
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00031B58 File Offset: 0x0002FD58
	public Value ResolveValue(DT.Field field, string sval, string comment, bool main_value)
	{
		if (field.Type() == "color")
		{
			return new Value(global::Defs.ColorFromString(sval));
		}
		if (!sval.StartsWith("assets/", StringComparison.OrdinalIgnoreCase))
		{
			return Value.Unknown;
		}
		Profile.BeginSection("Resolve asset value");
		string path = sval;
		string text = null;
		int subasset_index = 1;
		int num = sval.LastIndexOf(':');
		if (num >= 0)
		{
			path = sval.Substring(0, num);
			text = sval.Substring(num + 1);
		}
		if (text != null)
		{
			num = text.IndexOf('#');
			if (num > 0)
			{
				int.TryParse(text.Substring(num + 1), out subasset_index);
				text = text.Substring(0, num);
			}
		}
		object @object = Assets.GetObject(path, text, subasset_index);
		Value result = new Value(@object);
		global::Defs.fixnull(ref result);
		Profile.EndSection("Resolve asset value");
		return result;
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00031C1C File Offset: 0x0002FE1C
	private void ResolveValue(DT.Field field)
	{
		if (!field.value.is_valid)
		{
			Value value = this.ResolveValue(field, field.ValueStr(), field.Comment1(), true);
			if (value.is_valid)
			{
				field.value = value;
			}
			return;
		}
		if (field.value.type == Value.Type.String)
		{
			string sval = field.value;
			Value value2 = this.ResolveValue(field, sval, field.comment1, true);
			if (value2.is_valid)
			{
				field.value = value2;
			}
			return;
		}
		List<DT.SubValue> list = field.value.obj_val as List<DT.SubValue>;
		if (list != null)
		{
			int i = 0;
			while (i < list.Count)
			{
				DT.SubValue subValue = list[i];
				string sval2;
				if (!subValue.value.is_valid)
				{
					sval2 = subValue.value_str;
					goto IL_CF;
				}
				if (subValue.value.type == Value.Type.String)
				{
					sval2 = (string)subValue.value.obj_val;
					goto IL_CF;
				}
				IL_F4:
				i++;
				continue;
				IL_CF:
				Value value3 = this.ResolveValue(field, sval2, subValue.comment2, false);
				if (value3.is_valid)
				{
					subValue.value = value3;
					goto IL_F4;
				}
				goto IL_F4;
			}
			return;
		}
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00031D30 File Offset: 0x0002FF30
	private void LoadDefFile(string path)
	{
		try
		{
			this.dt.LoadDefFile(path);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00031D64 File Offset: 0x0002FF64
	private void LoadCSVFile(string path)
	{
		try
		{
			DT.LoadCSVFile(this.dt, path, '\0', "<value>", "");
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00031DA4 File Offset: 0x0002FFA4
	private void LoadFile(string path)
	{
		string a = Path.GetExtension(path).ToLowerInvariant();
		if (a == ".def" || a == ".dt")
		{
			this.LoadDefFile(path);
			return;
		}
		if (a == ".csv")
		{
			this.LoadCSVFile(path);
			return;
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00031DF4 File Offset: 0x0002FFF4
	private void LoadTextFile(string path)
	{
		try
		{
			string text = Path.GetExtension(path).ToLowerInvariant();
			if (!(text != ".def") || !(text != ".csv") || !(text != ".wiki"))
			{
				string text2 = DT.ReadTextFile(path);
				this.LoadTexts(path, text2, text);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00031E60 File Offset: 0x00030060
	public void Reload()
	{
		this.Load();
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0002C538 File Offset: 0x0002A738
	public static bool GetSaveStrings(UnityEngine.Object obj, ref string value_str, ref string comment)
	{
		return false;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00031E68 File Offset: 0x00030068
	public static bool GetSaveString(ref string value_str, Value value)
	{
		if (!string.IsNullOrEmpty(value_str))
		{
			return false;
		}
		switch (value.type)
		{
		case Value.Type.Int:
			value_str = value.int_val.ToString();
			return true;
		case Value.Type.Float:
			value_str = DT.FloatToStr(value.float_val, int.MaxValue);
			return true;
		case Value.Type.String:
			value_str = DT.Enquote(value.String(null));
			return true;
		default:
			return false;
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00031ED4 File Offset: 0x000300D4
	public static bool GetSaveString(List<DT.SubValue> values, ref string value_str, bool slashed)
	{
		if (values == null)
		{
			return false;
		}
		value_str = (slashed ? "" : "[");
		for (int i = 0; i < values.Count; i++)
		{
			DT.SubValue subValue = values[i];
			value_str += subValue.comment1;
			string value_str2 = subValue.value_str;
			global::Defs.GetSaveString(ref value_str2, subValue.value);
			string comment = subValue.comment2;
			global::Defs.GetSaveStrings(subValue.value.obj_val as UnityEngine.Object, ref value_str2, ref comment);
			global::Defs.GetSaveString(subValue.value.obj_val as List<DT.SubValue>, ref value_str2, false);
			value_str += value_str2;
			if (i + 1 < values.Count)
			{
				value_str += (slashed ? "/" : ",");
			}
			if (comment != "")
			{
				value_str = value_str + " " + comment + "\r\n";
			}
			value_str += subValue.comment3;
		}
		if (!slashed)
		{
			value_str += "]";
		}
		return true;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00031FE3 File Offset: 0x000301E3
	public static string GetSaveString(DT.Field field, string ident = "", DT.Field prev = null)
	{
		if (field == null)
		{
			return null;
		}
		if ((field.flags & DT.Field.Flags.DontSave) != (DT.Field.Flags)0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		global::Defs.Save(stringBuilder, field, ident, prev);
		return stringBuilder.ToString();
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00032010 File Offset: 0x00030210
	public static void Save(StringBuilder stringBuilder, DT.Field field, string ident = "", DT.Field prev = null)
	{
		if ((field.flags & DT.Field.Flags.DontSave) != (DT.Field.Flags)0)
		{
			return;
		}
		if ((field.flags & DT.Field.Flags.StartsAtSameLine) == (DT.Field.Flags)0)
		{
			stringBuilder.Append("\r\n" + ident);
		}
		else if (prev != null)
		{
			stringBuilder.Append((prev.children == null) ? "; " : " ");
		}
		else if (field.parent != null && (field.parent.parent != null || field.parent.type != "file"))
		{
			stringBuilder.Append(" ");
		}
		else
		{
			stringBuilder.Append("");
		}
		if (field.type != "")
		{
			stringBuilder.Append(field.type + " ");
		}
		stringBuilder.Append(field.key);
		if (field.base_path != null)
		{
			stringBuilder.Append(" : " + field.base_path);
		}
		string value_str = field.value_str;
		string text = field.comment1;
		if (field.value.is_object)
		{
			global::Defs.GetSaveStrings(field.value.obj_val as UnityEngine.Object, ref value_str, ref text);
			global::Defs.GetSaveString(field.value.obj_val as List<DT.SubValue>, ref value_str, (field.flags & DT.Field.Flags.SlashedList) > (DT.Field.Flags)0);
		}
		else
		{
			global::Defs.GetSaveString(ref value_str, field.value);
			if (text.StartsWith("//GUID:", StringComparison.Ordinal))
			{
				text = "";
			}
		}
		if (value_str != "")
		{
			stringBuilder.Append(" = " + value_str);
		}
		if (text != "")
		{
			if (field.key != "")
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(text);
		}
		if (field.children == null)
		{
			return;
		}
		if ((field.flags & DT.Field.Flags.OpenBraceAtSameLine) != (DT.Field.Flags)0 && field.comment1 == "")
		{
			stringBuilder.Append(" {");
		}
		else
		{
			stringBuilder.Append("\r\n" + ident + "{");
		}
		if (field.comment2 != "")
		{
			stringBuilder.Append(" " + field.comment2);
		}
		for (int i = 0; i < field.children.Count; i++)
		{
			stringBuilder.Append(global::Defs.Save(field.children[i], ident + "\t", (i > 0) ? field.children[i - 1] : null));
		}
		if ((field.flags & DT.Field.Flags.ClosingBraceAtSameLine) != (DT.Field.Flags)0)
		{
			stringBuilder.Append(" }");
		}
		else
		{
			stringBuilder.Append("\r\n" + ident + "}");
		}
		if (field.comment3 != "")
		{
			stringBuilder.Append(" " + field.comment3);
		}
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x000322ED File Offset: 0x000304ED
	public static string Save(DT.Field field, string ident = "", DT.Field prev = null)
	{
		return global::Defs.GetSaveString(field, ident, prev);
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x000322F8 File Offset: 0x000304F8
	public static string SaveCSV(DT.Field field, char delimiter = '\0', string value_key = "<value>")
	{
		if (delimiter == '\0')
		{
			delimiter = ',';
		}
		string str = field.comment2;
		if (field.comment1 != "")
		{
			str = field.comment1;
		}
		if (field.children != null)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				DT.Field field2 = field.children[i];
				string value_str = field2.value_str;
				if (field2.value.is_object)
				{
					string text = "";
					global::Defs.GetSaveStrings(field2.value.obj_val as UnityEngine.Object, ref value_str, ref text);
				}
				str = str + delimiter.ToString() + value_str;
			}
		}
		return str + "\r\n";
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x000323A8 File Offset: 0x000305A8
	public static void SaveFile(DT.Field file, string path = null)
	{
		if (path == null)
		{
			path = DT.Unquote(file.value_str);
		}
		string a = Path.GetExtension(path).ToLowerInvariant();
		bool flag = a == ".csv";
		StringBuilder stringBuilder = new StringBuilder(32768);
		if (flag)
		{
			stringBuilder.Append(file.comment1 + "\r\n");
		}
		if (file.children != null)
		{
			for (int i = 0; i < file.children.Count; i++)
			{
				DT.Field field = file.children[i];
				string value = flag ? global::Defs.SaveCSV(field, '\0', "<value>") : global::Defs.Save(field, "", (i > 0) ? file.children[i - 1] : null);
				stringBuilder.Append(value);
			}
		}
		string contents = stringBuilder.ToString();
		if (a == ".wiki")
		{
			path += ".def";
		}
		File.WriteAllText(path, contents, Encoding.UTF8);
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x000324A0 File Offset: 0x000306A0
	public void OnDefsProcessed()
	{
		try
		{
			MercenaryMission.LoadDefs(GameLogic.Get(false));
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		try
		{
			Logic.Settlement.ClearTypesCache();
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.ToString());
		}
		Profile.BeginSection("BaseUI.LoadDefs");
		try
		{
			BaseUI.LoadDefs();
		}
		catch (Exception ex3)
		{
			Debug.LogError(ex3.ToString());
		}
		Profile.EndSection("BaseUI.LoadDefs");
		Profile.BeginSection("ViewMode.LoadDefs");
		try
		{
			ViewMode.LoadDefs();
		}
		catch (Exception ex4)
		{
			Debug.LogError(ex4.ToString());
		}
		Profile.EndSection("ViewMode.LoadDefs");
		Profile.BeginSection("Recalc rebellion risk");
		try
		{
			if (Application.isPlaying)
			{
				Game game = GameLogic.Get(true);
				if (game.realms != null)
				{
					for (int i = 0; i < game.realms.Count; i++)
					{
						Logic.Realm realm = game.realms[i];
						if (((realm != null) ? realm.rebellionRisk : null) != null)
						{
							realm.rebellionRisk = new RebellionRisk(realm);
							realm.rebellionRisk.Recalc(false, true);
						}
					}
				}
				if (Troops.Initted)
				{
					Profile.BeginSection("Defs.LoadTroopDefs");
					TextureBaker texture_baker = Troops.texture_baker;
					if (texture_baker != null)
					{
						texture_baker.SetStanceColors();
					}
					BattleMap battleMap = BattleMap.Get();
					if (battleMap != null)
					{
						battleMap.SetTroopDefs();
					}
					Profile.EndSection("Defs.LoadTroopDefs");
				}
			}
		}
		catch (Exception ex5)
		{
			Debug.LogError(ex5.ToString());
		}
		Profile.EndSection("Recalc rebellion risk");
		Profile.BeginSection("OnDefsProcessedEvent.Invoke");
		try
		{
			Action onDefsProcessedEvent = global::Defs.OnDefsProcessedEvent;
			if (onDefsProcessedEvent != null)
			{
				onDefsProcessedEvent();
			}
		}
		catch (Exception ex6)
		{
			Debug.LogError(ex6.ToString());
		}
		Profile.EndSection("OnDefsProcessedEvent.Invoke");
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00032668 File Offset: 0x00030868
	[ConsoleMethod("rd", "Reload defs")]
	public void ReloadDefsCmd()
	{
		this.Reload();
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x000329BC File Offset: 0x00030BBC
	[CompilerGenerated]
	internal static char <FakeTranslate>g__ChooseRandomChar|112_0(ref global::Defs.<>c__DisplayClass112_0 A_0)
	{
		int num = A_0.rnd.Next(A_0.total_chars);
		for (int i = 0; i < A_0.ranges.Count; i++)
		{
			global::Defs.UnicodeRange unicodeRange = A_0.ranges[i];
			if (num < unicodeRange.length)
			{
				return (char)(unicodeRange.start_idx + num);
			}
			num -= unicodeRange.length;
		}
		return '\0';
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00032A1E File Offset: 0x00030C1E
	[CompilerGenerated]
	internal static void <LoadTexts>g__AddTiming|135_0(long t, string name, ref global::Defs.<>c__DisplayClass135_0 A_2)
	{
		if (t < 10L)
		{
			return;
		}
		A_2.log += string.Format("\n    {0}: {1}ms", name, t);
	}

	// Token: 0x0400040E RID: 1038
	public const string guid_comment = "//GUID:";

	// Token: 0x0400040F RID: 1039
	public static string set_language = null;

	// Token: 0x04000410 RID: 1040
	public static string def_language = "en";

	// Token: 0x04000411 RID: 1041
	public static int default_language_version;

	// Token: 0x04000412 RID: 1042
	[HideInInspector]
	public string cur_file;

	// Token: 0x04000413 RID: 1043
	[HideInInspector]
	public int verbose = 1;

	// Token: 0x04000414 RID: 1044
	public DT dt;

	// Token: 0x04000415 RID: 1045
	public List<DT.Field> languages = new List<DT.Field>();

	// Token: 0x04000416 RID: 1046
	public Dictionary<string, global::Defs.TextInfo> texts = new Dictionary<string, global::Defs.TextInfo>();

	// Token: 0x04000417 RID: 1047
	public Wiki wiki = new Wiki();

	// Token: 0x04000418 RID: 1048
	[HideInInspector]
	public int num_reloads;

	// Token: 0x04000419 RID: 1049
	[HideInInspector]
	public int num_changes;

	// Token: 0x0400041A RID: 1050
	private static global::Defs instance = null;

	// Token: 0x0400041B RID: 1051
	private static string language = null;

	// Token: 0x0400041C RID: 1052
	public static Vars empty_vars = new Vars();

	// Token: 0x0400041D RID: 1053
	private static char[] clr_separators = new char[]
	{
		',',
		' '
	};

	// Token: 0x0400041E RID: 1054
	private static Vars language_vars = null;

	// Token: 0x0400041F RID: 1055
	public static List<global::Defs.TextInfo> resolved_texts = new List<global::Defs.TextInfo>();

	// Token: 0x04000420 RID: 1056
	public static string[] RomanDigits = new string[]
	{
		"",
		"I",
		"II",
		"III",
		"IV",
		"V",
		"VI",
		"VII",
		"VIII",
		"IX"
	};

	// Token: 0x04000421 RID: 1057
	private static StringBuilder tmp_LowerCaseSB = new StringBuilder();

	// Token: 0x04000422 RID: 1058
	private static List<global::Defs.RVStackItem> rv_stack = new List<global::Defs.RVStackItem>(64);

	// Token: 0x04000423 RID: 1059
	public static string debug_replace_var = null;

	// Token: 0x04000424 RID: 1060
	public static bool allow_wildcard_form = true;

	// Token: 0x04000425 RID: 1061
	private static List<global::Defs.UnicodeRange> latin_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange('A', 'Z'),
		new global::Defs.UnicodeRange('a', 'z')
	};

	// Token: 0x04000426 RID: 1062
	private static List<global::Defs.UnicodeRange> cyrillic_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange('А', 'Я'),
		new global::Defs.UnicodeRange('а', 'я')
	};

	// Token: 0x04000427 RID: 1063
	private static List<global::Defs.UnicodeRange> CJK_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange(12288, 12351),
		new global::Defs.UnicodeRange(12448, 12543),
		new global::Defs.UnicodeRange(12352, 12447),
		new global::Defs.UnicodeRange(12784, 12799),
		new global::Defs.UnicodeRange(63744, 64217),
		new global::Defs.UnicodeRange(19968, 40869)
	};

	// Token: 0x04000428 RID: 1064
	private static List<global::Defs.UnicodeRange> traditional_chinese_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange(19968, 40908)
	};

	// Token: 0x04000429 RID: 1065
	private static List<global::Defs.UnicodeRange> hindi_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange(2304, 2431)
	};

	// Token: 0x0400042A RID: 1066
	private static List<global::Defs.UnicodeRange> korean_unicode_ranges = new List<global::Defs.UnicodeRange>
	{
		new global::Defs.UnicodeRange(4352, 4607),
		new global::Defs.UnicodeRange(12592, 12687),
		new global::Defs.UnicodeRange(43360, 43391),
		new global::Defs.UnicodeRange(44032, 55203),
		new global::Defs.UnicodeRange(55216, 55295)
	};

	// Token: 0x0400042B RID: 1067
	public static DT.Field currently_localizing = null;

	// Token: 0x0400042C RID: 1068
	private static string loading_language = null;

	// Token: 0x0400042D RID: 1069
	public static bool silent_reload = false;

	// Token: 0x0400042E RID: 1070
	public static int colorize_localized_texts = 0;

	// Token: 0x0400042F RID: 1071
	public static int enable_fallback_texts = 0;

	// Token: 0x04000430 RID: 1072
	public static int enable_proofread_texts_in_dev = 0;

	// Token: 0x04000431 RID: 1073
	public static string fake_translation_to = null;

	// Token: 0x04000432 RID: 1074
	private static readonly char[] markup_start_chars = new char[]
	{
		'{',
		'<',
		'[',
		']',
		'|'
	};

	// Token: 0x04000433 RID: 1075
	private static readonly char[] wiki_markup_start_chars = new char[]
	{
		'{',
		'<',
		'[',
		']',
		'|',
		'#',
		'/',
		'\r',
		'\n'
	};

	// Token: 0x04000434 RID: 1076
	private static readonly char[] eol_chars = new char[]
	{
		'\r',
		'\n'
	};

	// Token: 0x04000435 RID: 1077
	private static readonly char[] category_end_chars = new char[]
	{
		':',
		'\r',
		'\n'
	};

	// Token: 0x04000436 RID: 1078
	public static bool import_forms_as_languages = false;

	// Token: 0x04000437 RID: 1079
	public static List<global::Defs.TextInfo> store_loaded_texts_in = null;

	// Token: 0x04000438 RID: 1080
	public static string debug_load_text_key = null;

	// Token: 0x04000439 RID: 1081
	private static List<string> tmp_ignore_keys = new List<string>();

	// Token: 0x0400043A RID: 1082
	private static StringBuilder tmp_sb = new StringBuilder(128);

	// Token: 0x0400043B RID: 1083
	public static Action OnDefsProcessedEvent;

	// Token: 0x02000539 RID: 1337
	public class TextInfo
	{
		// Token: 0x0600434B RID: 17227 RVA: 0x001FCC72 File Offset: 0x001FAE72
		public DT.Field GetTextField(IVars vars = null, bool force_resolve = false)
		{
			if (this.txt_field != null && !force_resolve && !this.has_cases)
			{
				return this.txt_field;
			}
			this.txt_field = this.ResolveField(vars, true, force_resolve);
			return this.txt_field;
		}

		// Token: 0x0600434C RID: 17228 RVA: 0x001FCCA3 File Offset: 0x001FAEA3
		public DT.Field GetLocalizedField(string lang_key)
		{
			return this.GetLanguageInfo(lang_key, false).localized_field;
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x001FCCB4 File Offset: 0x001FAEB4
		public global::Defs.TextInfo.LangInfo GetLanguageInfo(string lang_key, out int idx, bool init_if_not_found = false)
		{
			idx = this.FindLanguageIdx(lang_key);
			global::Defs.TextInfo.LangInfo result;
			if (idx >= 0)
			{
				result = this.languages[idx];
			}
			else if (init_if_not_found)
			{
				result = new global::Defs.TextInfo.LangInfo
				{
					lang_key = lang_key
				};
			}
			else
			{
				result = default(global::Defs.TextInfo.LangInfo);
			}
			return result;
		}

		// Token: 0x0600434E RID: 17230 RVA: 0x001FCD00 File Offset: 0x001FAF00
		public global::Defs.TextInfo.LangInfo GetLanguageInfo(string lang_key, bool init_if_not_found = false)
		{
			int num;
			return this.GetLanguageInfo(lang_key, out num, init_if_not_found);
		}

		// Token: 0x0600434F RID: 17231 RVA: 0x001FCD18 File Offset: 0x001FAF18
		public void SetLanguageInfo(global::Defs.TextInfo.LangInfo li, int idx = -1000)
		{
			if (li.lang_key == null)
			{
				Game.Log(string.Format("Attempting to set unknown language info for {0}: {1}", this, li), Game.LogType.Error);
				return;
			}
			if (idx == -1000)
			{
				idx = this.FindLanguageIdx(li.lang_key);
			}
			if (idx < 0)
			{
				if (this.languages == null)
				{
					this.languages = new List<global::Defs.TextInfo.LangInfo>();
				}
				this.languages.Add(li);
				return;
			}
			this.languages[idx] = li;
		}

		// Token: 0x06004350 RID: 17232 RVA: 0x001FCD8C File Offset: 0x001FAF8C
		private int FindLanguageIdx(string lang_key)
		{
			if (string.IsNullOrEmpty(lang_key))
			{
				return -3;
			}
			if (this.languages == null)
			{
				return -2;
			}
			for (int i = 0; i < this.languages.Count; i++)
			{
				if (this.languages[i].lang_key == lang_key)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004351 RID: 17233 RVA: 0x001FCDE4 File Offset: 0x001FAFE4
		private DT.Field ResolveField(IVars vars = null, bool allow_fallbacks = true, bool force_resolve_def_field = false)
		{
			if (this.text_key == global::Defs.TextInfo.debug_resolve_field && allow_fallbacks)
			{
				Debug.Log("Resolving " + this.text_key);
			}
			global::Defs.TextInfo.LangInfo languageInfo = this.GetLanguageInfo("all", false);
			if (languageInfo.localized_field != null)
			{
				return languageInfo.localized_field;
			}
			string language = global::Defs.Language;
			global::Defs.TextInfo.LangInfo languageInfo2 = this.GetLanguageInfo(language, false);
			bool flag = false;
			if (!global::Defs.IsDevLanguage(language))
			{
				if (!global::Defs.TextInfo.IsEmpty(languageInfo2.localized_field))
				{
					return languageInfo2.localized_field;
				}
				this.ResolveDefField(vars, force_resolve_def_field);
				if (this.def_field != null && this.def_field.value.is_unknown && this.def_field.based_on != null)
				{
					global::Defs.TextInfo textInfo = global::Defs.GetTextInfo(this.def_field.based_on.Path(false, false, '.'), false, true);
					if (textInfo != null)
					{
						DT.Field textField = textInfo.GetTextField(null, false);
						if (textField != null)
						{
							return textField;
						}
					}
				}
				if (!global::Defs.TextInfo.IsEmpty(this.def_field))
				{
					if (this.def_field.Type() != "text")
					{
						return this.def_field;
					}
					string a = this.def_field.Path(false, false, '.');
					if (a != this.text_key)
					{
						global::Defs.TextInfo textInfo2 = global::Defs.GetTextInfo(a, false, true);
						if (textInfo2 == null)
						{
							return null;
						}
						return textInfo2.GetTextField(null, false);
					}
				}
				flag = true;
				languageInfo2 = this.GetLanguageInfo("dev", false);
			}
			DT.Field localized_field = languageInfo2.localized_field;
			if (localized_field == null)
			{
				this.ResolveDefField(vars, force_resolve_def_field);
				localized_field = this.def_field;
			}
			if (global::Defs.TextInfo.IsEmpty(localized_field))
			{
				return localized_field;
			}
			if (flag && global::Defs.enable_fallback_texts <= 0)
			{
				return null;
			}
			DT.Field field = localized_field;
			bool flag2 = false;
			if (global::Defs.enable_proofread_texts_in_dev > 0)
			{
				global::Defs.TextInfo.LangInfo languageInfo3 = this.GetLanguageInfo("en", false);
				if (!global::Defs.TextInfo.IsEmpty(languageInfo3.localized_field))
				{
					DT.Field cached_field = languageInfo2.cached_field ?? languageInfo3.cached_field;
					if (!global::Defs.TextInfo.TextModified(localized_field, cached_field))
					{
						field = languageInfo3.localized_field;
						if (global::Defs.enable_proofread_texts_in_dev == 1 && global::Defs.TextInfo.TextModified(field, cached_field))
						{
							flag2 = true;
						}
					}
					else if (global::Defs.enable_fallback_texts > 0 && global::Defs.TextInfo.TextModified(localized_field, languageInfo3.localized_field))
					{
						flag = true;
					}
				}
			}
			if (allow_fallbacks)
			{
				if (flag)
				{
					field = global::Defs.CreateFallbackTextField(field, global::Defs.enable_fallback_texts, "{NL}", "{/NL}");
				}
				else if (flag2)
				{
					field = global::Defs.CreateFallbackTextField(field, global::Defs.enable_proofread_texts_in_dev, "{PR}", "{/PR}");
				}
			}
			return field;
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x001FD034 File Offset: 0x001FB234
		public static bool IsEmpty(DT.Field f)
		{
			string a;
			return f == null || ((f.children == null || f.children.Count <= 0) && (!f.value.is_valid || ((a = (f.value.obj_val as string)) != null && a == "")));
		}

		// Token: 0x06004353 RID: 17235 RVA: 0x001FD094 File Offset: 0x001FB294
		public DT.Field ResolveDefField(IVars vars = null, bool force_resolve = false)
		{
			if (this.def_field != null && !force_resolve && !this.has_cases)
			{
				return this.def_field;
			}
			global::Defs defs = global::Defs.Get(false);
			DT.Field field;
			if (defs == null)
			{
				field = null;
			}
			else
			{
				DT dt = defs.dt;
				field = ((dt != null) ? dt.Find(this.text_key, vars) : null);
			}
			this.def_field = field;
			this.has_cases = DT.find_had_cases;
			return this.def_field;
		}

		// Token: 0x06004354 RID: 17236 RVA: 0x001FD0F8 File Offset: 0x001FB2F8
		private static Value ResolveInvalidOrSingleSubValue(Value value)
		{
			if (!value.is_valid)
			{
				return "";
			}
			List<DT.SubValue> list;
			if ((list = (value.obj_val as List<DT.SubValue>)) != null && list.Count == 1)
			{
				return list[0].value;
			}
			return value;
		}

		// Token: 0x06004355 RID: 17237 RVA: 0x001FD140 File Offset: 0x001FB340
		public static bool TextValueModified(Value text_value, Value cached_value)
		{
			text_value = global::Defs.TextInfo.ResolveInvalidOrSingleSubValue(text_value);
			cached_value = global::Defs.TextInfo.ResolveInvalidOrSingleSubValue(cached_value);
			if (text_value.type != cached_value.type)
			{
				return true;
			}
			if (text_value.is_string)
			{
				return text_value.String(null) != cached_value.String(null);
			}
			List<DT.SubValue> list = text_value.obj_val as List<DT.SubValue>;
			List<DT.SubValue> list2 = cached_value.obj_val as List<DT.SubValue>;
			if (list == null && list2 == null)
			{
				return text_value != cached_value;
			}
			if (list == null || list2 == null)
			{
				return true;
			}
			if (list.Count != list2.Count)
			{
				return true;
			}
			for (int i = 0; i < list.Count; i++)
			{
				DT.SubValue subValue = list[i];
				DT.SubValue subValue2 = list2[i];
				if (global::Defs.TextInfo.TextValueModified(subValue.value, subValue2.value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004356 RID: 17238 RVA: 0x001FD200 File Offset: 0x001FB400
		public static bool TextModified(DT.Field text_field, DT.Field cached_field)
		{
			if (cached_field == null)
			{
				return true;
			}
			if (global::Defs.TextInfo.TextValueModified(text_field.value, cached_field.value))
			{
				return true;
			}
			List<string> list = text_field.Keys(false, false);
			List<string> list2 = cached_field.Keys(false, false);
			if (list.Count != list2.Count)
			{
				return true;
			}
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				string text2 = list2[i];
				if (text != text2)
				{
					return true;
				}
				DT.Field text_field2 = text_field.FindChild(text, null, false, false, false, ' ');
				DT.Field cached_field2 = cached_field.FindChild(text2, null, false, false, false, ' ');
				if (global::Defs.TextInfo.TextModified(text_field2, cached_field2))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004357 RID: 17239 RVA: 0x001FD2A0 File Offset: 0x001FB4A0
		public static string Field2Str(DT.Field f, bool shorten = true)
		{
			if (f == null)
			{
				return "null";
			}
			string text = DT.Unquote(f.value_str);
			if (text == null)
			{
				text = "NULL";
			}
			else
			{
				if (shorten && text.Length > 20)
				{
					text = text.Substring(0, 17) + "...";
				}
				text = "'" + text + "'";
			}
			if (f.children != null && f.children.Count > 0)
			{
				text += string.Format(", forms: {0}", f.children.Count);
			}
			if (!shorten)
			{
				text += string.Format(" ({0}:{1})", f.FilePath(), f.line);
			}
			return text;
		}

		// Token: 0x06004358 RID: 17240 RVA: 0x001FD35C File Offset: 0x001FB55C
		public override string ToString()
		{
			global::Defs.TextInfo.<>c__DisplayClass21_0 CS$<>8__locals1;
			CS$<>8__locals1.s = (this.status ?? "");
			CS$<>8__locals1.tf = this.ResolveField(null, false, false);
			global::Defs.TextInfo.<ToString>g__AddField|21_0(this.def_field, "def", ref CS$<>8__locals1);
			if (this.languages != null)
			{
				for (int i = 0; i < this.languages.Count; i++)
				{
					global::Defs.TextInfo.LangInfo langInfo = this.languages[i];
					global::Defs.TextInfo.<ToString>g__AddField|21_0(langInfo.localized_field ?? langInfo.cached_field, langInfo.lang_key, ref CS$<>8__locals1);
				}
			}
			return string.Concat(new string[]
			{
				"[",
				CS$<>8__locals1.s,
				"] ",
				this.text_key,
				": ",
				global::Defs.TextInfo.Field2Str(CS$<>8__locals1.tf, true)
			});
		}

		// Token: 0x06004359 RID: 17241 RVA: 0x001FD430 File Offset: 0x001FB630
		public string Dump()
		{
			string text = this.ToString();
			text = text + "\n  [def] " + global::Defs.TextInfo.Field2Str(this.def_field, false);
			if (this.languages != null)
			{
				for (int i = 0; i < this.languages.Count; i++)
				{
					text = text + "\n  " + this.languages[i].Dump();
				}
			}
			return text;
		}

		// Token: 0x0600435C RID: 17244 RVA: 0x001FD49C File Offset: 0x001FB69C
		[CompilerGenerated]
		internal static void <ToString>g__AddField|21_0(DT.Field f, string lk, ref global::Defs.TextInfo.<>c__DisplayClass21_0 A_2)
		{
			if (f == null)
			{
				return;
			}
			if (f == A_2.tf)
			{
				lk = lk.ToUpperInvariant();
			}
			if (A_2.s != "")
			{
				A_2.s += ", ";
			}
			A_2.s += lk;
		}

		// Token: 0x04002F80 RID: 12160
		public string status;

		// Token: 0x04002F81 RID: 12161
		public string text_key;

		// Token: 0x04002F82 RID: 12162
		public DT.Field def_field;

		// Token: 0x04002F83 RID: 12163
		public List<global::Defs.TextInfo.LangInfo> languages;

		// Token: 0x04002F84 RID: 12164
		public DT.Field txt_field;

		// Token: 0x04002F85 RID: 12165
		public bool has_cases;

		// Token: 0x04002F86 RID: 12166
		private static string debug_resolve_field;

		// Token: 0x020009D4 RID: 2516
		public struct LangInfo
		{
			// Token: 0x17000725 RID: 1829
			// (get) Token: 0x060054E9 RID: 21737 RVA: 0x00247C00 File Offset: 0x00245E00
			public bool is_valid
			{
				get
				{
					return this.lang_key != null;
				}
			}

			// Token: 0x060054EA RID: 21738 RVA: 0x00247C0C File Offset: 0x00245E0C
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"[",
					this.lang_key,
					"] ",
					global::Defs.TextInfo.Field2Str(this.localized_field, true),
					" (",
					global::Defs.TextInfo.Field2Str(this.cached_field, true),
					")"
				});
			}

			// Token: 0x060054EB RID: 21739 RVA: 0x00247C6C File Offset: 0x00245E6C
			public string Dump()
			{
				string text = "[" + this.lang_key + "] " + global::Defs.TextInfo.Field2Str(this.localized_field, false);
				if (this.cached_field != null)
				{
					text = text + "\n    cached: " + global::Defs.TextInfo.Field2Str(this.cached_field, false);
				}
				return text;
			}

			// Token: 0x04004562 RID: 17762
			public string lang_key;

			// Token: 0x04004563 RID: 17763
			public DT.Field localized_field;

			// Token: 0x04004564 RID: 17764
			public DT.Field cached_field;
		}
	}

	// Token: 0x0200053A RID: 1338
	private struct RVStackItem
	{
		// Token: 0x04002F87 RID: 12167
		public string key;

		// Token: 0x04002F88 RID: 12168
		public string form;

		// Token: 0x04002F89 RID: 12169
		public DT.Field ctx_field;

		// Token: 0x04002F8A RID: 12170
		public IVars ctx_vars;
	}

	// Token: 0x0200053B RID: 1339
	private struct UnicodeRange
	{
		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x0600435D RID: 17245 RVA: 0x001FD4F8 File Offset: 0x001FB6F8
		public int start_idx
		{
			get
			{
				return (int)this.first_char;
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x0600435E RID: 17246 RVA: 0x001FD500 File Offset: 0x001FB700
		public int end_idx
		{
			get
			{
				return (int)this.last_char;
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x0600435F RID: 17247 RVA: 0x001FD508 File Offset: 0x001FB708
		public int length
		{
			get
			{
				return this.end_idx - this.start_idx + 1;
			}
		}

		// Token: 0x06004360 RID: 17248 RVA: 0x001FD519 File Offset: 0x001FB719
		public UnicodeRange(int start_idx, int end_idx)
		{
			this.first_char = (char)start_idx;
			this.last_char = (char)end_idx;
		}

		// Token: 0x06004361 RID: 17249 RVA: 0x001FD52B File Offset: 0x001FB72B
		public UnicodeRange(char start, char end)
		{
			this.first_char = start;
			this.last_char = end;
		}

		// Token: 0x06004362 RID: 17250 RVA: 0x001FD53C File Offset: 0x001FB73C
		public override string ToString()
		{
			if (this.end_idx != this.start_idx)
			{
				return string.Format("{0:04X}-{1:04X} ({2})", this.start_idx, this.end_idx, this.length);
			}
			return string.Format("{0:04X}", this.start_idx);
		}

		// Token: 0x04002F8B RID: 12171
		public char first_char;

		// Token: 0x04002F8C RID: 12172
		public char last_char;
	}

	// Token: 0x0200053C RID: 1340
	public class WordStats
	{
		// Token: 0x06004363 RID: 17251 RVA: 0x001FD598 File Offset: 0x001FB798
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				string.Format("fields: {0}\n", this.num_fields),
				string.Format("value variants: {0}\n", this.num_value_variants),
				string.Format("forms: {0}\n", this.num_forms),
				string.Format("form variants: {0}\n", this.num_form_variants),
				string.Format("words: {0}\n", this.num_words),
				string.Format("vars: {0}\n", this.num_vars),
				string.Format("tags: {0}\n", this.num_tags),
				string.Format("symbols: {0}\n", this.num_symbols),
				string.Format("wiki articles: {0}\n", this.num_wiki_articles),
				string.Format("wiki tags: {0}\n", this.num_wiki_tags),
				string.Format("wiki comments: {0}\n", this.num_wiki_comments),
				string.Format("wiki keywords: {0}\n", this.num_wiki_keywords),
				string.Format("wiki words: {0}\n", this.num_wiki_words)
			});
		}

		// Token: 0x04002F8D RID: 12173
		public int num_fields;

		// Token: 0x04002F8E RID: 12174
		public int num_value_variants;

		// Token: 0x04002F8F RID: 12175
		public int num_forms;

		// Token: 0x04002F90 RID: 12176
		public int num_form_variants;

		// Token: 0x04002F91 RID: 12177
		public int num_words;

		// Token: 0x04002F92 RID: 12178
		public int num_vars;

		// Token: 0x04002F93 RID: 12179
		public int num_tags;

		// Token: 0x04002F94 RID: 12180
		public int num_symbols;

		// Token: 0x04002F95 RID: 12181
		public int num_wiki_articles;

		// Token: 0x04002F96 RID: 12182
		public int num_wiki_tags;

		// Token: 0x04002F97 RID: 12183
		public int num_wiki_comments;

		// Token: 0x04002F98 RID: 12184
		public int num_wiki_keywords;

		// Token: 0x04002F99 RID: 12185
		public int num_wiki_words;
	}
}
