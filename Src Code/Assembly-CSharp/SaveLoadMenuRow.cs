using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200029A RID: 666
internal class SaveLoadMenuRow : MonoBehaviour
{
	// Token: 0x06002918 RID: 10520 RVA: 0x0015E084 File Offset: 0x0015C284
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.cellTexts == null)
		{
			this.cellTexts = new List<TextMeshProUGUI>();
		}
		if (this.m_SaveName != null)
		{
			this.cellTexts.Add(this.m_SaveName);
		}
		if (this.m_PlayTime != null)
		{
			this.cellTexts.Add(this.m_PlayTime);
		}
		if (this.m_SaveData != null)
		{
			this.cellTexts.Add(this.m_SaveData);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x0015E118 File Offset: 0x0015C318
	public SaveLoadMenuRow(SaveGame.Info info)
	{
		this.Init();
		this.SetSaveInfo(info);
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x0015E138 File Offset: 0x0015C338
	public void SetInfo(SaveGame.Info info)
	{
		this.Init();
		this.info = info;
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x0015E148 File Offset: 0x0015C348
	public void Refresh()
	{
		if (this.info == null)
		{
			return;
		}
		UIText.SetText(this.m_SaveName, this.info.name);
		if (this.m_PlayTime)
		{
			Vars vars = new Vars();
			TimeSpan timeSpan = new TimeSpan((long)this.info.session_time * 10000000L);
			vars.Set<string>("time_hours", "#" + Mathf.FloorToInt((float)((int)timeSpan.TotalHours)).ToString());
			vars.Set<string>("time_minutes", "#" + timeSpan.Minutes.ToString("D2"));
			vars.Set<string>("time_seconds", "#" + timeSpan.Seconds.ToString("D2"));
			UIText.SetText(this.m_PlayTime, global::Defs.Localize("SaveLoadMenuWindow.game_time", vars, null, true, true));
		}
		UIText.SetText(this.m_SaveData, this.info.date_time.ToString());
		if (this.m_ModCompatibilityIcon != null)
		{
			this.m_ModCompatibilityIcon.SetObject(this.info, null);
		}
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x0015E278 File Offset: 0x0015C478
	public void RefreshHeight()
	{
		if (this.cellTexts == null || this.cellTexts.Count == 0)
		{
			return;
		}
		float num = this.cellTexts[0].GetComponent<RectTransform>().rect.height;
		for (int i = 1; i < this.cellTexts.Count; i++)
		{
			num = Mathf.Max(this.cellTexts[i].GetComponent<RectTransform>().rect.height, num);
		}
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.rect.width, num);
		for (int j = 0; j < this.cellTexts.Count; j++)
		{
			RectTransform component2 = this.cellTexts[j].gameObject.transform.parent.parent.parent.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(component2.rect.width, num);
		}
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x0015E372 File Offset: 0x0015C572
	public void SetSaveInfo(SaveGame.Info info)
	{
		this.info = info;
	}

	// Token: 0x04001BD6 RID: 7126
	[UIFieldTarget("id_SaveName")]
	private TextMeshProUGUI m_SaveName;

	// Token: 0x04001BD7 RID: 7127
	[UIFieldTarget("id_Playtime")]
	private TextMeshProUGUI m_PlayTime;

	// Token: 0x04001BD8 RID: 7128
	[UIFieldTarget("id_SaveDate")]
	private TextMeshProUGUI m_SaveData;

	// Token: 0x04001BD9 RID: 7129
	[UIFieldTarget("id_ModCompatibilityIcon")]
	private UIModCompatibilityIcon m_ModCompatibilityIcon;

	// Token: 0x04001BDA RID: 7130
	public SaveGame.Info info;

	// Token: 0x04001BDB RID: 7131
	private List<TextMeshProUGUI> cellTexts = new List<TextMeshProUGUI>();

	// Token: 0x04001BDC RID: 7132
	private bool m_Initialzied;
}
