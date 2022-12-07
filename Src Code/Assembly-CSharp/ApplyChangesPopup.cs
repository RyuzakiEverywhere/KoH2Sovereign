using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class ApplyChangesPopup : MonoBehaviour
{
	// Token: 0x0600263B RID: 9787 RVA: 0x001502E7 File Offset: 0x0014E4E7
	public void DoWhenVisible(Action action = null)
	{
		this.actions.Add(action);
		this.wait_for_frames = 1;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x00150308 File Offset: 0x0014E508
	protected virtual void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, true);
		this.m_Initialzied = true;
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x00150321 File Offset: 0x0014E521
	private void OnEnable()
	{
		this.Init();
		if (this.m_text != null)
		{
			UIText.SetTextKey(this.m_text, "Preferences.applying_changes", null, null);
		}
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x0015034C File Offset: 0x0014E54C
	private void Update()
	{
		int num;
		if (this.actions.Count != 0)
		{
			if (this.wait_for_frames > 0)
			{
				num = this.wait_for_frames - 1;
				this.wait_for_frames = num;
				if (num > 0)
				{
					return;
				}
			}
			this.ExecuteAllActions();
			this.wait_for_frames = 1;
			return;
		}
		if (this.wait_for_frames == 0)
		{
			return;
		}
		num = this.wait_for_frames - 1;
		this.wait_for_frames = num;
		if (num > 0)
		{
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x001503BC File Offset: 0x0014E5BC
	private void ExecuteAllActions()
	{
		while (this.actions.Count > 0)
		{
			Action action = this.actions[this.actions.Count - 1];
			this.actions.RemoveAt(this.actions.Count - 1);
			if (action != null)
			{
				action();
			}
		}
	}

	// Token: 0x040019D6 RID: 6614
	[UIFieldTarget("id_ApplyingChangesText")]
	protected TextMeshProUGUI m_text;

	// Token: 0x040019D7 RID: 6615
	private bool m_Initialzied;

	// Token: 0x040019D8 RID: 6616
	private List<Action> actions = new List<Action>();

	// Token: 0x040019D9 RID: 6617
	private int wait_for_frames;
}
