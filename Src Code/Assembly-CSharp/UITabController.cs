using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class UITabController : MonoBehaviour
{
	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06002F1A RID: 12058 RVA: 0x00183B4C File Offset: 0x00181D4C
	// (set) Token: 0x06002F1B RID: 12059 RVA: 0x00183B54 File Offset: 0x00181D54
	public UITabButton activeTab { get; private set; }

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06002F1C RID: 12060 RVA: 0x00183B5D File Offset: 0x00181D5D
	// (set) Token: 0x06002F1D RID: 12061 RVA: 0x00183B65 File Offset: 0x00181D65
	public ReadOnlyCollection<UITabButton> Tabs { get; private set; }

	// Token: 0x06002F1E RID: 12062 RVA: 0x00183B6E File Offset: 0x00181D6E
	private void Awake()
	{
		this.Tabs = new ReadOnlyCollection<UITabButton>(this.tabs);
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x00183B81 File Offset: 0x00181D81
	public void Add(UITabButton tab)
	{
		if (tab == null)
		{
			return;
		}
		if (this.tabs.Contains(tab))
		{
			return;
		}
		this.tabs.Add(tab);
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x00183BA8 File Offset: 0x00181DA8
	public void Remove(UITabButton tab)
	{
		if (tab == null)
		{
			return;
		}
		this.tabs.Remove(tab);
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x00183BC4 File Offset: 0x00181DC4
	public void Select(UITabButton tab, bool force = false)
	{
		if (tab == null)
		{
			return;
		}
		if (this.activeTab == tab && !force)
		{
			return;
		}
		this.activeTab = tab;
		for (int i = 0; i < this.tabs.Count; i++)
		{
			UITabButton uitabButton = this.tabs[i];
			uitabButton.Select(uitabButton == tab, force);
			if (uitabButton == tab)
			{
				Action<UITabButton> action = this.onSelect;
				if (action != null)
				{
					action(uitabButton);
				}
			}
		}
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x00183C40 File Offset: 0x00181E40
	public void Select(int index)
	{
		if (this.tabs == null)
		{
			return;
		}
		if (index < 0 || index > this.tabs.Count - 1)
		{
			return;
		}
		UITabButton tab = this.tabs[index];
		this.Select(tab, false);
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x00183C80 File Offset: 0x00181E80
	public int GetTabIndex()
	{
		if (this.tabs == null)
		{
			return 0;
		}
		if (this.activeTab == null)
		{
			return 0;
		}
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (this.tabs[i] == this.activeTab)
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x04001FCD RID: 8141
	public RectTransform container;

	// Token: 0x04001FD0 RID: 8144
	public Action<UITabButton> onSelect;

	// Token: 0x04001FD1 RID: 8145
	private List<UITabButton> tabs = new List<UITabButton>();
}
