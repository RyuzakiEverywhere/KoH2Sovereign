using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class AnimationEventListener : MonoBehaviour
{
	// Token: 0x06000D6A RID: 3434 RVA: 0x00097350 File Offset: 0x00095550
	public void OnEvent(string eventName)
	{
		this.OnAnimationEvent(eventName);
	}

	// Token: 0x04000A4D RID: 2637
	public Action<string> OnAnimationEvent;
}
