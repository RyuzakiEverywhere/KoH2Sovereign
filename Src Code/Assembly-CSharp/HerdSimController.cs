using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class HerdSimController : MonoBehaviour
{
	// Token: 0x060000F2 RID: 242 RVA: 0x00008C8A File Offset: 0x00006E8A
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, this._roamingArea * 2f);
	}

	// Token: 0x04000186 RID: 390
	public Vector3 _roamingArea;

	// Token: 0x04000187 RID: 391
	public ParticleSystem _runPS;

	// Token: 0x04000188 RID: 392
	public ParticleSystem _deadPS;
}
