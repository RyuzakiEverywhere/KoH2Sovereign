using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SVNToolsNamespace
{
	// Token: 0x020003B9 RID: 953
	public class Program
	{
		// Token: 0x060035BB RID: 13755 RVA: 0x001AF1AC File Offset: 0x001AD3AC
		public static int Run(string path, string args, out string output, out string errorOutput, bool logProcessInfo = false)
		{
			if (logProcessInfo)
			{
				Debug.Log("$>>" + path + " " + args);
			}
			Process process = null;
			int num = -1;
			output = "";
			errorOutput = "";
			try
			{
				process = new Process
				{
					StartInfo = new ProcessStartInfo(path, args)
					{
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					}
				};
				process.Start();
				StreamReader standardOutput = process.StandardOutput;
				output = standardOutput.ReadToEnd();
				StreamReader standardError = process.StandardError;
				errorOutput = standardError.ReadToEnd();
				process.WaitForExit();
				num = process.ExitCode;
			}
			catch (Exception ex)
			{
				if (logProcessInfo)
				{
					Debug.Log(ex.ToString());
				}
				num = -1;
			}
			if (process != null)
			{
				process.Close();
			}
			if (logProcessInfo)
			{
				Debug.Log("$>>Exit Code: " + num);
			}
			return num;
		}
	}
}
