using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class SimpleMoveExample : MonoBehaviour
{
	// Token: 0x06000006 RID: 6 RVA: 0x00002115 File Offset: 0x00000315
	private void Start()
	{
		this.m_originalPosition = base.transform.position;
		this.m_previous = base.transform.position;
		this.m_target = base.transform.position;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000214C File Offset: 0x0000034C
	private void Update()
	{
		base.transform.position = Vector3.Slerp(this.m_previous, this.m_target, Time.deltaTime * this.Speed);
		this.m_previous = base.transform.position;
		if (Vector3.Distance(this.m_target, base.transform.position) < 0.1f)
		{
			this.m_target = base.transform.position + Random.onUnitSphere * Random.Range(0.7f, 4f);
			this.m_target.Set(Mathf.Clamp(this.m_target.x, this.m_originalPosition.x - this.BoundingVolume.x, this.m_originalPosition.x + this.BoundingVolume.x), Mathf.Clamp(this.m_target.y, this.m_originalPosition.y - this.BoundingVolume.y, this.m_originalPosition.y + this.BoundingVolume.y), Mathf.Clamp(this.m_target.z, this.m_originalPosition.z - this.BoundingVolume.z, this.m_originalPosition.z + this.BoundingVolume.z));
		}
	}

	// Token: 0x04000003 RID: 3
	private Vector3 m_previous;

	// Token: 0x04000004 RID: 4
	private Vector3 m_target;

	// Token: 0x04000005 RID: 5
	private Vector3 m_originalPosition;

	// Token: 0x04000006 RID: 6
	public Vector3 BoundingVolume = new Vector3(3f, 1f, 3f);

	// Token: 0x04000007 RID: 7
	public float Speed = 10f;
}
