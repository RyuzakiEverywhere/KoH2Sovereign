using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class ScreenResolution : MonoBehaviour
{
	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06001322 RID: 4898 RVA: 0x000C741F File Offset: 0x000C561F
	// (set) Token: 0x06001323 RID: 4899 RVA: 0x000023FD File Offset: 0x000005FD
	public static int width
	{
		get
		{
			return UserSettings.ScreenWidth;
		}
		private set
		{
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06001324 RID: 4900 RVA: 0x000C7426 File Offset: 0x000C5626
	// (set) Token: 0x06001325 RID: 4901 RVA: 0x000023FD File Offset: 0x000005FD
	public static int height
	{
		get
		{
			return UserSettings.ScreenHeight;
		}
		private set
		{
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06001326 RID: 4902 RVA: 0x000C742D File Offset: 0x000C562D
	// (set) Token: 0x06001327 RID: 4903 RVA: 0x000023FD File Offset: 0x000005FD
	public static int refresh_rate
	{
		get
		{
			return UserSettings.ScreenRefreshRate;
		}
		private set
		{
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06001328 RID: 4904 RVA: 0x000C7434 File Offset: 0x000C5634
	// (set) Token: 0x06001329 RID: 4905 RVA: 0x000023FD File Offset: 0x000005FD
	public static FullScreenMode fullscreen_mode
	{
		get
		{
			return UserSettings.FullScreenMode;
		}
		private set
		{
		}
	}

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600132A RID: 4906 RVA: 0x000C743C File Offset: 0x000C563C
	// (remove) Token: 0x0600132B RID: 4907 RVA: 0x000C7470 File Offset: 0x000C5670
	public static event ScreenResolution.OnWindowResolutionChange on_window_resolution_change;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600132C RID: 4908 RVA: 0x000C74A4 File Offset: 0x000C56A4
	// (remove) Token: 0x0600132D RID: 4909 RVA: 0x000C74D8 File Offset: 0x000C56D8
	public static event ScreenResolution.OnWindowFullScreenChange on_fullscreen_change;

	// Token: 0x0600132E RID: 4910 RVA: 0x000C750B File Offset: 0x000C570B
	private void Awake()
	{
		if (ScreenResolution.instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			ScreenResolution.instance = this;
			ScreenResolution.skip_frames = 30;
		}
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x000C7534 File Offset: 0x000C5734
	private void Update()
	{
		if (ScreenResolution.skip_frames-- > 0)
		{
			return;
		}
		if (Screen.fullScreenMode == ScreenResolution.fullscreen_mode)
		{
			if (ScreenResolution.width != Screen.width || ScreenResolution.height != Screen.height)
			{
				ScreenResolution.SetResolution(ScreenResolution.width, ScreenResolution.height, ScreenResolution.fullscreen_mode, ScreenResolution.refresh_rate);
				return;
			}
		}
		else
		{
			UserSettings.SettingData setting = UserSettings.GetSetting("window_mode");
			if (setting == null)
			{
				return;
			}
			setting.ApplyValue(Screen.fullScreenMode.ToString());
		}
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x000C75BC File Offset: 0x000C57BC
	public static void SetResolution(int width, int height, FullScreenMode fullscreen_mode, int refresh_rate)
	{
		if (Screen.currentResolution.width == width && Screen.currentResolution.height == height && Screen.fullScreenMode == fullscreen_mode && Screen.currentResolution.refreshRate == refresh_rate)
		{
			Debug.Log(string.Format("{0}: Tried changing resolution, but it was already {1}x{2}-{3}hz {4}", new object[]
			{
				DateTime.Now.ToString("HH:mm:ss.fff: "),
				Screen.currentResolution.width,
				Screen.currentResolution.height,
				Screen.currentResolution.refreshRate,
				Screen.fullScreenMode
			}));
			return;
		}
		Debug.Log(string.Format("{0}: Changed resolution from {1}x{2}-{3}hz {4} to {5}x{6}-{7}hz {8}", new object[]
		{
			DateTime.Now.ToString("HH:mm:ss.fff: "),
			Screen.currentResolution.width,
			Screen.currentResolution.height,
			Screen.currentResolution.refreshRate,
			Screen.fullScreenMode,
			width,
			height,
			refresh_rate,
			fullscreen_mode
		}));
		Screen.SetResolution(width, height, fullscreen_mode, refresh_rate);
		ScreenResolution.OnWindowResolutionChange onWindowResolutionChange = ScreenResolution.on_window_resolution_change;
		if (onWindowResolutionChange != null)
		{
			onWindowResolutionChange();
		}
		if (Screen.fullScreenMode != fullscreen_mode)
		{
			ScreenResolution.OnWindowFullScreenChange onWindowFullScreenChange = ScreenResolution.on_fullscreen_change;
			if (onWindowFullScreenChange != null)
			{
				onWindowFullScreenChange();
			}
		}
		ScreenResolution.skip_frames = Math.Max(5, ScreenResolution.skip_frames);
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x000C7760 File Offset: 0x000C5960
	public static void SetFullScrenMode(FullScreenMode fullscreen_mode)
	{
		if (Screen.fullScreenMode == fullscreen_mode)
		{
			Debug.Log(DateTime.Now.ToString("HH:mm:ss.fff: ") + ": Tried Changing fullscreen, but it was already " + Screen.fullScreenMode.ToString());
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			DateTime.Now.ToString("HH:mm:ss.fff: "),
			": Chaned fullscreen from ",
			Screen.fullScreenMode.ToString(),
			" to ",
			fullscreen_mode.ToString()
		}));
		Screen.fullScreenMode = fullscreen_mode;
		ScreenResolution.OnWindowFullScreenChange onWindowFullScreenChange = ScreenResolution.on_fullscreen_change;
		if (onWindowFullScreenChange != null)
		{
			onWindowFullScreenChange();
		}
		ScreenResolution.skip_frames = Math.Max(5, ScreenResolution.skip_frames);
	}

	// Token: 0x04000CB9 RID: 3257
	private static ScreenResolution instance;

	// Token: 0x04000CBA RID: 3258
	private static int skip_frames;

	// Token: 0x04000CBB RID: 3259
	private const int skip_amount_on_chanage = 5;

	// Token: 0x020006A1 RID: 1697
	// (Invoke) Token: 0x0600482A RID: 18474
	public delegate void OnWindowResolutionChange();

	// Token: 0x020006A2 RID: 1698
	// (Invoke) Token: 0x0600482E RID: 18478
	public delegate void OnWindowFullScreenChange();
}
