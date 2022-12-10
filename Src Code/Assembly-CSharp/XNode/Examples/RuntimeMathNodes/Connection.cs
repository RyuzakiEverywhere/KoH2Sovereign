using System;
using UnityEngine;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x02000361 RID: 865
	public class Connection : MonoBehaviour
	{
		// Token: 0x060033D0 RID: 13264 RVA: 0x001A0CDC File Offset: 0x0019EEDC
		public void SetPosition(Vector2 start, Vector2 end)
		{
			if (!this.rectTransform)
			{
				this.rectTransform = (RectTransform)base.transform;
			}
			base.transform.position = (start + end) * 0.5f;
			float z = Mathf.Atan2(start.y - end.y, start.x - end.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
			this.rectTransform.sizeDelta = new Vector2(Vector2.Distance(start, end), this.rectTransform.sizeDelta.y);
		}

		// Token: 0x040022D9 RID: 8921
		private RectTransform rectTransform;
	}
}
