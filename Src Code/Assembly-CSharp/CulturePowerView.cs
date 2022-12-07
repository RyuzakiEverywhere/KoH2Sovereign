using System;
using Logic;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class CulturePowerView : PoliticalView
{
	// Token: 0x06000B97 RID: 2967 RVA: 0x0008269C File Offset: 0x0008089C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.color_loyal_to_selection = global::Defs.GetColor(this.def_id, "color_loyal_to_selection", Color.clear);
		this.color_loyal_to_selection_outside = global::Defs.GetColor(this.def_id, "color_loyal_to_selection_outside", Color.clear);
		this.color_unloyal = global::Defs.GetColor(this.def_id, "color_unloyal", Color.clear);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x00082701 File Offset: 0x00080901
	protected override void OnActivate()
	{
		this.debug = (UICommon.GetKey(KeyCode.LeftShift, false) && Game.CheckCheatLevel(Game.CheatLevel.Low, "Debug view mode", true));
		base.OnActivate();
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0008272C File Offset: 0x0008092C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (!Application.isPlaying)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		global::Settlement settlement;
		if (worldUI == null)
		{
			settlement = null;
		}
		else
		{
			GameObject selected_obj = worldUI.selected_obj;
			settlement = ((selected_obj != null) ? selected_obj.GetComponent<global::Settlement>() : null);
		}
		global::Settlement settlement2 = settlement;
		global::Realm realm = global::Realm.Get((settlement2 == null) ? 0 : settlement2.GetRealmID());
		Logic.Realm realm2 = (realm != null) ? realm.logic : null;
		Logic.Kingdom kingdom;
		if ((kingdom = ((realm2 != null) ? realm2.GetKingdom() : null)) == null)
		{
			global::Kingdom kingdom2 = global::Kingdom.Get(this.wm.SrcKingdom);
			kingdom = ((kingdom2 != null) ? kingdom2.logic : null);
		}
		Logic.Kingdom kingdom3 = kingdom;
		for (int i = 0; i < this.wm.Realms.Count; i++)
		{
			global::Realm realm3 = this.wm.Realms[i];
			Logic.Realm realm4 = (realm3 != null) ? realm3.logic : null;
			Logic.Kingdom kingdom4 = (realm4 != null) ? realm4.GetKingdom() : null;
			if (kingdom4 == null)
			{
				this.SetRealmColor(i + 1, Color.clear);
			}
			else
			{
				Color color = this.color_none;
				float b = 1f;
				if (this.debug)
				{
					float num2;
					float num3;
					Logic.Kingdom kingdom5;
					float num = realm4.CalcPopInf(out num2, out num3, out kingdom5, null);
					if (realm4.pop_majority.kingdom == kingdom4)
					{
						if (num >= 0f)
						{
							color = ((realm4.pop_majority.strength >= 100f) ? Color.clear : Color.green);
						}
						else
						{
							color = Color.yellow;
						}
					}
					else if (num < 0f)
					{
						color = ((kingdom5 == kingdom4) ? (Color.red + Color.yellow) : Color.cyan);
					}
					else
					{
						color = Color.red;
					}
				}
				else
				{
					b = realm4.pop_majority.strength / 100f;
					if (realm4.pop_majority.kingdom == kingdom3)
					{
						color = ((kingdom4 == kingdom3) ? this.color_loyal_to_selection : this.color_loyal_to_selection_outside);
					}
					else if (kingdom4 == kingdom3)
					{
						color = this.color_unloyal;
					}
				}
				color *= b;
				this.SetRealmColor(i + 1, color);
			}
		}
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00082920 File Offset: 0x00080B20
	public override bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt != Tooltip.Event.Fill && evt != Tooltip.Event.Update)
		{
			return base.HandleTooltip(ui, tooltip, evt);
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return false;
		}
		global::Kingdom kingdom = global::Kingdom.Get(worldMap.SrcKingdom);
		if (kingdom == null || kingdom.logic == null)
		{
			return false;
		}
		global::Realm realm = global::Realm.Get(worldMap.highlighted_realm);
		if (realm == null || realm.logic == null || realm.IsSeaRealm())
		{
			tooltip.SetText("", null, null);
			return false;
		}
		Vars vars = tooltip.vars;
		if (vars == null)
		{
			vars = new Vars(realm.logic);
		}
		else
		{
			vars.obj = realm.logic;
		}
		tooltip.SetDef("CulturePowerTooltip", vars);
		return false;
	}

	// Token: 0x04000907 RID: 2311
	public bool debug;

	// Token: 0x04000908 RID: 2312
	private Color color_loyal_to_selection;

	// Token: 0x04000909 RID: 2313
	private Color color_loyal_to_selection_outside;

	// Token: 0x0400090A RID: 2314
	private Color color_unloyal;
}
