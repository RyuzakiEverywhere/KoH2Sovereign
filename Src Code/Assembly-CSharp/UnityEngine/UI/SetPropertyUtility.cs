using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	// Token: 0x0200034A RID: 842
	internal static class SetPropertyUtility
	{
		// Token: 0x060032D6 RID: 13014 RVA: 0x0019C378 File Offset: 0x0019A578
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060032D7 RID: 13015 RVA: 0x0019C3C7 File Offset: 0x0019A5C7
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060032D8 RID: 13016 RVA: 0x0019C3E8 File Offset: 0x0019A5E8
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
