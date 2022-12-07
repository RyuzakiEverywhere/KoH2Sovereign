using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
[ExecuteInEditMode]
public class FarPlanePerLayer : MonoBehaviour
{
	// Token: 0x06001025 RID: 4133 RVA: 0x000AC739 File Offset: 0x000AA939
	private void Start()
	{
		this.Apply();
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x000AC739 File Offset: 0x000AA939
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000AC744 File Offset: 0x000AA944
	public void Apply()
	{
		Camera component = base.GetComponent<Camera>();
		if (component == null)
		{
			return;
		}
		component.layerCullDistances = this.layerCullDistances;
	}

	// Token: 0x04000A9E RID: 2718
	public float[] layerCullDistances = new float[32];
}
