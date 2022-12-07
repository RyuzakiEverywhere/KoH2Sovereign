using System;
using System.Collections.Generic;
using System.IO;
using Logic;

// Token: 0x020002F8 RID: 760
public class WorldRadio
{
	// Token: 0x06002FD2 RID: 12242 RVA: 0x00186618 File Offset: 0x00184818
	public static void AddMessage(DT.Field def_field, Vars vars, int max_messages = 2147483647)
	{
		if (def_field == null)
		{
			return;
		}
		string text = vars.Get<string>("_localized_radio", null) + "\n";
		int num = (int)(GameLogic.Get(true).session_time.milliseconds / 1000L);
		int num2 = num % 60;
		int num3 = num / 60;
		int num4 = num3 % 60;
		string timestamp = string.Concat(new string[]
		{
			(num3 / 60).ToString(),
			":",
			num4.ToString("D2"),
			":",
			num2.ToString("D2")
		});
		string @string = def_field.GetString("category", null, "", true, true, true, '.');
		WorldRadio.Log item = new WorldRadio.Log
		{
			timestamp = timestamp,
			category = @string,
			text = text
		};
		WorldRadio.logs.Add(item);
		if (WorldRadio.logs.Count > max_messages)
		{
			WorldRadio.logs.RemoveAt(0);
		}
		WorldRadio.last = item;
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00186717 File Offset: 0x00184917
	public static void Clear()
	{
		WorldRadio.logs.Clear();
		WorldRadio.last = default(WorldRadio.Log);
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x00186730 File Offset: 0x00184930
	public static void Save(string path)
	{
		Game game = GameLogic.Get(false);
		if (WorldRadio.logs == null || game == null)
		{
			return;
		}
		string text = "";
		for (int i = 0; i < WorldRadio.logs.Count; i++)
		{
			if (!string.IsNullOrEmpty(WorldRadio.logs[i].text))
			{
				string text2 = ChatLinksUtils.ConvertLinksToMarkup(game, WorldRadio.logs[i].text);
				if (!string.IsNullOrEmpty(text2))
				{
					text = string.Concat(new string[]
					{
						text,
						"[",
						WorldRadio.logs[i].timestamp,
						"-",
						WorldRadio.logs[i].category,
						"]"
					});
					text += text2;
				}
			}
		}
		try
		{
			File.WriteAllText(path, text);
		}
		catch (Exception ex)
		{
			Game.Log(ex.ToString(), Game.LogType.Error);
		}
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x00186828 File Offset: 0x00184A28
	public static void Load(string path)
	{
		Game game = GameLogic.Get(false);
		if (game == null || game.IsMultiplayer() || !File.Exists(path))
		{
			return;
		}
		string text = "";
		try
		{
			text = File.ReadAllText(path);
		}
		catch (Exception ex)
		{
			Game.Log(ex.ToString(), Game.LogType.Error);
			return;
		}
		WorldRadio.logs.Clear();
		foreach (string text2 in text.Split(new char[]
		{
			'\n'
		}))
		{
			int num = text2.IndexOf(']');
			if (num != -1)
			{
				string[] array2 = text2.Substring(1, num - 1).Split(new char[]
				{
					'-'
				});
				string timestamp = array2[0];
				string category = "common";
				if (array2.Length > 1)
				{
					category = array2[1];
				}
				string text3 = text2.Substring(num + 1);
				if (!string.IsNullOrEmpty(text3))
				{
					string text4 = UIInGameChat.ConvertMarkupToLinks(game, text3).Replace(ChatLinksUtils.NoParseOpeningTag, "").Replace(ChatLinksUtils.NoParseClosingTag, "") + "\n";
					WorldRadio.logs.Add(new WorldRadio.Log
					{
						timestamp = timestamp,
						category = category,
						text = text4
					});
				}
			}
		}
		if (WorldRadio.logs.Count > 0)
		{
			WorldRadio.last = WorldRadio.logs[WorldRadio.logs.Count - 1];
		}
		WorldUI worldUI = WorldUI.Get();
		UILogger uilogger = (worldUI != null) ? worldUI.GetEventLogger() : null;
		if (uilogger != null)
		{
			uilogger.RecreateAllMessages();
			DT.Field defField = global::Defs.GetDefField("GameLoadedMessage", null);
			Vars vars = new Vars();
			vars.Set<string>("_localized_radio", global::Defs.Localize(defField, "radio", null, null, true, true));
			uilogger.AddMessage(defField, vars);
		}
	}

	// Token: 0x0400202E RID: 8238
	public static List<WorldRadio.Log> logs = new List<WorldRadio.Log>();

	// Token: 0x0400202F RID: 8239
	public static WorldRadio.Log last;

	// Token: 0x02000868 RID: 2152
	public struct Log
	{
		// Token: 0x04003F1A RID: 16154
		public string timestamp;

		// Token: 0x04003F1B RID: 16155
		public string category;

		// Token: 0x04003F1C RID: 16156
		public string text;
	}
}
