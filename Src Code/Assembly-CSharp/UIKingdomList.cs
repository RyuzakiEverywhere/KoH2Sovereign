using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class UIKingdomList : MonoBehaviour
{
	// Token: 0x06001FF4 RID: 8180 RVA: 0x00125EAC File Offset: 0x001240AC
	private void Awake()
	{
		this.Refresh();
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x00125EB4 File Offset: 0x001240B4
	private void Refresh()
	{
		GameLogic.Get(true);
	}
}
