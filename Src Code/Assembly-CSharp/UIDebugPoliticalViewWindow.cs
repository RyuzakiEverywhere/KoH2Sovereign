using System;
using Logic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200026C RID: 620
public class UIDebugPoliticalViewWindow : UIPoliticalViewFilter
{
	// Token: 0x0600261E RID: 9758 RVA: 0x0014FAB0 File Offset: 0x0014DCB0
	private void PopulateModes()
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (this.m_Dropdown == null)
		{
			this.m_Dropdown = global::Common.FindChildComponent<BSG_TMP_DropDown>(base.gameObject, "id_Dropdown");
			if (this.m_Dropdown == null)
			{
				return;
			}
			this.m_Dropdown.onValueChanged.AddListener(new UnityAction<int>(this.HandleOnValueChange));
			global::Defs.OnDefsProcessedEvent = (Action)Delegate.Combine(global::Defs.OnDefsProcessedEvent, new Action(this.PopulateModes));
		}
		this.m_Dropdown.ClearOptions();
		DebugPoliticalView debugView = ViewMode.DebugView;
		if (!debugView.IsActive())
		{
			return;
		}
		for (int i = 0; i < debugView.modes.Count; i++)
		{
			DebugPoliticalView.Mode mode = debugView.modes[i];
			this.m_Dropdown.options.Add(new TMP_Dropdown.OptionData(mode.field.key));
			string key = mode.field.key;
			DebugPoliticalView.Mode cur_mode = debugView.cur_mode;
			string b;
			if (cur_mode == null)
			{
				b = null;
			}
			else
			{
				DT.Field field = cur_mode.field;
				b = ((field != null) ? field.key : null);
			}
			if (key == b)
			{
				this.m_Dropdown.SetValueWithoutNotify(i);
			}
		}
	}

	// Token: 0x0600261F RID: 9759 RVA: 0x0014FBD4 File Offset: 0x0014DDD4
	private void OnDestroy()
	{
		global::Defs.OnDefsProcessedEvent = (Action)Delegate.Remove(global::Defs.OnDefsProcessedEvent, new Action(this.PopulateModes));
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x0014FBF8 File Offset: 0x0014DDF8
	private void HandleOnValueChange(int idx)
	{
		DebugPoliticalView debugView = ViewMode.DebugView;
		if (!debugView.IsActive())
		{
			return;
		}
		if (idx < 0 || idx >= debugView.modes.Count)
		{
			return;
		}
		debugView.cur_mode = debugView.modes[idx];
		debugView.Apply();
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x0014FC3F File Offset: 0x0014DE3F
	protected override void SpawnFilterButtons()
	{
		this.PopulateModes();
	}

	// Token: 0x06002622 RID: 9762 RVA: 0x000023FD File Offset: 0x000005FD
	public override void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e)
	{
	}

	// Token: 0x040019CB RID: 6603
	private BSG_TMP_DropDown m_Dropdown;
}
