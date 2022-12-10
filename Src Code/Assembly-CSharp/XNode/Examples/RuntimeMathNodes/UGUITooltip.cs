using System;
using UnityEngine;
using UnityEngine.UI;

namespace XNode.Examples.RuntimeMathNodes
{
	// Token: 0x0200036A RID: 874
	public class UGUITooltip : MonoBehaviour
	{
		// Token: 0x0600340F RID: 13327 RVA: 0x001A1C73 File Offset: 0x0019FE73
		private void Awake()
		{
			this.graph = base.GetComponentInParent<RuntimeMathGraph>();
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x001A1C81 File Offset: 0x0019FE81
		private void Start()
		{
			this.Hide();
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x001A1C89 File Offset: 0x0019FE89
		private void Update()
		{
			if (this.show)
			{
				this.UpdatePosition();
			}
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x001A1C99 File Offset: 0x0019FE99
		public void Show()
		{
			this.show = true;
			this.group.alpha = 1f;
			this.UpdatePosition();
			base.transform.SetAsLastSibling();
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x001A1CC3 File Offset: 0x0019FEC3
		public void Hide()
		{
			this.show = false;
			this.group.alpha = 0f;
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x001A1CDC File Offset: 0x0019FEDC
		private void UpdatePosition()
		{
			RectTransform rect = this.graph.scrollRect.content.transform as RectTransform;
			Camera worldCamera = this.graph.gameObject.GetComponentInParent<Canvas>().worldCamera;
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, worldCamera, out v);
			base.transform.localPosition = v;
		}

		// Token: 0x04002300 RID: 8960
		public CanvasGroup group;

		// Token: 0x04002301 RID: 8961
		public Text label;

		// Token: 0x04002302 RID: 8962
		private bool show;

		// Token: 0x04002303 RID: 8963
		private RuntimeMathGraph graph;
	}
}
