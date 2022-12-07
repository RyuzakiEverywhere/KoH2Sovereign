using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class RockTheBoat : MonoBehaviour
{
	// Token: 0x0600112E RID: 4398 RVA: 0x000B62D2 File Offset: 0x000B44D2
	private void Start()
	{
		this.ofs = Random.Range(0, 10000);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x000B62E8 File Offset: 0x000B44E8
	private void Update()
	{
		float num = Mathf.PerlinNoise(1f - Time.time * this.speed + (float)this.ofs, Time.time * this.speed + (float)this.ofs);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = (num - 0.5f) * 2f * this.amplitude;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x04000B6C RID: 2924
	public float amplitude = 10f;

	// Token: 0x04000B6D RID: 2925
	public float speed = 0.5f;

	// Token: 0x04000B6E RID: 2926
	private int ofs;
}
