using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002B8 RID: 696
public class UIProvinceFeature : ObjectIcon
{
	// Token: 0x06002BA2 RID: 11170 RVA: 0x0016FBB4 File Offset: 0x0016DDB4
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		if (obj is string)
		{
			this.def = this.FindResoureDef(obj as string);
			if (this.def == null)
			{
				Debug.LogWarning("Unknown ProvinceFeature: " + obj);
			}
			UICommon.FindComponents(this, false);
			this.Refresh();
			return;
		}
		Debug.Log("(UIGoodsIcon) object of type string expected");
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x0016FC14 File Offset: 0x0016DE14
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
		Vars vars = new Vars();
		vars.Set<ProvinceFeature.Def>("province_feature", this.def.def as ProvinceFeature.Def);
		Tooltip.Get(base.gameObject, true).SetDef("ProvinceFeatureTooltip", vars);
	}

	// Token: 0x06002BA4 RID: 11172 RVA: 0x0016FC90 File Offset: 0x0016DE90
	public static Value SetupHTTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		IVars vars = (ht != null) ? ht.vars : null;
		if (vars == null)
		{
			return Value.Unknown;
		}
		ProvinceFeature.Def def = vars.GetVar("province_feature", null, true).Get<ProvinceFeature.Def>();
		if (def == null)
		{
			def = vars.GetVar("obj", null, true).Get<ProvinceFeature.Def>();
			if (def == null)
			{
				return Value.Unknown;
			}
		}
		Vars vars2 = vars as Vars;
		if (vars2 == null)
		{
			vars2 = new Vars(vars);
			arg.ht.vars = vars2;
		}
		Logic.Kingdom k = (UIText.cur_article != null) ? null : BaseUI.LogicKingdom();
		UIResources.FillAvailability(vars2, "ProvinceFeature.availability_texts", def.id, k);
		return Value.Unknown;
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x0016FD37 File Offset: 0x0016DF37
	private void Update()
	{
		this.UpdateState();
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x0016FD40 File Offset: 0x0016DF40
	private UIProvinceFeature.State GetState()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		ResourceInfo resourceInfo;
		if (kingdom == null)
		{
			resourceInfo = null;
		}
		else
		{
			DT.Def def = this.def;
			resourceInfo = kingdom.GetResourceInfo((def != null) ? def.def.id : null, true, true);
		}
		ResourceInfo resourceInfo2 = resourceInfo;
		if (resourceInfo2 == null)
		{
			return UIProvinceFeature.State.Neutral;
		}
		if (resourceInfo2.availability == ResourceInfo.Availability.Available)
		{
			return UIProvinceFeature.State.Available;
		}
		return UIProvinceFeature.State.Missing;
	}

	// Token: 0x06002BA7 RID: 11175 RVA: 0x0016FD88 File Offset: 0x0016DF88
	private void UpdateState()
	{
		UIProvinceFeature.State state = this.GetState();
		if (state == this.state)
		{
			return;
		}
		this.state = state;
		if (this.m_Border != null)
		{
			string key = (this.state == UIProvinceFeature.State.Missing) ? "border_not_available" : "border_normal";
			this.m_Border.overrideSprite = global::Defs.GetObj<Sprite>("ProvinceFeatureIcon", key, null);
		}
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x0016FDE8 File Offset: 0x0016DFE8
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_Border != null)
		{
			Color color = UIBuildingSlot.Settings.RelationColor(this.m_CurRelationHighlight);
			this.m_Border.color = color;
		}
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x0016FE28 File Offset: 0x0016E028
	protected override void OnEnable()
	{
		BaseUI.on_picked_element_changed = (BaseUI.OnPickedElementChanged)Delegate.Combine(BaseUI.on_picked_element_changed, new BaseUI.OnPickedElementChanged(this.OnUIPickerChanged));
		base.OnEnable();
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x0016FE50 File Offset: 0x0016E050
	protected override void OnDisable()
	{
		BaseUI.on_picked_element_changed = (BaseUI.OnPickedElementChanged)Delegate.Remove(BaseUI.on_picked_element_changed, new BaseUI.OnPickedElementChanged(this.OnUIPickerChanged));
		base.OnDisable();
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x0016FE78 File Offset: 0x0016E078
	private string GetRelation(Hotspot picked_hotspot)
	{
		if (picked_hotspot == null)
		{
			return "none";
		}
		if (picked_hotspot == this)
		{
			return "this";
		}
		DT.Def def = this.def;
		if (((def != null) ? def.path : null) == null)
		{
			return "none";
		}
		UIBuildingSlot uibuildingSlot = picked_hotspot as UIBuildingSlot;
		if (((uibuildingSlot != null) ? uibuildingSlot.Def : null) != null)
		{
			int num = uibuildingSlot.Def.CalcRequires(this.def.path, uibuildingSlot.Castle, false);
			if (num == 1)
			{
				return "enables";
			}
			if (num > 1)
			{
				return "enables_indirect";
			}
		}
		return "none";
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x0016FF0C File Offset: 0x0016E10C
	private void OnUIPickerChanged(BaseUI ui)
	{
		string relation = this.GetRelation(ui.picked_hotspot);
		if (relation == this.m_CurRelationHighlight)
		{
			return;
		}
		this.m_CurRelationHighlight = relation;
		this.UpdateHighlight();
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x0016FF42 File Offset: 0x0016E142
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		this.OnSelected(this, e);
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x0016F6AC File Offset: 0x0016D8AC
	private DT.Def FindResoureDef(string key)
	{
		DT.Field defField = global::Defs.GetDefField(key, null);
		if (defField == null)
		{
			return null;
		}
		return defField.def;
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x0016FF58 File Offset: 0x0016E158
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
		GameObject obj = global::Defs.GetObj<GameObject>("ProvinceFeatureIcon", text, null);
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity, parent);
		UIProvinceFeature component = gameObject.GetComponent<UIProvinceFeature>();
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

	// Token: 0x06002BB0 RID: 11184 RVA: 0x0016FFF0 File Offset: 0x0016E1F0
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "obj_type")
		{
			return "ProvinceFeature";
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

	// Token: 0x04001DB6 RID: 7606
	private DT.Def def;

	// Token: 0x04001DB7 RID: 7607
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x04001DB8 RID: 7608
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04001DB9 RID: 7609
	public Action<UIProvinceFeature, PointerEventData> OnSelected;

	// Token: 0x04001DBA RID: 7610
	private string m_CurRelationHighlight = "none";

	// Token: 0x04001DBB RID: 7611
	private UIProvinceFeature.State state;

	// Token: 0x02000812 RID: 2066
	private enum State
	{
		// Token: 0x04003DA2 RID: 15778
		Neutral,
		// Token: 0x04003DA3 RID: 15779
		Available,
		// Token: 0x04003DA4 RID: 15780
		Missing
	}
}
