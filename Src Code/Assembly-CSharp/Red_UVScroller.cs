using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class Red_UVScroller : MonoBehaviour
{
	// Token: 0x0600006E RID: 110 RVA: 0x000047B2 File Offset: 0x000029B2
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000047C0 File Offset: 0x000029C0
	private void OnEnable()
	{
		if (this.rend == null)
		{
			return;
		}
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_MainTex", new Vector2(0f, 0f));
		this.timeWentX = 0f;
		this.timeWentY = 0f;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00004820 File Offset: 0x00002A20
	private void Update()
	{
		if (this.rend == null)
		{
			return;
		}
		this.timeWentY += Time.deltaTime * this.speedY;
		this.timeWentX += Time.deltaTime * this.speedX;
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_MainTex", new Vector2(this.timeWentX, this.timeWentY));
	}

	// Token: 0x04000098 RID: 152
	public int targetMaterialSlot;

	// Token: 0x04000099 RID: 153
	public float speedY = 0.5f;

	// Token: 0x0400009A RID: 154
	public float speedX = 0.5f;

	// Token: 0x0400009B RID: 155
	private float timeWentX;

	// Token: 0x0400009C RID: 156
	private float timeWentY;

	// Token: 0x0400009D RID: 157
	private Renderer rend;
}
