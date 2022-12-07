using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class TrebuchetDesync : MonoBehaviour
{
	// Token: 0x06001179 RID: 4473 RVA: 0x000B7BD0 File Offset: 0x000B5DD0
	private void Start()
	{
		this.Hide();
		base.Invoke("Show", Random.Range(this.min, this.max));
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x000B7BF4 File Offset: 0x000B5DF4
	private void Hide()
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x000B7C30 File Offset: 0x000B5E30
	private void Show()
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	// Token: 0x04000B91 RID: 2961
	public float min = 5f;

	// Token: 0x04000B92 RID: 2962
	public float max = 10f;
}
