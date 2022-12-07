using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class FlockWaypointTrigger : MonoBehaviour
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00006910 File Offset: 0x00004B10
	public void Start()
	{
		if (this._flockChild == null)
		{
			this._flockChild = base.transform.parent.GetComponent<FlockChild>();
		}
		float num = Random.Range(this._timer, this._timer * 3f);
		base.InvokeRepeating("Trigger", num, num);
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00006966 File Offset: 0x00004B66
	public void Trigger()
	{
		this._flockChild.Wander(0f);
	}

	// Token: 0x04000122 RID: 290
	public float _timer = 1f;

	// Token: 0x04000123 RID: 291
	public FlockChild _flockChild;
}
