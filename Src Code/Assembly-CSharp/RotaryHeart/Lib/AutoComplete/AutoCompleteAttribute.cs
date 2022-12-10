using System;
using UnityEngine;

namespace RotaryHeart.Lib.AutoComplete
{
	// Token: 0x02000346 RID: 838
	public class AutoCompleteAttribute : PropertyAttribute
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x0600329F RID: 12959 RVA: 0x0019ADC9 File Offset: 0x00198FC9
		public string[] Entries
		{
			get
			{
				return this.m_entries;
			}
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x0019ADD1 File Offset: 0x00198FD1
		public AutoCompleteAttribute(string[] entries)
		{
			this.m_entries = entries;
		}

		// Token: 0x04002246 RID: 8774
		private string[] m_entries;
	}
}
