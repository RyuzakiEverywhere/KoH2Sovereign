using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000488 RID: 1160
	public static class CustomSerialization
	{
		// Token: 0x06003D6B RID: 15723 RVA: 0x001D28E8 File Offset: 0x001D0AE8
		public static Type GetStandardAssembliesType(string s)
		{
			if (s.StartsWith("Plugins."))
			{
				s = s.Replace("Plugins", typeof(CustomSerialization).Namespace);
			}
			Type type = Type.GetType(s);
			if (type == null)
			{
				type = Type.GetType(s + ", UnityEngine");
			}
			if (type == null)
			{
				type = Type.GetType(s + ", Assembly-CSharp-Editor");
			}
			if (type == null)
			{
				type = Type.GetType(s + ", Assembly-CSharp");
			}
			if (type == null)
			{
				if (s.StartsWith("MapMagic"))
				{
					s = s.Replace("MapMagic", "Plugins");
				}
				if (s.StartsWith("Voxeland"))
				{
					s = s.Replace("Voxeland", "Plugins");
				}
				type = Type.GetType(s);
				if (type == null)
				{
					type = Type.GetType(s + ", UnityEngine");
				}
				if (type == null)
				{
					type = Type.GetType(s + ", Assembly-CSharp-Editor");
				}
				if (type == null)
				{
					type = Type.GetType(s + ", Assembly-CSharp");
				}
			}
			return type;
		}

		// Token: 0x06003D6C RID: 15724 RVA: 0x001D2A13 File Offset: 0x001D0C13
		private static IEnumerable<CustomSerialization.Value> Values(object obj)
		{
			Type objType = obj.GetType();
			if (objType.IsArray)
			{
				Type elementType = objType.GetElementType();
				Array array = (Array)obj;
				if (elementType.IsPrimitive)
				{
					yield return new CustomSerialization.Value
					{
						name = "items",
						type = objType,
						obj = array
					};
				}
				else
				{
					int num;
					for (int i = 0; i < array.Length; i = num + 1)
					{
						yield return new CustomSerialization.Value
						{
							name = "item" + i,
							type = elementType,
							obj = array.GetValue(i)
						};
						num = i;
					}
				}
				elementType = null;
				array = null;
			}
			else if (objType.IsSubclassOf(typeof(Object)))
			{
				yield return new CustomSerialization.Value
				{
					name = "Object",
					type = objType,
					obj = obj
				};
			}
			else
			{
				foreach (FieldInfo fieldInfo in objType.UsableFields(false, true))
				{
					yield return new CustomSerialization.Value
					{
						name = fieldInfo.Name,
						type = fieldInfo.FieldType,
						obj = fieldInfo.GetValue(obj)
					};
				}
				IEnumerator<FieldInfo> enumerator = null;
				foreach (PropertyInfo propertyInfo in objType.UsableProperties(false, true))
				{
					yield return new CustomSerialization.Value
					{
						name = propertyInfo.Name,
						type = propertyInfo.PropertyType,
						obj = propertyInfo.GetValue(obj, null)
					};
				}
				IEnumerator<PropertyInfo> enumerator2 = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06003D6D RID: 15725 RVA: 0x001D2A24 File Offset: 0x001D0C24
		public static int WriteClass(object obj, List<string> classes, List<Object> objects, List<float> floats, List<object> references)
		{
			if (references.Contains(obj))
			{
				return references.IndexOf(obj);
			}
			Type type = obj.GetType();
			Type type2 = type.IsArray ? type.GetElementType() : null;
			string str = type.ToString();
			int count = classes.Count;
			classes.Add(null);
			for (int i = references.Count; i < classes.Count; i++)
			{
				references.Add(null);
			}
			references[count] = obj;
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write("<" + str);
			if (type.IsArray)
			{
				stringWriter.Write(" length=" + ((Array)obj).Length);
			}
			stringWriter.WriteLine(">");
			Func<object, int> <>9__0;
			foreach (CustomSerialization.Value value in CustomSerialization.Values(obj))
			{
				if (value.type.IsPrimitive)
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" value=",
						value.obj,
						"/>"
					}));
				}
				else if (value.obj == null)
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" null/>"
					}));
				}
				else if (value.type == typeof(string))
				{
					string text = (string)value.obj;
					text = text.Replace("\n", "\\n");
					text = text.Replace(" ", "\\_");
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" value=",
						text,
						"/>"
					}));
				}
				else if (typeof(CustomSerialization.IStruct).IsAssignableFrom(value.type))
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" ",
						((CustomSerialization.IStruct)value.obj).Encode(),
						"/>"
					}));
				}
				else if (typeof(CustomSerialization.IStructLink).IsAssignableFrom(value.type))
				{
					Func<object, int> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = ((object linkObj) => CustomSerialization.WriteClass(linkObj, classes, objects, floats, references)));
					}
					Func<object, int> writeClass = func;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" ",
						((CustomSerialization.IStructLink)value.obj).Encode(writeClass),
						"/>"
					}));
				}
				else if (type == typeof(float[]))
				{
					float[] array = (float[])obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<items type=",
						value.type,
						" start=",
						floats.Count,
						" length=",
						array.Length,
						"/>"
					}));
					floats.AddRange(array);
				}
				else if (type.IsArray && type2.IsPrimitive)
				{
					stringWriter.Write("\t<items type=" + value.type + " values=");
					Array array2 = (Array)obj;
					for (int j = 0; j < array2.Length; j++)
					{
						stringWriter.Write(array2.GetValue(j));
						if (j != array2.Length - 1)
						{
							stringWriter.Write(',');
						}
					}
					stringWriter.WriteLine("/>");
				}
				else if (value.type.IsSubclassOf(typeof(Object)))
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" object=",
						objects.Count,
						"/>"
					}));
					objects.Add((Object)value.obj);
				}
				else if (value.obj == null)
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" link=-1/>"
					}));
				}
				else if (value.type.IsClass && !value.type.IsValueType)
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" link=",
						CustomSerialization.WriteClass(value.obj, classes, objects, floats, references),
						"/>"
					}));
				}
				else if (value.type == typeof(Vector2))
				{
					Vector2 vector = (Vector2)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" x=",
						vector.x,
						" y=",
						vector.y,
						"/>"
					}));
				}
				else if (value.type == typeof(Vector3))
				{
					Vector3 vector2 = (Vector3)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" x=",
						vector2.x,
						" y=",
						vector2.y,
						" z=",
						vector2.z,
						"/>"
					}));
				}
				else if (value.type == typeof(Rect))
				{
					Rect rect = (Rect)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" x=",
						rect.x,
						" y=",
						rect.y,
						" width=",
						rect.width,
						" height=",
						rect.height,
						"/>"
					}));
				}
				else if (value.type == typeof(Color))
				{
					Color color = (Color)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" r=",
						color.r,
						" g=",
						color.g,
						" b=",
						color.b,
						" a=",
						color.a,
						"/>"
					}));
				}
				else if (value.type == typeof(Vector4))
				{
					Vector4 vector3 = (Vector4)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" x=",
						vector3.x,
						" y=",
						vector3.y,
						" z=",
						vector3.z,
						" w=",
						vector3.w,
						"/>"
					}));
				}
				else if (value.type == typeof(Quaternion))
				{
					Quaternion quaternion = (Quaternion)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" x=",
						quaternion.x,
						" y=",
						quaternion.y,
						" z=",
						quaternion.z,
						" w=",
						quaternion.w,
						"/>"
					}));
				}
				else if (value.type.IsEnum)
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" value=",
						(int)value.obj,
						"/>"
					}));
				}
				else if (value.type == typeof(Keyframe))
				{
					Keyframe keyframe = (Keyframe)value.obj;
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" time=",
						keyframe.time,
						" value=",
						keyframe.value,
						" in=",
						keyframe.inTangent,
						" out=",
						keyframe.outTangent,
						" mode=",
						keyframe.tangentMode,
						"/>"
					}));
				}
				else
				{
					stringWriter.WriteLine(string.Concat(new object[]
					{
						"\t<",
						value.name,
						" type=",
						value.type,
						" link=",
						CustomSerialization.WriteClass(value.obj, classes, objects, floats, references),
						"/>"
					}));
				}
			}
			stringWriter.WriteLine("</" + str + ">");
			stringWriter.Close();
			classes[count] = stringWriter.ToString();
			return count;
		}

		// Token: 0x06003D6E RID: 15726 RVA: 0x001D36DC File Offset: 0x001D18DC
		public static object ReadClass(int slotNum, List<string> classes, List<Object> objects, List<float> floats, List<object> references)
		{
			for (int i = references.Count; i < classes.Count; i++)
			{
				references.Add(null);
			}
			if (references[slotNum] != null)
			{
				return references[slotNum];
			}
			object obj = null;
			StringReader stringReader = new StringReader(classes[slotNum]);
			string text = stringReader.ReadLine();
			text = text.Substring(1, text.Length - 2);
			int num = 0;
			if (text.Contains(" length="))
			{
				string[] array = text.Split(new char[]
				{
					' '
				});
				num = (int)array[1].Parse(typeof(int));
				text = array[0];
			}
			Type standardAssembliesType = CustomSerialization.GetStandardAssembliesType(text);
			if (standardAssembliesType == null)
			{
				Debug.Log("Calss: Could not load " + text + " as this type does not exists anymore.");
				return null;
			}
			Type type = standardAssembliesType.IsArray ? standardAssembliesType.GetElementType() : null;
			if (standardAssembliesType.IsArray)
			{
				obj = Activator.CreateInstance(standardAssembliesType, new object[]
				{
					num
				});
			}
			else
			{
				try
				{
					obj = Activator.CreateInstance(standardAssembliesType);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Error deserializing ",
						standardAssembliesType,
						" ",
						obj,
						": ",
						ex.ToString()
					}));
				}
			}
			references[slotNum] = obj;
			List<CustomSerialization.Value> list = new List<CustomSerialization.Value>();
			Func<int, object> <>9__0;
			for (;;)
			{
				string text2 = stringReader.ReadLine();
				if (text2 == null || text2.StartsWith("</"))
				{
					break;
				}
				text2 = text2.Substring(2, text2.Length - 4);
				string[] array2 = text2.Split(new char[]
				{
					' ',
					','
				});
				CustomSerialization.Value value = default(CustomSerialization.Value);
				value.name = array2[0];
				string text3 = array2[1].Remove(0, 5);
				value.type = CustomSerialization.GetStandardAssembliesType(text3);
				if (value.type == null)
				{
					Debug.Log("Value: Could not load " + text3 + " as this type does not exists anymore.");
				}
				else if (value.type.IsArray && value.name == "items")
				{
					if (value.type == typeof(float[]))
					{
						int num2 = (int)array2[2].Parse(typeof(int));
						for (int j = num2; j < num2 + num; j++)
						{
							list.Add(new CustomSerialization.Value
							{
								name = "item",
								type = type,
								obj = floats[j]
							});
						}
					}
					else
					{
						for (int k = 2; k < array2.Length; k++)
						{
							list.Add(new CustomSerialization.Value
							{
								name = "item",
								type = type,
								obj = array2[k].Parse(type)
							});
						}
					}
				}
				else
				{
					if (array2[2] == "null")
					{
						value.obj = null;
					}
					else if (typeof(CustomSerialization.IStruct).IsAssignableFrom(value.type))
					{
						value.obj = Activator.CreateInstance(value.type);
						((CustomSerialization.IStruct)value.obj).Decode(array2);
					}
					else if (typeof(CustomSerialization.IStructLink).IsAssignableFrom(value.type))
					{
						Func<int, object> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = ((int link) => CustomSerialization.ReadClass(link, classes, objects, floats, references)));
						}
						Func<int, object> readClass = func;
						value.obj = Activator.CreateInstance(value.type);
						((CustomSerialization.IStructLink)value.obj).Decode(array2, readClass);
					}
					else if (array2[2].StartsWith("link"))
					{
						value.obj = CustomSerialization.ReadClass(int.Parse(array2[2].Remove(0, 5)), classes, objects, floats, references);
					}
					else if (value.type.IsPrimitive)
					{
						value.obj = array2[2].Parse(value.type);
					}
					else if (value.type.IsSubclassOf(typeof(Object)))
					{
						value.obj = objects[(int)array2[2].Parse(typeof(int))];
					}
					else if (value.type == typeof(string))
					{
						string text4 = (string)array2[2].Parse(value.type);
						text4 = text4.Replace("\\n", "\n");
						text4 = text4.Replace("\\_", " ");
						value.obj = text4;
					}
					else if (value.type == typeof(Vector2))
					{
						value.obj = new Vector2((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)));
					}
					else if (value.type == typeof(Vector3))
					{
						value.obj = new Vector3((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)));
					}
					else if (value.type == typeof(Rect))
					{
						value.obj = new Rect((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)), (float)array2[5].Parse(typeof(float)));
					}
					else if (value.type == typeof(Color))
					{
						value.obj = new Color((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)), (float)array2[5].Parse(typeof(float)));
					}
					else if (value.type == typeof(Vector4))
					{
						value.obj = new Vector4((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)), (float)array2[5].Parse(typeof(float)));
					}
					else if (value.type == typeof(Quaternion))
					{
						value.obj = new Quaternion((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)), (float)array2[5].Parse(typeof(float)));
					}
					else if (value.type == typeof(Keyframe))
					{
						value.obj = new Keyframe((float)array2[2].Parse(typeof(float)), (float)array2[3].Parse(typeof(float)), (float)array2[4].Parse(typeof(float)), (float)array2[5].Parse(typeof(float)))
						{
							tangentMode = (int)array2[6].Parse(typeof(int))
						};
					}
					else if (value.type.IsEnum)
					{
						value.obj = Enum.ToObject(value.type, (int)array2[2].Parse(typeof(int)));
					}
					list.Add(value);
				}
			}
			int count = list.Count;
			if (standardAssembliesType.IsArray)
			{
				Array array3 = (Array)obj;
				for (int l = 0; l < array3.Length; l++)
				{
					array3.SetValue(list[l].obj, l);
				}
			}
			else
			{
				foreach (FieldInfo fieldInfo in standardAssembliesType.UsableFields(false, true))
				{
					string name = fieldInfo.Name;
					Type fieldType = fieldInfo.FieldType;
					for (int m = 0; m < count; m++)
					{
						if (list[m].name == name && (list[m].type == fieldType || list[m].type.IsSubclassOf(fieldType)))
						{
							fieldInfo.SetValue(obj, list[m].obj);
						}
					}
				}
				foreach (PropertyInfo propertyInfo in standardAssembliesType.UsableProperties(false, true))
				{
					string name2 = propertyInfo.Name;
					Type propertyType = propertyInfo.PropertyType;
					for (int n = 0; n < count; n++)
					{
						if (list[n].name == name2 && list[n].type == propertyType)
						{
							propertyInfo.SetValue(obj, list[n].obj, null);
						}
					}
				}
			}
			return obj;
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x001D41D4 File Offset: 0x001D23D4
		public static object DeepCopy(object src)
		{
			List<string> classes = new List<string>();
			List<Object> objects = new List<Object>();
			List<float> floats = new List<float>();
			List<object> references = new List<object>();
			List<object> references2 = new List<object>();
			return CustomSerialization.ReadClass(CustomSerialization.WriteClass(src, classes, objects, floats, references), classes, objects, floats, references2);
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x001D4214 File Offset: 0x001D2414
		public static string ExportXML(List<string> classes, List<Object> objects, List<float> floats)
		{
			StringWriter stringWriter = new StringWriter();
			for (int i = 0; i < classes.Count; i++)
			{
				stringWriter.Write(classes[i]);
			}
			stringWriter.Write("<Floats values=");
			int count = floats.Count;
			for (int j = 0; j < count; j++)
			{
				stringWriter.Write(floats[j].ToString());
				if (j != count - 1)
				{
					stringWriter.Write(",");
				}
			}
			stringWriter.WriteLine("/>");
			stringWriter.Close();
			return stringWriter.ToString();
		}

		// Token: 0x06003D71 RID: 15729 RVA: 0x001D42A4 File Offset: 0x001D24A4
		public static void ImportXML(string xml, out List<string> classes, out List<Object> objects, out List<float> floats)
		{
			StringReader stringReader = new StringReader(xml);
			classes = new List<string>();
			objects = new List<Object>();
			floats = new List<float>();
			StringWriter stringWriter = null;
			for (;;)
			{
				string text = stringReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (!text.StartsWith("<Object"))
				{
					if (text.StartsWith("<Floats"))
					{
						text = text.Replace("<Floats values=", "");
						text = text.Replace("/>", "");
						if (text.Length != 0)
						{
							string[] array = text.Split(new char[]
							{
								','
							});
							for (int i = 0; i < array.Length; i++)
							{
								floats.Add(float.Parse(array[i]));
							}
						}
					}
					else
					{
						if (!text.Contains("/>") && !text.Contains("</"))
						{
							if (stringWriter != null)
							{
								classes.Add(stringWriter.ToString());
							}
							stringWriter = new StringWriter();
						}
						stringWriter.WriteLine(text);
					}
				}
			}
			classes.Add(stringWriter.ToString());
		}

		// Token: 0x0200096E RID: 2414
		public interface IStruct
		{
			// Token: 0x060053E8 RID: 21480
			string Encode();

			// Token: 0x060053E9 RID: 21481
			void Decode(string[] lineMembers);
		}

		// Token: 0x0200096F RID: 2415
		public interface IStructLink
		{
			// Token: 0x060053EA RID: 21482
			string Encode(Func<object, int> writeClass);

			// Token: 0x060053EB RID: 21483
			void Decode(string[] lineMembers, Func<int, object> readClass);
		}

		// Token: 0x02000970 RID: 2416
		private struct Value
		{
			// Token: 0x040043DB RID: 17371
			public string name;

			// Token: 0x040043DC RID: 17372
			public Type type;

			// Token: 0x040043DD RID: 17373
			public object obj;
		}
	}
}
