using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
public class PlayerQuitTracker
{
	// Token: 0x060010CB RID: 4299 RVA: 0x000B3390 File Offset: 0x000B1590
	private static void Quit()
	{
		PlayerQuitTracker.isQuitting = true;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x000B3398 File Offset: 0x000B1598
	[RuntimeInitializeOnLoadMethod]
	private static void RunOnStart()
	{
		PlayerQuitTracker.isQuitting = false;
		Application.quitting += PlayerQuitTracker.Quit;
	}

	// Token: 0x04000B29 RID: 2857
	public static bool isQuitting;
}
