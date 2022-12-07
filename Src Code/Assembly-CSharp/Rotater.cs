using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class Rotater : MonoBehaviour
{
	// Token: 0x0600003A RID: 58 RVA: 0x000033FD File Offset: 0x000015FD
	private void Update()
	{
		base.transform.Rotate(0f, 1f * this.Speed, 0f, Space.Self);
	}

	// Token: 0x0400006A RID: 106
	public float Speed;
}
