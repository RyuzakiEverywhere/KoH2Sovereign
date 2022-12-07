using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class AnimatedUVs : MonoBehaviour
{
	// Token: 0x06000068 RID: 104 RVA: 0x000046FA File Offset: 0x000028FA
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00004708 File Offset: 0x00002908
	private void Update()
	{
		this.offsety += Time.deltaTime * this.speedY;
		this.offsetx += Time.deltaTime * this.speedx;
		this.rend.material.SetTextureOffset("_MainTex", new Vector2(this.offsetx, this.offsety));
	}

	// Token: 0x04000093 RID: 147
	public float speedY = 0.5f;

	// Token: 0x04000094 RID: 148
	public float speedx;

	// Token: 0x04000095 RID: 149
	private float offsety;

	// Token: 0x04000096 RID: 150
	private float offsetx;

	// Token: 0x04000097 RID: 151
	private Renderer rend;
}
