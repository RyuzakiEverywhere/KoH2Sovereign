using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A1 RID: 161
public class TerrainDecal : MonoBehaviour
{
	// Token: 0x060005A5 RID: 1445 RVA: 0x000023FD File Offset: 0x000005FD
	private void Cleanup()
	{
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0003E138 File Offset: 0x0003C338
	private void Recalc()
	{
		this.Cleanup();
		if (this.verticesPerMeter <= 0.1f)
		{
			this.verticesPerMeter = 0.1f;
		}
		int num = (int)(this.width * this.verticesPerMeter);
		int num2 = (int)(this.height * this.verticesPerMeter);
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		float x = this.width / (float)num;
		float z = this.height / (float)num2;
		this.mesh = MeshUtils.CreateGridMesh(new Vector3(-this.width / 2f, 0f, -this.height / 2f), num, num2, new Vector3(x, 0f, 0f), new Vector3(0f, 0f, z), this.tileHorizontally, this.tileVertically);
		this.mesh.RecalculateNormals();
		MeshFilter meshFilter = base.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		meshFilter.mesh = this.mesh;
		MeshRenderer meshRenderer = base.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material = this.material;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this.UpdateMesh();
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0003E270 File Offset: 0x0003C470
	private void UpdateMesh()
	{
		if (this.mesh == null)
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component != null)
			{
				this.mesh = component.mesh;
			}
		}
		if (this.mesh == null)
		{
			return;
		}
		MeshUtils.SnapMeshToTerrain(this.mesh, base.transform, this.heightOffset, false, null);
		this.mesh.RecalculateNormals();
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0003E2DB File Offset: 0x0003C4DB
	private void Start()
	{
		if (this.mesh == null)
		{
			this.Recalc();
			return;
		}
		this.UpdateMesh();
	}

	// Token: 0x0400052E RID: 1326
	public Material material;

	// Token: 0x0400052F RID: 1327
	public float width = 5f;

	// Token: 0x04000530 RID: 1328
	public float height = 5f;

	// Token: 0x04000531 RID: 1329
	public bool tileHorizontally;

	// Token: 0x04000532 RID: 1330
	public bool tileVertically;

	// Token: 0x04000533 RID: 1331
	public float verticesPerMeter = 1f;

	// Token: 0x04000534 RID: 1332
	public float heightOffset = 0.1f;

	// Token: 0x04000535 RID: 1333
	private Mesh mesh;
}
