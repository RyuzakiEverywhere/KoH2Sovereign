using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class ProducedResourcesView : PoliticalView
{
	// Token: 0x06000BBD RID: 3005 RVA: 0x00084008 File Offset: 0x00082208
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.highlight = global::Defs.GetColor(this.def_id, "highlighted_color", Color.clear);
		this.highlight_partial = global::Defs.GetColor(this.def_id, "highlighted_color_partial", Color.clear);
		this.highlighted_color_selected = global::Defs.GetColor(this.def_id, "highlighted_color_selected", Color.clear);
		this.highlighted_color_player = global::Defs.GetColor(this.def_id, "highlighted_color_player", Color.clear);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00084088 File Offset: 0x00082288
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.selectedGoods == null)
		{
			return;
		}
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		WorldUI.Get();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			if (realm.logic != null && realm.logic.castle != null)
			{
				Color color = (realm.kingdom == kingdom.id) ? this.highlighted_color_player : this.color_none;
				Color color2 = color;
				if (this.selectedGoods.Count != 0)
				{
					realm.logic.GetKingdom();
					int num = 0;
					int num2 = 0;
					for (int j = 0; j < this.selectedGoods.Count; j++)
					{
						ResourceInfo.Availability availability = realm.logic.castle.GetResourceInfo(this.selectedGoods[j], true, true).availability;
						if (availability == ResourceInfo.Availability.Available)
						{
							num++;
							num2++;
						}
						else if (availability == ResourceInfo.Availability.DirectlyObtainable || availability == ResourceInfo.Availability.IndirectlyObtainable)
						{
							num2++;
						}
					}
					if (num == this.selectedGoods.Count)
					{
						color = this.highlight;
					}
					else if (num > 0)
					{
						color = this.highlight_partial;
					}
					if (num2 == this.selectedGoods.Count)
					{
						color2 = this.highlight;
					}
					else if (num2 > 0)
					{
						color2 = this.highlight_partial;
					}
				}
				this.redChannel[i] = color2;
				this.greenChannel[i] = color;
				this.blueChannel[i] = color2;
			}
		}
	}

	// Token: 0x04000926 RID: 2342
	private Color highlight;

	// Token: 0x04000927 RID: 2343
	private Color highlight_partial;

	// Token: 0x04000928 RID: 2344
	private Color highlighted_color_selected;

	// Token: 0x04000929 RID: 2345
	private Color highlighted_color_player;

	// Token: 0x0400092A RID: 2346
	public List<string> selectedGoods = new List<string>();
}
