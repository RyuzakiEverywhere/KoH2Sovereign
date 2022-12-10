using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003BC RID: 956
	[CreateAssetMenu(menuName = "Malbers Animations/Actions")]
	public class Action : ScriptableObject
	{
		// Token: 0x060035C6 RID: 13766 RVA: 0x001AF44D File Offset: 0x001AD64D
		public static implicit operator int(Action reference)
		{
			return reference.ID;
		}

		// Token: 0x04002566 RID: 9574
		[Tooltip("Value for the transitions IDAction on the Animator in order to Execute the desirable animation clip")]
		public int ID;
	}
}
