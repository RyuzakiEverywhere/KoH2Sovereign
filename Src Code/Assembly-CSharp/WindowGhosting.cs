using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200015D RID: 349
public static class WindowGhosting
{
	// Token: 0x060011CB RID: 4555
	[DllImport("user32.dll")]
	private static extern void DisableProcessWindowsGhosting();

	// Token: 0x060011CC RID: 4556 RVA: 0x000BB660 File Offset: 0x000B9860
	public static void Disable()
	{
		try
		{
			WindowGhosting.DisableProcessWindowsGhosting();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error when trying to suppress window ghosting.\nMessage:\n" + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace);
		}
	}
}
