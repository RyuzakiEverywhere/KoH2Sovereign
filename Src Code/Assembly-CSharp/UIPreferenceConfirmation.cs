using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class UIPreferenceConfirmation : UIWindow
{
	// Token: 0x06002646 RID: 9798 RVA: 0x00150568 File Offset: 0x0014E768
	public override string GetDefId()
	{
		return UIPreferenceConfirmation.def_id;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x00150570 File Offset: 0x0014E770
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_ConfirmBtn != null)
		{
			this.m_ConfirmBtn.onClick = new BSGButton.OnClick(this.Confirm);
		}
		if (this.m_CancelBtn != null)
		{
			this.m_CancelBtn.onClick = new BSGButton.OnClick(this.Cancel);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x001505DE File Offset: 0x0014E7DE
	public static UIPreferenceConfirmation Create(DT.Field def, Transform parent, Action onConfirm, Action onCancel)
	{
		UIPreferenceConfirmation component = UICommon.LoadPrefab(UIPreferenceConfirmation.def_id, parent.transform).GetComponent<UIPreferenceConfirmation>();
		component.Open(def, onConfirm, onCancel);
		return component;
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x00150600 File Offset: 0x0014E800
	private void Open(DT.Field def, Action onConfirm, Action onCancel)
	{
		if (def == null)
		{
			return;
		}
		this.Init();
		this.def = def;
		this.onConfirm = onConfirm;
		this.onCancel = onCancel;
		UIText.SetText(this.m_Caption, def, "caption", null, null);
		UIText.SetText(this.m_ConfirmText, def, "body", null, null);
		UIText.SetText(this.m_ConfirmBtnLabel, def, "confirm", null, null);
		UIText.SetText(this.m_CancelBtnLabel, def, "cancel", null, null);
		this.timeout_time = ((def == null) ? 0f : def.GetFloat("timeout", null, 0f, true, true, true, '.'));
		this.m_ConfirmTimerText.gameObject.SetActive(this.timeout_time != 0f);
		this.Open();
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x001506C4 File Offset: 0x0014E8C4
	protected override void Update()
	{
		base.Update();
		if (this.m_ConfirmTimerText.gameObject.activeSelf)
		{
			this.timeout_time -= UnityEngine.Time.unscaledDeltaTime;
			if (this.timeout_time < 0f)
			{
				this.Cancel(null);
			}
			UIText.SetText(this.m_ConfirmTimerText, ((int)this.timeout_time).ToString());
		}
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x00150729 File Offset: 0x0014E929
	private void Cancel(BSGButton btn)
	{
		Action action = this.onCancel;
		if (action != null)
		{
			action();
		}
		this.Close(false);
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x00150743 File Offset: 0x0014E943
	private void Confirm(BSGButton btn)
	{
		Action action = this.onConfirm;
		if (action != null)
		{
			action();
		}
		this.Close(false);
	}

	// Token: 0x040019DC RID: 6620
	[UIFieldTarget("id_ConfirmCaption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x040019DD RID: 6621
	[UIFieldTarget("id_ConfirmText")]
	private TextMeshProUGUI m_ConfirmTextCaption;

	// Token: 0x040019DE RID: 6622
	[UIFieldTarget("id_ConfirmText")]
	private TextMeshProUGUI m_ConfirmText;

	// Token: 0x040019DF RID: 6623
	[UIFieldTarget("id_ConfirmTimerText")]
	private TextMeshProUGUI m_ConfirmTimerText;

	// Token: 0x040019E0 RID: 6624
	[UIFieldTarget("id_ConfirmBtn")]
	private BSGButton m_ConfirmBtn;

	// Token: 0x040019E1 RID: 6625
	[UIFieldTarget("id_ConfirmBtnLabel")]
	private TextMeshProUGUI m_ConfirmBtnLabel;

	// Token: 0x040019E2 RID: 6626
	[UIFieldTarget("id_CancelBtn")]
	private BSGButton m_CancelBtn;

	// Token: 0x040019E3 RID: 6627
	[UIFieldTarget("id_CancelBtnLabel")]
	private TextMeshProUGUI m_CancelBtnLabel;

	// Token: 0x040019E4 RID: 6628
	private bool m_Initialzied;

	// Token: 0x040019E5 RID: 6629
	public DT.Field def;

	// Token: 0x040019E6 RID: 6630
	private Action onConfirm;

	// Token: 0x040019E7 RID: 6631
	private Action onCancel;

	// Token: 0x040019E8 RID: 6632
	public float timeout_time;

	// Token: 0x040019E9 RID: 6633
	private static string def_id = "PreferencesConfirmationWindow";
}
