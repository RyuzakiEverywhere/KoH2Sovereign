using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000302 RID: 770
public class ObjectIcon : Hotspot
{
	// Token: 0x0600302F RID: 12335 RVA: 0x00187E38 File Offset: 0x00186038
	protected void HandleOnClick(PointerEventData e)
	{
		Logic.Object obj;
		if ((obj = (this.logicObject as Logic.Object)) != null && UICommon.CheckModifierKeys(UICommon.ModifierKey.Shift, UICommon.ModifierKey.None))
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI != null)
			{
				baseUI.HandleChatLink(obj);
			}
			return;
		}
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x00187E74 File Offset: 0x00186074
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		this.HandleOnClick(e);
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x00187E84 File Offset: 0x00186084
	public virtual void SetObject(object obj, Vars vars = null)
	{
		this.logicObject = obj;
		if (vars == null)
		{
			vars = new Vars(obj);
		}
		this.vars = vars;
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x00187E9F File Offset: 0x0018609F
	public virtual object GetObject(Logic.Object obj)
	{
		return this.logicObject;
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x00187EA7 File Offset: 0x001860A7
	public static GameObject GetIcon(Value value, Vars vars, RectTransform parent)
	{
		if (!value.is_valid)
		{
			return null;
		}
		if (value.is_object)
		{
			return ObjectIcon.GetIcon(value.obj_val, vars, parent);
		}
		return null;
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x00187ECC File Offset: 0x001860CC
	public static GameObject GetIcon(string className, Vars vars, RectTransform parent)
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
		DT.Def def = ObjectIcon.GetDef(className);
		if (def == null || def.field == null)
		{
			return null;
		}
		GameObject obj = global::Defs.GetObj<GameObject>(def.field, text, null);
		if (obj == null)
		{
			obj = global::Defs.GetObj<GameObject>(ObjectIcon.GetDef(className).field, "prefab", null);
		}
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj, Vector3.zero, Quaternion.identity, parent);
		ObjectIcon component = gameObject.GetComponent<ObjectIcon>();
		if (component != null)
		{
			component.SetObject(null, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object icon logic");
		}
		return gameObject;
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x00187F9C File Offset: 0x0018619C
	public static GameObject GetIcon(object obj, Vars vars, RectTransform parent)
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
		DT.Def def = ObjectIcon.GetDef(obj);
		if (def == null || def.field == null)
		{
			return null;
		}
		GameObject obj2 = global::Defs.GetObj<GameObject>(def.field, text, null);
		if (obj2 == null)
		{
			obj2 = global::Defs.GetObj<GameObject>(ObjectIcon.GetDef(obj).field, "prefab", null);
		}
		if (obj2 == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(obj2, parent, false, "");
		ObjectIcon component = gameObject.GetComponent<ObjectIcon>();
		if (component != null)
		{
			component.SetObject(obj, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object icon logic");
		}
		return gameObject;
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x00188068 File Offset: 0x00186268
	public static DT.Def GetDef(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return null;
		}
		Type type = obj.GetType();
		DT.Def def;
		for (;;)
		{
			string path = Logic.Object.TypeToStr(type) + "Icon";
			def = defs.dt.FindDef(path);
			if (def != null)
			{
				break;
			}
			type = type.BaseType;
			if (type == null)
			{
				goto Block_4;
			}
		}
		return def;
		Block_4:
		DT.Def def2 = null;
		if (obj is string)
		{
			def2 = defs.dt.FindDef(obj as string);
		}
		else if (obj is DT.Field)
		{
			string key = (obj as DT.Field).key;
			def2 = defs.dt.FindDef(key);
		}
		else if (obj is Def)
		{
			string key2 = (obj as Def).field.key;
			def2 = defs.dt.FindDef(key2);
		}
		if (def2 != null)
		{
			DT.Def def3;
			for (;;)
			{
				string path2 = def2.field.key + "Icon";
				def3 = defs.dt.FindDef(path2);
				if (def3 != null)
				{
					break;
				}
				path2 = def2.field.key + "Slot";
				def3 = defs.dt.FindDef(path2);
				if (def3 != null)
				{
					return def3;
				}
				DT.Def def4;
				if (def2 == null)
				{
					def4 = null;
				}
				else
				{
					Def def5 = def2.def;
					if (def5 == null)
					{
						def4 = null;
					}
					else
					{
						Def def6 = def5.BasedOn();
						def4 = ((def6 != null) ? def6.dt_def : null);
					}
				}
				def2 = def4;
				if (def2 == null)
				{
					goto IL_14A;
				}
			}
			return def3;
		}
		IL_14A:
		if (obj is string)
		{
			DT.Def def7 = defs.dt.FindDef(obj + "Icon");
			if (def7 != null)
			{
				return def7;
			}
		}
		return null;
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x001881E7 File Offset: 0x001863E7
	public Logic.Object GetLogicObj()
	{
		return this.logicObject as Logic.Object;
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x001881F4 File Offset: 0x001863F4
	public string GetObjTypeName()
	{
		if (this.logicObject == null)
		{
			return null;
		}
		BaseObject baseObject = this.logicObject as BaseObject;
		if (baseObject != null)
		{
			return baseObject.rtti.name;
		}
		return this.logicObject.GetType().Name;
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x00188236 File Offset: 0x00186436
	public Def GetObjDef()
	{
		return Def.Get(global::Defs.GetDefField(this.GetObjTypeName(), null));
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x0018824C File Offset: 0x0018644C
	public override Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "obj_type")
		{
			return this.GetObjTypeName();
		}
		if (key == "def_id")
		{
			Def objDef = this.GetObjDef();
			return (objDef != null) ? objDef.id : null;
		}
		if (key == "def")
		{
			return this.GetObjDef();
		}
		if (key == "obj")
		{
			return new Value(this.logicObject);
		}
		if (!(key == "vars"))
		{
			return base.GetVar(key, vars, as_value);
		}
		return new Value(vars);
	}

	// Token: 0x04002068 RID: 8296
	public object logicObject;

	// Token: 0x04002069 RID: 8297
	public Vars vars;
}
