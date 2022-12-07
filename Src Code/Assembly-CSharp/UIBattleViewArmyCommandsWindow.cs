using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class UIBattleViewArmyCommandsWindow : UIWindow
{
	// Token: 0x06001AA7 RID: 6823 RVA: 0x001021F9 File Offset: 0x001003F9
	public override void Show()
	{
		base.Show();
		this.Initialize();
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x00102207 File Offset: 0x00100407
	public override void Hide(bool silent = false)
	{
		this.RemoveListeners();
		base.Hide(silent);
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x00102216 File Offset: 0x00100416
	public override string GetDefId()
	{
		return UIBattleViewArmyCommandsWindow.def_id;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0010221D File Offset: 0x0010041D
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIBattleViewArmyCommandsWindow.def_id, null);
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x0010222A File Offset: 0x0010042A
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		this.AddListeners();
		this.LoadSettings();
		this.LoadArmyCommands();
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x00102245 File Offset: 0x00100445
	private void AddListeners()
	{
		if (this.m_Close != null)
		{
			BSGButton close = this.m_Close;
			close.onClick = (BSGButton.OnClick)Delegate.Combine(close.onClick, new BSGButton.OnClick(this.HandleOnClose));
		}
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x0010227C File Offset: 0x0010047C
	private void RemoveListeners()
	{
		if (this.m_Close != null)
		{
			BSGButton close = this.m_Close;
			close.onClick = (BSGButton.OnClick)Delegate.Remove(close.onClick, new BSGButton.OnClick(this.HandleOnClose));
		}
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x001022B4 File Offset: 0x001004B4
	private void LoadSettings()
	{
		this.m_mouseButton0 = global::Defs.GetObj<Sprite>(this.window_def, "mouse_button_0", null);
		this.m_mouseButton1 = global::Defs.GetObj<Sprite>(this.window_def, "mouse_button_1", null);
		this.m_mouseButton2 = global::Defs.GetObj<Sprite>(this.window_def, "mouse_button_2", null);
		this.m_keyboardKey = global::Defs.GetObj<Sprite>(this.window_def, "keyboard_key", null);
		this.m_commandBindingPrefab = global::Defs.GetObj<GameObject>(this.window_def, "command_binding_prefab", null);
		UIText.SetText(this.m_Caption, global::Defs.Localize(this.window_def, "title", null, null, true, true));
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x00102354 File Offset: 0x00100554
	private void LoadArmyCommands()
	{
		UICommon.DeleteChildren(this.m_Container);
		DT.Field field = this.window_def.FindChild("Commands.Hardcoded", null, true, true, true, '.');
		this.GenerateKeybindings(field.Children(), true);
		DT.Field field2 = this.window_def.FindChild("Commands.Assignable", null, true, true, true, '.');
		this.GenerateKeybindings(field2.Children(), false);
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x001023B4 File Offset: 0x001005B4
	private void GenerateKeybindings(List<DT.Field> commands, bool areCommandsHardcoded)
	{
		for (int i = 0; i < commands.Count; i++)
		{
			if (!string.IsNullOrEmpty(commands[i].key))
			{
				string @string = commands[i].GetString("name_of_keybind", null, "", true, true, true, '.');
				Vars vars = new Vars();
				if (areCommandsHardcoded)
				{
					string[] array = @string.Split(new char[]
					{
						KeyBindings.modifier_separator
					});
					if (array == null || array.Length == 0)
					{
						goto IL_1DB;
					}
					for (int j = 0; j < array.Length; j++)
					{
						int num = j + 1;
						vars.Set<Sprite>(string.Format("background_{0}", num), this.GetSpriteForKey(array[j]));
						string val = array[j].Contains("Mouse") ? string.Empty : global::Defs.GetLocalized(array[j], null);
						vars.Set<string>(string.Format("key_text_{0}", num), val);
					}
				}
				else
				{
					KeyBindings.KeyBind bindData = KeyBindings.GetBindData(@string, 0);
					string keycode = bindData.modifier.ToString();
					string keycode2 = bindData.key.ToString();
					if (bindData.modifier != KeyCode.None)
					{
						vars.Set<Sprite>("background_2", this.GetSpriteForKey(keycode));
						vars.Set<string>("key_text_2", bindData.LocalizeModifier());
					}
					if (bindData.key != KeyCode.None)
					{
						vars.Set<Sprite>("background_1", this.GetSpriteForKey(keycode2));
						vars.Set<string>("key_text_1", bindData.LocalizeKey());
					}
				}
				vars.Set<string>("command_text", global::Defs.Localize(commands[i], "command_text", null, null, true, true));
				vars.Set<string>("tooltip_localzied", "#" + global::Defs.Localize(commands[i], "tooltip", null, null, true, true));
				global::Common.Spawn(this.m_commandBindingPrefab, this.m_Container, false, "").GetComponent<UIBattleViewCommandBinding>().Set(vars);
			}
			IL_1DB:;
		}
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x001025AC File Offset: 0x001007AC
	private Sprite GetSpriteForKey(string keycode)
	{
		if (keycode == "Mouse0")
		{
			return this.m_mouseButton0;
		}
		if (keycode == "Mouse1")
		{
			return this.m_mouseButton1;
		}
		if (!(keycode == "Mouse2"))
		{
			return this.m_keyboardKey;
		}
		return this.m_mouseButton2;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x001025FD File Offset: 0x001007FD
	private void HandleOnClose(BSGButton b)
	{
		this.Close(false);
		this.on_close = null;
		if (this.m_Close)
		{
			this.m_Close.onClick = null;
		}
	}

	// Token: 0x04001155 RID: 4437
	private static string def_id = "UIBattleViewArmyCommandsWindow";

	// Token: 0x04001156 RID: 4438
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001157 RID: 4439
	[UIFieldTarget("id_Container")]
	private RectTransform m_Container;

	// Token: 0x04001158 RID: 4440
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001159 RID: 4441
	private Sprite m_mouseButton0;

	// Token: 0x0400115A RID: 4442
	private Sprite m_mouseButton1;

	// Token: 0x0400115B RID: 4443
	private Sprite m_mouseButton2;

	// Token: 0x0400115C RID: 4444
	private Sprite m_keyboardKey;

	// Token: 0x0400115D RID: 4445
	private GameObject m_commandBindingPrefab;
}
