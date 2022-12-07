using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class RotateGameObject : MonoBehaviour
{
	// Token: 0x06000179 RID: 377 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000F4A4 File Offset: 0x0000D6A4
	private void FixedUpdate()
	{
		if (this.local)
		{
			base.transform.Rotate(base.transform.up, Time.fixedDeltaTime * this.rot_speed_x, Space.Self);
			return;
		}
		base.transform.Rotate(Time.fixedDeltaTime * new Vector3(this.rot_speed_x, this.rot_speed_y, this.rot_speed_z), Space.World);
	}

	// Token: 0x0400028B RID: 651
	public float rot_speed_x;

	// Token: 0x0400028C RID: 652
	public float rot_speed_y;

	// Token: 0x0400028D RID: 653
	public float rot_speed_z;

	// Token: 0x0400028E RID: 654
	public bool local;
}
