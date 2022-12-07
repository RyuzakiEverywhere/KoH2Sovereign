using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000C7 RID: 199
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MapBorderMesh : MonoBehaviour
{
	// Token: 0x060008E9 RID: 2281 RVA: 0x00060EE0 File Offset: 0x0005F0E0
	public void UpdateMesh()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh mesh;
		if (component.sharedMesh != null)
		{
			mesh = component.sharedMesh;
		}
		else
		{
			mesh = new Mesh();
		}
		Vector3[] array = new Vector3[8];
		int[] array2 = new int[24];
		Vector2[] array3 = new Vector2[8];
		array[0] = new Vector3(0f, -100f, 0f);
		array[1] = new Vector3(0f, 100f, 0f);
		array[2] = new Vector3(this.width, -100f, 0f);
		array[3] = new Vector3(this.width, 100f, 0f);
		array[4] = new Vector3(this.width, -100f, this.length);
		array[5] = new Vector3(this.width, 100f, this.length);
		array[6] = new Vector3(0f, -100f, this.length);
		array[7] = new Vector3(0f, 100f, this.length);
		for (int i = 0; i < 2; i++)
		{
			array3[i * 4] = new Vector2(0f, 0f);
			array3[i * 4 + 1] = new Vector2(0f, 1f);
			array3[i * 4 + 2] = new Vector2(1f, 0f);
			array3[i * 4 + 3] = new Vector2(1f, 1f);
		}
		for (int j = 0; j < 4; j++)
		{
			int num = j * 2;
			int num2 = j * 6;
			array2[num2] = num;
			array2[num2 + 1] = num + 1;
			array2[num2 + 2] = (num + 2) % 8;
			array2[num2 + 3] = (num + 2) % 8;
			array2[num2 + 4] = num + 1;
			array2[num2 + 5] = (num + 3) % 8;
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.uv = array3;
		mesh.name = "Border mesh";
		component.sharedMesh = mesh;
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		this.mat.SetFloat("_WaterLevel", MapData.GetWaterLevel());
		component2.material = this.mat;
		component2.shadowCastingMode = ShadowCastingMode.Off;
	}

	// Token: 0x0400070B RID: 1803
	public float width = 1200f;

	// Token: 0x0400070C RID: 1804
	public float length = 1200f;

	// Token: 0x0400070D RID: 1805
	public Material mat;
}
