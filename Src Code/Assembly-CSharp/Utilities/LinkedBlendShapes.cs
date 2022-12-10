using System;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200046F RID: 1135
	[ExecuteInEditMode]
	public class LinkedBlendShapes : MonoBehaviour
	{
		// Token: 0x06003B68 RID: 15208 RVA: 0x001C6F42 File Offset: 0x001C5142
		private void Start()
		{
			base.enabled = false;
		}

		// Token: 0x06003B69 RID: 15209 RVA: 0x001C6F4B File Offset: 0x001C514B
		private void Update()
		{
			this.UpdateSlaveBlendShapes();
		}

		// Token: 0x06003B6A RID: 15210 RVA: 0x001C6F54 File Offset: 0x001C5154
		public virtual void UpdateSlaveBlendShapes()
		{
			if (this.master && this.slave && this.slave.sharedMesh)
			{
				for (int i = 0; i < this.slave.sharedMesh.blendShapeCount; i++)
				{
					this.slave.SetBlendShapeWeight(i, this.master.GetBlendShapeWeight(i));
				}
			}
		}

		// Token: 0x04002B33 RID: 11059
		public SkinnedMeshRenderer master;

		// Token: 0x04002B34 RID: 11060
		public SkinnedMeshRenderer slave;
	}
}
