using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001DD RID: 477
[RequireComponent(typeof(RectTransform))]
public class UISquadCounters : MonoBehaviour
{
	// Token: 0x06001C53 RID: 7251 RVA: 0x0010B848 File Offset: 0x00109A48
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		this.m_definition = global::Defs.GetDefField("UISquadCounters", null);
		if (this.m_rectTransform == null)
		{
			this.m_rectTransform = base.GetComponent<RectTransform>();
		}
		if (this.m_icons == null || this.m_icons.Count == 0)
		{
			this.m_icons = new List<UIBattleViewCounterIcon>(base.GetComponentsInChildren<UIBattleViewCounterIcon>());
			this.HideAll();
		}
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x0010B8B3 File Offset: 0x00109AB3
	public void SetSquad(BattleSimulation.Squad logic)
	{
		this.m_logic = logic;
		this.Initialize();
		this.Refresh();
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0010B8C8 File Offset: 0x00109AC8
	private void Refresh()
	{
		this.HideAll();
		this.m_counterIconPrefab = global::Defs.GetObj<GameObject>(this.m_definition.FindChild("icon_prefab", null, true, true, true, '.'), null);
		List<UISquadCounters.IconData> list = new List<UISquadCounters.IconData>();
		if (this.m_logic.def.counter != null)
		{
			foreach (string unit_type_name in this.m_logic.def.counter)
			{
				list.Add(new UISquadCounters.IconData
				{
					positive = true,
					unit_type_name = unit_type_name
				});
			}
		}
		if (this.m_logic.def.countered != null)
		{
			foreach (string unit_type_name2 in this.m_logic.def.countered)
			{
				list.Add(new UISquadCounters.IconData
				{
					positive = false,
					unit_type_name = unit_type_name2
				});
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (this.m_icons.Count <= i && !this.SpawnNewIcon())
			{
				return;
			}
			UIBattleViewCounterIcon uibattleViewCounterIcon = this.m_icons[i];
			UISquadCounters.IconData iconData = list[i];
			Sprite obj = global::Defs.GetObj<Sprite>(this.m_definition.FindChild(iconData.unit_type_name, null, true, true, true, '.').FindChild("icon", null, true, true, true, '.'), null);
			Sprite obj2;
			if (iconData.positive)
			{
				obj2 = global::Defs.GetObj<Sprite>(this.m_definition.FindChild("positive_background", null, true, true, true, '.'), null);
			}
			else
			{
				obj2 = global::Defs.GetObj<Sprite>(this.m_definition.FindChild("negative_background", null, true, true, true, '.'), null);
			}
			uibattleViewCounterIcon.SetUp(iconData.unit_type_name, iconData.positive, obj2, obj);
			uibattleViewCounterIcon.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x0010BADC File Offset: 0x00109CDC
	private void HideAll()
	{
		foreach (UIBattleViewCounterIcon uibattleViewCounterIcon in this.m_icons)
		{
			uibattleViewCounterIcon.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x0010BB34 File Offset: 0x00109D34
	private bool SpawnNewIcon()
	{
		UIBattleViewCounterIcon component = UnityEngine.Object.Instantiate<GameObject>(this.m_counterIconPrefab, this.m_rectTransform).GetComponent<UIBattleViewCounterIcon>();
		if (component != null)
		{
			this.m_icons.Add(component);
			return true;
		}
		return false;
	}

	// Token: 0x04001289 RID: 4745
	private List<UIBattleViewCounterIcon> m_icons = new List<UIBattleViewCounterIcon>();

	// Token: 0x0400128A RID: 4746
	private DT.Field m_definition;

	// Token: 0x0400128B RID: 4747
	private BattleSimulation.Squad m_logic;

	// Token: 0x0400128C RID: 4748
	private RectTransform m_rectTransform;

	// Token: 0x0400128D RID: 4749
	private GameObject m_counterIconPrefab;

	// Token: 0x0200071C RID: 1820
	private struct IconData
	{
		// Token: 0x0400385F RID: 14431
		public bool positive;

		// Token: 0x04003860 RID: 14432
		public string unit_type_name;
	}
}
