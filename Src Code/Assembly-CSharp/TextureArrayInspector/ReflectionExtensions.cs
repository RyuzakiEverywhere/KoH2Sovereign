using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000480 RID: 1152
	public static class ReflectionExtensions
	{
		// Token: 0x06003C4E RID: 15438 RVA: 0x001CBA66 File Offset: 0x001C9C66
		public static object CallStaticMethodFrom(string assembly, string type, string method, object[] parameters)
		{
			return Assembly.Load(assembly).GetType(type).GetMethod(method).Invoke(null, parameters);
		}

		// Token: 0x06003C4F RID: 15439 RVA: 0x001CBA84 File Offset: 0x001C9C84
		public static void GetPropertiesFrom<T1, T2>(this T1 dst, T2 src) where T1 : class where T2 : class
		{
			PropertyInfo[] properties = src.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
			PropertyInfo[] properties2 = src.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			for (int i = 0; i < properties.Length; i++)
			{
				for (int j = 0; j < properties2.Length; j++)
				{
					if (properties[i].Name == properties2[j].Name && properties2[j].CanWrite)
					{
						properties2[j].SetValue(dst, properties[i].GetValue(src, null), null);
					}
				}
			}
		}

		// Token: 0x06003C50 RID: 15440 RVA: 0x001CBB18 File Offset: 0x001C9D18
		public static IEnumerable<FieldInfo> UsableFields(this Type type, bool nonPublic = false, bool includeStatic = false)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if (nonPublic)
			{
				bindingFlags |= BindingFlags.NonPublic;
			}
			if (includeStatic)
			{
				bindingFlags |= BindingFlags.Static;
			}
			FieldInfo[] fields = type.GetFields(bindingFlags);
			int num;
			for (int i = 0; i < fields.Length; i = num + 1)
			{
				FieldInfo fieldInfo = fields[i];
				if (!fieldInfo.IsLiteral && !fieldInfo.FieldType.IsPointer && !fieldInfo.IsNotSerialized)
				{
					yield return fieldInfo;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06003C51 RID: 15441 RVA: 0x001CBB36 File Offset: 0x001C9D36
		public static IEnumerable<PropertyInfo> UsableProperties(this Type type, bool nonPublic = false, bool skipItems = true)
		{
			BindingFlags bindingAttr;
			if (nonPublic)
			{
				bindingAttr = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			else
			{
				bindingAttr = (BindingFlags.Instance | BindingFlags.Public);
			}
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			int num;
			for (int i = 0; i < properties.Length; i = num + 1)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.CanWrite && (!skipItems || !(propertyInfo.Name == "Item")))
				{
					yield return propertyInfo;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06003C52 RID: 15442 RVA: 0x001CBB54 File Offset: 0x001C9D54
		public static IEnumerable<MemberInfo> UsableMembers(this Type type, bool nonPublic = false, bool skipItems = true)
		{
			BindingFlags flags;
			if (nonPublic)
			{
				flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			else
			{
				flags = (BindingFlags.Instance | BindingFlags.Public);
			}
			FieldInfo[] fields = type.GetFields(flags);
			int num;
			for (int i = 0; i < fields.Length; i = num + 1)
			{
				FieldInfo fieldInfo = fields[i];
				if (!fieldInfo.IsLiteral && !fieldInfo.FieldType.IsPointer && !fieldInfo.IsNotSerialized)
				{
					yield return fieldInfo;
				}
				num = i;
			}
			PropertyInfo[] properties = type.GetProperties(flags);
			for (int i = 0; i < properties.Length; i = num + 1)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.CanWrite && (!skipItems || !(propertyInfo.Name == "Item")))
				{
					yield return propertyInfo;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06003C53 RID: 15443 RVA: 0x001CBB74 File Offset: 0x001C9D74
		public static void PrintAllFields(this Type type, BindingFlags flags)
		{
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				Debug.Log(fields[i].Name + ", field, " + flags.ToString());
			}
			PropertyInfo[] properties = type.GetProperties(flags);
			for (int j = 0; j < properties.Length; j++)
			{
				Debug.Log(properties[j].Name + ", property, " + flags.ToString());
			}
			MethodInfo[] methods = type.GetMethods(flags);
			for (int k = 0; k < methods.Length; k++)
			{
				Debug.Log(methods[k].Name + ", method, " + flags.ToString());
			}
		}

		// Token: 0x06003C54 RID: 15444 RVA: 0x001CBC38 File Offset: 0x001C9E38
		public static void PrintAllFields(this Type type)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
			type.PrintAllFields(flags);
			flags = (BindingFlags.Instance | BindingFlags.NonPublic);
			type.PrintAllFields(flags);
			flags = (BindingFlags.Static | BindingFlags.Public);
			type.PrintAllFields(flags);
			flags = (BindingFlags.Static | BindingFlags.NonPublic);
			type.PrintAllFields(flags);
		}

		// Token: 0x06003C55 RID: 15445 RVA: 0x001CBC70 File Offset: 0x001C9E70
		public static Component CopyComponent(Component src, GameObject go)
		{
			Type type = src.GetType();
			Component component = go.GetComponent(src.GetType());
			if (component == null)
			{
				component = go.AddComponent(type);
			}
			foreach (FieldInfo fieldInfo in type.UsableFields(true, false))
			{
				fieldInfo.SetValue(component, fieldInfo.GetValue(src));
			}
			foreach (PropertyInfo propertyInfo in type.UsableProperties(true, true))
			{
				if (!(propertyInfo.Name == "name"))
				{
					try
					{
						propertyInfo.SetValue(component, propertyInfo.GetValue(src, null), null);
					}
					catch
					{
					}
				}
			}
			return component;
		}

		// Token: 0x06003C56 RID: 15446 RVA: 0x001CBD60 File Offset: 0x001C9F60
		public static IEnumerable<Type> Subtypes(this Type parent)
		{
			Assembly assembly = Assembly.GetAssembly(parent);
			Type[] types = assembly.GetTypes();
			int num;
			for (int t = 0; t < types.Length; t = num + 1)
			{
				Type type = types[t];
				if (type.IsSubclassOf(parent) && !type.IsInterface && !type.IsAbstract)
				{
					yield return type;
				}
				num = t;
			}
			yield break;
		}

		// Token: 0x06003C57 RID: 15447 RVA: 0x000448AF File Offset: 0x00042AAF
		public static Type GetTerrainInspectorType()
		{
			return null;
		}

		// Token: 0x06003C58 RID: 15448 RVA: 0x001CBD70 File Offset: 0x001C9F70
		public static object GetTerrainInspectorField(string fieldName, Type inspectorType = null)
		{
			if (inspectorType == null)
			{
				inspectorType = ReflectionExtensions.GetTerrainInspectorType();
			}
			object[] array = Resources.FindObjectsOfTypeAll(inspectorType);
			object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				object value = inspectorType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(array2[i], null);
				if (value != null)
				{
					return value;
				}
			}
			return null;
		}

		// Token: 0x06003C59 RID: 15449 RVA: 0x001CBDC0 File Offset: 0x001C9FC0
		public static void SetTerrainInspectorField(string fieldName, object obj, Type inspectorType = null)
		{
			if (inspectorType == null)
			{
				inspectorType = ReflectionExtensions.GetTerrainInspectorType();
			}
			object[] array = Resources.FindObjectsOfTypeAll(inspectorType);
			object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				inspectorType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(array2[i], obj, null);
			}
		}

		// Token: 0x06003C5A RID: 15450 RVA: 0x001CBE08 File Offset: 0x001CA008
		public static T GetAddComponent<T>(this GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (t == null)
			{
				t = go.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06003C5B RID: 15451 RVA: 0x001CBE34 File Offset: 0x001CA034
		public static void ReflectionReset<T>(this T obj)
		{
			Type type = obj.GetType();
			T t = (T)((object)Activator.CreateInstance(type));
			foreach (FieldInfo fieldInfo in type.UsableFields(true, false))
			{
				fieldInfo.SetValue(obj, fieldInfo.GetValue(t));
			}
			foreach (PropertyInfo propertyInfo in type.UsableProperties(true, true))
			{
				propertyInfo.SetValue(obj, propertyInfo.GetValue(t, null), null);
			}
		}

		// Token: 0x06003C5C RID: 15452 RVA: 0x001CBF08 File Offset: 0x001CA108
		public static T ReflectionCopy<T>(this T obj)
		{
			Type type = obj.GetType();
			T t = (T)((object)Activator.CreateInstance(type));
			foreach (FieldInfo fieldInfo in type.UsableFields(true, false))
			{
				fieldInfo.SetValue(t, fieldInfo.GetValue(obj));
			}
			foreach (PropertyInfo propertyInfo in type.UsableProperties(true, true))
			{
				propertyInfo.SetValue(t, propertyInfo.GetValue(obj, null), null);
			}
			return t;
		}

		// Token: 0x06003C5D RID: 15453 RVA: 0x001CBFDC File Offset: 0x001CA1DC
		public static void ReflectionCopyFrom<T>(this T dst, object src)
		{
			Type type = dst.GetType();
			Type type2 = src.GetType();
			foreach (FieldInfo fieldInfo in type.UsableFields(true, false))
			{
				FieldInfo field = type2.GetField(fieldInfo.Name);
				if (field != null && field.FieldType == fieldInfo.FieldType)
				{
					fieldInfo.SetValue(dst, field.GetValue(src));
				}
			}
		}
	}
}
