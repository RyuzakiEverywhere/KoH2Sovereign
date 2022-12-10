using System;
using UnityEngine;

namespace XNode.Examples.StateGraph
{
	// Token: 0x02000360 RID: 864
	[CreateAssetMenu(fileName = "New State Graph", menuName = "xNode Examples/State Graph")]
	public class StateGraph : NodeGraph
	{
		// Token: 0x060033CE RID: 13262 RVA: 0x001A0CCF File Offset: 0x0019EECF
		public void Continue()
		{
			this.current.MoveNext();
		}

		// Token: 0x040022D8 RID: 8920
		public StateNode current;
	}
}
