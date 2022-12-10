using System;

namespace MalbersAnimations
{
	// Token: 0x020003ED RID: 1005
	public interface IInputSystem
	{
		// Token: 0x060037E4 RID: 14308
		float GetAxis(string Axis);

		// Token: 0x060037E5 RID: 14309
		float GetAxisRaw(string Axis);

		// Token: 0x060037E6 RID: 14310
		bool GetButtonDown(string button);

		// Token: 0x060037E7 RID: 14311
		bool GetButtonUp(string button);

		// Token: 0x060037E8 RID: 14312
		bool GetButton(string button);
	}
}
