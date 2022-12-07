using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class CandleFlicker : MonoBehaviour
{
	// Token: 0x06000BF5 RID: 3061 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00086648 File Offset: 0x00084848
	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float num = Mathf.PerlinNoise(CandleFlicker.t, 0f);
		CandleFlicker.t += this.speed;
		Light component = base.GetComponent<Light>();
		if (component != null)
		{
			component.color = this.HighClr * num + this.LowClr * (1f - num);
		}
	}

	// Token: 0x0400095E RID: 2398
	public Color HighClr;

	// Token: 0x0400095F RID: 2399
	public Color LowClr;

	// Token: 0x04000960 RID: 2400
	public float speed = 1f;

	// Token: 0x04000961 RID: 2401
	private static float t;
}
