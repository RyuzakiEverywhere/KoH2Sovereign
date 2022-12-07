using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Logic;
using UnityEngine;

// Token: 0x02000086 RID: 134
internal class PersistentLogs
{
	// Token: 0x0600051A RID: 1306 RVA: 0x0003A9E0 File Offset: 0x00038BE0
	public static void RemoveLoadedLogs()
	{
		try
		{
			string path = Path.Combine(PersistentLogs.GetSystemLogsDir(null), "llgs");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		PersistentLogs.stored_load_logs = false;
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0003AA2C File Offset: 0x00038C2C
	public static void StoreLoadLogs(string current_load_dir)
	{
		string text = Path.Combine(PersistentLogs.GetSystemLogsDir(null), "llgs");
		string path = Path.Combine(current_load_dir, "lgs");
		if (!Directory.Exists(path))
		{
			return;
		}
		if (Directory.Exists(text))
		{
			Directory.Delete(text, true);
		}
		Directory.CreateDirectory(text);
		foreach (FileInfo fileInfo in new DirectoryInfo(path).GetFiles())
		{
			File.Copy(fileInfo.FullName, Path.Combine(text, fileInfo.Name));
		}
		PersistentLogs.stored_load_logs = true;
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0003AAB4 File Offset: 0x00038CB4
	protected static void SaveLoadedLogs(string logs_save_dir, PersistentLogs.ThreadParams thread_params = null)
	{
		if (!PersistentLogs.stored_load_logs)
		{
			return;
		}
		string path = Path.Combine(PersistentLogs.GetSystemLogsDir(thread_params), "llgs");
		if (!Directory.Exists(path))
		{
			return;
		}
		foreach (FileInfo fileInfo in new DirectoryInfo(path).GetFiles())
		{
			File.Copy(fileInfo.FullName, Path.Combine(logs_save_dir, fileInfo.Name));
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0003AB18 File Offset: 0x00038D18
	protected static string GetSystemLogsDir(PersistentLogs.ThreadParams thread_params = null)
	{
		string text = ((thread_params != null) ? thread_params.companyName : null) ?? Application.companyName;
		string text2 = ((thread_params != null) ? thread_params.productName : null) ?? Application.productName;
		return Path.Combine(new string[]
		{
			Environment.GetEnvironmentVariable("AppData"),
			"..",
			"LocalLow",
			text,
			text2
		});
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0003AB84 File Offset: 0x00038D84
	protected static void Save(PersistentLogs.ThreadParams thread_params)
	{
		string text = Path.Combine(thread_params.save_path, "lgs");
		string text2 = Path.Combine(text, "tmp");
		string path = "Player.log";
		string text3 = Path.Combine(PersistentLogs.GetSystemLogsDir(thread_params), path);
		string text4 = Path.Combine(Game.GetSavesRootDir(Game.SavesRoot.Root), AudioLog.fileName);
		if (Directory.Exists(text))
		{
			Directory.Delete(text, true);
		}
		Directory.CreateDirectory(text2);
		if (File.Exists(text3))
		{
			File.Copy(text3, Path.Combine(text2, path));
		}
		if (File.Exists(text4))
		{
			File.Copy(text4, Path.Combine(text2, AudioLog.fileName));
		}
		ZipFile.CreateFromDirectory(text2, Path.Combine(text, "lg-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
		Directory.Delete(text2, true);
		PersistentLogs.SaveLoadedLogs(text, thread_params);
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0003AC50 File Offset: 0x00038E50
	private static void SaveThread(object thread_params)
	{
		try
		{
			PersistentLogs.Save(thread_params as PersistentLogs.ThreadParams);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		PersistentLogs.save_thread = null;
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0003AC8C File Offset: 0x00038E8C
	public static void Save(string save_path)
	{
		if (PersistentLogs.save_thread != null)
		{
			return;
		}
		PersistentLogs.save_thread = new Thread(new ParameterizedThreadStart(PersistentLogs.SaveThread));
		PersistentLogs.save_thread.Name = "Save Player Logs thread";
		PersistentLogs.ThreadParams threadParams = new PersistentLogs.ThreadParams();
		threadParams.save_path = save_path;
		threadParams.companyName = Application.companyName;
		threadParams.productName = Application.productName;
		PersistentLogs.save_thread.Start(threadParams);
	}

	// Token: 0x040004D1 RID: 1233
	public const string log_name = "lg-";

	// Token: 0x040004D2 RID: 1234
	private const string logs_dir_name = "lgs";

	// Token: 0x040004D3 RID: 1235
	private const string loaded_logs_dir_name = "llgs";

	// Token: 0x040004D4 RID: 1236
	public static Thread save_thread;

	// Token: 0x040004D5 RID: 1237
	public static bool stored_load_logs;

	// Token: 0x02000557 RID: 1367
	protected class ThreadParams
	{
		// Token: 0x0400300A RID: 12298
		public string save_path;

		// Token: 0x0400300B RID: 12299
		public string companyName;

		// Token: 0x0400300C RID: 12300
		public string productName;
	}
}
