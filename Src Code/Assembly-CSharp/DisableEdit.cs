using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class DisableEdit : MonoBehaviour
{
	// Token: 0x06001018 RID: 4120 RVA: 0x000AC0C1 File Offset: 0x000AA2C1
	public void Hide()
	{
		this.Apply(HideFlags.HideInHierarchy | HideFlags.NotEditable);
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x000AC0CB File Offset: 0x000AA2CB
	public void Show()
	{
		this.Apply(HideFlags.None);
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x000AC0D4 File Offset: 0x000AA2D4
	private void Apply(HideFlags flags)
	{
		int childCount = base.transform.childCount;
		if (childCount == 0)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Component[] components = child.GetComponents<Component>();
			for (int j = 0; j < components.Length; j++)
			{
				if (!(components[j] == null))
				{
					components[j].hideFlags = flags;
				}
			}
			child.gameObject.hideFlags = flags;
		}
	}
}
