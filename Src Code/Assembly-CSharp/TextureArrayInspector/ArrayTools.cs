using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000485 RID: 1157
	public static class ArrayTools
	{
		// Token: 0x06003CDD RID: 15581 RVA: 0x001D0479 File Offset: 0x001CE679
		public static void RemoveAt<T>(ref T[] array, int num)
		{
			array = ArrayTools.RemoveAt<T>(array, num);
		}

		// Token: 0x06003CDE RID: 15582 RVA: 0x001D0488 File Offset: 0x001CE688
		public static T[] RemoveAt<T>(T[] array, int num)
		{
			if (num >= array.Length || num < 0)
			{
				num = array.Length - 1;
			}
			T[] array2 = new T[array.Length - 1];
			if (num != 0)
			{
				Array.Copy(array, array2, num);
			}
			if (num != array.Length)
			{
				Array.Copy(array, num + 1, array2, num, array2.Length - num);
			}
			return array2;
		}

		// Token: 0x06003CDF RID: 15583 RVA: 0x001D04D3 File Offset: 0x001CE6D3
		public static void Remove<T>(ref T[] array, T obj) where T : class
		{
			array = ArrayTools.Remove<T>(array, obj);
		}

		// Token: 0x06003CE0 RID: 15584 RVA: 0x001D04E0 File Offset: 0x001CE6E0
		public static T[] Remove<T>(T[] array, T obj) where T : class
		{
			int num = array.Find(obj);
			return ArrayTools.RemoveAt<T>(array, num);
		}

		// Token: 0x06003CE1 RID: 15585 RVA: 0x001D04FC File Offset: 0x001CE6FC
		public static void Add<T>(ref T[] array, T element)
		{
			array = ArrayTools.Add<T>(array, element);
		}

		// Token: 0x06003CE2 RID: 15586 RVA: 0x001D0508 File Offset: 0x001CE708
		public static T[] Add<T>(T[] array, T element)
		{
			if (array == null || array.Length == 0)
			{
				return new T[]
				{
					element
				};
			}
			T[] array2 = new T[array.Length + 1];
			Array.Copy(array, array2, array.Length);
			array2[array.Length] = element;
			return array2;
		}

		// Token: 0x06003CE3 RID: 15587 RVA: 0x001D054B File Offset: 0x001CE74B
		public static void Insert<T>(ref T[] array, int pos, T element)
		{
			array = ArrayTools.Insert<T>(array, pos, element);
		}

		// Token: 0x06003CE4 RID: 15588 RVA: 0x001D0558 File Offset: 0x001CE758
		public static T[] Insert<T>(T[] array, int pos, T element)
		{
			if (array == null || array.Length == 0)
			{
				return new T[]
				{
					element
				};
			}
			if (pos > array.Length || pos < 0)
			{
				pos = array.Length;
			}
			T[] array2 = new T[array.Length + 1];
			if (pos != 0)
			{
				Array.Copy(array, array2, pos);
			}
			if (pos != array.Length)
			{
				Array.Copy(array, pos, array2, pos + 1, array.Length - pos);
			}
			array2[pos] = element;
			return array2;
		}

		// Token: 0x06003CE5 RID: 15589 RVA: 0x001D05C0 File Offset: 0x001CE7C0
		public static T[] InsertRange<T>(T[] array, int after, T[] add)
		{
			if (after > array.Length || after < 0)
			{
				after = array.Length;
			}
			T[] array2 = new T[array.Length + add.Length];
			if (after != 0)
			{
				Array.Copy(array, array2, after);
			}
			Array.Copy(add, 0, array2, after, add.Length);
			if (after != array.Length)
			{
				Array.Copy(array, after, array2, after + add.Length, array.Length - after);
			}
			return array2;
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x001D0619 File Offset: 0x001CE819
		public static void Resize<T>(ref T[] array, int newSize, Func<int, T> createElement = null)
		{
			array = ArrayTools.Resize<T>(array, newSize, createElement);
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x001D0628 File Offset: 0x001CE828
		public static T[] Resize<T>(T[] array, int newSize, Func<int, T> createElement = null)
		{
			T[] array2 = new T[newSize];
			Array.Copy(array, array2, (newSize < array.Length) ? newSize : array.Length);
			if (newSize > array.Length && createElement != null)
			{
				for (int i = array.Length; i < newSize; i++)
				{
					array2[i] = createElement(i);
				}
			}
			return array2;
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x001D0675 File Offset: 0x001CE875
		public static void Append<T>(ref T[] array, T[] additional)
		{
			array = ArrayTools.Append<T>(array, additional);
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x001D0684 File Offset: 0x001CE884
		public static T[] Append<T>(T[] array, T[] additional)
		{
			T[] array2 = new T[array.Length + additional.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			for (int j = 0; j < additional.Length; j++)
			{
				array2[j + array.Length] = additional[j];
			}
			return array2;
		}

		// Token: 0x06003CEA RID: 15594 RVA: 0x001D06DC File Offset: 0x001CE8DC
		public static void Switch<T>(T[] array, int num1, int num2)
		{
			if (num1 < 0 || num1 >= array.Length || num2 < 0 || num2 >= array.Length)
			{
				return;
			}
			T t = array[num1];
			array[num1] = array[num2];
			array[num2] = t;
		}

		// Token: 0x06003CEB RID: 15595 RVA: 0x001D071C File Offset: 0x001CE91C
		public static void Switch<T>(T[] array, T obj1, T obj2) where T : class
		{
			int num = array.Find(obj1);
			int num2 = array.Find(obj2);
			ArrayTools.Switch<T>(array, num, num2);
		}

		// Token: 0x06003CEC RID: 15596 RVA: 0x001D0744 File Offset: 0x001CE944
		public static T[] Truncated<T>(this T[] src, int length)
		{
			T[] array = new T[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = src[i];
			}
			return array;
		}

		// Token: 0x06003CED RID: 15597 RVA: 0x001D0774 File Offset: 0x001CE974
		public static bool Equals<T>(T[] a1, T[] a2) where T : class
		{
			if (a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003CEE RID: 15598 RVA: 0x001D07B8 File Offset: 0x001CE9B8
		public static bool EqualsEquatable<T>(T[] a1, T[] a2) where T : IEquatable<T>
		{
			if (a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				if (!object.Equals(a1[i], a2[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003CEF RID: 15599 RVA: 0x001D0800 File Offset: 0x001CEA00
		public static bool EqualsVector3(Vector3[] a1, Vector3[] a2, float delta = 1E-45f)
		{
			if (a1 == null || a2 == null || a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				float num = a1[i].x - a2[i].x;
				if (num >= delta || -num >= delta)
				{
					return false;
				}
				num = a1[i].y - a2[i].y;
				if (num >= delta || -num >= delta)
				{
					return false;
				}
				num = a1[i].z - a2[i].z;
				if (num >= delta || -num >= delta)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003CF0 RID: 15600 RVA: 0x001D089C File Offset: 0x001CEA9C
		public static int Find<T>(this T[] array, T obj)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (object.Equals(array[i], obj))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x001D08D4 File Offset: 0x001CEAD4
		public static int FindCount<T>(this T[] array, T obj)
		{
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (object.Equals(array[i], obj))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003CF2 RID: 15602 RVA: 0x001D0910 File Offset: 0x001CEB10
		public static bool Contains<T>(this T[] array, T obj)
		{
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (object.Equals(array[i], obj))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003CF3 RID: 15603 RVA: 0x001D094C File Offset: 0x001CEB4C
		public static void RemoveAll<T>(ref T[] array, T obj) where T : class
		{
			array = ArrayTools.RemoveAll<T>(array, obj);
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x001D0958 File Offset: 0x001CEB58
		public static T[] RemoveAll<T>(T[] array, T obj) where T : class
		{
			bool[] array2 = new bool[array.Length];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (object.Equals(array[i], obj))
				{
					num++;
					array2[i] = true;
				}
			}
			T[] array3 = new T[array.Length - num];
			int num2 = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (!array2[j])
				{
					array3[num2] = array[j];
					num2++;
				}
			}
			return array3;
		}

		// Token: 0x06003CF5 RID: 15605 RVA: 0x001D09E0 File Offset: 0x001CEBE0
		public static bool MatchElements<T>(T[] arr1, T[] arr2)
		{
			if (arr1.Length != arr2.Length)
			{
				return false;
			}
			bool[] array = new bool[arr1.Length];
			for (int i = 0; i < arr1.Length; i++)
			{
				for (int j = 0; j < arr2.Length; j++)
				{
					if (!array[j] && object.Equals(arr1[i], arr2[j]))
					{
						array[j] = true;
						break;
					}
				}
			}
			for (int k = 0; k < array.Length; k++)
			{
				if (!array[k])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003CF6 RID: 15606 RVA: 0x001D0A5C File Offset: 0x001CEC5C
		public static void CheckAdd<T>(ref T[] array, T element)
		{
			array = ArrayTools.CheckAdd<T>(array, element);
		}

		// Token: 0x06003CF7 RID: 15607 RVA: 0x001D0A68 File Offset: 0x001CEC68
		public static T[] CheckAdd<T>(T[] array, T element)
		{
			if (array.FindCount(element) == 0)
			{
				return ArrayTools.Add<T>(array, element);
			}
			return array;
		}

		// Token: 0x06003CF8 RID: 15608 RVA: 0x001D0A7C File Offset: 0x001CEC7C
		public static T Any<T>(T[] array) where T : class
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					return array[i];
				}
			}
			return default(T);
		}

		// Token: 0x06003CF9 RID: 15609 RVA: 0x001D0AB8 File Offset: 0x001CECB8
		public static bool IsEmpty<T>(T[] array) where T : class
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x001D0AE4 File Offset: 0x001CECE4
		public static string ToStringMemberwise(this Array array, string separator = ", ")
		{
			string text = "";
			if (array.Length == 0)
			{
				return text;
			}
			for (int i = 0; i < array.Length; i++)
			{
				object value = array.GetValue(i);
				text += ((value != null) ? value.ToString() : "");
				if (i != array.Length - 1)
				{
					text += separator;
				}
			}
			return text;
		}

		// Token: 0x06003CFB RID: 15611 RVA: 0x001D0B45 File Offset: 0x001CED45
		public static void QSort(float[] array)
		{
			ArrayTools.QSort(array, 0, array.Length - 1);
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x001D0B54 File Offset: 0x001CED54
		public static void QSort(float[] array, int l, int r)
		{
			float num = array[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				while (array[i] < num)
				{
					i++;
				}
				while (array[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = array[i];
					array[i] = array[num2];
					array[num2] = num3;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayTools.QSort(array, i, r);
			}
			if (l < num2)
			{
				ArrayTools.QSort(array, l, num2);
			}
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x001D0BBD File Offset: 0x001CEDBD
		public static void QSort<T>(T[] array, float[] reference)
		{
			ArrayTools.QSort<T>(array, reference, 0, reference.Length - 1);
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x001D0BCC File Offset: 0x001CEDCC
		public static void QSort<T>(T[] array, float[] reference, int l, int r)
		{
			float num = reference[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				while (reference[i] < num)
				{
					i++;
				}
				while (reference[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = reference[i];
					reference[i] = reference[num2];
					reference[num2] = num3;
					T t = array[i];
					array[i] = array[num2];
					array[num2] = t;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayTools.QSort<T>(array, reference, i, r);
			}
			if (l < num2)
			{
				ArrayTools.QSort<T>(array, reference, l, num2);
			}
		}

		// Token: 0x06003CFF RID: 15615 RVA: 0x001D0C57 File Offset: 0x001CEE57
		public static void QSort<T>(List<T> list, float[] reference)
		{
			ArrayTools.QSort<T>(list, reference, 0, reference.Length - 1);
		}

		// Token: 0x06003D00 RID: 15616 RVA: 0x001D0C68 File Offset: 0x001CEE68
		public static void QSort<T>(List<T> list, float[] reference, int l, int r)
		{
			float num = reference[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				while (reference[i] < num)
				{
					i++;
				}
				while (reference[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = reference[i];
					reference[i] = reference[num2];
					reference[num2] = num3;
					T value = list[i];
					list[i] = list[num2];
					list[num2] = value;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayTools.QSort<T>(list, reference, i, r);
			}
			if (l < num2)
			{
				ArrayTools.QSort<T>(list, reference, l, num2);
			}
		}

		// Token: 0x06003D01 RID: 15617 RVA: 0x001D0CF4 File Offset: 0x001CEEF4
		public static int[] Order(int[] array, int[] order = null, int max = 0, int steps = 1000000, int[] stepsArray = null)
		{
			if (max == 0)
			{
				max = array.Length;
			}
			if (stepsArray == null)
			{
				stepsArray = new int[steps + 1];
			}
			else
			{
				steps = stepsArray.Length - 1;
			}
			int[] array2 = new int[steps + 1];
			for (int i = 0; i < max; i++)
			{
				array2[array[i]]++;
			}
			int num = 0;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] += num;
				num = array2[j];
			}
			for (int k = array2.Length - 1; k > 0; k--)
			{
				array2[k] = array2[k - 1];
			}
			array2[0] = 0;
			if (order == null)
			{
				order = new int[max];
			}
			for (int l = 0; l < max; l++)
			{
				int num2 = array[l];
				int num3 = array2[num2];
				order[num3] = l;
				array2[num2]++;
			}
			return order;
		}

		// Token: 0x06003D02 RID: 15618 RVA: 0x001D0DC4 File Offset: 0x001CEFC4
		public static T[] Convert<T, Y>(Y[] src)
		{
			T[] array = new T[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i] = (T)((object)src[i]);
			}
			return array;
		}
	}
}
