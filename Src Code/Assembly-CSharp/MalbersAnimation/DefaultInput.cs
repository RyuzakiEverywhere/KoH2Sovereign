using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F0 RID: 1008
	public class DefaultInput : IInputSystem
	{
		// Token: 0x060037EE RID: 14318 RVA: 0x001BABC1 File Offset: 0x001B8DC1
		public float GetAxis(string Axis)
		{
			return Input.GetAxis(Axis);
		}

		// Token: 0x060037EF RID: 14319 RVA: 0x001BABC9 File Offset: 0x001B8DC9
		public float GetAxisRaw(string Axis)
		{
			return Input.GetAxisRaw(Axis);
		}

		// Token: 0x060037F0 RID: 14320 RVA: 0x001BABD1 File Offset: 0x001B8DD1
		public bool GetButton(string button)
		{
			return Input.GetButton(button);
		}

		// Token: 0x060037F1 RID: 14321 RVA: 0x001BABD9 File Offset: 0x001B8DD9
		public bool GetButtonDown(string button)
		{
			return Input.GetButtonDown(button);
		}

		// Token: 0x060037F2 RID: 14322 RVA: 0x001BABE1 File Offset: 0x001B8DE1
		public bool GetButtonUp(string button)
		{
			return Input.GetButtonUp(button);
		}

		// Token: 0x060037F3 RID: 14323 RVA: 0x001BABE9 File Offset: 0x001B8DE9
		public static IInputSystem GetInputSystem(string PlayerID = "")
		{
			return new CrossPlatform();
		}
	}
}
