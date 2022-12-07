using System;
using System.Text;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class SystemInfoLogger : MonoBehaviour
{
	// Token: 0x060004AF RID: 1199 RVA: 0x00036A50 File Offset: 0x00034C50
	private void Awake()
	{
		if (SystemInfoLogger.instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			SystemInfoLogger.instance = this;
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			this.Log();
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00036A98 File Offset: 0x00034C98
	private void Log()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Running on the following system:");
		stringBuilder.AppendLine(string.Format("\tCPU: {0} ({1}x{2}MHz)", SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.processorFrequency));
		stringBuilder.AppendLine(string.Format("\tRAM: {0}MB", SystemInfo.systemMemorySize));
		stringBuilder.AppendLine(string.Format("\tGPU: {0} ({1}MB)", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize));
		stringBuilder.AppendLine("\tOS: " + SystemInfo.operatingSystem);
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x0400048B RID: 1163
	private static SystemInfoLogger instance;
}
