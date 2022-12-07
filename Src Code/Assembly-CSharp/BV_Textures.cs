using System;
using UnityEngine;

// Token: 0x020000B9 RID: 185
[ExecuteInEditMode]
public class BV_Textures : MonoBehaviour
{
	// Token: 0x060007A2 RID: 1954 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x000508FA File Offset: 0x0004EAFA
	private void OnValidate()
	{
		Shader.SetGlobalTexture("_SplatsTA", this.TA_details);
		Shader.SetGlobalTexture("_SplatsTA_normals", this.TA_details_normals);
	}

	// Token: 0x0400063D RID: 1597
	[SerializeField]
	public Texture2DArray TA_details;

	// Token: 0x0400063E RID: 1598
	public Texture2DArray TA_details_normals;

	// Token: 0x0400063F RID: 1599
	[SerializeField]
	public Vector3[] splat_indexes;
}
