using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class MinimapPalette : MonoBehaviour
{
	// Token: 0x040017B1 RID: 6065
	[NonSerialized]
	public GameObject selection;

	// Token: 0x040017B2 RID: 6066
	[NonSerialized]
	public List<ViewMode> selectedButtons = new List<ViewMode>();
}
