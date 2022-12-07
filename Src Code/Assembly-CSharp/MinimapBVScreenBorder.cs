using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class MinimapBVScreenBorder : MonoBehaviour
{
	// Token: 0x06002360 RID: 9056 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Start()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x0013FADC File Offset: 0x0013DCDC
	public void Minimap_OnZoomChanged()
	{
		BaseUI baseUI = BaseUI.Get();
		MiniMap miniMap = (baseUI != null) ? baseUI.minimap : null;
		if (miniMap == null)
		{
			return;
		}
		if (miniMap.GetZoom() <= this.thinBorderZoomThreshold)
		{
			GameObject gameObject = this.thinBorder;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			GameObject gameObject2 = this.mediumBorder;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			GameObject gameObject3 = this.thickBorder;
			if (gameObject3 == null)
			{
				return;
			}
			gameObject3.SetActive(true);
			return;
		}
		else if (miniMap.GetZoom() <= this.mediumBorderZoomThreshold)
		{
			GameObject gameObject4 = this.thinBorder;
			if (gameObject4 != null)
			{
				gameObject4.SetActive(false);
			}
			GameObject gameObject5 = this.mediumBorder;
			if (gameObject5 != null)
			{
				gameObject5.SetActive(true);
			}
			GameObject gameObject6 = this.thickBorder;
			if (gameObject6 == null)
			{
				return;
			}
			gameObject6.SetActive(false);
			return;
		}
		else
		{
			GameObject gameObject7 = this.thinBorder;
			if (gameObject7 != null)
			{
				gameObject7.SetActive(true);
			}
			GameObject gameObject8 = this.mediumBorder;
			if (gameObject8 != null)
			{
				gameObject8.SetActive(false);
			}
			GameObject gameObject9 = this.thickBorder;
			if (gameObject9 == null)
			{
				return;
			}
			gameObject9.SetActive(false);
			return;
		}
	}

	// Token: 0x040017AB RID: 6059
	[UIFieldTarget("id_ThinBorder")]
	private GameObject thinBorder;

	// Token: 0x040017AC RID: 6060
	[UIFieldTarget("id_MediumBorder")]
	private GameObject mediumBorder;

	// Token: 0x040017AD RID: 6061
	[UIFieldTarget("id_ThickBorder")]
	private GameObject thickBorder;

	// Token: 0x040017AE RID: 6062
	[SerializeField]
	private float thinBorderZoomThreshold;

	// Token: 0x040017AF RID: 6063
	[SerializeField]
	private float mediumBorderZoomThreshold;
}
