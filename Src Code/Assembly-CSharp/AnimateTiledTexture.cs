using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200000B RID: 11
internal class AnimateTiledTexture : MonoBehaviour
{
	// Token: 0x06000016 RID: 22 RVA: 0x00002754 File Offset: 0x00000954
	private void Start()
	{
		base.StartCoroutine(this.updateTiling());
		Vector2 value = new Vector2(1f / (float)this.columns, 1f / (float)this.rows);
		base.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", value);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000027A5 File Offset: 0x000009A5
	private IEnumerator updateTiling()
	{
		for (;;)
		{
			this.index++;
			if (this.index >= this.rows * this.columns)
			{
				this.index = 0;
			}
			Vector2 value = new Vector2((float)this.index / (float)this.columns - (float)(this.index / this.columns), (float)(this.index / this.columns) / (float)this.rows);
			base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", value);
			yield return new WaitForSeconds(1f / this.framesPerSecond);
		}
		yield break;
	}

	// Token: 0x04000011 RID: 17
	public int columns = 2;

	// Token: 0x04000012 RID: 18
	public int rows = 2;

	// Token: 0x04000013 RID: 19
	public float framesPerSecond = 10f;

	// Token: 0x04000014 RID: 20
	private int index;
}
