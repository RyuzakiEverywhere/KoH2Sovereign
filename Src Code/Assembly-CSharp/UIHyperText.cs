using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B3 RID: 179
public class UIHyperText : MonoBehaviour, IVars
{
	// Token: 0x06000660 RID: 1632 RVA: 0x000443D0 File Offset: 0x000425D0
	public void Clear()
	{
		this.ClearRTToObj();
		if (this.rows == null)
		{
			return;
		}
		for (int i = this.rows.Count - 1; i >= 0; i--)
		{
			UIHyperText.Row row = this.rows[i];
			if (row.elements != null)
			{
				for (int j = row.elements.Count - 1; j >= 0; j--)
				{
					ObjectPool.DestroyObj(row.elements[j].rt.gameObject);
				}
			}
			Transform transform = row.rt.Find("background");
			if (transform != null)
			{
				ObjectPool.DestroyObj(transform.gameObject);
			}
			if (UIHyperText.warn_non_empty_rows && row.rt.childCount != 0)
			{
				UIHyperText.warn_non_empty_rows = false;
				string text = string.Format("HyperText row not empty: {0}", row);
				for (int k = 0; k < row.rt.childCount; k++)
				{
					Transform child = row.rt.GetChild(k);
					text += string.Format("\n{0}", child);
				}
				Debug.LogWarning(text);
				if (row.elements != null)
				{
					for (int l = 0; l < row.elements.Count; l++)
					{
						ObjectPool.DestroyObj(row.elements[l].rt.gameObject);
					}
				}
			}
			if (row.rt == UIHyperText.selected_row_rt)
			{
				UIHyperText.selected_row_rt = null;
			}
			ObjectPool.DestroyObj(row.rt.gameObject);
			row.elements = null;
		}
		this.rows = null;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0004455D File Offset: 0x0004275D
	public static RectTransform SpawnTemplate(string template_name, string obj_name, Transform parent, bool activate, params Type[] component_types)
	{
		return global::Common.SpawnTemplate<RectTransform>(template_name, obj_name, parent, activate, component_types);
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0004456A File Offset: 0x0004276A
	public static RectTransform SpawnPrefab(GameObject prefab, Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
		return ((gameObject != null) ? gameObject.transform : null) as RectTransform;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00044584 File Offset: 0x00042784
	public UIHyperText.Row NewRow(string name = null, float min_height = 0f)
	{
		Profile.BeginSection("HyperText.NewRow");
		if (string.IsNullOrEmpty(name))
		{
			name = "row" + ((this.rows == null) ? 1 : (this.rows.Count + 1));
		}
		RectTransform rectTransform = UIHyperText.SpawnTemplate("ht_row", name, base.transform, false, new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement),
			typeof(HorizontalLayoutGroup)
		});
		rectTransform.gameObject.hideFlags = HideFlags.DontSave;
		HorizontalLayoutGroup component = rectTransform.GetComponent<HorizontalLayoutGroup>();
		component.childControlWidth = true;
		component.childControlHeight = true;
		component.childForceExpandWidth = false;
		component.childForceExpandHeight = false;
		LayoutElement component2 = rectTransform.GetComponent<LayoutElement>();
		if (min_height != 0f)
		{
			component2.minHeight = min_height;
		}
		else
		{
			component2.minHeight = -1f;
		}
		UIHyperText.Row row = new UIHyperText.Row();
		row.ht = this;
		row.rt = rectTransform;
		row.lg = component;
		row.le = component2;
		if (this.rows == null)
		{
			this.rows = new List<UIHyperText.Row>();
		}
		this.rows.Add(row);
		UIHyperText.rt_to_obj.Add(rectTransform, row);
		rectTransform.gameObject.SetActive(true);
		Profile.EndSection("HyperText.NewRow");
		return row;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000446C4 File Offset: 0x000428C4
	private UIHyperText.Row EnsureLastRow()
	{
		UIHyperText.Row row = this.last_row;
		if (row == null)
		{
			row = this.NewRow(null, 0f);
		}
		return row;
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000446EC File Offset: 0x000428EC
	public UIHyperText.Element AddElement(GameObject prefab, float width = 0f, float height = 0f, bool resize = true)
	{
		if (prefab == null)
		{
			return null;
		}
		UIHyperText.Row row = this.EnsureLastRow();
		RectTransform key = UIHyperText.SpawnPrefab(prefab, row.rt);
		UIHyperText.Element element = row.AddElement(key, width, height);
		UIHyperText.rt_to_obj.Add(key, element);
		if (resize)
		{
			this.ResizeElement(element);
		}
		return element;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0004473C File Offset: 0x0004293C
	public UIHyperText.Element AddElement(string prefab_key, float width = 0f, float height = 0f, bool resize = true)
	{
		GameObject prefab = UIHyperText.GetPrefab(prefab_key, null);
		return this.AddElement(prefab, width, height, resize);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0004475C File Offset: 0x0004295C
	public UIHyperText.Element AddElement(RectTransform rt, float width = 0f, float height = 0f, bool resize = true)
	{
		if (rt == null)
		{
			return null;
		}
		UIHyperText.Row row = this.last_row;
		if (row == null)
		{
			row = this.NewRow(null, 0f);
			rt.SetParent(row.rt, false);
		}
		UIHyperText.Element element = row.AddElement(rt, width, height);
		UIHyperText.rt_to_obj.Add(rt, element);
		if (resize)
		{
			this.ResizeElement(element);
		}
		return element;
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x000447BC File Offset: 0x000429BC
	public UIHyperText.Element AddSpace(float width, float height, bool resize = true)
	{
		UIHyperText.Row row = this.EnsureLastRow();
		RectTransform rectTransform = UIHyperText.SpawnTemplate("ht_space", "space", row.rt, true, new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement)
		});
		UIHyperText.Element element = this.AddElement(rectTransform, width, height, resize);
		element.type = "space";
		return element;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0004481C File Offset: 0x00042A1C
	public UIHyperText.Element AddSeparator(Color clr, float height = 1f, RectOffset padding = null)
	{
		UIHyperText.Row row = this.NewRow("separator", 0f);
		if (padding != null)
		{
			row.lg.padding = padding;
		}
		UIHyperText.Element element = this.AddIcon(0f, 0f, null, false);
		LayoutElement le = element.le;
		element.le.preferredHeight = height;
		le.minHeight = height;
		element.le.flexibleWidth = 1f;
		element.img.sprite = null;
		element.img.color = clr;
		element.rt.name = "image";
		return element;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000448AF File Offset: 0x00042AAF
	public UIHyperText.Element AddObjectIcon(float width, float height, object obj, IVars vars = null)
	{
		return null;
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x000448B4 File Offset: 0x00042AB4
	public UIHyperText.Element AddIcon(float width, float height, Sprite icon, bool resize = true)
	{
		UIHyperText.Row row = this.EnsureLastRow();
		RectTransform rectTransform = UIHyperText.SpawnTemplate("ht_icon", "icon", row.rt, false, new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement),
			typeof(Image)
		});
		Image component = rectTransform.GetComponent<Image>();
		component.overrideSprite = icon;
		UIHyperText.Element element = this.AddElement(rectTransform, width, height, resize);
		element.type = "icon";
		element.script = component;
		rectTransform.gameObject.SetActive(true);
		return element;
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00044944 File Offset: 0x00042B44
	public UIHyperText.Element AddIcon(float width, float height, string icon_key)
	{
		string text;
		string key;
		UIHyperText.ResolveKey(icon_key, out text, out key, null);
		Sprite obj = global::Defs.GetObj<Sprite>(text, key, null);
		return this.AddIcon(width, height, obj, true);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00044970 File Offset: 0x00042B70
	public UIHyperText.Element AddText(string text_key = null, IVars vars = null, float width = 0f, float height = 0f, string prefab_key = "default_text")
	{
		UIHyperText.Element element = this.AddElement(prefab_key, width, height, true);
		if (element == null)
		{
			return null;
		}
		TextMeshProUGUI component = element.rt.GetComponent<TextMeshProUGUI>();
		if (component == null)
		{
			return null;
		}
		element.type = "text";
		element.script = component;
		if (text_key != null)
		{
			UIText.SetTextKey(component, text_key, vars, null);
		}
		return element;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0002C538 File Offset: 0x0002A738
	public bool AppendText(string text_key, IVars vars = null)
	{
		return false;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x000449C5 File Offset: 0x00042BC5
	public void Refresh()
	{
		Profile.BeginSection("HyperText.Refresh");
		this.LoadInternal(this.def, this.vars);
		Profile.EndSection("HyperText.Refresh");
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x000449F0 File Offset: 0x00042BF0
	public void Load(string def_id, IVars vars = null)
	{
		DT.Field defField = global::Defs.GetDefField(def_id, null);
		this.Load(defField, vars);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00044A0D File Offset: 0x00042C0D
	public void Load(DT.Field def, IVars vars = null)
	{
		Profile.BeginSection("HyperText.Load");
		this.LoadInternal(def, vars);
		Profile.EndSection("HyperText.Load");
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00044A2C File Offset: 0x00042C2C
	private void LoadInternal(DT.Field def, IVars vars)
	{
		bool activeSelf = base.gameObject.activeSelf;
		if (activeSelf)
		{
			base.gameObject.SetActive(false);
		}
		this.defs_version = global::Defs.Version;
		this.Clear();
		this.def = def;
		this.vars = vars;
		if (def == null)
		{
			if (activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			return;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		UIHyperText.Callback(def, "setup", this, null, null, -1, null);
		this.LoadDefaults();
		this.LoadRows();
		this.HideEmptyElements();
		Vars.PopReflectionMode(old_mode);
		if (activeSelf)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00044AC4 File Offset: 0x00042CC4
	private void LoadDefaults()
	{
		if (this.lg == null)
		{
			this.SetupLayoutGroup();
		}
		RectOffset rectOffset = UIHyperText.LoadPadding(this.def, null, "");
		if (rectOffset != null)
		{
			this.lg.padding = rectOffset;
		}
		this.lg.spacing = this.def.GetFloat("spacing.vertical", null, 0f, true, true, true, '.');
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00044B2C File Offset: 0x00042D2C
	public static UIHyperText.CallbackFunc GetCallback(string class_name, string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		Type type = null;
		string key;
		if (string.IsNullOrEmpty(class_name))
		{
			key = "UIHyperText." + name;
			type = typeof(UIHyperText);
		}
		else
		{
			key = class_name + "." + name;
		}
		UIHyperText.CallbackFunc callbackFunc;
		if (UIHyperText.callbacks.TryGetValue(key, out callbackFunc))
		{
			return callbackFunc;
		}
		if (type == null)
		{
			type = Type.GetType(class_name);
			if (type == null)
			{
				UIHyperText.callbacks.Add(key, null);
				return null;
			}
		}
		callbackFunc = (UIHyperText.CallbackFunc)Delegate.CreateDelegate(typeof(UIHyperText.CallbackFunc), type, name, false, false);
		UIHyperText.callbacks.Add(key, callbackFunc);
		return callbackFunc;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00044BD4 File Offset: 0x00042DD4
	public static Value Callback(DT.Field f, string key, UIHyperText ht = null, UIHyperText.Row row = null, UIHyperText.Element e = null, int instance_idx = -1, object instance_obj = null)
	{
		Profile.BeginSection("HyperText.Callback");
		DT.Field field = (f != null) ? f.FindChild(key, null, true, true, true, '.') : null;
		if (field == null)
		{
			Profile.EndSection("HyperText.Callback");
			return Value.Unknown;
		}
		Value value = field.Value(null, false, true);
		bool flag = value.obj_val is Expression;
		if (!value.is_string && !flag)
		{
			Profile.EndSection("HyperText.Callback");
			return Value.Unknown;
		}
		UIHyperText.CallbackParams callbackParams = new UIHyperText.CallbackParams
		{
			ht = ht,
			row = row,
			e = e,
			def = f,
			cb_field = field,
			instance_idx = instance_idx,
			instance_obj = instance_obj
		};
		if (flag)
		{
			Value result = field.Value(callbackParams, true, true);
			Profile.EndSection("HyperText.Callback");
			return result;
		}
		string text = value.String(null);
		if (string.IsNullOrEmpty(text))
		{
			Profile.EndSection("HyperText.Callback");
			return Value.Unknown;
		}
		string text2 = null;
		int num = text.IndexOf('.');
		if (num >= 0)
		{
			text2 = text.Substring(0, num);
			text = text.Substring(num + 1);
		}
		if (text2 == null)
		{
			DT.Field field2 = UIHyperText.FindField(f, "cs_class", "", null, null);
			text2 = ((field2 != null) ? field2.String(null, "") : null);
		}
		UIHyperText.CallbackFunc callback = UIHyperText.GetCallback(text2, text);
		if (callback == null)
		{
			Profile.EndSection("HyperText.Callback");
			return Value.Unknown;
		}
		Value result2 = callback(callbackParams);
		Profile.EndSection("HyperText.Callback");
		return result2;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00044D53 File Offset: 0x00042F53
	public static Value Callback(UIHyperText.Row row, string key)
	{
		return UIHyperText.Callback(row.def, key, row.ht, row, null, row.instance_idx, row.instance_obj);
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00044D75 File Offset: 0x00042F75
	public static Value Callback(UIHyperText.Element e, string key)
	{
		return UIHyperText.Callback(e.def, key, e.row.ht, e.row, e, e.instance_idx, e.instance_obj);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00044DA4 File Offset: 0x00042FA4
	private void LoadRows()
	{
		DT.Field field = this.def;
		List<DT.Field> list = (field != null) ? field.Children() : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i];
			string text = field2.Type();
			UIHyperText.Loader loader;
			if (!string.IsNullOrEmpty(text) && UIHyperText.loaders.TryGetValue(text, out loader))
			{
				if (text != "row" && text != "rows" && text != "separator")
				{
					UIHyperText.Row row = this.NewRow(field2.key, 0f);
					row.def = field2;
					RectOffset rectOffset = UIHyperText.LoadPadding(this.def, "", "row_");
					if (rectOffset != null)
					{
						row.lg.padding = rectOffset;
					}
				}
				if (!(UIHyperText.Callback(field2, "validate", this, this.last_row, null, -1, null) == false))
				{
					loader(this, field2, -1, null);
				}
			}
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00044EA4 File Offset: 0x000430A4
	private static DT.Field FindField(DT.Field f, string key, string parents_prefix = null, DT.Field stop_at_field = null, string stop_at_type = "hypertext")
	{
		if (f == null)
		{
			return null;
		}
		DT.Field field = f.FindChild(key, null, true, true, true, '.');
		if (field != null)
		{
			return field;
		}
		if (parents_prefix == null)
		{
			return null;
		}
		if (f == stop_at_field)
		{
			return null;
		}
		if (f.type == stop_at_type && stop_at_type != null)
		{
			return null;
		}
		string path = parents_prefix + key;
		for (DT.Field parent = f.parent; parent != null; parent = parent.parent)
		{
			field = parent.FindChild(path, null, true, true, true, '.');
			if (field != null)
			{
				return field;
			}
			if (parent == stop_at_field || (parent.type == stop_at_type && stop_at_type != null))
			{
				break;
			}
		}
		return null;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00044F30 File Offset: 0x00043130
	private static float FindFloat(DT.Field f, string key, string parents_prefix = null, float def_val = 0f)
	{
		DT.Field field = UIHyperText.FindField(f, key, parents_prefix, null, "hypertext");
		if (field == null)
		{
			return def_val;
		}
		return field.Float(null, def_val);
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00044F5C File Offset: 0x0004315C
	private static string FindString(DT.Field f, string key, string parents_prefix = null, string def_val = null)
	{
		DT.Field field = UIHyperText.FindField(f, key, parents_prefix, null, "hypertext");
		if (field == null)
		{
			return def_val;
		}
		return field.String(null, def_val);
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00044F88 File Offset: 0x00043188
	private static DT.Field FindTextParamField(DT.Field f, string key, DT.Field style_field, DT.Field style_def)
	{
		DT.Field field = UIHyperText.FindField(f, key, "text_", (style_field != null) ? style_field.parent : null, "hypertext");
		if (field != null)
		{
			return field;
		}
		field = ((style_def != null) ? style_def.FindChild(key, null, true, true, true, '.') : null);
		if (field != null)
		{
			return field;
		}
		return null;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00044FD4 File Offset: 0x000431D4
	private static float FindFloatTextParam(DT.Field f, string key, DT.Field style_field, DT.Field style_def, IVars vars = null, float def_val = 0f)
	{
		DT.Field field = UIHyperText.FindTextParamField(f, key, style_field, style_def);
		if (field == null)
		{
			return def_val;
		}
		return field.Float(vars, def_val);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00044FFC File Offset: 0x000431FC
	private static string FindStringTextParam(DT.Field f, string key, DT.Field style_field, DT.Field style_def, IVars vars = null, string def_val = null)
	{
		DT.Field field = UIHyperText.FindTextParamField(f, key, style_field, style_def);
		if (field == null)
		{
			return def_val;
		}
		return field.String(vars, def_val);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00045024 File Offset: 0x00043224
	private static void LoadSize(UIHyperText.Element e, DT.Field f)
	{
		string parents_prefix = null;
		if (!string.IsNullOrEmpty(e.type))
		{
			parents_prefix = e.type + "_";
		}
		DT.Field field = UIHyperText.FindField(f, "size", parents_prefix, null, "hypertext");
		if (field != null)
		{
			Value value = field.Value(e, true, true);
			object obj_val;
			if (value.is_number)
			{
				float height = value.Float(0f);
				e.width = (e.height = height);
			}
			else if ((obj_val = value.obj_val) is Point)
			{
				Point point = (Point)obj_val;
				e.width = point.x;
				e.height = point.y;
			}
		}
		DT.Field field2 = UIHyperText.FindField(f, "width", parents_prefix, null, "hypertext");
		if (field2 != null)
		{
			e.width = field2.Float(e, 0f);
		}
		DT.Field field3 = UIHyperText.FindField(f, "height", parents_prefix, null, "hypertext");
		if (field3 != null)
		{
			e.height = field3.Float(e, 0f);
		}
		DT.Field field4 = UIHyperText.FindField(f, "flexible_width", parents_prefix, null, "hypertext");
		if (field4 != null)
		{
			e.flexible_width = field4.Float(e, 0f);
			return;
		}
		if (e.width == 0f)
		{
			e.flexible_width = 100f;
		}
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00045168 File Offset: 0x00043368
	private static Value SetupElement(UIHyperText.Element e, DT.Field f, string name = null)
	{
		e.def = f;
		e.rt.name = (name ?? f.key);
		UIHyperText.LoadSize(e, f);
		e.row.ht.ResizeElement(e);
		string parents_prefix = (e.type == null) ? null : (e.type + "_");
		DT.Field field = UIHyperText.FindField(f, "optional", parents_prefix, null, "hypertext");
		if (field != null)
		{
			e.optional = field.Bool(e, false);
		}
		else
		{
			e.optional = (e.type != "icon");
		}
		UIHyperText.SetupTooltip(e.row.ht, f, e.rt, e);
		return UIHyperText.Callback(e, "setup");
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00045228 File Offset: 0x00043428
	private static void LoadTextField(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		DT.Field field = null;
		DT.Field field2;
		if (f.type == "text")
		{
			field2 = f;
		}
		else
		{
			field2 = global::Defs.FindTextField(f, "text");
			field = UIHyperText.FindField(f, "style", "text_", null, "hypertext");
		}
		DT.Field field3 = null;
		if (field != null)
		{
			string str = field.String(ht.last_row, "");
			field3 = UIHyperText.FindField(f, "text_styles." + str, "", null, null);
			if (field3 == null)
			{
				Debug.LogWarning("text style '" + str + "' not found");
			}
		}
		if (field3 == null)
		{
			field3 = UIHyperText.FindField(f, "text_styles.default", "", null, null);
			if (field3 == null)
			{
				Debug.LogError(f.Path(false, false, '.') + ".text_styles.default not found");
				return;
			}
		}
		GameObject obj = global::Defs.GetObj<GameObject>(field3, null);
		if (obj == null)
		{
			Debug.LogError(field3.Path(true, false, '.') + ": Invalid prefab");
			return;
		}
		UIHyperText.Element element = ht.AddElement(obj, 0f, 0f, false);
		if (element == null)
		{
			return;
		}
		element.instance_idx = grp_idx;
		element.instance_obj = grp_obj;
		TextMeshProUGUI component = element.rt.GetComponent<TextMeshProUGUI>();
		if (component == null)
		{
			return;
		}
		element.type = "text";
		element.script = component;
		TextMeshProUGUI component2 = obj.GetComponent<TextMeshProUGUI>();
		string name = null;
		if (element.row.def.type != "row" && element.row.def.type != "rows")
		{
			name = "text";
		}
		if (UIHyperText.SetupElement(element, f, name) == false)
		{
			return;
		}
		RectOffset rectOffset = UIHyperText.LoadPadding(f, "text_", "");
		if (rectOffset != null)
		{
			component.margin = new Vector4((float)rectOffset.left, (float)rectOffset.top, (float)rectOffset.right, (float)rectOffset.bottom);
		}
		else
		{
			component.margin = component2.margin;
		}
		DT.Field field4 = UIHyperText.FindTextParamField(f, "color", field, field3);
		if (field4 != null)
		{
			component.color = global::Defs.GetColor(field4, Color.clear, element);
		}
		else
		{
			component.color = component2.color;
		}
		float num = UIHyperText.FindFloatTextParam(f, "font_size", field, field3, element, 0f);
		if (num > 0f)
		{
			component.fontSize = num;
		}
		else
		{
			component.fontSize = component2.fontSize;
		}
		string a = UIHyperText.FindStringTextParam(f, "alignment", field, field3, element, null);
		if (!(a == "left"))
		{
			if (!(a == "center"))
			{
				if (!(a == "right"))
				{
					component.alignment = component2.alignment;
				}
				else
				{
					component.alignment = TextAlignmentOptions.Right;
				}
			}
			else
			{
				component.alignment = TextAlignmentOptions.Center;
			}
		}
		else
		{
			component.alignment = TextAlignmentOptions.Left;
		}
		if (field2 != null)
		{
			string text = global::Defs.Localize(field2, element, null, true, true);
			text = text.Trim();
			UIText.SetText(component, text);
			return;
		}
		component.text = "";
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0004553C File Offset: 0x0004373C
	private static GameObject CreateIconWithFrameTemplate()
	{
		GameObject gameObject = new GameObject("ht_icon_frame", new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement),
			typeof(Image)
		});
		gameObject.SetActive(false);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		UICommon.FillParent(component);
		RectTransform component2 = new GameObject("frame", new Type[]
		{
			typeof(RectTransform),
			typeof(Image)
		}).GetComponent<RectTransform>();
		component2.SetParent(component, false);
		UICommon.FillParent(component2);
		return gameObject;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000455D4 File Offset: 0x000437D4
	private static GameObject CreateIconWithMaskTemplate()
	{
		GameObject gameObject = new GameObject("ht_icon_mask", new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement),
			typeof(Image),
			typeof(Mask)
		});
		gameObject.SetActive(false);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		UICommon.FillParent(component);
		gameObject.GetComponent<Mask>().showMaskGraphic = false;
		RectTransform component2 = new GameObject("image", new Type[]
		{
			typeof(RectTransform),
			typeof(Image)
		}).GetComponent<RectTransform>();
		component2.SetParent(component, false);
		UICommon.FillParent(component2);
		return gameObject;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00045684 File Offset: 0x00043884
	private static GameObject CreateIconWithFrameAndMaskTemplate()
	{
		GameObject gameObject = new GameObject("ht_icon_frame_mask", new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement)
		});
		gameObject.SetActive(false);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		UICommon.FillParent(component);
		GameObject gameObject2 = new GameObject("mask", new Type[]
		{
			typeof(RectTransform),
			typeof(Image),
			typeof(Mask)
		});
		RectTransform component2 = gameObject2.GetComponent<RectTransform>();
		component2.SetParent(component, false);
		UICommon.FillParent(component2);
		gameObject2.GetComponent<Mask>().showMaskGraphic = false;
		RectTransform component3 = new GameObject("image", new Type[]
		{
			typeof(RectTransform),
			typeof(Image)
		}).GetComponent<RectTransform>();
		component3.SetParent(component2, false);
		UICommon.FillParent(component3);
		RectTransform component4 = new GameObject("frame", new Type[]
		{
			typeof(RectTransform),
			typeof(Image)
		}).GetComponent<RectTransform>();
		component4.SetParent(component, false);
		UICommon.FillParent(component4);
		return gameObject;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x000457A0 File Offset: 0x000439A0
	private static void LoadIcon(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		DT.Field field;
		if (f.Type() == "sprite")
		{
			field = f;
		}
		else
		{
			field = f.FindChild("image", null, true, true, true, '.');
		}
		DT.Field field2 = UIHyperText.FindField(f, "frame", "icon_", null, "hypertext");
		if (field2 != null && field2.value.is_null)
		{
			field2 = null;
		}
		DT.Field field3 = UIHyperText.FindField(f, "mask", "icon_", null, "hypertext");
		if (field3 != null && field3.value.is_null)
		{
			field3 = null;
		}
		UIHyperText.Row row = ht.EnsureLastRow();
		Image image = null;
		Image image2 = null;
		RectTransform rectTransform;
		Image image3;
		if (field2 == null && field3 == null)
		{
			rectTransform = UIHyperText.SpawnTemplate("ht_icon", null, row.rt, false, new Type[]
			{
				typeof(RectTransform),
				typeof(LayoutElement),
				typeof(Image)
			});
			image3 = rectTransform.GetComponent<Image>();
		}
		else if (field3 == null)
		{
			GameObject gameObject = global::Common.FindTemplate("ht_icon_frame");
			if (gameObject == null)
			{
				gameObject = UIHyperText.CreateIconWithFrameTemplate();
				global::Common.AddTemplate("ht_icon_frame", gameObject);
			}
			rectTransform = UIHyperText.SpawnPrefab(gameObject, row.rt);
			image3 = rectTransform.GetComponent<Image>();
			image = global::Common.FindChildComponent<Image>(rectTransform.gameObject, "frame");
		}
		else if (field2 == null)
		{
			GameObject gameObject2 = global::Common.FindTemplate("ht_icon_mask");
			if (gameObject2 == null)
			{
				gameObject2 = UIHyperText.CreateIconWithMaskTemplate();
				global::Common.AddTemplate("ht_icon_mask", gameObject2);
			}
			rectTransform = UIHyperText.SpawnPrefab(gameObject2, row.rt);
			image3 = global::Common.FindChildComponent<Image>(rectTransform.gameObject, "image");
			image2 = rectTransform.GetComponent<Image>();
		}
		else
		{
			GameObject gameObject3 = global::Common.FindTemplate("ht_icon_frame_mask");
			if (gameObject3 == null)
			{
				gameObject3 = UIHyperText.CreateIconWithFrameAndMaskTemplate();
				global::Common.AddTemplate("ht_icon_frame_mask", gameObject3);
			}
			rectTransform = UIHyperText.SpawnPrefab(gameObject3, row.rt);
			image2 = global::Common.FindChildComponent<Image>(rectTransform.gameObject, "mask");
			image3 = global::Common.FindChildComponent<Image>(image2.gameObject, "image");
			image = global::Common.FindChildComponent<Image>(rectTransform.gameObject, "frame");
		}
		UIHyperText.Element element = ht.AddElement(rectTransform, 0f, 0f, true);
		element.type = "icon";
		element.instance_idx = grp_idx;
		element.instance_obj = grp_obj;
		element.script = image3;
		element.mask = image2;
		element.frame = image;
		if (UIHyperText.SetupElement(element, f, null) == false)
		{
			return;
		}
		if (image2 != null)
		{
			image2.sprite = global::Defs.GetObj<Sprite>(field3, element);
		}
		if (image3 != null)
		{
			image3.sprite = global::Defs.GetObj<Sprite>(field, element);
			DT.Field field4 = UIHyperText.FindField(f, "color", "icon_", null, "hypertext");
			if (field4 != null)
			{
				image3.color = global::Defs.GetColor(field4, Color.clear, element);
			}
			else
			{
				image3.color = Color.white;
			}
		}
		if (image != null)
		{
			image.sprite = global::Defs.GetObj<Sprite>(field2, element);
			DT.Field field5 = UIHyperText.FindField(field2, "color", "frame_", null, "hypertext");
			if (field5 != null)
			{
				image.color = global::Defs.GetColor(field5, Color.clear, element);
			}
			else
			{
				image.color = Color.white;
			}
			image.gameObject.SetActive(image.sprite != null);
		}
		rectTransform.gameObject.SetActive(true);
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00045B04 File Offset: 0x00043D04
	private static void LoadObjectIcon(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		UIHyperText.Row row = ht.EnsureLastRow();
		RectTransform rectTransform = UIHyperText.SpawnTemplate("ht_object_icon", "object_icon", row.rt, false, new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement)
		});
		UIHyperText.Element element = ht.AddElement(rectTransform, 0f, 0f, true);
		element.def = f;
		element.type = "object_icon";
		element.instance_idx = grp_idx;
		element.instance_obj = grp_obj;
		GameObject icon = ObjectIcon.GetIcon(f.Value(element, true, true), new Vars(element), element.rt);
		UICommon.FillParent(((icon != null) ? icon.transform : null) as RectTransform);
		UIHyperText.SetupElement(element, f, null);
		rectTransform.gameObject.SetActive(true);
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00045BC8 File Offset: 0x00043DC8
	private static void LoadPrefab(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		GameObject obj = global::Defs.GetObj<GameObject>(f, ht);
		if (obj == null)
		{
			return;
		}
		UIHyperText.Row row = ht.EnsureLastRow();
		RectTransform rectTransform = UIHyperText.SpawnPrefab(obj, row.rt);
		UIHyperText.Element element = ht.AddElement(rectTransform, 0f, 0f, true);
		element.def = f;
		element.type = "prefab";
		element.instance_idx = grp_idx;
		element.instance_obj = grp_obj;
		UIHyperText.SetupElement(element, f, null);
		rectTransform.gameObject.SetActive(true);
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00045C41 File Offset: 0x00043E41
	private static void LoadSpace(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		UIHyperText.SetupElement(ht.AddSpace(0f, 0f, false), f, null);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00045C5C File Offset: 0x00043E5C
	private static void LoadHyperText(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		GameObject gameObject = UIHyperText.GetPrefab(f.String(null, ""), null);
		UIHyperText.Row row = ht.EnsureLastRow();
		if (gameObject == null)
		{
			gameObject = global::Common.FindTemplate("ht_hypertext");
			if (gameObject == null)
			{
				gameObject = global::Common.CreateTemplate("ht_hypertext", new Type[]
				{
					typeof(RectTransform),
					typeof(LayoutElement),
					typeof(UIHyperText),
					typeof(VerticalLayoutGroup)
				});
				VerticalLayoutGroup component = gameObject.GetComponent<VerticalLayoutGroup>();
				component.childControlWidth = true;
				component.childControlHeight = true;
				component.childForceExpandWidth = true;
				component.childForceExpandHeight = false;
				global::Common.AddTemplate("ht_hypertext", gameObject);
			}
		}
		RectTransform rectTransform = UIHyperText.SpawnPrefab(gameObject, row.rt);
		rectTransform.name = f.key;
		UIHyperText orAddComponent = rectTransform.GetOrAddComponent<UIHyperText>();
		orAddComponent.def_id = null;
		UIHyperText.Element element = ht.AddElement(rectTransform, 0f, 0f, true);
		element.instance_idx = grp_idx;
		element.instance_obj = grp_obj;
		element.script = orAddComponent;
		orAddComponent.parent_element = element;
		UIHyperText.SetupElement(element, f, null);
		orAddComponent.LoadInternal(f, null);
		rectTransform.gameObject.SetActive(true);
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00045D90 File Offset: 0x00043F90
	private static void LoadElements(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		Value value = UIHyperText.Callback(f, "populate", ht, ht.last_row, null, -1, null);
		if (value.is_unknown)
		{
			value = f.Value(ht.last_row, true, false);
		}
		if (value.type == Value.Type.Int)
		{
			int num = value.Int(0);
			for (int i = 0; i < num; i++)
			{
				UIHyperText.LoadChildElements(ht, f, i, null);
			}
			return;
		}
		if (!value.is_object)
		{
			return;
		}
		DT.Field field;
		if ((field = (value.obj_val as DT.Field)) != null)
		{
			if (field.children == null)
			{
				return;
			}
			int num2 = 0;
			for (int j = 0; j < field.children.Count; j++)
			{
				DT.Field field2 = field.children[j];
				if (!string.IsNullOrEmpty(field2.key))
				{
					UIHyperText.LoadChildElements(ht, f, num2, field2);
					num2++;
				}
			}
			return;
		}
		else
		{
			IList list;
			if ((list = (value.obj_val as IList)) != null)
			{
				int count = list.Count;
				for (int k = 0; k < count; k++)
				{
					object obj = list[k];
					DT.SubValue subValue;
					if ((subValue = (obj as DT.SubValue)) != null)
					{
						obj = subValue.value.Object(true);
					}
					UIHyperText.LoadChildElements(ht, f, k, obj);
				}
				return;
			}
			return;
		}
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00045EC0 File Offset: 0x000440C0
	private static void LoadRow(UIHyperText ht, DT.Field f, int instance_idx, object instance_obj = null)
	{
		if (UIHyperText.Callback(f, "validate", ht, null, null, instance_idx, instance_obj) == false)
		{
			return;
		}
		if (UIHyperText.Callback(f, "row_validate", ht, null, null, instance_idx, instance_obj) == false)
		{
			return;
		}
		UIHyperText.Row row = ht.NewRow((f != null) ? f.key : null, 0f);
		row.def = f;
		row.instance_idx = instance_idx;
		row.instance_obj = instance_obj;
		RectOffset rectOffset = UIHyperText.LoadPadding(f, "row_", "");
		if (rectOffset != null)
		{
			row.lg.padding = rectOffset;
		}
		else
		{
			row.lg.padding = null;
		}
		DT.Field field = UIHyperText.FindField(f, "spacing", null, null, "hypertext");
		if (field != null)
		{
			row.lg.spacing = field.Float(null, 0f);
		}
		else
		{
			row.lg.spacing = ht.def.GetFloat("spacing.horizontal", null, 0f, true, true, true, '.');
		}
		string a = UIHyperText.FindString(f, "alignment", "row_", "top");
		if (!(a == "top"))
		{
			if (a == "center")
			{
				row.lg.childAlignment = TextAnchor.MiddleLeft;
				goto IL_151;
			}
			if (a == "bottom")
			{
				row.lg.childAlignment = TextAnchor.LowerLeft;
				goto IL_151;
			}
		}
		row.lg.childAlignment = TextAnchor.UpperLeft;
		IL_151:
		UIHyperText.LoadBackground(ht, f, row.rt, row);
		UIHyperText.SetupTooltip(ht, f, row.rt, row);
		if (UIHyperText.Callback(row, "setup") == false)
		{
			return;
		}
		UIHyperText.Row row2 = UIHyperText.posses_first_link_row;
		if (row.def.GetBool("possess_first_link", null, false, true, true, true, '.'))
		{
			UIHyperText.posses_first_link_row = row;
		}
		UIHyperText.LoadChildElements(ht, f, -1, null);
		UIHyperText.posses_first_link_row = row2;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00046088 File Offset: 0x00044288
	private static void SetupTooltip(UIHyperText ht, DT.Field f, RectTransform rt, IVars vars)
	{
		DT.Field field = (f != null) ? f.FindChild("tooltip", vars, true, true, true, '.') : null;
		if (field == null)
		{
			return;
		}
		Tooltip tooltip = Tooltip.Get(rt.gameObject, true);
		Vars vars2 = vars as Vars;
		if (vars2 == null && vars != null)
		{
			vars2 = new Vars(vars);
		}
		if (field.Type() == "text")
		{
			tooltip.SetText(field.Path(false, false, '.'), null, new Vars(vars));
			return;
		}
		DT.Field field2 = field.Ref(vars, true, true);
		if (field2 != null)
		{
			field = field2;
		}
		tooltip.SetDef(field.def, vars2, null);
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0004611C File Offset: 0x0004431C
	private static void LoadBackground(UIHyperText ht, DT.Field f, RectTransform rtParent, IVars vars)
	{
		DT.Field field = f.FindChild("background", vars, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		Sprite obj = global::Defs.GetObj<Sprite>(field, "image", vars);
		Color color = global::Defs.GetColor(field, "color", Color.clear, vars);
		if (color == Color.clear)
		{
			color = global::Defs.GetColor(field, Color.clear, vars);
		}
		if (color == Color.clear)
		{
			if (obj == null)
			{
				return;
			}
			color = Color.white;
		}
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		RectOffset rectOffset = UIHyperText.LoadPadding(field, null, "");
		if (rectOffset != null)
		{
			zero = new Vector2((float)rectOffset.left, (float)rectOffset.bottom);
			zero2 = new Vector2((float)(-(float)rectOffset.right), (float)(-(float)rectOffset.top));
		}
		UIHyperText.CreateBackground(rtParent, color, obj, zero, zero2);
		if (UIHyperText.selected_row_rt == null)
		{
			UIHyperText.selected_row_rt = rtParent;
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00046204 File Offset: 0x00044404
	private static void LoadChildElements(UIHyperText ht, DT.Field f, int grp_idx = -1, object instance_obj = null)
	{
		List<DT.Field> list = (f != null) ? f.Children() : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field = list[i];
			string text = field.Type();
			UIHyperText.Loader loader;
			if (!string.IsNullOrEmpty(text) && UIHyperText.loaders.TryGetValue(text, out loader))
			{
				if (text == "row" || text == "rows" || text == "separator")
				{
					Debug.LogError(f.Path(true, false, '.') + ": nested row");
				}
				else if (!(UIHyperText.Callback(field, "validate", ht, ht.last_row, null, grp_idx, instance_obj) == false))
				{
					loader(ht, field, grp_idx, instance_obj);
				}
			}
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000462D0 File Offset: 0x000444D0
	private static RectOffset LoadPadding(DT.Field f, string default_prefix = null, string prefix = "")
	{
		return new RectOffset
		{
			left = (int)UIHyperText.FindFloat(f, prefix + "padding.left", default_prefix, 0f),
			right = (int)UIHyperText.FindFloat(f, prefix + "padding.right", default_prefix, 0f),
			top = (int)UIHyperText.FindFloat(f, prefix + "padding.top", default_prefix, 0f),
			bottom = (int)UIHyperText.FindFloat(f, prefix + "padding.bottom", default_prefix, 0f)
		};
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0004635C File Offset: 0x0004455C
	private static void LoadRows(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		Value value = UIHyperText.Callback(f, "populate", ht, null, null, -1, null);
		if (value.is_unknown)
		{
			value = f.Value(ht, true, false);
		}
		if (value.type == Value.Type.Int)
		{
			int num = value.Int(0);
			for (int i = 0; i < num; i++)
			{
				UIHyperText.LoadRow(ht, f, i, null);
			}
			return;
		}
		if (!value.is_object)
		{
			return;
		}
		DT.Field field;
		if ((field = (value.obj_val as DT.Field)) != null)
		{
			List<DT.Field> list = field.Children();
			if (list == null)
			{
				return;
			}
			int num2 = 0;
			for (int j = 0; j < list.Count; j++)
			{
				DT.Field field2 = list[j];
				if (!string.IsNullOrEmpty(field2.key))
				{
					UIHyperText.LoadRow(ht, f, num2, field2);
					num2++;
				}
			}
			return;
		}
		else
		{
			IList list2;
			if ((list2 = (value.obj_val as IList)) != null)
			{
				int count = list2.Count;
				for (int k = 0; k < count; k++)
				{
					object obj = list2[k];
					DT.SubValue subValue;
					if ((subValue = (obj as DT.SubValue)) != null)
					{
						obj = subValue.value.Object(true);
					}
					UIHyperText.LoadRow(ht, f, k, obj);
				}
				return;
			}
			return;
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00046480 File Offset: 0x00044680
	private static void LoadSeparator(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null)
	{
		Color color = global::Defs.GetColor(UIHyperText.FindField(f, "color", "separator_", null, "hypertext"), Color.gray, null);
		float height = UIHyperText.FindFloat(f, "height", "separator_", 1f);
		RectOffset padding = UIHyperText.LoadPadding(f, "separator_", "");
		UIHyperText.Element element = ht.AddSeparator(color, height, padding);
		element.row.rt.name = f.key;
		element.row.def = f;
		element.def = f;
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000692 RID: 1682 RVA: 0x00046507 File Offset: 0x00044707
	public UIHyperText.Row last_row
	{
		get
		{
			if (this.rows != null && this.rows.Count != 0)
			{
				return this.rows[this.rows.Count - 1];
			}
			return null;
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00046538 File Offset: 0x00044738
	public UIHyperText ParentHT()
	{
		if (this.parent_element == null)
		{
			return null;
		}
		UIHyperText.Row row = this.parent_element.row;
		if (row == null)
		{
			return null;
		}
		return row.ht;
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0004655C File Offset: 0x0004475C
	public UIHyperText RootHT()
	{
		UIHyperText uihyperText = this;
		for (;;)
		{
			UIHyperText uihyperText2 = uihyperText.ParentHT();
			if (uihyperText2 == null)
			{
				break;
			}
			uihyperText = uihyperText2;
		}
		return uihyperText;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00046580 File Offset: 0x00044780
	public IVars RootVars()
	{
		return this.RootHT().vars;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00046590 File Offset: 0x00044790
	public UIHyperText.Row FindRow(string name)
	{
		if (this.rows == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		for (int i = 0; i < this.rows.Count; i++)
		{
			UIHyperText.Row row = this.rows[i];
			string a;
			if (row == null)
			{
				a = null;
			}
			else
			{
				DT.Field field = row.def;
				a = ((field != null) ? field.key : null);
			}
			if (!(a != name))
			{
				return row;
			}
		}
		return null;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000465F8 File Offset: 0x000447F8
	public Value GetVar(string key, IVars _vars = null, bool as_value = true)
	{
		if (this.vars != null)
		{
			Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
			Value var = this.vars.GetVar(key, _vars, as_value);
			Vars.PopReflectionMode(old_mode);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		if (key == "def")
		{
			return new Value(this.def);
		}
		if (key == "element")
		{
			return new Value(this.parent_element);
		}
		if (key == "parent" || key == "row")
		{
			UIHyperText.Element element = this.parent_element;
			return new Value((element != null) ? element.row : null);
		}
		if (key == "ht")
		{
			UIHyperText.Element element2 = this.parent_element;
			object val;
			if (element2 == null)
			{
				val = null;
			}
			else
			{
				UIHyperText.Row row = element2.row;
				val = ((row != null) ? row.ht : null);
			}
			return new Value(val);
		}
		if (this.parent_element != null)
		{
			Value var = this.parent_element.GetVar(key, _vars ?? this, as_value);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		if (key == "vars" && this.vars != null)
		{
			return new Value(this.vars);
		}
		UIHyperText.Row row2 = this.FindRow(key);
		if (row2 != null)
		{
			return new Value(row2);
		}
		if (this.def != null)
		{
			Value var = this.def.GetVar(key, _vars ?? this, as_value);
			if (!var.is_unknown)
			{
				return var;
			}
		}
		if (key == "vars" || key == "obj")
		{
			return Value.Null;
		}
		if (this.vars != null)
		{
			IVars vars = this.vars.GetVar("link_vars", null, true).Get<IVars>();
			if (vars != null)
			{
				Value var = vars.GetVar(key, _vars ?? this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
		}
		return Value.Unknown;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000467AF File Offset: 0x000449AF
	public override string ToString()
	{
		string str = "HyperText: ";
		DT.Field field = this.def;
		return str + ((field != null) ? field.Path(false, false, '.') : null);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000467D4 File Offset: 0x000449D4
	public static void ResolveKey(string key, out string def_id, out string path, string default_prefix = null)
	{
		int num = key.IndexOf('.');
		if (num >= 0)
		{
			def_id = key.Substring(0, num);
			path = key.Substring(num + 1);
			return;
		}
		def_id = "HyperText";
		if (default_prefix != null)
		{
			path = default_prefix + key;
			return;
		}
		path = key;
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0004681C File Offset: 0x00044A1C
	public static GameObject GetPrefab(string prefab_key, string default_prefix = null)
	{
		if (string.IsNullOrEmpty(prefab_key))
		{
			return null;
		}
		string text;
		string key;
		UIHyperText.ResolveKey(prefab_key, out text, out key, default_prefix);
		return global::Defs.GetObj<GameObject>(text, key, null);
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00046848 File Offset: 0x00044A48
	public static RectTransform CreateBackground(RectTransform rtParent, Color clr, Sprite sprite = null, Vector2 pad_min = default(Vector2), Vector2 pad_max = default(Vector2))
	{
		RectTransform rectTransform = UIHyperText.SpawnTemplate("ht_background", "background", rtParent, false, new Type[]
		{
			typeof(RectTransform),
			typeof(LayoutElement),
			typeof(Image)
		});
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = pad_min;
		rectTransform.offsetMax = pad_max;
		Image component = rectTransform.GetComponent<Image>();
		component.color = clr;
		component.overrideSprite = sprite;
		rectTransform.GetComponent<LayoutElement>().ignoreLayout = true;
		rectTransform.gameObject.SetActive(true);
		return rectTransform;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x000468F8 File Offset: 0x00044AF8
	public float CalcWidth(UIHyperText.Element e)
	{
		return e.width;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00046900 File Offset: 0x00044B00
	public float CalcHeight(UIHyperText.Element e)
	{
		return e.height;
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00046908 File Offset: 0x00044B08
	public void ResizeElement(UIHyperText.Element e)
	{
		if (e == null)
		{
			return;
		}
		if (e.width != 0f)
		{
			e.le.minWidth = (e.le.preferredWidth = this.CalcWidth(e));
		}
		else
		{
			e.le.minWidth = (e.le.preferredWidth = -1f);
		}
		if (e.height != 0f)
		{
			e.le.minHeight = (e.le.preferredHeight = this.CalcHeight(e));
		}
		else
		{
			e.le.minHeight = (e.le.preferredHeight = -1f);
		}
		if (e.flexible_width != 0f)
		{
			e.le.flexibleWidth = e.flexible_width;
			return;
		}
		e.le.flexibleWidth = -1f;
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000469E2 File Offset: 0x00044BE2
	private static bool IsEmpty(Image img)
	{
		return img == null || (!(img.sprite != null) && !(img.overrideSprite != null));
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00046A10 File Offset: 0x00044C10
	private static bool CheckEmpty(UIHyperText.Element e)
	{
		string type = e.type;
		if (!(type == "icon"))
		{
			if (type == "text")
			{
				TextMeshProUGUI txt = e.txt;
				e.empty = string.IsNullOrEmpty((txt != null) ? txt.text : null);
			}
		}
		else
		{
			e.empty = UIHyperText.IsEmpty(e.img);
		}
		return e.empty;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00046A76 File Offset: 0x00044C76
	private static bool HideIfEmpty(UIHyperText.Element e)
	{
		if (!e.optional)
		{
			return false;
		}
		if (!UIHyperText.CheckEmpty(e))
		{
			return false;
		}
		e.rt.gameObject.SetActive(false);
		return true;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00046AA0 File Offset: 0x00044CA0
	private static bool HideIfEmpty(UIHyperText.Row row)
	{
		if (row.def == null)
		{
			return false;
		}
		DT.Field field = UIHyperText.FindField(row.def, "optional", "rows_", null, "hypertext");
		if (field != null)
		{
			row.optional = field.Bool(row, false);
		}
		else
		{
			row.optional = true;
		}
		if (!row.optional)
		{
			return false;
		}
		row.empty = true;
		if (row.elements != null)
		{
			for (int i = 0; i < row.elements.Count; i++)
			{
				UIHyperText.Element element = row.elements[i];
				if (!UIHyperText.HideIfEmpty(element) && (!(element.type == "space") || !element.optional))
				{
					row.empty = false;
				}
			}
		}
		if (!row.empty)
		{
			return false;
		}
		row.rt.gameObject.SetActive(false);
		return true;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00046B70 File Offset: 0x00044D70
	private void HideEmptyElements()
	{
		bool empty = true;
		if (this.rows != null)
		{
			for (int i = 0; i < this.rows.Count; i++)
			{
				if (!UIHyperText.HideIfEmpty(this.rows[i]))
				{
					empty = false;
				}
			}
		}
		if (this.parent_element != null)
		{
			this.parent_element.empty = empty;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00046BC8 File Offset: 0x00044DC8
	private void ClearRTToObj()
	{
		if (this.rows == null)
		{
			return;
		}
		for (int i = 0; i < this.rows.Count; i++)
		{
			UIHyperText.Row row = this.rows[i];
			UIHyperText.rt_to_obj.Remove(row.rt);
			if (row.elements != null)
			{
				for (int j = 0; j < row.elements.Count; j++)
				{
					UIHyperText.Element element = row.elements[j];
					UIHyperText.rt_to_obj.Remove(element.rt);
				}
			}
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x00046C50 File Offset: 0x00044E50
	public bool FindElement(RectTransform rt, out UIHyperText.Row row, out UIHyperText.Element e)
	{
		object obj = null;
		while (rt != null && !UIHyperText.rt_to_obj.TryGetValue(rt, out obj))
		{
			rt = (rt.parent as RectTransform);
		}
		UIHyperText.Element element;
		if ((element = (obj as UIHyperText.Element)) != null)
		{
			e = element;
			row = e.row;
			return true;
		}
		UIHyperText.Row row2;
		if ((row2 = (obj as UIHyperText.Row)) != null)
		{
			row = row2;
			e = null;
			return true;
		}
		row = null;
		e = null;
		return false;
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x00046CB8 File Offset: 0x00044EB8
	public DT.Field FindDefField(RectTransform rt)
	{
		object obj = null;
		while (rt != null && !UIHyperText.rt_to_obj.TryGetValue(rt, out obj))
		{
			if (rt == this.rt)
			{
				return this.def;
			}
			rt = (rt.parent as RectTransform);
		}
		UIHyperText.Element element;
		if ((element = (obj as UIHyperText.Element)) != null)
		{
			return element.def;
		}
		UIHyperText.Row row;
		if ((row = (obj as UIHyperText.Row)) != null)
		{
			return row.def;
		}
		return null;
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x00046D28 File Offset: 0x00044F28
	private void SetupLayoutGroup()
	{
		this.lg = base.GetComponent<VerticalLayoutGroup>();
		if (this.lg != null)
		{
			return;
		}
		this.lg = base.gameObject.AddComponent<VerticalLayoutGroup>();
		this.lg.childControlWidth = true;
		this.lg.childControlHeight = true;
		this.lg.childForceExpandWidth = true;
		this.lg.childForceExpandHeight = false;
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00046D94 File Offset: 0x00044F94
	private void SetupContentSizeFitter()
	{
		this.csf = base.GetComponent<ContentSizeFitter>();
		if (this.csf != null)
		{
			return;
		}
		this.csf = base.gameObject.AddComponent<ContentSizeFitter>();
		this.csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		this.csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00046DE5 File Offset: 0x00044FE5
	private void OnEnable()
	{
		this.rt = base.gameObject.GetOrAddComponent<RectTransform>();
		if (this.lg == null)
		{
			this.SetupLayoutGroup();
		}
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00046E0C File Offset: 0x0004500C
	private void OnDestroy()
	{
		this.ClearRTToObj();
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00046E14 File Offset: 0x00045014
	private void Update()
	{
		if (this.defs_version != global::Defs.Version && !string.IsNullOrEmpty(this.def_id) && this.parent_element == null)
		{
			this.Load(this.def_id, this.vars);
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00046E4A File Offset: 0x0004504A
	public void OnPoolDeactivated()
	{
		base.gameObject.SetActive(false);
		this.vars = null;
		this.Clear();
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00046E68 File Offset: 0x00045068
	private void Test()
	{
		this.Clear();
		HorizontalLayoutGroup horizontalLayoutGroup = this.NewRow(null, 0f).lg;
		horizontalLayoutGroup.spacing = 8f;
		horizontalLayoutGroup.padding.left = 16;
		this.AddIcon(32f, 32f, "TestHyperText.icon").img.color = Color.green;
		this.AddText("TestHyperText.text1", null, 0f, 0f, "default_text");
		HorizontalLayoutGroup horizontalLayoutGroup2 = this.NewRow(null, 0f).lg;
		horizontalLayoutGroup2.spacing = 8f;
		horizontalLayoutGroup2.padding.left = 16;
		this.AddIcon(32f, 32f, "TestHyperText.icon").img.color = Color.red;
		this.AddText("TestHyperText.text2", null, 0f, 0f, "default_text");
	}

	// Token: 0x040005DB RID: 1499
	public DT.Field def;

	// Token: 0x040005DC RID: 1500
	public IVars vars;

	// Token: 0x040005DD RID: 1501
	public UIHyperText.Element parent_element;

	// Token: 0x040005DE RID: 1502
	private static bool warn_non_empty_rows = true;

	// Token: 0x040005DF RID: 1503
	private int defs_version;

	// Token: 0x040005E0 RID: 1504
	private static Dictionary<string, UIHyperText.CallbackFunc> callbacks = new Dictionary<string, UIHyperText.CallbackFunc>();

	// Token: 0x040005E1 RID: 1505
	private static Dictionary<string, UIHyperText.Loader> loaders = new Dictionary<string, UIHyperText.Loader>
	{
		{
			"text_field",
			new UIHyperText.Loader(UIHyperText.LoadTextField)
		},
		{
			"text",
			new UIHyperText.Loader(UIHyperText.LoadTextField)
		},
		{
			"icon",
			new UIHyperText.Loader(UIHyperText.LoadIcon)
		},
		{
			"sprite",
			new UIHyperText.Loader(UIHyperText.LoadIcon)
		},
		{
			"object_icon",
			new UIHyperText.Loader(UIHyperText.LoadObjectIcon)
		},
		{
			"prefab",
			new UIHyperText.Loader(UIHyperText.LoadPrefab)
		},
		{
			"space",
			new UIHyperText.Loader(UIHyperText.LoadSpace)
		},
		{
			"hypertext",
			new UIHyperText.Loader(UIHyperText.LoadHyperText)
		},
		{
			"elements",
			new UIHyperText.Loader(UIHyperText.LoadElements)
		},
		{
			"row",
			new UIHyperText.Loader(UIHyperText.LoadRow)
		},
		{
			"rows",
			new UIHyperText.Loader(UIHyperText.LoadRows)
		},
		{
			"separator",
			new UIHyperText.Loader(UIHyperText.LoadSeparator)
		}
	};

	// Token: 0x040005E2 RID: 1506
	public static UIHyperText.Row posses_first_link_row = null;

	// Token: 0x040005E3 RID: 1507
	public static RectTransform selected_row_rt = null;

	// Token: 0x040005E4 RID: 1508
	private List<UIHyperText.Row> rows;

	// Token: 0x040005E5 RID: 1509
	private RectTransform rt;

	// Token: 0x040005E6 RID: 1510
	private VerticalLayoutGroup lg;

	// Token: 0x040005E7 RID: 1511
	private ContentSizeFitter csf;

	// Token: 0x040005E8 RID: 1512
	private static Dictionary<RectTransform, object> rt_to_obj = new Dictionary<RectTransform, object>();

	// Token: 0x040005E9 RID: 1513
	public string def_id;

	// Token: 0x0200057B RID: 1403
	public class Row : IVars
	{
		// Token: 0x060043E2 RID: 17378 RVA: 0x001FF2B0 File Offset: 0x001FD4B0
		public UIHyperText.Element AddElement(RectTransform rt, float width, float height)
		{
			if (rt == null)
			{
				return null;
			}
			UIHyperText.Element element = new UIHyperText.Element
			{
				row = this,
				rt = rt,
				le = rt.GetOrAddComponent<LayoutElement>(),
				width = width,
				height = height
			};
			if (element.le != null)
			{
				element.le.minWidth = (element.le.preferredWidth = -1f);
				element.le.minHeight = (element.le.preferredHeight = -1f);
				element.le.flexibleWidth = -1f;
			}
			if (this.elements == null)
			{
				this.elements = new List<UIHyperText.Element>();
			}
			this.elements.Add(element);
			return element;
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x001FF370 File Offset: 0x001FD570
		public UIHyperText.Element FindElement(string name)
		{
			if (this.elements == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			for (int i = 0; i < this.elements.Count; i++)
			{
				UIHyperText.Element element = this.elements[i];
				DT.Field field = element.def;
				if (((field != null) ? field.key : null) == name)
				{
					return element;
				}
			}
			return null;
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x001FF3D4 File Offset: 0x001FD5D4
		public bool IsEmpty()
		{
			if (this.elements == null)
			{
				return true;
			}
			for (int i = 0; i < this.elements.Count; i++)
			{
				UIHyperText.Element element = this.elements[i];
				if (element != null && !element.IsEmpty())
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x001FF41C File Offset: 0x001FD61C
		public Value GetVar(string key, IVars _vars = null, bool as_value = true)
		{
			IVars vars;
			if ((vars = (this.instance_obj as IVars)) != null)
			{
				Value var = vars.GetVar(key, _vars ?? this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			if (this.vars != null)
			{
				Value var = this.vars.GetVar(key, _vars, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num > 1141775739U)
			{
				if (num <= 1728792806U)
				{
					if (num != 1295698969U)
					{
						if (num != 1728792806U)
						{
							goto IL_179;
						}
						if (!(key == "instance_idx"))
						{
							goto IL_179;
						}
						if (this.instance_idx >= 0)
						{
							return this.instance_idx;
						}
						goto IL_179;
					}
					else if (!(key == "ht"))
					{
						goto IL_179;
					}
				}
				else if (num != 3310976652U)
				{
					if (num != 3939368189U)
					{
						goto IL_179;
					}
					if (!(key == "parent"))
					{
						goto IL_179;
					}
				}
				else
				{
					if (!(key == "def"))
					{
						goto IL_179;
					}
					return new Value(this.def);
				}
				return new Value(this.ht);
			}
			if (num != 193386898U)
			{
				if (num != 413646574U)
				{
					if (num == 1141775739U)
					{
						if (key == "row")
						{
							return new Value(this);
						}
					}
				}
				else if (key == "empty")
				{
					return this.IsEmpty();
				}
			}
			else if (key == "instance")
			{
				if (this.instance_idx >= 0)
				{
					return new Value(this.instance_obj);
				}
			}
			IL_179:
			if (this.ht != null)
			{
				Value var = this.ht.GetVar(key, _vars ?? this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			if (key == "vars" && this.vars != null)
			{
				return new Value(this.vars);
			}
			UIHyperText.Element element = this.FindElement(key);
			if (element != null)
			{
				return new Value(element);
			}
			return Value.Unknown;
		}

		// Token: 0x060043E6 RID: 17382 RVA: 0x001FF608 File Offset: 0x001FD808
		public override string ToString()
		{
			string str = "Row: ";
			string type = this.def.type;
			string str2 = " ";
			DT.Field field = this.def;
			string text = str + type + str2 + ((field != null) ? field.Path(false, false, '.') : null);
			if (this.instance_idx >= 0)
			{
				text += string.Format(": {0}, {1}", this.instance_idx, this.instance_obj);
			}
			return text;
		}

		// Token: 0x0400307D RID: 12413
		public UIHyperText ht;

		// Token: 0x0400307E RID: 12414
		public DT.Field def;

		// Token: 0x0400307F RID: 12415
		public IVars vars;

		// Token: 0x04003080 RID: 12416
		public int instance_idx = -1;

		// Token: 0x04003081 RID: 12417
		public object instance_obj;

		// Token: 0x04003082 RID: 12418
		public RectTransform rt;

		// Token: 0x04003083 RID: 12419
		public HorizontalLayoutGroup lg;

		// Token: 0x04003084 RID: 12420
		public LayoutElement le;

		// Token: 0x04003085 RID: 12421
		public List<UIHyperText.Element> elements;

		// Token: 0x04003086 RID: 12422
		public bool optional;

		// Token: 0x04003087 RID: 12423
		public bool empty;

		// Token: 0x04003088 RID: 12424
		public UIText possessed_link_ui_text;
	}

	// Token: 0x0200057C RID: 1404
	public class Element : IVars
	{
		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x060043E8 RID: 17384 RVA: 0x001FF681 File Offset: 0x001FD881
		public TextMeshProUGUI txt
		{
			get
			{
				return this.script as TextMeshProUGUI;
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x060043E9 RID: 17385 RVA: 0x001FF68E File Offset: 0x001FD88E
		public Image img
		{
			get
			{
				return this.script as Image;
			}
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x001FF69B File Offset: 0x001FD89B
		public bool IsEmpty()
		{
			UIHyperText.CheckEmpty(this);
			return this.empty;
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x001FF6AC File Offset: 0x001FD8AC
		public Value GetVar(string key, IVars _vars = null, bool as_value = true)
		{
			IVars vars;
			if ((vars = (this.instance_obj as IVars)) != null)
			{
				Value var = vars.GetVar(key, _vars ?? this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			if (this.vars != null)
			{
				Value var = this.vars.GetVar(key, _vars, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1295698969U)
			{
				if (num <= 413646574U)
				{
					if (num != 193386898U)
					{
						if (num != 413646574U)
						{
							goto IL_1BB;
						}
						if (!(key == "empty"))
						{
							goto IL_1BB;
						}
						return this.IsEmpty();
					}
					else
					{
						if (!(key == "instance"))
						{
							goto IL_1BB;
						}
						if (this.instance_idx >= 0)
						{
							return new Value(this.instance_obj);
						}
						goto IL_1BB;
					}
				}
				else if (num != 1141775739U)
				{
					if (num != 1295698969U)
					{
						goto IL_1BB;
					}
					if (!(key == "ht"))
					{
						goto IL_1BB;
					}
					UIHyperText.Row row = this.row;
					return new Value((row != null) ? row.ht : null);
				}
				else if (!(key == "row"))
				{
					goto IL_1BB;
				}
			}
			else if (num <= 1728792806U)
			{
				if (num != 1330461687U)
				{
					if (num != 1728792806U)
					{
						goto IL_1BB;
					}
					if (!(key == "instance_idx"))
					{
						goto IL_1BB;
					}
					if (this.instance_idx >= 0)
					{
						return this.instance_idx;
					}
					goto IL_1BB;
				}
				else
				{
					if (!(key == "element"))
					{
						goto IL_1BB;
					}
					return new Value(this);
				}
			}
			else if (num != 3310976652U)
			{
				if (num != 3939368189U)
				{
					goto IL_1BB;
				}
				if (!(key == "parent"))
				{
					goto IL_1BB;
				}
			}
			else
			{
				if (!(key == "def"))
				{
					goto IL_1BB;
				}
				return new Value(this.def);
			}
			return new Value(this.row);
			IL_1BB:
			if (this.row != null)
			{
				Value var = this.row.GetVar(key, _vars ?? this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			if (key == "vars" && this.vars != null)
			{
				return new Value(this.vars);
			}
			return Value.Unknown;
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x001FF8C4 File Offset: 0x001FDAC4
		public override string ToString()
		{
			string text = this.type ?? "element";
			string[] array = new string[5];
			array[0] = text;
			array[1] = ": ";
			int num = 2;
			DT.Field field = this.def;
			array[num] = ((field != null) ? field.type : null);
			array[3] = " ";
			int num2 = 4;
			DT.Field field2 = this.def;
			array[num2] = ((field2 != null) ? field2.Path(false, false, '.') : null);
			string text2 = string.Concat(array);
			if (this.instance_idx >= 0)
			{
				text2 += string.Format(": {0}, {1}", this.instance_idx, this.instance_obj);
			}
			return text2;
		}

		// Token: 0x04003089 RID: 12425
		public string type;

		// Token: 0x0400308A RID: 12426
		public UIHyperText.Row row;

		// Token: 0x0400308B RID: 12427
		public DT.Field def;

		// Token: 0x0400308C RID: 12428
		public IVars vars;

		// Token: 0x0400308D RID: 12429
		public int instance_idx = -1;

		// Token: 0x0400308E RID: 12430
		public object instance_obj;

		// Token: 0x0400308F RID: 12431
		public RectTransform rt;

		// Token: 0x04003090 RID: 12432
		public LayoutElement le;

		// Token: 0x04003091 RID: 12433
		public MonoBehaviour script;

		// Token: 0x04003092 RID: 12434
		public Image mask;

		// Token: 0x04003093 RID: 12435
		public Image frame;

		// Token: 0x04003094 RID: 12436
		public bool optional;

		// Token: 0x04003095 RID: 12437
		public bool empty;

		// Token: 0x04003096 RID: 12438
		public float width;

		// Token: 0x04003097 RID: 12439
		public float height;

		// Token: 0x04003098 RID: 12440
		public float flexible_width;
	}

	// Token: 0x0200057D RID: 1405
	public struct CallbackParams : IVars
	{
		// Token: 0x060043EE RID: 17390 RVA: 0x001FF96C File Offset: 0x001FDB6C
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			if (this.cb_field != null)
			{
				Value var = this.cb_field.GetVar(key, this, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1295698969U)
			{
				if (num != 193386898U)
				{
					if (num != 1141775739U)
					{
						if (num == 1295698969U)
						{
							if (key == "ht")
							{
								return new Value(this.ht);
							}
						}
					}
					else if (key == "row")
					{
						return new Value(this.row);
					}
				}
				else if (key == "instance")
				{
					if (this.instance_idx >= 0)
					{
						return new Value(this.instance_obj);
					}
				}
			}
			else if (num <= 3310976652U)
			{
				if (num != 1728792806U)
				{
					if (num == 3310976652U)
					{
						if (key == "def")
						{
							return new Value(this.def);
						}
					}
				}
				else if (key == "instance_idx")
				{
					if (this.instance_idx >= 0)
					{
						return new Value(this.instance_idx);
					}
				}
			}
			else if (num != 3758891744U)
			{
				if (num == 4262377039U)
				{
					if (key == "cb_field")
					{
						return new Value(this.cb_field);
					}
				}
			}
			else if (key == "e")
			{
				return new Value(this.e);
			}
			if (this.e != null)
			{
				return this.e.GetVar(key, vars, as_value);
			}
			if (this.row != null)
			{
				return this.row.GetVar(key, vars, as_value);
			}
			if (this.ht != null)
			{
				return this.ht.GetVar(key, vars, as_value);
			}
			return Value.Unknown;
		}

		// Token: 0x04003099 RID: 12441
		public UIHyperText ht;

		// Token: 0x0400309A RID: 12442
		public UIHyperText.Row row;

		// Token: 0x0400309B RID: 12443
		public UIHyperText.Element e;

		// Token: 0x0400309C RID: 12444
		public DT.Field def;

		// Token: 0x0400309D RID: 12445
		public DT.Field cb_field;

		// Token: 0x0400309E RID: 12446
		public int instance_idx;

		// Token: 0x0400309F RID: 12447
		public object instance_obj;
	}

	// Token: 0x0200057E RID: 1406
	// (Invoke) Token: 0x060043F0 RID: 17392
	public delegate Value CallbackFunc(UIHyperText.CallbackParams arg);

	// Token: 0x0200057F RID: 1407
	// (Invoke) Token: 0x060043F4 RID: 17396
	private delegate void Loader(UIHyperText ht, DT.Field f, int grp_idx = -1, object grp_obj = null);
}
