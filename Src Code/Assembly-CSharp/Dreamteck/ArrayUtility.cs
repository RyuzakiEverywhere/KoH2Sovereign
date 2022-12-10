using System;

namespace Dreamteck
{
	// Token: 0x020004A2 RID: 1186
	public static class ArrayUtility
	{
		// Token: 0x06003E48 RID: 15944 RVA: 0x001DC7D8 File Offset: 0x001DA9D8
		public static void Add<T>(ref T[] array, T item)
		{
			T[] array2 = new T[array.Length + 1];
			array.CopyTo(array2, 0);
			array2[array2.Length - 1] = item;
			array = array2;
		}

		// Token: 0x06003E49 RID: 15945 RVA: 0x001DC80C File Offset: 0x001DAA0C
		public static bool Contains<T>(T[] array, T item)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003E4A RID: 15946 RVA: 0x001DC848 File Offset: 0x001DAA48
		public static int IndexOf<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(value))
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06003E4B RID: 15947 RVA: 0x001DC884 File Offset: 0x001DAA84
		public static void Insert<T>(ref T[] array, int index, T item)
		{
			T[] array2 = new T[array.Length + 1];
			for (int i = 0; i < array2.Length; i++)
			{
				if (i < index)
				{
					array2[i] = array[i];
				}
				else if (i > index)
				{
					array2[i] = array[i - 1];
				}
				else
				{
					array2[i] = item;
				}
			}
			array = array2;
		}

		// Token: 0x06003E4C RID: 15948 RVA: 0x001DC8E4 File Offset: 0x001DAAE4
		public static void RemoveAt<T>(ref T[] array, int index)
		{
			if (array.Length == 0)
			{
				return;
			}
			T[] array2 = new T[array.Length - 1];
			for (int i = 0; i < array.Length; i++)
			{
				if (i < index)
				{
					array2[i] = array[i];
				}
				else if (i > index)
				{
					array2[i - 1] = array[i];
				}
			}
			array = array2;
		}
	}
}
