using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class RandomMaterial : MonoBehaviour
{
	// Token: 0x06000076 RID: 118 RVA: 0x00004A16 File Offset: 0x00002C16
	public void Start()
	{
		this.ChangeMaterial();
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004A1E File Offset: 0x00002C1E
	public void ChangeMaterial()
	{
		this.targetRenderer.sharedMaterial = this.materials[Random.Range(0, this.materials.Length)];
	}

	// Token: 0x040000A4 RID: 164
	public Renderer targetRenderer;

	// Token: 0x040000A5 RID: 165
	public Material[] materials;
}
