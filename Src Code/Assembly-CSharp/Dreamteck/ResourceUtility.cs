using System;
using System.IO;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004A7 RID: 1191
	public static class ResourceUtility
	{
		// Token: 0x06003E7F RID: 15999 RVA: 0x001DE9C8 File Offset: 0x001DCBC8
		public static string FindFolder(string dir, string folderPattern)
		{
			if (folderPattern.StartsWith("/"))
			{
				folderPattern = folderPattern.Substring(1);
			}
			if (!dir.EndsWith("/"))
			{
				dir += "/";
			}
			if (folderPattern == "")
			{
				return "";
			}
			string[] array = folderPattern.Split(new char[]
			{
				'/'
			});
			if (array.Length == 0)
			{
				return "";
			}
			string text = "";
			try
			{
				foreach (string text2 in Directory.GetDirectories(dir))
				{
					if (new DirectoryInfo(text2).Name == array[0])
					{
						text = text2;
						string text3 = ResourceUtility.FindFolder(text2, string.Join("/", array, 1, array.Length - 1));
						if (text3 != "")
						{
							text = text3;
							break;
						}
					}
				}
				if (text == "")
				{
					string[] directories = Directory.GetDirectories(dir);
					for (int i = 0; i < directories.Length; i++)
					{
						text = ResourceUtility.FindFolder(directories[i], string.Join("/", array));
						if (text != "")
						{
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return "";
			}
			return text;
		}

		// Token: 0x06003E80 RID: 16000 RVA: 0x001DEB08 File Offset: 0x001DCD08
		public static Texture2D LoadTexture(string dreamteckPath, string textureFileName)
		{
			string text = Application.dataPath + "/Dreamteck/" + dreamteckPath;
			if (!Directory.Exists(text))
			{
				text = ResourceUtility.FindFolder(Application.dataPath, "Dreamteck/" + dreamteckPath);
				if (!Directory.Exists(text))
				{
					return null;
				}
			}
			if (!File.Exists(text + "/" + textureFileName))
			{
				return null;
			}
			byte[] data = File.ReadAllBytes(text + "/" + textureFileName);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.name = textureFileName;
			texture2D.LoadImage(data);
			return texture2D;
		}

		// Token: 0x06003E81 RID: 16001 RVA: 0x001DEB8C File Offset: 0x001DCD8C
		public static Texture2D LoadTexture(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(1, 1);
			FileInfo fileInfo = new FileInfo(path);
			texture2D.name = fileInfo.Name;
			texture2D.LoadImage(data);
			return texture2D;
		}
	}
}
