using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x0200006E RID: 110
public static class Common
{
	// Token: 0x0600032C RID: 812 RVA: 0x0002A888 File Offset: 0x00028A88
	public static float clamp(float v, float min, float max)
	{
		if (v < min)
		{
			return min;
		}
		if (v > max)
		{
			return max;
		}
		return v;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0002A897 File Offset: 0x00028A97
	public static int clamp(int v, int min, int max)
	{
		if (v < min)
		{
			return min;
		}
		if (v > max)
		{
			return max;
		}
		return v;
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0002A8A6 File Offset: 0x00028AA6
	public static float map(float v, float vmin, float vmax, float rmin, float rmax, bool clamp = false)
	{
		if (vmax == vmin)
		{
			return rmin;
		}
		if (clamp)
		{
			v = global::Common.clamp(v, vmin, vmax);
		}
		return rmin + (v - vmin) * (rmax - rmin) / (vmax - vmin);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0002A8CA File Offset: 0x00028ACA
	public static int map(int v, int vmin, int vmax, int rmin, int rmax, bool clamp = false)
	{
		if (vmax == vmin)
		{
			return rmin;
		}
		if (clamp)
		{
			v = global::Common.clamp(v, vmin, vmax);
		}
		return rmin + (v - vmin) * (rmax - rmin) / (vmax - vmin);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0002A8EE File Offset: 0x00028AEE
	public static float NormalizeAngle360(float a)
	{
		while (a < 0f)
		{
			a += 360f;
		}
		while (a >= 360f)
		{
			a -= 360f;
		}
		return a;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0002A917 File Offset: 0x00028B17
	public static float NormalizeAngle180(float a)
	{
		while (a <= -180f)
		{
			a += 360f;
		}
		while (a > 180f)
		{
			a -= 360f;
		}
		return a;
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0002A940 File Offset: 0x00028B40
	public static void DestroyObj(UnityEngine.Object obj)
	{
		GameObject obj2;
		if ((obj2 = (obj as GameObject)) != null)
		{
			ObjectPool.DestroyObj(obj2);
			return;
		}
		if (obj == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(obj);
			return;
		}
		UnityEngine.Object.DestroyImmediate(obj);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0002A97C File Offset: 0x00028B7C
	public static void DestroyChildren(Transform t)
	{
		if (t == null)
		{
			return;
		}
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.DestroyImmediate(t.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0002A9B7 File Offset: 0x00028BB7
	public static void DestroyChildren(GameObject obj)
	{
		global::Common.DestroyChildren(obj.transform);
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0002A9C4 File Offset: 0x00028BC4
	public static Transform GetChild(Transform t, int idx)
	{
		if (t == null || idx < 0 || idx >= t.childCount)
		{
			return null;
		}
		return t.GetChild(idx);
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0002A9E8 File Offset: 0x00028BE8
	public static T GetComponent<T>(Transform t, string child_name = null) where T : UnityEngine.Component
	{
		t = InstanceHolder.Resolve(t);
		if (t == null)
		{
			return default(T);
		}
		Transform transform;
		if (child_name == null)
		{
			transform = t;
		}
		else
		{
			transform = t.Find(child_name);
			transform = InstanceHolder.Resolve(transform);
		}
		if (transform == null)
		{
			return default(T);
		}
		return transform.GetComponent<T>();
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0002AA40 File Offset: 0x00028C40
	public static T GetComponent<T>(GameObject go, string child_name = null) where T : UnityEngine.Component
	{
		if (go == null)
		{
			return default(T);
		}
		return global::Common.GetComponent<T>(go.transform, child_name);
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0002AA6C File Offset: 0x00028C6C
	public static T GetComponent<T>(UnityEngine.Component component, string child_name = null) where T : UnityEngine.Component
	{
		if (component == null)
		{
			return default(T);
		}
		return global::Common.GetComponent<T>(component.transform, child_name);
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0002AA98 File Offset: 0x00028C98
	public static void DestroyComponents(Transform t, Type type, bool recursive = true)
	{
		UnityEngine.Component component = t.GetComponent(type);
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		if (!recursive)
		{
			return;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			global::Common.DestroyComponents(t.GetChild(i), type, recursive);
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0002AADF File Offset: 0x00028CDF
	public static void DestroyComponents(GameObject go, Type type, bool recursive = true)
	{
		global::Common.DestroyComponents(go.transform, type, recursive);
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002AAEE File Offset: 0x00028CEE
	public static void DestroyComponents<T>(GameObject go, bool recursive = true) where T : UnityEngine.Component
	{
		global::Common.DestroyComponents(go.transform, typeof(T), recursive);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0002AB08 File Offset: 0x00028D08
	public static GameObject FindChildWithTag(GameObject go, string tag, bool recursive = true)
	{
		if (go == null)
		{
			return null;
		}
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.tag == tag)
			{
				return transform.gameObject;
			}
			if (!recursive)
			{
				Transform transform2 = InstanceHolder.Resolve(transform);
				if (transform2 != null && transform2.tag == tag)
				{
					return transform2.gameObject;
				}
			}
			else
			{
				GameObject gameObject = global::Common.FindChildWithTag(transform.gameObject, tag, true);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0002ABCC File Offset: 0x00028DCC
	public static GameObject FindChildByNameDFS(GameObject go, string name, bool recursive = true, bool resolve_instance_holders = true)
	{
		if (go == null)
		{
			return null;
		}
		GameObject result = null;
		int i = 0;
		while (i < go.transform.childCount)
		{
			Transform child = go.transform.GetChild(i);
			if (child.name.Equals(name))
			{
				if (!resolve_instance_holders)
				{
					return child.gameObject;
				}
				return InstanceHolder.Resolve(child.gameObject);
			}
			else
			{
				if (!recursive)
				{
					if (resolve_instance_holders)
					{
						Transform transform = InstanceHolder.Resolve(child);
						if (transform != null && transform.name.Equals(name))
						{
							return transform.gameObject;
						}
					}
				}
				else
				{
					GameObject gameObject = global::Common.FindChildByNameDFS(child.gameObject, name, true, resolve_instance_holders);
					if (gameObject != null)
					{
						return gameObject;
					}
				}
				i++;
			}
		}
		return result;
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0002AC7C File Offset: 0x00028E7C
	public static GameObject FindChildByNameBFS(GameObject go, string name, bool recursive = true, bool resolve_instance_holders = true)
	{
		if (go == null)
		{
			return null;
		}
		global::Common.bfsQueue.Clear();
		global::Common.bfsQueue.Enqueue(go.transform);
		while (global::Common.bfsQueue.Count > 0)
		{
			Transform transform = global::Common.bfsQueue.Dequeue();
			if (transform.name.Equals(name, StringComparison.Ordinal))
			{
				if (!resolve_instance_holders)
				{
					return transform.gameObject;
				}
				return InstanceHolder.Resolve(transform.gameObject);
			}
			else
			{
				if (resolve_instance_holders)
				{
					Transform transform2 = InstanceHolder.Resolve(transform);
					if (transform2 != null && transform2.name.Equals(name, StringComparison.Ordinal))
					{
						return transform2.gameObject;
					}
				}
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (!recursive)
					{
						if (child.name.Equals(name, StringComparison.Ordinal))
						{
							if (!resolve_instance_holders)
							{
								return child.gameObject;
							}
							return InstanceHolder.Resolve(child.gameObject);
						}
						else if (resolve_instance_holders)
						{
							Transform transform3 = InstanceHolder.Resolve(child);
							if (transform3 != null && transform3.name.Equals(name, StringComparison.Ordinal))
							{
								return transform3.gameObject;
							}
						}
					}
					else
					{
						global::Common.bfsQueue.Enqueue(child);
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0002AD9E File Offset: 0x00028F9E
	public static GameObject FindChildByName(GameObject go, string name, bool recursive = true, bool resolve_instance_holders = true)
	{
		if (go == null)
		{
			return null;
		}
		return global::Common.FindChildByNameBFS(go, name, recursive, resolve_instance_holders);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0002ADB4 File Offset: 0x00028FB4
	public static void FindChildrenByName(List<GameObject> result, GameObject go, string name, bool recursive = true, bool resolve_instance_holders = true)
	{
		if (go == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		Transform transform = go.transform;
		int i = 0;
		int childCount = transform.childCount;
		while (i < childCount)
		{
			Transform child = transform.GetChild(i);
			if (child.name == name)
			{
				result.Add(resolve_instance_holders ? InstanceHolder.Resolve(child.gameObject) : child.gameObject);
			}
			if (!recursive)
			{
				if (resolve_instance_holders)
				{
					Transform transform2 = InstanceHolder.Resolve(child);
					if (transform2 != null && transform2.name == name)
					{
						result.Add(transform2.gameObject);
					}
				}
			}
			else
			{
				global::Common.FindChildrenByName(result, child.gameObject, name, true, resolve_instance_holders);
			}
			i++;
		}
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0002AE6C File Offset: 0x0002906C
	public static void FindChildrenWithComponent<T>(GameObject go, List<T> lst)
	{
		lst.Clear();
		if (go == null)
		{
			return;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			T component = go.transform.GetChild(i).GetComponent<T>();
			if (component != null)
			{
				lst.Add(component);
			}
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0002AEC0 File Offset: 0x000290C0
	public static void FindChildrenWithComponentRecursive<T>(GameObject go, List<T> lst)
	{
		global::Common.<>c__DisplayClass23_0<T> CS$<>8__locals1;
		CS$<>8__locals1.go = go;
		CS$<>8__locals1.lst = lst;
		CS$<>8__locals1.lst.Clear();
		global::Common.<FindChildrenWithComponentRecursive>g__Scan|23_0<T>(CS$<>8__locals1.go, ref CS$<>8__locals1);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0002AEF8 File Offset: 0x000290F8
	public static void IterateThroughChildrenRecursive<T>(Transform transform, Action<T> action) where T : UnityEngine.Component
	{
		global::Common.<>c__DisplayClass24_0<T> CS$<>8__locals1;
		CS$<>8__locals1.action = action;
		global::Common.<IterateThroughChildrenRecursive>g__Iterate|24_0<T>(transform, ref CS$<>8__locals1);
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0002AF18 File Offset: 0x00029118
	public static T FindChildComponent<T>(GameObject go, string child_name = null) where T : UnityEngine.Component
	{
		GameObject gameObject = (child_name == null) ? go : global::Common.FindChildByName(go, child_name, true, false);
		if (gameObject == null)
		{
			return default(T);
		}
		T component = gameObject.GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		InstanceHolder component2 = gameObject.GetComponent<InstanceHolder>();
		if (component2 == null || component2.instance == null)
		{
			return default(T);
		}
		return component2.instance.GetComponent<T>();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0002AF94 File Offset: 0x00029194
	public static T GetParentComponent<T>(GameObject go) where T : UnityEngine.Component
	{
		Transform transform = go.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0002AFDC File Offset: 0x000291DC
	public static bool IsParent(GameObject parent, GameObject child)
	{
		if (parent == null || child == null)
		{
			return false;
		}
		Transform transform = parent.transform;
		Transform transform2 = child.transform;
		while (transform2 != null)
		{
			if (transform2 == transform)
			{
				return true;
			}
			transform2 = transform2.parent;
		}
		return false;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0002B02C File Offset: 0x0002922C
	public static string ObjPath(GameObject obj)
	{
		if (obj == null)
		{
			return "";
		}
		string text = obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = obj.name + "/" + text;
		}
		return text;
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0002B089 File Offset: 0x00029289
	public static string AppMode()
	{
		if (Application.isPlaying)
		{
			return "game";
		}
		return "editor";
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0002B09D File Offset: 0x0002929D
	public static string ToString<T>(T script) where T : UnityEngine.Component
	{
		return typeof(T).ToString() + " " + ((script == null) ? "<null>" : global::Common.ObjPath(script.gameObject));
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0002B0E0 File Offset: 0x000292E0
	public static void Log(string msg, string mode = null)
	{
		string text = global::Common.AppMode();
		if (mode != null && mode != text)
		{
			return;
		}
		Debug.Log("[" + text + "] " + msg);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0002B116 File Offset: 0x00029316
	public static void Log(GameObject obj, string msg, string mode = null)
	{
		global::Common.Log(global::Common.ObjPath(obj) + ": " + msg, mode);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0002B12F File Offset: 0x0002932F
	public static void Log<T>(T script, string msg, string mode = null) where T : UnityEngine.Component
	{
		global::Common.Log(script.ToString() + ": " + msg, mode);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0002B150 File Offset: 0x00029350
	public static GameObject Spawn(GameObject prefab, bool keep_prefab = false, bool force_pooled = false)
	{
		if (prefab == null)
		{
			return null;
		}
		GameObject result;
		using (Game.Profile("Common.Spawn(prefab)", false, 0f, null))
		{
			result = ObjectPool.Spawn(prefab, null, force_pooled);
		}
		return result;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0002B1A4 File Offset: 0x000293A4
	public static GameObject Spawn(GameObject prefab, Transform parent, bool keep_prefab = false, string parentInnerCategory = "")
	{
		if (prefab == null)
		{
			return null;
		}
		GameObject result;
		using (Game.Profile("Common.Spawn(prefab, parent)", false, 0f, null))
		{
			bool flag = true;
			GameObject gameObject;
			if (string.IsNullOrEmpty(parentInnerCategory))
			{
				gameObject = ObjectPool.Spawn(prefab, parent, false);
				flag = false;
			}
			else
			{
				gameObject = ObjectPool.Spawn(prefab, null, false);
			}
			if (flag)
			{
				global::Common.SetObjectParent(gameObject, parent, parentInnerCategory);
			}
			result = gameObject;
		}
		return result;
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0002B21C File Offset: 0x0002941C
	public static GameObject SpawnPooled(GameObject prefab, Transform parent, bool keep_prefab = false, string parentInnerCategory = "")
	{
		if (prefab == null)
		{
			return null;
		}
		GameObject result;
		using (Game.Profile("Common.Spawn(prefab, parent)", false, 0f, null))
		{
			bool flag = true;
			GameObject gameObject;
			if (string.IsNullOrEmpty(parentInnerCategory))
			{
				gameObject = ObjectPool.Spawn(prefab, parent, true);
				flag = false;
			}
			else
			{
				gameObject = ObjectPool.Spawn(prefab, null, true);
			}
			if (flag)
			{
				global::Common.SetObjectParent(gameObject, parent, parentInnerCategory);
			}
			result = gameObject;
		}
		return result;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0002B294 File Offset: 0x00029494
	public static GameObject FindTemplate(string template_name)
	{
		GameObject result;
		global::Common.template_objects.TryGetValue(template_name, out result);
		return result;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002B2B0 File Offset: 0x000294B0
	public static GameObject CreateTemplate(string template_name, params Type[] component_types)
	{
		GameObject gameObject = new GameObject(template_name);
		gameObject.SetActive(false);
		foreach (Type componentType in component_types)
		{
			gameObject.AddComponent(componentType);
		}
		return gameObject;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0002B2E8 File Offset: 0x000294E8
	public static void AddTemplate(string template_name, GameObject template)
	{
		global::Common.template_objects.Add(template_name, template);
		if (global::Common.templates_root == null)
		{
			global::Common.templates_root = new GameObject("Templates");
			global::Common.templates_root.SetActive(false);
			UnityEngine.Object.DontDestroyOnLoad(global::Common.templates_root);
		}
		if (template.transform is RectTransform)
		{
			if (global::Common.ui_templates_root == null)
			{
				global::Common.ui_templates_root = new GameObject("UI", new Type[]
				{
					typeof(Canvas)
				});
				global::Common.ui_templates_root.transform.SetParent(global::Common.templates_root.transform, false);
			}
			template.transform.SetParent(global::Common.ui_templates_root.transform);
			return;
		}
		if (global::Common.map_templates_root == null)
		{
			global::Common.map_templates_root = new GameObject("Map");
			global::Common.map_templates_root.transform.SetParent(global::Common.templates_root.transform, false);
		}
		template.transform.SetParent(global::Common.map_templates_root.transform);
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0002B3EC File Offset: 0x000295EC
	public static GameObject SpawnTemplate(string template_name, string obj_name, Transform parent, bool activate, params Type[] component_types)
	{
		Profile.BeginSection("Common.SpawnTemplate");
		GameObject gameObject = global::Common.FindTemplate(template_name);
		if (gameObject == null)
		{
			gameObject = global::Common.CreateTemplate(template_name, component_types);
			global::Common.AddTemplate(template_name, gameObject);
		}
		GameObject gameObject2;
		if (parent != null)
		{
			gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, parent, false);
		}
		else
		{
			gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		}
		if (obj_name != null)
		{
			gameObject2.name = obj_name;
		}
		if (activate)
		{
			gameObject2.SetActive(true);
		}
		Profile.EndSection("Common.SpawnTemplate");
		return gameObject2;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0002B45D File Offset: 0x0002965D
	public static T SpawnTemplate<T>(string template_name, string obj_name, Transform parent, bool activate, params Type[] component_types) where T : UnityEngine.Component
	{
		return global::Common.SpawnTemplate(template_name, obj_name, parent, activate, component_types).GetComponent<T>();
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002B470 File Offset: 0x00029670
	public static void SetObjectParent(GameObject obj, Transform parent, string parentInnerCategory = "")
	{
		if (parent == null)
		{
			return;
		}
		Transform parent2;
		if (parentInnerCategory != "")
		{
			GameObject gameObject = global::Common.FindChildByName(parent.gameObject, parentInnerCategory, true, true);
			if (gameObject == null)
			{
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.hierarchyCapacity = 5000;
				gameObject2.name = parentInnerCategory;
				gameObject2.transform.SetParent(parent, false);
				parent2 = gameObject2.transform;
			}
			else
			{
				parent2 = gameObject.transform;
			}
		}
		else
		{
			parent2 = parent;
		}
		obj.transform.SetParent(parent2, false);
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002B4F7 File Offset: 0x000296F7
	public static float GetHeight(Vector3 pos, Terrain terrain = null, float water_level = -1f, bool low_res = false)
	{
		float terrainHeight = global::Common.GetTerrainHeight(pos, terrain, low_res);
		if (water_level < 0f)
		{
			water_level = MapData.GetWaterLevel();
		}
		return Mathf.Max(terrainHeight, water_level);
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002B516 File Offset: 0x00029716
	public static Vector3 Point3D(PPos pos, Game game)
	{
		return pos.Point3D(game, global::Common.GetHeight(pos, null, -1f, false), 0f);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0002B53C File Offset: 0x0002973C
	public static bool IsInWater(Vector3 pos, Terrain terrain = null, float water_level = -1f)
	{
		float terrainHeight = global::Common.GetTerrainHeight(pos, terrain, false);
		if (water_level < 0f)
		{
			water_level = MapData.GetWaterLevel();
		}
		return terrainHeight < water_level;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0002B558 File Offset: 0x00029758
	public static Vector3 GetRightVector(Vector3 vForward, float len = 0f)
	{
		Vector3 vector = new Vector3(vForward.z, vForward.y, -vForward.x);
		if (len != 0f)
		{
			float magnitude = vForward.magnitude;
			if (magnitude > Mathf.Epsilon)
			{
				vector *= len / magnitude;
			}
		}
		return vector;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0002B5A4 File Offset: 0x000297A4
	public static Rect CalcWorldExtents()
	{
		Rect result = new Rect(0f, 0f, 0f, 0f);
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return result;
		}
		float x = activeTerrain.transform.position.x;
		float z = activeTerrain.transform.position.z;
		float num = x + activeTerrain.terrainData.bounds.max.x;
		float num2 = z + activeTerrain.terrainData.bounds.max.z;
		if (x < result.xMin)
		{
			result.xMin = x;
		}
		if (z < result.yMin)
		{
			result.yMin = z;
		}
		if (num > result.xMax)
		{
			result.xMax = num;
		}
		if (num2 > result.yMax)
		{
			result.yMax = num2;
		}
		return result;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0002B688 File Offset: 0x00029888
	public static Terrain GetBiggestTerrain()
	{
		float num = 0f;
		Terrain result = null;
		Terrain[] activeTerrains = Terrain.activeTerrains;
		for (int i = 0; i < activeTerrains.Length; i++)
		{
			float x = activeTerrains[i].terrainData.size.x;
			if (x > num)
			{
				result = activeTerrains[i];
				num = x;
			}
		}
		return result;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0002B6D4 File Offset: 0x000298D4
	public static Terrain GetTerrainAt(Vector3 pos, out Vector3 ptLocal, Terrain t = null)
	{
		if (t != null)
		{
			TerrainData terrainData = t.terrainData;
			if (terrainData != null)
			{
				Vector3 vector = t.transform.InverseTransformPoint(pos);
				Vector3 max = terrainData.bounds.max;
				if (vector.x >= 0f && vector.x < max.x && vector.z >= 0f && vector.z < max.z)
				{
					ptLocal = vector;
					return t;
				}
			}
		}
		Terrain[] activeTerrains = Terrain.activeTerrains;
		ptLocal = Vector3.zero;
		foreach (Terrain t in activeTerrains)
		{
			if (!(t == null) && !(t.tag != "Main"))
			{
				TerrainData terrainData2 = t.terrainData;
				if (!(terrainData2 == null))
				{
					Vector3 vector2 = t.transform.InverseTransformPoint(pos);
					Vector3 max2 = terrainData2.bounds.max;
					if (vector2.x >= 0f && vector2.x <= max2.x && vector2.z >= 0f && vector2.z <= max2.z)
					{
						ptLocal = vector2;
						return t;
					}
				}
			}
		}
		foreach (Terrain t in activeTerrains)
		{
			if (!(t == null))
			{
				Vector3 vector3 = t.transform.InverseTransformPoint(pos);
				TerrainData terrainData3 = t.terrainData;
				if (!(terrainData3 == null))
				{
					Vector3 max3 = terrainData3.bounds.max;
					if (vector3.x >= 0f && vector3.x <= max3.x && vector3.z >= 0f && vector3.z <= max3.z)
					{
						ptLocal = vector3;
						return t;
					}
				}
			}
		}
		ptLocal = Vector3.zero;
		return null;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0002B8C8 File Offset: 0x00029AC8
	public static float GetTerrainHeight(Vector3 pos, Terrain terrain = null, bool low_res = false)
	{
		Vector3 vector;
		Terrain terrainAt = global::Common.GetTerrainAt(pos, out vector, terrain);
		if (terrainAt == null)
		{
			return pos.y;
		}
		if (low_res)
		{
			float num = TerrainHeightsRenderer.Get().GetLowResHeight(pos) / 255f;
			if (num >= 0f)
			{
				return num * terrainAt.terrainData.size.y;
			}
		}
		TerrainData terrainData = terrainAt.terrainData;
		float x = vector.x / terrainData.bounds.max.x;
		float y = vector.z / terrainData.bounds.max.z;
		return terrainData.GetInterpolatedHeight(x, y);
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0002B970 File Offset: 0x00029B70
	public static Vector3 SnapToTerrain(Vector3 pos, float ofs = 0f, Terrain terrain = null, float water_level = -1f, bool low_res = false)
	{
		pos.y = global::Common.GetHeight(pos, terrain, water_level, low_res) + ofs;
		return pos;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002B986 File Offset: 0x00029B86
	public static void SnapToTerrain(Transform t, float ofs = 0f, Terrain terrain = null, float water_level = -1f)
	{
		t.position = global::Common.SnapToTerrain(t.position, ofs, terrain, water_level, false);
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002B99D File Offset: 0x00029B9D
	public static void SnapToTerrain(GameObject obj, float ofs = 0f, Terrain terrain = null, float water_level = -1f)
	{
		obj.transform.position = global::Common.SnapToTerrain(obj.transform.position, ofs, terrain, water_level, false);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0002B9C0 File Offset: 0x00029BC0
	public static Vector3 GetTerrainNormal(Vector3 pos, Terrain terrain = null)
	{
		Vector3 vector;
		Terrain terrainAt = global::Common.GetTerrainAt(pos, out vector, terrain);
		if (terrainAt == null)
		{
			return Vector3.zero;
		}
		TerrainData terrainData = terrainAt.terrainData;
		float x = vector.x / terrainData.bounds.max.x;
		float y = vector.z / terrainData.bounds.max.z;
		return terrainData.GetInterpolatedNormal(x, y);
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002BA30 File Offset: 0x00029C30
	public static float GetTerrainSlope(Vector3 pos, Terrain terrain = null)
	{
		Vector3 vector;
		Terrain terrainAt = global::Common.GetTerrainAt(pos, out vector, terrain);
		if (terrainAt == null)
		{
			return float.PositiveInfinity;
		}
		TerrainData terrainData = terrainAt.terrainData;
		float x = vector.x / terrainData.bounds.max.x;
		float y = vector.z / terrainData.bounds.max.z;
		return terrainData.GetSteepness(x, y);
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002BAA0 File Offset: 0x00029CA0
	public static Point GetNearbyLand(Point pt, float max_dist = 16f, float step = 2f)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		TerrainData terrainData = (activeTerrain != null) ? activeTerrain.terrainData : null;
		if (terrainData == null)
		{
			return pt;
		}
		float waterLevel = MapData.GetWaterLevel();
		if (waterLevel <= 0f)
		{
			return pt;
		}
		float x = pt.x / terrainData.bounds.max.x;
		float y = pt.y / terrainData.bounds.max.z;
		if (terrainData.GetInterpolatedHeight(x, y) >= waterLevel)
		{
			return pt;
		}
		for (float num = step; num <= max_dist; num += step)
		{
			for (int i = 0; i < 12; i++)
			{
				float num2 = (float)i / 12f * 360f;
				float num3 = pt.x + num * Mathf.Cos(3.1415927f * num2 / 180f);
				float num4 = pt.y + num * Mathf.Sin(3.1415927f * num2 / 180f);
				x = num3 / terrainData.bounds.max.x;
				y = num4 / terrainData.bounds.max.z;
				if (terrainData.GetInterpolatedHeight(x, y) >= waterLevel)
				{
					return new Point(num3, num4);
				}
			}
		}
		return Point.Invalid;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0002BBE8 File Offset: 0x00029DE8
	public static Point GetNearbyWater(Point pt, float max_dist = 16f, float step = 2f)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		TerrainData terrainData = (activeTerrain != null) ? activeTerrain.terrainData : null;
		if (terrainData == null)
		{
			return Point.Invalid;
		}
		float waterLevel = MapData.GetWaterLevel();
		if (waterLevel <= 0f)
		{
			return Point.Invalid;
		}
		float x = pt.x / terrainData.bounds.max.x;
		float y = pt.y / terrainData.bounds.max.z;
		if (terrainData.GetInterpolatedHeight(x, y) < waterLevel)
		{
			return pt;
		}
		for (float num = step; num <= max_dist; num += step)
		{
			for (int i = 0; i < 12; i++)
			{
				float num2 = (float)i / 12f * 360f;
				float num3 = pt.x + num * Mathf.Cos(3.1415927f * num2 / 180f);
				float num4 = pt.y + num * Mathf.Sin(3.1415927f * num2 / 180f);
				x = num3 / terrainData.bounds.max.x;
				y = num4 / terrainData.bounds.max.z;
				if (terrainData.GetInterpolatedHeight(x, y) < waterLevel)
				{
					return new Point(num3, num4);
				}
			}
		}
		return Point.Invalid;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0002BD38 File Offset: 0x00029F38
	public static Point GetNearbyForest(Point pt, float max_dist = 16f, float step = 2f, TerrainTypesBuilder ttb = null)
	{
		if (ttb == null)
		{
			ttb = TerrainTypesBuilder.AnalyzeOnlyTrees();
		}
		if (ttb.HasType(pt, TerrainType.Forest))
		{
			return pt;
		}
		for (float num = step; num <= max_dist; num += step)
		{
			for (int i = 0; i < 12; i++)
			{
				float num2 = (float)i / 12f * 360f;
				float x = pt.x + num * Mathf.Cos(3.1415927f * num2 / 180f);
				float y = pt.y + num * Mathf.Sin(3.1415927f * num2 / 180f);
				Point point = new Point(x, y);
				if (ttb.HasType(point, TerrainType.Forest))
				{
					return point;
				}
			}
		}
		return Point.Invalid;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002BDDC File Offset: 0x00029FDC
	public static void WorldToTerrainTexUV(Vector3 ptw, Vector3 vTerrainSize, out float u, out float v)
	{
		float x = vTerrainSize.x;
		float z = vTerrainSize.z;
		u = ptw.x / x;
		v = (ptw.z + (x - z) / 2f) / x;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002BE18 File Offset: 0x0002A018
	public static Ray PickingRay()
	{
		Camera mainCamera = CameraController.MainCamera;
		if (mainCamera == null)
		{
			return new Ray(Vector3.zero, Vector3.zero);
		}
		return mainCamera.ScreenPointToRay(Input.mousePosition);
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002BE50 File Offset: 0x0002A050
	public static Vector3 GetPickedTerrainPoint()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain == null)
		{
			return Vector3.zero;
		}
		TerrainCollider component = activeTerrain.GetComponent<TerrainCollider>();
		if (component == null)
		{
			return Vector3.zero;
		}
		Ray ray = global::Common.PickingRay();
		RaycastHit raycastHit;
		if (!component.Raycast(ray, out raycastHit, 3.4028235E+38f))
		{
			return Vector3.zero;
		}
		return raycastHit.point;
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0002BEAC File Offset: 0x0002A0AC
	public static void DrawTerrainLine(Vector3 from, Vector3 to, float ofs = 0.1f, float segment_len = 2f)
	{
		Vector3 vector = global::Common.SnapToTerrain(from, ofs, null, -1f, false);
		if (segment_len > 0f)
		{
			Vector3 vector2 = to - from;
			int num = (int)(vector2.magnitude / segment_len);
			if (num > 0)
			{
				vector2 /= (float)num;
				for (int i = 0; i < num; i++)
				{
					Vector3 vector3 = global::Common.SnapToTerrain(vector + vector2, ofs, null, -1f, false);
					Gizmos.DrawLine(vector, vector3);
					vector = vector3;
				}
			}
		}
		Gizmos.DrawLine(vector, global::Common.SnapToTerrain(to, ofs, null, -1f, false));
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002BF34 File Offset: 0x0002A134
	public static void DrawHex(Vector3 center, float radius, float rot = 0f, float ofs = 0.1f, float segment_len = 0f)
	{
		Point[] array = new Point[]
		{
			new Point(-1f, 0f),
			new Point(-0.5f, -0.866f),
			new Point(0.5f, -0.866f),
			new Point(1f, 0f),
			new Point(0.5f, 0.866f),
			new Point(-0.5f, 0.866f)
		};
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 a = array[i].GetRotated(rot);
			Vector3 a2 = array[(i + 1) % array.Length].GetRotated(rot);
			global::Common.DrawTerrainLine(center + a * radius, center + a2 * radius, ofs, segment_len);
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002C028 File Offset: 0x0002A228
	public static void DrawRect(Vector3 center, float width, float height, float rot = 0f, float ofs = 0.1f, float segment_len = 0f)
	{
		Point[] array = new Point[]
		{
			new Point(-width, -height),
			new Point(width, -height),
			new Point(width, height),
			new Point(-width, height)
		};
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 b = array[i].GetRotated(rot);
			Vector3 b2 = array[(i + 1) % array.Length].GetRotated(rot);
			global::Common.DrawTerrainLine(center + b, center + b2, ofs, segment_len);
		}
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0002C0CC File Offset: 0x0002A2CC
	public static void DrawTerrainGrid(Vector3 center, Vector2Int size, Vector2 tile_size, float ofs = 0.1f, float segment_len = 0f)
	{
		Vector3 a = center;
		a.x -= (float)size.x * tile_size.x * 0.5f;
		a.z -= (float)size.y * tile_size.y * 0.5f;
		for (int i = 0; i <= size.y; i++)
		{
			for (int j = 0; j <= size.x; j++)
			{
				Vector3 vector = a + new Vector3((float)j * tile_size.x, 0f, (float)i * tile_size.y);
				if (j < size.x)
				{
					Vector3 to = vector;
					to.x += tile_size.x;
					global::Common.DrawTerrainLine(vector, to, ofs, segment_len);
				}
				if (i < size.y)
				{
					Vector3 to2 = vector;
					to2.z += tile_size.y;
					global::Common.DrawTerrainLine(vector, to2, ofs, segment_len);
				}
			}
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0002C1C0 File Offset: 0x0002A3C0
	public static Camera CreateTerrainCam(Terrain terrain = null)
	{
		if (terrain == null)
		{
			terrain = Terrain.activeTerrain;
			if (terrain == null)
			{
				return null;
			}
		}
		GameObject gameObject = GameObject.Find("TerrainCam");
		if (gameObject != null)
		{
			global::Common.DestroyObj(gameObject);
		}
		gameObject = new GameObject("TerrainCam");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		Camera camera = gameObject.GetComponent<Camera>();
		if (camera == null)
		{
			camera = gameObject.AddComponent<Camera>();
		}
		camera.enabled = false;
		Vector3 size = terrain.terrainData.size;
		camera.orthographic = true;
		camera.aspect = size.x / size.z;
		camera.orthographicSize = Mathf.Min(size.x, size.z) / 2f;
		camera.transform.position = new Vector3(size.x / 2f, size.y + 1f, size.z / 2f);
		camera.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
		camera.nearClipPlane = 0f;
		camera.farClipPlane = size.y + 10f;
		camera.useOcclusionCulling = false;
		camera.allowMSAA = false;
		camera.allowHDR = false;
		camera.clearFlags = CameraClearFlags.Color;
		camera.backgroundColor = Color.black;
		camera.cullingMask = 1 << terrain.gameObject.layer;
		return camera;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0002C32C File Offset: 0x0002A52C
	public static Camera CreatePerspectiveTerrainCam(Terrain terrain = null)
	{
		if (terrain == null)
		{
			terrain = Terrain.activeTerrain;
			if (terrain == null)
			{
				return null;
			}
		}
		GameObject gameObject = GameObject.Find("TerrainCam");
		if (gameObject != null)
		{
			global::Common.DestroyObj(gameObject);
		}
		gameObject = new GameObject("TerrainCam");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		Camera camera = gameObject.GetComponent<Camera>();
		if (camera == null)
		{
			camera = gameObject.AddComponent<Camera>();
		}
		Bounds bounds = terrain.terrainData.bounds;
		camera.transform.position = bounds.center + Vector3.up * bounds.size.x;
		camera.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		camera.clearFlags = CameraClearFlags.Color;
		camera.nearClipPlane = bounds.size.x * 0.5f;
		camera.farClipPlane = bounds.size.x * 2f;
		Matrix4x4 projectionMatrix = Matrix4x4.Perspective(54f, 1f, 1f, 10000f);
		camera.projectionMatrix = projectionMatrix;
		camera.useOcclusionCulling = false;
		camera.backgroundColor = Color.black;
		return camera;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0002C45C File Offset: 0x0002A65C
	public static bool SaveTexture(Texture2D tex, string path, bool reload = true)
	{
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		string text = Path.GetExtension(path).ToLowerInvariant();
		byte[] array = null;
		try
		{
			if (!(text == ".png"))
			{
				if (!(text == ".tga"))
				{
					if (!(text == ".jpg"))
					{
						Debug.LogError("Unknown extension: " + text);
						return false;
					}
					array = tex.EncodeToJPG();
				}
				else
				{
					array = tex.EncodeToTGA();
				}
			}
			else
			{
				array = tex.EncodeToPNG();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
			return false;
		}
		if (array == null || array.Length == 0)
		{
			return false;
		}
		try
		{
			File.WriteAllBytes(path, array);
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.ToString());
			return false;
		}
		return true;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0002C538 File Offset: 0x0002A738
	public static bool IsPrefab(GameObject obj)
	{
		return false;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0002C538 File Offset: 0x0002A738
	public static bool IsPrefabRoot(GameObject obj)
	{
		return false;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0002C53B File Offset: 0x0002A73B
	public static bool EditorProgress(string task, string step = null, float progress = 0f, bool cancelable = false)
	{
		return true;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002C53E File Offset: 0x0002A73E
	public static T GetItem<T>(List<T> lst, int idx, T def_val = default(T))
	{
		if (lst == null || idx < 0 || idx >= lst.Count)
		{
			return def_val;
		}
		return lst[idx];
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0002C559 File Offset: 0x0002A759
	public static T GetItem<T>(T[] arr, int idx, T def_val = default(T))
	{
		if (arr == null || idx < 0 || idx >= arr.Length)
		{
			return def_val;
		}
		return arr[idx];
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0002C574 File Offset: 0x0002A774
	public static void Desaturate(ref Color color, float desaturationMagnitute)
	{
		float num = color.r * 0.299f + color.g * 0.587f + color.b * 0.144f;
		color.r = num * desaturationMagnitute + color.r * (1f - desaturationMagnitute);
		color.g = num * desaturationMagnitute + color.g * (1f - desaturationMagnitute);
		color.b = num * desaturationMagnitute + color.b * (1f - desaturationMagnitute);
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0002C5F0 File Offset: 0x0002A7F0
	public static string Intl2ASCII(string text)
	{
		string text2 = text.Normalize(NormalizationForm.FormD);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in text2)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}
		string text4 = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		string text5 = "łløoŁLıi";
		int num = 0;
		while (num + 1 < text5.Length)
		{
			char oldChar = text5[num];
			char newChar = text5[num + 1];
			text4 = text4.Replace(oldChar, newChar);
			num += 2;
		}
		foreach (char c2 in text4)
		{
			int num2 = (int)c2;
			if (num2 >= 128)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Uncnown character ",
					num2,
					":'",
					c2.ToString(),
					"' in string '",
					text,
					"'"
				}));
			}
		}
		return text4;
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0002C700 File Offset: 0x0002A900
	public static void SetRendererLayer(GameObject go, int layer)
	{
		if (go == null)
		{
			return;
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0002C73C File Offset: 0x0002A93C
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
	{
		T t = gameObject.GetComponent<T>();
		if (t == null)
		{
			t = gameObject.AddComponent<T>();
		}
		return t;
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0002C768 File Offset: 0x0002A968
	public static T GetOrAddComponent<T>(this UnityEngine.Component comp) where T : UnityEngine.Component
	{
		T t = comp.gameObject.GetComponent<T>();
		if (t == null)
		{
			t = comp.gameObject.AddComponent<T>();
		}
		return t;
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0002C79C File Offset: 0x0002A99C
	public static bool Intersect(Point line1V1, Point line1V2, Point line2V1, Point line2V2, out Point resPoint)
	{
		float num = line1V2.y - line1V1.y;
		float num2 = line1V1.x - line1V2.x;
		float num3 = num * line1V1.x + num2 * line1V1.y;
		float num4 = line2V2.y - line2V1.y;
		float num5 = line2V1.x - line2V2.x;
		float num6 = num4 * line2V1.x + num5 * line2V1.y;
		float num7 = num * num5 - num4 * num2;
		if (Math.Abs(num7) <= 0.001f)
		{
			resPoint = line1V1;
			return false;
		}
		float x = (num5 * num3 - num2 * num6) / num7;
		float y = (num * num6 - num4 * num3) / num7;
		resPoint = new Point(x, y);
		return true;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0002C858 File Offset: 0x0002AA58
	public static bool CheckIfPointIsInRectangle(Point point, Point recPos1, Point recPos2, double error = 0.0)
	{
		float x = point.x;
		float y = point.y;
		return (((double)recPos1.x - error <= (double)x && (double)x <= (double)recPos2.x + error) || ((double)recPos1.x + error >= (double)x && (double)x >= (double)recPos2.x - error)) && (((double)recPos1.y - error <= (double)y && (double)y <= (double)recPos2.y + error) || ((double)recPos1.y + error >= (double)y && (double)y >= (double)recPos2.y - error));
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0002C8E8 File Offset: 0x0002AAE8
	public static bool IsOutsidePolygon(List<Point> vertices, Point pt, Point center)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			Point point = vertices[i];
			Point point2 = vertices[(i + 1) % vertices.Count];
			Point point3;
			if (global::Common.Intersect(point, point2, pt, center, out point3) && global::Common.CheckIfPointIsInRectangle(point3, point, point2, 0.0) && global::Common.CheckIfPointIsInRectangle(point3, pt, center, 0.0))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0002C958 File Offset: 0x0002AB58
	public static Point RandomOnUnitCircle()
	{
		Point result = new Point(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		result.Normalize();
		return result;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0002C9BC File Offset: 0x0002ABBC
	[CompilerGenerated]
	internal static void <FindChildrenWithComponentRecursive>g__Scan|23_0<T>(GameObject game_object, ref global::Common.<>c__DisplayClass23_0<T> A_1)
	{
		if (A_1.go == null)
		{
			return;
		}
		for (int i = 0; i < A_1.go.transform.childCount; i++)
		{
			Transform child = A_1.go.transform.GetChild(i);
			T component = child.GetComponent<T>();
			if (component != null)
			{
				A_1.lst.Add(component);
			}
			global::Common.<FindChildrenWithComponentRecursive>g__Scan|23_0<T>(child.gameObject, ref A_1);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0002CA2C File Offset: 0x0002AC2C
	[CompilerGenerated]
	internal static void <IterateThroughChildrenRecursive>g__Iterate|24_0<T>(Transform t, ref global::Common.<>c__DisplayClass24_0<T> A_1) where T : UnityEngine.Component
	{
		int childCount = t.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			T obj;
			if (child.TryGetComponent<T>(out obj))
			{
				A_1.action(obj);
			}
			global::Common.<IterateThroughChildrenRecursive>g__Iterate|24_0<T>(child, ref A_1);
		}
	}

	// Token: 0x04000409 RID: 1033
	private static Queue<Transform> bfsQueue = new Queue<Transform>();

	// Token: 0x0400040A RID: 1034
	private static Dictionary<string, GameObject> template_objects = new Dictionary<string, GameObject>();

	// Token: 0x0400040B RID: 1035
	private static GameObject templates_root = null;

	// Token: 0x0400040C RID: 1036
	private static GameObject map_templates_root = null;

	// Token: 0x0400040D RID: 1037
	private static GameObject ui_templates_root = null;
}
