using System;
using System.Collections.Generic;

// Token: 0x020001BC RID: 444
public class ButtonsGroup
{
	// Token: 0x06001A53 RID: 6739 RVA: 0x000FED44 File Offset: 0x000FCF44
	public void AddButton(BSGButton button, bool isSelectable = true)
	{
		if (button == null || this.m_buttons.Contains(button))
		{
			return;
		}
		button.onClick = (BSGButton.OnClick)Delegate.Combine(button.onClick, new BSGButton.OnClick(this.Button_OnClick));
		button.AllowSelection(isSelectable);
		this.m_buttons.Add(button);
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000FEDA0 File Offset: 0x000FCFA0
	public void RemoveButton(BSGButton button)
	{
		if (button == null || !this.m_buttons.Contains(button))
		{
			return;
		}
		button.onClick = (BSGButton.OnClick)Delegate.Remove(button.onClick, new BSGButton.OnClick(this.Button_OnClick));
		if (button == this.m_currentlySelectedButton)
		{
			this.m_currentlySelectedButton = null;
		}
		this.m_buttons.Remove(button);
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000FEE09 File Offset: 0x000FD009
	public void AllowSwitchOff(bool isSwitchOffAllowed)
	{
		this.m_isSwitchOffAllowed = isSwitchOffAllowed;
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x000FEE14 File Offset: 0x000FD014
	public void Refresh()
	{
		foreach (BSGButton bsgbutton in this.m_buttons)
		{
			if (bsgbutton.IsSelected())
			{
				this.m_currentlySelectedButton = bsgbutton;
				break;
			}
		}
		bool flag = false;
		foreach (BSGButton bsgbutton2 in this.m_buttons)
		{
			if (bsgbutton2.IsSelected() && bsgbutton2 != this.m_currentlySelectedButton)
			{
				bsgbutton2.SetSelected(false, false);
				ButtonsGroup.OnSelected onSelected = this.onButtonSelected;
				if (onSelected != null)
				{
					onSelected(this.m_currentlySelectedButton);
				}
			}
			if (bsgbutton2.IsSelected())
			{
				flag = true;
			}
		}
		if (!flag)
		{
			this.m_currentlySelectedButton = null;
		}
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x000FEEF8 File Offset: 0x000FD0F8
	public void Clear()
	{
		for (int i = 0; i < this.m_buttons.Count; i++)
		{
			if (!(this.m_buttons[i] == null))
			{
				BSGButton bsgbutton = this.m_buttons[i];
				bsgbutton.onClick = (BSGButton.OnClick)Delegate.Remove(bsgbutton.onClick, new BSGButton.OnClick(this.Button_OnClick));
			}
		}
		this.m_buttons.Clear();
		this.m_currentlySelectedButton = null;
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x000FEF6E File Offset: 0x000FD16E
	public void ButtonSelected(BSGButton button)
	{
		this.UpdateSelection(button);
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x000FEF6E File Offset: 0x000FD16E
	private void Button_OnClick(BSGButton button)
	{
		this.UpdateSelection(button);
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x000FEF78 File Offset: 0x000FD178
	private void UpdateSelection(BSGButton button)
	{
		if (this.m_currentlySelectedButton != null && this.m_currentlySelectedButton.IsSelected())
		{
			this.m_currentlySelectedButton.SetSelected(false, false);
			ButtonsGroup.OnSelected onSelected = this.onButtonSelected;
			if (onSelected != null)
			{
				onSelected(this.m_currentlySelectedButton);
			}
		}
		if (this.m_currentlySelectedButton == button && this.m_isSwitchOffAllowed)
		{
			this.m_currentlySelectedButton = null;
		}
		else
		{
			this.m_currentlySelectedButton = button;
			this.m_currentlySelectedButton.SetSelected(true, false);
			ButtonsGroup.OnSelected onSelected2 = this.onButtonSelected;
			if (onSelected2 != null)
			{
				onSelected2(this.m_currentlySelectedButton);
			}
		}
		this.Refresh();
	}

	// Token: 0x040010DE RID: 4318
	public ButtonsGroup.OnSelected onButtonSelected;

	// Token: 0x040010DF RID: 4319
	private bool m_isSwitchOffAllowed;

	// Token: 0x040010E0 RID: 4320
	private List<BSGButton> m_buttons = new List<BSGButton>();

	// Token: 0x040010E1 RID: 4321
	private BSGButton m_currentlySelectedButton;

	// Token: 0x02000719 RID: 1817
	// (Invoke) Token: 0x060049D9 RID: 18905
	public delegate void OnSelected(BSGButton btn);
}
