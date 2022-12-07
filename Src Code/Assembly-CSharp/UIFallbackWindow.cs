using System;
using TMPro;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class UIFallbackWindow : MonoBehaviour
{
	// Token: 0x06002C4F RID: 11343 RVA: 0x001731B0 File Offset: 0x001713B0
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnQuit != null)
		{
			this.btnQuit.onClick = new BSGButton.OnClick(this.HanldeOnQuitToTitle);
		}
		if (this.m_QuitLabel != null)
		{
			UIText.SetTextKey(this.m_QuitLabel, "Menu.return_to_main_game", null, null);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x00173219 File Offset: 0x00171419
	private void Awake()
	{
		if (UIFallbackWindow.instance != null)
		{
			Object.Destroy(UIFallbackWindow.instance.gameObject);
		}
		UIFallbackWindow.instance = this;
		this.Show(false);
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x00173244 File Offset: 0x00171444
	private void OnDestroy()
	{
		if (this.btnQuit != null)
		{
			this.btnQuit.onClick = null;
		}
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x00101716 File Offset: 0x000FF916
	private void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x00173260 File Offset: 0x00171460
	private static void LoadPrefab(string caption, string body, string button)
	{
		if (BaseUI.Get() != null)
		{
			UIFallbackWindow.instance = UICommon.LoadPrefab("FallbackWindow", null).GetComponent<UIFallbackWindow>();
			UIFallbackWindow.SetCaption(caption);
			UIFallbackWindow.SetBody(body);
			UIFallbackWindow.SetButton(button);
		}
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x00173296 File Offset: 0x00171496
	private static void SetCaption(string text)
	{
		UIFallbackWindow uifallbackWindow = UIFallbackWindow.instance;
		if (uifallbackWindow != null)
		{
			uifallbackWindow.Init();
		}
		UIFallbackWindow uifallbackWindow2 = UIFallbackWindow.instance;
		UIText.SetText((uifallbackWindow2 != null) ? uifallbackWindow2.m_Caption : null, text);
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x001732BF File Offset: 0x001714BF
	private static void SetBody(string text)
	{
		UIFallbackWindow uifallbackWindow = UIFallbackWindow.instance;
		if (uifallbackWindow != null)
		{
			uifallbackWindow.Init();
		}
		UIFallbackWindow uifallbackWindow2 = UIFallbackWindow.instance;
		UIText.SetText((uifallbackWindow2 != null) ? uifallbackWindow2.m_Body : null, text);
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x001732E8 File Offset: 0x001714E8
	private static void SetButton(string text)
	{
		UIFallbackWindow uifallbackWindow = UIFallbackWindow.instance;
		if (uifallbackWindow != null)
		{
			uifallbackWindow.Init();
		}
		UIFallbackWindow uifallbackWindow2 = UIFallbackWindow.instance;
		UIText.SetText((uifallbackWindow2 != null) ? uifallbackWindow2.m_QuitLabel : null, text);
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x00173314 File Offset: 0x00171514
	public static bool ShowDialog(string caption, string body, string button, Action btnAction)
	{
		if (UIFallbackWindow.instance == null)
		{
			UIFallbackWindow.LoadPrefab(caption, body, button);
		}
		if (UIFallbackWindow.instance != null)
		{
			UIFallbackWindow.instance.OnBtnClick = btnAction;
			UIFallbackWindow.SetCaption(caption);
			UIFallbackWindow.SetBody(body);
			UIFallbackWindow.SetButton(button);
			UIFallbackWindow.instance.Show(true);
			return true;
		}
		return false;
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x0017336E File Offset: 0x0017156E
	private void HanldeOnQuitToTitle(BSGButton but)
	{
		if (this.OnBtnClick != null)
		{
			this.OnBtnClick();
			this.Show(false);
			return;
		}
		UIFallbackWindow.DefaultButtonBehaviour();
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x00173390 File Offset: 0x00171590
	public static void DefaultButtonBehaviour()
	{
		GameLogic.QuitToTitle();
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x00173397 File Offset: 0x00171597
	public static void Close()
	{
		if (UIFallbackWindow.instance != null)
		{
			UIFallbackWindow.instance.Show(false);
		}
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x001733B4 File Offset: 0x001715B4
	public static bool ShowDisconnectedWindow(string caption, string body, string button = null, Action btnAction = null)
	{
		string caption2 = Defs.Localize(caption, null, null, true, true);
		string body2 = Defs.Localize(body, null, null, true, true);
		string button2;
		if (button == null)
		{
			button2 = Defs.Localize("Menu.return_to_main_game", null, null, true, true);
		}
		else
		{
			button2 = Defs.Localize(button, null, null, true, true);
		}
		return UIFallbackWindow.ShowDialog(caption2, body2, button2, btnAction);
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x001733FD File Offset: 0x001715FD
	public static void HideDisconnectedWindow()
	{
		UIFallbackWindow.Close();
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x00173404 File Offset: 0x00171604
	public static bool IsActive()
	{
		return !(UIFallbackWindow.instance == null) && UIFallbackWindow.instance.gameObject.activeInHierarchy;
	}

	// Token: 0x04001E3F RID: 7743
	private static UIFallbackWindow instance;

	// Token: 0x04001E40 RID: 7744
	[UIFieldTarget("id_Quit")]
	private BSGButton btnQuit;

	// Token: 0x04001E41 RID: 7745
	[UIFieldTarget("id_QuitLabel")]
	private TextMeshProUGUI m_QuitLabel;

	// Token: 0x04001E42 RID: 7746
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001E43 RID: 7747
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI m_Body;

	// Token: 0x04001E44 RID: 7748
	private Action OnBtnClick;

	// Token: 0x04001E45 RID: 7749
	private bool m_Initialzied;
}
