using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class ProvinceFeaturesView : PoliticalView
{
	// Token: 0x06000BC0 RID: 3008 RVA: 0x00084230 File Offset: 0x00082430
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.highlight = global::Defs.GetColor(this.def_id, "highlighted_color", Color.clear);
		this.highlight_partial = global::Defs.GetColor(this.def_id, "highlighted_color_partial", Color.clear);
		this.highlighted_color_selected = global::Defs.GetColor(this.def_id, "highlighted_color_selected", Color.clear);
		this.highlighted_color_player = global::Defs.GetColor(this.def_id, "highlighted_color_player", Color.clear);
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x000842B0 File Offset: 0x000824B0
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.selectedFeatures == null)
		{
			return;
		}
		if (GameLogic.Get(true) == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Realm selecteRelam = base.GetSelecteRelam();
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			if (realm.logic != null && realm.logic.castle != null)
			{
				List<string> features = realm.logic.features;
				bool flag = this.selectedFeatures != null && this.selectedFeatures.Count > 0;
				Color color = (realm.kingdom == kingdom.id) ? this.highlighted_color_player : this.color_none;
				if (!flag)
				{
					color = ((selecteRelam != null && selecteRelam.id == i) ? this.highlighted_color_selected : color);
				}
				else
				{
					int num = 0;
					for (int j = 0; j < features.Count; j++)
					{
						if (this.selectedFeatures.IndexOf(features[j]) >= 0)
						{
							num++;
						}
					}
					if (num == this.selectedFeatures.Count)
					{
						color = this.highlight;
					}
					else if (num > 0)
					{
						color = this.highlight_partial;
					}
				}
				if (realm.kingdom == kingdom.id || !flag)
				{
					this.SetRealmColor(i, color);
				}
				else
				{
					this.redChannel[i] = this.color_none;
					this.greenChannel[i] = color;
					this.blueChannel[i] = this.color_none;
				}
			}
		}
	}

	// Token: 0x0400092B RID: 2347
	private Color highlight;

	// Token: 0x0400092C RID: 2348
	private Color highlight_partial;

	// Token: 0x0400092D RID: 2349
	private Color highlighted_color_selected;

	// Token: 0x0400092E RID: 2350
	private Color highlighted_color_player;

	// Token: 0x0400092F RID: 2351
	public List<string> selectedFeatures = new List<string>();
}
