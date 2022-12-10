using System;
using System.Collections.Generic;
using UnityEngine;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A8 RID: 936
	public interface ITreesList
	{
		// Token: 0x06003557 RID: 13655
		List<TreeInstance> Get(int cx, int cy);
	}
}
