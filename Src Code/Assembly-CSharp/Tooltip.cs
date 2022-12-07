using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class Tooltip : MonoBehaviour, IVars
{
	// Token: 0x06000612 RID: 1554 RVA: 0x00042068 File Offset: 0x00040268
	public static Tooltip Get(GameObject obj, bool bCreateIfNeeded = true)
	{
		if (obj == null)
		{
			return null;
		}
		Tooltip tooltip = obj.GetComponent<Tooltip>();
		if (tooltip == null)
		{
			if (!bCreateIfNeeded)
			{
				return null;
			}
			tooltip = obj.AddComponent<Tooltip>();
		}
		return tooltip;
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x000420A0 File Offset: 0x000402A0
	public static Tooltip FindInParents(Transform t)
	{
		bool active = Tutorial.active;
		while (t != null)
		{
			Tooltip component = t.GetComponent<Tooltip>();
			if (!(component == null) && component.gameObject.activeInHierarchy && (!active || component.tutorial_hotspot_def != null) && (active || !component.is_tutorial_tooltip))
			{
				return component;
			}
			t = t.parent;
		}
		return null;
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x000420FC File Offset: 0x000402FC
	public static Tooltip FindInParents(GameObject go)
	{
		return Tooltip.FindInParents((go != null) ? go.transform : null);
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00042110 File Offset: 0x00040310
	public void Clear(bool resolve_tutorial_hotspot_def = true)
	{
		this.def = null;
		this.TextKey = (this.CaptionKey = null);
		this.text = (this.caption = null);
		this.obj = null;
		if (resolve_tutorial_hotspot_def)
		{
			this.ResolveTutorialHotspotDef();
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00042154 File Offset: 0x00040354
	public void SetObj(object obj, DT.Def def = null, Vars vars = null)
	{
		this.Clear(false);
		if (obj == null)
		{
			return;
		}
		if (def == null)
		{
			def = Tooltip.GetDef(obj, "");
		}
		if (vars == null)
		{
			vars = new Vars(obj);
		}
		this.SetDef(def, vars, obj);
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00042188 File Offset: 0x00040388
	private void FillTooltipVarsFromDef()
	{
		DT.Def def = this.def;
		DT.Field field;
		if (def == null)
		{
			field = null;
		}
		else
		{
			DT.Field field2 = def.field;
			field = ((field2 != null) ? field2.FindChild("tooltip_vars", null, true, true, true, '.') : null);
		}
		DT.Field field3 = field;
		if (field3 == null)
		{
			return;
		}
		List<DT.Field> list = field3.Children();
		if (list == null || list.Count == 0)
		{
			return;
		}
		this.vars = new Vars(this.vars);
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field4 = list[i];
			Value val = field4.Value(this.vars, true, true);
			if (!val.is_unknown)
			{
				this.vars.Set<Value>(field4.key, val);
			}
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00042234 File Offset: 0x00040434
	private void LoadParams(DT.Field f)
	{
		if (f == null)
		{
			return;
		}
		this.delay = f.GetFloat("delay", null, this.delay, true, true, true, '.');
		this.LoadDelay();
		this.min_width = f.GetFloat("min_width", null, this.min_width, true, true, true, '.');
		this.max_width = f.GetFloat("max_width", null, this.max_width, true, true, true, '.');
		this.alignment = f.GetString("alignment", null, this.alignment, true, true, true, '.');
		this.pinable = f.GetInt("pinable", null, this.pinable, true, true, true, '.');
		this.pinable_delay = f.GetFloat("pinable_delay", null, this.pinable_delay, true, true, true, '.');
		this.prefer_aligned = f.GetFloat("prefer_aligned", null, this.prefer_aligned, true, true, true, '.');
		this.prefer_vertical = f.GetFloat("prefer_vertical", null, this.prefer_vertical, true, true, true, '.');
		this.prefer_top = f.GetFloat("prefer_top", null, this.prefer_top, true, true, true, '.');
		this.prefer_right = f.GetFloat("prefer_right", null, this.prefer_right, true, true, true, '.');
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00042370 File Offset: 0x00040570
	private void LoadDelay()
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("tooltip_delay");
		if (setting != null)
		{
			this.delay = setting.value;
		}
		UserSettings.SettingData setting2 = UserSettings.GetSetting("pinable_delay");
		if (setting2 != null)
		{
			this.pinable_delay = setting2.value;
		}
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000423BC File Offset: 0x000405BC
	public void SetDef(DT.Def def, Vars vars = null, object obj = null)
	{
		this.Clear(false);
		if (def == null || def.field == null)
		{
			this.vars = vars;
			this.obj = obj;
			this.RefreshTutorialHotspot(true);
			return;
		}
		DT.Field field = def.field.FindChild("obj", null, true, true, true, '.');
		if (field != null)
		{
			object obj2 = field.Value(vars, true, true).Object(true);
			if (obj2 == null)
			{
				return;
			}
			if (vars != null)
			{
				vars.obj = new Value(obj2);
			}
			obj = obj2;
		}
		if (obj == null && vars != null)
		{
			obj = vars.obj.obj_val;
		}
		this.def = def;
		this.vars = vars;
		this.obj = obj;
		this.FillTooltipVarsFromDef();
		this.LoadParams(def.field);
		this.Refresh();
		this.RefreshTutorialHotspot(true);
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0004247C File Offset: 0x0004067C
	public void SetDef(string def_id, Vars vars = null)
	{
		this.Clear(false);
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return;
		}
		DT.Def def = defs.dt.FindDef(def_id);
		this.SetDef(def, vars, null);
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000424B8 File Offset: 0x000406B8
	public void SetVars(Vars vars)
	{
		this.text = (this.caption = null);
		this.vars = vars;
		if (vars == null)
		{
			return;
		}
		this.delay = vars.Get<float>("delay", this.delay);
		this.min_width = vars.Get<float>("min_width", this.min_width);
		this.max_width = vars.Get<float>("max_width", this.max_width);
		this.alignment = vars.Get<string>("alignment", this.alignment);
		this.Refresh();
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00042544 File Offset: 0x00040744
	public static DT.Def GetDef(object obj, string additional_prefix = "")
	{
		if (obj == null)
		{
			return null;
		}
		DT.Field field = null;
		Def def;
		DT.Field field2;
		if ((def = (obj as Def)) != null)
		{
			field = def.field;
		}
		else if ((field2 = (obj as DT.Field)) != null)
		{
			field = field2;
		}
		if (field != null)
		{
			DT.Def defFromField = Tooltip.GetDefFromField(field, additional_prefix);
			if (defFromField != null)
			{
				return defFromField;
			}
		}
		Game game = GameLogic.Get(true);
		Type type = obj.GetType();
		DT.Def def2;
		for (;;)
		{
			string text = Logic.Object.TypeToStr(type) + additional_prefix + "Tooltip";
			def2 = game.dt.FindDef(text);
			if (def2 != null)
			{
				break;
			}
			type = type.BaseType;
			if (type == null)
			{
				goto Block_7;
			}
		}
		return def2;
		Block_7:
		return null;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x000425D4 File Offset: 0x000407D4
	public static DT.Def GetDefFromField(DT.Field field, string additional_prefix = "")
	{
		Game game = GameLogic.Get(true);
		while (field != null)
		{
			string text = field.Path(false, false, '.') + additional_prefix + "Tooltip";
			DT.Def def = game.dt.FindDef(text);
			if (def != null)
			{
				return def;
			}
			field = field.based_on;
		}
		return null;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00042620 File Offset: 0x00040820
	public string GetCaption()
	{
		if (this.def != null && this.def.field != null)
		{
			return global::Defs.Localize(this.def.field, "caption", this.vars, null, false, true) ?? "";
		}
		if (string.IsNullOrEmpty(this.CaptionKey))
		{
			return "";
		}
		return global::Defs.Localize(this.CaptionKey, this.vars, null, true, true) ?? "";
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0004269C File Offset: 0x0004089C
	public string GetText()
	{
		string form = null;
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "debug-mode tooltip", false))
		{
			form = "alt";
		}
		if (this.def != null && this.def.field != null)
		{
			return global::Defs.Localize(this.def.field, "text", this.vars, form, false, true) ?? "";
		}
		return global::Defs.Localize(this.TextKey, this.vars, form, true, true) ?? "";
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x00042728 File Offset: 0x00040928
	public Sprite GetIcon()
	{
		if (this.vars == null)
		{
			return null;
		}
		if (this.vars.ContainsKey("icon"))
		{
			return this.vars.Get<Sprite>("icon", null);
		}
		return global::Defs.GetObj<Sprite>(this.def.field, "icon", this.vars);
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x00042780 File Offset: 0x00040980
	public bool CallHandler(BaseUI ui, Tooltip.Event evt)
	{
		DT.Field.SetSeed(this.instance);
		this.no_refresh = true;
		bool flag = false;
		if (this.handler != null)
		{
			flag = this.handler(ui, this, evt);
		}
		if (!flag && this.instance != null)
		{
			Tooltip.IHandler component = this.instance.GetComponent<Tooltip.IHandler>();
			if (component != null)
			{
				flag = component.HandleTooltip(ui, this, evt);
			}
		}
		this.no_refresh = false;
		return flag;
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x000427EC File Offset: 0x000409EC
	public bool RecalcTexts()
	{
		string a = this.GetText();
		string a2 = this.GetCaption();
		if (a == this.text && a2 == this.caption)
		{
			return false;
		}
		this.text = a;
		this.caption = a2;
		return true;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00042834 File Offset: 0x00040A34
	public void SetText(string txt, string title = null, Vars vars = null)
	{
		this.Clear(false);
		this.TextKey = txt;
		if (title != null)
		{
			this.CaptionKey = title;
		}
		this.vars = vars;
		this.LoadParams(global::Defs.GetDefField("Tooltip", null));
		this.Refresh();
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0004286C File Offset: 0x00040A6C
	public void Refresh()
	{
		if (this.no_refresh)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.RefreshTooltip(this, false);
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0004289C File Offset: 0x00040A9C
	private void OnDestroy()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null && baseUI.tooltip == this)
		{
			baseUI.DestroyTooltip();
		}
		UserSettings.OnSettingChange -= this.HandleOnSettingsChange;
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000627 RID: 1575 RVA: 0x000428DD File Offset: 0x00040ADD
	// (set) Token: 0x06000628 RID: 1576 RVA: 0x000428E5 File Offset: 0x00040AE5
	public GameObject tutorial_highlight_obj { get; private set; }

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000629 RID: 1577 RVA: 0x000428EE File Offset: 0x00040AEE
	// (set) Token: 0x0600062A RID: 1578 RVA: 0x000428F6 File Offset: 0x00040AF6
	public GameObject tutorial_highlight_prefab { get; private set; }

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x0600062B RID: 1579 RVA: 0x000428FF File Offset: 0x00040AFF
	public Tooltip main_tooltip
	{
		get
		{
			return this.tutorial_tooltip_parent ?? this;
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0004290C File Offset: 0x00040B0C
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		Vars vars2 = this.main_tooltip.vars;
		if (vars2 != null)
		{
			Value var = vars2.GetVar(key, vars, as_value);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
		if (num > 3199239212U)
		{
			if (num <= 3539798275U)
			{
				if (num != 3310976652U)
				{
					if (num != 3343205242U)
					{
						if (num != 3539798275U)
						{
							goto IL_26C;
						}
						if (!(key == "tutorial_hotspot_def"))
						{
							goto IL_26C;
						}
						return new Value(this.tutorial_hotspot_def);
					}
					else
					{
						if (!(key == "obj"))
						{
							goto IL_26C;
						}
						goto IL_1EF;
					}
				}
				else if (!(key == "def"))
				{
					goto IL_26C;
				}
			}
			else if (num != 3540643963U)
			{
				if (num != 3750093428U)
				{
					if (num != 3831535364U)
					{
						goto IL_26C;
					}
					if (!(key == "tooltip_def_id"))
					{
						goto IL_26C;
					}
					goto IL_1D7;
				}
				else if (!(key == "tooltip_def"))
				{
					goto IL_26C;
				}
			}
			else
			{
				if (!(key == "tutorial_tooltip"))
				{
					goto IL_26C;
				}
				return new Value(this.tutorial_tooltip);
			}
			return new Value(this.def);
		}
		if (num <= 1612489561U)
		{
			if (num != 1243594586U)
			{
				if (num != 1514523754U)
				{
					if (num != 1612489561U)
					{
						goto IL_26C;
					}
					if (!(key == "tutorial_hotspot_def_id"))
					{
						goto IL_26C;
					}
					Tutorial.HotspotDef hotspotDef = this.tutorial_hotspot_def;
					return new Value((hotspotDef != null) ? hotspotDef.id : null);
				}
				else
				{
					if (!(key == "seen"))
					{
						goto IL_26C;
					}
					if (this.tutorial_hotspot_def != null)
					{
						return new Value(this.tutorial_hotspot_def.seen);
					}
					return Value.Null;
				}
			}
			else
			{
				if (!(key == "OBJ"))
				{
					goto IL_26C;
				}
				goto IL_1EF;
			}
		}
		else if (num != 2223459638U)
		{
			if (num != 2604199392U)
			{
				if (num != 3199239212U)
				{
					goto IL_26C;
				}
				if (!(key == "def_id"))
				{
					goto IL_26C;
				}
			}
			else
			{
				if (!(key == "main_tooltip"))
				{
					goto IL_26C;
				}
				return new Value(this.tutorial_tooltip_parent);
			}
		}
		else
		{
			if (!(key == "path"))
			{
				goto IL_26C;
			}
			return this.path;
		}
		IL_1D7:
		DT.Def def = this.def;
		return new Value((def != null) ? def.path : null);
		IL_1EF:
		return new Value(this.obj);
		IL_26C:
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			Value var2 = game.GetVar(key, vars, as_value);
			if (!var2.is_unknown)
			{
				return var2;
			}
		}
		return Value.Unknown;
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00042BAC File Offset: 0x00040DAC
	public bool IsWikiLinkTooltip()
	{
		DT.Field field;
		if ((field = (this.obj as DT.Field)) == null)
		{
			return false;
		}
		DT.Field parent = field.parent;
		return !(((parent != null) ? parent.key : null) != "wiki");
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00042BEB File Offset: 0x00040DEB
	private void Awake()
	{
		UserSettings.OnSettingChange += this.HandleOnSettingsChange;
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00042BFE File Offset: 0x00040DFE
	private void OnEnable()
	{
		if (Tooltip.dont_on_enable || this.is_tutorial_tooltip)
		{
			return;
		}
		this.RefreshTutorialHotspot(true);
		Tutorial.OnHotspotEnabled(this);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00042C1D File Offset: 0x00040E1D
	private void OnDisable()
	{
		if (this.is_tutorial_tooltip)
		{
			return;
		}
		Tutorial.OnHotspotDisabled(this);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00042C2E File Offset: 0x00040E2E
	private void HandleOnSettingsChange()
	{
		this.LoadDelay();
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00042C36 File Offset: 0x00040E36
	private void OnTransformParentChanged()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Tutorial.OnHotspotDisabled(this);
		}
		this.RefreshTutorialHotspot(true);
		if (base.gameObject.activeInHierarchy)
		{
			Tutorial.OnHotspotEnabled(this);
		}
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00042C65 File Offset: 0x00040E65
	private void OnTransformChildrenChanged()
	{
		if (this.tutorial_highlight_obj != null && this.tutorial_highlight_obj.transform != null)
		{
			this.tutorial_highlight_obj.transform.SetAsLastSibling();
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00042C98 File Offset: 0x00040E98
	private GameObject DecideHighlightPrefab()
	{
		if (this.is_tutorial_tooltip)
		{
			return null;
		}
		if (this.tutorial_hotspot_def == null)
		{
			return null;
		}
		if (!Tutorial.active)
		{
			bool flag = false;
			if (this.tutorial_hotspot_def.force_visible_field != null)
			{
				Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
				flag = this.tutorial_hotspot_def.force_visible_field.Bool(this, false);
				Vars.PopReflectionMode(old_mode);
			}
			if (!flag && !this.tutorial_hotspot_def.CheckShowGreenHighlighth())
			{
				return null;
			}
		}
		if (this.tutorial_hotspot_def.visible_field != null)
		{
			Vars.ReflectionMode old_mode2 = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
			bool flag2 = this.tutorial_hotspot_def.visible_field.Bool(this, true);
			Vars.PopReflectionMode(old_mode2);
			if (!flag2)
			{
				return null;
			}
		}
		if (this.tutorial_hotspot_def.CheckShowGreenHighlighth())
		{
			return this.tutorial_hotspot_def.highlight_prefab_green;
		}
		if (this.tutorial_hotspot_def.seen)
		{
			return this.tutorial_hotspot_def.highlight_prefab_seen;
		}
		return this.tutorial_hotspot_def.highlight_prefab;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00042D6C File Offset: 0x00040F6C
	private void RefreshHighlight()
	{
		GameObject gameObject = this.DecideHighlightPrefab();
		if (gameObject == null)
		{
			if (this.tutorial_highlight_obj != null)
			{
				Tooltip x = Tooltip.Get(this.tutorial_highlight_obj, false);
				if (x != null)
				{
					UnityEngine.Object.Destroy(x);
				}
				this.tutorial_highlight_obj.SetActive(false);
			}
			return;
		}
		if (gameObject != this.tutorial_highlight_prefab)
		{
			global::Common.DestroyObj(this.tutorial_highlight_obj);
			this.tutorial_highlight_prefab = gameObject;
			this.tutorial_highlight_obj = global::Common.SpawnPooled(this.tutorial_highlight_prefab, base.transform, false, "");
		}
		if (this.tutorial_highlight_obj == null)
		{
			return;
		}
		Tooltip.dont_on_enable = true;
		this.tutorial_tooltip = Tooltip.Get(this.tutorial_highlight_obj, true);
		this.tutorial_tooltip.is_tutorial_tooltip = true;
		this.tutorial_tooltip.tutorial_tooltip_parent = this;
		this.tutorial_tooltip.tutorial_hotspot_def = this.tutorial_hotspot_def;
		Tooltip tooltip = this.tutorial_tooltip;
		Tutorial.HotspotDef hotspotDef = this.tutorial_hotspot_def;
		tooltip.SetDef((hotspotDef != null) ? hotspotDef.tooltip_def : null, null, null);
		Tooltip.dont_on_enable = false;
		this.tutorial_highlight_obj.SetActive(true);
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00042E7E File Offset: 0x0004107E
	public void ResolveTutorialHotspotDef()
	{
		this.tutorial_hotspot_def = Tutorial.ResolveHotspotDef(this);
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00042E8C File Offset: 0x0004108C
	public void RefreshTutorialHotspot(bool resolve_def)
	{
		if (resolve_def)
		{
			this.ResolveTutorialHotspotDef();
		}
		this.RefreshHighlight();
	}

	// Token: 0x040005B3 RID: 1459
	public string CaptionKey = "";

	// Token: 0x040005B4 RID: 1460
	public string TextKey = "";

	// Token: 0x040005B5 RID: 1461
	public string TutorialHotspotDef = "";

	// Token: 0x040005B6 RID: 1462
	public float delay;

	// Token: 0x040005B7 RID: 1463
	public GameObject prefab;

	// Token: 0x040005B8 RID: 1464
	[HideInInspector]
	public GameObject instance;

	// Token: 0x040005B9 RID: 1465
	public object obj;

	// Token: 0x040005BA RID: 1466
	public DT.Def def;

	// Token: 0x040005BB RID: 1467
	public Vars vars;

	// Token: 0x040005BC RID: 1468
	public string text;

	// Token: 0x040005BD RID: 1469
	public string caption;

	// Token: 0x040005BE RID: 1470
	public Sprite icon;

	// Token: 0x040005BF RID: 1471
	public float min_width;

	// Token: 0x040005C0 RID: 1472
	public float max_width = 600f;

	// Token: 0x040005C1 RID: 1473
	public string alignment = "left";

	// Token: 0x040005C2 RID: 1474
	public int pinable = -1;

	// Token: 0x040005C3 RID: 1475
	public float pinable_delay;

	// Token: 0x040005C4 RID: 1476
	public float prefer_aligned = 0.5f;

	// Token: 0x040005C5 RID: 1477
	public float prefer_vertical = 1.25f;

	// Token: 0x040005C6 RID: 1478
	public float prefer_top = 1.1f;

	// Token: 0x040005C7 RID: 1479
	public float prefer_right = 1.05f;

	// Token: 0x040005C8 RID: 1480
	public Tooltip.Handler handler;

	// Token: 0x040005C9 RID: 1481
	private bool no_refresh;

	// Token: 0x040005CA RID: 1482
	public Tutorial.HotspotDef tutorial_hotspot_def;

	// Token: 0x040005CB RID: 1483
	[NonSerialized]
	public string path;

	// Token: 0x040005CC RID: 1484
	[NonSerialized]
	public bool is_tutorial_tooltip;

	// Token: 0x040005CD RID: 1485
	[NonSerialized]
	public Tooltip tutorial_tooltip;

	// Token: 0x040005CE RID: 1486
	[NonSerialized]
	public Tooltip tutorial_tooltip_parent;

	// Token: 0x040005D1 RID: 1489
	private static bool dont_on_enable;

	// Token: 0x02000576 RID: 1398
	public enum Event
	{
		// Token: 0x04003061 RID: 12385
		Hide,
		// Token: 0x04003062 RID: 12386
		Show,
		// Token: 0x04003063 RID: 12387
		Fill,
		// Token: 0x04003064 RID: 12388
		Place,
		// Token: 0x04003065 RID: 12389
		Update
	}

	// Token: 0x02000577 RID: 1399
	public interface IHandler
	{
		// Token: 0x060043DD RID: 17373
		bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt);
	}

	// Token: 0x02000578 RID: 1400
	// (Invoke) Token: 0x060043DF RID: 17375
	public delegate bool Handler(BaseUI ui, Tooltip tooltip, Tooltip.Event evt);
}
