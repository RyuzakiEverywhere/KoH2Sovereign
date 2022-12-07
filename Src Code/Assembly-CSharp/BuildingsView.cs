using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class BuildingsView : PoliticalView
{
	// Token: 0x06000B91 RID: 2961 RVA: 0x00082378 File Offset: 0x00080578
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.highlight = global::Defs.GetColor(this.def_id, "highlighted_color", Color.clear);
		this.highlight_partial = global::Defs.GetColor(this.def_id, "highlighted_color_partial", Color.clear);
		this.highlighted_color_selected = global::Defs.GetColor(this.def_id, "highlighted_color_selected", Color.clear);
		this.highlighted_color_player = global::Defs.GetColor(this.def_id, "highlighted_color_player", Color.clear);
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x000823F8 File Offset: 0x000805F8
	public override void OnApply(bool secondary)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		base.OnApply(secondary);
		if (this.selectedBuildings == null)
		{
			return;
		}
		List<global::Realm> realms = this.wm.Realms;
		if (realms == null)
		{
			return;
		}
		WorldUI.Get();
		Logic.Realm selecteRelam = base.GetSelecteRelam();
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Game game = GameLogic.Get(false);
		for (int i = 1; i <= realms.Count; i++)
		{
			global::Realm realm = realms[i - 1];
			Castle castle;
			if (realm == null)
			{
				castle = null;
			}
			else
			{
				Logic.Realm logic = realm.logic;
				castle = ((logic != null) ? logic.castle : null);
			}
			Castle castle2 = castle;
			if (castle2 != null)
			{
				Color color = (realm.kingdom == kingdom.id) ? this.highlighted_color_player : this.color_none;
				Color color2 = color;
				bool flag = selecteRelam != null && selecteRelam.id == realm.id;
				int num = 0;
				int num2 = 0;
				if (game != null)
				{
					for (int j = 0; j < this.selectedBuildings.Count; j++)
					{
						Building.Def def = this.selectedBuildings[j];
						if (castle2.MayBuildBuilding(def, true))
						{
							num2++;
						}
						if (castle2.HasBuilding(def))
						{
							num++;
						}
					}
				}
				if (this.selectedBuildings.Count > 0)
				{
					if (num2 == this.selectedBuildings.Count)
					{
						color = this.highlight;
					}
					else if (num2 > 0)
					{
						color = this.highlight_partial;
					}
					if (num == this.selectedBuildings.Count)
					{
						color2 = this.highlight;
					}
					else if (num > 0)
					{
						color2 = this.highlight_partial;
					}
				}
				else
				{
					color = (flag ? this.highlighted_color_selected : color);
					color2 = color;
				}
				this.redChannel[i] = color;
				this.greenChannel[i] = color2;
				this.blueChannel[i] = color;
			}
		}
	}

	// Token: 0x04000901 RID: 2305
	private Color highlight;

	// Token: 0x04000902 RID: 2306
	private Color highlight_partial;

	// Token: 0x04000903 RID: 2307
	private Color highlighted_color_selected;

	// Token: 0x04000904 RID: 2308
	private Color highlighted_color_player;

	// Token: 0x04000905 RID: 2309
	public List<Building.Def> selectedBuildings = new List<Building.Def>();
}
