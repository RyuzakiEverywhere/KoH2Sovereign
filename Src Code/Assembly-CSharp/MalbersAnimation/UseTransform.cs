using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x02000411 RID: 1041
	public class UseTransform : MonoBehaviour
	{
		// Token: 0x0600387B RID: 14459 RVA: 0x001BC028 File Offset: 0x001BA228
		private void Update()
		{
			if (this.updateMode == UseTransform.UpdateMode.Update)
			{
				this.SetTransformReference();
			}
		}

		// Token: 0x0600387C RID: 14460 RVA: 0x001BC038 File Offset: 0x001BA238
		private void LateUpdate()
		{
			if (this.updateMode == UseTransform.UpdateMode.LateUpdate)
			{
				this.SetTransformReference();
			}
		}

		// Token: 0x0600387D RID: 14461 RVA: 0x001BC049 File Offset: 0x001BA249
		private void FixedUpdate()
		{
			if (this.updateMode == UseTransform.UpdateMode.FixedUpdate)
			{
				this.SetTransformReference();
			}
		}

		// Token: 0x0600387E RID: 14462 RVA: 0x001BC05A File Offset: 0x001BA25A
		private void SetTransformReference()
		{
			if (!this.Reference)
			{
				return;
			}
			base.transform.position = this.Reference.position;
			base.transform.rotation = this.Reference.rotation;
		}

		// Token: 0x040028DC RID: 10460
		public Transform Reference;

		// Token: 0x040028DD RID: 10461
		public UseTransform.UpdateMode updateMode = UseTransform.UpdateMode.LateUpdate;

		// Token: 0x02000930 RID: 2352
		public enum UpdateMode
		{
			// Token: 0x040042B6 RID: 17078
			Update,
			// Token: 0x040042B7 RID: 17079
			LateUpdate,
			// Token: 0x040042B8 RID: 17080
			FixedUpdate
		}
	}
}
