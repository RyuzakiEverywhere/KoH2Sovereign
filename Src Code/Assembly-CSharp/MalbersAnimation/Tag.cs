using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F8 RID: 1016
	[CreateAssetMenu(menuName = "Malbers Animations/Tag")]
	public class Tag : ScriptableObject
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x0600383B RID: 14395 RVA: 0x001BB4EB File Offset: 0x001B96EB
		public int ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x0600383C RID: 14396 RVA: 0x001BB4F3 File Offset: 0x001B96F3
		public static implicit operator int(Tag reference)
		{
			return reference.ID;
		}

		// Token: 0x0600383D RID: 14397 RVA: 0x001BB4FB File Offset: 0x001B96FB
		private void OnEnable()
		{
			this.id = base.name.GetHashCode();
		}

		// Token: 0x04002851 RID: 10321
		[SerializeField]
		private int id;
	}
}
