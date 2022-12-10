using System;

namespace UnityEngine.UI
{
	// Token: 0x0200034D RID: 845
	[ExecuteInEditMode]
	public class VariableGridLayoutGroup : MonoBehaviour
	{
		// Token: 0x0600330D RID: 13069 RVA: 0x0019CFB1 File Offset: 0x0019B1B1
		public void Init()
		{
			this.children = base.GetComponentsInChildren<VariableGridCell>(true);
			this.rect = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600330E RID: 13070 RVA: 0x0019CFCC File Offset: 0x0019B1CC
		private void OnEnable()
		{
			this.Init();
		}

		// Token: 0x0600330F RID: 13071 RVA: 0x0019CFD4 File Offset: 0x0019B1D4
		private void Update()
		{
			this.Refresh();
		}

		// Token: 0x06003310 RID: 13072 RVA: 0x0019CFDC File Offset: 0x0019B1DC
		public void Refresh()
		{
			this.rect_size = this.rect.rect.width;
			float num = this.padding_left - this.rect_size / 2f;
			float num2 = 0f;
			for (int i = 0; i < this.children.Length - 1; i++)
			{
				if (this.children[i].desired_val != 0f)
				{
					this.rect_size -= this.spacing;
				}
			}
			this.rect_size -= this.padding_left;
			for (int j = 0; j < this.children.Length; j++)
			{
				VariableGridCell variableGridCell = this.children[j];
				if (variableGridCell.gameObject.activeInHierarchy)
				{
					float num3 = variableGridCell.desired_val;
					num2 += num3;
					float num4 = num2 - 1f;
					if (num4 > 0f)
					{
						num3 -= num4;
					}
					variableGridCell.SetProgress(num3, this.rect_size);
					float num5 = num + this.spacing + variableGridCell.rect.rect.width * variableGridCell.rect.localScale.x;
					variableGridCell.transform.localPosition = new Vector3(num + variableGridCell.rect.rect.width * variableGridCell.rect.localScale.x / 2f, 0f, 0f);
					num = num5;
				}
			}
		}

		// Token: 0x04002266 RID: 8806
		private VariableGridCell[] children;

		// Token: 0x04002267 RID: 8807
		private float rect_size;

		// Token: 0x04002268 RID: 8808
		private RectTransform rect;

		// Token: 0x04002269 RID: 8809
		public float spacing = 1f;

		// Token: 0x0400226A RID: 8810
		public float padding_left;
	}
}
