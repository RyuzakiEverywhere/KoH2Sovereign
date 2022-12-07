using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class UIBattleViewControlGroup : MonoBehaviour
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06001ADC RID: 6876 RVA: 0x00103544 File Offset: 0x00101744
	// (remove) Token: 0x06001ADD RID: 6877 RVA: 0x0010357C File Offset: 0x0010177C
	public event Action<UIBattleViewControlGroup> onClick;

	// Token: 0x06001ADE RID: 6878 RVA: 0x001035B1 File Offset: 0x001017B1
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x001035BC File Offset: 0x001017BC
	public void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_definition == null)
		{
			this.m_definition = global::Defs.GetDefField("UIBattleViewControlGroup", null);
		}
		this.m_Button = base.GetComponent<BSGButton>();
		if (this.m_Button != null)
		{
			this.m_Button.AllowSelection(true);
			BSGButton button = this.m_Button;
			button.onClick = (BSGButton.OnClick)Delegate.Combine(button.onClick, new BSGButton.OnClick(this.Button_OnClicked));
		}
		this.m_isInitialized = true;
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x00103646 File Offset: 0x00101846
	public void SetText(string text)
	{
		this.m_GroupId.text = text;
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x00103654 File Offset: 0x00101854
	public void Select(bool select)
	{
		if (this.m_Button != null)
		{
			this.m_Button.SetSelected(select, true);
		}
		DT.Field field = select ? this.m_definition.FindChild("Selected", null, true, true, true, '.') : this.m_definition.FindChild("Default", null, true, true, true, '.');
		if (this.m_GroupId != null)
		{
			this.m_GroupId.color = global::Defs.GetColor(field, "text_color", null);
		}
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x001036D3 File Offset: 0x001018D3
	private void Button_OnClicked(BSGButton button)
	{
		Action<UIBattleViewControlGroup> action = this.onClick;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x001036E6 File Offset: 0x001018E6
	private void Clear()
	{
		if (this.m_Button != null)
		{
			BSGButton button = this.m_Button;
			button.onClick = (BSGButton.OnClick)Delegate.Remove(button.onClick, new BSGButton.OnClick(this.Button_OnClicked));
		}
	}

	// Token: 0x0400117F RID: 4479
	private BSGButton m_Button;

	// Token: 0x04001180 RID: 4480
	[UIFieldTarget("id_GroupId")]
	private TextMeshProUGUI m_GroupId;

	// Token: 0x04001181 RID: 4481
	private DT.Field m_definition;

	// Token: 0x04001182 RID: 4482
	private bool m_isInitialized;
}
