using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class ScrollingUVs : MonoBehaviour
{
	// Token: 0x06000024 RID: 36 RVA: 0x00002BC4 File Offset: 0x00000DC4
	private void LateUpdate()
	{
		this.uvOffset += this.uvAnimationRate * Time.deltaTime;
		if (base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.textureName, this.uvOffset);
			if (this.ScrollBump)
			{
				base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.bumpName, this.uvOffset);
			}
		}
	}

	// Token: 0x0400002B RID: 43
	public int materialIndex;

	// Token: 0x0400002C RID: 44
	public Vector2 uvAnimationRate = new Vector2(1f, 0f);

	// Token: 0x0400002D RID: 45
	public string textureName = "_MainTex";

	// Token: 0x0400002E RID: 46
	public bool ScrollBump = true;

	// Token: 0x0400002F RID: 47
	public string bumpName = "_BumpMap";

	// Token: 0x04000030 RID: 48
	private Vector2 uvOffset = Vector2.zero;
}
