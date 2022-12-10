using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F9 RID: 1017
	public class Tags : MonoBehaviour
	{
		// Token: 0x0600383F RID: 14399 RVA: 0x001BB510 File Offset: 0x001B9710
		private void Start()
		{
			this.tags_Dic = new Dictionary<int, Tag>();
			foreach (Tag tag in this.tags)
			{
				if (!this.tags_Dic.ContainsValue(tag))
				{
					this.tags_Dic.Add(tag.ID, tag);
				}
			}
			this.tags = new List<Tag>();
			foreach (KeyValuePair<int, Tag> keyValuePair in this.tags_Dic)
			{
				this.tags.Add(keyValuePair.Value);
			}
		}

		// Token: 0x06003840 RID: 14400 RVA: 0x001BB5E0 File Offset: 0x001B97E0
		public bool HasTag(Tag tag)
		{
			return this.tags_Dic.ContainsValue(tag);
		}

		// Token: 0x06003841 RID: 14401 RVA: 0x001BB5EE File Offset: 0x001B97EE
		public bool HasTag(int key)
		{
			return this.tags_Dic.ContainsKey(key);
		}

		// Token: 0x04002852 RID: 10322
		public List<Tag> tags = new List<Tag>();

		// Token: 0x04002853 RID: 10323
		protected Dictionary<int, Tag> tags_Dic;
	}
}
