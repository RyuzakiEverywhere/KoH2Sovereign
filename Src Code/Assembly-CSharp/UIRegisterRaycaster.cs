using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000308 RID: 776
public class UIRegisterRaycaster : MonoBehaviour
{
	// Token: 0x06003074 RID: 12404 RVA: 0x00188E01 File Offset: 0x00187001
	private void Awake()
	{
		this.raycaster = base.GetComponent<GraphicRaycaster>();
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x00188E0F File Offset: 0x0018700F
	private void OnEnable()
	{
		if (this.raycaster != null)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null)
			{
				return;
			}
			baseUI.raycasters.Add(this.raycaster);
		}
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x00188E39 File Offset: 0x00187039
	private void OnDisable()
	{
		if (this.raycaster != null)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null)
			{
				return;
			}
			baseUI.raycasters.Remove(this.raycaster);
		}
	}

	// Token: 0x04002086 RID: 8326
	private GraphicRaycaster raycaster;
}
