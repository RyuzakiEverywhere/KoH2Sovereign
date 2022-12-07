using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(UnderWaterFog))]
[ExecuteInEditMode]
public class FogControl : MonoBehaviour
{
	// Token: 0x06000040 RID: 64 RVA: 0x0000343D File Offset: 0x0000163D
	private void OnEnable()
	{
		this.init();
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000343D File Offset: 0x0000163D
	private void Start()
	{
		this.init();
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003448 File Offset: 0x00001648
	private void Update()
	{
		this.Rate += Time.deltaTime / this.FadeSpeed;
		this.Rate = Mathf.Clamp(this.Rate, 0f, this.FadeSpeed);
		if (this.cam.transform.position.y <= this.fog.height)
		{
			if (!this.fog.enabled)
			{
				this.fog.enabled = true;
			}
			this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 1f, this.Rate);
			return;
		}
		this.fog.fogColor.a = Mathf.Lerp(this.fog.fogColor.a, 0f, this.Rate * 2f);
		if (this.fog.fogColor.a <= 0.01f)
		{
			this.fog.enabled = false;
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003550 File Offset: 0x00001750
	private void init()
	{
		if (this.cam == null)
		{
			this.cam = base.GetComponent<Camera>();
		}
		if (this.fog == null)
		{
			this.fog = base.GetComponent<UnderWaterFog>();
		}
		if (this.cam.transform.position.y >= this.fog.height)
		{
			this.fog.fogColor.a = 0f;
		}
	}

	// Token: 0x0400006B RID: 107
	public float FadeSpeed = 10f;

	// Token: 0x0400006C RID: 108
	private float Rate = 1f;

	// Token: 0x0400006D RID: 109
	private UnderWaterFog fog;

	// Token: 0x0400006E RID: 110
	private Camera cam;
}
