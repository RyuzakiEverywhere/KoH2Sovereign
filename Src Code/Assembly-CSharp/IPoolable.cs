using System;

// Token: 0x0200007F RID: 127
public interface IPoolable
{
	// Token: 0x060004D0 RID: 1232
	void OnPoolSpawned();

	// Token: 0x060004D1 RID: 1233
	void OnPoolActivated();

	// Token: 0x060004D2 RID: 1234
	void OnPoolDeactivated();

	// Token: 0x060004D3 RID: 1235
	void OnPoolDestroyed();
}
