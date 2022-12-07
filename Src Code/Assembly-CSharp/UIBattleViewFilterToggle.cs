using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001C9 RID: 457
public class UIBattleViewFilterToggle : MonoBehaviour
{
	// Token: 0x06001AEB RID: 6891 RVA: 0x001037FD File Offset: 0x001019FD
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x00103805 File Offset: 0x00101A05
	public void Set(DT.Field definition, FiltersEventHub filtersEventHub)
	{
		this.m_definition = definition;
		this.m_filtersEventHub = filtersEventHub;
		this.Initialize();
		if (this.m_filtersEventHub == null)
		{
			return;
		}
		this.SetUp();
		this.Refresh();
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x00103830 File Offset: 0x00101A30
	public void SetOn(bool isOn, bool callEvent = false)
	{
		this.m_callEvent = callEvent;
		this.m_Toggle.isOn = isOn;
		this.m_callEvent = true;
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x0010384C File Offset: 0x00101A4C
	public bool IsSelected()
	{
		return this.m_Toggle != null && this.m_Toggle.isOn;
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x0010386C File Offset: 0x00101A6C
	private void Initialize()
	{
		if (this.m_isInitialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Toggle != null)
		{
			this.m_Toggle.onValueChanged.AddListener(new UnityAction<bool>(this.Toggle_OnClick));
		}
		this.m_isInitialized = true;
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x001038BC File Offset: 0x00101ABC
	private void SetUp()
	{
		this.m_eventKey = this.m_definition.GetString("event_key", null, "", true, true, true, '.');
		this.m_Toggle.isOn = BattleViewPreferences.GetPreference(this.m_eventKey, false);
		UIText.SetText(this.m_CommandLabel, global::Defs.Localize(this.m_definition, "label", null, null, true, true));
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x00103920 File Offset: 0x00101B20
	private void Refresh()
	{
		this.m_CommandLabel.color = global::Defs.GetColor(this.m_definition, this.m_Toggle.isOn ? "selected_color" : "not_selected_color", null);
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x00103954 File Offset: 0x00101B54
	private void Toggle_OnClick(bool value)
	{
		if (this.m_filtersEventHub == null)
		{
			return;
		}
		this.Refresh();
		if (this.m_callEvent)
		{
			this.m_filtersEventHub.CallEvent(this.m_eventKey, this.m_Toggle.isOn);
		}
		BattleViewPreferences.SavePreference(this.m_eventKey, this.m_Toggle.isOn);
	}

	// Token: 0x04001189 RID: 4489
	[UIFieldTarget("id_CommandLabel")]
	private TextMeshProUGUI m_CommandLabel;

	// Token: 0x0400118A RID: 4490
	[UIFieldTarget("id_Toggle")]
	private Toggle m_Toggle;

	// Token: 0x0400118B RID: 4491
	private DT.Field m_definition;

	// Token: 0x0400118C RID: 4492
	private bool m_isInitialized;

	// Token: 0x0400118D RID: 4493
	private FiltersEventHub m_filtersEventHub;

	// Token: 0x0400118E RID: 4494
	private string m_eventKey;

	// Token: 0x0400118F RID: 4495
	private bool m_callEvent = true;
}
