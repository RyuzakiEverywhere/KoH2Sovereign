using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class ScreenEdgeBlock : MonoBehaviour
{
	// Token: 0x06002995 RID: 10645 RVA: 0x0016140C File Offset: 0x0015F60C
	private void OnEnable()
	{
		ScreenEdgeBlock.screenEdgeBlocks.Add(this);
		if (this.rect == null)
		{
			this.rect = base.GetComponent<RectTransform>();
			if (this.rect == null)
			{
				this.rect = base.GetComponentInChildren<RectTransform>();
			}
		}
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x00161458 File Offset: 0x0015F658
	public static void CacheEdges()
	{
		for (int i = 0; i < ScreenEdgeBlock.screenEdgeBlocks.Count; i++)
		{
			ScreenEdgeBlock screenEdgeBlock = ScreenEdgeBlock.screenEdgeBlocks[i];
			screenEdgeBlock.rect.GetWorldCorners(screenEdgeBlock.v);
		}
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x00161497 File Offset: 0x0015F697
	private void OnDisable()
	{
		ScreenEdgeBlock.screenEdgeBlocks.Remove(this);
	}

	// Token: 0x04001C32 RID: 7218
	public static List<ScreenEdgeBlock> screenEdgeBlocks = new List<ScreenEdgeBlock>();

	// Token: 0x04001C33 RID: 7219
	[NonSerialized]
	public RectTransform rect;

	// Token: 0x04001C34 RID: 7220
	[NonSerialized]
	public Vector3[] v = new Vector3[4];
}
