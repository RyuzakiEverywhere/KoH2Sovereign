using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000168 RID: 360
public static class PropertyChanger
{
	// Token: 0x06001238 RID: 4664 RVA: 0x000BF1ED File Offset: 0x000BD3ED
	public static GameObject GetObj(GameObject obj, string name)
	{
		if (obj != null)
		{
			return obj;
		}
		return GameObject.Find(name);
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x000BF200 File Offset: 0x000BD400
	public static string[] ListComponents(GameObject obj)
	{
		List<string> list = new List<string>();
		if (obj != null)
		{
			Component[] components = obj.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				string name = components[i].GetType().Name;
				list.Add(name);
			}
		}
		else
		{
			list.Add("RenderSettings");
		}
		return list.ToArray();
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x000BF25C File Offset: 0x000BD45C
	public static Component GetComponent(GameObject obj, string component)
	{
		if (obj == null || component == "")
		{
			return null;
		}
		foreach (Component component2 in obj.GetComponents<Component>())
		{
			if (component2.GetType().Name == component)
			{
				return component2;
			}
		}
		return null;
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x000BF2B0 File Offset: 0x000BD4B0
	public static string[] ListVariables(Type type, Type var_type = null, bool bWriteableOnly = false)
	{
		List<string> list = new List<string>();
		if (type != null)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
			foreach (FieldInfo fieldInfo in type.GetFields(bindingAttr))
			{
				if (!(var_type != null) || !(fieldInfo.FieldType != var_type))
				{
					string name = fieldInfo.Name;
					list.Add(name);
				}
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingAttr))
			{
				if ((!(var_type != null) || !(propertyInfo.PropertyType != var_type)) && (!bWriteableOnly || propertyInfo.CanWrite))
				{
					string name2 = propertyInfo.Name;
					list.Add(name2);
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x000BF370 File Offset: 0x000BD570
	public static Type GetType(GameObject obj, string component)
	{
		Type result = null;
		if (obj != null && component != "")
		{
			Component component2 = PropertyChanger.GetComponent(obj, component);
			if (component2 != null)
			{
				result = component2.GetType();
			}
		}
		else if (component == "RenderSettings")
		{
			result = typeof(RenderSettings);
		}
		return result;
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000BF3C8 File Offset: 0x000BD5C8
	public static string[] ListVariables(GameObject obj, string component, Type var_type = null, bool bWriteableOnly = false)
	{
		return PropertyChanger.ListVariables(PropertyChanger.GetType(obj, component), var_type, bWriteableOnly);
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x000BF3D8 File Offset: 0x000BD5D8
	public static bool GetTypeAndInstance(GameObject obj, string component, out Type type, out object instance)
	{
		type = null;
		instance = null;
		if (component == "")
		{
			return false;
		}
		if (obj != null)
		{
			instance = PropertyChanger.GetComponent(obj, component);
			if (instance == null)
			{
				return false;
			}
			type = instance.GetType();
		}
		else if (component == "RenderSettings")
		{
			type = typeof(RenderSettings);
		}
		return type != null;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x000BF440 File Offset: 0x000BD640
	public static object GetVar(GameObject obj, string component, string variable)
	{
		if (variable == "")
		{
			return null;
		}
		Type type;
		object obj2;
		if (!PropertyChanger.GetTypeAndInstance(obj, component, out type, out obj2))
		{
			return null;
		}
		try
		{
			FieldInfo field = type.GetField(variable);
			if (field != null)
			{
				return field.GetValue(obj2);
			}
			PropertyInfo property = type.GetProperty(variable);
			if (property != null)
			{
				return property.GetValue(obj2, null);
			}
		}
		catch
		{
		}
		return null;
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x000BF4C0 File Offset: 0x000BD6C0
	public static bool SetVar(GameObject obj, string component, string variable, object value)
	{
		if (variable == "")
		{
			return false;
		}
		Type type;
		object obj2;
		if (!PropertyChanger.GetTypeAndInstance(obj, component, out type, out obj2))
		{
			return false;
		}
		try
		{
			FieldInfo field = type.GetField(variable);
			if (field != null)
			{
				field.SetValue(obj2, value);
				return true;
			}
			PropertyInfo property = type.GetProperty(variable);
			if (property != null)
			{
				property.SetValue(obj2, value, null);
				return true;
			}
		}
		catch
		{
		}
		return false;
	}
}
