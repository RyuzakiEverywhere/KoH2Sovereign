using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class MultipleObjectWindow : MonoBehaviour, IListener, BaseUI.ISelectionPanel
{
	// Token: 0x06003015 RID: 12309 RVA: 0x00187989 File Offset: 0x00185B89
	protected virtual void Awake()
	{
		this.wasInitalzied = true;
		if (this.addListeners)
		{
			this.AddListeners();
		}
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x001879A0 File Offset: 0x00185BA0
	public virtual void SetObjects(List<Logic.Object> obj, Vars vars = null)
	{
		this.RemoveListeners();
		this.logicObjects = obj;
		this.vars = vars;
		if (this.wasInitalzied)
		{
			this.AddListeners();
		}
		else
		{
			this.addListeners = true;
		}
		Vars.PopReflectionMode(Vars.PushReflectionMode(Vars.ReflectionMode.Enabled));
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x001879D8 File Offset: 0x00185BD8
	public virtual void AddListeners()
	{
		if (this.logicObjects == null || this.logicObjects.Count == 0)
		{
			return;
		}
		foreach (Logic.Object @object in this.logicObjects)
		{
			if (@object != null)
			{
				@object.AddListener(this);
			}
		}
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x00187A44 File Offset: 0x00185C44
	public virtual void RemoveListeners()
	{
		if (this.logicObjects == null)
		{
			return;
		}
		foreach (Logic.Object @object in this.logicObjects)
		{
			if (@object != null)
			{
				@object.DelListener(this);
			}
		}
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void Refresh()
	{
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x00187AA4 File Offset: 0x00185CA4
	protected virtual void OnDestroy()
	{
		this.RemoveListeners();
		this.logicObjects = null;
		if (this.prototype != null && MultipleObjectWindow.sm_FreeInstances.ContainsKey(this.prototype))
		{
			MultipleObjectWindow.sm_FreeInstances[this.prototype].Remove(this);
		}
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x00187AF5 File Offset: 0x00185CF5
	protected virtual void Update()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.Refresh();
		}
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x00187B0C File Offset: 0x00185D0C
	public virtual List<Logic.Object> GetObjects(Logic.Object obj)
	{
		return this.logicObjects;
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x00187B14 File Offset: 0x00185D14
	public static GameObject GetPrefab(List<Logic.Object> obj, Vars vars = null)
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
		DT.Def def = MultipleObjectWindow.GetDef(obj, vars);
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

	// Token: 0x0600301E RID: 12318 RVA: 0x00187BB0 File Offset: 0x00185DB0
	public static MultipleObjectWindow GetFreeWindow(GameObject prefab)
	{
		if (MultipleObjectWindow.sm_FreeInstances == null)
		{
			return null;
		}
		if (!MultipleObjectWindow.sm_FreeInstances.ContainsKey(prefab))
		{
			return null;
		}
		if (MultipleObjectWindow.sm_FreeInstances[prefab] == null)
		{
			return null;
		}
		if (MultipleObjectWindow.sm_FreeInstances[prefab].Count == 0)
		{
			return null;
		}
		MultipleObjectWindow multipleObjectWindow = MultipleObjectWindow.sm_FreeInstances[prefab][0];
		MultipleObjectWindow.sm_FreeInstances[prefab].Remove(multipleObjectWindow);
		return multipleObjectWindow;
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x00187C20 File Offset: 0x00185E20
	public static GameObject GetWindow(List<Logic.Object> obj, Vars vars, RectTransform parent)
	{
		GameObject prefab = MultipleObjectWindow.GetPrefab(obj, vars);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
		MultipleObjectWindow component = gameObject.GetComponent<MultipleObjectWindow>();
		if (component != null)
		{
			component.SetObjects(obj, vars);
		}
		else
		{
			Debug.Log("instanceGO " + gameObject + "is missing an object window logic");
		}
		return gameObject;
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x00187C78 File Offset: 0x00185E78
	public static DT.Def GetDef(List<Logic.Object> obj, Vars vars = null)
	{
		if (obj == null)
		{
			return null;
		}
		global::Defs defs = global::Defs.Get(false);
		if (defs == null || vars == null)
		{
			return null;
		}
		string text = vars.Get<string>("window_name", null);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return defs.dt.FindDef(text);
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x00187CC2 File Offset: 0x00185EC2
	void IListener.OnMessage(object obj, string message, object param)
	{
		this.HandleLogicMessage(obj, message, param);
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x00187CD0 File Offset: 0x00185ED0
	protected virtual void HandleLogicMessage(object obj, string message, object param)
	{
		if (obj == null)
		{
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			Logic.Object @object = obj as Logic.Object;
			if (!this.logicObjects.Contains(@object))
			{
				return;
			}
			@object.DelListener(this);
			this.logicObjects.Remove(@object);
			if (base.gameObject != null && this.logicObjects.Count == 0)
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
		else
		{
			if (message == "stat_changed")
			{
				this.dirty = true;
				return;
			}
			return;
		}
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x000FC240 File Offset: 0x000FA440
	public bool IsDestoryed()
	{
		return this == null && this != null;
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void StoreState()
	{
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void RestoreState()
	{
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x00187D78 File Offset: 0x00185F78
	public virtual void Release()
	{
		this.RemoveListeners();
		this.logicObjects.Clear();
		this.logicObjects = null;
		if (this.prototype != null)
		{
			base.gameObject.SetActive(false);
			if (!MultipleObjectWindow.sm_FreeInstances.ContainsKey(this.prototype))
			{
				MultipleObjectWindow.sm_FreeInstances.Add(this.prototype, new List<MultipleObjectWindow>());
			}
			MultipleObjectWindow.sm_FreeInstances[this.prototype].Add(this);
			return;
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x0002C53B File Offset: 0x0002A73B
	public virtual bool PreserveWindow()
	{
		return true;
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x00187E00 File Offset: 0x00186000
	public GameObject GetPrototype()
	{
		return this.prototype;
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x00187E08 File Offset: 0x00186008
	public void SetPrototype(GameObject prorotype)
	{
		this.prototype = prorotype;
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void ValidateSelectionObject()
	{
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x000FC361 File Offset: 0x000FA561
	GameObject BaseUI.ISelectionPanel.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x00187E30 File Offset: 0x00186030
	T BaseUI.ISelectionPanel.GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x04002061 RID: 8289
	public List<Logic.Object> logicObjects = new List<Logic.Object>();

	// Token: 0x04002062 RID: 8290
	public Vars vars;

	// Token: 0x04002063 RID: 8291
	public bool dirty;

	// Token: 0x04002064 RID: 8292
	private bool wasInitalzied;

	// Token: 0x04002065 RID: 8293
	private bool addListeners;

	// Token: 0x04002066 RID: 8294
	private GameObject prototype;

	// Token: 0x04002067 RID: 8295
	private static Dictionary<GameObject, List<MultipleObjectWindow>> sm_FreeInstances = new Dictionary<GameObject, List<MultipleObjectWindow>>();
}
