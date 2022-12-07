using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Token: 0x02000339 RID: 825
internal class ReportsTCPServer
{
	// Token: 0x06003268 RID: 12904 RVA: 0x00199030 File Offset: 0x00197230
	private static long DirSize(DirectoryInfo d)
	{
		long num = 0L;
		foreach (FileInfo fileInfo in d.GetFiles())
		{
			num += fileInfo.Length;
		}
		foreach (DirectoryInfo d2 in d.GetDirectories())
		{
			num += ReportsTCPServer.DirSize(d2);
		}
		return num;
	}

	// Token: 0x06003269 RID: 12905 RVA: 0x0019908C File Offset: 0x0019728C
	private static void IncrementExistingPath(ref string path)
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

	// Token: 0x0600326A RID: 12906 RVA: 0x00199148 File Offset: 0x00197348
	public static void Main()
	{
		for (;;)
		{
			TcpListener tcpListener = null;
			TcpClient tcpClient = null;
			try
			{
				int num = 13000;
				string text = null;
				foreach (IPAddress ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
				{
					if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
					{
						text = ipaddress.ToString();
					}
				}
				string text2 = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Replace('\n', ' ').TrimEnd(Array.Empty<char>());
				Console.WriteLine(string.Concat(new object[]
				{
					"Listening on [Local ",
					text,
					":",
					num,
					"][Public ",
					text2,
					":",
					num,
					"]"
				}));
				tcpListener = new TcpListener(IPAddress.Parse(text), num);
				tcpListener.Start();
				byte[] array = new byte[1024];
				for (;;)
				{
					Console.WriteLine("Waiting for a connection... ");
					tcpClient = tcpListener.AcceptTcpClient();
					Console.WriteLine("Connected!");
					NetworkStream stream = tcpClient.GetStream();
					string text3 = null;
					string text4 = null;
					int count;
					if ((count = stream.Read(array, 0, array.Length)) != 0)
					{
						text3 = Encoding.Unicode.GetString(array, 0, count).Replace("\0", string.Empty).Trim();
						Console.Write("File: {0}", text3);
						text4 = Path.Combine(ReportsTCPServer.saveDir, text3);
						ReportsTCPServer.IncrementExistingPath(ref text4);
						byte[] bytes = Encoding.Unicode.GetBytes(text4);
						stream.Write(bytes, 0, bytes.Length);
					}
					if (text3 != null)
					{
						using (FileStream fileStream = File.Create(text4))
						{
							Console.Write(" - Recieving");
							byte[] array2 = new byte[1024];
							int count2;
							while ((count2 = stream.Read(array2, 0, array2.Length)) > 0)
							{
								fileStream.Write(array2, 0, count2);
							}
							Console.Write("-Recieved");
						}
						long num2 = ReportsTCPServer.DirSize(new DirectoryInfo(ReportsTCPServer.saveDir)) / 1048576L;
						Console.Write(" - Dir size: " + num2 + "MB");
						if (num2 > 102400L)
						{
							List<FileSystemInfo> list = new DirectoryInfo(ReportsTCPServer.saveDir).GetFileSystemInfos().OrderBy(delegate(FileSystemInfo fi)
							{
								fi.Refresh();
								return fi.LastWriteTime;
							}).ToList<FileSystemInfo>();
							int num3 = 0;
							while (num3 < 10 && num2 > 1024L)
							{
								FileSystemInfo fileSystemInfo = list[num3];
								Console.Write(" - Deleting: " + fileSystemInfo.FullName);
								if (File.Exists(fileSystemInfo.FullName))
								{
									File.Delete(Path.Combine(ReportsTCPServer.saveDir, fileSystemInfo.FullName));
								}
								num2 = ReportsTCPServer.DirSize(new DirectoryInfo(ReportsTCPServer.saveDir)) / 1048576L;
								num3++;
							}
						}
						Console.WriteLine();
					}
					tcpClient.Close();
				}
			}
			catch (SocketException arg)
			{
				Console.WriteLine("Socket Exception: " + arg);
			}
			catch (Exception arg2)
			{
				Console.WriteLine("Exception: " + arg2);
			}
			finally
			{
				tcpClient.Close();
				tcpListener.Stop();
			}
		}
	}

	// Token: 0x040021E8 RID: 8680
	private static string saveDir = "ReportsRecieved";
}
