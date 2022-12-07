using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class SimpleGPUInstancingExample : MonoBehaviour
{
	// Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
	private void Awake()
	{
		this.InstancedMaterial.enableInstancing = true;
		float num = 4f;
		for (int i = 0; i < 1000; i++)
		{
			Component component = Object.Instantiate<Transform>(this.Prefab, new Vector3(Random.Range(-num, num), num + Random.Range(-num, num), Random.Range(-num, num)), Quaternion.identity);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			Color value = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			materialPropertyBlock.SetColor("_Color", value);
			component.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
		}
	}

	// Token: 0x04000001 RID: 1
	public Transform Prefab;

	// Token: 0x04000002 RID: 2
	public Material InstancedMaterial;
}
