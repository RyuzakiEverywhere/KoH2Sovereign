using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class LockAxis : MonoBehaviour
{
	// Token: 0x06000200 RID: 512 RVA: 0x0001F7E4 File Offset: 0x0001D9E4
	private void Update()
	{
		Vector3 position = base.transform.position;
		if (this.LockX)
		{
			position.x = 0f;
		}
		if (this.LockY)
		{
			position.y = 0f;
		}
		if (this.LockZ)
		{
			position.z = 0f;
		}
		base.transform.position = position;
	}

	// Token: 0x04000316 RID: 790
	public bool LockX = true;

	// Token: 0x04000317 RID: 791
	public bool LockY;

	// Token: 0x04000318 RID: 792
	public bool LockZ;
}
