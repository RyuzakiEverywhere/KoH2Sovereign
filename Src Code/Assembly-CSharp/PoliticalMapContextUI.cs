using System;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class PoliticalMapContextUI : MonoBehaviour
{
	// Token: 0x06002601 RID: 9729 RVA: 0x0014F327 File Offset: 0x0014D527
	private void Start()
	{
		ViewMode.view_changed = (ViewMode.ViewChanged)Delegate.Combine(ViewMode.view_changed, new ViewMode.ViewChanged(this.ViewChanged));
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x0014F349 File Offset: 0x0014D549
	private void OnDestroy()
	{
		ViewMode.view_changed = (ViewMode.ViewChanged)Delegate.Remove(ViewMode.view_changed, new ViewMode.ViewChanged(this.ViewChanged));
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x0014F36C File Offset: 0x0014D56C
	public void ViewChanged(ViewMode mode)
	{
		if (this.currentWindow != null)
		{
			Object.Destroy(this.currentWindow);
		}
		GameObject prefab = UICommon.GetPrefab(mode.def_id + "Screen", null);
		if (prefab == null)
		{
			return;
		}
		this.currentWindow = Object.Instantiate<GameObject>(prefab, base.transform);
		UIPoliticalViewFilter component = this.currentWindow.GetComponent<UIPoliticalViewFilter>();
		if (!component)
		{
			Debug.LogWarning("PV Filter Missing");
			return;
		}
		component.Init(mode.def_id);
	}

	// Token: 0x040019BF RID: 6591
	private GameObject currentWindow;
}
