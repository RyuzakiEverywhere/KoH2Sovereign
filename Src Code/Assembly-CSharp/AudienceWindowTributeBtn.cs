using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001A9 RID: 425
public class AudienceWindowTributeBtn : MonoBehaviour
{
	// Token: 0x06001894 RID: 6292 RVA: 0x000F1E40 File Offset: 0x000F0040
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_BtnEmpty != null)
		{
			this.m_BtnEmpty.onClick = delegate(BSGButton BSGbtn)
			{
				AudienceWindowTributeBtn.OnClick onClick = this.onClick;
				if (onClick == null)
				{
					return;
				}
				onClick(BSGbtn);
			};
			this.m_BtnEmpty.SetAudioSet("DefaultAudioSetPaper");
		}
		if (this.m_BtnFull != null)
		{
			this.m_BtnFull.onClick = delegate(BSGButton BSGbtn)
			{
				AudienceWindowTributeBtn.OnClick onClick = this.onClick;
				if (onClick == null)
				{
					return;
				}
				onClick(BSGbtn);
			};
			this.m_BtnFull.SetAudioSet("DefaultAudioSetPaper");
			this.m_BtnFull.onEvent = delegate(BSGButton btn, BSGButton.Event e, PointerEventData ed)
			{
				if (e == BSGButton.Event.Enter)
				{
					if (this.m_ResetButton != null)
					{
						this.m_ResetButton.gameObject.SetActive(true);
						return;
					}
				}
				else if (e == BSGButton.Event.Leave && this.m_ResetButton != null)
				{
					this.m_ResetButton.gameObject.SetActive(false);
				}
			};
		}
		if (this.m_ResetButton != null)
		{
			this.m_ResetButton.onClick = delegate(BSGButton BSGbtn)
			{
				AudienceWindowTributeBtn.OnClick onClick = this.onReset;
				if (onClick == null)
				{
					return;
				}
				onClick(BSGbtn);
			};
			this.m_ResetButton.SetAudioSet("DefaultAudioSetPaper");
		}
		this.m_Initialized = true;
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x000F1F1C File Offset: 0x000F011C
	public void SetOffer(Offer offer)
	{
		if (offer == null)
		{
			return;
		}
		this.Init();
		this.offer = offer;
		bool flag = offer is EmptyOffer;
		this.m_GroupEmpty.SetActive(flag);
		this.m_GroupFull.SetActive(!flag);
		if (!flag && this.m_FullBtnText != null)
		{
			string text = global::Defs.Localize(offer.def.field.FindChild("label", null, true, true, true, '.'), offer, null, true, true);
			if (offer.Validate() != "ok")
			{
				text = "<color=#865757>" + text + "</color>";
			}
			UIText.SetText(this.m_FullBtnText, text);
			Tooltip.Get(base.gameObject, true).SetText(offer.def.field.key + ".tooltip", null, new Vars(offer));
		}
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x000F2004 File Offset: 0x000F0204
	public void SetSelected(bool selected)
	{
		BSGButton btnEmpty = this.m_BtnEmpty;
		if (btnEmpty != null)
		{
			btnEmpty.AllowSelection(true);
		}
		BSGButton btnEmpty2 = this.m_BtnEmpty;
		if (btnEmpty2 != null)
		{
			btnEmpty2.SetSelected(selected, false);
		}
		BSGButton btnEmpty3 = this.m_BtnEmpty;
		if (btnEmpty3 != null)
		{
			btnEmpty3.AllowSelection(false);
		}
		BSGButton btnFull = this.m_BtnFull;
		if (btnFull != null)
		{
			btnFull.AllowSelection(true);
		}
		BSGButton btnFull2 = this.m_BtnFull;
		if (btnFull2 != null)
		{
			btnFull2.SetSelected(selected, false);
		}
		BSGButton btnFull3 = this.m_BtnFull;
		if (btnFull3 == null)
		{
			return;
		}
		btnFull3.AllowSelection(false);
	}

	// Token: 0x04000FD2 RID: 4050
	[UIFieldTarget("id_GroupEmpty")]
	private GameObject m_GroupEmpty;

	// Token: 0x04000FD3 RID: 4051
	[UIFieldTarget("id_BtnEmpty")]
	private BSGButton m_BtnEmpty;

	// Token: 0x04000FD4 RID: 4052
	[UIFieldTarget("id_GroupFull")]
	private GameObject m_GroupFull;

	// Token: 0x04000FD5 RID: 4053
	[UIFieldTarget("id_BtnFull")]
	private BSGButton m_BtnFull;

	// Token: 0x04000FD6 RID: 4054
	[UIFieldTarget("id_FullBtnText")]
	private TextMeshProUGUI m_FullBtnText;

	// Token: 0x04000FD7 RID: 4055
	[UIFieldTarget("id_ResetButton")]
	private BSGButton m_ResetButton;

	// Token: 0x04000FD8 RID: 4056
	private bool m_Initialized;

	// Token: 0x04000FD9 RID: 4057
	public Offer offer;

	// Token: 0x04000FDA RID: 4058
	public AudienceWindowTributeBtn.OnClick onClick;

	// Token: 0x04000FDB RID: 4059
	public AudienceWindowTributeBtn.OnClick onReset;

	// Token: 0x020006FC RID: 1788
	// (Invoke) Token: 0x0600492E RID: 18734
	public delegate void OnClick(BSGButton btn);
}
