using System;
using TMPro;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class UINetworkErrorWindow : MonoBehaviour
{
	// Token: 0x06002C5F RID: 11359 RVA: 0x00173424 File Offset: 0x00171624
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.btnClose != null)
		{
			this.btnClose.onClick = new BSGButton.OnClick(this.HanldeOnClose);
		}
		if (this.btnQuit != null)
		{
			this.btnQuit.onClick = new BSGButton.OnClick(this.HanldeOnQuitToMainMenu);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x00173492 File Offset: 0x00171692
	private void Awake()
	{
		if (UINetworkErrorWindow.instance != null)
		{
			Object.Destroy(UINetworkErrorWindow.instance.gameObject);
		}
		UINetworkErrorWindow.instance = this;
		this.Show(false);
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x001734BD File Offset: 0x001716BD
	private void OnDestroy()
	{
		if (this.btnQuit != null)
		{
			this.btnQuit.onClick = null;
		}
		if (this.btnClose != null)
		{
			this.btnClose.onClick = null;
		}
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x00101716 File Offset: 0x000FF916
	private void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x001734F3 File Offset: 0x001716F3
	private static void LoadPrefab(string caption, string body)
	{
		if (BaseUI.Get() != null)
		{
			GameObject gameObject = UICommon.LoadPrefab("NetworkErrorWindow", null);
			UINetworkErrorWindow.instance = ((gameObject != null) ? gameObject.GetComponent<UINetworkErrorWindow>() : null);
			UINetworkErrorWindow.SetCaption(caption);
			UINetworkErrorWindow.SetBody(body);
		}
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x0017352A File Offset: 0x0017172A
	private static void SetCaption(string text)
	{
		UINetworkErrorWindow uinetworkErrorWindow = UINetworkErrorWindow.instance;
		if (uinetworkErrorWindow != null)
		{
			uinetworkErrorWindow.Init();
		}
		UINetworkErrorWindow uinetworkErrorWindow2 = UINetworkErrorWindow.instance;
		UIText.SetText((uinetworkErrorWindow2 != null) ? uinetworkErrorWindow2.m_Caption : null, text);
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x00173553 File Offset: 0x00171753
	private static void SetBody(string text)
	{
		UINetworkErrorWindow uinetworkErrorWindow = UINetworkErrorWindow.instance;
		if (uinetworkErrorWindow != null)
		{
			uinetworkErrorWindow.Init();
		}
		UINetworkErrorWindow uinetworkErrorWindow2 = UINetworkErrorWindow.instance;
		UIText.SetText((uinetworkErrorWindow2 != null) ? uinetworkErrorWindow2.m_Body : null, text);
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x0017357C File Offset: 0x0017177C
	public static bool ShowWindow(string caption, string body)
	{
		if (UINetworkErrorWindow.instance == null)
		{
			UINetworkErrorWindow.LoadPrefab(caption, body);
		}
		if (UINetworkErrorWindow.instance != null)
		{
			UINetworkErrorWindow.SetCaption(caption);
			UINetworkErrorWindow.SetBody(body);
			UINetworkErrorWindow.instance.Show(true);
			return true;
		}
		return false;
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x001735B9 File Offset: 0x001717B9
	public static bool ShowNetworkMessageHeaderErrorWindow(string msg)
	{
		string caption = Defs.Localize("System.Network.network_error_caption", null, null, true, true);
		if (msg == null)
		{
			msg = Defs.Localize("System.Network.network_error_default", null, null, true, true);
		}
		return UINetworkErrorWindow.ShowWindow(caption, msg);
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x000C4358 File Offset: 0x000C2558
	private void HanldeOnClose(BSGButton btn)
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06002C69 RID: 11369 RVA: 0x001735E2 File Offset: 0x001717E2
	private void HanldeOnQuitToMainMenu(BSGButton btn)
	{
		this.Show(false);
		GameLogic.QuitToTitle();
	}

	// Token: 0x04001E46 RID: 7750
	private static UINetworkErrorWindow instance;

	// Token: 0x04001E47 RID: 7751
	[UIFieldTarget("id_Quit")]
	private BSGButton btnQuit;

	// Token: 0x04001E48 RID: 7752
	[UIFieldTarget("id_QuitLabel")]
	private TextMeshProUGUI m_QuitLabel;

	// Token: 0x04001E49 RID: 7753
	[UIFieldTarget("id_Close")]
	private BSGButton btnClose;

	// Token: 0x04001E4A RID: 7754
	[UIFieldTarget("id_CloseLabel")]
	private TextMeshProUGUI m_CloseLabel;

	// Token: 0x04001E4B RID: 7755
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001E4C RID: 7756
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI m_Body;

	// Token: 0x04001E4D RID: 7757
	private bool m_Initialzied;
}
