using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class UIResetTweenOnEnable : MonoBehaviour
{
	// Token: 0x06002F95 RID: 12181 RVA: 0x00184AF2 File Offset: 0x00182CF2
	private void Awake()
	{
		this.m_Tweens = base.GetComponents<UITweener>();
		if (this.m_Tweens == null)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x00184B10 File Offset: 0x00182D10
	private void OnEnable()
	{
		for (int i = 0; i < this.m_Tweens.Length; i++)
		{
			this.m_Tweens[i].ResetToBeginning();
			this.m_Tweens[i].PlayForward();
		}
	}

	// Token: 0x04001FFC RID: 8188
	private UITweener[] m_Tweens;
}
