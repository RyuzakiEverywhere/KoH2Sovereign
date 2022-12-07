using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class PooledObject : MonoBehaviour
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x00038A58 File Offset: 0x00036C58
	private void OnDestroy()
	{
		ObjectPool.OnPooledObjectDestoryed(base.gameObject);
	}
}
