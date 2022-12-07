using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class Red_UVScroller_refr : MonoBehaviour
{
	// Token: 0x06000072 RID: 114 RVA: 0x000048B9 File Offset: 0x00002AB9
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06000073 RID: 115 RVA: 0x000048C8 File Offset: 0x00002AC8
	private void OnEnable()
	{
		if (this.rend == null)
		{
			return;
		}
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_DistortTex", new Vector2(0f, 0f));
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_RefractionLayer", new Vector2(0f, 0f));
		this.timeWentX = 0f;
		this.timeWentY = 0f;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004950 File Offset: 0x00002B50
	private void Update()
	{
		if (this.rend == null)
		{
			return;
		}
		this.timeWentY += Time.deltaTime * this.speedY;
		this.timeWentX += Time.deltaTime * this.speedX;
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_DistortTex", new Vector2(this.timeWentX, this.timeWentY));
		this.rend.materials[this.targetMaterialSlot].SetTextureOffset("_RefractionLayer", new Vector2(this.timeWentX, this.timeWentY));
	}

	// Token: 0x0400009E RID: 158
	public int targetMaterialSlot;

	// Token: 0x0400009F RID: 159
	public float speedY = 0.5f;

	// Token: 0x040000A0 RID: 160
	public float speedX = 0.5f;

	// Token: 0x040000A1 RID: 161
	private float timeWentX;

	// Token: 0x040000A2 RID: 162
	private float timeWentY;

	// Token: 0x040000A3 RID: 163
	private Renderer rend;
}
