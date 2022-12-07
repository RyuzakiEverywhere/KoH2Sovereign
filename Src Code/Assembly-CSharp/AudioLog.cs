using System;
using System.IO;
using Logic;

// Token: 0x02000069 RID: 105
public static class AudioLog
{
	// Token: 0x060002B8 RID: 696 RVA: 0x0002589B File Offset: 0x00023A9B
	private static string GetFilePath()
	{
		return Path.Combine(Game.GetSavesRootDir(Game.SavesRoot.Root), AudioLog.fileName);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x000258AD File Offset: 0x00023AAD
	public static void Clear()
	{
		File.Delete(AudioLog.GetFilePath());
		AudioLog.cleared = true;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x000258C0 File Offset: 0x00023AC0
	private static void LogImpl(string msg, AudioLog.LogLevel logLevel)
	{
		if (!AudioLog.cleared)
		{
			AudioLog.Clear();
		}
		Game game = GameLogic.Get(false);
		DevSettings.Def def = (game != null) ? game.GetDevSettingsDef() : null;
		if (def != null && def.audio_log_level < (int)logLevel)
		{
			return;
		}
		try
		{
			File.AppendAllText(AudioLog.GetFilePath(), string.Concat(new string[]
			{
				logLevel.ToString(),
				", ",
				DateTime.Now.ToString("HH:mm:ss.fff:"),
				"\r\n",
				msg,
				"\r\n\r\n"
			}));
			if (AudioLog.printInMainLog)
			{
				Game.LogType type = Game.LogType.Error;
				switch (logLevel)
				{
				case AudioLog.LogLevel.Nothing:
					return;
				case AudioLog.LogLevel.Error:
					type = Game.LogType.Error;
					break;
				case AudioLog.LogLevel.Warning:
					type = Game.LogType.Warning;
					break;
				case AudioLog.LogLevel.Info:
					type = Game.LogType.Message;
					break;
				}
				Game.Log(string.Format("AudioLog.{0}:\n{1}", logLevel, msg), type);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060002BB RID: 699 RVA: 0x000259AC File Offset: 0x00023BAC
	public static void Error(string msg)
	{
		AudioLog.LogImpl(msg, AudioLog.LogLevel.Error);
	}

	// Token: 0x060002BC RID: 700 RVA: 0x000259B5 File Offset: 0x00023BB5
	public static void Warning(string msg)
	{
		AudioLog.LogImpl(msg, AudioLog.LogLevel.Warning);
	}

	// Token: 0x060002BD RID: 701 RVA: 0x000259BE File Offset: 0x00023BBE
	public static void Info(string msg)
	{
		AudioLog.LogImpl(msg, AudioLog.LogLevel.Info);
	}

	// Token: 0x040003DF RID: 991
	public static string fileName = "audio.log";

	// Token: 0x040003E0 RID: 992
	public static bool printInMainLog = false;

	// Token: 0x040003E1 RID: 993
	private static bool cleared = false;

	// Token: 0x02000526 RID: 1318
	private enum LogLevel
	{
		// Token: 0x04002F1B RID: 12059
		Nothing,
		// Token: 0x04002F1C RID: 12060
		Error,
		// Token: 0x04002F1D RID: 12061
		Warning,
		// Token: 0x04002F1E RID: 12062
		Info
	}
}
