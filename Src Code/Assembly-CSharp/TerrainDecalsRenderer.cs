using System;
using UnityEngine;

// Token: 0x02000326 RID: 806
[ExecuteInEditMode]
public class TerrainDecalsRenderer : MonoBehaviour
{
	// Token: 0x06003217 RID: 12823 RVA: 0x000023FD File Offset: 0x000005FD
	private void OnWillRenderObject()
	{
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x001962D8 File Offset: 0x001944D8
	private void OnRenderObject()
	{
		if (this.mesh == null || this.material == null || this.pos == null)
		{
			return;
		}
		MeshFilter component = this.mesh.GetComponent<MeshFilter>();
		if (component == null || component.sharedMesh == null)
		{
			return;
		}
		this.material.SetPass(0);
		Graphics.DrawMeshNow(component.sharedMesh, this.pos.transform.position, this.pos.transform.rotation);
	}

	// Token: 0x0400217D RID: 8573
	public GameObject mesh;

	// Token: 0x0400217E RID: 8574
	public Material material;

	// Token: 0x0400217F RID: 8575
	public GameObject pos;
}
