using System;
using UnityStandardAssets.CrossPlatformInput;

namespace MalbersAnimations
{
	// Token: 0x020003F1 RID: 1009
	public class CrossPlatform : IInputSystem
	{
		// Token: 0x060037F5 RID: 14325 RVA: 0x001BABF0 File Offset: 0x001B8DF0
		public float GetAxis(string Axis)
		{
			return CrossPlatformInputManager.GetAxis(Axis);
		}

		// Token: 0x060037F6 RID: 14326 RVA: 0x001BABF8 File Offset: 0x001B8DF8
		public float GetAxisRaw(string Axis)
		{
			return CrossPlatformInputManager.GetAxisRaw(Axis);
		}

		// Token: 0x060037F7 RID: 14327 RVA: 0x001BAC00 File Offset: 0x001B8E00
		public bool GetButton(string button)
		{
			return CrossPlatformInputManager.GetButton(button);
		}

		// Token: 0x060037F8 RID: 14328 RVA: 0x001BAC08 File Offset: 0x001B8E08
		public bool GetButtonDown(string button)
		{
			return CrossPlatformInputManager.GetButtonDown(button);
		}

		// Token: 0x060037F9 RID: 14329 RVA: 0x001BAC10 File Offset: 0x001B8E10
		public bool GetButtonUp(string button)
		{
			return CrossPlatformInputManager.GetButtonUp(button);
		}
	}
}
