using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class BSGCloneHelper
{
	// Token: 0x0600059F RID: 1439 RVA: 0x0003DF20 File Offset: 0x0003C120
	public static BSGCloneHelper.ObjectCloneAction GetObjectCloneAction(GameObject go)
	{
		if (go == null)
		{
			return BSGCloneHelper.ObjectCloneAction.Ignore;
		}
		Component[] components = go.GetComponents<Component>();
		bool flag = true;
		foreach (Component component in components)
		{
			if (!(component == null))
			{
				Type type = component.GetType();
				if (Array.IndexOf<Type>(BSGCloneHelper.DontCloneObjectsWithComponents, type) >= 0)
				{
					return BSGCloneHelper.ObjectCloneAction.Ignore;
				}
				if (Array.IndexOf<Type>(BSGCloneHelper.CloneWholeObjectsWithComponents, type) >= 0)
				{
					return BSGCloneHelper.ObjectCloneAction.Clone;
				}
				if (Array.IndexOf<Type>(BSGCloneHelper.CloneChildrenForObjectsWithComponents, type) >= 0)
				{
					return BSGCloneHelper.ObjectCloneAction.GoDeeper;
				}
				if (!(component is Transform))
				{
					flag = false;
				}
			}
		}
		if (go.transform.position == Vector3.zero || flag)
		{
			return BSGCloneHelper.ObjectCloneAction.GoDeeper;
		}
		return BSGCloneHelper.ObjectCloneAction.Clone;
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x0003DFC0 File Offset: 0x0003C1C0
	public static List<GameObject> GetObjectsToClone(GameObject gameObject)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			BSGCloneHelper.ObjectCloneAction objectCloneAction = BSGCloneHelper.GetObjectCloneAction(child.gameObject);
			if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.Clone)
			{
				list.Add(child.gameObject);
			}
			else if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.GoDeeper)
			{
				List<GameObject> collection = new List<GameObject>();
				collection = BSGCloneHelper.GetObjectsToClone(child.gameObject);
				list.AddRange(collection);
			}
		}
		return list;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0003E034 File Offset: 0x0003C234
	public static List<GameObject> GetObjectsToClone(GameObject gameObject, LayerMask layerMask)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			if (layerMask.value == (layerMask.value | 1 << child.gameObject.layer))
			{
				BSGCloneHelper.ObjectCloneAction objectCloneAction = BSGCloneHelper.GetObjectCloneAction(child.gameObject);
				if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.Clone)
				{
					list.Add(child.gameObject);
				}
				else if (objectCloneAction == BSGCloneHelper.ObjectCloneAction.GoDeeper)
				{
					List<GameObject> collection = new List<GameObject>();
					collection = BSGCloneHelper.GetObjectsToClone(child.gameObject, layerMask);
					list.AddRange(collection);
				}
			}
		}
		return list;
	}

	// Token: 0x0400052B RID: 1323
	private static Type[] DontCloneObjectsWithComponents = new Type[]
	{
		typeof(Camera),
		typeof(Light),
		typeof(Terrain),
		typeof(BSGTerrainEdit)
	};

	// Token: 0x0400052C RID: 1324
	private static Type[] CloneWholeObjectsWithComponents = new Type[0];

	// Token: 0x0400052D RID: 1325
	private static Type[] CloneChildrenForObjectsWithComponents = new Type[0];

	// Token: 0x02000568 RID: 1384
	public enum ObjectCloneAction
	{
		// Token: 0x04003036 RID: 12342
		GoDeeper,
		// Token: 0x04003037 RID: 12343
		Clone,
		// Token: 0x04003038 RID: 12344
		Ignore
	}
}
