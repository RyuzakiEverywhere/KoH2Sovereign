using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200026A RID: 618
public class RebellionRiskWindow : UIPoliticalViewFilter
{
	// Token: 0x06002614 RID: 9748 RVA: 0x0014F684 File Offset: 0x0014D884
	public override void SelectFilter(PoliticalViewFilterIcon icon, PointerEventData e)
	{
		RebellionRiskView rebellionRiskView = ViewMode.Get("RebellionRisk") as RebellionRiskView;
		if (!rebellionRiskView.IsActive())
		{
			return;
		}
		if (icon.DataDef == null)
		{
			return;
		}
		RebellionRiskCategory.Def def = icon.DataDef as RebellionRiskCategory.Def;
		if (rebellionRiskView.selectedIndex == def.total_index)
		{
			rebellionRiskView.selectedIndex = -1;
		}
		else
		{
			rebellionRiskView.selectedIndex = def.total_index;
		}
		this.ResetButtons(rebellionRiskView.selectedIndex);
		rebellionRiskView.Apply();
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x0014F6F4 File Offset: 0x0014D8F4
	private void ResetButtons(int s_index)
	{
		PoliticalViewFilterIcon[] componentsInChildren = this.buttons.GetComponentsInChildren<PoliticalViewFilterIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ChangeState(i == s_index + 1);
		}
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x0014F72C File Offset: 0x0014D92C
	protected override void SpawnFilterButtons()
	{
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		RebellionRiskView rebellionRiskView = ViewMode.Get("RebellionRisk") as RebellionRiskView;
		if (!rebellionRiskView.IsActive())
		{
			return;
		}
		List<RebellionRiskCategory.Def> risk_categories = rebellionRiskView.risk_categories;
		GameObject obj = global::Defs.GetObj<GameObject>("RebellionRiskViewScreen", "buttons.icon_prefab", null);
		for (int i = 0; i < risk_categories.Count; i++)
		{
			RebellionRiskCategory.Def def = risk_categories[i];
			if (def != null)
			{
				if (obj == null)
				{
					return;
				}
				GameObject gameObject = global::Common.Spawn(obj, this.buttons.transform, false, "");
				gameObject.name = def.field.key;
				PoliticalViewFilterIcon component = gameObject.GetComponent<PoliticalViewFilterIcon>();
				if (component)
				{
					component.DataDef = def;
					component.ButtonDef = this.def;
				}
				if (rebellionRiskView.selectedIndex == def.total_index)
				{
					this.selected.Add(component);
					component.ChangeState(true);
				}
				else
				{
					component.ChangeState(false);
				}
			}
		}
	}

	// Token: 0x040019C9 RID: 6601
	[UIFieldTarget("id_buttonContainer")]
	protected GameObject buttonContainer;
}
