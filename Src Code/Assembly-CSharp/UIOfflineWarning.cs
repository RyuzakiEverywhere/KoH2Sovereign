using System;
using TMPro;

// Token: 0x0200025C RID: 604
public class UIOfflineWarning : UIWindow
{
	// Token: 0x06002537 RID: 9527 RVA: 0x0014B62D File Offset: 0x0014982D
	public override string GetDefId()
	{
		return UIOfflineWarning.wnd_def_id;
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x0014B634 File Offset: 0x00149834
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Confirm != null)
		{
			this.m_Confirm.onClick = new BSGButton.OnClick(this.HandleOnConfirm);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x0014B674 File Offset: 0x00149874
	public void SetReasonText(string reason)
	{
		this.Init();
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, UIOfflineWarning.wnd_def_id + ".caption", null, null);
		}
		if (this.m_ConfirmLabel != null)
		{
			UIText.SetTextKey(this.m_ConfirmLabel, UIOfflineWarning.wnd_def_id + ".confirm", null, null);
		}
		string text = this.ReasonToMessage(reason);
		if (this.m_ReasonMessage != null)
		{
			UIText.SetText(this.m_ReasonMessage, text);
		}
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x0014B6FD File Offset: 0x001498FD
	public override void Show()
	{
		base.Show();
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x00145FC9 File Offset: 0x001441C9
	public override void Close(bool silent = false)
	{
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x0014B711 File Offset: 0x00149911
	private string ReasonToMessage(string reason)
	{
		return Defs.Localize("TitleScreen.Multiplayer.OfflineWarning." + reason, null, null, true, true);
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x0014B727 File Offset: 0x00149927
	private void HandleOnConfirm(BSGButton btn)
	{
		Action<string> action = this.onUserChoice;
		if (action != null)
		{
			action("ok");
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001951 RID: 6481
	private static string wnd_def_id = "OfflineWarning";

	// Token: 0x04001952 RID: 6482
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001953 RID: 6483
	[UIFieldTarget("id_ReasonMessage")]
	private TextMeshProUGUI m_ReasonMessage;

	// Token: 0x04001954 RID: 6484
	[UIFieldTarget("id_ConfirmLabel")]
	public TextMeshProUGUI m_ConfirmLabel;

	// Token: 0x04001955 RID: 6485
	[UIFieldTarget("id_Confirm")]
	public BSGButton m_Confirm;

	// Token: 0x04001956 RID: 6486
	public Action<string> onUserChoice;
}
