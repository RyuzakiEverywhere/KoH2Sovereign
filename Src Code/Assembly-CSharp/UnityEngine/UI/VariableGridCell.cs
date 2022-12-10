using System;

namespace UnityEngine.UI
{
	// Token: 0x0200034C RID: 844
	[ExecuteInEditMode]
	public class VariableGridCell : MonoBehaviour
	{
		// Token: 0x06003309 RID: 13065 RVA: 0x0019CEDA File Offset: 0x0019B0DA
		private void Init()
		{
			this.rect = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600330A RID: 13066 RVA: 0x0019CEE8 File Offset: 0x0019B0E8
		private void OnEnable()
		{
			this.Init();
		}

		// Token: 0x0600330B RID: 13067 RVA: 0x0019CEF0 File Offset: 0x0019B0F0
		public float SetProgress(float val, float max)
		{
			if (base.gameObject == null)
			{
				return 0f;
			}
			if (max <= 0f)
			{
				base.gameObject.SetActive(false);
				return 0f;
			}
			this.rect.sizeDelta = new Vector2(max, this.rect.sizeDelta.y);
			if (val > 1f)
			{
				val = 1f;
			}
			if (val < 0f)
			{
				val = 0f;
			}
			base.gameObject.SetActive(true);
			float num = val;
			if (float.IsNaN(num))
			{
				num = 0f;
			}
			Vector3 localScale = base.gameObject.transform.localScale;
			localScale.x = num;
			base.gameObject.transform.localScale = localScale;
			return num;
		}

		// Token: 0x04002264 RID: 8804
		public float desired_val;

		// Token: 0x04002265 RID: 8805
		public RectTransform rect;
	}
}
