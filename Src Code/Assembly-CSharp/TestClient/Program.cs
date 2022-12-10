using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TestClient
{
	// Token: 0x02000340 RID: 832
	internal class Program
	{
		// Token: 0x06003298 RID: 12952 RVA: 0x0019AC00 File Offset: 0x00198E00
		private static void SendFile(string server, string FileLocation, string FileName)
		{
			try
			{
				int port = 13000;
				TcpClient tcpClient = new TcpClient(server, port);
				byte[] array = new byte[1024];
				for (int i = 0; i < array.Length; i++)
				{
					if (i < FileName.Length)
					{
						array[i] = (byte)FileName[i];
					}
					else
					{
						array[i] = 32;
					}
				}
				byte[] array2 = File.ReadAllBytes(FileName);
				NetworkStream stream = tcpClient.GetStream();
				stream.Write(array, 0, array.Length);
				stream.Write(array2, 0, array2.Length);
				Console.WriteLine("Sent: {0}", FileName);
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
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x0019AD10 File Offset: 0x00198F10
		private static void Main(string[] args)
		{
			string fileName = "Report1.zip";
			string fileLocation = "C:\\Users\\User\\Desktop\\TestServ\\FilesSend\\";
			Program.SendFile("192.168.1.134", fileLocation, fileName);
			Console.WriteLine("\n Press Enter to continue...");
			Console.Read();
		}
	}
}
