using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class AttributeConsoleHelper
{
	// Token: 0x0600012A RID: 298 RVA: 0x0000B67C File Offset: 0x0000987C
	public static T FindComponent<T>(int instanceID) where T : Component
	{
		foreach (T t in Object.FindObjectsOfType<T>())
		{
			if (t.GetInstanceID().Equals(instanceID))
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000B6C8 File Offset: 0x000098C8
	public static GameObject FindGameObject(int instanceID)
	{
		foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>())
		{
			if (gameObject.GetInstanceID().Equals(instanceID))
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000B704 File Offset: 0x00009904
	public static bool TryParseGameObject(string parse, out GameObject obj)
	{
		obj = null;
		int instanceID;
		if (!int.TryParse(parse, out instanceID))
		{
			return false;
		}
		obj = AttributeConsoleHelper.FindGameObject(instanceID);
		return true;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000B72C File Offset: 0x0000992C
	public static bool TryParseVector3(string parse, out Vector3 vector)
	{
		vector = default(Vector3);
		string[] array = parse.Replace("(", "").Replace(")", "").Trim().Split(new char[]
		{
			','
		});
		return array.Length == 3 && float.TryParse(array[0], out vector.x) && float.TryParse(array[1], out vector.y) && float.TryParse(array[2], out vector.z);
	}

	// Token: 0x0600012E RID: 302 RVA: 0x0000B7B4 File Offset: 0x000099B4
	public static bool TryParseVector2(string parse, out Vector2 vector)
	{
		vector = default(Vector2);
		string[] array = parse.Replace("(", "").Replace(")", "").Trim().Split(new char[]
		{
			','
		});
		return array.Length == 2 && float.TryParse(array[0], out vector.x) && float.TryParse(array[1], out vector.y);
	}
}
