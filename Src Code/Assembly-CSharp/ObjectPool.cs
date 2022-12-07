using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class ObjectPool : MonoBehaviour
{
	// Token: 0x060004D4 RID: 1236 RVA: 0x000384DC File Offset: 0x000366DC
	public static GameObject Spawn(GameObject prefab, Transform parent = null, bool force_pooled = false)
	{
		if (prefab == null)
		{
			return null;
		}
		List<ObjectPool.FreedObject> list;
		ObjectPool.AllocatedObject allocatedObject;
		if (ObjectPool.freed.TryGetValue(prefab, out list))
		{
			while (list.Count > 0)
			{
				ObjectPool.FreedObject freedObject = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				if (!(freedObject.obj == null))
				{
					allocatedObject.prefab = prefab;
					allocatedObject.pool = list;
					allocatedObject.script = freedObject.script;
					ObjectPool.allocated.Add(freedObject.obj, allocatedObject);
					ObjectPool.ActivateObject(freedObject.obj, allocatedObject, parent, false);
					return freedObject.obj;
				}
				Debug.LogWarning(string.Format("Pooled object destroyed: prefab: {0}, obj: {1}, script: {2}", prefab, freedObject.obj, freedObject.script));
			}
		}
		GameObject gameObject = ObjectPool.SpawnObject(prefab);
		if (gameObject == null)
		{
			return null;
		}
		IPoolable component = gameObject.GetComponent<IPoolable>();
		if (component == null && !force_pooled)
		{
			if (parent != null)
			{
				gameObject.transform.SetParent(parent, false);
			}
			return gameObject;
		}
		if (!ObjectPool.active)
		{
			Debug.LogError(string.Format("Spawning pooled object {0} during scene destruction", gameObject));
		}
		ObjectPool.Get();
		allocatedObject.prefab = prefab;
		allocatedObject.pool = list;
		allocatedObject.script = component;
		ObjectPool.allocated.Add(gameObject, allocatedObject);
		gameObject.AddComponent<PooledObject>();
		ObjectPool.ActivateObject(gameObject, allocatedObject, parent, true);
		return gameObject;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00038634 File Offset: 0x00036834
	public static void DestroyObj(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		ObjectPool.AllocatedObject allocatedObject;
		if (!ObjectPool.allocated.TryGetValue(obj, out allocatedObject))
		{
			if (obj != null)
			{
				obj.SetActive(false);
				Profile.BeginSection("ObjectPool.DestroyPooledChildrenRecursive");
				ObjectPool.DestroyPooledChildrenRecursive(obj);
				Profile.EndSection("ObjectPool.DestroyPooledChildrenRecursive");
				ObjectPool.DestroyObject(obj);
			}
			return;
		}
		ObjectPool.allocated.Remove(obj);
		if (obj == null)
		{
			return;
		}
		ObjectPool.DeactivateObject(obj, allocatedObject);
		if (allocatedObject.pool == null && !ObjectPool.freed.TryGetValue(allocatedObject.prefab, out allocatedObject.pool))
		{
			allocatedObject.pool = new List<ObjectPool.FreedObject>(1000);
			ObjectPool.freed.Add(allocatedObject.prefab, allocatedObject.pool);
		}
		ObjectPool.FreedObject item;
		item.obj = obj;
		item.script = allocatedObject.script;
		allocatedObject.pool.Add(item);
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0003870C File Offset: 0x0003690C
	public static void DestroyPooledChildrenRecursive(GameObject obj)
	{
		Transform transform = obj.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			ObjectPool.AllocatedObject allocatedObject;
			if (ObjectPool.allocated.TryGetValue(gameObject, out allocatedObject))
			{
				ObjectPool.DestroyObj(gameObject);
				i--;
			}
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			ObjectPool.DestroyPooledChildrenRecursive(transform.GetChild(j).gameObject);
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0003877E File Offset: 0x0003697E
	private static GameObject SpawnObject(GameObject prefab)
	{
		Profile.BeginSection("ObjectPool.SpawnObject");
		GameObject result = Object.Instantiate<GameObject>(prefab);
		Profile.EndSection("ObjectPool.SpawnObject");
		return result;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0003879C File Offset: 0x0003699C
	private static void DestroyObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Profile.BeginSection("ObjectPool.DestroyObject");
		if (Application.isPlaying)
		{
			if (obj.transform is RectTransform && ObjectPool.active)
			{
				obj.SetActive(false);
				obj.transform.SetParent(null, false);
			}
			Object.Destroy(obj);
		}
		else
		{
			Object.DestroyImmediate(obj);
		}
		Profile.EndSection("ObjectPool.DestroyObject");
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00038804 File Offset: 0x00036A04
	private static void ActivateObject(GameObject go, ObjectPool.AllocatedObject ao, Transform parent, bool first_time)
	{
		Profile.BeginSection("ObjectPool.ActivateObject");
		if (parent != null)
		{
			go.transform.SetParent(parent, false);
		}
		if (ao.script != null)
		{
			if (first_time)
			{
				ao.script.OnPoolSpawned();
			}
			else
			{
				ao.script.OnPoolActivated();
			}
		}
		go.SetActive(true);
		Profile.EndSection("ObjectPool.ActivateObject");
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00038868 File Offset: 0x00036A68
	private static void DeactivateObject(GameObject go, ObjectPool.AllocatedObject ao)
	{
		Profile.BeginSection("ObjectPool.DeactivateObject");
		if (ao.script != null)
		{
			ao.script.OnPoolDeactivated();
		}
		go.SetActive(false);
		Transform transform = ObjectPool.Get().transform;
		if (go.GetComponent<RectTransform>())
		{
			transform = transform.Find("UI");
		}
		else
		{
			transform = transform.Find("Map");
		}
		go.transform.SetParent(transform, false);
		Profile.EndSection("ObjectPool.DeactivateObject");
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x000388E2 File Offset: 0x00036AE2
	private static void CleanPools()
	{
		ObjectPool.freed.Clear();
		ObjectPool.allocated.Clear();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x000388F8 File Offset: 0x00036AF8
	public static void OnPooledObjectDestoryed(GameObject obj)
	{
		if (!ObjectPool.active)
		{
			return;
		}
		ObjectPool.AllocatedObject allocatedObject;
		if (!ObjectPool.allocated.TryGetValue(obj, out allocatedObject))
		{
			return;
		}
		string format = "Pooled object '{0}' (prefab: {1}) is destroyed directly";
		GameObject prefab = allocatedObject.prefab;
		Debug.LogWarning(string.Format(format, obj, (prefab != null) ? prefab.name : null));
		ObjectPool.allocated.Remove(obj);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0003894B File Offset: 0x00036B4B
	private static ObjectPool Get()
	{
		if (ObjectPool.instance == null)
		{
			ObjectPool.instance = ObjectPool.Create();
		}
		return ObjectPool.instance;
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0003896C File Offset: 0x00036B6C
	private static ObjectPool Create()
	{
		GameObject gameObject = new GameObject("Object Pool");
		ObjectPool result = gameObject.AddComponent<ObjectPool>();
		GameObject gameObject2 = new GameObject("UI", new Type[]
		{
			typeof(Canvas)
		});
		gameObject2.SetActive(false);
		gameObject2.transform.SetParent(gameObject.transform);
		GameObject gameObject3 = new GameObject("Map");
		gameObject3.SetActive(false);
		gameObject3.transform.SetParent(gameObject.transform);
		return result;
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000389E1 File Offset: 0x00036BE1
	private void OnEnable()
	{
		if (ObjectPool.instance != null && ObjectPool.instance != this)
		{
			Debug.LogError("Object pool created more than once");
		}
		ObjectPool.instance = this;
		ObjectPool.CleanPools();
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00038A12 File Offset: 0x00036C12
	private void OnDisable()
	{
		if (ObjectPool.instance == this)
		{
			ObjectPool.CleanPools();
			ObjectPool.instance = null;
		}
	}

	// Token: 0x040004B6 RID: 1206
	private static Dictionary<GameObject, List<ObjectPool.FreedObject>> freed = new Dictionary<GameObject, List<ObjectPool.FreedObject>>(1000);

	// Token: 0x040004B7 RID: 1207
	private static Dictionary<GameObject, ObjectPool.AllocatedObject> allocated = new Dictionary<GameObject, ObjectPool.AllocatedObject>(1000);

	// Token: 0x040004B8 RID: 1208
	private static ObjectPool instance = null;

	// Token: 0x040004B9 RID: 1209
	public static bool active = true;

	// Token: 0x02000551 RID: 1361
	private struct FreedObject
	{
		// Token: 0x04002FEB RID: 12267
		public GameObject obj;

		// Token: 0x04002FEC RID: 12268
		public IPoolable script;
	}

	// Token: 0x02000552 RID: 1362
	private struct AllocatedObject
	{
		// Token: 0x04002FED RID: 12269
		public GameObject prefab;

		// Token: 0x04002FEE RID: 12270
		public List<ObjectPool.FreedObject> pool;

		// Token: 0x04002FEF RID: 12271
		public IPoolable script;
	}
}
