using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class ScrollUV_Clouds : MonoBehaviour
{
	// Token: 0x06000033 RID: 51 RVA: 0x00003104 File Offset: 0x00001304
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00003114 File Offset: 0x00001314
	private void Update()
	{
		if (this.rend == null)
		{
			Object.Destroy(this);
			return;
		}
		float x = Time.time * this.scrollSpeed_X;
		float y = Time.time * this.scrollSpeed_Y;
		this.rend.material.mainTextureOffset = new Vector2(x, y);
	}

	// Token: 0x04000058 RID: 88
	public float scrollSpeed_X = 0.5f;

	// Token: 0x04000059 RID: 89
	public float scrollSpeed_Y = 0.5f;

	// Token: 0x0400005A RID: 90
	private Renderer rend;
}
