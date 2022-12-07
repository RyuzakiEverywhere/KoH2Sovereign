using System;
using UnityEngine;
using UnityEngine.Sprites;

// Token: 0x02000009 RID: 9
[ExecuteInEditMode]
public class SpriteMaskController : MonoBehaviour
{
	// Token: 0x06000011 RID: 17 RVA: 0x000023BE File Offset: 0x000005BE
	private void OnEnable()
	{
		this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.m_uvs = DataUtility.GetInnerUV(this.m_spriteRenderer.sprite);
		this.m_spriteRenderer.sharedMaterial.SetVector("_CustomUVS", this.m_uvs);
	}

	// Token: 0x0400000D RID: 13
	private SpriteRenderer m_spriteRenderer;

	// Token: 0x0400000E RID: 14
	private Vector4 m_uvs;
}
