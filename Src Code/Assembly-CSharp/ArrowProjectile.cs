using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class ArrowProjectile : MonoBehaviour
{
	// Token: 0x06000182 RID: 386 RVA: 0x0000F68C File Offset: 0x0000D88C
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000F694 File Offset: 0x0000D894
	private Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
	{
		if (Mathf.Abs(start.y - end.y) < 0.1f)
		{
			Vector3 a = end - start;
			Vector3 result = start + t * a;
			result.y += Mathf.Sin(t * 3.1415927f) * height;
			return result;
		}
		Vector3 vector = end - start;
		Vector3 rhs = end - new Vector3(start.x, end.y, start.z);
		Vector3 a2 = Vector3.Cross(Vector3.Cross(vector, rhs), vector);
		if (end.y > start.y)
		{
			a2 = -a2;
		}
		return start + t * vector + Mathf.Sin(t * 3.1415927f) * height * a2.normalized;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000F768 File Offset: 0x0000D968
	private void Init()
	{
		if (this.start == Vector3.zero || this.end == Vector3.zero)
		{
			return;
		}
		this.started = false;
		this.pos = base.transform.position;
		this.t = 0f;
		base.transform.position = this.start;
		this.dir = this.end - this.start;
		this.dir = this.dir.normalized;
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000F68C File Offset: 0x0000D88C
	private void OnValidate()
	{
		this.Init();
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000F7F6 File Offset: 0x0000D9F6
	private IEnumerator DisableAfterDelay()
	{
		yield return new WaitForSeconds(this.time - this.t);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000F805 File Offset: 0x0000DA05
	public void Disable()
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine("DisableAfterDelay");
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000F820 File Offset: 0x0000DA20
	private void Update()
	{
		if (this.t < this.time)
		{
			this.t += Time.deltaTime;
			base.transform.position = this.SampleParabola(this.start, this.end, this.trajectoryHeight, this.t / this.time);
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, base.transform.position - this.pos);
			this.pos = base.transform.position;
			return;
		}
		if (this.loop)
		{
			this.Init();
		}
	}

	// Token: 0x04000298 RID: 664
	public Vector3 start = Vector3.zero;

	// Token: 0x04000299 RID: 665
	public Vector3 end = Vector3.zero;

	// Token: 0x0400029A RID: 666
	public GameObject arrowPrefab;

	// Token: 0x0400029B RID: 667
	public float trajectoryHeight = 30f;

	// Token: 0x0400029C RID: 668
	private GameObject arrow;

	// Token: 0x0400029D RID: 669
	public float time;

	// Token: 0x0400029E RID: 670
	private float t;

	// Token: 0x0400029F RID: 671
	public bool started;

	// Token: 0x040002A0 RID: 672
	public bool loop = true;

	// Token: 0x040002A1 RID: 673
	private Vector3 dir = Vector3.zero;

	// Token: 0x040002A2 RID: 674
	private Vector3 pos = Vector3.zero;
}
