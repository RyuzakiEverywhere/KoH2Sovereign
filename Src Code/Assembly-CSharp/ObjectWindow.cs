using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class ObjectWindow : MonoBehaviour, IListener, BaseUI.ISelectionPanel
{
	// Token: 0x0600303C RID: 12348 RVA: 0x001882E8 File Offset: 0x001864E8
	protected virtual void Awake()
	{
		this.wasInitalzied = true;
		if (this.addListeners)
		{
			this.AddListeners();
		}
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x00188300 File Offset: 0x00186500
	public virtual void SetObject(Logic.Object obj, Vars vars = null)
	{
		this.RemoveListeners();
		this.logicObject = obj;
		this.vars = vars;
		if (this.wasInitalzied)
		{
			this.AddListeners();
		}
		else
		{
			this.addListeners = true;
		}
		Vars.ReflectionMode old_mode = Vars.PushReflectionMode(Vars.ReflectionMode.Enabled);
		Vars.GetExact(obj, "stats", null, true).Get<Stats>();
		Vars.PopReflectionMode(old_mode);
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x00188359 File Offset: 0x00186559
	public virtual void AddListeners()
	{
		if (this.logicObject != null)
		{
			this.logicObject.AddListener(this);
		}
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x0018836F File Offset: 0x0018656F
	public virtual void RemoveListeners()
	{
		if (this.logicObject != null)
		{
			this.logicObject.DelListener(this);
		}
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void Refresh()
	{
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00188388 File Offset: 0x00186588
	protected virtual void OnDestroy()
	{
		this.RemoveListeners();
		this.logicObject = null;
		if (this.prototype != null && ObjectWindow.sm_FreeInstances.ContainsKey(this.prototype))
		{
			ObjectWindow.sm_FreeInstances[this.prototype].Remove(this);
		}
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x001883D9 File Offset: 0x001865D9
	protected virtual void Update()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.Refresh();
		}
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x001883F0 File Offset: 0x001865F0
	public virtual Logic.Object GetObject(Logic.Object obj)
	{
		return this.logicObject;
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x001883F8 File Offset: 0x001865F8
	public static GameObject GetPrefab(Logic.Object obj, Vars vars = null)
	{
		if (obj == null)
		{
			Debug.LogWarning("Logic.Object is null");
			return null;
		}
		string text = "prefab";
		if (vars != null)
		{
			string text2 = vars.Get<string>("variant", null);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "." + text2;
			}
		}
		DT.Def def = ObjectWindow.GetDef(obj);
		if (def == null || def.field == null)
		{
			Debug.LogWarning("Cannot get def from " + obj.ToString());
			return null;
		}
		GameObject obj2 = global::Defs.GetObj<GameObject>(def.field, text, null);
		if (obj2 == null)
		{
			obj2 = global::Defs.GetObj<GameObject>(def.field, "prefab", null);
		}
		return obj2;
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x00188494 File Offset: 0x00186694
	public static ObjectWindow GetFreeWindow(GameObject prefab)
	{
		if (ObjectWindow.sm_FreeInstances == null)
		{
			return null;
		}
		if (!ObjectWindow.sm_FreeInstances.ContainsKey(prefab))
		{
			return null;
		}
		if (ObjectWindow.sm_FreeInstances[prefab] == null)
		{
			return null;
		}
		if (ObjectWindow.sm_FreeInstances[prefab].Count == 0)
		{
			return null;
		}
		ObjectWindow objectWindow = ObjectWindow.sm_FreeInstances[prefab][0];
		ObjectWindow.sm_FreeInstances[prefab].Remove(objectWindow);
		return objectWindow;
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x00188504 File Offset: 0x00186704
	public static GameObject GetWindow(Logic.Object obj, Vars vars, RectTransform parent)
	{
		GameObject prefab = ObjectWindow.GetPrefab(obj, vars);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
		ObjectWindow component = gameObject.GetComponent<ObjectWindow>();
		if (component != null)
		{
			component.SetObject(obj, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object window logic");
		}
		return gameObject;
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x0018855C File Offset: 0x0018675C
	public static DT.Def GetDef(Logic.Object obj)
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
		Reflection.TypeInfo typeInfo = obj.rtti;
		DT.Def def;
		for (;;)
		{
			string path = typeInfo.name + "Window";
			def = defs.dt.FindDef(path);
			if (def != null)
			{
				break;
			}
			typeInfo = typeInfo.base_rtti;
			if (typeInfo == null)
			{
				goto Block_4;
			}
		}
		return def;
		Block_4:
		typeInfo = obj.rtti;
		DT.Def def2;
		for (;;)
		{
			string name = typeInfo.name;
			def2 = defs.dt.FindDef(name);
			if (def2 != null)
			{
				break;
			}
			typeInfo = typeInfo.base_rtti;
			if (typeInfo == null)
			{
				goto Block_6;
			}
		}
		return def2;
		Block_6:
		return null;
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x001885E4 File Offset: 0x001867E4
	void IListener.OnMessage(object obj, string message, object param)
	{
		this.HandleLogicMessage(obj, message, param);
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x001885F0 File Offset: 0x001867F0
	protected virtual void HandleLogicMessage(object obj, string message, object param)
	{
		if (obj != this.logicObject)
		{
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			(obj as Logic.Object).DelListener(this);
			this.logicObject = null;
			if (base.gameObject != null)
			{
				if (this != null && ((BaseUI.ISelectionPanel)this).PreserveWindow())
				{
					((BaseUI.ISelectionPanel)this).Release();
					return;
				}
				global::Common.DestroyObj(base.gameObject);
			}
			return;
		}
		if (message == "stat_changed")
		{
			this.dirty = true;
			return;
		}
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x000FC240 File Offset: 0x000FA440
	public bool IsDestoryed()
	{
		return this == null && this != null;
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void StoreState()
	{
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void RestoreState()
	{
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x00188678 File Offset: 0x00186878
	public virtual void Release()
	{
		this.RemoveListeners();
		this.logicObject = null;
		if (this.prototype != null)
		{
			base.gameObject.SetActive(false);
			if (!ObjectWindow.sm_FreeInstances.ContainsKey(this.prototype))
			{
				ObjectWindow.sm_FreeInstances.Add(this.prototype, new List<ObjectWindow>());
			}
			ObjectWindow.sm_FreeInstances[this.prototype].Add(this);
			return;
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x0002C53B File Offset: 0x0002A73B
	public virtual bool PreserveWindow()
	{
		return true;
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x001886F5 File Offset: 0x001868F5
	public GameObject GetPrototype()
	{
		return this.prototype;
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x001886FD File Offset: 0x001868FD
	public void SetPrototype(GameObject prorotype)
	{
		this.prototype = prorotype;
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void ValidateSelectionObject()
	{
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x000FC361 File Offset: 0x000FA561
	GameObject BaseUI.ISelectionPanel.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x00188712 File Offset: 0x00186912
	T BaseUI.ISelectionPanel.GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x0400206A RID: 8298
	public Logic.Object logicObject;

	// Token: 0x0400206B RID: 8299
	public Vars vars;

	// Token: 0x0400206C RID: 8300
	public bool dirty;

	// Token: 0x0400206D RID: 8301
	private bool wasInitalzied;

	// Token: 0x0400206E RID: 8302
	private bool addListeners;

	// Token: 0x0400206F RID: 8303
	private GameObject prototype;

	// Token: 0x04002070 RID: 8304
	private static Dictionary<GameObject, List<ObjectWindow>> sm_FreeInstances = new Dictionary<GameObject, List<ObjectWindow>>();
}
