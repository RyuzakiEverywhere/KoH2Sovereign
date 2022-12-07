using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B5 RID: 693
public class UIGoodsIcon : ObjectIcon
{
	// Token: 0x06002B7F RID: 11135 RVA: 0x0016F578 File Offset: 0x0016D778
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		UICommon.FindComponents(this, false);
		if (obj is string)
		{
			this.def = this.FindResoureDef(obj as string);
			this.Refresh();
			return;
		}
		if (obj is DT.Def)
		{
			this.def = (obj as DT.Def);
			this.Refresh();
			return;
		}
		Debug.Log("(UIGoodsIcon) object of type string expected");
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x0016F5DC File Offset: 0x0016D7DC
	private void Refresh()
	{
		if (this.def == null)
		{
			return;
		}
		if (this.m_Icon)
		{
			this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.def.field, "icon", null);
		}
		Vars vars = new Vars(this.def.def);
		vars.Set<Def>("resource", this.def.def);
		vars.Set<Vars.Func0>("kingdom", new Vars.Func0(BaseUI.LogicKingdom));
		Tooltip.Get(base.gameObject, true).SetDef("ResourceTooltip", vars);
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000DB26E File Offset: 0x000D946E
	public void UpdateHighlight()
	{
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x0016F67A File Offset: 0x0016D87A
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x0016F689 File Offset: 0x0016D889
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x0016F698 File Offset: 0x0016D898
	public override void OnClick(PointerEventData e)
	{
		Action<UIGoodsIcon, PointerEventData> onSelected = this.OnSelected;
		if (onSelected == null)
		{
			return;
		}
		onSelected(this, e);
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x0016F6AC File Offset: 0x0016D8AC
	private DT.Def FindResoureDef(string key)
	{
		DT.Field defField = global::Defs.GetDefField(key, null);
		if (defField == null)
		{
			return null;
		}
		return defField.def;
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x0016F6C0 File Offset: 0x0016D8C0
	public new static GameObject GetIcon(string key, Vars vars, RectTransform parent)
	{
		string text = "prefab";
		if (vars != null)
		{
			string text2 = vars.Get<string>("variant", null);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "." + text2;
			}
		}
		GameObject obj = global::Defs.GetObj<GameObject>("GoodsIcon", text, null);
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity, parent);
		ObjectIcon component = gameObject.GetComponent<ObjectIcon>();
		if (component != null)
		{
			component.SetObject(key, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object icon logic");
		}
		return gameObject;
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x0016F758 File Offset: 0x0016D958
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "obj_type")
		{
			return "Resource";
		}
		if (key == "def_id")
		{
			DT.Def def = this.def;
			return (def != null) ? def.path : null;
		}
		if (key == "def")
		{
			DT.Def def2 = this.def;
			return (def2 != null) ? def2.def : null;
		}
		if (!(key == "obj"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return new Value(this.def);
	}

	// Token: 0x04001DAA RID: 7594
	private DT.Def def;

	// Token: 0x04001DAB RID: 7595
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001DAC RID: 7596
	public Action<UIGoodsIcon, PointerEventData> OnSelected;
}
