using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class InstanceHolder : MonoBehaviour
{
	// Token: 0x0600049E RID: 1182 RVA: 0x0003628C File Offset: 0x0003448C
	public static GameObject Resolve(GameObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		InstanceHolder component = obj.GetComponent<InstanceHolder>();
		if (component == null)
		{
			return obj;
		}
		if (component.instance != null)
		{
			return component.instance;
		}
		return obj;
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x000362CC File Offset: 0x000344CC
	public static Transform Resolve(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		InstanceHolder component = t.GetComponent<InstanceHolder>();
		if (component == null)
		{
			return t;
		}
		if (component.instance == null)
		{
			return null;
		}
		return component.instance.transform;
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00036311 File Offset: 0x00034511
	public override string ToString()
	{
		return Common.ToString<InstanceHolder>(this) + " (" + this.prefab.name + ")";
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000023FD File Offset: 0x000005FD
	public void Log(string msg)
	{
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00036334 File Offset: 0x00034534
	public void Despawn()
	{
		Transform transform = base.transform.Find("_prefab_instance");
		if (transform != null)
		{
			this.Log("despawned");
			Object.DestroyImmediate(transform.gameObject);
		}
		this.instance = null;
		this.cur_prefab = null;
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x00036380 File Offset: 0x00034580
	private void RefreshRecursive(Transform t)
	{
		InstanceHolder component = t.GetComponent<InstanceHolder>();
		if (component != null)
		{
			component.Refresh(false);
			return;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			this.RefreshRecursive(child);
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000363C8 File Offset: 0x000345C8
	public void Refresh(bool force_recreate = false)
	{
		if (InstanceHolder.no_refresh > 0)
		{
			return;
		}
		RectTransform component = base.GetComponent<RectTransform>();
		if (component != null)
		{
			force_recreate = true;
		}
		this.instance = null;
		List<Transform> list = null;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (!(transform.name != "_prefab_instance"))
			{
				if (this.instance == null)
				{
					this.instance = transform.gameObject;
				}
				else
				{
					if (list == null)
					{
						list = new List<Transform>();
					}
					list.Add(transform);
				}
			}
		}
		if (list != null)
		{
			foreach (Transform transform2 in list)
			{
				Object.DestroyImmediate(transform2.gameObject);
			}
			list = null;
		}
		if (this.freeze && this.instance != null)
		{
			return;
		}
		if (!force_recreate && this.instance != null && this.cur_prefab == this.prefab)
		{
			return;
		}
		this.Despawn();
		if (!force_recreate && (!base.enabled || !base.gameObject.activeInHierarchy))
		{
			return;
		}
		this.cur_prefab = this.prefab;
		if (this.prefab == null)
		{
			return;
		}
		this.Log("spawning");
		if (component == null)
		{
			this.instance = Object.Instantiate<GameObject>(this.prefab, base.transform.position, base.transform.rotation, base.transform);
			if (this.instance == null)
			{
				return;
			}
			this.instance.name = "_prefab_instance";
		}
		else
		{
			InstanceHolder.no_refresh++;
			this.instance = Object.Instantiate<GameObject>(this.prefab, base.transform);
			if (this.instance == null)
			{
				return;
			}
			this.instance.name = "_prefab_instance";
			this.instance.transform.localPosition = Vector3.zero;
			this.instance.transform.localRotation = Quaternion.identity;
			InstanceHolder.no_refresh--;
			this.RefreshRecursive(this.instance.transform);
			this.instance.SetActive(true);
		}
		this.instance.transform.localScale = Vector3.one;
		this.instance.hideFlags = HideFlags.DontSave;
		RectTransform component2 = this.instance.GetComponent<RectTransform>();
		if (component != null && component2 != null)
		{
			component2.pivot = new Vector2(0.5f, 0.5f);
			component2.anchorMin = Vector2.zero;
			component2.anchorMax = Vector2.one;
			component2.sizeDelta = Vector2.zero;
			component2.offsetMin = Vector2.zero;
			component2.offsetMax = Vector2.zero;
		}
		this.Log("spawned");
		if (Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000366E0 File Offset: 0x000348E0
	private void OnEnable()
	{
		this.Refresh(false);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnDisable()
	{
	}

	// Token: 0x04000485 RID: 1157
	public GameObject prefab;

	// Token: 0x04000486 RID: 1158
	public bool freeze;

	// Token: 0x04000487 RID: 1159
	[HideInInspector]
	public GameObject instance;

	// Token: 0x04000488 RID: 1160
	[HideInInspector]
	public GameObject cur_prefab;

	// Token: 0x04000489 RID: 1161
	private static int no_refresh;
}
