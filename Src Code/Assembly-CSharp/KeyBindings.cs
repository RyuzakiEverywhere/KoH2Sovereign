using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200017A RID: 378
public static class KeyBindings
{
	// Token: 0x0600142B RID: 5163 RVA: 0x000CAA24 File Offset: 0x000C8C24
	public static List<KeyBindings.KeyBind> GetBindsData(string actionName)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return null;
		}
		List<KeyBindings.KeyBind> result;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x000CAA50 File Offset: 0x000C8C50
	public static KeyBindings.KeyBind GetBindData(string actionName, int idx = 0)
	{
		if (idx < 0 || string.IsNullOrEmpty(actionName))
		{
			return default(KeyBindings.KeyBind);
		}
		List<KeyBindings.KeyBind> list;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out list) || list == null || idx >= list.Count)
		{
			return default(KeyBindings.KeyBind);
		}
		return list[idx];
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x000CAAA0 File Offset: 0x000C8CA0
	public static void ChangeKeybind(string actionName, KeyCode key, KeyCode modifier = KeyCode.None, int idx = 0)
	{
		KeyBindings.ChangeKeybind(actionName, new KeyBindings.KeyBind
		{
			key = key,
			modifier = modifier
		}, idx);
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x000CAAD0 File Offset: 0x000C8CD0
	public static void ChangeKeybind(string actionName, KeyBindings.KeyBind keybind, int idx = 0)
	{
		if (KeyBindings.IsKeyForbidden(keybind.key) || string.IsNullOrEmpty(actionName))
		{
			return;
		}
		List<KeyBindings.KeyBind> list;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out list) || list == null)
		{
			list = new List<KeyBindings.KeyBind>();
			KeyBindings.keybindings[actionName] = list;
		}
		if (idx > list.Count - 1)
		{
			if (idx > list.Count)
			{
				Debug.LogError(string.Format("Trying to set keybind with idx {0} when existing binds are up to index {1}", idx, list.Count - 1));
			}
			list.Add(keybind);
			return;
		}
		list[idx] = keybind;
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x000CAB60 File Offset: 0x000C8D60
	public static void ChangeKeybinds(string actionName, string parseStr)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return;
		}
		List<KeyBindings.KeyBind> binds;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out binds))
		{
			binds = new List<KeyBindings.KeyBind>();
		}
		if (KeyBindings.TryParse(parseStr, binds))
		{
			KeyBindings.ChangeKeybinds(actionName, binds);
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x000CAB9B File Offset: 0x000C8D9B
	public static void ChangeKeybinds(string actionName, List<KeyBindings.KeyBind> binds)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return;
		}
		KeyBindings.keybindings[actionName] = binds;
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x000CABB2 File Offset: 0x000C8DB2
	public static bool IsKeyModifier(KeyCode key)
	{
		return KeyBindings.modifiers.Contains(key);
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x000CABBF File Offset: 0x000C8DBF
	public static bool IsKeyForbidden(KeyCode key)
	{
		return KeyBindings.forbiddenKeyCodes.Contains(key);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x000CABCC File Offset: 0x000C8DCC
	public static HashSet<KeyCode> GetModifiers()
	{
		return KeyBindings.modifiers;
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x000CABD3 File Offset: 0x000C8DD3
	private static UICommon.ModifierKey ConvertModifier(KeyCode modifier)
	{
		switch (modifier)
		{
		case KeyCode.RightShift:
			return UICommon.ModifierKey.RightShift;
		case KeyCode.LeftShift:
			return UICommon.ModifierKey.LeftShift;
		case KeyCode.RightControl:
			return UICommon.ModifierKey.RightCtrl;
		case KeyCode.LeftControl:
			return UICommon.ModifierKey.LeftCtrl;
		case KeyCode.RightAlt:
			return UICommon.ModifierKey.RightAlt;
		case KeyCode.LeftAlt:
			return UICommon.ModifierKey.LeftAlt;
		default:
			return UICommon.ModifierKey.None;
		}
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x000CAC14 File Offset: 0x000C8E14
	public static bool GetBindDown(string actionName)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return false;
		}
		List<KeyBindings.KeyBind> list;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out list))
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (KeyBindings.GetBindDown(list[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x000CAC60 File Offset: 0x000C8E60
	public static bool GetBindDown(string actionName, int idx)
	{
		List<KeyBindings.KeyBind> list;
		return !string.IsNullOrEmpty(actionName) && idx >= 0 && KeyBindings.keybindings.TryGetValue(actionName, out list) && list != null && idx < list.Count && KeyBindings.GetBindDown(list[idx]);
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x000CACA4 File Offset: 0x000C8EA4
	private static bool GetBindDown(KeyBindings.KeyBind bind)
	{
		UICommon.ModifierKey modifierKey = KeyBindings.ConvertModifier(bind.modifier);
		return UICommon.GetKeyDown(bind.key, modifierKey, modifierKey);
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x000CACCC File Offset: 0x000C8ECC
	public static bool GetBindUp(string actionName)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return false;
		}
		List<KeyBindings.KeyBind> list;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out list))
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (KeyBindings.GetBindUp(list[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x000CAD18 File Offset: 0x000C8F18
	public static bool GetBindUp(string actionName, int idx)
	{
		List<KeyBindings.KeyBind> list;
		return !string.IsNullOrEmpty(actionName) && idx >= 0 && KeyBindings.keybindings.TryGetValue(actionName, out list) && list != null && idx < list.Count && KeyBindings.GetBindUp(list[idx]);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x000CAD5C File Offset: 0x000C8F5C
	private static bool GetBindUp(KeyBindings.KeyBind bind)
	{
		UICommon.ModifierKey modifierKey = KeyBindings.ConvertModifier(bind.modifier);
		return UICommon.GetKeyUp(bind.key, modifierKey, modifierKey);
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x000CAD84 File Offset: 0x000C8F84
	public static bool GetBind(string actionName, bool ignoreInputFocused = false)
	{
		if (string.IsNullOrEmpty(actionName))
		{
			return false;
		}
		List<KeyBindings.KeyBind> list;
		if (!KeyBindings.keybindings.TryGetValue(actionName, out list) || list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (KeyBindings.GetBind(list[i], ignoreInputFocused))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x000CADD4 File Offset: 0x000C8FD4
	public static bool GetBind(string actionName, int idx, bool ignoreInputFocused = false)
	{
		List<KeyBindings.KeyBind> list;
		return !string.IsNullOrEmpty(actionName) && idx >= 0 && KeyBindings.keybindings.TryGetValue(actionName, out list) && list != null && idx < list.Count && KeyBindings.GetBind(list[idx], ignoreInputFocused);
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x000CAE1C File Offset: 0x000C901C
	private static bool GetBind(KeyBindings.KeyBind bind, bool ignoreInputFocused = false)
	{
		if (!UICommon.GetKey(bind.key, ignoreInputFocused))
		{
			return false;
		}
		UICommon.ModifierKey modifierKey = KeyBindings.ConvertModifier(bind.modifier);
		if (KeyBindings.IsKeyModifier(bind.key))
		{
			modifierKey |= KeyBindings.ConvertModifier(bind.key);
		}
		return UICommon.CheckModifierKeys(modifierKey, modifierKey);
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000CAE6C File Offset: 0x000C906C
	public static bool TryParse(string parseStr, List<KeyBindings.KeyBind> binds)
	{
		if (parseStr == null)
		{
			return false;
		}
		if (binds != null)
		{
			binds.Clear();
		}
		string[] array = parseStr.Split(new char[]
		{
			KeyBindings.bind_separator
		});
		for (int i = 0; i < array.Length; i++)
		{
			KeyBindings.KeyBind item;
			if (KeyBindings.TryParseSingleBind(array[i], out item))
			{
				if (binds == null)
				{
					binds = new List<KeyBindings.KeyBind>();
				}
				binds.Add(item);
			}
		}
		return binds != null && binds.Count > 0;
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x000CAED8 File Offset: 0x000C90D8
	public static bool TryParseSingleBind(string parseStr, out KeyBindings.KeyBind keybind)
	{
		keybind = default(KeyBindings.KeyBind);
		if (parseStr == null)
		{
			return false;
		}
		string[] array = parseStr.Split(new char[]
		{
			KeyBindings.modifier_separator
		});
		if (array.Length == 1)
		{
			if (!Enum.TryParse<KeyCode>(array[0], out keybind.key))
			{
				return false;
			}
			if (KeyBindings.IsKeyForbidden(keybind.key))
			{
				return false;
			}
		}
		else
		{
			if (array.Length != 2)
			{
				return false;
			}
			if (!Enum.TryParse<KeyCode>(array[1], out keybind.key))
			{
				return false;
			}
			if (KeyBindings.IsKeyForbidden(keybind.key))
			{
				return false;
			}
			if (!Enum.TryParse<KeyCode>(array[0], out keybind.modifier))
			{
				return false;
			}
			if (!KeyBindings.IsKeyModifier(keybind.modifier))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x000CAF78 File Offset: 0x000C9178
	public static string ParseToStr(params KeyBindings.KeyBind[] binds)
	{
		string text = "";
		for (int i = 0; i < binds.Length; i++)
		{
			if (i > 0)
			{
				text += KeyBindings.bind_separator.ToString();
			}
			text += binds[i].ToString();
		}
		return text;
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x000CAFC8 File Offset: 0x000C91C8
	public static string LocalizeKeybind(string acitonName, int idx = 0, bool addHash = true)
	{
		if (idx < 0)
		{
			return null;
		}
		List<KeyBindings.KeyBind> bindsData = KeyBindings.GetBindsData(acitonName);
		if (bindsData == null || idx >= bindsData.Count)
		{
			return null;
		}
		string text = bindsData[idx].Localize();
		if (!addHash)
		{
			return text;
		}
		return "#" + text;
	}

	// Token: 0x04000D0F RID: 3343
	private static Dictionary<string, List<KeyBindings.KeyBind>> keybindings = new Dictionary<string, List<KeyBindings.KeyBind>>();

	// Token: 0x04000D10 RID: 3344
	private static HashSet<KeyCode> forbiddenKeyCodes = new HashSet<KeyCode>
	{
		KeyCode.Escape,
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.F5,
		KeyCode.F9,
		KeyCode.F1
	};

	// Token: 0x04000D11 RID: 3345
	private static HashSet<KeyCode> modifiers = new HashSet<KeyCode>
	{
		KeyCode.LeftControl,
		KeyCode.RightControl,
		KeyCode.LeftShift,
		KeyCode.RightShift,
		KeyCode.LeftAlt,
		KeyCode.RightAlt
	};

	// Token: 0x04000D12 RID: 3346
	public static char modifier_separator = '-';

	// Token: 0x04000D13 RID: 3347
	public static char bind_separator = ';';

	// Token: 0x020006A8 RID: 1704
	public struct KeyBind
	{
		// Token: 0x06004851 RID: 18513 RVA: 0x0021757C File Offset: 0x0021577C
		public override string ToString()
		{
			if (this.modifier == KeyCode.None)
			{
				return this.key.ToString();
			}
			return this.modifier.ToString() + KeyBindings.modifier_separator.ToString() + this.key.ToString();
		}

		// Token: 0x06004852 RID: 18514 RVA: 0x002175D4 File Offset: 0x002157D4
		public string Localize()
		{
			string text = this.LocalizeKey();
			if (this.modifier == KeyCode.None)
			{
				return text;
			}
			string str = this.LocalizeModifier();
			Vars vars = new Vars();
			vars.Set<string>("modifier", "#" + str);
			vars.Set<string>("key", "#" + text);
			return global::Defs.Localize("Preferences.UnityKeyCodes.combined", vars, null, true, true);
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x00217639 File Offset: 0x00215839
		public string LocalizeKey()
		{
			return global::Defs.Localize("Preferences.UnityKeyCodes." + this.key.ToString(), null, null, true, true);
		}

		// Token: 0x06004854 RID: 18516 RVA: 0x0021765F File Offset: 0x0021585F
		public string LocalizeModifier()
		{
			return global::Defs.Localize("Preferences.UnityKeyCodes." + this.modifier.ToString(), null, null, true, true);
		}

		// Token: 0x04003645 RID: 13893
		public KeyCode key;

		// Token: 0x04003646 RID: 13894
		public KeyCode modifier;
	}

	// Token: 0x020006A9 RID: 1705
	public class DefsLocalizer : IVars
	{
		// Token: 0x06004855 RID: 18517 RVA: 0x00217685 File Offset: 0x00215885
		public static KeyBindings.DefsLocalizer Get()
		{
			if (KeyBindings.DefsLocalizer.instance == null)
			{
				KeyBindings.DefsLocalizer.instance = new KeyBindings.DefsLocalizer();
			}
			return KeyBindings.DefsLocalizer.instance;
		}

		// Token: 0x06004856 RID: 18518 RVA: 0x0021769D File Offset: 0x0021589D
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			return KeyBindings.LocalizeKeybind(key, 0, true);
		}

		// Token: 0x04003647 RID: 13895
		public static KeyBindings.DefsLocalizer instance;
	}
}
