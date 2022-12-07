using System;
using System.Collections.Generic;
using System.Threading;
using Logic;
using UnityEngine;

// Token: 0x02000083 RID: 131
public static class Profile
{
	// Token: 0x060004EA RID: 1258 RVA: 0x00038A65 File Offset: 0x00036C65
	static Profile()
	{
		Profile.Init();
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00038A81 File Offset: 0x00036C81
	public static void Init()
	{
		Profile.main_thread = Thread.CurrentThread;
		Game.fnBeginProfile = new Action<string>(Profile.BeginSection);
		Game.fnEndProfile = new Action<string>(Profile.EndSection);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00038AAF File Offset: 0x00036CAF
	public static void Enable(bool enable)
	{
		Profile.enabled = enable;
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00038AB7 File Offset: 0x00036CB7
	public static void BeginSection(string name)
	{
		if (!Profile.enabled)
		{
			return;
		}
		if (Thread.CurrentThread != Profile.main_thread)
		{
			return;
		}
		Profile.stack.Add(name);
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00038ADC File Offset: 0x00036CDC
	public static void EndSection(string name)
	{
		if (!Profile.enabled)
		{
			return;
		}
		if (Thread.CurrentThread != Profile.main_thread)
		{
			return;
		}
		if (Profile.stack.Count <= 0)
		{
			Debug.LogError("Profiling mismatch!\nThe EndSection request: \"" + name + "\" without BeginSection");
			return;
		}
		if (Profile.stack[Profile.stack.Count - 1] == name)
		{
			Profile.stack.RemoveAt(Profile.stack.Count - 1);
			return;
		}
		string text = "";
		int num = -1;
		for (int i = Profile.stack.Count - 1; i >= 0; i--)
		{
			string text2 = Profile.stack[i];
			if (text2 == name)
			{
				num = i;
			}
			text = text + text2 + "\n";
		}
		if (num >= 0)
		{
			Profile.stack.RemoveRange(num, Profile.stack.Count - num);
		}
		Debug.LogError("Profiling mismatch!\nThe EndSection request: \"" + name + "\" does not match the last BeginSection call.\nProfiling stack:\n" + text);
	}

	// Token: 0x040004BA RID: 1210
	public static List<string> stack = new List<string>(1024);

	// Token: 0x040004BB RID: 1211
	private static bool enabled = true;

	// Token: 0x040004BC RID: 1212
	private static Thread main_thread;
}
