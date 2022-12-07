using System;
using UnityEngine;

// Token: 0x0200031C RID: 796
public class SetFPS : MonoBehaviour
{
	// Token: 0x060031F8 RID: 12792 RVA: 0x0019550F File Offset: 0x0019370F
	private void Start()
	{
		Application.targetFrameRate = this.fps;
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x0019551C File Offset: 0x0019371C
	private void OnValidate()
	{
		if (base.enabled)
		{
			Application.targetFrameRate = this.fps;
		}
	}

	// Token: 0x04002160 RID: 8544
	public int fps = 60;
}
