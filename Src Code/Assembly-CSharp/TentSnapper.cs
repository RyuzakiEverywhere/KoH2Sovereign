using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class TentSnapper : MonoBehaviour
{
	// Token: 0x06001166 RID: 4454 RVA: 0x000B7479 File Offset: 0x000B5679
	private void Awake()
	{
		this.CacheOffcets();
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x000B7481 File Offset: 0x000B5681
	private void OnEnable()
	{
		this.Snap();
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x000B748C File Offset: 0x000B568C
	private void Snap()
	{
		if (this.m_Offsets == null)
		{
			return;
		}
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			float terrainHeight = Common.GetTerrainHeight(child.position, null, false);
			Vector3 position = child.position;
			position.y = terrainHeight;
			child.position = position;
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x000B74EC File Offset: 0x000B56EC
	private void CacheOffcets()
	{
		int childCount = base.transform.childCount;
		this.m_Offsets = new Vector3[childCount];
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			this.m_Offsets[i] = child.localPosition;
		}
	}

	// Token: 0x04000B89 RID: 2953
	private Vector3[] m_Offsets;
}
