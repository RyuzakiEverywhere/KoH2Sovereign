using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class DebugPoliticalView : PoliticalView
{
	// Token: 0x06000BA3 RID: 2979 RVA: 0x00082F54 File Offset: 0x00081154
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.modes.Clear();
		DebugPoliticalView.Mode mode = this.cur_mode;
		string text;
		if (mode == null)
		{
			text = null;
		}
		else
		{
			DT.Field field2 = mode.field;
			text = ((field2 != null) ? field2.key : null);
		}
		string b = text;
		this.cur_mode = null;
		if (((field != null) ? field.children : null) == null)
		{
			return;
		}
		DT.Field based_on = field.FindChild("Mode", null, true, true, true, '.');
		for (int i = 0; i < field.children.Count; i++)
		{
			DT.Field field3 = field.children[i];
			if (!(field3.Type() != "mode"))
			{
				if (field3.based_on == null)
				{
					field3.based_on = based_on;
				}
				DebugPoliticalView.Mode mode2 = new DebugPoliticalView.Mode
				{
					field = field3
				};
				this.modes.Add(mode2);
				if (this.cur_mode == null || mode2.field.key == b)
				{
					this.cur_mode = mode2;
				}
			}
		}
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x00083040 File Offset: 0x00081240
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		WorldUI.Get();
		List<global::Realm> realms = this.wm.Realms;
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		for (int i = 0; i < realms.Count; i++)
		{
			Logic.Realm logic = realms[i].logic;
			Color newColor = this.CalcRealmColor(logic);
			this.SetRealmColor(i + 1, newColor);
		}
		Vars.PopReflectionMode(old_mode);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x000830A8 File Offset: 0x000812A8
	private void SetVars(Logic.Realm r)
	{
		this.vars.obj = r;
		Logic.Kingdom kingdom = (r != null) ? r.GetKingdom() : null;
		this.vars.Set<Logic.Kingdom>("kingdom", kingdom);
		this.vars.Set<Logic.Realm>("realm", r);
		WorldUI worldUI = WorldUI.Get();
		Logic.Kingdom kingdom2;
		if (worldUI == null)
		{
			kingdom2 = null;
		}
		else
		{
			global::Kingdom selected_kingdom = worldUI.selected_kingdom;
			kingdom2 = ((selected_kingdom != null) ? selected_kingdom.logic : null);
		}
		Logic.Kingdom kingdom3 = kingdom2;
		Vars vars = this.vars;
		string key = "mode";
		string str = "#";
		DebugPoliticalView.Mode mode = this.cur_mode;
		string str2;
		if (mode == null)
		{
			str2 = null;
		}
		else
		{
			DT.Field field = mode.field;
			str2 = ((field != null) ? field.key : null);
		}
		vars.Set<string>(key, str + str2);
		this.vars.Set<Logic.Kingdom>("plr_kingdom", BaseUI.LogicKingdom());
		this.vars.Set<Logic.Kingdom>("sel_kingdom", kingdom3);
		Logic.Settlement settlement = worldUI.selected_logic_obj as Logic.Settlement;
		Logic.Realm realm = (settlement != null) ? settlement.GetRealm() : null;
		this.vars.Set<Logic.Realm>("sel_realm", realm);
		Logic.Realm realm3;
		if (!(this.wm == null))
		{
			global::Realm realm2 = global::Realm.Get(this.wm.highlighted_realm);
			realm3 = ((realm2 != null) ? realm2.logic : null);
		}
		else
		{
			realm3 = null;
		}
		Logic.Realm realm4 = realm3;
		this.vars.Set<Logic.Realm>("highlighted_realm", realm4);
		Logic.Kingdom kingdom4 = (realm4 != null) ? realm4.GetKingdom() : null;
		this.vars.Set<Logic.Kingdom>("highlighted_kingdom", kingdom4);
		this.vars.Set<bool>("is_selected_realm", realm == r);
		this.vars.Set<bool>("is_selected_kingdom", kingdom3 == kingdom);
		this.vars.Set<bool>("is_highlighted_realm", realm4 == r);
		this.vars.Set<bool>("is_highlighted_kingdom", kingdom4 == kingdom);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0008324C File Offset: 0x0008144C
	private Color CalcRealmColor(Logic.Realm r)
	{
		DebugPoliticalView.Mode mode = this.cur_mode;
		DT.Field field = (mode != null) ? mode.field : null;
		if (field == null)
		{
			return Color.black;
		}
		this.SetVars(r);
		Color color = global::Defs.GetColor(field, "color", Color.clear, this.vars);
		if (color != Color.clear)
		{
			return color;
		}
		float num = field.Float(this.vars, 0f);
		this.vars.Set<float>("value", num);
		float num2 = field.GetFloat("min_val", this.vars, -1f, true, true, true, '.');
		this.vars.Set<float>("min_val", num2);
		float num3 = field.GetFloat("max_val", this.vars, 1f, true, true, true, '.');
		this.vars.Set<float>("max_val", num3);
		float @float = field.GetFloat("mid_val", this.vars, 0f, true, true, true, '.');
		this.vars.Set<float>("mid_val", @float);
		if (num2 > num3)
		{
			float num4 = num3;
			num3 = num2;
			num2 = num4;
			num = num3 - (num - num2);
		}
		if (num < num2)
		{
			num = num2;
		}
		else if (num > num3)
		{
			num = num3;
		}
		Color color2 = global::Defs.GetColor(field, "clr_min", Color.red, this.vars);
		Color color3 = global::Defs.GetColor(field, "clr_mid", Color.clear, this.vars);
		Color color4 = global::Defs.GetColor(field, "clr_max", Color.green, this.vars);
		if (num <= @float)
		{
			color = Color.Lerp(color2, color3, (num - num2) / (@float - num2));
		}
		else
		{
			color = Color.Lerp(color3, color4, (num - @float) / (num3 - @float));
		}
		return color;
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x000833E8 File Offset: 0x000815E8
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
		global::Realm realm = global::Realm.Get(worldMap.highlighted_realm);
		Logic.Realm realm2 = (realm != null) ? realm.logic : null;
		if (realm2 != null)
		{
			DebugPoliticalView.Mode mode = this.cur_mode;
			if (((mode != null) ? mode.field : null) != null)
			{
				this.SetVars(realm2);
				Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
				float val = this.cur_mode.field.Float(this.vars, 0f);
				this.vars.Set<float>("value", val);
				string text = this.cur_mode.field.GetString("tooltip", this.vars, "", true, true, true, '.');
				if (string.IsNullOrEmpty(text))
				{
					text = val.ToString();
				}
				string @string = this.cur_mode.field.GetString("tooltip_caption", this.vars, "{realm} ({kingdom})", true, true, true, '.');
				tooltip.SetText("@" + text, "@" + @string, this.vars);
				Vars.PopReflectionMode(old_mode);
			}
		}
		return false;
	}

	// Token: 0x04000914 RID: 2324
	public List<DebugPoliticalView.Mode> modes = new List<DebugPoliticalView.Mode>();

	// Token: 0x04000915 RID: 2325
	public DebugPoliticalView.Mode cur_mode;

	// Token: 0x04000916 RID: 2326
	public Vars vars = new Vars();

	// Token: 0x02000604 RID: 1540
	public class Mode
	{
		// Token: 0x060046A5 RID: 18085 RVA: 0x0020F85B File Offset: 0x0020DA5B
		public override string ToString()
		{
			DT.Field field = this.field;
			if (field == null)
			{
				return null;
			}
			return field.key;
		}

		// Token: 0x04003383 RID: 13187
		public DT.Field field;
	}
}
