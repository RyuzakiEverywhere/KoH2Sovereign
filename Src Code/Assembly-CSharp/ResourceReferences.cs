using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class ResourceReferences : ScriptableObject
{
	// Token: 0x040004BD RID: 1213
	public List<ResourceReferences.Ref> refs;

	// Token: 0x02000553 RID: 1363
	[Serializable]
	public struct Ref
	{
		// Token: 0x0600439D RID: 17309 RVA: 0x001FDFD2 File Offset: 0x001FC1D2
		public override string ToString()
		{
			return this.path + ": " + ((this.obj == null) ? "NULL" : this.obj.ToString());
		}

		// Token: 0x04002FF0 RID: 12272
		public string path;

		// Token: 0x04002FF1 RID: 12273
		public Object obj;
	}
}
