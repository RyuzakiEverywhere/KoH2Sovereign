using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class FixMyself : MonoBehaviour
{
	// Token: 0x060001F4 RID: 500 RVA: 0x0001F708 File Offset: 0x0001D908
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		material.shader = Shader.Find(material.shader.name);
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000023FD File Offset: 0x000005FD
	private void Update()
	{
	}
}
