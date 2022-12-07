using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Logic;
using UnityEngine;

// Token: 0x0200029C RID: 668
internal class GameCapture : MonoBehaviour
{
	// Token: 0x06002938 RID: 10552 RVA: 0x0015EB38 File Offset: 0x0015CD38
	protected IEnumerator TakeScreeenShotAndCapture()
	{
		yield return this.CaptureScreenShot();
		GameCapture.CaptureGame(this.captureParams);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x0015EB47 File Offset: 0x0015CD47
	public IEnumerator CaptureScreenShot()
	{
		if (this.captureParams == null)
		{
			this.captureParams = new GameCapture.CaptureParams();
		}
		yield return new WaitForEndOfFrame();
		this.captureParams.screenShot = ScreenCapture.CaptureScreenshotAsTexture();
		yield break;
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x0015EB58 File Offset: 0x0015CD58
	public static string CaptureDefaultDirectory()
	{
		string text = PlayerPrefs.GetString("BSG_BugReport_DirDestination");
		if (!Directory.Exists(text))
		{
			text = Path.Combine(Game.GetSavesRootDir(Game.SavesRoot.Root), "BugReports");
		}
		return text;
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x0015EB8C File Offset: 0x0015CD8C
	public static void CopyDirectory(string source, string target)
	{
		try
		{
			if (Directory.Exists(source))
			{
				if (!Directory.Exists(target))
				{
					Directory.CreateDirectory(target);
				}
				GameCapture.CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(target));
			}
		}
		catch (Exception ex)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Copy Directory error [",
				source,
				"]-[",
				target,
				"]: ",
				ex.ToString()
			}));
		}
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x0015EC14 File Offset: 0x0015CE14
	public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
	{
		foreach (DirectoryInfo directoryInfo in source.GetDirectories())
		{
			GameCapture.CopyDirectory(directoryInfo, target.CreateSubdirectory(directoryInfo.Name));
		}
		foreach (FileInfo fileInfo in source.GetFiles())
		{
			fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name));
		}
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x0015EC80 File Offset: 0x0015CE80
	public static void IncrementExistingPath(ref string path)
	{
		int num = 1;
		string text = path;
		if (Directory.Exists(path))
		{
			while (Directory.Exists(text))
			{
				num++;
				text = string.Concat(new object[]
				{
					path,
					" (",
					num,
					")"
				});
			}
		}
		else if (File.Exists(path))
		{
			string directoryName = Path.GetDirectoryName(path);
			string extension = Path.GetExtension(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			while (File.Exists(text))
			{
				num++;
				text = Path.Combine(directoryName, string.Concat(new object[]
				{
					fileNameWithoutExtension,
					" (",
					num,
					")",
					extension
				}));
			}
		}
		path = text;
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x0015ED3C File Offset: 0x0015CF3C
	private static void SendFile(string server, string fileLocation, string fileName)
	{
		try
		{
			int port = 13000;
			TcpClient tcpClient = new TcpClient(server, port);
			byte[] array = new byte[1024];
			Encoding.Unicode.GetBytes(fileName, 0, fileName.Length, array, 0);
			byte[] array2 = File.ReadAllBytes(Path.Combine(fileLocation, fileName));
			NetworkStream stream = tcpClient.GetStream();
			stream.Write(array, 0, array.Length);
			stream.Write(array2, 0, array2.Length);
			Console.WriteLine("Sent: {0}", fileName);
			byte[] array3 = new byte[1024];
			string arg = string.Empty;
			int count = stream.Read(array3, 0, array3.Length);
			arg = Encoding.ASCII.GetString(array3, 0, count);
			Console.WriteLine("Received: {0}", arg);
			stream.Close();
			tcpClient.Close();
		}
		catch (ArgumentNullException arg2)
		{
			Console.WriteLine("ArgumentNullException: {0}", arg2);
		}
		catch (SocketException arg3)
		{
			Console.WriteLine("SocketException: {0}", arg3);
		}
		catch (Exception arg4)
		{
			Console.WriteLine("Other report exception: {0}", arg4);
		}
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x0015EE4C File Offset: 0x0015D04C
	private static void SendToServer(string fullPathFile)
	{
		string directoryName = Path.GetDirectoryName(fullPathFile);
		string fileName = Path.GetFileName(fullPathFile);
		if (new WebClient().DownloadString("https://ipv4.icanhazip.com/").Replace('\n', ' ').TrimEnd(Array.Empty<char>()) == "87.227.137.218")
		{
			GameCapture.SendFile("192.168.1.134", directoryName, fileName);
			return;
		}
		GameCapture.SendFile("87.227.137.218", directoryName, fileName);
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x0015EEAE File Offset: 0x0015D0AE
	private static void OpenWithFileSelection(string filepath)
	{
		Process.Start(new ProcessStartInfo
		{
			Arguments = "/select, \"" + filepath + "\"",
			FileName = "explorer.exe"
		});
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x0015EEDC File Offset: 0x0015D0DC
	private static void CaptureThread(object param)
	{
		GameCapture.CaptureParams captureParams = param as GameCapture.CaptureParams;
		string text = captureParams.destinationPath;
		string text2 = captureParams.captureName;
		string descriptionActual = captureParams.descriptionActual;
		string descriptionExpected = captureParams.descriptionExpected;
		string descriptionReproduce = captureParams.descriptionReproduce;
		byte[] screenShotBytes = captureParams.screenShotBytes;
		string saveDir = captureParams.saveDir;
		string saveName = captureParams.saveName;
		string campaignPath = captureParams.campaignPath;
		string companyName = captureParams.companyName;
		string productName = captureParams.productName;
		string exePath = captureParams.exePath;
		bool openOnCreate = captureParams.openOnCreate;
		try
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom != null && !text2.EndsWith("_host"))
			{
				if (kingdom.game.IsMultiplayer())
				{
					text2 += "_MP";
					if (kingdom.IsAuthority())
					{
						text2 += "h";
					}
					else
					{
						kingdom.SendEvent(new Logic.Kingdom.AuthorityCaptureGameEvent(text2));
						text2 += "c";
					}
				}
				else
				{
					text2 += "_SP";
				}
			}
			text2 = text2.Trim();
			if (string.IsNullOrEmpty(text) || !Directory.Exists(text))
			{
				text = saveDir;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "Game Capture";
			}
			if (!string.IsNullOrEmpty(saveDir) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				string text3 = Path.Combine(saveDir, "temp");
				GameCapture.IncrementExistingPath(ref text3);
				Directory.CreateDirectory(text3);
				if (screenShotBytes != null)
				{
					string path = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
					File.WriteAllBytes(Path.Combine(text3, path), screenShotBytes);
				}
				if (SaveGame.CanSave(true))
				{
					SaveGame.WaitSaveComplete();
					string text4 = Path.Combine(saveDir, saveName);
					if (Directory.Exists(text4))
					{
						Directory.Move(text4, Path.Combine(text3, saveName));
					}
				}
				Game game = GameLogic.Get(true);
				string text5;
				if (game == null)
				{
					text5 = null;
				}
				else
				{
					Game game2 = GameLogic.Get(true);
					string playerName;
					if (game2 == null)
					{
						playerName = null;
					}
					else
					{
						Logic.Multiplayer multiplayer = game2.multiplayer;
						if (multiplayer == null)
						{
							playerName = null;
						}
						else
						{
							Logic.Multiplayer.PlayerData playerData = multiplayer.playerData;
							playerName = ((playerData != null) ? playerData.name : null);
						}
					}
					text5 = game.GetGameStateDumpFilePath(playerName, Game.GameStateDumpFileType.Local);
				}
				string text6 = text5;
				if (File.Exists(text6))
				{
					File.Copy(text6, Path.Combine(text3, "GameState.txt"));
				}
				if (campaignPath != null && Directory.Exists(campaignPath))
				{
					Directory.CreateDirectory(Path.Combine(text3, "Campaign"));
					GameCapture.CopyDirectory(campaignPath, Path.Combine(text3, "Campaign"));
				}
				string path2 = Game.GetSavesRootDir(Game.SavesRoot.Root);
				string text7 = Path.Combine(text3, "logs");
				Directory.CreateDirectory(text7);
				if (UserInteractionLoggerLogic.logsEnabled)
				{
					string path3 = "user_interaction_log.txt";
					string text8 = Path.Combine(path2, path3);
					if (File.Exists(text8))
					{
						File.Copy(text8, Path.Combine(text7, path3));
					}
				}
				if (UserInteractionLoggerLogic.logsEnabled)
				{
					string path3 = "multiplayer.log";
					string text8 = Path.Combine(path2, path3);
					if (File.Exists(text8))
					{
						File.Copy(text8, Path.Combine(text7, path3));
					}
				}
				if (exePath != null)
				{
					string text9 = Path.Combine(Path.GetDirectoryName(exePath), "logs");
					if (Directory.Exists(text9))
					{
						string text10 = Path.Combine(text7, "thqno");
						Directory.CreateDirectory(text10);
						GameCapture.CopyDirectory(text9, text10);
					}
				}
				if (Directory.Exists(Path.Combine(text3, saveName, "lgs")))
				{
					GameCapture.CopyDirectory(Path.Combine(text3, saveName, "lgs"), text7);
					foreach (FileInfo fileInfo in new DirectoryInfo(text7).GetFiles())
					{
						if (fileInfo.Name.StartsWith("lg-", StringComparison.Ordinal) && string.IsNullOrEmpty(fileInfo.Extension))
						{
							File.Move(fileInfo.FullName, Path.ChangeExtension(fileInfo.FullName, ".zip"));
						}
					}
				}
				else
				{
					string text8 = Path.Combine(path2, AudioLog.fileName);
					if (File.Exists(text8))
					{
						File.Copy(text8, Path.Combine(text7, AudioLog.fileName));
					}
					string path3 = "Player.log";
					path2 = Path.Combine(new string[]
					{
						Environment.GetEnvironmentVariable("AppData"),
						"..",
						"LocalLow",
						companyName,
						productName
					});
					File.Copy(Path.Combine(path2, path3), Path.Combine(text7, path3));
				}
				if (!string.IsNullOrEmpty(descriptionActual) || !string.IsNullOrEmpty(descriptionExpected) || !string.IsNullOrEmpty(descriptionReproduce))
				{
					string s = string.Concat(new string[]
					{
						"Actual:\n",
						descriptionActual,
						"\n\nExpected:\n",
						descriptionExpected,
						"\n\nReproduce:\n",
						descriptionReproduce
					});
					using (FileStream fileStream = File.OpenWrite(Path.Combine(text3, "Description.txt")))
					{
						byte[] bytes = new UTF8Encoding(true).GetBytes(s);
						fileStream.Write(bytes, 0, bytes.Length);
					}
				}
				string text11 = Path.Combine(saveDir, text2 + ".zip");
				GameCapture.IncrementExistingPath(ref text11);
				string text12 = Path.Combine(text, text2 + ".zip");
				GameCapture.IncrementExistingPath(ref text12);
				ZipFile.CreateFromDirectory(text3, text11);
				Directory.Delete(text3, true);
				if (!Path.GetFullPath(text11).Equals(Path.GetFullPath(text12), StringComparison.InvariantCultureIgnoreCase))
				{
					File.Move(text11, text12);
				}
				if (screenShotBytes != null)
				{
					File.WriteAllBytes(Path.Combine(text, Path.ChangeExtension(text12, ".png")), screenShotBytes);
				}
				Debug.Log("before trying to open " + (openOnCreate ? "toggled" : "not toggled"));
				if (openOnCreate)
				{
					GameCapture.OpenWithFileSelection(text12);
				}
				GameCapture.SendToServer(text12);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Game capture exception: " + ex.ToString());
		}
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x0015F480 File Offset: 0x0015D680
	protected static void CaptureGame(GameCapture.CaptureParams captureParams)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (captureParams == null)
		{
			return;
		}
		string text = Title.Version(false);
		Debug.Log(string.Concat(new string[]
		{
			"Game Capture. User - ",
			THQNORequest.playerName,
			"(",
			THQNORequest.userId,
			")",
			(!string.IsNullOrEmpty(text)) ? (" Game Ver - " + text) : ""
		}));
		string text2 = GameCapture.CaptureDefaultDirectory();
		if (SaveGame.CanSave(true))
		{
			captureParams.saveName = captureParams.captureName + " Save";
			SaveGame.Save(Path.Combine(text2, captureParams.saveName), captureParams.saveName, -1, -1, null);
		}
		if (captureParams.screenShot != null)
		{
			Texture2D texture2D = new Texture2D(captureParams.screenShot.width, captureParams.screenShot.height, TextureFormat.RGB24, false);
			texture2D.SetPixels(captureParams.screenShot.GetPixels());
			texture2D.Apply();
			captureParams.screenShotBytes = captureParams.screenShot.EncodeToPNG();
		}
		Game game = GameLogic.Get(true);
		string campaignPath;
		if (game == null)
		{
			campaignPath = null;
		}
		else
		{
			Campaign campaign = game.campaign;
			campaignPath = ((campaign != null) ? campaign.Dir() : null);
		}
		captureParams.campaignPath = campaignPath;
		captureParams.saveDir = text2;
		captureParams.companyName = Application.companyName;
		captureParams.productName = Application.productName;
		captureParams.exePath = Application.dataPath;
		GameCapture.capture_thread = new Thread(new ParameterizedThreadStart(GameCapture.CaptureThread));
		GameCapture.capture_thread.Start(captureParams);
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x0015F5F2 File Offset: 0x0015D7F2
	protected static void CaptureGame(string destinationPath, string name, Texture2D screenShot = null, string descriptionActual = null, string descriptionExpected = null, string descriptionReproduce = null, bool openOnCreate = false)
	{
		GameCapture.CaptureGame(new GameCapture.CaptureParams
		{
			destinationPath = destinationPath,
			captureName = name,
			descriptionActual = descriptionActual,
			descriptionExpected = descriptionExpected,
			descriptionReproduce = descriptionReproduce,
			screenShot = screenShot,
			openOnCreate = openOnCreate
		});
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x0015F634 File Offset: 0x0015D834
	private static void DumpGameStateLocally()
	{
		try
		{
			Game game = GameLogic.Get(true);
			if (game != null)
			{
				game.DumpGameStateLocally();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x0015F670 File Offset: 0x0015D870
	public static void Capture(string destinationPath, string name, Texture2D screenShot = null, string descriptionActual = null, string descriptionExpected = null, string descriptionReproduce = null, bool openOnCreate = false)
	{
		GameCapture.DumpGameStateLocally();
		if (screenShot != null)
		{
			GameCapture.CaptureGame(destinationPath, name, screenShot, descriptionActual, descriptionExpected, descriptionReproduce, openOnCreate);
			return;
		}
		GameCapture gameCapture = new GameObject("temp").AddComponent<GameCapture>();
		gameCapture.captureParams = new GameCapture.CaptureParams();
		gameCapture.captureParams.destinationPath = destinationPath;
		gameCapture.captureParams.captureName = name;
		gameCapture.captureParams.descriptionActual = descriptionActual;
		gameCapture.captureParams.descriptionExpected = descriptionExpected;
		gameCapture.captureParams.descriptionReproduce = descriptionReproduce;
		gameCapture.captureParams.screenShot = screenShot;
		gameCapture.captureParams.openOnCreate = openOnCreate;
		gameCapture.StartCoroutine(gameCapture.TakeScreeenShotAndCapture());
	}

	// Token: 0x04001BEF RID: 7151
	public static Thread capture_thread = null;

	// Token: 0x04001BF0 RID: 7152
	public static object Lock = new object();

	// Token: 0x04001BF1 RID: 7153
	private GameCapture.CaptureParams captureParams;

	// Token: 0x020007EE RID: 2030
	public class CaptureParams
	{
		// Token: 0x04003CFB RID: 15611
		public string destinationPath;

		// Token: 0x04003CFC RID: 15612
		public string captureName;

		// Token: 0x04003CFD RID: 15613
		public Texture2D screenShot;

		// Token: 0x04003CFE RID: 15614
		public byte[] screenShotBytes;

		// Token: 0x04003CFF RID: 15615
		public string descriptionActual;

		// Token: 0x04003D00 RID: 15616
		public string descriptionExpected;

		// Token: 0x04003D01 RID: 15617
		public string descriptionReproduce;

		// Token: 0x04003D02 RID: 15618
		public string saveDir;

		// Token: 0x04003D03 RID: 15619
		public string saveName;

		// Token: 0x04003D04 RID: 15620
		public string campaignPath;

		// Token: 0x04003D05 RID: 15621
		public string companyName;

		// Token: 0x04003D06 RID: 15622
		public string productName;

		// Token: 0x04003D07 RID: 15623
		public string exePath;

		// Token: 0x04003D08 RID: 15624
		public bool openOnCreate;
	}
}
