using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class UIWindowDispatcher
{
	// Token: 0x06002FE9 RID: 12265 RVA: 0x001871BC File Offset: 0x001853BC
	public void SetFocusWindow(UIWindow obj)
	{
		UIWindow prev = this.focus_window;
		this.focus_window = obj;
		if (obj != null)
		{
			if (this.active_windows_stack.Contains(obj))
			{
				this.active_windows_stack.Remove(obj);
			}
			this.active_windows_stack.Add(obj);
		}
		if (UIWindowDispatcher.on_active_wnd_changed != null)
		{
			UIWindowDispatcher.on_active_wnd_changed(BaseUI.Get(), prev);
		}
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x0018721E File Offset: 0x0018541E
	public void DismissWindow(UIWindow o)
	{
		this.active_windows_stack.Remove(o);
		this.SetFocusWindow((this.active_windows_stack.Count > 0) ? this.active_windows_stack[this.active_windows_stack.Count - 1] : null);
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x0018725C File Offset: 0x0018545C
	public void CloseAllWindows()
	{
		for (int i = this.active_windows_stack.Count - 1; i >= 0; i--)
		{
			this.active_windows_stack[i].Close(false);
		}
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x00187293 File Offset: 0x00185493
	public UIWindow GetFocusWindow()
	{
		return this.focus_window;
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x0018729B File Offset: 0x0018549B
	public bool HasFocusWindow()
	{
		return this.GetFocusWindow() != null;
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x001872AC File Offset: 0x001854AC
	public bool HandleBackInputAction()
	{
		UIWindow focusWindow = this.GetFocusWindow();
		return !(focusWindow == null) && focusWindow.OnBackInputAction();
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x001872D4 File Offset: 0x001854D4
	public UIWindow GetWindowFromExclusiveSet(string tag)
	{
		if (UIWindow.NoExclusiveWindowSet(tag))
		{
			return null;
		}
		for (int i = this.active_windows_stack.Count - 1; i >= 0; i--)
		{
			if (this.active_windows_stack[i].IsFromExclusiveWindowSet(tag))
			{
				return this.active_windows_stack[i];
			}
		}
		return null;
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x00187328 File Offset: 0x00185528
	public WindowType GetWindow<WindowType>() where WindowType : UIWindow
	{
		for (int i = this.active_windows_stack.Count - 1; i >= 0; i--)
		{
			if (this.active_windows_stack[i] is WindowType)
			{
				return (WindowType)((object)this.active_windows_stack[i]);
			}
		}
		return default(WindowType);
	}

	// Token: 0x04002049 RID: 8265
	public static Dictionary<string, Vector3> dict = new Dictionary<string, Vector3>();

	// Token: 0x0400204A RID: 8266
	public static UIWindowDispatcher.OnActiveWndChanged on_active_wnd_changed = null;

	// Token: 0x0400204B RID: 8267
	public List<UIWindow> active_windows_stack = new List<UIWindow>();

	// Token: 0x0400204C RID: 8268
	public UIWindow focus_window;

	// Token: 0x0200086C RID: 2156
	// (Invoke) Token: 0x06005116 RID: 20758
	public delegate void OnActiveWndChanged(BaseUI ui, UIWindow prev);
}
