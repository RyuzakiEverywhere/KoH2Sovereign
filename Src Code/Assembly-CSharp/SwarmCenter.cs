using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
public class SwarmCenter : MonoBehaviour
{
	// Token: 0x06000021 RID: 33 RVA: 0x00002B71 File Offset: 0x00000D71
	private void Start()
	{
		base.transform.Rotate(new Vector3(0f, Random.Range(0f, 360f), 0f));
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002B9C File Offset: 0x00000D9C
	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, this.speed * Time.deltaTime, 0f));
	}

	// Token: 0x0400002A RID: 42
	public float speed;
}
